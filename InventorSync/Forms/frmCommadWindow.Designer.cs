
namespace DigiposZen.Forms
{
    partial class frmCommadWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmCommadWindow));
            this.tlpMain = new System.Windows.Forms.TableLayoutPanel();
            this.tlpHeader = new System.Windows.Forms.TableLayoutPanel();
            this.lblExecute = new System.Windows.Forms.Label();
            this.btnExecute = new System.Windows.Forms.Button();
            this.btnExport = new System.Windows.Forms.Button();
            this.lblHeading = new System.Windows.Forms.Label();
            this.lblExport = new System.Windows.Forms.Label();
            this.btnMinimize = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnYtubeTutorial = new System.Windows.Forms.Button();
            this.lblFooter = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnSearchFwd = new System.Windows.Forms.Button();
            this.btnSearchBwd = new System.Windows.Forms.Button();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.DgvData = new System.Windows.Forms.DataGridView();
            this.txtQuery = new System.Windows.Forms.TextBox();
            this.chkMatchCase = new System.Windows.Forms.CheckBox();
            this.chkExactWordOnly = new System.Windows.Forms.CheckBox();
            this.tlpMain.SuspendLayout();
            this.tlpHeader.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DgvData)).BeginInit();
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
            this.tlpHeader.ColumnCount = 8;
            this.tlpHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 79F));
            this.tlpHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 83F));
            this.tlpHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tlpHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tlpHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tlpHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tlpHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 65F));
            this.tlpHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 67F));
            this.tlpHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 18F));
            this.tlpHeader.Controls.Add(this.lblExecute, 0, 1);
            this.tlpHeader.Controls.Add(this.btnExecute, 0, 0);
            this.tlpHeader.Controls.Add(this.btnExport, 1, 0);
            this.tlpHeader.Controls.Add(this.lblHeading, 2, 0);
            this.tlpHeader.Controls.Add(this.lblExport, 1, 1);
            this.tlpHeader.Controls.Add(this.btnMinimize, 6, 0);
            this.tlpHeader.Controls.Add(this.btnClose, 7, 0);
            this.tlpHeader.Controls.Add(this.btnYtubeTutorial, 2, 1);
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
            // lblExecute
            // 
            this.lblExecute.Font = new System.Drawing.Font("Tahoma", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblExecute.ForeColor = System.Drawing.Color.Black;
            this.lblExecute.Location = new System.Drawing.Point(3, 51);
            this.lblExecute.Name = "lblExecute";
            this.lblExecute.Size = new System.Drawing.Size(73, 23);
            this.lblExecute.TabIndex = 7;
            this.lblExecute.Text = "Execute";
            this.lblExecute.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnExecute
            // 
            this.btnExecute.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnExecute.FlatAppearance.BorderSize = 0;
            this.btnExecute.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExecute.Image = global::DigiposZen.Properties.Resources.execute_1;
            this.btnExecute.Location = new System.Drawing.Point(3, 2);
            this.btnExecute.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnExecute.Name = "btnExecute";
            this.btnExecute.Size = new System.Drawing.Size(73, 47);
            this.btnExecute.TabIndex = 0;
            this.btnExecute.UseVisualStyleBackColor = true;
            this.btnExecute.Click += new System.EventHandler(this.btnExecute_Click);
            // 
            // btnExport
            // 
            this.btnExport.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnExport.FlatAppearance.BorderSize = 0;
            this.btnExport.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExport.Image = global::DigiposZen.Properties.Resources.export_csv;
            this.btnExport.Location = new System.Drawing.Point(82, 2);
            this.btnExport.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(77, 47);
            this.btnExport.TabIndex = 6;
            this.btnExport.UseVisualStyleBackColor = true;
            // 
            // lblHeading
            // 
            this.lblHeading.AutoSize = true;
            this.lblHeading.BackColor = System.Drawing.Color.Transparent;
            this.tlpHeader.SetColumnSpan(this.lblHeading, 4);
            this.lblHeading.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblHeading.Font = new System.Drawing.Font("Tahoma", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHeading.ForeColor = System.Drawing.Color.Black;
            this.lblHeading.Location = new System.Drawing.Point(180, 0);
            this.lblHeading.Margin = new System.Windows.Forms.Padding(18, 0, 18, 0);
            this.lblHeading.Name = "lblHeading";
            this.lblHeading.Size = new System.Drawing.Size(884, 51);
            this.lblHeading.TabIndex = 2;
            this.lblHeading.Text = "Advanced Search";
            this.lblHeading.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblExport
            // 
            this.lblExport.Font = new System.Drawing.Font("Tahoma", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblExport.ForeColor = System.Drawing.Color.Black;
            this.lblExport.Location = new System.Drawing.Point(82, 51);
            this.lblExport.Name = "lblExport";
            this.lblExport.Size = new System.Drawing.Size(77, 23);
            this.lblExport.TabIndex = 7;
            this.lblExport.Text = "Export";
            this.lblExport.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnMinimize
            // 
            this.btnMinimize.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnMinimize.FlatAppearance.BorderSize = 0;
            this.btnMinimize.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMinimize.Image = global::DigiposZen.Properties.Resources.minimize_finalised;
            this.btnMinimize.Location = new System.Drawing.Point(1085, 2);
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
            this.btnClose.Location = new System.Drawing.Point(1150, 2);
            this.btnClose.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(61, 47);
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
            this.btnYtubeTutorial.Location = new System.Drawing.Point(165, 53);
            this.btnYtubeTutorial.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnYtubeTutorial.Name = "btnYtubeTutorial";
            this.btnYtubeTutorial.Size = new System.Drawing.Size(224, 35);
            this.btnYtubeTutorial.TabIndex = 10;
            this.btnYtubeTutorial.Text = "Tutorial";
            this.btnYtubeTutorial.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnYtubeTutorial.UseVisualStyleBackColor = true;
            this.btnYtubeTutorial.Visible = false;
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
            this.panel1.Controls.Add(this.chkExactWordOnly);
            this.panel1.Controls.Add(this.chkMatchCase);
            this.panel1.Controls.Add(this.btnSearchFwd);
            this.panel1.Controls.Add(this.btnSearchBwd);
            this.panel1.Controls.Add(this.txtSearch);
            this.panel1.Controls.Add(this.DgvData);
            this.panel1.Controls.Add(this.txtQuery);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 81);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1208, 471);
            this.panel1.TabIndex = 5;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // btnSearchFwd
            // 
            this.btnSearchFwd.Image = global::DigiposZen.Properties.Resources.fast_forward;
            this.btnSearchFwd.Location = new System.Drawing.Point(679, 105);
            this.btnSearchFwd.Name = "btnSearchFwd";
            this.btnSearchFwd.Size = new System.Drawing.Size(39, 30);
            this.btnSearchFwd.TabIndex = 5;
            this.btnSearchFwd.UseVisualStyleBackColor = true;
            this.btnSearchFwd.Click += new System.EventHandler(this.btnSearchFwd_Click);
            // 
            // btnSearchBwd
            // 
            this.btnSearchBwd.Image = global::DigiposZen.Properties.Resources.fast_backwards;
            this.btnSearchBwd.Location = new System.Drawing.Point(634, 105);
            this.btnSearchBwd.Name = "btnSearchBwd";
            this.btnSearchBwd.Size = new System.Drawing.Size(39, 30);
            this.btnSearchBwd.TabIndex = 4;
            this.btnSearchBwd.UseVisualStyleBackColor = true;
            this.btnSearchBwd.Click += new System.EventHandler(this.btnSearchBwd_Click);
            // 
            // txtSearch
            // 
            this.txtSearch.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSearch.Location = new System.Drawing.Point(9, 109);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(609, 23);
            this.txtSearch.TabIndex = 2;
            // 
            // DgvData
            // 
            this.DgvData.AllowUserToAddRows = false;
            this.DgvData.AllowUserToDeleteRows = false;
            this.DgvData.BackgroundColor = System.Drawing.Color.White;
            this.DgvData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DgvData.Location = new System.Drawing.Point(9, 138);
            this.DgvData.Name = "DgvData";
            this.DgvData.ReadOnly = true;
            this.DgvData.RowHeadersWidth = 51;
            this.DgvData.RowTemplate.Height = 24;
            this.DgvData.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.DgvData.Size = new System.Drawing.Size(1190, 327);
            this.DgvData.TabIndex = 1;
            // 
            // txtQuery
            // 
            this.txtQuery.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtQuery.Font = new System.Drawing.Font("Tahoma", 9F);
            this.txtQuery.Location = new System.Drawing.Point(9, 13);
            this.txtQuery.Multiline = true;
            this.txtQuery.Name = "txtQuery";
            this.txtQuery.Size = new System.Drawing.Size(1190, 87);
            this.txtQuery.TabIndex = 0;
            // 
            // chkMatchCase
            // 
            this.chkMatchCase.AutoSize = true;
            this.chkMatchCase.Location = new System.Drawing.Point(802, 111);
            this.chkMatchCase.Name = "chkMatchCase";
            this.chkMatchCase.Size = new System.Drawing.Size(100, 21);
            this.chkMatchCase.TabIndex = 6;
            this.chkMatchCase.Text = "Match Case";
            this.chkMatchCase.UseVisualStyleBackColor = true;
            // 
            // chkExactWordOnly
            // 
            this.chkExactWordOnly.AutoSize = true;
            this.chkExactWordOnly.Location = new System.Drawing.Point(946, 111);
            this.chkExactWordOnly.Name = "chkExactWordOnly";
            this.chkExactWordOnly.Size = new System.Drawing.Size(136, 21);
            this.chkExactWordOnly.TabIndex = 7;
            this.chkExactWordOnly.Text = "Exact Word Only";
            this.chkExactWordOnly.UseVisualStyleBackColor = true;
            // 
            // frmCommadWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(1214, 577);
            this.Controls.Add(this.tlpMain);
            this.Font = new System.Drawing.Font("Tahoma", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmCommadWindow";
            this.Padding = new System.Windows.Forms.Padding(1);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmCommadWindow";
            this.tlpMain.ResumeLayout(false);
            this.tlpHeader.ResumeLayout(false);
            this.tlpHeader.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DgvData)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlpMain;
        private System.Windows.Forms.TableLayoutPanel tlpHeader;
        private System.Windows.Forms.Label lblExecute;
        private System.Windows.Forms.Button btnExecute;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.Label lblHeading;
        private System.Windows.Forms.Label lblExport;
        private System.Windows.Forms.Button btnMinimize;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnYtubeTutorial;
        private System.Windows.Forms.Label lblFooter;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.DataGridView DgvData;
        private System.Windows.Forms.TextBox txtQuery;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Button btnSearchFwd;
        private System.Windows.Forms.Button btnSearchBwd;
        private System.Windows.Forms.CheckBox chkExactWordOnly;
        private System.Windows.Forms.CheckBox chkMatchCase;
    }
}