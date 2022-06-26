using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StringExtensions
{
    public static class NullReferenceResolution
    {
        public static string Coalesce<T>(T line)
        {
            if (line == null)
                return "null";
            else
                return line.ToString();
        }
    }
}
