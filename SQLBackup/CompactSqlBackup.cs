using System.Data;
using System.IO;
using System.IO.Compression;

namespace SQLBackup
{
    public class CompactSqlBackup : SqlBackup
    {
        public CompactSqlBackup(IDbCommand command) : base(command)
        {
        }

        public override void BackupDb(Stream s)
        {
            using (var compressionStream = new GZipStream(s, CompressionMode.Compress))
            {
                base.BackupDb(compressionStream);
            }
        }

        public override void RestoreDb(Stream s)
        {
            using (var compressionStream = new GZipStream(s, CompressionMode.Decompress))
            {
                base.RestoreDb(compressionStream);
            }
        }
    }
}