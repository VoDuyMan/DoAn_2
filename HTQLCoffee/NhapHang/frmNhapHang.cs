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
    public partial class frmNhapHang : Form
    {
        string connection = ConfigurationManager.ConnectionStrings["HTQLCoffee.Properties.Settings.QLCoffeeConnectionString"].ConnectionString;
        private List<string> _selectedProductCodes;

        public frmNhapHang(List<string> selectedProductCodes)
        {
            InitializeComponent();
            _selectedProductCodes = selectedProductCodes;
        }

        private void frmNhapHang_Load(object sender, EventArgs e)
        {
            // Load danh sách nhà cung cấp vào ComboBox
            LoadNhaCungCap();

            // Hiển thị danh sách sản phẩm lên DataGridView
            LoadProducts();

            dgvNhapHang.CellFormatting += dgvNhapHang_CellFormatting;
        }

        private void LoadNhaCungCap()
        {
            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();
                string query = "SELECT MaNhaCungCap, TenNhaCungCap FROM NhaCungCap";
                SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                cboNhaCungCap.DisplayMember = "TenNhaCungCap";
                cboNhaCungCap.ValueMember = "MaNhaCungCap";
                cboNhaCungCap.DataSource = dt;
            }
        }

        private void LoadProducts()
        {
            // Kiểm tra danh sách mã sản phẩm
            if (_selectedProductCodes == null || _selectedProductCodes.Count == 0)
            {
                MessageBox.Show("Không có sản phẩm được chọn!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Tiến hành tải sản phẩm vào DataGridView
            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();
                string query = "SELECT MaSanPham, TenSanPham, LoaiSanPham, GiaBan FROM SanPham WHERE MaSanPham IN (@MaSanPham)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@MaSanPham", string.Join(",", _selectedProductCodes));

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                dgvNhapHang.DataSource = dt;
                dgvNhapHang.Columns["MaSanPham"].HeaderText = "Mã sản phẩm";
                dgvNhapHang.Columns["TenSanPham"].HeaderText = "Tên sản phẩm";
                dgvNhapHang.Columns["GiaBan"].HeaderText = "Giá bán";
                dgvNhapHang.Columns["LoaiSanPham"].HeaderText = "Loại";

                // Thêm cột nhập số lượng
                DataGridViewTextBoxColumn colSoLuong = new DataGridViewTextBoxColumn();
                colSoLuong.Name = "SoLuong";
                colSoLuong.HeaderText = "Số lượng";
                dgvNhapHang.Columns.Add(colSoLuong);

                // Thêm cột nhập đơn giá (cho người dùng nhập)
                DataGridViewTextBoxColumn colDonGia = new DataGridViewTextBoxColumn();
                colDonGia.Name = "DonGia";
                colDonGia.HeaderText = "Đơn giá";
                dgvNhapHang.Columns.Add(colDonGia);

                // Thêm cột tính thành tiền
                DataGridViewTextBoxColumn colThanhTien = new DataGridViewTextBoxColumn();
                colThanhTien.Name = "ThanhTien";
                colThanhTien.HeaderText = "Thành tiền";
                colThanhTien.ReadOnly = true; // Không cho chỉnh sửa thành tiền
                dgvNhapHang.Columns.Add(colThanhTien);

                // Sự kiện khi thay đổi số lượng hoặc đơn giá, tính lại thành tiền
                dgvNhapHang.CellEndEdit += DgvNhapHang_CellEndEdit;
            }
        }

        private void DgvNhapHang_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            // Kiểm tra nếu người dùng sửa cột Số lượng hoặc Đơn giá
            if (e.ColumnIndex == dgvNhapHang.Columns["SoLuong"].Index || e.ColumnIndex == dgvNhapHang.Columns["DonGia"].Index)
            {
                try
                {
                    // Lấy số lượng và đơn giá đã nhập
                    var soLuong = Convert.ToInt32(dgvNhapHang.Rows[e.RowIndex].Cells["SoLuong"].Value);
                    var donGia = Convert.ToDecimal(dgvNhapHang.Rows[e.RowIndex].Cells["DonGia"].Value);

                    // Tính thành tiền
                    var thanhTien = soLuong * donGia;

                    // Hiển thị thành tiền vào cột Thành tiền
                    dgvNhapHang.Rows[e.RowIndex].Cells["ThanhTien"].Value = thanhTien;
                }
                catch (Exception)
                {
                    MessageBox.Show("Vui lòng nhập số lượng và đơn giá hợp lệ!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private string GenerateRandomCode(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Random random = new Random();
            return new string(Enumerable.Range(0, length)
                                        .Select(_ => chars[random.Next(chars.Length)])
                                        .ToArray());
        }

        private void btnNhap_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();

                // Lấy mã chi nhánh từ bảng ChiNhanh
                string maChiNhanhQuery = "SELECT TOP 1 MaChiNhanh FROM ChiNhanh";
                SqlCommand chiNhanhCmd = new SqlCommand(maChiNhanhQuery, conn);
                string maChiNhanh = chiNhanhCmd.ExecuteScalar().ToString();

                if (maChiNhanh == null)
                {
                    MessageBox.Show("Không tìm thấy mã chi nhánh!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Kiểm tra mã nhà cung cấp
                if (cboNhaCungCap.SelectedValue == null)
                {
                    MessageBox.Show("Vui lòng thêm nhà cung cấp trước khi nhập hàng!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Duyệt qua từng dòng trong DataGridView để lưu thông tin nhập hàng
                foreach (DataGridViewRow row in dgvNhapHang.Rows)
                {
                    if (row.IsNewRow) continue;

                    // Kiểm tra ô "Số lượng" và "Đơn giá" trước khi lưu
                    if (row.Cells["SoLuong"].Value == null || row.Cells["DonGia"].Value == null ||
                        string.IsNullOrWhiteSpace(row.Cells["SoLuong"].Value.ToString()) ||
                        string.IsNullOrWhiteSpace(row.Cells["DonGia"].Value.ToString()))
                    {
                        MessageBox.Show("Vui lòng nhập đầy đủ Số lượng và Đơn giá cho sản phẩm!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Kiểm tra Số lượng hợp lệ
                    string soLuongStr = row.Cells["SoLuong"].Value.ToString();
                    int soLuong;
                    if (!int.TryParse(soLuongStr, out soLuong) || soLuong <= 0)
                    {
                        MessageBox.Show("Vui lòng nhập số lượng hợp lệ!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Lấy đơn giá từ DataGridView
                    decimal donGia;
                    if (!decimal.TryParse(row.Cells["DonGia"].Value.ToString(), out donGia) || donGia <= 0)
                    {
                        MessageBox.Show("Vui lòng nhập đơn giá hợp lệ!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Tính thành tiền từ số lượng và đơn giá
                    decimal thanhTien = soLuong * donGia;

                    // Tạo mã nhập hàng ngẫu nhiên
                    string maNhapHang = GenerateRandomCode(10);

                    // Lưu thông tin nhập hàng vào bảng NhapHang
                    string queryNhapHang = "INSERT INTO NhapHang (MaNhapHang, MaSanPham, NgayNhapHang, SoLuong, DonGia, ThanhTien, MaNhaCungCap, MaChiNhanh) " +
                                           "VALUES (@MaNhapHang, @MaSanPham, @NgayNhapHang, @SoLuong, @DonGia, @ThanhTien, @MaNhaCungCap, @MaChiNhanh)";
                    SqlCommand cmdNhapHang = new SqlCommand(queryNhapHang, conn);

                    cmdNhapHang.Parameters.AddWithValue("@MaNhapHang", maNhapHang);
                    cmdNhapHang.Parameters.AddWithValue("@MaSanPham", row.Cells["MaSanPham"].Value.ToString());
                    cmdNhapHang.Parameters.AddWithValue("@NgayNhapHang", DateTime.Now);
                    cmdNhapHang.Parameters.AddWithValue("@SoLuong", soLuong);
                    cmdNhapHang.Parameters.AddWithValue("@DonGia", donGia);
                    cmdNhapHang.Parameters.AddWithValue("@ThanhTien", thanhTien);
                    cmdNhapHang.Parameters.AddWithValue("@MaNhaCungCap", cboNhaCungCap.SelectedValue);
                    cmdNhapHang.Parameters.AddWithValue("@MaChiNhanh", maChiNhanh);

                    cmdNhapHang.ExecuteNonQuery();

                    // Cập nhật số lượng tồn trong bảng QuanLyKho
                    string queryUpdateKho = "UPDATE QuanLyKho SET SoLuongTon = SoLuongTon + @SoLuong WHERE MaSanPham = @MaSanPham";
                    SqlCommand cmdUpdateKho = new SqlCommand(queryUpdateKho, conn);

                    cmdUpdateKho.Parameters.AddWithValue("@SoLuong", soLuong);
                    cmdUpdateKho.Parameters.AddWithValue("@MaSanPham", row.Cells["MaSanPham"].Value.ToString());

                    cmdUpdateKho.ExecuteNonQuery();
                }

                // Thông báo sau khi nhập hàng thành công
                MessageBox.Show("Nhập hàng thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
        }
        private void dgvNhapHang_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            // Kiểm tra nếu cột là "Số lượng" hoặc "Đơn giá"
            if (dgvNhapHang.Columns[e.ColumnIndex].Name == "SoLuong" || dgvNhapHang.Columns[e.ColumnIndex].Name == "DonGia")
            {
                // Kiểm tra nếu dòng là dòng cuối cùng (dòng mới)
                if (dgvNhapHang.Rows[e.RowIndex].IsNewRow)
                {
                    // Không thay đổi màu cho dòng cuối cùng
                    return;
                }

                // Kiểm tra nếu ô đang trống
                if (e.Value == null || e.Value == DBNull.Value)
                {
                    // Chuyển màu nền của ô trống thành màu sáng như "LightGray"
                    e.CellStyle.BackColor = Color.Cornsilk;
                }
                else
                {
                    // Nếu ô có giá trị, giữ lại màu nền cyan
                    e.CellStyle.BackColor = Color.Cyan;
                }
            }
        }


        private void dgvNhapHang_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            // Kiểm tra nếu dòng là dòng cuối cùng (dòng mới)
            if (dgvNhapHang.Rows[e.RowIndex].IsNewRow)
            {
                e.Cancel = true;
                MessageBox.Show("Không thể nhập dữ liệu vào dòng này. Hãy điền các thông tin khác trước.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Kiểm tra nếu dòng có mã sản phẩm
            var maSanPham = dgvNhapHang.Rows[e.RowIndex].Cells["MaSanPham"].Value;

            if (maSanPham != null && maSanPham.ToString() != "")
            {
                // Nếu người dùng đang chỉnh sửa ô không phải "Số lượng" hoặc "Đơn giá", hủy thao tác
                if (dgvNhapHang.Columns[e.ColumnIndex].Name != "SoLuong" && dgvNhapHang.Columns[e.ColumnIndex].Name != "DonGia")
                {
                    e.Cancel = true;
                    MessageBox.Show("Chỉ được sửa cột Số lượng và Đơn giá", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
