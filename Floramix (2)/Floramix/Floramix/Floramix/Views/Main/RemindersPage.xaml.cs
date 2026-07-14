using System;
using System.Collections.Generic;
using System.Linq;
using FloraMix.Models;
using FloraMix.Services;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Graphics;

namespace FloraMix.Views.Main
{
    public partial class RemindersPage : ContentPage
    {
        private int _currentMonth;
        private int _currentYear;
        private string _selectedType = "birthday";

        private static readonly string[] MonthNames = new string[]
        {
            "January", "February", "March", "April", "May", "June",
            "July", "August", "September", "October", "November", "December"
        };

        public RemindersPage()
        {
            InitializeComponent();

            _currentMonth = 6;
            _currentYear = 2026;

            BuildTypeSelector();
            RenderCalendar();
            RenderEventsList();
        }

        // ---------- CALENDAR ----------
        private void RenderCalendar()
        {
            MonthYearLabel.Text = MonthNames[_currentMonth - 1] + " " + _currentYear;
            DayFieldLabel.Text = "Day (" + MonthNames[_currentMonth - 1] + ")";

            CalendarGrid.Children.Clear();
            CalendarGrid.RowDefinitions.Clear();

            DateTime firstOfMonth = new DateTime(_currentYear, _currentMonth, 1);
            int startOffset = (int)firstOfMonth.DayOfWeek; // 0 = Sunday
            int daysInMonth = DateTime.DaysInMonth(_currentYear, _currentMonth);

            int totalCells = startOffset + daysInMonth;
            int rows = (int)Math.Ceiling(totalCells / 7.0);

            for (int r = 0; r < rows; r++)
                CalendarGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));

            for (int i = 0; i < totalCells; i++)
            {
                int dayNumber = i - startOffset + 1;
                int row = i / 7;
                int col = i % 7;

                if (dayNumber < 1 || dayNumber > daysInMonth)
                    continue;

                var cell = CreateDayCell(dayNumber);
                Grid.SetRow(cell, row);
                Grid.SetColumn(cell, col);
                CalendarGrid.Children.Add(cell);
            }
        }

        private Border CreateDayCell(int day)
        {
            bool isToday = day == 3 && _currentMonth == 6 && _currentYear == 2026;

            var dayEvents = ReminderManager.Events.Where(e => e.Day == day && e.Month == _currentMonth && e.Year == _currentYear).ToList();
            bool hasEvent = dayEvents.Count > 0;

            Color backgroundColor;
            Color textColor;

            if (isToday)
            {
                backgroundColor = (Color)Application.Current.Resources["FloraBlush"];
                textColor = Colors.White;
            }
            else if (hasEvent)
            {
                backgroundColor = GetLightColorForType(dayEvents[0].Type);
                textColor = GetColorForType(dayEvents[0].Type);
            }
            else
            {
                backgroundColor = Colors.Transparent;
                textColor = (Color)Application.Current.Resources["FloraCharcoal"];
            }

            var dayLabel = new Label
            {
                Text = day.ToString(),
                FontSize = 14,
                FontAttributes = (isToday || hasEvent) ? FontAttributes.Bold : FontAttributes.None,
                TextColor = textColor,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center
            };

            var circle = new Border
            {
                HeightRequest = 36,
                WidthRequest = 36,
                StrokeShape = new RoundRectangle { CornerRadius = 18 },
                StrokeThickness = 0,
                BackgroundColor = backgroundColor,
                Content = dayLabel,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                Padding = 0
            };

            var dot = new Border
            {
                HeightRequest = 4,
                WidthRequest = 4,
                StrokeShape = new RoundRectangle { CornerRadius = 2 },
                StrokeThickness = 0,
                BackgroundColor = hasEvent ? GetColorForType(dayEvents[0].Type) : Colors.Transparent,
                HorizontalOptions = LayoutOptions.Center
            };

            var cellStack = new VerticalStackLayout
            {
                Spacing = 3,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                Children = { circle, dot }
            };

            var wrapper = new Border
            {
                HeightRequest = 52,
                WidthRequest = 52,
                BackgroundColor = Colors.Transparent,
                StrokeThickness = 0,
                Content = cellStack,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };

            return wrapper;
        }

        private Color GetColorForType(string type)
        {
            if (type == "birthday") return (Color)Application.Current.Resources["FloraBlush"];
            if (type == "anniversary") return Color.FromArgb("#A8C5A0");
            if (type == "holiday") return Color.FromArgb("#C08552");
            return Color.FromArgb("#9B8AA8"); // other
        }

        private Color GetLightColorForType(string type)
        {
            if (type == "birthday") return Color.FromArgb("#FDEEF1");
            if (type == "anniversary") return Color.FromArgb("#EAF4EA");
            if (type == "holiday") return Color.FromArgb("#F5EAE0");
            return Color.FromArgb("#EFEAF2"); // other
        }

        private string GetEmojiForType(string type)
        {
            if (type == "birthday") return "\U0001F382";
            if (type == "anniversary") return "\U0001F48D";
            if (type == "holiday") return "\U0001F343";
            return "\U0001F4CC"; // other - pushpin
        }

        private void OnPrevMonthTapped(object sender, EventArgs e)
        {
            _currentMonth--;
            if (_currentMonth < 1)
            {
                _currentMonth = 12;
                _currentYear--;
            }
            RenderCalendar();
            RenderEventsList();
        }

        private void OnNextMonthTapped(object sender, EventArgs e)
        {
            _currentMonth++;
            if (_currentMonth > 12)
            {
                _currentMonth = 1;
                _currentYear++;
            }
            RenderCalendar();
            RenderEventsList();
        }

        // ---------- EVENTS LIST ----------
        private void RenderEventsList()
        {
            MonthSectionLabel.Text = MonthNames[_currentMonth - 1];
            EventsStack.Children.Clear();

            var monthEvents = ReminderManager.Events
                .Where(e => e.Month == _currentMonth && e.Year == _currentYear)
                .OrderBy(e => e.Day)
                .ToList();

            foreach (var ev in monthEvents)
            {
                EventsStack.Children.Add(CreateEventRow(ev));
            }

            if (monthEvents.Count == 0)
            {
                EventsStack.Children.Add(new Label
                {
                    Text = "No events this month.",
                    FontSize = 13,
                    TextColor = (Color)Application.Current.Resources["FloraMuted"]
                });
            }
        }

        private Border CreateEventRow(ReminderEvent ev)
        {
            var dayBadge = new Border
            {
                BackgroundColor = GetLightColorForType(ev.Type),
                HeightRequest = 44,
                WidthRequest = 44,
                StrokeShape = new RoundRectangle { CornerRadius = 22 },
                StrokeThickness = 0,
                Content = new Label
                {
                    Text = ev.Day.ToString(),
                    FontAttributes = FontAttributes.Bold,
                    FontSize = 15,
                    TextColor = GetColorForType(ev.Type),
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center
                }
            };

            var nameLabel = new Label { Text = ev.Name, FontAttributes = FontAttributes.Bold, FontSize = 15, TextColor = (Color)Application.Current.Resources["FloraCharcoal"] };
            var subLabel = new Label { Text = ev.PersonOrLabel, FontSize = 12, TextColor = (Color)Application.Current.Resources["FloraMuted"] };
            var textStack = new VerticalStackLayout { Spacing = 2, Children = { nameLabel, subLabel } };

            var emojiLabel = new Label { Text = GetEmojiForType(ev.Type), FontSize = 20, VerticalOptions = LayoutOptions.Center };

            var deleteLabel = new Label
            {
                Text = "\uE74D", // Segoe Fluent Icons: Delete
                FontFamily = "Segoe Fluent Icons",
                FontSize = 16,
                TextColor = (Color)Application.Current.Resources["FloraMuted"],
                VerticalOptions = LayoutOptions.Center,
                Padding = new Thickness(8)
            };
            var deleteTap = new TapGestureRecognizer();
            deleteTap.Tapped += (s, e) => OnDeleteEventTapped(ev);
            deleteLabel.GestureRecognizers.Add(deleteTap);

            var row = new Grid
            {
                ColumnDefinitions = new ColumnDefinitionCollection
                {
                    new ColumnDefinition(GridLength.Auto),
                    new ColumnDefinition(GridLength.Star),
                    new ColumnDefinition(GridLength.Auto),
                    new ColumnDefinition(GridLength.Auto)
                },
                ColumnSpacing = 12
            };
            row.Children.Add(dayBadge);
            Grid.SetColumn(dayBadge, 0);
            row.Children.Add(textStack);
            Grid.SetColumn(textStack, 1);
            row.Children.Add(emojiLabel);
            Grid.SetColumn(emojiLabel, 2);
            row.Children.Add(deleteLabel);
            Grid.SetColumn(deleteLabel, 3);

            return new Border
            {
                BackgroundColor = Colors.White,
                StrokeShape = new RoundRectangle { CornerRadius = 16 },
                StrokeThickness = 0,
                Padding = 14,
                Content = row
            };
        }

        private async void OnDeleteEventTapped(ReminderEvent ev)
        {
            bool confirm = await DisplayAlert("Delete Reminder", $"Delete \"{ev.Name}\"?", "Delete", "Cancel");
            if (!confirm)
                return;

            ReminderManager.RemoveEvent(ev);
            RenderCalendar();
            RenderEventsList();
        }

        // ---------- ADD REMINDER MODAL ----------
        private void BuildTypeSelector()
        {
            TypeSelectorStack.Children.Clear();

            var types = new List<(string Key, string Emoji, string Label)>
            {
                ("birthday", "\U0001F382", "birthday"),
                ("anniversary", "\U0001F48D", "anniversary"),
                ("holiday", "\U0001F343", "holiday"),
                ("other", "\U0001F4CC", "other")
            };

            foreach (var t in types)
            {
                bool isSelected = t.Key == _selectedType;

                var label = new Label
                {
                    Text = t.Emoji + " " + t.Label,
                    FontSize = 13,
                    FontAttributes = FontAttributes.Bold,
                    TextColor = isSelected ? Colors.White : (Color)Application.Current.Resources["FloraCharcoal"]
                };

                var pill = new Border
                {
                    BackgroundColor = isSelected ? GetColorForType(t.Key) : Colors.White,
                    StrokeShape = new RoundRectangle { CornerRadius = 18 },
                    StrokeThickness = 0,
                    Padding = new Thickness(16, 8),
                    Margin = new Thickness(0, 0, 8, 8),
                    Content = label
                };

                string key = t.Key;
                var tap = new TapGestureRecognizer();
                tap.Tapped += (s, e) =>
                {
                    _selectedType = key;
                    BuildTypeSelector();
                };
                pill.GestureRecognizers.Add(tap);

                TypeSelectorStack.Children.Add(pill);
            }
        }

        private void OnAddReminderTapped(object sender, EventArgs e)
        {
            EventNameEntry.Text = string.Empty;
            DayEntry.Text = string.Empty;
            _selectedType = "birthday";
            BuildTypeSelector();
            AddReminderOverlay.IsVisible = true;
        }

        private void OnCloseAddReminderTapped(object sender, EventArgs e)
        {
            AddReminderOverlay.IsVisible = false;
        }

        private async void OnSaveReminderTapped(object sender, EventArgs e)
        {
            string name = EventNameEntry.Text;
            string dayText = DayEntry.Text;

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(dayText))
            {
                await DisplayAlert("Missing Info", "Please enter an event name and day.", "OK");
                return;
            }

            int day;
            bool parsed = int.TryParse(dayText, out day);

            if (!parsed || day < 1 || day > DateTime.DaysInMonth(_currentYear, _currentMonth))
            {
                await DisplayAlert("Invalid Day", "Please enter a valid day for this month.", "OK");
                return;
            }

            var newEvent = new ReminderEvent
            {
                Name = name,
                PersonOrLabel = name,
                Type = _selectedType,
                Day = day,
                Month = _currentMonth,
                Year = _currentYear
            };

            ReminderManager.AddEvent(newEvent);

            AddReminderOverlay.IsVisible = false;
            RenderCalendar();
            RenderEventsList();
        }
    }
}