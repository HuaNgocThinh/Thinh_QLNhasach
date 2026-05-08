using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Linq;

namespace Thinh_QLNhasach.Views
{
    public partial class FrmDashboard : Form
    {
        // Sử dụng chuỗi kết nối chung từ file DbConnection
        string strConn = Thinh_QLNhasach.Database.DbConnection.connStr;

        // Khai báo 3 Label chứa tỷ lệ % ở đây để dùng chung
        Label lblSubDoanhThu = new Label();
        Label lblSubHoaDon = new Label();
        Label lblSubTonKho = new Label();

        public FrmDashboard()
        {
            InitializeComponent();
            dgvTop5.CellPainting += dgvTop5_CellPainting;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            CapNhatKichThuocVaToaDo();
        }

        // =========================================================
        // HÀM CẬP NHẬT TỌA ĐỘ KIỂU MỚI: TÍNH TOÁN ĐỘNG CHỐNG ĐÈ CHỮ
        // =========================================================
        private void CapNhatKichThuocVaToaDo()
        {
            if (pnlMain != null)
            {
                int margin = 30;
                int panelWidth = pnlMain.Width;
                int gridWidth = (panelWidth - (margin * 3)) / 2;

                // 1. TỌA ĐỘ LABEL TỔNG QUAN
                if (lblHeaderTieuDe != null)
                {
                    lblHeaderTieuDe.Location = new Point(margin, 20); // Cách đỉnh 20px
                }

                // 2. TỌA ĐỘ 3 THẺ: Lấy "đít" của chữ TỔNG QUAN + thêm 20px
                int cardY = (lblHeaderTieuDe != null) ? lblHeaderTieuDe.Bottom + 20 : 60;
                int cardWidth = (panelWidth - (margin * 4)) / 3;
                int cardHeight = 140;

                if (pnlDoanhThu != null)
                {
                    pnlDoanhThu.Location = new Point(margin, cardY);
                    pnlDoanhThu.Size = new Size(cardWidth, cardHeight);
                    pnlDoanhThu.Invalidate();
                }
                if (pnlHoaDon != null)
                {
                    pnlHoaDon.Location = new Point(margin * 2 + cardWidth, cardY);
                    pnlHoaDon.Size = new Size(cardWidth, cardHeight);
                    pnlHoaDon.Invalidate();
                }
                if (pnlTonKho != null)
                {
                    pnlTonKho.Location = new Point(margin * 3 + cardWidth * 2, cardY);
                    pnlTonKho.Size = new Size(cardWidth, cardHeight);
                    pnlTonKho.Invalidate();
                }

                // 3. TỌA ĐỘ TIÊU ĐỀ BẢNG VÀ BIỂU ĐỒ: Lấy "đít" của Thẻ + thêm 40px
                int labelY = cardY + cardHeight + 40;

                if (lblTieuDeDgv != null)
                {
                    lblTieuDeDgv.Location = new Point(margin, labelY);
                }
                if (lblTieuDeChart != null)
                {
                    lblTieuDeChart.Location = new Point(margin * 2 + gridWidth, labelY);
                }

                // 4. TỌA ĐỘ CỦA BẢNG VÀ BIỂU ĐỒ: Lấy "đít" của Label + thêm 15px khoảng trống
                int labelHeight = (lblTieuDeDgv != null) ? lblTieuDeDgv.Height : 40;
                int dgvY = labelY + labelHeight + 15;
                int contentHeight = pnlMain.Height - dgvY - margin;

                if (dgvTop5 != null)
                {
                    dgvTop5.Location = new Point(margin, dgvY);
                    dgvTop5.Width = gridWidth;
                    dgvTop5.Height = contentHeight > 200 ? contentHeight : 300;
                    GianDongDeuDgv();
                }

                if (chartDoanhSo != null)
                {
                    chartDoanhSo.Location = new Point(margin * 2 + gridWidth, dgvY);
                    chartDoanhSo.Width = gridWidth;
                    chartDoanhSo.Height = contentHeight > 200 ? contentHeight : 300;
                }
            }
        }

        private void FrmDashboard_Load(object sender, EventArgs e)
        {
            SetupCardUI();
            LoadThongKe();
            SetupDataGridView();
            LoadTop5Books();
            SetupAndLoadChart();

            CapNhatKichThuocVaToaDo();
        }

        // =========================================================
        // HÀM VẼ GIAO DIỆN BO GÓC CHO 3 THẺ TỔNG QUAN
        // =========================================================
        private void SetupCardUI()
        {
            if (pnlDoanhThu != null) pnlDoanhThu.BorderStyle = BorderStyle.None;
            if (pnlHoaDon != null) pnlHoaDon.BorderStyle = BorderStyle.None;
            if (pnlTonKho != null) pnlTonKho.BorderStyle = BorderStyle.None;

            if (pnlDoanhThu != null)
                pnlDoanhThu.Paint += (s, e) => DrawCardStyle(pnlDoanhThu, e, Color.FromArgb(32, 201, 151));
            if (pnlHoaDon != null)
                pnlHoaDon.Paint += (s, e) => DrawCardStyle(pnlHoaDon, e, Color.FromArgb(13, 110, 253));
            if (pnlTonKho != null)
                pnlTonKho.Paint += (s, e) => DrawCardStyle(pnlTonKho, e, Color.FromArgb(253, 126, 20));

            FormatLabelsInPanel(pnlDoanhThu, lblTongDoanhThu);
            FormatLabelsInPanel(pnlHoaDon, lblHoaDonHomNay);
            FormatLabelsInPanel(pnlTonKho, lblSachTonKho);

            if (lblTongDoanhThu != null) lblTongDoanhThu.ForeColor = Color.FromArgb(13, 110, 253);
            if (lblHoaDonHomNay != null) lblHoaDonHomNay.ForeColor = Color.Black;
            if (lblSachTonKho != null) lblSachTonKho.ForeColor = Color.Black;

            // Cài đặt 3 Label Subtitle (%)
            lblSubDoanhThu.Font = new Font("Segoe UI", 9.5f);
            lblSubDoanhThu.AutoSize = true;
            lblSubDoanhThu.BackColor = Color.White;
            lblSubDoanhThu.Location = new Point(18, 100);

            lblSubHoaDon.Font = new Font("Segoe UI", 9.5f);
            lblSubHoaDon.AutoSize = true;
            lblSubHoaDon.BackColor = Color.White;
            lblSubHoaDon.Location = new Point(18, 100);

            lblSubTonKho.Font = new Font("Segoe UI", 9.5f);
            lblSubTonKho.AutoSize = true;
            lblSubTonKho.BackColor = Color.White;
            lblSubTonKho.Location = new Point(18, 100);

            pnlDoanhThu?.Controls.Add(lblSubDoanhThu);
            pnlHoaDon?.Controls.Add(lblSubHoaDon);
            pnlTonKho?.Controls.Add(lblSubTonKho);
        }

        private void FormatLabelsInPanel(Panel pnl, Label mainValueLabel)
        {
            if (pnl == null) return;

            foreach (Control c in pnl.Controls)
            {
                if (c is Label lbl && lbl != mainValueLabel && lbl != lblSubDoanhThu && lbl != lblSubHoaDon && lbl != lblSubTonKho)
                {
                    lbl.AutoSize = true;
                    lbl.BackColor = Color.White;
                    lbl.Font = new Font("Segoe UI", 10, FontStyle.Regular);
                    lbl.ForeColor = Color.FromArgb(100, 100, 100);
                    lbl.Location = new Point(18, 15);
                }
            }

            if (mainValueLabel != null)
            {
                mainValueLabel.AutoSize = true;
                mainValueLabel.BackColor = Color.White;
                mainValueLabel.Font = new Font("Segoe UI", 18, FontStyle.Bold);
                mainValueLabel.Location = new Point(18, 45);
            }
        }

        private void DrawCardStyle(Panel pnl, PaintEventArgs e, Color leftBorderColor)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.Clear(pnl.Parent?.BackColor ?? Color.WhiteSmoke);

            Rectangle rect = new Rectangle(0, 0, pnl.Width - 1, pnl.Height - 1);
            int radius = 12;

            using (GraphicsPath path = GetRoundedPath(rect, radius))
            {
                e.Graphics.FillPath(Brushes.White, path);
                e.Graphics.DrawPath(new Pen(Color.FromArgb(220, 224, 232), 1), path);
            }

            using (GraphicsPath pathClip = GetRoundedPath(rect, radius))
            {
                e.Graphics.SetClip(pathClip);
                e.Graphics.FillRectangle(new SolidBrush(leftBorderColor), 0, 0, 6, pnl.Height);
                e.Graphics.ResetClip();
            }
        }

        private GraphicsPath GetRoundedPath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int d = radius * 2;
            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        // =========================================================
        // HÀM LẤY SỐ LIỆU TỪ DATABASE VÀ TÍNH TOÁN PHẦN TRĂM
        // =========================================================
        private void LoadThongKe()
        {
            using (SqlConnection conn = new SqlConnection(strConn))
            {
                try
                {
                    conn.Open();

                    // 1. LẤY DOANH THU THÁNG NÀY & THÁNG TRƯỚC
                    string queryDoanhThu = @"
                        DECLARE @CurrentRev FLOAT = (SELECT SUM(ThanhTien) FROM HoaDon WHERE MONTH(NgayLap) = MONTH(GETDATE()) AND YEAR(NgayLap) = YEAR(GETDATE()));
                        DECLARE @LastRev FLOAT = (SELECT SUM(ThanhTien) FROM HoaDon WHERE MONTH(NgayLap) = MONTH(DATEADD(month, -1, GETDATE())) AND YEAR(NgayLap) = YEAR(DATEADD(month, -1, GETDATE())));
                        SELECT ISNULL(@CurrentRev, 0) AS CurrentRev, ISNULL(@LastRev, 0) AS LastRev;
                    ";

                    using (SqlCommand cmd = new SqlCommand(queryDoanhThu, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                double currRev = Convert.ToDouble(reader["CurrentRev"]);
                                double lastRev = Convert.ToDouble(reader["LastRev"]);
                                lblTongDoanhThu.Text = currRev.ToString("N0") + " đ";
                                TinhToanPhanTram(lblSubDoanhThu, currRev, lastRev, "tháng trước");
                            }
                        }
                    }

                    // 2. LẤY HÓA ĐƠN HÔM NAY & HÔM QUA
                    string queryHoaDon = @"
                        DECLARE @TodayInv INT = (SELECT COUNT(*) FROM HoaDon WHERE CAST(NgayLap AS DATE) = CAST(GETDATE() AS DATE));
                        DECLARE @YestInv INT = (SELECT COUNT(*) FROM HoaDon WHERE CAST(NgayLap AS DATE) = CAST(DATEADD(day, -1, GETDATE()) AS DATE));
                        SELECT @TodayInv AS TodayInv, @YestInv AS YestInv;
                    ";

                    using (SqlCommand cmd = new SqlCommand(queryHoaDon, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                double currInv = Convert.ToDouble(reader["TodayInv"]);
                                double lastInv = Convert.ToDouble(reader["YestInv"]);
                                lblHoaDonHomNay.Text = currInv.ToString();
                                TinhToanPhanTram(lblSubHoaDon, currInv, lastInv, "hôm qua");
                            }
                        }
                    }

                    // 3. TỔNG SÁCH TỒN KHO & CẢNH BÁO
                    string queryTonKho = @"
                        SELECT ISNULL(SUM(SoLuongTon), 0) AS TongTon, 
                               (SELECT COUNT(*) FROM Sach WHERE SoLuongTon < 10) AS SapHet 
                        FROM Sach;
                    ";

                    using (SqlCommand cmd = new SqlCommand(queryTonKho, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                lblSachTonKho.Text = reader["TongTon"].ToString();
                                int sapHet = Convert.ToInt32(reader["SapHet"]);

                                if (sapHet > 0)
                                {
                                    lblSubTonKho.Text = $"⚠ {sapHet} đầu sách sắp hết hàng";
                                    lblSubTonKho.ForeColor = Color.IndianRed;
                                }
                                else
                                {
                                    lblSubTonKho.Text = "✓ Tồn kho ổn định";
                                    lblSubTonKho.ForeColor = Color.MediumSeaGreen;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi lấy dữ liệu thống kê: " + ex.Message);
                }
            }
        }

        private void TinhToanPhanTram(Label lbl, double current, double previous, string timeFrame)
        {
            if (previous == 0)
            {
                if (current > 0)
                {
                    lbl.Text = $"↑ 100% so với {timeFrame}";
                    lbl.ForeColor = Color.MediumSeaGreen;
                }
                else
                {
                    lbl.Text = $"- 0% so với {timeFrame}";
                    lbl.ForeColor = Color.Gray;
                }
            }
            else
            {
                double percent = ((current - previous) / previous) * 100;

                if (percent >= 0)
                {
                    lbl.Text = $"↑ {Math.Abs(percent):F1}% so với {timeFrame}";
                    lbl.ForeColor = Color.MediumSeaGreen;
                }
                else
                {
                    lbl.Text = $"↓ {Math.Abs(percent):F1}% so với {timeFrame}";
                    lbl.ForeColor = Color.IndianRed;
                }
            }
        }

        // =========================================================
        // PHẦN BẢNG TOP SÁCH VÀ VẼ GIAO DIỆN
        // =========================================================
        private void GianDongDeuDgv()
        {
            if (dgvTop5 != null && dgvTop5.Rows.Count > 0)
            {
                int totalHeight = dgvTop5.ClientSize.Height - 2;
                int rowHeight = totalHeight / dgvTop5.Rows.Count;

                if (rowHeight < 60)
                {
                    rowHeight = 60;
                }

                foreach (DataGridViewRow row in dgvTop5.Rows)
                {
                    row.Height = rowHeight;
                }
            }
        }

        private void SetupDataGridView()
        {
            dgvTop5.EnableHeadersVisualStyles = false;
            dgvTop5.BackgroundColor = Color.White;
            dgvTop5.BorderStyle = BorderStyle.None;
            dgvTop5.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvTop5.GridColor = Color.FromArgb(235, 238, 245);
            dgvTop5.ColumnHeadersVisible = false;
            dgvTop5.RowHeadersVisible = false;
            dgvTop5.DefaultCellStyle.BackColor = Color.White;
            dgvTop5.DefaultCellStyle.SelectionBackColor = Color.White;
            dgvTop5.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgvTop5.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dgvTop5.RowTemplate.Height = 60;
            dgvTop5.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvTop5.AllowUserToAddRows = false;

            dgvTop5.ReadOnly = true;
            dgvTop5.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        private void LoadTop5Books()
        {
            using (SqlConnection conn = new SqlConnection(strConn))
            {
                try
                {
                    conn.Open();
                    string query = @"
                        SELECT TOP 5 
                            CAST(ROW_NUMBER() OVER(ORDER BY SUM(c.SoLuong) DESC) AS VARCHAR) + '. ' + s.TenSach AS [TenSach], 
                            tg.TenTG AS [TacGia],
                            s.HinhAnh AS [HinhAnh],
                            SUM(c.SoLuong) AS [SoLuongBan]
                        FROM ChiTietHoaDon c
                        INNER JOIN HoaDon h ON c.MaHD = h.MaHD
                        INNER JOIN Sach s ON c.MaSach = s.MaSach
                        LEFT JOIN TacGia tg ON s.MaTG = tg.MaTG
                        GROUP BY s.TenSach, tg.TenTG, s.HinhAnh
                        ORDER BY SUM(c.SoLuong) DESC";

                    using (SqlDataAdapter da = new SqlDataAdapter(query, conn))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        dgvTop5.DataSource = null;
                        dgvTop5.Columns.Clear();

                        dgvTop5.Columns.Add("ColInfor", "Thong Tin");
                        dgvTop5.Columns.Add("ColSL", "So Luong");
                        dgvTop5.Columns.Add("ColHinhAnh", "Hinh Anh");
                        dgvTop5.Columns["ColHinhAnh"].Visible = false;

                        dgvTop5.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                        dgvTop5.Columns[1].Width = 140;

                        dgvTop5.Rows.Clear();

                        foreach (DataRow row in dt.Rows)
                        {
                            string tenSach = row["TenSach"].ToString();
                            string tacGia = row["TacGia"].ToString();
                            string hinhAnh = row["HinhAnh"].ToString();

                            string thongTin = tenSach + (string.IsNullOrWhiteSpace(tacGia) ? "" : "\n" + tacGia);
                            string soLuong = row["SoLuongBan"].ToString();

                            dgvTop5.Rows.Add(thongTin, soLuong, hinhAnh);
                        }

                        GianDongDeuDgv();
                        dgvTop5.ClearSelection();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi lấy dữ liệu Top 5 Sách: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void dgvTop5_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex >= 0 && e.Value != null)
            {
                // VẼ HUY HIỆU
                if (e.ColumnIndex == 1)
                {
                    e.PaintBackground(e.CellBounds, true);

                    string text = e.Value.ToString() + " quyển";
                    Font badgeFont = new Font("Segoe UI", 10, FontStyle.Bold);
                    SizeF textSize = e.Graphics.MeasureString(text, badgeFont);

                    int width = (int)textSize.Width + 28;
                    int height = (int)textSize.Height + 14;

                    int x = e.CellBounds.Right - width - 35;
                    int y = e.CellBounds.Top + (e.CellBounds.Height - height) / 2;

                    GraphicsPath path = new GraphicsPath();
                    path.AddArc(x, y, height, height, 90, 180);
                    path.AddArc(x + width - height, y, height, height, 270, 180);
                    path.CloseFigure();

                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                    using (SolidBrush brush = new SolidBrush(Color.FromArgb(13, 110, 253)))
                    {
                        e.Graphics.FillPath(brush, path);
                    }

                    using (SolidBrush textBrush = new SolidBrush(Color.White))
                    {
                        StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                        e.Graphics.DrawString(text, badgeFont, textBrush, new Rectangle(x, y, width, height), sf);
                    }

                    e.Handled = true;
                }
                // VẼ ẢNH VÀ CHỮ
                else if (e.ColumnIndex == 0)
                {
                    e.PaintBackground(e.CellBounds, true);
                    string[] parts = e.Value.ToString().Split('\n');

                    int iconSize = 80;
                    Rectangle iconRect = new Rectangle(e.CellBounds.Left + 10, e.CellBounds.Top + (e.CellBounds.Height - iconSize) / 2, iconSize, iconSize);

                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                    string tenAnh = dgvTop5.Rows[e.RowIndex].Cells[2].Value?.ToString();
                    bool veAnhThanhCong = false;

                    if (!string.IsNullOrEmpty(tenAnh))
                    {
                        string duongDanAnh = System.IO.Path.Combine(Application.StartupPath, "Images", tenAnh);
                        if (System.IO.File.Exists(duongDanAnh))
                        {
                            try
                            {
                                using (Image img = Image.FromFile(duongDanAnh))
                                {
                                    e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                                    e.Graphics.DrawImage(img, iconRect);
                                    veAnhThanhCong = true;
                                }
                            }
                            catch { }
                        }
                    }

                    if (!veAnhThanhCong)
                    {
                        e.Graphics.FillRectangle(new SolidBrush(Color.Gainsboro), iconRect);
                        e.Graphics.DrawRectangle(new Pen(Color.DarkGray, 1), iconRect);
                        e.Graphics.FillRectangle(new SolidBrush(Color.White), new Rectangle(iconRect.X + 2, iconRect.Y + 2, 5, iconRect.Height - 4));
                    }

                    int startX = iconRect.Right + 25;
                    Font titleFont = new Font("Segoe UI", 12, FontStyle.Bold);
                    Font authorFont = new Font("Segoe UI", 9.5f, FontStyle.Regular);

                    if (parts.Length == 1 || string.IsNullOrWhiteSpace(parts[1]))
                    {
                        int startY = e.CellBounds.Top + (e.CellBounds.Height - titleFont.Height) / 2;
                        e.Graphics.DrawString(parts[0], titleFont, Brushes.Black, new Point(startX, startY));
                    }
                    else
                    {
                        int khoangCachGiaHaiDong = 10;
                        int totalTextHeight = titleFont.Height + khoangCachGiaHaiDong + authorFont.Height;
                        int startY = e.CellBounds.Top + (e.CellBounds.Height - totalTextHeight) / 2;

                        // Vẽ Tên sách
                        e.Graphics.DrawString(parts[0], titleFont, Brushes.Black, new Point(startX, startY));

                        // Vẽ Tác giả ngay bên dưới
                        int authorY = startY + titleFont.Height + khoangCachGiaHaiDong;
                        e.Graphics.DrawString(parts[1], authorFont, Brushes.Gray, new Point(startX, authorY));
                    }

                    e.Graphics.DrawLine(new Pen(Color.FromArgb(235, 238, 245)), e.CellBounds.Left, e.CellBounds.Bottom - 1, e.CellBounds.Right, e.CellBounds.Bottom - 1);

                    e.Handled = true;
                }
            }
        }

        // =========================================================
        // PHẦN BIỂU ĐỒ
        // =========================================================
        private void SetupAndLoadChart()
        {
            chartDoanhSo.BackColor = Color.White;
            chartDoanhSo.ChartAreas[0].BackColor = Color.White;
            chartDoanhSo.ChartAreas[0].BorderWidth = 0;

            chartDoanhSo.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.Gainsboro;
            chartDoanhSo.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.Gainsboro;
            chartDoanhSo.ChartAreas[0].AxisX.LineColor = Color.LightGray;
            chartDoanhSo.ChartAreas[0].AxisY.LineColor = Color.LightGray;

            chartDoanhSo.ChartAreas[0].AxisX.LabelStyle.Font = new Font("Segoe UI", 9F);
            chartDoanhSo.ChartAreas[0].AxisY.LabelStyle.Font = new Font("Segoe UI", 9F);
            chartDoanhSo.ChartAreas[0].AxisX.LabelStyle.ForeColor = Color.DimGray;
            chartDoanhSo.ChartAreas[0].AxisY.LabelStyle.ForeColor = Color.DimGray;

            if (chartDoanhSo.Series.Count > 0)
            {
                chartDoanhSo.Series[0].Color = Color.FromArgb(70, 130, 180);
                chartDoanhSo.Series[0].BackGradientStyle = GradientStyle.TopBottom;
                chartDoanhSo.Series[0].BackSecondaryColor = Color.FromArgb(176, 196, 222);
                chartDoanhSo.Series[0].Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                chartDoanhSo.Series[0].LabelForeColor = Color.Navy;
                chartDoanhSo.Series[0].IsValueShownAsLabel = true;
                chartDoanhSo.Series[0].LabelFormat = "{0:N0}";
            }

            chartDoanhSo.Legends[0].Enabled = false;

            using (SqlConnection conn = new SqlConnection(strConn))
            {
                try
                {
                    conn.Open();
                    string query = @"
                        SELECT TOP 7 
                            CAST(NgayLap AS DATE) AS Ngay, 
                            SUM(ThanhTien) AS DoanhThu 
                        FROM HoaDon 
                        GROUP BY CAST(NgayLap AS DATE) 
                        ORDER BY CAST(NgayLap AS DATE) DESC";

                    using (SqlDataAdapter da = new SqlDataAdapter(query, conn))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        chartDoanhSo.Series[0].Points.Clear();

                        for (int i = dt.Rows.Count - 1; i >= 0; i--)
                        {
                            DataRow row = dt.Rows[i];
                            DateTime ngay = Convert.ToDateTime(row["Ngay"]);
                            double doanhThu = Convert.ToDouble(row["DoanhThu"]);

                            chartDoanhSo.Series[0].Points.AddXY(ngay.ToString("dd/MM"), doanhThu);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi hiển thị biểu đồ: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}