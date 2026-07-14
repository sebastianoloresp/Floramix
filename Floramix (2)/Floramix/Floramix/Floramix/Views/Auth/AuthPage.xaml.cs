namespace FloraMix.Views.Auth;

[QueryProperty(nameof(Role), "role")]
public partial class AuthPage : ContentPage
{
    private bool isSignIn = true;
    private bool isPasswordVisible = false;
    private string _role = "customer";

    public string Role
    {
        set
        {
            _role = value == "owner" ? "owner" : "customer";
            RoleLabel.Text = value == "owner" ? "Owner" : "Customer";
        }
    }

    public AuthPage()
    {
        InitializeComponent();
    }

    private async void OnBackTapped(object sender, EventArgs e) => await Shell.Current.GoToAsync("///role");

    private void OnSignInTabTapped(object sender, EventArgs e)
    {
        isSignIn = true;
        UpdateTabVisuals();
    }

    private void OnSignUpTabTapped(object sender, EventArgs e)
    {
        isSignIn = false;
        UpdateTabVisuals();
    }

    private void OnTogglePasswordVisibility(object sender, EventArgs e)
    {
        isPasswordVisible = !isPasswordVisible;
        PasswordEntry.IsPassword = !isPasswordVisible;
        PasswordEyeIcon.Text = isPasswordVisible ? "\uED1A" : "\uE7B3";
    }

    private void UpdateTabVisuals()
    {
        if (isSignIn)
        {
            SignInTabFrame.BackgroundColor = Colors.White;
            SignInTabLabel.TextColor = (Color)Application.Current!.Resources["FloraCharcoal"];
            SignUpTabFrame.BackgroundColor = Colors.Transparent;
            SignUpTabLabel.TextColor = (Color)Application.Current!.Resources["FloraMuted"];

            NameLabel.IsVisible = false;
            NameFrame.IsVisible = false;
            ForgotPasswordLabel.IsVisible = true;
            SubmitButton.Text = "Sign In";
        }
        else
        {
            SignUpTabFrame.BackgroundColor = Colors.White;
            SignUpTabLabel.TextColor = (Color)Application.Current!.Resources["FloraCharcoal"];
            SignInTabFrame.BackgroundColor = Colors.Transparent;
            SignInTabLabel.TextColor = (Color)Application.Current!.Resources["FloraMuted"];

            NameLabel.IsVisible = true;
            NameFrame.IsVisible = true;
            ForgotPasswordLabel.IsVisible = false;
            SubmitButton.Text = "Sign Up";
        }
    }

    private async void OnSignInClicked(object sender, EventArgs e)
    {
        SubmitButton.IsEnabled = false;

        if (isSignIn)
        {
            var (success, error) = await FloraMix.Services.AuthManager.SignInAsync(EmailEntry.Text, PasswordEntry.Text);
            if (!success)
            {
                await DisplayAlert("Sign In Failed", error, "OK");
                SubmitButton.IsEnabled = true;
                return;
            }
        }
        else
        {
            var (success, error) = await FloraMix.Services.AuthManager.SignUpAsync(NameEntry.Text, EmailEntry.Text, PasswordEntry.Text, _role);
            if (!success)
            {
                await DisplayAlert("Sign Up Failed", error, "OK");
                SubmitButton.IsEnabled = true;
                return;
            }
        }

        var account = FloraMix.Services.AuthManager.CurrentAccount;
        if (account != null)
        {
            FloraMix.Services.ProfileManager.FullName = account.FullName;
            FloraMix.Services.ProfileManager.Email = account.Email;
        }

        SubmitButton.IsEnabled = true;

        if (_role == "owner")
        {
            try
            {
                await Launcher.Default.OpenAsync(new Uri("https://localhost:7273"));
            }
            catch (Exception ex)
            {
                await DisplayAlert("Couldn't open Shop Portal", ex.Message, "OK");
            }
        }
        else
        {
            await Shell.Current.GoToAsync("///home");
        }
    }

    private async void OnGoogleSignInTapped(object sender, EventArgs e)
    {
        GoogleSignInFrame.IsEnabled = false;
        GoogleSignInLabel.Text = "Connecting to Google...";

        var (success, error) = await FloraMix.Services.AuthManager.SignInWithGoogleAsync(_role);

        if (!success)
        {
            await DisplayAlert("Google Sign-In Failed", error, "OK");
            GoogleSignInFrame.IsEnabled = true;
            GoogleSignInLabel.Text = "Continue with Google";
            return;
        }

        var account = FloraMix.Services.AuthManager.CurrentAccount;
        if (account != null)
        {
            FloraMix.Services.ProfileManager.FullName = account.FullName;
            FloraMix.Services.ProfileManager.Email = account.Email;
        }

        GoogleSignInFrame.IsEnabled = true;
        GoogleSignInLabel.Text = "Continue with Google";

        if (_role == "owner")
        {
            try
            {
                await Launcher.Default.OpenAsync(new Uri("https://localhost:7273"));
            }
            catch (Exception ex)
            {
                await DisplayAlert("Couldn't open Shop Portal", ex.Message, "OK");
            }
        }
        else
        {
            await Shell.Current.GoToAsync("///home");
        }
    }
}