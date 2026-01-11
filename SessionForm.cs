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
        private readonly List<Trainer> trainers;
        private readonly List<Member> members;
        private readonly List<Facility> facilities;
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
            cbTrainer.SelectedIndexChanged += cbTrainer_SelectedIndexChanged;
            // populate facilities according to selected trainer
            PopulateFacilitiesForSelectedTrainer();

            // populate members list according to selected facility
            UpdateMembersListForSelectedFacility();
        }

        private void cbFacility_SelectedIndexChanged(object? sender, EventArgs e)
        {
            UpdateMembersListForSelectedFacility();
        }

        private void cbTrainer_SelectedIndexChanged(object? sender, EventArgs e)
        {
            PopulateFacilitiesForSelectedTrainer();
        }

        private void PopulateFacilitiesForSelectedTrainer()
        {
            cbFacility.Items.Clear();
            if (facilities == null) return;
            // find selected trainer
            var trainerName = cbTrainer.SelectedItem?.ToString();
            Trainer? selTrainer = null;
            if (!string.IsNullOrWhiteSpace(trainerName)) selTrainer = trainers.FirstOrDefault(t => t.FullName == trainerName);

            IEnumerable<Facility> listToShow;
            if (selTrainer != null)
            {
                listToShow = SpecializationMapping.FilterFacilitiesForTrainer(selTrainer, facilities);
            }
            else
            {
                listToShow = facilities;
            }

            cbFacility.DisplayMember = "Name";
            foreach (var f in listToShow) cbFacility.Items.Add(f);

            // try to restore previously selected facility
            if (Session.Facility != null)
            {
                var sel = listToShow.FirstOrDefault(x => x.Name == Session.Facility.Name && x.Type == Session.Facility.Type);
                if (sel != null) cbFacility.SelectedItem = sel;
            }
            if (cbFacility.SelectedItem == null)
            {
                if (!string.IsNullOrWhiteSpace(Session.Location))
                {
                    var selByName = listToShow.FirstOrDefault(x => x.Name == Session.Location);
                    if (selByName != null) cbFacility.SelectedItem = selByName;
                }
                if (cbFacility.Items.Count > 0 && cbFacility.SelectedItem == null) cbFacility.SelectedIndex = 0;
            }

            UpdateMembersListForSelectedFacility();
        }

        private void UpdateMembersListForSelectedFacility()
        {
            clbMembers.Items.Clear();
            if (members == null) return;

            int requiredLevel = 1;
            if (cbFacility.SelectedItem is Facility selFacility)
            {
                requiredLevel = selFacility.RequiredAccessLevel;
            }

            var list = members.Where(m =>
            {
                var access = m.Subscription as IAccessLevel;
                return access == null || access.GetAccessLevel() >= requiredLevel;
            }).ToList();
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
                // Перевірка рівня доступу через GetAccessLevel()
                int requiredLevel = selFacility.RequiredAccessLevel;
                var insufficientAccess = selected.Where(m =>
                {
                    var access = m.Subscription as IAccessLevel;
                    return access == null || access.GetAccessLevel() < requiredLevel;
                }).ToList();
                if (insufficientAccess.Any())
                {
                    MessageBox.Show($"Це приміщення ({selFacility.Name}) вимагає рівень доступу {requiredLevel}. Видаліть учасників без необхідної підписки або оберіть інше місце.");
                    return;
                }
            }

            Session.Participants = selected;
            Session.Facility = selFacility;
            Session.Location = selFacility?.Name ?? Session.Location;
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
