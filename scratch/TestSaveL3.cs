using System;
using DataAccess.EntityClasses;
using DataAccess.HelperClasses;
using NewaccNet.Wpf.AppSystem.Base;
using NewaccNet.Wpf.Views.Base;
using SD.LLBLGen.Pro.ORMSupportClasses;

class Program
{
    static void Main()
    {
        try
        {
            using var adapter = AppDataAccessAdapter.Create();
            var ent = new AccountTaxObligationEntity(23);
            bool fetched = adapter.FetchEntity(ent);
            Console.WriteLine($""Fetched Id=23: {fetched}, Current AccountId='{ent.AccountId}', IsDirty={ent.IsDirty}"");

            string oldVal = ent.AccountId;
            string newVal = oldVal == ""3331"" ? ""33312"" : ""3331"";
            ent.AccountId = newVal;
            Console.WriteLine($""After change: AccountId='{ent.AccountId}', IsDirty={ent.IsDirty}"");

            bool saved = adapter.SaveEntity(ent, refetchAfterSave: true);
            Console.WriteLine($""SaveEntity result: {saved}, Refetched AccountId='{ent.AccountId}'"");
        }
        catch (Exception ex)
        {
            Console.WriteLine(""EXCEPTION: "" + ex);
        }
    }
}
