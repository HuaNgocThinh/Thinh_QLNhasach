using OfficeOpenXml;        // Thêm thư viện EPPlus Xuất Excel
using OfficeOpenXml.Style;  // Thêm thư viện Trang trí Excel
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;

namespace Thinh_QLNhasach
{
    public partial class FrmThongKe : Form
    {
        private Dictionary<string, Image> imageCache = new Dictionary<string, Image>();

        public FrmThongKe()
        {
            InitializeComponent();

            dgvTopSach.CellPainting += dgvTopSach_CellPainting;
            dgvTopSach.RowPostPaint += dgvTopSach_RowPostPaint;
            dgvTopSach.Resize += (s, e) => DanDeuDongDGV(dgvTopSach);
            dgvTopSach.SelectionChanged += (s, e) => dgvTopSach.ClearSelection();
        }

        private void FrmThongKe_Load(object sender, EventArgs e)
        {
            dtpThang.Value = DateTime.Now;

            dgvTopSach.CellBorderStyle = DataGridViewCellBorderStyle.None;
            dgvTopSach.AdvancedCellBorderStyle.All = DataGridViewAdvancedCellBorderStyle.None;
            dgvTopSach.GridColor = Color.White;
            dgvTopSach.BackgroundColor = Color.White;

            dgvTopSach.ReadOnly = true;
            dgvTopSach.AllowUserToResizeColumns = false;
            dgvTopSach.AllowUserToResizeRows = false;
            dgvTopSach.RowHeadersVisible = false;
            dgvTopSach.ColumnHeadersVisible = false;

            // ĐÃ FIX: Tắt hoàn toàn thanh cuộn, chơi hệ Dashboard
            dgvTopSach.ScrollBars = ScrollBars.None;

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
                                ROW_NUMBER() OVER(ORDER BY SUM(c.SoLuong) DESC) AS STT,
                                s.MaSach,
                                s.TenSach, 
                                tg.TenTG AS TacGia, 
                                s.HinhAnh, 
                                SUM(c.SoLuong) AS SoLuongBan
                             FROM ChiTietHoaDon c
                             INNER JOIN HoaDon h ON c.MaHD = h.MaHD
                             INNER JOIN Sach s ON c.MaSach = s.MaSach
                             LEFT JOIN TacGia tg ON s.MaTG = tg.MaTG 
                             WHERE MONTH(h.NgayLap) = @Thang AND YEAR(h.NgayLap) = @Nam
                             GROUP BY s.MaSach, s.TenSach, tg.TenTG, s.HinhAnh
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

                    if (dgvTopSach.Columns.Contains("STT")) dgvTopSach.Columns["STT"].Visible = false;
                    if (dgvTopSach.Columns.Contains("MaSach")) dgvTopSach.Columns["MaSach"].Visible = false;
                    if (dgvTopSach.Columns.Contains("TacGia")) dgvTopSach.Columns["TacGia"].Visible = false;
                    if (dgvTopSach.Columns.Contains("HinhAnh")) dgvTopSach.Columns["HinhAnh"].Visible = false;

                    if (dgvTopSach.Columns.Contains("TenSach") && dgvTopSach.Columns.Contains("SoLuongBan"))
                    {
                        dgvTopSach.Columns["TenSach"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                        dgvTopSach.Columns["TenSach"].DividerWidth = 0;

                        dgvTopSach.Columns["SoLuongBan"].Width = 140;
                        dgvTopSach.Columns["SoLuongBan"].DividerWidth = 0;
                    }

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

        private void dgvTopSach_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                DataRowView drv = (DataRowView)dgvTopSach.Rows[e.RowIndex].DataBoundItem;
                if (drv == null) return;

                Rectangle clearRect = new Rectangle(e.CellBounds.X - 1, e.CellBounds.Y - 1, e.CellBounds.Width + 2, e.CellBounds.Height + 2);
                e.Graphics.FillRectangle(Brushes.White, clearRect);

                int colTenSach = dgvTopSach.Columns["TenSach"].Index;
                int colSoLuong = dgvTopSach.Columns["SoLuongBan"].Index;

                if (e.ColumnIndex == colTenSach)
                {
                    // =========================================================
                    // ĐÃ FIX: TỰ ĐỘNG CO GIÃN ẢNH VÀ CHỮ THEO CHIỀU CAO THỰC TẾ
                    // =========================================================

                    int padding = 12; // Chừa khoảng lề trên/dưới 6px
                    int imgSize = e.CellBounds.Height - padding;
                    if (imgSize > 80) imgSize = 80; // To tối đa 80px để không bị vỡ ảnh
                    if (imgSize < 30) imgSize = 30; // Nhỏ tối đa 30px để còn nhìn thấy

                    int imgX = e.CellBounds.X + 20;
                    int imgY = e.CellBounds.Y + (e.CellBounds.Height - imgSize) / 2;

                    string maSach = drv["MaSach"].ToString();
                    object imgData = drv["HinhAnh"];
                    Image imgToDraw = null;

                    if (!imageCache.ContainsKey(maSach))
                    {
                        if (imgData != DBNull.Value && imgData != null)
                        {
                            try
                            {
                                if (imgData is byte[])
                                {
                                    using (MemoryStream ms = new MemoryStream((byte[])imgData))
                                    {
                                        imageCache[maSach] = Image.FromStream(ms);
                                    }
                                }
                                else if (imgData is string && !string.IsNullOrWhiteSpace(imgData.ToString()))
                                {
                                    string path = Path.Combine(Application.StartupPath, "Images", imgData.ToString());
                                    if (File.Exists(path)) imageCache[maSach] = Image.FromFile(path);
                                    else imageCache[maSach] = null;
                                }
                                else { imageCache[maSach] = null; }
                            }
                            catch { imageCache[maSach] = null; }
                        }
                        else { imageCache[maSach] = null; }
                    }
                    imgToDraw = imageCache[maSach];

                    if (imgToDraw != null)
                    {
                        e.Graphics.DrawImage(imgToDraw, new Rectangle(imgX, imgY, imgSize, imgSize));
                    }
                    else
                    {
                        e.Graphics.FillRectangle(Brushes.WhiteSmoke, imgX, imgY, imgSize, imgSize);
                        e.Graphics.DrawRectangle(Pens.LightGray, imgX, imgY, imgSize, imgSize);
                    }

                    // TÍNH TOÁN FONT CHỮ TO NHỎ THEO CỠ ẢNH
                    int fontSizeTitle = imgSize >= 60 ? 12 : (imgSize >= 45 ? 11 : 9);
                    int fontSizeAuthor = imgSize >= 60 ? 10 : 8;

                    Font titleFont = new Font("Segoe UI", fontSizeTitle, FontStyle.Bold);
                    Font authorFont = new Font("Segoe UI", fontSizeAuthor, FontStyle.Regular);

                    string titleText = $"{drv["STT"]}. {drv["TenSach"]}";
                    string authorText = drv["TacGia"].ToString();

                    int textX = imgX + imgSize + 15;
                    int totalTextHeight = titleFont.Height + 4 + authorFont.Height;
                    int startY = e.CellBounds.Y + (e.CellBounds.Height - totalTextHeight) / 2;

                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    e.Graphics.DrawString(titleText, titleFont, Brushes.Black, textX, startY);

                    using (SolidBrush authorBrush = new SolidBrush(Color.Gray))
                    {
                        e.Graphics.DrawString(authorText, authorFont, authorBrush, textX, startY + titleFont.Height + 4);
                    }
                }
                else if (e.ColumnIndex == colSoLuong)
                {
                    string text = drv["SoLuongBan"].ToString() + " quyển";

                    // Giãn chữ Huy Hiệu theo độ cao dòng
                    int badgeFontSize = e.CellBounds.Height >= 70 ? 10 : 9;
                    Font badgeFont = new Font("Segoe UI", badgeFontSize, FontStyle.Bold);
                    SizeF textSize = e.Graphics.MeasureString(text, badgeFont);

                    int width = (int)textSize.Width + 24;
                    int height = (int)textSize.Height + 10;

                    int x = e.CellBounds.Right - width - 20;
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
                }

                e.Handled = true;
            }
        }

        private void dgvTopSach_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.None;
            using (Pen p = new Pen(Color.FromArgb(235, 235, 235), 1))
            {
                int y = e.RowBounds.Bottom - 1;
                e.Graphics.DrawLine(p, e.RowBounds.Left, y, e.RowBounds.Right, y);
            }
        }

        // =========================================================
        // ĐÃ FIX: CHIA ĐỀU CHIỀU CAO KHÔNG BỊ HỞ HAY CẮT MẨU
        // =========================================================
        private void DanDeuDongDGV(DataGridView dgv)
        {
            if (dgv != null && dgv.Rows.Count > 0)
            {
                dgv.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;

                int totalHeight = dgv.ClientSize.Height;
                if (dgv.ColumnHeadersVisible) totalHeight -= dgv.ColumnHeadersHeight;

                // Chia đều phần nguyên và giữ lại phần dư
                int rowHeight = totalHeight / dgv.Rows.Count;
                int remainder = totalHeight % dgv.Rows.Count;

                if (rowHeight < 30) rowHeight = 30; // Chỉ khóa min 30px để chống sập UI

                for (int i = 0; i < dgv.Rows.Count; i++)
                {
                    // Dòng cuối cùng gánh thêm phần lẻ dư để lấp đầy 100% khe hở dưới cùng
                    if (i == dgv.Rows.Count - 1 && totalHeight >= 30 * dgv.Rows.Count)
                    {
                        dgv.Rows[i].Height = rowHeight + remainder;
                    }
                    else
                    {
                        dgv.Rows[i].Height = rowHeight;
                    }
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

                        ws.Cells[7, 1].Value = "Tên Sách";
                        ws.Cells[7, 1].Style.Font.Bold = true;
                        ws.Cells[7, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        ws.Cells[7, 1].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                        ws.Cells[7, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                        ws.Cells[7, 2].Value = "Số Lượng Bán";
                        ws.Cells[7, 2].Style.Font.Bold = true;
                        ws.Cells[7, 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        ws.Cells[7, 2].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                        ws.Cells[7, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                        for (int i = 0; i < dgvTopSach.Rows.Count; i++)
                        {
                            DataRowView drv = (DataRowView)dgvTopSach.Rows[i].DataBoundItem;

                            ws.Cells[i + 8, 1].Value = $"{drv["STT"]}. {drv["TenSach"]}";
                            ws.Cells[i + 8, 2].Value = drv["SoLuongBan"].ToString();

                            ws.Cells[i + 8, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            ws.Cells[i + 8, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);
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