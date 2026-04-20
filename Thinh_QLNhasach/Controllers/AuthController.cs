using System;
using System.Data;
using System.Data.SqlClient;
using Thinh_QLNhasach.Database;
using Thinh_QLNhasach.Models;

namespace Thinh_QLNhasach.Controllers
{
    public class AuthController
    {
        public NguoiDung Login(string username, string password)
        {
            using (SqlConnection conn = new SqlConnection(DbConnection.connStr))
            {
                // QUAN TRỌNG: Phải có CAST(@p AS VARCHAR(MAX)) để nó băm khớp với SQL
                string query = "SELECT MaND, TenDangNhap, HoTen, VaiTro FROM NguoiDung " +
                               "WHERE TenDangNhap = @u AND MatKhau = HASHBYTES('MD5', CAST(@p AS VARCHAR(MAX)))";

                SqlCommand cmd = new SqlCommand(query, conn);
                // Ép kiểu VarChar ở đây luôn cho chắc cú, không cho C# tự ý dùng NVarChar (Unicode)
                cmd.Parameters.Add("@u", SqlDbType.NVarChar).Value = username;
                cmd.Parameters.Add("@p", SqlDbType.VarChar).Value = password;

                try
                {
                    conn.Open();
                    SqlDataReader rd = cmd.ExecuteReader();

                    if (rd.Read())
                    {
                        return new NguoiDung
                        {
                            MaND = Convert.ToInt32(rd["MaND"]),
                            TenDangNhap = rd["TenDangNhap"].ToString(),
                            HoTen = rd["HoTen"].ToString(),
                            VaiTro = rd["VaiTro"].ToString()
                        };
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Lỗi Database: " + ex.Message);
                }
            }
            return null;
        }
    }
}