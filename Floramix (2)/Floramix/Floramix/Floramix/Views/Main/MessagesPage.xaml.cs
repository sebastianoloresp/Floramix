using System;
using FloraMix.Models;
using FloraMix.Services;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Graphics;

namespace FloraMix.Views.Main
{
    public partial class MessagesPage : ContentPage
    {
        public MessagesPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            BuildConversationsList();
        }

        private void BuildConversationsList()
        {
            ConversationsStack.Children.Clear();

            foreach (var convo in MessageManager.Conversations)
            {
                var image = new Image { Source = convo.AvatarSource, Aspect = Aspect.AspectFill, HeightRequest = 52, WidthRequest = 52 };
                var imageWrapper = new Border { StrokeShape = new RoundRectangle { CornerRadius = 26 }, StrokeThickness = 0, Content = image, HeightRequest = 52, WidthRequest = 52 };

                var onlineDot = new Border { HeightRequest = 10, WidthRequest = 10, StrokeShape = new RoundRectangle { CornerRadius = 5 }, StrokeThickness = 1.5, Stroke = Colors.White, BackgroundColor = Color.FromArgb("#5B8C5A"), HorizontalOptions = LayoutOptions.End, VerticalOptions = LayoutOptions.End };

                var avatarStack = new Grid { WidthRequest = 52, HeightRequest = 52 };
                avatarStack.Children.Add(imageWrapper);
                if (convo.IsOnline) avatarStack.Children.Add(onlineDot);

                var nameLabel = new Label { Text = convo.ShopName, FontAttributes = FontAttributes.Bold, FontSize = 15, TextColor = (Color)Application.Current.Resources["FloraCharcoal"] };
                var messageLabel = new Label { Text = convo.LastMessage, FontSize = 13, TextColor = (Color)Application.Current.Resources["FloraMuted"], LineBreakMode = LineBreakMode.TailTruncation };
                var textStack = new VerticalStackLayout { Spacing = 2, Children = { nameLabel, messageLabel } };

                var timeLabel = new Label { Text = convo.TimeLabel, FontSize = 11, TextColor = (Color)Application.Current.Resources["FloraMuted"], HorizontalOptions = LayoutOptions.End };

                var rightStack = new VerticalStackLayout { Spacing = 6, HorizontalOptions = LayoutOptions.End, Children = { timeLabel } };
                if (convo.UnreadCount > 0)
                {
                    var badge = new Border
                    {
                        HeightRequest = 20,
                        WidthRequest = 20,
                        StrokeShape = new RoundRectangle { CornerRadius = 10 },
                        StrokeThickness = 0,
                        BackgroundColor = (Color)Application.Current.Resources["FloraBlush"],
                        HorizontalOptions = LayoutOptions.End,
                        Content = new Label { Text = convo.UnreadCount.ToString(), FontSize = 11, FontAttributes = FontAttributes.Bold, TextColor = Colors.White, HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center }
                    };
                    rightStack.Children.Add(badge);
                }

                var row = new Grid { ColumnDefinitions = new ColumnDefinitionCollection { new ColumnDefinition(GridLength.Auto), new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto) }, ColumnSpacing = 12 };
                row.Children.Add(avatarStack); Grid.SetColumn(avatarStack, 0);
                row.Children.Add(textStack); Grid.SetColumn(textStack, 1);
                row.Children.Add(rightStack); Grid.SetColumn(rightStack, 2);

                var card = new Border { BackgroundColor = Colors.White, StrokeShape = new RoundRectangle { CornerRadius = 16 }, StrokeThickness = 0, Padding = 14, Content = row };

                var convoCaptured = convo;
                var tap = new TapGestureRecognizer();
                tap.Tapped += async (s, e) =>
                {
                    MessageManager.MarkAsReadAndSave(convoCaptured);
                    await Navigation.PushAsync(new ChatPage(convoCaptured));
                };
                card.GestureRecognizers.Add(tap);

                ConversationsStack.Children.Add(card);
            }
        }
    }
}