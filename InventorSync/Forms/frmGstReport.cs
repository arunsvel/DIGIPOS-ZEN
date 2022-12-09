using System;
using System.Data;
using System.Windows.Forms;

using DigiposZen.InventorBL.Helper;

using System.Data.SqlClient;

using System.Windows.Forms.DataVisualization.Charting;

using System.IO;
using System.Text;
using System.Collections.Generic;

namespace DigiposZen
{
    public partial class frmGstReport : Form
    {
        public frmGstReport()
        {
            InitializeComponent();
            
        }
        string conn = @"Data Source = GAMERADICTION\DIGIPOS; Initial Catalog = DigiposDemo; User ID = sa; Password =#infinitY@279";
       
        private void frmGstReport_Load(object sender, EventArgs e)
        {
            SqlConnection con = new SqlConnection(conn);
            con.Open();
            string strCmd = "select VchTypeID,VchType from tblVchType where ParentID=1";
            SqlCommand cmd = new SqlCommand(strCmd, con);
            SqlDataAdapter da = new SqlDataAdapter(strCmd, con);
            DataSet ds = new DataSet();
            da.Fill(ds);
            cmd.ExecuteNonQuery();
            con.Close();
            cmbVoucherType.DataSource = ds.Tables[0];
            cmbVoucherType.DisplayMember = "VchType";
            cmbVoucherType.ValueMember = "VchTypeID";


            cmbVoucherType.Enabled = true;
            //dtpFD.MinDate = AppSettings.FinYearStart;
            //dtpFD.MaxDate = Convert.ToDateTime(AppSettings.FinYearEnd);
            //dtpTD.MinDate = AppSettings.FinYearStart;
            //dtpTD.MaxDate = Convert.ToDateTime(AppSettings.FinYearEnd);

        }
        private void frmGstReport_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
              
                if (e.KeyCode == Keys.Escape)
                {
                    this.Close();
                }
               
            }
            catch (Exception ex)
            {
                MessageBox.Show("Shortcut Keys not working ?" + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
           
        }
        private void lblToDate_Click(object sender, EventArgs e)
        {

        }

        private void dtpTD_ValueChanged(object sender, EventArgs e)
        {

        }

        private void lblFromDate_Click(object sender, EventArgs e)
        {

        }

        private void dtpFD_ValueChanged(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void btnShow_Click(object sender, EventArgs e)
        {
            DateTime FD = Convert.ToDateTime(dtpFD.Text);
            DateTime TD = Convert.ToDateTime(dtpTD.Text);

            try
            {
                using (SqlConnection con = new SqlConnection(conn))
                {
                    using (SqlCommand cmd = new SqlCommand("select PartyGSTIN as[GSTIN/UIN of Recipient],Party as [Receiver Name], (tblSales.Prefix+InvNo) as [Invoice Number],format(InvDate,'yyyy/MM/dd') as [Invoice date],cast(BillAmt as numeric(36,2)) as [Invoice Value],(StateCode + '-' + State ) as [Place Of Supply],'n'as [Reverse Charge],'' as [Applicable % of Tax Rate] ,BillType as [Invoive Type],'' as [E-commerce GSTIN],tblSalesItem.TaxPer as [Rate],cast(sum(tblSalesItem.ITaxableAmount) as numeric(36,2)) as [Taxable Value],IcessAmt as [Cess Amount] from tblSales  join tblSalesItem on tblSalesItem.InvID=tblSales.InvId join tblStates on tblSales.StateID=tblStates.StateId join tblVchType on tblVchType.VchTypeID=tblSales.VchTypeID where invdate BETWEEN '" + FD.ToString("dd-MMM-yyyy") + "' and '" + TD.ToString("dd-MMM-yyyy") + "' and ParentID=1  and CountryID ='21' and tblSales.VchTypeID='" + cmbVoucherType.SelectedValue + "' and GSTType='b2b' group by PartyGSTIN,Party,tblSales.Prefix,tblSales.InvNo,tblsales.InvDate,tblSales.BillAmt,tblStates.StateCode,tblStates.State,tblSales.BillType,tblSalesItem.TaxPer,tblSalesItem.IcessAmt  order by InvDate,InvNo", con))
                    {
                        cmd.CommandType = CommandType.Text;
                        using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                sda.Fill(dt);
                                dgvb2b.DataSource = dt;


                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error :" + ex.Message);
            }

            try
            {
                using (SqlConnection con = new SqlConnection(conn))
                {
                    using (SqlCommand cmd = new SqlCommand("select (tblSales.Prefix+InvNo) as [Invoice Number],format(InvDate,'yyyy/MM/dd') as [Invoice date],BillAmt as [Invoice Value] ,(StateCode + '-' + State ) as [Place Of Supply],'' as [Applicable % of Tax Rate],TaxPer as [Rate],ITaxableAmount as [Taxable Value],IcessAmt as [Cess Amount],'' as [E-commerce GSTIN] from tblSales join tblSalesItem on tblSales.InvId=tblSalesItem.InvID join tblStates on tblSales.StateID=tblStates.StateId join tblVchType on tblSales.VchTypeID=tblVchType.VchTypeID where invdate BETWEEN '" + FD.ToString("dd-MMM-yyyy") + "' and '" + TD.ToString("dd-MMM-yyyy") + "' and GSTType='b2c' and ParentID='1' and tblSales.StateId !='32' and CountryID ='21' and BillAmt>25000 and tblSales.VchTypeID='" + cmbVoucherType.SelectedValue + "' order by InvDate,InvNo", con))
                    {
                        cmd.CommandType = CommandType.Text;
                        using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                sda.Fill(dt);
                                dgvb2cl.DataSource = dt;

                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error :" + ex.Message);
            }


            try
            {
                using (SqlConnection con = new SqlConnection(conn))
                {
                    using (SqlCommand cmd = new SqlCommand("select 'OE' as Type,BillAmt as [Invoice Value] ,(StateCode + '-' + State ) as [Place Of Supply],'' as [Applicable % of Tax Rate],TaxPer as [Rate],ITaxableAmount as [Taxable Value],IcessAmt as [Cess Amount],'' as [E-commerce GSTIN] from tblSales join tblSalesItem on tblSales.InvId=tblSalesItem.InvID join tblStates on tblSales.StateID=tblStates.StateId join tblVchType on tblSales.VchTypeID=tblVchType.VchTypeID where invdate BETWEEN '" + FD.ToString("dd-MMM-yyyy") + "' and '" + TD.ToString("dd-MMM-yyyy") + "' and GSTType='b2c' and ParentID='1' and tblSales.StateId='32' and CountryID ='21' and tblSales.VchTypeID='" + cmbVoucherType.SelectedValue + "' order by InvDate,InvNo", con))
                    {
                        cmd.CommandType = CommandType.Text;
                        using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                        {
                            using (DataTable dt = new DataTable())
                            {

                                sda.Fill(dt);
                                dgvb2cs.DataSource = dt;

                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error :" + ex.Message);
            }

            try
            {

                using (SqlConnection con = new SqlConnection(conn))
                {
                    using (SqlCommand cmd = new SqlCommand("SELECT (CASE WHEN TaxAmt > 0 THEN 'WPAY'  WHEN TaxAmt = 0 THEN 'WOPAY'  END) as [Export Type],(Prefix+InvNo) as [Invoice Number],format(InvDate,'yyyy/MM/dd') as [Invoice Date],BillAmt as [Invoice Value],DeliveryDetails as [Port Code],DespatchDetails as [Shipping Bill Number],TermsOfDelivery as [Shipping Bill Date],TaxPer as Rate,Taxable as [Taxable Value],IcessAmt as [Cess Amount] FROM tblSales join tblSalesItem on tblSalesItem.InvID=tblSales.InvId join tblStates on tblSales.StateID=tblStates.StateId where invdate BETWEEN '" + FD.ToString("dd-MMM-yyyy") + "' and '" + TD.ToString("dd-MMM-yyyy") + "' and CountryID!='21' and tblSales.VchTypeID='" + cmbVoucherType.SelectedValue + "' order by InvDate,InvNo", con))
                    {
                        cmd.CommandType = CommandType.Text;
                        using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                sda.Fill(dt);
                                dgvexp.DataSource = dt;

                            }
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show("Error :" + ex.Message);
            }

            try
            {

                using (SqlConnection con = new SqlConnection(conn))
                {
                    using (SqlCommand cmd = new SqlCommand("select HSNID as HSN,'' as Description,Unit as UQC,sum(tblSalesItem.qty) as [Total Quantity],sum(INetAmount) as [Total Value],TaxPer as Rate,sum(ITaxableAmount) as [Taxable Value],sum(IGSTTaxAmt) as [Intagrated Tax Amount],sum(CGSTTaxAmt) as [Central Tax Amount],sum(SGSTTaxAmt) as [State/UT Tax amount],sum(IcessAmt) as [Cess amount] from tblSales join tblSalesItem on tblSales.InvId=tblSalesItem.InvID join tblItemMaster on tblSalesItem.ItemId=tblItemMaster.ItemID group by tblItemMaster.HSNID,tblSalesItem.Unit,tblSalesItem.TaxPer", con))
                    {
                        cmd.CommandType = CommandType.Text;
                        using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                sda.Fill(dt);
                                dgvhsn.DataSource = dt;

                            }
                        }
                    }
                }
            }
         
            catch (Exception ex)
            {
                MessageBox.Show("Error :" + ex.Message);
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            DateTime FD = Convert.ToDateTime(dtpFD.Text);
            if (dgvb2b.Rows.Count > 0)
            {


                bool exists = System.IO.Directory.Exists(Application.StartupPath + @"\ExportCsv\GSTR1 " + FD.ToString("yyyy-MMM"));

                if (!exists)

                    System.IO.Directory.CreateDirectory(Application.StartupPath + @"\ExportCsv\GSTR1 " + FD.ToString("yyyy-MMM"));
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "CSV (*.csv)|*.csv";
                sfd.FileName = Application.StartupPath + @"\ExportCsv\GSTR1 " + FD.ToString("yyyy-MMM") + @"\b2b.csv";
                bool fileError = false;


                if (!fileError)
                {
                    try
                    {
                        int sum1 = 0;
                        int sum2 = 0;
                        int sum3 = 0;
                        for (int s = 0; s < dgvb2b.Rows.Count; ++s)
                        {
                            sum1 += Convert.ToInt32(dgvb2b.Rows[s].Cells[4].Value);
                            sum2 += Convert.ToInt32(dgvb2b.Rows[s].Cells[11].Value);
                            sum3 += Convert.ToInt32(dgvb2b.Rows[s].Cells[12].Value);
                        }

                        int columnCount = dgvb2b.Columns.Count;
                        string columnNames = "";
                        string[] outputCsv = new string[dgvb2b.Rows.Count + 3];
                        for (int i = 0; i < columnCount; i++)
                        {
                            columnNames += dgvb2b.Columns[i].HeaderText.ToString() + ",";
                        }

                        outputCsv[0] = columnNames;

                        for (int i = 0; (i) < dgvb2b.Rows.Count - 1; i++)
                        {
                            for (int j = 0; j < columnCount; j++)
                            {
                                outputCsv[i + 1] += dgvb2b.Rows[i].Cells[j].Value.ToString().Replace(",", " ") + ",";
                            }

                        }
                        

                        File.WriteAllLines(sfd.FileName, outputCsv, Encoding.UTF8);

                        MessageBox.Show("Data Exported To:  " + sfd.FileName, "Info");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error :" + ex.Message);
                    }
                }
            }


            if (dgvb2cl.Rows.Count > 0)
            {


                bool exists = System.IO.Directory.Exists(Application.StartupPath + @"\ExportCsv\GSTR1 " + FD.ToString("yyyy-MMM"));

                if (!exists)
                System.IO.Directory.CreateDirectory(Application.StartupPath + @"\ExportCsv\GSTR1 " + FD.ToString("yyyy-MMM"));
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "CSV (*.csv)|*.csv";
                sfd.FileName = Application.StartupPath + @"\ExportCsv\GSTR1 " + FD.ToString("yyyy-MMM") + @"\b2cl.csv";
                bool fileError = false;

                if (!fileError)
                {
                    try
                    {
                        int sum1 = 0;
                        int sum2 = 0;

                        for (int s = 0; s < dgvb2cl.Rows.Count; ++s)
                        {

                            sum1 += Convert.ToInt32(dgvb2cl.Rows[s].Cells[2].Value);
                            sum2 += Convert.ToInt32(dgvb2cl.Rows[s].Cells[6].Value);



                        }

                        int columnCount = dgvb2cl.Columns.Count;
                        string columnNames = "";
                        string[] outputCsv = new string[dgvb2cl.Rows.Count + 3];
                        for (int i = 0; i < columnCount; i++)
                        {
                            columnNames += dgvb2cl.Columns[i].HeaderText.ToString() + ",";
                        }

                        outputCsv[0] = columnNames;

                        for (int i = 0; (i) < dgvb2cl.Rows.Count - 1; i++)
                        {
                            for (int j = 0; j < columnCount; j++)
                            {
                                outputCsv[i + 1] += dgvb2cl.Rows[i].Cells[j].Value.ToString().Replace(",", " ") + ",";
                            }

                        }


                        File.WriteAllLines(sfd.FileName, outputCsv, Encoding.UTF8);


                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error :" + ex.Message);
                    }

                }
            }

            if (dgvb2cs.Rows.Count > 0)
            {

                bool exists = System.IO.Directory.Exists(Application.StartupPath + @"\ExportCsv\GSTR1 " + FD.ToString("yyyy-MMM"));
               
                if (!exists)
                System.IO.Directory.CreateDirectory(Application.StartupPath + @"\ExportCsv\GSTR1 " + FD.ToString("yyyy-MMM"));
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "CSV (*.csv)|*.csv";
                sfd.FileName = Application.StartupPath + @"\ExportCsv\GSTR1 " + FD.ToString("yyyy-MMM") + @"\b2cs.csv";
                bool fileError = false;


                if (!fileError)
                {
                    try
                    {
                        int sum1 = 0;
                        int sum2 = 0;
                        for (int s = 0; s < dgvb2cs.Rows.Count; ++s)
                        {

                            sum1 += Convert.ToInt32(dgvb2cs.Rows[s].Cells[5].Value);
                            sum2 += Convert.ToInt32(dgvb2cs.Rows[s].Cells[6].Value);

                        }

                        int columnCount = dgvb2cs.Columns.Count;
                        string columnNames = "";
                        string[] outputCsv = new string[dgvb2cs.Rows.Count + 3];
                        for (int i = 0; i < columnCount; i++)
                        {
                            columnNames += dgvb2cs.Columns[i].HeaderText.ToString() + ",";
                        }

                        
                        outputCsv[0] = columnNames;


                        for (int i = 0; (i) < dgvb2cs.Rows.Count - 1; i++)
                        {
                            for (int j = 0; j < columnCount; j++)
                            {
                                outputCsv[i + 1] += dgvb2cs.Rows[i].Cells[j].Value.ToString().Replace(",", " ") + ",";
                            }

                        }

                        File.WriteAllLines(sfd.FileName, outputCsv, Encoding.UTF8);

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error :" + ex.Message);
                    }

                }
            }

        

            if (dgvexp.Rows.Count > 0)
            {


                bool exists = System.IO.Directory.Exists(Application.StartupPath + @"\ExportCsv\GSTR1 " + FD.ToString("yyyy-MMM"));

                if (!exists)
                System.IO.Directory.CreateDirectory(Application.StartupPath + @"\ExportCsv\GSTR1 " + FD.ToString("yyyy-MMM"));
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "CSV (*.csv)|*.csv";
                sfd.FileName = Application.StartupPath + @"\ExportCsv\GSTR1 " + FD.ToString("yyyy-MMM") + @"\exp.csv";
                bool fileError = false;


                if (!fileError)
                {
                    try
                    {
                        int sum1 = 0;
                        int sum2 = 0;

                        for (int s = 0; s < dgvexp.Rows.Count; ++s)
                        {

                            sum1 += Convert.ToInt32(dgvexp.Rows[s].Cells[3].Value);
                            sum2 += Convert.ToInt32(dgvexp.Rows[s].Cells[8].Value);

                        }

                        int columnCount = dgvexp.Columns.Count;
                        string columnNames = "";
                        string[] outputCsv = new string[dgvexp.Rows.Count + 3];
                        for (int i = 0; i < columnCount; i++)
                        {
                            columnNames += dgvexp.Columns[i].HeaderText.ToString() + ",";
                        }

                        outputCsv[0] = columnNames;

                        for (int i = 0; (i) < dgvexp.Rows.Count - 1; i++)
                        {
                            for (int j = 0; j < columnCount; j++)
                            {
                                outputCsv[i + 1] += dgvexp.Rows[i].Cells[j].Value.ToString().Replace(",", " ") + ",";
                            }

                        }


                        File.WriteAllLines(sfd.FileName, outputCsv, Encoding.UTF8);

                       
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error :" + ex.Message);
                    }

                }
            }




            if (dgvhsn.Rows.Count > 0)
            {

                bool exists = System.IO.Directory.Exists(Application.StartupPath + @"\ExportCsv\GSTR1 " + FD.ToString("yyyy-MMM"));

                if (!exists)

                    System.IO.Directory.CreateDirectory(Application.StartupPath + @"\ExportCsv\GSTR1 " + FD.ToString("yyyy-MMM"));
                    SaveFileDialog sfd = new SaveFileDialog();
                    sfd.Filter = "CSV (*.csv)|*.csv";
                    sfd.FileName = Application.StartupPath + @"\ExportCsv\GSTR1 " + FD.ToString("yyyy-MMM") + @"\HSN.csv";
                    bool fileError = false;


                if (!fileError)
                {
                    try
                    {
                        int sum1 = 0;
                        int sum2 = 0;
                        int sum3 = 0;
                        int sum4 = 0;
                        int sum5 = 0;
                        int sum6 = 0;

                        for (int s = 0; s < dgvhsn.Rows.Count; ++s)
                        {

                            sum1 += Convert.ToInt32(dgvhsn.Rows[s].Cells[4].Value);
                            sum2 += Convert.ToInt32(dgvhsn.Rows[s].Cells[6].Value);
                            sum3 += Convert.ToInt32(dgvhsn.Rows[s].Cells[7].Value);
                            sum4 += Convert.ToInt32(dgvhsn.Rows[s].Cells[8].Value);
                            sum5 += Convert.ToInt32(dgvhsn.Rows[s].Cells[9].Value);
                            sum6 += Convert.ToInt32(dgvhsn.Rows[s].Cells[10].Value);
                       
                        }

                        int columnCount = dgvhsn.Columns.Count;
                        string columnNames = "";
                        string[] outputCsv = new string[dgvhsn.Rows.Count + 3];
                        for (int i = 0; i < columnCount; i++)
                        {
                            columnNames += dgvhsn.Columns[i].HeaderText.ToString() + ",";
                        }
                          
                        outputCsv[0] = columnNames;
                         
                        for (int i = 0; (i) < dgvhsn.Rows.Count - 1; i++)
                        {
                            for (int j = 0; j < columnCount; j++)
                            {
                                outputCsv[i + 1] += dgvhsn.Rows[i].Cells[j].Value.ToString().Replace(",", " ") + ",";
                            }

                        }


                        File.WriteAllLines(sfd.FileName, outputCsv, Encoding.UTF8);

                        
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error :" + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("No Data To Export");
            }
        }

       
    }
}

            