using System;

namespace NewaccNet.Contracts
{
    /// <summary>
    /// Ngữ cảnh làm việc do Core cấp cho Plugin khi khởi tạo.
    /// Giúp Plugin nhận biết môi trường và đăng ký các dịch vụ, nút bấm giao diện.
    /// </summary>
    public interface IPluginContext
    {
        // ── 1. Thông tin Môi trường Kế toán ──────────────────────────
        string DatabasePath { get; }
        string CompanyName { get; }
        string TaxCode { get; }
        int WorkingYear { get; }

        // ── 2. Cổng Đăng ký Giao diện ────────────────────────────────
        void RegisterRibbonAction(PluginRibbonAction action);

        // ── 3. Túi Dịch vụ dùng chung (Service Container) ────────────
        void RegisterService<TService>(TService implementation) where TService : class;
        TService? GetService<TService>() where TService : class;

        // ── 4. Kênh Sự kiện (Event Bus) ──────────────────────────────
        IEventBus EventBus { get; }
    }
}
