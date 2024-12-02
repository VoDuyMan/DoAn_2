using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HTQLCoffee
{
    public partial class frmThongTinChiNhanh : Form
    {
        private string branchCode;
        private string expectedCode;

        public frmThongTinChiNhanh(string code, string password)
        {
            InitializeComponent();
            branchCode = code;
            expectedCode = password;

            LoadBranchInfo();
        }

        private void LoadBranchInfo()
        {

            string serverConnectionString = ConfigurationManager.ConnectionStrings["HTQLCoffee.Properties.Settings.QLCoffeeConnectionString"].ConnectionString;


            using (SqlConnection connection = new SqlConnection(serverConnectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT MaChiNhanh, TenChiNhanh, DiaChi, SoDienThoai FROM ChiNhanh WHERE MaChiNhanh = @MaChiNhanh";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@MaChiNhanh", branchCode);

                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        labelBranchCode.Text = reader["MaChiNhanh"].ToString();
                        textBoxBranchName.Text = reader["TenChiNhanh"].ToString();
                        textBoxBranchAddress.Text = reader["DiaChi"].ToString();
                        textBoxBranchPhone.Text = reader["SoDienThoai"].ToString();


                        SetEditMode(false);
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy thông tin chi nhánh.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi lấy thông tin chi nhánh: " + ex.Message);
                }
            }
        }

        private void SetEditMode(bool isEditable)
        {
            textBoxBranchName.Enabled = isEditable;
            textBoxBranchAddress.Enabled = isEditable;
            textBoxBranchPhone.Enabled = isEditable;
            btnSave.Enabled = isEditable;
        }

        private void btnCheckPassword_Click(object sender, EventArgs e)
        {
            if (textBoxPassword.Text.Trim() == expectedCode)
            {
                MessageBox.Show("Đã chuyển sang chế độ chỉnh sửa.");
                SetEditMode(true);
            }
            else
            {
                MessageBox.Show("Mật khẩu không đúng. Bạn chỉ được quyền xem thông tin.");
                SetEditMode(false);
            }
        }
        private void btnSave_Click(object sender, EventArgs e)
        {

            if (!ValidateInputs())
            {
                return;
            }


            string serverConnectionString = ConfigurationManager.ConnectionStrings["HTQLCoffee.Properties.Settings.QLCoffeeConnectionString"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(serverConnectionString))
            {
                try
                {
                    connection.Open();
                    string query = "UPDATE ChiNhanh SET TenChiNhanh = @TenChiNhanh, DiaChi = @DiaChi, SoDienThoai = @SoDienThoai WHERE MaChiNhanh = @MaChiNhanh";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@MaChiNhanh", branchCode);
                    command.Parameters.AddWithValue("@TenChiNhanh", textBoxBranchName.Text);
                    command.Parameters.AddWithValue("@DiaChi", textBoxBranchAddress.Text);
                    command.Parameters.AddWithValue("@SoDienThoai", textBoxBranchPhone.Text);

                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Cập nhật thông tin chi nhánh thành công.");
                    }
                    else
                    {
                        MessageBox.Show("Cập nhật thông tin chi nhánh thất bại.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi cập nhật thông tin chi nhánh: " + ex.Message);
                }
            }
        }

        private bool ValidateInputs()
        {

            if (textBoxBranchName.Text.Length > 20)
            {
                MessageBox.Show("Tên chi nhánh không được quá 20 ký tự.");
                return false;
            }

            // Kiểm tra địa chỉ
            if (textBoxBranchAddress.Text.Length > 255)
            {
                MessageBox.Show("Địa chỉ không được quá 255 ký tự.");
                return false;
            }


            string phoneNumber = textBoxBranchPhone.Text.Trim();
            if (!IsValidPhoneNumber(phoneNumber))
            {
                MessageBox.Show("Số điện thoại không hợp lệ. Vui lòng nhập lại số điện thoại hợp lệ (10-15 chữ số và bắt đầu bằng đầu số Việt Nam).");
                return false;
            }

            return true;
        }

        private bool IsValidPhoneNumber(string phoneNumber)
        {

            if (phoneNumber.Length < 10 || phoneNumber.Length > 15)
            {
                return false;
            }

            string[] validPrefixes = { "01", "02", "03", "04", "05", "06", "07", "08", "09", "84" };

            if (!phoneNumber.StartsWith("0") && !phoneNumber.StartsWith("84"))
            {
                return false;
            }


            foreach (char c in phoneNumber)
            {
                if (!char.IsDigit(c))
                {
                    return false;
                }
            }

            return true;
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
