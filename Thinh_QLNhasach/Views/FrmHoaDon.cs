using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Printing; // Thư viện in ấn gốc của WinForms
using System.Windows.Forms;
using FontAwesome.Sharp;
using Thinh_QLNhasach.Utility;
using System.IO;
using System.Net; // Thư viện dùng để tải ảnh VietQR từ Internet

namespace Thinh_QLNhasach.Views
{
    public partial class FrmHoaDon : Form
    {
        string connectionString = @"Data Source=.\SQLEXPRESS01;Initial Catalog=BookShop;Integrated Security=True";
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
                // Bắt cứng sự kiện thay đổi Giảm Giá bằng Code để đảm bảo 100% luôn chạy
                cboGiamGia.SelectedIndexChanged -= cboGiamGia_SelectedIndexChanged;

                cboGiamGia.Items.Clear();
                cboGiamGia.Items.AddRange(new string[] { "0%", "5%", "10%", "15%" });
                cboGiamGia.DropDownStyle = ComboBoxStyle.DropDownList;
                cboGiamGia.SelectedIndex = 0;

                // Nối lại sự kiện sau khi set Value để sẵn sàng bắt tương tác của User
                cboGiamGia.SelectedIndexChanged += cboGiamGia_SelectedIndexChanged;
            }

            LoadComboBoxNguoiDung();

            // ĐÃ THÊM: Tải Thể Loại trước
            LoadComboBoxTheLoai();

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

                // BẢNG LỊCH SỬ HÓA ĐƠN ĐÃ GỘP
                if (e.Column.Name == "MaHD") { e.Column.HeaderText = "Mã HD"; e.Column.FillWeight = 80; }
                if (e.Column.Name == "NgayLap") { e.Column.HeaderText = "Ngày Lập"; e.Column.DefaultCellStyle.Format = "dd/MM/yyyy HH:mm"; }
                if (e.Column.Name == "NhanVien") { e.Column.HeaderText = "Nhân Viên"; e.Column.FillWeight = 120; }
                if (e.Column.Name == "TenKhachHang") { e.Column.HeaderText = "Khách Hàng"; e.Column.FillWeight = 120; }
                if (e.Column.Name == "TenSach" && dgv.Name == "dgvHoaDon") { e.Column.HeaderText = "Sách Đã Mua"; e.Column.FillWeight = 220; }
                if (e.Column.Name == "SoLuong" && dgv.Name == "dgvHoaDon") { e.Column.HeaderText = "Tổng SL"; e.Column.FillWeight = 50; e.Column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter; }

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

        // =========================================================================
        // ĐÃ THÊM: HÀM LOAD COMBOBOX THỂ LOẠI (LỌC SÁCH LÚC BÁN HÀNG)
        // =========================================================================
        private void LoadComboBoxTheLoai()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    SqlDataAdapter da = new SqlDataAdapter("SELECT MaTL, TenTL FROM TheLoai", conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    // Chèn thêm dòng Tất cả sách lên đầu
                    DataRow row = dt.NewRow();
                    row["MaTL"] = 0;
                    row["TenTL"] = "--- Tất cả sách ---";
                    dt.Rows.InsertAt(row, 0);

                    if (this.Controls.Find("cboTheLoai", true).Length > 0)
                    {
                        ComboBox cboTL = (ComboBox)this.Controls.Find("cboTheLoai", true)[0];

                        // Tắt event trước khi đổ data để tránh lỗi
                        cboTL.SelectedIndexChanged -= cboTheLoai_SelectedIndexChanged;

                        cboTL.DataSource = dt;
                        cboTL.DisplayMember = "TenTL";
                        cboTL.ValueMember = "MaTL";
                        cboTL.SelectedIndex = 0;

                        // Bật lại event
                        cboTL.SelectedIndexChanged += cboTheLoai_SelectedIndexChanged;
                    }
                }
                catch (Exception ex) { MessageBox.Show("Lỗi load Thể loại: " + ex.Message); }
            }
        }

        // =========================================================================
        // ĐÃ SỬA: LOAD SÁCH THEO THỂ LOẠI (CHỈ LOAD SÁCH CÒN TỒN KHO > 0)
        // =========================================================================
        private void LoadComboBoxSach(int maTL = 0)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    // Khi bán hàng, chỉ lấy sách có số lượng > 0
                    string sql = "SELECT MaSach, TenSach, GiaBan, SoLuongTon FROM Sach WHERE SoLuongTon > 0";

                    if (maTL > 0)
                    {
                        sql += " AND MaTL = " + maTL;
                    }

                    SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    cboMaSach.SelectedIndexChanged -= cboMaSach_SelectedIndexChanged;

                    cboMaSach.DataSource = null;
                    this.BindingContext = new BindingContext();
                    cboMaSach.DataSource = dt;
                    cboMaSach.DisplayMember = "TenSach";
                    cboMaSach.ValueMember = "MaSach";

                    if (cboMaSach.Items.Count > 0) cboMaSach.SelectedIndex = -1;

                    cboMaSach.SelectedIndexChanged += cboMaSach_SelectedIndexChanged;
                }
                catch (Exception ex) { MessageBox.Show("Lỗi tải danh sách sách: " + ex.Message); }
            }
        }

        // =========================================================================
        // ĐÃ THÊM: SỰ KIỆN KHI ĐỔI THỂ LOẠI THÌ LỌC LẠI SÁCH
        // =========================================================================
        private void cboTheLoai_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cboTL = sender as ComboBox;
            if (cboTL != null && cboTL.SelectedValue != null)
            {
                int maTL = 0;
                int.TryParse(cboTL.SelectedValue.ToString(), out maTL);

                LoadComboBoxSach(maTL);

                // Khi vừa đổi thể loại thì xóa trắng ô đơn giá
                txtDonGia.Clear();
                nudSoLuong.Value = 1;
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
                        LoadComboBoxSach(); // Tải lại giỏ sách để cập nhật Tồn kho
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
                DialogResult result = MessageBox.Show("Bạn có chắc muốn hủy đơn hàng đang tạo này không?", "Xác nhận hủy", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes) ResetForm();
            }
            else ResetForm();
        }

        private void LoadLichSuHoaDon(bool filter = false)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string sql = @"
                        SELECT 
                            h.MaHD, 
                            h.NgayLap, 
                            n.HoTen as NhanVien, 
                            h.TenKhachHang, 
                            STUFF((SELECT ', ' + s.TenSach FROM ChiTietHoaDon ct JOIN Sach s ON ct.MaSach = s.MaSach WHERE ct.MaHD = h.MaHD FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 2, '') AS TenSach,
                            (SELECT SUM(SoLuong) FROM ChiTietHoaDon WHERE MaHD = h.MaHD) AS SoLuong,
                            h.TongTien, h.GiamGia, h.ThanhTien as ThucThu 
                        FROM HoaDon h 
                        JOIN NguoiDung n ON h.MaND = n.MaND 
                        WHERE 1=1";

                    if (filter)
                    {
                        sql += " AND h.NgayLap BETWEEN @tuNgay AND @denNgay";
                        if (!string.IsNullOrWhiteSpace(txtSearch.Text))
                        {
                            sql += " AND (h.MaHD LIKE @search OR h.TenKhachHang LIKE @search OR (SELECT COUNT(*) FROM ChiTietHoaDon ct JOIN Sach s ON ct.MaSach=s.MaSach WHERE ct.MaHD=h.MaHD AND s.TenSach LIKE @search) > 0)";
                        }
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

                textBox3.Text = "---";

                double tongTienHD = 0;
                if (row.Cells["TongTien"].Value != null) double.TryParse(row.Cells["TongTien"].Value.ToString(), out tongTienHD);
                textBox4.Text = tongTienHD.ToString("N0");
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

        private void btnIn_Click(object sender, EventArgs e)
        {
            if (dgvHoaDon.CurrentRow == null || dgvHoaDon.CurrentRow.IsNewRow || dgvHoaDon.CurrentRow.Cells["MaHD"].Value == null)
            {
                MessageBox.Show("Vui lòng click chọn một hóa đơn hợp lệ trong bảng Lịch sử Hóa đơn để in!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            PrintPreviewDialog ppd = new PrintPreviewDialog();
            PrintDocument pd = new PrintDocument();

            pd.PrintPage += new PrintPageEventHandler(InHoaDon_PrintPage);

            ppd.Document = pd;
            ppd.Width = 800;
            ppd.Height = 1000;
            ppd.ShowDialog();
        }

        private void InHoaDon_PrintPage(object sender, PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;
            Font fontTitle = new Font("Segoe UI", 22, FontStyle.Bold);
            Font fontHeader = new Font("Segoe UI", 12, FontStyle.Bold);
            Font fontNormal = new Font("Segoe UI", 12, FontStyle.Regular);
            Font fontItalic = new Font("Segoe UI", 11, FontStyle.Italic);
            Font fontTongTien = new Font("Segoe UI", 16, FontStyle.Bold);

            StringFormat centerFormat = new StringFormat();
            centerFormat.Alignment = StringAlignment.Center;
            int centerX = e.PageBounds.Width / 2;

            int y = 50;
            int left = 50;

            g.DrawString("CỬA HÀNG SÁCH DT STORE", fontHeader, Brushes.Black, left, y);
            y += 25;
            g.DrawString("Địa chỉ: Hà Nội - Điện thoại: 0987.654.321", fontNormal, Brushes.Black, left, y);
            y += 60;

            g.DrawString("HÓA ĐƠN BÁN HÀNG", fontTitle, Brushes.Black, centerX, y, centerFormat);
            y += 50;

            DataGridViewRow rowHD = dgvHoaDon.CurrentRow;
            string maHD = rowHD.Cells["MaHD"].Value.ToString();

            string ngayLap = "";
            if (rowHD.Cells["NgayLap"].Value != null)
            {
                DateTime dtNgayLap;
                if (DateTime.TryParse(rowHD.Cells["NgayLap"].Value.ToString(), out dtNgayLap))
                    ngayLap = dtNgayLap.ToString("dd/MM/yyyy HH:mm");
            }

            string nhanVien = rowHD.Cells["NhanVien"].Value?.ToString() ?? "";
            string khachHang = rowHD.Cells["TenKhachHang"].Value?.ToString() ?? "Khách lẻ";

            g.DrawString("Mã HĐ: " + maHD, fontNormal, Brushes.Black, left, y);
            y += 25;
            g.DrawString("Ngày lập: " + ngayLap, fontNormal, Brushes.Black, left, y);
            y += 25;
            g.DrawString("Thu ngân: " + nhanVien, fontNormal, Brushes.Black, left, y);
            y += 25;
            g.DrawString("Khách hàng: " + khachHang, fontNormal, Brushes.Black, left, y);
            y += 40;

            g.DrawLine(Pens.Black, left, y, 780, y);
            y += 10;
            g.DrawString("STT", fontHeader, Brushes.Black, left, y);
            g.DrawString("Tên Sách", fontHeader, Brushes.Black, left + 50, y);
            g.DrawString("SL", fontHeader, Brushes.Black, left + 400, y);
            g.DrawString("Đơn Giá", fontHeader, Brushes.Black, left + 480, y);
            g.DrawString("Thành Tiền", fontHeader, Brushes.Black, left + 620, y);
            y += 25;
            g.DrawLine(Pens.Black, left, y, 780, y);
            y += 15;

            int stt = 1;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string sqlCT = "SELECT s.TenSach, ct.SoLuong, ct.DonGia, ct.ThanhTien FROM ChiTietHoaDon ct JOIN Sach s ON ct.MaSach = s.MaSach WHERE ct.MaHD = @maHD";
                SqlCommand cmdCT = new SqlCommand(sqlCT, conn);
                cmdCT.Parameters.AddWithValue("@maHD", maHD);

                using (SqlDataReader reader = cmdCT.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        g.DrawString(stt.ToString(), fontNormal, Brushes.Black, left, y);

                        string tenSach = reader["TenSach"].ToString();
                        if (tenSach.Length > 35) tenSach = tenSach.Substring(0, 35) + "...";
                        g.DrawString(tenSach, fontNormal, Brushes.Black, left + 50, y);

                        g.DrawString(reader["SoLuong"].ToString(), fontNormal, Brushes.Black, left + 400, y);
                        g.DrawString(Convert.ToDouble(reader["DonGia"]).ToString("N0"), fontNormal, Brushes.Black, left + 480, y);
                        g.DrawString(Convert.ToDouble(reader["ThanhTien"]).ToString("N0"), fontNormal, Brushes.Black, left + 620, y);

                        stt++;
                        y += 30;
                    }
                }
            }

            y += 10;
            g.DrawLine(Pens.Black, left, y, 780, y);
            y += 20;

            double tongTienNum = 0, giamGiaNum = 0, thucThuNum = 0;
            if (rowHD.Cells["TongTien"].Value != null) double.TryParse(rowHD.Cells["TongTien"].Value.ToString(), out tongTienNum);
            if (rowHD.Cells["GiamGia"].Value != null) double.TryParse(rowHD.Cells["GiamGia"].Value.ToString(), out giamGiaNum);
            if (rowHD.Cells["ThucThu"].Value != null) double.TryParse(rowHD.Cells["ThucThu"].Value.ToString(), out thucThuNum);

            g.DrawString("Tổng tiền gốc: " + tongTienNum.ToString("N0") + " VNĐ", fontNormal, Brushes.Black, left + 420, y);
            y += 25;
            g.DrawString("Giảm giá: -" + giamGiaNum.ToString("N0") + " VNĐ", fontNormal, Brushes.Black, left + 420, y);
            y += 35;
            g.DrawString("THỰC THU: " + thucThuNum.ToString("N0") + " VNĐ", fontTongTien, Brushes.Black, left + 400, y);
            y += 60;

            try
            {
                string soTienNguyen = Math.Round(thucThuNum).ToString("0");

                string urlQR = $"https://img.vietqr.io/image/TCB-8888332999-compact2.png?amount={soTienNguyen}&addInfo=THANH TOAN HD {maHD}";

                using (WebClient wc = new WebClient())
                {
                    byte[] bytes = wc.DownloadData(urlQR);
                    using (MemoryStream ms = new MemoryStream(bytes))
                    {
                        Image imgQR = Image.FromStream(ms);

                        g.DrawString("--- QUÉT MÃ ĐỂ THANH TOÁN ---", fontItalic, Brushes.Black, centerX, y, centerFormat);
                        y += 30;

                        int qrSize = 280;
                        g.DrawImage(imgQR, centerX - (qrSize / 2), y, qrSize, qrSize);

                        y += qrSize + 25;
                    }
                }
            }
            catch
            {
            }

            g.DrawString("Cảm ơn Quý Khách & Hẹn gặp lại!", fontItalic, Brushes.Black, centerX, y, centerFormat);
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

            // ĐÃ THÊM: Khi tạo phiếu mới thì mặc định nhảy về "Tất cả sách" để khỏi bị lọc sai
            if (this.Controls.Find("cboTheLoai", true).Length > 0)
            {
                ComboBox cboTL = (ComboBox)this.Controls.Find("cboTheLoai", true)[0];
                if (cboTL.Items.Count > 0) cboTL.SelectedIndex = 0;
            }

            cboMaSach.SelectedIndex = -1;
            nudSoLuong.Value = 1;
            txtDonGia.Clear();

            dtGioHang.Clear();

            if (cboGiamGia != null && cboGiamGia.Items.Count > 0)
                cboGiamGia.SelectedIndex = 0;

            CapNhatTien();
        }

        private void guna2TabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (guna2TabControl1.SelectedIndex == 1) LoadLichSuHoaDon();
        }
    }
}