using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SQLBackup.SQLObjects
{
    internal class Table : ISqlWritable
    {
        public Table(string name, string ddl)
        {
            Name = name;
            // Modify the DDL to reset AUTO_INCREMENT value
            Ddl = Regex.Replace(ddl, @"\sAUTO_INCREMENT=\d+", string.Empty);
        }

        public string Name { get; set; }
        public IEnumerable<Column> Columns { get; set; }
        public string Ddl { get; set; }

        public string GetDdl()
        {
            return Ddl;
        }

        public string GetDropSql()
        {
            return $"DROP TABLE IF EXISTS `{Name}`;";
        }

        public override string ToString()
        {
            return Name;
        }
    }
}