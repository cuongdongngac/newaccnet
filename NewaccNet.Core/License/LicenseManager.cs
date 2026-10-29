using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace NewaccNet.Core.License
{
    public class LicenseInfo
    {
        public string TargetId { get; set; } = string.Empty; // MachineCode hoặc TaxCode
        public string ExpiryDate { get; set; } = "20991231"; // YYYYMMDD (20991231 = Dài hạn / Vĩnh viễn)
        public string LicenseKey { get; set; } = string.Empty;
        public DateTime RegisteredDate { get; set; } = DateTime.Now;
        public string LicenseType { get; set; } = "Service"; // "Service" (Kế toán dịch vụ) hoặc "Enterprise" (Doanh nghiệp)
    }

    public static class LicenseManager
    {
        // Khóa bí mật nội bộ chỉ tác giả Phạm Việt Cường sở hữu
        private const string MasterSecretKey = "PVCUONG@NEWACCNET#2026$LICENSE!KEY_CONTROL_V1";

        public const int TrialVoucherLimit = 100; // Giới hạn 100 chứng từ ở bản dùng thử

        private static LicenseInfo? _currentLicense;
        private static bool _isInitialized = false;

        public static bool IsLicensed => CheckIsValidLicense();
        public static string LicenseStatusText => GetStatusDescription();
        public static DateTime? ExpiryDate => GetCurrentExpiryDate();

        public static int MaxVouchers
        {
            get
            {
                try
                {
                    var cfg = DbConfigService.Current;
                    string machineCode = HardwareHelper.GetMachineCode();
                    if (DbConfigService.ValidateRegistrationCode(cfg, machineCode, out int maxRecords))
                    {
                        return maxRecords;
                    }
                }
                catch { }
                return TrialVoucherLimit; // 100
            }
        }

        private static string GetLicenseFilePath()
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string folder = Path.Combine(appData, "NewaccNet");
            if (!System.IO.Directory.Exists(folder))
            {
                System.IO.Directory.CreateDirectory(folder);
            }
            return Path.Combine(folder, "license.lic");
        }

        public static void Initialize()
        {
            if (_isInitialized) return;
            LoadLicenseFile();
            _isInitialized = true;
        }

        private static void LoadLicenseFile()
        {
            try
            {
                string path = GetLicenseFilePath();
                if (File.Exists(path))
                {
                    string json = File.ReadAllText(path);
                    _currentLicense = JsonSerializer.Deserialize<LicenseInfo>(json);
                }
            }
            catch
            {
                _currentLicense = null;
            }
        }

        /// <summary>
        /// Kích hoạt bản quyền bằng License Key
        /// Định dạng: KEY-YYYYMMDD-XXXX-XXXX-XXXX
        /// </summary>
        public static bool Activate(string licenseKey, out string errorMessage)
        {
            errorMessage = string.Empty;
            if (string.IsNullOrWhiteSpace(licenseKey))
            {
                errorMessage = "Vui lòng nhập mã kích hoạt bản quyền!";
                return false;
            }

            licenseKey = licenseKey.Trim().ToUpperInvariant();
            string machineCode = HardwareHelper.GetMachineCode();

            // Kiểm tra tính hợp lệ của key
            if (!ValidateLicenseKey(machineCode, licenseKey, out string expiryDateStr))
            {
                errorMessage = "Mã kích hoạt không hợp lệ hoặc không khớp với mã máy này!";
                return false;
            }

            // Kiểm tra ngày hết hạn
            if (DateTime.TryParseExact(expiryDateStr, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out DateTime expDate))
            {
                if (expDate < DateTime.Today)
                {
                    errorMessage = $"Mã kích hoạt này đã hết hạn vào ngày {expDate:dd/MM/yyyy}!";
                    return false;
                }
            }

            // Lưu thông tin bản quyền
            _currentLicense = new LicenseInfo
            {
                TargetId = machineCode,
                ExpiryDate = expiryDateStr,
                LicenseKey = licenseKey,
                RegisteredDate = DateTime.Now,
                LicenseType = "Service"
            };

            try
            {
                string json = JsonSerializer.Serialize(_currentLicense, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(GetLicenseFilePath(), json);
                return true;
            }
            catch (Exception ex)
            {
                errorMessage = "Không thể ghi tệp bản quyền: " + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Thuật toán sinh Khóa kích hoạt (Dành cho Tác giả tạo key gửi khách hàng)
        /// targetId: MachineCode hoặc TaxCode
        /// expiryDateStr: định dạng YYYYMMDD (ví dụ 20991231 hoặc 20271231)
        /// </summary>
        public static string GenerateLicenseKey(string targetId, string expiryDateStr = "20991231")
        {
            targetId = targetId.Trim().ToUpperInvariant();
            expiryDateStr = expiryDateStr.Trim();

            string rawPayload = $"{targetId}#{expiryDateStr}#{MasterSecretKey}";

            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(MasterSecretKey)))
            {
                byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(rawPayload));
                string hex = Convert.ToHexString(hash).ToUpperInvariant();

                // Sinh ra 3 cụm 4 ký tự: S1-S2-S3
                string s1 = hex.Substring(0, 4);
                string s2 = hex.Substring(4, 4);
                string s3 = hex.Substring(8, 4);

                return $"KEY-{expiryDateStr}-{s1}-{s2}-{s3}";
            }
        }

        public static bool ValidateLicenseKey(string targetId, string licenseKey, out string expiryDateStr)
        {
            expiryDateStr = "20991231";
            try
            {
                var parts = licenseKey.Split('-');
                if (parts.Length != 5 || parts[0] != "KEY")
                {
                    return false;
                }

                expiryDateStr = parts[1];
                string expectedKey = GenerateLicenseKey(targetId, expiryDateStr);
                return string.Equals(licenseKey, expectedKey, StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        private static bool CheckIsValidLicense()
        {
            // 1. Kiểm tra bản quyền lưu trong CSDL (DbConfig)
            try
            {
                var cfg = DbConfigService.Current;
                string machineCode = HardwareHelper.GetMachineCode();
                if (DbConfigService.ValidateRegistrationCode(cfg, machineCode, out int maxRecords))
                {
                    return true;
                }
            }
            catch { }

            // 2. Fallback kiểm tra file license.lic cũ nếu có
            Initialize();
            if (_currentLicense != null && !string.IsNullOrEmpty(_currentLicense.LicenseKey))
            {
                string machineCode = HardwareHelper.GetMachineCode();
                if (string.Equals(_currentLicense.TargetId, machineCode, StringComparison.OrdinalIgnoreCase))
                {
                    if (ValidateLicenseKey(machineCode, _currentLicense.LicenseKey, out string expStr))
                    {
                        if (DateTime.TryParseExact(expStr, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out DateTime expDate))
                        {
                            if (expDate >= DateTime.Today)
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        private static DateTime? GetCurrentExpiryDate()
        {
            // 1. Lấy hạn dùng từ CSDL (DbConfig)
            try
            {
                var cfg = DbConfigService.Current;
                string machineCode = HardwareHelper.GetMachineCode();
                if (DbConfigService.ValidateRegistrationCode(cfg, machineCode, out DateTime? dbExp))
                {
                    return dbExp;
                }
            }
            catch { }

            // 2. Fallback lấy từ file license.lic
            if (_currentLicense != null && DateTime.TryParseExact(_currentLicense.ExpiryDate, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out DateTime exp))
            {
                return exp;
            }
            return null;
        }

        private static string GetStatusDescription()
        {
            if (IsLicensed)
            {
                int max = MaxVouchers;
                string limitStr = max >= 999999 ? "Vô hạn (Không giới hạn)" : $"{max:N0} chứng từ";
                return $"Bản quyền chính thức (Gói {limitStr})";
            }
            return "Bản Dùng Thử (Giới hạn 100 chứng từ)";
        }
    }
}

