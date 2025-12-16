
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
        public decimal Price { get; set; } = 0m;
        public int DurationDays { get; set; } = 0;

        public Membership()
        {
            createdAt = DateTime.Now;
        }

        public Membership(string type, decimal price, int days) : this()
        {
            Type = type;
            Price = price;
            DurationDays = days;
        }

        public virtual string GetInfo()
        {
            return $"{Type} - {Price:C} for {DurationDays} days";
        }

        public abstract int GetAccessLevel(); // abstract method
    }
}
