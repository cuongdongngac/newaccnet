using System;
using DataAccess.DatabaseSpecific;
using DataAccess.EntityClasses;
using DataAccess.FactoryClasses;
using DataAccess.HelperClasses;

class Program
{
    static void Main()
    {
        try {
            string connStr = "Data Source=localhost\\SQLEXPRESS;Initial Catalog=NewaccNet;Integrated Security=True;TrustServerCertificate=True";
            DataAccessAdapter.SetSqlServerCompatibilityLevel(SD.LLBLGen.Pro.DQE.SqlServer.SqlServerCompatibilityLevel.SqlServer2012);
            using var adapter = new DataAccessAdapter(connStr);
            
            var ent = new AccountTaxObligationEntity(23);
            bool fetched = adapter.FetchEntity(ent);
            Console.WriteLine($"Fetched Id=23: {fetched}, Current AccountId='{ent.AccountId}', IsDirty={ent.IsDirty}");

            // Test UnitOfWork2
            var uow = new SD.LLBLGen.Pro.ORMSupportClasses.UnitOfWork2();
            string oldVal = ent.AccountId;
            ent.AccountId = oldVal == "3331" ? "33312" : "3331";
            Console.WriteLine($"Changed to: '{ent.AccountId}', IsDirty={ent.IsDirty}");

            uow.AddForSave(ent);
            Console.WriteLine($"Entities to save in UOW: {uow.GetEntityElementsToSave().Count}");

            uow.Commit(adapter, true);
            Console.WriteLine("Committed UOW!");

            var ent2 = new AccountTaxObligationEntity(23);
            adapter.FetchEntity(ent2);
            Console.WriteLine($"Re-fetched Id=23 from DB: AccountId='{ent2.AccountId}'");
        } catch (Exception ex) {
            Console.WriteLine("EXCEPTION: " + ex);
        }
    }
}
