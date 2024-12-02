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

namespace SERVERQLCoffee.DMKH
{
    public partial class frmKhachHang : Form
    {
        string connection = ConfigurationManager.ConnectionStrings["SERVERQLCoffee.Properties.Settings.QLCoffeeConnectionString"].ConnectionString;
        public frmKhachHang()
        {
            InitializeComponent();
        }

        private void frmKhachHang_Load(object sender, EventArgs e)
        {
            LoadKhachHang();
            LoadChiNhanh();

            toolTipBtn.SetToolTip(this.btnThoat, "Thoát Trang");
            toolTipBtn.SetToolTip(this.btnTimKiem, "Tìm Kiếm Khách Hàng");
        }

        private void LoadChiNhanh()
        {
            using (SqlConnection conn = new SqlConnection(connection))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT MaChiNhanh, TenChiNhanh FROM ChiNhanh";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    // Gắn dữ liệu vào ComboBox
                    cbxTenChiNhanh.DataSource = dt;
                    cbxTenChiNhanh.DisplayMember = "TenChiNhanh"; // Hiển thị tên chi nhánh
                    cbxTenChiNhanh.ValueMember = "MaChiNhanh";    // Lấy giá trị mã chi nhánh
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi tải dữ liệu chi nhánh: " + ex.Message);
                }
            }
        }

        private void LoadKhachHang(string maChiNhanh = null)
        {
            using (SqlConnection conn = new SqlConnection(connection))
            {
                try
                {
                    conn.Open();
                    string query = @"SELECT MaKhachHang, ChiNhanh.TenChiNhanh, HoTen, KhachHang.SoDienThoai, Email, NgaySinh, KhachHang.DiaChi, GioiTinh, LoaiKhachHang, NgayTao, NgayCapNhat, GhiChu 
                                     FROM KhachHang 
                                     JOIN ChiNhanh ON KhachHang.MaChiNhanh = ChiNhanh.MaChiNhanh";

                    // Nếu có mã chi nhánh, thêm điều kiện lọc
                    if (!string.IsNullOrEmpty(maChiNhanh))
                    {
                        query += " WHERE KhachHang.MaChiNhanh = @MaChiNhanh";
                    }

                    SqlCommand cmd = new SqlCommand(query, conn);

                    if (!string.IsNullOrEmpty(maChiNhanh))
                    {
                        cmd.Parameters.AddWithValue("@MaChiNhanh", maChiNhanh);
                    }

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    // Gắn dữ liệu vào DataGridView
                    dtgKhachHang.DataSource = dt;

                    // Đặt tiêu đề cột như ban đầu
                    SetDataGridViewHeaders();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi tải danh sách khách hàng: " + ex.Message);
                }
            }
        }

        private void SetDataGridViewHeaders()
        {
            dtgKhachHang.Columns["MaKhachHang"].HeaderText = "ID";
            dtgKhachHang.Columns["TenChiNhanh"].HeaderText = "Chi Nhánh";
            dtgKhachHang.Columns["HoTen"].HeaderText = "Họ Tên";
            dtgKhachHang.Columns["SoDienThoai"].HeaderText = "Số Điện Thoại";
            dtgKhachHang.Columns["Email"].HeaderText = "Email";
            dtgKhachHang.Columns["NgaySinh"].HeaderText = "Ngày Sinh";
            dtgKhachHang.Columns["DiaChi"].HeaderText = "Địa Chỉ";
            dtgKhachHang.Columns["GioiTinh"].HeaderText = "Giới Tính";
            dtgKhachHang.Columns["LoaiKhachHang"].HeaderText = "Loại Khách Hàng";
            dtgKhachHang.Columns["NgayTao"].HeaderText = "Ngày Tạo";
            dtgKhachHang.Columns["NgayCapNhat"].HeaderText = "Ngày Cập Nhật";
            dtgKhachHang.Columns["GhiChu"].HeaderText = "Ghi Chú";

            foreach (DataGridViewColumn column in dtgKhachHang.Columns)
            {
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            }
        }

        // Sự kiện khi thay đổi lựa chọn trên ComboBox
        private void cbxTenChiNhanh_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxTenChiNhanh.SelectedValue is string selectedValue)
            {
                LoadKhachHang(selectedValue); // Lọc khách hàng theo mã chi nhánh
            }
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            string searchPhone = txtTimKiem.Text.Trim();

            if (string.IsNullOrEmpty(searchPhone) || searchPhone == "Nhập vào số điện thoại")
            {
                MessageBox.Show("Vui lòng nhập số điện thoại để tìm kiếm!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string query = @"SELECT MaKhachHang, HoTen, SoDienThoai, Email, NgaySinh, DiaChi, GioiTinh, LoaiKhachHang, NgayTao, NgayCapNhat, GhiChu 
                     FROM KhachHang WHERE SoDienThoai = @SDT or HoTen = @SDT or Email = @SDT";

            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    command.Parameters.AddWithValue("@SDT", searchPhone);
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    // Kiểm tra xem có kết quả hay không
                    if (dt.Rows.Count > 0)
                    {
                        dtgKhachHang.DataSource = dt; // Cập nhật DataGridView
                        dtgKhachHang.Rows[0].Selected = true; // Chọn dòng đầu tiên
                        dtgKhachHang.CurrentCell = dtgKhachHang.Rows[0].Cells[0]; // Đặt vị trí hiện tại

                        // Cập nhật thông tin vào các TextBox
                        DataRow row = dt.Rows[0]; // Lấy dòng đầu tiên
                        txtMaKhach.Text = row["MaKhachHang"].ToString();
                        txtHoTen.Text = row["HoTen"].ToString();
                        txtDiaChi.Text = row["DiaChi"].ToString();
                        txtLoaiKhach.Text = row["LoaiKhachHang"].ToString();
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy khách hàng với số điện thoại này!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }

        private void txtTimKiem_TextChanged(object sender, EventArgs e)
        {
            string searchText = txtTimKiem.Text.Trim();


            // Hiển thị gợi ý chỉ khi có ít nhất 2 ký tự
            if (searchText.Length >= 1)
            {
                ShowSuggestions(searchText);
            }
            else
            {
                listBoxSuggestions.Visible = false; // Ẩn nếu không đủ ký tự
            }
        }

        private void ShowSuggestions(string searchText)
        {
            listBoxSuggestions.Items.Clear(); // Xóa các gợi ý cũ

            // Xác định tìm kiếm theo số hay chữ
            bool isNumeric = searchText.All(char.IsDigit);

            string query;

            if (string.IsNullOrWhiteSpace(searchText))
            {
                // Nếu không nhập gì, hiển thị toàn bộ danh sách
                query = "SELECT SoDienThoai, HoTen, Email FROM KhachHang";
            }
            else if (isNumeric)
            {
                // Nếu là số, tìm kiếm theo số điện thoại
                query = "SELECT SoDienThoai FROM KhachHang WHERE SoDienThoai LIKE @SearchText";
            }
            else
            {
                // Nếu là chữ, tìm kiếm theo họ tên hoặc email
                query = "SELECT HoTen FROM KhachHang WHERE HoTen LIKE @SearchText UNION SELECT Email FROM KhachHang WHERE Email LIKE @SearchText";
            }

            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();

                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    if (!string.IsNullOrWhiteSpace(searchText))
                    {
                        command.Parameters.AddWithValue("@SearchText", "%" + searchText + "%");
                    }

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Chỉ hiển thị giá trị gợi ý theo kiểu tìm kiếm
                            string suggestion = reader[0].ToString();
                            listBoxSuggestions.Items.Add(suggestion);
                        }
                    }
                }
            }

            // Hiển thị gợi ý nếu có ít nhất một kết quả
            listBoxSuggestions.Visible = listBoxSuggestions.Items.Count > 0;
        }
        private void listBoxSuggestions_MouseClick(object sender, MouseEventArgs e)
        {
            if (listBoxSuggestions.SelectedItem != null)
            {
                txtTimKiem.Text = listBoxSuggestions.SelectedItem.ToString();
                listBoxSuggestions.Visible = false; // Ẩn gợi ý sau khi chọn
            }
        }

        private void txtTimKiem_Enter(object sender, EventArgs e)
        {
            if (txtTimKiem.Text == "Nhập vào số điện thoại...")
            {
                txtTimKiem.Text = ""; // Xóa placeholder
                txtTimKiem.ForeColor = Color.Black; // Đặt lại màu chữ
            }
        }

        private string customerId;
        private void dtgKhachHang_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < dtgKhachHang.Rows.Count - 1)
            {
                // Lấy MaKhachHang từ DataGridView và lưu dưới dạng string
                customerId = dtgKhachHang.Rows[e.RowIndex].Cells["MaKhachHang"].Value.ToString();
                txtMaKhach.Text = customerId;
                txtHoTen.Text = dtgKhachHang.Rows[e.RowIndex].Cells["HoTen"].Value.ToString();
                txtDiaChi.Text = dtgKhachHang.Rows[e.RowIndex].Cells["DiaChi"].Value.ToString();
                txtLoaiKhach.Text = dtgKhachHang.Rows[e.RowIndex].Cells["LoaiKhachHang"].Value.ToString();
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
            worksheet.Cells[1, 1] = "Mã khách hàng";
            worksheet.Cells[1, 2] = "Tên khách hàng";
            worksheet.Cells[1, 3] = "Số điện thoại";
            worksheet.Cells[1, 4] = "Email";
            worksheet.Cells[1, 5] = "Ngày sinh";
            worksheet.Cells[1, 6] = "Địa chỉ";
            worksheet.Cells[1, 7] = "Giới tính";
            worksheet.Cells[1, 8] = "Loại khách hàng";
            worksheet.Cells[1, 9] = "Tên chi nhánh";

            // Canh giữa cho tiêu đề cột
            Excel.Range headerRange = worksheet.Range["A1:F1"];
            headerRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            headerRange.Font.Bold = true;

            // Thêm dữ liệu từ DataGridView
            int rowIndex = 2;
            foreach (DataGridViewRow row in dtgKhachHang.Rows)
            {
                if (row.Cells["MaKhachHang"].Value != null) // Kiểm tra ô không rỗng
                {
                    worksheet.Cells[rowIndex, 1] = row.Cells["MaKhachHang"].Value;
                    worksheet.Cells[rowIndex, 2] = row.Cells["HoTen"].Value;
                    worksheet.Cells[rowIndex, 3] = row.Cells["SoDienThoai"].Value;
                    worksheet.Cells[rowIndex, 4] = row.Cells["Email"].Value;
                    worksheet.Cells[rowIndex, 5] = row.Cells["NgaySinh"].Value;
                    worksheet.Cells[rowIndex, 6] = row.Cells["DiaChi"].Value;
                    worksheet.Cells[rowIndex, 7] = row.Cells["GioiTinh"].Value;
                    worksheet.Cells[rowIndex, 8] = row.Cells["LoaiKhachHang"].Value;
                    worksheet.Cells[rowIndex, 9] = row.Cells["TenChiNhanh"].Value;
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
            saveFileDialog.FileName = "dulieukhachhang.xlsx"; // Tên mặc định cho file

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

        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadKhachHang();
        }
    }
}
