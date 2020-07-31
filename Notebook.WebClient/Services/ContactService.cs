using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Notebook.Database;
using Notebook.Database.Extension;
using Notebook.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Notebook.DTO.Models.Request;

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
                _logger.LogError($"Cannot add contact with name {newContact.FirstName} with id {newContact.Id}",
                    exception);
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
                _logger.LogInformation(
                    $"New information added to contact with name {newContactInformation.Contact.FirstName}");
                return newContactInformation.ContactId;
            }
            catch (Exception exception)
            {
                _logger.LogError($"Cannot add information for contact {newContactInformation.Contact.FirstName}",
                    exception);
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
                    _logger.LogError($"Cannot add list information to contact {contactInfo.Contact.FirstName}",
                        exception);
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
        public async Task<List<ContactCreateModel>> GetAllContactsAsync()
        {
            return await _context.Contacts
                .GetOrderedContacts().Select(x => new ContactCreateModel()
                {
                    FirstName = x.FirstName,
                    BirthDate = x.BirthDate //TODO
                }).ToListAsync();
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
                    .Include(info => info.CollectionInformations) // TODo так оно удалит информатин в совокупе?
                    .FirstOrDefaultAsync(x => x.Id == contactId);

                if (contact != null)
                {
                    _context.Remove(contact);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation(
                        $"Contact with id {contactId} was successfully removed with all correlated contact information");
                }

                return true;
            }
            catch (Exception exception)
            {
                _logger.LogError($"Cannot remove contact with id {contactId}", exception);
                return false;
            }
        }

        /// <summary>
        /// Get current record contact information
        /// </summary>
        /// <param name="contactInfoId">Id of contact information</param>
        /// <returns>Contact Information entity</returns>
        public async Task<ContactInformation> GetCurrentContactInformationAsync(long contactInfoId)
        {
            try
            {
                return await _context.ContactInformations.FirstOrDefaultAsync(x => x.Id == contactInfoId);
            }
            catch (Exception exception)
            {
                _logger.LogError($"Couldn't find information  with id {contactInfoId}", exception);
                throw;
            }
        }

        /// <summary>
        /// Remove particular contact information 
        /// </summary>
        /// <param name="contInfoId">Id of information record</param>
        /// <returns>Whether the contact information deleted successfully</returns>
        public async Task<bool> RemoveCurrentContactInformation(long contInfoId)
        {
            try
            {
                var currentInformation = await GetCurrentContactInformationAsync(contInfoId);
                if (currentInformation == null)
                {
                    _logger.LogInformation($"Contact information with id {contInfoId} wasn't found");
                    return false;
                }

                _context.Remove(currentInformation);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Contact information with Id {contInfoId} for contact {currentInformation.Contact.FirstName} was successfully removed");
                return true;

            }
            catch (Exception exception)
            {
                _logger.LogError($"Unable to remove contact information with Id {contInfoId}", exception);
                return false;
            }
        }
    }
}
