using Microsoft.EntityFrameworkCore;
using Notebook.Domain.Entity;
using System.Linq;

namespace Notebook.Database.Extension
{
    /// <summary>
    /// Extension methods for Record data
    /// </summary>
    public static class RecordExtension
    {
        /// <summary>
        /// Request record from Db which will be handled by IQueryable in Db backed level
        /// </summary>
        /// <param name="records">Record from database</param>
        /// <returns>List of not deleted records</returns>
        public static IQueryable<Record> GetNotDeleteRecords(this DbSet<Record> records)
        {
            return records
                .Include(x => x.RecordType)
                .Where(x => x.IsDeleted == false )
                .OrderByDescending(x => x.CreatedAt);
        }
    }
}