using DevExpress.XtraReports.UI;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;

namespace NewaccNet.Wpf.AppSystem.Reports.Fincance
{
    public partial class TrialBalance : DevExpress.XtraReports.UI.XtraReport
    {
        public TrialBalance()
        {
            InitializeComponent();
            
            // Xử lý sự kiện BeforePrint của row để in đậm các tài khoản cha (Splite = 1)
            this.tableRow2.BeforePrint += TableRow2_BeforePrint;
        }

        private void TableRow2_BeforePrint(object sender, CancelEventArgs e)
        {
            var row = sender as XRTableRow;
            if (row != null)
            {
                // Lấy giá trị của cột Splite từ data source của dòng hiện tại
                bool isSplite = Convert.ToBoolean(GetCurrentColumnValue("Splite"));
                
                if (isSplite)
                {
                    row.Font = new Font(row.Font, FontStyle.Bold);
                }
                else
                {
                    row.Font = new Font(row.Font, FontStyle.Regular);
                }
            }
        }
    }
}
