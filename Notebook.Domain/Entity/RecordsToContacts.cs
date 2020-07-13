namespace Notebook.Domain.Entity
{
    public class RecordsToContacts
    {
        public long RecordId { get; set; }
        public long ContactId { get; set; }
        public Record Record { get; set; }
        public Contact Contact { get; set; }
    }
}
