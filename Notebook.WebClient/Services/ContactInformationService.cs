using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Notebook.Database;
using Notebook.DTO.Models.Request;
using Notebook.DTO.Models.Response;
using Notebook.WebClient.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Notebook.WebClient.Services
{
    public class ContactInformationService
    {
        private readonly NotebookDbContext _context;
        private readonly ILogger<ContactInformationService> _logger;

        public ContactInformationService(NotebookDbContext context, ILogger<ContactInformationService> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
        /// Get current record contact information
        /// </summary>
        /// <param name="contactInfoId">Id of contact information</param>
        /// <returns>Contact Information entity</returns>
        public async Task<ContactInformationRequestModel> GetCurrentContactInformationRequestAsync(long contactInfoId)
        {
            try
            {
                var infoFromDb = await _context.ContactInformations.FirstOrDefaultAsync(x => x.Id == contactInfoId);
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
    }
}
