using System.Collections.Generic;
using System.Text.Json;
using SQLite;

namespace FloraMix.Models
{
    public class Conversation
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        // 0 = not tied to a specific order (e.g. general inbox chat)
        public int OrderId { get; set; } = 0;

        // The order's id on the web portal's database (Order.ServerOrderId).
        // 0 = order hasn't synced to the server yet, so chat stays local-only.
        public int ServerOrderId { get; set; } = 0;

        public string ShopName { get; set; }
        public string AvatarSource { get; set; }
        public string LastMessage { get; set; }
        public string TimeLabel { get; set; }
        public int UnreadCount { get; set; }
        public bool IsOnline { get; set; }

        public string MessagesJson { get; set; } = "[]";

        [Ignore]
        public List<ChatMessage> Messages
        {
            get => string.IsNullOrEmpty(MessagesJson) ? new List<ChatMessage>() : JsonSerializer.Deserialize<List<ChatMessage>>(MessagesJson);
            set => MessagesJson = JsonSerializer.Serialize(value);
        }
    }

    public class ChatMessage
    {
        public string Text { get; set; }
        public string TimeLabel { get; set; }
        public bool IsFromUser { get; set; }
    }

    public class OrderProgressStep
    {
        public string Name { get; set; }
        public string TimeLabel { get; set; }
        public bool IsDone { get; set; }
        public bool IsActive { get; set; }
    }
}