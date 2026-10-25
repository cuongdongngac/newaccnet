using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Win32;
using DataAccess.EntityClasses;
using DevExpress.Spreadsheet;
using DevExpress.XtraRichEdit;
using NewaccNet.AI.Configuration;
using NewaccNet.AI.Contracts;
using NewaccNet.AI.Gateway;
using NewaccNet.AI.Services;
using NewaccNet.Wpf.AppSystem;
using NewaccNet.Wpf.AppSystem.Voucher;
using NewaccNet.Wpf.Views.Base;

namespace NewaccNet.Wpf.AppSystem.AI
{
    public enum OfficeDocumentType
    {
        Spreadsheet,
        Word
    }

    public class OfficeChatMessageItem
    {
        public bool IsUser { get; set; }
        public string SenderName => IsUser ? "Bạn" : "AI Copilot";
        public string Message { get; set; } = string.Empty;
        public string TimeText { get; set; } = DateTime.Now.ToString("HH:mm");
        public string? BadgeText { get; set; }

        public Visibility BadgeVisibility => string.IsNullOrWhiteSpace(BadgeText) ? Visibility.Collapsed : Visibility.Visible;

        public Brush BackgroundBrush => IsUser 
            ? new SolidColorBrush(Color.FromRgb(224, 242, 254)) // Light Blue
            : new SolidColorBrush(Color.FromRgb(248, 250, 252)); // Light Slate

        public Brush BadgeColor => BadgeText?.Contains("Excel") == true 
            ? new SolidColorBrush(Color.FromRgb(22, 163, 74)) // Green
            : new SolidColorBrush(Color.FromRgb(37, 99, 235)); // Blue

        public HorizontalAlignment Alignment => IsUser ? HorizontalAlignment.Right : HorizontalAlignment.Left;
    }

    public partial class OfficeStudioWindow : BaseWindow
    {
        public ObservableCollection<OfficeChatMessageItem> ChatMessages { get; } = new();

        public OfficeStudioWindow()
        {
            InitializeComponent();
            officeTabControl.SelectedIndex = 0;
            lstChatMessages.ItemsSource = ChatMessages;

            // Welcome message
            ChatMessages.Add(new OfficeChatMessageItem
            {
                IsUser = false,
                Message = "Xin chào! Em là trợ lý AI tích hợp trong Office Studio. Anh/chị có thể yêu cầu em lập bảng tính Excel (có cột, số liệu, tính tổng) hoặc soạn thảo văn bản Word (thuyết minh BCTC, biên bản đối chiếu, tờ trình). Tùy câu hỏi, em sẽ tự động tạo bảng Excel hoặc tài liệu Word ở khung bên phải."
            });

            lblOfficeStatus.Text = "Sẵn sàng làm việc với Excel và Word tích hợp AI.";
        }

        public OfficeStudioWindow(MemoryStream stream, OfficeDocumentType docType, string title = "") : this()
        {
            if (!string.IsNullOrWhiteSpace(title))
            {
                this.Title = $"Office Studio & AI - {title}";
            }

            if (docType == OfficeDocumentType.Spreadsheet)
            {
                officeTabControl.SelectedIndex = 0;
                LoadSpreadsheetStream(stream);
            }
            else
            {
                officeTabControl.SelectedIndex = 1;
                LoadWordStream(stream);
            }
        }

        public void LoadSpreadsheetStream(MemoryStream stream)
        {
            try
            {
                stream.Seek(0, SeekOrigin.Begin);
                spreadsheetControl.LoadDocument(stream, DevExpress.Spreadsheet.DocumentFormat.Xlsx);
                lblOfficeStatus.Text = "✅ Đã nạp bảng tính Excel OpenXML từ bộ nhớ RAM (Zero-Disk I/O).";
            }
            catch (Exception ex)
            {
                lblOfficeStatus.Text = "❌ Lỗi nạp bảng tính: " + ex.Message;
                MessageBox.Show("Lỗi nạp bảng tính: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void LoadWordStream(MemoryStream stream)
        {
            try
            {
                stream.Seek(0, SeekOrigin.Begin);
                richEditControl.LoadDocument(stream, DevExpress.XtraRichEdit.DocumentFormat.OpenXml);
                lblOfficeStatus.Text = "✅ Đã nạp văn bản Word OpenXML từ bộ nhớ RAM (Zero-Disk I/O).";
            }
            catch (Exception ex)
            {
                lblOfficeStatus.Text = "❌ Lỗi nạp văn bản: " + ex.Message;
                MessageBox.Show("Lỗi nạp văn bản: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public static OfficeStudioWindow OpenExcel(MemoryStream stream, string title = "Bảng tính Excel")
        {
            var win = new OfficeStudioWindow(stream, OfficeDocumentType.Spreadsheet, title);
            win.Show();
            return win;
        }

        public static OfficeStudioWindow OpenWord(MemoryStream stream, string title = "Văn bản Word")
        {
            var win = new OfficeStudioWindow(stream, OfficeDocumentType.Word, title);
            win.Show();
            return win;
        }

        private void BtnToggleChat_Click(object sender, RoutedEventArgs e)
        {
            if (colChatPane.Width.Value > 0)
            {
                colChatPane.Width = new GridLength(0);
            }
            else
            {
                colChatPane.Width = new GridLength(380);
            }
        }

        private void BtnClearChat_Click(object sender, RoutedEventArgs e)
        {
            ChatMessages.Clear();
            ChatMessages.Add(new OfficeChatMessageItem
            {
                IsUser = false,
                Message = "Đoạn chat đã được làm mới. Anh/chị cần em hỗ trợ bảng tính hay văn bản nào tiếp theo ạ?"
            });
        }

        private void TxtChatInput_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                BtnSendChat_Click(sender, e);
                e.Handled = true;
            }
        }

        private async void BtnSendChat_Click(object sender, RoutedEventArgs e)
        {
            string prompt = txtChatInput.Text?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(prompt)) return;

            txtChatInput.Text = string.Empty;
            await SendPromptToAiAsync(prompt);
        }

        private async Task SendPromptToAiAsync(string prompt)
        {
            var config = ConfigManager.Current;
            if (string.IsNullOrWhiteSpace(config.OpenRouterApiKey))
            {
                MessageBox.Show("Vui lòng cấu hình OpenRouter API Key tại Cài đặt AI trước.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Thêm tin nhắn của User vào khung chat
            ChatMessages.Add(new OfficeChatMessageItem
            {
                IsUser = true,
                Message = prompt
            });
            ScrollChatToBottom();

            try
            {
                btnSendChat.IsEnabled = false;
                lblTyping.Visibility = Visibility.Visible;
                lblOfficeStatus.Text = "⏳ AI Copilot đang phân tích yêu cầu và tự động chọn định dạng Excel/Word...";

                var options = new OpenRouterOptions
                {
                    ApiKey = config.OpenRouterApiKey,
                    BaseUrl = config.OpenRouterBaseUrl,
                    Model = config.OpenRouterChatModel
                };

                using var gateway = new OpenRouterGateway(options);
                var agent = new AccountingAgentService(gateway);
                var openXmlService = new OpenXmlReportService();

                // Lấy ngữ cảnh văn phòng hiện tại (nếu đang mở tài liệu)
                string activeContext = GetActiveOfficeContext();

                var response = await agent.ProcessOfficeChatAsync(prompt, activeContext);

                if (response != null)
                {
                    string badge = null;

                    if (response.OutputType == OfficeOutputType.Spreadsheet && response.Spreadsheet != null)
                    {
                        // Sinh file Excel OpenXML và nạp vào SpreadsheetControl
                        badge = "📊 Đã tạo Bảng tính Excel";
                        officeTabControl.SelectedIndex = 0;

                        using var stream = openXmlService.GenerateDynamicSpreadsheetStream(response.Spreadsheet);
                        LoadSpreadsheetStream(stream);
                    }
                    else if (response.OutputType == OfficeOutputType.WordDocument && response.WordDocument != null)
                    {
                        // Sinh file Word OpenXML và nạp vào RichEditControl
                        badge = "📄 Đã tạo Văn bản Word";
                        officeTabControl.SelectedIndex = 1;

                        using var stream = openXmlService.GenerateDynamicWordStream(response.WordDocument);
                        LoadWordStream(stream);
                    }

                    // Thêm phản hồi của AI vào Chat
                    ChatMessages.Add(new OfficeChatMessageItem
                    {
                        IsUser = false,
                        Message = string.IsNullOrWhiteSpace(response.ChatMessage) ? "Đã xử lý xong yêu cầu của anh/chị." : response.ChatMessage,
                        BadgeText = badge
                    });
                }
                else
                {
                    ChatMessages.Add(new OfficeChatMessageItem
                    {
                        IsUser = false,
                        Message = "Xin lỗi, AI chưa thể xử lý yêu cầu lúc này. Vui lòng thử lại câu hỏi khác."
                    });
                }

                lblOfficeStatus.Text = "✅ Hoàn tất xử lý yêu cầu.";
            }
            catch (Exception ex)
            {
                lblOfficeStatus.Text = "❌ Lỗi: " + ex.Message;
                ChatMessages.Add(new OfficeChatMessageItem
                {
                    IsUser = false,
                    Message = "Lỗi xử lý AI: " + ex.Message
                });
            }
            finally
            {
                btnSendChat.IsEnabled = true;
                lblTyping.Visibility = Visibility.Collapsed;
                ScrollChatToBottom();
            }
        }

        private string GetActiveOfficeContext()
        {
            try
            {
                if (officeTabControl.SelectedIndex == 0 && spreadsheetControl.ActiveWorksheet != null)
                {
                    var sheet = spreadsheetControl.ActiveWorksheet;
                    string context = "BẢNG TÍNH EXCEL HIỆN TẠI:\n";
                    for (int r = 0; r < Math.Min(15, sheet.Rows.LastUsedIndex + 1); r++)
                    {
                        var rowVals = Enumerable.Range(0, Math.Min(8, sheet.Columns.LastUsedIndex + 1))
                            .Select(c => sheet.Cells[r, c]?.DisplayText ?? "");
                        context += string.Join(" | ", rowVals) + "\n";
                    }
                    return context;
                }
                else if (officeTabControl.SelectedIndex == 1 && !string.IsNullOrWhiteSpace(richEditControl.Text))
                {
                    return "VĂN BẢN WORD HIỆN TẠI:\n" + richEditControl.Text.Substring(0, Math.Min(1000, richEditControl.Text.Length));
                }
            }
            catch { }
            return string.Empty;
        }

        private void ScrollChatToBottom()
        {
            if (ChatMessages.Count > 0)
            {
                lstChatMessages.ScrollIntoView(ChatMessages.Last());
            }
        }

        // ==========================================
        // QUICK PROMPT CHIPS
        // ==========================================

        private void BtnChipExpense_Click(object sender, RoutedEventArgs e)
        {
            _ = SendPromptToAiAsync("Lập bảng kê chi phí quý 1 năm 2026 chi tiết theo từng yếu tố (NVL, nhân công, chi phí chung, khấu hao, dịch vụ mua ngoài) có cột Kế hoạch, Thực hiện, Chênh lệch và Tỷ lệ hoàn thành %");
        }

        private void BtnChipDebt_Click(object sender, RoutedEventArgs e)
        {
            _ = SendPromptToAiAsync("Lập bảng theo dõi tuổi nợ nhà cung cấp quá hạn gồm các cột: STT, Mã NCC, Tên Nhà cung cấp, MST, Tổng dư nợ, Quá hạn dưới 30 ngày, 30-60 ngày, Trên 90 ngày và Đánh giá rủi ro");
        }

        private void BtnChipReconcile_Click(object sender, RoutedEventArgs e)
        {
            _ = SendPromptToAiAsync("Soạn thảo biên bản đối chiếu và xác nhận công nợ cuối năm tài chính 2026 giữa Công ty Kế toán NewaccNet và Công ty Cổ phần Thương mại Minh Trí MST 0312456789 số tiền còn nợ 35.000.000đ");
        }

        private void BtnChipNotes_Click(object sender, RoutedEventArgs e)
        {
            _ = SendPromptToAiAsync("Viết bản Thuyết minh Báo cáo Tài chính cho phần Hàng tồn kho năm 2026, giải trình biến động tăng giảm nguyên vật liệu, thành phẩm và căn cứ trích lập dự phòng giảm giá hàng tồn kho");
        }

        // ==========================================
        // STANDARD OFFICE BUTTONS
        // ==========================================

        private void BtnNewExcel_Click(object sender, RoutedEventArgs e)
        {
            if (officeTabControl.SelectedIndex == 0)
            {
                spreadsheetControl.CreateNewDocument();
                lblOfficeStatus.Text = "Đã khởi tạo bảng tính Excel mới.";
            }
            else
            {
                richEditControl.CreateNewDocument();
                lblOfficeStatus.Text = "Đã khởi tạo văn bản Word mới.";
            }
        }

        private void BtnOpenOfficeFile_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                Title = "Mở tệp Văn phòng",
                Filter = "Tất cả tệp văn phòng (*.xlsx;*.xls;*.csv;*.docx;*.rtf;*.txt)|*.xlsx;*.xls;*.csv;*.docx;*.rtf;*.txt|Excel (*.xlsx;*.xls;*.csv)|*.xlsx;*.xls;*.csv|Word (*.docx;*.rtf;*.txt)|*.docx;*.rtf;*.txt"
            };

            if (dlg.ShowDialog() == true)
            {
                string ext = Path.GetExtension(dlg.FileName).ToLowerInvariant();
                try
                {
                    if (ext is ".xlsx" or ".xls" or ".csv")
                    {
                        officeTabControl.SelectedIndex = 0;
                        spreadsheetControl.LoadDocument(dlg.FileName);
                        lblOfficeStatus.Text = $"✅ Đã mở bảng tính: {Path.GetFileName(dlg.FileName)}";
                    }
                    else
                    {
                        officeTabControl.SelectedIndex = 1;
                        richEditControl.LoadDocument(dlg.FileName);
                        lblOfficeStatus.Text = $"✅ Đã mở văn bản: {Path.GetFileName(dlg.FileName)}";
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi mở tệp: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnSaveOfficeFile_Click(object sender, RoutedEventArgs e)
        {
            if (officeTabControl.SelectedIndex == 0)
            {
                var dlg = new SaveFileDialog
                {
                    Title = "Lưu Bảng tính Excel",
                    Filter = "Excel Workbook (*.xlsx)|*.xlsx|CSV (*.csv)|*.csv|PDF (*.pdf)|*.pdf",
                    FileName = $"BangTinh_{DateTime.Now:yyyyMMdd_HHmm}.xlsx"
                };

                if (dlg.ShowDialog() == true)
                {
                    spreadsheetControl.SaveDocument(dlg.FileName);
                    lblOfficeStatus.Text = $"✅ Đã lưu bảng tính: {dlg.FileName}";
                    MessageBox.Show("Lưu bảng tính thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                var dlg = new SaveFileDialog
                {
                    Title = "Lưu Văn bản Word",
                    Filter = "Word Document (*.docx)|*.docx|PDF (*.pdf)|*.pdf|Rich Text (*.rtf)|*.rtf",
                    FileName = $"VanBan_{DateTime.Now:yyyyMMdd_HHmm}.docx"
                };

                if (dlg.ShowDialog() == true)
                {
                    richEditControl.SaveDocument(dlg.FileName, DevExpress.XtraRichEdit.DocumentFormat.OpenXml);
                    lblOfficeStatus.Text = $"✅ Đã lưu văn bản: {dlg.FileName}";
                    MessageBox.Show("Lưu văn bản thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void BtnPushToVoucher_Click(object sender, RoutedEventArgs e)
        {
            if (officeTabControl.SelectedIndex != 0)
            {
                MessageBox.Show("Vui lòng chuyển sang tab Bảng tính Excel để chọn dòng dữ liệu đẩy vào chứng từ.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                var sheet = spreadsheetControl.ActiveWorksheet;
                if (sheet == null) return;

                var selectedRange = spreadsheetControl.Selection;
                int startRow = selectedRange.TopRowIndex;
                int endRow = selectedRange.BottomRowIndex;

                var voucher = new JournalVoucherEntity
                {
                    VoucherNo = "XLS" + DateTime.Now.ToString("yyMMddHHmm"),
                    VoucherDate = DateTime.Today,
                    Contents = "Nhập liệu từ Bảng tính Excel Office Studio",
                    Bookflag = true
                };

                for (int r = startRow; r <= endRow; r++)
                {
                    string cellVal0 = sheet.Cells[r, 0]?.DisplayText?.Trim() ?? "";
                    string cellVal1 = sheet.Cells[r, 1]?.DisplayText?.Trim() ?? "";
                    string cellVal2 = sheet.Cells[r, 2]?.DisplayText?.Trim() ?? "";

                    short dbcr = cellVal0.Equals("Có", StringComparison.OrdinalIgnoreCase) || cellVal0 == "-1" ? (short)-1 : (short)1;
                    string accountId = !string.IsNullOrWhiteSpace(cellVal1) ? cellVal1 : "156";
                    double amount = 0;
                    if (double.TryParse(cellVal2.Replace(",", "").Replace(".", "").Trim(), out var parsedAmt))
                    {
                        amount = parsedAmt;
                    }

                    var entry = new JournalEntryEntity
                    {
                        Dbcr = dbcr,
                        AccountId = accountId,
                        Amount = amount
                    };
                    voucher.JournalEntries.Add(entry);
                }

                if (voucher.JournalEntries.Count == 0)
                {
                    MessageBox.Show("Không có dòng dữ liệu hợp lệ trong vùng chọn.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var win = VoucherEntryWindow.CreateForAi(voucher);
                win.Owner = this;
                win.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tạo chứng từ từ Excel: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
