using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace Hddt
{
    /// <summary>
    /// Lưu dữ liệu tải về ra thư mục:
    /// <code>
    /// {root}/{mst}/{mua|ban}/{yyyy-MM}/
    ///     list_query_001.json, list_sco_001.json   nguyên văn phản hồi danh sách
    ///     {nbmst}_{khmshdon}_{khhdon}_{shdon}.json  chi tiết
    ///     {nbmst}_{khmshdon}_{khhdon}_{shdon}.zip   XML gốc (nếu tải)
    ///     {nbmst}_{khmshdon}_{khhdon}_{shdon}/      XML đã giải nén (nếu chọn)
    ///     index.json   danh sách hóa đơn của tháng + tình trạng từng file
    ///     _log.json    thời điểm tải, số lượng, lỗi
    /// </code>
    /// Tên file theo bộ 4 khóa nên tải lại sẽ ghi đè, không bị trùng.
    /// </summary>
    public sealed class InvoiceFileStore
    {
        internal static readonly JsonSerializerOptions JsonOut = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, // giữ tiếng Việt dễ đọc
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

        private static readonly UTF8Encoding Utf8NoBom = new UTF8Encoding(false);

        public InvoiceFileStore(string rootFolder)
        {
            RootFolder = Path.GetFullPath(rootFolder);
        }

        public string RootFolder { get; }

        public string MonthFolder(string mst, InvoiceDirection direction, DateRange range)
        {
            var dir = Path.Combine(RootFolder, mst, direction == InvoiceDirection.Purchase ? "mua" : "ban", range.MonthLabel);
            Directory.CreateDirectory(dir);
            return dir;
        }

        /// <summary>Xoá các trang danh sách cũ của một nguồn trước khi tải lại.</summary>
        public void ClearListPages(string folder, InvoiceSource source)
        {
            foreach (var f in Directory.GetFiles(folder, "list_" + SourceTag(source) + "_*.json"))
                File.Delete(f);
        }

        public void SaveListPage(string folder, InvoiceSource source, int pageNo, string rawJson) =>
            WriteText(Path.Combine(folder, "list_" + SourceTag(source) + "_" + pageNo.ToString("000") + ".json"), rawJson);

        public string DetailPath(string folder, InvoiceKey key) => Path.Combine(folder, key.FileStem + ".json");
        public string XmlZipPath(string folder, InvoiceKey key) => Path.Combine(folder, key.FileStem + ".zip");

        public void SaveDetail(string folder, InvoiceKey key, string rawJson) => WriteText(DetailPath(folder, key), rawJson);

        public void SaveXmlZip(string folder, InvoiceKey key, byte[] zip, bool extract)
        {
            var path = XmlZipPath(folder, key);
            WriteBytes(path, zip);
            if (!extract) return;

            var target = Path.Combine(folder, key.FileStem);
            Directory.CreateDirectory(target);
            var targetFull = Path.GetFullPath(target) + Path.DirectorySeparatorChar;
            using var archive = ZipFile.OpenRead(path);
            foreach (var entry in archive.Entries)
            {
                if (string.IsNullOrEmpty(entry.Name)) continue; // thư mục
                var dest = Path.GetFullPath(Path.Combine(target, entry.Name)); // bỏ đường dẫn con, chặn zip-slip
                if (!dest.StartsWith(targetFull, StringComparison.OrdinalIgnoreCase)) continue;
                entry.ExtractToFile(dest, overwrite: true);
            }
        }

        public void SaveJson<T>(string path, T value) => WriteText(path, JsonSerializer.Serialize(value, JsonOut));

        internal static string SourceTag(InvoiceSource s) => s == InvoiceSource.Sco ? "sco" : "query";

        // Ghi ra file tạm rồi đổi tên, tránh để lại file hỏng nếu bị ngắt giữa chừng.
        private static void WriteText(string path, string text) => WriteBytes(path, Utf8NoBom.GetBytes(text));

        private static void WriteBytes(string path, byte[] data)
        {
            var tmp = path + ".tmp";
            File.WriteAllBytes(tmp, data);
            if (File.Exists(path)) File.Delete(path);
            File.Move(tmp, path);
        }
    }
}
