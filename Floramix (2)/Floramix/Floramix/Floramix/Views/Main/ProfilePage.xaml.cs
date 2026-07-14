using System;
using System.Collections.Generic;
using FloraMix.Models;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Graphics;

namespace FloraMix.Views.Main
{
    public partial class ProfilePage : ContentPage
    {
        public ProfilePage()
        {
            InitializeComponent();

            BuildOrderHistory();
            BuildAddresses();
            RefreshQuickLinkCounts();
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            ProfileNameLabel.Text = FloraMix.Services.ProfileManager.FullName;
            ProfileEmailLabel.Text = FloraMix.Services.ProfileManager.Email;
            ProfileLocationLabel.Text = FloraMix.Services.ProfileManager.Location;
            BuildOrderHistory();
            BuildAddresses();
            RefreshQuickLinkCounts();
        }

        private void RefreshQuickLinkCounts()
        {
            OrdersCountLabel.Text = FloraMix.Services.OrderHistoryManager.Orders.Count.ToString();
            ReviewsCountLabel.Text = FloraMix.Services.OrderHistoryManager.ReviewedCount.ToString();
            WishlistCountLabel.Text = FloraMix.Services.CartManager.Wishlist.Count.ToString();
            NotificationsCountLabel.Text = $"{FloraMix.Services.NotificationManager.Notifications.FindAll(n => n.IsUnread).Count} new";
            PaymentMethodsCountLabel.Text = $"{FloraMix.Services.CartManager.Cards.Count} cards";
            WishlistItemsCountLabel.Text = $"{FloraMix.Services.CartManager.Wishlist.Count} items";
        }

        private void BuildOrderHistory()
        {
            OrderHistoryStack.Children.Clear();

            foreach (var order in FloraMix.Services.OrderHistoryManager.Orders)
            {
                var image = new Image { Source = order.ImageSource, Aspect = Aspect.AspectFill, HeightRequest = 56, WidthRequest = 56 };
                var imageWrapper = new Border { StrokeShape = new RoundRectangle { CornerRadius = 12 }, StrokeThickness = 0, Content = image, HeightRequest = 56, WidthRequest = 56 };

                var nameLabel = new Label { Text = order.Name, FontAttributes = FontAttributes.Bold, FontSize = 15, TextColor = (Color)Application.Current.Resources["FloraCharcoal"] };
                var dateLabel = new Label { Text = order.Date, FontSize = 12, TextColor = (Color)Application.Current.Resources["FloraMuted"] };

                bool isDelivered = order.Status == "Delivered";
                bool isCancelled = order.Status == "Cancelled";

                Color statusColor = isDelivered ? Color.FromArgb("#A8C5A0")
                    : isCancelled ? Color.FromArgb("#D9534F")
                    : (Color)Application.Current.Resources["FloraBlush"];

                var statusDot = new Border { HeightRequest = 6, WidthRequest = 6, StrokeShape = new RoundRectangle { CornerRadius = 3 }, StrokeThickness = 0, BackgroundColor = statusColor };
                var statusLabel = new Label { Text = order.Status, FontSize = 12, TextColor = statusColor };
                var statusRow = new HorizontalStackLayout { Spacing = 6, Children = { statusDot, statusLabel }, VerticalOptions = LayoutOptions.Center };

                var leftTextStack = new VerticalStackLayout { Spacing = 4, Children = { nameLabel, dateLabel, statusRow } };

                var priceLabel = new Label { Text = "\u20B1" + order.Price.ToString("N0"), FontAttributes = FontAttributes.Bold, FontSize = 16, TextColor = (Color)Application.Current.Resources["FloraCharcoal"], HorizontalOptions = LayoutOptions.End };

                View bottomRightView;
                if (order.Status == "In Transit")
                {
                    bottomRightView = new Label { Text = "Track ->", FontSize = 12, FontAttributes = FontAttributes.Bold, TextColor = (Color)Application.Current.Resources["FloraBlush"], HorizontalOptions = LayoutOptions.End };
                }
                else if (isCancelled)
                {
                    bottomRightView = new Label { Text = "View details ->", FontSize = 11, TextColor = Color.FromArgb("#D9534F"), HorizontalOptions = LayoutOptions.End };
                }
                else
                {
                    string stars = order.Rating > 0
                        ? new string('\u2605', order.Rating) + new string('\u2606', 5 - order.Rating) + " Your rating"
                        : "Rate this order ->";
                    bottomRightView = new Label { Text = stars, FontSize = 11, TextColor = Color.FromArgb("#E8B84B"), HorizontalOptions = LayoutOptions.End };
                }

                var rightStackChildren = new List<View> { priceLabel, bottomRightView };

                if (isCancelled || isDelivered)
                {
                    var removeOrderLabel = new Label { Text = "Remove", FontSize = 11, FontAttributes = FontAttributes.Bold, TextColor = Color.FromArgb("#D9534F"), HorizontalOptions = LayoutOptions.End };
                    var capturedOrderForRemove = order;
                    removeOrderLabel.GestureRecognizers.Add(new TapGestureRecognizer { Command = new Command(() => RemoveOrderFromHistory(capturedOrderForRemove)) });
                    rightStackChildren.Add(removeOrderLabel);
                }

                var rightStack = new VerticalStackLayout { Spacing = 6, HorizontalOptions = LayoutOptions.End };
                foreach (var child in rightStackChildren)
                    rightStack.Children.Add(child);

                var mainGrid = new Grid
                {
                    ColumnDefinitions = new ColumnDefinitionCollection
                    {
                        new ColumnDefinition(GridLength.Auto),
                        new ColumnDefinition(GridLength.Star),
                        new ColumnDefinition(GridLength.Auto)
                    },
                    ColumnSpacing = 12
                };
                mainGrid.Children.Add(imageWrapper);
                Grid.SetColumn(imageWrapper, 0);
                mainGrid.Children.Add(leftTextStack);
                Grid.SetColumn(leftTextStack, 1);
                mainGrid.Children.Add(rightStack);
                Grid.SetColumn(rightStack, 2);

                var card = new Border
                {
                    BackgroundColor = Colors.White,
                    StrokeShape = new RoundRectangle { CornerRadius = 16 },
                    StrokeThickness = 0,
                    Padding = 14,
                    Content = mainGrid
                };

                var capturedOrder = order;
                if (order.Status == "In Transit")
                {
                    card.GestureRecognizers.Add(new TapGestureRecognizer { Command = new Command(() => OnTrackOrderTapped(capturedOrder)) });
                }
                else
                {
                    card.GestureRecognizers.Add(new TapGestureRecognizer { Command = new Command(() => OnDeliveredOrderTapped(capturedOrder)) });
                }

                OrderHistoryStack.Children.Add(card);
            }
        }

        private void BuildAddresses()
        {
            AddressesStack.Children.Clear();

            foreach (var address in FloraMix.Services.CartManager.Addresses)
            {
                var labelRow = new HorizontalStackLayout { Spacing = 8 };
                labelRow.Children.Add(new Label { Text = address.Label, FontAttributes = FontAttributes.Bold, FontSize = 15, TextColor = (Color)Application.Current.Resources["FloraCharcoal"] });

                if (address.IsDefault)
                {
                    var badge = new Border
                    {
                        BackgroundColor = Color.FromArgb("#FDEEF1"),
                        StrokeShape = new RoundRectangle { CornerRadius = 8 },
                        StrokeThickness = 0,
                        Padding = new Thickness(8, 2),
                        Content = new Label { Text = "Default", FontSize = 10, FontAttributes = FontAttributes.Bold, TextColor = (Color)Application.Current.Resources["FloraBlush"] }
                    };
                    labelRow.Children.Add(badge);
                }

                var addressLineLabel = new Label { Text = address.AddressLine, FontSize = 13, TextColor = (Color)Application.Current.Resources["FloraMuted"] };
                var cityLineLabel = new Label { Text = address.CityLine, FontSize = 13, TextColor = (Color)Application.Current.Resources["FloraMuted"] };

                var textStack = new VerticalStackLayout { Spacing = 4, Children = { labelRow, addressLineLabel, cityLineLabel } };

                var capturedAddress = address;

                var editLabel = new Label { Text = "Edit", FontAttributes = FontAttributes.Bold, FontSize = 13, TextColor = (Color)Application.Current.Resources["FloraBlush"] };
                editLabel.GestureRecognizers.Add(new TapGestureRecognizer { Command = new Command(() => EditAddress(capturedAddress)) });

                var removeLabel = new Label { Text = "Remove", FontAttributes = FontAttributes.Bold, FontSize = 13, TextColor = Color.FromArgb("#D9534F") };
                removeLabel.GestureRecognizers.Add(new TapGestureRecognizer { Command = new Command(() => RemoveAddress(capturedAddress)) });

                var actionsRow = new HorizontalStackLayout { Spacing = 16, HorizontalOptions = LayoutOptions.End, VerticalOptions = LayoutOptions.Start, Children = { editLabel, removeLabel } };

                var row = new Grid
                {
                    ColumnDefinitions = new ColumnDefinitionCollection { new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto) }
                };
                row.Children.Add(textStack);
                Grid.SetColumn(textStack, 0);
                row.Children.Add(actionsRow);
                Grid.SetColumn(actionsRow, 1);

                var card = new Border
                {
                    BackgroundColor = Colors.White,
                    StrokeShape = new RoundRectangle { CornerRadius = 16 },
                    StrokeThickness = 0,
                    Padding = 16,
                    Content = row
                };

                AddressesStack.Children.Add(card);
            }
        }

        private async void EditAddress(SavedAddress address)
        {
            await Navigation.PushAsync(new AddAddressPage(address));
        }

        private async void RemoveAddress(SavedAddress address)
        {
            bool confirm = await DisplayAlert("Remove Address", $"Remove \"{address.Label}\"?", "Remove", "Cancel");
            if (confirm)
            {
                FloraMix.Services.CartManager.RemoveAddressAndDelete(address);
                if (address.IsDefault && FloraMix.Services.CartManager.Addresses.Count > 0)
                {
                    FloraMix.Services.CartManager.Addresses[0].IsDefault = true;
                    FloraMix.Services.CartManager.UpdateAddressAndSave(FloraMix.Services.CartManager.Addresses[0]);
                }
                BuildAddresses();
            }
        }

        private async void OnTrackOrderTapped(OrderHistoryItem order)
        {
            await Navigation.PushAsync(new OrderTrackingPage(order));
        }

        private async void RemoveOrderFromHistory(OrderHistoryItem order)
        {
            bool confirm = await DisplayAlert("Remove Order", $"Remove \"{order.Name}\" from your order history?", "Remove", "Cancel");
            if (confirm)
            {
                FloraMix.Services.OrderHistoryManager.RemoveOrder(order);
                BuildOrderHistory();
                RefreshQuickLinkCounts();
            }
        }

        private async void OnDeliveredOrderTapped(OrderHistoryItem order)
        {
            await Navigation.PushAsync(new OrderDetailsPage(order));
        }

        private async void OnAddAddressTapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AddAddressPage());
        }

        private async void OnNotificationsTapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new NotificationsPage());
        }

        private async void OnPaymentMethodsTapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new PaymentMethodsPage());
        }

        private async void OnSavedAddressesTapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SavedAddressesPage());
        }

        private async void OnWishlistTapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new WishlistPage());
        }
        private async void OnSettingsTapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SettingsPage());
        }

        private async void OnHelpSupportTapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new HelpSupportPage());
        }

        private async void OnSignOutTapped(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlert("Sign Out", "Are you sure you want to sign out?", "Yes", "Cancel");
            if (confirm)
            {
                Application.Current.MainPage = new AppShell();
                await Shell.Current.GoToAsync("///onboarding");
            }
        }
    }
}