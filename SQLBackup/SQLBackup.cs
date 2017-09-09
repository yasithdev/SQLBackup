using System;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
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

        public void BackupDb(Stream s)
        {
            var dbReader = new DbReader(DbCommand);
            using (var backupCreator = new BackupCreator(new StreamWriter(s), dbReader))
            {
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
            }
        }

        public void RestoreDb(Stream s)
        {
            var sr = new StreamReader(s);
            var command = string.Empty;
            var delimiter = ";";
            while (!sr.EndOfStream)
            {
                // Current Line
                var line = sr.ReadLine()?.Trim();
                if (string.IsNullOrWhiteSpace(line)) continue;
                // First Character of Line
                var chr = line[0];
                if (chr == '#' || chr == '-') continue;

                while (true)
                {
                    line = line.Trim();
                    if (line == string.Empty) break;
                    // Matcher for Delimiter
                    var dm = Regex.Match(line, $@"(?<!(\\|\')){delimiter}", RegexOptions.IgnoreCase);
                    // Matcher for New Delimiter
                    var ndm = Regex.Match(line, @"(?<=DELIMITER\s+).+?(?=(\s|$))", RegexOptions.IgnoreCase);

                    // If no matches found, append to command and break
                    if (!dm.Success && !ndm.Success)
                    {
                        command += line + "\r\n";
                        break;
                    }
                    // One or more regexes have matched. Assign matching index or max int value
                    var di = dm.Success ? dm.Index : int.MaxValue;
                    var ndi = ndm.Success ? ndm.Index : int.MaxValue;
                    // Check which match comes first. Ignore other matches to be handled in the next iteration
                    if (di < ndi)
                    {
                        // First match is delimiter
                        command += line.Substring(0, dm.Index);
                        DbCommand.CommandText = command;
                        DbCommand.ExecuteNonQuery();
                        command = string.Empty;
                        line = line.Substring(dm.Index + dm.Length).Trim();
                    }
                    else
                    {
                        // First occurence is new delimiter definition
                        delimiter = Regex.Escape(line.Substring(ndm.Index, ndm.Length));
                        line = line.Substring(ndm.Index + ndm.Length).Trim();
                    }
                }
            }
        }
    }
}