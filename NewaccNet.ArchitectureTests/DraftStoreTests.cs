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

        [Fact]
        public void MasterDataDraft_OneTier_MatchingAndAiProposal_ShouldWork()
        {
            var masterDraft = new MasterDataDraft
            {
                Kind = "Partner",
                SourceRef = "HoaDonTCT_Thang10.xml",
                Items = new()
                {
                    new MasterDataDraftItem
                    {
                        LineNo = 1,
                        RawTaxCode = "0101234567",
                        RawName = "Công ty TNHH Thép Việt Nhật",
                        RawAddress = "KCN Thăng Long, Hà Nội",
                        ExistsInDb = false,
                        ProposedCode = "NCC_THEPVN",
                        ProposedName = "Thép Việt Nhật",
                        ProposedGroup = "NHA_CUNG_CAP",
                        ProposedDefaultAccount = "3311",
                        Match = new MatchInfo
                        {
                            Field = "TaxCode",
                            Method = MatchMethod.Ai,
                            Confidence = 0.95,
                            Reason = "AI đề xuất mã NCC dựa trên tên công ty và ngành hàng thép"
                        },
                        IsApproved = true
                    }
                }
            };

            var doc = StagingDocument.FromPayload("MD_001", "MasterData", masterDraft);
            var parsed = doc.As<MasterDataDraft>();

            Assert.NotNull(parsed);
            Assert.Equal("Partner", parsed.Kind);
            Assert.Single(parsed.Items);
            Assert.Equal("0101234567", parsed.Items[0].RawTaxCode);
            Assert.Equal("NCC_THEPVN", parsed.Items[0].ProposedCode);
            Assert.Equal(MatchMethod.Ai, parsed.Items[0].Match.Method);
        }

        [Fact]
        public void ThreeTierVoucherTransformer_FromStage2_ToStage3_DebtDetails_ShouldTransformCorrectly()
        {
            var stage2Draft = new VoucherDraft
            {
                DraftId = "STAGE2_001",
                DraftType = DraftType.PurchaseInvoice,
                Status = DraftStatus.Approved,
                Header = new DraftHeader
                {
                    VoucherDate = new DateTime(2026, 10, 31),
                    VoucherSourceCode = "NM",
                    VoucherNo = "NM0001",
                    Contents = "Nhập mua nguyên vật liệu",
                    PersonName = "Anh Tuấn"
                },
                Counterparty = new DraftCounterparty
                {
                    Raw = new CounterpartyRaw
                    {
                        Name = "Công ty TNHH Xi Măng Hà Tiên",
                        TaxCode = "0300445566",
                        Address = "TP. Hồ Chí Minh"
                    },
                    Resolved = new CounterpartyResolved
                    {
                        PartnerId = "NCC_HATIEN",
                        IsNew = false
                    }
                },
                Invoice = new DraftInvoiceInfo
                {
                    Number = "0001234",
                    Series = "1C26TBB",
                    Date = new DateTime(2026, 10, 30)
                },
                Settlement = new DraftSettlement
                {
                    CreditAccountId = "3311",
                    DueDate = new DateTime(2026, 11, 30)
                },
                Lines = new()
                {
                    new DraftLine
                    {
                        LineNo = 1,
                        Raw = new DraftLineRaw
                        {
                            ItemName = "Xi măng PCB40",
                            Unit = "Tấn",
                            Quantity = 10,
                            UnitPrice = 1500000,
                            Amount = 15000000,
                            VatRate = 10,
                            VatAmount = 1500000
                        },
                        Resolved = new DraftLineResolved
                        {
                            MaterialId = "XM_PCB40",
                            WarehouseId = "KHO_CHINH",
                            DebitAccountId = "1521",
                            VatAccountId = "13311"
                        }
                    }
                }
            };

            // Chuyển hóa sang DTO 3 tầng (Vòng đời 3)
            var threeTier = NewaccNet.Core.Posting.ThreeTierVoucherTransformer.Transform(stage2Draft);

            Assert.NotNull(threeTier);
            Assert.Equal(DraftLifecycleStage.Stage3_VoucherReady, threeTier.Stage);

            // Tầng Top
            Assert.Equal("NM", threeTier.TopVoucher.VoucherSourceCode);
            Assert.Equal("NCC_HATIEN", threeTier.TopVoucher.PartnerId);
            Assert.Equal("0300445566", threeTier.TopVoucher.TaxCode);
            Assert.True(threeTier.TopVoucher.IsDebtTransaction);
            Assert.Equal(16500000, threeTier.TopVoucher.TotalAmount);

            // Tầng Giữa (Phiếu kho)
            Assert.NotNull(threeTier.InventoryVoucher);
            Assert.Equal("NK", threeTier.InventoryVoucher.MovementType);
            Assert.Equal("KHO_CHINH", threeTier.InventoryVoucher.DefaultWarehouseId);

            // Tầng Dưới (Chi tiết định khoản & gắn công nợ)
            Assert.Single(threeTier.Details);
            var line = threeTier.Details[0];
            Assert.Equal("XM_PCB40", line.MaterialId);
            Assert.Equal("1521", line.DebitAccountId);
            Assert.Equal("3311", line.CreditAccountId);
            // Giao dịch nợ nên MST và PartnerId được đưa vào chi tiết để lên sổ 331
            Assert.Equal("NCC_HATIEN", line.DebtPartnerId);
            Assert.Equal("0300445566", line.DebtPartnerTaxCode);
        }

        [Fact]
        public async Task ThreeTierDraft_LoadAndResumeCheckpoint_RoundTrip()
        {
            string tempFolder = Path.Combine(Path.GetTempPath(), "NewaccNet_Test_ThreeTier_" + Guid.NewGuid().ToString("N"));
            try
            {
                var store = new JsonFileDraftStore(tempFolder);

                var threeTier = new AccountingVoucherThreeTierDraft
                {
                    Stage = DraftLifecycleStage.Stage3_VoucherReady,
                    TopVoucher = new TopVoucherTier
                    {
                        VoucherDate = DateTime.Today,
                        VoucherSourceCode = "NM",
                        VoucherNo = "NM_PAUSE_01",
                        PartnerId = "NCC_01",
                        PartnerName = "Công ty Tạm Dừng Để Mai Làm Tiếp",
                        TaxCode = "0109998888",
                        IsDebtTransaction = true,
                        TotalAmount = 5000000
                    },
                    InventoryVoucher = new MiddleInventoryTier
                    {
                        MovementType = "NK",
                        DefaultWarehouseId = "KHO_A"
                    },
                    Details = new()
                    {
                        new DetailLineTier
                        {
                            LineNo = 1,
                            MaterialId = "VT_01",
                            Quantity = 5,
                            UnitPrice = 1000000,
                            Amount = 5000000,
                            DebitAccountId = "1561",
                            CreditAccountId = "3311",
                            DebtPartnerId = "NCC_01"
                        }
                    }
                };

                // Lưu bản nháp Vòng 3 ở trạng thái chờ
                var doc = StagingDocument.FromPayload("DRAFT_V3_001", "AccountingVoucher3Tier", threeTier);
                await store.SaveDocumentAsync(doc);

                // Lần sau mở máy lên: load lại đúng điểm dừng để hiệu chỉnh tiếp
                var loadedDoc = await store.GetDocumentAsync("DRAFT_V3_001");
                Assert.NotNull(loadedDoc);

                var resumedTier = loadedDoc.As<AccountingVoucherThreeTierDraft>();
                Assert.NotNull(resumedTier);
                Assert.Equal(DraftLifecycleStage.Stage3_VoucherReady, resumedTier.Stage);
                Assert.Equal("NM_PAUSE_01", resumedTier.TopVoucher.VoucherNo);
                Assert.NotNull(resumedTier.InventoryVoucher);
                Assert.Single(resumedTier.Details);
                Assert.Equal("VT_01", resumedTier.Details[0].MaterialId);
                Assert.Equal("NCC_01", resumedTier.Details[0].DebtPartnerId);
            }
            finally
            {
                if (Directory.Exists(tempFolder))
                {
                    try { Directory.Delete(tempFolder, true); } catch { }
                }
            }
        }
    }
}
