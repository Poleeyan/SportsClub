using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using SportsClub.Models;
using SportsClub.Services;

namespace SportsClub
{
    public partial class MainForm : Form
    {
        private List<Member> members = new List<Member>();
        private List<Trainer> trainers = new List<Trainer>();
        private List<TrainingSession> sessions = new List<TrainingSession>();
        private readonly List<Facility> facilities = new List<Facility>();

        // header-based filters
        private DateTime? headerFilterFrom;
        private DateTime? headerFilterTo;
        private string? headerFilterCoach;
        private string? headerFilterType;
        private string? headerFilterPlace;
        private int? headerFilterMinParticipants;
        private bool headerParticipantsDesc = true;

        public MainForm()
        {
            // static facilities: зал, басейн, тенісний корт
            facilities = new List<Facility>
            {
                new Facility("Зал", "Hall", 50, requiredLevel: 1),        // Доступ для Basic
                new Facility("Басейн", "Pool", 20, requiredLevel: 3),      // Тільки Premium
                new Facility("Тенісний корт", "Tennis Court", 4, requiredLevel: 3) // Тільки Premium
            };
            InitializeComponent();
        }

        private void BtnAdd_Click(object? sender, EventArgs e)
        {
            using var form = new MemberForm();
            form.Member = new Member();
            if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                members.Add(form.Member);
                RefreshAllGrids();
            }
        }

        private void BtnEdit_Click(object? sender, EventArgs e)
        {
            if (dgvMembers!.CurrentRow == null) { MessageBox.Show("Select a member"); return; }
            var idx = dgvMembers!.CurrentRow!.Index;
            if (idx < 0 || idx >= members.Count) return;
            var m = members[idx];
            var form = new MemberForm
            {
                Member = new Member(m.FullName)
                {
                    Id = m.Id,
                    Registered = m.Registered,
                    Subscription = m.Subscription
                }
            };
            if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                members[idx] = form.Member;
                RefreshAllGrids();
            }
        }

        private void BtnDelete_Click(object? sender, EventArgs e)
        {
            if (dgvMembers!.CurrentRow == null) { MessageBox.Show("Select a member"); return; }
            var idx = dgvMembers!.CurrentRow!.Index;
            if (idx < 0 || idx >= members.Count) return;
            if (MessageBox.Show("Delete selected member?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                members.RemoveAt(idx);
                RefreshAllGrids();
            }
        }

        private void RefreshMembersGrid()
        {
            dgvMembers!.DataSource = null;
            members.Sort(); // ensure IComparable is actually used
            static string FormatSession(TrainingSession s)
            {
                var spec = s.Coach?.Specialization ?? string.Empty;
                var place = string.IsNullOrEmpty(s.Location) ? s.Facility?.Name : s.Location;
                var prefix = string.IsNullOrEmpty(spec) ? string.Empty : spec + ": ";
                return $"{s.Date:yyyy-MM-dd} ({prefix}{place})";
            }

            var view = members.Select(m => new
            {
                m.FullName,
                Sessions = string.Join(", ", sessions.Where(s => s.Participants != null && s.Participants.Any(p => p.Id == m.Id)).Select(s => FormatSession(s))),
                Subscription = m.Subscription?.Type,
                // compute remaining days and active flag based on subscription validity
                RemainingDays = m.Subscription != null ? Math.Max(0, (m.Subscription.DurationDays - (DateTime.Now - m.Registered).Days)) : 0,
                Active = m.Subscription != null ? Math.Max(0, (m.Subscription.DurationDays - (DateTime.Now - m.Registered).Days)) > 0 : false,
                // prorated price for remaining days
                Price = m.Subscription != null ? m.Subscription.GetPrice(Math.Max(0, (m.Subscription.DurationDays - (DateTime.Now - m.Registered).Days))) : 0.0
            }).ToList();

            // update IsActive flags based on remaining days
            for (int i = 0; i < members.Count; i++)
            {
                var remaining = view[i].RemainingDays;
                members[i].IsActive = remaining > 0;
            }
            dgvMembers!.DataSource = view;
            // format Price column as currency if present
            var _priceCol = dgvMembers!.Columns["Price"];
            if (_priceCol != null)
            {
                _priceCol.DefaultCellStyle.Format = "C2";
                _priceCol.DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            }
        }

        private void RefreshTrainersGrid()
        {
            dgvTrainers!.DataSource = null;
            var view = trainers.Select(t => new { t.FullName, t.Specialization, t.ExperienceYears }).ToList();
            dgvTrainers!.DataSource = view;
        }

        private void RefreshSessionsGrid()
        {
            dgvSessions!.DataSource = null;
            // apply header-based filters
            IEnumerable<TrainingSession> filtered = sessions;
            if (headerFilterPlace != null)
            {
                filtered = filtered.Where(s => (s.Facility != null && s.Facility.Name == headerFilterPlace) || s.Location == headerFilterPlace);
            }
            if (headerFilterCoach != null)
            {
                filtered = filtered.Where(s => s.Coach != null && s.Coach.FullName == headerFilterCoach);
            }
            if (headerFilterType != null)
            {
                filtered = filtered.Where(s => (s.Coach?.Specialization ?? string.Empty) == headerFilterType);
            }
            if (headerFilterFrom.HasValue && headerFilterTo.HasValue)
            {
                var from = headerFilterFrom.Value.Date;
                var to = headerFilterTo.Value.Date.AddDays(1).AddTicks(-1);
                filtered = filtered.Where(s => s.Date >= from && s.Date <= to);
            }
            if (headerFilterMinParticipants.HasValue)
            {
                var min = headerFilterMinParticipants.Value;
                filtered = filtered.Where(s => (s.Participants?.Count ?? 0) >= min);
            }

            // Active vs Past toggle
            var today = DateTime.Now.Date;
            var showPast = rbSessionsPast != null && rbSessionsPast.Checked;
            if (showPast)
            {
                filtered = filtered.Where(s => s.Date.Date < today);
            }
            else
            {
                filtered = filtered.Where(s => s.Date.Date >= today);
            }

            var filteredList = filtered.ToList();
            if (headerParticipantsDesc)
            {
                filteredList = filteredList.OrderByDescending(s => s.Participants?.Count ?? 0).ToList();
            }
            else
            {
                filteredList = filteredList.OrderBy(s => s.Participants?.Count ?? 0).ToList();
            }

            var view = filteredList.Select(s => new {
                Date = s.Date.ToString("g"),
                Coach = s.Coach?.FullName,
                TrainingType = s.Coach?.Specialization,
                Participants = s.Participants?.Count ?? 0,
                Place = string.IsNullOrEmpty(s.Location) ? s.Facility?.Name : s.Location
            }).ToList();
            dgvSessions!.DataSource = view;

            // underline past sessions in the grid for visual cue
            for (int i = 0; i < filteredList.Count && i < dgvSessions.Rows.Count; i++)
            {
                var row = dgvSessions.Rows[i];
                var isPast = filteredList[i].Date.Date < today;
                var styleFont = new Font(dgvSessions.Font, isPast ? FontStyle.Underline : FontStyle.Regular);
                row.DefaultCellStyle.Font = styleFont;
            }
            // rename TrainingType column header to the requested Ukrainian label
            var _ttCol = dgvSessions!.Columns["TrainingType"];
            if (_ttCol != null)
            {
                _ttCol.HeaderText = "Тренерування";
            }
        }

        private void DgvSessions_ColumnHeaderMouseClick(object? sender, DataGridViewCellMouseEventArgs e)
        {
            if (dgvSessions == null) return;
            if (e.ColumnIndex < 0 || e.ColumnIndex >= dgvSessions.Columns.Count) return;
            var col = dgvSessions.Columns[e.ColumnIndex];
            var colName = col.Name ?? col.HeaderText;
            // Determine action by column
            if (colName == "Date")
            {
                var defaultFrom = headerFilterFrom ?? (sessions.Any() ? sessions.Min(s => s.Date).Date : DateTime.Now.Date.AddDays(-7));
                var defaultTo = headerFilterTo ?? (sessions.Any() ? sessions.Max(s => s.Date).Date : DateTime.Now.Date);
                using var f = new DateRangeForm(defaultFrom, defaultTo);
                if (f.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    headerFilterFrom = f.From;
                    headerFilterTo = f.To;
                    RefreshSessionsGrid();
                }
            }
            else if (colName == "Participants")
            {
                // Toggle participants sort order (descending <-> ascending)
                headerParticipantsDesc = !headerParticipantsDesc;
                RefreshSessionsGrid();
            }
            else if (colName == "Coach" || colName == "TrainingType" || colName == "Place")
            {
                // collect distinct values for this column from sessions
                var values = new List<string>();
                foreach (var s in sessions)
                {
                    string? v = null;
                    if (colName == "Coach") v = s.Coach?.FullName;
                    else if (colName == "TrainingType") v = s.Coach?.Specialization;
                    else if (colName == "Place") v = string.IsNullOrEmpty(s.Location) ? s.Facility?.Name : s.Location;
                    if (!string.IsNullOrWhiteSpace(v) && !values.Contains(v)) values.Add(v);
                }
                values.Sort();
                var menu = new ContextMenuStrip();
                menu.Items.Add("(All)").Click += (ss, ee) => { if (colName == "Coach") headerFilterCoach = null; else if (colName == "TrainingType") headerFilterType = null; else if (colName == "Place") headerFilterPlace = null; RefreshSessionsGrid(); };
                menu.Items.Add(new ToolStripSeparator());
                foreach (var val in values)
                {
                    var item = menu.Items.Add(val);
                    item.Click += (ss, ee) => { if (colName == "Coach") headerFilterCoach = val; else if (colName == "TrainingType") headerFilterType = val; else if (colName == "Place") headerFilterPlace = val; RefreshSessionsGrid(); };
                }
                menu.Show(Cursor.Position);
            }
        }

        private void BtnApplyFilters_Click(object? sender, EventArgs e)
        {
            RefreshSessionsGrid();
        }

        private void BtnClearFilters_Click(object? sender, EventArgs e)
        {
            // clear header-based filters
            headerFilterFrom = null;
            headerFilterTo = null;
            headerFilterCoach = null;
            headerFilterType = null;
            headerFilterPlace = null;
            headerFilterMinParticipants = null;
            // also safely clear any UI filter controls if present
            if (dtpFilterFrom != null) dtpFilterFrom.Value = DateTime.Now.Date;
            if (dtpFilterTo != null) dtpFilterTo.Value = DateTime.Now.Date;
            RefreshSessionsGrid();
        }

        private void BtnCalendar_Click(object? sender, EventArgs e)
        {
            if (dgvMembers!.CurrentRow == null) { MessageBox.Show("Select a member"); return; }
            var idx = dgvMembers!.CurrentRow!.Index;
            if (idx < 0 || idx >= members.Count) return;
            var m = members[idx];
            using var f = new MemberCalendarForm(m, sessions);
            f.ShowDialog();
        }

        private void RefreshFacilitiesGrid()
        {
            if (dgvFacilities == null) return; // facilities tab removed; nothing to refresh
            dgvFacilities!.DataSource = null;
            var view = facilities.Select(f => new { f.Name, f.Type, f.Capacity }).ToList();
            dgvFacilities!.DataSource = view;
        }

        private void RefreshAllGrids()
        {
            RefreshMembersGrid();
            RefreshTrainersGrid();
            RefreshSessionsGrid();
            // only refresh facilities grid if it exists in UI
            if (dgvFacilities != null) RefreshFacilitiesGrid();
        }

        // Trainer handlers
        private void BtnAddTrainer_Click(object? sender, EventArgs e)
        {
            using var f = new TrainerForm();
            if (f.ShowDialog() == DialogResult.OK)
            {
                trainers.Add(f.Trainer);
                RefreshTrainersGrid();
            }
        }

        private void BtnEditTrainer_Click(object? sender, EventArgs e)
        {
            if (dgvTrainers!.CurrentRow == null) return;
            var idx = dgvTrainers!.CurrentRow!.Index;
            if (idx < 0 || idx >= trainers.Count) return;
            var t = trainers[idx];
            using var f = new TrainerForm();
            f.Trainer = new Trainer(t.FullName, t.Specialization, t.ExperienceYears);
            if (f.ShowDialog() == DialogResult.OK)
            {
                trainers[idx] = f.Trainer;
                RefreshTrainersGrid();
            }
        }

        private void BtnDeleteTrainer_Click(object? sender, EventArgs e)
        {
            if (dgvTrainers!.CurrentRow == null) return;
            var idx = dgvTrainers!.CurrentRow!.Index;
            if (idx < 0 || idx >= trainers.Count) return;
            if (MessageBox.Show("Delete trainer?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                trainers.RemoveAt(idx);
                RefreshTrainersGrid();
            }
        }

        // Session handlers
        private void BtnAddSession_Click(object? sender, EventArgs e)
        {
            using var f = new SessionForm(trainers, members, facilities);
            if (f.ShowDialog() == DialogResult.OK)
            {
                sessions.Add(f.Session);
                RefreshAllGrids();
            }
        }

        private void BtnEditSession_Click(object? sender, EventArgs e)
        {
            if (dgvSessions!.CurrentRow == null) return;
            // Find matching session from filtered grid by comparing displayed values
            var row = dgvSessions!.CurrentRow;
            var dateStr = row.Cells["Date"]?.Value?.ToString() ?? string.Empty;
            var coachStr = row.Cells["Coach"]?.Value?.ToString() ?? string.Empty;
            var participantsStr = row.Cells["Participants"]?.Value?.ToString() ?? "0";
            
            if (!int.TryParse(participantsStr, out var displayParticipants)) return;
            
            // Match by date, coach, and participant count to handle filtered/sorted grids correctly
            var s = sessions.FirstOrDefault(x => 
                x.Date.ToString("g") == dateStr && 
                (x.Coach?.FullName ?? "") == coachStr &&
                (x.Participants?.Count ?? 0) == displayParticipants);
            
            if (s == null) return;

            using var f = new SessionForm(trainers, members, facilities);
            f.Session = new TrainingSession(s.Date, s.Coach, s.Facility) { Participants = s.Participants, Facility = s.Facility, Location = s.Location };
            if (f.ShowDialog() == DialogResult.OK)
            {
                var idx = sessions.IndexOf(s);
                if (idx >= 0) sessions[idx] = f.Session;
                RefreshAllGrids();
            }
        }

        private void BtnDeleteSession_Click(object? sender, EventArgs e)
        {
            if (dgvSessions!.CurrentRow == null) return;
            // Find matching session from filtered grid by comparing displayed values
            var row = dgvSessions!.CurrentRow;
            var dateStr = row.Cells["Date"]?.Value?.ToString() ?? string.Empty;
            var coachStr = row.Cells["Coach"]?.Value?.ToString() ?? string.Empty;
            var participantsStr = row.Cells["Participants"]?.Value?.ToString() ?? "0";
            
            if (!int.TryParse(participantsStr, out var displayParticipants)) return;
            
            // Match by date, coach, and participant count to handle filtered/sorted grids correctly
            var s = sessions.FirstOrDefault(x => 
                x.Date.ToString("g") == dateStr && 
                (x.Coach?.FullName ?? "") == coachStr &&
                (x.Participants?.Count ?? 0) == displayParticipants);
            
            if (s == null) return;

            if (MessageBox.Show("Delete session?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                sessions.Remove(s);
                RefreshAllGrids();
            }
        }

        // Facility handlers
        private void BtnAddFacility_Click(object? sender, EventArgs e)
        {
            using var f = new FacilityForm();
            if (f.ShowDialog() == DialogResult.OK)
            {
                facilities.Add(f.Facility);
                RefreshFacilitiesGrid();
            }
        }

        private void BtnEditFacility_Click(object? sender, EventArgs e)
        {
            if (dgvFacilities!.CurrentRow == null) return;
            var idx = dgvFacilities!.CurrentRow!.Index;
            if (idx < 0 || idx >= facilities.Count) return;
            var it = facilities[idx];
            using var f = new FacilityForm();
            f.Facility = new Facility(it.Name, it.Type, it.Capacity);
            if (f.ShowDialog() == DialogResult.OK)
            {
                facilities[idx] = f.Facility;
                RefreshFacilitiesGrid();
            }
        }

        private void BtnDeleteFacility_Click(object? sender, EventArgs e)
        {
            if (dgvFacilities!.CurrentRow == null) return;
            var idx = dgvFacilities!.CurrentRow!.Index;
            if (idx < 0 || idx >= facilities.Count) return;
            if (MessageBox.Show("Delete facility?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                facilities.RemoveAt(idx);
                RefreshFacilitiesGrid();
            }
        }

        private void BtnExportSessions_Click(object? sender, EventArgs e)
        {
            using var dlg = new SaveFileDialog();
            dlg.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
            dlg.FileName = "attendance_report.csv";
            if (dlg.ShowDialog() != DialogResult.OK) return;
            try
            {
                ReportsService.GenerateAttendanceCsv(dlg.FileName, sessions);
                MessageBox.Show("Report saved: " + dlg.FileName, "Export", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to save report: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
