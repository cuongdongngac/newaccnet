using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Grid.LookUp;
using DataAccess.EntityClasses;
using DataAccess.HelperClasses;
using SD.LLBLGen.Pro.ORMSupportClasses;
using System.Collections.Generic;
using NewaccNet.Wpf.AppSystem;
using System.ComponentModel;
using System.Windows.Media;

namespace NewaccNet.Wpf.AppSystem.Voucher
{
    public class DbcrItem
    {
        public short Id { get; set; }
        public string Name { get; set; }
    }

    public partial class VoucherEntryWindow : DevExpress.Xpf.Core.ThemedWindow
    {
        private ObservableCollection<JournalEntryEntity> _masterEntries;
        private JournalVoucherEntity _currentVoucher;
        private bool _isDirty = false;
        public bool HasSaved { get; private set; } = false;

        public ObservableCollection<ChartOfAccountEntity> Accounts { get; set; }
        public List<DbcrItem> DbcrList { get; set; }

        public VoucherEntryWindow(int? voucherId = null)
        {
            InitializeComponent();
            
            _masterEntries = new ObservableCollection<JournalEntryEntity>();
            Accounts = new ObservableCollection<ChartOfAccountEntity>();
            
            DbcrList = new List<DbcrItem>
            {
                new DbcrItem { Id = 1, Name = "Nợ" },
                new DbcrItem { Id = -1, Name = "Có" }
            };

                        gridLeft.ItemsSource = _masterEntries;

            // Khóa UI Lưới Trái: Chỉ cho phép nhập 1 dòng thủ công.
            _masterEntries.CollectionChanged += (s, e) => {
                var view = gridLeft.View as DevExpress.Xpf.Grid.TableView;
                if (view != null)
                {
                    // Delay việc ẩn dòng NewItemRow lại một nhịp (Asynchronous)
                    // Vì khi sự kiện này nổ ra, lưới vẫn đang trong quá trình chuyển giao dòng (từ NewItemRow sang Dòng thật).
                    // Nếu ẩn ngay lập tức, UI element bị phá hủy đột ngột làm dòng bị tàng hình.
                    Application.Current.Dispatcher.BeginInvoke(new Action(() => {
                        view.NewItemRowPosition = _masterEntries.Count >= 1 
                            ? DevExpress.Xpf.Grid.NewItemRowPosition.None 
                            : DevExpress.Xpf.Grid.NewItemRowPosition.Bottom;
                    }), System.Windows.Threading.DispatcherPriority.Loaded);
                }
            };
            this.DataContext = this;

            LoadDictionaries();

            if (voucherId.HasValue)
            {
                LoadExistingVoucher(voucherId.Value);
            }
            else
            {
                BtnNew_Click(null, null);
            }
            
            txtContents.EditValueChanged += (s,e) => _isDirty = true;
            
            var viewRight = gridRight.View as DevExpress.Xpf.Grid.TableView;
            if (viewRight != null)
            {
                viewRight.InitNewRow += GridRight_InitNewRow;
            }
            
            var viewLeft = gridLeft.View as DevExpress.Xpf.Grid.TableView;
            if (viewLeft != null)
            {
                viewLeft.InitNewRow += GridLeft_InitNewRow;
            }
        }

                private void TableViewLeft_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            // Bắt sự kiện Tab hoặc Enter ở lưới Trái
            if (e.Key == System.Windows.Input.Key.Tab || e.Key == System.Windows.Input.Key.Enter)
            {
                var view = sender as DevExpress.Xpf.Grid.TableView;
                // Nếu đang đứng ở cột cuối cùng (Số tiền - Amount)
                if (view != null && view.Grid.CurrentColumn != null && view.Grid.CurrentColumn.FieldName == "Amount")
                {
                    // Ép Grid kết thúc việc nhập liệu hiện tại
                    view.CommitEditing();
                    view.CloseEditor();

                    // Chuyển Focus sang lưới Phải
                    gridRight.Focus();
                    var rightView = gridRight.View as DevExpress.Xpf.Grid.TableView;
                    if (rightView != null)
                    {
                        // Chuyển thẳng xuống dòng New Item Row của lưới Phải
                        rightView.FocusedRowHandle = DevExpress.Xpf.Grid.GridControl.NewItemRowHandle;
                        // Đưa con trỏ vào ô AccountId luôn (bỏ qua Dbcr vì đã được Auto)
                        var accountCol = gridRight.Columns["AccountId"];
                        if (accountCol != null)
                        {
                            rightView.FocusedColumn = accountCol;
                        }
                        rightView.ShowEditor();
                    }
                    e.Handled = true;
                }
            }
        }

        private void gridLeft_SelectedItemChanged(object sender, SelectedItemChangedEventArgs e)
        {
            var selectedMaster = gridLeft.SelectedItem as JournalEntryEntity;
            if (selectedMaster != null)
            {
                // LLBLGen magic: bind directly to the SubEntries collection of the selected master!
                gridRight.ItemsSource = selectedMaster.SubEntries;
            }
            else
            {
                gridRight.ItemsSource = null;
            }
        }

                private void GridRight_InitNewRow(object sender, InitNewRowEventArgs e)
        {
            var newEntry = gridRight.GetRow(e.RowHandle) as JournalEntryEntity;
            var parent = gridLeft.SelectedItem as JournalEntryEntity;
            
            if (newEntry != null && parent != null)
            {
                newEntry.ParentEntry = parent;
                if (parent.Id != 0)
                {
                    newEntry.ParentId = parent.Id;
                }
                
                // Tự động đảo chiều Nợ/Có so với Master
                if (parent.Dbcr.HasValue)
                {
                    newEntry.Dbcr = (short)-parent.Dbcr.Value;
                }
                else 
                {
                    newEntry.Dbcr = -1; // Default
                }

                // Tự động bù trừ số tiền (Amount)
                double parentAmount = parent.Amount ?? 0;
                double sumExisting = 0;
                if (parent.SubEntries != null)
                {
                    foreach (var child in parent.SubEntries)
                    {
                        if (child != newEntry) // Bỏ qua chính nó vừa tạo
                        {
                            sumExisting += (child.Amount ?? 0);
                        }
                    }
                }
                newEntry.Amount = parentAmount - sumExisting;

                _isDirty = true;
                CalculateBalance();
            }
        }
        
        private void GridLeft_InitNewRow(object sender, InitNewRowEventArgs e)
        {
            var newEntry = gridLeft.GetRow(e.RowHandle) as JournalEntryEntity;
            if (newEntry != null)
            {
                _isDirty = true;
            }
        }
        
        private void GridView_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            _isDirty = true;
            CalculateBalance();
        }

        public void CalculateBalance()
        {
            if (_masterEntries == null) return;
            
            double total = 0;
            // Tính tổng tất cả Master
            foreach (var master in _masterEntries)
            {
                total += (master.Amount ?? 0) * (master.Dbcr ?? 0);
                
                // Tính tổng tất cả Detail của Master đó
                if (master.SubEntries != null)
                {
                    foreach (var detail in master.SubEntries)
                    {
                        total += (detail.Amount ?? 0) * (detail.Dbcr ?? 0);
                    }
                }
            }
            
            txtBalance.Text = total.ToString("N0");
            if (total == 0)
                txtBalance.Foreground = new SolidColorBrush(Colors.Green);
            else
                txtBalance.Foreground = new SolidColorBrush(Colors.Red);
        }

                /// <summary>
        /// Quét lại toàn bộ cây dữ liệu (Object Graph) trong bộ nhớ và vẽ lại 2 lưới.
        /// Hàm này rất hữu ích khi các Form phụ (Voucher Generator) thao tác sinh/đảo chiều dữ liệu ngầm.
        /// </summary>
        public void RefreshVoucherUI()
        {
            if (_currentVoucher == null) return;
            
            // Ép UI chốt dữ liệu đang gõ dở
            gridLeft.View.CommitEditing();
            gridRight.View.CommitEditing();

            var currentSelected = gridLeft.SelectedItem as JournalEntryEntity;

            _masterEntries.Clear();
            gridRight.ItemsSource = null;
            
            // Quét lại toàn bộ JournalEntries phẳng
            foreach (var entry in _currentVoucher.JournalEntries)
            {
                // Dòng là Master nếu không trỏ ID cha và cũng không trỏ Object cha
                bool isMaster = (entry.ParentId == null || entry.ParentId == 0) && entry.ParentEntry == null;
                
                if (isMaster)
                {
                    _masterEntries.Add(entry);
                    
                    // Tìm các dòng con ruột (Link theo Object hoặc Link theo ID)
                    var children = _currentVoucher.JournalEntries
                        .Where(x => x.ParentEntry == entry || (x.ParentId != null && x.ParentId != 0 && x.ParentId == entry.Id))
                        .ToList();
                        
                    foreach(var child in children)
                    {
                        // Đảm bảo con nằm trong SubEntries của cha
                        if (!entry.SubEntries.Contains(child))
                        {
                            entry.SubEntries.Add(child);
                            child.ParentEntry = entry; // Fix ngược lại nếu thiếu
                        }
                    }
                }
            }
            
            // Phục hồi lại dòng đang chọn (nếu có)
            if (currentSelected != null && _masterEntries.Contains(currentSelected))
            {
                gridLeft.SelectedItem = currentSelected;
            }
            else if (_masterEntries.Count > 0)
            {
                gridLeft.SelectedItem = _masterEntries[0];
            }
            
            CalculateBalance();
        }

        private void LoadExistingVoucher(int voucherId)
        {
            using (var adapter = AppDataAccessAdapter.Create())
            {
                var prefetchPath = new PrefetchPath2((int)DataAccess.EntityType.JournalVoucherEntity);
                var jeNode = prefetchPath.Add(JournalVoucherEntity.PrefetchPathJournalEntries);
                
                // Nạp chi tiết Công nợ (DebtDetails) cho từng dòng JournalEntry
                var debtNode = jeNode.SubPath.Add(JournalEntryEntity.PrefetchPathDebtDetails);
                
                // Nạp chi tiết Ngoại tệ (CurrencyLiabilityLine) cho từng dòng DebtDetail
                debtNode.SubPath.Add(DebtDetailEntity.PrefetchPathCurrencyLiabilityLine);

                // Nạp thêm chi tiết vật tư (nếu có sau này)
                // jeNode.SubPath.Add(JournalEntryEntity.PrefetchPathInventoryDetails);
                
                var freshVoucher = new JournalVoucherEntity(voucherId);
                if (adapter.FetchEntity(freshVoucher, prefetchPath))
                {
                    _currentVoucher = freshVoucher;
                    BindHeader();
                    
                    _masterEntries.Clear();
                    
                    // Chỉ nạp các dòng cha (Master) vào lưới Trái
                                        foreach (var entry in _currentVoucher.JournalEntries)
                    {
                        if (entry.ParentId == null || entry.ParentId == 0)
                        {
                            _masterEntries.Add(entry);
                            
                            // Thủ thuật: Tự link SubEntries từ danh sách phẳng (tránh bị duplicate object của ORM)
                            var children = _currentVoucher.JournalEntries.Where(x => x.ParentId == entry.Id).ToList();
                            foreach(var child in children)
                            {
                                if (!entry.SubEntries.Contains(child))
                                    entry.SubEntries.Add(child);
                            }
                        }
                    }
                    
                    if (_masterEntries.Count > 0)
                        gridLeft.SelectedItem = _masterEntries[0];
                        
                    _isDirty = false;
                    CalculateBalance();
                }
            }
        }

        private void LoadDictionaries()
        {
            using (var adapter = AppDataAccessAdapter.Create())
            {
                var accounts = new EntityCollection<ChartOfAccountEntity>();
                adapter.FetchEntityCollection(accounts, null);
                
                var parentIds = accounts.Select(a => a.ParentId).Where(id => !string.IsNullOrEmpty(id)).Distinct().ToList();
                var detailAccounts = accounts.Where(a => !parentIds.Contains(a.AccountId)).OrderBy(a => a.AccountId);
                
                foreach (var acc in detailAccounts)
                {
                    Accounts.Add(acc);
                }
            }
        }

        private void BtnNew_Click(object sender, RoutedEventArgs e)
        {
            if (_isDirty && !ConfirmDiscardChanges()) return;

            _currentVoucher = new JournalVoucherEntity();
            _currentVoucher.VoucherDate = DateTime.Today;
            _currentVoucher.VoucherNo = "N" + DateTime.Now.ToString("yyMMddHH");
            
            _masterEntries.Clear();
            gridRight.ItemsSource = null;

            BindHeader();
            CalculateBalance();
            _isDirty = false;
        }

        private void BindHeader()
        {
            txtVoucherId.EditValue = _currentVoucher.Id;
            txtVoucherNo.EditValue = _currentVoucher.VoucherNo;
            txtVoucherDate.EditValue = _currentVoucher.VoucherDate;
            txtContents.EditValue = _currentVoucher.Contents;
            chkBookflag.EditValue = _currentVoucher.Bookflag;
        }

        private void UnbindHeader()
        {
            _currentVoucher.VoucherNo = txtVoucherNo.Text;
            _currentVoucher.VoucherDate = txtVoucherDate.DateTime;
            _currentVoucher.Contents = txtContents.Text;
            _currentVoucher.Bookflag = chkBookflag.IsChecked ?? false;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Force update UI bindings
                gridLeft.View.CommitEditing();
                gridRight.View.CommitEditing();
                
                CalculateBalance();
                if (txtBalance.Text != "0")
                {
                    MessageBox.Show("Chứng từ chưa cân đối (Tổng Nợ khác Tổng Có)!\nBạn phải cân đối chứng từ trước khi lưu.", 
                        "Lỗi cân đối", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                
                UnbindHeader();

                _currentVoucher.JournalEntries.Clear();
                foreach (var master in _masterEntries)
                {
                    _currentVoucher.JournalEntries.Add(master);
                    // Add details so adapter knows they belong to the voucher
                    foreach (var detail in master.SubEntries)
                    {
                        _currentVoucher.JournalEntries.Add(detail);
                    }
                }

                using (var adapter = AppDataAccessAdapter.Create())
                {
                    adapter.SaveEntity(_currentVoucher, true, recurse: true);
                }

                _isDirty = false;
                LoadExistingVoucher(_currentVoucher.Id);

                MessageBox.Show("Lưu thành công! Các cột ID và ParentId đã được DB cấp tự động.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lưu: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool ConfirmDiscardChanges()
        {
            var result = MessageBox.Show("Chứng từ đã bị thay đổi nhưng chưa lưu. Bạn có chắc chắn muốn bỏ qua thay đổi?", 
                                         "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            return result == MessageBoxResult.Yes;
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ThemedWindow_Closing(object sender, CancelEventArgs e)
        {
            if (_isDirty)
            {
                if (!ConfirmDiscardChanges())
                {
                    e.Cancel = true;
                }
            }
        }

        private void BtnFirst_Click(object sender, RoutedEventArgs e) { MessageBox.Show("Navigate Đầu tiên"); }
        private void BtnPrev_Click(object sender, RoutedEventArgs e) { MessageBox.Show("Navigate Trước"); }
        private void BtnNext_Click(object sender, RoutedEventArgs e) { MessageBox.Show("Navigate Tiếp"); }
        private void BtnLast_Click(object sender, RoutedEventArgs e) { MessageBox.Show("Navigate Cuối cùng"); }
    }
}
