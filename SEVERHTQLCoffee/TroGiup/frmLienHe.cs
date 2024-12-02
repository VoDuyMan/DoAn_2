using System;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace SERVERQLCoffee.TroGiup
{
    public partial class frmLienHe : Form
    {
        public frmLienHe()
        {
            InitializeComponent();
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            string fullName = txtFullName.Text;
            string email = txtEmail.Text;
            string phone = txtPhone.Text;
            string subject = txtSubject.Text;
            string description = txtDescription.Text;

            // Kiểm tra nếu các trường thông tin không rỗng
            if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(phone) || string.IsNullOrWhiteSpace(subject) ||
                string.IsNullOrWhiteSpace(description))
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin.");
                return;
            }

            // Kiểm tra định dạng email
            if (!IsValidEmail(email))
            {
                MessageBox.Show("Địa chỉ email không hợp lệ.");
                return;
            }

            // Kiểm tra số điện thoại có đầu số Việt Nam
            if (!IsValidPhoneNumber(phone))
            {
                MessageBox.Show("Số điện thoại không hợp lệ. Vui lòng nhập đúng đầu số Việt Nam.");
                return;
            }

            // Xác nhận gửi email
            var result = MessageBox.Show("Bạn có muốn gửi thông tin qua email không?",
                                         "Xác nhận gửi email",
                                         MessageBoxButtons.YesNo,
                                         MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    // Khởi tạo Outlook
                    Outlook.Application outlookApp = new Outlook.Application();
                    Outlook.MailItem mail = outlookApp.CreateItem(Outlook.OlItemType.olMailItem);

                    // Cấu hình email
                    mail.Subject = subject;
                    mail.Body = $@"
                        <html>
                        <body style='font-family:Calibri; font-size:14px;'>
                            <div style='text-align:center;'>
                                <img src='\Image\star_empty.png' alt='Banner' style='max-width:100%; height:auto;' />
                            </div>
                            <p><strong>Họ tên:</strong> {fullName}</p>
                            <p><strong>Email:</strong> {email}</p>
                            <p><strong>Số điện thoại:</strong> {phone}</p>
                            <p><strong>Mô tả vấn đề:</strong></p>
                            <p>{description}</p>
                        </body>
                        </html>";

                    // Đặt định dạng email là HTML
                    mail.HTMLBody = mail.Body;

                    // Đặt người nhận
                    mail.To = "voduyman2111@gmail.com"; // Thay đổi email người nhận

                    // Hiển thị email trong Outlook (để người dùng có thể xác nhận trước khi gửi)
                    mail.Display(true); // Gọi Display() sẽ hiển thị email trước khi gửi

                    // Nếu bạn muốn gửi luôn, thay Display() bằng Send()
                    // mail.Send();

                    MessageBox.Show("Email đã được chuẩn bị trong Outlook.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi mở Outlook: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Yêu cầu chưa được gửi.");
            }
        }

        // Kiểm tra định dạng email bằng biểu thức chính quy
        private bool IsValidEmail(string email)
        {
            var emailRegex = new Regex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");
            return emailRegex.IsMatch(email);
        }

        // Kiểm tra số điện thoại có đầu số Việt Nam (đầu số +84 hoặc 0)
        private bool IsValidPhoneNumber(string phone)
        {
            var phoneRegex = new Regex(@"^(0|\+84)[0-9]{9,10}$");
            return phoneRegex.IsMatch(phone);
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
