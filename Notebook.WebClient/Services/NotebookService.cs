using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Notebook.Database;
using Notebook.Database.Extension;
using Notebook.Domain.Entity;
using Notebook.DTO.Models.Request;
using Notebook.DTO.Models.Response;
using Notebook.WebClient.Extension;
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
        public async Task<long> AddRecordAsync(NoteCreateModel newRecord)
        {
            try
            {
                var adaptModelToEntity = newRecord.AdaptToRecord();
                adaptModelToEntity.CreatedAt = DateTime.Now.Date;
                await _context.Records.AddAsync(adaptModelToEntity);
                await _context.SaveChangesAsync();

                _logger.LogError($"New record was added with Id {adaptModelToEntity.Id}");
                return adaptModelToEntity.Id;
            }
            catch (Exception exception)
            {
                _logger.LogError($"Cannot add record with model data at {DateTime.Now}", exception);
                throw;
            }
        }

        /// <summary>
        /// Get all not deleted records
        /// </summary>
        /// <param name="from">Time range from current date</param>
        /// <param name="to">Time range till current date</param>
        /// <returns>List of not deleted records in some time range</returns>
        public async Task<List<NoteCreateResponseModel>> GetAllNotDeletedRecordsAsync(DateTime? from, DateTime? to)
        {
            var notes = await _context.Records
                .GetNotDeleteRecords().OrderByDescending(x => x.StartDate).ToListAsync(); //TODO check how filter works
            var adaptToCreateModel = notes.Select(x => x.AdaptToNoteCreateResponseModel()).ToList();
            return adaptToCreateModel;
        }

        /// <summary>
        /// Get record with particular Id
        /// </summary>
        /// <param name="recordId">Id of record</param>
        /// <returns>Record entity</returns>
        public async Task<NoteCreateModel> GetRecordByIdAsync(long recordId)
        {
            var noteFromDb = await _context.Records.GetNotDeleteRecords()
                .FirstOrDefaultAsync(x => x.Id == recordId);
            var adaptToModel = noteFromDb.AdaptToNoteCreateModel();
            return adaptToModel;
        }

        /// <summary>
        /// Mark record as completed
        /// </summary>
        /// <param name="recordId">Id of record</param>
        /// <param name="isCompleted">Indicates if the record is completed or not</param>
        /// <returns>Whether the entity was successfully marked or not</returns>
        public async Task<NoteCreateModel> MarkRecordAsCompletedAsync(long recordId, bool isCompleted)
        {
            try
            {
                var item = await _context.Records.FirstOrDefaultAsync(x => x.Id == recordId);
                item.IsComplete = isCompleted;
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Record with Id {recordId} was successfully mark as Completed = {isCompleted}");
                 
                return item.AdaptToNoteCreateModel();                                
            }
            catch (Exception exception)
            {
                _logger.LogError($"Cannot mart record with Id {recordId}", exception);
                throw;
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
                var item = await _context.Records.GetNotDeleteRecords()
                    .FirstOrDefaultAsync(x => x.Id == recordId);
               
                if (item != null)
                {
                    item.IsDeleted = true;
                    _context.Records.Update(item);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"Record with id {recordId} was successfully marked as deleted");
                    return true;
                }

                return false; 
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
        /// <param name="record">Record for updating</param>
        /// <returns>Updated record</returns>
        public async Task<NoteCreateModel> UpdateRecordAsync(NoteCreateResponseModel record)
        {
            try
            {
                var recordFromBase = await _context.Records.GetNotDeleteRecords().FirstOrDefaultAsync(x => x.Id == record.Id);
                if (recordFromBase == null)
                {
                    return null;
                }
                recordFromBase.StartDate = record.StartDate;
                recordFromBase.IsComplete = record.IsComplete;
                recordFromBase.EndDate = record.EndDate;
                recordFromBase.Place = record.Place;
                recordFromBase.Theme = record.Theme;
                recordFromBase.CreatedAt = recordFromBase.CreatedAt;
                recordFromBase.RecordTypeId = record.RecordTypeId; 

                await _context.SaveChangesAsync();
                _logger.LogInformation($"Record with id {record.Id} was successfully updated");
                var adaptToModel = recordFromBase.AdaptToNoteCreateModel();
                return adaptToModel;
            }
            catch (Exception exception)
            {
                _logger.LogError($"Cannot update record with id: {record.Id} ", exception);
                throw;
            }
        }

        /// <summary>
        /// Update list of contacts assigned to current note (meeting)
        /// </summary>
        /// <param name="noteId">Id of current notes</param>
        /// <param name="contactIds">Note with new contacts</param>
        /// <returns>Note with updated model</returns>
        public async Task<bool> UpdateContactsForNoteAsync(long noteId, ICollection<long> contactIds)
        {
            try
            {
                var exist = await _context.RecordsToContacts.Where(c => c.RecordId == noteId).Select(x => x.ContactId).ToListAsync();

                // get all contact ids which exist in Db for current cote and transform it to type RecordToContact for being able to use AddRange
                var contactForAdd = contactIds.Except(exist).Select(x => new RecordsToContacts() { ContactId = x, RecordId = noteId}).ToList();

                // get contact ids which was removed in front-end and now should be removed from Db
                var contactForDelete = exist.Except(contactIds).ToList();

                if (contactForAdd.Any())
                {
                    await _context.RecordsToContacts.AddRangeAsync(contactForAdd);
                    await _context.SaveChangesAsync();
                }

                if (contactForDelete.Any())
                {
                    var needToDel = await  _context.RecordsToContacts.Where(x => contactForDelete.Contains(x.ContactId) && x.RecordId == noteId).ToListAsync();
                    _context.RecordsToContacts.RemoveRange(needToDel);
                    await _context.SaveChangesAsync();
                }

                return true;
            }
            catch (Exception exception)
            {
                _logger.LogError($"Cannot update contact list for record with id: {noteId}", exception);
                return false;
            }
        }
    }
}
