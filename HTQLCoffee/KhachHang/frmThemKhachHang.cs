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

namespace HTQLCoffee.KhachHang
{
    public partial class frmThemKhachHang : Form
    {
        string connection = ConfigurationManager.ConnectionStrings["HTQLCoffee.Properties.Settings.QLCoffeeConnectionString"].ConnectionString;
        string maChiNhanh;
        // Random mã khách hàng
        string maKhachHang = GenerateRandomCode(10);
        public frmThemKhachHang()
        {
            InitializeComponent();
            LoadComboBoxes();
        }

        private static string GenerateRandomCode(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Random random = new Random();
            string result = new string(Enumerable.Repeat(chars, length)
                                              .Select(s => s[random.Next(s.Length)])
                                              .ToArray());
            return result;
        }

        // Load các giá trị cho ComboBox
        private void LoadComboBoxes()
        {
            // Giới tính
            cbxGioiTinh.Items.Add("Nam");
            cbxGioiTinh.Items.Add("Nữ");
            cbxGioiTinh.Items.Add("Khác");
            cbxGioiTinh.SelectedIndex = 0;

            // Mã chi nhánh: Select top 1 mã chi nhánh
            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();
                string query = "SELECT TOP 1 MaChiNhanh FROM ChiNhanh";
                SqlCommand command = new SqlCommand(query, conn);
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    maChiNhanh = reader["MaChiNhanh"].ToString();
                }
            }
        }

        // Kiểm tra định dạng số điện thoại và email
        private bool IsValidPhoneNumber(string phoneNumber)
        {
            string pattern = @"^(\+84|0)[3|5|7|8|9]\d{8}$"; // Định dạng số điện thoại VN
            return Regex.IsMatch(phoneNumber, pattern);
        }

        private bool IsValidEmail(string email)
        {
            string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"; // Định dạng email cơ bản
            return Regex.IsMatch(email, pattern);
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            string hoTen = txtTenKhach.Text;
            string soDienThoai = txtSDT.Text;
            DateTime ngaySinh = dtpNgaySinh.Value;
            string email = txtEmail.Text;
            string gioiTinh = cbxGioiTinh.SelectedItem.ToString();
            string diaChi = txtDiaChi.Text;
            string loaiKhachHang = "Thường";

            // Kiểm tra các trường bắt buộc
            if (string.IsNullOrWhiteSpace(hoTen) || string.IsNullOrWhiteSpace(soDienThoai))
            {
                MessageBox.Show("Họ tên và số điện thoại là bắt buộc.");
                return;
            }

            // Kiểm tra định dạng số điện thoại và email
            if (!IsValidPhoneNumber(soDienThoai))
            {
                MessageBox.Show("Số điện thoại không hợp lệ.");
                return;
            }

            if (!string.IsNullOrEmpty(email))
            {
                string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"; // Mẫu email hợp lệ
                if (!Regex.IsMatch(email, emailPattern))
                {
                    MessageBox.Show("Email không hợp lệ. Vui lòng nhập lại.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtEmail.Focus();
                    return;
                }
            }



            // Thực hiện thêm khách hàng vào cơ sở dữ liệu
            try
            {
                using (SqlConnection conn = new SqlConnection(connection))
                {
                    conn.Open();
                    string query = "INSERT INTO KhachHang (MaKhachHang, HoTen, SoDienThoai, Email, GioiTinh, DiaChi, NgaySinh, MaChiNhanh, LoaiKhachHang, NgayTao) " +
                                   "VALUES (@MaKhachHang, @HoTen, @SoDienThoai, @Email, @GioiTinh, @DiaChi, @NgaySinh, @MaChiNhanh, @LoaiKhachHang, @NgayTao)";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@MaKhachHang", maKhachHang);
                    cmd.Parameters.AddWithValue("@HoTen", hoTen);
                    cmd.Parameters.AddWithValue("@SoDienThoai", soDienThoai);
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@GioiTinh", gioiTinh);
                    cmd.Parameters.AddWithValue("@DiaChi", diaChi);
                    cmd.Parameters.AddWithValue("@MaChiNhanh", maChiNhanh);
                    cmd.Parameters.AddWithValue("@LoaiKhachHang", loaiKhachHang);
                    cmd.Parameters.AddWithValue("@NgayTao", DateTime.Now);
                    cmd.Parameters.AddWithValue("@NgaySinh", ngaySinh);

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Khách hàng đã được thêm thành công!");
                    this.Close(); // Đóng form sau khi thêm
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thêm khách hàng: " + ex.Message);
            }
        }

        private void frmThemKhachHang_Load(object sender, EventArgs e)
        {
            txtMaKhachHang.Text = maKhachHang;
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
