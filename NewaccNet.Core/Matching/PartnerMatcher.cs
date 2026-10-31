using System;
using System.Collections.Generic;
using System.Linq;
using DataAccess.EntityClasses;
using NewaccNet.Contracts;
using NewaccNet.Core.Staging;

namespace NewaccNet.Core.Matching
{
    public class PartnerMatchResult
    {
        public string PartnerId { get; set; } = string.Empty;
        public string PartnerName { get; set; } = string.Empty;
        public bool IsNew { get; set; }
        public MatchMethod Method { get; set; } = MatchMethod.None;
        public double Confidence { get; set; } = 0.0;
        public string Reason { get; set; } = string.Empty;
        public ProposedMasterData? Proposed { get; set; }
    }

    public static class PartnerMatcher
    {
        public static PartnerMatchResult Match(string? rawTaxCode, string? rawName, string? rawAddress, IEnumerable<PartnerEntity> existingPartners)
        {
            var res = new PartnerMatchResult();
            var tax = rawTaxCode?.Trim() ?? "";
            var name = rawName?.Trim() ?? "";
            var addr = rawAddress?.Trim() ?? "";

            // 1. So khớp tuyệt đối theo Mã số thuế (Exact TaxNo)
            if (!string.IsNullOrWhiteSpace(tax))
            {
                var match = existingPartners.FirstOrDefault(p => string.Equals(p.Taxno?.Trim(), tax, StringComparison.OrdinalIgnoreCase));
                if (match != null)
                {
                    res.PartnerId = match.Id;
                    res.PartnerName = match.CustomerName ?? name;
                    res.IsNew = false;
                    res.Method = MatchMethod.TaxCodeExact;
                    res.Confidence = 1.0;
                    res.Reason = $"Trùng khớp 100% Mã số thuế với đối tác '{match.Id} - {match.CustomerName}'";
                    return res;
                }
            }

            // 2. So khớp theo tên chuẩn hóa (nếu không có MST)
            if (!string.IsNullOrWhiteSpace(name))
            {
                var normTarget = MappingMemoryStore.RemoveDiacritics(name).ToLowerInvariant();
                var match = existingPartners.FirstOrDefault(p => 
                    !string.IsNullOrWhiteSpace(p.CustomerName) && 
                    string.Equals(MappingMemoryStore.RemoveDiacritics(p.CustomerName).ToLowerInvariant(), normTarget, StringComparison.OrdinalIgnoreCase));

                if (match != null)
                {
                    res.PartnerId = match.Id;
                    res.PartnerName = match.CustomerName ?? name;
                    res.IsNew = false;
                    res.Method = MatchMethod.CodeExact;
                    res.Confidence = 0.95;
                    res.Reason = $"Trùng khớp tên công ty với đối tác '{match.Id}'";
                    return res;
                }
            }

            // 3. Đối tác mới hoàn toàn -> Đề xuất tạo mới
            var proposedId = !string.IsNullOrWhiteSpace(tax) ? tax : "DT_" + DateTime.Now.ToString("yyMMddHHmm");
            res.PartnerId = proposedId;
            res.PartnerName = name;
            res.IsNew = true;
            res.Method = MatchMethod.None;
            res.Confidence = 0.0;
            res.Reason = "Đối tác mới chưa có trong danh mục hệ thống";

            var proposed = new ProposedMasterData
            {
                Kind = "Partner",
                TempKey = proposedId,
                Approved = false
            };
            proposed.Fields["Id"] = proposedId;
            proposed.Fields["Taxno"] = tax;
            proposed.Fields["CustomerName"] = name;
            proposed.Fields["Address"] = addr;
            res.Proposed = proposed;

            return res;
        }
    }
}
