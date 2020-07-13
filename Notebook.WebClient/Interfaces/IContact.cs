using Notebook.Domain.Entity;
using System.Collections.Generic;

namespace Notebook.WebClient.Interfaces
{
    /// <summary>
    /// Manipulation with DB
    /// </summary>
    public interface IContact
    {
        /// <summary>
        /// Get all exist contacts
        /// </summary>
        /// <returns>List of contacts</returns>
        IEnumerable<Contact> GetAllContacts();

        /// <summary>
        /// Get contact with particular Id
        /// </summary>
        /// <param name="contactId">Id of contact</param>
        /// <returns>Particular contact</returns>
        Contact GetContactById(long contactId);

        /// <summary>
        /// Get all information for current contact 
        /// </summary>
        /// <param name="contactId">Id of current contact</param>
        /// <returns>List of Contact information for current contact</returns>
        IEnumerable<ContactInformation> GetAllInfoForContact(long contactId);

        /// <summary>
        /// Add new contact
        /// </summary>
        /// <param name="newContact">new contact</param>
        void Add(Contact newContact);

        /// <summary>
        /// Add new information for contact
        /// </summary>
        /// <param name="newContactInformation">New information about current contact</param>
        void AddContactInformation(ContactInformation newContactInformation);

        /// <summary>
        /// Remove contact with all correlate information about him
        /// </summary>
        /// <param name="contactId">Current id of the contact</param>
        void RemoveContact(long contactId);
    }
}
