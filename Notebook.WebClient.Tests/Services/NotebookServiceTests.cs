using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Notebook.Database;
using Notebook.Domain.Entity;
using Notebook.Domain.Enum;
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
    public class NotebookServiceTests
    {
        private readonly NotebookDbContext _context;
        private readonly NotebookService _service;
        private readonly ContactService _contactService;

        public NotebookServiceTests()
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

            var mockLogger = new Mock<ILogger<NotebookService>>();
            var mockLoggerContact = new Mock<ILogger<ContactService>>();
            _service = new NotebookService(_context, mockLogger.Object);
            _contactService = new ContactService(_context, mockLoggerContact.Object);
        }
        
        [Fact]
        public async Task AddRecordAsync_WhenAdded_AddedRecordExpected()
        {
            // Arrange
            var testRecord = InitRecord();

            // Act
            var recordsCountBefore = await _context.Records.CountAsync();
            await _service.AddRecordAsync(testRecord);
            var recordsCountAfter = await _context.Records.CountAsync();
            var record = await  _context.Records.FirstOrDefaultAsync();

            // Assert
            Assert.NotEqual(recordsCountBefore, recordsCountAfter);
            Assert.Equal(testRecord.Place, record.Place);
        }

        [Fact]
        public async Task GetAllNotDeletedRecordsAsync_WhenGet_GotRecordsExpected()
        {
            // Arrange 
            var testRecord = InitRecord();
            await _service.AddRecordAsync(testRecord);

            // Act
            var records = await _service.GetAllNotDeletedRecordsAsync(null, null);

            // Assert
            Assert.NotNull(records);
            Assert.NotEmpty(records);
        }

        [Fact]
        public async Task GetRecordByIdAsync_WhenGet_GetRecordExpected()
        {
            // Arrange
            var initCont = InitContact();
            var initContId = await _contactService.AddContactAsync(initCont);

            var initRecord = InitRecord();
            initRecord.ContactIds = new List<long>() { initContId };
            await _context.SaveChangesAsync();
            var initId = await _service.AddRecordAsync(initRecord);

            var rel = await _service.UpdateContactsForNoteAsync(initId, initRecord.ContactIds);

            var allRecordId = await _context.Records.Where(x => x.IsDeleted == false).ToListAsync();  
            var curId = allRecordId.FirstOrDefault()?.Id;
            Assert.NotNull(curId);

            // Act
            var getCurrentRecord = await _service.GetRecordByIdAsync(curId.Value);

            // Assert
            Assert.NotNull(getCurrentRecord);
        }

        [Theory]
        [InlineData(true, false)]
        [InlineData(true, true)]
        [InlineData(false, false)]
        [InlineData(false, true)]
        public async Task MarkRecordAsCompletedAsync_WhenMark_ChangeStatusExpected(bool stateAfter, bool stateBefore) 
        {
            // Arrange 
            var initRecord = InitRecord();
            initRecord.IsComplete = stateBefore;
            await _context.SaveChangesAsync();
            var initId = await _service.AddRecordAsync(initRecord);

            // Act
            await _service.MarkRecordAsCompletedAsync(initId, stateAfter);
            var result = await _context.Records.FirstOrDefaultAsync(x => x.Id ==initId);

            // Assert 
            Assert.Equal(stateAfter, result.IsComplete);
        }

        [Fact]
        public async Task DeletedRecordAsync_WhenDelete_DeleteRecordExpected()
        {
            // Arrange
            var initRecord = InitRecord();
            await _service.AddRecordAsync(initRecord);

            var allRecordIdBefore = await  _context.Records.Where(x => x.IsDeleted == false).ToListAsync();  
            var recordId = allRecordIdBefore.FirstOrDefault()?.Id;
            Assert.NotNull(recordId);

            // Act
            await _service.DeletedRecordAsync(recordId.Value);

            var allRecordIdAfter = await _service.GetAllNotDeletedRecordsAsync(null, null);
            var resultBefore = allRecordIdBefore.Count;
            var resultAfter = allRecordIdAfter.Count;
            
            // Assert
            Assert.NotEqual(resultBefore, resultAfter);
        }

        [Fact]
        public async Task UpdateRecordAsync_WhenUpdate_UpdatedRecordExpected()
        {
            // Arrange
            var initRecord = InitRecord();
            var initId = await _service.AddRecordAsync(initRecord);

            var adaptModel = new NoteCreateResponseModel
            {
                Id =  initId,
                Place = "new place",
                Theme = initRecord.Theme,
                IsComplete = initRecord.IsComplete,
                StartDate = initRecord.StartDate,
                RecordTypeId = 2
            };

            // Act
            var updatedModel = await _service.UpdateRecordAsync(adaptModel);

            // Assert
            Assert.NotEqual(initRecord.Place, updatedModel.Place);
        }

        [Fact]
        public async Task UpdateContactsForNoteAsync_WhenAdd_UpdatedListOfContactsExpected()
        {
            // Arrange
            var initCont = InitContact();
            var initContId = await _contactService.AddContactAsync(initCont);

            var initRecord = InitRecord();
            initRecord.ContactIds = new List<long>() {initContId};
            await _context.SaveChangesAsync();
            var initId = await _service.AddRecordAsync(initRecord);

            var initSec = InitSecondContact();
            var secId =await _contactService.AddContactAsync(initSec);

            await _service.UpdateContactsForNoteAsync(initId, new List<long>() {initContId});
            var contList = new List<long> {initContId, secId};

            // Act
            var contactBefore = await _context.RecordsToContacts.Where(x => x.RecordId == initId).Select(c=>c.ContactId).ToListAsync();
            var countBefore = contactBefore.Count();

            var res = await  _service.UpdateContactsForNoteAsync(initId, contList);

            var contactAfter = await _context.RecordsToContacts.Where(x => x.RecordId == initId).Select(c => c.ContactId).ToListAsync();
            var countAfter = contactAfter.Count();
            
            // Assert
            Assert.True(res);
            Assert.NotEqual(countBefore, countAfter);
            Assert.NotEqual(contactBefore, contactAfter);
        }

        [Fact]
        public async Task UpdateContactsForNoteAsync_WhenNotChanged_OriginalListOfContacts()
        {
            // Arrange
            var initCont = InitContact();
            var initContId = await _contactService.AddContactAsync(initCont);

            var initRecord = InitRecord();
            initRecord.ContactIds = new List<long>() { initContId };
            await _context.SaveChangesAsync();

            var initId = await _service.AddRecordAsync(initRecord);

            var contList = new List<long> { initContId };
            await _context.RecordsToContacts.AddAsync(new RecordsToContacts() { ContactId = initContId, RecordId = initId });
            await _context.SaveChangesAsync();

            // Act
            var contactBefore = await _context.RecordsToContacts.Where(x => x.RecordId == initId).Select(c => c.ContactId).ToListAsync();
            var countBefore = contactBefore.Count(); 

            var res = await _service.UpdateContactsForNoteAsync(initId, contList);

            var contactAfter = await _context.RecordsToContacts.Where(x => x.RecordId == initId).Select(c => c.ContactId).ToListAsync();
            var countAfter = contactAfter.Count();

            // Assert
            Assert.True(res);
            Assert.Equal(countBefore, countAfter);
            Assert.Collection(contactBefore, x =>  contactAfter.Contains(x));
        }


        [Fact]
        public async Task UpdateContactsForNoteAsync_WhenRemoveOneContactFromList_UpdatedListOfContactsExpected()
        {
            // Arrange
            var initCont = InitContact();
            var initContId = await _contactService.AddContactAsync(initCont);

            var initRecord = InitRecord();
            initRecord.ContactIds = new List<long>() { initContId };
            await _context.SaveChangesAsync();
            var initId = await _service.AddRecordAsync(initRecord);

            var initSec = InitSecondContact();
            var secId = await _contactService.AddContactAsync(initSec);


            await _context.RecordsToContacts.AddAsync(new RecordsToContacts() {ContactId = initContId, RecordId = initId});
            await _context.RecordsToContacts.AddAsync(new RecordsToContacts() {ContactId = secId, RecordId = initId});
            
            await _context.SaveChangesAsync();
            var contList = new List<long> { initContId};

            // Act
            var contactBefore = await _context.RecordsToContacts.Where(x => x.RecordId == initId).Select(c => c.ContactId).ToListAsync();
            var countBefore = contactBefore.Count();

            var res = await _service.UpdateContactsForNoteAsync(initId, contList);

            var contactAfter = await _context.RecordsToContacts.Where(x => x.RecordId == initId).Select(c => c.ContactId).ToListAsync();
            var countAfter = contactAfter.Count();
            

            // Assert
            Assert.True(res);
            Assert.True(!contactAfter.Contains(secId));
            Assert.NotEqual(countBefore, countAfter);
        }

        [Fact]
        public async Task UpdateContactsForNoteAsync_WhenAddAndRemoveOneContactFromList_UpdatedListOfContactsExpected()
        {
            // Arrange
            var initCont = InitContact();
            var initContId = await _contactService.AddContactAsync(initCont);

            var initRecord = InitRecord();
            initRecord.ContactIds = new List<long>() { initContId };
            await _context.SaveChangesAsync();
            var initId = await _service.AddRecordAsync(initRecord);

            var initSec = InitSecondContact();
            var secId = await _contactService.AddContactAsync(initSec);

            await _context.RecordsToContacts.AddAsync(new RecordsToContacts() { ContactId = initContId, RecordId = initId });

            await _context.SaveChangesAsync();
            var contList = new List<long> { secId };

            // Act
            var contactBefore = await _context.RecordsToContacts.Where(x => x.RecordId == initId).Select(c => c.ContactId).ToListAsync();

            var res = await _service.UpdateContactsForNoteAsync(initId, contList);

            var contactAfter = await _context.RecordsToContacts.Where(x => x.RecordId == initId).Select(c => c.ContactId).ToListAsync();

            // Assert
            //Func<IEnumerable<long>, IEnumerable<long>, bool> testFunc = (contIdBefore, contIdAfter) =>
            //    !contactAfter.Except(contactBefore).Any(); 
            //Assert.Collection(contactBefore, x => contactAfter.Contains(x));
            Assert.True(res);
            Assert.True(contactAfter.Except(contactBefore).Any()); //id after not contains id before
           
        }

        /// <summary>
        /// Initiate record in DB for making test
        /// </summary>
        /// <returns></returns>
        private static NoteCreateModel InitRecord()
        {
            var testRecord = new NoteCreateModel()
            {
                StartDate = DateTime.Now,
                Theme = "ho-ho-ho",
                EndDate = DateTime.Now.AddDays(1),
                Place = "Kenig",
                RecordTypeId = (long)RecordTypeEnum.Deal
            };
            return testRecord;
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

        private static ContactCreateModel InitSecondContact()
        {
            var initContact = new ContactCreateModel()
            {
                FirstName = "Kris",
                BirthDate = new DateTime(2021, 12, 27),
                LastName = "",
                OrganizationName = ""
            };
            return initContact;
        }
    }
}
