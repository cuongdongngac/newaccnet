using DevExpress.DataAccess.ObjectBinding;
using System;

namespace NewaccNet.Reports
{
    /// <summary>
    /// Data Transfer Object cho Báo cáo Chi tiết Công nợ thuần tài chính (DeptDetailReport).
    /// Đã lược bỏ các trường Thời hạn nợ (Due) và Ngoại tệ (Currency) để tránh dữ liệu bị trống.
    /// Các báo cáo Thời hạn nợ và Ngoại tệ sẽ dùng DTO riêng tương ứng với từng bảng phẳng.
    /// </summary>
    [HighlightedClass]
    public class DebtDetailReportDTO
    {
        // Constructor rỗng bắt buộc để DevExpress Report Designer nhận diện qua ObjectDataSource
        public DebtDetailReportDTO()
        {
        }

        /// <summary>
        /// ID chứng từ gốc (JournalVoucher.Id) dùng để mở ngược lại màn hình chứng từ (VoucherEntryWindow).
        /// </summary>
        public int? JournalVoucherId { get; set; }

        /// <summary>
        /// Mã Nhóm Cấp 1 (PartnerId nếu sắp xếp theo Đối tượng; DebtTypeId nếu sắp xếp theo Nội dung).
        /// </summary>
        public string Group1Id { get; set; } = string.Empty;

        /// <summary>
        /// Tên Nhóm Cấp 1 (Tên Đối tượng hoặc Tên Nội dung công nợ tùy chế độ sắp xếp).
        /// </summary>
        public string Group1Name { get; set; } = string.Empty;

        /// <summary>
        /// Mã Nhóm Cấp 2 (DebtTypeId nếu sắp xếp theo Đối tượng; PartnerId nếu sắp xếp theo Nội dung).
        /// </summary>
        public string Group2Id { get; set; } = string.Empty;

        /// <summary>
        /// Tên Nhóm Cấp 2 (Tên Nội dung hoặc Tên Đối tượng công nợ tùy chế độ sắp xếp).
        /// </summary>
        public string Group2Name { get; set; } = string.Empty;

        /// <summary>
        /// Số chứng từ ([Bill No] / Số CT).
        /// </summary>
        public string VoucherNo { get; set; } = string.Empty;

        /// <summary>
        /// Ngày chứng từ ([Bill Date] / Ngày CT).
        /// </summary>
        public DateTime? VoucherDate { get; set; }

        /// <summary>
        /// Nội dung / Diễn giải chứng từ (Contents / Nội dung).
        /// </summary>
        public string Contents { get; set; } = string.Empty;

        /// <summary>
        /// Phát sinh trong kỳ - Số tiền Nợ (Debit).
        /// </summary>
        public decimal Debit { get; set; }

        /// <summary>
        /// Phát sinh trong kỳ - Số tiền Có (Credit).
        /// </summary>
        public decimal Credit { get; set; }
    }
}
