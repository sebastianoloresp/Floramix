using System;
using System.Linq;
using FloraMix.Models;
using FloraMix.Services;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Graphics;

namespace FloraMix.Views.Main
{
    public partial class ProductDetailPage : ContentPage
    {
        private readonly Product _product;
        private int _quantity = 1;

        public ProductDetailPage(Product product)
        {
            InitializeComponent();
            _product = product;
            PopulateUI();
        }

        private void PopulateUI()
        {
            ImageCarousel.ItemsSource = _product.GalleryImages;
            UpdateImageNavButtons();
            StockLabel.Text = _product.StockCount.ToString() + " left in stock";
            ProductNameLabel.Text = _product.Name;
            RatingLabel.Text = "Rating " + _product.Rating.ToString();
            ReviewCountLabel.Text = _product.ReviewCount.ToString() + " reviews";
            PriceLabel.Text = "\u20B1" + _product.Price.ToString("N0");
            DescriptionLabel.Text = _product.Description;
            SeeAllReviewsLabel.Text = "See all " + _product.ReviewCount.ToString();

            UpdateHeartIcon();

            ColorPaletteStack.Children.Clear();
            foreach (var hex in _product.ColorPalette)
            {
                var swatch = new Border();
                swatch.HeightRequest = 32;
                swatch.WidthRequest = 32;
                swatch.StrokeShape = new RoundRectangle { CornerRadius = 16 };
                swatch.Stroke = Colors.LightGray;
                swatch.StrokeThickness = hex == "#FFFFFF" ? 1 : 0;
                swatch.BackgroundColor = Color.FromArgb(hex);
                ColorPaletteStack.Children.Add(swatch);
            }

            IncludedStack.Children.Clear();
            foreach (var item in _product.WhatsIncluded)
            {
                var dot = new Border();
                dot.HeightRequest = 6;
                dot.WidthRequest = 6;
                dot.StrokeShape = new RoundRectangle { CornerRadius = 3 };
                dot.BackgroundColor = (Color)Application.Current.Resources["FloraBlush"];
                dot.VerticalOptions = LayoutOptions.Center;

                var label = new Label();
                label.Text = item;
                label.FontSize = 14;
                label.TextColor = (Color)Application.Current.Resources["FloraCharcoal"];

                var row = new HorizontalStackLayout();
                row.Spacing = 8;
                row.Children.Add(dot);
                row.Children.Add(label);

                IncludedStack.Children.Add(row);
            }

            ReviewsStack.Children.Clear();
            foreach (var review in _product.Reviews)
            {
                var avatarBorder = new Border
                {
                    BackgroundColor = (Color)Application.Current.Resources["FloraBlush"],
                    HeightRequest = 32,
                    WidthRequest = 32,
                    StrokeShape = new RoundRectangle { CornerRadius = 16 },
                    StrokeThickness = 0,
                    Content = new Label
                    {
                        Text = review.ReviewerName.Substring(0, 1),
                        TextColor = Colors.White,
                        FontAttributes = FontAttributes.Bold,
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center
                    }
                };

                var nameLabel = new Label
                {
                    Text = review.ReviewerName,
                    FontAttributes = FontAttributes.Bold,
                    FontSize = 14,
                    TextColor = (Color)Application.Current.Resources["FloraCharcoal"],
                    Margin = new Thickness(10, 0, 0, 0),
                    VerticalOptions = LayoutOptions.Center
                };

                var starsLabel = new Label
                {
                    Text = review.Stars + " stars",
                    FontSize = 12,
                    VerticalOptions = LayoutOptions.Center
                };

                var headerGrid = new Grid
                {
                    ColumnDefinitions = new ColumnDefinitionCollection
                    {
                        new ColumnDefinition(GridLength.Auto),
                        new ColumnDefinition(GridLength.Star),
                        new ColumnDefinition(GridLength.Auto)
                    }
                };
                headerGrid.Children.Add(avatarBorder); Grid.SetColumn(avatarBorder, 0);
                headerGrid.Children.Add(nameLabel); Grid.SetColumn(nameLabel, 1);
                headerGrid.Children.Add(starsLabel); Grid.SetColumn(starsLabel, 2);

                var commentLabel = new Label
                {
                    Text = review.Comment,
                    FontSize = 13,
                    TextColor = (Color)Application.Current.Resources["FloraMuted"],
                    LineHeight = 1.3
                };

                var dateLabel = new Label
                {
                    Text = review.Date,
                    FontSize = 11,
                    TextColor = (Color)Application.Current.Resources["FloraMuted"]
                };

                var contentStack = new VerticalStackLayout
                {
                    Spacing = 8,
                    Children = { headerGrid, commentLabel, dateLabel }
                };

                var card = new Border
                {
                    BackgroundColor = Colors.White,
                    StrokeShape = new RoundRectangle { CornerRadius = 16 },
                    StrokeThickness = 0,
                    Padding = 16,
                    Content = contentStack
                };

                ReviewsStack.Children.Add(card);
            }
        }

        private void UpdateHeartIcon()
        {
            HeartBorder.BackgroundColor = _product.IsWishlisted
                ? (Color)Application.Current.Resources["FloraBlush"]
                : Colors.White;
            HeartIconLabel.TextColor = _product.IsWishlisted
                ? Colors.White
                : (Color)Application.Current.Resources["FloraCharcoal"];
        }

        private async void OnBackTapped(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private void OnDecreaseQuantity(object sender, EventArgs e)
        {
            if (_quantity > 1)
            {
                _quantity = _quantity - 1;
            }
            QuantityLabel.Text = _quantity.ToString();
        }

        private void OnNextImageTapped(object sender, EventArgs e)
        {
            var images = _product.GalleryImages;
            if (images.Count <= 1) return;

            if (ImageCarousel.Position < images.Count - 1)
                ImageCarousel.Position++;

            UpdateImageNavButtons();
        }

        private void OnPrevImageTapped(object sender, EventArgs e)
        {
            if (ImageCarousel.Position > 0)
                ImageCarousel.Position--;

            UpdateImageNavButtons();
        }

        private void UpdateImageNavButtons()
        {
            var images = _product.GalleryImages;
            PrevImageButton.IsVisible = ImageCarousel.Position > 0;
            NextImageButton.IsVisible = images.Count > 1 && ImageCarousel.Position < images.Count - 1;
        }

        private void OnIncreaseQuantity(object sender, EventArgs e)
        {
            _quantity = _quantity + 1;
            QuantityLabel.Text = _quantity.ToString();
        }

        private void OnHeartTapped(object sender, EventArgs e)
        {
            ProductManager.ToggleWishlist(_product);
            UpdateHeartIcon();
        }

        private async void OnCustomizeTapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new BouquetBuilderPage(GetPrimaryFlowerName(_product.Name)));
        }

        private string GetPrimaryFlowerName(string productName)
        {
            string name = productName.ToLower();
            if (name.Contains("rose")) return "Rose";
            if (name.Contains("tulip")) return "Tulip";
            if (name.Contains("sunflower")) return "Sunflower";
            if (name.Contains("gerbera")) return "Gerbera";
            if (name.Contains("lily")) return "Lily";
            if (name.Contains("carnation")) return "Carnation";
            return null;
        }

        private async void OnAddToCartTapped(object sender, EventArgs e)
        {
            ProductManager.AddToCart(_product, _quantity);
            string message = _quantity.ToString() + " x " + _product.Name + " added to your cart.";
            await DisplayAlert("Added to Cart", message, "OK");
        }
    }
}