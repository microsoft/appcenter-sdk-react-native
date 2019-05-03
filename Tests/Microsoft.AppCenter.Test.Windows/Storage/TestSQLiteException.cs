// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

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
