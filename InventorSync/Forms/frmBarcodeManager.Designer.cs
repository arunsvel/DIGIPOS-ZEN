
namespace DigiposZen.Forms
{
    partial class frmBarcodeManager
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmBarcodeManager));
            this.tlpMain = new System.Windows.Forms.TableLayoutPanel();
            this.tlpHeader = new System.Windows.Forms.TableLayoutPanel();
            this.lblFind = new System.Windows.Forms.Label();
            this.lblSave = new System.Windows.Forms.Label();
            this.btnFind = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.lblHeading = new System.Windows.Forms.Label();
            this.btnMinimize = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnYtubeTutorial = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.cboSalesStaff = new System.Windows.Forms.ComboBox();
            this.label14 = new System.Windows.Forms.Label();
            this.cboCostCentre = new System.Windows.Forms.ComboBox();
            this.lblCostCenter = new System.Windows.Forms.Label();
            this.txtPrefix = new System.Windows.Forms.TextBox();
            this.txtInvAutoNo = new System.Windows.Forms.TextBox();
            this.lblInvNo = new System.Windows.Forms.Label();
            this.btnNext = new System.Windows.Forms.Button();
            this.btnprev = new System.Windows.Forms.Button();
            this.dtpInvDate = new System.Windows.Forms.DateTimePicker();
            this.lblInvDate = new System.Windows.Forms.Label();
            this.lblFooter = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tabBarcode = new System.Windows.Forms.TabControl();
            this.tbpBatchDeactivator = new System.Windows.Forms.TabPage();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtFillSearch = new System.Windows.Forms.TextBox();
            this.btnFillData = new System.Windows.Forms.Button();
            this.cmbDisplayStyle = new System.Windows.Forms.ComboBox();
            this.chkExactWordOnly = new System.Windows.Forms.CheckBox();
            this.chkMatchCase = new System.Windows.Forms.CheckBox();
            this.btnSearchFwd = new System.Windows.Forms.Button();
            this.btnSearchBwd = new System.Windows.Forms.Button();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.DgvData = new System.Windows.Forms.DataGridView();
            this.tbpBarcodeChanger = new System.Windows.Forms.TabPage();
            this.tlpMain.SuspendLayout();
            this.tlpHeader.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tabBarcode.SuspendLayout();
            this.tbpBatchDeactivator.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DgvData)).BeginInit();
            this.SuspendLayout();
            // 
            // tlpMain
            // 
            this.tlpMain.BackColor = System.Drawing.Color.White;
            this.tlpMain.ColumnCount = 1;
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 1387F));
            this.tlpMain.Controls.Add(this.tlpHeader, 0, 0);
            this.tlpMain.Controls.Add(this.lblFooter, 0, 2);
            this.tlpMain.Controls.Add(this.panel1, 0, 1);
            this.tlpMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpMain.Location = new System.Drawing.Point(1, 1);
            this.tlpMain.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tlpMain.Name = "tlpMain";
            this.tlpMain.RowCount = 3;
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 95F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 22F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 22F));
            this.tlpMain.Size = new System.Drawing.Size(1404, 573);
            this.tlpMain.TabIndex = 2;
            // 
            // tlpHeader
            // 
            this.tlpHeader.BackColor = System.Drawing.Color.White;
            this.tlpHeader.ColumnCount = 8;
            this.tlpHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 77F));
            this.tlpHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 77F));
            this.tlpHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tlpHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tlpHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tlpHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tlpHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 74F));
            this.tlpHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 84F));
            this.tlpHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 21F));
            this.tlpHeader.Controls.Add(this.lblFind, 1, 1);
            this.tlpHeader.Controls.Add(this.lblSave, 0, 1);
            this.tlpHeader.Controls.Add(this.btnFind, 1, 0);
            this.tlpHeader.Controls.Add(this.btnSave, 0, 0);
            this.tlpHeader.Controls.Add(this.lblHeading, 2, 0);
            this.tlpHeader.Controls.Add(this.btnMinimize, 6, 0);
            this.tlpHeader.Controls.Add(this.btnClose, 7, 0);
            this.tlpHeader.Controls.Add(this.btnYtubeTutorial, 6, 1);
            this.tlpHeader.Controls.Add(this.panel2, 2, 1);
            this.tlpHeader.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpHeader.Location = new System.Drawing.Point(0, 0);
            this.tlpHeader.Margin = new System.Windows.Forms.Padding(0);
            this.tlpHeader.Name = "tlpHeader";
            this.tlpHeader.RowCount = 2;
            this.tlpHeader.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 51F));
            this.tlpHeader.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 51F));
            this.tlpHeader.Size = new System.Drawing.Size(1404, 95);
            this.tlpHeader.TabIndex = 0;
            // 
            // lblFind
            // 
            this.lblFind.Font = new System.Drawing.Font("Tahoma", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFind.ForeColor = System.Drawing.Color.Black;
            this.lblFind.Location = new System.Drawing.Point(80, 51);
            this.lblFind.Name = "lblFind";
            this.lblFind.Size = new System.Drawing.Size(71, 30);
            this.lblFind.TabIndex = 14;
            this.lblFind.Text = "Find";
            this.lblFind.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblSave
            // 
            this.lblSave.Font = new System.Drawing.Font("Tahoma", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSave.ForeColor = System.Drawing.Color.Black;
            this.lblSave.Location = new System.Drawing.Point(3, 51);
            this.lblSave.Name = "lblSave";
            this.lblSave.Size = new System.Drawing.Size(71, 30);
            this.lblSave.TabIndex = 13;
            this.lblSave.Text = "Save";
            this.lblSave.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnFind
            // 
            this.btnFind.BackColor = System.Drawing.Color.Transparent;
            this.btnFind.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnFind.FlatAppearance.BorderSize = 0;
            this.btnFind.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFind.Image = global::DigiposZen.Properties.Resources.find_finalised_3030;
            this.btnFind.Location = new System.Drawing.Point(80, 2);
            this.btnFind.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnFind.Name = "btnFind";
            this.btnFind.Size = new System.Drawing.Size(71, 47);
            this.btnFind.TabIndex = 12;
            this.btnFind.UseVisualStyleBackColor = false;
            // 
            // btnSave
            // 
            this.btnSave.BackColor = System.Drawing.Color.Transparent;
            this.btnSave.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnSave.FlatAppearance.BorderSize = 0;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Image = global::DigiposZen.Properties.Resources.save240402;
            this.btnSave.Location = new System.Drawing.Point(3, 2);
            this.btnSave.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(71, 47);
            this.btnSave.TabIndex = 11;
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // lblHeading
            // 
            this.lblHeading.AutoSize = true;
            this.lblHeading.BackColor = System.Drawing.Color.Transparent;
            this.tlpHeader.SetColumnSpan(this.lblHeading, 4);
            this.lblHeading.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblHeading.Font = new System.Drawing.Font("Tahoma", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHeading.ForeColor = System.Drawing.Color.Black;
            this.lblHeading.Location = new System.Drawing.Point(175, 0);
            this.lblHeading.Margin = new System.Windows.Forms.Padding(21, 0, 21, 0);
            this.lblHeading.Name = "lblHeading";
            this.lblHeading.Size = new System.Drawing.Size(1050, 51);
            this.lblHeading.TabIndex = 2;
            this.lblHeading.Text = "Barcode Manager";
            this.lblHeading.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnMinimize
            // 
            this.btnMinimize.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnMinimize.FlatAppearance.BorderSize = 0;
            this.btnMinimize.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMinimize.Image = global::DigiposZen.Properties.Resources.minimize_finalised;
            this.btnMinimize.Location = new System.Drawing.Point(1249, 2);
            this.btnMinimize.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnMinimize.Name = "btnMinimize";
            this.btnMinimize.Size = new System.Drawing.Size(68, 47);
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
            this.btnClose.Location = new System.Drawing.Point(1323, 2);
            this.btnClose.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(78, 47);
            this.btnClose.TabIndex = 6;
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnYtubeTutorial
            // 
            this.btnYtubeTutorial.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnYtubeTutorial.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnYtubeTutorial.FlatAppearance.BorderSize = 0;
            this.btnYtubeTutorial.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnYtubeTutorial.ForeColor = System.Drawing.Color.Black;
            this.btnYtubeTutorial.Image = ((System.Drawing.Image)(resources.GetObject("btnYtubeTutorial.Image")));
            this.btnYtubeTutorial.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnYtubeTutorial.Location = new System.Drawing.Point(1249, 53);
            this.btnYtubeTutorial.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnYtubeTutorial.Name = "btnYtubeTutorial";
            this.btnYtubeTutorial.Size = new System.Drawing.Size(68, 47);
            this.btnYtubeTutorial.TabIndex = 10;
            this.btnYtubeTutorial.Text = "Tutorial";
            this.btnYtubeTutorial.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnYtubeTutorial.UseVisualStyleBackColor = true;
            this.btnYtubeTutorial.Visible = false;
            // 
            // panel2
            // 
            this.tlpHeader.SetColumnSpan(this.panel2, 4);
            this.panel2.Controls.Add(this.cboSalesStaff);
            this.panel2.Controls.Add(this.label14);
            this.panel2.Controls.Add(this.cboCostCentre);
            this.panel2.Controls.Add(this.lblCostCenter);
            this.panel2.Controls.Add(this.txtPrefix);
            this.panel2.Controls.Add(this.txtInvAutoNo);
            this.panel2.Controls.Add(this.lblInvNo);
            this.panel2.Controls.Add(this.btnNext);
            this.panel2.Controls.Add(this.btnprev);
            this.panel2.Controls.Add(this.dtpInvDate);
            this.panel2.Controls.Add(this.lblInvDate);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(157, 54);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1086, 45);
            this.panel2.TabIndex = 15;
            // 
            // cboSalesStaff
            // 
            this.cboSalesStaff.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cboSalesStaff.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboSalesStaff.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSalesStaff.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboSalesStaff.FormattingEnabled = true;
            this.cboSalesStaff.Location = new System.Drawing.Point(957, 8);
            this.cboSalesStaff.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cboSalesStaff.Name = "cboSalesStaff";
            this.cboSalesStaff.Size = new System.Drawing.Size(111, 29);
            this.cboSalesStaff.TabIndex = 35;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.Location = new System.Drawing.Point(857, 12);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(94, 21);
            this.label14.TabIndex = 36;
            this.label14.Text = "Sales Staff:";
            this.label14.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // cboCostCentre
            // 
            this.cboCostCentre.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cboCostCentre.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboCostCentre.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboCostCentre.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboCostCentre.FormattingEnabled = true;
            this.cboCostCentre.Location = new System.Drawing.Point(722, 8);
            this.cboCostCentre.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cboCostCentre.Name = "cboCostCentre";
            this.cboCostCentre.Size = new System.Drawing.Size(116, 29);
            this.cboCostCentre.TabIndex = 33;
            // 
            // lblCostCenter
            // 
            this.lblCostCenter.AutoSize = true;
            this.lblCostCenter.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCostCenter.Location = new System.Drawing.Point(613, 12);
            this.lblCostCenter.Name = "lblCostCenter";
            this.lblCostCenter.Size = new System.Drawing.Size(103, 21);
            this.lblCostCenter.TabIndex = 34;
            this.lblCostCenter.Text = "Cost Centre:";
            this.lblCostCenter.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // txtPrefix
            // 
            this.txtPrefix.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPrefix.Location = new System.Drawing.Point(192, 8);
            this.txtPrefix.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtPrefix.Name = "txtPrefix";
            this.txtPrefix.Size = new System.Drawing.Size(47, 28);
            this.txtPrefix.TabIndex = 28;
            this.txtPrefix.Visible = false;
            // 
            // txtInvAutoNo
            // 
            this.txtInvAutoNo.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtInvAutoNo.Location = new System.Drawing.Point(192, 8);
            this.txtInvAutoNo.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtInvAutoNo.MaxLength = 100;
            this.txtInvAutoNo.Name = "txtInvAutoNo";
            this.txtInvAutoNo.Size = new System.Drawing.Size(127, 28);
            this.txtInvAutoNo.TabIndex = 29;
            this.txtInvAutoNo.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtInvAutoNo.Leave += new System.EventHandler(this.txtInvAutoNo_Leave);
            // 
            // lblInvNo
            // 
            this.lblInvNo.AutoSize = true;
            this.lblInvNo.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblInvNo.ForeColor = System.Drawing.Color.Black;
            this.lblInvNo.Location = new System.Drawing.Point(19, 12);
            this.lblInvNo.Name = "lblInvNo";
            this.lblInvNo.Size = new System.Drawing.Size(132, 21);
            this.lblInvNo.TabIndex = 30;
            this.lblInvNo.Text = "Invoice Number:";
            this.lblInvNo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnNext
            // 
            this.btnNext.FlatAppearance.BorderSize = 0;
            this.btnNext.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNext.Image = global::DigiposZen.Properties.Resources.fast_forward;
            this.btnNext.Location = new System.Drawing.Point(316, 7);
            this.btnNext.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(28, 31);
            this.btnNext.TabIndex = 31;
            this.btnNext.UseVisualStyleBackColor = true;
            // 
            // btnprev
            // 
            this.btnprev.FlatAppearance.BorderSize = 0;
            this.btnprev.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnprev.Image = global::DigiposZen.Properties.Resources.fast_backwards;
            this.btnprev.Location = new System.Drawing.Point(162, 7);
            this.btnprev.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnprev.Name = "btnprev";
            this.btnprev.Size = new System.Drawing.Size(28, 31);
            this.btnprev.TabIndex = 32;
            this.btnprev.UseVisualStyleBackColor = true;
            // 
            // dtpInvDate
            // 
            this.dtpInvDate.CustomFormat = "dd/MMM/yyyy";
            this.dtpInvDate.Font = new System.Drawing.Font("Tahoma", 9F);
            this.dtpInvDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpInvDate.Location = new System.Drawing.Point(432, 9);
            this.dtpInvDate.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.dtpInvDate.Name = "dtpInvDate";
            this.dtpInvDate.Size = new System.Drawing.Size(145, 26);
            this.dtpInvDate.TabIndex = 26;
            // 
            // lblInvDate
            // 
            this.lblInvDate.AutoSize = true;
            this.lblInvDate.Font = new System.Drawing.Font("Tahoma", 9F);
            this.lblInvDate.ForeColor = System.Drawing.Color.Black;
            this.lblInvDate.Location = new System.Drawing.Point(357, 13);
            this.lblInvDate.Name = "lblInvDate";
            this.lblInvDate.Size = new System.Drawing.Size(68, 18);
            this.lblInvDate.TabIndex = 27;
            this.lblInvDate.Text = "VchDate:";
            this.lblInvDate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblFooter
            // 
            this.lblFooter.BackColor = System.Drawing.Color.White;
            this.lblFooter.ForeColor = System.Drawing.Color.Black;
            this.lblFooter.Location = new System.Drawing.Point(3, 551);
            this.lblFooter.Name = "lblFooter";
            this.lblFooter.Size = new System.Drawing.Size(961, 22);
            this.lblFooter.TabIndex = 4;
            this.lblFooter.Text = "Keyboard Shortcuts :  F5 Execute, F6 Export CSV";
            this.lblFooter.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.tabBarcode);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 98);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1398, 450);
            this.panel1.TabIndex = 5;
            // 
            // tabBarcode
            // 
            this.tabBarcode.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabBarcode.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.tabBarcode.Controls.Add(this.tbpBatchDeactivator);
            this.tabBarcode.Controls.Add(this.tbpBarcodeChanger);
            this.tabBarcode.Font = new System.Drawing.Font("Tahoma", 9F);
            this.tabBarcode.Location = new System.Drawing.Point(11, 14);
            this.tabBarcode.Name = "tabBarcode";
            this.tabBarcode.SelectedIndex = 0;
            this.tabBarcode.Size = new System.Drawing.Size(1376, 433);
            this.tabBarcode.TabIndex = 0;
            // 
            // tbpBatchDeactivator
            // 
            this.tbpBatchDeactivator.BackColor = System.Drawing.Color.White;
            this.tbpBatchDeactivator.Controls.Add(this.label2);
            this.tbpBatchDeactivator.Controls.Add(this.label1);
            this.tbpBatchDeactivator.Controls.Add(this.txtFillSearch);
            this.tbpBatchDeactivator.Controls.Add(this.btnFillData);
            this.tbpBatchDeactivator.Controls.Add(this.cmbDisplayStyle);
            this.tbpBatchDeactivator.Controls.Add(this.chkExactWordOnly);
            this.tbpBatchDeactivator.Controls.Add(this.chkMatchCase);
            this.tbpBatchDeactivator.Controls.Add(this.btnSearchFwd);
            this.tbpBatchDeactivator.Controls.Add(this.btnSearchBwd);
            this.tbpBatchDeactivator.Controls.Add(this.txtSearch);
            this.tbpBatchDeactivator.Controls.Add(this.DgvData);
            this.tbpBatchDeactivator.Location = new System.Drawing.Point(4, 30);
            this.tbpBatchDeactivator.Name = "tbpBatchDeactivator";
            this.tbpBatchDeactivator.Padding = new System.Windows.Forms.Padding(3);
            this.tbpBatchDeactivator.Size = new System.Drawing.Size(1368, 399);
            this.tbpBatchDeactivator.TabIndex = 0;
            this.tbpBatchDeactivator.Text = "Batch Deactivator";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 87);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(340, 18);
            this.label2.TabIndex = 24;
            this.label2.Text = "Search for a text or value in the details filled below:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(403, 18);
            this.label1.TabIndex = 23;
            this.label1.Text = "Enter barcode or part of barcode or itemcode  or itemname:";
            // 
            // txtFillSearch
            // 
            this.txtFillSearch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFillSearch.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtFillSearch.Location = new System.Drawing.Point(15, 44);
            this.txtFillSearch.Name = "txtFillSearch";
            this.txtFillSearch.Size = new System.Drawing.Size(443, 26);
            this.txtFillSearch.TabIndex = 22;
            // 
            // btnFillData
            // 
            this.btnFillData.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFillData.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFillData.Location = new System.Drawing.Point(704, 38);
            this.btnFillData.Name = "btnFillData";
            this.btnFillData.Size = new System.Drawing.Size(96, 34);
            this.btnFillData.TabIndex = 21;
            this.btnFillData.Text = "Fill Data";
            this.btnFillData.UseVisualStyleBackColor = true;
            this.btnFillData.Click += new System.EventHandler(this.btnFillData_Click);
            // 
            // cmbDisplayStyle
            // 
            this.cmbDisplayStyle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbDisplayStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDisplayStyle.FormattingEnabled = true;
            this.cmbDisplayStyle.Items.AddRange(new object[] {
            "<ALL ITEMS>",
            "Batches Having No Transactions",
            "Negative /Zero Qty Batches",
            "Active Batches",
            "Deactive Batches"});
            this.cmbDisplayStyle.Location = new System.Drawing.Point(465, 44);
            this.cmbDisplayStyle.Name = "cmbDisplayStyle";
            this.cmbDisplayStyle.Size = new System.Drawing.Size(233, 26);
            this.cmbDisplayStyle.TabIndex = 20;
            // 
            // chkExactWordOnly
            // 
            this.chkExactWordOnly.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkExactWordOnly.AutoSize = true;
            this.chkExactWordOnly.Location = new System.Drawing.Point(946, 111);
            this.chkExactWordOnly.Name = "chkExactWordOnly";
            this.chkExactWordOnly.Size = new System.Drawing.Size(140, 22);
            this.chkExactWordOnly.TabIndex = 19;
            this.chkExactWordOnly.Text = "Exact Word Only";
            this.chkExactWordOnly.UseVisualStyleBackColor = true;
            // 
            // chkMatchCase
            // 
            this.chkMatchCase.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkMatchCase.AutoSize = true;
            this.chkMatchCase.Location = new System.Drawing.Point(822, 112);
            this.chkMatchCase.Name = "chkMatchCase";
            this.chkMatchCase.Size = new System.Drawing.Size(107, 22);
            this.chkMatchCase.TabIndex = 18;
            this.chkMatchCase.Text = "Match Case";
            this.chkMatchCase.UseVisualStyleBackColor = true;
            // 
            // btnSearchFwd
            // 
            this.btnSearchFwd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSearchFwd.Image = global::DigiposZen.Properties.Resources.fast_forward;
            this.btnSearchFwd.Location = new System.Drawing.Point(756, 105);
            this.btnSearchFwd.Name = "btnSearchFwd";
            this.btnSearchFwd.Size = new System.Drawing.Size(45, 34);
            this.btnSearchFwd.TabIndex = 17;
            this.btnSearchFwd.UseVisualStyleBackColor = true;
            this.btnSearchFwd.Click += new System.EventHandler(this.btnSearchFwd_Click);
            // 
            // btnSearchBwd
            // 
            this.btnSearchBwd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSearchBwd.Image = global::DigiposZen.Properties.Resources.fast_backwards;
            this.btnSearchBwd.Location = new System.Drawing.Point(704, 105);
            this.btnSearchBwd.Name = "btnSearchBwd";
            this.btnSearchBwd.Size = new System.Drawing.Size(45, 34);
            this.btnSearchBwd.TabIndex = 16;
            this.btnSearchBwd.UseVisualStyleBackColor = true;
            this.btnSearchBwd.Click += new System.EventHandler(this.btnSearchBwd_Click);
            // 
            // txtSearch
            // 
            this.txtSearch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSearch.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSearch.Location = new System.Drawing.Point(15, 110);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(683, 26);
            this.txtSearch.TabIndex = 15;
            // 
            // DgvData
            // 
            this.DgvData.AllowUserToAddRows = false;
            this.DgvData.AllowUserToDeleteRows = false;
            this.DgvData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DgvData.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.DgvData.BackgroundColor = System.Drawing.Color.White;
            this.DgvData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DgvData.Location = new System.Drawing.Point(15, 146);
            this.DgvData.Name = "DgvData";
            this.DgvData.RowHeadersWidth = 51;
            this.DgvData.RowTemplate.Height = 24;
            this.DgvData.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.DgvData.Size = new System.Drawing.Size(1340, 236);
            this.DgvData.TabIndex = 14;
            this.DgvData.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DgvData_CellContentClick);
            // 
            // tbpBarcodeChanger
            // 
            this.tbpBarcodeChanger.BackColor = System.Drawing.Color.White;
            this.tbpBarcodeChanger.Location = new System.Drawing.Point(4, 30);
            this.tbpBarcodeChanger.Name = "tbpBarcodeChanger";
            this.tbpBarcodeChanger.Padding = new System.Windows.Forms.Padding(3);
            this.tbpBarcodeChanger.Size = new System.Drawing.Size(1368, 399);
            this.tbpBarcodeChanger.TabIndex = 1;
            this.tbpBarcodeChanger.Text = "Barcode Changer";
            // 
            // frmBarcodeManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(1406, 575);
            this.Controls.Add(this.tlpMain);
            this.Font = new System.Drawing.Font("Tahoma", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmBarcodeManager";
            this.Padding = new System.Windows.Forms.Padding(1);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmBarcodeManager";
            this.tlpMain.ResumeLayout(false);
            this.tlpHeader.ResumeLayout(false);
            this.tlpHeader.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.tabBarcode.ResumeLayout(false);
            this.tbpBatchDeactivator.ResumeLayout(false);
            this.tbpBatchDeactivator.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DgvData)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlpMain;
        private System.Windows.Forms.TableLayoutPanel tlpHeader;
        private System.Windows.Forms.Label lblHeading;
        private System.Windows.Forms.Button btnMinimize;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnYtubeTutorial;
        private System.Windows.Forms.Label lblFooter;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TabControl tabBarcode;
        private System.Windows.Forms.TabPage tbpBatchDeactivator;
        private System.Windows.Forms.CheckBox chkExactWordOnly;
        private System.Windows.Forms.CheckBox chkMatchCase;
        private System.Windows.Forms.Button btnSearchFwd;
        private System.Windows.Forms.Button btnSearchBwd;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.DataGridView DgvData;
        private System.Windows.Forms.TabPage tbpBarcodeChanger;
        private System.Windows.Forms.ComboBox cmbDisplayStyle;
        private System.Windows.Forms.Button btnFillData;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtFillSearch;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnFind;
        private System.Windows.Forms.Label lblSave;
        private System.Windows.Forms.Label lblFind;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.DateTimePicker dtpInvDate;
        private System.Windows.Forms.Label lblInvDate;
        private System.Windows.Forms.TextBox txtPrefix;
        private System.Windows.Forms.TextBox txtInvAutoNo;
        private System.Windows.Forms.Label lblInvNo;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Button btnprev;
        private System.Windows.Forms.ComboBox cboCostCentre;
        private System.Windows.Forms.Label lblCostCenter;
        private System.Windows.Forms.ComboBox cboSalesStaff;
        private System.Windows.Forms.Label label14;
    }
}