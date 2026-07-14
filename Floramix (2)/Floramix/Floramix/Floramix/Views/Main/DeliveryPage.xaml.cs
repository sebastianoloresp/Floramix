using System;
using System.Collections.Generic;
using FloraMix.Models;
using FloraMix.Services;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Graphics;

namespace FloraMix.Views.Main
{
    public partial class DeliveryPage : ContentPage
    {
        private static readonly DateTime FakeToday = new DateTime(2026, 6, 3);
        private DateTime _selectedDate;
        private string _selectedTimeSlot = "";
        private double _dateCardWidth = 64;
        private double _lastWidth = -1;

        private const double TargetCardWidth = 70;
        private const double CardSpacing = 10;
        private const double PagePadding = 48; // 24 left + 24 right

        private readonly List<(string Label, bool IsBooked)> _timeSlots = new List<(string, bool)>
        {
            ("9:00 - 11:00 AM", false),
            ("11:00 - 1:00 PM", false),
            ("1:00 - 3:00 PM", false),
            ("3:00 - 5:00 PM", false),
            ("5:00 - 7:00 PM", true),
        };

        public DeliveryPage()
        {
            InitializeComponent();
            _selectedDate = CartManager.SelectedDate != default ? CartManager.SelectedDate : FakeToday;
            _selectedTimeSlot = CartManager.SelectedTimeSlot ?? "";

            CheckoutStepHelper.BuildStepIndicator(StepIndicatorGrid, 2);
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            if (width <= 0 || Math.Abs(width - _lastWidth) < 0.5)
                return;

            _lastWidth = width;

            double available = width - PagePadding;

            // Work out how many whole cards fit per row at the target width,
            // then stretch/shrink each card so the row fills edge-to-edge with no partial card.
            int columns = Math.Max(1, (int)((available + CardSpacing) / (TargetCardWidth + CardSpacing)));
            double computed = (available - (CardSpacing * (columns - 1))) / columns;

            // Keep cards within a sensible size range regardless of device
            _dateCardWidth = Math.Max(56, Math.Min(computed, 90));

            BuildDateStack();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (CartManager.SelectedAddress == null && CartManager.Addresses.Count > 0)
                CartManager.SelectedAddress = CartManager.Addresses[0];

            DeliveryNoteEditor.Text = CartManager.DeliveryNote;

            BuildAddressList();
            BuildDateStack();
            BuildTimeSlotGrid();
        }

        private void BuildAddressList()
        {
            AddressStack.Children.Clear();

            foreach (var address in CartManager.Addresses)
            {
                bool isSelected = CartManager.SelectedAddress != null && CartManager.SelectedAddress.AddressLine == address.AddressLine;

                var labelRow = new HorizontalStackLayout { Spacing = 8 };
                labelRow.Children.Add(new Label { Text = address.Label, FontAttributes = FontAttributes.Bold, FontSize = 15, TextColor = (Color)Application.Current.Resources["FloraCharcoal"] });
                if (address.IsDefault)
                {
                    var badge = new Border { BackgroundColor = Color.FromArgb("#FDEEF1"), StrokeShape = new RoundRectangle { CornerRadius = 8 }, StrokeThickness = 0, Padding = new Thickness(8, 2), Content = new Label { Text = "Default", FontSize = 10, FontAttributes = FontAttributes.Bold, TextColor = (Color)Application.Current.Resources["FloraBlush"] } };
                    labelRow.Children.Add(badge);
                }

                var addressLine = new Label { Text = address.AddressLine, FontSize = 13, TextColor = (Color)Application.Current.Resources["FloraMuted"] };
                var cityLine = new Label { Text = address.CityLine, FontSize = 13, TextColor = (Color)Application.Current.Resources["FloraMuted"] };
                var textStack = new VerticalStackLayout { Spacing = 4, Children = { labelRow, addressLine, cityLine } };

                var radioOuter = new Border { HeightRequest = 20, WidthRequest = 20, StrokeShape = new RoundRectangle { CornerRadius = 10 }, Stroke = isSelected ? (Color)Application.Current.Resources["FloraBlush"] : (Color)Application.Current.Resources["FloraMuted"], StrokeThickness = 2, BackgroundColor = Colors.Transparent, VerticalOptions = LayoutOptions.Center };
                if (isSelected)
                {
                    radioOuter.Content = new Border { HeightRequest = 10, WidthRequest = 10, StrokeShape = new RoundRectangle { CornerRadius = 5 }, StrokeThickness = 0, BackgroundColor = (Color)Application.Current.Resources["FloraBlush"], HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center };
                }

                var row = new Grid { ColumnDefinitions = new ColumnDefinitionCollection { new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto) } };
                row.Children.Add(textStack); Grid.SetColumn(textStack, 0);
                row.Children.Add(radioOuter); Grid.SetColumn(radioOuter, 1);

                var card = new Border
                {
                    BackgroundColor = isSelected ? Color.FromArgb("#FDEEF1") : Colors.White,
                    Stroke = isSelected ? (Color)Application.Current.Resources["FloraBlush"] : Colors.Transparent,
                    StrokeThickness = isSelected ? 1.5 : 0,
                    StrokeShape = new RoundRectangle { CornerRadius = 16 },
                    Padding = 16,
                    Content = row
                };

                var tap = new TapGestureRecognizer();
                tap.Tapped += (s, e) => { CartManager.SelectedAddress = address; BuildAddressList(); };
                card.GestureRecognizers.Add(tap);

                AddressStack.Children.Add(card);
            }
        }

        private void BuildDateStack()
        {
            DateStack.Children.Clear();

            int daysInMonth = DateTime.DaysInMonth(FakeToday.Year, FakeToday.Month);
            int dayCount = daysInMonth - FakeToday.Day + 1;

            double cardHeight = _dateCardWidth * 1.22;
            double topFontSize = Math.Max(8, _dateCardWidth * 0.14);
            double dayFontSize = Math.Max(14, _dateCardWidth * 0.28);
            double monthFontSize = Math.Max(9, _dateCardWidth * 0.155);

            for (int i = 0; i < dayCount; i++)
            {
                DateTime date = FakeToday.AddDays(i);
                bool isSelected = date.Date == _selectedDate.Date;

                string topLabel = i == 0 ? "TODAY" : (i == 1 ? "TOMORROW" : date.ToString("ddd").ToUpper());
                string dayNumber = date.Day.ToString();
                string monthLabel = date.ToString("MMM");

                var topText = new Label { Text = topLabel, FontSize = topFontSize, FontAttributes = FontAttributes.Bold, TextColor = isSelected ? Colors.White : (Color)Application.Current.Resources["FloraBlush"], HorizontalOptions = LayoutOptions.Center, LineBreakMode = LineBreakMode.NoWrap };
                var dayText = new Label { Text = dayNumber, FontSize = dayFontSize, FontAttributes = FontAttributes.Bold, TextColor = isSelected ? Colors.White : (Color)Application.Current.Resources["FloraCharcoal"], HorizontalOptions = LayoutOptions.Center };
                var monthText = new Label { Text = monthLabel, FontSize = monthFontSize, TextColor = isSelected ? Colors.White : (Color)Application.Current.Resources["FloraMuted"], HorizontalOptions = LayoutOptions.Center };

                var contentStack = new VerticalStackLayout { Spacing = 2, HorizontalOptions = LayoutOptions.Center, Children = { topText, dayText, monthText } };

                var card = new Border
                {
                    WidthRequest = _dateCardWidth,
                    HeightRequest = cardHeight,
                    Margin = new Thickness(0, 0, 10, 10),
                    BackgroundColor = isSelected ? (Color)Application.Current.Resources["FloraBlush"] : Colors.White,
                    StrokeShape = new RoundRectangle { CornerRadius = 14 },
                    StrokeThickness = 0,
                    Content = contentStack
                };

                DateTime capturedDate = date;
                var tap = new TapGestureRecognizer();
                tap.Tapped += (s, e) => { _selectedDate = capturedDate; CartManager.SelectedDate = capturedDate; BuildDateStack(); };
                card.GestureRecognizers.Add(tap);

                DateStack.Children.Add(card);
            }
        }

        private void BuildTimeSlotGrid()
        {
            TimeSlotGrid.Children.Clear();
            TimeSlotGrid.RowDefinitions.Clear();

            int rows = (int)Math.Ceiling(_timeSlots.Count / 2.0);
            for (int r = 0; r < rows; r++)
                TimeSlotGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));

            for (int i = 0; i < _timeSlots.Count; i++)
            {
                var slot = _timeSlots[i];
                bool isSelected = _selectedTimeSlot == slot.Label;

                Label label;
                Border card;

                if (slot.IsBooked)
                {
                    label = new Label { Text = slot.Label, FontSize = 13, TextColor = (Color)Application.Current.Resources["FloraMuted"], HorizontalOptions = LayoutOptions.Center };
                    var subLabel = new Label { Text = "Fully booked", FontSize = 10, TextColor = (Color)Application.Current.Resources["FloraMuted"], HorizontalOptions = LayoutOptions.Center };
                    var stack = new VerticalStackLayout { Spacing = 2, Children = { label, subLabel } };
                    card = new Border { BackgroundColor = (Color)Application.Current.Resources["FloraFieldBg"], StrokeShape = new RoundRectangle { CornerRadius = 12 }, StrokeThickness = 0, Padding = 14, Content = stack, Opacity = 0.6 };
                }
                else
                {
                    label = new Label { Text = slot.Label, FontSize = 13, FontAttributes = isSelected ? FontAttributes.Bold : FontAttributes.None, TextColor = isSelected ? (Color)Application.Current.Resources["FloraBlush"] : (Color)Application.Current.Resources["FloraCharcoal"], HorizontalOptions = LayoutOptions.Center };
                    card = new Border
                    {
                        BackgroundColor = Colors.White,
                        Stroke = isSelected ? (Color)Application.Current.Resources["FloraBlush"] : Color.FromArgb("#EFECE6"),
                        StrokeThickness = isSelected ? 1.5 : 1,
                        StrokeShape = new RoundRectangle { CornerRadius = 12 },
                        Padding = 14,
                        Content = label
                    };

                    string capturedSlot = slot.Label;
                    var tap = new TapGestureRecognizer();
                    tap.Tapped += (s, e) => { _selectedTimeSlot = capturedSlot; CartManager.SelectedTimeSlot = capturedSlot; BuildTimeSlotGrid(); };
                    card.GestureRecognizers.Add(tap);
                }

                Grid.SetRow(card, i / 2);
                Grid.SetColumn(card, i % 2);
                TimeSlotGrid.Children.Add(card);
            }
        }

        private async void OnAddAddressTapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AddAddressPage());
        }

        private async void OnContinueTapped(object sender, EventArgs e)
        {
            if (CartManager.SelectedAddress == null)
            {
                await DisplayAlert("Select an Address", "Please choose a delivery address.", "OK");
                return;
            }

            if (string.IsNullOrEmpty(_selectedTimeSlot))
            {
                await DisplayAlert("Select a Time", "Please choose a delivery time slot.", "OK");
                return;
            }

            CartManager.DeliveryNote = DeliveryNoteEditor.Text ?? "";
            await Navigation.PushAsync(new PaymentPage());
        }

        private async void OnBackTapped(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}