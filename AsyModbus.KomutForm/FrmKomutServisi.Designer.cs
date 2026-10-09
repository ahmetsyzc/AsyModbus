namespace AsyModbus.KomutForm
{
    partial class FrmKomutServisi
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.pnlBaslik = new System.Windows.Forms.Panel();
            this.lblBaslik = new System.Windows.Forms.Label();
            this.lblDurum = new System.Windows.Forms.Label();
            this.btnBaslat = new System.Windows.Forms.Button();
            this.btnDurdur = new System.Windows.Forms.Button();
            this.pnlKayitlar = new System.Windows.Forms.Panel();
            this.lblKayitlar = new System.Windows.Forms.Label();
            this.lstLog = new System.Windows.Forms.ListBox();
            this.timerKomut = new System.Windows.Forms.Timer(this.components);
            this.pnlBaslik.SuspendLayout();
            this.pnlKayitlar.SuspendLayout();
            this.SuspendLayout();
            // pnlBaslik
            this.pnlBaslik.BackColor = System.Drawing.Color.FromArgb(33, 37, 41);
            this.pnlBaslik.Controls.Add(this.lblBaslik);
            this.pnlBaslik.Controls.Add(this.lblDurum);
            this.pnlBaslik.Controls.Add(this.btnBaslat);
            this.pnlBaslik.Controls.Add(this.btnDurdur);
            this.pnlBaslik.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlBaslik.Location = new System.Drawing.Point(20, 20);
            this.pnlBaslik.Name = "pnlBaslik";
            this.pnlBaslik.Size = new System.Drawing.Size(860, 104);
            this.pnlBaslik.TabIndex = 0;
            // lblBaslik
            this.lblBaslik.AutoSize = true;
            this.lblBaslik.Font = new System.Drawing.Font("Segoe UI", 17F, System.Drawing.FontStyle.Bold);
            this.lblBaslik.ForeColor = System.Drawing.Color.White;
            this.lblBaslik.Location = new System.Drawing.Point(20, 17);
            this.lblBaslik.Name = "lblBaslik";
            this.lblBaslik.TabIndex = 0;
            // lblDurum
            this.lblDurum.AutoSize = true;
            this.lblDurum.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblDurum.ForeColor = System.Drawing.Color.FromArgb(225, 225, 225);
            this.lblDurum.Location = new System.Drawing.Point(22, 61);
            this.lblDurum.Name = "lblDurum";
            this.lblDurum.TabIndex = 1;
            // btnBaslat
            this.btnBaslat.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.btnBaslat.BackColor = System.Drawing.Color.FromArgb(25, 135, 84);
            this.btnBaslat.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBaslat.FlatAppearance.BorderSize = 0;
            this.btnBaslat.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnBaslat.ForeColor = System.Drawing.Color.White;
            this.btnBaslat.Location = new System.Drawing.Point(600, 30);
            this.btnBaslat.Name = "btnBaslat";
            this.btnBaslat.Size = new System.Drawing.Size(112, 44);
            this.btnBaslat.TabIndex = 2;
            this.btnBaslat.UseVisualStyleBackColor = false;
            this.btnBaslat.Click += new System.EventHandler(this.btnBaslat_Click);
            // btnDurdur
            this.btnDurdur.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.btnDurdur.BackColor = System.Drawing.Color.FromArgb(220, 53, 69);
            this.btnDurdur.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDurdur.FlatAppearance.BorderSize = 0;
            this.btnDurdur.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnDurdur.ForeColor = System.Drawing.Color.White;
            this.btnDurdur.Location = new System.Drawing.Point(724, 30);
            this.btnDurdur.Name = "btnDurdur";
            this.btnDurdur.Size = new System.Drawing.Size(112, 44);
            this.btnDurdur.TabIndex = 3;
            this.btnDurdur.UseVisualStyleBackColor = false;
            this.btnDurdur.Click += new System.EventHandler(this.btnDurdur_Click);
            // pnlKayitlar
            this.pnlKayitlar.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.pnlKayitlar.BackColor = System.Drawing.Color.White;
            this.pnlKayitlar.Controls.Add(this.lstLog);
            this.pnlKayitlar.Controls.Add(this.lblKayitlar);
            this.pnlKayitlar.Location = new System.Drawing.Point(20, 140);
            this.pnlKayitlar.Name = "pnlKayitlar";
            this.pnlKayitlar.Size = new System.Drawing.Size(860, 360);
            this.pnlKayitlar.TabIndex = 1;
            // lblKayitlar
            this.lblKayitlar.AutoSize = true;
            this.lblKayitlar.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblKayitlar.ForeColor = System.Drawing.Color.FromArgb(33, 37, 41);
            this.lblKayitlar.Location = new System.Drawing.Point(18, 16);
            this.lblKayitlar.Name = "lblKayitlar";
            this.lblKayitlar.TabIndex = 0;
            // lstLog
            this.lstLog.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.lstLog.BackColor = System.Drawing.Color.FromArgb(248, 249, 250);
            this.lstLog.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lstLog.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lstLog.ForeColor = System.Drawing.Color.FromArgb(33, 37, 41);
            this.lstLog.FormattingEnabled = true;
            this.lstLog.HorizontalScrollbar = true;
            this.lstLog.IntegralHeight = false;
            this.lstLog.ItemHeight = 17;
            this.lstLog.Location = new System.Drawing.Point(20, 52);
            this.lstLog.Name = "lstLog";
            this.lstLog.Size = new System.Drawing.Size(820, 288);
            this.lstLog.TabIndex = 1;
            // timerKomut
            this.timerKomut.Interval = 5000;
            this.timerKomut.Tick += new System.EventHandler(this.timerKomut_Tick);
            // FrmKomutServisi
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(225, 225, 225);
            this.ClientSize = new System.Drawing.Size(900, 520);
            this.Controls.Add(this.pnlKayitlar);
            this.Controls.Add(this.pnlBaslik);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.MinimumSize = new System.Drawing.Size(800, 400);
            this.Name = "FrmKomutServisi";
            this.Padding = new System.Windows.Forms.Padding(20);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmKomutServisi_FormClosing);
            this.Load += new System.EventHandler(this.FrmKomutServisi_Load);
            this.pnlBaslik.ResumeLayout(false);
            this.pnlBaslik.PerformLayout();
            this.pnlKayitlar.ResumeLayout(false);
            this.pnlKayitlar.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel pnlBaslik;
        private System.Windows.Forms.Label lblBaslik;
        private System.Windows.Forms.Panel pnlKayitlar;
        private System.Windows.Forms.Label lblKayitlar;
        private System.Windows.Forms.Button btnBaslat;
        private System.Windows.Forms.Button btnDurdur;
        private System.Windows.Forms.Label lblDurum;
        private System.Windows.Forms.ListBox lstLog;
        private System.Windows.Forms.Timer timerKomut;
    }
}
