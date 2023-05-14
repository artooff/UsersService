using Microsoft.EntityFrameworkCore;
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
    }
}
