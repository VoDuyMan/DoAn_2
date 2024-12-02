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

namespace HTQLCoffee.NhaCungCap
{
    public partial class frmThemNCC : Form
    {
        string connection = ConfigurationManager.ConnectionStrings["HTQLCoffee.Properties.Settings.QLCoffeeConnectionString"].ConnectionString;

        public frmThemNCC()
        {
            InitializeComponent();
            txtIDCungCap.Text = GenerateRandomCode();
        }

        private string GenerateRandomCode()
        {
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            Random random = new Random();
            char[] buffer = new char[10];

            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = chars[random.Next(chars.Length)];
            }

            return new string(buffer);
        }
        private void btnLuu_Click(object sender, EventArgs e)
        {
            string maNhaCungCap = txtIDCungCap.Text.Trim();
            string tenNhaCungCap = txtTenNhaCungCap.Text.Trim();
            string soDienThoai = txtSoDienThoai.Text.Trim();
            string diaChi = txtDiaChi.Text.Trim();

            // Kiểm tra dữ liệu đầu vào
            if (!ValidateInput(maNhaCungCap, tenNhaCungCap, soDienThoai))
                return;

            try
            {
                using (SqlConnection conn = new SqlConnection(connection))
                {
                    conn.Open();
                    string maChiNhanhQuery = "SELECT TOP 1 MaChiNhanh FROM ChiNhanh";
                    SqlCommand chiNhanhCmd = new SqlCommand(maChiNhanhQuery, conn);
                    string maChiNhanh = chiNhanhCmd.ExecuteScalar().ToString();

                    if (maChiNhanh == null)
                    {
                        MessageBox.Show("Không tìm thấy mã chi nhánh!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    string query = "INSERT INTO NhaCungCap (MaNhaCungCap, TenNhaCungCap, DiaChi, SoDienThoai, MaChiNhanh) " +
                                   "VALUES (@MaNhaCungCap, @TenNhaCungCap, @DiaChi, @SoDienThoai, @MaChiNhanh)";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@MaNhaCungCap", maNhaCungCap);
                        cmd.Parameters.AddWithValue("@TenNhaCungCap", tenNhaCungCap);
                        cmd.Parameters.AddWithValue("@DiaChi", diaChi);
                        cmd.Parameters.AddWithValue("@SoDienThoai", soDienThoai);
                        cmd.Parameters.AddWithValue("@MaChiNhanh", maChiNhanh);

                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Thêm nhà cung cấp thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Đóng form
                        this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thêm nhà cung cấp: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidateInput(string maNhaCungCap, string tenNhaCungCap, string soDienThoai)
        {
            // Kiểm tra tên nhà cung cấp
            if (string.IsNullOrEmpty(tenNhaCungCap) || !Regex.IsMatch(tenNhaCungCap, @"^[\p{L}0-9\s]+$"))
            {
                MessageBox.Show("Tên nhà cung cấp chỉ được chứa chữ cái , số và không được để trống.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Kiểm tra số điện thoại
            if (!Regex.IsMatch(soDienThoai, @"^\d{10,11}$"))
            {
                MessageBox.Show("Số điện thoại phải là số và có từ 10 đến 11 chữ số.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmThemNCC_Load(object sender, EventArgs e)
        {

        }
    }
}
