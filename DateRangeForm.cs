using System;
using System.Drawing;
using System.Windows.Forms;

namespace SportsClub
{
    public class DateRangeForm : Form
    {
        private DateTimePicker dtpFrom;
        private DateTimePicker dtpTo;
        private Button btnOk;
        private Button btnCancel;

        public DateTime? From { get; private set; }
        public DateTime? To { get; private set; }

        public DateRangeForm(DateTime from, DateTime to)
        {
            Text = "Select Date Range";
            ClientSize = new Size(420, 140);
            StartPosition = FormStartPosition.CenterParent;

            var margin = 12;
            var pickerWidth = 160;
            var top = 16;

            dtpFrom = new DateTimePicker
            {
                Format = DateTimePickerFormat.Short,
                Value = from.Date,
                Left = margin,
                Top = top,
                Width = pickerWidth
            };

            dtpTo = new DateTimePicker
            {
                Format = DateTimePickerFormat.Short,
                Value = to.Date,
                Left = margin + pickerWidth + 12,
                Top = top,
                Width = pickerWidth
            };

            btnOk = new Button { Text = "OK", Width = 100, Height = 28 };
            btnCancel = new Button { Text = "Cancel", Width = 100, Height = 28 };

            // position buttons centered below pickers
            var buttonsTop = top + dtpFrom.Height + 16;
            var totalButtonsWidth = btnOk.Width + 12 + btnCancel.Width;
            var startX = (ClientSize.Width - totalButtonsWidth) / 2;
            btnOk.Left = startX;
            btnCancel.Left = startX + btnOk.Width + 12;
            btnOk.Top = btnCancel.Top = buttonsTop;

            btnOk.Click += (s, e) => { From = dtpFrom.Value.Date; To = dtpTo.Value.Date; DialogResult = DialogResult.OK; Close(); };
            btnCancel.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };

            Controls.Add(dtpFrom);
            Controls.Add(dtpTo);
            Controls.Add(btnOk);
            Controls.Add(btnCancel);
        }
    }
}
