using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public partial class VatDeclareDesignWindow : BaseWindow
    {
        private EntityCollection<VatDeclareItemEntity> _items = new EntityCollection<VatDeclareItemEntity>();
        private List<VatDeclareItemEntity> _origItems = new List<VatDeclareItemEntity>();
        private Dictionary<int, List<AccountVatDeclareItemEntity>> _origDetailsByItemId = new Dictionary<int, List<AccountVatDeclareItemEntity>>();

        public VatDeclareDesignWindow()
        {
            InitializeComponent();
            this.Loaded += (_, _) =>
            {
                DesignLookupHelper.EnsureAccountsLoaded();
                LoadData();
            };
        }

        public void LoadData()
        {
            try
            {
                gridMain.IsEnabled = false;
                using var adapter = AppDataAccessAdapter.Create();
                var list = new EntityCollection<VatDeclareItemEntity>();

                var prefetch = new PrefetchPath2((int)DataAccess.EntityType.VatDeclareItemEntity);
                prefetch.Add(VatDeclareItemEntity.PrefetchPathAccountVatDeclareItems);

                adapter.FetchEntityCollection(list, null, prefetch);
                _items = list;
                gridMain.ItemsSource = list;

                SnapshotOriginsForDeltaDelete(list);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải cấu hình Tờ khai thuế GTGT Mẫu 01/GTGT: " + ex.Message, "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                gridMain.IsEnabled = true;
            }
        }

        private void SnapshotOriginsForDeltaDelete(EntityCollection<VatDeclareItemEntity> list)
        {
            _origItems.Clear();
            _origDetailsByItemId.Clear();
            foreach (var it in list)
            {
                var itClone = new VatDeclareItemEntity();
                itClone.Fields = it.Fields.Clone();
                itClone.IsNew = it.IsNew;
                itClone.IsDirty = it.IsDirty;
                _origItems.Add(itClone);

                if (it.AccountVatDeclareItems != null && it.Id > 0)
                {
                    var detailClones = new List<AccountVatDeclareItemEntity>();
                    foreach (var det in it.AccountVatDeclareItems)
                    {
                        var detClone = new AccountVatDeclareItemEntity();
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
            Keyboard.ClearFocus();

            if (gridMain.View is TableView masterTv)
            {
                masterTv.PostEditor();
                masterTv.CommitEditing();
                masterTv.CloseEditor();
                masterTv.FocusedRowHandle = GridControl.InvalidRowHandle;
            }

            if (gridMain.DetailDescriptor is DataControlDetailDescriptor d)
            {
                for (int i = 0; i < gridMain.VisibleRowCount; i++)
                {
                    int rowHandle = gridMain.GetRowHandleByVisibleIndex(i);
                    if (gridMain.GetDetail(rowHandle, d) is GridControl childGrid && childGrid.View is TableView childTv)
                    {
                        childTv.PostEditor();
                        childTv.CommitEditing();
                        childTv.CloseEditor();
                        childTv.FocusedRowHandle = GridControl.InvalidRowHandle;
                    }
                }
            }

            if (gridMain.View?.FocusedView is TableView focusedTv && focusedTv != gridMain.View)
            {
                focusedTv.PostEditor();
                focusedTv.CommitEditing();
                focusedTv.CloseEditor();
                focusedTv.FocusedRowHandle = GridControl.InvalidRowHandle;
            }

            foreach (var l1 in _items)
            {
                (l1 as IEditableObject)?.EndEdit();
                if (l1.AccountVatDeclareItems != null)
                {
                    foreach (var l2 in l1.AccountVatDeclareItems)
                    {
                        (l2 as IEditableObject)?.EndEdit();
                    }
                }
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
                var uow = new UnitOfWork2();

                // 1) Xóa Delta
                var currentDetailIds = new HashSet<int>();
                foreach (var it in _items)
                {
                    if (it.AccountVatDeclareItems == null) continue;
                    foreach (var det in it.AccountVatDeclareItems)
                    {
                        if (det.Id > 0) currentDetailIds.Add(det.Id);
                    }
                }

                foreach (var kvp in _origDetailsByItemId)
                {
                    foreach (var oldDet in kvp.Value)
                    {
                        if (!currentDetailIds.Contains(oldDet.Id))
                        {
                            uow.AddForDelete(oldDet);
                        }
                    }
                }

                var currentItemIds = new HashSet<int>(_items.Where(x => x.Id > 0).Select(x => x.Id));
                foreach (var oldIt in _origItems)
                {
                    if (!currentItemIds.Contains(oldIt.Id))
                    {
                        uow.AddForDelete(oldIt);
                    }
                }

                // 2) Save Graph
                foreach (var l1 in _items)
                {
                    uow.AddForSave(l1, null, true, false);

                    if (l1.AccountVatDeclareItems == null) continue;
                    foreach (var l2 in l1.AccountVatDeclareItems)
                    {
                        if (l1.Id > 0)
                        {
                            l2.VatDeclareItemId = l1.Id;
                        }
                        l2.VatDeclareItem = l1;

                        uow.AddForSave(l2, null, true, false);
                    }
                }

                uow.Commit(adapter, true);
                LoadData();

                MessageBox.Show("✅ Đã lưu cấu hình Tờ khai thuế GTGT Mẫu 01/GTGT thành công xuống CSDL!", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Lỗi lưu cấu hình Tờ khai thuế GTGT: " + ex.Message, "Lỗi",
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

        private void DeleteFocusedRow(bool confirm)
        {
            var root = gridMain.View as TableView;
            var focusedView = root?.FocusedView as TableView ?? root;
            var grid = focusedView?.Grid;
            var item = grid?.CurrentItem;

            if (grid == null || item == null)
            {
                MessageBox.Show("Vui lòng chọn dòng cần xóa (ở Tầng 1 hoặc Tầng 2).", "Cảnh báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (confirm)
            {
                string msg;
                if (item is VatDeclareItemEntity vatItem)
                {
                    msg = $"Xóa chỉ tiêu '{vatItem.ItemName}' và toàn bộ tài khoản liên kết?";
                }
                else if (item is AccountVatDeclareItemEntity accItem)
                {
                    msg = $"Xóa dòng tài khoản '{accItem.AccountId}' (TK đối ứng: {accItem.CounterAccountId})?";
                }
                else
                {
                    msg = "Xóa dòng đang chọn?";
                }

                if (MessageBox.Show(msg, "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                    return;
            }

            focusedView.CancelRowEdit();

            if (grid.ItemsSource is System.Collections.IList list && list.Contains(item))
            {
                list.Remove(item);
            }
        }

        private void BtnDelete_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            DeleteFocusedRow(confirm: true);
        }

        private void gridMain_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Delete) return;

            var root = gridMain.View as TableView;
            var focusedView = root?.FocusedView as TableView ?? root;
            if (focusedView == null || focusedView.ActiveEditor != null) return;

            DeleteFocusedRow(confirm: true);
            e.Handled = true;
        }

        private void BtnBestFit_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            (gridMain.View as TableView)?.BestFitColumns();
        }

        private void BtnExportJson_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            DesignJsonService.ExportVatDeclare(this);
        }

        private void BtnImportJson_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            if (DesignJsonService.ImportVatDeclare(this))
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

