using System;
using FloraMix.Services;
using Microsoft.Maui.Controls;

namespace FloraMix.Views.Main
{
    public partial class OrderConfirmationPage : ContentPage
    {
        public OrderConfirmationPage()
        {
            InitializeComponent();

            var shopName = FloraMix.Services.OrderHistoryManager.LastOrderShopName;
            ShopMessageLabel.Text = shopName + " has received your order and will begin preparing it shortly.";

            var latestOrder = OrderHistoryManager.Orders.Count > 0 ? OrderHistoryManager.Orders[0] : null;
            OrderNumberLabel.Text = !string.IsNullOrEmpty(latestOrder?.OrderCode)
                ? "Order #" + latestOrder.OrderCode
                : "Order pending sync...";
        }
        private async void OnTrackOrderTapped(object sender, EventArgs e)
        {
            var latestOrder = OrderHistoryManager.Orders.Count > 0 ? OrderHistoryManager.Orders[0] : null;
            await Navigation.PushAsync(new OrderTrackingPage(latestOrder));
        }

        private async void OnMessageShopTapped(object sender, EventArgs e)
        {
            var latestOrder = OrderHistoryManager.Orders.Count > 0 ? OrderHistoryManager.Orders[0] : null;
            int orderId = latestOrder?.Id ?? 0;
            string shopName = latestOrder?.Shop ?? "Wild Flowers";
            await Navigation.PushAsync(new ChatPage(await MessageManager.GetOrCreateOrderConversationAsync(orderId, shopName)));
        }

        private async void OnBackToCartTapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CartPage());
        }
    }
}