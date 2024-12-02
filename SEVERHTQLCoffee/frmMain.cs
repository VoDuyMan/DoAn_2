using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SERVERQLCoffee.DMKH;
using SERVERQLCoffee.QLNV;
using SERVERQLCoffee.QLHD;
using SERVERQLCoffee.QLChiPhi;
using SERVERQLCoffee.ThongKe;
using SERVERQLCoffee.TroGiup;
using Vlc.DotNet.Forms;
using ServerHTQLKaraoke;

namespace SERVERQLCoffee
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            toolStripTrangChu.Visible = true;
            btnQLChiPhi.Visible = false;
            btnThongKe.Visible = false;
            btnHuongDan.Visible = false;
            btnLienHe.Visible = false;
            btnThongKe.Visible = true;
            using (frmDangNhap frmDangNhap = new frmDangNhap())
            {
                if (frmDangNhap.ShowDialog() != DialogResult.OK)
                {
                    Application.Exit();
                }
            }

            
            panel3.BorderStyle = BorderStyle.None;
            panel2.BorderStyle = BorderStyle.None;
           
            button1.FlatStyle = FlatStyle.Flat; 
            button1.FlatAppearance.BorderSize = 0;
            button2.FlatAppearance.BorderSize = 0;
            button2.FlatStyle = FlatStyle.Flat;

            // Đảm bảo không thay đổi màu khi hover
            button1.FlatAppearance.MouseOverBackColor = button1.BackColor; // Không thay đổi màu nền khi hover
            button1.FlatAppearance.MouseDownBackColor = button1.BackColor;

            // Đảm bảo không thay đổi màu khi hover
            button2.FlatAppearance.MouseOverBackColor = button2.BackColor; // Không thay đổi màu nền khi hover
            button2.FlatAppearance.MouseDownBackColor = button2.BackColor;



        }

        private void trangChuToolStripMenuItem_Click(object sender, EventArgs e)
        {
           
            btnQLNV.Visible = true;
            btnQLHoaDon.Visible = true;
            btnQLChiPhi.Visible = false;
            btnThongKe.Visible = true;
            btnHuongDan.Visible = false;
            btnLienHe.Visible = false;
            btnThoat.Visible = true;
        }

        private void heThongToolStripMenuItem1_Click(object sender, EventArgs e)
        {
          
            btnQLNV.Visible = false;
            btnQLHoaDon.Visible = false;
            btnQLChiPhi.Visible = false;
            btnThongKe.Visible = true;
            btnHuongDan.Visible = false;
            btnLienHe.Visible = false;
            btnThoat.Visible = true;
        }

        private void quanTriToolStripMenuItem_Click(object sender, EventArgs e)
        {
           
            btnQLNV.Visible = true;
            btnQLHoaDon.Visible = true;
            btnQLChiPhi.Visible = true;
            btnThongKe.Visible = false;
            btnHuongDan.Visible = false;
            btnLienHe.Visible = false;
            btnThoat.Visible = true;
        }

        private void troGiupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            btnQLNV.Visible = false;
            btnQLHoaDon.Visible = false;
            btnQLChiPhi.Visible = false;
            btnThongKe.Visible = false;
            btnHuongDan.Visible = true;
            btnLienHe.Visible = true;
            btnThoat.Visible = true;

           

        }

        private void btnQLKH_Click(object sender, EventArgs e)
        {
            frmKhachHang frmKhachHang = new frmKhachHang();
            frmKhachHang.ShowDialog();
        }

        private void btnQLNV_Click(object sender, EventArgs e)
        {
            frmQLNV frmQLNV = new frmQLNV();
            frmQLNV.ShowDialog();
        }

        private void btnQLHoaDon_Click(object sender, EventArgs e)
        {
            frmHoaDon frm = new frmHoaDon();
            frm.ShowDialog();
        }

        private void btnQLChiPhi_Click(object sender, EventArgs e)
        {
            frmQLChiPhi frm = new frmQLChiPhi();
            frm.ShowDialog();
        }


        private void btnThongKe_Click(object sender, EventArgs e)
        {
            frmThongKe frm = new frmThongKe();
            frm.ShowDialog();
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Bạn có chắc chắn muốn thoát ứng dụng không?",
                "Xác nhận thoát",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void btnLienHe_Click(object sender, EventArgs e)
        {
            frmLienHe frm = new frmLienHe();
            frm.ShowDialog();
        }

        private void axWindowsMediaPlayer1_Enter(object sender, EventArgs e)
        {
            // Đường dẫn tới video của bạn
            string videoPath = @"\HTQLCoffee\Image\video.mp4";  // Đảm bảo đường dẫn tuyệt đối đúng

            // Kiểm tra nếu video tồn tại
            if (System.IO.File.Exists(videoPath))
            {
                axWindowsMediaPlayer1.URL = videoPath; // Gán video vào Windows Media Player
                axWindowsMediaPlayer1.Ctlcontrols.play(); // Phát video

                // Chuyển sang chế độ toàn màn hình
                axWindowsMediaPlayer1.uiMode = "none"; // Ẩn các điều khiển của Windows Media Player
                this.WindowState = FormWindowState.Maximized; // Tối đa hóa Form
                this.FormBorderStyle = FormBorderStyle.None; // Ẩn border của Form

                // Đảm bảo Windows Media Player chiếm toàn bộ Form
                axWindowsMediaPlayer1.Dock = DockStyle.Fill; // Đảm bảo video sẽ lấp đầy toàn bộ Form

                // Đảm bảo video được kéo giãn để lấp đầy Form mà không bị méo hình
                axWindowsMediaPlayer1.stretchToFit = true; // Lấp đầy video, điều chỉnh theo kích thước control

                // Đăng ký sự kiện PlayStateChange
                axWindowsMediaPlayer1.PlayStateChange += AxWindowsMediaPlayer1_PlayStateChange;
            }
            else
            {
                MessageBox.Show("Video không tồn tại tại đường dẫn được chỉ định!",
                                "Lỗi",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
        }

        // Hàm xử lý sự kiện PlayStateChange
        private void AxWindowsMediaPlayer1_PlayStateChange(object sender, AxWMPLib._WMPOCXEvents_PlayStateChangeEvent e)
        {
            // Kiểm tra trạng thái của trình phát
            if (e.newState == (int)WMPLib.WMPPlayState.wmppsStopped) // Khi video dừng (kết thúc)
            {
                axWindowsMediaPlayer1.Ctlcontrols.play(); // Phát lại video
            }
        }

        private void btnHuongDan_Click(object sender, EventArgs e)
        {
            frmHoTro frm = new frmHoTro();
            frm.ShowDialog();
        }
    }
}
