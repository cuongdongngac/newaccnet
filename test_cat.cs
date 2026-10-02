using System;
using DataAccess.DatabaseSpecific;
using DataAccess.EntityClasses;
using DataAccess.FactoryClasses;
using DataAccess.HelperClasses;
using SD.LLBLGen.Pro.QuerySpec;
using SD.LLBLGen.Pro.QuerySpec.Adapter;

class Program
{
    static void Main()
    {
        try {
            string connStr = ""Data Source=localhost\\SQLEXPRESS;Initial Catalog=NewaccNet;Integrated Security=True;TrustServerCertificate=True"";
            DataAccessAdapter.SetSqlServerCompatibilityLevel(SD.LLBLGen.Pro.DQE.SqlServer.SqlServerCompatibilityLevel.SqlServer2012);
            using var adapter = new DataAccessAdapter(connStr);
            var qf = new QueryFactory();
            var q = qf.ChartOfAccount.Where(ChartOfAccountFields.CategoryId.Equal(""A"")).Limit(5);
            var results = adapter.FetchQuery(q);
            foreach (ChartOfAccountEntity c in results) {
                Console.WriteLine(c.AccountId + "" - "" + c.AccountName + "" ("" + c.CategoryId + "")"");
            }
        } catch (Exception ex) {
            Console.WriteLine(ex.Message);
        }
    }
}
