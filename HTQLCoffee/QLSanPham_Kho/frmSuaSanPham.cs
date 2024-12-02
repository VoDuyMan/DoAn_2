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
    public partial class frmSuaSanPham : Form
    {
        string connection = ConfigurationManager.ConnectionStrings["HTQLCoffee.Properties.Settings.QLCoffeeConnectionString"].ConnectionString;

        string maSanPham;
        public frmSuaSanPham(string maSanPham)
        {
            InitializeComponent();
            this.maSanPham = maSanPham;
        }

        private void frmSuaSanPham_Load(object sender, EventArgs e)
        {
            // Thiết lập giá trị cho ComboBox Loại sản phẩm và Đơn vị tính
            cbxLoaiSanPham.Items.AddRange(new string[] { "COFFE", "TRA", "NUOCMAT", "FREEZE" });
            cbxDonViTinh.Items.AddRange(new string[] { "Ly", "Chai"});

            // Hiển thị thông tin sản phẩm để sửa
            LoadThongTinSanPham();
        }

        // Hàm hiển thị thông tin sản phẩm lên các TextBox
        private void LoadThongTinSanPham()
        {
            if (string.IsNullOrEmpty(maSanPham))
            {
                MessageBox.Show("Vui lòng chọn một sản phẩm để sửa thông tin sản phẩm!");
                return;
            }
            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();
                string query = "SELECT TenSanPham, GiaBan, LoaiSanPham, DonViTinh, SoLuongTon FROM SanPham INNER JOIN QuanLyKho ON SanPham.MaSanPham = QuanLyKho.MaSanPham WHERE SanPham.MaSanPham = @MaSanPham";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@MaSanPham", maSanPham);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            txtMaSanPham.Text = maSanPham;
                            txtTenSanPham.Text = reader["TenSanPham"].ToString();
                            cbxLoaiSanPham.SelectedItem = reader["LoaiSanPham"].ToString();
                            cbxDonViTinh.SelectedItem = reader["DonViTinh"].ToString();
                            txtGiaBan.Text = reader["GiaBan"].ToString().Replace(".00", "");

                            txtSoLuong.Text = reader["SoLuongTon"].ToString();
                        }
                        else
                        {
                            MessageBox.Show("Không tìm thấy sản phẩm.");
                            this.Close();
                        }
                    }
                }
            }
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            // Kiểm tra dữ liệu đầu vào (giống với form thêm sản phẩm)
            if (txtTenSanPham.Text.Length == 0 || txtTenSanPham.Text.Length > 20)
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

            // Cập nhật thông tin sản phẩm vào cơ sở dữ liệu
            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();

                // Cập nhật bảng SanPham
                string updateSanPham = @"UPDATE SanPham SET TenSanPham = @TenSanPham, GiaBan = @GiaBan, LoaiSanPham = @LoaiSanPham, DonViTinh = @DonViTinh WHERE MaSanPham = @MaSanPham";
                using (SqlCommand cmd = new SqlCommand(updateSanPham, conn))
                {
                    cmd.Parameters.AddWithValue("@MaSanPham", maSanPham);
                    cmd.Parameters.AddWithValue("@TenSanPham", txtTenSanPham.Text);
                    cmd.Parameters.AddWithValue("@GiaBan", giaBan);
                    cmd.Parameters.AddWithValue("@LoaiSanPham", cbxLoaiSanPham.SelectedItem.ToString());
                    cmd.Parameters.AddWithValue("@DonViTinh", cbxDonViTinh.SelectedItem.ToString());

                    cmd.ExecuteNonQuery();
                }

                // Cập nhật bảng QuanLyKho
                string updateKho = @"UPDATE QuanLyKho SET SoLuongTon = @SoLuongTon WHERE MaSanPham = @MaSanPham";
                using (SqlCommand cmd = new SqlCommand(updateKho, conn))
                {
                    cmd.Parameters.AddWithValue("@MaSanPham", maSanPham);
                    cmd.Parameters.AddWithValue("@SoLuongTon", soLuong);

                    cmd.ExecuteNonQuery();
                }
            }

            MessageBox.Show("Cập nhật sản phẩm thành công!");
            this.Close(); // Đóng form sau khi cập nhật xong
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
