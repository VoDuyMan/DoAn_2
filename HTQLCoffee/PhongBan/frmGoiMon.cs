using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HTQLCoffee.PhongBan
{
    public partial class frmGoiMon : Form
    {
        string connection = ConfigurationManager.ConnectionStrings["HTQLCoffee.Properties.Settings.QLCoffeeConnectionString"].ConnectionString;
        string maBan, tenBan, MaGoiMon;
        public frmGoiMon(string maGoiMon, string tenBan, string maBan)
        {
            InitializeComponent();
            this.MaGoiMon = maGoiMon; 
            this.maBan = maBan; 
            this.tenBan = tenBan;

        }

        private void LoadProductData(string loaiSanPham)
        {
            // Kết nối tới cơ sở dữ liệu và lấy danh sách sản phẩm theo loại
            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();
                // Truy vấn JOIN giữa bảng SanPham và QuanLyKho
                string query = @"SELECT sp.MaSanPham, sp.TenSanPham, sp.GiaBan, qlk.SoLuongTon, sp.DonViTinh
                         FROM SanPham sp
                         JOIN QuanLyKho qlk ON sp.MaSanPham = qlk.MaSanPham
                         WHERE sp.LoaiSanPham = @LoaiSanPham";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@LoaiSanPham", loaiSanPham);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        flowLayoutPanel.Controls.Clear(); // Xóa các button cũ trước khi thêm mới

                        while (reader.Read())
                        {
                            // Lấy thông tin sản phẩm
                            string maSanPham = reader["MaSanPham"].ToString();
                            string tenSanPham = reader["TenSanPham"].ToString();
                            decimal giaBan = Convert.ToDecimal(reader["GiaBan"]);
                            int soLuong = Convert.ToInt32(reader["SoLuongTon"]);
                            string donViTinh = reader["DonViTinh"].ToString();

                            // Tạo button cho mỗi sản phẩm
                            Button btnProduct = new Button();
                            btnProduct.Text = string.Format("{0}\n{1:N0}₫", tenSanPham, giaBan);
                            btnProduct.Size = new Size(140, 180);
                            btnProduct.Font = new Font("Microsoft Sans Serif", 12, FontStyle.Bold); // Đặt font chữ in đậm và kích thước 12
                            btnProduct.TextAlign = ContentAlignment.BottomCenter; // Văn bản nằm ở giữa đáy button
                            btnProduct.ImageAlign = ContentAlignment.TopCenter;
                            btnProduct.Tag = maSanPham; // Lưu mã sản phẩm vào tag để dùng sau này

                            // Sự kiện click để hiển thị chi tiết sản phẩm

                            btnProduct.Click += (s, e) =>
                            {
                                ShowProductDetails(maSanPham, tenSanPham, giaBan, soLuong, donViTinh, loaiSanPham);
                            };

                            // Chọn hình ảnh dựa vào trạng thái
                            try
                            {
                                if (loaiSanPham == "COFFE")
                                {
                                    btnProduct.Image = Image.FromFile(@"\HTQLCoffee\Image\3.png");
                                }
                                else if (loaiSanPham == "TRA")
                                {
                                    btnProduct.Image = Image.FromFile(@"\HTQLCoffee\Image\1.png");// Sử dụng @ để tránh escape
                                }
                                else if (loaiSanPham == "NUOCMAT")
                                {
                                    btnProduct.Image = Image.FromFile(@"\HTQLCoffee\Image\4.png"); // Sử dụng @ để tránh escape
                                }
                                else if (loaiSanPham == "FREEZE")
                                {
                                    btnProduct.Image = Image.FromFile(@"\HTQLCoffee\Image\2.png");// Sử dụng @ để tránh escape
                                }
                               
                            }
                            catch (FileNotFoundException ex)
                            {
                                MessageBox.Show("Không tìm thấy file hình ảnh: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Đã xảy ra lỗi khi tải hình ảnh: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }

                            flowLayoutPanel.Controls.Add(btnProduct);
                        }
                    }
                }
            }
        }

        private void ShowProductDetails(string maSanPham, string tenSanPham, decimal giaSanPham, int soLuong, string donViTinh, string loaiSanPham)
        {
            // Hiển thị thông tin sản phẩm trong các TextBox
            groupBoxProductDetails.Text = string.Format("Thông Tin Sản Phẩm: {0}", tenSanPham);
            txtMaSanPham.Text = maSanPham;
            txtTenSanPham.Text = tenSanPham;
            txtLoaiSanPham.Text = loaiSanPham;  // Hiển thị loại sản phẩm
            txtGiaBan.Text = giaSanPham.ToString("N0") + "₫";
            txtSoLuong.Text = soLuong.ToString();
            txtDonViTinh.Text = donViTinh;

            // Cập nhật thông tin trong groupbox
            groupBoxProductDetails.Visible = true;
        }

        private void btnTra_Click(object sender, EventArgs e)
        {
            LoadProductData("TRA");
        }

        private void btnNuocMat_Click(object sender, EventArgs e)
        {
            LoadProductData("NUOCMAT");
        }

        private void btnFreeze_Click(object sender, EventArgs e)
        {
            LoadProductData("FREEZE");
        }
        private void btnCoffee_Click(object sender, EventArgs e)
        {
            LoadProductData("COFFE");
        }

        private void btnDatMon_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtMaSanPham.Text))
            {
                MessageBox.Show("Vui lòng chọn sản phẩm để order.");
                return;
            }


            string maSanPham = txtMaSanPham.Text;
            decimal donGia = Convert.ToDecimal(txtGiaBan.Text.Replace("₫", "").Replace(",", ""));

            frmChiTietGoiMon frm = new frmChiTietGoiMon(MaGoiMon, maSanPham, tenBan, txtTenSanPham.Text, donGia, maBan);
            
            frm.ShowDialog();
            
        }

        private void flowLayoutPanel_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnDonHang_Click(object sender, EventArgs e)
        {
            string maBan = txtMaPhong.Text;
            frmDonHangHienCo frmDonHang = new frmDonHangHienCo(MaGoiMon, maBan);

            frmDonHang.FormClosed += (s, args) =>
            {
                frmGoiMon_Load(sender, e);
            };

            frmDonHang.ShowDialog();
        }

        private void frmGoiMon_Load(object sender, EventArgs e)
        {
            // Generate random MaGoiMon and set to the corresponding textbox
         
            txtMaDonHang.Text = MaGoiMon;
            txtMaPhong.Text = maBan;
            txtTenBan.Text = tenBan;



            string queryGoiMon = "SELECT GhiChu FROM GoiMon WHERE MaGoiMon = @MaGoiMon and TrangThai = 0";
            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(queryGoiMon, conn))
                {
                    // Thêm tham số cho câu truy vấn
                    cmd.Parameters.AddWithValue("@MaGoiMon", MaGoiMon);

                    // Sử dụng SqlDataReader để đọc kết quả từ cơ sở dữ liệu
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read()) // Nếu có kết quả trả về
                        {
                            // Lấy giá trị GhiChu từ cơ sở dữ liệu và gán vào TextBox
                            txtGhiChu.Text = reader["GhiChu"].ToString();
                        }
                        else
                        {
                            // Nếu không tìm thấy kết quả, có thể gán một thông báo mặc định hoặc làm trống
                            txtGhiChu.Text = "Không có ghi chú";
                        }
                    }
                }
            }
        }
        





    }
}
