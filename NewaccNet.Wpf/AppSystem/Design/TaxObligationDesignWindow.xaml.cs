using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using DevExpress.Xpf.Grid;
using DataAccess.EntityClasses;
using DataAccess.HelperClasses;
using SD.LLBLGen.Pro.ORMSupportClasses;
using NewaccNet.Wpf.Views.Base;

namespace NewaccNet.Wpf.AppSystem.Design
{
    public partial class TaxObligationDesignWindow : BaseWindow
    {
        private EntityCollection<TaxObligationEntity> _obligations = new EntityCollection<TaxObligationEntity>();
        private List<TaxObligationEntity> _origObligations = new List<TaxObligationEntity>();
        private Dictionary<int, List<TaxObligationItemEntity>> _origItemsByObligationId = new Dictionary<int, List<TaxObligationItemEntity>>();
        private Dictionary<int, List<AccountTaxObligationEntity>> _origAccountItemsByItemId = new Dictionary<int, List<AccountTaxObligationEntity>>();

        public TaxObligationDesignWindow()
        {
            InitializeComponent();
            this.Loaded += (_, _) =>
            {
                DesignLookupHelper.EnsureAccountsLoaded();
                LoadData();
            };

            // Link giữa 2 grid: khi chọn Obligation trên gridObligations → load Items tương ứng
            gridObligations.SelectedItemChanged += GridObligations_SelectedItemChanged;
        }

        private void GridObligations_SelectedItemChanged(object sender, SelectedItemChangedEventArgs e)
        {
            var selectedObligation = gridObligations.SelectedItem as TaxObligationEntity;
            if (selectedObligation != null)
            {
                // Bind gridItems vào TaxObligationItems của Obligation đã chọn
                gridItems.ItemsSource = selectedObligation.TaxObligationItems;
            }
            else
            {
                gridItems.ItemsSource = null;
            }
        }

        public new void LoadData()
        {
            try
            {
                gridObligations.IsEnabled = false;
                gridItems.IsEnabled = false;

                using var adapter = AppDataAccessAdapter.Create();
                var list = new EntityCollection<TaxObligationEntity>();

                var prefetch = new PrefetchPath2((int)DataAccess.EntityType.TaxObligationEntity);
                // 3 tầng: TaxObligation → TaxObligationItems → AccountTaxObligations
                prefetch.Add(TaxObligationEntity.PrefetchPathTaxObligationItems)
                        .SubPath.Add(TaxObligationItemEntity.PrefetchPathAccountTaxObligations);

                adapter.FetchEntityCollection(list, null, prefetch);
                _obligations = list;
                gridObligations.ItemsSource = list;

                SnapshotOriginsForDeltaDelete(list);

                // Auto-select obligation đầu tiên để load items
                if (list.Count > 0)
                {
                    gridObligations.SelectedItem = list[0];
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải cấu hình Nghĩa Vụ Thuế: " + ex.Message, "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                gridObligations.IsEnabled = true;
                gridItems.IsEnabled = true;
            }
        }

        private void SnapshotOriginsForDeltaDelete(EntityCollection<TaxObligationEntity> list)
        {
            _origObligations.Clear();
            _origItemsByObligationId.Clear();
            _origAccountItemsByItemId.Clear();

            foreach (var ob in list)
            {
                var obClone = new TaxObligationEntity();
                obClone.Fields = ob.Fields.Clone();
                obClone.IsNew = ob.IsNew;
                obClone.IsDirty = ob.IsDirty;
                _origObligations.Add(obClone);

                if (ob.TaxObligationItems != null)
                {
                    var itemClones = new List<TaxObligationItemEntity>();
                    foreach (var item in ob.TaxObligationItems)
                    {
                        var itemClone = new TaxObligationItemEntity();
                        itemClone.Fields = item.Fields.Clone();
                        itemClone.IsNew = item.IsNew;
                        itemClone.IsDirty = item.IsDirty;
                        itemClones.Add(itemClone);

                        if (item.AccountTaxObligations != null)
                        {
                            var accItemClones = new List<AccountTaxObligationEntity>();
                            foreach (var accItem in item.AccountTaxObligations)
                            {
                                var accItemClone = new AccountTaxObligationEntity();
                                accItemClone.Fields = accItem.Fields.Clone();
                                accItemClone.IsNew = accItem.IsNew;
                                accItemClone.IsDirty = accItem.IsDirty;
                                accItemClones.Add(accItemClone);
                            }
                            _origAccountItemsByItemId[item.Id] = accItemClones;
                        }
                    }
                    _origItemsByObligationId[ob.Id] = itemClones;
                }
            }
        }

        private void CommitEditingAllGrid()
        {
            var tvOb = gridObligations.View as TableView;
            if (tvOb != null)
            {
                var focusedOb = tvOb.FocusedView as TableView ?? tvOb;
                focusedOb.CommitEditing();
                focusedOb.CloseEditor();
                tvOb.CommitEditing();
                tvOb.CloseEditor();
                tvOb.FocusedRowHandle = GridControl.InvalidRowHandle;
            }

            var tvItem = gridItems.View as TableView;
            if (tvItem != null)
            {
                var focusedItem = tvItem.FocusedView as TableView ?? tvItem;
                focusedItem.CommitEditing();
                focusedItem.CloseEditor();
                tvItem.CommitEditing();
                tvItem.CloseEditor();
                tvItem.FocusedRowHandle = GridControl.InvalidRowHandle;
            }
        }

        private void BtnLoad_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            if (MessageBox.Show("Tải lại cấu hình từ CSDL? Các thay đổi chưa lưu sẽ bị mất.", "Xác nhận",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                LoadData();
            }
        }

        private void BtnSave_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            try
            {
                CommitEditingAllGrid();

                // Đảm bảo quan hệ 2 chiều giữa cha và con cho mọi dòng mới thêm
                foreach (var ob in _obligations)
                {
                    if (ob.TaxObligationItems == null) continue;
                    foreach (var item in ob.TaxObligationItems)
                    {
                        if (item.TaxObligation == null) item.TaxObligation = ob;
                        if (ob.Id > 0 && (item.TaxObligationId == null || item.TaxObligationId <= 0))
                            item.TaxObligationId = ob.Id;

                        if (item.AccountTaxObligations == null) continue;
                        foreach (var acc in item.AccountTaxObligations)
                        {
                            if (acc.TaxObligationItem == null) acc.TaxObligationItem = item;
                            if (item.Id > 0 && (acc.TaxObligationItemId == null || acc.TaxObligationItemId <= 0))
                                acc.TaxObligationItemId = item.Id;
                        }
                    }
                }

                using var adapter = AppDataAccessAdapter.Create();
                adapter.StartTransaction(System.Data.IsolationLevel.ReadCommitted, "UOW_DesignTaxObligation");
                try
                {
                    // 1) DELETE delta (con trước, cha sau)
                    // Lv 3: AccountTaxObligations
                    foreach (var ob in _obligations)
                    {
                        if (ob.TaxObligationItems == null) continue;
                        foreach (var item in ob.TaxObligationItems)
                        {
                            if (item.AccountTaxObligations == null || item.Id <= 0) continue;
                            if (!_origAccountItemsByItemId.TryGetValue(item.Id, out var origAccItems)) continue;
                            var currentAccItemIds = new HashSet<int>(item.AccountTaxObligations.Where(x => x.Id > 0).Select(x => x.Id));
                            foreach (var oldAccItem in origAccItems)
                            {
                                if (!currentAccItemIds.Contains(oldAccItem.Id))
                                {
                                    adapter.DeleteEntity(oldAccItem);
                                }
                            }
                        }
                    }

                    // Lv 2: TaxObligationItems
                    foreach (var ob in _obligations)
                    {
                        if (ob.Id <= 0 || ob.TaxObligationItems == null) continue;
                        if (!_origItemsByObligationId.TryGetValue(ob.Id, out var origItems)) continue;
                        var currentItemIds = new HashSet<int>(ob.TaxObligationItems.Where(x => x.Id > 0).Select(x => x.Id));
                        foreach (var oldItem in origItems)
                        {
                            if (!currentItemIds.Contains(oldItem.Id))
                            {
                                adapter.DeleteEntity(oldItem);
                            }
                        }
                    }

                    // Lv 1: TaxObligations
                    {
                        var currentObligationIds = new HashSet<int>(_obligations.Where(x => x.Id > 0).Select(x => x.Id));
                        foreach (var oldOb in _origObligations)
                        {
                            if (!currentObligationIds.Contains(oldOb.Id))
                            {
                                adapter.DeleteEntity(oldOb);
                            }
                        }
                    }

                    // 2) Tạo object graph và save từng TaxObligation với recurse
                    // LLBLGen sẽ tự động save các entities con và cập nhật FK
                    foreach (var ob in _obligations)
                    {
                        adapter.SaveEntity(ob, refetchAfterSave: true, recurse: true);
                    }

                    adapter.Commit();

                    // Refresh dữ liệu gốc sau save
                    LoadData();

                    MessageBox.Show("✅ Lưu cấu hình Nghĩa Vụ Thuế thành công!", "Thông báo",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch
                {
                    adapter.Rollback();
                    throw;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Lỗi lưu cấu hình Nghĩa Vụ Thuế: " + ex.Message, "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnNewObligation_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            var tv = gridObligations.View as TableView;
            if (tv == null) return;
            tv.AddNewRow();
            tv.MoveLastRow();
        }

        private void BtnNewItem_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            var selectedObligation = gridObligations.SelectedItem as TaxObligationEntity;
            if (selectedObligation == null)
            {
                MessageBox.Show("Vui lòng chọn một Nghĩa Vụ Thuế trên lưới trên trước khi thêm Chỉ tiêu.", "Cảnh báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var tv = gridItems.View as TableView;
            if (tv == null) return;
            tv.AddNewRow();
            tv.MoveLastRow();
        }

        private void BtnDelete_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            var focusedGrid = gridItems.IsKeyboardFocusWithin ? gridItems : gridObligations;

            if (focusedGrid == gridItems)
            {
                var rootTv = gridItems.View as TableView;
                var focusedView = rootTv?.FocusedView as TableView ?? rootTv;
                var currentItem = focusedView?.Grid?.CurrentItem;

                if (currentItem is AccountTaxObligationEntity accItem)
                {
                    if (MessageBox.Show($"Xóa tài khoản tham chiếu '{accItem.AccountId}'?", "Xác nhận",
                        MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        focusedView?.CancelRowEdit();
                        if (focusedView?.Grid.ItemsSource is System.Collections.IList list && list.Contains(accItem))
                        {
                            list.Remove(accItem);
                        }
                    }
                }
                else if (currentItem is TaxObligationItemEntity item)
                {
                    if (MessageBox.Show($"Xóa chỉ tiêu '{item.ItemName}' và toàn bộ tài khoản tham chiếu?", "Xác nhận",
                        MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        rootTv?.CancelRowEdit();
                        var src = gridItems.ItemsSource as EntityCollection<TaxObligationItemEntity>;
                        src?.Remove(item);
                    }
                }
                else
                {
                    MessageBox.Show("Vui lòng chọn dòng Chỉ tiêu hoặc Tài khoản cần xóa.", "Cảnh báo",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                var sel = gridObligations.SelectedItem as TaxObligationEntity;
                if (sel == null)
                {
                    MessageBox.Show("Vui lòng chọn dòng Nghĩa Vụ Thuế cần xóa.", "Cảnh báo",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (MessageBox.Show($"Xóa nghĩa vụ '{sel.ObligationName}' và toàn bộ cấu trúc con bên dưới?", "Xác nhận",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    var tv = gridObligations.View as TableView;
                    tv?.CancelRowEdit();
                    _obligations.Remove(sel);
                    gridItems.ItemsSource = null;
                }
            }
        }

        private void Grid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Delete) return;

            var tv = (sender as GridControl)?.View as TableView;
            var focusedView = tv?.FocusedView as TableView ?? tv;
            if (focusedView == null || focusedView.ActiveEditor != null) return; // Đang sửa ô text thì không xóa

            BtnDelete_ItemClick(sender, null);
            e.Handled = true;
        }

        private void BtnBestFit_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            (gridObligations.View as TableView)?.BestFitColumns();
            (gridItems.View as TableView)?.BestFitColumns();
        }

        private void BtnExportJson_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            DesignJsonService.ExportTaxObligation(this);
        }

        private void BtnImportJson_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            if (DesignJsonService.ImportTaxObligation(this))
            {
                LoadData();
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
