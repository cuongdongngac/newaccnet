using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using NewaccNet.Contracts;

namespace NewaccNet.Core.Staging
{
    public class MappingMemoryItem
    {
        public string SellerTaxCode { get; set; } = string.Empty;
        public string InvoiceItemName { get; set; } = string.Empty;
        public string InvoiceUnit { get; set; } = string.Empty;
        public string InternalMaterialId { get; set; } = string.Empty;
        public string InternalMaterialName { get; set; } = string.Empty;
        public string DefaultWarehouseId { get; set; } = string.Empty;
        public string DefaultDebitAccount { get; set; } = "1561";
        public string DefaultVatAccount { get; set; } = "13311";
        public DateTime LastUpdated { get; set; } = DateTime.Now;
    }

    /// <summary>
    /// Sổ tay ghi nhớ ánh xạ (Mapping Memory Store) tại tầng Core.
    /// Tự động ghi nhớ vĩnh viễn các liên kết đã được người dùng xác nhận hoặc đã ghi sổ thành công.
    /// Giúp các lần nhập hóa đơn sau từ cùng nhà cung cấp tự động khớp 100% tất định mà không cần AI.
    /// </summary>
    public class MappingMemoryStore
    {
        private static MappingMemoryStore? _instance;
        private static readonly object _lock = new();

        public static MappingMemoryStore Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        _instance ??= new MappingMemoryStore();
                    }
                }
                return _instance;
            }
        }

        private readonly Dictionary<string, MappingMemoryItem> _cache = new(StringComparer.OrdinalIgnoreCase);
        private readonly string _filePath;

        public MappingMemoryStore()
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var folder = Path.Combine(appData, "NewaccNet");
            Directory.CreateDirectory(folder);
            _filePath = Path.Combine(folder, "MappingMemoryStore.json");
            Load();
        }

        public MappingMemoryStore(string customFilePath)
        {
            _filePath = customFilePath;
            Load();
        }

        public static string RemoveDiacritics(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return string.Empty;
            var normalizedString = text.Normalize(System.Text.NormalizationForm.FormD);
            var stringBuilder = new System.Text.StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != System.Globalization.UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(System.Text.NormalizationForm.FormC).Replace('đ', 'd').Replace('Đ', 'D');
        }

        private static string BuildKey(string? sellerTaxCode, string itemName, string? unit)
        {
            var stc = sellerTaxCode?.Trim() ?? "";
            var normName = RemoveDiacritics(itemName ?? "").Trim().ToLowerInvariant();
            var normUnit = RemoveDiacritics(unit ?? "").Trim().ToLowerInvariant();
            return $"{stc}|{normName}|{normUnit}";
        }

        private static string BuildGeneralKey(string itemName, string? unit)
        {
            var normName = RemoveDiacritics(itemName ?? "").Trim().ToLowerInvariant();
            var normUnit = RemoveDiacritics(unit ?? "").Trim().ToLowerInvariant();
            return $"*|{normName}|{normUnit}";
        }

        public bool TryFindMapping(string? sellerTaxCode, string itemName, string? unit, out MappingMemoryItem? record)
        {
            record = null;
            if (string.IsNullOrWhiteSpace(itemName)) return false;

            // 1. Ưu tiên khớp chính xác theo NCC + Tên hàng + ĐVT
            if (!string.IsNullOrWhiteSpace(sellerTaxCode))
            {
                var specificKey = BuildKey(sellerTaxCode, itemName, unit);
                if (_cache.TryGetValue(specificKey, out record))
                {
                    return true;
                }
            }

            // 2. Tra theo Tên hàng + ĐVT chung
            var generalKey = BuildGeneralKey(itemName, unit);
            if (_cache.TryGetValue(generalKey, out record))
            {
                return true;
            }

            return false;
        }

        public void Learn(string? sellerTaxCode, string invoiceItemName, string? unit, string internalMaterialId, string internalMaterialName, string warehouseId = "", string debitAccount = "1561", string vatAccount = "13311")
        {
            if (string.IsNullOrWhiteSpace(invoiceItemName) || string.IsNullOrWhiteSpace(internalMaterialId)) return;

            var item = new MappingMemoryItem
            {
                SellerTaxCode = sellerTaxCode?.Trim() ?? "",
                InvoiceItemName = invoiceItemName.Trim(),
                InvoiceUnit = unit?.Trim() ?? "",
                InternalMaterialId = internalMaterialId.Trim(),
                InternalMaterialName = internalMaterialName.Trim(),
                DefaultWarehouseId = warehouseId.Trim(),
                DefaultDebitAccount = string.IsNullOrWhiteSpace(debitAccount) ? "1561" : debitAccount.Trim(),
                DefaultVatAccount = string.IsNullOrWhiteSpace(vatAccount) ? "13311" : vatAccount.Trim(),
                LastUpdated = DateTime.Now
            };

            lock (_lock)
            {
                var specificKey = BuildKey(sellerTaxCode, invoiceItemName, unit);
                _cache[specificKey] = item;

                var generalKey = BuildGeneralKey(invoiceItemName, unit);
                _cache[generalKey] = item;

                Save();
            }
        }

        public void LearnFromDraft(VoucherDraft draft)
        {
            if (draft == null || draft.Lines == null) return;
            var sellerTax = draft.Counterparty?.Raw?.TaxCode;

            foreach (var line in draft.Lines)
            {
                if (!string.IsNullOrWhiteSpace(line.Resolved?.MaterialId))
                {
                    Learn(sellerTax, line.Raw.ItemName, line.Raw.Unit, line.Resolved.MaterialId, line.Raw.ItemName, line.Resolved.WarehouseId, line.Resolved.DebitAccountId, line.Resolved.VatAccountId);
                }
            }
        }

        private void Load()
        {
            try
            {
                if (!File.Exists(_filePath)) return;
                var json = File.ReadAllText(_filePath);
                var items = JsonSerializer.Deserialize<List<MappingMemoryItem>>(json);
                if (items == null) return;

                lock (_lock)
                {
                    _cache.Clear();
                    foreach (var item in items)
                    {
                        var specificKey = BuildKey(item.SellerTaxCode, item.InvoiceItemName, item.InvoiceUnit);
                        _cache[specificKey] = item;

                        var generalKey = BuildGeneralKey(item.InvoiceItemName, item.InvoiceUnit);
                        _cache[generalKey] = item;
                    }
                }
            }
            catch { }
        }

        private void Save()
        {
            try
            {
                var uniqueList = new HashSet<MappingMemoryItem>(_cache.Values);
                var options = new JsonSerializerOptions { WriteIndented = true };
                var json = JsonSerializer.Serialize(uniqueList, options);
                File.WriteAllText(_filePath, json);
            }
            catch { }
        }
    }
}
