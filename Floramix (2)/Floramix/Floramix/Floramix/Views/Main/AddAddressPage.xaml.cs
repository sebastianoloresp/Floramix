using System;
using FloraMix.Models;
using FloraMix.Services;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace FloraMix.Views.Main
{
    public partial class AddAddressPage : ContentPage
    {
        private readonly SavedAddress _editingAddress;
        private string _selectedType = "Home";

        public AddAddressPage()
        {
            InitializeComponent();
            _editingAddress = null;
            SetSelectedType("Home");
        }

        public AddAddressPage(SavedAddress addressToEdit)
        {
            InitializeComponent();
            _editingAddress = addressToEdit;

            TitleLabel.Text = "Edit Address";
            LabelEntry.Text = addressToEdit.Label;
            StreetEntry.Text = addressToEdit.AddressLine;
            ApartmentEntry.Text = addressToEdit.ApartmentLine;
            CityEntry.Text = addressToEdit.City;
            PostcodeEntry.Text = addressToEdit.Postcode;

            SetSelectedType(addressToEdit.AddressType ?? "Home");
        }

        private void OnHomeTypeTapped(object sender, EventArgs e) => SetSelectedType("Home");
        private void OnWorkTypeTapped(object sender, EventArgs e) => SetSelectedType("Work");
        private void OnOtherTypeTapped(object sender, EventArgs e) => SetSelectedType("Other");

        private void SetSelectedType(string type)
        {
            _selectedType = type;

            var blush = (Color)Application.Current.Resources["FloraBlush"];
            var charcoal = (Color)Application.Current.Resources["FloraCharcoal"];
            var fieldBg = (Color)Application.Current.Resources["FloraFieldBg"];

            (Border border, Label icon, Label label)[] items =
            {
                (HomeTypeBorder, HomeTypeIcon, HomeTypeLabel),
                (WorkTypeBorder, WorkTypeIcon, WorkTypeLabel),
                (OtherTypeBorder, OtherTypeIcon, OtherTypeLabel),
            };
            string[] names = { "Home", "Work", "Other" };

            for (int i = 0; i < items.Length; i++)
            {
                bool selected = names[i] == type;
                items[i].border.BackgroundColor = selected ? Color.FromArgb("#FDEEF1") : Colors.White;
                items[i].border.Stroke = selected ? blush : fieldBg;
                items[i].icon.TextColor = selected ? blush : charcoal;
                items[i].label.TextColor = selected ? blush : charcoal;
            }
        }

        private async void OnSaveTapped(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(StreetEntry.Text) || string.IsNullOrWhiteSpace(CityEntry.Text) || string.IsNullOrWhiteSpace(PostcodeEntry.Text))
            {
                await DisplayAlert("Missing Info", "Please fill in street address, city, and postcode.", "OK");
                return;
            }

            string label = string.IsNullOrWhiteSpace(LabelEntry.Text) ? _selectedType : LabelEntry.Text;

            if (_editingAddress != null)
            {
                _editingAddress.Label = label;
                _editingAddress.AddressType = _selectedType;
                _editingAddress.AddressLine = StreetEntry.Text;
                _editingAddress.ApartmentLine = ApartmentEntry.Text;
                _editingAddress.City = CityEntry.Text;
                _editingAddress.Postcode = PostcodeEntry.Text;
                _editingAddress.CityLine = $"{CityEntry.Text}, {PostcodeEntry.Text}";

                CartManager.UpdateAddressAndSave(_editingAddress);
            }
            else
            {
                var newAddress = new SavedAddress
                {
                    Label = label,
                    AddressType = _selectedType,
                    IsDefault = CartManager.Addresses.Count == 0,
                    AddressLine = StreetEntry.Text,
                    ApartmentLine = ApartmentEntry.Text,
                    City = CityEntry.Text,
                    Postcode = PostcodeEntry.Text,
                    CityLine = $"{CityEntry.Text}, {PostcodeEntry.Text}"
                };

                CartManager.AddAddressAndSave(newAddress);
            }

            await Navigation.PopAsync();
        }

        private async void OnBackTapped(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}