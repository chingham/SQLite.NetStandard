using System;
using System.Diagnostics;
using SQLite;
using UIKit;

namespace Blank {
    using static SQLite.SQLite;
    
    class MyController : UIViewController {
        public override void ViewDidLoad() {
            base.ViewDidLoad();
            
            SetProvider(new SQLiteProvider());

            Error error;
            var path = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/database.db";
            error = sqlite3_open(path, out sqlite3 db);
            using (db) {
                error = sqlite3_exec(db, "CREATE TABLE IF NOT EXISTS demo1 (id INTEGER PRIMARY KEY AUTOINCREMENT, name TEXT)");
                error = sqlite3_exec(db, "DELETE FROM demo1");

                error = sqlite3_prepare_v2(db, "INSERT INTO demo1 (name) VALUES (?)", out sqlite3_stmt stmt);
                using (stmt) {
                    error = sqlite3_bind_text(stmt, 1, "Foo");
                    var result = sqlite3_step(stmt);
                }

                error = sqlite3_prepare_v2(db, "SELECT * FROM demo1", out stmt);
                using (stmt) {
                    while (sqlite3_step(stmt) == StepResult.Row) {
                        var columnCount = sqlite3_column_count(stmt);
                        var text = sqlite3_column_text(stmt, columnCount - 1);
                    }
                }
            }
            
        }
    }
}