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
