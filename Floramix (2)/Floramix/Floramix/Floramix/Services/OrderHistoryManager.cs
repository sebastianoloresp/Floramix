using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FloraMix.Models;

namespace FloraMix.Services
{
    public static class OrderHistoryManager
    {
        public static DatabaseService? Db { get; set; }

        public static List<OrderHistoryItem> Orders { get; private set; } = new List<OrderHistoryItem>();

        public static int ReviewedCount => Orders.Count(o => o.Rating > 0);

        public static string LastOrderShopName { get; private set; } = "The shop";

        public static async Task InitializeAsync(DatabaseService db)
        {
            Db = db;

            var savedOrders = await db.GetOrdersAsync();
            if (savedOrders.Count > 0)
            {
                Orders = savedOrders;
                LastOrderShopName = Orders[0].Shop;
            }
            else
            {
                var seedOrder = new OrderHistoryItem
                {
                    ImageSource = "bouquet_sunflower_yellow.png",
                    Name = "Sunflower Bouquet",
                    Shop = "Wild Flowers",
                    Date = "May 28, 2026",
                    Status = "Delivered",
                    Price = 1150,
                    Rating = 5,
                    Items = new List<OrderLineItem>
                    {
                        new OrderLineItem { ImageSource = "bouquet_sunflower_yellow.png", Name = "Sunflower Bouquet", Price = 1150, Quantity = 1 }
                    }
                };
                await db.SaveOrderAsync(seedOrder);
                Orders = new List<OrderHistoryItem> { seedOrder };
            }
        }

        public static async Task AddOrderFromCart()
        {
            if (CartManager.Items.Count == 0) return;

            LastOrderShopName = CartManager.Items[0].Shop;

            string name = CartManager.Items.Count == 1
                ? CartManager.Items[0].Name
                : CartManager.Items[0].Name + $" + {CartManager.Items.Count - 1} more";

            var lineItems = CartManager.Items.Select(i => new OrderLineItem
            {
                ImageSource = i.ImageSource,
                Name = i.Name,
                Price = i.Price,
                Quantity = i.Quantity
            }).ToList();

            var order = new OrderHistoryItem
            {
                ImageSource = CartManager.Items[0].ImageSource,
                Name = name,
                Shop = CartManager.Items[0].Shop,
                Date = DateTime.Now.ToString("MMM d, yyyy"),
                Status = "In Transit",
                Price = CartManager.GetTotal(),
                Rating = 0,
                Items = lineItems
            };

            // Build the API payload BEFORE anything awaits, while the cart is still intact
            var payload = new OrderApiService.OrderPayload
            {
                CustomerName = ProfileManager.FullName,
                CustomerEmail = ProfileManager.Email,
                DeliveryLabel = string.IsNullOrEmpty(CartManager.SelectedTimeSlot)
                    ? "Not scheduled"
                    : CartManager.SelectedTimeSlot,
                Occasion = CartManager.Items[0].ColorTag ?? "Other",
                Items = CartManager.Items.Select(i => new OrderApiService.OrderItemPayload
                {
                    BouquetName = i.Name,
                    Price = (decimal)i.Price,
                    Quantity = i.Quantity
                }).ToList()
            };

            Orders.Insert(0, order);

            if (Db != null)
                await Db.SaveOrderAsync(order);

            await OrderApiService.SubmitOrderAsync(payload);
        }

        public static async void SaveRatingChange(OrderHistoryItem order)
        {
            if (Db != null)
                await Db.SaveOrderAsync(order);
        }

        public static async void CancelOrder(OrderHistoryItem order)
        {
            order.Status = "Cancelled";
            if (Db != null)
                await Db.SaveOrderAsync(order);
        }

        public static async void RemoveOrder(OrderHistoryItem order)
        {
            Orders.Remove(order);
            if (Db != null)
                await Db.DeleteOrderAsync(order);
        }
    }
}