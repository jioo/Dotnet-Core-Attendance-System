using System.Linq;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApi.Entities;


namespace WebApi.Infrastructures
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // modelBuilder.Entity<Employee>()
            //     .Property(b => b.Created)
            //     .ValueGeneratedOnAdd()
            //     .HasDefaultValueSql("GETUTCDATE()");

            // modelBuilder.Entity<Employee>()
            //     .Property(b => b.Updated)
            //     .ValueGeneratedOnUpdate()
            //     .HasDefaultValueSql("GETUTCDATE()");

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Log> Logs { get; set; }

        public DbSet<Configuration> Configurations { get; set; }
    }
}