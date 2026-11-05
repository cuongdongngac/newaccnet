using System;
using System.Collections.Generic;
using DataAccess.EntityClasses;

namespace NewaccNet.Plugin.CashBank.Services
{
    public class CashVoucherLookupDto
    {
        public int Id { get; set; }
        public string VoucherNo { get; set; } = "";
        public DateTime VoucherDate { get; set; }
        public string VoucherType { get; set; } = ""; // PT, PC, BC, BN, CKT
        public string Description { get; set; } = "";
        public string PartnerName { get; set; } = "";
        public string CashAccount { get; set; } = "";
        public decimal TotalAmount { get; set; }
        public bool Bookflag { get; set; }

        public string DisplayText => $"{VoucherNo} - {VoucherDate:dd/MM/yyyy} - {TotalAmount:N0} đ - {Description}";
    }

    public class CashVoucherLineDto
    {
        public int LineId { get; set; }
        public string AliasCode { get; set; } = "";
        public string AliasName { get; set; } = "";
        public string AccountId { get; set; } = "";
        public string AccountName { get; set; } = "";
        public string CategoryId { get; set; } = "";
        public string Description { get; set; } = "";
        public decimal Amount { get; set; }

        // Đối tượng chi tiết dòng (nếu cần theo dõi riêng từng dòng)
        public string? PartnerId { get; set; }
        public string? CostCenterId { get; set; }

        // Danh sách chi tiết công nợ / chi phí
        public List<DebtDetailEntity> DebtDetails { get; set; } = new();
        public List<CostDetailEntity> CostDetails { get; set; } = new();

        public bool HasDetails => (DebtDetails != null && DebtDetails.Count > 0) || (CostDetails != null && CostDetails.Count > 0);
    }

    public class CashVoucherSaveParams
    {
        public int VoucherId { get; set; }
        public string VoucherType { get; set; } = "PT"; // PT (Phiếu thu), PC (Phiếu chi), BC (Báo có), BN (Báo nợ), CKT (Chuyển khoản)
        public string VoucherNo { get; set; } = "";
        public DateTime VoucherDate { get; set; } = DateTime.Today;
        public bool Bookflag { get; set; } = false; // Mặc định là false (chưa ghi sổ)

        public string Method { get; set; } = "Cash"; // "Cash" hoặc "Bank"
        public string CashAccountId { get; set; } = "1111"; // Tài khoản tiền: 1111, 1121...
        public string DestinationAccountId { get; set; } = ""; // Dùng cho chuyển tiền nội bộ CKT

        public string? PartnerId { get; set; }
        public string? PartnerName { get; set; }
        public string? Address { get; set; }
        public string? TaxCode { get; set; }

        // Thông tin in ấn và giao dịch ngân hàng
        public string PersonName { get; set; } = "";       // Người nộp / người nhận (Personname)
        public string PersonAddress { get; set; } = "";    // Địa chỉ người nộp / nhận (Personaddress)
        public string InvoicesNumber { get; set; } = "";    // Kèm theo chứng từ gốc / số HĐ (Invoicesnumber)
        public string Description { get; set; } = "";

        // Thông tin ủy nhiệm chi / ngân hàng
        public string Paymentacc { get; set; } = "";        // Số tài khoản chi
        public string Paymentbank { get; set; } = "";       // Ngân hàng chi
        public string Recievedcorp { get; set; } = "";      // Đơn vị thụ hưởng
        public string Recievedacc { get; set; } = "";       // Số tài khoản nhận
        public string Recievedbank { get; set; } = "";      // Ngân hàng nhận

        public List<CashVoucherLineDto> Lines { get; set; } = new();

        public decimal TotalAmount
        {
            get
            {
                decimal sum = 0;
                foreach (var line in Lines) sum += line.Amount;
                return sum;
            }
        }
    }
}
