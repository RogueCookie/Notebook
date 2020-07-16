using System;
using System.Collections.Generic;

namespace Notebook.Domain.Entity
{
    public  class Record
    {
        public long Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Theme { get; set; }
        public string Place { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsComplete { get; set; }
        public DateTime CreatedAt { get; set; }
        public RecordType RecordType { get; set; }
        public virtual ICollection<RecordsToContacts> RecordsToContacts { get; set; }
    }
}
