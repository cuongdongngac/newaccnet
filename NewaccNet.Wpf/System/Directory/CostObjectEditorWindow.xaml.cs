using System.Windows;
using DataAccess.EntityClasses;
using NewaccNet.Wpf.Views.Base;

namespace NewaccNet.Wpf.AppSystem.Directory
{
    public partial class CostObjectEditorWindow : BaseWindow
    {
        private CostObjectEntity _entity;
        private bool _isNew;

        public CostObjectEditorWindow(CostObjectEntity entity, bool isNew)
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

            txtObjectName.Text = _entity.ObjectName;
            chkSync.IsChecked = _entity.Syncronizeflag;

            txtId.Focus();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtId.Text))
            {
                MessageBox.Show("Vui lòng nhập Mã đối tượng.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtId.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtObjectName.Text))
            {
                MessageBox.Show("Vui lòng nhập Tên đối tượng.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtObjectName.Focus();
                return;
            }

            _entity.Id = txtId.Text.Trim();
            _entity.ObjectName = txtObjectName.Text.Trim();
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
