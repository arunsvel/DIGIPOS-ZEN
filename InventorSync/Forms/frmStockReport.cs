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

namespace InventorSync.Forms
{
    public partial class frmStockReport : Form
    {
        public frmStockReport()
        {
            InitializeComponent();

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

        }
        #region "VARIABLES --------------------------------------------- >>"
        string constr = DigiposZen.Properties.Settings.Default.ConnectionString; //@"Data Source=NAHUM\DIGIPOS;Initial Catalog=DemoDB;Persist Security Info=True;User ID=sa;Password=#infinitY@279;Integrated Security=true";
        //string constr = @"Data Source=GAMERADICTION\DIGIPOS;Initial Catalog=DemoDB;Persist Security Info=True;User ID=sa;Password=#infinitY@279;Integrated Security=true";
        clsCostCentre clsccntr = new clsCostCentre();
        clsJsonVoucherType clsVchType = new clsJsonVoucherType();
        UspGetLedgerInfo GetLedinfo = new UspGetLedgerInfo();
        clsLedger clsLedg = new clsLedger();
        Common Comm = new Common();
        UspGetItemMasterInfo GetItem = new UspGetItemMasterInfo();
        clsItemMaster clsitem = new clsItemMaster();
        #endregion

        #region "EVENTS ------------------------------------------------ >>"
        private void frmStockReport_Load(object sender, EventArgs e)
        {
            dtpFD.MinDate = AppSettings.FinYearStart;
            dtpFD.MaxDate = Convert.ToDateTime(AppSettings.FinYearEnd);
            rdoStockConsolidate.Checked = true;
        }
        private void rdoStockValue_Click(object sender, EventArgs e)
        {
            try
            {


                checkBox1.Visible = true;


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                new frmCompactCheckedListSearch(GetFromCheckedListPtype, sQuery, "ProductType", txtProductType.Location.X + 453, txtProductType.Location.Y + 280, 0, 2, txtProductType.Text, 0, 0, "", lblPType.Text, null, "Product Type").ShowDialog();
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
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
                new frmCompactCheckedListSearch(GetFromCheckedListMnf, sQuery, "Category", txtMnf.Location.X + 453, txtMnf.Location.Y + 280, 0, 2, txtMnf.Text, 0, 0, "", lblMnfIds.Text, null, "Manufacturer").ShowDialog();
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
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

        private void txtSupplier_Click(object sender, EventArgs e)
        {

            try
            {
                 toolTipArea.SetToolTip(txtSupplier, "Specify the unique Area Name");

                string sQuery = "SELECT LName+LAliasName+Phone+MobileNo+Address as AnyWhere,LedgerCode as [Supplier Code],LedgerName as [Supplier Name] ,MobileNo ,Address,LID,Email  FROM tblLedger L";
                if (clsVchType.CustomerSupplierAccGroupList != "")
                    sQuery = sQuery + " INNER JOIN tblAccountGroup A ON A.AccountGroupID = 11";
                sQuery = sQuery + " WHERE UPPER(L.groupName)='SUPPLIER' AND L.TenantID=" + Global.gblTenantID + "";
                new frmCompactSearch(GetFromSupplierSearch, sQuery, "AnyWhere|LedgerCode|LedgerName|MobileNo|Address", txtSupplier.Location.X + 450, txtSupplier.Location.Y + 10, 4, 0, txtSupplier.Text, 4, 0, "ORDER BY L.LedgerName ASC", 0, 0, "Supplier Search ...", 0, "100,200,100,200", true, "frmSupplier").ShowDialog();

                this.txtSupplier.TextChanged -= this.txtSupplier_Click;
                txtSupplier.Focus();
                this.txtSupplier.TextChanged += this.txtSupplier_Click;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
       
        private void btnClose_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(txtItem.Text))
            {
                DialogResult dlgResult = MessageBox.Show("Your are in the middle of an Entry. Do you want to exit?", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                if (dlgResult.Equals(DialogResult.Yes))
                    this.Close();
            }
            else
            {
                this.Close();
            }
        }

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;

        }
        private void rdoStockConsolidate_CheckedChanged(object sender, EventArgs e)
        {
            tlpSupplier.Visible = false;
            tlpMnf.Visible = false;
            tlpItem.Visible = true;
            tlpMnf.Visible = false;
            tlpProduct.Visible = false;
            tlpCategory.Visible = false;
            checkBox1.Visible = false;
        }

        private void rdoStockValue_CheckedChanged(object sender, EventArgs e)
        {
            tlpSupplier.Visible = false;
            tlpMnf.Visible = true;
            tlpItem.Visible = true;
            tlpMnf.Visible = true;
            tlpProduct.Visible = true;
            tlpCategory.Visible = true;
        }

        private void rdoRol_CheckedChanged(object sender, EventArgs e)
        {
            tlpSupplier.Visible = false;
            tlpMnf.Visible = true;
            tlpItem.Visible = true;
            tlpMnf.Visible = true;
            tlpProduct.Visible = true;
            tlpCategory.Visible = true;
            checkBox1.Visible = false;
        }

        private void rdoMoq_CheckedChanged(object sender, EventArgs e)
        {
            tlpSupplier.Visible = false;
            tlpMnf.Visible = true;
            tlpItem.Visible = true;
            tlpMnf.Visible = true;
            tlpProduct.Visible = true;
            tlpCategory.Visible = true;
            checkBox1.Visible = false;
        }

        private void rdoZerostock_CheckedChanged(object sender, EventArgs e)
        {
            tlpSupplier.Visible = false;
            tlpMnf.Visible = true;
            tlpItem.Visible = true;
            tlpMnf.Visible = true;
            tlpProduct.Visible = true;
            tlpCategory.Visible = true;
            checkBox1.Checked = false;
        }

        private void rdoFastmoving_CheckedChanged(object sender, EventArgs e)
        {
            tlpSupplier.Visible = false;
            tlpMnf.Visible = true;
            tlpItem.Visible = true;
            tlpMnf.Visible = true;
            tlpProduct.Visible = true;
            tlpCategory.Visible = true;
            checkBox1.Visible = false;
        }

        private void rdoSlowmoving_CheckedChanged(object sender, EventArgs e)
        {
            tlpSupplier.Visible = false;
            tlpMnf.Visible = true;
            tlpItem.Visible = true;
            tlpMnf.Visible = true;
            tlpProduct.Visible = true;
            tlpCategory.Visible = true;
            checkBox1.Visible = false;
        }

        private void rdoNonMoving_CheckedChanged(object sender, EventArgs e)
        {
            tlpSupplier.Visible = false;
            tlpMnf.Visible = true;
            tlpItem.Visible = true;
            tlpMnf.Visible = true;
            tlpProduct.Visible = true;
            tlpCategory.Visible = true;
            checkBox1.Visible = false;
        }

        private void rdoSupplierstock_CheckedChanged(object sender, EventArgs e)
        {
            tlpSupplier.Visible = true;
            tlpMnf.Visible = false;
            tlpItem.Visible = false;
            tlpMnf.Visible = false;
            tlpProduct.Visible = false;
            tlpCategory.Visible = false;
            checkBox1.Visible = false;
        }

        private void rdoSup_CheckedChanged(object sender, EventArgs e)
        {
            tlpSupplier.Visible = true;
            tlpMnf.Visible = false;
            tlpItem.Visible = false;
            tlpMnf.Visible = false;
            tlpProduct.Visible = false;
            tlpCategory.Visible = false;
            checkBox1.Visible = false;
        }
        #endregion

        #region "METHODS ----------------------------------------------- >>"
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
                                //this.txtSupplier.TextChanged -= this.txtSupplier_Click;
                                //txtSupplier.Text = dtManf.Rows[0]["LName"].ToString();
                                //this.txtSupplier.TextChanged += this.txtSupplier_Click;
                                //txtSupplier.Tag = dtManf.Rows[0]["LID"].ToString();

                                this.txtSupplier.TextChanged -= this.txtSupplier_Click;
                                txtSupplier.Text = dtManf.Rows[0]["LedgerName"].ToString();
                                this.txtSupplier.TextChanged += this.txtSupplier_Click;
                                lblLID.Text = dtManf.Rows[0]["LID"].ToString();
                                txtSupplier.Tag = dtManf.Rows[0]["LedgerCode"].ToString();
                                return true;
                            }
                            return true;
                        }
                        else
                        {
                            this.txtSupplier.TextChanged -= this.txtSupplier_Click;
                            txtSupplier.Text = sCompSearchData[1].ToString();
                            this.txtSupplier.TextChanged += this.txtSupplier_Click;
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
       


        private void txtItem_Click(object sender, EventArgs e)
        {
            try
            {
                string sQuery = "SELECT (I.ItemCode+I.ItemName+CONVERT(VARCHAR,ISNULL(I.IGSTTaxPer,0))) as AnyWhere,I.ItemCode,I.ItemName,CONVERT(DECIMAL(18,2),I.IGSTTaxPer) as [GST %],I.CategoryID,I.ItemID,I.UNITID FROM tblItemMaster I INNER JOIN tblCategories C ON C.CategoryID = I.CategoryID  AND I.ActiveStatus = 1 ";

                new frmCompactSearch(GetFromItemSearch, sQuery, "AnyWhere|ItemCode|ItemName", txtItem.Location.X + 455, txtItem.Location.Y + 50, 4, 0, txtItem.Text, 4, 0, "ORDER BY ItemName ASC", 0, 0, "Item Name ...", 0, "270,270,0,0,0", true, "frmItemMaster").ShowDialog();

                this.txtItem.TextChanged -= this.txtItem_Click;
                txtItem.Focus();
                this.txtItem.TextChanged += this.txtItem_Click;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
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

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
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
                string sqlitem = "";

                if (txtItem.Text != "")
                {
                    sqlitem = " tblItemMaster.ItemName = '" + txtItem.Text + "' And";
                }
                DropPurchaseView();
                string Sql2 = "create view vwstock as select CCName as[Cost Center],ItemCode as[Item Code],ItemName as [Item Name],tblStockHistory.BatchUnique,UnitName,tblStock.MRP,cast(sum(QtyIn-QtyOut)as numeric(36, 2)) as QOH from tblStockHistory join tblItemMaster on tblStockHistory.ItemID=tblItemMaster.ItemID join tblCostCentre on tblStockHistory.CCID=tblCostCentre.CCID join tblUnit on tblItemMaster.UNITID=tblUnit.UnitID join tblstock on tblStockHistory.BatchUnique = tblStock.BatchUnique where " + sqlitem + " ID !=0 And tblItemMaster.ProductTypeID IN (" + lblPType.Text + ")  group by CCName,ItemCode,ItemName,UnitName,tblStockHistory.BatchUnique,tblStock.MRP";
                SqlConnection conn2 = new SqlConnection(constr);
                conn2.Open();
                SqlCommand cmd2 = new SqlCommand(Sql2, conn2);
                cmd2.ExecuteNonQuery();

                conn2.Close();
                string supplier = txtSupplier.Text;
                string product = txtProductType.Text;
                string mnf = txtMnf.Text;

                if (rdoStockConsolidate.Checked == true)
                {
                    if (txtItem.Text == "")
                    {
                        MessageBox.Show("Select Item..........");

                    }
                    else
                    {
                        DropPurchaseView();
                        DateTime FD = Convert.ToDateTime(dtpFD.Text);
                        string Fname = "Stock Consolidate";
                        string Sql1 = "create view vwstock as  select VchDate,VchType,CCName as[Cost Center],ItemCode as[Item Code],ItemName as [Item Name],tblStock.BatchUnique,cast(tblStock.MRP as numeric(36, 2)) as MRP,QtyIn,QtyOut from tblStockHistory join tblItemMaster on tblStockHistory.ItemID = tblItemMaster.ItemID join tblCostCentre on tblStockHistory.CCID = tblCostCentre.CCID join tblstock on tblStockHistory.BatchUnique = tblStock.BatchUnique where " + sqlitem + "   VchDate >= '" + FD.ToString("dd-MMM-yyyy") + "'";
                        SqlConnection conn1 = new SqlConnection(constr);
                        conn1.Open();
                        SqlCommand cmd1 = new SqlCommand(Sql1, conn1);
                        cmd1.ExecuteNonQuery();

                        conn1.Close();

                        new frmReportView1(Fname, "", "", "", dtpFD.Text, "", this.MdiParent, "", "", "", "", supplier, product, mnf, txtItem.Text).Show();
                    }
                }
                if (rdoStockValue.Checked == true)
                {
                    tlpSupplier.Visible = false;
                    DropPurchaseView();
                    string Fname = "Stock Value";
                    if (checkBox1.Checked == false)
                    {
                        string Sql1 = "create view vwstock as select CCName as[Cost Center],ItemCode as[Item Code],ItemName as [Item Name],tblStock.BatchUnique,cast(tblStock.MRP as numeric(36,2)) as MRP,cast (sum(QtyIn-QtyOut) as numeric(36,2))  as QOH,cast(tblStock.prate as numeric(36,2)) as [Purchase Rate],cast(((sum(QtyIn-QtyOut)*tblStock.PRate))as numeric(36,2)) as [Purchase Value],cast(tblStock.CostRateInc as numeric(36,2)) as [Cost Rate Inclusive],cast(sum(QtyIn-QtyOut)*tblStock.CostRateInc as numeric(36,2)) as [Cost Value],cast(tblStock.TaxPer as numeric(36,2 )) as [Tax Per],cast((sum(QtyIn-QtyOut)*tblStock.PRate)*tblStock.TaxPer/100 as numeric(36,2)) as [Tax Amount] from tblStockHistory join tblItemMaster on tblStockHistory.ItemID=tblItemMaster.ItemID join tblCostCentre on tblStockHistory.CCID=tblCostCentre.CCID join tblstock on tblStockHistory.BatchUnique=tblStock.BatchUnique where " + sqlitem + "  ID !=0 group by tblStockHistory.ItemID,ItemName,ItemCode,tblStock.PRate,tblStock.CostRateInc,tblStock.TaxPer,tblStock.MRP,tblStock.CCID,CCName,tblStock.BatchUnique ";
                        SqlConnection conn1 = new SqlConnection(constr);
                        conn1.Open();
                        SqlCommand cmd1 = new SqlCommand(Sql1, conn1);
                        cmd1.ExecuteNonQuery();
                        conn1.Close();
                    }
                    else
                    {


                        string Sql1 = "create view vwstock as select CCName as[Cost Center],ItemCode as[Item Code],ItemName as [Item Name],tblStock.BatchUnique,cast(tblStock.MRP as numeric(36,2)) as MRP,cast (sum(QtyIn-QtyOut) as numeric(36,2))  as QOH,cast(tblStock.prate as numeric(36,2)) as [Purchase Rate],cast(((sum(QtyIn-QtyOut)*tblStock.PRate))as numeric(36,2)) as [Purchase Value],cast(tblStock.CostRateInc as numeric(36,2)) as [Cost Rate Inclusive],cast(sum(QtyIn-QtyOut)*tblStock.CostRateInc as numeric(36,2)) as [Cost Value],cast(tblStock.TaxPer as numeric(36,2 )) as [Tax Per],cast((sum(QtyIn-QtyOut)*tblStock.PRate)*tblStock.TaxPer/100 as numeric(36,2)) as [Tax Amount] from tblStockHistory join tblItemMaster on tblStockHistory.ItemID=tblItemMaster.ItemID join tblCostCentre on tblStockHistory.CCID=tblCostCentre.CCID join tblstock on tblStockHistory.BatchUnique=tblStock.BatchUnique where " + sqlitem + "  ID !=0 group by tblStockHistory.ItemID,ItemName,ItemCode,tblStock.PRate,tblStock.CostRateInc,tblStock.TaxPer,tblStock.MRP,tblStockHistory.CCID,CCName,tblStock.BatchUnique having sum(QtyIn-QtyOut)>0";
                        SqlConnection conn1 = new SqlConnection(constr);
                        conn1.Open();
                        SqlCommand cmd1 = new SqlCommand(Sql1, conn1);
                        cmd1.ExecuteNonQuery();
                        conn1.Close();
                    }
                   

                    new frmReportView1(Fname, "", "", "", "", "",this.MdiParent, "", "", "", "", supplier, product, mnf).Show();
                }
                if (rdoRol.Checked == true)
                {
                    tlpSupplier.Visible = false;
                    string Fname = "Stock Rol";
                    new frmReportView1(Fname, "", "", "", "", "", this.MdiParent, "", "", "", "", supplier, product, mnf).Show();
                }
                if (rdoMoq.Checked == true)
                {
                    tlpSupplier.Visible = false;
                    string Fname = "Stock Moq";
                    new frmReportView1(Fname, "", "", "", "", "", this.MdiParent, "", "", "", "", supplier, product, mnf).Show();
                }
                if (rdoZerostock.Checked == true)
                {
                    tlpSupplier.Visible = false;
                    string Fname = "Stock Zero";
                    new frmReportView1(Fname, "", "", "", "", "", this.MdiParent, "", "", "", "", supplier, product, mnf).Show();
                }
                if (rdoFastmoving.Checked == true)
                {
                    string Fname = "Fast Moving";
                    new frmReportView1(Fname, "", "", "", "", "", this.MdiParent, "", "", "", "", supplier, product, mnf).Show();
                }
                if (rdoSlowmoving.Checked == true)
                {
                    tlpSupplier.Visible = false;
                    string Fname = "Slow Moving";
                    new frmReportView1(Fname, "", "", "", "", "", this.MdiParent, "", "", "", "", supplier, product, mnf).Show();
                }
                if (rdoNonMoving.Checked == true)
                {
                    tlpSupplier.Visible = false;
                    string Fname = "Non Moving";
                    new frmReportView1(Fname, "", "", "", "", "",this.MdiParent, "", "", "", "", supplier, product, mnf).Show();
                }
                if (rdoSupplierstock.Checked == true)
                {
                    tlpSupplier.Visible = true;
                    DropPurchaseView();
                    string sqlSupplier = "tblPurchase.LedgerId =100";

                    if (txtSupplier.Text != "")
                    {
                        sqlSupplier = " tblPurchase.LedgerId = " + Conversion.Val(lblLID.Text) + " ";
                    }
                    string Sql1 = "create view vwstock as select (select distinct partyCode from tblPurchase where " + sqlSupplier + ") as party,CCName as CostCenter,ItemCode,ItemName,tblStockHistory.BatchUnique,tblStock.MRP,UnitName,ROUND(sum(QtyIn - QtyOut), 2) as QOH,ROL,(ROL-ROUND(sum(QtyIn - QtyOut), 2)) as  MinPurchaseQty from tblStockHistory join tblItemMaster on tblStockHistory.ItemID = tblItemMaster.ItemID join tblCostCentre on tblStockHistory.CCID = tblCostCentre.CCID join tblUnit on tblItemMaster.UNITID = tblUnit.UnitID  join tblstock on tblStockHistory.BatchUnique = tblStock.BatchUnique  where tblStockHistory.itemid in (SELECT tblPurchaseItem.ItemId FROM tblPurchaseitem  INNER JOIN tblpurchase ON tblpurchase.InvId = tblPurchaseitem.InvId INNER JOIN tblItemMaster on tblItemMaster.ItemID = tblPurchaseItem.ItemId where " + sqlSupplier + "  ) group by CCName,ItemCode,ItemName,UnitName,ROL,tblStockHistory.BatchUnique,tblStock.MRP";
                    SqlConnection conn1 = new SqlConnection(constr);
                    conn1.Open();
                    SqlCommand cmd1 = new SqlCommand(Sql1, conn1);
                    cmd1.ExecuteNonQuery();
                    conn1.Close();

                    string Fname = "Supplier Wise ROL";
                    new frmReportView1(Fname, "", "", "", "", "", this.MdiParent, "", "", "", "", supplier, product, mnf).Show();
                }
                if (rdoSup.Checked == true)
                {
                    tlpSupplier.Visible = true;
                    DropPurchaseView();
                    string sqlSupplier = "tblPurchase.LedgerId =100";

                    if (txtSupplier.Text != "")
                    {
                        sqlSupplier = " tblPurchase.LedgerId = " + Conversion.Val(lblLID.Text) + "";
                    }
                    string Sql1 = "create view vwstock as select (select distinct partyCode from tblPurchase where " + sqlSupplier + ") as party,CCName as CostCenter,ItemCode,ItemName,tblStockHistory.BatchUnique,tblStock.MRP,UnitName,ROUND(sum(QtyIn - QtyOut), 2) as QOH,ROL from tblStockHistory join tblItemMaster on tblStockHistory.ItemID = tblItemMaster.ItemID join tblCostCentre on tblStockHistory.CCID = tblCostCentre.CCID join tblUnit on tblItemMaster.UNITID = tblUnit.UnitID join tblstock on tblStockHistory.BatchUnique = tblStock.BatchUnique  where tblStockHistory.itemid in (SELECT tblPurchaseItem.ItemId FROM tblPurchaseitem  INNER JOIN tblpurchase ON tblpurchase.InvId = tblPurchaseitem.InvId INNER JOIN tblItemMaster on tblItemMaster.ItemID = tblPurchaseItem.ItemId  where " + sqlSupplier + "  ) group by CCName,ItemCode,ItemName,UnitName,ROL,tblStockHistory.BatchUnique,tblStock.MRP";
                    SqlConnection conn1 = new SqlConnection(constr);
                    conn1.Open();
                    SqlCommand cmd1 = new SqlCommand(Sql1, conn1);
                    cmd1.ExecuteNonQuery();
                    conn1.Close();

                    string Fname = "Supplier Items";
                    new frmReportView1(Fname, "", "", "", "", "", this.MdiParent, "", "", "", "", supplier, product, mnf).Show();
                }

                if (rdoExpiry.Checked == true)
                {
                    DropPurchaseView();
                    string Fname = "Stock Expiry";
                    DateTime TD = Convert.ToDateTime(dtpFD.Text);

                    string Sql1 = "create view vwstock as select CCName,ItemCode,ItemName,Batchcode,tblStockHistory.MRP,ROUND(sum(QtyIn-QtyOut),2) as QOH,prate,((sum(QtyIn-QtyOut)*PRate)) as Pvalue,CostRateInc,(sum(QtyIn-QtyOut)*CostRateInc)as Cvalue,TaxPer,(sum(QtyIn-QtyOut)*PRate)*TaxPer/100 as TaxAmount,Expiry from tblStockHistory join tblItemMaster on tblStockHistory.ItemID=tblItemMaster.ItemID join tblCostCentre on tblStockHistory.CCID=tblCostCentre.CCID where " + sqlitem + " ID !=0 And Expiry < '" + TD.ToString("yyyy-MMM-dd") + "' group by tblStockHistory.ItemID,ItemName,ItemCode,PRate,CostRateInc,TaxPer,tblStockHistory.MRP,tblStockHistory.CCID,CCName,Expiry,Batchcode";
                    SqlConnection conn1 = new SqlConnection(constr);
                    conn1.Open();
                    SqlCommand cmd1 = new SqlCommand(Sql1, conn1);
                    cmd1.ExecuteNonQuery();
                    conn1.Close();


                    new frmReportView1(Fname, "", "", "", "", "", this.MdiParent, "", "", "", "", supplier, product, mnf).Show();




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

        private void tlpMain_Paint(object sender, PaintEventArgs e)
        {

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
                        new frmCompactCheckedListSearch(GetFromCheckedListCat, sQuery, "Category", txtCategory.Location.X + 780, txtCategory.Location.Y + 280, 0, 2, txtCategory.Text, 0, 0, "", lblCatIds.Text, null, "Category").ShowDialog();
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

        private void rdoStockConsolidate_Click(object sender, EventArgs e)
        {

            tlpSupplier.Visible = false;
            tlpMnf.Visible = false;
            tlpItem.Visible = true;
            tlpMnf.Visible = false;
            tlpProduct.Visible = false;
            tlpCategory.Visible = false;
            checkBox1.Visible = false;

        }
    }
    #endregion

}
