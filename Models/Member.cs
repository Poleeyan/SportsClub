using System;
using System.Collections.Generic;

namespace SportsClub.Models
{
    public class Member : IComparable<Member>
    {
        // минимум 4 элементов данных
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public DateTime Registered { get; set; }
        public Membership? Subscription { get; set; }
        public int Visits { get; set; } // инкапсуляция через свойство для сериализации
        public bool IsActive { get; set; } // дополнительное поле — состояние активности

        // конструктор без аргументов
        public Member()
        {
            Id = Guid.NewGuid();
            Registered = DateTime.Now;
            Visits = 0;
            FullName = string.Empty;
            IsActive = true;
        }

        // делегирование конструктора
        public Member(string name) : this()
        {
            FullName = name;
        }

        public void AddVisit()
        {
            Visits++;
        }

        public void AddVisit(DateTime date)
        {
            // перегрузка: можно расширить запись истории
            Visits++;
        }

        public int GetVisits()
        {
            return Visits;
        }

        public override string ToString()
        {
            return $"{FullName} ({Id.ToString().Substring(0,6)}) - Visits: {Visits} - {Subscription?.GetInfo() ?? "No sub"}";
        }

        public int CompareTo(Member? other)
        {
            if (other == null) return 1;
            return string.Compare(FullName, other.FullName, StringComparison.OrdinalIgnoreCase);
        }
    }
}
