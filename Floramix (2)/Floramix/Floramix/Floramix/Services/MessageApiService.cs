using System.Net.Http.Json;

namespace FloraMix.Services
{
    public static class MessageApiService
    {
        // Change this if your Shop Portal runs on a different port
        private const string BaseUrl = "https://localhost:7273";

        private static readonly HttpClient _http = CreateClient();

        private static HttpClient CreateClient()
        {
#if WINDOWS
            // Trust the local ASP.NET Core dev-cert on Windows so HttpClient
            // doesn't reject it as untrusted during local development.
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (msg, cert, chain, errors) => true
            };
            return new HttpClient(handler) { BaseAddress = new Uri(BaseUrl) };
#else
            return new HttpClient { BaseAddress = new Uri(BaseUrl) };
#endif
        }

        public class MessageDto
        {
            public int Id { get; set; }
            public string Sender { get; set; } = "";
            public string Text { get; set; } = "";
            public DateTime SentAt { get; set; }
            public bool IsRead { get; set; }
        }

        private class MessagesResponse
        {
            public int ConversationId { get; set; }
            public List<MessageDto> Messages { get; set; } = new();
        }

        // Pulls the full message thread for an order's conversation from the web portal.
        public static async Task<List<MessageDto>?> FetchMessagesAsync(int serverOrderId)
        {
            if (serverOrderId == 0) return null;

            try
            {
                var response = await _http.GetFromJsonAsync<MessagesResponse>($"/api/orders/{serverOrderId}/messages");
                return response?.Messages;
            }
            catch
            {
                // Web portal not reachable (not running, wrong network, etc.)
                return null;
            }
        }

        // Sends a message from the customer app up to the web portal, where the shop owner will see it.
        public static async Task<MessageDto?> SendMessageAsync(int serverOrderId, string text, bool asShop = false)
        {
            if (serverOrderId == 0) return null;

            try
            {
                var payload = new { Sender = asShop ? "Shop" : "Customer", Text = text };
                var response = await _http.PostAsJsonAsync($"/api/orders/{serverOrderId}/messages", payload);
                if (!response.IsSuccessStatusCode)
                    return null;

                return await response.Content.ReadFromJsonAsync<MessageDto>();
            }
            catch
            {
                // Web portal not reachable — message still saved locally, just not synced.
                return null;
            }
        }
    }
}