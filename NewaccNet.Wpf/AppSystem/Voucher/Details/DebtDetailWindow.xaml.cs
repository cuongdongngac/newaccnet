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

        public EntityCollection<PartnerEntity> Partners { get; } = new EntityCollection<PartnerEntity>();
        public EntityCollection<DebtTypeEntity> DebtTypes { get; } = new EntityCollection<DebtTypeEntity>();
        public EntityCollection<DebtReasonEntity> DebtReasons { get; } = new EntityCollection<DebtReasonEntity>();

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
                    Partners.Clear();
                    adapter.FetchEntityCollection(Partners, null);

                    DebtTypes.Clear();
                    adapter.FetchEntityCollection(DebtTypes, null);

                    DebtReasons.Clear();
                    adapter.FetchEntityCollection(DebtReasons, null);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh mục: " + ex.Message);
            }
        }

        private void BtnAddPartner_Click(object sender, RoutedEventArgs e)
        {
            var newPartner = new PartnerEntity();
            var editor = new Directory.PartnerEditorWindow(newPartner, true);
            editor.Owner = this;
            if (editor.ShowDialog() == true)
            {
                try
                {
                    using (var adapter = AppDataAccessAdapter.Create())
                    {
                        adapter.SaveEntity(newPartner, true);
                    }

                    if (!Partners.Any(x => x.Id == newPartner.Id))
                    {
                        Partners.Add(newPartner);
                    }

                    var buttonInfo = sender as DevExpress.Xpf.Editors.ButtonInfo;
                    var cellData = buttonInfo?.DataContext as EditGridCellData;
                    var row = cellData?.RowData?.Row as DebtDetailEntity;
                    if (row == null)
                    {
                        row = GridDebtDetails.SelectedItem as DebtDetailEntity;
                    }

                    if (row != null)
                    {
                        row.PartnerId = newPartner.Id;
                    }
                    GridDebtDetails.SetCellValue(TableViewDebt.FocusedRowHandle, "PartnerId", newPartner.Id);
                    TableViewDebt.CloseEditor();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi lưu đối tượng công nợ: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnAddDebtType_Click(object sender, RoutedEventArgs e)
        {
            var newDebtType = new DebtTypeEntity();
            var editor = new Directory.DebtTypeEditorWindow(newDebtType, true);
            editor.Owner = this;
            if (editor.ShowDialog() == true)
            {
                try
                {
                    using (var adapter = AppDataAccessAdapter.Create())
                    {
                        adapter.SaveEntity(newDebtType, true);
                    }

                    if (!DebtTypes.Any(x => x.Id == newDebtType.Id))
                    {
                        DebtTypes.Add(newDebtType);
                    }

                    var buttonInfo = sender as DevExpress.Xpf.Editors.ButtonInfo;
                    var cellData = buttonInfo?.DataContext as EditGridCellData;
                    var row = cellData?.RowData?.Row as DebtDetailEntity;
                    if (row == null)
                    {
                        row = GridDebtDetails.SelectedItem as DebtDetailEntity;
                    }

                    if (row != null)
                    {
                        row.DebtTypeId = newDebtType.Id;
                    }
                    GridDebtDetails.SetCellValue(TableViewDebt.FocusedRowHandle, "DebtTypeId", newDebtType.Id);
                    TableViewDebt.CloseEditor();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi lưu nội dung công nợ: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnAddDebtReason_Click(object sender, RoutedEventArgs e)
        {
            var newReason = new DebtReasonEntity();
            var editor = new Directory.DebtReasonEditorWindow(newReason, true);
            editor.Owner = this;
            if (editor.ShowDialog() == true)
            {
                try
                {
                    using (var adapter = AppDataAccessAdapter.Create())
                    {
                        adapter.SaveEntity(newReason, true);
                    }

                    if (!DebtReasons.Any(x => x.Id == newReason.Id))
                    {
                        DebtReasons.Add(newReason);
                    }

                    var buttonInfo = sender as DevExpress.Xpf.Editors.ButtonInfo;
                    var cellData = buttonInfo?.DataContext as EditGridCellData;
                    var row = cellData?.RowData?.Row as DebtDetailEntity;
                    if (row == null)
                    {
                        row = GridDebtDetails.SelectedItem as DebtDetailEntity;
                    }

                    if (row != null)
                    {
                        row.DebtReasonId = newReason.Id;
                    }
                    GridDebtDetails.SetCellValue(TableViewDebt.FocusedRowHandle, "DebtReasonId", newReason.Id);
                    TableViewDebt.CloseEditor();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi lưu lý do công nợ: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
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
            
            // Dùng Reflection kiểm tra UpdateFilter: Nếu UpdateFilter = true thì chỉ cập nhật từ con vật tư
            if (_parentEntry.CanUpdateFromChild(this))
            {
                double totalDebt = _parentEntry.DebtDetails.Sum(x => x.Amount ?? 0);
                _parentEntry.Amount = Math.Round(totalDebt, 0);
            }

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
