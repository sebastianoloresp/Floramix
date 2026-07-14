namespace FloraMix.Shared.Models;

public class Conversation
{
    public int Id { get; set; }
    public int ShopId { get; set; }
    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;
    public List<Message> Messages { get; set; } = new();
}