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
    public partial class DebtDetailWindow : NewaccNet.Wpf.Views.Base.BaseDetailWindow
    {
        private JournalEntryEntity _parentEntry;
        private System.Collections.Generic.List<DebtDetailEntity> _originalList;
        private bool _isSaved = false;

        public DebtDetailWindow(JournalEntryEntity parentEntry)
        {
            InitializeComponent();
            _parentEntry = parentEntry;
            this.DataContext = _parentEntry;

            _originalList = _parentEntry.DebtDetails.ToList();
            foreach (var item in _originalList) item.SaveFields("undo");

            LoadDictionaries();
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
            if (row == null) return;

            if (string.IsNullOrEmpty(row.PartnerId) || string.IsNullOrEmpty(row.DebtTypeId))
            {
                MessageBox.Show("Vui lòng chọn Đối tượng và Nội dung trước khi mở bảng theo dõi cam kết!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            LiabilityDueWindow liabilityForm;

            // Nếu dòng đã được liên kết với một LiabilityDue cụ thể → mở thẳng theo Id
            if (row.LiabilityDuesId.HasValue)
            {
                liabilityForm = new LiabilityDueWindow(row.LiabilityDuesId.Value);
            }
            else
            {
                // Chưa liên kết → tìm theo (PartnerId, DebtTypeId) hoặc tạo mới
                liabilityForm = new LiabilityDueWindow(row.PartnerId, row.DebtTypeId);
            }

            liabilityForm.Owner = this;
            if (liabilityForm.ShowDialog() == true) { if (liabilityForm.IsDeleted) { row.LiabilityDuesId = null; } else if (liabilityForm.SavedId.HasValue && row.LiabilityDuesId != liabilityForm.SavedId) { row.LiabilityDuesId = liabilityForm.SavedId.Value; } }
        }

        private void BtnCurrency_Click(object sender, RoutedEventArgs e)
        {
            var row = GetRowFromButton(sender);
            if (row != null)
            {
                var currencyForm = new CurrencyLiabilityWindow(row);
                currencyForm.Owner = this;
                if (currencyForm.ShowDialog() == true)
                {
                    if (currencyForm.IsDeleted)
                    {
                        row.CurrencyLiabilityLine = null;
                    }
                }
            }
        }

        protected override void BtnAccept_Click(object sender, RoutedEventArgs e)
        {
            TableViewDebt.CloseEditor();
            TableViewDebt.FocusedRowHandle = DevExpress.Xpf.Grid.GridControl.InvalidRowHandle;
            TableViewDebt.CommitEditing();
            
            double totalDebt = _parentEntry.DebtDetails.Sum(x => x.Amount ?? 0);
            _parentEntry.Amount = totalDebt;

            _isSaved = true;
            this.DialogResult = true;
            this.Close();
        }

        protected override void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (!_isSaved)
            {
                foreach (var item in _originalList) item.RollbackFields("undo");
                _parentEntry.DebtDetails.Clear();
                foreach (var item in _originalList) _parentEntry.DebtDetails.Add(item);
            }
            base.OnClosing(e);
        }
    }
}
