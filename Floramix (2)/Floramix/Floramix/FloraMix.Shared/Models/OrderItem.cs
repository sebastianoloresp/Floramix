namespace FloraMix.Shared.Models;

public class OrderItem
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int BouquetId { get; set; }
    public string BouquetName { get; set; } = "";
    public string? ImageUrl { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
}