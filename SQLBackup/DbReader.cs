using System.Collections.Generic;
using System.Data;
using System.Linq;
using SQLBackup.SQLObjects;

namespace SQLBackup
{
    internal class DbReader
    {
        private readonly IDbCommand _command;

        /// <summary>
        ///     Database Reader that Queries for Information using an IDbCommand
        /// </summary>
        /// <param name="command"></param>
        public DbReader(IDbCommand command)
        {
            _command = command;
        }

        #region Variables

        public string GetCurrentUser()
        {
            _command.CommandText = "SELECT CURRENT_USER";
            return _command.ExecuteScalar().ToString();
        }

        public string GetServerVersion()
        {
            string version = string.Empty, versionComment = string.Empty;
            _command.CommandText = "SHOW VARIABLES LIKE 'version'";
            using (var reader = _command.ExecuteReader())
            {
                if (reader.Read()) version = reader[1].ToString();
            }
            _command.CommandText = "SHOW VARIABLES LIKE 'version_comment'";
            using (var reader = _command.ExecuteReader())
            {
                if (reader.Read()) versionComment = reader[1].ToString();
            }
            return $"{version} {versionComment}";
        }

        public string GetDbName()
        {
            return _command.Connection.Database;
        }

        public string GetDatabaseCharset()
        {
            var charset = string.Empty;
            _command.CommandText = "SHOW VARIABLES LIKE 'character_set_database'";
            using (var reader = _command.ExecuteReader())
            {
                if (reader.Read()) charset = reader[1].ToString();
            }
            return charset;
        }

        #endregion

        #region Triggers

        public IEnumerable<string> GetTriggerNames()
        {
            _command.CommandText = $"SHOW TRIGGERS FROM `{GetDbName()}`";
            using (var result = _command.ExecuteReader())
            {
                var results = new List<string>();
                while (result.Read())
                    results.Add(result["trigger"].ToString());
                return results;
            }
        }

        public string GetTriggerDdl(string triggerName)
        {
            _command.CommandText = $"SHOW CREATE TRIGGER `{triggerName}`";
            using (var result = _command.ExecuteReader())
            {
                return result.Read() ? result["sql original statement"].ToString() : null;
            }
        }

        #endregion

        #region Tables

        public IEnumerable<string> GetTableNames()
        {
            _command.CommandText =
                $"SHOW FULL TABLES FROM {GetDbName()} WHERE table_type = 'BASE TABLE'";
            using (var result = _command.ExecuteReader())
            {
                var results = new List<string>();
                while (result.Read())
                    results.Add(result[0].ToString());
                return results;
            }
        }

        public string GetTableDdl(string tableName)
        {
            _command.CommandText =
                $"SHOW CREATE TABLE {tableName}";
            using (var result = _command.ExecuteReader())
            {
                return result.Read() ? result["create table"].ToString() : null;
            }
        }

        public IEnumerable<Column> GetTableColumnDetails(string tableName)
        {
            _command.CommandText = $"SHOW FULL COLUMNS FROM `{tableName}`";
            using (var result = _command.ExecuteReader())
            {
                var results = new List<Column>();
                while (result.Read())
                    results.Add(new Column(
                        result["field"].ToString(),
                        result["type"].ToString(),
                        result["null"].ToString(),
                        result["key"].ToString(),
                        result["default"].ToString(),
                        result["extra"].ToString()));
                return results;
            }
        }

        public IDataReader GetTableDataReader(Table table)
        {
            var columnNames = table.Columns.Select(c => c.Field);
            _command.CommandText = $"SELECT {Common.GenerateParamString(columnNames)} FROM `{table.Name}`";
            return _command.ExecuteReader();
        }

        #endregion

        #region Views

        public IEnumerable<string> GetViewNames()
        {
            _command.CommandText = $"SHOW FULL TABLES FROM {GetDbName()} WHERE table_type = 'VIEW'";
            using (var result = _command.ExecuteReader())
            {
                var results = new List<string>();
                while (result.Read())
                    results.Add(result[0].ToString());
                return results;
            }
        }

        public string GetViewDdl(string viewName)
        {
            _command.CommandText = $"SHOW CREATE VIEW `{viewName}`";
            using (var result = _command.ExecuteReader())
            {
                return result.Read() ? $"{result["create view"].ToString()};" : string.Empty;
            }
        }

        #endregion

        #region Events

        public IEnumerable<string> GetEventNames()
        {
            _command.CommandText = $"SHOW EVENTS FROM `{GetDbName()}`";
            using (var result = _command.ExecuteReader())
            {
                var results = new List<string>();
                while (result.Read())
                    results.Add(result["name"].ToString());
                return results;
            }
        }

        public string GetEventDdl(string eventName)
        {
            _command.CommandText = $"SHOW CREATE EVENT `{eventName}`";
            using (var result = _command.ExecuteReader())
            {
                return result.Read() ? result["create event"].ToString() : null;
            }
        }

        #endregion

        #region Functions

        public IEnumerable<string> GetFunctionNames()
        {
            _command.CommandText = $"SHOW FUNCTION STATUS WHERE db='{GetDbName()}'";
            using (var result = _command.ExecuteReader())
            {
                var results = new List<string>();
                while (result.Read())
                    results.Add(result["name"].ToString());
                return results;
            }
        }

        public string GetFunctionDdl(string functionName)
        {
            _command.CommandText = $"SHOW CREATE FUNCTION `{functionName}`";
            using (var result = _command.ExecuteReader())
            {
                return result.Read() ? result["create function"].ToString() : null;
            }
        }

        #endregion

        #region Procedures

        public IEnumerable<string> GetProcedureNames()
        {
            _command.CommandText = $"SHOW PROCEDURE STATUS WHERE db='{GetDbName()}'";
            using (var result = _command.ExecuteReader())
            {
                var results = new List<string>();
                while (result.Read())
                    results.Add(result["name"].ToString());
                return results;
            }
        }

        public string GetProcedureDdl(string procedureName)
        {
            _command.CommandText = $"SHOW CREATE PROCEDURE `{procedureName}`";
            using (var result = _command.ExecuteReader())
            {
                return result.Read() ? result["create procedure"].ToString() : null;
            }
        }

        #endregion
    }
}