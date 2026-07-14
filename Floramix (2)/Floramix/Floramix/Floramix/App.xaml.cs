using Microsoft.Extensions.DependencyInjection;
using FloraMix.Services;

namespace FloraMix
{
    public partial class App : Application
    {
        public App(DatabaseService databaseService)
        {
            InitializeComponent();
            UserAppTheme = AppTheme.Light;

            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                var ex = e.ExceptionObject as Exception;
                System.Diagnostics.Debug.WriteLine($"UNHANDLED EXCEPTION: {ex}");
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    if (Application.Current?.Windows.Count > 0)
                        await Application.Current.Windows[0].Page!.DisplayAlert("Unhandled Exception", ex?.ToString() ?? "Unknown error", "OK");
                });
            };

            TaskScheduler.UnobservedTaskException += (s, e) =>
            {
                System.Diagnostics.Debug.WriteLine($"UNOBSERVED TASK EXCEPTION: {e.Exception}");
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    if (Application.Current?.Windows.Count > 0)
                        await Application.Current.Windows[0].Page!.DisplayAlert("Unobserved Task Exception", e.Exception.ToString(), "OK");
                });
                e.SetObserved();
            };

            _ = InitializeDataAsync(databaseService);
            System.Diagnostics.Debug.WriteLine($"DB PATH: {FloraMix.Services.Constants.DatabasePath}");
        }

        private async Task InitializeDataAsync(DatabaseService databaseService)
        {
            CartManager.Db = databaseService;
            AuthManager.Initialize(databaseService);
            await ProductManager.InitializeAsync(databaseService);
            await CartManager.InitializeAsync(databaseService);
            await OrderHistoryManager.InitializeAsync(databaseService);
            await ProfileManager.InitializeAsync(databaseService);
            await ReminderManager.InitializeAsync(databaseService);
            await MessageManager.InitializeAsync(databaseService);
            await NotificationManager.InitializeAsync(databaseService);
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}