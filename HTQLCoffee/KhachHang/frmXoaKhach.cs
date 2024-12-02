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

namespace HTQLCoffee.KhachHang
{
    public partial class frmXoaKhach : Form
    {
        string connection = ConfigurationManager.ConnectionStrings["HTQLCoffee.Properties.Settings.QLCoffeeConnectionString"].ConnectionString;


        private string customerId; // Mã khách hàng
        public frmXoaKhach(string id)
        {
            InitializeComponent();
            SetupComboBox();
            customerId = id;
            LoadCustomerData();

            toolTipbtn.SetToolTip(this.btnXoa, "Xóa Thông Tin");
            toolTipbtn.SetToolTip(this.btnHuy, "Thoát Trang");
        }

        private void SetupComboBox()
        {
            // Cài đặt lựa chọn cho ComboBox Giới tính
            cbxGioiTinh.Items.AddRange(new string[] { "Nam", "Nữ", "Khác" });
            cbxGioiTinh.SelectedIndex = 0; // Mặc định chọn Nam
        }

        private void LoadCustomerData()
        {
            // Kết nối đến cơ sở dữ liệu và lấy thông tin khách hàng theo customerId
            string query = "SELECT * FROM KhachHang WHERE MaKhachHang = @Id";

            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", customerId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Gán giá trị vào các TextBox
                            txtHoTen.Text = reader["HoTen"].ToString();
                            txtDiaChi.Text = reader["DiaChi"].ToString();
                            txtEmail.Text = reader["Email"].ToString();
                            txtSDT.Text = reader["SoDienThoai"].ToString();
                            dtpNgaySinh.Value = reader["NgaySinh"] != DBNull.Value ? Convert.ToDateTime(reader["NgaySinh"]) : DateTime.Now;

                            // Kiểm tra và gán giá trị cho ComboBox
                            cbxGioiTinh.Text = reader["GioiTinh"].ToString();
                            cbxGioiTinh.SelectedItem = reader["GioiTinh"].ToString();
                        }
                    }
                }
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {

            try
            {
                using (SqlConnection conn = new SqlConnection(connection))
                {
                    conn.Open();

                    // Kiểm tra xem khách hàng có đặt phòng với trạng thái "Chưa thanh toán" hay không
                    string checkUnpaidBookingQuery = @"
                SELECT COUNT(*) 
                FROM DatBan 
                WHERE MaKhachHang = @MaKhachHang 
                AND TinhTrang = N'Chưa thanh toán'";

                    using (SqlCommand cmd = new SqlCommand(checkUnpaidBookingQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@MaKhachHang", customerId);
                        int unpaidBookingCount = (int)cmd.ExecuteScalar();

                        if (unpaidBookingCount > 0)
                        {
                            MessageBox.Show("Khách hàng này đang trong quá trình đặt phòng (chưa thanh toán), không thể xóa khách hàng.",
                                "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }

                    // Kiểm tra dữ liệu liên quan trong các bảng khác
                    string[] tables = { "HoaDon",  "DatBan" };
                    List<string> affectedTables = new List<string>();

                    foreach (string table in tables)
                    {
                        string checkQuery = "SELECT COUNT(*) FROM " + table + " WHERE MaKhachHang = @MaKhachHang";
                        using (SqlCommand cmd = new SqlCommand(checkQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@MaKhachHang", customerId);
                            int count = (int)cmd.ExecuteScalar();

                            if (count > 0)
                            {
                                affectedTables.Add(table);
                            }
                        }
                    }

                    // Hiển thị thông báo nếu có dữ liệu liên quan
                    if (affectedTables.Count > 0)
                    {
                        string message = "Khách hàng này có dữ liệu liên quan trong các bảng sau:\n" +
                                         string.Join("\n", affectedTables) +
                                         "\n\nToàn bộ dữ liệu trong các bảng này sẽ bị xóa.\nBạn có chắc chắn muốn tiếp tục?";
                        DialogResult result = MessageBox.Show(message, "Xác nhận xóa", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);

                        if (result != DialogResult.OK)
                        {
                            return; // Hủy xóa nếu người dùng chọn Cancel
                        }
                    }

                    // Xóa dữ liệu khách hàng
                    PerformDeleteCustomer(customerId);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi kiểm tra dữ liệu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            this.Close();
        }

        private void PerformDeleteCustomer(string maKhachHang)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connection))
                {
                    conn.Open();

                    using (SqlTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            // Xóa dữ liệu trong các bảng phụ
                            string[] deleteQueries =
                            {
                                "DELETE FROM HoaDon WHERE MaKhachHang = @MaKhachHang",
                                "DELETE FROM DatBan WHERE MaKhachHang = @MaKhachHang"
                            };

                            foreach (string query in deleteQueries)
                            {
                                ExecuteDeleteQuery(query, maKhachHang, conn, transaction);
                            }

                            // Xóa khách hàng khỏi bảng KhachHang
                            string deleteCustomerQuery = "DELETE FROM KhachHang WHERE MaKhachHang = @MaKhachHang";
                            using (SqlCommand cmd = new SqlCommand(deleteCustomerQuery, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@MaKhachHang", maKhachHang);
                                cmd.ExecuteNonQuery();
                            }

                            // Xác nhận giao dịch
                            transaction.Commit();
                            MessageBox.Show("Xóa khách hàng và các dữ liệu liên quan thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // Cập nhật lại giao diện sau khi xóa
                            LoadCustomerData();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            MessageBox.Show("Lỗi khi xóa dữ liệu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thực hiện xóa: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Hàm thực thi câu lệnh DELETE
        private void ExecuteDeleteQuery(string query, string maKhachHang, SqlConnection conn, SqlTransaction transaction)
        {
            using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
            {
                cmd.Parameters.AddWithValue("@MaKhachHang", maKhachHang);
                cmd.ExecuteNonQuery();
            }
        }


        private void btnHuy_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmXoaKhach_Load(object sender, EventArgs e)
        {

        }
    }
}
