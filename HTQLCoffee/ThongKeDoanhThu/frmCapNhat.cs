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

namespace HTQLCoffee.ThongKeDoanhThu
{
    public partial class frmCapNhat : Form
    {
        string connection = ConfigurationManager.ConnectionStrings["HTQLCoffee.Properties.Settings.QLCoffeeConnectionString"].ConnectionString;

        decimal tongChiPhi, tongDoanhThu, loiNhuan;
        public frmCapNhat(decimal tongChiPhi, decimal tongDoanhThu, decimal loiNhuan)
        {
            InitializeComponent();
            this.tongChiPhi = tongChiPhi;
            this.tongDoanhThu = tongDoanhThu;
            this.loiNhuan = loiNhuan;
        }

        private void frmCapNhat_Load(object sender, EventArgs e)
        {
            txtMaDoanhThu.Text = GenerateRandomCode(10);

            // Hiển thị ngày hiện tại mặc định
            dtpThang.Value = DateTime.Now;

            // Load dữ liệu doanh thu
            txtTongDoanhThu.Text = tongDoanhThu.ToString("N0") + " VNĐ";
            txtTongChiPhi.Text = tongChiPhi.ToString("N0") + " VNĐ";
            txtLoiNhuan.Text = loiNhuan.ToString("N0") + " VNĐ";
            lblTenChiNhanh.Text = "Chi nhánh " + getTenChiNhanh();
        }

        private string GenerateRandomCode(int length)
        {
            var random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(connection))
            {
                string query = "INSERT INTO DoanhThu (MaDoanhThu, Thang, TongDoanhThu, TongChiPhi, LoiNhuan, GhiChu, MaChiNhanh) " +
                               "VALUES (@MaDoanhThu, @Thang, @TongDoanhThu, @TongChiPhi, @LoiNhuan, @GhiChu, @MaChiNhanh)";

                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    command.Parameters.AddWithValue("@MaDoanhThu", txtMaDoanhThu.Text);
                    command.Parameters.AddWithValue("@Thang", dtpThang.Value);
                    command.Parameters.AddWithValue("@TongDoanhThu", tongDoanhThu);
                    command.Parameters.AddWithValue("@TongChiPhi", tongChiPhi);
                    command.Parameters.AddWithValue("@LoiNhuan", loiNhuan);
                    command.Parameters.AddWithValue("@GhiChu", txtGhiChu.Text);
                    command.Parameters.AddWithValue("@MaChiNhanh", getMaChiNhanh());

                    try
                    {
                        conn.Open();
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Cập nhật thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Cập nhật thất bại.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi: " + ex.Message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private string getTenChiNhanh()
        {
            string tenChiNhanh = string.Empty;

            using (SqlConnection conn = new SqlConnection(connection))
            {
                try
                {
                    conn.Open();

                    string query = "SELECT TOP 1 TenChiNhanh FROM ChiNhanh";

                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        object result = command.ExecuteScalar();

                        if (result != null)
                        {
                            tenChiNhanh = result.ToString();
                        }
                        else
                        {
                            MessageBox.Show("Không tìm thấy tên chi nhánh.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: " + ex.Message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            return tenChiNhanh;
        }
        private string getMaChiNhanh()
        {
            string maChiNhanh = string.Empty; // Biến lưu tên chi nhánh

            using (SqlConnection conn = new SqlConnection(connection))
            {
                try
                {
                    conn.Open();

                    string query = "SELECT TOP 1 MaChiNhanh FROM ChiNhanh";

                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        object result = command.ExecuteScalar();

                        if (result != null)
                        {
                            maChiNhanh = result.ToString();
                        }
                        else
                        {
                            MessageBox.Show("Không tìm thấy tên chi nhánh.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: " + ex.Message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            return maChiNhanh;
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
