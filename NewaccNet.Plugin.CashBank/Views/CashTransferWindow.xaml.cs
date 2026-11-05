using NewaccNet.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using DevExpress.Xpf.Core;
using NewaccNet.Core.Helpers;
using NewaccNet.Plugin.CashBank.Services;

namespace NewaccNet.Plugin.CashBank.Views
{
    public partial class CashTransferWindow : ThemedWindow
    {
        private List<PluginAccountConfigItemDto> _cashAccounts = new();

        public CashTransferWindow()
        {
            InitializeComponent();
            InitData();
        }

        private void InitData()
        {
            try
            {
                _cashAccounts = CashVoucherDataService.LoadCashAccounts();
                cboSourceAccount.ItemsSource = _cashAccounts;
                cboDestinationAccount.ItemsSource = _cashAccounts;

                var cfg = CashVoucherDataService.GetPostingConfig();
                cboSourceAccount.EditValue = cfg.DefaultBankAccount ?? "1121";
                cboDestinationAccount.EditValue = cfg.DefaultCashAccount ?? "1111";

                txtVoucherNo.Text = CashVoucherDataService.GenerateNextVoucherNo("CKT");
                dpVoucherDate.DateTime = DateTime.Today;
                chkBookflag.IsChecked = false; // Mặc định chưa ghi sổ

                UpdateDefaultDescription();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khởi tạo chuyển tiền nội bộ: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Account_EditValueChanged(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e)
        {
            UpdateDefaultDescription();
        }

        private void UpdateDefaultDescription()
        {
            string src = cboSourceAccount.EditValue?.ToString() ?? "";
            string dst = cboDestinationAccount.EditValue?.ToString() ?? "";

            if (src.StartsWith("112") && dst.StartsWith("111"))
            {
                txtDescription.Text = "Rút tiền gửi ngân hàng về nhập quỹ tiền mặt";
            }
            else if (src.StartsWith("111") && dst.StartsWith("112"))
            {
                txtDescription.Text = "Nộp tiền mặt tại quỹ vào tài khoản ngân hàng";
            }
            else if (src.StartsWith("112") && dst.StartsWith("112"))
            {
                txtDescription.Text = "Chuyển tiền giữa các tài khoản ngân hàng";
            }
            else
            {
                txtDescription.Text = "Chuyển tiền nội bộ";
            }
        }

        private void TxtAmount_EditValueChanged(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e)
        {
            decimal amt = 0;
            if (txtAmount.EditValue != null && decimal.TryParse(txtAmount.EditValue.ToString(), out decimal val))
            {
                amt = val;
            }
            lblAmountWords.Text = NumberToWordsConverter.ConvertToWords(amt, "đồng");
        }

        private void BtnAccept_Click(object sender, RoutedEventArgs e)
        {
            string voucherNo = txtVoucherNo.Text?.Trim() ?? "";
            if (string.IsNullOrEmpty(voucherNo))
            {
                MessageBox.Show("Vui lòng nhập Số chứng từ.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtVoucherNo.Focus();
                return;
            }

            string src = cboSourceAccount.EditValue?.ToString() ?? "";
            string dst = cboDestinationAccount.EditValue?.ToString() ?? "";

            if (string.IsNullOrEmpty(src) || string.IsNullOrEmpty(dst))
            {
                MessageBox.Show("Vui lòng chọn cả Tài khoản nguồn và Tài khoản đích.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (src.Equals(dst, StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("Tài khoản nguồn và Tài khoản đích không được trùng nhau!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            decimal amt = 0;
            if (txtAmount.EditValue != null && decimal.TryParse(txtAmount.EditValue.ToString(), out decimal val))
            {
                amt = val;
            }

            if (amt <= 0)
            {
                MessageBox.Show("Số tiền chuyển phải lớn hơn 0.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtAmount.Focus();
                return;
            }

            var p = new CashVoucherSaveParams
            {
                VoucherType = "CKT",
                VoucherNo = voucherNo,
                VoucherDate = dpVoucherDate.DateTime,
                Bookflag = chkBookflag.IsChecked ?? false,
                CashAccountId = src,
                DestinationAccountId = dst,
                PersonName = txtPersonName.Text?.Trim() ?? "",
                Description = txtDescription.Text?.Trim() ?? "Chuyển tiền nội bộ",
                Lines = new List<CashVoucherLineDto>
                {
                    new CashVoucherLineDto
                    {
                        LineId = 1,
                        AccountId = dst,
                        Description = txtDescription.Text?.Trim() ?? "Chuyển tiền nội bộ",
                        Amount = amt
                    }
                }
            };

            bool ok = CashVoucherDataService.SaveVoucher(p, out string err, out int savedId);
            if (ok)
            {
                MessageBox.Show($"Đã lưu thành công phiếu chuyển tiền nội bộ {voucherNo}!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Lỗi khi lưu phiếu chuyển tiền: " + err, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
