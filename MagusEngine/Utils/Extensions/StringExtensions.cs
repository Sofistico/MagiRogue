using System;
using System.Text;
using System.Text.RegularExpressions;

namespace MagusEngine.Utils.Extensions
{
    public static class StringExtensions
    {
        public static string ToAscii(this string text, int codepage = 437)
        {
            byte[] stringBytes = CodePagesEncodingProvider.Instance.GetEncoding(codepage)!.GetBytes(text);
            char[] stringChars = new char[stringBytes.Length];

            for (int i = 0; i < stringBytes.Length; i++)
                stringChars[i] = (char)stringBytes[i];

            return new string(stringChars);
        }

        public static string ConvertGlyphs(this string text)
        {
            var result = string.Empty;
            foreach (var symbol in text)
            {
                result += (char)GlyphHelper.GetGlyph(symbol);
            }

            return result;
        }

        public static string SeparateByUpperLetter(this string text)
        {
            return Regex.Replace(text, @"(\p{Lu})(?<=\p{Ll}\1|(\p{Lu}|\p{Ll})\1(?=\p{Ll}))", " $1", RegexOptions.None, TimeSpan.FromSeconds(10)).Trim();
        }

        public static string FirstLetterUpper(this string? input) =>
            input switch
            {
                null => throw new ArgumentNullException(nameof(input)),
                "" => throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input)),
                _ => string.Concat(input[0].ToString().ToUpper(), input.AsSpan(1))
            };

        public static char? GetChar(this string? input)
        {
            if (char.TryParse(input, out char result))
            {
                return result;
            }
            else
            {
                return null;
            }
        }

        public static bool IsNullOrEmpty(this string? str)
        {
            return string.IsNullOrEmpty(str);
        }

        public static bool IsNullOrWhiteSpace(this string? str)
        {
            return string.IsNullOrWhiteSpace(str);
        }
    }
}
