using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Printing; // Thư viện dùng để in ấn
using System.Windows.Forms;
using Thinh_QLNhasach.Utility; // Khai báo để lấy tên người dùng từ Session

namespace Thinh_QLNhasach
{
    public partial class FrmNhapHang : Form
    {
        string connectionString = @"Data Source=.\SQLEXPRESS01;Initial Catalog=BookShop;Integrated Security=True";

        DataTable dtGioHang = new DataTable();
        private bool isEditingPhieu = false;

        DataTable dtNCCTam = new DataTable();
        private int indexChonNCC = -1;
        private bool isEditingNCC = false;

        public FrmNhapHang()
        {
            InitializeComponent();
        }

        private void FrmNhapHang_Load(object sender, EventArgs e)
        {
            FormatDataGridView(dgvPhieuNhap);
            FormatDataGridView(dgvNCC);

            // BẢN NÂNG CẤP: Thêm cột Giá Bán vào giỏ hàng
            if (dtGioHang.Columns.Count == 0)
            {
                dtGioHang.Columns.Add("MaSach", typeof(string));
                dtGioHang.Columns.Add("TenSach", typeof(string));
                dtGioHang.Columns.Add("SoLuong", typeof(int));
                dtGioHang.Columns.Add("DonGiaNhap", typeof(double));
                dtGioHang.Columns.Add("GiaBan", typeof(double)); // <-- Cột mới thêm
                dtGioHang.Columns.Add("ThanhTien", typeof(double), "SoLuong * DonGiaNhap");
            }

            SetupDataTableNCC();

            LoadData();
            LoadComboBoxNCC();
            LoadComboBoxTheLoai(); // ĐÃ THÊM: Tải danh sách Thể loại trước
            LoadComboBoxSach();
            LoadComboBoxNguoiDung();
            LoadDataNhaCungCap();
            ResetTabLapPhieu();

            dgvPhieuNhap.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvNCC.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        // =========================================================================
        // NÚT GỌI FORM TẠO SÁCH MỚI
        // =========================================================================
        private void btnTaoSachMoi_Click(object sender, EventArgs e)
        {
            Views.FrmTaoSachMoi frm = new Views.FrmTaoSachMoi(); // Gọi đúng Namespace Views
            if (frm.ShowDialog() == DialogResult.OK)
            {
                LoadComboBoxSach();
                cboMaSach.SelectedValue = frm.MaSachVuaTao;
                txtDongianhap.Clear();
                txtGiaBan.Clear();
                MessageBox.Show("Hãy nhập Số Lượng, Giá Nhập và Giá Bán cho sách mới rồi bấm Thêm (+)", "Hướng dẫn", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private string TuSinhMaPhieu()
        {
            string maMoi = "PN001";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string sql = "SELECT TOP 1 MaPN FROM PhieuNhap ORDER BY MaPN DESC";
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    object result = cmd.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        string maCu = result.ToString();
                        int soHienTai = int.Parse(maCu.Substring(2));
                        maMoi = "PN" + (soHienTai + 1).ToString("D3");
                    }
                }
                catch { maMoi = "PN001"; }
            }
            return maMoi;
        }

        private void FormatDataGridView(DataGridView dgv)
        {
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.Gray;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            dgv.ColumnHeadersHeight = 35;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            dgv.GridColor = System.Drawing.Color.Black;
            dgv.RowHeadersVisible = false;
            dgv.BackgroundColor = System.Drawing.Color.White;
            dgv.BorderStyle = BorderStyle.FixedSingle;
            dgv.DefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 10F);
            dgv.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(231, 229, 255);
            dgv.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.Black;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.RowTemplate.Height = 33;

            if (dgv.Name == "dgvPhieuNhap")
            {
                dgv.ColumnAdded += (s, e) =>
                {
                    if (e.Column.Name == "GiaBan") e.Column.HeaderText = "Giá Bán Cập Nhật";
                    if (e.Column.Name == "DonGiaNhap") e.Column.HeaderText = "Giá Nhập";
                };
            }
        }

        private void LoadData()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                // ======================================================================================
                // ĐÃ FIX SQL: Dùng STUFF gom nhóm tất cả sách vào cột ChiTietSach, 1 Phiếu = 1 Dòng
                // ======================================================================================
                string sql = @"
                    SELECT 
                        pn.MaPN, 
                        pn.NgayNhap, 
                        ncc.TenNCC, 
                        nd.HoTen AS NguoiLap,
                        (
                            SELECT STUFF((
                                SELECT ', ' + s.TenSach + ' (SL: ' + CAST(ct.SoLuong AS VARCHAR) + ')'
                                FROM ChiTietPhieuNhap ct
                                JOIN Sach s ON ct.MaSach = s.MaSach
                                WHERE ct.MaPN = pn.MaPN
                                FOR XML PATH('')
                            ), 1, 2, '')
                        ) AS ChiTietSach,
                        pn.TongTien, 
                        pn.GhiChu, 
                        pn.MaNCC, 
                        pn.MaND 
                    FROM PhieuNhap pn
                    LEFT JOIN NguoiDung nd ON pn.MaND = nd.MaND
                    LEFT JOIN NhaCungCap ncc ON pn.MaNCC = ncc.MaNCC
                    ORDER BY pn.NgayNhap DESC";

                SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dgvPhieuNhap.DataSource = dt;

                if (dgvPhieuNhap.Columns.Count > 0)
                {
                    // Ẩn ID
                    if (dgvPhieuNhap.Columns.Contains("MaNCC")) dgvPhieuNhap.Columns["MaNCC"].Visible = false;
                    if (dgvPhieuNhap.Columns.Contains("MaND")) dgvPhieuNhap.Columns["MaND"].Visible = false;

                    // Đổi tên cột
                    dgvPhieuNhap.Columns["MaPN"].HeaderText = "Mã Phiếu";
                    dgvPhieuNhap.Columns["MaPN"].FillWeight = 50;

                    dgvPhieuNhap.Columns["NgayNhap"].HeaderText = "Ngày Nhập";
                    dgvPhieuNhap.Columns["NgayNhap"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
                    dgvPhieuNhap.Columns["NgayNhap"].FillWeight = 80;

                    dgvPhieuNhap.Columns["TenNCC"].HeaderText = "Nhà Cung Cấp";
                    dgvPhieuNhap.Columns["NguoiLap"].HeaderText = "Người Lập";

                    dgvPhieuNhap.Columns["ChiTietSach"].HeaderText = "Sách Đã Nhập (Gộp)";
                    dgvPhieuNhap.Columns["ChiTietSach"].FillWeight = 150; // Cho cột này rộng ra để đọc được nhiều sách

                    dgvPhieuNhap.Columns["TongTien"].HeaderText = "Tổng Tiền";
                    dgvPhieuNhap.Columns["TongTien"].DefaultCellStyle.Format = "N0";
                    dgvPhieuNhap.Columns["GhiChu"].HeaderText = "Ghi Chú";
                }
            }
        }

        private void BocDuLieuPhieuNhap(int rowIndex)
        {
            if (rowIndex < 0) return;
            DataGridViewRow r = dgvPhieuNhap.Rows[rowIndex];

            if (dgvPhieuNhap.DataSource == dtGioHang)
            {
                // Khi đang bốc dữ liệu từ Giỏ hàng tạm
                cboMaSach.SelectedValue = r.Cells["MaSach"].Value.ToString();
                nudSoLuong.Value = Convert.ToDecimal(r.Cells["SoLuong"].Value);
                txtDongianhap.Text = r.Cells["DonGiaNhap"].Value.ToString();
                if (r.Cells["GiaBan"] != null) txtGiaBan.Text = r.Cells["GiaBan"].Value.ToString();
            }
            else
            {
                // Khi click vào dòng Lịch sử (Đã gộp sách), chỉ bốc dữ liệu Master lên
                string maPN = r.Cells["MaPN"].Value.ToString();
                txtMaPN.Text = maPN;
                dtpNgayNhap.Value = Convert.ToDateTime(r.Cells["NgayNhap"].Value);

                if (r.Cells["MaNCC"].Value != DBNull.Value) cboMaNCC.SelectedValue = r.Cells["MaNCC"].Value;
                if (r.Cells["MaND"].Value != DBNull.Value) cboMaND.SelectedValue = r.Cells["MaND"].Value;

                txtTongtien.Text = Convert.ToDouble(r.Cells["TongTien"].Value).ToString("N0");
                txtGhichu.Text = r.Cells["GhiChu"].Value.ToString();

                // Xóa trắng ô sách vì dòng lịch sử hiện tại đại diện cho RẤT NHIỀU sách
                cboMaSach.SelectedIndex = -1;
                nudSoLuong.Value = 1;
                txtDongianhap.Clear();
                txtGiaBan.Clear();
                txtThanhTien.Text = "0";

                // Chuyển Tag sang EDIT_MASTER để khi bấm Save nó chỉ update Ghi chú và NCC
                btnSave.Tag = new { Mode = "EDIT_MASTER", MaPN = maPN };
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (dgvPhieuNhap.DataSource != dtGioHang)
            {
                dgvPhieuNhap.DataSource = dtGioHang;
                if (dgvPhieuNhap.Columns.Contains("MaSach")) dgvPhieuNhap.Columns["MaSach"].Visible = true;
            }
            if (cboMaSach.SelectedValue == null) { MessageBox.Show("Vui lòng chọn Sách!"); return; }
            if (string.IsNullOrWhiteSpace(txtGiaBan.Text)) { MessageBox.Show("Vui lòng nhập Giá Bán để cập nhật hệ thống!"); return; }

            double giaNhap = string.IsNullOrEmpty(txtDongianhap.Text) ? 0 : Convert.ToDouble(txtDongianhap.Text);
            double giaBan = Convert.ToDouble(txtGiaBan.Text);

            dtGioHang.Rows.Add(cboMaSach.SelectedValue.ToString(), cboMaSach.Text, nudSoLuong.Value, giaNhap, giaBan);
            CapNhatTongTien();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // === LOGIC SỬA PHIẾU CŨ (CHỈ SỬA MASTER VÌ ĐÃ GỘP PHIẾU) ===
            if (btnSave.Tag != null && btnSave.Tag.ToString().Contains("EDIT_MASTER"))
            {
                dynamic data = btnSave.Tag;
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    try
                    {
                        string updatePN = "UPDATE PhieuNhap SET GhiChu = @ghi, MaNCC = @ncc WHERE MaPN = @ma";
                        SqlCommand cmdPN = new SqlCommand(updatePN, conn);
                        cmdPN.Parameters.AddWithValue("@ghi", txtGhichu.Text);
                        cmdPN.Parameters.AddWithValue("@ncc", cboMaNCC.SelectedValue);
                        cmdPN.Parameters.AddWithValue("@ma", data.MaPN);
                        cmdPN.ExecuteNonQuery();

                        btnSave.Tag = null;
                        isEditingPhieu = false;

                        MessageBox.Show("Cập nhật thôngত্তি phiếu thành công (Chỉ sửa Nhà Cung Cấp & Ghi Chú)!");
                        AppLogger.GhiLog(Session.Username, "Sửa Phiếu Nhập", $"Đã sửa thông tin tổng quan phiếu nhập mã {data.MaPN}");

                        LoadData();
                        ResetTabLapPhieu();
                        return;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi: " + ex.Message);
                        return;
                    }
                }
            }

            // === LOGIC LƯU PHIẾU MỚI (CÓ CẬP NHẬT GIÁ BÁN) ===
            if (dtGioHang.Rows.Count == 0 || cboMaNCC.SelectedValue == null) return;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlTransaction trans = conn.BeginTransaction();
                try
                {
                    SqlCommand cmdPN = new SqlCommand("INSERT INTO PhieuNhap (MaPN, NgayNhap, MaNCC, MaND, TongTien, GhiChu) VALUES (@ma, @ngay, @ncc, @nd, @tong, @ghi)", conn, trans);
                    cmdPN.Parameters.AddWithValue("@ma", txtMaPN.Text);
                    cmdPN.Parameters.AddWithValue("@ngay", dtpNgayNhap.Value);
                    cmdPN.Parameters.AddWithValue("@ncc", cboMaNCC.SelectedValue);
                    cmdPN.Parameters.AddWithValue("@nd", cboMaND.SelectedValue);
                    cmdPN.Parameters.AddWithValue("@tong", Convert.ToDouble(txtTongtien.Text.Replace(",", "")));
                    cmdPN.Parameters.AddWithValue("@ghi", txtGhichu.Text ?? "");
                    cmdPN.ExecuteNonQuery();

                    foreach (DataRow dr in dtGioHang.Rows)
                    {
                        SqlCommand cmdCT = new SqlCommand("INSERT INTO ChiTietPhieuNhap (MaPN, MaSach, SoLuong, DonGiaNhap, ThanhTien) VALUES (@id, @ms, @sl, @dg, @tt)", conn, trans);
                        cmdCT.Parameters.AddWithValue("@id", txtMaPN.Text);
                        cmdCT.Parameters.AddWithValue("@ms", dr["MaSach"]);
                        cmdCT.Parameters.AddWithValue("@sl", dr["SoLuong"]);
                        cmdCT.Parameters.AddWithValue("@dg", dr["DonGiaNhap"]);
                        cmdCT.Parameters.AddWithValue("@tt", dr["ThanhTien"]);
                        cmdCT.ExecuteNonQuery();

                        // NÂNG CẤP: Cập nhật CẢ Giá Nhập và Giá Bán thẳng vào bảng Sách
                        string sqlUpdateSach = $"UPDATE Sach SET SoLuongTon = SoLuongTon + {dr["SoLuong"]}, GiaNhap = {dr["DonGiaNhap"]}, GiaBan = {dr["GiaBan"]} WHERE MaSach='{dr["MaSach"]}'";
                        new SqlCommand(sqlUpdateSach, conn, trans).ExecuteNonQuery();
                    }
                    trans.Commit();
                    MessageBox.Show("Đã chốt phiếu và Cập nhật Tồn kho, Giá bán thành công!");

                    AppLogger.GhiLog(Session.Username, "Lập Phiếu Nhập", $"Đã lập phiếu nhập mới mã {txtMaPN.Text} với tổng tiền {txtTongtien.Text} VNĐ");

                    dtGioHang.Clear();
                    ResetTabLapPhieu();
                    LoadData();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    MessageBox.Show("Lỗi: " + ex.Message);
                }
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dgvPhieuNhap.CurrentRow == null)
            {
                MessageBox.Show("Hãy chọn một dòng dưới bảng trước!");
                return;
            }

            if (!isEditingPhieu)
            {
                isEditingPhieu = true;
                MessageBox.Show("Đã bật chế độ Sửa! Click vào bất kỳ dòng nào dưới bảng để sửa (Chỉ sửa thông tin Nhà Cung Cấp và Ghi Chú).");
                BocDuLieuPhieuNhap(dgvPhieuNhap.CurrentRow.Index);
            }
            else
            {
                isEditingPhieu = false;
                MessageBox.Show("Đã tắt chế độ sửa.");
                ResetTabLapPhieu();
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            dtGioHang.Clear();
            isEditingPhieu = false;
            ResetTabLapPhieu();
            LoadData();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string keyword = txtSearch.Text.Trim().Replace("'", "''");
            DataTable dtCurrent = dgvPhieuNhap.DataSource as DataTable;

            if (dtCurrent != null)
            {
                try
                {
                    if (dtCurrent == dtGioHang)
                    {
                        if (string.IsNullOrEmpty(keyword)) dtCurrent.DefaultView.RowFilter = "";
                        else dtCurrent.DefaultView.RowFilter = string.Format("Convert(TenSach, 'System.String') LIKE '%{0}%'", keyword);
                    }
                    else
                    {
                        // Đã thay 'TenSach' bằng 'ChiTietSach' do bảng đã gộp
                        if (string.IsNullOrEmpty(keyword)) dtCurrent.DefaultView.RowFilter = "";
                        else dtCurrent.DefaultView.RowFilter = string.Format("Convert(ChiTietSach, 'System.String') LIKE '%{0}%' OR Convert(MaPN, 'System.String') LIKE '%{0}%'", keyword);
                    }
                }
                catch (Exception ex) { MessageBox.Show("Lỗi cú pháp tìm kiếm: " + ex.Message); }
            }
        }

        private void dgvPhieuNhap_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                if (isEditingPhieu || dgvPhieuNhap.DataSource == dtGioHang)
                {
                    BocDuLieuPhieuNhap(e.RowIndex);
                }
            }
        }

        // =========================================================================
        // TAB NHÀ CUNG CẤP
        // =========================================================================
        private void SetupDataTableNCC()
        {
            if (dtNCCTam.Columns.Count == 0)
            {
                dtNCCTam.Columns.Add("MaNCC", typeof(int));
                dtNCCTam.Columns.Add("TenNCC");
                dtNCCTam.Columns.Add("DiaChi");
                dtNCCTam.Columns.Add("SoDienThoai");
                dtNCCTam.Columns.Add("Email");
            }
        }

        private void LoadDataNhaCungCap()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlDataAdapter da = new SqlDataAdapter("SELECT MaNCC, TenNCC, DiaChi, SoDienThoai, Email FROM NhaCungCap", conn);
                dtNCCTam.Clear();
                da.Fill(dtNCCTam);
                dgvNCC.DataSource = dtNCCTam;
            }
        }

        private void BocDuLieuNCC(int index)
        {
            DataRow dr = dtNCCTam.Rows[index];
            txtMaNCC.Text = dr["MaNCC"].ToString();
            txtTenNCC.Text = dr["TenNCC"].ToString();
            txtDiaChi.Text = dr["DiaChi"].ToString();
            txtSdt.Text = dr["SoDienThoai"].ToString();
            txtEmail.Text = dr["Email"].ToString();
        }

        private void dgvNCC_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            indexChonNCC = e.RowIndex;

            if (isEditingNCC)
            {
                BocDuLieuNCC(indexChonNCC);
            }
        }

        private void btnAddNCC_Click(object sender, EventArgs e)
        {
            isEditingNCC = false;
            if (string.IsNullOrWhiteSpace(txtTenNCC.Text)) { MessageBox.Show("Vui lòng nhập tên Nhà Cung Cấp!"); return; }

            DataRow dr = dtNCCTam.NewRow();
            dr["TenNCC"] = txtTenNCC.Text.Trim();
            dr["DiaChi"] = txtDiaChi.Text.Trim();
            dr["SoDienThoai"] = txtSdt.Text.Trim();
            dr["Email"] = txtEmail.Text.Trim();

            dtNCCTam.Rows.Add(dr);
            indexChonNCC = -1;

            MessageBox.Show("Đã thêm tạm! Nhấn Save để chốt vào Database.");
            ResetTabNCC();
        }

        private void btnEditNCC_Click(object sender, EventArgs e)
        {
            isEditingNCC = !isEditingNCC;

            if (isEditingNCC)
            {
                MessageBox.Show("Đã bật chế độ Sửa!");
                if (indexChonNCC != -1) BocDuLieuNCC(indexChonNCC);
            }
            else
            {
                MessageBox.Show("Đã TẮT chế độ Sửa.");
                ResetTabNCC();
            }
        }

        private void btnDelNCC_Click(object sender, EventArgs e)
        {
            if (dgvNCC.CurrentRow != null)
            {
                dtNCCTam.Rows.RemoveAt(dgvNCC.CurrentRow.Index);
                indexChonNCC = -1;
            }
        }

        private void btnSearchNCC_Click(object sender, EventArgs e)
        {
            string filter = string.Format("TenNCC LIKE '%{0}%'", txtSearchNCC.Text.Trim());
            dtNCCTam.DefaultView.RowFilter = filter;
        }

        private void btnResetNCC_Click(object sender, EventArgs e)
        {
            ResetTabNCC();
            if (dtNCCTam != null) dtNCCTam.DefaultView.RowFilter = "";
            indexChonNCC = -1;
            isEditingNCC = false;
        }

        private void btnSaveNCC_Click(object sender, EventArgs e)
        {
            dgvNCC.EndEdit();

            if (isEditingNCC && indexChonNCC != -1)
            {
                DataRow dr = dtNCCTam.Rows[indexChonNCC];
                dr["TenNCC"] = txtTenNCC.Text.Trim();
                dr["DiaChi"] = txtDiaChi.Text.Trim();
                dr["SoDienThoai"] = txtSdt.Text.Trim();
                dr["Email"] = txtEmail.Text.Trim();
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlTransaction trans = conn.BeginTransaction();
                try
                {
                    foreach (DataRow dr in dtNCCTam.Rows)
                    {
                        if (dr.RowState == DataRowState.Deleted) continue;

                        string sql = @"IF EXISTS (SELECT 1 FROM NhaCungCap WHERE MaNCC = @ma)
                                UPDATE NhaCungCap SET TenNCC=@ten, DiaChi=@dc, SoDienThoai=@sdt, Email=@email WHERE MaNCC=@ma
                               ELSE 
                                INSERT INTO NhaCungCap (TenNCC, DiaChi, SoDienThoai, Email) VALUES (@ten, @dc, @sdt, @email)";

                        SqlCommand cmd = new SqlCommand(sql, conn, trans);
                        cmd.Parameters.AddWithValue("@ma", dr["MaNCC"] == DBNull.Value ? -1 : dr["MaNCC"]);
                        cmd.Parameters.AddWithValue("@ten", dr["TenNCC"]);
                        cmd.Parameters.AddWithValue("@dc", dr["DiaChi"]);
                        cmd.Parameters.AddWithValue("@sdt", dr["SoDienThoai"]);
                        cmd.Parameters.AddWithValue("@email", dr["Email"]);
                        cmd.ExecuteNonQuery();
                    }
                    trans.Commit();
                    MessageBox.Show("Đã chốt cứng danh sách Nhà cung cấp vào DB!");

                    AppLogger.GhiLog(Session.Username, "Cập nhật Nhà Cung Cấp", "Đã lưu thay đổi danh sách Nhà cung cấp vào hệ thống");

                    LoadDataNhaCungCap();
                    ResetTabNCC();
                    indexChonNCC = -1;
                    isEditingNCC = false;
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    MessageBox.Show("Lỗi lưu DB: " + ex.Message);
                }
            }
        }

        private void LoadComboBoxNCC()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlDataAdapter da = new SqlDataAdapter("SELECT MaNCC, TenNCC FROM NhaCungCap", conn);
                DataTable dt = new DataTable(); da.Fill(dt);
                cboMaNCC.DataSource = dt; cboMaNCC.DisplayMember = "TenNCC"; cboMaNCC.ValueMember = "MaNCC"; cboMaNCC.SelectedIndex = -1;
            }
        }

        // =========================================================================
        // ĐÃ THÊM: HÀM LOAD COMBOBOX THỂ LOẠI
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

                    // Chèn thêm dòng Tất cả sách lên đầu để nếu thích thì xem full sách
                    DataRow row = dt.NewRow();
                    row["MaTL"] = 0;
                    row["TenTL"] = "--- Tất cả sách ---";
                    dt.Rows.InsertAt(row, 0);

                    // Gắn vào cboTheLoai (nếu tồn tại trên form)
                    if (this.Controls.Find("cboTheLoai", true).Length > 0)
                    {
                        ComboBox cboTL = (ComboBox)this.Controls.Find("cboTheLoai", true)[0];

                        // Tắt event trước khi đổ data
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
        // ĐÃ SỬA: HÀM LOAD COMBOBOX SÁCH CÓ THÊM CHỨC NĂNG LỌC THEO THỂ LOẠI
        // =========================================================================
        private void LoadComboBoxSach(int maTL = 0)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string sql = "SELECT MaSach, TenSach, GiaNhap, GiaBan FROM Sach";

                    // Lọc theo thể loại nếu maTL > 0
                    if (maTL > 0)
                    {
                        sql += " WHERE MaTL = " + maTL;
                    }

                    SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                    DataTable dt = new DataTable(); da.Fill(dt);

                    // Tắt sự kiện để chống lỗi nhảy liên hoàn
                    cboMaSach.SelectedIndexChanged -= cboMaSach_SelectedIndexChanged;

                    cboMaSach.DataSource = null;
                    this.BindingContext = new BindingContext();
                    cboMaSach.DataSource = dt;
                    cboMaSach.DisplayMember = "TenSach";
                    cboMaSach.ValueMember = "MaSach";

                    if (cboMaSach.Items.Count > 0) cboMaSach.SelectedIndex = -1;

                    // Bật lại sự kiện
                    cboMaSach.SelectedIndexChanged += cboMaSach_SelectedIndexChanged;
                }
                catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
            }
        }

        // =========================================================================
        // ĐÃ THÊM: SỰ KIỆN KHI CHỌN THỂ LOẠI -> LỌC SÁCH
        // =========================================================================
        private void cboTheLoai_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cboTL = sender as ComboBox;
            if (cboTL != null && cboTL.SelectedValue != null)
            {
                int maTL = 0;
                int.TryParse(cboTL.SelectedValue.ToString(), out maTL);

                // Gọi hàm load sách và truyền mã thể loại vào để lọc
                LoadComboBoxSach(maTL);

                // Xóa trắng ô đơn giá vì danh sách sách vừa bị đổi
                txtDongianhap.Clear();
                txtGiaBan.Clear();
            }
        }

        private void LoadComboBoxNguoiDung()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlDataAdapter da = new SqlDataAdapter("SELECT MaND, HoTen FROM NguoiDung", conn);
                DataTable dt = new DataTable(); da.Fill(dt);
                cboMaND.DataSource = dt; cboMaND.DisplayMember = "HoTen"; cboMaND.ValueMember = "MaND"; cboMaND.SelectedIndex = -1;
            }
        }

        private void ResetTabLapPhieu()
        {
            txtMaPN.Text = TuSinhMaPhieu();
            txtMaPN.ReadOnly = true;

            // Mặc định về dòng "Tất cả sách" nếu có control này trên giao diện
            if (this.Controls.Find("cboTheLoai", true).Length > 0)
            {
                ((ComboBox)this.Controls.Find("cboTheLoai", true)[0]).SelectedIndex = 0;
            }

            cboMaSach.SelectedIndex = -1;
            cboMaNCC.SelectedIndex = -1;

            if (cboMaND.Items.Count > 0)
            {
                cboMaND.Text = Session.Username;
                cboMaND.Enabled = false;
            }

            txtDongianhap.Clear();
            txtGiaBan.Clear();
            txtTongtien.Text = "0";
            txtGhichu.Clear();
            nudSoLuong.Value = 1;
        }

        private void ResetTabNCC()
        {
            txtMaNCC.Clear();
            txtTenNCC.Clear();
            txtDiaChi.Clear();
            txtSdt.Clear();
            txtEmail.Clear();
            txtMaNCC.ReadOnly = true;
        }

        private void CapNhatTongTien()
        {
            object sum = dtGioHang.Compute("Sum(ThanhTien)", "");
            txtTongtien.Text = sum.ToString() == "" ? "0" : Convert.ToDouble(sum).ToString("N0");
        }

        private void TinhThanhTienHienTai()
        {
            if (double.TryParse(txtDongianhap.Text, out double dg))
                txtThanhTien.Text = ((double)nudSoLuong.Value * dg).ToString("N0"); // Thêm phẩy cho đẹp
            else
                txtThanhTien.Text = "0";
        }

        private void nudSoLuong_ValueChanged(object sender, EventArgs e) => TinhThanhTienHienTai();

        private void txtDongianhap_TextChanged(object sender, EventArgs e) => TinhThanhTienHienTai();

        private void tcPhieuHang_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tcPhieuHang.SelectedIndex == 0) LoadComboBoxSach();
        }

        // TỰ ĐỘNG ĐIỀN GIÁ NHẬP & GIÁ BÁN KHI CHỌN SÁCH CŨ
        private void cboMaSach_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboMaSach.SelectedIndex >= 0 && cboMaSach.SelectedItem != null)
            {
                DataRowView row = cboMaSach.SelectedItem as DataRowView;
                if (row != null)
                {
                    txtDongianhap.Text = row["GiaNhap"].ToString();
                    txtGiaBan.Text = row["GiaBan"].ToString();
                }
            }
            else
            {
                txtDongianhap.Clear();
                txtGiaBan.Clear();
            }
        }

        // =========================================================================
        // TÍNH NĂNG IN PHIẾU NHẬP HÀNG 
        // =========================================================================
        private void btnIn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtMaPN.Text) || dtGioHang.Rows.Count > 0)
            {
                MessageBox.Show("Vui lòng Lưu phiếu hoặc Chọn một phiếu dưới bảng để in!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            PrintPreviewDialog ppd = new PrintPreviewDialog();
            PrintDocument pd = new PrintDocument();

            pd.PrintPage += new PrintPageEventHandler(InPhieuNhap_PrintPage);

            ppd.Document = pd;
            ppd.Width = 800;
            ppd.Height = 1000;
            ppd.ShowDialog();
        }

        private void InPhieuNhap_PrintPage(object sender, PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;
            Font fontTitle = new Font("Segoe UI", 22, FontStyle.Bold);
            Font fontHeader = new Font("Segoe UI", 12, FontStyle.Bold);
            Font fontNormal = new Font("Segoe UI", 12, FontStyle.Regular);
            Font fontItalic = new Font("Segoe UI", 11, FontStyle.Italic);

            int y = 50;
            int left = 50;

            g.DrawString("CỬA HÀNG SÁCH DT STORE", fontHeader, Brushes.Black, left, y);
            y += 25;
            g.DrawString("Địa chỉ: Hà Nội - Điện thoại: 0987.654.321", fontNormal, Brushes.Black, left, y);
            y += 60;

            g.DrawString("PHIẾU NHẬP KHO", fontTitle, Brushes.Black, new PointF(280, y));
            y += 50;

            g.DrawString("Mã phiếu: " + txtMaPN.Text, fontNormal, Brushes.Black, left, y);
            y += 25;
            g.DrawString("Ngày nhập: " + dtpNgayNhap.Value.ToString("dd/MM/yyyy HH:mm"), fontNormal, Brushes.Black, left, y);
            y += 25;
            g.DrawString("Nhà cung cấp: " + cboMaNCC.Text, fontNormal, Brushes.Black, left, y);
            y += 25;
            g.DrawString("Người lập: " + cboMaND.Text, fontNormal, Brushes.Black, left, y);
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

            double tongTien = 0;
            int stt = 1;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string sql = "SELECT s.TenSach, ct.SoLuong, ct.DonGiaNhap, ct.ThanhTien FROM ChiTietPhieuNhap ct JOIN Sach s ON ct.MaSach = s.MaSach WHERE ct.MaPN = @ma";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@ma", txtMaPN.Text);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    g.DrawString(stt.ToString(), fontNormal, Brushes.Black, left, y);

                    string tenSach = reader["TenSach"].ToString();
                    if (tenSach.Length > 35) tenSach = tenSach.Substring(0, 35) + "...";
                    g.DrawString(tenSach, fontNormal, Brushes.Black, left + 50, y);

                    g.DrawString(reader["SoLuong"].ToString(), fontNormal, Brushes.Black, left + 400, y);
                    g.DrawString(Convert.ToDouble(reader["DonGiaNhap"]).ToString("N0"), fontNormal, Brushes.Black, left + 480, y);
                    g.DrawString(Convert.ToDouble(reader["ThanhTien"]).ToString("N0"), fontNormal, Brushes.Black, left + 620, y);

                    tongTien += Convert.ToDouble(reader["ThanhTien"]);
                    stt++;
                    y += 30;
                }
            }

            y += 10;
            g.DrawLine(Pens.Black, left, y, 780, y);
            y += 20;

            Font fontTongTien = new Font("Segoe UI", 16, FontStyle.Bold);
            g.DrawString("TỔNG CỘNG: " + tongTien.ToString("N0") + " VNĐ", fontTongTien, Brushes.Black, left + 420, y);

            y += 60;

            g.DrawString("Người giao hàng", fontHeader, Brushes.Black, left + 50, y);
            g.DrawString("Người lập phiếu", fontHeader, Brushes.Black, left + 550, y);
            y += 20;
            g.DrawString("(Ký, ghi rõ họ tên)", fontItalic, Brushes.Black, left + 60, y);
            g.DrawString("(Ký, ghi rõ họ tên)", fontItalic, Brushes.Black, left + 560, y);
        }
    }
}