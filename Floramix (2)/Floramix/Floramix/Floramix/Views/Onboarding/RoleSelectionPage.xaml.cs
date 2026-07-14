namespace FloraMix.Views.Onboarding;

public partial class RoleSelectionPage : ContentPage
{
    public RoleSelectionPage()
    {
        InitializeComponent();
    }

    private async void OnBackTapped(object sender, EventArgs e) => await Shell.Current.GoToAsync("///onboarding");
    private async void OnCustomerTapped(object sender, EventArgs e) => await Shell.Current.GoToAsync("///auth?role=customer");
    private async void OnOwnerTapped(object sender, EventArgs e) => await Shell.Current.GoToAsync("///auth?role=owner");
}