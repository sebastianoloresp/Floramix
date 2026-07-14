using System;
using FloraMix.Services;
using Microsoft.Maui.Controls;

namespace FloraMix.Views.Main
{
    public partial class AllProductsPage : ContentPage
    {
        private bool _dataLoaded = false;

        public AllProductsPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (!_dataLoaded)
            {
                await ProductManager.Ready;
                _dataLoaded = true;
            }

            ProductManager.SyncWishlistState();
            AllProductsCollectionView.ItemsSource = ProductManager.AllProducts;
        }

        private void OnHeartTapped(object sender, TappedEventArgs e)
        {
            if (e.Parameter is FloraMix.Models.Product product)
                ProductManager.ToggleWishlist(product);
        }

        private async void OnProductCartTapped(object sender, TappedEventArgs e)
        {
            if (e.Parameter is FloraMix.Models.Product product)
                await Navigation.PushAsync(new ProductDetailPage(product));
        }

        private async void OnBackTapped(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}