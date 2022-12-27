
namespace DigiposZen.Forms
{
    partial class frmActiveMonitor
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
            this.tlpMain = new System.Windows.Forms.TableLayoutPanel();
            this.tlpHeader = new System.Windows.Forms.TableLayoutPanel();
            this.lblHeading = new System.Windows.Forms.Label();
            this.btnMinimize = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.lblFooter = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.chkSystemName = new System.Windows.Forms.CheckBox();
            this.chkWindowsName = new System.Windows.Forms.CheckBox();
            this.chkUser = new System.Windows.Forms.CheckBox();
            this.chkAction = new System.Windows.Forms.CheckBox();
            this.cblSystemName = new System.Windows.Forms.CheckedListBox();
            this.cblWindowsName = new System.Windows.Forms.CheckedListBox();
            this.cbluser = new System.Windows.Forms.CheckedListBox();
            this.cblaction = new System.Windows.Forms.CheckedListBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.txtDataLoadNew = new System.Windows.Forms.TextBox();
            this.txtDataLoadOld = new System.Windows.Forms.TextBox();
            this.lblToDate = new System.Windows.Forms.Label();
            this.dtpTD = new System.Windows.Forms.DateTimePicker();
            this.lblFromDate = new System.Windows.Forms.Label();
            this.dtpFD = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.lblUser = new System.Windows.Forms.Label();
            this.lblWindowsName = new System.Windows.Forms.Label();
            this.lblAction = new System.Windows.Forms.Label();
            this.btnShow = new System.Windows.Forms.Button();
            this.DgvLoadData = new System.Windows.Forms.DataGridView();
            this.tlpMain.SuspendLayout();
            this.tlpHeader.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DgvLoadData)).BeginInit();
            this.SuspendLayout();
            // 
            // tlpMain
            // 
            this.tlpMain.BackColor = System.Drawing.Color.White;
            this.tlpMain.ColumnCount = 1;
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 1214F));
            this.tlpMain.Controls.Add(this.tlpHeader, 0, 0);
            this.tlpMain.Controls.Add(this.lblFooter, 0, 2);
            this.tlpMain.Controls.Add(this.panel1, 0, 1);
            this.tlpMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpMain.Location = new System.Drawing.Point(1, 1);
            this.tlpMain.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tlpMain.Name = "tlpMain";
            this.tlpMain.RowCount = 3;
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 78F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpMain.Size = new System.Drawing.Size(1212, 575);
            this.tlpMain.TabIndex = 2;
            // 
            // tlpHeader
            // 
            this.tlpHeader.BackColor = System.Drawing.Color.White;
            this.tlpHeader.ColumnCount = 6;
            this.tlpHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tlpHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tlpHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tlpHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tlpHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 65F));
            this.tlpHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 69F));
            this.tlpHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpHeader.Controls.Add(this.lblHeading, 0, 0);
            this.tlpHeader.Controls.Add(this.btnMinimize, 4, 0);
            this.tlpHeader.Controls.Add(this.btnClose, 5, 0);
            this.tlpHeader.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpHeader.Location = new System.Drawing.Point(0, 0);
            this.tlpHeader.Margin = new System.Windows.Forms.Padding(0);
            this.tlpHeader.Name = "tlpHeader";
            this.tlpHeader.RowCount = 2;
            this.tlpHeader.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 51F));
            this.tlpHeader.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 39F));
            this.tlpHeader.Size = new System.Drawing.Size(1214, 78);
            this.tlpHeader.TabIndex = 0;
            // 
            // lblHeading
            // 
            this.lblHeading.AutoSize = true;
            this.lblHeading.BackColor = System.Drawing.Color.Transparent;
            this.tlpHeader.SetColumnSpan(this.lblHeading, 4);
            this.lblHeading.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblHeading.Font = new System.Drawing.Font("Tahoma", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHeading.ForeColor = System.Drawing.Color.Black;
            this.lblHeading.Location = new System.Drawing.Point(18, 0);
            this.lblHeading.Margin = new System.Windows.Forms.Padding(18, 0, 18, 0);
            this.lblHeading.Name = "lblHeading";
            this.lblHeading.Size = new System.Drawing.Size(1044, 51);
            this.lblHeading.TabIndex = 2;
            this.lblHeading.Text = "Active Monitor";
            this.lblHeading.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnMinimize
            // 
            this.btnMinimize.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnMinimize.FlatAppearance.BorderSize = 0;
            this.btnMinimize.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMinimize.Image = global::DigiposZen.Properties.Resources.minimize_finalised;
            this.btnMinimize.Location = new System.Drawing.Point(1083, 2);
            this.btnMinimize.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnMinimize.Name = "btnMinimize";
            this.btnMinimize.Size = new System.Drawing.Size(59, 47);
            this.btnMinimize.TabIndex = 6;
            this.btnMinimize.UseVisualStyleBackColor = true;
            this.btnMinimize.Click += new System.EventHandler(this.btnMinimize_Click);
            // 
            // btnClose
            // 
            this.btnClose.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnClose.FlatAppearance.BorderSize = 0;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Image = global::DigiposZen.Properties.Resources.logout_Final;
            this.btnClose.Location = new System.Drawing.Point(1148, 2);
            this.btnClose.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(63, 47);
            this.btnClose.TabIndex = 6;
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // lblFooter
            // 
            this.lblFooter.BackColor = System.Drawing.Color.White;
            this.lblFooter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblFooter.ForeColor = System.Drawing.Color.Black;
            this.lblFooter.Location = new System.Drawing.Point(3, 555);
            this.lblFooter.Name = "lblFooter";
            this.lblFooter.Size = new System.Drawing.Size(1208, 20);
            this.lblFooter.TabIndex = 4;
            this.lblFooter.Text = "Keyboard Shortcuts : - F5 Execute, F6 Export CSV";
            this.lblFooter.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.chkSystemName);
            this.panel1.Controls.Add(this.chkWindowsName);
            this.panel1.Controls.Add(this.chkUser);
            this.panel1.Controls.Add(this.chkAction);
            this.panel1.Controls.Add(this.cblSystemName);
            this.panel1.Controls.Add(this.cblWindowsName);
            this.panel1.Controls.Add(this.cbluser);
            this.panel1.Controls.Add(this.cblaction);
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Controls.Add(this.lblToDate);
            this.panel1.Controls.Add(this.dtpTD);
            this.panel1.Controls.Add(this.lblFromDate);
            this.panel1.Controls.Add(this.dtpFD);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.lblUser);
            this.panel1.Controls.Add(this.lblWindowsName);
            this.panel1.Controls.Add(this.lblAction);
            this.panel1.Controls.Add(this.btnShow);
            this.panel1.Controls.Add(this.DgvLoadData);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 81);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1208, 471);
            this.panel1.TabIndex = 5;
            // 
            // chkSystemName
            // 
            this.chkSystemName.AutoSize = true;
            this.chkSystemName.Location = new System.Drawing.Point(151, 220);
            this.chkSystemName.Name = "chkSystemName";
            this.chkSystemName.Size = new System.Drawing.Size(18, 17);
            this.chkSystemName.TabIndex = 26;
            this.chkSystemName.UseVisualStyleBackColor = true;
            this.chkSystemName.CheckedChanged += new System.EventHandler(this.chkSystemName_CheckedChanged);
            // 
            // chkWindowsName
            // 
            this.chkWindowsName.AutoSize = true;
            this.chkWindowsName.Location = new System.Drawing.Point(151, 153);
            this.chkWindowsName.Name = "chkWindowsName";
            this.chkWindowsName.Size = new System.Drawing.Size(18, 17);
            this.chkWindowsName.TabIndex = 27;
            this.chkWindowsName.UseVisualStyleBackColor = true;
            this.chkWindowsName.CheckedChanged += new System.EventHandler(this.chkWindowsName_CheckedChanged);
            // 
            // chkUser
            // 
            this.chkUser.AutoSize = true;
            this.chkUser.Location = new System.Drawing.Point(151, 86);
            this.chkUser.Name = "chkUser";
            this.chkUser.Size = new System.Drawing.Size(18, 17);
            this.chkUser.TabIndex = 26;
            this.chkUser.UseVisualStyleBackColor = true;
            this.chkUser.CheckedChanged += new System.EventHandler(this.chkUser_CheckedChanged);
            // 
            // chkAction
            // 
            this.chkAction.AutoSize = true;
            this.chkAction.Location = new System.Drawing.Point(151, 17);
            this.chkAction.Name = "chkAction";
            this.chkAction.Size = new System.Drawing.Size(18, 17);
            this.chkAction.TabIndex = 25;
            this.chkAction.UseVisualStyleBackColor = true;
            this.chkAction.CheckedChanged += new System.EventHandler(this.chkAction_CheckedChanged);
            // 
            // cblSystemName
            // 
            this.cblSystemName.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cblSystemName.FormattingEnabled = true;
            this.cblSystemName.Location = new System.Drawing.Point(11, 236);
            this.cblSystemName.Name = "cblSystemName";
            this.cblSystemName.Size = new System.Drawing.Size(158, 46);
            this.cblSystemName.TabIndex = 24;
            // 
            // cblWindowsName
            // 
            this.cblWindowsName.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cblWindowsName.FormattingEnabled = true;
            this.cblWindowsName.Location = new System.Drawing.Point(11, 169);
            this.cblWindowsName.Name = "cblWindowsName";
            this.cblWindowsName.Size = new System.Drawing.Size(158, 46);
            this.cblWindowsName.TabIndex = 23;
            // 
            // cbluser
            // 
            this.cbluser.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbluser.FormattingEnabled = true;
            this.cbluser.Location = new System.Drawing.Point(11, 101);
            this.cbluser.Name = "cbluser";
            this.cbluser.Size = new System.Drawing.Size(158, 46);
            this.cbluser.TabIndex = 22;
            // 
            // cblaction
            // 
            this.cblaction.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cblaction.FormattingEnabled = true;
            this.cblaction.Location = new System.Drawing.Point(11, 33);
            this.cblaction.Name = "cblaction";
            this.cblaction.Size = new System.Drawing.Size(158, 46);
            this.cblaction.TabIndex = 21;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.txtDataLoadNew);
            this.panel2.Controls.Add(this.txtDataLoadOld);
            this.panel2.Location = new System.Drawing.Point(193, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1006, 456);
            this.panel2.TabIndex = 20;
            // 
            // txtDataLoadNew
            // 
            this.txtDataLoadNew.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtDataLoadNew.Location = new System.Drawing.Point(503, 0);
            this.txtDataLoadNew.Multiline = true;
            this.txtDataLoadNew.Name = "txtDataLoadNew";
            this.txtDataLoadNew.ReadOnly = true;
            this.txtDataLoadNew.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtDataLoadNew.Size = new System.Drawing.Size(500, 462);
            this.txtDataLoadNew.TabIndex = 1;
            this.txtDataLoadNew.DoubleClick += new System.EventHandler(this.txtDataLoadNew_DoubleClick);
            // 
            // txtDataLoadOld
            // 
            this.txtDataLoadOld.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtDataLoadOld.Location = new System.Drawing.Point(0, 0);
            this.txtDataLoadOld.Multiline = true;
            this.txtDataLoadOld.Name = "txtDataLoadOld";
            this.txtDataLoadOld.ReadOnly = true;
            this.txtDataLoadOld.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtDataLoadOld.Size = new System.Drawing.Size(497, 453);
            this.txtDataLoadOld.TabIndex = 0;
            this.txtDataLoadOld.DoubleClick += new System.EventHandler(this.txtDataLoad_DoubleClick);
            // 
            // lblToDate
            // 
            this.lblToDate.AutoSize = true;
            this.lblToDate.BackColor = System.Drawing.Color.Transparent;
            this.lblToDate.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblToDate.ForeColor = System.Drawing.Color.Black;
            this.lblToDate.Location = new System.Drawing.Point(11, 349);
            this.lblToDate.Name = "lblToDate";
            this.lblToDate.Size = new System.Drawing.Size(72, 18);
            this.lblToDate.TabIndex = 17;
            this.lblToDate.Text = "To Date :";
            this.lblToDate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // dtpTD
            // 
            this.dtpTD.CustomFormat = "dd/MMM/yyyy";
            this.dtpTD.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtpTD.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpTD.Location = new System.Drawing.Point(12, 373);
            this.dtpTD.Name = "dtpTD";
            this.dtpTD.Size = new System.Drawing.Size(158, 26);
            this.dtpTD.TabIndex = 18;
            // 
            // lblFromDate
            // 
            this.lblFromDate.AutoSize = true;
            this.lblFromDate.BackColor = System.Drawing.Color.Transparent;
            this.lblFromDate.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFromDate.ForeColor = System.Drawing.SystemColors.WindowText;
            this.lblFromDate.Location = new System.Drawing.Point(11, 285);
            this.lblFromDate.Name = "lblFromDate";
            this.lblFromDate.Size = new System.Drawing.Size(88, 18);
            this.lblFromDate.TabIndex = 16;
            this.lblFromDate.Text = "From Date :";
            this.lblFromDate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // dtpFD
            // 
            this.dtpFD.CustomFormat = "dd/MMM/yyyy";
            this.dtpFD.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtpFD.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpFD.Location = new System.Drawing.Point(12, 309);
            this.dtpFD.Name = "dtpFD";
            this.dtpFD.Size = new System.Drawing.Size(158, 26);
            this.dtpFD.TabIndex = 19;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(11, 215);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(101, 18);
            this.label1.TabIndex = 15;
            this.label1.Text = "System Name";
            // 
            // lblUser
            // 
            this.lblUser.AutoSize = true;
            this.lblUser.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUser.Location = new System.Drawing.Point(11, 81);
            this.lblUser.Name = "lblUser";
            this.lblUser.Size = new System.Drawing.Size(38, 18);
            this.lblUser.TabIndex = 13;
            this.lblUser.Text = "User";
            // 
            // lblWindowsName
            // 
            this.lblWindowsName.AutoSize = true;
            this.lblWindowsName.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblWindowsName.Location = new System.Drawing.Point(11, 148);
            this.lblWindowsName.Name = "lblWindowsName";
            this.lblWindowsName.Size = new System.Drawing.Size(109, 18);
            this.lblWindowsName.TabIndex = 11;
            this.lblWindowsName.Text = "Windows Name";
            // 
            // lblAction
            // 
            this.lblAction.AutoSize = true;
            this.lblAction.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAction.Location = new System.Drawing.Point(11, 12);
            this.lblAction.Name = "lblAction";
            this.lblAction.Size = new System.Drawing.Size(47, 18);
            this.lblAction.TabIndex = 9;
            this.lblAction.Text = "Action";
            // 
            // btnShow
            // 
            this.btnShow.FlatAppearance.BorderSize = 0;
            this.btnShow.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnShow.Image = global::DigiposZen.Properties.Resources.Find14040;
            this.btnShow.Location = new System.Drawing.Point(131, 417);
            this.btnShow.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnShow.Name = "btnShow";
            this.btnShow.Size = new System.Drawing.Size(38, 39);
            this.btnShow.TabIndex = 7;
            this.btnShow.UseVisualStyleBackColor = true;
            this.btnShow.Click += new System.EventHandler(this.btnShow_Click);
            // 
            // DgvLoadData
            // 
            this.DgvLoadData.AllowUserToAddRows = false;
            this.DgvLoadData.AllowUserToDeleteRows = false;
            this.DgvLoadData.BackgroundColor = System.Drawing.Color.White;
            this.DgvLoadData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DgvLoadData.Location = new System.Drawing.Point(193, 0);
            this.DgvLoadData.Name = "DgvLoadData";
            this.DgvLoadData.ReadOnly = true;
            this.DgvLoadData.RowHeadersWidth = 51;
            this.DgvLoadData.RowTemplate.Height = 24;
            this.DgvLoadData.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.DgvLoadData.Size = new System.Drawing.Size(1006, 465);
            this.DgvLoadData.TabIndex = 1;
            this.DgvLoadData.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DgvLoadData_CellClick);
            // 
            // frmActiveMonitor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(1214, 577);
            this.Controls.Add(this.tlpMain);
            this.Font = new System.Drawing.Font("Tahoma", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmActiveMonitor";
            this.Padding = new System.Windows.Forms.Padding(1);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmCommadWindow";
            this.Load += new System.EventHandler(this.frmActiveMonitor_Load);
            this.tlpMain.ResumeLayout(false);
            this.tlpHeader.ResumeLayout(false);
            this.tlpHeader.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DgvLoadData)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlpMain;
        private System.Windows.Forms.TableLayoutPanel tlpHeader;
        private System.Windows.Forms.Label lblHeading;
        private System.Windows.Forms.Button btnMinimize;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label lblFooter;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.DataGridView DgvLoadData;
        private System.Windows.Forms.Button btnShow;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblUser;
        private System.Windows.Forms.Label lblWindowsName;
        private System.Windows.Forms.Label lblAction;
        private System.Windows.Forms.Label lblToDate;
        private System.Windows.Forms.DateTimePicker dtpTD;
        private System.Windows.Forms.Label lblFromDate;
        private System.Windows.Forms.DateTimePicker dtpFD;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TextBox txtDataLoadOld;
        private System.Windows.Forms.TextBox txtDataLoadNew;
        private System.Windows.Forms.CheckedListBox cblaction;
        private System.Windows.Forms.CheckedListBox cblSystemName;
        private System.Windows.Forms.CheckedListBox cblWindowsName;
        private System.Windows.Forms.CheckedListBox cbluser;
        private System.Windows.Forms.CheckBox chkSystemName;
        private System.Windows.Forms.CheckBox chkWindowsName;
        private System.Windows.Forms.CheckBox chkUser;
        private System.Windows.Forms.CheckBox chkAction;
    }
}