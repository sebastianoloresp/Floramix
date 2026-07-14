using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FloraMix.Models;

namespace FloraMix.Services
{
    public static class CartManager
    {
        public const double DeliveryFee = 100;

        public static DatabaseService? Db { get; set; }

        public static List<CartItem> Items { get; set; } = new List<CartItem>();

        public static List<SavedAddress> Addresses { get; set; } = new List<SavedAddress>
        {
            new SavedAddress { Label = "Home", AddressType = "Home", IsDefault = true, AddressLine = "Paseo de Sta. Rosa", ApartmentLine = "Blk 4 Lot 12", CityLine = "Sta. Rosa, Laguna", City = "Sta. Rosa", Postcode = "4026" },
            new SavedAddress { Label = "Work", AddressType = "Work", IsDefault = false, AddressLine = "Mapúa Malayan Colleges Laguna, Diversion Rd", ApartmentLine = "", CityLine = "Cabuyao, Laguna", City = "Cabuyao", Postcode = "4025" },
        };

        public static List<SavedCard> Cards { get; set; } = new List<SavedCard>
        {
            new SavedCard { Brand = "VISA", Last4 = "4291", ExpiryText = "09/28", CardHolder = "Emma Rose", IsSelected = true },
            new SavedCard { Brand = "AMEX", Last4 = "7834", ExpiryText = "03/27", CardHolder = "Emma Rose", IsSelected = false },
        };

        public static bool CashOnDeliveryEnabled { get; set; } = true;
        public static bool IsCashOnDeliverySelected { get; set; } = false;

        public static List<WishlistItem> Wishlist { get; set; } = new List<WishlistItem>();

        public static SavedAddress SelectedAddress { get; set; }
        public static DateTime SelectedDate { get; set; } = new DateTime(2026, 6, 3);
        public static string SelectedTimeSlot { get; set; } = "";
        public static string DeliveryNote { get; set; } = "";
        public static double DiscountPercent { get; set; } = 0;

        public static async Task InitializeAsync(DatabaseService db)
        {
            Db = db;

            var savedCart = await db.GetCartItemsAsync();
            if (savedCart.Count > 0)
                Items = savedCart;

            var savedWishlist = await db.GetWishlistAsync();
            if (savedWishlist.Count > 0)
                Wishlist = savedWishlist;

            var savedCards = await db.GetCardsAsync();
            if (savedCards.Count > 0)
                Cards = savedCards;

            var savedAddresses = await db.GetAddressesAsync();
            if (savedAddresses.Count > 0)
                Addresses = savedAddresses;
        }

        public static SavedCard GetSelectedCard()
        {
            foreach (var c in Cards)
                if (c.IsSelected) return c;
            return Cards.Count > 0 ? Cards[0] : null;
        }

        public static void AddItem(CartItem item) => Items.Add(item);

        public static async void AddItemAndSave(CartItem item)
        {
            Items.Add(item);
            if (Db != null)
                await Db.SaveCartItemAsync(item);
        }

        public static void RemoveItem(CartItem item) => RemoveItemAndDelete(item);

        public static async void RemoveItemAndDelete(CartItem item)
        {
            Items.Remove(item);
            if (Db != null)
                await Db.DeleteCartItemAsync(item);
        }

        public static async void SaveItem(CartItem item)
        {
            if (Db != null)
                await Db.SaveCartItemAsync(item);
        }

        public static async void ClearCart()
        {
            Items.Clear();
            if (Db != null)
                await Db.ClearCartAsync();
        }

        public static async void AddCardAndSave(SavedCard card)
        {
            Cards.Add(card);
            if (Db != null)
                await Db.SaveCardAsync(card);
        }

        public static async void RemoveCardAndDelete(SavedCard card)
        {
            Cards.Remove(card);
            if (Db != null)
                await Db.DeleteCardAsync(card);
        }

        public static async void AddAddressAndSave(SavedAddress address)
        {
            Addresses.Add(address);
            if (Db != null)
                await Db.SaveAddressAsync(address);
        }

        public static async void UpdateAddressAndSave(SavedAddress address)
        {
            if (Db != null)
                await Db.SaveAddressAsync(address);
        }

        public static async void RemoveAddressAndDelete(SavedAddress address)
        {
            Addresses.Remove(address);
            if (Db != null)
                await Db.DeleteAddressAsync(address);
        }

        public static double GetSubtotal()
        {
            double total = 0;
            foreach (var item in Items)
                total += item.Price * item.Quantity;
            return total;
        }

        public static async void SaveCardChange(SavedCard card)
        {
            if (Db != null)
                await Db.SaveCardAsync(card);
        }

        public static double GetDiscount() => GetSubtotal() * DiscountPercent;

        public static double GetTotal() => GetSubtotal() - GetDiscount() + DeliveryFee;
    }
}