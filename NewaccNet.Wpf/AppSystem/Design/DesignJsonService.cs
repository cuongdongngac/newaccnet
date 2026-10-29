using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Windows;
using Microsoft.Win32;
using DataAccess.EntityClasses;
using DataAccess.HelperClasses;
using SD.LLBLGen.Pro.ORMSupportClasses;

namespace NewaccNet.Wpf.AppSystem.Design
{
    #region DTOs
    // ==========================================
    // 1. Balance Sheet DTOs (B01-DN)
    // ==========================================
    public class BalanceSheetDesignDto
    {
        public string ReportTitle { get; set; } = "Bảng Cân Đối Kế Toán (Mẫu B01-DN)";
        public string Version { get; set; } = "1.0";
        public DateTime ExportedAt { get; set; } = DateTime.Now;
        public List<ReportSectionExportDto> Sections { get; set; } = new List<ReportSectionExportDto>();
    }

    public class ReportSectionExportDto
    {
        public string SectionName { get; set; } = "";
        public bool Indebit { get; set; }
        public List<ReportCategoryExportDto> Categories { get; set; } = new List<ReportCategoryExportDto>();
    }

    public class ReportCategoryExportDto
    {
        public string Code { get; set; } = "";
        public string CategoryName { get; set; } = "";
        public List<ReportItemExportDto> Items { get; set; } = new List<ReportItemExportDto>();
    }

    public class ReportItemExportDto
    {
        public string Code { get; set; } = "";
        public string EntryName { get; set; } = "";
        public List<ReportFormulaExportDto> Formulas { get; set; } = new List<ReportFormulaExportDto>();
    }

    public class ReportFormulaExportDto
    {
        public string Code { get; set; } = "";
        public string ItemsName { get; set; } = "";
        public string Formula { get; set; } = "";
        public bool Calculate { get; set; }
        public string Illu { get; set; } = "";
        public int? Longorshortterm { get; set; }
        public int Order { get; set; }
        public bool Print { get; set; }
        public List<string> Accounts { get; set; } = new List<string>();
    }

    // ==========================================
    // 2. Business Result DTOs (B02-DN)
    // ==========================================
    public class BusinessResultDesignDto
    {
        public string ReportTitle { get; set; } = "Báo Cáo Kết Quả Kinh Doanh (Mẫu B02-DN)";
        public string Version { get; set; } = "1.0";
        public DateTime ExportedAt { get; set; } = DateTime.Now;
        public List<BusinessResultItemExportDto> Items { get; set; } = new List<BusinessResultItemExportDto>();
    }

    public class BusinessResultItemExportDto
    {
        public string Code { get; set; } = "";
        public string ItemName { get; set; } = "";
        public string Illustration { get; set; } = "";
        public bool IsDecrease { get; set; }
        public bool IsBold { get; set; }
        public bool IsItalic { get; set; }
        public List<AccountBusinessResultExportDto> Accounts { get; set; } = new List<AccountBusinessResultExportDto>();
    }

    public class AccountBusinessResultExportDto
    {
        public string AccountId { get; set; } = "";
        public string CounterAccountId { get; set; } = "";
        public bool IsAdd { get; set; } = true;
        public short? Method { get; set; }
    }

    // ==========================================
    // 3. Cash Flow DTOs (B03-DN)
    // ==========================================
    public class CashFlowDesignDto
    {
        public string ReportTitle { get; set; } = "Báo Cáo Lưu Chuyển Tiền Tệ (Mẫu B03-DN)";
        public string Version { get; set; } = "1.0";
        public DateTime ExportedAt { get; set; } = DateTime.Now;
        public List<CashFlowCategoryExportDto> Categories { get; set; } = new List<CashFlowCategoryExportDto>();
    }

    public class CashFlowCategoryExportDto
    {
        public string CashFlowName { get; set; } = "";
        public string CashFlowNameRepeate { get; set; } = "";
        public List<CashFlowItemExportDto> Items { get; set; } = new List<CashFlowItemExportDto>();
    }

    public class CashFlowItemExportDto
    {
        public string Code { get; set; } = "";
        public string ItemName { get; set; } = "";
        public string Illustration { get; set; } = "";
        public bool Dbcr { get; set; } = true;
        public bool IsBold { get; set; }
        public bool IsItalic { get; set; }
        public List<AccountCashFlowItemExportDto> Accounts { get; set; } = new List<AccountCashFlowItemExportDto>();
    }

    public class AccountCashFlowItemExportDto
    {
        public string AccountId { get; set; } = "";
        public string CounterAccountId { get; set; } = "";
        public bool Dbcr { get; set; } = true;
    }

    // ==========================================
    // 4. Tax Obligation DTOs
    // ==========================================
    public class TaxObligationDesignDto
    {
        public string ReportTitle { get; set; } = "Báo Cáo Tình Hình Nghĩa Vụ Thuế Với NSNN";
        public string Version { get; set; } = "1.0";
        public DateTime ExportedAt { get; set; } = DateTime.Now;
        public List<TaxObligationExportDto> Obligations { get; set; } = new List<TaxObligationExportDto>();
    }

    public class TaxObligationExportDto
    {
        public string ObligationName { get; set; } = "";
        public List<TaxObligationItemExportDto> Items { get; set; } = new List<TaxObligationItemExportDto>();
    }

    public class TaxObligationItemExportDto
    {
        public string Code { get; set; } = "";
        public string ItemName { get; set; } = "";
        public List<AccountTaxObligationExportDto> Accounts { get; set; } = new List<AccountTaxObligationExportDto>();
    }

    public class AccountTaxObligationExportDto
    {
        public string AccountId { get; set; } = "";
        public string CounterAccountId { get; set; } = "";
        public int? ColIndex { get; set; }
        public bool IsAdd { get; set; } = true;
        public short? Method { get; set; }
    }

    // ==========================================
    // 5. Vat Declare DTOs (Mẫu 01/GTGT)
    // ==========================================
    public class VatDeclareDesignDto
    {
        public string ReportTitle { get; set; } = "Tờ Khai Thuế GTGT (Mẫu 01/GTGT)";
        public string Version { get; set; } = "1.0";
        public DateTime ExportedAt { get; set; } = DateTime.Now;
        public List<VatDeclareItemExportDto> Items { get; set; } = new List<VatDeclareItemExportDto>();
    }

    public class VatDeclareItemExportDto
    {
        public string Code { get; set; } = "";
        public string ItemName { get; set; } = "";
        public bool IsBold { get; set; }
        public bool IsItalic { get; set; }
        public List<AccountVatDeclareItemExportDto> Accounts { get; set; } = new List<AccountVatDeclareItemExportDto>();
    }

    public class AccountVatDeclareItemExportDto
    {
        public string AccountId { get; set; } = "";
        public string CounterAccountId { get; set; } = "";
        public short? ColumnNo { get; set; }
        public bool IsAdd { get; set; } = true;
        public short? Method { get; set; }
    }

    // ==========================================
    // 6. Vat Return DTOs (Mẫu 01-3/GTGT)
    // ==========================================
    public class VatReturnDesignDto
    {
        public string ReportTitle { get; set; } = "Bảng Kê Đề Nghị Hoàn Thuế GTGT (Mẫu 01-3/GTGT)";
        public string Version { get; set; } = "1.0";
        public DateTime ExportedAt { get; set; } = DateTime.Now;
        public List<VatReturnItemExportDto> Items { get; set; } = new List<VatReturnItemExportDto>();
    }

    public class VatReturnItemExportDto
    {
        public string Code { get; set; } = "";
        public string ItemName { get; set; } = "";
        public bool IsBold { get; set; }
        public bool IsItalic { get; set; }
        public List<AccountVatReturnItemExportDto> Accounts { get; set; } = new List<AccountVatReturnItemExportDto>();
    }

    public class AccountVatReturnItemExportDto
    {
        public string AccountId { get; set; } = "";
        public string CounterAccountId { get; set; } = "";
        public bool IsAdd { get; set; } = true;
        public short? Method { get; set; }
    }
    #endregion

    /// <summary>
    /// Dịch vụ Xuất & Nhập cấu hình mẫu cho 6 form Thiết Kế Báo Cáo Tài Chính và Thuế.
    /// Xử lý triệt để bài toán Khóa chính Autonumber/Identity:
    /// - Khi xuất: Chuyển toàn bộ mô hình thành DTO phân cấp (Object Graph Hierarchy).
    /// - Khi nhập: Xóa sạch (Empty) dữ liệu cũ trong transaction, tái tạo entity với Id = 0 (IsNew = true).
    /// - Dùng adapter.SaveEntity(root, recurse: true) để CSDL tự sinh Id mới và ORM tự nối khóa ngoại hoàn hảo.
    /// </summary>
    public static class DesignJsonService
    {
        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            PropertyNameCaseInsensitive = true
        };

        private static bool ConfirmImport(string reportName)
        {
            var result = MessageBox.Show(
                $"Hành động này sẽ XÓA TOÀN BỘ dữ liệu cấu hình hiện tại của '{reportName}' và thay thế bằng mẫu mới từ tệp JSON.\n\nBạn có chắc chắn muốn tiếp tục không?",
                "Xác nhận nạp mẫu thiết kế từ JSON",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);
            return result == MessageBoxResult.Yes;
        }

        #region 1. Balance Sheet
        public static void ExportBalanceSheet(Window owner)
        {
            try
            {
                using var adapter = AppDataAccessAdapter.Create();
                var list = new EntityCollection<ReportSectionEntity>();
                var prefetch = new PrefetchPath2((int)DataAccess.EntityType.ReportSectionEntity);
                prefetch.Add(ReportSectionEntity.PrefetchPathReportCategories)
                        .SubPath.Add(ReportCategoryEntity.PrefetchPathReportItems)
                        .SubPath.Add(ReportItemEntity.PrefetchPathReportFormulas)
                        .SubPath.Add(ReportFormulaEntity.PrefetchPathReportFormulaAccounts);
                adapter.FetchEntityCollection(list, null, prefetch);

                var dto = new BalanceSheetDesignDto();
                foreach (var sec in list)
                {
                    var sDto = new ReportSectionExportDto
                    {
                        SectionName = sec.SectionName ?? "",
                        Indebit = sec.Indebit
                    };
                    if (sec.ReportCategories != null)
                    {
                        foreach (var cat in sec.ReportCategories)
                        {
                            var cDto = new ReportCategoryExportDto
                            {
                                Code = cat.Code ?? "",
                                CategoryName = cat.CategoryName ?? ""
                            };
                            if (cat.ReportItems != null)
                            {
                                foreach (var item in cat.ReportItems)
                                {
                                    var iDto = new ReportItemExportDto
                                    {
                                        Code = item.Code ?? "",
                                        EntryName = item.EntryName ?? ""
                                    };
                                    if (item.ReportFormulas != null)
                                    {
                                        foreach (var f in item.ReportFormulas)
                                        {
                                            var fDto = new ReportFormulaExportDto
                                            {
                                                Code = f.Code ?? "",
                                                ItemsName = f.ItemsName ?? "",
                                                Formula = f.Formula ?? "",
                                                Calculate = f.Calculate,
                                                Illu = f.Illu ?? "",
                                                Longorshortterm = f.Longorshortterm,
                                                Order = f.Order,
                                                Print = f.Print
                                            };
                                            if (f.ReportFormulaAccounts != null)
                                            {
                                                foreach (var acc in f.ReportFormulaAccounts)
                                                {
                                                    if (!string.IsNullOrWhiteSpace(acc.AccountId))
                                                        fDto.Accounts.Add(acc.AccountId.Trim());
                                                }
                                            }
                                            iDto.Formulas.Add(fDto);
                                        }
                                    }
                                    cDto.Items.Add(iDto);
                                }
                            }
                            sDto.Categories.Add(cDto);
                        }
                    }
                    dto.Sections.Add(sDto);
                }

                var dlg = new SaveFileDialog
                {
                    Title = "Xuất cấu hình Bảng Cân Đối Kế Toán ra JSON",
                    Filter = "JSON Files (*.json)|*.json|All Files (*.*)|*.*",
                    FileName = "BalanceSheet_Design.json"
                };

                if (dlg.ShowDialog(owner) == true)
                {
                    string json = JsonSerializer.Serialize(dto, _jsonOptions);
                    File.WriteAllText(dlg.FileName, json, System.Text.Encoding.UTF8);
                    MessageBox.Show(owner, "Đã xuất cấu hình ra tệp JSON thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(owner, "Lỗi khi xuất JSON: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public static bool ImportBalanceSheet(Window owner)
        {
            var dlg = new OpenFileDialog
            {
                Title = "Chọn tệp JSON cấu hình Bảng Cân Đối Kế Toán",
                Filter = "JSON Files (*.json)|*.json|All Files (*.*)|*.*",
                FileName = "BalanceSheet_Design.json"
            };

            if (dlg.ShowDialog(owner) != true) return false;

            if (!ConfirmImport("Bảng Cân Đối Kế Toán")) return false;

            try
            {
                string json = File.ReadAllText(dlg.FileName);
                var dto = JsonSerializer.Deserialize<BalanceSheetDesignDto>(json, _jsonOptions);
                if (dto == null || dto.Sections == null || dto.Sections.Count == 0)
                {
                    MessageBox.Show(owner, "Tệp JSON không chứa dữ liệu hợp lệ!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }

                using var adapter = AppDataAccessAdapter.Create();
                adapter.StartTransaction(System.Data.IsolationLevel.ReadCommitted, "Import_BalanceSheet");

                try
                {
                    // 1) Xóa sạch (Empty) các bảng từ con lên cha
                    adapter.DeleteEntitiesDirectly(typeof(ReportFormulaAccountEntity), null);
                    adapter.DeleteEntitiesDirectly(typeof(ReportFormulaEntity), null);
                    adapter.DeleteEntitiesDirectly(typeof(ReportItemEntity), null);
                    adapter.DeleteEntitiesDirectly(typeof(ReportCategoryEntity), null);
                    adapter.DeleteEntitiesDirectly(typeof(ReportSectionEntity), null);

                    // 2) Tái tạo Entity graph mới tinh
                    foreach (var sDto in dto.Sections)
                    {
                        var sec = new ReportSectionEntity
                        {
                            SectionName = sDto.SectionName ?? "",
                            Indebit = sDto.Indebit
                        };

                        if (sDto.Categories != null)
                        {
                            foreach (var cDto in sDto.Categories)
                            {
                                var cat = new ReportCategoryEntity
                                {
                                    Code = cDto.Code ?? "",
                                    CategoryName = cDto.CategoryName ?? ""
                                };

                                if (cDto.Items != null)
                                {
                                    foreach (var iDto in cDto.Items)
                                    {
                                        var item = new ReportItemEntity
                                        {
                                            Code = iDto.Code ?? "",
                                            EntryName = iDto.EntryName ?? ""
                                        };

                                        if (iDto.Formulas != null)
                                        {
                                            foreach (var fDto in iDto.Formulas)
                                            {
                                                var formula = new ReportFormulaEntity
                                                {
                                                    Code = fDto.Code ?? "",
                                                    ItemsName = fDto.ItemsName ?? "",
                                                    Formula = fDto.Formula ?? "",
                                                    Calculate = fDto.Calculate,
                                                    Illu = fDto.Illu ?? "",
                                                    Longorshortterm = fDto.Longorshortterm,
                                                    Order = fDto.Order,
                                                    Print = fDto.Print
                                                };

                                                if (fDto.Accounts != null)
                                                {
                                                    foreach (var accId in fDto.Accounts)
                                                    {
                                                        if (!string.IsNullOrWhiteSpace(accId))
                                                        {
                                                            formula.ReportFormulaAccounts.Add(new ReportFormulaAccountEntity
                                                            {
                                                                AccountId = accId.Trim()
                                                            });
                                                        }
                                                    }
                                                }

                                                item.ReportFormulas.Add(formula);
                                            }
                                        }

                                        cat.ReportItems.Add(item);
                                    }
                                }

                                sec.ReportCategories.Add(cat);
                            }
                        }

                        // CSDL tự sinh Id và ORM tự động map lại toàn bộ FK cho các tầng con
                        adapter.SaveEntity(sec, refetchAfterSave: true, recurse: true);
                    }

                    adapter.Commit();
                    MessageBox.Show(owner, "Đã nạp mẫu Bảng Cân Đối Kế Toán từ JSON thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    return true;
                }
                catch
                {
                    adapter.Rollback();
                    throw;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(owner, "Lỗi khi nạp dữ liệu từ JSON: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
        #endregion

        #region 2. Business Result
        public static void ExportBusinessResult(Window owner)
        {
            try
            {
                using var adapter = AppDataAccessAdapter.Create();
                var list = new EntityCollection<BusinessResultItemEntity>();
                var prefetch = new PrefetchPath2((int)DataAccess.EntityType.BusinessResultItemEntity);
                prefetch.Add(BusinessResultItemEntity.PrefetchPathAccountBusinessResults);
                adapter.FetchEntityCollection(list, null, prefetch);

                var dto = new BusinessResultDesignDto();
                foreach (var item in list)
                {
                    var iDto = new BusinessResultItemExportDto
                    {
                        Code = item.Code ?? "",
                        ItemName = item.ItemName ?? "",
                        Illustration = item.Illustration ?? "",
                        IsDecrease = item.IsDecrease,
                        IsBold = item.IsBold,
                        IsItalic = item.IsItalic
                    };

                    if (item.AccountBusinessResults != null)
                    {
                        foreach (var acc in item.AccountBusinessResults)
                        {
                            iDto.Accounts.Add(new AccountBusinessResultExportDto
                            {
                                AccountId = acc.AccountId ?? "",
                                CounterAccountId = acc.CounterAccountId ?? "",
                                IsAdd = acc.IsAdd,
                                Method = acc.Method
                            });
                        }
                    }

                    dto.Items.Add(iDto);
                }

                var dlg = new SaveFileDialog
                {
                    Title = "Xuất cấu hình Báo Cáo KQKD ra JSON",
                    Filter = "JSON Files (*.json)|*.json|All Files (*.*)|*.*",
                    FileName = "BusinessResult_Design.json"
                };

                if (dlg.ShowDialog(owner) == true)
                {
                    string json = JsonSerializer.Serialize(dto, _jsonOptions);
                    File.WriteAllText(dlg.FileName, json, System.Text.Encoding.UTF8);
                    MessageBox.Show(owner, "Đã xuất cấu hình ra tệp JSON thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(owner, "Lỗi khi xuất JSON: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public static bool ImportBusinessResult(Window owner)
        {
            var dlg = new OpenFileDialog
            {
                Title = "Chọn tệp JSON cấu hình Báo Cáo KQKD",
                Filter = "JSON Files (*.json)|*.json|All Files (*.*)|*.*",
                FileName = "BusinessResult_Design.json"
            };

            if (dlg.ShowDialog(owner) != true) return false;

            if (!ConfirmImport("Báo Cáo Kết Quả Kinh Doanh")) return false;

            try
            {
                string json = File.ReadAllText(dlg.FileName);
                var dto = JsonSerializer.Deserialize<BusinessResultDesignDto>(json, _jsonOptions);
                if (dto == null || dto.Items == null || dto.Items.Count == 0)
                {
                    MessageBox.Show(owner, "Tệp JSON không chứa dữ liệu hợp lệ!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }

                using var adapter = AppDataAccessAdapter.Create();
                adapter.StartTransaction(System.Data.IsolationLevel.ReadCommitted, "Import_BusinessResult");

                try
                {
                    adapter.DeleteEntitiesDirectly(typeof(AccountBusinessResultEntity), null);
                    adapter.DeleteEntitiesDirectly(typeof(BusinessResultItemEntity), null);

                    foreach (var iDto in dto.Items)
                    {
                        var item = new BusinessResultItemEntity
                        {
                            Code = iDto.Code ?? "",
                            ItemName = iDto.ItemName ?? "",
                            Illustration = iDto.Illustration ?? "",
                            IsDecrease = iDto.IsDecrease,
                            IsBold = iDto.IsBold,
                            IsItalic = iDto.IsItalic
                        };

                        if (iDto.Accounts != null)
                        {
                            foreach (var accDto in iDto.Accounts)
                            {
                                item.AccountBusinessResults.Add(new AccountBusinessResultEntity
                                {
                                    AccountId = accDto.AccountId ?? "",
                                    CounterAccountId = accDto.CounterAccountId ?? "",
                                    IsAdd = accDto.IsAdd,
                                    Method = accDto.Method
                                });
                            }
                        }

                        adapter.SaveEntity(item, refetchAfterSave: true, recurse: true);
                    }

                    adapter.Commit();
                    MessageBox.Show(owner, "Đã nạp mẫu Báo Cáo KQKD từ JSON thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    return true;
                }
                catch
                {
                    adapter.Rollback();
                    throw;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(owner, "Lỗi khi nạp dữ liệu từ JSON: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
        #endregion

        #region 3. Cash Flow
        public static void ExportCashFlow(Window owner)
        {
            try
            {
                using var adapter = AppDataAccessAdapter.Create();
                var list = new EntityCollection<CashFlowCategoryEntity>();
                var prefetch = new PrefetchPath2((int)DataAccess.EntityType.CashFlowCategoryEntity);
                prefetch.Add(CashFlowCategoryEntity.PrefetchPathCashFlowItems)
                        .SubPath.Add(CashFlowItemEntity.PrefetchPathAccountCashFlowItems);
                adapter.FetchEntityCollection(list, null, prefetch);

                var dto = new CashFlowDesignDto();
                foreach (var cat in list)
                {
                    var cDto = new CashFlowCategoryExportDto
                    {
                        CashFlowName = cat.CashFlowName ?? "",
                        CashFlowNameRepeate = cat.CashFlowNameRepeate ?? ""
                    };

                    if (cat.CashFlowItems != null)
                    {
                        foreach (var item in cat.CashFlowItems)
                        {
                            var iDto = new CashFlowItemExportDto
                            {
                                Code = item.Code ?? "",
                                ItemName = item.ItemName ?? "",
                                Illustration = item.Illustration ?? "",
                                Dbcr = item.Dbcr,
                                IsBold = item.IsBold,
                                IsItalic = item.IsItalic
                            };

                            if (item.AccountCashFlowItems != null)
                            {
                                foreach (var acc in item.AccountCashFlowItems)
                                {
                                    iDto.Accounts.Add(new AccountCashFlowItemExportDto
                                    {
                                        AccountId = acc.AccountId ?? "",
                                        CounterAccountId = acc.CounterAccountId ?? "",
                                        Dbcr = acc.Dbcr
                                    });
                                }
                            }

                            cDto.Items.Add(iDto);
                        }
                    }

                    dto.Categories.Add(cDto);
                }

                var dlg = new SaveFileDialog
                {
                    Title = "Xuất cấu hình Báo Cáo Lưu Chuyển Tiền Tệ ra JSON",
                    Filter = "JSON Files (*.json)|*.json|All Files (*.*)|*.*",
                    FileName = "CashFlow_Design.json"
                };

                if (dlg.ShowDialog(owner) == true)
                {
                    string json = JsonSerializer.Serialize(dto, _jsonOptions);
                    File.WriteAllText(dlg.FileName, json, System.Text.Encoding.UTF8);
                    MessageBox.Show(owner, "Đã xuất cấu hình ra tệp JSON thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(owner, "Lỗi khi xuất JSON: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public static bool ImportCashFlow(Window owner)
        {
            var dlg = new OpenFileDialog
            {
                Title = "Chọn tệp JSON cấu hình Báo Cáo Lưu Chuyển Tiền Tệ",
                Filter = "JSON Files (*.json)|*.json|All Files (*.*)|*.*",
                FileName = "CashFlow_Design.json"
            };

            if (dlg.ShowDialog(owner) != true) return false;

            if (!ConfirmImport("Báo Cáo Lưu Chuyển Tiền Tệ")) return false;

            try
            {
                string json = File.ReadAllText(dlg.FileName);
                var dto = JsonSerializer.Deserialize<CashFlowDesignDto>(json, _jsonOptions);
                if (dto == null || dto.Categories == null || dto.Categories.Count == 0)
                {
                    MessageBox.Show(owner, "Tệp JSON không chứa dữ liệu hợp lệ!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }

                using var adapter = AppDataAccessAdapter.Create();
                adapter.StartTransaction(System.Data.IsolationLevel.ReadCommitted, "Import_CashFlow");

                try
                {
                    adapter.DeleteEntitiesDirectly(typeof(AccountCashFlowItemEntity), null);
                    adapter.DeleteEntitiesDirectly(typeof(CashFlowItemEntity), null);
                    adapter.DeleteEntitiesDirectly(typeof(CashFlowCategoryEntity), null);

                    foreach (var cDto in dto.Categories)
                    {
                        var cat = new CashFlowCategoryEntity
                        {
                            CashFlowName = cDto.CashFlowName ?? "",
                            CashFlowNameRepeate = cDto.CashFlowNameRepeate ?? ""
                        };

                        if (cDto.Items != null)
                        {
                            foreach (var iDto in cDto.Items)
                            {
                                var item = new CashFlowItemEntity
                                {
                                    Code = iDto.Code ?? "",
                                    ItemName = iDto.ItemName ?? "",
                                    Illustration = iDto.Illustration ?? "",
                                    Dbcr = iDto.Dbcr,
                                    IsBold = iDto.IsBold,
                                    IsItalic = iDto.IsItalic
                                };

                                if (iDto.Accounts != null)
                                {
                                    foreach (var accDto in iDto.Accounts)
                                    {
                                        item.AccountCashFlowItems.Add(new AccountCashFlowItemEntity
                                        {
                                            AccountId = accDto.AccountId ?? "",
                                            CounterAccountId = accDto.CounterAccountId ?? "",
                                            Dbcr = accDto.Dbcr
                                        });
                                    }
                                }

                                cat.CashFlowItems.Add(item);
                            }
                        }

                        adapter.SaveEntity(cat, refetchAfterSave: true, recurse: true);
                    }

                    adapter.Commit();
                    MessageBox.Show(owner, "Đã nạp mẫu Báo Cáo Lưu Chuyển Tiền Tệ từ JSON thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    return true;
                }
                catch
                {
                    adapter.Rollback();
                    throw;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(owner, "Lỗi khi nạp dữ liệu từ JSON: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
        #endregion

        #region 4. Tax Obligation
        public static void ExportTaxObligation(Window owner)
        {
            try
            {
                using var adapter = AppDataAccessAdapter.Create();
                var list = new EntityCollection<TaxObligationEntity>();
                var prefetch = new PrefetchPath2((int)DataAccess.EntityType.TaxObligationEntity);
                prefetch.Add(TaxObligationEntity.PrefetchPathTaxObligationItems)
                        .SubPath.Add(TaxObligationItemEntity.PrefetchPathAccountTaxObligations);
                adapter.FetchEntityCollection(list, null, prefetch);

                var dto = new TaxObligationDesignDto();
                foreach (var ob in list)
                {
                    var oDto = new TaxObligationExportDto
                    {
                        ObligationName = ob.ObligationName ?? ""
                    };

                    if (ob.TaxObligationItems != null)
                    {
                        foreach (var item in ob.TaxObligationItems)
                        {
                            var iDto = new TaxObligationItemExportDto
                            {
                                Code = item.Code ?? "",
                                ItemName = item.ItemName ?? ""
                            };

                            if (item.AccountTaxObligations != null)
                            {
                                foreach (var acc in item.AccountTaxObligations)
                                {
                                    iDto.Accounts.Add(new AccountTaxObligationExportDto
                                    {
                                        AccountId = acc.AccountId ?? "",
                                        CounterAccountId = acc.CounterAccountId ?? "",
                                        ColIndex = acc.ColIndex,
                                        IsAdd = acc.IsAdd,
                                        Method = acc.Method
                                    });
                                }
                            }

                            oDto.Items.Add(iDto);
                        }
                    }

                    dto.Obligations.Add(oDto);
                }

                var dlg = new SaveFileDialog
                {
                    Title = "Xuất cấu hình Báo Cáo Nghĩa Vụ Thuế ra JSON",
                    Filter = "JSON Files (*.json)|*.json|All Files (*.*)|*.*",
                    FileName = "TaxObligation_Design.json"
                };

                if (dlg.ShowDialog(owner) == true)
                {
                    string json = JsonSerializer.Serialize(dto, _jsonOptions);
                    File.WriteAllText(dlg.FileName, json, System.Text.Encoding.UTF8);
                    MessageBox.Show(owner, "Đã xuất cấu hình ra tệp JSON thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(owner, "Lỗi khi xuất JSON: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public static bool ImportTaxObligation(Window owner)
        {
            var dlg = new OpenFileDialog
            {
                Title = "Chọn tệp JSON cấu hình Báo Cáo Nghĩa Vụ Thuế",
                Filter = "JSON Files (*.json)|*.json|All Files (*.*)|*.*",
                FileName = "TaxObligation_Design.json"
            };

            if (dlg.ShowDialog(owner) != true) return false;

            if (!ConfirmImport("Báo Cáo Tình Hình Nghĩa Vụ Thuế")) return false;

            try
            {
                string json = File.ReadAllText(dlg.FileName);
                var dto = JsonSerializer.Deserialize<TaxObligationDesignDto>(json, _jsonOptions);
                if (dto == null || dto.Obligations == null || dto.Obligations.Count == 0)
                {
                    MessageBox.Show(owner, "Tệp JSON không chứa dữ liệu hợp lệ!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }

                using var adapter = AppDataAccessAdapter.Create();
                adapter.StartTransaction(System.Data.IsolationLevel.ReadCommitted, "Import_TaxObligation");

                try
                {
                    adapter.DeleteEntitiesDirectly(typeof(AccountTaxObligationEntity), null);
                    adapter.DeleteEntitiesDirectly(typeof(TaxObligationItemEntity), null);
                    adapter.DeleteEntitiesDirectly(typeof(TaxObligationEntity), null);

                    foreach (var oDto in dto.Obligations)
                    {
                        var ob = new TaxObligationEntity
                        {
                            ObligationName = oDto.ObligationName ?? ""
                        };

                        if (oDto.Items != null)
                        {
                            foreach (var iDto in oDto.Items)
                            {
                                var item = new TaxObligationItemEntity
                                {
                                    Code = iDto.Code ?? "",
                                    ItemName = iDto.ItemName ?? ""
                                };

                                if (iDto.Accounts != null)
                                {
                                    foreach (var accDto in iDto.Accounts)
                                    {
                                        item.AccountTaxObligations.Add(new AccountTaxObligationEntity
                                        {
                                            AccountId = accDto.AccountId ?? "",
                                            CounterAccountId = accDto.CounterAccountId ?? "",
                                            ColIndex = accDto.ColIndex,
                                            IsAdd = accDto.IsAdd,
                                            Method = accDto.Method
                                        });
                                    }
                                }

                                ob.TaxObligationItems.Add(item);
                            }
                        }

                        adapter.SaveEntity(ob, refetchAfterSave: true, recurse: true);
                    }

                    adapter.Commit();
                    MessageBox.Show(owner, "Đã nạp mẫu Báo Cáo Nghĩa Vụ Thuế từ JSON thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    return true;
                }
                catch
                {
                    adapter.Rollback();
                    throw;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(owner, "Lỗi khi nạp dữ liệu từ JSON: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
        #endregion

        #region 5. Vat Declare (Mẫu 01/GTGT)
        public static void ExportVatDeclare(Window owner)
        {
            try
            {
                using var adapter = AppDataAccessAdapter.Create();
                var list = new EntityCollection<VatDeclareItemEntity>();
                var prefetch = new PrefetchPath2((int)DataAccess.EntityType.VatDeclareItemEntity);
                prefetch.Add(VatDeclareItemEntity.PrefetchPathAccountVatDeclareItems);
                adapter.FetchEntityCollection(list, null, prefetch);

                var dto = new VatDeclareDesignDto();
                foreach (var item in list)
                {
                    var iDto = new VatDeclareItemExportDto
                    {
                        Code = item.Code ?? "",
                        ItemName = item.ItemName ?? "",
                        IsBold = item.IsBold,
                        IsItalic = item.IsItalic
                    };

                    if (item.AccountVatDeclareItems != null)
                    {
                        foreach (var acc in item.AccountVatDeclareItems)
                        {
                            iDto.Accounts.Add(new AccountVatDeclareItemExportDto
                            {
                                AccountId = acc.AccountId ?? "",
                                CounterAccountId = acc.CounterAccountId ?? "",
                                ColumnNo = acc.ColumnNo,
                                IsAdd = acc.IsAdd,
                                Method = acc.Method
                            });
                        }
                    }

                    dto.Items.Add(iDto);
                }

                var dlg = new SaveFileDialog
                {
                    Title = "Xuất cấu hình Tờ Khai Thuế GTGT (01/GTGT) ra JSON",
                    Filter = "JSON Files (*.json)|*.json|All Files (*.*)|*.*",
                    FileName = "VatDeclare_Design.json"
                };

                if (dlg.ShowDialog(owner) == true)
                {
                    string json = JsonSerializer.Serialize(dto, _jsonOptions);
                    File.WriteAllText(dlg.FileName, json, System.Text.Encoding.UTF8);
                    MessageBox.Show(owner, "Đã xuất cấu hình ra tệp JSON thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(owner, "Lỗi khi xuất JSON: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public static bool ImportVatDeclare(Window owner)
        {
            var dlg = new OpenFileDialog
            {
                Title = "Chọn tệp JSON cấu hình Tờ Khai Thuế GTGT (01/GTGT)",
                Filter = "JSON Files (*.json)|*.json|All Files (*.*)|*.*",
                FileName = "VatDeclare_Design.json"
            };

            if (dlg.ShowDialog(owner) != true) return false;

            if (!ConfirmImport("Tờ Khai Thuế GTGT (Mẫu 01/GTGT)")) return false;

            try
            {
                string json = File.ReadAllText(dlg.FileName);
                var dto = JsonSerializer.Deserialize<VatDeclareDesignDto>(json, _jsonOptions);
                if (dto == null || dto.Items == null || dto.Items.Count == 0)
                {
                    MessageBox.Show(owner, "Tệp JSON không chứa dữ liệu hợp lệ!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }

                using var adapter = AppDataAccessAdapter.Create();
                adapter.StartTransaction(System.Data.IsolationLevel.ReadCommitted, "Import_VatDeclare");

                try
                {
                    adapter.DeleteEntitiesDirectly(typeof(AccountVatDeclareItemEntity), null);
                    adapter.DeleteEntitiesDirectly(typeof(VatDeclareItemEntity), null);

                    foreach (var iDto in dto.Items)
                    {
                        var item = new VatDeclareItemEntity
                        {
                            Code = iDto.Code ?? "",
                            ItemName = iDto.ItemName ?? "",
                            IsBold = iDto.IsBold,
                            IsItalic = iDto.IsItalic
                        };

                        if (iDto.Accounts != null)
                        {
                            foreach (var accDto in iDto.Accounts)
                            {
                                item.AccountVatDeclareItems.Add(new AccountVatDeclareItemEntity
                                {
                                    AccountId = accDto.AccountId ?? "",
                                    CounterAccountId = accDto.CounterAccountId ?? "",
                                    ColumnNo = accDto.ColumnNo,
                                    IsAdd = accDto.IsAdd,
                                    Method = accDto.Method
                                });
                            }
                        }

                        adapter.SaveEntity(item, refetchAfterSave: true, recurse: true);
                    }

                    adapter.Commit();
                    MessageBox.Show(owner, "Đã nạp mẫu Tờ Khai Thuế GTGT từ JSON thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    return true;
                }
                catch
                {
                    adapter.Rollback();
                    throw;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(owner, "Lỗi khi nạp dữ liệu từ JSON: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
        #endregion

        #region 6. Vat Return (Mẫu 01-3/GTGT)
        public static void ExportVatReturn(Window owner)
        {
            try
            {
                using var adapter = AppDataAccessAdapter.Create();
                var list = new EntityCollection<Vat3TaxItemEntity>();
                var prefetch = new PrefetchPath2((int)DataAccess.EntityType.Vat3TaxItemEntity);
                prefetch.Add(Vat3TaxItemEntity.PrefetchPathAccountVat3Items);
                adapter.FetchEntityCollection(list, null, prefetch);

                var dto = new VatReturnDesignDto();
                foreach (var item in list)
                {
                    var iDto = new VatReturnItemExportDto
                    {
                        Code = item.Code ?? "",
                        ItemName = item.ItemName ?? "",
                        IsBold = item.IsBold,
                        IsItalic = item.IsItalic
                    };

                    if (item.AccountVat3Items != null)
                    {
                        foreach (var acc in item.AccountVat3Items)
                        {
                            iDto.Accounts.Add(new AccountVatReturnItemExportDto
                            {
                                AccountId = acc.AccountId ?? "",
                                CounterAccountId = acc.CounterAccountId ?? "",
                                IsAdd = acc.IsAdd,
                                Method = acc.Method
                            });
                        }
                    }

                    dto.Items.Add(iDto);
                }

                var dlg = new SaveFileDialog
                {
                    Title = "Xuất cấu hình Bảng Kê Hoàn Thuế GTGT ra JSON",
                    Filter = "JSON Files (*.json)|*.json|All Files (*.*)|*.*",
                    FileName = "VatReturn_Design.json"
                };

                if (dlg.ShowDialog(owner) == true)
                {
                    string json = JsonSerializer.Serialize(dto, _jsonOptions);
                    File.WriteAllText(dlg.FileName, json, System.Text.Encoding.UTF8);
                    MessageBox.Show(owner, "Đã xuất cấu hình ra tệp JSON thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(owner, "Lỗi khi xuất JSON: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public static bool ImportVatReturn(Window owner)
        {
            var dlg = new OpenFileDialog
            {
                Title = "Chọn tệp JSON cấu hình Bảng Kê Hoàn Thuế GTGT",
                Filter = "JSON Files (*.json)|*.json|All Files (*.*)|*.*",
                FileName = "VatReturn_Design.json"
            };

            if (dlg.ShowDialog(owner) != true) return false;

            if (!ConfirmImport("Bảng Kê Đề Nghị Hoàn Thuế GTGT")) return false;

            try
            {
                string json = File.ReadAllText(dlg.FileName);
                var dto = JsonSerializer.Deserialize<VatReturnDesignDto>(json, _jsonOptions);
                if (dto == null || dto.Items == null || dto.Items.Count == 0)
                {
                    MessageBox.Show(owner, "Tệp JSON không chứa dữ liệu hợp lệ!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }

                using var adapter = AppDataAccessAdapter.Create();
                adapter.StartTransaction(System.Data.IsolationLevel.ReadCommitted, "Import_VatReturn");

                try
                {
                    adapter.DeleteEntitiesDirectly(typeof(AccountVat3ItemEntity), null);
                    adapter.DeleteEntitiesDirectly(typeof(Vat3TaxItemEntity), null);

                    foreach (var iDto in dto.Items)
                    {
                        var item = new Vat3TaxItemEntity
                        {
                            Code = iDto.Code ?? "",
                            ItemName = iDto.ItemName ?? "",
                            IsBold = iDto.IsBold,
                            IsItalic = iDto.IsItalic
                        };

                        if (iDto.Accounts != null)
                        {
                            foreach (var accDto in iDto.Accounts)
                            {
                                item.AccountVat3Items.Add(new AccountVat3ItemEntity
                                {
                                    AccountId = accDto.AccountId ?? "",
                                    CounterAccountId = accDto.CounterAccountId ?? "",
                                    IsAdd = accDto.IsAdd,
                                    Method = accDto.Method
                                });
                            }
                        }

                        adapter.SaveEntity(item, refetchAfterSave: true, recurse: true);
                    }

                    adapter.Commit();
                    MessageBox.Show(owner, "Đã nạp mẫu Bảng Kê Hoàn Thuế GTGT từ JSON thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    return true;
                }
                catch
                {
                    adapter.Rollback();
                    throw;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(owner, "Lỗi khi nạp dữ liệu từ JSON: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
        #endregion
    }
}
