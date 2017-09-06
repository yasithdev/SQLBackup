# SQLBackup - SQL Backup/Restore Toolkit for .NET

## What is SQLBackup?
**SQLBackup** is a toolkit for .NET developers that allow them to create SQL Dumps of their databases and restore from them without depending on `mysqldump` and other external executables.

### Supported Databases
The current version of SQLBackup supports **MySQL** and **MariaDB**.

### Plans for Future Development
* Support for PostgreSQL
* Test cases involving ALL data formats for each DBMS

## Features

### 1. Backup to a Stream
```C#
string someConnectionString;
// Can be a MemoryStream, FileStream or anything with the Base Class `Stream`
Stream someStream;

using (IDBConnection connection = new MySqlConnection(someConnectionString))
{
    connection.Open();
    var command = connection.CreateCommand();
    // To Create a SQLBackup instance, you need an instance of IDBCommand
    var backupProvider = new SqlBackup(command);
    // To create a Backup, the method accepts a StreamWriter object, to which it writes SQL.
    var streamWriter = new StreamWriter(someStream);
    backupProvider.BackupDb(streamWriter);
}
```
Here, if a `MemoryStream` is used as the underlying Stream, you can obtain a `string` output (if required), simply by calling 
```C#
new StreamReader(memoryStream).ReadToEnd();
```

### 2. Restore from a Stream
```C#
string someConnectionString;
// Can be a MemoryStream, FileStream or anything with the Base Class `Stream`
Stream someStream;

using (IDBConnection connection = new MySqlConnection(someConnectionString))
{
    connection.Open();
    var command = connection.CreateCommand();
    // To Create a SQLBackup instance, you need an instance of IDBCommand
    var backupProvider = new SqlBackup(command);
    // To restore from a Backup, the method accepts a StreamReader object, using which it can read the SQL.
    var streamReader = new StreamReader(someStream);
    backupProvider.RestoreDb(streamReader);
}
```

NOTE: Restoring from a string input is discouraged since it increases the memory footprint of this tool. However, if you really need it, you can use a method like this
```C#
// Method to get a Stream from a String
public static Stream StringToStream(string s)
{
    MemoryStream stream = new MemoryStream();
    StreamWriter writer = new StreamWriter(stream);
    writer.Write(s);
    writer.Flush();
    stream.Position = 0;
    return stream;
}
// And in your code, you can use this stream as needed
```
