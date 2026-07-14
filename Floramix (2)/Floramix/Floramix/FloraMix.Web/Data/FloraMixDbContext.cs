using Microsoft.EntityFrameworkCore;
using FloraMix.Shared.Models;

namespace FloraMix.Web.Data;

public class FloraMixDbContext : DbContext
{
    public FloraMixDbContext(DbContextOptions<FloraMixDbContext> options)
        : base(options)
    {
    }

    public DbSet<Shop> Shops => Set<Shop>();
    public DbSet<Bouquet> Bouquets => Set<Bouquet>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<Conversation> Conversations => Set<Conversation>();
    public DbSet<Message> Messages => Set<Message>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>()
            .HasMany(o => o.Items)
            .WithOne()
            .HasForeignKey(i => i.OrderId);

        modelBuilder.Entity<Bouquet>()
            .Property(b => b.Price)
            .HasPrecision(10, 2);

        modelBuilder.Entity<OrderItem>()
            .Property(i => i.Price)
            .HasPrecision(10, 2);

        modelBuilder.Entity<Conversation>()
            .HasOne(c => c.Order)
            .WithMany()
            .HasForeignKey(c => c.OrderId);

        modelBuilder.Entity<Conversation>()
            .HasMany(c => c.Messages)
            .WithOne()
            .HasForeignKey(m => m.ConversationId);

        base.OnModelCreating(modelBuilder);
    }
}