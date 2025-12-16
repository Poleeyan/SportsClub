using System;

namespace SportsClub.Models
{
    public class PremiumMembership : Membership
    {
        public bool HasPoolAccess { get; set; }

        public PremiumMembership() : base("Premium", 59.99m, 30)
        {
            HasPoolAccess = true;
        }

        public override int GetAccessLevel()
        {
            return 3; // full access
        }

        public override string GetInfo()
        {
            return base.GetInfo() + (HasPoolAccess ? " + Pool" : "") + " (Premium)";
        }
    }
}
