namespace SQLBackup.SQLObjects
{
    internal class View : ISqlWritable
    {
        public View(string name, string ddl)
        {
            Name = name;
            Ddl = ddl;
        }

        public string Name { get; set; }
        public string Ddl { get; set; }

        public string GetDdl()
        {
            return Ddl;
        }

        public string GetDropSql()
        {
            return $"DROP VIEW IF EXISTS `{Name}`;";
        }

        public override string ToString()
        {
            return Name;
        }
    }
}