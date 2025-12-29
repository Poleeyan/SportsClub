using System;
using System.Windows.Forms;
using System.ComponentModel;
using SportsClub.Models;

namespace SportsClub
{
    public partial class TrainerForm : Form
    {
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Trainer Trainer { get; set; } = new Trainer();

        public TrainerForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            txtName.Text = Trainer.FullName;
            cbSpec.Items.Clear();
            cbSpec.Items.AddRange(SpecializationMapping.AllSpecializations.ToArray());
            if (!string.IsNullOrWhiteSpace(Trainer.Specialization) && cbSpec.Items.Contains(Trainer.Specialization)) cbSpec.SelectedItem = Trainer.Specialization;
            else if (cbSpec.Items.Count > 0) cbSpec.SelectedIndex = 0;
            nudExp.Value = Trainer.ExperienceYears;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            Trainer.FullName = txtName.Text.Trim();
            Trainer.Specialization = cbSpec.SelectedItem?.ToString() ?? string.Empty;
            Trainer.ExperienceYears = (int)nudExp.Value;
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
