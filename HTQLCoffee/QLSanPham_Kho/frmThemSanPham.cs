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

namespace HTQLCoffee.QLSanPham_Kho
{
    public partial class frmThemSanPham : Form
    {
        string connection = ConfigurationManager.ConnectionStrings["HTQLCoffee.Properties.Settings.QLCoffeeConnectionString"].ConnectionString;
        public frmThemSanPham()
        {
            InitializeComponent();
        }

        

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void frmThemSanPham_Load(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();
                string newMaSP;
                do
                {
                    newMaSP = GenerateRandomString(10); // Gọi hàm tạo mã ngẫu nhiên
                } while (DoesMaSanPhamExist(newMaSP, conn)); // Kiểm tra mã sản phẩm đã tồn tại hay chưa

                txtMaSanPham.Text = newMaSP; // Hiển thị mã sản phẩm lên textbox
            }

            // Thiết lập giá trị cho ComboBox Loại sản phẩm
            cbxLoaiSanPham.Items.AddRange(new string[] { "COFFE", "TRA", "NUOCMAT", "FREEZE"});
            cbxDonViTinh.Items.AddRange(new string[] { "Ly" });
        }

        private string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            StringBuilder result = new StringBuilder(length);
            Random random = new Random();

            for (int i = 0; i < length; i++)
            {
                result.Append(chars[random.Next(chars.Length)]);
            }
            return result.ToString();
        }


        // Hàm kiểm tra mã sản phẩm đã tồn tại trong cơ sở dữ liệu hay chưa
        private bool DoesMaSanPhamExist(string maSanPham, SqlConnection conn)
        {
            string query = "SELECT COUNT(*) FROM SanPham WHERE MaSanPham = @MaSanPham";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@MaSanPham", maSanPham);
                int count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            // Kiểm tra dữ liệu đầu vào
            if (string.IsNullOrWhiteSpace(txtTenSanPham.Text) || txtTenSanPham.Text.Length > 20)
            {
                MessageBox.Show("Tên sản phẩm không được rỗng và không quá 20 ký tự.");
                return;
            }

            if (cbxLoaiSanPham.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn loại sản phẩm.");
                return;
            }

            if (cbxDonViTinh.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn đơn vị tính.");
                return;
            }

            decimal giaBan;
            if (!decimal.TryParse(txtGiaBan.Text, out giaBan) || giaBan <= 1000)
            {
                MessageBox.Show("Giá bán phải là số lớn hơn 1000.");
                return;
            }

            int soLuong;
            if (!int.TryParse(txtSoLuong.Text, out soLuong) || soLuong <= 0 || soLuong > 100)
            {
                MessageBox.Show("Số lượng phải lớn hơn 0 và nhỏ hơn hoặc bằng 100.");
                return;
            }

            // Lấy mã chi nhánh duy nhất từ bảng ChiNhanh
            string maChiNhanh = GetMaChiNhanh();
            if (string.IsNullOrEmpty(maChiNhanh)) // Kiểm tra nếu mã chi nhánh không tìm thấy
            {
                return;
            }

            // Thêm sản phẩm vào cơ sở dữ liệu
            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();

                // Thêm vào bảng SanPham
                string insertSanPham = @"INSERT INTO SanPham (MaSanPham, TenSanPham, GiaBan, LoaiSanPham, DonViTinh, MaChiNhanh)
                                 VALUES (@MaSanPham, @TenSanPham, @GiaBan, @LoaiSanPham, @DonViTinh, @MaChiNhanh)";
                using (SqlCommand cmd = new SqlCommand(insertSanPham, conn))
                {
                    cmd.Parameters.AddWithValue("@MaSanPham", txtMaSanPham.Text);
                    cmd.Parameters.AddWithValue("@TenSanPham", txtTenSanPham.Text);
                    cmd.Parameters.AddWithValue("@GiaBan", giaBan);
                    cmd.Parameters.AddWithValue("@LoaiSanPham", cbxLoaiSanPham.SelectedItem.ToString());
                    cmd.Parameters.AddWithValue("@DonViTinh", cbxDonViTinh.SelectedItem.ToString());
                    cmd.Parameters.AddWithValue("@MaChiNhanh", maChiNhanh);

                    cmd.ExecuteNonQuery();
                }

                // Thêm vào bảng QuanLyKho
                string insertKho = @"INSERT INTO QuanLyKho (MaSanPham, SoLuongTon, MaChiNhanh)
                             VALUES (@MaSanPham, @SoLuongTon, @MaChiNhanh)";
                using (SqlCommand cmd = new SqlCommand(insertKho, conn))
                {
                    cmd.Parameters.AddWithValue("@MaSanPham", txtMaSanPham.Text);
                    cmd.Parameters.AddWithValue("@SoLuongTon", soLuong);
                    cmd.Parameters.AddWithValue("@MaChiNhanh", maChiNhanh);

                    cmd.ExecuteNonQuery();
                }
            }

            MessageBox.Show("Thêm sản phẩm thành công!");
            this.Close(); // Đóng form sau khi thêm xong
        }

        // Hàm lấy mã chi nhánh duy nhất
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

        private void btnHuy_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
