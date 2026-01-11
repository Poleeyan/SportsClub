using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using SportsClub.Models;

namespace SportsClub
{
    public class MemberCalendarForm : Form
    {
        private MonthCalendar cal;
        private ListBox lstUpcoming;
        private ListBox lstPast;
        private Button btnClose;

        public MemberCalendarForm(Member member, List<TrainingSession> allSessions)
        {
            if (member == null) throw new ArgumentNullException(nameof(member));
            if (allSessions == null) throw new ArgumentNullException(nameof(allSessions));

            Text = "Calendar - " + member.FullName;
            Width = 700; Height = 500; StartPosition = FormStartPosition.CenterParent;

            cal = new MonthCalendar { Dock = DockStyle.Top, MaxSelectionCount = 1, Height = 160 };
            lstUpcoming = new ListBox { Dock = DockStyle.Left, Width = 340 };
            lstPast = new ListBox { Dock = DockStyle.Right, Width = 340 };
            btnClose = new Button { Text = "Close", Dock = DockStyle.Bottom, Height = 36 };
            btnClose.Click += (s, e) => Close();

            Controls.Add(cal);
            Controls.Add(lstPast);
            Controls.Add(lstUpcoming);
            Controls.Add(btnClose);

            LoadSessions(member, allSessions);
        }

        private void LoadSessions(Member member, List<TrainingSession> allSessions)
        {
            var memberSessions = allSessions.Where(s => s.Participants != null && s.Participants.Any(p => p.Id == member.Id)).OrderBy(s => s.Date).ToList();
            var today = DateTime.Now.Date;
            var upcoming = memberSessions.Where(s => s.Date.Date >= today).ToList();
            var past = memberSessions.Where(s => s.Date.Date < today).ToList();

            // Bold all session dates
            cal.RemoveAllBoldedDates();
            foreach (var s in memberSessions)
            {
                cal.AddBoldedDate(s.Date.Date);
            }
            cal.UpdateBoldedDates();

            lstUpcoming.Items.Clear();
            foreach (var s in upcoming)
            {
                lstUpcoming.Items.Add($"{s.Date:yyyy-MM-dd} - {s.Coach?.FullName ?? ""} @ {(string.IsNullOrEmpty(s.Location) ? s.Facility?.Name : s.Location)}");
            }

            lstPast.Items.Clear();
            foreach (var s in past)
            {
                lstPast.Items.Add($"{s.Date:yyyy-MM-dd} - {s.Coach?.FullName ?? ""} @ {(string.IsNullOrEmpty(s.Location) ? s.Facility?.Name : s.Location)}");
            }
        }
    }
}
