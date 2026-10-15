#!/usr/bin/env bash
# Decides whether test.sh may test the test project's target frameworks at the same time, from the CPU and memory limits
# visible to this process (its cgroup and the cgroups above it as far as its cgroup namespace shows them), not from the
# host's totals.
#
#   parallel-test-resources.sh [ROOT]
#
# Prints one line, "parallel: ..." or "sequential: ...", and exits 0 only for parallel. It says parallel only when it
# has read every required limit it can see, so anything it cannot place, read or parse means sequential, and test.sh
# then tests one framework after another as it does without the opt-in. ROOT (a directory standing for /, for this
# script's own tests) defaults to the real one.
#
# It reads one layout: the cgroup v2 hierarchy alone (/proc/self/cgroup is a single "0::PATH" line), mounted once at
# /sys/fs/cgroup from its root (as Docker mounts it in a container), with this process's cgroup at /sys/fs/cgroup/PATH.
# From that cgroup up to the mount, every cgroup is read:
# - CPUs: those the process's affinity allows (Cpus_allowed_list, which already reflects cpuset), capped by each
#   cgroup's cpu.max quota, rounded down to whole CPUs;
# - memory: what the kernel reports available (MemAvailable), capped by each cgroup's memory.max and memory.high, less
#   its memory.current.
# A cgroup that lists the cpu or memory controller in its cgroup.controllers must show that controller's limit files
# readable (cpu.max; memory.max, memory.high and memory.current), or its limit is unknown and the answer is sequential.
# That holds at the visible root too: in a cgroup namespace the mount's root is the namespace's root, which can be a
# cgroup with limits of its own (https://docs.kernel.org/admin-guide/cgroup-v2.html#namespace), so "/" there does not
# show the hierarchy's real root, whose missing limit files would mean no limit; a real root is sequential as well.
# A cgroup that does not list a controller has no limit of it. Cgroups above the visible root cannot be seen at all, so
# parallel means only that the limits this process can see leave the floors below.
#
# Two test hosts run at once, one per framework, each with its own in-process loopback servers and the child processes
# some tests start; within a host the tests run one at a time. Four CPUs and four GiB are conservative heuristic floors
# for two such hosts, not a share assigned to each and not derived from a measured working set: whether the SDK's tests
# run correctly and faster at the same time is for ordinary CI to show. Below either, the frameworks run one after
# another, which costs only time.
set -u

root="${1:-}"
min_cpus=4
min_bytes=$((4 * 1024 * 1024 * 1024))
mount=/sys/fs/cgroup

sequential() {
  echo "sequential: $*"
  exit 1
}

for file in /proc/self/status /proc/meminfo /proc/self/cgroup /proc/self/mountinfo; do
  [[ -r "${root}${file}" ]] || sequential "cannot read ${file}, so this job's CPU and memory limits are unknown"
done

# CPUs the affinity allows, from a list such as 0-3,6,8-9.
allowed="$(sed -n 's/^Cpus_allowed_list:[[:space:]]*//p' "${root}/proc/self/status")"
[[ "${allowed}" =~ ^[0-9]+(-[0-9]+)?(,[0-9]+(-[0-9]+)?)*$ ]] || sequential "cannot parse Cpus_allowed_list (${allowed:-missing})"
cpus=0
IFS=, read -r -a ranges <<< "${allowed}"
for range in "${ranges[@]}"; do
  cpus=$((cpus + ${range#*-} - ${range%-*} + 1))
done
cpu_source="affinity"

available_kb="$(sed -n 's/^MemAvailable:[[:space:]]*\([0-9][0-9]*\) kB$/\1/p' "${root}/proc/meminfo")"
[[ "${available_kb}" =~ ^[0-9]+$ ]] || sequential "cannot parse MemAvailable in /proc/meminfo"
available=$((available_kb * 1024))
memory_source="MemAvailable"

# This process's cgroup, which must be the cgroup v2 hierarchy alone.
lines="$(wc -l < "${root}/proc/self/cgroup" | tr -d ' ')"
path="$(sed -n 's/^0:://p' "${root}/proc/self/cgroup")"
[[ "${lines}" == 1 && "${path}" == /* ]] ||
  sequential "/proc/self/cgroup is not the cgroup v2 hierarchy alone (a single 0::PATH line), so its limits are not read here"

# The hierarchy, mounted once at /sys/fs/cgroup from its root. mountinfo fields: 4 the mounted root, 5 the mount point;
# after the "-" separator, the file system type.
mounts="$(awk -v at="${mount}" '$5 == at { for (i = 7; i <= NF; i++) if ($i == "-") { print $4, $(i + 1); break } }' "${root}/proc/self/mountinfo")"
[[ "${mounts}" == "/ cgroup2" ]] ||
  sequential "${mount} is not the cgroup v2 hierarchy mounted once from its root (${mounts:-not mounted}), so its limits are not read here"

dir="${root}${mount}${path%/}"
[[ -d "${dir}" && -r "${dir}/cgroup.controllers" ]] ||
  sequential "this process's cgroup ${path} is not readable under ${mount}, so its limits are unknown"

# Reads the limit file NAME of the cgroup in DIR into the variable "value", for CONTROLLER listed in its
# cgroup.controllers: returns 1 when the cgroup does not list CONTROLLER, and says sequential when it lists it but the
# file is missing or unreadable, at the visible root as below it.
read_limit() {
  local dir="$1" controller="$2" name="$3"
  value=""
  [[ " $(< "${dir}/cgroup.controllers") " == *" ${controller} "* ]] || return 1
  [[ -r "${dir}/${name}" ]] || sequential "the ${controller} controller is enabled for ${dir#"${root}"} but its ${name} is not readable"
  value="$(< "${dir}/${name}")"
}

while :; do
  [[ -r "${dir}/cgroup.controllers" ]] || sequential "cannot read ${dir#"${root}"}/cgroup.controllers"
  cgroup="${dir#"${root}${mount}"}"
  cgroup="${cgroup:-/}"
  if read_limit "${dir}" cpu cpu.max; then
    read -r quota period <<< "${value}"
    if [[ "${quota}" != max ]]; then
      [[ "${quota}" =~ ^[0-9]+$ && "${period}" =~ ^[1-9][0-9]*$ ]] || sequential "cannot parse ${dir#"${root}"}/cpu.max (${value})"
      if (( quota / period < cpus )); then
        cpus=$((quota / period))
        cpu_source="cpu.max of cgroup ${cgroup}"
      fi
    fi
  fi
  if read_limit "${dir}" memory memory.max; then
    limit="${value}"
    read_limit "${dir}" memory memory.current
    usage="${value}"
    [[ ( "${limit}" == max || "${limit}" =~ ^[0-9]+$ ) && "${usage}" =~ ^[0-9]+$ ]] ||
      sequential "cannot parse ${dir#"${root}"}/memory.max or memory.current"
    read_limit "${dir}" memory memory.high
    if [[ "${value}" != max ]]; then
      [[ "${value}" =~ ^[0-9]+$ ]] || sequential "cannot parse ${dir#"${root}"}/memory.high (${value})"
      if [[ "${limit}" == max ]] || (( value < limit )); then
        limit="${value}"
      fi
    fi
    if [[ "${limit}" != max ]]; then
      headroom=$((limit > usage ? limit - usage : 0))
      if (( headroom < available )); then
        available=${headroom}
        memory_source="memory limit less usage of cgroup ${cgroup}"
      fi
    fi
  fi
  [[ "${dir}" == "${root}${mount}" ]] && break
  dir="$(dirname "${dir}")"
done

measured="${cpus} CPUs (${cpu_source}), $((available / 1024 / 1024)) MiB available (${memory_source})"
(( cpus >= min_cpus )) || sequential "${measured}; testing frameworks at once needs ${min_cpus} CPUs"
(( available >= min_bytes )) || sequential "${measured}; testing frameworks at once needs $((min_bytes / 1024 / 1024)) MiB"
echo "parallel: ${measured}"
