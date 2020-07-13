using System;
using System.Collections.Generic;

namespace Notebook.Domain.Entity
{
    /// <summary>
    /// Base information about all contacts
    /// </summary>
    public class Contact
    {
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Patronymic { get; set; }
        public DateTime? BirthDate { get; set; }
        public string OrganizationName { get; set; }
        public string Position { get; set; }
        public virtual ICollection<ContactInformation> CollectionInformations  { get; set; }
        public virtual ICollection<RecordsToContacts> RecordsToContacts { get; set; }

    }
}
