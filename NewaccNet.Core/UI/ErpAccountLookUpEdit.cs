using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Data.Filtering;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Grid;
using NewaccNet.Core.Data;

namespace NewaccNet.Core.UI
{
    /// <summary>
    /// Control chọn tài khoản chuyên biệt cho Plugin ERP Kho và các phân hệ mở rộng:
    /// - Kế thừa 100% từ AccountLookUpEditBase: có JournalEntry (bind 2 chiều với JournalEntryEntity).
    /// - Hiển thị CHỈ TÊN AccountName (DisplayMember = "AccountName") sau khi chọn, thân thiện cho người dùng ERP.
    /// - Hỗ trợ SEARCH THEO TÊN và theo MÃ TK (IsTextEditable = true, IncrementalFiltering = true, SubstituteDisplayFilter).
    /// - Dropdown hiển thị rõ ràng Mã TK và Tên tài khoản, tự động co giãn.
    /// - Tự động chọn tài khoản IsDefault và đồng bộ vào JournalEntry để logic Save/Delete hoạt động chuẩn xác theo Accounting First.
    /// - Tự động cấm mở chi tiết con với tài khoản vật tư B (DisabledDetailCategories = "B" hoặc AllowDetail = false).
    /// </summary>
    public class ErpAccountLookUpEdit : AccountLookUpEditBase
    {
        #region Popup Content Template
        private static readonly Lazy<ControlTemplate> _erpPopupTemplate = new Lazy<ControlTemplate>(() =>
        {
            string xaml = @"
                <ControlTemplate xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
                                 xmlns:dx=""http://schemas.devexpress.com/winfx/2008/xaml/core""
                                 xmlns:dxg=""http://schemas.devexpress.com/winfx/2008/xaml/grid"">
                    <dxg:GridControl Name=""PART_GridControl"" MinWidth=""450"">
                        <dxg:GridControl.Columns>
                            <dxg:GridColumn FieldName=""AccountId"" Header=""Số TK"" Width=""85"" FixedWidth=""True""/>
                            <dxg:GridColumn FieldName=""AccountName"" Header=""Tên tài khoản / Nghiệp vụ ERP"" Width=""*"" MinWidth=""220""/>
                        </dxg:GridControl.Columns>
                        <dxg:GridControl.View>
                            <dxg:TableView ShowGroupPanel=""False"" AutoWidth=""True"" ShowColumnHeaders=""True"" RowMinHeight=""26""/>
                        </dxg:GridControl.View>
                    </dxg:GridControl>
                </ControlTemplate>";
            return (ControlTemplate)System.Windows.Markup.XamlReader.Parse(xaml);
        });
        #endregion

        public ErpAccountLookUpEdit()
        {
            // Hiển thị tên tài khoản (AccountName) sau khi chọn
            this.DisplayMember = "AccountName";
            this.ValueMember = "AccountId";
            this.PopupContentTemplate = _erpPopupTemplate.Value;
            this.PopupWidth = 460;
            this.PopupMinWidth = 420;

            // Hỗ trợ tìm kiếm theo tên và theo mã
            this.IsTextEditable = true;
            this.AutoComplete = false;
            this.ImmediatePopup = true;
            this.IncrementalFiltering = true;
            this.FilterCondition = DevExpress.Data.Filtering.FilterCondition.Contains;

            // Mặc định cấm mở form chi tiết cho tài khoản vật tư (nhóm B) trên các form kho
            this.DisabledDetailCategories = "B";

            this.SubstituteDisplayFilter += ErpAccountLookUpEdit_SubstituteDisplayFilter;
        }

        private void ErpAccountLookUpEdit_SubstituteDisplayFilter(object? sender, SubstituteDisplayFilterEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(e.DisplayText)) return;
            string raw = e.DisplayText.Trim();

            // Tìm kiếm đa năng: khớp chuỗi chứa trong Tên tài khoản hoặc bắt đầu bằng Mã TK
            e.DisplayFilter = new GroupOperator(
                GroupOperatorType.Or,
                new FunctionOperator(FunctionOperatorType.Contains,
                    new OperandProperty("AccountName"),
                    new ConstantValue(raw)),
                new FunctionOperator(FunctionOperatorType.StartsWith,
                    new OperandProperty("AccountId"),
                    new ConstantValue(raw))
            );
            e.Handled = true;
        }

        public override void RefreshItemsSource()
        {
            // Đối với ErpAccountLookUpEdit, ItemsSource do từng slot nạp cụ thể
        }

        /// <summary>
        /// Nạp danh sách tài khoản theo Slot từ cấu hình DTO, tự động gán giá trị ngầm định
        /// </summary>
        public void SetSlotOptions(IEnumerable<VoucherPostingAccountOptionDto>? options, string? defaultAccountId = null)
        {
            var list = options?.ToList() ?? new List<VoucherPostingAccountOptionDto>();
            this.ItemsSource = list;

            string? targetAccId = defaultAccountId;
            if (string.IsNullOrEmpty(targetAccId))
            {
                var def = list.FirstOrDefault(x => x.IsDefault);
                if (def != null && !string.IsNullOrEmpty(def.AccountId))
                {
                    targetAccId = def.AccountId;
                }
                else if (list.Count > 0)
                {
                    targetAccId = list[0].AccountId;
                }
            }

            if (!string.IsNullOrEmpty(targetAccId))
            {
                this.EditValue = targetAccId;
                if (this.JournalEntry != null)
                {
                    this.JournalEntry.AccountId = targetAccId;
                }
            }

            UpdateAccountAppearance(this.EditValue?.ToString());
        }

        #region Kiểm soát Chi tiết Tài khoản & Nhắc nhở Dòng Đỏ (Accounting First)
        public bool RequiresDetail { get; private set; }
        public bool HasDetail { get; private set; }
        public string? DetailCategory { get; private set; }
        public string? DetailCategoryName { get; private set; }

        /// <summary>
        /// Cho phép tầng Form bên ngoài cung cấp điều kiện kiểm tra chi tiết bổ sung (VD: 331/131 đã chọn Partner trên Header)
        /// </summary>
        public Func<bool>? ExternalDetailChecker { get; set; }

        public event EventHandler? DetailStatusChanged;

        private TextBlock? _reminderTextBlock;
        /// <summary>
        /// TextBlock nhắc nhở dòng đỏ bên dưới ô chọn tài khoản
        /// </summary>
        public TextBlock? ReminderTextBlock
        {
            get => _reminderTextBlock;
            set
            {
                if (_reminderTextBlock != null)
                {
                    _reminderTextBlock.MouseLeftButtonUp -= ReminderTextBlock_MouseLeftButtonUp;
                }
                _reminderTextBlock = value;
                if (_reminderTextBlock != null)
                {
                    _reminderTextBlock.MouseLeftButtonUp += ReminderTextBlock_MouseLeftButtonUp;
                    UpdateDetailStatus();
                }
            }
        }

        private void ReminderTextBlock_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            TriggerDetailForm();
        }

        public override void UpdateAccountAppearance(string? accountId)
        {
            base.UpdateAccountAppearance(accountId);
            UpdateDetailStatus();
        }

        protected override void OnDetailWindowClosed(bool success)
        {
            base.OnDetailWindowClosed(success);
            UpdateDetailStatus();
        }

        /// <summary>
        /// Cập nhật trạng thái chi tiết kế toán, hiển thị dòng đỏ nhắc nhở và viền đỏ nếu thiếu chi tiết
        /// </summary>
        public void UpdateDetailStatus()
        {
            string? accId = this.EditValue?.ToString();
            if (string.IsNullOrEmpty(accId))
            {
                RequiresDetail = false;
                HasDetail = false;
                DetailCategory = null;
                DetailCategoryName = null;
                UpdateVisualReminder();
                DetailStatusChanged?.Invoke(this, EventArgs.Empty);
                return;
            }

            RequiresDetail = IsDetailAccountForControl(accId);
            if (!RequiresDetail)
            {
                HasDetail = false;
                DetailCategory = null;
                DetailCategoryName = null;
                UpdateVisualReminder();
                DetailStatusChanged?.Invoke(this, EventArgs.Empty);
                return;
            }

            string cat = GetCategoryForAccount(accId);
            DetailCategory = cat;
            DetailCategoryName = cat switch
            {
                "A" => "Công nợ",
                "B" => "Vật tư, kho",
                "C" => "Ngoại tệ",
                "D" => "Tài sản cố định",
                "E" => "Khoản mục / Chi phí",
                "S" => "Cổ phiếu / Đầu tư",
                _ => "Chi tiết"
            };

            HasDetail = CheckHasDetail(cat);
            UpdateVisualReminder();
            DetailStatusChanged?.Invoke(this, EventArgs.Empty);
        }

        private bool CheckHasDetail(string cat)
        {
            if (ExternalDetailChecker != null && ExternalDetailChecker())
            {
                return true;
            }

            var entry = GetCurrentEntry();
            if (entry == null) return false;

            return cat switch
            {
                "E" => entry.CostDetails != null && entry.CostDetails.Count > 0,
                "A" => entry.DebtDetails != null && entry.DebtDetails.Count > 0,
                "C" => entry.CurrencyDetails != null && entry.CurrencyDetails.Count > 0,
                "D" => entry.AssetDetails != null && entry.AssetDetails.Count > 0,
                "S" => entry.InvestmentDetails != null && entry.InvestmentDetails.Count > 0,
                "B" => (entry.InventoryVouchers != null && entry.InventoryVouchers.Count > 0) || entry.IsMaterialBound,
                _ => true
            };
        }

        public int GetDetailCount()
        {
            var entry = GetCurrentEntry();
            if (entry == null) return 0;
            string cat = DetailCategory ?? GetCategoryForAccount(this.EditValue?.ToString());

            return cat switch
            {
                "E" => entry.CostDetails?.Count ?? 0,
                "A" => entry.DebtDetails?.Count ?? 0,
                "C" => entry.CurrencyDetails?.Count ?? 0,
                "D" => entry.AssetDetails?.Count ?? 0,
                "S" => entry.InvestmentDetails?.Count ?? 0,
                "B" => entry.InventoryVouchers?.Count ?? 0,
                _ => 0
            };
        }

        private void UpdateVisualReminder()
        {
            if (RequiresDetail)
            {
                if (!HasDetail)
                {
                    // Thiếu chi tiết: Viền dưới đỏ và dòng chữ đỏ nhắc nhở
                    this.BorderBrush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(211, 47, 47)); // #D32F2F
                    this.BorderThickness = new Thickness(1, 1, 1, 2);

                    if (ReminderTextBlock != null)
                    {
                        ReminderTextBlock.Visibility = Visibility.Visible;
                        ReminderTextBlock.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(211, 47, 47));
                        ReminderTextBlock.FontWeight = FontWeights.Normal;
                        ReminderTextBlock.Text = $"* Yêu cầu nhập chi tiết {DetailCategoryName} (Bấm [...] để nhập)";
                        ReminderTextBlock.ToolTip = $"Tài khoản '{EditValue}' yêu cầu chi tiết {DetailCategoryName}. Bấm vào đây hoặc nút [...] để mở bảng chi tiết.";
                        ReminderTextBlock.Cursor = System.Windows.Input.Cursors.Hand;
                    }
                }
                else
                {
                    // Đã có chi tiết: Trả lại viền chuẩn và hiển thị xác nhận màu xanh lá
                    this.ClearValue(BorderBrushProperty);
                    this.ClearValue(BorderThicknessProperty);

                    if (ReminderTextBlock != null)
                    {
                        ReminderTextBlock.Visibility = Visibility.Visible;
                        ReminderTextBlock.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(46, 125, 50)); // #2E7D32
                        ReminderTextBlock.FontWeight = FontWeights.Normal;
                        int count = GetDetailCount();
                        ReminderTextBlock.Text = count > 0
                            ? $"✓ Đã nhập chi tiết {DetailCategoryName} ({count} dòng)"
                            : $"✓ Đã có chi tiết {DetailCategoryName}";
                        ReminderTextBlock.ToolTip = $"Đã có dữ liệu chi tiết cho tài khoản '{EditValue}'. Bấm vào đây hoặc nút [...] để xem/sửa.";
                        ReminderTextBlock.Cursor = System.Windows.Input.Cursors.Hand;
                    }
                }
            }
            else
            {
                // Không yêu cầu chi tiết: Xóa viền và ẩn nhắc nhở
                this.ClearValue(BorderBrushProperty);
                this.ClearValue(BorderThicknessProperty);

                if (ReminderTextBlock != null)
                {
                    ReminderTextBlock.Visibility = Visibility.Collapsed;
                    ReminderTextBlock.Text = "";
                    ReminderTextBlock.ToolTip = null;
                }
            }
        }

        /// <summary>
        /// Kiểm soát hợp lệ chi tiết kế toán trước khi lưu phiếu.
        /// Trả về false và cung cấp warningMessage nếu tài khoản thiếu chi tiết bắt buộc.
        /// </summary>
        public bool ValidateDetailRequirement(out string? warningMessage)
        {
            UpdateDetailStatus();
            if (RequiresDetail && !HasDetail)
            {
                var acc = GetLeafAccounts()?.FirstOrDefault(a => a.AccountId == this.EditValue?.ToString());
                string accDisplay = acc != null ? $"{acc.AccountId} - {acc.AccountName}" : (this.EditValue?.ToString() ?? "");
                warningMessage = $"Tài khoản '{accDisplay}' yêu cầu phải có dữ liệu chi tiết ({DetailCategoryName}) nhưng hiện tại chưa được nhập.\n\nVui lòng bấm vào nút [...] trên ô tài khoản hoặc dòng chữ màu đỏ bên dưới để nhập chi tiết trước khi lưu phiếu.";
                return false;
            }
            warningMessage = null;
            return true;
        }
        #endregion
    }
}
