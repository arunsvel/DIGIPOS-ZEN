namespace InventorSync.Controls
{
    partial class ToastForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ToastForm));
            this.lblToastMessage = new System.Windows.Forms.Label();
            this.tlpMain = new System.Windows.Forms.TableLayoutPanel();
            this.lblToastHeading = new System.Windows.Forms.Label();
            this.lblTmrVal = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.lifeTimer = new System.Windows.Forms.Timer(this.components);
            this.tlpMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblToastMessage
            // 
            this.lblToastMessage.AutoSize = true;
            this.lblToastMessage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblToastMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblToastMessage.ForeColor = System.Drawing.Color.White;
            this.lblToastMessage.Location = new System.Drawing.Point(33, 30);
            this.lblToastMessage.Name = "lblToastMessage";
            this.lblToastMessage.Size = new System.Drawing.Size(316, 1);
            this.lblToastMessage.TabIndex = 1;
            this.lblToastMessage.Text = "Message will appear here";
            this.lblToastMessage.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tlpMain
            // 
            this.tlpMain.BackColor = System.Drawing.Color.Teal;
            this.tlpMain.ColumnCount = 3;
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tlpMain.Controls.Add(this.lblToastMessage, 1, 1);
            this.tlpMain.Controls.Add(this.lblToastHeading, 0, 0);
            this.tlpMain.Controls.Add(this.lblTmrVal, 2, 2);
            this.tlpMain.Controls.Add(this.btnClose, 2, 0);
            this.tlpMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpMain.Location = new System.Drawing.Point(0, 0);
            this.tlpMain.Name = "tlpMain";
            this.tlpMain.RowCount = 3;
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tlpMain.Size = new System.Drawing.Size(382, 58);
            this.tlpMain.TabIndex = 1;
            // 
            // lblToastHeading
            // 
            this.lblToastHeading.AutoSize = true;
            this.tlpMain.SetColumnSpan(this.lblToastHeading, 2);
            this.lblToastHeading.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblToastHeading.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblToastHeading.ForeColor = System.Drawing.Color.Khaki;
            this.lblToastHeading.Location = new System.Drawing.Point(3, 0);
            this.lblToastHeading.Name = "lblToastHeading";
            this.lblToastHeading.Size = new System.Drawing.Size(346, 30);
            this.lblToastHeading.TabIndex = 0;
            this.lblToastHeading.Text = "Toast Form";
            this.lblToastHeading.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblTmrVal
            // 
            this.lblTmrVal.AutoSize = true;
            this.lblTmrVal.Location = new System.Drawing.Point(355, 28);
            this.lblTmrVal.Name = "lblTmrVal";
            this.lblTmrVal.Size = new System.Drawing.Size(0, 17);
            this.lblTmrVal.TabIndex = 3;
            this.lblTmrVal.Visible = false;
            // 
            // btnClose
            // 
            this.btnClose.FlatAppearance.BorderSize = 0;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Image = global::DigiposZen.Properties.Resources.logout_Final;
            this.btnClose.Location = new System.Drawing.Point(355, 3);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(24, 23);
            this.btnClose.TabIndex = 4;
            this.btnClose.UseVisualStyleBackColor = true;
            // 
            // ToastForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(382, 58);
            this.Controls.Add(this.tlpMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "ToastForm";
            this.Text = "ToastForm";
            this.tlpMain.ResumeLayout(false);
            this.tlpMain.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblToastMessage;
        private System.Windows.Forms.TableLayoutPanel tlpMain;
        private System.Windows.Forms.Label lblToastHeading;
        private System.Windows.Forms.Label lblTmrVal;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Timer lifeTimer;
    }
}