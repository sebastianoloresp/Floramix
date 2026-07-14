using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json;
using SQLite;

namespace FloraMix.Models
{
    public class Product : INotifyPropertyChanged
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string ImageSource { get; set; }
        public string DetailImageSource { get; set; }
        public string Shop { get; set; }
        public string Name { get; set; }
        public double Rating { get; set; }
        public int ReviewCount { get; set; }
        public double Price { get; set; }
        public string Tag { get; set; }
        public string Category { get; set; }
        public string Location { get; set; } = "Sta Rosa, Laguna";
        public int StockCount { get; set; } = 8;
        public string Description { get; set; } = "A stunning arrangement of premium blooms, hand-selected from our trusted growers and arranged by our master florists.";

        // Raw JSON columns actually stored in SQLite
        public string WhatsIncludedJson { get; set; } = "[]";
        public string ColorPaletteJson { get; set; } = "[]";
        public string ReviewsJson { get; set; } = "[]";

        [Ignore]
        public List<string> WhatsIncluded
        {
            get => string.IsNullOrEmpty(WhatsIncludedJson) ? new List<string>() : JsonSerializer.Deserialize<List<string>>(WhatsIncludedJson);
            set => WhatsIncludedJson = JsonSerializer.Serialize(value);
        }

        [Ignore]
        public List<string> ColorPalette
        {
            get => string.IsNullOrEmpty(ColorPaletteJson) ? new List<string>() : JsonSerializer.Deserialize<List<string>>(ColorPaletteJson);
            set => ColorPaletteJson = JsonSerializer.Serialize(value);
        }

        [Ignore]
        public List<ReviewItem> Reviews
        {
            get => string.IsNullOrEmpty(ReviewsJson) ? new List<ReviewItem>() : JsonSerializer.Deserialize<List<ReviewItem>>(ReviewsJson);
            set => ReviewsJson = JsonSerializer.Serialize(value);
        }

        [Ignore]
        public List<string> GalleryImages
        {
            get
            {
                var list = new List<string>();
                if (!string.IsNullOrEmpty(DetailImageSource)) list.Add(DetailImageSource);
                if (!string.IsNullOrEmpty(ImageSource)) list.Add(ImageSource);
                return list;
            }
        }

        private bool _isWishlisted;
        [Ignore]
        public bool IsWishlisted
        {
            get => _isWishlisted;
            set
            {
                if (_isWishlisted != value)
                {
                    _isWishlisted = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}