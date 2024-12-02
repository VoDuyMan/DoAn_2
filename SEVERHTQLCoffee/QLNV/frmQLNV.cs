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

namespace SERVERQLCoffee.QLNV
{
    public partial class frmQLNV : Form
    {
        string connection = ConfigurationManager.ConnectionStrings["SERVERQLCoffee.Properties.Settings.QLCoffeeConnectionString"].ConnectionString;
        public frmQLNV()
        {
            InitializeComponent();
            LoadBranches();
            LoadEmployeeData();
        }
        private void LoadBranches()
        {
            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();
                string query = "SELECT TenChiNhanh FROM ChiNhanh";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        cbxChiNhanh.Items.Clear();
                        cbxChiNhanh.Items.Add("Tất cả"); // Thêm mục 'Tất cả'

                        while (reader.Read())
                        {
                            string tenChiNhanh = reader["TenChiNhanh"].ToString();
                            cbxChiNhanh.Items.Add(tenChiNhanh);
                        }

                        // Đặt mục đầu tiên là 'Tất cả' khi hiển thị
                        cbxChiNhanh.SelectedIndex = 0;
                    }
                }
            }
        }
        private void LoadEmployeeData(string branchName = "Tất cả")
        {
            // Kết nối tới cơ sở dữ liệu và lấy danh sách nhân viên
            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();
                string query = @"SELECT NhanVien.MaNhanVien, NhanVien.HoTen, NhanVien.SoDienThoai, NhanVien.Email, 
                         NhanVien.ChucVu, NhanVien.LuongCoBan, NhanVien.NgayVaoLam, NhanVien.GhiChu, 
                         NhanVien.MaChiNhanh, ChiNhanh.TenChiNhanh, NhanVien.NgayTao, NhanVien.NgayCapNhat
                         FROM NhanVien
                         JOIN ChiNhanh ON NhanVien.MaChiNhanh = ChiNhanh.MaChiNhanh";

                // Nếu lọc theo chi nhánh, thêm điều kiện WHERE
                if (branchName != "Tất cả")
                {
                    query += " WHERE ChiNhanh.TenChiNhanh = @TenChiNhanh";
                }

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    if (branchName != "Tất cả")
                    {
                        cmd.Parameters.AddWithValue("@TenChiNhanh", branchName);
                    }

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        flowLayoutPanel.Controls.Clear(); // Xóa các button cũ trước khi thêm mới

                        while (reader.Read())
                        {
                            // Lấy thông tin nhân viên
                            string maNhanVien = reader["MaNhanVien"].ToString();
                            string hoTen = reader["HoTen"].ToString();
                            string soDienThoai = reader["SoDienThoai"].ToString();
                            string email = reader["Email"].ToString();
                            string chucVu = reader["ChucVu"].ToString();
                            decimal luongCoBan = Convert.ToDecimal(reader["LuongCoBan"]);
                            DateTime ngayVaoLam = Convert.ToDateTime(reader["NgayVaoLam"]);
                            string ghiChu = reader["GhiChu"].ToString();
                            string maChiNhanh = reader["MaChiNhanh"].ToString();
                            string tenChiNhanh = reader["TenChiNhanh"].ToString();
                            DateTime ngayTao = Convert.ToDateTime(reader["NgayTao"]);
                            DateTime ngayCapNhat = Convert.ToDateTime(reader["NgayCapNhat"]);

                            // Tạo button cho mỗi nhân viên
                            Button btnEmployee = new Button
                            {
                                Text = string.Format("{0}\n{1}", hoTen, chucVu),
                                Size = new Size(175, 175),
                                Font = new Font("Microsoft Sans Serif", 12, FontStyle.Bold),
                                TextAlign = ContentAlignment.BottomCenter,
                                ImageAlign = ContentAlignment.TopCenter,
                                Tag = maNhanVien
                            };

                            // Sự kiện click để hiển thị chi tiết nhân viên
                            btnEmployee.Click += (s, e) =>
                            {
                                ShowEmployeeDetails(maNhanVien, hoTen, chucVu, soDienThoai, email, luongCoBan, ngayVaoLam, ghiChu, ngayTao, ngayCapNhat, tenChiNhanh);
                            };

                            // Chọn hình ảnh dựa vào chức vụ
                            try
                            {
                                string imagePath;
                                if (chucVu == "Phục Vụ")
                                    imagePath = @"\HTQLCoffee\Image\phucvu.gif";
                                else if (chucVu == "Pha Chế")
                                    imagePath = @"\HTQLCoffee\Image\phache.gif";
                                else if (chucVu == "Bảo Vệ")
                                    imagePath = @"\HTQLCoffee\Image\baove.gif";
                                else
                                    imagePath = @"\HTQLCoffee\Image\phucvu.gif";

                                btnEmployee.Image = Image.FromFile(imagePath);
                            }
                            catch (FileNotFoundException ex)
                            {
                                MessageBox.Show("Không tìm thấy file hình ảnh: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Đã xảy ra lỗi khi tải hình ảnh: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }

                            flowLayoutPanel.Controls.Add(btnEmployee);
                        }
                    }
                }
            }
        }

        private void ShowEmployeeDetails(string maNhanVien, string hoTen, string chucVu, string soDienThoai, string email, decimal luongCoBan, DateTime ngayVaoLam, string ghiChu, DateTime ngayTao, DateTime ngayCapNhat, string tenChiNhanh)
        {
            groupBoxEmployeeDetails.Text = string.Format("Thông Tin Nhân Viên: {0}", hoTen);
            txtMaNhanVien.Text = maNhanVien;
            txtHoTen.Text = hoTen;
            txtChucVu.Text = chucVu;
            txtSoDienThoai.Text = soDienThoai;
            txtEmail.Text = email;
            txtLuongCoBan.Text = luongCoBan.ToString("N0") + "₫"; // Định dạng tiền tệ
            txtNgayVaoLam.Text = ngayVaoLam.ToString("dd/MM/yyyy");
            txtGhiChu.Text = ghiChu;
            txtNgayTao.Text = ngayTao.ToString("dd/MM/yyyy HH:mm:ss");
            txtNgayCapNhat.Text = ngayCapNhat.ToString("dd/MM/yyyy HH:mm:ss");
            txtTenChiNhanh.Text = tenChiNhanh;

            CalculateTotalWorkHours(maNhanVien);
            groupBoxEmployeeDetails.Visible = true;
        }

        private void cbxChiNhanh_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedBranch = cbxChiNhanh.SelectedItem?.ToString() ?? "Tất cả";
            LoadEmployeeData(selectedBranch);
        }
        private void CalculateTotalWorkHours(string maNhanVien)
        {
            // Lấy thời gian hiện tại để xác định tháng và năm
            int currentMonth = DateTime.Now.Month;
            int currentYear = DateTime.Now.Year;

            // Kết nối tới cơ sở dữ liệu để tính tổng giờ làm
            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();
                // Truy vấn SQL để tính tổng thời gian làm việc trong tháng
                string query = @"SELECT SUM(DATEDIFF(SECOND, ThoiGianDiLam, ThoiGianVeLam)) AS TongGiayLam
                         FROM DiemDanh
                         WHERE MaNhanVien = @MaNhanVien
                         AND MONTH(ThoiGianDiLam) = @ThangHienTai
                         AND YEAR(ThoiGianDiLam) = @NamHienTai";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@MaNhanVien", maNhanVien);
                    cmd.Parameters.AddWithValue("@ThangHienTai", currentMonth);
                    cmd.Parameters.AddWithValue("@NamHienTai", currentYear);

                    // Lấy kết quả tổng giây làm việc
                    object result = cmd.ExecuteScalar();
                    if (result != DBNull.Value)
                    {
                        long totalSeconds = Convert.ToInt64(result);

                        // Chuyển đổi tổng giây thành giờ, phút
                        int totalHours = (int)(totalSeconds / 3600);
                        int totalMinutes = (int)((totalSeconds % 3600) / 60);

                        // Hiển thị tổng giờ làm ra textbox
                        txtTongGioLam.Text = string.Format("{0} giờ {1} phút", totalHours, totalMinutes);
                    }
                    else
                    {
                        // Nếu không có dữ liệu, hiển thị 0 giờ
                        txtTongGioLam.Text = "0 giờ";
                    }
                }
            }
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void QLLuong_Click(object sender, EventArgs e)
        {
            frmQuanLyLuong frmQuanLyLuong = new frmQuanLyLuong(txtMaNhanVien.Text);
            if (string.IsNullOrEmpty(txtMaNhanVien.Text))
            {
                MessageBox.Show("Vui lòng chọn một nhân viên xem lương.");
                return;
            }
            else
            {
                frmQuanLyLuong.FormClosed += (s, args) =>
                {
                    LoadEmployeeData();
                };
                frmQuanLyLuong.ShowDialog();
            }
        }

        private void BaoCaoThongKe_Click(object sender, EventArgs e)
        {
            frmThongKeLuong frmThongke = new frmThongKeLuong();
            frmThongke.ShowDialog();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadEmployeeData();
        }
    }
}
