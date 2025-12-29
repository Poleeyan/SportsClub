using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using SportsClub.Models;
using SportsClub.Services;

namespace SportsClub
{
    public partial class SessionsManagerForm : Form
    {
        private readonly List<TrainingSession> sessions;
        private readonly List<Trainer> trainers;
        private readonly List<Member> members;
        private readonly List<Facility> facilities;

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
            f.Session = new TrainingSession(s.Date, s.Coach, s.Facility) { Participants = s.Participants, Facility = s.Facility, Location = s.Location };
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
            dgv.DataSource = sessions.Select(s => new { Date = s.Date.ToString("g"), Coach = s.Coach?.FullName, Participants = s.Participants?.Count ?? 0, Place = string.IsNullOrEmpty(s.Location) ? s.Facility?.Name : s.Location }).ToList();
        }

        private void btnExport_Click(object sender, EventArgs e)
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
