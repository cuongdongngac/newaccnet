using System;
using System.Windows;
using System.Windows.Input;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Grid;
using DevExpress.Mvvm;

namespace NewaccNet.Wpf.Views.Base
{
    public class AppDetailTableView : TableView
    {
        public AppDetailTableView()
        {
            // Context menu setup
            var addBtn = new BarButtonItem()
            {
                Content = "Thêm dòng mới",
                Glyph = new DevExpress.Xpf.Core.SvgImageSourceExtension() { Uri = new Uri("pack://application:,,,/DevExpress.Images.v25.2;component/svgimages/icon%20builder/actions_add.svg") }.ProvideValue(null) as System.Windows.Media.ImageSource
            };
            addBtn.ItemClick += (s, e) => AddNewRow();
            
            var deleteBtn = new BarButtonItem()
            {
                Content = "Xóa dòng",
                Glyph = new DevExpress.Xpf.Core.SvgImageSourceExtension() { Uri = new Uri("pack://application:,,,/DevExpress.Images.v25.2;component/svgimages/icon%20builder/actions_delete.svg") }.ProvideValue(null) as System.Windows.Media.ImageSource
            };
            deleteBtn.ItemClick += (s, e) => DeleteCurrentRow();

            this.RowCellMenuCustomizations.Add(addBtn);
            this.RowCellMenuCustomizations.Add(deleteBtn);

            this.PreviewKeyDown += AppDetailTableView_PreviewKeyDown;
        }

        private void AppDetailTableView_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete && this.ActiveEditor == null)
            {
                DeleteCurrentRow();
                e.Handled = true;
            }
        }

        private void DeleteCurrentRow()
        {
            if (this.FocusedRowHandle >= 0 && this.Grid.CurrentItem != null)
            {
                if (MessageBox.Show("Bạn có chắc muốn xóa dòng này?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    this.DeleteRow(this.FocusedRowHandle);
                }
            }
        }
    }
}
