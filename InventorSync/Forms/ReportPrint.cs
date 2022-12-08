using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using Microsoft.Reporting.WinForms;
using InventorSync.InventorBL.Helper;

namespace InventorSync.Forms
{
    public partial class ReportPrint : Form
    {
        Common Comm = new Common();

        public ReportPrint(string inv = "", string PrintScheme = "", object MDIParent = null)
        {
            InitializeComponent();

            frmMDI form = (frmMDI)MDIParent;
            this.MdiParent = form;

            strinv = inv;
            strPrintScheme = PrintScheme;

        }
        string strinv;
        string strPrintScheme;
        private void ReportPrint_Load(object sender, EventArgs e)
        {
            try
            {
                reportViewer1.LocalReport.ReportPath = Application.StartupPath + "\\PrintScheme\\" + strPrintScheme;
                DataSet dsr = new DataSet();
                string constr = DigiposZen.Properties.Settings.Default.ConnectionString;
                SqlConnection conn = new SqlConnection(constr);


                SqlDataAdapter adp = new SqlDataAdapter("Select * from tblSales where invid='" + strinv + "' ", conn);
                DataTable tbl1 = new DataTable();
                tbl1.TableName = "DataSet2";
                adp.Fill(tbl1);

                SqlDataAdapter adp1 = new SqlDataAdapter("Select * from tblSalesItem join tblItemMaster on tblItemMaster.ItemID=tblSalesItem.ItemId where invid= '" + strinv + "'", conn);
                DataTable tbl2 = new DataTable();
                tbl2.TableName = "DataSet1";
                adp1.Fill(tbl2);

                SqlDataAdapter adp2 = new SqlDataAdapter("Select * from tblSalesItem join tblItemMaster on tblItemMaster.ItemID=tblSalesItem.ItemId where invid= '" + strinv + "'", conn);
                DataTable tbl3 = new DataTable();
                tbl3.TableName = "DataSet3";
                adp2.Fill(tbl3);
                SqlDataAdapter adp3 = new SqlDataAdapter("select tblSalesItem.taxper,sum(ITaxableAmount) as ITaxableAmount,sum(InonTaxableAmount) as InonTaxableAmount,sum(CGSTTaxAmt) as CGSTTaxAmt,sum(SGSTTaxAmt) as SGSTTaxAmt from tblItemMaster join tblSalesItem on tblItemMaster.ItemId=tblSalesItem.ItemID where InvID='" + strinv + "' GROUP BY (TaxPer)", conn);
                DataTable tbl4 = new DataTable();
                tbl4.TableName = "DataSet4";
                adp3.Fill(tbl4);
                SqlDataAdapter adp4 = new SqlDataAdapter("select ValueName from tblAppSettings where keyname='BLNSHOWCOMPANYNAME'", conn);
                DataTable tbl5 = new DataTable();
                tbl5.TableName = "DataSet5";
                adp4.Fill(tbl5);
                SqlDataAdapter adp5 = new SqlDataAdapter("select ValueName from tblAppSettings where keyname='BLNSHOWCOMPANYADDRESS'", conn);
                DataTable tbl6 = new DataTable();
                tbl6.TableName = "DataSet6";
                adp5.Fill(tbl6);

                dsr.Tables.Add(tbl1);
                dsr.Tables.Add(tbl2);
                dsr.Tables.Add(tbl3);
                dsr.Tables.Add(tbl4);
                dsr.Tables.Add(tbl5);
                dsr.Tables.Add(tbl6);

                ReportDataSource rds = new ReportDataSource("DataSet2", dsr.Tables[0]);
                ReportDataSource rds1 = new ReportDataSource("DataSet1", dsr.Tables[1]);
                ReportDataSource rds2 = new ReportDataSource("DataSet3", dsr.Tables[2]);
                ReportDataSource rds3 = new ReportDataSource("DataSet4", dsr.Tables[3]);
                ReportDataSource rds4 = new ReportDataSource("DataSet5", dsr.Tables[4]);
                ReportDataSource rds5 = new ReportDataSource("DataSet6", dsr.Tables[5]);

                this.reportViewer1.LocalReport.DataSources.Clear();
                this.reportViewer1.LocalReport.DataSources.Add(rds);
                this.reportViewer1.LocalReport.DataSources.Add(rds1);
                this.reportViewer1.LocalReport.DataSources.Add(rds2);
                this.reportViewer1.LocalReport.DataSources.Add(rds3);

                this.reportViewer1.LocalReport.DataSources.Add(rds4);
                this.reportViewer1.LocalReport.DataSources.Add(rds5);

                this.reportViewer1.RefreshReport();
            }
            catch(Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        public void PrintReport(string PrintSettings, string ReportName, decimal NoOfItems)
        {
            try
            {
                Printing obj = new Printing();
                obj.Run(reportViewer1, PrintSettings, ReportName, NoOfItems);
            }
            catch 
            { 

            }
        }
    }
}
