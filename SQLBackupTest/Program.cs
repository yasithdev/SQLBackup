using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using MySql.Data.MySqlClient;
using SQLBackup;

namespace SQLBackupTest
{
    internal class Program
    {
        private const string ConnectionStringMac =
                "server=localhost.mac;user=onion;password=onion;persistsecurityinfo=True;database=onion;allowbatch=true;"
            ;

        private const string FilePath = @"C:\Onion Inc\Onion\Backups\onion.backup";
        private const string FileHeader = @"HEADER";
        private const string Key = @"Password";

        private static byte[] GetBytes(string input)
        {
            return Encoding.ASCII.GetBytes(input);
        }

        private static byte[] ComputeMd5Hash(string input)
        {
            return new MD5Cng().ComputeHash(GetBytes(input));
        }

        public static void Main(string[] args)
        {
            var passCount = 0;
            using (var connection = new MySqlConnection(ConnectionStringMac))
            {
                connection.Open();
                var command = connection.CreateCommand();
                // Without Encryption
                try
                {
                    Console.WriteLine("TEST 1 - Backup Database (Encryption OFF)");
                    BackupTestUnencrypted(command);
                    Console.WriteLine("PASS!\n\n");
                    passCount++;
                }
                catch (Exception)
                {
                    Console.WriteLine("FAIL!\n\n");
                }
                try
                {
                    Console.WriteLine("TEST 2 - Restore Database (Encryption OFF)");
                    RestoreTestUnencrypted(command);
                    Console.WriteLine("PASS!\n\n");
                    passCount++;
                }
                catch (Exception)
                {
                    Console.WriteLine("FAIL!\n\n");
                }

                // With Encryption
                try
                {
                    Console.WriteLine("TEST 1 - Backup Database (Encryption ON)");
                    BackupTestEncrypted(command);
                    Console.WriteLine("PASS!\n\n");
                    passCount++;
                }
                catch (Exception)
                {
                    Console.WriteLine("FAIL!\n\n");
                }
                try
                {
                    Console.WriteLine("TEST 2 - Restore Database (Encryption ON)");
                    RestoreTestEncrypted(command);
                    Console.WriteLine("PASS!\n\n");
                    passCount++;
                }
                catch (Exception)
                {
                    Console.WriteLine("FAIL!\n\n");
                }
            }
            Console.WriteLine($"{passCount} of 4 tests passed");
            Console.ReadLine();
        }

        public static void BackupTestUnencrypted(IDbCommand command)
        {
            using (var fileStreamOutput = new FileStream(FilePath, FileMode.Create, FileAccess.Write))
            {
                Console.WriteLine("Backing up database...");
                new SqlBackup(command).BackupDb(fileStreamOutput);
                Console.WriteLine("Backup Completed!");
            }
        }

        public static void RestoreTestUnencrypted(IDbCommand command)
        {
            using (var fileStreamInput = new FileStream(FilePath, FileMode.Open, FileAccess.Read))
            {
                Console.WriteLine("Restoring database...");
                new SqlBackup(command).RestoreDb(fileStreamInput);
                Console.WriteLine("Restore Completed!");
            }
        }

        public static void BackupTestEncrypted(IDbCommand command)
        {
            using (var fileStreamOutput = new FileStream(FilePath, FileMode.Create, FileAccess.Write))
            {
                // Reset Stream Position
                fileStreamOutput.Position = 0;
                // Write the File Header
                Console.Write("Writing File Header...");
                foreach (var b in GetBytes(FileHeader))
                    fileStreamOutput.WriteByte(b);
                Console.WriteLine("DONE!");
                // Write the encryption Key
                Console.Write("Writing Encryption Key...");
                var encryptionKey = ComputeMd5Hash(Key);
                foreach (var b in encryptionKey)
                    fileStreamOutput.WriteByte(b);
                Console.WriteLine("DONE!");
                Console.WriteLine("Backing up database...");
                new EncryptedSqlBackup(command, encryptionKey).BackupDb(fileStreamOutput);
                Console.WriteLine("Backup Completed!");
            }
        }

        public static void RestoreTestEncrypted(IDbCommand command)
        {
            using (var fileStreamInput = new FileStream(FilePath, FileMode.Open, FileAccess.Read))
            {
                // Reset Stream Position
                fileStreamInput.Position = 0;
                // Read the File Header
                Console.Write("Validating File Header...");
                var header = new byte[FileHeader.Length];
                for (var i = 0; i < FileHeader.Length; i++)
                    header[i] = (byte) fileStreamInput.ReadByte();
                // Check if the Headers match
                if (!header.SequenceEqual(GetBytes(FileHeader)))
                    throw new InvalidDataException("Invalid Header!");
                Console.WriteLine("VALIDATED!");
                // Read Encryption Key
                Console.Write("Reading Encryption Key...");
                var encryptionKey = new byte[16];
                for (var i = 0; i < encryptionKey.Length; i++)
                    encryptionKey[i] = (byte) fileStreamInput.ReadByte();
                Console.WriteLine("DONE!");
                Console.WriteLine("Restoring database...");
                new EncryptedSqlBackup(command, encryptionKey).RestoreDb(fileStreamInput);
                Console.WriteLine("Restore Completed!");
            }
        }
    }
}