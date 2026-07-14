using System;
using FloraMix.Models;
using FloraMix.Services;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Graphics;

namespace FloraMix.Views.Main
{
    public partial class SavedAddressesPage : ContentPage
    {
        public SavedAddressesPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            BuildAddresses();
        }

        private void BuildAddresses()
        {
            AddressesStack.Children.Clear();

            foreach (var address in CartManager.Addresses)
            {
                var labelRow = new HorizontalStackLayout { Spacing = 8 };
                labelRow.Children.Add(new Label { Text = address.Label, FontAttributes = FontAttributes.Bold, FontSize = 15, TextColor = (Color)Application.Current.Resources["FloraCharcoal"] });

                if (address.IsDefault)
                {
                    labelRow.Children.Add(new Border
                    {
                        BackgroundColor = Color.FromArgb("#FDEEF1"),
                        StrokeShape = new RoundRectangle { CornerRadius = 8 },
                        StrokeThickness = 0,
                        Padding = new Thickness(8, 2),
                        Content = new Label { Text = "Default", FontSize = 10, FontAttributes = FontAttributes.Bold, TextColor = (Color)Application.Current.Resources["FloraBlush"] }
                    });
                }

                var addressLineLabel = new Label { Text = address.AddressLine + (string.IsNullOrWhiteSpace(address.ApartmentLine) ? "" : $", {address.ApartmentLine}"), FontSize = 13, TextColor = (Color)Application.Current.Resources["FloraMuted"] };
                var cityLineLabel = new Label { Text = address.CityLine, FontSize = 13, TextColor = (Color)Application.Current.Resources["FloraMuted"] };

                var textStack = new VerticalStackLayout { Spacing = 4, Children = { labelRow, addressLineLabel, cityLineLabel } };

                var capturedAddress = address;

                var editLabel = new Label { Text = "Edit", FontAttributes = FontAttributes.Bold, FontSize = 13, TextColor = (Color)Application.Current.Resources["FloraBlush"] };
                editLabel.GestureRecognizers.Add(new TapGestureRecognizer { Command = new Command(() => EditAddress(capturedAddress)) });

                var actionsRow = new HorizontalStackLayout { Spacing = 16, HorizontalOptions = LayoutOptions.Start };
                actionsRow.Children.Add(editLabel);

                if (!address.IsDefault)
                {
                    var setDefaultLabel = new Label { Text = "Set Default", FontAttributes = FontAttributes.Bold, FontSize = 13, TextColor = Color.FromArgb("#7A9A72") };
                    setDefaultLabel.GestureRecognizers.Add(new TapGestureRecognizer { Command = new Command(() => SetDefault(capturedAddress)) });
                    actionsRow.Children.Add(setDefaultLabel);
                }

                var removeLabel = new Label { Text = "Remove", FontAttributes = FontAttributes.Bold, FontSize = 13, TextColor = Color.FromArgb("#D9534F") };
                removeLabel.GestureRecognizers.Add(new TapGestureRecognizer { Command = new Command(() => RemoveAddress(capturedAddress)) });
                actionsRow.Children.Add(removeLabel);

                var topRow = new Grid
                {
                    ColumnDefinitions = new ColumnDefinitionCollection { new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto) }
                };
                topRow.Children.Add(textStack);
                Grid.SetColumn(textStack, 0);

                var iconGlyph = address.AddressType switch
                {
                    "Work" => "\uE821",
                    "Other" => "\uE81D",
                    _ => "\uE80F"
                };
                var iconBadge = new Border
                {
                    HeightRequest = 36,
                    WidthRequest = 36,
                    StrokeShape = new RoundRectangle { CornerRadius = 18 },
                    StrokeThickness = 0,
                    BackgroundColor = (Color)Application.Current.Resources["FloraFieldBg"],
                    Content = new Label { Text = iconGlyph, FontFamily = "Segoe Fluent Icons", FontSize = 14, TextColor = (Color)Application.Current.Resources["FloraCharcoal"], HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center }
                };
                topRow.Children.Add(iconBadge);
                Grid.SetColumn(iconBadge, 1);

                var cardContent = new VerticalStackLayout { Spacing = 12, Children = { topRow, actionsRow } };

                var card = new Border
                {
                    BackgroundColor = Colors.White,
                    StrokeShape = new RoundRectangle { CornerRadius = 16 },
                    StrokeThickness = 0,
                    Padding = 16,
                    Content = cardContent
                };

                AddressesStack.Children.Add(card);
            }
        }

        private void SetDefault(SavedAddress address)
        {
            foreach (var a in CartManager.Addresses)
            {
                bool shouldBeDefault = a == address;
                if (a.IsDefault != shouldBeDefault)
                {
                    a.IsDefault = shouldBeDefault;
                    CartManager.UpdateAddressAndSave(a);
                }
            }
            BuildAddresses();
        }

        private async void RemoveAddress(SavedAddress address)
        {
            bool confirm = await DisplayAlert("Remove Address", $"Remove \"{address.Label}\"?", "Remove", "Cancel");
            if (confirm)
            {
                CartManager.RemoveAddressAndDelete(address);

                if (address.IsDefault && CartManager.Addresses.Count > 0)
                {
                    CartManager.Addresses[0].IsDefault = true;
                    CartManager.UpdateAddressAndSave(CartManager.Addresses[0]);
                }

                BuildAddresses();
            }
        }

        private async void EditAddress(SavedAddress address)
        {
            await Navigation.PushAsync(new AddAddressPage(address));
        }

        private async void OnAddAddressTapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AddAddressPage());
        }

        private async void OnBackTapped(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}