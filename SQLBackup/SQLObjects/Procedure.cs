namespace SQLBackup.SQLObjects
{
    internal class Procedure : ISqlWritable
    {
        public Procedure(string name, string procedureDdl)
        {
            Name = name;
            ProcedureDdl = procedureDdl;
        }

        public string Name { get; set; }
        public string ProcedureDdl { get; set; }

        public string GetDdl()
        {
            return $"DELIMITER %%\r\n{ProcedureDdl}%%\r\nDELIMITER ;\r\n";
        }

        public string GetDropSql()
        {
            return $"DROP PROCEDURE IF EXISTS `{Name}`;";
        }

        public override string ToString()
        {
            return Name;
        }
    }
}