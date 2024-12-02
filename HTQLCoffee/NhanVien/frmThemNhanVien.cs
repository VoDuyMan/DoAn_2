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
    public partial class frmThemNhanVien : Form
    {
        string connection = ConfigurationManager.ConnectionStrings["HTQLCoffee.Properties.Settings.QLCoffeeConnectionString"].ConnectionString;

        public frmThemNhanVien()
        {
            InitializeComponent();
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmThemNhanVien_Load(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();

                string newMaNV;
                bool isDuplicate;

                do
                {
                    // Gọi hàm tạo mã ngẫu nhiên
                    newMaNV = GenerateRandomCode(10);

                    // Kiểm tra xem mã nhân viên đã tồn tại trong cơ sở dữ liệu chưa
                    string query = "SELECT COUNT(*) FROM NhanVien WHERE MaNhanVien = @MaNhanVien";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@MaNhanVien", newMaNV);
                        int count = (int)cmd.ExecuteScalar();

                        // Nếu mã đã tồn tại, đặt isDuplicate = true để tiếp tục vòng lặp
                        isDuplicate = count > 0;
                    }

                } while (isDuplicate); // Tiếp tục tạo mã mới nếu mã đã tồn tại

                txtMaNhanVien.Text = newMaNV; // Hiển thị mã nhân viên mới lên textbox
            }

            // Thiết lập giá trị cho ComboBox chức vụ
            cbxChucVu.Items.AddRange(new string[] { "Phục Vụ", "Pha Chế", "Bảo Vệ" });
            cbxChucVu.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        // Hàm tạo chuỗi ngẫu nhiên với độ dài bất kỳ
        private string GenerateRandomCode(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Random random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        private void btnLuu_Click(object sender, EventArgs e)
        {
            // Kiểm tra dữ liệu đầu vào
            if (string.IsNullOrWhiteSpace(txtTenNhanVien.Text) || txtTenNhanVien.Text.Length > 18)
            {
                MessageBox.Show("Tên nhân viên không được rỗng và không quá 18 ký tự.");
                return;
            }

            if (cbxChucVu.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn chức vụ của nhân viên");
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

            // Khai báo biến luongCoBan
            decimal luongCoBan;
            if (!decimal.TryParse(txtLuongCoBan.Text, out luongCoBan) || luongCoBan <= 0)
            {
                MessageBox.Show("Lương cơ bản phải là số lớn hơn 0.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtLuongCoBan.Focus();
                return;
            }

            // Lấy mã chi nhánh duy nhất từ bảng ChiNhanh
            string maChiNhanh = GetMaChiNhanh();
            if (string.IsNullOrEmpty(maChiNhanh)) // Kiểm tra nếu mã chi nhánh không tìm thấy
            {
                return;
            }

            // Thêm nhân viên vào cơ sở dữ liệu
            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();

                string insertNhanVien = @"INSERT INTO NhanVien (MaNhanVien, HoTen, SoDienThoai, Email, ChucVu, LuongCoBan, NgayVaoLam, GhiChu, MaChiNhanh)
                                  VALUES (@MaNhanVien, @HoTen, @SoDienThoai, @Email, @ChucVu, @LuongCoBan, @NgayVaoLam, @GhiChu, @MaChiNhanh)";
                using (SqlCommand cmd = new SqlCommand(insertNhanVien, conn))
                {
                    cmd.Parameters.AddWithValue("@MaNhanVien", txtMaNhanVien.Text);
                    cmd.Parameters.AddWithValue("@HoTen", txtTenNhanVien.Text);
                    cmd.Parameters.AddWithValue("@SoDienThoai", txtSoDienThoai.Text);
                    cmd.Parameters.AddWithValue("@Email", txtEmail.Text);
                    cmd.Parameters.AddWithValue("@ChucVu", cbxChucVu.SelectedItem.ToString());
                    cmd.Parameters.AddWithValue("@LuongCoBan", luongCoBan);
                    cmd.Parameters.AddWithValue("@NgayVaoLam", dtpNgayVaoLam.Value);
                    cmd.Parameters.AddWithValue("@GhiChu", string.IsNullOrWhiteSpace(txtGhiChu.Text) ? (object)DBNull.Value : txtGhiChu.Text);
                    cmd.Parameters.AddWithValue("@MaChiNhanh", maChiNhanh);

                    cmd.ExecuteNonQuery();
                }
            }

            MessageBox.Show("Thêm nhân viên thành công!");
            this.Close(); // Đóng form sau khi thêm xong
        }

        private string GetMaChiNhanh()
        {
            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();
                string query = "SELECT TOP 1 MaChiNhanh FROM ChiNhanh";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        return result.ToString();
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy mã chi nhánh.");
                        return string.Empty;
                    }
                }
            }
        }
    }
}
