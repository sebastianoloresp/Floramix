using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Graphics;

namespace FloraMix.Services
{
    public static class CheckoutStepHelper
    {
        private static readonly string[] StepNames = { "Cart", "Delivery", "Payment", "Review" };

        // currentStep: 1 = Cart, 2 = Delivery, 3 = Payment, 4 = Review
        public static void BuildStepIndicator(Grid grid, int currentStep)
        {
            grid.Children.Clear();
            grid.ColumnDefinitions.Clear();

            for (int i = 0; i < 4; i++)
                grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));

            var blush = (Color)Application.Current.Resources["FloraBlush"];
            var fieldBg = (Color)Application.Current.Resources["FloraFieldBg"];
            var muted = (Color)Application.Current.Resources["FloraMuted"];
            var charcoal = (Color)Application.Current.Resources["FloraCharcoal"];

            for (int i = 0; i < 4; i++)
            {
                int stepNumber = i + 1;
                bool isDone = stepNumber < currentStep;
                bool isActive = stepNumber == currentStep;

                Label circleLabel;
                Color circleBg;

                if (isDone)
                {
                    circleLabel = new Label { Text = "\uE73E", FontFamily = "Segoe Fluent Icons", FontSize = 14, TextColor = Colors.White, HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center };
                    circleBg = blush;
                }
                else if (isActive)
                {
                    circleLabel = new Label { Text = stepNumber.ToString(), FontAttributes = FontAttributes.Bold, FontSize = 14, TextColor = Colors.White, HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center };
                    circleBg = blush;
                }
                else
                {
                    circleLabel = new Label { Text = stepNumber.ToString(), FontAttributes = FontAttributes.Bold, FontSize = 14, TextColor = muted, HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center };
                    circleBg = fieldBg;
                }

                var circle = new Border { HeightRequest = 32, WidthRequest = 32, StrokeShape = new RoundRectangle { CornerRadius = 16 }, StrokeThickness = 0, BackgroundColor = circleBg, Content = circleLabel };
                var nameLabel = new Label { Text = StepNames[i], FontSize = 11, FontAttributes = isActive ? FontAttributes.Bold : FontAttributes.None, TextColor = isActive ? blush : muted, HorizontalOptions = LayoutOptions.Center };

                var colStack = new VerticalStackLayout { Spacing = 4, HorizontalOptions = LayoutOptions.Center, Children = { circle, nameLabel } };

                grid.Children.Add(colStack);
                Grid.SetColumn(colStack, i);
            }
        }
    }
}