using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Correios.NET.Extensions
{
    public static class StringExtensions
    {
        public const char SPACE = ' ';
        public const string HYPHEN = "-";

        public static string RemoveLineEndings(this string text)
        {
            return Regex.Replace(text, @"(\r\n?|\n|\t)", SPACE.ToString()).Trim();
        }

        public static string RemoveHyphens(this string text)
        {
            return text.Replace(HYPHEN, string.Empty).Trim();
        }

        public static string[] SplitSpaces(this string text, int count = 1)
        {
            var separator = new string(SPACE, count);
            return text.Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries);
        }

        public static DateTime ExtractDateTime(this string text, string pattern)
        {
            var regex = new Regex(pattern);
            var match = regex.Match(text);
            if (match.Success)
            {
                var matchValue = $"{match.Groups[1]} {match.Groups[2]}";
                var dateTime = DateTime.ParseExact(matchValue, "dd/MM/yyyy HH:mm", CultureInfo.GetCultureInfo("pt-BR"));
                return dateTime;
            }

            return DateTime.MinValue;
        }
    }
}
