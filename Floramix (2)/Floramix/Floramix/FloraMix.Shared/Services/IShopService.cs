using FloraMix.Shared.Models;

namespace FloraMix.Shared.Services;

public interface IShopService
{
    Task<Shop?> GetShopAsync(int shopId);
    Task<Shop> SaveShopAsync(Shop shop);

    Task<List<Bouquet>> GetBouquetsAsync(int shopId);
    Task<Bouquet?> GetBouquetAsync(int bouquetId);
    Task<Bouquet> SaveBouquetAsync(Bouquet bouquet);
    Task DeleteBouquetAsync(int bouquetId);

    Task<List<Order>> GetOrdersAsync(int shopId);
    Task<Order?> GetOrderAsync(int orderId);
    Task UpdateOrderStatusAsync(int orderId, OrderStatus status);
    Task ArchiveOrderAsync(int orderId, bool archived);

    Task<List<Conversation>> GetConversationsAsync(int shopId);
    Task<Conversation?> GetConversationAsync(int conversationId);
    Task<Message> SendMessageAsync(int conversationId, MessageSender sender, string text);
    Task MarkConversationReadAsync(int conversationId);
    Task<int> GetUnreadMessageCountAsync(int shopId);
}