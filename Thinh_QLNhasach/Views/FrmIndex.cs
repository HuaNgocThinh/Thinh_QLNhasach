using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
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
            // Đăng ký sự kiện Resize để form con luôn nằm giữa khi ông phóng to/thu nhỏ màn hình
            panelMain.Resize += panelMain_Resize;
        }

        /// <summary>
        /// Sự kiện chạy khi Form Chính nạp lên
        /// </summary>
        private void FrmIndex_Load(object sender, EventArgs e)
        {
            // 1. HIỂN THỊ THÔNG TIN NGƯỜI DÙNG (Phần ông vừa làm giao diện)
            // Lấy dữ liệu từ Session đã lưu lúc Đăng nhập
            lblNguoiDung.Text = Session.Username;
            lblRole.Text = Session.Role;
            // 3. PHÂN QUYỀN NGƯỜI DÙNG
            if (string.IsNullOrEmpty(Role))
            {
                Role = Session.Role;
            }
            PhanQuyen();
        }

        // ================= PHÂN QUYỀN LẢ LƯỚT =================
        void PhanQuyen()
        {
            // Nếu là admin (không phân biệt hoa thường) thì mới hiện nút Nhân viên
            if (Role != null && Role.ToLower() == "admin")
            {
                btnUser.Enabled = true;
                btnUser.Visible = true;
            }
            else
            {
                // Nếu là Staff/Nhân viên thì ẩn nút quản lý Nhân viên đi cho bảo mật
                btnUser.Enabled = false;
                btnUser.Visible = false;
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
            frm.Dock = DockStyle.None; // Để None mới dùng hàm CenterForm được

            panelMain.Controls.Add(frm);
            frm.Show();

            // Gọi hàm căn giữa Form con trong lòng Panel chính
            CenterForm(frm);
            frm.BringToFront();
        }

        private void CenterForm(Form frm)
        {
            if (frm == null || panelMain == null) return;

            // Tính toán tọa độ X, Y để đặt Form vào chính giữa Panel
            int x = (panelMain.Width - frm.Width) / 2;
            int y = (panelMain.Height - frm.Height) / 2;

            frm.Location = new System.Drawing.Point(Math.Max(x, 0), Math.Max(y, 0));
        }

        private void panelMain_Resize(object sender, EventArgs e)
        {
            // Khi kích thước FrmIndex thay đổi, ép Form con tính lại vị trí chính giữa
            if (panelMain.Controls.Count > 0 && panelMain.Controls[0] is Form)
            {
                CenterForm(panelMain.Controls[0] as Form);
            }
        }

        // ================= CÁC SỰ KIỆN CLICK NÚT SIDEBAR =================

        // Quản lý Nhân viên (Chỉ Admin thấy)
        private void btnUser_Click(object sender, EventArgs e)
        {
            FrmAccount frm = new FrmAccount();
            OpenFormInPanel(frm);
        }

        // Quản lý Tác giả
        private void btnAuthor_Click(object sender, EventArgs e)
        {
            FrmTacGia frm = new FrmTacGia(); // Nhớ check đúng tên class FrmAuthor hay FrmTacGia
            OpenFormInPanel(frm);
        }

        // Quản lý Sách
        private void btnBook_Click(object sender, EventArgs e)
        {
            FrmBook frm = new FrmBook();
            OpenFormInPanel(frm);
        }

        // Quản lý Hóa đơn (Bán hàng)
        private void btnHoaDon_Click(object sender, EventArgs e)
        {
            FrmHoaDon frm = new FrmHoaDon();
            OpenFormInPanel(frm);
        }

        // Quản lý Nhập hàng
        private void btnNhapHang_Click(object sender, EventArgs e)
        {
            FrmNhapHang frm = new FrmNhapHang();
            OpenFormInPanel(frm);
        }

        // Nút Đăng xuất
        private void btnLogout_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("Ông muốn đăng xuất và quay lại màn hình Login hả Thịnh?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dr == DialogResult.Yes)
            {
                // Đóng form hiện tại và mở lại form đăng nhập (giả sử tên là FrmLogin)
                this.Hide();
                FrmLogin login = new FrmLogin();
                login.Show();
            }
        }

        private void btnThongKe_Click(object sender, EventArgs e)
        {
            FrmThongKe frm = new FrmThongKe();
            OpenFormInPanel(frm);

        }

        private void btnNhatKy_Click(object sender, EventArgs e)
        {
            FrmNhatKy frm = new FrmNhatKy();
            OpenFormInPanel(frm);
        }
    }
}
