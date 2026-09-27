using System;
using System.Windows;
using SD.LLBLGen.Pro.ORMSupportClasses;
using DataAccess.EntityClasses;
using NewaccNet.Wpf.AppSystem;
using DataAccess.HelperClasses;

namespace NewaccNet.Wpf.AppSystem.Voucher.Details
{
    public partial class CurrencyLiabilityWindow : DevExpress.Xpf.Core.ThemedWindow
    {
        private DebtDetailEntity _debtRow;
        private CurrencyLiabilityLineEntity _currencyLine;

        public CurrencyLiabilityWindow(DebtDetailEntity debtRow)
        {
            InitializeComponent();
            _debtRow = debtRow;
            
            // Nếu chưa có chi tiết ngoại tệ thì tạo mới
            _currencyLine = _debtRow.CurrencyLiabilityLine ?? new CurrencyLiabilityLineEntity();
            
            this.DataContext = _currencyLine;
            UpdateTotalText();
            LoadDictionaries();
        }

        private void LoadDictionaries()
        {
            try
            {
                using (var adapter = AppDataAccessAdapter.Create())
                {
                    var currencies = new EntityCollection<CurrencyEntity>();
                    adapter.FetchEntityCollection(currencies, null);
                    lookupCurrency.ItemsSource = currencies;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh mục tiền tệ: " + ex.Message);
            }
        }

        private void CalculateTotal_EditValueChanged(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e)
        {
            UpdateTotalText();
        }

        private void UpdateTotalText()
        {
            double qty = _currencyLine.Quantity ?? 0;
            double rate = _currencyLine.ExchangeRate ?? 0;
            txtTotalAmount.Text = (qty * rate).ToString("N0");
        }

        private void BtnAccept_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_currencyLine.CurrencyId))
            {
                MessageBox.Show("Vui lòng chọn Mã ngoại tệ!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Gắn lại vào dòng công nợ
            _debtRow.CurrencyLiabilityLine = _currencyLine;
            
            // Tự động tính lại số tiền VND trên lưới công nợ
            double qty = _currencyLine.Quantity ?? 0;
            double rate = _currencyLine.ExchangeRate ?? 0;
            if (qty > 0 && rate > 0)
            {
                _debtRow.Amount = qty * rate;
            }

            this.DialogResult = true;
            this.Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            // Trả về false để form gọi biết đường uncheck ô Ngoại tệ nếu vừa tạo mới
            this.DialogResult = false;
            this.Close();
        }
    }
}