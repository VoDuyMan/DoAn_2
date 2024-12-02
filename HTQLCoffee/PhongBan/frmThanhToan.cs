using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Configuration;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using DrawingRectangle = System.Drawing.Rectangle;
using PdfRectangle = iTextSharp.text.Rectangle;
using System.Drawing.Printing;

namespace HTQLCoffee.PhongBan
{
    public partial class frmThanhToan : Form
    {
        string connection = ConfigurationManager.ConnectionStrings["HTQLCoffee.Properties.Settings.QLCoffeeConnectionString"].ConnectionString;

        string maBan, tenBan, tenKhachHang, tenChiNhanh, ngayTao;
        decimal tongThanhToan, giamGia;
        private Bitmap billImage;

        public frmThanhToan(string maBan, string tenBan, string tenKhachHang, string ngayTao)
        {
            InitializeComponent();
            this.tenBan = tenBan;
            this.tenKhachHang = tenKhachHang;
            this.maBan = maBan;
            this.ngayTao = ngayTao;
           
        }
        


        private void frmThanhToan_Load(object sender, EventArgs e)
        {
            lblHoaDonBan.Text = lblHoaDonBan.Text + " " + tenBan;
            lblTenKhachHang.Text = tenKhachHang;
            
            lblNgayTaoDon.Text = ngayTao;
           
            lblTenChiNhanh.Text = lblTenChiNhanh.Text + " " + getTenChiNhanh();
            

            if (dgvDichVu_SanPham.Columns.Count == 0)
            {
                dgvDichVu_SanPham.Columns.Add("Ten", "Tên");
                dgvDichVu_SanPham.Columns.Add("SoLuong", "SL");
                dgvDichVu_SanPham.Columns.Add("Gia", "Giá");
                dgvDichVu_SanPham.Columns.Add("Tong", "Tổng tiền");
            }

            dgvDichVu_SanPham.Columns["Ten"].Width = 150;
            dgvDichVu_SanPham.Columns["SoLuong"].Width = 50;
            dgvDichVu_SanPham.Columns["Gia"].Width = 100;
            dgvDichVu_SanPham.Columns["Tong"].Width = 100;

            LoadDichVuVaSanPham();
            string maKhachHang = GetMaKhachHangByTen(tenKhachHang);
            if (!string.IsNullOrEmpty(maKhachHang))
            {
                // Cập nhật thông tin giảm giá
                UpdateDiscount(GetMaKhachHangByTen(tenKhachHang));
            }
        }

        

        private void ExportBillToPDF()
        {
            try
            {
                using (FolderBrowserDialog folderBrowser = new FolderBrowserDialog())
                {
                    folderBrowser.Description = "Chọn thư mục để lưu hóa đơn PDF";
                    folderBrowser.ShowNewFolderButton = true;

                    if (folderBrowser.ShowDialog() == DialogResult.OK)
                    {
                        string folderPath = folderBrowser.SelectedPath;

                        // Tạo tên file PDF
                        string fileName = "HoaDon_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".pdf";
                        string filePath = Path.Combine(folderPath, fileName);

                        // Tạo tài liệu PDF
                        Document document = new Document(PageSize.A4, 25, 25, 30, 30);
                        PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(filePath, FileMode.Create));
                        document.Open();

                        // Chụp nội dung GroupBox và lưu vào file ảnh Bitmap
                        billImage = CaptureGroupBox(grbHoaDon, 2.0f); // Tăng tỷ lệ để ảnh rõ hơn
                        if (billImage != null)
                        {
                            AddImageToPDF(document, billImage); // Thêm ảnh vào PDF
                        }
                        else
                        {
                            throw new Exception("Không thể tạo ảnh hóa đơn.");
                        }

                        // Đóng tài liệu
                        document.Close();

                        MessageBox.Show("Hóa đơn đã được xuất ra file PDF thành công tại: " + filePath, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi xảy ra khi xuất hóa đơn: " + ex.Message + "\n" + ex.StackTrace, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void AddImageToPDF(Document document, Bitmap bitmap)
        {
            using (var ms = new MemoryStream())
            {
                bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                iTextSharp.text.Image pdfImage = iTextSharp.text.Image.GetInstance(ms.ToArray());

                pdfImage.ScaleToFit(document.PageSize.Width - document.LeftMargin - document.RightMargin, document.PageSize.Height - document.TopMargin - document.BottomMargin);
                pdfImage.Alignment = Element.ALIGN_CENTER;

                document.Add(pdfImage);
            }
        }

        private Bitmap CaptureGroupBox(GroupBox groupBox, float scale = 2.0f)
        {
            int width = (int)(groupBox.Width * scale);
            int height = (int)(groupBox.Height * scale);

            Bitmap bmp = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                g.ScaleTransform(scale, scale);

                System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, groupBox.Width, groupBox.Height);
                groupBox.DrawToBitmap(bmp, rect);
            }
            return bmp;
        }




        private void UpdateStockQuantity()
        {
            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();

                foreach (DataGridViewRow row in dgvDichVu_SanPham.Rows)
                {
                    if (row.Cells["Ten"].Value != null) // Kiểm tra xem ô tên không trống
                    {
                        string tenSanPham = row.Cells["Ten"].Value.ToString();
                        int soLuong = Convert.ToInt32(row.Cells["SoLuong"].Value);

                        // Lấy mã sản phẩm từ tên sản phẩm (bạn có thể cần điều chỉnh truy vấn nếu cần)
                        string maSanPham = GetMaSanPhamByTen(tenSanPham, conn);

                        if (!string.IsNullOrEmpty(maSanPham))
                        {
                            // Cập nhật số lượng tồn kho
                            string updateQuery = "UPDATE QuanLyKho SET SoLuongTon = SoLuongTon - @SoLuong WHERE MaSanPham = @MaSanPham";
                            using (SqlCommand cmd = new SqlCommand(updateQuery, conn))
                            {
                                cmd.Parameters.AddWithValue("@SoLuong", soLuong);
                                cmd.Parameters.AddWithValue("@MaSanPham", maSanPham);
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }
        }


        private string GetMaSanPhamByTen(string tenSanPham, SqlConnection conn)
        {
            string maSanPham = null;
            string query = "SELECT MaSanPham FROM SanPham WHERE TenSanPham = @TenSanPham";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@TenSanPham", tenSanPham);
                object result = cmd.ExecuteScalar();
                if (result != null)
                {
                    maSanPham = result.ToString();
                }
            }
            return maSanPham;
        }

        private void CheckAndUpdateVIPStatus(string maKhachHang)
        {
            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();
                string queryCount = "SELECT COUNT(*) FROM HoaDon WHERE MaKhachHang = @MaKhachHang";
                using (SqlCommand cmdCount = new SqlCommand(queryCount, conn))
                {
                    cmdCount.Parameters.AddWithValue("@MaKhachHang", maKhachHang);
                    int count = (int)cmdCount.ExecuteScalar();
                    if (count >= 10)
                    {
                        string updateVIP = "UPDATE KhachHang SET LoaiKhachHang = 'VIP' WHERE MaKhachHang = @MaKhachHang";
                        using (SqlCommand cmdUpdate = new SqlCommand(updateVIP, conn))
                        {
                            cmdUpdate.Parameters.AddWithValue("@MaKhachHang", maKhachHang);
                            cmdUpdate.ExecuteNonQuery();
                        }
                    }
                }
            }
        }

        private void UpdateGoiMonStatus(string maBan)
        {
            using (SqlConnection conn = new SqlConnection(connection))
            {
                string query = "UPDATE GoiMon SET TrangThai = 1 WHERE MaBan = @MaBan AND TrangThai = 0"; // Chỉ cập nhật trạng thái thành true cho các đơn hàng có trạng thái = 0
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@MaBan", maBan);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void UpdateBookingStatus(string maKhachHang, string maBan)
        {
            using (SqlConnection conn = new SqlConnection(connection))
            {
                string query = "UPDATE DatBan SET TinhTrang = N'Đã thanh toán' WHERE MaKhachHang = @MaKhachHang AND MaBan = @MaBan AND TinhTrang = N'Chưa thanh toán'";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@MaKhachHang", maKhachHang);
                    cmd.Parameters.AddWithValue("@MaBan", maBan);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void UpdateTableStatus(string maBan)
        {
            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();
                string query = "UPDATE BanAn SET TrangThai = N'Chưa đặt bàn' WHERE MaBan = @MaBan";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@MaBan", maBan);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void SaveHoaDonToDatabase(string maHoaDon, string maKhachHang, string maBan, DateTime ngayLapHoaDon, decimal tongTien, decimal thanhToan, decimal giamGia, string ghiChu)
        {
            using (SqlConnection conn = new SqlConnection(connection))
            {
                string query = "INSERT INTO HoaDon (MaHoaDon, MaKhachHang, MaBan, NgayLapHoaDon, TongTien, ThanhToan, GiamGia, GhiChu, NgayTao, NgayCapNhat) " +
                               "VALUES (@MaHoaDon, @MaKhachHang, @MaBan, @NgayLapHoaDon, @TongTien, @ThanhToan, @GiamGia, @GhiChu, GETDATE(), GETDATE())";
                SqlCommand cmd = new SqlCommand(query, conn);

                // Truyền vào giá trị NULL nếu không có mã khách hàng
                cmd.Parameters.AddWithValue("@MaHoaDon", maHoaDon);
                cmd.Parameters.AddWithValue("@MaKhachHang", string.IsNullOrEmpty(maKhachHang) ? (object)DBNull.Value : maKhachHang);
                cmd.Parameters.AddWithValue("@MaBan", maBan);
                cmd.Parameters.AddWithValue("@NgayLapHoaDon", ngayLapHoaDon);
                cmd.Parameters.AddWithValue("@TongTien", tongTien);
                cmd.Parameters.AddWithValue("@ThanhToan", thanhToan);  // Không cần chuyển sang string "N0" vì SQL Server sẽ tự động chuyển kiểu dữ liệu.
                cmd.Parameters.AddWithValue("@GiamGia", giamGia);
                cmd.Parameters.AddWithValue("@GhiChu", ghiChu);

                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }


        private string GenerateRandomCode(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Random random = new Random();
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private void btnInHoaDon_Click(object sender, EventArgs e)
        {
            // Hỏi khách hàng xác nhận xuất hóa đơn
            DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn xuất hóa đơn?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                // Tạo mã hóa đơn ngẫu nhiên
                string maHoaDon = GenerateRandomCode(10);

                // Lấy mã khách hàng từ tên khách hàng
                string tenKhachHang = lblTenKhachHang.Text;
                string maKhachHang = GetMaKhachHangByTen(tenKhachHang);

                // Lấy thông tin hóa đơn
                DateTime ngayLapHoaDon = DateTime.Now;
                string ghiChu = "Đã thanh toán";
                decimal tongTien = Convert.ToDecimal(lblTongTien.Text.Replace(" VND", ""));

                // Nếu không tìm thấy mã khách hàng, gán NULL cho cột MaKhachHang
                if (string.IsNullOrEmpty(maKhachHang))
                {
                    maKhachHang = null; // SQL Server sẽ nhận NULL
                    MessageBox.Show("Không tìm thấy mã khách hàng, hóa đơn sẽ được lưu mà không có thông tin khách hàng.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                // Lưu vào cơ sở dữ liệu
                SaveHoaDonToDatabase(maHoaDon, maKhachHang, maBan, ngayLapHoaDon, tongTien, tongThanhToan, giamGia, ghiChu);


                // Bắt đầu cập nhật thông tin
                UpdateTableStatus(maBan);

                // Chỉ cập nhật trạng thái đặt bàn và kiểm tra VIP nếu tìm thấy mã khách hàng
                if (!string.IsNullOrEmpty(maKhachHang))
                {
                    UpdateBookingStatus(maKhachHang, maBan);
                    CheckAndUpdateVIPStatus(maKhachHang);
                }

                UpdateGoiMonStatus(maBan);
                UpdateStockQuantity();
                MessageBox.Show("Xuất hóa đơn thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                 ExportBillToPDF();

            }
            try
            {
                if (billImage == null)
                {
                    MessageBox.Show("Vui lòng xuất ra file trước khi in", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Khởi tạo đối tượng PrintDocument
                PrintDocument printDocument = new PrintDocument();

                // Sự kiện để vẽ ảnh hóa đơn lên trang in
                printDocument.PrintPage += new PrintPageEventHandler(PrintBillImage);

                // Hiển thị hộp thoại để chọn máy in
                PrintDialog printDialog = new PrintDialog();
                printDialog.Document = printDocument;

                // Kiểm tra nếu người dùng chọn máy in và nhấn in
                if (printDialog.ShowDialog() == DialogResult.OK)
                {
                    // Cập nhật máy in được chọn
                    printDocument.PrinterSettings = printDialog.PrinterSettings;

                    // In hóa đơn ra giấy
                    printDocument.Print();

                    MessageBox.Show("Hóa đơn đã được in thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi xảy ra khi in hóa đơn: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PrintBillImage(object sender, PrintPageEventArgs e)
        {
            Graphics graphics = e.Graphics;

            if (billImage != null)
            {
                // Tính toán kích thước ảnh sao cho phù hợp với trang in
                float x = 100;
                float y = 100;
                float width = e.PageBounds.Width - 2 * x;
                float height = e.PageBounds.Height - 2 * y;

                // Vẽ ảnh lên trang in
                graphics.DrawImage(billImage, x, y, width, height);

                // Đặt giá trị HasMorePages nếu muốn in nhiều trang (nếu có)
                e.HasMorePages = false;  // Chỉ có một trang trong ví dụ này
            }
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private string GetMaKhachHangByTen(string tenKhachHang)
        {
            string maKhachHang = null;

            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();
                string query = "SELECT MaKhachHang FROM KhachHang WHERE HoTen = @HoTen";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@HoTen", tenKhachHang);
                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        maKhachHang = result.ToString();
                    }
                }
            }
            return maKhachHang;
        }


        private void UpdateDiscount(string maKhachHang)
        {
            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();
                string query = @"
            SELECT kh.LoaiKhachHang, km.PhanTramGiam
            FROM KhachHang kh
            LEFT JOIN KhuyenMai km ON km.TenKhuyenMai = N'Khuyến mãi khách hàng VIP'
            WHERE kh.MaKhachHang = @MaKhachHang";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@MaKhachHang", maKhachHang);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string loaiKhachHang = reader["LoaiKhachHang"].ToString();

                            if (loaiKhachHang == "VIP")
                            {
                                // Nếu là khách hàng VIP, tính giảm giá
                                decimal phanTramGiam = reader["PhanTramGiam"] != DBNull.Value
                                    ? Convert.ToDecimal(reader["PhanTramGiam"])
                                    : 0;

                                giamGia = Convert.ToDecimal(lblTongTien.Text.Replace(" VND", "")) * (phanTramGiam / 100);
                            }
                            else
                            {
                                // Nếu là khách hàng Thường, không giảm giá
                                giamGia = 0;
                            }
                        }
                        else
                        {
                            // Trường hợp không tìm thấy khách hàng
                            giamGia = 0;
                        }
                    }
                }
            }

            // Cập nhật lại các thông tin hiển thị
            lblGiamGia.Text = giamGia.ToString("N0") + " VND";
            tongThanhToan = Convert.ToDecimal(lblTongTien.Text.Replace(" VND", "")) - giamGia;
            lblThanhToan.Text = tongThanhToan.ToString("N0") + " VND";
        }

        private void LoadDichVuVaSanPham()
        {
            
            decimal tongSanPham = 0;

            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();
               
                // Lấy danh sách sản phẩm
                string querySanPham = @"
                SELECT sp.TenSanPham AS Ten, ctdh.SoLuong, sp.GiaBan AS Gia, ctdh.ThanhTien AS Tong
                FROM ChiTietGoiMon ctdh
                JOIN SanPham sp ON ctdh.MaSanPham = sp.MaSanPham
                JOIN GoiMon dh ON ctdh.MaGoiMon = dh.MaGoiMon
                WHERE dh.MaBan = @maBan and dh.TrangThai = 0";

                using (SqlCommand cmdSanPham = new SqlCommand(querySanPham, conn))
                {
                    cmdSanPham.Parameters.AddWithValue("@maBan", maBan);
                    using (SqlDataReader reader = cmdSanPham.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string ten = reader["Ten"].ToString();
                            int soLuong = Convert.ToInt32(reader["SoLuong"]);
                            decimal gia = Convert.ToDecimal(reader["Gia"]);
                            decimal tong = Convert.ToDecimal(reader["Tong"]);

                            tongSanPham += tong;

                            dgvDichVu_SanPham.Rows.Add(ten, soLuong, gia.ToString("N0"), tong.ToString("N0")); // Hiển thị không có chữ số thập phân
                        }
                    }
                }
            }

            // Gán tổng tiền dịch vụ và sản phẩm lên các Label
           
            lblTongSanPham.Text = tongSanPham.ToString("N0") + " VND";


          
            giamGia = IsKhachHangVIP(tenKhachHang) ? tongSanPham * 0.05m : 0;
            lblGiamGia.Text = giamGia.ToString("N0") + " VND";

            lblTongTien.Text = tongSanPham.ToString("N0") + " VND";
            // Tính tổng số tiền phải thanh toán
            tongThanhToan = tongSanPham - giamGia;
            lblThanhToan.Text = tongThanhToan.ToString("N0") + " VND";
        }


        private bool IsKhachHangVIP(string tenKhachHang)
        {
            // Thực hiện truy vấn để kiểm tra loại khách hàng
            using (SqlConnection conn = new SqlConnection(connection))
            {
                string query = "SELECT LoaiKhachHang FROM KhachHang WHERE HoTen = @TenKhachHang";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@TenKhachHang", tenKhachHang);
                conn.Open();

                // Thực hiện truy vấn và kiểm tra null
                var result = cmd.ExecuteScalar();
                conn.Close();

                // Nếu result là null, trả về false vì khách hàng không tồn tại
                if (result == null)
                {
                    return false;
                }

                string loaiKhachHang = result.ToString();
                return loaiKhachHang == "VIP";
            }
        }
        private string getTenChiNhanh()
        {
            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();

                string query = @"select top 1 TenChiNhanh from ChiNhanh";

                using (SqlCommand command = new SqlCommand(query, conn))
                {

                    object result = command.ExecuteScalar();
                    if (result != null)
                    {
                        tenChiNhanh = result.ToString(); ;
                    }
                }
            }

            return tenChiNhanh;
        }
    }
}
