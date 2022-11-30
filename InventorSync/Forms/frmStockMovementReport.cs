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
    public partial class frmStockMovementReport : Form
    {
        // ======================================================== >>
        // Description:  Purchase Report          
        // Developed By: Anjitha K K           
        // Completed Date & Time: 24/02/2022 6:21 PM
        // Last Edited By:       
        // Last Edited Date & Time:
        // ======================================================== >>

        public frmStockMovementReport()
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
            rdoDaybook.Checked = true;
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
                string sQuery = "Select VchTypeID,VchType from tblVchType where ParentID=41 AND TenantID = '" + Global.gblTenantID + "'";
                new frmCompactCheckedListSearch(GetFromCheckedListVoucher, sQuery, "VchType", txtVoucherTypeList.Location.X + 550, txtVoucherTypeList.Location.Y + 190, 0, 2, txtVoucherTypeList.Text, 0, 0, "", lblVoucherIds.Text, null, "Voucher Type").ShowDialog();

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
                    string Sql = "Select VchTypeID,VchType from tblVchType where ParentID = 41 AND TenantID = '" + Global.gblTenantID + "'";
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
                new frmCompactCheckedListSearch(GetFromCheckedListStaff, sQuery, "Name", txtstafflist.Location.X + 873, txtstafflist.Location.Y + 190, 0, 2, txtstafflist.Text, 0, 0, "", lblstaffIds.Text, null, "Sales Staff").ShowDialog();

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



                string d = "";

                string vchtype = txtVoucherTypeList.Text;

                string from = dtpFD.Text;
                string to = dtpTD.Text;

                if (rdoDaybook.Checked == true)
                {
                    DateTime FD = Convert.ToDateTime(dtpFD.Text);
                    DateTime TD = Convert.ToDateTime(dtpTD.Text);
                    string Fname = "Stock Movement";
                    string sql = "select InvNo as Invoice_No,InvDate as Invoice_Date,VchType as Vaucher_Date,ItemName,tblStock.BatchUnique,tblStockJournalItem.Qty as Actual_Qty from tblStockJournal join tblStockJournalItem on tblStockJournal.InvId=tblStockJournalItem.InvID join tblItemMaster on tblItemMaster.ItemID=tblStockJournalItem.ItemId join tblCategories on tblCategories.CategoryID=tblItemMaster.CategoryID join tblStock on tblstock.BatchUnique=tblStockJournalItem.BatchUnique WHERE InvDate BETWEEN '" + FD.ToString("dd-MMM-yyyy") + "' and '" + TD.ToString("dd-MMM-yyyy") + "' order by  InvNo,InvDate";
                    // Fname, vchtype, cost, mop, from, to, Sql1
                    new frmReportView1(Fname, "", "", "", from, to, this.MdiParent, sql, "").Show();
                }
                if (rdoDaybookDetail.Checked == true)
                {
                    DateTime FD = Convert.ToDateTime(dtpFD.Text);
                    DateTime TD = Convert.ToDateTime(dtpTD.Text);
                    string Fname = "Stock Movement Detail";
                    string sid = lblstaffIds.Text;
                    string sql = "select InvNo as Invoice_No,InvDate as Invoice_Date,tblStockJournal.VchType as Vaucher_Date,ItemCode,ItemName,Category,tblStock.BatchUnique,tblStockJournalItem.Qty as Actual_Qty,StockQty AS SQOH_Befor_Edit ,tblStockJournalItem.Qty-StockQty as Adjust_Qty,Name as Staff_Name from tblStockJournal join tblStockJournalItem on tblStockJournal.InvId=tblStockJournalItem.InvID join tblItemMaster on tblItemMaster.ItemID=tblStockJournalItem.ItemId join tblCategories on tblCategories.CategoryID=tblItemMaster.CategoryID join tblStock on tblstock.BatchUnique=tblStockJournalItem.BatchUnique join tblEmployee on tblEmployee.EmpID = dbo.tblStockJournal.SalesManID WHERE InvDate BETWEEN '" + FD.ToString("dd-MMM-yyyy") + "' and '" + TD.ToString("dd-MMM-yyyy") + "' And  tblEmployee.EmpID in (" + lblstaffIds.Text + ")  order by  InvNo,InvDate";
                    // Fname, vchtype, cost, mop, from, to, Sql1
                    new frmReportView1(Fname, "", "", "", from, to, this.MdiParent, sql, "").Show();
                }
                if (rdoItemWise.Checked == true)
                {
                    if (txtItem.Text != "")
                    {
                        string Fname = "Stock Movement Item Wise";
                        string sql = "select InvNo as Invoice_No,InvDate as Invoice_Date,tblStockJournal.VchType as Vaucher_Date,ItemCode,ItemName,Category,tblStock.BatchUnique,tblStockJournalItem.Qty as Actual_Qty,StockQty AS SQOH_Befor_Edit ,tblStockJournalItem.Qty-StockQty as Adjust_Qty,Name as Staff_Name from tblStockJournal join tblStockJournalItem on tblStockJournal.InvId=tblStockJournalItem.InvID join tblItemMaster on tblItemMaster.ItemID=tblStockJournalItem.ItemId join tblCategories on tblCategories.CategoryID=tblItemMaster.CategoryID join tblStock on tblstock.BatchUnique=tblStockJournalItem.BatchUnique join tblEmployee on tblEmployee.EmpID = dbo.tblStockJournal.SalesManID where ItemName in ('" + txtItem.Text + "') and tblEmployee.EmpID in (" + lblstaffIds.Text + ") order by  InvNo,InvDate";
                        // Fname, vchtype, cost, mop, from, to, Sql1
                        new frmReportView1(Fname, "", "", "", from, to, this.MdiParent, sql, "").Show();
                    }
                    else
                    {
                        MessageBox.Show("Select Item....");
                    }
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

        private void txtItem_Click(object sender, EventArgs e)
        {
            try
            {
                string sQuery = "SELECT (I.ItemCode+I.ItemName+CONVERT(VARCHAR,ISNULL(I.IGSTTaxPer,0))) as AnyWhere,I.ItemCode,I.ItemName,CONVERT(DECIMAL(18,2),I.IGSTTaxPer) as [GST %],I.CategoryID,I.ItemID,I.UNITID FROM tblItemMaster I INNER JOIN tblCategories C ON C.CategoryID = I.CategoryID  AND I.ActiveStatus = 1 ";

                new frmCompactSearch(GetFromItemSearch, sQuery, "AnyWhere|ItemCode|ItemName", txtItem.Location.X + 553, txtItem.Location.Y + 10, 4, 0, txtItem.Text, 4, 0, "ORDER BY ItemName ASC", 0, 0, "Item Name ...", 0, "270,270,0,0,0", true, "frmItemMaster").ShowDialog();

                this.txtItem.TextChanged -= this.txtItem_Click;
                txtItem.Focus();
                this.txtItem.TextChanged += this.txtItem_Click;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void rdoDaybook_Click(object sender, EventArgs e)
        {
            tlpSaleStaff.Visible = false;
            tlpVoucher.Visible = false;
            tlpSupplier.Visible = false;

        }

        private void rdoDaybook_CheckedChanged(object sender, EventArgs e)
        {

            tlpSaleStaff.Visible = false;
            tlpVoucher.Visible = false;
            tlpSupplier.Visible = false;

        }

        private void rdoDaybookDetail_CheckedChanged(object sender, EventArgs e)
        {
            tlpSupplier.Visible = false;
            tlpSaleStaff.Visible = true;
            tlpVoucher.Visible = true;

        }

        private void rdoItemWise_Click(object sender, EventArgs e)
        {
            tlpSaleStaff.Visible = true;
            tlpVoucher.Visible = true;
            tlpSupplier.Visible = true;

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
