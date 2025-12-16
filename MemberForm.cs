using System;
using System.Windows.Forms;
using System.ComponentModel;
using SportsClub.Models;

namespace SportsClub
{
    public partial class MemberForm : Form
    {
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Member Member { get; set; } = new Member();

        public MemberForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            txtName.Text = Member.FullName;
            nudVisits.Value = Member.Visits;
            cbSubscription.Items.Clear();
            cbSubscription.Items.Add("Basic");
            cbSubscription.Items.Add("Premium");
            if (Member.Subscription != null)
            {
                cbSubscription.SelectedItem = Member.Subscription.Type;
            }
            else cbSubscription.SelectedIndex = 0;
            // set checkbox state
            if (this.Controls.ContainsKey("chkActive") && chkActive != null)
            {
                chkActive.Checked = Member.IsActive;
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            Member.FullName = txtName.Text.Trim();
            Member.Visits = (int)nudVisits.Value;
            var sel = cbSubscription.SelectedItem?.ToString();
            if (sel == "Premium") Member.Subscription = new PremiumMembership(); else Member.Subscription = new BasicMembership();
            if (this.Controls.ContainsKey("chkActive") && chkActive != null) Member.IsActive = chkActive.Checked;
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
