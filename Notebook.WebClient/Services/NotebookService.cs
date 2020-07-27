using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Notebook.Database;
using Notebook.Database.Extension;
using Notebook.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Notebook.WebClient.Services
{
    /// <summary>
    /// Manipulation with records in notebook
    /// </summary>
    public class NotebookService 
    {
        private readonly NotebookDbContext _context;
        private readonly ILogger<NotebookService> _logger;

        public NotebookService(NotebookDbContext context, ILogger<NotebookService> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Add new records in notebook
        /// </summary>
        /// <param name="newRecord">New record</param>
        /// <returns>No object or value is returned by this method when it completes</returns>
        public async Task<long> AddRecordAsync(Record newRecord)
        {
            try
            {
                newRecord.CreatedAt = DateTime.Now.Date;
                await _context.AddAsync(newRecord);
                await _context.SaveChangesAsync();
                _logger.LogError($"New record was added with Id {newRecord.Id}");
                return newRecord.Id;
            }
            catch (Exception exception)
            {
                _logger.LogError($"Cannot add record with Id {newRecord.Id}", exception);
                throw;
            }
            
        }

        /// <summary>
        /// Get all not deleted records
        /// </summary>
        /// <param name="from">Time range from current date</param>
        /// <param name="to">Time range till current date</param>
        /// <returns>List of not deleted records in some time range</returns>
        public async Task<List<Record>> GetAllNotDeletedRecordsAsync(DateTime? from, DateTime? to)
        {
            return await _context.Records
                .GetNotDeleteRecords().ToListAsync();
        }

        /// <summary>
        /// Get record with particular Id
        /// </summary>
        /// <param name="recordId">Id of record</param>
        /// <returns>Record entity</returns>
        public async Task<Record> GetRecordByIdAsync(long recordId)
        {
            return await _context.Records
                .FirstOrDefaultAsync(x => x.Id == recordId);
        }

        /// <summary>
        /// Mark record as completed
        /// </summary>
        /// <param name="recordId">Id of record</param>
        /// <param name="isCompleted">Indicates if the record is completed or not</param>
        /// <returns>Whether the entity was successfully marked or not</returns>
        public async Task<bool> MarkRecordAsCompletedAsync(long recordId, bool isCompleted)
        {
            try
            {
                var item = await GetRecordByIdAsync(recordId);
                item.IsComplete = isCompleted;
                //UpdateRecord(item);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Record with Id {recordId} was successfully mark as Completed = {isCompleted}");
                return true;                                
            }
            catch (Exception exception)
            {
                _logger.LogError($"Cannot mart record with Id {recordId}", exception);
                return false;
            }
        }

        /// <summary>
        /// Mark record as deleted
        /// </summary>
        /// <param name="recordId">Id of records</param>
        /// <returns>Whether the entry was successfully deleted or not</returns>
        public async Task<bool> DeletedRecordAsync(long recordId)
        {
            try
            {
                var item = await GetRecordByIdAsync(recordId);
                item.IsDeleted = true;
                //UpdateRecord(item);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Record with id {recordId} was successfully deleted");
                return true;
            }
            catch (Exception exception)
            {
                _logger.LogError($"Cannot delete record with id: {recordId} ", exception);
                return false;
            }
        }

        /// <summary>
        /// Update existing record
        /// </summary>
        /// <param name="record">Record entity for updating</param>
        /// <returns>Whether the entry was successfully updated or not</returns>
        public async Task<bool> UpdateRecordAsync(Record record)
        {
            try
            {
                //var recordFromService =  _context.Records.GetNotDeleteRecords().FirstOrDefault(x => x.Id == record.Id);
                _context.Update(record);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Record with id {record.Id} was successfully updated");
                return true;
            }
            catch (Exception exception)
            {
                _logger.LogError($"Cannot update record with id: {record.Id} ", exception);
                return false;
            }
            
        }
    }
}
