
namespace DigiposZen
{
    partial class frmBackupManager
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmBackupManager));
            this.lblSave = new System.Windows.Forms.Label();
            this.lblHeading = new System.Windows.Forms.Label();
            this.lblDelete = new System.Windows.Forms.Label();
            this.tlpHeader = new System.Windows.Forms.TableLayoutPanel();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnYtubeTutorial = new System.Windows.Forms.Button();
            this.btnMinimize = new System.Windows.Forms.Button();
            this.tlpMain = new System.Windows.Forms.TableLayoutPanel();
            this.tabMain = new System.Windows.Forms.TabControl();
            this.tbpBackup = new System.Windows.Forms.TabPage();
            this.button1 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tvwUserCompanyBackup = new System.Windows.Forms.TreeView();
            this.txtBackupPath = new System.Windows.Forms.TextBox();
            this.tbpRestore = new System.Windows.Forms.TabPage();
            this.label4 = new System.Windows.Forms.Label();
            this.tvwUserCompanyRestore = new System.Windows.Forms.TreeView();
            this.button2 = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.lblFooter = new System.Windows.Forms.Label();
            this.toolTipArea = new System.Windows.Forms.ToolTip(this.components);
            this.tlpHeader.SuspendLayout();
            this.tlpMain.SuspendLayout();
            this.tabMain.SuspendLayout();
            this.tbpBackup.SuspendLayout();
            this.tbpRestore.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblSave
            // 
            this.lblSave.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSave.ForeColor = System.Drawing.Color.Black;
            this.lblSave.Location = new System.Drawing.Point(4, 57);
            this.lblSave.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblSave.Name = "lblSave";
            this.lblSave.Size = new System.Drawing.Size(102, 22);
            this.lblSave.TabIndex = 7;
            this.lblSave.Text = "Backup";
            this.lblSave.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblHeading
            // 
            this.lblHeading.AutoSize = true;
            this.lblHeading.BackColor = System.Drawing.Color.Transparent;
            this.lblHeading.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblHeading.Font = new System.Drawing.Font("Tahoma", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHeading.ForeColor = System.Drawing.Color.Black;
            this.lblHeading.Location = new System.Drawing.Point(253, 0);
            this.lblHeading.Margin = new System.Windows.Forms.Padding(27, 0, 27, 0);
            this.lblHeading.Name = "lblHeading";
            this.lblHeading.Size = new System.Drawing.Size(560, 48);
            this.lblHeading.TabIndex = 2;
            this.lblHeading.Text = "Backup Manager";
            this.lblHeading.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblDelete
            // 
            this.lblDelete.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDelete.ForeColor = System.Drawing.Color.Black;
            this.lblDelete.Location = new System.Drawing.Point(114, 57);
            this.lblDelete.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblDelete.Name = "lblDelete";
            this.lblDelete.Size = new System.Drawing.Size(100, 22);
            this.lblDelete.TabIndex = 7;
            this.lblDelete.Text = "Delete";
            this.lblDelete.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tlpHeader
            // 
            this.tlpHeader.BackColor = System.Drawing.Color.Transparent;
            this.tlpHeader.ColumnCount = 6;
            this.tlpHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 110F));
            this.tlpHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 108F));
            this.tlpHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 8F));
            this.tlpHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tlpHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tlpHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpHeader.Controls.Add(this.lblSave, 0, 2);
            this.tlpHeader.Controls.Add(this.btnSave, 0, 0);
            this.tlpHeader.Controls.Add(this.btnDelete, 1, 0);
            this.tlpHeader.Controls.Add(this.lblHeading, 3, 0);
            this.tlpHeader.Controls.Add(this.lblDelete, 1, 2);
            this.tlpHeader.Controls.Add(this.btnClose, 5, 0);
            this.tlpHeader.Controls.Add(this.btnYtubeTutorial, 3, 2);
            this.tlpHeader.Controls.Add(this.btnMinimize, 4, 0);
            this.tlpHeader.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpHeader.Location = new System.Drawing.Point(0, 0);
            this.tlpHeader.Margin = new System.Windows.Forms.Padding(0);
            this.tlpHeader.Name = "tlpHeader";
            this.tlpHeader.RowCount = 3;
            this.tlpHeader.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 48F));
            this.tlpHeader.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 9F));
            this.tlpHeader.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 79F));
            this.tlpHeader.Size = new System.Drawing.Size(1000, 92);
            this.tlpHeader.TabIndex = 0;
            this.tlpHeader.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tlpHeader_MouseDown);
            this.tlpHeader.MouseMove += new System.Windows.Forms.MouseEventHandler(this.tlpHeader_MouseMove);
            this.tlpHeader.MouseUp += new System.Windows.Forms.MouseEventHandler(this.tlpHeader_MouseUp);
            // 
            // btnSave
            // 
            this.btnSave.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnSave.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnSave.FlatAppearance.BorderSize = 0;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Image = global::DigiposZen.Properties.Resources.save240402;
            this.btnSave.Location = new System.Drawing.Point(4, 4);
            this.btnSave.Margin = new System.Windows.Forms.Padding(4);
            this.btnSave.Name = "btnSave";
            this.tlpHeader.SetRowSpan(this.btnSave, 2);
            this.btnSave.Size = new System.Drawing.Size(102, 49);
            this.btnSave.TabIndex = 4;
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnDelete.FlatAppearance.BorderSize = 0;
            this.btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDelete.Image = global::DigiposZen.Properties.Resources.delete340402;
            this.btnDelete.Location = new System.Drawing.Point(114, 4);
            this.btnDelete.Margin = new System.Windows.Forms.Padding(4);
            this.btnDelete.Name = "btnDelete";
            this.tlpHeader.SetRowSpan(this.btnDelete, 2);
            this.btnDelete.Size = new System.Drawing.Size(100, 49);
            this.btnDelete.TabIndex = 5;
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnClose
            // 
            this.btnClose.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnClose.FlatAppearance.BorderSize = 0;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Image = global::DigiposZen.Properties.Resources.logout_Final;
            this.btnClose.Location = new System.Drawing.Point(924, 4);
            this.btnClose.Margin = new System.Windows.Forms.Padding(4);
            this.btnClose.Name = "btnClose";
            this.tlpHeader.SetRowSpan(this.btnClose, 2);
            this.btnClose.Size = new System.Drawing.Size(72, 49);
            this.btnClose.TabIndex = 6;
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnYtubeTutorial
            // 
            this.btnYtubeTutorial.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnYtubeTutorial.FlatAppearance.BorderSize = 0;
            this.btnYtubeTutorial.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnYtubeTutorial.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnYtubeTutorial.ForeColor = System.Drawing.Color.Black;
            this.btnYtubeTutorial.Image = ((System.Drawing.Image)(resources.GetObject("btnYtubeTutorial.Image")));
            this.btnYtubeTutorial.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnYtubeTutorial.Location = new System.Drawing.Point(230, 61);
            this.btnYtubeTutorial.Margin = new System.Windows.Forms.Padding(4);
            this.btnYtubeTutorial.Name = "btnYtubeTutorial";
            this.btnYtubeTutorial.Size = new System.Drawing.Size(186, 30);
            this.btnYtubeTutorial.TabIndex = 10;
            this.btnYtubeTutorial.Text = "Tutorial";
            this.btnYtubeTutorial.UseVisualStyleBackColor = true;
            this.btnYtubeTutorial.Visible = false;
            // 
            // btnMinimize
            // 
            this.btnMinimize.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnMinimize.FlatAppearance.BorderSize = 0;
            this.btnMinimize.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMinimize.Image = global::DigiposZen.Properties.Resources.minimize_finalised;
            this.btnMinimize.Location = new System.Drawing.Point(844, 4);
            this.btnMinimize.Margin = new System.Windows.Forms.Padding(4);
            this.btnMinimize.Name = "btnMinimize";
            this.tlpHeader.SetRowSpan(this.btnMinimize, 2);
            this.btnMinimize.Size = new System.Drawing.Size(72, 49);
            this.btnMinimize.TabIndex = 6;
            this.btnMinimize.UseVisualStyleBackColor = true;
            this.btnMinimize.Click += new System.EventHandler(this.btnMinimize_Click);
            // 
            // tlpMain
            // 
            this.tlpMain.BackColor = System.Drawing.Color.WhiteSmoke;
            this.tlpMain.ColumnCount = 1;
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 977F));
            this.tlpMain.Controls.Add(this.tlpHeader, 0, 0);
            this.tlpMain.Controls.Add(this.tabMain, 0, 1);
            this.tlpMain.Controls.Add(this.panel1, 0, 2);
            this.tlpMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpMain.Location = new System.Drawing.Point(1, 1);
            this.tlpMain.Margin = new System.Windows.Forms.Padding(4);
            this.tlpMain.Name = "tlpMain";
            this.tlpMain.RowCount = 3;
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 92F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 59F));
            this.tlpMain.Size = new System.Drawing.Size(1000, 519);
            this.tlpMain.TabIndex = 4;
            // 
            // tabMain
            // 
            this.tabMain.Controls.Add(this.tbpBackup);
            this.tabMain.Controls.Add(this.tbpRestore);
            this.tabMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabMain.Location = new System.Drawing.Point(3, 95);
            this.tabMain.Name = "tabMain";
            this.tabMain.SelectedIndex = 0;
            this.tabMain.Size = new System.Drawing.Size(994, 362);
            this.tabMain.TabIndex = 5;
            // 
            // tbpBackup
            // 
            this.tbpBackup.Controls.Add(this.button1);
            this.tbpBackup.Controls.Add(this.label2);
            this.tbpBackup.Controls.Add(this.label1);
            this.tbpBackup.Controls.Add(this.tvwUserCompanyBackup);
            this.tbpBackup.Controls.Add(this.txtBackupPath);
            this.tbpBackup.Location = new System.Drawing.Point(4, 27);
            this.tbpBackup.Name = "tbpBackup";
            this.tbpBackup.Padding = new System.Windows.Forms.Padding(3);
            this.tbpBackup.Size = new System.Drawing.Size(986, 331);
            this.tbpBackup.TabIndex = 0;
            this.tbpBackup.Text = "Backup";
            this.tbpBackup.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(853, 138);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(103, 31);
            this.button1.TabIndex = 5;
            this.button1.Text = "Browse";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(411, 63);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(172, 18);
            this.label2.TabIndex = 4;
            this.label2.Text = "Choose the backup path:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(31, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(194, 18);
            this.label1.TabIndex = 3;
            this.label1.Text = "Select companies to backup:";
            // 
            // tvwUserCompanyBackup
            // 
            this.tvwUserCompanyBackup.CheckBoxes = true;
            this.tvwUserCompanyBackup.Location = new System.Drawing.Point(31, 39);
            this.tvwUserCompanyBackup.Name = "tvwUserCompanyBackup";
            this.tvwUserCompanyBackup.Size = new System.Drawing.Size(358, 283);
            this.tvwUserCompanyBackup.TabIndex = 2;
            // 
            // txtBackupPath
            // 
            this.txtBackupPath.Location = new System.Drawing.Point(414, 87);
            this.txtBackupPath.Name = "txtBackupPath";
            this.txtBackupPath.Size = new System.Drawing.Size(542, 26);
            this.txtBackupPath.TabIndex = 0;
            // 
            // tbpRestore
            // 
            this.tbpRestore.Controls.Add(this.label4);
            this.tbpRestore.Controls.Add(this.tvwUserCompanyRestore);
            this.tbpRestore.Controls.Add(this.button2);
            this.tbpRestore.Controls.Add(this.label3);
            this.tbpRestore.Controls.Add(this.textBox1);
            this.tbpRestore.Location = new System.Drawing.Point(4, 27);
            this.tbpRestore.Name = "tbpRestore";
            this.tbpRestore.Padding = new System.Windows.Forms.Padding(3);
            this.tbpRestore.Size = new System.Drawing.Size(986, 331);
            this.tbpRestore.TabIndex = 1;
            this.tbpRestore.Text = "Restore";
            this.tbpRestore.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(61, 94);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(194, 18);
            this.label4.TabIndex = 10;
            this.label4.Text = "Select companies to restore:";
            // 
            // tvwUserCompanyRestore
            // 
            this.tvwUserCompanyRestore.CheckBoxes = true;
            this.tvwUserCompanyRestore.Location = new System.Drawing.Point(64, 124);
            this.tvwUserCompanyRestore.Name = "tvwUserCompanyRestore";
            this.tvwUserCompanyRestore.Size = new System.Drawing.Size(844, 201);
            this.tvwUserCompanyRestore.TabIndex = 9;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(805, 80);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(103, 31);
            this.button2.TabIndex = 8;
            this.button2.Text = "Browse";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(61, 27);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(172, 18);
            this.label3.TabIndex = 7;
            this.label3.Text = "Choose the backup path:";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(64, 48);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(844, 26);
            this.textBox1.TabIndex = 6;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.progressBar1);
            this.panel1.Controls.Add(this.lblFooter);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 463);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(994, 53);
            this.panel1.TabIndex = 6;
            // 
            // progressBar1
            // 
            this.progressBar1.Dock = System.Windows.Forms.DockStyle.Top;
            this.progressBar1.Location = new System.Drawing.Point(0, 0);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(994, 24);
            this.progressBar1.TabIndex = 6;
            // 
            // lblFooter
            // 
            this.lblFooter.BackColor = System.Drawing.Color.Transparent;
            this.lblFooter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblFooter.Font = new System.Drawing.Font("Tahoma", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFooter.ForeColor = System.Drawing.Color.Black;
            this.lblFooter.Location = new System.Drawing.Point(0, 25);
            this.lblFooter.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblFooter.Name = "lblFooter";
            this.lblFooter.Size = new System.Drawing.Size(994, 28);
            this.lblFooter.TabIndex = 5;
            this.lblFooter.Text = "Keyboard Shortcuts : - F3 Find, F5 Save, F7 Delete, Esc Close ";
            this.lblFooter.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // frmBackupManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(1002, 521);
            this.Controls.Add(this.tlpMain);
            this.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmBackupManager";
            this.Padding = new System.Windows.Forms.Padding(1);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmBackupManager";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmBackupManager_FormClosing);
            this.Load += new System.EventHandler(this.frmBackupManager_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmBackupManager_KeyDown);
            this.tlpHeader.ResumeLayout(false);
            this.tlpHeader.PerformLayout();
            this.tlpMain.ResumeLayout(false);
            this.tabMain.ResumeLayout(false);
            this.tbpBackup.ResumeLayout(false);
            this.tbpBackup.PerformLayout();
            this.tbpRestore.ResumeLayout(false);
            this.tbpRestore.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label lblSave;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.TableLayoutPanel tlpHeader;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Label lblHeading;
        private System.Windows.Forms.Label lblDelete;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnYtubeTutorial;
        private System.Windows.Forms.Button btnMinimize;
        private System.Windows.Forms.TableLayoutPanel tlpMain;
        private System.Windows.Forms.ToolTip toolTipArea;
        private System.Windows.Forms.TabControl tabMain;
        private System.Windows.Forms.TabPage tbpBackup;
        private System.Windows.Forms.TabPage tbpRestore;
        private System.Windows.Forms.TextBox txtBackupPath;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TreeView tvwUserCompanyBackup;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label lblFooter;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TreeView tvwUserCompanyRestore;
    }
}