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
        public static async Task<Conversation> GetOrCreateOrderConversationAsync(int orderId, string shopName = "Wild Flowers", string avatarSource = "bouquet_rose_chocolate.png", int serverOrderId = 0)
        {
            var existing = Conversations.FirstOrDefault(c => c.OrderId == orderId && orderId != 0);
            if (existing != null)
            {
                // The order may have finished syncing to the server after the conversation
                // was first created locally (e.g. it was offline at checkout time).
                if (serverOrderId != 0 && existing.ServerOrderId != serverOrderId)
                {
                    existing.ServerOrderId = serverOrderId;
                    if (_db != null)
                        await _db.SaveConversationAsync(existing);
                }
                return existing;
            }

            var convo = new Conversation
            {
                OrderId = orderId,
                ServerOrderId = serverOrderId,
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

        // Pulls the latest messages from the web portal and merges any new ones in,
        // so replies typed by the shop owner on the portal show up in the app.
        public static async Task<bool> SyncFromServerAsync(Conversation conversation)
        {
            if (conversation.ServerOrderId == 0) return false;

            var remoteMessages = await MessageApiService.FetchMessagesAsync(conversation.ServerOrderId);
            if (remoteMessages == null) return false;

            var localMessages = conversation.Messages;
            bool changed = false;

            // The app doesn't track server message ids locally, so de-dupe by
            // (sender + text + minute-rounded timestamp) which is good enough here.
            bool AlreadyHave(MessageApiService.MessageDto remote) => localMessages.Any(m =>
                m.Text == remote.Text &&
                m.IsFromUser == (remote.Sender == "Customer") &&
                m.TimeLabel == remote.SentAt.ToLocalTime().ToString("h:mm tt"));

            foreach (var remote in remoteMessages.OrderBy(m => m.SentAt))
            {
                if (AlreadyHave(remote)) continue;

                localMessages.Add(new ChatMessage
                {
                    Text = remote.Text,
                    TimeLabel = remote.SentAt.ToLocalTime().ToString("h:mm tt"),
                    IsFromUser = remote.Sender == "Customer"
                });
                changed = true;
            }

            if (changed)
            {
                conversation.Messages = localMessages;
                var last = localMessages.LastOrDefault();
                if (last != null)
                {
                    conversation.LastMessage = last.Text;
                    conversation.TimeLabel = last.TimeLabel;
                }
                if (_db != null)
                    await _db.SaveConversationAsync(conversation);
            }

            return changed;
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