using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.ComponentModel;
using SportsClub.Models;

namespace SportsClub
{
    public partial class SessionForm : Form
    {
        private List<Trainer> trainers;
        private List<Member> members;
        private List<Facility> facilities;
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public TrainingSession Session { get; set; } = new TrainingSession();

        public SessionForm(List<Trainer> trainers, List<Member> members, List<Facility> facilities)
        {
            this.trainers = trainers;
            this.members = members;
            this.facilities = facilities;
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            dtpDate.Value = Session.Date == default ? DateTime.Now : Session.Date;
            cbTrainer.Items.Clear();
            foreach (var t in trainers) cbTrainer.Items.Add(t.FullName);
            if (Session.Coach != null) cbTrainer.SelectedItem = Session.Coach.FullName;
            else if (cbTrainer.Items.Count > 0) cbTrainer.SelectedIndex = 0;
            // populate facilities
            cbFacility.Items.Clear();
            if (facilities != null)
            {
                cbFacility.DisplayMember = "Name";
                foreach (var f in facilities) cbFacility.Items.Add(f);
                if (Session.Facility != null)
                {
                    var sel = facilities.FirstOrDefault(x => x.Name == Session.Facility.Name && x.Type == Session.Facility.Type);
                    if (sel != null) cbFacility.SelectedItem = sel;
                }
                else if (cbFacility.Items.Count > 0) cbFacility.SelectedIndex = 0;
            }

            // populate members list according to selected facility
            UpdateMembersListForSelectedFacility();
        }

        private void cbFacility_SelectedIndexChanged(object? sender, EventArgs e)
        {
            UpdateMembersListForSelectedFacility();
        }

        private void UpdateMembersListForSelectedFacility()
        {
            clbMembers.Items.Clear();
            if (members == null) return;

            var selFacility = cbFacility.SelectedItem as Facility;
            bool premiumOnly = false;
            if (selFacility != null)
            {
                var txt = ((selFacility.Type ?? "") + " " + (selFacility.Name ?? "")).ToLowerInvariant();
                premiumOnly = txt.Contains("pool") || txt.Contains("басейн") || txt.Contains("tennis") || txt.Contains("теніс") || txt.Contains("корт");
            }

            var list = premiumOnly ? members.Where(m => m.Subscription != null && m.Subscription.Type == "Premium").ToList() : members;
            foreach (var m in list) clbMembers.Items.Add(m.FullName);

            // restore checked state from Session.Participants if any
            if (Session.Participants != null && Session.Participants.Count > 0)
            {
                for (int i = 0; i < clbMembers.Items.Count; i++)
                {
                    var name = clbMembers.Items[i].ToString();
                    if (Session.Participants.Any(p => p.FullName == name)) clbMembers.SetItemChecked(i, true);
                }
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            Session.Date = dtpDate.Value;
            var sel = cbTrainer.SelectedItem?.ToString();
            Session.Coach = trainers.FirstOrDefault(t => t.FullName == sel);
            // collect selected participants
            var selected = new List<Member>();
            if (members != null)
            {
                for (int i = 0; i < clbMembers.Items.Count; i++)
                {
                    if (clbMembers.GetItemChecked(i))
                    {
                        var name = clbMembers.Items[i].ToString();
                        var m = members.FirstOrDefault(x => x.FullName == name);
                        if (m != null) selected.Add(m);
                    }
                }
            }

            // facility selection
            var selFacility = cbFacility.SelectedItem as Facility;
            if (selFacility != null)
            {
                // premium-only check for pool/tennis (support both English and Ukrainian names)
                var txt = ((selFacility.Type ?? "") + " " + (selFacility.Name ?? "")).ToLowerInvariant();
                bool premiumOnly = txt.Contains("pool") || txt.Contains("басейн") || txt.Contains("tennis") || txt.Contains("теніс") || txt.Contains("корт");
                if (premiumOnly)
                {
                    var notPremium = selected.Where(m => m.Subscription == null || m.Subscription.Type != "Premium").ToList();
                    if (notPremium.Any())
                    {
                        MessageBox.Show("Training in pool or tennis court is allowed only for Premium members. Remove non-premium participants or choose another place.");
                        return;
                    }
                }
            }

            Session.Participants = selected;
            Session.Facility = selFacility;
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
