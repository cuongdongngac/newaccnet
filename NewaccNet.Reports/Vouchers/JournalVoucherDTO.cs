using DevExpress.DataAccess.ObjectBinding;
using System;
using System.Collections.Generic;

namespace NewaccNet.Reports.Vouchers
{
    [HighlightedClass]
    public class JournalEntryDTO
    {
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public string AccountId { get; set; }
        public double? Amount { get; set; }
        public short? Dbcr { get; set; }
        
        // Navigation properties cho report
        public string AccountName { get; set; }
        
        public JournalEntryDTO() { }
    }

    [HighlightedClass]
    public class JournalVoucherDTO
    {
        public int Id { get; set; }
        public string VoucherNo { get; set; }
        public DateTime? VoucherDate { get; set; }
        public string Contents { get; set; }
        public string Personname { get; set; }
        public string Personaddress { get; set; }
        public string Invoicesnumber { get; set; }
        
        // Đây chính là điểm "không phẳng" (Master-Detail) để XtraReport tự động nhận diện cấp con (DetailReportBand)
        public List<JournalEntryDTO> JournalEntries { get; set; }

        public JournalVoucherDTO()
        {
            JournalEntries = new List<JournalEntryDTO>();
        }
    }
}
