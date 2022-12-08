
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
            this.lblCompanyCode = new System.Windows.Forms.Label();
            this.lblUserName = new System.Windows.Forms.Label();
            this.lblServerClient = new System.Windows.Forms.Label();
            this.lblCDKey = new System.Windows.Forms.Label();
            this.lblLicensedTo = new System.Windows.Forms.Label();
            this.lblLogOut = new System.Windows.Forms.Label();
            this.lblChangePassword = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblSqlServer
            // 
            this.lblSqlServer.AutoSize = true;
            this.lblSqlServer.Location = new System.Drawing.Point(308, 44);
            this.lblSqlServer.Name = "lblSqlServer";
            this.lblSqlServer.Size = new System.Drawing.Size(70, 17);
            this.lblSqlServer.TabIndex = 0;
            this.lblSqlServer.Text = "SqlServer";
            // 
            // lblCompanyCode
            // 
            this.lblCompanyCode.AutoSize = true;
            this.lblCompanyCode.Location = new System.Drawing.Point(308, 113);
            this.lblCompanyCode.Name = "lblCompanyCode";
            this.lblCompanyCode.Size = new System.Drawing.Size(100, 17);
            this.lblCompanyCode.TabIndex = 1;
            this.lblCompanyCode.Text = "CompanyCode";
            // 
            // lblUserName
            // 
            this.lblUserName.AutoSize = true;
            this.lblUserName.Location = new System.Drawing.Point(308, 152);
            this.lblUserName.Name = "lblUserName";
            this.lblUserName.Size = new System.Drawing.Size(75, 17);
            this.lblUserName.TabIndex = 2;
            this.lblUserName.Text = "UserName";
            this.lblUserName.Click += new System.EventHandler(this.label1_Click);
            // 
            // lblServerClient
            // 
            this.lblServerClient.AutoSize = true;
            this.lblServerClient.Location = new System.Drawing.Point(308, 77);
            this.lblServerClient.Name = "lblServerClient";
            this.lblServerClient.Size = new System.Drawing.Size(85, 17);
            this.lblServerClient.TabIndex = 3;
            this.lblServerClient.Text = "ServerClient";
            // 
            // lblCDKey
            // 
            this.lblCDKey.AutoSize = true;
            this.lblCDKey.Location = new System.Drawing.Point(308, 194);
            this.lblCDKey.Name = "lblCDKey";
            this.lblCDKey.Size = new System.Drawing.Size(51, 17);
            this.lblCDKey.TabIndex = 4;
            this.lblCDKey.Text = "CDKey";
            // 
            // lblLicensedTo
            // 
            this.lblLicensedTo.AutoSize = true;
            this.lblLicensedTo.Location = new System.Drawing.Point(308, 262);
            this.lblLicensedTo.Name = "lblLicensedTo";
            this.lblLicensedTo.Size = new System.Drawing.Size(82, 17);
            this.lblLicensedTo.TabIndex = 5;
            this.lblLicensedTo.Text = "LicensedTo";
            // 
            // lblLogOut
            // 
            this.lblLogOut.AutoSize = true;
            this.lblLogOut.Location = new System.Drawing.Point(308, 319);
            this.lblLogOut.Name = "lblLogOut";
            this.lblLogOut.Size = new System.Drawing.Size(55, 17);
            this.lblLogOut.TabIndex = 6;
            this.lblLogOut.Text = "LogOut";
            // 
            // lblChangePassword
            // 
            this.lblChangePassword.AutoSize = true;
            this.lblChangePassword.Location = new System.Drawing.Point(472, 319);
            this.lblChangePassword.Name = "lblChangePassword";
            this.lblChangePassword.Size = new System.Drawing.Size(118, 17);
            this.lblChangePassword.TabIndex = 7;
            this.lblChangePassword.Text = "ChangePassword";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(308, 230);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(51, 17);
            this.label1.TabIndex = 8;
            this.label1.Text = "CDKey";
            // 
            // frmConnectionProperties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(970, 507);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblChangePassword);
            this.Controls.Add(this.lblLogOut);
            this.Controls.Add(this.lblLicensedTo);
            this.Controls.Add(this.lblCDKey);
            this.Controls.Add(this.lblServerClient);
            this.Controls.Add(this.lblUserName);
            this.Controls.Add(this.lblCompanyCode);
            this.Controls.Add(this.lblSqlServer);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmConnectionProperties";
            this.Text = "frmConnectionProperties";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblSqlServer;
        private System.Windows.Forms.Label lblCompanyCode;
        private System.Windows.Forms.Label lblUserName;
        private System.Windows.Forms.Label lblServerClient;
        private System.Windows.Forms.Label lblCDKey;
        private System.Windows.Forms.Label lblLicensedTo;
        private System.Windows.Forms.Label lblLogOut;
        private System.Windows.Forms.Label lblChangePassword;
        private System.Windows.Forms.Label label1;
    }
}