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
using HTQLCoffee.KhachHang; 

namespace HTQLCoffee.PhongBan
{
    public partial class frmDatBan : Form
    {
        string connection = ConfigurationManager.ConnectionStrings["HTQLCoffee.Properties.Settings.QLCoffeeConnectionString"].ConnectionString;
        public frmDatBan(string maBan, string tenBan)
        {
            InitializeComponent();

            txtTenBan.Text = tenBan.ToString();
            txtMaBan.Text = maBan;
        }

        private void btnXacNhan_Click(object sender, EventArgs e)
        {
            // Kiểm tra thông tin nhập
            if (string.IsNullOrWhiteSpace(txtMaKhach.Text))
            {
                MessageBox.Show("Vui lòng nhập thông tin khách hàng.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Lấy thông tin từ form
                string maKhachHang = txtMaKhach.Text;
                string maBan = txtMaBan.Text;
                string tinhTrang = "Chưa thanh toán";
                string ghiChu = txtGhiChu.Text;
                DateTime ngayTao = DateTime.Now;

                // Lấy mã đặt bàn tiếp theo
                string maDatBan = GetNextBookingCode(connection);

                // Lưu thông tin đặt bàn vào CSDL
                using (SqlConnection conn = new SqlConnection(connection))
                {
                    conn.Open();

                    string sql = @"INSERT INTO DatBan (MaDatBan, MaKhachHang, MaBan, TinhTrang, GhiChu, NgayTao) 
                           VALUES (@MaDatBan, @MaKhachHang, @MaBan, @TinhTrang, @GhiChu, @NgayTao)";

                    using (SqlCommand cmdDatBan = new SqlCommand(sql, conn))
                    {
                        cmdDatBan.Parameters.AddWithValue("@MaDatBan", maDatBan);
                        cmdDatBan.Parameters.AddWithValue("@MaKhachHang", maKhachHang);
                        cmdDatBan.Parameters.AddWithValue("@MaBan", maBan);
                        cmdDatBan.Parameters.AddWithValue("@TinhTrang", tinhTrang);
                        cmdDatBan.Parameters.AddWithValue("@GhiChu", string.IsNullOrWhiteSpace(ghiChu) ? DBNull.Value : (object)ghiChu);
                        cmdDatBan.Parameters.AddWithValue("@NgayTao", ngayTao);

                        int rowsAffected = cmdDatBan.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            // Cập nhật trạng thái bàn
                            UpdateTableStatus(maBan, "Đã đặt bàn");
                            MessageBox.Show("Đặt bàn thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Không thể đặt bàn. Vui lòng thử lại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi xảy ra: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private string GetNextBookingCode(string connectionString)
        {
            string newBookingCode;
            bool isDuplicate;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                do
                {
                    newBookingCode = GenerateRandomCode(10);

                    string query = "SELECT COUNT(*) FROM DatBan WHERE MaDatBan = @MaDatBan";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@MaDatBan", newBookingCode);
                        int count = (int)cmd.ExecuteScalar();
                        isDuplicate = count > 0;
                    }
                } while (isDuplicate);
            }

            return newBookingCode;
        }

        private string GenerateRandomCode(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Random random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private void UpdateTableStatus(string maBan, string trangThai)
        {
            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("UPDATE BanAn SET TrangThai = @TrangThai WHERE MaBan = @MaBan", conn);
                cmd.Parameters.AddWithValue("@TrangThai", trangThai);
                cmd.Parameters.AddWithValue("@MaBan", maBan);
                cmd.ExecuteNonQuery();
            }
        }

        private void txtTimKiem_TextChanged(object sender, EventArgs e)
        {
            string searchText = txtTimKiem.Text.Trim();


            // Hiển thị gợi ý chỉ khi có ít nhất 2 ký tự
            if (searchText.Length >= 2)
            {
                ShowSuggestions(searchText);
            }
            else
            {
                listBoxSuggestions.Visible = false; // Ẩn nếu không đủ ký tự
            }
        }
        private void ShowSuggestions(string searchText)
        {
            listBoxSuggestions.Items.Clear(); // Xóa các gợi ý cũ
            string query = "SELECT SoDienThoai FROM KhachHang WHERE SoDienThoai LIKE @SearchText";

            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();

                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    command.Parameters.AddWithValue("@SearchText", "%" + searchText + "%");
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        string soDienThoai = reader["SoDienThoai"].ToString();
                        listBoxSuggestions.Items.Add(soDienThoai);
                    }

                    reader.Close();
                }
            }

            // Hiển thị gợi ý nếu có ít nhất một kết quả
            listBoxSuggestions.Visible = listBoxSuggestions.Items.Count > 0;
        }

        private void listBoxSuggestions_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxSuggestions.SelectedItem != null)
            {
                txtTimKiem.Text = listBoxSuggestions.SelectedItem.ToString();
                listBoxSuggestions.Visible = false; // Ẩn gợi ý sau khi chọn
            }
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            // Lấy số điện thoại từ ô tìm kiếm
            string soDienThoai = txtTimKiem.Text.Trim();

            if (string.IsNullOrWhiteSpace(soDienThoai))
            {
                MessageBox.Show("Vui lòng nhập số điện thoại để tìm kiếm!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();

                // Truy vấn tìm kiếm khách hàng theo số điện thoại
                string query = "SELECT MaKhachHang, HoTen, SoDienThoai FROM KhachHang WHERE SoDienThoai = @SoDienThoai";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@SoDienThoai", soDienThoai);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        // Nếu tìm thấy khách hàng, hiển thị thông tin
                        txtMaKhach.Text = reader["MaKhachHang"].ToString();
                        txtHoTen.Text = reader["HoTen"].ToString();
                        txtSoDienThoai.Text = reader["SoDienThoai"].ToString();
                    }
                    else
                    {
                        // Nếu không tìm thấy, hiển thị thông báo và mở form thêm khách hàng
                        MessageBox.Show("Không tìm thấy khách hàng! Vui lòng thêm khách hàng mới.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        frmThemKhachHang frmThem = new frmThemKhachHang();
                        frmThem.ShowDialog();
                        
                    }
                }
            }
        }

        private void frmDatBan_Load(object sender, EventArgs e)
        {
            txtGhiChu.TextAlign = HorizontalAlignment.Left;
            txtSoDienThoai.TextAlign = HorizontalAlignment.Left;
            txtHoTen.TextAlign = HorizontalAlignment.Left;
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
