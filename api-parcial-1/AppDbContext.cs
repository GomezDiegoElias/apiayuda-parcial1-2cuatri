using api_parcial_1.Data;
using Microsoft.EntityFrameworkCore;

namespace api_parcial_1
{
    public class AppDbContext : DbContext
    {

        public DbSet<Person> Persons { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Person>()
                .HasIndex(p => p.Dni)
                .IsUnique();
        }

    }
}
