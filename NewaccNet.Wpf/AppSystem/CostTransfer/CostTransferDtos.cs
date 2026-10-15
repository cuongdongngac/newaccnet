using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace NewaccNet.Wpf.AppSystem.CostTransfer
{
    /// <summary>
    /// Một dòng trên lưới kết chuyển chi phí. Chỉ tồn tại trong bộ nhớ (không có bảng tạm trong DB).
    /// Quy tắc dựng dữ liệu: xem .ai_context/FlatTableRules.md
    /// </summary>
    public class CostTransferRow : INotifyPropertyChanged
    {
        private bool _selected;
        private double? _keep;

        public string AccountId { get; set; } = string.Empty;

        public string CostObjectId { get; set; } = string.Empty;
        public string CostObjectName { get; set; } = string.Empty;

        public string CostElementId { get; set; } = string.Empty;
        public string CostElementName { get; set; } = string.Empty;

        /// <summary>Số dư chi phí = Σ(Dbcr × Amount) đến "Đến ngày".</summary>
        public double Rest { get; set; }

        /// <summary>Cột KC: dòng được chọn để kết chuyển.</summary>
        public bool Selected
        {
            get => _selected;
            set { if (_selected != value) { _selected = value; OnPropertyChanged(); } }
        }

        /// <summary>Chi phí để lại (người dùng nhập). Phần còn lại sẽ được kết chuyển.</summary>
        public double? Keep
        {
            get => _keep;
            set
            {
                if (_keep != value)
                {
                    _keep = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(TransferAmount));
                }
            }
        }

        /// <summary>Số sẽ kết chuyển = Số dư - Chi phí để lại.</summary>
        public double TransferAmount => Rest - (Keep ?? 0);

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    /// <summary>
    /// Tham số đầu vào cho tính toán và kết chuyển chi phí.
    /// </summary>
    public class CostTransferParams
    {
        public System.DateTime EndDate { get; set; } = System.DateTime.Today;
        public string SourceAccountId { get; set; } = string.Empty;
        public string DestinationAccountId { get; set; } = string.Empty;
        public string Contents { get; set; } = string.Empty;
    }
}
