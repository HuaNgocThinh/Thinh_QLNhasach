using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Thinh_QLNhasach.Views
{
    public partial class FrmThongKe : Form
    {
        string connectionString = @"Data Source=THINHLALUOT\SQLEXPRESS01;Initial Catalog=BookShop;Integrated Security=True";

        public FrmThongKe()
        {
            InitializeComponent();
        }

        private void FrmThongKe_Load(object sender, EventArgs e)
        {
            dtpTuNgay.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            dtpDenNgay.Value = DateTime.Now;
            btnThongKe.PerformClick();
        }

        private void btnThongKe_Click(object sender, EventArgs e)
        {
            DateTime tu = dtpTuNgay.Value.Date;
            DateTime den = dtpDenNgay.Value.Date.AddDays(1).AddSeconds(-1);

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    string sqlSum = @"
                        SELECT 
                            (SELECT SUM(ThanhTien) FROM HoaDon WHERE NgayLap BETWEEN @tu AND @den) as DoanhThu,
                            (SELECT COUNT(MaHD) FROM HoaDon WHERE NgayLap BETWEEN @tu AND @den) as SoHD,
                            (SELECT SUM(ct.SoLuong) FROM ChiTietHoaDon ct JOIN HoaDon h ON ct.MaHD = h.MaHD WHERE h.NgayLap BETWEEN @tu AND @den) as TongSach";

                    SqlCommand cmd = new SqlCommand(sqlSum, conn);
                    cmd.Parameters.AddWithValue("@tu", tu);
                    cmd.Parameters.AddWithValue("@den", den);

                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        double doanhThu = dr["DoanhThu"] != DBNull.Value ? Convert.ToDouble(dr["DoanhThu"]) : 0;
                        lblTongDoanhThu.Text = doanhThu.ToString("N0") + " VNĐ";
                        lblSoHoaDon.Text = dr["SoHD"].ToString();
                        lblSachDaBan.Text = dr["TongSach"] != DBNull.Value ? dr["TongSach"].ToString() : "0";
                    }
                    dr.Close();

                    string sqlGrid = @"
                        SELECT TOP 10 s.TenSach as [Tên Sách], SUM(ct.SoLuong) as [Số Lượng Bán], SUM(ct.ThanhTien) as [Doanh Thu]
                        FROM ChiTietHoaDon ct 
                        JOIN Sach s ON ct.MaSach = s.MaSach
                        JOIN HoaDon h ON ct.MaHD = h.MaHD
                        WHERE h.NgayLap BETWEEN @tu AND @den
                        GROUP BY s.TenSach 
                        ORDER BY [Số Lượng Bán] DESC";

                    SqlDataAdapter da = new SqlDataAdapter(sqlGrid, conn);
                    da.SelectCommand.Parameters.AddWithValue("@tu", tu);
                    da.SelectCommand.Parameters.AddWithValue("@den", den);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    dgvThongKe.DataSource = dt;
                    FormatDataGridView(dgvThongKe);

                    chartThongKe.Series.Clear();
                    Series series = new Series("Số lượng bán");
                    series.ChartType = SeriesChartType.Column;
                    series.IsValueShownAsLabel = true;

                    foreach (DataRow row in dt.Rows)
                    {
                        series.Points.AddXY(row["Tên Sách"].ToString(), row["Số Lượng Bán"]);
                    }

                    chartThongKe.Series.Add(series);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi thống kê: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void FormatDataGridView(DataGridView dgv)
        {
            dgv.EnableHeadersVisualStyles = false;

            // Căn đều cột, nền bảng màu trắng
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.BackgroundColor = Color.White;

            // Kẻ viền xám nhạt
            dgv.BorderStyle = BorderStyle.FixedSingle;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            dgv.GridColor = Color.LightGray;

            // 1. HEADER (Giữ nguyên xám, chữ trắng)
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.Gray;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgv.ColumnHeadersHeight = 35;
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;

            // 2. DÒNG BÌNH THƯỜNG (Toàn bộ màu trắng)
            dgv.DefaultCellStyle.BackColor = Color.White;
            dgv.DefaultCellStyle.ForeColor = Color.Black;
            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.White;
            dgv.RowTemplate.Height = 30;

            // ==========================================
            // 3. KHI BẤM VÀO: HIỆN MÀU TÍM LAVENDER, CHỮ ĐEN
            // ==========================================
            dgv.DefaultCellStyle.SelectionBackColor = Color.Lavender;
            dgv.DefaultCellStyle.SelectionForeColor = Color.Black;

            // 4. Ẩn cột mũi tên bên trái và chọn cả dòng
            dgv.RowHeadersVisible = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }
    }
}