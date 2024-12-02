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

namespace SERVERQLCoffee.QLChiPhi
{
    public partial class frmQLChiPhi : Form
    {
        string connection = ConfigurationManager.ConnectionStrings["SERVERQLCoffee.Properties.Settings.QLCoffeeConnectionString"].ConnectionString;
        public frmQLChiPhi()
        {
            InitializeComponent();
            InitializeDataGridView();
            LoadMonths();
            LoadBranches();
            LoadAllExpenses();
            btnFilter.Click += BtnFilter_Click;
        }

        private void InitializeDataGridView()
        {
            dataGridViewExpenses.ColumnCount = 6;
            dataGridViewExpenses.Columns[0].Name = "Mã Chi Phí";
            dataGridViewExpenses.Columns[1].Name = "Tên Chi Phí";
            dataGridViewExpenses.Columns[2].Name = "Số Tiền";
            dataGridViewExpenses.Columns[3].Name = "Ngày Chi";
            dataGridViewExpenses.Columns[4].Name = "Tên Chi Nhánh"; // Hiển thị tên chi nhánh
            dataGridViewExpenses.Columns[5].Name = "Ghi Chú";
            dataGridViewExpenses.Columns[5].Width = 230;
        }

        private void LoadMonths()
        {
            for (int i = 1; i <= 12; i++)
            {
                comboBoxMonths.Items.Add(i.ToString());
            }
            comboBoxMonths.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        private void LoadBranches()
        {
            cbxChiNhanh.Items.Clear();
            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();
                string query = "SELECT MaChiNhanh, TenChiNhanh FROM ChiNhanh"; // Lấy danh sách chi nhánh
                SqlCommand cmd = new SqlCommand(query, conn);

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    cbxChiNhanh.Items.Add(new KeyValuePair<string, string>(
                        reader["MaChiNhanh"].ToString(),
                        reader["TenChiNhanh"].ToString()
                    ));
                }
            }

            cbxChiNhanh.DisplayMember = "Value"; // Hiển thị tên chi nhánh
            cbxChiNhanh.ValueMember = "Key"; // Lưu mã chi nhánh
            cbxChiNhanh.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        private void LoadAllExpenses()
        {
            dataGridViewExpenses.Rows.Clear();
            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();
                string query = @"
                    SELECT cp.MaChiPhi, cp.TenChiPhi, cp.SoTien, cp.NgayChi, cn.TenChiNhanh, cp.GhiChu 
                    FROM ChiPhiKhac cp
                    LEFT JOIN ChiNhanh cn ON cp.MaChiNhanh = cn.MaChiNhanh"; // Kết hợp với bảng ChiNhanh

                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    dataGridViewExpenses.Rows.Add(
                        reader["MaChiPhi"].ToString(),
                        reader["TenChiPhi"].ToString(),
                        reader["SoTien"].ToString(),
                        Convert.ToDateTime(reader["NgayChi"]).ToShortDateString(),
                        reader["TenChiNhanh"].ToString(),
                        reader["GhiChu"].ToString()
                    );
                }
            }
        }

        private void BtnFilter_Click(object sender, EventArgs e)
        {
            int? selectedMonth = comboBoxMonths.SelectedItem != null ? int.Parse(comboBoxMonths.SelectedItem.ToString()) : (int?)null;
            string selectedBranchId = cbxChiNhanh.SelectedItem != null ? ((KeyValuePair<string, string>)cbxChiNhanh.SelectedItem).Key : null;

            LoadFilteredExpenses(selectedMonth, selectedBranchId);
        }

        private void LoadFilteredExpenses(int? month, string branchId)
        {
            dataGridViewExpenses.Rows.Clear();

            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();
                var query = @"
                    SELECT cp.MaChiPhi, cp.TenChiPhi, cp.SoTien, cp.NgayChi, cn.TenChiNhanh, cp.GhiChu 
                    FROM ChiPhiKhac cp
                    LEFT JOIN ChiNhanh cn ON cp.MaChiNhanh = cn.MaChiNhanh
                    WHERE (@Month IS NULL OR MONTH(cp.NgayChi) = @Month)
                      AND (@BranchId IS NULL OR cp.MaChiNhanh = @BranchId)";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Month", month.HasValue ? (object)month.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@BranchId", !string.IsNullOrEmpty(branchId) ? (object)branchId : DBNull.Value);

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    dataGridViewExpenses.Rows.Add(
                        reader["MaChiPhi"].ToString(),
                        reader["TenChiPhi"].ToString(),
                        reader["SoTien"].ToString(),
                        Convert.ToDateTime(reader["NgayChi"]).ToShortDateString(),
                        reader["TenChiNhanh"].ToString(),
                        reader["GhiChu"].ToString()
                    );
                }
            }
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadAllExpenses();
        }
    }
}
