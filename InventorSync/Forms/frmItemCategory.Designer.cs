namespace DigiposZen
{
    partial class frmItemCategory
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmItemCategory));
            this.lblCategoryName = new Syncfusion.Windows.Forms.Tools.AutoLabel();
            this.txtCategoryName = new System.Windows.Forms.TextBox();
            this.lblParentCategory = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtDiscountPerc = new System.Windows.Forms.TextBox();
            this.txtRemarks = new System.Windows.Forms.TextBox();
            this.tlpMain = new System.Windows.Forms.TableLayoutPanel();
            this.tlpHeader = new System.Windows.Forms.TableLayoutPanel();
            this.lblSave = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.lblHeading = new System.Windows.Forms.Label();
            this.lblDelete = new System.Windows.Forms.Label();
            this.btnFind = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnMinimize = new System.Windows.Forms.Button();
            this.btnYtubeTutorial = new System.Windows.Forms.Button();
            this.lblFind = new System.Windows.Forms.Label();
            this.gboxMain = new System.Windows.Forms.Panel();
            this.lblMand1 = new System.Windows.Forms.Label();
            this.trvwParentcategory = new System.Windows.Forms.TreeView();
            this.imglistcategory = new System.Windows.Forms.ImageList(this.components);
            this.lblFooter = new System.Windows.Forms.Label();
            this.toolCategories = new System.Windows.Forms.ToolTip(this.components);
            this.picBackground = new System.Windows.Forms.PictureBox();
            this.tlpMain.SuspendLayout();
            this.tlpHeader.SuspendLayout();
            this.gboxMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picBackground)).BeginInit();
            this.SuspendLayout();
            // 
            // lblCategoryName
            // 
            this.lblCategoryName.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCategoryName.Location = new System.Drawing.Point(22, 6);
            this.lblCategoryName.Name = "lblCategoryName";
            this.lblCategoryName.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.lblCategoryName.Size = new System.Drawing.Size(130, 26);
            this.lblCategoryName.TabIndex = 4;
            this.lblCategoryName.Text = "Category Name:";
            // 
            // txtCategoryName
            // 
            this.txtCategoryName.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCategoryName.Location = new System.Drawing.Point(22, 35);
            this.txtCategoryName.MaxLength = 50;
            this.txtCategoryName.Name = "txtCategoryName";
            this.txtCategoryName.Size = new System.Drawing.Size(589, 26);
            this.txtCategoryName.TabIndex = 0;
            this.txtCategoryName.Click += new System.EventHandler(this.txtCategoryName_Click);
            this.txtCategoryName.TextChanged += new System.EventHandler(this.txtCategoryName_TextChanged);
            this.txtCategoryName.Enter += new System.EventHandler(this.txtCategoryName_Enter);
            this.txtCategoryName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox_KeyDown);
            this.txtCategoryName.Leave += new System.EventHandler(this.txtCategoryName_Leave);
            // 
            // lblParentCategory
            // 
            this.lblParentCategory.AutoSize = true;
            this.lblParentCategory.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblParentCategory.Location = new System.Drawing.Point(22, 67);
            this.lblParentCategory.Name = "lblParentCategory";
            this.lblParentCategory.Size = new System.Drawing.Size(135, 21);
            this.lblParentCategory.TabIndex = 6;
            this.lblParentCategory.Text = "Parent Category:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(22, 365);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(103, 21);
            this.label1.TabIndex = 7;
            this.label1.Text = "Discount %:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(159, 365);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(81, 21);
            this.label2.TabIndex = 8;
            this.label2.Text = "Remarks:";
            // 
            // txtDiscountPerc
            // 
            this.txtDiscountPerc.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtDiscountPerc.Location = new System.Drawing.Point(22, 389);
            this.txtDiscountPerc.MaxLength = 5;
            this.txtDiscountPerc.Name = "txtDiscountPerc";
            this.txtDiscountPerc.Size = new System.Drawing.Size(121, 26);
            this.txtDiscountPerc.TabIndex = 2;
            this.txtDiscountPerc.Click += new System.EventHandler(this.txtDiscountPerc_Click);
            this.txtDiscountPerc.Enter += new System.EventHandler(this.txtDiscountPerc_Enter);
            this.txtDiscountPerc.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtDiscountPerc_KeyDown);
            this.txtDiscountPerc.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtDiscountPerc_KeyPress);
            this.txtDiscountPerc.Leave += new System.EventHandler(this.txtDiscountPerc_Leave);
            // 
            // txtRemarks
            // 
            this.txtRemarks.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtRemarks.Location = new System.Drawing.Point(159, 389);
            this.txtRemarks.MaxLength = 50;
            this.txtRemarks.Name = "txtRemarks";
            this.txtRemarks.Size = new System.Drawing.Size(452, 26);
            this.txtRemarks.TabIndex = 3;
            this.txtRemarks.Click += new System.EventHandler(this.txtRemarks_Click);
            this.txtRemarks.Enter += new System.EventHandler(this.txtRemarks_Enter);
            this.txtRemarks.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtRemarks_KeyDown);
            this.txtRemarks.Leave += new System.EventHandler(this.txtRemarks_Leave);
            // 
            // tlpMain
            // 
            this.tlpMain.BackColor = System.Drawing.Color.WhiteSmoke;
            this.tlpMain.ColumnCount = 1;
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 669F));
            this.tlpMain.Controls.Add(this.tlpHeader, 0, 0);
            this.tlpMain.Controls.Add(this.gboxMain, 0, 1);
            this.tlpMain.Controls.Add(this.lblFooter, 0, 2);
            this.tlpMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpMain.Location = new System.Drawing.Point(1, 1);
            this.tlpMain.Name = "tlpMain";
            this.tlpMain.RowCount = 3;
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 81F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpMain.Size = new System.Drawing.Size(669, 531);
            this.tlpMain.TabIndex = 3;
            // 
            // tlpHeader
            // 
            this.tlpHeader.BackColor = System.Drawing.Color.Transparent;
            this.tlpHeader.ColumnCount = 6;
            this.tlpHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 66F));
            this.tlpHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 71F));
            this.tlpHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 67F));
            this.tlpHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 57F));
            this.tlpHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 67F));
            this.tlpHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpHeader.Controls.Add(this.lblSave, 0, 2);
            this.tlpHeader.Controls.Add(this.btnSave, 0, 0);
            this.tlpHeader.Controls.Add(this.btnDelete, 1, 0);
            this.tlpHeader.Controls.Add(this.lblHeading, 3, 0);
            this.tlpHeader.Controls.Add(this.lblDelete, 1, 2);
            this.tlpHeader.Controls.Add(this.btnFind, 2, 0);
            this.tlpHeader.Controls.Add(this.btnClose, 5, 0);
            this.tlpHeader.Controls.Add(this.btnMinimize, 4, 0);
            this.tlpHeader.Controls.Add(this.btnYtubeTutorial, 4, 2);
            this.tlpHeader.Controls.Add(this.lblFind, 2, 2);
            this.tlpHeader.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpHeader.Location = new System.Drawing.Point(0, 0);
            this.tlpHeader.Margin = new System.Windows.Forms.Padding(0);
            this.tlpHeader.Name = "tlpHeader";
            this.tlpHeader.RowCount = 3;
            this.tlpHeader.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tlpHeader.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27F));
            this.tlpHeader.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 41F));
            this.tlpHeader.Size = new System.Drawing.Size(669, 81);
            this.tlpHeader.TabIndex = 0;
            this.tlpHeader.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tlpHeader_MouseDown);
            this.tlpHeader.MouseMove += new System.Windows.Forms.MouseEventHandler(this.tlpHeader_MouseMove);
            this.tlpHeader.MouseUp += new System.Windows.Forms.MouseEventHandler(this.tlpHeader_MouseUp);
            // 
            // lblSave
            // 
            this.lblSave.BackColor = System.Drawing.Color.Transparent;
            this.lblSave.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSave.ForeColor = System.Drawing.Color.Black;
            this.lblSave.Location = new System.Drawing.Point(3, 57);
            this.lblSave.Name = "lblSave";
            this.lblSave.Size = new System.Drawing.Size(54, 18);
            this.lblSave.TabIndex = 7;
            this.lblSave.Text = "Save";
            this.lblSave.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnSave
            // 
            this.btnSave.BackColor = System.Drawing.Color.Transparent;
            this.btnSave.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnSave.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnSave.FlatAppearance.BorderSize = 0;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Image = global::DigiposZen.Properties.Resources.save240402;
            this.btnSave.Location = new System.Drawing.Point(3, 3);
            this.btnSave.Name = "btnSave";
            this.tlpHeader.SetRowSpan(this.btnSave, 2);
            this.btnSave.Size = new System.Drawing.Size(60, 51);
            this.btnSave.TabIndex = 4;
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.BackColor = System.Drawing.Color.Transparent;
            this.btnDelete.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnDelete.FlatAppearance.BorderSize = 0;
            this.btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDelete.Image = global::DigiposZen.Properties.Resources.delete340402;
            this.btnDelete.Location = new System.Drawing.Point(69, 3);
            this.btnDelete.Name = "btnDelete";
            this.tlpHeader.SetRowSpan(this.btnDelete, 2);
            this.btnDelete.Size = new System.Drawing.Size(65, 51);
            this.btnDelete.TabIndex = 5;
            this.btnDelete.UseVisualStyleBackColor = false;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // lblHeading
            // 
            this.lblHeading.AutoSize = true;
            this.lblHeading.BackColor = System.Drawing.Color.Transparent;
            this.lblHeading.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblHeading.Font = new System.Drawing.Font("Tahoma", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHeading.ForeColor = System.Drawing.Color.Black;
            this.lblHeading.Location = new System.Drawing.Point(224, 0);
            this.lblHeading.Margin = new System.Windows.Forms.Padding(20, 0, 20, 0);
            this.lblHeading.Name = "lblHeading";
            this.tlpHeader.SetRowSpan(this.lblHeading, 2);
            this.lblHeading.Size = new System.Drawing.Size(301, 57);
            this.lblHeading.TabIndex = 7;
            this.lblHeading.Text = "Category";
            this.lblHeading.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblDelete
            // 
            this.lblDelete.BackColor = System.Drawing.Color.Transparent;
            this.lblDelete.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDelete.ForeColor = System.Drawing.Color.Black;
            this.lblDelete.Location = new System.Drawing.Point(69, 57);
            this.lblDelete.Name = "lblDelete";
            this.lblDelete.Size = new System.Drawing.Size(63, 18);
            this.lblDelete.TabIndex = 7;
            this.lblDelete.Text = "Delete";
            this.lblDelete.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnFind
            // 
            this.btnFind.BackColor = System.Drawing.Color.Transparent;
            this.btnFind.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnFind.FlatAppearance.BorderSize = 0;
            this.btnFind.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFind.Image = global::DigiposZen.Properties.Resources.find_finalised_3030;
            this.btnFind.Location = new System.Drawing.Point(140, 3);
            this.btnFind.Name = "btnFind";
            this.tlpHeader.SetRowSpan(this.btnFind, 2);
            this.btnFind.Size = new System.Drawing.Size(61, 51);
            this.btnFind.TabIndex = 6;
            this.btnFind.UseVisualStyleBackColor = false;
            this.btnFind.Click += new System.EventHandler(this.btnFind_Click);
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.Color.Transparent;
            this.btnClose.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnClose.FlatAppearance.BorderSize = 0;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Image = global::DigiposZen.Properties.Resources.logout_Final;
            this.btnClose.Location = new System.Drawing.Point(605, 3);
            this.btnClose.Name = "btnClose";
            this.tlpHeader.SetRowSpan(this.btnClose, 2);
            this.btnClose.Size = new System.Drawing.Size(61, 51);
            this.btnClose.TabIndex = 9;
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnMinimize
            // 
            this.btnMinimize.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnMinimize.FlatAppearance.BorderSize = 0;
            this.btnMinimize.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMinimize.Image = global::DigiposZen.Properties.Resources.minimize_finalised;
            this.btnMinimize.Location = new System.Drawing.Point(548, 3);
            this.btnMinimize.Name = "btnMinimize";
            this.tlpHeader.SetRowSpan(this.btnMinimize, 2);
            this.btnMinimize.Size = new System.Drawing.Size(51, 51);
            this.btnMinimize.TabIndex = 8;
            this.btnMinimize.UseVisualStyleBackColor = true;
            this.btnMinimize.Click += new System.EventHandler(this.btnMinimize_Click);
            // 
            // btnYtubeTutorial
            // 
            this.btnYtubeTutorial.BackColor = System.Drawing.Color.Transparent;
            this.btnYtubeTutorial.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.tlpHeader.SetColumnSpan(this.btnYtubeTutorial, 2);
            this.btnYtubeTutorial.FlatAppearance.BorderSize = 0;
            this.btnYtubeTutorial.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnYtubeTutorial.Font = new System.Drawing.Font("Tahoma", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnYtubeTutorial.ForeColor = System.Drawing.Color.Black;
            this.btnYtubeTutorial.Image = ((System.Drawing.Image)(resources.GetObject("btnYtubeTutorial.Image")));
            this.btnYtubeTutorial.ImageAlign = System.Drawing.ContentAlignment.TopRight;
            this.btnYtubeTutorial.Location = new System.Drawing.Point(548, 60);
            this.btnYtubeTutorial.Name = "btnYtubeTutorial";
            this.btnYtubeTutorial.Size = new System.Drawing.Size(83, 24);
            this.btnYtubeTutorial.TabIndex = 10;
            this.btnYtubeTutorial.Text = "Tutorial";
            this.btnYtubeTutorial.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnYtubeTutorial.UseVisualStyleBackColor = false;
            this.btnYtubeTutorial.Visible = false;
            // 
            // lblFind
            // 
            this.lblFind.BackColor = System.Drawing.Color.Transparent;
            this.lblFind.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFind.ForeColor = System.Drawing.Color.Black;
            this.lblFind.Location = new System.Drawing.Point(140, 57);
            this.lblFind.Name = "lblFind";
            this.lblFind.Size = new System.Drawing.Size(53, 18);
            this.lblFind.TabIndex = 7;
            this.lblFind.Text = "Find";
            this.lblFind.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // gboxMain
            // 
            this.gboxMain.BackColor = System.Drawing.Color.Transparent;
            this.gboxMain.Controls.Add(this.lblMand1);
            this.gboxMain.Controls.Add(this.trvwParentcategory);
            this.gboxMain.Controls.Add(this.label2);
            this.gboxMain.Controls.Add(this.txtRemarks);
            this.gboxMain.Controls.Add(this.label1);
            this.gboxMain.Controls.Add(this.lblParentCategory);
            this.gboxMain.Controls.Add(this.txtDiscountPerc);
            this.gboxMain.Controls.Add(this.txtCategoryName);
            this.gboxMain.Controls.Add(this.lblCategoryName);
            this.gboxMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gboxMain.Location = new System.Drawing.Point(20, 81);
            this.gboxMain.Margin = new System.Windows.Forms.Padding(20, 0, 20, 0);
            this.gboxMain.Name = "gboxMain";
            this.gboxMain.Size = new System.Drawing.Size(629, 430);
            this.gboxMain.TabIndex = 0;
            // 
            // lblMand1
            // 
            this.lblMand1.AutoSize = true;
            this.lblMand1.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMand1.ForeColor = System.Drawing.Color.Red;
            this.lblMand1.Location = new System.Drawing.Point(145, 9);
            this.lblMand1.Name = "lblMand1";
            this.lblMand1.Size = new System.Drawing.Size(19, 21);
            this.lblMand1.TabIndex = 81;
            this.lblMand1.Text = "*";
            // 
            // trvwParentcategory
            // 
            this.trvwParentcategory.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.trvwParentcategory.HideSelection = false;
            this.trvwParentcategory.ImageIndex = 0;
            this.trvwParentcategory.ImageList = this.imglistcategory;
            this.trvwParentcategory.Location = new System.Drawing.Point(22, 92);
            this.trvwParentcategory.Name = "trvwParentcategory";
            this.trvwParentcategory.SelectedImageIndex = 1;
            this.trvwParentcategory.Size = new System.Drawing.Size(589, 269);
            this.trvwParentcategory.TabIndex = 1;
            this.trvwParentcategory.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.trvwParentcategory_NodeMouseDoubleClick);
            this.trvwParentcategory.Enter += new System.EventHandler(this.trvwParentcategory_Enter);
            this.trvwParentcategory.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox_KeyDown);
            this.trvwParentcategory.Leave += new System.EventHandler(this.trvwParentcategory_Leave);
            // 
            // imglistcategory
            // 
            this.imglistcategory.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imglistcategory.ImageStream")));
            this.imglistcategory.TransparentColor = System.Drawing.Color.Transparent;
            this.imglistcategory.Images.SetKeyName(0, "Node.png");
            this.imglistcategory.Images.SetKeyName(1, "select.png");
            // 
            // lblFooter
            // 
            this.lblFooter.BackColor = System.Drawing.Color.Transparent;
            this.lblFooter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblFooter.Font = new System.Drawing.Font("Tahoma", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFooter.ForeColor = System.Drawing.Color.White;
            this.lblFooter.Location = new System.Drawing.Point(3, 511);
            this.lblFooter.Name = "lblFooter";
            this.lblFooter.Size = new System.Drawing.Size(663, 20);
            this.lblFooter.TabIndex = 4;
            this.lblFooter.Text = "Keyboard Shortcuts : - F3 Find, F5 Save, F7 Delete, Esc Close ";
            this.lblFooter.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // picBackground
            // 
            this.picBackground.Location = new System.Drawing.Point(0, 0);
            this.picBackground.Name = "picBackground";
            this.picBackground.Size = new System.Drawing.Size(24, 21);
            this.picBackground.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picBackground.TabIndex = 4;
            this.picBackground.TabStop = false;
            this.picBackground.Visible = false;
            this.picBackground.LoadCompleted += new System.ComponentModel.AsyncCompletedEventHandler(this.picBackground_LoadCompleted);
            // 
            // frmItemCategory
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(671, 533);
            this.Controls.Add(this.picBackground);
            this.Controls.Add(this.tlpMain);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Tahoma", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmItemCategory";
            this.Padding = new System.Windows.Forms.Padding(1);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Category...";
            this.TopMost = true;
            this.Activated += new System.EventHandler(this.frmItemCategory_Activated);
            this.Load += new System.EventHandler(this.frmItemCategory_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmItemCategory_KeyDown);
            this.tlpMain.ResumeLayout(false);
            this.tlpHeader.ResumeLayout(false);
            this.tlpHeader.PerformLayout();
            this.gboxMain.ResumeLayout(false);
            this.gboxMain.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picBackground)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private Syncfusion.Windows.Forms.Tools.AutoLabel lblCategoryName;
        private System.Windows.Forms.Label lblParentCategory;
        private System.Windows.Forms.TextBox txtCategoryName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtDiscountPerc;
        private System.Windows.Forms.TextBox txtRemarks;
        private System.Windows.Forms.TableLayoutPanel tlpMain;
        private System.Windows.Forms.TableLayoutPanel tlpHeader;
        private System.Windows.Forms.Label lblSave;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Label lblHeading;
        private System.Windows.Forms.Label lblDelete;
        private System.Windows.Forms.Button btnFind;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label lblFind;
        private System.Windows.Forms.Button btnYtubeTutorial;
        private System.Windows.Forms.Button btnMinimize;
        private System.Windows.Forms.Panel gboxMain;
        private System.Windows.Forms.Label lblFooter;
        private System.Windows.Forms.TreeView trvwParentcategory;
        private System.Windows.Forms.ToolTip toolCategories;
        private System.Windows.Forms.ImageList imglistcategory;
        private System.Windows.Forms.Label lblMand1;
        private System.Windows.Forms.PictureBox picBackground;
    }
}