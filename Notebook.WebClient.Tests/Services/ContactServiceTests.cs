using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Notebook.Database;
using Notebook.DTO.Models.Request;
using Notebook.DTO.Models.Response;
using Notebook.WebClient.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Notebook.WebClient.Tests.Services
{
    public class ContactServiceTests
    {
        private readonly NotebookDbContext _context;
        private readonly ContactService _service;
        private readonly ContactInformationService _informationService;

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
            var mockLogger2 = new Mock<ILogger<ContactInformationService>>();
            _service = new ContactService(_context, mockLogger.Object);
            _informationService = new ContactInformationService(_context, mockLogger2.Object);
        }

        [Fact]
        public async Task AddContactAsync_WhenAdded_AddedContactExpected()
        {
            // Arrange
            var initContact = InitContact();

            // Act
            var contactsBefore = await _context.Contacts.CountAsync();
            await _service.AddContactAsync(initContact);
            var contactAfter = await _context.Contacts.CountAsync();

            var newContactData = await _context.Contacts.FirstOrDefaultAsync();

            // Assert
            Assert.NotEqual(contactsBefore, contactAfter);
            Assert.Equal(initContact.FirstName, newContactData.FirstName);
        }

        [Fact]
        public async Task UpdateContact_WhenUpdate_UpdatedModelExpected()
        {
            // Arrange
            var initContact = InitContact();
            var contId = await _service.AddContactAsync(initContact);
            var model = await  _service.GetContactByIdAsync(contId);

            var adapt = new ResponseContact
            {
                Id = contId,
                FirstName = "Updates",
                LastName = model.LastName,
                BirthDate = model.BirthDate,
                OrganizationName = model.OrganizationName,
                Patronymic = model.Patronymic,
                Position = model.Position
            };

            // Act
            var result = await _service.UpdateContact(adapt);

            // Assert
            Assert.NotEqual(initContact.FirstName, result.FirstName); 
        }

        [Fact]
        public async Task GetContactByIdAsync_WhenGet_GetCurrentContactExpected()
        {
            // Arrange
            var initContact = InitContact();
            var newContactId =await _service.AddContactAsync(initContact);

            // Act
            var contactInDb = await _service.GetContactByIdAsync(newContactId);
            var addedContact = await _context.Contacts.FirstOrDefaultAsync(x => x.Id == newContactId);
            
            // Assert
            Assert.NotNull(contactInDb);
            Assert.Equal(newContactId, addedContact.Id);
        }

        [Fact]
        public async Task GetAllContactsAsync_WhenGet_GetLostOfContactsExpected()
        {
            // Arrange
            InitContact();
            await _service.AddContactAsync(InitContact());

            // Act
            var contactInDb = await _service.GetAllContactsAsync();

            // Assert
            Assert.NotNull(contactInDb);
            Assert.NotEmpty(contactInDb);
        }

        [Fact]
        public async Task RemoveContactAsync_RemoveContact_RemovedContactExpected()
        {
            // Arrange
            var initContact = InitContact();
            var contId = await _service.AddContactAsync(initContact);
            var newContactInformation = InitContactInformation(contId);
            await _informationService.AddBulkContactInformationAsync(newContactInformation);
            var addedInfo = await  _context.ContactInformations.CountAsync(x => x.ContactId == contId);

            // Act
            var resultBefore = await _context.Contacts.CountAsync();
            await _service.RemoveContactAsync(contId);
            var resultAfter = await _context.Contacts.CountAsync();
            var infoAfter = await _context.ContactInformations.CountAsync(x => x.ContactId == contId);

            // Assert
            Assert.NotEqual(resultBefore, resultAfter);
            Assert.NotEqual(addedInfo, infoAfter);
        }

        private static ContactCreateModel InitContact()
        {
            var initContact = new ContactCreateModel()
            {
                FirstName = "Lera",
                BirthDate = new DateTime(1989, 1, 15),
                LastName = "dKi",
                OrganizationName = "Ara"
            };
            return initContact;
        }

        private static IEnumerable<ContactInformationRequestModel> InitContactInformation(long contactId)
        {
            var contactInfo = new List<ContactInformationRequestModel>
            {
                new ContactInformationRequestModel()
                {
                    ContactId = contactId,
                    PhoneNumber = "89527906422",
                    Skype = "skype"
                },
                new ContactInformationRequestModel()
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