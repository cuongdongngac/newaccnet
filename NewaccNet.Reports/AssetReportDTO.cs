using System;

namespace NewaccNet.Reports
{
    /// <summary>
    /// DTO dòng Báo cáo Biến động Tài sản Cố định (Asset Movement / Detail Report).
    /// Hỗ trợ cấu trúc Grouping 2 cấp:
    ///   - Group 1: Lý do tăng/giảm (ReasonId, ReasonName, Flag).
    ///   - Group 2: Tài sản cố định (AssetId, AssetName, OriginalPrice...).
    ///   - Detail: Các chứng từ / bút toán phát sinh trong kỳ.
    /// </summary>
    public class AssetMovementReportDTO
    {
        // -------------------------------------------------------------
        // NHÓM 1: LÝ DO TĂNG GIẢM (Reason)
        // -------------------------------------------------------------
        public string ReasonId { get; set; } = string.Empty;
        public string ReasonName { get; set; } = string.Empty;
        public bool ReasonFlag { get; set; } // true: Tăng, false: Giảm / Hao mòn

        public string ReasonCategoryTitle => ReasonFlag
            ? "I. CÁC NGHIỆP VỤ TĂNG TÀI SẢN CỐ ĐỊNH"
            : "II. CÁC NGHIỆP VỤ GIẢM / HAO MÒN TÀI SẢN CỐ ĐỊNH";

        public string ReasonDisplayName => string.IsNullOrWhiteSpace(ReasonId)
            ? "(Không xác định lý do)"
            : $"{ReasonId} - {ReasonName}";

        // -------------------------------------------------------------
        // NHÓM 2: TÀI SẢN CỐ ĐỊNH (Asset)
        // -------------------------------------------------------------
        public string AssetId { get; set; } = string.Empty;
        public string AssetName { get; set; } = string.Empty;
        public string AssetDisplayName => $"{AssetId} - {AssetName}";
        public double? OriginalPrice { get; set; }
        public double? UsingTimeYears { get; set; }
        public DateTime? AssetStartDate { get; set; }
        public string DepartmentId { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
        public string SourceCode { get; set; } = string.Empty;

        // -------------------------------------------------------------
        // DETAIL: CHỨNG TỪ / BÚT TOÁN PHÁT SINH
        // -------------------------------------------------------------
        public int DetailId { get; set; }
        public int JournalEntryId { get; set; }
        public int? JournalVoucherId { get; set; }
        public string VoucherNo { get; set; } = string.Empty;
        public DateTime? VoucherDate { get; set; }
        public string Contents { get; set; } = string.Empty;
        public string AccountId { get; set; } = string.Empty;
        public string CounterAccountId { get; set; } = string.Empty;
        public int Dbcr { get; set; } = 1;

        public double Quantity { get; set; }
        public double Price { get; set; }
        public double Amount { get; set; }

        public double DebitAmount => Dbcr == 1 ? Amount : 0.0;
        public double CreditAmount => Dbcr == -1 ? Amount : 0.0;

        public bool IsOpeningRow { get; set; }
    }
}
