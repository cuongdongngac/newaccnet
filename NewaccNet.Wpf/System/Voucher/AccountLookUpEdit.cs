using System;
using System.Windows;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Grid.LookUp;
using DataAccess.EntityClasses;
using NewaccNet.Wpf.AppSystem;

namespace NewaccNet.Wpf.AppSystem.Voucher
{
    public class AccountLookUpEdit : LookUpEdit
    {
        public AccountLookUpEdit()
        {
            this.DisplayMember = "AccountId";
            this.ValueMember = "AccountId";
            this.AutoPopulateColumns = false;
            this.AutoComplete = true;
            this.ImmediatePopup = true;
            this.IsTextEditable = true;
            this.PopupWidth = 500; 

            // Nút 3 chấm
            var btn = new ButtonInfo();
            btn.Content = "...";
            btn.ToolTip = "Chi tiết tài khoản (Mở form phụ)";
            btn.GlyphKind = GlyphKind.Regular; 
            btn.Click += Btn_Click;
            this.Buttons.Add(btn);

            this.EditValueChanged += AccountLookUpEdit_EditValueChanged;
        }

        private void AccountLookUpEdit_EditValueChanged(object sender, EditValueChangedEventArgs e)
        {
            if (!this.IsKeyboardFocusWithin) return;
            if (e.OldValue == e.NewValue) return;

            TriggerDetailForm();
        }

        private void Btn_Click(object sender, RoutedEventArgs e)
        {
            TriggerDetailForm();
        }

        private void TriggerDetailForm()
        {
            if (this.EditValue == null) return;
            string accountId = this.EditValue.ToString();

            // 1. Xác định context: Đang đứng trên JournalEntryEntity nào?
            JournalEntryEntity currentEntry = null;
            if (this.DataContext is DevExpress.Xpf.Grid.EditGridCellData cellData)
            {
                currentEntry = cellData.RowData.Row as JournalEntryEntity;
            }
            else
            {
                currentEntry = this.DataContext as JournalEntryEntity;
            }

            if (currentEntry == null) 
            {
                // Fallback nếu đang chạy thử độc lập ngoài lưới
                currentEntry = new JournalEntryEntity(); 
            }

            // 2. Phân tích tính chất tài khoản
            using (var adapter = AppDataAccessAdapter.Create())
            {
                var account = new ChartOfAccountEntity(accountId);
                if (adapter.FetchEntity(account))
                {
                    // Fallback test
                    if (string.IsNullOrEmpty(account.CategoryId) && (accountId.StartsWith("131") || accountId.StartsWith("331")))
                    {
                        account.CategoryId = "A"; // Công nợ
                    }

                    RouteToDetailForm(account, currentEntry);
                }
            }
        }

        private void RouteToDetailForm(ChartOfAccountEntity account, JournalEntryEntity entry)
        {
            var owner = Window.GetWindow(this);

            if (account.CategoryId == "A")
            {
                // Loại A: Mở Form Công nợ
                var debtForm = new Details.DebtDetailWindow(entry);
                if (owner != null) debtForm.Owner = owner;
                
                bool? result = debtForm.ShowDialog();
                if (result == true && owner is VoucherEntryWindow voucherWindow)
                {
                    // Lưới tự cập nhật số tiền qua Binding INotifyPropertyChanged
                    // Nhưng ta cần chủ động gọi tính lại Tổng cân đối (Total Balance) ở màn hình ngoài
                    voucherWindow.CalculateBalance();
                }
            }
            else if (account.CategoryId == "B")
            {
                MessageBox.Show($"Tài khoản {account.AccountId} là loại B. Sẽ bật form tiếp theo (Vật tư/Kho) ở giai đoạn sau!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}