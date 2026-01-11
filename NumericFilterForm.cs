using System;
using System.Windows.Forms;

namespace SportsClub
{
    public class NumericFilterForm : Form
    {
        private NumericUpDown nud;
        private Button btnOk;
        private Button btnCancel;

        public int Value { get; private set; }

        public NumericFilterForm(int initial)
        {
            Text = "Min Participants";
            Width = 300; Height = 140; StartPosition = FormStartPosition.CenterParent;
            nud = new NumericUpDown { Left = 20, Top = 12, Minimum = 0, Maximum = 1000, Value = Math.Max(0, initial) };
            btnOk = new Button { Text = "OK", Left = 40, Width = 80, Top = 56 };
            btnCancel = new Button { Text = "Cancel", Left = 140, Width = 80, Top = 56 };
            btnOk.Click += (s, e) => { Value = (int)nud.Value; DialogResult = DialogResult.OK; Close(); };
            btnCancel.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };
            Controls.Add(nud); Controls.Add(btnOk); Controls.Add(btnCancel);
        }
    }
}
