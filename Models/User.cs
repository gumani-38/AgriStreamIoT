using AgriStreamIoT.Models; // Adjust to match your User model namespace
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;

namespace AgriStreamIoT.Data
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = "Farmer";
    }
  
}