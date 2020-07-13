using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Notebook.Domain.Entity;
using Notebook.Domain.Enum;

namespace Notebook.Database
{
    public class NotebookDbContext : DbContext
    {
        private readonly string _schemaName;
        public NotebookDbContext(IConfiguration configuration, DbContextOptions<NotebookDbContext> options) : base(options) 
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

            modelBuilder.Entity<RecordsToContacts>().HasKey(kr => new {kr.ContactId, kr.RecordId} ); //состав ключ

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
            modelBuilder.Entity<RecordType>().Property(e => e.Allias)
                .HasConversion(
                    v => v.ToString(),
                    v => (RecordTypeEnum)System.Enum.Parse(typeof(RecordTypeEnum), v, true));

            modelBuilder.Entity<RecordType>().HasData(
                new RecordType
                {
                    Id = (long)RecordTypeEnum.Deal,
                    Allias = RecordTypeEnum.Deal,
                    Description = "Planning deal",
                    Name = "Deal"
                },
                new RecordType
                {
                    Id = (long)RecordTypeEnum.Meeting,
                    Allias = RecordTypeEnum.Meeting,
                    Description = "Planning meeting",
                    Name = "Meeting"
                },
                new RecordType
                {
                    Id = (long)RecordTypeEnum.Notes,
                    Allias = RecordTypeEnum.Notes,
                    Description = "Notes",
                    Name = "Notes"
                }
            );

            

            base.OnModelCreating(modelBuilder);
        }
    }
}
