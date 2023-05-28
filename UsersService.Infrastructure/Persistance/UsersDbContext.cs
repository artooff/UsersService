using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using UsersService.Application.Interfaces;
using UsersService.Domain.Models;

namespace UsersService.Infrastructure.Persistance
{
    public class UsersDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public UsersDbContext(DbContextOptions<UsersDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresExtension("uuid-ossp");

            modelBuilder.Entity<User>().HasKey(u => u.Guid);

            modelBuilder.Entity<User>()
                .Property(e => e.Guid)
                .HasDefaultValueSql("uuid_generate_v4()");

            modelBuilder.Entity<User>().HasIndex(u => u.Login).IsUnique();
        }
    }
}
