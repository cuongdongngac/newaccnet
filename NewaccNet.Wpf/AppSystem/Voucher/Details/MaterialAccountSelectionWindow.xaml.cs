using System;
using System.Windows;
using DataAccess.EntityClasses;
using DataAccess.HelperClasses;
using SD.LLBLGen.Pro.ORMSupportClasses;
using System.Linq;

namespace NewaccNet.Wpf.AppSystem.Voucher.Details
{
    public partial class MaterialAccountSelectionWindow : Window
    {
        private string _voucherType;

        // Results
        public string CostAccount { get; private set; }
        public string RevenueAccount { get; private set; }
        public string TaxAccount { get; private set; }
        public string TotalAccount { get; private set; }

        public MaterialAccountSelectionWindow(string voucherType, double totalCost, double totalSell, double totalTax)
        {
            InitializeComponent();
            _voucherType = voucherType;

            txtTotalCost.EditValue = totalCost;
            txtTotalSell.EditValue = totalSell;
            txtTotalTax.EditValue = totalTax;

            SetupUI(totalCost, totalSell, totalTax);
            LoadAccountData();
        }

        private void SetupUI(double totalCost, double totalSell, double totalTax)
        {
            if (_voucherType == "XB") // Xuất bán
            {
                itemRevenue.Visibility = Visibility.Visible;
                itemTax.Visibility = Visibility.Visible;
                itemTotalAll.Visibility = Visibility.Visible;
                itemTotalAll.Label = "Tổng thuế + Tiền bán";
                txtTotalAll.EditValue = totalSell + totalTax;
            }
            else if (_voucherType == "NM") // Nhập mua
            {
                itemTax.Visibility = Visibility.Visible;
                itemTotalAll.Visibility = Visibility.Visible;
                itemTotalAll.Label = "Tổng gốc + Tiền thuế";
                txtTotalAll.EditValue = totalCost + totalTax;
            }
            else // Nhập nội bộ, Xuất nội bộ
            {
                // Only Cost/Inventory
                itemTotalAll.Visibility = Visibility.Visible;
                itemTotalAll.Label = "Tài khoản đối ứng";
                txtTotalAll.EditValue = totalCost;
            }
        }

        private void LoadAccountData()
        {
            using (var adapter = AppDataAccessAdapter.Create())
            {
                var accounts = new EntityCollection<ChartOfAccountEntity>();
                adapter.FetchEntityCollection(accounts, null);
                var accList = accounts.ToList();

                lueCostAccount.ItemsSource = accList;
                lueRevenueAccount.ItemsSource = accList;
                lueTaxAccount.ItemsSource = accList;
                lueTotalAccount.ItemsSource = accList;
            }
        }

        private void BtnAccept_Click(object sender, RoutedEventArgs e)
        {
            CostAccount = lueCostAccount.EditValue as string;
            RevenueAccount = lueRevenueAccount.EditValue as string;
            TaxAccount = lueTaxAccount.EditValue as string;
            TotalAccount = lueTotalAccount.EditValue as string;

            if (string.IsNullOrEmpty(CostAccount))
            {
                MessageBox.Show("Vui lòng chọn tài khoản cho Tiền gốc.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_voucherType == "XB")
            {
                if (string.IsNullOrEmpty(RevenueAccount) || string.IsNullOrEmpty(TaxAccount) || string.IsNullOrEmpty(TotalAccount))
                {
                    MessageBox.Show("Vui lòng chọn đầy đủ tài khoản cho Doanh thu, Thuế và Phải thu.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }
            else if (_voucherType == "NM")
            {
                if (string.IsNullOrEmpty(TaxAccount) || string.IsNullOrEmpty(TotalAccount))
                {
                    MessageBox.Show("Vui lòng chọn đầy đủ tài khoản cho Thuế và Phải trả.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }
            else
            {
                if (string.IsNullOrEmpty(TotalAccount))
                {
                    MessageBox.Show("Vui lòng chọn tài khoản đối ứng.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            this.DialogResult = true;
            this.Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}


