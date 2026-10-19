using System;
using DataAccess;
using DataAccess.EntityClasses;
using DataAccess.HelperClasses;
using SD.LLBLGen.Pro.ORMSupportClasses;

namespace NewaccNet.Wpf.AppSystem.CostAllocation
{
    /// <summary>
    /// MẢNH GHÉP 4: Sinh chứng từ kế toán phân bổ chi phí (Distribute_Click trong vba-reference.cls).
    /// Cấu trúc chứng từ sinh ra:
    ///   - 1 JournalVoucher (Ngày chứng từ = Đến ngày, Bookflag = true)
    ///   - Bút toán 1 (Ghi Có TK Nguồn, Dbcr = -1): Các dòng CostDetail lấy từ Lưới trên (SourcePoolItems)
    ///     với cờ Transflag = true (để triệt tiêu số dư nguồn về 0 và đánh dấu bút toán tự động phân bổ).
    ///   - Bút toán 2 (Ghi Nợ TK Đích, Dbcr = +1, ParentEntry = creditEntry): Các dòng CostDetail lấy từ
    ///     Lưới dưới (CriteriaItems) gắn với Yếu tố đích (DestinationCostElementId) và Transflag = false.
    /// </summary>
    public class CostAllocationVoucherBuilder
    {
        private const double Epsilon = 0.0001;

        public JournalVoucherEntity CreateAllocationVoucher(
            CostAllocationParams param,
            CostAllocationCalculationResult calculation)
        {
            using (var adapter = AppDataAccessAdapter.Create())
            {
                // 1. Tạo Chứng từ phân bổ (Bills / JournalVoucher)
                var voucher = new JournalVoucherEntity
                {
                    VoucherDate = param.ToDate.Date,
                    VoucherNo = GenerateNextAllocationVoucherNo(adapter, param.ToDate.Date),
                    Contents = $"Phân bổ chi phí từ TK {param.SourceAccountId.Trim()} sang TK {param.DestinationAccountId.Trim()}",
                    Bookflag = true
                };

                // 2. Bút toán 1: Ghi Có giảm trừ Tài khoản nguồn (DBCR = -1)
                var creditEntry = new JournalEntryEntity
                {
                    AccountId = param.SourceAccountId.Trim(),
                    Dbcr = -1,
                    Amount = calculation.TotalPoolAmount,
                    Createdate = DateTime.Now
                };

                foreach (var srcItem in calculation.SourcePoolItems)
                {
                    var cd = new CostDetailEntity
                    {
                        CostObjectId = srcItem.CostObjectId,
                        CostElementId = srcItem.CostElementId,
                        Amount = srcItem.Amount,
                        // Đánh dấu bút toán kết chuyển/phân bổ tự động trên dòng giảm trừ TK nguồn (vba-reference.cls dòng 83)
                        Transflag = true
                    };
                    creditEntry.CostDetails.Add(cd);
                }
                voucher.JournalEntries.Add(creditEntry);

                // 3. Bút toán 2: Ghi Nợ ghi nhận vào Tài khoản đích theo từng Đối tượng (DBCR = +1)
                var debitEntry = new JournalEntryEntity
                {
                    AccountId = param.DestinationAccountId.Trim(),
                    Dbcr = 1,
                    Amount = calculation.TotalPoolAmount,
                    ParentEntry = creditEntry,
                    Createdate = DateTime.Now
                };

                string desCostElem = param.DestinationCostElementId?.Trim() ?? string.Empty;
                foreach (var critItem in calculation.CriteriaItems)
                {
                    if (Math.Abs(critItem.AllocatedAmount) < Epsilon) continue;

                    var cd = new CostDetailEntity
                    {
                        CostObjectId = critItem.CostObjectId,
                        CostElementId = desCostElem,
                        Amount = critItem.AllocatedAmount,
                        // Chi phí ghi nhận vào đối tượng đích là chi phí thực tế (vba-reference.cls dòng 94-99: không gán TransFlag = True)
                        Transflag = false
                    };
                    debitEntry.CostDetails.Add(cd);
                }
                voucher.JournalEntries.Add(debitEntry);

                // 4. Lưu toàn bộ trong một transaction thống nhất
                adapter.SaveEntity(voucher, true, recurse: true);

                return voucher;
            }
        }

        private string GenerateNextAllocationVoucherNo(IDataAccessAdapter adapter, DateTime voucherDate)
        {
            try
            {
                string prefix = "PB" + voucherDate.ToString("yyMM");
                var bucket = new RelationPredicateBucket(JournalVoucherFields.VoucherNo % (prefix + "%"));
                var existingVouchers = new EntityCollection<JournalVoucherEntity>();
                adapter.FetchEntityCollection(existingVouchers, bucket);

                int maxNum = 0;
                foreach (var v in existingVouchers)
                {
                    string no = v.VoucherNo ?? "";
                    if (no.StartsWith(prefix) && no.Length > prefix.Length)
                    {
                        string suffix = no.Substring(prefix.Length);
                        if (int.TryParse(suffix, out int n) && n > maxNum)
                        {
                            maxNum = n;
                        }
                    }
                }
                return $"{prefix}{(maxNum + 1):D3}";
            }
            catch
            {
                return $"PB{voucherDate:yyMMdd}01";
            }
        }
    }
}
