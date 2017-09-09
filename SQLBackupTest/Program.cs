using System;
using System.IO;
using MySql.Data.MySqlClient;
using SQLBackup;

namespace SQLBackupTest
{
    internal class Program
    {
        private const string ConnectionStringMac =
                "server=localhost.mac;user id=onion;password=onion;persistsecurityinfo=True;database=Onion;characterset=utf8;allowbatch=True;allowuservariables=True;"
            ;

        public static void Main(string[] args)
        {
            using (var connection = new MySqlConnection(ConnectionStringMac))
            {
                connection.Open();
                var command = connection.CreateCommand();
                var backupProvider = new SqlBackup(command);
                using (var fileStreamOutput = new FileStream("C:\\Onion Inc\\Onion\\Backups\\backup.sql",
                    FileMode.Create, FileAccess.Write))
                {
                    using (var streamWriter = new StreamWriter(fileStreamOutput))
                    {
                        Console.WriteLine("Backing up database");
                        backupProvider.BackupDb(streamWriter);
                    }
                }
                Console.WriteLine("DB Backup Completed");
                using (var fileStreamInput = new FileStream("C:\\Onion Inc\\Onion\\Backups\\backup.sql", FileMode.Open,
                    FileAccess.Read))
                {
                    using (var streamReader = new StreamReader(fileStreamInput))
                    {
                        Console.WriteLine("Restoring database");
                        backupProvider.RestoreDb(streamReader);
                    }
                }
                Console.WriteLine("DB Restore Completed");
                Console.ReadLine();
            }
        }
    }
}