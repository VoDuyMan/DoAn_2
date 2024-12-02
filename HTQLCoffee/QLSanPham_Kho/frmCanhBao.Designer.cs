
namespace HTQLCoffee.QLSanPham_Kho
{
    partial class frmCanhBao
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmCanhBao));
            this.dtgCanhBao = new System.Windows.Forms.DataGridView();
            this.label1 = new System.Windows.Forms.Label();
            this.btnNhapHang = new System.Windows.Forms.Button();
            this.btnHuy = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dtgCanhBao)).BeginInit();
            this.SuspendLayout();
            // 
            // dtgCanhBao
            // 
            this.dtgCanhBao.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dtgCanhBao.Location = new System.Drawing.Point(80, 71);
            this.dtgCanhBao.Margin = new System.Windows.Forms.Padding(4);
            this.dtgCanhBao.Name = "dtgCanhBao";
            this.dtgCanhBao.ReadOnly = true;
            this.dtgCanhBao.RowHeadersWidth = 51;
            this.dtgCanhBao.Size = new System.Drawing.Size(448, 223);
            this.dtgCanhBao.TabIndex = 57;
            this.dtgCanhBao.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dtgCanhBao_CellClick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.label1.ForeColor = System.Drawing.SystemColors.Highlight;
            this.label1.Location = new System.Drawing.Point(117, 20);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(361, 36);
            this.label1.TabIndex = 56;
            this.label1.Text = "Cảnh Báo Sắp Hết Hàng";
            // 
            // btnNhapHang
            // 
            this.btnNhapHang.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnNhapHang.Image = ((System.Drawing.Image)(resources.GetObject("btnNhapHang.Image")));
            this.btnNhapHang.Location = new System.Drawing.Point(22, 302);
            this.btnNhapHang.Margin = new System.Windows.Forms.Padding(4);
            this.btnNhapHang.Name = "btnNhapHang";
            this.btnNhapHang.Size = new System.Drawing.Size(134, 86);
            this.btnNhapHang.TabIndex = 59;
            this.btnNhapHang.UseVisualStyleBackColor = true;
            this.btnNhapHang.Click += new System.EventHandler(this.btnNhapHang_Click);
            // 
            // btnHuy
            // 
            this.btnHuy.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnHuy.Image = ((System.Drawing.Image)(resources.GetObject("btnHuy.Image")));
            this.btnHuy.Location = new System.Drawing.Point(437, 302);
            this.btnHuy.Margin = new System.Windows.Forms.Padding(4);
            this.btnHuy.Name = "btnHuy";
            this.btnHuy.Size = new System.Drawing.Size(121, 86);
            this.btnHuy.TabIndex = 58;
            this.btnHuy.UseVisualStyleBackColor = true;
            this.btnHuy.Click += new System.EventHandler(this.btnHuy_Click);
            // 
            // frmCanhBao
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.ClientSize = new System.Drawing.Size(591, 401);
            this.Controls.Add(this.btnNhapHang);
            this.Controls.Add(this.btnHuy);
            this.Controls.Add(this.dtgCanhBao);
            this.Controls.Add(this.label1);
            this.Name = "frmCanhBao";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Cảnh Báo Sắp Hết Hàng";
            this.Load += new System.EventHandler(this.frmCanhBao_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dtgCanhBao)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnNhapHang;
        private System.Windows.Forms.Button btnHuy;
        private System.Windows.Forms.DataGridView dtgCanhBao;
        private System.Windows.Forms.Label label1;
    }
}