using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using NewaccNet.AI.Contracts;

namespace NewaccNet.AI.Services
{
    public class OpenXmlReportService
    {
        /// <summary>
        /// Tạo bảng tính Excel trong bộ nhớ RAM từ danh sách chi tiết hóa đơn/vật tư (Zero-Disk I/O).
        /// Luôn trả về MemoryStream đã Seek(0, SeekOrigin.Begin).
        /// </summary>
        public MemoryStream GenerateInvoiceItemsSpreadsheet(
            string reportTitle,
            string partnerName,
            string invoiceInfo,
            IEnumerable<OcrInvoiceItemDto> items)
        {
            var ms = new MemoryStream();
            using (var document = SpreadsheetDocument.Create(ms, SpreadsheetDocumentType.Workbook, true))
            {
                var workbookPart = document.AddWorkbookPart();
                workbookPart.Workbook = new Workbook();

                var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                var sheetData = new SheetData();
                worksheetPart.Worksheet = new Worksheet(sheetData);

                var sheets = workbookPart.Workbook.AppendChild(new Sheets());
                sheets.Append(new Sheet
                {
                    Id = workbookPart.GetIdOfPart(worksheetPart),
                    SheetId = 1,
                    Name = "ChiTietVatTu"
                });

                // Style part
                var stylesPart = workbookPart.AddNewPart<WorkbookStylesPart>();
                stylesPart.Stylesheet = CreateDefaultStylesheet();
                stylesPart.Stylesheet.Save();

                uint rowIdx = 1;

                // 1. Title Row
                var titleRow = new Row { RowIndex = rowIdx++ };
                titleRow.Append(CreateTextCell("A", titleRow.RowIndex.Value, reportTitle.ToUpperInvariant()));
                sheetData.Append(titleRow);

                // 2. Info Rows
                var infoRow1 = new Row { RowIndex = rowIdx++ };
                infoRow1.Append(CreateTextCell("A", infoRow1.RowIndex.Value, $"Đối tác: {partnerName}"));
                sheetData.Append(infoRow1);

                var infoRow2 = new Row { RowIndex = rowIdx++ };
                infoRow2.Append(CreateTextCell("A", infoRow2.RowIndex.Value, $"Chứng từ: {invoiceInfo}"));
                sheetData.Append(infoRow2);

                rowIdx++; // Dòng trống

                // 3. Header Table Row
                var headerRow = new Row { RowIndex = rowIdx++ };
                headerRow.Append(CreateTextCell("A", headerRow.RowIndex.Value, "STT"));
                headerRow.Append(CreateTextCell("B", headerRow.RowIndex.Value, "Mã Hàng"));
                headerRow.Append(CreateTextCell("C", headerRow.RowIndex.Value, "Tên Hàng Hóa / Dịch Vụ"));
                headerRow.Append(CreateTextCell("D", headerRow.RowIndex.Value, "ĐVT"));
                headerRow.Append(CreateTextCell("E", headerRow.RowIndex.Value, "Số Lượng"));
                headerRow.Append(CreateTextCell("F", headerRow.RowIndex.Value, "Đơn Giá"));
                headerRow.Append(CreateTextCell("G", headerRow.RowIndex.Value, "Thành Tiền"));
                headerRow.Append(CreateTextCell("H", headerRow.RowIndex.Value, "Thuế Suất"));
                headerRow.Append(CreateTextCell("I", headerRow.RowIndex.Value, "Tiền Thuế"));
                headerRow.Append(CreateTextCell("J", headerRow.RowIndex.Value, "TK Nợ (Gợi ý)"));
                headerRow.Append(CreateTextCell("K", headerRow.RowIndex.Value, "TK Có (Gợi ý)"));
                headerRow.Append(CreateTextCell("L", headerRow.RowIndex.Value, "Mã Kho (Gợi ý)"));
                sheetData.Append(headerRow);

                // 4. Data Rows
                int stt = 1;
                decimal totalAmount = 0;
                decimal totalTax = 0;

                foreach (var item in items)
                {
                    var dataRow = new Row { RowIndex = rowIdx++ };
                    dataRow.Append(CreateNumberCell("A", dataRow.RowIndex.Value, stt++));
                    dataRow.Append(CreateTextCell("B", dataRow.RowIndex.Value, item.ItemCode ?? ""));
                    dataRow.Append(CreateTextCell("C", dataRow.RowIndex.Value, item.ItemName ?? ""));
                    dataRow.Append(CreateTextCell("D", dataRow.RowIndex.Value, item.Unit ?? ""));
                    dataRow.Append(CreateNumberCell("E", dataRow.RowIndex.Value, (decimal)item.Quantity));
                    dataRow.Append(CreateNumberCell("F", dataRow.RowIndex.Value, item.UnitPrice));
                    dataRow.Append(CreateNumberCell("G", dataRow.RowIndex.Value, item.Amount));
                    dataRow.Append(CreateTextCell("H", dataRow.RowIndex.Value, item.TaxRate ?? ""));
                    dataRow.Append(CreateNumberCell("I", dataRow.RowIndex.Value, item.TaxAmount));
                    dataRow.Append(CreateTextCell("J", dataRow.RowIndex.Value, "156"));
                    dataRow.Append(CreateTextCell("K", dataRow.RowIndex.Value, "331"));
                    dataRow.Append(CreateTextCell("L", dataRow.RowIndex.Value, "KHO_HH"));
                    sheetData.Append(dataRow);

                    totalAmount += item.Amount;
                    totalTax += item.TaxAmount;
                }

                // 5. Total Row
                var totalRow = new Row { RowIndex = rowIdx++ };
                totalRow.Append(CreateTextCell("C", totalRow.RowIndex.Value, "TỔNG CỘNG:"));
                totalRow.Append(CreateNumberCell("G", totalRow.RowIndex.Value, totalAmount));
                totalRow.Append(CreateNumberCell("I", totalRow.RowIndex.Value, totalTax));
                sheetData.Append(totalRow);

                worksheetPart.Worksheet.Save();
                workbookPart.Workbook.Save();
            }

            // Đảm bảo Seek về đầu stream để DevExpress SpreadsheetControl đọc được ngay
            ms.Seek(0, SeekOrigin.Begin);
            return ms;
        }

        /// <summary>
        /// Tạo văn bản Word trong bộ nhớ RAM từ phân tích điều hành CFO (Zero-Disk I/O).
        /// </summary>
        public MemoryStream GenerateExecutiveWordReport(ExecutiveInsightAiDto insight, string companyName)
        {
            var ms = new MemoryStream();
            using (var document = WordprocessingDocument.Create(ms, WordprocessingDocumentType.Document, true))
            {
                var mainPart = document.AddMainDocumentPart();
                mainPart.Document = new DocumentFormat.OpenXml.Wordprocessing.Document();
                var body = mainPart.Document.AppendChild(new Body());

                // Title
                body.Append(CreateParagraph(insight.Title.ToUpperInvariant(), bold: true, fontSize: 32));
                body.Append(CreateParagraph($"Đơn vị: {companyName} - Ngày lập: {DateTime.Now:dd/MM/yyyy HH:mm}", italic: true));
                body.Append(CreateParagraph($"Đánh giá sức khỏe tài chính: {insight.FinancialHealthStatus.ToUpperInvariant()}", bold: true));
                body.Append(CreateParagraph(""));

                // Summary
                body.Append(CreateParagraph("1. TỔNG QUAN TÌNH HÌNH TÀI CHÍNH & ĐIỀU HÀNH", bold: true, fontSize: 24));
                body.Append(CreateParagraph(insight.ExecutiveSummary));
                body.Append(CreateParagraph(""));

                // Cashflow
                body.Append(CreateParagraph("2. PHÂN TÍCH DÒNG TIỀN (CASH FLOW)", bold: true, fontSize: 24));
                body.Append(CreateParagraph(insight.CashFlowAnalysis));
                body.Append(CreateParagraph(""));

                // Revenue & Profit
                body.Append(CreateParagraph("3. DOANH THU & BIÊN LỢI NHUẬN", bold: true, fontSize: 24));
                body.Append(CreateParagraph(insight.RevenueProfitAnalysis));
                body.Append(CreateParagraph(""));

                // Debt
                body.Append(CreateParagraph("4. RỦI RO CÔNG NỢ & THU HỒI VỐN", bold: true, fontSize: 24));
                body.Append(CreateParagraph(insight.DebtRiskAnalysis));
                body.Append(CreateParagraph(""));

                // Alerts
                if (insight.KeyAlerts.Count > 0)
                {
                    body.Append(CreateParagraph("5. CÁC ĐIỂM NÓNG & CẢNH BÁO NGUY CƠ", bold: true, fontSize: 24));
                    foreach (var alert in insight.KeyAlerts)
                    {
                        body.Append(CreateParagraph("• " + alert));
                    }
                    body.Append(CreateParagraph(""));
                }

                // Recommendations
                if (insight.ActionableRecommendations.Count > 0)
                {
                    body.Append(CreateParagraph("6. KHUYẾN NGHỊ HÀNH ĐỘNG CHO BAN GIÁM ĐỐC", bold: true, fontSize: 24));
                    int recIdx = 1;
                    foreach (var rec in insight.ActionableRecommendations)
                    {
                        body.Append(CreateParagraph($"{recIdx++}. {rec}"));
                    }
                }

                mainPart.Document.Save();
            }

            ms.Seek(0, SeekOrigin.Begin);
            return ms;
        }

        /// <summary>
        /// Tạo bảng tính Excel động từ dữ liệu do AI Chatbot phân tích và đề xuất (Zero-Disk I/O).
        /// </summary>
        public MemoryStream GenerateDynamicSpreadsheetStream(DynamicSpreadsheetDto dto)
        {
            var ms = new MemoryStream();
            using (var document = SpreadsheetDocument.Create(ms, SpreadsheetDocumentType.Workbook, true))
            {
                var workbookPart = document.AddWorkbookPart();
                workbookPart.Workbook = new Workbook();

                var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                var sheetData = new SheetData();
                worksheetPart.Worksheet = new Worksheet(sheetData);

                var sheets = workbookPart.Workbook.AppendChild(new Sheets());
                sheets.Append(new Sheet
                {
                    Id = workbookPart.GetIdOfPart(worksheetPart),
                    SheetId = 1,
                    Name = string.IsNullOrWhiteSpace(dto.SheetName) ? "BaoCao" : dto.SheetName
                });

                var stylesPart = workbookPart.AddNewPart<WorkbookStylesPart>();
                stylesPart.Stylesheet = CreateDefaultStylesheet();
                stylesPart.Stylesheet.Save();

                uint rowIdx = 1;

                // 1. Tiêu đề chính
                var titleRow = new Row { RowIndex = rowIdx++ };
                titleRow.Append(CreateTextCell("A", titleRow.RowIndex.Value, (dto.Title ?? "BÁO CÁO KẾ TOÁN").ToUpperInvariant()));
                sheetData.Append(titleRow);

                // 2. Tiêu đề phụ (nếu có)
                if (!string.IsNullOrWhiteSpace(dto.Subtitle))
                {
                    var subRow = new Row { RowIndex = rowIdx++ };
                    subRow.Append(CreateTextCell("A", subRow.RowIndex.Value, dto.Subtitle));
                    sheetData.Append(subRow);
                }

                // Dòng trống
                sheetData.Append(new Row { RowIndex = rowIdx++ });

                // 3. Header Cột
                var headerRow = new Row { RowIndex = rowIdx++ };
                for (int c = 0; c < dto.Columns.Count; c++)
                {
                    string colLetter = GetColumnLetter(c);
                    headerRow.Append(CreateTextCell(colLetter, headerRow.RowIndex.Value, dto.Columns[c].Header));
                }
                sheetData.Append(headerRow);

                // 4. Các dòng dữ liệu
                foreach (var rowData in dto.Rows)
                {
                    var dataRow = new Row { RowIndex = rowIdx++ };
                    for (int c = 0; c < rowData.Count && c < dto.Columns.Count; c++)
                    {
                        string colLetter = GetColumnLetter(c);
                        string rawVal = rowData[c] ?? string.Empty;

                        // Nếu là số thì ghi NumberCell
                        if (decimal.TryParse(rawVal.Replace(",", "").Replace("%", "").Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out var numVal))
                        {
                            dataRow.Append(CreateNumberCell(colLetter, dataRow.RowIndex.Value, numVal));
                        }
                        else
                        {
                            dataRow.Append(CreateTextCell(colLetter, dataRow.RowIndex.Value, rawVal));
                        }
                    }
                    sheetData.Append(dataRow);
                }

                // 5. Dòng tổng cộng (nếu có)
                if (dto.SummaryRow != null && dto.SummaryRow.Count > 0)
                {
                    var sumRow = new Row { RowIndex = rowIdx++ };
                    for (int c = 0; c < dto.SummaryRow.Count && c < dto.Columns.Count; c++)
                    {
                        string colLetter = GetColumnLetter(c);
                        string rawVal = dto.SummaryRow[c] ?? string.Empty;
                        if (decimal.TryParse(rawVal.Replace(",", "").Replace("%", "").Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out var numVal))
                        {
                            sumRow.Append(CreateNumberCell(colLetter, sumRow.RowIndex.Value, numVal));
                        }
                        else
                        {
                            sumRow.Append(CreateTextCell(colLetter, sumRow.RowIndex.Value, rawVal));
                        }
                    }
                    sheetData.Append(sumRow);
                }

                workbookPart.Workbook.Save();
            }

            ms.Seek(0, SeekOrigin.Begin);
            return ms;
        }

        /// <summary>
        /// Tạo văn bản Word động từ dữ liệu do AI Chatbot phân tích hoặc soạn thảo (Zero-Disk I/O).
        /// </summary>
        public MemoryStream GenerateDynamicWordStream(DynamicWordDocumentDto dto)
        {
            var ms = new MemoryStream();
            using (var document = WordprocessingDocument.Create(ms, WordprocessingDocumentType.Document, true))
            {
                var mainPart = document.AddMainDocumentPart();
                mainPart.Document = new DocumentFormat.OpenXml.Wordprocessing.Document();
                var body = mainPart.Document.AppendChild(new Body());

                // Title
                body.Append(CreateParagraph((dto.Title ?? "VĂN BẢN QUẢN TRỊ").ToUpperInvariant(), bold: true, fontSize: 32));

                if (!string.IsNullOrWhiteSpace(dto.CompanyName))
                {
                    body.Append(CreateParagraph($"Đơn vị: {dto.CompanyName}", bold: true, fontSize: 22));
                }

                if (!string.IsNullOrWhiteSpace(dto.Subtitle))
                {
                    body.Append(CreateParagraph(dto.Subtitle, italic: true, fontSize: 20));
                }

                body.Append(CreateParagraph(""));

                // Các Section
                foreach (var sec in dto.Sections)
                {
                    if (!string.IsNullOrWhiteSpace(sec.Heading))
                    {
                        body.Append(CreateParagraph(sec.Heading, bold: true, fontSize: 24));
                    }

                    foreach (var p in sec.Paragraphs)
                    {
                        body.Append(CreateParagraph(p, fontSize: 22));
                    }

                    foreach (var b in sec.BulletPoints)
                    {
                        body.Append(CreateParagraph($"• {b}", fontSize: 22));
                    }

                    if (!string.IsNullOrWhiteSpace(sec.HighlightNote))
                    {
                        body.Append(CreateParagraph($"[Lưu ý quan trọng]: {sec.HighlightNote}", italic: true, bold: true, fontSize: 22));
                    }

                    body.Append(CreateParagraph(""));
                }

                mainPart.Document.Save();
            }

            ms.Seek(0, SeekOrigin.Begin);
            return ms;
        }

        private static string GetColumnLetter(int colIndex)
        {
            string colLetter = "";
            while (colIndex >= 0)
            {
                colLetter = (char)('A' + (colIndex % 26)) + colLetter;
                colIndex = (colIndex / 26) - 1;
            }
            return colLetter;
        }

        // ==========================================================
        // HELPERS
        // ==========================================================

        private static Cell CreateTextCell(string col, uint row, string text)
        {
            return new Cell
            {
                CellReference = $"{col}{row}",
                DataType = CellValues.String,
                CellValue = new CellValue(text ?? string.Empty)
            };
        }

        private static Cell CreateNumberCell(string col, uint row, decimal value)
        {
            return new Cell
            {
                CellReference = $"{col}{row}",
                DataType = CellValues.Number,
                CellValue = new CellValue(value.ToString(CultureInfo.InvariantCulture))
            };
        }

        private static Stylesheet CreateDefaultStylesheet()
        {
            return new Stylesheet(
                new DocumentFormat.OpenXml.Spreadsheet.Fonts(new DocumentFormat.OpenXml.Spreadsheet.Font(new DocumentFormat.OpenXml.Spreadsheet.FontSize { Val = 11 }, new FontName { Val = "Segoe UI" })),
                new Fills(new Fill(new PatternFill { PatternType = PatternValues.None }), new Fill(new PatternFill { PatternType = PatternValues.Gray125 })),
                new Borders(new DocumentFormat.OpenXml.Spreadsheet.Border()),
                new CellFormats(new CellFormat { FontId = 0, FillId = 0, BorderId = 0 })
            );
        }

        private static Paragraph CreateParagraph(string text, bool bold = false, bool italic = false, int fontSize = 22)
        {
            var p = new Paragraph();
            var r = p.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.Run());
            var rPr = r.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.RunProperties());

            if (bold) rPr.Append(new DocumentFormat.OpenXml.Wordprocessing.Bold());
            if (italic) rPr.Append(new DocumentFormat.OpenXml.Wordprocessing.Italic());
            rPr.Append(new DocumentFormat.OpenXml.Wordprocessing.FontSize { Val = fontSize.ToString() });

            r.Append(new DocumentFormat.OpenXml.Wordprocessing.Text(text ?? string.Empty));
            return p;
        }
    }
}
