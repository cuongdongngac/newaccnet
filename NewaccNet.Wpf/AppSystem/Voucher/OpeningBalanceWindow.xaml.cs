using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using DataAccess;
using DataAccess.EntityClasses;
using DataAccess.HelperClasses;
using SD.LLBLGen.Pro.ORMSupportClasses;

namespace NewaccNet.Wpf.AppSystem.Voucher
{
    public partial class OpeningBalanceWindow : DevExpress.Xpf.Core.ThemedWindow
    {
        public class DbcrItem
        {
            public short Id { get; set; }
            public string Name { get; set; }
        }

        public ObservableCollection<JournalEntryEntity> Entries { get; } = new ObservableCollection<JournalEntryEntity>();

        public List<DbcrItem> DbcrList { get; } = new List<DbcrItem>
        {
            new DbcrItem { Id = 1, Name = "Nợ" },
            new DbcrItem { Id = -1, Name = "Có" }
        };

        private readonly List<JournalEntryEntity> _deletedEntries = new List<JournalEntryEntity>();
        private bool _isDirty = false;

        public OpeningBalanceWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            this.PreviewKeyDown += OpeningBalanceWindow_PreviewKeyDown;
            this.Entries.CollectionChanged += Entries_CollectionChanged;

            LoadData();
        }

        private void Entries_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (JournalEntryEntity item in e.OldItems)
                {
                    item.PropertyChanged -= Entry_PropertyChanged;
                }
            }
            if (e.NewItems != null)
            {
                foreach (JournalEntryEntity item in e.NewItems)
                {
                    item.PropertyChanged -= Entry_PropertyChanged;
                    item.PropertyChanged += Entry_PropertyChanged;
                }
            }
        }

        private void Entry_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is JournalEntryEntity je && je.IsDominoSuspended) return;
            _isDirty = true;
        }

        private void LoadData()
        {
            try
            {
                using (var adapter = AppDataAccessAdapter.Create())
                {
                    var filter = new RelationPredicateBucket();
                    var pred = new PredicateExpression(JournalEntryFields.JournalVoucherId == DBNull.Value);
                    pred.AddWithOr(JournalEntryFields.JournalVoucherId == 0);
                    filter.PredicateExpression.Add(pred);

                    var prefetch = new PrefetchPath2(EntityType.JournalEntryEntity);
                    prefetch.Add(JournalEntryEntity.PrefetchPathDebtDetails);
                    prefetch.Add(JournalEntryEntity.PrefetchPathCostDetails);
                    prefetch.Add(JournalEntryEntity.PrefetchPathAssetDetails);
                    var pInv = prefetch.Add(JournalEntryEntity.PrefetchPathInventoryVouchers);
                    pInv.SubPath.Add(InventoryVoucherEntity.PrefetchPathInventoryVoucherLines);
                    prefetch.Add(JournalEntryEntity.PrefetchPathInvestmentDetails);
                    prefetch.Add(JournalEntryEntity.PrefetchPathCurrencyDetails);

                    var loaded = new EntityCollection<JournalEntryEntity>();
                    adapter.FetchEntityCollection(loaded, filter, prefetch);

                    Entries.Clear();
                    _deletedEntries.Clear();

                    foreach (var e in loaded.OrderBy(x => x.AccountId))
                    {
                        Entries.Add(e);
                    }

                    if (Entries.Count == 0)
                    {
                        Entries.Add(new JournalEntryEntity { Dbcr = 1, Amount = 0 });
                    }

                    _isDirty = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải dữ liệu số dư: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ViewEntries_ShowingEditor(object sender, DevExpress.Xpf.Grid.ShowingEditorEventArgs e)
        {
            if (e.Column != null && e.Column.FieldName == "Amount")
            {
                var row = e.Row as JournalEntryEntity ?? gridEntries.GetRow(e.RowHandle) as JournalEntryEntity;
                if (row != null && (row.IsMaterialBound || row.HasSubLedgerDetails()))
                {
                    e.Cancel = true; // Khóa sửa tay: Số tiền do bảng chi tiết quản lý và tự động đổ về từ ORM
                    return;
                }
            }

            if (e.Column != null && e.Column.FieldName == "AccountId")
            {
                short? dbcr = null;
                if (e.Row is JournalEntryEntity entry)
                {
                    dbcr = entry.Dbcr;
                }
                else
                {
                    var cellVal = gridEntries.GetCellValue(e.RowHandle, "Dbcr");
                    if (cellVal != null && short.TryParse(cellVal.ToString(), out short val))
                    {
                        dbcr = val;
                    }
                }

                if (!dbcr.HasValue || dbcr.Value == 0)
                {
                    e.Cancel = true;
                    if (gridEntries.Columns["Dbcr"] != null)
                    {
                        Dispatcher.BeginInvoke(new Action(() =>
                        {
                            gridEntries.CurrentColumn = gridEntries.Columns["Dbcr"];
                            viewEntries.ShowEditor();
                        }));
                    }
                    MessageBox.Show("Vui lòng chọn Nợ hoặc Có trước khi chọn tài khoản.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void ViewEntries_CellValueChanged(object sender, DevExpress.Xpf.Grid.CellValueChangedEventArgs e)
        {
            _isDirty = true;
        }

        private void ViewEntries_InitNewRow(object sender, DevExpress.Xpf.Grid.InitNewRowEventArgs e)
        {
            _isDirty = true;
            gridEntries.SetCellValue(e.RowHandle, "Dbcr", (short)1);
            gridEntries.SetCellValue(e.RowHandle, "Amount", 0.0);
        }

        private void ViewEntries_RowDoubleClick(object sender, DevExpress.Xpf.Grid.RowDoubleClickEventArgs e)
        {
            if (viewEntries.FocusedRow is JournalEntryEntity entry)
            {
                OpenDetailFormIfRequired(entry);
            }
        }

        public void OpenDetailFormIfRequired(JournalEntryEntity entry)
        {
            if (AccountLookUpEdit.OpenDetailWindow(entry, this, false, true))
            {
                _isDirty = true;
            }
        }

        private void BtnAdd_Click(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            var newEntry = new JournalEntryEntity
            {
                Dbcr = 1,
                Amount = 0,
                JournalVoucherId = null,
                ParentId = null
            };
            Entries.Add(newEntry);
            _isDirty = true;
            viewEntries.FocusedRowHandle = Entries.Count - 1;
            viewEntries.ShowEditor();
        }

        private void BtnDelete_Click(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            DeleteFocusedRow();
        }

        private void DeleteFocusedRow()
        {
            if (viewEntries.FocusedRow is JournalEntryEntity selected)
            {
                string desc = !string.IsNullOrEmpty(selected.AccountId) ? $"tài khoản '{selected.AccountId}'" : "dòng này";
                if (MessageBox.Show($"Bạn có chắc chắn muốn xóa số dư {desc}?", "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    if (selected.Id > 0)
                    {
                        _deletedEntries.Add(selected);
                    }
                    Entries.Remove(selected);
                    _isDirty = true;
                }
            }
        }

        private void ViewEntries_RowDeleting(object sender, Views.Base.RowDeletingEventArgs e)
        {
            if (e.RowItem is JournalEntryEntity selected)
            {
                if (selected.Id > 0)
                {
                    _deletedEntries.Add(selected);
                }
                _isDirty = true;
            }
        }

        private void GridEntries_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete && !viewEntries.IsEditing)
            {
                DeleteFocusedRow();
                e.Handled = true;
            }
        }

        private void BtnSave_Click(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            SaveData(true);
        }

        public void SaveData(bool showNotification = true)
        {
            try
            {
                viewEntries.CloseEditor();

                var validEntries = Entries.Where(x => !string.IsNullOrWhiteSpace(x.AccountId)).ToList();

                using (var adapter = AppDataAccessAdapter.Create())
                {
                    adapter.StartTransaction(System.Data.IsolationLevel.ReadCommitted, "SaveOpeningBalances");
                    try
                    {
                        // 1. Xóa các dòng đã đánh dấu xóa (bao gồm toàn bộ chi tiết con 1-nhiều)
                        foreach (var del in _deletedEntries)
                        {
                            if (del.Id > 0)
                            {
                                var dbDel = new JournalEntryEntity(del.Id);
                                var prefetchDel = new PrefetchPath2(EntityType.JournalEntryEntity);
                                var ivDel = prefetchDel.Add(JournalEntryEntity.PrefetchPathInventoryVouchers);
                                ivDel.SubPath.Add(InventoryVoucherEntity.PrefetchPathInventoryVoucherLines);
                                if (adapter.FetchEntity(dbDel, prefetchDel))
                                {
                                    foreach (var iv in dbDel.InventoryVouchers)
                                    {
                                        adapter.DeleteEntitiesDirectly(typeof(InventoryVoucherLineEntity), new RelationPredicateBucket(InventoryVoucherLineFields.InventoryVoucherId == iv.Id));
                                    }
                                    adapter.DeleteEntitiesDirectly(typeof(InventoryVoucherEntity), new RelationPredicateBucket(InventoryVoucherFields.JournalEntryId == del.Id));
                                }

                                adapter.DeleteEntitiesDirectly(typeof(DebtDetailEntity), new RelationPredicateBucket(DebtDetailFields.JournalEntryId == del.Id));
                                adapter.DeleteEntitiesDirectly(typeof(CostDetailEntity), new RelationPredicateBucket(CostDetailFields.JournalEntryId == del.Id));
                                adapter.DeleteEntitiesDirectly(typeof(AssetDetailEntity), new RelationPredicateBucket(AssetDetailFields.JournalEntryId == del.Id));
                                adapter.DeleteEntitiesDirectly(typeof(InvestmentDetailEntity), new RelationPredicateBucket(InvestmentDetailFields.JournalEntryId == del.Id));
                                adapter.DeleteEntitiesDirectly(typeof(CurrencyDetailEntity), new RelationPredicateBucket(CurrencyDetailFields.JournalEntryId == del.Id));
                                adapter.DeleteEntity(del);
                            }
                        }

                        // 2. Lưu các dòng hợp lệ (đồng bộ xóa các phiếu kho / dòng chi tiết con đã bị gỡ khỏi bộ nhớ)
                        foreach (var entry in validEntries)
                        {
                            entry.JournalVoucherId = null;
                            entry.ParentId = null;
                            if (entry.IsNew)
                            {
                                entry.Createdate = DateTime.Now;
                            }
                            else
                            {
                                entry.Modifydate = DateTime.Now;

                                if (entry.Id > 0)
                                {
                                    var dbJe = new JournalEntryEntity(entry.Id);
                                    var prefetchJe = new PrefetchPath2(EntityType.JournalEntryEntity);
                                    prefetchJe.Add(JournalEntryEntity.PrefetchPathDebtDetails);
                                    prefetchJe.Add(JournalEntryEntity.PrefetchPathCostDetails);
                                    prefetchJe.Add(JournalEntryEntity.PrefetchPathAssetDetails);
                                    prefetchJe.Add(JournalEntryEntity.PrefetchPathInvestmentDetails);
                                    prefetchJe.Add(JournalEntryEntity.PrefetchPathCurrencyDetails);
                                    var ivNode = prefetchJe.Add(JournalEntryEntity.PrefetchPathInventoryVouchers);
                                    ivNode.SubPath.Add(InventoryVoucherEntity.PrefetchPathInventoryVoucherLines);

                                    if (adapter.FetchEntity(dbJe, prefetchJe))
                                    {
                                        foreach (var dbDebt in dbJe.DebtDetails)
                                        {
                                            if (!entry.DebtDetails.Any(x => x.Id == dbDebt.Id && !x.IsNew))
                                                adapter.DeleteEntity(dbDebt);
                                        }
                                        foreach (var dbCost in dbJe.CostDetails)
                                        {
                                            if (!entry.CostDetails.Any(x => x.Id == dbCost.Id && !x.IsNew))
                                                adapter.DeleteEntity(dbCost);
                                        }
                                        foreach (var dbAsset in dbJe.AssetDetails)
                                        {
                                            if (!entry.AssetDetails.Any(x => x.Id == dbAsset.Id && !x.IsNew))
                                                adapter.DeleteEntity(dbAsset);
                                        }
                                        foreach (var dbInv in dbJe.InvestmentDetails)
                                        {
                                            if (!entry.InvestmentDetails.Any(x => x.Lineid == dbInv.Lineid && !x.IsNew))
                                                adapter.DeleteEntity(dbInv);
                                        }
                                        foreach (var dbCurr in dbJe.CurrencyDetails)
                                        {
                                            if (!entry.CurrencyDetails.Any(x => x.Id == dbCurr.Id && !x.IsNew))
                                                adapter.DeleteEntity(dbCurr);
                                        }
                                        foreach (var dbIv in dbJe.InventoryVouchers)
                                        {
                                            var ramIv = entry.InventoryVouchers.FirstOrDefault(x => x.Id == dbIv.Id && !x.IsNew);
                                            if (ramIv == null)
                                            {
                                                foreach (var dbLine in dbIv.InventoryVoucherLines)
                                                    adapter.DeleteEntity(dbLine);
                                                adapter.DeleteEntity(dbIv);
                                            }
                                            else
                                            {
                                                foreach (var dbLine in dbIv.InventoryVoucherLines)
                                                {
                                                    if (!ramIv.InventoryVoucherLines.Any(x => x.Id == dbLine.Id && !x.IsNew))
                                                        adapter.DeleteEntity(dbLine);
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            adapter.SaveEntity(entry, true, true);
                        }

                        adapter.Commit();
                        _deletedEntries.Clear();
                        _isDirty = false;

                        if (showNotification)
                        {
                            MessageBox.Show("Đã lưu số dư thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                    catch
                    {
                        adapter.Rollback();
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lưu số dư: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OpeningBalanceWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.S)
            {
                SaveData(true);
                e.Handled = true;
            }
            else if (e.Key == Key.F4)
            {
                if (viewEntries.FocusedRow is JournalEntryEntity entry)
                {
                    OpenDetailFormIfRequired(entry);
                    e.Handled = true;
                }
            }
            else if (e.Key == Key.Escape)
            {
                this.Close();
                e.Handled = true;
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (_isDirty)
            {
                var res = MessageBox.Show("Dữ liệu số dư có thay đổi chưa lưu. Bạn có muốn lưu trước khi đóng không?", "Xác nhận", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                if (res == MessageBoxResult.Yes)
                {
                    SaveData(false);
                }
                else if (res == MessageBoxResult.Cancel)
                {
                    e.Cancel = true;
                    return;
                }
            }
            base.OnClosing(e);
        }
    }
}
