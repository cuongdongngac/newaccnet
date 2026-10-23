using DevExpress.DataAccess.ObjectBinding;
using System;

namespace NewaccNet.Reports
{
    /// <summary>
    /// Data Transfer Object cho Báo cáo Tổng hợp Công nợ (SummaryDebtReport).
    /// Các số liệu tổng nhóm (Group) và tổng cộng báo cáo (Report) do XtraReport tự động tính qua sumSum(...).
    /// </summary>
    [HighlightedClass]
    public class DebtSummaryReportDTO
    {
        // Constructor rỗng bắt buộc để DevExpress Report Designer nhận diện qua ObjectDataSource
        public DebtSummaryReportDTO()
        {
        }

        // =====================================================================
        // 1. CÁC TRƯỜNG GOM NHÓM 2 CẤP ĐỘNG (ĐÁP ỨNG CẢ 2 CHẾ ĐỘ SẮP XẾP TRÊN CÙNG 1 MẪU REPORT)
        //    - Sắp xếp theo Đối tượng: Group1 = Partner (Đối tượng), Group2 = DebtType (Nội dung)
        //    - Sắp xếp theo Nội dung:  Group1 = DebtType (Nội dung), Group2 = Partner (Đối tượng)
        // =====================================================================

        /// <summary>
        /// Mã Nhóm Cấp 1 (PartnerId nếu sắp xếp theo Đối tượng; DebtTypeId nếu sắp xếp theo Nội dung).
        /// Khi Drill-down từ Group1 trên báo cáo tổng hợp sang báo cáo chi tiết, dùng mã này để lọc thứ cấp trên Flat rows.
        /// </summary>
        public string Group1Id { get; set; } = string.Empty;

        /// <summary>
        /// Tên Nhóm Cấp 1 (Tên Đối tượng hoặc Tên Nội dung công nợ tùy chế độ sắp xếp).
        /// </summary>
        public string Group1Name { get; set; } = string.Empty;

        /// <summary>
        /// Mã Chi tiết Cấp 2 (DebtTypeId nếu sắp xếp theo Đối tượng; PartnerId nếu sắp xếp theo Nội dung).
        /// </summary>
        public string Group2Id { get; set; } = string.Empty;

        /// <summary>
        /// Tên Chi tiết Cấp 2 (Tên Nội dung hoặc Tên Đối tượng công nợ tùy chế độ sắp xếp).
        /// </summary>
        public string Group2Name { get; set; } = string.Empty;

        // =====================================================================
        // 2. THÔNG TIN ĐỊNH DANH GỐC (TƯƠNG ỨNG OPTION CUSTOMER SUMMARY QUERY)
        // =====================================================================

        /// <summary>
        /// Mã đối tượng công nợ / Khách hàng / Nhà cung cấp ([Customer ID]).
        /// </summary>
        public string PartnerId { get; set; } = string.Empty;

        /// <summary>
        /// Tên đối tượng công nợ ([Customer Name]).
        /// </summary>
        public string PartnerName { get; set; } = string.Empty;

        /// <summary>
        /// Mã nội dung / loại hình công nợ ([Liability Things ID]).
        /// </summary>
        public string DebtTypeId { get; set; } = string.Empty;

        /// <summary>
        /// Tên nội dung / loại hình công nợ ([Liability Thing Name]).
        /// </summary>
        public string DebtTypeName { get; set; } = string.Empty;

        // =====================================================================
        // 3. SỐ LIỆU 6 CỘT TIỀN TRÊN DÒNG CHI TIẾT (DETAIL BAND)
        //    Các số tổng cộng theo Group1Id / Group2Id / Report được XtraReport tự cộng bằng sumSum(...)
        // =====================================================================

        /// <summary>
        /// Dư đầu kỳ - Nợ (BeginDebit).
        /// </summary>
        public decimal BeginDebit { get; set; }

        /// <summary>
        /// Dư đầu kỳ - Có (BeginCredit).
        /// </summary>
        public decimal BeginCredit { get; set; }

        /// <summary>
        /// Phát sinh trong kỳ - Nợ (IntDebit).
        /// </summary>
        public decimal IntDebit { get; set; }

        /// <summary>
        /// Phát sinh trong kỳ - Có (IntCredit).
        /// </summary>
        public decimal IntCredit { get; set; }

        /// <summary>
        /// Dư cuối kỳ - Nợ (EndDebit = DebitValue(BeginDebit - BeginCredit + IntDebit - IntCredit)).
        /// </summary>
        public decimal EndDebit { get; set; }

        /// <summary>
        /// Dư cuối kỳ - Có (EndCredit = CreditValue(BeginDebit - BeginCredit + IntDebit - IntCredit)).
        /// </summary>
        public decimal EndCredit { get; set; }
    }
}
