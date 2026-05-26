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

            if (dtGioHang.Columns.Count == 0)
            {
                dtGioHang.Columns.Add("MaSach", typeof(string));
                dtGioHang.Columns.Add("TenSach", typeof(string));
                dtGioHang.Columns.Add("SoLuong", typeof(int));
                dtGioHang.Columns.Add("DonGiaNhap", typeof(double));
                dtGioHang.Columns.Add("GiaBan", typeof(double));
                dtGioHang.Columns.Add("ThanhTien", typeof(double), "SoLuong * DonGiaNhap");
            }

            SetupDataTableNCC();

            LoadData();
            LoadComboBoxNCC();
            LoadComboBoxTheLoai(); 
            LoadComboBoxSach();
            LoadComboBoxNguoiDung();
            LoadDataNhaCungCap();
            ResetTabLapPhieu();

            dgvPhieuNhap.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvNCC.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        // =========================================================================
        // ĐÃ THÊM: HÀM QUẢN LÝ KHÓA/MỞ GIAO DIỆN THÔNG MINH (UX)
        // =========================================================================
        private void KhoaChucNang()
        {
            // Xác định xem có đang "dính" vào một Phiếu nhập cũ hay không
            bool dangXemPhieuCu = (btnSave.Tag != null && btnSave.Tag.ToString().Contains("EDIT_MASTER"));

            // 1. Xử lý khóa phần thông tin Master (Nhà cung cấp, Ghi chú)
            // Nếu đang bốc phiếu cũ lên xem mà CHƯA bật chế độ Sửa => KHÓA
            bool khoaMaster = dangXemPhieuCu && !isEditingPhieu;
            cboMaNCC.Enabled = !khoaMaster;
            txtGhichu.ReadOnly = khoaMaster;

            // 2. Xử lý khóa phần Nhập Sách (Thể loại, Sách, Số lượng, Giá)
            // Nếu đang dính vào phiếu cũ => TUYỆT ĐỐI KHÓA để không thêm nhầm sách vào phiếu đã lưu
            bool khoaNhapSach = dangXemPhieuCu;

            if (this.Controls.Find("cboTheLoai", true).Length > 0)
            {
                this.Controls.Find("cboTheLoai", true)[0].Enabled = !khoaNhapSach;
            }
            cboMaSach.Enabled = !khoaNhapSach;
            nudSoLuong.Enabled = !khoaNhapSach;
            txtDongianhap.ReadOnly = khoaNhapSach;
            txtGiaBan.ReadOnly = khoaNhapSach;
            btnAdd.Enabled = !khoaNhapSach;

            // 3. Khóa nút Save an toàn
            if (dangXemPhieuCu && !isEditingPhieu)
            {
                btnSave.Enabled = false;
            }
            else
            {
                btnSave.Enabled = true;
            }
        }

        // =========================================================================
        // NÚT GỌI FORM TẠO SÁCH MỚI
        // =========================================================================
        private void btnTaoSachMoi_Click(object sender, EventArgs e)
        {
            Views.FrmTaoSachMoi frm = new Views.FrmTaoSachMoi();
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
                    if (dgvPhieuNhap.Columns.Contains("MaNCC")) dgvPhieuNhap.Columns["MaNCC"].Visible = false;
                    if (dgvPhieuNhap.Columns.Contains("MaND")) dgvPhieuNhap.Columns["MaND"].Visible = false;

                    dgvPhieuNhap.Columns["MaPN"].HeaderText = "Mã Phiếu";
                    dgvPhieuNhap.Columns["MaPN"].FillWeight = 50;

                    dgvPhieuNhap.Columns["NgayNhap"].HeaderText = "Ngày Nhập";
                    dgvPhieuNhap.Columns["NgayNhap"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
                    dgvPhieuNhap.Columns["NgayNhap"].FillWeight = 80;

                    dgvPhieuNhap.Columns["TenNCC"].HeaderText = "Nhà Cung Cấp";
                    dgvPhieuNhap.Columns["NguoiLap"].HeaderText = "Người Lập";

                    dgvPhieuNhap.Columns["ChiTietSach"].HeaderText = "Sách Đã Nhập (Gộp)";
                    dgvPhieuNhap.Columns["ChiTietSach"].FillWeight = 150;

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
                cboMaSach.SelectedValue = r.Cells["MaSach"].Value.ToString();
                nudSoLuong.Value = Convert.ToDecimal(r.Cells["SoLuong"].Value);
                txtDongianhap.Text = r.Cells["DonGiaNhap"].Value.ToString();
                if (r.Cells["GiaBan"] != null) txtGiaBan.Text = r.Cells["GiaBan"].Value.ToString();

                KhoaChucNang();
            }
            else
            {
                string maPN = r.Cells["MaPN"].Value.ToString();
                txtMaPN.Text = maPN;
                dtpNgayNhap.Value = Convert.ToDateTime(r.Cells["NgayNhap"].Value);

                if (r.Cells["MaNCC"].Value != DBNull.Value) cboMaNCC.SelectedValue = r.Cells["MaNCC"].Value;
                if (r.Cells["MaND"].Value != DBNull.Value) cboMaND.SelectedValue = r.Cells["MaND"].Value;

                txtTongtien.Text = Convert.ToDouble(r.Cells["TongTien"].Value).ToString("N0");
                txtGhichu.Text = r.Cells["GhiChu"].Value.ToString();

                cboMaSach.SelectedIndex = -1;
                nudSoLuong.Value = 1;
                txtDongianhap.Clear();
                txtGiaBan.Clear();
                txtThanhTien.Text = "0";

                btnSave.Tag = new { Mode = "EDIT_MASTER", MaPN = maPN };

                KhoaChucNang();
            }
        }

        // =========================================================================
        // ĐÃ FIX: BẤM VÀO LƯỚI LÀ BỐC LÊN XEM LUÔN ĐỂ IN (TỰ ĐỘNG KHÓA NGAY LẬP TỨC)
        // =========================================================================
        private void dgvPhieuNhap_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                // Bỏ qua mọi rào cản, click là bốc lên form để xem/in
                BocDuLieuPhieuNhap(e.RowIndex);
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
                if (!isEditingPhieu)
                {
                    MessageBox.Show("Vui lòng BẬT CHẾ ĐỘ SỬA (Nút cờ lê) trước khi lưu!", "Cảnh báo");
                    return;
                }

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

                        MessageBox.Show("Cập nhật thông tin phiếu thành công (Chỉ sửa Nhà Cung Cấp & Ghi Chú)!");
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

        // =========================================================================
        // ĐÃ FIX: CHỈ LÀM NHIỆM VỤ MỞ KHÓA KHI ĐANG XEM PHIẾU CŨ
        // =========================================================================
        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dgvPhieuNhap.CurrentRow == null || dgvPhieuNhap.DataSource == dtGioHang)
            {
                MessageBox.Show("Hãy chọn một Phiếu nhập từ Lịch Sử dưới bảng trước!");
                return;
            }

            isEditingPhieu = !isEditingPhieu;

            if (isEditingPhieu)
            {
                MessageBox.Show("Đã BẬT chế độ Sửa!\nBạn có thể sửa thông tin Nhà Cung Cấp và Ghi Chú của phiếu nhập này.");
                KhoaChucNang(); // Cập nhật lại Form (Mở khóa)
            }
            else
            {
                MessageBox.Show("Đã TẮT chế độ Sửa.");
                KhoaChucNang(); // Cập nhật lại Form (Đóng khóa)
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            dtGioHang.Clear();
            isEditingPhieu = false;
            ResetTabLapPhieu();
            LoadData(); // Load lại lưới về Lịch sử CSDL
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
                        if (string.IsNullOrEmpty(keyword))
                            dtCurrent.DefaultView.RowFilter = "";
                        else
                            dtCurrent.DefaultView.RowFilter = string.Format("Convert(ChiTietSach, 'System.String') LIKE '%{0}%' OR Convert(MaPN, 'System.String') LIKE '%{0}%' OR Convert(TenNCC, 'System.String') LIKE '%{0}%' OR Convert(NguoiLap, 'System.String') LIKE '%{0}%'", keyword);
                    }
                }
                catch (Exception ex) { MessageBox.Show("Lỗi cú pháp tìm kiếm: " + ex.Message); }
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

        private void LoadComboBoxTheLoai()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    SqlDataAdapter da = new SqlDataAdapter("SELECT MaTL, TenTL FROM TheLoai", conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    DataRow row = dt.NewRow();
                    row["MaTL"] = 0;
                    row["TenTL"] = "--- Tất cả sách ---";
                    dt.Rows.InsertAt(row, 0);

                    if (this.Controls.Find("cboTheLoai", true).Length > 0)
                    {
                        ComboBox cboTL = (ComboBox)this.Controls.Find("cboTheLoai", true)[0];
                        cboTL.SelectedIndexChanged -= cboTheLoai_SelectedIndexChanged;

                        cboTL.DataSource = dt;
                        cboTL.DisplayMember = "TenTL";
                        cboTL.ValueMember = "MaTL";
                        cboTL.SelectedIndex = 0;

                        cboTL.SelectedIndexChanged += cboTheLoai_SelectedIndexChanged;
                    }
                }
                catch (Exception ex) { MessageBox.Show("Lỗi load Thể loại: " + ex.Message); }
            }
        }

        private void LoadComboBoxSach(int maTL = 0)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string sql = "SELECT MaSach, TenSach, GiaNhap, GiaBan FROM Sach";

                    if (maTL > 0)
                    {
                        sql += " WHERE MaTL = " + maTL;
                    }

                    SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                    DataTable dt = new DataTable(); da.Fill(dt);

                    cboMaSach.SelectedIndexChanged -= cboMaSach_SelectedIndexChanged;

                    cboMaSach.DataSource = null;
                    this.BindingContext = new BindingContext();
                    cboMaSach.DataSource = dt;
                    cboMaSach.DisplayMember = "TenSach";
                    cboMaSach.ValueMember = "MaSach";

                    if (cboMaSach.Items.Count > 0) cboMaSach.SelectedIndex = -1;

                    cboMaSach.SelectedIndexChanged += cboMaSach_SelectedIndexChanged;
                }
                catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
            }
        }

        private void cboTheLoai_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cboTL = sender as ComboBox;
            if (cboTL != null && cboTL.SelectedValue != null)
            {
                int maTL = 0;
                int.TryParse(cboTL.SelectedValue.ToString(), out maTL);

                LoadComboBoxSach(maTL);

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

        // =========================================================================
        // ĐÃ FIX: KHI RESET LÀ TRẢ VỀ FORM TRỐNG VÀ MỞ KHÓA MỌI THỨ
        // =========================================================================
        private void ResetTabLapPhieu()
        {
            txtMaPN.Text = TuSinhMaPhieu();
            txtMaPN.ReadOnly = true;

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

            btnSave.Tag = null; // Cực kỳ quan trọng: Báo cho hệ thống biết là đang lập phiếu MỚI
            isEditingPhieu = false;
            KhoaChucNang(); // Mở khóa toàn bộ
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
                txtThanhTien.Text = ((double)nudSoLuong.Value * dg).ToString("N0");
            else
                txtThanhTien.Text = "0";
        }

        private void nudSoLuong_ValueChanged(object sender, EventArgs e) => TinhThanhTienHienTai();

        private void txtDongianhap_TextChanged(object sender, EventArgs e) => TinhThanhTienHienTai();

        private void tcPhieuHang_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tcPhieuHang.SelectedIndex == 0) LoadComboBoxSach();
        }

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