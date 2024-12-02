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

namespace HTQLCoffee.NhanVien
{
    public partial class frmQuanLyLuong : Form
    {
        string connection = ConfigurationManager.ConnectionStrings["HTQLCoffee.Properties.Settings.QLCoffeeConnectionString"].ConnectionString;

        private string maNhanVien;
        public frmQuanLyLuong(string maNhanVien)
        {
            InitializeComponent();
            this.maNhanVien = maNhanVien;

            // Khởi tạo ComboBox chọn tháng
            InitializeComboBoxThang();

            // Tải dữ liệu lương mặc định cho tháng hiện tại
            LoadLuongNhanVien(DateTime.Now.Month, DateTime.Now.Year);
        }

        private void InitializeComboBoxThang()
        {
            comboBoxThang.Items.Clear();

            // Thêm các tháng từ 1 đến 12 vào ComboBox
            for (int i = 1; i <= 12; i++)
            {
                comboBoxThang.Items.Add(i);
            }

            // Đặt giá trị mặc định là tháng hiện tại
            comboBoxThang.SelectedItem = DateTime.Now.Month;

            // Xử lý sự kiện khi chọn tháng
            comboBoxThang.SelectedIndexChanged += ComboBoxThang_SelectedIndexChanged;
        }



        private void LoadLuongNhanVien(int thang, int nam)
        {
            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();

                string query = @"SELECT NV.MaNhanVien, NV.HoTen, DD.NgayDiemDanh, DD.ThoiGianDiLam, DD.ThoiGianVeLam, NV.LuongCoBan
                 FROM NhanVien NV
                 JOIN DiemDanh DD ON NV.MaNhanVien = DD.MaNhanVien
                 WHERE NV.MaNhanVien = @MaNhanVien 
                 AND MONTH(DD.NgayDiemDanh) = @Thang 
                 AND YEAR(DD.NgayDiemDanh) = @Nam
                 AND DD.ThoiGianVeLam IS NOT NULL
                 ORDER BY DD.NgayDiemDanh";


                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@MaNhanVien", maNhanVien);
                cmd.Parameters.AddWithValue("@Thang", thang);
                cmd.Parameters.AddWithValue("@Nam", nam);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show($"Không có dữ liệu lương cho tháng {thang}/{nam}.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    dgvLuongNhanVien.DataSource = null;
                    ClearSalaryDetails();
                    return;
                }

                dt.Columns.Add("GioLamTrongNgay", typeof(decimal));

                foreach (DataRow row in dt.Rows)
                {
                    if (row["ThoiGianVeLam"] == DBNull.Value)
                    {
                        MessageBox.Show("Có ca làm chưa kết thúc. Vui lòng kiểm tra lại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    DateTime thoiGianVao = Convert.ToDateTime(row["ThoiGianDiLam"]);
                    DateTime thoiGianRa = Convert.ToDateTime(row["ThoiGianVeLam"]);
                    TimeSpan gioLam = thoiGianRa - thoiGianVao;

                    row["GioLamTrongNgay"] = Math.Round((decimal)gioLam.TotalHours, 2);
                }

                dgvLuongNhanVien.DataSource = dt;

                dgvLuongNhanVien.Columns["MaNhanVien"].HeaderText = "Mã Nhân Viên";
                dgvLuongNhanVien.Columns["HoTen"].HeaderText = "Họ Tên";
                dgvLuongNhanVien.Columns["NgayDiemDanh"].HeaderText = "Ngày Điểm Danh";
                dgvLuongNhanVien.Columns["ThoiGianDiLam"].HeaderText = "Thời Gian Đi Làm";
                dgvLuongNhanVien.Columns["ThoiGianVeLam"].HeaderText = "Thời Gian Về Làm";
                dgvLuongNhanVien.Columns["LuongCoBan"].HeaderText = "Lương Cơ Bản";
                dgvLuongNhanVien.Columns["GioLamTrongNgay"].HeaderText = "Giờ Làm Trong Ngày";

                dgvLuongNhanVien.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

                TinhTongGioLam(dt);
            }
        }

        private void ComboBoxThang_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxThang.SelectedItem != null)
            {
                int selectedThang = (int)comboBoxThang.SelectedItem;
                int selectedNam = DateTime.Now.Year; // Có thể thêm ComboBox chọn năm nếu cần
                LoadLuongNhanVien(selectedThang, selectedNam);
            }
        }

        private void ClearSalaryDetails()
        {
            txtTongGioLam.Text = "";
            txtLuongThuong.Text = "";
            txtTienPhat.Text = "";
            txtLuongLanh.Text = "";
        }




        private void TinhTongGioLam(DataTable dt)
        {
            // Tính tổng giờ làm trong tháng
            decimal tongGioLam = 0;
            foreach (DataRow row in dt.Rows)
            {
                tongGioLam += Convert.ToDecimal(row["GioLamTrongNgay"]);
            }

            txtTongGioLam.Text = tongGioLam.ToString("N2") + " giờ";

            // Tính lương thưởng và lương lãnh
            TinhLuong(tongGioLam, Convert.ToDecimal(dt.Rows[0]["LuongCoBan"]));
        }

        private void TinhLuong(decimal tongGioLam, decimal luongCoBan)
        {
            decimal luongThuong = 0;
            decimal tienPhat = 0;
            decimal luongLanh = 0;

            // Tính lương thưởng
            if (tongGioLam > 100)
            {
                luongThuong = 500000;
            }
            else if (tongGioLam > 80)
            {
                luongThuong = 300000;
            }
            else if (tongGioLam > 70)
            {
                luongThuong = 100000;
            }
            else if (tongGioLam < 60)
            {
                // Tính phạt nếu dưới 60 tiếng
                tienPhat = (60 - tongGioLam) * 5000;

                // Giới hạn tiền phạt tối đa là 300,000
                if (tienPhat > 300000)
                {
                    tienPhat = 300000;
                }
            }

            // Tính lương lãnh
            luongLanh = (tongGioLam * luongCoBan) + luongThuong - tienPhat;

            txtLuongThuong.Text = luongThuong.ToString("N0") + "₫";
            txtLuongThuong.ReadOnly = true;
            txtLuongThuong.BackColor = SystemColors.Window;
            txtLuongThuong.ForeColor = Color.Green;

            txtTienPhat.Text = tienPhat > 0 ? "-" + tienPhat.ToString("N0") + "₫" : "0₫";
            txtTienPhat.ReadOnly = true;
            txtTienPhat.BackColor = SystemColors.Window;
            txtTienPhat.ForeColor = tienPhat > 0 ? Color.Red : Color.Black;

            txtLuongLanh.Text = luongLanh.ToString("N0") + "₫";
            txtLuongLanh.ReadOnly = true;
            txtLuongLanh.BackColor = SystemColors.Window;
            txtLuongLanh.ForeColor = Color.Blue;
        }

        private void btnNhanLuong_Click(object sender, EventArgs e)
        {
            if (comboBoxThang.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn tháng để nhận lương.", "Thông báo");
                return;
            }

            int selectedThang = (int)comboBoxThang.SelectedItem;
            int selectedNam = DateTime.Now.Year; // Có thể thêm ComboBox chọn năm nếu cần

            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();

                // Kiểm tra xem lương tháng này đã được nhận chưa
                string queryCheck = @"SELECT COUNT(*) FROM LichSuNhanLuong 
                              WHERE MaNhanVien = @MaNhanVien AND Thang = @Thang AND Nam = @Nam AND TrangThaiNhanLuong = 1";
                SqlCommand cmdCheck = new SqlCommand(queryCheck, conn);
                cmdCheck.Parameters.AddWithValue("@MaNhanVien", maNhanVien);
                cmdCheck.Parameters.AddWithValue("@Thang", selectedThang);
                cmdCheck.Parameters.AddWithValue("@Nam", selectedNam);

                int isLuongNhan = (int)cmdCheck.ExecuteScalar();

                if (isLuongNhan > 0)
                {
                    MessageBox.Show($"Lương tháng {selectedThang}/{selectedNam} đã được nhận. Bạn không thể nhận thêm lương.", "Thông báo");
                    return;
                }

                // Nếu chưa nhận, thì cập nhật trạng thái đã nhận lương vào bảng LichSuNhanLuong
                string queryInsert = @"INSERT INTO LichSuNhanLuong (MaNhanVien, Thang, Nam, TongGioLam, TongLuong, TrangThaiNhanLuong)
                               VALUES (@MaNhanVien, @Thang, @Nam, @TongGioLam, @TongLuong, @TrangThaiNhanLuong)";
                SqlCommand cmdInsert = new SqlCommand(queryInsert, conn);
                cmdInsert.Parameters.AddWithValue("@MaNhanVien", maNhanVien);
                cmdInsert.Parameters.AddWithValue("@Thang", selectedThang);
                cmdInsert.Parameters.AddWithValue("@Nam", selectedNam);

                // Chuyển đổi giá trị từ TextBox thành kiểu số (Decimal)
                decimal tongGioLam;
                decimal tongLuong;

                // Kiểm tra và chuyển đổi giá trị
                if (!decimal.TryParse(txtTongGioLam.Text.Replace(" giờ", "").Trim(), out tongGioLam))
                {
                    MessageBox.Show("Giá trị tổng giờ làm không hợp lệ.", "Lỗi");
                    return;
                }

                if (!decimal.TryParse(txtLuongLanh.Text.Replace("₫", "").Replace(",", "").Trim(), out tongLuong))
                {
                    MessageBox.Show("Giá trị tổng lương không hợp lệ.", "Lỗi");
                    return;
                }

                cmdInsert.Parameters.AddWithValue("@TongGioLam", tongGioLam);
                cmdInsert.Parameters.AddWithValue("@TongLuong", tongLuong);
                cmdInsert.Parameters.AddWithValue("@TrangThaiNhanLuong", 1);

                cmdInsert.ExecuteNonQuery();

                MessageBox.Show($"Lương tháng {selectedThang}/{selectedNam} đã được nhận thành công!", "Thông báo");
            }
        }


        private void btnLichSu_Click(object sender, EventArgs e)
        {
            frmLichSuNhanLuong frmLichSu = new frmLichSuNhanLuong(maNhanVien);
            frmLichSu.ShowDialog();
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmQuanLyLuong_Load(object sender, EventArgs e)
        {

        }
    }
}
