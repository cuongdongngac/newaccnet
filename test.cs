using System;
using System.Reflection;

class Program
{
    static void Main()
    {
        try {
            var asm = Assembly.Load("DevExpress.Xpf.Printing.v25.2");
            foreach (var t in asm.GetTypes())
            {
                if (t.Name.Contains("BarItemNames") || t.Name.Contains("PreviewBarItem"))
                {
                    Console.WriteLine(t.FullName);
                    foreach (var f in t.GetFields())
                    {
                        Console.WriteLine("  " + f.Name + " = " + f.GetValue(null));
                    }
                }
            }
        } catch (Exception ex) {
            Console.WriteLine(ex);
        }
    }
}
