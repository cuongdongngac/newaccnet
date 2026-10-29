using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace NewaccNet.Contracts
{
    /// <summary>
    /// Triển khai Event Bus nội bộ trong bộ nhớ RAM (In-Memory Event Bus).
    /// Siêu nhanh (0 latency), thread-safe, không tốn tài nguyên hạ tầng.
    /// </summary>
    public class InMemoryEventBus : IEventBus
    {
        private static InMemoryEventBus? _instance;
        private static readonly object _lock = new();

        public static InMemoryEventBus Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        _instance ??= new InMemoryEventBus();
                    }
                }
                return _instance;
            }
        }

        private readonly ConcurrentDictionary<Type, List<Delegate>> _handlers = new();

        public void Publish<TEvent>(TEvent @event)
        {
            if (@event == null) return;
            var eventType = typeof(TEvent);

            if (_handlers.TryGetValue(eventType, out var list))
            {
                Delegate[] copiedHandlers;
                lock (list)
                {
                    copiedHandlers = list.ToArray();
                }

                foreach (var handler in copiedHandlers)
                {
                    try
                    {
                        if (handler is Action<TEvent> typedHandler)
                        {
                            typedHandler(@event);
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"[EventBus Exception in {eventType.Name}]: {ex.Message}");
                    }
                }
            }
        }

        public void Subscribe<TEvent>(Action<TEvent> handler)
        {
            if (handler == null) return;
            var eventType = typeof(TEvent);

            var list = _handlers.GetOrAdd(eventType, _ => new List<Delegate>());
            lock (list)
            {
                if (!list.Contains(handler))
                {
                    list.Add(handler);
                }
            }
        }

        public void Unsubscribe<TEvent>(Action<TEvent> handler)
        {
            if (handler == null) return;
            var eventType = typeof(TEvent);

            if (_handlers.TryGetValue(eventType, out var list))
            {
                lock (list)
                {
                    list.Remove(handler);
                }
            }
        }
    }
}
