using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Grid;
using NewaccNet.Core.Data;

namespace NewaccNet.Wpf.AppSystem.Voucher
{
    /// <summary>
    /// Control chọn tài khoản theo khung mapping thân thiện cho người phi kế toán (ERP Mapped Account LookUp).
    /// Kế thừa 100% hành vi cốt lõi từ AccountLookUpEdit (tự nhận diện CategoryId nhóm A: công nợ, nhóm E: chi phí...
    /// và tự kích hoạt nút chi tiết '...' mở form con chuẩn của Accounting-first).
    /// 
    /// Điểm khác biệt:
    /// - Co hẹp danh sách tài khoản theo cấu hình Template (VoucherPostingConfig).
    /// - Hiển thị tên nghiệp vụ đời thường (DisplayMember = "Name") thay vì số hiệu tài khoản.
    /// - Trong suốt đối với người dùng ERP nhưng bên dưới vẫn gắn chặt với AccountId chuẩn kế toán.
    /// </summary>
    public class ErpMappedAccountLookUpEdit : AccountLookUpEdit
    {
        #region Dependency Properties
        public static readonly DependencyProperty MappedOptionsProperty =
            DependencyProperty.Register(
                nameof(MappedOptions),
                typeof(IEnumerable),
                typeof(ErpMappedAccountLookUpEdit),
                new FrameworkPropertyMetadata(null, OnMappedOptionsChanged));

        public IEnumerable? MappedOptions
        {
            get => (IEnumerable?)GetValue(MappedOptionsProperty);
            set => SetValue(MappedOptionsProperty, value);
        }

        private static void OnMappedOptionsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ErpMappedAccountLookUpEdit editor)
            {
                editor.ItemsSource = e.NewValue;
                // Nếu chưa có giá trị được chọn, chọn mục đầu tiên làm mặc định
                if (editor.EditValue == null && e.NewValue is IEnumerable list)
                {
                    var enumerator = list.GetEnumerator();
                    if (enumerator.MoveNext() && enumerator.Current != null)
                    {
                        var prop = enumerator.Current.GetType().GetProperty(editor.ValueMember);
                        if (prop != null)
                        {
                            editor.EditValue = prop.GetValue(enumerator.Current);
                        }
                    }
                }
            }
        }
        #endregion

        #region Popup Template
        private static readonly Lazy<ControlTemplate> _mappedPopupTemplate = new Lazy<ControlTemplate>(() =>
        {
            string xaml = @"
                <ControlTemplate xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
                                 xmlns:dx=""http://schemas.devexpress.com/winfx/2008/xaml/core""
                                 xmlns:dxg=""http://schemas.devexpress.com/winfx/2008/xaml/grid"">
                    <dxg:GridControl Name=""PART_GridControl"">
                        <dxg:GridControl.Columns>
                            <dxg:GridColumn FieldName=""Name"" Header=""Hình thức / Mục đích"" Width=""*""/>
                        </dxg:GridControl.Columns>
                        <dxg:GridControl.View>
                            <dxg:TableView ShowGroupPanel=""False"" AutoWidth=""True"" ShowColumnHeaders=""False""/>
                        </dxg:GridControl.View>
                    </dxg:GridControl>
                </ControlTemplate>";
            return (ControlTemplate)System.Windows.Markup.XamlReader.Parse(xaml);
        });
        #endregion

        public ErpMappedAccountLookUpEdit()
        {
            // Cấu hình hiển thị theo tên nghiệp vụ thân thiện
            this.DisplayMember = "Name";
            this.ValueMember = "AccountId";
            this.PopupContentTemplate = _mappedPopupTemplate.Value;
            this.PopupWidth = 320;
            this.AutoComplete = true;
            this.ImmediatePopup = true;
            this.IsTextEditable = false; // Người dùng ERP chỉ chọn trong danh mục được cấu hình, không gõ bừa

            // Đăng ký sự kiện cập nhật giao diện nút chi tiết khi đổi lựa chọn
            this.EditValueChanged += ErpMappedAccountLookUpEdit_EditValueChanged;
        }

        private void ErpMappedAccountLookUpEdit_EditValueChanged(object sender, EditValueChangedEventArgs e)
        {
            UpdateDetailButtonStatus(e.NewValue?.ToString());
        }

        /// <summary>
        /// Tự động điều chỉnh tooltip và khả năng bật/tắt của nút chi tiết '...' dựa trên Category của tài khoản base.
        /// </summary>
        public void UpdateDetailButtonStatus(string? accountId)
        {
            if (string.IsNullOrEmpty(accountId)) return;

            string cat = GetCategoryForAccount(accountId);
            bool hasDetail = !string.IsNullOrEmpty(cat) && cat != "I";

            // Tìm nút chi tiết trong collection Buttons
            var btn = this.Buttons.OfType<ButtonInfo>().FirstOrDefault(b => b.Content?.ToString() == "...");
            if (btn != null)
            {
                btn.Visibility = hasDetail ? Visibility.Visible : Visibility.Collapsed;
                btn.IsEnabled = hasDetail;

                if (cat == "A")
                {
                    btn.ToolTip = "Xem / Nhập chi tiết công nợ (hạn nợ, đối tác, điều khoản thanh toán)";
                }
                else if (cat == "E")
                {
                    btn.ToolTip = "Xem / Nhập chi tiết đối tượng chi phí, công trình, khoản mục chi phí";
                }
                else if (cat == "B")
                {
                    btn.ToolTip = "Xem / Nhập chi tiết vật tư, hàng hóa";
                }
                else
                {
                    btn.ToolTip = "Chi tiết tài khoản (Mở form phụ)";
                }
            }
        }

        /// <summary>
        /// Nạp nhanh cấu hình danh sách lựa chọn từ một danh sách PaymentMethodOptionDto
        /// </summary>
        public void SetOptions(IEnumerable<PaymentMethodOptionDto> options, string? defaultAccountId = null)
        {
            var list = options?.ToList() ?? new List<PaymentMethodOptionDto>();
            this.ItemsSource = list;

            if (!string.IsNullOrEmpty(defaultAccountId))
            {
                this.EditValue = defaultAccountId;
            }
            else if (list.Count > 0)
            {
                this.EditValue = list[0].AccountId;
            }
            UpdateDetailButtonStatus(this.EditValue?.ToString());
        }
    }
}
