using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Notebook.Database;
using Notebook.Domain.Entity;
using Notebook.WebClient.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Notebook.WebClient.Services
{
    public class ContactService : IContact
    {
        private readonly NotebookDbContext _context;
        private readonly ILogger<ContactService> _logger;

        public ContactService(NotebookDbContext context, ILogger<ContactService> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc />
        public void Add(Contact newContact)
        {
            _context.Add(newContact);
            _context.SaveChanges();
        }

        /// <inheritdoc />
        public void AddContactInformation(ContactInformation newContactInformation)
        {
            _context.Add(newContactInformation);
            _context.SaveChanges();
        }

        /// <inheritdoc />
        public Contact GetContactById(long contactId)
        {
            return GetAllContacts()
                .FirstOrDefault(contact => contact.Id == contactId);
        }

        /// <inheritdoc />
        public IEnumerable<Contact> GetAllContacts()
        {
            return _context.Contacts
                .Include(contactInfo => contactInfo.CollectionInformations)
                .OrderBy(x => x.FirstName); 
        }

        /// <inheritdoc />
        public IEnumerable<ContactInformation> GetAllInfoForContact(long contactId)
        {
            return _context.ContactInformations
                .Where(x => x.ContactId == contactId);
        }

        /// <inheritdoc />
        public void RemoveContact(long contactId)
        {
            var contact = _context.Contacts
                .FirstOrDefault(x => x.Id == contactId);

            if (contact != null)
            {
                _context.Remove(contact);
            }
        }
    }
}
