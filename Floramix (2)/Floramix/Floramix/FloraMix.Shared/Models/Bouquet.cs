namespace FloraMix.Shared.Models;

public enum ProductStatus { Active, LowStock, OutOfStock }

public class Bouquet
{
    public int Id { get; set; }
    public int ShopId { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsAvailable { get; set; } = true;
    public int StockCount { get; set; }

    public Occasion Category { get; set; } = Occasion.Other;
    public double Rating { get; set; } = 4.5;
    public int SoldCount { get; set; }
    public ProductStatus Status { get; set; } = ProductStatus.Active;
}