using System;
using System.Reflection;
using System.Resources;
using System.Collections;
class P {
    static void Main() {
        var asm = Assembly.LoadFile(@"D:\NewaccNet\NewaccNet.Wpf\bin\Debug\net10.0-windows\DevExpress.Images.v25.2.dll");
        var stream = asm.GetManifestResourceStream("DevExpress.Images.v25.2.g.resources");
        if (stream == null) { Console.WriteLine("No resources found"); return; }
        using (var reader = new ResourceReader(stream)) {
            foreach (DictionaryEntry entry in reader) {
                if (entry.Key.ToString().Contains("svg")) {
                    Console.WriteLine(entry.Key);
                }
            }
        }
    }
}
