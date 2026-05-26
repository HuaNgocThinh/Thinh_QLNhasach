using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace Thinh_QLNhasach.Views
{
    public partial class FrmNhatKy : Form
    {
        // Ép cứng chuỗi kết nối siêu an toàn
        string strConn = @"Data Source=.\SQLEXPRESS01;Initial Catalog=BookShop;Integrated Security=True";

        public FrmNhatKy()
        {
            InitializeComponent();
        }

        private void FrmNhatKy_Load(object sender, EventArgs e)
        {
            // Setup ngày mặc định: Từ đầu tháng đến hôm nay
            dtpTuNgay.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            dtpDenNgay.Value = DateTime.Now;

            // TỰ ĐỘNG MÓC NỐI SỰ KIỆN: Khỏi sợ lỗi do quên click đúp ở màn hình Design
            txtSearch.TextChanged -= txtSearch_TextChanged;
            txtSearch.TextChanged += txtSearch_TextChanged;

            dtpTuNgay.ValueChanged -= dtpTuNgay_ValueChanged;
            dtpTuNgay.ValueChanged += dtpTuNgay_ValueChanged;

            dtpDenNgay.ValueChanged -= dtpDenNgay_ValueChanged;
            dtpDenNgay.ValueChanged += dtpDenNgay_ValueChanged;

            // Tìm và gắn sự kiện cho nút "Lọc kết quả" tự động
            AutoWireSearchButton(this);

            // Tải dữ liệu lần đầu
            LoadDataNhatKy();
        }

        // ==============================================================
        // HÀM TỰ ĐỘNG TÌM NÚT LỌC VÀ GẮN SỰ KIỆN
        // ==============================================================
        private void AutoWireSearchButton(Control parent)
        {
            foreach (Control c in parent.Controls)
            {
                if (c is Button && c.Text.Contains("Lọc"))
                {
                    c.Click -= btnSearch_Click;
                    c.Click += btnSearch_Click;
                }
                else if (c.HasChildren)
                {
                    AutoWireSearchButton(c);
                }
            }
        }

        // ==============================================================
        // 1. HÀM TRANG TRÍ BẢNG
        // ==============================================================
        private void FormatDataGridView(DataGridView dgv)
        {
            if (dgv == null) return;

            dgv.EnableHeadersVisualStyles = false;
            dgv.BackgroundColor = Color.White;
            dgv.BorderStyle = BorderStyle.FixedSingle;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            dgv.GridColor = Color.DarkGray;
            dgv.RowHeadersVisible = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // Header
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.Gray;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            dgv.ColumnHeadersHeight = 45;

            // Nội dung dòng
            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 11F);
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(230, 230, 250);
            dgv.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgv.RowTemplate.Height = 40;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250);
        }

        // ==============================================================
        // 2. HÀM TẢI DỮ LIỆU (BỘ LỌC CHỐNG "MÙ" TIẾNG VIỆT)
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

                    string keyword = txtSearch.Text.Trim().Normalize(System.Text.NormalizationForm.FormC);

                    string sql = @"
                        SELECT MaLog, TenDangNhap, HanhDong, ChiTiet, ThoiGian 
                        FROM NhatKyHoatDong
                        WHERE ThoiGian >= @tu AND ThoiGian <= @den ";

                    if (!string.IsNullOrEmpty(keyword))
                    {
                        sql += @" AND (
                                    LOWER(TenDangNhap) LIKE LOWER(@kw) 
                                    OR LOWER(HanhDong) LIKE LOWER(@kw) 
                                    OR LOWER(ChiTiet) LIKE LOWER(@kw)
                                  ) ";
                    }

                    sql += " ORDER BY ThoiGian DESC";

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.Add("@tu", SqlDbType.DateTime).Value = tu;
                    cmd.Parameters.Add("@den", SqlDbType.DateTime).Value = den;

                    if (!string.IsNullOrEmpty(keyword))
                    {
                        cmd.Parameters.Add("@kw", SqlDbType.NVarChar).Value = "%" + keyword + "%";
                    }

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    dgvNhatKy.DataSource = dt;
                    FormatDataGridView(dgvNhatKy);

                    if (dgvNhatKy.Columns.Count > 0)
                    {
                        dgvNhatKy.Columns["MaLog"].HeaderText = "Mã Log";
                        dgvNhatKy.Columns["TenDangNhap"].HeaderText = "Người Dùng";
                        dgvNhatKy.Columns["HanhDong"].HeaderText = "Hành Động";
                        dgvNhatKy.Columns["ChiTiet"].HeaderText = "Chi Tiết Nhật Ký";
                        dgvNhatKy.Columns["ThoiGian"].HeaderText = "Thời Gian";
                        dgvNhatKy.Columns["ThoiGian"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm:ss";
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi tải nhật ký: " + ex.Message, "Thông báo lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // ==============================================================
        // 3. CÁC SỰ KIỆN KÍCH HOẠT TÌM KIẾM
        // ==============================================================
        private void btnSearch_Click(object sender, EventArgs e)
        {
            LoadDataNhatKy();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            LoadDataNhatKy();
        }

        private void dtpTuNgay_ValueChanged(object sender, EventArgs e)
        {
            LoadDataNhatKy();
        }

        private void dtpDenNgay_ValueChanged(object sender, EventArgs e)
        {
            LoadDataNhatKy();
        }
    }
}