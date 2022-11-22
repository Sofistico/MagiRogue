﻿using System;
using System.Text;
using System.Text.RegularExpressions;

namespace MagiRogue.Utils
{
    public static class StringExtensions
    {
        public static string ToAscii(this string text, int codepage = 437)
        {
            byte[] stringBytes = CodePagesEncodingProvider.Instance.GetEncoding(codepage).GetBytes(text);
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
            return Regex.Replace(text, @"(\p{Lu})(?<=\p{Ll}\1|(\p{Lu}|\p{Ll})\1(?=\p{Ll}))", " $1").Trim();
        }

        public static string FirstLetterUpper(this string input) =>
            input switch
            {
                null => throw new ArgumentNullException(nameof(input)),
                "" => throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input)),
                _ => string.Concat(input[0].ToString().ToUpper(), input.AsSpan(1))
            };
    }
}