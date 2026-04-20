using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using FontAwesome.Sharp;

namespace Thinh_QLNhasach.Views
{
    public partial class FrmHoaDon : Form
    {
        // Chuỗi kết nối đến cơ sở dữ liệu SQL Server
        string connectionString = @"Data Source=THINHLALUOT\SQLEXPRESS01;Initial Catalog=BookShop;Integrated Security=True";

        // DataTable dùng làm giỏ hàng tạm thời để hiển thị trên DataGridView
        DataTable dtGioHang = new DataTable();

        // Biến cờ (Flag) dùng để ngăn chặn việc người dùng click quá nhanh gây nhân đôi dữ liệu
        bool isProcessing = false;

        public FrmHoaDon()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Sự kiện chạy khi Form được nạp lên
        /// </summary>
        private void FrmHoaDon_Load(object sender, EventArgs e)
        {
            // Khởi tạo các cột cho giỏ hàng nếu chưa có
            if (dtGioHang.Columns.Count == 0)
            {
                dtGioHang.Columns.Add("MaSach", typeof(string));
                dtGioHang.Columns.Add("TenSach", typeof(string));
                dtGioHang.Columns.Add("SoLuong", typeof(int));
                dtGioHang.Columns.Add("DonGia", typeof(double));
                // Cột Thành tiền tự động tính toán dựa trên Số lượng và Đơn giá
                dtGioHang.Columns.Add("ThanhTien", typeof(double), "SoLuong * DonGia");
            }
            dgvGioHang.DataSource = dtGioHang;

            // Định dạng hiển thị cho các DataGridView (Đã sửa lại để giống dgvTacGia)
            FormatDataGridView(dgvGioHang);
            FormatDataGridView(dgvHoaDon);

            // Ẩn cột Mã sách đi cho đẹp giao diện nhưng vẫn giữ dữ liệu ngầm
            if (dgvGioHang.Columns.Contains("MaSach")) dgvGioHang.Columns["MaSach"].Visible = false;

            // Nạp dữ liệu vào các ComboBox và làm mới Form
            LoadComboBoxNguoiDung();
            LoadComboBoxSach();
            ResetForm();
            LoadLichSuHoaDon(); // Tải lịch sử hóa đơn ở Tab 2
        }

        /// <summary>
        /// Hàm định dạng giao diện cho DataGridView (Header xám đen, chữ trắng, Row trắng lả lướt)
        /// </summary>
        private void FormatDataGridView(DataGridView dgv)
        {
            if (dgv == null) return;

            // 1. HIỆN GRIDLINES (ĐƯỜNG KẺ Ô)
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            dgv.GridColor = Color.FromArgb(224, 224, 224);
            dgv.BackgroundColor = Color.White;
            dgv.BorderStyle = BorderStyle.None;
            dgv.EnableHeadersVisualStyles = false;

            // 2. HEADER TO VÀ CÓ VIỀN
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(128, 128, 128);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            dgv.ColumnHeadersHeight = 45;

            // 3. NỘI DUNG CHỮ TO
            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 11F);
            dgv.RowTemplate.Height = 40;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.RowHeadersVisible = false;

            // 4. CHỐNG CO RÚM: Dàn đều cột ra toàn bảng
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // 5. TIẾNG VIỆT & TÔ MÀU XANH CỘT ĐẦU
            dgv.ColumnAdded += (s, e) =>
            {
                // Việt hóa bảng Giỏ hàng
                if (e.Column.Name == "TenSach") e.Column.HeaderText = "Tên Sách";
                if (e.Column.Name == "SoLuong") { e.Column.HeaderText = "Số Lượng"; e.Column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter; }
                if (e.Column.Name == "DonGia") { e.Column.HeaderText = "Đơn Giá"; e.Column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight; }
                if (e.Column.Name == "ThanhTien") { e.Column.HeaderText = "Thành Tiền"; e.Column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight; }

                // Việt hóa bảng Lịch sử (dgvHoaDon) - Fix cái ảnh bị co của ông đây
                if (e.Column.Name == "MaHD") e.Column.HeaderText = "Mã Hóa Đơn";
                if (e.Column.Name == "NgayLap") e.Column.HeaderText = "Ngày Lập";
                if (e.Column.Name == "NhanVien") e.Column.HeaderText = "Nhân Viên";
                if (e.Column.Name == "TenKhachHang") e.Column.HeaderText = "Khách Hàng";
                if (e.Column.Name == "ThanhTien" && dgv.Name == "dgvHoaDon") e.Column.HeaderText = "Tổng Tiền";

                // Chỉ tô màu xanh cho cột ĐẦU TIÊN của mỗi bảng
                if (e.Column.Index == 0 || (dgv.Columns.Count > 0 && !dgv.Columns[0].Visible && e.Column.Index == 1))
                {
                    e.Column.HeaderCell.Style.BackColor = Color.FromArgb(0, 120, 215);
                }
            };

            // 6. MÀU XEN KẼ
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
                        int soHienTai = int.Parse(maCu.Substring(2));
                        maMoi = "HD" + (soHienTai + 1).ToString("D3");
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

                foreach (DataRow dr in dtGioHang.Rows)
                {
                    if (dr["MaSach"].ToString() == maSach)
                    {
                        if (Convert.ToInt32(dr["SoLuong"]) + soLuongThem > tonKho)
                        {
                            MessageBox.Show($"Kho chỉ còn {tonKho} cuốn!");
                            return;
                        }
                        dr["SoLuong"] = Convert.ToInt32(dr["SoLuong"]) + soLuongThem;
                        CapNhatTien();
                        return;
                    }
                }

                if (soLuongThem > tonKho) { MessageBox.Show($"Kho chỉ còn {tonKho} cuốn!"); return; }
                dtGioHang.Rows.Add(maSach, cboMaSach.Text, soLuongThem, Convert.ToDouble(txtDonGia.Text));
                CapNhatTien();
            }
            finally { isProcessing = false; }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dgvGioHang.CurrentRow == null) return;
            DataGridViewRow r = dgvGioHang.CurrentRow;
            cboMaSach.SelectedValue = r.Cells["MaSach"].Value.ToString();
            nudSoLuong.Value = Convert.ToDecimal(r.Cells["SoLuong"].Value);
            txtDonGia.Text = r.Cells["DonGia"].Value.ToString();
            dtGioHang.Rows.RemoveAt(r.Index);
            CapNhatTien();
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            if (dgvGioHang.CurrentRow != null)
            {
                dtGioHang.Rows.RemoveAt(dgvGioHang.CurrentRow.Index);
                CapNhatTien();
            }
        }

        private void CapNhatTien()
        {
            object sum = dtGioHang.Compute("Sum(ThanhTien)", "");
            double tongTien = sum.ToString() == "" ? 0 : Convert.ToDouble(sum);
            txtTongTien.Text = tongTien.ToString("N0");

            double giamGia = 0;
            string rawGiamGia = txtGiamGia.Text.Replace("%", "").Trim();
            if (double.TryParse(rawGiamGia, out double parseGiamGia)) giamGia = parseGiamGia;

            double thanhTien = tongTien - giamGia;
            if (thanhTien < 0) thanhTien = 0;
            txtThanhTien.Text = thanhTien.ToString("N0");
        }

        private void txtGiamGia_TextChanged(object sender, EventArgs e)
        {
            CapNhatTien();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // 1. CHỐT CHẶN TỨC THÌ: Nếu giỏ hàng rỗng thì thoát ngay
            if (dtGioHang == null || dtGioHang.Rows.Count == 0) return;
            if (cboMaNV.SelectedValue == null) { MessageBox.Show("Vui lòng chọn Nhân viên!"); return; }

            // 2. COPY DỮ LIỆU RA BIẾN TẠM VÀ XÓA GIỎ GỐC (Chống nhân đôi dữ liệu)
            DataTable dtTemp = dtGioHang.Copy();
            dtGioHang.Clear();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    SqlTransaction trans = conn.BeginTransaction();
                    try
                    {
                        // 3. LƯU HÓA ĐƠN CHÍNH
                        string sqlHD = @"INSERT INTO HoaDon (MaHD, MaND, TenKhachHang, NgayLap, TongTien, GiamGia, ThanhTien) 
                                 VALUES (@mahd, @mand, @tenkh, @ngay, @tong, @giam, @thanhtien)";

                        SqlCommand cmdHD = new SqlCommand(sqlHD, conn, trans);
                        cmdHD.Parameters.AddWithValue("@mahd", txtMaHD.Text);
                        cmdHD.Parameters.AddWithValue("@mand", cboMaNV.SelectedValue);
                        cmdHD.Parameters.AddWithValue("@tenkh", string.IsNullOrWhiteSpace(txtTenKH.Text) ? "Khách lẻ" : txtTenKH.Text.Trim());

                        // FIX Ở ĐÂY: Lấy giá trị từ DateTimePicker thay vì DateTime.Now
                        cmdHD.Parameters.AddWithValue("@ngay", dtpNgayLap.Value);

                        // Xử lý convert số an toàn
                        double tongTien = double.Parse(txtTongTien.Text.Replace(",", "").Replace(".", ""));
                        double giamGia = string.IsNullOrWhiteSpace(txtGiamGia.Text) ? 0 : double.Parse(txtGiamGia.Text.Replace(",", "").Replace(".", ""));
                        double thanhTien = double.Parse(txtThanhTien.Text.Replace(",", "").Replace(".", ""));

                        cmdHD.Parameters.AddWithValue("@tong", tongTien);
                        cmdHD.Parameters.AddWithValue("@giam", giamGia);
                        cmdHD.Parameters.AddWithValue("@thanhtien", thanhTien);
                        cmdHD.ExecuteNonQuery();

                        // 4. DUYỆT GIỎ HÀNG ĐỂ LƯU CHI TIẾT VÀ TRỪ KHO
                        foreach (DataRow dr in dtTemp.Rows)
                        {
                            // Lưu chi tiết
                            SqlCommand cmdCT = new SqlCommand("INSERT INTO ChiTietHoaDon (MaHD, MaSach, SoLuong, DonGia, ThanhTien) VALUES (@mahd, @masach, @sl, @dg, @tt)", conn, trans);
                            cmdCT.Parameters.AddWithValue("@mahd", txtMaHD.Text);
                            cmdCT.Parameters.AddWithValue("@masach", dr["MaSach"]);
                            cmdCT.Parameters.AddWithValue("@sl", dr["SoLuong"]);
                            cmdCT.Parameters.AddWithValue("@dg", dr["DonGia"]);
                            cmdCT.Parameters.AddWithValue("@tt", dr["ThanhTien"]);
                            cmdCT.ExecuteNonQuery();

                            // Cập nhật kho (Đảm bảo đã xóa Trigger TRG_TuDongGiamTonKhiBan trong SQL trước khi dùng dòng này)
                            SqlCommand cmdUp = new SqlCommand("UPDATE Sach SET SoLuongTon = SoLuongTon - @sl WHERE MaSach=@ms", conn, trans);
                            cmdUp.Parameters.AddWithValue("@sl", dr["SoLuong"]);
                            cmdUp.Parameters.AddWithValue("@ms", dr["MaSach"]);
                            cmdUp.ExecuteNonQuery();
                        }

                        trans.Commit(); // Chốt đơn thành công
                        MessageBox.Show("Thanh toán thành công!");

                        // 5. LÀM MỚI GIAO DIỆN
                        ResetForm(); // Hàm này của ông đã có TuSinhMaHD() bên trong rồi
                        LoadComboBoxSach(); // Cập nhật lại số tồn kho lên UI
                        LoadLichSuHoaDon(); // Cập nhật danh sách bên Tab lịch sử
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback(); // Có biến là hoàn tác ngay
                        dtGioHang = dtTemp.Copy(); // Trả lại hàng vào giỏ để ông không phải nhập lại từ đầu
                        MessageBox.Show("Lỗi SQL: " + ex.Message);
                    }
                }
                catch (Exception ex) { MessageBox.Show("Lỗi kết nối: " + ex.Message); }
            }
        }

        private void LoadLichSuHoaDon(bool filter = false) // Thêm cái cờ filter để biết khi nào cần lọc
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    // Câu lệnh gốc: Lấy tất cả
                    string sql = @"SELECT h.MaHD, h.NgayLap, n.HoTen as NhanVien, h.TenKhachHang, h.ThanhTien 
                           FROM HoaDon h JOIN NguoiDung n ON h.MaND = n.MaND WHERE 1=1";

                    // Nếu filter = true (nghĩa là ông vừa bấm nút Tìm kiếm) thì mới thêm điều kiện vào
                    if (filter)
                    {
                        sql += " AND h.NgayLap BETWEEN @tuNgay AND @denNgay";
                        if (!string.IsNullOrWhiteSpace(txtSearch.Text))
                            sql += " AND (h.MaHD LIKE @search OR h.TenKhachHang LIKE @search)";
                    }

                    sql += " ORDER BY h.NgayLap DESC"; // Luôn hiện hóa đơn mới nhất lên đầu

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
                string maHD = dgvHoaDon.Rows[e.RowIndex].Cells["MaHD"].Value.ToString();
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string sql = @"SELECT TOP 1 s.TenSach, ct.SoLuong, ct.DonGia, ct.ThanhTien FROM ChiTietHoaDon ct JOIN Sach s ON ct.MaSach = s.MaSach WHERE ct.MaHD = @ma";
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@ma", maHD);
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        textBox1.Text = reader["TenSach"].ToString();
                        textBox2.Text = reader["SoLuong"].ToString();
                        textBox3.Text = reader["DonGia"].ToString();
                        textBox4.Text = reader["ThanhTien"].ToString();
                    }
                    reader.Close();
                }
            }
        }

        private void BtnSearch_Click(object sender, EventArgs e) { LoadLichSuHoaDon(); }

        private void btnResetFilter_Click(object sender, EventArgs e)
        {
            txtSearch.Clear();
            dtpTuNgay.Value = DateTime.Now;
            dtpDenNgay.Value = DateTime.Now;
            LoadLichSuHoaDon();
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            if (dgvHoaDon.CurrentRow == null) return;
            string maHD = dgvHoaDon.CurrentRow.Cells["MaHD"].Value.ToString();
            if (MessageBox.Show($"Xác nhận HỦY hóa đơn {maHD} và HOÀN KHO?", "Cảnh báo", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlTransaction trans = conn.BeginTransaction();
                    try
                    {
                        DataTable dt = new DataTable();
                        SqlDataAdapter da = new SqlDataAdapter("SELECT MaSach, SoLuong FROM ChiTietHoaDon WHERE MaHD=@ma", conn);
                        da.SelectCommand.Transaction = trans; da.SelectCommand.Parameters.AddWithValue("@ma", maHD);
                        da.Fill(dt);
                        foreach (DataRow r in dt.Rows)
                        {
                            new SqlCommand($"UPDATE Sach SET SoLuongTon = SoLuongTon + {r["SoLuong"]} WHERE MaSach='{r["MaSach"]}'", conn, trans).ExecuteNonQuery();
                        }
                        new SqlCommand($"DELETE FROM ChiTietHoaDon WHERE MaHD='{maHD}'", conn, trans).ExecuteNonQuery();
                        new SqlCommand($"DELETE FROM HoaDon WHERE MaHD='{maHD}'", conn, trans).ExecuteNonQuery();
                        trans.Commit();
                        MessageBox.Show("Hủy hóa đơn thành công!");
                        LoadLichSuHoaDon(); LoadComboBoxSach();
                    }
                    catch (Exception ex) { trans.Rollback(); MessageBox.Show("Lỗi: " + ex.Message); }
                }
            }
        }

        private void btnIn_Click(object sender, EventArgs e)
        {
            if (dgvHoaDon.CurrentRow != null)
                MessageBox.Show("Đang in lại hóa đơn: " + dgvHoaDon.CurrentRow.Cells["MaHD"].Value.ToString());
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
            txtGiamGia.Clear();
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