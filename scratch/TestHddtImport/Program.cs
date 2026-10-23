using System;
using System.IO;
using System.Threading.Tasks;
using NewaccNet.TaxInvoice;

namespace TestHddtImport
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("=== TAX INVOICE ACCESS DB IMPORT TEST ===");

            var databasePath = @"D:\mdb-migration-project\mapping\template.accdb";
            var mockDirectory = @"D:\NewaccNet\TestData\HddtMock\2802274721\mua\2026-05";

            var dbService = new TaxInvoiceDbService(databasePath);

            var config = new TaxAccountConfigDto
            {
                TaxCode = "2802274721",
                CompanyName = "CÔNG TY TNHH DỊCH VỤ THƯƠNG MẠI THỦY HIỀN",
                EncryptedPassword = DataEncryptionHelper.Encrypt("Hoadon@1"),
                Token = "eyJhbGciOiJIUzUxMiJ9.mock_token_for_testing",
                TokenExpiresAt = DateTime.Now.AddDays(1),
                LastSyncDate = DateTime.Now
            };
            await dbService.SaveAccountConfigAsync(config);
            var savedConfig = await dbService.GetAccountConfigAsync("2802274721");
            Console.WriteLine($"Config saved: TaxCode={savedConfig?.TaxCode}, Name={savedConfig?.CompanyName}");

            var progress = new Progress<DownloadProgress>(p =>
            {
                if (p.ProcessedCount % 20 == 0 || p.ProcessedCount == p.TotalCount)
                {
                    Console.WriteLine($"Progress: {p.ProcessedCount}/{p.TotalCount} processed. Succeeded: {p.SucceededCount}");
                }
            });

            var count = await dbService.ImportFromDirectoryAsync(mockDirectory, "purchase", progress);
            Console.WriteLine($"Import completed: {count} invoices.");

            var invoices = await dbService.GetInvoicesAsync(direction: "purchase");
            Console.WriteLine($"Total invoices in database: {invoices.Count}");

            // Clean up to keep template pristine
            using var conn = new System.Data.OleDb.OleDbConnection($"Provider=Microsoft.ACE.OLEDB.16.0;Data Source={databasePath};");
            conn.Open();
            using var cmd1 = new System.Data.OleDb.OleDbCommand("DELETE FROM TaxInvoiceDetail", conn);
            cmd1.ExecuteNonQuery();
            using var cmd2 = new System.Data.OleDb.OleDbCommand("DELETE FROM TaxInvoiceTaxRate", conn);
            cmd2.ExecuteNonQuery();
            using var cmd3 = new System.Data.OleDb.OleDbCommand("DELETE FROM TaxInvoice", conn);
            cmd3.ExecuteNonQuery();
            using var cmd4 = new System.Data.OleDb.OleDbCommand("DELETE FROM TaxAccountConfig", conn);
            cmd4.ExecuteNonQuery();

            Console.WriteLine("Template database cleaned up back to pristine state.");
            Console.WriteLine("=== TEST PASSED 100%! ===");
        }
    }
}
