using System;
using System.Collections.Generic;
using System.Linq;
using FloraMix.Models;
using Microsoft.Maui.Graphics;

namespace FloraMix.Drawables
{
    public class BouquetPreviewDrawable : IDrawable
    {
        public List<FlowerOption> Flowers { get; set; }
        public List<ColorOption> Colors { get; set; }
        public List<FillerOption> Fillers { get; set; }
        public List<WrappingOption> Wrappings { get; set; }
        public List<AddOnOption> AddOns { get; set; }

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            canvas.SaveState();

            canvas.FillColor = Color.FromArgb("#F5EFE6");
            canvas.FillRoundedRectangle(dirtyRect, 20);

            float centerX = dirtyRect.Width / 2;
            float wrapTopY = dirtyRect.Height * 0.55f;

            var selectedWrap = Wrappings?.FirstOrDefault(w => w.IsSelected);
            bool isBox = selectedWrap?.Name == "Modern Box";

            if (isBox)
            {
                // Box-shaped wrap, pink colored (instead of a triangle/cone)
                canvas.FillColor = Color.FromArgb("#E8879F");
                float boxWidth = 140;
                float boxHeight = (dirtyRect.Height - 10) - wrapTopY;
                var boxRect = new RectF(centerX - boxWidth / 2, wrapTopY, boxWidth, boxHeight);
                canvas.FillRoundedRectangle(boxRect, 10);

                canvas.StrokeColor = Color.FromArgb("#C2617C");
                canvas.StrokeSize = 2;
                canvas.DrawRoundedRectangle(boxRect, 10);
            }
            else
            {
                Color wrapColor = selectedWrap?.Name switch
                {
                    "Satin Wrap" => Color.FromArgb("#F2A6BC"),
                    "Rustic Hessian" => Color.FromArgb("#C2A878"),
                    _ => Color.FromArgb("#E8D9B5")
                };

                // V-shaped wrap: wide at the top (holding the flowers), narrowing to a point at the bottom (tied)
                canvas.FillColor = wrapColor;
                var wrapPath = new PathF();
                wrapPath.MoveTo(centerX - 75, wrapTopY);
                wrapPath.LineTo(centerX + 75, wrapTopY);
                wrapPath.LineTo(centerX, dirtyRect.Height - 10);
                wrapPath.Close();
                canvas.FillPath(wrapPath);
            }

            canvas.StrokeColor = Color.FromArgb("#8B5A2B");
            canvas.StrokeSize = 3;
            canvas.DrawLine(centerX - 40, dirtyRect.Height - 45, centerX + 40, dirtyRect.Height - 45);

            var chosenColors = Colors?.Where(c => c.IsSelected)
                .Select(c => Color.FromArgb(c.HexColor)).ToList();
            if (chosenColors == null || chosenColors.Count == 0)
                chosenColors = new List<Color> { Color.FromArgb("#F2A6BC") };

            int totalStems = Flowers?.Where(f => f.IsSelected).Sum(f => f.Stems) ?? 0;
            int dotsToShow = Math.Min(totalStems, 14);

            var fillerColors = new Dictionary<string, Color>
            {
                { "Baby's Breath", Color.FromArgb("#F6D8E3") },
                { "Eucalyptus", Color.FromArgb("#7A9A72") },
                { "Waxflower", Color.FromArgb("#E8A0BE") },
                { "Lisianthus", Color.FromArgb("#9B6BA8") },
                { "Ruscus Leaves", Color.FromArgb("#4F6B3A") },
                { "Dried Pampas", Color.FromArgb("#D9C29C") },
            };

            var selectedFillers = Fillers?.Where(f => f.IsSelected).ToList() ?? new List<FillerOption>();
            if (selectedFillers.Count > 0)
            {
                const int itemsPerFiller = 3;
                int totalItems = selectedFillers.Count * itemsPerFiller;
                float fillerCenterY = wrapTopY - 40;
                for (int k = 0; k < totalItems; k++)
                {
                    var filler = selectedFillers[k / itemsPerFiller];
                    Color fillerColor = fillerColors.TryGetValue(filler.Name, out var fc) ? fc : Color.FromArgb("#7A9A72");
                    double angle = k * (Math.PI * 2 / totalItems);
                    float lx = centerX + (float)(Math.Cos(angle) * 95);
                    float ly = fillerCenterY + (float)(Math.Sin(angle) * 60);
                    DrawFillerShape(canvas, lx, ly, filler.Name, fillerColor);
                }
            }

            if (dotsToShow == 0)
            {
                canvas.FontColor = Color.FromArgb("#9B9B9B");
                canvas.FontSize = 13;
                canvas.DrawString("Pick flowers to see a preview", 0, wrapTopY - 60, dirtyRect.Width, 24,
                    HorizontalAlignment.Center, VerticalAlignment.Center);
            }
            else
            {
                // Evenly spaced golden-angle spiral so flowers don't overlap and 1 stem = 1 flower
                float clusterCenterY = wrapTopY - 50;
                const double goldenAngle = 2.39996; // ~137.5 degrees
                const float spacing = 22f;
                const float flowerSize = 15f;
                for (int i = 0; i < dotsToShow; i++)
                {
                    double angle = i * goldenAngle;
                    double radius = spacing * Math.Sqrt(i);
                    float x = centerX + (float)(Math.Cos(angle) * radius);
                    float y = clusterCenterY + (float)(Math.Sin(angle) * radius * 0.7);
                    DrawFlower(canvas, x, y, chosenColors[i % chosenColors.Count], flowerSize);
                }
            }

            bool hasRibbon = AddOns?.Any(a => a.Name == "Luxury Ribbon" && a.IsSelected) ?? false;
            if (hasRibbon)
            {
                canvas.FillColor = Color.FromArgb("#B5322C");
                canvas.FillCircle(centerX - 14, dirtyRect.Height - 45, 10);
                canvas.FillCircle(centerX + 14, dirtyRect.Height - 45, 10);
                canvas.FillCircle(centerX, dirtyRect.Height - 45, 6);
            }

            bool hasGreetingCard = AddOns?.Any(a => a.Name == "Greeting Card" && a.IsSelected) ?? false;
            if (hasGreetingCard)
            {
                canvas.FontSize = 22;
                canvas.DrawString("\u2709\uFE0F", dirtyRect.Width - 46, wrapTopY - 30, 36, 36,
                    HorizontalAlignment.Center, VerticalAlignment.Center);
            }

            bool hasChocolates = AddOns?.Any(a => a.Name == "Chocolates" && a.IsSelected) ?? false;
            if (hasChocolates)
            {
                canvas.FontSize = 22;
                canvas.DrawString("\U0001F36B", 10, wrapTopY - 30, 36, 36,
                    HorizontalAlignment.Center, VerticalAlignment.Center);
            }

            canvas.RestoreState();
        }

        private void DrawFlower(ICanvas canvas, float cx, float cy, Color petalColor, float size)
        {
            canvas.SaveState();
            canvas.Translate(cx, cy);

            const int petalCount = 5;
            float petalLength = size;
            float petalWidth = size * 0.55f;

            canvas.FillColor = petalColor;
            for (int p = 0; p < petalCount; p++)
            {
                canvas.SaveState();
                canvas.Rotate(360f / petalCount * p);
                canvas.FillEllipse(-petalWidth / 2, -petalLength, petalWidth, petalLength);
                canvas.StrokeColor = Color.FromArgb("#22000000");
                canvas.StrokeSize = 0.75f;
                canvas.DrawEllipse(-petalWidth / 2, -petalLength, petalWidth, petalLength);
                canvas.RestoreState();
            }

            float centerR = size * 0.32f;
            canvas.FillColor = Color.FromArgb("#F4C542");
            canvas.FillCircle(0, 0, centerR);
            canvas.StrokeColor = Color.FromArgb("#33000000");
            canvas.StrokeSize = 1;
            canvas.DrawCircle(0, 0, centerR);

            canvas.RestoreState();
        }

        private void DrawFillerShape(ICanvas canvas, float cx, float cy, string fillerName, Color color)
        {
            canvas.SaveState();
            canvas.Translate(cx, cy);

            switch (fillerName)
            {
                case "Baby's Breath":
                    // Tiny cluster of small puff dots
                    canvas.FillColor = color;
                    canvas.FillCircle(-4, -2, 3.2f);
                    canvas.FillCircle(4, -1, 3.2f);
                    canvas.FillCircle(0, 4, 3.2f);
                    break;

                case "Eucalyptus":
                    // Small round leaves along a stem
                    canvas.FillColor = color;
                    canvas.FillCircle(-5, 1, 5f);
                    canvas.FillCircle(5, 4, 5f);
                    canvas.FillCircle(0, -6, 4.5f);
                    break;

                case "Waxflower":
                    // Tiny 5-petal flower, smaller/simpler than the main flowers
                    canvas.FillColor = color;
                    for (int p = 0; p < 5; p++)
                    {
                        canvas.SaveState();
                        canvas.Rotate(72f * p);
                        canvas.FillEllipse(-3, -9, 6, 9);
                        canvas.RestoreState();
                    }
                    canvas.FillColor = Color.FromArgb("#F4C542");
                    canvas.FillCircle(0, 0, 3);
                    break;

                case "Lisianthus":
                    // Ruffled flower with 6 overlapping petals
                    canvas.FillColor = color;
                    for (int p = 0; p < 6; p++)
                    {
                        canvas.SaveState();
                        canvas.Rotate(60f * p);
                        canvas.FillEllipse(-4.5f, -11, 9, 11);
                        canvas.RestoreState();
                    }
                    canvas.FillColor = Color.FromArgb("#FFF7FA");
                    canvas.FillCircle(0, 0, 3.5f);
                    break;

                case "Ruscus Leaves":
                    // Pointed elongated leaf
                    canvas.FillColor = color;
                    var leafPath = new PathF();
                    leafPath.MoveTo(0, -14);
                    leafPath.CurveTo(6, -8, 6, 8, 0, 14);
                    leafPath.CurveTo(-6, 8, -6, -8, 0, -14);
                    leafPath.Close();
                    canvas.FillPath(leafPath);
                    break;

                case "Dried Pampas":
                    // Fluffy plume made of thin wisps
                    canvas.StrokeColor = color;
                    canvas.StrokeSize = 2;
                    for (int w = -2; w <= 2; w++)
                    {
                        canvas.DrawLine(0, 8, w * 4, -12);
                    }
                    break;

                default:
                    canvas.FillColor = color;
                    canvas.FillEllipse(-6, -14, 12, 28);
                    break;
            }

            canvas.RestoreState();
        }
    }
}