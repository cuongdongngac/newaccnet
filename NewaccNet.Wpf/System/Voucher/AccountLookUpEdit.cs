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
                currentEntry = new JournalEntryEntity(); 
            }

            using (var adapter = AppDataAccessAdapter.Create())
            {
                var account = new ChartOfAccountEntity(accountId);
                if (adapter.FetchEntity(account))
                {
                    if (string.IsNullOrEmpty(account.CategoryId) && (accountId.StartsWith("131") || accountId.StartsWith("331")))
                    {
                        account.CategoryId = "A"; 
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
                var debtForm = new Details.DebtDetailWindow(entry);
                if (owner != null) debtForm.Owner = owner;
                
                bool? result = debtForm.ShowDialog();
                if (result == true && owner is VoucherEntryWindow voucherWindow)
                {
                    voucherWindow.CalculateBalance();
                }
            }
            else if (account.CategoryId == "B")
            {
                var materialForm = new Details.MaterialDetailWindow(entry, true);
                if (owner != null) materialForm.Owner = owner;
                bool? result = materialForm.ShowDialog();
                if (result == true && owner is VoucherEntryWindow voucherWindow)
                {
                    if (materialForm.VoucherType == "XB") // Xuất bán
                    {
                        entry.AccountId = materialForm.SelectedCostAccount;
                        entry.Dbcr = (short)1;
                        entry.Amount = materialForm.TotalCostAmount;
                        
                        var subCost = new JournalEntryEntity();
                        subCost.AccountId = account.AccountId; 
                        subCost.Dbcr = (short)-1;
                        subCost.Amount = materialForm.TotalCostAmount;
                        entry.SubEntries.Add(subCost);

                        var revMaster = new JournalEntryEntity();
                        revMaster.AccountId = materialForm.SelectedTotalAccount; 
                        revMaster.Dbcr = (short)1;
                        revMaster.Amount = materialForm.TotalSellAmount + materialForm.TotalTaxAmount;
                        
                        var subRev = new JournalEntryEntity();
                        subRev.AccountId = materialForm.SelectedRevenueAccount; 
                        subRev.Dbcr = (short)-1;
                        subRev.Amount = materialForm.TotalSellAmount;
                        revMaster.SubEntries.Add(subRev);
                        
                        if (materialForm.TotalTaxAmount > 0)
                        {
                            var subTax = new JournalEntryEntity();
                            subTax.AccountId = materialForm.SelectedTaxAccount; 
                            subTax.Dbcr = (short)-1;
                            subTax.Amount = materialForm.TotalTaxAmount;
                            revMaster.SubEntries.Add(subTax);
                        }
                        
                        ((System.Collections.IList)voucherWindow.gridLeft.ItemsSource).Add(revMaster);
                    }
                    else if (materialForm.VoucherType == "NM") // Nhập mua
                    {
                        entry.AccountId = materialForm.SelectedTotalAccount;
                        entry.Dbcr = (short)-1;
                        entry.Amount = materialForm.TotalCostAmount + materialForm.TotalTaxAmount;
                        
                        var subCost = new JournalEntryEntity();
                        subCost.AccountId = account.AccountId; 
                        subCost.Dbcr = (short)1;
                        subCost.Amount = materialForm.TotalCostAmount;
                        entry.SubEntries.Add(subCost);
                        
                        if (materialForm.TotalTaxAmount > 0)
                        {
                            var subTax = new JournalEntryEntity();
                            subTax.AccountId = materialForm.SelectedTaxAccount; 
                            subTax.Dbcr = (short)1;
                            subTax.Amount = materialForm.TotalTaxAmount;
                            entry.SubEntries.Add(subTax);
                        }
                    }
                    else // Nhập/Xuất nội bộ
                    {
                        entry.AccountId = materialForm.SelectedTotalAccount;
                        entry.Amount = materialForm.TotalCostAmount;
                        
                        var subCost = new JournalEntryEntity();
                        subCost.AccountId = account.AccountId; 
                        subCost.Dbcr = (short)(entry.Dbcr == 1 ? -1 : 1);
                        subCost.Amount = materialForm.TotalCostAmount;
                        entry.SubEntries.Add(subCost);
                    }

                    voucherWindow.CalculateBalance();
                    var view = voucherWindow.gridLeft.View as DevExpress.Xpf.Grid.TableView;
                    if (view != null) 
                    {
                        view.CloseEditor();
                        voucherWindow.gridLeft.RefreshRow(view.FocusedRowHandle);
                    }
                }
            }
            else if (account.CategoryId == "E")
            {
                bool hasTax = account.Taxflag;
                var costForm = new Details.CostDetailWindow(entry, hasTax);
                if (owner != null) costForm.Owner = owner;
                bool? result = costForm.ShowDialog();
                if (result == true && owner is VoucherEntryWindow voucherWindow)
                {
                    voucherWindow.CalculateBalance();
                    var view = voucherWindow.gridLeft.View as DevExpress.Xpf.Grid.TableView;
                    if (view != null) 
                    {
                        view.CloseEditor();
                        voucherWindow.gridLeft.RefreshRow(view.FocusedRowHandle);
                    }
                }
            }
        }
    }
}

