namespace SQLBackup.SQLObjects
{
    internal class Column
    {
        public Column(string field, string type, string @null, string key, string @default, string extra)
        {
            Field = field;
            Type = type;
            Null = @null;
            Key = key;
            Default = @default;
            Extra = extra;
        }

        public string Field { get; set; }
        public string Type { get; set; }
        public string Null { get; set; }
        public string Key { get; set; }
        public string Default { get; set; }
        public string Extra { get; set; }
        public bool IsGenerated => Extra.ToUpper().Contains("GENERATED");

        public override string ToString()
        {
            return Field;
        }
    }
}