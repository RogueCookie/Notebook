using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Notebook.Database;
using Notebook.Database.Extension;
using Notebook.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Notebook.WebClient.Services
{
    public class ContactService 
    {
        private readonly NotebookDbContext _context;
        private readonly ILogger<ContactService> _logger;

        public ContactService(NotebookDbContext context, ILogger<ContactService> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Add(Contact newContact)
        {
           await  _context.AddAsync(newContact);
           await  _context.SaveChangesAsync();
        }

        public async Task AddContactInformation(ContactInformation newContactInformation)
        {
            await _context.AddAsync(newContactInformation);
            await _context.SaveChangesAsync();
        }

        public async Task<Contact> GetContactById(long contactId)
        {
            return await _context.Contacts
                .Include(x => x.CollectionInformations)
                .FirstOrDefaultAsync(contact => contact.Id == contactId);
        }

        public async Task<List<Contact>> GetAllContacts()
        {
            return await _context.Contacts
                .GetOrderedContacts().ToListAsync();
        }

        public async Task<List<ContactInformation>> GetAllInfoForContact(long contactId)
        {
            return await _context.ContactInformations
                .Where(x => x.ContactId == contactId)
                .ToListAsync();
        }

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
