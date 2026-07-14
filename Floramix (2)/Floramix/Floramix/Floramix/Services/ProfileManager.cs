using System.Threading.Tasks;
using FloraMix.Models;

namespace FloraMix.Services
{
    public static class ProfileManager
    {
        private static DatabaseService? _db;

        private static UserProfile _profile = new UserProfile
        {
            FullName = "Seb",
            Email = "emma.rose@example.com",
            Phone = "+44 7700 900123",
            Location = "London, UK",
            DarkModeEnabled = false,
            Language = "English",
            Currency = "GBP \u00A3",
            FaceIdEnabled = true,
            TwoFactorEnabled = false
        };

        public const string AppVersion = "v2.4.1";

        public static string FullName
        {
            get => _profile.FullName;
            set { _profile.FullName = value; Save(); }
        }

        public static string Email
        {
            get => _profile.Email;
            set { _profile.Email = value; Save(); }
        }

        public static string Phone
        {
            get => _profile.Phone;
            set { _profile.Phone = value; Save(); }
        }

        public static string Location
        {
            get => _profile.Location;
            set { _profile.Location = value; Save(); }
        }

        public static bool DarkModeEnabled
        {
            get => _profile.DarkModeEnabled;
            set { _profile.DarkModeEnabled = value; Save(); }
        }

        public static string Language
        {
            get => _profile.Language;
            set { _profile.Language = value; Save(); }
        }

        public static string Currency
        {
            get => _profile.Currency;
            set { _profile.Currency = value; Save(); }
        }

        public static bool FaceIdEnabled
        {
            get => _profile.FaceIdEnabled;
            set { _profile.FaceIdEnabled = value; Save(); }
        }

        public static bool TwoFactorEnabled
        {
            get => _profile.TwoFactorEnabled;
            set { _profile.TwoFactorEnabled = value; Save(); }
        }

        public static async Task InitializeAsync(DatabaseService db)
        {
            _db = db;

            var saved = await db.GetProfileAsync();
            if (saved != null)
                _profile = saved;
            else
                await db.SaveProfileAsync(_profile);
        }

        private static async void Save()
        {
            if (_db != null)
                await _db.SaveProfileAsync(_profile);
        }
    }
}