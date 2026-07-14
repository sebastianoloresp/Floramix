using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FloraMix.Models;
using FloraMix.Services;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Graphics;
using FloraMix.Drawables;

namespace FloraMix.Views.Main
{
    public partial class BouquetBuilderPage : ContentPage
    {
        private int _currentStep = 0;
        private const int TotalSteps = 5;
        private const int MaxColorSelections = 4;

        private List<FlowerOption> _flowers;
        private List<ColorOption> _colors;
        private List<FillerOption> _fillers;
        private List<WrappingOption> _wrappings;
        private List<AddOnOption> _addOns;

        // Field added to hold the preview drawable instance
        private BouquetPreviewDrawable _previewDrawable;

        public BouquetBuilderPage() : this(null) { }

        public BouquetBuilderPage(string preselectFlowerName)
        {
            InitializeComponent();

            _flowers = new List<FlowerOption>
            {
                new FlowerOption { Name = "Rose", Emoji = "\U0001F339", PricePerStem = 150 },
                new FlowerOption { Name = "Carnation", Emoji = "\U0001F338", PricePerStem = 100 },
                new FlowerOption { Name = "Tulip", Emoji = "\U0001F337", PricePerStem = 120 },
                new FlowerOption { Name = "Lily", Emoji = "\U0001F490", PricePerStem = 150 },
                new FlowerOption { Name = "Sunflower", Emoji = "\U0001F33B", PricePerStem = 100 },
                new FlowerOption { Name = "Gerbera", Emoji = "\U0001F33C", PricePerStem = 100 },
            };

            _colors = new List<ColorOption>
            {
                new ColorOption { Name = "Blush Pink", HexColor = "#F2A6BC" },
                new ColorOption { Name = "Deep Red", HexColor = "#B5322C" },
                new ColorOption { Name = "Pure White", HexColor = "#F2F0EC" },
                new ColorOption { Name = "Sunshine Yellow", HexColor = "#F0C419" },
                new ColorOption { Name = "Lavender", HexColor = "#B7A6D9" },
                new ColorOption { Name = "Peach", HexColor = "#F2B380" },
                new ColorOption { Name = "Gold", HexColor = "#D4AF37" },
                new ColorOption { Name = "Sage Green", HexColor = "#9CAF88" },
                new ColorOption { Name = "Hot Pink", HexColor = "#FF69B4" },
                new ColorOption { Name = "Orange", HexColor = "#FF8C00" },
                new ColorOption { Name = "Ivory Cream", HexColor = "#F5F0E6" },
                new ColorOption { Name = "Rustic Brown", HexColor = "#8B5A2B" },
            };

            _fillers = new List<FillerOption>
            {
                new FillerOption { Name = "Baby's Breath", Emoji = "\U0001F4AE", Price = 5 },
                new FillerOption { Name = "Eucalyptus", Emoji = "\U0001F343", Price = 5 },
                new FillerOption { Name = "Waxflower", Emoji = "\U0001F33F", Price = 5 },
                new FillerOption { Name = "Lisianthus", Emoji = "\U0001F33A", Price = 5 },
                new FillerOption { Name = "Ruscus Leaves", Emoji = "\U0001F343", Price = 5 },
                new FillerOption { Name = "Dried Pampas", Emoji = "\U0001F33E", Price = 5 },
            };

            _wrappings = new List<WrappingOption>
            {
                new WrappingOption { Name = "Classic Paper", Description = "Kraft paper with twine", Price = 0, IsSelected = true },
                new WrappingOption { Name = "Satin Wrap", Description = "Premium satin ribbon", Price = 300 },
                new WrappingOption { Name = "Rustic Hessian", Description = "Natural burlap and lace", Price = 200 },
                new WrappingOption { Name = "Modern Box", Description = "Sleek gift box", Price = 450 },
            };

            _addOns = new List<AddOnOption>
            {
                new AddOnOption { Name = "Luxury Ribbon", Emoji = "\U0001F380", Price = 150 },
                new AddOnOption { Name = "Greeting Card", Emoji = "\u2764\uFE0F", Price = 100 },
                new AddOnOption { Name = "Chocolates", Emoji = "\U0001F36B", Price = 450 },
            };

            if (!string.IsNullOrEmpty(preselectFlowerName))
            {
                var match = _flowers.FirstOrDefault(f => f.Name == preselectFlowerName);
                if (match != null)
                {
                    match.IsSelected = true;
                    match.Stems = 6;
                }
            }

            // Wire up the drawable with option data definitions
            _previewDrawable = new BouquetPreviewDrawable
            {
                Flowers = _flowers,
                Colors = _colors,
                Fillers = _fillers,
                Wrappings = _wrappings,
                AddOns = _addOns
            };
            BouquetPreview.Drawable = _previewDrawable;

            BuildFlowerGrid();
            BuildColorGrid();
            BuildFillersList();
            BuildWrappingList();
            BuildAddOnsList();
            BuildStemsSection();
            UpdatePrice();
            UpdateStepUI();

            // Refresh on safety first load
            BouquetPreview.Invalidate();
        }

        // ---------- STEP 1: FLOWERS ----------
        private void BuildFlowerGrid()
        {
            FlowerGrid.Children.Clear();
            for (int i = 0; i < _flowers.Count; i++)
            {
                var flower = _flowers[i];
                var card = CreateFlowerCard(flower);
                Grid.SetColumn(card, i % 3);
                Grid.SetRow(card, i / 3);
                FlowerGrid.Children.Add(card);
            }
        }

        private Border CreateFlowerCard(FlowerOption flower)
        {
            var nameLabel = new Label { Text = flower.Name, FontAttributes = FontAttributes.Bold, FontSize = 14, TextColor = (Color)Application.Current.Resources["FloraCharcoal"], HorizontalOptions = LayoutOptions.Center };
            var priceLabel = new Label { Text = "\u20B1" + flower.PricePerStem + "/stem", FontSize = 11, TextColor = (Color)Application.Current.Resources["FloraMuted"], HorizontalOptions = LayoutOptions.Center };
            var emojiLabel = new Label { Text = flower.Emoji, FontSize = 22, HorizontalOptions = LayoutOptions.Center };

            var content = new VerticalStackLayout { Spacing = 4, Children = { emojiLabel, nameLabel, priceLabel } };

            var card = new Border
            {
                Padding = 12,
                StrokeShape = new RoundRectangle { CornerRadius = 16 },
                BackgroundColor = flower.IsSelected ? Color.FromArgb("#FDEEF1") : Colors.White,
                Stroke = flower.IsSelected ? (Color)Application.Current.Resources["FloraBlush"] : Colors.Transparent,
                StrokeThickness = flower.IsSelected ? 1.5 : 0,
                Content = content
            };

            var tap = new TapGestureRecognizer();
            tap.Tapped += (s, e) =>
            {
                flower.IsSelected = !flower.IsSelected;
                if (flower.IsSelected && flower.Stems == 0) flower.Stems = 1;
                BuildFlowerGrid();
                BuildStemsSection();
                UpdatePrice();
            };
            card.GestureRecognizers.Add(tap);

            return card;
        }

        private void BuildStemsSection()
        {
            var selected = _flowers.Where(f => f.IsSelected).ToList();
            StemsSection.IsVisible = selected.Count > 0;
            StemsStack.Children.Clear();

            foreach (var flower in selected)
            {
                var nameLabel = new Label { Text = flower.Emoji + " " + flower.Name, FontSize = 14, TextColor = (Color)Application.Current.Resources["FloraCharcoal"], VerticalOptions = LayoutOptions.Center };

                var minusLabel = new Label { Text = "-", FontAttributes = FontAttributes.Bold, FontSize = 16, WidthRequest = 30, HorizontalTextAlignment = TextAlignment.Center, TextColor = (Color)Application.Current.Resources["FloraCharcoal"] };
                var countLabel = new Label { Text = flower.Stems.ToString(), FontAttributes = FontAttributes.Bold, FontSize = 14, WidthRequest = 24, HorizontalTextAlignment = TextAlignment.Center, TextColor = (Color)Application.Current.Resources["FloraCharcoal"] };
                var plusLabel = new Label { Text = "+", FontAttributes = FontAttributes.Bold, FontSize = 16, WidthRequest = 30, HorizontalTextAlignment = TextAlignment.Center, TextColor = Colors.White };

                var minusBorder = new Border { BackgroundColor = (Color)Application.Current.Resources["FloraFieldBg"], StrokeShape = new RoundRectangle { CornerRadius = 15 }, WidthRequest = 30, HeightRequest = 30, Content = minusLabel };
                var plusBorder = new Border { BackgroundColor = (Color)Application.Current.Resources["FloraBlush"], StrokeShape = new RoundRectangle { CornerRadius = 15 }, WidthRequest = 30, HeightRequest = 30, Content = plusLabel };

                var minusTap = new TapGestureRecognizer();
                minusTap.Tapped += (s, e) =>
                {
                    if (flower.Stems > 1) flower.Stems--;
                    countLabel.Text = flower.Stems.ToString();
                    UpdatePrice();
                };
                minusBorder.GestureRecognizers.Add(minusTap);

                var plusTap = new TapGestureRecognizer();
                plusTap.Tapped += (s, e) =>
                {
                    flower.Stems++;
                    countLabel.Text = flower.Stems.ToString();
                    UpdatePrice();
                };
                plusBorder.GestureRecognizers.Add(plusTap);

                var row = new Grid { ColumnDefinitions = new ColumnDefinitionCollection { new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto) } };
                var stepper = new HorizontalStackLayout { Spacing = 10, Children = { minusBorder, countLabel, plusBorder } };
                row.Children.Add(nameLabel);
                Grid.SetColumn(nameLabel, 0);
                row.Children.Add(stepper);
                Grid.SetColumn(stepper, 1);

                var wrapper = new Border { BackgroundColor = Colors.White, StrokeShape = new RoundRectangle { CornerRadius = 14 }, Padding = 14, Content = row };
                StemsStack.Children.Add(wrapper);
            }
        }

        // ---------- STEP 2: COLORS ----------
        private void BuildColorGrid()
        {
            ColorGrid.Children.Clear();
            for (int i = 0; i < _colors.Count; i++)
            {
                var color = _colors[i];
                var card = CreateColorCard(color);
                Grid.SetColumn(card, i % 3);
                Grid.SetRow(card, i / 3);
                ColorGrid.Children.Add(card);
            }
        }

        private Border CreateColorCard(ColorOption color)
        {
            var swatch = new Border
            {
                HeightRequest = 44,
                WidthRequest = 44,
                StrokeShape = new RoundRectangle { CornerRadius = 22 },
                StrokeThickness = color.HexColor == "#F2F0EC" ? 1 : 0,
                Stroke = (Color)Application.Current.Resources["FloraFieldBg"],
                BackgroundColor = Color.FromArgb(color.HexColor)
            };

            Grid swatchGrid = new Grid { WidthRequest = 44, HeightRequest = 44, HorizontalOptions = LayoutOptions.Center };
            swatchGrid.Children.Add(swatch);

            if (color.IsSelected)
            {
                var check = new Label
                {
                    Text = "\uE73E",
                    FontFamily = "Segoe Fluent Icons",
                    FontSize = 11,
                    TextColor = Colors.White,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center
                };
                var checkBadge = new Border
                {
                    HeightRequest = 18,
                    WidthRequest = 18,
                    StrokeShape = new RoundRectangle { CornerRadius = 9 },
                    StrokeThickness = 2,
                    Stroke = Colors.White,
                    BackgroundColor = (Color)Application.Current.Resources["FloraBlush"],
                    HorizontalOptions = LayoutOptions.End,
                    VerticalOptions = LayoutOptions.End,
                    Content = check
                };
                swatchGrid.Children.Add(checkBadge);
            }

            var nameLabel = new Label { Text = color.Name, FontSize = 12, TextColor = (Color)Application.Current.Resources["FloraCharcoal"], HorizontalOptions = LayoutOptions.Center, HorizontalTextAlignment = TextAlignment.Center };

            var content = new VerticalStackLayout { Spacing = 8, HorizontalOptions = LayoutOptions.Center, Children = { swatchGrid, nameLabel } };

            var card = new Border
            {
                Padding = 12,
                StrokeShape = new RoundRectangle { CornerRadius = 16 },
                BackgroundColor = color.IsSelected ? Color.FromArgb("#FDEEF1") : Colors.White,
                Stroke = color.IsSelected ? (Color)Application.Current.Resources["FloraBlush"] : Colors.Transparent,
                StrokeThickness = color.IsSelected ? 1.5 : 0,
                Content = content
            };

            var tap = new TapGestureRecognizer();
            tap.Tapped += async (s, e) =>
            {
                if (color.IsSelected)
                {
                    color.IsSelected = false;
                    BuildColorGrid();
                }
                else
                {
                    int selectedCount = _colors.Count(c => c.IsSelected);
                    if (selectedCount >= MaxColorSelections)
                    {
                        await DisplayAlert("Limit Reached", $"You can choose up to {MaxColorSelections} colors. Deselect one first.", "OK");
                        return;
                    }
                    color.IsSelected = true;
                    BuildColorGrid();
                }
                BouquetPreview.Invalidate();
            };
            card.GestureRecognizers.Add(tap);

            return card;
        }

        // ---------- STEP 3: FILLERS ----------
        private void BuildFillersList()
        {
            FillersStack.Children.Clear();
            foreach (var filler in _fillers)
            {
                var emojiLabel = new Label { Text = filler.Emoji, FontSize = 20, VerticalOptions = LayoutOptions.Center };
                var nameLabel = new Label { Text = filler.Name, FontAttributes = FontAttributes.Bold, FontSize = 15, TextColor = (Color)Application.Current.Resources["FloraCharcoal"], VerticalOptions = LayoutOptions.Center };
                var priceLabel = new Label { Text = "+\u20B1" + filler.Price, FontAttributes = FontAttributes.Bold, FontSize = 14, TextColor = (Color)Application.Current.Resources["FloraCharcoal"], VerticalOptions = LayoutOptions.Center };

                var row = new Grid { ColumnDefinitions = new ColumnDefinitionCollection { new ColumnDefinition(GridLength.Auto), new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto) }, ColumnSpacing = 12 };
                row.Children.Add(emojiLabel); Grid.SetColumn(emojiLabel, 0);
                row.Children.Add(nameLabel); Grid.SetColumn(nameLabel, 1);
                row.Children.Add(priceLabel); Grid.SetColumn(priceLabel, 2);

                var card = new Border
                {
                    Padding = 16,
                    StrokeShape = new RoundRectangle { CornerRadius = 16 },
                    BackgroundColor = filler.IsSelected ? Color.FromArgb("#FDEEF1") : Colors.White,
                    Stroke = filler.IsSelected ? (Color)Application.Current.Resources["FloraBlush"] : Colors.Transparent,
                    StrokeThickness = filler.IsSelected ? 1.5 : 0,
                    Content = row
                };

                var tap = new TapGestureRecognizer();
                tap.Tapped += (s, e) =>
                {
                    filler.IsSelected = !filler.IsSelected;
                    BuildFillersList();
                    UpdatePrice();
                };
                card.GestureRecognizers.Add(tap);

                FillersStack.Children.Add(card);
            }
        }

        // ---------- STEP 4: WRAPPING ----------
        private void BuildWrappingList()
        {
            WrappingStack.Children.Clear();
            foreach (var wrap in _wrappings)
            {
                var nameLabel = new Label { Text = wrap.Name, FontAttributes = FontAttributes.Bold, FontSize = 15, TextColor = (Color)Application.Current.Resources["FloraCharcoal"] };
                var descLabel = new Label { Text = wrap.Description, FontSize = 12, TextColor = (Color)Application.Current.Resources["FloraMuted"] };
                var textStack = new VerticalStackLayout { Spacing = 2, Children = { nameLabel, descLabel } };

                string priceText = wrap.Price == 0 ? "Free" : "+\u20B1" + wrap.Price;
                var priceLabel = new Label { Text = priceText, FontAttributes = FontAttributes.Bold, FontSize = 14, TextColor = (Color)Application.Current.Resources["FloraCharcoal"], VerticalOptions = LayoutOptions.Center };

                var row = new Grid { ColumnDefinitions = new ColumnDefinitionCollection { new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto), new ColumnDefinition(GridLength.Auto) }, ColumnSpacing = 10 };
                row.Children.Add(textStack);
                Grid.SetColumn(textStack, 0);
                row.Children.Add(priceLabel);
                Grid.SetColumn(priceLabel, 1);

                if (wrap.IsSelected)
                {
                    var check = new Label { Text = "\uE73E", FontFamily = "Segoe Fluent Icons", FontSize = 12, TextColor = Colors.White, HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center };
                    var checkBorder = new Border { BackgroundColor = (Color)Application.Current.Resources["FloraBlush"], HeightRequest = 22, WidthRequest = 22, StrokeShape = new RoundRectangle { CornerRadius = 11 }, Content = check, VerticalOptions = LayoutOptions.Center };
                    row.Children.Add(checkBorder);
                    Grid.SetColumn(checkBorder, 2);
                }

                var card = new Border
                {
                    Padding = 16,
                    StrokeShape = new RoundRectangle { CornerRadius = 16 },
                    BackgroundColor = wrap.IsSelected ? Color.FromArgb("#FDEEF1") : Colors.White,
                    Stroke = wrap.IsSelected ? (Color)Application.Current.Resources["FloraBlush"] : Colors.Transparent,
                    StrokeThickness = wrap.IsSelected ? 1.5 : 0,
                    Content = row
                };

                var tap = new TapGestureRecognizer();
                tap.Tapped += (s, e) =>
                {
                    foreach (var w in _wrappings) w.IsSelected = false;
                    wrap.IsSelected = true;
                    BuildWrappingList();
                    UpdatePrice();
                };
                card.GestureRecognizers.Add(tap);

                WrappingStack.Children.Add(card);
            }
        }

        // ---------- STEP 5: ADD-ONS ----------
        private void BuildAddOnsList()
        {
            AddOnsStack.Children.Clear();
            foreach (var addOn in _addOns)
            {
                var emojiLabel = new Label { Text = addOn.Emoji, FontSize = 20, VerticalOptions = LayoutOptions.Center };
                var nameLabel = new Label { Text = addOn.Name, FontAttributes = FontAttributes.Bold, FontSize = 15, TextColor = (Color)Application.Current.Resources["FloraCharcoal"], VerticalOptions = LayoutOptions.Center };
                var priceLabel = new Label { Text = "+\u20B1" + addOn.Price, FontAttributes = FontAttributes.Bold, FontSize = 14, TextColor = (Color)Application.Current.Resources["FloraCharcoal"], VerticalOptions = LayoutOptions.Center };

                var row = new Grid { ColumnDefinitions = new ColumnDefinitionCollection { new ColumnDefinition(GridLength.Auto), new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto) }, ColumnSpacing = 12 };
                row.Children.Add(emojiLabel);
                Grid.SetColumn(emojiLabel, 0);
                row.Children.Add(nameLabel);
                Grid.SetColumn(nameLabel, 1);
                row.Children.Add(priceLabel);
                Grid.SetColumn(priceLabel, 2);

                var card = new Border
                {
                    Padding = 16,
                    StrokeShape = new RoundRectangle { CornerRadius = 16 },
                    BackgroundColor = addOn.IsSelected ? Color.FromArgb("#FDEEF1") : Colors.White,
                    Stroke = addOn.IsSelected ? (Color)Application.Current.Resources["FloraBlush"] : Colors.Transparent,
                    StrokeThickness = addOn.IsSelected ? 1.5 : 0,
                    Content = row
                };

                var tap = new TapGestureRecognizer();
                tap.Tapped += (s, e) =>
                {
                    addOn.IsSelected = !addOn.IsSelected;
                    BuildAddOnsList();
                    UpdatePrice();
                };
                card.GestureRecognizers.Add(tap);

                AddOnsStack.Children.Add(card);
            }
        }

        // ---------- PRICE ----------
        private void UpdatePrice()
        {
            PriceBadgeLabel.Text = "\u20B1" + GetTotalPrice().ToString("N0");
            BouquetPreview.Invalidate();
        }

        private double GetTotalPrice()
        {
            double total = 0;
            foreach (var f in _flowers.Where(x => x.IsSelected))
                total += f.PricePerStem * f.Stems;
            foreach (var fi in _fillers.Where(x => x.IsSelected))
                total += fi.Price;
            var wrap = _wrappings.FirstOrDefault(w => w.IsSelected);
            if (wrap != null) total += wrap.Price;
            foreach (var a in _addOns.Where(x => x.IsSelected))
                total += a.Price;
            return total;
        }

        // ---------- STEP NAVIGATION (0=Flowers, 1=Colors, 2=Fillers, 3=Wrapping, 4=Add-ons) ----------
        private void UpdateStepUI()
        {
            StepFlowers.IsVisible = _currentStep == 0;
            StepColors.IsVisible = _currentStep == 1;
            StepFillers.IsVisible = _currentStep == 2;
            StepWrapping.IsVisible = _currentStep == 3;
            StepAddOns.IsVisible = _currentStep == 4;

            BackButton.IsVisible = _currentStep > 0;

            var blush = (Color)Application.Current.Resources["FloraBlush"];
            var muted = (Color)Application.Current.Resources["FloraMuted"];
            var fieldBg = (Color)Application.Current.Resources["FloraFieldBg"];

            Segment0.BackgroundColor = _currentStep >= 0 ? blush : fieldBg;
            Segment1.BackgroundColor = _currentStep >= 1 ? blush : fieldBg;
            Segment2.BackgroundColor = _currentStep >= 2 ? blush : fieldBg;
            Segment3.BackgroundColor = _currentStep >= 3 ? blush : fieldBg;
            Segment4.BackgroundColor = _currentStep >= 4 ? blush : fieldBg;

            LabelStepFlowers.TextColor = _currentStep == 0 ? blush : muted;
            LabelStepFlowers.FontAttributes = _currentStep == 0 ? FontAttributes.Bold : FontAttributes.None;
            LabelStepColors.TextColor = _currentStep == 1 ? blush : muted;
            LabelStepColors.FontAttributes = _currentStep == 1 ? FontAttributes.Bold : FontAttributes.None;
            LabelStepFillers.TextColor = _currentStep == 2 ? blush : muted;
            LabelStepFillers.FontAttributes = _currentStep == 2 ? FontAttributes.Bold : FontAttributes.None;
            LabelStepWrapping.TextColor = _currentStep == 3 ? blush : muted;
            LabelStepWrapping.FontAttributes = _currentStep == 3 ? FontAttributes.Bold : FontAttributes.None;
            LabelStepAddOns.TextColor = _currentStep == 4 ? blush : muted;
            LabelStepAddOns.FontAttributes = _currentStep == 4 ? FontAttributes.Bold : FontAttributes.None;

            if (_currentStep == TotalSteps - 1)
                NextButtonLabel.Text = "Add to Cart - \u20B1" + GetTotalPrice().ToString("N0");
            else
                NextButtonLabel.Text = "Next";
        }

        private void OnBackTapped(object sender, EventArgs e)
        {
            if (_currentStep > 0)
            {
                _currentStep--;
                UpdateStepUI();
            }
        }

        private async void OnNextTapped(object sender, EventArgs e)
        {
            if (_currentStep < TotalSteps - 1)
            {
                _currentStep++;
                UpdateStepUI();
            }
            else
            {
                var selectedFlowers = _flowers.Where(f => f.IsSelected).ToList();
                if (selectedFlowers.Count == 0)
                {
                    await DisplayAlert("Almost there", "Please choose at least one flower before adding to cart.", "OK");
                    _currentStep = 0;
                    UpdateStepUI();
                    return;
                }

                double total = GetTotalPrice();
                string name = BuildCustomBouquetName(selectedFlowers);
                string colorTag = BuildColorTag();
                string imageSource = "build_icon.png";

                CartManager.AddItemAndSave(new CartItem
                {
                    ImageSource = imageSource,
                    Name = name,
                    Shop = "Wild Flowers",
                    Price = total,
                    Quantity = 1,
                    ColorTag = colorTag
                });

                await DisplayAlert("Added to Cart", $"Your custom bouquet (\u20B1{total:N0}) has been added to your cart.", "OK");
                await Navigation.PopAsync();
            }
        }

        private string BuildCustomBouquetName(List<FlowerOption> selectedFlowers)
        {
            var names = selectedFlowers.Select(f => f.Name).ToList();
            string flowerPart = names.Count == 1 ? names[0] : string.Join(" & ", names);
            return "Custom " + flowerPart + " Bouquet";
        }

        private string BuildColorTag()
        {
            var selectedColors = _colors.Where(c => c.IsSelected).Select(c => c.Name).ToList();
            return selectedColors.Count > 0 ? string.Join(", ", selectedColors) : "Custom Mix";
        }
    }
}