using Microsoft.EntityFrameworkCore;
using Notebook.Domain.Entity;
using System.Linq;

namespace Notebook.Database.Extension
{
    /// <summary>
    /// Extension methods for Contact data
    /// </summary>
    public static class ContactExtensions
    {
        /// <summary>
        /// Get ordered list of contacts
        /// </summary>
        /// <param name="contacts">Contacts from database"</param>
        /// <returns>Ordered list of contacts</returns>
        public static IQueryable<Contact> GetOrderedContacts(this DbSet<Contact> contacts)
        {
            return contacts
                .Include(contactInfo => contactInfo.CollectionInformations)
                .OrderBy(x => x.FirstName);
        }


        
    }
}
