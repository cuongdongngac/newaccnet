using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using DevExpress.Xpf.Grid;
using DataAccess.EntityClasses;
using DataAccess.HelperClasses;
using SD.LLBLGen.Pro.ORMSupportClasses;
using NewaccNet.Wpf.Views.Base;

namespace NewaccNet.Wpf.AppSystem.Design
{
    public partial class CashFlowDesignWindow : BaseWindow
    {
        private EntityCollection<CashFlowCategoryEntity> _categories = new EntityCollection<CashFlowCategoryEntity>();
        private EntityCollection<ChartOfAccountEntity> _accounts = new EntityCollection<ChartOfAccountEntity>();

        private List<CashFlowCategoryEntity> _origCategories = new List<CashFlowCategoryEntity>();
        private Dictionary<int, List<CashFlowItemEntity>> _origItemsByCategoryId = new Dictionary<int, List<CashFlowItemEntity>>();
        private Dictionary<int, List<AccountCashFlowItemEntity>> _origAccountItemsByItemId = new Dictionary<int, List<AccountCashFlowItemEntity>>();

        public CashFlowDesignWindow()
        {
            InitializeComponent();
            this.Loaded += (_, _) =>
            {
                LoadLookups();
                LoadData();
            };

            // Link giữa 2 grid: khi chọn Category trên gridCategories → load Items tương ứng
            gridCategories.SelectedItemChanged += GridCategories_SelectedItemChanged;
        }

        private void LoadLookups()
        {
            try
            {
                using var adapter = AppDataAccessAdapter.Create();
                _accounts = new EntityCollection<ChartOfAccountEntity>();
                adapter.FetchEntityCollection(_accounts, null);

                cboAccDetail.ItemsSource = _accounts;
                cboCounterDetail.ItemsSource = _accounts;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải lookup tài khoản: " + ex.Message, "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GridCategories_SelectedItemChanged(object sender, SelectedItemChangedEventArgs e)
        {
            var selectedCategory = gridCategories.SelectedItem as CashFlowCategoryEntity;
            if (selectedCategory != null)
            {
                // Bind gridItems vào CashFlowItems của Category đã chọn
                gridItems.ItemsSource = selectedCategory.CashFlowItems;
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
                gridCategories.IsEnabled = false;
                gridItems.IsEnabled = false;

                using var adapter = AppDataAccessAdapter.Create();
                var list = new EntityCollection<CashFlowCategoryEntity>();

                var prefetch = new PrefetchPath2((int)DataAccess.EntityType.CashFlowCategoryEntity);
                // 3 tầng: Category → Items → AccountCashFlowItems
                prefetch.Add(CashFlowCategoryEntity.PrefetchPathCashFlowItems)
                        .SubPath.Add(CashFlowItemEntity.PrefetchPathAccountCashFlowItems);

                adapter.FetchEntityCollection(list, null, prefetch);
                _categories = list;
                gridCategories.ItemsSource = list;

                SnapshotOriginsForDeltaDelete(list);

                // Auto-select category đầu tiên để load items
                if (list.Count > 0)
                {
                    gridCategories.SelectedItem = list[0];
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải cấu hình LCTT: " + ex.Message, "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                gridCategories.IsEnabled = true;
                gridItems.IsEnabled = true;
            }
        }

        private void SnapshotOriginsForDeltaDelete(EntityCollection<CashFlowCategoryEntity> list)
        {
            _origCategories.Clear();
            _origItemsByCategoryId.Clear();
            _origAccountItemsByItemId.Clear();

            foreach (var cat in list)
            {
                // Manual clone since CloneUsingSerializable is not available
                var catClone = new CashFlowCategoryEntity();
                catClone.Fields = cat.Fields.Clone();
                catClone.IsNew = cat.IsNew;
                catClone.IsDirty = cat.IsDirty;
                _origCategories.Add(catClone);

                if (cat.CashFlowItems != null)
                {
                    var itemClones = new List<CashFlowItemEntity>();
                    foreach (var item in cat.CashFlowItems)
                    {
                        var itemClone = new CashFlowItemEntity();
                        itemClone.Fields = item.Fields.Clone();
                        itemClone.IsNew = item.IsNew;
                        itemClone.IsDirty = item.IsDirty;
                        itemClones.Add(itemClone);

                        if (item.AccountCashFlowItems != null)
                        {
                            var accItemClones = new List<AccountCashFlowItemEntity>();
                            foreach (var accItem in item.AccountCashFlowItems)
                            {
                                var accItemClone = new AccountCashFlowItemEntity();
                                accItemClone.Fields = accItem.Fields.Clone();
                                accItemClone.IsNew = accItem.IsNew;
                                accItemClone.IsDirty = accItem.IsDirty;
                                accItemClones.Add(accItemClone);
                            }
                            _origAccountItemsByItemId[item.Id] = accItemClones;
                        }
                    }
                    _origItemsByCategoryId[cat.Id] = itemClones;
                }
            }
        }

        private void CommitEditingAllGrid()
        {
            var tvCat = gridCategories.View as TableView;
            if (tvCat != null)
            {
                tvCat.CommitEditing();
                tvCat.CloseEditor();
                tvCat.FocusedRowHandle = GridControl.InvalidRowHandle;
            }

            var tvItem = gridItems.View as TableView;
            if (tvItem != null)
            {
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

                using var adapter = AppDataAccessAdapter.Create();
                adapter.StartTransaction(System.Data.IsolationLevel.ReadCommitted, "UOW_DesignCashFlow");
                try
                {
                    // 1) DELETE delta (con trước, cha sau)
                    // Lv 3: AccountCashFlowItems
                    foreach (var cat in _categories)
                    {
                        if (cat.CashFlowItems == null) continue;
                        foreach (var item in cat.CashFlowItems)
                        {
                            if (item.AccountCashFlowItems == null || item.Id <= 0) continue;
                            if (!_origAccountItemsByItemId.TryGetValue(item.Id, out var origAccItems)) continue;
                            var currentAccItemIds = new HashSet<int>(item.AccountCashFlowItems.Where(x => x.Id > 0).Select(x => x.Id));
                            foreach (var oldAccItem in origAccItems)
                            {
                                if (!currentAccItemIds.Contains(oldAccItem.Id))
                                {
                                    adapter.DeleteEntity(oldAccItem);
                                }
                            }
                        }
                    }

                    // Lv 2: CashFlowItems
                    foreach (var cat in _categories)
                    {
                        if (cat.Id <= 0 || cat.CashFlowItems == null) continue;
                        if (!_origItemsByCategoryId.TryGetValue(cat.Id, out var origItems)) continue;
                        var currentItemIds = new HashSet<int>(cat.CashFlowItems.Where(x => x.Id > 0).Select(x => x.Id));
                        foreach (var oldItem in origItems)
                        {
                            if (!currentItemIds.Contains(oldItem.Id))
                            {
                                adapter.DeleteEntity(oldItem);
                            }
                        }
                    }

                    // Lv 1: Categories
                    {
                        var currentCategoryIds = new HashSet<int>(_categories.Where(x => x.Id > 0).Select(x => x.Id));
                        foreach (var oldCat in _origCategories)
                        {
                            if (!currentCategoryIds.Contains(oldCat.Id))
                            {
                                adapter.DeleteEntity(oldCat);
                            }
                        }
                    }

                    // 2) Tạo object graph và save từng Category với recurse
                    // LLBLGen sẽ tự động save các entities con và cập nhật FK
                    foreach (var cat in _categories)
                    {
                        adapter.SaveEntity(cat, refetchAfterSave: true, recurse: true);
                    }

                    adapter.Commit();

                    // Refresh dữ liệu gốc sau save
                    LoadData();

                    MessageBox.Show("✅ Lưu cấu hình Báo cáo Lưu Chuyển Tiền tệ thành công!", "Thông báo",
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
                MessageBox.Show("❌ Lỗi lưu cấu hình LCTT: " + ex.Message, "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnNewCategory_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            var tv = gridCategories.View as TableView;
            if (tv == null) return;
            tv.AddNewRow();
            tv.MoveLastRow();
        }

        private void BtnNewItem_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            var selectedCategory = gridCategories.SelectedItem as CashFlowCategoryEntity;
            if (selectedCategory == null)
            {
                MessageBox.Show("Vui lòng chọn một Nhóm LCTT trên lưới trên trước khi thêm Chỉ tiêu.", "Cảnh báo",
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
            // Xóa dòng đang chọn trên grid nào đang có focus
            var focusedGrid = gridItems.IsKeyboardFocusWithin ? gridItems : gridCategories;

            if (focusedGrid == gridItems)
            {
                var sel = gridItems.SelectedItem as IEntityCore;
                if (sel == null)
                {
                    MessageBox.Show("Vui lòng chọn dòng Chỉ tiêu LCTT cần xóa.", "Cảnh báo",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (sel is CashFlowItemEntity item)
                {
                    if (MessageBox.Show($"Xóa chỉ tiêu '{item.ItemName}' và toàn bộ tài khoản tham chiếu?", "Xác nhận",
                        MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        var src = gridItems.ItemsSource as EntityCollection<CashFlowItemEntity>;
                        src?.Remove(item);
                    }
                }
            }
            else
            {
                var sel = gridCategories.SelectedItem as IEntityCore;
                if (sel == null)
                {
                    MessageBox.Show("Vui lòng chọn dòng Nhóm LCTT cần xóa.", "Cảnh báo",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (sel is CashFlowCategoryEntity category)
                {
                    if (MessageBox.Show($"Xóa nhóm '{category.CashFlowName}' và toàn bộ cấu trúc con bên dưới?", "Xác nhận",
                        MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        var src = gridCategories.ItemsSource as EntityCollection<CashFlowCategoryEntity>;
                        src?.Remove(category);
                        gridItems.ItemsSource = null; // Clear gridItems khi xóa category
                    }
                }
            }
        }

        private void BtnBestFit_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            (gridCategories.View as TableView)?.BestFitColumns();
            (gridItems.View as TableView)?.BestFitColumns();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
