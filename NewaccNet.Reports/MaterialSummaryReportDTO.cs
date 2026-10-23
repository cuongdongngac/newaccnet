using DevExpress.DataAccess.ObjectBinding;
using System;
using System.Collections.Generic;
using NewaccNet.Reports.FlattenedDetails;

namespace NewaccNet.Reports
{
    /// <summary>
    /// Data Transfer Object cho Báo cáo Tổng hợp Vật tư - Bảng Nhập - Xuất - Tồn.
    /// Sử dụng tiền tố Begin (Đầu kỳ) và End (Cuối kỳ) đồng bộ chuẩn kế toán.
    /// </summary>
    [HighlightedClass]
    public class MaterialSummaryReportDTO
    {
        public MaterialSummaryReportDTO()
        {
        }

        public string WarehouseId { get; set; } = string.Empty;
        public string WarehouseName { get; set; } = string.Empty;

        public string MaterialId { get; set; } = string.Empty;
        public string MaterialName { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;

        // TỒN ĐẦU KỲ
        public decimal BeginQty { get; set; }
        public decimal BeginAmount { get; set; }

        // NHẬP TRONG KỲ
        public decimal InQty { get; set; }
        public decimal InAmount { get; set; }

        // XUẤT TRONG KỲ
        public decimal OutQty { get; set; }
        public decimal OutAmount { get; set; }

        // TỒN CUỐI KỲ
        public decimal EndQty { get; set; }
        public decimal EndAmount { get; set; }

        /// <summary>
        /// Danh sách các dòng phẳng phát sinh trong kỳ của mặt hàng này tại kho này.
        /// Phục vụ Drill-down tức thời mà không cần query lại CSDL.
        /// </summary>
        public List<InventoryFlatRow> Details { get; set; } = new();
    }
}
