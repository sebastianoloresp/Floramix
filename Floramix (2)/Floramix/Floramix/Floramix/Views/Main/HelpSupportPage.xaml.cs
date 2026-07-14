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
    public partial class HelpSupportPage : ContentPage
    {
        private static readonly Random _rng = new Random();

        public HelpSupportPage()
        {
            InitializeComponent();
            BuildFaqSections(null);
        }

        private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            ClearSearchIcon.IsVisible = !string.IsNullOrEmpty(e.NewTextValue);
            BuildFaqSections(e.NewTextValue);
        }

        private void OnClearSearchTapped(object sender, EventArgs e)
        {
            SearchEntry.Text = "";
        }

        private void BuildFaqSections(string searchText)
        {
            FaqSectionsStack.Children.Clear();

            bool isSearching = !string.IsNullOrWhiteSpace(searchText);

            if (isSearching)
            {
                QuickActionsGrid.Margin = new Thickness(0);
                var matches = HelpManager.Faqs
                    .Where(f => f.Question.Contains(searchText, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                var flatList = new VerticalStackLayout { Spacing = 10 };
                foreach (var faq in matches)
                    flatList.Children.Add(BuildFaqRow(faq));

                if (matches.Count == 0)
                {
                    flatList.Children.Add(new Label
                    {
                        Text = "No matching questions found. Try a different search or submit a ticket below.",
                        FontSize = 13,
                        TextColor = (Color)Application.Current.Resources["FloraMuted"],
                        LineBreakMode = LineBreakMode.WordWrap
                    });
                }

                FaqSectionsStack.Children.Add(flatList);
            }
            else
            {
                foreach (var category in HelpManager.Faqs.Select(f => f.Category).Distinct())
                {
                    var categoryLabel = new Label
                    {
                        Text = category.ToUpper(),
                        FontSize = 11,
                        FontAttributes = FontAttributes.Bold,
                        CharacterSpacing = 1,
                        TextColor = (Color)Application.Current.Resources["FloraBlush"]
                    };

                    var itemsStack = new VerticalStackLayout { Spacing = 10 };
                    foreach (var faq in HelpManager.Faqs.Where(f => f.Category == category))
                        itemsStack.Children.Add(BuildFaqRow(faq));

                    var sectionStack = new VerticalStackLayout { Spacing = 10, Children = { categoryLabel, itemsStack } };
                    FaqSectionsStack.Children.Add(sectionStack);
                }
            }
        }

        private View BuildFaqRow(FaqItem faq)
        {
            var questionLabel = new Label
            {
                Text = faq.Question,
                FontSize = 14,
                TextColor = (Color)Application.Current.Resources["FloraCharcoal"],
                VerticalOptions = LayoutOptions.Center
            };

            var chevron = new Label
            {
                Text = faq.IsExpanded ? "\uE70E" : "\uE70D",
                FontFamily = "Segoe Fluent Icons",
                FontSize = 12,
                TextColor = (Color)Application.Current.Resources["FloraMuted"],
                VerticalOptions = LayoutOptions.Center
            };

            var questionIcon = new Label
            {
                Text = "\uE897",
                FontFamily = "Segoe Fluent Icons",
                FontSize = 13,
                TextColor = (Color)Application.Current.Resources["FloraBlush"],
                VerticalOptions = LayoutOptions.Center
            };

            var headerRow = new Grid
            {
                ColumnDefinitions = new ColumnDefinitionCollection
                {
                    new ColumnDefinition(GridLength.Auto),
                    new ColumnDefinition(GridLength.Star),
                    new ColumnDefinition(GridLength.Auto)
                },
                ColumnSpacing = 10
            };
            headerRow.Children.Add(questionIcon); Grid.SetColumn(questionIcon, 0);
            headerRow.Children.Add(questionLabel); Grid.SetColumn(questionLabel, 1);
            headerRow.Children.Add(chevron); Grid.SetColumn(chevron, 2);

            var answerLabel = new Label
            {
                Text = faq.Answer,
                FontSize = 13,
                TextColor = (Color)Application.Current.Resources["FloraMuted"],
                LineBreakMode = LineBreakMode.WordWrap,
                Margin = new Thickness(23, 8, 0, 0),
                IsVisible = faq.IsExpanded
            };

            var contentStack = new VerticalStackLayout { Spacing = 0, Children = { headerRow, answerLabel } };

            var card = new Border
            {
                BackgroundColor = Colors.White,
                StrokeShape = new RoundRectangle { CornerRadius = 14 },
                StrokeThickness = 0,
                Padding = 14,
                Content = contentStack
            };
            card.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() =>
                {
                    faq.IsExpanded = !faq.IsExpanded;
                    BuildFaqSections(SearchEntry.Text);
                })
            });

            return card;
        }

        private async void OnLiveChatTapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MessagesPage());
        }

        private async void OnEmailUsTapped(object sender, EventArgs e)
        {
            await DisplayAlert("Email Us", "Reach us anytime at support@floramix.com and we'll respond within 24 hours.", "OK");
        }

        private async void OnCallUsTapped(object sender, EventArgs e)
        {
            await DisplayAlert("Call Us", "Give us a call at +44 20 7946 0958, Monday\u2013Friday, 9 AM\u20136 PM.", "OK");
        }

        private async void OnSubmitTicketTapped(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TicketEditor.Text))
            {
                await DisplayAlert("Missing Info", "Please describe your issue before submitting.", "OK");
                return;
            }

            int reference = _rng.Next(10000, 99999);
            TicketReferenceLabel.Text = $"Reference #SUP-{reference}. We'll email you shortly.";

            TicketFormStack.IsVisible = false;
            TicketConfirmationBorder.IsVisible = true;
        }

        private async void OnBackTapped(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}