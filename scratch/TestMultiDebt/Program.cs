using System;
using System.Linq;
using DataAccess.EntityClasses;
using DataAccess.HelperClasses;

class Program
{
    static void Main()
    {
        try
        {
            string conn = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=D:\mdb-migration-project\output\DLNB2018.accdb";
            System.Data.Common.DbProviderFactories.RegisterFactory("System.Data.OleDb", System.Data.OleDb.OleDbFactory.Instance);
            SD.LLBLGen.Pro.ORMSupportClasses.RuntimeConfiguration.ConfigureDQE<SD.LLBLGen.Pro.DQE.Access.AccessDQEConfiguration>(
                c => c.AddDbProviderFactory(typeof(System.Data.OleDb.OleDbFactory)));

            var adapter = new DataAccess.MsAccess.DatabaseSpecific.DataAccessAdapter(conn);
            var cols = new EntityCollection<ChartOfAccountEntity>();
            adapter.FetchEntityCollection(cols, null);
            
            var parentIds = cols.Select(a => a.ParentId).Where(id => !string.IsNullOrEmpty(id)).Distinct().ToList();

            string[] prefixes = new[] { "632", "511", "331", "131", "133", "333" };
            foreach(var p in prefixes)
            {
                var matches = cols.Where(x => x.AccountId.StartsWith(p)).ToList();
                Console.WriteLine($"=== Prefix {p} ===");
                foreach(var m in matches)
                {
                    bool isDetail = !parentIds.Contains(m.AccountId);
                    Console.WriteLine($"  {m.AccountId} | {m.AccountName} | Detail: {isDetail}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("EXCEPTION: " + ex);
        }
    }
}
