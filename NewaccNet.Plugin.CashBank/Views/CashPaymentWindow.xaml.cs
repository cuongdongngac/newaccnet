using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using DevExpress.Xpf.Core;
using DataAccess.EntityClasses;
using NewaccNet.Core.Data;
using NewaccNet.Core.Helpers;
using NewaccNet.Core.Posting;
using NewaccNet.Core.UI;
using NewaccNet.Plugin.CashBank.Services;
using NewaccNet.Reports.Vouchers;

namespace NewaccNet.Plugin.CashBank.Views
{
    public partial class CashPaymentWindow : ThemedWindow
    {
        private List<PluginAccountConfigItemDto> _cashAccounts = new();
        private List<CashAliasItemDto> _paymentAliases = new();
        private List<PartnerEntity> _partners = new();
        private List<AccountOption> _allAccounts = new();
        private Dictionary<string, AccountOption> _accountMap = new(StringComparer.OrdinalIgnoreCase);

        private List<int> _voucherIds = new();
        private int _currentVoucherIndex = -1;
        private int _currentVoucherId = 0;
        private bool _isCurrentVoucherPosted = false;

        private List<DebtDetailEntity>? _manualDebtDetails = null;

        public CashPaymentWindow()
        {
            InitializeComponent();
            InitData();
            RefreshVoucherList();
            RefreshNavigation();
            SetNewVoucherMode();
        }

        private void InitData()
        {
            try
            {
                var cfg = CashVoucherDataService.GetPostingConfig();
                _paymentAliases = cfg.PaymentAliases?.Where(x => x.IsActive).ToList() ?? new List<CashAliasItemDto>();
                cboAlias.ItemsSource = _paymentAliases;

                _cashAccounts = CashVoucherDataService.LoadCashAccounts();
                cboCashAccount.ItemsSource = _cashAccounts;

                _partners = CashVoucherDataService.LoadPartners();
                luePartner.ItemsSource = _partners;

                _allAccounts = CashVoucherDataService.LoadAccounts();
                _accountMap = _allAccounts.ToDictionary(x => x.AccountId, StringComparer.OrdinalIgnoreCase);
                lueOffsetAccount.ItemsSource = _allAccounts;

                FilterCashAccountsByMethod("PC");
            }
            catch (Exception ex)
            {
                ThemedMessageBox.Show("Lỗi nạp dữ liệu", ex.Message, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void FilterCashAccountsByMethod(string vType)
        {
            bool isBank = (vType == "BN" || vType == "UNC");
            var filtered = _cashAccounts.Where(x => isBank ? (x.Category == "Bank" || x.AccountId.StartsWith("112"))
                                                           : (x.Category == "Cash" || x.AccountId.StartsWith("111"))).ToList();
            cboCashAccount.ItemsSource = filtered;
            if (filtered.Count > 0 && (cboCashAccount.SelectedItem == null || !filtered.Contains((PluginAccountConfigItemDto)cboCashAccount.SelectedItem)))
            {
                cboCashAccount.SelectedItem = filtered[0];
            }

            grpBankDetails.Visibility = isBank ? Visibility.Visible : Visibility.Collapsed;
        }

        private void SetNewVoucherMode()
        {
            _currentVoucherId = 0;
            _currentVoucherIndex = -1;
            _isCurrentVoucherPosted = false;
            _manualDebtDetails = null;

            dpVoucherDate.DateTime = DateTime.Today;
            string prefix = GetCurrentVoucherType();
            txtVoucherNo.Text = CashVoucherDataService.GenerateNextVoucherNo(prefix);
            chkBookflag.IsChecked = false; // ERP rule: mặc định chưa ghi sổ

            txtAmount.EditValue = 0m;
            lblAmountWords.Text = "Không đồng";
            txtDescription.Text = "";
            txtPersonName.Text = "";
            txtPersonAddress.Text = "";
            txtInvoicesNumber.Text = "";
            txtPaymentAcc.Text = "";
            txtPaymentBank.Text = "";
            txtRecievedCorp.Text = "";
            txtRecievedAcc.Text = "";
            txtRecievedBank.Text = "";
            luePartner.EditValue = null;

            if (_paymentAliases.Count > 0)
            {
                cboAlias.SelectedItem = _paymentAliases[0];
            }

            lblNavPosition.Content = "Phiếu mới (chưa lưu)";
            UpdateDebtStatus();
        }

        private string GetCurrentVoucherType()
        {
            var item = cboMethod.SelectedItem as DevExpress.Xpf.Editors.ComboBoxEditItem;
            return item?.Tag?.ToString() ?? "PC";
        }

        private void CboMethod_EditValueChanged(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e)
        {
            string prefix = GetCurrentVoucherType();
            FilterCashAccountsByMethod(prefix);
            if (_currentVoucherId == 0)
            {
                txtVoucherNo.Text = CashVoucherDataService.GenerateNextVoucherNo(prefix);
            }
            RefreshVoucherList();
            UpdateDebtStatus();
        }

        private void CboAlias_EditValueChanged(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e)
        {
            if (cboAlias.SelectedItem is CashAliasItemDto alias)
            {
                if (!string.IsNullOrEmpty(alias.OffsetAccount))
                {
                    lueOffsetAccount.EditValue = alias.OffsetAccount;
                }
                if (string.IsNullOrWhiteSpace(txtDescription.Text))
                {
                    txtDescription.Text = !string.IsNullOrWhiteSpace(alias.DefaultDescription) ? alias.DefaultDescription : alias.Name;
                }
            }
            UpdateDebtStatus();
        }

        private void CboCashAccount_EditValueChanged(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e)
        {
            UpdateDebtStatus();
        }

        private void LueOffsetAccount_EditValueChanged(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e)
        {
            _manualDebtDetails = null;
            UpdateDebtStatus();
        }

        private void LuePartner_EditValueChanged(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e)
        {
            if (luePartner.SelectedItem is PartnerEntity p)
            {
                if (string.IsNullOrWhiteSpace(txtPersonName.Text)) txtPersonName.Text = p.CustomerName ?? "";
                if (string.IsNullOrWhiteSpace(txtPersonAddress.Text)) txtPersonAddress.Text = p.Address ?? "";
                if (string.IsNullOrWhiteSpace(txtRecievedCorp.Text)) txtRecievedCorp.Text = p.CustomerName ?? "";
            }
            UpdateDebtStatus();
        }

        private void TxtAmount_EditValueChanged(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e)
        {
            decimal amt = Convert.ToDecimal(txtAmount.EditValue ?? 0m);
            lblAmountWords.Text = NumberToWordsConverter.ConvertToWords(amt);
            UpdateDebtStatus();
        }

        private bool IsDebtAccount(string? accountId)
        {
            if (string.IsNullOrWhiteSpace(accountId)) return false;
            return accountId.StartsWith("331") || accountId.StartsWith("131")
                || accountId.StartsWith("141") || accountId.StartsWith("338")
                || accountId.StartsWith("138") || accountId.StartsWith("334")
                || accountId.StartsWith("341");
        }

        private void UpdateDebtStatus()
        {
            string cashAcc = (cboCashAccount.SelectedItem as PluginAccountConfigItemDto)?.AccountId ?? "1111";
            string offsetAcc = lueOffsetAccount.EditValue?.ToString() ?? "";
            decimal amt = Convert.ToDecimal(txtAmount.EditValue ?? 0m);

            lblPostingSummary.Text = $"Định khoản: Nợ TK {offsetAcc} - Có TK {cashAcc} (Số tiền: {amt:N0} đ)";

            bool isDebt = IsDebtAccount(offsetAcc);
            string partnerId = luePartner.EditValue?.ToString() ?? "";

            if (isDebt)
            {
                if (!string.IsNullOrEmpty(partnerId))
                {
                    string pName = (luePartner.SelectedItem as PartnerEntity)?.CustomerName ?? partnerId;
                    lblDebtStatus.Text = $"✓ Đã tự động liên kết công nợ với đối tượng: {pName} ({partnerId})";
                    lblDebtStatus.Foreground = new SolidColorBrush(Color.FromRgb(22, 163, 74)); // Green
                }
                else
                {
                    lblDebtStatus.Text = $"* BẮT BUỘC CHỌN ĐỐI TƯỢNG: Tài khoản {offsetAcc} là tài khoản theo dõi công nợ!";
                    lblDebtStatus.Foreground = new SolidColorBrush(Color.FromRgb(220, 38, 38)); // Red
                }
            }
            else
            {
                lblDebtStatus.Text = "✓ Hạch toán trực tiếp, không yêu cầu chi tiết công nợ.";
                lblDebtStatus.Foreground = new SolidColorBrush(Color.FromRgb(71, 85, 105)); // Slate
            }
        }

        private void BtnOpenDetail_Click(object sender, RoutedEventArgs e)
        {
            string offsetAcc = lueOffsetAccount.EditValue?.ToString() ?? "";
            if (string.IsNullOrWhiteSpace(offsetAcc))
            {
                ThemedMessageBox.Show("Thông báo", "Vui lòng chọn Tài khoản đối ứng trước khi mở chi tiết!", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            decimal amt = Convert.ToDecimal(txtAmount.EditValue ?? 0m);
            string pid = luePartner.EditValue?.ToString() ?? "";

            var tempEntry = new JournalEntryEntity
            {
                AccountId = offsetAcc,
                Dbcr = 1, // Chi tiền thì TK đối ứng ghi Nợ (Dbcr = 1)
                Amount = amt
            };

            if (_manualDebtDetails != null && _manualDebtDetails.Count > 0)
            {
                foreach (var d in _manualDebtDetails) tempEntry.DebtDetails.Add(d);
            }
            else if (!string.IsNullOrEmpty(pid) && IsDebtAccount(offsetAcc))
            {
                tempEntry.DebtDetails.Add(new DebtDetailEntity
                {
                    PartnerId = pid,
                    Amount = amt,
                    Liabilitydate = dpVoucherDate.DateTime
                });
            }

            try
            {
                AccountLookUpEditBase.DetailWindowOpener?.Invoke(tempEntry, this, false);
                if (tempEntry.DebtDetails.Count > 0)
                {
                    _manualDebtDetails = tempEntry.DebtDetails.ToList();
                    UpdateDebtStatus();
                }
            }
            catch (Exception ex)
            {
                ThemedMessageBox.Show("Chi tiết", "Không thể mở cửa sổ chi tiết: " + ex.Message, MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnAccept_Click(object sender, RoutedEventArgs e)
        {
            if (SaveCurrentVoucher())
            {
                ThemedMessageBox.Show("Thành công", $"Đã lưu chứng từ {txtVoucherNo.Text.Trim()} thành công!", MessageBoxButton.OK, MessageBoxImage.Information);
                RefreshVoucherList();
                _currentVoucherIndex = _voucherIds.IndexOf(_currentVoucherId);
                RefreshNavigation();
            }
        }

        private void BtnSaveAndNew_Click(object sender, RoutedEventArgs e)
        {
            if (SaveCurrentVoucher())
            {
                ThemedMessageBox.Show("Thành công", $"Đã lưu chứng từ {txtVoucherNo.Text.Trim()} thành công!", MessageBoxButton.OK, MessageBoxImage.Information);
                RefreshVoucherList();
                SetNewVoucherMode();
                RefreshNavigation();
            }
        }

        private bool SaveCurrentVoucher()
        {
            string vNo = txtVoucherNo.Text.Trim();
            if (string.IsNullOrWhiteSpace(vNo))
            {
                ThemedMessageBox.Show("Cảnh báo", "Số chứng từ không được để trống!", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtVoucherNo.Focus();
                return false;
            }

            string cashAcc = (cboCashAccount.SelectedItem as PluginAccountConfigItemDto)?.AccountId ?? "";
            if (string.IsNullOrWhiteSpace(cashAcc))
            {
                ThemedMessageBox.Show("Cảnh báo", "Vui lòng chọn tài khoản quỹ/ngân hàng chi tiền!", MessageBoxButton.OK, MessageBoxImage.Warning);
                cboCashAccount.Focus();
                return false;
            }

            string offsetAcc = lueOffsetAccount.EditValue?.ToString() ?? "";
            if (string.IsNullOrWhiteSpace(offsetAcc))
            {
                ThemedMessageBox.Show("Cảnh báo", "Vui lòng chọn Tài khoản đối ứng (Nợ)!", MessageBoxButton.OK, MessageBoxImage.Warning);
                lueOffsetAccount.Focus();
                return false;
            }

            decimal amt = Convert.ToDecimal(txtAmount.EditValue ?? 0m);
            if (amt <= 0)
            {
                ThemedMessageBox.Show("Cảnh báo", "Số tiền chi phải lớn hơn 0!", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtAmount.Focus();
                return false;
            }

            string partnerId = luePartner.EditValue?.ToString() ?? "";
            bool isDebt = IsDebtAccount(offsetAcc);

            // KIỂM SOÁT NGHIÊM NGẶT CÔNG NỢ (Accounting First):
            if (isDebt && string.IsNullOrWhiteSpace(partnerId))
            {
                ThemedMessageBox.Show("Cảnh báo công nợ", 
                    $"Tài khoản đối ứng {offsetAcc} là tài khoản theo dõi công nợ.\nBạn bắt buộc phải chọn Mã đối tượng nhận trước khi lưu!", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                luePartner.Focus();
                return false;
            }

            string vType = GetCurrentVoucherType();
            var p = new CashVoucherSaveParams
            {
                VoucherId = _currentVoucherId,
                VoucherType = vType,
                VoucherNo = vNo,
                VoucherDate = dpVoucherDate.DateTime,
                Bookflag = chkBookflag.IsChecked == true,
                Method = (vType == "BN" || vType == "UNC") ? "Bank" : "Cash",
                CashAccountId = cashAcc,
                PartnerId = partnerId,
                PartnerName = (luePartner.SelectedItem as PartnerEntity)?.CustomerName ?? "",
                PersonName = txtPersonName.Text.Trim(),
                PersonAddress = txtPersonAddress.Text.Trim(),
                InvoicesNumber = txtInvoicesNumber.Text.Trim(),
                Description = txtDescription.Text.Trim(),
                Paymentacc = txtPaymentAcc.Text.Trim(),
                Paymentbank = txtPaymentBank.Text.Trim(),
                Recievedcorp = txtRecievedCorp.Text.Trim(),
                Recievedacc = txtRecievedAcc.Text.Trim(),
                Recievedbank = txtRecievedBank.Text.Trim()
            };

            var line = new CashVoucherLineDto
            {
                LineId = 1,
                AccountId = offsetAcc,
                Description = txtDescription.Text.Trim(),
                Amount = amt,
                PartnerId = partnerId
            };

            if (_manualDebtDetails != null && _manualDebtDetails.Count > 0)
            {
                line.DebtDetails = _manualDebtDetails;
            }
            else if (isDebt && !string.IsNullOrEmpty(partnerId))
            {
                line.DebtDetails.Add(new DebtDetailEntity
                {
                    PartnerId = partnerId,
                    Amount = amt,
                    Liabilitydate = p.VoucherDate
                });
            }

            p.Lines.Add(line);

            if (!CashVoucherDataService.SaveVoucher(p, out string error, out int savedId))
            {
                ThemedMessageBox.Show("Lỗi lưu chứng từ", error, MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            _currentVoucherId = savedId;
            return true;
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (_currentVoucherId == 0)
            {
                ThemedMessageBox.Show("Thông báo", "Phiếu chưa được lưu vào cơ sở dữ liệu, không thể xóa!", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (_isCurrentVoucherPosted)
            {
                ThemedMessageBox.Show("Cảnh báo", "Chứng từ này đã ghi sổ cái, không được phép xóa!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var res = ThemedMessageBox.Show("Xác nhận xóa", 
                $"Bạn có chắc chắn muốn xóa chứng từ số {txtVoucherNo.Text.Trim()} không?", 
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (res == MessageBoxResult.Yes)
            {
                if (CashVoucherDataService.DeleteVoucher(_currentVoucherId, out string error))
                {
                    ThemedMessageBox.Show("Thành công", "Đã xóa chứng từ thành công!", MessageBoxButton.OK, MessageBoxImage.Information);
                    RefreshVoucherList();
                    if (_voucherIds.Count > 0)
                    {
                        if (_currentVoucherIndex >= _voucherIds.Count) _currentVoucherIndex = _voucherIds.Count - 1;
                        NavigateToVoucher(_currentVoucherIndex);
                    }
                    else
                    {
                        SetNewVoucherMode();
                    }
                    RefreshNavigation();
                }
                else
                {
                    ThemedMessageBox.Show("Lỗi xóa", error, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnPrint_Click(object sender, RoutedEventArgs e)
        {
            if (_currentVoucherId == 0)
            {
                ThemedMessageBox.Show("Thông báo", "Vui lòng lưu chứng từ trước khi in ấn!", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var p = CashVoucherDataService.LoadVoucherDetail(_currentVoucherId);
            if (p != null)
            {
                CashPrintHelper.PrintPayment(this, p);
            }
        }

        private void BtnNew_Click(object sender, RoutedEventArgs e)
        {
            SetNewVoucherMode();
            RefreshNavigation();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void RefreshVoucherList()
        {
            string prefix = GetCurrentVoucherType();
            _voucherIds = CashVoucherDataService.GetVoucherIds(prefix);
        }

        private void RefreshNavigation()
        {
            btnFirst.IsEnabled = _voucherIds.Count > 0 && _currentVoucherIndex > 0;
            btnPrev.IsEnabled = _voucherIds.Count > 0 && _currentVoucherIndex > 0;
            btnNext.IsEnabled = _voucherIds.Count > 0 && _currentVoucherIndex < _voucherIds.Count - 1;
            btnLast.IsEnabled = _voucherIds.Count > 0 && _currentVoucherIndex < _voucherIds.Count - 1;

            if (_currentVoucherId == 0)
            {
                lblNavPosition.Content = "Phiếu mới (chưa lưu)";
            }
            else
            {
                string statusText = _isCurrentVoucherPosted ? " [Đã ghi sổ]" : " [Chưa ghi sổ]";
                lblNavPosition.Content = $"Phiếu {_currentVoucherIndex + 1}/{_voucherIds.Count}{statusText}";
            }
        }

        private void NavigateToVoucher(int index)
        {
            if (index < 0 || index >= _voucherIds.Count) return;

            int vId = _voucherIds[index];
            var p = CashVoucherDataService.LoadVoucherDetail(vId);
            if (p == null) return;

            _currentVoucherId = p.VoucherId;
            _currentVoucherIndex = index;
            _isCurrentVoucherPosted = p.Bookflag;
            _manualDebtDetails = null;

            txtVoucherNo.Text = p.VoucherNo;
            dpVoucherDate.DateTime = p.VoucherDate;
            chkBookflag.IsChecked = p.Bookflag;

            if (p.VoucherType == "UNC") cboMethod.SelectedIndex = 2;
            else if (p.VoucherType == "BN") cboMethod.SelectedIndex = 1;
            else cboMethod.SelectedIndex = 0;

            var matchCash = _cashAccounts.FirstOrDefault(x => x.AccountId == p.CashAccountId);
            if (matchCash != null) cboCashAccount.SelectedItem = matchCash;

            txtPersonName.Text = p.PersonName;
            txtPersonAddress.Text = p.PersonAddress;
            txtInvoicesNumber.Text = p.InvoicesNumber;
            txtDescription.Text = p.Description;
            txtPaymentAcc.Text = p.Paymentacc;
            txtPaymentBank.Text = p.Paymentbank;
            txtRecievedCorp.Text = p.Recievedcorp;
            txtRecievedAcc.Text = p.Recievedacc;
            txtRecievedBank.Text = p.Recievedbank;

            if (p.Lines != null && p.Lines.Count > 0)
            {
                var line = p.Lines[0];
                lueOffsetAccount.EditValue = line.AccountId;
                txtAmount.EditValue = line.Amount;
                if (line.DebtDetails != null && line.DebtDetails.Count > 0)
                {
                    _manualDebtDetails = line.DebtDetails;
                    string pid = line.DebtDetails.FirstOrDefault(x => !string.IsNullOrEmpty(x.PartnerId))?.PartnerId ?? "";
                    if (!string.IsNullOrEmpty(pid)) p.PartnerId = pid;
                }
            }
            else
            {
                txtAmount.EditValue = 0m;
            }

            luePartner.EditValue = p.PartnerId;
            UpdateDebtStatus();
            RefreshNavigation();
        }

        private void BtnFirst_Click(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            if (_voucherIds.Count > 0) NavigateToVoucher(0);
        }

        private void BtnPrev_Click(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            if (_currentVoucherIndex > 0) NavigateToVoucher(_currentVoucherIndex - 1);
        }

        private void BtnNext_Click(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            if (_currentVoucherIndex < _voucherIds.Count - 1) NavigateToVoucher(_currentVoucherIndex + 1);
        }

        private void BtnLast_Click(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            if (_voucherIds.Count > 0) NavigateToVoucher(_voucherIds.Count - 1);
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) { Close(); e.Handled = true; }
            else if (e.Key == Key.S && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
                {
                    BtnSaveAndNew_Click(sender, e);
                }
                else
                {
                    BtnAccept_Click(sender, e);
                }
                e.Handled = true;
            }
            else if (e.Key == Key.N && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                BtnNew_Click(sender, e);
                e.Handled = true;
            }
            else if (e.Key == Key.P && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                BtnPrint_Click(sender, e);
                e.Handled = true;
            }
            else if (e.Key == Key.Home)
            {
                BtnFirst_Click(sender, null!);
                e.Handled = true;
            }
            else if (e.Key == Key.PageUp)
            {
                BtnPrev_Click(sender, null!);
                e.Handled = true;
            }
            else if (e.Key == Key.PageDown)
            {
                BtnNext_Click(sender, null!);
                e.Handled = true;
            }
            else if (e.Key == Key.End)
            {
                BtnLast_Click(sender, null!);
                e.Handled = true;
            }
        }
    }
}
