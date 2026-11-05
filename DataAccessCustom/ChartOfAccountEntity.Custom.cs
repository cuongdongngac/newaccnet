#nullable enable
using System;

namespace DataAccess.EntityClasses
{
    public partial class ChartOfAccountEntity
    {
        /// <summary>
        /// Tên nhóm phân loại chi tiết thân thiện (A: Công nợ, B: Vật tư, C: Ngoại tệ, D: TSCĐ, E: Chi phí, S: Cổ phiếu...)
        /// </summary>
        public string CategoryDescription => CategoryId?.Trim().ToUpper() switch
        {
            "A" => "A - Công nợ",
            "B" => "B - Vật tư, kho",
            "C" => "C - Ngoại tệ",
            "D" => "D - TSCĐ",
            "E" => "E - Chi phí",
            "S" => "S - Cổ phiếu",
            _ => "Thường"
        };

        /// <summary>
        /// Chuỗi hiển thị đầy đủ Mã + Tên tài khoản
        /// </summary>
        public string DisplayFull => $"{AccountId} - {AccountName}";
    }
}
