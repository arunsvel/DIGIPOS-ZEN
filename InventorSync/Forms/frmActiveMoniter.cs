using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using InventorSync.InventorBL.Master;
using InventorSync.InventorBL.Helper;
using InventorSync.Info;
using Syncfusion.WinForms.Controls;
using InventorSync.Forms;
using Newtonsoft.Json;
using System.Collections;
using InventorSync.JsonClass;
using System.Runtime.InteropServices;
using System.Data.SqlClient;

namespace InventorSync
{
    public partial class frmActiveMoniter : Form
    {
        public frmActiveMoniter(int iID = 0, bool bFromEdit = false)
        {
            InitializeComponent();




        }



        #region "VARIABLES  -------------------------------------------- >>"
        Common Comm = new Common();
        clsVoucherType clsvr = new clsVoucherType();
        clsCostCentre clsccntr = new clsCostCentre();
        string constr = Properties.Settings.Default.ConnectionString; //@"Data Source=NAHUM\DIGIPOS;Initial Catalog=DemoDB;Persist Security Info=True;User ID=sa;Password=#infinitY@279;Integrated Security=true";

        #endregion

        #region "EVENTS ------------------------------------------------ >>"

        #endregion

        #region "METHODS ----------------------------------------------- >>"

        #endregion

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
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
                string sQuery = "Select VchTypeID,VchType from tblVchType where TenantID = '" + Global.gblTenantID + "'";
                new frmCompactCheckedListSearch(GetFromCheckedListVoucher, sQuery, "VchType", txtVoucherTypeList.Location.X + 10, txtVoucherTypeList.Location.Y + 505, 0, 2, txtVoucherTypeList.Text, 0, 0, "", lblVoucherIds.Text, null, "Voucher Type").ShowDialog();

            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private string GetUserAsperIDs(string sIDs = "")
        {
            try
            {
                UspGetUserCheckedListInfo GetUserChk = new UspGetUserCheckedListInfo();
                string sRetResult = "";
                if (sIDs != "")
                {
                    DataTable dtData = new DataTable();
                    GetUserChk.IDs = sIDs;
                    dtData = clsccntr.GetCheckedListUser(GetUserChk);
                    if (dtData.Rows.Count > 0)
                    {
                        sRetResult = dtData.Rows[0][0].ToString();
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
        private Boolean GetFromCheckedListUser(string sSelIDs)
        {
            try
            {
                lblUserIds.Text = sSelIDs;
                lblUserIds.Tag = lblUserIds.Text;
                this.txtuser.TextChanged -= this.txtuser_Click;
                txtuser.Text = GetUserAsperIDs(sSelIDs);
                this.txtuser.TextChanged += this.txtuser_Click;
                string[] strCostIDs = sSelIDs.Split(',');

                return true;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
        }

        private void txtuser_Click(object sender, EventArgs e)
        {
            try
            {

                if (string.IsNullOrEmpty(txtuser.Text))
                {
                    lblUserIds.Text = Convert.ToString(txtuser.Tag);
                    lblUserIds.Text = "";
                }
                if (this.ActiveControl.Name != "txtuser")
                    return;
                string sQuery = "Select UserID,UserName from tblUserMaster";
                new frmCompactCheckedListSearch(GetFromCheckedListUser, sQuery, "UserName", txtuser.Location.X + 10, txtuser.Location.Y + 420, 0, 2, txtuser.Text, 0, 0, "", lblUserIds.Text, null, "User").ShowDialog();

            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void chkuser_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                txtuser.ReadOnly = true;
                if (chkuser.Checked == true)
                {
                    string Sql = "Select UserID,UserName from tblUserMaster";
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
                        sStrNames = sStrNames + dt.Rows[i]["UserName"].ToString() + ",";
                        sStrIds = sStrIds + dt.Rows[i]["UserID"].ToString() + ",";

                    }
                    txtuser.Text = sStrNames.Remove(sStrNames.Length - 1, 1);
                    lblUserIds.Text = sStrIds.Remove(sStrIds.Length - 1, 1);
                }
                else
                {
                    txtuser.Text = "";
                    chkuser.Checked = false;

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void chkVoucher_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkVoucher.Checked == true)
                {
                    string Sql = "Select VchTypeID,VchType from tblVchType where TenantID = '" + Global.gblTenantID + "'";
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
        private string GetActionAsperIDs(string sIDs = "")
        {
            try
            {
                //UspGetActionCheckedListInfo GetActionChk = new UspGetActionCheckedListInfo();
                string sRetResult = "";
                //if (sIDs != "")
                //{
                //    DataTable dtData = new DataTable();
                //    GetActionChk.IDs = sIDs;
                //    dtData = clsccntr.GetCheckedListAction(GetActionChk);
                //    if (dtData.Rows.Count > 0)
                //    {
                //        foreach (DataRow dr in dtData.Rows)
                //            sRetResult = sRetResult + "'" + dr[0].ToString() + "',";

                //        if (sRetResult.Length > 0)
                //            sRetResult = sRetResult.Remove(sRetResult.Length - 1, 1);

                //    }
                //}
            
                return sRetResult;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return "";
            }
        }
        private Boolean GetFromCheckedListAction(string sSelIDs)
        {
            try
            {
                lblactionId.Text = sSelIDs;
                lblactionId.Tag = lblactionId.Text;
                this.txtAction.TextChanged -= this.txtAction_Click;
                txtAction.Text = GetActionAsperIDs(sSelIDs);
                this.txtAction.TextChanged += this.txtAction_Click;
                string[] strCostIDs = sSelIDs.Split(',');

                return true;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
        }
        private void txtAction_Click(object sender, EventArgs e)
        {
            try
            {

                if (string.IsNullOrEmpty(txtAction.Text))
                {
                    txtAction.Text = Convert.ToString(txtAction.Tag);
                    txtAction.Text = "";
                }
                if (this.ActiveControl.Name != "txtMop")
                    return;
                string sQuery = "Select Distinct mop as MID,MOP as MopName from tblPurchase Order By MOP";
                new frmCompactCheckedListSearch(GetFromCheckedListAction, sQuery, "MopName", txtAction.Location.X + 772, txtAction.Location.Y + 405, 0, 2, txtAction.Text, 0, 0, "", lblactionId.Text, null, "Mod of Payment", "PurchaseReport", true).ShowDialog();

            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
}
