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
        private List<Member> members = new();
        private List<Trainer> trainers = new();
        private List<TrainingSession> sessions = new();
        private List<Facility> facilities = new();

        public MainForm()
        {
            // static facilities: зал, басейн, тенісний корт
            facilities = new List<Facility>
            {
                new Facility("Зал", "Hall", 50),
                new Facility("Басейн", "Pool", 20),
                new Facility("Тенісний корт", "Tennis Court", 4)
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
            var idx = dgvMembers.CurrentRow.Index;
            if (idx < 0 || idx >= members.Count) return;
            var m = members[idx];
            var form = new MemberForm();
            form.Member = new Member(m.FullName)
            {
                Id = m.Id,
                Registered = m.Registered,
                Visits = m.Visits,
                Subscription = m.Subscription,
                PurchasedDays = m.PurchasedDays
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
            var idx = dgvMembers.CurrentRow.Index;
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
            var view = members.Select(m => new
            {
                m.FullName,
                Visits = (m.Subscription != null && m.Subscription.Type == "Premium") ? "Unlimited" : (object)m.GetVisits(),
                Subscription = m.Subscription?.Type,
                Days = m.PurchasedDays,
                Price = m.Subscription != null ? m.Subscription.GetPrice(m.PurchasedDays, m.GetVisits()) : 0.0
            }).ToList();
            dgvMembers!.DataSource = view;
            // format Price column as currency if present
            if (dgvMembers.Columns.Contains("Price"))
            {
                dgvMembers.Columns["Price"].DefaultCellStyle.Format = "C2";
                dgvMembers.Columns["Price"].DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
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
            var view = sessions.Select(s => new { Date = s.Date.ToString("g"), Coach = s.Coach?.FullName, Participants = s.Participants?.Count ?? 0, Place = string.IsNullOrEmpty(s.Location) ? s.Facility?.Name : s.Location }).ToList();
            dgvSessions!.DataSource = view;
        }

        private void RefreshFacilitiesGrid()
        {
            if (dgvFacilities == null) return; // facilities tab removed; nothing to refresh
            dgvFacilities.DataSource = null;
            var view = facilities.Select(f => new { f.Name, f.Type, f.Capacity }).ToList();
            dgvFacilities.DataSource = view;
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
            var idx = dgvTrainers.CurrentRow.Index;
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
            var idx = dgvTrainers.CurrentRow.Index;
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
                RefreshSessionsGrid();
            }
        }

        private void BtnEditSession_Click(object? sender, EventArgs e)
        {
            if (dgvSessions!.CurrentRow == null) return;
            var idx = dgvSessions.CurrentRow.Index;
            if (idx < 0 || idx >= sessions.Count) return;
            var s = sessions[idx];
            using var f = new SessionForm(trainers, members, facilities);
            f.Session = new TrainingSession(s.Date, s.Coach, s.Facility) { Participants = s.Participants, Facility = s.Facility, Location = s.Location };
            if (f.ShowDialog() == DialogResult.OK)
            {
                sessions[idx] = f.Session;
                RefreshSessionsGrid();
            }
        }

        private void BtnDeleteSession_Click(object? sender, EventArgs e)
        {
            if (dgvSessions!.CurrentRow == null) return;
            var idx = dgvSessions.CurrentRow.Index;
            if (idx < 0 || idx >= sessions.Count) return;
            if (MessageBox.Show("Delete session?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                sessions.RemoveAt(idx);
                RefreshSessionsGrid();
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
            var idx = dgvFacilities.CurrentRow.Index;
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
            var idx = dgvFacilities.CurrentRow.Index;
            if (idx < 0 || idx >= facilities.Count) return;
            if (MessageBox.Show("Delete facility?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                facilities.RemoveAt(idx);
                RefreshFacilitiesGrid();
            }
        }
    }
}
