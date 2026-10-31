using System;
using System.Collections.Generic;
using System.Linq;
using DataAccess.EntityClasses;
using NewaccNet.Contracts;
using NewaccNet.Core.Staging;

namespace NewaccNet.Core.Matching
{
    public class MaterialMatchResult
    {
        public string MaterialId { get; set; } = string.Empty;
        public string MaterialName { get; set; } = string.Empty;
        public string WarehouseId { get; set; } = string.Empty;
        public string DebitAccountId { get; set; } = "1561";
        public string VatAccountId { get; set; } = "13311";
        public bool IsNew { get; set; }
        public MatchMethod Method { get; set; } = MatchMethod.None;
        public double Confidence { get; set; } = 0.0;
        public string Reason { get; set; } = string.Empty;
        public ProposedMasterData? Proposed { get; set; }
    }

    public static class MaterialMatcher
    {
        public static MaterialMatchResult Match(
            string? sellerTaxCode,
            string rawItemName,
            string rawUnit,
            string defaultWarehouseId,
            IEnumerable<InventoryItemEntity> existingMaterials)
        {
            var res = new MaterialMatchResult
            {
                WarehouseId = defaultWarehouseId,
                DebitAccountId = "1561",
                VatAccountId = "13311"
            };

            var name = rawItemName?.Trim() ?? "";
            var unit = rawUnit?.Trim() ?? "";
            if (string.IsNullOrWhiteSpace(name))
            {
                res.Reason = "Tên mặt hàng trống";
                return res;
            }

            // 1. Tra cứu Sổ tay ghi nhớ lịch sử (Mapping Memory Store)
            if (MappingMemoryStore.Instance.TryFindMapping(sellerTaxCode, name, unit, out var mapped) && mapped != null)
            {
                res.MaterialId = mapped.InternalMaterialId;
                res.MaterialName = mapped.InternalMaterialName;
                if (!string.IsNullOrWhiteSpace(mapped.DefaultWarehouseId)) res.WarehouseId = mapped.DefaultWarehouseId;
                if (!string.IsNullOrWhiteSpace(mapped.DefaultDebitAccount)) res.DebitAccountId = mapped.DefaultDebitAccount;
                if (!string.IsNullOrWhiteSpace(mapped.DefaultVatAccount)) res.VatAccountId = mapped.DefaultVatAccount;
                res.IsNew = false;
                res.Method = MatchMethod.AliasMemory;
                res.Confidence = 0.98;
                res.Reason = string.IsNullOrWhiteSpace(mapped.SellerTaxCode)
                    ? "Đã từng ánh xạ trong sổ tay ghi nhớ hệ thống"
                    : $"Đã từng ánh xạ từ NCC {mapped.SellerTaxCode}";
                return res;
            }

            // 2. Tra cứu trùng khớp chính xác tên vật tư trong DB (Exact Name)
            var normTarget = MappingMemoryStore.RemoveDiacritics(name).ToLowerInvariant();
            var exact = existingMaterials.FirstOrDefault(m =>
                !string.IsNullOrWhiteSpace(m.MaterialName) &&
                string.Equals(MappingMemoryStore.RemoveDiacritics(m.MaterialName).ToLowerInvariant(), normTarget, StringComparison.OrdinalIgnoreCase));

            if (exact != null)
            {
                res.MaterialId = exact.MaterialId;
                res.MaterialName = exact.MaterialName;
                res.IsNew = false;
                res.Method = MatchMethod.CodeExact;
                res.Confidence = 1.0;
                res.Reason = $"Trùng khớp tên vật tư '{exact.MaterialId} - {exact.MaterialName}' trong danh mục";
                return res;
            }

            // 3. So khớp mờ (Fuzzy Match) theo Bigram Dice & Token Jaccard
            var tupleList = existingMaterials.Select(m => (m.MaterialId, m.MaterialName ?? "", (string?)m.Categoryid)).ToList();
            var fuzzy = FuzzyMatchHelper.FindBestMatch(name, tupleList, threshold: 0.72);
            if (fuzzy != null)
            {
                res.MaterialId = fuzzy.MaterialId;
                res.MaterialName = fuzzy.MaterialName;
                res.IsNew = false;
                res.Method = MatchMethod.Fuzzy;
                res.Confidence = Math.Round(fuzzy.SimilarityScore, 2);
                res.Reason = $"Gợi ý tương đồng {(int)(fuzzy.SimilarityScore * 100)}% với '{fuzzy.MaterialName}'";
                return res;
            }

            // 4. Mặt hàng mới hoàn toàn -> Đề xuất tạo mới
            var codePrefix = "VT_";
            var tokens = MappingMemoryStore.RemoveDiacritics(name).Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length >= 2)
            {
                var initials = string.Concat(tokens.Take(3).Select(t => char.ToUpperInvariant(t[0])));
                codePrefix = initials + "_";
            }
            var proposedCode = codePrefix + DateTime.Now.ToString("yyMMddHHmm");

            res.MaterialId = proposedCode;
            res.MaterialName = name;
            res.IsNew = true;
            res.Method = MatchMethod.None;
            res.Confidence = 0.0;
            res.Reason = "Mặt hàng mới chưa có trong danh mục kho";

            var proposed = new ProposedMasterData
            {
                Kind = "Material",
                TempKey = proposedCode,
                Approved = false
            };
            proposed.Fields["MaterialId"] = proposedCode;
            proposed.Fields["MaterialName"] = name;
            proposed.Fields["Unit"] = unit;
            proposed.Fields["CategoryId"] = "156";
            res.Proposed = proposed;

            return res;
        }
    }
}
