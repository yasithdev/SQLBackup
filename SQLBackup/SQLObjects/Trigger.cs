namespace SQLBackup.SQLObjects
{
    internal class Trigger : ISqlWritable
    {
        public Trigger(string name, string triggerDdl)
        {
            Name = name;
            TriggerDdl = triggerDdl;
        }

        public string Name { get; set; }
        public string TriggerDdl { get; set; }

        public string GetDdl()
        {
            return $"DELIMITER %%\r\n{TriggerDdl}%%\r\nDELIMITER ;\r\n";
        }

        public string GetDropSql()
        {
            return $"DROP TRIGGER IF EXISTS `{Name}`;";
        }

        public override string ToString()
        {
            return Name;
        }
    }
}