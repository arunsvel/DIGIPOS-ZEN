
namespace DigiposZen.Forms
{
    partial class ReportPrint
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
            this.tblAccountGroupBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.digiposDemoDataSet = new DigiposZen.DigiposDemoDataSet();
            this.tblAccountGroupTableAdapter = new DigiposZen.DigiposDemoDataSetTableAdapters.tblAccountGroupTableAdapter();
            this.reportViewer1 = new Microsoft.Reporting.WinForms.ReportViewer();
            ((System.ComponentModel.ISupportInitialize)(this.tblAccountGroupBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.digiposDemoDataSet)).BeginInit();
            this.SuspendLayout();
            // 
            // tblAccountGroupBindingSource
            // 
            this.tblAccountGroupBindingSource.DataMember = "tblAccountGroup";
            this.tblAccountGroupBindingSource.DataSource = this.digiposDemoDataSet;
            // 
            // digiposDemoDataSet
            // 
            this.digiposDemoDataSet.DataSetName = "DigiposDemoDataSet";
            this.digiposDemoDataSet.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // tblAccountGroupTableAdapter
            // 
            this.tblAccountGroupTableAdapter.ClearBeforeFill = true;
            // 
            // reportViewer1
            // 
            this.reportViewer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.reportViewer1.LocalReport.ReportEmbeddedResource = "DigiposZen.Forms.Report1.rdlc";
            this.reportViewer1.Location = new System.Drawing.Point(0, 0);
            this.reportViewer1.Name = "reportViewer1";
            this.reportViewer1.ServerReport.BearerToken = null;
            this.reportViewer1.Size = new System.Drawing.Size(800, 450);
            this.reportViewer1.TabIndex = 0;
            // 
            // ReportPrint
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.reportViewer1);
            this.Name = "ReportPrint";
            this.Text = "Form3";
            this.Load += new System.EventHandler(this.ReportPrint_Load);
            ((System.ComponentModel.ISupportInitialize)(this.tblAccountGroupBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.digiposDemoDataSet)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private DigiposZen.DigiposDemoDataSet digiposDemoDataSet;
        private System.Windows.Forms.BindingSource tblAccountGroupBindingSource;
        private DigiposZen.DigiposDemoDataSetTableAdapters.tblAccountGroupTableAdapter tblAccountGroupTableAdapter;
        private Microsoft.Reporting.WinForms.ReportViewer reportViewer1;
    }
}