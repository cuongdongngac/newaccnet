using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using SD.LLBLGen.Pro.ORMSupportClasses;
using DataAccess.EntityClasses;
using DataAccess.HelperClasses;
using NewaccNet.Wpf.AppSystem;

namespace NewaccNet.Wpf.AppSystem.Voucher.Details
{
    public partial class LiabilityDueWindow : NewaccNet.Wpf.Views.Base.BaseDetailWindow
    {
        private LiabilityDueEntity _currentEntity;
        private string _partnerId;
        private string _debtTypeId;
        private bool _isDirty = false;
        
        public bool IsDeleted { get; private set; } = false;

        /// <summary>
        /// Id của LiabilityDue sau khi lưu thành công.
        /// Caller (DebtDetailWindow) dùng giá trị này để gán vào DebtDetail.LiabilityDuesId.
        /// </summary>
        public int? SavedId => _currentEntity?.IsNew == false ? _currentEntity.Id : (int?)null;

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            Delete();
        }

        public override void Delete()
        {
            if (MessageBox.Show("Bạn có chắc muốn xóa cam kết/lãi suất này?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                if (_currentEntity != null && !_currentEntity.IsNew)
                {
                    try
                    {
                        using (var adapter = AppDataAccessAdapter.Create())
                        {
                            adapter.DeleteEntity(_currentEntity);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi khi xóa: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
                
                IsDeleted = true;
                _currentEntity = null;
                this.DialogResult = true;
                this.Close();
            }
        }

        // ── Constructor 1: Mở bằng Id đã biết (DebtDetail đã có LiabilityDuesId) ──
        public LiabilityDueWindow(int liabilityDueId)
        {
            InitializeComponent();
            LoadById(liabilityDueId);
            this.Closing += LiabilityDueWindow_Closing;
        }

        // ── Constructor 2: Mở bằng (PartnerId, DebtTypeId) - khi chưa có liên kết ──
        public LiabilityDueWindow(string partnerId, string debtTypeId)
        {
            InitializeComponent();
            _partnerId = partnerId?.Trim();
            _debtTypeId = debtTypeId?.Trim();
            LoadByKeys();
            this.Closing += LiabilityDueWindow_Closing;
        }

        // ── Load theo Id (nhanh, chắc chắn đúng bản ghi) ──
        private void LoadById(int id)
        {
            try
            {
                using (var adapter = AppDataAccessAdapter.Create())
                {
                    _currentEntity = new LiabilityDueEntity(id);
                    if (!adapter.FetchEntity(_currentEntity))
                    {
                        MessageBox.Show("Không tìm thấy thông tin cam kết! Dữ liệu có thể đã bị xóa.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                        _currentEntity = new LiabilityDueEntity();
                    }
                    else
                    {
                        // Hiển thị tên Đối tượng & Nội dung từ FK
                        _partnerId = _currentEntity.CustomerId;
                        _debtTypeId = _currentEntity.DebtTypeId;
                        LoadNames(adapter);
                    }

                    _currentEntity.PropertyChanged += (s, e) => { _isDirty = true; };
                    this.DataContext = _currentEntity;
                    this.Loaded += (s, e2) => PopulateNumericFields();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu cam kết: " + ex.Message);
            }
        }

        // ── Load theo (PartnerId, DebtTypeId) - query thư viện ──
        private void LoadByKeys()
        {
            try
            {
                using (var adapter = AppDataAccessAdapter.Create())
                {
                    LoadNames(adapter);

                    var filter = new RelationPredicateBucket();
                    filter.PredicateExpression.Add(LiabilityDueFields.CustomerId == _partnerId);
                    filter.PredicateExpression.AddWithAnd(LiabilityDueFields.DebtTypeId == _debtTypeId);

                    var coll = new EntityCollection<LiabilityDueEntity>();
                    adapter.FetchEntityCollection(coll, filter, 1);

                    _currentEntity = coll.FirstOrDefault();

                    if (_currentEntity == null)
                    {
                        // Bản ghi chưa tồn tại → tạo mới
                        _currentEntity = new LiabilityDueEntity();
                        _currentEntity.CustomerId = _partnerId;
                        _currentEntity.DebtTypeId = _debtTypeId;
                        _currentEntity.Settledflag = false;
                        _isDirty = true;
                    }

                    _currentEntity.PropertyChanged += (s, e) => { _isDirty = true; };
                    this.DataContext = _currentEntity;
                    // TextBox không bind Nullable<double> nên fill thủ công
                    this.Loaded += (s, e2) => PopulateNumericFields();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu cam kết: " + ex.Message);
            }
        }

        private void LoadNames(IDataAccessAdapter adapter)
        {
            if (!string.IsNullOrEmpty(_partnerId))
            {
                var partner = new PartnerEntity(_partnerId);
                if (adapter.FetchEntity(partner)) txtPartnerName.Text = partner.CustomerName;
            }
            if (!string.IsNullOrEmpty(_debtTypeId))
            {
                var debtType = new DebtTypeEntity(_debtTypeId);
                if (adapter.FetchEntity(debtType)) txtDebtTypeName.Text = debtType.TypeName;
            }
        }

        // ── Populate plain TextBox sau khi entity được load ──
        // (TextBox không data-bind được vào Nullable<double> trực tiếp nên ta fill thủ công)
        private void PopulateNumericFields()
        {
            txtInterestrate.Text = _currentEntity.Interestrate.HasValue ? _currentEntity.Interestrate.Value.ToString("N2") : "";
            txtFeedbackrate.Text = _currentEntity.Feedbackrate.HasValue ? _currentEntity.Feedbackrate.Value.ToString("N2") : "";
            txtOutvatrate.Text   = _currentEntity.Outvatrate.HasValue   ? _currentEntity.Outvatrate.Value.ToString("N2")   : "";
        }

        // ── Parse số khi user rời khỏi ô TextBox ──
        public void NumericField_LostFocus(object sender, RoutedEventArgs e)
        {
            if (_currentEntity == null) return;
            var tb = sender as TextBox;
            if (tb == null) return;

            double val;
            bool ok = double.TryParse(tb.Text.Replace(",", "").Replace("%", "").Trim(), 
                                      NumberStyles.Any, CultureInfo.InvariantCulture, out val);
            
            switch (tb.Name)
            {
                case "txtInterestrate":  _currentEntity.Interestrate = ok ? val : (double?)null; break;
                case "txtFeedbackrate":  _currentEntity.Feedbackrate = ok ? val : (double?)null; break;
                case "txtOutvatrate":    _currentEntity.Outvatrate   = ok ? val : (double?)null; break;
            }
            _isDirty = true;
            // Format lại ô sau khi parse xong
            if (ok) tb.Text = val.ToString("N2");
        }

        private void LiabilityDueWindow_Closing(object sender, CancelEventArgs e)
        {
            if (_isDirty)
            {
                var result = MessageBox.Show("Dữ liệu chưa được lưu. Bạn có muốn lưu lại trước khi thoát không?", "Cảnh báo", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    if (!SaveData()) e.Cancel = true;
                }
                else if (result == MessageBoxResult.Cancel)
                {
                    e.Cancel = true;
                }
            }
        }

        private bool SaveData()
        {
            try
            {
                using (var adapter = AppDataAccessAdapter.Create())
                {
                    _currentEntity.IsDirty = true;
                    bool success = adapter.SaveEntity(_currentEntity, refetchAfterSave: true);
                    if (success) _isDirty = false;
                    return success;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi lưu dữ liệu: " + ex.Message);
                return false;
            }
        }

        private void BtnAccept_Click(object sender, RoutedEventArgs e)
        {
            if (SaveData())
            {
                MessageBox.Show("Đã lưu thông tin thời hạn công nợ thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                _isDirty = false; // Đánh dấu đã lưu sạch, tránh hỏi lại khi đóng
                this.Close();
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}