using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace Thinh_QLNhasach
{
    public partial class FrmAccount : Form
    {
        string connStr = @"Data Source=THINHLALUOT\SQLEXPRESS01;Initial Catalog=BookShop;Integrated Security=True";
        DataTable dtTam = new DataTable();
        int indexChon = -1;
        bool isEditing = false;

        public FrmAccount()
        {
            InitializeComponent();
            InitComboBoxes();
            LoadDataTuDatabase();
        }

        private void FrmAccount_Load(object sender, EventArgs e)
        {
            if (dtTam.Columns.Count == 0)
            {
                dtTam.Columns.Add("MaND", typeof(int));
                dtTam.Columns.Add("TenDangNhap", typeof(string));
                dtTam.Columns.Add("MatKhau", typeof(object)); // Để object vì SQL trả về byte[]
                dtTam.Columns.Add("HoTen", typeof(string));
                dtTam.Columns.Add("NgaySinh", typeof(DateTime));
                dtTam.Columns.Add("SoDienThoai", typeof(string));
                dtTam.Columns.Add("VaiTro", typeof(string));
                dtTam.Columns.Add("TrangThai", typeof(string));
            }
            LoadDataTuDatabase();
        }

        private void InitComboBoxes()
        {
            cboTrangthai.Items.Clear();
            cboTrangthai.Items.AddRange(new object[] { "Còn làm", "Nghỉ việc" });
            if (cboTrangthai.Items.Count > 0) cboTrangthai.SelectedIndex = 0;

            cboRole.Items.Clear();
            cboRole.Items.AddRange(new object[] { "Admin", "Staff", "Nhân viên" });
            if (cboRole.Items.Count > 0) cboRole.SelectedIndex = 0;
        }

        private void LoadDataTuDatabase()
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    // LƯU Ý: Không load cột MatKhau lên Grid để bảo mật và tránh lỗi hiển thị byte[]
                    SqlDataAdapter da = new SqlDataAdapter("SELECT MaND, TenDangNhap, HoTen, NgaySinh, SoDienThoai, VaiTro, TrangThai, MatKhau FROM NguoiDung", conn);
                    dtTam.Clear();
                    da.Fill(dtTam);
                    dgvAccount.DataSource = dtTam;
                    FormatGridUI();
                    indexChon = -1;
                    isEditing = false;
                }
                catch (Exception ex) { MessageBox.Show("Lỗi load DB: " + ex.Message); }
            }
        }

        private void FormatGridUI()
        {
            dgvAccount.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvAccount.RowHeadersVisible = false;
            dgvAccount.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            dgvAccount.GridColor = Color.FromArgb(231, 229, 255);
            dgvAccount.EnableHeadersVisualStyles = false;
            dgvAccount.ColumnHeadersHeight = 45;

            // --- ĐÂY LÀ PHÔNG CHỮ CHO TIÊU ĐỀ CỘT ---
            dgvAccount.ColumnHeadersDefaultCellStyle.BackColor = Color.Gray;
            dgvAccount.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvAccount.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 12, FontStyle.Bold);

            dgvAccount.RowTemplate.Height = 40;

            // --- ĐÂY LÀ CHỖ TÔI VỪA THÊM ĐỂ FIX CHỮ BÉ CHO ÔNG ---
            dgvAccount.DefaultCellStyle.BackColor = Color.White;
            dgvAccount.DefaultCellStyle.ForeColor = Color.Black;
            dgvAccount.DefaultCellStyle.Font = new Font("Segoe UI", 11); // Tăng size chữ lên 11
            dgvAccount.DefaultCellStyle.SelectionBackColor = Color.FromArgb(231, 229, 255);
            dgvAccount.DefaultCellStyle.SelectionForeColor = Color.Black;
            // ------------------------------------------------------

            if (dgvAccount.Columns.Count > 0)
            {
                dgvAccount.Columns["MaND"].HeaderText = "Mã";
                dgvAccount.Columns["TenDangNhap"].HeaderText = "Tên Đăng Nhập";
                dgvAccount.Columns["MatKhau"].Visible = false; // ẨN CỘT MẬT KHẨU ĐÃ MÃ HÓA ĐI
                dgvAccount.Columns["HoTen"].HeaderText = "Họ Tên";
                dgvAccount.Columns["NgaySinh"].HeaderText = "Ngày Sinh";
                dgvAccount.Columns["NgaySinh"].DefaultCellStyle.Format = "dd/MM/yyyy";
                dgvAccount.Columns["SoDienThoai"].HeaderText = "SĐT";
                dgvAccount.Columns["VaiTro"].HeaderText = "Vai Trò";
                dgvAccount.Columns["TrangThai"].HeaderText = "Trạng Thái";
            }
        }

        private void dgvAccount_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                indexChon = e.RowIndex;
                if (isEditing) BocDuLieuLenForm(indexChon);
            }
        }

        private void BocDuLieuLenForm(int index)
        {
            DataRow dr = dtTam.Rows[index];
            txtUser.Text = dr["TenDangNhap"].ToString();
            txtPass.Text = ""; // Luôn để trống pass khi bốc lên để bảo mật
            txtHoten.Text = dr["HoTen"].ToString();
            txtSdt.Text = dr["SoDienThoai"].ToString();

            string vaitro = dr["VaiTro"].ToString();
            if (cboRole.Items.Contains(vaitro)) cboRole.SelectedItem = vaitro;

            string trangthai = dr["TrangThai"].ToString();
            if (cboTrangthai.Items.Contains(trangthai)) cboTrangthai.SelectedItem = trangthai;

            dtpNgaysinh.Value = dr["NgaySinh"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(dr["NgaySinh"]);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            isEditing = false;
            if (string.IsNullOrWhiteSpace(txtUser.Text) || string.IsNullOrWhiteSpace(txtPass.Text))
            {
                MessageBox.Show("Vui lòng nhập đủ Tài khoản và Mật khẩu!");
                return;
            }

            DataRow dr = dtTam.NewRow();
            dr["TenDangNhap"] = txtUser.Text.Trim();
            dr["MatKhau"] = txtPass.Text.Trim(); // Gán tạm text thường, tí nữa SQL tự băm
            dr["HoTen"] = txtHoten.Text.Trim();
            dr["NgaySinh"] = dtpNgaysinh.Value;
            dr["SoDienThoai"] = txtSdt.Text.Trim();
            dr["VaiTro"] = cboRole.Text;
            dr["TrangThai"] = cboTrangthai.Text;
            dtTam.Rows.Add(dr);

            MessageBox.Show("Đã thêm tạm! Nhấn Save để chốt vào DB.");
            ClearForm();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (indexChon == -1) { MessageBox.Show("Chọn một dòng dưới bảng trước!"); return; }

            if (!isEditing)
            {
                isEditing = true;
                BocDuLieuLenForm(indexChon);
                MessageBox.Show("Chế độ Sửa bật! Click dòng nào bốc dòng đó.");
            }
            else
            {
                DataRow dr = dtTam.Rows[indexChon];
                dr["HoTen"] = txtHoten.Text.Trim();
                dr["NgaySinh"] = dtpNgaysinh.Value;
                dr["SoDienThoai"] = txtSdt.Text.Trim();
                dr["VaiTro"] = cboRole.Text;
                dr["TrangThai"] = cboTrangthai.Text;

                // Nếu có nhập pass mới thì mới gán vào
                if (!string.IsNullOrWhiteSpace(txtPass.Text))
                    dr["MatKhau"] = txtPass.Text.Trim();

                MessageBox.Show("Cập nhật danh sách chờ thành công!");
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            dgvAccount.EndEdit();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlTransaction trans = null;
                try
                {
                    conn.Open();
                    trans = conn.BeginTransaction();
                    foreach (DataRow dr in dtTam.Rows)
                    {
                        if (dr.RowState == DataRowState.Deleted) continue;

                        bool isNew = (dr["MaND"] == DBNull.Value || Convert.ToInt32(dr["MaND"]) <= 0);
                        string sql = "";

                        // 1. CHIA LOGIC RÕ RÀNG CHO INSERT VÀ UPDATE
                        if (isNew)
                        {
                            // Thêm mới: Bắt buộc băm mật khẩu và phải có CAST(... AS VARCHAR) cho chuẩn
                            sql = "INSERT INTO NguoiDung (TenDangNhap, MatKhau, HoTen, NgaySinh, SoDienThoai, VaiTro, TrangThai) " +
                                  "VALUES (@u, HASHBYTES('MD5', CAST(@p AS VARCHAR(MAX))), @t, @n, @s, @r, @tt)";
                        }
                        else
                        {
                            // Sửa: Kiểm tra xem cột mật khẩu trong DataTable đang là chữ (gõ mới) hay mảng byte (từ DB lên)
                            if (dr["MatKhau"] is string)
                            {
                                // Có gõ pass mới -> Cập nhật cả cột MatKhau
                                sql = "UPDATE NguoiDung SET MatKhau = HASHBYTES('MD5', CAST(@p AS VARCHAR(MAX))), " +
                                      "HoTen=@t, NgaySinh=@n, SoDienThoai=@s, VaiTro=@r, TrangThai=@tt WHERE MaND=@ma";
                            }
                            else
                            {
                                // Giữ nguyên pass cũ (nó đang là byte[]) -> BỎ QUA CỘT MatKhau, chỉ update thông tin khác
                                sql = "UPDATE NguoiDung SET HoTen=@t, NgaySinh=@n, SoDienThoai=@s, VaiTro=@r, TrangThai=@tt WHERE MaND=@ma";
                            }
                        }

                        using (SqlCommand cmd = new SqlCommand(sql, conn, trans))
                        {
                            if (!isNew) cmd.Parameters.AddWithValue("@ma", dr["MaND"]);

                            cmd.Parameters.AddWithValue("@u", dr["TenDangNhap"]);

                            // 2. CHỈ TRUYỀN PARAMETER @p NẾU CÂU SQL CÓ DÙNG TỚI NÓ
                            if (isNew || dr["MatKhau"] is string)
                            {
                                cmd.Parameters.AddWithValue("@p", dr["MatKhau"].ToString());
                            }

                            cmd.Parameters.AddWithValue("@t", dr["HoTen"]);

                            // Xử lý an toàn cho ngày sinh nếu lỡ bị Null
                            cmd.Parameters.AddWithValue("@n", dr["NgaySinh"] == DBNull.Value ? (object)DBNull.Value : dr["NgaySinh"]);

                            cmd.Parameters.AddWithValue("@s", dr["SoDienThoai"]);
                            cmd.Parameters.AddWithValue("@r", dr["VaiTro"]);
                            cmd.Parameters.AddWithValue("@tt", dr["TrangThai"]);

                            cmd.ExecuteNonQuery();
                        }
                    }
                    trans.Commit();
                    MessageBox.Show("Đã lưu vào Database thành công lả lướt!");
                    LoadDataTuDatabase();
                }
                catch (Exception ex)
                {
                    if (trans != null) trans.Rollback();
                    MessageBox.Show("Lỗi: " + ex.Message);
                }
            }
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            if (dgvAccount.CurrentRow != null)
            {
                dtTam.Rows.RemoveAt(dgvAccount.CurrentRow.Index);
                indexChon = -1;
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            dtTam.DefaultView.RowFilter = string.Format("TenDangNhap LIKE '%{0}%' OR HoTen LIKE '%{0}%'", txtSearch.Text.Trim());
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            ClearForm();
            LoadDataTuDatabase();
        }

        private void ClearForm()
        {
            txtUser.Clear(); txtPass.Clear(); txtHoten.Clear(); txtSdt.Clear(); txtSearch.Clear();
            indexChon = -1; isEditing = false;
        }
    }
}