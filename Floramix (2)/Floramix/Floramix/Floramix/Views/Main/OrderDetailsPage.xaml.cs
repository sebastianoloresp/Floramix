using System;
using System.Collections.Generic;
using FloraMix.Models;
using FloraMix.Services;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Graphics;

namespace FloraMix.Views.Main
{
    public partial class OrderDetailsPage : ContentPage
    {
        private const string ShopPhoneNumber = "09054269748";

        private readonly OrderHistoryItem _order;
        private readonly Label[] _starLabels = new Label[5];
        private Label _rateHintLabel;

        public OrderDetailsPage(OrderHistoryItem order)
        {
            InitializeComponent();
            _order = order;
            BuildOrderDetails();
        }

        private void BuildOrderDetails()
        {
            OrderDetailsStack.Children.Clear();

            var items = _order.Items;
            var displayItems = (items != null && items.Count > 0)
                ? items
                : new List<OrderLineItem> { new OrderLineItem { ImageSource = _order.ImageSource, Name = _order.Name, Price = _order.Price, Quantity = 1 } };

            foreach (var lineItem in displayItems)
            {
                var itemImage = new Image { Source = lineItem.ImageSource, Aspect = Aspect.AspectFill, HeightRequest = 56, WidthRequest = 56 };
                var itemImageWrapper = new Border { StrokeShape = new RoundRectangle { CornerRadius = 12 }, StrokeThickness = 0, Content = itemImage, HeightRequest = 56, WidthRequest = 56 };

                var itemNameLabel = new Label { Text = lineItem.Name, FontAttributes = FontAttributes.Bold, FontSize = 14, TextColor = (Color)Application.Current.Resources["FloraCharcoal"] };
                var itemQtyLabel = new Label { Text = "Qty: " + lineItem.Quantity, FontSize = 12, TextColor = (Color)Application.Current.Resources["FloraMuted"] };
                var itemTextStack = new VerticalStackLayout { Spacing = 4, Children = { itemNameLabel, itemQtyLabel } };

                var itemPriceLabel = new Label { Text = "\u20B1" + (lineItem.Price * lineItem.Quantity).ToString("N0"), FontAttributes = FontAttributes.Bold, FontSize = 14, TextColor = (Color)Application.Current.Resources["FloraCharcoal"], VerticalOptions = LayoutOptions.Center };

                var itemRow = new Grid { ColumnDefinitions = new ColumnDefinitionCollection { new ColumnDefinition(GridLength.Auto), new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto) }, ColumnSpacing = 12 };
                itemRow.Children.Add(itemImageWrapper); Grid.SetColumn(itemImageWrapper, 0);
                itemRow.Children.Add(itemTextStack); Grid.SetColumn(itemTextStack, 1);
                itemRow.Children.Add(itemPriceLabel); Grid.SetColumn(itemPriceLabel, 2);

                OrderDetailsStack.Children.Add(itemRow);
            }

            var shopLabel = new Label { Text = string.IsNullOrEmpty(_order.Shop) ? "Wild Flowers" : _order.Shop, FontSize = 12, TextColor = (Color)Application.Current.Resources["FloraMuted"] };

            var starsRow = new HorizontalStackLayout { Spacing = 2 };
            for (int i = 0; i < 5; i++)
            {
                int starValue = i + 1;
                var star = new Label
                {
                    Text = "\u2606",
                    FontSize = 16,
                    TextColor = Color.FromArgb("#E8B84B")
                };
                star.GestureRecognizers.Add(new TapGestureRecognizer { Command = new Command(() => SetRating(starValue)) });
                _starLabels[i] = star;
                starsRow.Children.Add(star);
            }

            _rateHintLabel = new Label
            {
                Text = _order.Rating > 0 ? "Your rating" : "Tap to rate",
                FontSize = 11,
                TextColor = (Color)Application.Current.Resources["FloraBlush"],
                VerticalOptions = LayoutOptions.Center
            };

            var ratingRow = new HorizontalStackLayout { Spacing = 6, Children = { starsRow, _rateHintLabel } };

            var totalLabel = new Label { Text = "Total: \u20B1" + _order.Price.ToString("N0"), FontAttributes = FontAttributes.Bold, FontSize = 15, TextColor = (Color)Application.Current.Resources["FloraCharcoal"] };

            var footerStack = new VerticalStackLayout { Spacing = 8, Children = { shopLabel, ratingRow, totalLabel } };
            OrderDetailsStack.Children.Add(footerStack);

            RefreshStars();
        }

        private void SetRating(int value)
        {
            _order.Rating = value;
            RefreshStars();
            _rateHintLabel.Text = "Your rating";
            OrderHistoryManager.SaveRatingChange(_order);
        }

        private void RefreshStars()
        {
            for (int i = 0; i < _starLabels.Length; i++)
            {
                _starLabels[i].Text = i < _order.Rating ? "\u2605" : "\u2606";
            }
        }

        private async void OnMessageShopTapped(object sender, EventArgs e)
        {
            string shopName = string.IsNullOrEmpty(_order.Shop) ? "Wild Flowers" : _order.Shop;
            await Navigation.PushAsync(new ChatPage(await MessageManager.GetOrCreateOrderConversationAsync(_order.Id, shopName, serverOrderId: _order.ServerOrderId)));
        }

        private async void OnCallShopTapped(object sender, EventArgs e)
        {
            string shopName = string.IsNullOrEmpty(_order.Shop) ? "Wild Flowers" : _order.Shop;
            await DisplayAlert("Call Shop", $"{shopName} - {ShopPhoneNumber}", "OK");
        }

        private async void OnBackTapped(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}