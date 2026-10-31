using System;
using System.IO;
using System.Linq;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using NewaccNet.Contracts;

namespace NewaccNet.Office
{
    public interface IDraftWorkbookRenderer
    {
        /// <summary>
        /// Tạo trang tính Excel trên MemoryStream từ VoucherDraft.
        /// Tuyệt đối Zero-Disk I/O. Sau khi tạo xong con trỏ stream được Seek về 0.
        /// </summary>
        MemoryStream RenderToStream(VoucherDraft draft);
    }

    public class DraftWorkbookRenderer : IDraftWorkbookRenderer
    {
        public MemoryStream RenderToStream(VoucherDraft draft)
        {
            if (draft == null) throw new ArgumentNullException(nameof(draft));

            var stream = new MemoryStream();
            using (var doc = SpreadsheetDocument.Create(stream, SpreadsheetDocumentType.Workbook, true))
            {
                var workbookPart = doc.AddWorkbookPart();
                workbookPart.Workbook = new Workbook();

                var sheets = doc.WorkbookPart!.Workbook.AppendChild(new Sheets());

                // Sheet 1: Header (Thông tin chung)
                var headerPart = workbookPart.AddNewPart<WorksheetPart>();
                headerPart.Worksheet = CreateHeaderWorksheet(draft);
                sheets.Append(new Sheet
                {
                    Id = doc.WorkbookPart.GetIdOfPart(headerPart),
                    SheetId = 1,
                    Name = "ThongTinChung"
                });

                // Sheet 2: Lines (Chi tiết hàng hóa & Ánh xạ mã)
                var linesPart = workbookPart.AddNewPart<WorksheetPart>();
                linesPart.Worksheet = CreateLinesWorksheet(draft);
                sheets.Append(new Sheet
                {
                    Id = doc.WorkbookPart.GetIdOfPart(linesPart),
                    SheetId = 2,
                    Name = "ChiTietHangHoa"
                });

                // Sheet 3: Issues (Cảnh báo nếu có)
                if (draft.Issues != null && draft.Issues.Count > 0)
                {
                    var issuesPart = workbookPart.AddNewPart<WorksheetPart>();
                    issuesPart.Worksheet = CreateIssuesWorksheet(draft);
                    sheets.Append(new Sheet
                    {
                        Id = doc.WorkbookPart.GetIdOfPart(issuesPart),
                        SheetId = 3,
                        Name = "CanhBao"
                    });
                }

                // Nhúng VoucherDraft JSON vào CustomXmlPart
                DraftWorkbookPackage.EmbedDraftJson(doc, draft);

                workbookPart.Workbook.Save();
            }

            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }

        private static Worksheet CreateHeaderWorksheet(VoucherDraft draft)
        {
            var sheetData = new SheetData();

            sheetData.Append(CreateRow(1, "THÔNG TIN HÓA ĐƠN & CHỨNG TỪ", ""));
            sheetData.Append(CreateRow(2, "Số hóa đơn:", draft.Invoice.Number));
            sheetData.Append(CreateRow(3, "Ký hiệu mẫu & số:", $"{draft.Invoice.Template} - {draft.Invoice.Series}"));
            sheetData.Append(CreateRow(4, "Ngày hóa đơn:", draft.Invoice.Date.ToString("dd/MM/yyyy")));
            sheetData.Append(CreateRow(5, "Đơn vị bán / Đối tác:", draft.Counterparty.Raw.Name));
            sheetData.Append(CreateRow(6, "Mã số thuế:", draft.Counterparty.Raw.TaxCode));
            sheetData.Append(CreateRow(7, "Địa chỉ:", draft.Counterparty.Raw.Address));
            sheetData.Append(CreateRow(8, "Mã đối tác (hệ thống):", draft.Counterparty.Resolved.PartnerId));
            sheetData.Append(CreateRow(9, "Tổng tiền trước thuế:", draft.Invoice.AmountBeforeTax.ToString("N0")));
            sheetData.Append(CreateRow(10, "Tiền thuế GTGT:", draft.Invoice.VatAmount.ToString("N0")));
            sheetData.Append(CreateRow(11, "Tổng thanh toán:", draft.Invoice.TotalAmount.ToString("N0")));
            sheetData.Append(CreateRow(12, "Số chứng từ sinh ra:", draft.Header.VoucherNo ?? ""));
            sheetData.Append(CreateRow(13, "Trạng thái nháp:", draft.Status.ToString()));

            return new Worksheet(sheetData);
        }

        private static Worksheet CreateLinesWorksheet(VoucherDraft draft)
        {
            var sheetData = new SheetData();

            // Header row
            var headerRow = new Row { RowIndex = 1 };
            string[] headers = new[]
            {
                "STT", "Tên hàng (HĐ gốc)", "ĐVT", "Số lượng", "Đơn giá", "Thành tiền", "Thuế suất", "Tiền thuế",
                "Mã VT (Hệ thống)", "Mã kho", "TK Nợ", "TK Thuế", "Nguồn gợi ý", "Độ tin cậy", "Căn cứ gợi ý"
            };

            for (int i = 0; i < headers.Length; i++)
            {
                headerRow.Append(CreateCell(GetColName(i + 1) + "1", headers[i]));
            }
            sheetData.Append(headerRow);

            // Data rows
            uint rowIndex = 2;
            foreach (var line in draft.Lines)
            {
                var row = new Row { RowIndex = rowIndex };

                var primaryMatch = line.Matches?.FirstOrDefault();
                string method = primaryMatch?.Method.ToString() ?? "";
                string conf = primaryMatch != null ? $"{primaryMatch.Confidence * 100:0}%" : "";
                string reason = primaryMatch?.Reason ?? "";

                row.Append(CreateCell("A" + rowIndex, line.LineNo.ToString()));
                row.Append(CreateCell("B" + rowIndex, line.Raw.ItemName));
                row.Append(CreateCell("C" + rowIndex, line.Raw.Unit));
                row.Append(CreateCell("D" + rowIndex, line.Raw.Quantity.ToString("G29")));
                row.Append(CreateCell("E" + rowIndex, line.Raw.UnitPrice.ToString("N0")));
                row.Append(CreateCell("F" + rowIndex, line.Raw.Amount.ToString("N0")));
                row.Append(CreateCell("G" + rowIndex, line.Raw.VatRate.ToString("G29") + "%"));
                row.Append(CreateCell("H" + rowIndex, line.Raw.VatAmount.ToString("N0")));

                // Các cột resolved (người dùng có thể sửa)
                row.Append(CreateCell("I" + rowIndex, line.Resolved.MaterialId));
                row.Append(CreateCell("J" + rowIndex, line.Resolved.WarehouseId));
                row.Append(CreateCell("K" + rowIndex, line.Resolved.DebitAccountId));
                row.Append(CreateCell("L" + rowIndex, line.Resolved.VatAccountId));

                // Các cột giải thích
                row.Append(CreateCell("M" + rowIndex, method));
                row.Append(CreateCell("N" + rowIndex, conf));
                row.Append(CreateCell("O" + rowIndex, reason));

                sheetData.Append(row);
                rowIndex++;
            }

            return new Worksheet(sheetData);
        }

        private static Worksheet CreateIssuesWorksheet(VoucherDraft draft)
        {
            var sheetData = new SheetData();
            var headerRow = new Row { RowIndex = 1 };
            headerRow.Append(CreateCell("A1", "Mức độ"));
            headerRow.Append(CreateCell("B1", "Mã lỗi"));
            headerRow.Append(CreateCell("C1", "Vị trí"));
            headerRow.Append(CreateCell("D1", "Nội dung cảnh báo"));
            sheetData.Append(headerRow);

            uint r = 2;
            foreach (var issue in draft.Issues)
            {
                var row = new Row { RowIndex = r };
                row.Append(CreateCell("A" + r, issue.Severity.ToString()));
                row.Append(CreateCell("B" + r, issue.Code));
                row.Append(CreateCell("C" + r, issue.Field));
                row.Append(CreateCell("D" + r, issue.Message));
                sheetData.Append(row);
                r++;
            }

            return new Worksheet(sheetData);
        }

        private static Row CreateRow(uint rowIndex, string colA, string colB)
        {
            var row = new Row { RowIndex = rowIndex };
            row.Append(CreateCell("A" + rowIndex, colA));
            row.Append(CreateCell("B" + rowIndex, colB));
            return row;
        }

        private static Cell CreateCell(string cellReference, string value)
        {
            return new Cell
            {
                CellReference = cellReference,
                DataType = CellValues.String,
                CellValue = new CellValue(value ?? string.Empty)
            };
        }

        private static string GetColName(int colIndex)
        {
            int dividend = colIndex;
            string colName = string.Empty;
            while (dividend > 0)
            {
                int modulo = (dividend - 1) % 26;
                colName = Convert.ToChar(65 + modulo) + colName;
                dividend = (dividend - modulo) / 26;
            }
            return colName;
        }
    }
}
