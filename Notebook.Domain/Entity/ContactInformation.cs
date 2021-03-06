﻿namespace Notebook.Domain.Entity
{
    /// <summary>
    /// Represents contact information for call
    /// </summary>
    public class ContactInformation
    {
        /// <summary>
        /// Id of the contact
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Phone number of the contact
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Email of the contact
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Skype of the contact
        /// </summary>
        public string Skype { get; set; }

        /// <summary>
        /// Additional information
        /// </summary>
        public string Other { get; set; }

        //navigation prop

        /// <summary>
        /// Id of the contact
        /// </summary>
        public long ContactId { get; set; }

        /// <summary>
        /// Reference to contact data
        /// </summary>
        public Contact Contact { get; set; }
    }
}
