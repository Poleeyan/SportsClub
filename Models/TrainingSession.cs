using System;
using System.Collections.Generic;

namespace SportsClub.Models
{
    public class TrainingSession
    {
        public DateTime Date { get; set; }
        public Trainer? Coach { get; set; }
        public List<Member> Participants { get; set; } = [];
        public Facility? Facility { get; set; }
        public string Location { get; set; } = string.Empty;

        public TrainingSession() { }

        public TrainingSession(DateTime date, Trainer? coach, Facility? facility = null)
        {
            Date = date;
            Coach = coach;
            Facility = facility;
            Location = facility?.Name ?? string.Empty;
        }
    }
}
