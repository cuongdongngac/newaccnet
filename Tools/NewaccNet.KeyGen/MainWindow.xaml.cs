using System;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace NewaccNet.KeyGen
{
    public partial class MainWindow : Window
    {
        // Khóa bí mật đồng bộ 100% với Google Apps Script và NewaccNet Core
        private const string SecretSalt = "PVCUONG#NEWACC2026$RECORD_TIER";
        private const int MaskVal = 0xA75C31;

        // Ma trận vị trí đan xen tinh vi (6 vị trí cho số lượng, 10 vị trí cho chữ ký)
        private static readonly int[] LimitIndices = new int[] { 2, 5, 8, 11, 13, 15 };
        private static readonly int[] HashIndices  = new int[] { 0, 1, 3, 4, 6, 7, 9, 10, 12, 14 };

        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnPaste_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Clipboard.ContainsText())
                {
                    txtTargetId.Text = Clipboard.GetText().Trim();
                }
            }
            catch { }
        }

        private void RadLimit_Checked(object sender, RoutedEventArgs e)
        {
            if (txtCustomLimit == null) return;

            if (rad500.IsChecked == true) txtCustomLimit.Text = "500";
            else if (rad1000.IsChecked == true) txtCustomLimit.Text = "1000";
            else if (rad2000.IsChecked == true) txtCustomLimit.Text = "2000";
            else if (rad5000.IsChecked == true) txtCustomLimit.Text = "5000";
            else if (rad10000.IsChecked == true) txtCustomLimit.Text = "10000";
            else if (radUnlimited.IsChecked == true) txtCustomLimit.Text = "999999";
        }

        private void TxtCustomLimit_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Tùy biến số lượng
        }

        private void BtnGenerate_Click(object sender, RoutedEventArgs e)
        {
            string machineCode = txtTargetId.Text?.Trim().ToUpperInvariant() ?? "";
            if (string.IsNullOrWhiteSpace(machineCode))
            {
                MessageBox.Show("Vui lòng nhập hoặc dán Mã máy tính của khách hàng!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(txtCustomLimit.Text?.Trim(), out int limit) || limit <= 0)
            {
                MessageBox.Show("Vui lòng nhập số lượng bản ghi hợp lệ (lớn hơn 0)!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string code = GenerateCode(machineCode, limit);
            txtResultCode.Text = code;
            txtVerifyCode.Text = code; // Điền sẵn xuống khung đối chiếu

            string limitDesc = limit >= 999999 ? "Vô hạn (Không giới hạn)" : $"{limit:N0} chứng từ";
            txtSummary.Text = $"✔ Đã tạo Mã thành công cho Máy [{machineCode}] • Gói: {limitDesc}";
        }

        private void BtnCopy_Click(object sender, RoutedEventArgs e)
        {
            string code = txtResultCode.Text?.Trim() ?? "";
            if (string.IsNullOrWhiteSpace(code))
            {
                MessageBox.Show("Chưa có Mã đăng ký nào để sao chép!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                Clipboard.SetText(code);
                MessageBox.Show($"Đã sao chép Mã đăng ký vào Clipboard:\n\n{code}\n\nBạn có thể dán (Paste) gửi Zalo / Email cho khách hàng ngay.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi sao chép: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnVerify_Click(object sender, RoutedEventArgs e)
        {
            string machineCode = txtTargetId.Text?.Trim().ToUpperInvariant() ?? "";
            string code = txtVerifyCode.Text?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(code))
            {
                MessageBox.Show("Vui lòng nhập mã đăng ký cần kiểm tra!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (DecodeCode(machineCode, code, out int limit))
            {
                string desc = limit >= 999999 ? "VÔ HẠN (Không giới hạn)" : $"{limit:N0} chứng từ JournalVoucher";
                txtVerifyResult.Foreground = new SolidColorBrush(Color.FromRgb(5, 150, 105));
                txtVerifyResult.Text = $"✔ HỢP LỆ TUYỆT ĐỐI! Phần mềm sẽ luận ra: Gói {desc} (Khớp 100% với Google Sheet)";
            }
            else
            {
                txtVerifyResult.Foreground = new SolidColorBrush(Color.FromRgb(220, 38, 38));
                txtVerifyResult.Text = "❌ MÃ KHÔNG HỢP LỆ! (Chữ ký không khớp, sai Mã máy hoặc bị sửa đổi)";
            }
        }

        public static string GenerateCode(string machineCode, int limit)
        {
            machineCode = (machineCode ?? "").Replace("-", "").Trim().ToUpperInvariant();
            limit = Math.Max(0, limit);

            // 1. Mã hóa số bản ghi thành 6 ký tự hex
            int maskedLimit = (limit ^ MaskVal) & 0xFFFFFF;
            string limitHex = maskedLimit.ToString("X6");

            // 2. Chữ ký số băm MD5
            string raw = $"{machineCode}#{limit}#{SecretSalt}";
            using var md5 = MD5.Create();
            byte[] hashBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(raw));
            string hashHex = Convert.ToHexString(hashBytes).ToUpperInvariant();

            // 3. Đan xen 6 ký tự limit vào 10 ký tự hash
            char[] result = new char[16];
            for (int i = 0; i < LimitIndices.Length; i++)
            {
                result[LimitIndices[i]] = limitHex[i];
            }
            for (int i = 0; i < HashIndices.Length; i++)
            {
                result[HashIndices[i]] = hashHex[i];
            }

            string s = new string(result);
            return $"{s.Substring(0, 4)}-{s.Substring(4, 4)}-{s.Substring(8, 4)}-{s.Substring(12, 4)}";
        }

        public static bool DecodeCode(string machineCode, string code, out int limit)
        {
            limit = 0;
            if (string.IsNullOrWhiteSpace(code)) return false;

            machineCode = (machineCode ?? "").Replace("-", "").Trim().ToUpperInvariant();
            string clean = code.Replace("-", "").Trim().ToUpperInvariant();
            if (clean.Length != 16) return false;

            // 1. Bóc tách 6 ký tự limit
            char[] limitChars = new char[6];
            for (int i = 0; i < LimitIndices.Length; i++)
            {
                limitChars[i] = clean[LimitIndices[i]];
            }

            try
            {
                int masked = Convert.ToInt32(new string(limitChars), 16);
                limit = (masked ^ MaskVal) & 0xFFFFFF;
            }
            catch
            {
                return false;
            }

            // 2. Bóc tách 10 ký tự hash
            char[] hashChars = new char[10];
            for (int i = 0; i < HashIndices.Length; i++)
            {
                hashChars[i] = clean[HashIndices[i]];
            }
            string extractedHash = new string(hashChars);

            // 3. Tính chữ ký kỳ vọng
            string raw = $"{machineCode}#{limit}#{SecretSalt}";
            using var md5 = MD5.Create();
            byte[] hashBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(raw));
            string expectedHash = Convert.ToHexString(hashBytes).ToUpperInvariant().Substring(0, 10);

            return string.Equals(extractedHash, expectedHash, StringComparison.OrdinalIgnoreCase);
        }
    }
}
