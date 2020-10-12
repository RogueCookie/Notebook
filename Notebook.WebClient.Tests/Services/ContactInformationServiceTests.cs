using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Notebook.Database;
using Notebook.DTO.Models.Request;
using Notebook.WebClient.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Notebook.WebClient.Tests.Services
{
    public class ContactInformationServiceTests
    {
        private readonly NotebookDbContext _context;
        private readonly ContactInformationService _informationService;
        private readonly ContactService _contactService;

        public ContactInformationServiceTests()
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

            var mockLogger = new Mock<ILogger<ContactInformationService>>();
            var mockLogger2 = new Mock<ILogger<ContactService>>();
            _informationService = new ContactInformationService(_context, mockLogger.Object);
            _contactService = new ContactService(_context, mockLogger2.Object);
        }

        [Fact]
        public async Task UpdateContactInfo_WhenUpdated_RefreshEntityExpected()
        {
            // Arrange
            var initContact = InitContact();
            var contactId = await _contactService.AddContactAsync(initContact);
            var info = InitContactInformation(contactId);
            var firstInfo = info.FirstOrDefault();
            Assert.NotNull(firstInfo); 

            // Act
            var addedInfo = await _informationService.AddContactInformationAsync(firstInfo);
            var result = await _informationService.GetCurrentContactInformationResponseAsync(addedInfo);
            result.Other = "updated info";
            await _informationService.UpdateContactInformation(result);
            var infoFromDb = _context.ContactInformations.FirstOrDefault(x => x.Id == result.Id);
            Assert.NotNull(infoFromDb);

            // Assert
            Assert.NotEqual(firstInfo.Other, infoFromDb.Other);
            Assert.Equal(result.Other, infoFromDb.Other);
        }


        [Fact]
        public async Task AddContactInformationAsync_WhenAdded_AddedContactInformationExpected()
        {
            // Arrange
            var initContact = InitContact();
            var addedContact = await _contactService.AddContactAsync(initContact);
            var newContactInformation = InitContactInformation(addedContact).ToList();
            await _informationService.AddBulkContactInformationAsync(newContactInformation);

            // Act
            var allContactInformationBefore = await _informationService.GetAllInfoForContactAsync(addedContact);
            await _informationService.AddBulkContactInformationAsync(newContactInformation);
            var allContactInformationAfter = await _informationService.GetAllInfoForContactAsync(addedContact);
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
            var contactId = await _contactService.AddContactAsync(initContact);
            var newContactInformation = InitContactInformation(contactId).ToList();

            //Act
            var allContactInformationBefore = await _context.ContactInformations.CountAsync(x => x.ContactId == contactId);
            await _informationService.AddBulkContactInformationAsync(newContactInformation);
            var allContactInformationAfter = await _context.ContactInformations.CountAsync(x => x.ContactId == contactId);
            var id = newContactInformation.FirstOrDefault()?.ContactId;

            // Assert
            Assert.NotNull(newContactInformation.FirstOrDefault());
            Assert.NotEqual(allContactInformationBefore, allContactInformationAfter);
            Assert.Equal(id, contactId);
        }

        [Fact]
        public async Task RemoveCurrentContactInformation_WhenRemoved_ExpectedRemovedInformationById()
        {
            // Arrange
            var initContact = InitContact();
            var contId = await _contactService.AddContactAsync(initContact);
            var newContactInformation = InitContactInformation(contId);
            await _informationService.AddBulkContactInformationAsync(newContactInformation);
            var infoForContact = await _context.ContactInformations.FirstOrDefaultAsync(x => x.ContactId == contId);

            // Act
            var infoBefore = await _context.ContactInformations.CountAsync(x => x.ContactId == contId);
            await _informationService.RemoveCurrentContactInformationAsync(infoForContact.Id);
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