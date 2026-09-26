using System;
using System.Reflection;
using System.Resources;
using System.Collections;
class P {
    static void Main() {
        var asm = Assembly.LoadFile(@"D:\NewaccNet\NewaccNet.Wpf\bin\Debug\net10.0-windows\DevExpress.Images.v25.2.dll");
        var stream = asm.GetManifestResourceStream("DevExpress.Images.v25.2.g.resources");
        using (var reader = new ResourceReader(stream)) {
            foreach (DictionaryEntry entry in reader) {
                string key = entry.Key.ToString();
                if (key.StartsWith("svgimages/") && !key.Contains("%20")) {
                    Console.WriteLine(key);
                }
            }
        }
    }
}
