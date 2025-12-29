using System;

namespace SportsClub.Models
{
    public class BasicMembership : Membership
    {
        public BasicMembership() : base("Basic", 29.99, 30) { }

        public override int GetAccessLevel()
        {
            return 1; // limited access
        }

        public override string GetInfo()
        {
            return base.GetInfo() + " (Basic access)";
        }

        public override double GetPrice(int days, int visits)
        {
            // Price for Basic is day-based only (visits removed)
            return GetPriceForDays(days);
        }
    }
}
