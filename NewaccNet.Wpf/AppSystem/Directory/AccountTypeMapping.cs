using System.Collections.Generic;

namespace NewaccNet.Wpf.AppSystem.Directory
{
    public class AccountTypeMapping
    {
        public string Code { get; set; }
        public string Name { get; set; }

        public static List<AccountTypeMapping> GetAccountTypes()
        {
            return new List<AccountTypeMapping>
            {
                new AccountTypeMapping { Code = "A", Name = "Công nợ" },
                new AccountTypeMapping { Code = "B", Name = "Vật tư, hàng hóa" },
                new AccountTypeMapping { Code = "C", Name = "Ngoại tệ" },
                new AccountTypeMapping { Code = "D", Name = "Tài sản" },
                new AccountTypeMapping { Code = "E", Name = "Chi phí" },
                new AccountTypeMapping { Code = "I", Name = "Tiền mặt, không theo dõi" },
                new AccountTypeMapping { Code = "S", Name = "Cổ phiếu" },
                new AccountTypeMapping { Code = "V", Name = "Thuế VAT" }
            };
        }
    }
}
