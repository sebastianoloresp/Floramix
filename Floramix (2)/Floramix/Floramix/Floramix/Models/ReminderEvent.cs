using System;
using SQLite;

namespace FloraMix.Models
{
    public class ReminderEvent
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }
        public string PersonOrLabel { get; set; }
        public string Type { get; set; } // "birthday", "anniversary", "holiday"
        public int Day { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
    }
}