using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using DataAccess;
using DataAccess.EntityClasses;
using NewaccNet.Wpf.AppSystem.Helpers;

namespace NewaccNet.Wpf.AppSystem.CostTransfer
{
    public partial class CostTransferWindow : NewaccNet.Wpf.Views.Base.BaseWindow
    {
        // Chỉ cho chọn tài khoản loại I (không theo dõi chi tiết) và E (chi phí), và phải là tài khoản lá.
        private static readonly string[] AllowedCategories = { "I", "E" };

        private const string KeyEndDate = "CostTransfer_EndDate";
        private const string KeyLastSource = "CostTransfer_SourceAccount";
        private const string KeyDestPrefix = "CostTransfer_Dest_";        // theo tài khoản nguồn
        private const string KeyContentsPrefix = "CostTransfer_Contents_"; // theo tài khoản nguồn

        private readonly ICostTransferService _service = new CostTransferService();
        private readonly ObservableCollection<CostTransferRow> _rows = new ObservableCollection<CostTransferRow>();
        private bool _isInitializing = true;

        public CostTransferWindow()
        {
            InitializeComponent();
            gridRows.ItemsSource = _rows;

            Loaded += CostTransferWindow_Loaded;
            Closing += (s, e) => SaveSettings();
        }

        private void CostTransferWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _isInitializing = true;
            try
            {
                LoadLookups();
                LoadSettings();
            }
            finally
            {
                _isInitializing = false;
            }
        }

        private void LoadLookups()
        {
            try
            {
                using (var adapter = AppDataAccessAdapter.Create())
                {
                    var accounts = new EntityCollection<ChartOfAccountEntity>();
                    adapter.FetchEntityCollection(accounts, null);

                    var parentIds = accounts
                        .Select(a => a.ParentId)
                        .Where(id => !string.IsNullOrWhiteSpace(id))
                        .Select(id => id.Trim())
                        .ToHashSet(StringComparer.OrdinalIgnoreCase);

                    List<ChartOfAccountEntity> selectable = accounts
                        .Where(a => !string.IsNullOrWhiteSpace(a.AccountId)
                                    && !parentIds.Contains(a.AccountId.Trim())
                                    && AllowedCategories.Contains(a.CategoryId?.Trim().ToUpper()))
                        .OrderBy(a => a.AccountId)
                        .ToList();

                    lueSourceAccount.ItemsSource = selectable;
                    lueDestAccount.ItemsSource = selectable;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi nạp danh mục tài khoản: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ---- UserSettings: lần đầu nhập thì lưu, lần sau tự điền theo tài khoản nguồn ----

        private void LoadSettings()
        {
            try
            {
                string endStr = UserPreferencesHelper.GetSetting(KeyEndDate);
                dtEndDate.DateTime = !string.IsNullOrEmpty(endStr) && DateTime.TryParse(endStr, out var dt)
                    ? dt
                    : DateTime.Today;

                string src = UserPreferencesHelper.GetSetting(KeyLastSource);
                if (!string.IsNullOrEmpty(src))
                {
                    lueSourceAccount.EditValue = src;
                    ApplySettingsOfSource(src);
                }
            }
            catch { }
        }

        private void ApplySettingsOfSource(string sourceAccountId)
        {
            string dest = UserPreferencesHelper.GetSetting(KeyDestPrefix + sourceAccountId);
            lueDestAccount.EditValue = string.IsNullOrEmpty(dest) ? null : dest;

            string contents = UserPreferencesHelper.GetSetting(KeyContentsPrefix + sourceAccountId);
            txtContents.EditValue = string.IsNullOrEmpty(contents) ? null : contents;
        }

        private void SaveSettings()
        {
            if (_isInitializing) return;
            try
            {
                if (dtEndDate.DateTime != DateTime.MinValue)
                    UserPreferencesHelper.SetSetting(KeyEndDate, dtEndDate.DateTime.ToString("yyyy-MM-dd"));

                string src = lueSourceAccount.EditValue?.ToString()?.Trim();
                if (string.IsNullOrEmpty(src)) return;

                UserPreferencesHelper.SetSetting(KeyLastSource, src);

                string dest = lueDestAccount.EditValue?.ToString()?.Trim();
                if (!string.IsNullOrEmpty(dest))
                    UserPreferencesHelper.SetSetting(KeyDestPrefix + src, dest);

                string contents = txtContents.EditValue?.ToString();
                if (!string.IsNullOrEmpty(contents))
                    UserPreferencesHelper.SetSetting(KeyContentsPrefix + src, contents);
            }
            catch { }
        }

        // ---- Sự kiện tham số ----

        private void SourceAccount_Changed(object sender, EventArgs e)
        {
            if (_isInitializing) return;

            string src = lueSourceAccount.EditValue?.ToString()?.Trim();
            _isInitializing = true; // tránh Param_Changed ghi đè khi đang nạp thiết lập của tài khoản mới
            try
            {
                if (!string.IsNullOrEmpty(src)) ApplySettingsOfSource(src);
            }
            finally
            {
                _isInitializing = false;
            }
            ResetCalculationState();
        }

        private void Param_Changed(object sender, EventArgs e)
        {
            if (_isInitializing) return;
            SaveSettings();
            ResetCalculationState();
        }

        private void ResetCalculationState()
        {
            _rows.Clear();
            btnTransfer.IsEnabled = false;
            lblStatus.Text = "Tham số đã thay đổi. Vui lòng bấm 'Tính toán'.";
        }

        private CostTransferParams BuildParams() => new CostTransferParams
        {
            EndDate = dtEndDate.DateTime,
            SourceAccountId = lueSourceAccount.EditValue?.ToString()?.Trim() ?? string.Empty,
            DestinationAccountId = lueDestAccount.EditValue?.ToString()?.Trim() ?? string.Empty,
            Contents = txtContents.EditValue?.ToString() ?? string.Empty
        };

        // ---- Nút ----

        private async void BtnCalculate_Click(object sender, RoutedEventArgs e)
        {
            var p = BuildParams();
            if (string.IsNullOrWhiteSpace(p.SourceAccountId))
            {
                MessageBox.Show("Vui lòng chọn Tài khoản nguồn.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(p.DestinationAccountId))
            {
                MessageBox.Show("Vui lòng chọn Tài khoản đích.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            SaveSettings();

            try
            {
                btnCalculate.IsEnabled = false;
                lblStatus.Text = "Đang tính số dư chi phí...";

                var result = await _service.CalculateAsync(p);

                _rows.Clear();
                foreach (var row in result)
                {
                    row.PropertyChanged += Row_PropertyChanged;
                    _rows.Add(row);
                }

                if (_rows.Count == 0)
                    lblStatus.Text = $"Tài khoản {p.SourceAccountId} không còn số dư chi phí đến {p.EndDate:dd/MM/yyyy}.";
                else
                    UpdateSummary();
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Lỗi tính toán: " + ex.Message;
                MessageBox.Show("Lỗi tính toán: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                btnCalculate.IsEnabled = true;
            }
        }

        private void BtnSelectAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (var r in _rows) r.Selected = true;
        }

        private void BtnSelectNone_Click(object sender, RoutedEventArgs e)
        {
            foreach (var r in _rows) r.Selected = false;
        }

        private void Row_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            // Chỉ các thay đổi ảnh hưởng đến số kết chuyển mới cần cập nhật tổng
            if (e.PropertyName == nameof(CostTransferRow.Selected)
                || e.PropertyName == nameof(CostTransferRow.Keep)
                || e.PropertyName == nameof(CostTransferRow.TransferAmount))
            {
                UpdateSummary();
            }
        }

        private void UpdateSummary()
        {
            var chosen = _rows.Where(r => r.Selected).ToList();
            lblStatus.Text = $"{_rows.Count} dòng | Tổng số dư: {_rows.Sum(r => r.Rest):#,##0} | " +
                             $"Đã chọn {chosen.Count} dòng, số kết chuyển: {chosen.Sum(r => r.TransferAmount):#,##0}";
        }

        private void BtnTransfer_Click(object sender, RoutedEventArgs e)
        {
            // TODO bước 4: tạo chứng từ kết chuyển từ các dòng được chọn
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
