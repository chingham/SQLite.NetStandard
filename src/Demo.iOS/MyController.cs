using System;
using System.Diagnostics;
using SQLite;
using UIKit;

namespace Demo.iOS {
    using static SQLite.SQLite;
    
    class MyController : UIViewController {
        public override void ViewDidLoad() {
            base.ViewDidLoad();
            
            SetProvider(new SQLiteProvider());
            
            TestClassic();
            TestSimplified();
        }

        /// <summary>
        /// Creates a new database, with a new table, empty it (in case the database already existed).
        /// Then it inserts a value, and read it back.
        /// </summary>
        void TestClassic() {
            Error error;
            StepResult result;

            //Open database
            var path = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/database.db";
            error = sqlite3_open(path, out sqlite3 db);
            Debug.Assert(error == Error.OK);
            
            //Create table
            error = sqlite3_exec(db,
                "CREATE TABLE IF NOT EXISTS demo1 (id INTEGER PRIMARY KEY AUTOINCREMENT, name TEXT)");
            Debug.Assert(error == Error.OK);
            
            //Empty table
            error = sqlite3_exec(db, "DELETE FROM demo1");
            Debug.Assert(error == Error.OK);

            //Prepare insert statement
            error = sqlite3_prepare_v2(db, "INSERT INTO demo1 (name) VALUES (?)", out sqlite3_stmt stmt);
            Debug.Assert(error == Error.OK);
            
            //Bind value
            error = sqlite3_bind_text(stmt, 1, "Foo");
            Debug.Assert(error == Error.OK);
            
            //Execute
            result = sqlite3_step(stmt);
            Debug.Assert(result == StepResult.Done);

            //Finalize
            error = sqlite3_finalize(stmt);
            Debug.Assert(error == Error.OK);

            //Prepare select statement
            error = sqlite3_prepare_v2(db, "SELECT * FROM demo1", out stmt);
            Debug.Assert(error == Error.OK);
            
            //Step
            while (sqlite3_step(stmt) == StepResult.Row) {
                
                //Column count
                var columnCount = sqlite3_column_count(stmt);
                Debug.Assert(columnCount == 2);
                
                //Last column as text
                var text = sqlite3_column_text(stmt, columnCount - 1);
                Debug.Assert(text == "Foo");
            }

            //Finalize
            error = sqlite3_finalize(stmt);
            Debug.Assert(error == Error.OK);

            //Close database
            error = sqlite3_close(db);
            Debug.Assert(error == Error.OK);
        }

        /// <summary>
        /// Executes exactly the same as Test1, but using the simplified API.
        /// </summary>
        void TestSimplified() {
            
            //Open database
            var path = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/database.db";
            using (var db = sqlite3.Open(path)) {
                
                //Create table
                db.Execute("CREATE TABLE IF NOT EXISTS demo1 (id INTEGER PRIMARY KEY AUTOINCREMENT, name TEXT)");

                //Empty table
                db.Execute("DELETE FROM demo1");

                //Prepare insert statement
                var query = new Query("INSERT INTO demo1 (name) VALUES (?)");
                query.AddParameter("Foo");
                db.Execute(query);
                
                //Select
                db.QueryReader("SELECT * FROM demo1", row => {
                    
                    //Column count
                    var columnCount = row.Columns.Count;
                    Debug.Assert(columnCount == 2);
                
                    //Last column as text
                    var text = row.GetString(columnCount - 1);
                    Debug.Assert(text == "Foo");
                    
                    //Last column as text from column name
                    text = row.GetString("name");
                    Debug.Assert(text == "Foo");
                });
            }
        }
    }
}