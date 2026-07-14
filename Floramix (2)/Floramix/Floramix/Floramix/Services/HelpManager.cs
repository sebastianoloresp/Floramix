using System.Collections.Generic;
using FloraMix.Models;

namespace FloraMix.Services
{
    public static class HelpManager
    {
        public static List<FaqItem> Faqs { get; private set; } = new List<FaqItem>
        {
            new FaqItem
            {
                Category = "Orders",
                Question = "How do I track my order?",
                Answer = "You can track your order in real time from the Order History section of your profile — tap \"Track\" on any active order to see live status and estimated delivery time."
            },
            new FaqItem
            {
                Category = "Orders",
                Question = "Can I cancel my order?",
                Answer = "Orders can be cancelled free of charge within 15 minutes of placing them. After that, please reach out through Live Chat and we'll do our best to accommodate your request before the bouquet is prepared."
            },
            new FaqItem
            {
                Category = "Orders",
                Question = "What if I received the wrong order?",
                Answer = "We're sorry about that! Please reach out through Live Chat or submit a ticket below with your order number, and we'll arrange a replacement or refund right away."
            },
            new FaqItem
            {
                Category = "Delivery",
                Question = "What are the delivery times?",
                Answer = "Standard delivery windows are 9 AM\u20132 PM and 2 PM\u20137 PM. You'll get a more precise estimate (\u00B115 minutes) once your order is out for delivery."
            },
            new FaqItem
            {
                Category = "Delivery",
                Question = "Do you deliver on weekends?",
                Answer = "Yes! We deliver 7 days a week, including public holidays, so your flowers arrive exactly when you need them."
            },
            new FaqItem
            {
                Category = "Delivery",
                Question = "What if no one is home?",
                Answer = "Our courier will leave the bouquet with a neighbor or in a safe, shaded spot when possible, and text you a photo along with the exact location. You can also add delivery notes at checkout."
            },
            new FaqItem
            {
                Category = "Payments & Refunds",
                Question = "When am I charged?",
                Answer = "Your card (or Cash on Delivery order) is charged as soon as your order is confirmed by the shop, not the moment you place it."
            },
            new FaqItem
            {
                Category = "Payments & Refunds",
                Question = "How do refunds work?",
                Answer = "Refunds are processed back to your original payment method within 3\u20135 business days. For Cash on Delivery orders, we'll arrange a bank transfer instead."
            },
        };
    }
}