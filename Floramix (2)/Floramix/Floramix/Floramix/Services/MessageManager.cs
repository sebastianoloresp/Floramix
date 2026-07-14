using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FloraMix.Models;

namespace FloraMix.Services
{
    public static class MessageManager
    {
        private static DatabaseService? _db;

        public static List<Conversation> Conversations { get; private set; } = new List<Conversation>();

        public static async Task InitializeAsync(DatabaseService db)
        {
            _db = db;
            Conversations = await db.GetConversationsAsync();
        }

        // Returns the existing conversation for this order if one exists,
        // otherwise creates and persists a new one.
        public static async Task<Conversation> GetOrCreateOrderConversationAsync(int orderId, string shopName = "Wild Flowers", string avatarSource = "bouquet_rose_chocolate.png")
        {
            var existing = Conversations.FirstOrDefault(c => c.OrderId == orderId && orderId != 0);
            if (existing != null)
                return existing;

            var convo = new Conversation
            {
                OrderId = orderId,
                ShopName = shopName,
                AvatarSource = avatarSource,
                LastMessage = "",
                TimeLabel = "Now",
                UnreadCount = 0,
                IsOnline = true,
                Messages = new List<ChatMessage>()
            };

            Conversations.Insert(0, convo);

            if (_db != null)
                await _db.SaveConversationAsync(convo);

            return convo;
        }

        public static async void SaveConversation(Conversation conversation)
        {
            if (_db != null)
                await _db.SaveConversationAsync(conversation);
        }

        public static async void MarkAsReadAndSave(Conversation conversation)
        {
            conversation.UnreadCount = 0;
            if (_db != null)
                await _db.SaveConversationAsync(conversation);
        }
    }
}