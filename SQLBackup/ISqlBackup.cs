using System.IO;

namespace SQLBackup
{
    internal interface ISqlBackup
    {
        /// <summary>
        ///     Get Database Backup as a String
        /// </summary>
        /// <param name="s"></param>
        void BackupDb(out string s);

        /// <summary>
        ///     Write Database Backup using the given StreamWriter.
        ///     Preferable due to low memory requirements
        /// </summary>
        /// <param name="s"></param>
        void BackupDb(StreamWriter s);

        /// <summary>
        ///     Restore Database using a SQL Backup String
        /// </summary>
        /// <param name="s"></param>
        void RestoreDb(string s);

        /// <summary>
        ///     Restore Database using a StreamReader that reads a SQL Backup
        /// </summary>
        /// <param name="s"></param>
        void RestoreDb(StreamReader s);
    }
}