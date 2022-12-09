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
    public partial class frmRepackingReport : Form
    {
        // ======================================================== >>
        // Description:  Purchase Report          
        // Developed By: Anjitha K K           
        // Completed Date & Time: 24/02/2022 6:21 PM
        // Last Edited By:       
        // Last Edited Date & Time:
        // ======================================================== >>

        public frmRepackingReport()
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
        string constr = @"Data Source=DESKTOP-THO19HQ\DIGIPOS;Initial Catalog=DigiposDemo;User ID=sa;Password=#infinitY@279";//DigiposZen.Properties.Settings.Default.ConnectionString; //
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
            rdoRepackingDaybook.Checked = true;
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
       
        private void lblHeading_MouseUp(object sender, MouseEventArgs e)
        {
            dragging = false;
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
                string Sql = "DROP VIEW vwstock";
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
                new frmCompactCheckedListSearch(GetFromCheckedListCost, sQuery, "CCName", txtCostCenterList.Location.X + 500, txtCostCenterList.Location.Y + 130, 0, 2, txtCostCenterList.Text, 0, 0, "", lblCostCenterIds.Text, null, "Cost Center").ShowDialog();

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
                string sQuery = "Select VchTypeID,VchType from tblVchType where ParentID = 20 AND TenantID = '" + Global.gblTenantID + "'";
                new frmCompactCheckedListSearch(GetFromCheckedListVoucher, sQuery, "VchType", txtVoucherTypeList.Location.X + 780, txtVoucherTypeList.Location.Y + 130, 0, 2, txtVoucherTypeList.Text, 0, 0, "", lblVoucherIds.Text, null, "Voucher Type").ShowDialog();
                 
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
                    string Sql = "Select VchTypeID,VchType from tblVchType where ParentID = 20 AND TenantID = '" + Global.gblTenantID + "'";
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


        private void rdoPurchaseDaybook_Click_1(object sender, EventArgs e)
        {
            try
            {
                tlpItem.Visible = false;
                     

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }

       

        private void button1_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (txtCostCenterList.Text == "")
                {
                    chkCostCenter.Checked = true;
                }
                if (txtVoucherTypeList.Text == "")
                {
                    chkVoucher.Checked = true;
                }
                if (txtstafflist.Text == "")
                {
                    chkstaff.Checked = true;
                }

                string cost = txtCostCenterList.Text;
                string vchtype = txtVoucherTypeList.Text;
                string from = dtpFD.Text;
                string to = dtpTD.Text;
               
                if (rdoRepackingDaybook.Checked == true)
                {
                    DropPurchaseView();
                    string Fname = "Repacking Daybook";

                    if (rdoDefault.Checked == true)
                    {

                        DateTime FD = Convert.ToDateTime(dtpFD.Text);
                        DateTime TD = Convert.ToDateTime(dtpTD.Text);

                        

                        string Sql1 = "create view vwstock as SELECT InvNo,InvDate, (SELECT Sum(ri.qty) FROM tblrepackingitem as ri JOIN tblitemmaster ON tblitemmaster.itemid = ri.itemid JOIN tblcategories ON tblitemmaster.categoryid = tblcategories.categoryid WHERE blnqtyin = 1 and r.invid = ri.invid) AS QtyIn,(SELECT Sum(ri.INetAmount) FROM tblrepackingitem as ri JOIN tblitemmaster ON tblitemmaster.itemid = ri.itemid JOIN tblcategories ON tblitemmaster.categoryid = tblcategories.categoryid WHERE  blnqtyin = 1 and r.invid = ri.invid) as AmountIn,(SELECT Sum(ri.qty) FROM tblrepackingitem as ri JOIN tblitemmaster ON tblitemmaster.itemid = ri.itemid JOIN tblcategories ON tblitemmaster.categoryid = tblcategories.categoryid WHERE blnqtyin = 0 and r.invid = ri.invid) AS QtyOut,(SELECT Sum(ri.INetAmount) FROM tblrepackingitem as ri JOIN tblitemmaster ON tblitemmaster.itemid = ri.itemid JOIN tblcategories ON tblitemmaster.categoryid = tblcategories.categoryid WHERE  blnqtyin = 0 and r.invid = ri.invid) as AmountOut FROM tblrepacking as r JOIN tblemployee ON tblemployee.empid = r.salesmanid  where invdate BETWEEN '" + FD.ToString("dd-MMM-yyyy") + "' and '" + TD.ToString("dd-MMM-yyyy") + "' ";

                        SqlConnection conn1 = new SqlConnection(constr);
                        conn1.Open();
                        SqlCommand cmd1 = new SqlCommand(Sql1, conn1);
                        cmd1.ExecuteNonQuery();

                        conn1.Close();

                        new frmReportView1(Fname, vchtype, cost, "", from, to, this.MdiParent, "").Show();
                    }
                    else if (rdoexcel.Checked == true)
                    {
                        Com.MessageboxToasted("Report", "Report Show in Excel....");

                    }
                }
               


                if (rdbRawMaterial.Checked == true)
                {


                    DropPurchaseView();

                    string Fname = "Raw Material";

                    if (rdoDefault.Checked == true)
                    {

                        DateTime FD = Convert.ToDateTime(dtpFD.Text);
                        DateTime TD = Convert.ToDateTime(dtpTD.Text);
                        string sqlitem = "";
                      
                        if (txtItem.Text != "")
                        {
                            sqlitem = "tblItemMaster.ItemCode='" + txtItem.Text + "' AND ";
                        }

                        string Sql1 = "create view vwstock AS select ItemName,tblRepackingItem.Qty as QtyOut,tblRepackingItem.CRate,tblRepackingItem.INetAmount from tblRepacking join tblRepackingItem on tblRepacking.InvId=tblRepackingItem.InvID join tblItemMaster on tblItemMaster.ItemID=tblRepackingItem.ItemId join tblCategories on tblItemMaster.CategoryID=tblCategories.CategoryID join tblEmployee on tblEmployee.EmpID=tblRepacking.SalesManID where blnQtyIN=0 and invdate BETWEEN '" + FD.ToString("dd-MMM-yyyy") + "' and '" + TD.ToString("dd-MMM-yyyy") + "' and SalesManID in ("+lblstaffIds.Text+ ") and VchTypeID in ("+lblVoucherIds.Text+ ") and CCID in ("+lblCostCenterIds.Text+")";



                        SqlConnection conn1 = new SqlConnection(constr);
                        conn1.Open();
                        SqlCommand cmd1 = new SqlCommand(Sql1, conn1);
                        cmd1.ExecuteNonQuery();

                        conn1.Close();

                        new frmReportView1(Fname, vchtype, cost, "", from, to, this.MdiParent).Show();
                    }
                    else if (rdoexcel.Checked == true)
                    {
                        Com.MessageboxToasted("Report", "Report Show in Excel....");

                    }
                }
                if (rdbFinishedGoods.Checked == true)
                {


                    DropPurchaseView();

                    string Fname = "Finished Goods";

                    if (rdoDefault.Checked == true)
                    {

                        DateTime FD = Convert.ToDateTime(dtpFD.Text);
                        DateTime TD = Convert.ToDateTime(dtpTD.Text);
                        string sqlitem = "";

                        if (txtItem.Text != "")
                        {
                            sqlitem = "tblItemMaster.ItemCode='" + txtItem.Text + "' AND ";
                        }

                        string Sql1 = "create view vwstock AS select ItemName,tblRepackingItem.Qty as QtyIn,tblRepackingItem.CRate,tblRepackingItem.INetAmount from tblRepacking join tblRepackingItem on tblRepacking.InvId=tblRepackingItem.InvID join tblItemMaster on tblItemMaster.ItemID=tblRepackingItem.ItemId join tblCategories on tblItemMaster.CategoryID=tblCategories.CategoryID join tblEmployee on tblEmployee.EmpID=tblRepacking.SalesManID where blnQtyIN=1 and invdate BETWEEN '" + FD.ToString("dd-MMM-yyyy") + "' and '" + TD.ToString("dd-MMM-yyyy") + "' and SalesManID in (" + lblstaffIds.Text + ") and VchTypeID in (" + lblVoucherIds.Text + ") and CCID in (" + lblCostCenterIds.Text + ")";



                        SqlConnection conn1 = new SqlConnection(constr);
                        conn1.Open();
                        SqlCommand cmd1 = new SqlCommand(Sql1, conn1);
                        cmd1.ExecuteNonQuery();

                        conn1.Close();

                        new frmReportView1(Fname, vchtype, cost, "", from, to, this.MdiParent).Show();
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
