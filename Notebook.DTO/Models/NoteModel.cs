using System;
using System.Collections.Generic;

namespace Notebook.DTO.Models
{
    /// <summary>
    /// Represent information about records in notebook
    /// </summary>
    public class NoteModel
    {
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
        /// Indicates whether record was completed or not
        /// </summary>
        public bool IsComplete { get; set; }

        /// <summary>
        /// Represent type of record 
        /// </summary>
        public long RecordTypeId { get; set; }

        /// <summary>
        /// Assigned people for current meeting
        /// </summary>
        public ICollection<long> ContactIds { get; set; }
    }
}