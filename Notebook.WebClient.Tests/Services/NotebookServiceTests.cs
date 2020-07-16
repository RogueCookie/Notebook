using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Notebook.Database;
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
            _service = new NotebookService(_context, mockLogger.Object);
        }

        
        [Fact]
        public async Task AddRecordAsync_WhenAdded_AddedRecordExpected()
        {
            // Arrange
            var testRecord = InitRecord();

            // Act
            var recordsCountBefore = _context.Records.Count();
            await _service.AddRecordAsync(testRecord);
            var recordsCountAfter = _context.Records.Count();
            var record = _context.Records.First();

            // Assert
            Assert.NotEqual(recordsCountBefore, recordsCountAfter);
            Assert.Equal(testRecord.Place, record.Place);
        }

        [Fact]
        public async Task GetAllNotDeletedRecordsAsync_WhenGet_GotRecordsExpected()
        {
            // Act
            var records = await _service.GetAllNotDeletedRecordsAsync(null, null);

            Assert.NotNull(records);
        }

        [Fact]
        public async Task GetRecordByIdAsync_WhenGet_GetRecordExpected()
        {
            // Arrange
            var initRecord = InitRecord();
            await _service.AddRecordAsync(initRecord);

            var allRecordId = await _service.GetAllNotDeletedRecordsAsync(null, null);
            var recordId = allRecordId.FirstOrDefault().Id;

            // Act
            var getCurrentRecord = await _service.GetRecordByIdAsync(recordId);

            // Assert
            Assert.NotEqual(0, getCurrentRecord.Id);
            Assert.Equal(1, getCurrentRecord.Id);
        }

        [Theory]
        [InlineData(true, false)]
        [InlineData(true, true)]
        [InlineData(false, false)]
        [InlineData(false, true)]
        public async Task MarkRecordAsCompletedAsync_WhenMark_ChangeStatusExpected(bool stateAfter, bool stateBefore)
        {
            var newRecord = new Domain.Entity.Record
            {
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1),
                Place = "Kenig",
                IsComplete = stateBefore
            };
            // Arrange 
            await _context.Records.AddAsync(newRecord);
            await  _context.SaveChangesAsync();

            // Act
            await _service.MarkRecordAsCompletedAsync(newRecord.Id, stateAfter);
            var result = await _context.Records.FirstAsync(x => x.Id == newRecord.Id);
            
            // Assert 
            Assert.Equal(stateAfter, result.IsComplete);     
        }

        [Fact]
        public async Task DeletedRecordAsync_WhenDelete_DeleteRecordExpected()
        {
            // Arrange
            var initRecord = InitRecord();
            await _service.AddRecordAsync(initRecord);

            var allRecordIdBefore = await _service.GetAllNotDeletedRecordsAsync(null, null);
            var recordId = allRecordIdBefore.FirstOrDefault().Id;

            // Act
            await _service.DeletedRecordAsync(recordId);

            var allRecordIdAfter = await _service.GetAllNotDeletedRecordsAsync(null, null);
            var resultBefore = allRecordIdBefore.Count;
            var resultAfter = allRecordIdAfter.Count;                                   
            // Assert
            Assert.NotEqual(resultBefore, resultAfter);
        }

        private static Domain.Entity.Record InitRecord()
        {
            var testRecord = new Domain.Entity.Record()
            {
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1),
                Place = "Kenig"
            };
            return testRecord;
        }
    }
}
