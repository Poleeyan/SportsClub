using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SportsClub.Models;

namespace SportsClub.Services
{
    public static class ReportsService
    {
        // Generates a CSV attendance report listing each session and its participants.
        // Columns: SessionDate, Coach, Facility, MemberId, MemberName
        public static void GenerateAttendanceCsv(string path, IEnumerable<TrainingSession> sessions, DateTime? from = null, DateTime? to = null)
        {
            var rows = new List<string>();
            rows.Add("SessionDate,Coach,Facility,MemberId,MemberName");

            var filtered = sessions ?? Enumerable.Empty<TrainingSession>();
            if (from.HasValue) filtered = filtered.Where(s => s.Date >= from.Value);
            if (to.HasValue) filtered = filtered.Where(s => s.Date <= to.Value);

            foreach (var s in filtered.OrderBy(s => s.Date))
            {
                var date = s.Date.ToString("o");
                var coach = s.Coach?.FullName ?? string.Empty;
                var facility = string.IsNullOrEmpty(s.Location) ? s.Facility?.Name ?? string.Empty : s.Location;
                if (s.Participants == null || s.Participants.Count == 0)
                {
                    rows.Add($"{Escape(date)},{Escape(coach)},{Escape(facility)},,");
                    continue;
                }
                foreach (var m in s.Participants)
                {
                    var id = m?.Id.ToString() ?? string.Empty;
                    var name = m?.FullName ?? string.Empty;
                    rows.Add($"{Escape(date)},{Escape(coach)},{Escape(facility)},{Escape(id)},{Escape(name)}");
                }
            }

            File.WriteAllLines(path, rows, Encoding.UTF8);
        }

        private static string Escape(string? v)
        {
            if (string.IsNullOrEmpty(v)) return string.Empty;
            if (v.Contains(',') || v.Contains('"') || v.Contains('\n'))
            {
                return '"' + v.Replace("\"", "\"\"") + '"';
            }
            return v;
        }
    }
}
