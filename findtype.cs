using System;
using System.Linq;
using System.Reflection;

class Program
{
    static void Main()
    {
        try {
            var files = System.IO.Directory.GetFiles("C:\\Program Files\\DevExpress 25.2\\Components\\Bin\\Framework\\", "DevExpress.Xpf.*.dll");
            foreach(var file in files) {
                var asm = Assembly.LoadFile(file);
                var type = asm.GetTypes().FirstOrDefault(t => t.Name == "AddBarButtonItemAction");
                if (type != null) {
                    Console.WriteLine("FOUND: " + type.FullName + " in " + file);
                }
            }
        } catch (Exception ex) {
            Console.WriteLine(ex);
        }
    }
}
