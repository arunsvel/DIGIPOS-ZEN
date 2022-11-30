namespace InventorSync
{
    partial class frmManufacturer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmManufacturer));
            this.tlpMain = new System.Windows.Forms.TableLayoutPanel();
            this.tlpHeader = new System.Windows.Forms.TableLayoutPanel();
            this.lblSave = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.lblHeading = new System.Windows.Forms.Label();
            this.lblDelete = new System.Windows.Forms.Label();
            this.btnFind = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.lblFind = new System.Windows.Forms.Label();
            this.btnYtubeTutorial = new System.Windows.Forms.Button();
            this.btnMinimize = new System.Windows.Forms.Button();
            this.gboxMain = new System.Windows.Forms.GroupBox();
            this.lblMand1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.txtManfShortName = new System.Windows.Forms.TextBox();
            this.lblManfShort = new System.Windows.Forms.Label();
            this.lblManufacture = new System.Windows.Forms.Label();
            this.txtDiscountPerc = new System.Windows.Forms.TextBox();
            this.txtManufacture = new System.Windows.Forms.TextBox();
            this.lblManfDisc = new System.Windows.Forms.Label();
            this.lblFooter = new System.Windows.Forms.Label();
            this.toolTipManufacturer = new System.Windows.Forms.ToolTip(this.components);
            this.tlpMain.SuspendLayout();
            this.tlpHeader.SuspendLayout();
            this.gboxMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // tlpMain
            // 
            this.tlpMain.BackColor = System.Drawing.Color.WhiteSmoke;
            this.tlpMain.ColumnCount = 1;
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 656F));
            this.tlpMain.Controls.Add(this.tlpHeader, 0, 0);
            this.tlpMain.Controls.Add(this.gboxMain, 0, 1);
            this.tlpMain.Controls.Add(this.lblFooter, 0, 2);
            this.tlpMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpMain.Location = new System.Drawing.Point(1, 1);
            this.tlpMain.Name = "tlpMain";
            this.tlpMain.RowCount = 3;
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 98F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpMain.Size = new System.Drawing.Size(669, 337);
            this.tlpMain.TabIndex = 2;
            // 
            // tlpHeader
            // 
            this.tlpHeader.BackColor = System.Drawing.Color.WhiteSmoke;
            this.tlpHeader.ColumnCount = 6;
            this.tlpHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tlpHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tlpHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tlpHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tlpHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tlpHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpHeader.Controls.Add(this.lblSave, 0, 2);
            this.tlpHeader.Controls.Add(this.btnSave, 0, 0);
            this.tlpHeader.Controls.Add(this.btnDelete, 1, 0);
            this.tlpHeader.Controls.Add(this.lblHeading, 3, 0);
            this.tlpHeader.Controls.Add(this.lblDelete, 1, 2);
            this.tlpHeader.Controls.Add(this.btnFind, 2, 0);
            this.tlpHeader.Controls.Add(this.btnClose, 5, 0);
            this.tlpHeader.Controls.Add(this.lblFind, 2, 2);
            this.tlpHeader.Controls.Add(this.btnYtubeTutorial, 3, 2);
            this.tlpHeader.Controls.Add(this.btnMinimize, 4, 0);
            this.tlpHeader.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpHeader.Location = new System.Drawing.Point(0, 0);
            this.tlpHeader.Margin = new System.Windows.Forms.Padding(0);
            this.tlpHeader.Name = "tlpHeader";
            this.tlpHeader.RowCount = 3;
            this.tlpHeader.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tlpHeader.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            this.tlpHeader.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tlpHeader.Size = new System.Drawing.Size(669, 98);
            this.tlpHeader.TabIndex = 0;
            this.tlpHeader.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tlpHeader_MouseDown);
            this.tlpHeader.MouseMove += new System.Windows.Forms.MouseEventHandler(this.tlpHeader_MouseMove);
            this.tlpHeader.MouseUp += new System.Windows.Forms.MouseEventHandler(this.tlpHeader_MouseUp);
            // 
            // lblSave
            // 
            this.lblSave.AutoSize = true;
            this.lblSave.BackColor = System.Drawing.Color.Transparent;
            this.lblSave.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblSave.Font = new System.Drawing.Font("Tahoma", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSave.ForeColor = System.Drawing.Color.Black;
            this.lblSave.Location = new System.Drawing.Point(3, 68);
            this.lblSave.Name = "lblSave";
            this.lblSave.Size = new System.Drawing.Size(54, 30);
            this.lblSave.TabIndex = 7;
            this.lblSave.Text = "Save";
            this.lblSave.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnSave
            // 
            this.btnSave.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnSave.FlatAppearance.BorderSize = 0;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Image = global::InventorSync.Properties.Resources.save240402;
            this.btnSave.Location = new System.Drawing.Point(3, 3);
            this.btnSave.Name = "btnSave";
            this.tlpHeader.SetRowSpan(this.btnSave, 2);
            this.btnSave.Size = new System.Drawing.Size(54, 62);
            this.btnSave.TabIndex = 6;
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnDelete.FlatAppearance.BorderSize = 0;
            this.btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDelete.Image = global::InventorSync.Properties.Resources.delete340402;
            this.btnDelete.Location = new System.Drawing.Point(63, 3);
            this.btnDelete.Name = "btnDelete";
            this.tlpHeader.SetRowSpan(this.btnDelete, 2);
            this.btnDelete.Size = new System.Drawing.Size(54, 62);
            this.btnDelete.TabIndex = 6;
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // lblHeading
            // 
            this.lblHeading.AutoSize = true;
            this.lblHeading.BackColor = System.Drawing.Color.Transparent;
            this.lblHeading.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblHeading.Font = new System.Drawing.Font("Tahoma", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHeading.ForeColor = System.Drawing.Color.Black;
            this.lblHeading.Location = new System.Drawing.Point(200, 0);
            this.lblHeading.Margin = new System.Windows.Forms.Padding(20, 0, 20, 0);
            this.lblHeading.Name = "lblHeading";
            this.lblHeading.Size = new System.Drawing.Size(329, 30);
            this.lblHeading.TabIndex = 2;
            this.lblHeading.Text = "Manufacturer";
            this.lblHeading.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblDelete
            // 
            this.lblDelete.AutoSize = true;
            this.lblDelete.BackColor = System.Drawing.Color.Transparent;
            this.lblDelete.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblDelete.Font = new System.Drawing.Font("Tahoma", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDelete.ForeColor = System.Drawing.Color.Black;
            this.lblDelete.Location = new System.Drawing.Point(63, 68);
            this.lblDelete.Name = "lblDelete";
            this.lblDelete.Size = new System.Drawing.Size(54, 30);
            this.lblDelete.TabIndex = 7;
            this.lblDelete.Text = "Delete";
            this.lblDelete.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnFind
            // 
            this.btnFind.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnFind.FlatAppearance.BorderSize = 0;
            this.btnFind.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFind.Image = global::InventorSync.Properties.Resources.find_finalised_3030;
            this.btnFind.Location = new System.Drawing.Point(123, 3);
            this.btnFind.Name = "btnFind";
            this.tlpHeader.SetRowSpan(this.btnFind, 2);
            this.btnFind.Size = new System.Drawing.Size(54, 62);
            this.btnFind.TabIndex = 6;
            this.btnFind.UseVisualStyleBackColor = true;
            this.btnFind.Click += new System.EventHandler(this.btnFind_Click);
            // 
            // btnClose
            // 
            this.btnClose.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnClose.FlatAppearance.BorderSize = 0;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Image = global::InventorSync.Properties.Resources.logout_Final;
            this.btnClose.Location = new System.Drawing.Point(612, 3);
            this.btnClose.Name = "btnClose";
            this.tlpHeader.SetRowSpan(this.btnClose, 2);
            this.btnClose.Size = new System.Drawing.Size(54, 62);
            this.btnClose.TabIndex = 6;
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // lblFind
            // 
            this.lblFind.AutoSize = true;
            this.lblFind.BackColor = System.Drawing.Color.Transparent;
            this.lblFind.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblFind.Font = new System.Drawing.Font("Tahoma", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFind.ForeColor = System.Drawing.Color.Black;
            this.lblFind.Location = new System.Drawing.Point(123, 68);
            this.lblFind.Name = "lblFind";
            this.lblFind.Size = new System.Drawing.Size(54, 30);
            this.lblFind.TabIndex = 7;
            this.lblFind.Text = "Find";
            this.lblFind.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnYtubeTutorial
            // 
            this.btnYtubeTutorial.BackColor = System.Drawing.Color.Transparent;
            this.btnYtubeTutorial.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnYtubeTutorial.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnYtubeTutorial.FlatAppearance.BorderSize = 0;
            this.btnYtubeTutorial.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnYtubeTutorial.Font = new System.Drawing.Font("Tahoma", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnYtubeTutorial.ForeColor = System.Drawing.Color.Black;
            this.btnYtubeTutorial.Image = ((System.Drawing.Image)(resources.GetObject("btnYtubeTutorial.Image")));
            this.btnYtubeTutorial.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnYtubeTutorial.Location = new System.Drawing.Point(183, 71);
            this.btnYtubeTutorial.Name = "btnYtubeTutorial";
            this.btnYtubeTutorial.Size = new System.Drawing.Size(106, 24);
            this.btnYtubeTutorial.TabIndex = 10;
            this.btnYtubeTutorial.Text = "Tutorial";
            this.btnYtubeTutorial.UseVisualStyleBackColor = false;
            this.btnYtubeTutorial.Visible = false;
            // 
            // btnMinimize
            // 
            this.btnMinimize.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnMinimize.FlatAppearance.BorderSize = 0;
            this.btnMinimize.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMinimize.Image = global::InventorSync.Properties.Resources.minimize_finalised;
            this.btnMinimize.Location = new System.Drawing.Point(552, 3);
            this.btnMinimize.Name = "btnMinimize";
            this.tlpHeader.SetRowSpan(this.btnMinimize, 2);
            this.btnMinimize.Size = new System.Drawing.Size(54, 62);
            this.btnMinimize.TabIndex = 6;
            this.btnMinimize.UseVisualStyleBackColor = true;
            this.btnMinimize.Click += new System.EventHandler(this.btnMinimize_Click);
            // 
            // gboxMain
            // 
            this.gboxMain.BackColor = System.Drawing.Color.White;
            this.gboxMain.Controls.Add(this.lblMand1);
            this.gboxMain.Controls.Add(this.panel1);
            this.gboxMain.Controls.Add(this.txtManfShortName);
            this.gboxMain.Controls.Add(this.lblManfShort);
            this.gboxMain.Controls.Add(this.lblManufacture);
            this.gboxMain.Controls.Add(this.txtDiscountPerc);
            this.gboxMain.Controls.Add(this.txtManufacture);
            this.gboxMain.Controls.Add(this.lblManfDisc);
            this.gboxMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gboxMain.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.gboxMain.Location = new System.Drawing.Point(20, 98);
            this.gboxMain.Margin = new System.Windows.Forms.Padding(20, 0, 20, 0);
            this.gboxMain.Name = "gboxMain";
            this.gboxMain.Size = new System.Drawing.Size(629, 219);
            this.gboxMain.TabIndex = 0;
            this.gboxMain.TabStop = false;
            // 
            // lblMand1
            // 
            this.lblMand1.AutoSize = true;
            this.lblMand1.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMand1.ForeColor = System.Drawing.Color.Red;
            this.lblMand1.Location = new System.Drawing.Point(184, 17);
            this.lblMand1.Name = "lblMand1";
            this.lblMand1.Size = new System.Drawing.Size(19, 21);
            this.lblMand1.TabIndex = 78;
            this.lblMand1.Text = "*";
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panel1.Location = new System.Drawing.Point(344, 23);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(263, 177);
            this.panel1.TabIndex = 75;
            // 
            // txtManfShortName
            // 
            this.txtManfShortName.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtManfShortName.Location = new System.Drawing.Point(22, 98);
            this.txtManfShortName.MaxLength = 50;
            this.txtManfShortName.Name = "txtManfShortName";
            this.txtManfShortName.Size = new System.Drawing.Size(310, 28);
            this.txtManfShortName.TabIndex = 1;
            this.txtManfShortName.Click += new System.EventHandler(this.txtManfShortName_Click);
            this.txtManfShortName.Enter += new System.EventHandler(this.txtManfShortName_Enter);
            this.txtManfShortName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox_KeyDown);
            this.txtManfShortName.Leave += new System.EventHandler(this.txtManfShortName_Leave);
            // 
            // lblManfShort
            // 
            this.lblManfShort.AutoSize = true;
            this.lblManfShort.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblManfShort.Location = new System.Drawing.Point(22, 74);
            this.lblManfShort.Name = "lblManfShort";
            this.lblManfShort.Size = new System.Drawing.Size(206, 21);
            this.lblManfShort.TabIndex = 73;
            this.lblManfShort.Text = "Manufacturer Short Name:";
            // 
            // lblManufacture
            // 
            this.lblManufacture.AutoSize = true;
            this.lblManufacture.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblManufacture.Location = new System.Drawing.Point(22, 17);
            this.lblManufacture.Name = "lblManufacture";
            this.lblManufacture.Size = new System.Drawing.Size(162, 21);
            this.lblManufacture.TabIndex = 69;
            this.lblManufacture.Text = "Manufacturer Name:";
            // 
            // txtDiscountPerc
            // 
            this.txtDiscountPerc.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtDiscountPerc.Location = new System.Drawing.Point(22, 154);
            this.txtDiscountPerc.MaxLength = 5;
            this.txtDiscountPerc.Name = "txtDiscountPerc";
            this.txtDiscountPerc.Size = new System.Drawing.Size(168, 28);
            this.txtDiscountPerc.TabIndex = 2;
            this.txtDiscountPerc.Click += new System.EventHandler(this.txtDiscountPerc_Click);
            this.txtDiscountPerc.Enter += new System.EventHandler(this.txtDiscountPerc_Enter);
            this.txtDiscountPerc.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtDiscountPerc_KeyDown);
            this.txtDiscountPerc.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtDiscountPerc_KeyPress);
            this.txtDiscountPerc.Leave += new System.EventHandler(this.txtDiscountPerc_Leave);
            // 
            // txtManufacture
            // 
            this.txtManufacture.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtManufacture.Location = new System.Drawing.Point(22, 41);
            this.txtManufacture.MaxLength = 50;
            this.txtManufacture.Name = "txtManufacture";
            this.txtManufacture.Size = new System.Drawing.Size(310, 28);
            this.txtManufacture.TabIndex = 0;
            this.txtManufacture.Click += new System.EventHandler(this.txtManufacture_Click);
            this.txtManufacture.TextChanged += new System.EventHandler(this.txtManufacture_TextChanged);
            this.txtManufacture.Enter += new System.EventHandler(this.txtManufacture_Enter);
            this.txtManufacture.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtManufacture_KeyDown);
            this.txtManufacture.Leave += new System.EventHandler(this.txtManufacture_Leave);
            // 
            // lblManfDisc
            // 
            this.lblManfDisc.AutoSize = true;
            this.lblManfDisc.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblManfDisc.Location = new System.Drawing.Point(22, 131);
            this.lblManfDisc.Name = "lblManfDisc";
            this.lblManfDisc.Size = new System.Drawing.Size(103, 21);
            this.lblManfDisc.TabIndex = 71;
            this.lblManfDisc.Text = "Discount %:";
            // 
            // lblFooter
            // 
            this.lblFooter.BackColor = System.Drawing.Color.Transparent;
            this.lblFooter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblFooter.Font = new System.Drawing.Font("Tahoma", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFooter.ForeColor = System.Drawing.Color.Black;
            this.lblFooter.Location = new System.Drawing.Point(3, 317);
            this.lblFooter.Name = "lblFooter";
            this.lblFooter.Size = new System.Drawing.Size(663, 20);
            this.lblFooter.TabIndex = 4;
            this.lblFooter.Text = "Keyboard Shortcuts : - F3 Find, F5 Save, F7 Delete, Esc Close ";
            this.lblFooter.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // toolTipManufacturer
            // 
            this.toolTipManufacturer.BackColor = System.Drawing.Color.DimGray;
            this.toolTipManufacturer.ForeColor = System.Drawing.Color.White;
            this.toolTipManufacturer.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            // 
            // frmManufacturer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(671, 339);
            this.Controls.Add(this.tlpMain);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.Font = new System.Drawing.Font("Tahoma", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmManufacturer";
            this.Padding = new System.Windows.Forms.Padding(1);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Manufacture...";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.frmManufacture_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmManufacturer_KeyDown);
            this.tlpMain.ResumeLayout(false);
            this.tlpHeader.ResumeLayout(false);
            this.tlpHeader.PerformLayout();
            this.gboxMain.ResumeLayout(false);
            this.gboxMain.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TableLayoutPanel tlpMain;
        private System.Windows.Forms.TableLayoutPanel tlpHeader;
        private System.Windows.Forms.Label lblSave;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Label lblHeading;
        private System.Windows.Forms.Label lblDelete;
        private System.Windows.Forms.Button btnFind;
        private System.Windows.Forms.Button btnMinimize;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label lblFind;
        private System.Windows.Forms.Button btnYtubeTutorial;
        private System.Windows.Forms.GroupBox gboxMain;
        private System.Windows.Forms.Label lblFooter;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox txtManfShortName;
        private System.Windows.Forms.Label lblManfShort;
        private System.Windows.Forms.Label lblManufacture;
        private System.Windows.Forms.TextBox txtDiscountPerc;
        private System.Windows.Forms.TextBox txtManufacture;
        private System.Windows.Forms.Label lblManfDisc;
        private System.Windows.Forms.ToolTip toolTipManufacturer;
        private System.Windows.Forms.Label lblMand1;
    }
}