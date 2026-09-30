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
    public partial class BusinessResultDesignWindow : BaseWindow
    {
        private EntityCollection<BusinessResultItemEntity> _items = new EntityCollection<BusinessResultItemEntity>();
        private EntityCollection<ChartOfAccountEntity> _accounts = new EntityCollection<ChartOfAccountEntity>();

        private List<BusinessResultItemEntity> _origItems = new List<BusinessResultItemEntity>();
        private Dictionary<int, List<AccountBusinessResultEntity>> _origDetailsByItemId = new Dictionary<int, List<AccountBusinessResultEntity>>();

        public BusinessResultDesignWindow()
        {
            InitializeComponent();
            this.Loaded += (_, _) =>
            {
                LoadLookups();
                LoadData();
            };
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

        public void LoadData()
        {
            try
            {
                gridMain.IsEnabled = false;
                using var adapter = AppDataAccessAdapter.Create();
                var list = new EntityCollection<BusinessResultItemEntity>();

                var prefetch = new PrefetchPath2((int)DataAccess.EntityType.BusinessResultItemEntity);
                prefetch.Add(BusinessResultItemEntity.PrefetchPathAccountBusinessResults);

                adapter.FetchEntityCollection(list, null, prefetch);
                _items = list;
                gridMain.ItemsSource = list;

                SnapshotOriginsForDeltaDelete(list);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải cấu hình KQKD: " + ex.Message, "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                gridMain.IsEnabled = true;
            }
        }

        private void SnapshotOriginsForDeltaDelete(EntityCollection<BusinessResultItemEntity> list)
        {
            _origItems.Clear();
            _origDetailsByItemId.Clear();
            foreach (var it in list)
            {
                // Manual clone since CloneUsingSerializable is not available
                var itClone = new BusinessResultItemEntity();
                itClone.Fields = it.Fields.Clone();
                itClone.IsNew = it.IsNew;
                itClone.IsDirty = it.IsDirty;
                _origItems.Add(itClone);

                if (it.AccountBusinessResults != null && it.Id > 0)
                {
                    var detailClones = new List<AccountBusinessResultEntity>();
                    foreach (var det in it.AccountBusinessResults)
                    {
                        var detClone = new AccountBusinessResultEntity();
                        detClone.Fields = det.Fields.Clone();
                        detClone.IsNew = det.IsNew;
                        detClone.IsDirty = det.IsDirty;
                        detailClones.Add(detClone);
                    }
                    _origDetailsByItemId[it.Id] = detailClones;
                }
            }
        }

        private void CommitEditingAllGrid()
        {
            var tv = gridMain.View as TableView;
            if (tv != null)
            {
                tv.CommitEditing();
                tv.CloseEditor();
                tv.FocusedRowHandle = GridControl.InvalidRowHandle;
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
                adapter.StartTransaction(System.Data.IsolationLevel.ReadCommitted, "UOW_DesignBusinessResult");
                try
                {
                    // 1) DELETE delta (con trước, cha sau)
                    // Lv 2: AccountBusinessResults
                    foreach (var it in _items)
                    {
                        if (it.AccountBusinessResults == null || it.Id <= 0) continue;
                        if (!_origDetailsByItemId.TryGetValue(it.Id, out var origDetails)) continue;
                        var currentDetailIds = new HashSet<int>(it.AccountBusinessResults.Where(x => x.Id > 0).Select(x => x.Id));
                        foreach (var oldDet in origDetails)
                        {
                            if (!currentDetailIds.Contains(oldDet.Id))
                            {
                                adapter.DeleteEntity(oldDet);
                            }
                        }
                    }

                    // Lv 1: Items
                    {
                        var currentItemIds = new HashSet<int>(_items.Where(x => x.Id > 0).Select(x => x.Id));
                        foreach (var oldIt in _origItems)
                        {
                            if (!currentItemIds.Contains(oldIt.Id))
                            {
                                adapter.DeleteEntity(oldIt);
                            }
                        }
                    }

                    // 2) Tạo object graph và save từng Item với recurse
                    // LLBLGen sẽ tự động save các entities con và cập nhật FK
                    foreach (var it in _items)
                    {
                        adapter.SaveEntity(it, refetchAfterSave: true, recurse: true);
                    }

                    adapter.Commit();

                    // Refresh dữ liệu gốc sau save
                    LoadData();

                    MessageBox.Show("✅ Lưu cấu hình Báo cáo KQ HĐKD thành công!", "Thông báo",
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
                MessageBox.Show("❌ Lỗi lưu cấu hình KQ HĐKD: " + ex.Message, "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnNew_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            var tv = gridMain.View as TableView;
            if (tv == null) return;
            tv.AddNewRow();
            tv.MoveLastRow();
        }

        private void BtnDelete_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            var sel = gridMain.SelectedItem as BusinessResultItemEntity;
            if (sel == null)
            {
                MessageBox.Show("Vui lòng chọn dòng Chỉ tiêu KQKD cần xóa trên lưới chính.", "Cảnh báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (MessageBox.Show($"Xóa chỉ tiêu '{sel.ItemName}' và toàn bộ công thức con?", "Xác nhận",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                var src = gridMain.ItemsSource as EntityCollection<BusinessResultItemEntity>;
                src?.Remove(sel);
            }
        }

        private void BtnBestFit_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            (gridMain.View as TableView)?.BestFitColumns();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
