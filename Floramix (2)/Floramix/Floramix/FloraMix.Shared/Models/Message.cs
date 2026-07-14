namespace FloraMix.Shared.Models;

public enum MessageSender { Shop, Customer }

public class Message
{
    public int Id { get; set; }
    public int ConversationId { get; set; }
    public MessageSender Sender { get; set; }
    public string Text { get; set; } = "";
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
    public bool IsRead { get; set; }
}