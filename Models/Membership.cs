
using System;
using System.Xml.Serialization;

namespace SportsClub.Models
{
    [XmlInclude(typeof(BasicMembership))]
    [XmlInclude(typeof(PremiumMembership))]
    public abstract class Membership : IAccessLevel
    {
        public enum MembershipPeriod
        {
            Monthly,
            Annual
        }

        public MembershipPeriod Period { get; set; } = MembershipPeriod.Monthly;

        protected DateTime CreatedAt { get; } = DateTime.Now; // protected to demonstrate access level

        public string Type { get; set; } = string.Empty;
        public double Price { get; set; } = 0.0;
        public int DurationDays { get; set; } = 0;

        public Membership() { }

        public Membership(string type, double price, int days) : this()
        {
            Type = type;
            Price = price;
            DurationDays = days;
        }

        public virtual string GetInfo()
        {
            return $"{Type} - {Price:C} for {DurationDays} days (created {CreatedAt:yyyy-MM-dd})";
        }

        // Calculate price for arbitrary number of days based on the
        // configured period price (linear scaling).
        public double GetPriceForDays(int days)
        {
            if (days <= 0) return 0.0;
            if (DurationDays <= 0) return Price * days; // fallback
            return Price * ((double)days / DurationDays);
        }

        public virtual double GetPrice(int days)
        {
            return GetPriceForDays(days);
        }

        // Overload to demonstrate static polymorphism: default price for full period
        public virtual double GetPrice()
        {
            return GetPriceForDays(DurationDays);
        }

        public abstract int GetAccessLevel(); // abstract method
    }
}
