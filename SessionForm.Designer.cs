namespace SportsClub
{
    partial class SessionForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DateTimePicker dtpDate;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cbTrainer;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckedListBox clbMembers;
        private System.Windows.Forms.Label labelFacility;
        private System.Windows.Forms.ComboBox cbFacility;

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
            this.label1 = new System.Windows.Forms.Label();
            this.dtpDate = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.cbTrainer = new System.Windows.Forms.ComboBox();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.clbMembers = new System.Windows.Forms.CheckedListBox();
            this.labelFacility = new System.Windows.Forms.Label();
            this.cbFacility = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(34, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "Date:";
            // 
            // dtpDate
            // 
            this.dtpDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpDate.CustomFormat = "yyyy-MM-dd HH:mm";
            this.dtpDate.Location = new System.Drawing.Point(70, 12);
            this.dtpDate.Name = "dtpDate";
            this.dtpDate.Size = new System.Drawing.Size(200, 23);
            this.dtpDate.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 52);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(47, 15);
            this.label2.TabIndex = 2;
            this.label2.Text = "Trainer:";
            // 
            // cbTrainer
            // 
            this.cbTrainer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbTrainer.FormattingEnabled = true;
            this.cbTrainer.Location = new System.Drawing.Point(70, 49);
            this.cbTrainer.Name = "cbTrainer";
            this.cbTrainer.Size = new System.Drawing.Size(200, 23);
            this.cbTrainer.TabIndex = 3;
            // 
            // labelFacility
            // 
            this.labelFacility.AutoSize = true;
            this.labelFacility.Location = new System.Drawing.Point(12, 85);
            this.labelFacility.Name = "labelFacility";
            this.labelFacility.Size = new System.Drawing.Size(44, 15);
            this.labelFacility.TabIndex = 6;
            this.labelFacility.Text = "Place:";
            // 
            // cbFacility
            // 
            this.cbFacility.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbFacility.FormattingEnabled = true;
            this.cbFacility.Location = new System.Drawing.Point(70, 82);
            this.cbFacility.Name = "cbFacility";
            this.cbFacility.Size = new System.Drawing.Size(200, 23);
            this.cbFacility.TabIndex = 7;
            this.cbFacility.SelectedIndexChanged += new System.EventHandler(this.cbFacility_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 120);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(60, 15);
            this.label3.TabIndex = 8;
            this.label3.Text = "Members:";
            // 
            // clbMembers
            // 
            this.clbMembers.CheckOnClick = true;
            this.clbMembers.FormattingEnabled = true;
            this.clbMembers.Location = new System.Drawing.Point(70, 117);
            this.clbMembers.Name = "clbMembers";
            this.clbMembers.Size = new System.Drawing.Size(200, 100);
            this.clbMembers.TabIndex = 9;
            
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(70, 240);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(80, 30);
            this.btnOk.TabIndex = 8;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(160, 240);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(80, 30);
            this.btnCancel.TabIndex = 9;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // SessionForm
            // 
            this.ClientSize = new System.Drawing.Size(300, 320);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.clbMembers);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cbFacility);
            this.Controls.Add(this.labelFacility);
            this.Controls.Add(this.cbTrainer);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.dtpDate);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SessionForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Session";
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
