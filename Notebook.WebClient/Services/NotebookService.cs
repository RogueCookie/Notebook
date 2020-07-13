using Notebook.Database;
using Notebook.Domain.Entity;
using Notebook.WebClient.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Notebook.WebClient.Services
{
    public class NotebookService : INotebook
    {
        private readonly NotebookDbContext _context;
        public NotebookService(NotebookDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <inheritdoc />
        public void AddRecord(Record newRecord)
        {
            _context.Add(newRecord);
            _context.SaveChanges();
        }

        /// <inheritdoc />
        public IEnumerable<Record> GetAllRecords()
        {
            return _context.Records
                .Where(x => x.IsDeleted == false && x.IsComplete == false);
        }

        /// <inheritdoc />
        public Record GetRecordById(long recordId)
        {
            return GetAllRecords()
                .FirstOrDefault(x => x.Id == recordId);
        }

        /// <inheritdoc />
        public void MarkRecordAsCompleted(long recordId, bool isCompleted)
        {
            var item = GetRecordById(recordId);
            item.IsComplete = isCompleted;
            UpdateRecord(item);
        }

        /// <inheritdoc />
        public void UpdateRecord(Record record)
        {
            _context.Records.Remove(record);
            _context.Records.Update(record);
            _context.SaveChanges();
        }
    }
}
