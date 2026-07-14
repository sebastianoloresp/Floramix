using System;
using System.Collections.ObjectModel;
using System.Linq;
using FloraMix.Models;
using FloraMix.Services;
using Microsoft.Maui.Controls;

namespace FloraMix.Views.Main
{
    public partial class HomePage : ContentPage
    {
        private string _activeCategory = "All";
        private static readonly Random _rng = new Random();
        private bool _dataLoaded = false;

        public ObservableCollection<Product> FilteredProducts { get; set; }

        public HomePage()
        {
            InitializeComponent();

            FilteredProducts = new ObservableCollection<Product>();
            ProductsCollectionView.ItemsSource = FilteredProducts;

            SetActivePill(PillAll, LabelAll);
            UpdateGreetingName();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (!_dataLoaded)
            {
                await ProductManager.Ready;
                _dataLoaded = true;
            }

            await ProductManager.RefreshFromApiAsync();
            ProductManager.SyncWishlistState();
            ApplyFilters();
            SetRandomBanner();

            UpdateGreetingName();
        }

        private void UpdateGreetingName()
        {
            string fullName = FloraMix.Services.ProfileManager.FullName;
            if (!string.IsNullOrWhiteSpace(fullName))
                GreetingNameLabel.Text = fullName.Split(' ')[0];
            else
                GreetingNameLabel.Text = "Seb";
        }

        private void SetRandomBanner()
        {
            if (ProductManager.AllProducts.Count == 0) return;

            var featured = ProductManager.AllProducts[_rng.Next(ProductManager.AllProducts.Count)];
            BannerImage.Source = featured.ImageSource;
            BannerTitleLabel.Text = featured.Name;
        }

        private void OnCategoryTapped(object sender, TappedEventArgs e)
        {
            _activeCategory = e.Parameter?.ToString() ?? "All";
            ApplyFilters();

            ResetAllPills();

            var (activePill, activeLabel) = _activeCategory switch
            {
                "Birthday" => (PillBirthday, LabelBirthday),
                "Wedding" => (PillWedding, LabelWedding),
                "Romance" => (PillRomance, LabelRomance),
                "Sympathy" => (PillSympathy, LabelSympathy),
                _ => (PillAll, LabelAll)
            };
            SetActivePill(activePill, activeLabel);
        }

        private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            ClearSearchIcon.IsVisible = !string.IsNullOrEmpty(e.NewTextValue);
            ApplyFilters();
        }

        private void OnClearSearchTapped(object sender, EventArgs e)
        {
            SearchEntry.Text = "";
        }

        private void ApplyFilters()
        {
            string searchText = SearchEntry.Text?.Trim() ?? "";

            var results = _activeCategory == "All"
                ? ProductManager.AllProducts.AsEnumerable()
                : ProductManager.AllProducts.Where(p => p.Category == _activeCategory);

            if (!string.IsNullOrEmpty(searchText))
            {
                results = results.Where(p =>
                    p.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                    p.Shop.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                    p.Tag.Contains(searchText, StringComparison.OrdinalIgnoreCase));
            }

            var filteredList = results.ToList();

            FilteredProducts.Clear();
            foreach (var product in filteredList)
                FilteredProducts.Add(product);

            NoResultsLabel.IsVisible = filteredList.Count == 0;
        }

        private void ResetAllPills()
        {
            foreach (var child in CategoryStack.Children)
            {
                if (child is Border border)
                {
                    border.BackgroundColor = Colors.White;
                    if (border.Content is Label label)
                        label.TextColor = (Color)Application.Current.Resources["FloraCharcoal"];
                }
            }
        }

        private void SetActivePill(Border pill, Label label)
        {
            pill.BackgroundColor = (Color)Application.Current.Resources["FloraBlush"];
            label.TextColor = Colors.White;
        }

        private void OnHeartTapped(object sender, TappedEventArgs e)
        {
            if (e.Parameter is Product product)
            {
                ProductManager.ToggleWishlist(product);
            }
        }

        private async void OnProductCartTapped(object sender, TappedEventArgs e)
        {
            if (e.Parameter is Product product)
                await Navigation.PushAsync(new ProductDetailPage(product));
        }

        private async void OnShopNowTapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AllProductsPage());
        }

        private async void OnSeeAllTapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AllProductsPage());
        }
    }
}