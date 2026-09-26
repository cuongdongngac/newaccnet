using System.Windows;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Printing;

namespace NewaccNet.Wpf.Reports
{
    public static class ReportManager
    {
        /// <summary>
        /// Mở cửa sổ Preview In ấn (tạo Report) dựa trên GridControl hiện tại.
        /// Sử dụng tính năng PrintableControlLink / ShowPrintPreview của DevExpress WPF.
        /// </summary>
        public static void PrintGridControl(GridControl grid, string reportTitle)
        {
            if (grid == null || grid.View == null) return;
            
                        var view = grid.View as TableView;
            if (view != null)
            {
                // Tự động co giãn các cột để vừa khít với trang giấy (không bị tràn mất cột)
                view.PrintAutoWidth = true;

                // Sử dụng PrintableControlLink để tạo document report từ grid
                var link = new PrintableControlLink(view);
                
                // Mặc định xoay ngang giấy (Landscape) để chứa được nhiều cột hơn
                link.Landscape = true;
                
                // Định dạng tiêu đề báo cáo
                link.ReportHeaderTemplate = CreateHeaderTemplate(reportTitle);
                link.CreateDocument(true);

                // Mở cửa sổ Preview
                PrintHelper.ShowPrintPreview(Application.Current.MainWindow, link);
            }
        }

        private static DataTemplate CreateHeaderTemplate(string title)
        {
            // Trả về một DataTemplate đơn giản chứa TextBlock làm tiêu đề
            string xaml = $@"
                <DataTemplate xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
                              xmlns:dxe='http://schemas.devexpress.com/winfx/2008/xaml/editors'>
                    <TextBlock Text='{title}' FontSize='18' FontWeight='Bold' HorizontalAlignment='Center' Margin='0,10,0,20'/>
                </DataTemplate>";
            return (DataTemplate)System.Windows.Markup.XamlReader.Parse(xaml);
        }
    }
}
