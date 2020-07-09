using System;

namespace Notebook.Domain.Entity
{
    /// <summary>
    /// Base information about all contacts
    /// </summary>
    public class Contact
    {
        /// <summary>
        /// ID of current records
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// User ID
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// First name of contact
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Last Name of contact
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Patronymic of the contact
        /// </summary>
        public string Patronymic { get; set; }

        /// <summary>
        /// Date of the birth
        /// </summary>
        public DateTime? BirthDate { get; set; }

        /// <summary>
        /// Name of organization
        /// </summary>
        public string OrganizationName { get; set; }

        /// <summary>
        /// Current position in organization
        /// </summary>
        public string Position { get; set; }
    }
}
