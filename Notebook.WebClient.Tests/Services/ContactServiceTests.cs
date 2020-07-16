using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Notebook.Database;
using Notebook.Domain.Entity;
using Notebook.WebClient.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Notebook.WebClient.Tests.Services
{
    public class ContactServiceTests
    {
        private readonly NotebookDbContext _context;
        private readonly ContactService _service;

        public ContactServiceTests()
        {
            var myConfiguration = new Dictionary<string, string>
            {
                {"SchemaName", "TestSchema"}
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(myConfiguration)
                .Build();

            _context = new NotebookDbContext(new DbContextOptionsBuilder<NotebookDbContext>()
                .UseSqlite("Filename=:memory:")
                .UseSnakeCaseNamingConvention()
                .Options, configuration);
            _context.Database.OpenConnection();
            _context.Database.EnsureCreated();

            var mockLogger = new Mock<ILogger<ContactService>>();
            _service = new ContactService(_context, mockLogger.Object);
        }

        [Fact]
        public async Task AddContactAsync_WhenAdded_AddedContactExpected()
        {
            // Arrange
            var initContact = InitContact();

            // Act
            var contactsBefore = _context.Contacts.Count();
            await _service.AddContactAsync(initContact);
            var contactAfter = _context.Contacts.Count();

            var newContactData = _context.Contacts.FirstOrDefault();

            // Assert
            Assert.NotEqual(contactsBefore, contactAfter);
            Assert.Equal(initContact.FirstName, newContactData.FirstName);
        }

        [Fact]
        public async Task AddContactInformationAsync_WhenAdded_AddedContactInformationExpected()
        {
            // Arrange
            var initContact = InitContact();
            await _service.AddContactAsync(initContact);
            var newContactInformation = InitContactInformation(initContact.Id);

            // Act
            var allContactInformationBefore = await _service.GetAllInfoForContactAsync(initContact.Id);
            await _service.AddBulkContactInformationAsync(newContactInformation);
            var allContactInformationAfter = await _service.GetAllInfoForContactAsync(initContact.Id);
            var contactId = allContactInformationAfter.FirstOrDefault();
            
            // Assert
            Assert.NotNull(allContactInformationAfter);
            Assert.NotEqual(allContactInformationBefore.Count, allContactInformationAfter.Count);
            Assert.Equal(initContact.Id, contactId.Id);

        }

        [Fact]
        public async Task AddBulkContactInformationAsync_WhenAdded_AddedListContactInformationExpected()
        {
            // Arrange
            var initContact = InitContact();
            await _service.AddContactAsync(initContact);
            var newContactInformation = InitContactInformation(initContact.Id);

            //Act
            var allContactInformationBefore = await _service.GetAllInfoForContactAsync(initContact.Id);  //TODO опять несколько методов может extension для контакт инфо добавить?
            await _service.AddBulkContactInformationAsync(newContactInformation);
            var allContactInformationAfter = await _service.GetAllInfoForContactAsync(initContact.Id);
            var contactId = allContactInformationAfter.FirstOrDefault();

            // Assert
            Assert.NotNull(allContactInformationAfter);
            Assert.NotEqual(allContactInformationBefore.Count, allContactInformationAfter.Count);
            Assert.Equal(initContact.Id, contactId.Id);
        }

        [Fact]
        public async Task GetContactByIdAsync_WhenGet_GetCurrentContactExpected()
        {
            // Arrange
            var initContact = InitContact();
            await _service.AddContactAsync(initContact);

            // Act
            var contactInDb = await _service.GetContactByIdAsync(initContact.Id);
            
            // Assert
            Assert.NotNull(contactInDb);
            Assert.Equal(initContact.Id, contactInDb.Id);
        }

        [Fact]
        public async Task GetAllContactsAsync_WhenGet_GetLostOfContactsExpected()
        {
            // Arrange
            var initContact = InitContact();
            var listContacts = new List<Contact> { initContact };
            foreach (var item in listContacts)
            {
                await _service.AddContactAsync(item);
            }
            
            // Act
            var contactInDb = await _service.GetAllContactsAsync();

            // Assert
            Assert.NotNull(listContacts);
        }

        [Fact]
        public async Task GetAllInfoForContactAsync_WhenGet_GotListContactInformation()
        {
            
            // Arrange
            var initContact = InitContact();
            await _service.AddContactAsync(initContact);

            var newContactInformation = InitContactInformation(initContact.Id);

            // Act
            var allContactInformationBefore = await _service.GetAllInfoForContactAsync(initContact.Id);
            await _service.AddBulkContactInformationAsync(newContactInformation);
            var allContactInformationAfter = await _service.GetAllInfoForContactAsync(initContact.Id);
           
            // Assert
            Assert.NotEmpty(allContactInformationAfter);
            Assert.NotEqual(allContactInformationBefore, allContactInformationAfter);
        }

        [Fact]
        public async Task RemoveContactAsync_RemoveContact_RemovedContactExpected()
        {
            // Arrange
            var initContact = InitContact();
            await _service.AddContactAsync(initContact);
            var newContactInformation = InitContactInformation(initContact.Id);

            // Act
            var resultBefore = _context.Contacts.Count();
            await _service.RemoveContactAsync(initContact.Id);
            var resultAfter = _context.Contacts.Count();
            
            // Assert
            Assert.NotEqual(resultBefore, resultAfter);
        }

        private static Contact InitContact()
        {
            var initContact = new Contact
            {
                FirstName = "Lera",
                BirthDate = new DateTime(1989, 1, 15),
                LastName = "dKi",
                OrganizationName = "Ara"

            };
            return initContact;
        }

        private static IEnumerable<ContactInformation> InitContactInformation(long contactId)
        {
            var contactInfo = new List<ContactInformation>
            {
                new ContactInformation
                {
                    ContactId = contactId,
                    PhoneNumber = "89527906422",
                    Skype = "skype"
                },
                new ContactInformation
                {
                    ContactId = contactId,
                    PhoneNumber = "891127906433",
                    Skype = "skype2"
                }
            };
            return contactInfo;
        }
    }
}