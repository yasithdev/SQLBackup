using System.IO;

namespace SQLBackup
{
    internal interface ISqlBackup
    {
        /// <summary>
        ///     Write Database Backup using the given StreamWriter.
        ///     Preferable due to low memory requirements
        /// </summary>
        /// <param name="s"></param>
        void BackupDb(Stream s);

        /// <summary>
        ///     Restore Database using a StreamReader that reads a SQL Backup
        /// </summary>
        /// <param name="sw"></param>
        void RestoreDb(Stream sw);
    }
}