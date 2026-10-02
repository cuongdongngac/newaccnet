using System;
using System.Security.Cryptography;
using System.Text;

namespace NewaccNet.Wpf.AppSystem
{
    /// <summary>
    /// Lớp tiện ích mã hóa mật khẩu sử dụng SHA256 (one-way hash).
    /// Mật khẩu được hash và lưu vào DB dưới dạng chuỗi Base64.
    /// Khi đăng nhập, hash mật khẩu người dùng nhập và so sánh với giá trị trong DB.
    /// </summary>
    public static class SecurityHelper
    {
        /// <summary>
        /// Mã hóa (hash) mật khẩu thành chuỗi Base64 sử dụng SHA256.
        /// </summary>
        public static string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText)) return string.Empty;

            using (var sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(plainText));
                return Convert.ToBase64String(bytes);
            }
        }

        /// <summary>
        /// So sánh mật khẩu người dùng nhập với mật khẩu đã mã hóa trong DB.
        /// </summary>
        public static bool Verify(string plainText, string hashedPassword)
        {
            string hashed = Encrypt(plainText);
            return string.Equals(hashed, hashedPassword, StringComparison.Ordinal);
        }
    }
}
