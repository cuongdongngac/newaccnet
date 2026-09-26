using System.Windows.Markup;
using System.IO;
using System.Windows;
public class Test {
    public static void Run() {
        try {
            var fs = new FileStream("test.xaml", FileMode.Open);
            var win = (Window)XamlReader.Load(fs);
            System.Console.WriteLine("Success!");
        } catch(System.Exception ex) {
            System.Console.WriteLine("Error: " + ex.Message);
        }
    }
}
