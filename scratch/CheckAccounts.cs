using System;
using System.Linq;
using DataAccess.EntityClasses;
using DataAccess.HelperClasses;
using NewaccNet.Wpf;

App.ConfigureDatabase();
using var adapter = AppDataAccessAdapter.Create();
var cols = new EntityCollection<ChartOfAccountEntity>();
adapter.FetchEntityCollection(cols, null);
var list = cols.Where(x => x.AccountId.StartsWith("133") || x.AccountId.StartsWith("333")).ToList();
foreach(var a in list)
{
    Console.WriteLine($"{a.AccountId} | {a.AccountName} | Parent: {a.ParentId}");
}
