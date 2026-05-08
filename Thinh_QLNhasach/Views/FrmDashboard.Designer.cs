namespace Thinh_QLNhasach.Views
{
    partial class FrmDashboard
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Panel pnlMain;
        private System.Windows.Forms.Label lblHeaderTieuDe;
        
        private System.Windows.Forms.Panel pnlDoanhThu;
        private System.Windows.Forms.Label lblTitleDoanhThu;
        private System.Windows.Forms.Label lblTongDoanhThu;

        private System.Windows.Forms.Panel pnlHoaDon;
        private System.Windows.Forms.Label lblTitleHoaDon;
        private System.Windows.Forms.Label lblHoaDonHomNay;

        private System.Windows.Forms.Panel pnlTonKho;
        private System.Windows.Forms.Label lblTitleTonKho;
        private System.Windows.Forms.Label lblSachTonKho;

        private System.Windows.Forms.DataGridView dgvTop5;
        private System.Windows.Forms.Label lblTieuDeDgv;

        private System.Windows.Forms.DataVisualization.Charting.Chart chartDoanhSo;
        private System.Windows.Forms.Label lblTieuDeChart;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.pnlMain = new System.Windows.Forms.Panel();
            this.chartDoanhSo = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.lblTieuDeChart = new System.Windows.Forms.Label();
            this.dgvTop5 = new System.Windows.Forms.DataGridView();
            this.lblTieuDeDgv = new System.Windows.Forms.Label();
            this.pnlTonKho = new System.Windows.Forms.Panel();
            this.lblSachTonKho = new System.Windows.Forms.Label();
            this.lblTitleTonKho = new System.Windows.Forms.Label();
            this.pnlHoaDon = new System.Windows.Forms.Panel();
            this.lblHoaDonHomNay = new System.Windows.Forms.Label();
            this.lblTitleHoaDon = new System.Windows.Forms.Label();
            this.pnlDoanhThu = new System.Windows.Forms.Panel();
            this.lblTongDoanhThu = new System.Windows.Forms.Label();
            this.lblTitleDoanhThu = new System.Windows.Forms.Label();
            this.lblHeaderTieuDe = new System.Windows.Forms.Label();
            this.pnlMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartDoanhSo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTop5)).BeginInit();
            this.pnlTonKho.SuspendLayout();
            this.pnlHoaDon.SuspendLayout();
            this.pnlDoanhThu.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlMain
            // 
            this.pnlMain.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(246)))), ((int)(((byte)(249)))));
            this.pnlMain.Controls.Add(this.chartDoanhSo);
            this.pnlMain.Controls.Add(this.lblTieuDeChart);
            this.pnlMain.Controls.Add(this.dgvTop5);
            this.pnlMain.Controls.Add(this.lblTieuDeDgv);
            this.pnlMain.Controls.Add(this.pnlTonKho);
            this.pnlMain.Controls.Add(this.pnlHoaDon);
            this.pnlMain.Controls.Add(this.pnlDoanhThu);
            this.pnlMain.Controls.Add(this.lblHeaderTieuDe);
            this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMain.Location = new System.Drawing.Point(0, 0);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Size = new System.Drawing.Size(1904, 987);
            this.pnlMain.TabIndex = 0;
            // 
            // chartDoanhSo
            // 
            chartArea2.Name = "ChartArea1";
            this.chartDoanhSo.ChartAreas.Add(chartArea2);
            legend2.Name = "Legend1";
            this.chartDoanhSo.Legends.Add(legend2);
            this.chartDoanhSo.Location = new System.Drawing.Point(592, 270);
            this.chartDoanhSo.Name = "chartDoanhSo";
            series2.ChartArea = "ChartArea1";
            series2.IsValueShownAsLabel = true;
            series2.Legend = "Legend1";
            series2.Name = "DoanhSo";
            this.chartDoanhSo.Series.Add(series2);
            this.chartDoanhSo.Size = new System.Drawing.Size(534, 300);
            this.chartDoanhSo.TabIndex = 7;
            this.chartDoanhSo.Text = "chart1";
            // 
            // lblTieuDeChart
            // 
            this.lblTieuDeChart.AutoSize = true;
            this.lblTieuDeChart.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTieuDeChart.Location = new System.Drawing.Point(595, 230);
            this.lblTieuDeChart.Name = "lblTieuDeChart";
            this.lblTieuDeChart.Size = new System.Drawing.Size(619, 51);
            this.lblTieuDeChart.TabIndex = 6;
            this.lblTieuDeChart.Text = "📈 BIỂU ĐỒ DOANH THU 7 NGÀY";
            // 
            // dgvTop5
            // 
            this.dgvTop5.AllowUserToAddRows = false;
            this.dgvTop5.AllowUserToDeleteRows = false;
            this.dgvTop5.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvTop5.BackgroundColor = System.Drawing.Color.White;
            this.dgvTop5.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTop5.Location = new System.Drawing.Point(36, 270);
            this.dgvTop5.Name = "dgvTop5";
            this.dgvTop5.ReadOnly = true;
            this.dgvTop5.RowHeadersVisible = false;
            this.dgvTop5.RowHeadersWidth = 82;
            this.dgvTop5.RowTemplate.Height = 35;
            this.dgvTop5.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvTop5.Size = new System.Drawing.Size(534, 300);
            this.dgvTop5.TabIndex = 5;
            // 
            // lblTieuDeDgv
            // 
            this.lblTieuDeDgv.AutoSize = true;
            this.lblTieuDeDgv.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTieuDeDgv.Location = new System.Drawing.Point(31, 230);
            this.lblTieuDeDgv.Name = "lblTieuDeDgv";
            this.lblTieuDeDgv.Size = new System.Drawing.Size(503, 51);
            this.lblTieuDeDgv.TabIndex = 4;
            this.lblTieuDeDgv.Text = "🏆 TOP 5 SÁCH BÁN CHẠY";
            // 
            // pnlTonKho
            // 
            this.pnlTonKho.BackColor = System.Drawing.Color.White;
            this.pnlTonKho.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlTonKho.Controls.Add(this.lblSachTonKho);
            this.pnlTonKho.Controls.Add(this.lblTitleTonKho);
            this.pnlTonKho.Location = new System.Drawing.Point(776, 80);
            this.pnlTonKho.Name = "pnlTonKho";
            this.pnlTonKho.Size = new System.Drawing.Size(350, 120);
            this.pnlTonKho.TabIndex = 3;
            // 
            // lblSachTonKho
            // 
            this.lblSachTonKho.AutoSize = true;
            this.lblSachTonKho.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold);
            this.lblSachTonKho.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(253)))), ((int)(((byte)(126)))), ((int)(((byte)(20)))));
            this.lblSachTonKho.Location = new System.Drawing.Point(18, 45);
            this.lblSachTonKho.Name = "lblSachTonKho";
            this.lblSachTonKho.Size = new System.Drawing.Size(74, 86);
            this.lblSachTonKho.TabIndex = 0;
            this.lblSachTonKho.Text = "0";
            // 
            // lblTitleTonKho
            // 
            this.lblTitleTonKho.AutoSize = true;
            this.lblTitleTonKho.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblTitleTonKho.ForeColor = System.Drawing.Color.Gray;
            this.lblTitleTonKho.Location = new System.Drawing.Point(20, 15);
            this.lblTitleTonKho.Name = "lblTitleTonKho";
            this.lblTitleTonKho.Size = new System.Drawing.Size(262, 45);
            this.lblTitleTonKho.TabIndex = 1;
            this.lblTitleTonKho.Text = "SÁCH TỒN KHO";
            // 
            // pnlHoaDon
            // 
            this.pnlHoaDon.BackColor = System.Drawing.Color.White;
            this.pnlHoaDon.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlHoaDon.Controls.Add(this.lblHoaDonHomNay);
            this.pnlHoaDon.Controls.Add(this.lblTitleHoaDon);
            this.pnlHoaDon.Location = new System.Drawing.Point(406, 80);
            this.pnlHoaDon.Name = "pnlHoaDon";
            this.pnlHoaDon.Size = new System.Drawing.Size(350, 120);
            this.pnlHoaDon.TabIndex = 2;
            // 
            // lblHoaDonHomNay
            // 
            this.lblHoaDonHomNay.AutoSize = true;
            this.lblHoaDonHomNay.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold);
            this.lblHoaDonHomNay.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(13)))), ((int)(((byte)(110)))), ((int)(((byte)(253)))));
            this.lblHoaDonHomNay.Location = new System.Drawing.Point(18, 45);
            this.lblHoaDonHomNay.Name = "lblHoaDonHomNay";
            this.lblHoaDonHomNay.Size = new System.Drawing.Size(74, 86);
            this.lblHoaDonHomNay.TabIndex = 0;
            this.lblHoaDonHomNay.Text = "0";
            // 
            // lblTitleHoaDon
            // 
            this.lblTitleHoaDon.AutoSize = true;
            this.lblTitleHoaDon.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblTitleHoaDon.ForeColor = System.Drawing.Color.Gray;
            this.lblTitleHoaDon.Location = new System.Drawing.Point(20, 15);
            this.lblTitleHoaDon.Name = "lblTitleHoaDon";
            this.lblTitleHoaDon.Size = new System.Drawing.Size(338, 45);
            this.lblTitleHoaDon.TabIndex = 1;
            this.lblTitleHoaDon.Text = "HÓA ĐƠN HÔM NAY";
            // 
            // pnlDoanhThu
            // 
            this.pnlDoanhThu.BackColor = System.Drawing.Color.White;
            this.pnlDoanhThu.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlDoanhThu.Controls.Add(this.lblTongDoanhThu);
            this.pnlDoanhThu.Controls.Add(this.lblTitleDoanhThu);
            this.pnlDoanhThu.Location = new System.Drawing.Point(36, 80);
            this.pnlDoanhThu.Name = "pnlDoanhThu";
            this.pnlDoanhThu.Size = new System.Drawing.Size(350, 120);
            this.pnlDoanhThu.TabIndex = 1;
            // 
            // lblTongDoanhThu
            // 
            this.lblTongDoanhThu.AutoSize = true;
            this.lblTongDoanhThu.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold);
            this.lblTongDoanhThu.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(201)))), ((int)(((byte)(151)))));
            this.lblTongDoanhThu.Location = new System.Drawing.Point(18, 45);
            this.lblTongDoanhThu.Name = "lblTongDoanhThu";
            this.lblTongDoanhThu.Size = new System.Drawing.Size(74, 86);
            this.lblTongDoanhThu.TabIndex = 0;
            this.lblTongDoanhThu.Text = "0";
            // 
            // lblTitleDoanhThu
            // 
            this.lblTitleDoanhThu.AutoSize = true;
            this.lblTitleDoanhThu.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblTitleDoanhThu.ForeColor = System.Drawing.Color.Gray;
            this.lblTitleDoanhThu.Location = new System.Drawing.Point(20, 15);
            this.lblTitleDoanhThu.Name = "lblTitleDoanhThu";
            this.lblTitleDoanhThu.Size = new System.Drawing.Size(317, 45);
            this.lblTitleDoanhThu.TabIndex = 1;
            this.lblTitleDoanhThu.Text = "TỔNG DOANH THU";
            // 
            // lblHeaderTieuDe
            // 
            this.lblHeaderTieuDe.AutoSize = true;
            this.lblHeaderTieuDe.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHeaderTieuDe.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.lblHeaderTieuDe.Location = new System.Drawing.Point(30, 20);
            this.lblHeaderTieuDe.Name = "lblHeaderTieuDe";
            this.lblHeaderTieuDe.Size = new System.Drawing.Size(399, 65);
            this.lblHeaderTieuDe.TabIndex = 0;
            this.lblHeaderTieuDe.Text = "🖥 TỔNG QUAN";
            // 
            // FrmDashboard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(14F, 36F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1904, 987);
            this.Controls.Add(this.pnlMain);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "FrmDashboard";
            this.Text = "Dashboard - Tổng quan";
            this.Load += new System.EventHandler(this.FrmDashboard_Load);
            this.pnlMain.ResumeLayout(false);
            this.pnlMain.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartDoanhSo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTop5)).EndInit();
            this.pnlTonKho.ResumeLayout(false);
            this.pnlTonKho.PerformLayout();
            this.pnlHoaDon.ResumeLayout(false);
            this.pnlHoaDon.PerformLayout();
            this.pnlDoanhThu.ResumeLayout(false);
            this.pnlDoanhThu.PerformLayout();
            this.ResumeLayout(false);

        }
    }
}