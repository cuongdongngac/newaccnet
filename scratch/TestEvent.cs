using System;
using DevExpress.Xpf.Grid;
class Test {
  public static void Main() {
      foreach(var prop in typeof(FocusedRowChangedEventArgs).GetProperties()) {
          Console.WriteLine(prop.Name);
      }
  }
}
