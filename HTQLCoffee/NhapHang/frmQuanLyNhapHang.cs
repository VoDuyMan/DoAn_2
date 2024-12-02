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

namespace HTQLCoffee.NhapHang
{
    public partial class frmQuanLyNhapHang : Form
    {
        string connection = ConfigurationManager.ConnectionStrings["HTQLCoffee.Properties.Settings.QLCoffeeConnectionString"].ConnectionString;

        public frmQuanLyNhapHang()
        {
            InitializeComponent();
        }

        private string selectedMaNhapHang;

        private void frmQLNhapHang_Load(object sender, EventArgs e)
        {
            LoadData();
            LoadMonthsToComboBox();
        }

        private void LoadMonthsToComboBox()
        {
            cbxLocTheoThang.Items.Clear();
            for (int i = 1; i <= 12; i++)
            {
                cbxLocTheoThang.Items.Add("Tháng " + i);
            }
            cbxLocTheoThang.SelectedIndex = DateTime.Now.Month - 1; // Chọn tháng hiện tại mặc định
        }

        private void FilterDataByMonth(int month)
        {
            using (SqlConnection conn = new SqlConnection(connection))
            {
                try
                {
                    conn.Open();

                    // Lấy năm hiện tại
                    int currentYear = DateTime.Now.Year;

                    // Truy vấn lọc dữ liệu theo tháng và năm
                    string query = @"
            SELECT 
                nh.MaNhapHang,
                nh.MaSanPham,
                sp.TenSanPham,
                nh.NgayNhapHang,
                nh.SoLuong,
                nh.DonGia,
                nh.SoLuong * nh.DonGia AS ThanhTien,
                nc.TenNhaCungCap
            FROM 
                NhapHang nh
            INNER JOIN 
                SanPham sp ON nh.MaSanPham = sp.MaSanPham
            INNER JOIN 
                NhaCungCap nc ON nh.MaNhaCungCap = nc.MaNhaCungCap
            WHERE 
                MONTH(nh.NgayNhapHang) = @Month AND YEAR(nh.NgayNhapHang) = @Year
            ORDER BY 
                nh.NgayNhapHang DESC";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Month", month);
                    cmd.Parameters.AddWithValue("@Year", currentYear);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    // Gán dữ liệu lọc vào DataGridView
                    dtgNhapHang.DataSource = dt;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi lọc dữ liệu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void LoadData()
        {
            // Mở kết nối với cơ sở dữ liệu
            using (SqlConnection conn = new SqlConnection(connection))
            {
                try
                {
                    conn.Open();

                    // Câu truy vấn để lấy dữ liệu cần thiết
                    string query = @"
                    SELECT 
                        nh.MaNhapHang,
                        nh.MaSanPham,
                        sp.TenSanPham,
                        nh.NgayNhapHang,
                        nh.SoLuong,
                        nh.DonGia,
                        nh.SoLuong * nh.DonGia AS ThanhTien,
                        nc.TenNhaCungCap
                    FROM 
                        NhapHang nh
                    INNER JOIN 
                        SanPham sp ON nh.MaSanPham = sp.MaSanPham
                    INNER JOIN 
                        NhaCungCap nc ON nh.MaNhaCungCap = nc.MaNhaCungCap
                    ORDER BY 
                        nh.NgayNhapHang DESC";

                    // Tạo đối tượng SqlDataAdapter để lấy dữ liệu từ cơ sở dữ liệu
                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();

                    // Đổ dữ liệu vào DataTable
                    da.Fill(dt);

                    // Gán dữ liệu cho DataGridView
                    dtgNhapHang.DataSource = dt;

                    // Đặt header cho các cột (tiếng Việt)
                    dtgNhapHang.Columns["MaNhapHang"].HeaderText = "Mã Nhập Hàng";
                    dtgNhapHang.Columns["MaSanPham"].HeaderText = "Mã Sản Phẩm";
                    dtgNhapHang.Columns["TenSanPham"].HeaderText = "Tên Sản Phẩm";
                    dtgNhapHang.Columns["NgayNhapHang"].HeaderText = "Ngày Nhập Hàng";
                    dtgNhapHang.Columns["SoLuong"].HeaderText = "Số Lượng"; 
                    dtgNhapHang.Columns["DonGia"].DefaultCellStyle.Format = "N0"; 
                    dtgNhapHang.Columns["ThanhTien"].DefaultCellStyle.Format = "N0";
                    dtgNhapHang.Columns["TenNhaCungCap"].HeaderText = "Tên Nhà Cung Cấp";
                    dtgNhapHang.Columns["TenNhaCungCap"].Width = 150;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi tải dữ liệu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedMaNhapHang))
            {
                MessageBox.Show("Vui lòng chọn một mục để sửa.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Mở form sửa nhập hàng và truyền mã nhập hàng vào
            frmSuaNhapHang frm = new frmSuaNhapHang(selectedMaNhapHang);
            frm.FormClosed += (s, args) =>
            {
                LoadData();
            };
            frm.ShowDialog();
        }

        private void dtgNhapHang_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Kiểm tra nếu cột "MaNhapHang" có dữ liệu và không phải là header row
            if (e.RowIndex >= 0)
            {
                // Lấy mã nhập hàng từ cột "MaNhapHang" ở dòng được chọn
                selectedMaNhapHang = dtgNhapHang.Rows[e.RowIndex].Cells["MaNhapHang"].Value.ToString();
            }
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cbxLocTheoThang_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selectedMonth = cbxLocTheoThang.SelectedIndex + 1;
            FilterDataByMonth(selectedMonth);
        }
    }
}
