using Sealkeen.CSCourse2016.JSONParser.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace MediaStreamer.DataAccess.CrossPlatform
{
    public class CrossTable
    {
        public static void MapNavigation<TSource, TTarget>(TSource sourceTable, TTarget targetTable, string sourceProperty)
        {
            if (sourceTable == null || targetTable == null || string.IsNullOrEmpty(sourceProperty))
            {
                return;
            }
        }
    }
}
