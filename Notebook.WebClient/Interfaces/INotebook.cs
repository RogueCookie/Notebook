using Notebook.Domain.Entity;
using System.Collections.Generic;

namespace Notebook.WebClient.Interfaces
{
    /// <summary>
    /// Manipulation with Records table
    /// </summary>
    public interface INotebook
    {
        /// <summary>
        /// Get current record
        /// </summary>
        /// <param name="recordId">Id of record</param>
        /// <returns>Particular record</returns>
        Record GetRecordById(long recordId);

        /// <summary>
        /// Get all existing records
        /// </summary>
        /// <returns>List of records</returns>
        IEnumerable<Record> GetAllRecords();

        /// <summary>
        /// Add new record
        /// </summary>
        /// <param name="newRecord"></param>
        void AddRecord(Record newRecord);

        /// <summary>
        /// Update current record
        /// </summary>
        /// <param name="record">Id  of record</param>
        void UpdateRecord(Record record);

        /// <summary>
        /// Update 
        /// </summary>
        /// <param name="recordId">Id of record</param>
        /// <param name="isCompleted">Current status of record</param>
        void MarkRecordAsCompleted(long recordId, bool isCompleted);
    }
}
