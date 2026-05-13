namespace Thinh_QLNhasach.Views
{
    partial class FrmIndex
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.panelMenu = new System.Windows.Forms.Panel();
            this.btnTaiKhoan = new FontAwesome.Sharp.IconButton();
            this.btnNhatKy = new FontAwesome.Sharp.IconButton();
            this.btnLogout = new FontAwesome.Sharp.IconButton();
            this.btnChart = new FontAwesome.Sharp.IconButton();
            this.btnNhap = new FontAwesome.Sharp.IconButton();
            this.btnBan = new FontAwesome.Sharp.IconButton();
            this.btnUser = new FontAwesome.Sharp.IconButton();
            this.btnTacgia = new FontAwesome.Sharp.IconButton();
            this.btnBook = new FontAwesome.Sharp.IconButton();
            this.btnTrangchu = new FontAwesome.Sharp.IconButton();
            this.panelTop = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblRole = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lblNguoiDung = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lblUser = new System.Windows.Forms.Label();
            this.panelMain = new System.Windows.Forms.Panel();
            this.lblThoiGian = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.panelMenu.SuspendLayout();
            this.panelTop.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelMenu
            // 
            this.panelMenu.AutoScroll = true;
            this.panelMenu.BackColor = System.Drawing.Color.DarkGray;
            this.panelMenu.Controls.Add(this.btnTaiKhoan);
            this.panelMenu.Controls.Add(this.btnNhatKy);
            this.panelMenu.Controls.Add(this.btnLogout);
            this.panelMenu.Controls.Add(this.btnChart);
            this.panelMenu.Controls.Add(this.btnNhap);
            this.panelMenu.Controls.Add(this.btnBan);
            this.panelMenu.Controls.Add(this.btnUser);
            this.panelMenu.Controls.Add(this.btnTacgia);
            this.panelMenu.Controls.Add(this.btnBook);
            this.panelMenu.Controls.Add(this.btnTrangchu);
            this.panelMenu.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelMenu.ForeColor = System.Drawing.Color.White;
            this.panelMenu.Location = new System.Drawing.Point(0, 145);
            this.panelMenu.Name = "panelMenu";
            this.panelMenu.Size = new System.Drawing.Size(386, 1031);
            this.panelMenu.TabIndex = 3;
            // 
            // btnTaiKhoan
            // 
            this.btnTaiKhoan.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btnTaiKhoan.FlatAppearance.BorderSize = 0;
            this.btnTaiKhoan.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTaiKhoan.Font = new System.Drawing.Font("Segoe UI", 10.875F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTaiKhoan.ForeColor = System.Drawing.Color.White;
            this.btnTaiKhoan.IconChar = FontAwesome.Sharp.IconChar.Key;
            this.btnTaiKhoan.IconColor = System.Drawing.Color.White;
            this.btnTaiKhoan.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnTaiKhoan.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnTaiKhoan.Location = new System.Drawing.Point(0, 839);
            this.btnTaiKhoan.Name = "btnTaiKhoan";
            this.btnTaiKhoan.Padding = new System.Windows.Forms.Padding(0, 0, 20, 0);
            this.btnTaiKhoan.Size = new System.Drawing.Size(386, 96);
            this.btnTaiKhoan.TabIndex = 18;
            this.btnTaiKhoan.Text = "Đổi mật khẩu";
            this.btnTaiKhoan.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnTaiKhoan.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnTaiKhoan.UseVisualStyleBackColor = true;
            this.btnTaiKhoan.Click += new System.EventHandler(this.btnTaiKhoan_Click);
            // 
            // btnNhatKy
            // 
            this.btnNhatKy.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnNhatKy.FlatAppearance.BorderSize = 0;
            this.btnNhatKy.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNhatKy.Font = new System.Drawing.Font("Segoe UI", 10.875F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNhatKy.ForeColor = System.Drawing.Color.White;
            this.btnNhatKy.IconChar = FontAwesome.Sharp.IconChar.History;
            this.btnNhatKy.IconColor = System.Drawing.Color.White;
            this.btnNhatKy.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnNhatKy.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnNhatKy.Location = new System.Drawing.Point(0, 672);
            this.btnNhatKy.Name = "btnNhatKy";
            this.btnNhatKy.Padding = new System.Windows.Forms.Padding(0, 0, 20, 0);
            this.btnNhatKy.Size = new System.Drawing.Size(386, 96);
            this.btnNhatKy.TabIndex = 17;
            this.btnNhatKy.Text = "Nhật ký hoạt động";
            this.btnNhatKy.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnNhatKy.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnNhatKy.UseVisualStyleBackColor = true;
            this.btnNhatKy.Click += new System.EventHandler(this.btnNhatKy_Click);
            // 
            // btnLogout
            // 
            this.btnLogout.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btnLogout.FlatAppearance.BorderSize = 0;
            this.btnLogout.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLogout.Font = new System.Drawing.Font("Segoe UI", 10.875F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLogout.ForeColor = System.Drawing.Color.White;
            this.btnLogout.IconChar = FontAwesome.Sharp.IconChar.PowerOff;
            this.btnLogout.IconColor = System.Drawing.Color.White;
            this.btnLogout.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnLogout.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnLogout.Location = new System.Drawing.Point(0, 935);
            this.btnLogout.Name = "btnLogout";
            this.btnLogout.Padding = new System.Windows.Forms.Padding(0, 0, 20, 0);
            this.btnLogout.Size = new System.Drawing.Size(386, 96);
            this.btnLogout.TabIndex = 15;
            this.btnLogout.Text = "Đăng xuất";
            this.btnLogout.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnLogout.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnLogout.UseVisualStyleBackColor = true;
            this.btnLogout.Click += new System.EventHandler(this.btnLogout_Click);
            // 
            // btnChart
            // 
            this.btnChart.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnChart.FlatAppearance.BorderSize = 0;
            this.btnChart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnChart.Font = new System.Drawing.Font("Segoe UI", 10.875F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnChart.ForeColor = System.Drawing.Color.White;
            this.btnChart.IconChar = FontAwesome.Sharp.IconChar.ChartColumn;
            this.btnChart.IconColor = System.Drawing.Color.White;
            this.btnChart.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnChart.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnChart.Location = new System.Drawing.Point(0, 576);
            this.btnChart.Name = "btnChart";
            this.btnChart.Padding = new System.Windows.Forms.Padding(0, 0, 20, 0);
            this.btnChart.Size = new System.Drawing.Size(386, 96);
            this.btnChart.TabIndex = 14;
            this.btnChart.Text = "Thống kê";
            this.btnChart.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnChart.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnChart.UseVisualStyleBackColor = true;
            this.btnChart.Click += new System.EventHandler(this.btnThongKe_Click);
            // 
            // btnNhap
            // 
            this.btnNhap.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnNhap.FlatAppearance.BorderSize = 0;
            this.btnNhap.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNhap.Font = new System.Drawing.Font("Segoe UI", 10.875F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNhap.ForeColor = System.Drawing.Color.White;
            this.btnNhap.IconChar = FontAwesome.Sharp.IconChar.Dolly;
            this.btnNhap.IconColor = System.Drawing.Color.White;
            this.btnNhap.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnNhap.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnNhap.Location = new System.Drawing.Point(0, 480);
            this.btnNhap.Name = "btnNhap";
            this.btnNhap.Padding = new System.Windows.Forms.Padding(0, 0, 20, 0);
            this.btnNhap.Size = new System.Drawing.Size(386, 96);
            this.btnNhap.TabIndex = 11;
            this.btnNhap.Text = "Nhập hàng";
            this.btnNhap.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnNhap.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnNhap.UseVisualStyleBackColor = true;
            this.btnNhap.Click += new System.EventHandler(this.btnNhapHang_Click);
            // 
            // btnBan
            // 
            this.btnBan.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnBan.FlatAppearance.BorderSize = 0;
            this.btnBan.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBan.Font = new System.Drawing.Font("Segoe UI", 10.875F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBan.ForeColor = System.Drawing.Color.White;
            this.btnBan.IconChar = FontAwesome.Sharp.IconChar.MoneyBill1Wave;
            this.btnBan.IconColor = System.Drawing.Color.White;
            this.btnBan.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnBan.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnBan.Location = new System.Drawing.Point(0, 384);
            this.btnBan.Name = "btnBan";
            this.btnBan.Padding = new System.Windows.Forms.Padding(0, 0, 20, 0);
            this.btnBan.Size = new System.Drawing.Size(386, 96);
            this.btnBan.TabIndex = 10;
            this.btnBan.Text = "Hóa đơn";
            this.btnBan.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnBan.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnBan.UseVisualStyleBackColor = true;
            this.btnBan.Click += new System.EventHandler(this.btnHoaDon_Click);
            // 
            // btnUser
            // 
            this.btnUser.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnUser.FlatAppearance.BorderSize = 0;
            this.btnUser.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUser.Font = new System.Drawing.Font("Segoe UI", 10.875F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnUser.ForeColor = System.Drawing.Color.White;
            this.btnUser.IconChar = FontAwesome.Sharp.IconChar.Users;
            this.btnUser.IconColor = System.Drawing.Color.White;
            this.btnUser.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnUser.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnUser.Location = new System.Drawing.Point(0, 288);
            this.btnUser.Name = "btnUser";
            this.btnUser.Padding = new System.Windows.Forms.Padding(0, 0, 20, 0);
            this.btnUser.Size = new System.Drawing.Size(386, 96);
            this.btnUser.TabIndex = 13;
            this.btnUser.Text = "Nhân viên";
            this.btnUser.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnUser.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnUser.UseVisualStyleBackColor = true;
            this.btnUser.Click += new System.EventHandler(this.btnUser_Click);
            // 
            // btnTacgia
            // 
            this.btnTacgia.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnTacgia.FlatAppearance.BorderSize = 0;
            this.btnTacgia.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTacgia.Font = new System.Drawing.Font("Segoe UI", 10.875F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTacgia.ForeColor = System.Drawing.Color.White;
            this.btnTacgia.IconChar = FontAwesome.Sharp.IconChar.UserEdit;
            this.btnTacgia.IconColor = System.Drawing.Color.White;
            this.btnTacgia.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnTacgia.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnTacgia.Location = new System.Drawing.Point(0, 192);
            this.btnTacgia.Name = "btnTacgia";
            this.btnTacgia.Padding = new System.Windows.Forms.Padding(0, 0, 20, 0);
            this.btnTacgia.Size = new System.Drawing.Size(386, 96);
            this.btnTacgia.TabIndex = 16;
            this.btnTacgia.Text = "Tác giả";
            this.btnTacgia.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnTacgia.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnTacgia.UseVisualStyleBackColor = true;
            this.btnTacgia.Click += new System.EventHandler(this.btnAuthor_Click);
            // 
            // btnBook
            // 
            this.btnBook.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnBook.FlatAppearance.BorderSize = 0;
            this.btnBook.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBook.Font = new System.Drawing.Font("Segoe UI", 10.875F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBook.ForeColor = System.Drawing.Color.White;
            this.btnBook.IconChar = FontAwesome.Sharp.IconChar.Book;
            this.btnBook.IconColor = System.Drawing.Color.White;
            this.btnBook.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnBook.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnBook.Location = new System.Drawing.Point(0, 96);
            this.btnBook.Margin = new System.Windows.Forms.Padding(0, 5, 0, 5);
            this.btnBook.Name = "btnBook";
            this.btnBook.Padding = new System.Windows.Forms.Padding(0, 0, 20, 0);
            this.btnBook.Size = new System.Drawing.Size(386, 96);
            this.btnBook.TabIndex = 9;
            this.btnBook.Text = "Quản lý sách";
            this.btnBook.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnBook.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnBook.UseVisualStyleBackColor = true;
            this.btnBook.Click += new System.EventHandler(this.btnBook_Click);
            // 
            // btnTrangchu
            // 
            this.btnTrangchu.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnTrangchu.FlatAppearance.BorderSize = 0;
            this.btnTrangchu.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTrangchu.Font = new System.Drawing.Font("Segoe UI", 10.875F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTrangchu.ForeColor = System.Drawing.Color.White;
            this.btnTrangchu.IconChar = FontAwesome.Sharp.IconChar.House;
            this.btnTrangchu.IconColor = System.Drawing.Color.White;
            this.btnTrangchu.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnTrangchu.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnTrangchu.Location = new System.Drawing.Point(0, 0);
            this.btnTrangchu.Margin = new System.Windows.Forms.Padding(0, 5, 0, 5);
            this.btnTrangchu.Name = "btnTrangchu";
            this.btnTrangchu.Padding = new System.Windows.Forms.Padding(0, 0, 20, 0);
            this.btnTrangchu.Size = new System.Drawing.Size(386, 96);
            this.btnTrangchu.TabIndex = 8;
            this.btnTrangchu.Text = "Trang chủ";
            this.btnTrangchu.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnTrangchu.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnTrangchu.UseVisualStyleBackColor = true;
            this.btnTrangchu.Click += new System.EventHandler(this.btnTrangchu_Click);
            // 
            // panelTop
            // 
            this.panelTop.BackColor = System.Drawing.Color.DarkGray;
            this.panelTop.Controls.Add(this.lblThoiGian);
            this.panelTop.Controls.Add(this.lblTitle);
            this.panelTop.Controls.Add(this.lblRole);
            this.panelTop.Controls.Add(this.label3);
            this.panelTop.Controls.Add(this.lblNguoiDung);
            this.panelTop.Controls.Add(this.label1);
            this.panelTop.Controls.Add(this.lblUser);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(2371, 145);
            this.panelTop.TabIndex = 4;
            // 
            // lblTitle
            // 
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(1189, 38);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(842, 65);
            this.lblTitle.TabIndex = 5;
            this.lblTitle.Text = "Quản lý cửa hàng sách";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblRole
            // 
            this.lblRole.AutoSize = true;
            this.lblRole.Font = new System.Drawing.Font("Segoe UI", 10.125F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRole.ForeColor = System.Drawing.Color.White;
            this.lblRole.Location = new System.Drawing.Point(541, 78);
            this.lblRole.Name = "lblRole";
            this.lblRole.Size = new System.Drawing.Size(0, 37);
            this.lblRole.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 10.125F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(406, 78);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(113, 37);
            this.label3.TabIndex = 3;
            this.label3.Text = "Quyền: ";
            // 
            // lblNguoiDung
            // 
            this.lblNguoiDung.AutoSize = true;
            this.lblNguoiDung.Font = new System.Drawing.Font("Segoe UI", 10.125F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNguoiDung.ForeColor = System.Drawing.Color.White;
            this.lblNguoiDung.Location = new System.Drawing.Point(541, 27);
            this.lblNguoiDung.Name = "lblNguoiDung";
            this.lblNguoiDung.Size = new System.Drawing.Size(0, 37);
            this.lblNguoiDung.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 10.125F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(406, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(141, 37);
            this.label1.TabIndex = 1;
            this.label1.Text = "Xin chào: ";
            // 
            // lblUser
            // 
            this.lblUser.AutoSize = true;
            this.lblUser.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUser.ForeColor = System.Drawing.Color.White;
            this.lblUser.Location = new System.Drawing.Point(55, 38);
            this.lblUser.Name = "lblUser";
            this.lblUser.Size = new System.Drawing.Size(222, 65);
            this.lblUser.TabIndex = 0;
            this.lblUser.Text = "Xin chào";
            // 
            // panelMain
            // 
            this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMain.Location = new System.Drawing.Point(386, 145);
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size(1985, 1031);
            this.panelMain.TabIndex = 5;
            // 
            // lblThoiGian
            // 
            this.lblThoiGian.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblThoiGian.Font = new System.Drawing.Font("Segoe UI", 10.125F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblThoiGian.ForeColor = System.Drawing.Color.White;
            this.lblThoiGian.Location = new System.Drawing.Point(2000, 60);
            this.lblThoiGian.Name = "lblThoiGian";
            this.lblThoiGian.Size = new System.Drawing.Size(359, 37);
            this.lblThoiGian.TabIndex = 6;
            this.lblThoiGian.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // FrmIndex
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(2371, 1176);
            this.Controls.Add(this.panelMain);
            this.Controls.Add(this.panelMenu);
            this.Controls.Add(this.panelTop);
            this.Name = "FrmIndex";
            this.Text = "FrmIndex";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.FrmIndex_Load);
            this.panelMenu.ResumeLayout(false);
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelMenu;
        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Label lblUser;
        private System.Windows.Forms.Panel panelMain;
        private FontAwesome.Sharp.IconButton btnLogout;
        private FontAwesome.Sharp.IconButton btnChart;
        private FontAwesome.Sharp.IconButton btnUser;
        private FontAwesome.Sharp.IconButton btnNhap;
        private FontAwesome.Sharp.IconButton btnBan;
        private FontAwesome.Sharp.IconButton btnBook;
        private FontAwesome.Sharp.IconButton btnTrangchu;
        private FontAwesome.Sharp.IconButton btnTacgia;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblNguoiDung;
        private System.Windows.Forms.Label lblRole;
        private FontAwesome.Sharp.IconButton btnNhatKy;
        private System.Windows.Forms.Label lblTitle;
        private FontAwesome.Sharp.IconButton btnTaiKhoan;
        private System.Windows.Forms.Label lblThoiGian;
        private System.Windows.Forms.Timer timer1;
    }
}