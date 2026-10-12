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

            LoadData();
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
            if (entry == null) return;
            if (string.IsNullOrEmpty(entry.AccountId))
            {
                MessageBox.Show("Vui lòng chọn tài khoản trước khi nhập chi tiết.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            using (var adapter = AppDataAccessAdapter.Create())
            {
                var account = new ChartOfAccountEntity(entry.AccountId);
                if (!adapter.FetchEntity(account)) return;

                string category = account.CategoryId;
                if (string.IsNullOrEmpty(category))
                {
                    if (entry.AccountId.StartsWith("131") || entry.AccountId.StartsWith("331") || entry.AccountId.StartsWith("138") || entry.AccountId.StartsWith("338")) category = "A";
                    else if (entry.AccountId.StartsWith("6") || entry.AccountId.StartsWith("154")) category = "E";
                }

                if (category == "A")
                {
                    var debtForm = new Details.DebtDetailWindow(entry);
                    debtForm.Owner = this;
                    if (debtForm.ShowDialog() == true)
                    {
                        _isDirty = true;
                    }
                }
                else if (category == "E")
                {
                    var costForm = new Details.CostDetailWindow(entry, false);
                    costForm.Owner = this;
                    if (costForm.ShowDialog() == true)
                    {
                        _isDirty = true;
                    }
                }
                else
                {
                    MessageBox.Show($"Tài khoản {entry.AccountId} ({account.AccountName}) không theo dõi sổ chi tiết công nợ hoặc chi phí.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
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
                        // 1. Xóa các dòng đã đánh dấu xóa
                        foreach (var del in _deletedEntries)
                        {
                            if (del.Id > 0)
                            {
                                adapter.DeleteEntitiesDirectly(typeof(DebtDetailEntity), new RelationPredicateBucket(DebtDetailFields.JournalEntryId == del.Id));
                                adapter.DeleteEntitiesDirectly(typeof(CostDetailEntity), new RelationPredicateBucket(CostDetailFields.JournalEntryId == del.Id));
                                adapter.DeleteEntity(del);
                            }
                        }

                        // 2. Lưu các dòng hợp lệ
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

        internal void SaveDataSilently()
        {
            SaveData(false);
        }

        private void OpeningBalanceWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.S)
            {
                SaveData(true);
                e.Handled = true;
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
