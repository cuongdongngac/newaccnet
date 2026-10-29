using System;

namespace NewaccNet.Contracts
{
    /// <summary>
    /// Kênh phát sóng và tiếp nhận sự kiện dùng chung (Event Bus).
    /// Giúp Core và các Plugin giao tiếp bất đồng bộ, lỏng lẻo (Decoupled),
    /// tương tự như Hook/EventDispatcher trong Drupal.
    /// Có thể hoán đổi sang RabbitMQ, Kafka hoặc Redis trong tương lai.
    /// </summary>
    public interface IEventBus
    {
        /// <summary>
        /// Phát đi một sự kiện vào hệ thống
        /// </summary>
        void Publish<TEvent>(TEvent @event);

        /// <summary>
        /// Đăng ký lắng nghe sự kiện
        /// </summary>
        void Subscribe<TEvent>(Action<TEvent> handler);

        /// <summary>
        /// Hủy đăng ký lắng nghe sự kiện
        /// </summary>
        void Unsubscribe<TEvent>(Action<TEvent> handler);
    }
}
