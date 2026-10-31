using System;
using System.IO;
using System.Threading.Tasks;
using NewaccNet.Contracts;
using NewaccNet.Core.Staging;
using Xunit;

namespace NewaccNet.ArchitectureTests
{
    public class DraftStoreTests
    {
        [Fact]
        public async Task JsonFileDraftStore_Save_Get_Query_Delete_ShouldWorkCorrectly()
        {
            string tempFolder = Path.Combine(Path.GetTempPath(), "NewaccNet_Test_Staging_" + Guid.NewGuid().ToString("N"));
            try
            {
                var store = new JsonFileDraftStore(tempFolder);

                var draft = new VoucherDraft
                {
                    DraftId = "TEST_DRAFT_001",
                    DraftType = DraftType.PurchaseInvoice,
                    Status = DraftStatus.Imported,
                    Source = new DraftSourceInfo { Kind = "TaxInvoice", Ref = "INV_999" },
                    Header = new DraftHeader { VoucherDate = DateTime.Today, VoucherNo = "NM0001" },
                    Counterparty = new DraftCounterparty
                    {
                        Raw = new CounterpartyRaw { Name = "Công ty TNHH Thử Nghiệm", TaxCode = "0109999999" }
                    }
                };

                // 1. Lưu
                await store.SaveAsync(draft);

                // 2. Lấy lại theo ID
                var fetched = await store.GetAsync("TEST_DRAFT_001");
                Assert.NotNull(fetched);
                Assert.Equal("TEST_DRAFT_001", fetched.DraftId);
                Assert.Equal("Công ty TNHH Thử Nghiệm", fetched.Counterparty.Raw.Name);

                // 3. Truy vấn theo SearchText
                var queryResult = await store.QueryAsync(new DraftQueryFilter { SearchText = "0109999999" });
                Assert.Single(queryResult);

                // 4. Kiểm tra tồn tại qua SourceRef
                bool exists = await store.ExistsBySourceRefAsync("TaxInvoice", "INV_999");
                Assert.True(exists);

                // 5. Xóa
                await store.DeleteAsync("TEST_DRAFT_001");
                var afterDelete = await store.GetAsync("TEST_DRAFT_001");
                Assert.Null(afterDelete);
            }
            finally
            {
                if (Directory.Exists(tempFolder))
                {
                    try { Directory.Delete(tempFolder, true); } catch { }
                }
            }
        }

        [Fact]
        public void DraftStoreFactory_ModeConfiguration_ShouldReturnExpectedStoreType()
        {
            // Mặc định là JsonFile
            DraftStoreFactory.ConfiguredMode = StagingStorageMode.JsonFile;
            var jsonStore = DraftStoreFactory.Create();
            Assert.IsType<JsonFileDraftStore>(jsonStore);

            // Gạt công tắc sang Database
            DraftStoreFactory.ConfiguredMode = StagingStorageMode.Database;
            var dbStore = DraftStoreFactory.Create();
            Assert.IsType<DbDraftStore>(dbStore);

            // Override trực tiếp qua tham số
            var explicitJson = DraftStoreFactory.Create(StagingStorageMode.JsonFile);
            Assert.IsType<JsonFileDraftStore>(explicitJson);

            // Trả lại mặc định
            DraftStoreFactory.ConfiguredMode = StagingStorageMode.JsonFile;
        }

        [Fact]
        public void ConfigManager_StagingStorageMode_DefaultsToJsonFile_AndCanBeConfigured()
        {
            var config = new NewaccNet.Core.Data.AppConfig();
            Assert.Equal(StagingStorageMode.JsonFile, config.StagingStorageMode);

            config.StagingStorageMode = StagingStorageMode.Database;
            Assert.Equal(StagingStorageMode.Database, config.StagingStorageMode);
        }

        [Fact]
        public void DraftMigrationService_CanBeInstantiated()
        {
            var migration = new DraftMigrationService();
            Assert.NotNull(migration);
        }

        [Fact]
        public async Task StagingDocument_SchemaLess_BankStatement_ShouldRoundTripCorrectly()
        {
            string tempFolder = Path.Combine(Path.GetTempPath(), "NewaccNet_Test_BankDraft_" + Guid.NewGuid().ToString("N"));
            try
            {
                var store = new JsonFileDraftStore(tempFolder);

                var bankDraft = new BankStatementDraft
                {
                    BankName = "Vietcombank",
                    AccountNumber = "001100223344",
                    OpeningBalance = 50_000_000,
                    ClosingBalance = 75_000_000,
                    Transactions = new()
                    {
                        new BankStatementLine
                        {
                            LineNo = 1,
                            TransactionDate = DateTime.Today,
                            RefNo = "FT2401010001",
                            DebitAmount = 0,
                            CreditAmount = 25_000_000,
                            BalanceAfter = 75_000_000,
                            Description = "Công ty ABC thanh toán tiền hàng đợt 1",
                            RawCounterpartyName = "CONG TY TNHH ABC",
                            ResolvedPartnerId = "KH_ABC",
                            ResolvedAccountId = "131"
                        }
                    }
                };

                var doc = StagingDocument.FromPayload(
                    draftId: "BANK_DOC_001",
                    draftType: "BankStatement",
                    payload: bankDraft,
                    status: DraftStatus.Imported,
                    sourceKind: "BankXml",
                    sourceRef: "VCB_20261031.xml",
                    title: bankDraft.GetDisplayTitle()
                );

                // Lưu StagingDocument
                await store.SaveDocumentAsync(doc);

                // Lấy lại
                var fetched = await store.GetDocumentAsync("BANK_DOC_001");
                Assert.NotNull(fetched);
                Assert.Equal("BankStatement", fetched.DraftType);

                // Ánh xạ ra DTO phân cấp
                var parsedDto = fetched.As<BankStatementDraft>();
                Assert.NotNull(parsedDto);
                Assert.Equal("Vietcombank", parsedDto.BankName);
                Assert.Equal("001100223344", parsedDto.AccountNumber);
                Assert.Single(parsedDto.Transactions);
                Assert.Equal(25_000_000, parsedDto.Transactions[0].CreditAmount);
                Assert.Equal("131", parsedDto.Transactions[0].ResolvedAccountId);
            }
            finally
            {
                if (Directory.Exists(tempFolder))
                {
                    try { Directory.Delete(tempFolder, true); } catch { }
                }
            }
        }

        [Fact]
        public void StagingDocument_InventoryDraft_DynamicPayload_ShouldDeserializeViaRegistry()
        {
            var invDraft = new InventoryDraft
            {
                WarehouseId = "KHO_TONG",
                WarehouseName = "Kho Tổng Hà Nội",
                VoucherDate = DateTime.Today,
                MovementType = "KK",
                Reason = "Kiểm kê định kỳ cuối tháng",
                Items = new()
                {
                    new InventoryDraftItem
                    {
                        LineNo = 1,
                        RawItemCode = "VT01",
                        RawItemName = "Thép cuộn 10mm",
                        RawUnit = "Kg",
                        BookQuantity = 100,
                        ActualQuantity = 98,
                        UnitPrice = 15000,
                        TotalAmount = 1470000,
                        ResolvedMaterialId = "THEP_CUON_10",
                        ResolvedAccountId = "1521"
                    }
                }
            };

            var doc = StagingDocument.FromPayload("INV_001", "InventoryMovement", invDraft);
            Assert.Equal("InventoryMovement", doc.DraftType);

            // Phân giải đa hình qua DraftPayloadRegistry
            var resolvedObj = DraftPayloadRegistry.Deserialize(doc.DraftType, doc.PayloadJson);
            Assert.NotNull(resolvedObj);
            Assert.IsType<InventoryDraft>(resolvedObj);

            var typedInv = (InventoryDraft)resolvedObj;
            Assert.Equal("KHO_TONG", typedInv.WarehouseId);
            Assert.Equal(-2, typedInv.Items[0].DiffQuantity);
        }

        [Fact]
        public void StagingDocument_PayrollDraft_CustomStructure_ShouldRoundTripCorrectly()
        {
            var payroll = new PayrollDraft
            {
                PeriodMonth = 10,
                PeriodYear = 2026,
                Department = "Phòng Kỹ thuật",
                TotalGrossSalary = 30_000_000,
                TotalInsurance = 3_150_000,
                TotalTax = 1_000_000,
                TotalNetPay = 25_850_000,
                Employees = new()
                {
                    new PayrollEmployeeLine
                    {
                        LineNo = 1,
                        EmployeeCode = "NV01",
                        FullName = "Nguyễn Văn A",
                        BaseSalary = 25_000_000,
                        GrossSalary = 30_000_000,
                        PersonalIncomeTax = 1_000_000,
                        NetPay = 25_850_000,
                        CostAccountId = "6421",
                        PayableAccountId = "3341"
                    }
                }
            };

            var doc = StagingDocument.FromPayload("PAY_202610", "Payroll", payroll);
            var parsedPayroll = doc.As<PayrollDraft>();

            Assert.NotNull(parsedPayroll);
            Assert.Equal(10, parsedPayroll.PeriodMonth);
            Assert.Equal(2026, parsedPayroll.PeriodYear);
            Assert.Single(parsedPayroll.Employees);
            Assert.Equal("Nguyễn Văn A", parsedPayroll.Employees[0].FullName);
            Assert.Equal(25_850_000, parsedPayroll.Employees[0].NetPay);
        }
    }
}
