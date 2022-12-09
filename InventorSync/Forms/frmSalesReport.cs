using System;
using System.Data;
using System.Windows.Forms;
using DigiposZen.InventorBL.Master;
using DigiposZen.InventorBL.Helper;
using DigiposZen.Info;
using DigiposZen.Forms;
using DigiposZen.JsonClass;
using System.Data.SqlClient;
using DigiposZen.InventorBL.Accounts;
using Microsoft.VisualBasic;
using System.Drawing;

namespace DigiposZen
{
    public partial class frmSalesReport : Form
    {
        // ======================================================== >>
        // Description:  Purchase Report          
        // Developed By: Anjitha K K           
        // Completed Date & Time: 24/02/2022 6:21 PM
        // Last Edited By:       
        // Last Edited Date & Time:
        // ======================================================== >>

        public frmSalesReport()
        {
            Cursor.Current = Cursors.WaitCursor;
            InitializeComponent();
            this.BackColor = Global.gblFormBorderColor;
            Cursor.Current = Cursors.Default;

            try
            {
                Comm.TransparentControls(this);
                Comm.SetControlColours(this);

                tlpMain.BackColor = Color.FromArgb(249, 246, 238);
                this.BackColor = Color.Black;
                this.Padding = new Padding(1);

                //Comm.LoadBGImage(this, picBackground);

                lblHeading.Font = new Font("Tahoma", 21, FontStyle.Regular, GraphicsUnit.Pixel);

                btnMinimize.Image = global::DigiposZen.Properties.Resources.minimize_finalised;
                btnClose.Image = global::DigiposZen.Properties.Resources.logout_Final;

            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
            }

            txtCostCenterList.ReadOnly = true;
            txtVoucherTypeList.ReadOnly = true;

        }

        #region "VARIABLES --------------------------------------------- >>"
        string constr = DigiposZen.Properties.Settings.Default.ConnectionString; //@"Data Source=NAHUM\DIGIPOS;Initial Catalog=DemoDB;Persist Security Info=True;User ID=sa;Password=#infinitY@279;Integrated Security=true";
        bool dragging = false;
        int xOffset = 0, yOffset = 0;
        Common Comm = new Common();
        clsCostCentre clsccntr = new clsCostCentre();
        clsVoucherType clsvr = new clsVoucherType();
        clsJsonVoucherType clsVchType = new clsJsonVoucherType();
        clsLedger clsLedg = new clsLedger();
        clsItemMaster clsitem = new clsItemMaster();
        UspGetLedgerInfo GetLedinfo = new UspGetLedgerInfo();
        InventorBL.Helper.Common Com = new InventorBL.Helper.Common();

        UspGetItemMasterInfo GetItem = new UspGetItemMasterInfo();
        #endregion

        #region "EVENTS ------------------------------------------------ >>"
        private void frmPurchaseReport_Load(object sender, EventArgs e)
        {
            rdoSalesDaybook.Checked = true;
            rdoDefault.Checked = true;
            dtpFD.MinDate = AppSettings.FinYearStart;
            dtpFD.MaxDate = Convert.ToDateTime(AppSettings.FinYearEnd);
            dtpTD.MinDate = AppSettings.FinYearStart;
            dtpTD.MaxDate = Convert.ToDateTime(AppSettings.FinYearEnd);

        }
        private void frmPurchaseReport_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                if (e.KeyCode == Keys.Escape)
                {
                    this.Close();
                }
                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Shortcut Keys not working ?" + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }
        private void lblHeading_MouseDown(object sender, MouseEventArgs e)
        {

        }
        private void lblHeading_MouseMove(object sender, MouseEventArgs e)
        {

        }
        private void lblHeading_MouseUp(object sender, MouseEventArgs e)
        {
            dragging = false;
        }
        private void tlpHeading_MouseDown(object sender, MouseEventArgs e)
        {

        }
        private void tlpHeading_MouseMove(object sender, MouseEventArgs e)
        {

        }
        private void tlpHeading_MouseUp(object sender, MouseEventArgs e)
        {
            dragging = false;
        }
        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion

        #region "METHODS ----------------------------------------------- >>"
        private void DropPurchaseView()
        {
            try
            {
                string Sql = "DROP VIEW vwpurchase";
                SqlConnection conn = new SqlConnection(constr);
                conn.Open();
                SqlCommand cmd = new SqlCommand(Sql, conn);
                cmd.ExecuteNonQuery();
                conn.Close();
            }
            catch
            {

            }
        }
        private Boolean GetFromCheckedListCost(string sSelIDs)
        {
            try
            {
                lblCostCenterIds.Text = sSelIDs;
                lblCostCenterIds.Tag = lblCostCenterIds.Text;
                this.txtCostCenterList.TextChanged -= this.txtCostCenterList_Click;
                txtCostCenterList.Text = GetCostCenterAsperIDs(sSelIDs);
                this.txtCostCenterList.TextChanged += this.txtCostCenterList_Click;

                string[] strCostIDs = sSelIDs.Split(',');

                return true;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
        }
        private Boolean GetFromCheckedListTax(string sSelIDs)
        {
            try
            {
                lblTaxIds.Text = sSelIDs;
                lblTaxIds.Tag = lblCostCenterIds.Text;
                this.txtTaxMode.TextChanged -= this.txtTaxMode_Click;
                txtTaxMode.Text = GetTaxModeAsperIDs(sSelIDs);
                this.txtTaxMode.TextChanged += this.txtTaxMode_Click;

                string[] strCostIDs = sSelIDs.Split(',');

                return true;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
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
        private Boolean GetFromCheckedListAgent(string sSelIDs)
        {
            try
            {
                lblAgentIds.Text = sSelIDs;
                lblAgentIds.Tag = lblAgentIds.Text;
                this.txtAgent.TextChanged -= this.txtAgent_Click_1;
                txtAgent.Text = GetAgentAsperIDs(sSelIDs);
                this.txtAgent.TextChanged += this.txtAgent_Click_1;
                string[] strCostIDs = sSelIDs.Split(',');

                return true;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
        }
        private Boolean GetFromCheckedListBill(string sSelIDs)
        {
            try
            {
                lblBillIds.Text = sSelIDs;
                lblBillIds.Tag = lblBillIds.Text;
                this.txtBillType.TextChanged -= this.txtBillType_Click;
                txtBillType.Text = GetBillAsperIDs(sSelIDs);
                this.txtBillType.TextChanged += this.txtBillType_Click;
                string[] strCostIDs = sSelIDs.Split(',');

                return true;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
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
        private Boolean GetFromCheckedListMop(string sSelIDs)
        {
            try
            {
                lblMopIds.Text = sSelIDs;
                lblMopIds.Tag = lblMopIds.Text;
                this.txtMop.TextChanged -= this.txtMop_Click;
                txtMop.Text = GetMopAsperIDs(sSelIDs);
                this.txtMop.TextChanged += this.txtMop_Click;
                string[] strCostIDs = sSelIDs.Split(',');

                return true;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
        }
        private Boolean GetFromCheckedListCat(string sSelIDs)
        {
            try
            {
                lblCatIds.Text = sSelIDs;
                lblCatIds.Tag = lblVoucherIds.Text;
                this.txtCategory.TextChanged -= this.txtCategory_Click;
                txtCategory.Text = GetCatAsperIDs(sSelIDs);
                this.txtCategory.TextChanged += this.txtCategory_Click;
                string[] strCostIDs = sSelIDs.Split(',');
                return true;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
        }
        private Boolean GetFromCheckedListMnf(string sSelIDs)
        {
            try
            {
                lblMnfIds.Text = sSelIDs;
                lblMnfIds.Tag = lblMnfIds.Text;
                this.txtMnf.TextChanged -= this.txtMnf_Click;
                txtMnf.Text = GetMnfAsperIDs(sSelIDs);
                this.txtMnf.TextChanged += this.txtMnf_Click;
                string[] strCostIDs = sSelIDs.Split(',');
                return true;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
        }
        private Boolean GetFromCheckedListPtype(string sSelIDs)
        {
            try
            {
                lblPType.Text = sSelIDs;
                lblPType.Tag = lblPType.Text;
                this.txtProductType.TextChanged -= this.txtProductType_Click;
                txtProductType.Text = GetPtypeAsperIDs(sSelIDs);
                this.txtProductType.TextChanged += this.txtProductType_Click;
                string[] strCostIDs = sSelIDs.Split(',');
                return true;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
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
        private string GetTaxModeAsperIDs(string sIDs = "")
        {
            try
            {
                UspGetTaxModeCheckedListtInfo GetTaxModeChk = new UspGetTaxModeCheckedListtInfo();
                string sRetResult = "";
                if (sIDs != "")
                {
                    DataTable dtData = new DataTable();
                    GetTaxModeChk.IDs = sIDs;
                    GetTaxModeChk.TenantId = Global.gblTenantID;
                    dtData = clsccntr.GetTaxModeCheckedList(GetTaxModeChk);
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
        private string GetAgentAsperIDs(string sIDs = "")
        {
            try
            {
                UspGetAgentCheckedListInfo GetAgentChk = new UspGetAgentCheckedListInfo();
                string sRetResult = "";
                if (sIDs != "")
                {
                    DataTable dtData = new DataTable();
                    GetAgentChk.IDs = sIDs;
                    GetAgentChk.TenantId = Global.gblTenantID;
                    dtData = clsccntr.GetCheckedListAgent(GetAgentChk);
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
        private string GetBillAsperIDs(string sIDs = "")
        {
            try
            {
                UspGetBillCheckedListInfo GetBillChk = new UspGetBillCheckedListInfo();
                string sRetResult = "";
                if (sIDs != "")
                {
                    DataTable dtData = new DataTable();
                    GetBillChk.IDs = sIDs;
                    dtData = clsccntr.GetCheckedListBill(GetBillChk);
                    if (dtData.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dtData.Rows)
                            sRetResult = sRetResult + "'" + dr[0].ToString() + "',";

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
        private string GetMopAsperIDs(string sIDs = "")
        {
            try
            {
                UspGetMopCheckedListInfo GetMopChk = new UspGetMopCheckedListInfo();
                string sRetResult = "";
                if (sIDs != "")
                {
                    DataTable dtData = new DataTable();
                    GetMopChk.IDs = sIDs;
                    dtData = clsccntr.GetCheckedListMop(GetMopChk);
                    if (dtData.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dtData.Rows)
                            sRetResult = sRetResult + "'" + dr[0].ToString() + "',";

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
        private string GetCostCenterAsperIDs(string sIDs = "")
        {
            try
            {
                UspGetCostCenterCheckedListInfo GetCostChk = new UspGetCostCenterCheckedListInfo();
                string sRetResult = "";
                if (sIDs != "")
                {
                    DataTable dtData = new DataTable();
                    GetCostChk.IDs = sIDs;
                    GetCostChk.TenantId = Global.gblTenantID;
                    dtData = clsccntr.GetCostCeneterCheckedList(GetCostChk);
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

        private void txtCostCenterList_Click(object sender, EventArgs e)
        {
            try
            {

                if (string.IsNullOrEmpty(txtCostCenterList.Text))
                {
                    lblCostCenterIds.Text = Convert.ToString(txtCostCenterList.Tag);
                    lblCostCenterIds.Text = "";
                }
                if (this.ActiveControl.Name != "txtCostCenterList")
                    return;
                string sQuery = "SELECT CCID,CCNAme FROM tblCostCentre WHERE TenantID = " + Global.gblTenantID + "";
                new frmCompactCheckedListSearch(GetFromCheckedListCost, sQuery, "CCName", txtCostCenterList.Location.X + 453, txtCostCenterList.Location.Y + 270, 0, 2, txtCostCenterList.Text, 0, 0, "", lblCostCenterIds.Text, null, "Cost Center").ShowDialog();

            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }


        private void txtVoucherTypeList_Click(object sender, EventArgs e)
        {
            txtCostCenterList.ReadOnly = true;
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
                new frmCompactCheckedListSearch(GetFromCheckedListVoucher, sQuery, "VchType", txtVoucherTypeList.Location.X + 771, txtVoucherTypeList.Location.Y + 270, 0, 2, txtVoucherTypeList.Text, 0, 0, "", lblVoucherIds.Text, null, "Voucher Type").ShowDialog();

            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
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

        private void chkCostCenter_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkCostCenter.Checked == true)
                {
                    string Sql = "SELECT CCID,CCNAme FROM tblCostCentre WHERE TenantID = " + Global.gblTenantID + "";
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
                        sStrNames = sStrNames + dt.Rows[i]["CCName"].ToString() + ",";
                        sStrIds = sStrIds + dt.Rows[i]["CCID"].ToString() + ",";

                    }
                    txtCostCenterList.Text = sStrNames.Remove(sStrNames.Length - 1, 1);
                    lblCostCenterIds.Text = sStrIds.Remove(sStrIds.Length - 1, 1);
                }
                else
                {
                    txtCostCenterList.Text = "";
                    chkCostCenter.Checked = false;

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }





        private void chkTaxMode_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                txtTaxMode.ReadOnly = true;
                if (chkTaxMode.Checked == true)
                {
                    string Sql = "Select TaxModeID,TaxMode from tblTaxMode where TenantID = '" + Global.gblTenantID + "'";
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
                        sStrNames = sStrNames + dt.Rows[i]["TaxMode"].ToString() + ",";
                        sStrIds = sStrIds + dt.Rows[i]["TaxModeID"].ToString() + ",";

                    }
                    txtTaxMode.Text = sStrNames.Remove(sStrNames.Length - 1, 1);
                    lblTaxIds.Text = sStrIds.Remove(sStrIds.Length - 1, 1);
                }
                else
                {
                    txtTaxMode.Text = "";
                    chkTaxMode.Checked = false;

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void txtTaxMode_Click(object sender, EventArgs e)
        {
            try
            {

                if (string.IsNullOrEmpty(txtTaxMode.Text))
                {
                    lblTaxIds.Text = Convert.ToString(txtTaxMode.Tag);
                    lblTaxIds.Text = "";
                }
                if (this.ActiveControl.Name != "txtTaxMode")
                    return;
                string sQuery = "Select TaxModeID,TaxMode from tblTaxMode where TenantID = '" + Global.gblTenantID + "'";
                new frmCompactCheckedListSearch(GetFromCheckedListTax, sQuery, "TaxMode", txtTaxMode.Location.X + 772, txtTaxMode.Location.Y + 325, 0, 2, txtTaxMode.Text, 0, 0, "", lblTaxIds.Text, null, "Tax Mode").ShowDialog();

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
                new frmCompactCheckedListSearch(GetFromCheckedListStaff, sQuery, "Name", txtstafflist.Location.X + 453, txtstafflist.Location.Y + 325, 0, 2, txtstafflist.Text, 0, 0, "", lblstaffIds.Text, null, "Sales Staff").ShowDialog();

            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void chkAgent_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                txtAgent.ReadOnly = true;
                if (chkAgent.Checked == true)
                {
                    string Sql = "Select AgentID,AgentName from tblAgent where TenantID = '" + Global.gblTenantID + "'";
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
                        sStrNames = sStrNames + dt.Rows[i]["AgentName"].ToString() + ",";
                        sStrIds = sStrIds + dt.Rows[i]["AgentID"].ToString() + ",";

                    }
                    txtAgent.Text = sStrNames.Remove(sStrNames.Length - 1, 1);
                    lblAgentIds.Text = sStrIds.Remove(sStrIds.Length - 1, 1);
                }
                else
                {
                    txtAgent.Text = "";
                    chkAgent.Checked = false;

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void txtAgent_Click_1(object sender, EventArgs e)
        {
            try
            {

                if (string.IsNullOrEmpty(txtAgent.Text))
                {
                    txtAgent.Text = Convert.ToString(txtAgent.Tag);
                    txtAgent.Text = "";
                }
                if (this.ActiveControl.Name != "txtAgent")
                    return;
                string sQuery = "Select AgentID,AgentName from tblAgent where TenantID = '" + Global.gblTenantID + "'";
                new frmCompactCheckedListSearch(GetFromCheckedListAgent, sQuery, "Name", txtAgent.Location.X + 453, txtAgent.Location.Y + 360, 0, 2, txtAgent.Text, 0, 0, "", lblAgentIds.Text, null, "Agent").ShowDialog();

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

        private void txtuser_Click(object sender, EventArgs e)
        {
            try
            {

                if (string.IsNullOrEmpty(txtuser.Text))
                {
                    txtuser.Text = Convert.ToString(txtuser.Tag);
                    txtuser.Text = "";
                }
                if (this.ActiveControl.Name != "txtuser")
                    return;
                string sQuery = "Select UserID,UserName from tblUserMaster";
                new frmCompactCheckedListSearch(GetFromCheckedListUser, sQuery, "UserName", txtuser.Location.X + 772, txtuser.Location.Y + 360, 0, 2, txtuser.Text, 0, 0, "", lblUserIds.Text, null, "User").ShowDialog();

            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void chkBillType_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                txtBillType.ReadOnly = true;
                if (chkBillType.Checked == true)
                {
                    //string Sql = "Select BID,BillTypeName from tblBillType";
                    string Sql = "Select Distinct 0 as BID,GSTType as BillTypeName from tblSales Order By GSTType ";
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
                        sStrNames = sStrNames + "'" + dt.Rows[i]["BillTypeName"].ToString() + "',";
                        sStrIds = sStrIds + dt.Rows[i]["BID"].ToString() + ",";

                    }
                    txtBillType.Text = sStrNames.Remove(sStrNames.Length - 1, 1);
                    lblBillIds.Text = sStrIds.Remove(sStrIds.Length - 1, 1);
                }
                else
                {
                    txtBillType.Text = "";
                    chkBillType.Checked = false;

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void txtBillType_Click(object sender, EventArgs e)
        {
            try
            {

                if (string.IsNullOrEmpty(txtBillType.Text))
                {
                    txtBillType.Text = Convert.ToString(txtBillType.Tag);
                    txtBillType.Text = "";
                }
                if (this.ActiveControl.Name != "txtBillType")
                    return;
                string sQuery = "Select Distinct GSTType as BID,GSTType as BillTypeName from tblSales Order By GSTType";
                new frmCompactCheckedListSearch(GetFromCheckedListBill, sQuery, "BillTypeName", txtBillType.Location.X + 453, txtBillType.Location.Y + 405, 0, 2, txtBillType.Text, 0, 0, "", lblBillIds.Text, null, "Bill Type", "PurchaseReport", true).ShowDialog();

            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }
        private bool FillSupplierUsingID(int iLedgerID)
        {
            try
            {
                DataTable dtSupp = new DataTable();

                GetLedinfo.LID = iLedgerID;
                GetLedinfo.TenantID = Global.gblTenantID;
                GetLedinfo.GroupName = "SUPPLIER";
                dtSupp = clsLedg.GetLedger(GetLedinfo);
                if (dtSupp.Rows.Count > 0)
                {
                    this.txtCustomer.TextChanged -= this.txtCustomer_Click;
                    txtCustomer.Text = dtSupp.Rows[0]["LName"].ToString();
                    this.txtCustomer.TextChanged += this.txtCustomer_Click;
                    lblLID.Text = dtSupp.Rows[0]["LID"].ToString();
                    txtCustomer.Tag = dtSupp.Rows[0]["LedgerCode"].ToString();
                    return true;
                }
                else
                    return false;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
        }

        private int ConvertI32(decimal dVal)
        {
            try
            {
                return Convert.ToInt32(dVal);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return 0;
            }
        }



        private Boolean GetFromSupplierSearch(string LstIDandText)
        {
            try
            {
                string[] sCompSearchData = LstIDandText.Split('|');
                DataTable dtManf = new DataTable();
                if (sCompSearchData[0].ToString() == "NOTEXIST")
                {
                    return true;
                }
                else
                {
                    if (sCompSearchData.Length > 0)
                    {
                        if (Convert.ToInt32(sCompSearchData[0].ToString()) != 0)
                        {
                            GetLedinfo.LID = Convert.ToInt32(sCompSearchData[0].ToString());
                            GetLedinfo.TenantID = Global.gblTenantID;
                            GetLedinfo.GroupName = "SUPPLIER";
                            dtManf = clsLedg.GetLedger(GetLedinfo);
                            if (dtManf.Rows.Count > 0)
                            {
                                //this.txtCustomer.TextChanged -= this.txtCustomer_Click;
                                //txtCustomer.Text = dtManf.Rows[0]["LName"].ToString();
                                //this.txtCustomer.TextChanged += this.txtCustomer_Click;
                                //txtCustomer.Tag = dtManf.Rows[0]["LID"].ToString();

                                this.txtCustomer.TextChanged -= this.txtCustomer_Click;
                                txtCustomer.Text = dtManf.Rows[0]["LedgerName"].ToString();
                                this.txtCustomer.TextChanged += this.txtCustomer_Click;
                                lblLID.Text = dtManf.Rows[0]["LID"].ToString();
                                txtCustomer.Tag = dtManf.Rows[0]["LedgerCode"].ToString();
                                return true;
                            }
                            return true;
                        }
                        else
                        {
                            this.txtCustomer.TextChanged -= this.txtCustomer_Click;
                            txtCustomer.Text = sCompSearchData[1].ToString();
                            this.txtCustomer.TextChanged += this.txtCustomer_Click;
                            return true;
                        }
                    }
                    else
                        return false;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
        }
        private Boolean GetFromItemSearch(string LstIDandText)
        {
            try
            {
                string[] sCompSearchData = LstIDandText.Split('|');
                DataTable dtManf = new DataTable();
                if (sCompSearchData[0].ToString() == "NOTEXIST")
                {
                    return true;
                }
                else
                {
                    if (sCompSearchData.Length > 0)
                    {
                        if (Convert.ToInt32(sCompSearchData[0].ToString()) != 0)
                        {
                            GetItem.ItemID = Convert.ToInt32(sCompSearchData[0].ToString());
                            GetItem.TenantID = Global.gblTenantID;
                            dtManf = clsitem.GetItemMaster(GetItem);
                            if (dtManf.Rows.Count > 0)
                            {
                                this.txtItem.TextChanged -= this.txtItem_Click;
                                txtItem.Text = dtManf.Rows[0]["ItemName"].ToString();
                                this.txtItem.TextChanged += this.txtItem_Click;
                                txtItem.Tag = dtManf.Rows[0]["ItemID"].ToString();
                            }
                            return true;
                        }
                        else
                        {
                            this.txtItem.TextChanged -= this.txtItem_Click;
                            txtItem.Text = sCompSearchData[1].ToString();
                            this.txtItem.TextChanged += this.txtItem_Click;
                            return true;
                        }
                    }
                    else
                        return false;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
        }

        private void chkMop_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                txtTaxMode.ReadOnly = true;
                if (chkMop.Checked == true)
                {
                    string Sql = "Select distinct 0 as MID, mop as MopName from tblSales Order By MOP";
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
                        sStrNames = sStrNames + "'" + dt.Rows[i]["MopName"].ToString() + "',";
                        sStrIds = sStrIds + dt.Rows[i]["MID"].ToString() + ",";

                    }
                    txtMop.Text = sStrNames.Remove(sStrNames.Length - 1, 1);
                    lblMopIds.Text = sStrIds.Remove(sStrIds.Length - 1, 1);
                }
                else
                {
                    txtMop.Text = "";
                    chkMop.Checked = false;

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void txtMop_Click(object sender, EventArgs e)
        {

            try
            {

                if (string.IsNullOrEmpty(txtMop.Text))
                {
                    txtMop.Text = Convert.ToString(txtMop.Tag);
                    txtMop.Text = "";
                }
                if (this.ActiveControl.Name != "txtMop")
                    return;
                string sQuery = "Select Distinct mop as MID,MOP as MopName from tblSales Order By MOP";
                new frmCompactCheckedListSearch(GetFromCheckedListMop, sQuery, "MopName", txtMop.Location.X + 772, txtMop.Location.Y + 405, 0, 2, txtMop.Text, 0, 0, "", lblMopIds.Text, null, "Mod of Payment", "PurchaseReport", true).ShowDialog();

            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void txtItem_Click(object sender, EventArgs e)
        {
            try
            {
                string sQuery = "SELECT (I.ItemCode+I.ItemName+CONVERT(VARCHAR,ISNULL(I.IGSTTaxPer,0))) as AnyWhere,I.ItemCode,I.ItemName,CONVERT(DECIMAL(18,2),I.IGSTTaxPer) as [GST %],I.CategoryID,I.ItemID,I.UNITID FROM tblItemMaster I INNER JOIN tblCategories C ON C.CategoryID = I.CategoryID  AND I.ActiveStatus = 1 ";

                new frmCompactSearch(GetFromItemSearch, sQuery, "AnyWhere|ItemCode|ItemName", txtItem.Location.X + 453, txtItem.Location.Y + 300, 4, 0, txtItem.Text, 4, 0, "ORDER BY ItemName ASC", 0, 0, "Item Name ...", 0, "270,270,0,0,0", true, "frmItemMaster").ShowDialog();

                this.txtItem.TextChanged -= this.txtItem_Click;
                txtItem.Focus();
                this.txtItem.TextChanged += this.txtItem_Click;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void chkCategory_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                txtCategory.ReadOnly = true;
                if (chkCategory.Checked == true)
                {
                    string Sql = "Select * from tblCategories";
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
                        sStrNames = sStrNames + dt.Rows[i]["Category"].ToString() + ",";
                        sStrIds = sStrIds + dt.Rows[i]["CategoryID"].ToString() + ",";

                    }
                    txtCategory.Text = sStrNames.Remove(sStrNames.Length - 1, 1);
                    lblCatIds.Text = sStrIds.Remove(sStrIds.Length - 1, 1);
                }
                else
                {
                    txtCategory.Text = "";
                    chkCategory.Checked = false;

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void chkMnf_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                txtMnf.ReadOnly = true;
                if (chkMnf.Checked == true)
                {
                    string Sql = "Select MnfID,MnfName from tblManufacturer where TenantID = '" + Global.gblTenantID + "'";
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
                        sStrNames = sStrNames + dt.Rows[i]["MnfName"].ToString() + ",";
                        sStrIds = sStrIds + dt.Rows[i]["MnfID"].ToString() + ",";

                    }
                    txtMnf.Text = sStrNames.Remove(sStrNames.Length - 1, 1);
                    lblMnfIds.Text = sStrIds.Remove(sStrIds.Length - 1, 1);
                }
                else
                {
                    txtMnf.Text = "";
                    chkMnf.Checked = false;

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void chkProductType_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                txtProductType.ReadOnly = true;
                if (chkProductType.Checked == true)
                {
                    string Sql = "Select DISTINCT ProductTypeID,ProductType from tblItemMaster";
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
                        sStrNames = sStrNames + dt.Rows[i]["ProductType"].ToString() + ",";
                        sStrIds = sStrIds + dt.Rows[i]["ProductTypeID"].ToString() + ",";

                    }
                    txtProductType.Text = sStrNames.Remove(sStrNames.Length - 1, 1);
                    lblPType.Text = sStrIds.Remove(sStrIds.Length - 1, 1);
                }
                else
                {
                    txtProductType.Text = "";
                    chkProductType.Checked = false;

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }



        private void txtCategory_Click(object sender, EventArgs e)
        {
            try
            {
                txtCategory.ReadOnly = true;
                try
                {
                    if (string.IsNullOrEmpty(txtCategory.Text))
                    {
                        lblCatIds.Text = Convert.ToString(txtCategory.Tag);
                        lblCatIds.Text = "";
                    }
                    if (this.ActiveControl.Name != "txtCategory")
                        return;
                    string sQuery = "Select CategoryID,Category from tblCategories where TenantID = '" + Global.gblTenantID + "'";
                    new frmCompactCheckedListSearch(GetFromCheckedListCat, sQuery, "Category", txtCategory.Location.X + 453, txtCategory.Location.Y + 450, 0, 2, txtCategory.Text, 0, 0, "", lblCatIds.Text, null, "Category").ShowDialog();
                }
                catch (Exception ex)
                {
                    Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }


        private void rdoDiscountSales_Click(object sender, EventArgs e)
        {
            try
            {
                chkfield.Visible = true;
                chkselectAll.Visible = true;
                chkselectAll.Checked = false;
                flowLayoutPanel1.Visible = true;
                flowLayoutPanel2.Visible = true;
                tlpItem.Visible = false;
                tlpMnf.Visible = false;
                tlpCategory.Visible = false;
                tlpProduct.Visible = false;
                //tableLayoutPanel38.Visible = true; 

                DropPurchaseView();

                //string Sql = "DROP VIEW vwpurchase";
                string Sql1 = "create view vwpurchase as select top 1 CCName as [Cost Center],Mop,VchType as [Voucher Type],Party,PartyCode as [Party Code] ,PartyAddress as [Party Address],PartyGSTIN as [Party GSTIN],BillType as [Bill Type],TaxAmt as [Tax Amount],GrossAmt as [Gross Amount],ItemDiscountTotal as [Item Discount Total],NonTaxable as [Non Taxable],Taxable,CGSTTotal as [CGST Total],SGSTTotal as[SGST Total],IGSTTotal as [IGST Total],CashDiscount as[Cash Discount],tblSales.Discount,OtherExpense as [Other Expense],NetAmount as [Net Amount],RoundOff as [Round Off],BillAmt as [Bill Amount] from tblSales join tblSalesItem on tblSales.InvId = tblSalesItem.InvID join tblItemMaster on tblItemMaster.ItemID = tblSalesItem.ItemId join tblCostCentre on tblCostCentre.CCID=tblSales.CCID";
                SqlConnection conn = new SqlConnection(constr);
                conn.Open();
                //SqlCommand cmd = new SqlCommand(Sql, conn);
                SqlCommand cmd1 = new SqlCommand(Sql1, conn);
                //cmd.ExecuteNonQuery();
                cmd1.ExecuteNonQuery();
                conn.Close();
                string sQuery = "SELECT column_name FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'vwpurchase'";
                DataTable dtCCntr = Comm.fnGetData(sQuery).Tables[0];

                if (dtCCntr.Rows.Count > 0)
                {
                    chkfield.DataSource = dtCCntr;
                    chkfield.DisplayMember = "column_name";

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }
        private void txtCustomer_Click(object sender, EventArgs e)
        {
            try
            {
                toolTipArea.SetToolTip(txtCustomer, "Specify the unique Area Name");

                string sQuery = "SELECT LName+LAliasName+MobileNo+Address as AnyWhere,LALiasname as [Supplier Code],lname as [Supplier Name] ,MobileNo ,Address,LID,Email  FROM tblLedger L";
                if (clsVchType.CustomerSupplierAccGroupList != "")
                    sQuery = sQuery + " INNER JOIN tblAccountGroup A ON A.AccountGroupID = L.AccountGroupID AND A.AccountGroupID IN (" + clsVchType.CustomerSupplierAccGroupList + ")";
                sQuery = sQuery + " WHERE UPPER(L.groupName)='SUPPLIER' or UPPER(L.groupName)='CUSTOMER' AND L.TenantID=" + Global.gblTenantID + "";
                new frmCompactSearch(GetFromSupplierSearch, sQuery, "AnyWhere|LAliasName|LName|MobileNo|Address", txtCustomer.Location.X + 480, txtCustomer.Location.Y - 10, 4, 0, txtCustomer.Text, 4, 0, "ORDER BY L.LAliasName ASC", 0, 0, " Search ...", 0, "100,200,100,200,0", true, "").ShowDialog();

                this.txtCustomer.TextChanged -= this.txtCustomer_Click;
                txtCustomer.Focus();
                this.txtCustomer.TextChanged += this.txtCustomer_Click;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void txtMnf_Click(object sender, EventArgs e)
        {
            txtMnf.ReadOnly = true;
            try
            {
                if (string.IsNullOrEmpty(txtMnf.Text))
                {
                    lblMnfIds.Text = Convert.ToString(txtMnf.Tag);
                    lblMnfIds.Text = "";
                }
                if (this.ActiveControl.Name != "txtMnf")
                    return;
                string sQuery = "Select MnfID,MnfName from tblManufacturer where TenantID = '" + Global.gblTenantID + "'";
                new frmCompactCheckedListSearch(GetFromCheckedListMnf, sQuery, "Category", txtMnf.Location.X + 772, txtMnf.Location.Y + 450, 0, 2, txtMnf.Text, 0, 0, "", lblMnfIds.Text, null, "Manufacturer").ShowDialog();
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private string GetCatAsperIDs(string sIDs = "")
        {
            try
            {
                UspGetCategoryCheckedListInfo GetCatChk = new UspGetCategoryCheckedListInfo();
                string sRetResult = "";
                if (sIDs != "")
                {
                    DataTable dtData = new DataTable();
                    GetCatChk.IDs = sIDs;
                    GetCatChk.TenantId = Global.gblTenantID;
                    dtData = clsccntr.GetCheckedListCat(GetCatChk);
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



        private void txtProductType_Click(object sender, EventArgs e)
        {

            txtProductType.ReadOnly = true;
            try
            {
                if (string.IsNullOrEmpty(txtProductType.Text))
                {
                    lblPType.Text = Convert.ToString(txtProductType.Tag);
                    lblPType.Text = "";
                }
                if (this.ActiveControl.Name != "txtProductType")
                    return;
                string sQuery = "Select DISTINCT ProductTypeID,ProductType from tblItemMaster";
                new frmCompactCheckedListSearch(GetFromCheckedListPtype, sQuery, "ProductType", txtProductType.Location.X + 445, txtProductType.Location.Y + 450, 0, 2, txtProductType.Text, 0, 0, "", lblPType.Text, null, "Product Type").ShowDialog();
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private string GetMnfAsperIDs(string sIDs = "")
        {
            try
            {
                UspGetMnfCheckedListInfo GetCatChk = new UspGetMnfCheckedListInfo();
                string sRetResult = "";
                if (sIDs != "")
                {
                    DataTable dtData = new DataTable();
                    GetCatChk.IDs = sIDs;
                    GetCatChk.TenantId = Global.gblTenantID;
                    dtData = clsccntr.GetMnfCheckedList(GetCatChk);
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

        private void rdoSalesDaybook_Click(object sender, EventArgs e)
        {
            try
            {
                chkfield.Visible = true;
                chkselectAll.Visible = true;
                chkselectAll.Checked = false;
                flowLayoutPanel1.Visible = true;
                flowLayoutPanel2.Visible = true;
                tlpItem.Visible = false;
                tlpMnf.Visible = false;
                tlpCategory.Visible = false;
                tlpProduct.Visible = false;
                //tableLayoutPanel38.Visible = true; 

                DropPurchaseView();

                //string Sql = "DROP VIEW vwpurchase";
                string Sql1 = "create view vwpurchase as select top 1 CCName as [Cost Center],Mop,VchType as [Voucher Type],Party,PartyCode as [Party Code] ,PartyAddress as [Party Address],PartyGSTIN as [Party GSTIN],BillType as [Bill Type],TaxAmt as [Tax Amount],GrossAmt as [Gross Amount],ItemDiscountTotal as [Item Discount Total],NonTaxable as [Non Taxable],Taxable,CGSTTotal as [CGST Total],SGSTTotal as[SGST Total],IGSTTotal as [IGST Total],CashDiscount as[Cash Discount],tblSales.Discount,OtherExpense as [Other Expense],NetAmount as [Net Amount],RoundOff as [Round Off],BillAmt as [Bill Amount] from tblSales join tblSalesItem on tblSales.InvId = tblSalesItem.InvID join tblItemMaster on tblItemMaster.ItemID = tblSalesItem.ItemId join tblCostCentre on tblCostCentre.CCID=tblSales.CCID";
                SqlConnection conn = new SqlConnection(constr);
                conn.Open();
                //SqlCommand cmd = new SqlCommand(Sql, conn);
                SqlCommand cmd1 = new SqlCommand(Sql1, conn);
                //cmd.ExecuteNonQuery();
                cmd1.ExecuteNonQuery();
                conn.Close();
                string sQuery = "SELECT column_name FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'vwpurchase'";
                DataTable dtCCntr = Comm.fnGetData(sQuery).Tables[0];

                if (dtCCntr.Rows.Count > 0)
                {
                    chkfield.DataSource = dtCCntr;
                    chkfield.DisplayMember = "column_name";

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void rdoDaybookDetails_Click(object sender, EventArgs e)
        {
            try
            {
                tlpItem.Visible = true;
                tlpMnf.Visible = true;
                tlpCategory.Visible = true;
                tlpProduct.Visible = true;
                chkselectAll.Visible = true;
                chkselectAll.Checked = false;
                chkfield.Visible = true;
                string sQuery = "SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = N'tblSales' and ORDINAL_POSITION in(2,5,6,7,10,28,11,12,13,14,16,18,19,20,21,24,31,33,42,43,44,45,78,79,80,83)";
                DataTable dtCCntr = Comm.fnGetData(sQuery).Tables[0];

                if (dtCCntr.Rows.Count > 0)
                {
                    chkfield.DataSource = dtCCntr;
                    chkfield.DisplayMember = "COLUMN_NAME";
                    chkfield.ValueMember = "ORDINAL_POSITION";
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void rdoPurchaseSummary_Click(object sender, EventArgs e)
        {
            try
            {
                tlpItem.Visible = false;
                tlpMnf.Visible = false;
                tlpCategory.Visible = false;
                tlpProduct.Visible = false;
                chkselectAll.Checked = false;
                chkfield.Visible = true;
                string sQuery = "SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = N'tblSales' and ORDINAL_POSITION in(2,5,6,7,10,28,11,12,13,14,16,18,19,20,21,24,31,33,42,43,44,45,78,79,80,83)";
                DataTable dtCCntr = Comm.fnGetData(sQuery).Tables[0];

                if (dtCCntr.Rows.Count > 0)
                {
                    chkfield.DataSource = dtCCntr;
                    chkfield.DisplayMember = "COLUMN_NAME";
                    chkfield.ValueMember = "ORDINAL_POSITION";
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void chkselectAll_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkselectAll.Checked == true)
                {
                    for (int i = 0; i < chkfield.Items.Count; i++)
                    {
                        chkfield.SetItemChecked(i, true);
                    }
                }
                else
                {
                    for (int i = 0; i < chkfield.Items.Count; i++)
                    {
                        chkfield.SetItemChecked(i, false);
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }



        private string GetPtypeAsperIDs(string sIDs = "")
        {
            try
            {
                UspGetPtypeCheckedListInfo GetCatChk = new UspGetPtypeCheckedListInfo();
                string sRetResult = "";
                if (sIDs != "")
                {
                    DataTable dtData = new DataTable();
                    GetCatChk.IDs = sIDs;
                    dtData = clsccntr.GetPtypeCheckedList(GetCatChk);
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

        private void button1_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (txtBillType.Text == "")
                {
                    chkBillType.Checked = true;
                }
                if (txtCostCenterList.Text == "")
                {
                    chkCostCenter.Checked = true;
                }
                if (txtVoucherTypeList.Text == "")
                {
                    chkVoucher.Checked = true;
                }
                if (txtuser.Text == "")
                {
                    chkuser.Checked = true;
                }
                if (txtTaxMode.Text == "")
                {
                    chkTaxMode.Checked = true;
                }
                if (txtstafflist.Text == "")
                {
                    chkstaff.Checked = true;
                }
                if (txtProductType.Text == "")
                {
                    chkProductType.Checked = true;
                }
                if (txtMnf.Text == "")
                {
                    chkMnf.Checked = true;
                }
                if (txtCategory.Text == "")
                {
                    chkCategory.Checked = true;
                }
                if (txtAgent.Text == "")
                {
                    chkAgent.Checked = true;
                }
                if (txtMop.Text == "")
                {
                    chkMop.Checked = true;
                }
                if (chkfield.CheckedItems.Count == 0)
                {
                    chkselectAll.Checked = true;
                }

                string d = "";
                string cost = txtCostCenterList.Text;
                string vchtype = txtVoucherTypeList.Text;
                string mop = txtMop.Text;
                string from = dtpFD.Text;
                string to = dtpTD.Text;
                if (chkfield.CheckedItems.Count != 0)
                {

                    string s = "InvNo as [Invoice No],InvDate as [Invoice Date],AutoNum,";


                    for (int x = 0; x < chkfield.CheckedItems.Count; x++)
                    {
                        if (chkfield.GetItemText(chkfield.CheckedItems[x]) == "Qty" || chkfield.GetItemText(chkfield.CheckedItems[x]) == "Rate" || chkfield.GetItemText(chkfield.CheckedItems[x]) == "MRP")
                        {
                            s = s + "tblSalesItem." + chkfield.GetItemText(chkfield.CheckedItems[x]) + ",";
                        }
                        else if (chkfield.GetItemText(chkfield.CheckedItems[x]) == "Cost Center")
                        {
                            s = s + "CCName as [Cost Center],";
                        }
                        else if (chkfield.GetItemText(chkfield.CheckedItems[x]) == "Voucher Type")
                        {
                            s = s + "VchType as [Voucher Type],";
                        }
                        else if (chkfield.GetItemText(chkfield.CheckedItems[x]) == "Party Code")
                        {
                            s = s + " PartyCode as [Party Code],";
                        }
                        else if (chkfield.GetItemText(chkfield.CheckedItems[x]) == "Party Address")
                        {
                            s = s + "PartyAddress as [Party Address],";
                        }
                        else if (chkfield.GetItemText(chkfield.CheckedItems[x]) == "Party GSTIN")
                        {
                            s = s + "PartyGSTIN as [Party GSTIN],";
                        }
                        else if (chkfield.GetItemText(chkfield.CheckedItems[x]) == "Bill Type")
                        {
                            s = s + "BillType as [Bill Type],";
                        }
                        else if (chkfield.GetItemText(chkfield.CheckedItems[x]) == "Tax Amount")
                        {
                            s = s + "cast(TaxAmt as numeric(36,2)) as [Tax Amount],";
                        }
                        else if (chkfield.GetItemText(chkfield.CheckedItems[x]) == "ITax Amount")
                        {
                            s = s + "cast(TaxAmount as numeric(36,2)) as [Tax Amount],";
                        }
                        else if (chkfield.GetItemText(chkfield.CheckedItems[x]) == "Gross Amount")
                        {
                            s = s + "cast(GrossAmt as numeric(36,2)) as [Gross Amount],";
                        }
                        else if (chkfield.GetItemText(chkfield.CheckedItems[x]) == "IGross Amount")
                        {
                            s = s + "cast(GrossAmount as numeric(36,2)) as [Gross Amount],";
                        }
                        else if (chkfield.GetItemText(chkfield.CheckedItems[x]) == "Item Discount Total")
                        {
                            s = s + "cast(ItemDiscountTotal as numeric(36,2)) as [Item Discount Total],";
                        }
                        else if (chkfield.GetItemText(chkfield.CheckedItems[x]) == "Non Taxable")
                        {
                            s = s + "cast(NonTaxable as numeric(36,2)) as [Non Taxable],";
                        }
                        else if (chkfield.GetItemText(chkfield.CheckedItems[x]) == "INon Taxable")
                        {
                            s = s + "cast(InonTaxableAmount as numeric(36,2)) as [Non Taxable],";
                        }
                        else if (chkfield.GetItemText(chkfield.CheckedItems[x]) == "ITaxableAmount")
                        {
                            s = s + "cast(ITaxableAmount as numeric(36,2)) as [ITaxable],";
                        }
                        else if (chkfield.GetItemText(chkfield.CheckedItems[x]) == "ITaxable")
                        {
                            s = s + "cast(TaxableAmount as numeric(36,2)) as [Taxable],";
                        }
                        else if (chkfield.GetItemText(chkfield.CheckedItems[x]) == "IGST Total")
                        {
                            s = s + "cast(IGSTTotal as numeric(36,2)) as [IGST Total],";
                        }
                        else if (chkfield.GetItemText(chkfield.CheckedItems[x]) == "CGST Total")
                        {
                            s = s + "cast(CGSTTotal as numeric(36,2)) as [CGST Total],";
                        }
                        else if (chkfield.GetItemText(chkfield.CheckedItems[x]) == "SGST Total")
                        {
                            s = s + "cast(SGSTTotal as numeric(36,2)) as [SGST Total],";
                        }
                        else if (chkfield.GetItemText(chkfield.CheckedItems[x]) == "Bill Amount")
                        {
                            s = s + "cast(BillAmt as numeric(36,2)) as [Bill Amount],";
                        }
                        else if (chkfield.GetItemText(chkfield.CheckedItems[x]) == "Party Name")
                        {
                            s = s + "Party as [Party Name],";
                        }
                        else if (chkfield.GetItemText(chkfield.CheckedItems[x]) == "Round Off")
                        {
                            s = s + "cast(Roundoff as numeric(36,2)) as [Round Off],";
                        }
                        else if (chkfield.GetItemText(chkfield.CheckedItems[x]) == "Cash Discount")
                        {
                            s = s + "cast(CashDiscount as numeric(36,2)) as [Cash Discount],";
                        }
                        else if (chkfield.GetItemText(chkfield.CheckedItems[x]) == "Other Expense")
                        {
                            s = s + "cast(OtherExpense as numeric(36,2)) as [Other Expense],";
                        }
                        else if (chkfield.GetItemText(chkfield.CheckedItems[x]) == "Item Code")
                        {
                            s = s + "ItemCode as [Item Code],";
                        }
                        else if (chkfield.GetItemText(chkfield.CheckedItems[x]) == "Item Name")
                        {
                            s = s + "ItemName as [Item Name],";
                        }
                        else if (chkfield.GetItemText(chkfield.CheckedItems[x]) == "HsnCode")
                        {
                            s = s + "HSNID as HsnCode,";
                        }
                        else if (chkfield.GetItemText(chkfield.CheckedItems[x]) == "Product Type")
                        {
                            s = s + "ProductType as [Product Type],";
                        }
                        else if (chkfield.GetItemText(chkfield.CheckedItems[x]) == "Item Discount")
                        {
                            s = s + "cast(ItemDiscount as numeric(36,2)) as [Item Discount],";
                        }
                        else if (chkfield.GetItemText(chkfield.CheckedItems[x]) == "Net Amount")
                        {
                            s = s + "cast(NetAmount as numeric(36,2)) as [Net Amount],";
                        }
                        else if (chkfield.GetItemText(chkfield.CheckedItems[x]) == "Item Net Amount")
                        {
                            s = s + "cast(INetAmount as numeric(36,2)) as [Net Amount],";
                        }

                    
                        else if (chkfield.GetItemText(chkfield.CheckedItems[x]) == "CGST Tax Per")
                        {
                            s = s + "CGSTTaxPer as [CGST Tax Per],";
                        }
                        else if (chkfield.GetItemText(chkfield.CheckedItems[x]) == "CGST Tax Amt")
                        {
                            s = s + "CGSTTaxAmt as [CGST Tax Amt],";
                        }
                        else if (chkfield.GetItemText(chkfield.CheckedItems[x]) == "SGST Tax Per")
                        {
                            s = s + "SGSTTaxPer as [SGST Tax Per],";
                        }
                        else if (chkfield.GetItemText(chkfield.CheckedItems[x]) == "SGST Tax Amt")
                        {
                            s = s + "cast(SGSTTaxAmt as numeric(36,2)) as [SGST Tax Amt],";
                        }
                        else if (chkfield.GetItemText(chkfield.CheckedItems[x]) == "IGST Tax Per")
                        {
                            s = s + "IGSTTaxPer as[IGST Tax Per],";
                        }

                        else if (chkfield.GetItemText(chkfield.CheckedItems[x]) == "IGST Tax Amt")
                        {
                            s = s + "cast(IGSTTaxAmt as numeric(36,2)) as [IGST Tax Amt],";
                        }
                        else
                        {
                            s = s + chkfield.GetItemText(chkfield.CheckedItems[x]) + ",";
                        }
                        

                    }



                    d = s.Remove(s.Length - 1, 1);
                }
                if (rdoSalesDaybook.Checked == true)
                {
                    DropPurchaseView();
                    string Fname = "Sales Daybook";

                    if (rdoDefault.Checked == true)
                    {

                        DateTime FD = Convert.ToDateTime(dtpFD.Text);
                        DateTime TD = Convert.ToDateTime(dtpTD.Text);

                        string sqlSupplier = "";

                        if (txtCustomer.Text != "")
                        {
                            sqlSupplier = " tblSales.LedgerId = " + Conversion.Val(lblLID.Text) + "  AND ";
                        }

                        string Sql1 = "create view vwpurchase as SELECT " + d +
                                        " " +
                                        " FROM   tblSales INNER JOIN tblcostcentre    ON tblcostcentre.ccid = tblSales.ccid" +
                                        " WHERE " + sqlSupplier + " invdate BETWEEN '" + FD.ToString("dd-MMM-yyyy") + "' and '" + TD.ToString("dd-MMM-yyyy") + "'  AND tblSales.VchTypeID IN (" + lblVoucherIds.Text + ") " +
                                        " AND tblSales.TaxModeID IN (" + lblTaxIds.Text + ")   AND tblSales.billtype IN (" + txtBillType.Text + ")   AND mop  IN (" + txtMop.Text + ")" +
                                        " AND salesmanid         IN (" + lblstaffIds.Text + ") " +
                                        " AND tblSales.ccid IN (" + lblCostCenterIds.Text + ") " +
                                        " AND tblSales.userid IN (" + lblUserIds.Text + ") " +
                                        " AND agentid            IN (" + lblAgentIds.Text + ") ";

                        SqlConnection conn1 = new SqlConnection(constr);
                        conn1.Open();
                        SqlCommand cmd1 = new SqlCommand(Sql1, conn1);
                        cmd1.ExecuteNonQuery();

                        conn1.Close();

                        new frmReportView1(Fname, vchtype, cost, mop, from, to, this.MdiParent, "").Show();
                    }
                    else if (rdoexcel.Checked == true)
                    {
                        Com.MessageboxToasted("Report", "Report Show in Excel....");

                    }
                }
                if (rdoDaybookDetails.Checked == true)
                {

                    DropPurchaseView();

                    string Fname = "Sales Detail Daybook";

                    if (rdoDefault.Checked == true)
                    {

                        DateTime FD = Convert.ToDateTime(dtpFD.Text);
                        DateTime TD = Convert.ToDateTime(dtpTD.Text);

                        string sqlSupplier = "";
                        if (txtCustomer.Text != "")
                        {
                            sqlSupplier = " tblSales.LedgerId = " + Conversion.Val(lblLID.Text) + "  AND ";
                        }

                        string Sql1 = "create view vwpurchase as SELECT " + d +
                                         " " +
                                         " FROM   tblSales INNER JOIN tblSalesitem ON tblSales.InvId=tblSalesitem.InvId INNER JOIN tblItemMaster on tblItemMaster.ItemID=tblSalesItem.ItemId INNER JOIN tblcostcentre ON tblcostcentre.ccid = tblSales.ccid" +
                                         " WHERE " + sqlSupplier + " invdate BETWEEN '" + FD.ToString("dd-MMM-yyyy") + "' and '" + TD.ToString("dd-MMM-yyyy") + "'  AND tblSales.VchTypeID IN (" + lblVoucherIds.Text + ") " +
                                         " AND tblSales.TaxModeID IN (" + lblTaxIds.Text + ")   AND tblSales.billtype IN (" + txtBillType.Text + ")   AND mop  IN (" + txtMop.Text + ")" +
                                         " AND salesmanid         IN (" + lblstaffIds.Text + ") " +
                                         " AND tblSales.ccid IN (" + lblCostCenterIds.Text + ") " +
                                         " AND tblSales.userid IN (" + lblUserIds.Text + ") " +
                                         " AND agentid            IN (" + lblAgentIds.Text + ") ";



                        SqlConnection conn1 = new SqlConnection(constr);
                        conn1.Open();
                        SqlCommand cmd1 = new SqlCommand(Sql1, conn1);
                        cmd1.ExecuteNonQuery();

                        conn1.Close();
                        new frmReportView1(Fname, vchtype, cost, mop, from, to, this.MdiParent).Show();
                    }
                    else if (rdoexcel.Checked == true)
                    {
                        Com.MessageboxToasted("Report", "Report Show in Excel....");

                    }
                }

                if (rdbHsncode.Checked == true)
                {

                    DropPurchaseView();

                    string Fname = "Sales Hsncode Wise";

                    if (rdoDefault.Checked == true)
                    {

                        DateTime FD = Convert.ToDateTime(dtpFD.Text);
                        DateTime TD = Convert.ToDateTime(dtpTD.Text);
                        string sqlSupplier = "";
                        if (txtCustomer.Text != "")
                        {
                            sqlSupplier = " tblSales.LedgerId = " + Conversion.Val(lblLID.Text) + "  AND ";
                        }

                        string Sql1 = "create view vwpurchase as SELECT " + d +
                                         " " +
                                         " FROM   tblSales INNER JOIN tblSalesitem ON tblSales.InvId=tblSalesitem.InvId INNER JOIN tblItemMaster on tblItemMaster.ItemID=tblSalesItem.ItemId INNER JOIN tblcostcentre ON tblcostcentre.ccid = tblSales.ccid" +
                                         " WHERE " + sqlSupplier + " invdate BETWEEN '" + FD.ToString("dd-MMM-yyyy") + "' and '" + TD.ToString("dd-MMM-yyyy") + "'  AND tblSales.VchTypeID IN (" + lblVoucherIds.Text + ") " +
                                         " AND tblSales.TaxModeID IN (" + lblTaxIds.Text + ")   AND tblSales.billtype IN (" + txtBillType.Text + ")   AND mop  IN (" + txtMop.Text + ")" +
                                         " AND salesmanid         IN (" + lblstaffIds.Text + ") " +
                                         " AND tblSales.ccid IN (" + lblCostCenterIds.Text + ") " +
                                         " AND tblSales.userid IN (" + lblUserIds.Text + ") " +
                                         " AND agentid            IN (" + lblAgentIds.Text + ") ";



                        SqlConnection conn1 = new SqlConnection(constr);
                        conn1.Open();
                        SqlCommand cmd1 = new SqlCommand(Sql1, conn1);
                        cmd1.ExecuteNonQuery();

                        conn1.Close();
                        new frmReportView1(Fname, vchtype, cost, mop, from, to, this.MdiParent).Show();
                    }
                    else if (rdoexcel.Checked == true)
                    {
                        Com.MessageboxToasted("Report", "Report Show in Excel....");

                    }
                }
                if (rdoDiscountSales.Checked == true)
                {
                    DropPurchaseView();
                    string Fname = "Sales Discount";

                    if (rdoDefault.Checked == true)
                    {

                        DateTime FD = Convert.ToDateTime(dtpFD.Text);
                        DateTime TD = Convert.ToDateTime(dtpTD.Text);

                        string sqlSales = "";

                        if (txtCustomer.Text != "")
                        {
                            sqlSales = " tblSales.LedgerId = " + Conversion.Val(lblLID.Text) + "  AND ";
                        }

                        string Sql1 = "create view vwpurchase as SELECT " + d +
                                        " " +
                                        " FROM   tblSales INNER JOIN tblcostcentre    ON tblcostcentre.ccid = tblSales.ccid" +
                                        " WHERE " + sqlSales + " invdate BETWEEN '" + FD.ToString("dd-MMM-yyyy") + "' and '" + TD.ToString("dd-MMM-yyyy") + "'  AND tblSales.VchTypeID IN (" + lblVoucherIds.Text + ") " +
                                        " AND tblSales.TaxModeID IN (" + lblTaxIds.Text + ")   AND tblSales.billtype IN (" + txtBillType.Text + ") AND  Discount>0  AND mop  IN (" + txtMop.Text + ")" +
                                        " AND salesmanid         IN (" + lblstaffIds.Text + ") " +
                                        " AND tblSales.ccid IN (" + lblCostCenterIds.Text + ") " +
                                        " AND tblSales.userid IN (" + lblUserIds.Text + ") " +
                                        " AND agentid            IN (" + lblAgentIds.Text + ") ";

                        SqlConnection conn1 = new SqlConnection(constr);
                        conn1.Open();
                        SqlCommand cmd1 = new SqlCommand(Sql1, conn1);
                        cmd1.ExecuteNonQuery();

                        conn1.Close();

                        new frmReportView1(Fname, vchtype, cost, mop, from, to, this.MdiParent).Show();
                    }
                    else if (rdoexcel.Checked == true)
                    {
                        Com.MessageboxToasted("Report", "Report Show in Excel....");

                    }
                }
                if (rdbItems.Checked == true)
                {


                    DropPurchaseView();

                    string Fname = "Sales Item Wise";

                    if (rdoDefault.Checked == true)
                    {

                        DateTime FD = Convert.ToDateTime(dtpFD.Text);
                        DateTime TD = Convert.ToDateTime(dtpTD.Text);
                        string sqlSupplier = "";
                        string sqlitem = "";
                        if (txtCustomer.Text != "")
                        {
                            sqlSupplier = " tblSales.LedgerId = " + Conversion.Val(lblLID.Text) + "  AND ";
                        }
                        if (txtItem.Text != "")
                        {
                            sqlitem = "tblItemMaster.ItemCode='" + txtItem.Text + "' AND ";
                        }

                        string Sql1 = "create view vwpurchase as SELECT " + d +
                                         " " +
                                         " FROM   tblSales INNER JOIN tblSalesitem ON tblSales.InvId=tblSalesitem.InvId INNER JOIN tblItemMaster on tblItemMaster.ItemID=tblSalesItem.ItemId INNER JOIN tblcostcentre ON tblcostcentre.ccid = tblSales.ccid INNER JOIN tblCategories on tblCategories.CategoryID = tblItemMaster.CategoryID" +
                                         " WHERE " + sqlSupplier + " invdate BETWEEN '" + FD.ToString("dd-MMM-yyyy") + "' and '" + TD.ToString("dd-MMM-yyyy") + "'  AND tblSales.VchTypeID IN (" + lblVoucherIds.Text + ") " +
                                         " AND " + sqlitem + " tblSales.TaxModeID IN (" + lblTaxIds.Text + ")   AND tblSales.billtype IN (" + txtBillType.Text + ")   AND mop  IN (" + txtMop.Text + ")" +
                                         " AND salesmanid         IN (" + lblstaffIds.Text + ") " +
                                         " AND tblSales.ccid IN (" + lblCostCenterIds.Text + ") " +
                                         " AND tblSales.userid IN (" + lblUserIds.Text + ") " +
                                         " AND agentid            IN (" + lblAgentIds.Text + ") " +
                                         " AND tblItemMaster.MNFID IN (" + lblMnfIds.Text + ") " +
                                         " AND tblItemMaster.CategoryID IN (" + lblCatIds.Text + ") " +
                                         " AND tblItemMaster.ProductTypeID IN (" + lblPType.Text + ")";



                        SqlConnection conn1 = new SqlConnection(constr);
                        conn1.Open();
                        SqlCommand cmd1 = new SqlCommand(Sql1, conn1);
                        cmd1.ExecuteNonQuery();

                        conn1.Close();

                        new frmReportView1(Fname, vchtype, cost, mop, from, to, this.MdiParent).Show();
                    }
                    else if (rdoexcel.Checked == true)
                    {
                        Com.MessageboxToasted("Report", "Report Show in Excel....");

                    }
                }
                if (rdbTax.Checked == true)
                {
                    //string Sql = "DROP VIEW vwpurchase";
                    //SqlConnection conn = new SqlConnection(constr);
                    //conn.Open();
                    //SqlCommand cmd = new SqlCommand(Sql, conn);
                    //cmd.ExecuteNonQuery();
                    //conn.Close();

                    DropPurchaseView();

                    string Fname = "Sales Tax split";

                    if (rdoDefault.Checked == true)
                    {
                        DateTime FD = Convert.ToDateTime(dtpFD.Text);
                        DateTime TD = Convert.ToDateTime(dtpTD.Text);

                        string sqlSupplier = "";
                        if (txtCustomer.Text != "")
                        {
                            sqlSupplier = " tblSales.LedgerId = " + Conversion.Val(lblLID.Text) + "  AND ";
                        }

                        string Sql1 = "create view vwpurchase as SELECT " + d +
                                         " " +
                                         " FROM   tblSales INNER JOIN tblSalesitem ON tblSales.InvId=tblSalesitem.InvId  INNER JOIN tblcostcentre ON tblcostcentre.ccid = tblSales.ccid" +
                                         " WHERE " + sqlSupplier + " invdate BETWEEN '" + FD.ToString("dd-MMM-yyyy") + "' and '" + TD.ToString("dd-MMM-yyyy") + "'  AND tblSales.VchTypeID IN (" + lblVoucherIds.Text + ") " +
                                         " AND tblSales.TaxModeID IN (" + lblTaxIds.Text + ")   AND tblSales.billtype IN (" + txtBillType.Text + ")   AND mop  IN (" + txtMop.Text + ")" +
                                         " AND salesmanid         IN (" + lblstaffIds.Text + ") " +
                                         " AND tblSales.ccid IN (" + lblCostCenterIds.Text + ") " +
                                         " AND tblSales.userid IN (" + lblUserIds.Text + ") " +
                                         " AND agentid            IN (" + lblAgentIds.Text + ") ";



                        SqlConnection conn1 = new SqlConnection(constr);
                        conn1.Open();
                        SqlCommand cmd1 = new SqlCommand(Sql1, conn1);
                        cmd1.ExecuteNonQuery();

                        conn1.Close();
                        new frmReportView1(Fname, vchtype, cost, mop, from, to, this.MdiParent).Show();
                    }
                    else if (rdoexcel.Checked == true)
                    {
                        Com.MessageboxToasted("Report", "Report Show in Excel....");

                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void rdoPurchaseDaybook_Click(object sender, EventArgs e)
        {
            try
            {
                chkfield.Visible = true;
                chkselectAll.Visible = true;
                chkselectAll.Checked = false;
                flowLayoutPanel1.Visible = true;
                flowLayoutPanel2.Visible = true;
                tlpItem.Visible = false;
                tlpMnf.Visible = false;
                tlpCategory.Visible = false;
                tlpProduct.Visible = false;
                //tableLayoutPanel38.Visible = true; 

                DropPurchaseView();

                //string Sql = "DROP VIEW vwpurchase";
                // string Sql1 = "create view vwpurchase as select top 1 CCName as [Cost Center],Mop,VchType,Party,PartyCode,PartyAddress,PartyGSTIN,BillType,TaxAmt,GrossAmt,ItemDiscountTotal,NonTaxable,Taxable,CGSTTotal,SGSTTotal,IGSTTotal,FloodCessTot,CashDiscount,OtherExpense,NetAmount,RoundOff,BillAmt from tblSales join tblSalesItem on tblSales.InvId = tblSalesItem.InvID join tblItemMaster on tblItemMaster.ItemID = tblSalesItem.ItemId join tblCostCentre on tblCostCentre.CCID=tblSales.CCID";
                string Sql1 = "create view vwpurchase as select top 1 CCName as [Cost Center],Mop,VchType as [Voucher Type],Party,PartyCode as [Party Code] ,PartyAddress as [Party Address],PartyGSTIN as [Party GSTIN],BillType as [Bill Type],TaxAmt as [Tax Amount],GrossAmt as [Gross Amount],ItemDiscountTotal as [ItemDiscount Total],NonTaxable as [Non Taxable],Taxable,CGSTTotal,SGSTTotal,IGSTTotal,FloodCessTot as [FloodCess Total],CashDiscount,OtherExpense,NetAmount,RoundOff,BillAmt as [Bill Amount] from tblSales join tblSalesItem on tblSales.InvId = tblSalesItem.InvID join tblItemMaster on tblItemMaster.ItemID = tblSalesItem.ItemId join tblCostCentre on tblCostCentre.CCID=tblSales.CCID";
                SqlConnection conn = new SqlConnection(constr);
                conn.Open();
                //SqlCommand cmd = new SqlCommand(Sql, conn);
                SqlCommand cmd1 = new SqlCommand(Sql1, conn);
                //cmd.ExecuteNonQuery();
                cmd1.ExecuteNonQuery();
                conn.Close();
                string sQuery = "SELECT column_name FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'vwpurchase'";
                DataTable dtCCntr = Comm.fnGetData(sQuery).Tables[0];

                if (dtCCntr.Rows.Count > 0)
                {
                    chkfield.DataSource = dtCCntr;
                    chkfield.DisplayMember = "column_name";

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }

        private void tlpSearch_Paint(object sender, PaintEventArgs e)
        {

        }

        private void rdoSalesDaybook_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                chkfield.Visible = true;
                chkselectAll.Visible = true;
                chkselectAll.Checked = false;
                flowLayoutPanel1.Visible = true;
                flowLayoutPanel2.Visible = true;
                tlpItem.Visible = false;
                tlpMnf.Visible = false;
                tlpCategory.Visible = false;
                tlpProduct.Visible = false;
                //tableLayoutPanel38.Visible = true; 

                DropPurchaseView();

                //string Sql = "DROP VIEW vwpurchase";
                string Sql1 = "create view vwpurchase as select top 1 CCName as [Cost Center],Mop,VchType as [Voucher Type],Party,PartyCode as [Party Code] ,PartyAddress as [Party Address],PartyGSTIN as [Party GSTIN],BillType as [Bill Type],TaxAmt as [Tax Amount],GrossAmt as [Gross Amount],ItemDiscountTotal as [Item Discount Total],NonTaxable as [Non Taxable],Taxable,CGSTTotal as [CGST Total],SGSTTotal as[SGST Total],IGSTTotal as [IGST Total],CashDiscount as[Cash Discount],tblSales.Discount,OtherExpense as [Other Expense],NetAmount as [Net Amount],RoundOff as [Round Off],BillAmt as [Bill Amount] from tblSales join tblSalesItem on tblSales.InvId = tblSalesItem.InvID join tblItemMaster on tblItemMaster.ItemID = tblSalesItem.ItemId join tblCostCentre on tblCostCentre.CCID=tblSales.CCID";
                SqlConnection conn = new SqlConnection(constr);
                conn.Open();
                //SqlCommand cmd = new SqlCommand(Sql, conn);
                SqlCommand cmd1 = new SqlCommand(Sql1, conn);
                //cmd.ExecuteNonQuery();
                cmd1.ExecuteNonQuery();
                conn.Close();
                string sQuery = "SELECT column_name FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'vwpurchase'";
                DataTable dtCCntr = Comm.fnGetData(sQuery).Tables[0];

                if (dtCCntr.Rows.Count > 0)
                {
                    chkfield.DataSource = dtCCntr;
                    chkfield.DisplayMember = "column_name";

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }

        private void rdoDaybookDetails_Click_1(object sender, EventArgs e)
        {
            try
            {
                chkfield.Visible = true;
                chkselectAll.Visible = true;
                chkselectAll.Checked = false;
                tlpItem.Visible = false;
                tlpMnf.Visible = false;
                tlpCategory.Visible = false;
                tlpProduct.Visible = false;
                flowLayoutPanel1.Visible = true;
                flowLayoutPanel2.Visible = true;
                //tableLayoutPanel38.Visible = true;

                DropPurchaseView();

                string Sql1 = "create view vwpurchase as select top 1 CCName as [Cost Center],Mop,VchType as [Voucher Type],Party,PartyCode as [Party Code] ,PartyAddress as [Party Address],PartyGSTIN as [Party GSTIN],BillType as [Bill Type],ItemCode as [Item Code],ItemName as [Item Name],tblSalesItem.Qty as Qty,tblSalesItem.Rate as Rate,GrossAmount as [IGross Amount],TaxAmount as [ITax Amount],ItemDiscountTotal as [Item Discount Total],InonTaxableAmount as [INon Taxable],ITaxableAmount,CGSTTotal as [CGST Total],SGSTTotal as[SGST Total],IGSTTotal as [IGST Total],CashDiscount as[Cash Discount],OtherExpense as [Other Expense],INetAmount as [Item Net Amount] from tblSales join tblSalesItem on tblSales.InvId = tblSalesItem.InvID join tblItemMaster on tblItemMaster.ItemID = tblSalesItem.ItemId join tblCostCentre on tblCostCentre.CCID=tblSales.CCID";
                SqlConnection conn = new SqlConnection(constr);
                conn.Open();
                //SqlCommand cmd = new SqlCommand(Sql, conn);
                SqlCommand cmd1 = new SqlCommand(Sql1, conn);
                //cmd.ExecuteNonQuery();
                cmd1.ExecuteNonQuery();
                conn.Close();
                string sQuery = "SELECT column_name FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'vwpurchase'";
                DataTable dtCCntr = Comm.fnGetData(sQuery).Tables[0];

                if (dtCCntr.Rows.Count > 0)
                {
                    chkfield.DataSource = dtCCntr;
                    chkfield.DisplayMember = "column_name";

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }

        private void rdoPurchaseSummary_Click_1(object sender, EventArgs e)
        {
            try
            {
                chkfield.Visible = true;
                chkselectAll.Visible = true;
                tlpItem.Visible = false;
                tlpMnf.Visible = false;
                tlpCategory.Visible = false;
                flowLayoutPanel1.Visible = true;
                flowLayoutPanel2.Visible = true;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void rdbItems_Click(object sender, EventArgs e)
        {
            try
            {
                chkfield.Visible = true;
                chkselectAll.Visible = true;
                chkselectAll.Checked = false;
                tlpItem.Visible = true;
                tlpMnf.Visible = true;
                tlpProduct.Visible = true;
                tlpCategory.Visible = true;
                flowLayoutPanel1.Visible = true;
                //tableLayoutPanel38.Visible = false;
                flowLayoutPanel2.Visible = true;

                DropPurchaseView();

                //string Sql = "DROP VIEW vwpurchase";
                string Sql1 = "create view vwpurchase as select top 1 ItemCode as [Item Code],ItemName as [Item Name],Category,Manufacturer,ProductType as [Product Type],tblSalesItem.Qty,tblSalesItem.Rate,tblSalesItem.MRP,GrossAmount as [IGross Amount],ItemDiscount as [Item Discount],HSNID as HsnCode,TaxPer,InonTaxableAmount as [INon Taxable],ITaxableAmount,TaxAmount as [ITax Amount],INetAmount as [Item Net Amount] from tblSales join tblSalesItem on tblSales.InvId = tblSalesItem.InvID join tblItemMaster on tblItemMaster.ItemID = tblSalesItem.ItemId join tblCategories on tblCategories.CategoryID=tblItemMaster.CategoryID";
                SqlConnection conn = new SqlConnection(constr);
                conn.Open();
                //SqlCommand cmd = new SqlCommand(Sql, conn);
                SqlCommand cmd1 = new SqlCommand(Sql1, conn);
                //cmd.ExecuteNonQuery();
                cmd1.ExecuteNonQuery();
                conn.Close();
                string sQuery = "SELECT column_name FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'vwpurchase'";
                DataTable dtCCntr = Comm.fnGetData(sQuery).Tables[0];

                if (dtCCntr.Rows.Count > 0)
                {
                    chkfield.DataSource = dtCCntr;
                    chkfield.DisplayMember = "column_name";

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void rdbHsncode_Click(object sender, EventArgs e)
        {
            try
            {
                chkfield.Visible = true;
                chkselectAll.Visible = true;
                chkselectAll.Checked = false;
                tlpItem.Visible = false;
                tlpMnf.Visible = false;
                tlpCategory.Visible = false;
                tlpProduct.Visible = false;
                flowLayoutPanel1.Visible = true;
                flowLayoutPanel2.Visible = true;
                //tableLayoutPanel38.Visible = true;

                DropPurchaseView();
                //string Sql = "DROP VIEW vwpurchase";
                string Sql1 = "create view vwpurchase as select top 1 HSNID as HsnCode,tblSalesItem.Qty,tblSalesItem.Rate,tblSalesItem.MRP,ItemDiscount as [Item Discount],NonTaxable as [Non Taxable],Taxable,GrossAmount as [Gross Amount],TaxPer,TaxAmount as [Tax Amount],NetAmount from tblSales join tblSalesItem on tblSales.InvId = tblSalesItem.InvID join tblItemMaster on tblItemMaster.ItemID = tblSalesItem.ItemId";
                SqlConnection conn = new SqlConnection(constr);
                conn.Open();
                //SqlCommand cmd = new SqlCommand(Sql, conn);
                SqlCommand cmd1 = new SqlCommand(Sql1, conn);
                //cmd.ExecuteNonQuery();
                cmd1.ExecuteNonQuery();
                conn.Close();
                string sQuery = "SELECT column_name FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'vwpurchase'";
                DataTable dtCCntr = Comm.fnGetData(sQuery).Tables[0];

                if (dtCCntr.Rows.Count > 0)
                {
                    chkfield.DataSource = dtCCntr;
                    chkfield.DisplayMember = "column_name";

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }

        private void txtTaxMode_TextChanged(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void rdbTax_Click(object sender, EventArgs e)
        {
            try
            {
                chkfield.Visible = true;
                chkselectAll.Visible = true;
                chkselectAll.Checked = false;
                tlpItem.Visible = false;
                tlpMnf.Visible = false;
                tlpCategory.Visible = false;
                tlpProduct.Visible = false;
                flowLayoutPanel1.Visible = true;
                flowLayoutPanel2.Visible = true;
                //tableLayoutPanel38.Visible = true;

                DropPurchaseView();

                //string Sql = "DROP VIEW vwpurchase";
                string Sql1 = "create view vwpurchase as select top 1 Mop,VchType as [Voucher Type],Party,PartyCode as [Party Code],PartyAddress as [Party Address],PartyGSTIN as [Party GSTIN],BillType as [Bill Type],TaxAmt as [Tax Amount],GrossAmt as [Gross Amount],ItemDiscountTotal as [Item Discount Total],NonTaxable as [Non Taxable],Taxable,CGSTTaxPer as [CGST Tax Per],CGSTTaxAmt as [CGST Tax Amt],SGSTTaxPer as [SGST Tax Per],SGSTTaxAmt as [SGST Tax Amt],IGSTTaxPer as[IGST Tax Per],IGSTTaxAmt as [IGST Tax Amt],CashDiscount as [Cash Discount],OtherExpense as [Other Expense],NetAmount as [Net Amount],RoundOff as [Round Off] ,BillAmt as [Bill Amount] from tblSales join tblSalesItem on tblSales.InvId = tblSalesItem.InvID";
                SqlConnection conn = new SqlConnection(constr);
                conn.Open();
                //SqlCommand cmd = new SqlCommand(Sql, conn);
                SqlCommand cmd1 = new SqlCommand(Sql1, conn);
                //cmd.ExecuteNonQuery();
                cmd1.ExecuteNonQuery();
                conn.Close();
                string sQuery = "SELECT column_name FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'vwpurchase'";
                DataTable dtCCntr = Comm.fnGetData(sQuery).Tables[0];

                if (dtCCntr.Rows.Count > 0)
                {
                    chkfield.DataSource = dtCCntr;
                    chkfield.DisplayMember = "column_name";

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void txtAgent_TextChanged(object sender, EventArgs e)
        {

        }

        private void rdoDiscountSales_MouseCaptureChanged(object sender, EventArgs e)
        {

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
                        //sRetResult = dtData.Rows[0][0].ToString();

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
        #endregion    
    }
}
