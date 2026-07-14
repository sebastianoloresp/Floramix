using System;
using FloraMix.Models;
using FloraMix.Services;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Graphics;

namespace FloraMix.Views.Main
{
    public partial class PaymentPage : ContentPage
    {
        public PaymentPage()
        {
            InitializeComponent();
            CheckoutStepHelper.BuildStepIndicator(StepIndicatorGrid, 3);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            BuildCardsList();
            UpdateCardPreview();
        }

        private void BuildCardsList()
        {
            CardsStack.Children.Clear();

            foreach (var card in CartManager.Cards)
            {
                var brandDot = new Border { HeightRequest = 30, WidthRequest = 44, StrokeShape = new RoundRectangle { CornerRadius = 6 }, StrokeThickness = 0, BackgroundColor = card.Brand == "VISA" ? (Color)Application.Current.Resources["FloraBlush"] : Color.FromArgb("#5B8C5A") };
                var brandLabel = new Label { Text = card.Brand, FontAttributes = FontAttributes.Bold, FontSize = 15, TextColor = (Color)Application.Current.Resources["FloraCharcoal"] };
                var numberLabel = new Label { Text = "\u2022\u2022\u2022\u2022 " + card.Last4, FontSize = 13, TextColor = (Color)Application.Current.Resources["FloraMuted"] };
                var expiryLabel = new Label { Text = "Expires " + card.ExpiryText, FontSize = 11, TextColor = (Color)Application.Current.Resources["FloraMuted"] };

                var titleRow = new HorizontalStackLayout { Spacing = 8, Children = { brandLabel, numberLabel } };
                var textStack = new VerticalStackLayout { Spacing = 2, Children = { titleRow, expiryLabel } };

                bool isSelected = card.IsSelected && !CartManager.IsCashOnDeliverySelected;
                var radioOuter = new Border { HeightRequest = 20, WidthRequest = 20, StrokeShape = new RoundRectangle { CornerRadius = 10 }, Stroke = isSelected ? (Color)Application.Current.Resources["FloraBlush"] : (Color)Application.Current.Resources["FloraMuted"], StrokeThickness = 2, BackgroundColor = Colors.Transparent, VerticalOptions = LayoutOptions.Center };
                if (isSelected)
                    radioOuter.Content = new Border { HeightRequest = 10, WidthRequest = 10, StrokeShape = new RoundRectangle { CornerRadius = 5 }, StrokeThickness = 0, BackgroundColor = (Color)Application.Current.Resources["FloraBlush"], HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center };

                var row = new Grid { ColumnDefinitions = new ColumnDefinitionCollection { new ColumnDefinition(GridLength.Auto), new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto) }, ColumnSpacing = 12 };
                row.Children.Add(brandDot); Grid.SetColumn(brandDot, 0);
                row.Children.Add(textStack); Grid.SetColumn(textStack, 1);
                row.Children.Add(radioOuter); Grid.SetColumn(radioOuter, 2);

                var cardBorder = new Border
                {
                    BackgroundColor = isSelected ? Color.FromArgb("#FDEEF1") : Colors.White,
                    Stroke = isSelected ? (Color)Application.Current.Resources["FloraBlush"] : Colors.Transparent,
                    StrokeThickness = isSelected ? 1.5 : 0,
                    StrokeShape = new RoundRectangle { CornerRadius = 14 },
                    Padding = 14,
                    Content = row
                };

                var tap = new TapGestureRecognizer();
                tap.Tapped += (s, e) =>
                {
                    foreach (var c in CartManager.Cards) c.IsSelected = false;
                    card.IsSelected = true;
                    CartManager.IsCashOnDeliverySelected = false;
                    BuildCardsList();
                    UpdateCardPreview();
                };
                cardBorder.GestureRecognizers.Add(tap);

                CardsStack.Children.Add(cardBorder);
            }

            if (CartManager.CashOnDeliveryEnabled)
            {
                BuildCodRow();
            }
        }

        private void BuildCodRow()
        {
            bool isCodSelected = CartManager.IsCashOnDeliverySelected;

            var brandDot = new Border { HeightRequest = 30, WidthRequest = 44, StrokeShape = new RoundRectangle { CornerRadius = 6 }, StrokeThickness = 0, BackgroundColor = Color.FromArgb("#5B8C5A") };
            var titleLabel = new Label { Text = "Cash on Delivery", FontAttributes = FontAttributes.Bold, FontSize = 15, TextColor = (Color)Application.Current.Resources["FloraCharcoal"] };
            var subLabel = new Label { Text = "Pay when your order arrives", FontSize = 11, TextColor = (Color)Application.Current.Resources["FloraMuted"] };
            var textStack = new VerticalStackLayout { Spacing = 2, Children = { titleLabel, subLabel } };

            var radioOuter = new Border { HeightRequest = 20, WidthRequest = 20, StrokeShape = new RoundRectangle { CornerRadius = 10 }, Stroke = isCodSelected ? (Color)Application.Current.Resources["FloraBlush"] : (Color)Application.Current.Resources["FloraMuted"], StrokeThickness = 2, BackgroundColor = Colors.Transparent, VerticalOptions = LayoutOptions.Center };
            if (isCodSelected)
                radioOuter.Content = new Border { HeightRequest = 10, WidthRequest = 10, StrokeShape = new RoundRectangle { CornerRadius = 5 }, StrokeThickness = 0, BackgroundColor = (Color)Application.Current.Resources["FloraBlush"], HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center };

            var row = new Grid { ColumnDefinitions = new ColumnDefinitionCollection { new ColumnDefinition(GridLength.Auto), new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto) }, ColumnSpacing = 12 };
            row.Children.Add(brandDot); Grid.SetColumn(brandDot, 0);
            row.Children.Add(textStack); Grid.SetColumn(textStack, 1);
            row.Children.Add(radioOuter); Grid.SetColumn(radioOuter, 2);

            var codBorder = new Border
            {
                BackgroundColor = isCodSelected ? Color.FromArgb("#FDEEF1") : Colors.White,
                Stroke = isCodSelected ? (Color)Application.Current.Resources["FloraBlush"] : Colors.Transparent,
                StrokeThickness = isCodSelected ? 1.5 : 0,
                StrokeShape = new RoundRectangle { CornerRadius = 14 },
                Padding = 14,
                Content = row
            };

            var tap = new TapGestureRecognizer();
            tap.Tapped += (s, e) =>
            {
                foreach (var c in CartManager.Cards) c.IsSelected = false;
                CartManager.IsCashOnDeliverySelected = true;
                BuildCardsList();
                UpdateCardPreview();
            };
            codBorder.GestureRecognizers.Add(tap);

            CardsStack.Children.Add(codBorder);
        }

        private void UpdateCardPreview()
        {
            if (CartManager.IsCashOnDeliverySelected)
            {
                CardBrandLabel.Text = "COD";
                CardNumberLabel.Text = "Cash on Delivery";
                CardHolderLabel.Text = "Pay when it arrives";
                CardExpiryLabel.Text = "";
                return;
            }

            var selected = CartManager.GetSelectedCard();
            if (selected == null) return;

            CardBrandLabel.Text = selected.Brand;
            CardNumberLabel.Text = "\u2022\u2022\u2022\u2022  \u2022\u2022\u2022\u2022  \u2022\u2022\u2022\u2022  " + selected.Last4;
            CardHolderLabel.Text = selected.CardHolder;
            CardExpiryLabel.Text = selected.ExpiryText;
        }

        private async void OnAddCardTapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AddCardPage());
        }

        private async void OnReviewOrderTapped(object sender, EventArgs e)
        {
            bool hasCardSelected = CartManager.GetSelectedCard() != null && !CartManager.IsCashOnDeliverySelected;
            bool hasCodSelected = CartManager.IsCashOnDeliverySelected;

            if (!hasCardSelected && !hasCodSelected)
            {
                await DisplayAlert("Select Payment Method", "Please choose a payment method to continue.", "OK");
                return;
            }

            await Navigation.PushAsync(new ReviewOrderPage());
        }

        private async void OnBackTapped(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}