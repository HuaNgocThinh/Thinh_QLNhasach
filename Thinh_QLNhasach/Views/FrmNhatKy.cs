using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Controls;
using System.Windows.Forms;

namespace Thinh_QLNhasach.Views // Nếu đang dùng thư mục Views thì sửa thành Thinh_QLNhasach.Views nhé
{
    public partial class FrmNhatKy : Form
    {
        // Nhớ check lại chuỗi kết nối cho chuẩn với máy ông
        string strConn = @"Data Source=THINHLALUOT\SQLEXPRESS01; Initial Catalog=BookShop; Integrated Security=True";

        public FrmNhatKy()
        {
            InitializeComponent();
        }

        private void FrmNhatKy_Load(object sender, EventArgs e)
        {
            // 1. Khởi tạo ngày mặc định: Từ đầu tháng đến ngày hôm nay
            dtpTuNgay.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            dtpDenNgay.Value = DateTime.Now;

            // 2. Khóa cứng Bảng dữ liệu (Tuyệt đối không cho thêm/sửa/xóa ở màn hình này)
            dgvNhatKy.ReadOnly = true;
            dgvNhatKy.AllowUserToAddRows = false;
            dgvNhatKy.AllowUserToDeleteRows = false;

            // 3. Tải dữ liệu lần đầu
            LoadDataNhatKy();
        }

        // ===== HÀM TẢI DỮ LIỆU (KẾT HỢP LỌC NGÀY VÀ TÌM KIẾM) =====
        private void LoadDataNhatKy()
        {
            using (SqlConnection conn = new SqlConnection(strConn))
            {
                try
                {
                    conn.Open();

                    // Lấy giá trị ngày (Cộng thêm 1 ngày trừ 1 giây để lấy trọn vẹn 23:59:59 của ngày kết thúc)
                    DateTime tu = dtpTuNgay.Value.Date;
                    DateTime den = dtpDenNgay.Value.Date.AddDays(1).AddSeconds(-1);
                    string keyword = "%" + txtSearch.Text.Trim() + "%";

                    // Câu SQL thần thánh: Lọc theo ngày VÀ tìm từ khóa cùng lúc, xếp mới nhất lên đầu
                    string sql = @"
                        SELECT 
                            MaLog as [Mã Log], 
                            TenDangNhap as [Người Dùng], 
                            HanhDong as [Hành Động], 
                            ChiTiet as [Chi Tiết], 
                            ThoiGian as [Thời Gian]
                        FROM NhatKyHoatDong
                        WHERE ThoiGian BETWEEN @tu AND @den
                          AND (TenDangNhap LIKE @kw OR HanhDong LIKE @kw OR ChiTiet LIKE @kw)
                        ORDER BY ThoiGian DESC";

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@tu", tu);
                    cmd.Parameters.AddWithValue("@den", den);
                    cmd.Parameters.AddWithValue("@kw", keyword);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    dgvNhatKy.DataSource = dt;

                    // Hô biến bảng thành giao diện phẳng
                    FormatDataGridView(dgvNhatKy);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi tải nhật ký: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // ===== NÚT TÌM KIẾM =====
        private void BtnSearch_Click(object sender, EventArgs e)
        {
            // Chỉ cần gọi lại hàm Load là nó tự động lấy ngày và text mới để truy vấn
            LoadDataNhatKy();
        }

        // ===== TRANG TRÍ BẢNG (Giống y hệt tông màu tím Lavender ông thích) =====
        private void FormatDataGridView(DataGridView dgv)
        {
            dgv.EnableHeadersVisualStyles = false;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.BackgroundColor = Color.White;

            // Kẻ viền xám nhạt
            dgv.BorderStyle = BorderStyle.FixedSingle;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            dgv.GridColor = Color.LightGray;

            // Header xám chữ trắng
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.Gray;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgv.ColumnHeadersHeight = 35;
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;

            // Dòng bình thường màu trắng
            dgv.DefaultCellStyle.BackColor = Color.White;
            dgv.DefaultCellStyle.ForeColor = Color.Black;
            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.White;
            dgv.RowTemplate.Height = 30;

            // Bấm vào hiện màu tím Lavender
            dgv.DefaultCellStyle.SelectionBackColor = Color.Lavender;
            dgv.DefaultCellStyle.SelectionForeColor = Color.Black;

            dgv.RowHeadersVisible = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // Chỉnh lại kích thước các cột cho cân đối (Nội dung chi tiết cho rộng ra)
            if (dgv.Columns.Count > 0)
            {
                dgv.Columns["Mã Log"].FillWeight = 50;
                dgv.Columns["Người Dùng"].FillWeight = 100;
                dgv.Columns["Hành Động"].FillWeight = 150;
                dgv.Columns["Chi Tiết"].FillWeight = 300;
                dgv.Columns["Thời Gian"].FillWeight = 150;

                // Format lại giờ phút giây cho ngầu
                dgv.Columns["Thời Gian"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm:ss";
            }
        }
    }
}