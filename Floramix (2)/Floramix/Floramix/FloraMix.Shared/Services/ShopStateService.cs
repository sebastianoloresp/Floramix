namespace FloraMix.Shared.Services;

/// <summary>
/// Lightweight notifier so components (like NavMenu) can react instantly
/// when shop data is updated elsewhere (like ShopSettings), without a
/// page navigation or full reload.
/// </summary>
public class ShopStateService
{
    public event Action? OnShopChanged;

    public void NotifyShopChanged() => OnShopChanged?.Invoke();
}