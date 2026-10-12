using System;
using System.Threading;
using System.Windows;
using NewaccNet.Wpf.AppSystem.Design;

class Program
{
    [STAThread]
    static void Main()
    {
        try
        {
            if (Application.Current == null)
            {
                new Application();
            }
            var win = new TaxObligationDesignWindow();
            Console.WriteLine('SUCCESS');
        }
        catch (Exception ex)
        {
            Console.WriteLine('ERROR: ' + ex.ToString());
            if (ex.InnerException != null)
            {
                Console.WriteLine('INNER: ' + ex.InnerException.ToString());
            }
        }
    }
}
