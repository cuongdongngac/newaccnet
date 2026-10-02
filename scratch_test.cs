using System.Reflection;
using DevExpress.Xpf.Printing;
using System.Linq;

class Program
{
    static void Main()
    {
        var t = typeof(DocumentPreviewControl);
        foreach (var evt in t.GetEvents(BindingFlags.Public | BindingFlags.Instance).Where(x => x.Name.Contains("Click")))
        {
            System.Console.WriteLine(evt.Name);
        }
    }
}
