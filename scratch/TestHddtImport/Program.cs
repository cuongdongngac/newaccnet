using System;
using System.Data;
using System.Data.OleDb;
using System.IO;

namespace TestHddtImport
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            string[] paths = new[] {
                @"D:\NewaccNet\Template\template.accdb",
                @"D:\mdb-migration-project\output\DLNB2018.accdb",
                @"D:\mdb-migration-project\mapping\template.accdb"
            };

            foreach (var path in paths)
            {
                Console.WriteLine("====================================================");
                Console.WriteLine("Checking: " + path);
                if (!File.Exists(path))
                {
                    Console.WriteLine("File DOES NOT EXIST!");
                    continue;
                }

                try
                {
                    using var conn = new OleDbConnection($"Provider=Microsoft.ACE.OLEDB.16.0;Data Source={path};Persist Security Info=False;");
                    conn.Open();
                    var schema = conn.GetSchema("Tables");
                    bool hasDbConfig = false;
                    foreach (DataRow row in schema.Rows)
                    {
                        string tName = row["TABLE_NAME"]?.ToString() ?? "";
                        string tType = row["TABLE_TYPE"]?.ToString() ?? "";
                        if (tType == "TABLE")
                        {
                            if (string.Equals(tName, "DbConfig", StringComparison.OrdinalIgnoreCase)) hasDbConfig = true;
                            if (tName.IndexOf("config", StringComparison.OrdinalIgnoreCase) >= 0 ||
                                tName.IndexOf("company", StringComparison.OrdinalIgnoreCase) >= 0 ||
                                tName.IndexOf("thongtin", StringComparison.OrdinalIgnoreCase) >= 0 ||
                                tName.IndexOf("donvi", StringComparison.OrdinalIgnoreCase) >= 0)
                            {
                                Console.WriteLine("  Candidate table: " + tName);
                            }
                        }
                    }

                    Console.WriteLine("  Has DbConfig table? " + hasDbConfig);
                    if (hasDbConfig)
                    {
                        using var cmd = conn.CreateCommand();
                        cmd.CommandText = "SELECT * FROM DbConfig";
                        using var reader = cmd.ExecuteReader();
                        int rowIdx = 0;
                        while (reader.Read())
                        {
                            rowIdx++;
                            Console.WriteLine($"  --- Row #{rowIdx} ---");
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                Console.WriteLine($"    {reader.GetName(i)} = {reader[i]}");
                            }
                        }
                        if (rowIdx == 0) Console.WriteLine("  DbConfig table is EMPTY (0 rows).");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("  Error checking: " + ex.Message);
                }
            }
        }
    }
}
