using System;
using FloraMix.Models;
using FloraMix.Services;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Graphics;

namespace FloraMix.Views.Main
{
    public partial class WishlistPage : ContentPage
    {
        public WishlistPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            BuildWishlist();
        }

        private void BuildWishlist()
        {
            WishlistGrid.Children.Clear();
            WishlistGrid.RowDefinitions.Clear();

            var items = CartManager.Wishlist;
            ItemCountLabel.Text = $"{items.Count} items";

            for (int i = 0; i < items.Count; i++)
            {
                if (i % 2 == 0)
                    WishlistGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));

                var item = items[i];

                var image = new Image { Source = item.ImageSource, Aspect = Aspect.AspectFill, HeightRequest = 130 };
                var imageBorder = new Border { StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(16, 16, 0, 0) }, StrokeThickness = 0, Content = image, HeightRequest = 130 };

                var heart = new Border
                {
                    HeightRequest = 28,
                    WidthRequest = 28,
                    StrokeShape = new RoundRectangle { CornerRadius = 14 },
                    StrokeThickness = 0,
                    BackgroundColor = Color.FromArgb("#CCFFFFFF"),
                    HorizontalOptions = LayoutOptions.End,
                    VerticalOptions = LayoutOptions.Start,
                    Margin = new Thickness(0, 10, 10, 0),
                    Content = new Label { Text = "\u2665", FontSize = 13, TextColor = (Color)Application.Current.Resources["FloraBlush"], HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center }
                };
                var capturedItem = item;
                heart.GestureRecognizers.Add(new TapGestureRecognizer { Command = new Command(() => RemoveFromWishlist(capturedItem)) });

                var tagBadge = new Border
                {
                    BackgroundColor = Color.FromArgb("#CC000000"),
                    StrokeShape = new RoundRectangle { CornerRadius = 8 },
                    StrokeThickness = 0,
                    Padding = new Thickness(8, 3),
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.End,
                    Margin = new Thickness(10, 0, 0, 10),
                    Content = new Label { Text = item.Tag, FontSize = 10, TextColor = Colors.White }
                };

                var imageStack = new Grid { Children = { imageBorder, heart, tagBadge } };

                var shopLabel = new Label { Text = item.Shop, FontSize = 11, TextColor = (Color)Application.Current.Resources["FloraBlush"] };
                var nameLabel = new Label { Text = item.Name, FontAttributes = FontAttributes.Bold, FontSize = 14, TextColor = (Color)Application.Current.Resources["FloraCharcoal"], LineBreakMode = LineBreakMode.TailTruncation };

                var priceLabel = new Label { Text = "\u20B1" + item.Price.ToString("N0"), FontAttributes = FontAttributes.Bold, FontSize = 14, TextColor = (Color)Application.Current.Resources["FloraCharcoal"], VerticalOptions = LayoutOptions.Center };
                var addToCartBtn = new Border
                {
                    BackgroundColor = (Color)Application.Current.Resources["FloraBlush"],
                    StrokeShape = new RoundRectangle { CornerRadius = 14 },
                    StrokeThickness = 0,
                    Padding = new Thickness(12, 6),
                    Content = new Label { Text = "Add to Cart", FontSize = 11, FontAttributes = FontAttributes.Bold, TextColor = Colors.White }
                };
                addToCartBtn.GestureRecognizers.Add(new TapGestureRecognizer { Command = new Command(() => AddToCart(capturedItem)) });

                var priceRow = new Grid
                {
                    ColumnDefinitions = new ColumnDefinitionCollection { new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto) },
                    Margin = new Thickness(0, 4, 0, 0)
                };
                priceRow.Children.Add(priceLabel);
                Grid.SetColumn(priceLabel, 0);
                priceRow.Children.Add(addToCartBtn);
                Grid.SetColumn(addToCartBtn, 1);

                var infoStack = new VerticalStackLayout { Spacing = 3, Padding = new Thickness(10, 10, 10, 12), Children = { shopLabel, nameLabel, priceRow } };

                var cardStack = new VerticalStackLayout { Spacing = 0, Children = { imageStack, infoStack } };

                var card = new Border
                {
                    BackgroundColor = Colors.White,
                    StrokeShape = new RoundRectangle { CornerRadius = 16 },
                    StrokeThickness = 0,
                    Content = cardStack
                };

                int row = i / 2;
                int col = i % 2;
                WishlistGrid.Children.Add(card);
                Grid.SetRow(card, row);
                Grid.SetColumn(card, col);
            }
        }

        private void AddToCart(WishlistItem item)
        {
            CartManager.Items.Add(new CartItem
            {
                ImageSource = item.ImageSource,
                Name = item.Name,
                Shop = item.Shop,
                Price = item.Price,
                Quantity = 1,
                ColorTag = "Mixed"
            });
            DisplayAlert("Added to Cart", $"{item.Name} was added to your cart.", "OK");
        }

        private void RemoveFromWishlist(WishlistItem item)
        {
            CartManager.Wishlist.Remove(item);
            BuildWishlist();
        }

        private async void OnBackTapped(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}