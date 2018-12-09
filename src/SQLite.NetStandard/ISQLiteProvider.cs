using System;

// ReSharper disable UnusedParameter.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo

namespace SQLite {
    
    public interface ISQLiteProvider {
        int sqlite3_open(string filename, out IntPtr db);
        int sqlite3_open_v2(string filename, out IntPtr db, int flags, string vfs);
        int sqlite3_close(IntPtr db);
        int sqlite3_close_v2(IntPtr db);
        long sqlite3_last_insert_rowid(IntPtr db);
        string sqlite3_errmsg(IntPtr db);
        int sqlite3_prepare_v2(IntPtr db, string sql, out IntPtr stmt, out string remain);
        int sqlite3_step(IntPtr stmt);
        int sqlite3_finalize(IntPtr stmt);
        int sqlite3_bind_blob(IntPtr stmt, int index, byte[] blob);
        int sqlite3_bind_double(IntPtr stmt, int index, double val);
        int sqlite3_bind_int(IntPtr stmt, int index, int val);
        int sqlite3_bind_int64(IntPtr stmt, int index, long val);
        int sqlite3_bind_null(IntPtr stmt, int index);
        int sqlite3_bind_text(IntPtr stmt, int index, string text);
        string sqlite3_column_name(IntPtr stmt, int index);
        string sqlite3_column_text(IntPtr stmt, int index);
        int sqlite3_column_count(IntPtr stmt);
        double sqlite3_column_double(IntPtr stmt, int index);
        int sqlite3_column_int(IntPtr stmt, int index);
        long sqlite3_column_int64(IntPtr stmt, int index);
        byte[] sqlite3_column_blob(IntPtr stmt, int index);
        int sqlite3_column_type(IntPtr stmt, int index);
        int sqlite3_exec(IntPtr db, string sql, ExecCallback callback, object userData, out string errMsg);
    }
}