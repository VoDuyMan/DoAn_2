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

namespace HTQLCoffee.QLChiPhi
{
    public partial class frmQLChiPhi : Form
    {
        string connection = ConfigurationManager.ConnectionStrings["HTQLCoffee.Properties.Settings.QLCoffeeConnectionString"].ConnectionString;

        public frmQLChiPhi()
        {
            InitializeComponent();
            InitializeDataGridView();
            LoadMonths();
            LoadAllExpenses();
            btnFilter.Click += BtnFilter_Click;
        }

        private void InitializeDataGridView()
        {
            dataGridViewExpenses.ColumnCount = 5;
            dataGridViewExpenses.Columns[0].Name = "Mã Chi Phí";
            dataGridViewExpenses.Columns[1].Name = "Tên Chi Phí";
            dataGridViewExpenses.Columns[2].Name = "Số Tiền";
            dataGridViewExpenses.Columns[3].Name = "Ngày Chi";
            dataGridViewExpenses.Columns[4].Name = "Ghi Chú";
            dataGridViewExpenses.Columns[4].Width = 230;
        }

        private void LoadMonths()
        {
            // Tải danh sách tháng vào ComboBox
            for (int i = 1; i <= 12; i++)
            {
                comboBoxMonths.Items.Add(i.ToString());
            }
            comboBoxMonths.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        private void LoadAllExpenses()
        {
            dataGridViewExpenses.Rows.Clear();

            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();
                string query = "SELECT MaChiPhi, TenChiPhi, SoTien, NgayChi, GhiChu FROM ChiPhiKhac"; // Lấy toàn bộ dữ liệu
                SqlCommand cmd = new SqlCommand(query, conn);

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    dataGridViewExpenses.Rows.Add(
                        reader["MaChiPhi"].ToString(),
                        reader["TenChiPhi"].ToString(),
                        reader["SoTien"].ToString().Replace(".00", ""),
                        Convert.ToDateTime(reader["NgayChi"]).ToShortDateString(),
                        reader["GhiChu"].ToString()
                    );
                }
            }
        }

        private void BtnFilter_Click(object sender, EventArgs e)
        {
            if (comboBoxMonths.SelectedItem != null)
            {
                int selectedMonth = int.Parse(comboBoxMonths.SelectedItem.ToString());
                LoadExpenses(selectedMonth);
            }
            else
            {
                MessageBox.Show("Vui lòng chọn tháng để lọc!");
            }
        }

        private void LoadExpenses(int month)
        {
            dataGridViewExpenses.Rows.Clear();

            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();
                string query = "SELECT MaChiPhi, TenChiPhi, SoTien, NgayChi, GhiChu FROM ChiPhiKhac WHERE MONTH(NgayChi) = @Month";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Month", month);

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    dataGridViewExpenses.Rows.Add(
                        reader["MaChiPhi"].ToString(),
                        reader["TenChiPhi"].ToString(),
                        reader["SoTien"].ToString(),
                        Convert.ToDateTime(reader["NgayChi"]).ToShortDateString(),
                        reader["GhiChu"].ToString()
                    );
                }
            }
        }

        private void btnThemPhong_Click(object sender, EventArgs e)
        {
            frmThemChiPhi frm = new frmThemChiPhi();
            frm.FormClosed += (s, args) =>
            {
                LoadAllExpenses();
            };

            frm.ShowDialog();
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            // Kiểm tra xem có dòng nào được chọn trong DataGridView không
            if (dataGridViewExpenses.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn chi phí cần xóa!");
                return;
            }

            // Lấy mã chi phí của dòng được chọn
            string tenChiPhi = dataGridViewExpenses.SelectedRows[0].Cells["Tên Chi Phí"].Value.ToString();
            string maChiPhi = dataGridViewExpenses.SelectedRows[0].Cells["Mã Chi Phí"].Value.ToString();


            // Hỏi người dùng có chắc chắn muốn xóa không
            var result = MessageBox.Show("Bạn có chắc chắn muốn xóa chi phí " + tenChiPhi + " không?",
                                           "Xác Nhận Xóa",
                                           MessageBoxButtons.YesNo,
                                           MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                // Kết nối đến cơ sở dữ liệu và xóa chi phí
                using (SqlConnection conn = new SqlConnection(connection))
                {
                    conn.Open();
                    string query = "DELETE FROM ChiPhiKhac WHERE MaChiPhi = @MaChiPhi";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@MaChiPhi", maChiPhi);
                    cmd.ExecuteNonQuery();
                }

                // Xóa dòng khỏi DataGridView
                dataGridViewExpenses.Rows.RemoveAt(dataGridViewExpenses.SelectedRows[0].Index);

                MessageBox.Show("Xóa chi phí thành công!");
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (dataGridViewExpenses.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn chi phí cần sửa!");
                return;
            }

            var selectedRow = dataGridViewExpenses.SelectedRows[0];

            if (selectedRow.Cells["Mã Chi Phí"].Value == null || string.IsNullOrWhiteSpace(selectedRow.Cells["Mã Chi Phí"].Value.ToString()))
            {
                MessageBox.Show("Vui lòng chọn chi phí cần sửa, không chọn dòng trống!");
                return;
            }
            string maChiPhi = selectedRow.Cells["Mã Chi Phí"].Value.ToString();
            frmSuaChiPhi frm = new frmSuaChiPhi(maChiPhi);
            frm.FormClosed += (s, args) =>
            {
                LoadAllExpenses();
            };

            frm.ShowDialog();
        }
    }
}
