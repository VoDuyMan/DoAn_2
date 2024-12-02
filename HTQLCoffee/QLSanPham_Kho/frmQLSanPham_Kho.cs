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

namespace HTQLCoffee.QLSanPham_Kho
{
    public partial class frmQLSanPham_Kho : Form
    {
        string maSanPham;
        string connection = ConfigurationManager.ConnectionStrings["HTQLCoffee.Properties.Settings.QLCoffeeConnectionString"].ConnectionString;
        public frmQLSanPham_Kho()
        {
            InitializeComponent();
        }

        private void frmQLSanPham_Kho_Load(object sender, EventArgs e)
        {

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
                                    btnProduct.Image = Image.FromFile(@"D:\HTQLCoffee\Image\3.png");
                                }
                                else if (loaiSanPham == "TRA")
                                {
                                    btnProduct.Image = Image.FromFile(@"D:\HTQLCoffee\Image\1.png");// Sử dụng @ để tránh escape
                                }
                                else if (loaiSanPham == "NUOCMAT")
                                {
                                    btnProduct.Image = Image.FromFile(@"D:\HTQLCoffee\Image\4.png"); // Sử dụng @ để tránh escape
                                }
                                else if (loaiSanPham == "FREEZE")
                                {
                                    btnProduct.Image = Image.FromFile(@"D:\HTQLCoffee\Image\2.png");// Sử dụng @ để tránh escape
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

        private void btnThemSanPham_Click(object sender, EventArgs e)
        {
            frmThemSanPham frmThemSanPham = new frmThemSanPham();
            frmThemSanPham.FormClosed += (s, args) =>
            {
                LoadAllProducts();
            };

            frmThemSanPham.ShowDialog();
        }

        private void LoadAllProducts()
        {
            // Bạn có thể thêm tùy chọn nếu muốn load toàn bộ hoặc gọi lại từng loại.
            LoadProductData("COFFE");
            LoadProductData("TRA");
            LoadProductData("NUOCMAT");
            LoadProductData("FREEZE");
            
        }

        private void btnSuaSanPham_Click(object sender, EventArgs e)
        { 
            if(string.IsNullOrEmpty(txtMaSanPham.Text))
            {
                MessageBox.Show("Vui lòng chọn một sản phẩm để sửa thông tin sản phẩm!");
                return;
            }    
            frmSuaSanPham frmSuaSanPham = new frmSuaSanPham(txtMaSanPham.Text);
            frmSuaSanPham.FormClosed += (s, args) =>
            {
                LoadAllProducts();
            };

            frmSuaSanPham.ShowDialog();
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnThongKe_Click(object sender, EventArgs e)
        {
            frmThongKeSanPham frmThongKe = new frmThongKeSanPham();
            frmThongKe.ShowDialog();
        }

        private void btnCanhBao_Click(object sender, EventArgs e)
        {
            frmCanhBao frmCanhBao = new frmCanhBao();
            frmCanhBao.ShowDialog();
        }

        private void btnXoaSanPham_Click(object sender, EventArgs e)
        {
            // Kiểm tra xem có sản phẩm nào được chọn hay chưa
            if (string.IsNullOrEmpty(txtMaSanPham.Text))
            {
                MessageBox.Show("Vui lòng chọn một sản phẩm để xóa.");
                return;
            }

            // Xác nhận việc xóa sản phẩm
            DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn xóa sản phẩm này không?",
                                                  "Xác nhận xóa",
                                                  MessageBoxButtons.OKCancel,
                                                  MessageBoxIcon.Warning);

            if (result == DialogResult.OK)
            {
                maSanPham = txtMaSanPham.Text;

                using (SqlConnection conn = new SqlConnection(connection))
                {
                    conn.Open();

                    // Bắt đầu một transaction để đảm bảo tính toàn vẹn dữ liệu
                    SqlTransaction transaction = conn.BeginTransaction();

                    try
                    {
                        // Xóa sản phẩm khỏi bảng QuanLyKho trước
                        string deleteQuanLyKho = @"DELETE FROM QuanLyKho WHERE MaSanPham = @MaSanPham";
                        using (SqlCommand cmd = new SqlCommand(deleteQuanLyKho, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@MaSanPham", maSanPham);
                            cmd.ExecuteNonQuery();
                        }

                        // Xóa sản phẩm khỏi bảng NhapHang
                        string deleteNhapHang = @"DELETE FROM NhapHang WHERE MaSanPham = @MaSanPham";
                        using (SqlCommand cmd = new SqlCommand(deleteNhapHang, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@MaSanPham", maSanPham);
                            cmd.ExecuteNonQuery();
                        }

                        // Kiểm tra sự tồn tại của sản phẩm trong ChiTietDonHang và lấy trạng thái, mã phòng từ DonHang
                        string queryChiTietGoiMon = @"
                            SELECT dh.TrangThai, dh.MaBan
                            FROM ChiTietGoiMon ctdh
                            INNER JOIN GoiMon dh ON ctdh.MaGoiMon = dh.MaGoiMon
                            WHERE ctdh.MaSanPham = @MaSanPham";

                        using (SqlCommand cmd = new SqlCommand(queryChiTietGoiMon, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@MaSanPham", maSanPham);
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    bool trangThai = (bool)reader["TrangThai"];
                                    string maBan = reader["MaBan"].ToString();

                                    // Nếu trạng thái là 1 (đã thanh toán), xóa sản phẩm khỏi ChiTietDonHang
                                    if (trangThai)
                                    {
                                        // Xóa sản phẩm khỏi ChiTietDonHang
                                        string deleteChiTietGoiMon = @"DELETE FROM ChiTietGoiMon WHERE MaSanPham = @MaSanPham";
                                        using (SqlCommand cmdDelete = new SqlCommand(deleteChiTietGoiMon, conn, transaction))
                                        {
                                            cmdDelete.Parameters.AddWithValue("@MaSanPham", maSanPham);
                                            cmdDelete.ExecuteNonQuery();
                                        }
                                    }
                                    else
                                    {
                                        // Thông báo cho người dùng biết sản phẩm đang được order
                                        MessageBox.Show("Sản phẩm hiện đang được order tại bàn " + maBan + " và không thể tiến hành xóa sản phẩm lúc này!",
                                                        "Thông báo",
                                                        MessageBoxButtons.OK,
                                                        MessageBoxIcon.Warning);
                                        transaction.Rollback();
                                        return;
                                    }
                                }
                            }
                        }

                        // Commit transaction sau khi xóa thành công
                        transaction.Commit();
                        MessageBox.Show("Sản phẩm đã được xóa thành công.");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show("Có lỗi xảy ra khi xóa sản phẩm " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                // Làm mới danh sách sản phẩm sau khi xóa
                LoadAllProducts();
            }
        }
    }
}
