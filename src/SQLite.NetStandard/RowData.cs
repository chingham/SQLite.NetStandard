using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;

namespace SQLite {
    public class RowData {
        internal RowData(sqlite3_stmt statement, bool cache = false) {
            this.statement = statement;

            var columnCount = SQLite.sqlite3_column_count(statement);
            var columns = new List<string>();
            for (var i = 0; i < columnCount; i++) {
                var name = SQLite.sqlite3_column_name(statement, i);
                ordinals.Add(name, i);
                columns.Add(name);
            }

            Columns = new ReadOnlyCollection<string>(columns);

            if (cache) {
                for (var i = 0; i < columnCount; i++) {
                    CacheValue(i);
                }
            }
        }
        readonly sqlite3_stmt statement;
        
        public IReadOnlyCollection<string> Columns { get; }

        readonly Dictionary<string, int> ordinals = new Dictionary<string, int>();
        readonly Dictionary<int, ColumnType> types = new Dictionary<int, ColumnType>();
        readonly Dictionary<int, object> values = new Dictionary<int, object>();
        int GetOrdinal(string name) {
            if (ordinals.TryGetValue(name, out var i))
                return i;
            return -1;
        }
        ColumnType GetType(int i) {
            if (types.TryGetValue(i, out var type))
                return type;
            type = statement.GetDataType(i);
            types.Add(i, type);
            return type;
        }
        object CacheValue(int i) {
            var type = GetType(i);
            switch (type) {
                case ColumnType.Blob:
                    var bytes = statement.GetBytes(i);
                    values.Add(i, bytes);
                    return bytes;
                case ColumnType.Integer:
                    var integer = statement.GetInt(i);
                    values.Add(i, integer);
                    return integer;
                case ColumnType.Float:
                    var f = statement.GetDouble(i);
                    values.Add(i, f);
                    return f;
                case ColumnType.Text:
                    var t = statement.GetString(i);
                    values.Add(i, t);
                    return t;
                default:
                    values.Add(i, null);
                    return null;
            }
        }

        public void ClearForNewRow() {
            types.Clear();
            values.Clear();
        }

        public bool? GetBoolean(int i) {
            var r = GetInt32(i);
            return r.HasValue && r.Value > 0;
        }
        public bool? GetBoolean(string name) {
            return GetBoolean(GetOrdinal(name));
        }

        public int? GetInt32(int i) {
            if (i < 0 || i >= ordinals.Count)
                return null;
			
            if (!values.TryGetValue(i, out var rv))
                rv = CacheValue(i);
			
            switch (rv) {
                case int integer: return integer;
                case double f: return (int)f;
                case string t:
                    if (int.TryParse(t, out var r))
                        return r;
                    else
                        return null;
                default: return null;
            }
        }
        public int? GetInt32(string name) {
            return GetInt32(GetOrdinal(name));
        }

        public double? GetDouble(int i) {
            if (i < 0 || i >= ordinals.Count)
                return null;

            if (!values.TryGetValue(i, out var rv))
                rv = CacheValue(i);

            switch (rv) {
                case int integer: return integer;
                case double f: return f;
                case string t:
                    if (double.TryParse(t, NumberStyles.Any, CultureInfo.InvariantCulture, out var r))
                        return r;
                    else
                        return null;
                default: return null;
            }
        }
        public double? GetDouble(string name) {
            return GetDouble(GetOrdinal(name));
        }

        public string GetString(int i) {
            if (i < 0 || i >= ordinals.Count)
                return null;
			
            if (!values.TryGetValue(i, out var rv))
                rv = CacheValue(i);

            switch (rv) {
                case int integer: return integer.ToString(CultureInfo.InvariantCulture);
                case double f: return f.ToString(CultureInfo.InvariantCulture);
                case string t: return t;
                default: return null;
            }
        }
        public string GetString(string name) {
            return GetString(GetOrdinal(name));
        }

        public byte[] GetBlob(int i) {
            if (i < 0 || i >= ordinals.Count)
                return null;

            if (!values.TryGetValue(i, out var rv))
                rv = CacheValue(i);

            switch (rv) {
                case byte[] bytes: return bytes;
                default: return null;
            }
        }
        public byte[] GetBlob(string name) {
            return GetBlob(GetOrdinal(name));
        }

        static readonly DateTime UnixBaseDate = new DateTime(1970, 1, 1);
        public DateTime? GetDateTime(int i) {
            if (i < 0 || i >= ordinals.Count)
                return null;

            if (!values.TryGetValue(i, out var rv))
                rv = CacheValue(i);

            switch (rv) {
                case int integer: return UnixBaseDate + TimeSpan.FromSeconds(integer);
                case double f: return UnixBaseDate + TimeSpan.FromSeconds(f);
                case string t:
                    if (DateTime.TryParse(t, out var dt))
                        return dt;
                    else
                        return null;
                default: return null;
            }
        }
        public DateTime? GetDateTime(string name) {
            return GetDateTime(GetOrdinal(name));
        }
    }
}