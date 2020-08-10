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
                _logger.LogInformation($"Contact with name {newContact.FirstName} was successfully added with Id {adaptToEntity.Id}");
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
        /// Add contact new information about contact
        /// </summary>
        /// <param name="newContactInformation">New information entity about contact</param>
        /// <returns>Id of contact to whom was added new information</returns>
        public async Task<long> AddContactInformationAsync(ContactInformationRequestModel newContactInformation)
        {
            try
            {
                var adaptedModelToEntity = newContactInformation.AdaptToContactInfo();
                await _context.ContactInformations.AddAsync(adaptedModelToEntity);
                await _context.SaveChangesAsync();
                _logger.LogInformation(
                    $"New information added to contact with id {adaptedModelToEntity.ContactId}");
                return newContactInformation.ContactId;
            }
            catch (Exception exception)
            {
                _logger.LogError($"Cannot add information for contact {newContactInformation.ContactId}",
                    exception);
                throw;
            }
        }

        /// <summary>
        /// Add list of contact information
        /// </summary>
        /// <param name="newContactsInformation"></param>
        /// <returns>Whether list of info added successfully or not</returns>
        public async Task<bool> AddBulkContactInformationAsync(IEnumerable<ContactInformationRequestModel> newContactsInformation)
        {
            try
            {
                foreach (var contactInfo in newContactsInformation)
                {
                    var adaptedModelToEntity = contactInfo.AdaptToContactInfo();
                    await _context.ContactInformations.AddAsync(adaptedModelToEntity);
                    await _context.SaveChangesAsync();
                }

                return true;
            }
            catch (Exception exception)
            {
                foreach (var contactInfo in newContactsInformation)
                {
                    _logger.LogError($"Cannot add list information to contact {contactInfo.ContactId}",
                        exception);
                }

                return false;
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
                    //.Include(x => x.CollectionInformations)
                    .FirstOrDefaultAsync(contact => contact.Id == contactId);
                if (contact != null)
                {
                    var result = contact.AdaptToContactCreateModel();
                    return result;
                }
                return  new ContactCreateModel();
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
        public async Task<AddNewContact> GetContactWithIdAsync(long contactId)
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
        public async Task<List<AddNewContact>> GetAllContactsAsync()
        {
            var contacts = await _context.Contacts
                .GetOrderedContacts().ToListAsync();
            var result = contacts.Select(x => x.AdaptToAddNewContactModel()).ToList();
            return result;
        }

        /// <summary>
        /// Get all contact information for current contact
        /// </summary>
        /// <param name="contactId">Id of contact</param>
        /// <returns>List of information for particular contact</returns>
        public async Task<List<ContactInformationResponseModel>> GetAllInfoForContactAsync(long contactId)
        {
            var info = await _context.ContactInformations
                .Where(x => x.ContactId == contactId)
                .ToListAsync();
            var result = info.Select(x => x.AdaptToContactInformationResponseModel()).ToList();
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
        /// Get current record contact information
        /// </summary>
        /// <param name="contactInfoId">Id of contact information</param>
        /// <returns>Contact Information entity</returns>
        public async Task<ContactInformationRequestModel> GetCurrentContactInformationRequestAsync(long contactInfoId)
        {
            try
            {
               var infoFromDb =  await _context.ContactInformations.FirstOrDefaultAsync(x => x.Id == contactInfoId);
               var result = infoFromDb.AdaptToContactInformationRequestModel();
               return result;
            }
            catch (Exception exception)
            {
                _logger.LogError($"Couldn't find information  with id {contactInfoId}", exception);
                throw;
            }
        }

        /// <summary>
        /// Get contactInformationResponse model
        /// </summary>
        /// <param name="contactInfoId">Id of contact info</param>
        /// <returns></returns>
        public async Task<ContactInformationResponseModel> GetCurrentContactInformationResponseAsync(long contactInfoId)
        {
            try
            {
                var infoFromDb = await _context.ContactInformations.FirstOrDefaultAsync(x => x.Id == contactInfoId);
                var result = infoFromDb.AdaptToContactInformationResponseModel();
                return result;
            }
            catch (Exception exception)
            {
                _logger.LogError($"Couldn't find information  with id {contactInfoId}", exception);
                throw;
            }
        }


        /// <summary>
        /// Update information
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ContactInformationResponseModel> UpdateContactInformation(ContactInformationResponseModel model)
        {
            try
            {
                var info = await _context.ContactInformations.FirstOrDefaultAsync(x => x.Id == model.Id);
                info.Id = model.Id;
                info.ContactId = model.ContactId;
                info.PhoneNumber = model.PhoneNumber;
                info.Email = model.Email;
                info.Skype = model.Skype;
                info.Other = model.Other;

                _context.ContactInformations.Update(info);
                await _context.SaveChangesAsync();

                var changedModel = info.AdaptToContactInformationResponseModel();
                return changedModel;
            }
            catch (Exception exception)
            {
                _logger.LogError($"Couldn't update information with id {model.Id}", exception);
                throw;
            }
        }

        /// <summary>
        /// Update particular contact
        /// </summary>
        /// <param name="contact">Entity for updates</param>
        /// <returns>Updated model</returns>
        public async Task<ContactCreateModel> UpdateContact(AddNewContact contact)
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

        /// <summary>
        /// Remove particular contact information 
        /// </summary>
        /// <param name="contInfoId">Id of information record</param>
        /// <returns>Whether the contact information deleted successfully</returns>
        public async Task<bool> RemoveCurrentContactInformationAsync(long contInfoId)
        {
            try
            {
                var currentInformation = await _context.ContactInformations.FirstOrDefaultAsync(x => x.Id == contInfoId);
                if (currentInformation == null)
                {
                    _logger.LogInformation($"Contact information with id {contInfoId} wasn't found");
                    return false;
                }

                _context.ContactInformations.Remove(currentInformation);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Contact information with Id {contInfoId} for contact was successfully removed");
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
