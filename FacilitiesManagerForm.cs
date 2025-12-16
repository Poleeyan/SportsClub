using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using SportsClub.Models;

namespace SportsClub
{
    public partial class FacilitiesManagerForm : Form
    {
        private List<Facility> facilities;

        public FacilitiesManagerForm(List<Facility> facilities)
        {
            this.facilities = facilities;
            InitializeComponent();
        }

        private void FacilitiesManagerForm_Load(object sender, EventArgs e)
        {
            RefreshGrid();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            using var f = new FacilityForm();
            if (f.ShowDialog() == DialogResult.OK)
            {
                facilities.Add(f.Facility);
                RefreshGrid();
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dgv.CurrentRow == null) return;
            var idx = dgv.CurrentRow.Index;
            if (idx < 0 || idx >= facilities.Count) return;
            var it = facilities[idx];
            using var f = new FacilityForm();
            f.Facility = new Facility(it.Name, it.Type, it.Capacity);
            if (f.ShowDialog() == DialogResult.OK)
            {
                facilities[idx] = f.Facility;
                RefreshGrid();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgv.CurrentRow == null) return;
            var idx = dgv.CurrentRow.Index;
            if (idx < 0 || idx >= facilities.Count) return;
            if (MessageBox.Show("Delete facility?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                facilities.RemoveAt(idx);
                RefreshGrid();
            }
        }

        private void RefreshGrid()
        {
            dgv.DataSource = null;
            dgv.DataSource = facilities.Select(f => new { f.Name, f.Type, f.Capacity }).ToList();
        }
    }
}
