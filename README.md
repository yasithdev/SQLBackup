# SQLBackup - SQL Backup/Restore Toolkit for .NET

## What is SQLBackup?
**SQLBackup** is a toolkit for .NET developers that allow them to create SQL Dumps of their databases and restore from them without depending on `mysqldump` and other external executables.

### Features
* The current version of SQLBackup supports **MySQL** and **MariaDB**.
* **NEW!** Added Encryption Support. To use encryption, simply instantiate `EncryptedSQLBackup` class instead of the `SQLBackup` class

### Future Development
* Add Customization options to specify the Content needed as the Output (i.e. Only Data, Only Tables and Views, etc)
* Support for PostgreSQL and Microsoft SQL Server
* Separate Test cases for each RDBMS

## Sample Code

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
    backupProvider.BackupDb(someStream);
}
```
Here, if a `MemoryStream` is used as the underlying Stream, you can obtain a `string` output (if required), simply by calling 
```C#
new StreamReader(someStream).ReadToEnd();
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
    backupProvider.RestoreDb(someStream);
}
```

### 3. Using Encrypted Backup/Restore
You can create encrypted backups similar to the given example. You only need to replace 
```C#
var backupProvider = new SqlBackup(command);
```
with the class `EncryptedSqlBackup`, and pass an `encryption key` (string) to encrypt/decrypt the stream content
```C#
var backupProvider = new EncryptedSqlBackup(command, encryptionKey);
```

NOTE: Restoring from a string input is discouraged since it increases the memory footprint of this tool. However, if you really need it, you can use a method like this
```C#
// Method to get a Stream from a String
public static Stream StringToStream(string s)
{
    MemoryStream stream = new MemoryStream();
    using(StreamWriter writer = new StreamWriter(stream)){
        writer.Write(s);
        writer.Flush();
    }
    stream.Position = 0;
    return stream;
}
// And in your code, you can use this stream as needed
```
