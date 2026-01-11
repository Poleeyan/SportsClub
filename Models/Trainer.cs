using System;

namespace SportsClub.Models
{
    public class Trainer
    {
        private string _fullName = string.Empty;
        private string _specialization = string.Empty;
        private int _experienceYears;

        public string FullName
        {
            get => _fullName;
            set => _fullName = value?.Trim() ?? string.Empty;
        }

        public string Specialization
        {
            get => _specialization;
            set => _specialization = value?.Trim() ?? string.Empty;
        }

        public int ExperienceYears
        {
            get => _experienceYears;
            set => _experienceYears = Math.Max(0, value);
        }

        public Trainer() { }

        public Trainer(string name, string spec, int exp)
        {
            FullName = name;
            Specialization = spec;
            ExperienceYears = exp;
        }
    }
}
