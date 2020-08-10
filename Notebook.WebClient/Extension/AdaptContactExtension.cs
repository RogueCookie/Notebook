using Notebook.Domain.Entity;
using Notebook.DTO.Models.Request;
using Notebook.DTO.Models.Response;

namespace Notebook.WebClient.Extension
{
    /// <summary>
    /// Helpers allow transfer entity to Model and back
    /// </summary>
    public static class AdaptContactExtension
    {
        /// <summary>
        /// Adapt Contact entity to ContactCreateModel
        /// </summary>
        /// <param name="contact">Contact entity</param>
        /// <returns>Adapted model</returns>
        public static ContactCreateModel AdaptToContactCreateModel(this Contact contact)
        {
            var result = new ContactCreateModel()
            {
                FirstName = contact.FirstName,
                LastName = contact.LastName,
                Patronymic = contact.Patronymic,
                BirthDate = contact.BirthDate,
                OrganizationName = contact.OrganizationName,
                Position = contact.Position
            };
            return result;
        }

        /// <summary>
        /// Adapt contact entity to AddNewContact model
        /// </summary>
        /// <param name="model">Contact entity</param>
        /// <returns>Adapted model</returns>
        public static AddNewContact AdaptToAddNewContactModel(this Contact model)
        {
            var result = new AddNewContact()
            {
                Id = model.Id,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Patronymic = model.Patronymic,
                BirthDate = model.BirthDate,
                OrganizationName = model.OrganizationName,
                Position = model.Position
            };
            return result;
        }

        /// <summary>
        /// Adapt contactCreate model to entity
        /// </summary>
        /// <param name="model">New contactCreateModel</param>
        /// <returns>Contact Entity</returns>
        public static Contact AdaptToContact(this ContactCreateModel model)
        {
            var result = new Contact()
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Patronymic = model.Patronymic,
                BirthDate = model.BirthDate,
                OrganizationName = model.OrganizationName,
                Position = model.Position
            };
            return result;
        }

        /// <summary>
        /// Adapt information entity to ContactInformationResponse Model
        /// </summary>
        /// <param name="info">ContactInformation entity</param>
        /// <returns>Adapt model</returns>
        public static ContactInformationResponseModel AdaptToContactInformationResponseModel(this ContactInformation info)
        {
            var result = new ContactInformationResponseModel()
            {
                Id = info.Id,
                Email = info.Email,
                PhoneNumber = info.PhoneNumber,
                Skype = info.Skype,
                Other = info.Other,
                ContactId = info.ContactId
            };
            return result;
        }

        /// <summary>
        /// Adapt information entity to ContactInformationRequest Model
        /// </summary>
        /// <param name="info">ContactInformation entity</param>
        /// <returns>Adapt model</returns>
        public static ContactInformationRequestModel AdaptToContactInformationRequestModel(this ContactInformation info)
        {
            var result = new ContactInformationRequestModel() 
            {
                Email = info.Email,
                PhoneNumber = info.PhoneNumber,
                Skype = info.Skype,
                Other = info.Other,
                ContactId = info.ContactId
            };
            return result;
        }

        /// <summary>
        /// Adapt information model to entity
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static ContactInformation AdaptToContactInfo(this ContactInformationRequestModel model)
        {
            var result = new ContactInformation()
            {
                PhoneNumber = model.PhoneNumber,
                Email = model.Email,
                Skype = model.Skype,
                Other = model.Other,
                ContactId = model.ContactId
            };
            return result;
        }
    }
}