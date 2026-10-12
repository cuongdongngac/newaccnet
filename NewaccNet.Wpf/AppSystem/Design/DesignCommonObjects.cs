using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Grid;
using DataAccess.EntityClasses;
using DataAccess.HelperClasses;

namespace NewaccNet.Wpf.AppSystem.Design
{
    /// <summary>
    /// Đối tượng đại diện cho một Phương pháp tính (Method) trong các form thiết kế báo cáo.
    /// Giá trị chuẩn:
    ///   0  -> Số dư
    ///   1  -> Phát sinh nợ
    ///  -1  -> Phát sinh có
    /// </summary>
    public class ReportMethodItem
    {
        public short Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public ReportMethodItem() { }

        public ReportMethodItem(short id, string name)
        {
            Id = id;
            Name = name;
        }

        public override string ToString() => Name;
    }

    /// <summary>
    /// Helper tập trung quản lý các đối tượng Lookup dùng chung cho TẤT CẢ các Form Thiết kế báo cáo:
    /// (BalanceSheetDesign, CashFlowDesign, BusinessResultDesign, TaxObligationDesign, VatReturnDesign...).
    /// Giúp loại bỏ code lặp lại, hoạt động ổn định trong DataControlDetailDescriptor của GridControl.
    /// </summary>
    public static class DesignLookupHelper
    {
        private static readonly object _lock = new object();
        private static bool _isAccountsLoaded = false;

        /// <summary>
        /// Danh sách Phương pháp tính chuẩn dùng chung:
        ///  0  : Số dư
        ///  1  : Phát sinh nợ
        /// -1  : Phát sinh có
        /// </summary>
        public static List<ReportMethodItem> Methods { get; } = new List<ReportMethodItem>
        {
            new ReportMethodItem(0, "Số dư"),
            new ReportMethodItem(1, "Phát sinh nợ"),
            new ReportMethodItem(-1, "Phát sinh có")
        };

        /// <summary>
        /// Danh mục tài khoản dùng chung cho các màn hình thiết kế báo cáo.
        /// Sử dụng ObservableCollection để tự động cập nhật lên UI khi nạp dữ liệu.
        /// </summary>
        public static ObservableCollection<ChartOfAccountEntity> Accounts { get; } = new ObservableCollection<ChartOfAccountEntity>();

        /// <summary>
        /// Đảm bảo danh mục tài khoản đã được nạp từ CSDL vào bộ nhớ.
        /// </summary>
        public static void EnsureAccountsLoaded(bool forceReload = false)
        {
            if (_isAccountsLoaded && !forceReload) return;

            lock (_lock)
            {
                if (_isAccountsLoaded && !forceReload) return;

                try
                {
                    using var adapter = AppDataAccessAdapter.Create();
                    var list = new EntityCollection<ChartOfAccountEntity>();
                    adapter.FetchEntityCollection(list, null);

                    Accounts.Clear();
                    foreach (var acc in list.OrderBy(x => x.AccountId))
                    {
                        Accounts.Add(acc);
                    }
                    _isAccountsLoaded = true;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[DesignLookupHelper] Lỗi tải tài khoản: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Tạo ComboBoxEditSettings chuẩn cho cột Phương pháp tính (Method).
        /// </summary>
        public static ComboBoxEditSettings CreateMethodEditSettings()
        {
            return new ComboBoxEditSettings
            {
                ItemsSource = Methods,
                DisplayMember = nameof(ReportMethodItem.Name),
                ValueMember = nameof(ReportMethodItem.Id),
                AutoComplete = true,
                ImmediatePopup = true
            };
        }

        /// <summary>
        /// Tạo ComboBoxEditSettings chuẩn cho cột Tài khoản (kèm hiển thị Mã TK và Tên TK).
        /// </summary>
        public static ComboBoxEditSettings CreateAccountEditSettings()
        {
            EnsureAccountsLoaded();

            var settings = new ComboBoxEditSettings
            {
                ItemsSource = Accounts,
                DisplayMember = nameof(ChartOfAccountEntity.AccountId),
                ValueMember = nameof(ChartOfAccountEntity.AccountId),
                AutoComplete = true,
                ImmediatePopup = true
            };

            // Template hiển thị popup 2 cột: [Mã TK | Tên TK] từ file Lego XAML chung AccountTemplates.xaml
            try
            {
                if (Application.Current?.TryFindResource("AccountDropDownItemTemplate") is DataTemplate tpl)
                {
                    settings.ItemTemplate = tpl;
                }
            }
            catch
            {
                // Fallback nếu không nạp được resource
            }

            return settings;
        }
    }

    /// <summary>
    /// Cột GridColumn chọn Phương pháp tính (Method) dùng chung trong các form thiết kế báo cáo.
    /// Tự động gắn sẵn EditSettings (0: Số dư, 1: Phát sinh nợ, -1: Phát sinh có).
    /// </summary>
    public class MethodGridColumn : GridColumn
    {
        public MethodGridColumn()
        {
            Header = "Phương pháp";
            Width = 150;
            HorizontalHeaderContentAlignment = HorizontalAlignment.Center;
            EditSettings = DesignLookupHelper.CreateMethodEditSettings();
        }
    }

    /// <summary>
    /// Cột GridColumn chọn Tài khoản dùng chung trong các form thiết kế báo cáo.
    /// Tự động nạp danh mục tài khoản và gắn EditSettings với popup hỗ trợ xem Tên tài khoản.
    /// Không chứa bất kỳ action xử lý chứng từ nào, chuyên dụng cho màn hình Thiết kế.
    /// </summary>
    public class AccountGridColumn : GridColumn
    {
        public AccountGridColumn()
        {
            Header = "Tài khoản (*)";
            Width = 140;
            EditSettings = DesignLookupHelper.CreateAccountEditSettings();
        }
    }
}
