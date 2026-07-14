using System.Collections.Generic;
using System.Text.Json;
using SQLite;

namespace FloraMix.Models
{
    public class OrderLineItem
    {
        public string ImageSource { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
    }

    public class OrderHistoryItem
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string ImageSource { get; set; }
        public string Name { get; set; }
        public string Shop { get; set; }
        public string Date { get; set; }
        public string Status { get; set; } // "In Transit" or "Delivered"
        public double Price { get; set; }
        public int Rating { get; set; } // 0 if not yet rated

        public string ItemsJson { get; set; } = "[]";

        [Ignore]
        public List<OrderLineItem> Items
        {
            get => string.IsNullOrEmpty(ItemsJson) ? new List<OrderLineItem>() : JsonSerializer.Deserialize<List<OrderLineItem>>(ItemsJson);
            set => ItemsJson = JsonSerializer.Serialize(value);
        }
    }

    public class SavedAddress
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Label { get; set; }
        public string AddressType { get; set; } = "Home"; // "Home", "Work", "Other"
        public bool IsDefault { get; set; }
        public string AddressLine { get; set; }
        public string ApartmentLine { get; set; }
        public string CityLine { get; set; }
        public string City { get; set; }
        public string Postcode { get; set; }
    }

    public class NotificationItem
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string IconGlyph { get; set; }
        public string IconBackground { get; set; } // "Pink", "Blue", "Yellow", "Neutral"
        public string Title { get; set; }
        public string Message { get; set; }
        public string TimeAgo { get; set; }
        public bool IsUnread { get; set; }
    }

    public class WishlistItem
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string ImageSource { get; set; }
        public string Shop { get; set; }
        public string Name { get; set; }
        public string Tag { get; set; }
        public double Price { get; set; }
    }

    // Single-row table holding the current user's profile & settings.
    public class UserProfile
    {
        [PrimaryKey]
        public int Id { get; set; } = 1;

        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Location { get; set; }

        public bool DarkModeEnabled { get; set; }
        public string Language { get; set; }
        public string Currency { get; set; }
        public bool FaceIdEnabled { get; set; }
        public bool TwoFactorEnabled { get; set; }
    }
}