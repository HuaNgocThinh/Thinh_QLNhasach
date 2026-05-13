using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace Thinh_QLNhasach
{
    public partial class FrmAccount : Form
    {
        // Chuỗi kết nối Database của đại ca
        string connStr = @"Data Source=THINHLALUOT\SQLEXPRESS01;Initial Catalog=BookShop;Integrated Security=True";

        // CÁC BIẾN KIỂM SOÁT TRẠNG THÁI
        int currentMaND = -1;   // Lưu mã người dùng đang được click chọn
        bool isEditing = false; // Công tắc chế độ Sửa

        public FrmAccount()
        {
            InitializeComponent();
            InitComboBoxes();
            LoadDataTuDatabase();
        }

        private void FrmAccount_Load(object sender, EventArgs e)
        {
            LoadDataTuDatabase();
        }

        // =========================================================
        // KHỞI TẠO COMBOBOX
        // =========================================================
        private void InitComboBoxes()
        {
            cboTrangthai.Items.Clear();
            cboTrangthai.Items.AddRange(new object[] { "Còn làm", "Nghỉ việc" });
            if (cboTrangthai.Items.Count > 0) cboTrangthai.SelectedIndex = 0;

            cboRole.Items.Clear();
            cboRole.Items.AddRange(new object[] { "Admin", "Nhân viên" });
            if (cboRole.Items.Count > 0) cboRole.SelectedIndex = 0;
        }

        // =========================================================
        // TẢI DỮ LIỆU TỪ DATABASE
        // =========================================================
        private void LoadDataTuDatabase(string searchKw = "")
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    // ĐÃ FIX: Xóa cột MatKhau khỏi câu truy vấn
                    string sql = "SELECT MaND, TenDangNhap, HoTen, NgaySinh, SoDienThoai, VaiTro, TrangThai FROM NguoiDung";

                    if (!string.IsNullOrEmpty(searchKw))
                    {
                        sql += " WHERE TenDangNhap LIKE @kw OR HoTen LIKE @kw";
                    }

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    if (!string.IsNullOrEmpty(searchKw))
                        cmd.Parameters.AddWithValue("@kw", "%" + searchKw + "%");

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    dgvAccount.DataSource = dt;
                    FormatGridUI();
                    ClearForm(); // Load xong thì trả form về trạng thái an toàn
                }
                catch (Exception ex) { MessageBox.Show("Lỗi load DB: " + ex.Message, "Lỗi"); }
            }
        }

        // =========================================================
        // LÀM ĐẸP GIAO DIỆN BẢNG DỮ LIỆU
        // =========================================================
        private void FormatGridUI()
        {
            dgvAccount.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvAccount.RowHeadersVisible = false;
            dgvAccount.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            dgvAccount.GridColor = Color.FromArgb(231, 229, 255);
            dgvAccount.EnableHeadersVisualStyles = false;
            dgvAccount.ColumnHeadersHeight = 45;

            // Tiêu đề
            dgvAccount.ColumnHeadersDefaultCellStyle.BackColor = Color.Gray;
            dgvAccount.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvAccount.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 12, FontStyle.Bold);

            // Nội dung dòng
            dgvAccount.RowTemplate.Height = 40;
            dgvAccount.DefaultCellStyle.BackColor = Color.White;
            dgvAccount.DefaultCellStyle.ForeColor = Color.Black;
            dgvAccount.DefaultCellStyle.Font = new Font("Segoe UI", 11);

            // Màu khi click chọn
            dgvAccount.DefaultCellStyle.SelectionBackColor = Color.FromArgb(231, 229, 255);
            dgvAccount.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgvAccount.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            if (dgvAccount.Columns.Count > 0)
            {
                dgvAccount.Columns["MaND"].HeaderText = "Mã";
                dgvAccount.Columns["MaND"].FillWeight = 50;
                dgvAccount.Columns["TenDangNhap"].HeaderText = "Tên Đăng Nhập";
                dgvAccount.Columns["HoTen"].HeaderText = "Họ Tên";
                dgvAccount.Columns["NgaySinh"].HeaderText = "Ngày Sinh";
                dgvAccount.Columns["NgaySinh"].DefaultCellStyle.Format = "dd/MM/yyyy";
                dgvAccount.Columns["SoDienThoai"].HeaderText = "SĐT";
                dgvAccount.Columns["VaiTro"].HeaderText = "Vai Trò";
                dgvAccount.Columns["TrangThai"].HeaderText = "Trạng Thái";
            }
        }

        // =========================================================
        // HÀM HỖ TRỢ: BỐC DỮ LIỆU TỪ LƯỚI LÊN FORM
        // =========================================================
        private void BocDuLieuLenForm(DataGridViewRow row)
        {
            txtUser.Text = row.Cells["TenDangNhap"].Value.ToString();
            txtHoten.Text = row.Cells["HoTen"].Value.ToString();
            txtSdt.Text = row.Cells["SoDienThoai"].Value.ToString();

            string vaitro = row.Cells["VaiTro"].Value.ToString();
            if (cboRole.Items.Contains(vaitro)) cboRole.SelectedItem = vaitro;

            string trangthai = row.Cells["TrangThai"].Value.ToString();
            if (cboTrangthai.Items.Contains(trangthai)) cboTrangthai.SelectedItem = trangthai;

            if (row.Cells["NgaySinh"].Value != DBNull.Value)
                dtpNgaysinh.Value = Convert.ToDateTime(row.Cells["NgaySinh"].Value);
        }

        // =========================================================
        // SỰ KIỆN CLICK VÀO LƯỚI (KHÔNG LÀM NHẢY FORM KHI CHƯA BẬT SỬA)
        // =========================================================
        private void dgvAccount_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvAccount.Rows[e.RowIndex];

                // 1. Luôn lưu lại ID của người dùng đang được chọn
                currentMaND = Convert.ToInt32(row.Cells["MaND"].Value);

                // 2. CHỈ KHI NÀO ĐANG BẬT CHẾ ĐỘ SỬA THÌ MỚI BỐC DỮ LIỆU LÊN FORM
                if (isEditing)
                {
                    BocDuLieuLenForm(row);
                }
            }
        }

        // =========================================================
        // CÁC NÚT ĐIỀU KHIỂN CHỨC NĂNG (SỬA / XÓA)
        // =========================================================

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (currentMaND == -1 || dgvAccount.CurrentRow == null)
            {
                MessageBox.Show("Chọn một dòng sau đó bấm sửa", "Thông báo");
                return;
            }

            // Đảo trạng thái công tắc
            isEditing = !isEditing;

            if (isEditing)
            {
                btnEdit.Text = "[Đang Sửa]"; // Đổi chữ để báo hiệu

                // Bốc dữ liệu của dòng ĐANG ĐƯỢC CHỌN lên form
                BocDuLieuLenForm(dgvAccount.CurrentRow);
                MessageBox.Show("Chế độ SỬA đã bật.\nNhập lại thông tin rồi nhấn 'Save' để chốt nhé!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                // Bấm lần nữa để Tắt chế độ sửa
                btnEdit.Text = "Sửa";
                ClearForm();
            }
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            if (currentMaND == -1)
            {
                MessageBox.Show("Chọn một tài khoản để xóa!", "Cảnh báo"); return;
            }

            if (MessageBox.Show("Bạn có muốn xóa tài khoản này không?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    try
                    {
                        conn.Open();
                        SqlCommand cmd = new SqlCommand("DELETE FROM NguoiDung WHERE MaND = @ma", conn);
                        cmd.Parameters.AddWithValue("@ma", currentMaND);
                        cmd.ExecuteNonQuery();

                        MessageBox.Show("Xóa thành công!", "Thông báo");
                        LoadDataTuDatabase();
                    }
                    catch (Exception ex) { MessageBox.Show("Lỗi xóa: " + ex.Message); }
                }
            }
        }

        // =========================================================
        // LƯU DỮ LIỆU (UPDATE) VÀO DATABASE
        // =========================================================
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!isEditing)
            {
                MessageBox.Show("Phải bấm nút 'Sửa' trước khi Lưu!", "Thông báo");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtUser.Text))
            {
                MessageBox.Show("Tên đăng nhập không được để trống!", "Lỗi"); return;
            }

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    conn.Open();
                    string sql = "";
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;

                    if (isEditing)
                    {
                        // ĐÃ FIX: Update thuần túy các thông tin, hoàn toàn không đụng chạm mật khẩu
                        sql = "UPDATE NguoiDung SET TenDangNhap=@u, HoTen=@t, NgaySinh=@n, SoDienThoai=@s, VaiTro=@r, TrangThai=@tt WHERE MaND=@ma";
                        cmd.Parameters.AddWithValue("@ma", currentMaND);
                    }

                    cmd.Parameters.AddWithValue("@u", txtUser.Text.Trim());
                    cmd.Parameters.AddWithValue("@t", txtHoten.Text.Trim());
                    cmd.Parameters.AddWithValue("@n", dtpNgaysinh.Value);
                    cmd.Parameters.AddWithValue("@s", txtSdt.Text.Trim());
                    cmd.Parameters.AddWithValue("@r", cboRole.Text);
                    cmd.Parameters.AddWithValue("@tt", cboTrangthai.Text);

                    cmd.CommandText = sql;
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Cập nhật thông tin thành công!", "Thông báo");
                    LoadDataTuDatabase();
                }
                catch (Exception ex) { MessageBox.Show("Lỗi lưu dữ liệu: " + ex.Message, "Lỗi SQL"); }
            }
        }

        // =========================================================
        // TÌM KIẾM & LÀM MỚI
        // =========================================================
        private void btnSearch_Click(object sender, EventArgs e)
        {
            LoadDataTuDatabase(txtSearch.Text.Trim());
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            LoadDataTuDatabase();
        }

        // =========================================================
        // HÀM DỌN DẸP FORM VỀ TRẠNG THÁI GỐC
        // =========================================================
        private void ClearForm()
        {
            txtUser.Clear(); txtHoten.Clear(); txtSdt.Clear(); txtSearch.Clear();
            currentMaND = -1;
            isEditing = false;
            btnEdit.Text = "Sửa";
        }
    }
}