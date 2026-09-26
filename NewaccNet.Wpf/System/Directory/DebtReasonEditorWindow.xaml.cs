using System.Windows;
using DataAccess.EntityClasses;
using NewaccNet.Wpf.Views.Base;

namespace NewaccNet.Wpf.AppSystem.Directory
{
    public partial class DebtReasonEditorWindow : BaseWindow
    {
        private DebtReasonEntity _entity;
        private bool _isNew;

        public DebtReasonEditorWindow(DebtReasonEntity entity, bool isNew)
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
                txtId.Text = _entity.Id.ToString();
            }

            txtReasonName.Text = _entity.ReasonName;
            chkVisible.IsChecked = _entity.Visible;

            txtReasonName.Focus();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtReasonName.Text))
            {
                MessageBox.Show("Vui lòng nhập Tên lý do.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtReasonName.Focus();
                return;
            }

            _entity.ReasonName = txtReasonName.Text.Trim();
            _entity.Visible = chkVisible.IsChecked ?? true;

            // Nếu DB không tự động tăng ID và bắt buộc nhập bằng tay thì xử lý ở đây
            // Hiện tại giả định ID là Identity tự tăng

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
