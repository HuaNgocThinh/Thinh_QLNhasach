using System;
using System.Data.SqlClient;

namespace Thinh_QLNhasach // Sửa lại namespace cho chuẩn project của ông nhé
{
    public static class AppLogger
    {
        // Hàm này gọi được ở bất cứ form nào mà không cần new
        public static void GhiLog(string tenDangNhap, string hanhDong, string chiTiet)
        {
            // Dùng đúng chuỗi kết nối máy ông
            string connStr = @"Data Source=THINHLALUOT\SQLEXPRESS01;Initial Catalog=BookShop;Integrated Security=True";

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    conn.Open();
                    string sql = "INSERT INTO NhatKyHoatDong (TenDangNhap, HanhDong, ChiTiet, ThoiGian) VALUES (@user, @hd, @ct, GETDATE())";
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@user", tenDangNhap);
                    cmd.Parameters.AddWithValue("@hd", hanhDong);
                    cmd.Parameters.AddWithValue("@ct", chiTiet);

                    cmd.ExecuteNonQuery();
                }
                catch
                {
                    // Bỏ qua lỗi nếu ghi log thất bại để không làm sập phần mềm chính
                }
            }
        }
    }
}