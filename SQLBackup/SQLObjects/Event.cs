namespace SQLBackup.SQLObjects
{
    internal class Event : ISqlWritable
    {
        public Event(string name, string eventDdl)
        {
            Name = name;
            EventDdl = eventDdl;
        }

        public string Name { get; set; }
        public string EventDdl { get; set; }

        public string GetDdl()
        {
            return $"DELIMITER %%\r\n{EventDdl}%%\r\nDELIMITER ;\r\n";
        }

        public string GetDropSql()
        {
            return $"DROP EVENT IF EXISTS `{Name}`;";
        }

        public override string ToString()
        {
            return Name;
        }
    }
}