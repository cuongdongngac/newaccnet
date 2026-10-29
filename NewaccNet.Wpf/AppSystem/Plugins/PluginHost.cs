using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using NewaccNet.Contracts;
using NewaccNet.Wpf.AppSystem.Helpers;

namespace NewaccNet.Wpf.AppSystem.Plugins
{
    /// <summary>
    /// Triển khai IPluginContext cho Core của NewaccNet.
    /// Cung cấp thông tin môi trường, đăng ký dịch vụ, phát sóng sự kiện và nhận các nút bấm giao diện từ Plugin.
    /// </summary>
    public class PluginHost : IPluginContext
    {
        private static PluginHost? _instance;
        private static readonly object _lock = new();

        public static PluginHost Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        _instance ??= new PluginHost();
                    }
                }
                return _instance;
            }
        }

        private readonly ConcurrentDictionary<Type, object> _services = new();
        private readonly List<PluginRibbonAction> _registeredRibbonActions = new();

        public string DatabasePath => ConfigManager.Current.AccessFilePath;
        public string CompanyName => DbConfigService.Current.CompanyName;
        public string TaxCode => DbConfigService.Current.TaxCode;
        public int WorkingYear => DbConfigService.Current.AccountingYear > 0 ? DbConfigService.Current.AccountingYear : DateTime.Today.Year;

        public IEventBus EventBus => InMemoryEventBus.Instance;

        public IReadOnlyList<PluginRibbonAction> RegisteredRibbonActions
        {
            get
            {
                lock (_registeredRibbonActions)
                {
                    return _registeredRibbonActions.ToArray();
                }
            }
        }

        public void RegisterRibbonAction(PluginRibbonAction action)
        {
            if (action == null) return;
            lock (_registeredRibbonActions)
            {
                _registeredRibbonActions.Add(action);
            }
        }

        public void RegisterService<TService>(TService implementation) where TService : class
        {
            if (implementation == null) return;
            _services[typeof(TService)] = implementation;
        }

        public TService? GetService<TService>() where TService : class
        {
            if (_services.TryGetValue(typeof(TService), out var svc) && svc is TService typedSvc)
            {
                return typedSvc;
            }
            return null;
        }

        public void ClearRegistrations()
        {
            lock (_registeredRibbonActions)
            {
                _registeredRibbonActions.Clear();
            }
            _services.Clear();
        }
    }
}
