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
    public partial class VatReturnDesignWindow : BaseWindow
    {
        private EntityCollection<Vat3TaxItemEntity> _items = new EntityCollection<Vat3TaxItemEntity>();
        private List<Vat3TaxItemEntity> _origItems = new List<Vat3TaxItemEntity>();
        private Dictionary<int, List<AccountVat3ItemEntity>> _origDetailsByItemId = new Dictionary<int, List<AccountVat3ItemEntity>>();

        public VatReturnDesignWindow()
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
                var list = new EntityCollection<Vat3TaxItemEntity>();

                var prefetch = new PrefetchPath2((int)DataAccess.EntityType.Vat3TaxItemEntity);
                prefetch.Add(Vat3TaxItemEntity.PrefetchPathAccountVat3Items);

                adapter.FetchEntityCollection(list, null, prefetch);
                _items = list;
                gridMain.ItemsSource = list;

                SnapshotOriginsForDeltaDelete(list);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải cấu hình Tờ khai thuế GTGT: " + ex.Message, "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                gridMain.IsEnabled = true;
            }
        }

        private void SnapshotOriginsForDeltaDelete(EntityCollection<Vat3TaxItemEntity> list)
        {
            _origItems.Clear();
            _origDetailsByItemId.Clear();
            foreach (var it in list)
            {
                var itClone = new Vat3TaxItemEntity();
                itClone.Fields = it.Fields.Clone();
                itClone.IsNew = it.IsNew;
                itClone.IsDirty = it.IsDirty;
                _origItems.Add(itClone);

                if (it.AccountVat3Items != null && it.Id > 0)
                {
                    var detailClones = new List<AccountVat3ItemEntity>();
                    foreach (var det in it.AccountVat3Items)
                    {
                        var detClone = new AccountVat3ItemEntity();
                        detClone.Fields = det.Fields.Clone();
                        detClone.IsNew = det.IsNew;
                        detClone.IsDirty = det.IsDirty;
                        detailClones.Add(detClone);
                    }
                    _origDetailsByItemId[it.Id] = detailClones;
                }
            }
        }

        /// <summary>
        /// Ép chốt dữ liệu đang sửa trên mọi View (cha và con) và gọi EndEdit() để LLBLGen kích hoạt IsDirty.
        /// </summary>
        private void CommitEditingAllGrid()
        {
            Keyboard.ClearFocus();

            // 1. Chốt Master View
            if (gridMain.View is TableView masterTv)
            {
                masterTv.PostEditor();
                masterTv.CommitEditing();
                masterTv.CloseEditor();
                masterTv.FocusedRowHandle = GridControl.InvalidRowHandle;
            }

            // 2. Chốt tất cả Detail View đang mở
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

            // 3. Chốt View ảo đang focus nếu có
            if (gridMain.View?.FocusedView is TableView focusedTv && focusedTv != gridMain.View)
            {
                focusedTv.PostEditor();
                focusedTv.CommitEditing();
                focusedTv.CloseEditor();
                focusedTv.FocusedRowHandle = GridControl.InvalidRowHandle;
            }

            // 4. Ép hoàn tất chế độ Edit IEditableObject trên mọi Entity
            foreach (var l1 in _items)
            {
                (l1 as IEditableObject)?.EndEdit();
                if (l1.AccountVat3Items != null)
                {
                    foreach (var l2 in l1.AccountVat3Items)
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

                // 1) XÓA DELTA (con trước, cha sau)
                var currentDetailIds = new HashSet<int>();
                foreach (var it in _items)
                {
                    if (it.AccountVat3Items == null) continue;
                    foreach (var det in it.AccountVat3Items)
                    {
                        if (det.Id > 0) currentDetailIds.Add(det.Id);
                    }
                }

                // Xóa chi tiết (AccountVat3Item)
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

                // Xóa cha (Vat3TaxItem)
                var currentItemIds = new HashSet<int>(_items.Where(x => x.Id > 0).Select(x => x.Id));
                foreach (var oldIt in _origItems)
                {
                    if (!currentItemIds.Contains(oldIt.Id))
                    {
                        uow.AddForDelete(oldIt);
                    }
                }

                // 2) SAVE TOÀN BỘ GRAPH BẰNG UNIT OF WORK (Nạp trực tiếp cả 2 tầng vào UOW)
                foreach (var l1 in _items)
                {
                    uow.AddForSave(l1, null, true, false);

                    if (l1.AccountVat3Items == null) continue;
                    foreach (var l2 in l1.AccountVat3Items)
                    {
                        // Đồng bộ khóa ngoại FK
                        if (l1.Id > 0)
                        {
                            l2.Vat3TaxItemId = l1.Id;
                        }
                        l2.Vat3TaxItem = l1;

                        uow.AddForSave(l2, null, true, false);
                    }
                }

                // 3) THỰC THI TRANSACTION QUA UNIT OF WORK
                uow.Commit(adapter, true);

                // 4) Nạp lại dữ liệu tươi mới từ DB
                LoadData();

                MessageBox.Show("✅ Đã lưu cấu hình Tờ khai thuế GTGT thành công xuống CSDL!", "Thông báo",
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
                if (item is Vat3TaxItemEntity vatItem)
                {
                    msg = $"Xóa chỉ tiêu '{vatItem.ItemName}' và toàn bộ tài khoản liên kết?";
                }
                else if (item is AccountVat3ItemEntity accItem)
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

            // Hủy trạng thái edit nếu đang mở
            focusedView.CancelRowEdit();

            // Chỉ xóa ra khỏi danh sách nguồn trong bộ nhớ (không gọi DB)
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
            if (focusedView == null || focusedView.ActiveEditor != null) return; // Đang sửa trong ô text thì để Delete xóa ký tự

            DeleteFocusedRow(confirm: true);
            e.Handled = true;
        }

        private void BtnBestFit_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            (gridMain.View as TableView)?.BestFitColumns();
        }

        private void BtnExportJson_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            DesignJsonService.ExportVatReturn(this);
        }

        private void BtnImportJson_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            if (DesignJsonService.ImportVatReturn(this))
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
