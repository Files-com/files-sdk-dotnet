using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

[assembly: InternalsVisibleTo("FilesTests")]

namespace FilesCom.Util
{
    public class PathUtil
    {
        private static readonly Dictionary<int, string> ComparisonMap = LoadComparisonMap();

        private static Dictionary<int, string> LoadComparisonMap()
        {
            using (var stream = typeof(PathUtil).Assembly.GetManifestResourceStream("FilesCom.Util.path_comparison.json"))
            using (var document = JsonDocument.Parse(stream))
            {
                var result = new Dictionary<int, string>();
                foreach (var entry in document.RootElement.GetProperty("mapping").EnumerateObject())
                {
                    result.Add(Convert.ToInt32(entry.Name, 16), entry.Value.GetString());
                }
                return result;
            }
        }

        private static string NULL_BYTE = @"\x00{1,}";
        private static string BACKSLASH = @"\\{1,}";
        private static string LEADING_AND_TRAILING_SLASHES = @"(/)*$|^(/)*";
        private static string TWO_OR_MORE_SLASHES = @"(/){2,}";


        /// <summary>
        /// The Files.com path made of <paramref name="args"/>, in the form Files.com compares paths in: joined with /,
        /// without empty, . or .. segments, with letters in lower case and other characters mapped as Files.com maps them.
        /// </summary>
        /// <param name="args">The paths or path segments to join, with / or \ between their segments.</param>
        /// <returns>The path to compare.</returns>
        public static string normalize_for_comparison(params string[] args)
        {
            string path = normalize(args);
            var result = new StringBuilder(path.Length);
            for (int offset = 0; offset < path.Length;)
            {
                int width = char.IsSurrogatePair(path, offset) ? 2 : 1;
                int scalar = width == 2 ? char.ConvertToUtf32(path, offset) : path[offset];
                if (scalar >= ' ' && scalar <= '~')
                {
                    result.Append((char)(scalar >= 'A' && scalar <= 'Z' ? scalar + 'a' - 'A' : scalar));
                }
                else if (ComparisonMap.TryGetValue(scalar, out string replacement))
                {
                    result.Append(replacement);
                }
                else
                {
                    result.Append(path, offset, width);
                }
                offset += width;
            }
            return result.ToString();
        }

        /// <summary>
        /// Whether two Files.com paths name the same file or folder, as Files.com compares paths
        /// (<see cref="normalize_for_comparison"/>).
        /// </summary>
        /// <param name="a">A path.</param>
        /// <param name="b">The path to compare it with.</param>
        /// <returns>True when Files.com takes them for the same path.</returns>
        public static bool same(string a, string b)
        {
            return normalize_for_comparison(a).Equals(normalize_for_comparison(b));
        }

        /// <summary>
        /// A path segment as Files.com takes it: without NUL characters, with \ as /, without leading, trailing or
        /// repeated slashes, and empty when it is . or ..
        /// </summary>
        /// <param name="str">The segment to clean.</param>
        /// <returns>The cleaned segment.</returns>
        public static string cleanpath(string str)
        {
            string newStr = str;
            newStr = Regex.Replace(newStr, NULL_BYTE, "");
            newStr = Regex.Replace(newStr, BACKSLASH, "/");
            newStr = Regex.Replace(newStr, LEADING_AND_TRAILING_SLASHES, "");
            newStr = Regex.Replace(newStr, TWO_OR_MORE_SLASHES, "/");

            if (newStr.Equals(".") || newStr.Equals(".."))
            {
                return "";
            }


            return newStr;
        }

        internal static string normalize(params string[] args)
        {
            List<string> all_paths = new List<string>();
            foreach (string arg in args)
            {
                string[] paths = Regex.Replace(arg, BACKSLASH, "/").Split('/');
                foreach (string path in paths)
                {
                    string clnPath = cleanpath(Regex.Replace(path, NULL_BYTE, ""));
                    if (clnPath != null && clnPath.Length > 0)
                    {
                        all_paths.Add(clnPath);
                    }
                }
            }

            return string.Join("/", all_paths);
        }

    }
}