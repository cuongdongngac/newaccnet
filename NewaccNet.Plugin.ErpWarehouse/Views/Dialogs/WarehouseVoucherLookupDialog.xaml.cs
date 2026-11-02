using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using DevExpress.Xpf.Core;
using NewaccNet.Core.Posting;
using NewaccNet.Plugin.ErpWarehouse.Services;

namespace NewaccNet.Plugin.ErpWarehouse.Views.Dialogs
{
    public partial class WarehouseVoucherLookupDialog : ThemedWindow
    {
        private readonly string _voucherType;
        public int? SelectedVoucherId { get; private set; }

        public WarehouseVoucherLookupDialog(string voucherType)
        {
            InitializeComponent();
            _voucherType = voucherType;

            DateTime today = DateTime.Today;
            dtFrom.DateTime = new DateTime(today.Year, 1, 1);
            dtTo.DateTime = today;
            chkUnpostedOnly.IsChecked = true;

            string titleTypeName = voucherType switch
            {
                "NM" => "Phiếu nhập mua (NM)",
                "XB" => "Phiếu xuất bán (XB)",
                "NNB" => "Phiếu nhập nội bộ (NNB)",
                "XNB" => "Phiếu xuất nội bộ (XNB)",
                "CK" => "Phiếu chuyển kho (CK)",
                _ => voucherType
            };
            this.Title = $"Tìm kiếm chứng từ kho - {titleTypeName}";

            this.Loaded += (s, e) => LoadVouchers();
        }

        private void LoadVouchers()
        {
            try
            {
                DateTime? from = dtFrom.DateTime.Date;
                DateTime? to = dtTo.DateTime.Date.AddDays(1).AddSeconds(-1);
                bool unpostedOnly = chkUnpostedOnly.IsChecked == true;

                var list = WarehouseVoucherDataService.SearchWarehouseVouchers(_voucherType, unpostedOnly, from, to);
                gridVouchers.ItemsSource = list;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tìm kiếm: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            LoadVouchers();
        }

        private void GridVouchers_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            SelectCurrent();
        }

        private void BtnSelect_Click(object sender, RoutedEventArgs e)
        {
            SelectCurrent();
        }

        private void SelectCurrent()
        {
            if (gridVouchers.SelectedItem is WarehouseVoucherLookupDto selected)
            {
                SelectedVoucherId = selected.VoucherId;
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một chứng từ trong danh sách.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
