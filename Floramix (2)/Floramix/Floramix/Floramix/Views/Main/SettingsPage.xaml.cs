using System;
using FloraMix.Services;
using Microsoft.Maui.Controls;

namespace FloraMix.Views.Main
{
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage()
        {
            InitializeComponent();
            LoadValues();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadValues();
        }

        private void LoadValues()
        {
            AvatarInitialLabel.Text = string.IsNullOrWhiteSpace(ProfileManager.FullName) ? "?" : ProfileManager.FullName.Substring(0, 1).ToUpper();
            AccountNameLabel.Text = ProfileManager.FullName;
            AccountEmailLabel.Text = ProfileManager.Email;
            AccountPhoneLabel.Text = ProfileManager.Phone;

            DarkModeSwitch.Toggled -= OnDarkModeToggled;
            DarkModeSwitch.IsToggled = ProfileManager.DarkModeEnabled;
            DarkModeSwitch.Toggled += OnDarkModeToggled;

            LanguageValueLabel.Text = ProfileManager.Language;
            CurrencyValueLabel.Text = ProfileManager.Currency;

            FaceIdSwitch.Toggled -= OnFaceIdToggled;
            FaceIdSwitch.IsToggled = ProfileManager.FaceIdEnabled;
            FaceIdSwitch.Toggled += OnFaceIdToggled;

            TwoFactorSwitch.Toggled -= OnTwoFactorToggled;
            TwoFactorSwitch.IsToggled = ProfileManager.TwoFactorEnabled;
            TwoFactorSwitch.Toggled += OnTwoFactorToggled;

            AppVersionLabel.Text = ProfileManager.AppVersion;
        }

        private void OnDarkModeToggled(object sender, ToggledEventArgs e)
        {
            ProfileManager.DarkModeEnabled = e.Value;
            Application.Current.UserAppTheme = e.Value ? AppTheme.Dark : AppTheme.Light;
        }

        private void OnFaceIdToggled(object sender, ToggledEventArgs e)
        {
            ProfileManager.FaceIdEnabled = e.Value;
        }

        private void OnTwoFactorToggled(object sender, ToggledEventArgs e)
        {
            ProfileManager.TwoFactorEnabled = e.Value;
        }

        private async void OnLanguageTapped(object sender, EventArgs e)
        {
            string result = await DisplayActionSheet("Select Language", "Cancel", null, "English", "Spanish", "French", "German");
            if (!string.IsNullOrEmpty(result) && result != "Cancel")
            {
                ProfileManager.Language = result;
                LanguageValueLabel.Text = result;
            }
        }

        private async void OnCurrencyTapped(object sender, EventArgs e)
        {
            string result = await DisplayActionSheet("Select Currency", "Cancel", null, "GBP \u00A3", "USD $", "EUR \u20AC", "PHP \u20B1");
            if (!string.IsNullOrEmpty(result) && result != "Cancel")
            {
                ProfileManager.Currency = result;
                CurrencyValueLabel.Text = result;
            }
        }

        private async void OnEditProfileTapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new EditProfilePage());
        }

        private void OnChangePasswordTapped(object sender, EventArgs e)
        {
            CurrentPasswordEntry.Text = "";
            NewPasswordEntry.Text = "";
            ConfirmPasswordEntry.Text = "";
            PasswordOverlay.IsVisible = true;
        }

        private void OnClosePasswordOverlay(object sender, EventArgs e)
        {
            PasswordOverlay.IsVisible = false;
        }

        private async void OnUpdatePasswordTapped(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(CurrentPasswordEntry.Text) ||
                string.IsNullOrWhiteSpace(NewPasswordEntry.Text) ||
                string.IsNullOrWhiteSpace(ConfirmPasswordEntry.Text))
            {
                await DisplayAlert("Missing Info", "Please fill in all password fields.", "OK");
                return;
            }

            if (NewPasswordEntry.Text != ConfirmPasswordEntry.Text)
            {
                await DisplayAlert("Passwords Don't Match", "New password and confirmation must match.", "OK");
                return;
            }

            PasswordOverlay.IsVisible = false;
            await DisplayAlert("Success", "Your password has been updated.", "OK");
        }

        private async void OnBackTapped(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}