#!/usr/bin/env bash
# Tests parallel-test-resources.sh against made-up /proc and /sys/fs/cgroup trees: it must say parallel only when it has
# read every required CPU and memory limit visible to this process and they leave enough, and say sequential, with the
# reason, for every layout, missing cgroup, missing or unreadable limit and parse error it cannot account for.
#   ./parallel-test-resources-test.sh   (exit 0 when every case passes)

set -euo pipefail

guard="$(cd "$(dirname "$0")" && pwd)/parallel-test-resources.sh"
work="$(mktemp -d "${TMPDIR:-/tmp}/parallel-test-resources-test.XXXXXX")"
trap 'rm -rf -- "${work}"' EXIT
gib=$((1024 * 1024 * 1024))
failures=0

# A tree for one case: this process's affinity, MemAvailable in GiB and cgroup PATH, mounted as Docker mounts cgroup v2.
tree() {
  local name="$1" cpus="$2" available_gib="$3" path="$4"
  root="${work}/${name}"
  mkdir -p "${root}/proc/self" "${root}/sys/fs/cgroup${path%/}"
  printf 'Name:\tdotnet\nCpus_allowed_list:\t%s\n' "${cpus}" > "${root}/proc/self/status"
  printf 'MemTotal:       67108864 kB\nMemAvailable:   %s kB\n' "$((available_gib * 1024 * 1024))" > "${root}/proc/meminfo"
  echo "0::${path}" > "${root}/proc/self/cgroup"
  echo "685 684 0:30 / /sys/fs/cgroup ro,nosuid,nodev,noexec,relatime - cgroup2 cgroup rw,nsdelegate" > "${root}/proc/self/mountinfo"
}
# Writes the cgroup at PATH: its controllers, then FILE=CONTENT pairs.
cgroup() {
  local path="$1" controllers="$2"
  shift 2
  local dir="${root}/sys/fs/cgroup${path%/}" pair
  mkdir -p "${dir}"
  echo "${controllers}" > "${dir}/cgroup.controllers"
  for pair in "$@"; do printf '%s\n' "${pair#*=}" > "${dir}/${pair%%=*}"; done
}
unlimited=(cpu.max="max 100000" memory.max=max memory.high=max memory.current=$((100 * 1024 * 1024)))
expect() {
  local description="$1" want_status="$2" want_text="$3" output status=0
  output="$("${guard}" "${root}")" || status=$?
  if [[ ${status} -eq ${want_status} && "${output}" == *"${want_text}"* ]]; then
    echo "ok - ${description}"
  else
    echo "not ok - ${description}: exit ${status}, ${output}"
    failures=$((failures + 1))
  fi
}

# Enough of everything, read at every level.
tree unlimited 0-7 16 /
cgroup / "cpuset cpu io memory pids" "${unlimited[@]}"
expect "parallel with 8 CPUs and 16 GiB and no limit in the container's cgroup" 0 "parallel: 8 CPUs (affinity), 16384 MiB available (MemAvailable)"

tree quota-rounds-down 0-15 16 /
cgroup / "cpu memory" cpu.max="450000 100000" memory.max=max memory.high=max memory.current=0
expect "a 4.5-CPU quota counts as 4 CPUs, enough" 0 "parallel: 4 CPUs (cpu.max of cgroup /)"

# Each limit, alone, below what two test hosts need.
tree cpu-quota 0-15 16 /
cgroup / "cpu memory" cpu.max="200000 100000" memory.max=max memory.high=max memory.current=0
expect "a 2-CPU quota is sequential" 1 "sequential: 2 CPUs (cpu.max of cgroup /)"

tree affinity 0-2,7 16 /
cgroup / "cpu memory" "${unlimited[@]}"
expect "an affinity of 4 CPUs is enough, of fewer is not" 0 "parallel: 4 CPUs (affinity)"
tree affinity-low 0-2 16 /
cgroup / "cpu memory" "${unlimited[@]}"
expect "an affinity of 3 CPUs is sequential" 1 "sequential: 3 CPUs (affinity)"

tree memory-max 0-7 16 /
cgroup / "cpu memory" cpu.max="max 100000" memory.max=$((3 * gib)) memory.high=max memory.current=0
expect "a 3 GiB memory.max is sequential" 1 "3072 MiB available (memory limit less usage of cgroup /)"

tree memory-usage 0-7 16 /
cgroup / "cpu memory" cpu.max="max 100000" memory.max=$((6 * gib)) memory.high=max memory.current=$((3 * gib))
expect "6 GiB memory.max with 3 GiB in use is sequential" 1 "3072 MiB available (memory limit less usage of cgroup /)"

tree memory-high 0-7 16 /
cgroup / "cpu memory" cpu.max="max 100000" memory.max=max memory.high=$((2 * gib)) memory.current=0
expect "a 2 GiB memory.high is sequential" 1 "2048 MiB available (memory limit less usage of cgroup /)"

tree host-memory 0-7 2 /
cgroup / "cpu memory" "${unlimited[@]}"
expect "2 GiB MemAvailable with no cgroup limit is sequential" 1 "2048 MiB available (MemAvailable)"

# Visible ancestors' limits apply to the process's cgroup below them.
tree ancestor-cpu 0-15 16 /runner/job
cgroup / "cpu memory" "${unlimited[@]}"
cgroup /runner "cpu memory" cpu.max="200000 100000" memory.max=max memory.high=max memory.current=0
cgroup /runner/job "cpu memory" "${unlimited[@]}"
expect "a 2-CPU quota on a visible ancestor is sequential" 1 "sequential: 2 CPUs (cpu.max of cgroup /runner)"

tree ancestor-memory 0-15 16 /runner/job
cgroup / "cpu memory" "${unlimited[@]}"
cgroup /runner "cpu memory" cpu.max="max 100000" memory.max=$((3 * gib)) memory.high=max memory.current=$((gib / 2))
cgroup /runner/job "cpu memory" "${unlimited[@]}"
expect "a 3 GiB memory.max on a visible ancestor is sequential" 1 "2560 MiB available (memory limit less usage of cgroup /runner)"

# A cgroup without the cpu and memory controllers has no such limits; those above it still count.
tree disabled-controllers 0-7 16 /user.slice/job
cgroup / "cpuset cpu io memory pids" "${unlimited[@]}"
cgroup /user.slice "cpu memory pids" "${unlimited[@]}"
cgroup /user.slice/job "pids"
expect "a cgroup without the cpu and memory controllers is no limit of them" 0 "parallel: 8 CPUs"

# The visible root lists the controllers without their limit files, as the hierarchy's real root does. In a cgroup
# namespace the visible root can be a cgroup with limits, so this is unknown, not no limit.
tree visible-root-no-files 0-7 16 /user.slice/job
cgroup / "cpuset cpu io memory pids"
cgroup /user.slice "cpu memory pids" "${unlimited[@]}"
cgroup /user.slice/job "pids"
expect "a visible root listing the controllers without their limit files is sequential" 1 "the cpu controller is enabled for /sys/fs/cgroup but its cpu.max is not readable"

# The visible root of a cgroup namespace (0::/) shows its memory limits but not its CPU quota.
tree visible-root-no-cpu-max 0-7 16 /
cgroup / "cpu memory" memory.max=$((16 * gib)) memory.high=max memory.current=0
expect "a visible root with the cpu controller enabled but no cpu.max is sequential" 1 "the cpu controller is enabled for /sys/fs/cgroup but its cpu.max is not readable"

# The process's cgroup shows memory.max and memory.current but not memory.high.
tree no-memory-high 0-7 16 /job
cgroup / "cpu memory" cpu.max="max 100000" memory.max=$((16 * gib)) memory.high=max memory.current=0
cgroup /job "cpu memory" cpu.max="max 100000" memory.max=$((16 * gib)) memory.current=0
expect "memory.max and memory.current without memory.high is sequential" 1 "the memory controller is enabled for /sys/fs/cgroup/job but its memory.high is not readable"

# The process's own cgroup must be there and readable.
tree missing-leaf 0-15 32 /
rmdir "${root}/sys/fs/cgroup"
mkdir -p "${root}/sys/fs/cgroup"
echo "0::/job" > "${root}/proc/self/cgroup"
expect "a process cgroup missing from the mount is sequential" 1 "sequential: this process's cgroup /job is not readable"

tree missing-mountinfo 0-15 32 /
rm "${root}/proc/self/mountinfo"
echo "0::/job" > "${root}/proc/self/cgroup"
expect "no mountinfo is sequential" 1 "sequential: cannot read /proc/self/mountinfo"

tree no-proc 0-15 32 /
rm -r "${root}/proc"
expect "no /proc is sequential" 1 "sequential: cannot read /proc/self/status"

# Hierarchies and mounts other than the one this reads.
tree cgroup-v1 0-15 32 /
printf '12:memory:/docker/abc\n4:cpu,cpuacct:/docker/abc\n1:name=systemd:/docker/abc\n' > "${root}/proc/self/cgroup"
expect "cgroup v1 is sequential" 1 "is not the cgroup v2 hierarchy alone"

tree hybrid 0-15 32 /
printf '12:memory:/docker/abc\n0::/\n' > "${root}/proc/self/cgroup"
cgroup / "pids"
expect "cgroup v1 beside v2 (hybrid) is sequential" 1 "is not the cgroup v2 hierarchy alone"

tree elsewhere 0-15 32 /
echo "685 684 0:30 / /sys/fs/cgroup/unified rw - cgroup2 cgroup2 rw" > "${root}/proc/self/mountinfo"
cgroup / "cpu memory" "${unlimited[@]}"
expect "cgroup v2 mounted somewhere else is sequential" 1 "is not the cgroup v2 hierarchy mounted once from its root (not mounted)"

tree subtree 0-15 32 /
echo "685 684 0:30 /docker/abc /sys/fs/cgroup rw - cgroup2 cgroup2 rw" > "${root}/proc/self/mountinfo"
cgroup / "cpu memory" "${unlimited[@]}"
expect "cgroup v2 mounted from below its root is sequential" 1 "(/docker/abc cgroup2)"

tree tmpfs 0-15 32 /
echo "30 25 0:26 / /sys/fs/cgroup ro - tmpfs tmpfs ro" > "${root}/proc/self/mountinfo"
cgroup / "cpu memory" "${unlimited[@]}"
expect "a tmpfs at /sys/fs/cgroup is sequential" 1 "(/ tmpfs)"

# Limits that apply but cannot be read or parsed.
tree listed-no-file 0-15 32 /job
cgroup / "cpu memory" "${unlimited[@]}"
cgroup /job "cpu memory" memory.max=max memory.high=max memory.current=0
expect "the cpu controller enabled below the mount without cpu.max is sequential" 1 "its cpu.max is not readable"

tree unreadable 0-15 32 /job
cgroup / "cpu memory" "${unlimited[@]}"
cgroup /job "cpu memory" memory.max=max memory.high=max memory.current=0
ln -s missing "${root}/sys/fs/cgroup/job/cpu.max"
expect "an unreadable cpu.max is sequential" 1 "its cpu.max is not readable"

tree bad-cpu 0-15 32 /
cgroup / "cpu memory" cpu.max="abc 100000" memory.max=max memory.high=max memory.current=0
expect "an unparsable cpu.max is sequential" 1 "cannot parse /sys/fs/cgroup/cpu.max"

tree no-current 0-15 32 /job
cgroup / "cpu memory" "${unlimited[@]}"
cgroup /job "cpu memory" cpu.max="max 100000" memory.max=$((8 * gib)) memory.high=max
expect "memory.max without memory.current is sequential" 1 "its memory.current is not readable"

tree bad-affinity 0-15 32 /
sed -i.bak 's/^Cpus_allowed_list:.*/Cpus_allowed_list:\tff/' "${root}/proc/self/status"
cgroup / "cpu memory" "${unlimited[@]}"
expect "an unparsable Cpus_allowed_list is sequential" 1 "cannot parse Cpus_allowed_list (ff)"

if [[ ${failures} -ne 0 ]]; then
  echo "${failures} case(s) failed" >&2
  exit 1
fi
