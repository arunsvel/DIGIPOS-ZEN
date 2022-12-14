using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DigiposZen.InventorBL.Master;
using DigiposZen.InventorBL.Helper;
using DigiposZen.Info;
using System.Runtime.InteropServices;
using DigiposZen.JsonClass;
using DigiposZen.InventorBL.Accounts;
using DigiposZen.Forms;
using System.Data.SqlClient;

namespace DigiposZen
{
    // ======================================================== >>
    // Description:Color Creation
    // Developed By:Pramod Philip
    // Completed Date & Time: 09/09/2021 3.30 PM
    // Last Edited By:Anjitha k k
    // Last Edited Date & Time:01-March-2022 02:30 PM
    // ======================================================== >>

    public partial class frmCashDeskReport : Form
    {

        public frmCashDeskReport()
        {
            InitializeComponent();

        }
        Common Comm = new Common();
        clsVoucherType clsvr = new clsVoucherType();
        clsCostCentre clsccntr = new clsCostCentre();

        string constr = DigiposZen.Properties.Settings.Default.ConnectionString; //@"Data Source=NAHUM\DIGIPOS;Initial Catalog=DemoDB;Persist Security Info=True;User ID=sa;Password=#infinitY@279;Integrated Security=true";

        private void frmCashDeskReport_Load(object sender, EventArgs e)
        {
            dtpFD.MinDate = AppSettings.FinYearStart;
            dtpFD.MaxDate = Convert.ToDateTime(AppSettings.FinYearEnd);
            dtpTD.MinDate = AppSettings.FinYearStart;
            dtpTD.MaxDate = Convert.ToDateTime(AppSettings.FinYearEnd);
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnshow_Click(object sender, EventArgs e)
        {
            try
            {

                if (txtstafflist.Text == "")
                {
                    chkstaff.Checked = true;
                }
                if (txtVoucherTypeList.Text == "")
                {
                    chkVoucher.Checked = true;
                }
                string Sql = "DROP VIEW vwpurchase";
                SqlConnection conn = new SqlConnection(constr);
                conn.Open();
                try
                {
                    SqlCommand cmd = new SqlCommand(Sql, conn);
                    cmd.ExecuteNonQuery();
                }
                catch
                { }

                conn.Close();
                string Fname = "Cash Desk";
                DateTime FD = Convert.ToDateTime(dtpFD.Text);
                DateTime TD = Convert.ToDateTime(dtpTD.Text);
                string Sql1 = "create view vwpurchase as select InvNo,InvDate,tblCashDeskItems.ID,Name,tblVchType.VchType,MOP,PreviousBalance,CurrentReceipt,CurrentBalance as Balance,PaymentType,cast(format(tblCashDeskItems.Amount, 'F2', 'en-us') as float) as Amount,BillAmount from tblCashDeskdetails join tblCashDeskItems on tblCashDeskItems.ID=tblCashDeskdetails.id join tblSales ON tblSales.InvId=tblCashDeskdetails.InvID join tblEmployee on tblEmployee.EmpID =tblSales.SalesManID join tblVchType on tblVchType.VchTypeID=tblSales.VchTypeID where tblSales.VchTypeID in (" + lblVoucherIds.Text + ") and tblSales.SalesManID in (" + lblstaffIds.Text + ") and InvDate BETWEEN '" + FD.ToString("dd-MMM-yyyy") + "' and '" + TD.ToString("dd-MMM-yyyy") + "'  ";

                SqlConnection conn1 = new SqlConnection(constr);
                conn1.Open();
                SqlCommand cmd1 = new SqlCommand(Sql1, conn1);
                cmd1.ExecuteNonQuery();
                conn1.Close();
                new frmReportView1(Fname, "", "", "", FD.ToString(), TD.ToString(), this.MdiParent, "", "").Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
        }

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void chkVoucher_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkVoucher.Checked == true)
                {
                    string Sql = "Select VchTypeID,VchType from tblVchType where ParentID = 1 AND TenantID = '" + Global.gblTenantID + "'";
                    SqlConnection conn = new SqlConnection(constr);
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(Sql, conn);
                    DataTable dt = new DataTable();
                    SqlDataAdapter ad = new SqlDataAdapter(cmd);
                    ad.Fill(dt);
                    string sStrIds = "";
                    string sStrNames = "";

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        sStrNames = sStrNames + dt.Rows[i]["VchType"].ToString() + ",";
                        sStrIds = sStrIds + dt.Rows[i]["VchTypeID"].ToString() + ",";

                    }
                    txtVoucherTypeList.Text = sStrNames.Remove(sStrNames.Length - 1, 1);
                    lblVoucherIds.Text = sStrIds.Remove(sStrIds.Length - 1, 1);


                }
                else
                {
                    txtVoucherTypeList.Text = "";
                    chkVoucher.Checked = false;

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private Boolean GetFromCheckedListVoucher(string sSelIDs)
        {
            try
            {
                lblVoucherIds.Text = sSelIDs;
                lblVoucherIds.Tag = lblVoucherIds.Text;
                this.txtVoucherTypeList.TextChanged -= this.txtVoucherTypeList_Click;
                txtVoucherTypeList.Text = GetVoucherAsperIDs(sSelIDs);
                this.txtVoucherTypeList.TextChanged += this.txtVoucherTypeList_Click;
                string[] strCostIDs = sSelIDs.Split(',');
                return true;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
        }
        private string GetVoucherAsperIDs(string sIDs = "")
        {
            try
            {
                UspGetVoucherCheckedListInfo GetVoucherChk = new UspGetVoucherCheckedListInfo();
                string sRetResult = "";
                if (sIDs != "")
                {
                    DataTable dtData = new DataTable();
                    GetVoucherChk.IDs = sIDs;
                    GetVoucherChk.TenantId = Global.gblTenantID;
                    dtData = clsvr.GetVoucherCheckedList(GetVoucherChk);
                    if (dtData.Rows.Count > 0)
                    {

                        foreach (DataRow dr in dtData.Rows)
                            sRetResult = sRetResult + dr[0].ToString() + ",";

                        if (sRetResult.Length > 0)
                            sRetResult = sRetResult.Remove(sRetResult.Length - 1, 1);
                    }
                }
                return sRetResult;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return "";
            }
        }
        private void txtVoucherTypeList_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtVoucherTypeList.Text))
                {
                    lblVoucherIds.Text = Convert.ToString(txtVoucherTypeList.Tag);
                    lblVoucherIds.Text = "";
                }
                if (this.ActiveControl.Name != "txtVoucherTypeList")
                    return;
                string sQuery = "Select VchTypeID,VchType from tblVchType where ParentID=1 AND TenantID = '" + Global.gblTenantID + "'";
                new frmCompactCheckedListSearch(GetFromCheckedListVoucher, sQuery, "VchType", txtVoucherTypeList.Location.X + 570, txtVoucherTypeList.Location.Y + 400, 0, 2, txtVoucherTypeList.Text, 0, 0, "", lblVoucherIds.Text, null, "Voucher Type").ShowDialog();

            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void chkstaff_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                txtstafflist.ReadOnly = true;
                if (chkstaff.Checked == true)
                {
                    string Sql = "Select EmpID,Name from tblEmployee where TenantID = '" + Global.gblTenantID + "'";
                    SqlConnection conn = new SqlConnection(constr);
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(Sql, conn);
                    DataTable dt = new DataTable();
                    SqlDataAdapter ad = new SqlDataAdapter(cmd);
                    ad.Fill(dt);
                    string sStrIds = "";
                    string sStrNames = "";
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        sStrNames = sStrNames + dt.Rows[i]["Name"].ToString() + ",";
                        sStrIds = sStrIds + dt.Rows[i]["EmpID"].ToString() + ",";

                    }
                    txtstafflist.Text = sStrNames.Remove(sStrNames.Length - 1, 1);
                    lblstaffIds.Text = sStrIds.Remove(sStrIds.Length - 1, 1);
                }
                else
                {
                    txtstafflist.Text = "";
                    chkstaff.Checked = false;

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private Boolean GetFromCheckedListStaff(string sSelIDs)
        {
            try
            {
                lblstaffIds.Text = sSelIDs;
                lblstaffIds.Tag = lblstaffIds.Text;
                this.txtstafflist.TextChanged -= this.txtstafflist_Click;
                txtstafflist.Text = GetStaffAsperIDs(sSelIDs);
                this.txtstafflist.TextChanged += this.txtstafflist_Click;
                string[] strCostIDs = sSelIDs.Split(',');

                return true;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
        }
        private string GetStaffAsperIDs(string sIDs = "")
        {
            try
            {
                UspGetStaffCheckedListInfo GetStaffChk = new UspGetStaffCheckedListInfo();
                string sRetResult = "";
                if (sIDs != "")
                {
                    DataTable dtData = new DataTable();
                    GetStaffChk.IDs = sIDs;
                    GetStaffChk.TenantId = Global.gblTenantID;
                    dtData = clsccntr.GetCheckedListStaff(GetStaffChk);
                    if (dtData.Rows.Count > 0)
                    {
                        //sRetResult = dtData.Rows[0][0].ToString();
                        foreach (DataRow dr in dtData.Rows)
                            sRetResult = sRetResult + dr[1].ToString() + ",";

                        if (sRetResult.Length > 0)
                            sRetResult = sRetResult.Remove(sRetResult.Length - 1, 1);
                    }
                }
                return sRetResult;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return "";
            }
        }
        private void txtstafflist_Click(object sender, EventArgs e)
        {
            try
            {

                if (string.IsNullOrEmpty(txtstafflist.Text))
                {
                    txtstafflist.Text = Convert.ToString(txtstafflist.Tag);
                    txtstafflist.Text = "";
                }
                if (this.ActiveControl.Name != "txtstafflist")
                    return;
                string sQuery = "Select EmpID,Name from tblEmployee where TenantID = '" + Global.gblTenantID + "'";
                new frmCompactCheckedListSearch(GetFromCheckedListStaff, sQuery, "Name", txtstafflist.Location.X + 550, txtstafflist.Location.Y + 400, 0, 2, txtstafflist.Text, 0, 0, "", lblstaffIds.Text, null, "Sales Staff").ShowDialog();

            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

       
    }

}
