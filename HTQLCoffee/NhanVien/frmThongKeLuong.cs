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
    public partial class frmThongKeLuong : Form
    {
        string connection = ConfigurationManager.ConnectionStrings["HTQLCoffee.Properties.Settings.QLCoffeeConnectionString"].ConnectionString;

        public frmThongKeLuong()
        {
            InitializeComponent();
        }
        private void LoadLuongThang(int thang)
        {

            string query = @"
        SELECT  lsnl.MaNhanVien, nv.HoTen, lsnl.TongGioLam, lsnl.TongLuong, lsnl.TrangThaiNhanLuong
        FROM LichSuNhanLuong lsnl
        INNER JOIN NhanVien nv ON lsnl.MaNhanVien = nv.MaNhanVien
        WHERE lsnl.Thang = @Thang";

            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Thang", thang);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);


                dgvLuongNhanVien.DataSource = dt;


                dgvLuongNhanVien.Columns["MaNhanVien"].HeaderText = "Mã Nhân Viên";
                dgvLuongNhanVien.Columns["HoTen"].HeaderText = "Tên Nhân Viên";
                dgvLuongNhanVien.Columns["TongGioLam"].HeaderText = "Tổng Giờ Làm";
                dgvLuongNhanVien.Columns["TongLuong"].HeaderText = "Tổng Lương";
                dgvLuongNhanVien.Columns["TrangThaiNhanLuong"].HeaderText = "Trạng Thái Nhận Lương";


                dgvLuongNhanVien.Columns["HoTen"].Width = 120;
                dgvLuongNhanVien.Columns["MaNhanVien"].Width = 95;
                dgvLuongNhanVien.Columns["TongGioLam"].Width = 95;
                dgvLuongNhanVien.Columns["TongLuong"].Width = 85;
                dgvLuongNhanVien.Columns["TrangThaiNhanLuong"].Width = 140;


                decimal tongLuong = 0;
                foreach (DataRow row in dt.Rows)
                {
                    tongLuong += Convert.ToDecimal(row["TongLuong"]);
                }


                txtTongLuong.Text = tongLuong.ToString("N0") + " VND";
            }
        }


        private void cbThang_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Lấy tháng được chọn từ ComboBox
            int thang = Convert.ToInt32(cbThang.SelectedItem);

            // Gọi phương thức để tải dữ liệu
            LoadLuongThang(thang);
        }

        private void frmThongKeLuong_Load(object sender, EventArgs e)
        {
            // Thêm các tháng 1 - 12 vào ComboBox
            for (int i = 1; i <= 12; i++)
            {
                cbThang.Items.Add(i);
            }

            // Thiết lập tháng mặc định là tháng 1
            cbThang.SelectedIndex = 0;
            cbThang.DropDownStyle = ComboBoxStyle.DropDownList;

            // Gọi phương thức để hiển thị lương của tháng mặc định
            LoadLuongThang(1);
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        
    }
}
