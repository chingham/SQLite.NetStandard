using System;
using System.Diagnostics;
using System.Reflection;

namespace SQLite {
    using static SQLite;
	
    public static class Helper {
        internal static void ThrowIfNeeded(sqlite3 db, int result, int expected, string prefix) {
            if (db != null && result != expected) {
                
                string err = null;
                try {
                    err = sqlite3_errmsg(db);
                }
                catch { }

                throw new SQLiteException($"{prefix} : {result} ({(Error)result}) : {err ?? "No error message"}");
            }
        }
    }
}