using System;
using System.Data;
using System.Windows.Forms;
using InventorSync.InventorBL.Master;
using InventorSync.InventorBL.Helper;
using InventorSync.Info;
using InventorSync.Forms;
using InventorSync.JsonClass;
using System.Data.SqlClient;
using InventorSync.InventorBL.Accounts;
using Microsoft.VisualBasic;
using System.Drawing;

namespace InventorSync
{
    public partial class frmAccountReport : Form
    {
        // ======================================================== >>
        // Description:  Purchase Report          
        // Developed By: Anjitha K K           
        // Completed Date & Time: 24/02/2022 6:21 PM
        // Last Edited By:       
        // Last Edited Date & Time:
        // ======================================================== >>

        public frmAccountReport()
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

                btnMinimize.Image = global::InventorSync.Properties.Resources.minimize_finalised;
                btnClose.Image = global::InventorSync.Properties.Resources.logout_Final;

            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
            }

            txtVoucherTypeList.ReadOnly = true;
         
        }

        #region "VARIABLES --------------------------------------------- >>"
        string constr = Properties.Settings.Default.ConnectionString; //@"Data Source=NAHUM\DIGIPOS;Initial Catalog=DemoDB;Persist Security Info=True;User ID=sa;Password=#infinitY@279;Integrated Security=true";
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
            dtpFD.MinDate = AppSettings.FinYearStart;
            dtpFD.MaxDate = Convert.ToDateTime(AppSettings.FinYearEnd);
            dtpTD.MinDate = AppSettings.FinYearStart;
            dtpTD.MaxDate = Convert.ToDateTime(AppSettings.FinYearEnd);
            rdoLedger.Checked = true;
            rdoDefault.Checked = true;
            dtpFD.Value = AppSettings.FinYearStart;
            tlpVoucher.Visible = false;
            tlpSaleStaff.Visible = false;
            tlpSupplier.Visible = false;
            tableLayoutPanel3.Visible = false;
            tlpSupplier.Visible = true;
            panel1.Visible = false;
            tableLayoutPanel5.Visible = false;
          
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
        private Boolean GetFromCheckedListAccountGroup(string sSelIDs)
        {
            try
            {
                lblAccountId.Text = sSelIDs;
                lblAccountId.Tag = lblAccountId.Text;
                this.txtAccountGroup.TextChanged -= this.txtAccountGroup_Click;
                txtAccountGroup.Text = GetAccountGroupAsperIDs(sSelIDs);
                this.txtAccountGroup.TextChanged += this.txtAccountGroup_Click;

                string[] strCostIDs = sSelIDs.Split(',');

                return true;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
        }
        private Boolean GetFromCheckedListArea(string sSelIDs)
        {
            try
            {
                lblAreaIds.Text = sSelIDs;
                lblAreaIds.Tag = lblAreaIds.Text;
                this.txtArea.TextChanged -= this.txtArea_Click;
                txtArea.Text = GetAreaAsperIDs(sSelIDs);
                this.txtArea.TextChanged += this.txtArea_Click;

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
        private string GetAccountGroupAsperIDs(string sIDs = "")
        {
            try
            {
                UspGetAccountGroupCheckedListInfo GetAccountGroupChk = new UspGetAccountGroupCheckedListInfo();
                string sRetResult = "";
                if (sIDs != "")
                {
                    DataTable dtData = new DataTable();
                    GetAccountGroupChk.IDs = sIDs;
                    GetAccountGroupChk.TenantId = Global.gblTenantID;
                    dtData = clsccntr.GetAccountGroupCheckedList(GetAccountGroupChk);
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
        private string GetAreaAsperIDs(string sIDs = "")
        {
            try
            {
                UspGetAreaCheckedListInfo GetCostChk = new UspGetAreaCheckedListInfo();
                string sRetResult = "";
                if (sIDs != "")
                {
                    DataTable dtData = new DataTable();
                    GetCostChk.IDs = sIDs;
                    GetCostChk.TenantId = Global.gblTenantID;
                    dtData = clsccntr.GetAreaCheckedList(GetCostChk);
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
      
        private void txtVoucherTypeList_Click(object sender, EventArgs e)
        {
            txtVoucherTypeList.ReadOnly = true;
            try
            {
                if (string.IsNullOrEmpty(txtVoucherTypeList.Text))
                {
                    lblVoucherIds.Text = Convert.ToString(txtVoucherTypeList.Tag);
                    lblVoucherIds.Text = "";
                }
                if (this.ActiveControl.Name != "txtVoucherTypeList")
                    return;
                string sQuery = "Select VchTypeID,VchType from tblVchType where ParentID in (7,8,9,10) AND TenantID = '" + Global.gblTenantID + "'";
                new frmCompactCheckedListSearch(GetFromCheckedListVoucher, sQuery, "VchType", txtVoucherTypeList.Location.X + 791, txtVoucherTypeList.Location.Y + 200, 0, 2, txtVoucherTypeList.Text, 0, 0, "", lblVoucherIds.Text, null, "Voucher Type").ShowDialog();

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
                    string Sql = "Select VchTypeID,VchType from tblVchType where ParentID in(7,8,9,10) AND TenantID = '" + Global.gblTenantID + "'";
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
                new frmCompactCheckedListSearch(GetFromCheckedListStaff, sQuery, "Name", txtstafflist.Location.X + 473, txtstafflist.Location.Y + 260, 0, 2, txtstafflist.Text, 0, 0, "", lblstaffIds.Text, null, "Sales Staff").ShowDialog();

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
                    this.txtLedger.TextChanged -= this.txtLedger_Click;
                    txtLedger.Text = dtSupp.Rows[0]["LName"].ToString();
                    this.txtLedger.TextChanged += this.txtLedger_Click;
                    lblLID.Text = dtSupp.Rows[0]["LID"].ToString();
                    txtLedger.Tag = dtSupp.Rows[0]["LedgerCode"].ToString();
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
       
 
       
        private Boolean GetFromCustomerSearch(string LstIDandText)
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
                            GetLedinfo.GroupName = "";
                            dtManf = clsLedg.GetLedger(GetLedinfo);
                            if (dtManf.Rows.Count > 0)
                            {
                              
                                this.txtLedger.TextChanged -= this.txtLedger_Click;
                                txtLedger.Text = dtManf.Rows[0]["LedgerName"].ToString();
                                this.txtLedger.TextChanged += this.txtLedger_Click;
                                lblLID.Text = dtManf.Rows[0]["LID"].ToString();
                                txtLedger.Tag = dtManf.Rows[0]["LedgerCode"].ToString();
                                return true;

                            }
                            return true;
                        }
                        else
                        {

                            this.txtLedger.TextChanged -= this.txtLedger_Click;
                            txtLedger.Text = sCompSearchData[1].ToString();
                            this.txtLedger.TextChanged += this.txtLedger_Click;
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
                
                if (txtVoucherTypeList.Text == "")
                {
                    chkVoucher.Checked = true;
                }
               
                if (txtstafflist.Text == "")
                {
                    chkstaff.Checked = true;
                }

                if (txtAccountGroup.Text=="")
                {
                    checkBox2.Checked = true;
                }

              string vchtype = txtVoucherTypeList.Text;
                string Ledger = txtLedger.Text;
                string from = dtpFD.Text;
                string to = dtpTD.Text;
              
                if(rdoLedger.Checked==true)
                {
                    string Fname = "Ledger";
                    if (txtLedger.Text!="")
                    {
                        DateTime FD = Convert.ToDateTime(dtpFD.Text);
                        DateTime TD = Convert.ToDateTime(dtpTD.Text);
                        new frmReportView1(Fname, vchtype, "", Ledger, from, to, this.MdiParent, "", "").Show();
                    }
                    else
                    {
                        MessageBox.Show(" Select Ledger...");

                    }
                }
               
                if (rdoAccountGroupWise.Checked == true)
                {

                    string amt = textBox1.Text;
                    string ids = lblAccountId.Text;
                    string Fname = "Account Group Wise";
                    DateTime FD = Convert.ToDateTime(dtpFD.Text);
                    DateTime TD = Convert.ToDateTime(dtpTD.Text);
                    new frmReportView1(Fname, vchtype, "", Ledger, from, to, this.MdiParent, "", amt,ids,"").Show();


                }
                  if (rdoSupplierOutstanding.Checked == true)
                {
                    string Fname = "Supplier Outstanding";
                    string amt = "";
                    string area = txtArea.Text;
                    if (textBox1.Text == "")
                    {
                        amt = "00";
                    }
                    else
                    {
                        amt = textBox1.Text;
                    }
                    new frmReportView1(Fname, "", "", "","","", this.MdiParent,"",amt,"",area).Show();
                }
                if (rdoCustomerOutstanding.Checked == true)
                {
                    string Fname = "Customer Outstanding";
                    string amt = "";
                    string area = txtArea.Text;
                    if (textBox1.Text == "")
                    {
                         amt = "00";
                    }
                    else
                    {
                        amt = textBox1.Text;
                    }
                    new frmReportView1(Fname, "", "", "", "", "" ,this.MdiParent, "",amt,"",area).Show(); 
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void rdoSalesDaybook_Click(object sender, EventArgs e)
        {
            try
            {
        
                flowLayoutPanel1.Visible = true;
 
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            
        }

        private void tlpSearch_Paint(object sender, PaintEventArgs e)
        {

        }

       
        

        private void rdoPurchaseSummary_Click_1(object sender, EventArgs e)
        {
            try
            {
                
                flowLayoutPanel1.Visible = true;
              

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
                flowLayoutPanel1.Visible = true;
               
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
                flowLayoutPanel1.Visible = true;
              
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void txtLedger_Click(object sender, EventArgs e)
        {
           
            string strCondition = "";
            if (clsVchType.ParentID == 7)
                strCondition = " and  accountgroupid in (16,17) ";
            else if (clsVchType.ParentID == 8)
                strCondition = " and  accountgroupid in (16,17) ";
            else if (clsVchType.ParentID == 9)
                strCondition = " and  accountgroupid in (16,17) ";

            string sQuery = "SELECT LedgerCode + LedgerName + MobileNo + Email + TaxNo AS anywhere, LedgerCode, LedgerName, MobileNo, CurBal, Email, TaxNo, LID " +
                    " FROM     vwLedger   Where isnull(ActiveStatus, 1) = 1 " + strCondition;
            frmDetailedSearch2 frmN = new frmDetailedSearch2(GetFromLedgerSearch, sQuery, "Anywhere|LedgerCode|LedgerName", this.Left + 155, this.Top + 20, 7, 0,"", 7, 0, "ORDER BY LedgerCode ASC", 0, 0, "Ledger Search...", 0, "250,250,150,150,150,150,0", true, "frmLedger", 20);
            frmN.MdiParent = this.MdiParent;
            frmN.Show();
            //string sQuery = "SELECT LedgerCode + LedgerName + MobileNo + Email + TaxNo AS anywhere, LedgerCode, LedgerName, MobileNo, CurBal, Email, TaxNo, LID " +
            //              " FROM     vwLedger   Where isnull(ActiveStatus, 1) = 1 ";
            //frmDetailedSearch2 frmN = new frmDetailedSearch2(GetFromLedgerSearch, sQuery, "Anywhere|LedgerCode|LedgerName", this.Left + 155, this.Top + 20, 6, 0, "", 5, 0, "ORDER BY LedgerCode ASC", 0, 0, "Ledger Search...", 0, "250,250,150,150,150,150,0", true, "frmLedger", 20);
            //frmN.MdiParent = this.MdiParent;
            //frmN.Show();

            //try
            //{

            //        toolTipArea.SetToolTip(txtLedger, "Specify the unique Area Name");
            //        txtLedger.SelectAll();
            //        string sQuery = "SELECT LedgerName+LedgerCode+Phone+MobileNo+Address as AnyWhere,LedgerCode as [Supplier Code],LedgerName as [Supplier Name] ,MobileNo ,Address,LID,Email  FROM tblLedger L join tblVoucher on L.LID =tblVoucher.LedgerID";
            //        if (clsVchType.CustomerSupplierAccGroupList != "")
            //            sQuery = sQuery + " INNER JOIN tblAccountGroup A ON A.AccountGroupID = 10 join tblVoucher on L.LID =tblVoucher.LedgerID";
            //        sQuery = sQuery + " WHERE  L.TenantID=" + Global.gblTenantID + " group by LedgerCode,LedgerName,phone,MobileNo,Address,LID,Email";
            //        new frmCompactSearch(GetFromCustomerSearch, sQuery, "AnyWhere|LedgerCode|LedgerName|MobileNo|Address", txtLedger.Location.X + 473, txtLedger.Location.Y + 5, 4, 0, txtLedger.Text, 4, 0, "ORDER BY L.LedgerName ASC", 0, 0, "Ledger", 0, "100,200,100,200", true, "").ShowDialog();

            //        this.txtLedger.TextChanged -= this.txtLedger_Click;
            //        txtLedger.Focus();
            //        this.txtLedger.TextChanged += this.txtLedger_Click;

            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            //}
        }
        private void txtLedger_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.ActiveControl.Name == txtLedger.Name)
                {
                    if (txtLedger.Text != "")
                    {
                        if (ConvertI32(clsVchType.blnShowSearchWindowByDefault) == 1)
                        {
                            //string sQuery = "SELECT LName+LAliasName+MobileNo+Address as AnyWhere,LALiasname as [Supplier Code],lname as [Supplier Name] ,MobileNo ,Address,LID,Email  FROM tblLedger L";
                            //if (clsVchType.CustomerSupplierAccGroupList != "")
                            //    sQuery = sQuery + " INNER JOIN tblAccountGroup A ON A.AccountGroupID = L.AccountGroupID AND A.AccountGroupID IN (" + clsVchType.CustomerSupplierAccGroupList + ")";
                            //sQuery = sQuery + " WHERE UPPER(L.groupName)='SUPPLIER' AND L.TenantID=" + Global.gblTenantID + "";
                            //new frmCompactSearch(GetFromSupplierSearch, sQuery, "AnyWhere|LAliasName|LName|MobileNo|Address", txtLedger.Location.X + 800, txtLedger.Location.Y - 20, 4, 0, txtLedger.Text, 4, 0, "ORDER BY L.LAliasName ASC", 0, 0, "Supplier Search ...", 0, "100,200,100,200,0", true, "frmSupplier").ShowDialog();

                            //string sQuery = "SELECT LedgerCode + LedgerName + MobileNo + Email + TaxNo AS anywhere, LedgerCode, LedgerName, MobileNo, CurBal, Email, TaxNo, LID " +
                            //        " FROM     vwLedger   Where isnull(ActiveStatus, 1) = 1 ";
                            //frmDetailedSearch2 frmN = new frmDetailedSearch2(GetFromLedgerSearch, sQuery, "Anywhere|LedgerCode|LedgerName", this.Left + 155, this.Top + 20, 6, 0, "", 5, 0, "ORDER BY LedgerCode ASC", 0, 0, "Ledger Search...", 0, "250,250,150,150,150,150,0", true, "frmLedger", 20);
                            //frmN.MdiParent = this.MdiParent;
                            //frmN.Show();

                            //dgvItems.CurrentCell = dgvItems.Rows[0].Cells[GetEnum(gridColIndexes.CLedgerCode)];
                            //dgvItems.Focus();
                        }
                    }
                    else
                        lblLID.Text = "0";
                }
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
        }
        private Boolean GetFromLedgerSearch(string sReturn)
        {
            try
            {
                DataTable dtSupp = new DataTable();

                string[] sCompSearchData = sReturn.Split('|');
              
                if (sCompSearchData[0].ToString() == "NOTEXIST")
                {
                    return false;
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
                            dtSupp = clsLedg.GetLedger(GetLedinfo);

                            if (dtSupp.Rows.Count > 0)
                            {
                                this.txtLedger.TextChanged -= this.txtLedger_TextChanged;
                                txtLedger.Text = dtSupp.Rows[0].Field<string>("LedgerCode"); //sCompSearchData[1].ToString();
                                this.txtLedger.TextChanged += this.txtLedger_TextChanged;
                                //lblLID.Text = dtSupp.Rows[0].Field<int>("LID").ToString();

                                return true;
                            }

                            else
                                return false;
                        }
                        else
                            return false;
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
        private void rdoDiscountSales_Click(object sender, EventArgs e)
        {
            try
            {
                tlpVoucher.Visible = false;
                tlpSaleStaff.Visible = false;
                tlpSupplier.Visible = false;
                tableLayoutPanel3.Visible = true;
                tlpSupplier.Visible = false;
                panel1.Visible = true;
                tableLayoutPanel5.Visible = false;


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }

        private void rdoLedger_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                tlpVoucher.Visible = false;
                tlpSaleStaff.Visible = false;
                tlpSupplier.Visible = false;
                tableLayoutPanel3.Visible = false;
                tlpSupplier.Visible = true; 
                panel1.Visible = false;
                tableLayoutPanel5.Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void txtArea_Click(object sender, EventArgs e)
        {
            try
            {

                if (string.IsNullOrEmpty(txtArea.Text))
                {
                    lblAreaIds.Text = Convert.ToString(txtArea.Tag);
                    lblAreaIds.Text = "";
                }
                if (this.ActiveControl.Name != "txtArea")
                    return;
                string sQuery = "SELECT AreaID,Area FROM tblArea WHERE TenantID = " + Global.gblTenantID + "";
                new frmCompactCheckedListSearch(GetFromCheckedListArea, sQuery, "Area", txtArea.Location.X + 570, txtArea.Location.Y + 200, 0, 2, txtArea.Text, 0, 0, "", lblAreaIds.Text, null, "Area").ShowDialog();

            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void rdoAccountGroupWise_Click(object sender, EventArgs e)
        {
            tlpSaleStaff.Visible = false;
            tlpSupplier.Visible = false;
            tableLayoutPanel3.Visible = false;
            tlpSupplier.Visible = false;
            panel1.Visible = false;
            tableLayoutPanel5.Visible = true;
        }

        private void txtAccountGroup_Click(object sender, EventArgs e)
        {
            
            try
            {

                if (string.IsNullOrEmpty(txtAccountGroup.Text))
                {
                    lblAccountId.Text = Convert.ToString(txtAccountGroup.Tag);
                    lblAccountId.Text = "";
                }
                if (this.ActiveControl.Name != "txtAccountGroup")
                    return;
                string sQuery = "Select AccountGroupID, AccountGroup From tblAccountGroup WHERE TenantID = " + Global.gblTenantID + "";
                new frmCompactCheckedListSearch(GetFromCheckedListAccountGroup, sQuery, "AccountGroup", txtAccountGroup.Location.X + 560, txtAccountGroup.Location.Y + 190, 0, 2, txtAccountGroup.Text, 0, 0, "", lblAccountId.Text, null, "Account Group").ShowDialog();

            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkBox2.Checked == true)
                {
                    string Sql = "Select AccountGroupID, AccountGroup From tblAccountGroup WHERE TenantID = " + Global.gblTenantID + "";
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
                        sStrNames = sStrNames + dt.Rows[i]["AccountGroup"].ToString() + ",";
                        sStrIds = sStrIds + dt.Rows[i]["AccountGroupID"].ToString() + ",";

                    }
                    txtAccountGroup.Text = sStrNames.Remove(sStrNames.Length - 1, 1);
                    lblAccountId.Text = sStrIds.Remove(sStrIds.Length - 1, 1);
                }
                else
                {
                    txtAccountGroup.Text = "";
                    checkBox2.Checked = false;

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void rdoCustomerOutstanding_Click(object sender, EventArgs e)
        {
            try
            {
                tlpVoucher.Visible = false;
                tlpSaleStaff.Visible = false;
                tlpSupplier.Visible = false;
                tableLayoutPanel3.Visible = true;
                tlpSupplier.Visible = false;
                panel1.Visible = true;
                tableLayoutPanel5.Visible = false;


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
              
                if (checkBox1.Checked == true)
                {
                    string Sql = "SELECT AreaID,Area FROM tblArea WHERE TenantID = " + Global.gblTenantID + "";
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
                        sStrNames = sStrNames + dt.Rows[i]["Area"].ToString() + ",";
                        sStrIds = sStrIds + dt.Rows[i]["AreaID"].ToString() + ",";

                    }
                    txtArea.Text = sStrNames.Remove(sStrNames.Length - 1, 1);
                    lblAreaIds.Text = sStrIds.Remove(sStrIds.Length - 1, 1);
                }
                else
                {
                    txtArea.Text = "";
                    checkBox1.Checked = false;

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
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
        #endregion    
    }
}
