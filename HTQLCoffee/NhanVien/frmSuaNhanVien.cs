using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HTQLCoffee.NhanVien
{
    public partial class frmSuaNhanVien : Form
    {
        string connection = ConfigurationManager.ConnectionStrings["HTQLCoffee.Properties.Settings.QLCoffeeConnectionString"].ConnectionString;

        string maNhanVien;
        public frmSuaNhanVien(string maNhanVien)
        {
            InitializeComponent();
            this.maNhanVien = maNhanVien;
        }
        private void LoadEmployeeData(string maNhanVien)
        {
            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();
                string query = "SELECT HoTen, SoDienThoai, Email, ChucVu, LuongCoBan, NgayVaoLam, GhiChu FROM NhanVien WHERE MaNhanVien = @MaNhanVien";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@MaNhanVien", maNhanVien);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            txtTenNhanVien.Text = reader["HoTen"].ToString();
                            txtSoDienThoai.Text = reader["SoDienThoai"].ToString();
                            txtEmail.Text = reader["Email"].ToString();
                            cbxChucVu.SelectedItem = reader["ChucVu"].ToString();
                            txtLuongCoBan.Text = reader["LuongCoBan"].ToString();
                            dtpNgayVaoLam.Value = Convert.ToDateTime(reader["NgayVaoLam"]);
                            txtGhiChu.Text = reader["GhiChu"] != DBNull.Value ? reader["GhiChu"].ToString() : string.Empty;
                            txtMaNhanVien.Text = maNhanVien;
                        }
                        else
                        {
                            MessageBox.Show("Không tìm thấy nhân viên với mã này.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
            }
        }

        private void frmSuaNhanVien_Load(object sender, EventArgs e)
        {
            cbxChucVu.Items.AddRange(new string[] { "Phục Vụ", "Pha Chế", "Bảo Vệ" });
            cbxChucVu.DropDownStyle = ComboBoxStyle.DropDownList;
            // Hiển thị thông tin nhân viên
            LoadEmployeeData(maNhanVien);
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            // Kiểm tra mã nhân viên
            if (string.IsNullOrWhiteSpace(txtMaNhanVien.Text))
            {
                MessageBox.Show("Vui lòng nhập mã nhân viên cần sửa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMaNhanVien.Focus();
                return;
            }

            // Kiểm tra dữ liệu đầu vào
            if (string.IsNullOrWhiteSpace(txtTenNhanVien.Text) || txtTenNhanVien.Text.Length > 18)
            {
                MessageBox.Show("Tên nhân viên không được rỗng và không quá 18 ký tự.");
                return;
            }

            if (cbxChucVu.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn chức vụ của nhân viên.");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
            }
            else if (!Regex.IsMatch(txtEmail.Text, @"^[\w-\.]+@gmail\.com$"))
            {
                MessageBox.Show("Email phải có định dạng @gmail.com", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEmail.Focus();
                return;
            }

            // Kiểm tra Số điện thoại không được để trống và đúng định dạng đầu số của Việt Nam (10 đến 12 số)
            if (string.IsNullOrWhiteSpace(txtSoDienThoai.Text))
            {
                MessageBox.Show("Vui lòng nhập Số điện thoại.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSoDienThoai.Focus();
                return;
            }
            else if (!Regex.IsMatch(txtSoDienThoai.Text, @"^(0|\+84)[3|5|7|8|9][0-9]{8,11}$"))
            {
                MessageBox.Show("Số điện thoại phải từ 10 đến 12 số và đúng đầu số của Việt Nam", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSoDienThoai.Focus();
                return;
            }

            // Kiểm tra Ngày vào làm không được để trống
            if (dtpNgayVaoLam.Value == null || dtpNgayVaoLam.Value > DateTime.Now)
            {
                MessageBox.Show("Vui lòng nhập Ngày vào làm hợp lệ.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dtpNgayVaoLam.Focus();
                return;
            }

            // Kiểm tra Lương cơ bản không được để trống và phải là số > 0
            if (string.IsNullOrWhiteSpace(txtLuongCoBan.Text))
            {
                MessageBox.Show("Vui lòng nhập Lương cơ bản.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtLuongCoBan.Focus();
                return;
            }

            decimal luongCoBan;
            if (!decimal.TryParse(txtLuongCoBan.Text, out luongCoBan) || luongCoBan <= 0)
            {
                MessageBox.Show("Lương cơ bản phải là số lớn hơn 0.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtLuongCoBan.Focus();
                return;
            }

            // Cập nhật thông tin nhân viên
            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();

                string updateQuery = @"UPDATE NhanVien 
                               SET HoTen = @HoTen, 
                                   SoDienThoai = @SoDienThoai, 
                                   Email = @Email, 
                                   ChucVu = @ChucVu, 
                                   LuongCoBan = @LuongCoBan, 
                                   NgayVaoLam = @NgayVaoLam, 
                                   GhiChu = @GhiChu, 
                                   NgayCapNhat = GETDATE() 
                               WHERE MaNhanVien = @MaNhanVien";
                using (SqlCommand cmd = new SqlCommand(updateQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@HoTen", txtTenNhanVien.Text);
                    cmd.Parameters.AddWithValue("@SoDienThoai", txtSoDienThoai.Text);
                    cmd.Parameters.AddWithValue("@Email", txtEmail.Text);
                    cmd.Parameters.AddWithValue("@ChucVu", cbxChucVu.SelectedItem.ToString());
                    cmd.Parameters.AddWithValue("@LuongCoBan", luongCoBan);
                    cmd.Parameters.AddWithValue("@NgayVaoLam", dtpNgayVaoLam.Value);
                    cmd.Parameters.AddWithValue("@GhiChu", string.IsNullOrWhiteSpace(txtGhiChu.Text) ? (object)DBNull.Value : txtGhiChu.Text);
                    cmd.Parameters.AddWithValue("@MaNhanVien", txtMaNhanVien.Text);

                    cmd.ExecuteNonQuery();
                }
            }

            MessageBox.Show("Cập nhật thông tin nhân viên thành công!");
            this.Close(); // Đóng form sau khi cập nhật xong
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
