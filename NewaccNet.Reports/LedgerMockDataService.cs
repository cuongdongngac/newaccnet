using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text.Json;
using System.Xml.Serialization;

namespace NewaccNet.Reports
{
    public class LedgerMockDataService
    {
        public List<LedgerReportDTO> GenerateMockList()
        {
            return new List<LedgerReportDTO>
            {
                new LedgerReportDTO
                {
                    RowType = 0, // Đầu kỳ
                    VoucherDate = new DateTime(2023, 1, 1),
                    VoucherNo = "",
                    Contents = "Số dư đầu kỳ",
                    AccountId = "1111",
                    CounterAccountId = "",
                    DebitAmount = 0,
                    CreditAmount = 0,
                    Balance = 150000000m,
                    JournalVoucherId = null
                },
                new LedgerReportDTO
                {
                    RowType = 1, // Phát sinh
                    VoucherDate = new DateTime(2023, 1, 5),
                    VoucherNo = "PT001",
                    Contents = "Rút tiền gửi ngân hàng nhập quỹ",
                    AccountId = "1111",
                    CounterAccountId = "1121",
                    DebitAmount = 50000000m,
                    CreditAmount = 0,
                    Balance = 200000000m,
                    JournalVoucherId = 1
                },
                new LedgerReportDTO
                {
                    RowType = 1, // Phát sinh
                    VoucherDate = new DateTime(2023, 1, 10),
                    VoucherNo = "PC001",
                    Contents = "Chi tiền điện thoại",
                    AccountId = "1111",
                    CounterAccountId = "642",
                    DebitAmount = 0,
                    CreditAmount = 2000000m,
                    Balance = 198000000m,
                    JournalVoucherId = 2
                },
                new LedgerReportDTO
                {
                    RowType = 2, // Cộng phát sinh
                    VoucherDate = new DateTime(2023, 1, 31),
                    VoucherNo = "",
                    Contents = "Cộng phát sinh",
                    AccountId = "1111",
                    CounterAccountId = "",
                    DebitAmount = 50000000m,
                    CreditAmount = 2000000m,
                    Balance = 198000000m,
                    JournalVoucherId = null
                },
                new LedgerReportDTO
                {
                    RowType = 3, // Dư cuối kỳ
                    VoucherDate = new DateTime(2023, 1, 31),
                    VoucherNo = "",
                    Contents = "Số dư cuối kỳ",
                    AccountId = "1111",
                    CounterAccountId = "",
                    DebitAmount = 0,
                    CreditAmount = 0,
                    Balance = 198000000m,
                    JournalVoucherId = null
                }
            };
        }

        public void ExportMockToJson(string filePath)
        {
            var data = GenerateMockList();
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(data, options);
            File.WriteAllText(filePath, jsonString);
        }

        public void ExportSchemaToXsd(string filePath)
        {
            using (var ds = new DataSet("LedgerReportData"))
            {
                var dt = new DataTable("LedgerReportDTO");
                foreach (var prop in typeof(LedgerReportDTO).GetProperties())
                {
                    dt.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
                }
                ds.Tables.Add(dt);
                ds.WriteXmlSchema(filePath);
            }
        }
    }
}
