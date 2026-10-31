using System;
using System.Collections.Generic;
using NewaccNet.Contracts;
using NewaccNet.Office;
using Xunit;

namespace NewaccNet.ArchitectureTests
{
    public class DraftWorkbookTests
    {
        [Fact]
        public void RenderAndRead_DraftWorkbook_ShouldPreserveDataAndZeroDiskIO()
        {
            var draft = new VoucherDraft
            {
                DraftId = "DRAFT_EXCEL_TEST_101",
                DraftType = DraftType.PurchaseInvoice,
                Header = new DraftHeader
                {
                    VoucherDate = new DateTime(2026, 10, 31),
                    VoucherNo = "NM26103101",
                    Contents = "Hóa đơn mua vật tư thiết bị văn phòng"
                },
                Counterparty = new DraftCounterparty
                {
                    Raw = new CounterpartyRaw { Name = "Công ty Cổ phần Alpha", TaxCode = "0108888888", Address = "Hà Nội" },
                    Resolved = new CounterpartyResolved { PartnerId = "ALPHA01" }
                },
                Invoice = new DraftInvoiceInfo
                {
                    Number = "0005678",
                    Series = "C26TBB",
                    Date = new DateTime(2026, 10, 31),
                    AmountBeforeTax = 20000000,
                    VatAmount = 2000000,
                    TotalAmount = 22000000
                },
                Lines = new List<DraftLine>
                {
                    new DraftLine
                    {
                        LineNo = 1,
                        Raw = new DraftLineRaw
                        {
                            ItemName = "Máy in đa năng Canon",
                            Unit = "Chiếc",
                            Quantity = 2,
                            UnitPrice = 10000000,
                            Amount = 20000000,
                            VatRate = 10,
                            VatAmount = 2000000
                        },
                        Resolved = new DraftLineResolved
                        {
                            MaterialId = "MAYIN01",
                            WarehouseId = "KHO_TB",
                            DebitAccountId = "1531",
                            VatAccountId = "13311"
                        }
                    }
                }
            };

            // 1. Render ra MemoryStream
            var renderer = new DraftWorkbookRenderer();
            using var stream = renderer.RenderToStream(draft);

            // Kiểm tra Zero-Disk I/O và con trỏ ở vị trí 0
            Assert.NotNull(stream);
            Assert.True(stream.Length > 0);
            Assert.Equal(0, stream.Position);

            // 2. Đọc ngược lại từ stream
            var reader = new DraftWorkbookReader();
            var readDraft = reader.ReadFromStream(stream);

            Assert.NotNull(readDraft);
            Assert.Equal("DRAFT_EXCEL_TEST_101", readDraft.DraftId);
            Assert.Equal("0005678", readDraft.Invoice.Number);
            Assert.Equal("Công ty Cổ phần Alpha", readDraft.Counterparty.Raw.Name);
            Assert.Single(readDraft.Lines);
            Assert.Equal("MAYIN01", readDraft.Lines[0].Resolved.MaterialId);
            Assert.Equal("1531", readDraft.Lines[0].Resolved.DebitAccountId);
        }
    }
}
