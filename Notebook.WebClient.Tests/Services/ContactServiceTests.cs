using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Notebook.Database;
using Notebook.Domain.Entity;
using Notebook.DTO.Models.Request;
using Notebook.WebClient.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Notebook.DTO.Models.Responce;
using Notebook.DTO.Models.Response;
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
        public async Task UpdateContactInfo_WhenUpdated_RefreshEntityExpected()
        {
            // Arrange
            var initContact = InitContact();
            var contactId =await _service.AddContactAsync(initContact);
            var info = InitContactInformation(contactId);
            var firstInfo = info.FirstOrDefault();

            // Act
            var addedInfo = await _service.AddContactInformationAsync(firstInfo);
            var result = await _service.GetCurrentContactInformationResponseAsync(addedInfo);
            result.Other = "updated info";
            await _service.UpdateContactInformation(result);
            var infoFromDb = _context.ContactInformations.FirstOrDefault(x =>x.Id == result.Id);
            
            // Assert
            Assert.NotEqual(firstInfo.Other, infoFromDb.Other);
            Assert.Equal(result.Other, infoFromDb.Other);

        }

        [Fact]
        public async Task AddContactInformationAsync_WhenAdded_AddedContactInformationExpected()
        {
            // Arrange
            var initContact = InitContact();
            var addedContact = await _service.AddContactAsync(initContact);
            var newContactInformation = InitContactInformation(addedContact);
            await _service.AddBulkContactInformationAsync(newContactInformation);

            // Act
            var allContactInformationBefore = await _service.GetAllInfoForContactAsync(addedContact);
            await _service.AddBulkContactInformationAsync(newContactInformation);
            var allContactInformationAfter = await _service.GetAllInfoForContactAsync(addedContact);
            var contactInformationResponse = allContactInformationAfter.FirstOrDefault();

            // Assert 
            Assert.NotNull(allContactInformationAfter);
            Assert.NotEqual(allContactInformationBefore.Count, allContactInformationAfter.Count);
            Assert.Equal(addedContact, contactInformationResponse.Id);
        }

        [Fact]
        public async Task AddBulkContactInformationAsync_WhenAdded_AddedListContactInformationExpected()
        {
            // Arrange
            var initContact = InitContact();
            var contactId = await _service.AddContactAsync(initContact);
            var newContactInformation = InitContactInformation(contactId);

            //Act
            var allContactInformationBefore = _context.ContactInformations.Count(x => x.ContactId == contactId);
            await _service.AddBulkContactInformationAsync(newContactInformation);
            var allContactInformationAfter = _context.ContactInformations.Count(x => x.ContactId == contactId);

            // Assert
            Assert.NotEqual(allContactInformationBefore, allContactInformationAfter);
            Assert.Equal(newContactInformation.FirstOrDefault().ContactId, contactId);
        }

        [Fact]
        public async Task GetContactByIdAsync_WhenGet_GetCurrentContactExpected()
        {
            // Arrange
            var initContact = InitContact();
            var newContactId =await _service.AddContactAsync(initContact);

            // Act
            var contactInDb = await _service.GetContactByIdAsync(newContactId);
            var addedContact = _context.Contacts.FirstOrDefault(x => x.Id == newContactId);
            
            // Assert
            Assert.NotNull(contactInDb);
            Assert.Equal(newContactId, addedContact.Id);
        }

        [Fact]
        public async Task GetAllContactsAsync_WhenGet_GetLostOfContactsExpected()
        {
            // Arrange
            var initContact = InitContact();
            var savesContId = await _service.AddContactAsync(InitContact());

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
            await _service.AddBulkContactInformationAsync(newContactInformation);
            var addedInfo = _context.ContactInformations.Count(x => x.ContactId == contId);

            // Act
            var resultBefore = _context.Contacts.Count();
            await _service.RemoveContactAsync(contId);
            var resultAfter = _context.Contacts.Count();
            var infoAfter = _context.ContactInformations.Count(x => x.ContactId == contId);

            // Assert
            Assert.NotEqual(resultBefore, resultAfter);
            Assert.NotEqual(addedInfo, infoAfter);
        }

        [Fact]
        public async Task RemoveCurrentContactInformation_WhenRemoved_ExpectedRemovedInformationById()
        {
            // Arrange
            var initContact = InitContact();
            var contId = await _service.AddContactAsync(initContact);
            var newContactInformation = InitContactInformation(contId);
            await _service.AddBulkContactInformationAsync(newContactInformation);
            var infoForContact = _context.ContactInformations.FirstOrDefault(x => x.ContactId == contId);

            // Act
            var infoBefore = _context.ContactInformations.Count(x => x.ContactId == contId);
            await _service.RemoveCurrentContactInformationAsync(infoForContact.Id);
            var infoAfter = _context.ContactInformations.Count(x => x.ContactId == contId);

            // Assert
            Assert.NotEqual(infoBefore, infoAfter);
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