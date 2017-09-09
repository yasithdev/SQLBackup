using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace SQLBackup
{
    public class EncryptedSqlBackup : SqlBackup
    {
        public EncryptedSqlBackup(IDbCommand command, byte[] encryptionKey) : base(command)
        {
            if (encryptionKey == null || encryptionKey.Length != 16)
                throw new ArgumentNullException(nameof(encryptionKey), @"128-bit Encryption Key needed");
            Algorithm = new RijndaelManaged
            {
                Key = encryptionKey,
                IV = encryptionKey.Reverse().ToArray()
            };
            Encryptor = Algorithm.CreateEncryptor(Algorithm.Key, Algorithm.IV);
            Decryptor = Algorithm.CreateDecryptor(Algorithm.Key, Algorithm.IV);
        }

        private SymmetricAlgorithm Algorithm { get; }
        private ICryptoTransform Encryptor { get; }
        private ICryptoTransform Decryptor { get; }

        public new void BackupDb(Stream s)
        {
            using (var cryptoStream = new CryptoStream(s, Encryptor, CryptoStreamMode.Write))
            {
                base.BackupDb(cryptoStream);
            }
        }

        public new void RestoreDb(Stream s)
        {
            using (var cryptoStream = new CryptoStream(s, Decryptor, CryptoStreamMode.Read))
            {
                base.RestoreDb(cryptoStream);
            }
        }
    }
}