using System.Net.Http.Json;
using FloraMix.Models;

namespace FloraMix.Services
{
    public static class ProductApiService
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

        private class BouquetDto
        {
            public int Id { get; set; }
            public string Name { get; set; } = "";
            public string Description { get; set; } = "";
            public decimal Price { get; set; }
            public string? ImageUrl { get; set; }
            public string Category { get; set; } = "";
            public double Rating { get; set; }
            public int SoldCount { get; set; }
            public int StockCount { get; set; }
            public string Status { get; set; } = "";
            public bool IsAvailable { get; set; }
        }

        public static async Task<List<Product>?> FetchProductsAsync()
        {
            try
            {
                var dtos = await _http.GetFromJsonAsync<List<BouquetDto>>("/api/bouquets");
                if (dtos == null) return null;

                return dtos.Select(d => new Product
                {
                    Id = d.Id,
                    ImageSource = ResolveImageSource(d.ImageUrl),
                    DetailImageSource = ResolveImageSource(d.ImageUrl),
                    Shop = "Wild Flowers",
                    Name = d.Name,
                    Rating = d.Rating,
                    ReviewCount = d.SoldCount,
                    Price = (double)d.Price,
                    Tag = d.Category,
                    Category = d.Category,
                    StockCount = d.StockCount,
                    Description = d.Description
                }).ToList();
            }
            catch
            {
                // Web portal not reachable (not running, wrong network, etc.)
                // Callers should fall back to whatever is cached locally.
                return null;
            }
        }

        private static string ResolveImageSource(string? imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
                return "placeholder.png";

            // Web portal returns a relative path like "/images/products/x.png"
            return imageUrl.StartsWith("/") ? $"{BaseUrl}{imageUrl}" : imageUrl;
        }
    }
}