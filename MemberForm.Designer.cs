namespace SportsClub
{
    partial class MemberForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cbSubscription;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown nudVisits;
        private System.Windows.Forms.CheckBox chkActive;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;

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
            this.txtName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cbSubscription = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.nudVisits = new System.Windows.Forms.NumericUpDown();
            this.chkActive = new System.Windows.Forms.CheckBox();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.nudVisits)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "Name:";
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(80, 12);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(250, 23);
            this.txtName.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 52);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(66, 15);
            this.label2.TabIndex = 2;
            this.label2.Text = "Subscription:";
            // 
            // cbSubscription
            // 
            this.cbSubscription.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbSubscription.FormattingEnabled = true;
            this.cbSubscription.Location = new System.Drawing.Point(80, 49);
            this.cbSubscription.Name = "cbSubscription";
            this.cbSubscription.Size = new System.Drawing.Size(150, 23);
            this.cbSubscription.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 92);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(38, 15);
            this.label3.TabIndex = 4;
            this.label3.Text = "Visits:";
            // 
            // chkActive
            // 
            this.chkActive.AutoSize = true;
            this.chkActive.Location = new System.Drawing.Point(250, 50);
            this.chkActive.Name = "chkActive";
            this.chkActive.Size = new System.Drawing.Size(60, 19);
            this.chkActive.TabIndex = 4;
            this.chkActive.Text = "Active";
            this.chkActive.UseVisualStyleBackColor = true;
            // 
            // nudVisits
            // 
            this.nudVisits.Location = new System.Drawing.Point(80, 90);
            this.nudVisits.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            this.nudVisits.Name = "nudVisits";
            this.nudVisits.Size = new System.Drawing.Size(80, 23);
            this.nudVisits.TabIndex = 5;
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(80, 130);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(80, 30);
            this.btnOk.TabIndex = 6;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(170, 130);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(80, 30);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // MemberForm
            // 
            this.ClientSize = new System.Drawing.Size(350, 180);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.nudVisits);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.chkActive);
            this.Controls.Add(this.cbSubscription);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MemberForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Member";
            ((System.ComponentModel.ISupportInitialize)(this.nudVisits)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
