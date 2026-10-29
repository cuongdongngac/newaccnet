using System;
using System.Windows;
using System.Windows.Input;
using NewaccNet.Core.Reports.Calculators;

namespace NewaccNet.Wpf.AppSystem.Reports
{
    public static class DiaryReportService
    {
        public static void ShowPreview(Window owner, DateTime fromDate, DateTime toDate, bool onlyBooked)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;
                var calc = new DiaryCalculator();
                var rows = calc.Calculate(fromDate, toDate, onlyBooked);

                if (rows == null || rows.Count == 0)
                {
                    Mouse.OverrideCursor = null;
                    MessageBox.Show(owner,
                        $"Không tìm thấy bút toán nào trong kỳ từ {fromDate:dd/MM/yyyy} đến {toDate:dd/MM/yyyy}{(onlyBooked ? " (đã ghi sổ)" : "")}.\n\nVui lòng kiểm tra lại khoảng thời gian hoặc bỏ chọn 'Chỉ CT đã ghi sổ'.",
                        "Sổ Nhật ký trống",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                var dtoList = new NewaccNet.Reports.DiaryCalculator().Calculate(rows);
                var report = new NewaccNet.Wpf.AppSystem.Reports.diary.DiaryReport(dtoList)
                {
                    RequestParameters = false
                };

                if (report.Parameters["pPeriodTitle"] != null)
                {
                    report.Parameters["pPeriodTitle"].Value = $"Từ ngày: {fromDate:dd/MM/yyyy} Đến ngày: {toDate:dd/MM/yyyy}";
                }

                string companyName = DbConfigService.Current.CompanyName ?? string.Empty;
                if (report.Parameters["pCompanyName"] != null)
                {
                    report.Parameters["pCompanyName"].Value = companyName;
                }
                var lblComp = report.FindControl("lblCompanyName", true) as DevExpress.XtraReports.UI.XRLabel;
                if (lblComp != null)
                {
                    lblComp.Text = companyName;
                }
                Mouse.OverrideCursor = null;
                DevExpress.Xpf.Printing.PrintHelper.ShowPrintPreview(owner, report);
            }
            catch (Exception ex)
            {
                Mouse.OverrideCursor = null;
                string detail = ex.InnerException?.Message ?? ex.Message;
                MessageBox.Show(owner,
                    "Lỗi khi xây dựng Nhật ký: " + detail,
                    "Lỗi hệ thống",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

