namespace SQLBackup.SQLObjects
{
    internal class Function : ISqlWritable
    {
        public Function(string name, string functionDdl)
        {
            Name = name;
            FunctionDdl = functionDdl;
        }

        public string Name { get; set; }
        public string FunctionDdl { get; set; }

        public string GetDdl()
        {
            return $"DELIMITER |\r\n{FunctionDdl}|\r\nDELIMITER ;\r\n";
        }

        public string GetDropSql()
        {
            return $"DROP FUNCTION IF EXISTS `{Name}`;";
        }

        public override string ToString()
        {
            return Name;
        }
    }
}