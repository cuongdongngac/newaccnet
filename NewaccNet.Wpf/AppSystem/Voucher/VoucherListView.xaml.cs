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
using DataAccess.FactoryClasses;
using SD.LLBLGen.Pro.QuerySpec;
using SD.LLBLGen.Pro.QuerySpec.Adapter;

namespace NewaccNet.Wpf.AppSystem.Voucher
{
    public class VoucherListDto
    {
        public int Id { get; set; }
        public string VoucherNo { get; set; }
        public DateTime VoucherDate { get; set; }
        public string Contents { get; set; }
        public bool Bookflag { get; set; }
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
            // Không tự động load dữ liệu khi mở form, chỉ tìm kiếm khi người dùng bấm nút Tìm kiếm
        }

        private void InitDefaults()
        {
            var types = AccountTypeMapping.GetAccountTypes();
            cboTransactionType.ItemsSource = types;
            cboTransactionType.EditValue = "I"; // Mặc định là Tiền mặt, không theo dõi

            DateTime today = DateTime.Today;
            // Mặc định lấy từ đầu năm đến hôm nay để không bỏ sót chứng từ
            dtFrom.DateTime = new DateTime(today.Year, 1, 1);
            dtTo.DateTime = today;
            cboStatus.SelectedIndex = 0; // Tất cả
            
            // Load lại state đã lưu (nếu có)
            UserPreferencesHelper.LoadState(this);

            if (cboTransactionType.EditValue == null || string.IsNullOrEmpty(cboTransactionType.EditValue.ToString()))
            {
                cboTransactionType.EditValue = "I";
            }

            // Đảm bảo dtTo không bị bé hơn dtFrom
            if (dtTo.DateTime < dtFrom.DateTime)
            {
                dtTo.DateTime = today;
            }
        }

        public override void LoadData()
        {
            DoSearch();
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
                // Bao gồm trọn vẹn cả ngày ToDate đến 23:59:59
                ToDate = dtTo.DateTime.Date.AddDays(1).AddSeconds(-1);
                StatusIndex = cboStatus.SelectedIndex;
                string transactionTypeFilter = cboTransactionType.EditValue?.ToString();
                if (string.IsNullOrEmpty(transactionTypeFilter))
                {
                    transactionTypeFilter = "I";
                    cboTransactionType.EditValue = "I";
                }
                
                // Lưu state
                UserPreferencesHelper.SaveState(this);
                
                using (var adapter = AppDataAccessAdapter.Create())
                {
                    var qf = new QueryFactory();

                    // Lấy danh mục tài khoản để map CategoryId
                    var coaList = adapter.FetchQuery(qf.ChartOfAccount).Cast<ChartOfAccountEntity>().ToList();
                    var coaCatMap = coaList.Where(c => !string.IsNullOrEmpty(c.CategoryId))
                                           .ToDictionary(c => c.AccountId?.Trim() ?? "", c => c.CategoryId);

                    var allTypes = AccountTypeMapping.GetAccountTypes();
                    var matchedType = allTypes.FirstOrDefault(t => t.Code == transactionTypeFilter);
                    string transTypeName = matchedType?.Name ?? "Khác";

                    // Lấy danh sách các tài khoản thuộc loại giao dịch đang chọn
                    var targetAccounts = coaList
                        .Where(c => GetCategoryForAccount(c.AccountId, coaCatMap) == transactionTypeFilter)
                        .Select(c => c.AccountId?.Trim())
                        .Where(a => !string.IsNullOrEmpty(a))
                        .Distinct()
                        .ToList();

                    if (targetAccounts.Count == 0)
                    {
                        gridControl.ItemsSource = null;
                        return;
                    }

                    var predicate = JournalVoucherFields.VoucherDate.GreaterEqual(FromDate)
                        .And(JournalVoucherFields.VoucherDate.LesserEqual(ToDate))
                        .And(ChartOfAccountFields.AccountId.In(targetAccounts));

                    if (StatusIndex == 1) // Chưa ghi sổ
                    {
                        predicate = predicate.And(JournalVoucherFields.Bookflag.Equal(false));
                    }
                    else if (StatusIndex == 2) // Đã ghi sổ
                    {
                        predicate = predicate.And(JournalVoucherFields.Bookflag.Equal(true));
                    }

                    // Dùng bảng FLAT (DiaryTypedList) lọc trực tiếp theo danh sách tài khoản từ Database
                    var q = qf.GetDiaryTypedList()
                        .Where(predicate)
                        .OrderBy(JournalVoucherFields.VoucherDate.Descending(), JournalVoucherFields.Id.Descending());

                    var flatRows = adapter.FetchQuery(q);

                    // Nhóm theo JournalVoucherId để lấy UNIQUE chứng từ
                    var grouped = flatRows
                        .Where(r => r.JournalVoucherId.HasValue && r.JournalVoucherId.Value > 0)
                        .GroupBy(r => r.JournalVoucherId.Value);

                    var listDto = new List<VoucherListDto>();

                    foreach (var g in grouped)
                    {
                        var first = g.First();

                        listDto.Add(new VoucherListDto
                        {
                            Id = g.Key,
                            VoucherNo = first.VoucherNo,
                            VoucherDate = first.VoucherDate ?? DateTime.Today,
                            Contents = first.Contents,
                            Bookflag = first.Bookflag,
                            TransactionType = transTypeName
                        });
                    }

                    // Sắp xếp chứng từ mới nhất lên đầu danh sách
                    var sortedDto = listDto.OrderByDescending(x => x.VoucherDate)
                                           .ThenByDescending(x => x.Id)
                                           .ToList();

                    gridControl.ItemsSource = null;
                    gridControl.ItemsSource = sortedDto;
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

        private static string GetCategoryForAccount(string accountId, Dictionary<string, string> coaCatMap)
        {
            if (string.IsNullOrEmpty(accountId)) return "";
            string trimmed = accountId.Trim();
            if (coaCatMap.TryGetValue(trimmed, out var catId) && !string.IsNullOrEmpty(catId))
                return catId;

            // Nhận diện theo đầu số chuẩn kế toán
            if (trimmed.StartsWith("15")) return "B"; // Kho / Vật tư
            if (trimmed.StartsWith("131") || trimmed.StartsWith("331") || trimmed.StartsWith("141")) return "A"; // Công nợ
            if (trimmed.StartsWith("6") || trimmed.StartsWith("8")) return "E"; // Chi phí
            if (trimmed.StartsWith("21")) return "D"; // Tài sản
            if (trimmed.StartsWith("11")) return "I"; // Tiền mặt / Ngân hàng
            if (trimmed.StartsWith("333")) return "V"; // Thuế VAT

            return "";
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e) => DoSearch();

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (!DbConfigService.CheckRecordLimit(out int count, out int maxLimit, out string warningMsg))
            {
                var result = ThemedMessageBox.Show(
                    "Hạn mức Chứng từ",
                    $"{warningMsg}\n\nBạn có muốn mở cửa sổ Đăng ký Bản quyền để gia tăng số lượng chứng từ không?",
                    MessageBoxButton.YesNo, MessageBoxImage.Information);
                if (result == MessageBoxResult.Yes)
                {
                    var regWin = new NewaccNet.Wpf.AppSystem.CompanyRegistrationWindow { Owner = this };
                    regWin.ShowDialog();
                }
                return;
            }

            var frm = new VoucherEntryWindow(null);
            frm.ShowDialog();
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

