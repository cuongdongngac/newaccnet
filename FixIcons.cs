using System;
using System.IO;
using System.Text.RegularExpressions;

class Program
{
    static void Main()
    {
        string path = @""D:\NewaccNet\NewaccNet.Wpf\MainWindow.xaml"";
        string content = File.ReadAllText(path);

        // Define replacements (Name to SVG)
        var replacements = new System.Collections.Generic.Dictionary<string, string>
        {
            { ""Lý do công nợ"", ""bo_validation.svg"" },
            { ""Kho hàng"", ""bo_address.svg"" },
            { ""Danh mục vật tư"", ""bo_product.svg"" },
            { ""Loại tiền"", ""bo_price_item.svg"" },
            { ""Đối tượng CP"", ""bo_organization.svg"" },
            { ""Yếu tố CP"", ""bo_department.svg"" },
            { ""Nguồn tài sản"", ""bo_project.svg"" },
            { ""Lý do tăng giảm"", ""bo_appearance.svg"" },
            { ""Danh mục CP"", ""bo_pivotchart.svg"" },
            { ""Phiếu thu"", ""bo_sale.svg"" },
            { ""Phiếu chi"", ""bo_sale_item.svg"" },
            { ""Nhập kho"", ""bo_order.svg"" },
            { ""Xuất kho"", ""bo_order_item.svg"" }
        };

        foreach (var kvp in replacements)
        {
            string pattern = @""(?s)(<dxb:BarButtonItem Content=""""" + kvp.Key + @""""".*?LargeGlyph=""""""\{dx:SvgImageSource\s+Uri='pack://application:,,,/DevExpress\.Images\.v25\.2;component/svgimages/business%20objects/)bo_document\.svg""";
            string replacement = ""$1"" + kvp.Value + ""\""";
            content = Regex.Replace(content, pattern, replacement);
            
            // Try to catch bo_list.svg as well for Nguồn tài sản
            string pattern2 = @""(?s)(<dxb:BarButtonItem Content=""""" + kvp.Key + @""""".*?LargeGlyph=""""""\{dx:SvgImageSource\s+Uri='pack://application:,,,/DevExpress\.Images\.v25\.2;component/svgimages/business%20objects/)bo_list\.svg""";
            content = Regex.Replace(content, pattern2, replacement);
        }

        File.WriteAllText(path, content, System.Text.Encoding.UTF8);
    }
}
