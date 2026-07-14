using System;
using FloraMix.Models;
using FloraMix.Services;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Graphics;

namespace FloraMix.Views.Main
{
    public partial class ReviewOrderPage : ContentPage
    {
        public ReviewOrderPage()
        {
            InitializeComponent();
            CheckoutStepHelper.BuildStepIndicator(StepIndicatorGrid, 4);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            BuildOrderItems();
            PopulateDeliveryDetails();
            PopulatePaymentDetails();
            PopulateTotals();
        }

        private void BuildOrderItems()
        {
            OrderItemsStack.Children.Clear();

            foreach (var item in CartManager.Items)
            {
                var image = new Image { Source = item.ImageSource, Aspect = Aspect.AspectFill, HeightRequest = 44, WidthRequest = 44 };
                var imageWrapper = new Border { StrokeShape = new RoundRectangle { CornerRadius = 10 }, StrokeThickness = 0, Content = image, HeightRequest = 44, WidthRequest = 44 };

                var nameLabel = new Label { Text = item.Name, FontSize = 14, TextColor = (Color)Application.Current.Resources["FloraCharcoal"], VerticalOptions = LayoutOptions.Center };
                var priceLabel = new Label { Text = "\u20B1" + (item.Price * item.Quantity).ToString("N0"), FontAttributes = FontAttributes.Bold, FontSize = 14, TextColor = (Color)Application.Current.Resources["FloraCharcoal"], VerticalOptions = LayoutOptions.Center };

                var row = new Grid { ColumnDefinitions = new ColumnDefinitionCollection { new ColumnDefinition(GridLength.Auto), new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto) }, ColumnSpacing = 12 };
                row.Children.Add(imageWrapper); Grid.SetColumn(imageWrapper, 0);
                row.Children.Add(nameLabel); Grid.SetColumn(nameLabel, 1);
                row.Children.Add(priceLabel); Grid.SetColumn(priceLabel, 2);

                OrderItemsStack.Children.Add(row);
            }
        }

        private void PopulateDeliveryDetails()
        {
            var address = CartManager.SelectedAddress;
            if (address != null)
            {
                DeliveryAddressLabelLabel.Text = address.Label;
                DeliveryAddressLineLabel.Text = address.AddressLine + ", " + address.CityLine;
            }

            var date = CartManager.SelectedDate;
            var fakeToday = new DateTime(2026, 6, 3);
            DeliveryDateLabel.Text = date.Date == fakeToday.Date ? "Today" : date.ToString("ddd, MMM d");
            DeliveryTimeLabel.Text = string.IsNullOrEmpty(CartManager.SelectedTimeSlot) ? "No time selected" : CartManager.SelectedTimeSlot;

            if (!string.IsNullOrWhiteSpace(CartManager.DeliveryNote))
            {
                DeliveryNoteRow.IsVisible = true;
                DeliveryNoteLabel.Text = CartManager.DeliveryNote;
            }
            else
            {
                DeliveryNoteRow.IsVisible = false;
            }
        }

        private void PopulatePaymentDetails()
        {
            if (CartManager.IsCashOnDeliverySelected)
            {
                PaymentCardLabel.Text = "Cash on Delivery";
                PaymentExpiryLabel.Text = "Pay when your order arrives";
                return;
            }

            var card = CartManager.GetSelectedCard();
            if (card != null)
            {
                PaymentCardLabel.Text = card.Brand + " \u2022\u2022\u2022\u2022 " + card.Last4;
                PaymentExpiryLabel.Text = "Expires " + card.ExpiryText;
            }
        }

        private void PopulateTotals()
        {
            ReviewSubtotalLabel.Text = "\u20B1" + CartManager.GetSubtotal().ToString("N0");
            ReviewDeliveryLabel.Text = "\u20B1" + CartManager.DeliveryFee.ToString("N0");

            if (CartManager.DiscountPercent > 0)
            {
                ReviewDiscountRow.IsVisible = true;
                ReviewDiscountLabel.Text = "-\u20B1" + CartManager.GetDiscount().ToString("N0");
            }
            else
            {
                ReviewDiscountRow.IsVisible = false;
            }

            double total = CartManager.GetTotal();
            ReviewTotalLabel.Text = "\u20B1" + total.ToString("N0");
            PlaceOrderLabel.Text = "Place Order - \u20B1" + total.ToString("N0");
        }

        private async void OnPlaceOrderTapped(object sender, EventArgs e)
        {
            await OrderHistoryManager.AddOrderFromCart();
            CartManager.ClearCart();
            await Navigation.PushAsync(new OrderConfirmationPage());
        }

        private async void OnBackTapped(object sender, TappedEventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}