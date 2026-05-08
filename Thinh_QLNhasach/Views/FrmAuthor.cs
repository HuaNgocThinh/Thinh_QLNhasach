using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using Thinh_QLNhasach.Utility;

namespace Thinh_QLNhasach
{
    public partial class FrmTacGia : Form
    {
        string strConn = @"Data Source=THINHLALUOT\SQLEXPRESS01; Initial Catalog=BookShop; Integrated Security=True";
        SqlConnection conn;

        // ===== BẢNG TẠM VÀ BIẾN KIỂM SOÁT =====
        DataTable dtTemp = new DataTable();
        int editingRowIndex = -1;
        bool isEditing = false; // CÔNG TẮC: Bật/Tắt chế độ sửa thủ công

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

            LoadDataFromDB();
        }

        // ===== LOAD TỪ DB VÀO BẢNG TẠM =====
        void LoadDataFromDB()
        {
            try
            {
                SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM TacGia", conn);
                dtTemp = new DataTable();
                da.Fill(dtTemp);

                if (!dtTemp.Columns.Contains("_Status"))
                    dtTemp.Columns.Add("_Status", typeof(string));

                foreach (DataRow r in dtTemp.Rows)
                    r["_Status"] = "none";

                BindDGV();
            }
            catch (Exception ex) { MessageBox.Show("Lỗi load: " + ex.Message); }
        }

        void BindDGV()
        {
            dgvTacGia.DataSource = null;
            dgvTacGia.DataSource = dtTemp;

            if (dgvTacGia.Columns.Contains("_Status"))
                dgvTacGia.Columns["_Status"].Visible = false;

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

            ColorRows();
        }

        void ColorRows()
        {
            foreach (DataGridViewRow row in dgvTacGia.Rows)
            {
                if (row.DataBoundItem == null) continue;
                DataRowView drv = (DataRowView)row.DataBoundItem;
                string status = drv["_Status"]?.ToString();

                if (status == "add")
                    row.DefaultCellStyle.BackColor = Color.FromArgb(198, 239, 206);
                else
                    row.DefaultCellStyle.BackColor = Color.White;
            }
        }

        private void BocDuLieuTacGia(int index)
        {
            if (index < 0 || index >= dtTemp.Rows.Count) return;

            DataRow row = dtTemp.Rows[index];
            txtTenTG.Text = row["TenTG"]?.ToString();
            txtQuequan.Text = row["QueQuan"]?.ToString();

            if (row["NamSinh"] != DBNull.Value && DateTime.TryParse(row["NamSinh"].ToString(), out DateTime ns))
                dtpNgaySinh.Value = ns;
            else
                dtpNgaySinh.Value = DateTime.Now;

            if (row["NamMat"] == DBNull.Value || row["NamMat"] == null || string.IsNullOrEmpty(row["NamMat"].ToString()))
            {
                chkDamat.Checked = false;
                dtpNgayMat.Enabled = false;
            }
            else
            {
                chkDamat.Checked = true;
                dtpNgayMat.Enabled = true;
                if (DateTime.TryParse(row["NamMat"].ToString(), out DateTime nm))
                    dtpNgayMat.Value = nm;
            }
        }

        // ===== CLICK DGV → BỐC DỮ LIỆU BẤT CHẤP (KHÔNG TẮT CÔNG TẮC NỮA) =====
        private void dgvTacGia_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            editingRowIndex = e.RowIndex;
            BocDuLieuTacGia(editingRowIndex);
        }

        // ===== NÚT THÊM =====
        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTenTG.Text)) { MessageBox.Show("Vui lòng nhập tên tác giả!"); return; }

            DataRow newRow = dtTemp.NewRow();
            newRow["MaTG"] = DBNull.Value;
            newRow["TenTG"] = txtTenTG.Text.Trim();
            newRow["QueQuan"] = txtQuequan.Text.Trim();
            newRow["NamSinh"] = dtpNgaySinh.Value;
            newRow["NamMat"] = chkDamat.Checked ? (object)dtpNgayMat.Value : DBNull.Value;
            newRow["_Status"] = "add";

            dtTemp.Rows.Add(newRow);
            BindDGV();
            ClearInputs();
            MessageBox.Show("Đã thêm vào bảng chờ. Nhấn [Save] để chốt vào Database!");
        }

        // ===== NÚT SỬA LÀ CÔNG TẮC DUY NHẤT (Chỉ tắt/bật khi chính tay ông nhấn vào) =====
        private void btnEdit_Click(object sender, EventArgs e)
        {
            isEditing = !isEditing;

            if (isEditing)
            {
                btnEdit.Text = "Đang Sửa ✏️";
                btnEdit.BackColor = Color.Orange;
                if (editingRowIndex >= 0) BocDuLieuTacGia(editingRowIndex);
                MessageBox.Show("Đã bật chế độ sửa!");
            }
            else
            {
                btnEdit.Text = "Sửa ";
                btnEdit.UseVisualStyleBackColor = true;
                ClearInputs();
                editingRowIndex = -1;
            }
        }

        // ===== NÚT XÓA =====
        private void btnDel_Click(object sender, EventArgs e)
        {
            if (dgvTacGia.CurrentRow == null) return;
            int idx = dgvTacGia.CurrentRow.Index;
            DataRow row = dtTemp.Rows[idx];
            string ten = row["TenTG"].ToString();

            if (MessageBox.Show($"Bạn có chắc chắn muốn XÓA vĩnh viễn tác giả '{ten}' khỏi hệ thống?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.No) return;

            if (row["_Status"].ToString() == "add")
            {
                dtTemp.Rows.RemoveAt(idx);
                BindDGV();
            }
            else
            {
                try
                {
                    if (conn.State == ConnectionState.Closed) conn.Open();
                    SqlCommand cmd = new SqlCommand("DELETE FROM TacGia WHERE MaTG=@id", conn);
                    cmd.Parameters.AddWithValue("@id", row["MaTG"]);
                    cmd.ExecuteNonQuery();

                    AppLogger.GhiLog(Session.Username, "Xóa Tác giả", $"Xóa tác giả: {ten}");

                    LoadDataFromDB();
                    MessageBox.Show("Đã xóa tác giả khỏi Database!");
                }
                catch (SqlException ex)
                {
                    if (ex.Number == 547)
                        MessageBox.Show($"Không xóa được '{ten}' vì đang có sách thuộc tác giả này trong Kho!", "Cảnh báo Khóa Ngoại", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    else MessageBox.Show("Lỗi CSDL: " + ex.Message);
                }
                catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
                finally { if (conn.State == ConnectionState.Open) conn.Close(); }
            }

            // Xóa xong chỉ clear ô nhập liệu, CÔNG TẮC SỬA VẪN BẬT NẾU TRƯỚC ĐÓ ĐANG BẬT
            ClearInputs();
            editingRowIndex = -1;
        }

        // ===== NÚT SAVE: LƯU DB MÀ KHÔNG LÀM TẮT CÔNG TẮC SỬA =====
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (isEditing && editingRowIndex >= 0 && string.IsNullOrWhiteSpace(txtTenTG.Text))
            {
                MessageBox.Show("Vui lòng nhập tên tác giả để lưu cập nhật!");
                return;
            }

            bool hasNew = false;
            foreach (DataRow r in dtTemp.Rows)
                if (r["_Status"].ToString() == "add") { hasNew = true; break; }

            if (!hasNew && !(isEditing && editingRowIndex >= 0))
            {
                MessageBox.Show("Chưa chọn Tác giả nào để Sửa và cũng không có Tác giả mới nào để Thêm!");
                return;
            }

            try
            {
                if (conn.State == ConnectionState.Closed) conn.Open();
                int addCount = 0;
                int editCount = 0;

                // 1. CHỐT SỬA VÀO DATABASE
                if (isEditing && editingRowIndex >= 0)
                {
                    DataRow row = dtTemp.Rows[editingRowIndex];

                    if (row["_Status"].ToString() == "add")
                    {
                        // Nếu lỡ đang bật Sửa cho dòng vừa ấn Thêm -> cập nhật bảng chờ
                        row.BeginEdit();
                        row["TenTG"] = txtTenTG.Text.Trim();
                        row["QueQuan"] = txtQuequan.Text.Trim();
                        row["NamSinh"] = dtpNgaySinh.Value;
                        row["NamMat"] = chkDamat.Checked ? (object)dtpNgayMat.Value : DBNull.Value;
                        row.EndEdit();
                    }
                    else
                    {
                        // UPDATE thẳng vào Database
                        SqlCommand cmdEdit = new SqlCommand(
                            "UPDATE TacGia SET TenTG=@ten, NamSinh=@ns, NamMat=@nm, QueQuan=@qq WHERE MaTG=@id", conn);
                        cmdEdit.Parameters.AddWithValue("@id", row["MaTG"]);
                        cmdEdit.Parameters.AddWithValue("@ten", txtTenTG.Text.Trim());
                        cmdEdit.Parameters.AddWithValue("@ns", dtpNgaySinh.Value);
                        cmdEdit.Parameters.AddWithValue("@nm", chkDamat.Checked ? (object)dtpNgayMat.Value : DBNull.Value);
                        cmdEdit.Parameters.AddWithValue("@qq", txtQuequan.Text.Trim());
                        cmdEdit.ExecuteNonQuery();
                        editCount++;
                    }
                }

                // 2. CHỐT THÊM MỚI VÀO DATABASE
                foreach (DataRow row in dtTemp.Rows)
                {
                    if (row["_Status"].ToString() == "add")
                    {
                        SqlCommand cmdAdd = new SqlCommand(
                            "INSERT INTO TacGia (TenTG, NamSinh, NamMat, QueQuan) VALUES (@ten, @ns, @nm, @qq)", conn);
                        cmdAdd.Parameters.AddWithValue("@ten", row["TenTG"]);
                        cmdAdd.Parameters.AddWithValue("@ns", row["NamSinh"]);
                        cmdAdd.Parameters.AddWithValue("@nm", row["NamMat"] == DBNull.Value ? (object)DBNull.Value : row["NamMat"]);
                        cmdAdd.Parameters.AddWithValue("@qq", row["QueQuan"]);
                        cmdAdd.ExecuteNonQuery();
                        addCount++;
                    }
                }

                MessageBox.Show($"Đã Lưu Thành Công!\n- Thêm mới: {addCount} tác giả.\n- Cập nhật: {editCount} tác giả.");
                AppLogger.GhiLog(Session.Username, "Cập nhật Tác giả", $"Thêm: {addCount}, Sửa: {editCount}");

                // LƯU XONG KHÔNG TẮT CÔNG TẮC ĐỂ ÔNG LÀM TIẾP
                ClearInputs();
                editingRowIndex = -1;
                LoadDataFromDB();
            }
            catch (Exception ex) { MessageBox.Show("Lỗi khi lưu Database: " + ex.Message); }
            finally { if (conn.State == ConnectionState.Open) conn.Close(); }
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

        // ===== NÚT RESET SẼ TẮT LUÔN CÔNG TẮC =====
        private void btnReset_Click(object sender, EventArgs e)
        {
            ClearInputs();
            editingRowIndex = -1;

            isEditing = false;
            btnEdit.Text = "Sửa ";
            btnEdit.UseVisualStyleBackColor = true;

            LoadDataFromDB();
        }

        void ClearInputs()
        {
            txtTenTG.Clear();
            txtQuequan.Clear();
            txtSearch.Clear();
            dtpNgayMat.Checked = false;
            dtpNgayMat.Value = DateTime.Now;
            dtpNgaySinh.Value = DateTime.Now;
            chkDamat.Checked = false;
        }
    }
}