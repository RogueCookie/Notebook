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
    /// Manipulation with contact entities
    /// </summary>
    public class ContactService 
    {
        private readonly NotebookDbContext _context;
        private readonly ILogger<ContactService> _logger;

        public ContactService(NotebookDbContext context, ILogger<ContactService> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Add new Contact
        /// </summary>
        /// <param name="newContact">New contact</param>
        /// <returns>Id of the new added contact</returns>
        public async Task<long> AddContactAsync(Contact newContact)
        {
            try
            {
                await _context.AddAsync(newContact);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Contact with name {newContact.FirstName} was successfully added");
                return newContact.Id;
            }
            catch (Exception exception)
            {
                _logger.LogError($"Cannot add contact with name {newContact.FirstName} with id {newContact.Id}", exception);
                throw;
            }
        }

        /// <summary>
        /// Add contact new information about contact
        /// </summary>
        /// <param name="newContactInformation">New information entity about contact</param>
        /// <returns>Id of contact to whom was added new information</returns>
        public async Task<long> AddContactInformationAsync(ContactInformation newContactInformation)      
        {
            try
            {
                await _context.AddAsync(newContactInformation);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"New information added to contact with name {newContactInformation.Contact.FirstName}");
                return newContactInformation.ContactId;
            }
            catch (Exception exception)
            {
                _logger.LogError($"Cannot add information for contact {newContactInformation.Contact.FirstName}", exception);
                throw;
            }
        }

        /// <summary>
        /// Add list of contact information
        /// </summary>
        /// <param name="newContactsInformation"></param>
        /// <returns></returns>
        public async Task<bool> AddBulkContactInformationAsync(IEnumerable<ContactInformation> newContactsInformation)
        {
            try
            {
                foreach (var contactInfo in newContactsInformation)
                {
                    await _context.AddAsync(contactInfo);
                    await _context.SaveChangesAsync();
                }
               
                return true;
            }
            catch (Exception exception)
            {
                foreach (var contactInfo in newContactsInformation)
                {
                    _logger.LogError($"Cannot add list information to contact {contactInfo.Contact.FirstName}", exception);
                }
                return false;
            }
            
        }

        /// <summary>
        /// Get contact with particular Id
        /// </summary>
        /// <param name="contactId">Id of contact</param>
        /// <returns>Contact entity with particular ID</returns>
        public async Task<Contact> GetContactByIdAsync(long contactId)
        {
            return await _context.Contacts
                .Include(x => x.CollectionInformations)
                .FirstOrDefaultAsync(contact => contact.Id == contactId);
        }

        /// <summary>
        /// Get ordered contacts
        /// </summary>
        /// <returns>List of ordered contacts</returns>
        public async Task<List<Contact>> GetAllContactsAsync()
        {
            return await _context.Contacts
                .GetOrderedContacts().ToListAsync();
        }

        /// <summary>
        /// Get all contact information for current contact
        /// </summary>
        /// <param name="contactId">Id of contact</param>
        /// <returns>List of information for particular contact</returns>
        public async Task<List<ContactInformation>> GetAllInfoForContactAsync(long contactId)
        {
            return await _context.ContactInformations
                .Where(x => x.ContactId == contactId)
                .ToListAsync();
        }

        /// <summary>
        /// Remove contact with all correlate to him contact information
        /// </summary>
        /// <param name="contactId">Id of contact</param>
        public async Task<bool> RemoveContactAsync(long contactId)
        {
            try
            {
                var contact = await _context.Contacts
                    .Include(info => info.CollectionInformations)// TODo так оно удалит информатин в совокупе?
                    .FirstOrDefaultAsync(x => x.Id == contactId);

                if (contact != null)
                {
                    _context.Remove(contact);     
                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"Contact with id {contactId} was successfully removed with all correlated contact information");
                }
                return true;
            }
            catch (Exception exception)
            {
                _logger.LogError($"Cannot remove contact with id {contactId}", exception);
                return false;
            }
            
        }
    }
}
