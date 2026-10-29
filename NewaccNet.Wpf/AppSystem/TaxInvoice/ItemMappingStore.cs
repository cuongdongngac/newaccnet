using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace NewaccNet.Wpf.AppSystem.TaxInvoice
{
    public class ItemMappingRecord
    {
        public string SellerTaxCode { get; set; } = string.Empty;
        public string InvoiceItemName { get; set; } = string.Empty;
        public string InvoiceUnit { get; set; } = string.Empty;
        public string InternalMaterialId { get; set; } = string.Empty;
        public string InternalMaterialName { get; set; } = string.Empty;
        public DateTime LastUpdated { get; set; } = DateTime.Now;
    }

    /// <summary>
    /// Sổ tay ghi nhớ ánh xạ danh mục vật tư (Tier 1: Alias Memory Store).
    /// Ghi nhớ vĩnh viễn việc ghép nối giữa tên hàng trên hóa đơn và mã nội bộ NewaccNet.
    /// </summary>
    public class ItemMappingStore
    {
        private static ItemMappingStore? _instance;
        private static readonly object _lock = new();

        public static ItemMappingStore Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        _instance ??= new ItemMappingStore();
                    }
                }
                return _instance;
            }
        }

        private readonly Dictionary<string, ItemMappingRecord> _cache = new(StringComparer.OrdinalIgnoreCase);
        private readonly string _filePath;

        public ItemMappingStore()
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var folder = System.IO.Path.Combine(appData, "NewaccNet");
            System.IO.Directory.CreateDirectory(folder);
            _filePath = System.IO.Path.Combine(folder, "ItemMappingStore.json");
            Load();
        }

        public ItemMappingStore(string customFilePath)
        {
            _filePath = customFilePath;
            Load();
        }

        private static string BuildKey(string? sellerTaxCode, string itemName, string? unit)
        {
            var stc = sellerTaxCode?.Trim() ?? "";
            var normName = MaterialCodeGenerator.RemoveDiacritics(itemName ?? "").Trim().ToLowerInvariant();
            var normUnit = MaterialCodeGenerator.RemoveDiacritics(unit ?? "").Trim().ToLowerInvariant();
            return $"{stc}|{normName}|{normUnit}";
        }

        private static string BuildGeneralKey(string itemName, string? unit)
        {
            var normName = MaterialCodeGenerator.RemoveDiacritics(itemName ?? "").Trim().ToLowerInvariant();
            var normUnit = MaterialCodeGenerator.RemoveDiacritics(unit ?? "").Trim().ToLowerInvariant();
            return $"*|{normName}|{normUnit}";
        }

        public bool TryFindMapping(string? sellerTaxCode, string itemName, string? unit, out ItemMappingRecord? record)
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

            // 2. Nếu không có theo NCC, tra theo Tên hàng + ĐVT chung
            var generalKey = BuildGeneralKey(itemName, unit);
            if (_cache.TryGetValue(generalKey, out record))
            {
                return true;
            }

            return false;
        }

        public void SaveMapping(string? sellerTaxCode, string invoiceItemName, string? unit, string internalId, string internalName)
        {
            if (string.IsNullOrWhiteSpace(invoiceItemName) || string.IsNullOrWhiteSpace(internalId)) return;

            var rec = new ItemMappingRecord
            {
                SellerTaxCode = sellerTaxCode?.Trim() ?? "",
                InvoiceItemName = invoiceItemName.Trim(),
                InvoiceUnit = unit?.Trim() ?? "",
                InternalMaterialId = internalId.Trim().ToUpperInvariant(),
                InternalMaterialName = internalName?.Trim() ?? "",
                LastUpdated = DateTime.Now
            };

            // Lưu cả key theo NCC và key chung
            if (!string.IsNullOrWhiteSpace(sellerTaxCode))
            {
                _cache[BuildKey(sellerTaxCode, invoiceItemName, unit)] = rec;
            }
            _cache[BuildGeneralKey(invoiceItemName, unit)] = rec;

            Persist();
        }

        private void Load()
        {
            try
            {
                if (System.IO.File.Exists(_filePath))
                {
                    var json = System.IO.File.ReadAllText(_filePath);
                    var items = JsonSerializer.Deserialize<List<ItemMappingRecord>>(json);
                    if (items != null)
                    {
                        foreach (var item in items)
                        {
                            if (!string.IsNullOrWhiteSpace(item.SellerTaxCode))
                            {
                                _cache[BuildKey(item.SellerTaxCode, item.InvoiceItemName, item.InvoiceUnit)] = item;
                            }
                            _cache[BuildGeneralKey(item.InvoiceItemName, item.InvoiceUnit)] = item;
                        }
                    }
                }
            }
            catch { }
        }

        private void Persist()
        {
            try
            {
                var uniqueRecords = _cache.Values
                    .GroupBy(x => $"{x.SellerTaxCode}|{x.InvoiceItemName}|{x.InvoiceUnit}", StringComparer.OrdinalIgnoreCase)
                    .Select(g => g.First())
                    .ToList();

                var json = JsonSerializer.Serialize(uniqueRecords, new JsonSerializerOptions { WriteIndented = true });
                System.IO.File.WriteAllText(_filePath, json);
            }
            catch { }
        }
    }
}
