using OfficeOpenXml;        // Thêm thư viện EPPlus Xuất Excel
using OfficeOpenXml.Style;  // Thêm thư viện Trang trí Excel
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;            // Thêm thư viện xử lý File
using System.Windows.Forms;

namespace Thinh_QLNhasach
{
    public partial class FrmThongKe : Form
    {
        public FrmThongKe()
        {
            InitializeComponent();

            // Đăng ký sự kiện TỰ TAY VẼ BẢNG 100%
            dgvTopSach.CellPainting += dgvTopSach_CellPainting;

            // Đăng ký sự kiện tự động dàn lại dòng khi Form/Bảng bị thay đổi kích thước
            dgvTopSach.Resize += (s, e) => DanDeuDongDGV(dgvTopSach);

            // CHIÊU BẤT TỬ: Khóa hoàn toàn tính năng bôi đen (chọn dòng)
            dgvTopSach.SelectionChanged += (s, e) => dgvTopSach.ClearSelection();
        }

        private void FrmThongKe_Load(object sender, EventArgs e)
        {
            dtpThang.Value = DateTime.Now;

            // ☢️ BỘ HỦY DIỆT: ĐẬP NÁT MỌI LOẠI ĐƯỜNG VIỀN MẶC ĐỊNH
            dgvTopSach.CellBorderStyle = DataGridViewCellBorderStyle.None;
            dgvTopSach.AdvancedCellBorderStyle.All = DataGridViewAdvancedCellBorderStyle.None;
            dgvTopSach.GridColor = Color.White; // Ép màu Grid thành Trắng tàng hình
            dgvTopSach.BackgroundColor = Color.White;
            dgvTopSach.BorderStyle = BorderStyle.None; // Ẩn viền bao quanh DGV

            // Khóa mọi thao tác tương tác
            dgvTopSach.ReadOnly = true;
            dgvTopSach.AllowUserToResizeColumns = false;
            dgvTopSach.AllowUserToResizeRows = false;
            dgvTopSach.RowHeadersVisible = false;
            dgvTopSach.ColumnHeadersVisible = false;

            ThucHienThongKe();
        }

        private void btnThongKe_Click(object sender, EventArgs e)
        {
            ThucHienThongKe();
        }

        private void ThucHienThongKe()
        {
            int thang = dtpThang.Value.Month;
            int nam = dtpThang.Value.Year;

            lblTieuDeDoanhThu.Text = $"Doanh thu tháng {thang}/{nam}";

            using (SqlConnection conn = new SqlConnection(Thinh_QLNhasach.Database.DbConnection.connStr))
            {
                try
                {
                    if (conn.State == ConnectionState.Closed)
                        conn.Open();

                    LoadDoanhThu(conn, thang, nam);
                    LoadTop10Sach(conn, thang, nam);
                    LoadCanhBaoTonKho(conn);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi hệ thống: " + ex.Message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void LoadDoanhThu(SqlConnection conn, int thang, int nam)
        {
            string query = "SELECT SUM(ThanhTien) FROM HoaDon WHERE MONTH(NgayLap) = @Thang AND YEAR(NgayLap) = @Nam";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Thang", thang);
                cmd.Parameters.AddWithValue("@Nam", nam);

                object result = cmd.ExecuteScalar();
                if (result != DBNull.Value && result != null)
                {
                    lblDoanhThu.Text = Convert.ToDouble(result).ToString("N0") + " VNĐ";
                }
                else
                {
                    lblDoanhThu.Text = "0 VNĐ";
                }
            }
        }

        private void LoadTop10Sach(SqlConnection conn, int thang, int nam)
        {
            string query = @"SELECT TOP 10 
                                CAST(ROW_NUMBER() OVER(ORDER BY SUM(c.SoLuong) DESC) AS VARCHAR) + '. ' + s.TenSach AS [Tên Sách], 
                                SUM(c.SoLuong) AS [Số Lượng Bán]
                             FROM ChiTietHoaDon c
                             INNER JOIN HoaDon h ON c.MaHD = h.MaHD
                             INNER JOIN Sach s ON c.MaSach = s.MaSach
                             WHERE MONTH(h.NgayLap) = @Thang AND YEAR(h.NgayLap) = @Nam
                             GROUP BY s.TenSach
                             ORDER BY SUM(c.SoLuong) DESC";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Thang", thang);
                cmd.Parameters.AddWithValue("@Nam", nam);

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgvTopSach.DataSource = dt;

                    if (dgvTopSach.Columns.Count >= 2)
                    {
                        dgvTopSach.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                        dgvTopSach.Columns[0].DividerWidth = 0; // Chặn kẽ hở cột 0

                        dgvTopSach.Columns[1].Width = 140;
                        dgvTopSach.Columns[1].DividerWidth = 0; // Chặn kẽ hở cột 1
                    }

                    // Tránh scrollbar nếu số dòng nhỏ (làm giao diện ko đẹp)
                    dgvTopSach.ScrollBars = dt.Rows.Count > 10 ? ScrollBars.Vertical : ScrollBars.None;

                    // Gọi dàn dòng sau khi nạp dữ liệu
                    DanDeuDongDGV(dgvTopSach);
                }
            }
        }

        private void LoadCanhBaoTonKho(SqlConnection conn)
        {
            string query = "SELECT COUNT(*) FROM Sach WHERE SoLuongTon < 10";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                int soSachSapHet = Convert.ToInt32(cmd.ExecuteScalar());
                if (soSachSapHet > 0)
                {
                    lblCanhBao.Text = $"⚠ CHÚ Ý: Hiện đang có {soSachSapHet} đầu sách sắp hết hàng (Dưới 10 cuốn)!";
                    lblCanhBao.BackColor = Color.LightPink;
                    lblCanhBao.ForeColor = Color.DarkRed;
                }
                else
                {
                    lblCanhBao.Text = "Tồn kho ổn định, không có sách nào dưới 10 cuốn.";
                    lblCanhBao.BackColor = Color.Honeydew;
                    lblCanhBao.ForeColor = Color.DarkGreen;
                }
            }
        }

        // =========================================================
        // TỰ VẼ 100% GIAO DIỆN - QUÉT SƠN ĐÈ LÊN MỌI LỖI CỦA WINFORMS
        // =========================================================
        private void dgvTopSach_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                // 1. Phóng to cọ quét sang 2 bên thêm 2 pixel để "đè bẹp" bất kỳ đường kẻ dọc nào còn sót lại giữa các cột
                Rectangle clearRect = new Rectangle(e.CellBounds.X - 1, e.CellBounds.Y, e.CellBounds.Width + 2, e.CellBounds.Height);
                e.Graphics.FillRectangle(Brushes.White, clearRect);

                // 2. Tự kẻ tay 1 đường line xám mỏng manh dưới đáy (Xóa sổ vĩnh viễn đường kẻ dọc)
                using (Pen p = new Pen(Color.FromArgb(235, 238, 245), 1))
                {
                    // Trừ đi 1 px để ko bị viền chèn
                    e.Graphics.DrawLine(p, e.CellBounds.Left, e.CellBounds.Bottom - 1, e.CellBounds.Right, e.CellBounds.Bottom - 1);
                }

                // 3. TỰ VẼ NỘI DUNG VÀO Ô
                if (e.Value != null)
                {
                    // CỘT 0: VẼ TÊN SÁCH
                    if (e.ColumnIndex == 0)
                    {
                        Font textFont = new Font("Segoe UI", 11, FontStyle.Regular);
                        int textY = e.CellBounds.Y + (e.CellBounds.Height - textFont.Height) / 2;

                        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                        e.Graphics.DrawString(e.Value.ToString(), textFont, Brushes.Black, e.CellBounds.X + 15, textY);
                    }
                    // CỘT 1: VẼ HUY HIỆU SỐ LƯỢNG
                    else if (e.ColumnIndex == 1)
                    {
                        string text = e.Value.ToString() + " quyển";
                        Font badgeFont = new Font("Segoe UI", 10, FontStyle.Bold);
                        SizeF textSize = e.Graphics.MeasureString(text, badgeFont);

                        int width = (int)textSize.Width + 24;
                        int height = (int)textSize.Height + 12;

                        int x = e.CellBounds.Right - width - 30; // Chừa margin phải
                        int y = e.CellBounds.Top + (e.CellBounds.Height - height) / 2;

                        GraphicsPath path = new GraphicsPath();
                        path.AddArc(x, y, height, height, 90, 180);
                        path.AddArc(x + width - height, y, height, height, 270, 180);
                        path.CloseFigure();

                        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                        using (SolidBrush brush = new SolidBrush(Color.FromArgb(41, 128, 185)))
                        {
                            e.Graphics.FillPath(brush, path);
                        }

                        using (SolidBrush textBrush = new SolidBrush(Color.White))
                        {
                            StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                            e.Graphics.DrawString(text, badgeFont, textBrush, new Rectangle(x, y, width, height), sf);
                        }
                    }
                }

                // 4. CHỐT CHẶN TỐI THƯỢNG: Báo cho WinForms "Tao vẽ xong rồi, cấm mày vẽ thêm cái gì nữa"
                e.Handled = true;
            }
        }

        private void DanDeuDongDGV(DataGridView dgv)
        {
            if (dgv != null && dgv.Rows.Count > 0)
            {
                dgv.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
                
                // Lấy chiều cao vùng Data khả dụng (bỏ viền)
                int totalHeight = dgv.ClientRectangle.Height;

                if (dgv.ColumnHeadersVisible) totalHeight -= dgv.ColumnHeadersHeight;

                // Chia đều cho các hàng
                int rowHeight = totalHeight / dgv.Rows.Count;
                if (rowHeight < 40) rowHeight = 40;

                // Gán chiều cao
                foreach (DataGridViewRow row in dgv.Rows)
                {
                    row.Height = rowHeight;
                }
            }
        }

        private void btnXuatExcel_Click(object sender, EventArgs e)
        {
            if (dgvTopSach.Rows.Count == 0)
            {
                MessageBox.Show("Chưa có dữ liệu thống kê để xuất!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Excel Files (*.xlsx)|*.xlsx";
            sfd.FileName = "BaoCaoThongKe_" + DateTime.Now.ToString("ddMMyyyy_HHmm") + ".xlsx";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (ExcelPackage excel = new ExcelPackage())
                    {
                        ExcelWorksheet ws = excel.Workbook.Worksheets.Add("Báo Cáo Thống Kê");

                        ws.Cells["A1"].Value = "BÁO CÁO THỐNG KÊ KINH DOANH NHÀ SÁCH";
                        ws.Cells["A1:B1"].Merge = true;
                        ws.Cells["A1"].Style.Font.Bold = true;
                        ws.Cells["A1"].Style.Font.Size = 14;
                        ws.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        ws.Cells["A3"].Value = lblTieuDeDoanhThu.Text;
                        ws.Cells["A3"].Style.Font.Bold = true;
                        ws.Cells["A4"].Value = "Tổng doanh thu:";
                        ws.Cells["B4"].Value = lblDoanhThu.Text;
                        ws.Cells["B4"].Style.Font.Bold = true;
                        ws.Cells["B4"].Style.Font.Color.SetColor(Color.Green);

                        ws.Cells["A6"].Value = "DANH SÁCH TOP 10 SÁCH BÁN CHẠY NHẤT";
                        ws.Cells["A6:B6"].Merge = true;
                        ws.Cells["A6"].Style.Font.Bold = true;

                        for (int i = 0; i < dgvTopSach.Columns.Count; i++)
                        {
                            ws.Cells[7, i + 1].Value = dgvTopSach.Columns[i].HeaderText;
                            ws.Cells[7, i + 1].Style.Font.Bold = true;
                            ws.Cells[7, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            ws.Cells[7, i + 1].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                            ws.Cells[7, i + 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        }

                        for (int i = 0; i < dgvTopSach.Rows.Count; i++)
                        {
                            for (int j = 0; j < dgvTopSach.Columns.Count; j++)
                            {
                                ws.Cells[i + 8, j + 1].Value = dgvTopSach.Rows[i].Cells[j].Value?.ToString();
                                ws.Cells[i + 8, j + 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            }
                        }

                        int lastRow = dgvTopSach.Rows.Count + 10;
                        ws.Cells[$"A{lastRow}"].Value = "Tình trạng tồn kho:";
                        ws.Cells[$"A{lastRow}"].Style.Font.Bold = true;
                        ws.Cells[$"A{lastRow + 1}"].Value = lblCanhBao.Text;
                        ws.Cells[$"A{lastRow + 1}"].Style.Font.Color.SetColor(lblCanhBao.Text.Contains("⚠") ? Color.Red : Color.Green);

                        ws.Cells[ws.Dimension.Address].AutoFitColumns();

                        FileInfo excelFile = new FileInfo(sfd.FileName);
                        excel.SaveAs(excelFile);

                        MessageBox.Show("Xuất báo cáo Excel thành công xuất sắc!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        System.Diagnostics.Process.Start(sfd.FileName);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi xuất Excel: Vui lòng đóng file Excel nếu nó đang được mở.\n" + ex.Message, "Lỗi");
                }
            }
        }
    }
}