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
using HTQLCoffee.PhongBan;
using HTQLCoffee.KhachHang;
using HTQLCoffee.QLSanPham_Kho;
using System.IO;
using HTQLCoffee.NhanVien;
using HTQLCoffee.NhaCungCap;
using HTQLCoffee.QLHoaDon;
using HTQLCoffee.ThongKeDoanhThu;
using HTQLCoffee.QLChiPhi;
using HTQLCoffee.NhapHang;

namespace HTQLCoffee
{
    public partial class frmMain : Form
    {
        string connection = ConfigurationManager.ConnectionStrings["HTQLCoffee.Properties.Settings.QLCoffeeConnectionString"].ConnectionString;
        public frmMain()
        {
            InitializeComponent();
            LoadBanAn();
        }

        private void trangChuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripPhongBan.Visible = true;
            toolStripNhanVien.Visible = true;
            toolStripKhachHang.Visible = true;
            toolStripHoaDon.Visible = true;
            toolStripCaiDat.Visible = false;
            toolStripChiPhiKhac.Visible = false;
            toolStripNhaCungCap.Visible = false;
            toolStripNhapHang.Visible = false;
            toolStripSanPham.Visible = false;
            toolStripThongKe.Visible = false;
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            toolStripPhongBan.Visible = true;
            toolStripNhanVien.Visible = true;
            toolStripKhachHang.Visible = true;
            toolStripHoaDon.Visible = true;
            toolStripCaiDat.Visible = false;
            toolStripChiPhiKhac.Visible = false;
            toolStripNhaCungCap.Visible = false;
            toolStripNhapHang.Visible = false;
            toolStripSanPham.Visible = false;
            toolStripThongKe.Visible = false;


           

        }

        private void heThongToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripPhongBan.Visible = false;
            toolStripNhanVien.Visible = false;
            toolStripKhachHang.Visible = false;
            toolStripHoaDon.Visible = false;
            toolStripCaiDat.Visible = true;
            toolStripChiPhiKhac.Visible = false;
            toolStripNhaCungCap.Visible = false;
            toolStripNhapHang.Visible = false;
            toolStripSanPham.Visible = false;
            toolStripThongKe.Visible = true;
        }

        private void quanLyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripPhongBan.Visible = false;
            toolStripNhanVien.Visible = true;
            toolStripKhachHang.Visible = false;
            toolStripHoaDon.Visible = true;
            toolStripCaiDat.Visible = false;
            toolStripChiPhiKhac.Visible = true;
            toolStripNhaCungCap.Visible = false;
            toolStripNhapHang.Visible = false;
            toolStripSanPham.Visible = false;
            toolStripThongKe.Visible = true;
        }

        private void LoadBanAn()
        {
            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();
                string query = "SELECT MaBan, TenBan, TrangThai, NgayTao FROM BanAn ORDER BY TenBan ASC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        flowLayoutPanel.Controls.Clear();

                        while (reader.Read())
                        {
                            string tenBan = reader["TenBan"].ToString();
                            string maBan = reader["MaBan"].ToString();
                            string trangThai = reader["TrangThai"].ToString();
                            string NgayTao = reader["NgayTao"].ToString();


                            Button btn = new Button();
                            btn.Text = string.Format("Bàn {0}", tenBan);
                            btn.Size = new Size(135, 160);
                            btn.Font = new Font("Microsoft Sans Serif", 12, FontStyle.Bold);
                            btn.TextAlign = ContentAlignment.BottomRight;
                            btn.ImageAlign = ContentAlignment.TopCenter;

                            btn.Click += (s, e) =>
                            {
                                ShowTableDetails(maBan, tenBan, trangThai, NgayTao);
                            };

                            try
                            {
                                string imagePath = string.Empty;
                                switch (trangThai)
                                {
                                    case "Chưa đặt bàn":
                                        imagePath = @"\HTQLCoffee\Image\chuadatban.gif";
                                        break;
                                    case "Đã đặt bàn":
                                        imagePath = @"\HTQLCoffee\Image\dadatban.gif";
                                        break;
                                    case "Đang sử dụng":
                                        imagePath = @"\HTQLCoffee\Image\dangsudung.gif";
                                        break;
                                }
                                btn.Image = Image.FromFile(imagePath);
                            }
                            catch (FileNotFoundException ex)
                            {
                                MessageBox.Show("Không tìm thấy file hình ảnh: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Đã xảy ra lỗi khi tải hình ảnh: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }

                            flowLayoutPanel.Controls.Add(btn);
                        }
                    }
                }
            }
        }
        string maKhach;
        private void ShowTableDetails(string maBan, string tenBan, string trangThai, string ngayTao)
        {
            groupBox1.Text = string.Format("Thông Tin bàn {0}", tenBan);
            txtMaBan.Text = maBan.ToString();
            txtTenBan.Text = tenBan.ToString();
            txtNgayTao.Text = ngayTao;

            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();

                string countQuery = @"
        SELECT COUNT(*)
        FROM ChiTietGoiMon join GoiMon on ChiTietGoiMon.MaGoiMon = GoiMon.MaGoiMon
        WHERE MaBan = @maBan AND TrangThai = 0 ";

                using (SqlCommand countCmd = new SqlCommand(countQuery, conn))
                {
                    countCmd.Parameters.AddWithValue("@maBan", maBan);
                    int count = (int)countCmd.ExecuteScalar();

                    if (count > 0)
                    {
                        string updateQuery = @"
                UPDATE BanAn
                SET TrangThai = N'Đang sử dụng'
                WHERE MaBan = @maBan";

                        using (SqlCommand updateCmd = new SqlCommand(updateQuery, conn))
                        {
                            updateCmd.Parameters.AddWithValue("@maBan", maBan);
                            updateCmd.ExecuteNonQuery();
                        }

                        trangThai = "Đang sử dụng";
                    }
                }
            }

            txtTrangThai.Text = trangThai;

            if (trangThai == "Đã đặt bàn")
            {
                using (SqlConnection conn = new SqlConnection(connection))
                {
                    conn.Open();

                    string query = @"
        SELECT k.MaKhachHang, k.HoTen, d.NgayTao, d.GhiChu
        FROM DatBan d
        JOIN KhachHang k ON d.MaKhachHang = k.MaKhachHang
        WHERE d.MaBan = @maBan AND d.TinhTrang = N'Chưa thanh toán'";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@maBan", maBan);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Lấy thông tin từ reader
                                string tenKhachHang = reader["HoTen"].ToString();
                                string ghiChuDatBan = reader["GhiChu"].ToString();

                                txtKhachDangDat.Text = tenKhachHang;
                                txtGhiChu.Text = ghiChuDatBan;  // Cập nhật GhiChu vào TextBox

                                maKhach = reader["MaKhachHang"].ToString();
                            }
                            else
                            {
                                MessageBox.Show("Không tìm thấy thông tin đặt bàn.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                    }
                }
            }
            else
            {
                using (SqlConnection conn = new SqlConnection(connection))
                {
                    conn.Open();

                    string query = @"
        SELECT k.MaKhachHang, k.HoTen, d.GhiChu
        FROM DatBan d
        JOIN KhachHang k ON d.MaKhachHang = k.MaKhachHang
        WHERE d.MaBan = @maBan AND d.TinhTrang = N'Chưa thanh toán'";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@maBan", maBan);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Lấy thông tin từ reader nếu tồn tại
                                string tenKhachHang = reader["HoTen"].ToString();
                                string ghiChuDatBan = reader["GhiChu"].ToString();

                                txtKhachDangDat.Text = tenKhachHang;
                                txtGhiChu.Text = ghiChuDatBan;  // Cập nhật GhiChu vào TextBox

                                maKhach = reader["MaKhachHang"].ToString();
                            }
                            else
                            {
                                // Không tìm thấy, gán giá trị trống
                                txtKhachDangDat.Text = string.Empty;
                                txtGhiChu.Text = string.Empty;
                            }
                        }
                    }
                }
            }


            // Cập nhật thông tin trong groupbox
            groupBox1.Visible = true; // Hiển thị groupbox nếu cần
        }


        private void toolStripPhongBan_Click(object sender, EventArgs e)
        {

            // Gọi phương thức LoadBanAn
            LoadBanAn();
        }



        private void btnThem_Click(object sender, EventArgs e)
        {
            frmThemBan frm = new frmThemBan();
            frm.FormClosed += (s, args) =>
            {
                LoadBanAn();
            };
            frm.ShowDialog();
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (txtTenBan.Text == "")
            {
                MessageBox.Show("Vui lòng chọn bàn để sửa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            frmSuaBan frm = new frmSuaBan(txtMaBan.Text);

            frm.FormClosed += (s, args) =>
            {
                LoadBanAn(); 
            };

            frm.ShowDialog();
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (txtTenBan.Text == "")
            {
                MessageBox.Show("Vui lòng chọn bàn cần xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Kiểm tra trạng thái của bàn ăn
            if (txtTrangThai.Text != "Chưa đặt bàn")
            {
                MessageBox.Show("Bàn đã được đặt, không thể xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            

            // Liệt kê các bảng liên quan sẽ bị xóa
            string message = "Bạn có chắc chắn muốn xóa bàn ăn này?\nCác bảng dữ liệu liên quan sẽ bị xóa:\n" +
                             "- Gọi món\n- Chi tiết gọi món\n- Đặt bàn\n- Hóa đơn";

            var confirmResult = MessageBox.Show(message, "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirmResult == DialogResult.Yes)
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(connection))
                    {
                        conn.Open();

                        // Bắt đầu giao dịch (transaction) để xóa dữ liệu liên quan
                        SqlTransaction transaction = conn.BeginTransaction();

                        try
                        {
                            // Xóa dữ liệu trong bảng ChiTietGoiMon
                            string deleteChiTietGoiMonQuery = "DELETE FROM ChiTietGoiMon WHERE MaGoiMon IN (SELECT MaGoiMon FROM GoiMon WHERE MaBan = @MaBan)";
                            SqlCommand cmdChiTietGoiMon = new SqlCommand(deleteChiTietGoiMonQuery, conn, transaction);
                            cmdChiTietGoiMon.Parameters.AddWithValue("@MaBan", txtMaBan.Text);
                            cmdChiTietGoiMon.ExecuteNonQuery();

                            // Xóa dữ liệu trong bảng GoiMon
                            string deleteGoiMonQuery = "DELETE FROM GoiMon WHERE MaBan = @MaBan";
                            SqlCommand cmdGoiMon = new SqlCommand(deleteGoiMonQuery, conn, transaction);
                            cmdGoiMon.Parameters.AddWithValue("@MaBan", txtMaBan.Text);
                            cmdGoiMon.ExecuteNonQuery();

                            // Xóa dữ liệu trong bảng DatBan
                            string deleteDatBanQuery = "DELETE FROM DatBan WHERE MaBan = @MaBan";
                            SqlCommand cmdDatBan = new SqlCommand(deleteDatBanQuery, conn, transaction);
                            cmdDatBan.Parameters.AddWithValue("@MaBan", txtMaBan.Text);
                            cmdDatBan.ExecuteNonQuery();

                            // Xóa dữ liệu trong bảng HoaDon
                            string deleteHoaDonQuery = "DELETE FROM HoaDon WHERE MaBan = @MaBan";
                            SqlCommand cmdHoaDon = new SqlCommand(deleteHoaDonQuery, conn, transaction);
                            cmdHoaDon.Parameters.AddWithValue("@MaBan", txtMaBan.Text);
                            cmdHoaDon.ExecuteNonQuery();

                            // Cuối cùng, xóa dữ liệu trong bảng BanAn
                            string deleteBanAnQuery = "DELETE FROM BanAn WHERE MaBan = @MaBan";
                            SqlCommand cmdBanAn = new SqlCommand(deleteBanAnQuery, conn, transaction);
                            cmdBanAn.Parameters.AddWithValue("@MaBan", txtMaBan.Text);
                            cmdBanAn.ExecuteNonQuery();

                            // Commit giao dịch (transaction)
                            transaction.Commit();

                            MessageBox.Show("Xóa bàn ăn thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadBanAn(); // Reload danh sách bàn sau khi xóa
                        }
                        catch (Exception ex)
                        {
                            // Rollback nếu có lỗi xảy ra
                            transaction.Rollback();
                            MessageBox.Show("Lỗi khi xóa bàn ăn: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi kết nối cơ sở dữ liệu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void toolStripKhachHang_Click(object sender, EventArgs e)
        {
            frmKhachHang frm = new frmKhachHang();
            frm.ShowDialog();
                
        }

        private void btnDatBan_Click(object sender, EventArgs e)
        {
            if(txtTrangThai.Text != "Chưa đặt bàn")
            {
                MessageBox.Show("Bàn đang có trạng thái là "+ txtTrangThai.Text +" vui lòng chọn bàn trống để đặt bàn mới!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (txtTenBan.Text == "")
            {
                MessageBox.Show("Vui lòng chọn bàn để đặt!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            frmDatBan frm = new frmDatBan(txtMaBan.Text, txtTenBan.Text); // Truyền mã bàn sang frmDatBan
            frm.FormClosed += (s, args) =>
            {
                LoadBanAn(); // Tải lại danh sách bàn nếu cần
            };
            frm.ShowDialog();
        }



        private void btnGoiMon_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtTenBan.Text.Trim()))
            {
                MessageBox.Show("Vui lòng chọn một bàn để gọi món.");
                return;
            }

            string maBan = GetMaBanByTen(txtTenBan.Text.Trim());

            if (string.IsNullOrEmpty(maBan))
            {
                MessageBox.Show("Không tìm thấy mã bàn.");
                return; // Hoặc xử lý lỗi tùy theo yêu cầu
            }

            string maGoiMon = null;
          
            // Truy vấn kiểm tra đơn hàng có mã bàn và trạng thái là false
            string checkOrderQuery = @"
            SELECT MaGoiMon
            FROM GoiMon
            WHERE MaBan = @MaBan AND TrangThai ='0'"; // Chỉ lấy đơn hàng chưa thanh toán

            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(checkOrderQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@MaBan", maBan ?? (object)DBNull.Value);


                    try
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Lấy mã đơn hàng nếu có trạng thái là false
                                maGoiMon = reader["MaGoiMon"].ToString();
                              
                            }
                            else
                            {
                                // Nếu không có dữ liệu trả về (không có mã bàn nào với trạng thái là false)
                                maGoiMon = null;
                               
                            }
                        }
                    }
                    catch (SqlException ex)
                    {
                        MessageBox.Show("Lỗi khi kiểm tra mã đơn hàng: " + ex.Message);
                        return;
                    }
                }
            }

           if (maGoiMon == null)
            {
                // Tạo đơn hàng mới nếu không tìm thấy đơn hàng nào với mã bàn và trạng thái là false
                maGoiMon = GenerateOrderCode();
                InsertNewOrder(maGoiMon, GetMaBanByTen(txtTenBan.Text));
            }

            // Chuyển mã đơn hàng hợp lệ sang form khác
            frmGoiMon frm = new frmGoiMon(maGoiMon, txtTenBan.Text, GetMaBanByTen(txtTenBan.Text));
            frm.FormClosed += (s, args) =>
            {
                LoadBanAn();
            };

            frm.ShowDialog();
        }

        public string GetMaBanByTen(string tenBan)
        {
            string maBan = null;

            // Câu truy vấn SQL để lấy MaBan từ TenBan
            string query = "SELECT MaBan FROM BanAn WHERE TenBan = @TenBan";

            // Tạo kết nối đến cơ sở dữ liệu
            using (SqlConnection conn = new SqlConnection(connection))
            {
                try
                {
                    conn.Open();

                    // Tạo đối tượng SqlCommand
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        // Thêm tham số TenBan vào truy vấn
                        command.Parameters.AddWithValue("@TenBan", tenBan);

                        // Thực thi truy vấn và lấy kết quả
                        var result = command.ExecuteScalar();

                        if (result != DBNull.Value && result != null)
                        {
                            maBan = result.ToString();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Lỗi khi truy vấn: " + ex.Message);
                }
            }

            return maBan;
        }

        private string GenerateOrderCode()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Random random = new Random();
            string randomString = new string(Enumerable.Repeat(chars, 10)
                                                  .Select(s => s[random.Next(s.Length)]).ToArray());
            return randomString;
        }

        private void InsertNewOrder(string maGoiMon, string maBan)
        {
            string insertDonHangQuery = "INSERT INTO GoiMon (MaGoiMon, MaBan, NgayTao, TrangThai) VALUES (@MaGoiMon, @MaBan, GETDATE(), 0)";

            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(insertDonHangQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@MaGoiMon", maGoiMon);
                    cmd.Parameters.AddWithValue("@MaBan", maBan);

                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch (SqlException ex)
                    {
                        MessageBox.Show("Lỗi khi thêm đơn hàng mới: " + ex.Message);
                    }
                }
            }
        }

        private void btnThanhToan_Click(object sender, EventArgs e)
        {
            if(txtTrangThai.Text == "Chưa đặt bàn")
            {
                MessageBox.Show("Bàn này chưa được đặt nên không thể thanh toán !!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return;
            }    
            frmThanhToan frm = new frmThanhToan(txtMaBan.Text, txtTenBan.Text, txtKhachDangDat.Text, txtNgayTao.Text);
            frm.FormClosed += (s, args) =>
            {
                LoadBanAn();
            };
            frm.ShowDialog();

        }

        private void khoHangToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripPhongBan.Visible = false;
            toolStripNhanVien.Visible = false;
            toolStripKhachHang.Visible = false;
            toolStripHoaDon.Visible = false;
            toolStripCaiDat.Visible = false;
            toolStripChiPhiKhac.Visible = false;
            toolStripNhaCungCap.Visible = true;
            toolStripNhapHang.Visible = true;
            toolStripSanPham.Visible = true;
            toolStripThongKe.Visible = false;
        }

        private void toolStripSanPham_Click(object sender, EventArgs e)
        {
            frmQLSanPham_Kho frm = new frmQLSanPham_Kho();
            frm.ShowDialog();
        }

        private void btnHuyDat_Click(object sender, EventArgs e)
        {
                // Kiểm tra xem người dùng đã chọn bàn hay chưa
                if (string.IsNullOrWhiteSpace(txtMaBan.Text))
                {
                    MessageBox.Show("Vui lòng chọn bàn cần dừng!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Kiểm tra trạng thái bàn hiện tại
                string trangThaiHienTai = txtTrangThai.Text;
                string tenBan = txtTenBan.Text;

                // Nếu bàn đang là "Chưa đặt bàn"
                if (trangThaiHienTai == "Chưa đặt bàn")
                {
                    MessageBox.Show("Bàn này chưa có động thái đặc biệt, không thể dừng!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (SqlConnection conn = new SqlConnection(connection))
                {
                    conn.Open();

                    if (trangThaiHienTai == "Đang sử dụng")
                    {
                        DialogResult result = MessageBox.Show("Bàn đang trong trạng thái 'Đang sử dụng' và chưa thanh toán, không thể dừng",
                                                              "Xác nhận", MessageBoxButtons.OK, MessageBoxIcon.Question);
                        return;

                    }
                    // Nếu bàn đang là "Đã đặt bàn"
                    else if (trangThaiHienTai == "Đã đặt bàn")
                    {
                        DialogResult result = MessageBox.Show("Bàn đang trong trạng thái 'Đã đặt bàn', bạn có chắc chắn muốn hủy đặt bàn không?",
                                                              "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                        if (result == DialogResult.Yes)
                        {
                            // Lấy mã đặt bàn dựa trên mã bàn và tình trạng là "Chưa thanh toán"
                            string selectQuery = "SELECT MaDatban FROM DatBan WHERE MaBan = @MaBan AND TinhTrang = N'Chưa thanh toán'";
                            string maDatBan;

                            using (SqlCommand selectCmd = new SqlCommand(selectQuery, conn))
                            {
                                selectCmd.Parameters.AddWithValue("@MaBan", txtMaBan.Text);

                                maDatBan = (string)selectCmd.ExecuteScalar();
                            }

                            if (maDatBan != null)
                            {
                                string updateDatPhongQuery = "Delete from DatBan where MaDatBan = @maDatBan";
                                using (SqlCommand updateCmd = new SqlCommand(updateDatPhongQuery, conn))
                                {
                                    updateCmd.Parameters.AddWithValue("@maDatBan", maDatBan);
                                    updateCmd.ExecuteNonQuery();
                                }

                                // Cập nhật trạng thái của bàn hát trong bảng PhongHat
                                string updateBanAnQuery = "UPDATE BanAn SET TrangThai = N'Chưa đặt bàn' WHERE MaBan = @MaBan";
                                using (SqlCommand updateCmd = new SqlCommand(updateBanAnQuery, conn))
                                {
                                    updateCmd.Parameters.AddWithValue("@MaBan", txtMaBan.Text);
                                    updateCmd.ExecuteNonQuery();
                                }

                                MessageBox.Show("Bàn đã được hủy thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                            {
                                MessageBox.Show("Không tìm thấy mã đặt bàn chưa thanh toán cho bàn này.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                    }
                }

                // Cập nhật giao diện sau khi dừng bàn
                txtTrangThai.Text = "Chưa đặt bàn";
                LoadBanAn();
        }

        private void toolStripNhapHang_Click(object sender, EventArgs e)
        {
            frmQuanLyNhapHang frm = new frmQuanLyNhapHang();
            frm.ShowDialog();
        }

        private void toolStripThongKe_Click(object sender, EventArgs e)
        {
            frmThongKe frm = new frmThongKe();
            frm.ShowDialog();
        }

        private void toolStripThoat_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                  "Bạn có chắc chắn muốn thoát ứng dụng không?",
                  "Xác nhận thoát",
                  MessageBoxButtons.YesNo,
                  MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void toolStripNhanVien_Click(object sender, EventArgs e)
        {
            frmQLNhanVien frm = new frmQLNhanVien();
            frm.ShowDialog(); 
        }

        private void toolStripNhaCungCap_Click(object sender, EventArgs e)
        {
            frmNhaCungCap frm = new frmNhaCungCap();
            frm.ShowDialog();
        }

        private void toolStripHoaDon_Click(object sender, EventArgs e)
        {
            frmQuanLyHoaDon frm = new frmQuanLyHoaDon();
            frm.ShowDialog();
        }

        private void toolStripCaiDat_Click(object sender, EventArgs e)
        {
            frmCaiDatHeThong frm = new frmCaiDatHeThong();
            frm.ShowDialog();
        }

        

        private void toolStripChiPhiKhac_Click(object sender, EventArgs e)
        {
            frmQLChiPhi frm = new frmQLChiPhi();
            frm.ShowDialog();
        }
    }
}
