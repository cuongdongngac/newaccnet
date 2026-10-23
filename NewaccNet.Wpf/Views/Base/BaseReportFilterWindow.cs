using System.ComponentModel;
using System.Windows;
using NewaccNet.Wpf.AppSystem.Helpers;

namespace NewaccNet.Wpf.Views.Base
{
    /// <summary>
    /// Lớp cơ sở (Base Class) cho tất cả các cửa sổ Bộ lọc Báo cáo (Report Filter Window).
    /// Tự động chuẩn hóa vòng đời:
    ///   1. Nạp danh mục & giá trị mặc định (InitializeFilterData)
    ///   2. Tự động nạp lại các tham số đã lưu từ UserSettings.json (LoadFilterState -> UserPreferencesHelper.LoadState)
    ///   3. Đồng bộ giao diện sau khi nạp tham số (OnAfterFilterStateLoaded)
    ///   4. Tự động lưu lại toàn bộ tham số vào UserSettings.json khi nhấn xem báo cáo hoặc khi đóng form (SaveFilterState)
    /// </summary>
    public class BaseReportFilterWindow : BaseWindow
    {
        /// <summary>
        /// Cờ cho biết form đang trong quá trình khởi tạo và nạp UserSettings (tránh kích hoạt sự kiện lọc chéo sớm).
        /// </summary>
        protected bool IsInitializingFilter { get; private set; } = true;

        public BaseReportFilterWindow()
        {
            this.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            this.ShowInTaskbar = false;

            this.Loaded += BaseReportFilterWindow_Loaded;
            this.Closing += BaseReportFilterWindow_Closing;
        }

        private void BaseReportFilterWindow_Loaded(object sender, RoutedEventArgs e)
        {
            IsInitializingFilter = true;
            try
            {
                InitializeFilterData();
                LoadFilterState();
                OnAfterFilterStateLoaded();
            }
            finally
            {
                IsInitializingFilter = false;
            }
        }

        private void BaseReportFilterWindow_Closing(object? sender, CancelEventArgs e)
        {
            SaveFilterState();
        }

        /// <summary>
        /// Bước 1: Lớp con ghi đè để nạp danh mục (ItemsSource) và thiết lập giá trị mặc định ban đầu.
        /// </summary>
        protected virtual void InitializeFilterData()
        {
        }

        /// <summary>
        /// Bước 2: Tự động đọc lại trạng thái các control có đặt x:Name từ UserSettings.json.
        /// Lớp con có thể ghi đè hoặc gọi base.LoadFilterState().
        /// </summary>
        protected virtual void LoadFilterState()
        {
            UserPreferencesHelper.LoadState(this);
        }

        /// <summary>
        /// Bước 3: Được gọi ngay sau khi UserSettings đã nạp xong lên các control.
        /// Lớp con ghi đè để cập nhật nhãn hiển thị hoặc lọc chéo danh sách.
        /// </summary>
        protected virtual void OnAfterFilterStateLoaded()
        {
        }

        /// <summary>
        /// Tự động lưu toàn bộ giá trị các control trên form vào UserSettings.json.
        /// </summary>
        public virtual void SaveFilterState()
        {
            UserPreferencesHelper.SaveState(this);
        }
    }
}
