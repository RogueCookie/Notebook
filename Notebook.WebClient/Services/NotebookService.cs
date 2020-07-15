using Notebook.Database;
using Notebook.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Notebook.WebClient.Services
{
    public class NotebookService 
    {
        private readonly NotebookDbContext _context;
        public NotebookService(NotebookDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public void AddRecord(Record newRecord)
        {
            _context.Add(newRecord);
            _context.SaveChanges();
        }

        public IEnumerable<Record> GetAllRecords()
        {
            return _context.Records
                .Where(x => x.IsDeleted == false && x.IsComplete == false);
        }

        public Record GetRecordById(long recordId)
        {
            return GetAllRecords()
                .FirstOrDefault(x => x.Id == recordId);
        }

        public void MarkRecordAsCompleted(long recordId, bool isCompleted)
        {
            var item = GetRecordById(recordId);
            item.IsComplete = isCompleted;
            UpdateRecord(item);
        }

        public void MarkRecordAsDeleted(long recordId)
        {
            var item = GetRecordById(recordId);
            item.IsComplete = true;
            UpdateRecord(item);
        }

        public void UpdateRecord(Record record)
        {
            _context.Records.Remove(record);
            _context.Records.Update(record);
            _context.SaveChanges();
        }
    }
}
