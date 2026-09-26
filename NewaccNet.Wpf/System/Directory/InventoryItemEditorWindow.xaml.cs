using System.Windows;
using DataAccess.EntityClasses;
using DataAccess.HelperClasses;
using SD.LLBLGen.Pro.ORMSupportClasses;
using NewaccNet.Wpf.Views.Base;

namespace NewaccNet.Wpf.AppSystem.Directory
{
    public partial class InventoryItemEditorWindow : BaseWindow
    {
        private InventoryItemEntity _entity;
        private bool _isNew;

        public InventoryItemEditorWindow(InventoryItemEntity entity, bool isNew)
        {
            InitializeComponent();
            _entity = entity;
            _isNew = isNew;

            LoadCategories();
            BindData();
        }

        private void LoadCategories()
        {
            try
            {
                using (var adapter = AppDataAccessAdapter.Create())
                {
                    var categories = new EntityCollection<CategoryEntity>();
                    adapter.FetchEntityCollection(categories, null);
                    cboCategory.ItemsSource = categories;
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Lỗi tải danh mục nhóm vật tư: " + ex.Message);
            }
        }

        private void BindData()
        {
            if (!_isNew)
            {
                txtId.Text = _entity.MaterialId;
                txtId.IsReadOnly = true; 
            }

            txtName.Text = _entity.MaterialName;
            txtUnit.Text = _entity.Unit;
            cboCategory.EditValue = _entity.Categoryid;

            txtId.Focus();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtId.Text))
            {
                MessageBox.Show("Vui lòng nhập Mã vật tư.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtId.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Vui lòng nhập Tên vật tư.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtName.Focus();
                return;
            }

            _entity.MaterialId = txtId.Text.Trim();
            _entity.MaterialName = txtName.Text.Trim();
            _entity.Unit = txtUnit.Text.Trim();
            
            if (cboCategory.EditValue != null)
                _entity.Categoryid = cboCategory.EditValue.ToString();
            else
                _entity.Categoryid = null;

            this.DialogResult = true;
            this.Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
