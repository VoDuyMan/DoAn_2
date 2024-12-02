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
    public partial class frmSuaNhapHang : Form
    {
        string connection = ConfigurationManager.ConnectionStrings["HTQLCoffee.Properties.Settings.QLCoffeeConnectionString"].ConnectionString;

        private string maNhapHang;

        public frmSuaNhapHang(string maNhapHang)
        {
            InitializeComponent();
            this.maNhapHang = maNhapHang;
        }

        private void frmSuaNhapHang_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            using (SqlConnection conn = new SqlConnection(connection))
            {
                try
                {
                    conn.Open();

                    // Câu truy vấn lấy thông tin nhập hàng từ cơ sở dữ liệu
                    string query = @"
                    SELECT 
                        nh.MaNhapHang,
                        sp.TenSanPham,
                        nh.SoLuong,
                        nh.DonGia,
                        nc.MaNhaCungCap,
                        nc.TenNhaCungCap
                    FROM 
                        NhapHang nh
                    INNER JOIN 
                        SanPham sp ON nh.MaSanPham = sp.MaSanPham
                    INNER JOIN 
                        NhaCungCap nc ON nh.MaNhaCungCap = nc.MaNhaCungCap
                    WHERE 
                        nh.MaNhapHang = @MaNhapHang";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@MaNhapHang", maNhapHang);

                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        // Lấy dữ liệu từ câu truy vấn và hiển thị lên các điều khiển
                        txtMaNhapHang.Text = reader["MaNhapHang"].ToString();
                        txtTenSanPham.Text = reader["TenSanPham"].ToString();
                        txtSoLuong.Text = reader["SoLuong"].ToString();
                        txtDonGia.Text = reader["DonGia"].ToString().Replace(".00","");
                        txtThanhTien.Text = (Convert.ToInt32(reader["SoLuong"]) * Convert.ToDecimal(reader["DonGia"])).ToString().Replace(".00", ""); ;

                        // Load tên nhà cung cấp vào ComboBox
                        string maNhaCungCap = reader["MaNhaCungCap"].ToString();
                        LoadNhaCungCap(maNhaCungCap);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi tải dữ liệu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void LoadNhaCungCap(string selectedMaNhaCungCap)
        {
            using (SqlConnection conn = new SqlConnection(connection))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT MaNhaCungCap, TenNhaCungCap FROM NhaCungCap";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataReader reader = cmd.ExecuteReader();

                    // Đổ dữ liệu vào ComboBox
                    while (reader.Read())
                    {
                        comboBoxNhaCungCap.Items.Add(new ComboBoxItem
                        {
                            Value = reader["MaNhaCungCap"].ToString(),
                            Text = reader["TenNhaCungCap"].ToString()
                        });
                    }

                    // Chọn nhà cung cấp hiện tại
                    comboBoxNhaCungCap.SelectedItem = comboBoxNhaCungCap.Items
                        .Cast<ComboBoxItem>()
                        .FirstOrDefault(item => item.Value == selectedMaNhaCungCap);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi tải nhà cung cấp: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // ComboBoxItem class để sử dụng trong ComboBox
        public class ComboBoxItem
        {
            public string Value { get; set; }
            public string Text { get; set; }

            public override string ToString()
            {
                return Text;
            }
        }

        private void txtSoLuong_TextChanged(object sender, EventArgs e)
        {
            UpdateThanhTien();
        }

        private void txtDonGia_TextChanged(object sender, EventArgs e)
        {
            UpdateThanhTien();
        }

        private void UpdateThanhTien()
        {
            // Cập nhật thành tiền tự động khi thay đổi số lượng hoặc đơn giá
            int soLuong;
            decimal donGia;

            // Kiểm tra xem số lượng và đơn giá có hợp lệ không
            if (int.TryParse(txtSoLuong.Text, out soLuong) && decimal.TryParse(txtDonGia.Text, out donGia))
            {
                // Kiểm tra điều kiện số lượng phải lớn hơn 0 và nhỏ hơn 100, đơn giá phải dương
                if (soLuong > 0 && soLuong < 100 && donGia > 0)
                {
                    // Tính thành tiền
                    decimal thanhTien = soLuong * donGia;

                    // Đảm bảo định dạng đúng cho tiền (không hiển thị phần thập phân nếu không cần thiết)
                    txtThanhTien.Text = thanhTien.ToString("N0"); // Chỉnh lại định dạng theo yêu cầu (ví dụ: không có phần thập phân)
                }
                else
                {
                    MessageBox.Show("Số lượng phải từ 1 đến 99 và đơn giá phải lớn hơn 0", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                // Nếu giá trị không hợp lệ, đặt lại thành tiền
                txtThanhTien.Clear();
            }
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            // Kiểm tra các điều kiện dữ liệu
            int soLuong;
            decimal donGia;

            // Kiểm tra xem số lượng và đơn giá có hợp lệ không
            bool isSoLuongValid = int.TryParse(txtSoLuong.Text, out soLuong);
            bool isDonGiaValid = decimal.TryParse(txtDonGia.Text, out donGia);

            if (isSoLuongValid && soLuong > 0 && soLuong < 100 && isDonGiaValid && donGia > 0)
            {
                // Cập nhật vào cơ sở dữ liệu
                using (SqlConnection conn = new SqlConnection(connection))
                {
                    try
                    {
                        conn.Open();

                        string query = @"
                    UPDATE NhapHang
                    SET 
                        SoLuong = @SoLuong,
                        DonGia = @DonGia,
                        MaNhaCungCap = @MaNhaCungCap
                    WHERE 
                        MaNhapHang = @MaNhapHang";

                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@MaNhapHang", txtMaNhapHang.Text);
                        cmd.Parameters.AddWithValue("@SoLuong", soLuong);
                        cmd.Parameters.AddWithValue("@DonGia", donGia);
                        cmd.Parameters.AddWithValue("@MaNhaCungCap", ((ComboBoxItem)comboBoxNhaCungCap.SelectedItem).Value);

                        cmd.ExecuteNonQuery();

                        MessageBox.Show("Cập nhật thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi khi cập nhật: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Số lượng và đơn giá không hợp lệ!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
