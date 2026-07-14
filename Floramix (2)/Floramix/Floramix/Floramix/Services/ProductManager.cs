using System.Collections.Generic;
using System.Linq;
using FloraMix.Models;

namespace FloraMix.Services
{
    public static class ProductManager
    {
        public static List<Product> AllProducts { get; private set; } = new List<Product>();
        private static bool _initialized = false;
        private static TaskCompletionSource _readyTcs = new TaskCompletionSource();
        public static Task Ready => _readyTcs.Task;
        private static DatabaseService? _db;

        public static async Task InitializeAsync(DatabaseService db)
        {
            if (_initialized) return;
            _db = db;

            var fromApi = await ProductApiService.FetchProductsAsync();
            if (fromApi != null && fromApi.Count > 0)
            {
                await db.DeleteAllProductsAsync();
                foreach (var product in fromApi)
                    await db.SaveProductFromApiAsync(product);
            }
            else
            {
                var existingCount = await db.GetProductCountAsync();
                if (existingCount == 0)
                {
                    foreach (var product in SeedProducts())
                        await db.SaveProductAsync(product);
                }
            }

            AllProducts = await db.GetProductsAsync();
            _initialized = true;
            _readyTcs.TrySetResult();
        }

        public static async Task RefreshFromApiAsync()
        {
            if (_db == null) return;

            var fromApi = await ProductApiService.FetchProductsAsync();
            if (fromApi == null || fromApi.Count == 0) return; // portal unreachable, keep current data

            await _db.DeleteAllProductsAsync();
            foreach (var product in fromApi)
                await _db.SaveProductFromApiAsync(product);

            AllProducts = await _db.GetProductsAsync();
        }

        private static List<Product> SeedProducts()
        {
            return new List<Product>
            {
                new Product
                {
                    ImageSource = "bouquet_rose_chocolate.png",
                    DetailImageSource = "bouquet_rose_chocolate_detail.png",
                    Shop = "Wild Flowers",
                    Name = "Rose & Ferrero Rocher Combo",
                    Rating = 4.9,
                    ReviewCount = 127,
                    Price = 1599,
                    Tag = "Romance",
                    Category = "Romance",
                    Location = "Sta Rosa, Laguna",
                    Description = "A romantic pairing of fresh pink roses and premium Ferrero Rocher chocolates, beautifully wrapped together in soft craft paper. Hand-tied by our florists and finished with a satin ribbon, it's a classic gift for anniversaries, Valentine's Day, or any moment that calls for a grand, memorable gesture.",
                    WhatsIncluded = new List<string> { "6 Pink Roses", "Ferrero Rocher box (16 pcs)", "Eucalyptus fillers", "Satin ribbon wrap" },
                    ColorPalette = new List<string> { "#E8A0B4", "#D4AF37", "#FFFFFF", "#8B2E2E" },
                    Reviews = new List<ReviewItem>
                    {
                        new ReviewItem { ReviewerName = "Sophie M.", Stars = 5, Comment = "Absolutely beautiful! My mum cried happy tears. The roses were so fresh and the chocolates were a lovely surprise touch.", Date = "May 28, 2026" },
                        new ReviewItem { ReviewerName = "James T.", Stars = 5, Comment = "Ordered this for my anniversary - arrived on time and looked exactly like the photo. Will definitely order again!", Date = "May 15, 2026" },
                    }
                },
                new Product
                {
                    ImageSource = "bouquet_red_lavender.png",
                    DetailImageSource = "bouquet_red_lavender_detail.png",
                    Shop = "Wild Flowers",
                    Name = "Red Rose Bouquet with Lavender Fillers",
                    Rating = 4.8,
                    ReviewCount = 204,
                    Price = 1850,
                    Tag = "Romance",
                    Category = "Romance",
                    Location = "Sta Rosa, Laguna",
                    Description = "A timeless arrangement of deep red roses layered with fragrant lavender sprigs, finished with soft craft paper for an understated, elegant presentation. The scent of lavender lingers long after the roses are placed, making this an especially memorable choice for anniversaries and heartfelt declarations.",
                    WhatsIncluded = new List<string> { "10 Red Roses", "5 Lavender sprigs", "4 Eucalyptus stems", "Craft paper wrap" },
                    ColorPalette = new List<string> { "#8B2E2E", "#967BB6", "#FFFFFF", "#9CAF88" },
                    Reviews = new List<ReviewItem>
                    {
                        new ReviewItem { ReviewerName = "Andrea L.", Stars = 5, Comment = "The lavender scent made this so much more special than a plain rose bouquet. Roses lasted almost two weeks!", Date = "Jun 1, 2026" },
                        new ReviewItem { ReviewerName = "Marco D.", Stars = 4, Comment = "Beautiful arrangement, though a couple of the roses were slightly bruised on arrival. Still very happy overall.", Date = "May 20, 2026" },
                    }
                },
                new Product
                {
                    ImageSource = "bouquet_sunflower_yellow.png",
                    DetailImageSource = "bouquet_sunflower_yellow_detail.png",
                    Shop = "Wild Flowers",
                    Name = "Sunflower Bouquet",
                    Rating = 4.6,
                    ReviewCount = 52,
                    Price = 1150,
                    Tag = "Birthday",
                    Category = "Birthday",
                    Location = "Sta Rosa, Laguna",
                    Description = "A bright, sunny bouquet of fresh sunflowers paired with rustic dried pampas grass and soft greenery, wrapped in natural kraft paper and tied with twine. Cheerful and full of personality, it's a perfect pick-me-up for birthdays, thank-you gifts, or simply brightening someone's day.",
                    WhatsIncluded = new List<string> { "6 Sunflowers", "4 Dried Pampas stems", "Eucalyptus fillers", "Natural twine tie" },
                    ColorPalette = new List<string> { "#FFD23F", "#8B5A2B", "#5B7B4F", "#F5F0E6" },
                    Reviews = new List<ReviewItem>
                    {
                        new ReviewItem { ReviewerName = "Kristine P.", Stars = 5, Comment = "So bright and happy looking! Perfect for my sister's birthday, she loved it.", Date = "May 28, 2026" },
                        new ReviewItem { ReviewerName = "Paolo R.", Stars = 4, Comment = "Great value and the sunflowers were huge and fresh. Delivery was right on schedule.", Date = "May 10, 2026" },
                    }
                },
                new Product
                {
                    ImageSource = "bouquet_tulip_mixed.png",
                    DetailImageSource = "bouquet_tulip_mixed_detail.png",
                    Shop = "Wild Flowers",
                    Name = "Dutch Tulip Bouquet",
                    Rating = 4.8,
                    ReviewCount = 89,
                    Price = 2300,
                    Tag = "Wedding",
                    Category = "Wedding",
                    Location = "Sta Rosa, Laguna",
                    Description = "A fresh, imported mix of Dutch tulips in soft pastel tones, elegantly wrapped in delicate lace-style paper. With their smooth, graceful petals and refined color palette, these tulips are a wonderful choice for weddings, bridal showers, and other milestone celebrations that call for understated beauty.",
                    WhatsIncluded = new List<string> { "12 Mixed Dutch Tulips", "Lisianthus fillers", "Lavender sprigs", "Lace wrap paper" },
                    ColorPalette = new List<string> { "#F4C2C2", "#967BB6", "#FFFFFF", "#FFDAB9" },
                    Reviews = new List<ReviewItem>
                    {
                        new ReviewItem { ReviewerName = "Camille V.", Stars = 5, Comment = "Used these for our bridal shower centerpieces - gorgeous colors and they held up beautifully all day.", Date = "Jun 2, 2026" },
                        new ReviewItem { ReviewerName = "Ian F.", Stars = 5, Comment = "The tulips were fresher than I expected for an imported flower. Very impressed with the presentation.", Date = "May 22, 2026" },
                    }
                },
                new Product
                {
                    ImageSource = "bouquet_gerbera_colorful.png",
                    DetailImageSource = "bouquet_gerbera_colorful_detail.png",
                    Shop = "Wild Flowers",
                    Name = "Gerbera Daisy Bouquet (Mixed Colors)",
                    Rating = 4.7,
                    ReviewCount = 93,
                    Price = 950,
                    Tag = "Birthday",
                    Category = "Birthday",
                    Location = "Sta Rosa, Laguna",
                    Description = "A vibrant, colorful mix of gerbera daisies in cheerful pink, orange, and yellow hues, loosely arranged with kraft paper wrapping for a playful, feel-good look. Affordable and eye-catching, it's a favorite for birthdays, get-well wishes, or simply brightening someone's ordinary day.",
                    WhatsIncluded = new List<string> { "8 Mixed Gerbera Daisies", "Waxflower fillers", "Ruscus stems", "Kraft paper wrap" },
                    ColorPalette = new List<string> { "#FF69B4", "#FF8C00", "#FFD700", "#FFFFFF" },
                    Reviews = new List<ReviewItem>
                    {
                        new ReviewItem { ReviewerName = "Sophie M.", Stars = 5, Comment = "Such a cheerful bouquet! The colors were even more vivid in person than in the photos.", Date = "May 30, 2026" },
                        new ReviewItem { ReviewerName = "Ella G.", Stars = 4, Comment = "Sent this to a friend who was feeling under the weather and it really brightened her day. Great price too.", Date = "May 18, 2026" },
                    }
                },
                new Product
                {
                    ImageSource = "bouquet_lily_white.png",
                    DetailImageSource = "bouquet_lily_white_detail.png",
                    Shop = "Wild Flowers",
                    Name = "White Lily Bouquet",
                    Rating = 4.9,
                    ReviewCount = 167,
                    Price = 1700,
                    Tag = "Sympathy",
                    Category = "Sympathy",
                    Location = "Sta Rosa, Laguna",
                    Description = "A serene, all-white arrangement of fragrant lilies and roses accented with soft eucalyptus, designed to convey comfort and quiet respect. Simple, elegant, and gently fragrant, this bouquet is a thoughtful choice for sympathy, condolence, or any occasion calling for a peaceful gesture.",
                    WhatsIncluded = new List<string> { "8 White Lilies", "6 White Roses", "5 Eucalyptus stems", "White ribbon" },
                    ColorPalette = new List<string> { "#FFFFFF", "#F5F5F0", "#9CAF88", "#FFFFF0" },
                    Reviews = new List<ReviewItem>
                    {
                        new ReviewItem { ReviewerName = "Renz A.", Stars = 5, Comment = "Elegant and tasteful, exactly what we needed. The florist even called to confirm the delivery time was appropriate.", Date = "May 25, 2026" },
                        new ReviewItem { ReviewerName = "Michelle C.", Stars = 5, Comment = "Beautifully arranged and the lilies smelled wonderful without being overpowering. Very thoughtful presentation.", Date = "May 12, 2026" },
                    }
                },
            };
        }

        public static void SyncWishlistState()
        {
            foreach (var product in AllProducts)
                product.IsWishlisted = CartManager.Wishlist.Any(w => w.Name == product.Name);
        }

        public static async void ToggleWishlist(Product product)
        {
            var existing = CartManager.Wishlist.FirstOrDefault(w => w.Name == product.Name);
            if (existing != null)
            {
                CartManager.Wishlist.Remove(existing);
                product.IsWishlisted = false;
                if (_db != null) await _db.DeleteWishlistItemAsync(existing);
            }
            else
            {
                var newItem = new WishlistItem
                {
                    ImageSource = product.ImageSource,
                    Shop = product.Shop,
                    Name = product.Name,
                    Tag = product.Tag,
                    Price = product.Price
                };
                CartManager.Wishlist.Add(newItem);
                product.IsWishlisted = true;
                if (_db != null) await _db.SaveWishlistItemAsync(newItem);
            }
        }

        public static async void AddToCart(Product product, int quantity)
        {
            var existingItem = CartManager.Items.FirstOrDefault(i => i.Name == product.Name);
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
                if (_db != null) await _db.SaveCartItemAsync(existingItem);
            }
            else
            {
                var newItem = new CartItem
                {
                    ProductId = product.Id,
                    ImageSource = product.ImageSource,
                    Name = product.Name,
                    Shop = product.Shop,
                    Price = product.Price,
                    Quantity = quantity,
                    ColorTag = product.Tag
                };
                CartManager.Items.Add(newItem);
                if (_db != null) await _db.SaveCartItemAsync(newItem);
            }
        }
    }
}