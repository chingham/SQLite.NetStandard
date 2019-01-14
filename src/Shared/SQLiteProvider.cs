using System;
using System.Runtime.InteropServices;
// ReSharper disable IdentifierTypo

namespace SQLite {
    public class SQLiteProvider : ISQLiteProvider {
        
        #region PInvoke

        const string SqliteDll = "sqlite3";
        
        [DllImport(SqliteDll, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        static extern void sqlite3_free(IntPtr ptr);

        [DllImport(SqliteDll, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        static extern int sqlite3_open(byte[] filename, out IntPtr db);

        [DllImport(SqliteDll, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        static extern int sqlite3_open_v2(byte[] filename, out IntPtr db, int flags, byte[] vfs);

        [DllImport(SqliteDll, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        static extern int sqlite3_close(IntPtr db);

        [DllImport(SqliteDll, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        static extern int sqlite3_close_v2(IntPtr db);

        [DllImport(SqliteDll, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        static extern long sqlite3_last_insert_rowid(IntPtr db);

        [DllImport(SqliteDll, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr sqlite3_errmsg(IntPtr db);

        [DllImport(SqliteDll, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        static extern int sqlite3_prepare_v2(IntPtr db, IntPtr sql, int count, out IntPtr stmt, out IntPtr remain);

        [DllImport(SqliteDll, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        static extern int sqlite3_step(IntPtr stmt);

        [DllImport(SqliteDll, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        static extern int sqlite3_finalize(IntPtr stmt);

        [DllImport(SqliteDll, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        static extern int sqlite3_bind_blob(IntPtr stmt, int index, IntPtr blob, int size, IntPtr transient);

        [DllImport(SqliteDll, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        static extern int sqlite3_bind_double(IntPtr stmt, int index, double val);

        [DllImport(SqliteDll, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        static extern int sqlite3_bind_int(IntPtr stmt, int index, int val);

        [DllImport(SqliteDll, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        static extern int sqlite3_bind_int64(IntPtr stmt, int index, long val);

        [DllImport(SqliteDll, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        static extern int sqlite3_bind_null(IntPtr stmt, int index);

        [DllImport(SqliteDll, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        static extern int sqlite3_bind_text(IntPtr stmt, int index, byte[] text, int len, IntPtr reserved);

        [DllImport(SqliteDll, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr sqlite3_column_name(IntPtr stmt, int index);

        [DllImport(SqliteDll, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr sqlite3_column_text(IntPtr stmt, int index);

        [DllImport(SqliteDll, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        static extern int sqlite3_column_count(IntPtr stmt);

        [DllImport(SqliteDll, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        static extern double sqlite3_column_double(IntPtr stmt, int index);

        [DllImport(SqliteDll, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        static extern int sqlite3_column_int(IntPtr stmt, int index);

        [DllImport(SqliteDll, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        static extern long sqlite3_column_int64(IntPtr stmt, int index);

        [DllImport(SqliteDll, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr sqlite3_column_blob(IntPtr stmt, int index);

        [DllImport(SqliteDll, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        static extern int sqlite3_column_bytes(IntPtr stmt, int index);

        [DllImport(SqliteDll, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        static extern int sqlite3_column_type(IntPtr stmt, int index);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate int CallbackExec(IntPtr db, int n, IntPtr values, IntPtr names);

        [DllImport(SqliteDll, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        static extern int sqlite3_exec(IntPtr db, byte[] sql, CallbackExec callback, IntPtr userData,
            out IntPtr errMsg);

        #endregion
        
        int ISQLiteProvider.sqlite3_open(string filename, out IntPtr db) {
            return sqlite3_open(filename.ToUTF8Bytes(), out db);
        }
        int ISQLiteProvider.sqlite3_open_v2(string filename, out IntPtr db, int flags, string vfs) {
            return sqlite3_open_v2(filename.ToUTF8Bytes(), out db, flags, vfs.ToUTF8Bytes());
        }
        int ISQLiteProvider.sqlite3_close(IntPtr db) {
            return sqlite3_close(db);
        }
        int ISQLiteProvider.sqlite3_close_v2(IntPtr db) {
            return sqlite3_close_v2(db);
        }
        long ISQLiteProvider.sqlite3_last_insert_rowid(IntPtr db) {
            return sqlite3_last_insert_rowid(db);
        }
        string ISQLiteProvider.sqlite3_errmsg(IntPtr db) {
            var ptr = sqlite3_errmsg(db);
            if (ptr == IntPtr.Zero)
                return null;
            
            var result = ptr.ToUTF8String();
            return result;
        }
        int ISQLiteProvider.sqlite3_prepare_v2(IntPtr db, string sql, out IntPtr stmt, out string remain) {
            var result = 0;
            var _remain = IntPtr.Zero;
            var _stmt = IntPtr.Zero;
            sql.UsingUTF8Pointer((ptr, len) => {
                result = sqlite3_prepare_v2(db, ptr, len, out _stmt, out _remain);
            });
            stmt = _stmt;
            
            remain = _remain == IntPtr.Zero ? null : _remain.ToUTF8String();
            return result;
        }
        int ISQLiteProvider.sqlite3_step(IntPtr stmt) {
            return sqlite3_step(stmt);
        }
        int ISQLiteProvider.sqlite3_finalize(IntPtr stmt) {
            return sqlite3_finalize(stmt);
        }
        unsafe int ISQLiteProvider.sqlite3_bind_blob(IntPtr stmt, int index, byte[] blob) {
            fixed (byte* ptr = blob) {
                return sqlite3_bind_blob(stmt, index, (IntPtr)ptr, blob.Length, new IntPtr(-1));
            }
        }
        int ISQLiteProvider.sqlite3_bind_double(IntPtr stmt, int index, double val) {
            return sqlite3_bind_double(stmt, index, val);
        }
        int ISQLiteProvider.sqlite3_bind_int(IntPtr stmt, int index, int val) {
            return sqlite3_bind_int(stmt, index, val);
        }
        int ISQLiteProvider.sqlite3_bind_int64(IntPtr stmt, int index, long val) {
            return sqlite3_bind_int64(stmt, index, val);
        }
        int ISQLiteProvider.sqlite3_bind_null(IntPtr stmt, int index) {
            return sqlite3_bind_null(stmt, index);
        }
        int ISQLiteProvider.sqlite3_bind_text(IntPtr stmt, int index, string text) {
            return sqlite3_bind_text(stmt, index, text.ToUTF8Bytes(), -1, new IntPtr(-1));
        }
        string ISQLiteProvider.sqlite3_column_name(IntPtr stmt, int index) {
            var ptr = sqlite3_column_name(stmt, index);
            if (ptr == IntPtr.Zero)
                return null;
            
            var result = ptr.ToUTF8String();
            return result;
        }
        string ISQLiteProvider.sqlite3_column_text(IntPtr stmt, int index) {
            var ptr = sqlite3_column_text(stmt, index);
            if (ptr == IntPtr.Zero)
                return null;
            
            var result = ptr.ToUTF8String();
            return result;
        }
        int ISQLiteProvider.sqlite3_column_count(IntPtr stmt) {
            return sqlite3_column_count(stmt);
        }
        double ISQLiteProvider.sqlite3_column_double(IntPtr stmt, int index) {
            return sqlite3_column_double(stmt, index);
        }
        int ISQLiteProvider.sqlite3_column_int(IntPtr stmt, int index) {
            return sqlite3_column_int(stmt, index);
        }
        long ISQLiteProvider.sqlite3_column_int64(IntPtr stmt, int index) {
            return sqlite3_column_int64(stmt, index);
        }
        byte[] ISQLiteProvider.sqlite3_column_blob(IntPtr stmt, int index) {
            var ptr = sqlite3_column_blob(stmt, index);
            if (ptr == IntPtr.Zero)
                return null;
            
            var len = sqlite3_column_bytes(stmt, index);
            var bytes = new byte[len];
            if (len > 0)
                Marshal.Copy(ptr, bytes, 0, len);
            
            return bytes;
        }
        int ISQLiteProvider.sqlite3_column_type(IntPtr stmt, int index) {
            return sqlite3_column_type(stmt, index);
        }
        int ISQLiteProvider.sqlite3_exec(IntPtr db, string sql, ExecCallback callback, object userData, out string errMsg) {
            if (callback != null)
                throw new NotSupportedException();
            
            var result = sqlite3_exec(db, sql.ToUTF8Bytes(), null, IntPtr.Zero, out var _errMsg);
            if (_errMsg != IntPtr.Zero) {
                errMsg = _errMsg.ToUTF8String();
                sqlite3_free(_errMsg);
            }
            else
                errMsg = null;
            return result;

        }
    }
}