using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using NewaccNet.Contracts;
using NewaccNet.Core.Matching;
using NewaccNet.Core.Posting;
using NewaccNet.Core.Staging;

namespace NewaccNet.AI.Tools
{
    /// <summary>
    /// Tool AI: Tra cứu đối tác trong CSDL theo Mã số thuế hoặc tên.
    /// </summary>
    public class SearchPartnersTool : IAiTool
    {
        public string Name => "search_partners";
        public string Description => "Tra cứu danh mục đối tác khách hàng, nhà cung cấp trong CSDL theo Mã số thuế (taxCode) hoặc từ khóa tên (name).";
        public AiToolAccess Access => AiToolAccess.Read;

        public string ParametersJsonSchema => """
        {
            "type": "object",
            "properties": {
                "taxCode": { "type": "string", "description": "Mã số thuế của đối tác" },
                "name": { "type": "string", "description": "Tên hoặc từ khóa tên công ty" }
            }
        }
        """;

        public Task<string> ExecuteAsync(JsonElement args, CancellationToken ct = default)
        {
            string? taxCode = args.TryGetProperty("taxCode", out var tc) ? tc.GetString() : null;
            string? name = args.TryGetProperty("name", out var n) ? n.GetString() : null;

            var partners = WarehousePostingGateway.LoadPartners();
            var matches = partners.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(taxCode))
            {
                var cleanTax = taxCode.Trim();
                matches = matches.Where(p => (p.Taxno ?? "").Contains(cleanTax, StringComparison.OrdinalIgnoreCase));
            }
            else if (!string.IsNullOrWhiteSpace(name))
            {
                var normName = MappingMemoryStore.RemoveDiacritics(name).ToLowerInvariant();
                matches = matches.Where(p => MappingMemoryStore.RemoveDiacritics(p.CustomerName ?? "").ToLowerInvariant().Contains(normName));
            }

            var result = matches.Take(5).Select(p => new
            {
                p.Id,
                p.Taxno,
                p.CustomerName,
                p.Address
            }).ToList();

            return Task.FromResult(JsonSerializer.Serialize(result));
        }
    }

    /// <summary>
    /// Tool AI: Tra cứu danh mục hàng hóa, vật tư trong CSDL kho.
    /// </summary>
    public class SearchMaterialsTool : IAiTool
    {
        public string Name => "search_materials";
        public string Description => "Tra cứu danh mục vật tư hàng hóa trong kho theo từ khóa tên hoặc mã vật tư.";
        public AiToolAccess Access => AiToolAccess.Read;

        public string ParametersJsonSchema => """
        {
            "type": "object",
            "properties": {
                "keyword": { "type": "string", "description": "Từ khóa tên hoặc mã mặt hàng cần tìm" }
            }
        }
        """;

        public Task<string> ExecuteAsync(JsonElement args, CancellationToken ct = default)
        {
            string? keyword = args.TryGetProperty("keyword", out var kw) ? kw.GetString() : null;
            if (string.IsNullOrWhiteSpace(keyword)) return Task.FromResult("[]");

            var materials = WarehousePostingGateway.LoadMaterials();
            var normKeyword = MappingMemoryStore.RemoveDiacritics(keyword).ToLowerInvariant();

            var matches = materials.Where(m =>
                (m.MaterialId ?? "").Contains(normKeyword, StringComparison.OrdinalIgnoreCase) ||
                MappingMemoryStore.RemoveDiacritics(m.MaterialName ?? "").ToLowerInvariant().Contains(normKeyword)
            ).Take(5).Select(m => new
            {
                m.MaterialId,
                m.MaterialName,
                m.Unit,
                m.Categoryid
            }).ToList();

            return Task.FromResult(JsonSerializer.Serialize(matches));
        }
    }

    /// <summary>
    /// Tool AI: Tra cứu sổ tay ghi nhớ lịch sử ánh xạ (Mapping Memory).
    /// </summary>
    public class GetMappingMemoryTool : IAiTool
    {
        public string Name => "get_mapping_memory";
        public string Description => "Tra cứu các liên kết ánh xạ đã từng được người dùng xác nhận trong lịch sử cho nhà cung cấp này.";
        public AiToolAccess Access => AiToolAccess.Read;

        public string ParametersJsonSchema => """
        {
            "type": "object",
            "properties": {
                "sellerTaxCode": { "type": "string", "description": "Mã số thuế của nhà cung cấp" },
                "itemName": { "type": "string", "description": "Tên mặt hàng gốc trên hóa đơn" },
                "unit": { "type": "string", "description": "Đơn vị tính trên hóa đơn (tùy chọn)" }
            },
            "required": ["itemName"]
        }
        """;

        public Task<string> ExecuteAsync(JsonElement args, CancellationToken ct = default)
        {
            string? sellerTaxCode = args.TryGetProperty("sellerTaxCode", out var stc) ? stc.GetString() : null;
            string? itemName = args.TryGetProperty("itemName", out var itm) ? itm.GetString() : null;
            string? unit = args.TryGetProperty("unit", out var u) ? u.GetString() : null;

            if (string.IsNullOrWhiteSpace(itemName)) return Task.FromResult("null");

            if (MappingMemoryStore.Instance.TryFindMapping(sellerTaxCode, itemName, unit, out var record) && record != null)
            {
                return Task.FromResult(JsonSerializer.Serialize(record));
            }

            return Task.FromResult("null");
        }
    }

    /// <summary>
    /// Tool AI: Ghi đề xuất gợi ý mã vào Draft.
    /// </summary>
    public class SuggestFieldTool : IAiTool
    {
        public string Name => "suggest_field";
        public string Description => "Gợi ý mã đối tác, mã vật tư hoặc tài khoản cho các trường còn thiếu trong chứng từ nháp.";
        public AiToolAccess Access => AiToolAccess.DraftWrite;

        public List<MatchInfo> Suggestions { get; } = new();

        public string ParametersJsonSchema => """
        {
            "type": "object",
            "properties": {
                "lineNo": { "type": "integer", "description": "Số dòng chi tiết hàng hóa (0 nếu là cấp chứng từ/đối tác)" },
                "field": { "type": "string", "description": "Tên trường đề xuất (PartnerId, MaterialId, WarehouseId, DebitAccountId)" },
                "value": { "type": "string", "description": "Mã giá trị được đề xuất" },
                "confidence": { "type": "number", "description": "Độ tin cậy từ 0.0 đến 1.0" },
                "reason": { "type": "string", "description": "Lý do căn cứ cho đề xuất này" }
            },
            "required": ["lineNo", "field", "value", "confidence", "reason"]
        }
        """;

        public Task<string> ExecuteAsync(JsonElement args, CancellationToken ct = default)
        {
            var field = args.TryGetProperty("field", out var f) ? f.GetString() ?? "" : "";
            var value = args.TryGetProperty("value", out var v) ? v.GetString() ?? "" : "";
            var confidence = args.TryGetProperty("confidence", out var c) ? c.GetDouble() : 0.8;
            var reason = args.TryGetProperty("reason", out var r) ? r.GetString() ?? "" : "";

            var suggestion = new MatchInfo
            {
                Field = $"{field}:{value}",
                Method = MatchMethod.Ai,
                Confidence = confidence,
                Reason = reason
            };

            Suggestions.Add(suggestion);
            return Task.FromResult("{\"status\": \"suggested\"}");
        }
    }
}
