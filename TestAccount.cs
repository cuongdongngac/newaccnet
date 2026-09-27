using System;
using DataAccess.EntityClasses;
using DataAccess.DatabaseSpecific;
using NewaccNet.Wpf.AppSystem;
using SD.LLBLGen.Pro.ORMSupportClasses;

class Program
{
    static void Main()
    {
        try 
        {
            using (var adapter = AppDataAccessAdapter.Create())
            {
                var accounts = new EntityCollection<ChartOfAccountEntity>();
                adapter.FetchEntityCollection(accounts, null);
                Console.WriteLine("Total accounts: " + accounts.Count);
                foreach(var acc in accounts)
                {
                    if(acc.AccountId == "131" || acc.AccountId == "331")
                    {
                        Console.WriteLine($"Account {acc.AccountId} - {acc.AccountName}, CategoryId: {acc.CategoryId}");
                    }
                }
            }
        }
        catch(Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }
}