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
using Excel = Microsoft.Office.Interop.Excel;

namespace HTQLCoffee.QLSanPham_Kho
{
    public partial class frmThongKeSanPham : Form
    {
        string connection = ConfigurationManager.ConnectionStrings["HTQLCoffee.Properties.Settings.QLCoffeeConnectionString"].ConnectionString;

        public frmThongKeSanPham()
        {
            InitializeComponent();
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmThongKeSanPham_Load(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connection))
                {
                    conn.Open();

                    // Câu truy vấn để lấy thông tin sản phẩm và số lượng tồn kho
                    string query = @"
                SELECT sp.MaSanPham, sp.TenSanPham, sp.LoaiSanPham, sp.GiaBan, sp.DonViTinh, qlk.SoLuongTon
                FROM SanPham sp
                LEFT JOIN QuanLyKho qlk ON sp.MaSanPham = qlk.MaSanPham";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        // Sử dụng SqlDataAdapter để lấy dữ liệu và điền vào DataTable
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);

                        // Gán dữ liệu cho DataGridView (dtgThongKeSanPham)
                        dtgThongKeSanPham.DataSource = dataTable;
                        // Đặt tiêu đề cột cho DataGridView theo đúng ý muốn
                        dtgThongKeSanPham.Columns["MaSanPham"].HeaderText = "Mã sản phẩm";
                        dtgThongKeSanPham.Columns["TenSanPham"].HeaderText = "Tên sản phẩm";
                        dtgThongKeSanPham.Columns["LoaiSanPham"].HeaderText = "Loại sản phẩm";
                        dtgThongKeSanPham.Columns["GiaBan"].HeaderText = "Giá bán";
                        dtgThongKeSanPham.Columns["DonViTinh"].HeaderText = "Đơn vị tính";
                        dtgThongKeSanPham.Columns["SoLuongTon"].HeaderText = "Số lượng tồn";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi xảy ra: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnExportToExcel_Click(object sender, EventArgs e)
        {
            // Hỏi người dùng có muốn xuất file Excel không
            DialogResult result = MessageBox.Show("Bạn có muốn xuất dữ liệu ra file Excel không?", "Xuất File Excel", MessageBoxButtons.YesNo);

            if (result == DialogResult.Yes)
            {
                ExportToExcel(); // Gọi hàm xuất dữ liệu ra Excel
            }
            else
            {
                // Nếu người dùng chọn No, không làm gì cả
                MessageBox.Show("Xuất file bị hủy!");
            }
        }
        private void ExportToExcel()
        {
            Excel.Application excelApp = new Excel.Application();
            Excel.Workbook workbook = excelApp.Workbooks.Add();
            Excel.Worksheet worksheet = (Excel.Worksheet)workbook.Sheets[1];

            // Thêm tiêu đề cột
            worksheet.Cells[1, 1] = "Mã sản phẩm";
            worksheet.Cells[1, 2] = "Tên sản phẩm";
            worksheet.Cells[1, 3] = "Loại sản phẩm";
            worksheet.Cells[1, 4] = "Giá bán";
            worksheet.Cells[1, 5] = "Đơn vị tính";
            worksheet.Cells[1, 6] = "Số lượng tồn";

            // Canh giữa cho tiêu đề cột
            Excel.Range headerRange = worksheet.Range["A1:F1"];
            headerRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            headerRange.Font.Bold = true;

            // Thêm dữ liệu từ DataGridView
            int rowIndex = 2;
            foreach (DataGridViewRow row in dtgThongKeSanPham.Rows)
            {
                if (row.Cells["MaSanPham"].Value != null) // Kiểm tra ô không rỗng
                {
                    worksheet.Cells[rowIndex, 1] = row.Cells["MaSanPham"].Value;
                    worksheet.Cells[rowIndex, 2] = row.Cells["TenSanPham"].Value;
                    worksheet.Cells[rowIndex, 3] = row.Cells["LoaiSanPham"].Value;
                    worksheet.Cells[rowIndex, 4] = row.Cells["GiaBan"].Value;
                    worksheet.Cells[rowIndex, 5] = row.Cells["DonViTinh"].Value;
                    worksheet.Cells[rowIndex, 6] = row.Cells["SoLuongTon"].Value;
                    rowIndex++;
                }
            }

            // Kẻ bảng
            Excel.Range dataRange = worksheet.Range["A1:F" + (rowIndex - 1).ToString()];
            dataRange.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

            // Canh lề cho dữ liệu
            dataRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;

            // Điều chỉnh kích thước cột tự động
            worksheet.Columns.AutoFit();

            // Lưu file
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Excel Files|*.xls;*.xlsx";
            saveFileDialog.Title = "Lưu file Excel";
            saveFileDialog.FileName = "dulieusanpham.xlsx"; // Tên mặc định cho file

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                workbook.SaveAs(saveFileDialog.FileName);
                workbook.Close();
                excelApp.Quit();
                MessageBox.Show("Xuất dữ liệu thành công!");
            }
            else
            {
                workbook.Close(false);
                excelApp.Quit();
            }
        }
    }
}
