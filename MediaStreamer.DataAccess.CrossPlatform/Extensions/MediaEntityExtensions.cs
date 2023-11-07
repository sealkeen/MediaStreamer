using MediaStreamer.Domain;
using Sealkeen.CSCourse2016.JSONParser.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace MediaStreamer.DataAccess.CrossPlatform.Extensions
{
    public static class MediaEntityExtensions
    {
        public static JItem GetMediaEntityIdValue(this MediaEntity entity)
        {
            JItemType type = JItemType.SingleValue;
            Guid toParse = Guid.Empty;
            if (Guid.TryParse(entity.GetId(), out toParse)) {
                type = JItemType.String;
            }
            return JItem.Factory(type, DataBase.Coalesce(entity.GetId()));
        }

        public static JItem GetMediaEntityIdValue(string id)
        {
            JItemType type = JItemType.SingleValue;
            Guid toParse = Guid.Empty;
            if (Guid.TryParse(id, out toParse)) {
                type = JItemType.String;
            }
            return JItem.Factory(type, DataBase.Coalesce(id));
        }
    }
}
