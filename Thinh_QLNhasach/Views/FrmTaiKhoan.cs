using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Thinh_QLNhasach.Views
{
    public partial class FrmTaiKhoan : Form
    {
        // Sử dụng chuỗi kết nối từ class DbConnection của đại ca
        string strConn = Thinh_QLNhasach.Database.DbConnection.connStr;

        // Biến lưu tên tài khoản đang đăng nhập để biết đổi pass cho ai
        string tenDangNhapHienTai = "";

        // HÀM KHỞI TẠO ĐÃ ĐƯỢC ĐỔI TÊN CHUẨN XÁC
        public FrmTaiKhoan(string username)
        {
            InitializeComponent();
            tenDangNhapHienTai = username;
        }

        // ==============================================================
        // SỰ KIỆN KHI BẤM NÚT LƯU (CẬP NHẬT MẬT KHẨU)
        // ==============================================================
        private void btnLuu_Click(object sender, EventArgs e)
        {
            string mkCu = txtMatKhauCu.Text.Trim();
            string mkMoi = txtMatKhauMoi.Text.Trim();
            string xacNhan = txtXacNhan.Text.Trim();

            // 1. Kiểm tra rỗng
            if (string.IsNullOrEmpty(mkCu) || string.IsNullOrEmpty(mkMoi) || string.IsNullOrEmpty(xacNhan))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin vào các ô!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. Kiểm tra khớp mật khẩu
            if (mkMoi != xacNhan)
            {
                MessageBox.Show("Mật khẩu xác nhận không khớp. Vui lòng gõ lại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtXacNhan.Focus();
                return;
            }

            // 3. Kết nối Database
            using (SqlConnection conn = new SqlConnection(strConn))
            {
                try
                {
                    if (conn.State == ConnectionState.Closed) conn.Open();

                    // A. ĐÃ SỬA: Đổi 'TaiKhoan' thành 'NguoiDung'
                    // ĐÃ THÊM: Dùng HASHBYTES('MD5', ...) để băm mật khẩu gõ vào cho khớp với DB
                    string checkSql = "SELECT COUNT(*) FROM NguoiDung WHERE TenDangNhap = @tk AND MatKhau = HASHBYTES('MD5', CAST(@mkCu AS VARCHAR(100)))";
                    SqlCommand cmdCheck = new SqlCommand(checkSql, conn);
                    cmdCheck.Parameters.AddWithValue("@tk", tenDangNhapHienTai);
                    cmdCheck.Parameters.AddWithValue("@mkCu", mkCu);

                    int count = (int)cmdCheck.ExecuteScalar();

                    if (count > 0)
                    {
                        // B. ĐÃ SỬA: Update mật khẩu mới cũng phải băm sang MD5
                        string updateSql = "UPDATE NguoiDung SET MatKhau = HASHBYTES('MD5', CAST(@mkMoi AS VARCHAR(100))) WHERE TenDangNhap = @tk";
                        SqlCommand cmdUpdate = new SqlCommand(updateSql, conn);
                        cmdUpdate.Parameters.AddWithValue("@mkMoi", mkMoi);
                        cmdUpdate.Parameters.AddWithValue("@tk", tenDangNhapHienTai);

                        cmdUpdate.ExecuteNonQuery();

                        MessageBox.Show("Đổi mật khẩu thành công! Tuyệt vời đại ca!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.Close(); // Đổi xong tự đóng form
                    }
                    else
                    {
                        MessageBox.Show("Mật khẩu cũ không chính xác!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtMatKhauCu.Focus();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi kết nối cơ sở dữ liệu: " + ex.Message, "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // ==============================================================
        // SỰ KIỆN KHI BẤM NÚT HỦY
        // ==============================================================
        private void btnHuy_Click(object sender, EventArgs e)
        {
            // Hỏi nhẹ một câu trước khi đóng cho chắc cốp
            DialogResult rs = MessageBox.Show("Ông có chắc muốn hủy bỏ việc đổi mật khẩu không?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (rs == DialogResult.Yes)
            {
                this.Close();
            }
        }
    }
}