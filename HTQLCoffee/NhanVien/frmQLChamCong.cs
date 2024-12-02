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

namespace HTQLCoffee.NhanVien
{
    public partial class frmQLChamCong : Form
    {
        string connection = ConfigurationManager.ConnectionStrings["HTQLCoffee.Properties.Settings.QLCoffeeConnectionString"].ConnectionString;

        string maNhanVien;
        string tenNhanVien;

        public frmQLChamCong(string maNhanVien, string tenNhanVien)
        {
            InitializeComponent();
            this.maNhanVien = maNhanVien;
            this.tenNhanVien = tenNhanVien;
            LoadChamCongData();
        }

        private void LoadChamCongData()
        {
            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();
                string query = "SELECT MaDiemDanh, ThoiGianDiLam, ThoiGianVeLam FROM DiemDanh WHERE MaNhanVien = @MaNhanVien AND NgayDiemDanh = @NgayDiemDanh";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@MaNhanVien", maNhanVien);
                    cmd.Parameters.AddWithValue("@NgayDiemDanh", DateTime.Now.Date);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            txtMaDiemDanh.Text = reader["MaDiemDanh"].ToString();
                            txtThoiGianDiLam.Text = reader["ThoiGianDiLam"] != DBNull.Value ? Convert.ToDateTime(reader["ThoiGianDiLam"]).ToString(@"HH:mm:ss") : string.Empty;
                            txtThoiGianVeLam.Text = reader["ThoiGianVeLam"] != DBNull.Value ? Convert.ToDateTime(reader["ThoiGianVeLam"]).ToString(@"HH:mm:ss") : string.Empty;

                            if (reader["ThoiGianDiLam"] != DBNull.Value && reader["ThoiGianVeLam"] != DBNull.Value)
                            {
                                DateTime thoiGianDiLam = Convert.ToDateTime(reader["ThoiGianDiLam"]);
                                DateTime thoiGianVeLam = Convert.ToDateTime(reader["ThoiGianVeLam"]);
                                double tongGioLam = (thoiGianVeLam - thoiGianDiLam).TotalHours;
                                txtTongGioLam.Text = tongGioLam.ToString("F2");
                            }
                        }
                    }
                }

                // Tính tổng giờ làm cho toàn bộ ngày
                string totalHoursQuery = "SELECT SUM(DATEDIFF(MINUTE, ThoiGianDiLam, ThoiGianVeLam)) AS TongGio FROM DiemDanh WHERE MaNhanVien = @MaNhanVien AND NgayDiemDanh = @NgayDiemDanh AND ThoiGianVeLam IS NOT NULL";
                using (SqlCommand totalHoursCmd = new SqlCommand(totalHoursQuery, conn))
                {
                    totalHoursCmd.Parameters.AddWithValue("@MaNhanVien", maNhanVien);
                    totalHoursCmd.Parameters.AddWithValue("@NgayDiemDanh", DateTime.Now.Date);

                    object result = totalHoursCmd.ExecuteScalar();
                    if (result != DBNull.Value)
                    {
                        double totalMinutes = Convert.ToDouble(result);
                        double totalHours = totalMinutes / 60.0;
                        txtTongGioLam.Text = totalHours.ToString("F2");
                    }
                    else
                    {
                        txtTongGioLam.Text = "0.00";
                    }
                }

                // Tạo mã điểm danh mới
                txtMaDiemDanh.Text = GenerateUniqueMaDiemDanh(conn);
            }

            txtMaNhanVien.Text = maNhanVien;
            txtTenNhanVien.Text = tenNhanVien;
            txtNgayDiemDanh.Text = DateTime.Now.ToShortDateString();
            groupboxChamCong.Text = "Chấm Công Nhân Viên: " + tenNhanVien;
            groupboxChamCong.Visible = true;
        }

        private string GenerateUniqueMaDiemDanh(SqlConnection conn)
        {
            string newMaDiemDanh;
            do
            {
                // Tạo mã ngẫu nhiên
                newMaDiemDanh = GenerateRandomString(10);
            }
            while (DoesMaDiemDanhExist(newMaDiemDanh, conn)); // Kiểm tra mã đã tồn tại trong cơ sở dữ liệu
            return newMaDiemDanh;
        }

        private string GenerateRandomString(int length)
        {
            // Bao gồm cả ký tự đặc biệt như yêu cầu
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789()";
            StringBuilder result = new StringBuilder(length);
            Random random = new Random();

            for (int i = 0; i < length; i++)
            {
                // Chọn ngẫu nhiên một ký tự từ chuỗi `chars`
                result.Append(chars[random.Next(chars.Length)]);
            }
            return result.ToString();
        }

        private bool DoesMaDiemDanhExist(string maDiemDanh, SqlConnection conn)
        {
            string checkQuery = "SELECT COUNT(*) FROM DiemDanh WHERE MaDiemDanh = @MaDiemDanh";
            using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
            {
                checkCmd.Parameters.AddWithValue("@MaDiemDanh", maDiemDanh);
                int count = (int)checkCmd.ExecuteScalar();
                return count > 0;
            }
        }

        private void btnDiemDanh_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();

                string checkQuery = "SELECT COUNT(*) FROM DiemDanh WHERE MaNhanVien = @MaNhanVien AND NgayDiemDanh = @NgayDiemDanh AND ThoiGianVeLam IS NULL";
                using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                {
                    checkCmd.Parameters.AddWithValue("@MaNhanVien", txtMaNhanVien.Text);
                    checkCmd.Parameters.AddWithValue("@NgayDiemDanh", DateTime.Now.Date);

                    int count = (int)checkCmd.ExecuteScalar();
                    if (count > 0)
                    {
                        MessageBox.Show("Nhân viên đã điểm danh trong ca làm. Vui lòng kết thúc ca làm trước khi điểm danh lại.");
                        return;
                    }
                }

                txtThoiGianDiLam.Text = DateTime.Now.ToString(@"HH:mm:ss");

                string query = "INSERT INTO DiemDanh (MaDiemDanh, MaNhanVien, NgayDiemDanh, ThoiGianDiLam) VALUES (@MaDiemDanh, @MaNhanVien, @NgayDiemDanh, @ThoiGianDiLam)";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@MaDiemDanh", txtMaDiemDanh.Text);
                    cmd.Parameters.AddWithValue("@MaNhanVien", txtMaNhanVien.Text);
                    cmd.Parameters.AddWithValue("@NgayDiemDanh", DateTime.Now.Date);
                    cmd.Parameters.AddWithValue("@ThoiGianDiLam", DateTime.Parse(txtThoiGianDiLam.Text));

                    if (cmd.ExecuteNonQuery() > 0)
                    {
                        MessageBox.Show("Điểm danh thành công!");
                        LoadChamCongData(); // Cập nhật lại dữ liệu chấm công
                    }
                    else
                    {
                        MessageBox.Show("Có lỗi xảy ra trong quá trình điểm danh. Vui lòng thử lại.");
                    }
                }
            }
        }

        private void btnTanLam_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtMaNhanVien.Text))
            {
                MessageBox.Show("Vui lòng chọn một nhân viên để kết thúc điểm danh!");
                return;
            }

            txtThoiGianVeLam.Text = DateTime.Now.ToString(@"HH:mm:ss");

            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();

                string query = "UPDATE DiemDanh SET ThoiGianVeLam = @ThoiGianVeLam WHERE MaNhanVien = @MaNhanVien AND NgayDiemDanh = @NgayDiemDanh AND ThoiGianVeLam IS NULL";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ThoiGianVeLam", DateTime.Parse(txtThoiGianVeLam.Text));
                    cmd.Parameters.AddWithValue("@MaNhanVien", txtMaNhanVien.Text);
                    cmd.Parameters.AddWithValue("@NgayDiemDanh", DateTime.Now.Date);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Cập nhật thời gian về làm thành công!");
                    }
                    else
                    {
                        MessageBox.Show("Nhân viên chưa bắt đầu điểm danh, không thể kết thúc điểm danh!");
                    }
                }
            }
            LoadChamCongData();
        }
        private void frmQLChamCong_Load(object sender, EventArgs e)
        {

        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close(); 
        }

        
    }
}
