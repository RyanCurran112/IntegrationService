using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace BehrensGroup_ClassLibrary.Functions
{
    public static class StringHelper
    {
        public static string ToTitleCase1(this string str)
        {
            var tokens = str.Split(new[] { " ", "-" }, StringSplitOptions.RemoveEmptyEntries);
            for (var i = 0; i < tokens.Length; i++)
            {
                var token = tokens[i];
                tokens[i] = token == token.ToUpper()
                    ? token
                    : token.Substring(0, 1).ToUpper() + token.Substring(1).ToLower();
            }

            return string.Join(" ", tokens);
        }

        private static readonly TextInfo myTI = new CultureInfo("en-GB", false).TextInfo;

        public static string ToTitleCase(this string str)
        {
            str = myTI.ToLower(str);
            str = myTI.ToTitleCase(str);
            return str;
        }
    }
}
