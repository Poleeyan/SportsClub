
using System;
using System.Xml.Serialization;

namespace SportsClub.Models
{
    [XmlInclude(typeof(BasicMembership))]
    [XmlInclude(typeof(PremiumMembership))]
    public abstract class Membership
    {
        [XmlIgnore]
        protected DateTime createdAt; // protected example, ignored for XML

        public string Type { get; set; } = string.Empty;
        // Price is total price for the period defined by DurationDays.
        // Use double here per requirement (and for simple calculations).
        public double Price { get; set; } = 0.0;
        public int DurationDays { get; set; } = 0;

        public Membership()
        {
            createdAt = DateTime.Now;
        }

        public Membership(string type, double price, int days) : this()
        {
            Type = type;
            Price = price;
            DurationDays = days;
        }

        public virtual string GetInfo()
        {
            return $"{Type} - {Price.ToString("C")} for {DurationDays} days";
        }

        // Calculate price for arbitrary number of days based on the
        // configured period price (linear scaling).
        public double GetPriceForDays(int days)
        {
            if (days <= 0) return 0.0;
            if (DurationDays <= 0) return Price * days; // fallback
            return Price * ((double)days / DurationDays);
        }

        // Calculate full price including per-visit charges when applicable.
        // Default: day-based price plus per-visit fee.
        public virtual double GetPrice(int days, int visits)
        {
            double basePrice = GetPriceForDays(days);
            const double visitFee = 2.0; // default per-visit fee
            if (visits <= 0) return basePrice;
            return basePrice + visitFee * visits;
        }

        public abstract int GetAccessLevel(); // abstract method
    }
}
