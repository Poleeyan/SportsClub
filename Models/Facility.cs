using System;

namespace SportsClub.Models
{
    public class Facility
    {
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public int RequiredAccessLevel { get; set; } = 1; // 1 = Basic, 2 = Advanced, 3 = Premium

        public Facility() { Name = string.Empty; Type = string.Empty; }

        public Facility(string name, string type, int cap, int requiredLevel = 1)
        {
            Name = name;
            Type = type;
            Capacity = cap;
            RequiredAccessLevel = requiredLevel;
        }
    }
}
