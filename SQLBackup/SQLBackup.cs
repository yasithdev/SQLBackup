using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
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

        public void RestoreDb(StreamReader s)
        {
            s.BaseStream.Position = 0;
            // Command
            var c = string.Empty;
            // Delimiter
            var d = ";";

            while (!s.EndOfStream)
            {
                // Current Line
                var l = s.ReadLine()?.Trim();
                if (string.IsNullOrWhiteSpace(l)) continue;
                // First Character of Line
                var chr = l[0];
                if (chr == '#' || chr == '-') continue;

                while (true)
                {
                    // Matcher for Delimiter
                    var dm = Regex.Match(l, $@"(?<!\\){d}", RegexOptions.IgnoreCase);
                    // Matcher for New Delimiter
                    var ndm = Regex.Match(l, @"(?<=DELIMITER\s+).+?(?=(\s|$))", RegexOptions.IgnoreCase);

                    // If no matches found
                    if (!dm.Success && !ndm.Success)
                    {
                        // Append whole line to command and break
                        if (l.Trim() != string.Empty) c += l.Trim() + "\r\n";
                        break;
                    }
                    // One or more regexes have matched. Get assign indexes or MaxValue to variables
                    var di = dm.Success ? dm.Index : int.MaxValue;
                    var ndi = ndm.Success ? ndm.Index : int.MaxValue;
                    // Check which comes first
                    if (di < ndi)
                    {
                        // First occurence is delimiter index.
                        // Ignore others since they get handled in next iteration
                        c += l.Substring(0, dm.Index);
                        DbCommand.CommandText = c;
                        DbCommand.ExecuteNonQuery();
                        c = string.Empty;
                        l = l.Substring(dm.Index + dm.Length).Trim();
                    }
                    else
                    {
                        // First occurence is new delimiter.
                        // Ignore others since they get handled in next iteration
                        // IMPORTANT: When assigning the delimiter, make sure to escape all characters
                        d = Regex.Escape(l.Substring(ndm.Index, ndm.Length));
                        l = l.Substring(ndm.Index + ndm.Length).Trim();
                    }
                    if (l == string.Empty) break;
                }
            }
        }
    }
}