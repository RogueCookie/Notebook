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
            var contactsBefore = await _context.Contacts.CountAsync();
            await _service.AddContactAsync(initContact);
            var contactAfter = await _context.Contacts.CountAsync();

            var newContactData = await _context.Contacts.FirstOrDefaultAsync();

            // Assert
            Assert.NotEqual(contactsBefore, contactAfter);
            Assert.Equal(initContact.FirstName, newContactData.FirstName);
        }

        [Fact]
        public async Task UpdateContactInfo_WhenUpdated_RefreshEntityExpected()
        {
            // Arrange
            var initContact = InitContact();
            var contactId = await _service.AddContactAsync(initContact);
            var info = InitContactInformation(contactId);
            var firstInfo = info.FirstOrDefault();
            Assert.NotNull(firstInfo); // TODO

            // Act
            var addedInfo = await _service.AddContactInformationAsync(firstInfo);
            var result = await _service.GetCurrentContactInformationResponseAsync(addedInfo);
            result.Other = "updated info";
            await _service.UpdateContactInformation(result);
            var infoFromDb = _context.ContactInformations.FirstOrDefault(x =>x.Id == result.Id);
            Assert.NotNull(infoFromDb);

            // Assert
            Assert.NotEqual(firstInfo.Other, infoFromDb.Other);
            Assert.Equal(result.Other, infoFromDb.Other);

        }

        [Fact]
        public async Task UpdateContact_WhenUpdate_UpdatedModelExpected()
        {
            // Arrange
            var initContact = InitContact();
            var contId = await _service.AddContactAsync(initContact);
            var model = await  _service.GetContactByIdAsync(contId);

            var adapt = new AddNewContact
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
        public async Task AddContactInformationAsync_WhenAdded_AddedContactInformationExpected()
        {
            // Arrange
            var initContact = InitContact();
            var addedContact = await _service.AddContactAsync(initContact);
            var newContactInformation = InitContactInformation(addedContact).ToList();
            await _service.AddBulkContactInformationAsync(newContactInformation);
            
            // Act
            var allContactInformationBefore = await _service.GetAllInfoForContactAsync(addedContact);
            await _service.AddBulkContactInformationAsync(newContactInformation);
            var allContactInformationAfter = await _service.GetAllInfoForContactAsync(addedContact);
            var contactInformationResponse = allContactInformationAfter.FirstOrDefault();

            // Assert 
            Assert.NotNull(contactInformationResponse);
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
            var newContactInformation = InitContactInformation(contactId).ToList();

            //Act
            var allContactInformationBefore = await  _context.ContactInformations.CountAsync(x => x.ContactId == contactId);
            await _service.AddBulkContactInformationAsync(newContactInformation);
            var allContactInformationAfter = await _context.ContactInformations.CountAsync(x => x.ContactId == contactId);
            var id = newContactInformation.FirstOrDefault()?.ContactId;

            // Assert
            Assert.NotNull(newContactInformation.FirstOrDefault());
            Assert.NotEqual(allContactInformationBefore, allContactInformationAfter);
            Assert.Equal(id, contactId);
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
            await _service.AddBulkContactInformationAsync(newContactInformation);
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

        [Fact]
        public async Task RemoveCurrentContactInformation_WhenRemoved_ExpectedRemovedInformationById()
        {
            // Arrange
            var initContact = InitContact();
            var contId = await _service.AddContactAsync(initContact);
            var newContactInformation = InitContactInformation(contId);
            await _service.AddBulkContactInformationAsync(newContactInformation);
            var infoForContact = await _context.ContactInformations.FirstOrDefaultAsync(x => x.ContactId == contId);

            // Act
            var infoBefore = await _context.ContactInformations.CountAsync(x => x.ContactId == contId);
            await _service.RemoveCurrentContactInformationAsync(infoForContact.Id);
            var infoAfter = await _context.ContactInformations.CountAsync(x => x.ContactId == contId);

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