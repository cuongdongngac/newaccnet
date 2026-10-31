using System;
using System.IO;
using System.Linq;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using NewaccNet.Contracts;

namespace NewaccNet.Office
{
    public interface IDraftWorkbookReader
    {
        /// <summary>
        /// Đọc ngược lại VoucherDraft từ Stream của file Excel (đã được người dùng sửa trên Office Studio).
        /// </summary>
        VoucherDraft? ReadFromStream(Stream stream);
    }

    public class DraftWorkbookReader : IDraftWorkbookReader
    {
        public VoucherDraft? ReadFromStream(Stream stream)
        {
            if (stream == null) return null;

            using var doc = SpreadsheetDocument.Open(stream, false);

            // 1. Bóc tách JSON gốc trong CustomXmlPart
            var draft = DraftWorkbookPackage.ExtractDraftJson(doc);
            if (draft == null) return null;

            // 2. Đọc các ô trên Sheet "ChiTietHangHoa" để bắt sự thay đổi của người dùng
            var workbookPart = doc.WorkbookPart;
            if (workbookPart == null) return draft;

            var linesSheet = workbookPart.Workbook.Sheets?
                .Elements<Sheet>()
                .FirstOrDefault(s => s.Name == "ChiTietHangHoa");

            if (linesSheet == null || linesSheet.Id == null) return draft;

            var worksheetPart = (WorksheetPart)workbookPart.GetPartById(linesSheet.Id!);
            var sheetData = worksheetPart.Worksheet.Elements<SheetData>().FirstOrDefault();
            if (sheetData == null) return draft;

            var rows = sheetData.Elements<Row>().Skip(1).ToList(); // Bỏ qua header row 1

            for (int i = 0; i < Math.Min(rows.Count, draft.Lines.Count); i++)
            {
                var row = rows[i];
                var line = draft.Lines[i];

                string matId = GetCellValue(doc, row, "I" + row.RowIndex);
                string whId = GetCellValue(doc, row, "J" + row.RowIndex);
                string debAcc = GetCellValue(doc, row, "K" + row.RowIndex);
                string vatAcc = GetCellValue(doc, row, "L" + row.RowIndex);

                bool changed = false;
                if (!string.IsNullOrWhiteSpace(matId) && !matId.Equals(line.Resolved.MaterialId, StringComparison.OrdinalIgnoreCase))
                {
                    line.Resolved.MaterialId = matId;
                    changed = true;
                }
                if (!string.IsNullOrWhiteSpace(whId) && !whId.Equals(line.Resolved.WarehouseId, StringComparison.OrdinalIgnoreCase))
                {
                    line.Resolved.WarehouseId = whId;
                    changed = true;
                }
                if (!string.IsNullOrWhiteSpace(debAcc) && !debAcc.Equals(line.Resolved.DebitAccountId, StringComparison.OrdinalIgnoreCase))
                {
                    line.Resolved.DebitAccountId = debAcc;
                    changed = true;
                }
                if (!string.IsNullOrWhiteSpace(vatAcc) && !vatAcc.Equals(line.Resolved.VatAccountId, StringComparison.OrdinalIgnoreCase))
                {
                    line.Resolved.VatAccountId = vatAcc;
                    changed = true;
                }

                if (changed)
                {
                    line.Matches.Insert(0, new MatchInfo
                    {
                        Field = "ManualEdit",
                        Method = MatchMethod.User,
                        Confidence = 1.0,
                        Reason = "Người dùng chỉnh sửa trực tiếp trên bảng tính Excel"
                    });

                    draft.Audit.Add(new DraftAuditEntry
                    {
                        At = DateTime.Now,
                        By = "User",
                        Action = "EditRow",
                        Detail = $"Dòng {line.LineNo}: Đã cập nhật mã {line.Resolved.MaterialId}, kho {line.Resolved.WarehouseId}, TK {line.Resolved.DebitAccountId}"
                    });
                }
            }

            return draft;
        }

        private static string GetCellValue(SpreadsheetDocument doc, Row row, string cellReference)
        {
            var cell = row.Elements<Cell>().FirstOrDefault(c => c.CellReference == cellReference);
            if (cell == null || cell.CellValue == null) return string.Empty;

            string value = cell.CellValue.Text;
            if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
            {
                var stringTable = doc.WorkbookPart?.SharedStringTablePart?.SharedStringTable;
                if (stringTable != null && int.TryParse(value, out int id))
                {
                    return stringTable.ElementAt(id).InnerText;
                }
            }

            return value;
        }
    }
}
