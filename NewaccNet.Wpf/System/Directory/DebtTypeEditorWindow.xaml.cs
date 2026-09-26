using System.Windows;
using DataAccess.EntityClasses;
using NewaccNet.Wpf.Views.Base;

namespace NewaccNet.Wpf.AppSystem.Directory
{
    public partial class DebtTypeEditorWindow : BaseWindow
    {
        private DebtTypeEntity _entity;
        private bool _isNew;

        public DebtTypeEditorWindow(DebtTypeEntity entity, bool isNew)
        {
            InitializeComponent();
            _entity = entity;
            _isNew = isNew;

            BindData();
        }

        private void BindData()
        {
            if (!_isNew)
            {
                txtId.Text = _entity.Id;
                txtId.IsReadOnly = true; 
            }

            txtTypeName.Text = _entity.TypeName;
            chkSync.IsChecked = _entity.Syncronizeflag;

            txtId.Focus();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtId.Text))
            {
                MessageBox.Show("Vui lòng nhập Mã nội dung.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtId.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtTypeName.Text))
            {
                MessageBox.Show("Vui lòng nhập Tên nội dung.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtTypeName.Focus();
                return;
            }

            _entity.Id = txtId.Text.Trim();
            _entity.TypeName = txtTypeName.Text.Trim();
            _entity.Syncronizeflag = chkSync.IsChecked ?? false;

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
