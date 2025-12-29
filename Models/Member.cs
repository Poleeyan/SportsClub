using System;
using System.Collections.Generic;

namespace SportsClub.Models
{
    // Requirement notes:
    // - implements IComparable<Member> to demonstrate Comparable (Requirement 5)
    // - has parameterless constructor and delegated constructor (Requirements 3,4)
    public class Member : IComparable<Member>
    {
        // минимум 4 элементов данных
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public DateTime Registered { get; set; }
        public Membership? Subscription { get; set; }
        public bool IsActive { get; set; } // дополнительное поле — состояние активности
        // number of days this member purchased for the subscription
        public int PurchasedDays { get; set; }

        // конструктор без аргументов
        public Member()
        {
            Id = Guid.NewGuid();
            Registered = DateTime.Now;
            PurchasedDays = 0;
            FullName = string.Empty;
            IsActive = true;
        }

        // делегирование конструктора
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
