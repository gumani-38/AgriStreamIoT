namespace AgriStreamIoT.Models
{
    public class User
    {
  
            public int Id { get; set; }
            public string Username { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty; // In production, store hashed passwords
            public string Role { get; set; } = string.Empty; // Farmer, Agronomist, Admin
        

}
}
