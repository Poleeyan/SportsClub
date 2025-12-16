using System;
using System.Windows.Forms;
using System.ComponentModel;
using SportsClub.Models;

namespace SportsClub
{
    public partial class FacilityForm : Form
    {
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Facility Facility { get; set; } = new Facility();

        public FacilityForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            txtName.Text = Facility.Name;
            txtType.Text = Facility.Type;
            nudCap.Value = Facility.Capacity;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            Facility.Name = txtName.Text.Trim();
            Facility.Type = txtType.Text.Trim();
            Facility.Capacity = (int)nudCap.Value;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
