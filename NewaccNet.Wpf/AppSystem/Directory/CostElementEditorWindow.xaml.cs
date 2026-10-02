using System.Windows;
using DataAccess.EntityClasses;
using NewaccNet.Wpf.Views.Base;

namespace NewaccNet.Wpf.AppSystem.Directory
{
    public partial class CostElementEditorWindow : BaseWindow
    {
        private CostElementEntity _entity;
        private bool _isNew;

        public CostElementEditorWindow(CostElementEntity entity, bool isNew)
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

            txtElementName.Text = _entity.ElementName;
            chkTaxflag.IsChecked = _entity.Taxflag;

            txtId.Focus();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtId.Text))
            {
                MessageBox.Show("Vui lòng nhập Mã yếu tố.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtId.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtElementName.Text))
            {
                MessageBox.Show("Vui lòng nhập Tên yếu tố.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtElementName.Focus();
                return;
            }

            _entity.Id = txtId.Text.Trim();
            _entity.ElementName = txtElementName.Text.Trim();
            _entity.Taxflag = chkTaxflag.IsChecked ?? false;

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
