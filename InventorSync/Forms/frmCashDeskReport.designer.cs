
namespace InventorSync
{
    partial class frmCashDeskReport
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
            this.components = new System.ComponentModel.Container();
            this.toolTipColor = new System.Windows.Forms.ToolTip(this.components);
            this.lblHeading = new System.Windows.Forms.Label();
            this.tlpHeader = new System.Windows.Forms.TableLayoutPanel();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnMinimize = new System.Windows.Forms.Button();
            this.tlpMain = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblstaffIds = new System.Windows.Forms.Label();
            this.lblVoucherIds = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.chkVoucher = new System.Windows.Forms.CheckBox();
            this.txtVoucherTypeList = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tlpSaleStaff = new System.Windows.Forms.TableLayoutPanel();
            this.label4 = new System.Windows.Forms.Label();
            this.chkstaff = new System.Windows.Forms.CheckBox();
            this.txtstafflist = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.btnshow = new System.Windows.Forms.Button();
            this.lblToDate = new System.Windows.Forms.Label();
            this.dtpTD = new System.Windows.Forms.DateTimePicker();
            this.lblFromDate = new System.Windows.Forms.Label();
            this.dtpFD = new System.Windows.Forms.DateTimePicker();
            this.tlpHeader.SuspendLayout();
            this.tlpMain.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tlpSaleStaff.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblHeading
            // 
            this.lblHeading.AutoSize = true;
            this.lblHeading.BackColor = System.Drawing.Color.Transparent;
            this.lblHeading.Font = new System.Drawing.Font("Tahoma", 15.8F, System.Drawing.FontStyle.Bold);
            this.lblHeading.ForeColor = System.Drawing.Color.Black;
            this.lblHeading.Location = new System.Drawing.Point(198, 23);
            this.lblHeading.Margin = new System.Windows.Forms.Padding(27, 0, 27, 0);
            this.lblHeading.Name = "lblHeading";
            this.lblHeading.Size = new System.Drawing.Size(255, 33);
            this.lblHeading.TabIndex = 2;
            this.lblHeading.Text = "Cash Desk Report";
            this.lblHeading.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tlpHeader
            // 
            this.tlpHeader.BackColor = System.Drawing.Color.Transparent;
            this.tlpHeader.ColumnCount = 6;
            this.tlpHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tlpHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tlpHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 11F));
            this.tlpHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 90F));
            this.tlpHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 81F));
            this.tlpHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpHeader.Controls.Add(this.btnClose, 5, 0);
            this.tlpHeader.Controls.Add(this.btnMinimize, 4, 0);
            this.tlpHeader.Controls.Add(this.lblHeading, 3, 1);
            this.tlpHeader.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpHeader.Location = new System.Drawing.Point(0, 0);
            this.tlpHeader.Margin = new System.Windows.Forms.Padding(0);
            this.tlpHeader.Name = "tlpHeader";
            this.tlpHeader.RowCount = 3;
            this.tlpHeader.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 23F));
            this.tlpHeader.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 41F));
            this.tlpHeader.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 57F));
            this.tlpHeader.Size = new System.Drawing.Size(658, 99);
            this.tlpHeader.TabIndex = 0;
            // 
            // btnClose
            // 
            this.btnClose.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnClose.FlatAppearance.BorderSize = 0;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Image = global::DigiposZen.Properties.Resources.logout_Final;
            this.btnClose.Location = new System.Drawing.Point(581, 4);
            this.btnClose.Margin = new System.Windows.Forms.Padding(4);
            this.btnClose.Name = "btnClose";
            this.tlpHeader.SetRowSpan(this.btnClose, 2);
            this.btnClose.Size = new System.Drawing.Size(73, 56);
            this.btnClose.TabIndex = 6;
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnMinimize
            // 
            this.btnMinimize.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnMinimize.FlatAppearance.BorderSize = 0;
            this.btnMinimize.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMinimize.Image = global::DigiposZen.Properties.Resources.minimize_finalised;
            this.btnMinimize.Location = new System.Drawing.Point(491, 4);
            this.btnMinimize.Margin = new System.Windows.Forms.Padding(4);
            this.btnMinimize.Name = "btnMinimize";
            this.tlpHeader.SetRowSpan(this.btnMinimize, 2);
            this.btnMinimize.Size = new System.Drawing.Size(82, 56);
            this.btnMinimize.TabIndex = 6;
            this.btnMinimize.UseVisualStyleBackColor = true;
            this.btnMinimize.Click += new System.EventHandler(this.btnMinimize_Click);
            // 
            // tlpMain
            // 
            this.tlpMain.BackColor = System.Drawing.Color.WhiteSmoke;
            this.tlpMain.ColumnCount = 1;
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpMain.Controls.Add(this.tlpHeader, 0, 0);
            this.tlpMain.Controls.Add(this.panel1, 0, 1);
            this.tlpMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpMain.Location = new System.Drawing.Point(1, 1);
            this.tlpMain.Margin = new System.Windows.Forms.Padding(4);
            this.tlpMain.Name = "tlpMain";
            this.tlpMain.RowCount = 3;
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 99F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpMain.Size = new System.Drawing.Size(658, 540);
            this.tlpMain.TabIndex = 4;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.dtpFD);
            this.panel1.Controls.Add(this.lblstaffIds);
            this.panel1.Controls.Add(this.lblVoucherIds);
            this.panel1.Controls.Add(this.tableLayoutPanel1);
            this.panel1.Controls.Add(this.tlpSaleStaff);
            this.panel1.Controls.Add(this.btnshow);
            this.panel1.Controls.Add(this.lblToDate);
            this.panel1.Controls.Add(this.dtpTD);
            this.panel1.Controls.Add(this.lblFromDate);
            this.panel1.Location = new System.Drawing.Point(3, 102);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(652, 410);
            this.panel1.TabIndex = 1;
            // 
            // lblstaffIds
            // 
            this.lblstaffIds.AutoSize = true;
            this.lblstaffIds.Location = new System.Drawing.Point(512, 214);
            this.lblstaffIds.Name = "lblstaffIds";
            this.lblstaffIds.Size = new System.Drawing.Size(0, 17);
            this.lblstaffIds.TabIndex = 52;
            this.lblstaffIds.Visible = false;
            // 
            // lblVoucherIds
            // 
            this.lblVoucherIds.AutoSize = true;
            this.lblVoucherIds.Location = new System.Drawing.Point(513, 136);
            this.lblVoucherIds.Name = "lblVoucherIds";
            this.lblVoucherIds.Size = new System.Drawing.Size(0, 17);
            this.lblVoucherIds.TabIndex = 51;
            this.lblVoucherIds.Visible = false;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.BackColor = System.Drawing.Color.White;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 244F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 180F));
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.chkVoucher, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.txtVoucherTypeList, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label2, 1, 1);
            this.tableLayoutPanel1.Font = new System.Drawing.Font("Tahoma", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tableLayoutPanel1.Location = new System.Drawing.Point(136, 110);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 0F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(394, 70);
            this.tableLayoutPanel1.TabIndex = 50;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.White;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Font = new System.Drawing.Font("Segoe UI Semibold", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(238, 35);
            this.label1.TabIndex = 2;
            this.label1.Text = "Voucher Type";
            // 
            // chkVoucher
            // 
            this.chkVoucher.AutoSize = true;
            this.chkVoucher.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkVoucher.ForeColor = System.Drawing.Color.Black;
            this.chkVoucher.Location = new System.Drawing.Point(247, 3);
            this.chkVoucher.Name = "chkVoucher";
            this.chkVoucher.Size = new System.Drawing.Size(101, 27);
            this.chkVoucher.TabIndex = 4;
            this.chkVoucher.Text = "Select All";
            this.chkVoucher.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.chkVoucher.UseVisualStyleBackColor = true;
            this.chkVoucher.CheckedChanged += new System.EventHandler(this.chkVoucher_CheckedChanged);
            // 
            // txtVoucherTypeList
            // 
            this.txtVoucherTypeList.BackColor = System.Drawing.SystemColors.Window;
            this.txtVoucherTypeList.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tableLayoutPanel1.SetColumnSpan(this.txtVoucherTypeList, 2);
            this.txtVoucherTypeList.Font = new System.Drawing.Font("Tahoma", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtVoucherTypeList.Location = new System.Drawing.Point(3, 38);
            this.txtVoucherTypeList.MaxLength = 50;
            this.txtVoucherTypeList.Name = "txtVoucherTypeList";
            this.txtVoucherTypeList.Size = new System.Drawing.Size(388, 29);
            this.txtVoucherTypeList.TabIndex = 3;
            this.txtVoucherTypeList.Click += new System.EventHandler(this.txtVoucherTypeList_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(3, 70);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(0, 1);
            this.label2.TabIndex = 22;
            // 
            // tlpSaleStaff
            // 
            this.tlpSaleStaff.BackColor = System.Drawing.Color.White;
            this.tlpSaleStaff.ColumnCount = 2;
            this.tlpSaleStaff.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 244F));
            this.tlpSaleStaff.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 180F));
            this.tlpSaleStaff.Controls.Add(this.label4, 0, 0);
            this.tlpSaleStaff.Controls.Add(this.chkstaff, 1, 0);
            this.tlpSaleStaff.Controls.Add(this.txtstafflist, 0, 1);
            this.tlpSaleStaff.Controls.Add(this.label5, 1, 1);
            this.tlpSaleStaff.Font = new System.Drawing.Font("Tahoma", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tlpSaleStaff.Location = new System.Drawing.Point(135, 188);
            this.tlpSaleStaff.Margin = new System.Windows.Forms.Padding(0);
            this.tlpSaleStaff.Name = "tlpSaleStaff";
            this.tlpSaleStaff.RowCount = 1;
            this.tlpSaleStaff.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpSaleStaff.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpSaleStaff.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 0F));
            this.tlpSaleStaff.Size = new System.Drawing.Size(394, 70);
            this.tlpSaleStaff.TabIndex = 49;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.White;
            this.label4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label4.Font = new System.Drawing.Font("Segoe UI Semibold", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.Black;
            this.label4.Location = new System.Drawing.Point(3, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(238, 35);
            this.label4.TabIndex = 2;
            this.label4.Text = "Sale Staff";
            // 
            // chkstaff
            // 
            this.chkstaff.AutoSize = true;
            this.chkstaff.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkstaff.ForeColor = System.Drawing.Color.Black;
            this.chkstaff.Location = new System.Drawing.Point(247, 3);
            this.chkstaff.Name = "chkstaff";
            this.chkstaff.Size = new System.Drawing.Size(101, 27);
            this.chkstaff.TabIndex = 4;
            this.chkstaff.Text = "Select All";
            this.chkstaff.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.chkstaff.UseVisualStyleBackColor = true;
            this.chkstaff.CheckedChanged += new System.EventHandler(this.chkstaff_CheckedChanged);
            // 
            // txtstafflist
            // 
            this.txtstafflist.BackColor = System.Drawing.SystemColors.Window;
            this.txtstafflist.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tlpSaleStaff.SetColumnSpan(this.txtstafflist, 2);
            this.txtstafflist.Font = new System.Drawing.Font("Tahoma", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtstafflist.Location = new System.Drawing.Point(3, 38);
            this.txtstafflist.MaxLength = 50;
            this.txtstafflist.Name = "txtstafflist";
            this.txtstafflist.Size = new System.Drawing.Size(388, 29);
            this.txtstafflist.TabIndex = 3;
            this.txtstafflist.Click += new System.EventHandler(this.txtstafflist_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Tahoma", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(3, 70);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(0, 1);
            this.label5.TabIndex = 22;
            // 
            // btnshow
            // 
            this.btnshow.BackColor = System.Drawing.Color.Transparent;
            this.btnshow.ForeColor = System.Drawing.Color.Black;
            this.btnshow.Location = new System.Drawing.Point(391, 320);
            this.btnshow.Name = "btnshow";
            this.btnshow.Size = new System.Drawing.Size(108, 40);
            this.btnshow.TabIndex = 14;
            this.btnshow.Text = "SHOW";
            this.btnshow.UseVisualStyleBackColor = false;
            this.btnshow.Click += new System.EventHandler(this.btnshow_Click);
            // 
            // lblToDate
            // 
            this.lblToDate.AutoSize = true;
            this.lblToDate.BackColor = System.Drawing.Color.Transparent;
            this.lblToDate.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.lblToDate.ForeColor = System.Drawing.Color.Black;
            this.lblToDate.Location = new System.Drawing.Point(189, 322);
            this.lblToDate.Name = "lblToDate";
            this.lblToDate.Size = new System.Drawing.Size(87, 28);
            this.lblToDate.TabIndex = 11;
            this.lblToDate.Text = "To Date :";
            this.lblToDate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // dtpTD
            // 
            this.dtpTD.CustomFormat = "dd/MMM/yyyy";
            this.dtpTD.Font = new System.Drawing.Font("Tahoma", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtpTD.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpTD.Location = new System.Drawing.Point(194, 353);
            this.dtpTD.Name = "dtpTD";
            this.dtpTD.Size = new System.Drawing.Size(174, 29);
            this.dtpTD.TabIndex = 12;
            // 
            // lblFromDate
            // 
            this.lblFromDate.AutoSize = true;
            this.lblFromDate.BackColor = System.Drawing.Color.Transparent;
            this.lblFromDate.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.lblFromDate.ForeColor = System.Drawing.SystemColors.WindowText;
            this.lblFromDate.Location = new System.Drawing.Point(189, 259);
            this.lblFromDate.Name = "lblFromDate";
            this.lblFromDate.Size = new System.Drawing.Size(113, 28);
            this.lblFromDate.TabIndex = 10;
            this.lblFromDate.Text = "From Date :";
            this.lblFromDate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // dtpFD
            // 
            this.dtpFD.CustomFormat = "dd/MMM/yyyy";
            this.dtpFD.Font = new System.Drawing.Font("Tahoma", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtpFD.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpFD.Location = new System.Drawing.Point(194, 290);
            this.dtpFD.Name = "dtpFD";
            this.dtpFD.Size = new System.Drawing.Size(174, 29);
            this.dtpFD.TabIndex = 53;
            // 
            // frmCashDeskReport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(660, 542);
            this.Controls.Add(this.tlpMain);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "frmCashDeskReport";
            this.Padding = new System.Windows.Forms.Padding(1);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Color Master ";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.frmCashDeskReport_Load);
            this.tlpHeader.ResumeLayout(false);
            this.tlpHeader.PerformLayout();
            this.tlpMain.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tlpSaleStaff.ResumeLayout(false);
            this.tlpSaleStaff.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ToolTip toolTipColor;
        private System.Windows.Forms.TableLayoutPanel tlpHeader;
        private System.Windows.Forms.Label lblHeading;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnMinimize;
        private System.Windows.Forms.TableLayoutPanel tlpMain;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblToDate;
        private System.Windows.Forms.DateTimePicker dtpTD;
        private System.Windows.Forms.Label lblFromDate;
        private System.Windows.Forms.Button btnshow;
        private System.Windows.Forms.TableLayoutPanel tlpSaleStaff;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox chkstaff;
        private System.Windows.Forms.TextBox txtstafflist;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox chkVoucher;
        private System.Windows.Forms.TextBox txtVoucherTypeList;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblVoucherIds;
        private System.Windows.Forms.Label lblstaffIds;
        private System.Windows.Forms.DateTimePicker dtpFD;
    }
}