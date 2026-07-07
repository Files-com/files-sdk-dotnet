using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

[assembly: InternalsVisibleTo("FilesTests")]

namespace FilesCom.Util
{
    public class PathUtil
    {
        private static Dictionary<char, string> TRANSLIT_MAP = new Dictionary<char, string>(){
            {'À', "A"},
            {'Á', "A"},
            {'Â', "A"},
            {'Ã', "A"},
            {'Ä', "A"},
            {'Å', "A"},
            {'Æ', "AE"},
            {'Ç', "C"},
            {'È', "E"},
            {'É', "E"},
            {'Ê', "E"},
            {'Ë', "E"},
            {'Ì', "I"},
            {'Í', "I"},
            {'Î', "I"},
            {'Ï', "I"},
            {'Ð', "D"},
            {'Ñ', "N"},
            {'Ò', "O"},
            {'Ó', "O"},
            {'Ô', "O"},
            {'Õ', "O"},
            {'Ö', "O"},
            {'Ø', "O"},
            {'Ù', "U"},
            {'Ú', "U"},
            {'Û', "U"},
            {'Ü', "U"},
            {'Ý', "Y"},
            {'ß', "ss"},
            {'à', "a"},
            {'á', "a"},
            {'â', "a"},
            {'ã', "a"},
            {'ä', "a"},
            {'å', "a"},
            {'æ', "ae"},
            {'ç', "c"},
            {'è', "e"},
            {'é', "e"},
            {'ê', "e"},
            {'ë', "e"},
            {'ì', "i"},
            {'í', "i"},
            {'î', "i"},
            {'ï', "i"},
            {'ð', "d"},
            {'ñ', "n"},
            {'ò', "o"},
            {'ó', "o"},
            {'ô', "o"},
            {'õ', "o"},
            {'ö', "o"},
            {'ø', "o"},
            {'ù', "u"},
            {'ú', "u"},
            {'û', "u"},
            {'ü', "u"},
            {'ý', "y"},
            {'ÿ', "y"},
            {'Ā', "A"},
            {'ā', "a"},
            {'Ă', "A"},
            {'ă', "a"},
            {'Ą', "A"},
            {'ą', "a"},
            {'Ć', "C"},
            {'ć', "c"},
            {'Ĉ', "C"},
            {'ĉ', "c"},
            {'Ċ', "C"},
            {'ċ', "c"},
            {'Č', "C"},
            {'č', "c"},
            {'Ď', "D"},
            {'ď', "d"},
            {'Đ', "D"},
            {'đ', "d"},
            {'Ē', "E"},
            {'ē', "e"},
            {'Ĕ', "E"},
            {'ĕ', "e"},
            {'Ė', "E"},
            {'ė', "e"},
            {'Ę', "E"},
            {'ę', "e"},
            {'Ě', "E"},
            {'ě', "e"},
            {'Ĝ', "G"},
            {'ĝ', "g"},
            {'Ğ', "G"},
            {'ğ', "g"},
            {'Ġ', "G"},
            {'ġ', "g"},
            {'Ģ', "G"},
            {'ģ', "g"},
            {'Ĥ', "H"},
            {'ĥ', "h"},
            {'Ħ', "H"},
            {'ħ', "h"},
            {'Ĩ', "I"},
            {'ĩ', "i"},
            {'Ī', "I"},
            {'ī', "i"},
            {'Ĭ', "I"},
            {'ĭ', "i"},
            {'Į', "I"},
            {'į', "i"},
            {'İ', "I"},
            {'Ĳ', "IJ"},
            {'ĳ', "ij"},
            {'Ĵ', "J"},
            {'ĵ', "j"},
            {'Ķ', "K"},
            {'ķ', "k"},
            {'Ĺ', "L"},
            {'ĺ', "l"},
            {'Ļ', "L"},
            {'ļ', "l"},
            {'Ľ', "L"},
            {'ľ', "l"},
            {'Ł', "L"},
            {'ł', "l"},
            {'Ń', "N"},
            {'ń', "n"},
            {'Ņ', "N"},
            {'ņ', "n"},
            {'Ň', "N"},
            {'ň', "n"},
            {'ŉ', "'n"},
            {'Ō', "O"},
            {'ō', "o"},
            {'Ŏ', "O"},
            {'ŏ', "o"},
            {'Ő', "O"},
            {'ő', "o"},
            {'Œ', "OE"},
            {'œ', "oe"},
            {'Ŕ', "R"},
            {'ŕ', "r"},
            {'Ŗ', "R"},
            {'ŗ', "r"},
            {'Ř', "R"},
            {'ř', "r"},
            {'Ś', "S"},
            {'ś', "s"},
            {'Ŝ', "S"},
            {'ŝ', "s"},
            {'Ş', "S"},
            {'ş', "s"},
            {'Š', "S"},
            {'š', "s"},
            {'Ţ', "T"},
            {'ţ', "t"},
            {'Ť', "T"},
            {'ť', "t"},
            {'Ũ', "U"},
            {'ũ', "u"},
            {'Ū', "U"},
            {'ū', "u"},
            {'Ŭ', "U"},
            {'ŭ', "u"},
            {'Ů', "U"},
            {'ů', "u"},
            {'Ű', "U"},
            {'ű', "u"},
            {'Ų', "U"},
            {'ų', "u"},
            {'Ŵ', "W"},
            {'ŵ', "w"},
            {'Ŷ', "Y"},
            {'ŷ', "y"},
            {'Ÿ', "Y"},
            {'Ź', "Z"},
            {'ź', "z"},
            {'Ż', "Z"},
            {'ż', "z"},
            {'Ž', "Z"},
            {'ž', "z"},
        };

        private static string NULL_BYTE = @"\x00{1,}";
        private static string BACKSLASH = @"\\{1,}";
        private static string LEADING_AND_TRAILING_SLASHES = @"(/)*$|^(/)*";
        private static string TWO_OR_MORE_SLASHES = @"(/){2,}";


        public static string normalize_for_comparison(params string[] args)
        {
            return unicode_normalize_and_transliterate(normalize(args)).ToLower().TrimEnd();
        }

        public static bool same(string a, string b)
        {
            return normalize_for_comparison(a).Equals(normalize_for_comparison(b));
        }

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

        private static string u8(string str)
        {
            return Encoding.UTF8.GetString(Encoding.Default.GetBytes(str));
        }

        private static string unicode_normalize_and_transliterate(string str)
        {
            string strNorm = cleanpath(str).Normalize(NormalizationForm.FormKC);
            List<string> joiner = new List<string>();
            foreach (string subStr in strNorm.Split('/'))
            {
                if (!subStr.Equals(".") && !subStr.Equals(".."))
                {
                    joiner.Add(subStr);
                }
            }
            var newStr = string.Join("/", joiner);
            foreach (char c in newStr)
            {
                if (TRANSLIT_MAP.ContainsKey(c))
                {
                    newStr = newStr.Replace(Char.ToString(c), TRANSLIT_MAP[c]);
                }
            }
            return newStr;
        }
    }
}