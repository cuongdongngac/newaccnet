using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace NewaccNet.Wpf.AppSystem.AssetDepreciation
{
    // MANH 1 - DU LIEU: cac lop chung de cac manh khac trao doi voi nhau.
    // Sua cot/thuoc tinh o day, cac manh khac chi doc.

    /// <summary>
    /// Anh chup moi nhat cua mot tai san theo (AssetId, SourceCode), ket hop giua AssetTrackingDep va bang phang AssetDetail.
    /// </summary>
    public class AssetSnapshot
    {
        public string AssetId { get; set; } = string.Empty;
        public string AssetName { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
        public string SourceCode { get; set; } = string.Empty;

        /// <summary>So luong tai san hien con (RestQty).</summary>
        public double Quantity { get; set; }

        /// <summary>Gia tinh khau hao 1 don vi tai san (AssetTrackingDep.DepPrice).</summary>
        public double OriginalValue { get; set; }

        /// <summary>Hao mon luy ke ban dau khai bao tren AssetTrackingDep (RemainValue).</summary>
        public double InitialAccumulatedDep { get; set; }

        /// <summary>Tri gia con lai (Rest) tinh tu bang phang AssetDetail den EndDate.</summary>
        public double RestValue { get; set; }

        /// <summary>Ngay dua vao su dung (Asset.Date).</summary>
        public DateTime PurchaseDate { get; set; }

        /// <summary>Thoi gian su dung (nam) - quy doi tu Asset.TimeUsing hoac AssetTracking.Timeusing.</summary>
        public double UsingYears { get; set; }

        /// <summary>Ty le khau hao nam (AssetTrackingDep.DepRate).</summary>
        public double DepRate { get; set; }

        /// <summary>Tai khoan tai san / khau hao nguon (AssetTrackingDep.AsaccountId).</summary>
        public string AssetAccountId { get; set; } = string.Empty;

        /// <summary>Tai khoan chi phi dich (AssetTrackingDep.ExaccountId).</summary>
        public string ExpenseAccountId { get; set; } = string.Empty;

        /// <summary>Doi tuong chi phi dich (AssetTrackingDep.ExobjectId).</summary>
        public string CostObjectId { get; set; } = string.Empty;

        public string DepartmentId { get; set; } = string.Empty;
    }

    /// <summary>
    /// Mot dong tren luoi khau hao (tuong ung Depreciation SubForm trong VBA). Chi ton tai trong bo nho.
    /// </summary>
    public class AssetDepreciationRow : INotifyPropertyChanged
    {
        private bool _selected = true;
        private double _unitDepAmount;
        private double _depAmount;

        public AssetSnapshot Snapshot { get; set; } = new AssetSnapshot();

        public string AssetId => Snapshot.AssetId;
        public string AssetName => Snapshot.AssetName;
        public string Unit => Snapshot.Unit;
        public string SourceCode => Snapshot.SourceCode;
        public double Quantity => Snapshot.Quantity;
        public double RestValue => Snapshot.RestValue;
        public string AssetAccountId => Snapshot.AssetAccountId;
        public string ExpenseAccountId => Snapshot.ExpenseAccountId;
        public string CostObjectId => Snapshot.CostObjectId;

        /// <summary>Cot KH: chon dong de trich khau hao (Flag trong VBA).</summary>
        public bool Selected
        {
            get => _selected;
            set { if (_selected != value) { _selected = value; OnPropertyChanged(); } }
        }

        /// <summary>Muc khau hao cua 1 don vi tai san (Amount trong VBA).</summary>
        public double UnitDepAmount
        {
            get => _unitDepAmount;
            set
            {
                if (Math.Abs(_unitDepAmount - value) > 0.0001)
                {
                    _unitDepAmount = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>So tien khau hao = UnitDepAmount * Quantity (kẹp <= RestValue).</summary>
        public double DepAmount
        {
            get => _depAmount;
            set
            {
                if (Math.Abs(_depAmount - value) > 0.0001)
                {
                    _depAmount = value;
                    if (Quantity > 0.0001)
                    {
                        _unitDepAmount = Math.Round(_depAmount / Quantity, 0, MidpointRounding.AwayFromZero);
                        OnPropertyChanged(nameof(UnitDepAmount));
                    }
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    /// <summary>
    /// Tham so dau vao cua tinh toan va ghi so khau hao.
    /// </summary>
    public class AssetDepreciationParams
    {
        public DateTime BeginDate { get; set; } = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        public DateTime EndDate { get; set; } = DateTime.Today;

        /// <summary>Tai khoan khau hao nguon tren form (SrAccountID - ghi Co).</summary>
        public string DepAccountId { get; set; } = string.Empty;

        /// <summary>Ma nguon von tren form (SrCode).</summary>
        public string SourceCode { get; set; } = string.Empty;

        /// <summary>Ma yeu to chi phi tren form (ExAtribute ID).</summary>
        public string CostElementId { get; set; } = string.Empty;

        /// <summary>Ten yeu to chi phi (dung lam Contents mac dinh neu Contents de trong).</summary>
        public string CostElementName { get; set; } = string.Empty;

        /// <summary>Ma ly do tren form (ReasonID).</summary>
        public string ReasonId { get; set; } = string.Empty;

        /// <summary>Noi dung chung tu (Contents).</summary>
        public string Contents { get; set; } = string.Empty;
    }
}
