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

            // Seed Initial Data with BCrypt Hashed Passwords
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Username = "admin_user",
                    Email = "admin@agristream.co.za",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                    Role = "Admin"
                },
                new User
                {
                    Id = 2,
                    Username = "farm_manager",
                    Email = "manager@agristream.co.za",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Manager@123"),
                    Role = "Manager"
                }
            );
        }
    }
}