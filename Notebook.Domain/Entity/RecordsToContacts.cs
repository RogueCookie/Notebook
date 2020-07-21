namespace Notebook.Domain.Entity
{
    public class RecordsToContacts
    {
        /// <summary>
        /// Id of record
        /// </summary>
        public long RecordId { get; set; }

        /// <summary>
        /// Id of the contact
        /// </summary>
        public long ContactId { get; set; }

        //Navigation property

        /// <summary>
        /// Describes additional information about record 
        /// </summary>
        public Record Record { get; set; }

        /// <summary>
        /// Represent relationship one to many
        /// </summary>
        public Contact Contact { get; set; }
    }
}
