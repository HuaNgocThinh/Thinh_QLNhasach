using System;
using System.Windows.Forms;
using Thinh_QLNhasach.Controllers;
using Thinh_QLNhasach.Utility;

namespace Thinh_QLNhasach.Views
{
    public partial class FrmLogin : Form
    {
        // Khởi tạo Controller xử lý đăng nhập
        AuthController auth = new AuthController();

        public FrmLogin()
        {
            InitializeComponent();
        }

        // Khi Form hiển thị
        private void FrmLogin_Load(object sender, EventArgs e)
        {
            // Tự động ẩn mật khẩu bằng ký tự hệ thống
            txtPass.UseSystemPasswordChar = true;
        }

        // Sự kiện Click nút Đăng nhập (btnLogin)
        private void btnLogin_Click(object sender, EventArgs e)
        {
            // Vẫn dùng Tên đăng nhập (ví dụ: admin) để login
            string username = txtUser.Text.Trim();
            string password = txtPass.Text.Trim();

            // Kiểm tra nhập liệu cơ bản
            if (string.IsNullOrEmpty(username))
            {
                MessageBox.Show("Vui lòng nhập tên tài khoản!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtUser.Focus();
                return;
            }

            if (string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Vui lòng nhập mật khẩu!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPass.Focus();
                return;
            }

            try
            {
                // Gọi hàm Login từ AuthController
                var user = auth.Login(username, password);

                if (user != null)
                {
                    // FIX Ở ĐÂY: Gán Họ Tên vào Session để Form Index hiển thị cho đẹp
                    // Thay vì user.TenDangNhap, hãy dùng user.HoTen (hoặc tên biến tương ứng trong model của ông)
                    Session.Username = user.HoTen;
                    Session.Role = user.VaiTro;

                    MessageBox.Show("Đăng nhập thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Mở Form chính (FrmIndex)
                    FrmIndex frm = new FrmIndex();
                    frm.Role = user.VaiTro;

                    frm.Show();
                    this.Hide(); // Ẩn form login đi
                }
                else
                {
                    MessageBox.Show("Sai tài khoản hoặc mật khẩu!", "Thất bại", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtPass.Clear();
                    txtPass.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi hệ thống: " + ex.Message, "Lỗi kết nối", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        // Nút Thoát (btnThoat)
        private void btnThoat_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Bạn muốn thoát hệ thống?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        // Nhấn Enter ở ô Password để Đăng nhập luôn cho nhanh
        private void txtPass_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnLog.PerformClick(); // Chú ý: btnLog hay btnLogin tùy theo tên ông đặt
            }
        }

        // Nút Đăng ký (btnReg)
        private void btnReg_Click(object sender, EventArgs e)
        {
            FrmRegister frm = new FrmRegister();
            frm.Show();
            this.Hide();
        }
    }
}