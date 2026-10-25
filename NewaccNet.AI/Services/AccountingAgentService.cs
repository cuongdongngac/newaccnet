using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using NewaccNet.AI.Contracts;
using NewaccNet.AI.Gateway;
using NewaccNet.AI.Prompts;

namespace NewaccNet.AI.Services
{
    public class AccountingAgentService
    {
        private readonly OpenRouterGateway _gateway;

        public AccountingAgentService(OpenRouterGateway gateway)
        {
            _gateway = gateway ?? throw new ArgumentNullException(nameof(gateway));
        }

        /// <summary>
        /// Phân tích câu lệnh tự nhiên của kế toán viên thành đối tượng JournalEntryAiDto để nạp vào form chứng từ.
        /// </summary>
        public async Task<JournalEntryAiDto?> ParseNaturalLanguageJournalAsync(
            string userInput,
            string? chartOfAccountsContext = null,
            CancellationToken cancellationToken = default)
        {
            var systemPrompt = AiPromptTemplates.AccountingAssistantSystemPrompt;
            if (!string.IsNullOrWhiteSpace(chartOfAccountsContext))
            {
                systemPrompt += $"\n\n[HỆ THỐNG TÀI KHOẢN VÀ DANH MỤC THỰC TẾ TRONG CSDL HIỆN TẠI]:\n{chartOfAccountsContext}";
            }

            var userPrompt = $"Hãy phân tích nghiệp vụ kế toán sau và trả về JSON cấu trúc JournalEntryAiDto:\n\"{userInput.Trim()}\"";

            return await _gateway.SendStructuredRequestAsync<JournalEntryAiDto>(
                systemPrompt,
                userPrompt,
                cancellationToken: cancellationToken);
        }

        public async Task<JournalEntryAiDto?> ParseNaturalLanguageJournalAsync(
            string userInput,
            IEnumerable<(string AccountId, string AccountName)> availableAccounts,
            IEnumerable<(string Srcode, string Srname)> availableSources,
            CancellationToken cancellationToken = default)
        {
            var accStr = string.Join(", ", availableAccounts.Select(a => $"{a.AccountId}: {a.AccountName}"));
            var srcStr = string.Join(", ", availableSources.Select(s => $"{s.Srcode}: {s.Srname}"));
            return await ParseNaturalLanguageJournalAsync(userInput, $"{accStr}\nNguồn chứng từ: {srcStr}", cancellationToken);
        }

        /// <summary>
        /// Phân tích số liệu tài chính tổng hợp dành cho Giám đốc / Lãnh đạo (CFO Ảo).
        /// </summary>
        public async Task<ExecutiveInsightAiDto?> AnalyzeFinancialDataAsync(
            string leadershipQuestion,
            string financialSummaryContext,
            CancellationToken cancellationToken = default)
        {
            var systemPrompt = AiPromptTemplates.ExecutiveCfoSystemPrompt;
            var userPrompt = $"[CÂU HỎI CỦA LÃNH ĐẠO]:\n{leadershipQuestion.Trim()}\n\n[DỮ LIỆU TÀI CHÍNH TỔNG HỢP HIỆN TẠI]:\n{financialSummaryContext}\n\nHãy phân tích và trả về cấu trúc JSON ExecutiveInsightAiDto chuyên nghiệp cho Giám đốc.";

            return await _gateway.SendStructuredRequestAsync<ExecutiveInsightAiDto>(
                systemPrompt,
                userPrompt,
                cancellationToken: cancellationToken);
        }

        public async Task<ExecutiveInsightAiDto?> AnalyzeFinancialDataAsync(
            string companyName,
            string period,
            string balanceSheetJson,
            string incomeStatementJson,
            string userQuestion,
            CancellationToken cancellationToken = default)
        {
            var context = $"Doanh nghiệp: {companyName}\nKỳ phân tích: {period}\n\n[Bảng Cân đối Kế toán]:\n{balanceSheetJson}\n\n[Kết quả Hoạt động Kinh doanh]:\n{incomeStatementJson}";
            var question = string.IsNullOrWhiteSpace(userQuestion)
                ? "Phân tích toàn diện sức khỏe tài chính, dòng tiền và đưa ra khuyến nghị quản trị cho Ban Giám đốc."
                : userQuestion;

            return await AnalyzeFinancialDataAsync(question, context, cancellationToken);
        }

        public async Task SuggestInvoiceItemPostingAsync(
            OcrInvoiceResultDto invoice,
            IEnumerable<(string Code, string Name)> availableAccounts,
            IEnumerable<(string Code, string Name)> availableWarehouses,
            CancellationToken cancellationToken = default)
        {
            if (invoice?.Items == null || invoice.Items.Count == 0) return;

            var accContext = string.Join(", ", availableAccounts.Select(a => $"{a.Code}: {a.Name}"));
            var whContext = string.Join(", ", availableWarehouses.Select(w => $"{w.Code}: {w.Name}"));

            var suggestions = await SuggestItemPostingsAsync(invoice.Items, $"{accContext}\nKho: {whContext}", cancellationToken);
            foreach (var item in invoice.Items)
            {
                var match = suggestions.FirstOrDefault(s => s.LineNo == item.LineNo || s.ItemName == item.ItemName);
                if (match != null)
                {
                    item.SuggestedAccount = match.DebitAccount;
                    item.SuggestedWarehouse = match.WarehouseCode;
                }
                else
                {
                    item.SuggestedAccount = "156";
                    item.SuggestedWarehouse = "KHO_HH";
                }
            }
        }

        /// <summary>
        /// Gợi ý tài khoản Kho (152, 156, 153), Chi phí (642, 641, 154) và Kho nhập cho danh sách mặt hàng trên HĐĐT.
        /// </summary>
        public async Task<List<ItemPostingSuggestionDto>> SuggestItemPostingsAsync(
            IEnumerable<OcrInvoiceItemDto> items,
            string? accountsContext = null,
            CancellationToken cancellationToken = default)
        {
            var systemPrompt = @"Bạn là trợ lý kế toán vật tư. Hãy phân loại từng mặt hàng trên hóa đơn vào đúng:
- DebitAccount (TK Nợ): 152 (Nguyên vật liệu), 156 (Hàng hóa), 153 (CCDC), 642/641 (Chi phí dùng ngay), 242 (CP trả trước)...
- WarehouseCode (Kho đề xuất): KHO_NVL, KHO_HH, KHO_CCDC...
- Note: Giải thích ngắn gọn lý do phân loại.
Trả về JSON danh sách các phần tử khớp với ItemPostingSuggestionDto.";

            var itemsJson = JsonSerializer.Serialize(items);
            var userPrompt = $"[DANH SÁCH MẶT HÀNG TRÊN HÓA ĐƠN]:\n{itemsJson}";
            if (!string.IsNullOrWhiteSpace(accountsContext))
            {
                userPrompt += $"\n\n[DANH MỤC TK KHO/CHI PHÍ HIỆN CÓ]:\n{accountsContext}";
            }

            var result = await _gateway.SendStructuredRequestAsync<List<ItemPostingSuggestionDto>>(
                systemPrompt,
                userPrompt,
                cancellationToken: cancellationToken);

            return result ?? new List<ItemPostingSuggestionDto>();
        }

        /// <summary>
        /// Phân tích yêu cầu tự nhiên của người dùng trong Office Studio (kèm đính kèm ảnh/PDF/hóa đơn nếu có).
        /// Tự động xác định OutputType (ChatOnly, Spreadsheet, WordDocument) và tạo DTO tương ứng.
        /// </summary>
        public async Task<OfficeChatResponseDto?> ProcessOfficeChatAsync(
            string userPrompt,
            string? activeDocumentContext = null,
            string? fileDataUri = null,
            CancellationToken cancellationToken = default)
        {
            var systemPrompt = @"Bạn là trợ lý AI cao cấp tích hợp trong ứng dụng Kế toán Doanh nghiệp NewaccNet (Office Studio & AI).
Nhiệm vụ của bạn là lắng nghe câu hỏi/yêu cầu của người dùng, phân tích tài liệu đính kèm (hóa đơn, chứng từ, ảnh chụp scan hoặc file PDF nếu có) và quyết định định dạng kết quả phù hợp nhất:

1. NẾU NGƯỜI DÙNG YÊU CẦU DẠNG BẢNG BIỂU, HÓA ĐƠN MUA HÀNG, SỐ LIỆU TÍNH TOÁN, CỘT/DÒNG (HOẶC ĐÍNH KÈM HÓA ĐƠN CẦN BÓC TÁCH BẢNG):
   - OutputType = 1 (Spreadsheet)
   - Điền dữ liệu vào thuộc tính Spreadsheet:
     + Title: Tiêu đề bảng tính (ví dụ: BẢNG KÊ HÓA ĐƠN ĐẦU VÀO, BẢNG TÍNH CHI PHÍ...)
     + Subtitle: Thông tin người bán, số hóa đơn, ngày lập hoặc kỳ báo cáo.
     + SheetName: Tên sheet (ví dụ: HoaDon, ChiPhi, TonKho).
     + Columns: Danh sách các cột rõ ràng (ví dụ: STT, Mã hàng, Tên hàng hóa dịch vụ, ĐVT, Số lượng, Đơn giá, Thành tiền, Thuế %, Tiền thuế, TK Nợ gợi ý, TK Có gợi ý, Kho gợi ý).
     + Rows: Dữ liệu từng dòng tương ứng với các cột.
     + SummaryRow: Dòng tổng cộng (ví dụ: Cộng tiền hàng, Tổng thuế, Tổng thanh toán).
   - ChatMessage = lời giải thích ngắn gọn, thân thiện bằng tiếng Việt về bảng tính đã tạo và hướng dẫn kế toán kiểm tra.

2. NẾU NGƯỜI DÙNG YÊU CẦU DẠNG ĐOẠN VĂN, BIÊN BẢN GIAO NHẬN, THUYẾT MINH, HỢP ĐỒNG, PARAGRAPH NỐI TIẾP:
   - OutputType = 2 (WordDocument)
   - Điền dữ liệu vào thuộc tính WordDocument:
     + Title: Tiêu đề văn bản (in hoa, căn giữa).
     + Subtitle: Số biên bản, ngày tháng năm.
     + CompanyName: Tên doanh nghiệp / đơn vị phát hành.
     + Sections: Các phần nội dung, mỗi phần gồm Heading, các Paragraphs dạng văn xuôi mạch lạc, và BulletPoints nếu có điều khoản liệt kê.
   - ChatMessage = lời tóm tắt ngắn gọn về văn bản đã soạn thảo.

3. NẾU NGƯỜI DÙNG CHỈ TRAO ĐỔI, GIẢI THÍCH, TƯ VẤN KẾ TOÁN/THUẾ THÔNG THƯỜNG:
   - OutputType = 0 (ChatOnly)
   - ChatMessage = nội dung câu trả lời chi tiết, dễ hiểu.

Hãy trả về JSON cấu trúc OfficeChatResponseDto.";

            var fullPrompt = $"[YÊU CẦU CỦA NGƯỜI DÙNG]:\n{userPrompt.Trim()}";
            if (!string.IsNullOrWhiteSpace(activeDocumentContext))
            {
                fullPrompt += $"\n\n[DỮ LIỆU ĐANG MỞ TRONG OFFICE STUDIO]:\n{activeDocumentContext}";
            }

            if (!string.IsNullOrWhiteSpace(fileDataUri))
            {
                // Có đính kèm ảnh/tài liệu scan -> gửi qua vision multi-modal
                return await _gateway.SendStructuredVisionRequestAsync<OfficeChatResponseDto>(
                    systemPrompt,
                    fullPrompt,
                    new[] { fileDataUri },
                    cancellationToken: cancellationToken);
            }

            return await _gateway.SendStructuredRequestAsync<OfficeChatResponseDto>(
                systemPrompt,
                fullPrompt,
                cancellationToken: cancellationToken);
        }
    }

    public class ItemPostingSuggestionDto
    {
        public int LineNo { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public string DebitAccount { get; set; } = "156";
        public string CreditAccount { get; set; } = "331";
        public string WarehouseCode { get; set; } = "KHO_HH";
        public string Note { get; set; } = string.Empty;
    }
}
