using Notebook.Domain.Entity;
using Notebook.DTO.Models.Request;
using Notebook.DTO.Models.Response;
using System.Linq;

namespace Notebook.WebClient.Extension
{
    /// <summary>
    /// Helpers allow transfer entities to Model and back
    /// </summary>
    public static class AdaptNoteExtension
    {
        /// <summary>
        /// Adapt record to NoteCreate model
        /// </summary>
        /// <param name="record">Record entity</param>
        /// <returns>Adapted NoteCreate model</returns>
        public static NoteCreateResponseModel AdaptToNoteCreateResponseModel(this Record record)
        {
            var result = new NoteCreateResponseModel
            {
                Id = record.Id,
                Place = record.Place,
                Theme = record.Theme,
                StartDate = record.StartDate,
                EndDate = record.EndDate
            };
            return result;
        }

        /// <summary>
        /// Adapt to NoteCreate model
        /// </summary>
        /// <param name="record">Record entity</param>
        /// <returns></returns>
        public static NoteCreateModel AdaptToNoteCreateModel(this Record record)
        {
            var result = new NoteCreateModel
            {
                Place = record.Place,
                Theme = record.Theme,
                StartDate = record.StartDate,
                EndDate = record.EndDate,
                RecordTypeId = record.RecordTypeId,
                ContactIds = record.RecordsToContacts?.Select(x => x.ContactId).ToList()
            };
            return result;
        }

        /// <summary>
        /// Adapt to Record entity
        /// </summary>
        /// <param name="model">NoteCreate model</param>
        /// <returns>Adapted entity</returns>
        public static Record AdaptToRecord(this NoteCreateModel model)
        {
            var result = new Record
            {
                RecordTypeId =  model.RecordTypeId,
                Place = model.Place,
                Theme = model.Theme,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                IsComplete = model.IsComplete,
            };
            return result;
        }
    }
}