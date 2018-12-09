﻿using System;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo

namespace SQLite {
    public static class SQLite {
        
        public static void SetProvider(ISQLiteProvider sqliteProvider) {
            provider = sqliteProvider ?? throw new ArgumentNullException(nameof(sqliteProvider));
        }
        static ISQLiteProvider provider;

        #region API
        
        public static Error sqlite3_open(string filename, out sqlite3 db) {
            var result = provider.sqlite3_open(filename, out var handle);
            db = new sqlite3(handle, () => provider.sqlite3_close(handle));
            return (Error)result;
        }
        public static Error sqlite3_open_v2(string filename, out sqlite3 db, OpenFlags flags, string vfs) {
            var result = provider.sqlite3_open_v2(filename, out var handle, (int)flags, vfs);
            db = new sqlite3(handle, () => provider.sqlite3_close_v2(handle));
            return (Error)result;
        }
        public static Error sqlite3_close(sqlite3 db) => (Error)provider.sqlite3_close(db.Handle);
        public static Error sqlite3_close_v2(sqlite3 db) => (Error)provider.sqlite3_close_v2(db.Handle);

        public static string sqlite3_errmsg(sqlite3 db) => provider.sqlite3_errmsg(db.Handle);

        public static Error sqlite3_exec(sqlite3 db, string command) =>
            (Error)provider.sqlite3_exec(db.Handle, command, null, null, out _);
        public static Error sqlite3_prepare_v2(sqlite3 db, string command, out sqlite3_stmt stmt) {
            var result = provider.sqlite3_prepare_v2(db.Handle, command, out var handle, out _);
            stmt = new sqlite3_stmt(handle, () => provider.sqlite3_finalize(handle));
            return (Error)result;
        }
        public static Error sqlite3_finalize(sqlite3_stmt stmt) =>
            (Error)provider.sqlite3_finalize(stmt.Handle);

        public static StepResult sqlite3_step(sqlite3_stmt stmt) => (StepResult)provider.sqlite3_step(stmt.Handle);
        public static long sqlite3_last_insert_rowid(sqlite3 db) => provider.sqlite3_last_insert_rowid(db.Handle);

        public static int sqlite3_column_count(sqlite3_stmt stmt) => provider.sqlite3_column_count(stmt.Handle);
        public static string sqlite3_column_name(sqlite3_stmt stmt, int column) =>
            provider.sqlite3_column_name(stmt.Handle, column);
        public static int sqlite3_column_type(sqlite3_stmt stmt, int column) =>
            provider.sqlite3_column_type(stmt.Handle, column);

        public static Error sqlite3_bind_null(sqlite3_stmt stmt, int index) =>
            (Error)provider.sqlite3_bind_null(stmt.Handle, index);
        public static Error sqlite3_bind_int(sqlite3_stmt stmt, int index, int value) =>
            (Error)provider.sqlite3_bind_int(stmt.Handle, index, value);
        public static Error sqlite3_bind_int64(sqlite3_stmt stmt, int index, long value) =>
            (Error)provider.sqlite3_bind_int64(stmt.Handle, index, value);
        public static Error sqlite3_bind_double(sqlite3_stmt stmt, int index, double value) =>
            (Error)provider.sqlite3_bind_double(stmt.Handle, index, value);
        public static Error sqlite3_bind_blob(sqlite3_stmt stmt, int index, byte[] value) =>
            (Error)provider.sqlite3_bind_blob(stmt.Handle, index, value);
        public static Error sqlite3_bind_text(sqlite3_stmt stmt, int index, string value) =>
            (Error)provider.sqlite3_bind_text(stmt.Handle, index, value);

        public static int sqlite3_column_int(sqlite3_stmt stmt, int index) =>
            provider.sqlite3_column_int(stmt.Handle, index);
        public static long sqlite3_column_int64(sqlite3_stmt stmt, int index) =>
            provider.sqlite3_column_int64(stmt.Handle, index);
        public static double sqlite3_column_double(sqlite3_stmt stmt, int index) =>
            provider.sqlite3_column_double(stmt.Handle, index);
        public static byte[] sqlite3_column_blob(sqlite3_stmt stmt, int index) =>
            provider.sqlite3_column_blob(stmt.Handle, index);
        public static string sqlite3_column_text(sqlite3_stmt stmt, int index) =>
            provider.sqlite3_column_text(stmt.Handle, index);
        
        #endregion
        
    }

    public sealed class sqlite3 : Resource {
        internal sqlite3(IntPtr handle, Action disposing) : base(handle, disposing) { }
    }
    
    public sealed class sqlite3_stmt : Resource {
        internal sqlite3_stmt(IntPtr handle, Action disposing) : base(handle, disposing) { }
    }
}