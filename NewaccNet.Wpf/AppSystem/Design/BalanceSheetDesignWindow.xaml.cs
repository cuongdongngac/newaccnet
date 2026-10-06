using System.Windows.Input;
using DataAccess.FactoryClasses;
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
    public partial class BalanceSheetDesignWindow : BaseWindow
    {
        private EntityCollection<ReportSectionEntity> _sections = new EntityCollection<ReportSectionEntity>();
        private Dictionary<int, decimal> _testSectionBalances = new Dictionary<int, decimal>();
        private Dictionary<int, decimal> _testItemBalances = new Dictionary<int, decimal>();
        private EntityCollection<ChartOfAccountEntity> _accounts = new EntityCollection<ChartOfAccountEntity>();

        private List<ReportSectionEntity> _origSections = new List<ReportSectionEntity>();
        private Dictionary<int, List<ReportCategoryEntity>> _origCategoriesBySectionId = new Dictionary<int, List<ReportCategoryEntity>>();
        private Dictionary<int, List<ReportItemEntity>> _origItemsByCategoryId = new Dictionary<int, List<ReportItemEntity>>();
        private Dictionary<int, List<ReportFormulaEntity>> _origFormulasByItemId = new Dictionary<int, List<ReportFormulaEntity>>();
        private Dictionary<int, List<ReportFormulaAccountEntity>> _origFormulaAccountsByFormulaId = new Dictionary<int, List<ReportFormulaAccountEntity>>();

        public BalanceSheetDesignWindow()
        {
            InitializeComponent();
            this.Loaded += (_, _) =>
            {
                LoadLookups();
                LoadData();
            };

            // Link giữa 2 grid: khi user expand Category detail row → load Items tương ứng
            // Sử dụng MasterRowExpanded event trên GridControl
            }

        private void LoadLookups()
        {
            try
            {
                using var adapter = AppDataAccessAdapter.Create();
                _accounts = new EntityCollection<ChartOfAccountEntity>();
                adapter.FetchEntityCollection(_accounts, null);

                cboAccount.ItemsSource = _accounts;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải lookup tài khoản: " + ex.Message, "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

                private void GridSections_MasterRowExpanded(object sender, RowEventArgs e)
        {
            var section = gridSections.GetRow(e.RowHandle) as ReportSectionEntity;
            if (section != null && section.ReportCategories != null && section.ReportCategories.Count > 0)
            {
                // Auto-bind to first category when expanded
                gridItems.ItemsSource = section.ReportCategories[0].ReportItems;
            }
        }

        private void CategoryView_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
        {
            var view = sender as TableView;
            if (view == null) return;
            var category = view.Grid.CurrentItem as ReportCategoryEntity;
            if (category != null && category.ReportItems != null)
            {
                gridItems.ItemsSource = category.ReportItems;
            }
        }

        public void LoadData()
        {
            try
            {
                gridSections.IsEnabled = false;
                gridItems.IsEnabled = false;
                using var adapter = AppDataAccessAdapter.Create();
                var list = new EntityCollection<ReportSectionEntity>();

                var prefetch = new PrefetchPath2((int)DataAccess.EntityType.ReportSectionEntity);
                prefetch.Add(ReportSectionEntity.PrefetchPathReportCategories)
                        .SubPath.Add(ReportCategoryEntity.PrefetchPathReportItems)
                        .SubPath.Add(ReportItemEntity.PrefetchPathReportFormulas)
                        .SubPath.Add(ReportFormulaEntity.PrefetchPathReportFormulaAccounts);

                adapter.FetchEntityCollection(list, null, prefetch);
                _sections = list;
                gridSections.ItemsSource = list;
                gridItems.ItemsSource = null;

                SnapshotOriginsForDeltaDelete(list);

                // Auto-bind gridItems vào Category đầu tiên nếu có
                if (list.Count > 0 && list[0].ReportCategories != null && list[0].ReportCategories.Count > 0)
                {
                    var firstCategory = list[0].ReportCategories[0];
                    if (firstCategory.ReportItems != null)
                    {
                        gridItems.ItemsSource = firstCategory.ReportItems;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải cấu hình BCTC: " + ex.Message, "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                gridSections.IsEnabled = true;
                gridItems.IsEnabled = true;
            }
        }

        private void SnapshotOriginsForDeltaDelete(EntityCollection<ReportSectionEntity> list)
        {
            _origSections.Clear();
            _origCategoriesBySectionId.Clear();
            _origItemsByCategoryId.Clear();
            _origFormulasByItemId.Clear();
            _origFormulaAccountsByFormulaId.Clear();

            foreach (var sec in list)
            {
                // Manual clone since CloneUsingSerializable is not available
                var secClone = new ReportSectionEntity();
                secClone.Fields = sec.Fields.Clone();
                secClone.IsNew = sec.IsNew;
                secClone.IsDirty = sec.IsDirty;
                _origSections.Add(secClone);

                if (sec.ReportCategories != null)
                {
                    var catClones = new List<ReportCategoryEntity>();
                    foreach (var cat in sec.ReportCategories)
                    {
                        var catClone = new ReportCategoryEntity();
                        catClone.Fields = cat.Fields.Clone();
                        catClone.IsNew = cat.IsNew;
                        catClone.IsDirty = cat.IsDirty;
                        catClones.Add(catClone);

                        if (cat.ReportItems != null)
                        {
                            var itemClones = new List<ReportItemEntity>();
                            foreach (var item in cat.ReportItems)
                            {
                                var itemClone = new ReportItemEntity();
                                itemClone.Fields = item.Fields.Clone();
                                itemClone.IsNew = item.IsNew;
                                itemClone.IsDirty = item.IsDirty;
                                itemClones.Add(itemClone);

                                if (item.ReportFormulas != null)
                                {
                                    var formulaClones = new List<ReportFormulaEntity>();
                                    foreach (var f in item.ReportFormulas)
                                    {
                                        var fClone = new ReportFormulaEntity();
                                        fClone.Fields = f.Fields.Clone();
                                        fClone.IsNew = f.IsNew;
                                        fClone.IsDirty = f.IsDirty;
                                        formulaClones.Add(fClone);

                                        if (f.ReportFormulaAccounts != null)
                                        {
                                            var accClones = new List<ReportFormulaAccountEntity>();
                                            foreach (var acc in f.ReportFormulaAccounts)
                                            {
                                                var accClone = new ReportFormulaAccountEntity();
                                                accClone.Fields = acc.Fields.Clone();
                                                accClone.IsNew = acc.IsNew;
                                                accClone.IsDirty = acc.IsDirty;
                                                accClones.Add(accClone);
                                            }
                                            _origFormulaAccountsByFormulaId[f.Id] = accClones;
                                        }
                                    }
                                    _origFormulasByItemId[item.Id] = formulaClones;
                                }
                            }
                            _origItemsByCategoryId[cat.Id] = itemClones;
                        }
                    }
                    _origCategoriesBySectionId[sec.Id] = catClones;
                }
            }
        }

        private void CommitEditingAllGrid()
        {
            var tvSections = gridSections.View as TableView;
            if (tvSections != null)
            {
                tvSections.CommitEditing();
                tvSections.CloseEditor();
                tvSections.FocusedRowHandle = GridControl.InvalidRowHandle;
            }

            var tvItems = gridItems.View as TableView;
            if (tvItems != null)
            {
                tvItems.CommitEditing();
                tvItems.CloseEditor();
                tvItems.FocusedRowHandle = GridControl.InvalidRowHandle;
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
                adapter.StartTransaction(System.Data.IsolationLevel.ReadCommitted, "UOW_DesignBalanceSheet");
                try
                {
                    // 1) DELETE delta (con trước, cha sau)
                    // Lv 5: ReportFormulaAccounts
                    foreach (var sec in _sections)
                    {
                        if (sec.ReportCategories == null) continue;
                        foreach (var cat in sec.ReportCategories)
                        {
                            if (cat.ReportItems == null) continue;
                            foreach (var item in cat.ReportItems)
                            {
                                if (item.ReportFormulas == null) continue;
                                foreach (var f in item.ReportFormulas)
                                {
                                    if (f.ReportFormulaAccounts == null || f.Id <= 0) continue;
                                    if (!_origFormulaAccountsByFormulaId.TryGetValue(f.Id, out var origAccs)) continue;
                                    var currentAccIds = new HashSet<int>(f.ReportFormulaAccounts.Where(x => x.Id > 0).Select(x => x.Id));
                                    foreach (var oldAcc in origAccs)
                                    {
                                        if (!currentAccIds.Contains(oldAcc.Id))
                                        {
                                            adapter.DeleteEntity(oldAcc);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    // Lv 4: ReportFormulas
                    foreach (var sec in _sections)
                    {
                        if (sec.ReportCategories == null) continue;
                        foreach (var cat in sec.ReportCategories)
                        {
                            if (cat.ReportItems == null) continue;
                            foreach (var item in cat.ReportItems)
                            {
                                if (item.ReportFormulas == null || item.Id <= 0) continue;
                                if (!_origFormulasByItemId.TryGetValue(item.Id, out var origFormulas)) continue;
                                var currentFormulaIds = new HashSet<int>(item.ReportFormulas.Where(x => x.Id > 0).Select(x => x.Id));
                                foreach (var oldF in origFormulas)
                                {
                                    if (!currentFormulaIds.Contains(oldF.Id))
                                    {
                                        adapter.DeleteEntity(oldF);
                                    }
                                }
                            }
                        }
                    }

                    // Lv 3: ReportItems
                    foreach (var sec in _sections)
                    {
                        if (sec.ReportCategories == null) continue;
                        foreach (var cat in sec.ReportCategories)
                        {
                            if (cat.ReportItems == null || cat.Id <= 0) continue;
                            if (!_origItemsByCategoryId.TryGetValue(cat.Id, out var origItems)) continue;
                            var currentItemIds = new HashSet<int>(cat.ReportItems.Where(x => x.Id > 0).Select(x => x.Id));
                            foreach (var oldItem in origItems)
                            {
                                if (!currentItemIds.Contains(oldItem.Id))
                                {
                                    adapter.DeleteEntity(oldItem);
                                }
                            }
                        }
                    }

                    // Lv 2: ReportCategories
                    foreach (var sec in _sections)
                    {
                        if (sec.ReportCategories == null || sec.Id <= 0) continue;
                        if (!_origCategoriesBySectionId.TryGetValue(sec.Id, out var origCats)) continue;
                        var currentCatIds = new HashSet<int>(sec.ReportCategories.Where(x => x.Id > 0).Select(x => x.Id));
                        foreach (var oldCat in origCats)
                        {
                            if (!currentCatIds.Contains(oldCat.Id))
                            {
                                adapter.DeleteEntity(oldCat);
                            }
                        }
                    }

                    // Lv 1: Sections
                    {
                        var currentSectionIds = new HashSet<int>(_sections.Where(x => x.Id > 0).Select(x => x.Id));
                        foreach (var oldSec in _origSections)
                        {
                            if (!currentSectionIds.Contains(oldSec.Id))
                            {
                                adapter.DeleteEntity(oldSec);
                            }
                        }
                    }

                    // 2) Tạo object graph và save từng Section với recurse
                    // LLBLGen sẽ tự động save các entities con và cập nhật FK
                    foreach (var sec in _sections)
                    {
                        adapter.SaveEntity(sec, refetchAfterSave: true, recurse: true);
                    }

                    adapter.Commit();

                    // Refresh dữ liệu gốc sau save
                    LoadData();

                    MessageBox.Show("✅ Lưu cấu hình Bảng Cân Đối Kế Toán thành công!", "Thông báo",
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
                MessageBox.Show("❌ Lỗi lưu cấu hình CĐKT: " + ex.Message, "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnNewSection_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            var tv = gridSections.View as TableView;
            if (tv == null) return;
            tv.AddNewRow();
            tv.MoveLastRow();
        }

        private void BtnNewCategory_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            var selectedSection = gridSections.SelectedItem as ReportSectionEntity;
            if (selectedSection == null)
            {
                MessageBox.Show("Vui lòng chọn một Mục lớn trên lưới trên trước khi thêm Nhóm chỉ tiêu.", "Cảnh báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Thêm Category mới vào Section đang chọn
            // ReportCategories is read-only, so we add to existing collection
            var newCat = new ReportCategoryEntity
            {
                ReportSectionId = selectedSection.Id,
                IsNew = true
            };
            selectedSection.ReportCategories.Add(newCat);
        }

        private void BtnNewItem_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            if (gridItems.ItemsSource == null)
            {
                MessageBox.Show("Vui lòng chọn một Nhóm chỉ tiêu trên lưới trên trước khi thêm Chỉ tiêu.", "Cảnh báo",
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
            var focusedGrid = gridItems.IsKeyboardFocusWithin ? gridItems : gridSections;

            if (focusedGrid == gridSections)
            {
                // Xóa Section từ gridSections
                var sel = gridSections.SelectedItem as IEntityCore;
                if (sel == null)
                {
                    MessageBox.Show("Vui lòng chọn dòng cần xóa.", "Cảnh báo",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (sel is ReportSectionEntity section)
                {
                    if (MessageBox.Show($"Xóa mục lớn '{section.SectionName}' và toàn bộ cấu trúc con bên dưới?", "Xác nhận",
                        MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        var src = gridSections.ItemsSource as EntityCollection<ReportSectionEntity>;
                        src?.Remove(section);
                        gridItems.ItemsSource = null; // Clear gridItems khi xóa section
                    }
                }
                else if (sel is ReportCategoryEntity category)
                {
                    if (MessageBox.Show($"Xóa nhóm chỉ tiêu '{category.CategoryName}' và toàn bộ cấu trúc con bên dưới?", "Xác nhận",
                        MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        // Tìm parent section và remove category
                        foreach (var sec in _sections)
                        {
                            if (sec.ReportCategories != null && sec.ReportCategories.Contains(category))
                            {
                                sec.ReportCategories.Remove(category);
                                gridItems.ItemsSource = null;
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                // Xóa Item, Formula hoặc Account từ gridItems
                var sel = gridItems.SelectedItem as IEntityCore;
                if (sel == null)
                {
                    MessageBox.Show("Vui lòng chọn dòng cần xóa.", "Cảnh báo",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (sel is ReportFormulaAccountEntity account)
                {
                    if (MessageBox.Show($"Xóa tài khoản tham chiếu?", "Xác nhận",
                        MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        // Tìm parent formula và remove account
                        var src = gridItems.ItemsSource as EntityCollection<ReportItemEntity>;
                        if (src != null)
                        {
                            foreach (var item in src)
                            {
                                if (item.ReportFormulas != null)
                                {
                                    foreach (var formula in item.ReportFormulas)
                                    {
                                        if (formula.ReportFormulaAccounts != null && formula.ReportFormulaAccounts.Contains(account))
                                        {
                                            formula.ReportFormulaAccounts.Remove(account);
                                            return;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else if (sel is ReportFormulaEntity formula)
                {
                    if (MessageBox.Show($"Xóa công thức và toàn bộ tài khoản tham chiếu?", "Xác nhận",
                        MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        // Tìm parent item và remove formula
                        var src = gridItems.ItemsSource as EntityCollection<ReportItemEntity>;
                        if (src != null)
                        {
                            foreach (var item in src)
                            {
                                if (item.ReportFormulas != null && item.ReportFormulas.Contains(formula))
                                {
                                    item.ReportFormulas.Remove(formula);
                                    return;
                                }
                            }
                        }
                    }
                }
                else if (sel is ReportItemEntity item)
                {
                    if (MessageBox.Show($"Xóa chỉ tiêu '{item.EntryName}' và toàn bộ cấu trúc con bên dưới?", "Xác nhận",
                        MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        var src = gridItems.ItemsSource as EntityCollection<ReportItemEntity>;
                        src?.Remove(item);
                    }
                }
            }
        }

                private async void BtnTestCalc_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;
                _testSectionBalances.Clear();
                _testItemBalances.Clear();

                var dtos = await System.Threading.Tasks.Task.Run(() =>
                {
                    using var adapter = NewaccNet.Wpf.AppSystem.AppDataAccessAdapter.Create();
                    var service = new NewaccNet.Reports.BalanceSheet.BalanceSheetService();
                    return service.GenerateReport(adapter);
                });

                foreach (var dto in dtos)
                {
                    if (!_testSectionBalances.ContainsKey(dto.SectionId)) _testSectionBalances[dto.SectionId] = 0;
                    _testSectionBalances[dto.SectionId] += dto.EndAmount;

                    if (!_testItemBalances.ContainsKey(dto.ItemId)) _testItemBalances[dto.ItemId] = 0;
                    _testItemBalances[dto.ItemId] += dto.EndAmount;
                }

                gridSections.RefreshData();
                gridItems.RefreshData();
                MessageBox.Show("Đã test tính toán và gắn số dư lên lưới thành công!", "Test CĐKT", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

        private void GridSections_CustomUnboundColumnData(object sender, DevExpress.Xpf.Grid.GridColumnDataEventArgs e)
        {
            if (e.Column.FieldName == "TestBalance" && e.IsGetData)
            {
                if (gridSections.GetRowByListIndex(e.ListSourceRowIndex) is DataAccess.EntityClasses.ReportSectionEntity section)
                {
                    if (_testSectionBalances.TryGetValue(section.Id, out decimal val))
                        e.Value = val;
                }
            }
        }

        private void GridItems_CustomUnboundColumnData(object sender, DevExpress.Xpf.Grid.GridColumnDataEventArgs e)
        {
            if (e.Column.FieldName == "TestBalance" && e.IsGetData)
            {
                if (gridItems.GetRowByListIndex(e.ListSourceRowIndex) is DataAccess.EntityClasses.ReportItemEntity item)
                {
                    if (_testItemBalances.TryGetValue(item.Id, out decimal val))
                        e.Value = val;
                }
            }
        }

        private void BtnBestFit_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            (gridSections.View as TableView)?.BestFitColumns();
            (gridItems.View as TableView)?.BestFitColumns();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}






