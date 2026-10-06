using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;

namespace NewaccNet.Wpf.AppSystem.Helpers
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
            var settings = LoadAllSettings();
            if (settings.TryGetValue(key, out string val))
            {
                return val;
            }
            return null;
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
            // Ưu tiên controls của DevExpress (LookUpEdit, DateEdit, TextEdit...)
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
            PropertyInfo editValueProp = element.GetType().GetProperty("EditValue");
            if (editValueProp != null)
            {
                Type targetType = editValueProp.PropertyType;
                Type underlyingType = Nullable.GetUnderlyingType(targetType) ?? targetType;

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
            if (targetType == typeof(string)) return strVal;
            if (targetType == typeof(DateTime)) return DateTime.Parse(strVal);
            if (targetType == typeof(int)) return int.Parse(strVal);
            if (targetType == typeof(double)) return double.Parse(strVal);
            if (targetType == typeof(decimal)) return decimal.Parse(strVal);
            if (targetType == typeof(bool)) return bool.Parse(strVal);

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


