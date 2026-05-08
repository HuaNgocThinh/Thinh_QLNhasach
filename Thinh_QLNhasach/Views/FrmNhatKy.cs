using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace Thinh_QLNhasach.Views
{
    public partial class FrmNhatKy : Form
    {
        // Sử dụng chuỗi kết nối từ Class DbConnection của ông
        string strConn = Thinh_QLNhasach.Database.DbConnection.connStr;

        public FrmNhatKy()
        {
            InitializeComponent();
        }

        private void FrmNhatKy_Load(object sender, EventArgs e)
        {
            // Setup ngày mặc định: Từ đầu tháng đến hôm nay
            dtpTuNgay.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            dtpDenNgay.Value = DateTime.Now;

            // Tải dữ liệu lần đầu
            LoadDataNhatKy();
        }

        // ==============================================================
        // 1. HÀM TRANG TRÍ BẢNG (GIỐNG HỆT HÌNH MẪU NHÂN VIÊN)
        // ==============================================================
        private void FormatDataGridView(DataGridView dgv)
        {
            if (dgv == null) return;

            // 1. CẤU HÌNH CƠ BẢN
            dgv.EnableHeadersVisualStyles = false;
            dgv.BackgroundColor = Color.White;
            dgv.BorderStyle = BorderStyle.FixedSingle;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            dgv.GridColor = Color.DarkGray;
            dgv.RowHeadersVisible = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // 2. HEADER TỔNG THỂ (Màu xám mặc định cho các cột sau)
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(128, 128, 128); // Xám
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            dgv.ColumnHeadersHeight = 45;

            // 3. NỘI DUNG & MÀU TÍM LAVENDER KHI CLICK
            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 11F);
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(230, 230, 250); // Tím Lavender
            dgv.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgv.RowTemplate.Height = 40;

            // 4. DÀN TRẢI CỘT
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // 5. VIỆT HÓA & TÔ MÀU XANH CỘT ĐẦU (CHỐNG TRÔI MÀU)
            // Thay vì dùng ColumnAdded, mình duyệt trực tiếp danh sách cột đã có
            foreach (DataGridViewColumn col in dgv.Columns)
            {
                // Chỉ đổi tên tiếng Việt và độ rộng, không ép màu nữa
                if (col.Name.Contains("MaLog")) { col.HeaderText = "Mã Log"; col.FillWeight = 60; }
                if (col.Name.Contains("TenDangNhap")) { col.HeaderText = "Người Dùng"; col.FillWeight = 100; }
                if (col.Name.Contains("HanhDong")) { col.HeaderText = "Hành Động"; col.FillWeight = 120; }
                if (col.Name.Contains("ChiTiet")) { col.HeaderText = "Chi Tiết Nhật Ký"; col.FillWeight = 250; }
                if (col.Name.Contains("ThoiGian"))
                {
                    col.HeaderText = "Thời Gian";
                    col.DefaultCellStyle.Format = "dd/MM/yyyy HH:mm:ss";
                    col.FillWeight = 130;
                }
            }

            // 6. MÀU XEN KẼ
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250);

            // 7. HIỆU ỨNG CHỌN DÒNG (ĐÃ CẬP NHẬT MÀU TÍM LAVENDER)
            // Màu nền khi click chọn dòng
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(230, 230, 250);

            // Chữ màu đen cho dễ đọc trên nền tím nhạt (Đừng để màu trắng nó sẽ bị mờ)
            dgv.DefaultCellStyle.SelectionForeColor = Color.Black;

            // Đảm bảo chọn nguyên dòng chứ không phải chọn từng ô lẻ
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // Tắt cái viền xanh xanh bao quanh ô khi click (cho nó mượt)
            dgv.Focus();
        }

        // ==============================================================
        // 2. HÀM TẢI DỮ LIỆU (KẾT HỢP LỌC VÀ ĐỊNH DẠNG)
        // ==============================================================
        private void LoadDataNhatKy()
        {
            using (SqlConnection conn = new SqlConnection(strConn))
            {
                try
                {
                    if (conn.State == ConnectionState.Closed) conn.Open();

                    DateTime tu = dtpTuNgay.Value.Date;
                    DateTime den = dtpDenNgay.Value.Date.AddDays(1).AddSeconds(-1);
                    string keyword = "%" + txtSearch.Text.Trim() + "%";

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

                    // Đổ dữ liệu vào bảng
                    dgvNhatKy.DataSource = dt;

                    // PHẢI GỌI HÀM NÀY SAU KHI CÓ DATA THÌ NÓ MỚI HIỆN MÀU XANH VÀ VIỀN Ô
                    FormatDataGridView(dgvNhatKy);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi tải nhật ký: " + ex.Message, "Thông báo");
                }
            }
        }

        // Sự kiện khi bấm nút Lọc
        private void btnSearch_Click(object sender, EventArgs e)
        {
            LoadDataNhatKy();
        }

        // Tự động lọc khi đang gõ chữ
        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            LoadDataNhatKy();
        }
    }
}