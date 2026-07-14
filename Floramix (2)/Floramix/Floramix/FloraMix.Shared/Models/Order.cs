namespace FloraMix.Shared.Models;

public enum OrderStatus { Pending, Confirmed, Preparing, Ready, Delivered, Cancelled }

public enum Occasion { Romance, Birthday, Wedding, Sympathy, Other }

public class Order
{
    public int Id { get; set; }
    public bool IsArchived { get; set; } = false;
    public int ShopId { get; set; }
    public string CustomerName { get; set; } = "";
    public string CustomerEmail { get; set; } = "";
    public string DeliveryLabel { get; set; } = "";
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public Occasion Occasion { get; set; } = Occasion.Other;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public List<OrderItem> Items { get; set; } = new();

    public decimal Total => Items.Sum(i => i.Price * i.Quantity);
    public string OrderCode => $"FM-{2837 + Id}";
}