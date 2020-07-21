using System;
using System.Collections.Generic;

namespace Notebook.Domain.Entity
{
    /// <summary>
    /// Represent information about records in notebook
    /// </summary>
    public  class Record
    {
        /// <summary>
        /// Id of the record
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Start date for current deal (meeting...)
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// End date for current deal (meeting...)
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Theme of the deal (meeting...)
        /// </summary>
        public string Theme { get; set; }
        
        /// <summary>
        /// Place for the meeting (deal...)
        /// </summary>
        public string Place { get; set; }

        /// <summary>
        /// Indicates whether record was deleted or not
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Indicates whether record was completed or not
        /// </summary>
        public bool IsComplete { get; set; }

        /// <summary>
        /// Mark when record was created
        /// </summary>
        public DateTime CreatedAt { get; set; }

        // Navigation property

        /// <summary>
        /// Represent type of record 
        /// </summary>
        public RecordType RecordType { get; set; }

        /// <summary>
        /// Represent relationship with contact
        /// </summary>
        public virtual ICollection<RecordsToContacts> RecordsToContacts { get; set; }
    }
}
