using System;
using System.Reflection;
using System.Resources;
using System.Collections;

class Program
{
    static void Main()
    {
        try
        {
            var asm = Assembly.Load("DevExpress.Images.v25.2");
            var resName = asm.GetName().Name + ".g.resources";
            using var stream = asm.GetManifestResourceStream(resName);
            using var reader = new ResourceReader(stream);
            bool found = false;
            foreach (DictionaryEntry entry in reader)
            {
                var key = entry.Key.ToString();
                if (key.Contains("findpoint"))
                {
                    Console.WriteLine("Found: " + key);
                    found = true;
                }
            }
            if (!found) Console.WriteLine("NOT FOUND findpoint!");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }
}
