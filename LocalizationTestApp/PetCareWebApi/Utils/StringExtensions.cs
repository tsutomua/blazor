using System;
using System.Collections.Generic;
using System.Linq;

namespace Utils
{
    public static class StringExtensions
    {
        public static List<string> ToStringListFromCommaDelimitedStringEmptyNotAllowed(this string commaDelimitedString)
        {
            return commaDelimitedString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToList();
        }
    }
}