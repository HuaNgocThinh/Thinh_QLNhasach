using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using Thinh_QLNhasach.Database;
using Thinh_QLNhasach.Views;

namespace Thinh_QLNhasach
{
    public partial class FrmRegister : Form
    {
        SqlConnection conn;

        public FrmRegister()
        {
            InitializeComponent();

            conn = new SqlConnection(
                @"Data Source=THINHLALUOT\SQLEXPRESS01;
                  Initial Catalog=BookShop;
                  Integrated Security=True");
        }

        // ================= LOAD =================
        private void FrmRegister_Load(object sender, EventArgs e)
        {
            dtpNgaysinh.Value = DateTime.Now;
        }

        // ================= VALIDATE =================
        bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtUser.Text))
            {
                MessageBox.Show("Vui lòng nhập tên đăng nhập!");
                txtUser.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtPass.Text))
            {
                MessageBox.Show("Vui lòng nhập mật khẩu!");
                txtPass.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtHoten.Text))
            {
                MessageBox.Show("Vui lòng nhập họ tên!");
                txtHoten.Focus();
                return false;
            }

            return true;
        }

        // ================= CHECK TRÙNG USER =================
        bool CheckUserExists(string username)
        {
            string sql = "SELECT COUNT(*) FROM NguoiDung WHERE TenDangNhap=@user";

            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@user", username);
                int count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
        }

        // ================= CLEAR FORM =================
        void ClearForm()
        {
            txtUser.Clear();
            txtPass.Clear();
            txtHoten.Clear();
            txtSdt.Clear();

            dtpNgaysinh.Value = DateTime.Now;

            txtUser.Focus();
        }

        // ================= ĐĂNG KÝ =================
        private void btnReg_Click(object sender, EventArgs e)
        {
            // Lấy dữ liệu từ các TextBox
            string user = txtUser.Text.Trim();
            string pass = txtPass.Text.Trim();
            string hoten = txtHoten.Text.Trim();
            string sdt = txtSdt.Text.Trim();
            DateTime ngaysinh = dtpNgaysinh.Value;

            // Validate cơ bản
            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
            {
                MessageBox.Show("Vui lòng nhập đủ tài khoản và mật khẩu!");
                return;
            }

            using (SqlConnection conn = new SqlConnection(DbConnection.connStr))
            {
                try
                {
                    conn.Open();
                    // CHỖ NÀY QUAN TRỌNG NHẤT: Thêm HASHBYTES và CAST mật khẩu
                    string sql = "INSERT INTO NguoiDung (TenDangNhap, MatKhau, HoTen, SoDienThoai, NgaySinh, VaiTro, TrangThai) " +
                                 "VALUES (@u, HASHBYTES('MD5', CAST(@p AS VARCHAR(MAX))), @t, @s, @n, @r, @tt)";

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@u", user);
                    cmd.Parameters.AddWithValue("@p", pass); // Truyền chuỗi '123' bình thường
                    cmd.Parameters.AddWithValue("@t", hoten);
                    cmd.Parameters.AddWithValue("@s", sdt);
                    cmd.Parameters.AddWithValue("@n", ngaysinh);
                    cmd.Parameters.AddWithValue("@r", "Nhân viên"); // Mặc định đăng ký là Nhân viên
                    cmd.Parameters.AddWithValue("@tt", "Còn làm");

                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Đăng ký thành công lả lướt rồi Thịnh ơi!");

                    // Chuyển về trang Login
                    FrmLogin frm = new FrmLogin();
                    frm.Show();
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi đăng ký: " + ex.Message);
                }
            }
        }

        // ================= CHUYỂN LOGIN =================
        private void btnLog_Click(object sender, EventArgs e)
        {
            FrmLogin frm = new FrmLogin();
            frm.Show();
            this.Hide();
        }

        // ================= THOÁT =================
        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}