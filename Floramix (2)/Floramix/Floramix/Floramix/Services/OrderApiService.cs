using System.Net.Http.Json;

namespace FloraMix.Services
{
    public static class OrderApiService
    {
        private const string BaseUrl = "https://localhost:7273";

        private static readonly HttpClient _http = CreateClient();

        private static HttpClient CreateClient()
        {
#if WINDOWS
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (msg, cert, chain, errors) => true
            };
            return new HttpClient(handler) { BaseAddress = new Uri(BaseUrl) };
#else
            return new HttpClient { BaseAddress = new Uri(BaseUrl) };
#endif
        }

        public class OrderItemPayload
        {
            public string BouquetName { get; set; } = "";
            public decimal Price { get; set; }
            public int Quantity { get; set; }
        }

        public class OrderPayload
        {
            public string CustomerName { get; set; } = "";
            public string CustomerEmail { get; set; } = "";
            public string DeliveryLabel { get; set; } = "";
            public string Occasion { get; set; } = "Other";
            public List<OrderItemPayload> Items { get; set; } = new();
        }

        public static async Task<bool> SubmitOrderAsync(OrderPayload payload)
        {
            try
            {
                var response = await _http.PostAsJsonAsync("/api/orders", payload);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                // Web portal not reachable — order still exists locally, just not synced.
                return false;
            }
        }
    }
}