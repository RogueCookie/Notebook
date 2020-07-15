using Microsoft.EntityFrameworkCore;
using Notebook.Domain.Entity;
using System.Linq;

namespace Notebook.Database.Extension
{
    public static class ContactExtensions
    {
        /// <summary>
        /// Get ordered list of contacts
        /// </summary>
        /// <param name="contacts"></param>
        /// <returns></returns>
        public static IQueryable<Contact> GetOrderedContacts(this DbSet<Contact> contacts)
        {
            return contacts
                .Include(contactInfo => contactInfo.CollectionInformations)
                .OrderBy(x => x.FirstName);
        }
    }
}
