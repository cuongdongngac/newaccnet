using System;
using System.Windows;
using DevExpress.Xpf.Printing;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.UI;
using NewaccNet.Wpf.AppSystem.Voucher;

namespace NewaccNet.Wpf.AppSystem.Reports
{
    /// <summary>
    /// Bộ điều phối Drill-down tổng quát cho toàn bộ hệ thống Báo cáo (Bảng tổng hợp & Sổ chi tiết).
    /// Áp dụng mô hình chuẩn 2 cấp Drill-down:
    ///   - Cấp 1 (Tổng hợp -> Chi tiết): Click vào dòng tổng hợp để mở Sổ chi tiết theo đối tượng.
    ///   - Cấp 2 (Chi tiết -> Chứng từ): Click vào dòng chứng từ chi tiết để mở thẳng VoucherEntryWindow.
    /// </summary>
    public static class ReportDrillDownHelper
    {
        /// <summary>
        /// Mở màn hình chứng từ gốc (VoucherEntryWindow) từ JournalVoucherId.
        /// Chạy an toàn trên luồng UI Dispatcher.
        /// </summary>
        public static void OpenVoucher(int voucherId, Window? owner = null)
        {
            if (voucherId <= 0) return;

            var app = Application.Current;
            if (app == null) return;

            app.Dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    var win = new VoucherEntryWindow(voucherId)
                    {
                        Owner = owner ?? app.MainWindow
                    };
                    win.Show();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"Không thể mở chứng từ #{voucherId}:\n{ex.Message}",
                        "Lỗi mở chứng từ",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }));
        }

        /// <summary>
        /// Gắn cơ chế nhận diện click chứng từ (Cấp 2) lên DocumentPreviewWindow.
        /// Khi người dùng click vào bất kỳ ô nào có chứa JournalVoucherId, hệ thống sẽ mở VoucherEntryWindow.
        /// </summary>
        public static void WireVoucherClick(DocumentPreviewWindow previewWindow)
        {
            if (previewWindow?.PreviewControl == null) return;

            previewWindow.PreviewControl.DocumentPreviewMouseClick += (sPreview, eClick) =>
            {
                if (eClick.Brick is VisualBrick visualBrick)
                {
                    object? val = visualBrick.Value;
                    if (val != null)
                    {
                        if (val is int vid && vid > 0)
                        {
                            OpenVoucher(vid, previewWindow);
                            return;
                        }

                        if (int.TryParse(val.ToString(), out int parsedId) && parsedId > 0)
                        {
                            OpenVoucher(parsedId, previewWindow);
                            return;
                        }
                    }
                }
            };
        }

        /// <summary>
        /// Gắn cơ chế nhận diện click đối tượng (Cấp 1) từ Báo cáo Tổng hợp.
        /// Chuỗi giá trị trong Brick có định dạng "Kho|VậtTư" hoặc "Group1Id|Group2Id".
        /// </summary>
        public static void WireDrillDownKeys(
            DocumentPreviewWindow summaryWindow,
            Action<string, string> onPairSelected)
        {
            if (summaryWindow?.PreviewControl == null) return;

            summaryWindow.PreviewControl.DocumentPreviewMouseClick += (sPreview, eClick) =>
            {
                if (eClick.Brick is VisualBrick visualBrick && visualBrick.Value != null)
                {
                    string rawVal = visualBrick.Value.ToString()?.Trim() ?? string.Empty;
                    if (!string.IsNullOrEmpty(rawVal))
                    {
                        string[] parts = rawVal.Split('|');
                        if (parts.Length == 2)
                        {
                            onPairSelected(parts[0].Trim(), parts[1].Trim());
                        }
                        else if (parts.Length == 1)
                        {
                            onPairSelected(parts[0].Trim(), string.Empty);
                        }
                    }
                }
            };
        }
    }
}
