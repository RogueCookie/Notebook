using System.ComponentModel.DataAnnotations;

namespace Notebook.DTO.Models.Request
{
    /// <summary>
    /// Represents contact information for call
    /// </summary>
    public class ContactInformationModel : ContactModel
    {
        /// <summary>
        /// Phone number of the contact
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Email of the contact
        /// </summary>
        [EmailAddress]
        [Display(Name = "Email", Prompt = "example@gmail.com")]
        public string Email { get; set; }

        /// <summary>
        /// Skype of the contact
        /// </summary>
        public string Skype { get; set; }

        /// <summary>
        /// Additional information
        /// </summary>
        public string Other { get; set; }

       
        /// <summary>
        /// Id of the contact
        /// </summary>
        public long ContactId { get; set; } //TODO do we need it?
        
        /// <summary>
        /// Reference to contact data
        /// </summary>
        public ContactModel Contact { get; set; } //TODO do we need it?
    }
}