using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Grid;
using SD.LLBLGen.Pro.ORMSupportClasses;
using DataAccess.EntityClasses;
using DataAccess.HelperClasses;
using DataAccess.FactoryClasses;

using NewaccNet.Wpf.AppSystem;
using System.ComponentModel;

namespace NewaccNet.Wpf.AppSystem.Voucher.Details
{
    public partial class DebtDetailWindow : DevExpress.Xpf.Core.ThemedWindow
    {
        private JournalEntryEntity _parentEntry;

        public DebtDetailWindow(JournalEntryEntity parentEntry)
        {
            InitializeComponent();
            _parentEntry = parentEntry;
            this.DataContext = _parentEntry;

            LoadDictionaries();

            TableViewDebt.PreviewKeyDown += TableViewDebt_PreviewKeyDown;
        }

        private void TableViewDebt_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Delete)
            {
                var row = GridDebtDetails.SelectedItem as DebtDetailEntity;
                if (row != null && MessageBox.Show("Bạn có chắc muốn xóa dòng này?", "Xác nhận", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    _parentEntry.DebtDetails.Remove(row);
                }
            }
        }

        private void LoadDictionaries()
        {
            try
            {
                using (var adapter = AppDataAccessAdapter.Create())
                {
                    var partners = new EntityCollection<PartnerEntity>();
                    adapter.FetchEntityCollection(partners, null);
                    lookupPartner.ItemsSource = partners;

                    var debtTypes = new EntityCollection<DebtTypeEntity>();
                    adapter.FetchEntityCollection(debtTypes, null);
                    lookupDebtType.ItemsSource = debtTypes;

                    var debtReasons = new EntityCollection<DebtReasonEntity>();
                    adapter.FetchEntityCollection(debtReasons, null);
                    lookupDebtReason.ItemsSource = debtReasons;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh mục: " + ex.Message);
            }
        }

        private void TableViewDebt_InitNewRow(object sender, InitNewRowEventArgs e)
        {
            var newRow = GridDebtDetails.GetRow(e.RowHandle) as DebtDetailEntity;
            if (newRow != null)
            {
                double totalExist = _parentEntry.DebtDetails.Where(x => x != newRow).Sum(x => x.Amount ?? 0);
                double remain = (_parentEntry.Amount ?? 0) - totalExist;
                
                newRow.Amount = remain > 0 ? remain : 0;
                newRow.Liabilitydate = _parentEntry.JournalVoucher?.VoucherDate ?? DateTime.Today;
            }
        }

        private void TableViewDebt_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
        }

        private DebtDetailEntity GetRowFromButton(object sender)
        {
            if (sender is Button btn && btn.DataContext is EditGridCellData cellData)
            {
                return cellData.RowData.Row as DebtDetailEntity;
            }
            return GridDebtDetails.SelectedItem as DebtDetailEntity;
        }

        private void BtnTrackLiability_Click(object sender, RoutedEventArgs e)
        {
            var row = GetRowFromButton(sender);
            if (row != null)
            {
                if (string.IsNullOrEmpty(row.PartnerId) || string.IsNullOrEmpty(row.DebtTypeId))
                {
                    MessageBox.Show("Vui lòng chọn Đối tượng và Nội dung trước khi mở bảng theo dõi cam kết!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var liabilityForm = new LiabilityDueWindow(row.PartnerId, row.DebtTypeId);
                liabilityForm.Owner = this;
                liabilityForm.ShowDialog();
            }
        }

        private void BtnCurrency_Click(object sender, RoutedEventArgs e)
        {
            var row = GetRowFromButton(sender);
            if (row != null)
            {
                var currencyForm = new CurrencyLiabilityWindow(row);
                currencyForm.Owner = this;
                currencyForm.ShowDialog();
            }
        }

        private void BtnAccept_Click(object sender, RoutedEventArgs e)
        {
            TableViewDebt.CommitEditing();
            
            double totalDebt = _parentEntry.DebtDetails.Sum(x => x.Amount ?? 0);
            _parentEntry.Amount = totalDebt;

            this.DialogResult = true;
            this.Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}