using System;
using System.Collections.Generic;
using System.Text;

namespace LinqExtensions
{
    public static class StringExtensions
    {
        public static bool Equals(string path1, string path2)
        {
            return new Uri(path1) == new Uri(path2);
        }
    }
}
