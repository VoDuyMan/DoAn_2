namespace SERVERQLCoffee.QLHD
{
    partial class frmHoaDon
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmHoaDon));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.dtpFilterDate = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cbMonthFilter = new System.Windows.Forms.ComboBox();
            this.BtnFilterThisYear = new System.Windows.Forms.Button();
            this.BtnFilterThisMonth = new System.Windows.Forms.Button();
            this.BtnFilterToday = new System.Windows.Forms.Button();
            this.dataGridViewHoaDon = new System.Windows.Forms.DataGridView();
            this.lblTitle = new System.Windows.Forms.Label();
            this.btnThoat = new System.Windows.Forms.Button();
            this.cbxChiNhanh = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewHoaDon)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.SkyBlue;
            this.groupBox1.Controls.Add(this.dtpFilterDate);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.cbMonthFilter);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.groupBox1.Location = new System.Drawing.Point(400, 505);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox1.Size = new System.Drawing.Size(431, 124);
            this.groupBox1.TabIndex = 91;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Lọc Thời Điểm Khác";
            // 
            // dtpFilterDate
            // 
            this.dtpFilterDate.Location = new System.Drawing.Point(171, 85);
            this.dtpFilterDate.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.dtpFilterDate.Name = "dtpFilterDate";
            this.dtpFilterDate.Size = new System.Drawing.Size(245, 28);
            this.dtpFilterDate.TabIndex = 3;
            this.dtpFilterDate.ValueChanged += new System.EventHandler(this.DtpFilterDate_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.label2.ForeColor = System.Drawing.Color.DarkSlateGray;
            this.label2.Location = new System.Drawing.Point(24, 85);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(117, 24);
            this.label2.TabIndex = 2;
            this.label2.Text = "Chọn ngày:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.label1.ForeColor = System.Drawing.Color.DarkSlateGray;
            this.label1.Location = new System.Drawing.Point(24, 38);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(132, 24);
            this.label1.TabIndex = 1;
            this.label1.Text = "Chọn Tháng:";
            // 
            // cbMonthFilter
            // 
            this.cbMonthFilter.FormattingEnabled = true;
            this.cbMonthFilter.Location = new System.Drawing.Point(171, 34);
            this.cbMonthFilter.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cbMonthFilter.Name = "cbMonthFilter";
            this.cbMonthFilter.Size = new System.Drawing.Size(160, 30);
            this.cbMonthFilter.TabIndex = 0;
            this.cbMonthFilter.SelectedIndexChanged += new System.EventHandler(this.CbMonthFilter_SelectedIndexChanged);
            // 
            // BtnFilterThisYear
            // 
            this.BtnFilterThisYear.BackColor = System.Drawing.Color.Transparent;
            this.BtnFilterThisYear.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.BtnFilterThisYear.ForeColor = System.Drawing.Color.Indigo;
            this.BtnFilterThisYear.Image = ((System.Drawing.Image)(resources.GetObject("BtnFilterThisYear.Image")));
            this.BtnFilterThisYear.Location = new System.Drawing.Point(219, 605);
            this.BtnFilterThisYear.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.BtnFilterThisYear.Name = "BtnFilterThisYear";
            this.BtnFilterThisYear.Size = new System.Drawing.Size(157, 62);
            this.BtnFilterThisYear.TabIndex = 90;
            this.BtnFilterThisYear.UseVisualStyleBackColor = false;
            this.BtnFilterThisYear.Click += new System.EventHandler(this.BtnFilterThisYear_Click);
            // 
            // BtnFilterThisMonth
            // 
            this.BtnFilterThisMonth.BackColor = System.Drawing.Color.Transparent;
            this.BtnFilterThisMonth.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.BtnFilterThisMonth.ForeColor = System.Drawing.Color.FloralWhite;
            this.BtnFilterThisMonth.Image = ((System.Drawing.Image)(resources.GetObject("BtnFilterThisMonth.Image")));
            this.BtnFilterThisMonth.Location = new System.Drawing.Point(219, 530);
            this.BtnFilterThisMonth.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.BtnFilterThisMonth.Name = "BtnFilterThisMonth";
            this.BtnFilterThisMonth.Size = new System.Drawing.Size(157, 67);
            this.BtnFilterThisMonth.TabIndex = 89;
            this.BtnFilterThisMonth.UseVisualStyleBackColor = false;
            this.BtnFilterThisMonth.Click += new System.EventHandler(this.BtnFilterThisMonth_Click);
            // 
            // BtnFilterToday
            // 
            this.BtnFilterToday.BackColor = System.Drawing.Color.FloralWhite;
            this.BtnFilterToday.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.BtnFilterToday.ForeColor = System.Drawing.Color.AliceBlue;
            this.BtnFilterToday.Image = ((System.Drawing.Image)(resources.GetObject("BtnFilterToday.Image")));
            this.BtnFilterToday.Location = new System.Drawing.Point(219, 459);
            this.BtnFilterToday.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.BtnFilterToday.Name = "BtnFilterToday";
            this.BtnFilterToday.Size = new System.Drawing.Size(157, 63);
            this.BtnFilterToday.TabIndex = 88;
            this.BtnFilterToday.UseVisualStyleBackColor = false;
            this.BtnFilterToday.Click += new System.EventHandler(this.BtnFilterToday_Click);
            // 
            // dataGridViewHoaDon
            // 
            this.dataGridViewHoaDon.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewHoaDon.Location = new System.Drawing.Point(65, 129);
            this.dataGridViewHoaDon.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.dataGridViewHoaDon.Name = "dataGridViewHoaDon";
            this.dataGridViewHoaDon.RowHeadersWidth = 51;
            this.dataGridViewHoaDon.Size = new System.Drawing.Size(1108, 322);
            this.dataGridViewHoaDon.TabIndex = 87;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.lblTitle.ForeColor = System.Drawing.SystemColors.Highlight;
            this.lblTitle.Location = new System.Drawing.Point(495, 48);
            this.lblTitle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(267, 36);
            this.lblTitle.TabIndex = 86;
            this.lblTitle.Text = "Quản Lý Hóa Đơn";
            // 
            // btnThoat
            // 
            this.btnThoat.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnThoat.Image = ((System.Drawing.Image)(resources.GetObject("btnThoat.Image")));
            this.btnThoat.Location = new System.Drawing.Point(960, 559);
            this.btnThoat.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnThoat.Name = "btnThoat";
            this.btnThoat.Size = new System.Drawing.Size(152, 70);
            this.btnThoat.TabIndex = 92;
            this.btnThoat.UseVisualStyleBackColor = true;
            this.btnThoat.Click += new System.EventHandler(this.btnThoat_Click);
            // 
            // cbxChiNhanh
            // 
            this.cbxChiNhanh.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.cbxChiNhanh.FormattingEnabled = true;
            this.cbxChiNhanh.Location = new System.Drawing.Point(960, 88);
            this.cbxChiNhanh.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cbxChiNhanh.Name = "cbxChiNhanh";
            this.cbxChiNhanh.Size = new System.Drawing.Size(205, 33);
            this.cbxChiNhanh.TabIndex = 93;
            this.cbxChiNhanh.SelectedIndexChanged += new System.EventHandler(this.CbxChiNhanh_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.label3.ForeColor = System.Drawing.Color.DarkSlateGray;
            this.label3.Location = new System.Drawing.Point(755, 91);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(179, 25);
            this.label3.TabIndex = 4;
            this.label3.Text = "Chọn Chi Nhánh:";
            // 
            // btnRefresh
            // 
            this.btnRefresh.BackColor = System.Drawing.Color.White;
            this.btnRefresh.Image = ((System.Drawing.Image)(resources.GetObject("btnRefresh.Image")));
            this.btnRefresh.Location = new System.Drawing.Point(65, 39);
            this.btnRefresh.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(140, 82);
            this.btnRefresh.TabIndex = 94;
            this.btnRefresh.UseVisualStyleBackColor = false;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // frmHoaDon
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightBlue;
            this.ClientSize = new System.Drawing.Size(1237, 675);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cbxChiNhanh);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.BtnFilterThisYear);
            this.Controls.Add(this.BtnFilterThisMonth);
            this.Controls.Add(this.BtnFilterToday);
            this.Controls.Add(this.dataGridViewHoaDon);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.btnThoat);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "frmHoaDon";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Quản Lý Hóa Đơn";
            this.Load += new System.EventHandler(this.frmHoaDon_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewHoaDon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DateTimePicker dtpFilterDate;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbMonthFilter;
        private System.Windows.Forms.Button BtnFilterThisYear;
        private System.Windows.Forms.Button BtnFilterThisMonth;
        private System.Windows.Forms.Button BtnFilterToday;
        private System.Windows.Forms.DataGridView dataGridViewHoaDon;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Button btnThoat;
        private System.Windows.Forms.ComboBox cbxChiNhanh;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnRefresh;
    }
}