/* =====================================================================================
 * LicenseHelper.cs — HƯỚNG DẪN TRIỂN KHAI (dành cho người/AI tiếp nhận code)
 * =====================================================================================
 *
 * MỤC TIÊU
 *   Ứng dụng desktop .NET cho dùng thử N ngày tính từ ngày cài; sau đó cần key bản quyền
 *   gắn với máy. Mức bảo vệ "vừa đủ": phần mềm giá trị không cao, ưu tiên KHÔNG làm phiền
 *   khách hàng hợp lệ và dễ hỗ trợ. Không cần chống crack tuyệt đối.
 *
 * YÊU CẦU KỸ THUẬT
 *   - .NET Framework 3.5 SP1 hoặc 4.0 (4.0 là bản cuối chạy trên Windows XP).
 *   - Thêm tham chiếu System.Management.dll.
 *   - Thuần .NET: không hợp ngữ, không DLL native.
 *
 * CẤU TRÚC FILE NÀY (đã viết xong, chỉ cần tích hợp)
 *   1. HardwareId      — lấy mã máy từ WMI: CPU, Mainboard, BIOS, UUID (KHÔNG dùng ổ cứng).
 *                        Mỗi thành phần băm riêng -> mã dạng XXXXXXXX-XXXXXXXX-XXXXXXXX-XXXXXXXX.
 *                        So khớp "mờ": đủ 3/4 thành phần khớp là hợp lệ (tối thiểu 2 nếu máy
 *                        đọc được ít thông tin), nên thay RAM/ổ cứng/cài lại Windows không mất key.
 *   2. LicenseInfo / LicenseVerifier — key = base64(nội dung) + "." + base64(chữ ký RSA-SHA256).
 *                        Ứng dụng chỉ chứa KHÓA CÔNG KHAI nên không ai tự tạo được key.
 *   3. LicenseIssuer   — sinh cặp khóa và cấp key. CHỈ dùng trong tool nội bộ, PHẢI XÓA hoặc
 *                        loại khỏi project ứng dụng phát hành.
 *
 * VIỆC CẦN LÀM
 *   B1. Tạo project tool nội bộ "KeyIssuer" (WinForms/Console), copy file này vào.
 *       Gọi LicenseIssuer.GenerateKeys MỘT LẦN DUY NHẤT; lưu privateXml ở nơi an toàn
 *       (không commit lên git); dán publicXml vào hằng LicenseVerifier.PublicKeyXml.
 *       Tool có form: nhập Tên khách, Mã máy, Ngày hết hạn (hoặc vĩnh viễn), Tính năng
 *       -> gọi LicenseIssuer.Issue -> hiện key để copy gửi khách.
 *   B2. Trong ứng dụng chính: xóa lớp LicenseIssuer khỏi bản copy của file này.
 *   B3. Viết lớp TrialManager theo đặc tả "DÙNG THỬ" bên dưới.
 *   B4. Viết màn hình "Bản quyền":
 *       - Hiển thị mã máy (HardwareId.GetMachineCode) + nút Copy + thông tin liên hệ hỗ trợ.
 *       - Ô dán key + nút Kích hoạt -> LicenseVerifier.Verify; báo lỗi bằng chuỗi 'error'.
 *       - Key hợp lệ thì lưu vào %ProgramData%\<Công ty>\<Ứng dụng>\license.key
 *         (fallback %AppData% nếu không có quyền ghi).
 *   B5. Khi khởi động:
 *       - Có license.key và Verify thành công -> chạy đầy đủ (kiểm tra HasFeature nếu cần).
 *       - Ngược lại: còn hạn dùng thử -> chạy, hiện "Còn X ngày dùng thử";
 *         hết hạn -> mở màn hình Bản quyền, giới hạn chức năng (ví dụ không cho lưu/xuất)
 *         thay vì chặn mở ứng dụng, để khách vẫn xem được dữ liệu của mình.
 *   B6. Kiểm thử trên Windows XP SP3 (nếu còn hỗ trợ), Windows 7, Windows 10/11; trên máy ảo
 *       (thường thiếu serial mainboard/BIOS) để chắc chắn không có lỗi.
 *
 * DÙNG THỬ (TrialManager) — đặc tả
 *   - Số ngày dùng thử: hằng số TrialDays (ví dụ 30).
 *   - Ngày bắt đầu = ngày cài đặt. Nguồn đọc, lấy ngày SỚM NHẤT trong các nguồn tìm thấy:
 *       a) HKLM\Software\<Công ty>\<Ứng dụng>\InstallDate  (do bộ cài ghi, nếu có)
 *       b) HKCU\Software\<Công ty>\<Ứng dụng>\FirstRun
 *       c) %ProgramData%\<Công ty>\<Ứng dụng>\state.dat
 *     Nếu không có nguồn nào -> coi hôm nay là ngày bắt đầu, ghi vào (b) và (c).
 *     Như vậy bộ cài ghi ngày là TÙY CHỌN; ứng dụng tự ghi khi chạy lần đầu cũng đủ.
 *   - Định dạng ngày: "yyyy-MM-dd" (InvariantCulture). Kèm 8 ký tự hash
 *     (SHA256 của ngày + chuỗi bí mật cố định) để phát hiện sửa tay; hash sai thì bỏ qua nguồn đó.
 *   - Lưu LastRun (cùng nơi). Nếu ngày hiện tại < LastRun - 1 ngày -> đồng hồ bị lùi:
 *     coi như hết hạn dùng thử và báo người dùng chỉnh lại giờ.
 *   - Gỡ cài đặt KHÔNG xóa các giá trị trên (Inno Setup: flag uninsneveruninstall).
 *   - Ghi registry/file bọc try/catch; lỗi quyền ghi thì bỏ qua, không làm sập ứng dụng.
 *   - Lưu ý 64-bit: bộ cài 32-bit ghi HKLM vào nhánh WOW6432Node. Ứng dụng đọc HKLM bằng
 *     RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32) (.NET 4.0+),
 *     hoặc build ứng dụng x86 để cùng nhánh.
 *   - Ví dụ Inno Setup (tùy chọn):
 *       [Registry]
 *       Root: HKLM; Subkey: "Software\<Công ty>\<Ứng dụng>"; ValueType: string;
 *         ValueName: "InstallDate"; ValueData: "{code:InstallDateValue}";
 *         Flags: createvalueifdoesntexist uninsneveruninstall
 *     Hàm InstallDateValue trả về ngày + hash theo đúng định dạng trên (hoặc chỉ ghi ngày
 *     và để ứng dụng chấp nhận nguồn HKLM không cần hash).
 *
 * KHÔNG ĐƯỢC THAY ĐỔI SAU KHI ĐÃ PHÁT HÀNH (sẽ làm hỏng mọi key/mã máy cũ)
 *   - Hằng HardwareId.Salt, thứ tự và danh sách thành phần phần cứng.
 *   - Định dạng payload "v1\n..." trong LicenseInfo; muốn đổi thì thêm "v2" và vẫn đọc được "v1".
 *   - Cặp khóa RSA.
 *
 * NÊN LÀM THÊM (không bắt buộc)
 *   - Làm rối mã bản phát hành (ConfuserEx / .NET Reactor / Eazfuscator).
 *   - Ghi log khi Verify thất bại (lý do + mã máy) để hỗ trợ khách nhanh hơn.
 * ===================================================================================== */

using System;
using System.Globalization;
using System.Management;
using System.Security.Cryptography;
using System.Text;

namespace MyApp.Licensing
{
    // ------------------------------------------------------------------
    // 1. MÃ MÁY: nhiều thành phần phần cứng, mỗi thành phần băm riêng
    // ------------------------------------------------------------------
    public static class HardwareId
    {
        // Đổi chuỗi này cho riêng ứng dụng của bạn TRƯỚC lần phát hành đầu tiên
        private const string Salt = "MYAPP-HWID-v1";

        // Thứ tự cố định: CPU, BOARD, BIOS, UUID
        public const int SlotCount = 4;

        public static string[] GetRawComponents()
        {
            return new string[] {
                Read("SELECT ProcessorId FROM Win32_Processor", "ProcessorId"),
                Read("SELECT SerialNumber FROM Win32_BaseBoard", "SerialNumber"),
                Read("SELECT SerialNumber FROM Win32_BIOS", "SerialNumber"),
                Read("SELECT UUID FROM Win32_ComputerSystemProduct", "UUID")
            };
        }

        /// <summary>Mã máy hiển thị cho khách, dạng XXXXXXXX-XXXXXXXX-XXXXXXXX-XXXXXXXX. 00000000 = không đọc được.</summary>
        public static string GetMachineCode()
        {
            string[] raw = GetRawComponents();
            string[] parts = new string[SlotCount];
            for (int i = 0; i < SlotCount; i++)
                parts[i] = raw[i] == null ? "00000000" : ShortHash(i + "|" + raw[i]);
            return string.Join("-", parts);
        }

        /// <summary>So khớp "mờ": cho phép thay vài linh kiện mà không mất bản quyền.</summary>
        public static bool Matches(string licensedCode, string currentCode, int minMatches)
        {
            string[] a = licensedCode.Split('-');
            string[] b = currentCode.Split('-');
            if (a.Length != SlotCount || b.Length != SlotCount) return false;

            int usable = 0, matched = 0;
            for (int i = 0; i < SlotCount; i++)
            {
                if (a[i] == "00000000") continue;
                usable++;
                if (string.Equals(a[i], b[i], StringComparison.OrdinalIgnoreCase)) matched++;
            }
            // Máy đọc được ít thành phần thì hạ ngưỡng, nhưng tối thiểu 2
            int need = Math.Max(2, Math.Min(minMatches, usable));
            return usable > 0 && matched >= need;
        }

        private static string Read(string wql, string prop)
        {
            try
            {
                using (ManagementObjectSearcher s = new ManagementObjectSearcher(wql))
                using (ManagementObjectCollection col = s.Get())
                {
                    foreach (ManagementBaseObject o in col)
                    {
                        object v = o[prop];
                        string n = Normalize(v == null ? null : v.ToString());
                        if (n != null) return n;
                    }
                }
            }
            catch (ManagementException) { }      // lớp/thuộc tính không tồn tại trên OS cũ
            catch (UnauthorizedAccessException) { }
            catch (System.Runtime.InteropServices.COMException) { } // dịch vụ WMI hỏng
            return null;
        }

        // Loại các giá trị "rác" hãng để mặc định — nếu không, hàng nghìn máy sẽ trùng mã
        private static readonly string[] Junk = {
            "TOBEFILLEDBYO.E.M.", "DEFAULTSTRING", "NONE", "NOTAPPLICABLE", "N/A",
            "SYSTEMSERIALNUMBER", "SERIALNUMBER", "0123456789", "OEM", "INVALID",
            "03000200-0400-0500-0006-000700080009"
        };

        private static string Normalize(string s)
        {
            if (s == null) return null;
            StringBuilder sb = new StringBuilder();
            foreach (char c in s.Trim().ToUpperInvariant())
                if (!char.IsWhiteSpace(c) && c != '\0') sb.Append(c);
            string r = sb.ToString();
            if (r.Length < 3) return null;
            foreach (string j in Junk) if (r == j) return null;

            bool allSame = true; // "00000000", "FFFF-FFFF..."
            string core = r.Replace("-", "");
            for (int i = 1; i < core.Length; i++) if (core[i] != core[0]) { allSame = false; break; }
            return allSame ? null : r;
        }

        private static string ShortHash(string s)
        {
            using (SHA256 sha = new SHA256Managed())
            {
                byte[] h = sha.ComputeHash(Encoding.UTF8.GetBytes(Salt + "|" + s));
                return BitConverter.ToString(h, 0, 4).Replace("-", "");
            }
        }
    }

    // ------------------------------------------------------------------
    // 2. GIẤY PHÉP: nội dung + chữ ký RSA. App chỉ giữ KHÓA CÔNG KHAI.
    // ------------------------------------------------------------------
    public sealed class LicenseInfo
    {
        public string Customer;
        public string MachineCode;
        public DateTime Expires;   // DateTime.MaxValue = vĩnh viễn
        public string Features;    // vd: "PRO,EXPORT"

        public string ToPayload()
        {
            string exp = Expires == DateTime.MaxValue ? "never" : Expires.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            return "v1\n" + Customer + "\n" + MachineCode + "\n" + exp + "\n" + Features;
        }

        public static LicenseInfo FromPayload(string p)
        {
            string[] l = p.Split('\n');
            if (l.Length != 5 || l[0] != "v1") return null;
            LicenseInfo li = new LicenseInfo();
            li.Customer = l[1];
            li.MachineCode = l[2];
            li.Expires = l[3] == "never" ? DateTime.MaxValue
                : DateTime.ParseExact(l[3], "yyyy-MM-dd", CultureInfo.InvariantCulture);
            li.Features = l[4];
            return li;
        }

        public bool HasFeature(string f)
        {
            return ("," + Features + ",").IndexOf("," + f + ",", StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }

    public static class LicenseVerifier
    {
        // Dán khóa công khai (XML) sinh bởi LicenseIssuer.GenerateKeys vào đây
        private const string PublicKeyXml = "<RSAKeyValue><Modulus>...</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

        /// <summary>licenseText dạng "base64(nội dung).base64(chữ ký)"</summary>
        public static LicenseInfo Verify(string licenseText, out string error)
        {
            error = null;
            try
            {
                string[] parts = licenseText.Trim().Split('.');
                if (parts.Length != 2) { error = "Định dạng key không hợp lệ."; return null; }

                byte[] data = Convert.FromBase64String(parts[0]);
                byte[] sig = Convert.FromBase64String(parts[1]);

                using (RSACryptoServiceProvider rsa = Crypto.CreateRsa())
                {
                    rsa.FromXmlString(PublicKeyXml);
                    if (!rsa.VerifyData(data, "SHA256", sig)) { error = "Chữ ký không hợp lệ."; return null; }
                }

                LicenseInfo li = LicenseInfo.FromPayload(Encoding.UTF8.GetString(data));
                if (li == null) { error = "Nội dung key không hợp lệ."; return null; }

                if (!HardwareId.Matches(li.MachineCode, HardwareId.GetMachineCode(), 3))
                { error = "Key không dành cho máy này."; return null; }

                if (li.Expires != DateTime.MaxValue && DateTime.Now.Date > li.Expires)
                { error = "Key đã hết hạn."; return null; }

                return li;
            }
            catch (FormatException) { error = "Key bị hỏng."; return null; }
            catch (CryptographicException) { error = "Không kiểm tra được chữ ký."; return null; }
        }
    }

    internal static class Crypto
    {
        // PROV_RSA_AES (24): bắt buộc để dùng SHA256 trên Windows XP/2003;
        // CSP mặc định ở đó báo lỗi "Invalid algorithm specified".
        public static RSACryptoServiceProvider CreateRsa()
        {
            CspParameters cp = new CspParameters(24);
            cp.Flags = CspProviderFlags.UseMachineKeyStore;
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(2048, cp);
            rsa.PersistKeyInCsp = false;
            return rsa;
        }
    }

    // ------------------------------------------------------------------
    // 3. CÔNG CỤ CẤP KEY — chỉ nằm trong tool nội bộ, XÓA khỏi ứng dụng phát hành
    // ------------------------------------------------------------------
    public static class LicenseIssuer
    {
        public static void GenerateKeys(out string publicXml, out string privateXml)
        {
            using (RSACryptoServiceProvider rsa = Crypto.CreateRsa())
            {
                publicXml = rsa.ToXmlString(false);
                privateXml = rsa.ToXmlString(true); // GIỮ BÍ MẬT TUYỆT ĐỐI
            }
        }

        public static string Issue(LicenseInfo li, string privateXml)
        {
            byte[] data = Encoding.UTF8.GetBytes(li.ToPayload());
            using (RSACryptoServiceProvider rsa = Crypto.CreateRsa())
            {
                rsa.FromXmlString(privateXml);
                byte[] sig = rsa.SignData(data, "SHA256");
                return Convert.ToBase64String(data) + "." + Convert.ToBase64String(sig);
            }
        }
    }
}
