using System.Windows;
using DataAccess.EntityClasses;
using NewaccNet.Wpf.Views.Base;

namespace NewaccNet.Wpf.AppSystem.Directory
{
    public partial class PartnerEditorWindow : BaseWindow
    {
        private PartnerEntity _partner;
        private bool _isNew;

        public PartnerEditorWindow(PartnerEntity partner, bool isNew)
        {
            InitializeComponent();
            _partner = partner;
            _isNew = isNew;

            BindData();
        }

        private void BindData()
        {
            if (!_isNew)
            {
                txtId.Text = _partner.Id;
                txtId.IsReadOnly = true; // Không cho phép sửa mã nếu đang edit
            }

            txtCustomerName.Text = _partner.CustomerName;
            txtAddress.Text = _partner.Address;
            txtTaxno.Text = _partner.Taxno;
            txtPhonenumber.Text = _partner.Phonenumber;
            txtFaxnumber.Text = _partner.Faxnumber;
            chkIsEmployee.IsChecked = _partner.Isemployee;
            txtNote.Text = _partner.Note;

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

            if (string.IsNullOrWhiteSpace(txtCustomerName.Text))
            {
                MessageBox.Show("Vui lòng nhập Tên đối tượng.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtCustomerName.Focus();
                return;
            }

            _partner.Id = txtId.Text.Trim();
            _partner.CustomerName = txtCustomerName.Text.Trim();
            _partner.Address = txtAddress.Text.Trim();
            _partner.Taxno = txtTaxno.Text.Trim();
            _partner.Phonenumber = txtPhonenumber.Text.Trim();
            _partner.Faxnumber = txtFaxnumber.Text.Trim();
            _partner.Isemployee = chkIsEmployee.IsChecked ?? false;
            _partner.Note = txtNote.Text.Trim();

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
