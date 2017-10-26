using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace Microsoft.AppCenter.Test
{
    public class TestSqliteException : SQLiteException
    {
        public TestSqliteException() : base(SQLite3.Result.Error, "message")
        {  
        }
    }
}
