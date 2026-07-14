using System;
using FloraMix.Models;
using FloraMix.Services;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Graphics;

namespace FloraMix.Views.Main
{
    public partial class CartPage : ContentPage
    {
        public CartPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            CheckoutStepHelper.BuildStepIndicator(StepIndicatorGrid, 1);
            BuildCartList();
            UpdateTotals();
        }

        private void BuildCartList()
        {
            CartItemsStack.Children.Clear();
            ItemCountLabel.Text = CartManager.Items.Count + " items";

            foreach (var item in CartManager.Items)
                CartItemsStack.Children.Add(CreateCartItemCard(item));
        }

        private Border CreateCartItemCard(CartItem item)
        {
            var image = new Image { Source = item.ImageSource, Aspect = Aspect.AspectFill, HeightRequest = 64, WidthRequest = 64 };
            var imageWrapper = new Border { StrokeShape = new RoundRectangle { CornerRadius = 12 }, StrokeThickness = 0, Content = image, HeightRequest = 64, WidthRequest = 64 };

            var nameLabel = new Label { Text = item.Name, FontAttributes = FontAttributes.Bold, FontSize = 15, TextColor = (Color)Application.Current.Resources["FloraCharcoal"] };
            var shopLabel = new Label { Text = item.Shop, FontSize = 12, TextColor = (Color)Application.Current.Resources["FloraBlush"] };

            var colorDot = new Border { HeightRequest = 6, WidthRequest = 6, StrokeShape = new RoundRectangle { CornerRadius = 3 }, StrokeThickness = 0, BackgroundColor = (Color)Application.Current.Resources["FloraBlush"] };
            var colorLabel = new Label { Text = item.ColorTag, FontSize = 11, TextColor = (Color)Application.Current.Resources["FloraMuted"] };
            var colorRow = new HorizontalStackLayout { Spacing = 6, Children = { colorDot, colorLabel }, VerticalOptions = LayoutOptions.Center };

            var minusLabel = new Label { Text = "-", FontAttributes = FontAttributes.Bold, FontSize = 15, TextColor = (Color)Application.Current.Resources["FloraCharcoal"], HorizontalTextAlignment = TextAlignment.Center, WidthRequest = 26 };
            var quantityLabel = new Label { Text = item.Quantity.ToString(), FontAttributes = FontAttributes.Bold, FontSize = 14, TextColor = (Color)Application.Current.Resources["FloraCharcoal"], HorizontalTextAlignment = TextAlignment.Center, WidthRequest = 20 };
            var plusLabel = new Label { Text = "+", FontAttributes = FontAttributes.Bold, FontSize = 15, TextColor = Colors.White, HorizontalTextAlignment = TextAlignment.Center, WidthRequest = 26 };

            var minusBorder = new Border { BackgroundColor = (Color)Application.Current.Resources["FloraFieldBg"], StrokeShape = new RoundRectangle { CornerRadius = 13 }, HeightRequest = 26, WidthRequest = 26, Content = minusLabel };
            var plusBorder = new Border { BackgroundColor = (Color)Application.Current.Resources["FloraBlush"], StrokeShape = new RoundRectangle { CornerRadius = 13 }, HeightRequest = 26, WidthRequest = 26, Content = plusLabel };

            var minusTap = new TapGestureRecognizer();
            minusTap.Tapped += (s, e) => { if (item.Quantity > 1) item.Quantity--; quantityLabel.Text = item.Quantity.ToString(); CartManager.SaveItem(item); UpdateTotals(); };
            minusBorder.GestureRecognizers.Add(minusTap);

            var plusTap = new TapGestureRecognizer();
            plusTap.Tapped += (s, e) => { item.Quantity++; quantityLabel.Text = item.Quantity.ToString(); CartManager.SaveItem(item); UpdateTotals(); };
            plusBorder.GestureRecognizers.Add(plusTap);

            var stepperRow = new HorizontalStackLayout { Spacing = 10, Children = { minusBorder, quantityLabel, plusBorder } };

            var deleteLabel = new Label { Text = "\uE74D", FontFamily = "Segoe Fluent Icons", FontSize = 16, TextColor = (Color)Application.Current.Resources["FloraMuted"] };
            var deleteTap = new TapGestureRecognizer();
            deleteTap.Tapped += (s, e) => { CartManager.RemoveItem(item); BuildCartList(); UpdateTotals(); };
            deleteLabel.GestureRecognizers.Add(deleteTap);

            var priceLabel = new Label { Text = "\u20B1" + item.Price.ToString("N0"), FontAttributes = FontAttributes.Bold, FontSize = 16, TextColor = (Color)Application.Current.Resources["FloraCharcoal"] };

            var textStack = new VerticalStackLayout { Spacing = 4, Children = { nameLabel, shopLabel, colorRow, stepperRow } };
            var rightStack = new VerticalStackLayout { Spacing = 8, HorizontalOptions = LayoutOptions.End, Children = { deleteLabel, priceLabel } };

            var mainGrid = new Grid { ColumnDefinitions = new ColumnDefinitionCollection { new ColumnDefinition(GridLength.Auto), new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto) }, ColumnSpacing = 12 };
            mainGrid.Children.Add(imageWrapper); Grid.SetColumn(imageWrapper, 0);
            mainGrid.Children.Add(textStack); Grid.SetColumn(textStack, 1);
            mainGrid.Children.Add(rightStack); Grid.SetColumn(rightStack, 2);

            return new Border { BackgroundColor = Colors.White, StrokeShape = new RoundRectangle { CornerRadius = 16 }, StrokeThickness = 0, Padding = 14, Content = mainGrid };
        }

        private void UpdateTotals()
        {
            SubtotalTitleLabel.Text = "Subtotal (" + CartManager.Items.Count + " items)";
            SubtotalLabel.Text = "\u20B1" + CartManager.GetSubtotal().ToString("N0");
            DeliveryLabel.Text = "\u20B1" + CartManager.DeliveryFee.ToString("N0");

            if (CartManager.DiscountPercent > 0)
            {
                DiscountRow.IsVisible = true;
                DiscountLabel.Text = "-\u20B1" + CartManager.GetDiscount().ToString("N0");
            }
            else
            {
                DiscountRow.IsVisible = false;
            }

            TotalLabel.Text = "\u20B1" + CartManager.GetTotal().ToString("N0");
            ItemCountLabel.Text = CartManager.Items.Count + " items";
        }

        private async void OnApplyPromoTapped(object sender, EventArgs e)
        {
            string code = PromoEntry.Text != null ? PromoEntry.Text.Trim().ToUpper() : "";
            if (code == "SEBPOGI")
            {
                CartManager.DiscountPercent = 1.0;
                UpdateTotals();
                await DisplayAlert("Promo Applied", "100% discount applied! \uD83C\uDF89", "OK");
            }
            else if (code == "FLORA10")
            {
                CartManager.DiscountPercent = 0.10;
                UpdateTotals();
                await DisplayAlert("Promo Applied", "10% discount applied.", "OK");
            }
            else
            {
                await DisplayAlert("Invalid Code", "That promo code is not valid.", "OK");
            }
        }

        private async void OnContinueTapped(object sender, EventArgs e)
        {
            if (CartManager.Items.Count == 0)
            {
                await DisplayAlert("Cart Empty", "Add items before continuing.", "OK");
                return;
            }
            await Navigation.PushAsync(new DeliveryPage());
        }

        private async void OnBackTapped(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}