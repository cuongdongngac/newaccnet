using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using DocumentFormat.OpenXml.Packaging;
using NewaccNet.Contracts;

namespace NewaccNet.Office
{
    /// <summary>
    /// Đóng gói và bóc tách VoucherDraft JSON vào CustomXmlPart bên trong tệp OpenXML (.xlsx).
    /// Một file vừa có trang tính cho người nhìn, vừa có dữ liệu chuẩn cho máy xử lý.
    /// Hoạt động 100% trên MemoryStream (Zero-Disk I/O).
    /// </summary>
    public static class DraftWorkbookPackage
    {
        private const string DraftXmlPartUri = "/customXml/voucherDraft.xml";
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            WriteIndented = false,
            PropertyNameCaseInsensitive = true
        };

        /// <summary>
        /// Nhúng chuỗi JSON của VoucherDraft vào CustomXmlPart của tệp Excel.
        /// </summary>
        public static void EmbedDraftJson(SpreadsheetDocument document, VoucherDraft draft)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));
            if (draft == null) throw new ArgumentNullException(nameof(draft));

            var mainPart = document.WorkbookPart;
            if (mainPart == null) return;

            // Xóa CustomXmlPart cũ nếu đã có
            var existingParts = mainPart.CustomXmlParts.ToList();
            foreach (var part in existingParts)
            {
                mainPart.DeletePart(part);
            }

            // Tạo CustomXmlPart mới
            var customXmlPart = mainPart.AddCustomXmlPart(CustomXmlPartType.CustomXml);
            string json = JsonSerializer.Serialize(draft, JsonOptions);
            byte[] bytes = Encoding.UTF8.GetBytes(json);

            using var partStream = customXmlPart.GetStream(FileMode.Create, FileAccess.Write);
            partStream.Write(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// Bóc tách VoucherDraft từ CustomXmlPart của tệp Excel.
        /// </summary>
        public static VoucherDraft? ExtractDraftJson(SpreadsheetDocument document)
        {
            if (document == null) return null;

            var mainPart = document.WorkbookPart;
            if (mainPart == null) return null;

            foreach (var customPart in mainPart.CustomXmlParts)
            {
                try
                {
                    using var stream = customPart.GetStream(FileMode.Open, FileAccess.Read);
                    using var reader = new StreamReader(stream, Encoding.UTF8);
                    string content = reader.ReadToEnd();

                    if (!string.IsNullOrWhiteSpace(content) && content.TrimStart().StartsWith("{"))
                    {
                        var draft = JsonSerializer.Deserialize<VoucherDraft>(content, JsonOptions);
                        if (draft != null) return draft;
                    }
                }
                catch { }
            }

            return null;
        }
    }
}
