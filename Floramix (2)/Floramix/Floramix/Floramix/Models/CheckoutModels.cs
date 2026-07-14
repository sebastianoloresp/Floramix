using SQLite;

namespace FloraMix.Models
{
    public class SavedCard
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Brand { get; set; }
        public string Last4 { get; set; }
        public string ExpiryText { get; set; }
        public string CardHolder { get; set; }
        public bool IsSelected { get; set; }
    }
}