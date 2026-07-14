using Foundation;
using UIKit;

namespace FloraMix
{
    [Register("AppDelegate")]
    public class AppDelegate : MauiUIApplicationDelegate
    {
        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
        public override bool OpenUrl(UIApplication app, NSUrl url, NSDictionary options)
        {
            if (Microsoft.Maui.Authentication.WebAuthenticator.Default.OpenUrl(url))
                return true;
            return base.OpenUrl(app, url, options);
        }
    }
}