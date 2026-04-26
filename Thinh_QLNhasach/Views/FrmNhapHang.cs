using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using Thinh_QLNhasach.Utility; // Khai báo để lấy tên người dùng từ Session

namespace Thinh_QLNhasach
{
    public partial class FrmNhapHang : Form
    {
        string connectionString = @"Data Source=THINHLALUOT\SQLEXPRESS01;Initial Catalog=BookShop;Integrated Security=True";

        DataTable dtGioHang = new DataTable();
        private bool isEditingPhieu = false;

        DataTable dtNCCTam = new DataTable();
        private int indexChonNCC = -1;
        private bool isEditingNCC = false;

        public FrmNhapHang()
        {
            InitializeComponent();
            LoadData();
            LoadComboBoxSach();
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
            dgv.GridColor = System.Drawing.Color.Silver;
            dgv.RowHeadersVisible = false;
            dgv.BackgroundColor = System.Drawing.Color.White;
            dgv.BorderStyle = BorderStyle.None;
            dgv.DefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 10F);
            dgv.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(231, 229, 255);
            dgv.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.Black;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
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
                dtGioHang.Columns.Add("ThanhTien", typeof(double), "SoLuong * DonGiaNhap");
            }

            SetupDataTableNCC();

            LoadData();
            LoadComboBoxNCC();
            LoadComboBoxSach();
            LoadComboBoxNguoiDung();
            LoadDataNhaCungCap();
            ResetTabLapPhieu();

            dgvPhieuNhap.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvNCC.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void LoadData()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = @"SELECT ct.MaPN, ct.MaSach, s.TenSach, ct.SoLuong, ct.DonGiaNhap, ct.ThanhTien, 
                               pn.NgayNhap, pn.GhiChu, pn.MaNCC, pn.MaND, pn.TongTien 
                       FROM ChiTietPhieuNhap ct
                       JOIN PhieuNhap pn ON ct.MaPN = pn.MaPN
                       JOIN Sach s ON ct.MaSach = s.MaSach
                       ORDER BY pn.NgayNhap DESC";

                SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dgvPhieuNhap.DataSource = dt;

                if (dgvPhieuNhap.Columns.Count > 0)
                {
                    if (dgvPhieuNhap.Columns.Contains("MaSach")) dgvPhieuNhap.Columns["MaSach"].Visible = false;
                    if (dgvPhieuNhap.Columns.Contains("MaNCC")) dgvPhieuNhap.Columns["MaNCC"].Visible = false;
                    if (dgvPhieuNhap.Columns.Contains("MaND")) dgvPhieuNhap.Columns["MaND"].Visible = false;
                    if (dgvPhieuNhap.Columns.Contains("TongTien")) dgvPhieuNhap.Columns["TongTien"].Visible = false;

                    dgvPhieuNhap.Columns["MaPN"].HeaderText = "Mã Phiếu";
                    dgvPhieuNhap.Columns["TenSach"].HeaderText = "Tên Sách";
                    dgvPhieuNhap.Columns["SoLuong"].HeaderText = "SL";
                    dgvPhieuNhap.Columns["DonGiaNhap"].HeaderText = "Đơn Giá";
                    dgvPhieuNhap.Columns["ThanhTien"].HeaderText = "Thành Tiền";
                    dgvPhieuNhap.Columns["NgayNhap"].HeaderText = "Ngày Nhập";
                    if (dgvPhieuNhap.Columns.Contains("GhiChu")) dgvPhieuNhap.Columns["GhiChu"].HeaderText = "Ghi Chú";

                    dgvPhieuNhap.Columns["ThanhTien"].DefaultCellStyle.Format = "N0";
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

                cboMaSach.Text = r.Cells["TenSach"].Value.ToString();
                nudSoLuong.Value = Convert.ToDecimal(r.Cells["SoLuong"].Value);
                txtDongianhap.Text = r.Cells["DonGiaNhap"].Value.ToString();
                txtThanhTien.Text = Convert.ToDouble(r.Cells["ThanhTien"].Value).ToString("N0");

                btnSave.Tag = new { Mode = "EDIT_LE", MaPN = maPN, MaSachCu = r.Cells["MaSach"].Value.ToString(), SoLuongCu = r.Cells["SoLuong"].Value };
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (dgvPhieuNhap.DataSource != dtGioHang)
            {
                dgvPhieuNhap.DataSource = dtGioHang;
                if (dgvPhieuNhap.Columns.Contains("MaSach")) dgvPhieuNhap.Columns["MaSach"].Visible = true;
            }
            if (cboMaSach.SelectedValue == null) return;
            dtGioHang.Rows.Add(cboMaSach.SelectedValue.ToString(), cboMaSach.Text, nudSoLuong.Value, Convert.ToDouble(txtDongianhap.Text));
            CapNhatTongTien();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (btnSave.Tag != null && btnSave.Tag.ToString().Contains("EDIT_LE"))
            {
                dynamic data = btnSave.Tag;
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlTransaction trans = conn.BeginTransaction();
                    try
                    {
                        int slMoi = (int)nudSoLuong.Value;
                        double dgMoi = Convert.ToDouble(txtDongianhap.Text);

                        new SqlCommand($"UPDATE ChiTietPhieuNhap SET SoLuong={slMoi}, DonGiaNhap={dgMoi}, ThanhTien={slMoi * dgMoi} WHERE MaPN='{data.MaPN}' AND MaSach='{data.MaSachCu}'", conn, trans).ExecuteNonQuery();

                        new SqlCommand($"UPDATE Sach SET SoLuongTon = SoLuongTon - {data.SoLuongCu} + {slMoi} WHERE MaSach='{data.MaSachCu}'", conn, trans).ExecuteNonQuery();

                        string updatePN = "UPDATE PhieuNhap SET GhiChu = @ghi, MaNCC = @ncc, MaND = @nd WHERE MaPN = @ma";
                        SqlCommand cmdPN = new SqlCommand(updatePN, conn, trans);
                        cmdPN.Parameters.AddWithValue("@ghi", txtGhichu.Text);
                        cmdPN.Parameters.AddWithValue("@ncc", cboMaNCC.SelectedValue);
                        cmdPN.Parameters.AddWithValue("@nd", cboMaND.SelectedValue);
                        cmdPN.Parameters.AddWithValue("@ma", data.MaPN);
                        cmdPN.ExecuteNonQuery();

                        new SqlCommand($"UPDATE PhieuNhap SET TongTien = (SELECT SUM(ThanhTien) FROM ChiTietPhieuNhap WHERE MaPN='{data.MaPN}') WHERE MaPN='{data.MaPN}'", conn, trans).ExecuteNonQuery();

                        trans.Commit();
                        btnSave.Tag = null;
                        isEditingPhieu = false;

                        MessageBox.Show("Sửa phiếu thành công!");

                        // ========================================================
                        // GHI LOG: SỬA PHIẾU NHẬP
                        // ========================================================
                        AppLogger.GhiLog(Session.Username, "Sửa Phiếu Nhập", $"Đã sửa thông tin phiếu nhập mã {data.MaPN}");

                        LoadData();
                        ResetTabLapPhieu();
                        return;
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        MessageBox.Show("Lỗi: " + ex.Message);
                        return;
                    }
                }
            }

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
                        new SqlCommand($"UPDATE Sach SET SoLuongTon = SoLuongTon + {dr["SoLuong"]} WHERE MaSach='{dr["MaSach"]}'", conn, trans).ExecuteNonQuery();
                    }
                    trans.Commit();
                    MessageBox.Show("Đã chốt phiếu " + txtMaPN.Text);

                    // ========================================================
                    // GHI LOG: LẬP PHIẾU NHẬP MỚI
                    // ========================================================
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
                MessageBox.Show("Đã bật chế độ Sửa! Click vào bất kỳ dòng nào dưới bảng để bốc thông tin lên sửa.");
                BocDuLieuPhieuNhap(dgvPhieuNhap.CurrentRow.Index);
            }
            else
            {
                isEditingPhieu = false;
                MessageBox.Show("Đã tắt chế độ sửa.");
                ResetTabLapPhieu();
            }
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            if (dgvPhieuNhap.CurrentRow == null) return;
            if (dgvPhieuNhap.DataSource == dtGioHang)
            {
                dtGioHang.Rows.RemoveAt(dgvPhieuNhap.CurrentRow.Index);
                CapNhatTongTien();
            }
            else MessageBox.Show("Hàng đã lưu DB, dùng SQL xóa!");
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
            string keyword = txtSearch.Text.Trim();
            dtGioHang.DefaultView.RowFilter = string.IsNullOrEmpty(keyword) ? "" : string.Format("TenSach LIKE '%{0}%'", keyword);
        }

        private void dgvPhieuNhap_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                if (isEditingPhieu)
                {
                    BocDuLieuPhieuNhap(e.RowIndex);
                }
                else if (dgvPhieuNhap.DataSource == dtGioHang)
                {
                    BocDuLieuPhieuNhap(e.RowIndex);
                }
            }
        }

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
            // Đảo ngược trạng thái: Đang tắt thành bật, đang bật thành tắt
            isEditingNCC = !isEditingNCC;

            if (isEditingNCC)
            {
                MessageBox.Show("Đã bật chế độ Sửa!");

                // Nếu ông đang click chọn sẵn 1 dòng dưới bảng thì nó bốc lên luôn cho lẹ
                if (indexChonNCC != -1)
                {
                    BocDuLieuNCC(indexChonNCC);
                }
            }
            else
            {
                MessageBox.Show("Đã TẮT chế độ Sửa.");
                ResetTabNCC(); // Tắt đi thì xóa trắng form cho gọn
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

            // =========================================================================
            // ĐIỂM ĂN TIỀN Ở ĐÂY: Nếu đang bật chế độ Sửa, tự động ép TextBox vào bảng tạm
            // =========================================================================
            if (isEditingNCC && indexChonNCC != -1)
            {
                DataRow dr = dtNCCTam.Rows[indexChonNCC];
                dr["TenNCC"] = txtTenNCC.Text.Trim();
                dr["DiaChi"] = txtDiaChi.Text.Trim();
                dr["SoDienThoai"] = txtSdt.Text.Trim();
                dr["Email"] = txtEmail.Text.Trim();
            }

            // =========================================================================
            // DƯỚI NÀY LÀ LOGIC LƯU DATABASE NHƯ CŨ, KHÔNG ĐỔI
            // =========================================================================
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

                    // ========================================================
                    // GHI LOG: CẬP NHẬT NHÀ CUNG CẤP
                    // ========================================================
                    AppLogger.GhiLog(Session.Username, "Cập nhật Nhà Cung Cấp", "Đã lưu thay đổi danh sách Nhà cung cấp vào hệ thống");

                    LoadDataNhaCungCap();
                    ResetTabNCC();
                    indexChonNCC = -1;
                    isEditingNCC = false; // Lưu xong thì tự động tắt cờ Sửa đi
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

        private void LoadComboBoxSach()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    SqlDataAdapter da = new SqlDataAdapter("SELECT MaSach, TenSach FROM Sach", conn);
                    DataTable dt = new DataTable(); da.Fill(dt);
                    cboMaSach.DataSource = null; this.BindingContext = new BindingContext();
                    cboMaSach.DataSource = dt; cboMaSach.DisplayMember = "TenSach"; cboMaSach.ValueMember = "MaSach";
                    if (cboMaSach.Items.Count > 0) cboMaSach.SelectedIndex = -1;
                }
                catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
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
            cboMaSach.SelectedIndex = -1;
            cboMaNCC.SelectedIndex = -1;
            txtDongianhap.Clear();
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
            if (double.TryParse(txtDongianhap.Text, out double dg)) txtThanhTien.Text = ((double)nudSoLuong.Value * dg).ToString();
        }

        private void nudSoLuong_ValueChanged(object sender, EventArgs e) => TinhThanhTienHienTai();

        private void txtDongianhap_TextChanged(object sender, EventArgs e) => TinhThanhTienHienTai();

        private void tcPhieuHang_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tcPhieuHang.SelectedIndex == 0) LoadComboBoxSach();
        }

        private void cboMaSach_Enter(object sender, EventArgs e) => LoadComboBoxSach();
    }
}