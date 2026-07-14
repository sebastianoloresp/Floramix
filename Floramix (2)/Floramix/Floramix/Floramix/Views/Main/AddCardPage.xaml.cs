using System;
using System.Linq;
using FloraMix.Models;
using FloraMix.Services;
using Microsoft.Maui.Controls;

namespace FloraMix.Views.Main
{
    public partial class AddCardPage : ContentPage
    {
        private bool _cvvVisible = false;

        public AddCardPage()
        {
            InitializeComponent();
        }

        private void OnCardNumberChanged(object sender, TextChangedEventArgs e)
        {
            string digitsOnly = new string((e.NewTextValue ?? "").Where(char.IsDigit).ToArray());
            if (digitsOnly.Length > 16) digitsOnly = digitsOnly.Substring(0, 16);

            string formatted = string.Join(" ", Enumerable.Range(0, (digitsOnly.Length + 3) / 4)
                .Select(i => digitsOnly.Substring(i * 4, Math.Min(4, digitsOnly.Length - i * 4))));

            if (CardNumberEntry.Text != formatted)
            {
                CardNumberEntry.TextChanged -= OnCardNumberChanged;
                CardNumberEntry.Text = formatted;
                CardNumberEntry.CursorPosition = formatted.Length;
                CardNumberEntry.TextChanged += OnCardNumberChanged;
            }

            string display = digitsOnly.Length == 0 ? "\u2022\u2022\u2022\u2022  \u2022\u2022\u2022\u2022  \u2022\u2022\u2022\u2022  \u2022\u2022\u2022\u2022"
                : string.Join("  ", Enumerable.Range(0, 4).Select(i =>
                {
                    int start = i * 4;
                    if (start >= digitsOnly.Length) return "\u2022\u2022\u2022\u2022";
                    string chunk = digitsOnly.Substring(start, Math.Min(4, digitsOnly.Length - start));
                    return chunk.PadRight(4, '\u2022');
                }));

            PreviewNumberLabel.Text = display;
        }

        private void OnCardHolderChanged(object sender, TextChangedEventArgs e)
        {
            PreviewHolderLabel.Text = string.IsNullOrWhiteSpace(e.NewTextValue) ? "YOUR NAME" : e.NewTextValue.ToUpper();
        }

        private void OnExpiryChanged(object sender, TextChangedEventArgs e)
        {
            string digitsOnly = new string((e.NewTextValue ?? "").Where(char.IsDigit).ToArray());
            if (digitsOnly.Length > 4) digitsOnly = digitsOnly.Substring(0, 4);

            string formatted = digitsOnly.Length > 2
                ? digitsOnly.Substring(0, 2) + "/" + digitsOnly.Substring(2)
                : digitsOnly;

            if (ExpiryEntry.Text != formatted)
            {
                ExpiryEntry.TextChanged -= OnExpiryChanged;
                ExpiryEntry.Text = formatted;
                ExpiryEntry.CursorPosition = formatted.Length;
                ExpiryEntry.TextChanged += OnExpiryChanged;
            }

            PreviewExpiryLabel.Text = string.IsNullOrEmpty(formatted) ? "MM/YY" : formatted;
        }

        private void OnToggleCvvVisibility(object sender, EventArgs e)
        {
            _cvvVisible = !_cvvVisible;
            CvvEntry.IsPassword = !_cvvVisible;
            CvvEyeIcon.Text = _cvvVisible ? "\uE7B3" : "\uE7B3";
        }

        private async void OnSaveCardTapped(object sender, EventArgs e)
        {
            string digits = new string((CardNumberEntry.Text ?? "").Where(char.IsDigit).ToArray());

            if (digits.Length < 16 || string.IsNullOrWhiteSpace(CardHolderEntry.Text) ||
                string.IsNullOrWhiteSpace(ExpiryEntry.Text) || ExpiryEntry.Text.Length < 5 ||
                string.IsNullOrWhiteSpace(CvvEntry.Text))
            {
                await DisplayAlert("Missing Info", "Please fill in all card details correctly.", "OK");
                return;
            }

            string brand = digits.StartsWith("4") ? "VISA" : digits.StartsWith("3") ? "AMEX" : "CARD";

            var newCard = new SavedCard
            {
                Brand = brand,
                Last4 = digits.Substring(digits.Length - 4),
                ExpiryText = ExpiryEntry.Text,
                CardHolder = CardHolderEntry.Text,
                IsSelected = CartManager.Cards.Count == 0
            };

            CartManager.AddCardAndSave(newCard);
            CartManager.IsCashOnDeliverySelected = false;

            await Navigation.PopAsync();
        }

        private async void OnBackTapped(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}