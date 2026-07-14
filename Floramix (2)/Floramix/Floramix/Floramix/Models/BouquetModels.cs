namespace FloraMix.Models
{
    public class FlowerOption
    {
        public string Name { get; set; }
        public string Emoji { get; set; }
        public double PricePerStem { get; set; }
        public bool IsSelected { get; set; }
        public int Stems { get; set; } = 1;
    }

    public class ColorOption
    {
        public string Name { get; set; }
        public string HexColor { get; set; }
        public bool IsSelected { get; set; }
    }

    public class FillerOption
    {
        public string Name { get; set; }
        public string Emoji { get; set; }
        public double Price { get; set; }
        public bool IsSelected { get; set; }
    }

    public class WrappingOption
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public bool IsSelected { get; set; }
    }

    public class AddOnOption
    {
        public string Name { get; set; }
        public string Emoji { get; set; }
        public double Price { get; set; }
        public bool IsSelected { get; set; }
    }
}