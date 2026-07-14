using SQLite;

namespace FloraMix.Models
{
    public class UserAccount
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Indexed(Unique = true)]
        public string Email { get; set; }

        public string PasswordHash { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; } = "customer"; // "customer" or "owner"
        public string AuthProvider { get; set; } = "local"; // "local" or "google"
    }
}