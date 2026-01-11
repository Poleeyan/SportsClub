using System;

namespace SportsClub.Models
{
    public class BasicMembership : Membership
    {
        private const double MonthlyPrice = 30.00;
        private const double AnnualPrice = 300.00;

        public BasicMembership() : this(MembershipPeriod.Monthly) { }

        public BasicMembership(MembershipPeriod period) : base()
        {
            Period = period;
            if (period == MembershipPeriod.Monthly)
            {
                Type = "Basic (Monthly)";
                Price = MonthlyPrice;
                DurationDays = 30;
            }
            else
            {
                Type = "Basic (Annual)";
                Price = AnnualPrice;
                DurationDays = 365;
            }
        }

        public override int GetAccessLevel()
        {
            return 1; // limited access
        }

        public override string GetInfo()
        {
            return base.GetInfo() + " (Basic access)";
        }

        public override double GetPrice(int days)
        {
            return GetPriceForDays(days);
        }
    }
}
