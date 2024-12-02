using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ServerHTQLKaraoke
{
    public partial class frmHoTro : Form
    {
        public frmHoTro()
        {
            InitializeComponent();
        }

        private void axWindowsMediaPlayer1_Enter(object sender, EventArgs e)
        {
            // Đường dẫn tới video của bạn
            string videoPath = @"\HTQLCoffee\Image\videohuongdan.mp4";  // Đảm bảo đường dẫn tuyệt đối đúng

            // Kiểm tra nếu video tồn tại
            if (System.IO.File.Exists(videoPath))
            {
                axWindowsMediaPlayer1.URL = videoPath; // Gán video vào Windows Media Player
                axWindowsMediaPlayer1.Ctlcontrols.play(); // Phát video

               

                // Đảm bảo video được kéo giãn để lấp đầy Form mà không bị méo hình
                axWindowsMediaPlayer1.stretchToFit = true; // Lấp đầy video, điều chỉnh theo kích thước control

              
            }
            else
            {
                MessageBox.Show("Video không tồn tại tại đường dẫn được chỉ định!",
                                "Lỗi",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
        }
    }
}
