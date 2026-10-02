using System.Collections.Generic;
using System.Linq;
using DataAccess.TypedListClasses;

namespace NewaccNet.Reports
{
    /// <summary>
    /// Wrapper/Calculator để chuẩn bị dữ liệu cho Sổ nhật ký.
    /// Nhận TypedList/Row từ Database và đóng gói thành DTO.
    /// </summary>
    public class DiaryCalculator
    {
        public List<DiaryReportDTO> Calculate(IEnumerable<DiaryRow> dataSource)
        {
            if (dataSource == null) return new List<DiaryReportDTO>();

            return dataSource.Select(row => new DiaryReportDTO
            {
                JournalVoucherId = row.JournalVoucherId ?? 0,
                VoucherNo = row.VoucherNo,
                VoucherDate = row.VoucherDate ?? System.DateTime.MinValue,
                Contents = row.Contents,
                AccountName = row.AccountName,
                DebitAccount = row.DebitAccount,
                CreditAccount = row.CreditAccount,
                Debit = row.Debit,
                Credit = row.Credit
            }).ToList();
        }
    }
}
