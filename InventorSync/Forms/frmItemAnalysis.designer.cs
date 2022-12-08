
namespace InventorSync
{
    partial class frmItemAnalysis
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmItemAnalysis));
            this.tblpForms = new System.Windows.Forms.TableLayoutPanel();
            this.dgvStockHistory = new System.Windows.Forms.DataGridView();
            this.tlpMenuButton = new System.Windows.Forms.TableLayoutPanel();
            this.rdoStockHistory = new System.Windows.Forms.RadioButton();
            this.rdoStockReport = new System.Windows.Forms.RadioButton();
            this.tlpHeading = new System.Windows.Forms.TableLayoutPanel();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnMinimize = new System.Windows.Forms.Button();
            this.lblHeading = new System.Windows.Forms.Label();
            this.tlpMain = new System.Windows.Forms.TableLayoutPanel();
            this.lblFooter = new System.Windows.Forms.Label();
            this.tlpSearch = new System.Windows.Forms.TableLayoutPanel();
            this.dtpToDate = new System.Windows.Forms.DateTimePicker();
            this.lblToDate = new System.Windows.Forms.Label();
            this.lblFromDate = new System.Windows.Forms.Label();
            this.lblBatchUnique = new System.Windows.Forms.Label();
            this.lblItemName = new System.Windows.Forms.Label();
            this.txtSearchItem = new System.Windows.Forms.TextBox();
            this.lblVoucherType = new System.Windows.Forms.Label();
            this.lblCostCentre = new System.Windows.Forms.Label();
            this.dtpFromDate = new System.Windows.Forms.DateTimePicker();
            this.cboVoucherType = new System.Windows.Forms.ComboBox();
            this.cboBatchUnique = new System.Windows.Forms.ComboBox();
            this.cboCostCentre = new System.Windows.Forms.ComboBox();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.lblRefresh = new System.Windows.Forms.Label();
            this.lblRptHeading = new System.Windows.Forms.Label();
            this.lblItemID = new System.Windows.Forms.Label();
            this.tlpBottom = new System.Windows.Forms.TableLayoutPanel();
            this.flpQty = new System.Windows.Forms.FlowLayoutPanel();
            this.tlpQOH = new System.Windows.Forms.TableLayoutPanel();
            this.lblQOHAmt = new System.Windows.Forms.Label();
            this.lblQOH = new System.Windows.Forms.Label();
            this.tlpQtyOut = new System.Windows.Forms.TableLayoutPanel();
            this.lblQtyOutAmt = new System.Windows.Forms.Label();
            this.lblQtyOut = new System.Windows.Forms.Label();
            this.tlpQtyIn = new System.Windows.Forms.TableLayoutPanel();
            this.lblQtyIn = new System.Windows.Forms.Label();
            this.lblQtyInAmt = new System.Windows.Forms.Label();
            this.tblpForms.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStockHistory)).BeginInit();
            this.tlpMenuButton.SuspendLayout();
            this.tlpHeading.SuspendLayout();
            this.tlpMain.SuspendLayout();
            this.tlpSearch.SuspendLayout();
            this.tlpBottom.SuspendLayout();
            this.flpQty.SuspendLayout();
            this.tlpQOH.SuspendLayout();
            this.tlpQtyOut.SuspendLayout();
            this.tlpQtyIn.SuspendLayout();
            this.SuspendLayout();
            // 
            // tblpForms
            // 
            this.tblpForms.ColumnCount = 1;
            this.tblpForms.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 1160F));
            this.tblpForms.Controls.Add(this.dgvStockHistory, 0, 0);
            this.tblpForms.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblpForms.Location = new System.Drawing.Point(188, 161);
            this.tblpForms.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tblpForms.Name = "tblpForms";
            this.tblpForms.RowCount = 1;
            this.tblpForms.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblpForms.Size = new System.Drawing.Size(1134, 546);
            this.tblpForms.TabIndex = 10;
            // 
            // dgvStockHistory
            // 
            this.dgvStockHistory.AllowUserToAddRows = false;
            this.dgvStockHistory.BackgroundColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.LightYellow;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvStockHistory.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvStockHistory.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvStockHistory.Location = new System.Drawing.Point(3, 2);
            this.dgvStockHistory.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.dgvStockHistory.Name = "dgvStockHistory";
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Tahoma", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.LightYellow;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvStockHistory.RowHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvStockHistory.RowHeadersVisible = false;
            this.dgvStockHistory.RowHeadersWidth = 51;
            this.dgvStockHistory.RowTemplate.Height = 24;
            this.dgvStockHistory.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvStockHistory.Size = new System.Drawing.Size(1124, 537);
            this.dgvStockHistory.TabIndex = 0;
            // 
            // tlpMenuButton
            // 
            this.tlpMenuButton.BackColor = System.Drawing.Color.Transparent;
            this.tlpMenuButton.ColumnCount = 1;
            this.tlpMenuButton.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpMenuButton.Controls.Add(this.rdoStockHistory, 0, 0);
            this.tlpMenuButton.Controls.Add(this.rdoStockReport, 0, 1);
            this.tlpMenuButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpMenuButton.Location = new System.Drawing.Point(3, 161);
            this.tlpMenuButton.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tlpMenuButton.Name = "tlpMenuButton";
            this.tlpMenuButton.RowCount = 3;
            this.tlpMenuButton.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tlpMenuButton.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tlpMenuButton.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpMenuButton.Size = new System.Drawing.Size(179, 546);
            this.tlpMenuButton.TabIndex = 1;
            // 
            // rdoStockHistory
            // 
            this.rdoStockHistory.Appearance = System.Windows.Forms.Appearance.Button;
            this.rdoStockHistory.AutoSize = true;
            this.rdoStockHistory.BackColor = System.Drawing.Color.Transparent;
            this.rdoStockHistory.Checked = true;
            this.rdoStockHistory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rdoStockHistory.FlatAppearance.BorderSize = 0;
            this.rdoStockHistory.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rdoStockHistory.ForeColor = System.Drawing.Color.Black;
            this.rdoStockHistory.Image = ((System.Drawing.Image)(resources.GetObject("rdoStockHistory.Image")));
            this.rdoStockHistory.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.rdoStockHistory.Location = new System.Drawing.Point(3, 2);
            this.rdoStockHistory.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.rdoStockHistory.Name = "rdoStockHistory";
            this.rdoStockHistory.Size = new System.Drawing.Size(173, 46);
            this.rdoStockHistory.TabIndex = 9;
            this.rdoStockHistory.TabStop = true;
            this.rdoStockHistory.Text = "    Stock History";
            this.rdoStockHistory.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rdoStockHistory.UseVisualStyleBackColor = false;
            this.rdoStockHistory.Click += new System.EventHandler(this.rdoStockHistory_Click);
            // 
            // rdoStockReport
            // 
            this.rdoStockReport.Appearance = System.Windows.Forms.Appearance.Button;
            this.rdoStockReport.AutoSize = true;
            this.rdoStockReport.BackColor = System.Drawing.Color.Transparent;
            this.rdoStockReport.Checked = true;
            this.rdoStockReport.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rdoStockReport.FlatAppearance.BorderSize = 0;
            this.rdoStockReport.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rdoStockReport.ForeColor = System.Drawing.Color.Black;
            this.rdoStockReport.Image = ((System.Drawing.Image)(resources.GetObject("rdoStockReport.Image")));
            this.rdoStockReport.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.rdoStockReport.Location = new System.Drawing.Point(3, 52);
            this.rdoStockReport.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.rdoStockReport.Name = "rdoStockReport";
            this.rdoStockReport.Size = new System.Drawing.Size(173, 46);
            this.rdoStockReport.TabIndex = 10;
            this.rdoStockReport.TabStop = true;
            this.rdoStockReport.Text = "    Stock Report";
            this.rdoStockReport.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rdoStockReport.UseVisualStyleBackColor = false;
            this.rdoStockReport.Visible = false;
            this.rdoStockReport.Click += new System.EventHandler(this.rdoStockReport_Click);
            // 
            // tlpHeading
            // 
            this.tlpHeading.BackColor = System.Drawing.Color.Transparent;
            this.tlpHeading.ColumnCount = 10;
            this.tlpMain.SetColumnSpan(this.tlpHeading, 2);
            this.tlpHeading.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tlpHeading.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tlpHeading.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tlpHeading.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 125F));
            this.tlpHeading.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 140F));
            this.tlpHeading.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 125F));
            this.tlpHeading.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpHeading.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tlpHeading.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tlpHeading.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tlpHeading.Controls.Add(this.btnClose, 9, 0);
            this.tlpHeading.Controls.Add(this.btnMinimize, 8, 0);
            this.tlpHeading.Controls.Add(this.lblHeading, 2, 0);
            this.tlpHeading.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpHeading.Location = new System.Drawing.Point(0, 0);
            this.tlpHeading.Margin = new System.Windows.Forms.Padding(0);
            this.tlpHeading.Name = "tlpHeading";
            this.tlpHeading.RowCount = 1;
            this.tlpHeading.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 69F));
            this.tlpHeading.Size = new System.Drawing.Size(1325, 64);
            this.tlpHeading.TabIndex = 0;
            this.tlpHeading.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tlpHeading_MouseDown);
            this.tlpHeading.MouseMove += new System.Windows.Forms.MouseEventHandler(this.tlpHeading_MouseMove);
            this.tlpHeading.MouseUp += new System.Windows.Forms.MouseEventHandler(this.tlpHeading_MouseUp);
            // 
            // btnClose
            // 
            this.btnClose.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnClose.FlatAppearance.BorderSize = 0;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Image = global::DigiposZen.Properties.Resources.logout_Final;
            this.btnClose.Location = new System.Drawing.Point(1228, 2);
            this.btnClose.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(94, 65);
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
            this.btnMinimize.Location = new System.Drawing.Point(1128, 2);
            this.btnMinimize.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnMinimize.Name = "btnMinimize";
            this.btnMinimize.Size = new System.Drawing.Size(94, 65);
            this.btnMinimize.TabIndex = 7;
            this.btnMinimize.UseVisualStyleBackColor = true;
            this.btnMinimize.Click += new System.EventHandler(this.btnMinimize_Click);
            // 
            // lblHeading
            // 
            this.lblHeading.AutoSize = true;
            this.lblHeading.BackColor = System.Drawing.Color.Transparent;
            this.tlpHeading.SetColumnSpan(this.lblHeading, 5);
            this.lblHeading.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblHeading.Font = new System.Drawing.Font("Microsoft Tai Le", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHeading.ForeColor = System.Drawing.Color.Black;
            this.lblHeading.Location = new System.Drawing.Point(220, 0);
            this.lblHeading.Margin = new System.Windows.Forms.Padding(20, 0, 20, 0);
            this.lblHeading.Name = "lblHeading";
            this.lblHeading.Size = new System.Drawing.Size(785, 69);
            this.lblHeading.TabIndex = 2;
            this.lblHeading.Text = "Item Analysis";
            this.lblHeading.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblHeading.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lblHeading_MouseDown);
            this.lblHeading.MouseMove += new System.Windows.Forms.MouseEventHandler(this.lblHeading_MouseMove);
            this.lblHeading.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lblHeading_MouseUp);
            // 
            // tlpMain
            // 
            this.tlpMain.BackColor = System.Drawing.Color.WhiteSmoke;
            this.tlpMain.ColumnCount = 2;
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 185F));
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpMain.Controls.Add(this.lblFooter, 0, 4);
            this.tlpMain.Controls.Add(this.tlpHeading, 0, 0);
            this.tlpMain.Controls.Add(this.tlpMenuButton, 0, 2);
            this.tlpMain.Controls.Add(this.tblpForms, 1, 2);
            this.tlpMain.Controls.Add(this.tlpSearch, 0, 1);
            this.tlpMain.Controls.Add(this.tlpBottom, 1, 3);
            this.tlpMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpMain.Location = new System.Drawing.Point(1, 1);
            this.tlpMain.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tlpMain.Name = "tlpMain";
            this.tlpMain.RowCount = 5;
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 64F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 95F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 550F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 90F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpMain.Size = new System.Drawing.Size(1325, 765);
            this.tlpMain.TabIndex = 2;
            // 
            // lblFooter
            // 
            this.lblFooter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.tlpMain.SetColumnSpan(this.lblFooter, 2);
            this.lblFooter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblFooter.ForeColor = System.Drawing.Color.Silver;
            this.lblFooter.Location = new System.Drawing.Point(3, 799);
            this.lblFooter.Name = "lblFooter";
            this.lblFooter.Size = new System.Drawing.Size(1319, 20);
            this.lblFooter.TabIndex = 13;
            this.lblFooter.Text = "Keyboard Shortcuts : -  F5 Refresh,Esc Close";
            this.lblFooter.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tlpSearch
            // 
            this.tlpSearch.BackColor = System.Drawing.Color.Transparent;
            this.tlpSearch.ColumnCount = 7;
            this.tlpMain.SetColumnSpan(this.tlpSearch, 5);
            this.tlpSearch.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 251F));
            this.tlpSearch.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 220F));
            this.tlpSearch.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 260F));
            this.tlpSearch.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 180F));
            this.tlpSearch.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 149F));
            this.tlpSearch.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 149F));
            this.tlpSearch.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 109F));
            this.tlpSearch.Controls.Add(this.dtpToDate, 5, 1);
            this.tlpSearch.Controls.Add(this.lblToDate, 5, 0);
            this.tlpSearch.Controls.Add(this.lblFromDate, 4, 0);
            this.tlpSearch.Controls.Add(this.lblBatchUnique, 2, 0);
            this.tlpSearch.Controls.Add(this.lblItemName, 0, 0);
            this.tlpSearch.Controls.Add(this.txtSearchItem, 0, 1);
            this.tlpSearch.Controls.Add(this.lblVoucherType, 1, 0);
            this.tlpSearch.Controls.Add(this.lblCostCentre, 3, 0);
            this.tlpSearch.Controls.Add(this.dtpFromDate, 4, 1);
            this.tlpSearch.Controls.Add(this.cboVoucherType, 1, 1);
            this.tlpSearch.Controls.Add(this.cboBatchUnique, 2, 1);
            this.tlpSearch.Controls.Add(this.cboCostCentre, 3, 1);
            this.tlpSearch.Controls.Add(this.btnRefresh, 6, 0);
            this.tlpSearch.Controls.Add(this.lblRefresh, 6, 2);
            this.tlpSearch.Controls.Add(this.lblRptHeading, 0, 2);
            this.tlpSearch.Controls.Add(this.lblItemID, 0, 3);
            this.tlpSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpSearch.Location = new System.Drawing.Point(5, 66);
            this.tlpSearch.Margin = new System.Windows.Forms.Padding(5, 2, 5, 2);
            this.tlpSearch.Name = "tlpSearch";
            this.tlpSearch.RowCount = 3;
            this.tlpSearch.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 36.76471F));
            this.tlpSearch.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 63.23529F));
            this.tlpSearch.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tlpSearch.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 0F));
            this.tlpSearch.Size = new System.Drawing.Size(1315, 91);
            this.tlpSearch.TabIndex = 11;
            // 
            // dtpToDate
            // 
            this.dtpToDate.CustomFormat = "dd/MMM/yyyy";
            this.dtpToDate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dtpToDate.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtpToDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpToDate.Location = new System.Drawing.Point(1063, 25);
            this.dtpToDate.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.dtpToDate.Name = "dtpToDate";
            this.dtpToDate.Size = new System.Drawing.Size(143, 28);
            this.dtpToDate.TabIndex = 6;
            this.dtpToDate.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dtpToDate_KeyDown);
            // 
            // lblToDate
            // 
            this.lblToDate.AutoSize = true;
            this.lblToDate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblToDate.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblToDate.ForeColor = System.Drawing.Color.Black;
            this.lblToDate.Location = new System.Drawing.Point(1063, 0);
            this.lblToDate.Name = "lblToDate";
            this.lblToDate.Size = new System.Drawing.Size(143, 23);
            this.lblToDate.TabIndex = 6;
            this.lblToDate.Text = "To Date :";
            this.lblToDate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblFromDate
            // 
            this.lblFromDate.AutoSize = true;
            this.lblFromDate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblFromDate.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFromDate.ForeColor = System.Drawing.Color.Black;
            this.lblFromDate.Location = new System.Drawing.Point(914, 0);
            this.lblFromDate.Name = "lblFromDate";
            this.lblFromDate.Size = new System.Drawing.Size(143, 23);
            this.lblFromDate.TabIndex = 5;
            this.lblFromDate.Text = "From Date :";
            this.lblFromDate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblBatchUnique
            // 
            this.lblBatchUnique.AutoSize = true;
            this.lblBatchUnique.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblBatchUnique.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBatchUnique.ForeColor = System.Drawing.Color.Black;
            this.lblBatchUnique.Location = new System.Drawing.Point(474, 0);
            this.lblBatchUnique.Name = "lblBatchUnique";
            this.lblBatchUnique.Size = new System.Drawing.Size(254, 23);
            this.lblBatchUnique.TabIndex = 3;
            this.lblBatchUnique.Text = "Batch :";
            this.lblBatchUnique.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblItemName
            // 
            this.lblItemName.AutoSize = true;
            this.lblItemName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblItemName.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblItemName.ForeColor = System.Drawing.Color.Black;
            this.lblItemName.Location = new System.Drawing.Point(3, 0);
            this.lblItemName.Name = "lblItemName";
            this.lblItemName.Size = new System.Drawing.Size(245, 23);
            this.lblItemName.TabIndex = 0;
            this.lblItemName.Text = "Search Item :";
            this.lblItemName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtSearchItem
            // 
            this.txtSearchItem.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtSearchItem.Font = new System.Drawing.Font("Tahoma", 10.2F);
            this.txtSearchItem.Location = new System.Drawing.Point(3, 25);
            this.txtSearchItem.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtSearchItem.MaxLength = 5000;
            this.txtSearchItem.Name = "txtSearchItem";
            this.txtSearchItem.Size = new System.Drawing.Size(245, 28);
            this.txtSearchItem.TabIndex = 1;
            this.txtSearchItem.TextChanged += new System.EventHandler(this.txtSearchItem_TextChanged);
            this.txtSearchItem.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSearchItem_KeyDown);
            // 
            // lblVoucherType
            // 
            this.lblVoucherType.AutoSize = true;
            this.lblVoucherType.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblVoucherType.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblVoucherType.ForeColor = System.Drawing.Color.Black;
            this.lblVoucherType.Location = new System.Drawing.Point(254, 0);
            this.lblVoucherType.Name = "lblVoucherType";
            this.lblVoucherType.Size = new System.Drawing.Size(214, 23);
            this.lblVoucherType.TabIndex = 2;
            this.lblVoucherType.Text = "Voucher Type :";
            this.lblVoucherType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblCostCentre
            // 
            this.lblCostCentre.AutoSize = true;
            this.lblCostCentre.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblCostCentre.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCostCentre.ForeColor = System.Drawing.Color.Black;
            this.lblCostCentre.Location = new System.Drawing.Point(734, 0);
            this.lblCostCentre.Name = "lblCostCentre";
            this.lblCostCentre.Size = new System.Drawing.Size(174, 23);
            this.lblCostCentre.TabIndex = 4;
            this.lblCostCentre.Text = "Cost Centre :";
            this.lblCostCentre.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // dtpFromDate
            // 
            this.dtpFromDate.CustomFormat = "dd/MMM/yyyy";
            this.dtpFromDate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dtpFromDate.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtpFromDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpFromDate.Location = new System.Drawing.Point(914, 25);
            this.dtpFromDate.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.dtpFromDate.Name = "dtpFromDate";
            this.dtpFromDate.Size = new System.Drawing.Size(143, 28);
            this.dtpFromDate.TabIndex = 5;
            this.dtpFromDate.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dtpFromDate_KeyDown);
            // 
            // cboVoucherType
            // 
            this.cboVoucherType.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cboVoucherType.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboVoucherType.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cboVoucherType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboVoucherType.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboVoucherType.FormattingEnabled = true;
            this.cboVoucherType.Location = new System.Drawing.Point(254, 25);
            this.cboVoucherType.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cboVoucherType.Name = "cboVoucherType";
            this.cboVoucherType.Size = new System.Drawing.Size(214, 29);
            this.cboVoucherType.TabIndex = 2;
            this.cboVoucherType.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cboVoucherType_KeyDown);
            // 
            // cboBatchUnique
            // 
            this.cboBatchUnique.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cboBatchUnique.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboBatchUnique.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cboBatchUnique.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboBatchUnique.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboBatchUnique.FormattingEnabled = true;
            this.cboBatchUnique.Location = new System.Drawing.Point(474, 25);
            this.cboBatchUnique.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cboBatchUnique.Name = "cboBatchUnique";
            this.cboBatchUnique.Size = new System.Drawing.Size(254, 29);
            this.cboBatchUnique.TabIndex = 3;
            this.cboBatchUnique.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cboBatchUnique_KeyDown);
            // 
            // cboCostCentre
            // 
            this.cboCostCentre.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cboCostCentre.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboCostCentre.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cboCostCentre.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboCostCentre.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboCostCentre.FormattingEnabled = true;
            this.cboCostCentre.Location = new System.Drawing.Point(734, 25);
            this.cboCostCentre.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cboCostCentre.Name = "cboCostCentre";
            this.cboCostCentre.Size = new System.Drawing.Size(174, 29);
            this.cboCostCentre.TabIndex = 4;
            this.cboCostCentre.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cboCostCentre_KeyDown);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnRefresh.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.btnRefresh.FlatAppearance.BorderSize = 0;
            this.btnRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRefresh.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRefresh.ForeColor = System.Drawing.Color.Black;
            this.btnRefresh.Image = global::DigiposZen.Properties.Resources.refresh_removebg1;
            this.btnRefresh.Location = new System.Drawing.Point(1212, 2);
            this.btnRefresh.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnRefresh.Name = "btnRefresh";
            this.tlpSearch.SetRowSpan(this.btnRefresh, 2);
            this.btnRefresh.Size = new System.Drawing.Size(103, 58);
            this.btnRefresh.TabIndex = 7;
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // lblRefresh
            // 
            this.lblRefresh.AutoSize = true;
            this.lblRefresh.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblRefresh.Font = new System.Drawing.Font("Tahoma", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRefresh.ForeColor = System.Drawing.Color.Black;
            this.lblRefresh.Location = new System.Drawing.Point(1212, 62);
            this.lblRefresh.Name = "lblRefresh";
            this.lblRefresh.Size = new System.Drawing.Size(103, 28);
            this.lblRefresh.TabIndex = 12;
            this.lblRefresh.Text = "Refresh";
            this.lblRefresh.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblRptHeading
            // 
            this.lblRptHeading.AutoSize = true;
            this.lblRptHeading.BackColor = System.Drawing.Color.Transparent;
            this.tlpSearch.SetColumnSpan(this.lblRptHeading, 4);
            this.lblRptHeading.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblRptHeading.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRptHeading.ForeColor = System.Drawing.Color.Black;
            this.lblRptHeading.Location = new System.Drawing.Point(3, 69);
            this.lblRptHeading.Name = "lblRptHeading";
            this.lblRptHeading.Size = new System.Drawing.Size(905, 21);
            this.lblRptHeading.TabIndex = 13;
            this.lblRptHeading.Text = "Item Analysis Details";
            this.lblRptHeading.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblItemID
            // 
            this.lblItemID.AutoSize = true;
            this.lblItemID.BackColor = System.Drawing.Color.Transparent;
            this.lblItemID.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblItemID.Location = new System.Drawing.Point(3, 90);
            this.lblItemID.Name = "lblItemID";
            this.lblItemID.Size = new System.Drawing.Size(0, 1);
            this.lblItemID.TabIndex = 16;
            this.lblItemID.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.lblItemID.Visible = false;
            // 
            // tlpBottom
            // 
            this.tlpBottom.ColumnCount = 1;
            this.tlpBottom.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpBottom.Controls.Add(this.flpQty, 0, 0);
            this.tlpBottom.Location = new System.Drawing.Point(188, 711);
            this.tlpBottom.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tlpBottom.Name = "tlpBottom";
            this.tlpBottom.RowCount = 1;
            this.tlpBottom.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpBottom.Size = new System.Drawing.Size(1080, 34);
            this.tlpBottom.TabIndex = 12;
            // 
            // flpQty
            // 
            this.flpQty.Controls.Add(this.tlpQOH);
            this.flpQty.Controls.Add(this.tlpQtyOut);
            this.flpQty.Controls.Add(this.tlpQtyIn);
            this.flpQty.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpQty.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flpQty.Location = new System.Drawing.Point(0, 0);
            this.flpQty.Margin = new System.Windows.Forms.Padding(0);
            this.flpQty.Name = "flpQty";
            this.flpQty.Size = new System.Drawing.Size(1080, 34);
            this.flpQty.TabIndex = 1;
            // 
            // tlpQOH
            // 
            this.tlpQOH.ColumnCount = 2;
            this.tlpQOH.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 69F));
            this.tlpQOH.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 95F));
            this.tlpQOH.Controls.Add(this.lblQOHAmt, 1, 0);
            this.tlpQOH.Controls.Add(this.lblQOH, 0, 0);
            this.tlpQOH.Location = new System.Drawing.Point(916, 0);
            this.tlpQOH.Margin = new System.Windows.Forms.Padding(0);
            this.tlpQOH.Name = "tlpQOH";
            this.tlpQOH.RowCount = 1;
            this.tlpQOH.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpQOH.Size = new System.Drawing.Size(164, 25);
            this.tlpQOH.TabIndex = 0;
            // 
            // lblQOHAmt
            // 
            this.lblQOHAmt.AutoSize = true;
            this.lblQOHAmt.BackColor = System.Drawing.Color.White;
            this.lblQOHAmt.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblQOHAmt.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblQOHAmt.Location = new System.Drawing.Point(72, 0);
            this.lblQOHAmt.Name = "lblQOHAmt";
            this.lblQOHAmt.Size = new System.Drawing.Size(89, 25);
            this.lblQOHAmt.TabIndex = 0;
            this.lblQOHAmt.Text = "000000";
            this.lblQOHAmt.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblQOH
            // 
            this.lblQOH.AutoSize = true;
            this.lblQOH.BackColor = System.Drawing.Color.Transparent;
            this.lblQOH.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblQOH.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblQOH.ForeColor = System.Drawing.Color.Black;
            this.lblQOH.Location = new System.Drawing.Point(3, 0);
            this.lblQOH.Name = "lblQOH";
            this.lblQOH.Size = new System.Drawing.Size(63, 25);
            this.lblQOH.TabIndex = 0;
            this.lblQOH.Text = "QOH:";
            this.lblQOH.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tlpQtyOut
            // 
            this.tlpQtyOut.ColumnCount = 2;
            this.tlpQtyOut.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 135F));
            this.tlpQtyOut.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 95F));
            this.tlpQtyOut.Controls.Add(this.lblQtyOutAmt, 1, 0);
            this.tlpQtyOut.Controls.Add(this.lblQtyOut, 0, 0);
            this.tlpQtyOut.Location = new System.Drawing.Point(687, 0);
            this.tlpQtyOut.Margin = new System.Windows.Forms.Padding(0);
            this.tlpQtyOut.Name = "tlpQtyOut";
            this.tlpQtyOut.RowCount = 1;
            this.tlpQtyOut.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpQtyOut.Size = new System.Drawing.Size(229, 25);
            this.tlpQtyOut.TabIndex = 0;
            // 
            // lblQtyOutAmt
            // 
            this.lblQtyOutAmt.AutoSize = true;
            this.lblQtyOutAmt.BackColor = System.Drawing.Color.White;
            this.lblQtyOutAmt.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblQtyOutAmt.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblQtyOutAmt.Location = new System.Drawing.Point(138, 0);
            this.lblQtyOutAmt.Name = "lblQtyOutAmt";
            this.lblQtyOutAmt.Size = new System.Drawing.Size(89, 25);
            this.lblQtyOutAmt.TabIndex = 0;
            this.lblQtyOutAmt.Text = "000000";
            this.lblQtyOutAmt.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblQtyOut
            // 
            this.lblQtyOut.AutoSize = true;
            this.lblQtyOut.BackColor = System.Drawing.Color.Transparent;
            this.lblQtyOut.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblQtyOut.ForeColor = System.Drawing.Color.Black;
            this.lblQtyOut.Location = new System.Drawing.Point(3, 0);
            this.lblQtyOut.Name = "lblQtyOut";
            this.lblQtyOut.Size = new System.Drawing.Size(117, 21);
            this.lblQtyOut.TabIndex = 0;
            this.lblQtyOut.Text = "Total Qty Out:";
            this.lblQtyOut.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tlpQtyIn
            // 
            this.tlpQtyIn.ColumnCount = 2;
            this.tlpQtyIn.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.tlpQtyIn.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 97F));
            this.tlpQtyIn.Controls.Add(this.lblQtyIn, 0, 0);
            this.tlpQtyIn.Controls.Add(this.lblQtyInAmt, 1, 0);
            this.tlpQtyIn.Location = new System.Drawing.Point(470, 0);
            this.tlpQtyIn.Margin = new System.Windows.Forms.Padding(0);
            this.tlpQtyIn.Name = "tlpQtyIn";
            this.tlpQtyIn.RowCount = 1;
            this.tlpQtyIn.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpQtyIn.Size = new System.Drawing.Size(217, 25);
            this.tlpQtyIn.TabIndex = 0;
            // 
            // lblQtyIn
            // 
            this.lblQtyIn.AutoSize = true;
            this.lblQtyIn.BackColor = System.Drawing.Color.Transparent;
            this.lblQtyIn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblQtyIn.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblQtyIn.ForeColor = System.Drawing.Color.Black;
            this.lblQtyIn.Location = new System.Drawing.Point(3, 0);
            this.lblQtyIn.Name = "lblQtyIn";
            this.lblQtyIn.Size = new System.Drawing.Size(114, 25);
            this.lblQtyIn.TabIndex = 0;
            this.lblQtyIn.Text = "Total Qty In :";
            this.lblQtyIn.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblQtyInAmt
            // 
            this.lblQtyInAmt.AutoSize = true;
            this.lblQtyInAmt.BackColor = System.Drawing.Color.White;
            this.lblQtyInAmt.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblQtyInAmt.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblQtyInAmt.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblQtyInAmt.Location = new System.Drawing.Point(123, 0);
            this.lblQtyInAmt.Name = "lblQtyInAmt";
            this.lblQtyInAmt.Size = new System.Drawing.Size(91, 25);
            this.lblQtyInAmt.TabIndex = 0;
            this.lblQtyInAmt.Text = "000000";
            this.lblQtyInAmt.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // frmItemAnalysis
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(1327, 767);
            this.Controls.Add(this.tlpMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.MinimizeBox = false;
            this.Name = "frmItemAnalysis";
            this.Padding = new System.Windows.Forms.Padding(1);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmItemAnalysis";
            this.Load += new System.EventHandler(this.frmItemAnalysis_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmItemAnalysis_KeyDown);
            this.tblpForms.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvStockHistory)).EndInit();
            this.tlpMenuButton.ResumeLayout(false);
            this.tlpMenuButton.PerformLayout();
            this.tlpHeading.ResumeLayout(false);
            this.tlpHeading.PerformLayout();
            this.tlpMain.ResumeLayout(false);
            this.tlpSearch.ResumeLayout(false);
            this.tlpSearch.PerformLayout();
            this.tlpBottom.ResumeLayout(false);
            this.flpQty.ResumeLayout(false);
            this.tlpQOH.ResumeLayout(false);
            this.tlpQOH.PerformLayout();
            this.tlpQtyOut.ResumeLayout(false);
            this.tlpQtyOut.PerformLayout();
            this.tlpQtyIn.ResumeLayout(false);
            this.tlpQtyIn.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tblpForms;
        private System.Windows.Forms.TableLayoutPanel tlpMenuButton;
        private System.Windows.Forms.RadioButton rdoStockHistory;
        private System.Windows.Forms.TableLayoutPanel tlpHeading;
        private System.Windows.Forms.TableLayoutPanel tlpMain;
        private System.Windows.Forms.TableLayoutPanel tlpSearch;
        private System.Windows.Forms.Label lblHeading;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnMinimize;
        private System.Windows.Forms.Label lblItemName;
        private System.Windows.Forms.TextBox txtSearchItem;
        private System.Windows.Forms.Label lblVoucherType;
        private System.Windows.Forms.Label lblBatchUnique;
        private System.Windows.Forms.Label lblCostCentre;
        private System.Windows.Forms.Label lblToDate;
        private System.Windows.Forms.Label lblFromDate;
        private System.Windows.Forms.DateTimePicker dtpToDate;
        private System.Windows.Forms.DateTimePicker dtpFromDate;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.ComboBox cboVoucherType;
        private System.Windows.Forms.ComboBox cboBatchUnique;
        private System.Windows.Forms.ComboBox cboCostCentre;
        private System.Windows.Forms.DataGridView dgvStockHistory;
        private System.Windows.Forms.TableLayoutPanel tlpBottom;
        private System.Windows.Forms.FlowLayoutPanel flpQty;
        private System.Windows.Forms.TableLayoutPanel tlpQtyIn;
        private System.Windows.Forms.Label lblQtyInAmt;
        private System.Windows.Forms.Label lblQtyIn;
        private System.Windows.Forms.TableLayoutPanel tlpQtyOut;
        private System.Windows.Forms.Label lblQtyOutAmt;
        private System.Windows.Forms.Label lblQtyOut;
        private System.Windows.Forms.TableLayoutPanel tlpQOH;
        private System.Windows.Forms.Label lblQOHAmt;
        private System.Windows.Forms.Label lblQOH;
        private System.Windows.Forms.Label lblRefresh;
        private System.Windows.Forms.Label lblRptHeading;
        private System.Windows.Forms.Label lblItemID;
        private System.Windows.Forms.Label lblFooter;
        private System.Windows.Forms.RadioButton rdoStockReport;
    }
}