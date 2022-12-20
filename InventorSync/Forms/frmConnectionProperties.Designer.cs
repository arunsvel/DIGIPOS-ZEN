
namespace DigiposZen.Forms
{
    partial class frmConnectionProperties
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
            this.lblSqlServer = new System.Windows.Forms.Label();
            this.lblLicensedTo = new System.Windows.Forms.Label();
            this.lblCDKey = new System.Windows.Forms.Label();
            this.lblUserName = new System.Windows.Forms.Label();
            this.lblCompanyCode = new System.Windows.Forms.Label();
            this.lblServerClient = new System.Windows.Forms.Label();
            this.txtSqlServer = new System.Windows.Forms.TextBox();
            this.txtServerClient = new System.Windows.Forms.TextBox();
            this.txtCompanyCode = new System.Windows.Forms.TextBox();
            this.txtLicensedTo = new System.Windows.Forms.TextBox();
            this.txtCDKey = new System.Windows.Forms.TextBox();
            this.txtUserName = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.picBanner = new System.Windows.Forms.PictureBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnLogOut = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picBanner)).BeginInit();
            this.SuspendLayout();
            // 
            // lblSqlServer
            // 
            this.lblSqlServer.AutoSize = true;
            this.lblSqlServer.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSqlServer.Location = new System.Drawing.Point(41, 182);
            this.lblSqlServer.Name = "lblSqlServer";
            this.lblSqlServer.Size = new System.Drawing.Size(78, 18);
            this.lblSqlServer.TabIndex = 7;
            this.lblSqlServer.Text = "Sql Server:";
            this.lblSqlServer.Click += new System.EventHandler(this.label1_Click_1);
            // 
            // lblLicensedTo
            // 
            this.lblLicensedTo.AutoSize = true;
            this.lblLicensedTo.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLicensedTo.Location = new System.Drawing.Point(41, 342);
            this.lblLicensedTo.Name = "lblLicensedTo";
            this.lblLicensedTo.Size = new System.Drawing.Size(91, 18);
            this.lblLicensedTo.TabIndex = 10;
            this.lblLicensedTo.Text = "Licensed To:";
            // 
            // lblCDKey
            // 
            this.lblCDKey.AutoSize = true;
            this.lblCDKey.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCDKey.Location = new System.Drawing.Point(41, 310);
            this.lblCDKey.Name = "lblCDKey";
            this.lblCDKey.Size = new System.Drawing.Size(62, 18);
            this.lblCDKey.TabIndex = 11;
            this.lblCDKey.Text = "CD Key:";
            // 
            // lblUserName
            // 
            this.lblUserName.AutoSize = true;
            this.lblUserName.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUserName.Location = new System.Drawing.Point(41, 278);
            this.lblUserName.Name = "lblUserName";
            this.lblUserName.Size = new System.Drawing.Size(87, 18);
            this.lblUserName.TabIndex = 12;
            this.lblUserName.Text = "User Name:";
            // 
            // lblCompanyCode
            // 
            this.lblCompanyCode.AutoSize = true;
            this.lblCompanyCode.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCompanyCode.Location = new System.Drawing.Point(41, 246);
            this.lblCompanyCode.Name = "lblCompanyCode";
            this.lblCompanyCode.Size = new System.Drawing.Size(113, 18);
            this.lblCompanyCode.TabIndex = 13;
            this.lblCompanyCode.Text = "Company Code:";
            // 
            // lblServerClient
            // 
            this.lblServerClient.AutoSize = true;
            this.lblServerClient.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblServerClient.Location = new System.Drawing.Point(41, 214);
            this.lblServerClient.Name = "lblServerClient";
            this.lblServerClient.Size = new System.Drawing.Size(94, 18);
            this.lblServerClient.TabIndex = 14;
            this.lblServerClient.Text = "Server Client:";
            // 
            // txtSqlServer
            // 
            this.txtSqlServer.BackColor = System.Drawing.SystemColors.Control;
            this.txtSqlServer.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtSqlServer.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSqlServer.Location = new System.Drawing.Point(159, 181);
            this.txtSqlServer.Name = "txtSqlServer";
            this.txtSqlServer.ReadOnly = true;
            this.txtSqlServer.Size = new System.Drawing.Size(199, 20);
            this.txtSqlServer.TabIndex = 21;
            this.txtSqlServer.TextChanged += new System.EventHandler(this.txtSqlServer_TextChanged);
            // 
            // txtServerClient
            // 
            this.txtServerClient.BackColor = System.Drawing.SystemColors.Control;
            this.txtServerClient.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtServerClient.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtServerClient.Location = new System.Drawing.Point(159, 213);
            this.txtServerClient.Name = "txtServerClient";
            this.txtServerClient.ReadOnly = true;
            this.txtServerClient.Size = new System.Drawing.Size(199, 20);
            this.txtServerClient.TabIndex = 22;
            // 
            // txtCompanyCode
            // 
            this.txtCompanyCode.BackColor = System.Drawing.SystemColors.Control;
            this.txtCompanyCode.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtCompanyCode.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCompanyCode.Location = new System.Drawing.Point(159, 245);
            this.txtCompanyCode.Name = "txtCompanyCode";
            this.txtCompanyCode.ReadOnly = true;
            this.txtCompanyCode.Size = new System.Drawing.Size(199, 20);
            this.txtCompanyCode.TabIndex = 23;
            // 
            // txtLicensedTo
            // 
            this.txtLicensedTo.BackColor = System.Drawing.SystemColors.Control;
            this.txtLicensedTo.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtLicensedTo.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtLicensedTo.Location = new System.Drawing.Point(159, 341);
            this.txtLicensedTo.Name = "txtLicensedTo";
            this.txtLicensedTo.ReadOnly = true;
            this.txtLicensedTo.Size = new System.Drawing.Size(199, 20);
            this.txtLicensedTo.TabIndex = 26;
            // 
            // txtCDKey
            // 
            this.txtCDKey.BackColor = System.Drawing.SystemColors.Control;
            this.txtCDKey.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtCDKey.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCDKey.Location = new System.Drawing.Point(159, 309);
            this.txtCDKey.Name = "txtCDKey";
            this.txtCDKey.ReadOnly = true;
            this.txtCDKey.Size = new System.Drawing.Size(199, 20);
            this.txtCDKey.TabIndex = 25;
            // 
            // txtUserName
            // 
            this.txtUserName.BackColor = System.Drawing.SystemColors.Control;
            this.txtUserName.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtUserName.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtUserName.Location = new System.Drawing.Point(159, 277);
            this.txtUserName.Name = "txtUserName";
            this.txtUserName.ReadOnly = true;
            this.txtUserName.Size = new System.Drawing.Size(199, 20);
            this.txtUserName.TabIndex = 24;
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.btnLogOut);
            this.panel1.Controls.Add(this.btnOK);
            this.panel1.Controls.Add(this.lblCompanyCode);
            this.panel1.Controls.Add(this.picBanner);
            this.panel1.Controls.Add(this.lblSqlServer);
            this.panel1.Controls.Add(this.txtLicensedTo);
            this.panel1.Controls.Add(this.lblLicensedTo);
            this.panel1.Controls.Add(this.txtCDKey);
            this.panel1.Controls.Add(this.lblCDKey);
            this.panel1.Controls.Add(this.txtUserName);
            this.panel1.Controls.Add(this.lblUserName);
            this.panel1.Controls.Add(this.txtCompanyCode);
            this.panel1.Controls.Add(this.lblServerClient);
            this.panel1.Controls.Add(this.txtServerClient);
            this.panel1.Controls.Add(this.txtSqlServer);
            this.panel1.Location = new System.Drawing.Point(1, 2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(455, 435);
            this.panel1.TabIndex = 28;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // picBanner
            // 
            this.picBanner.Image = global::DigiposZen.Properties.Resources.logo;
            this.picBanner.Location = new System.Drawing.Point(0, 0);
            this.picBanner.Name = "picBanner";
            this.picBanner.Size = new System.Drawing.Size(455, 154);
            this.picBanner.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picBanner.TabIndex = 27;
            this.picBanner.TabStop = false;
            // 
            // btnOK
            // 
            this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOK.Location = new System.Drawing.Point(238, 372);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 40);
            this.btnOK.TabIndex = 29;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnLogOut
            // 
            this.btnLogOut.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLogOut.Location = new System.Drawing.Point(349, 372);
            this.btnLogOut.Name = "btnLogOut";
            this.btnLogOut.Size = new System.Drawing.Size(75, 40);
            this.btnLogOut.TabIndex = 30;
            this.btnLogOut.Text = "Log Out";
            this.btnLogOut.UseVisualStyleBackColor = true;
            // 
            // frmConnectionProperties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(457, 439);
            this.Controls.Add(this.panel1);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Segoe UI Semibold", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmConnectionProperties";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "frmConnectionProperties";
            this.Load += new System.EventHandler(this.frmConnectionProperties_Load);
            this.Shown += new System.EventHandler(this.frmConnectionProperties_Shown);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picBanner)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label lblSqlServer;
        private System.Windows.Forms.Label lblLicensedTo;
        private System.Windows.Forms.Label lblCDKey;
        private System.Windows.Forms.Label lblUserName;
        private System.Windows.Forms.Label lblCompanyCode;
        private System.Windows.Forms.Label lblServerClient;
        private System.Windows.Forms.TextBox txtSqlServer;
        private System.Windows.Forms.TextBox txtServerClient;
        private System.Windows.Forms.TextBox txtCompanyCode;
        private System.Windows.Forms.TextBox txtLicensedTo;
        private System.Windows.Forms.TextBox txtCDKey;
        private System.Windows.Forms.TextBox txtUserName;
        private System.Windows.Forms.PictureBox picBanner;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnLogOut;
        private System.Windows.Forms.Button btnOK;
    }
}