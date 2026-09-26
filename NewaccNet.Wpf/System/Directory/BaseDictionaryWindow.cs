using System.Windows;
using DevExpress.Xpf.Core;

namespace NewaccNet.Wpf.AppSystem.Directory
{
    public class BaseDictionaryWindow : NewaccNet.Wpf.Views.Base.BaseWindow
    {
        // Thay thế vòng lặp foreach trong WinForms
        // Trong WPF, vô hiệu hóa form sẽ tự động làm mờ và vô hiệu hóa tất cả các control con
        public bool ControlEnabled 
        {
            get { return this.IsEnabled; }
            set { this.IsEnabled = value; }
        }

        public BaseDictionaryWindow()
        {
            // Không tự động gọi LoadData nữa để quy trình rõ ràng hơn (gọi thủ công từ ngoài)
        }

        // Các hàm ảo (virtual) để các form con ghi đè (override)
        public virtual void SaveRecord() { }
        public virtual void DeleteRecord() { }
        public virtual void PrintRecord() { }
        public virtual void BindingData() { }
        public virtual void ReBinding() { }
        public virtual void NewRecord() { }
        public override void LoadData() { }
        public virtual void WriteLog(string log) { }

        // --- Các hàm hỗ trợ chung cho mọi màn hình ---
        
        // Hiển thị vòng quay tải dữ liệu (Sử dụng DevExpress SplashScreenManager)
        public void ShowLoading(string message = "Đang tải dữ liệu...")
        {
            var manager = DevExpress.Xpf.Core.SplashScreenManager.CreateWaitIndicator();
            manager.Show();
        }

        // Ẩn vòng quay tải dữ liệu
        public void HideLoading()
        {
            DevExpress.Xpf.Core.SplashScreenManager.CloseAll();
        }
    }
}

