using Microsoft.EntityFrameworkCore;
using VeloGreen.Users.Api.Entities;

namespace VeloGreen.Users.Api.Storage
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }

        public DbSet<User> Users { get; set; }
    }
}
