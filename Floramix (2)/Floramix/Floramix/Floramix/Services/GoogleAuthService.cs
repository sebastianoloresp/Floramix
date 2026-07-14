using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Maui.Authentication;

namespace FloraMix.Services
{
    public static class GoogleAuthService
    {
#if WINDOWS
        private const string ClientId = "72864643286-v6bprsliaro129m4clp9ichb8q9f9ous.apps.googleusercontent.com";
        private const string ClientSecret = "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";
        private const string RedirectUri = "http://127.0.0.1";
#else
        private const string ClientId = "72864643286-f5f8764huqc8rumh17e1p1a3bq38nmh1.apps.googleusercontent.com";
        private const string ClientSecret = "";
        private const string RedirectUri = "com.companyname.floramix:/oauth2redirect";
#endif

        public static async Task<(bool Success, string Email, string FullName, string Error)> SignInAsync()
        {
            try
            {
                string codeVerifier = GenerateCodeVerifier();
                string codeChallenge = GenerateCodeChallenge(codeVerifier);

                string authUrl = "https://accounts.google.com/o/oauth2/v2/auth" +
                    $"?client_id={ClientId}" +
                    $"&redirect_uri={Uri.EscapeDataString(RedirectUri)}" +
                    "&response_type=code" +
                    "&scope=" + Uri.EscapeDataString("openid email profile") +
                    $"&code_challenge={codeChallenge}" +
                    "&code_challenge_method=S256" +
                    "&prompt=select_account";

                string? code;

#if WINDOWS
                code = await FloraMix.Platforms.Windows.GoogleAuthWindowsService.AuthenticateAsync(authUrl, RedirectUri);
#else
                WebAuthenticatorResult authResult = await WebAuthenticator.Default.AuthenticateAsync(
                    new WebAuthenticatorOptions
                    {
                        Url = new Uri(authUrl),
                        CallbackUrl = new Uri(RedirectUri)
                    });

                authResult.Properties.TryGetValue("code", out code);
#endif

                if (string.IsNullOrEmpty(code))
                    return (false, "", "", "No authorization code received from Google.");

                using var http = new HttpClient();
                var tokenRequest = new Dictionary<string, string>
                {
                    ["code"] = code,
                    ["client_id"] = ClientId,
                    ["redirect_uri"] = RedirectUri,
                    ["grant_type"] = "authorization_code",
                    ["code_verifier"] = codeVerifier
                };

                if (!string.IsNullOrEmpty(ClientSecret))
                    tokenRequest["client_secret"] = ClientSecret;

                var response = await http.PostAsync(
                    "https://oauth2.googleapis.com/token",
                    new FormUrlEncodedContent(tokenRequest));

                var json = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    return (false, "", "", $"Token exchange failed: {json}");

                using var doc = JsonDocument.Parse(json);
                string idToken = doc.RootElement.GetProperty("id_token").GetString()!;

                var (email, fullName) = DecodeIdToken(idToken);
                return (true, email, fullName, "");
            }
            catch (TaskCanceledException)
            {
                return (false, "", "", "Sign-in was cancelled.");
            }
            catch (Exception ex)
            {
                return (false, "", "", ex.Message);
            }
        }

        private static (string Email, string FullName) DecodeIdToken(string idToken)
        {
            var payload = idToken.Split('.')[1];
            payload = payload.Replace('-', '+').Replace('_', '/');
            switch (payload.Length % 4)
            {
                case 2: payload += "=="; break;
                case 3: payload += "="; break;
            }

            var bytes = Convert.FromBase64String(payload);
            var json = Encoding.UTF8.GetString(bytes);
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            string email = root.GetProperty("email").GetString() ?? "";
            string name = root.TryGetProperty("name", out var n) ? n.GetString() ?? email : email;
            return (email, name);
        }

        private static string GenerateCodeVerifier()
        {
            var bytes = new byte[32];
            RandomNumberGenerator.Fill(bytes);
            return Base64UrlEncode(bytes);
        }

        private static string GenerateCodeChallenge(string codeVerifier)
        {
            using var sha256 = SHA256.Create();
            var challengeBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(codeVerifier));
            return Base64UrlEncode(challengeBytes);
        }

        private static string Base64UrlEncode(byte[] bytes)
        {
            return Convert.ToBase64String(bytes)
                .Replace('+', '-')
                .Replace('/', '_')
                .TrimEnd('=');
        }
    }
}