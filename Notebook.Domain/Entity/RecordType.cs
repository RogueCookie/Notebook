using System.Collections;
using System.Collections.Generic;
using Notebook.Domain.Enum;

namespace Notebook.Domain.Entity
{
    /// <summary>
    /// Describes record type information
    /// </summary>
    public class RecordType
    {
        /// <summary>
        /// Id or record
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Name of record
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Type of record (from enum)
        /// </summary>
        public RecordTypeEnum Alias { get; set; }

        /// <summary>
        /// Description for record 
        /// </summary>
        public string Description { get; set; }

        public virtual ICollection<Record> Records { get; set; }
    }
}