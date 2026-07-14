using System;
using System.Threading.Tasks;
using FloraMix.Models;
using FloraMix.Services;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Graphics;

namespace FloraMix.Views.Main
{
    public partial class ChatPage : ContentPage
    {
        private readonly Conversation _conversation;

        public ChatPage(Conversation conversation)
        {
            InitializeComponent();
            _conversation = conversation;

            ShopNameLabel.Text = conversation.ShopName;
            OnlineStatusLabel.Text = conversation.IsOnline ? "Online now" : "Offline";

            BuildMessages();
        }

        private void BuildMessages()
        {
            MessagesStack.Children.Clear();

            foreach (var msg in _conversation.Messages)
                MessagesStack.Children.Add(CreateMessageBubble(msg));
        }

        private View CreateMessageBubble(ChatMessage msg)
        {
            var textLabel = new Label
            {
                Text = msg.Text,
                FontSize = 14,
                TextColor = msg.IsFromUser ? Colors.White : (Color)Application.Current.Resources["FloraCharcoal"],
                LineBreakMode = LineBreakMode.WordWrap
            };

            var timeLabel = new Label
            {
                Text = msg.TimeLabel + (msg.IsFromUser ? " \u2713\u2713" : ""),
                FontSize = 10,
                TextColor = msg.IsFromUser ? Color.FromArgb("#DDFFFFFF") : (Color)Application.Current.Resources["FloraMuted"],
                HorizontalOptions = msg.IsFromUser ? LayoutOptions.End : LayoutOptions.Start
            };

            var bubbleContent = new VerticalStackLayout { Spacing = 4, Children = { textLabel, timeLabel } };

            var bubble = new Border
            {
                BackgroundColor = msg.IsFromUser ? (Color)Application.Current.Resources["FloraBlush"] : Colors.White,
                StrokeShape = new RoundRectangle { CornerRadius = 16 },
                StrokeThickness = 0,
                Padding = 14,
                MaximumWidthRequest = 260,
                Content = bubbleContent,
                HorizontalOptions = msg.IsFromUser ? LayoutOptions.End : LayoutOptions.Start
            };

            if (!msg.IsFromUser)
            {
                string initial = string.IsNullOrWhiteSpace(_conversation.ShopName) ? "W" : _conversation.ShopName.Substring(0, 1);

                var avatar = new Border
                {
                    HeightRequest = 28,
                    WidthRequest = 28,
                    StrokeShape = new RoundRectangle { CornerRadius = 14 },
                    StrokeThickness = 0,
                    BackgroundColor = (Color)Application.Current.Resources["FloraFieldBg"],
                    Content = new Label { Text = initial, FontSize = 12, FontAttributes = FontAttributes.Bold, TextColor = (Color)Application.Current.Resources["FloraCharcoal"], HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center },
                    VerticalOptions = LayoutOptions.Start
                };

                var row = new HorizontalStackLayout { Spacing = 8, HorizontalOptions = LayoutOptions.Start, Children = { avatar, bubble } };
                return row;
            }

            return bubble;
        }

        private async void OnSendTapped(object sender, EventArgs e)
        {
            string text = MessageEntry.Text;
            if (string.IsNullOrWhiteSpace(text)) return;

            var newMessage = new ChatMessage { Text = text, TimeLabel = DateTime.Now.ToString("h:mm tt"), IsFromUser = true };
            var messages = _conversation.Messages;
            messages.Add(newMessage);
            _conversation.Messages = messages;
            _conversation.LastMessage = newMessage.Text;
            _conversation.TimeLabel = newMessage.TimeLabel;
            MessagesStack.Children.Add(CreateMessageBubble(newMessage));
            MessageManager.SaveConversation(_conversation);

            MessageEntry.Text = string.Empty;

            await SendAutoReplyAsync();
        }

        private async Task SendAutoReplyAsync()
        {
            // Simulate the shop "typing" briefly before its automatic reply arrives
            await Task.Delay(1200);

            var reply = new ChatMessage
            {
                Text = "Thanks for your message! We've received it and one of our florists at Wild Flowers will get back to you shortly. 🌸",
                TimeLabel = DateTime.Now.ToString("h:mm tt"),
                IsFromUser = false
            };

            var messages = _conversation.Messages;
            messages.Add(reply);
            _conversation.Messages = messages;
            _conversation.LastMessage = reply.Text;
            _conversation.TimeLabel = reply.TimeLabel;
            MessagesStack.Children.Add(CreateMessageBubble(reply));
            MessageManager.SaveConversation(_conversation);
        }

        private async void OnBackTapped(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}