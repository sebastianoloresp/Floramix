namespace FloraMix.Shared.Models;

public class Shop
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Address { get; set; } = "";
    public string Location { get; set; } = "";
    public string Hours { get; set; } = "";
    public string OwnerUserId { get; set; } = "";

    public string Email { get; set; } = "";
    public string Phone { get; set; } = "";
    public string BusinessHoursJson { get; set; } = "";

    public bool NotifyNewOrders { get; set; } = true;
    public bool NotifyCustomerMessages { get; set; } = true;
    public bool NotifyLowStock { get; set; } = true;
    public bool NotifyDailyRevenue { get; set; } = false;
    public bool NotifyWeeklyAnalytics { get; set; } = false;
}