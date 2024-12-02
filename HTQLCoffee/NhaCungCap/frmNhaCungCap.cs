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

namespace HTQLCoffee.NhaCungCap
{
    public partial class frmNhaCungCap : Form
    {
        string maNhaCungCap = "";
        string connection = ConfigurationManager.ConnectionStrings["HTQLCoffee.Properties.Settings.QLCoffeeConnectionString"].ConnectionString;

        public frmNhaCungCap()
        {
            InitializeComponent();
        }

        private void frmNhaCungCap_Load(object sender, EventArgs e)
        {
            LoadNhaCungCapData();
        }

        private void LoadNhaCungCapData()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connection))
                {
                    conn.Open();
                    string query = "SELECT MaNhaCungCap, TenNhaCungCap, DiaChi, SoDienThoai, MaChiNhanh FROM NhaCungCap";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    dtgNhaCungCap.DataSource = dt;

                    // Đổi header sang tiếng Việt
                    dtgNhaCungCap.Columns["MaNhaCungCap"].HeaderText = "ID";
                    dtgNhaCungCap.Columns["TenNhaCungCap"].HeaderText = "Nơi Cung Cấp";
                    dtgNhaCungCap.Columns["TenNhaCungCap"].Width = 125;
                    dtgNhaCungCap.Columns["DiaChi"].HeaderText = "Địa Chỉ";
                    dtgNhaCungCap.Columns["SoDienThoai"].HeaderText = "Số Điện Thoại";
                    dtgNhaCungCap.Columns["MaChiNhanh"].HeaderText = "Mã Chi Nhánh";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải dữ liệu: " + ex.Message);
            }
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            frmThemNCC frm = new frmThemNCC();
            frm.FormClosed += (s, args) =>
            {
                LoadNhaCungCapData();
            };
            frm.ShowDialog();
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(maNhaCungCap))
            {
                MessageBox.Show("Vui lòng chọn nhà cung cấp cần sửa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Mở form sửa và truyền mã nhà cung cấp vào
            frmSuaNCC frm = new frmSuaNCC(maNhaCungCap);

            frm.FormClosed += (s, args) =>
            {
                LoadNhaCungCapData(); // Gọi lại phương thức load dữ liệu sau khi form sửa đóng
            };

            frm.ShowDialog();
        }


        private void dtgNhaCungCap_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                // Lấy giá trị mã nhà cung cấp từ ô "MaNhaCungCap" của dòng được chọn
                var selectedRow = dtgNhaCungCap.Rows[e.RowIndex];
                if (selectedRow.Cells["MaNhaCungCap"].Value != null)
                {
                    maNhaCungCap = selectedRow.Cells["MaNhaCungCap"].Value.ToString();
                }
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            // Kiểm tra nếu mã nhà cung cấp chưa được chọn
            if (string.IsNullOrEmpty(maNhaCungCap))
            {
                MessageBox.Show("Vui lòng chọn nhà cung cấp cần xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Xác nhận xóa
            var result = MessageBox.Show("Bạn có chắc chắn muốn xóa nhà cung cấp này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                using (SqlConnection conn = new SqlConnection(connection))
                {
                    try
                    {
                        // Mở kết nối
                        conn.Open();

                        // Xóa dữ liệu trong bảng NhapHang liên quan đến mã nhà cung cấp
                        string deleteNhapHangQuery = "DELETE FROM NhapHang WHERE MaNhaCungCap = @MaNhaCungCap";
                        using (SqlCommand cmd = new SqlCommand(deleteNhapHangQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@MaNhaCungCap", maNhaCungCap);
                            cmd.ExecuteNonQuery();
                        }

                        // Xóa nhà cung cấp trong bảng NhaCungCap
                        string deleteNhaCungCapQuery = "DELETE FROM NhaCungCap WHERE MaNhaCungCap = @MaNhaCungCap";
                        using (SqlCommand cmd = new SqlCommand(deleteNhaCungCapQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@MaNhaCungCap", maNhaCungCap);
                            cmd.ExecuteNonQuery();
                        }

                        // Cập nhật lại dữ liệu trong DataGridView
                        LoadNhaCungCapData();

                        MessageBox.Show("Xóa nhà cung cấp thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Có lỗi xảy ra khi xóa nhà cung cấp: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
