using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace NewaccNet.TaxInvoice
{
    public sealed class TaxInvoiceFileStore
    {
        internal static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

        private static readonly UTF8Encoding Utf8NoBom = new UTF8Encoding(false);

        public TaxInvoiceFileStore(string rootFolder)
        {
            RootFolder = Path.GetFullPath(rootFolder);
        }

        public string RootFolder { get; }

        public string GetMonthFolder(string taxCode, InvoiceDirection direction, DateRange range)
        {
            var dir = Path.Combine(RootFolder, taxCode, direction == InvoiceDirection.Purchase ? "purchase" : "sold", range.MonthLabel);
            Directory.CreateDirectory(dir);
            return dir;
        }

        public void ClearListPages(string folderPath, InvoiceSource source)
        {
            if (!Directory.Exists(folderPath)) return;
            foreach (var filePath in Directory.GetFiles(folderPath, "list_" + GetSourceTag(source) + "_*.json"))
            {
                File.Delete(filePath);
            }
        }

        public void SaveListPage(string folderPath, InvoiceSource source, int pageNumber, string rawJson)
        {
            var fileName = "list_" + GetSourceTag(source) + "_" + pageNumber.ToString("000") + ".json";
            WriteText(Path.Combine(folderPath, fileName), rawJson);
        }

        public string GetDetailPath(string folderPath, InvoiceKey key) => Path.Combine(folderPath, key.FileStem + ".json");
        public string GetXmlZipPath(string folderPath, InvoiceKey key) => Path.Combine(folderPath, key.FileStem + ".zip");

        public void SaveDetail(string folderPath, InvoiceKey key, string rawJson) => WriteText(GetDetailPath(folderPath, key), rawJson);

        public void SaveXmlZip(string folderPath, InvoiceKey key, byte[] zipBytes, bool extract)
        {
            var zipPath = GetXmlZipPath(folderPath, key);
            WriteBytes(zipPath, zipBytes);
            if (!extract) return;

            var targetDir = Path.Combine(folderPath, key.FileStem);
            Directory.CreateDirectory(targetDir);
            var targetFullDir = Path.GetFullPath(targetDir) + Path.DirectorySeparatorChar;

            using var archive = ZipFile.OpenRead(zipPath);
            foreach (var entry in archive.Entries)
            {
                if (string.IsNullOrEmpty(entry.Name)) continue;
                var destinationPath = Path.GetFullPath(Path.Combine(targetDir, entry.Name));
                if (!destinationPath.StartsWith(targetFullDir, StringComparison.OrdinalIgnoreCase)) continue;
                entry.ExtractToFile(destinationPath, overwrite: true);
            }
        }

        public void SaveJson<T>(string filePath, T value) => WriteText(filePath, JsonSerializer.Serialize(value, JsonOptions));

        internal static string GetSourceTag(InvoiceSource source) => source == InvoiceSource.Sco ? "sco" : "query";

        private static void WriteText(string filePath, string text) => WriteBytes(filePath, Utf8NoBom.GetBytes(text));

        private static void WriteBytes(string filePath, byte[] data)
        {
            var tempPath = filePath + ".tmp";
            File.WriteAllBytes(tempPath, data);
            if (File.Exists(filePath)) File.Delete(filePath);
            File.Move(tempPath, filePath);
        }
    }
}
