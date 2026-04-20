using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace Thinh_QLNhasach
{
    public partial class FrmTacGia : Form
    {
        string strConn = @"Data Source=THINHLALUOT\SQLEXPRESS01; Initial Catalog=BookShop; Integrated Security=True";
        SqlConnection conn;

        // ===== BẢNG TẠM =====
        DataTable dtTemp = new DataTable(); // bảng tạm hiển thị trên DGV
        int editingRowIndex = -1;           // đang sửa dòng nào (-1 = không sửa)

        public FrmTacGia()
        {
            InitializeComponent();
            conn = new SqlConnection(strConn);
            LoadDataFromDB();
        }

        private void FrmTacGia_Load(object sender, EventArgs e)
        {
            dtpNgaySinh.Format = DateTimePickerFormat.Custom;
            dtpNgaySinh.CustomFormat = "dd/MM/yyyy";
            dtpNgayMat.Format = DateTimePickerFormat.Custom;
            dtpNgayMat.CustomFormat = "dd/MM/yyyy";
            dtpNgayMat.ShowCheckBox = true;

            dgvTacGia.AllowUserToAddRows = false;
            dgvTacGia.ReadOnly = true;
            dgvTacGia.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            LoadDataFromDB(); // load từ DB vào dtTemp lúc mở form
        }

        // ===== LOAD TỪ DB VÀO BẢNG TẠM =====
        void LoadDataFromDB()
        {
            try
            {
                SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM TacGia", conn);
                dtTemp = new DataTable();
                da.Fill(dtTemp);

                // Đánh dấu thêm cột trạng thái (không hiện ra DGV)
                if (!dtTemp.Columns.Contains("_Status"))
                    dtTemp.Columns.Add("_Status", typeof(string)); // "none" / "add" / "edit" / "delete"

                foreach (DataRow r in dtTemp.Rows)
                    r["_Status"] = "none";

                BindDGV();
            }
            catch (Exception ex) { MessageBox.Show("Lỗi load: " + ex.Message); }
        }

        // ===== GẮN BẢNG TẠM VÀO DGV (ẨN CỘT _Status) =====
        void BindDGV()
        {
            dgvTacGia.DataSource = null;
            dgvTacGia.DataSource = dtTemp;

            // Ẩn cột trạng thái
            if (dgvTacGia.Columns.Contains("_Status"))
                dgvTacGia.Columns["_Status"].Visible = false;

            // Styling
            dgvTacGia.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvTacGia.RowHeadersVisible = false;
            dgvTacGia.EnableHeadersVisualStyles = false;
            dgvTacGia.ColumnHeadersHeight = 45;
            dgvTacGia.ColumnHeadersDefaultCellStyle.BackColor = Color.Gray;
            dgvTacGia.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvTacGia.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            dgvTacGia.DefaultCellStyle.BackColor = Color.White;
            dgvTacGia.DefaultCellStyle.ForeColor = Color.Black;
            dgvTacGia.DefaultCellStyle.Font = new Font("Segoe UI", 11);
            dgvTacGia.DefaultCellStyle.SelectionBackColor = Color.FromArgb(231, 229, 255);
            dgvTacGia.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgvTacGia.RowTemplate.Height = 40;
            dgvTacGia.BackgroundColor = Color.White;
            dgvTacGia.GridColor = Color.FromArgb(231, 229, 255);

            if (dgvTacGia.Columns.Contains("MaTG"))
            {
                dgvTacGia.Columns["MaTG"].HeaderText = "Mã TG";
                dgvTacGia.Columns["TenTG"].HeaderText = "Tên Tác Giả";
                dgvTacGia.Columns["QueQuan"].HeaderText = "Quê Quán";
                dgvTacGia.Columns["NamSinh"].HeaderText = "Năm Sinh";
                dgvTacGia.Columns["NamSinh"].DefaultCellStyle.Format = "dd/MM/yyyy";
                dgvTacGia.Columns["NamMat"].HeaderText = "Năm Mất";
                dgvTacGia.Columns["NamMat"].DefaultCellStyle.Format = "dd/MM/yyyy";
            }

            // Tô màu dòng theo trạng thái
            ColorRows();
        }

        // ===== TÔ MÀU DÒNG THEO TRẠNG THÁI =====
        void ColorRows()
        {
            foreach (DataGridViewRow row in dgvTacGia.Rows)
            {
                if (row.DataBoundItem == null) continue;
                DataRowView drv = (DataRowView)row.DataBoundItem;
                string status = drv["_Status"]?.ToString();

                switch (status)
                {
                    case "add": row.DefaultCellStyle.BackColor = Color.FromArgb(198, 239, 206); break; // xanh lá nhạt
                    case "edit": row.DefaultCellStyle.BackColor = Color.FromArgb(255, 235, 156); break; // vàng nhạt
                    case "delete": row.DefaultCellStyle.BackColor = Color.FromArgb(255, 199, 206); break; // đỏ nhạt
                    default: row.DefaultCellStyle.BackColor = Color.White; break;
                }
            }
        }

        // ===== NÚT THÊM → THÊM VÀO BẢNG TẠM =====
        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTenTG.Text)) { MessageBox.Show("Vui lòng nhập tên tác giả!"); return; }

            DataRow newRow = dtTemp.NewRow();
            newRow["MaTG"] = DBNull.Value; // chưa có ID thật
            newRow["TenTG"] = txtTenTG.Text.Trim();
            newRow["QueQuan"] = txtQuequan.Text.Trim();
            newRow["NamSinh"] = dtpNgaySinh.Value;
            newRow["NamMat"] = chkDamat.Checked ? (object)dtpNgayMat.Value : DBNull.Value;
            newRow["_Status"] = "add";

            dtTemp.Rows.Add(newRow);
            BindDGV();
            ClearInputs();
            MessageBox.Show("Đã thêm vào bảng tạm. Nhấn Save để lưu vào database!");
        }

        // ===== NÚT SỬA → CẬP NHẬT BẢNG TẠM =====
        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (editingRowIndex < 0) { MessageBox.Show("Vui lòng chọn dòng cần sửa!"); return; }

            DataRow row = dtTemp.Rows[editingRowIndex];

            // Lần 1: đổ dữ liệu lên GroupBox
            if (row["_Status"].ToString() != "edit_pending")
            {
                txtTenTG.Text = row["TenTG"]?.ToString();
                txtQuequan.Text = row["QueQuan"]?.ToString();
                dtpNgaySinh.Value = row["NamSinh"] != DBNull.Value ? Convert.ToDateTime(row["NamSinh"]) : DateTime.Now;

                if (row["NamMat"] == DBNull.Value || row["NamMat"] == null)
                {
                    chkDamat.Checked = false;
                    dtpNgayMat.Enabled = false;
                }
                else
                {
                    chkDamat.Checked = true;
                    dtpNgayMat.Enabled = true;
                    dtpNgayMat.Value = Convert.ToDateTime(row["NamMat"]);
                }

                row["_Status"] = "edit_pending";
                BindDGV();
                return; // chờ người dùng chỉnh
            }

            // Lần 2: chỉ cập nhật vào bảng tạm, KHÔNG save DB
            if (string.IsNullOrWhiteSpace(txtTenTG.Text)) { MessageBox.Show("Vui lòng nhập tên tác giả!"); return; }

            row.BeginEdit();
            row["TenTG"] = txtTenTG.Text.Trim();
            row["QueQuan"] = txtQuequan.Text.Trim();
            row["NamSinh"] = dtpNgaySinh.Value;
            row["NamMat"] = chkDamat.Checked ? (object)dtpNgayMat.Value : DBNull.Value;
            row["_Status"] = "edit"; // đánh dấu chờ Save
            row.EndEdit();

            BindDGV();
            ClearInputs();
            editingRowIndex = -1;
            MessageBox.Show("Đã cập nhật bảng tạm. Nhấn Save để lưu vào database!"); // nhắc nhở
        }

        // ===== NÚT XÓA → ĐÁNH DẤU DELETE TRONG BẢNG TẠM =====
        private void btnDel_Click(object sender, EventArgs e)
        {
            if (dgvTacGia.CurrentRow == null) return;
            int idx = dgvTacGia.CurrentRow.Index;
            DataRow row = dtTemp.Rows[idx];
            string ten = row["TenTG"].ToString();

            if (MessageBox.Show($"Đánh dấu xóa '{ten}'?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.No) return;

            if (row["_Status"].ToString() == "add")
            {
                // Dòng mới chưa lưu DB → xóa thẳng khỏi bảng tạm
                dtTemp.Rows.RemoveAt(idx);
            }
            else
            {
                // Dòng đã có trong DB → đánh dấu để Save xử lý
                row["_Status"] = "delete";
            }

            BindDGV();
        }

        // ===== NÚT SAVE → LƯU TẤT CẢ VÀO DATABASE =====
        private void btnSave_Click(object sender, EventArgs e)
        {
            bool hasPending = false;
            foreach (DataRow r in dtTemp.Rows)
                if (r["_Status"].ToString() != "none" && r["_Status"].ToString() != "edit_pending")
                { hasPending = true; break; }

            if (!hasPending) { MessageBox.Show("Không có thay đổi nào để lưu!"); return; }

            try
            {
                if (conn.State == ConnectionState.Open) conn.Close();
                conn.Open();

                int addCount = 0, editCount = 0, delCount = 0;

                foreach (DataRow row in dtTemp.Rows)
                {
                    string status = row["_Status"].ToString();

                    if (status == "add")
                    {
                        SqlCommand cmd = new SqlCommand(
                            "INSERT INTO TacGia (TenTG, NamSinh, NamMat, QueQuan) VALUES (@ten, @ns, @nm, @qq)", conn);
                        cmd.Parameters.AddWithValue("@ten", row["TenTG"]);
                        cmd.Parameters.AddWithValue("@ns", row["NamSinh"]);
                        cmd.Parameters.AddWithValue("@nm", row["NamMat"] == DBNull.Value ? (object)DBNull.Value : row["NamMat"]);
                        cmd.Parameters.AddWithValue("@qq", row["QueQuan"]);
                        cmd.ExecuteNonQuery();
                        addCount++;
                    }
                    else if (status == "edit")
                    {
                        SqlCommand cmd = new SqlCommand(
                            "UPDATE TacGia SET TenTG=@ten, NamSinh=@ns, NamMat=@nm, QueQuan=@qq WHERE MaTG=@id", conn);
                        cmd.Parameters.AddWithValue("@id", row["MaTG"]);
                        cmd.Parameters.AddWithValue("@ten", row["TenTG"]);
                        cmd.Parameters.AddWithValue("@ns", row["NamSinh"]);
                        cmd.Parameters.AddWithValue("@nm", row["NamMat"] == DBNull.Value ? (object)DBNull.Value : row["NamMat"]);
                        cmd.Parameters.AddWithValue("@qq", row["QueQuan"]);
                        cmd.ExecuteNonQuery();
                        editCount++;
                    }
                    else if (status == "delete")
                    {
                        SqlCommand cmd = new SqlCommand("DELETE FROM TacGia WHERE MaTG=@id", conn);
                        cmd.Parameters.AddWithValue("@id", row["MaTG"]);
                        try { cmd.ExecuteNonQuery(); delCount++; }
                        catch (SqlException ex)
                        {
                            if (ex.Number == 547)
                                MessageBox.Show($"Không xóa được '{row["TenTG"]}' vì còn sách liên kết!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            else throw;
                        }
                    }
                }

                MessageBox.Show($"Lưu thành công!\nThêm: {addCount} | Sửa: {editCount} | Xóa: {delCount}");
                LoadDataFromDB();
                ClearInputs();
                editingRowIndex = -1;
            }
            catch (Exception ex) { MessageBox.Show("Lỗi lưu: " + ex.Message); }
            finally { if (conn.State == ConnectionState.Open) conn.Close(); }
        }

        // ===== CLICK DGV → ĐỔ DỮ LIỆU LÊN FORM =====
        private void dgvTacGia_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            editingRowIndex = e.RowIndex; // chỉ ghi nhớ dòng đang chọn
        }

        // ===== TÌM KIẾM =====
        private void btnSearch_Click(object sender, EventArgs e)
        {
            string keyword = txtSearch.Text.Trim().ToLower();
            DataView dv = dtTemp.DefaultView;
            dv.RowFilter = $"TenTG LIKE '%{keyword}%' OR QueQuan LIKE '%{keyword}%'";
            dgvTacGia.DataSource = dv;
            ColorRows();
        }

        // ===== RESET =====
        private void btnReset_Click(object sender, EventArgs e)
        {
            ClearInputs();
            editingRowIndex = -1;
            LoadDataFromDB();
        }

        void ClearInputs()
        {
            txtTenTG.Clear(); txtQuequan.Clear(); txtSearch.Clear();
            dtpNgayMat.Checked = false;
            dtpNgayMat.Value = DateTime.Now;
            dtpNgaySinh.Value = DateTime.Now;
            chkDamat.Checked = false;
        }
    }
}