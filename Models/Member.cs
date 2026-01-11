using System;

namespace SportsClub.Models
{
    public class Member : IComparable<Member>
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public DateTime Registered { get; set; }
        public Membership? Subscription { get; set; }
        public bool IsActive { get; set; }

        public Member()
        {
            Id = Guid.NewGuid();
            Registered = DateTime.Now;
            FullName = string.Empty;
            IsActive = true;
        }

        public Member(string name) : this()
        {
            FullName = name;
        }

        public override string ToString()
        {
            return $"{FullName} ({Id.ToString()[..6]}) - {Subscription?.GetInfo() ?? "No sub"}";
        }

        public int CompareTo(Member? other)
        {
            if (other == null) return 1;
            return string.Compare(FullName, other.FullName, StringComparison.OrdinalIgnoreCase);
        }

    }
}
