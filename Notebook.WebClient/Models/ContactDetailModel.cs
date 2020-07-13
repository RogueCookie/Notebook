using Notebook.Domain.Entity;
using System;
using System.Collections.Generic;

namespace Notebook.WebClient.Models
{
    public class ContactDetailModel
    {
        public long Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Patronymic { get; set; }

        public string FullName => FirstName + " " + LastName;

        public DateTime? BirthDate { get; set; }

        public string OrganizationName { get; set; }

        public string Position { get; set; }

        public IEnumerable<ContactInformation> ContactInformations { get; set; }
    }
}
