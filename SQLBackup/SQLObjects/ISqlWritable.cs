namespace SQLBackup.SQLObjects
{
    internal interface ISqlWritable
    {
        string GetDdl();
        string GetDropSql();
    }
}