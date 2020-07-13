using Notebook.Domain.Enum;

namespace Notebook.Domain.Entity
{
    public class RecordType
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public RecordTypeEnum Allias { get; set; }
        public string Description { get; set; }
    }
}