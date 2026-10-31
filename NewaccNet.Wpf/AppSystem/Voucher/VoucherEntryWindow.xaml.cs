using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Grid.LookUp;
using DataAccess.EntityClasses;
using DataAccess.HelperClasses;
using SD.LLBLGen.Pro.ORMSupportClasses;
using System.Collections.Generic;
using NewaccNet.Wpf.AppSystem;
using System.ComponentModel;
using System.Windows.Media;
using NewaccNet.Wpf.AppSystem.Voucher.Reports;
using LicenseManager = NewaccNet.Core.License.LicenseManager;
using NewaccNet.Core.Posting;

namespace NewaccNet.Wpf.AppSystem.Voucher
{
    public enum VoucherPrintType
    {
        [Description("Phiếu thông thường")]
        Normal = 0,
        [Description("Phiếu thu")]
        CashReceipt = 1,
        [Description("Phiếu chi")]
        CashPayment = 2,
        [Description("Ủy nhiệm chi")]
        BankPaymentOrder = 3,
        [Description("Giấy nộp tiền")]
        BankDeposit = 4
    }

    public class VoucherPrintTypeItem
    {
        public VoucherPrintType Type { get; set; }
        public string Name { get; set; } = "";
    }

    public class DbcrItem
    {
        public short Id { get; set; }
        public string Name { get; set; }
    }

    public partial class VoucherEntryWindow : DevExpress.Xpf.Core.ThemedWindow
    {
        private ObservableCollection<JournalVoucherEntity> _voucherBatch;
        public ObservableCollection<JournalVoucherEntity> VoucherBatch => _voucherBatch;
        private int _currentVoucherIndex = -1;
        public int CurrentVoucherIndex => _currentVoucherIndex;

        private ObservableCollection<JournalEntryEntity> _masterEntries;
        public ObservableCollection<JournalEntryEntity> MasterEntries => _masterEntries;
        private JournalVoucherEntity _currentVoucher;
        public JournalVoucherEntity CurrentVoucher => _currentVoucher;
        private bool _isDirty = false;
        public bool HasSaved { get; private set; } = false;
        private List<JournalVoucherEntity> _deletedVouchersInBatch = new List<JournalVoucherEntity>();

        private readonly IVoucherPostingService _postingService = new VoucherPostingService();
        private readonly IVoucherNumberService _numberService = new VoucherNumberService();

        public ObservableCollection<ChartOfAccountEntity> Accounts { get; set; }
        public List<DbcrItem> DbcrList { get; set; }

        public List<VoucherPrintTypeItem> PrintTypeList { get; } = new List<VoucherPrintTypeItem>
        {
            new VoucherPrintTypeItem { Type = VoucherPrintType.Normal, Name = "Phiếu thông thường" },
            new VoucherPrintTypeItem { Type = VoucherPrintType.CashReceipt, Name = "Phiếu thu" },
            new VoucherPrintTypeItem { Type = VoucherPrintType.CashPayment, Name = "Phiếu chi" },
            new VoucherPrintTypeItem { Type = VoucherPrintType.BankPaymentOrder, Name = "Ủy nhiệm chi" },
            new VoucherPrintTypeItem { Type = VoucherPrintType.BankDeposit, Name = "Giấy nộp tiền" }
        };

        public VoucherEntryWindow(int? voucherId = null)
        {
            InitializeComponent();
            cboPrintType.EditValue = VoucherPrintType.Normal;
            
            _voucherBatch = new ObservableCollection<JournalVoucherEntity>();
            _masterEntries = new ObservableCollection<JournalEntryEntity>();
            Accounts = new ObservableCollection<ChartOfAccountEntity>();
            
            DbcrList = new List<DbcrItem>
            {
                new DbcrItem { Id = 1, Name = "Nợ" },
                new DbcrItem { Id = -1, Name = "Có" }
            };

            gridLeft.ItemsSource = _masterEntries;

            _masterEntries.CollectionChanged += (s, e) => {
                if (_masterEntries.Count == 0)
                {
                    Application.Current.Dispatcher.BeginInvoke(new Action(() => {
                        if (_masterEntries.Count == 0)
                        {
                            var emptyMaster = new JournalEntryEntity();
                            _masterEntries.Add(emptyMaster);
                            gridLeft.SelectedItem = emptyMaster;
                        }
                    }));
                }
            };
            this.DataContext = this;

            LoadDictionaries();

            if (voucherId.HasValue)
            {
                var fetched = FetchVoucherById(voucherId.Value);
                if (fetched != null)
                {
                    _voucherBatch.Add(fetched);
                    _currentVoucherIndex = 0;
                    SetCurrentVoucher(fetched);
                }
                else
                {
                    if (EnsureCanAddNewVoucher())
                    {
                        BtnNew_Click(null, null);
                    }
                    else
                    {
                        this.Loaded += (s, e) => this.Close();
                    }
                }
            }
            else
            {
                if (EnsureCanAddNewVoucher())
                {
                    BtnNew_Click(null, null);
                }
                else
                {
                    this.Loaded += (s, e) => this.Close();
                }
            }
            
            AttachHeaderDirtyEvents();
            
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

        private void AttachHeaderDirtyEvents()
        {
            txtVoucherNo.EditValueChanged += (s, e) => _isDirty = true;
            txtVoucherDate.EditValueChanged += (s, e) => _isDirty = true;
            txtContents.EditValueChanged += (s, e) => _isDirty = true;
            chkBookflag.EditValueChanged += (s, e) => _isDirty = true;

            txtPersonname.EditValueChanged += (s, e) => _isDirty = true;
            txtPersonaddress.EditValueChanged += (s, e) => _isDirty = true;
            txtInvoicesnumber.EditValueChanged += (s, e) => _isDirty = true;
            txtPaymentacc.EditValueChanged += (s, e) => _isDirty = true;
            txtPaymentbank.EditValueChanged += (s, e) => _isDirty = true;
            txtRecievedcorp.EditValueChanged += (s, e) => _isDirty = true;
            txtRecievedacc.EditValueChanged += (s, e) => _isDirty = true;
            txtRecievedbank.EditValueChanged += (s, e) => _isDirty = true;
        }

        private void TableView_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            // Phím tắt F4: Mở nhanh bảng chi tiết phụ (Vật tư, Công nợ, Chi phí) cho dòng đang chọn
            if (e.Key == System.Windows.Input.Key.F4)
            {
                var view = sender as DevExpress.Xpf.Grid.TableView;
                var entry = view?.Grid?.SelectedItem as JournalEntryEntity;
                if (entry != null)
                {
                    OpenDetailFormIfRequired(entry, autoPrompt: false);
                    e.Handled = true;
                }
            }
        }

        private void TableView_RowDoubleClick(object sender, DevExpress.Xpf.Grid.RowDoubleClickEventArgs e)
        {
            var view = sender as DevExpress.Xpf.Grid.TableView;
            var entry = view?.Grid?.SelectedItem as JournalEntryEntity;
            if (entry != null)
            {
                OpenDetailFormIfRequired(entry, autoPrompt: false);
            }
        }

        private void gridLeft_SelectedItemChanged(object sender, SelectedItemChangedEventArgs e)
        {
            var selectedMaster = (gridLeft.SelectedItem ?? gridLeft.CurrentItem ?? _masterEntries.FirstOrDefault()) as JournalEntryEntity;
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

        private void gridLeft_CurrentItemChanged(object sender, CurrentItemChangedEventArgs e)
        {
            var selectedMaster = (gridLeft.CurrentItem ?? gridLeft.SelectedItem ?? _masterEntries.FirstOrDefault()) as JournalEntryEntity;
            if (selectedMaster != null)
            {
                gridRight.ItemsSource = selectedMaster.SubEntries;
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
                    newEntry.Dbcr = null;
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

        private void TableView_ShowingEditor(object sender, ShowingEditorEventArgs e)
        {
            if (e.Column != null && e.Column.FieldName == "Amount")
            {
                var view = sender as TableView;
                var row = e.Row as JournalEntryEntity;
                if (row == null && view != null && view.Grid != null)
                {
                    row = view.Grid.GetRow(e.RowHandle) as JournalEntryEntity;
                }

                if (row != null)
                {
                    // Chỉ đặt ReadOnly khi tài khoản/dòng này CÓ CHI TIẾT (Vật tư, Công nợ, Chi phí, Tài sản, Cổ phiếu, Ngoại tệ...)
                    // Các tài khoản không có chi tiết (tiền mặt, tài khoản thường) thì người dùng vẫn gõ tiền bình thường.
                    if (row.IsMaterialBound || row.HasSubLedgerDetails())
                    {
                        e.Cancel = true; // Khóa sửa tay: Số tiền do bảng chi tiết quản lý và tự động đổ về từ ORM
                        return;
                    }
                }
            }

            if (e.Column != null && e.Column.FieldName == "AccountId")
            {
                var view = sender as TableView;
                if (view == null) return;

                if (sender == gridRight.View)
                {
                    // Lưới Phải (Detail): Kiểm tra xem dòng Master bên trái đã chọn Nợ/Có chưa
                    var parent = (gridLeft.SelectedItem ?? gridLeft.CurrentItem ?? _masterEntries.FirstOrDefault()) as JournalEntryEntity;
                    if (parent == null || !parent.Dbcr.HasValue || parent.Dbcr.Value == 0)
                    {
                        e.Cancel = true;
                        gridLeft.Focus();
                        if (gridLeft.View is TableView leftView && leftView.Grid != null && leftView.Grid.Columns["Dbcr"] != null)
                        {
                            Dispatcher.BeginInvoke(new Action(() =>
                            {
                                leftView.Grid.CurrentColumn = leftView.Grid.Columns["Dbcr"];
                                leftView.ShowEditor();
                            }));
                        }
                        MessageBox.Show("Vui lòng chọn Nợ hoặc Có ở dòng chính (Master - Lưới Trái) trước.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }

                    // Tự động gán Dbcr đảo chiều cho dòng con ở lưới phải nếu chưa có
                    var subRow = e.Row as JournalEntryEntity;
                    if (subRow == null && view.Grid != null)
                    {
                        subRow = view.Grid.GetRow(e.RowHandle) as JournalEntryEntity;
                    }
                    if (subRow != null)
                    {
                        if (!subRow.Dbcr.HasValue || subRow.Dbcr == 0)
                        {
                            subRow.Dbcr = (short)-parent.Dbcr.Value;
                        }
                        if (subRow.ParentEntry == null)
                        {
                            subRow.ParentEntry = parent;
                        }
                        if (parent.Id != 0 && (!subRow.ParentId.HasValue || subRow.ParentId == 0))
                        {
                            subRow.ParentId = parent.Id;
                        }
                    }
                }
                else
                {
                    // Lưới Trái (Master): Kiểm tra chính dòng Master
                    short? dbcr = null;
                    if (e.Row is JournalEntryEntity entry)
                    {
                        dbcr = entry.Dbcr;
                    }
                    else
                    {
                        var cellVal = view.Grid.GetCellValue(e.RowHandle, "Dbcr");
                        if (cellVal != null && short.TryParse(cellVal.ToString(), out short val))
                        {
                            dbcr = val;
                        }
                    }

                    if (!dbcr.HasValue || dbcr.Value == 0)
                    {
                        e.Cancel = true;
                        if (view.Grid != null && view.Grid.Columns["Dbcr"] != null)
                        {
                            Dispatcher.BeginInvoke(new Action(() =>
                            {
                                view.Grid.CurrentColumn = view.Grid.Columns["Dbcr"];
                                view.ShowEditor();
                            }));
                        }
                        MessageBox.Show("Vui lòng chọn Nợ hoặc Có trước khi chọn tài khoản.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
        }
        
        private void GridView_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            _isDirty = true;
            if (e.Column.FieldName == "Dbcr" && sender == gridLeft.View)
            {
                var masterRow = e.Row as JournalEntryEntity;
                if (masterRow != null && masterRow.Dbcr.HasValue && masterRow.SubEntries != null)
                {
                    foreach (var sub in masterRow.SubEntries)
                    {
                        sub.Dbcr = (short)-masterRow.Dbcr.Value;
                    }
                    gridRight.RefreshData();
                }
            }
            if (sender == gridRight.View)
            {
                var subRow = e.Row as JournalEntryEntity;
                var parent = (gridLeft.SelectedItem ?? gridLeft.CurrentItem ?? _masterEntries.FirstOrDefault()) as JournalEntryEntity;
                if (subRow != null && parent != null && parent.Dbcr.HasValue)
                {
                    if (!subRow.Dbcr.HasValue || subRow.Dbcr == 0)
                    {
                        subRow.Dbcr = (short)-parent.Dbcr.Value;
                    }
                    if (subRow.ParentEntry == null)
                    {
                        subRow.ParentEntry = parent;
                    }
                    if (parent.Id != 0 && (!subRow.ParentId.HasValue || subRow.ParentId == 0))
                    {
                        subRow.ParentId = parent.Id;
                    }
                }
            }

            CalculateBalance();
        }

        private void TableView_RowDeleted(object sender, Views.Base.RowDeletedEventArgs e)
        {
            _isDirty = true;
            if (e.RowItem is JournalEntryEntity entry)
            {
                if (sender == gridLeft.View && entry.SubEntries != null)
                {
                    foreach (var sub in entry.SubEntries.ToList())
                    {
                        _currentVoucher?.JournalEntries.Remove(sub);
                    }
                }
                _currentVoucher?.JournalEntries.Remove(entry);
            }
            CalculateBalance();
        }

        public void OpenDetailFormIfRequired(JournalEntryEntity entry, bool autoPrompt)
        {
            if (AccountLookUpEdit.OpenDetailWindow(entry, this, false, !autoPrompt))
            {
                _isDirty = true;
            }
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
            
            total = Math.Round(total, 0);
            txtBalance.Text = Math.Abs(total).ToString("N0");
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
            
            // Đóng editor đang mở dở trên lưới thay vì ép commit đè lên dữ liệu mới
            (gridLeft.View as TableView)?.HideEditor();
            (gridRight.View as TableView)?.HideEditor();

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
                gridLeft.CurrentItem = currentSelected;
                gridRight.ItemsSource = currentSelected.SubEntries;
            }
            else if (_masterEntries.Count > 0)
            {
                gridLeft.SelectedItem = _masterEntries[0];
                gridLeft.CurrentItem = _masterEntries[0];
                gridRight.ItemsSource = _masterEntries[0].SubEntries;
            }
            else
            {
                var emptyMaster = new JournalEntryEntity();
                _masterEntries.Add(emptyMaster);
                gridLeft.SelectedItem = emptyMaster;
                gridLeft.CurrentItem = emptyMaster;
                gridRight.ItemsSource = emptyMaster.SubEntries;
            }
            
            CalculateBalance();
        }

        private JournalVoucherEntity FetchVoucherById(int voucherId)
        {
            using (var adapter = AppDataAccessAdapter.Create())
            {
                var prefetchPath = new PrefetchPath2((int)DataAccess.EntityType.JournalVoucherEntity);
                var jeNode = prefetchPath.Add(JournalVoucherEntity.PrefetchPathJournalEntries);
                
                // Nạp chi tiết Công nợ (DebtDetails) cho từng dòng JournalEntry
                var debtNode = jeNode.SubPath.Add(JournalEntryEntity.PrefetchPathDebtDetails);
                
                // Nạp chi tiết Ngoại tệ (CurrencyLiabilityLine) cho từng dòng DebtDetail
                debtNode.SubPath.Add(DebtDetailEntity.PrefetchPathCurrencyLiabilityLine);
                
                // Nạp LiabilityDue (Cam kết lãi suất) cho từng dòng DebtDetail (để có LiabilityDuesId)
                debtNode.SubPath.Add(DebtDetailEntity.PrefetchPathLiabilityDue);

                jeNode.SubPath.Add(JournalEntryEntity.PrefetchPathCurrencyDetails);
                jeNode.SubPath.Add(JournalEntryEntity.PrefetchPathInvestmentDetails);
                jeNode.SubPath.Add(JournalEntryEntity.PrefetchPathAssetDetails);
                var costNode = jeNode.SubPath.Add(JournalEntryEntity.PrefetchPathCostDetails);
                costNode.SubPath.Add(CostDetailEntity.PrefetchPathExpenseTaxLine);

                var ivNode = jeNode.SubPath.Add(JournalEntryEntity.PrefetchPathInventoryVouchers);
                var ivlNode = ivNode.SubPath.Add(InventoryVoucherEntity.PrefetchPathInventoryVoucherLines);
                ivlNode.SubPath.Add(InventoryVoucherLineEntity.PrefetchPathMaterialTaxLine);
                
                var freshVoucher = new JournalVoucherEntity(voucherId);
                if (adapter.FetchEntity(freshVoucher, prefetchPath))
                {
                    return freshVoucher;
                }
            }
            return null;
        }

        private void SetCurrentVoucher(JournalVoucherEntity voucher)
        {
            if (voucher == null) return;
            _currentVoucher = voucher;

            BindHeader();
            
            gridLeft.View.CommitEditing();
            gridRight.View.CommitEditing();

            _masterEntries.Clear();
            
            foreach (var entry in _currentVoucher.JournalEntries)
            {
                bool isMaster = (entry.ParentId == null || entry.ParentId == 0) && entry.ParentEntry == null;
                if (isMaster)
                {
                    _masterEntries.Add(entry);
                    
                    var children = _currentVoucher.JournalEntries
                        .Where(x => x.ParentEntry == entry || (x.ParentId != null && x.ParentId != 0 && x.ParentId == entry.Id))
                        .ToList();
                        
                    foreach(var child in children)
                    {
                        if (!entry.SubEntries.Contains(child))
                        {
                            entry.SubEntries.Add(child);
                            child.ParentEntry = entry;
                        }
                    }
                }
            }
            
            if (_masterEntries.Count > 0)
            {
                gridLeft.SelectedItem = _masterEntries[0];
                gridLeft.CurrentItem = _masterEntries[0];
                gridRight.ItemsSource = _masterEntries[0].SubEntries;
            }
            else
            {
                var emptyMaster = new JournalEntryEntity();
                _masterEntries.Add(emptyMaster);
                gridLeft.SelectedItem = emptyMaster;
                gridLeft.CurrentItem = emptyMaster;
                gridRight.ItemsSource = emptyMaster.SubEntries;
            }
            
            CalculateBalance();
            if (cboPrintType != null)
            {
                cboPrintType.EditValue = VoucherPrintType.Normal;
            }
            UpdateNavigationUI();
        }

        private void UpdateNavigationUI()
        {
            int count = _voucherBatch.Count;
            int current = _currentVoucherIndex >= 0 ? _currentVoucherIndex + 1 : 0;
            if (lblNavPosition != null)
            {
                lblNavPosition.Content = $"{current} / {count}";
            }

            if (btnFirst != null) btnFirst.IsEnabled = _currentVoucherIndex > 0;
            if (btnPrev != null) btnPrev.IsEnabled = _currentVoucherIndex > 0;
            if (btnNext != null) btnNext.IsEnabled = _currentVoucherIndex < count - 1;
            if (btnLast != null) btnLast.IsEnabled = _currentVoucherIndex < count - 1;
        }

        private void CommitCurrentVoucherToBatch()
        {
            if (_currentVoucher == null) return;

            gridLeft.View.CommitEditing();
            gridRight.View.CommitEditing();
            UnbindHeader();

            _currentVoucher.JournalEntries.Clear();
            foreach (var master in _masterEntries)
            {
                if (string.IsNullOrEmpty(master.AccountId) && (master.Amount == null || master.Amount == 0))
                    continue;

                _currentVoucher.JournalEntries.Add(master);
                foreach (var detail in master.SubEntries)
                {
                    if (string.IsNullOrEmpty(detail.AccountId) && (detail.Amount == null || detail.Amount == 0))
                        continue;

                    detail.ParentEntry = master;
                    if (master.Id != 0 && (!detail.ParentId.HasValue || detail.ParentId == 0))
                    {
                        detail.ParentId = master.Id;
                    }
                    if (master.Dbcr.HasValue && (!detail.Dbcr.HasValue || detail.Dbcr == 0))
                    {
                        detail.Dbcr = (short)-master.Dbcr.Value;
                    }

                    _currentVoucher.JournalEntries.Add(detail);
                }
            }
        }

        private void NavigateTo(int newIndex)
        {
            if (newIndex < 0 || newIndex >= _voucherBatch.Count || newIndex == _currentVoucherIndex)
                return;

            CommitCurrentVoucherToBatch();
            _currentVoucherIndex = newIndex;
            SetCurrentVoucher(_voucherBatch[_currentVoucherIndex]);
        }

        private void BtnFirst_Click(object sender, RoutedEventArgs e) => NavigateTo(0);
        private void BtnPrev_Click(object sender, RoutedEventArgs e) => NavigateTo(_currentVoucherIndex - 1);
        private void BtnNext_Click(object sender, RoutedEventArgs e) => NavigateTo(_currentVoucherIndex + 1);
        private void BtnLast_Click(object sender, RoutedEventArgs e) => NavigateTo(_voucherBatch.Count - 1);

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

        private string GenerateNextVoucherNo()
        {
            return _numberService.GenerateNextInBatch(
                _voucherBatch.Select(v => v.VoucherNo),
                "CT",
                _voucherBatch.Count);
        }

        private JournalVoucherEntity CreateNewVoucher()
        {
            var newVoucher = new JournalVoucherEntity();
            newVoucher.VoucherDate = _currentVoucher?.VoucherDate ?? DateTime.Today;
            newVoucher.VoucherNo = GenerateNextVoucherNo();
            newVoucher.Bookflag = true;

            var firstMaster = new JournalEntryEntity();
            newVoucher.JournalEntries.Add(firstMaster);
            return newVoucher;
        }

        private bool EnsureCanAddNewVoucher()
        {
            if (!DbConfigService.CheckRecordLimit(out int count, out int maxLimit, out string warningMsg))
            {
                var result = ThemedMessageBox.Show(
                    "Hạn mức Chứng từ",
                    $"{warningMsg}\n\nBạn có muốn mở cửa sổ Đăng ký Bản quyền để gia tăng số lượng chứng từ không?",
                    MessageBoxButton.YesNo, MessageBoxImage.Information);
                if (result == MessageBoxResult.Yes)
                {
                    var regWin = new NewaccNet.Wpf.AppSystem.CompanyRegistrationWindow { Owner = this.Owner ?? this };
                    regWin.ShowDialog();
                }
                return false;
            }
            return true;
        }

        private void BtnNew_Click(object sender, RoutedEventArgs e)
        {
            if (!EnsureCanAddNewVoucher())
            {
                return;
            }

            CommitCurrentVoucherToBatch();

            var newVoucher = CreateNewVoucher();

            _voucherBatch.Add(newVoucher);
            _currentVoucherIndex = _voucherBatch.Count - 1;
            SetCurrentVoucher(newVoucher);
            _isDirty = true;
        }

        private void BindHeader()
        {
            txtVoucherId.EditValue = _currentVoucher.Id;
            txtVoucherNo.EditValue = _currentVoucher.VoucherNo;
            txtVoucherDate.EditValue = _currentVoucher.VoucherDate;
            txtContents.EditValue = _currentVoucher.Contents;
            chkBookflag.EditValue = _currentVoucher.Bookflag;

            // Accordion: Thông tin in chứng từ & ngân hàng
            txtPersonname.EditValue = _currentVoucher.Personname;
            txtPersonaddress.EditValue = _currentVoucher.Personaddress;
            txtInvoicesnumber.EditValue = _currentVoucher.Invoicesnumber;
            txtPaymentacc.EditValue = _currentVoucher.Paymentacc;
            txtPaymentbank.EditValue = _currentVoucher.Paymentbank;
            txtRecievedcorp.EditValue = _currentVoucher.Recievedcorp;
            txtRecievedacc.EditValue = _currentVoucher.Recievedacc;
            txtRecievedbank.EditValue = _currentVoucher.Recievedbank;
        }

        private void UnbindHeader()
        {
            if (_currentVoucher == null) return;
            string vNo = txtVoucherNo.Text?.Trim() ?? string.Empty;
            _currentVoucher.VoucherNo = vNo.Length > 10 ? vNo.Substring(0, 10) : vNo;
            _currentVoucher.VoucherDate = txtVoucherDate.DateTime;

            string contents = txtContents.Text?.Trim() ?? string.Empty;
            _currentVoucher.Contents = contents.Length > 100 ? contents.Substring(0, 100) : contents;
            _currentVoucher.Bookflag = chkBookflag.IsChecked ?? false;

            // Accordion: Thông tin in chứng từ & ngân hàng
            string personName = txtPersonname.Text?.Trim();
            _currentVoucher.Personname = personName != null && personName.Length > 50 ? personName.Substring(0, 50) : personName;

            string personAddress = txtPersonaddress.Text?.Trim();
            _currentVoucher.Personaddress = personAddress != null && personAddress.Length > 255 ? personAddress.Substring(0, 255) : personAddress;

            string invoicesNumber = txtInvoicesnumber.Text?.Trim();
            _currentVoucher.Invoicesnumber = invoicesNumber != null && invoicesNumber.Length > 2 ? invoicesNumber.Substring(0, 2) : invoicesNumber;

            string paymentAcc = txtPaymentacc.Text?.Trim();
            _currentVoucher.Paymentacc = paymentAcc != null && paymentAcc.Length > 50 ? paymentAcc.Substring(0, 50) : paymentAcc;

            string paymentBank = txtPaymentbank.Text?.Trim();
            _currentVoucher.Paymentbank = paymentBank != null && paymentBank.Length > 150 ? paymentBank.Substring(0, 150) : paymentBank;

            string receivedCorp = txtRecievedcorp.Text?.Trim();
            _currentVoucher.Recievedcorp = receivedCorp != null && receivedCorp.Length > 150 ? receivedCorp.Substring(0, 150) : receivedCorp;

            string receivedAcc = txtRecievedacc.Text?.Trim();
            _currentVoucher.Recievedacc = receivedAcc != null && receivedAcc.Length > 50 ? receivedAcc.Substring(0, 50) : receivedAcc;

            string receivedBank = txtRecievedbank.Text?.Trim();
            _currentVoucher.Recievedbank = receivedBank != null && receivedBank.Length > 150 ? receivedBank.Substring(0, 150) : receivedBank;
        }

        private void cboPrintType_EditValueChanged(object sender, EditValueChangedEventArgs e)
        {
            if (cboPrintType.EditValue is VoucherPrintType selectedType)
            {
                bool isSpecificPrint = (selectedType != VoucherPrintType.Normal);
                if (btnPrintToolbar != null) btnPrintToolbar.IsEnabled = isSpecificPrint;
                if (btnPrintHeader != null) btnPrintHeader.IsEnabled = isSpecificPrint;

                if (grpPrintAndBankInfo != null)
                {
                    grpPrintAndBankInfo.IsCollapsed = !isSpecificPrint;
                }
            }
        }

        private void BtnPrint_Click(object sender, RoutedEventArgs e)
        {
            ExecutePrint();
        }

        private void BtnPrint_Click(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            ExecutePrint();
        }

        private void ExecutePrint()
        {
            UnbindHeader();
            var selectedType = (cboPrintType.EditValue is VoucherPrintType pt) ? pt : VoucherPrintType.Normal;

            switch (selectedType)
            {
                case VoucherPrintType.CashReceipt:
                    PrintCashReceipt(_currentVoucher);
                    break;
                case VoucherPrintType.CashPayment:
                    PrintCashPayment(_currentVoucher);
                    break;
                case VoucherPrintType.BankPaymentOrder:
                    PrintBankPaymentOrder(_currentVoucher);
                    break;
                case VoucherPrintType.BankDeposit:
                    PrintBankDeposit(_currentVoucher);
                    break;
                case VoucherPrintType.Normal:
                default:
                    PrintNormalVoucher(_currentVoucher);
                    break;
            }
        }

        // =========================================================================
        // KHUNG HÀM IN CHỨNG TỪ (STUB METHODS - SẼ TRIỂN KHAI CHI TIẾT SAU)
        // =========================================================================

        private void PrintNormalVoucher(JournalVoucherEntity voucher)
        {
            string voucherNo = voucher?.VoucherNo ?? "";
            MessageBox.Show(
                $"[Khung chờ triển khai] In Phiếu kế toán thông thường\n- Số chứng từ: {voucherNo}\n- Ngày: {voucher?.VoucherDate:dd/MM/yyyy}",
                "In Phiếu thông thường",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private CashVoucherPrintModel BuildCashPrintModel(JournalVoucherEntity voucher, bool isReceipt)
        {
            var model = new CashVoucherPrintModel();
            var dbCfg = DbConfigService.Current;
            if (!string.IsNullOrWhiteSpace(dbCfg.CompanyName)) model.CompanyName = dbCfg.CompanyName;
            if (!string.IsNullOrWhiteSpace(dbCfg.Address)) model.CompanyAddress = dbCfg.Address;
            if (!string.IsNullOrWhiteSpace(dbCfg.TaxCode)) model.CompanyTaxCode = dbCfg.TaxCode;
            if (voucher == null) return model;

            model.VoucherNo = voucher.VoucherNo ?? "";
            model.VoucherDate = voucher.VoucherDate ?? DateTime.Today;
            model.PersonName = voucher.Personname ?? "";
            model.PersonAddress = voucher.Personaddress ?? "";
            model.Contents = voucher.Contents ?? "";
            model.OriginalVouchersCount = voucher.Invoicesnumber ?? "";

            // Căn cứ vào Journal Entry (Master) đang được định vị trên gridLeft
            var selectedMaster = gridLeft.SelectedItem as JournalEntryEntity
                                 ?? _masterEntries.FirstOrDefault(m => !string.IsNullOrEmpty(m.AccountId));

            if (selectedMaster != null)
            {
                // Danh sách toàn bộ định khoản trong cụm giao dịch đang chọn (gồm Master và các Detail con của nó)
                var clusterEntries = new List<JournalEntryEntity> { selectedMaster };
                if (selectedMaster.SubEntries != null && selectedMaster.SubEntries.Count > 0)
                {
                    clusterEntries.AddRange(selectedMaster.SubEntries.Where(d => !string.IsNullOrEmpty(d.AccountId)));
                }

                var debitEntries = clusterEntries.Where(e => e.Dbcr == 1).ToList();
                var creditEntries = clusterEntries.Where(e => e.Dbcr == -1).ToList();

                // Định dạng hiển thị các dòng Nợ
                if (debitEntries.Count > 0)
                {
                    model.DebitAccountsText = string.Join("\n", debitEntries.Select(d => $"Nợ {d.AccountId} : {d.Amount:N0} đ"));
                }
                else
                {
                    model.DebitAccountsText = "Nợ:";
                }

                // Định dạng hiển thị các dòng Có
                if (creditEntries.Count > 0)
                {
                    model.CreditAccountsText = string.Join("\n", creditEntries.Select(c => $"Có {c.AccountId} : {c.Amount:N0} đ"));
                }
                else
                {
                    model.CreditAccountsText = "Có:";
                }

                // Tính tổng tiền theo định khoản phát sinh của cụm giao dịch
                decimal sumDebit = (decimal)debitEntries.Sum(d => d.Amount ?? 0.0);
                decimal sumCredit = (decimal)creditEntries.Sum(c => c.Amount ?? 0.0);
                model.Amount = isReceipt
                    ? (sumDebit > 0 ? sumDebit : sumCredit)
                    : (sumCredit > 0 ? sumCredit : sumDebit);

                if (model.Amount == 0 && selectedMaster.Amount.HasValue)
                {
                    model.Amount = (decimal)selectedMaster.Amount.Value;
                }
            }
            else
            {
                model.Amount = 0;
            }

            model.AmountInWords = NumberToWordsConverter.ConvertToWords(model.Amount, "đồng");
            return model;
        }

        private void PrintCashReceipt(JournalVoucherEntity voucher)
        {
            try
            {
                var model = BuildCashPrintModel(voucher, isReceipt: true);

                var report = new Reports.CashReceiptReport();
                report.SetModel(model);
                report.CreateDocument(false);

                var previewWindow = new DevExpress.Xpf.Printing.DocumentPreviewWindow
                {
                    Owner = this,
                    Title = $"In Phiếu Thu - Số: {model.VoucherNo}"
                };
                previewWindow.PreviewControl.CommandBarStyle = DevExpress.Xpf.DocumentViewer.CommandBarStyle.Ribbon;
                previewWindow.PreviewControl.DocumentSource = report;
                previewWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi in Phiếu thu: {ex.Message}", "Lỗi In ấn", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void PrintCashPayment(JournalVoucherEntity voucher)
        {
            try
            {
                var model = BuildCashPrintModel(voucher, isReceipt: false);

                var report = new Reports.CashPaymentReport();
                report.SetModel(model);
                report.CreateDocument(false);

                var previewWindow = new DevExpress.Xpf.Printing.DocumentPreviewWindow
                {
                    Owner = this,
                    Title = $"In Phiếu Chi - Số: {model.VoucherNo}"
                };
                previewWindow.PreviewControl.CommandBarStyle = DevExpress.Xpf.DocumentViewer.CommandBarStyle.Ribbon;
                previewWindow.PreviewControl.DocumentSource = report;
                previewWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi in Phiếu chi: {ex.Message}", "Lỗi In ấn", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void PrintBankPaymentOrder(JournalVoucherEntity voucher)
        {
            string voucherNo = voucher?.VoucherNo ?? "";
            MessageBox.Show(
                $"[Khung chờ triển khai] In Ủy nhiệm chi ngân hàng\n- Số chứng từ: {voucherNo}\n- TK trả: {voucher?.Paymentacc} ({voucher?.Paymentbank})\n- Đơn vị nhận: {voucher?.Recievedcorp} - TK: {voucher?.Recievedacc} ({voucher?.Recievedbank})",
                "In Ủy nhiệm chi",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void PrintBankDeposit(JournalVoucherEntity voucher)
        {
            string voucherNo = voucher?.VoucherNo ?? "";
            MessageBox.Show(
                $"[Khung chờ triển khai] In Giấy nộp tiền ngân hàng\n- Số chứng từ: {voucherNo}\n- Người nộp: {voucher?.Personname}\n- Đơn vị nhận: {voucher?.Recievedcorp} - TK: {voucher?.Recievedacc} ({voucher?.Recievedbank})",
                "In Giấy nộp tiền",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CommitCurrentVoucherToBatch();

                // Gọi Cổng ghi sổ Posting Service (Core)
                var result = _postingService.PostBatch(_voucherBatch, _deletedVouchersInBatch);

                if (!result.Success)
                {
                    // Nếu lỗi do hạn mức bản quyền
                    var limitError = result.Issues.FirstOrDefault(i => i.Code == "RECORD_LIMIT_EXCEEDED");
                    if (limitError != null)
                    {
                        var prompt = ThemedMessageBox.Show(
                            "Hạn mức Chứng từ",
                            $"{limitError.Message}\n\nBạn có muốn mở cửa sổ Đăng ký Bản quyền để gia tăng số lượng chứng từ không?",
                            MessageBoxButton.YesNo, MessageBoxImage.Information);
                        if (prompt == MessageBoxResult.Yes)
                        {
                            var regWin = new NewaccNet.Wpf.AppSystem.CompanyRegistrationWindow { Owner = this };
                            regWin.ShowDialog();
                        }
                        return;
                    }

                    // Nếu lỗi do mất cân đối hoặc khóa sổ, nhảy đến chứng từ có lỗi
                    var unbalanced = result.Issues.FirstOrDefault(i => i.Code == "UNBALANCED_ENTRIES" || i.Code == "PERIOD_LOCKED_OR_EXPIRED");
                    if (unbalanced != null)
                    {
                        for (int i = 0; i < _voucherBatch.Count; i++)
                        {
                            var v = _voucherBatch[i];
                            double bal = v.JournalEntries.Sum(je => (je.Amount ?? 0) * (je.Dbcr ?? 0));
                            if (Math.Round(bal, 0) != 0 || (v.VoucherDate.HasValue && !NewaccNet.Core.Data.DbConfigService.CanUpdateVoucher(v.VoucherDate.Value, out _)))
                            {
                                NavigateTo(i);
                                break;
                            }
                        }
                    }

                    MessageBox.Show(result.ErrorMessage, "Lỗi khi lưu chứng từ", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                _deletedVouchersInBatch.Clear();
                _isDirty = false;
                HasSaved = true;

                // Nạp lại voucher hiện tại từ DB để cập nhật ID mới
                if (_currentVoucher != null && _currentVoucher.Id != 0)
                {
                    var fresh = FetchVoucherById(_currentVoucher.Id);
                    if (fresh != null)
                    {
                        _voucherBatch[_currentVoucherIndex] = fresh;
                        SetCurrentVoucher(fresh);
                    }
                }

                MessageBox.Show($"Lưu thành công toàn bộ {_voucherBatch.Count} chứng từ trong lô!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lưu lô chứng từ: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnDeleteVoucher_Click(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            if (_currentVoucher == null) return;

            string voucherNo = !string.IsNullOrWhiteSpace(_currentVoucher.VoucherNo) ? _currentVoucher.VoucherNo : $"ID {_currentVoucher.Id}";
            if (MessageBox.Show($"Bạn có chắc chắn muốn xóa chứng từ '{voucherNo}' khỏi lô đang nhập?", 
                                "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
            {
                return;
            }

            try
            {
                if (_currentVoucher.Id > 0)
                {
                    _deletedVouchersInBatch.Add(_currentVoucher);
                }

                int removeIndex = _currentVoucherIndex;
                _voucherBatch.RemoveAt(removeIndex);

                if (_voucherBatch.Count == 0)
                {
                    var fresh = CreateNewVoucher();
                    _voucherBatch.Add(fresh);
                    _currentVoucherIndex = 0;
                    SetCurrentVoucher(fresh);
                }
                else
                {
                    int nextIndex = removeIndex >= _voucherBatch.Count ? _voucherBatch.Count - 1 : removeIndex;
                    _currentVoucherIndex = -1;
                    NavigateTo(nextIndex);
                }

                _isDirty = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xóa chứng từ: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
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
    }
}












