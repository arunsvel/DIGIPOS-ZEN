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
    public partial class Form1 : Form
    {
        
        public Form1(string sFormName = "",string vchtype="",string cost="",string mop="",string from="",string to="")
        {
            strFormName = sFormName;
            strVchtype = vchtype;
            strFrom = from;
            strTo = to;
            strcost = cost;
            InitializeComponent();
        }
        string strFormName;
        string strVchtype;
        string strcost;
        string strFrom;
        string strTo;
        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                label1.Text = strFormName + " Report";
                label2.Text = "FROM DATE:" + strFrom + ",TO DATE:" + strTo + ",Cost Center :" + strcost + ",Voucher Type:" + strVchtype + "";
                this.WindowState = FormWindowState.Maximized;
                SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM vwpurchase ORDER BY INVDATE,referenceautono,INVNO", Properties.Settings.Default.ConnectionString);
                DataSet ds = new DataSet();
                da.Fill(ds, "vwpurchase");
                dataGridView1.DataSource = ds.Tables["vwpurchase"].DefaultView;
                //this.dataGridView1.Columns["invid"].Visible = false;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);

            }

        }
    }
}
