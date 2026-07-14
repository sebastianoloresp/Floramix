using System.Collections.Generic;
using System.Threading.Tasks;
using FloraMix.Models;

namespace FloraMix.Services
{
    public static class ReminderManager
    {
        private static DatabaseService? _db;

        public static List<ReminderEvent> Events { get; private set; } = new List<ReminderEvent>();

        public static async Task InitializeAsync(DatabaseService db)
        {
            _db = db;

            var saved = await db.GetRemindersAsync();
            if (saved.Count > 0)
            {
                Events = saved;
            }
            else
            {
                var seedEvents = new List<ReminderEvent>
                {
                    new ReminderEvent { Name = "Mum's Birthday", PersonOrLabel = "Mum", Type = "birthday", Day = 8, Month = 6, Year = 2026 },
                    new ReminderEvent { Name = "Wedding Anniversary", PersonOrLabel = "Sophie & Tom", Type = "anniversary", Day = 15, Month = 6, Year = 2026 },
                    new ReminderEvent { Name = "Summer Solstice", PersonOrLabel = "Holiday", Type = "holiday", Day = 21, Month = 6, Year = 2026 },
                    new ReminderEvent { Name = "Dad's Birthday", PersonOrLabel = "Dad", Type = "birthday", Day = 22, Month = 6, Year = 2026 },
                };

                foreach (var ev in seedEvents)
                    await db.SaveReminderAsync(ev);

                Events = seedEvents;
            }
        }

        public static async void AddEvent(ReminderEvent e)
        {
            Events.Add(e);
            if (_db != null)
                await _db.SaveReminderAsync(e);
        }

        public static async void RemoveEvent(ReminderEvent e)
        {
            Events.Remove(e);
            if (_db != null)
                await _db.DeleteReminderAsync(e);
        }
    }
}