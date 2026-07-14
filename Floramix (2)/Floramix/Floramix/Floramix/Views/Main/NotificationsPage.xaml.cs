using System;
using System.Linq;
using FloraMix.Models;
using FloraMix.Services;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Graphics;

namespace FloraMix.Views.Main
{
    public partial class NotificationsPage : ContentPage
    {
        public NotificationsPage()
        {
            InitializeComponent();
            BuildNotifications();
        }

        private void BuildNotifications()
        {
            NotificationsStack.Children.Clear();

            int unread = NotificationManager.Notifications.Count(n => n.IsUnread);
            UnreadCountLabel.Text = $"{unread} unread";

            foreach (var note in NotificationManager.Notifications)
            {
                var capturedNote = note;

                Color iconBg = note.IconBackground switch
                {
                    "Pink" => Color.FromArgb("#FDEEF1"),
                    "Blue" => Color.FromArgb("#E6EEF7"),
                    "Yellow" => Color.FromArgb("#FBF0D9"),
                    _ => (Color)Application.Current.Resources["FloraFieldBg"]
                };
                Color iconColor = note.IconBackground switch
                {
                    "Pink" => (Color)Application.Current.Resources["FloraBlush"],
                    "Blue" => Color.FromArgb("#5B8DBE"),
                    "Yellow" => Color.FromArgb("#D9A441"),
                    _ => (Color)Application.Current.Resources["FloraCharcoal"]
                };

                var iconBadge = new Border
                {
                    HeightRequest = 40,
                    WidthRequest = 40,
                    StrokeShape = new RoundRectangle { CornerRadius = 20 },
                    StrokeThickness = 0,
                    BackgroundColor = iconBg,
                    Content = new Label { Text = note.IconGlyph, FontFamily = "Segoe Fluent Icons", FontSize = 16, TextColor = iconColor, HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center }
                };

                var titleLabel = new Label { Text = note.Title, FontAttributes = FontAttributes.Bold, FontSize = 14, TextColor = (Color)Application.Current.Resources["FloraCharcoal"] };
                var messageLabel = new Label { Text = note.Message, FontSize = 12, TextColor = (Color)Application.Current.Resources["FloraMuted"] };
                var timeLabel = new Label { Text = note.TimeAgo, FontSize = 11, TextColor = (Color)Application.Current.Resources["FloraMuted"], Margin = new Thickness(0, 2, 0, 0) };

                var textStack = new VerticalStackLayout { Spacing = 3, Children = { titleLabel, messageLabel, timeLabel } };

                var mainGrid = new Grid
                {
                    ColumnDefinitions = new ColumnDefinitionCollection
                    {
                        new ColumnDefinition(GridLength.Auto),
                        new ColumnDefinition(GridLength.Star),
                        new ColumnDefinition(GridLength.Auto)
                    },
                    ColumnSpacing = 12
                };
                mainGrid.Children.Add(iconBadge);
                Grid.SetColumn(iconBadge, 0);
                mainGrid.Children.Add(textStack);
                Grid.SetColumn(textStack, 1);

                var actionsStack = new HorizontalStackLayout { Spacing = 10, VerticalOptions = LayoutOptions.Start };

                if (note.IsUnread)
                {
                    var dot = new Border
                    {
                        HeightRequest = 8,
                        WidthRequest = 8,
                        StrokeShape = new RoundRectangle { CornerRadius = 4 },
                        StrokeThickness = 0,
                        BackgroundColor = (Color)Application.Current.Resources["FloraBlush"],
                        VerticalOptions = LayoutOptions.Center,
                        Margin = new Thickness(0, 4, 0, 0)
                    };
                    actionsStack.Children.Add(dot);
                }

                var deleteIcon = new Label
                {
                    Text = "\uE74D",
                    FontFamily = "Segoe Fluent Icons",
                    FontSize = 14,
                    TextColor = (Color)Application.Current.Resources["FloraMuted"],
                    VerticalOptions = LayoutOptions.Center
                };
                deleteIcon.GestureRecognizers.Add(new TapGestureRecognizer { Command = new Command(() => DeleteNotification(capturedNote)) });
                actionsStack.Children.Add(deleteIcon);

                mainGrid.Children.Add(actionsStack);
                Grid.SetColumn(actionsStack, 2);

                var card = new Border
                {
                    BackgroundColor = note.IsUnread ? Color.FromArgb("#FBEEF0") : Colors.White,
                    StrokeShape = new RoundRectangle { CornerRadius = 16 },
                    StrokeThickness = 0,
                    Padding = 14,
                    Content = mainGrid
                };
                card.GestureRecognizers.Add(new TapGestureRecognizer { Command = new Command(() => MarkAsRead(capturedNote)) });

                NotificationsStack.Children.Add(card);
            }
        }

        private void MarkAsRead(NotificationItem note)
        {
            if (note.IsUnread)
            {
                NotificationManager.MarkAsReadAndSave(note);
                BuildNotifications();
            }
        }

        private void DeleteNotification(NotificationItem note)
        {
            NotificationManager.DeleteNotificationAndSave(note);
            BuildNotifications();
        }

        private void OnMarkAllAsReadTapped(object sender, EventArgs e)
        {
            NotificationManager.MarkAllAsReadAndSave();
            BuildNotifications();
        }

        private async void OnBackTapped(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}