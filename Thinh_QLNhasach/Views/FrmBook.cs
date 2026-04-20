using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using Thinh_QLNhasach.Database;

namespace Thinh_QLNhasach.Views
{
    public partial class FrmBook : Form
    {
        // ==================== KHAI BÁO BIẾN TOÀN CỤC ====================
        private string connStr = DbConnection.connStr;
        private DataTable dtSachTam = new DataTable();
        private DataTable dtTLTam = new DataTable();
        private DataTable dtNXBTam = new DataTable(); // Thêm bảng tạm cho NXB

        // Biến kiểm soát vùng chờ cho TAB SÁCH
        private int indexChonSach = -1;
        private bool isEditingSach = false;

        // Biến kiểm soát vùng chờ cho TAB THỂ LOẠI
        private int indexChonTL = -1;
        private bool isEditingTL = false;

        // Biến kiểm soát vùng chờ cho TAB NXB
        private int indexChonNXB = -1;
        private bool isEditingNXB = false;

        public FrmBook()
        {
            InitializeComponent();
        }

        private void FrmBook_Load(object sender, EventArgs e)
        {
            SetupDataTableTam();
            LoadDataTuDatabase();

            dgvBook.DataSource = dtSachTam;
            dgvTheLoai.DataSource = dtTLTam;
            dgvNXB.DataSource = dtNXBTam; // Gán nguồn dữ liệu cho bảng NXB

            LoadComboBoxData();
            ApplyCustomInterface(); // Áp dụng giao diện xám trắng
        }

        // ==================== 1. VÙNG GIAO DIỆN (LÀM ĐẸP GRID) ====================
        private void ApplyCustomInterface()
        {
            // --- GIAO DIỆN BẢNG SÁCH (DGVBOOK) ---
            dgvBook.Theme = Guna.UI2.WinForms.Enums.DataGridViewPresetThemes.Default;
            dgvBook.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvBook.RowHeadersVisible = false;
            dgvBook.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            dgvBook.GridColor = Color.FromArgb(231, 229, 255);
            dgvBook.BorderStyle = BorderStyle.FixedSingle;

            dgvBook.EnableHeadersVisualStyles = false;
            dgvBook.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvBook.ColumnHeadersHeight = 45;
            dgvBook.ColumnHeadersDefaultCellStyle.BackColor = Color.Gray;
            dgvBook.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvBook.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 12, FontStyle.Bold);

            dgvBook.DefaultCellStyle.BackColor = Color.White;
            dgvBook.DefaultCellStyle.ForeColor = Color.Black;
            dgvBook.DefaultCellStyle.Font = new Font("Segoe UI", 11);
            dgvBook.DefaultCellStyle.SelectionBackColor = Color.FromArgb(231, 229, 255);
            dgvBook.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgvBook.RowTemplate.Height = 40;
            dgvBook.BackgroundColor = Color.White;

            if (dgvBook.Columns.Count > 0)
            {
                dgvBook.Columns["MaSach"].HeaderText = "Mã";
                dgvBook.Columns["TenSach"].HeaderText = "Tên Sách";
                dgvBook.Columns["TenTG"].HeaderText = "Tác Giả";
                dgvBook.Columns["TenTL"].HeaderText = "Thể Loại";
                dgvBook.Columns["TenNXB"].HeaderText = "Nhà XB";
                dgvBook.Columns["NamXuatBan"].HeaderText = "Năm XB";
                dgvBook.Columns["GiaNhap"].HeaderText = "Giá Nhập";
                dgvBook.Columns["GiaBan"].HeaderText = "Giá Bán";
                dgvBook.Columns["SoLuongTon"].HeaderText = "Tồn Kho";
                dgvBook.Columns["MoTa"].HeaderText = "Mô Tả";

                dgvBook.Columns["MaSach"].FillWeight = 40;
                dgvBook.Columns["TenSach"].FillWeight = 160;
                dgvBook.Columns["TenTG"].FillWeight = 110;
                dgvBook.Columns["TenTL"].FillWeight = 90;
                dgvBook.Columns["TenNXB"].FillWeight = 100;
                dgvBook.Columns["NamXuatBan"].FillWeight = 70;
                dgvBook.Columns["GiaNhap"].FillWeight = 80;
                dgvBook.Columns["GiaBan"].FillWeight = 80;
                dgvBook.Columns["SoLuongTon"].FillWeight = 60;
                dgvBook.Columns["MoTa"].FillWeight = 120;
            }

            // --- GIAO DIỆN BẢNG THỂ LOẠI (DGVTHELOAI) ---
            dgvTheLoai.Theme = Guna.UI2.WinForms.Enums.DataGridViewPresetThemes.Default;
            dgvTheLoai.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvTheLoai.RowHeadersVisible = false;
            dgvTheLoai.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            dgvTheLoai.GridColor = Color.FromArgb(231, 229, 255);
            dgvTheLoai.BorderStyle = BorderStyle.FixedSingle;

            dgvTheLoai.EnableHeadersVisualStyles = false;
            dgvTheLoai.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvTheLoai.ColumnHeadersHeight = 45;
            dgvTheLoai.ColumnHeadersDefaultCellStyle.BackColor = Color.Gray;
            dgvTheLoai.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvTheLoai.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 12, FontStyle.Bold);

            dgvTheLoai.DefaultCellStyle.Font = new Font("Segoe UI", 11);
            dgvTheLoai.DefaultCellStyle.SelectionBackColor = Color.FromArgb(231, 229, 255);
            dgvTheLoai.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgvTheLoai.RowTemplate.Height = 40;
            dgvTheLoai.BackgroundColor = Color.White;

            if (dgvTheLoai.Columns.Count > 0)
            {
                dgvTheLoai.Columns["MaTL"].HeaderText = "Mã TL";
                dgvTheLoai.Columns["TenTL"].HeaderText = "Tên Thể Loại";
                dgvTheLoai.Columns["MoTa"].HeaderText = "Mô Tả";
                dgvTheLoai.Columns["MaTL"].FillWeight = 50;
                dgvTheLoai.Columns["TenTL"].FillWeight = 150;
                dgvTheLoai.Columns["MoTa"].FillWeight = 200;
            }

            // --- GIAO DIỆN BẢNG NHÀ XUẤT BẢN (DGVNXB) ---
            dgvNXB.Theme = Guna.UI2.WinForms.Enums.DataGridViewPresetThemes.Default;
            dgvNXB.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvNXB.RowHeadersVisible = false;
            dgvNXB.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            dgvNXB.GridColor = Color.FromArgb(231, 229, 255);
            dgvNXB.BorderStyle = BorderStyle.FixedSingle;

            dgvNXB.EnableHeadersVisualStyles = false;
            dgvNXB.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvNXB.ColumnHeadersHeight = 45;
            dgvNXB.ColumnHeadersDefaultCellStyle.BackColor = Color.Gray;
            dgvNXB.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvNXB.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 12, FontStyle.Bold);

            dgvNXB.DefaultCellStyle.Font = new Font("Segoe UI", 11);
            dgvNXB.DefaultCellStyle.SelectionBackColor = Color.FromArgb(231, 229, 255);
            dgvNXB.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgvNXB.RowTemplate.Height = 40;
            dgvNXB.BackgroundColor = Color.White;

            if (dgvNXB.Columns.Count > 0)
            {
                dgvNXB.Columns["MaNXB"].HeaderText = "Mã NXB";
                dgvNXB.Columns["TenNXB"].HeaderText = "Tên Nhà Xuất Bản";
                dgvNXB.Columns["DiaChi"].HeaderText = "Địa Chỉ";
                dgvNXB.Columns["SoDienThoai"].HeaderText = "Số Điện Thoại";
                dgvNXB.Columns["Email"].HeaderText = "Email";
            }
        }

        // ==================== 2. TAB SÁCH (LOGIC VÙNG CHỜ) ====================

        private void BocDuLieuSach(int index)
        {
            DataRow dr = dtSachTam.Rows[index];
            txtMasach.Text = dr["MaSach"].ToString();
            txtTensach.Text = dr["TenSach"].ToString();
            cboMaTG.Text = dr["TenTG"].ToString();
            cboMaTL.Text = dr["TenTL"].ToString();
            cboMaNXB.Text = dr["TenNXB"].ToString();
            txtGiaNhap.Text = dr["GiaNhap"].ToString();
            txtGiaBan.Text = dr["GiaBan"].ToString();
            txtTonKho.Text = dr["SoLuongTon"].ToString();
            txtMoTa.Text = dr["MoTa"].ToString();
        }

        private void dgvBook_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            indexChonSach = e.RowIndex;
            if (isEditingSach) BocDuLieuSach(indexChonSach);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            isEditingSach = false;
            if (string.IsNullOrWhiteSpace(txtTensach.Text)) { MessageBox.Show("Vui lòng nhập tên sách!"); return; }

            DataRow dr = dtSachTam.NewRow();
            dr["TenSach"] = txtTensach.Text.Trim();
            dr["TenTG"] = cboMaTG.Text;
            dr["TenTL"] = cboMaTL.Text;
            dr["TenNXB"] = cboMaNXB.Text;
            dr["GiaNhap"] = txtGiaNhap.Text;
            dr["GiaBan"] = txtGiaBan.Text;
            dr["SoLuongTon"] = txtTonKho.Text;
            dr["MoTa"] = txtMoTa.Text.Trim();
            dtSachTam.Rows.Add(dr);

            MessageBox.Show("Đã thêm tạm! Nhấn Save để chốt vào DB.");
            btnReset_Click(null, null);
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (indexChonSach == -1) { MessageBox.Show("Hãy chọn một dòng sách dưới bảng!"); return; }

            if (!isEditingSach)
            {
                isEditingSach = true;
                BocDuLieuSach(indexChonSach);
                MessageBox.Show("Chế độ Sửa bật! Click dòng nào bốc dòng đó.");
            }
            else
            {
                DataRow dr = dtSachTam.Rows[indexChonSach];
                dr["TenSach"] = txtTensach.Text.Trim();
                dr["TenTG"] = cboMaTG.Text;
                dr["TenTL"] = cboMaTL.Text;
                dr["TenNXB"] = cboMaNXB.Text;
                dr["GiaNhap"] = txtGiaNhap.Text;
                dr["GiaBan"] = txtGiaBan.Text;
                dr["SoLuongTon"] = txtTonKho.Text;
                dr["MoTa"] = txtMoTa.Text.Trim();

                MessageBox.Show("Cập nhật danh sách chờ thành công!");
            }
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            if (dgvBook.CurrentRow != null)
            {
                dtSachTam.Rows.RemoveAt(dgvBook.CurrentRow.Index);
                indexChonSach = -1;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            dgvBook.EndEdit();
            LuuDatabase("Sach");
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string filter = string.Format("TenSach LIKE '%{0}%' OR Convert(MaSach, 'System.String') LIKE '%{0}%'", txtSearch.Text.Trim());
            dtSachTam.DefaultView.RowFilter = filter;
        }

        // ==================== 3. TAB THỂ LOẠI (LOGIC VÙNG CHỜ) ====================

        private void BocDuLieuTheLoai(int index)
        {
            DataRow dr = dtTLTam.Rows[index];
            txtMaTL.Text = dr["MaTL"].ToString();
            txtTenTL.Text = dr["TenTL"].ToString();
            textMoTa.Text = dr["MoTa"].ToString();
        }

        private void dgvTheLoai_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            indexChonTL = e.RowIndex;
            if (isEditingTL) BocDuLieuTheLoai(indexChonTL);
        }

        private void btnAddTL_Click(object sender, EventArgs e)
        {
            isEditingTL = false;
            if (string.IsNullOrWhiteSpace(txtTenTL.Text)) { MessageBox.Show("Vui lòng nhập tên thể loại!"); return; }

            DataRow dr = dtTLTam.NewRow();
            dr["TenTL"] = txtTenTL.Text.Trim();
            dr["MoTa"] = textMoTa.Text.Trim();
            dtTLTam.Rows.Add(dr);

            MessageBox.Show("Đã thêm tạm! Nhấn Save để chốt vào DB.");
            btnResetTL_Click(null, null);
        }

        private void btnEditTL_Click(object sender, EventArgs e)
        {
            if (indexChonTL == -1) { MessageBox.Show("Hãy chọn một dòng thể loại dưới bảng!"); return; }

            if (!isEditingTL)
            {
                isEditingTL = true;
                BocDuLieuTheLoai(indexChonTL);
                MessageBox.Show("Chế độ Sửa bật! Click dòng nào bốc dòng đó.");
            }
            else
            {
                DataRow dr = dtTLTam.Rows[indexChonTL];
                dr["TenTL"] = txtTenTL.Text.Trim();
                dr["MoTa"] = textMoTa.Text.Trim();

                MessageBox.Show("Cập nhật danh sách chờ thành công!");
            }
        }

        private void btnDelTL_Click(object sender, EventArgs e)
        {
            if (dgvTheLoai.CurrentRow != null)
            {
                dtTLTam.Rows.RemoveAt(dgvTheLoai.CurrentRow.Index);
                indexChonTL = -1;
            }
        }

        private void btnSaveTL_Click(object sender, EventArgs e)
        {
            dgvTheLoai.EndEdit();
            LuuDatabase("TheLoai");
        }

        private void btnSearchTL_Click(object sender, EventArgs e)
        {
            string filter = string.Format("TenTL LIKE '%{0}%' OR Convert(MaTL, 'System.String') LIKE '%{0}%'", txtSearchTL.Text.Trim());
            dtTLTam.DefaultView.RowFilter = filter;
        }

        // ==================== 4. TAB NHÀ XUẤT BẢN (LOGIC VÙNG CHỜ) ====================

        private void BocDuLieuNXB(int index)
        {
            DataRow dr = dtNXBTam.Rows[index];
            txtMaNXB.Text = dr["MaNXB"].ToString();
            txtTenNXB.Text = dr["TenNXB"].ToString();
            txtDiachi.Text = dr["DiaChi"].ToString();
            txtSdt.Text = dr["SoDienThoai"].ToString();
            txtEmail.Text = dr["Email"].ToString();
        }

        private void dgvNXB_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            indexChonNXB = e.RowIndex;
            if (isEditingNXB) BocDuLieuNXB(indexChonNXB);
        }

        private void btnAddNXB_Click(object sender, EventArgs e)
        {
            isEditingNXB = false;
            if (string.IsNullOrWhiteSpace(txtTenNXB.Text)) { MessageBox.Show("Vui lòng nhập tên Nhà xuất bản!"); return; }

            DataRow dr = dtNXBTam.NewRow();
            dr["TenNXB"] = txtTenNXB.Text.Trim();
            dr["DiaChi"] = txtDiachi.Text.Trim();
            dr["SoDienThoai"] = txtSdt.Text.Trim();
            dr["Email"] = txtEmail.Text.Trim();
            dtNXBTam.Rows.Add(dr);

            MessageBox.Show("Đã thêm tạm! Nhấn Save để chốt vào DB.");
            btnResetNXB_Click(null, null);
        }

        private void btnEditNXB_Click(object sender, EventArgs e)
        {
            if (indexChonNXB == -1) { MessageBox.Show("Hãy chọn một dòng nhà xuất bản dưới bảng!"); return; }

            if (!isEditingNXB)
            {
                isEditingNXB = true;
                BocDuLieuNXB(indexChonNXB);
                MessageBox.Show("Chế độ Sửa bật! Click dòng nào bốc dòng đó.");
            }
            else
            {
                DataRow dr = dtNXBTam.Rows[indexChonNXB];
                dr["TenNXB"] = txtTenNXB.Text.Trim();
                dr["DiaChi"] = txtDiachi.Text.Trim();
                dr["SoDienThoai"] = txtSdt.Text.Trim();
                dr["Email"] = txtEmail.Text.Trim();

                MessageBox.Show("Cập nhật danh sách chờ thành công!");
            }
        }

        private void btnDelNXB_Click(object sender, EventArgs e)
        {
            if (dgvNXB.CurrentRow != null)
            {
                dtNXBTam.Rows.RemoveAt(dgvNXB.CurrentRow.Index);
                indexChonNXB = -1;
            }
        }

        private void btnSaveNXB_Click(object sender, EventArgs e)
        {
            dgvNXB.EndEdit();
            LuuDatabase("NXB");
        }

        private void btnSearchNXB_Click(object sender, EventArgs e)
        {
            string filter = string.Format("TenNXB LIKE '%{0}%' OR Convert(MaNXB, 'System.String') LIKE '%{0}%'", txtSearchNXB.Text.Trim());
            dtNXBTam.DefaultView.RowFilter = filter;
        }

        private void btnResetNXB_Click(object sender, EventArgs e)
        {
            txtMaNXB.Clear();
            txtTenNXB.Clear();
            txtDiachi.Clear();
            txtSdt.Clear();
            txtEmail.Clear();
            txtSearchNXB.Clear();

            if (dtNXBTam != null) dtNXBTam.DefaultView.RowFilter = "";

            indexChonNXB = -1;
            isEditingNXB = false;
        }

        // ==================== 5. LOGIC LƯU DATABASE (TRANSACTION) ====================
        private void LuuDatabase(string loai)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                SqlTransaction trans = conn.BeginTransaction();
                try
                {
                    if (loai == "Sach")
                    {
                        foreach (DataRow dr in dtSachTam.Rows)
                        {
                            if (dr.RowState == DataRowState.Deleted) continue;

                            string sql = @"IF EXISTS (SELECT 1 FROM Sach WHERE MaSach = @ma)
                                UPDATE Sach SET TenSach=@ten, MaTG=(SELECT TOP 1 MaTG FROM TacGia WHERE TenTG=@tg), 
                                MaTL=(SELECT TOP 1 MaTL FROM TheLoai WHERE TenTL=@tl), MaNXB=(SELECT TOP 1 MaNXB FROM NhaXuatBan WHERE TenNXB=@nxb), 
                                GiaNhap=@gn, GiaBan=@gb, SoLuongTon=@sl, MoTa=@mt WHERE MaSach=@ma
                                ELSE INSERT INTO Sach (TenSach, MaTG, MaTL, MaNXB, GiaNhap, GiaBan, SoLuongTon, MoTa) 
                                VALUES (@ten, (SELECT TOP 1 MaTG FROM TacGia WHERE TenTG=@tg), (SELECT TOP 1 MaTL FROM TheLoai WHERE TenTL=@tl), 
                                (SELECT TOP 1 MaNXB FROM NhaXuatBan WHERE TenNXB=@nxb), @gn, @gb, @sl, @mt)";

                            SqlCommand cmd = new SqlCommand(sql, conn, trans);
                            cmd.Parameters.AddWithValue("@ma", dr["MaSach"] == DBNull.Value ? -1 : dr["MaSach"]);
                            cmd.Parameters.AddWithValue("@ten", dr["TenSach"]);
                            cmd.Parameters.AddWithValue("@tg", dr["TenTG"]);
                            cmd.Parameters.AddWithValue("@tl", dr["TenTL"]);
                            cmd.Parameters.AddWithValue("@nxb", dr["TenNXB"]);
                            cmd.Parameters.AddWithValue("@gn", dr["GiaNhap"]);
                            cmd.Parameters.AddWithValue("@gb", dr["GiaBan"]);
                            cmd.Parameters.AddWithValue("@sl", dr["SoLuongTon"]);
                            cmd.Parameters.AddWithValue("@mt", dr["MoTa"]);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    else if (loai == "TheLoai")
                    {
                        foreach (DataRow dr in dtTLTam.Rows)
                        {
                            if (dr.RowState == DataRowState.Deleted) continue;

                            string sql = @"IF EXISTS (SELECT 1 FROM TheLoai WHERE MaTL = @ma)
                                UPDATE TheLoai SET TenTL=@ten, MoTa=@mt WHERE MaTL=@ma
                                ELSE INSERT INTO TheLoai (TenTL, MoTa) VALUES (@ten, @mt)";

                            SqlCommand cmd = new SqlCommand(sql, conn, trans);
                            cmd.Parameters.AddWithValue("@ma", dr["MaTL"] == DBNull.Value ? -1 : dr["MaTL"]);
                            cmd.Parameters.AddWithValue("@ten", dr["TenTL"]);
                            cmd.Parameters.AddWithValue("@mt", dr["MoTa"]);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    else if (loai == "NXB")
                    {
                        foreach (DataRow dr in dtNXBTam.Rows)
                        {
                            if (dr.RowState == DataRowState.Deleted) continue;

                            string sql = @"IF EXISTS (SELECT 1 FROM NhaXuatBan WHERE MaNXB = @ma)
                                UPDATE NhaXuatBan SET TenNXB=@ten, DiaChi=@dc, SoDienThoai=@sdt, Email=@email WHERE MaNXB=@ma
                                ELSE INSERT INTO NhaXuatBan (TenNXB, DiaChi, SoDienThoai, Email) VALUES (@ten, @dc, @sdt, @email)";

                            SqlCommand cmd = new SqlCommand(sql, conn, trans);
                            cmd.Parameters.AddWithValue("@ma", dr["MaNXB"] == DBNull.Value ? -1 : dr["MaNXB"]);
                            cmd.Parameters.AddWithValue("@ten", dr["TenNXB"]);
                            cmd.Parameters.AddWithValue("@dc", dr["DiaChi"]);
                            cmd.Parameters.AddWithValue("@sdt", dr["SoDienThoai"]);
                            cmd.Parameters.AddWithValue("@email", dr["Email"]);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    trans.Commit();
                    MessageBox.Show("Đã lưu cứng lả lướt vào Database!");
                    LoadDataTuDatabase();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    MessageBox.Show("Lỗi lưu DB: " + ex.Message);
                }
            }
        }

        // ==================== 6. LOAD DỮ LIỆU GỐC & COMBOBOX ====================
        private void SetupDataTableTam()
        {
            if (dtSachTam.Columns.Count == 0)
            {
                dtSachTam.Columns.Add("MaSach", typeof(int));
                dtSachTam.Columns.Add("TenSach");
                dtSachTam.Columns.Add("TenTG");
                dtSachTam.Columns.Add("TenTL");
                dtSachTam.Columns.Add("TenNXB");
                dtSachTam.Columns.Add("NamXuatBan");
                dtSachTam.Columns.Add("GiaNhap");
                dtSachTam.Columns.Add("GiaBan");
                dtSachTam.Columns.Add("SoLuongTon");
                dtSachTam.Columns.Add("MoTa");
            }
            if (dtTLTam.Columns.Count == 0)
            {
                dtTLTam.Columns.Add("MaTL", typeof(int));
                dtTLTam.Columns.Add("TenTL");
                dtTLTam.Columns.Add("MoTa");
            }
            if (dtNXBTam.Columns.Count == 0)
            {
                dtNXBTam.Columns.Add("MaNXB", typeof(int));
                dtNXBTam.Columns.Add("TenNXB");
                dtNXBTam.Columns.Add("DiaChi");
                dtNXBTam.Columns.Add("SoDienThoai");
                dtNXBTam.Columns.Add("Email");
            }
        }

        private void LoadDataTuDatabase()
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlDataAdapter daS = new SqlDataAdapter(@"SELECT s.MaSach, s.TenSach, tg.TenTG, tl.TenTL, nxb.TenNXB, 
                    s.NamXuatBan, s.GiaNhap, s.GiaBan, s.SoLuongTon, s.MoTa 
                    FROM Sach s 
                    LEFT JOIN TacGia tg ON s.MaTG = tg.MaTG 
                    LEFT JOIN TheLoai tl ON s.MaTL = tl.MaTL 
                    LEFT JOIN NhaXuatBan nxb ON s.MaNXB = nxb.MaNXB", conn);
                dtSachTam.Clear();
                daS.Fill(dtSachTam);

                SqlDataAdapter daT = new SqlDataAdapter("SELECT MaTL, TenTL, MoTa FROM TheLoai", conn);
                dtTLTam.Clear();
                daT.Fill(dtTLTam);

                SqlDataAdapter daNXB = new SqlDataAdapter("SELECT MaNXB, TenNXB, DiaChi, SoDienThoai, Email FROM NhaXuatBan", conn);
                dtNXBTam.Clear();
                daNXB.Fill(dtNXBTam);
            }
        }

        private void LoadComboBoxData()
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                SqlDataAdapter daTG = new SqlDataAdapter("SELECT MaTG, TenTG FROM TacGia", conn);
                DataTable dtTG = new DataTable(); daTG.Fill(dtTG);
                cboMaTG.DataSource = dtTG; cboMaTG.DisplayMember = "TenTG"; cboMaTG.ValueMember = "MaTG";

                SqlDataAdapter daTL = new SqlDataAdapter("SELECT MaTL, TenTL FROM TheLoai", conn);
                DataTable dtTL = new DataTable(); daTL.Fill(dtTL);
                cboMaTL.DataSource = dtTL; cboMaTL.DisplayMember = "TenTL"; cboMaTL.ValueMember = "MaTL";

                SqlDataAdapter daNXB = new SqlDataAdapter("SELECT MaNXB, TenNXB FROM NhaXuatBan", conn);
                DataTable dtNXB = new DataTable(); daNXB.Fill(dtNXB);
                cboMaNXB.DataSource = dtNXB; cboMaNXB.DisplayMember = "TenNXB"; cboMaNXB.ValueMember = "MaNXB";
            }
        }

        // ==================== 7. CÁC NÚT RESET LÀM MỚI ====================
        private void btnReset_Click(object sender, EventArgs e)
        {
            txtMasach.Clear();
            txtTensach.Clear();
            txtGiaNhap.Clear();
            txtGiaBan.Clear();
            txtTonKho.Clear();
            txtMoTa.Clear();
            txtSearch.Clear();

            if (cboMaTG.Items.Count > 0) cboMaTG.SelectedIndex = 0;
            if (cboMaTL.Items.Count > 0) cboMaTL.SelectedIndex = 0;
            if (cboMaNXB.Items.Count > 0) cboMaNXB.SelectedIndex = 0;

            if (dtSachTam != null) dtSachTam.DefaultView.RowFilter = "";

            indexChonSach = -1;
            isEditingSach = false;
        }

        private void btnResetTL_Click(object sender, EventArgs e)
        {
            txtMaTL.Clear();
            txtTenTL.Clear();
            textMoTa.Clear();
            txtSearchTL.Clear();

            if (dtTLTam != null) dtTLTam.DefaultView.RowFilter = "";

            indexChonTL = -1;
            isEditingTL = false;
        }
    }
}