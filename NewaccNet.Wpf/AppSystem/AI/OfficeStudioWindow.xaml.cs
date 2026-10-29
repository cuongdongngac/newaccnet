using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.OleDb;
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
using NewaccNet.TaxInvoice;
using NewaccNet.Wpf.AppSystem;
using NewaccNet.Wpf.AppSystem.TaxInvoice;
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
        private string? _attachedFilePath = null;
        private string? _attachedFileDataUri = null;
        private string? _databasePath = null;
        private TaxInvoiceDbService? _dbService = null;

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

        public OfficeStudioWindow(MemoryStream stream, OfficeDocumentType docType, string title = "", string? databasePath = null) : this()
        {
            _databasePath = databasePath;
            if (!string.IsNullOrEmpty(_databasePath) && File.Exists(_databasePath))
            {
                _dbService = new TaxInvoiceDbService(_databasePath);
            }

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
            if (string.IsNullOrWhiteSpace(prompt))
            {
                if (_attachedFilePath != null)
                {
                    prompt = "Hãy phân tích và bóc tách toàn bộ dữ liệu trong tệp đính kèm này thành bảng tính Excel đầy đủ cột số liệu.";
                }
                else
                {
                    return;
                }
            }

            txtChatInput.Text = string.Empty;
            await SendPromptToAiAsync(prompt);
        }

        private void BtnAttachFile_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                Title = "Chọn tệp Hóa đơn, Chứng từ scan hoặc Bảng tính (Ảnh, PDF, Excel)",
                Filter = "Ảnh và Tài liệu (*.jpg;*.jpeg;*.png;*.pdf;*.xlsx)|*.jpg;*.jpeg;*.png;*.pdf;*.xlsx|Tất cả tệp (*.*)|*.*"
            };

            if (dlg.ShowDialog() == true)
            {
                SetAttachedFile(dlg.FileName);
            }
        }

        private void TxtChatInput_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files != null && files.Length > 0)
                {
                    SetAttachedFile(files[0]);
                }
            }
        }

        private void SetAttachedFile(string filePath)
        {
            if (!File.Exists(filePath)) return;

            _attachedFilePath = filePath;
            var fileInfo = new FileInfo(filePath);
            txtAttachedFileName.Text = fileInfo.Name;
            txtAttachedFileSize.Text = $"{fileInfo.Length / 1024.0:F1} KB";
            pnlAttachedFile.Visibility = Visibility.Visible;

            try
            {
                byte[] bytes = File.ReadAllBytes(filePath);
                string ext = fileInfo.Extension.ToLowerInvariant();
                string mimeType = ext switch
                {
                    ".jpg" or ".jpeg" => "image/jpeg",
                    ".png" => "image/png",
                    ".webp" => "image/webp",
                    ".gif" => "image/gif",
                    ".bmp" => "image/bmp",
                    ".pdf" => "application/pdf",
                    _ => "application/octet-stream"
                };
                _attachedFileDataUri = $"data:{mimeType};base64,{Convert.ToBase64String(bytes)}";
                lblOfficeStatus.Text = $"📎 Đã đính kèm tệp: {fileInfo.Name}. Hãy nhập yêu cầu cho AI (ví dụ: bóc tách thành bảng Excel hoặc văn bản Word).";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể đọc tệp đính kèm: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                ClearAttachedFile();
            }
        }

        private void BtnRemoveAttached_Click(object sender, RoutedEventArgs e)
        {
            ClearAttachedFile();
        }

        private void ClearAttachedFile()
        {
            _attachedFilePath = null;
            _attachedFileDataUri = null;
            pnlAttachedFile.Visibility = Visibility.Collapsed;
            lblOfficeStatus.Text = "Sẵn sàng.";
        }

        private async Task SendPromptToAiAsync(string prompt)
        {
            var config = ConfigManager.Current;
            if (string.IsNullOrWhiteSpace(config.OpenRouterApiKey))
            {
                MessageBox.Show("Vui lòng cấu hình OpenRouter API Key tại Cài đặt AI trước.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string? currentFileUri = _attachedFileDataUri;
            string? currentFileName = _attachedFilePath != null ? Path.GetFileName(_attachedFilePath) : null;

            // Thêm tin nhắn của User vào khung chat
            string displayMessage = !string.IsNullOrWhiteSpace(currentFileName)
                ? $"📎 [Tệp đính kèm: {currentFileName}]\n{prompt}"
                : prompt;

            ChatMessages.Add(new OfficeChatMessageItem
            {
                IsUser = true,
                Message = displayMessage
            });
            ScrollChatToBottom();

            // Clear file attachment in UI after preparing to send
            ClearAttachedFile();

            try
            {
                btnSendChat.IsEnabled = false;
                lblTyping.Visibility = Visibility.Visible;
                lblOfficeStatus.Text = "⏳ AI Copilot đang phân tích yêu cầu và tự động chọn định dạng Excel/Word...";

                var options = new OpenRouterOptions
                {
                    ApiKey = config.OpenRouterApiKey,
                    BaseUrl = config.OpenRouterBaseUrl,
                    Model = config.OpenRouterChatModel,
                    VisionModel = config.OpenRouterVisionModel
                };

                using var gateway = new OpenRouterGateway(options);
                var agent = new AccountingAgentService(gateway);
                var openXmlService = new OpenXmlReportService();

                // Lấy ngữ cảnh văn phòng hiện tại (nếu đang mở tài liệu)
                string activeContext = GetActiveOfficeContext();

                var response = await agent.ProcessOfficeChatAsync(prompt, activeContext, currentFileUri);

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

        private void BtnChipOcrExcel_Click(object sender, RoutedEventArgs e)
        {
            string prompt = _attachedFilePath != null
                ? "Hãy bóc tách hóa đơn/chứng từ đính kèm thành bảng tính Excel chi tiết gồm các cột: STT, Mã hàng gợi ý, Tên hàng hóa dịch vụ, ĐVT, Số lượng, Đơn giá, Thành tiền, Thuế suất %, Tiền thuế, TK Nợ gợi ý (152/156), TK Có gợi ý (331/111), Kho gợi ý."
                : "Lập bảng kê nhập mua hàng hóa vật tư trong kỳ gồm các cột: STT, Mã vật tư, Tên hàng, ĐVT, Số lượng, Đơn giá, Thành tiền, Thuế VAT 10%, TK Nợ 152, TK Có 331, Kho KHO_01.";
            _ = SendPromptToAiAsync(prompt);
        }

        private void BtnChipOcrWord_Click(object sender, RoutedEventArgs e)
        {
            string prompt = _attachedFilePath != null
                ? "Hãy trích xuất nội dung từ tệp đính kèm và định dạng thành biên bản giao nhận hàng hóa kiêm nghiệm thu hoàn chỉnh bằng văn bản Word (gồm căn cứ hợp đồng, các bên giao nhận, bảng liệt kê hoặc các đoạn văn mô tả tình trạng bàn giao)."
                : "Soạn thảo biên bản giao nhận hàng hóa kiêm nghiệm thu vật tư thiết bị hoàn chỉnh (Word Document) giữa bên bán và bên mua, đầy đủ các điều khoản nghiệm thu, bảo hành và đại diện các bên ký kết.";
            _ = SendPromptToAiAsync(prompt);
        }

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

        // ==========================================
        // BRIDGE TO NEWACCNET ACCOUNTING & DATA ENTRY
        // ==========================================

        private void BtnPushToVoucher_Click(object sender, RoutedEventArgs e)
        {
            if (officeTabControl.SelectedIndex != 0)
            {
                MessageBox.Show("Vui lòng chuyển sang tab Bảng tính Excel để lấy số liệu đẩy vào chứng từ.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                var sheet = spreadsheetControl.ActiveWorksheet;
                if (sheet == null) return;

                var invoiceDto = ExtractInvoiceDtoFromSheet(sheet);
                if (invoiceDto.Details == null || invoiceDto.Details.Count == 0)
                {
                    MessageBox.Show("Không tìm thấy dòng dữ liệu mặt hàng/chi phí nào trong bảng tính Excel.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var voucher = new JournalVoucherEntity
                {
                    VoucherNo = "XLS" + DateTime.Now.ToString("yyMMddHHmm"),
                    VoucherDate = DateTime.Today,
                    Contents = !string.IsNullOrWhiteSpace(sheet.Cells[0, 0]?.DisplayText) ? sheet.Cells[0, 0].DisplayText.Trim() : "Nhập mua vật tư hàng hóa từ Office Studio",
                    Bookflag = true
                };

                // 1. Master Payable Entry: Có 331 (hoặc 111) cho toàn bộ tổng tiền
                var payMaster = new JournalEntryEntity
                {
                    Dbcr = -1, // Có
                    AccountId = "331",
                    Amount = (double)invoiceDto.TotalAmount
                };
                voucher.JournalEntries.Add(payMaster);

                // 2. Sub entries: Nợ 152/156 cho từng mặt hàng
                foreach (var item in invoiceDto.Details)
                {
                    var goodsEntry = new JournalEntryEntity
                    {
                        Dbcr = 1, // Nợ
                        AccountId = "156",
                        Amount = (double)item.Amount,
                        ParentEntry = payMaster
                    };
                    payMaster.SubEntries.Add(goodsEntry);
                    voucher.JournalEntries.Add(goodsEntry);
                }

                // 3. Tiền thuế VAT (Nợ 1331) nếu có thuế
                if (invoiceDto.TotalTaxAmount > 0)
                {
                    var taxEntry = new JournalEntryEntity
                    {
                        Dbcr = 1, // Nợ
                        AccountId = "1331",
                        Amount = (double)invoiceDto.TotalTaxAmount,
                        ParentEntry = payMaster
                    };
                    payMaster.SubEntries.Add(taxEntry);
                    voucher.JournalEntries.Add(taxEntry);
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

        public void LoadSpreadsheetFile(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    officeTabControl.SelectedIndex = 0;
                    spreadsheetControl.LoadDocument(filePath);
                    lblOfficeStatus.Text = $"✅ Đã mở bảng tính: {Path.GetFileName(filePath)}";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi mở tệp Excel: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string GetEffectiveDbPath()
        {
            if (!string.IsNullOrEmpty(_databasePath) && File.Exists(_databasePath)) return _databasePath;
            var cur = ConfigManager.Current.AccessFilePath;
            if (!string.IsNullOrEmpty(cur) && File.Exists(cur)) return cur;
            var template = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "template.accdb");
            if (File.Exists(template)) return template;
            return string.Empty;
        }

        private void BtnPushToInventoryItem_Click(object sender, RoutedEventArgs e)
        {
            if (officeTabControl.SelectedIndex != 0)
            {
                MessageBox.Show("Vui lòng chuyển sang tab Bảng tính Excel để nạp dữ liệu vật tư.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var dbPath = GetEffectiveDbPath();
            if (string.IsNullOrEmpty(dbPath) || !File.Exists(dbPath))
            {
                MessageBox.Show("Không tìm thấy đường dẫn CSDL Access hợp lệ để cập nhật danh mục vật tư.", "Lỗi CSDL", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                var sheet = spreadsheetControl.ActiveWorksheet;
                if (sheet == null) return;

                int maxRow = sheet.Rows.LastUsedIndex;
                int maxCol = Math.Min(sheet.Columns.LastUsedIndex, 30);
                if (maxRow < 0 || maxCol < 0)
                {
                    MessageBox.Show("Bảng tính hiện tại không có dữ liệu.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // 1. Tìm dòng tiêu đề cột
                int headerRow = -1;
                int colCode = -1, colName = -1, colUnit = -1, colCat = -1;

                for (int r = 0; r <= Math.Min(maxRow, 10); r++)
                {
                    for (int c = 0; c <= maxCol; c++)
                    {
                        string txt = sheet.Cells[r, c]?.DisplayText?.Trim().ToLowerInvariant() ?? "";
                        if (string.IsNullOrWhiteSpace(txt)) continue;

                        if (txt.Contains("mã") || txt.Contains("materialid") || txt.Contains("tên") || txt.Contains("vật tư") || txt.Contains("hàng hóa"))
                        {
                            headerRow = r;
                            break;
                        }
                    }
                    if (headerRow != -1) break;
                }

                if (headerRow == -1) headerRow = 0;

                for (int c = 0; c <= maxCol; c++)
                {
                    string h = sheet.Cells[headerRow, c]?.DisplayText?.Trim().ToLowerInvariant() ?? "";
                    if (string.IsNullOrWhiteSpace(h)) continue;

                    if (colCode == -1 && (h.Contains("mã vt") || h.Contains("mã vật tư") || h.Contains("mã hàng") || h == "materialid" || h == "mã" || h == "code"))
                        colCode = c;
                    else if (colName == -1 && (h.Contains("tên vt") || h.Contains("tên vật tư") || h.Contains("tên hàng") || h.Contains("tên mặt hàng") || h == "materialname" || h == "tên"))
                        colName = c;
                    else if (colUnit == -1 && (h.Contains("đvt") || h.Contains("đơn vị") || h == "unit" || h == "dvt"))
                        colUnit = c;
                    else if (colCat == -1 && (h.Contains("nhóm") || h.Contains("category") || h == "categoryid" || h == "loại"))
                        colCat = c;
                }

                if (colName == -1 && maxCol >= 1) colName = 1;
                if (colCode == -1 && maxCol >= 0) colCode = 0;

                // 2. Thu thập danh sách vật tư
                var items = new List<(string Id, string Name, string Unit, string Cat)>();
                for (int r = headerRow + 1; r <= maxRow; r++)
                {
                    string name = colName >= 0 ? sheet.Cells[r, colName]?.DisplayText?.Trim() ?? "" : "";
                    string code = colCode >= 0 ? sheet.Cells[r, colCode]?.DisplayText?.Trim() ?? "" : "";
                    string unit = colUnit >= 0 ? sheet.Cells[r, colUnit]?.DisplayText?.Trim() ?? "" : "";
                    string cat = colCat >= 0 ? sheet.Cells[r, colCat]?.DisplayText?.Trim() ?? "" : "";

                    if (string.IsNullOrWhiteSpace(name) && string.IsNullOrWhiteSpace(code)) continue;
                    if (name.StartsWith("Tổng", StringComparison.OrdinalIgnoreCase) || name.StartsWith("Cộng", StringComparison.OrdinalIgnoreCase)) continue;

                    if (string.IsNullOrWhiteSpace(name)) name = code;
                    if (string.IsNullOrWhiteSpace(code)) code = MaterialCodeGenerator.GenerateAcronym(name);

                    if (code.Length > 20) code = code.Substring(0, 20);
                    if (name.Length > 255) name = name.Substring(0, 255);
                    if (unit.Length > 10) unit = unit.Substring(0, 10);
                    if (cat.Length > 10) cat = cat.Substring(0, 10);

                    items.Add((code, name, unit, cat));
                }

                if (items.Count == 0)
                {
                    MessageBox.Show("Không tìm thấy dòng vật tư hợp lệ nào trên bảng tính để nạp vào CSDL.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var confirm = MessageBox.Show(
                    $"Tìm thấy {items.Count} vật tư trên bảng tính.\nBạn có chắc chắn muốn nạp/cập nhật vào CSDL:\n{dbPath}?",
                    "Xác nhận nạp Danh mục Vật tư", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (confirm != MessageBoxResult.Yes) return;

                int inserted = 0;
                int updated = 0;
                var connStr = $"Provider=Microsoft.ACE.OLEDB.16.0;Data Source={dbPath};Persist Security Info=False;";
                using (var conn = new OleDbConnection(connStr))
                {
                    conn.Open();
                    const string checkSql = "SELECT [MaterialId] FROM [InventoryItem] WHERE [MaterialId] = ?";
                    const string insertSql = "INSERT INTO [InventoryItem] ([MaterialId], [MaterialName], [Unit], [Categoryid]) VALUES (?, ?, ?, ?)";
                    const string updateSql = "UPDATE [InventoryItem] SET [MaterialName] = ?, [Unit] = ?, [Categoryid] = ? WHERE [MaterialId] = ?";

                    foreach (var item in items)
                    {
                        bool exists = false;
                        using (var cmd = new OleDbCommand(checkSql, conn))
                        {
                            cmd.Parameters.Add("?", OleDbType.VarWChar, 20).Value = item.Id;
                            exists = cmd.ExecuteScalar() != null;
                        }

                        if (!exists)
                        {
                            using var ins = new OleDbCommand(insertSql, conn);
                            ins.Parameters.Add("?", OleDbType.VarWChar, 20).Value = item.Id;
                            ins.Parameters.Add("?", OleDbType.VarWChar, 255).Value = item.Name;
                            ins.Parameters.Add("?", OleDbType.VarWChar, 10).Value = string.IsNullOrEmpty(item.Unit) ? (object)DBNull.Value : item.Unit;
                            ins.Parameters.Add("?", OleDbType.VarWChar, 10).Value = string.IsNullOrEmpty(item.Cat) ? (object)DBNull.Value : item.Cat;
                            ins.ExecuteNonQuery();
                            inserted++;
                        }
                        else
                        {
                            using var upd = new OleDbCommand(updateSql, conn);
                            upd.Parameters.Add("?", OleDbType.VarWChar, 255).Value = item.Name;
                            upd.Parameters.Add("?", OleDbType.VarWChar, 10).Value = string.IsNullOrEmpty(item.Unit) ? (object)DBNull.Value : item.Unit;
                            upd.Parameters.Add("?", OleDbType.VarWChar, 10).Value = string.IsNullOrEmpty(item.Cat) ? (object)DBNull.Value : item.Cat;
                            upd.Parameters.Add("?", OleDbType.VarWChar, 20).Value = item.Id;
                            upd.ExecuteNonQuery();
                            updated++;
                        }

                        // Ghi nhớ vào Sổ tay ánh xạ (Tier 1)
                        ItemMappingStore.Instance.SaveMapping("", item.Name, item.Unit, item.Id, item.Name);
                    }
                }

                lblOfficeStatus.Text = $"✅ Đã nạp danh mục vật tư: Thêm mới {inserted}, Cập nhật {updated}.";
                MessageBox.Show($"Đã nạp thành công vào CSDL Danh mục Vật tư:\n- Thêm mới: {inserted} mặt hàng\n- Cập nhật: {updated} mặt hàng", "Hoàn tất nạp CSDL", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi nạp Danh mục Vật tư: {ex.Message}", "Lỗi CSDL", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnPushToPartner_Click(object sender, RoutedEventArgs e)
        {
            if (officeTabControl.SelectedIndex != 0)
            {
                MessageBox.Show("Vui lòng chuyển sang tab Bảng tính Excel để nạp dữ liệu đối tác.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var dbPath = GetEffectiveDbPath();
            if (string.IsNullOrEmpty(dbPath) || !File.Exists(dbPath))
            {
                MessageBox.Show("Không tìm thấy đường dẫn CSDL Access hợp lệ để cập nhật danh mục đối tác.", "Lỗi CSDL", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                var sheet = spreadsheetControl.ActiveWorksheet;
                if (sheet == null) return;

                int maxRow = sheet.Rows.LastUsedIndex;
                int maxCol = Math.Min(sheet.Columns.LastUsedIndex, 30);
                if (maxRow < 0 || maxCol < 0)
                {
                    MessageBox.Show("Bảng tính hiện tại không có dữ liệu.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // 1. Tìm dòng tiêu đề cột
                int headerRow = -1;
                int colId = -1, colName = -1, colTax = -1, colAddress = -1, colNote = -1;

                for (int r = 0; r <= Math.Min(maxRow, 10); r++)
                {
                    for (int c = 0; c <= maxCol; c++)
                    {
                        string txt = sheet.Cells[r, c]?.DisplayText?.Trim().ToLowerInvariant() ?? "";
                        if (string.IsNullOrWhiteSpace(txt)) continue;

                        if (txt.Contains("đối tác") || txt.Contains("đối tượng") || txt.Contains("khách hàng") || txt.Contains("nhà cung cấp") || txt.Contains("mst") || txt.Contains("mã số thuế") || txt.Contains("taxno"))
                        {
                            headerRow = r;
                            break;
                        }
                    }
                    if (headerRow != -1) break;
                }

                if (headerRow == -1) headerRow = 0;

                for (int c = 0; c <= maxCol; c++)
                {
                    string h = sheet.Cells[headerRow, c]?.DisplayText?.Trim().ToLowerInvariant() ?? "";
                    if (string.IsNullOrWhiteSpace(h)) continue;

                    if (colId == -1 && (h.Contains("mã đt") || h.Contains("mã đối tác") || h.Contains("mã đối tượng") || h == "partnerid" || h == "id" || h == "mã"))
                        colId = c;
                    else if (colName == -1 && (h.Contains("tên đt") || h.Contains("tên đối tác") || h.Contains("tên đối tượng") || h.Contains("tên công ty") || h.Contains("khách hàng") || h.Contains("nhà cung cấp") || h == "customername" || h == "tên"))
                        colName = c;
                    else if (colTax == -1 && (h.Contains("mst") || h.Contains("mã số thuế") || h.Contains("tax") || h == "taxno"))
                        colTax = c;
                    else if (colAddress == -1 && (h.Contains("địa chỉ") || h.Contains("address")))
                        colAddress = c;
                    else if (colNote == -1 && (h.Contains("ghi chú") || h.Contains("note") || h.Contains("loại") || h.Contains("role")))
                        colNote = c;
                }

                if (colName == -1 && maxCol >= 1) colName = 1;
                if (colId == -1 && maxCol >= 0) colId = 0;

                // 2. Thu thập danh sách đối tác
                var partners = new List<(string Id, string Name, string TaxNo, string Address, string Note)>();
                for (int r = headerRow + 1; r <= maxRow; r++)
                {
                    string name = colName >= 0 ? sheet.Cells[r, colName]?.DisplayText?.Trim() ?? "" : "";
                    string id = colId >= 0 ? sheet.Cells[r, colId]?.DisplayText?.Trim() ?? "" : "";
                    string tax = colTax >= 0 ? sheet.Cells[r, colTax]?.DisplayText?.Trim() ?? "" : "";
                    string addr = colAddress >= 0 ? sheet.Cells[r, colAddress]?.DisplayText?.Trim() ?? "" : "";
                    string note = colNote >= 0 ? sheet.Cells[r, colNote]?.DisplayText?.Trim() ?? "" : "";

                    if (string.IsNullOrWhiteSpace(name) && string.IsNullOrWhiteSpace(id) && string.IsNullOrWhiteSpace(tax)) continue;
                    if (name.StartsWith("Tổng", StringComparison.OrdinalIgnoreCase) || name.StartsWith("Cộng", StringComparison.OrdinalIgnoreCase)) continue;

                    if (string.IsNullOrWhiteSpace(name)) name = !string.IsNullOrWhiteSpace(id) ? id : tax;
                    if (string.IsNullOrWhiteSpace(id))
                    {
                        if (!string.IsNullOrWhiteSpace(tax))
                            id = tax.Length <= 20 ? tax : tax.Substring(0, 20);
                        else
                            id = MaterialCodeGenerator.GenerateAcronym(name);
                    }

                    if (id.Length > 20) id = id.Substring(0, 20);
                    var shortName = name.Length > 50 ? name.Substring(0, 50) : name;
                    if (name.Length > 50 && string.IsNullOrEmpty(note)) note = $"Tên đầy đủ: {name}";
                    if (addr.Length > 255) addr = addr.Substring(0, 255);
                    if (tax.Length > 50) tax = tax.Substring(0, 50);

                    partners.Add((id, shortName, tax, addr, note));
                }

                if (partners.Count == 0)
                {
                    MessageBox.Show("Không tìm thấy dòng đối tác hợp lệ nào trên bảng tính để nạp vào CSDL.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var confirm = MessageBox.Show(
                    $"Tìm thấy {partners.Count} đối tác trên bảng tính.\nBạn có chắc chắn muốn nạp/cập nhật vào CSDL:\n{dbPath}?",
                    "Xác nhận nạp Danh mục Đối tác", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (confirm != MessageBoxResult.Yes) return;

                int inserted = 0;
                int updated = 0;
                var connStr = $"Provider=Microsoft.ACE.OLEDB.16.0;Data Source={dbPath};Persist Security Info=False;";
                using (var conn = new OleDbConnection(connStr))
                {
                    conn.Open();
                    const string checkSql = "SELECT [Id] FROM [Partner] WHERE [Id] = ?";
                    const string insertSql = @"INSERT INTO [Partner] 
                        ([Id], [CustomerName], [Address], [Phonenumber], [Faxnumber], [Note], [Taxno], [Isemployee]) 
                        VALUES (?, ?, ?, '', '', ?, ?, false)";
                    const string updateSql = @"UPDATE [Partner] SET 
                        [CustomerName] = ?, [Address] = ?, [Note] = ?, [Taxno] = ? 
                        WHERE [Id] = ?";

                    foreach (var p in partners)
                    {
                        bool exists = false;
                        using (var cmd = new OleDbCommand(checkSql, conn))
                        {
                            cmd.Parameters.Add("?", OleDbType.VarWChar, 20).Value = p.Id;
                            exists = cmd.ExecuteScalar() != null;
                        }

                        if (!exists)
                        {
                            using var ins = new OleDbCommand(insertSql, conn);
                            ins.Parameters.Add("?", OleDbType.VarWChar, 20).Value = p.Id;
                            ins.Parameters.Add("?", OleDbType.VarWChar, 50).Value = p.Name;
                            ins.Parameters.Add("?", OleDbType.VarWChar, 255).Value = p.Address;
                            ins.Parameters.Add("?", OleDbType.LongVarWChar).Value = p.Note;
                            ins.Parameters.Add("?", OleDbType.VarWChar, 50).Value = p.TaxNo;
                            ins.ExecuteNonQuery();
                            inserted++;
                        }
                        else
                        {
                            using var upd = new OleDbCommand(updateSql, conn);
                            upd.Parameters.Add("?", OleDbType.VarWChar, 50).Value = p.Name;
                            upd.Parameters.Add("?", OleDbType.VarWChar, 255).Value = p.Address;
                            upd.Parameters.Add("?", OleDbType.LongVarWChar).Value = p.Note;
                            upd.Parameters.Add("?", OleDbType.VarWChar, 50).Value = p.TaxNo;
                            upd.Parameters.Add("?", OleDbType.VarWChar, 20).Value = p.Id;
                            upd.ExecuteNonQuery();
                            updated++;
                        }
                    }
                }

                lblOfficeStatus.Text = $"✅ Đã nạp danh mục đối tác: Thêm mới {inserted}, Cập nhật {updated}.";
                MessageBox.Show($"Đã nạp thành công vào CSDL Danh mục Đối tác:\n- Thêm mới: {inserted} đối tác\n- Cập nhật: {updated} đối tác", "Hoàn tất nạp CSDL", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi nạp Danh mục Đối tác: {ex.Message}", "Lỗi CSDL", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void BtnPushToDbInvoice_Click(object sender, RoutedEventArgs e)
        {
            if (officeTabControl.SelectedIndex != 0)
            {
                MessageBox.Show("Vui lòng chuyển sang tab Bảng tính Excel để lưu dữ liệu hóa đơn.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                var sheet = spreadsheetControl.ActiveWorksheet;
                if (sheet == null) return;

                var invoiceDto = ExtractInvoiceDtoFromSheet(sheet);
                if (invoiceDto.Details == null || invoiceDto.Details.Count == 0)
                {
                    MessageBox.Show("Không tìm thấy dòng dữ liệu mặt hàng nào trong bảng tính Excel.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (_dbService == null)
                {
                    var curPath = ConfigManager.Current.AccessFilePath;
                    if (File.Exists(curPath))
                    {
                        _dbService = new TaxInvoiceDbService(curPath);
                    }
                }

                if (_dbService == null)
                {
                    MessageBox.Show("Chưa xác định được Cơ sở dữ liệu kế toán để lưu trữ.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                btnPushToDbInvoice.IsEnabled = false;
                await _dbService.UpsertInvoiceAsync(invoiceDto);
                MessageBox.Show($"Đã lưu thành công Hóa đơn số {invoiceDto.InvoiceNo} ({invoiceDto.Details.Count} dòng hàng) vào CSDL!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                lblOfficeStatus.Text = $"✅ Đã lưu Hóa đơn số {invoiceDto.InvoiceNo} vào CSDL kế toán.";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lưu vào CSDL: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                btnPushToDbInvoice.IsEnabled = true;
            }
        }

        private TaxInvoiceDto ExtractInvoiceDtoFromSheet(Worksheet sheet)
        {
            var invoice = new TaxInvoiceDto
            {
                InvoiceNo = DateTime.Now.Ticks % 10000000,
                InvoiceSeries = "XLS",
                InvoiceDate = DateTime.Today,
                SellerName = "Nhà cung cấp từ Bảng tính",
                Details = new List<TaxInvoiceDetailDto>()
            };

            int maxRow = Math.Min(sheet.Rows.LastUsedIndex, 200);
            int maxCol = Math.Min(sheet.Columns.LastUsedIndex, 25);
            if (maxRow < 0 || maxCol < 0) return invoice;

            // 1. Tìm dòng tiêu đề cột (Header Row)
            int headerRow = -1;
            int colName = -1, colUnit = -1, colQty = -1, colPrice = -1, colAmount = -1, colTaxRate = -1, colTaxAmt = -1;
            int colDebitAcc = -1, colCreditAcc = -1, colWarehouse = -1;

            for (int r = 0; r <= Math.Min(maxRow, 10); r++)
            {
                for (int c = 0; c <= maxCol; c++)
                {
                    string txt = sheet.Cells[r, c]?.DisplayText?.ToLowerInvariant() ?? "";
                    if (string.IsNullOrWhiteSpace(txt)) continue;

                    if (txt.Contains("tên") || txt.Contains("mặt hàng") || txt.Contains("hàng hóa") || txt.Contains("diễn giải"))
                    {
                        headerRow = r;
                        break;
                    }
                }
                if (headerRow != -1) break;
            }

            if (headerRow == -1) headerRow = 0;

            // Xác định vị trí từng cột
            for (int c = 0; c <= maxCol; c++)
            {
                string h = sheet.Cells[headerRow, c]?.DisplayText?.ToLowerInvariant() ?? "";
                if (string.IsNullOrWhiteSpace(h)) continue;

                if (colName == -1 && (h.Contains("tên") || h.Contains("mặt hàng") || h.Contains("hàng hóa") || h.Contains("nội dung") || h.Contains("diễn giải"))) colName = c;
                else if (colUnit == -1 && (h.Contains("đvt") || h.Contains("đơn vị") || h.Contains("dvt"))) colUnit = c;
                else if (colQty == -1 && (h.Contains("số lượng") || h.Contains("sl") || h == "quantity")) colQty = c;
                else if (colPrice == -1 && (h.Contains("đơn giá") || h.Contains("giá") || h == "unitprice" || h == "price")) colPrice = c;
                else if (colAmount == -1 && (h.Contains("thành tiền") || h.Contains("tiền hàng") || h == "amount")) colAmount = c;
                else if (colTaxRate == -1 && (h.Contains("thuế %") || h.Contains("thuế suất") || h.Contains("vat %"))) colTaxRate = c;
                else if (colTaxAmt == -1 && (h.Contains("tiền thuế") || h.Contains("thuế vat") || h.Contains("vat amt"))) colTaxAmt = c;
                else if (colDebitAcc == -1 && (h.Contains("tk nợ") || h.Contains("nợ") || h == "tk no")) colDebitAcc = c;
                else if (colCreditAcc == -1 && (h.Contains("tk có") || h.Contains("có") || h == "tk co")) colCreditAcc = c;
                else if (colWarehouse == -1 && (h.Contains("kho") || h == "warehouse")) colWarehouse = c;
            }

            if (colName == -1) colName = Math.Min(1, maxCol);
            if (colAmount == -1 && maxCol >= 4) colAmount = Math.Min(5, maxCol);

            // 2. Đọc các dòng dữ liệu
            int lineNo = 1;
            for (int r = headerRow + 1; r <= maxRow; r++)
            {
                string name = colName >= 0 ? sheet.Cells[r, colName]?.DisplayText?.Trim() ?? "" : "";
                if (string.IsNullOrWhiteSpace(name) || name.StartsWith("Tổng", StringComparison.OrdinalIgnoreCase) || name.StartsWith("Cộng", StringComparison.OrdinalIgnoreCase))
                    continue;

                string unit = colUnit >= 0 ? sheet.Cells[r, colUnit]?.DisplayText?.Trim() ?? "Cái" : "Cái";
                double qty = 1;
                if (colQty >= 0 && double.TryParse(sheet.Cells[r, colQty]?.DisplayText?.Replace(",", "").Replace(".", "").Trim(), out var q))
                    qty = q;

                decimal price = 0;
                if (colPrice >= 0 && decimal.TryParse(sheet.Cells[r, colPrice]?.DisplayText?.Replace(",", "").Replace(".", "").Trim(), out var p))
                    price = p;

                decimal amount = 0;
                if (colAmount >= 0 && decimal.TryParse(sheet.Cells[r, colAmount]?.DisplayText?.Replace(",", "").Replace(".", "").Trim(), out var a))
                    amount = a;
                else if (price > 0 && qty > 0)
                    amount = price * (decimal)qty;

                decimal taxAmt = 0;
                if (colTaxAmt >= 0 && decimal.TryParse(sheet.Cells[r, colTaxAmt]?.DisplayText?.Replace(",", "").Replace(".", "").Trim(), out var t))
                    taxAmt = t;

                invoice.Details.Add(new TaxInvoiceDetailDto
                {
                    LineNo = lineNo++,
                    ItemName = name,
                    UnitName = unit,
                    Quantity = qty,
                    UnitPrice = price,
                    Amount = amount,
                    TaxAmount = taxAmt
                });
            }

            invoice.TotalAmountWithoutTax = invoice.Details.Sum(i => i.Amount);
            invoice.TotalTaxAmount = invoice.Details.Sum(i => i.TaxAmount);
            invoice.TotalAmount = invoice.TotalAmountWithoutTax + invoice.TotalTaxAmount;

            return invoice;
        }
    }
}
