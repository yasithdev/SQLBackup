using System;
using System.Data;
using System.IO;
using SQLBackup.SQLObjects;

namespace SQLBackup
{
    public class SqlBackup : ISqlBackup, IDisposable
    {
        public SqlBackup(IDbCommand command)
        {
            DbCommand = command;
        }

        public IDbCommand DbCommand { get; set; }

        public void Dispose()
        {
            DbCommand.Dispose();
        }

        public void BackupDb(out string s)
        {
            using (var memoryStream = new MemoryStream())
            {
                // Backup to the memory stream 
                var streamWriter = new StreamWriter(memoryStream);
                BackupDb(streamWriter);
                streamWriter.Flush();
                // Create string from memory stream
                memoryStream.Position = 0;
                s = new StreamReader(memoryStream).ReadToEnd();
            }
        }

        public void BackupDb(StreamWriter s)
        {
            var dbReader = new DbReader(DbCommand);
            var backupCreator = new BackupCreator(s, dbReader);
            // Header
            backupCreator.WriteFileHeader();
            // Tables
            var tableNames = dbReader.GetTableNames();
            foreach (var name in tableNames)
                backupCreator.WriteSqlContent(new Table(name, dbReader.GetTableDdl(name)));
            // Views
            var viewNames = dbReader.GetViewNames();
            foreach (var name in viewNames)
                backupCreator.WriteSqlContent(new View(name, dbReader.GetViewDdl(name)));
            // Events
            var eventNames = dbReader.GetEventNames();
            foreach (var name in eventNames)
                backupCreator.WriteSqlContent(new Event(name, dbReader.GetEventDdl(name)));
            // Functions
            var functionNames = dbReader.GetFunctionNames();
            foreach (var name in functionNames)
                backupCreator.WriteSqlContent(new Function(name, dbReader.GetFunctionDdl(name)));
            // Stored Procedures
            var procedureNames = dbReader.GetProcedureNames();
            foreach (var name in procedureNames)
                backupCreator.WriteSqlContent(new Procedure(name, dbReader.GetProcedureDdl(name)));
            // Triggers
            var triggerNames = dbReader.GetTriggerNames();
            foreach (var name in triggerNames)
                backupCreator.WriteSqlContent(new Trigger(name, dbReader.GetTriggerDdl(name)));
            // Writing to Stream Is Complete
            backupCreator.WriteFileFooter();
            s.Flush();
        }

        public void RestoreDb(string s)
        {
            DbCommand.CommandText = s;
            DbCommand.ExecuteNonQuery();
        }

        public void RestoreDb(StreamReader s)
        {
            s.BaseStream.Position = 0;
            DbCommand.CommandText = s.ReadToEnd();
            DbCommand.ExecuteNonQuery();
        }
    }
}