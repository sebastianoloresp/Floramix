using FloraMix.Shared.Models;
using FloraMix.Shared.Services;
using FloraMix.Web.Data;
using Microsoft.EntityFrameworkCore;

namespace FloraMix.Web.Services;

public class ShopService : IShopService
{
    private readonly IDbContextFactory<FloraMixDbContext> _dbFactory;

    public ShopService(IDbContextFactory<FloraMixDbContext> dbFactory)
    {
        _dbFactory = dbFactory;
    }

    // ---- Shop ----

    public async Task<Shop?> GetShopAsync(int shopId)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        return await db.Shops.FindAsync(shopId);
    }

    public async Task<Shop> SaveShopAsync(Shop shop)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();

        if (shop.Id == 0)
        {
            db.Shops.Add(shop);
        }
        else
        {
            db.Shops.Update(shop);
        }

        await db.SaveChangesAsync();
        return shop;
    }

    // ---- Bouquets ----

    public async Task<List<Bouquet>> GetBouquetsAsync(int shopId)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        return await db.Bouquets
            .Where(b => b.ShopId == shopId)
            .OrderBy(b => b.Id)
            .ToListAsync();
    }

    public async Task<Bouquet?> GetBouquetAsync(int bouquetId)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        return await db.Bouquets.FindAsync(bouquetId);
    }

    public async Task<Bouquet> SaveBouquetAsync(Bouquet bouquet)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();

        if (bouquet.Id == 0)
        {
            db.Bouquets.Add(bouquet);
        }
        else
        {
            db.Bouquets.Update(bouquet);
        }

        await db.SaveChangesAsync();
        return bouquet;
    }

    public async Task DeleteBouquetAsync(int bouquetId)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        var bouquet = await db.Bouquets.FindAsync(bouquetId);
        if (bouquet is not null)
        {
            db.Bouquets.Remove(bouquet);
            await db.SaveChangesAsync();
        }
    }

    // ---- Orders ----

    public async Task<List<Order>> GetOrdersAsync(int shopId)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        return await db.Orders
            .Include(o => o.Items)
            .Where(o => o.ShopId == shopId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
    }

    public async Task<Order?> GetOrderAsync(int orderId)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        return await db.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == orderId);
    }

    public async Task UpdateOrderStatusAsync(int orderId, OrderStatus status)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        var order = await db.Orders.FindAsync(orderId);
        if (order is not null)
        {
            order.Status = status;
            await db.SaveChangesAsync();
        }
    }

    public async Task ArchiveOrderAsync(int orderId, bool archived)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        var order = await db.Orders.FindAsync(orderId);
        if (order is not null)
        {
            order.IsArchived = archived;
            await db.SaveChangesAsync();
        }
    }

    // ---- Conversations & Messages ----

    public async Task<List<Conversation>> GetConversationsAsync(int shopId)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        var conversations = await db.Conversations
            .Include(c => c.Order)
            .Include(c => c.Messages)
            .Where(c => c.ShopId == shopId)
            .ToListAsync();

        return conversations
            .OrderByDescending(c => c.Messages.Count == 0 ? DateTime.MinValue : c.Messages.Max(m => m.SentAt))
            .ToList();
    }

    public async Task<Conversation?> GetConversationAsync(int conversationId)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        return await db.Conversations
            .Include(c => c.Order)
            .Include(c => c.Messages)
            .FirstOrDefaultAsync(c => c.Id == conversationId);
    }

    public async Task<Message> SendMessageAsync(int conversationId, MessageSender sender, string text)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        var message = new Message
        {
            ConversationId = conversationId,
            Sender = sender,
            Text = text,
            SentAt = DateTime.UtcNow,
            IsRead = sender == MessageSender.Shop
        };

        db.Messages.Add(message);
        await db.SaveChangesAsync();
        return message;
    }

    public async Task MarkConversationReadAsync(int conversationId)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        var unread = await db.Messages
            .Where(m => m.ConversationId == conversationId
                && m.Sender == MessageSender.Customer
                && !m.IsRead)
            .ToListAsync();

        foreach (var m in unread)
        {
            m.IsRead = true;
        }

        if (unread.Count > 0)
        {
            await db.SaveChangesAsync();
        }
    }

    public async Task<int> GetUnreadMessageCountAsync(int shopId)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        var conversationIds = await db.Conversations
            .Where(c => c.ShopId == shopId)
            .Select(c => c.Id)
            .ToListAsync();

        return await db.Messages
            .Where(m => conversationIds.Contains(m.ConversationId)
                && m.Sender == MessageSender.Customer
                && !m.IsRead)
            .CountAsync();
    }
}