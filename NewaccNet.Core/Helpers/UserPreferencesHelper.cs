using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;

namespace NewaccNet.Core.Helpers
{
    public static class UserPreferencesHelper
    {
        private static string GetSettingsFilePath()
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string folder = Path.Combine(appData, "NewaccNet");
            if (!System.IO.Directory.Exists(folder))
            {
                System.IO.Directory.CreateDirectory(folder);
            }
            return Path.Combine(folder, "UserSettings.json");
        }

        private static Dictionary<string, string> LoadAllSettings()
        {
            string path = GetSettingsFilePath();
            if (File.Exists(path))
            {
                try
                {
                    string json = File.ReadAllText(path);
                    return JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? new Dictionary<string, string>();
                }
                catch { }
            }
            return new Dictionary<string, string>();
        }

        public static string GetSetting(string key)
        {
            if (string.Equals(key, "CompanyName", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    var cfg = DbConfigService.Current;
                    return cfg?.CompanyName ?? string.Empty;
                }
                catch { }
                return string.Empty;
            }

            var settings = LoadAllSettings();
            if (settings.TryGetValue(key, out string val))
            {
                return val;
            }
            return null;
        }

        public static void SetSetting(string key, string value)
        {
            if (string.Equals(key, "CompanyName", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    var cfg = DbConfigService.LoadDbConfig();
                    cfg.CompanyName = value;
                    DbConfigService.SaveDbConfig(cfg);
                }
                catch { }
            }

            var settings = LoadAllSettings();
            settings[key] = value;
            SaveAllSettings(settings);
        }

        private static void SaveAllSettings(Dictionary<string, string> settings)
        {
            try
            {
                string json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(GetSettingsFilePath(), json);
            }
            catch { }
        }

        public static void SaveState(Window window)
        {
            if (window == null) return;
            var settings = LoadAllSettings();
            string windowName = window.GetType().Name;

            var elements = FindLogicalChildren<FrameworkElement>(window).Where(x => !string.IsNullOrEmpty(x.Name));

            foreach (var element in elements)
            {
                string key = $"Shared_{element.Name}";
                object val = GetControlValue(element);
                if (val != null)
                {
                    if (val is DateTime dt)
                        settings[key] = dt.ToString("O"); // ISO 8601
                    else
                        settings[key] = val.ToString();
                }
            }

            SaveAllSettings(settings);
        }

        public static void LoadState(Window window)
        {
            if (window == null) return;
            var settings = LoadAllSettings();
            string windowName = window.GetType().Name;

            var elements = FindLogicalChildren<FrameworkElement>(window).Where(x => !string.IsNullOrEmpty(x.Name));

            foreach (var element in elements)
            {
                string key = $"Shared_{element.Name}";
                if (settings.TryGetValue(key, out string strVal) && !string.IsNullOrEmpty(strVal))
                {
                    try
                    {
                        SetControlValue(element, strVal);
                    }
                    catch { } // Ignore parse errors for individual controls
                }
            }
        }

        private static object GetControlValue(FrameworkElement element)
        {
            if (element is DevExpress.Xpf.Editors.DateEdit de)
            {
                return de.DateTime;
            }

            if (element is DevExpress.Xpf.Editors.CheckEdit ce)
            {
                return ce.IsChecked;
            }

            if (element is RadioButton rb)
            {
                return rb.IsChecked;
            }

            if (element is DevExpress.Xpf.Editors.ComboBoxEdit cbe
                && cbe.StyleSettings is DevExpress.Xpf.Editors.CheckedComboBoxStyleSettings)
            {
                if (cbe.EditValue is System.Collections.IEnumerable list and not string)
                {
                    var items = new List<string>();
                    foreach (var item in list)
                    {
                        string? code = item?.ToString()?.Trim();
                        if (!string.IsNullOrWhiteSpace(code))
                            items.Add(code);
                    }
                    int totalCount = (cbe.ItemsSource as System.Collections.ICollection)?.Count ?? 0;
                    if (totalCount > 0 && items.Count >= totalCount)
                        return "*";
                    return string.Join(",", items);
                }
                return string.Empty;
            }

            // Ưu tiên controls của DevExpress (LookUpEdit, ComboBoxEdit, TextEdit...)
            PropertyInfo editValueProp = element.GetType().GetProperty("EditValue");
            if (editValueProp != null)
            {
                return editValueProp.GetValue(element);
            }

            // Fallback controls chuẩn WPF
            if (element is TextBox tb) return tb.Text;
            if (element is CheckBox cb) return cb.IsChecked;
            if (element is DatePicker dp) return dp.SelectedDate;
            if (element is ComboBox combo) return combo.SelectedValue ?? combo.Text;
            
            return null;
        }

        private static void SetControlValue(FrameworkElement element, string strVal)
        {
            if (element is DevExpress.Xpf.Editors.DateEdit de)
            {
                if (DateTime.TryParse(strVal, out DateTime dt))
                {
                    de.DateTime = dt;
                }
                return;
            }

            if (element is DevExpress.Xpf.Editors.CheckEdit ce)
            {
                if (bool.TryParse(strVal, out bool b))
                {
                    ce.IsChecked = b;
                }
                return;
            }

            if (element is RadioButton rb)
            {
                if (bool.TryParse(strVal, out bool b))
                {
                    rb.IsChecked = b;
                }
                return;
            }

            if (element is DevExpress.Xpf.Editors.ComboBoxEdit cbe
                && cbe.StyleSettings is DevExpress.Xpf.Editors.CheckedComboBoxStyleSettings)
            {
                if (strVal == "*")
                {
                    cbe.SelectAll();
                }
                else
                {
                    var parts = strVal.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                                      .Select(x => (object)x.Trim())
                                      .ToList();
                    if (parts.Count > 0)
                        cbe.EditValue = parts;
                    else
                        cbe.SelectAll();
                }
                return;
            }

            PropertyInfo editValueProp = element.GetType().GetProperty("EditValue");
            if (editValueProp != null)
            {
                Type targetType = editValueProp.PropertyType;
                Type underlyingType = Nullable.GetUnderlyingType(targetType) ?? targetType;

                // Nếu EditValue có kiểu object nhưng đang có giá trị cụ thể (VD: Enum trong ComboBoxEdit), ưu tiên convert theo kiểu của giá trị hiện tại
                if (underlyingType == typeof(object))
                {
                    object? currentVal = editValueProp.GetValue(element);
                    if (currentVal != null)
                    {
                        underlyingType = currentVal.GetType();
                    }
                }

                object convertedValue = ConvertString(strVal, underlyingType);
                editValueProp.SetValue(element, convertedValue);
                return;
            }

            if (element is TextBox tb) tb.Text = strVal;
            else if (element is CheckBox cb && bool.TryParse(strVal, out bool b)) cb.IsChecked = b;
            else if (element is DatePicker dp && DateTime.TryParse(strVal, out DateTime dt)) dp.SelectedDate = dt;
        }

        private static object ConvertString(string strVal, Type targetType)
        {
            if (targetType == typeof(string) || targetType == typeof(object)) return strVal;
            if (targetType == typeof(DateTime)) return DateTime.Parse(strVal);
            if (targetType == typeof(int)) return int.Parse(strVal);
            if (targetType == typeof(double)) return double.Parse(strVal);
            if (targetType == typeof(decimal)) return decimal.Parse(strVal);
            if (targetType == typeof(bool)) return bool.Parse(strVal);
            if (targetType.IsEnum && Enum.TryParse(targetType, strVal, true, out object? enumVal))
            {
                return enumVal!;
            }

            var converter = TypeDescriptor.GetConverter(targetType);
            if (converter != null && converter.CanConvertFrom(typeof(string)))
            {
                return converter.ConvertFromString(strVal);
            }

            return strVal;
        }

        private static IEnumerable<T> FindLogicalChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                foreach (object child in LogicalTreeHelper.GetChildren(depObj))
                {
                    if (child is DependencyObject depChild)
                    {
                        if (child is T t)
                        {
                            yield return t;
                        }
                        foreach (T childOfChild in FindLogicalChildren<T>(depChild))
                        {
                            yield return childOfChild;
                        }
                    }
                }
            }
        }
    }
}


