# SQLite.NetStandard
[![MIT license](https://img.shields.io/badge/License-MIT-blue.svg)](https://lbesson.mit-license.org/)
[![Generic badge](https://img.shields.io/badge/.netstandard-2.0-green.svg)](https://shields.io/)

.NetStandard SQLite Abstraction

This project consists of a "core" library (SQLite.NetStandard) targeting .NetStandard 2.0, and one implementation for each platform.

# Supported plateforms
* Xamarin.iOS
* More coming soon (At least Xamarin.Android, UWP, Xamarin.Mac, .NET Framework 4.7.2)

# Usage
In your shared project:
Reference SQLite.NetStandard and use it. The API is deliberately very similar to the original C API.
```csharp
using static SQLite.SQLite

//...

var error = sqlite3_open(path_to_file, out sqlite3 db);
if (error != Error.OK)
  throw new Exception();
  
using (db) {
    error = sqlite3_exec(db, "CREATE TABLE IF NOT EXISTS demo1 (id INTEGER PRIMARY KEY AUTOINCREMENT, name TEXT)");
    if (error != Error.OK)
      throw new Exception();
  
    error = sqlite3_exec(db, "DELETE FROM demo1");
    if (error != Error.OK)
      throw new Exception();

    error = sqlite3_prepare_v2(db, "INSERT INTO demo1 (name) VALUES (?)", out sqlite3_stmt stmt);
    if (error != Error.OK)
      throw new Exception();
      
    using (stmt) {
        error = sqlite3_bind_text(stmt, 1, "Foo");
        if (error != Error.OK)
          throw new Exception();
      
        var result = sqlite3_step(stmt);
        if (result != Error.Done)
          throw new Exception();
    }

    error = sqlite3_prepare_v2(db, "SELECT * FROM demo1", out stmt);
    if (error != Error.OK)
      throw new Exception();
      
    using (stmt) {
        while (sqlite3_step(stmt) == StepResult.Row) {
            var columnCount = sqlite3_column_count(stmt);
            var text = sqlite3_column_text(stmt, columnCount - 1);
        }
    }
}
```

In your platform project, reference the corresponding platform project (eg. SQLite.NetStandard.iOS on Xamarin.iOS) and initialize it before invoking your shared library code.

```csharp
SQLite.SQLite.SetProvider(new SQLite.SQLiteProvider());
```
