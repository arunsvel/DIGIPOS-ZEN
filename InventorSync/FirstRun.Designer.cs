
namespace InventorSync.Forms
{
    partial class FirstRun
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
            this.prgInitialize = new System.Windows.Forms.ProgressBar();
            this.lblIniitialize = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // prgInitialize
            // 
            this.prgInitialize.Dock = System.Windows.Forms.DockStyle.Top;
            this.prgInitialize.Location = new System.Drawing.Point(0, 0);
            this.prgInitialize.Name = "prgInitialize";
            this.prgInitialize.Size = new System.Drawing.Size(1227, 67);
            this.prgInitialize.TabIndex = 2;
            this.prgInitialize.Visible = false;
            // 
            // lblIniitialize
            // 
            this.lblIniitialize.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblIniitialize.Font = new System.Drawing.Font("Tahoma", 12F);
            this.lblIniitialize.Location = new System.Drawing.Point(0, 67);
            this.lblIniitialize.Name = "lblIniitialize";
            this.lblIniitialize.Size = new System.Drawing.Size(1227, 81);
            this.lblIniitialize.TabIndex = 3;
            this.lblIniitialize.Text = "Please wait while we initialize database";
            this.lblIniitialize.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblIniitialize.Visible = false;
            // 
            // FirstRun
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1227, 160);
            this.ControlBox = false;
            this.Controls.Add(this.lblIniitialize);
            this.Controls.Add(this.prgInitialize);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FirstRun";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Shown += new System.EventHandler(this.FirstRun_Shown);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ProgressBar prgInitialize;
        private System.Windows.Forms.Label lblIniitialize;
    }
}