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
    public partial class frmLichSuNhanLuong : Form
    {
        string connection = ConfigurationManager.ConnectionStrings["HTQLCoffee.Properties.Settings.QLCoffeeConnectionString"].ConnectionString;

        private string maNhanVien;
        public frmLichSuNhanLuong(string maNhanVien)
        {
            InitializeComponent();
            this.maNhanVien = maNhanVien;
        }

        private void frmLichSuNhanLuong_Load(object sender, EventArgs e)
        {
            for (int i = 1; i <= 12; i++)
            {
                cbxThang.Items.Add(i);
            }
            cbxThang.SelectedIndex = 0;
            cbxThang.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        private void cbxThang_SelectedIndexChanged(object sender, EventArgs e)
        {
            int thang = Convert.ToInt32(cbxThang.SelectedItem);
            int nam = DateTime.Now.Year;

            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();

                string query = @"SELECT TongGioLam, TongLuong, TrangThaiNhanLuong 
                                     FROM LichSuNhanLuong 
                                     WHERE MaNhanVien = @MaNhanVien AND Thang = @Thang AND Nam = @Nam";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@MaNhanVien", maNhanVien);
                cmd.Parameters.AddWithValue("@Thang", thang);
                cmd.Parameters.AddWithValue("@Nam", nam);

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    // Hiển thị thông tin lương đã nhận
                    decimal tongGioLam = reader.GetDecimal(0);
                    decimal tongLuong = reader.GetDecimal(1);
                    string trangThai = reader.GetBoolean(2) ? "Đã nhận" : "Chưa nhận"; // Trang thái nhận lương

                    txtTongGioLam.Text = tongGioLam.ToString();
                    txtTongLuong.Text = tongLuong.ToString();
                    txtTrangThai.Text = trangThai.ToString();
                }
                else
                {
                    txtTrangThai.Text = "Chưa nhận lương";
                    txtTongGioLam.Text = "";
                    txtTongLuong.Text = "";
                }
            }
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        
    }
}
