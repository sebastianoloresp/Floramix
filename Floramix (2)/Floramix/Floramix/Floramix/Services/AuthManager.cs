using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using FloraMix.Models;

namespace FloraMix.Services
{
    public static class AuthManager
    {
        private static DatabaseService? _db;

        public static UserAccount? CurrentAccount { get; private set; }

        public static void Initialize(DatabaseService db)
        {
            _db = db;
        }

        private static string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        public static async Task<(bool Success, string Error)> SignUpAsync(string fullName, string email, string password, string role)
        {
            if (_db == null)
                return (false, "Database not ready. Please try again.");

            if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return (false, "Please fill in all fields.");

            string normalizedEmail = email.Trim().ToLowerInvariant();

            var existing = await _db.GetAccountByEmailAsync(normalizedEmail);
            if (existing != null)
                return (false, "An account with that email already exists.");

            var account = new UserAccount
            {
                Email = normalizedEmail,
                PasswordHash = HashPassword(password),
                FullName = fullName.Trim(),
                Role = role,
                AuthProvider = "local"
            };

            await _db.SaveAccountAsync(account);
            CurrentAccount = account;
            return (true, "");
        }

        public static async Task<(bool Success, string Error)> SignInAsync(string email, string password)
        {
            if (_db == null)
                return (false, "Database not ready. Please try again.");

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return (false, "Please enter your email and password.");

            string normalizedEmail = email.Trim().ToLowerInvariant();

            var account = await _db.GetAccountByEmailAsync(normalizedEmail);
            if (account == null)
                return (false, "No account found with that email.");

            if (account.PasswordHash != HashPassword(password))
                return (false, "Incorrect password.");

            CurrentAccount = account;
            return (true, "");
        }

        // ---------- Real Google Sign-In ----------
        public static async Task<(bool Success, string Error)> SignInWithGoogleAsync(string role)
        {
            if (_db == null)
                return (false, "Database not ready. Please try again.");

            var (success, email, fullName, error) = await GoogleAuthService.SignInAsync();
            if (!success)
                return (false, error);

            string normalizedEmail = email.Trim().ToLowerInvariant();

            var existing = await _db.GetAccountByEmailAsync(normalizedEmail);
            if (existing != null)
            {
                CurrentAccount = existing;
                return (true, "");
            }

            var account = new UserAccount
            {
                Email = normalizedEmail,
                PasswordHash = HashPassword(Guid.NewGuid().ToString()), // unused for Google accounts
                FullName = fullName,
                Role = role,
                AuthProvider = "google"
            };

            await _db.SaveAccountAsync(account);
            CurrentAccount = account;
            return (true, "");
        }
    }
}