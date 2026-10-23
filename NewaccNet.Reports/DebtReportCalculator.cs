using System;
using System.Collections.Generic;
using System.Linq;
using DataAccess.EntityClasses;
using NewaccNet.Reports.FlattenedDetails;

namespace NewaccNet.Reports
{
    /// <summary>
    /// Cơ chế sắp xếp / Gom nhóm 2 cấp cho Báo cáo Công nợ.
    /// </summary>
    public enum DebtReportSortOrder
    {
        /// <summary>Cấp 1 = Đối tượng công nợ (Partner), Cấp 2 = Nội dung công nợ (DebtType)</summary>
        ByPartnerThenDebtType = 0,

        /// <summary>Cấp 1 = Nội dung công nợ (DebtType), Cấp 2 = Đối tượng công nợ (Partner)</summary>
        ByDebtTypeThenPartner = 1
    }

    /// <summary>
    /// Ngữ cảnh dữ liệu phẳng đã lọc từ Form Filter Công nợ.
    /// Giữ lại danh sách DebtFlatRow trên bộ nhớ để khi người dùng Drill-down từ Báo cáo Tổng hợp
    /// xuống Báo cáo Chi tiết theo Group1Id (hoặc Group1Id + Group2Id), hệ thống chỉ cần lọc thứ cấp (Where)
    /// trực tiếp trên FilteredFlatRows mà KHÔNG cần truy vấn lại CSDL hay tính toán lại từ đầu.
    /// </summary>
    public class DebtReportContext
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string AccountFilterDisplay { get; set; } = "*";
        public string AccountNameDisplay { get; set; } = "Toàn bộ tài khoản công nợ";
        public DebtReportSortOrder SortOrder { get; set; } = DebtReportSortOrder.ByPartnerThenDebtType;
        public string? CurrencyId { get; set; }

        /// <summary>
        /// Danh sách dòng phẳng công nợ đã lọc theo điều kiện ở Form Filter (Tài khoản cha/con, Đối tượng, Nội dung, Đến ngày).
        /// </summary>
        public List<DebtFlatRow> FilteredFlatRows { get; set; } = new List<DebtFlatRow>();

        /// <summary>
        /// Cho biết Group1Id hiện tại trên báo cáo đang đại diện cho Đối tượng công nợ (true) hay Nội dung công nợ (false).
        /// </summary>
        public bool IsGroup1Partner => SortOrder == DebtReportSortOrder.ByPartnerThenDebtType;

        /// <summary>
        /// Lọc thứ cấp trên tập Flat Rows có sẵn khi người dùng click Drill-down vào Group1Id (hoặc cả Group2Id) trên Báo cáo Tổng hợp.
        /// </summary>
        public List<DebtFlatRow> GetDrillDownFlatRows(string group1Id, string? group2Id = null)
        {
            if (string.IsNullOrWhiteSpace(group1Id))
                return FilteredFlatRows;

            string g1 = group1Id.Trim();
            string? g2 = string.IsNullOrWhiteSpace(group2Id) ? null : group2Id.Trim();

            IEnumerable<DebtFlatRow> query = FilteredFlatRows;

            if (SortOrder == DebtReportSortOrder.ByPartnerThenDebtType)
            {
                // Group1Id là Đối tượng (PartnerId), Group2Id (nếu có) là Nội dung (DebtTypeId)
                query = query.Where(r => string.Equals(r.PartnerId, g1, StringComparison.OrdinalIgnoreCase));
                if (g2 != null)
                {
                    query = query.Where(r => string.Equals(r.DebtTypeId, g2, StringComparison.OrdinalIgnoreCase));
                }
            }
            else
            {
                // Group1Id là Nội dung (DebtTypeId), Group2Id (nếu có) là Đối tượng (PartnerId)
                query = query.Where(r => string.Equals(r.DebtTypeId, g1, StringComparison.OrdinalIgnoreCase));
                if (g2 != null)
                {
                    query = query.Where(r => string.Equals(r.PartnerId, g2, StringComparison.OrdinalIgnoreCase));
                }
            }

            return query
                .OrderBy(r => r.VoucherDate ?? DateTime.MinValue)
                .ThenBy(r => r.VoucherNo)
                .ThenBy(r => r.DetailId)
                .ToList();
        }
    }

    /// <summary>
    /// Bộ tính toán số liệu cho Báo cáo Công nợ từ bảng phẳng DebtFlatRow.
    /// </summary>
    public static class DebtReportCalculator
    {
        /// <summary>
        /// Tạo ngữ cảnh dữ liệu phẳng (DebtReportContext) đã áp dụng bộ lọc từ Form Filter.
        /// </summary>
        public static DebtReportContext BuildContext(
            IEnumerable<DebtFlatRow> rawFlatRows,
            DateTime fromDate,
            DateTime toDate,
            HashSet<string>? matchingAccountIds,
            HashSet<string>? selectedPartnerIds,
            HashSet<string>? selectedDebtTypeIds,
            DebtReportSortOrder sortOrder,
            string accountFilterDisplay = "*",
            string accountNameDisplay = "Toàn bộ tài khoản công nợ",
            string? currencyId = null)
        {
            DateTime endDate = toDate.Date;

            IEnumerable<DebtFlatRow> query = rawFlatRows ?? Enumerable.Empty<DebtFlatRow>();

            // 1. Lọc đến ngày kết thúc kỳ báo cáo (Số dư đầu kỳ hoặc Bill Date <= EndDate)
            query = query.Where(r => !r.VoucherDate.HasValue || r.VoucherDate.Value.Date <= endDate);

            // 2. Lọc theo Tài khoản (bao gồm cả TK cha và toàn bộ các TK con)
            if (matchingAccountIds != null && matchingAccountIds.Count > 0)
            {
                query = query.Where(r => matchingAccountIds.Contains(r.AccountId?.Trim() ?? string.Empty));
            }

            // 3. Lọc theo Đối tượng công nợ (Partner)
            if (selectedPartnerIds != null && selectedPartnerIds.Count > 0)
            {
                query = query.Where(r => selectedPartnerIds.Contains(r.PartnerId?.Trim() ?? string.Empty));
            }

            // 4. Lọc theo Nội dung / Loại hình công nợ (DebtType)
            if (selectedDebtTypeIds != null && selectedDebtTypeIds.Count > 0)
            {
                query = query.Where(r => selectedDebtTypeIds.Contains(r.DebtTypeId?.Trim() ?? string.Empty));
            }

            // 5. Lọc theo Ngoại tệ (nếu là báo cáo ngoại tệ)
            if (!string.IsNullOrWhiteSpace(currencyId))
            {
                string curr = currencyId.Trim();
                query = query.Where(r => string.Equals(r.CurrencyId?.Trim(), curr, StringComparison.OrdinalIgnoreCase));
            }

            return new DebtReportContext
            {
                FromDate = fromDate.Date,
                ToDate = endDate,
                AccountFilterDisplay = accountFilterDisplay,
                AccountNameDisplay = accountNameDisplay,
                SortOrder = sortOrder,
                CurrencyId = currencyId,
                FilteredFlatRows = query.ToList()
            };
        }

        /// <summary>
        /// Tính toán danh sách DebtSummaryReportDTO cho Báo cáo Tổng hợp Công nợ (SummaryDebtReport)
        /// theo đúng công thức của Option Customer Summary Query trong Query.txt.
        /// </summary>
        public static List<DebtSummaryReportDTO> CalculateSummary(DebtReportContext context)
        {
            if (context == null || context.FilteredFlatRows.Count == 0)
                return new List<DebtSummaryReportDTO>();

            DateTime beginDate = context.FromDate.Date;
            DateTime endDate = context.ToDate.Date;
            bool byPartnerFirst = context.SortOrder == DebtReportSortOrder.ByPartnerThenDebtType;

            // Gom nhóm theo cặp (PartnerId, DebtTypeId) giống GROUP BY [Customer ID], [Liability Things ID]
            var groupedItems = context.FilteredFlatRows
                .GroupBy(r => new
                {
                    PartnerId = (r.PartnerId ?? string.Empty).Trim(),
                    DebtTypeId = (r.DebtTypeId ?? string.Empty).Trim()
                })
                .Select(g =>
                {
                    var first = g.First();
                    string partnerId = g.Key.PartnerId;
                    string partnerName = !string.IsNullOrWhiteSpace(first.PartnerName) ? first.PartnerName.Trim() : partnerId;
                    string debtTypeId = g.Key.DebtTypeId;
                    string debtTypeName = !string.IsNullOrWhiteSpace(first.DebtTypeName) ? first.DebtTypeName.Trim() : debtTypeId;

                    // Dư đầu kỳ (Bill Date < BeginDate): bù trừ Nợ - Có trước, sau đó tách DebitValue / CreditValue
                    decimal beginNet = g
                        .Where(r => r.IsOpeningAt(beginDate))
                        .Sum(r => (decimal)r.SignedAmount);

                    decimal beginDebit = beginNet > 0m ? beginNet : 0m;
                    decimal beginCredit = beginNet < 0m ? -beginNet : 0m;

                    // Phát sinh trong kỳ (BeginDate <= Bill Date <= EndDate)
                    decimal intDebit = g
                        .Where(r => r.IsInPeriod(beginDate, endDate) && r.Dbcr == 1)
                        .Sum(r => (decimal)r.Amount);

                    decimal intCredit = g
                        .Where(r => r.IsInPeriod(beginDate, endDate) && r.Dbcr == -1)
                        .Sum(r => (decimal)r.Amount);

                    // Dư cuối kỳ: EndNet = BeginDebit - BeginCredit + IntDebit - IntCredit
                    decimal endNet = beginDebit - beginCredit + intDebit - intCredit;
                    decimal endDebit = endNet > 0m ? endNet : 0m;
                    decimal endCredit = endNet < 0m ? -endNet : 0m;

                    string group1Id = byPartnerFirst ? partnerId : debtTypeId;
                    string group1Name = byPartnerFirst ? partnerName : debtTypeName;
                    string group2Id = byPartnerFirst ? debtTypeId : partnerId;
                    string group2Name = byPartnerFirst ? debtTypeName : partnerName;

                    return new DebtSummaryReportDTO
                    {
                        Group1Id = group1Id,
                        Group1Name = group1Name,
                        Group2Id = group2Id,
                        Group2Name = group2Name,
                        PartnerId = partnerId,
                        PartnerName = partnerName,
                        DebtTypeId = debtTypeId,
                        DebtTypeName = debtTypeName,
                        BeginDebit = beginDebit,
                        BeginCredit = beginCredit,
                        IntDebit = intDebit,
                        IntCredit = intCredit,
                        EndDebit = endDebit,
                        EndCredit = endCredit
                    };
                })
                // Chỉ giữ lại các dòng có số dư đầu kỳ, phát sinh trong kỳ, hoặc số dư cuối kỳ khác 0
                .Where(dto => dto.BeginDebit != 0m || dto.BeginCredit != 0m
                           || dto.IntDebit != 0m || dto.IntCredit != 0m
                           || dto.EndDebit != 0m || dto.EndCredit != 0m)
                .OrderBy(dto => dto.Group1Id, StringComparer.OrdinalIgnoreCase)
                .ThenBy(dto => dto.Group2Id, StringComparer.OrdinalIgnoreCase)
                .ToList();

            return groupedItems;
        }

        /// <summary>
        /// Tính toán danh sách DebtDetailReportDTO cho Báo cáo Chi tiết Công nợ
        /// (hỗ trợ cả mở trực tiếp từ Form Filter hoặc lọc thứ cấp khi Drill-down từ Group1Id trên báo cáo tổng hợp).
        /// </summary>
        public static List<DebtDetailReportDTO> CalculateDetail(
            DebtReportContext context,
            string? drillDownGroup1Id = null,
            string? drillDownGroup2Id = null)
        {
            if (context == null || context.FilteredFlatRows.Count == 0)
                return new List<DebtDetailReportDTO>();

            DateTime beginDate = context.FromDate.Date;
            DateTime endDate = context.ToDate.Date;
            bool byPartnerFirst = context.SortOrder == DebtReportSortOrder.ByPartnerThenDebtType;

            // Nếu có drillDownGroup1Id, lọc thứ cấp ngay trên bộ nhớ từ FilteredFlatRows
            IEnumerable<DebtFlatRow> sourceRows = !string.IsNullOrWhiteSpace(drillDownGroup1Id)
                ? context.GetDrillDownFlatRows(drillDownGroup1Id!, drillDownGroup2Id)
                : context.FilteredFlatRows;

            // Báo cáo chi tiết lấy các chứng từ phát sinh trong kỳ (Between BeginDate And EndDate)
            var detailDtos = sourceRows
                .Where(r => r.IsInPeriod(beginDate, endDate))
                .OrderBy(r => byPartnerFirst ? (r.PartnerId ?? string.Empty) : (r.DebtTypeId ?? string.Empty), StringComparer.OrdinalIgnoreCase)
                .ThenBy(r => byPartnerFirst ? (r.DebtTypeId ?? string.Empty) : (r.PartnerId ?? string.Empty), StringComparer.OrdinalIgnoreCase)
                .ThenBy(r => r.VoucherDate ?? DateTime.MinValue)
                .ThenBy(r => r.VoucherNo, StringComparer.OrdinalIgnoreCase)
                .ThenBy(r => r.DetailId)
                .Select(r =>
                {
                    string partnerId = (r.PartnerId ?? string.Empty).Trim();
                    string partnerName = !string.IsNullOrWhiteSpace(r.PartnerName) ? r.PartnerName.Trim() : partnerId;
                    string debtTypeId = (r.DebtTypeId ?? string.Empty).Trim();
                    string debtTypeName = !string.IsNullOrWhiteSpace(r.DebtTypeName) ? r.DebtTypeName.Trim() : debtTypeId;

                    string group1Id = byPartnerFirst ? partnerId : debtTypeId;
                    string group1Name = byPartnerFirst ? partnerName : debtTypeName;
                    string group2Id = byPartnerFirst ? debtTypeId : partnerId;
                    string group2Name = byPartnerFirst ? debtTypeName : partnerName;

                    decimal amount = (decimal)r.Amount;
                    decimal debit = r.Dbcr == 1 ? amount : 0m;
                    decimal credit = r.Dbcr == -1 ? amount : 0m;

                    return new DebtDetailReportDTO
                    {
                        JournalVoucherId = r.JournalVoucherId,
                        Group1Id = group1Id,
                        Group1Name = group1Name,
                        Group2Id = group2Id,
                        Group2Name = group2Name,
                        VoucherNo = r.VoucherNo ?? string.Empty,
                        VoucherDate = r.VoucherDate,
                        Contents = r.Contents ?? string.Empty,
                        Debit = debit,
                        Credit = credit
                    };
                })
                .Where(dto => dto.Debit != 0m || dto.Credit != 0m)
                .ToList();

            return detailDtos;
        }

        /// <summary>
        /// Tính toán danh sách DebtDueReportDTO cho Báo cáo Chi tiết Công nợ theo thời hạn / khế ước (DebtDueReport).
        /// Lấy Khế ước (LiabilityDue) làm trọng tâm nhóm Cấp 2:
        /// - Chỉ các giao dịch có liên kết khế ước hợp lệ (trong dueMap) mới được xuất ra báo cáo.
        /// - Thông tin thời hạn, lãi suất, tỷ lệ khấu trừ được đưa lên tiêu đề nhóm Khế ước (Group2Name).
        /// - Chi tiết bên dưới hiển thị các lần phát sinh Nợ/Có của khế ước đó.
        /// </summary>
        public static List<DebtDueReportDTO> CalculateDueDetail(
            DebtReportContext context,
            IReadOnlyDictionary<int, LiabilityDueEntity>? dueMap,
            string? drillDownGroup1Id = null,
            string? drillDownGroup2Id = null)
        {
            if (context == null || context.FilteredFlatRows.Count == 0 || dueMap == null || dueMap.Count == 0)
                return new List<DebtDueReportDTO>();

            DateTime beginDate = context.FromDate.Date;
            DateTime endDate = context.ToDate.Date;
            bool byPartnerFirst = context.SortOrder == DebtReportSortOrder.ByPartnerThenDebtType;

            IEnumerable<DebtFlatRow> sourceRows = !string.IsNullOrWhiteSpace(drillDownGroup1Id)
                ? context.GetDrillDownFlatRows(drillDownGroup1Id!, drillDownGroup2Id)
                : context.FilteredFlatRows;

            // Tạo từ điển tra cứu khế ước theo cả 2 cơ chế:
            // 1. Theo Id (nếu dòng có LiabilityDuesId)
            // 2. Theo cặp (PartnerId, DebtTypeId) - chuẩn liên kết mặc định trong CSDL Newacc
            var dueByPair = new Dictionary<string, LiabilityDueEntity>(StringComparer.OrdinalIgnoreCase);
            foreach (var due in dueMap.Values)
            {
                if (!string.IsNullOrWhiteSpace(due.CustomerId) && !string.IsNullOrWhiteSpace(due.DebtTypeId))
                {
                    string pairKey = $"{due.CustomerId.Trim()}|{due.DebtTypeId.Trim()}";
                    if (!dueByPair.ContainsKey(pairKey))
                    {
                        dueByPair[pairKey] = due;
                    }
                }
            }

            // Nhóm các dòng theo từng Khế ước (cặp Partner + DebtType)
            var contractGroups = sourceRows
                .Where(r => (!r.VoucherDate.HasValue || r.VoucherDate.Value.Date <= endDate))
                .Select(r =>
                {
                    LiabilityDueEntity? due = null;
                    if (r.LiabilityDuesId.HasValue && dueMap.TryGetValue(r.LiabilityDuesId.Value, out var d1))
                    {
                        due = d1;
                    }
                    else
                    {
                        string pairKey = $"{(r.PartnerId ?? string.Empty).Trim()}|{(r.DebtTypeId ?? string.Empty).Trim()}";
                        dueByPair.TryGetValue(pairKey, out due);
                    }

                    return new { Row = r, Due = due };
                })
                .Where(x => x.Due != null)
                .GroupBy(x => new
                {
                    PartnerId = (x.Row.PartnerId ?? string.Empty).Trim(),
                    DebtTypeId = (x.Row.DebtTypeId ?? string.Empty).Trim(),
                    DueId = x.Due!.Id
                })
                .OrderBy(g => byPartnerFirst ? g.Key.PartnerId : g.Key.DebtTypeId, StringComparer.OrdinalIgnoreCase)
                .ThenBy(g => byPartnerFirst ? g.Key.DebtTypeId : g.Key.PartnerId, StringComparer.OrdinalIgnoreCase)
                .ToList();

            var resultDtos = new List<DebtDueReportDTO>();

            foreach (var group in contractGroups)
            {
                var firstItem = group.First();
                var due = firstItem.Due!;

                string partnerId = group.Key.PartnerId;
                string partnerName = !string.IsNullOrWhiteSpace(firstItem.Row.PartnerName) ? firstItem.Row.PartnerName.Trim() : partnerId;
                string debtTypeId = group.Key.DebtTypeId;
                string debtTypeName = !string.IsNullOrWhiteSpace(firstItem.Row.DebtTypeName) ? firstItem.Row.DebtTypeName.Trim() : debtTypeId;

                string group1Id = byPartnerFirst ? partnerId : debtTypeId;
                string group1Name = byPartnerFirst ? partnerName : debtTypeName;

                // Tên Khế ước chuẩn: "[Tên Đối Tượng] — [Tên Nội Dung/Type]" kèm Thời hạn và Lãi suất
                string dateInfo = $"Hạn TT: {(due.EndDate.HasValue ? due.EndDate.Value.ToString("dd/MM/yyyy") : "---")}";
                if (due.BeginDate.HasValue)
                {
                    dateInfo = $"Thời hạn: {due.BeginDate.Value:dd/MM/yyyy} -> {(due.EndDate.HasValue ? due.EndDate.Value.ToString("dd/MM/yyyy") : "---")}";
                }

                string rateInfo = $"Lãi suất: {(due.Interestrate.HasValue ? due.Interestrate.Value.ToString("0.##") + "%" : "0%")}";
                if (due.Feedbackrate.HasValue && due.Feedbackrate.Value > 0)
                    rateInfo += $" | Khấu trừ: {due.Feedbackrate.Value:0.##}%";
                if (due.Outvatrate.HasValue && due.Outvatrate.Value > 0)
                    rateInfo += $" | VAT: {due.Outvatrate.Value:0.##}%";

                string group2Id = $"DUE_{due.Id}";
                // Tiêu đề khế ước trực quan: Tên đối tượng — Tên type (Thời hạn | Lãi suất)
                string group2Name = $"{partnerName} — {debtTypeName} ({dateInfo} | {rateInfo})";

                // 1. TÍNH VÀ GỘP SỐ DƯ ĐẦU KỲ (CỘNG DỒN NHƯ SỔ CÁI)
                var openingRows = group
                    .Select(x => x.Row)
                    .Where(r => r.IsOpeningAt(beginDate))
                    .ToList();

                decimal beginNet = openingRows.Sum(r => (decimal)r.SignedAmount);
                if (beginNet != 0m)
                {
                    decimal beginDebit = beginNet > 0m ? beginNet : 0m;
                    decimal beginCredit = beginNet < 0m ? -beginNet : 0m;

                    resultDtos.Add(new DebtDueReportDTO
                    {
                        JournalVoucherId = null,
                        LiabilityDuesId = due.Id,
                        PartnerId = partnerId,
                        PartnerName = partnerName,
                        DebtTypeId = debtTypeId,
                        DebtTypeName = debtTypeName,
                        Group1Id = group1Id,
                        Group1Name = group1Name,
                        Group2Id = group2Id,
                        Group2Name = group2Name,
                        VoucherNo = "SDDK",
                        VoucherDate = beginDate,
                        Contents = "Số dư đầu kỳ mang sang",
                        BeginDate = due.BeginDate,
                        EndDate = due.EndDate,
                        InterestRate = due.Interestrate,
                        FeedbackRate = due.Feedbackrate,
                        OutVatRate = due.Outvatrate,
                        Remark = due.Remark ?? string.Empty,
                        SettledFlag = due.Settledflag,
                        Debit = beginDebit,
                        Credit = beginCredit
                    });
                }

                // 2. CÁC CHỨNG TỪ PHÁT SINH THỰC TẾ TRONG KỲ (Có đầy đủ Số CT, Ngày CT, Diễn giải để Drill-down)
                var periodRows = group
                    .Select(x => x.Row)
                    .Where(r => r.IsInPeriod(beginDate, endDate))
                    .OrderBy(r => r.VoucherDate ?? DateTime.MinValue)
                    .ThenBy(r => r.VoucherNo, StringComparer.OrdinalIgnoreCase)
                    .ThenBy(r => r.DetailId)
                    .ToList();

                foreach (var r in periodRows)
                {
                    decimal amount = (decimal)r.Amount;
                    decimal debit = r.Dbcr == 1 ? amount : 0m;
                    decimal credit = r.Dbcr == -1 ? amount : 0m;

                    if (debit == 0m && credit == 0m) continue;

                    resultDtos.Add(new DebtDueReportDTO
                    {
                        JournalVoucherId = r.JournalVoucherId,
                        LiabilityDuesId = due.Id,
                        PartnerId = partnerId,
                        PartnerName = partnerName,
                        DebtTypeId = debtTypeId,
                        DebtTypeName = debtTypeName,
                        Group1Id = group1Id,
                        Group1Name = group1Name,
                        Group2Id = group2Id,
                        Group2Name = group2Name,
                        VoucherNo = r.VoucherNo ?? string.Empty,
                        VoucherDate = r.VoucherDate,
                        Contents = r.Contents ?? string.Empty,
                        BeginDate = due.BeginDate,
                        EndDate = due.EndDate,
                        InterestRate = due.Interestrate,
                        FeedbackRate = due.Feedbackrate,
                        OutVatRate = due.Outvatrate,
                        Remark = due.Remark ?? string.Empty,
                        SettledFlag = due.Settledflag,
                        Debit = debit,
                        Credit = credit
                    });
                }
            }

            return resultDtos
                .Where(dto => dto.Debit != 0m || dto.Credit != 0m)
                .ToList();
        }

        /// <summary>
        /// Phiên bản nạp nhanh nếu không truyền dueMap (trả về danh sách rỗng để bảo toàn chữ ký hàm).
        /// </summary>
        public static List<DebtDueReportDTO> CalculateDueDetail(
            DebtReportContext context,
            string? drillDownGroup1Id = null,
            string? drillDownGroup2Id = null)
        {
            return CalculateDueDetail(context, null, drillDownGroup1Id, drillDownGroup2Id);
        }
    }
}

