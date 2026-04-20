using System;

namespace Thinh_QLNhasach.Models
{
    public class NguoiDung
    {
        // Mã người dùng (Primary Key)
        public int MaND { get; set; }

        // Tên đăng nhập
        public string TenDangNhap { get; set; }

        // Mật khẩu (Trong DB là VARBINARY nên để object hoặc byte[] cho an toàn)
        public object MatKhau { get; set; }

        // Họ và tên (CÁI NÀY QUAN TRỌNG, THIẾU LÀ LỖI ĐĂNG NHẬP)
        public string HoTen { get; set; }

        // Ngày sinh
        public DateTime? NgaySinh { get; set; }

        // Số điện thoại
        public string SoDienThoai { get; set; }

        // Vai trò (Admin, Staff, Nhân viên)
        public string VaiTro { get; set; }

        // Trạng thái (Còn làm, Nghỉ việc)
        public string TrangThai { get; set; }
    }
}