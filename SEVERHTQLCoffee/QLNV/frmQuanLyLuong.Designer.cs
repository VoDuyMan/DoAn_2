namespace SERVERQLCoffee.QLNV
{
    partial class frmQuanLyLuong
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmQuanLyLuong));
            this.btnHuy = new System.Windows.Forms.Button();
            this.btnLichSu = new System.Windows.Forms.Button();
            this.txtLuongLanh = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtTienPhat = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtLuongThuong = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtTongGioLam = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.dgvLuongNhanVien = new System.Windows.Forms.DataGridView();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLuongNhanVien)).BeginInit();
            this.SuspendLayout();
            // 
            // btnHuy
            // 
            this.btnHuy.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnHuy.Image = ((System.Drawing.Image)(resources.GetObject("btnHuy.Image")));
            this.btnHuy.Location = new System.Drawing.Point(839, 341);
            this.btnHuy.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnHuy.Name = "btnHuy";
            this.btnHuy.Size = new System.Drawing.Size(158, 84);
            this.btnHuy.TabIndex = 86;
            this.btnHuy.UseVisualStyleBackColor = true;
            this.btnHuy.Click += new System.EventHandler(this.btnHuy_Click);
            // 
            // btnLichSu
            // 
            this.btnLichSu.Image = ((System.Drawing.Image)(resources.GetObject("btnLichSu.Image")));
            this.btnLichSu.Location = new System.Drawing.Point(65, 343);
            this.btnLichSu.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnLichSu.Name = "btnLichSu";
            this.btnLichSu.Size = new System.Drawing.Size(148, 82);
            this.btnLichSu.TabIndex = 85;
            this.btnLichSu.UseVisualStyleBackColor = true;
            this.btnLichSu.Click += new System.EventHandler(this.btnLichSu_Click);
            // 
            // txtLuongLanh
            // 
            this.txtLuongLanh.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.txtLuongLanh.ForeColor = System.Drawing.SystemColors.Highlight;
            this.txtLuongLanh.Location = new System.Drawing.Point(649, 395);
            this.txtLuongLanh.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtLuongLanh.Name = "txtLuongLanh";
            this.txtLuongLanh.ReadOnly = true;
            this.txtLuongLanh.Size = new System.Drawing.Size(131, 30);
            this.txtLuongLanh.TabIndex = 84;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.label4.Location = new System.Drawing.Point(527, 403);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(96, 20);
            this.label4.TabIndex = 83;
            this.label4.Text = "Lương lãnh:";
            // 
            // txtTienPhat
            // 
            this.txtTienPhat.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.txtTienPhat.ForeColor = System.Drawing.Color.Red;
            this.txtTienPhat.Location = new System.Drawing.Point(649, 341);
            this.txtTienPhat.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtTienPhat.Name = "txtTienPhat";
            this.txtTienPhat.ReadOnly = true;
            this.txtTienPhat.Size = new System.Drawing.Size(131, 30);
            this.txtTienPhat.TabIndex = 82;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.label3.Location = new System.Drawing.Point(527, 349);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(99, 20);
            this.label3.TabIndex = 81;
            this.label3.Text = "Lương Phạt:";
            // 
            // txtLuongThuong
            // 
            this.txtLuongThuong.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.txtLuongThuong.ForeColor = System.Drawing.Color.Green;
            this.txtLuongThuong.Location = new System.Drawing.Point(369, 395);
            this.txtLuongThuong.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtLuongThuong.Name = "txtLuongThuong";
            this.txtLuongThuong.ReadOnly = true;
            this.txtLuongThuong.Size = new System.Drawing.Size(131, 30);
            this.txtLuongThuong.TabIndex = 80;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.label2.Location = new System.Drawing.Point(235, 403);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(115, 20);
            this.label2.TabIndex = 79;
            this.label2.Text = "Lương thưởng:";
            // 
            // txtTongGioLam
            // 
            this.txtTongGioLam.Enabled = false;
            this.txtTongGioLam.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.txtTongGioLam.Location = new System.Drawing.Point(369, 341);
            this.txtTongGioLam.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtTongGioLam.Name = "txtTongGioLam";
            this.txtTongGioLam.ReadOnly = true;
            this.txtTongGioLam.Size = new System.Drawing.Size(131, 30);
            this.txtTongGioLam.TabIndex = 78;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.label13.Location = new System.Drawing.Point(235, 349);
            this.label13.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(110, 20);
            this.label13.TabIndex = 77;
            this.label13.Text = "Tổng giờ làm:";
            // 
            // dgvLuongNhanVien
            // 
            this.dgvLuongNhanVien.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvLuongNhanVien.Location = new System.Drawing.Point(65, 74);
            this.dgvLuongNhanVien.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.dgvLuongNhanVien.Name = "dgvLuongNhanVien";
            this.dgvLuongNhanVien.ReadOnly = true;
            this.dgvLuongNhanVien.RowHeadersWidth = 51;
            this.dgvLuongNhanVien.Size = new System.Drawing.Size(932, 234);
            this.dgvLuongNhanVien.TabIndex = 76;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.label1.ForeColor = System.Drawing.SystemColors.Highlight;
            this.label1.Location = new System.Drawing.Point(327, 21);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(392, 36);
            this.label1.TabIndex = 75;
            this.label1.Text = "Quản Lý Lương Nhân Viên";
            // 
            // frmQuanLyLuong
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.ClientSize = new System.Drawing.Size(1075, 454);
            this.Controls.Add(this.btnHuy);
            this.Controls.Add(this.btnLichSu);
            this.Controls.Add(this.txtLuongLanh);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtTienPhat);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtLuongThuong);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtTongGioLam);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.dgvLuongNhanVien);
            this.Controls.Add(this.label1);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "frmQuanLyLuong";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Quản Lý Lương Nhân Viên";
            ((System.ComponentModel.ISupportInitialize)(this.dgvLuongNhanVien)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnHuy;
        private System.Windows.Forms.Button btnLichSu;
        private System.Windows.Forms.TextBox txtLuongLanh;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtTienPhat;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtLuongThuong;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtTongGioLam;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.DataGridView dgvLuongNhanVien;
        private System.Windows.Forms.Label label1;
    }
}