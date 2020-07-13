using Notebook.Domain.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Notebook.WebClient.Models
{
    public class ContactCreateModel
    {
        public long Id { get; set; }

        [Required]
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Patronymic { get; set; }
        public DateTime? BirthDate { get; set; }
        public string OrganizationName { get; set; }
        public string Position { get; set; }
        public virtual ICollection<ContactInformation> CollectionInformations { get; set; }
        public virtual ICollection<RecordsToContacts> RecordsToContacts { get; set; }
    }
}
