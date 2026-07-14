using System;
using System.Collections.Generic;
using FloraMix.Models;
using FloraMix.Services;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Graphics;

namespace FloraMix.Views.Main
{
    public partial class OrderTrackingPage : ContentPage
    {
        private const string ShopName = "Wild Flowers";
        private const string ShopPhoneNumber = "09054269748";

        private readonly OrderHistoryItem _order;

        public OrderTrackingPage() : this(null) { }

        public OrderTrackingPage(OrderHistoryItem order)
        {
            InitializeComponent();
            _order = order;

            string addressText = "Paseo de Sta. Rosa, Sta. Rosa, Laguna";

            var address = CartManager.SelectedAddress;
            if (address != null)
                addressText = address.AddressLine + ", " + address.CityLine.Split(',')[0];

            DeliveryAddressLabel.Text = addressText;
            LoadMap(addressText);

            BuildEstimatedDelivery();
            BuildProgress();
            BuildOrderDetails();
            UpdateStatusUI();
        }

        private void UpdateStatusUI()
        {
            if (_order == null) return;

            StatusBadgeLabel.Text = _order.Status;

            bool isCancellable = _order.Status == "In Transit";
            CancelOrderButton.IsVisible = isCancellable;

            if (_order.Status == "Cancelled")
            {
                StatusBadgeLabel.TextColor = Color.FromArgb("#D9534F");
                if (StatusBadgeLabel.Parent is Border badgeBorder)
                    badgeBorder.BackgroundColor = Color.FromArgb("#FCE8E8");
            }
        }

        private void LoadMap(string address)
        {
            string query = Uri.EscapeDataString(address + ", Philippines");
            string mapUrl = $"https://www.google.com/maps?q={query}&output=embed";

            DeliveryMapView.Source = new HtmlWebViewSource
            {
                Html = $@"<html><body style='margin:0;padding:0;'>
                            <iframe width='100%' height='180' style='border:0;'
                                loading='lazy' allowfullscreen
                                src='{mapUrl}'>
                            </iframe>
                          </body></html>"
            };
        }

        private void BuildEstimatedDelivery()
        {
            var estimatedDelivery = DateTime.Now.AddMinutes(45);
            EstimatedTimeLabel.Text = "Today, " + estimatedDelivery.ToString("h:mm tt");
        }

        private void BuildProgress()
        {
            ProgressStack.Children.Clear();

            var now = DateTime.Now;
            var placedTime = now.AddMinutes(-90);
            var confirmedTime = now.AddMinutes(-87);
            var preparingTime = now.AddMinutes(-30);
            var outForDeliveryEst = now.AddMinutes(20);
            var deliveredEst = now.AddMinutes(45);

            var steps = new List<OrderProgressStep>
            {
                new OrderProgressStep { Name = "Order Placed", TimeLabel = placedTime.ToString("h:mm tt"), IsDone = true },
                new OrderProgressStep { Name = "Confirmed", TimeLabel = confirmedTime.ToString("h:mm tt"), IsDone = true },
                new OrderProgressStep { Name = "Preparing", TimeLabel = preparingTime.ToString("h:mm tt"), IsActive = true },
                new OrderProgressStep { Name = "Out for Delivery", TimeLabel = "Est. " + outForDeliveryEst.ToString("h:mm tt") },
                new OrderProgressStep { Name = "Delivered", TimeLabel = "Est. " + deliveredEst.ToString("h:mm tt") },
            };

            for (int i = 0; i < steps.Count; i++)
            {
                var step = steps[i];
                bool isLast = i == steps.Count - 1;

                Color circleColor;
                Label iconLabel;

                if (step.IsDone)
                {
                    circleColor = (Color)Application.Current.Resources["FloraBlush"];
                    iconLabel = new Label { Text = "\uE73E", FontFamily = "Segoe Fluent Icons", FontSize = 14, TextColor = Colors.White, HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center };
                }
                else if (step.IsActive)
                {
                    circleColor = (Color)Application.Current.Resources["FloraBlush"];
                    iconLabel = new Label { Text = "\uE7B8", FontFamily = "Segoe Fluent Icons", FontSize = 14, TextColor = Colors.White, HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center };
                }
                else
                {
                    circleColor = (Color)Application.Current.Resources["FloraFieldBg"];
                    string icon = step.Name == "Out for Delivery" ? "\uE7C3" : (step.Name == "Delivered" ? "\uE73E" : "\uE7B8");
                    iconLabel = new Label { Text = icon, FontFamily = "Segoe Fluent Icons", FontSize = 14, TextColor = (Color)Application.Current.Resources["FloraMuted"], HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center };
                }

                var circle = new Border { HeightRequest = 36, WidthRequest = 36, StrokeShape = new RoundRectangle { CornerRadius = 18 }, StrokeThickness = 0, BackgroundColor = circleColor, Content = iconLabel };

                var connector = new BoxView
                {
                    WidthRequest = 2,
                    HeightRequest = 30,
                    Color = step.IsDone ? (Color)Application.Current.Resources["FloraBlush"] : (Color)Application.Current.Resources["FloraFieldBg"],
                    HorizontalOptions = LayoutOptions.Center
                };

                var leftColumn = new VerticalStackLayout { HorizontalOptions = LayoutOptions.Center, Spacing = 0, Children = { circle } };
                if (!isLast) leftColumn.Children.Add(connector);

                var nameLabel = new Label
                {
                    Text = step.Name,
                    FontAttributes = step.IsActive ? FontAttributes.Bold : FontAttributes.None,
                    FontSize = 14,
                    TextColor = step.IsActive ? (Color)Application.Current.Resources["FloraBlush"] : (step.IsDone ? (Color)Application.Current.Resources["FloraCharcoal"] : (Color)Application.Current.Resources["FloraMuted"])
                };

                var textStack = new VerticalStackLayout { Spacing = 2, Children = { nameLabel } };
                if (step.IsActive)
                    textStack.Children.Add(new Label { Text = "Currently in progress...", FontSize = 11, TextColor = (Color)Application.Current.Resources["FloraBlush"] });

                var timeLabel = new Label { Text = step.TimeLabel, FontSize = 12, TextColor = (Color)Application.Current.Resources["FloraMuted"], HorizontalOptions = LayoutOptions.End };

                var row = new Grid { ColumnDefinitions = new ColumnDefinitionCollection { new ColumnDefinition(GridLength.Auto), new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto) }, ColumnSpacing = 14 };
                row.Children.Add(leftColumn); Grid.SetColumn(leftColumn, 0);
                row.Children.Add(textStack); Grid.SetColumn(textStack, 1);
                row.Children.Add(timeLabel); Grid.SetColumn(timeLabel, 2);

                ProgressStack.Children.Add(row);
            }
        }

        private void BuildOrderDetails()
        {
            OrderDetailsStack.Children.Clear();

            var order = _order ?? (OrderHistoryManager.Orders.Count > 0 ? OrderHistoryManager.Orders[0] : null);
            if (order == null) return;

            var items = order.Items;
            var displayItems = (items != null && items.Count > 0)
                ? items
                : new List<OrderLineItem> { new OrderLineItem { ImageSource = order.ImageSource, Name = order.Name, Price = order.Price, Quantity = 1 } };

            foreach (var lineItem in displayItems)
            {
                var image = new Image { Source = lineItem.ImageSource, Aspect = Aspect.AspectFill, HeightRequest = 56, WidthRequest = 56 };
                var imageWrapper = new Border { StrokeShape = new RoundRectangle { CornerRadius = 12 }, StrokeThickness = 0, Content = image, HeightRequest = 56, WidthRequest = 56 };

                var nameLabel = new Label { Text = lineItem.Name, FontAttributes = FontAttributes.Bold, FontSize = 14, TextColor = (Color)Application.Current.Resources["FloraCharcoal"] };
                var qtyLabel = new Label { Text = "Qty: " + lineItem.Quantity, FontSize = 12, TextColor = (Color)Application.Current.Resources["FloraMuted"] };

                var textStack = new VerticalStackLayout { Spacing = 4, Children = { nameLabel, qtyLabel } };
                var priceLabel = new Label { Text = "\u20B1" + (lineItem.Price * lineItem.Quantity).ToString("N0"), FontAttributes = FontAttributes.Bold, FontSize = 15, TextColor = (Color)Application.Current.Resources["FloraCharcoal"], VerticalOptions = LayoutOptions.Start };

                var row = new Grid { ColumnDefinitions = new ColumnDefinitionCollection { new ColumnDefinition(GridLength.Auto), new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto) }, ColumnSpacing = 12 };
                row.Children.Add(imageWrapper); Grid.SetColumn(imageWrapper, 0);
                row.Children.Add(textStack); Grid.SetColumn(textStack, 1);
                row.Children.Add(priceLabel); Grid.SetColumn(priceLabel, 2);

                OrderDetailsStack.Children.Add(row);
            }
        }

        private async void OnMessageShopTapped(object sender, EventArgs e)
        {
            var order = _order ?? (OrderHistoryManager.Orders.Count > 0 ? OrderHistoryManager.Orders[0] : null);
            int orderId = order?.Id ?? 0;
            await Navigation.PushAsync(new ChatPage(await MessageManager.GetOrCreateOrderConversationAsync(orderId, ShopName)));
        }

        private async void OnCallShopTapped(object sender, EventArgs e)
        {
            await DisplayAlert("Call Shop", $"{ShopName} - {ShopPhoneNumber}", "OK");
        }

        private async void OnCancelOrderTapped(object sender, EventArgs e)
        {
            if (_order == null) return;

            bool confirm = await DisplayAlert("Cancel Order", "Are you sure you want to cancel this order? This cannot be undone.", "Yes, Cancel", "No");
            if (!confirm) return;

            OrderHistoryManager.CancelOrder(_order);
            UpdateStatusUI();

            await DisplayAlert("Order Cancelled", "Your order has been cancelled.", "OK");
            await Navigation.PopAsync();
        }

        private async void OnBackTapped(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}