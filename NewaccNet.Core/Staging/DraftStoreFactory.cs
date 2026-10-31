using System;

namespace NewaccNet.Core.Staging
{
    public enum StagingStorageMode
    {
        /// <summary>
        /// Lưu trữ dạng tệp JSON theo tháng tại thư mục AppData/NewaccNet/Staging/.
        /// Thích hợp cho CSDL cục bộ MS Access hoặc khi muốn lưu trữ độc lập dạng file.
        /// </summary>
        JsonFile = 0,

        /// <summary>
        /// Lưu trữ vào bảng StagingDraft trong CSDL đang kết nối (SQL Server, PostgreSQL, v.v.).
        /// Thích hợp cho nhiều máy trạm trong mạng cùng truy cập.
        /// </summary>
        Database = 1
    }

    /// <summary>
    /// Nhà máy cung cấp IDraftStore dựa trên cấu hình người dùng thiết lập.
    /// Minh bạch, không tự động âm thầm chuyển đổi.
    /// </summary>
    public static class DraftStoreFactory
    {
        public static StagingStorageMode ConfiguredMode { get; set; } = StagingStorageMode.JsonFile;

        public static IDraftStore Create(StagingStorageMode? mode = null)
        {
            var targetMode = mode ?? ConfiguredMode;
            return targetMode == StagingStorageMode.Database
                ? new DbDraftStore()
                : new JsonFileDraftStore();
        }
    }
}
