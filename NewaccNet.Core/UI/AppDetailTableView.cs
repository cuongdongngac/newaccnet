using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Mvvm;

namespace NewaccNet.Core.UI
{
    public class RowDeletingEventArgs : EventArgs
    {
        public object RowItem { get; }
        public int RowHandle { get; }
        public bool Cancel { get; set; }

        public RowDeletingEventArgs(object rowItem, int rowHandle)
        {
            RowItem = rowItem;
            RowHandle = rowHandle;
        }
    }

    public class RowDeletedEventArgs : EventArgs
    {
        public object RowItem { get; }
        public int RowHandle { get; }

        public RowDeletedEventArgs(object rowItem, int rowHandle)
        {
            RowItem = rowItem;
            RowHandle = rowHandle;
        }
    }

    public class AppDetailTableView : TableView
    {
        public const string DeleteColumnName = "colRowDeleteAction";

        public static readonly DependencyProperty ShowRowDeleteButtonProperty =
            DependencyProperty.Register(
                nameof(ShowRowDeleteButton),
                typeof(bool),
                typeof(AppDetailTableView),
                new PropertyMetadata(true, OnShowRowDeleteButtonChanged));

        public bool ShowRowDeleteButton
        {
            get => (bool)GetValue(ShowRowDeleteButtonProperty);
            set => SetValue(ShowRowDeleteButtonProperty, value);
        }

        public static readonly DependencyProperty FixDeleteButtonToRightProperty =
            DependencyProperty.Register(
                nameof(FixDeleteButtonToRight),
                typeof(bool),
                typeof(AppDetailTableView),
                new PropertyMetadata(false));

        public bool FixDeleteButtonToRight
        {
            get => (bool)GetValue(FixDeleteButtonToRightProperty);
            set => SetValue(FixDeleteButtonToRightProperty, value);
        }

        public static readonly DependencyProperty ConfirmDeleteProperty =
            DependencyProperty.Register(
                nameof(ConfirmDelete),
                typeof(bool),
                typeof(AppDetailTableView),
                new PropertyMetadata(false));

        public bool ConfirmDelete
        {
            get => (bool)GetValue(ConfirmDeleteProperty);
            set => SetValue(ConfirmDeleteProperty, value);
        }

        public event EventHandler<RowDeletingEventArgs> RowDeleting;
        public event EventHandler<RowDeletedEventArgs> RowDeleted;

        public ICommand RowDeleteCommand { get; }

        public AppDetailTableView()
        {
            RowDeleteCommand = new DelegateCommand<object>(param =>
            {
                int rowHandle = -1;
                if (param is int h)
                {
                    rowHandle = h;
                }
                else if (param is EditGridCellData cellData && cellData.RowData != null)
                {
                    rowHandle = cellData.RowData.RowHandle.Value;
                }
                else if (param != null && int.TryParse(param.ToString(), out int parsed))
                {
                    rowHandle = parsed;
                }
                else
                {
                    rowHandle = this.FocusedRowHandle;
                }

                if (rowHandle >= 0)
                {
                    ExecuteDeleteRow(rowHandle);
                }
            });

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
            this.Loaded += AppDetailTableView_Loaded;
        }

        private static void OnShowRowDeleteButtonChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is AppDetailTableView view)
            {
                view.UpdateDeleteColumnVisibility();
            }
        }

        private void AppDetailTableView_Loaded(object sender, RoutedEventArgs e)
        {
            EnsureDeleteColumn();
            EnsureDateFormatForColumns();
            EnsureHeadersCentered();
        }

        public void EnsureHeadersCentered()
        {
            if (Grid == null) return;

            if (Grid.Columns != null)
            {
                foreach (var col in Grid.Columns)
                {
                    col.HorizontalHeaderContentAlignment = HorizontalAlignment.Center;
                }
            }

            if (Grid.Bands != null)
            {
                foreach (var band in Grid.Bands)
                {
                    band.HorizontalHeaderContentAlignment = HorizontalAlignment.Center;
                    if (band.Columns != null)
                    {
                        foreach (var col in band.Columns)
                        {
                            col.HorizontalHeaderContentAlignment = HorizontalAlignment.Center;
                        }
                    }
                }
            }
        }

        public void EnsureDateFormatForColumns()
        {
            if (Grid == null) return;

            if (Grid.Columns != null)
            {
                foreach (var col in Grid.Columns)
                {
                    ApplyDateFormatToColumn(col);
                }
            }

            if (Grid.Bands != null)
            {
                foreach (var band in Grid.Bands)
                {
                    if (band.Columns != null)
                    {
                        foreach (var col in band.Columns)
                        {
                            ApplyDateFormatToColumn(col);
                        }
                    }
                }
            }
        }

        private static void ApplyDateFormatToColumn(GridColumn col)
        {
            if (col == null) return;

            if (col.EditSettings is DateEditSettings des)
            {
                AppDateEditSettings.ApplyDefaultDateFormat(des);
            }
            else if (col.EditSettings == null)
            {
                string fn = col.FieldName?.ToLower() ?? "";
                if (fn.EndsWith("date") || fn.Contains("date") || fn.Contains("ngay"))
                {
                    col.EditSettings = new AppDateEditSettings();
                }
            }
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
            if (this.FocusedRowHandle >= 0 && this.Grid?.CurrentItem != null)
            {
                ExecuteDeleteRow(this.FocusedRowHandle);
            }
        }

        public virtual void ExecuteDeleteRow(int rowHandle)
        {
            if (rowHandle < 0 || Grid == null) return;

            var rowItem = Grid.GetRow(rowHandle);
            if (rowItem == null) return;

            var cancelArgs = new RowDeletingEventArgs(rowItem, rowHandle);
            OnRowDeleting(cancelArgs);
            if (cancelArgs.Cancel) return;

            if (ConfirmDelete)
            {
                if (MessageBox.Show("Bạn có chắc muốn xóa dòng này?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
                {
                    return;
                }
            }

            if (this.ActiveEditor != null)
            {
                this.HideEditor();
            }

            this.DeleteRow(rowHandle);

            OnRowDeleted(new RowDeletedEventArgs(rowItem, rowHandle));
        }

        protected virtual void OnRowDeleting(RowDeletingEventArgs e)
        {
            RowDeleting?.Invoke(this, e);
        }

        protected virtual void OnRowDeleted(RowDeletedEventArgs e)
        {
            RowDeleted?.Invoke(this, e);
        }

        public void EnsureDeleteColumn()
        {
            if (!ShowRowDeleteButton || Grid == null) return;

            // Xử lý Grid dùng Bands
            if (Grid.Bands != null && Grid.Bands.Count > 0)
            {
                var existingBand = Grid.Bands.FirstOrDefault(b => b.Name == "bandRowActions");
                if (existingBand == null)
                {
                    bool isAutoWidth = this.AutoWidth;
                    bool shouldFixRight = FixDeleteButtonToRight && !isAutoWidth;

                    var col = CreateDeleteColumn();
                    var actionBand = new GridControlBand
                    {
                        Name = "bandRowActions",
                        Header = "",
                        OverlayHeaderByChildren = true,
                        Fixed = shouldFixRight ? FixedStyle.Right : FixedStyle.None
                    };
                    actionBand.Columns.Add(col);
                    Grid.Bands.Add(actionBand);
                }
                return;
            }

            // Xử lý Grid thường (Columns)
            if (!Grid.Columns.Any(c => c.Name == DeleteColumnName))
            {
                var col = CreateDeleteColumn();
                Grid.Columns.Add(col);
            }
        }

        private void UpdateDeleteColumnVisibility()
        {
            if (Grid == null) return;

            if (Grid.Bands != null && Grid.Bands.Count > 0)
            {
                var band = Grid.Bands.FirstOrDefault(b => b.Name == "bandRowActions");
                if (band != null)
                {
                    band.Visible = ShowRowDeleteButton;
                }
                else if (ShowRowDeleteButton)
                {
                    EnsureDeleteColumn();
                }
                return;
            }

            var col = Grid.Columns.FirstOrDefault(c => c.Name == DeleteColumnName);
            if (col != null)
            {
                col.Visible = ShowRowDeleteButton;
            }
            else if (ShowRowDeleteButton)
            {
                EnsureDeleteColumn();
            }
        }

        private GridColumn CreateDeleteColumn()
        {
            bool isAutoWidth = this.AutoWidth;
            bool shouldFixRight = FixDeleteButtonToRight && !isAutoWidth;

            return new GridColumn
            {
                Name = DeleteColumnName,
                Header = "",
                Width = 36,
                FixedWidth = true,
                Fixed = (Grid.Bands == null || Grid.Bands.Count == 0) && shouldFixRight ? FixedStyle.Right : FixedStyle.None,
                AllowResizing = DevExpress.Utils.DefaultBoolean.False,
                AllowMoving = DevExpress.Utils.DefaultBoolean.False,
                AllowSorting = DevExpress.Utils.DefaultBoolean.False,
                AllowColumnFiltering = DevExpress.Utils.DefaultBoolean.False,
                AllowGrouping = DevExpress.Utils.DefaultBoolean.False,
                AllowEditing = DevExpress.Utils.DefaultBoolean.False,
                AllowFocus = false,
                ShowInColumnChooser = false,
                HorizontalHeaderContentAlignment = HorizontalAlignment.Center,
                CellTemplate = CreateDeleteButtonTemplate()
            };
        }

        private static DataTemplate CreateDeleteButtonTemplate()
        {
            string xaml = @"
            <DataTemplate xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
                          xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
                <Grid Background=""Transparent"">
                    <Button ToolTip=""Xóa dòng""
                            Command=""{Binding View.RowDeleteCommand}""
                            CommandParameter=""{Binding RowData.RowHandle.Value}""
                            Width=""22"" Height=""22""
                            HorizontalAlignment=""Center"" VerticalAlignment=""Center""
                            Focusable=""False"" Cursor=""Hand"">
                        <Button.Template>
                            <ControlTemplate TargetType=""Button"">
                                <Border x:Name=""border"" Background=""Transparent"" CornerRadius=""11"">
                                    <Path x:Name=""icon"" Width=""13"" Height=""13"" Stretch=""Uniform""
                                          Fill=""#9E9E9E"" HorizontalAlignment=""Center"" VerticalAlignment=""Center""
                                          Data=""M6,19c0,1.1 0.9,2 2,2h8c1.1,0 2,-0.9 2,-2V7H6v12zM19,4h-3.5l-1,-1h-5l-1,1H5v2h14V4z""/>
                                </Border>
                                <ControlTemplate.Triggers>
                                    <Trigger Property=""IsMouseOver"" Value=""True"">
                                        <Setter TargetName=""border"" Property=""Background"" Value=""#FFEBEE""/>
                                        <Setter TargetName=""icon"" Property=""Fill"" Value=""#E53935""/>
                                    </Trigger>
                                    <Trigger Property=""IsPressed"" Value=""True"">
                                        <Setter TargetName=""border"" Property=""Background"" Value=""#FFCDD2""/>
                                        <Setter TargetName=""icon"" Property=""Fill"" Value=""#C62828""/>
                                    </Trigger>
                                </ControlTemplate.Triggers>
                            </ControlTemplate>
                        </Button.Template>
                        <Button.Style>
                            <Style TargetType=""Button"">
                                <Setter Property=""Visibility"" Value=""Visible""/>
                                <Style.Triggers>
                                    <DataTrigger Binding=""{Binding RowData.RowHandle.Value}"" Value=""-2147483647"">
                                        <Setter Property=""Visibility"" Value=""Collapsed""/>
                                    </DataTrigger>
                                    <DataTrigger Binding=""{Binding RowData.RowHandle.Value}"" Value=""-2147483646"">
                                        <Setter Property=""Visibility"" Value=""Collapsed""/>
                                    </DataTrigger>
                                </Style.Triggers>
                            </Style>
                        </Button.Style>
                    </Button>
                </Grid>
            </DataTemplate>";

            return (DataTemplate)System.Windows.Markup.XamlReader.Parse(xaml);
        }
    }
}

