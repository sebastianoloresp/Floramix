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

        // What the server sends back after creating an order
        public class OrderCreatedResponse
        {
            public int Id { get; set; }
            public string OrderCode { get; set; } = "";
        }

        // What the server sends back when checking status
        public class OrderStatusResponse
        {
            public int Id { get; set; }
            public string OrderCode { get; set; } = "";
            public string Status { get; set; } = "";
        }

        // Now returns the created order info instead of just true/false
        public static async Task<OrderCreatedResponse?> SubmitOrderAsync(OrderPayload payload)
        {
            try
            {
                var response = await _http.PostAsJsonAsync("/api/orders", payload);
                if (!response.IsSuccessStatusCode)
                    return null;

                return await response.Content.ReadFromJsonAsync<OrderCreatedResponse>();
            }
            catch
            {
                // Web portal not reachable — order still exists locally, just not synced.
                return null;
            }
        }

        // Lets the app cancel an order on the server (so the shop portal and DB stay in sync)
        public static async Task<OrderStatusResponse?> CancelOrderAsync(int serverOrderId)
        {
            try
            {
                var response = await _http.PutAsync($"/api/orders/{serverOrderId}/cancel", null);
                if (!response.IsSuccessStatusCode)
                    return null;

                return await response.Content.ReadFromJsonAsync<OrderStatusResponse>();
            }
            catch
            {
                // Web portal not reachable — caller decides how to handle offline cancellation.
                return null;
            }
        }

        // New: lets the app check the latest status of an order
        public static async Task<OrderStatusResponse?> GetOrderStatusAsync(int serverOrderId)
        {
            try
            {
                var response = await _http.GetAsync($"/api/orders/{serverOrderId}");
                if (!response.IsSuccessStatusCode)
                    return null;

                return await response.Content.ReadFromJsonAsync<OrderStatusResponse>();
            }
            catch
            {
                return null;
            }
        }
    }
}