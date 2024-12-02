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

namespace HTQLCoffee.PhongBan
{
    public partial class frmSuaBan : Form
    {
        string connection = ConfigurationManager.ConnectionStrings["HTQLCoffee.Properties.Settings.QLCoffeeConnectionString"].ConnectionString;

        string maBan;
        public frmSuaBan(string maBan)
        {
            InitializeComponent();
            this.maBan = maBan;
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connection))
                {
                    conn.Open();
                    string query = "UPDATE BanAn SET TenBan = @TenBan, TrangThai = @TrangThai, NgayCapNhat = @NgayCapNhat WHERE MaBan = @MaBan";
                    SqlCommand command = new SqlCommand(query, conn);

                    command.Parameters.AddWithValue("@MaBan", txtMaBan.Text.Trim());
                    command.Parameters.AddWithValue("@TenBan", txtTenBan.Text.Trim());
                    command.Parameters.AddWithValue("@TrangThai", txtTrangThai.Text.Trim());
                    command.Parameters.AddWithValue("@NgayCapNhat", DateTime.Now);

                    int result = command.ExecuteNonQuery();
                    if (result > 0)
                    {
                        MessageBox.Show("Cập nhật bàn ăn thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Không có dữ liệu nào được cập nhật.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi cập nhật bàn ăn: " + ex.Message);
            }
        }

        private void frmSuaBan_Load(object sender, EventArgs e)
        {
            LoadBanAnDetails();
        }

        private void LoadBanAnDetails()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connection))
                {
                    conn.Open();
                    string query = "SELECT MaBan, TenBan, TrangThai FROM BanAn WHERE MaBan = @MaBan";
                    SqlCommand command = new SqlCommand(query, conn);
                    command.Parameters.AddWithValue("@MaBan", maBan);

                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        txtMaBan.Text = reader["MaBan"].ToString();
                        txtTenBan.Text = reader["TenBan"].ToString();
                        txtTrangThai.Text = reader["TrangThai"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải dữ liệu bàn ăn: " + ex.Message);
            }
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
