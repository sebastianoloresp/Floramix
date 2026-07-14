using System.Collections.Generic;
using System.Threading.Tasks;
using FloraMix.Models;

namespace FloraMix.Services
{
    public static class NotificationManager
    {
        private static DatabaseService? _db;

        public static List<NotificationItem> Notifications { get; private set; } = new List<NotificationItem>();

        public static async Task InitializeAsync(DatabaseService db)
        {
            _db = db;

            var saved = await db.GetNotificationsAsync();
            if (saved.Count > 0)
            {
                Notifications = saved;
                return;
            }

            // First launch: seed example notifications, then persist them.
            var seedNotifications = new List<NotificationItem>
            {
                new NotificationItem { IconGlyph = "\uE7B8", IconBackground = "Neutral", Title = "Order Dispatched!", Message = "Your Rose & Ferrero Rocher Combo is on its way.", TimeAgo = "2 min ago", IsUnread = true },
                new NotificationItem { IconGlyph = "\uE8BD", IconBackground = "Blue", Title = "New message from Wild Flowers", Message = "Your order is being prepared right now...", TimeAgo = "18 min ago", IsUnread = true },
                new NotificationItem { IconGlyph = "\uEC92", IconBackground = "Yellow", Title = "Mum's Birthday in 3 days \uD83C\uDF82", Message = "Don't forget to send flowers!", TimeAgo = "1 hr ago", IsUnread = true },
                new NotificationItem { IconGlyph = "\uEB51", IconBackground = "Pink", Title = "10% off this weekend", Message = "Use code WEEKEND10 before Sunday midnight.", TimeAgo = "Yesterday", IsUnread = false },
                new NotificationItem { IconGlyph = "\uE7B8", IconBackground = "Neutral", Title = "Order Delivered", Message = "Your Sunflower Bouquet has been delivered.", TimeAgo = "May 28", IsUnread = false },
            };

            foreach (var note in seedNotifications)
                await db.SaveNotificationAsync(note); // assigns each notification its auto-increment Id

            Notifications = seedNotifications;
        }

        public static async void MarkAsReadAndSave(NotificationItem note)
        {
            if (!note.IsUnread) return;
            note.IsUnread = false;
            if (_db != null)
                await _db.SaveNotificationAsync(note);
        }

        public static async void MarkAllAsReadAndSave()
        {
            foreach (var note in Notifications)
                note.IsUnread = false;

            if (_db != null)
                foreach (var note in Notifications)
                    await _db.SaveNotificationAsync(note);
        }

        public static async void DeleteNotificationAndSave(NotificationItem note)
        {
            Notifications.Remove(note);
            if (_db != null)
                await _db.DeleteNotificationAsync(note);
        }
    }
}