using System;
using System.IO;
using System.Linq;
using System.Windows;
using Microsoft.Win32;
using DataAccess.EntityClasses;
using DevExpress.Spreadsheet;
using DevExpress.XtraRichEdit;
using NewaccNet.AI.Configuration;
using NewaccNet.AI.Gateway;
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

    public partial class OfficeStudioWindow : BaseWindow
    {
        public OfficeStudioWindow()
        {
            InitializeComponent();
            officeTabControl.SelectedIndex = 0;
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

                // Đọc các dòng được chọn trên sheet
                for (int r = startRow; r <= endRow; r++)
                {
                    // Lấy cell Nợ/Có, Tài khoản, Tiền, Diễn giải
                    string cellVal0 = sheet.Cells[r, 0]?.DisplayText?.Trim() ?? "";
                    string cellVal1 = sheet.Cells[r, 1]?.DisplayText?.Trim() ?? "";
                    string cellVal2 = sheet.Cells[r, 2]?.DisplayText?.Trim() ?? "";
                    string cellVal3 = sheet.Cells[r, 3]?.DisplayText?.Trim() ?? "";

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

                // Mở VoucherEntryWindow trên RAM
                var win = VoucherEntryWindow.CreateForAi(voucher);
                win.Owner = this;
                win.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tạo chứng từ từ Excel: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void BtnAiOfficeAsk_Click(object sender, RoutedEventArgs e)
        {
            string prompt = txtAiOfficePrompt.Text?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(prompt))
            {
                MessageBox.Show("Vui lòng nhập câu hỏi hoặc yêu cầu cho AI (ví dụ: 'Phân tích tổng hợp số liệu vùng chọn' hoặc 'Viết thuyết minh cho bảng tính này').",
                    "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var config = ConfigManager.Current;
            if (string.IsNullOrWhiteSpace(config.OpenRouterApiKey))
            {
                MessageBox.Show("Vui lòng cấu hình OpenRouter API Key tại Cài đặt AI trước.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                btnAiOfficeAsk.IsEnabled = false;
                lblOfficeStatus.Text = "⏳ AI Copilot đang xử lý nội dung văn phòng...";

                var options = new OpenRouterOptions
                {
                    ApiKey = config.OpenRouterApiKey,
                    BaseUrl = config.OpenRouterBaseUrl,
                    Model = config.OpenRouterChatModel
                };

                using var gateway = new OpenRouterGateway(options);

                if (officeTabControl.SelectedIndex == 0)
                {
                    // Lấy dữ liệu 20 dòng đầu của Active Sheet để làm ngữ cảnh
                    var sheet = spreadsheetControl.ActiveWorksheet;
                    string context = "";
                    if (sheet != null)
                    {
                        for (int r = 0; r < Math.Min(30, sheet.Rows.LastUsedIndex + 1); r++)
                        {
                            var rowVals = Enumerable.Range(0, Math.Min(10, sheet.Columns.LastUsedIndex + 1))
                                .Select(c => sheet.Cells[r, c]?.DisplayText ?? "");
                            context += string.Join(" | ", rowVals) + "\n";
                        }
                    }

                    string fullPrompt = $"Bạn là chuyên gia tài chính và Excel cấp cao. Dưới đây là dữ liệu bảng tính hiện tại:\n{context}\n\n[YÊU CẦU CỦA NGƯỜI DÙNG]:\n{prompt}";
                    var resp = await gateway.SendChatAsync(fullPrompt);

                    lblOfficeStatus.Text = "✅ AI đã hoàn tất phản hồi.";
                    MessageBox.Show(resp.Content, "🤖 AI Trợ lý Bảng tính Phản hồi", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    // Lấy text của tài liệu Word
                    string selectedText = richEditControl.Document.Selection != null
                        ? richEditControl.Document.GetText(richEditControl.Document.Selection)
                        : "";

                    string docText = !string.IsNullOrWhiteSpace(selectedText)
                        ? selectedText
                        : richEditControl.Text;

                    string fullPrompt = $"Bạn là trợ lý soạn thảo tài chính và pháp lý doanh nghiệp. Dưới đây là nội dung văn bản:\n{docText}\n\n[YÊU CẦU CỦA NGƯỜI DÙNG]:\n{prompt}\n\nHãy viết nội dung phản hồi sạch để có thể dán vào tài liệu Word.";
                    var resp = await gateway.SendChatAsync(fullPrompt);

                    richEditControl.Document.AppendText("\n\n" + resp.Content);
                    lblOfficeStatus.Text = "✅ AI đã chèn nội dung vào cuối tài liệu Word.";
                }
            }
            catch (Exception ex)
            {
                lblOfficeStatus.Text = "❌ Lỗi AI: " + ex.Message;
                MessageBox.Show("Lỗi AI: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                btnAiOfficeAsk.IsEnabled = true;
            }
        }
    }
}
