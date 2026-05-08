using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using FontAwesome.Sharp;
using Thinh_QLNhasach.Utility;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;

namespace Thinh_QLNhasach.Views
{
    public partial class FrmHoaDon : Form
    {
        string connectionString = @"Data Source=THINHLALUOT\SQLEXPRESS01;Initial Catalog=BookShop;Integrated Security=True";
        DataTable dtGioHang = new DataTable();
        bool isProcessing = false;

        public FrmHoaDon()
        {
            InitializeComponent();
            InitGioHang();
        }

        private void InitGioHang()
        {
            if (dtGioHang.Columns.Count == 0)
            {
                dtGioHang.Columns.Add("MaSach", typeof(string));
                dtGioHang.Columns.Add("TenSach", typeof(string));
                dtGioHang.Columns.Add("SoLuong", typeof(int));
                dtGioHang.Columns.Add("DonGia", typeof(double));
                dtGioHang.Columns.Add("ThanhTien", typeof(double));
            }
        }

        private void FrmHoaDon_Load(object sender, EventArgs e)
        {
            dgvGioHang.DataSource = dtGioHang;

            FormatDataGridView(dgvGioHang);
            FormatDataGridView(dgvHoaDon);

            if (dgvGioHang.Columns.Contains("MaSach")) dgvGioHang.Columns["MaSach"].Visible = false;

            if (cboGiamGia != null)
            {
                cboGiamGia.Items.Clear();
                cboGiamGia.Items.AddRange(new string[] { "0%", "5%", "10%", "15%" });
                cboGiamGia.DropDownStyle = ComboBoxStyle.DropDownList;
                cboGiamGia.SelectedIndex = 0;
            }

            LoadComboBoxNguoiDung();
            LoadComboBoxSach();
            ResetForm();
            LoadLichSuHoaDon();
        }

        private void FormatDataGridView(DataGridView dgv)
        {
            if (dgv == null) return;

            dgv.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            dgv.GridColor = Color.DarkGray;
            dgv.BackgroundColor = Color.White;
            dgv.BorderStyle = BorderStyle.FixedSingle;
            dgv.EnableHeadersVisualStyles = false;

            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(128, 128, 128);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 11F, FontStyle.Bold);
            dgv.ColumnHeadersHeight = 45;

            dgv.DefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 11F);
            dgv.RowTemplate.Height = 40;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.RowHeadersVisible = false;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dgv.ColumnAdded += (s, e) =>
            {
                // BẢNG GIỎ HÀNG
                if (e.Column.Name == "TenSach" && dgv.Name == "dgvGioHang") e.Column.HeaderText = "Tên Sách";
                if (e.Column.Name == "SoLuong" && dgv.Name == "dgvGioHang") { e.Column.HeaderText = "Số Lượng"; e.Column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter; }
                if (e.Column.Name == "DonGia" && dgv.Name == "dgvGioHang") { e.Column.HeaderText = "Đơn Giá"; e.Column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight; }
                if (e.Column.Name == "ThanhTien" && dgv.Name == "dgvGioHang") { e.Column.HeaderText = "Thành Tiền"; e.Column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight; }

                // BẢNG LỊCH SỬ HÓA ĐƠN
                if (e.Column.Name == "MaHD") { e.Column.HeaderText = "Mã HD"; e.Column.FillWeight = 80; }
                if (e.Column.Name == "NgayLap") { e.Column.HeaderText = "Ngày Lập"; e.Column.DefaultCellStyle.Format = "dd/MM/yyyy HH:mm"; }
                if (e.Column.Name == "NhanVien") { e.Column.HeaderText = "Nhân Viên"; e.Column.FillWeight = 120; }
                if (e.Column.Name == "TenKhachHang") { e.Column.HeaderText = "Khách Hàng"; e.Column.FillWeight = 120; }
                if (e.Column.Name == "TenSach" && dgv.Name == "dgvHoaDon") { e.Column.HeaderText = "Tên Sách"; e.Column.FillWeight = 150; }
                if (e.Column.Name == "SoLuong" && dgv.Name == "dgvHoaDon") { e.Column.HeaderText = "SL"; e.Column.FillWeight = 50; e.Column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter; }
                if (e.Column.Name == "DonGia" && dgv.Name == "dgvHoaDon") { e.Column.HeaderText = "Đơn Giá"; e.Column.DefaultCellStyle.Format = "N0"; e.Column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight; }
                if (e.Column.Name == "ThanhTien" && dgv.Name == "dgvHoaDon") { e.Column.HeaderText = "Tiền Sách"; e.Column.DefaultCellStyle.Format = "N0"; e.Column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight; }

                // 3 CỘT MỚI UPDATE: TỔNG TIỀN GỐC, GIẢM GIÁ, THỰC THU
                if (e.Column.Name == "TongTien" && dgv.Name == "dgvHoaDon") { e.Column.HeaderText = "Tổng Tiền Gốc"; e.Column.DefaultCellStyle.Format = "N0"; e.Column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight; }
                if (e.Column.Name == "GiamGia" && dgv.Name == "dgvHoaDon") { e.Column.HeaderText = "Giảm Giá"; e.Column.DefaultCellStyle.Format = "N0"; e.Column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight; e.Column.DefaultCellStyle.ForeColor = Color.IndianRed; }
                if (e.Column.Name == "ThucThu" && dgv.Name == "dgvHoaDon") { e.Column.HeaderText = "Thực Thu"; e.Column.DefaultCellStyle.Format = "N0"; e.Column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight; e.Column.DefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 11F, FontStyle.Bold); e.Column.DefaultCellStyle.ForeColor = Color.MediumSeaGreen; }

                if (e.Column.Index == 0 || (dgv.Columns.Count > 0 && !dgv.Columns[0].Visible && e.Column.Index == 1))
                {
                    e.Column.HeaderCell.Style.BackColor = Color.FromArgb(0, 120, 215);
                }
            };

            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250);
        }

        private string TuSinhMaHD()
        {
            string maMoi = "HD001";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("SELECT TOP 1 MaHD FROM HoaDon ORDER BY MaHD DESC", conn);
                    object result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        string maCu = result.ToString();
                        if (int.TryParse(maCu.Substring(2), out int soHienTai))
                        {
                            maMoi = "HD" + (soHienTai + 1).ToString("D3");
                        }
                    }
                }
                catch { maMoi = "HD001"; }
            }
            return maMoi;
        }

        private void LoadComboBoxNguoiDung()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlDataAdapter da = new SqlDataAdapter("SELECT MaND, HoTen FROM NguoiDung", conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                cboMaNV.DataSource = dt;
                cboMaNV.DisplayMember = "HoTen";
                cboMaNV.ValueMember = "MaND";
                cboMaNV.SelectedIndex = -1;
            }
        }

        private void LoadComboBoxSach()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlDataAdapter da = new SqlDataAdapter("SELECT MaSach, TenSach, GiaBan, SoLuongTon FROM Sach WHERE SoLuongTon > 0", conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                cboMaSach.DataSource = dt;
                cboMaSach.DisplayMember = "TenSach";
                cboMaSach.ValueMember = "MaSach";
                cboMaSach.SelectedIndex = -1;
            }
        }

        private void cboMaSach_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboMaSach.SelectedIndex >= 0 && cboMaSach.SelectedItem != null)
            {
                DataRowView row = cboMaSach.SelectedItem as DataRowView;
                if (row != null) txtDonGia.Text = row["GiaBan"].ToString();
            }
            else { txtDonGia.Clear(); }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (isProcessing) return;
            isProcessing = true;

            try
            {
                if (cboMaSach.SelectedValue == null) { MessageBox.Show("Vui lòng chọn sách!"); return; }
                string maSach = cboMaSach.SelectedValue.ToString();
                int soLuongThem = (int)nudSoLuong.Value;

                DataRowView row = cboMaSach.SelectedItem as DataRowView;
                int tonKho = Convert.ToInt32(row["SoLuongTon"]);

                foreach (DataRow r in dtGioHang.Rows)
                {
                    if (r["MaSach"].ToString() == maSach)
                    {
                        int slHienTai = Convert.ToInt32(r["SoLuong"]);
                        if (slHienTai + soLuongThem > tonKho)
                        {
                            MessageBox.Show($"Kho chỉ còn {tonKho} cuốn!");
                            return;
                        }

                        r["SoLuong"] = slHienTai + soLuongThem;
                        double gia = Convert.ToDouble(r["DonGia"]);
                        r["ThanhTien"] = (slHienTai + soLuongThem) * gia;

                        CapNhatTien();
                        return;
                    }
                }

                if (soLuongThem > tonKho) { MessageBox.Show($"Kho chỉ còn {tonKho} cuốn!"); return; }

                double donGia = 0;
                double.TryParse(txtDonGia.Text, out donGia);
                double thanhTien = soLuongThem * donGia;

                dtGioHang.Rows.Add(maSach, cboMaSach.Text, soLuongThem, donGia, thanhTien);
                CapNhatTien();
            }
            finally { isProcessing = false; }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dgvGioHang.CurrentRow == null || dgvGioHang.CurrentRow.IsNewRow) return;

            DataRowView drv = (DataRowView)dgvGioHang.CurrentRow.DataBoundItem;
            cboMaSach.SelectedValue = drv["MaSach"].ToString();

            decimal sl = 1;
            if (drv["SoLuong"] != DBNull.Value) decimal.TryParse(drv["SoLuong"].ToString(), out sl);

            if (sl > nudSoLuong.Maximum) nudSoLuong.Maximum = sl + 500;
            nudSoLuong.Value = sl;

            txtDonGia.Text = drv["DonGia"].ToString();
            dtGioHang.Rows.Remove(drv.Row);
            CapNhatTien();
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            if (dgvGioHang.CurrentRow != null && !dgvGioHang.CurrentRow.IsNewRow)
            {
                DataRowView drv = (DataRowView)dgvGioHang.CurrentRow.DataBoundItem;
                dtGioHang.Rows.Remove(drv.Row);
                CapNhatTien();
            }
        }

        private void CapNhatTien()
        {
            double tongTien = 0;
            foreach (DataRow r in dtGioHang.Rows)
            {
                if (r["ThanhTien"] != DBNull.Value)
                {
                    tongTien += Convert.ToDouble(r["ThanhTien"]);
                }
            }
            txtTongTien.Text = tongTien.ToString("N0");

            double phanTramGiam = 0;
            if (cboGiamGia.SelectedItem != null)
            {
                string rawGiamGia = cboGiamGia.SelectedItem.ToString().Replace("%", "").Trim();
                if (double.TryParse(rawGiamGia, out double parseGiamGia))
                {
                    phanTramGiam = parseGiamGia / 100.0;
                }
            }

            double giamGiaTien = tongTien * phanTramGiam;
            double thanhTien = tongTien - giamGiaTien;

            if (thanhTien < 0) thanhTien = 0;

            txtThanhTien.Text = thanhTien.ToString("N0");
        }

        private void txtGiamGia_TextChanged(object sender, EventArgs e) { CapNhatTien(); }

        private void cboGiamGia_SelectedIndexChanged(object sender, EventArgs e) { CapNhatTien(); }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (dtGioHang == null || dtGioHang.Rows.Count == 0) return;
            if (cboMaNV.SelectedValue == null) { MessageBox.Show("Vui lòng chọn Nhân viên!"); return; }

            DataTable dtTemp = dtGioHang.Copy();
            dtGioHang.Clear();

            double tongTien = 0;
            double thanhTien = 0;
            double giamGiaTien = 0;

            double.TryParse(txtTongTien.Text.Replace(",", "").Replace(".", ""), out tongTien);
            double.TryParse(txtThanhTien.Text.Replace(",", "").Replace(".", ""), out thanhTien);

            giamGiaTien = tongTien - thanhTien;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    SqlTransaction trans = conn.BeginTransaction();
                    try
                    {
                        string sqlHD = @"INSERT INTO HoaDon (MaHD, MaND, TenKhachHang, NgayLap, TongTien, GiamGia, ThanhTien) 
                                 VALUES (@mahd, @mand, @tenkh, @ngay, @tong, @giam, @thanhtien)";

                        SqlCommand cmdHD = new SqlCommand(sqlHD, conn, trans);
                        cmdHD.Parameters.AddWithValue("@mahd", txtMaHD.Text);
                        cmdHD.Parameters.AddWithValue("@mand", cboMaNV.SelectedValue);
                        cmdHD.Parameters.AddWithValue("@tenkh", string.IsNullOrWhiteSpace(txtTenKH.Text) ? "Khách lẻ" : txtTenKH.Text.Trim());
                        cmdHD.Parameters.AddWithValue("@ngay", dtpNgayLap.Value);
                        cmdHD.Parameters.AddWithValue("@tong", tongTien);
                        cmdHD.Parameters.AddWithValue("@giam", giamGiaTien);
                        cmdHD.Parameters.AddWithValue("@thanhtien", thanhTien);
                        cmdHD.ExecuteNonQuery();

                        foreach (DataRow dr in dtTemp.Rows)
                        {
                            SqlCommand cmdCT = new SqlCommand("INSERT INTO ChiTietHoaDon (MaHD, MaSach, SoLuong, DonGia, ThanhTien) VALUES (@mahd, @masach, @sl, @dg, @tt)", conn, trans);
                            cmdCT.Parameters.AddWithValue("@mahd", txtMaHD.Text);
                            cmdCT.Parameters.AddWithValue("@masach", dr["MaSach"]);
                            cmdCT.Parameters.AddWithValue("@sl", dr["SoLuong"]);
                            cmdCT.Parameters.AddWithValue("@dg", dr["DonGia"]);
                            cmdCT.Parameters.AddWithValue("@tt", dr["ThanhTien"]);
                            cmdCT.ExecuteNonQuery();

                            SqlCommand cmdUp = new SqlCommand("UPDATE Sach SET SoLuongTon = SoLuongTon - @sl WHERE MaSach=@ms", conn, trans);
                            cmdUp.Parameters.AddWithValue("@sl", dr["SoLuong"]);
                            cmdUp.Parameters.AddWithValue("@ms", dr["MaSach"]);
                            cmdUp.ExecuteNonQuery();
                        }

                        trans.Commit();
                        MessageBox.Show("Thanh toán thành công!");

                        AppLogger.GhiLog(Session.Username, "Tạo Hóa Đơn", $"Lập thành công hóa đơn {txtMaHD.Text} - Tổng tiền: {thanhTien.ToString("N0")} VNĐ");

                        ResetForm();
                        LoadComboBoxSach();
                        LoadLichSuHoaDon();
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        MessageBox.Show("Lỗi CSDL khi lưu hóa đơn: " + ex.Message);
                    }
                }
                catch (Exception ex) { MessageBox.Show("Lỗi kết nối: " + ex.Message); }
            }
        }

        private void btnHuyDon_Click(object sender, EventArgs e)
        {
            if (dtGioHang.Rows.Count > 0)
            {
                DialogResult result = MessageBox.Show("Đại ca có chắc chắn muốn hủy toàn bộ đơn hàng đang tạo này không?", "Xác nhận hủy", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes) ResetForm();
            }
            else ResetForm();
        }

        // =========================================================================
        // FIX: CÂU TRUY VẤN SQL BỔ SUNG CỘT GIẢM GIÁ & THỰC THU CHO BẢNG LỊCH SỬ
        // =========================================================================
        private void LoadLichSuHoaDon(bool filter = false)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string sql = @"SELECT h.MaHD, h.NgayLap, n.HoTen as NhanVien, h.TenKhachHang, 
                                          s.TenSach, ct.SoLuong, ct.DonGia, ct.ThanhTien, 
                                          h.TongTien, h.GiamGia, h.ThanhTien as ThucThu 
                                   FROM HoaDon h 
                                   JOIN NguoiDung n ON h.MaND = n.MaND 
                                   JOIN ChiTietHoaDon ct ON h.MaHD = ct.MaHD
                                   JOIN Sach s ON ct.MaSach = s.MaSach
                                   WHERE 1=1";

                    if (filter)
                    {
                        sql += " AND h.NgayLap BETWEEN @tuNgay AND @denNgay";
                        if (!string.IsNullOrWhiteSpace(txtSearch.Text))
                            sql += " AND (h.MaHD LIKE @search OR h.TenKhachHang LIKE @search OR s.TenSach LIKE @search)";
                    }

                    sql += " ORDER BY h.NgayLap DESC";

                    SqlCommand cmd = new SqlCommand(sql, conn);

                    if (filter)
                    {
                        cmd.Parameters.AddWithValue("@tuNgay", dtpTuNgay.Value.Date);
                        cmd.Parameters.AddWithValue("@denNgay", dtpDenNgay.Value.Date.AddDays(1).AddSeconds(-1));
                        if (!string.IsNullOrWhiteSpace(txtSearch.Text))
                            cmd.Parameters.AddWithValue("@search", "%" + txtSearch.Text.Trim() + "%");
                    }

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgvHoaDon.DataSource = dt;
                }
                catch (Exception ex) { MessageBox.Show("Lỗi load lịch sử: " + ex.Message); }
            }
        }

        private void dgvHoaDon_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvHoaDon.Rows[e.RowIndex];

                if (row.IsNewRow || row.Cells["MaHD"].Value == null) return;

                textBox1.Text = row.Cells["TenSach"].Value?.ToString() ?? "";
                textBox2.Text = row.Cells["SoLuong"].Value?.ToString() ?? "0";

                double donGia = 0;
                double thanhTienSach = 0;

                if (row.Cells["DonGia"].Value != null) double.TryParse(row.Cells["DonGia"].Value.ToString(), out donGia);

                // Lấy tiền của cuốc sách đó thôi (Cột ThanhTien của ChiTietHoaDon)
                if (row.Cells["ThanhTien"].Value != null) double.TryParse(row.Cells["ThanhTien"].Value.ToString(), out thanhTienSach);

                textBox3.Text = donGia.ToString("N0");
                textBox4.Text = thanhTienSach.ToString("N0");
            }
        }

        private void BtnSearch_Click(object sender, EventArgs e) { LoadLichSuHoaDon(true); }

        private void btnResetFilter_Click(object sender, EventArgs e)
        {
            txtSearch.Clear();
            dtpTuNgay.Value = DateTime.Now;
            dtpDenNgay.Value = DateTime.Now;
            LoadLichSuHoaDon();
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            if (dgvHoaDon.CurrentRow == null || dgvHoaDon.CurrentRow.IsNewRow) return;

            var cellMaHD = dgvHoaDon.CurrentRow.Cells["MaHD"].Value;
            if (cellMaHD == null) return;

            string maHD = cellMaHD.ToString();

            if (MessageBox.Show($"Xác nhận HỦY TOÀN BỘ hóa đơn {maHD} và HOÀN KHO?", "Cảnh báo", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlTransaction trans = conn.BeginTransaction();
                    try
                    {
                        DataTable dt = new DataTable();
                        SqlCommand cmdLayCT = new SqlCommand("SELECT MaSach, SoLuong FROM ChiTietHoaDon WHERE MaHD=@ma", conn, trans);
                        cmdLayCT.Parameters.AddWithValue("@ma", maHD);
                        SqlDataAdapter da = new SqlDataAdapter(cmdLayCT);
                        da.Fill(dt);

                        foreach (DataRow r in dt.Rows)
                        {
                            SqlCommand cmdUp = new SqlCommand("UPDATE Sach SET SoLuongTon = SoLuongTon + @sl WHERE MaSach=@ms", conn, trans);
                            cmdUp.Parameters.AddWithValue("@sl", r["SoLuong"]);
                            cmdUp.Parameters.AddWithValue("@ms", r["MaSach"]);
                            cmdUp.ExecuteNonQuery();
                        }

                        SqlCommand cmdDelCT = new SqlCommand("DELETE FROM ChiTietHoaDon WHERE MaHD=@ma", conn, trans);
                        cmdDelCT.Parameters.AddWithValue("@ma", maHD);
                        cmdDelCT.ExecuteNonQuery();

                        SqlCommand cmdDelHD = new SqlCommand("DELETE FROM HoaDon WHERE MaHD=@ma", conn, trans);
                        cmdDelHD.Parameters.AddWithValue("@ma", maHD);
                        cmdDelHD.ExecuteNonQuery();

                        trans.Commit();
                        MessageBox.Show("Hủy hóa đơn thành công!");

                        AppLogger.GhiLog(Session.Username, "Hủy Hóa Đơn", $"Đã hủy hóa đơn {maHD} và hoàn lại sách vào kho");

                        LoadLichSuHoaDon();
                        LoadComboBoxSach();
                    }
                    catch (Exception ex) { trans.Rollback(); MessageBox.Show("Lỗi: " + ex.Message); }
                }
            }
        }

        // =========================================================================
        // HÀM IN HÓA ĐƠN PDF CẬP NHẬT GIAO DIỆN 3 DÒNG: TỔNG GỐC - GIẢM - THỰC THU
        // =========================================================================
        private void btnIn_Click(object sender, EventArgs e)
        {
            if (dgvHoaDon.CurrentRow == null || dgvHoaDon.CurrentRow.IsNewRow || dgvHoaDon.CurrentRow.Cells["MaHD"].Value == null)
            {
                MessageBox.Show("Vui lòng click chọn một hóa đơn hợp lệ trong bảng Lịch sử Hóa đơn để in!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string maHD = dgvHoaDon.CurrentRow.Cells["MaHD"].Value.ToString();
            if (string.IsNullOrWhiteSpace(maHD)) return;

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "PDF Document (*.pdf)|*.pdf";
            sfd.FileName = "HoaDon_" + maHD + "_" + DateTime.Now.ToString("ddMMyyyy_HHmm") + ".pdf";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string fontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "arial.ttf");
                    if (!File.Exists(fontPath)) fontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "times.ttf");

                    if (!File.Exists(fontPath))
                    {
                        MessageBox.Show("Máy tính của bạn thiếu Font hệ thống để in tiếng Việt!", "Lỗi Font", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    iTextSharp.text.pdf.BaseFont bf = iTextSharp.text.pdf.BaseFont.CreateFont(fontPath, iTextSharp.text.pdf.BaseFont.IDENTITY_H, iTextSharp.text.pdf.BaseFont.EMBEDDED);
                    iTextSharp.text.Font fontTitle = new iTextSharp.text.Font(bf, 18, iTextSharp.text.Font.BOLD);
                    iTextSharp.text.Font fontBold = new iTextSharp.text.Font(bf, 11, iTextSharp.text.Font.BOLD);
                    iTextSharp.text.Font fontNormal = new iTextSharp.text.Font(bf, 11, iTextSharp.text.Font.NORMAL);

                    using (FileStream fs = new FileStream(sfd.FileName, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        iTextSharp.text.Rectangle pageSizeA5 = new iTextSharp.text.Rectangle(420f, 595f);
                        iTextSharp.text.Document doc = new iTextSharp.text.Document(pageSizeA5, 20f, 20f, 30f, 20f);

                        iTextSharp.text.pdf.PdfWriter.GetInstance(doc, fs);
                        doc.Open();

                        var rowHD = dgvHoaDon.CurrentRow;
                        string ngayLap = "";
                        if (rowHD.Cells["NgayLap"].Value != null)
                        {
                            DateTime dtNgayLap;
                            if (DateTime.TryParse(rowHD.Cells["NgayLap"].Value.ToString(), out dtNgayLap))
                                ngayLap = dtNgayLap.ToString("dd/MM/yyyy HH:mm");
                        }

                        string nhanVien = rowHD.Cells["NhanVien"].Value?.ToString() ?? "";
                        string khachHang = rowHD.Cells["TenKhachHang"].Value?.ToString() ?? "Khách lẻ";

                        // Lấy 3 thông số tiền
                        double tongTienNum = 0, giamGiaNum = 0, thucThuNum = 0;
                        if (rowHD.Cells["TongTien"].Value != null) double.TryParse(rowHD.Cells["TongTien"].Value.ToString(), out tongTienNum);
                        if (rowHD.Cells["GiamGia"].Value != null) double.TryParse(rowHD.Cells["GiamGia"].Value.ToString(), out giamGiaNum);
                        if (rowHD.Cells["ThucThu"].Value != null) double.TryParse(rowHD.Cells["ThucThu"].Value.ToString(), out thucThuNum);

                        iTextSharp.text.Paragraph shopName = new iTextSharp.text.Paragraph("CỬA HÀNG SÁCH DT STORE", fontBold) { Alignment = iTextSharp.text.Element.ALIGN_CENTER };
                        iTextSharp.text.Paragraph title = new iTextSharp.text.Paragraph("HÓA ĐƠN THANH TOÁN", fontTitle) { Alignment = iTextSharp.text.Element.ALIGN_CENTER };
                        title.SpacingAfter = 15;
                        doc.Add(shopName);
                        doc.Add(title);

                        doc.Add(new iTextSharp.text.Paragraph($"Mã HĐ: {maHD}", fontNormal));
                        doc.Add(new iTextSharp.text.Paragraph($"Ngày lập: {ngayLap}", fontNormal));
                        doc.Add(new iTextSharp.text.Paragraph($"Thu ngân: {nhanVien}", fontNormal));
                        doc.Add(new iTextSharp.text.Paragraph($"Khách hàng: {khachHang}", fontNormal));
                        doc.Add(new iTextSharp.text.Paragraph("----------------------------------------------------------------------", fontNormal) { Alignment = iTextSharp.text.Element.ALIGN_CENTER, SpacingAfter = 10 });

                        iTextSharp.text.pdf.PdfPTable table = new iTextSharp.text.pdf.PdfPTable(4);
                        table.WidthPercentage = 100;
                        table.SetWidths(new float[] { 4.5f, 1f, 2.5f, 2.5f });

                        table.AddCell(new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Tên sách", fontBold)) { Border = 0, PaddingBottom = 5 });
                        table.AddCell(new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("SL", fontBold)) { Border = 0, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER });
                        table.AddCell(new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Đơn giá", fontBold)) { Border = 0, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT });
                        table.AddCell(new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Tiền sách", fontBold)) { Border = 0, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT });

                        foreach (DataGridViewRow r in dgvHoaDon.Rows)
                        {
                            if (r.IsNewRow) continue;

                            var cellMaHD = r.Cells["MaHD"].Value;
                            if (cellMaHD != null && cellMaHD.ToString() == maHD)
                            {
                                string tenSach = r.Cells["TenSach"].Value?.ToString() ?? "";
                                string soLuong = r.Cells["SoLuong"].Value?.ToString() ?? "0";

                                double donGia = 0;
                                if (r.Cells["DonGia"].Value != null) double.TryParse(r.Cells["DonGia"].Value.ToString(), out donGia);

                                double tienSach = 0;
                                if (r.Cells["ThanhTien"].Value != null) double.TryParse(r.Cells["ThanhTien"].Value.ToString(), out tienSach);

                                table.AddCell(new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(tenSach, fontNormal)) { Border = 0, PaddingBottom = 5 });
                                table.AddCell(new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(soLuong, fontNormal)) { Border = 0, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER });
                                table.AddCell(new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(donGia.ToString("N0"), fontNormal)) { Border = 0, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT });
                                table.AddCell(new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(tienSach.ToString("N0"), fontNormal)) { Border = 0, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT });
                            }
                        }

                        doc.Add(table);
                        doc.Add(new iTextSharp.text.Paragraph("----------------------------------------------------------------------", fontNormal) { Alignment = iTextSharp.text.Element.ALIGN_CENTER });

                        // THAY ĐỔI ĐỂ IN 3 DÒNG RÕ RÀNG
                        iTextSharp.text.Paragraph txtTongTien = new iTextSharp.text.Paragraph($"TỔNG TIỀN GỐC: {tongTienNum.ToString("N0")} VNĐ", fontNormal) { Alignment = iTextSharp.text.Element.ALIGN_RIGHT, SpacingBefore = 5 };
                        iTextSharp.text.Paragraph txtGiamGia = new iTextSharp.text.Paragraph($"GIẢM GIÁ: -{giamGiaNum.ToString("N0")} VNĐ", fontNormal) { Alignment = iTextSharp.text.Element.ALIGN_RIGHT };
                        iTextSharp.text.Paragraph txtThucThu = new iTextSharp.text.Paragraph($"THỰC THU (KHÁCH TRẢ): {thucThuNum.ToString("N0")} VNĐ", fontBold) { Alignment = iTextSharp.text.Element.ALIGN_RIGHT, SpacingAfter = 20 };

                        doc.Add(txtTongTien);
                        doc.Add(txtGiamGia);
                        doc.Add(txtThucThu);

                        iTextSharp.text.Paragraph footer = new iTextSharp.text.Paragraph("Cảm ơn Quý Khách & Hẹn gặp lại!", fontNormal) { Alignment = iTextSharp.text.Element.ALIGN_CENTER };
                        doc.Add(footer);

                        doc.Close();
                    }

                    MessageBox.Show("Đã xuất Hóa đơn PDF thành công xuất sắc!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    try
                    {
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
                        {
                            FileName = sfd.FileName,
                            UseShellExecute = true
                        });
                    }
                    catch
                    {
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi trong quá trình tạo PDF:\n" + ex.Message, "Lỗi Hệ Thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            if (dtGioHang.Rows.Count > 0) { MessageBox.Show("Vui lòng bấm Thanh toán trước khi in!"); return; }
            MessageBox.Show("In hóa đơn thành công: " + txtMaHD.Text);
        }

        private void btnReset_Click(object sender, EventArgs e) { ResetForm(); }

        private void ResetForm()
        {
            txtMaHD.Text = TuSinhMaHD();
            txtTenKH.Clear();
            cboMaNV.SelectedIndex = -1;
            cboMaSach.SelectedIndex = -1;
            nudSoLuong.Value = 1;
            txtDonGia.Clear();

            if (cboGiamGia != null && cboGiamGia.Items.Count > 0) cboGiamGia.SelectedIndex = 0;

            txtTongTien.Text = "0";
            txtThanhTien.Text = "0";
            dtGioHang.Clear();
        }

        private void guna2TabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (guna2TabControl1.SelectedIndex == 1) LoadLichSuHoaDon();
        }
    }
}