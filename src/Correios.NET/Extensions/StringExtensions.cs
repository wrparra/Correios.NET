using System.Text.RegularExpressions;

namespace Correios.NET.Extensions
{
    public static class StringExtensions
    {
        public const string SPACE = " ";
        public const string HYPHEN = "-";

        public static string RemoveLineEndings(this string text)
        {
            return Regex.Replace(text, @"(\r\n?|\n)", SPACE).Trim();
        }

        public static string RemoveHyphens(this string text)
        {
            return text.Replace(HYPHEN, string.Empty).Trim();
        }
    }
}
