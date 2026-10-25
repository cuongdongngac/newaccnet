using System;
using System.Windows;
using System.Windows.Media;
using NewaccNet.AI.Configuration;
using NewaccNet.AI.Gateway;
using NewaccNet.Wpf.AppSystem;
using NewaccNet.Wpf.Views.Base;

namespace NewaccNet.Wpf.AppSystem.AI
{
    public partial class AiConfigWindow : BaseWindow
    {
        public AiConfigWindow()
        {
            InitializeComponent();
            LoadConfigToUI();
        }

        private void LoadConfigToUI()
        {
            var config = ConfigManager.Current;
            txtApiKey.Text = config.OpenRouterApiKey ?? string.Empty;
            txtBaseUrl.Text = string.IsNullOrWhiteSpace(config.OpenRouterBaseUrl) 
                ? "https://openrouter.ai/api/v1" 
                : config.OpenRouterBaseUrl;
            
            cboChatModel.EditValue = string.IsNullOrWhiteSpace(config.OpenRouterChatModel)
                ? "google/gemini-2.5-flash"
                : config.OpenRouterChatModel;

            cboVisionModel.EditValue = string.IsNullOrWhiteSpace(config.OpenRouterVisionModel)
                ? "google/gemini-2.5-flash"
                : config.OpenRouterVisionModel;

            cboAnalysisModel.EditValue = string.IsNullOrWhiteSpace(config.OpenRouterAnalysisModel)
                ? "google/gemini-2.5-pro"
                : config.OpenRouterAnalysisModel;
        }

        private async void BtnTest_Click(object sender, RoutedEventArgs e)
        {
            string apiKey = txtApiKey.Text?.Trim() ?? string.Empty;
            string baseUrl = txtBaseUrl.Text?.Trim() ?? string.Empty;
            string chatModel = cboChatModel.EditValue?.ToString() ?? "google/gemini-2.5-flash";

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                lblStatus.Text = "⚠ Vui lòng nhập OpenRouter API Key trước khi kiểm tra.";
                lblStatus.Foreground = Brushes.OrangeRed;
                return;
            }

            try
            {
                btnTest.IsEnabled = false;
                lblStatus.Text = "⏳ Đang kết nối tới OpenRouter API gateway...";
                lblStatus.Foreground = Brushes.SteelBlue;

                var options = new OpenRouterOptions
                {
                    ApiKey = apiKey,
                    BaseUrl = string.IsNullOrWhiteSpace(baseUrl) ? "https://openrouter.ai/api/v1" : baseUrl,
                    Model = chatModel
                };

                using var gateway = new OpenRouterGateway(options);
                bool ok = await gateway.TestConnectionAsync();

                if (ok)
                {
                    lblStatus.Text = "✅ Kết nối OpenRouter API thành công!";
                    lblStatus.Foreground = Brushes.Green;
                }
                else
                {
                    lblStatus.Text = "❌ Kết nối thất bại. Vui lòng kiểm tra lại API Key.";
                    lblStatus.Foreground = Brushes.Red;
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"❌ Lỗi kết nối: {ex.Message}";
                lblStatus.Foreground = Brushes.Red;
            }
            finally
            {
                btnTest.IsEnabled = true;
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            var config = ConfigManager.Current;
            config.OpenRouterApiKey = txtApiKey.Text?.Trim() ?? string.Empty;
            config.OpenRouterBaseUrl = txtBaseUrl.Text?.Trim() ?? "https://openrouter.ai/api/v1";
            config.OpenRouterChatModel = cboChatModel.EditValue?.ToString() ?? "google/gemini-2.5-flash";
            config.OpenRouterVisionModel = cboVisionModel.EditValue?.ToString() ?? "google/gemini-2.5-flash";
            config.OpenRouterAnalysisModel = cboAnalysisModel.EditValue?.ToString() ?? "google/gemini-2.5-pro";

            ConfigManager.SaveConfig();
            MessageBox.Show("Đã lưu cấu hình AI thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            this.DialogResult = true;
            this.Close();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
