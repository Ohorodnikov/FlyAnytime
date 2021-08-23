using System;
using System.Collections.Generic;
using System.Text;

namespace FlyAnytime.Tools
{
    public static class StringExt
    {
        public static bool IsNullOrEmpty(this string s)
        {
            return string.IsNullOrEmpty(s);
        }

        public static bool IsNotNullOrEmpty(this string s)
        {
            return !string.IsNullOrEmpty(s);
        }
    }
}
