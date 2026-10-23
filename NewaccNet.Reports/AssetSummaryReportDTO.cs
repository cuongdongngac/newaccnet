using DevExpress.DataAccess.ObjectBinding;
using System;
using System.Collections.Generic;
using NewaccNet.Reports.FlattenedDetails;

namespace NewaccNet.Reports
{
    /// <summary>
    /// Data Transfer Object cho Báo cáo Tổng hợp Tài sản cố định (Bảng Tăng - Giảm - Tồn TSCĐ).
    /// </summary>
    [HighlightedClass]
    public class AssetSummaryReportDTO
    {
        public AssetSummaryReportDTO()
        {
        }

        public string AssetId { get; set; } = string.Empty;
        public string AssetName { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;

        // DƯ ĐẦU KỲ
        public decimal BeginQty { get; set; }
        public decimal BeginAmount { get; set; }

        // TĂNG TRONG KỲ (PHÁT SINH NỢ)
        public decimal InQty { get; set; }
        public decimal InAmount { get; set; }

        // GIẢM TRONG KỲ (PHÁT SINH CÓ)
        public decimal OutQty { get; set; }
        public decimal OutAmount { get; set; }

        // DƯ CUỐI KỲ
        public decimal EndQty { get; set; }
        public decimal EndAmount { get; set; }

        /// <summary>
        /// Danh sách các dòng phẳng phát sinh trong kỳ của tài sản này.
        /// Phục vụ Drill-down tức thời mở Sổ chi tiết TSCĐ mà không cần query lại CSDL.
        /// </summary>
        public List<AssetFlatRow> Details { get; set; } = new();
    }
}
