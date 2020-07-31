using System;
using System.Collections.Generic;

namespace Notebook.Domain.Entity
{
    /// <summary>
    /// Base information about all contacts
    /// </summary>
    public class Contact
    {
        /// <summary>
        /// Id of the contact
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// First name of the contact
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Last name of the contact
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Patronymic of the contact("otchestvo")
        /// </summary>
        public string Patronymic { get; set; }

        /// <summary>
        /// Birth date of the contact
        /// </summary>
        public DateTime? BirthDate { get; set; }

        /// <summary>
        /// Name of Organization where contact work
        /// </summary>
        public string OrganizationName { get; set; }

        /// <summary>
        /// Position which contact has in organization
        /// </summary>
        public string Position { get; set; }

        // Navigation property

        /// <summary>
        /// Collection information correlated to current contact (personal call data)
        /// </summary>
        public virtual ICollection<ContactInformation> CollectionInformations  { get; set; }

        /// <summary>
        /// Records correlated to current contact (relationship between tables one to many)
        /// </summary>
        public virtual ICollection<RecordsToContacts> RecordsToContacts { get; set; }
    }
}
