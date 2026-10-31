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
    }
}
