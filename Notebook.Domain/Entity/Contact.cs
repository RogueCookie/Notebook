using System;

namespace Notebook.Domain.Entity
{
    /// <summary>
    /// Base information about all contacts
    /// </summary>
    public class Contact
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Patronymic { get; set; }
        public DateTime? BirthDate { get; set; }
        public string OrganizationName { get; set; }
        public string Position { get; set; }
    }
}
