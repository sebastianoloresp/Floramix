using FloraMix.Shared.Services;
using FloraMix.Web.Components;
using FloraMix.Web.Services;
using FloraMix.Web.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add the EF Core database context
builder.Services.AddDbContextFactory<FloraMixDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

// Add the shop service (used by the owner portal pages)
builder.Services.AddScoped<IShopService, ShopService>();
builder.Services.AddScoped<ShopStateService>();

// Add device-specific services used by the FloraMix.Shared project
builder.Services.AddSingleton<IFormFactor, FormFactor>();

var app = builder.Build();

// Seed a test shop so the owner pages have data to work with
using (var scope = app.Services.CreateScope())
{
    var dbFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<FloraMixDbContext>>();
    using var db = dbFactory.CreateDbContext();
    db.Database.EnsureCreated();
    if (!db.Shops.Any())
    {
        db.Shops.Add(new FloraMix.Shared.Models.Shop
        {
            Name = "Wild Flowers",
            Address = "Sta. Rosa, Laguna",
            Location = "Sta. Rosa, Laguna",
            Hours = "Mon–Sat 9am–6pm"
        });

        db.Bouquets.AddRange(
            new FloraMix.Shared.Models.Bouquet
            {
                ShopId = 1,
                Name = "Rose & Ferrero Rocher Combo",
                Description = "A romantic pairing of fresh pink roses and premium Ferrero Rocher chocolates, beautifully wrapped together in soft craft paper. Hand-tied by our florists and finished with a satin ribbon, it's a classic gift for anniversaries, Valentine's Day, or any moment that calls for a grand, memorable gesture.",
                Price = 1599,
                StockCount = 8,
                Category = FloraMix.Shared.Models.Occasion.Romance,
                Rating = 4.9,
                SoldCount = 127,
                Status = FloraMix.Shared.Models.ProductStatus.Active,
                ImageUrl = "/images/products/bouquet_rose_chocolate.png"
            },
            new FloraMix.Shared.Models.Bouquet
            {
                ShopId = 1,
                Name = "Red Rose Bouquet with Lavender Fillers",
                Description = "A timeless arrangement of deep red roses layered with fragrant lavender sprigs, finished with soft craft paper for an understated, elegant presentation. The scent of lavender lingers long after the roses are placed, making this an especially memorable choice for anniversaries and heartfelt declarations.",
                Price = 1850,
                StockCount = 8,
                Category = FloraMix.Shared.Models.Occasion.Romance,
                Rating = 4.8,
                SoldCount = 204,
                Status = FloraMix.Shared.Models.ProductStatus.Active,
                ImageUrl = "/images/products/bouquet_red_lavender.png"
            },
            new FloraMix.Shared.Models.Bouquet
            {
                ShopId = 1,
                Name = "Sunflower Bouquet",
                Description = "A bright, sunny bouquet of fresh sunflowers paired with rustic dried pampas grass and soft greenery, wrapped in natural kraft paper and tied with twine. Cheerful and full of personality, it's a perfect pick-me-up for birthdays, thank-you gifts, or simply brightening someone's day.",
                Price = 1150,
                StockCount = 8,
                Category = FloraMix.Shared.Models.Occasion.Birthday,
                Rating = 4.6,
                SoldCount = 52,
                Status = FloraMix.Shared.Models.ProductStatus.Active,
                ImageUrl = "/images/products/bouquet_sunflower_yellow.png"
            },
            new FloraMix.Shared.Models.Bouquet
            {
                ShopId = 1,
                Name = "Dutch Tulip Bouquet",
                Description = "A fresh, imported mix of Dutch tulips in soft pastel tones, elegantly wrapped in delicate lace-style paper. With their smooth, graceful petals and refined color palette, these tulips are a wonderful choice for weddings, bridal showers, and other milestone celebrations that call for understated beauty.",
                Price = 2300,
                StockCount = 8,
                Category = FloraMix.Shared.Models.Occasion.Wedding,
                Rating = 4.8,
                SoldCount = 89,
                Status = FloraMix.Shared.Models.ProductStatus.Active,
                ImageUrl = "/images/products/bouquet_tulip_mixed.png"
            },
            new FloraMix.Shared.Models.Bouquet
            {
                ShopId = 1,
                Name = "Gerbera Daisy Bouquet (Mixed Colors)",
                Description = "A vibrant, colorful mix of gerbera daisies in cheerful pink, orange, and yellow hues, loosely arranged with kraft paper wrapping for a playful, feel-good look. Affordable and eye-catching, it's a favorite for birthdays, get-well wishes, or simply brightening someone's ordinary day.",
                Price = 950,
                StockCount = 8,
                Category = FloraMix.Shared.Models.Occasion.Birthday,
                Rating = 4.7,
                SoldCount = 93,
                Status = FloraMix.Shared.Models.ProductStatus.Active,
                ImageUrl = "/images/products/bouquet_gerbera_colorful.png"
            },
            new FloraMix.Shared.Models.Bouquet
            {
                ShopId = 1,
                Name = "White Lily Bouquet",
                Description = "A serene, all-white arrangement of fragrant lilies and roses accented with soft eucalyptus, designed to convey comfort and quiet respect. Simple, elegant, and gently fragrant, this bouquet is a thoughtful choice for sympathy, condolence, or any occasion calling for a peaceful gesture.",
                Price = 1700,
                StockCount = 8,
                Category = FloraMix.Shared.Models.Occasion.Sympathy,
                Rating = 4.9,
                SoldCount = 167,
                Status = FloraMix.Shared.Models.ProductStatus.Active,
                ImageUrl = "/images/products/bouquet_lily_white.png"
            }
        );

        var oliver = new FloraMix.Shared.Models.Order
        {
            ShopId = 1,
            CustomerName = "Oliver Park",
            CustomerEmail = "oliver@example.com",
            Status = FloraMix.Shared.Models.OrderStatus.Delivered,
            DeliveryLabel = "Delivered 10:45 AM",
            Occasion = FloraMix.Shared.Models.Occasion.Sympathy,
            CreatedAt = new DateTime(2026, 6, 3, 9, 0, 0),
            Items = new() { new FloraMix.Shared.Models.OrderItem { BouquetName = "Red Rose Bouquet with Lavender Fillers", Price = 1850, Quantity = 1 } }
        };
        var sophie = new FloraMix.Shared.Models.Order
        {
            ShopId = 1,
            CustomerName = "Sophie Chen",
            CustomerEmail = "sophie@example.com",
            Status = FloraMix.Shared.Models.OrderStatus.Ready,
            DeliveryLabel = "Today, 12–2 PM",
            Occasion = FloraMix.Shared.Models.Occasion.Wedding,
            CreatedAt = new DateTime(2026, 6, 3, 10, 15, 0),
            Items = new() { new FloraMix.Shared.Models.OrderItem { BouquetName = "Dutch Tulip Bouquet", Price = 2300, Quantity = 1 } }
        };
        var james = new FloraMix.Shared.Models.Order
        {
            ShopId = 1,
            CustomerName = "James Taylor",
            CustomerEmail = "james@example.com",
            Status = FloraMix.Shared.Models.OrderStatus.Pending,
            DeliveryLabel = "Today, 4–6 PM",
            Occasion = FloraMix.Shared.Models.Occasion.Birthday,
            CreatedAt = new DateTime(2026, 6, 3, 10, 45, 0),
            Items = new() { new FloraMix.Shared.Models.OrderItem { BouquetName = "Gerbera Daisy Bouquet (Mixed Colors)", Price = 950, Quantity = 1 } }
        };
        var emma = new FloraMix.Shared.Models.Order
        {
            ShopId = 1,
            CustomerName = "Emma Rose",
            CustomerEmail = "emma@example.com",
            Status = FloraMix.Shared.Models.OrderStatus.Preparing,
            DeliveryLabel = "Today, 2–4 PM",
            Occasion = FloraMix.Shared.Models.Occasion.Romance,
            CreatedAt = new DateTime(2026, 6, 3, 11, 30, 0),
            Items = new() { new FloraMix.Shared.Models.OrderItem { BouquetName = "Rose & Ferrero Rocher Combo", Price = 1599, Quantity = 1 } }
        };

        db.Orders.AddRange(oliver, sophie, james, emma);
        db.SaveChanges();

        // ---- Conversations (each tied to one order) ----
        var emmaConvo = new FloraMix.Shared.Models.Conversation { ShopId = 1, OrderId = emma.Id };
        var sophieConvo = new FloraMix.Shared.Models.Conversation { ShopId = 1, OrderId = sophie.Id };
        var jamesConvo = new FloraMix.Shared.Models.Conversation { ShopId = 1, OrderId = james.Id };

        db.Conversations.AddRange(emmaConvo, sophieConvo, jamesConvo);
        db.SaveChanges();

        db.Messages.AddRange(
            // Emma Rose thread
            new FloraMix.Shared.Models.Message { ConversationId = emmaConvo.Id, Sender = FloraMix.Shared.Models.MessageSender.Customer, Text = "Hi! I wanted to add a personalised card message to my order. Is that possible?", SentAt = new DateTime(2026, 6, 3, 10, 45, 0), IsRead = false },
            new FloraMix.Shared.Models.Message { ConversationId = emmaConvo.Id, Sender = FloraMix.Shared.Models.MessageSender.Shop, Text = "Of course! We'd be happy to add a personal touch. What would you like it to say? 🌸", SentAt = new DateTime(2026, 6, 3, 10, 48, 0), IsRead = true },
            new FloraMix.Shared.Models.Message { ConversationId = emmaConvo.Id, Sender = FloraMix.Shared.Models.MessageSender.Customer, Text = "\"Happy Birthday Mum, with all my love\" — is that okay?", SentAt = new DateTime(2026, 6, 3, 10, 51, 0), IsRead = false },
            new FloraMix.Shared.Models.Message { ConversationId = emmaConvo.Id, Sender = FloraMix.Shared.Models.MessageSender.Shop, Text = "Perfect, that's beautiful! We'll write it on our signature cream card. ✨", SentAt = new DateTime(2026, 6, 3, 10, 53, 0), IsRead = true },
            new FloraMix.Shared.Models.Message { ConversationId = emmaConvo.Id, Sender = FloraMix.Shared.Models.MessageSender.Customer, Text = "Can you also add an extra ribbon? I saw the satin one on your page.", SentAt = new DateTime(2026, 6, 3, 10, 56, 0), IsRead = true },
            new FloraMix.Shared.Models.Message { ConversationId = emmaConvo.Id, Sender = FloraMix.Shared.Models.MessageSender.Shop, Text = "Absolutely! We'll add the ivory satin ribbon at no extra charge since it's such a special occasion. 💕", SentAt = new DateTime(2026, 6, 3, 10, 58, 0), IsRead = true },
            new FloraMix.Shared.Models.Message { ConversationId = emmaConvo.Id, Sender = FloraMix.Shared.Models.MessageSender.Shop, Text = "Your order is being prepared right now and should be ready for delivery around 2 PM!", SentAt = new DateTime(2026, 6, 3, 11, 2, 0), IsRead = true },

            // Sophie Chen thread
            new FloraMix.Shared.Models.Message { ConversationId = sophieConvo.Id, Sender = FloraMix.Shared.Models.MessageSender.Customer, Text = "Thank you, they were gorgeous! 🌷", SentAt = new DateTime(2026, 6, 3, 10, 30, 0), IsRead = true },

            // James Taylor thread
            new FloraMix.Shared.Models.Message { ConversationId = jamesConvo.Id, Sender = FloraMix.Shared.Models.MessageSender.Customer, Text = "What time will the delivery arrive?", SentAt = new DateTime(2026, 6, 3, 9, 48, 0), IsRead = false }
        );

        db.SaveChanges();
    }
}

// ---- Minimal API for the MAUI customer app to read shop data ----
app.MapGet("/api/bouquets", async (IDbContextFactory<FloraMixDbContext> dbFactory) =>
{
    using var db = dbFactory.CreateDbContext();
    var bouquets = await db.Bouquets
        .Where(b => b.ShopId == 1)
        .Select(b => new
        {
            b.Id,
            b.Name,
            b.Description,
            b.Price,
            b.ImageUrl,
            Category = b.Category.ToString(),
            b.Rating,
            b.SoldCount,
            b.StockCount,
            Status = b.Status.ToString(),
            b.IsAvailable
        })
        .ToListAsync();

    return Results.Ok(bouquets);
});

// ---- Minimal API for the MAUI customer app to submit orders ----
app.MapPost("/api/orders", async (IDbContextFactory<FloraMixDbContext> dbFactory, OrderRequest request) =>
{
    using var db = dbFactory.CreateDbContext();

    var occasion = Enum.TryParse<FloraMix.Shared.Models.Occasion>(request.Occasion, true, out var occ)
        ? occ
        : FloraMix.Shared.Models.Occasion.Other;

    var order = new FloraMix.Shared.Models.Order
    {
        ShopId = 1,
        CustomerName = request.CustomerName,
        CustomerEmail = request.CustomerEmail,
        DeliveryLabel = request.DeliveryLabel,
        Status = FloraMix.Shared.Models.OrderStatus.Pending,
        Occasion = occasion,
        CreatedAt = DateTime.Now,
        Items = request.Items.Select(i => new FloraMix.Shared.Models.OrderItem
        {
            BouquetName = i.BouquetName,
            Price = i.Price,
            Quantity = i.Quantity
        }).ToList()
    };

    db.Orders.Add(order);
    await db.SaveChangesAsync();

    return Results.Ok(new { order.Id, order.OrderCode });
});

app.MapGet("/api/orders/{id:int}", async (IDbContextFactory<FloraMixDbContext> dbFactory, int id) =>
{
    using var db = dbFactory.CreateDbContext();

    var order = await db.Orders.FindAsync(id);
    if (order == null)
        return Results.NotFound();

    return Results.Ok(new
    {
        order.Id,
        order.OrderCode,
        Status = order.Status.ToString()
    });
});

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.UseStaticFiles();
app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddAdditionalAssemblies(
        typeof(FloraMix.Shared._Imports).Assembly);

app.Run();

record OrderRequest(string CustomerName, string CustomerEmail, string DeliveryLabel, string Occasion, List<OrderItemRequest> Items);
record OrderItemRequest(string BouquetName, decimal Price, int Quantity);