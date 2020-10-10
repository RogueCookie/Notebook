using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Notebook.Domain.Entity;
using Notebook.Domain.Enum;

namespace Notebook.Database
{
    public class NotebookDbContext : DbContext
    {
        private readonly string _schemaName;
        public NotebookDbContext( DbContextOptions<NotebookDbContext> options, IConfiguration configuration) : base(options) 
        {
           _schemaName = configuration.GetValue<string>("SchemaName");
        }


        public DbSet<Contact> Contacts { get; set; }
        public DbSet<ContactInformation> ContactInformations { get; set; }
        public DbSet<RecordType> RecordTypes { get; set; }
        public DbSet<RecordsToContacts> RecordsToContacts { get; set; }
        public DbSet<Record> Records { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (!string.IsNullOrEmpty(_schemaName))
            {
                modelBuilder.HasDefaultSchema(_schemaName);
            }

            modelBuilder.Entity<Contact>().HasKey("Id");
            modelBuilder.Entity<ContactInformation>().HasKey("Id");
            modelBuilder.Entity<ContactInformation>()
                .HasOne(sc => sc.Contact)
                .WithMany(s => s.CollectionInformations)
                .HasForeignKey(sc => sc.ContactId);

            modelBuilder.Entity<Record>().HasKey(x => x.Id);

            // allow to save in Db as a string
            modelBuilder.Entity<Record>()
                .Property(vs => vs.RecordPayLoadValue)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v),
                    v => JsonConvert.DeserializeObject<RecordPayLoad>(v?? string.Empty)); //xчерез репозиторий проверить не возможно

            modelBuilder.Entity<RecordsToContacts>().HasKey(kr => new { kr.ContactId, kr.RecordId });
            //many to many
            modelBuilder.Entity<RecordsToContacts>()
                .HasOne(sc => sc.Contact)
                .WithMany(s => s.RecordsToContacts)
                .HasForeignKey(s => s.ContactId);

            modelBuilder.Entity<RecordsToContacts>()
                .HasOne(sc => sc.Record)
                .WithMany(s => s.RecordsToContacts)
                .HasForeignKey(s => s.RecordId);

            modelBuilder.Entity<RecordType>().HasKey("Id");

            modelBuilder.Entity<RecordType>()
                .HasMany(sc => sc.Records)
                .WithOne(s => s.RecordType)
                .HasForeignKey(s => s.RecordTypeId);

            modelBuilder.Entity<RecordType>().Property(e => e.Alias)
                .HasConversion(
                    v => v.ToString(),
                    v => (RecordTypeEnum)System.Enum.Parse(typeof(RecordTypeEnum), v, true));

            modelBuilder.Entity<RecordType>().HasData(
                new RecordType
                {
                    Id = (long)RecordTypeEnum.Deal,
                    Alias = RecordTypeEnum.Deal,
                    Description = "Planning deal",
                    Name = "Deal"
                },
                new RecordType
                {
                    Id = (long)RecordTypeEnum.Meeting,
                    Alias = RecordTypeEnum.Meeting,
                    Description = "Planning meeting",
                    Name = "Meeting"
                },
                new RecordType
                {
                    Id = (long)RecordTypeEnum.Notes,
                    Alias = RecordTypeEnum.Notes,
                    Description = "Notes",
                    Name = "Notes"
                }
            );

            base.OnModelCreating(modelBuilder);
        }
    }
}
