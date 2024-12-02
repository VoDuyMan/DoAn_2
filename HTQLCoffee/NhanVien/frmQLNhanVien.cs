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

namespace HTQLCoffee.NhanVien
{
    public partial class frmQLNhanVien : Form
    {
        string connection = ConfigurationManager.ConnectionStrings["HTQLCoffee.Properties.Settings.QLCoffeeConnectionString"].ConnectionString;

        public frmQLNhanVien()
        {
            InitializeComponent();
            LoadEmployeeData();
        }


        private void LoadEmployeeData()
        {
            // Kết nối tới cơ sở dữ liệu và lấy danh sách nhân viên
            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();
                // Truy vấn lấy thông tin nhân viên và tên chi nhánh bằng phép nối (JOIN)
                string query = @"SELECT NhanVien.MaNhanVien, NhanVien.HoTen, NhanVien.SoDienThoai, NhanVien.Email, 
                                NhanVien.ChucVu, NhanVien.LuongCoBan, NhanVien.NgayVaoLam, NhanVien.GhiChu, 
                                NhanVien.MaChiNhanh, ChiNhanh.TenChiNhanh, NhanVien.NgayTao, NhanVien.NgayCapNhat
                         FROM NhanVien
                         JOIN ChiNhanh ON NhanVien.MaChiNhanh = ChiNhanh.MaChiNhanh";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
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
                            string tenChiNhanh = reader["TenChiNhanh"].ToString(); // Lấy tên chi nhánh
                            DateTime ngayTao = Convert.ToDateTime(reader["NgayTao"]);
                            DateTime ngayCapNhat = Convert.ToDateTime(reader["NgayCapNhat"]);

                            // Tạo button cho mỗi nhân viên
                            Button btnEmployee = new Button();
                            btnEmployee.Text = string.Format("{0}\n{1}", hoTen, chucVu); // Thêm tên chi nhánh vào nút
                            btnEmployee.Size = new Size(160, 175);
                            btnEmployee.Font = new Font("Microsoft Sans Serif", 12, FontStyle.Bold); // Đặt font chữ in đậm và kích thước 12
                            btnEmployee.TextAlign = ContentAlignment.BottomCenter; // Văn bản nằm ở giữa đáy button
                            btnEmployee.ImageAlign = ContentAlignment.TopCenter;
                            btnEmployee.Tag = maNhanVien; // Lưu mã nhân viên vào tag để dùng sau này

                            // Sự kiện click để hiển thị chi tiết nhân viên, thêm ghi chú và mã chi nhánh
                            btnEmployee.Click += (s, e) =>
                            {
                                ShowEmployeeDetails(maNhanVien, hoTen, chucVu, soDienThoai, email, luongCoBan, ngayVaoLam, ghiChu, ngayTao, ngayCapNhat);
                            };

                            // Chọn hình ảnh dựa vào chức vụ
                            try
                            {
                                if (chucVu == "Phục Vụ")
                                {
                                    btnEmployee.Image = Image.FromFile(@"\HTQLCoffee\Image\phucvu.gif");
                                    
                                }
                                else if (chucVu == "Pha Chế")
                                {
                                    btnEmployee.Image = Image.FromFile(@"\HTQLCoffee\Image\phache.gif");
                                }
                                else if (chucVu == "Bảo Vệ")
                                {
                                    btnEmployee.Image = Image.FromFile(@"\HTQLCoffee\Image\baove.gif");
                                    
                                }
                                else
                                {
                                    btnEmployee.Image = Image.FromFile(@"\HTQLCoffee\Image\phucvu.gif");
                                    
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

                            flowLayoutPanel.Controls.Add(btnEmployee);
                        }
                    }
                }
            }
        }
        private void ShowEmployeeDetails(string maNhanVien, string hoTen, string chucVu, string soDienThoai, string email, decimal luongCoBan, DateTime ngayVaoLam, string ghiChu, DateTime ngayTao, DateTime ngayCapNhat)
        {
            // Hiển thị thông tin nhân viên trong các TextBox
            groupBoxEmployeeDetails.Text = string.Format("Thông Tin Nhân Viên: {0}", hoTen);
            txtMaNhanVien.Text = maNhanVien;
            txtHoTen.Text = hoTen;
            txtChucVu.Text = chucVu;
            txtSoDienThoai.Text = soDienThoai;
            txtEmail.Text = email;
            txtLuongCoBan.Text = luongCoBan.ToString("N0") + "₫"; // Định dạng tiền tệ
            txtNgayVaoLam.Text = ngayVaoLam.ToString("dd/MM/yyyy"); // Hiển thị ngày vào làm theo định dạng dd/MM/yyyy
            txtGhiChu.Text = ghiChu;
            txtNgayTao.Text = ngayTao.ToString("dd/MM/yyyy HH:mm:ss"); // Ngày tạo
            txtNgayCapNhat.Text = ngayCapNhat.ToString("dd/MM/yyyy HH:mm:ss"); // Ngày cập nhật

            // Tính tổng giờ làm trong tháng
            CalculateTotalWorkHours(maNhanVien);

            // Cập nhật thông tin trong groupbox
            groupBoxEmployeeDetails.Visible = true; // Hiển thị groupbox chứa thông tin nhân viên
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

        private void frmQLNhanVien_Load(object sender, EventArgs e)
        {

        }

        private void btnThemNhanVien_Click(object sender, EventArgs e)
        {
            frmThemNhanVien frmThemNhanVien = new frmThemNhanVien();
            frmThemNhanVien.FormClosed += (s, args) =>
            {
                LoadEmployeeData();
            };

            frmThemNhanVien.ShowDialog();
        }

        private void btnXoaNhanVien_Click(object sender, EventArgs e)
        {
            // Kiểm tra xem có nhân viên nào được chọn hay chưa
            if (string.IsNullOrEmpty(txtMaNhanVien.Text))
            {
                MessageBox.Show("Vui lòng chọn một nhân viên để xóa.");
                return;
            }

            // Xác nhận việc xóa nhân viên
            DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn xóa nhân viên này không?",
                                                  "Xác nhận xóa",
                                                  MessageBoxButtons.OKCancel,
                                                  MessageBoxIcon.Warning);
            if (result == DialogResult.No)
            {
                return;
            }

            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();

                // Xóa dữ liệu liên quan đến nhân viên
                string deleteQuery = @"
            DELETE FROM BangLuong WHERE MaNhanVien = @MaNhanVien;
            DELETE FROM LichSuNhanLuong WHERE MaNhanVien = @MaNhanVien;
            DELETE FROM DiemDanh WHERE MaNhanVien = @MaNhanVien;
            DELETE FROM NhanVien WHERE MaNhanVien = @MaNhanVien;
        ";

                using (SqlCommand cmd = new SqlCommand(deleteQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@MaNhanVien", txtMaNhanVien.Text);
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        LoadEmployeeData();
                        ClearGroupBox(groupBoxEmployeeDetails);
                        MessageBox.Show("Nhân viên đã được xóa thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy nhân viên với mã này.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
        }
        private void ClearGroupBox(GroupBox groupBox)
        {
            foreach (Control control in groupBox.Controls)
            {
                if (control is TextBox)
                {
                    (control as TextBox).Clear();
                }
                else if (control is ComboBox)
                {
                    (control as ComboBox).SelectedIndex = -1;
                }
                // Nếu có các loại control khác cần clear, bạn có thể thêm điều kiện ở đây
            }
        }

        private void btnSuaNhanVien_Click(object sender, EventArgs e)
        {
            // Kiểm tra xem có nhân viên nào được chọn hay chưa
            if (string.IsNullOrEmpty(txtMaNhanVien.Text))
            {
                MessageBox.Show("Vui lòng chọn một nhân viên để sửa thông tin.");
                return;
            }

            frmSuaNhanVien frmSuaNhanVien = new frmSuaNhanVien(txtMaNhanVien.Text);
            frmSuaNhanVien.FormClosed += (s, args) =>
            {
                LoadEmployeeData();
            };

            frmSuaNhanVien.ShowDialog();
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void QLChamCong_Click(object sender, EventArgs e)
        {
            frmQLChamCong frmQLChamCong = new frmQLChamCong(txtMaNhanVien.Text, txtHoTen.Text);
            if (string.IsNullOrEmpty(txtMaNhanVien.Text))
            {
                MessageBox.Show("Vui lòng chọn một nhân viên để điểm danh.");
                return;
            }
            else
            {
                frmQLChamCong.FormClosed += (s, args) =>
                {
                    LoadEmployeeData();
                };

                frmQLChamCong.ShowDialog();
            }
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
    }
}
