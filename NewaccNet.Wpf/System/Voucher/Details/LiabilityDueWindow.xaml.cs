using System;
using System.Linq;
using System.Windows;
using System.ComponentModel;
using SD.LLBLGen.Pro.ORMSupportClasses;
using SD.LLBLGen.Pro.QuerySpec;
using DataAccess.EntityClasses;
using DataAccess.HelperClasses;
using DataAccess.FactoryClasses;
using NewaccNet.Wpf.AppSystem;

namespace NewaccNet.Wpf.AppSystem.Voucher.Details
{
    public partial class LiabilityDueWindow : DevExpress.Xpf.Core.ThemedWindow
    {
        private LiabilityDueEntity _currentEntity;
        private string _partnerId;
        private string _debtTypeId;
        private bool _isDirty = false;

        public LiabilityDueWindow(string partnerId, string debtTypeId)
        {
            InitializeComponent();
            _partnerId = partnerId?.Trim();
            _debtTypeId = debtTypeId?.Trim();
            
            LoadData();
            this.Closing += LiabilityDueWindow_Closing;
        }

        private void LoadData()
        {
            try
            {
                using (var adapter = AppDataAccessAdapter.Create())
                {
                    var partner = new PartnerEntity(_partnerId);
                    if (adapter.FetchEntity(partner)) txtPartnerName.Text = partner.CustomerName;

                    var debtType = new DebtTypeEntity(_debtTypeId);
                    if (adapter.FetchEntity(debtType)) txtDebtTypeName.Text = debtType.TypeName;

                    var filter = new RelationPredicateBucket();
                    filter.PredicateExpression.Add(LiabilityDueFields.CustomerId == _partnerId);
                    filter.PredicateExpression.AddWithAnd(LiabilityDueFields.DebtTypeId == _debtTypeId);
                    
                    var coll = new EntityCollection<LiabilityDueEntity>();
                    adapter.FetchEntityCollection(coll, filter, 1);
                    
                    _currentEntity = coll.FirstOrDefault();

                    if (_currentEntity == null)
                    {
                        _currentEntity = new LiabilityDueEntity();
                        _currentEntity.CustomerId = _partnerId;
                        _currentEntity.DebtTypeId = _debtTypeId;
                        _currentEntity.Settledflag = false;
                        _isDirty = true; // New entity needs saving
                    }

                    _currentEntity.PropertyChanged += (s, e) => { _isDirty = true; };

                    this.DataContext = _currentEntity;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu cam kết: " + ex.Message);
            }
        }

        private void LiabilityDueWindow_Closing(object sender, CancelEventArgs e)
        {
            if (_isDirty)
            {
                var result = MessageBox.Show("Dữ liệu chưa được lưu. Bạn có muốn lưu lại trước khi thoát không?", "Cảnh báo", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    if (!SaveData())
                    {
                        e.Cancel = true; // Lỗi lưu thì không thoát
                    }
                }
                else if (result == MessageBoxResult.Cancel)
                {
                    e.Cancel = true; // Hủy lệnh thoát
                }
            }
        }

        private bool SaveData()
        {
            try
            {
                using (var adapter = AppDataAccessAdapter.Create())
                {
                    // Fix LLBLGen save for new entity
                    _currentEntity.IsDirty = true; 
                    bool success = adapter.SaveEntity(_currentEntity, true);
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

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (SaveData())
            {
                MessageBox.Show("Đã lưu thông tin thời hạn công nợ thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}