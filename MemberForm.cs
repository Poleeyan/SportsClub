using System;
using System.Windows.Forms;
using System.ComponentModel;
using System.Linq;
using System.Collections.Generic;
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
            
            cbSubscription.Items.Clear();
            cbSubscription.Items.Add("Basic");
            cbSubscription.Items.Add("Premium");
            if (Member.Subscription != null)
            {
                cbSubscription.SelectedItem = Member.Subscription.Type;
            }
            else cbSubscription.SelectedIndex = 0;
            // initialize days control and price display
            // set initial days value: prefer PurchasedDays if present, otherwise subscription default
            if (Member.PurchasedDays > 0)
            {
                nudDays.Value = Member.PurchasedDays;
            }
            else if (Member.Subscription != null && Member.Subscription.DurationDays > 0)
            {
                nudDays.Value = Member.Subscription.DurationDays;
            }
            else
            {
                nudDays.Value = 30;
            }
            UpdatePriceDisplay();
            // set checkbox state
            if (this.Controls.ContainsKey("chkActive") && chkActive != null)
            {
                chkActive.Checked = Member.IsActive;
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            Member.FullName = txtName.Text.Trim();
            var sel = cbSubscription.SelectedItem?.ToString();
            if (sel == "Premium") Member.Subscription = new PremiumMembership(); else Member.Subscription = new BasicMembership();
            // store purchased days on member (do not mutate subscription plan)
            Member.PurchasedDays = (int)nudDays.Value;
            if (this.Controls.ContainsKey("chkActive") && chkActive != null) Member.IsActive = chkActive.Checked;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void cbSubscription_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdatePriceDisplay();
        }

        private void nudDays_ValueChanged(object sender, EventArgs e)
        {
            UpdatePriceDisplay();
        }


        private void UpdatePriceDisplay()
        {
            try
            {
                var sel = cbSubscription.SelectedItem?.ToString();
                Membership? temp = null;
                if (sel == "Premium") temp = new PremiumMembership(); else temp = new BasicMembership();
                if (temp == null) { lblPrice.Text = "0.00"; return; }
                var days = (int)nudDays.Value;
                var price = temp.GetPrice(days, 0);
                lblPrice.Text = price.ToString("C");
            }
            catch
            {
                lblPrice.Text = "0.00";
            }
        }

        
    }
}
