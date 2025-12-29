using System.Collections.Generic;
using System.Linq;

namespace SportsClub.Models
{
    public static class SpecializationMapping
    {
        // Facility type (as used in Facility.Type) -> allowed specializations (Ukrainian)
        public static readonly Dictionary<string, string[]> FacilityToSpecializations = new()
        {
            { "Hall", new[] { "Тяжка атлетика", "Легка атлетика" } },
            { "Tennis Court", new[] { "Теніс", "Волейбол" } },
            { "Pool", new[] { "Спортивне плавання", "Водне поло" } }
        };

        public static IEnumerable<string> AllSpecializations => FacilityToSpecializations.Values.SelectMany(x => x).Distinct();

        public static bool FacilitySupportsSpecialization(Facility facility, string specialization)
        {
            if (facility == null || string.IsNullOrWhiteSpace(specialization)) return false;
            if (string.IsNullOrWhiteSpace(facility.Type)) return false;
            var key = facility.Type.Trim();
            if (FacilityToSpecializations.TryGetValue(key, out var specs))
            {
                return specs.Contains(specialization);
            }
            // fallback: try to match by keywords
            var t = key.ToLowerInvariant();
            if ((t.Contains("hall") || t.Contains("зал")) && (specialization == "Тяжка атлетика" || specialization == "Легка атлетика")) return true;
            if ((t.Contains("tennis") || t.Contains("корт") || t.Contains("теніс")) && (specialization == "Теніс" || specialization == "Волейбол")) return true;
            if ((t.Contains("pool") || t.Contains("басейн")) && (specialization == "Спортивне плавання" || specialization == "Водне поло")) return true;
            return false;
        }

        public static IEnumerable<Facility> FilterFacilitiesForTrainer(Trainer trainer, IEnumerable<Facility> allFacilities)
        {
            if (trainer == null || allFacilities == null) return new List<Facility>();
            var spec = trainer.Specialization ?? string.Empty;
            if (string.IsNullOrWhiteSpace(spec)) return allFacilities;
            return allFacilities.Where(f => FacilitySupportsSpecialization(f, spec));
        }
    }
}
