using DevExpress.DataAccess.ObjectBinding;
using System;

namespace NewaccNet.Reports.BalanceSheet
{
    /// <summary>
    /// DTO phẳng cho Báo cáo Cân đối kế toán (Đã map theo đúng cấu trúc Entity mới)
    /// </summary>
    [HighlightedClass]
    public class BalanceSheetReportDto
    {

        public BalanceSheetReportDto()
        {
            
        }

        // =========================================================
        // TẦNG 1 (GROUP LEVEL 1) - Tương đương Entity [ReportSection]
        // =========================================================
        public int SectionId { get; set; }          
        public string SectionName { get; set; }     // VD: TÀI SẢN
        public bool InDebit { get; set; }           // Đảo dấu lúc tính số dư

        // =========================================================
        // TẦNG 2 (GROUP LEVEL 2) - Tương đương Entity [ReportCategory]
        // =========================================================
        public int CategoryId { get; set; }     
        public string CategoryCode { get; set; }    
        public string CategoryName { get; set; }    // VD: A. TÀI SẢN NGẮN HẠN

        // =========================================================
        // TẦNG 3 (GROUP LEVEL 3) - Tương đương Entity [ReportItem]
        // =========================================================
        public int ItemId { get; set; }            
        public string ItemCode { get; set; }       
        public string EntryName { get; set; }       // VD: I. Tiền và các khoản tương đương tiền

        // =========================================================
        // TẦNG 4 (DETAIL TIER) - Tương đương Entity [ReportFormula] (Nấc cuối)
        // =========================================================
        public int FormulaId { get; set; }          
        public string FormulaCode { get; set; }     // Mã số trên báo cáo (VD: 111, 112)
        public string ItemsName { get; set; }       // Tên chỉ tiêu báo cáo (VD: 1. Tiền)
        
        public string Illu { get; set; }            // Thuyết minh
        public int Order { get; set; }              // Thứ tự sắp xếp
        public bool Calculate { get; set; }         // Đánh dấu có cần tính toán hay không
        public bool Print { get; set; }             // Đánh dấu có in lên báo cáo không

        // =========================================================
        // SỐ LIỆU TÀI CHÍNH (Sau khi đã Lookup từ TrialBalance / DebtBalance)
        // =========================================================
        public string AccountId { get; set; }       // TÀI KHOẢN (Chi tiết để Drill-Down)
        public decimal BeginAmount { get; set; }    // Số Đầu kỳ
        public decimal EndAmount { get; set; }      // Số Cuối kỳ
    }
}

