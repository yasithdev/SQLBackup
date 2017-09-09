using System.Data;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace SQLBackup
{
    public class EncryptedSqlBackup : SqlBackup
    {
        public EncryptedSqlBackup(IDbCommand command, string encryptionKey) : base(command)
        {
            EncryptionKey = Encoding.ASCII.GetBytes(encryptionKey);
        }

        public byte[] EncryptionKey { get; set; }

        public new void BackupDb(Stream s)
        {
            var cryptoStream = new CryptoStream(s, new HMACSHA512(EncryptionKey), CryptoStreamMode.Write);
            base.BackupDb(cryptoStream);
        }

        public new void RestoreDb(Stream s)
        {
            var cryptoStream = new CryptoStream(s, new HMACSHA512(EncryptionKey), CryptoStreamMode.Read);
            base.RestoreDb(cryptoStream);
        }
    }
}