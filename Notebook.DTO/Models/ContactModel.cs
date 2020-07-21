using Notebook.DTO.Models.Request;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Notebook.DTO.Models
{
    /// <summary>
    /// Describes main information about contact
    /// </summary>
    public abstract class ContactModel : BaseModel
    {
        /// <summary>
        /// First name of the contact
        /// </summary>
        [Required]
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
        [DataType(DataType.Date)]
        public DateTime? BirthDate { get; set; }

        /// <summary>
        /// Name of Organization where contact work
        /// </summary>
        public string OrganizationName { get; set; }

        /// <summary>
        /// Position which contact has in organization
        /// </summary>
        public string Position { get; set; }

        /// <summary>
        /// Collection information correlated to current contact (personal call data)
        /// </summary>
        public virtual ICollection<ContactInformationModel> CollectionInformations { get; set; }
    }
}