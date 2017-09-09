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
        void BackupDb(StreamWriter s);

        /// <summary>
        ///     Restore Database using a StreamReader that reads a SQL Backup
        /// </summary>
        /// <param name="s"></param>
        void RestoreDb(StreamReader s);
    }
}