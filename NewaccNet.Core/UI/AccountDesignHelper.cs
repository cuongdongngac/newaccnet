using System;
using System.Collections.Generic;
using System.Windows;
using DevExpress.Xpf.Editors.Settings;
using DataAccess.EntityClasses;

namespace NewaccNet.Core.UI
{
    /// <summary>
    /// Helper chuẩn dùng chung cho các màn hình Thiết kế & Cấu hình tài khoản (Design & Config):
    /// - Cung cấp ComboBoxEditSettings chuẩn hóa (hiển thị Mã TK và Tên TK trong dropdown).
    /// - Không chứa các hành vi hạch toán/xử lý chứng từ, chuyên dụng cho màn hình Thiết kế cấu hình.
    /// - Đồng nhất 100% với phong cách thiết kế báo cáo (BusinessResultDesign, BalanceSheetDesign...).
    /// </summary>
    public static class AccountDesignHelper
    {
        private static readonly Lazy<DataTemplate> _accountItemTemplate = new Lazy<DataTemplate>(() =>
        {
            string xaml = @"
                <DataTemplate xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"">
                    <Grid Margin=""4,2"">
                        <Grid.ColumnDefinitions>
                            <ColumnDefinition Width=""85""/>
                            <ColumnDefinition Width=""*""/>
                        </Grid.ColumnDefinitions>
                        <TextBlock Grid.Column=""0"" Text=""{Binding AccountId}"" FontWeight=""SemiBold"" VerticalAlignment=""Center""/>
                        <TextBlock Grid.Column=""1"" Text=""{Binding AccountName}"" Foreground=""#555555"" Margin=""10,0,0,0"" VerticalAlignment=""Center""/>
                    </Grid>
                </DataTemplate>";
            return (DataTemplate)System.Windows.Markup.XamlReader.Parse(xaml);
        });

        public static DataTemplate AccountItemTemplate => _accountItemTemplate.Value;

        public static ComboBoxEditSettings CreateAccountComboBoxEditSettings(IEnumerable<ChartOfAccountEntity> items)
        {
            return new ComboBoxEditSettings
            {
                ItemsSource = items,
                DisplayMember = nameof(ChartOfAccountEntity.AccountId),
                ValueMember = nameof(ChartOfAccountEntity.AccountId),
                AutoComplete = true,
                ImmediatePopup = true,
                ItemTemplate = _accountItemTemplate.Value
            };
        }
    }
}
