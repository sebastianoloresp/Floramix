using FloraMix.Shared.Models;
using FloraMix.Shared.Services;

namespace FloraMix.Services
{
    // In-memory IShopService implementation for the MAUI (BlazorWebView) owner pages.
    // FloraMix.Web's ShopService uses EF Core + SQL Server, which isn't available
    // on-device, so this gives the owner dashboard something to run against.
    public class MauiShopService : IShopService
    {
        private readonly List<Shop> _shops = new();
        private readonly List<Bouquet> _bouquets = new();
        private readonly List<Order> _orders = new();
        private readonly List<Conversation> _conversations = new();
        private int _nextBouquetId = 1;
        private int _nextOrderId = 1;
        private int _nextConversationId = 1;
        private int _nextMessageId = 1;

        public MauiShopService()
        {
            Seed();
        }

        private void Seed()
        {
            _shops.Add(new Shop
            {
                Id = 1,
                Name = "La Petite Fleur",
                Address = "123 Main St",
                Location = "Marylebone, London",
                Hours = "Mon–Sat 9am–6pm"
            });

            _bouquets.AddRange(new[]
            {
                new Bouquet { Id = _nextBouquetId++, ShopId = 1, Name = "Garden Reverie", Price = 58, StockCount = 5, Category = Occasion.Wedding, Rating = 4.8, SoldCount = 204, Status = ProductStatus.Active, ImageUrl = "/images/products/garden-reverie.png" },
                new Bouquet { Id = _nextBouquetId++, ShopId = 1, Name = "Soft Whisper", Price = 52, StockCount = 3, Category = Occasion.Sympathy, Rating = 4.9, SoldCount = 167, Status = ProductStatus.LowStock, ImageUrl = "/images/products/soft-whisper.png" },
                new Bouquet { Id = _nextBouquetId++, ShopId = 1, Name = "Eternal Bliss Bouquet", Price = 68, StockCount = 8, Category = Occasion.Romance, Rating = 4.9, SoldCount = 127, Status = ProductStatus.Active, ImageUrl = "/images/products/eternal-bliss.png" },
                new Bouquet { Id = _nextBouquetId++, ShopId = 1, Name = "Wild at Heart", Price = 72, StockCount = 0, Category = Occasion.Romance, Rating = 4.7, SoldCount = 93, Status = ProductStatus.OutOfStock, ImageUrl = "/images/products/wild-at-heart.png" },
                new Bouquet { Id = _nextBouquetId++, ShopId = 1, Name = "Spring Delight", Price = 45, StockCount = 15, Category = Occasion.Birthday, Rating = 4.7, SoldCount = 89, Status = ProductStatus.Active, ImageUrl = "/images/products/spring-delight.png" },
                new Bouquet { Id = _nextBouquetId++, ShopId = 1, Name = "Sunrise Melody", Price = 38, StockCount = 20, Category = Occasion.Birthday, Rating = 4.6, SoldCount = 52, Status = ProductStatus.Active, ImageUrl = "/images/products/sunrise-melody.png" },
            });

            var emma = new Order { Id = _nextOrderId++, ShopId = 1, CustomerName = "Emma Rose", CustomerEmail = "emma@example.com", Status = OrderStatus.Preparing, DeliveryLabel = "Today, 2–4 PM", Occasion = Occasion.Romance, CreatedAt = new DateTime(2026, 6, 3, 11, 30, 0), Items = new() { new OrderItem { BouquetName = "Eternal Bliss Bouquet", Price = 68, Quantity = 1 } } };
            var sophie = new Order { Id = _nextOrderId++, ShopId = 1, CustomerName = "Sophie Chen", CustomerEmail = "sophie@example.com", Status = OrderStatus.Ready, DeliveryLabel = "Today, 12–2 PM", Occasion = Occasion.Wedding, CreatedAt = new DateTime(2026, 6, 3, 10, 15, 0), Items = new() { new OrderItem { BouquetName = "Spring Delight", Price = 45, Quantity = 2 } } };
            var james = new Order { Id = _nextOrderId++, ShopId = 1, CustomerName = "James Taylor", CustomerEmail = "james@example.com", Status = OrderStatus.Pending, DeliveryLabel = "Today, 4–6 PM", Occasion = Occasion.Birthday, CreatedAt = new DateTime(2026, 6, 3, 10, 45, 0), Items = new() { new OrderItem { BouquetName = "Garden Reverie", Price = 58, Quantity = 1 } } };
            var isabella = new Order { Id = _nextOrderId++, ShopId = 1, CustomerName = "Isabella Moore", CustomerEmail = "isa@example.com", Status = OrderStatus.Delivered, DeliveryLabel = "Delivered 9:15 AM", Occasion = Occasion.Other, CreatedAt = new DateTime(2026, 6, 3, 8, 30, 0), Items = new() { new OrderItem { BouquetName = "Soft Whisper", Price = 52, Quantity = 1 } } };
            var charlotte = new Order { Id = _nextOrderId++, ShopId = 1, CustomerName = "Charlotte Brown", CustomerEmail = "char@example.com", Status = OrderStatus.Delivered, DeliveryLabel = "Delivered Jun 1", Occasion = Occasion.Birthday, CreatedAt = new DateTime(2026, 6, 1, 14, 45, 0), Items = new() { new OrderItem { BouquetName = "Pink Bliss Custom", Price = 95, Quantity = 1 } } };

            _orders.AddRange(new[] { emma, sophie, james, isabella, charlotte });

            AddConversation(emma, new[]
            {
                (MessageSender.Customer, "Hi! I wanted to add a personalised card message to my order. Is that possible?", false),
                (MessageSender.Shop, "Of course! We'd be happy to add a personal touch. What would you like it to say? 🌸", true),
                (MessageSender.Customer, "\"Happy Birthday Mum, with all my love\" — is that okay?", false),
                (MessageSender.Shop, "Perfect, that's beautiful! We'll write it on our signature cream card. ✨", true),
            });
            AddConversation(sophie, new[] { (MessageSender.Customer, "Thank you, they were gorgeous! 🌷", true) });
            AddConversation(james, new[] { (MessageSender.Customer, "What time will the delivery arrive?", false) });
            AddConversation(isabella, new[] { (MessageSender.Customer, "Perfect, thank you so much!", true) });
            AddConversation(charlotte, new[] { (MessageSender.Customer, "Do you do same-day delivery?", true) });
        }

        private void AddConversation(Order order, (MessageSender Sender, string Text, bool IsRead)[] messages)
        {
            var convo = new Conversation { Id = _nextConversationId++, ShopId = order.ShopId, OrderId = order.Id, Order = order };
            foreach (var m in messages)
            {
                convo.Messages.Add(new Message
                {
                    Id = _nextMessageId++,
                    ConversationId = convo.Id,
                    Sender = m.Sender,
                    Text = m.Text,
                    SentAt = order.CreatedAt,
                    IsRead = m.IsRead
                });
            }
            _conversations.Add(convo);
        }

        public Task<Shop?> GetShopAsync(int shopId) =>
            Task.FromResult(_shops.FirstOrDefault(s => s.Id == shopId));

        public Task<Shop> SaveShopAsync(Shop shop)
        {
            var existing = _shops.FirstOrDefault(s => s.Id == shop.Id);
            if (existing != null)
                _shops.Remove(existing);
            if (shop.Id == 0)
                shop.Id = _shops.Count == 0 ? 1 : _shops.Max(s => s.Id) + 1;
            _shops.Add(shop);
            return Task.FromResult(shop);
        }

        public Task<List<Bouquet>> GetBouquetsAsync(int shopId) =>
            Task.FromResult(_bouquets.Where(b => b.ShopId == shopId).ToList());

        public Task<Bouquet?> GetBouquetAsync(int bouquetId) =>
            Task.FromResult(_bouquets.FirstOrDefault(b => b.Id == bouquetId));

        public Task<Bouquet> SaveBouquetAsync(Bouquet bouquet)
        {
            var existing = _bouquets.FirstOrDefault(b => b.Id == bouquet.Id);
            if (existing != null)
                _bouquets.Remove(existing);
            if (bouquet.Id == 0)
                bouquet.Id = _nextBouquetId++;
            _bouquets.Add(bouquet);
            return Task.FromResult(bouquet);
        }

        public Task DeleteBouquetAsync(int bouquetId)
        {
            _bouquets.RemoveAll(b => b.Id == bouquetId);
            return Task.CompletedTask;
        }

        public Task<List<Order>> GetOrdersAsync(int shopId) =>
            Task.FromResult(_orders.Where(o => o.ShopId == shopId).ToList());

        public Task<Order?> GetOrderAsync(int orderId) =>
            Task.FromResult(_orders.FirstOrDefault(o => o.Id == orderId));

        public Task UpdateOrderStatusAsync(int orderId, OrderStatus status)
        {
            var order = _orders.FirstOrDefault(o => o.Id == orderId);
            if (order != null)
                order.Status = status;
            return Task.CompletedTask;
        }

        public Task ArchiveOrderAsync(int orderId, bool archived)
        {
            var order = _orders.FirstOrDefault(o => o.Id == orderId);
            if (order != null)
                order.IsArchived = archived;
            return Task.CompletedTask;
        }

        public Task<List<Conversation>> GetConversationsAsync(int shopId) =>
            Task.FromResult(_conversations.Where(c => c.ShopId == shopId).ToList());

        public Task<Conversation?> GetConversationAsync(int conversationId) =>
            Task.FromResult(_conversations.FirstOrDefault(c => c.Id == conversationId));

        public Task<Message> SendMessageAsync(int conversationId, MessageSender sender, string text)
        {
            var convo = _conversations.FirstOrDefault(c => c.Id == conversationId);
            var message = new Message
            {
                Id = _nextMessageId++,
                ConversationId = conversationId,
                Sender = sender,
                Text = text,
                SentAt = DateTime.Now,
                IsRead = sender == MessageSender.Shop
            };
            convo?.Messages.Add(message);
            return Task.FromResult(message);
        }

        public Task MarkConversationReadAsync(int conversationId)
        {
            var convo = _conversations.FirstOrDefault(c => c.Id == conversationId);
            if (convo != null)
            {
                foreach (var m in convo.Messages.Where(m => m.Sender == MessageSender.Customer))
                    m.IsRead = true;
            }
            return Task.CompletedTask;
        }

        public Task<int> GetUnreadMessageCountAsync(int shopId)
        {
            var count = _conversations
                .Where(c => c.ShopId == shopId)
                .SelectMany(c => c.Messages)
                .Count(m => m.Sender == MessageSender.Customer && !m.IsRead);
            return Task.FromResult(count);
        }
    }
}