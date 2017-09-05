using System.Data;
using System.IO;
using System.Linq;
using SQLBackup.SQLObjects;

namespace SQLBackup
{
    internal class BackupCreator
    {
        private readonly DbReader _dbReader;
        private readonly StreamWriter _streamWriter;

        /// <summary>
        ///     Creates a Stream Writer Backup Creator object
        ///     that accepts a "StreamWriter" and "DbReader" as parameters
        /// </summary>
        /// <param name="streamWriter"></param>
        /// <param name="dbReader"></param>
        public BackupCreator(StreamWriter streamWriter, DbReader dbReader)
        {
            _streamWriter = streamWriter;
            _dbReader = dbReader;
        }

        #region Private Methods

        /// <summary>
        ///     Writes Table Data in the form of an Insert Query
        /// </summary>
        /// <param name="table"></param>
        /// <param name="dataReader"></param>
        private void WriteTableData(Table table, IDataReader dataReader)
        {
            // When writing table data, should only write to columns that are not generated.
            // Therefore filter out only the required columns for the inserts.
            var columns = table.Columns.Where(c => !c.IsGenerated).Select(c => c.Field).ToList();
            if (!dataReader.Read()) return;
            // Write Table Lock and Disable Keys Before Insert Statements
            _streamWriter.Write(Common.GenerateTableLock(table.Name));
            _streamWriter.Write(
                $"INSERT INTO `{table.Name}` ({Common.GenerateParamString(columns)}) VALUES ");
            while (true)
            {
                var record = $"({Common.GenerateRowInsert(dataReader, columns)})";
                if (dataReader.Read())
                {
                    record += ", ";
                    _streamWriter.Write(record);
                }
                else
                {
                    _streamWriter.Write(record);
                    break;
                }
            }
            _streamWriter.Write(";");
            // Write table Unlock and Enable Keys After Insert Statements
            _streamWriter.Write(Common.GenerateTableUnlock(table.Name));
        }

        #endregion

        /// <summary>
        ///     Writes a Header that indicates the beginning of the Backup File
        /// </summary>
        public void WriteFileHeader()
        {
            var username = _dbReader.GetCurrentUser();
            var dbName = _dbReader.GetDbName();
            var serverVersion = _dbReader.GetServerVersion();
            var charset = _dbReader.GetDatabaseCharset();
            var header = Common.GenerateFileHeader(username, dbName, serverVersion, charset);
            _streamWriter.WriteLine(header);
        }

        public void WriteFileFooter()
        {
            var footer = Common.GenerateFileFooter();
            _streamWriter.Write(footer);
        }

        /// <summary>
        ///     Accepts a ISqlWritable object, and write its Backup into the StreamWriter
        /// </summary>
        /// <param name="source"></param>
        /// <param name="includeDropSql"></param>
        /// <param name="includeData"></param>
        public void WriteSqlContent(ISqlWritable source, bool includeDropSql = true, bool includeData = true)
        {
            // Write header and dropSql (if requested only)
            _streamWriter.WriteLine(Common.GenerateHeader(source.GetType(), source.ToString()));
            if (includeDropSql)
                _streamWriter.WriteLine(source.GetDropSql() + "\r\n");

            // Write Ddl
            _streamWriter.WriteLine(source.GetDdl().Trim(';', '\r', '\n') + ";");

            // If source is a table and includeData = true, back up data too
            if (source is Table table && includeData)
            {
                // Initialize table column details
                table.Columns = _dbReader.GetTableColumnDetails(table.Name);
                using (var reader = _dbReader.GetTableDataReader(table))
                {
                    WriteTableData(table, reader);
                }
            }

            // Insert New Line
            _streamWriter.WriteLine();
        }
    }
}