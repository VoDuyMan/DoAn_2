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

namespace SERVERQLCoffee.QLHD
{
    public partial class frmHoaDon : Form
    {
        string connection = ConfigurationManager.ConnectionStrings["SERVERQLCoffee.Properties.Settings.QLCoffeeConnectionString"].ConnectionString;
        public frmHoaDon()
        {
            InitializeComponent();
        }

        private void frmHoaDon_Load(object sender, EventArgs e)
        {
            LoadChiNhanh();
            LoadHoaDon();
            for (int month = 1; month <= 12; month++)
            {
                cbMonthFilter.Items.Add(month);
            }
            cbMonthFilter.DropDownStyle = ComboBoxStyle.DropDownList;
            cbMonthFilter.SelectedItem = DateTime.Now.Month;

            cbxChiNhanh.DropDownStyle = ComboBoxStyle.DropDownList;
            cbxChiNhanh.SelectedIndexChanged += CbxChiNhanh_SelectedIndexChanged;
        }
        private void LoadChiNhanh()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connection))
                {
                    string query = "SELECT TenChiNhanh FROM ChiNhanh";
                    SqlCommand command = new SqlCommand(query, conn);
                    conn.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        cbxChiNhanh.Items.Add(reader["TenChiNhanh"].ToString());
                    }

                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
        private void LoadHoaDon()
        {
            try
            {
                // Initialize a new SQL connection
                using (SqlConnection conn = new SqlConnection(connection))
                {
                    // SQL query with JOINs to get the required data
                    string query = @"
                SELECT 
                    hd.MaHoaDon, 
                    kh.HoTen, 
                    ph.TenBan, 
                    hd.NgayLapHoaDon, 
                    hd.TongTien, 
                    hd.GiamGia, 
                    hd.ThanhToan 
                FROM [dbo].[HoaDon] hd
                LEFT JOIN [dbo].[KhachHang] kh ON hd.MaKhachHang = kh.MaKhachHang
                LEFT JOIN [dbo].[BanAn] ph ON hd.MaBan = ph.MaBan
                ORDER BY hd.NgayLapHoaDon DESC"; // Optional: Order by date

                    // Initialize a SQL command with the query and connection
                    SqlCommand command = new SqlCommand(query, conn);

                    // Open the SQL connection
                    conn.Open();

                    // Use SqlDataAdapter to fill a DataTable with the result of the query
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    // Set the DataGridView's DataSource to the DataTable
                    dataGridViewHoaDon.DataSource = dataTable;

                    dataGridViewHoaDon.Columns["MaHoaDon"].HeaderText = "Mã Hóa Đơn";
                    dataGridViewHoaDon.Columns["HoTen"].HeaderText = "Tên Khách Hàng";
                    dataGridViewHoaDon.Columns["TenBan"].HeaderText = "Tên Bàn";
                    
                    dataGridViewHoaDon.Columns["NgayLapHoaDon"].HeaderText = "Ngày Lập";
                    dataGridViewHoaDon.Columns["TongTien"].HeaderText = "Tổng Tiền";
                    dataGridViewHoaDon.Columns["GiamGia"].HeaderText = "Giảm Giá";
                    dataGridViewHoaDon.Columns["ThanhToan"].HeaderText = "Thanh Toán";
                }
            }
            catch (Exception ex)
            {
                // Display any errors that occur
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        // Lọc theo ngày hiện tại
        private void BtnFilterToday_Click(object sender, EventArgs e)
        {
            string today = DateTime.Now.ToString("yyyy-MM-dd");
            FilterData("CONVERT(date, NgayLapHoaDon) = '" + today + "'");
        }

        // Lọc theo tháng hiện tại
        private void BtnFilterThisMonth_Click(object sender, EventArgs e)
        {
            int currentMonth = DateTime.Now.Month;
            int currentYear = DateTime.Now.Year;
            FilterData("MONTH(NgayLapHoaDon) = " + currentMonth + " AND YEAR(NgayLapHoaDon) = " + currentYear);
        }

        // Lọc theo năm hiện tại
        private void BtnFilterThisYear_Click(object sender, EventArgs e)
        {
            int currentYear = DateTime.Now.Year;
            FilterData("YEAR(NgayLapHoaDon) = " + currentYear);
        }

        // Lọc theo tháng cụ thể từ ComboBox
        private void CbMonthFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selectedMonth = int.Parse(cbMonthFilter.SelectedItem.ToString());
            int currentYear = DateTime.Now.Year;
            FilterData("MONTH(NgayLapHoaDon) = " + selectedMonth + " AND YEAR(NgayLapHoaDon) = " + currentYear);
        }

        // Lọc theo ngày cụ thể từ DateTimePicker
        private void DtpFilterDate_ValueChanged(object sender, EventArgs e)
        {
            string selectedDate = dtpFilterDate.Value.ToString("yyyy-MM-dd");
            FilterData("CONVERT(date, NgayLapHoaDon) = '" + selectedDate + "'");
        }

        private void FilterData(string filterCondition)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connection))
                {
                    // Query with filter condition
                    string query =
                        "SELECT " +
                        "hd.MaHoaDon AS [Mã Hóa Đơn], " +
                        "kh.HoTen AS [Tên Khách Hàng], " +
                        "ph.TenBan AS [Tên Bàn], " +
                       
                        "hd.NgayLapHoaDon AS [Ngày Lập Hóa Đơn], " +
                        "hd.TongTien AS [Tổng Tiền], " +
                        "hd.GiamGia AS [Giảm Giá], " +
                        "hd.ThanhToan AS [Thanh Toán] " +
                        "FROM [dbo].[HoaDon] hd " +
                        "LEFT JOIN [dbo].[KhachHang] kh ON hd.MaKhachHang = kh.MaKhachHang " +
                        "LEFT JOIN [dbo].[BanAn] ph ON hd.MaBan = ph.MaBan " +
                        "WHERE " + filterCondition +
                        " ORDER BY hd.NgayLapHoaDon DESC";

                    SqlCommand command = new SqlCommand(query, conn);
                    conn.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    dataGridViewHoaDon.DataSource = dataTable;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void CbxChiNhanh_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedBranch = cbxChiNhanh.SelectedItem.ToString();
            FilterDataByBranch(selectedBranch);
        }

        private void FilterDataByBranch(string branchName)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connection))
                {
                    string query = @"
                    SELECT 
                        hd.MaHoaDon AS [Mã Hóa Đơn], 
                        kh.HoTen AS [Tên Khách Hàng], 
                        ph.TenBan AS [Tên Bàn], 
                       
                        hd.NgayLapHoaDon AS [Ngày Lập Hóa Đơn], 
                        hd.TongTien AS [Tổng Tiền], 
                        hd.GiamGia AS [Giảm Giá], 
                        hd.ThanhToan AS [Thanh Toán] 
                    FROM [dbo].[HoaDon] hd 
                    LEFT JOIN [dbo].[KhachHang] kh ON hd.MaKhachHang = kh.MaKhachHang 
                    LEFT JOIN [dbo].[BanAn] ph ON hd.MaBan = ph.MaBan 
                    LEFT JOIN [dbo].[ChiNhanh] cn ON ph.MaChiNhanh = cn.MaChiNhanh 
                    WHERE cn.TenChiNhanh = @BranchName 
                    ORDER BY hd.NgayLapHoaDon DESC";

                    SqlCommand command = new SqlCommand(query, conn);
                    command.Parameters.AddWithValue("@BranchName", branchName);

                    conn.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    dataGridViewHoaDon.DataSource = dataTable;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadHoaDon();
        }
    }
}
