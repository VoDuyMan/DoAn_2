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
    public partial class frmSuaNCC : Form
    {
        string connection = ConfigurationManager.ConnectionStrings["HTQLCoffee.Properties.Settings.QLCoffeeConnectionString"].ConnectionString;

        private string maNhaCungCap;

        public frmSuaNCC(string maNhaCungCap)
        {
            InitializeComponent();
            this.maNhaCungCap = maNhaCungCap;
        }

        private void frmSua_Load(object sender, EventArgs e)
        {
            // Lấy thông tin nhà cung cấp từ database dựa trên mã nhà cung cấp
            LoadNhaCungCapInfo();
        }

        private void LoadNhaCungCapInfo()
        {
            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();
                string query = "SELECT MaNhaCungCap, TenNhaCungCap, SoDienThoai, DiaChi FROM NhaCungCap WHERE MaNhaCungCap = @MaNhaCungCap";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@MaNhaCungCap", maNhaCungCap);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            txtIDCungCap.Text = reader["MaNhaCungCap"].ToString();
                            txtTenNhaCungCap.Text = reader["TenNhaCungCap"].ToString();
                            txtSoDienThoai.Text = reader["SoDienThoai"].ToString();
                            txtDiaChi.Text = reader["DiaChi"].ToString();
                        }
                    }
                }
            }
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            string maNhaCungCap = txtIDCungCap.Text.Trim();
            string tenNhaCungCap = txtTenNhaCungCap.Text.Trim();
            string soDienThoai = txtSoDienThoai.Text.Trim();
            string diaChi = txtDiaChi.Text.Trim();

            // Kiểm tra dữ liệu đầu vào
            if (!ValidateInput(tenNhaCungCap, soDienThoai))
                return;

            try
            {
                using (SqlConnection conn = new SqlConnection(connection))
                {
                    conn.Open();
                    string query = "UPDATE NhaCungCap SET TenNhaCungCap = @TenNhaCungCap, SoDienThoai = @SoDienThoai, DiaChi = @DiaChi " +
                                   "WHERE MaNhaCungCap = @MaNhaCungCap";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@MaNhaCungCap", maNhaCungCap);
                        cmd.Parameters.AddWithValue("@TenNhaCungCap", tenNhaCungCap);
                        cmd.Parameters.AddWithValue("@SoDienThoai", soDienThoai);
                        cmd.Parameters.AddWithValue("@DiaChi", diaChi);

                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Cập nhật thông tin nhà cung cấp thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Đóng form
                        this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi cập nhật nhà cung cấp: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private bool ValidateInput(string tenNhaCungCap, string soDienThoai)
        {
            // Kiểm tra tên nhà cung cấp
            if (string.IsNullOrEmpty(tenNhaCungCap) || !Regex.IsMatch(tenNhaCungCap, @"^[\p{L}0-9\s]+$"))
            {
                MessageBox.Show("Tên nhà cung cấp chỉ được chứa chữ cái (bao gồm tiếng Việt), số và không được để trống.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
    }
}
