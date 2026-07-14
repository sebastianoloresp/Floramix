using SQLite;

namespace FloraMix.Models
{
    public class CartItem
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public int ProductId { get; set; }

        public string ImageSource { get; set; }
        public string Name { get; set; }
        public string Shop { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; } = 1;
        public string ColorTag { get; set; } = "Mixed";
    }
}