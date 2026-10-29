using System;
using System.IO;
using System.Management;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace NewaccNet.Core.License
{
    /// <summary>
    /// Thu thập định danh phần cứng (Hardware Fingerprint) duy nhất của máy tính.
    /// Tương thích 100% với cả môi trường 32-bit (x86) và 64-bit (x64).
    /// </summary>
    public static class HardwareHelper
    {
        private static string? _cachedMachineCode;

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool GetVolumeInformation(
            string rootPathName,
            StringBuilder? volumeNameBuffer,
            int volumeNameSize,
            out uint volumeSerialNumber,
            out uint maximumComponentLength,
            out uint fileSystemFlags,
            StringBuilder? fileSystemNameBuffer,
            int fileSystemNameSize);

        /// <summary>
        /// Lấy Mã máy tính chuẩn định dạng: NAC-XXXX-XXXX-XXXX (16 ký tự hexa chia làm 4 cụm)
        /// </summary>
        public static string GetMachineCode()
        {
            if (!string.IsNullOrEmpty(_cachedMachineCode))
            {
                return _cachedMachineCode;
            }

            try
            {
                string mbSerial = GetMotherboardSerial();
                string cpuId = GetCpuId();
                string diskSerial = GetSystemDriveSerial();

                // Chuỗi định danh kết hợp
                string rawHardwareInfo = $"{mbSerial}|{cpuId}|{diskSerial}";

                if (string.IsNullOrWhiteSpace(rawHardwareInfo.Replace("|", "")))
                {
                    // Fallback nếu không đọc được từ WMI
                    rawHardwareInfo = GetFallbackIdentifier();
                }

                // Băm bằng SHA256
                using (var sha = SHA256.Create())
                {
                    byte[] hash = sha.ComputeHash(Encoding.UTF8.GetBytes(rawHardwareInfo));
                    string hex = Convert.ToHexString(hash).ToUpperInvariant();

                    // Lấy 16 ký tự đầu: format NAC-XXXX-XXXX-XXXX
                    _cachedMachineCode = $"NAC-{hex.Substring(0, 4)}-{hex.Substring(4, 4)}-{hex.Substring(8, 4)}-{hex.Substring(12, 4)}";
                }
            }
            catch
            {
                _cachedMachineCode = GetFallbackIdentifier();
            }

            return _cachedMachineCode;
        }

        private static string GetMotherboardSerial()
        {
            try
            {
                using (var searcher = new ManagementObjectSearcher("SELECT SerialNumber FROM Win32_BaseBoard"))
                {
                    foreach (ManagementObject mo in searcher.Get())
                    {
                        var val = mo["SerialNumber"]?.ToString()?.Trim();
                        if (!string.IsNullOrEmpty(val) && val != "To be filled by O.E.M." && val != "None")
                        {
                            return val;
                        }
                    }
                }
            }
            catch { }
            return string.Empty;
        }

        private static string GetCpuId()
        {
            try
            {
                using (var searcher = new ManagementObjectSearcher("SELECT ProcessorId FROM Win32_Processor"))
                {
                    foreach (ManagementObject mo in searcher.Get())
                    {
                        var val = mo["ProcessorId"]?.ToString()?.Trim();
                        if (!string.IsNullOrEmpty(val))
                        {
                            return val;
                        }
                    }
                }
            }
            catch { }
            return string.Empty;
        }

        private static string GetSystemDriveSerial()
        {
            try
            {
                // Thử lấy Volume Serial của ổ hệ điều hành C:\ qua Win32 API
                string root = Path.GetPathRoot(Environment.SystemDirectory) ?? "C:\\";
                if (GetVolumeInformation(root, null, 0, out uint serialNum, out _, out _, null, 0))
                {
                    return serialNum.ToString("X8");
                }
            }
            catch { }

            try
            {
                // Thử lấy qua Win32_DiskDrive
                using (var searcher = new ManagementObjectSearcher("SELECT SerialNumber FROM Win32_DiskDrive"))
                {
                    foreach (ManagementObject mo in searcher.Get())
                    {
                        var val = mo["SerialNumber"]?.ToString()?.Trim();
                        if (!string.IsNullOrEmpty(val))
                        {
                            return val;
                        }
                    }
                }
            }
            catch { }

            return string.Empty;
        }

        private static string GetFallbackIdentifier()
        {
            string fallback = $"{Environment.MachineName}-{Environment.ProcessorCount}-{Environment.OSVersion}";
            using (var sha = SHA256.Create())
            {
                byte[] hash = sha.ComputeHash(Encoding.UTF8.GetBytes(fallback));
                string hex = Convert.ToHexString(hash).ToUpperInvariant();
                return $"NAC-{hex.Substring(0, 4)}-{hex.Substring(4, 4)}-{hex.Substring(8, 4)}-{hex.Substring(12, 4)}";
            }
        }
    }
}

