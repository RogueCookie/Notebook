using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Notebook.Database;
using Notebook.Database.Extension;
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
        /// <returns>Id of the new added contact entity</returns>
        public async Task<long> AddContactAsync(ContactCreateModel newContact)
        {
            try
            {
                var adaptToEntity = newContact.AdaptToContact();
                await _context.Contacts.AddAsync(adaptToEntity);
                await _context.SaveChangesAsync();
                _logger.LogInformation(
                    $"Contact with name {newContact.FirstName} was successfully added with Id {adaptToEntity.Id}");
                return adaptToEntity.Id;
            }
            catch (Exception exception)
            {
                _logger.LogError($"Cannot add contact with name {newContact.FirstName}",
                    exception);
                throw;
            }
        }

        /// <summary>
        /// Get contact by particular Id
        /// </summary>
        /// <param name="contactId">Id of contact</param>
        /// <returns>Contact entity with particular ID</returns>
        public async Task<ContactCreateModel> GetContactByIdAsync(long contactId)
        {
            try
            {
                var contact = await _context.Contacts
                    .Include(x => x.CollectionInformations)
                    .FirstOrDefaultAsync(contact => contact.Id == contactId);
                if (contact != null)
                {
                    var result = contact.AdaptToContactCreateModel();
                    return result;
                }

                return new ContactCreateModel();
                _logger.LogInformation($"Contact with id {contactId} is not exist");
            }
            catch (Exception exception)
            {
                _logger.LogError($"Cannot find contact with Id {contactId}", exception);
                throw;
            }

        }

        /// <summary>
        /// Get contact with particular Id
        /// </summary>
        /// <param name="contactId">Id of contact</param>
        /// <returns>Contact entity with particular ID</returns>
        public async Task<ResponseContact> GetContactWithIdAsync(long contactId)
        {
            var contact = await _context.Contacts
                .Include(x => x.CollectionInformations)
                .FirstOrDefaultAsync(contact => contact.Id == contactId);
            var result = contact.AdaptToAddNewContactModel();
            return result;
        }

        /// <summary>
        /// Get ordered contacts
        /// </summary>
        /// <returns>List of ordered contacts</returns>
        public async Task<List<ResponseContact>> GetAllContactsAsync()
        {
            var contacts = await _context.Contacts
                .GetOrderedContacts().ToListAsync();
            var result = contacts.Select(x => x.AdaptToAddNewContactModel()).ToList(); //TODO косяк с инфрматио адапт
            return result;
        }

        /// <summary>
        /// Remove contact with all correlate to him contact information
        /// </summary>
        /// <param name="contactId">Id of contact</param>
        /// <returns>Whether the contact deleted successfully or not</returns>
        public async Task<bool> RemoveContactAsync(long contactId)
        {
            try
            {
                var contact = await _context.Contacts
                    .Include(info => info.CollectionInformations) // TODo check ???
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
        /// Update particular contact
        /// </summary>
        /// <param name="contact">Entity for updates</param>
        /// <returns>Updated model</returns>
        public async Task<ContactCreateModel> UpdateContact(ResponseContact contact)
        {
            try
            {
                var newContact = await _context.Contacts.FirstOrDefaultAsync(x => x.Id == contact.Id);
                newContact.BirthDate = contact.BirthDate;
                newContact.FirstName = contact.FirstName;
                newContact.LastName = contact.LastName;
                newContact.Patronymic = contact.LastName;
                newContact.OrganizationName = contact.OrganizationName;
                newContact.Position = contact.Position;

                _context.Contacts.Update(newContact);
                await _context.SaveChangesAsync();
                var adaptModel = newContact.AdaptToContactCreateModel();
                return adaptModel;
            }
            catch (Exception exception)
            {
                _logger.LogError($"Couldn't update contact with id {contact.Id}", exception);
                throw;
            }
        }
    }
}
