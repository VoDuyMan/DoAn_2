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
    public partial class frmSua : Form
    {
        string maKhachHang;
        string connection = ConfigurationManager.ConnectionStrings["HTQLCoffee.Properties.Settings.QLCoffeeConnectionString"].ConnectionString;

        public frmSua(string customerId)
        {
            InitializeComponent();
            this.maKhachHang = customerId;
            LoadKhachHangInfo();
        }

        // Load thông tin khách hàng vào các TextBox và ComboBox
        private void LoadKhachHangInfo()
        {
            

            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();
                string query = "SELECT * FROM KhachHang WHERE MaKhachHang = @MaKhachHang";
                SqlCommand command = new SqlCommand(query, conn);
                command.Parameters.AddWithValue("@MaKhachHang", maKhachHang);

                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    txtMaKhachHang.Text = reader["MaKhachHang"].ToString();
                    txtTenKhach.Text = string.IsNullOrEmpty(reader["HoTen"].ToString()) ? "Không có dữ liệu" : reader["HoTen"].ToString();
                    txtSDT.Text = string.IsNullOrEmpty(reader["SoDienThoai"].ToString()) ? "Không có dữ liệu" : reader["SoDienThoai"].ToString();
                    txtEmail.Text = string.IsNullOrEmpty(reader["Email"].ToString()) ? "Không có dữ liệu" : reader["Email"].ToString();

                    // Load ComboBox giới tính với các giá trị Nam, Nữ, Khác
                    cbxGioiTinh.Items.Clear(); // Xóa các mục cũ nếu có
                    cbxGioiTinh.Items.Add("Nam");
                    cbxGioiTinh.Items.Add("Nữ");
                    cbxGioiTinh.Items.Add("Khác");

                    // Set mặc định là Nam
                    cbxGioiTinh.SelectedItem = string.IsNullOrEmpty(reader["GioiTinh"].ToString()) ? "Nam" : reader["GioiTinh"].ToString();

                    txtDiaChi.Text = string.IsNullOrEmpty(reader["DiaChi"].ToString()) ? "Không có dữ liệu" : reader["DiaChi"].ToString();

                    // DateTimePicker NgaySinh (Kiểm tra nếu có dữ liệu)
                    if (reader["NgaySinh"] != DBNull.Value)
                    {
                        dtpNgaySinh.Value = Convert.ToDateTime(reader["NgaySinh"]);
                    }
                    else
                    {
                        dtpNgaySinh.Value = DateTime.Now; // Nếu không có dữ liệu, có thể để ngày hiện tại
                    }
                }
                else
                {
                    MessageBox.Show("Không tìm thấy thông tin khách hàng.");
                    return;
                }
            }
        }


        // Xử lý sự kiện lưu thay đổi
        private void btnLuu_Click(object sender, EventArgs e)
        {
            // Lấy thông tin từ các TextBox, ComboBox
            string hoTen = txtTenKhach.Text;
            string soDienThoai = txtSDT.Text;
            string email = txtEmail.Text;
            string gioiTinh = cbxGioiTinh.SelectedItem.ToString();
            string diaChi = txtDiaChi.Text;
            DateTime ngaySinh = dtpNgaySinh.Value;

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

            if (!IsValidEmail(email))
            {
                MessageBox.Show("Email không hợp lệ.");
                return;
            }

            // Cập nhật thông tin khách hàng vào cơ sở dữ liệu
            try
            {
                using (SqlConnection conn = new SqlConnection(connection))
                {
                    conn.Open();
                    string query = "UPDATE KhachHang SET HoTen = @HoTen, SoDienThoai = @SoDienThoai, Email = @Email, " +
                                   "GioiTinh = @GioiTinh, DiaChi = @DiaChi, MaChiNhanh = @MaChiNhanh, " +
                                   "NgaySinh = @NgaySinh, NgayCapNhat = @NgayCapNhat WHERE MaKhachHang = @MaKhachHang";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@MaKhachHang", maKhachHang);
                    cmd.Parameters.AddWithValue("@HoTen", hoTen);
                    cmd.Parameters.AddWithValue("@SoDienThoai", soDienThoai);
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@GioiTinh", gioiTinh);
                    cmd.Parameters.AddWithValue("@DiaChi", diaChi);
                    cmd.Parameters.AddWithValue("@MaChiNhanh", GetMaChiNhanh());
                    cmd.Parameters.AddWithValue("@NgaySinh", ngaySinh);
                    cmd.Parameters.AddWithValue("@NgayCapNhat", DateTime.Now);

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Thông tin khách hàng đã được cập nhật thành công!");
                    this.Close(); // Đóng form sửa sau khi cập nhật
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi cập nhật thông tin khách hàng: " + ex.Message);
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

        private string GetMaChiNhanh()
        {
            string maChiNhanh = string.Empty;
            try
            {
                using (SqlConnection conn = new SqlConnection(connection))
                {
                    conn.Open();
                    string query = "SELECT TOP 1 MaChiNhanh FROM ChiNhanh"; // Lấy 1 bản ghi đầu tiên từ bảng ChiNhanh
                    SqlCommand command = new SqlCommand(query, conn);

                    // Thực thi truy vấn và lấy kết quả
                    maChiNhanh = command.ExecuteScalar()?.ToString(); // ExecuteScalar trả về giá trị đầu tiên trong kết quả, nếu không có trả về null
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lấy mã chi nhánh: " + ex.Message);
            }

            return maChiNhanh;
        }


        private void frmSua_Load(object sender, EventArgs e)
        {

        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
