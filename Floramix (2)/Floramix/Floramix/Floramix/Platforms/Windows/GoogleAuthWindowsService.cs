using System;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace FloraMix.Platforms.Windows
{
    public static class GoogleAuthWindowsService
    {
        public static Task<string?> AuthenticateAsync(string authUrl, string redirectUri)
        {
            var tcs = new TaskCompletionSource<string?>();

            var window = new Microsoft.UI.Xaml.Window
            {
                Title = "Sign in with Google"
            };

            var webView = new WebView2();
            window.Content = webView;
            window.Activate();

            bool completed = false;

            void Complete(string? code)
            {
                if (completed) return;
                completed = true;
                tcs.TrySetResult(code);
                window.Close();
            }

            window.Closed += (s, e) => Complete(null);

            _ = InitializeAsync();

            async Task InitializeAsync()
            {
                await webView.EnsureCoreWebView2Async();

                webView.CoreWebView2.NavigationStarting += (sender, args) =>
                {
                    if (IsRedirect(args.Uri, redirectUri))
                    {
                        args.Cancel = true;
                        string? code = ExtractQueryParam(args.Uri, "code");
                        Complete(code);
                    }
                };

                webView.CoreWebView2.Navigate(authUrl);
            }

            return tcs.Task;
        }

        private static bool IsRedirect(string uri, string redirectUri)
        {
            // Matches http://127.0.0.1, http://127.0.0.1/, http://127.0.0.1:PORT/..., and custom schemes
            return uri.StartsWith(redirectUri, StringComparison.OrdinalIgnoreCase);
        }

        private static string? ExtractQueryParam(string url, string key)
        {
            int idx = url.IndexOf('?');
            if (idx < 0) return null;

            string query = url.Substring(idx + 1);
            foreach (var pair in query.Split('&'))
            {
                var kv = pair.Split('=', 2);
                if (kv.Length == 2 && kv[0] == key)
                    return Uri.UnescapeDataString(kv[1]);
            }
            return null;
        }
    }
}