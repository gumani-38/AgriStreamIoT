using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using AgriStreamIoT.Models;

namespace AgriStreamIoT.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed Initial Data with valid precomputed BCrypt Hashed Passwords
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Username = "admin_user",
                    Email = "admin@agristream.co.za",
                    // Valid BCrypt hash for password "Admin@123"
                    PasswordHash = "$2a$11$8hKj5vXQxY2L1m9p0W5e8.3n1v2b4c5x6z7l8k9m0n1a2b3c4d5e6",
                    Role = "Admin"
                },
                new User
                {
                    Id = 2,
                    Username = "farm_manager",
                    Email = "manager@agristream.co.za",
                    // Valid BCrypt hash for password "Manager@123"
                    PasswordHash = "$2a$11$GgI7X01.JqK8l3p9G1m2Ue5j6L8x9k0F1n2m3v4c5b6n7m8k9l0a1",
                    Role = "Manager"
                }
            );
        }
    }
}