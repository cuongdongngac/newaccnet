using DevExpress.DataAccess.ObjectBinding;
using System;

namespace NewaccNet.Reports
{
    /// <summary>
    /// Data Transfer Object cho Báo cáo Công nợ theo thời hạn.
    /// Cấu trúc tương tự DebtDetailReportDTO nhưng bổ sung thêm trường EndDate (Hạn thanh toán) lấy từ bảng LiabilityDue.
    /// </summary>
    [HighlightedClass]
    public class DebtDueReportDTO
    {
        // Constructor rỗng bắt buộc để DevExpress Report Designer nhận diện qua ObjectDataSource
        public DebtDueReportDTO()
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

        // =====================================================================
        // THÔNG TIN THỜI HẠN & LÃI SUẤT CÔNG NỢ (TỪ BẢNG LiabilityDue)
        // =====================================================================

        /// <summary>
        /// Mã khế ước / Hạn nợ (LiabilityDue.Id).
        /// </summary>
        public int LiabilityDuesId { get; set; }

        /// <summary>
        /// Mã đối tượng công nợ.
        /// </summary>
        public string PartnerId { get; set; } = string.Empty;

        /// <summary>
        /// Tên đối tượng công nợ.
        /// </summary>
        public string PartnerName { get; set; } = string.Empty;

        /// <summary>
        /// Mã nội dung / loại hình công nợ.
        /// </summary>
        public string DebtTypeId { get; set; } = string.Empty;

        /// <summary>
        /// Tên nội dung / loại hình công nợ.
        /// </summary>
        public string DebtTypeName { get; set; } = string.Empty;

        /// <summary>
        /// Từ ngày / Ngày bắt đầu khế ước (LiabilityDue.BeginDate).
        /// </summary>
        public DateTime? BeginDate { get; set; }

        /// <summary>
        /// Đến ngày / Hạn thanh toán (LiabilityDue.EndDate).
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Ghi chú / Số hợp đồng khế ước (LiabilityDue.Remark).
        /// </summary>
        public string Remark { get; set; } = string.Empty;

        /// <summary>
        /// Trạng thái đã tất toán (LiabilityDue.Settledflag).
        /// </summary>
        public bool? SettledFlag { get; set; }

        /// <summary>
        /// Lãi suất (%) (LiabilityDue.Interestrate).
        /// </summary>
        public double? InterestRate { get; set; }

        /// <summary>
        /// % Khấu trừ (LiabilityDue.Feedbackrate).
        /// </summary>
        public double? FeedbackRate { get; set; }

        /// <summary>
        /// VAT đầu ra (%) (LiabilityDue.Outvatrate).
        /// </summary>
        public double? OutVatRate { get; set; }

        // =====================================================================
        // SỐ TIỀN PHÁT SINH TRONG KỲ
        // =====================================================================

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
