using System;
using System.Collections.Generic;
using DataAccess.EntityClasses;
using DataAccess.HelperClasses;
using NewaccNet.Wpf.AppSystem.Base;   // AppDataAccessAdapter
using SD.LLBLGen.Pro.ORMSupportClasses;

namespace Research
{
    class Program
    {
        /// <summary>
        /// NGHIÊN CỨU 2 CÂU HỎI TẬN GỐC:
        /// 
        /// 1) TẠI SAO BalanceSheet adapter.DeleteEntity(oldAcc) với oldAcc = new Entity() + Fields.Clone() + IsNew=False BÌNH THƯỜNG?
        ///    Hay thực tế BalanceSheet cũng CHẾT nếu xóa ReportFormulaAccountEntity? (BalanceSheet chưa test xóa lv5?)
        ///    => Chúng ta thử 2 pattern trên TaxObligation / AccountTaxObligation entity đã có trong DB.
        /// 
        /// 2) LLBLGen 5.12 UOW AddForSave overload THỰC TẾ LÀ GÌ? (để dùng UOW chuẩn theo yêu cầu user)
        ///    => Gọi reflection in-memory trên UOW2 instance để liệt kê TẤT CẢ public void AddForSave overload signatures.
        ///    => Và thử gọi UOW commit save/update (luôn rollback sau để giữ DB sạch).
        /// </summary>
        static void Main()
        {
            Console.WriteLine("=== NGHIÊN CỨU TẬN GỐC TaxObligation vs BalanceSheet Delete + UOW Signature ===\n");

            try
            {
                using var adapter = AppDataAccessAdapter.Create();

                // ==========================================================
                // BƯỚC 0: Lấy entity có thật trong DB (bất kỳ TaxObligation nào)
                // ==========================================================
                var all = new EntityCollection<TaxObligationEntity>();
                var prefetch = new PrefetchPath2((int)DataAccess.EntityType.TaxObligationEntity);
                prefetch.Add(TaxObligationEntity.PrefetchPathTaxObligationItems)
                        .SubPath.Add(TaxObligationItemEntity.PrefetchPathAccountTaxObligations);
                adapter.FetchEntityCollection(all, null, prefetch);

                if (all.Count == 0)
                {
                    Console.WriteLine("DB không có TaxObligation nào cả. Thoát.");
                    return;
                }
                TaxObligationEntity realL1 = all[0];
                TaxObligationItemEntity realL2 = null;
                AccountTaxObligationEntity realL3 = null;
                foreach (var l2 in realL1.TaxObligationItems)
                {
                    realL2 = l2;
                    if (l2.AccountTaxObligations.Count > 0) { realL3 = l2.AccountTaxObligations[0]; break; }
                }
                if (realL2 == null) { Console.WriteLine("Không có TaxObligationItem để test. Thoát."); return; }
                if (realL3 == null) { Console.WriteLine("Không có AccountTaxObligation (L3) để test pattern Delete clone. Thoát."); return; }
                Console.WriteLine($"Đã chọn mẫu thực tế: L1 Id={realL1.Id} Name={realL1.ObligationName}, L2 Id={realL2.Id}, L3 Id={realL3.Id} Acc={realL3.AccountId}\n");

                // ==========================================================
                // BƯỚC 1: NGHIÊN CỨU #1 - Clone entity 2 pattern XÓA DB.
                // ==========================================================
                // Chúng ta KHÔNG xóa thật sự DB. Tất cả run trong một transaction mới và luôn ROLLBACK sau.
                adapter.StartTransaction(System.Data.IsolationLevel.ReadCommitted, "RESEARCH_TXN_ROLLBACK_ALWAYS");
                try
                {
                    Console.WriteLine("--- Pattern A (TaxObligation Pattern CŨ): new Entity() + Fields.Clone() + set IsNew/IsDirty tay ---");
                    try
                    {
                        var cloneA = new AccountTaxObligationEntity();
                        cloneA.Fields = realL3.Fields.Clone();
                        cloneA.IsNew = realL3.IsNew;         // realL3.IsNew = False (fetch từ DB)
                        cloneA.IsDirty = realL3.IsDirty;     // False

                        Console.WriteLine($"CloneA create OK. Id={cloneA.Id}, IsNew={cloneA.IsNew}, IsDirty={cloneA.IsDirty}, ObjectState={cloneA.Fields.State.ToString()}");

                        // Thử xóa (rollback sau)
                        bool deletedA = adapter.DeleteEntity(cloneA);
                        Console.WriteLine($"  ✅ adapter.DeleteEntity(cloneA) THÀNH CÔNG? Result={deletedA} (dự kiến FALSE vì rollback, nhưng không exception)");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"  ❌ adapter.DeleteEntity(cloneA) LỖI: {ex.GetType().Name}: {ex.Message}\n       Stack: {ex.StackTrace?.Substring(0, Math.Min(300, ex.StackTrace.Length))}\n");
                    }

                    Console.WriteLine("\n--- Pattern B (Constructor Id + CloneFields) ---");
                    try
                    {
                        var cloneB = new AccountTaxObligationEntity(realL3.Id);
                        cloneB.Fields = realL3.Fields.Clone();
                        // KHÔNG set IsNew/IsDirty tay
                        Console.WriteLine($"CloneB create OK. Id={cloneB.Id}, IsNew={cloneB.IsNew}, IsDirty={cloneB.IsDirty}, ObjectState={cloneB.Fields.State.ToString()}");
                        bool deletedB = adapter.DeleteEntity(cloneB);
                        Console.WriteLine($"  ✅ adapter.DeleteEntity(cloneB) THÀNH CÔNG? Result={deletedB}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"  ❌ adapter.DeleteEntity(cloneB) LỖI: {ex.GetType().Name}: {ex.Message}\n");
                    }

                    Console.WriteLine("\n--- Pattern C (BalanceSheet Chuẩn MẪU): new Entity() + Fields.Clone() + set IsNew=real.IsNew (False) + IsDirty=False ---");
                    // => Chính xác pattern BalanceSheet L177-L181!
                    try
                    {
                        var cloneC = new AccountTaxObligationEntity();
                        cloneC.Fields = realL3.Fields.Clone();
                        cloneC.IsNew = realL3.IsNew;  // False
                        cloneC.IsDirty = realL3.IsDirty;  // False
                        Console.WriteLine($"CloneC create OK (GỐC BALANCESHEET). Id={cloneC.Id}, IsNew={cloneC.IsNew}, IsDirty={cloneC.IsDirty}, ObjectState={cloneC.Fields.State.ToString()}");
                        bool deletedC = adapter.DeleteEntity(cloneC);
                        Console.WriteLine($"  ✅ adapter.DeleteEntity(cloneC) THÀNH CÔNG? Result={deletedC}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"  ❌ adapter.DeleteEntity(cloneC) LỖI: {ex.GetType().Name}: {ex.Message}\n");
                    }

                    Console.WriteLine("\n--- Pattern D (FETCH THẬT TỪ DB): Constructor Id + adapter.FetchEntity ---");
                    try
                    {
                        var cloneD = new AccountTaxObligationEntity(realL3.Id);
                        adapter.FetchEntity(cloneD);
                        Console.WriteLine($"CloneD fetch OK. Id={cloneD.Id}, IsNew={cloneD.IsNew}, IsDirty={cloneD.IsDirty}, ObjectState={cloneD.Fields.State.ToString()}");
                        bool deletedD = adapter.DeleteEntity(cloneD);
                        Console.WriteLine($"  ✅ adapter.DeleteEntity(cloneD) THÀNH CÔNG? Result={deletedD}. (ĐÂY LÀ CÁCH 100% OK NHƯNG ĐỘNG TỚI DB 1 LẦN THÊM)");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"  ❌ adapter.DeleteEntity(cloneD) LỖI: {ex.GetType().Name}: {ex.Message}\n");
                    }

                    // Luôn Rollback research transaction, KHÔNG ĐỘNG DB THẬT
                    adapter.Rollback();
                    Console.WriteLine("\n[RESEARCH TXN] Đã ROLLBACK toàn bộ delete thử nghiệm. DB nguyên vẹn.\n");
                }
                catch
                {
                    adapter.Rollback();
                    throw;
                }

                // ==========================================================
                // BƯỚC 2: NGHIÊN CỨU #2 - Liệt kê UOW2 AddForSave overloads thật sự.
                // ==========================================================
                Console.WriteLine("=== LLBLGen 5.12 UOW2 AddForSave OVERLOAD signature (reflect) ===");
                var uow = new UnitOfWork2();
                var tUow = uow.GetType();
                foreach (var mi in tUow.GetMethods(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public))
                {
                    if (mi.Name != "AddForSave") continue;
                    var p = mi.GetParameters();
                    var sigParts = new List<string>();
                    foreach (var pp in p)
                    {
                        sigParts.Add($"{pp.ParameterType.Name} {pp.Name}");
                    }
                    Console.WriteLine($"  public void AddForSave({string.Join(", ", sigParts)});");
                }
                Console.WriteLine("\n=== UOW2 AddForDelete overloads ===");
                foreach (var mi in tUow.GetMethods(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public))
                {
                    if (mi.Name != "AddForDelete") continue;
                    var p = mi.GetParameters();
                    var sigParts = new List<string>();
                    foreach (var pp in p) sigParts.Add($"{pp.ParameterType.Name} {pp.Name}");
                    Console.WriteLine($"  public void AddForDelete({string.Join(", ", sigParts)});");
                }
                Console.WriteLine("\n=== UOW2 Commit overloads ===");
                foreach (var mi in tUow.GetMethods(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public))
                {
                    if (mi.Name != "Commit") continue;
                    var p = mi.GetParameters();
                    var sigParts = new List<string>();
                    foreach (var pp in p) sigParts.Add($"{pp.ParameterType.Name} {pp.Name}");
                    Console.WriteLine($"  public void Commit({string.Join(", ", sigParts)});");
                }

                // ==========================================================
                // BƯỚC 3: NGHIÊN CỨU #2b - THỬ UOW2 Update L3 (rollback luôn)
                // ==========================================================
                Console.WriteLine("\n=== Test UOW Update L3 (rollback luôn) theo overload vừa tìm được ===");
                adapter.StartTransaction(System.Data.IsolationLevel.ReadCommitted, "RESEARCH_UOW_ROLLBACK");
                try
                {
                    string old = realL3.AccountId;
                    string neu = old == "3331" ? "33312" : "3331";
                    realL3.AccountId = neu;

                    // Dùng overload đầu tiên tìm được (tham số 2 là IPredicateExpression filter)
                    // Lưu ý research L2/L3 update chỉ có tác dụng nếu UOW recurse đúng.
                    var uow2 = new UnitOfWork2();

                    // ------ Dùng AddForSave overload tìm được ------
                    // Tạm comment cho đến khi biết chính xác overload:
                    // uow2.AddForSave(realL1, null, false, true);
                    // Tạm thời thử gọi theo reflection parameterless overload nào tồn tại
                    // Thay vào đó ta test 2 trường hợp:
                    //   TH1: AddForSave(realL3) đơn (không recurse)
                    //   TH2: AddForSave toàn bộ cây 3 l1/l2/l3 độc lập (3 vòng như TaxObligation cũ) rồi Commit
                    uow2.AddForDelete(realL3);   // Test xóa thử rồi rollback
                    uow2.AddForSave(realL3);     // Sau đó save lại (không có tác dụng vì delete+save nhưng test UOW accept)

                    bool rolledBack = false;
                    try
                    {
                        // KHÔNG commit thật (chạy sẽ lỗi delete/save conflict, ta bắt exception, rollback)
                        uow2.Commit(adapter, false);
                    }
                    catch (Exception exUow)
                    {
                        // Expected có thể lỗi vì delete+save cùng 1 entity
                        Console.WriteLine($"  UOW Commit exception (expected do test delete+save conflict): {exUow.GetType().Name}: {exUow.Message.Substring(0, Math.Min(200, exUow.Message.Length))}");
                        adapter.Rollback();
                        rolledBack = true;
                    }
                    finally
                    {
                        if (!rolledBack) adapter.Rollback();
                    }
                    // Restore giá trị
                    realL3.AccountId = old;
                    realL3.IsDirty = false;
                }
                catch
                {
                    adapter.Rollback();
                    throw;
                }

                Console.WriteLine("\n=== KẾT LUẬN RESEARCH ===");
                Console.WriteLine("Xem kết quả Pattern nào DeleteEntity OK mà KHÔNG cần fetch DB thêm. Đó là pattern chúng ta sẽ dùng cho TaxObligation Snapshot.");
                Console.WriteLine("Xem overload AddForSave nào tồn tại => dùng UOW chuẩn cho 4 design window sau.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nEXCEPTION TỔNG: {ex}");
            }
        }
    }
}
