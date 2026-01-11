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
            cbSubscription.Items.Add("Basic — Monthly");
            cbSubscription.Items.Add("Basic — Annual");
            cbSubscription.Items.Add("Premium — Monthly");
            cbSubscription.Items.Add("Premium — Annual");
            if (Member.Subscription != null)
            {
                // Member.Subscription.Type already contains a descriptive label
                // Try to select matching item, fallback to first
                var match = cbSubscription.Items.Cast<object?>().FirstOrDefault(i => (i?.ToString() ?? string.Empty) == Member.Subscription.Type);
                if (match != null) cbSubscription.SelectedItem = match; else cbSubscription.SelectedIndex = 0;
            }
            else cbSubscription.SelectedIndex = 0;
            // initialize start date picker from Member.Registered
            try { dtpStart.Value = Member.Registered; } catch { dtpStart.Value = DateTime.Now; }
            // days are calculated from registration/start date
            var elapsed = (DateTime.Now - dtpStart.Value).Days;
            if (elapsed < 0) elapsed = 0;
            dtpStart.ValueChanged += (s, ee) => UpdatePriceDisplay();
            UpdatePriceDisplay();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            Member.FullName = txtName.Text.Trim();
            var sel = cbSubscription.SelectedItem?.ToString() ?? string.Empty;
            if (sel.StartsWith("Premium"))
            {
                Member.Subscription = sel.Contains("Annual") ? new PremiumMembership(Membership.MembershipPeriod.Annual) as Membership : new PremiumMembership(Membership.MembershipPeriod.Monthly) as Membership;
            }
            else
            {
                Member.Subscription = sel.Contains("Annual") ? new BasicMembership(Membership.MembershipPeriod.Annual) as Membership : new BasicMembership(Membership.MembershipPeriod.Monthly) as Membership;
            }
            // Purchased days are no longer set manually; keep existing value for compatibility
            // Member.PurchasedDays remains unchanged so legacy data is preserved
            // update registration/start date from picker
            Member.Registered = dtpStart.Value;
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

        // nudDays removed from UI; handler intentionally removed


        private void UpdatePriceDisplay()
        {
            try
            {
                var sel = cbSubscription.SelectedItem?.ToString() ?? string.Empty;
                Membership? temp = null;
                if (sel.StartsWith("Premium")) temp = sel.Contains("Annual") ? new PremiumMembership(Membership.MembershipPeriod.Annual) : new PremiumMembership(Membership.MembershipPeriod.Monthly);
                else temp = sel.Contains("Annual") ? new BasicMembership(Membership.MembershipPeriod.Annual) : new BasicMembership(Membership.MembershipPeriod.Monthly);
                if (temp == null) { lblPrice.Text = "0.00"; return; }
                // show plan price in the form preview (full period price)
                var price = temp.Price;
                lblPrice.Text = price.ToString("C");
            }
            catch
            {
                lblPrice.Text = "0.00";
            }
        }

        
    }
}
