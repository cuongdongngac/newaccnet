using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

class Program
{
    static void Main()
    {
        string wpfDir = @"D:\NewaccNet\NewaccNet.Wpf";
        Console.WriteLine("Searching for Voucher lookups in: " + wpfDir);

        var patterns = new[]
        {
            @"Phiếu thu",
            @"Phiếu chi",
            @"Phiếu nhập",
            @"Phiếu xuất",
            @"VoucherType",
            @"VoucherKind",
            @"Loại phiếu",
            @"Lookup",
            @"VoucherCode",
            @"PT",
            @"PC",
            @"PKT"
        };

        foreach (var file in Directory.GetFiles(wpfDir, "*.cs", SearchOption.AllDirectories))
        {
            if (file.Contains(@"\obj\") || file.Contains(@"\bin\")) continue;
            var lines = File.ReadAllLines(file);
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                if (line.Contains("Phiếu") || line.Contains("VoucherType") || line.Contains("VoucherKind") || 
                    line.Contains("Loại phiếu") || line.Contains("List<") && line.Contains("Voucher") ||
                    line.Contains("cboVoucher") || line.Contains("VoucherNoPrefix"))
                {
                    Console.WriteLine($"{Path.GetFileName(file)}:{i + 1}: {line.Trim()}");
                }
            }
        }
    }
}
