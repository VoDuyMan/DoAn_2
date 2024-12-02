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
    public partial class frmDonHangHienCo : Form
    {
        string connection = ConfigurationManager.ConnectionStrings["HTQLCoffee.Properties.Settings.QLCoffeeConnectionString"].ConnectionString;
        string MaGoiMon;
        string MaBan;
        public frmDonHangHienCo(string MaGoiMon, string maBan)
        {
            InitializeComponent();
            this.MaGoiMon = MaGoiMon;
            this.MaBan = maBan;
        }

        private void frmDonHangHienCo_Load(object sender, EventArgs e)
        {
            LoadDonHang(); 
        }

        private void LoadDonHang()
        {
            string queryChiTietGoiMon = @"
    SELECT 
        GM.MaGoiMon, 
        GM.MaBan, 
        GM.NgayTao, 
        GM.TongTien, 
        GM.TrangThai, 
        GM.GhiChu,
        CTGM.MaSanPham, 
        SP.TenSanPham, 
        SP.LoaiSanPham, 
        SP.DonViTinh, 
        CTGM.SoLuong, 
        CTGM.DonGia, 
        CTGM.ThanhTien
    FROM GoiMon AS GM
    INNER JOIN ChiTietGoiMon AS CTGM ON GM.MaGoiMon = CTGM.MaGoiMon
    INNER JOIN SanPham AS SP ON CTGM.MaSanPham = SP.MaSanPham
    WHERE GM.MaBan = @MaBan AND GM.TrangThai = 0"; // Thêm điều kiện MaBan và TrangThai

            string queryDonHang = "SELECT NgayTao, TongTien, TrangThai, GhiChu FROM GoiMon WHERE MaBan = @MaBan AND TrangThai = 0";

            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();

                // Lấy dữ liệu chi tiết gọi món
                using (SqlCommand cmd = new SqlCommand(queryChiTietGoiMon, conn))
                {
                    cmd.Parameters.AddWithValue("@MaBan", MaBan);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dataTable = new DataTable();
                    try
                    {
                        adapter.Fill(dataTable);
                        dgvCTDH.DataSource = dataTable;

                        // Cấu hình lại DataGridView hiển thị đúng header và format
                        dgvCTDH.Columns["MaGoiMon"].HeaderText = "Mã Đơn Hàng";
                        dgvCTDH.Columns["MaSanPham"].HeaderText = "Mã Sản Phẩm";
                        dgvCTDH.Columns["TenSanPham"].HeaderText = "Tên Sản Phẩm";
                        dgvCTDH.Columns["SoLuong"].HeaderText = "Số Lượng";
                        dgvCTDH.Columns["DonGia"].HeaderText = "Đơn Giá";
                        dgvCTDH.Columns["ThanhTien"].HeaderText = "Thành Tiền";
                        dgvCTDH.Columns["LoaiSanPham"].HeaderText = "Loại Sản Phẩm";
                        dgvCTDH.Columns["DonViTinh"].HeaderText = "Đơn Vị Tính";

                        // Định dạng cột tiền tệ (nếu cần)
                        dgvCTDH.Columns["DonGia"].DefaultCellStyle.Format = "N0";
                        dgvCTDH.Columns["ThanhTien"].DefaultCellStyle.Format = "N0";
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi khi tải dữ liệu chi tiết gọi món: " + ex.Message);
                    }
                }

                // Lấy thông tin đơn hàng
                using (SqlCommand cmd = new SqlCommand(queryDonHang, conn))
                {
                    cmd.Parameters.AddWithValue("@MaBan", MaBan);
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        txtNgayTao.Text = Convert.ToDateTime(reader["NgayTao"]).ToString("dd/MM/yyyy HH:mm:ss");
                        txtTongTien.Text = reader["TongTien"].ToString();
                        txtTrangThai.Text = Convert.ToBoolean(reader["TrangThai"]) ? "Đã thanh toán" : "Chưa thanh toán";
                        txtGhiChu.Text = reader["GhiChu"].ToString();
                    }
                    reader.Close();
                }
            }

            // Tính tổng tiền
            TinhTongTien();
        }




        private void TinhTongTien()
        {
            decimal tongTien = 0;
            foreach (DataGridViewRow row in dgvCTDH.Rows)
            {
                tongTien += Convert.ToDecimal(row.Cells["ThanhTien"].Value);
            }

            // Định dạng số tiền theo kiểu tiền tệ của Việt Nam với đơn vị "VND"
            txtTongTien.Text = tongTien.ToString("N0") + " VND";
        }

        private void btnCapNhat_Click_1(object sender, EventArgs e)
        {
            string ghiChuMoi = txtGhiChu.Text;

            string queryUpdateGhiChu = "UPDATE GoiMon SET GhiChu = @GhiChu WHERE MaGoiMon = @MaGoiMon";

            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(queryUpdateGhiChu, conn))
                {
                    cmd.Parameters.AddWithValue("@GhiChu", ghiChuMoi);
                    cmd.Parameters.AddWithValue("@MaGoiMon", MaGoiMon);

                    cmd.ExecuteNonQuery();
                }
            }

            MessageBox.Show("Ghi chú đã được cập nhật!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }

        private void btnHuy_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }
        private void btnTraHang_Click(object sender, EventArgs e)
        {
            if (dgvCTDH.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn sản phẩm để thực hiện trả hàng.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataGridViewRow selectedRow = dgvCTDH.SelectedRows[0];
            string maSanPham = selectedRow.Cells["MaSanPham"].Value.ToString();
            decimal soLuongHienTai = Convert.ToDecimal(selectedRow.Cells["SoLuong"].Value);
            decimal donGia = Convert.ToDecimal(selectedRow.Cells["DonGia"].Value);
            decimal soLuongTra = numSoLuongTra.Value;

            if (soLuongTra > soLuongHienTai)
            {
                MessageBox.Show("Số lượng trả hàng lớn hơn số lượng order, vui lòng chọn lại số lượng.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            decimal thanhTienTra = soLuongTra * donGia;

            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();

                // Nếu số lượng sau khi trả bằng 0 thì xoá sản phẩm khỏi ChiTietGoiMon
                if (soLuongHienTai - soLuongTra == 0)
                {
                    string queryDelete = "DELETE FROM ChiTietGoiMon WHERE MaGoiMon = @MaGoiMon AND MaSanPham = @MaSanPham";
                    using (SqlCommand cmd = new SqlCommand(queryDelete, conn))
                    {
                        cmd.Parameters.AddWithValue("@MaGoiMon", MaGoiMon);
                        cmd.Parameters.AddWithValue("@MaSanPham", maSanPham);
                        cmd.ExecuteNonQuery();
                    }
                }
                else
                {
                    // Cập nhật số lượng và thành tiền nếu sản phẩm vẫn còn sau khi trả
                    string queryUpdateChiTiet = "UPDATE ChiTietGoiMon SET SoLuong = SoLuong - @SoLuongTra, ThanhTien = ThanhTien - @ThanhTienTra WHERE MaGoiMon = @MaGoiMon AND MaSanPham = @MaSanPham";
                    using (SqlCommand cmd = new SqlCommand(queryUpdateChiTiet, conn))
                    {
                        cmd.Parameters.AddWithValue("@SoLuongTra", soLuongTra);
                        cmd.Parameters.AddWithValue("@ThanhTienTra", thanhTienTra);
                        cmd.Parameters.AddWithValue("@MaGoiMon", MaGoiMon);
                        cmd.Parameters.AddWithValue("@MaSanPham", maSanPham);
                        cmd.ExecuteNonQuery();
                    }
                }

                // Cập nhật lại tổng tiền trong bảng DonHang
                decimal tongTienMoi = Convert.ToDecimal(txtTongTien.Text.Replace(" VND", "").Replace(",", "")) - thanhTienTra;
                string queryUpdateDonHang = "UPDATE GoiMon SET TongTien = @TongTien WHERE MaGoiMon = @MaGoiMon";
                using (SqlCommand cmd = new SqlCommand(queryUpdateDonHang, conn))
                {
                    cmd.Parameters.AddWithValue("@TongTien", tongTienMoi);
                    cmd.Parameters.AddWithValue("@MaGoiMon", MaGoiMon);
                    cmd.ExecuteNonQuery();
                }

                // Kiểm tra nếu không còn sản phẩm trong bảng ChiTietGoiMon
                string queryCheckEmpty = "SELECT COUNT(*) FROM ChiTietGoiMon WHERE MaGoiMon = @MaGoiMon";
                using (SqlCommand cmd = new SqlCommand(queryCheckEmpty, conn))
                {
                    cmd.Parameters.AddWithValue("@MaGoiMon", MaGoiMon);
                    int count = (int)cmd.ExecuteScalar();

                    if (count == 0)
                    {
                        // Nếu không còn sản phẩm, cập nhật trạng thái bàn ăn
                        string queryUpdateBanAn = "UPDATE BanAn SET TrangThai = N'Chưa đặt bàn' WHERE MaBan = @MaBan";
                        using (SqlCommand cmdUpdateBanAn = new SqlCommand(queryUpdateBanAn, conn))
                        {
                            cmdUpdateBanAn.Parameters.AddWithValue("@MaBan", MaBan);
                            cmdUpdateBanAn.ExecuteNonQuery();
                        }
                    }
                }
            }

            MessageBox.Show("Cập nhật trả hàng thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LoadDonHang(); // Cập nhật lại dữ liệu hiển thị
        }



        private void dgvCTDH_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        

        
    }
}
