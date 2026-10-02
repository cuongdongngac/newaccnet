using NewaccNet.Wpf.AppSystem.Directory;
using System;
using System.Linq;
using System.Windows;
using DevExpress.Xpf.Core;
using DataAccess.EntityClasses;
using DataAccess.HelperClasses;
using SD.LLBLGen.Pro.ORMSupportClasses;
using NewaccNet.Wpf.AppSystem.Helpers;
using System.Collections.Generic;
using DataAccess;

namespace NewaccNet.Wpf.AppSystem.Voucher
{
    public class VoucherListDto
    {
        public int Id { get; set; }
        public string VoucherNo { get; set; }
        public DateTime VoucherDate { get; set; }
        public string Contents { get; set; }
        public int Bookflag { get; set; }
        public string TransactionType { get; set; }
    }

    public partial class VoucherListView : BaseDictionaryWindow
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int StatusIndex { get; set; }

        public VoucherListView()
        {
            InitializeComponent();
            InitDefaults();
        }

        private void InitDefaults()
        {
            var types = AccountTypeMapping.GetAccountTypes();
            types.Insert(0, new AccountTypeMapping { Code = "", Name = "Tất cả" });
            cboTransactionType.ItemsSource = types;
            cboTransactionType.EditValue = "";

            DateTime today = DateTime.Today;
            dtFrom.DateTime = new DateTime(today.Year, today.Month, 1);
            dtTo.DateTime = today;
            cboStatus.SelectedIndex = 0; // Tất cả
            
            // Load lại state đã lưu
            UserPreferencesHelper.LoadState(this);
        }

        public override void LoadData()
        {
            // Do not load all data by default on start
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            DoSearch();
        }

        private void DoSearch()
        {
            try
            {
                ShowLoading("Đang tải danh sách chứng từ...");
                
                FromDate = dtFrom.DateTime.Date;
                ToDate = dtTo.DateTime.Date;
                StatusIndex = cboStatus.SelectedIndex;
                string transactionTypeFilter = cboTransactionType.EditValue?.ToString();
                
                // Lưu state
                UserPreferencesHelper.SaveState(this);
                
                using (var adapter = AppDataAccessAdapter.Create())
                {
                    var vouchers = new EntityCollection<JournalVoucherEntity>();
                    
                    var filter = new RelationPredicateBucket();
                    filter.PredicateExpression.Add(JournalVoucherFields.VoucherDate >= FromDate);
                    filter.PredicateExpression.Add(JournalVoucherFields.VoucherDate <= ToDate);
                    
                    if (StatusIndex == 1) // Chưa ghi sổ
                    {
                        filter.PredicateExpression.Add(JournalVoucherFields.Bookflag == false);
                    }
                    else if (StatusIndex == 2) // Đã ghi sổ
                    {
                        filter.PredicateExpression.Add(JournalVoucherFields.Bookflag == true);
                    }
                    
                    var prefetch = new PrefetchPath2((int)DataAccess.EntityType.JournalVoucherEntity);
                    prefetch.Add(JournalVoucherEntity.PrefetchPathJournalEntries)
                            .SubPath.Add(JournalEntryEntity.PrefetchPathChartOfAccount);
                    
                    adapter.FetchEntityCollection(vouchers, filter, 0, null, prefetch);
                    
                    var listDto = new List<VoucherListDto>();
                    var allTypes = AccountTypeMapping.GetAccountTypes();

                    foreach (var v in vouchers)
                    {
                        string transTypeCode = "";
                        string transTypeName = "Khác";
                        
                        foreach (var en in v.JournalEntries)
                        {
                            if (en.ChartOfAccount != null && !string.IsNullOrEmpty(en.ChartOfAccount.CategoryId))
                            {
                                transTypeCode = en.ChartOfAccount.CategoryId;
                                break;
                            }
                        }
                        
                        if (!string.IsNullOrEmpty(transTypeCode))
                        {
                            var matchedType = allTypes.FirstOrDefault(t => t.Code == transTypeCode);
                            if (matchedType != null)
                            {
                                transTypeName = matchedType.Name;
                            }
                        }
                        
                        // Nếu có filter loại giao dịch và chứng từ này không khớp thì bỏ qua
                        if (!string.IsNullOrEmpty(transactionTypeFilter) && transTypeCode != transactionTypeFilter)
                        {
                            continue;
                        }
                        
                        listDto.Add(new VoucherListDto
                        {
                            Id = v.Id,
                            VoucherNo = v.VoucherNo,
                            VoucherDate = v.VoucherDate ?? DateTime.Today,
                            Contents = v.Contents,
                            Bookflag = v.Bookflag ? 1 : 0,
                            TransactionType = transTypeName
                        });
                    }
                    
                    gridControl.ItemsSource = null;
                    gridControl.ItemsSource = listDto;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi lấy dữ liệu: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                HideLoading();
            }
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e) => DoSearch();

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var frm = new VoucherEntryWindow(null);
            frm.ShowDialog();
            DoSearch(); 
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e) => EditSelected();

        private void GridControl_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var view = gridControl.View as DevExpress.Xpf.Grid.TableView;
            if (view != null && view.CalcHitInfo(e.OriginalSource as DependencyObject).InRowCell)
            {
                EditSelected();
            }
        }

        private void EditSelected()
        {
            var selected = gridControl.SelectedItem as VoucherListDto;
            if (selected != null)
            {
                var frm = new VoucherEntryWindow(selected.Id);
                frm.ShowDialog();
                DoSearch();
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var selected = gridControl.SelectedItem as VoucherListDto;
            if (selected != null)
            {
                if (MessageBox.Show($"Bạn có chắc chắn muốn xóa chứng từ '{selected.VoucherNo}'?\n(Hệ thống sẽ tự động xóa các bút toán chi tiết liên quan)", "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    try
                    {
                        using (var adapter = AppDataAccessAdapter.Create())
                        {
                            var entity = new JournalVoucherEntity(selected.Id);
                            entity.IsNew = false;
                            adapter.DeleteEntity(entity);
                        }
                        DoSearch();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi xóa: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
