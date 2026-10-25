using System;
using System.Collections.Generic;

namespace NewaccNet.AI.Contracts
{
    public enum OfficeOutputType
    {
        ChatOnly,
        Spreadsheet,
        WordDocument
    }

    public class OfficeChatResponseDto
    {
        public OfficeOutputType OutputType { get; set; } = OfficeOutputType.ChatOnly;
        public string ChatMessage { get; set; } = string.Empty;

        // Dữ liệu bảng tính Excel nếu OutputType == Spreadsheet
        public DynamicSpreadsheetDto? Spreadsheet { get; set; }

        // Dữ liệu văn bản Word nếu OutputType == WordDocument
        public DynamicWordDocumentDto? WordDocument { get; set; }
    }

    public class DynamicSpreadsheetDto
    {
        public string Title { get; set; } = "BẢNG KÊ BÁO CÁO KẾ TOÁN";
        public string Subtitle { get; set; } = string.Empty;
        public string SheetName { get; set; } = "BaoCao";
        public List<DynamicColumnDto> Columns { get; set; } = new();
        public List<List<string>> Rows { get; set; } = new();
        public List<string>? SummaryRow { get; set; }
    }

    public class DynamicColumnDto
    {
        public string Header { get; set; } = string.Empty;
        public string Alignment { get; set; } = "Left"; // Left, Right, Center
        public string DataType { get; set; } = "Text";   // Text, Number, Currency, Date, Percentage
    }

    public class DynamicWordDocumentDto
    {
        public string Title { get; set; } = "BÁO CÁO QUẢN TRỊ DOANH NGHIỆP";
        public string Subtitle { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public List<WordDocumentSectionDto> Sections { get; set; } = new();
    }

    public class WordDocumentSectionDto
    {
        public string Heading { get; set; } = string.Empty;
        public List<string> Paragraphs { get; set; } = new();
        public List<string> BulletPoints { get; set; } = new();
        public string? HighlightNote { get; set; }
    }
}
