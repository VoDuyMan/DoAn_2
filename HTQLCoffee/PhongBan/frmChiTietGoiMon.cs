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
    public partial class frmChiTietGoiMon : Form
    {
        string connection = ConfigurationManager.ConnectionStrings["HTQLCoffee.Properties.Settings.QLCoffeeConnectionString"].ConnectionString;
        private string MaGoiMon, maSanPham, tenBan, maBan;

        

        decimal donGia;
       
        public frmChiTietGoiMon(string MaGoiMon, string maSanPham, string tenBan, string txtTenSanPham, decimal donGia, string maBan)
        {
            InitializeComponent();
            this.MaGoiMon = MaGoiMon;
            this.maSanPham = maSanPham;
            this.tenBan = tenBan;
            this.maBan = maBan;
            this.donGia = donGia;

            txtMaDonHang.Text = MaGoiMon;
            txtMaSanPham.Text = maSanPham;
            txtDonGia.Text = donGia.ToString("N0");
            txtSanPham.Text =txtTenSanPham ;

            numSoLuong.ValueChanged += new EventHandler(this.numSoLuong_ValueChanged);


        }

        private void btnCapNhat_Click(object sender, EventArgs e)
        {
            decimal soLuong = numSoLuong.Value;
            decimal thanhTien = soLuong * donGia;

            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();

                // Kiểm tra xem MaGoiMon đã tồn tại trong bảng GoiMon chưa
                bool donHangExists = DonHangExists(MaGoiMon, conn);

                if (!donHangExists)
                {
                    // Nếu MaGoiMon chưa tồn tại, thêm vào bảng GoiMon
                    string insertDonHangQuery = "INSERT INTO GoiMon (MaGoiMon, MaBan, NgayTao, TongTien, TrangThai) " +
                                                "VALUES (@MaGoiMon, @MaBan, @NgayTao, @TongTien, @TrangThai)";
                    using (SqlCommand cmdInsertDonHang = new SqlCommand(insertDonHangQuery, conn))
                    {
                        cmdInsertDonHang.Parameters.AddWithValue("@MaGoiMon", MaGoiMon);
                        cmdInsertDonHang.Parameters.AddWithValue("@MaBan", maBan); // Sử dụng maPhong thay vì maBan
                        cmdInsertDonHang.Parameters.AddWithValue("@NgayTao", DateTime.Now);
                        cmdInsertDonHang.Parameters.AddWithValue("@TongTien", thanhTien); // Tổng tiền là thanhTien ban đầu
                        cmdInsertDonHang.Parameters.AddWithValue("@TrangThai", "0"); // Trang thái 0 (chưa thanh toán)
                        cmdInsertDonHang.ExecuteNonQuery();
                    }
                    MessageBox.Show("Đơn gọi món đã được tạo mới.", "Thông báo");
                }

                // Kiểm tra xem sản phẩm đã tồn tại trong chi tiết gọi món chưa
                string queryCheck = "SELECT SoLuong FROM ChiTietGoiMon WHERE MaGoiMon = @MaGoiMon AND MaSanPham = @MaSanPham";
                using (SqlCommand cmdCheck = new SqlCommand(queryCheck, conn))
                {
                    cmdCheck.Parameters.AddWithValue("@MaGoiMon", MaGoiMon);
                    cmdCheck.Parameters.AddWithValue("@MaSanPham", maSanPham);
                    object result = cmdCheck.ExecuteScalar();

                    if (result != null)
                    {
                        // Sản phẩm đã tồn tại, cập nhật số lượng và thành tiền
                        decimal soLuongHienTai = Convert.ToDecimal(result);
                        decimal soLuongMoi = soLuongHienTai + soLuong;
                        decimal thanhTienMoi = soLuongMoi * donGia;

                        string queryUpdate = "UPDATE ChiTietGoiMon SET SoLuong = @SoLuong, ThanhTien = @ThanhTien WHERE MaGoiMon = @MaGoiMon AND MaSanPham = @MaSanPham";
                        using (SqlCommand cmdUpdate = new SqlCommand(queryUpdate, conn))
                        {
                            cmdUpdate.Parameters.AddWithValue("@SoLuong", soLuongMoi);
                            cmdUpdate.Parameters.AddWithValue("@ThanhTien", thanhTienMoi);
                            cmdUpdate.Parameters.AddWithValue("@MaGoiMon", MaGoiMon);
                            cmdUpdate.Parameters.AddWithValue("@MaSanPham", maSanPham);

                            cmdUpdate.ExecuteNonQuery(); // Cập nhật chi tiết gọi món
                        }

                        // Cập nhật tổng tiền đơn hàng
                        decimal tongTien = GetTongTienDonHang(MaGoiMon, conn);
                        string updateDonHangQuery = "UPDATE GoiMon SET TongTien = @TongTien WHERE MaGoiMon = @MaGoiMon";
                        using (SqlCommand cmdUpdateDonHang = new SqlCommand(updateDonHangQuery, conn))
                        {
                            cmdUpdateDonHang.Parameters.AddWithValue("@TongTien", tongTien);
                            cmdUpdateDonHang.Parameters.AddWithValue("@MaGoiMon", MaGoiMon);
                            cmdUpdateDonHang.ExecuteNonQuery(); // Cập nhật đơn hàng
                        }

                        MessageBox.Show("Số lượng đã được cập nhật.", "Thông báo");
                    }
                    else
                    {
                        // Sản phẩm chưa tồn tại, thêm mới vào ChiTietGoiMon
                        string queryChiTiet = "INSERT INTO ChiTietGoiMon (MaGoiMon, MaSanPham, SoLuong, DonGia, ThanhTien) " +
                                              "VALUES (@MaGoiMon, @MaSanPham, @SoLuong, @DonGia, @ThanhTien)";
                        using (SqlCommand cmdInsert = new SqlCommand(queryChiTiet, conn))
                        {
                            cmdInsert.Parameters.AddWithValue("@MaGoiMon", MaGoiMon);
                            cmdInsert.Parameters.AddWithValue("@MaSanPham", maSanPham);
                            cmdInsert.Parameters.AddWithValue("@SoLuong", soLuong);
                            cmdInsert.Parameters.AddWithValue("@DonGia", donGia);
                            cmdInsert.Parameters.AddWithValue("@ThanhTien", thanhTien);

                            cmdInsert.ExecuteNonQuery(); // Thêm chi tiết gọi món mới
                        }

                        // Tính tổng tiền đơn hàng
                        decimal tongTien = GetTongTienDonHang(MaGoiMon, conn) + thanhTien;

                        // Cập nhật hoặc thêm mới đơn hàng nếu cần
                        if (donHangExists)
                        {
                            // Cập nhật tổng tiền của đơn hàng đã tồn tại
                            string updateQuery = "UPDATE GoiMon SET TongTien = @TongTien WHERE MaGoiMon = @MaGoiMon";
                            using (SqlCommand cmd = new SqlCommand(updateQuery, conn))
                            {
                                cmd.Parameters.AddWithValue("@TongTien", tongTien);
                                cmd.Parameters.AddWithValue("@MaGoiMon", MaGoiMon);
                                cmd.ExecuteNonQuery(); // Cập nhật đơn hàng
                            }
                        }
                        else
                        {
                            // Cập nhật tổng tiền trong trường hợp MaGoiMon vừa được thêm mới
                            string updateQuery = "UPDATE GoiMon SET TongTien = @TongTien WHERE MaGoiMon = @MaGoiMon";
                            using (SqlCommand cmd = new SqlCommand(updateQuery, conn))
                            {
                                cmd.Parameters.AddWithValue("@TongTien", tongTien);
                                cmd.Parameters.AddWithValue("@MaGoiMon", MaGoiMon);
                                cmd.ExecuteNonQuery(); // Cập nhật đơn hàng
                            }
                        }

                        MessageBox.Show("Sản phẩm đã được thêm vào đơn hàng thành công.", "Thông báo");
                    }
                }
            }
            

            this.Close();
        }


        // Hàm kiểm tra xem DonHang đã tồn tại chưa
        private bool DonHangExists(string maDonHang, SqlConnection conn)
        {
            string query = "SELECT COUNT(*) FROM GoiMon WHERE MaGoiMon = @MaGoiMon";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@MaGoiMon", maDonHang);
                int count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
        }

        

        private void numSoLuong_ValueChanged(object sender, EventArgs e)
        {
            
            decimal soLuong = numSoLuong.Value;
            decimal thanhTien = soLuong * donGia;
            txtThanhTien.Text = thanhTien.ToString("N0");
        }

        private decimal GetTongTienDonHang(string maDonHang, SqlConnection conn)
        {
            string query = "SELECT SUM(ThanhTien) FROM ChiTietGoiMon WHERE MaGoiMon = @MaGoiMon";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@MaGoiMon", maDonHang);
                object result = cmd.ExecuteScalar();
                return result != DBNull.Value ? Convert.ToDecimal(result) : 0;
            }
        }


        private void btnHuy_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
