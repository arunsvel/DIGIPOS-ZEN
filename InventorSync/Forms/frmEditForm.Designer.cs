namespace InventorSync.Forms
{
    partial class frmEditForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmEditForm));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.dtpTo = new System.Windows.Forms.DateTimePicker();
            this.label3 = new System.Windows.Forms.Label();
            this.dtpFrom = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.gridGroupingControlSearch = new Syncfusion.Windows.Forms.Grid.Grouping.GridGroupingControl();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.btnCancelDeactive = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.btnNew = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnMinimize = new System.Windows.Forms.Button();
            this.btnMenu = new System.Windows.Forms.Button();
            this.btnYtubeTutorial = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.btnEdit = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.btnDelete = new System.Windows.Forms.Button();
            this.lblHeading = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.toolTipManufacturer = new System.Windows.Forms.ToolTip(this.components);
            this.label47 = new System.Windows.Forms.Label();
            this.tlpMain = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.grpBarMenu = new Syncfusion.Windows.Forms.Tools.GroupBar();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.rdoUnit = new System.Windows.Forms.RadioButton();
            this.rdoDiscGoup = new System.Windows.Forms.RadioButton();
            this.rdoColor = new System.Windows.Forms.RadioButton();
            this.rdoSize = new System.Windows.Forms.RadioButton();
            this.rdoBrand = new System.Windows.Forms.RadioButton();
            this.rdoManufacturer = new System.Windows.Forms.RadioButton();
            this.rdoCategories = new System.Windows.Forms.RadioButton();
            this.treeView2 = new System.Windows.Forms.TreeView();
            this.gbiMaster = new Syncfusion.Windows.Forms.Tools.GroupBarItem();
            this.gbiTransaction = new Syncfusion.Windows.Forms.Tools.GroupBarItem();
            this.gbiAccounts = new Syncfusion.Windows.Forms.Tools.GroupBarItem();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridGroupingControlSearch)).BeginInit();
            this.tableLayoutPanel2.SuspendLayout();
            this.tlpMain.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grpBarMenu)).BeginInit();
            this.grpBarMenu.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.panel2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.gridGroupingControlSearch, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 77F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 604F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1218, 920);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.groupBox1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(3, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1212, 71);
            this.panel2.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.comboBox1);
            this.groupBox1.Controls.Add(this.dtpTo);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.dtpFrom);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1212, 71);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Filter";
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "500",
            "1000",
            "2000",
            "5000"});
            this.comboBox1.Location = new System.Drawing.Point(6, 38);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(116, 24);
            this.comboBox1.TabIndex = 7;
            // 
            // dtpTo
            // 
            this.dtpTo.CustomFormat = "dd-MM-yyyy";
            this.dtpTo.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpTo.Location = new System.Drawing.Point(312, 38);
            this.dtpTo.Name = "dtpTo";
            this.dtpTo.Size = new System.Drawing.Size(178, 22);
            this.dtpTo.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(308, 15);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(63, 17);
            this.label3.TabIndex = 4;
            this.label3.Text = "To Date:";
            // 
            // dtpFrom
            // 
            this.dtpFrom.CustomFormat = "dd-MM-yyyy";
            this.dtpFrom.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpFrom.Location = new System.Drawing.Point(128, 38);
            this.dtpFrom.Name = "dtpFrom";
            this.dtpFrom.Size = new System.Drawing.Size(178, 22);
            this.dtpFrom.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(124, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(78, 17);
            this.label2.TabIndex = 2;
            this.label2.Text = "From Date:";
            // 
            // gridGroupingControlSearch
            // 
            this.gridGroupingControlSearch.AlphaBlendSelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(215)))));
            this.gridGroupingControlSearch.BackColor = System.Drawing.SystemColors.Window;
            this.gridGroupingControlSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridGroupingControlSearch.FreezeCaption = true;
            this.gridGroupingControlSearch.Location = new System.Drawing.Point(3, 80);
            this.gridGroupingControlSearch.Name = "gridGroupingControlSearch";
            this.gridGroupingControlSearch.ShowCurrentCellBorderBehavior = Syncfusion.Windows.Forms.Grid.GridShowCurrentCellBorder.GrayWhenLostFocus;
            this.gridGroupingControlSearch.ShowGroupDropArea = true;
            this.gridGroupingControlSearch.Size = new System.Drawing.Size(1212, 837);
            this.gridGroupingControlSearch.TabIndex = 1;
            this.gridGroupingControlSearch.TableOptions.AllowDragColumns = true;
            this.gridGroupingControlSearch.TableOptions.AllowDropDownCell = true;
            this.gridGroupingControlSearch.TableOptions.AllowMultiColumnSort = true;
            this.gridGroupingControlSearch.TableOptions.AllowSelection = Syncfusion.Windows.Forms.Grid.GridSelectionFlags.Row;
            this.gridGroupingControlSearch.Text = "gridGroupingControl1";
            this.gridGroupingControlSearch.UseRightToLeftCompatibleTextBox = true;
            this.gridGroupingControlSearch.VersionInfo = "19.2460.0.44";
            this.gridGroupingControlSearch.QueryCellStyleInfo += new Syncfusion.Windows.Forms.Grid.Grouping.GridTableCellStyleInfoEventHandler(this.gridGroupingControlSearch_QueryCellStyleInfo);
            this.gridGroupingControlSearch.FilterBarSelectedItemChanging += new Syncfusion.Windows.Forms.Grid.Grouping.FilterBarSelectedItemChangingEventHandler(this.gridGroupingControlSearch_FilterBarSelectedItemChanging);
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.BackColor = System.Drawing.Color.DimGray;
            this.tableLayoutPanel2.ColumnCount = 8;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 70F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanel2.Controls.Add(this.btnCancelDeactive, 3, 0);
            this.tableLayoutPanel2.Controls.Add(this.label4, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.btnNew, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.btnClose, 7, 0);
            this.tableLayoutPanel2.Controls.Add(this.btnMinimize, 6, 0);
            this.tableLayoutPanel2.Controls.Add(this.btnMenu, 5, 0);
            this.tableLayoutPanel2.Controls.Add(this.btnYtubeTutorial, 4, 2);
            this.tableLayoutPanel2.Controls.Add(this.label7, 3, 2);
            this.tableLayoutPanel2.Controls.Add(this.btnEdit, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.label6, 1, 2);
            this.tableLayoutPanel2.Controls.Add(this.btnDelete, 2, 0);
            this.tableLayoutPanel2.Controls.Add(this.lblHeading, 4, 0);
            this.tableLayoutPanel2.Controls.Add(this.label5, 2, 2);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 3;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(1460, 98);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // btnCancelDeactive
            // 
            this.btnCancelDeactive.FlatAppearance.BorderSize = 0;
            this.btnCancelDeactive.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancelDeactive.Image = ((System.Drawing.Image)(resources.GetObject("btnCancelDeactive.Image")));
            this.btnCancelDeactive.Location = new System.Drawing.Point(183, 3);
            this.btnCancelDeactive.Name = "btnCancelDeactive";
            this.tableLayoutPanel2.SetRowSpan(this.btnCancelDeactive, 2);
            this.btnCancelDeactive.Size = new System.Drawing.Size(64, 62);
            this.btnCancelDeactive.TabIndex = 11;
            this.btnCancelDeactive.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(3, 68);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(35, 17);
            this.label4.TabIndex = 7;
            this.label4.Text = "New";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnNew
            // 
            this.btnNew.FlatAppearance.BorderSize = 0;
            this.btnNew.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNew.Image = ((System.Drawing.Image)(resources.GetObject("btnNew.Image")));
            this.btnNew.Location = new System.Drawing.Point(3, 3);
            this.btnNew.Name = "btnNew";
            this.tableLayoutPanel2.SetRowSpan(this.btnNew, 2);
            this.btnNew.Size = new System.Drawing.Size(54, 62);
            this.btnNew.TabIndex = 6;
            this.btnNew.UseVisualStyleBackColor = true;
            // 
            // btnClose
            // 
            this.btnClose.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnClose.FlatAppearance.BorderSize = 0;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Image = ((System.Drawing.Image)(resources.GetObject("btnClose.Image")));
            this.btnClose.Location = new System.Drawing.Point(1403, 3);
            this.btnClose.Name = "btnClose";
            this.tableLayoutPanel2.SetRowSpan(this.btnClose, 2);
            this.btnClose.Size = new System.Drawing.Size(54, 62);
            this.btnClose.TabIndex = 6;
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnMinimize
            // 
            this.btnMinimize.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnMinimize.FlatAppearance.BorderSize = 0;
            this.btnMinimize.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMinimize.Image = ((System.Drawing.Image)(resources.GetObject("btnMinimize.Image")));
            this.btnMinimize.Location = new System.Drawing.Point(1343, 3);
            this.btnMinimize.Name = "btnMinimize";
            this.tableLayoutPanel2.SetRowSpan(this.btnMinimize, 2);
            this.btnMinimize.Size = new System.Drawing.Size(54, 62);
            this.btnMinimize.TabIndex = 6;
            this.btnMinimize.UseVisualStyleBackColor = true;
            this.btnMinimize.Click += new System.EventHandler(this.btnMinimize_Click);
            // 
            // btnMenu
            // 
            this.btnMenu.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnMenu.FlatAppearance.BorderSize = 0;
            this.btnMenu.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMenu.Image = ((System.Drawing.Image)(resources.GetObject("btnMenu.Image")));
            this.btnMenu.Location = new System.Drawing.Point(1283, 3);
            this.btnMenu.Name = "btnMenu";
            this.tableLayoutPanel2.SetRowSpan(this.btnMenu, 2);
            this.btnMenu.Size = new System.Drawing.Size(54, 62);
            this.btnMenu.TabIndex = 6;
            this.btnMenu.UseVisualStyleBackColor = true;
            this.btnMenu.Visible = false;
            // 
            // btnYtubeTutorial
            // 
            this.btnYtubeTutorial.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnYtubeTutorial.FlatAppearance.BorderSize = 0;
            this.btnYtubeTutorial.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnYtubeTutorial.ForeColor = System.Drawing.Color.White;
            this.btnYtubeTutorial.Image = ((System.Drawing.Image)(resources.GetObject("btnYtubeTutorial.Image")));
            this.btnYtubeTutorial.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnYtubeTutorial.Location = new System.Drawing.Point(253, 71);
            this.btnYtubeTutorial.Name = "btnYtubeTutorial";
            this.btnYtubeTutorial.Size = new System.Drawing.Size(186, 24);
            this.btnYtubeTutorial.TabIndex = 10;
            this.btnYtubeTutorial.Text = "Tutorial";
            this.btnYtubeTutorial.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.ForeColor = System.Drawing.Color.White;
            this.label7.Location = new System.Drawing.Point(183, 68);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(51, 17);
            this.label7.TabIndex = 12;
            this.label7.Text = "Cancel";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnEdit
            // 
            this.btnEdit.FlatAppearance.BorderSize = 0;
            this.btnEdit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEdit.Image = ((System.Drawing.Image)(resources.GetObject("btnEdit.Image")));
            this.btnEdit.Location = new System.Drawing.Point(63, 3);
            this.btnEdit.Name = "btnEdit";
            this.tableLayoutPanel2.SetRowSpan(this.btnEdit, 2);
            this.btnEdit.Size = new System.Drawing.Size(54, 62);
            this.btnEdit.TabIndex = 6;
            this.btnEdit.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.Color.White;
            this.label6.Location = new System.Drawing.Point(63, 68);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(32, 17);
            this.label6.TabIndex = 7;
            this.label6.Text = "Edit";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnDelete
            // 
            this.btnDelete.FlatAppearance.BorderSize = 0;
            this.btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDelete.Image = ((System.Drawing.Image)(resources.GetObject("btnDelete.Image")));
            this.btnDelete.Location = new System.Drawing.Point(123, 3);
            this.btnDelete.Name = "btnDelete";
            this.tableLayoutPanel2.SetRowSpan(this.btnDelete, 2);
            this.btnDelete.Size = new System.Drawing.Size(54, 62);
            this.btnDelete.TabIndex = 6;
            this.btnDelete.UseVisualStyleBackColor = true;
            // 
            // lblHeading
            // 
            this.lblHeading.AutoSize = true;
            this.lblHeading.BackColor = System.Drawing.Color.Transparent;
            this.lblHeading.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblHeading.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHeading.ForeColor = System.Drawing.Color.White;
            this.lblHeading.Location = new System.Drawing.Point(270, 0);
            this.lblHeading.Margin = new System.Windows.Forms.Padding(20, 0, 20, 0);
            this.lblHeading.Name = "lblHeading";
            this.lblHeading.Size = new System.Drawing.Size(990, 30);
            this.lblHeading.TabIndex = 2;
            this.lblHeading.Text = "Search Window";
            this.lblHeading.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(123, 68);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(49, 17);
            this.label5.TabIndex = 7;
            this.label5.Text = "Delete";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // toolTipManufacturer
            // 
            this.toolTipManufacturer.BackColor = System.Drawing.Color.DimGray;
            this.toolTipManufacturer.ForeColor = System.Drawing.Color.White;
            this.toolTipManufacturer.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            // 
            // label47
            // 
            this.label47.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label47.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label47.ForeColor = System.Drawing.Color.Silver;
            this.label47.Location = new System.Drawing.Point(3, 1024);
            this.label47.Name = "label47";
            this.label47.Size = new System.Drawing.Size(1454, 20);
            this.label47.TabIndex = 4;
            this.label47.Text = "Keyboard Shortcuts : - F3 Find, F7 Delete, Esc Close ";
            this.label47.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tlpMain
            // 
            this.tlpMain.BackColor = System.Drawing.Color.DimGray;
            this.tlpMain.ColumnCount = 1;
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 1460F));
            this.tlpMain.Controls.Add(this.tableLayoutPanel2, 0, 0);
            this.tlpMain.Controls.Add(this.label47, 0, 2);
            this.tlpMain.Controls.Add(this.panel1, 0, 1);
            this.tlpMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpMain.Location = new System.Drawing.Point(0, 0);
            this.tlpMain.Name = "tlpMain";
            this.tlpMain.RowCount = 3;
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 98F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpMain.Size = new System.Drawing.Size(1460, 1044);
            this.tlpMain.TabIndex = 5;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.splitContainer1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 101);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1454, 920);
            this.panel1.TabIndex = 5;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.BackColor = System.Drawing.Color.DimGray;
            this.splitContainer1.Panel1.Controls.Add(this.grpBarMenu);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tableLayoutPanel1);
            this.splitContainer1.Size = new System.Drawing.Size(1454, 920);
            this.splitContainer1.SplitterDistance = 232;
            this.splitContainer1.TabIndex = 2;
            // 
            // grpBarMenu
            // 
            this.grpBarMenu.AllowDrop = true;
            this.grpBarMenu.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.grpBarMenu.BeforeTouchSize = new System.Drawing.Size(232, 920);
            this.grpBarMenu.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.grpBarMenu.CollapseImage = ((System.Drawing.Image)(resources.GetObject("grpBarMenu.CollapseImage")));
            this.grpBarMenu.Controls.Add(this.tableLayoutPanel3);
            this.grpBarMenu.Controls.Add(this.treeView2);
            this.grpBarMenu.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpBarMenu.ExpandButtonToolTip = null;
            this.grpBarMenu.ExpandImage = ((System.Drawing.Image)(resources.GetObject("grpBarMenu.ExpandImage")));
            this.grpBarMenu.FlatLook = true;
            this.grpBarMenu.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F);
            this.grpBarMenu.ForeColor = System.Drawing.Color.Black;
            this.grpBarMenu.GroupBarDropDownToolTip = null;
            this.grpBarMenu.GroupBarItems.AddRange(new Syncfusion.Windows.Forms.Tools.GroupBarItem[] {
            this.gbiMaster,
            this.gbiTransaction,
            this.gbiAccounts});
            this.grpBarMenu.IndexOnVisibleItems = true;
            this.grpBarMenu.Location = new System.Drawing.Point(0, 0);
            this.grpBarMenu.MinimizeButtonToolTip = null;
            this.grpBarMenu.Name = "grpBarMenu";
            this.grpBarMenu.NavigationPaneTooltip = null;
            this.grpBarMenu.PopupClientSize = new System.Drawing.Size(0, 0);
            this.grpBarMenu.SelectedItem = 0;
            this.grpBarMenu.ShowItemImageInHeader = true;
            this.grpBarMenu.Size = new System.Drawing.Size(232, 920);
            this.grpBarMenu.SmartSizeBox = false;
            this.grpBarMenu.Splittercolor = System.Drawing.SystemColors.ControlDark;
            this.grpBarMenu.TabIndex = 0;
            this.grpBarMenu.ThemeName = "OfficeXP";
            this.grpBarMenu.ThemeStyle.CollapsedViewStyle.ItemStyle.SelectedForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.grpBarMenu.ThemeStyle.ItemStyle.SelectedForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.grpBarMenu.ThemeStyle.StackedViewStyle.CollapsedItemStyle.SelectedForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.BackColor = System.Drawing.Color.DimGray;
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 15F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 213F));
            this.tableLayoutPanel3.Controls.Add(this.rdoUnit, 1, 6);
            this.tableLayoutPanel3.Controls.Add(this.rdoDiscGoup, 1, 5);
            this.tableLayoutPanel3.Controls.Add(this.rdoColor, 1, 4);
            this.tableLayoutPanel3.Controls.Add(this.rdoSize, 1, 3);
            this.tableLayoutPanel3.Controls.Add(this.rdoBrand, 1, 2);
            this.tableLayoutPanel3.Controls.Add(this.rdoManufacturer, 1, 1);
            this.tableLayoutPanel3.Controls.Add(this.rdoCategories, 1, 0);
            this.tableLayoutPanel3.Location = new System.Drawing.Point(2, 29);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 10;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(228, 835);
            this.tableLayoutPanel3.TabIndex = 2;
            // 
            // rdoUnit
            // 
            this.rdoUnit.Appearance = System.Windows.Forms.Appearance.Button;
            this.rdoUnit.AutoSize = true;
            this.rdoUnit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rdoUnit.FlatAppearance.BorderSize = 0;
            this.rdoUnit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rdoUnit.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rdoUnit.ForeColor = System.Drawing.Color.White;
            this.rdoUnit.Image = ((System.Drawing.Image)(resources.GetObject("rdoUnit.Image")));
            this.rdoUnit.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.rdoUnit.Location = new System.Drawing.Point(18, 483);
            this.rdoUnit.Name = "rdoUnit";
            this.rdoUnit.Size = new System.Drawing.Size(207, 74);
            this.rdoUnit.TabIndex = 15;
            this.rdoUnit.Text = "Unit Master";
            this.rdoUnit.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rdoUnit.UseVisualStyleBackColor = true;
            // 
            // rdoDiscGoup
            // 
            this.rdoDiscGoup.Appearance = System.Windows.Forms.Appearance.Button;
            this.rdoDiscGoup.AutoSize = true;
            this.rdoDiscGoup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rdoDiscGoup.FlatAppearance.BorderSize = 0;
            this.rdoDiscGoup.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rdoDiscGoup.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rdoDiscGoup.ForeColor = System.Drawing.Color.White;
            this.rdoDiscGoup.Image = ((System.Drawing.Image)(resources.GetObject("rdoDiscGoup.Image")));
            this.rdoDiscGoup.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.rdoDiscGoup.Location = new System.Drawing.Point(18, 403);
            this.rdoDiscGoup.Name = "rdoDiscGoup";
            this.rdoDiscGoup.Size = new System.Drawing.Size(207, 74);
            this.rdoDiscGoup.TabIndex = 14;
            this.rdoDiscGoup.Text = "Disc Group";
            this.rdoDiscGoup.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rdoDiscGoup.UseVisualStyleBackColor = true;
            this.rdoDiscGoup.Click += new System.EventHandler(this.rdoDiscGoup_Click);
            // 
            // rdoColor
            // 
            this.rdoColor.Appearance = System.Windows.Forms.Appearance.Button;
            this.rdoColor.AutoSize = true;
            this.rdoColor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rdoColor.FlatAppearance.BorderSize = 0;
            this.rdoColor.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rdoColor.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rdoColor.ForeColor = System.Drawing.Color.White;
            this.rdoColor.Image = ((System.Drawing.Image)(resources.GetObject("rdoColor.Image")));
            this.rdoColor.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.rdoColor.Location = new System.Drawing.Point(18, 323);
            this.rdoColor.Name = "rdoColor";
            this.rdoColor.Size = new System.Drawing.Size(207, 74);
            this.rdoColor.TabIndex = 13;
            this.rdoColor.Text = "Color Master";
            this.rdoColor.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rdoColor.UseVisualStyleBackColor = true;
            this.rdoColor.Click += new System.EventHandler(this.rdoColor_Click);
            // 
            // rdoSize
            // 
            this.rdoSize.Appearance = System.Windows.Forms.Appearance.Button;
            this.rdoSize.AutoSize = true;
            this.rdoSize.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rdoSize.FlatAppearance.BorderSize = 0;
            this.rdoSize.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rdoSize.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rdoSize.ForeColor = System.Drawing.Color.White;
            this.rdoSize.Image = ((System.Drawing.Image)(resources.GetObject("rdoSize.Image")));
            this.rdoSize.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.rdoSize.Location = new System.Drawing.Point(18, 243);
            this.rdoSize.Name = "rdoSize";
            this.rdoSize.Size = new System.Drawing.Size(207, 74);
            this.rdoSize.TabIndex = 12;
            this.rdoSize.Text = "Size Master";
            this.rdoSize.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rdoSize.UseVisualStyleBackColor = true;
            this.rdoSize.Click += new System.EventHandler(this.rdoSize_Click);
            // 
            // rdoBrand
            // 
            this.rdoBrand.Appearance = System.Windows.Forms.Appearance.Button;
            this.rdoBrand.AutoSize = true;
            this.rdoBrand.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rdoBrand.FlatAppearance.BorderSize = 0;
            this.rdoBrand.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rdoBrand.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rdoBrand.ForeColor = System.Drawing.Color.White;
            this.rdoBrand.Image = ((System.Drawing.Image)(resources.GetObject("rdoBrand.Image")));
            this.rdoBrand.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.rdoBrand.Location = new System.Drawing.Point(18, 163);
            this.rdoBrand.Name = "rdoBrand";
            this.rdoBrand.Size = new System.Drawing.Size(207, 74);
            this.rdoBrand.TabIndex = 11;
            this.rdoBrand.Text = "Brand Master";
            this.rdoBrand.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rdoBrand.UseVisualStyleBackColor = true;
            this.rdoBrand.Click += new System.EventHandler(this.rdoBrand_Click);
            // 
            // rdoManufacturer
            // 
            this.rdoManufacturer.Appearance = System.Windows.Forms.Appearance.Button;
            this.rdoManufacturer.AutoSize = true;
            this.rdoManufacturer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rdoManufacturer.FlatAppearance.BorderSize = 0;
            this.rdoManufacturer.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rdoManufacturer.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rdoManufacturer.ForeColor = System.Drawing.Color.White;
            this.rdoManufacturer.Image = ((System.Drawing.Image)(resources.GetObject("rdoManufacturer.Image")));
            this.rdoManufacturer.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.rdoManufacturer.Location = new System.Drawing.Point(18, 83);
            this.rdoManufacturer.Name = "rdoManufacturer";
            this.rdoManufacturer.Size = new System.Drawing.Size(207, 74);
            this.rdoManufacturer.TabIndex = 10;
            this.rdoManufacturer.Text = "Manufacturer";
            this.rdoManufacturer.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rdoManufacturer.UseVisualStyleBackColor = true;
            this.rdoManufacturer.Click += new System.EventHandler(this.rdoManufacturer_Click);
            // 
            // rdoCategories
            // 
            this.rdoCategories.Appearance = System.Windows.Forms.Appearance.Button;
            this.rdoCategories.AutoSize = true;
            this.rdoCategories.Checked = true;
            this.rdoCategories.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rdoCategories.FlatAppearance.BorderSize = 0;
            this.rdoCategories.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rdoCategories.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rdoCategories.ForeColor = System.Drawing.Color.White;
            this.rdoCategories.Image = ((System.Drawing.Image)(resources.GetObject("rdoCategories.Image")));
            this.rdoCategories.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.rdoCategories.Location = new System.Drawing.Point(18, 3);
            this.rdoCategories.Name = "rdoCategories";
            this.rdoCategories.Size = new System.Drawing.Size(207, 74);
            this.rdoCategories.TabIndex = 10;
            this.rdoCategories.TabStop = true;
            this.rdoCategories.Text = "Category";
            this.rdoCategories.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rdoCategories.UseVisualStyleBackColor = true;
            this.rdoCategories.Click += new System.EventHandler(this.rdoCategories_Click);
            // 
            // treeView2
            // 
            this.treeView2.Location = new System.Drawing.Point(2, 56);
            this.treeView2.Name = "treeView2";
            this.treeView2.Size = new System.Drawing.Size(228, 862);
            this.treeView2.TabIndex = 1;
            // 
            // gbiMaster
            // 
            this.gbiMaster.BackColor = System.Drawing.Color.DimGray;
            this.gbiMaster.Client = this.tableLayoutPanel3;
            this.gbiMaster.ClientBorderColors = new Syncfusion.Windows.Forms.Tools.BorderColors(System.Drawing.SystemColors.ControlDarkDark, System.Drawing.SystemColors.ControlDarkDark, System.Drawing.Color.DimGray, System.Drawing.Color.DimGray);
            this.gbiMaster.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbiMaster.ForeColor = System.Drawing.Color.White;
            this.gbiMaster.LargeImageMode = true;
            this.gbiMaster.Padding = 3;
            this.gbiMaster.Text = "Master";
            // 
            // gbiTransaction
            // 
            this.gbiTransaction.BackColor = System.Drawing.Color.DimGray;
            this.gbiTransaction.Client = this.treeView2;
            this.gbiTransaction.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbiTransaction.ForeColor = System.Drawing.Color.White;
            this.gbiTransaction.Icon = ((System.Drawing.Icon)(resources.GetObject("gbiTransaction.Icon")));
            this.gbiTransaction.Padding = 3;
            this.gbiTransaction.Text = "Transaction";
            // 
            // gbiAccounts
            // 
            this.gbiAccounts.BackColor = System.Drawing.Color.DimGray;
            this.gbiAccounts.Client = null;
            this.gbiAccounts.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbiAccounts.ForeColor = System.Drawing.Color.White;
            this.gbiAccounts.Icon = ((System.Drawing.Icon)(resources.GetObject("gbiAccounts.Icon")));
            this.gbiAccounts.Padding = 3;
            this.gbiAccounts.Text = "Accounts";
            // 
            // frmEditForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1460, 1044);
            this.Controls.Add(this.tlpMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmEditForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmEditForm";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.frmEditForm_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridGroupingControlSearch)).EndInit();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.tlpMain.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grpBarMenu)).EndInit();
            this.grpBarMenu.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.RadioButton rdoColor;
        private System.Windows.Forms.RadioButton rdoSize;
        private System.Windows.Forms.RadioButton rdoBrand;
        private System.Windows.Forms.RadioButton rdoManufacturer;
        private System.Windows.Forms.RadioButton rdoCategories;
        private System.Windows.Forms.TreeView treeView2;
        private Syncfusion.Windows.Forms.Tools.GroupBarItem gbiMaster;
        private Syncfusion.Windows.Forms.Tools.GroupBarItem gbiTransaction;
        private Syncfusion.Windows.Forms.Tools.GroupBarItem gbiAccounts;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.DateTimePicker dtpTo;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DateTimePicker dtpFrom;
        private System.Windows.Forms.Label label2;
        private Syncfusion.Windows.Forms.Grid.Grouping.GridGroupingControl gridGroupingControlSearch;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Button btnCancelDeactive;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnNew;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnMinimize;
        private System.Windows.Forms.Button btnMenu;
        private System.Windows.Forms.Button btnYtubeTutorial;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button btnEdit;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Label lblHeading;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ToolTip toolTipManufacturer;
        private System.Windows.Forms.Label label47;
        private System.Windows.Forms.TableLayoutPanel tlpMain;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private Syncfusion.Windows.Forms.Tools.GroupBar grpBarMenu;
        private System.Windows.Forms.RadioButton rdoUnit;
        private System.Windows.Forms.RadioButton rdoDiscGoup;
    }
}