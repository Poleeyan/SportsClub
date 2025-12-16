using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using SportsClub.Models;

namespace SportsClub
{
    public partial class TrainersManagerForm : Form
    {
        private List<Trainer> trainers;

        public TrainersManagerForm(List<Trainer> trainers)
        {
            this.trainers = trainers;
            InitializeComponent();
        }

        private void TrainersManagerForm_Load(object sender, EventArgs e)
        {
            RefreshGrid();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            using var f = new TrainerForm();
            if (f.ShowDialog() == DialogResult.OK)
            {
                trainers.Add(f.Trainer);
                RefreshGrid();
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dgv.CurrentRow == null) return;
            var idx = dgv.CurrentRow.Index;
            if (idx < 0 || idx >= trainers.Count) return;
            var t = trainers[idx];
            using var f = new TrainerForm();
            f.Trainer = new Trainer(t.FullName, t.Specialization, t.ExperienceYears);
            if (f.ShowDialog() == DialogResult.OK)
            {
                trainers[idx] = f.Trainer;
                RefreshGrid();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgv.CurrentRow == null) return;
            var idx = dgv.CurrentRow.Index;
            if (idx < 0 || idx >= trainers.Count) return;
            if (MessageBox.Show("Delete trainer?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                trainers.RemoveAt(idx);
                RefreshGrid();
            }
        }

        private void RefreshGrid()
        {
            dgv.DataSource = null;
            dgv.DataSource = trainers.Select(t => new { t.FullName, t.Specialization, t.ExperienceYears }).ToList();
        }
    }
}
