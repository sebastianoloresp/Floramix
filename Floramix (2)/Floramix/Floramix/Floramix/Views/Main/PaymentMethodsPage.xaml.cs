using System;
using FloraMix.Models;
using FloraMix.Services;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Graphics;

namespace FloraMix.Views.Main
{
    public partial class PaymentMethodsPage : ContentPage
    {
        public PaymentMethodsPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            BuildCards();
        }

        private void BuildCards()
        {
            CardsStack.Children.Clear();

            foreach (var card in CartManager.Cards)
            {
                var (gradientStart, gradientEnd) = card.Brand == "VISA"
                    ? (Color.FromArgb("#E8919E"), Color.FromArgb("#D4A574"))
                    : (Color.FromArgb("#5B7A5A"), Color.FromArgb("#2E3D2C"));

                var cardFace = new Grid { Padding = 20, HeightRequest = 140 };
                cardFace.Children.Add(new Label
                {
                    Text = "\uE8C7",
                    FontFamily = "Segoe Fluent Icons",
                    FontSize = 26,
                    TextColor = Colors.White,
                    Opacity = 0.85,
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.Start
                });

                if (card.IsSelected)
                {
                    cardFace.Children.Add(new Border
                    {
                        BackgroundColor = Color.FromArgb("#33FFFFFF"),
                        StrokeShape = new RoundRectangle { CornerRadius = 8 },
                        StrokeThickness = 0,
                        Padding = new Thickness(8, 3),
                        HorizontalOptions = LayoutOptions.End,
                        VerticalOptions = LayoutOptions.Start,
                        Content = new Label { Text = "Default", FontSize = 10, TextColor = Colors.White }
                    });
                }

                cardFace.Children.Add(new Label
                {
                    Text = card.Brand,
                    FontAttributes = FontAttributes.Bold,
                    FontSize = 18,
                    TextColor = Colors.White,
                    HorizontalOptions = LayoutOptions.End,
                    VerticalOptions = LayoutOptions.Start,
                    Margin = new Thickness(0, 28, 0, 0)
                });

                cardFace.Children.Add(new Label
                {
                    Text = $"\u2022\u2022\u2022\u2022  \u2022\u2022\u2022\u2022  \u2022\u2022\u2022\u2022  {card.Last4}",
                    FontSize = 16,
                    TextColor = Colors.White,
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.Center,
                    CharacterSpacing = 1
                });

                var holderLabel = new VerticalStackLayout
                {
                    Spacing = 2,
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.End,
                    Children =
                    {
                        new Label { Text = "CARD HOLDER", FontSize = 9, TextColor = Color.FromArgb("#D9FFFFFF") },
                        new Label { Text = card.CardHolder, FontSize = 13, TextColor = Colors.White }
                    }
                };
                cardFace.Children.Add(holderLabel);

                var expiryLabel = new VerticalStackLayout
                {
                    Spacing = 2,
                    HorizontalOptions = LayoutOptions.End,
                    VerticalOptions = LayoutOptions.End,
                    Children =
                    {
                        new Label { Text = "EXPIRES", FontSize = 9, TextColor = Color.FromArgb("#D9FFFFFF"), HorizontalOptions = LayoutOptions.End },
                        new Label { Text = card.ExpiryText, FontSize = 13, TextColor = Colors.White, HorizontalOptions = LayoutOptions.End }
                    }
                };
                cardFace.Children.Add(expiryLabel);

                var cardBorder = new Border
                {
                    StrokeShape = new RoundRectangle { CornerRadius = 18 },
                    StrokeThickness = 0,
                    Background = new LinearGradientBrush(
                        new GradientStopCollection
                        {
                            new GradientStop(gradientStart, 0),
                            new GradientStop(gradientEnd, 1)
                        }, new Point(0, 0), new Point(1, 1)),
                    Content = cardFace
                };

                var setDefaultBtn = new Border
                {
                    BackgroundColor = Colors.White,
                    StrokeShape = new RoundRectangle { CornerRadius = 20 },
                    Stroke = (Color)Application.Current.Resources["FloraFieldBg"],
                    StrokeThickness = 1,
                    HeightRequest = 42,
                    Content = new HorizontalStackLayout
                    {
                        Spacing = 6,
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center,
                        Children =
                        {
                            new Label { Text = card.IsSelected ? "\u2713" : "", FontSize = 13, TextColor = Color.FromArgb("#7A9A72"), VerticalOptions = LayoutOptions.Center },
                            new Label { Text = "Set as Default", FontAttributes = FontAttributes.Bold, FontSize = 13, TextColor = card.IsSelected ? Color.FromArgb("#7A9A72") : (Color)Application.Current.Resources["FloraCharcoal"], VerticalOptions = LayoutOptions.Center }
                        }
                    }
                };
                var capturedCard = card;
                setDefaultBtn.GestureRecognizers.Add(new TapGestureRecognizer { Command = new Command(() => SetDefault(capturedCard)) });

                var removeBtn = new Border
                {
                    BackgroundColor = Color.FromArgb("#FDEEF1"),
                    StrokeShape = new RoundRectangle { CornerRadius = 20 },
                    StrokeThickness = 0,
                    HeightRequest = 42,
                    WidthRequest = 100,
                    Content = new HorizontalStackLayout
                    {
                        Spacing = 6,
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center,
                        Children =
                        {
                            new Label { Text = "\uE74D", FontFamily = "Segoe Fluent Icons", FontSize = 13, TextColor = Color.FromArgb("#D9534F"), VerticalOptions = LayoutOptions.Center },
                            new Label { Text = "Remove", FontAttributes = FontAttributes.Bold, FontSize = 13, TextColor = Color.FromArgb("#D9534F"), VerticalOptions = LayoutOptions.Center }
                        }
                    }
                };
                removeBtn.GestureRecognizers.Add(new TapGestureRecognizer { Command = new Command(() => RemoveCard(capturedCard)) });

                var actionsGrid = new Grid
                {
                    ColumnDefinitions = new ColumnDefinitionCollection { new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto) },
                    ColumnSpacing = 10
                };
                actionsGrid.Children.Add(setDefaultBtn);
                Grid.SetColumn(setDefaultBtn, 0);
                actionsGrid.Children.Add(removeBtn);
                Grid.SetColumn(removeBtn, 1);

                var wrapper = new VerticalStackLayout { Spacing = 10, Children = { cardBorder, actionsGrid } };
                CardsStack.Children.Add(wrapper);
            }

            if (CartManager.CashOnDeliveryEnabled)
            {
                BuildCodCard();
            }
        }

        private void BuildCodCard()
        {
            bool isCodSelected = CartManager.IsCashOnDeliverySelected;

            var codIcon = new Border
            {
                HeightRequest = 40,
                WidthRequest = 40,
                StrokeShape = new RoundRectangle { CornerRadius = 20 },
                StrokeThickness = 0,
                BackgroundColor = Color.FromArgb("#EAF4EA"),
                Content = new Label { Text = "\uE8C7", FontFamily = "Segoe Fluent Icons", FontSize = 16, TextColor = Color.FromArgb("#5B8C5A"), HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center }
            };

            var codText = new VerticalStackLayout
            {
                Spacing = 2,
                Children =
                {
                    new Label { Text = "Cash on Delivery", FontAttributes = FontAttributes.Bold, FontSize = 14, TextColor = (Color)Application.Current.Resources["FloraCharcoal"] },
                    new Label { Text = "Pay when your order arrives", FontSize = 12, TextColor = (Color)Application.Current.Resources["FloraMuted"] }
                }
            };

            var codDot = new Border
            {
                HeightRequest = 10,
                WidthRequest = 10,
                StrokeShape = new RoundRectangle { CornerRadius = 5 },
                StrokeThickness = 0,
                BackgroundColor = isCodSelected ? Color.FromArgb("#A8C5A0") : (Color)Application.Current.Resources["FloraFieldBg"],
                VerticalOptions = LayoutOptions.Center
            };

            var codGrid = new Grid
            {
                ColumnDefinitions = new ColumnDefinitionCollection { new ColumnDefinition(GridLength.Auto), new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto) },
                ColumnSpacing = 12
            };
            codGrid.Children.Add(codIcon); Grid.SetColumn(codIcon, 0);
            codGrid.Children.Add(codText); Grid.SetColumn(codText, 1);
            codGrid.Children.Add(codDot); Grid.SetColumn(codDot, 2);

            var codBorder = new Border
            {
                BackgroundColor = isCodSelected ? Color.FromArgb("#FDEEF1") : Colors.White,
                Stroke = isCodSelected ? (Color)Application.Current.Resources["FloraBlush"] : Colors.Transparent,
                StrokeThickness = isCodSelected ? 1.5 : 0,
                StrokeShape = new RoundRectangle { CornerRadius = 16 },
                Padding = 14,
                Content = codGrid
            };

            var codSetDefaultBtn = new Border
            {
                BackgroundColor = Colors.White,
                StrokeShape = new RoundRectangle { CornerRadius = 20 },
                Stroke = (Color)Application.Current.Resources["FloraFieldBg"],
                StrokeThickness = 1,
                HeightRequest = 42,
                Content = new HorizontalStackLayout
                {
                    Spacing = 6,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    Children =
                    {
                        new Label { Text = isCodSelected ? "\u2713" : "", FontSize = 13, TextColor = Color.FromArgb("#7A9A72"), VerticalOptions = LayoutOptions.Center },
                        new Label { Text = "Set as Default", FontAttributes = FontAttributes.Bold, FontSize = 13, TextColor = isCodSelected ? Color.FromArgb("#7A9A72") : (Color)Application.Current.Resources["FloraCharcoal"], VerticalOptions = LayoutOptions.Center }
                    }
                }
            };
            codSetDefaultBtn.GestureRecognizers.Add(new TapGestureRecognizer { Command = new Command(SetCodDefault) });

            var codRemoveBtn = new Border
            {
                BackgroundColor = Color.FromArgb("#FDEEF1"),
                StrokeShape = new RoundRectangle { CornerRadius = 20 },
                StrokeThickness = 0,
                HeightRequest = 42,
                WidthRequest = 100,
                Content = new HorizontalStackLayout
                {
                    Spacing = 6,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    Children =
                    {
                        new Label { Text = "\uE74D", FontFamily = "Segoe Fluent Icons", FontSize = 13, TextColor = Color.FromArgb("#D9534F"), VerticalOptions = LayoutOptions.Center },
                        new Label { Text = "Remove", FontAttributes = FontAttributes.Bold, FontSize = 13, TextColor = Color.FromArgb("#D9534F"), VerticalOptions = LayoutOptions.Center }
                    }
                }
            };
            codRemoveBtn.GestureRecognizers.Add(new TapGestureRecognizer { Command = new Command(RemoveCod) });

            var codActionsGrid = new Grid
            {
                ColumnDefinitions = new ColumnDefinitionCollection { new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto) },
                ColumnSpacing = 10
            };
            codActionsGrid.Children.Add(codSetDefaultBtn); Grid.SetColumn(codSetDefaultBtn, 0);
            codActionsGrid.Children.Add(codRemoveBtn); Grid.SetColumn(codRemoveBtn, 1);

            var codWrapper = new VerticalStackLayout { Spacing = 10, Children = { codBorder, codActionsGrid } };
            CardsStack.Children.Add(codWrapper);
        }

        private void SetDefault(SavedCard card)
        {
            foreach (var c in CartManager.Cards)
            {
                bool shouldBeSelected = c == card;
                if (c.IsSelected != shouldBeSelected)
                {
                    c.IsSelected = shouldBeSelected;
                    CartManager.SaveCardChange(c);
                }
            }
            CartManager.IsCashOnDeliverySelected = false;
            BuildCards();
        }

        private void SetCodDefault()
        {
            foreach (var c in CartManager.Cards)
            {
                if (c.IsSelected)
                {
                    c.IsSelected = false;
                    CartManager.SaveCardChange(c);
                }
            }
            CartManager.IsCashOnDeliverySelected = true;
            BuildCards();
        }

        private async void RemoveCard(SavedCard card)
        {
            bool confirm = await DisplayAlert("Remove Card", $"Remove {card.Brand} ending in {card.Last4}?", "Remove", "Cancel");
            if (confirm)
            {
                bool wasSelected = card.IsSelected;
                CartManager.RemoveCardAndDelete(card);

                if (wasSelected)
                {
                    if (CartManager.Cards.Count > 0)
                    {
                        CartManager.Cards[0].IsSelected = true;
                        CartManager.SaveCardChange(CartManager.Cards[0]);
                    }
                    else if (CartManager.CashOnDeliveryEnabled)
                        CartManager.IsCashOnDeliverySelected = true;
                }
                BuildCards();
            }
        }

        private async void RemoveCod()
        {
            bool confirm = await DisplayAlert("Remove Cash on Delivery", "Remove Cash on Delivery as a payment option?", "Remove", "Cancel");
            if (confirm)
            {
                CartManager.CashOnDeliveryEnabled = false;
                if (CartManager.IsCashOnDeliverySelected)
                {
                    CartManager.IsCashOnDeliverySelected = false;
                    if (CartManager.Cards.Count > 0)
                    {
                        CartManager.Cards[0].IsSelected = true;
                        CartManager.SaveCardChange(CartManager.Cards[0]);
                    }
                }
                BuildCards();
            }
        }

        private async void OnAddCardTapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AddCardPage());
        }

        private async void OnBackTapped(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}