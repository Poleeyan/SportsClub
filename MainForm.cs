using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using SportsClub.Models;
using SportsClub.Services;

namespace SportsClub
{
    public partial class MainForm : Form
    {
        private List<Member> members = [];
        private List<Trainer> trainers = [];
        private List<TrainingSession> sessions = [];
        private readonly List<Facility> facilities = [];

        public MainForm()
        {
            // static facilities: зал, басейн, тенісний корт
            facilities =
            [
                new("Зал", "Hall", 50),
                new("Басейн", "Pool", 20),
                new("Тенісний корт", "Tennis Court", 4)
            ];
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
                    Subscription = m.Subscription,
                    PurchasedDays = m.PurchasedDays
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
                Days = m.PurchasedDays,
                Price = m.Subscription != null ? m.Subscription.GetPrice(m.PurchasedDays, 0) : 0.0
            }).ToList();
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
            var view = sessions.Select(s => new {
                Date = s.Date.ToString("g"),
                Coach = s.Coach?.FullName,
                TrainingType = s.Coach?.Specialization,
                Participants = s.Participants?.Count ?? 0,
                Place = string.IsNullOrEmpty(s.Location) ? s.Facility?.Name : s.Location
            }).ToList();
            dgvSessions!.DataSource = view;
            // rename TrainingType column header to the requested Ukrainian label
            var _ttCol = dgvSessions!.Columns["TrainingType"];
            if (_ttCol != null)
            {
                _ttCol.HeaderText = "Тренерування";
            }
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
            var idx = dgvSessions!.CurrentRow!.Index;
            if (idx < 0 || idx >= sessions.Count) return;
            var s = sessions[idx];
            using var f = new SessionForm(trainers, members, facilities);
            f.Session = new TrainingSession(s.Date, s.Coach, s.Facility) { Participants = s.Participants, Facility = s.Facility, Location = s.Location };
            if (f.ShowDialog() == DialogResult.OK)
            {
                sessions[idx] = f.Session;
                RefreshAllGrids();
            }
        }

        private void BtnDeleteSession_Click(object? sender, EventArgs e)
        {
            if (dgvSessions!.CurrentRow == null) return;
            var idx = dgvSessions!.CurrentRow!.Index;
            if (idx < 0 || idx >= sessions.Count) return;
            if (MessageBox.Show("Delete session?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                sessions.RemoveAt(idx);
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
    }
}
