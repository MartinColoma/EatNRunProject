namespace EatNRunProject
{
    partial class EatnRun
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
            this.UserPickerPanel = new System.Windows.Forms.Panel();
            this.CashierPB = new System.Windows.Forms.PictureBox();
            this.ManagerPB = new System.Windows.Forms.PictureBox();
            this.AdminPB = new System.Windows.Forms.PictureBox();
            this.ENRLogo1 = new System.Windows.Forms.PictureBox();
            this.LoginPanel = new System.Windows.Forms.Panel();
            this.exitBtn = new FontAwesome.Sharp.IconButton();
            this.UserLbl = new System.Windows.Forms.Label();
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.UserPickerPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.CashierPB)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ManagerPB)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.AdminPB)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ENRLogo1)).BeginInit();
            this.LoginPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // UserPickerPanel
            // 
            this.UserPickerPanel.Controls.Add(this.CashierPB);
            this.UserPickerPanel.Controls.Add(this.ManagerPB);
            this.UserPickerPanel.Controls.Add(this.AdminPB);
            this.UserPickerPanel.Controls.Add(this.ENRLogo1);
            this.UserPickerPanel.Location = new System.Drawing.Point(99, 95);
            this.UserPickerPanel.Name = "UserPickerPanel";
            this.UserPickerPanel.Size = new System.Drawing.Size(498, 455);
            this.UserPickerPanel.TabIndex = 0;
            // 
            // CashierPB
            // 
            this.CashierPB.Image = global::EatNRunProject.Properties.Resources.Cashier;
            this.CashierPB.Location = new System.Drawing.Point(337, 163);
            this.CashierPB.Name = "CashierPB";
            this.CashierPB.Size = new System.Drawing.Size(128, 128);
            this.CashierPB.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.CashierPB.TabIndex = 3;
            this.CashierPB.TabStop = false;
            this.CashierPB.Click += new System.EventHandler(this.CashierPB_Click);
            // 
            // ManagerPB
            // 
            this.ManagerPB.Image = global::EatNRunProject.Properties.Resources.Manager;
            this.ManagerPB.Location = new System.Drawing.Point(182, 163);
            this.ManagerPB.Name = "ManagerPB";
            this.ManagerPB.Size = new System.Drawing.Size(128, 128);
            this.ManagerPB.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.ManagerPB.TabIndex = 2;
            this.ManagerPB.TabStop = false;
            this.ManagerPB.Click += new System.EventHandler(this.ManagerPB_Click);
            // 
            // AdminPB
            // 
            this.AdminPB.Image = global::EatNRunProject.Properties.Resources.Admin;
            this.AdminPB.Location = new System.Drawing.Point(33, 163);
            this.AdminPB.Name = "AdminPB";
            this.AdminPB.Size = new System.Drawing.Size(128, 128);
            this.AdminPB.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.AdminPB.TabIndex = 1;
            this.AdminPB.TabStop = false;
            this.AdminPB.Click += new System.EventHandler(this.AdminPB_Click);
            // 
            // ENRLogo1
            // 
            this.ENRLogo1.Image = global::EatNRunProject.Properties.Resources.AP_Logo_128x128;
            this.ENRLogo1.Location = new System.Drawing.Point(184, 0);
            this.ENRLogo1.Name = "ENRLogo1";
            this.ENRLogo1.Size = new System.Drawing.Size(128, 128);
            this.ENRLogo1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.ENRLogo1.TabIndex = 0;
            this.ENRLogo1.TabStop = false;
            // 
            // LoginPanel
            // 
            this.LoginPanel.Controls.Add(this.exitBtn);
            this.LoginPanel.Controls.Add(this.UserLbl);
            this.LoginPanel.Controls.Add(this.pictureBox4);
            this.LoginPanel.Location = new System.Drawing.Point(657, 95);
            this.LoginPanel.Name = "LoginPanel";
            this.LoginPanel.Size = new System.Drawing.Size(498, 455);
            this.LoginPanel.TabIndex = 4;
            // 
            // exitBtn
            // 
            this.exitBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(206)))), ((int)(((byte)(51)))), ((int)(((byte)(68)))));
            this.exitBtn.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(206)))), ((int)(((byte)(51)))), ((int)(((byte)(68)))));
            this.exitBtn.FlatAppearance.BorderSize = 0;
            this.exitBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.exitBtn.IconChar = FontAwesome.Sharp.IconChar.Xmark;
            this.exitBtn.IconColor = System.Drawing.SystemColors.ButtonHighlight;
            this.exitBtn.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.exitBtn.IconSize = 24;
            this.exitBtn.Location = new System.Drawing.Point(452, 13);
            this.exitBtn.Name = "exitBtn";
            this.exitBtn.Size = new System.Drawing.Size(43, 34);
            this.exitBtn.TabIndex = 1;
            this.exitBtn.UseVisualStyleBackColor = false;
            this.exitBtn.Click += new System.EventHandler(this.exitBtn_Click);
            // 
            // UserLbl
            // 
            this.UserLbl.Font = new System.Drawing.Font("Arial Black", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.UserLbl.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(222)))), ((int)(((byte)(141)))));
            this.UserLbl.Location = new System.Drawing.Point(60, 131);
            this.UserLbl.Name = "UserLbl";
            this.UserLbl.Size = new System.Drawing.Size(378, 47);
            this.UserLbl.TabIndex = 0;
            this.UserLbl.Text = "Welcome back, ";
            this.UserLbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pictureBox4
            // 
            this.pictureBox4.Image = global::EatNRunProject.Properties.Resources.AP_Logo_128x128;
            this.pictureBox4.Location = new System.Drawing.Point(184, 0);
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.Size = new System.Drawing.Size(128, 128);
            this.pictureBox4.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox4.TabIndex = 0;
            this.pictureBox4.TabStop = false;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.UserPickerPanel);
            this.panel1.Controls.Add(this.LoginPanel);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1264, 681);
            this.panel1.TabIndex = 5;
            // 
            // EatnRun
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(206)))), ((int)(((byte)(51)))), ((int)(((byte)(68)))));
            this.ClientSize = new System.Drawing.Size(1264, 681);
            this.Controls.Add(this.panel1);
            this.Name = "EatnRun";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "EAT N\' RUN";
            this.Load += new System.EventHandler(this.EatnRun_Load);
            this.UserPickerPanel.ResumeLayout(false);
            this.UserPickerPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.CashierPB)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ManagerPB)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.AdminPB)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ENRLogo1)).EndInit();
            this.LoginPanel.ResumeLayout(false);
            this.LoginPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel UserPickerPanel;
        private System.Windows.Forms.PictureBox ENRLogo1;
        private System.Windows.Forms.PictureBox CashierPB;
        private System.Windows.Forms.PictureBox ManagerPB;
        private System.Windows.Forms.PictureBox AdminPB;
        private System.Windows.Forms.Panel LoginPanel;
        private System.Windows.Forms.PictureBox pictureBox4;
        private System.Windows.Forms.Label UserLbl;
        private System.Windows.Forms.Panel panel1;
        private FontAwesome.Sharp.IconButton exitBtn;
    }
}

