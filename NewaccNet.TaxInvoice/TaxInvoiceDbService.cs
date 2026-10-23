using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace NewaccNet.TaxInvoice
{
    public sealed class TaxInvoiceDbService
    {
        private readonly string _connectionString;

        public TaxInvoiceDbService(string databasePath)
        {
            DatabasePath = databasePath;
            _connectionString = $"Provider=Microsoft.ACE.OLEDB.16.0;Data Source={databasePath};Persist Security Info=False;";
        }

        public string DatabasePath { get; }

        private OleDbConnection CreateConnection()
        {
            return new OleDbConnection(_connectionString);
        }

        // ==========================================
        // Access OLE DB Parameter Helpers
        // ==========================================

        private static void AddString(OleDbCommand cmd, string? val, int size = 255)
        {
            var p = cmd.Parameters.Add("?", OleDbType.VarWChar, size);
            p.Value = string.IsNullOrEmpty(val) ? DBNull.Value : val;
        }

        private static void AddMemo(OleDbCommand cmd, string? val)
        {
            var p = cmd.Parameters.Add("?", OleDbType.LongVarWChar);
            p.Value = string.IsNullOrEmpty(val) ? DBNull.Value : val;
        }

        private static void AddDate(OleDbCommand cmd, DateTime? dt)
        {
            var p = cmd.Parameters.Add("?", OleDbType.Date);
            p.Value = dt.HasValue ? dt.Value : DBNull.Value;
        }

        private static void AddCurrency(OleDbCommand cmd, decimal val)
        {
            var p = cmd.Parameters.Add("?", OleDbType.Currency);
            p.Value = val;
        }

        private static void AddDouble(OleDbCommand cmd, double val)
        {
            var p = cmd.Parameters.Add("?", OleDbType.Double);
            p.Value = val;
        }

        private static void AddInt(OleDbCommand cmd, int val)
        {
            var p = cmd.Parameters.Add("?", OleDbType.Integer);
            p.Value = val;
        }

        private static void AddLong(OleDbCommand cmd, long val)
        {
            var p = cmd.Parameters.Add("?", OleDbType.Integer);
            p.Value = (int)val;
        }

        // ==========================================
        // 1. Tax Account Configuration
        // ==========================================

        public async Task<TaxAccountConfigDto?> GetAccountConfigAsync(string taxCode)
        {
            using var conn = CreateConnection();
            await conn.OpenAsync().ConfigureAwait(false);

            const string sql = "SELECT Id, TaxCode, CompanyName, EncryptedPassword, Token, TokenExpiresAt, LastSyncDate, CreatedAt, UpdatedAt FROM TaxAccountConfig WHERE TaxCode = ?";
            using var cmd = new OleDbCommand(sql, conn);
            AddString(cmd, taxCode, 20);

            using var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false);
            if (await reader.ReadAsync().ConfigureAwait(false))
            {
                return new TaxAccountConfigDto
                {
                    Id = reader.GetInt32(0),
                    TaxCode = reader.GetString(1),
                    CompanyName = reader.IsDBNull(2) ? null : reader.GetString(2),
                    EncryptedPassword = reader.IsDBNull(3) ? null : reader.GetString(3),
                    Token = reader.IsDBNull(4) ? null : reader.GetString(4),
                    TokenExpiresAt = reader.IsDBNull(5) ? null : reader.GetDateTime(5),
                    LastSyncDate = reader.IsDBNull(6) ? null : reader.GetDateTime(6),
                    CreatedAt = reader.IsDBNull(7) ? null : reader.GetDateTime(7),
                    UpdatedAt = reader.IsDBNull(8) ? null : reader.GetDateTime(8)
                };
            }
            return null;
        }

        public async Task SaveAccountConfigAsync(TaxAccountConfigDto config)
        {
            using var conn = CreateConnection();
            await conn.OpenAsync().ConfigureAwait(false);

            const string checkSql = "SELECT Id FROM TaxAccountConfig WHERE TaxCode = ?";
            using var checkCmd = new OleDbCommand(checkSql, conn);
            AddString(checkCmd, config.TaxCode, 20);

            var existingId = await checkCmd.ExecuteScalarAsync().ConfigureAwait(false);
            if (existingId != null && existingId != DBNull.Value)
            {
                const string updateSql = @"UPDATE TaxAccountConfig SET 
                    CompanyName = ?, 
                    EncryptedPassword = ?, 
                    Token = ?, 
                    TokenExpiresAt = ?, 
                    LastSyncDate = ?, 
                    UpdatedAt = NOW() 
                    WHERE TaxCode = ?";
                using var cmd = new OleDbCommand(updateSql, conn);
                AddString(cmd, config.CompanyName, 255);
                AddMemo(cmd, config.EncryptedPassword);
                AddMemo(cmd, config.Token);
                AddDate(cmd, config.TokenExpiresAt);
                AddDate(cmd, config.LastSyncDate);
                AddString(cmd, config.TaxCode, 20);
                await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
            else
            {
                const string insertSql = @"INSERT INTO TaxAccountConfig 
                    (TaxCode, CompanyName, EncryptedPassword, Token, TokenExpiresAt, LastSyncDate, CreatedAt, UpdatedAt) 
                    VALUES (?, ?, ?, ?, ?, ?, NOW(), NOW())";
                using var cmd = new OleDbCommand(insertSql, conn);
                AddString(cmd, config.TaxCode, 20);
                AddString(cmd, config.CompanyName, 255);
                AddMemo(cmd, config.EncryptedPassword);
                AddMemo(cmd, config.Token);
                AddDate(cmd, config.TokenExpiresAt);
                AddDate(cmd, config.LastSyncDate);
                await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
        }

        // ==========================================
        // 2. Upsert Tax Invoices
        // ==========================================

        public async Task<long> UpsertInvoiceAsync(TaxInvoiceDto invoice)
        {
            using var conn = CreateConnection();
            await conn.OpenAsync().ConfigureAwait(false);

            const string checkSql = @"SELECT Id, VoucherId FROM TaxInvoice 
                WHERE SellerTaxCode = ? AND InvoiceTemplate = ? AND InvoiceSeries = ? AND InvoiceNo = ? AND Direction = ?";
            using var checkCmd = new OleDbCommand(checkSql, conn);
            AddString(checkCmd, invoice.SellerTaxCode, 20);
            AddString(checkCmd, invoice.InvoiceTemplate, 10);
            AddString(checkCmd, invoice.InvoiceSeries, 15);
            AddLong(checkCmd, invoice.InvoiceNo);
            AddString(checkCmd, invoice.Direction, 10);

            long invoiceId;

            using (var reader = await checkCmd.ExecuteReaderAsync().ConfigureAwait(false))
            {
                if (await reader.ReadAsync().ConfigureAwait(false))
                {
                    invoiceId = Convert.ToInt64(reader.GetValue(0));

                    const string updateSql = @"UPDATE TaxInvoice SET 
                        InvoiceDate = ?, 
                        SignedDate = ?, 
                        SellerName = ?, 
                        SellerAddress = ?, 
                        BuyerTaxCode = ?, 
                        BuyerName = ?, 
                        BuyerAddress = ?, 
                        CurrencyId = ?, 
                        ExchangeRate = ?, 
                        TotalAmountWithoutTax = ?, 
                        TotalTaxAmount = ?, 
                        TotalAmount = ?, 
                        TotalDiscountAmount = ?, 
                        InvoiceStatus = ?, 
                        ProcessStatus = ?, 
                        InvoiceSource = ?, 
                        TaxAuthorityCode = ?, 
                        RawJsonPath = ?, 
                        XmlFilePath = ?, 
                        DownloadedAt = NOW() 
                        WHERE Id = ?";

                    using var updateCmd = new OleDbCommand(updateSql, conn);
                    AddDate(updateCmd, invoice.InvoiceDate);
                    AddDate(updateCmd, invoice.SignedDate);
                    AddString(updateCmd, invoice.SellerName, 255);
                    AddMemo(updateCmd, invoice.SellerAddress);
                    AddString(updateCmd, invoice.BuyerTaxCode, 20);
                    AddString(updateCmd, invoice.BuyerName, 255);
                    AddMemo(updateCmd, invoice.BuyerAddress);
                    AddString(updateCmd, invoice.CurrencyId, 5);
                    AddDouble(updateCmd, invoice.ExchangeRate);
                    AddCurrency(updateCmd, invoice.TotalAmountWithoutTax);
                    AddCurrency(updateCmd, invoice.TotalTaxAmount);
                    AddCurrency(updateCmd, invoice.TotalAmount);
                    AddCurrency(updateCmd, invoice.TotalDiscountAmount);
                    AddInt(updateCmd, invoice.InvoiceStatus);
                    AddInt(updateCmd, invoice.ProcessStatus);
                    AddString(updateCmd, invoice.InvoiceSource, 10);
                    AddString(updateCmd, invoice.TaxAuthorityCode, 50);
                    AddMemo(updateCmd, invoice.RawJsonPath);
                    AddMemo(updateCmd, invoice.XmlFilePath);
                    AddLong(updateCmd, invoiceId);

                    await updateCmd.ExecuteNonQueryAsync().ConfigureAwait(false);

                    using var delRatesCmd = new OleDbCommand("DELETE FROM TaxInvoiceTaxRate WHERE TaxInvoiceId = ?", conn);
                    AddLong(delRatesCmd, invoiceId);
                    await delRatesCmd.ExecuteNonQueryAsync().ConfigureAwait(false);

                    using var delDetailsCmd = new OleDbCommand("DELETE FROM TaxInvoiceDetail WHERE TaxInvoiceId = ?", conn);
                    AddLong(delDetailsCmd, invoiceId);
                    await delDetailsCmd.ExecuteNonQueryAsync().ConfigureAwait(false);
                }
                else
                {
                    const string insertSql = @"INSERT INTO TaxInvoice 
                        (Direction, SellerTaxCode, InvoiceTemplate, InvoiceSeries, InvoiceNo, 
                         InvoiceDate, SignedDate, SellerName, SellerAddress, 
                         BuyerTaxCode, BuyerName, BuyerAddress, 
                         CurrencyId, ExchangeRate, TotalAmountWithoutTax, TotalTaxAmount, TotalAmount, TotalDiscountAmount, 
                         InvoiceStatus, ProcessStatus, InvoiceSource, TaxAuthorityCode, RawJsonPath, XmlFilePath, DownloadedAt) 
                        VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, NOW())";

                    using var insertCmd = new OleDbCommand(insertSql, conn);
                    AddString(insertCmd, invoice.Direction, 10);
                    AddString(insertCmd, invoice.SellerTaxCode, 20);
                    AddString(insertCmd, invoice.InvoiceTemplate, 10);
                    AddString(insertCmd, invoice.InvoiceSeries, 15);
                    AddLong(insertCmd, invoice.InvoiceNo);
                    AddDate(insertCmd, invoice.InvoiceDate);
                    AddDate(insertCmd, invoice.SignedDate);
                    AddString(insertCmd, invoice.SellerName, 255);
                    AddMemo(insertCmd, invoice.SellerAddress);
                    AddString(insertCmd, invoice.BuyerTaxCode, 20);
                    AddString(insertCmd, invoice.BuyerName, 255);
                    AddMemo(insertCmd, invoice.BuyerAddress);
                    AddString(insertCmd, invoice.CurrencyId, 5);
                    AddDouble(insertCmd, invoice.ExchangeRate);
                    AddCurrency(insertCmd, invoice.TotalAmountWithoutTax);
                    AddCurrency(insertCmd, invoice.TotalTaxAmount);
                    AddCurrency(insertCmd, invoice.TotalAmount);
                    AddCurrency(insertCmd, invoice.TotalDiscountAmount);
                    AddInt(insertCmd, invoice.InvoiceStatus);
                    AddInt(insertCmd, invoice.ProcessStatus);
                    AddString(insertCmd, invoice.InvoiceSource, 10);
                    AddString(insertCmd, invoice.TaxAuthorityCode, 50);
                    AddMemo(insertCmd, invoice.RawJsonPath);
                    AddMemo(insertCmd, invoice.XmlFilePath);

                    await insertCmd.ExecuteNonQueryAsync().ConfigureAwait(false);

                    using var identityCmd = new OleDbCommand("SELECT @@IDENTITY", conn);
                    invoiceId = Convert.ToInt64(await identityCmd.ExecuteScalarAsync().ConfigureAwait(false));
                }
            }

            // Insert tax rates
            if (invoice.TaxRates != null && invoice.TaxRates.Count > 0)
            {
                const string insertRateSql = @"INSERT INTO TaxInvoiceTaxRate (TaxInvoiceId, TaxRate, AmountWithoutTax, TaxAmount) VALUES (?, ?, ?, ?)";
                foreach (var tr in invoice.TaxRates)
                {
                    using var rateCmd = new OleDbCommand(insertRateSql, conn);
                    AddLong(rateCmd, invoiceId);
                    AddString(rateCmd, tr.TaxRate, 20);
                    AddCurrency(rateCmd, tr.AmountWithoutTax);
                    AddCurrency(rateCmd, tr.TaxAmount);
                    await rateCmd.ExecuteNonQueryAsync().ConfigureAwait(false);
                }
            }

            // Insert line items
            if (invoice.Details != null && invoice.Details.Count > 0)
            {
                const string insertDetailSql = @"INSERT INTO TaxInvoiceDetail 
                    (TaxInvoiceId, LineNo, ItemCode, ItemName, UnitName, Quantity, UnitPrice, Amount, TaxRate, TaxAmount) 
                    VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";
                foreach (var d in invoice.Details)
                {
                    using var detailCmd = new OleDbCommand(insertDetailSql, conn);
                    AddLong(detailCmd, invoiceId);
                    AddInt(detailCmd, d.LineNo);
                    AddString(detailCmd, d.ItemCode, 50);
                    AddString(detailCmd, d.ItemName, 255);
                    AddString(detailCmd, d.UnitName, 50);
                    AddDouble(detailCmd, d.Quantity);
                    AddCurrency(detailCmd, d.UnitPrice);
                    AddCurrency(detailCmd, d.Amount);
                    AddString(detailCmd, d.TaxRate, 20);
                    AddCurrency(detailCmd, d.TaxAmount);
                    await detailCmd.ExecuteNonQueryAsync().ConfigureAwait(false);
                }
            }

            invoice.Id = invoiceId;
            return invoiceId;
        }

        // ==========================================
        // 3. Query Invoices
        // ==========================================

        public async Task<List<TaxInvoiceDto>> GetInvoicesAsync(
            string? direction = null, 
            DateTime? fromDate = null, 
            DateTime? toDate = null, 
            string? keyword = null,
            int? status = null)
        {
            var list = new List<TaxInvoiceDto>();
            using var conn = CreateConnection();
            await conn.OpenAsync().ConfigureAwait(false);

            var sql = @"SELECT Id, Direction, SellerTaxCode, InvoiceTemplate, InvoiceSeries, InvoiceNo, 
                               InvoiceDate, SignedDate, SellerName, SellerAddress, 
                               BuyerTaxCode, BuyerName, BuyerAddress, 
                               CurrencyId, ExchangeRate, TotalAmountWithoutTax, TotalTaxAmount, TotalAmount, TotalDiscountAmount, 
                               InvoiceStatus, ProcessStatus, InvoiceSource, TaxAuthorityCode, RawJsonPath, XmlFilePath, 
                               DownloadedAt, VoucherId, ReconcileNote 
                        FROM TaxInvoice WHERE 1=1";

            var paramActions = new List<Action<OleDbCommand>>();

            if (!string.IsNullOrWhiteSpace(direction))
            {
                sql += " AND Direction = ?";
                var dir = direction.ToLowerInvariant();
                paramActions.Add(cmd => AddString(cmd, dir, 10));
            }

            if (fromDate.HasValue)
            {
                sql += " AND InvoiceDate >= ?";
                var dt = fromDate.Value.Date;
                paramActions.Add(cmd => AddDate(cmd, dt));
            }

            if (toDate.HasValue)
            {
                sql += " AND InvoiceDate <= ?";
                var dt = toDate.Value.Date.AddDays(1).AddSeconds(-1);
                paramActions.Add(cmd => AddDate(cmd, dt));
            }

            if (status.HasValue)
            {
                sql += " AND InvoiceStatus = ?";
                var st = status.Value;
                paramActions.Add(cmd => AddInt(cmd, st));
            }

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var kw = "%" + keyword.Trim() + "%";
                sql += " AND (SellerTaxCode LIKE ? OR BuyerTaxCode LIKE ? OR SellerName LIKE ? OR BuyerName LIKE ? OR InvoiceSeries LIKE ?)";
                paramActions.Add(cmd => AddString(cmd, kw, 255));
                paramActions.Add(cmd => AddString(cmd, kw, 255));
                paramActions.Add(cmd => AddString(cmd, kw, 255));
                paramActions.Add(cmd => AddString(cmd, kw, 255));
                paramActions.Add(cmd => AddString(cmd, kw, 255));
            }

            sql += " ORDER BY InvoiceDate DESC, InvoiceNo DESC";

            using var cmd = new OleDbCommand(sql, conn);
            foreach (var action in paramActions) action(cmd);

            using var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false);
            while (await reader.ReadAsync().ConfigureAwait(false))
            {
                list.Add(new TaxInvoiceDto
                {
                    Id = reader.GetInt32(0),
                    Direction = reader.GetString(1),
                    SellerTaxCode = reader.GetString(2),
                    InvoiceTemplate = reader.GetString(3),
                    InvoiceSeries = reader.GetString(4),
                    InvoiceNo = reader.GetInt32(5),
                    InvoiceDate = reader.GetDateTime(6),
                    SignedDate = reader.IsDBNull(7) ? null : reader.GetDateTime(7),
                    SellerName = reader.IsDBNull(8) ? null : reader.GetString(8),
                    SellerAddress = reader.IsDBNull(9) ? null : reader.GetString(9),
                    BuyerTaxCode = reader.IsDBNull(10) ? null : reader.GetString(10),
                    BuyerName = reader.IsDBNull(11) ? null : reader.GetString(11),
                    BuyerAddress = reader.IsDBNull(12) ? null : reader.GetString(12),
                    CurrencyId = reader.IsDBNull(13) ? "VND" : reader.GetString(13),
                    ExchangeRate = reader.IsDBNull(14) ? 1.0 : reader.GetDouble(14),
                    TotalAmountWithoutTax = reader.IsDBNull(15) ? 0 : reader.GetDecimal(15),
                    TotalTaxAmount = reader.IsDBNull(16) ? 0 : reader.GetDecimal(16),
                    TotalAmount = reader.IsDBNull(17) ? 0 : reader.GetDecimal(17),
                    TotalDiscountAmount = reader.IsDBNull(18) ? 0 : reader.GetDecimal(18),
                    InvoiceStatus = reader.IsDBNull(19) ? 0 : reader.GetInt32(19),
                    ProcessStatus = reader.IsDBNull(20) ? 0 : reader.GetInt32(20),
                    InvoiceSource = reader.IsDBNull(21) ? "query" : reader.GetString(21),
                    TaxAuthorityCode = reader.IsDBNull(22) ? null : reader.GetString(22),
                    RawJsonPath = reader.IsDBNull(23) ? null : reader.GetString(23),
                    XmlFilePath = reader.IsDBNull(24) ? null : reader.GetString(24),
                    DownloadedAt = reader.IsDBNull(25) ? DateTime.Now : reader.GetDateTime(25),
                    VoucherId = reader.IsDBNull(26) ? null : reader.GetString(26),
                    ReconcileNote = reader.IsDBNull(27) ? null : reader.GetString(27)
                });
            }

            return list;
        }

        public async Task LoadInvoiceDetailsAsync(TaxInvoiceDto invoice)
        {
            using var conn = CreateConnection();
            await conn.OpenAsync().ConfigureAwait(false);

            invoice.TaxRates.Clear();
            invoice.Details.Clear();

            // Load tax rates
            using (var cmdRate = new OleDbCommand("SELECT Id, TaxRate, AmountWithoutTax, TaxAmount FROM TaxInvoiceTaxRate WHERE TaxInvoiceId = ?", conn))
            {
                AddLong(cmdRate, invoice.Id);
                using var r = await cmdRate.ExecuteReaderAsync().ConfigureAwait(false);
                while (await r.ReadAsync().ConfigureAwait(false))
                {
                    invoice.TaxRates.Add(new TaxInvoiceTaxRateDto
                    {
                        Id = r.GetInt32(0),
                        TaxInvoiceId = invoice.Id,
                        TaxRate = r.GetString(1),
                        AmountWithoutTax = r.GetDecimal(2),
                        TaxAmount = r.GetDecimal(3)
                    });
                }
            }

            // Load detail items
            using (var cmdDetail = new OleDbCommand("SELECT Id, LineNo, ItemCode, ItemName, UnitName, Quantity, UnitPrice, Amount, TaxRate, TaxAmount FROM TaxInvoiceDetail WHERE TaxInvoiceId = ? ORDER BY LineNo", conn))
            {
                AddLong(cmdDetail, invoice.Id);
                using var r = await cmdDetail.ExecuteReaderAsync().ConfigureAwait(false);
                while (await r.ReadAsync().ConfigureAwait(false))
                {
                    invoice.Details.Add(new TaxInvoiceDetailDto
                    {
                        Id = r.GetInt32(0),
                        TaxInvoiceId = invoice.Id,
                        LineNo = r.IsDBNull(1) ? 0 : r.GetInt32(1),
                        ItemCode = r.IsDBNull(2) ? null : r.GetString(2),
                        ItemName = r.IsDBNull(3) ? null : r.GetString(3),
                        UnitName = r.IsDBNull(4) ? null : r.GetString(4),
                        Quantity = r.IsDBNull(5) ? 0 : r.GetDouble(5),
                        UnitPrice = r.IsDBNull(6) ? 0 : r.GetDecimal(6),
                        Amount = r.IsDBNull(7) ? 0 : r.GetDecimal(7),
                        TaxRate = r.IsDBNull(8) ? null : r.GetString(8),
                        TaxAmount = r.IsDBNull(9) ? 0 : r.GetDecimal(9)
                    });
                }
            }
        }

        // ==========================================
        // 4. Import from JSON Directory
        // ==========================================

        public async Task<int> ImportFromDirectoryAsync(string directoryPath, string direction = "purchase", IProgress<DownloadProgress>? progress = null, CancellationToken cancellationToken = default)
        {
            if (!Directory.Exists(directoryPath))
            {
                throw new DirectoryNotFoundException($"Directory not found: {directoryPath}");
            }

            var files = Directory.GetFiles(directoryPath, "*.json", SearchOption.AllDirectories);
            int importedCount = 0;
            int totalCount = files.Length;

            for (int i = 0; i < files.Length; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var filePath = files[i];
                var fileName = Path.GetFileName(filePath);

                if (fileName.Equals("index.json", StringComparison.OrdinalIgnoreCase) ||
                    fileName.Equals("_log.json", StringComparison.OrdinalIgnoreCase) ||
                    fileName.StartsWith("list_", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                try
                {
                    var jsonContent = await File.ReadAllTextAsync(filePath, cancellationToken).ConfigureAwait(false);
                    var xmlZipPath = Path.ChangeExtension(filePath, ".zip");
                    var dto = TaxInvoiceParser.ParseFromJson(jsonContent, direction, filePath, File.Exists(xmlZipPath) ? xmlZipPath : null);
                    if (dto != null)
                    {
                        await UpsertInvoiceAsync(dto).ConfigureAwait(false);
                        importedCount++;
                    }
                }
                catch { }

                progress?.Report(new DownloadProgress
                {
                    Stage = "import",
                    CurrentItem = fileName,
                    ProcessedCount = i + 1,
                    TotalCount = totalCount,
                    SucceededCount = importedCount
                });
            }

            return importedCount;
        }
    }
}
