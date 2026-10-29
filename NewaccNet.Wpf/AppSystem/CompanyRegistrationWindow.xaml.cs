using System;
using System.Windows;
using System.Windows.Media;
using DevExpress.Xpf.Core;

namespace NewaccNet.Wpf.AppSystem
{
    public partial class CompanyRegistrationWindow : NewaccNet.Wpf.Views.Base.BaseWindow
    {
        public CompanyRegistrationWindow()
        {
            InitializeComponent();
            this.Loaded += CompanyRegistrationWindow_Loaded;
        }

        private void CompanyRegistrationWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Đảm bảo bảng đã tồn tại
            DbConfigService.EnsureDbConfigTable();
            LoadData();
        }

        private void LoadData()
        {
            var cfg = DbConfigService.LoadDbConfig();

            txtCompanyName.Text = cfg.CompanyName;
            txtDepartmentName.Text = cfg.DepartmentName;
            txtAddress.Text = cfg.Address;
            txtBankAccount.Text = cfg.BankAccount;
            txtTaxCode.Text = cfg.TaxCode;

            cboAccountMethod.ItemsSource = DbConfigConstants.AccountMethods;
            cboAccountMethod.EditValue = cfg.AccountMethod > 0 ? cfg.AccountMethod : (short)1;

            cboMaterial.ItemsSource = DbConfigConstants.MaterialMethods;
            cboMaterial.EditValue = cfg.Material > 0 ? cfg.Material : (short)1;

            UpdateInstallationCode();
            txtRegistrationCode.Text = cfg.RegistrationCode;

            UpdateExpiryNotice(cfg);
        }

        private void UpdateInstallationCode()
        {
            txtInstallationCode.Text = HardwareHelper.GetMachineCode();
        }

        private void UpdateExpiryNotice(DbConfigDto cfg)
        {
            string machineCode = HardwareHelper.GetMachineCode();
            bool isValid = DbConfigService.ValidateRegistrationCode(cfg, machineCode, out int maxRecords);

            if (isValid)
            {
                string limitDesc = maxRecords >= 999999 ? "Vô hạn (Không giới hạn)" : $"{maxRecords:N0} chứng từ JournalVoucher";
                txtExpiryNotice.Text = $"Phiên bản ĐÃ ĐĂNG KÝ BẢN QUYỀN: Gói {limitDesc}.\nMã máy: {machineCode}";
                txtExpiryNotice.Foreground = new SolidColorBrush(Color.FromRgb(22, 101, 52)); // Xanh lá
            }
            else
            {
                txtExpiryNotice.Text = "Phiên bản chưa đăng ký bản quyền (Bản dùng thử miễn phí 100 chứng từ). Vui lòng nhập Số đăng ký để nâng hạn mức chứng từ.";
                txtExpiryNotice.Foreground = new SolidColorBrush(Color.FromRgb(217, 119, 6)); // Cam cảnh báo
            }
        }

        private void BtnCopyInstallationCode_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string code = txtInstallationCode.Text?.Trim();
                if (string.IsNullOrEmpty(code))
                {
                    ThemedMessageBox.Show("Thông báo", "Chưa có Mã máy tính để sao chép.", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                Clipboard.SetText(code);
                ThemedMessageBox.Show("Thông báo", $"Đã sao chép Mã máy tính: {code}\n\nHãy gửi mã này cho tác giả để nhận Mã đăng ký.", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                ThemedMessageBox.Show("Lỗi", "Không thể sao chép: " + ex.Message, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            var existingCfg = DbConfigService.Current;
            var cfg = new DbConfigDto
            {
                Id = existingCfg.Id,
                CompanyName = txtCompanyName.Text?.Trim() ?? "",
                DepartmentName = txtDepartmentName.Text?.Trim() ?? "",
                Address = txtAddress.Text?.Trim() ?? "",
                BankAccount = txtBankAccount.Text?.Trim() ?? "",
                TaxCode = txtTaxCode.Text?.Trim() ?? "",
                AccountingYear = existingCfg.AccountingYear,
                IsCompanyLicense = false,
                RegistrationCode = txtRegistrationCode.Text?.Trim().ToUpperInvariant() ?? "",
                ExpiryDate = "20991231",
                AccountMethod = cboAccountMethod.EditValue != null ? Convert.ToInt16(cboAccountMethod.EditValue) : (short)1,
                Material = cboMaterial.EditValue != null ? Convert.ToInt16(cboMaterial.EditValue) : (short)1
            };

            string machineCode = HardwareHelper.GetMachineCode();
            bool isValid = DbConfigService.ValidateRegistrationCode(cfg, machineCode, out int maxRecords);

            // Lưu vào CSDL (nếu hợp lệ, cfg.MaxRecords đã được giải mã đúng từ mã đăng ký)
            if (DbConfigService.SaveDbConfig(cfg))
            {
                DbConfigService.ClearCache();
                if (Application.Current.MainWindow is MainWindow mw)
                {
                    mw.UpdateAppTitle();
                }
                UpdateExpiryNotice(cfg);

                if (isValid)
                {
                    string limitDesc = maxRecords >= 999999 ? "VÔ HẠN (Không giới hạn)" : $"{maxRecords:N0} chứng từ";
                    ThemedMessageBox.Show(
                        "Đăng ký thành công",
                        $"ĐÃ ĐĂNG KÝ BẢN QUYỀN THÀNH CÔNG!\n\n• Gói hạn mức: {limitDesc}\n• Mã máy: {machineCode}\n\nThông tin đơn vị và bản quyền đã được lưu an toàn vào cơ sở dữ liệu.",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    this.DialogResult = true;
                    this.Close();
                }
                else
                {
                    if (string.IsNullOrEmpty(cfg.RegistrationCode))
                    {
                        ThemedMessageBox.Show(
                            "Thông báo",
                            "Đã lưu thông tin đơn vị vào CSDL thành công.\n(Chưa nhập Số đăng ký, phần mềm tiếp tục chạy ở chế độ Dùng Thử 100 chứng từ).",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                        this.Close();
                    }
                    else
                    {
                        ThemedMessageBox.Show(
                            "Cảnh báo",
                            "Thông tin đơn vị đã được lưu, nhưng Số đăng ký KHÔNG HỢP LỆ hoặc không khớp với mã máy này!\n\nVui lòng kiểm tra lại mã đã tạo từ Tool KeyGen hoặc Google Sheet.",
                            MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }
            else
            {
                string detail = !string.IsNullOrEmpty(DbConfigService.LastSaveError)
                    ? $"\n\nChi tiết kỹ thuật: {DbConfigService.LastSaveError}"
                    : "";
                ThemedMessageBox.Show("Lỗi", $"Không thể lưu thông tin vào cơ sở dữ liệu!{detail}", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnPrint_Click(object sender, RoutedEventArgs e)
        {
            ThemedMessageBox.Show("Thông tin Bản quyền", 
                $"ĐƠN VỊ: {txtCompanyName.Text}\n" +
                $"PHÒNG BAN: {txtDepartmentName.Text}\n" +
                $"ĐỊA CHỈ: {txtAddress.Text}\n" +
                $"MÃ SỐ THUẾ: {txtTaxCode.Text}\n" +
                $"SỐ ĐĂNG KÝ: {txtRegistrationCode.Text}", 
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnVoucherPostingConfig_Click(object sender, RoutedEventArgs e)
        {
            var win = new VoucherPostingConfigWindow { Owner = this };
            win.ShowDialog();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnHelp_Click(object sender, RoutedEventArgs e)
        {
            AboutWindow.OpenUrl("https://newacc.learning.io.vn");
        }
    }
}
