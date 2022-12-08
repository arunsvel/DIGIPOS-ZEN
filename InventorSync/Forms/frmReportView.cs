using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;

using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;

using InventorSync.InventorBL.Helper;

namespace InventorSync.Forms
{
    public partial class frmReportView : Form
    {
        
        public frmReportView(string sFormName = "",string vchtype="",string cost="",string mop="",string from="",string to="",object MDIParent = null)
        {
            InitializeComponent();

            strFormName = sFormName;
            strVchtype = vchtype;
            strFrom = from;
            strTo = to;
            strcost = cost;

            if (MDIParent != null)
            {
                frmMDI form = (frmMDI)MDIParent;
                this.MdiParent = form;
                int l = form.ClientSize.Width - 10; //(this.MdiParent.ClientSize.Width - this.Width) / 2;
                int t = form.ClientSize.Height - 100; //((this.MdiParent.ClientSize.Height - this.Height) / 2) - 30;
                this.SetBounds(5, 0, l, t);
            }
        }
        string strFormName;
        string strVchtype;
        string strcost;
        string strFrom;
        string strTo;
        private void frmReportView_Load(object sender, EventArgs e)
        {
            try
            {

                label1.Text = strFormName + " Report";
                label2.Text = "FROM DATE:" + strFrom + ",TO DATE:" + strTo + ",Cost Center :" + strcost + ",Voucher Type:" + strVchtype + "";
                //this.WindowState = FormWindowState.Maximized;
                SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM vwpurchase ORDER BY INVDATE,referenceautono,INVNO", DigiposZen.Properties.Settings.Default.ConnectionString);
                DataSet ds = new DataSet();
                da.Fill(ds, "vwpurchase");
                dataGridView1.DataSource = ds.Tables["vwpurchase"].DefaultView;
        

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);

            }

        }

        
    }
}
