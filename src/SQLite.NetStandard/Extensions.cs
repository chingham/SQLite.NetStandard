using System;
using System.Threading.Tasks;

namespace SQLite {
    public static class Extensions {
		
        public static sqlite3_stmt Prepare(this sqlite3 db, Query query) {
            sqlite3_stmt stmt;
            Error result;
            try {
                result = SQLite.sqlite3_prepare_v2(db, query.Command, out stmt);
            }
            catch (Exception ex) {
                throw new SQLiteException("An unknown error occured: " + ex.Message, ex);
            }
            Helper.ThrowIfNeeded(db, (int)result, (int)Error.OK, $"Unable to prepare sqlite statement\n{query}\n");

            return stmt;
        }
        public static void Finalize(this sqlite3_stmt stmt) {
            Error result;
            try {
                result = SQLite.sqlite3_finalize(stmt);
            }
            catch (Exception ex) {
                throw new SQLiteException("An unknown error occured: " + ex.Message, ex);
            }
            Helper.ThrowIfNeeded(null, (int)result, (int)Error.OK, "Unable to finalize sqlite statement");

            GC.SuppressFinalize(stmt);
        }

        public static void BindParameter(this sqlite3_stmt stmt, object parameter, int index) {
            try {
                if (parameter == null) {
                    SQLite.sqlite3_bind_null(stmt, index);
                    return;
                }

                var t = parameter.GetType();
                if (t == typeof(int))
                    SQLite.sqlite3_bind_int(stmt, index, (int)parameter);
                else if (t == typeof(uint))
                    SQLite.sqlite3_bind_int64(stmt, index, (uint)parameter);

                else if (t == typeof(short))
                    SQLite.sqlite3_bind_int(stmt, index, (short)parameter);
                else if (t == typeof(ushort))
                    SQLite.sqlite3_bind_int(stmt, index, (ushort)parameter);

                else if (t == typeof(double))
                    SQLite.sqlite3_bind_double(stmt, index, (double)parameter);
                else if (t == typeof(float))
                    SQLite.sqlite3_bind_double(stmt, index, (float)parameter);

                else if (t == typeof(long))
                    SQLite.sqlite3_bind_int64(stmt, index, (long)parameter);

                else if (t == typeof(byte[]))
                    SQLite.sqlite3_bind_blob(stmt, index, (byte[])parameter);

                else if (t == typeof(string))
                    SQLite.sqlite3_bind_text(stmt, index, (string)parameter);

                else
                    throw new NotSupportedException();
            }
            catch (NotSupportedException) { throw; }
            catch (Exception ex) {
                throw new SQLiteException("An unknown error occured: " + ex.Message, ex);
            }
        }
        public static void ClearBindings(this sqlite3_stmt stmt, sqlite3 db) {
            var result = SQLite.sqlite3_clear_bindings(stmt);
            if (result != Error.OK) {
                var err = SQLite.sqlite3_errmsg(db);
                throw new SQLiteException(err);
            }
        }
        public static void Reset(this sqlite3_stmt stmt, sqlite3 db) {
            var result = SQLite.sqlite3_reset(stmt);
            if (result != Error.OK) {
                var err = SQLite.sqlite3_errmsg(db);
                throw new SQLiteException(err);
            }
        }
        public static bool Step(this sqlite3_stmt stmt, sqlite3 db) {
            var result = SQLite.sqlite3_step(stmt);
            if (result != StepResult.Row && result != StepResult.Done) {
                var err = SQLite.sqlite3_errmsg(db);
                throw new SQLiteException(err);
            }

            return result == StepResult.Done;
        }

        public static byte[] GetBytes(this sqlite3_stmt stmt, int index) {
            try {
                return SQLite.sqlite3_column_blob(stmt, index);
            }
            catch (Exception ex) {
                throw new SQLiteException("An unknown error occured: " + ex.Message, ex);
            }
        }
        public static int GetInt(this sqlite3_stmt stmt, int index) {
            try {
                return SQLite.sqlite3_column_int(stmt, index);
            }
            catch (Exception ex) {
                throw new SQLiteException("An unknown error occured: " + ex.Message, ex);
            }
        }
        public static double GetDouble(this sqlite3_stmt stmt, int index) {
            try {
                return SQLite.sqlite3_column_double(stmt, index);
            }
            catch (Exception ex) {
                throw new SQLiteException("An unknown error occured: " + ex.Message, ex);
            }
        }
        public static string GetString(this sqlite3_stmt stmt, int index) {
            try {
                return SQLite.sqlite3_column_text(stmt, index);
            }
            catch (Exception ex) {
                throw new SQLiteException("An unknown error occured: " + ex.Message, ex);
            }
        }
        public static ColumnType GetDataType(this sqlite3_stmt stmt, int index) {
            try {
                return SQLite.sqlite3_column_type(stmt, index);
            }
            catch (Exception ex) {
                throw new SQLiteException("An unknown error occured: " + ex.Message, ex);
            }
        }
		
        #region Simplified API

        public static Task QueryReaderAsync(this sqlite3 db, Query query, Action<RowData> onRow)
            => Task.Run(() => QueryReader(db, query, onRow));
        public static void QueryReader(this sqlite3 db, Query query, Action<RowData> onRow) {
            var stmt = db.Prepare(query);
            try { 
                for (var i = 0; i < query.Parameters.Count; i++)
                    stmt.BindParameter(query.Parameters[i], i + 1);

                RowData row = null;
                while (true) {
                    var stepResult = SQLite.sqlite3_step(stmt);
                    if (stepResult != StepResult.Row)
                        break;
                    if (row == null)
                        row = new RowData(stmt);
                    else
                        row.ClearForNewRow();
                    onRow?.Invoke(row);
                }
            }
            finally {
                stmt.Finalize();
            }
        }
        
        public static Task<RowData> QuerySingleAsync(this sqlite3 db, Query query) 
            => Task.Run(() => QuerySingle(db, query));
        public static RowData QuerySingle(this sqlite3 db, Query query) {
            var stmt = db.Prepare(query);
            try {
                for (var i = 0; i < query.Parameters.Count; i++)
                    stmt.BindParameter(query.Parameters[i], i + 1);

                var result = SQLite.sqlite3_step(stmt);
                if (result == StepResult.Row)
                    return new RowData(stmt, true);
                if (result == StepResult.Done)
                    return null;
            
                var err = SQLite.sqlite3_errmsg(db);
                throw new SQLiteException(err);
            }
            finally {
                stmt.Finalize();
            }
        }

        public static Task ExecuteAsync(this sqlite3 db, Query query) 
            => Task.Run(() => Execute(db, query));
        public static void Execute(this sqlite3 db, Query query) {
            if (query.Parameters == null || query.Parameters.Count == 0) {
                //Parameter less execute
                var result = SQLite.sqlite3_exec(db, query.Command);
                if (result != Error.OK) {
                    var err = SQLite.sqlite3_errmsg(db);
                    throw new SQLiteException(err);
                }
            }
            else {
                //With parameters to bind
                var stmt = db.Prepare(query);
                try {
                    for (var i = 0; i < query.Parameters.Count; i++)
                        stmt.BindParameter(query.Parameters[i], i + 1);

                    var result = SQLite.sqlite3_step(stmt);
                    if (result != StepResult.Done && result != StepResult.Row) {
                        var err = SQLite.sqlite3_errmsg(db);
                        throw new SQLiteException(err);
                    }
                }
                finally {
                    stmt.Finalize();
                }
            }
        }

        public static Task<bool> TableColumnExistsAsync(this sqlite3 db, string table, string column) 
            => Task.Run(() => TableColumnExistsAsync(db, table, column));
        public static bool TableColumnExists(this sqlite3 db, string table, string column) {
            var exists = false;
            var query = $"PRAGMA table_info('{table}')";
            db.QueryReader(query, row => {
                var n = row.GetString("name");
                if (n == column)
                    exists = true;
            });
            return exists;
        }
        
        public static Task CreateIndexIfNotExistsAsync(this sqlite3 db, string table, string column)
            => Task.Run(() => CreateIndexIfNotExists(db, table, column));
        public static void CreateIndexIfNotExists(this sqlite3 db, string table, string column) {
            var exists = TableColumnExists(db, table, column);
            if (exists)
                return;

            var query = $"CREATE INDEX IF NOT EXISTS {table}_{column} ON {table}({column})";
            db.Execute(query);
        }
        
        public static Task AddColumnIfNotExistsAsync(this sqlite3 db, string table, string column, string type) 
            => Task.Run(() => AddColumnIfNotExists(db, table, column, type));
        public static void AddColumnIfNotExists(this sqlite3 db, string table, string column, string type) {
            var exists = TableColumnExists(db, table, column);
            if (exists)
                return;

            var query = $"ALTER TABLE {table} ADD COLUMN {column} {type}";
            var stmt = db.Prepare(query);
            try {
                var result = SQLite.sqlite3_step(stmt);
                if (result != StepResult.Done) {
                    var err = SQLite.sqlite3_errmsg(db);
                    throw new SQLiteException("Cannot add column: " + err);
                }
            }
            finally {
                stmt?.Finalize();
            }
        }

        public static Guid? GetGuid(this RowData row, string name) {
            var str = row.GetString(name);
            if (str == null || str.Length != 36)
                return null;
            return Guid.TryParse(str, out var id) ? id : (Guid?)null;
        }
        public static Guid? GetGuid(this RowData row, int index) {
            var str = row.GetString(index);
            if (str == null || str.Length != 36)
                return null;
            return Guid.TryParse(str, out var id) ? id : (Guid?)null;
        }

        #endregion
        
        //Helpers
        
        public static string ProtectForSql(this string str) {
            if (str == null)
                return "NULL";
            return $"'{str.Replace("'", "''")}'";
        }
    }
}