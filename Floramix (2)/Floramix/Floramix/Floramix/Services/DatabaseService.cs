using FloraMix.Models;
using SQLite;

namespace FloraMix.Services
{
    public class DatabaseService
    {
        private SQLiteAsyncConnection? _database;

        private async Task Init()
        {
            if (_database is not null)
                return;

            _database = new SQLiteAsyncConnection(Constants.DatabasePath);

            await _database.CreateTableAsync<Product>();
            await _database.CreateTableAsync<CartItem>();
            await _database.CreateTableAsync<WishlistItem>();
            await _database.CreateTableAsync<SavedCard>();
            await _database.CreateTableAsync<SavedAddress>();
            await _database.CreateTableAsync<OrderHistoryItem>();
            await _database.CreateTableAsync<UserProfile>();
            await _database.CreateTableAsync<ReminderEvent>();
            await _database.CreateTableAsync<Conversation>();
            await _database.CreateTableAsync<NotificationItem>();
            await _database.CreateTableAsync<UserAccount>();
        }

        // ---------- Product methods ----------

        public async Task<List<Product>> GetProductsAsync()
        {
            await Init();
            return await _database!.Table<Product>().ToListAsync();
        }

        public async Task<int> GetProductCountAsync()
        {
            await Init();
            return await _database!.Table<Product>().CountAsync();
        }

        public async Task<Product?> GetProductAsync(int id)
        {
            await Init();
            return await _database!.Table<Product>().Where(p => p.Id == id).FirstOrDefaultAsync();
        }

        public async Task<int> SaveProductAsync(Product product)
        {
            await Init();
            if (product.Id != 0)
                return await _database!.UpdateAsync(product);
            else
                return await _database!.InsertAsync(product);
        }

        public async Task DeleteAllProductsAsync()
        {
            await Init();
            await _database!.DeleteAllAsync<Product>();
        }

        public async Task SaveProductFromApiAsync(Product product)
        {
            await Init();
            await _database!.InsertOrReplaceAsync(product);
        }

        // ---------- CartItem methods ----------

        public async Task<List<CartItem>> GetCartItemsAsync()
        {
            await Init();
            return await _database!.Table<CartItem>().ToListAsync();
        }

        public async Task<int> SaveCartItemAsync(CartItem item)
        {
            await Init();
            if (item.Id != 0)
                return await _database!.UpdateAsync(item);
            else
                return await _database!.InsertAsync(item);
        }

        public async Task<int> DeleteCartItemAsync(CartItem item)
        {
            await Init();
            return await _database!.DeleteAsync(item);
        }

        public async Task<int> ClearCartAsync()
        {
            await Init();
            return await _database!.DeleteAllAsync<CartItem>();
        }

        // ---------- Wishlist methods ----------

        public async Task<List<WishlistItem>> GetWishlistAsync()
        {
            await Init();
            return await _database!.Table<WishlistItem>().ToListAsync();
        }

        public async Task<int> SaveWishlistItemAsync(WishlistItem item)
        {
            await Init();
            if (item.Id != 0)
                return await _database!.UpdateAsync(item);
            else
                return await _database!.InsertAsync(item);
        }

        public async Task<int> DeleteWishlistItemAsync(WishlistItem item)
        {
            await Init();
            return await _database!.DeleteAsync(item);
        }

        // ---------- SavedCard methods ----------

        public async Task<List<SavedCard>> GetCardsAsync()
        {
            await Init();
            return await _database!.Table<SavedCard>().ToListAsync();
        }

        public async Task<int> SaveCardAsync(SavedCard card)
        {
            await Init();
            if (card.Id != 0)
                return await _database!.UpdateAsync(card);
            else
                return await _database!.InsertAsync(card);
        }

        public async Task<int> DeleteCardAsync(SavedCard card)
        {
            await Init();
            return await _database!.DeleteAsync(card);
        }

        // ---------- SavedAddress methods ----------

        public async Task<List<SavedAddress>> GetAddressesAsync()
        {
            await Init();
            return await _database!.Table<SavedAddress>().ToListAsync();
        }

        public async Task<int> SaveAddressAsync(SavedAddress address)
        {
            await Init();
            if (address.Id != 0)
                return await _database!.UpdateAsync(address);
            else
                return await _database!.InsertAsync(address);
        }

        public async Task<int> DeleteAddressAsync(SavedAddress address)
        {
            await Init();
            return await _database!.DeleteAsync(address);
        }

        // ---------- OrderHistoryItem methods ----------

        public async Task<List<OrderHistoryItem>> GetOrdersAsync()
        {
            await Init();
            // Newest orders first
            return await _database!.Table<OrderHistoryItem>().OrderByDescending(o => o.Id).ToListAsync();
        }

        public async Task<int> SaveOrderAsync(OrderHistoryItem order)
        {
            await Init();
            if (order.Id != 0)
                return await _database!.UpdateAsync(order);
            else
                return await _database!.InsertAsync(order);
        }

        public async Task<int> DeleteOrderAsync(OrderHistoryItem order)
        {
            await Init();
            return await _database!.DeleteAsync(order);
        }

        // ---------- UserProfile methods ----------

        public async Task<UserProfile?> GetProfileAsync()
        {
            await Init();
            return await _database!.Table<UserProfile>().FirstOrDefaultAsync();
        }

        public async Task<int> SaveProfileAsync(UserProfile profile)
        {
            await Init();
            profile.Id = 1; // single-row table
            return await _database!.InsertOrReplaceAsync(profile);
        }

        // ---------- ReminderEvent methods ----------

        public async Task<List<ReminderEvent>> GetRemindersAsync()
        {
            await Init();
            return await _database!.Table<ReminderEvent>().ToListAsync();
        }

        public async Task<int> SaveReminderAsync(ReminderEvent reminder)
        {
            await Init();
            if (reminder.Id != 0)
                return await _database!.UpdateAsync(reminder);
            else
                return await _database!.InsertAsync(reminder);
        }

        public async Task<int> DeleteReminderAsync(ReminderEvent reminder)
        {
            await Init();
            return await _database!.DeleteAsync(reminder);
        }

        // ---------- Conversation methods ----------

        public async Task<List<Conversation>> GetConversationsAsync()
        {
            await Init();
            return await _database!.Table<Conversation>().ToListAsync();
        }

        public async Task<int> SaveConversationAsync(Conversation conversation)
        {
            await Init();
            if (conversation.Id != 0)
                return await _database!.UpdateAsync(conversation);
            else
                return await _database!.InsertAsync(conversation);
        }

        public async Task<int> DeleteConversationAsync(Conversation conversation)
        {
            await Init();
            return await _database!.DeleteAsync(conversation);
        }

        // ---------- NotificationItem methods ----------

        public async Task<List<NotificationItem>> GetNotificationsAsync()
        {
            await Init();
            return await _database!.Table<NotificationItem>().ToListAsync();
        }

        public async Task<int> SaveNotificationAsync(NotificationItem notification)
        {
            await Init();
            if (notification.Id != 0)
                return await _database!.UpdateAsync(notification);
            else
                return await _database!.InsertAsync(notification);
        }

        public async Task<int> DeleteNotificationAsync(NotificationItem notification)
        {
            await Init();
            return await _database!.DeleteAsync(notification);
        }

        // ---------- UserAccount methods ----------

        public async Task<UserAccount?> GetAccountByEmailAsync(string email)
        {
            await Init();
            return await _database!.Table<UserAccount>().Where(a => a.Email == email).FirstOrDefaultAsync();
        }

        public async Task<int> SaveAccountAsync(UserAccount account)
        {
            await Init();
            if (account.Id != 0)
                return await _database!.UpdateAsync(account);
            else
                return await _database!.InsertAsync(account);
        }
    }
}