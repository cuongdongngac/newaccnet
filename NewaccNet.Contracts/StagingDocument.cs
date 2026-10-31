using System;
using System.Text.Json;

namespace NewaccNet.Contracts
{
    /// <summary>
    /// Bản ghi Lớp đệm (Staging Document) dạng Schema-less Document Store.
    /// Cho phép lưu trữ mọi loại dữ liệu nháp cực kỳ biến động (Hóa đơn XML, Sao kê ngân hàng,
    /// Bảng kê kho, Bảng lương...) mà không gò ép vào các cột CSDL quan hệ cố định.
    /// Dữ liệu chi tiết dạng cây phân cấp được đóng gói toàn vẹn trong PayloadJson.
    /// </summary>
    public class StagingDocument
    {
        public static readonly JsonSerializerOptions DefaultJsonOptions = new()
        {
            WriteIndented = false,
            PropertyNameCaseInsensitive = true
        };

        public string DraftId { get; set; } = Guid.NewGuid().ToString("N");
        public string DraftType { get; set; } = "PurchaseInvoice";
        public DraftStatus Status { get; set; } = DraftStatus.Imported;
        public string? SourceKind { get; set; }
        public string? SourceRef { get; set; }
        public string? Title { get; set; }
        public string PayloadJson { get; set; } = "{}";
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public int? PostedVoucherId { get; set; }

        /// <summary>
        /// Giải mã PayloadJson thành DTO phân cấp cụ thể tương ứng với từng tình huống nghiệp vụ.
        /// </summary>
        public T? As<T>(JsonSerializerOptions? options = null) where T : class
        {
            if (string.IsNullOrWhiteSpace(PayloadJson) || PayloadJson == "{}")
                return null;

            return JsonSerializer.Deserialize<T>(PayloadJson, options ?? DefaultJsonOptions);
        }

        /// <summary>
        /// Đóng gói một cây DTO phân cấp vào PayloadJson.
        /// </summary>
        public void SetPayload<T>(T payload, JsonSerializerOptions? options = null)
        {
            PayloadJson = JsonSerializer.Serialize(payload, options ?? DefaultJsonOptions);
            UpdatedAt = DateTime.Now;
        }

        /// <summary>
        /// Tạo một StagingDocument từ một DTO phân cấp bất kỳ.
        /// </summary>
        public static StagingDocument FromPayload<T>(
            string draftId,
            string draftType,
            T payload,
            DraftStatus status = DraftStatus.Imported,
            string? sourceKind = null,
            string? sourceRef = null,
            string? title = null,
            JsonSerializerOptions? options = null)
        {
            var doc = new StagingDocument
            {
                DraftId = string.IsNullOrWhiteSpace(draftId) ? Guid.NewGuid().ToString("N") : draftId,
                DraftType = draftType,
                Status = status,
                SourceKind = sourceKind,
                SourceRef = sourceRef,
                Title = title,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            doc.SetPayload(payload, options);
            return doc;
        }
    }
}
