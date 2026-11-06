using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using DevExpress.Xpf.Grid;
using NewaccNet.AI.Contracts;
using NewaccNet.AI.Services;
using NewaccNet.Wpf.Views.Base;

namespace NewaccNet.Wpf.AppSystem.AI
{
    public partial class OpenRouterModelPickerWindow : BaseWindow
    {
        private List<OpenRouterModelDto> _allModels = new();
        private readonly string? _apiKey;
        private readonly string? _baseUrl;
        private bool _isUpdatingFromSelection = false;

        public string SelectedModelId { get; private set; } = string.Empty;

        public OpenRouterModelPickerWindow(string currentModelId = "", bool filterVisionOnly = false, string? apiKey = null, string? baseUrl = null)
        {
            InitializeComponent();
            SelectedModelId = currentModelId;
            _apiKey = apiKey;
            _baseUrl = baseUrl;

            if (filterVisionOnly)
            {
                chkVisionOnly.IsChecked = true;
            }

            txtManualId.Text = currentModelId;
            this.Loaded += async (_, _) => await LoadModelsAsync();
        }

        private async Task LoadModelsAsync(bool forceRefresh = false)
        {
            try
            {
                btnRefresh.IsEnabled = false;
                txtSearch.NullText = "⏳ Đang kết nối tới OpenRouter để tải danh sách mô hình...";

                _allModels = await OpenRouterModelService.FetchModelsAsync(_apiKey, _baseUrl, forceRefresh);

                // Điền danh sách nhà cung cấp (Providers)
                var providers = _allModels
                    .Select(m => m.Provider)
                    .Where(p => !string.IsNullOrWhiteSpace(p))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .OrderBy(p => p)
                    .ToList();

                cboProvider.Items.Clear();
                cboProvider.Items.Add("Tất cả hãng");
                foreach (var p in providers)
                {
                    cboProvider.Items.Add(p);
                }
                cboProvider.SelectedIndex = 0;

                txtSearch.NullText = "🔍 Tìm kiếm mô hình: gemini, claude, deepseek, gpt, flash...";
                ApplyFilter();

                // Tự động tìm và chọn model hiện tại
                if (!string.IsNullOrWhiteSpace(SelectedModelId))
                {
                    var current = _allModels.FirstOrDefault(m => string.Equals(m.Id, SelectedModelId, StringComparison.OrdinalIgnoreCase));
                    if (current != null)
                    {
                        gridModels.SelectedItem = current;
                        UpdateSelectedDisplay(current);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi load model: {ex.Message}");
            }
            finally
            {
                btnRefresh.IsEnabled = true;
            }
        }

        private void FilterChanged(object sender, EventArgs e)
        {
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            if (_allModels == null || _allModels.Count == 0) return;

            string query = txtSearch.Text?.Trim().ToLowerInvariant() ?? "";
            string[] words = query.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            string selectedProvider = cboProvider.EditValue?.ToString() ?? "";
            bool filterProvider = !string.IsNullOrWhiteSpace(selectedProvider) && selectedProvider != "Tất cả hãng";

            bool freeOnly = chkFreeOnly.IsChecked == true;
            bool visionOnly = chkVisionOnly.IsChecked == true;
            bool jsonOnly = chkJsonOnly.IsChecked == true;

            var filtered = _allModels.Where(m =>
            {
                if (filterProvider && !string.Equals(m.Provider, selectedProvider, StringComparison.OrdinalIgnoreCase))
                    return false;

                if (freeOnly && !m.IsFree)
                    return false;

                if (visionOnly && !m.SupportsVision)
                    return false;

                if (jsonOnly && !m.SupportsJson)
                    return false;

                if (words.Length > 0)
                {
                    string target = $"{m.Id} {m.Name} {m.Provider} {m.Description}".ToLowerInvariant();
                    return words.All(w => target.Contains(w));
                }

                return true;
            }).ToList();

            gridModels.ItemsSource = filtered;
            lblCount.Text = $"{filtered.Count:N0} / {_allModels.Count:N0} model";
        }

        private void GridModels_SelectedItemChanged(object sender, SelectedItemChangedEventArgs e)
        {
            if (gridModels.SelectedItem is OpenRouterModelDto model)
            {
                _isUpdatingFromSelection = true;
                UpdateSelectedDisplay(model);
                txtManualId.Text = model.Id;
                _isUpdatingFromSelection = false;
            }
        }

        private void UpdateSelectedDisplay(OpenRouterModelDto model)
        {
            txtSelectedName.Text = model.Name;
            txtSelectedId.Text = $"Mã API: {model.Id}";
            string info = $"• Hãng: {model.Provider.ToUpper()} | Ngữ cảnh: {model.DisplayContext} | Chi phí: {model.DisplayPrice}\n• Đặc điểm: {model.DisplayBadge}";
            if (!string.IsNullOrWhiteSpace(model.Description))
            {
                info += $"\n• Mô tả: {model.Description}";
            }
            txtSelectedDetails.Text = info;
        }

        private void TxtManualId_EditValueChanged(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e)
        {
            if (_isUpdatingFromSelection) return;

            string id = txtManualId.Text?.Trim() ?? "";
            SelectedModelId = id;
            var matched = _allModels?.FirstOrDefault(m => string.Equals(m.Id, id, StringComparison.OrdinalIgnoreCase));
            if (matched != null)
            {
                UpdateSelectedDisplay(matched);
            }
            else
            {
                txtSelectedName.Text = string.IsNullOrWhiteSpace(id) ? "Chưa chọn mô hình nào" : id;
                txtSelectedId.Text = string.IsNullOrWhiteSpace(id) ? "" : $"Mã tùy chỉnh: {id}";
                txtSelectedDetails.Text = string.IsNullOrWhiteSpace(id) ? "" : "Mô hình tùy chỉnh do người dùng nhập trực tiếp.";
            }
        }

        private async void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            await LoadModelsAsync(forceRefresh: true);
        }

        private void TableView_RowDoubleClick(object sender, RowDoubleClickEventArgs e)
        {
            SelectAndClose();
        }

        private void BtnSelect_Click(object sender, RoutedEventArgs e)
        {
            SelectAndClose();
        }

        private void SelectAndClose()
        {
            string id = txtManualId.Text?.Trim() ?? "";
            if (string.IsNullOrWhiteSpace(id) && gridModels.SelectedItem is OpenRouterModelDto m)
            {
                id = m.Id;
            }

            if (string.IsNullOrWhiteSpace(id))
            {
                DevExpress.Xpf.Core.ThemedMessageBox.Show("Thông báo", "Vui lòng chọn một mô hình AI từ danh sách hoặc nhập mã ID.", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            SelectedModelId = id;
            this.DialogResult = true;
            this.Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
