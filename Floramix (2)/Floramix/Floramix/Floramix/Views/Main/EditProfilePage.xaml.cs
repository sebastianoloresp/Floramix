using System;
using FloraMix.Services;
using Microsoft.Maui.Controls;

namespace FloraMix.Views.Main
{
    public partial class EditProfilePage : ContentPage
    {
        public EditProfilePage()
        {
            InitializeComponent();
            FullNameEntry.Text = ProfileManager.FullName;
            EmailEntry.Text = ProfileManager.Email;
            PhoneEntry.Text = ProfileManager.Phone;
            LocationEntry.Text = ProfileManager.Location;
        }

        private async void OnSaveTapped(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(FullNameEntry.Text) || string.IsNullOrWhiteSpace(EmailEntry.Text))
            {
                await DisplayAlert("Missing Info", "Name and email cannot be empty.", "OK");
                return;
            }

            ProfileManager.FullName = FullNameEntry.Text;
            ProfileManager.Email = EmailEntry.Text;
            ProfileManager.Phone = PhoneEntry.Text;
            ProfileManager.Location = LocationEntry.Text;

            await Navigation.PopAsync();
        }

        private async void OnCancelTapped(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}