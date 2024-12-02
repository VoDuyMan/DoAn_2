using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.Data.SqlClient;

namespace HTQLCoffee.PhongBan
{
    public partial class frmThemBan : Form
    {
        string connection = ConfigurationManager.ConnectionStrings["HTQLCoffee.Properties.Settings.QLCoffeeConnectionString"].ConnectionString;
        public frmThemBan()
        {
            InitializeComponent();
        }
        string maBan = GenerateRandomCode(10);

        private void btnLuu_Click(object sender, EventArgs e)
        {
            string tenBan = txtTenBan.Text.Trim();
            string trangThai = "Chưa đặt bàn";
             // Sinh mã bàn ngẫu nhiên
            string maChiNhanh = GetMaChiNhanh();  // Lấy mã chi nhánh từ bảng ChiNhanh

            if (string.IsNullOrEmpty(tenBan) || string.IsNullOrEmpty(trangThai))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(maChiNhanh))
            {
                MessageBox.Show("Không thể lấy mã chi nhánh!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connection))
                {
                    conn.Open();
                    string query = "INSERT INTO BanAn (MaBan, TenBan, TrangThai, MaChiNhanh, NgayTao, NgayCapNhat) VALUES (@MaBan, @TenBan, @TrangThai, @MaChiNhanh, @NgayTao, @NgayCapNhat)";
                    SqlCommand command = new SqlCommand(query, conn);

                    // Thêm tham số
                    command.Parameters.AddWithValue("@MaBan", maBan);
                    command.Parameters.AddWithValue("@TenBan", tenBan);
                    command.Parameters.AddWithValue("@TrangThai", trangThai);
                    command.Parameters.AddWithValue("@MaChiNhanh", maChiNhanh);
                    command.Parameters.AddWithValue("@NgayTao", DateTime.Now);
                    command.Parameters.AddWithValue("@NgayCapNhat", DateTime.Now);

                    int result = command.ExecuteNonQuery();
                    if (result > 0)
                    {
                        MessageBox.Show("Thêm bàn ăn thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.DialogResult = DialogResult.OK; // Đóng form và thông báo thành công
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Thêm bàn ăn thất bại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thêm bàn ăn: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static string GenerateRandomCode(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Random random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private string GetMaChiNhanh()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connection))
                {
                    conn.Open();
                    string query = "SELECT TOP 1 MaChiNhanh FROM ChiNhanh"; // Truy vấn lấy mã chi nhánh
                    SqlCommand command = new SqlCommand(query, conn);

                    object result = command.ExecuteScalar();
                    return result != null ? result.ToString() : string.Empty;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lấy mã chi nhánh: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return string.Empty;
            }
        }

        private void frmThemBan_Load(object sender, EventArgs e)
        {
            txtMaBan.Text = maBan; 
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnThoat_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}
