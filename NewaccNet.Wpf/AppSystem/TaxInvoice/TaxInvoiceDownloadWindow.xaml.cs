using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Microsoft.Win32;
using NewaccNet.TaxInvoice;
using NewaccNet.Wpf.Views.Base;

namespace NewaccNet.Wpf.AppSystem.TaxInvoice
{
    public partial class TaxInvoiceDownloadWindow : BaseDetailWindow
    {
        private readonly TaxInvoiceDbService? _dbService;
        private TaxInvoiceClient _client = new TaxInvoiceClient();
        private CancellationTokenSource? _cancellationTokenSource;

        public TaxInvoiceDownloadWindow(string? databasePath = null)
        {
            InitializeComponent();

            // Khởi tạo bộ giải Captcha AI Vision Fallback
            _client.CaptchaSolver = new NewaccNet.AI.Services.AiCaptchaSolver();

            if (!string.IsNullOrEmpty(databasePath) && System.IO.File.Exists(databasePath))
            {
                _dbService = new TaxInvoiceDbService(databasePath);
            }

            var today = DateTime.Today;
            dtFrom.DateTime = new DateTime(today.Year, today.Month, 1);
            dtTo.DateTime = new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month));

            var defaultRootPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "NewaccNet_Invoices");
            txtFolder.Text = defaultRootPath;

            this.Loaded += TaxInvoiceDownloadWindow_Loaded;
        }

        private async void TaxInvoiceDownloadWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (_dbService != null)
            {
                try
                {
                    var savedConfig = await _dbService.GetAccountConfigAsync(txtTaxCode.Text.Trim());
                    if (savedConfig != null)
                    {
                        txtTaxCode.Text = savedConfig.TaxCode;
                        txtPassword.Password = DataEncryptionHelper.Decrypt(savedConfig.EncryptedPassword ?? "");
                        if (!string.IsNullOrEmpty(savedConfig.Token))
                        {
                            _client.Token = new TaxInvoiceToken(savedConfig.Token, savedConfig.LastSyncDate ?? DateTime.Now);
                            UpdateTokenStatusDisplay(_client.Token.GetStatus());
                        }
                    }
                }
                catch { }
            }

            if (_client.Token == null)
            {
                lblTokenStatus.Text = "Chưa có Token đăng nhập.";
                lblTokenStatus.Foreground = new SolidColorBrush(Color.FromRgb(211, 47, 47));
            }
        }

        private void UpdateTokenStatusDisplay(TokenStatusInfo status)
        {
            lblTokenStatus.Text = status.StatusMessage;
            if (status.IsExpired || !status.HasToken)
            {
                lblTokenStatus.Foreground = new SolidColorBrush(Color.FromRgb(211, 47, 47));
            }
            else
            {
                lblTokenStatus.Foreground = new SolidColorBrush(Color.FromRgb(46, 125, 50));
            }
        }

        private async void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            var taxCode = txtTaxCode.Text.Trim();
            var password = txtPassword.Password.Trim();

            if (string.IsNullOrEmpty(taxCode) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Vui lòng nhập Mã số thuế và Mật khẩu Cổng Thuế.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            btnLogin.IsEnabled = false;
            lblTokenStatus.Text = "Đang kết nối cổng Thuế & giải captcha...";
            lblTokenStatus.Foreground = new SolidColorBrush(Color.FromRgb(21, 101, 192));

            try
            {
                var token = await _client.LoginAutoAsync(taxCode, password);
                UpdateTokenStatusDisplay(token.GetStatus());

                if (_dbService != null)
                {
                    await _dbService.SaveAccountConfigAsync(new TaxAccountConfigDto
                    {
                        TaxCode = taxCode,
                        EncryptedPassword = DataEncryptionHelper.Encrypt(password),
                        Token = token.Value,
                        TokenExpiresAt = token.ExpiresAt?.LocalDateTime,
                        LastSyncDate = DateTime.Now
                    });
                }

                AppendLog($"[Login] Authentication successful for Tax Code: {taxCode}.");
                MessageBox.Show("Đăng nhập Cổng Thuế thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                lblTokenStatus.Text = "Đăng nhập thất bại: " + ex.Message;
                lblTokenStatus.Foreground = new SolidColorBrush(Color.FromRgb(211, 47, 47));
                MessageBox.Show($"Lỗi đăng nhập: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                btnLogin.IsEnabled = true;
            }
        }

        private void BtnBrowseFolder_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFolderDialog
            {
                Title = "Chọn thư mục lưu tệp hóa đơn JSON/XML"
            };
            if (dialog.ShowDialog() == true)
            {
                txtFolder.Text = dialog.FolderName;
            }
        }

        private async void BtnDownload_Click(object sender, RoutedEventArgs e)
        {
            if (_client.Token == null || !_client.Token.IsValid())
            {
                MessageBox.Show("Token đăng nhập đã hết hạn hoặc chưa đăng nhập. Vui lòng bấm 'Đăng nhập' trước khi tải.", 
                                "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var rootFolder = txtFolder.Text.Trim();
            if (string.IsNullOrEmpty(rootFolder))
            {
                MessageBox.Show("Vui lòng chọn thư mục lưu trữ.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var taxCode = txtTaxCode.Text.Trim();
            var fromDate = dtFrom.DateTime.Date;
            var toDate = dtTo.DateTime.Date;

            if (fromDate > toDate)
            {
                MessageBox.Show("Từ ngày phải nhỏ hơn hoặc bằng Đến ngày.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var directions = new List<InvoiceDirection>();
            if (rbPurchase.IsChecked == true) directions.Add(InvoiceDirection.Purchase);
            else if (rbSold.IsChecked == true) directions.Add(InvoiceDirection.Sold);
            else { directions.Add(InvoiceDirection.Purchase); directions.Add(InvoiceDirection.Sold); }

            btnDownload.IsEnabled = false;
            _cancellationTokenSource = new CancellationTokenSource();

            var fileStore = new TaxInvoiceFileStore(rootFolder);
            var downloader = new TaxInvoiceDownloader(_client, fileStore);

            var downloadRequest = new DownloadRequest
            {
                TaxCode = taxCode,
                FromDate = fromDate,
                ToDate = toDate,
                Directions = directions,
                DownloadDetail = chkDetail.IsChecked == true,
                DownloadXml = chkXml.IsChecked == true,
                IncludeSco = true
            };

            progressBar.Value = 0;
            txtLog.Clear();
            AppendLog($"[Start] Downloading invoices from {fromDate:dd/MM/yyyy} to {toDate:dd/MM/yyyy}...");

            var progress = new Progress<DownloadProgress>(p =>
            {
                lblProgressStatus.Text = p.Message;
                if (p.Total > 0)
                {
                    progressBar.Maximum = p.Total;
                    progressBar.Value = p.Done;
                }
                AppendLog(p.ToString());
            });

            try
            {
                var downloadResult = await downloader.DownloadAsync(downloadRequest, progress, _cancellationTokenSource.Token);
                progressBar.Value = progressBar.Maximum;
                lblProgressStatus.Text = $"Hoàn tất: Đã tải {downloadResult.InvoiceCount} hóa đơn ({downloadResult.ErrorCount} lỗi).";
                AppendLog($"[Completed] Invoices saved to: {string.Join(", ", downloadResult.MonthFolders)}");
                MessageBox.Show($"Tải hoàn tất {downloadResult.InvoiceCount} hóa đơn về máy tính!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (OperationCanceledException)
            {
                lblProgressStatus.Text = "Tiến trình tải đã bị hủy.";
                AppendLog("[Canceled] Download process stopped by user.");
            }
            catch (Exception ex)
            {
                lblProgressStatus.Text = "Lỗi khi tải: " + ex.Message;
                AppendLog("[ERROR] " + ex.Message);
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi tải hóa đơn", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                btnDownload.IsEnabled = true;
            }
        }

        private void BtnOpenFolder_Click(object sender, RoutedEventArgs e)
        {
            var folder = txtFolder.Text.Trim();
            if (System.IO.Directory.Exists(folder))
            {
                Process.Start(new ProcessStartInfo("explorer.exe", folder) { UseShellExecute = true });
            }
            else
            {
                MessageBox.Show("Thư mục lưu trữ chưa tồn tại.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnReview_Click(object sender, RoutedEventArgs e)
        {
            var folder = txtFolder.Text.Trim();
            var reviewWin = new TaxInvoiceReviewWindow(folder, _dbService?.DatabasePath);
            reviewWin.Owner = this.Owner;
            reviewWin.Show();
            this.Close();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void AppendLog(string message)
        {
            txtLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}\n");
            txtLog.ScrollToEnd();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            _cancellationTokenSource?.Cancel();
            _client.Dispose();
        }
    }
}
