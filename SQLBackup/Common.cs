using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace SQLBackup
{
    internal static class Common
    {
        private static T GetAttribute<T>(this ICustomAttributeProvider assembly, bool inherit = false)
            where T : Attribute
        {
            return assembly
                .GetCustomAttributes(typeof(T), inherit)
                .OfType<T>()
                .FirstOrDefault();
        }

        private static string GetAssemblyTitle()
        {
            return Assembly.GetExecutingAssembly().GetAttribute<AssemblyTitleAttribute>()?.Title;
        }

        private static string GetAssemblyVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        private static string GetAssemblyDescription()
        {
            return Assembly.GetExecutingAssembly().GetAttribute<AssemblyDescriptionAttribute>()?.Description;
        }

        public static string GenerateFileHeader(string userName, string dbName, string serverVersion, string charset)
        {
            var fileHeader = $@"# ************************************************************
# {GetAssemblyTitle()} {GetAssemblyVersion()}
# {GetAssemblyDescription()}
#
# Host: {userName}
# Database: {dbName} ({serverVersion})
# Generation Time: {DateTime.Now:yyyy-MM-dd HH:mm:ss zzz}
# ************************************************************


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES {charset} */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;
";
            return fileHeader;
        }

        public static string GenerateParamString(IEnumerable<string> parameters)
        {
            return parameters.Aggregate(string.Empty, (current, param) => current + $"`{param}`, ").TrimEnd(',', ' ');
        }

        public static string GenerateHeader(Type type, string name)
        {
            var tableHeader = $@"
# Dump of [{type.Name}] - [{name}]
# ------------------------------------------------------------
";
            return tableHeader;
        }

        public static string GenerateRowInsert(IDataReader dataRow, IEnumerable<string> columns)
        {
            var result = columns.Aggregate(string.Empty,
                (current, column) => current + $"{GenerateParam(dataRow, column)}, ");
            return result
                .TrimEnd(',', ' ');
        }

        private static string GenerateParam(IDataRecord record, string column)
        {
            var index = record.GetOrdinal(column);
            var dataType = record.GetDataTypeName(index);
            var numerics = new[] {"INT", "DOUBLE", "FLOAT", "NUM", "REAL", "DEC", "BIT", "SERIAL", "BOOL", "FIXED"};
            var strings = new[] {"CHAR", "TEXT", "BLOB", "BIN", "SET"};
            var temporal = new[] {"DATE", "TIME", "YEAR"};

            if (record.IsDBNull(index)) return "NULL";

            var param = record[column];
            if (numerics.Any(n => dataType.ToUpper().Contains(n)))
                return $"{param.ToString()}";
            if (!temporal.Any(t => dataType.ToUpper().Contains(t))) return $"'{param.ToString()}'";
            switch (param)
            {
                case DateTime dt:
                    return $"'{dt:yyyy-MM-dd HH:mm:ss)}'";
                default:
                    return $"'{param.ToString()}'";
            }
        }

        public static string GenerateTableLock(string tableName)
        {
            var output = $@"
LOCK TABLES `{tableName}` WRITE;
/*!40000 ALTER TABLE `{tableName}` DISABLE KEYS */;
";
            return output;
        }

        public static string GenerateTableUnlock(string tableName)
        {
            var output = $@"
/*!40000 ALTER TABLE `{tableName}` ENABLE KEYS */;
UNLOCK TABLES;
";
            return output;
        }

        public static string GenerateFileFooter()
        {
            var footer = $@"
/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on {DateTime.Now:yyyy-MM-dd HH:mm:ss zzz}";
            return footer;
        }
    }
}