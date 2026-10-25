using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using DataAccess.EntityClasses;
using DataAccess.HelperClasses;
using SD.LLBLGen.Pro.ORMSupportClasses;
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
    public class DbcrDisplayItem
    {
        public short Value { get; set; }
        public string Display { get; set; } = "";
    }

    public partial class AiAssistantWindow : BaseWindow
    {
        private List<ChartOfAccountEntity> _accounts = new();
        private List<SourceEntity> _voucherSources = new();
        private List<WarehouseEntity> _warehouses = new();

        private JournalEntryAiDto? _currentAiJournal;
        private OcrInvoiceResultDto? _currentOcrInvoice;
        private ExecutiveInsightAiDto? _currentCfoInsight;

        public ObservableCollection<string> DbcrOptions { get; } = new() { "Nợ", "Có" };

        public AiAssistantWindow(int initialTab = 0)
        {
            InitializeComponent();
            this.DataContext = this;
            if (initialTab >= 0 && initialTab < mainTabControl.Items.Count)
            {
                mainTabControl.SelectedIndex = initialTab;
            }
            LoadDictionaries();
        }

        private void LoadDictionaries()
        {
            try
            {
                using var adapter = AppDataAccessAdapter.Create();
                var accCollection = new EntityCollection<ChartOfAccountEntity>();
                adapter.FetchEntityCollection(accCollection, null);
                _accounts = accCollection.OrderBy(x => x.AccountId).ToList();

                var vsCollection = new EntityCollection<SourceEntity>();
                adapter.FetchEntityCollection(vsCollection, null);
                _voucherSources = vsCollection.ToList();

                var whCollection = new EntityCollection<WarehouseEntity>();
                adapter.FetchEntityCollection(whCollection, null);
                _warehouses = whCollection.ToList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("LoadDictionaries error: " + ex.Message);
            }
        }

        private OpenRouterOptions GetAiOptions()
        {
            var config = ConfigManager.Current;
            return new OpenRouterOptions
            {
                ApiKey = config.OpenRouterApiKey ?? string.Empty,
                BaseUrl = string.IsNullOrWhiteSpace(config.OpenRouterBaseUrl) ? "https://openrouter.ai/api/v1" : config.OpenRouterBaseUrl,
                Model = string.IsNullOrWhiteSpace(config.OpenRouterChatModel) ? "google/gemini-2.5-flash" : config.OpenRouterChatModel,
                VisionModel = string.IsNullOrWhiteSpace(config.OpenRouterVisionModel) ? "google/gemini-2.5-flash" : config.OpenRouterVisionModel,
                AnalysisModel = string.IsNullOrWhiteSpace(config.OpenRouterAnalysisModel) ? "google/gemini-2.5-pro" : config.OpenRouterAnalysisModel
            };
        }

        private bool EnsureApiKeyConfigured()
        {
            var options = GetAiOptions();
            if (string.IsNullOrWhiteSpace(options.ApiKey))
            {
                var ask = MessageBox.Show(
                    "Bạn chưa thiết lập OpenRouter API Key.\nBạn có muốn mở cửa sổ Cài đặt AI để nhập API Key ngay bây giờ không?",
                    "Cần cấu hình AI", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (ask == MessageBoxResult.Yes)
                {
                    var dlg = new AiConfigWindow { Owner = this };
                    if (dlg.ShowDialog() == true)
                    {
                        return !string.IsNullOrWhiteSpace(ConfigManager.Current.OpenRouterApiKey);
                    }
                }
                return false;
            }
            return true;
        }

        // ==============================================================
        // TAB 1: TRỢ LÝ NHẬP LIỆU BẰNG LỆNH TỰ NHIÊN
        // ==============================================================

        private void BtnSampleVpp_Click(object sender, RoutedEventArgs e)
        {
            txtNaturalPrompt.Text = "Chi tiền mặt 1.850.000đ mua văn phòng phẩm (giấy in A4 Double A, bút bi Thiên Long) dùng cho phòng Kế toán ngày hôm nay.";
        }

        private void BtnSampleBuyGoods_Click(object sender, RoutedEventArgs e)
        {
            txtNaturalPrompt.Text = "Nhập kho 50 bao xi măng Vicem Hà Tiên PCB40 đơn giá 92.000đ/bao chưa VAT 10% chưa thanh toán cho Công ty TNHH Vật liệu Xây dựng Toàn Cầu MST 0108988776 nhập vào Kho nguyên vật liệu chính.";
        }

        private void BtnSampleCollectCustomer_Click(object sender, RoutedEventArgs e)
        {
            txtNaturalPrompt.Text = "Khách hàng Công ty Cổ phần Thương mại Minh Trí MST 0312456789 thanh toán tiền hàng đợt 1 số tiền 35.000.000đ qua tài khoản ngân hàng Vietcombank.";
        }

        private void BtnSampleAdvance_Click(object sender, RoutedEventArgs e)
        {
            txtNaturalPrompt.Text = "Chi tạm ứng tiền mặt 5.000.000đ cho nhân viên Nguyễn Văn Hùng đi công tác khảo sát thị trường Đà Nẵng theo giấy đề nghị tạm ứng.";
        }

        private async void BtnExecuteAgent_Click(object sender, RoutedEventArgs e)
        {
            string prompt = txtNaturalPrompt.Text?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(prompt))
            {
                MessageBox.Show("Vui lòng nhập nội dung nghiệp vụ cần hạch toán.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!EnsureApiKeyConfigured()) return;

            try
            {
                btnExecuteAgent.IsEnabled = false;
                lblAiStatus.Text = "⏳ Trợ lý AI đang phân tích nghiệp vụ kế toán theo TT200/TT133...";

                var options = GetAiOptions();
                using var gateway = new OpenRouterGateway(options);
                var agent = new AccountingAgentService(gateway);

                var accountPairs = _accounts.Select(a => (a.AccountId, a.AccountName)).ToList();
                var sourcePairs = _voucherSources.Select(s => (s.Srcode, s.Srname)).ToList();

                var result = await agent.ParseNaturalLanguageJournalAsync(prompt, accountPairs, sourcePairs);
                _currentAiJournal = result;

                if (result != null)
                {
                    // Bind UI
                    txtAgentContents.Text = result.Contents;
                    if (DateTime.TryParse(result.VoucherDate, out var dtVoucher))
                    {
                        dtAgentVoucherDate.DateTime = dtVoucher;
                    }
                    else
                    {
                        dtAgentVoucherDate.DateTime = DateTime.Today;
                    }
                    txtAgentPartnerName.Text = result.PersonName;
                    txtAgentSourceCode.Text = result.SuggestedVoucherSourceCode;

                    gridAgentEntries.ItemsSource = result.Entries;
                    lblAiStatus.Text = $"✅ AI đã hoàn tất định khoản ({result.Entries.Count} dòng). Kế toán viên có thể kiểm tra và đẩy vào phiếu.";
                }
            }
            catch (Exception ex)
            {
                lblAiStatus.Text = "❌ Lỗi: " + ex.Message;
                MessageBox.Show("Lỗi phân tích AI: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                btnExecuteAgent.IsEnabled = true;
            }
        }

        private void BtnOpenVoucher_Click(object sender, RoutedEventArgs e)
        {
            if (_currentAiJournal == null || _currentAiJournal.Entries.Count == 0)
            {
                MessageBox.Show("Chưa có dữ liệu định khoản để mở phiếu. Vui lòng bấm Hạch toán trước.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // Tạo đối tượng JournalVoucherEntity trên bộ nhớ RAM (In-Memory Entity)
            var voucher = new JournalVoucherEntity
            {
                VoucherNo = "AI" + DateTime.Now.ToString("yyMMddHHmm"),
                VoucherDate = dtAgentVoucherDate.DateTime != default ? dtAgentVoucherDate.DateTime : DateTime.Today,
                Contents = txtAgentContents.Text?.Trim() ?? _currentAiJournal.Contents,
                Personname = txtAgentPartnerName.Text?.Trim() ?? _currentAiJournal.PartnerName,
                Bookflag = true
            };

            var entries = _currentAiJournal.Entries;
            var debitEntries = entries.Where(x => x.DbcrText.Equals("Nợ", StringComparison.OrdinalIgnoreCase)).ToList();
            var creditEntries = entries.Where(x => x.DbcrText.Equals("Có", StringComparison.OrdinalIgnoreCase)).ToList();

            List<JournalEntryLineAiDto> masters;
            List<JournalEntryLineAiDto> details;

            if (debitEntries.Count <= creditEntries.Count && debitEntries.Count > 0)
            {
                masters = debitEntries;
                details = creditEntries;
            }
            else if (creditEntries.Count > 0)
            {
                masters = creditEntries;
                details = debitEntries;
            }
            else
            {
                masters = entries;
                details = new List<JournalEntryLineAiDto>();
            }

            foreach (var m in masters)
            {
                var masterEntity = new JournalEntryEntity
                {
                    Dbcr = (short)(m.DbcrText.Equals("Nợ", StringComparison.OrdinalIgnoreCase) ? 1 : -1),
                    AccountId = m.AccountId,
                    Amount = (double)m.Amount
                };

                voucher.JournalEntries.Add(masterEntity);

                if (m == masters.First())
                {
                    foreach (var d in details)
                    {
                        var childEntity = new JournalEntryEntity
                        {
                            Dbcr = (short)(d.DbcrText.Equals("Nợ", StringComparison.OrdinalIgnoreCase) ? 1 : -1),
                            AccountId = d.AccountId,
                            Amount = (double)d.Amount,
                            ParentEntry = masterEntity
                        };
                        masterEntity.SubEntries.Add(childEntity);
                        voucher.JournalEntries.Add(childEntity);
                    }
                }
            }

            // Mở cửa sổ VoucherEntryWindow nạp trực tiếp voucher này trên RAM
            var win = VoucherEntryWindow.CreateForAi(voucher);
            win.Owner = this;
            win.ShowDialog();
        }

        // ==============================================================
        // TAB 2: OCR HÓA ĐƠN & QUÉT CHỨNG TỪ (OCR VISION)
        // ==============================================================

        private void BtnBrowseOcr_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                Title = "Chọn tệp Hóa đơn hoặc Chứng từ scan (Ảnh / PDF)",
                Filter = "Tài liệu hóa đơn (*.jpg;*.jpeg;*.png;*.pdf)|*.jpg;*.jpeg;*.png;*.pdf|Tất cả tệp (*.*)|*.*"
            };

            if (dlg.ShowDialog() == true)
            {
                txtOcrFilePath.Text = dlg.FileName;
                LoadThumbnail(dlg.FileName);
            }
        }

        private void LoadThumbnail(string filePath)
        {
            try
            {
                string ext = Path.GetExtension(filePath).ToLowerInvariant();
                if (ext is ".jpg" or ".jpeg" or ".png")
                {
                    var bmp = new BitmapImage();
                    bmp.BeginInit();
                    bmp.UriSource = new Uri(filePath, UriKind.Absolute);
                    bmp.CacheOption = BitmapCacheOption.OnLoad;
                    bmp.EndInit();
                    imgOcrPreview.Source = bmp;
                }
                else
                {
                    imgOcrPreview.Source = null;
                }
            }
            catch
            {
                imgOcrPreview.Source = null;
            }
        }

        private async void BtnRunOcr_Click(object sender, RoutedEventArgs e)
        {
            string filePath = txtOcrFilePath.Text?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
            {
                MessageBox.Show("Vui lòng chọn một tệp hóa đơn hoặc chứng từ hợp lệ trên máy tính.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!EnsureApiKeyConfigured()) return;

            bool isInvoiceMode = cboOcrMode.SelectedIndex == 0;

            try
            {
                btnRunOcr.IsEnabled = false;
                lblAiStatus.Text = "⏳ Đang gửi tài liệu lên OpenRouter Vision để đọc chữ & trích xuất...";

                var options = GetAiOptions();
                using var gateway = new OpenRouterGateway(options);
                var ocrService = new OcrVisionService(gateway);

                if (isInvoiceMode)
                {
                    grpOcrItems.Visibility = Visibility.Visible;
                    grpOcrDocument.Visibility = Visibility.Collapsed;

                    var invoice = await ocrService.ExtractInvoiceFromImageOrPdfAsync(filePath);
                    _currentOcrInvoice = invoice;

                    // Bind header
                    txtOcrInvoiceNumber.Text = invoice.InvoiceNo;
                    if (DateTime.TryParse(invoice.InvoiceDate, out var dt))
                    {
                        dtOcrInvoiceDate.DateTime = dt;
                    }
                    else
                    {
                        dtOcrInvoiceDate.DateTime = DateTime.Today;
                    }

                    txtOcrSerial.Text = $"{invoice.InvoiceSeries} / {invoice.InvoiceType}";
                    txtOcrSellerName.Text = invoice.SellerName;
                    txtOcrSellerTaxCode.Text = invoice.SellerTaxCode;
                    txtOcrSellerAddress.Text = invoice.SellerAddress;
                    txtOcrBuyerName.Text = invoice.BuyerName;
                    txtOcrBuyerTaxCode.Text = invoice.BuyerTaxCode;
                    txtOcrTotalAmount.Text = invoice.TotalAmount.ToString("N0");

                    // Gợi ý tài khoản tự động cho các mặt hàng
                    var agent = new AccountingAgentService(gateway);
                    var accountPairs = _accounts.Select(a => (a.AccountId, a.AccountName)).ToList();
                    var warehousePairs = _warehouses.Select(w => (w.WarehouseId, w.WarehouseName)).ToList();
                    await agent.SuggestInvoiceItemPostingAsync(invoice, accountPairs, warehousePairs);

                    gridOcrItems.ItemsSource = invoice.Items;
                    lblAiStatus.Text = $"✅ Đã trích xuất xong HĐĐT số {invoice.InvoiceNo} ({invoice.Items.Count} mặt hàng).";
                }
                else
                {
                    grpOcrItems.Visibility = Visibility.Collapsed;
                    grpOcrDocument.Visibility = Visibility.Visible;

                    string docText = await ocrService.ExtractDocumentTextAsync(filePath);
                    txtOcrFullDocument.Text = docText;
                    lblAiStatus.Text = "✅ Đã hoàn tất OCR nhận dạng toàn văn văn bản dạng Markdown.";
                }
            }
            catch (Exception ex)
            {
                lblAiStatus.Text = "❌ Lỗi OCR: " + ex.Message;
                MessageBox.Show("Lỗi nhận dạng tài liệu: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                btnRunOcr.IsEnabled = true;
            }
        }

        private void BtnOcrToVoucher_Click(object sender, RoutedEventArgs e)
        {
            if (_currentOcrInvoice == null || _currentOcrInvoice.Items.Count == 0)
            {
                MessageBox.Show("Chưa có thông tin hóa đơn được trích xuất. Vui lòng chạy Quét & Trích xuất trước.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var inv = _currentOcrInvoice;
            var voucher = new JournalVoucherEntity
            {
                VoucherNo = "OCR" + DateTime.Now.ToString("yyMMddHHmm"),
                VoucherDate = dtOcrInvoiceDate.DateTime != default ? dtOcrInvoiceDate.DateTime : DateTime.Today,
                Invoicesnumber = inv.InvoiceNo,
                Contents = $"Mua hàng theo HĐ số {inv.InvoiceNo} từ {inv.SellerName}",
                Personname = inv.SellerName,
                Personaddress = inv.SellerAddress,
                Bookflag = true
            };

            // Master: Có TK 331 (Phải trả NCC) hoặc 1111/1121
            var payMaster = new JournalEntryEntity
            {
                Dbcr = -1, // Có
                AccountId = "331",
                Amount = (double)inv.TotalAmount
            };
            voucher.JournalEntries.Add(payMaster);

            // SubEntries:
            // 1. Tiền hàng (Nợ 152/156/153 theo dòng)
            decimal totalNet = inv.Items.Sum(x => x.Amount);
            string primaryAccount = inv.Items.FirstOrDefault()?.SuggestedAccount ?? "156";

            var goodsEntry = new JournalEntryEntity
            {
                Dbcr = 1, // Nợ
                AccountId = primaryAccount,
                Amount = (double)totalNet,
                ParentEntry = payMaster
            };
            payMaster.SubEntries.Add(goodsEntry);
            voucher.JournalEntries.Add(goodsEntry);

            // 2. Tiền thuế VAT (Nợ 1331) nếu có thuế
            decimal totalTax = inv.Items.Sum(x => x.TaxAmount);
            if (totalTax > 0)
            {
                var taxEntry = new JournalEntryEntity
                {
                    Dbcr = 1, // Nợ
                    AccountId = "1331",
                    Amount = (double)totalTax,
                    ParentEntry = payMaster
                };
                payMaster.SubEntries.Add(taxEntry);
                voucher.JournalEntries.Add(taxEntry);
            }

            // Mở form nhập liệu trên RAM để kế toán chỉnh sửa và bấm Lưu
            var win = VoucherEntryWindow.CreateForAi(voucher);
            win.Owner = this;
            win.ShowDialog();
        }

        private TaxInvoiceDto ConvertOcrToTaxInvoiceDto()
        {
            if (_currentOcrInvoice == null) return new TaxInvoiceDto();

            return new TaxInvoiceDto
            {
                InvoiceNo = long.TryParse(_currentOcrInvoice.InvoiceNo, out var no) ? no : 1,
                InvoiceSeries = _currentOcrInvoice.InvoiceSeries,
                InvoiceDate = dtOcrInvoiceDate.DateTime != default ? dtOcrInvoiceDate.DateTime : DateTime.Today,
                SellerName = _currentOcrInvoice.SellerName,
                SellerTaxCode = _currentOcrInvoice.SellerTaxCode,
                SellerAddress = _currentOcrInvoice.SellerAddress,
                BuyerName = _currentOcrInvoice.BuyerName,
                BuyerTaxCode = _currentOcrInvoice.BuyerTaxCode,
                TotalAmountWithoutTax = _currentOcrInvoice.TotalAmountWithoutTax,
                TotalTaxAmount = _currentOcrInvoice.TotalTaxAmount,
                TotalAmount = _currentOcrInvoice.TotalAmount,
                Details = _currentOcrInvoice.Items.Select(x => new TaxInvoiceDetailDto
                {
                    LineNo = x.LineNo,
                    ItemCode = x.ItemCode,
                    ItemName = x.ItemName,
                    UnitName = x.Unit,
                    Quantity = x.Quantity,
                    UnitPrice = x.UnitPrice,
                    Amount = x.Amount,
                    TaxRate = x.TaxRate,
                    TaxAmount = x.TaxAmount
                }).ToList()
            };
        }

        private void BtnOcrExtractItems_Click(object sender, RoutedEventArgs e)
        {
            if (_currentOcrInvoice == null || _currentOcrInvoice.Items.Count == 0)
            {
                MessageBox.Show("Chưa có mặt hàng nào từ hóa đơn để trích xuất vật tư.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var taxInvoice = ConvertOcrToTaxInvoiceDto();
            var win = new InventoryItemExtractWindow(new List<TaxInvoiceDto> { taxInvoice }, null);
            win.Owner = this;
            win.ShowDialog();
        }

        private void BtnOcrExtractPartner_Click(object sender, RoutedEventArgs e)
        {
            if (_currentOcrInvoice == null || string.IsNullOrWhiteSpace(_currentOcrInvoice.SellerName))
            {
                MessageBox.Show("Chưa có thông tin người bán trên hóa đơn để trích xuất đối tượng.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var taxInvoice = ConvertOcrToTaxInvoiceDto();
            var win = new PartnerExtractWindow(new List<TaxInvoiceDto> { taxInvoice }, null);
            win.Owner = this;
            win.ShowDialog();
        }

        private void BtnOcrExportExcel_Click(object sender, RoutedEventArgs e)
        {
            if (_currentOcrInvoice == null || _currentOcrInvoice.Items.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu mặt hàng để xuất Excel.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var dlg = new SaveFileDialog
            {
                Title = "Lưu Bảng tính Excel Chi tiết Hóa đơn",
                Filter = "Excel Workbook (*.xlsx)|*.xlsx",
                FileName = $"ChiTiet_HD_{_currentOcrInvoice.InvoiceNo}_{DateTime.Now:yyyyMMdd}.xlsx"
            };

            if (dlg.ShowDialog() == true)
            {
                try
                {
                    var openXmlService = new OpenXmlReportService();
                    using var stream = openXmlService.GenerateInvoiceItemsSpreadsheet(
                        $"BẢNG KÊ CHI TIẾT HÓA ĐƠN SỐ {_currentOcrInvoice.InvoiceNo}",
                        _currentOcrInvoice.SellerName,
                        $"MST: {_currentOcrInvoice.SellerTaxCode} - Ngày: {_currentOcrInvoice.InvoiceDate}",
                        _currentOcrInvoice.Items);

                    File.WriteAllBytes(dlg.FileName, stream.ToArray());
                    MessageBox.Show("Xuất file Excel OpenXML thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    Process.Start(new ProcessStartInfo(dlg.FileName) { UseShellExecute = true });
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi xuất Excel: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        // ==============================================================
        // TAB 3: GIÁM ĐỐC TÀI CHÍNH ẢO (BỘ NÃO THỨ HAI - CFO BRAIN)
        // ==============================================================

        private void BtnCfoCashFlow_Click(object sender, RoutedEventArgs e)
        {
            txtCfoQuestion.Text = "Đánh giá chi tiết tình hình lưu chuyển tiền tệ (Cash Flow) và khả năng thanh toán nợ ngắn hạn trong 3 tháng tới.";
        }

        private void BtnCfoProfit_Click(object sender, RoutedEventArgs e)
        {
            txtCfoQuestion.Text = "Phân tích cơ cấu doanh thu và biên lợi nhuận gộp, biên lợi nhuận ròng hiện tại, chỉ ra các chi phí đang biến động bất thường.";
        }

        private void BtnCfoDebt_Click(object sender, RoutedEventArgs e)
        {
            txtCfoQuestion.Text = "Đánh giá rủi ro công nợ phải thu quá hạn, hệ số quay vòng công nợ và kiến nghị chính sách tín dụng thương mại.";
        }

        private void BtnCfoInventory_Click(object sender, RoutedEventArgs e)
        {
            txtCfoQuestion.Text = "Phân tích hiệu quả quản trị hàng tồn kho, tốc độ luân chuyển vốn và các giải pháp giải phóng tồn kho chậm luân chuyển.";
        }

        private async void BtnRunCfo_Click(object sender, RoutedEventArgs e)
        {
            if (!EnsureApiKeyConfigured()) return;

            string question = txtCfoQuestion.Text?.Trim() ?? string.Empty;
            string period = cboCfoPeriod.Text ?? "Năm 2026";

            try
            {
                btnRunCfo.IsEnabled = false;
                lblAiStatus.Text = "⏳ Bộ não CFO ảo đang phân tích dữ liệu tài chính & tính toán các chỉ số kinh doanh...";

                var options = GetAiOptions();
                using var gateway = new OpenRouterGateway(options);
                var agent = new AccountingAgentService(gateway);

                string companyName = "CÔNG TY KẾ TOÁN NEWACCNET";
                string balanceSheetJson = "{\"TongTaiSan\": 25800000000, \"TaiSanNganHan\": 14200000000, \"TienMat\": 3500000000, \"PhaiThu\": 4200000000, \"HangTonKho\": 6500000000, \"NoPhaiTra\": 9800000000, \"VonChuSoHuu\": 16000000000}";
                string incomeStatementJson = "{\"DoanhThuThuan\": 48500000000, \"GiaVonHangBan\": 36200000000, \"LoiNhuanGop\": 12300000000, \"ChiPhiQuanLy\": 4500000000, \"ChiPhiTaiChinh\": 850000000, \"LoiNhuanTruocThue\": 6950000000, \"LoiNhuanSauThue\": 5560000000}";

                var insight = await agent.AnalyzeFinancialDataAsync(companyName, period, balanceSheetJson, incomeStatementJson, question);
                _currentCfoInsight = insight;

                if (insight != null)
                {
                    lblCfoHealth.Text = insight.FinancialHealthStatus.ToUpperInvariant();
                    txtCfoSummary.Text = insight.ExecutiveSummary;
                    txtCfoCashFlow.Text = insight.CashFlowAnalysis;
                    txtCfoRevenue.Text = insight.RevenueProfitAnalysis;
                    txtCfoDebt.Text = insight.DebtRiskAnalysis;

                    gridCfoMetrics.ItemsSource = insight.KeyMetrics;
                    lstCfoAlerts.ItemsSource = insight.KeyAlerts;
                    lstCfoRecommendations.ItemsSource = insight.ActionableRecommendations;

                    lblAiStatus.Text = "✅ Hoàn tất phân tích điều hành CFO. Có thể xuất báo cáo Word/Excel.";
                }
            }
            catch (Exception ex)
            {
                lblAiStatus.Text = "❌ Lỗi phân tích CFO: " + ex.Message;
                MessageBox.Show("Lỗi phân tích tài chính: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                btnRunCfo.IsEnabled = true;
            }
        }

        private void BtnExportWord_Click(object sender, RoutedEventArgs e)
        {
            if (_currentCfoInsight == null)
            {
                MessageBox.Show("Chưa có kết quả phân tích CFO để xuất báo cáo.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var dlg = new SaveFileDialog
            {
                Title = "Lưu Báo Cáo Quản Trị Giám Đốc Tài Chính (Word)",
                Filter = "Word Document (*.docx)|*.docx",
                FileName = $"BaoCao_CFO_{DateTime.Now:yyyyMMdd_HHmm}.docx"
            };

            if (dlg.ShowDialog() == true)
            {
                try
                {
                    var openXmlService = new OpenXmlReportService();
                    using var stream = openXmlService.GenerateExecutiveWordReport(_currentCfoInsight, "CÔNG TY KẾ TOÁN NEWACCNET");
                    File.WriteAllBytes(dlg.FileName, stream.ToArray());
                    MessageBox.Show("Xuất Báo cáo Quản trị Word OpenXML thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    Process.Start(new ProcessStartInfo(dlg.FileName) { UseShellExecute = true });
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi xuất Word: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnExportCfoExcel_Click(object sender, RoutedEventArgs e)
        {
            if (_currentCfoInsight == null || _currentCfoInsight.KeyMetrics.Count == 0)
            {
                MessageBox.Show("Chưa có số liệu chỉ số tài chính để xuất Excel.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var dlg = new SaveFileDialog
            {
                Title = "Lưu Bảng Tính Chỉ Số Tài Chính (Excel)",
                Filter = "Excel Workbook (*.xlsx)|*.xlsx",
                FileName = $"ChiSo_TaiChinh_{DateTime.Now:yyyyMMdd}.xlsx"
            };

            if (dlg.ShowDialog() == true)
            {
                try
                {
                    var openXmlService = new OpenXmlReportService();
                    var dummyItems = _currentCfoInsight.KeyMetrics.Select(m => new OcrInvoiceItemDto
                    {
                        ItemName = m.MetricName,
                        Unit = m.Evaluation,
                        Quantity = 1,
                        UnitPrice = 0,
                        Amount = decimal.TryParse(m.MetricValue.Replace("%", "").Replace(",", "").Trim(), out var val) ? val : 0,
                        TaxRate = m.Benchmark
                    });

                    using var stream = openXmlService.GenerateInvoiceItemsSpreadsheet(
                        "BẢNG CHỈ SỐ TÀI CHÍNH & HIỆU QUẢ HOẠT ĐỘNG",
                        "BAN GIÁM ĐỐC & CFO",
                        $"Kỳ đánh giá: {cboCfoPeriod.Text} - Trạng thái: {_currentCfoInsight.FinancialHealthStatus}",
                        dummyItems);

                    File.WriteAllBytes(dlg.FileName, stream.ToArray());
                    MessageBox.Show("Xuất file Excel OpenXML thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    Process.Start(new ProcessStartInfo(dlg.FileName) { UseShellExecute = true });
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi xuất Excel: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
