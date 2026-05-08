namespace Thinh_QLNhasach.Views
{
    partial class FrmTaoSachMoi
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtTenSach = new System.Windows.Forms.TextBox();
            this.cboMaTG = new System.Windows.Forms.ComboBox();
            this.cboMaTL = new System.Windows.Forms.ComboBox();
            this.cboMaNXB = new System.Windows.Forms.ComboBox();
            this.btnSave = new FontAwesome.Sharp.IconButton();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cboMaNXB);
            this.groupBox1.Controls.Add(this.cboMaTL);
            this.groupBox1.Controls.Add(this.cboMaTG);
            this.groupBox1.Controls.Add(this.txtTenSach);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Font = new System.Drawing.Font("Segoe UI", 10.125F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(763, 370);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Nhập thông tin";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 61);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(125, 37);
            this.label1.TabIndex = 0;
            this.label1.Text = "Tên sách";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 131);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(151, 37);
            this.label2.TabIndex = 1;
            this.label2.Text = "Mã tác giả";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 208);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(162, 37);
            this.label3.TabIndex = 2;
            this.label3.Text = "Mã thể loại";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 294);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(121, 37);
            this.label4.TabIndex = 3;
            this.label4.Text = "Mã NXB";
            // 
            // txtTenSach
            // 
            this.txtTenSach.Location = new System.Drawing.Point(221, 58);
            this.txtTenSach.Name = "txtTenSach";
            this.txtTenSach.Size = new System.Drawing.Size(517, 43);
            this.txtTenSach.TabIndex = 4;
            // 
            // cboMaTG
            // 
            this.cboMaTG.FormattingEnabled = true;
            this.cboMaTG.Location = new System.Drawing.Point(221, 128);
            this.cboMaTG.Name = "cboMaTG";
            this.cboMaTG.Size = new System.Drawing.Size(517, 45);
            this.cboMaTG.TabIndex = 5;
            // 
            // cboMaTL
            // 
            this.cboMaTL.FormattingEnabled = true;
            this.cboMaTL.Location = new System.Drawing.Point(221, 205);
            this.cboMaTL.Name = "cboMaTL";
            this.cboMaTL.Size = new System.Drawing.Size(517, 45);
            this.cboMaTL.TabIndex = 6;
            // 
            // cboMaNXB
            // 
            this.cboMaNXB.FormattingEnabled = true;
            this.cboMaNXB.Location = new System.Drawing.Point(221, 291);
            this.cboMaNXB.Name = "cboMaNXB";
            this.cboMaNXB.Size = new System.Drawing.Size(517, 45);
            this.cboMaNXB.TabIndex = 7;
            // 
            // btnSave
            // 
            this.btnSave.IconChar = FontAwesome.Sharp.IconChar.FloppyDisk;
            this.btnSave.IconColor = System.Drawing.Color.Black;
            this.btnSave.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnSave.IconSize = 32;
            this.btnSave.Location = new System.Drawing.Point(599, 408);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(176, 46);
            this.btnSave.TabIndex = 62;
            this.btnSave.Text = "Save";
            this.btnSave.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // FrmTaoSachMoi
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(985, 529);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.groupBox1);
            this.Name = "FrmTaoSachMoi";
            this.Text = "FrmTaoSachMoi";
            this.Click += new System.EventHandler(this.FrmTaoSachMoi_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cboMaNXB;
        private System.Windows.Forms.ComboBox cboMaTL;
        private System.Windows.Forms.ComboBox cboMaTG;
        private System.Windows.Forms.TextBox txtTenSach;
        private FontAwesome.Sharp.IconButton btnSave;
    }
}