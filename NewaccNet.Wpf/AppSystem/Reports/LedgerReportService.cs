using System;
using System.Windows;
using System.Windows.Input;
using System.Linq;
using SD.LLBLGen.Pro.QuerySpec;
using SD.LLBLGen.Pro.QuerySpec.Adapter;
using DataAccess.FactoryClasses;
using DataAccess.HelperClasses;
using DataAccess.EntityClasses;
using NewaccNet.Reports;

namespace NewaccNet.Wpf.AppSystem.Reports
{
    public static class LedgerReportService
    {
        public static void ShowPreview(Window owner, DateTime fromDate, DateTime toDate, string accountId, bool onlyBooked = true, string templatePath = "")
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;

                using var adapter = AppDataAccessAdapter.Create();
                var qf = new QueryFactory();

                // Lọc tất cả các JournalEntry liên quan đến AccountId này (bao gồm cả tài khoản con)
                var q = qf.JournalEntry
                    .Where(JournalEntryFields.AccountId.StartsWith(accountId))
                    .WithPath(
                        JournalEntryEntity.PrefetchPathJournalVoucher,
                        JournalEntryEntity.PrefetchPathParentEntry,
                        JournalEntryEntity.PrefetchPathSubEntries
                    );

                var rawEntries = adapter.FetchQuery(q).Cast<JournalEntryEntity>().ToList();

                if (rawEntries == null || rawEntries.Count == 0)
                {
                    Mouse.OverrideCursor = null;
                    MessageBox.Show(owner,
                        $"Không tìm thấy dữ liệu phát sinh nào cho tài khoản {accountId}.",
                        "Dữ liệu trống",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                // Gọi class tính toán ở project Reports
                var calc = new LedgerCalculator();
                var dtoList = calc.Calculate(adapter, rawEntries, accountId, fromDate, toDate, onlyBooked);

                // Lấy thông tin Tên Tài khoản để truyền vào header báo cáo
                string mockAccountName = $"Tài khoản {accountId}";
                var acc = adapter.FetchFirst(qf.ChartOfAccount.Where(ChartOfAccountFields.AccountId.Equal(accountId)));
                if (acc != null) mockAccountName = acc.AccountName;

                string mockCompanyName = "Công ty Cổ phần MOCK - Chờ lấy từ Config";

                Mouse.OverrideCursor = null;

                // Gọi factory để show report
                LedgerReportFactory.ShowPreview(owner, templatePath, dtoList, mockCompanyName, accountId, mockAccountName, fromDate, toDate, onlyBooked);
            }
            catch (Exception ex)
            {
                Mouse.OverrideCursor = null;
                string detail = ex.InnerException?.Message ?? ex.Message;
                MessageBox.Show(owner,
                    "Lỗi khi xây dựng Sổ cái: " + detail,
                    "Lỗi hệ thống",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
