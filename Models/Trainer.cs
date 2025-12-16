using System;

namespace SportsClub.Models
{
    public class Trainer
    {
        public string FullName { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
        public int ExperienceYears { get; set; }

        public Trainer() { FullName = string.Empty; Specialization = string.Empty; }

        public Trainer(string name, string spec, int exp)
        {
            FullName = name;
            Specialization = spec;
            ExperienceYears = exp;
        }
    }
}
