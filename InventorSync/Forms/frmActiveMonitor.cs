using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Windows.Controls;
using System.Windows.Forms;
using DigiposZen.InventorBL.Helper;

namespace DigiposZen.Forms
{
    public partial class frmActiveMonitor : Form
    {

        Common Comm = new Common();

        public frmActiveMonitor()
        {
            InitializeComponent();
        }
        // DigiposZen.Properties.Settings.Default.ConnectionString
        string constr = DigiposZen.Properties.Settings.Default.ConnectionString;



        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;

        }

        private void btnSearchResult_Click(object sender, EventArgs e)
        {

        }

        private void btnShow_Click(object sender, EventArgs e)
        {
            if (cblaction.CheckedItems.Count == 0)
            {
                chkAction.Checked = true;
            }
            if (cblSystemName.CheckedItems.Count == 0)
            {
                chkSystemName.Checked = true;
            }
            if (cbluser.CheckedItems.Count == 0)
            {
                chkUser.Checked = true;
            }
            if (cblWindowsName.CheckedItems.Count == 0)
            {
                chkWindowsName.Checked = true;
            }





            string Action = "";

            for (int i = 0; i < cblaction.CheckedItems.Count; i++)
            {

                Action += "'" + cblaction.GetItemText(cblaction.CheckedItems[i]).ToString() + "',";

            }
            string User = "";
    
            for (int i = 0; i < cbluser.CheckedItems.Count; i++)
            {

                User += "'" + cbluser.GetItemText(cbluser.CheckedItems[i]).ToString() + "',";

            }
            string SystemName = "";

            for (int i = 0; i < cblSystemName.CheckedItems.Count; i++)
            {

                SystemName += "'"+cblSystemName.GetItemText(cblSystemName.CheckedItems[i]).ToString() + "',";

            }
            string WindowsName = "";

            for (int i = 0; i < cblWindowsName.CheckedItems.Count; i++)
            {

                WindowsName += "'" + cblWindowsName.GetItemText(cblWindowsName.CheckedItems[i]).ToString() + "',";

            }

            DateTime FD = Convert.ToDateTime(dtpFD.Text);
            DateTime TD = Convert.ToDateTime(dtpTD.Text);
            string a = "select [Action],WindowName,format(DateOf, 'dd-MMM-yyyy') as Date,format(timeof, 'hh-mm-ss') as Time,UserName,SystemName,NewData,OldData from tbluserLog where Action in (" + Action.Remove(Action.Length - 1) + ") and userName in (" + User.Remove(User.Length - 1) + ") and Systemname in (" + SystemName.Remove(SystemName.Length - 1) + ") and WindowName in (" + WindowsName.Remove(WindowsName.Length - 1) + ") and DateOf BETWEEN '" + FD.ToString("dd-MMM-yyyy") + "' and '" + TD.ToString("dd-MMM-yyyy") + "' ";
            SqlDataAdapter da = new SqlDataAdapter("select [Action],WindowName,format(DateOf, 'dd-MMM-yyyy') as Date,format(timeof, 'hh-mm-ss') as Time,UserName,SystemName,NewData,OldData from tbluserLog where Action in (" + Action.Remove(Action.Length - 1) + ") and userName in (" + User.Remove(User.Length - 1) + ") and Systemname in (" + SystemName.Remove(SystemName.Length - 1) + ") and WindowName in (" + WindowsName.Remove(WindowsName.Length - 1) + ") and DateOf BETWEEN '" + FD.ToString("dd-MMM-yyyy") + "' and '" + TD.ToString("dd-MMM-yyyy") + "' order by DateOf", constr);
            DataSet ds = new DataSet();
            da.Fill(ds, "vwpurchase");
            DgvLoadData.DataSource = ds.Tables["vwpurchase"].DefaultView;


        }

        private void frmActiveMonitor_Load(object sender, EventArgs e)
        {
            panel2.Visible = false;

            string com = "Select  distinct Action from tbluserLog";
            SqlDataAdapter adpt = new SqlDataAdapter(com, constr);
            DataTable dt = new DataTable();
            adpt.Fill(dt);
            cblaction.DataSource = dt;
            cblaction.DisplayMember = "Action";

            string com1 = "Select  distinct userName from tbluserLog";
            SqlDataAdapter adpt1 = new SqlDataAdapter(com1, constr);
            DataTable dt1 = new DataTable();
            adpt1.Fill(dt1);
            cbluser.DataSource = dt1;
            cbluser.DisplayMember = "userName";

            string com2 = "Select  distinct Systemname from tbluserLog";
            SqlDataAdapter adpt2 = new SqlDataAdapter(com2, constr);
            DataTable dt2 = new DataTable();
            adpt2.Fill(dt2);
            cblSystemName.DataSource = dt2;
            cblSystemName.DisplayMember = "Systemname";

            string com3 = "Select  distinct WindowName from tbluserLog";
            SqlDataAdapter adpt3 = new SqlDataAdapter(com3, constr);
            DataTable dt3 = new DataTable();
            adpt3.Fill(dt3);
            cblWindowsName.DataSource = dt3;
            cblWindowsName.DisplayMember = "WindowName";

        }

        private void DgvLoadData_CellClick(object sender, DataGridViewCellEventArgs e)
        {
          
            
            if (DgvLoadData.CurrentRow.Cells[6].Value.ToString()!="" && DgvLoadData.CurrentRow.Cells[7].Value.ToString() == "")
            {
                panel2.Visible = true;
                txtDataLoadOld.Visible = false;
                txtDataLoadNew.Visible = true;
                string newData = "NEWDATA..........\r\n"+DgvLoadData.CurrentRow.Cells[6].Value.ToString().Replace(",", "\r\n");
                txtDataLoadNew.Text = newData;
                txtDataLoadNew.Dock = DockStyle.Fill;

            }
            else if(DgvLoadData.CurrentRow.Cells[7].Value.ToString() != "" && DgvLoadData.CurrentRow.Cells[6].Value.ToString() == "")
            {
                panel2.Visible = true;
                txtDataLoadNew.Visible = false;
                txtDataLoadOld.Visible = true;
                string olddata = "OLDDATA..........\r\n"+DgvLoadData.CurrentRow.Cells[7].Value.ToString().Replace(",", "\r\n");
                txtDataLoadOld.Text = olddata;
                txtDataLoadOld.Dock = DockStyle.Fill;


            }
            else if(DgvLoadData.CurrentRow.Cells[7].Value.ToString() != "" && DgvLoadData.CurrentRow.Cells[6].Value.ToString() != "")
            {
                panel2.Visible = true;
                txtDataLoadNew.Visible = true;
                txtDataLoadOld.Visible = true;
                string newData = "NEWDATA..........\r\n" + DgvLoadData.CurrentRow.Cells[6].Value.ToString().Replace(",", "\r\n");
                txtDataLoadNew.Text = newData;
                string olddata = "OLDDATA..........\r\n" + DgvLoadData.CurrentRow.Cells[7].Value.ToString().Replace(",", "\r\n");
                txtDataLoadOld.Text = olddata;

            }
        }

        private void txtDataLoad_DoubleClick(object sender, EventArgs e)
        {
            txtDataLoadNew.Text = "";
            txtDataLoadOld.Text = "";

            panel2.Visible = false;
        }

        private void txtDataLoadNew_DoubleClick(object sender, EventArgs e)
        {
            txtDataLoadNew.Text = "";
            txtDataLoadOld.Text = "";

            panel2.Visible = false;

        }

        private void chkAction_CheckedChanged(object sender, EventArgs e)
        {
            if (chkAction.Checked == true)
            {
                for (int i = 0; i < cblaction.Items.Count; i++)
                {
                    cblaction.SetItemChecked(i, true);
                }
            }
            else
            {
                for (int i = 0; i < cblaction.Items.Count; i++)
                {
                    cblaction.SetItemChecked(i, false);
                }
            }
        }

        private void chkUser_CheckedChanged(object sender, EventArgs e)
        {
            if (chkUser.Checked == true)
            {
                for (int i = 0; i < cbluser.Items.Count; i++)
                {
                    cbluser.SetItemChecked(i, true);
                }
            }
            else
            {
                for (int i = 0; i < cbluser.Items.Count; i++)
                {
                    cbluser.SetItemChecked(i, false);
                }
            }
        }

        private void chkWindowsName_CheckedChanged(object sender, EventArgs e)
        {
            if (chkWindowsName.Checked == true)
            {
                for (int i = 0; i < cblWindowsName.Items.Count; i++)
                {
                    cblWindowsName.SetItemChecked(i, true);
                }
            }
            else
            {
                for (int i = 0; i < cblWindowsName.Items.Count; i++)
                {
                    cblWindowsName.SetItemChecked(i, false);
                }
            }
        }

        private void chkSystemName_CheckedChanged(object sender, EventArgs e)
        {
            if (chkSystemName.Checked == true)
            {
                for (int i = 0; i < cblSystemName.Items.Count; i++)
                {
                    cblSystemName.SetItemChecked(i, true);
                }
            }
            else
            {
                for (int i = 0; i < cblSystemName.Items.Count; i++)
                {
                    cblSystemName.SetItemChecked(i, false);
                }
            }
        }

       
    }
}
