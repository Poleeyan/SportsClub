using System;

namespace SportsClub.Models
{
    public class PremiumMembership : Membership
    {
        private const double MonthlyPrice = 100.00;
        private const double AnnualPrice = 900.00;

        public PremiumMembership() : this(MembershipPeriod.Monthly) { }

        public PremiumMembership(MembershipPeriod period) : base()
        {
            Period = period;
            if (period == MembershipPeriod.Monthly)
            {
                Type = "Premium (Monthly)";
                Price = MonthlyPrice;
                DurationDays = 30;
            }
            else
            {
                Type = "Premium (Annual)";
                Price = AnnualPrice;
                DurationDays = 365;
            }
        }

        public override int GetAccessLevel()
        {
            return 3; // full access
        }

        public override string GetInfo()
        {
            return base.GetInfo() + " (Premium)";
        }

        public override double GetPrice(int days)
        {
            return GetPriceForDays(days);
        }
    }
}
