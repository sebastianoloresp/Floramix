namespace FloraMix.Models
{
    public class FaqItem
    {
        public string Category { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public bool IsExpanded { get; set; }
    }
}