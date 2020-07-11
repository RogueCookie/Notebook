using Microsoft.EntityFrameworkCore;
using Notebook.Domain.Entity;

namespace Notebook.Database
{
    public class PostgreDbContext : DbContext
    {
        public PostgreDbContext(DbContextOptions<PostgreDbContext> options): base(options) { }

        public DbSet<Contact> Contacts { get; set; }
    }
}
