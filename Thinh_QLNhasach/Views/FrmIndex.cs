using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using FontAwesome.Sharp;
using Thinh_QLNhasach.Utility; // Quan trọng: Để lấy class Session

namespace Thinh_QLNhasach.Views
{
    public partial class FrmIndex : Form
    {
        // Biến Role để nhận từ Form Login (nếu có truyền trực tiếp)
        public string Role { get; set; }

        public FrmIndex()
        {
            InitializeComponent();
            // Đã xóa sự kiện Resize vì dùng DockStyle.Fill nó tự động giãn nở rồi, không cần tính toán tay nữa!
        }

        /// <summary>
        /// Sự kiện chạy khi Form Chính nạp lên
        /// </summary>
        private void FrmIndex_Load(object sender, EventArgs e)
        {
            ApplyMenuTheme();
            // 1. HIỂN THỊ THÔNG TIN NGƯỜI DÙNG
            // Lấy dữ liệu từ Session đã lưu lúc Đăng nhập
            lblNguoiDung.Text = Session.Username;
            lblRole.Text = Session.Role;

            // 2. PHÂN QUYỀN NGƯỜI DÙNG
            if (string.IsNullOrEmpty(Role))
            {
                Role = Session.Role;
            }
            PhanQuyen();

            // =========================================================
            // 3. ĐÃ FIX: TỰ ĐỘNG BẤM NÚT "TRANG CHỦ" ĐỂ HIỆN TỔNG QUAN LUN
            // =========================================================
            btnTrangchu.PerformClick();
        }

        private void ApplyMenuTheme()
        {
            panelMenu.BackColor = Color.FromArgb(26, 95, 122);
            panelTop.BackColor = Color.FromArgb(24, 83, 108);

            IconButton[] menuButtons =
            {
                btnTrangchu, btnBook, btnTacgia, btnUser, btnBan, btnNhap, btnChart, btnNhatKy, btnTaiKhoan, btnLogout
            };

            for (int i = 0; i < menuButtons.Length; i++)
            {
                IconButton button = menuButtons[i];
                Color baseColor = GetMenuColor(i);

                button.BackColor = baseColor;
                button.ForeColor = Color.White;
                button.IconColor = Color.White;
                button.FlatStyle = FlatStyle.Flat;
                button.FlatAppearance.BorderSize = 0;
                button.FlatAppearance.MouseOverBackColor = Color.FromArgb(30, 116, 150);
                button.FlatAppearance.MouseDownBackColor = Color.FromArgb(0, 168, 150);
                button.TextAlign = ContentAlignment.MiddleLeft;
                button.TextImageRelation = TextImageRelation.ImageBeforeText;
                button.Padding = new Padding(16, 0, 12, 0);
            }
        }

        private Color GetMenuColor(int index)
        {
            int baseR = 26;
            int baseG = 95;
            int baseB = 122;
            int step = 8;
            int offset = Math.Min(index * step, 48);
            return Color.FromArgb(baseR + offset, baseG + offset, baseB + offset);
        }

        // ================= PHÂN QUYỀN LẢ LƯỚT =================
        void PhanQuyen()
        {
            // Nếu là admin (không phân biệt hoa thường) thì mới hiện các nút nhạy cảm
            if (Role != null && Role.ToLower() == "admin")
            {
                btnUser.Enabled = true;
                btnUser.Visible = true;

                // THÊM: Hiện nút Thống kê và Nhật ký cho Admin
                btnChart.Enabled = true;
                btnChart.Visible = true;

                btnNhatKy.Enabled = true;
                btnNhatKy.Visible = true;
            }
            else
            {
                // Nếu là Staff/Nhân viên thì ẩn các nút quản lý cấp cao đi cho bảo mật
                btnUser.Enabled = false;
                btnUser.Visible = false;

                // THÊM: Ẩn luôn 2 nút này nếu không phải Admin
                btnChart.Enabled = false;
                btnChart.Visible = false;

                btnNhatKy.Enabled = false;
                btnNhatKy.Visible = false;
            }
        }

        // ================= LOGIC MỞ FORM CON TRONG PANEL =================
        private void OpenFormInPanel(Form frm)
        {
            // Đóng Form cũ đang mở trong Panel để giải phóng RAM
            if (panelMain.Controls.Count > 0)
            {
                Form oldForm = panelMain.Controls[0] as Form;
                if (oldForm != null) oldForm.Close();
                panelMain.Controls.Clear();
            }

            // Thiết lập Form con
            frm.TopLevel = false;
            frm.FormBorderStyle = FormBorderStyle.None;

            // ======================================================
            // CHÌA KHÓA LÀ ĐÂY: Ép form con phóng to lấp đầy Panel
            frm.Dock = DockStyle.Fill;
            // ======================================================

            panelMain.Controls.Add(frm);
            frm.BringToFront();
            frm.Show();
        }

        // Đã xóa hàm CenterForm() và panelMain_Resize() vì dư thừa!

        // ================= CÁC SỰ KIỆN CLICK NÚT SIDEBAR =================

        // Quản lý Nhân viên (Chỉ Admin thấy)
        private void btnUser_Click(object sender, EventArgs e)
        {
            lblTitle.Text = "QUẢN LÝ NHÂN VIÊN";
            FrmAccount frm = new FrmAccount();
            OpenFormInPanel(frm);
        }

        // Quản lý Tác giả
        private void btnAuthor_Click(object sender, EventArgs e)
        {
            lblTitle.Text = "QUẢN LÝ TÁC GIẢ";
            FrmTacGia frm = new FrmTacGia();
            OpenFormInPanel(frm);
        }

        // Quản lý Sách
        private void btnBook_Click(object sender, EventArgs e)
        {
            lblTitle.Text = "QUẢN LÝ SÁCH";
            FrmBook frm = new FrmBook();
            OpenFormInPanel(frm);
        }

        // Quản lý Hóa đơn (Bán hàng)
        private void btnHoaDon_Click(object sender, EventArgs e)
        {
            lblTitle.Text = "QUẢN LÝ HÓA ĐƠN BÁN HÀNG";
            FrmHoaDon frm = new FrmHoaDon();
            OpenFormInPanel(frm);
        }

        // Quản lý Nhập hàng
        private void btnNhapHang_Click(object sender, EventArgs e)
        {
            lblTitle.Text = "QUẢN LÝ NHẬP HÀNG";
            FrmNhapHang frm = new FrmNhapHang();
            OpenFormInPanel(frm);
        }

        // Thống kê (Chỉ Admin thấy)
        private void btnThongKe_Click(object sender, EventArgs e)
        {
            lblTitle.Text = "BÁO CÁO THỐNG KÊ";
            FrmThongKe frm = new FrmThongKe();
            OpenFormInPanel(frm);
        }

        // Nhật ký hoạt động (Chỉ Admin thấy)
        private void btnNhatKy_Click(object sender, EventArgs e)
        {
            lblTitle.Text = "NHẬT KÝ HOẠT ĐỘNG HỆ THỐNG";
            FrmNhatKy frm = new FrmNhatKy();
            OpenFormInPanel(frm);
        }

        // Nút Đăng xuất
        private void btnLogout_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("Bạn muốn đăng xuất khỏi hệ thống?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dr == DialogResult.Yes)
            {
                // Đóng form hiện tại và mở lại form đăng nhập (giả sử tên là FrmLogin)
                this.Hide();
                FrmLogin login = new FrmLogin();
                login.Show();
            }
        }

        private void btnTaiKhoan_Click(object sender, EventArgs e)
        {
            // ĐÃ FIX: Không fix cứng chữ "admin" nữa, lấy chuẩn tên từ Session ra
            string userHienTai = Session.Username;
            FrmTaiKhoan frm = new FrmTaiKhoan(userHienTai);
            frm.ShowDialog();
        }

        private void btnTrangchu_Click(object sender, EventArgs e)
        {
            lblTitle.Text = "TỔNG QUAN HỆ THỐNG";
            FrmDashboard frm = new FrmDashboard();
            OpenFormInPanel(frm);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            lblThoiGian.Text = DateTime.Now.ToString("HH:mm:ss | dd/MM/yyyy");
        }
    }
}