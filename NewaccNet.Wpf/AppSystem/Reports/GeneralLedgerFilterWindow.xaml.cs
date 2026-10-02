using System;
using System.Linq;
using System.Windows;
using NewaccNet.Wpf.Views.Base;
using NewaccNet.Wpf.AppSystem.Helpers;
using SD.LLBLGen.Pro.QuerySpec;
using SD.LLBLGen.Pro.QuerySpec.Adapter;
using DataAccess.FactoryClasses;
using DataAccess.EntityClasses;

namespace NewaccNet.Wpf.AppSystem.Reports
{
    public partial class GeneralLedgerFilterWindow : BaseWindow
    {
        public DateTime FromDate { get; private set; }
        public DateTime ToDate { get; private set; }
        public string AccountId { get; private set; }
        public bool OnlyBooked { get; private set; }

        public GeneralLedgerFilterWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Thiết lập mặc định trước (nếu file JSON chưa có)
            DateTime today = DateTime.Today;
            dtFrom.DateTime = new DateTime(today.Year, today.Month, 1);
            dtTo.DateTime = today;

            // Load danh sách tài khoản
            LoadAccounts();

            // Load lại state từ JSON (sẽ ghi đè lên mặc định nếu đã từng lưu)
            UserPreferencesHelper.LoadState(this);

            btnOK.Focus();
        }

        private void LoadAccounts()
        {
            try
            {
                using var adapter = AppDataAccessAdapter.Create();
                var qf = new QueryFactory();
                // Lấy tất cả tài khoản
                var q = qf.ChartOfAccount;
                var accounts = adapter.FetchQuery(q).Cast<ChartOfAccountEntity>().ToList();
                lkAccount.ItemsSource = accounts;
            }
            catch (Exception ex)
            {
                // Bỏ qua lỗi hoặc log lại
            }
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

            if (lkAccount.EditValue == null || string.IsNullOrWhiteSpace(lkAccount.EditValue.ToString()))
            {
                MessageBox.Show(this,
                    "Vui lòng chọn 'Tài khoản' cần in sổ cái.",
                    "Thông số không hợp lệ",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                lkAccount.Focus();
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
            AccountId = lkAccount.EditValue.ToString().Trim();
            OnlyBooked = chkOnlyBooked.IsChecked == true;

            // Lưu lại state trước khi đóng
            UserPreferencesHelper.SaveState(this);

            NewaccNet.Wpf.AppSystem.Reports.LedgerReportService.ShowPreview(this, from, to, AccountId, OnlyBooked);
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}






