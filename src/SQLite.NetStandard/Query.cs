using System;
using System.Collections.Generic;
using System.Text;

namespace SQLite {
    public class Query {
        public Query(string cmd) {
            Command = cmd ?? throw new ArgumentNullException(nameof(cmd));
        }
        public Query(string cmd, params object[] parameters) {
            Command = cmd ?? throw new ArgumentNullException(nameof(cmd));
            Parameters.AddRange(parameters);
        }
        public Query(string cmd, IEnumerable<object> parameters) {
            Command = cmd ?? throw new ArgumentNullException(nameof(cmd));
            Parameters.AddRange(parameters);
        }

        public string Command { get; set; }
        public List<object> Parameters { get; } = new List<object>();
        public int AddParameter(object obj) {
            Parameters.Add(obj);
            return Parameters.Count;
        }

        public static implicit operator Query(string cmd) => new Query(cmd);

        public override string ToString() {
            var sb = new StringBuilder();
            sb.AppendLine(Command);
            for (var i = 0; i < Parameters.Count; i++)
                sb.AppendLine($"?{i + 1} : {Parameters[i]}");
            return sb.ToString();
        }
    }
}