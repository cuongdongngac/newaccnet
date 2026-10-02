using System;
using System.Windows;
using SD.LLBLGen.Pro.ORMSupportClasses;
using DataAccess.EntityClasses;
using NewaccNet.Wpf.AppSystem;
using DataAccess.HelperClasses;

namespace NewaccNet.Wpf.AppSystem.Voucher.Details
{
    public partial class CurrencyLiabilityWindow : NewaccNet.Wpf.Views.Base.BaseDetailWindow
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

            this.Loaded += (s, e) => PopulateNumericFields();
        }

        private void PopulateNumericFields()
        {
            txtQuantity.Text = _currencyLine.Quantity.HasValue ? _currencyLine.Quantity.Value.ToString("N2") : "";
            txtExchangeRate.Text = _currencyLine.ExchangeRate.HasValue ? _currencyLine.ExchangeRate.Value.ToString("N0") : "";
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

        public void NumericField_LostFocus(object sender, RoutedEventArgs e)
        {
            if (_currencyLine == null) return;
            var tb = sender as System.Windows.Controls.TextBox;
            if (tb == null) return;

            double val;
            bool ok = double.TryParse(tb.Text.Replace(",", "").Trim(),
                                      System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out val);

            if (tb.Name == "txtQuantity")
            {
                _currencyLine.Quantity = ok ? val : (double?)null;
                if (ok) tb.Text = val.ToString("N2");
            }
            else if (tb.Name == "txtExchangeRate")
            {
                _currencyLine.ExchangeRate = ok ? val : (double?)null;
                if (ok) tb.Text = val.ToString("N0");
            }

            UpdateTotalText();
        }

        private void UpdateTotalText()
        {
            double qty = _currencyLine.Quantity ?? 0;
            double rate = _currencyLine.ExchangeRate ?? 0;
            txtTotalAmount.Text = (qty * rate).ToString("N0");
        }

        protected override void BtnAccept_Click(object sender, RoutedEventArgs e)
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

        public bool IsDeleted { get; private set; } = false;

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            Delete();
        }

        public override void Delete()
        {
            if (MessageBox.Show("Bạn có chắc muốn xóa khai báo ngoại tệ 1-1 này?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                IsDeleted = true;
                this.DialogResult = true;
                this.Close();
            }
        }

        protected override void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            // Trả về false để form gọi biết đường uncheck ô Ngoại tệ nếu vừa tạo mới
            this.DialogResult = false;
            this.Close();
        }
    }
}