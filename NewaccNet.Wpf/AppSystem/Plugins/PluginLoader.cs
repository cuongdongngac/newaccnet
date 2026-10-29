using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using NewaccNet.Contracts;

namespace NewaccNet.Wpf.AppSystem.Plugins
{
    public class LoadedPluginInfo
    {
        public PluginRecord Record { get; set; } = new();
        public IPlugin? Instance { get; set; }
        public bool IsLoaded => Instance != null;
    }

    /// <summary>
    /// Bộ nạp và quản lý vòng đời Plugin (Discovery, Load, Enable/Disable, Install, Uninstall).
    /// </summary>
    public class PluginLoader
    {
        private static PluginLoader? _instance;
        private static readonly object _lock = new();

        public static PluginLoader Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        _instance ??= new PluginLoader();
                    }
                }
                return _instance;
            }
        }

        public string PluginsDirectory { get; }
        public string ManifestFilePath { get; }

        private readonly List<LoadedPluginInfo> _plugins = new();
        public event Action? PluginStateChanged;

        public IReadOnlyList<LoadedPluginInfo> Plugins
        {
            get
            {
                lock (_plugins)
                {
                    return _plugins.ToList();
                }
            }
        }

        public PluginLoader()
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var root = System.IO.Path.Combine(appData, "NewaccNet");
            PluginsDirectory = System.IO.Path.Combine(root, "Plugins");
            ManifestFilePath = System.IO.Path.Combine(root, "plugins.json");

            System.IO.Directory.CreateDirectory(PluginsDirectory);

            // Hỗ trợ AssemblyResolve để các DLL Plugin bên ngoài luôn tìm thấy NewaccNet.Contracts.dll và các thư viện Core
            AppDomain.CurrentDomain.AssemblyResolve += (s, args) =>
            {
                var assemblyName = new AssemblyName(args.Name).Name + ".dll";
                var appPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, assemblyName);
                if (File.Exists(appPath)) return Assembly.LoadFrom(appPath);
                var pluginPath = System.IO.Path.Combine(PluginsDirectory, assemblyName);
                if (File.Exists(pluginPath))
                {
                    try
                    {
                        byte[] bytes = File.ReadAllBytes(pluginPath);
                        return Assembly.Load(bytes);
                    }
                    catch
                    {
                        return Assembly.LoadFrom(pluginPath);
                    }
                }
                return null;
            };
        }

        public void LoadAllPlugins()
        {
            lock (_plugins)
            {
                // 1. Tắt các plugin đang chạy trước đó
                foreach (var p in _plugins)
                {
                    try
                    {
                        p.Instance?.Shutdown();
                    }
                    catch { }
                }
                _plugins.Clear();
                PluginHost.Instance.ClearRegistrations();

                // 2. Đọc file manifest đã lưu trước đó (loại trừ các id nội bộ nếu có)
                var records = LoadManifest().Where(r => !string.Equals(r.Id, "plugin.ai", StringComparison.OrdinalIgnoreCase)).ToList();
                var recordDict = records.ToDictionary(x => x.Id, StringComparer.OrdinalIgnoreCase);

                // 3. Quét các file .dll trong thư mục Plugins của người dùng
                var dllFiles = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                if (System.IO.Directory.Exists(PluginsDirectory))
                {
                    foreach (var file in System.IO.Directory.GetFiles(PluginsDirectory, "*.dll"))
                    {
                        dllFiles.Add(file);
                    }
                }

                // Quét thêm thư mục Plugins cục bộ trong BaseDirectory nếu có
                var localPluginDir = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins");
                if (System.IO.Directory.Exists(localPluginDir))
                {
                    foreach (var file in System.IO.Directory.GetFiles(localPluginDir, "*.dll"))
                    {
                        dllFiles.Add(file);
                    }
                }

                foreach (var dllPath in dllFiles)
                {
                    try
                    {
                        var fileName = System.IO.Path.GetFileName(dllPath);
                        // Đọc assembly vào bộ nhớ RAM để KHÔNG KHÓA tệp trên đĩa cứng (Zero File Lock)
                        byte[] rawBytes = File.ReadAllBytes(dllPath);
                        string pdbPath = System.IO.Path.ChangeExtension(dllPath, ".pdb");
                        byte[]? pdbBytes = File.Exists(pdbPath) ? File.ReadAllBytes(pdbPath) : null;
                        var asm = pdbBytes != null ? Assembly.Load(rawBytes, pdbBytes) : Assembly.Load(rawBytes);
                        var pluginTypes = asm.GetTypes().Where(t => typeof(IPlugin).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

                        foreach (var type in pluginTypes)
                        {
                            if (Activator.CreateInstance(type) is IPlugin plugin)
                            {
                                if (!recordDict.TryGetValue(plugin.Id, out var rec))
                                {
                                    rec = new PluginRecord
                                    {
                                        Id = plugin.Id,
                                        Name = plugin.Name,
                                        Version = plugin.Version,
                                        Description = plugin.Description,
                                        Author = plugin.Author,
                                        DllFileName = fileName,
                                        IsEnabled = true,
                                        InstalledDate = DateTime.Now
                                    };
                                    recordDict[plugin.Id] = rec;
                                }
                                else
                                {
                                    // Cập nhật thông tin mới nhất từ DLL
                                    rec.Name = plugin.Name;
                                    rec.Version = plugin.Version;
                                    rec.Description = plugin.Description;
                                    rec.Author = plugin.Author;
                                    rec.DllFileName = fileName;
                                }

                                var loadedInfo = new LoadedPluginInfo
                                {
                                    Record = rec,
                                    Instance = plugin
                                };

                                if (rec.IsEnabled)
                                {
                                    try
                                    {
                                        plugin.Initialize(PluginHost.Instance);
                                    }
                                    catch (Exception initEx)
                                    {
                                        System.Diagnostics.Debug.WriteLine($"Lỗi Initialize Plugin {plugin.Name}: {initEx.Message}");
                                    }
                                }

                                _plugins.Add(loadedInfo);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Lỗi nạp Assembly {dllPath}: {ex.Message}");
                    }
                }

                var activeDllNames = dllFiles.Select(System.IO.Path.GetFileName).ToHashSet(StringComparer.OrdinalIgnoreCase);
                var validRecords = recordDict.Values.Where(r => activeDllNames.Contains(r.DllFileName)).ToList();
                SaveManifest(validRecords);
                PluginStateChanged?.Invoke();
            }
        }

        private void ReapplyRegistrations()
        {
            PluginHost.Instance.ClearRegistrations();
            foreach (var p in _plugins)
            {
                if (p.Record.IsEnabled && p.Instance != null)
                {
                    try
                    {
                        p.Instance.Initialize(PluginHost.Instance);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Lỗi khởi tạo lại Plugin {p.Record.Name}: {ex.Message}");
                    }
                }
            }
        }

        public bool IsPluginEnabled(string pluginId)
        {
            lock (_plugins)
            {
                var target = _plugins.FirstOrDefault(x => string.Equals(x.Record.Id, pluginId, StringComparison.OrdinalIgnoreCase));
                return target?.Record.IsEnabled ?? false;
            }
        }

        public void SetPluginState(string pluginId, bool isEnabled)
        {
            lock (_plugins)
            {
                var target = _plugins.FirstOrDefault(x => string.Equals(x.Record.Id, pluginId, StringComparison.OrdinalIgnoreCase));
                if (target != null)
                {
                    target.Record.IsEnabled = isEnabled;
                    ReapplyRegistrations();
                    SaveManifest(_plugins.Select(x => x.Record));
                    PluginStateChanged?.Invoke();
                }
            }
        }

        public bool InstallPlugin(string sourceFilePath)
        {
            if (!File.Exists(sourceFilePath)) return false;

            try
            {
                var destName = System.IO.Path.GetFileName(sourceFilePath);
                var destPath = System.IO.Path.Combine(PluginsDirectory, destName);

                if (!string.Equals(sourceFilePath, destPath, StringComparison.OrdinalIgnoreCase))
                {
                    File.Copy(sourceFilePath, destPath, overwrite: true);

                    // Copy kèm tệp .pdb nếu có
                    var sourcePdb = System.IO.Path.ChangeExtension(sourceFilePath, ".pdb");
                    var destPdb = System.IO.Path.Combine(PluginsDirectory, System.IO.Path.GetFileName(sourcePdb));
                    if (File.Exists(sourcePdb))
                    {
                        try { File.Copy(sourcePdb, destPdb, overwrite: true); } catch { }
                    }
                }

                LoadAllPlugins();
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi cài Plugin: {ex.Message}");
                return false;
            }
        }

        public bool UninstallPlugin(string pluginId)
        {
            lock (_plugins)
            {
                var target = _plugins.FirstOrDefault(x => string.Equals(x.Record.Id, pluginId, StringComparison.OrdinalIgnoreCase));
                if (target == null) return false;

                try
                {
                    target.Instance?.Shutdown();
                }
                catch { }

                if (!string.IsNullOrEmpty(target.Record.DllFileName))
                {
                    // 1. Xóa sạch file DLL và file liên đới (.pdb, .deps.json) trong thư mục người dùng (%APPDATA%\NewaccNet\Plugins)
                    var appDataFile = System.IO.Path.Combine(PluginsDirectory, target.Record.DllFileName);
                    DeleteFileAndRelated(appDataFile);

                    // 2. Xóa sạch trong thư mục cài đặt ứng dụng (BaseDirectory\Plugins) nếu có
                    var localFile = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins", target.Record.DllFileName);
                    DeleteFileAndRelated(localFile);
                }

                _plugins.Remove(target);
                ReapplyRegistrations();
                SaveManifest(_plugins.Select(x => x.Record));
                PluginStateChanged?.Invoke();
                return true;
            }
        }

        private static void DeleteFileAndRelated(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                var pdbPath = System.IO.Path.ChangeExtension(filePath, ".pdb");
                if (File.Exists(pdbPath))
                {
                    File.Delete(pdbPath);
                }
                var depsPath = System.IO.Path.ChangeExtension(filePath, ".deps.json");
                if (File.Exists(depsPath))
                {
                    File.Delete(depsPath);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Không thể xóa file {filePath}: {ex.Message}");
            }
        }

        private List<PluginRecord> LoadManifest()
        {
            try
            {
                if (File.Exists(ManifestFilePath))
                {
                    var json = File.ReadAllText(ManifestFilePath);
                    return JsonSerializer.Deserialize<List<PluginRecord>>(json) ?? new List<PluginRecord>();
                }
            }
            catch { }
            return new List<PluginRecord>();
        }

        private void SaveManifest(IEnumerable<PluginRecord> records)
        {
            try
            {
                var json = JsonSerializer.Serialize(records, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(ManifestFilePath, json);
            }
            catch { }
        }
    }
}
