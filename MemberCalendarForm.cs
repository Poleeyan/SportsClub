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
        private RadioButton rbUpcoming;
        private RadioButton rbPast;
        private Button btnClose;
        private Label lblRange;

        public MemberCalendarForm(Member member, List<TrainingSession> allSessions)
        {
            if (member == null) throw new ArgumentNullException(nameof(member));
            if (allSessions == null) throw new ArgumentNullException(nameof(allSessions));

            Text = "Calendar - " + member.FullName;
            Width = 700; Height = 500; StartPosition = FormStartPosition.CenterParent;

            cal = new MonthCalendar { Dock = DockStyle.Top, MaxSelectionCount = 31, Height = 200 };
            var filterPanel = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true, Height = 40 };
            filterPanel.Controls.Add(new Label { Text = "Показати:", AutoSize = true, Margin = new Padding(8, 12, 0, 10) });
            rbUpcoming = new RadioButton { Text = "Активні", AutoSize = true, Checked = true, Margin = new Padding(8, 10, 0, 10) };
            rbPast = new RadioButton { Text = "Виконані", AutoSize = true, Margin = new Padding(8, 10, 0, 10) };
            filterPanel.Controls.Add(rbUpcoming);
            filterPanel.Controls.Add(rbPast);
            lblRange = new Label { Text = "Діапазон: всі дати", AutoSize = true, Margin = new Padding(12, 12, 0, 10) };
            filterPanel.Controls.Add(lblRange);
            lstUpcoming = new ListBox { Dock = DockStyle.Fill };
            lstPast = new ListBox { Dock = DockStyle.Fill, Visible = false };
            btnClose = new Button { Text = "Close", Dock = DockStyle.Bottom, Height = 36 };
            btnClose.Click += (s, e) => Close();

            // Add in reverse Z-order: bottom first, then fill, then top
            Controls.Add(btnClose);
            Controls.Add(lstUpcoming);
            Controls.Add(lstPast);
            Controls.Add(filterPanel);
            Controls.Add(cal);

            LoadSessions(member, allSessions);

            rbUpcoming.CheckedChanged += (s, e) => { if (rbUpcoming.Checked) { lstUpcoming.Visible = true; lstPast.Visible = false; } };
            rbPast.CheckedChanged += (s, e) => { if (rbPast.Checked) { lstUpcoming.Visible = false; lstPast.Visible = true; } };
            cal.DateChanged += (s, e) => LoadSessions(member, allSessions);
        }

        private void LoadSessions(Member member, List<TrainingSession> allSessions)
        {
            var memberSessions = allSessions.Where(s => s.Participants != null && s.Participants.Any(p => p.Id == member.Id)).OrderBy(s => s.Date).ToList();
            var today = DateTime.Now.Date;

            DateTime? from = cal.SelectionStart.Date;
            DateTime? to = cal.SelectionEnd.Date;
            if (from.HasValue && to.HasValue && to.Value < from.Value)
            {
                var tmp = from;
                from = to;
                to = tmp;
            }

            IEnumerable<TrainingSession> filtered = memberSessions;
            if (from.HasValue && to.HasValue)
            {
                lblRange.Text = $"Діапазон: {from:yyyy-MM-dd} — {to:yyyy-MM-dd}";
                filtered = filtered.Where(s => s.Date.Date >= from.Value && s.Date.Date <= to.Value);
            }
            else
            {
                lblRange.Text = "Діапазон: всі дати";
            }

            var upcoming = filtered.Where(s => s.Date.Date >= today).ToList();
            var past = filtered.Where(s => s.Date.Date < today).ToList();

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
