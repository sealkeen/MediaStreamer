using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SQLite;

namespace MediaStreamer.DataAccess.RawSQL
{
    public class ReadonlySQLiteContext : ReadonlyDBContext
    {
        public ReadonlySQLiteContext(string filename, Sealkeen.Abstractions.ILogger logger) : base(filename, logger)
        {
            //DbCommand = SQLiteCommand.;
        }
    }
}
