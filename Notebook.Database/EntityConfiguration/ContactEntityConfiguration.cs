using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Notebook.Domain.Entity;

namespace Notebook.Database.EntityConfiguration
{
    public class ContactEntityConfiguration : IEntityTypeConfiguration<Contact>
    {
        public void Configure(EntityTypeBuilder<Contact> builder)
        {
            //builder.ToTable("Contact");
            builder.HasKey(x => x.Id);

            //properties
            builder.Property(p => p.Id).HasDefaultValueSql("NEWID()");
        }
    }
}
