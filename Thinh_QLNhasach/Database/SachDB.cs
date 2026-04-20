using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using Thinh_QLNhasach.Models;

namespace Thinh_QLNhasach.Database
{
    public class SachDB
    {
        public List<Sach> GetAll()
        {
            List<Sach> list = new List<Sach>();

            using (SqlConnection conn = new SqlConnection(DbConnection.connStr))
            {
                conn.Open();
                string query = "SELECT * FROM Sach";

                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader rd = cmd.ExecuteReader();

                while (rd.Read())
                {
                    list.Add(new Sach
                    {
                        MaSach = (int)rd["MaSach"],
                        TenSach = rd["TenSach"].ToString(),
                        GiaBan = (decimal)rd["GiaBan"],
                        SoLuongTon = (int)rd["SoLuongTon"]
                    });
                }
            }
            return list;
        }

        public void Insert(Sach s)
        {
            using (SqlConnection conn = new SqlConnection(DbConnection.connStr))
            {
                conn.Open();

                string query = "INSERT INTO Sach(TenSach,GiaBan,SoLuongTon) VALUES(@t,@g,@sl)";
                SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@t", s.TenSach);
                cmd.Parameters.AddWithValue("@g", s.GiaBan);
                cmd.Parameters.AddWithValue("@sl", s.SoLuongTon);

                cmd.ExecuteNonQuery();
            }
        }
    }
}