using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using DataAccess.EntityClasses;
using DataAccess.HelperClasses;
using SD.LLBLGen.Pro.ORMSupportClasses;
using NewaccNet.Core.Data;

namespace NewaccNet.Wpf.AppSystem.Voucher.Details
{
    /// <summary>
    /// Quản lý giao dịch Vật tư theo mô hình ORM chuẩn mực:
    /// - Mọi JournalEntry sinh ra đều là con của JournalVoucher (Voucher.JournalEntries), không tạo mồ côi.
    /// - Khi mới gõ Nợ/Có: Mặc nhiên là Nhập/Xuất nội bộ (NNB/XNB), không mua bán, footer ẩn.
    /// - Chỉ khi chọn Mua bán (NM/XB): Cờ IsTrade bật, footer hiện, sinh thêm JournalEntry con/cha thuộc Voucher.
    /// - Khi sửa trên grid: Domino cập nhật Amount vào dòng con -> dòng cha.
    /// </summary>
    public class MaterialTransactionContext : INotifyPropertyChanged
    {
        private bool _hasTax = false;
        private string _warehouseId = "";

        private string _costAccountId = "6321";
        private string _revenueAccountId = "5111";
        private string _taxAccountId = "13311";
        private string _totalAccountId = "3311";

        private double _totalCostAmount = 0;
        private double _totalSellAmount = 0;
        private double _totalTaxAmount = 0;
        private double _totalAllAmount = 0;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        #region Properties Binding
        public bool IsTrade => Taxflag;
        public Visibility TradeFooterVisibility => IsTrade ? Visibility.Visible : Visibility.Collapsed;
        public Visibility NonTradeFooterVisibility => IsTrade ? Visibility.Collapsed : Visibility.Visible;
        public Visibility SalesVisibility => (VoucherType == "XB") ? Visibility.Visible : Visibility.Collapsed;
        public Visibility PurchaseVisibility => (VoucherType == "NM") ? Visibility.Visible : Visibility.Collapsed;

        public bool Taxflag
        {
            get => InventoryVoucher?.Taxflag ?? _hasTax;
            set
            {
                bool changed = (InventoryVoucher != null && InventoryVoucher.Taxflag != value) || (_hasTax != value);
                _hasTax = value;
                if (InventoryVoucher != null)
                {
                    InventoryVoucher.Taxflag = value;
                }

                if (changed)
                {
                    OnTaxflagChanged();
                }
            }
        }

        public string VoucherType
        {
            get
            {
                if (InventoryVoucher != null && !string.IsNullOrEmpty(InventoryVoucher.VoucherType))
                    return InventoryVoucher.VoucherType;

                int dbcr = MaterialEntry?.Dbcr ?? 1;
                return dbcr == -1
                    ? (Taxflag ? "XB" : "XNB")
                    : (Taxflag ? "NM" : "NNB");
            }
            set
            {
                if (InventoryVoucher != null)
                {
                    InventoryVoucher.VoucherType = value;
                }

                if (!string.IsNullOrEmpty(value))
                {
                    if (value == "XB" || value == "XNB")
                    {
                        if (MaterialEntry != null) MaterialEntry.Dbcr = -1;
                    }
                    else if (value == "NM" || value == "NNB")
                    {
                        if (MaterialEntry != null) MaterialEntry.Dbcr = 1;
                    }

                    Taxflag = (value == "NM" || value == "XB");
                }
            }
        }

        private void OnTaxflagChanged()
        {
            OnPropertyChanged(nameof(Taxflag));
            OnPropertyChanged(nameof(HasTax));
            OnPropertyChanged(nameof(VoucherType));
            OnPropertyChanged(nameof(IsTrade));
            OnPropertyChanged(nameof(TradeFooterVisibility));
            OnPropertyChanged(nameof(NonTradeFooterVisibility));
            OnPropertyChanged(nameof(SalesVisibility));
            OnPropertyChanged(nameof(PurchaseVisibility));

            if (Taxflag)
            {
                UpdateDefaultAccountsForType(VoucherType);
                RebuildTradeHierarchy();
            }
            else
            {
                TearDownTradeHierarchy();
            }

            RecalculateTotals();
        }

        public bool Printflag
        {
            get => InventoryVoucher?.Printflag ?? false;
            set
            {
                if (InventoryVoucher != null && InventoryVoucher.Printflag != value)
                {
                    InventoryVoucher.Printflag = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(TaxInfoVisibility));
                }
            }
        }

        public Visibility TaxInfoVisibility => (InventoryVoucher?.Printflag == true) ? Visibility.Visible : Visibility.Collapsed;

        public bool HasTax
        {
            get => Taxflag;
            set => Taxflag = value;
        }

        public string WarehouseId
        {
            get => _warehouseId;
            set
            {
                if (_warehouseId != value)
                {
                    _warehouseId = value;
                    if (InventoryVoucher != null)
                    {
                        InventoryVoucher.WarehouseId = value;
                    }
                    OnPropertyChanged();
                }
            }
        }

        public string CostAccountId
        {
            get => _costAccountId;
            set 
            { 
                if (_costAccountId != value) 
                { 
                    _costAccountId = value; 
                    OnPropertyChanged(); 
                    if (CostMasterEntry != null) CostMasterEntry.AccountId = value;
                } 
            }
        }

        public string RevenueAccountId
        {
            get => _revenueAccountId;
            set 
            { 
                if (_revenueAccountId != value) 
                { 
                    _revenueAccountId = value; 
                    OnPropertyChanged(); 
                    if (RevEntry != null) RevEntry.AccountId = value;
                } 
            }
        }

        public string TaxAccountId
        {
            get => _taxAccountId;
            set 
            { 
                if (_taxAccountId != value) 
                { 
                    _taxAccountId = value; 
                    OnPropertyChanged(); 
                    if (TaxEntry != null) TaxEntry.AccountId = value;
                } 
            }
        }

        public string TotalAccountId
        {
            get => _totalAccountId;
            set 
            { 
                if (_totalAccountId != value) 
                { 
                    _totalAccountId = value; 
                    OnPropertyChanged(); 
                    if (PayMasterEntry != null) PayMasterEntry.AccountId = value;
                    if (RevMasterEntry != null) RevMasterEntry.AccountId = value;
                } 
            }
        }

        public double TotalCostAmount
        {
            get => _totalCostAmount;
            set { if (_totalCostAmount != value) { _totalCostAmount = value; OnPropertyChanged(); } }
        }

        private double _currentVoucherCostAmount = 0;
        public double CurrentVoucherCostAmount
        {
            get => _currentVoucherCostAmount;
            set { if (_currentVoucherCostAmount != value) { _currentVoucherCostAmount = value; OnPropertyChanged(); } }
        }

        public double TotalSellAmount
        {
            get => _totalSellAmount;
            set { if (_totalSellAmount != value) { _totalSellAmount = value; OnPropertyChanged(); } }
        }

        public double TotalTaxAmount
        {
            get => _totalTaxAmount;
            set { if (_totalTaxAmount != value) { _totalTaxAmount = value; OnPropertyChanged(); } }
        }

        public double TotalAllAmount
        {
            get => _totalAllAmount;
            set { if (_totalAllAmount != value) { _totalAllAmount = value; OnPropertyChanged(); } }
        }

        private int _currentVoucherIndex = 0;
        public int CurrentVoucherIndex => _currentVoucherIndex;
        public int VoucherCount => MaterialEntry?.InventoryVouchers?.Count ?? 0;
        public string NavigationPositionText => $"Phiếu: {(VoucherCount > 0 ? _currentVoucherIndex + 1 : 0)} / {VoucherCount}";
        public bool CanNavigatePrev => _currentVoucherIndex > 0;
        public bool CanNavigateNext => _currentVoucherIndex < VoucherCount - 1;
        #endregion

        #region Underlying Entities
        /// <summary>
        /// Chứng từ JournalVoucher cha quản lý tất cả các JournalEntries
        /// </summary>
        public JournalVoucherEntity Voucher { get; private set; }

        private JournalEntryEntity _materialEntry;
        /// <summary>
        /// Dòng JournalEntry mang tập hợp InventoryVouchers (quan hệ 1-nhiều)
        /// </summary>
        public JournalEntryEntity MaterialEntry
        {
            get => _materialEntry;
            private set
            {
                if (!ReferenceEquals(_materialEntry, value))
                {
                    if (_materialEntry != null)
                    {
                        _materialEntry.PropertyChanged -= MaterialEntry_PropertyChanged;
                    }
                    _materialEntry = value;
                    if (_materialEntry != null)
                    {
                        _materialEntry.PropertyChanged += MaterialEntry_PropertyChanged;
                    }
                }
            }
        }

        private bool _isRecalculatingTotals = false;
        private void MaterialEntry_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (_isRecalculatingTotals) return;
            if (e.PropertyName == "ChildDetailsChanged")
            {
                RecalculateTotals();
            }
        }

        /// <summary>
        /// InventoryVoucher hiện hành đang hiển thị trên form (nằm trong MaterialEntry.InventoryVouchers)
        /// </summary>
        public InventoryVoucherEntity InventoryVoucher { get; private set; }

        // NM: Cha Có 331, Con 1 Nợ 152/156 (MaterialEntry), Con 2 Nợ 13311 (TaxEntry)
        public JournalEntryEntity PayMasterEntry { get; private set; }
        public JournalEntryEntity TaxEntry { get; private set; }

        // XB:
        // Cặp Giá vốn: Cha Nợ 6321 (CostMasterEntry), Con Có 156 (MaterialEntry)
        // Cặp Bán hàng: Cha Nợ 131 (RevMasterEntry), Con 1 Có 5111 (RevEntry), Con 2 Có 33311 (TaxEntry)
        public JournalEntryEntity CostMasterEntry { get; private set; }
        public JournalEntryEntity RevMasterEntry { get; private set; }
        public JournalEntryEntity RevEntry { get; private set; }

        /// <summary>
        /// Dòng hạch toán Đối ứng/Thanh toán tổng:
        /// - Khi Nhập mua (NM): là PayMasterEntry (Có 331)
        /// - Khi Xuất bán (XB): là RevMasterEntry (Nợ 131)
        /// </summary>
        public JournalEntryEntity TotalEntry => (VoucherType == "XB") ? RevMasterEntry : PayMasterEntry;

        public bool IsNew { get; private set; }

        /// <summary>
        /// Chuyển đổi phiếu kho (InventoryVoucher) đang hiển thị trong danh sách MaterialEntry.InventoryVouchers
        /// </summary>
        public void SetActiveInventoryVoucher(int index)
        {
            if (MaterialEntry?.InventoryVouchers == null || MaterialEntry.InventoryVouchers.Count == 0)
                return;

            if (index < 0) index = 0;
            if (index >= MaterialEntry.InventoryVouchers.Count)
                index = MaterialEntry.InventoryVouchers.Count - 1;

            _currentVoucherIndex = index;
            InventoryVoucher = MaterialEntry.InventoryVouchers[_currentVoucherIndex];
            _warehouseId = InventoryVoucher.WarehouseId ?? "";
            _hasTax = InventoryVoucher.Taxflag;

            OnPropertyChanged(nameof(InventoryVoucher));
            OnPropertyChanged(nameof(CurrentLines));
            OnPropertyChanged(nameof(WarehouseId));
            OnPropertyChanged(nameof(Taxflag));
            OnPropertyChanged(nameof(HasTax));
            OnPropertyChanged(nameof(Printflag));
            OnPropertyChanged(nameof(TaxInfoVisibility));
            OnPropertyChanged(nameof(VoucherType));
            OnPropertyChanged(nameof(CurrentVoucherIndex));
            OnPropertyChanged(nameof(VoucherCount));
            OnPropertyChanged(nameof(NavigationPositionText));
            OnPropertyChanged(nameof(CanNavigatePrev));
            OnPropertyChanged(nameof(CanNavigateNext));

            RecalculateTotals();
        }

        public EntityCollection<InventoryVoucherLineEntity> CurrentLines => InventoryVoucher?.InventoryVoucherLines;

        public void NavigateTo(int index)
        {
            if (index < 0 || index >= VoucherCount || index == _currentVoucherIndex)
                return;
            SetActiveInventoryVoucher(index);
        }

        public void MoveFirst() => NavigateTo(0);
        public void MovePrev() => NavigateTo(_currentVoucherIndex - 1);
        public void MoveNext() => NavigateTo(_currentVoucherIndex + 1);
        public void MoveLast() => NavigateTo(VoucherCount - 1);

        public InventoryVoucherEntity AddNewInventoryVoucher(string defaultWarehouseId = "")
        {
            var newIv = new InventoryVoucherEntity
            {
                Taxflag = this.Taxflag,
                Printflag = false,
                WarehouseId = defaultWarehouseId
            };
            MaterialEntry.InventoryVouchers.Add(newIv);
            SetActiveInventoryVoucher(MaterialEntry.InventoryVouchers.Count - 1);
            return newIv;
        }

        public void RemoveCurrentInventoryVoucher(string defaultWarehouseId = "")
        {
            if (MaterialEntry?.InventoryVouchers == null || InventoryVoucher == null)
                return;

            int removeIdx = _currentVoucherIndex;
            MaterialEntry.InventoryVouchers.Remove(InventoryVoucher);

            if (MaterialEntry.InventoryVouchers.Count == 0)
            {
                var freshIv = new InventoryVoucherEntity
                {
                    Taxflag = this.Taxflag,
                    Printflag = false,
                    WarehouseId = defaultWarehouseId
                };
                MaterialEntry.InventoryVouchers.Add(freshIv);
            }

            int nextIdx = Math.Min(removeIdx, MaterialEntry.InventoryVouchers.Count - 1);
            SetActiveInventoryVoucher(nextIdx);
        }
        #endregion

        /// <summary>
        /// Tạo mới cụm giao dịch từ một seed entry trên lưới.
        /// Mặc nhiên: Gõ Nợ là Nhập nội bộ (NNB), Gõ Có là Xuất nội bộ (XNB). Không mua bán!
        /// </summary>
        public static MaterialTransactionContext CreateNew(JournalEntryEntity seedEntry, JournalVoucherEntity voucher = null)
        {
            return CreateNew(seedEntry, null, voucher);
        }

        public static MaterialTransactionContext CreateNew(JournalEntryEntity seedEntry, string voucherType, JournalVoucherEntity voucher = null)
        {
            var ctx = new MaterialTransactionContext();
            ctx.IsNew = true;
            ctx.MaterialEntry = seedEntry ?? new JournalEntryEntity();
            ctx.Voucher = voucher ?? seedEntry?.JournalVoucher;

            // Đảm bảo MaterialEntry thuộc về Voucher
            if (ctx.Voucher != null && !ctx.Voucher.JournalEntries.Contains(ctx.MaterialEntry))
            {
                ctx.Voucher.JournalEntries.Add(ctx.MaterialEntry);
            }

            if (!string.IsNullOrEmpty(voucherType))
            {
                ctx.VoucherType = voucherType;
            }
            else
            {
                // Mặc định: Nợ = NNB, Có = XNB
                if (ctx.MaterialEntry.Dbcr == -1)
                {
                    ctx.MaterialEntry.Dbcr = -1;
                    ctx.Taxflag = false;
                }
                else
                {
                    ctx.MaterialEntry.Dbcr = 1;
                    ctx.Taxflag = false;
                }
            }

            // Gắn InventoryVoucher vào MaterialEntry (để ExobjectId là null tránh vi phạm khóa ngoại CostObject)
            ctx.InventoryVoucher = new InventoryVoucherEntity();
            ctx.InventoryVoucher.Taxflag = ctx.Taxflag;
            ctx.MaterialEntry.InventoryVouchers.Clear();
            ctx.MaterialEntry.InventoryVouchers.Add(ctx.InventoryVoucher);

            if (ctx.IsTrade)
            {
                ctx.UpdateDefaultAccountsForType(ctx.VoucherType);
                ctx.RebuildTradeHierarchy();
            }

            ctx.RecalculateTotals();
            return ctx;
        }

        /// <summary>
        /// Tái tạo cụm giao dịch từ Entity đã có sẵn trong Voucher hoặc DB.
        /// Tuyệt đối KHÔNG DỰA VÀO MÃ TÀI KHOẢN để nhận diện vị trí các dòng!
        /// Nhận diện hoàn toàn theo quan hệ Cha-Con (ParentId) và tính chất hóa đơn.
        /// </summary>
        public static MaterialTransactionContext FromExisting(JournalEntryEntity entry, JournalVoucherEntity voucher = null)
        {
            var ctx = new MaterialTransactionContext();
            ctx.IsNew = false;
            ctx.Voucher = voucher ?? entry?.JournalVoucher ?? entry?.ParentEntry?.JournalVoucher;

            // 1. Xác định dòng mang InventoryVoucher (Vật tư)
            JournalEntryEntity matEntry = null;
            if (entry.InventoryVouchers != null && entry.InventoryVouchers.Count > 0)
            {
                matEntry = entry;
            }
            else if (entry.SubEntries != null)
            {
                matEntry = entry.SubEntries.FirstOrDefault(s => s.InventoryVouchers != null && s.InventoryVouchers.Count > 0);
            }

            if (matEntry == null && ctx.Voucher != null)
            {
                matEntry = ctx.Voucher.JournalEntries.FirstOrDefault(s => s.InventoryVouchers != null && s.InventoryVouchers.Count > 0);
            }

            if (matEntry == null)
            {
                return CreateNew(entry, voucher);
            }

            ctx.MaterialEntry = matEntry;
            ctx.InventoryVoucher = matEntry.InventoryVouchers[0];
            ctx._warehouseId = ctx.InventoryVoucher.WarehouseId ?? "";
            // Gán trực tiếp backing field _hasTax để KHÔNG kích hoạt OnTaxflagChanged() / RebuildTradeHierarchy()
            // trước khi các entity liên kết (CostMasterEntry, RevMasterEntry, v.v.) được nhận diện!
            ctx._hasTax = ctx.InventoryVoucher.Taxflag;

            // 2. Nhận diện lại các Entity quan hệ nếu là MUA BÁN (NM / XB)
            if (ctx.VoucherType == "NM")
            {
                // Trong Nhập mua (NM): Có 1 Cha thanh toán, 2 Con (1 con Vật tư, 1 con Thuế)
                // Dòng Cha thanh toán (PayMasterEntry):
                JournalEntryEntity payMaster = matEntry.ParentEntry;
                if (payMaster == null && matEntry.ParentId.HasValue && ctx.Voucher != null)
                {
                    payMaster = ctx.Voucher.JournalEntries.FirstOrDefault(x => x.Id == matEntry.ParentId.Value);
                }
                if (payMaster == null && entry != matEntry && entry.Dbcr == -1)
                {
                    payMaster = entry;
                }
                if (payMaster == null && ctx.Voucher != null)
                {
                    payMaster = ctx.Voucher.JournalEntries.FirstOrDefault(x => x != matEntry && x.Dbcr == -1);
                }

                ctx.PayMasterEntry = payMaster;
                if (ctx.PayMasterEntry != null)
                {
                    ctx._totalAccountId = ctx.PayMasterEntry.AccountId ?? "3311";
                    matEntry.ParentEntry = ctx.PayMasterEntry;
                    if (!ctx.PayMasterEntry.SubEntries.Contains(matEntry))
                        ctx.PayMasterEntry.SubEntries.Add(matEntry);

                    // Dòng Con Thuế: Trong các con của PayMasterEntry, dòng con còn lại khác matEntry chính là Thuế!
                    var candidateSubs = ctx.Voucher != null
                        ? ctx.Voucher.JournalEntries.Where(s => s != matEntry && (s.ParentEntry == ctx.PayMasterEntry || (s.ParentId.HasValue && ctx.PayMasterEntry.Id != 0 && s.ParentId == ctx.PayMasterEntry.Id))).ToList()
                        : ctx.PayMasterEntry.SubEntries.Where(s => s != matEntry).ToList();

                    var tax = candidateSubs.FirstOrDefault();
                    if (tax != null)
                    {
                        ctx.TaxEntry = tax;
                        tax.ParentEntry = ctx.PayMasterEntry;
                        if (!ctx.PayMasterEntry.SubEntries.Contains(tax))
                            ctx.PayMasterEntry.SubEntries.Add(tax);
                        ctx._taxAccountId = tax.AccountId ?? "13311";
                    }
                }

                if (ctx.TaxEntry == null)
                {
                    ctx.TaxEntry = new JournalEntryEntity { Dbcr = 1, AccountId = "13311" };
                    if (ctx.Voucher != null && !ctx.Voucher.JournalEntries.Contains(ctx.TaxEntry))
                        ctx.Voucher.JournalEntries.Add(ctx.TaxEntry);
                    if (ctx.PayMasterEntry != null && !ctx.PayMasterEntry.SubEntries.Contains(ctx.TaxEntry))
                        ctx.PayMasterEntry.SubEntries.Add(ctx.TaxEntry);
                    ctx._taxAccountId = "13311";
                }
            }
            else if (ctx.VoucherType == "XB")
            {
                // Trong Xuất bán (XB):
                // Nhánh 1: Cặp Giá vốn (Cha CostMasterEntry -> Con matEntry mang InventoryVouchers)
                JournalEntryEntity costMaster = matEntry.ParentEntry;
                if (costMaster == null && matEntry.ParentId.HasValue && ctx.Voucher != null)
                {
                    costMaster = ctx.Voucher.JournalEntries.FirstOrDefault(x => x.Id == matEntry.ParentId.Value);
                }
                if (costMaster == null && ctx.Voucher != null)
                {
                    costMaster = ctx.Voucher.JournalEntries.FirstOrDefault(x => x != matEntry && x.SubEntries.Contains(matEntry));
                }

                ctx.CostMasterEntry = costMaster;
                if (ctx.CostMasterEntry != null)
                {
                    ctx._costAccountId = ctx.CostMasterEntry.AccountId ?? "6321";
                    matEntry.ParentEntry = ctx.CostMasterEntry;
                    if (!ctx.CostMasterEntry.SubEntries.Contains(matEntry))
                        ctx.CostMasterEntry.SubEntries.Add(matEntry);
                }

                // Nhánh 2: Cặp Bán hàng (Cha RevMasterEntry có 2 con: Doanh thu và Thuế)
                if (ctx.Voucher != null)
                {
                    var masterCandidates = ctx.Voucher.JournalEntries
                        .Where(j => j != ctx.CostMasterEntry && j != matEntry && (j.ParentEntry == null || j.ParentId == null || j.ParentId == 0))
                        .ToList();

                    // Thằng tổng thu là tài khoản Master có các dòng con
                    ctx.RevMasterEntry = masterCandidates.FirstOrDefault(m => {
                        int subCount = ctx.Voucher.JournalEntries.Count(x => x != m && (x.ParentEntry == m || (x.ParentId.HasValue && m.Id != 0 && x.ParentId == m.Id)));
                        return subCount >= 2;
                    }) ?? masterCandidates.FirstOrDefault();

                    if (ctx.RevMasterEntry != null)
                    {
                        ctx._totalAccountId = ctx.RevMasterEntry.AccountId ?? "131";

                        // Lấy tất cả các con của RevMasterEntry
                        var subRevList = ctx.Voucher.JournalEntries
                            .Where(x => x != ctx.RevMasterEntry && (x.ParentEntry == ctx.RevMasterEntry || (x.ParentId.HasValue && ctx.RevMasterEntry.Id != 0 && x.ParentId == ctx.RevMasterEntry.Id)))
                            .Distinct()
                            .ToList();

                        foreach (var sub in subRevList)
                        {
                            sub.ParentEntry = ctx.RevMasterEntry;
                            if (!ctx.RevMasterEntry.SubEntries.Contains(sub))
                                ctx.RevMasterEntry.SubEntries.Add(sub);
                        }

                        // ========================================================
                        // PHÂN ĐỊNH CON DOANH THU (GIÁ BÁN) VÀ CON THUẾ:
                        // Tuyệt đối không dựa vào chuỗi mã tài khoản!
                        // ========================================================
                        if (subRevList.Count >= 2)
                        {
                            // Tính trước tiền bán và tiền thuế từ các dòng chi tiết vật tư
                            double calcSell = 0;
                            double calcTax = 0;
                            if (ctx.InventoryVoucher?.InventoryVoucherLines != null)
                            {
                                foreach (var l in ctx.InventoryVoucher.InventoryVoucherLines)
                                {
                                    double s = (l.Quantity ?? 0) * (l.SellPrice ?? 0);
                                    calcSell += s;
                                    calcTax += s * (l.Rate ?? 0);
                                }
                            }

                            JournalEntryEntity revEntry = null;
                            JournalEntryEntity taxEntry = null;

                            // Tiêu chí 1: Kiểm tra theo chi tiết thuế phụ VatEntries (nếu dòng có kê khai thuế)
                            var withVat = subRevList.FirstOrDefault(s => s.VatEntries != null && s.VatEntries.Count > 0);
                            if (withVat != null)
                            {
                                taxEntry = withVat;
                                revEntry = subRevList.FirstOrDefault(s => s != taxEntry);
                            }

                            // Tiêu chí 2: So sánh số tiền khớp với tính toán từ chi tiết vật tư
                            if (revEntry == null && taxEntry == null && (calcSell > 0 || calcTax > 0) && Math.Abs(calcSell - calcTax) > 0.01)
                            {
                                var matchTax = subRevList.FirstOrDefault(s => Math.Abs((s.Amount ?? 0) - calcTax) < 0.01);
                                var matchSell = subRevList.FirstOrDefault(s => Math.Abs((s.Amount ?? 0) - calcSell) < 0.01);

                                if (matchTax != null && matchSell != null && matchTax != matchSell)
                                {
                                    taxEntry = matchTax;
                                    revEntry = matchSell;
                                }
                                else if (matchTax != null)
                                {
                                    taxEntry = matchTax;
                                    revEntry = subRevList.FirstOrDefault(s => s != taxEntry);
                                }
                                else if (matchSell != null)
                                {
                                    revEntry = matchSell;
                                    taxEntry = subRevList.FirstOrDefault(s => s != revEntry);
                                }
                            }

                            // Tiêu chí 3: Nếu số tiền bằng nhau hoặc chưa có, sắp xếp theo thứ tự Id (Doanh thu tạo trước Thuế)
                            if (revEntry == null && taxEntry == null)
                            {
                                var ordered = subRevList.OrderBy(s => s.Id).ToList();
                                revEntry = ordered[0];
                                taxEntry = ordered.Count > 1 ? ordered[1] : null;
                            }

                            ctx.RevEntry = revEntry;
                            ctx.TaxEntry = taxEntry;
                        }
                        else if (subRevList.Count == 1)
                        {
                            ctx.RevEntry = subRevList[0];
                        }

                        if (ctx.RevEntry != null) ctx._revenueAccountId = ctx.RevEntry.AccountId ?? "5111";
                        if (ctx.TaxEntry != null) ctx._taxAccountId = ctx.TaxEntry.AccountId ?? "33311";
                    }
                }

                if (ctx.RevMasterEntry == null)
                {
                    ctx.RevMasterEntry = new JournalEntryEntity { Dbcr = 1, AccountId = "131" };
                    if (ctx.Voucher != null && !ctx.Voucher.JournalEntries.Contains(ctx.RevMasterEntry))
                        ctx.Voucher.JournalEntries.Add(ctx.RevMasterEntry);
                }
                if (ctx.RevEntry == null)
                {
                    ctx.RevEntry = new JournalEntryEntity { Dbcr = -1, AccountId = "5111" };
                    if (ctx.Voucher != null && !ctx.Voucher.JournalEntries.Contains(ctx.RevEntry))
                        ctx.Voucher.JournalEntries.Add(ctx.RevEntry);
                    if (ctx.RevMasterEntry != null && !ctx.RevMasterEntry.SubEntries.Contains(ctx.RevEntry))
                        ctx.RevMasterEntry.SubEntries.Add(ctx.RevEntry);
                }
                if (ctx.TaxEntry == null)
                {
                    ctx.TaxEntry = new JournalEntryEntity { Dbcr = -1, AccountId = "33311" };
                    if (ctx.Voucher != null && !ctx.Voucher.JournalEntries.Contains(ctx.TaxEntry))
                        ctx.Voucher.JournalEntries.Add(ctx.TaxEntry);
                    if (ctx.RevMasterEntry != null && !ctx.RevMasterEntry.SubEntries.Contains(ctx.TaxEntry))
                        ctx.RevMasterEntry.SubEntries.Add(ctx.TaxEntry);
                }

                ctx._revenueAccountId = ctx.RevEntry.AccountId ?? "5111";
                ctx._taxAccountId = ctx.TaxEntry.AccountId ?? "33311";
                ctx._totalAccountId = ctx.RevMasterEntry.AccountId ?? "131";
            }

            ctx.RecalculateTotals();
            return ctx;
        }

        private void UpdateDefaultAccountsForType(string type)
        {
            var postingCfg = DbConfigService.Current.GetVoucherPostingConfig();
            if (postingCfg != null && postingCfg.Inventory != null && postingCfg.Inventory.TryGetValue(type, out var item))
            {
                if (!string.IsNullOrEmpty(item.CostAccountId)) _costAccountId = item.CostAccountId;
                if (!string.IsNullOrEmpty(item.RevenueAccountId)) _revenueAccountId = item.RevenueAccountId;
                if (!string.IsNullOrEmpty(item.TaxAccountId)) _taxAccountId = item.TaxAccountId;
                if (!string.IsNullOrEmpty(item.TotalAccountId)) _totalAccountId = item.TotalAccountId;
            }
            else
            {
                if (type == "NM")
                {
                    _costAccountId = "6321";
                    _revenueAccountId = "5111";
                    _taxAccountId = "13311";
                    _totalAccountId = "3311";
                }
                else if (type == "XB")
                {
                    _costAccountId = "6321";
                    _revenueAccountId = "5111";
                    _taxAccountId = "33311";
                    _totalAccountId = "131";
                }
            }

            OnPropertyChanged(nameof(CostAccountId));
            OnPropertyChanged(nameof(RevenueAccountId));
            OnPropertyChanged(nameof(TaxAccountId));
            OnPropertyChanged(nameof(TotalAccountId));
        }

        /// <summary>
        /// Xây dựng cấu trúc cây Entity (Cha - Con, Nợ - Có) khi chuyển sang MUA BÁN.
        /// Tất cả entity sinh ra đều được add vào Voucher.JournalEntries (không mồ côi).
        /// </summary>
        public void RebuildTradeHierarchy()
        {
            if (InventoryVoucher != null)
            {
                InventoryVoucher.Taxflag = HasTax;
            }

            if (VoucherType == "NM")
            {
                // Cha: Có 3311
                if (PayMasterEntry == null) PayMasterEntry = new JournalEntryEntity();
                PayMasterEntry.AccountId = TotalAccountId;
                PayMasterEntry.Dbcr = (short)-1;

                if (Voucher != null && !Voucher.JournalEntries.Contains(PayMasterEntry))
                    Voucher.JournalEntries.Add(PayMasterEntry);

                // Con 1: Nợ 152/156 (MaterialEntry)
                MaterialEntry.Dbcr = (short)1;

                // Con 2: Nợ 13311 (TaxEntry)
                if (TaxEntry == null) TaxEntry = new JournalEntryEntity();
                TaxEntry.AccountId = TaxAccountId;
                TaxEntry.Dbcr = (short)1;

                if (Voucher != null && !Voucher.JournalEntries.Contains(TaxEntry))
                    Voucher.JournalEntries.Add(TaxEntry);

                // Thiết lập quan hệ cha con
                MaterialEntry.ParentEntry = PayMasterEntry;
                TaxEntry.ParentEntry = PayMasterEntry;

                PayMasterEntry.SubEntries.Clear();
                if (!PayMasterEntry.SubEntries.Contains(MaterialEntry))
                    PayMasterEntry.SubEntries.Add(MaterialEntry);

                if (HasTax && !PayMasterEntry.SubEntries.Contains(TaxEntry))
                    PayMasterEntry.SubEntries.Add(TaxEntry);
            }
            else if (VoucherType == "XB")
            {
                // Cặp Giá vốn: Cha Nợ 6321 -> Con Có 156 (MaterialEntry)
                if (CostMasterEntry == null) CostMasterEntry = new JournalEntryEntity();
                CostMasterEntry.AccountId = CostAccountId;
                CostMasterEntry.Dbcr = (short)1;

                if (Voucher != null && !Voucher.JournalEntries.Contains(CostMasterEntry))
                    Voucher.JournalEntries.Add(CostMasterEntry);

                MaterialEntry.Dbcr = (short)-1;
                MaterialEntry.ParentEntry = CostMasterEntry;

                CostMasterEntry.SubEntries.Clear();
                if (!CostMasterEntry.SubEntries.Contains(MaterialEntry))
                    CostMasterEntry.SubEntries.Add(MaterialEntry);

                // Cặp Bán hàng: Cha Nợ 131 -> Con 1 Có 5111, Con 2 Có 33311
                if (RevMasterEntry == null) RevMasterEntry = new JournalEntryEntity();
                RevMasterEntry.AccountId = TotalAccountId;
                RevMasterEntry.Dbcr = (short)1;

                if (Voucher != null && !Voucher.JournalEntries.Contains(RevMasterEntry))
                    Voucher.JournalEntries.Add(RevMasterEntry);

                if (RevEntry == null) RevEntry = new JournalEntryEntity();
                RevEntry.AccountId = RevenueAccountId;
                RevEntry.Dbcr = (short)-1;

                if (Voucher != null && !Voucher.JournalEntries.Contains(RevEntry))
                    Voucher.JournalEntries.Add(RevEntry);

                if (TaxEntry == null) TaxEntry = new JournalEntryEntity();
                TaxEntry.AccountId = TaxAccountId;
                TaxEntry.Dbcr = (short)-1;

                if (Voucher != null && !Voucher.JournalEntries.Contains(TaxEntry))
                    Voucher.JournalEntries.Add(TaxEntry);

                RevEntry.ParentEntry = RevMasterEntry;
                TaxEntry.ParentEntry = RevMasterEntry;

                RevMasterEntry.SubEntries.Clear();
                if (!RevMasterEntry.SubEntries.Contains(RevEntry))
                    RevMasterEntry.SubEntries.Add(RevEntry);

                if (HasTax && !RevMasterEntry.SubEntries.Contains(TaxEntry))
                    RevMasterEntry.SubEntries.Add(TaxEntry);
            }

            if (MaterialEntry != null) { MaterialEntry.UpdateFilter = true; }
            if (PayMasterEntry != null) { PayMasterEntry.UpdateFilter = true; }
            if (CostMasterEntry != null) { CostMasterEntry.UpdateFilter = true; }
            if (RevMasterEntry != null) { RevMasterEntry.UpdateFilter = true; }
            if (RevEntry != null) { RevEntry.UpdateFilter = true; }
            if (TaxEntry != null) { TaxEntry.UpdateFilter = true; }

            OnPropertyChanged(nameof(TotalEntry));
            OnPropertyChanged(nameof(CostMasterEntry));
            OnPropertyChanged(nameof(RevEntry));
            OnPropertyChanged(nameof(TaxEntry));
            OnPropertyChanged(nameof(MaterialEntry));
        }

        /// <summary>
        /// Dọn dẹp cấu trúc mua bán khi chuyển về NỘI BỘ (NNB/XNB)
        /// Gỡ các entity phụ khỏi Voucher.JournalEntries
        /// </summary>
        public void TearDownTradeHierarchy()
        {
            if (InventoryVoucher != null)
            {
                InventoryVoucher.Taxflag = false;
            }

            // Gỡ MaterialEntry khỏi cha
            MaterialEntry.ParentEntry = null;
            MaterialEntry.SubEntries.Clear();
            MaterialEntry.Dbcr = (short)(VoucherType == "NNB" ? 1 : -1);

            // Gỡ các entity phụ khỏi Voucher.JournalEntries
            if (Voucher != null)
            {
                if (PayMasterEntry != null) Voucher.JournalEntries.Remove(PayMasterEntry);
                if (TaxEntry != null) Voucher.JournalEntries.Remove(TaxEntry);
                if (CostMasterEntry != null) Voucher.JournalEntries.Remove(CostMasterEntry);
                if (RevMasterEntry != null) Voucher.JournalEntries.Remove(RevMasterEntry);
                if (RevEntry != null) Voucher.JournalEntries.Remove(RevEntry);
            }

            PayMasterEntry = null;
            TaxEntry = null;
            CostMasterEntry = null;
            RevMasterEntry = null;
            RevEntry = null;
        }

        /// <summary>
        /// Dọn dẹp khi người dùng bấm Bỏ qua (Cancel) tạo mới
        /// </summary>
        public void Discard()
        {
            TearDownTradeHierarchy();
            if (IsNew && Voucher != null)
            {
                if (MaterialEntry != null)
                    Voucher.JournalEntries.Remove(MaterialEntry);
            }
        }

        /// <summary>
        /// Tính toán tổng tiền từ danh sách chi tiết của TOÀN BỘ các phiếu kho trong MaterialEntry.InventoryVouchers
        /// VÀ DOMINO CẬP NHẬT VÀO CÁC ENTITY
        /// </summary>
        public void RecalculateTotals()
        {
            if (_isRecalculatingTotals) return;
            try
            {
                _isRecalculatingTotals = true;

                CurrentVoucherCostAmount = InventoryVoucher?.TotalCostAmount ?? 0;

                var allVouchers = (MaterialEntry?.InventoryVouchers != null && MaterialEntry.InventoryVouchers.Count > 0)
                    ? MaterialEntry.InventoryVouchers.ToList()
                    : (InventoryVoucher != null ? new List<InventoryVoucherEntity> { InventoryVoucher } : new List<InventoryVoucherEntity>());

                double cost = 0;
                double sell = 0;
                double tax = 0;

                foreach (var iv in allVouchers)
                {
                    if (iv == null) continue;
                    cost += iv.TotalCostAmount;

                    bool ivHasTax = iv.Taxflag || HasTax;
                    if (ivHasTax)
                    {
                        if (VoucherType == "XB")
                        {
                            sell += iv.TotalSellAmount;
                            tax += iv.TotalTaxAmount;
                        }
                        else if (VoucherType == "NM")
                        {
                            tax += iv.TotalTaxAmount;
                        }
                    }
                }

                cost = Math.Round(cost, 0);
                sell = Math.Round(sell, 0);
                tax = Math.Round(tax, 0);

                TotalCostAmount = cost;
                TotalSellAmount = sell;
                TotalTaxAmount = tax;

                if (VoucherType == "XB")
                    TotalAllAmount = sell + tax;
                else if (VoucherType == "NM")
                    TotalAllAmount = cost + tax;
                else
                    TotalAllAmount = cost;

                // ==================== DOMINO CASCADING ====================
                // 1. Dòng vật tư nhận tổng tiền hàng của tất cả các phiếu kho thuộc nó (đồng bộ từ ORM)
                if (MaterialEntry != null && (!MaterialEntry.Amount.HasValue || Math.Abs(MaterialEntry.Amount.Value - cost) > 0.001))
                {
                    MaterialEntry.Amount = cost;
                }

                // 2. Nếu là Mua bán, domino gọi đến các Entity con và cha
                if (IsTrade)
                {
                    if (VoucherType == "NM")
                    {
                        if (TaxEntry != null)
                        {
                            TaxEntry.AccountId = TaxAccountId;
                            TaxEntry.Amount = tax;
                        }

                        if (PayMasterEntry != null)
                        {
                            PayMasterEntry.AccountId = TotalAccountId;
                            PayMasterEntry.Amount = cost + (HasTax ? tax : 0);
                        }
                    }
                    else if (VoucherType == "XB")
                    {
                        if (CostMasterEntry != null)
                        {
                            CostMasterEntry.AccountId = CostAccountId;
                            CostMasterEntry.Amount = cost;
                        }

                        if (RevEntry != null)
                        {
                            RevEntry.AccountId = RevenueAccountId;
                            RevEntry.Amount = sell;
                        }

                        if (TaxEntry != null)
                        {
                            TaxEntry.AccountId = TaxAccountId;
                            TaxEntry.Amount = tax;
                        }

                        if (RevMasterEntry != null)
                        {
                            RevMasterEntry.AccountId = TotalAccountId;
                            RevMasterEntry.Amount = sell + (HasTax ? tax : 0);
                        }
                    }
                }
            }
            finally
            {
                _isRecalculatingTotals = false;
            }
        }

        /// <summary>
        /// Nút Chấp nhận chỉ gọi hàm này để hoàn tất thông tin chứng từ.
        /// Toàn bộ số tiền đã được domino đồng bộ trong suốt quá trình nhập liệu.
        /// </summary>
        public void Apply()
        {
            RecalculateTotals();

            if (InventoryVoucher != null)
            {
                InventoryVoucher.Taxflag = HasTax;
                InventoryVoucher.WarehouseId = WarehouseId;
            }

            if (IsTrade && VoucherType == "NM")
            {
                if (PayMasterEntry != null)
                {
                    MaterialEntry.ParentEntry = PayMasterEntry;
                    if (PayMasterEntry.Id != 0) MaterialEntry.ParentId = PayMasterEntry.Id;
                    if (!PayMasterEntry.SubEntries.Contains(MaterialEntry))
                        PayMasterEntry.SubEntries.Add(MaterialEntry);

                    if (HasTax && TotalTaxAmount > 0)
                    {
                        if (TaxEntry != null)
                        {
                            TaxEntry.ParentEntry = PayMasterEntry;
                            if (PayMasterEntry.Id != 0) TaxEntry.ParentId = PayMasterEntry.Id;
                            if (!PayMasterEntry.SubEntries.Contains(TaxEntry))
                                PayMasterEntry.SubEntries.Add(TaxEntry);
                        }
                    }
                    else
                    {
                        // Nếu không có thuế, gỡ TaxEntry khỏi Voucher
                        if (Voucher != null && TaxEntry != null)
                            Voucher.JournalEntries.Remove(TaxEntry);
                    }
                }
            }
            else if (IsTrade && VoucherType == "XB")
            {
                if (CostMasterEntry != null)
                {
                    MaterialEntry.ParentEntry = CostMasterEntry;
                    if (CostMasterEntry.Id != 0) MaterialEntry.ParentId = CostMasterEntry.Id;
                    if (!CostMasterEntry.SubEntries.Contains(MaterialEntry))
                        CostMasterEntry.SubEntries.Add(MaterialEntry);
                }

                if (RevMasterEntry != null)
                {
                    if (RevEntry != null)
                    {
                        RevEntry.ParentEntry = RevMasterEntry;
                        if (RevMasterEntry.Id != 0) RevEntry.ParentId = RevMasterEntry.Id;
                        if (!RevMasterEntry.SubEntries.Contains(RevEntry))
                            RevMasterEntry.SubEntries.Add(RevEntry);
                    }

                    if (HasTax && TotalTaxAmount > 0)
                    {
                        if (TaxEntry != null)
                        {
                            TaxEntry.ParentEntry = RevMasterEntry;
                            if (RevMasterEntry.Id != 0) TaxEntry.ParentId = RevMasterEntry.Id;
                            if (!RevMasterEntry.SubEntries.Contains(TaxEntry))
                                RevMasterEntry.SubEntries.Add(TaxEntry);
                        }
                    }
                    else
                    {
                        if (Voucher != null && TaxEntry != null)
                            Voucher.JournalEntries.Remove(TaxEntry);
                    }
                }
            }

            // Đồng bộ UpdateFilter và chi tiết công nợ cho các dòng giao dịch vật tư
            if (MaterialEntry != null)
            {
                MaterialEntry.IsMaterialBound = true;
                MaterialEntry.UpdateFilter = true;
            }

            if (IsTrade)
            {
                if (VoucherType == "NM" && PayMasterEntry != null)
                {
                    PayMasterEntry.IsMaterialBound = true;
                    PayMasterEntry.UpdateFilter = true;
                    if (TaxEntry != null) { TaxEntry.UpdateFilter = true; }
                    if (!string.IsNullOrEmpty(InventoryVoucher?.Vatobjectid))
                    {
                        var debt = PayMasterEntry.DebtDetails.FirstOrDefault();
                        if (debt == null)
                        {
                            debt = new DebtDetailEntity();
                            PayMasterEntry.DebtDetails.Add(debt);
                        }
                        debt.PartnerId = InventoryVoucher.Vatobjectid;
                        debt.Amount = TotalAllAmount;
                        debt.Liabilitydate = Voucher?.VoucherDate ?? DateTime.Today;
                    }
                }
                else if (VoucherType == "XB")
                {
                    if (CostMasterEntry != null) { CostMasterEntry.IsMaterialBound = true; CostMasterEntry.UpdateFilter = true; }
                    if (RevEntry != null) { RevEntry.IsMaterialBound = true; RevEntry.UpdateFilter = true; }
                    if (TaxEntry != null) { TaxEntry.UpdateFilter = true; }
                    if (RevMasterEntry != null)
                    {
                        RevMasterEntry.IsMaterialBound = true;
                        RevMasterEntry.UpdateFilter = true;
                        if (!string.IsNullOrEmpty(InventoryVoucher?.Vatobjectid))
                        {
                            var debt = RevMasterEntry.DebtDetails.FirstOrDefault();
                            if (debt == null)
                            {
                                debt = new DebtDetailEntity();
                                RevMasterEntry.DebtDetails.Add(debt);
                            }
                            debt.PartnerId = InventoryVoucher.Vatobjectid;
                            debt.Amount = TotalAllAmount;
                            debt.Liabilitydate = Voucher?.VoucherDate ?? DateTime.Today;
                        }
                    }
                }
            }
        }
    }
}
