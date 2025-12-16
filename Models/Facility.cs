using System;

namespace SportsClub.Models
{
    public class Facility
    {
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public int Capacity { get; set; }

        public Facility() { Name = string.Empty; Type = string.Empty; }

        public Facility(string name, string type, int cap)
        {
            Name = name;
            Type = type;
            Capacity = cap;
        }
    }
}
