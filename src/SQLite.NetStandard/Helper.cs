namespace SQLite {
    using static SQLite;
	
    public static class Helper {
        internal static void ThrowIfNeeded(sqlite3 db, int result, int expected, string prefix) {
            if (db != null && result != expected) {
                
                try {
                    var err = sqlite3_errmsg(db);
                    throw new SQLiteException($"{prefix} : {result} ({(Error)result}) : {err ?? "No error message"}");
                }
                catch {
                    throw new SQLiteException($"{prefix} : {result} ({(Error)result}) : No error message");
                }

            }
        }
    }
}