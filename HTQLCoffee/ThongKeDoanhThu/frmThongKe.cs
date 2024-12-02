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
using System.Windows.Forms.DataVisualization.Charting;

namespace HTQLCoffee.ThongKeDoanhThu
{
    public partial class frmThongKe : Form
    {
        string connection = ConfigurationManager.ConnectionStrings["HTQLCoffee.Properties.Settings.QLCoffeeConnectionString"].ConnectionString;

        private int selectedMonth;
        private int selectedYear;
        private decimal tongChiPhi;
        private decimal tongDoanhThu;
        private decimal loiNhuan;
        public frmThongKe()
        {
            InitializeComponent();
        }
        private void frmThongKe_Load(object sender, EventArgs e)
        {
            AddExplanationLabels();
            // Kiểm tra comboBoxMonth có hợp lệ không
            if (comboBoxMonth != null)
            {
                comboBoxMonth.Items.Clear(); // Xóa các mục cũ nếu có
                // Load các tháng từ 1 đến 12
                for (int month = 1; month <= 12; month++)
                {
                    comboBoxMonth.Items.Add(month);
                }

                // Chọn tháng hiện tại nếu comboBoxMonth có ít nhất một mục
                if (comboBoxMonth.Items.Count > 0)
                {
                    comboBoxMonth.SelectedItem = DateTime.Now.Month;
                    selectedMonth = DateTime.Now.Month; // Lưu lại tháng hiện tại
                }
            }

            // Kiểm tra comboBoxYear có hợp lệ không
            if (comboBoxYear != null)
            {
                comboBoxYear.Items.Clear(); // Xóa các mục cũ nếu có
                // Load các năm từ 1990 đến năm hiện tại
                for (int year = 1990; year <= DateTime.Now.Year; year++)
                {
                    comboBoxYear.Items.Add(year);
                }

                // Chọn năm hiện tại nếu comboBoxYear có ít nhất một mục
                if (comboBoxYear.Items.Count > 0)
                {
                    comboBoxYear.SelectedItem = DateTime.Now.Year;
                    selectedYear = DateTime.Now.Year; // Lưu lại năm hiện tại
                }
            }
        }
        private void LoadChartData(int month, int year)
        {
            // Tính tổng chi tiêu
            decimal totalChiPhiKhac = GetTotalChiPhiKhac(month, year);
            decimal totalNhapHang = GetTotalNhapHang(month, year);
            
            decimal totalLuongNhanVien = GetTotalLuongNhanVien(month, year);

            // Tính tổng thu nhập
            decimal totalThuNhap = GetTotalThuNhap(month, year);

            // Tính lợi nhuận (Lợi nhuận = Tổng Thu - Tổng Chi)
            decimal totalChiTieu = totalChiPhiKhac + totalNhapHang  + totalLuongNhanVien;
            decimal profit = totalThuNhap - totalChiTieu;
            tongChiPhi = totalChiTieu;
            tongDoanhThu = totalThuNhap;
            loiNhuan = profit;
            // Cập nhật biểu đồ
            UpdateChart(totalChiPhiKhac, totalNhapHang, totalLuongNhanVien, totalThuNhap, profit);
        }
        private void UpdateChart(decimal totalChiPhiKhac, decimal totalNhapHang , decimal totalLuongNhanVien, decimal totalThuNhap, decimal profit)
        {
            // Cập nhật biểu đồ chi tiêu
            chartChiTieu.Series.Clear();
            Series seriesChiTieu = new Series("Chi Tiêu");
            seriesChiTieu.Points.AddXY("A", totalChiPhiKhac);
            seriesChiTieu.Points.AddXY("B", totalNhapHang);
            seriesChiTieu.Points.AddXY("C", totalLuongNhanVien);
            chartChiTieu.Series.Add(seriesChiTieu);

            // Cập nhật biểu đồ thu nhập
            chartThuNhap.Series.Clear();
            Series seriesThuNhap = new Series("Thu Nhập");
            seriesThuNhap.Points.AddXY("D", totalThuNhap);  // Thu nhập (E)
            chartThuNhap.Series.Add(seriesThuNhap);

            // Cập nhật biểu đồ lợi nhuận
            chartLoiNhuan.Series.Clear();
            Series seriesLoiNhuan = new Series("Lợi Nhuận");
            seriesLoiNhuan.Points.AddXY("E", profit);  // Lợi nhuận (F)
            chartLoiNhuan.Series.Add(seriesLoiNhuan);

            // Định dạng giá trị trên biểu đồ với VND
            FormatChartLabels(chartChiTieu, totalChiPhiKhac, totalNhapHang, totalLuongNhanVien);
            FormatChartLabels(chartThuNhap, totalThuNhap);
            FormatChartLabels(chartLoiNhuan, profit);

            // Định dạng các số trên trục Y
            FormatYAxisLabels(chartChiTieu);
            FormatYAxisLabels(chartThuNhap);
            FormatYAxisLabels(chartLoiNhuan);

            // Thay đổi chiều dài trục X
            IncreaseXAxisLength(chartChiTieu);
            IncreaseXAxisLength(chartThuNhap);
            IncreaseXAxisLength(chartLoiNhuan);
        }

        private void FormatChartLabels(Chart chart, params decimal[] values)
        {
            foreach (var series in chart.Series)
            {
                foreach (var point in series.Points)
                {
                    point.Label = string.Format("{0:N0} VND", point.YValues[0]);
                    point.LabelForeColor = Color.DarkBlue;
                    point.Font = new Font("Arial", 10, FontStyle.Bold);
                }
            }
        }

        
        private void IncreaseXAxisLength(Chart chart)
        {
            foreach (var chartArea in chart.ChartAreas)
            {
                chartArea.InnerPlotPosition = new ElementPosition(20, 10, 90, 85);

                // Điều chỉnh các nhãn trục X
                chartArea.AxisX.LabelStyle.Angle = 0;
                chartArea.AxisX.IsLabelAutoFit = true;
                chartArea.AxisX.Interval = 1;

            }
        }

        private void FormatYAxisLabels(Chart chart)
        {
            foreach (var axis in chart.ChartAreas)
            {
                // Lấy giá trị lớn nhất trong các series
                decimal maxValue = chart.Series
                    .SelectMany(series => series.Points)
                    .DefaultIfEmpty(new DataPoint(0, 1)) // Đảm bảo không lỗi khi không có dữ liệu
                    .Max(point => (decimal)point.YValues[0]);

                // Thiết lập giá trị tối thiểu và tối đa
                axis.AxisY.Minimum = 0;
                axis.AxisY.Maximum = maxValue > 0 ? (double)(maxValue * 1.1m) : 1; // Tối thiểu 1 để luôn hiển thị cột
                axis.AxisY.LabelStyle.Format = "#,##0";
            }
        }

        private void AddExplanationLabels()
        {
            // Cập nhật nội dung giải thích cho các ký hiệu
            labelExplanation.Text = "A: Chi Phí Khác\n" +
                                    "B: Nhập Hàng\n" +
                                    "C: Lương Nhân Viên\n" +
                                    "D: Thu Nhập (Hóa Đơn)\n" +
                                    "E: Lợi Nhuận (Thu Nhập - Tổng Chi Tiêu)";
            labelExplanation.ForeColor = Color.DodgerBlue;
        }


        private decimal GetTotalChiPhiKhac(int month, int year)
        {
            decimal total = 0;
            string query = @"SELECT SUM(SoTien)
                     FROM ChiPhiKhac
                     WHERE MONTH(NgayChi) = @Month AND YEAR(NgayChi) = @Year";

            // Thực hiện truy vấn và lấy tổng chi phí
            using (var conn = new SqlConnection(connection))
            {
                conn.Open();
                using (var command = new SqlCommand(query, conn))
                {
                    command.Parameters.AddWithValue("@Month", month);
                    command.Parameters.AddWithValue("@Year", year);

                    var result = command.ExecuteScalar();
                    total = result == DBNull.Value ? 0 : Convert.ToDecimal(result);
                }
            }

            return total;
        }

        private decimal GetTotalNhapHang(int month, int year)
        {
            decimal total = 0;
            string query = @"SELECT SUM(SoLuong * DonGia)
                     FROM NhapHang
                     WHERE MONTH(NgayNhapHang) = @Month AND YEAR(NgayNhapHang) = @Year";

            // Thực hiện truy vấn và lấy tổng nhập hàng
            using (var conn = new SqlConnection(connection))
            {
                conn.Open();
                using (var command = new SqlCommand(query, conn))
                {
                    command.Parameters.AddWithValue("@Month", month);
                    command.Parameters.AddWithValue("@Year", year);

                    var result = command.ExecuteScalar();
                    total = result == DBNull.Value ? 0 : Convert.ToDecimal(result);
                }
            }

            return total;
        }

        private decimal GetTotalThuNhap(int month, int year)
        {
            decimal total = 0;
            string query = @"
        SELECT SUM(TRY_CAST(REPLACE(ThanhToan, '.00', '') AS DECIMAL(18, 2))) 
        FROM HoaDon
        WHERE MONTH(NgayTao) = @Month AND YEAR(NgayTao) = @Year";

            // Thực hiện truy vấn và lấy tổng thu nhập
            using (var conn = new SqlConnection(connection))
            {
                conn.Open();
                using (var command = new SqlCommand(query, conn))
                {
                    command.Parameters.AddWithValue("@Month", month);
                    command.Parameters.AddWithValue("@Year", year);

                    var result = command.ExecuteScalar();
                    total = result == DBNull.Value ? 0 : Convert.ToDecimal(result);
                }
            }

            return total;
        }

        private decimal GetTotalLuongNhanVien(int month, int year)
        {
            decimal total = 0;
            string query = @"SELECT SUM(TongLuong)
                     FROM LichSuNhanLuong
                     WHERE MONTH(Thang) = @Month AND YEAR(Nam) = @Year AND TrangThaiNhanLuong = 1";

            // Thực hiện truy vấn và lấy tổng lương nhân viên
            using (var conn = new SqlConnection(connection))
            {
                conn.Open();
                using (var command = new SqlCommand(query, conn))
                {
                    command.Parameters.AddWithValue("@Month", month);
                    command.Parameters.AddWithValue("@Year", year);

                    // Kiểm tra nếu kết quả là DBNull, nếu có, trả về 0
                    var result = command.ExecuteScalar();
                    total = result == DBNull.Value ? 0 : Convert.ToDecimal(result);
                }
            }

            return total;
        }

        

        private void comboBoxMonth_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Cập nhật selectedMonth từ comboBoxMonth
            if (comboBoxMonth.SelectedItem != null)
            {
                selectedMonth = (int)comboBoxMonth.SelectedItem;
            }

            // Gọi lại LoadChartData khi thay đổi tháng
            LoadChartData(selectedMonth, selectedYear);
        }

        private void comboBoxYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Kiểm tra comboBoxYear có giá trị hợp lệ không
            if (comboBoxYear.SelectedItem != null)
            {
                selectedYear = int.Parse(comboBoxYear.SelectedItem.ToString());

                // Đảm bảo selectedMonth có giá trị hợp lệ trước khi gọi LoadChartData
                if (selectedMonth > 0 && selectedMonth <= 12)
                {
                    // Gọi hàm LoadChartData để cập nhật biểu đồ
                    LoadChartData(selectedMonth, selectedYear);
                }
            }
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnCapNhat_Click(object sender, EventArgs e)
        {
            frmCapNhat formCapNhat = new frmCapNhat(tongChiPhi, tongDoanhThu, loiNhuan);
            formCapNhat.ShowDialog();
        }
    }
}
