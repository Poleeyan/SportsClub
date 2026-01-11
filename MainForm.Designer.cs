#nullable enable
using System;
using System.Windows.Forms;
using System.Collections.Generic;

namespace SportsClub
{
    partial class MainForm
    {
        // Requirement notes:
        // - GUI with multiple dialogs and controls (Requirement 1,15)
        // - Uses TabControl and per-entity DataGridViews
        private System.ComponentModel.IContainer? components = null;
        private DataGridView? dgvMembers;
        private DataGridView? dgvTrainers;
        private DataGridView? dgvSessions;
        private RadioButton? rbSessionsActive;
        private RadioButton? rbSessionsPast;
        private DataGridView? dgvFacilities;
        private DateTimePicker? dtpFilterFrom;
        private DateTimePicker? dtpFilterTo;
        private Button? btnApplyFilters;
        private Button? btnClearFilters;
        
        private Button? btnSaveXml, btnLoadXml;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.Text = "Sports Club Manager";
            this.ClientSize = new System.Drawing.Size(980, 660);

            var root = new TableLayoutPanel();
            root.Dock = DockStyle.Fill;
            root.RowCount = 2;
            root.ColumnCount = 1;
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            this.Controls.Add(root);

            // persistence buttons (Save/Load XML) placed above tabs
            var persistFlow = new FlowLayoutPanel { AutoSize = true, FlowDirection = FlowDirection.LeftToRight, Margin = new Padding(6) };
            btnSaveXml = new Button { Text = "Save XML", AutoSize = true, Margin = new Padding(6) };
            btnSaveXml.Click += (s, e) => { try { Services.PersistenceService.SaveStateXml("state.xml", members, trainers, sessions, facilities); MessageBox.Show("Saved xml"); } catch (Exception ex) { MessageBox.Show(ex.Message); } };
            persistFlow.Controls.Add(btnSaveXml);
            btnLoadXml = new Button { Text = "Load XML", AutoSize = true, Margin = new Padding(6) };
            btnLoadXml.Click += (s, e) => { try {
                    var state = Services.PersistenceService.LoadStateXml("state.xml");
                    members = state.Members ?? new List<Models.Member>();
                    trainers = state.Trainers ?? new List<Models.Trainer>();
                    sessions = state.Sessions ?? new List<Models.TrainingSession>();
                    // re-associate session.Facility instances with the app's static facilities list
                    foreach (var ses in sessions)
                    {
                        if (ses.Facility != null)
                        {
                            var found = facilities.FirstOrDefault(f => f.Name == ses.Facility.Name && f.Type == ses.Facility.Type);
                            if (found != null) ses.Facility = found;
                        }
                        // ensure Location is set (prefer explicit Location from XML, otherwise facility name)
                        if (string.IsNullOrWhiteSpace(ses.Location) && ses.Facility != null) ses.Location = ses.Facility.Name;
                    }
                    RefreshAllGrids();
                } catch (Exception ex) { MessageBox.Show(ex.Message); } };
            persistFlow.Controls.Add(btnLoadXml);
            root.Controls.Add(persistFlow, 0, 0);

            // old single DataGridView removed in favor of TabControl with per-entity grids

            // create tab control
            var tabs = new TabControl { Dock = DockStyle.Fill, Margin = new Padding(6) };

            var tabMembers = new TabPage("Members");
            var tabTrainers = new TabPage("Trainers");
            var tabSessions = new TabPage("Sessions");

            // Members grid and member-specific controls panel
            var membersPanel = new Panel { Dock = DockStyle.Fill };
            var membersTop = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true, Height = 48 };
            var btnAddMember = new Button { Text = "Add Member", AutoSize = true, Margin = new Padding(6) };
            btnAddMember.Click += BtnAdd_Click;
            var btnEditMember = new Button { Text = "Edit Selected", AutoSize = true, Margin = new Padding(6) };
            btnEditMember.Click += BtnEdit_Click;
            var btnDeleteMember = new Button { Text = "Delete Selected", AutoSize = true, Margin = new Padding(6) };
            btnDeleteMember.Click += BtnDelete_Click;
            membersTop.Controls.Add(btnAddMember);
            membersTop.Controls.Add(btnEditMember);
            membersTop.Controls.Add(btnDeleteMember);
            dgvMembers = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AutoGenerateColumns = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
            membersPanel.Controls.Add(dgvMembers);
            membersPanel.Controls.Add(membersTop);
            tabMembers.Controls.Add(membersPanel);

            // Trainers grid + buttons panel
            var trainersPanel = new Panel { Dock = DockStyle.Fill };
            dgvTrainers = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
            var trainersButtons = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true, Height = 40 };
            var btnAddTrainer = new Button { Text = "Add", AutoSize = true, Margin = new Padding(6) };
            btnAddTrainer.Click += BtnAddTrainer_Click;
            var btnEditTrainer = new Button { Text = "Edit", AutoSize = true, Margin = new Padding(6) };
            btnEditTrainer.Click += BtnEditTrainer_Click;
            var btnDeleteTrainer = new Button { Text = "Delete", AutoSize = true, Margin = new Padding(6) };
            btnDeleteTrainer.Click += BtnDeleteTrainer_Click;
            trainersButtons.Controls.Add(btnAddTrainer);
            trainersButtons.Controls.Add(btnEditTrainer);
            trainersButtons.Controls.Add(btnDeleteTrainer);
            trainersPanel.Controls.Add(dgvTrainers);
            trainersPanel.Controls.Add(trainersButtons);
            tabTrainers.Controls.Add(trainersPanel);

            // Sessions grid + buttons
            var sessionsPanel = new Panel { Dock = DockStyle.Fill };
            dgvSessions = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
            dgvSessions.ColumnHeaderMouseClick += DgvSessions_ColumnHeaderMouseClick;
            var sessionsButtons = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true, Height = 40 };
            // filter controls
            dtpFilterFrom = new DateTimePicker { Format = DateTimePickerFormat.Short, Width = 110, Margin = new Padding(6) };
            dtpFilterTo = new DateTimePicker { Format = DateTimePickerFormat.Short, Width = 110, Margin = new Padding(6) };
            btnApplyFilters = new Button { Text = "Apply", AutoSize = true, Margin = new Padding(6) };
            btnApplyFilters.Click += BtnApplyFilters_Click;
            btnClearFilters = new Button { Text = "Clear", AutoSize = true, Margin = new Padding(6) };
            btnClearFilters.Click += BtnClearFilters_Click;
            var btnAddSession = new Button { Text = "Add", AutoSize = true, Margin = new Padding(6) };
            btnAddSession.Click += BtnAddSession_Click;
            var btnEditSession = new Button { Text = "Edit", AutoSize = true, Margin = new Padding(6) };
            btnEditSession.Click += BtnEditSession_Click;
            var btnDeleteSession = new Button { Text = "Delete", AutoSize = true, Margin = new Padding(6) };
            btnDeleteSession.Click += BtnDeleteSession_Click;
            sessionsButtons.Controls.Add(btnAddSession);
            sessionsButtons.Controls.Add(btnEditSession);
            sessionsButtons.Controls.Add(btnDeleteSession);
            // active/past toggle
            rbSessionsActive = new RadioButton { Text = "Active", AutoSize = true, Margin = new Padding(10, 12, 0, 10), Checked = true };
            rbSessionsPast = new RadioButton { Text = "Past", AutoSize = true, Margin = new Padding(6, 12, 0, 10) };
            rbSessionsActive.CheckedChanged += (s, e) => { if (rbSessionsActive!.Checked) RefreshSessionsGrid(); };
            rbSessionsPast.CheckedChanged += (s, e) => { if (rbSessionsPast!.Checked) RefreshSessionsGrid(); };
            var btnExportSession = new Button { Text = "Export", AutoSize = true, Margin = new Padding(6) };
            btnExportSession.Click += BtnExportSessions_Click;
            sessionsButtons.Controls.Add(btnExportSession);
            // add filter controls to sessions toolbar
            sessionsButtons.Controls.Add(new Label { Text = "From:", AutoSize = true, Margin = new Padding(10, 10, 0, 10) });
            sessionsButtons.Controls.Add(dtpFilterFrom);
            sessionsButtons.Controls.Add(new Label { Text = "To:", AutoSize = true, Margin = new Padding(10, 10, 0, 10) });
            sessionsButtons.Controls.Add(dtpFilterTo);
            sessionsButtons.Controls.Add(btnApplyFilters);
            sessionsButtons.Controls.Add(btnClearFilters);
            sessionsButtons.Controls.Add(new Label { Text = "Show:", AutoSize = true, Margin = new Padding(12, 10, 0, 10) });
            sessionsButtons.Controls.Add(rbSessionsActive);
            sessionsButtons.Controls.Add(rbSessionsPast);
            sessionsPanel.Controls.Add(dgvSessions);
            sessionsPanel.Controls.Add(sessionsButtons);
            tabSessions.Controls.Add(sessionsPanel);


            tabs.TabPages.Add(tabMembers);
            tabs.TabPages.Add(tabTrainers);
            tabs.TabPages.Add(tabSessions);
            // facilities are static; no editable Facilities tab

            // create a hidden facilities grid to satisfy code paths that reference it
            dgvFacilities = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, Visible = false };

            root.Controls.Add(tabs, 0, 1);

            // Ensure controls resize with form
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        
    }
}
