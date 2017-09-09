using System.IO;
using MySql.Data.MySqlClient;
using SQLBackup;

namespace SQLBackupTest
{
    internal class BackupTest
    {
        private const string ConnectionStringMac =
                "server=localhost.mac;user id=onion;password=onion;persistsecurityinfo=True;database=Onion;characterset=utf8;allowbatch=True;allowuservariables=True"
            ;

        private static void Main(string[] args)
        {
            using (var connection = new MySqlConnection(ConnectionStringMac))
            {
                connection.Open();
                var command = connection.CreateCommand();
                var backupProvider = new SqlBackup(command);
                var fileStream = new FileStream("C:\\Onion Inc\\Onion\\Backups\\backup.sql", FileMode.Create);
                backupProvider.BackupDb(new StreamWriter(fileStream));
            }
        }
    }
}