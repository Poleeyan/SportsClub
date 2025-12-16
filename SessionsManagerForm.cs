using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using SportsClub.Models;

namespace SportsClub
{
    public partial class SessionsManagerForm : Form
    {
        private List<TrainingSession> sessions;
        private List<Trainer> trainers;
        private List<Member> members;
        private List<Facility> facilities;

        public SessionsManagerForm(List<TrainingSession> sessions, List<Trainer> trainers, List<Member> members, List<Facility> facilities)
        {
            this.sessions = sessions;
            this.trainers = trainers;
            this.members = members;
            this.facilities = facilities;
            InitializeComponent();
        }

        private void SessionsManagerForm_Load(object sender, EventArgs e)
        {
            RefreshGrid();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            using var f = new SessionForm(trainers, members, facilities);
            if (f.ShowDialog() == DialogResult.OK)
            {
                sessions.Add(f.Session);
                RefreshGrid();
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dgv.CurrentRow == null) return;
            var idx = dgv.CurrentRow.Index;
            if (idx < 0 || idx >= sessions.Count) return;
            var s = sessions[idx];
            using var f = new SessionForm(trainers, members, facilities);
            f.Session = new TrainingSession(s.Date, s.Coach, s.Facility) { Participants = s.Participants, Facility = s.Facility };
            if (f.ShowDialog() == DialogResult.OK)
            {
                sessions[idx] = f.Session;
                RefreshGrid();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgv.CurrentRow == null) return;
            var idx = dgv.CurrentRow.Index;
            if (idx < 0 || idx >= sessions.Count) return;
            if (MessageBox.Show("Delete session?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                sessions.RemoveAt(idx);
                RefreshGrid();
            }
        }

        private void RefreshGrid()
        {
            dgv.DataSource = null;
            dgv.DataSource = sessions.Select(s => new { Date = s.Date.ToString("g"), Coach = s.Coach?.FullName, Participants = s.Participants?.Count ?? 0 }).ToList();
        }
    }
}
