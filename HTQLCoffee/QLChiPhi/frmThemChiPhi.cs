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

namespace HTQLCoffee.QLChiPhi
{
    public partial class frmThemChiPhi : Form
    {
        string connection = ConfigurationManager.ConnectionStrings["HTQLCoffee.Properties.Settings.QLCoffeeConnectionString"].ConnectionString;

        public frmThemChiPhi()
        {
            InitializeComponent();
            GenerateRandomMaChiPhi();
        }
        private void GenerateRandomMaChiPhi()
        {
            string maChiPhi = GenerateRandomCode(10);
            txtMaChiPhi.Text = maChiPhi;
        }
        private string GenerateRandomCode(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Random random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // Kiểm tra thông tin đã được nhập đầy đủ chưa
            if (string.IsNullOrWhiteSpace(txtTenChiPhi.Text) ||
                string.IsNullOrWhiteSpace(txtSoTien.Text) ||
                string.IsNullOrWhiteSpace(txtGhiChu.Text))
            {
                MessageBox.Show("Vui lòng nhập đủ thông tin!");
                return;
            }

            // Lấy mã chi nhánh từ bảng ChiNhanh
            string maChiNhanh = GetChiNhanhCode();

            // Lưu vào cơ sở dữ liệu
            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();
                string query = "INSERT INTO ChiPhiKhac (MaChiPhi, TenChiPhi, SoTien, NgayChi, GhiChu, MaChiNhanh) " +
                               "VALUES (@MaChiPhi, @TenChiPhi, @SoTien, @NgayChi, @GhiChu, @MaChiNhanh)";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@MaChiPhi", txtMaChiPhi.Text);
                cmd.Parameters.AddWithValue("@TenChiPhi", txtTenChiPhi.Text);
                cmd.Parameters.AddWithValue("@SoTien", decimal.Parse(txtSoTien.Text));
                cmd.Parameters.AddWithValue("@NgayChi", dateTimePickerNgayChi.Value);
                cmd.Parameters.AddWithValue("@GhiChu", txtGhiChu.Text);
                cmd.Parameters.AddWithValue("@MaChiNhanh", maChiNhanh);

                cmd.ExecuteNonQuery();
            }

            // Thông báo thêm thành công
            MessageBox.Show("Thêm chi phí thành công!");
            this.Close(); // Đóng form sau khi lưu thành công
        }

        private string GetChiNhanhCode()
        {
            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();
                string query = "SELECT MaChiNhanh FROM ChiNhanh";
                SqlCommand cmd = new SqlCommand(query, conn);
                return cmd.ExecuteScalar().ToString();
            }
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
