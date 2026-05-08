    using System;
    using System.Data;
    using System.Data.SqlClient;
    using System.Windows.Forms;

    namespace Thinh_QLNhasach.Views
    {
        public partial class FrmTaoSachMoi : Form
        {
            // Chuỗi kết nối Database
            string connectionString = @"Data Source=THINHLALUOT\SQLEXPRESS01;Initial Catalog=BookShop;Integrated Security=True";

            // Biến Public này giống như cái "túi" để tuồn Mã Sách vừa tạo về lại trang Nhập Hàng
            public int MaSachVuaTao { get; set; } = -1;

            public FrmTaoSachMoi()
            {
                InitializeComponent();
                this.Load += FrmTaoSachMoi_Load;
            }

            // Sự kiện chạy khi Form vừa được mở lên
            private void FrmTaoSachMoi_Load(object sender, EventArgs e)
            {
                LoadComboBoxes();
            }

            // Hàm nạp dữ liệu vào 3 cái ComboBox
            private void LoadComboBoxes()
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    try
                    {
                        // 1. Load Tác Giả
                        SqlDataAdapter daTG = new SqlDataAdapter("SELECT MaTG, TenTG FROM TacGia", conn);
                        DataTable dtTG = new DataTable(); daTG.Fill(dtTG);
                        cboMaTG.DataSource = dtTG; cboMaTG.DisplayMember = "TenTG"; cboMaTG.ValueMember = "MaTG";

                        // 2. Load Thể Loại
                        SqlDataAdapter daTL = new SqlDataAdapter("SELECT MaTL, TenTL FROM TheLoai", conn);
                        DataTable dtTL = new DataTable(); daTL.Fill(dtTL);
                        cboMaTL.DataSource = dtTL; cboMaTL.DisplayMember = "TenTL"; cboMaTL.ValueMember = "MaTL";

                        // 3. Load Nhà Xuất Bản
                        SqlDataAdapter daNXB = new SqlDataAdapter("SELECT MaNXB, TenNXB FROM NhaXuatBan", conn);
                        DataTable dtNXB = new DataTable(); daNXB.Fill(dtNXB);
                        cboMaNXB.DataSource = dtNXB; cboMaNXB.DisplayMember = "TenNXB"; cboMaNXB.ValueMember = "MaNXB";
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi load dữ liệu: " + ex.Message, "Báo lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            // Sự kiện khi bấm nút Save
            private void btnSave_Click(object sender, EventArgs e)
            {
                // 1. Kiểm tra xem đã nhập tên sách chưa
                if (string.IsNullOrWhiteSpace(txtTenSach.Text))
                {
                    MessageBox.Show("Vui lòng nhập tên sách!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // 2. Kiểm tra xem đã chọn đầy đủ các thông tin phụ chưa
                if (cboMaTG.SelectedValue == null || cboMaTL.SelectedValue == null || cboMaNXB.SelectedValue == null)
                {
                    MessageBox.Show("Vui lòng chọn đầy đủ Tác giả, Thể loại và Nhà xuất bản!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // 3. Tiến hành lưu vào CSDL
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    try
                    {
                        conn.Open();

                        // Lệnh OUTPUT INSERTED.MaSach giúp bắt ngay lập tức cái Mã của cuốn sách vừa tạo
                        // Khởi tạo Tồn Kho = 0, Giá Nhập = 0, Giá Bán = 0
                        string sql = @"INSERT INTO Sach (TenSach, MaTG, MaTL, MaNXB, SoLuongTon, GiaNhap, GiaBan) 
                                       OUTPUT INSERTED.MaSach
                                       VALUES (@ten, @tg, @tl, @nxb, 0, 0, 0)";

                        SqlCommand cmd = new SqlCommand(sql, conn);
                        cmd.Parameters.AddWithValue("@ten", txtTenSach.Text.Trim());
                        cmd.Parameters.AddWithValue("@tg", cboMaTG.SelectedValue);
                        cmd.Parameters.AddWithValue("@tl", cboMaTL.SelectedValue);
                        cmd.Parameters.AddWithValue("@nxb", cboMaNXB.SelectedValue);

                        object result = cmd.ExecuteScalar();
                        if (result != null)
                        {
                            MaSachVuaTao = Convert.ToInt32(result); // Gán mã sách vào biến Public

                            MessageBox.Show("Đã tạo hồ sơ sách mới thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            this.DialogResult = DialogResult.OK; // Phát tín hiệu OK về cho Form Nhập Hàng
                            this.Close(); // Tự động đóng cái Form nổi này lại
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi khi lưu sách mới: " + ex.Message, "Báo lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }