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
