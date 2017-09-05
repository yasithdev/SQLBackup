using System.Collections.Generic;

namespace SQLBackup.SQLObjects
{
    internal class Schema
    {
        public string Name { get; set; }
        public string Charset { get; set; }
        public IEnumerable<Table> Tables { get; set; }
        public IEnumerable<View> Views { get; set; }
        public IEnumerable<Event> Events { get; set; }
        public IEnumerable<Function> Functions { get; set; }
        public IEnumerable<Procedure> Procedures { get; set; }
        public IEnumerable<Trigger> Triggers { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}