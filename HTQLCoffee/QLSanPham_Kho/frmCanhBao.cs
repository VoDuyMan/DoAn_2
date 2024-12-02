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
using HTQLCoffee.NhapHang;

namespace HTQLCoffee.QLSanPham_Kho
{
    public partial class frmCanhBao : Form
    {
        string connection = ConfigurationManager.ConnectionStrings["HTQLCoffee.Properties.Settings.QLCoffeeConnectionString"].ConnectionString;

        private string selectedProductCode;
        public frmCanhBao()
        {
            InitializeComponent();
        }

        private void frmCanhBao_Load(object sender, EventArgs e)
        {
            int mucCanhBao = 10;

            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();
                string query = @"
            SELECT sp.MaSanPham, sp.TenSanPham, qlk.SoLuongTon
            FROM QuanLyKho qlk
            JOIN SanPham sp ON qlk.MaSanPham = sp.MaSanPham
            WHERE qlk.SoLuongTon < @MucCanhBao";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@MucCanhBao", mucCanhBao);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    // Hiển thị kết quả lên DataGridView (dgvCanhBao)
                    dtgCanhBao.DataSource = dataTable;
                    dtgCanhBao.Columns["MaSanPham"].HeaderText = "Mã sản phẩm";
                    dtgCanhBao.Columns["TenSanPham"].HeaderText = "Tên sản phẩm";
                    dtgCanhBao.Columns["SoLuongTon"].HeaderText = "Số lượng tồn";
                }
            }
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnNhapHang_Click(object sender, EventArgs e)
        {
            // Kiểm tra nếu đã chọn mã sản phẩm
            if (string.IsNullOrEmpty(selectedProductCode))
            {
                MessageBox.Show("Không có sản phẩm được chọn!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Tạo danh sách mã sản phẩm và truyền vào form Nhập Hàng
            List<string> selectedProductCodes = new List<string> { selectedProductCode };
            frmNhapHang frm = new frmNhapHang(selectedProductCodes);
            frm.ShowDialog();
        }


        private void dtgCanhBao_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Kiểm tra nếu cột "MaSanPham" có dữ liệu và không phải là header row
            if (e.RowIndex >= 0)
            {
                // Lấy mã sản phẩm từ cột "MaSanPham" ở dòng được chọn
                selectedProductCode = dtgCanhBao.Rows[e.RowIndex].Cells["MaSanPham"].Value.ToString();
            }
        }

        
    }
}
