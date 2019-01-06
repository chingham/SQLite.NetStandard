using System;
using System.Diagnostics;
using System.Reflection;

namespace SQLite {
    using static SQLite;
	
    public static class Helper {
        internal static void ThrowIfNeeded(sqlite3 db, int result, int expected, string prefix) {
            if (db != null && result == (int)Error.Error) {
                var err = sqlite3_errmsg(db);
                throw new SQLiteException($"{prefix} : {err}");
            }
            if (result != expected) {
                throw new SQLiteException($"{prefix}: {result}");
            }
        }
    }
}