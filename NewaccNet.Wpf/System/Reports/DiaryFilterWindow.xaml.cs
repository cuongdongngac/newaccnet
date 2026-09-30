using System;
using System.Windows;
using NewaccNet.Wpf.Views.Base;

namespace NewaccNet.Wpf.AppSystem.Reports
{
    public partial class DiaryFilterWindow : BaseWindow
    {
        public DateTime FromDate { get; private set; }
        public DateTime ToDate { get; private set; }
        public bool OnlyBooked { get; private set; }

        public DiaryFilterWindow()
        {
            InitializeComponent();
            Loaded += (_, _) => InitDefaults();
        }

        private void InitDefaults()
        {
            DateTime today = DateTime.Today;
            dtFrom.DateTime = new DateTime(today.Year, today.Month, 1);
            dtTo.DateTime = today;
            btnOK.Focus();
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            if (dtFrom.EditValue == null || dtTo.EditValue == null)
            {
                MessageBox.Show(this,
                    "Vui lòng chọn đầy đủ 'Từ ngày' và 'Đến ngày'.",
                    "Thông số không hợp lệ",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DateTime from = dtFrom.DateTime.Date;
            DateTime to = dtTo.DateTime.Date;

            if (from > to)
            {
                MessageBox.Show(this,
                    "'Từ ngày' phải nhỏ hơn hoặc bằng 'Đến ngày'.",
                    "Thông số không hợp lệ",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                dtFrom.Focus();
                return;
            }

            FromDate = from;
            ToDate = to;
            OnlyBooked = chkOnlyBooked.IsChecked == true;
            DialogResult = true;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
