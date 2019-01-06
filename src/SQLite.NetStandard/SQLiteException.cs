using System;

namespace SQLite {
    public class SQLiteException : Exception {
        internal SQLiteException(string message, Exception innerException) : base(message, innerException) { }
        internal SQLiteException(string message) : base(message) { }
    }
}