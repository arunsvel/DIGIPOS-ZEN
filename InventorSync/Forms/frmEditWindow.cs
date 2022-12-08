using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using InventorSync.InventorBL.Helper;
using InventorSync.InventorBL.Master;
using InventorSync.Info;
using InventorSync.InventorBL.Accounts;
using InventorSync.InventorBL.Transaction;
using InventorSync.JsonClass;
using System.Data.SqlClient;
using Newtonsoft.Json;
using System.Reflection;
using Syncfusion.GridHelperClasses;
using System.Runtime.InteropServices;

//using Syncfusion.WinForms.DataGrid;

namespace InventorSync
{
    public partial class frmEditWindow : Form, IMessageFilter
    {
        // ======================================================== >>
        // Description: The Edit Form for Edit/Delete Changes
        // Developed By: Dipu Joseph
        // Completed Date & Time: 09-Sep-2021 8.00 PM
        // Last Edited By:
        // Last Edited Date & Time:
        // ======================================================== >>

        //string mSelectedMenuType = "";

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        public const int WM_LBUTTONDOWN = 0x0201;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        private HashSet<Control> controlsToMove = new HashSet<Control>();

        int mVchtypeid = 0;
        int mParentID = 0;

        public frmEditWindow(string sFormName = "", object MDIParent = null, string DefaultFocus = "", int VchtypeID = 0, int ParentID = 0)
        {
            InitializeComponent();
            Application.AddMessageFilter(this);

            mVchtypeid = VchtypeID;
            mParentID = ParentID;

            try
            {
                Comm.TransparentControls(this);
                Comm.SetControlColours(this);

                this.BackColor = Color.FromArgb(249, 246, 238);

                lblHeader.Font = new Font("Tahoma", 21, FontStyle.Regular, GraphicsUnit.Pixel);
                lblNew.Font = new Font("Tahoma", 10, FontStyle.Regular, GraphicsUnit.Point);
                lblEdit.Font = new Font("Tahoma", 10, FontStyle.Regular, GraphicsUnit.Point);
                lblDelete.Font = new Font("Tahoma", 10, FontStyle.Regular, GraphicsUnit.Point);
                lblCancel.Font = new Font("Tahoma", 10, FontStyle.Regular, GraphicsUnit.Point);

                lblHeader.ForeColor = Color.Black;
                lblNew.ForeColor = Color.Black;
                lblEdit.ForeColor = Color.Black;
                lblDelete.ForeColor = Color.Black;
                lblCancel.ForeColor = Color.Black;

                rdoMasters.ForeColor = Color.Black;
                rdoTransaction.ForeColor = Color.Black;
                rdoAnalysis.ForeColor = Color.Black;
                rdoUserMngmt.ForeColor = Color.Black;
                rdoUserGroup.ForeColor = Color.Black;
                rdoUser.ForeColor = Color.Black;
                rdoStockAnalysis.ForeColor = Color.Black;

                btnNew.Image = global::DigiposZen.Properties.Resources.plus_1;
                btnCancelDeactive.Image = global::DigiposZen.Properties.Resources.archive123;
                btnDelete.Image = global::DigiposZen.Properties.Resources.delete340402;
                btnMinimize.Image = global::DigiposZen.Properties.Resources.minimize_finalised;
                btnClose.Image = global::DigiposZen.Properties.Resources.logout_Final;
            }
            catch
            { }

            rdoReport.Visible = false;

            controlsToMove.Add(this);
            controlsToMove.Add(this.lblHeader);//Add whatever controls here you want to move the form when it is clicked and dragged

            frmMDI form = (frmMDI)MDIParent;
            this.MdiParent = form;
            
            if (form != null)
            {
                int l = form.ClientSize.Width - 10; //(this.MdiParent.ClientSize.Width - this.Width) / 2;
                int t = form.ClientSize.Height - 80; //((this.MdiParent.ClientSize.Height - this.Height) / 2) - 30;
                this.SetBounds(5, 0, l, t);
            }

            //this.gridGroupingControlSearch.QueryCellStyleInfo += new Syncfusion.Windows.Forms.Grid.Grouping.GridTableCellStyleInfoEventHandler(gridGroupingControlSearch_QueryCellStyleInfo);
            //this.gridGroupingControlSearch.TableOptions.ListBoxSelectionMode = SelectionMode.One;
            //this.gridGroupingControlSearch.TableOptions.SelectionBackColor = Color.Cyan;

            //this.gridGroupingControlSearch.BrowseOnly = true;
            //Syncfusion.GridHelperClasses.GridExcelFilter gridExcelFilter = new GridExcelFilter();
            //gridExcelFilter.WireGrid(this.gridGroupingControlSearch);

            if (ParentID == 0)
            {
                if (sFormName.ToUpper() == "FRMITEMCATEGORY")
                {
                    rdoCategory.Checked = true;
                    GetDataAsperMenuClick("CATEGORIES");
                }
                if (sFormName.ToUpper() == "FRMMANUFACTURER")
                {
                    rdoManufacturer.Checked = true;
                    GetDataAsperMenuClick("MANUFACTURER");
                }
                if (sFormName.ToUpper() == "FRMBRANDMASTER")
                {
                    rdoBrand.Checked = true;
                    GetDataAsperMenuClick("BRAND");
                }
                if (sFormName.ToUpper() == "FRMSIZEMASTER")
                {
                    rdoSize.Checked = true;
                    GetDataAsperMenuClick("SIZE");
                }
                if (sFormName.ToUpper() == "FRMCOLORMASTER")
                {
                    rdoColor.Checked = true;
                    GetDataAsperMenuClick("COLOR");
                }
                if (sFormName.ToUpper() == "FRMUNITMASTER")
                {
                    rdoUnit.Checked = true;
                    GetDataAsperMenuClick("UNIT");
                }
                if (sFormName.ToUpper() == "FRMDISCOUNTGROUP")
                {
                    rdoDiscGroup.Checked = true;
                    GetDataAsperMenuClick("DISCGROUP");
                }
                if (sFormName.ToUpper() == "FRMAREAMASTER")
                {
                    rdoArea.Checked = true;
                    GetDataAsperMenuClick("AREA");
                }
                if (sFormName.ToUpper() == "FRMAGENTMASTER")
                {
                    rdoAgent.Checked = true;
                    GetDataAsperMenuClick("AGENT");
                }
                if (sFormName.ToUpper() == "FRMLEDGER")
                {
                    rdoLedger.Checked = true;
                    GetDataAsperMenuClick("LEDGER");
                }
                if (sFormName.ToUpper() == "FRMTAXMODE")
                {
                    rdoTaxMode.Checked = true;
                    GetDataAsperMenuClick("TAXMODE");
                }
                if (sFormName.ToUpper() == "FRMACCOUNTGROUP")
                {
                    rdoAccountGroup.Checked = true;
                    GetDataAsperMenuClick("ACCOUNTGROUP");
                }
                if (sFormName.ToUpper() == "FRMUSERGROUP")
                {
                    rdoUserGroup.Checked = true;
                    GetDataAsperMenuClick("USERGROUP");
                }
                if (sFormName.ToUpper() == "FRMSETTINGS")
                {
                    rdoSettings.Checked = true;
                    GetDataAsperMenuClick("SETTINGS");
                }
                if (sFormName.ToUpper() == "FRMSTATE")
                {
                    rdoState.Checked = true;
                    GetDataAsperMenuClick("STATE");
                }
                if (sFormName.ToUpper() == "FRMCOSTCENTRE")
                {
                    rdoCostCentre.Checked = true;
                    GetDataAsperMenuClick("COSTCENTRE");
                }
                if (sFormName.ToUpper() == "FRMEMPLOYEE")
                {
                    rdoEmployee.Checked = true;
                    GetDataAsperMenuClick("EMPLOYEE");
                }
                if (sFormName.ToUpper() == "FRMSTOCKDEPARTMENT")
                {
                    rdoStockDepartment.Checked = true;
                    GetDataAsperMenuClick("STOCKDEPARTMENT");
                }
                if (sFormName.ToUpper() == "FRMDEPARTMENT")
                {
                    rdoDepartment.Checked = true;
                    GetDataAsperMenuClick("DEPARTMENT");
                }
                if (sFormName.ToUpper() == "FRMCASHDESKMASTER")
                {
                    rdoCashDesk.Checked = true;
                    GetDataAsperMenuClick("CASHDESK");
                }
                if (sFormName.ToUpper() == "FRMUSER")
                {
                    rdoUser.Checked = true;
                    GetDataAsperMenuClick("USER");
                }
                if (sFormName.ToUpper() == "FRMITEMMASTER")
                {
                    rdoItemMaster.Checked = true;
                    GetDataAsperMenuClick("FRMITEMMASTER");
                }
            }

            Comm.LoadAppSettings();
            ApplicationSettings();
            RadioButtonDoubleClick();

            this.gridGroupingControlSearch.TableModel.ReadOnly = true;

        }

        #region "VARIABLES -------------------------------------------- >>"
        Common Comm = new Common();
        clsEditCommand EdtComm = new clsEditCommand();
        clsBrandMaster clsBrand = new clsBrandMaster();
        clsColorMaster clsColor = new clsColorMaster();
        clsSizeMaster clssize = new clsSizeMaster();
        clsDiscountGroup clsDiscG = new clsDiscountGroup();
        clsUnitMaster clsUnit = new clsUnitMaster();
        clsAgentMaster clsAgent = new clsAgentMaster();
        clsCategory clsCat = new clsCategory();
        clsManufacturer clsManf = new clsManufacturer();
        clsAreaMaster clsArea = new clsAreaMaster();
        clsItemMaster clsItem = new clsItemMaster();
        clsLedger clsLedger = new clsLedger();
        clsMaster clsMaster = new clsMaster();
        clsTaxMode clsTax = new clsTaxMode();
        clsAccountGroup clsAccGroup = new clsAccountGroup();
        clsUserGroup clsuser = new clsUserGroup();
        clsVoucherType clsVchTyp = new clsVoucherType();
        clsPurchase clsPur = new clsPurchase();
        clsSales clsSale = new clsSales();
        clsAccounts clsAcc = new clsAccounts();
        //clsPurchaseDetail clsPurDetail = new clsPurchaseDetail();
        clsState clsStat = new clsState();
        clsCostCentre clsCostCtr = new clsCostCentre();
        clsEmployee clsEmp = new clsEmployee();
        clsUser clsUsr = new clsUser();
        clsDepartment clsDepart = new clsDepartment();
        clsCashDeskMaster clsCashDesk = new clsCashDeskMaster();

        UspGetCategoriesinfo Catinfo = new UspGetCategoriesinfo();
        UspGetManufacturerInfo Manfinfo = new UspGetManufacturerInfo();
        UspGetBrandinfo GetBrandInfo = new UspGetBrandinfo();
        UspGetColorInfo GetcolorInfo = new UspGetColorInfo();
        UspGetSizeInfo Getsizeinfo = new UspGetSizeInfo();
        UspGetDiscountGroupInfo GetDiscGinfo = new UspGetDiscountGroupInfo();
        UspGetUnitInfo GetUnitInfo = new UspGetUnitInfo();
        UspGetAgentinfo GetAgentinfo = new UspGetAgentinfo();
        UspAreaMasterInfo GetAreaInfo = new UspAreaMasterInfo();
        UspGetItemMasterFromStockInfo GetItem = new UspGetItemMasterFromStockInfo();
        UspGetLedgerInfo GetLedgerInfo = new UspGetLedgerInfo();
        UspGetMasterInfo GetMaster = new UspGetMasterInfo();
        UspGetTaxModeInfo GetTaxModeInfo = new UspGetTaxModeInfo();
        UspGetAccountGroupInfo GetAccGroupInfo = new UspGetAccountGroupInfo();
        UspGetUserGroupMasterInfo GetuserInfo = new UspGetUserGroupMasterInfo();
        UspGetVchTypeInfo GetVch = new UspGetVchTypeInfo();
        UspGetPurchaseInfo GetPurchaseInfo = new UspGetPurchaseInfo();
        UspGetSalesInfo GetSalesInfo = new UspGetSalesInfo();
        UspGetAccountsInfo GetAccInfo = new UspGetAccountsInfo();
        UspGetStateInfo GetStateinfo = new UspGetStateInfo();
        UspGetCostCentreInfo GetCostCentreinfo = new UspGetCostCentreInfo();
        UspGetEmployeeInfo GetEmpInfo = new UspGetEmployeeInfo();
        UspGetUserMasterInfo GetUsrInfo = new UspGetUserMasterInfo();
        UspGetDepartmentInfo GetDepartmentinfo = new UspGetDepartmentInfo();

        UspInsertCategoryInfo Categoryinfo = new UspInsertCategoryInfo();
        UspManufacturerInsertInfo ManfInsert = new UspManufacturerInsertInfo();
        UspInsertBrandMasterInfo BrandInsert = new UspInsertBrandMasterInfo();
        UspInsertColorMasterInfo ColorInsert = new UspInsertColorMasterInfo();
        UspInsertSizeMasterInfo SizeInsert = new UspInsertSizeMasterInfo();
        UspInsertUnitMasterInfo UnitInsert = new UspInsertUnitMasterInfo();
        UspInsertDiscountGroupInfo DiscountInsert = new UspInsertDiscountGroupInfo();
        UspAreaMasterInfo Areainsert = new UspAreaMasterInfo();
        UspAgentMasterInfo Agentinsert = new UspAgentMasterInfo();
        UspItemMasterInsertInfo ItemInsert = new UspItemMasterInsertInfo();
        UspItemMasterInsertInfo itemInsertInfo = new UspItemMasterInsertInfo();
        UspLedgerInsertInfo LedgerInsert = new UspLedgerInsertInfo();
        UspTaxModeInsertInfo TaxModeInsert = new UspTaxModeInsertInfo();
        UspAccountGroupInsertInfo AccGroupInsert = new UspAccountGroupInsertInfo();
        UspUserGroupMasterInsertInfo UserGroupInfo = new UspUserGroupMasterInsertInfo();
        UspVchTypeInsertInfo VchInsertinfo = new UspVchTypeInsertInfo();
        UspInsertStateInfo Stateinfo = new UspInsertStateInfo();
        UspCostCentreInsertInfo CostCentrinfo = new UspCostCentreInsertInfo();
        UspEmployeeInsertInfo EmpInfo = new UspEmployeeInsertInfo();
        UspUserMasterInsertInfo UserInfo = new UspUserMasterInsertInfo();
        UspDepartmentInsertInfo Departmentinfo = new UspDepartmentInsertInfo();
        UspInsertCashDeskMaster CashDeskinfo = new UspInsertCashDeskMaster();
        UspGetCashDeskIMasterInfo GetCashDeskinfo = new UspGetCashDeskIMasterInfo();
        //clsJsonPurchaseMaster PurchaseMasterInfo = new clsJsonPurchaseMaster();
        //clsJsonPurchaseDetail PurchaseDetailInfo = new clsJsonPurchaseDetail();
        DataTable dtgetData = new DataTable();

        string strRowIndex;
        public bool bFromEdit = true;
        int ParentCount = 0;
        string sRet = "";
        string[] strResult;
        string strNodeName ="";
        int itvwSelectedNodeID;
        int itvwParentNodeID;
        int ITransParentID;
        bool bTransNode = false;
        string strGtidColumnSize = "";
        string strFormHeaderName = "";
        int ibtnNumber=1;
        #endregion

        #region "EVENTS ----------------------------------------------- >>"
        //SyncFusion Grid Event
        private void gridGroupingControlSearch_TableControlCellDoubleClick(object sender, Syncfusion.Windows.Forms.Grid.Grouping.GridTableControlCellClickEventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            if (rdoCategory.Checked == true)
            {
                frmItemCategory frm = new frmItemCategory(GetSelectedRowID("CategoryID"));
                frm.MdiParent = this.MdiParent;
                frm.Show();
                frm.BringToFront();
            }
            else if (rdoManufacturer.Checked == true)
            {
                frmManufacturer frm = new frmManufacturer(GetSelectedRowID("MnfID"));
                frm.MdiParent = this.MdiParent;
                frm.Show();
                frm.BringToFront();
            }
            else if (rdoBrand.Checked == true)
            {
                frmBrandMaster frm = new frmBrandMaster(GetSelectedRowID("brandID"));
                frm.MdiParent = this.MdiParent;
                frm.Show();
                frm.BringToFront();
            }
            else if (rdoColor.Checked == true)
            {
                frmBrandMaster frm = new frmBrandMaster(GetSelectedRowID("ColorID"));
                frm.MdiParent = this.MdiParent;
                frm.Show();
                frm.BringToFront();
            }
            else if (rdoUnit.Checked == true)
            {
                frmBrandMaster frm = new frmBrandMaster(GetSelectedRowID("UnitID"));
                frm.MdiParent = this.MdiParent;
                frm.Show();
                frm.BringToFront();
            }
            else if (rdoDiscGroup.Checked == true)
            {
                frmDiscountGroup frm = new frmDiscountGroup(GetSelectedRowID("DiscountGroupID"));
                frm.MdiParent = this.MdiParent;
                frm.Show();
                frm.BringToFront();
            }
            else if (rdoSize.Checked == true)
            {
                frmBrandMaster frm = new frmBrandMaster(GetSelectedRowID("SizeID"));
                frm.MdiParent = this.MdiParent;
                frm.Show();
                frm.BringToFront();
            }
            else if (rdoArea.Checked == true)
            {
                frmAreaMaster frm = new frmAreaMaster(GetSelectedRowID("AreaID"));
                frm.MdiParent = this.MdiParent;
                frm.Show();
                frm.BringToFront();
            }
            else if (rdoAgent.Checked == true)
            {
                frmAgentMaster frm = new frmAgentMaster(GetSelectedRowID("AgentID"));
                frm.MdiParent = this.MdiParent;
                frm.Show();
                frm.BringToFront();
            }
            else if (rdoLedger.Checked == true)
            {
                frmLedger frm = new frmLedger(GetSelectedRowID("LID"));
                frm.MdiParent = this.MdiParent;
                frm.Show();
                frm.BringToFront();
            }
            else if (rdoTaxMode.Checked == true)
            {
                frmTaxMode frm = new frmTaxMode(GetSelectedRowID("TaxModeID"));
                frm.MdiParent = this.MdiParent;
                frm.Show();
                frm.BringToFront();
            }
            else if (rdoAccountGroup.Checked == true)
            {
                frmAccountGroup frm = new frmAccountGroup(GetSelectedRowID("AccountGroupID"));
                frm.MdiParent = this.MdiParent;
                frm.Show();
                frm.BringToFront();
            }
            else if (rdoUserGroup.Checked == true)
            {
                frmUserGroup frm = new frmUserGroup(GetSelectedRowID("ID"));
                frm.MdiParent = this.MdiParent;
                frm.Show();
                frm.BringToFront();
            }
            else if (rdoState.Checked == true)
            {
                frmState frm = new frmState(GetSelectedRowID("StateId"));
                frm.MdiParent = this.MdiParent;
                frm.Show();
                frm.BringToFront();
            }
            else if (rdoCostCentre.Checked == true)
            {
                frmCostCentre frm = new frmCostCentre(GetSelectedRowID("CCID"));
                frm.MdiParent = this.MdiParent;
                frm.Show();
                frm.BringToFront();
            }
            Cursor.Current = Cursors.Default;
        }
        private void gridGroupingControlSearch_TableControlCellDoubleClick_1(object sender, Syncfusion.Windows.Forms.Grid.Grouping.GridTableControlCellClickEventArgs e)
        {
            try
            {
                btnEdit_Click(sender, e);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
            }

        }
        private void gridGroupingControlSearch_TableControlPrepareViewStyleInfo(object sender, Syncfusion.Windows.Forms.Grid.Grouping.GridTableControlPrepareViewStyleInfoEventArgs e)
        {
            //if (e.Inner.RowIndex == this.gridGroupingControlSearch.TableControl.CurrentCell.RowIndex)
            //e.Inner.Style.BackColor = Color.Yellow;
        }
        private void gridGroupingControlSearch_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            try
            {
                btnEdit_Click(sender, e);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
            }
        }
        private void gridGroupingControlSearch_QueryCellStyleInfo(object sender, Syncfusion.Windows.Forms.Grid.Grouping.GridTableCellStyleInfoEventArgs e)
        {
            //if (e.TableCellIdentity.DisplayElement.Kind == Syncfusion.Grouping.DisplayElementKind.Record && e.TableCellIdentity.DisplayElement.GetRecord().IsSelected())
            //{
            //    e.Style.Font.Bold = true;
            //    e.Style.TextColor = Color.Blue;
            //}
        }
        private void frmEditWindow_Load(object sender, EventArgs e)
        {
            rdoMasters.Checked = true;
            cboListCount.SelectedIndex = 0;
            txtSearch.Select(0, txtSearch.Text.Length);
            
            if (strFormHeaderName == "")
            {
                rdoCategory.Checked = true;
                rdoCategory.PerformClick();
            }

            HideButtons();
            FillTreeview();

            rdoMasters.Checked = true;
            rdoMasters.PerformClick();

            if (mParentID > 0)
            {
                sqlControl rs = new sqlControl();

                rs.Open("Select Parentid, Vchtypeid, vchtype from tblvchtype where vchtypeid=" + mVchtypeid.ToString());
                if (rs.eof() == false)
                {
                    itvwSelectedNodeID = Comm.ToInt32(rs.fields("Vchtypeid").ToString());

                    TreeNode tnParent = new TreeNode();
                    tnParent.Name = rs.fields("Parentid");
                    tnParent.Text = rs.fields("Parentid");

                    rdoTransaction.Checked = true;
                    this.tlpNavigator.RowStyles[1].Height = 0;
                    this.tlpNavigator.RowStyles[3].Height = 600;

                    Application.DoEvents();

                    trvwParentTransction.ExpandAll();
                    trvwParentTransction.Select();

                    trvwParentTransction.SelectedNode = trvwParentTransction.Nodes.Find(rs.fields("Vchtypeid"), true)[0];

                    FillTransactionDetails(tnParent);
                }
            }
        }
        private void frmEditWindow_KeyDown(object sender, KeyEventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            if (e.KeyCode == Keys.F3)//Find
            {
                try
                {
                    btnNew_Click(sender, e);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                finally
                {
                }
            }
            else if (e.KeyCode == Keys.F4)//Edit
            {
                try
                {
                        btnEdit_Click(sender, e);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                finally
                {
                }
            }
            else if (e.KeyCode == Keys.F5)//Refresh
            {
                try
                {
                    btnRefresh_Click(sender, e);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                finally
                {
                }
            }
            else if (e.KeyCode == Keys.F7)//delete
            {
                try
                {
                    btnDelete_Click(sender, e);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                finally
                {
                }
            }
            else if (e.KeyCode == Keys.Enter)
            {
                try
                {
                    if (this.ActiveControl.Name == "tableControl1" )
                    {
                        btnEdit_Click(sender, e);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                finally
                {
                }
            }
            Cursor.Current = Cursors.Default;
        }
        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        //Parent Radio Button checked
        private void rdoMasters_CheckedChanged(object sender, EventArgs e)
        {
            //commented By Anjitha 03/03/2022 for rdo transaction click take 2 times
            //if (rdoMasters.Checked == true)
            //{
            //    this.tlpNavigator.RowStyles[1].SizeType = SizeType.Absolute;
            //    this.tlpNavigator.RowStyles[1].Height = 660;
            //    TlpNavigatorSizeBasedonAppSettings();
            //}
        }
        private void rdoTransaction_CheckedChanged(object sender, EventArgs e)
        {
            //commented By Anjitha 03/03/2022 for rdo transaction click take 2 times
            //if (rdoTransaction.Checked == true)
            //{
            //    this.tlpNavigator.RowStyles[3].SizeType = SizeType.Absolute;
            //    this.tlpNavigator.RowStyles[3].Height = 800;
            //}
        }
        //Parent Radio Button click
        private void rdoMasters_Click(object sender, EventArgs e)
        {
            if (rdoMasters.Checked == true)
            {
                if (this.tlpNavigator.RowStyles[1].Height == 0)
                {
                    this.tlpNavigator.RowStyles[1].SizeType = SizeType.Absolute;
                    this.tlpNavigator.RowStyles[1].Height = 700;
                    TlpNavigatorSizeBasedonAppSettings();
                }
                else
                {
                    this.tlpNavigator.RowStyles[1].Height = 0;
                }
            }
        }
        private void rdoTransaction_Click(object sender, EventArgs e)
        {
            if (rdoTransaction.Checked == true)
            {
                if (this.tlpNavigator.RowStyles[3].Height == 0)
                {
                    if (this.tlpNavigator.RowStyles[1].Height > 0)
                    {
                        this.tlpNavigator.RowStyles[1].Height = 0;
                        this.tlpNavigator.RowStyles[3].Height = 600;
                    }
                    else
                    {
                        this.tlpNavigator.RowStyles[3].SizeType = SizeType.Absolute;
                        this.tlpNavigator.RowStyles[3].Height = 600;
                    }
                }
                else
                {
                    if (this.tlpNavigator.RowStyles[1].Height > 0)
                    {
                        this.tlpNavigator.RowStyles[1].Height = 0;
                        this.tlpNavigator.RowStyles[3].Height = 600;
                    }

                    else
                        this.tlpNavigator.RowStyles[3].Height = 0;
                }
            }
        }
        private void rdoAnalysis_Click(object sender, EventArgs e)
        {
            if (rdoAnalysis.Checked == true)
            {

               if (this.tlpNavigator.RowStyles[5].Height == 0)
               {
                    this.tlpNavigator.RowStyles[5].SizeType = SizeType.Absolute;
                    this.tlpNavigator.RowStyles[5].Height = 50;
               }
               else
                    this.tlpNavigator.RowStyles[5].Height = 0;
            }
        }
        private void rdoUserMngmt_Click(object sender, EventArgs e)
        {
            if (rdoUserMngmt.Checked == true)
            {
                if (this.tlpNavigator.RowStyles[7].Height == 0)
                {
                    this.tlpNavigator.RowStyles[7].SizeType = SizeType.Absolute;
                    this.tlpNavigator.RowStyles[7].Height = 100;
                }
                else
                    this.tlpNavigator.RowStyles[7].Height = 0;
            }
        }
        private void rdoReport_Click(object sender, EventArgs e)
        {
            if (rdoReport.Checked == true)
            {

                if (this.tlpNavigator.RowStyles[9].Height == 0)
                {
                    this.tlpNavigator.RowStyles[9].SizeType = SizeType.Absolute;
                    this.tlpNavigator.RowStyles[9].Height = 50;
                }
                else
                    this.tlpNavigator.RowStyles[9].Height = 0;
            }
        }
        //Master
        private void rdoCategory_Click(object sender, EventArgs e)
        {
            if (rdoCategory.Checked == true)
            {
                HideButtons();
                AnalysisChecked();
                UserManagementChecked();
                ReportChecked();
                strFormHeaderName = "Category";
                GetDataAsperMenuClick("CATEGORIES");
                ibtnNumber = 1;
            }
        }
        private void rdoManufacturer_Click(object sender, EventArgs e)
        {
            if (rdoManufacturer.Checked == true)
            {
                HideButtons();
                AnalysisChecked();
                UserManagementChecked();
                ReportChecked();
                strFormHeaderName = "Manufacturer";
                GetDataAsperMenuClick("MANUFACTURER");
                ibtnNumber = 2;
            }
        }
        private void rdoBrand_Click(object sender, EventArgs e)
        {
            if (rdoBrand.Checked == true)
            {
                HideButtons();
                AnalysisChecked();
                UserManagementChecked();
                ReportChecked();
                strFormHeaderName = "Brand";
                GetDataAsperMenuClick("BRAND");
                ibtnNumber = 3;
            }
        }
        private void rdoDiscGroup_Click(object sender, EventArgs e)
        {
            if (rdoDiscGroup.Checked == true)
            {
                HideButtons();
                AnalysisChecked();
                UserManagementChecked();
                ReportChecked();
                strFormHeaderName = "Disc.Group";
                GetDataAsperMenuClick("DISCGROUP");
                ibtnNumber = 4;
            }
        }
        private void rdoSize_Click(object sender, EventArgs e)
        {
            if (rdoSize.Checked == true)
            {
                HideButtons();
                AnalysisChecked();
                UserManagementChecked();
                ReportChecked();
                strFormHeaderName = "Size";
                GetDataAsperMenuClick("SIZE");
                ibtnNumber = 5;
            }
        }
        private void rdoColor_Click(object sender, EventArgs e)
        {
            if (rdoColor.Checked == true)
            {
                HideButtons();
                AnalysisChecked();
                UserManagementChecked();
                ReportChecked();
                strFormHeaderName = "Color";
                GetDataAsperMenuClick("COLOR");
                ibtnNumber = 6;
            }
        }
        private void rdoCashDesk_Click(object sender, EventArgs e)
        {
            if (rdoCashDesk.Checked == true)
            {
                HideButtons();
                AnalysisChecked();
                UserManagementChecked();
                ReportChecked();
                strFormHeaderName = "CASHDESK";
                GetDataAsperMenuClick("CASHDESK");
                ibtnNumber = 30;
            }
        }
        private void rdoUnit_Click(object sender, EventArgs e)
        {
            if (rdoUnit.Checked == true)
            {
                HideButtons();
                AnalysisChecked();
                UserManagementChecked();
                ReportChecked();
                strFormHeaderName = "Unit";
                GetDataAsperMenuClick("UNIT");
                ibtnNumber = 7;
            }
        }
        private void rdoItemMaster_Click(object sender, EventArgs e)
        {
            if (rdoItemMaster.Checked == true)
            {
                btnCancelDeactive.Visible = true;
                lblCancel.Visible = true;
                AnalysisChecked();
                UserManagementChecked();
                ReportChecked();
                strFormHeaderName = "Item Master";
                GetDataAsperMenuClick("ITEM");
                ibtnNumber = 8;
            }
        }
        private void rdoAgent_Click(object sender, EventArgs e)
        {
            if (rdoAgent.Checked == true)
            {
                HideButtons();
                AnalysisChecked();
                UserManagementChecked();
                ReportChecked();
                strFormHeaderName = "Agent";
                GetDataAsperMenuClick("AGENT");
                ibtnNumber =10;
            }
        }
        private void rdoArea_Click_1(object sender, EventArgs e)
        {
            if (rdoArea.Checked == true)
            {
                HideButtons();
                AnalysisChecked();
                UserManagementChecked();
                ReportChecked();
                strFormHeaderName = "Area";
                GetDataAsperMenuClick("AREA");
                ibtnNumber = 9;
            }
        }
        private void rdoLedger_Click(object sender, EventArgs e)
        {
            if (rdoLedger.Checked == true)
            {
                HideButtons();
                AnalysisChecked();
                UserManagementChecked();
                ReportChecked();
                strFormHeaderName = "Ledger";
                GetDataAsperMenuClick("LEDGER");
                ibtnNumber = 11;
            }
        }
        private void rdoTaxMode_Click(object sender, EventArgs e)
        {
            if (rdoTaxMode.Checked == true)
            {
                HideButtons();
                AnalysisChecked();
                UserManagementChecked();
                ReportChecked();
                strFormHeaderName = "Tax Mode";
                GetDataAsperMenuClick("TAXMODE");
                ibtnNumber = 12;
            }
        }
        private void rdoAccountGroup_Click(object sender, EventArgs e)
        {
            if (rdoAccountGroup.Checked == true)
            {
                HideButtons();
                AnalysisChecked();
                UserManagementChecked();
                ReportChecked();
                strFormHeaderName = "Account Group";
                GetDataAsperMenuClick("ACCOUNTGROUP");
                ibtnNumber = 13;
            }
        }
        private void rdoVoucherType_Click(object sender, EventArgs e)
        {
            if (rdoVoucherType.Checked == true)
            {
                HideButtons();
                AnalysisChecked();
                UserManagementChecked();
                ReportChecked();
                strFormHeaderName = "Voucher Type";
                GetDataAsperMenuClick("VOUCHERTYP");
                ibtnNumber = 14;
            }
        }
        private void rdoSettings_Click(object sender, EventArgs e)
        {
            if (rdoSettings.Checked == true)
            {
                HideButtons();
                AnalysisChecked();
                UserManagementChecked();
                ReportChecked();
                strFormHeaderName = "Settings";
                GetDataAsperMenuClick("SETTINGS");
                ibtnNumber = 15;
            }
        }
        private void rdoState_Click(object sender, EventArgs e)
        {
            if (rdoState.Checked == true)
            {
                HideButtons();
                AnalysisChecked();
                UserManagementChecked();
                ReportChecked();
                strFormHeaderName = "State";
                GetDataAsperMenuClick("STATE");
                ibtnNumber = 16;
            }
        }
        private void rdoCostCentre_Click(object sender, EventArgs e)
        {
            if (rdoCostCentre.Checked == true)
            {
                HideButtons();
                AnalysisChecked();
                UserManagementChecked();
                ReportChecked();
                strFormHeaderName = "Cost Centre";
                GetDataAsperMenuClick("COSTCENTRE");
                ibtnNumber = 17;
            }
        }
        private void rdoEmployee_Click(object sender, EventArgs e)
        {
            if (rdoEmployee.Checked == true)
            {
                HideButtons();
                AnalysisChecked();
                UserManagementChecked();
                ReportChecked();
                strFormHeaderName = "Employee";
                GetDataAsperMenuClick("EMPLOYEE");
                ibtnNumber = 18;
            }
        }
        private void rdoDepartment_Click(object sender, EventArgs e)
        {

        }
        private void rdoSupplier_Click(object sender, EventArgs e)
        {
            if (rdoSupplier.Checked == true)
            {
                HideButtons();
                AnalysisChecked();
                UserManagementChecked();
                ReportChecked();
                strFormHeaderName = "Supplier";
                GetDataAsperMenuClick("SUPPLIER");
                ibtnNumber = 24;
            }
        }
        private void rdoCustomer_Click(object sender, EventArgs e)
        {
            if (rdoCustomer.Checked == true)
            {
                HideButtons();
                AnalysisChecked();
                UserManagementChecked();
                ReportChecked();
                strFormHeaderName = "Customer";
                GetDataAsperMenuClick("CUSTOMER");
                ibtnNumber = 25;
            }
        }
        //Transaction
        private void trvwParentTransction_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
          
            TreeNode tn = e.Node;
            strNodeName = tn.Text;
            Masterchecked();
            AnalysisChecked();
            UserManagementChecked();
            ReportChecked();

            if (tn != null)
                itvwSelectedNodeID = Convert.ToInt32(tn.Name);

            bTransNode = true;
            lblHeader.Text = "Edit Window " + "(" + strNodeName + ")";
            this.Text = lblHeader.Text;
        }
        private void trvwParentTransction_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode tnParent = e.Node.Parent;
            if (tnParent == null)
                tnParent = e.Node;

            FillTransactionDetails(tnParent);
        }

        private void FillTransactionDetails(TreeNode tnParent)
        {
            itvwParentNodeID = 0;

            if (tnParent != null)
            {
                itvwParentNodeID = Convert.ToInt32(tnParent.Name);
                ITransParentID = itvwParentNodeID;
                if (itvwParentNodeID == 2)
                {
                    GetDataAsperMenuClick("PURCHASE");
                    ibtnNumber = 23;
                }
                else if (itvwParentNodeID == 4)
                {
                    GetDataAsperMenuClick("PURCHASE_RETURN");
                    ibtnNumber = 26;
                }
                else if (itvwParentNodeID == 6)
                {
                    GetDataAsperMenuClick("RECEIPT_NOTE");
                    ibtnNumber = 27;
                }
                if (itvwParentNodeID == 1)
                {
                    GetDataAsperMenuClick("SALES");
                    ibtnNumber = 24;
                }
                else if (itvwParentNodeID == 3)
                {
                    GetDataAsperMenuClick("SALES_RETURN");
                    ibtnNumber = 25;
                }
                else if (itvwParentNodeID == 5)
                {
                    GetDataAsperMenuClick("DELIVERY_NOTE");
                    ibtnNumber = 28;
                }
                else if (itvwParentNodeID == 7)
                {
                    GetDataAsperMenuClick("RECEIPT");
                    ibtnNumber = 29;
                }
                else if (itvwParentNodeID == 8)
                {
                    GetDataAsperMenuClick("PAYMENT");
                    ibtnNumber = 30;
                }
                else if (itvwParentNodeID == 9)
                {
                    GetDataAsperMenuClick("JOURNAL");
                    ibtnNumber = 30;
                }
                else if (itvwParentNodeID == 10)
                {
                    GetDataAsperMenuClick("CONTRA");
                    ibtnNumber = 30;
                }
            }
        }

        //Analysis
        private void rdoStockAnalysis_Click(object sender, EventArgs e)
        {
            if (rdoStockAnalysis.Checked == true)
            {
                HideButtons();
                Masterchecked();
                UserManagementChecked();
                ReportChecked();
                strFormHeaderName = "Stock Analysis";
                GetDataAsperMenuClick("STOCKANALYSIS");
                ibtnNumber = 20;
            }
        }
        //User Management
        private void rdoUserGroup_Click(object sender, EventArgs e)
        {
            if (rdoUserGroup.Checked == true)
            {
                HideButtons();
                Masterchecked();
                AnalysisChecked();
                ReportChecked();
                strFormHeaderName = "UserGroup";
                GetDataAsperMenuClick("USERGROUP");
                ibtnNumber = 21;
            }
        }
        private void rdoUser_Click(object sender, EventArgs e)
        {
            if (rdoUser.Checked == true)
            {
                HideButtons();
                Masterchecked();
                AnalysisChecked();
                ReportChecked();
                strFormHeaderName = "User";
                GetDataAsperMenuClick("USER");
                ibtnNumber = 22;
            }
        }
        private void rdoPurchaseReport_Click(object sender, EventArgs e)
        {
            if (rdoPurchaseReport.Checked == true)
            {
                HideButtons();
                Masterchecked();
                AnalysisChecked();
                UserManagementChecked();
                strFormHeaderName = "Purchase Report";
                GetDataAsperMenuClick("PURCHASEREPORT");
                ibtnNumber = 28;
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            if (itvwParentNodeID <= 0)
            {
                switch (ibtnNumber)
                {
                    case 1:
                        GetDataAsperMenuClick("CATEGORIES");
                        return;
                    case 2:
                        GetDataAsperMenuClick("MANUFACTURER");
                        return;
                    case 3:
                        GetDataAsperMenuClick("BRAND");
                        return;
                    case 4:
                        GetDataAsperMenuClick("DISCGROUP");
                        return;
                    case 5:
                        GetDataAsperMenuClick("SIZE");
                        return;
                    case 6:
                        GetDataAsperMenuClick("COLOR");
                        return;
                    case 7:
                        GetDataAsperMenuClick("UNIT");
                        return;
                    case 8:
                        GetDataAsperMenuClick("ITEM");
                        return;
                    case 9:
                        GetDataAsperMenuClick("AREA");
                        return;
                    case 10:
                        GetDataAsperMenuClick("AGENT");
                        return;
                    case 11:
                        GetDataAsperMenuClick("LEDGER");
                        return;
                    case 12:
                        GetDataAsperMenuClick("TAXMODE");
                        return;
                    case 13:
                        GetDataAsperMenuClick("ACCOUNTGROUP");
                        return;
                    case 14:
                        GetDataAsperMenuClick("VOUCHERTYP");
                        return;
                    case 15:
                        GetDataAsperMenuClick("SETTINGS");
                        return;
                    case 16:
                        GetDataAsperMenuClick("STATE");
                        return;
                    case 17:
                        GetDataAsperMenuClick("COSTCENTRE");
                        return;
                    case 18:
                        GetDataAsperMenuClick("EMPLOYEE");
                        return;
                    case 19:
                        GetDataAsperMenuClick("STOCKDEPARTMENT");
                        return;
                    case 20:
                        GetDataAsperMenuClick("STOCKANALYSIS");
                        return;
                    case 21:
                        GetDataAsperMenuClick("USERGROUP");
                        return;
                    case 22:
                        GetDataAsperMenuClick("USER");
                        return;
                    case 23:
                        GetDataAsperMenuClick("PURCHASE");
                        return;
                    case 24:
                        GetDataAsperMenuClick("SUPPLIER");
                        return;
                    case 25:
                        GetDataAsperMenuClick("CUSTOMER");
                        return;
                    case 26:
                        GetDataAsperMenuClick("PURCHASE_RETURN");
                        return;
                    case 27:
                        GetDataAsperMenuClick("RECEIPT_NOTE");
                        return;
                    case 28:
                        GetDataAsperMenuClick("PURCHASEREPORT");
                        return;
                    case 29:
                        GetDataAsperMenuClick("DEPARTMENT");
                        return;
                    case 30:
                        GetDataAsperMenuClick("CASHDESK");
                        return;
                }
            }
            else
            {
                if (trvwParentTransction.SelectedNode != null)
                {
                    TreeNode tnParent = trvwParentTransction.SelectedNode.Parent;
                    if (tnParent == null)
                        tnParent = trvwParentTransction.SelectedNode;

                    if (ITransParentID > 0)
                    {
                        if (tnParent != null)
                        {
                            itvwParentNodeID = Convert.ToInt32(tnParent.Name);
                            ITransParentID = itvwParentNodeID;
                            if (itvwParentNodeID == 2)
                            {
                                GetDataAsperMenuClick("PURCHASE");
                                ibtnNumber = 31;
                            }
                            else if (itvwParentNodeID == 4)
                            {
                                GetDataAsperMenuClick("PURCHASE_RETURN");
                                ibtnNumber = 32;
                            }
                            else if (itvwParentNodeID == 6)
                            {
                                GetDataAsperMenuClick("RECEIPT_NOTE");
                                ibtnNumber = 33;
                            }
                            if (itvwParentNodeID == 1)
                            {
                                GetDataAsperMenuClick("SALES");
                                ibtnNumber = 34;
                            }
                            else if (itvwParentNodeID == 3)
                            {
                                GetDataAsperMenuClick("SALES_RETURN");
                                ibtnNumber = 35;
                            }
                            else if (itvwParentNodeID == 5)
                            {
                                GetDataAsperMenuClick("DELIVERY_NOTE");
                                ibtnNumber = 36;
                            }
                            else if (itvwParentNodeID == 7)
                            {
                                GetDataAsperMenuClick("RECEIPT");
                                ibtnNumber = 37;
                            }
                            else if (itvwParentNodeID == 8)
                            {
                                GetDataAsperMenuClick("PAYMENT");
                                ibtnNumber = 38;
                            }
                            else if (itvwParentNodeID == 9)
                            {
                                GetDataAsperMenuClick("JOURNAL");
                                ibtnNumber = 39;
                            }
                            else if (itvwParentNodeID == 10)
                            {
                                GetDataAsperMenuClick("CONTRA");
                                ibtnNumber = 40;
                            }
                        }
                    }
                }
            }
        }
        private void btnNew_Click(object sender, EventArgs e)
        {
            if (rdoCategory.Checked == true)
            {
                frmItemCategory frmCat = new frmItemCategory(0, false);
                frmCat.MdiParent = this.MdiParent;
                frmCat.Show();
                frmCat.BringToFront();
            }
            else if (rdoManufacturer.Checked == true)
            {
                frmManufacturer frm = new frmManufacturer(0, false);
                frm.MdiParent = this.MdiParent;
                frm.Show();
                frm.BringToFront();
            }
            else if (rdoBrand.Checked == true)
            {
                frmBrandMaster frm = new frmBrandMaster(0, false);
                frm.MdiParent = this.MdiParent;
                frm.Show();
                frm.BringToFront();
            }
            else if (rdoDiscGroup.Checked == true)
            {
                frmDiscountGroup frm = new frmDiscountGroup(0, false);
                frm.MdiParent = this.MdiParent;
                frm.Show();
                frm.BringToFront();
            }
            else if (rdoSize.Checked == true)
            {
                FrmSizeMaster frm = new FrmSizeMaster(0, false);
                frm.MdiParent = this.MdiParent;
                frm.Show();
                frm.BringToFront();
            }
            else if (rdoColor.Checked == true)
            {
                frmColorMaster frm = new frmColorMaster(0, false);
                frm.MdiParent = this.MdiParent;
                frm.Show();
                frm.BringToFront();
            }
            else if (rdoUnit.Checked == true)
            {
                frmUnitMaster frm = new frmUnitMaster(0, false);
                frm.MdiParent = this.MdiParent;
                frm.Show();
                frm.BringToFront();
            }
            else if (rdoItemMaster.Checked == true)
            {
                frmItemMaster frm = new frmItemMaster(0, true);
                frm.MdiParent = this.MdiParent;
                frm.Show();
                frm.BringToFront();
            }
            else if (rdoArea.Checked == true)
            {
                frmAreaMaster frm = new frmAreaMaster(0, false);
                frm.MdiParent = this.MdiParent;
                frm.Show();
                frm.BringToFront();
            }
            else if (rdoAgent.Checked == true)
            {
                frmAgentMaster frm = new frmAgentMaster(0, false);
                frm.MdiParent = this.MdiParent;
                frm.Show();
                frm.BringToFront();
            }
            else if (rdoSupplier.Checked == true)
            {
                frmLedger frm = new frmLedger(0, false,0, "SUPPLIER");
                frm.MdiParent = this.MdiParent;
                frm.Show();
                frm.BringToFront();
            }
            else if (rdoCustomer.Checked == true)
            {
                frmLedger frm = new frmLedger(0, false, 0, "CUSTOMER");
                frm.MdiParent = this.MdiParent;
                frm.Show();
                frm.BringToFront();
            }
            else if (rdoLedger.Checked == true)
            {
                frmLedger frm = new frmLedger(0, false);
                frm.MdiParent = this.MdiParent;
                frm.Show();
                frm.BringToFront();
            }
            else if (rdoTaxMode.Checked == true)
            {
                frmTaxMode frm = new frmTaxMode(0, false);
                frm.MdiParent = this.MdiParent;
                frm.Show();
                frm.BringToFront();
            }
            else if (rdoAccountGroup.Checked == true)
            {
                frmAccountGroup frm = new frmAccountGroup(0, false);
                frm.MdiParent = this.MdiParent;
                frm.Show();
                frm.BringToFront();
            }
            else if (rdoVoucherType.Checked == true)
            {
                frmVouchertype frm = new frmVouchertype(0, false);
                frm.MdiParent = this.MdiParent;
                frm.Show();
                frm.BringToFront();
            }
            else if (rdoSettings.Checked == true)
            {
                frmSettings frm = new frmSettings(0, false);
                frm.MdiParent = this.MdiParent;
                frm.Show();
                frm.BringToFront();
            }
            else if (rdoState.Checked == true)
            {
                frmState frm = new frmState(0, false);
                frm.MdiParent = this.MdiParent;
                frm.Show();
                frm.BringToFront();
            }
            else if (rdoCostCentre.Checked == true)
            {
                frmCostCentre frm = new frmCostCentre(0, false);
                frm.MdiParent = this.MdiParent;
                frm.Show();
                frm.BringToFront();
            }
            else if (rdoEmployee.Checked == true)
            {
                frmEmployee frm = new frmEmployee(0, false);
                frm.MdiParent = this.MdiParent;
                frm.Show();
                frm.BringToFront();
            }
            else if (rdoStockDepartment.Checked == true)
            {
                frmDepartment frm = new frmDepartment(0, false, 0);
                frm.MdiParent = this.MdiParent;
                frm.Show();
                frm.BringToFront();
            }
            else if (rdoDepartment.Checked == true)
            {
                frmDepartment frm = new frmDepartment(0, false, 1);
                frm.MdiParent = this.MdiParent;
                frm.Show();
                frm.BringToFront();
            }
            else if (rdoCashDesk.Checked == true)
            {
                frmCashDeskMaster frm = new frmCashDeskMaster(0, false);
                frm.MdiParent = this.MdiParent;
                frm.Show();
                frm.BringToFront();
            }
            else if (rdoStockAnalysis.Checked == true) //Analysis
            {
                frmItemAnalysis frm = new frmItemAnalysis();
                frm.MdiParent = this.MdiParent;
                frm.Show();
                frm.BringToFront();
            }
            else if (rdoUserGroup.Checked == true) //User Management
            {
                frmUserGroup frm = new frmUserGroup(0, false);
                frm.MdiParent = this.MdiParent;
                frm.Show();
                frm.BringToFront();
            }
            else if (rdoUser.Checked == true)
            {
                frmUser frm = new frmUser(0, false);
                frm.MdiParent = this.MdiParent;
                frm.Show();
                frm.BringToFront();
            }
            else if (rdoPurchaseReport.Checked == true) //Analysis
            {
                frmPurchaseReport frm = new frmPurchaseReport();
                frm.MdiParent = this.MdiParent;
                frm.Show();
                frm.BringToFront();
            }
            else //Transaction Treeview
            {
                if (ITransParentID > 0)
                {
                    if (ITransParentID == 2) // PURCHASE
                    {
                        frmStockInVoucherNew frm = new frmStockInVoucherNew(itvwSelectedNodeID, 0, false, this.MdiParent);
                        frm.MdiParent = this.MdiParent;
                        frm.Show();
                    }
                    else if (ITransParentID == 4) // PURCHAE RETURN
                    {
                        frmStockInVoucherNew frm = new frmStockInVoucherNew(itvwSelectedNodeID, 0, false, this.MdiParent);
                        frm.MdiParent = this.MdiParent;
                        frm.Show();
                    }
                    else if (ITransParentID == 6) // RECEIPT NOTE
                    {
                        frmStockInVoucherNew frm = new frmStockInVoucherNew(itvwSelectedNodeID, 0, false, this.MdiParent);
                        frm.MdiParent = this.MdiParent;
                        frm.Show();
                    }
                }
            }
        }
        private void btnEdit_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            int iSelectedID = 0;
            if (rdoCategory.Checked == true)
            {
                iSelectedID = GetSelectedRowID("CategoryID");
                if (iSelectedID == 1)
                {
                    MessageBox.Show("Can't allow to Edit Default Category", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    frmItemCategory frmCat = new frmItemCategory(iSelectedID, true);
                    frmCat.MdiParent = this.MdiParent;
                    frmCat.Show();
                    frmCat.BringToFront();
                }
            }
            else if (rdoManufacturer.Checked == true)
            {
                iSelectedID = GetSelectedRowID("MnfID");
                if (iSelectedID == 1)
                {
                    MessageBox.Show("Can't allow to Edit Default Manufacture", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    frmManufacturer frmCat = new frmManufacturer(iSelectedID, true);
                    frmCat.MdiParent = this.MdiParent;
                    frmCat.Show();
                    frmCat.BringToFront();
                }
            }
            else if (rdoBrand.Checked == true)
            {
                iSelectedID = GetSelectedRowID("brandID");
                if (iSelectedID == 1)
                {
                    MessageBox.Show("Can't allow to Edit Default Brand", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    frmBrandMaster frmBrand = new frmBrandMaster(iSelectedID, true);
                    frmBrand.MdiParent = this.MdiParent;
                    frmBrand.Show();
                    frmBrand.BringToFront();
                    //GetDataAsperMenuClick("BRAND");
                }
            }
            else if (rdoColor.Checked == true)
            {
                iSelectedID = GetSelectedRowID("ColorID");
                if (iSelectedID == 1)
                {
                    MessageBox.Show("Can't allow to Edit Default Color", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    frmColorMaster frmColor = new frmColorMaster(iSelectedID, true);
                    frmColor.MdiParent = this.MdiParent;
                    frmColor.Show();
                    frmColor.BringToFront();
                    //GetDataAsperMenuClick("COLOR");
                }
            }
            else if (rdoDiscGroup.Checked == true)
            {
                iSelectedID = GetSelectedRowID("DiscountGroupID");
                if (iSelectedID == 1)
                {
                    MessageBox.Show("Can't allow to Edit Default Discpunt Group", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    frmDiscountGroup frmDiscount = new frmDiscountGroup(iSelectedID, true);
                    frmDiscount.MdiParent = this.MdiParent;
                    frmDiscount.Show();
                    frmDiscount.BringToFront();
                    //GetDataAsperMenuClick("DISCGROUP");
                }
            }
            else if (rdoUnit.Checked == true)
            {
                iSelectedID = GetSelectedRowID("UnitID");
                frmUnitMaster frmUnit = new frmUnitMaster(iSelectedID, true);
                frmUnit.MdiParent = this.MdiParent;
                frmUnit.Show();
                frmUnit.BringToFront();
                //GetDataAsperMenuClick("UNIT");
            }
            else if (rdoSize.Checked == true)
            {
                iSelectedID = GetSelectedRowID("SizeID");
                if (iSelectedID == 1)
                {
                    MessageBox.Show("Can't allow to Edit Default Size", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    FrmSizeMaster frmSize = new FrmSizeMaster(iSelectedID, true);
                    frmSize.MdiParent = this.MdiParent;
                    frmSize.Show();
                    frmSize.BringToFront();
                    //GetDataAsperMenuClick("SIZE");
                }
            }
            else if (rdoArea.Checked == true)
            {
                iSelectedID = GetSelectedRowID("AreaID");
                if (iSelectedID == 1)
                {
                    MessageBox.Show("Can't allow to Edit Default Area", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    frmAreaMaster frmArea = new frmAreaMaster(iSelectedID, true);
                    frmArea.MdiParent = this.MdiParent;
                    frmArea.Show();
                    frmArea.BringToFront();
                }
                //GetDataAsperMenuClick("AREA");
            }
            else if (rdoAgent.Checked == true)
            {
                iSelectedID = GetSelectedRowID("AgentID");
                if (iSelectedID == 1)
                {
                    MessageBox.Show("Can't allow to Edit Default Agent", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    frmAgentMaster frmagent = new frmAgentMaster(iSelectedID, true);
                    frmagent.MdiParent = this.MdiParent;
                    frmagent.Show();
                    frmagent.BringToFront();
                    // GetDataAsperMenuClick("AGENT");
                }
            }
            else if (rdoItemMaster.Checked == true)
            {
                iSelectedID = GetSelectedRowID("ItemID");
                frmItemMaster frmItm = new frmItemMaster(iSelectedID, true);
                frmItm.MdiParent = this.MdiParent;
                frmItm.Show();
                frmItm.BringToFront();
                //GetDataAsperMenuClick("ITEM");
            }
            else if (rdoSupplier.Checked == true)
            {
                iSelectedID = GetSelectedRowID("LID");
                frmLedger frmLed = new frmLedger(iSelectedID, true,0,"SUPPLIER");
                frmLed.MdiParent = this.MdiParent;
                frmLed.Show();
                frmLed.BringToFront();
            }
            else if (rdoCustomer.Checked == true)
            {
                iSelectedID = GetSelectedRowID("LID");
                frmLedger frmLed = new frmLedger(iSelectedID, true,0, "CUSTOMER");
                frmLed.MdiParent = this.MdiParent;
                frmLed.Show();
                frmLed.BringToFront();
            }
            else if (rdoLedger.Checked == true)
            {
                iSelectedID = GetSelectedRowID("LID");
                frmLedger frmLed = new frmLedger(iSelectedID, true);
                frmLed.MdiParent = this.MdiParent;
                frmLed.Show();
                frmLed.BringToFront();
                //GetDataAsperMenuClick("LEDGER");
            }
            else if (rdoTaxMode.Checked == true)
            {
                iSelectedID = GetSelectedRowID("TaxModeID");
                frmTaxMode frmTax = new frmTaxMode(iSelectedID, true);
                frmTax.MdiParent = this.MdiParent;
                frmTax.Show();
                frmTax.BringToFront();
                //GetDataAsperMenuClick("TAXMODE");
            }
            else if (rdoAccountGroup.Checked == true)
            {
                iSelectedID = GetSelectedRowID("AccountGroupID");
                frmAccountGroup frmAcc = new frmAccountGroup(iSelectedID, true);
                frmAcc.MdiParent = this.MdiParent;
                frmAcc.Show();
                frmAcc.BringToFront();
                //GetDataAsperMenuClick("ACCOUNTGROUP");
            }
            else if (rdoVoucherType.Checked == true)
            {
                iSelectedID = GetSelectedRowID("VchTypeID");
                frmVouchertype frm = new frmVouchertype(iSelectedID, false);
                frm.MdiParent = this.MdiParent;
                frm.Show();
                frm.BringToFront();
                //GetDataAsperMenuClick("VOUCHERTYP");
            }
            else if (rdoState.Checked == true)
            {
                iSelectedID = GetSelectedRowID("StateId");
                frmState frmState = new frmState(iSelectedID, true);
                frmState.MdiParent = this.MdiParent;
                frmState.Show();
                frmState.BringToFront();
                //GetDataAsperMenuClick("STATE");
            }
            else if (rdoCostCentre.Checked == true)
            {
                iSelectedID = GetSelectedRowID("CCID");
                frmCostCentre frmCCntr = new frmCostCentre(iSelectedID, true);
                frmCCntr.MdiParent = this.MdiParent;
                frmCCntr.Show();
                frmCCntr.BringToFront();
            }
            else if (rdoEmployee.Checked == true)
            {
                iSelectedID = GetSelectedRowID("EmpID");
                if (iSelectedID == 1)
                {
                    MessageBox.Show("Can't allow to Edit Default Employee", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    frmEmployee frmEmp = new frmEmployee(iSelectedID, true);
                    frmEmp.MdiParent = this.MdiParent;
                    frmEmp.Show();
                    frmEmp.BringToFront();
                }
            }
            else if (rdoStockDepartment.Checked == true)
            {
                iSelectedID = GetSelectedRowID("DepartmentID");
                if (iSelectedID == 1)
                {
                    MessageBox.Show("Can't allow to Edit Default Department", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    frmDepartment frmDep = new frmDepartment(iSelectedID, true, 0);
                    frmDep.MdiParent = this.MdiParent;
                    frmDep.Show();
                    frmDep.BringToFront();
                }
            }
            else if (rdoDepartment.Checked == true)
            {
                iSelectedID = GetSelectedRowID("DepartmentID");
                if (iSelectedID == 1)
                {
                    MessageBox.Show("Can't allow to Edit Default Department", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    frmDepartment frmDep = new frmDepartment(iSelectedID, true, 1);
                    frmDep.MdiParent = this.MdiParent;
                    frmDep.Show();
                    frmDep.BringToFront();
                }
            }
            else if (rdoCashDesk.Checked == true)
            {
                iSelectedID = GetSelectedRowID("PaymentID");
                if (iSelectedID <= 6)
                {
                    MessageBox.Show("Can't allow to Edit Default Cash Desk Masters", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    frmCashDeskMaster frmDep = new frmCashDeskMaster(iSelectedID, true);
                    frmDep.MdiParent = this.MdiParent;
                    frmDep.Show();
                    frmDep.BringToFront();
                }
            }

            ////User Management
            else if (rdoUserGroup.Checked == true)
            {
                iSelectedID = GetSelectedRowID("ID");
                frmUserGroup frmUserGp = new frmUserGroup(iSelectedID, true);
                frmUserGp.MdiParent = this.MdiParent;
                frmUserGp.Show();
                frmUserGp.BringToFront();
            }
            else if (rdoUser.Checked == true)
            {
                iSelectedID = GetSelectedRowID("UserID");
                frmUser frmUsr = new frmUser(iSelectedID, true);
                frmUsr.MdiParent = this.MdiParent;
                frmUsr.Show();
                frmUsr.BringToFront();
            }

            ////Transaction Treeview
            else
            {
                if (ITransParentID > 0)
                {
                    if (ITransParentID == 2) // PURCHASE
                    {
                        iSelectedID = GetSelectedRowID("InvId");
                        frmStockInVoucherNew frm = new frmStockInVoucherNew(itvwSelectedNodeID, iSelectedID, true, this.MdiParent);
                        frm.MdiParent = this.MdiParent;
                        //frm.WindowState = FormWindowState.Maximized;
                        frm.Show();
                        //GetDataAsperMenuClick("PURCHASE");
                    }
                    else if (ITransParentID == 4) // PURCHASE RETURN
                    {
                        iSelectedID = GetSelectedRowID("InvId");
                        frmStockInVoucherNew frm = new frmStockInVoucherNew(itvwSelectedNodeID, iSelectedID, true, this.MdiParent);
                        frm.MdiParent = this.MdiParent;
                        //frm.WindowState = FormWindowState.Maximized;
                        frm.Show();
                    }
                    else if (ITransParentID == 6) // RECEIPT NOTE
                    {
                        iSelectedID = GetSelectedRowID("InvId");
                        frmStockInVoucherNew frm = new frmStockInVoucherNew(itvwSelectedNodeID, iSelectedID, false, this.MdiParent);
                        frm.MdiParent = this.MdiParent;
                        //frm.WindowState = FormWindowState.Maximized;
                        frm.Show();
                    }
                    else if (ITransParentID == 1) // sales
                    {
                        iSelectedID = GetSelectedRowID("InvId");
                        frmStockOutVoucherNew frm = new frmStockOutVoucherNew(itvwSelectedNodeID, iSelectedID, true, this.MdiParent);
                        frm.MdiParent = this.MdiParent;
                        frm.Show();
                    }
                    else if (ITransParentID == 3) // sales return
                    {
                        iSelectedID = GetSelectedRowID("InvId");
                        frmStockOutVoucherNew frm = new frmStockOutVoucherNew(itvwSelectedNodeID, iSelectedID, true, this.MdiParent);
                        frm.MdiParent = this.MdiParent;
                        //frm.WindowState = FormWindowState.Maximized;
                        frm.Show();
                    }
                    else if (ITransParentID == 5) // delivery NOTE
                    {
                        iSelectedID = GetSelectedRowID("InvId");
                        frmStockOutVoucherNew frm = new frmStockOutVoucherNew(itvwSelectedNodeID, iSelectedID, false, this.MdiParent);
                        frm.MdiParent = this.MdiParent;
                        //frm.WindowState = FormWindowState.Maximized;
                        frm.Show();
                    }
                    else if (ITransParentID == 7) // receipt
                    {
                        iSelectedID = GetSelectedRowID("InvId");
                        frmReceipt frm = new frmReceipt(itvwSelectedNodeID, iSelectedID, false, this.MdiParent);
                        frm.MdiParent = this.MdiParent;
                        //frm.WindowState = FormWindowState.Maximized;
                        frm.Show();
                    }
                    else if (ITransParentID == 8) // payment 
                    {
                        iSelectedID = GetSelectedRowID("InvId");
                        frmReceipt frm = new frmReceipt(itvwSelectedNodeID, iSelectedID, false, this.MdiParent);
                        frm.MdiParent = this.MdiParent;
                        //frm.WindowState = FormWindowState.Maximized;
                        frm.Show();
                    }
                    else if (ITransParentID == 9) // contra
                    {
                        iSelectedID = GetSelectedRowID("InvId");
                        frmReceipt frm = new frmReceipt(itvwSelectedNodeID, iSelectedID, false, this.MdiParent);
                        frm.MdiParent = this.MdiParent;
                        //frm.WindowState = FormWindowState.Maximized;
                        frm.Show();
                    }
                    else if (ITransParentID == 10) // journal
                    {
                        iSelectedID = GetSelectedRowID("InvId");
                        frmReceipt frm = new frmReceipt(itvwSelectedNodeID, iSelectedID, false, this.MdiParent);
                        frm.MdiParent = this.MdiParent;
                        //frm.WindowState = FormWindowState.Maximized;
                        frm.Show();
                    }
                }
            }
            Cursor.Current = Cursors.Default;
        }
        private void btnDelete_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            int iAction = 2;
            if (rdoCategory.Checked == true)
            {
                if (GetSelectedRowID("CategoryID") > 5)
                {
                    DialogResult dlgResult1 = MessageBox.Show("Are you sure to delete category[" + GetSelectedRowData("CATEGORY") + "] Permanently ?", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                    if (dlgResult1.Equals(DialogResult.Yes))
                        if (dlgResult1 == DialogResult.Yes)
                        {
                            Categoryinfo.Category = "";
                            Categoryinfo.CatDiscPer = 0;
                            Categoryinfo.CategoryID = GetSelectedRowID("CategoryID");
                            Categoryinfo.HID = "";
                            Categoryinfo.LastUpdateDate = DateTime.Today;
                            Categoryinfo.LastUpdateTime = DateTime.Today;
                            Categoryinfo.ParentID = "";
                            Categoryinfo.Remarks = "";
                            Categoryinfo.SystemName = Environment.MachineName;
                            Categoryinfo.TenantId = Global.gblTenantID;
                            Categoryinfo.UserID = Global.gblUserID;

                            DataTable dtParent = clsCat.CheckParentIDExists(Categoryinfo.CategoryID, Global.gblTenantID);
                            ParentCount = Convert.ToInt32(dtParent.Rows[0]["ParentIDCount"]);
                            if (ParentCount == 0)
                            {
                                GetMaster.TYPE = "CATEGORY";
                                GetMaster.ID = Convert.ToInt32(Categoryinfo.CategoryID);
                                DataTable dtMaster = new DataTable();
                                dtMaster = clsMaster.GetColumnIDsData(GetMaster);
                                if (dtMaster.Rows.Count == 0)
                                {
                                    sRet = clsCat.InsertUpdateDeleteCategory(Categoryinfo, iAction);
                                    if (sRet.Length > 2)
                                    {
                                        strResult = sRet.Split('|');
                                        if (Convert.ToInt32(strResult[0].ToString()) == -1)
                                        {
                                            if (strResult[1].ToString().ToUpper().Contains("THE DELETE STATEMENT CONFLICTED WITH THE REFERENCE CONSTRAINT"))
                                                MessageBox.Show("Hey! There are Items Associated with this Category[" + GetSelectedRowData("CATEGORY") + "].Please Check.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                            else
                                                MessageBox.Show(strResult[1].ToString(), Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Question);

                                        }
                                        else
                                            RemoveGridRowAfterDelete();
                                    }
                                    else
                                        RemoveGridRowAfterDelete();

                                    //GetDataAsperMenuClick("CATEGORIES");

                                }
                                else
                                    MessageBox.Show("Can't allow to delete category [" + GetSelectedRowData("CATEGORY") + "] is  using in Item master", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {
                                MessageBox.Show("Can't allow to Delete Parent Category  (" + GetSelectedRowData("CATEGORY") + ")", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                }
                else
                    MessageBox.Show("Default Category [" + GetSelectedRowData("CATEGORY") + "] can't be deleted.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (rdoManufacturer.Checked == true)
            {
                if (GetSelectedRowID("MnfID") > 5)
                {
                    DialogResult dlgResult2 = MessageBox.Show("Are you sure to delete  manufacturer[" + GetSelectedRowData("Manufacturer") + "] Permanently ?", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                    if (dlgResult2.Equals(DialogResult.Yes))
                        if (dlgResult2 == DialogResult.Yes)
                        {
                            ManfInsert.MnfID = GetSelectedRowID("MnfID");
                            ManfInsert.MnfName = "";
                            ManfInsert.MnfShortName = "";
                            ManfInsert.DiscPer = 0;
                            ManfInsert.SystemName = Global.gblSystemName;
                            ManfInsert.UserID = Global.gblUserID;
                            ManfInsert.TenantID = Global.gblTenantID;
                            ManfInsert.LastUpdateDate = DateTime.Today;
                            ManfInsert.LastUpdateTime = DateTime.Now;
                            sRet = clsManf.InsertUpdateDeleteManufacturer(ManfInsert, iAction);
                            if (sRet.Length > 2)
                            {
                                strResult = sRet.Split('|');
                                if (Convert.ToInt32(strResult[0].ToString()) == -1)
                                {
                                    if (strResult[1].ToString().ToUpper().Contains("THE DELETE STATEMENT CONFLICTED WITH THE REFERENCE CONSTRAINT"))
                                        MessageBox.Show("Hey! There are Items Associated with this Manufacturer[" + GetSelectedRowData("Manufacturer") + "].Please Check.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    else
                                        MessageBox.Show(strResult[1].ToString(), Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Question);
                                }
                                else
                                    RemoveGridRowAfterDelete();
                            }
                            else
                                RemoveGridRowAfterDelete();
                            //GetDataAsperMenuClick("MANUFACTURER");
                        }
                }
                else
                    MessageBox.Show("Default manufacturer [" + GetSelectedRowData("Manufacturer") + "] can't be deleted.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            else if (rdoColor.Checked == true)
            {
                if (GetSelectedRowID("ColorID") > 5)
                {
                    DialogResult dlgResult3 = MessageBox.Show("Are you sure to delete  Color[" + GetSelectedRowData("Color") + "] Permanently ? ", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                    if (dlgResult3.Equals(DialogResult.Yes))
                        if (dlgResult3 == DialogResult.Yes)
                        {
                            ColorInsert.ColorID = GetSelectedRowID("ColorID");
                            ColorInsert.ColorName = "";
                            ColorInsert.ColorHexCode = "";
                            ColorInsert.TenantID = Global.gblTenantID;
                            GetMaster.TYPE = "COLOR";
                            GetMaster.ID = Convert.ToInt32(ColorInsert.ColorID);

                            dtgetData = clsMaster.GetColumnIDsData(GetMaster);
                            if (dtgetData.Rows.Count == 0)
                            {
                                sRet = clsColor.InsertUpdateDeleteColorMaster(ColorInsert, iAction);
                                if (sRet.Length > 2)
                                {
                                    strResult = sRet.Split('|');
                                    if (Convert.ToInt32(strResult[0].ToString()) == -1)
                                    {
                                        MessageBox.Show(strResult[1].ToString(), Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Question);
                                    }
                                    else
                                        RemoveGridRowAfterDelete();
                                }
                                else
                                    RemoveGridRowAfterDelete();
                                //GetDataAsperMenuClick("COLOR");
                            }
                            else
                            {
                                MessageBox.Show("Hey! There are Items Associated with this Color[" + GetSelectedRowData("Color") + "].Please Check.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                }
                else
                    MessageBox.Show("Default Color [" + GetSelectedRowData("Color") + "] can't be deleted.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (rdoSize.Checked == true)
            {
                if (GetSelectedRowID("SizeID") > 5)
                {
                    DialogResult dlgResult4 = MessageBox.Show("Are you sure to delete Size[" + GetSelectedRowData("Size") + "] Permanently ? ", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                    if (dlgResult4.Equals(DialogResult.Yes))
                        if (dlgResult4.Equals(DialogResult.Yes))
                            if (dlgResult4 == DialogResult.Yes)
                            {
                                SizeInsert.SizeID = GetSelectedRowID("SizeID");
                                SizeInsert.SizeName = "";
                                SizeInsert.SizeNameShort = "";
                                SizeInsert.SortOrder = 0;
                                SizeInsert.TenantID = Global.gblTenantID;
                                GetMaster.TYPE = "SIZE";
                                GetMaster.ID = Convert.ToInt32(SizeInsert.SizeID);
                                dtgetData = clsMaster.GetColumnIDsData(GetMaster);
                                if (dtgetData.Rows.Count == 0)
                                {
                                    sRet = clssize.InsertUpdateDeleteSizeMaster(SizeInsert, iAction);
                                    if (sRet.Length > 2)
                                    {
                                        strResult = sRet.Split('|');
                                        if (Convert.ToInt32(strResult[0].ToString()) == -1)
                                        {
                                            MessageBox.Show(strResult[1].ToString(), Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Question);
                                        }
                                        else
                                            RemoveGridRowAfterDelete();
                                    }
                                    else
                                        RemoveGridRowAfterDelete();
                                    // GetDataAsperMenuClick("SIZE");
                                }
                                else
                                    MessageBox.Show("Hey! There are Items Associated with this Size[" + GetSelectedRowData("Size") + "].Please Check.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                }
                else
                    MessageBox.Show("Default Size [" + GetSelectedRowData("Size") + "] can't be deleted.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            else if (rdoUnit.Checked == true)
            {
                if (GetSelectedRowID("UnitID") > 5)
                {
                    DialogResult dlgResult5 = MessageBox.Show("Are you sure to delete Unit[" + GetSelectedRowData("Unit") + "] Permanently ? ", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                    if (dlgResult5.Equals(DialogResult.Yes))
                        if (dlgResult5.Equals(DialogResult.Yes))
                            if (dlgResult5 == DialogResult.Yes)
                            {
                                UnitInsert.UnitID = GetSelectedRowID("UnitID");
                                UnitInsert.UnitName = "";
                                UnitInsert.UnitShortName = "";
                                UnitInsert.TenantID = Global.gblTenantID;
                                sRet = clsUnit.InsertUpdateDeleteUnitMaster(UnitInsert, iAction);
                                if (sRet.Length > 2)
                                {
                                    strResult = sRet.Split('|');
                                    if (Convert.ToInt32(strResult[0].ToString()) == -1)
                                    {
                                        if (strResult[1].ToString().ToUpper().Contains("THE DELETE STATEMENT CONFLICTED WITH THE REFERENCE CONSTRAINT"))
                                            MessageBox.Show("Hey! There are Items Associated with this Unit[" + GetSelectedRowData("Unit") + "].Please Check.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        else
                                            MessageBox.Show(strResult[1].ToString(), Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);

                                    }
                                    else
                                        RemoveGridRowAfterDelete();
                                }
                                else
                                    RemoveGridRowAfterDelete();
                                //GetDataAsperMenuClick("UNIT");
                            }
                }
                else
                    MessageBox.Show("Default Unit [" + GetSelectedRowData("Unit") + "] can't be deleted.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            else if (rdoDiscGroup.Checked == true)
            {
                if (GetSelectedRowID("DiscountGroupID") > 5)
                {
                    DialogResult dlgResult6 = MessageBox.Show("Are you sure to delete Discount Group[" + GetSelectedRowData("Discount Group") + "] Permanently ?", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                    if (dlgResult6.Equals(DialogResult.Yes))
                        if (dlgResult6 == DialogResult.Yes)
                        {
                            DiscountInsert.DiscountGroupID = GetSelectedRowID("DiscountGroupID");
                            DiscountInsert.DiscountGroupName = "";
                            DiscountInsert.DiscPer = 0;
                            DiscountInsert.SystemName = Global.gblSystemName;
                            DiscountInsert.UserID = Global.gblUserID;
                            DiscountInsert.TenantID = Global.gblTenantID;
                            DiscountInsert.LastUpdateDate = DateTime.Today;
                            DiscountInsert.LastUpdateTime = DateTime.Now;
                            sRet = clsDiscG.InsertUpdateDeleteDiscountGroup(DiscountInsert, iAction);

                            if (sRet.Length > 2)
                            {
                                strResult = sRet.Split('|');
                                if (Convert.ToInt32(strResult[0].ToString()) == -1)
                                {
                                    if (strResult[1].ToString().ToUpper().Contains("THE DELETE STATEMENT CONFLICTED WITH THE REFERENCE CONSTRAINT"))
                                        MessageBox.Show("Hey! There are Items Associated with this Discount Group[" + GetSelectedRowData("Discount Group") + "].Please Check.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    else
                                        MessageBox.Show(strResult[1].ToString(), Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);

                                }
                                else
                                    RemoveGridRowAfterDelete();
                            }
                            else
                                RemoveGridRowAfterDelete();
                            //GetDataAsperMenuClick("DISCGROUP");
                        }
                }
                else
                    MessageBox.Show("Default Discount Group [" + GetSelectedRowData("Discount Group") + "] can't be deleted.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            else if (rdoBrand.Checked == true)
            {
                if (GetSelectedRowID("brandID") > 5)
                {
                    DialogResult dlgResult7 = MessageBox.Show("Are you sure to delete Brand[" + GetSelectedRowData("Brand") + "] Permanently ?", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                    if (dlgResult7.Equals(DialogResult.Yes))
                        if (dlgResult7 == DialogResult.Yes)
                        {
                            BrandInsert.brandID = GetSelectedRowID("brandID");
                            BrandInsert.brandName = "";
                            BrandInsert.brandShortName = "";
                            BrandInsert.DiscPer = 0;
                            BrandInsert.SystemName = Global.gblSystemName;
                            BrandInsert.UserID = Global.gblUserID;
                            BrandInsert.TenantID = Global.gblTenantID;
                            BrandInsert.LastUpdateDate = DateTime.Today;
                            BrandInsert.LastUpdateTime = DateTime.Now;
                            sRet = clsBrand.InsertUpdateDeleteBrandMaster(BrandInsert, iAction);
                            if (sRet.Length > 2)
                            {
                                strResult = sRet.Split('|');
                                if (Convert.ToInt32(strResult[0].ToString()) == -1)
                                {
                                    if (strResult[1].ToString().ToUpper().Contains("THE DELETE STATEMENT CONFLICTED WITH THE REFERENCE CONSTRAINT"))
                                        MessageBox.Show("Hey! There are Items Associated with this Brand[" + GetSelectedRowData("Brand") + "].Please Check.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    //MessageBox.Show("Conflicted,  Hey! There are Items Associated with this BRand[].Please Check(" + GetSelectedRowData("Brand") + ")", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    else
                                        MessageBox.Show(strResult[1].ToString(), Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Question);
                                }
                                else
                                    RemoveGridRowAfterDelete();
                            }
                            else
                                RemoveGridRowAfterDelete();
                            //GetDataAsperMenuClick("BRAND");
                        }
                }
                else
                    MessageBox.Show("Default Brand [" + GetSelectedRowData("Brand") + "] can't be deleted.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            else if (rdoArea.Checked == true)
            {
                if (GetSelectedRowID("AreaID") > 5)
                {
                    DialogResult dlgResult8 = MessageBox.Show("Are you sure to delete Area[" + GetSelectedRowData("Area") + "] Permanently ?", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                    if (dlgResult8.Equals(DialogResult.Yes))
                    {
                        Areainsert.AreaID = GetSelectedRowID("AreaID");
                        Areainsert.Area = "";
                        Areainsert.Remarks = "";
                        Areainsert.ParentID = "0";
                        Areainsert.HID = "";
                        Areainsert.SystemName = Environment.MachineName;
                        Areainsert.UserID = Global.gblUserID;
                        Areainsert.LastUpdateDate = DateTime.Today;
                        Areainsert.LastUpdateTime = DateTime.Now;
                        Areainsert.TenantID = Global.gblTenantID;

                        DataTable dtParent = clsArea.CheckParentIDExists(Areainsert.AreaID, Areainsert.TenantID);
                        ParentCount = Convert.ToInt32(dtParent.Rows[0]["ParentIDCount"]);
                        if (ParentCount == 0)
                        {
                            clsArea.InsertUpdateDeleteAreaMaster(Areainsert, iAction);
                            if (sRet.Length > 2)
                            {
                                strResult = sRet.Split('|');
                                if (Convert.ToInt32(strResult[0].ToString()) == -1)
                                {
                                    if (strResult[1].ToString().ToUpper().Contains("THE DELETE STATEMENT CONFLICTED WITH THE REFERENCE CONSTRAINT"))
                                        MessageBox.Show("Hey! There are entries Associated with this Area[" + GetSelectedRowData("Area") + "].Please Check.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    else
                                        MessageBox.Show(strResult[1].ToString(), Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Question);
                                }
                                else
                                    RemoveGridRowAfterDelete();
                            }
                            else
                                RemoveGridRowAfterDelete();
                        }
                        else
                        {
                            MessageBox.Show("Can't allow to Delete the Parent Area (" + GetSelectedRowData("Area") + ")", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }

                    }
                }
                else
                    MessageBox.Show("Default Area [" + GetSelectedRowData("Area") + "] can't be deleted.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            else if (rdoAgent.Checked == true)
            {
                if (GetSelectedRowID("AgentID") > 5)
                {
                    DialogResult dlgResult9 = MessageBox.Show("Are you sure to delete  Agent[" + GetSelectedRowData("Agent Code") + "] Permanently ?", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                    if (dlgResult9.Equals(DialogResult.Yes))
                    {
                        Agentinsert.AgentID = GetSelectedRowID("AgentID");
                        Agentinsert.AgentCode = "";
                        Agentinsert.AgentName = "";
                        Agentinsert.Area = "";
                        Agentinsert.Commission = 0;
                        Agentinsert.blnPOstAccounts = 0;
                        Agentinsert.ADDRESS = "";
                        Agentinsert.LOCATION = "";
                        Agentinsert.PHONE = "";
                        Agentinsert.WEBSITE = "";
                        Agentinsert.EMAIL = "";
                        Agentinsert.BLNROOMRENT = 0;
                        Agentinsert.BLNSERVICES = 0;
                        Agentinsert.blnItemwiseCommission = 0;
                        Agentinsert.AgentDiscount = 0;
                        Agentinsert.LID = 0;
                        Agentinsert.SystemName = Environment.MachineName;
                        Agentinsert.UserID = Global.gblUserID;
                        Agentinsert.LastUpdateDate = DateTime.Today;
                        Agentinsert.LastUpdateTime = DateTime.Now;
                        Agentinsert.TenantID = Global.gblTenantID;
                        Agentinsert.AreaID = 0;
                        sRet = clsAgent.InsertUpdateDeleteAgentMaster(Agentinsert, iAction);
                        if (sRet.Length > 2)
                        {
                            strResult = sRet.Split('|');
                            if (Convert.ToInt32(strResult[0].ToString()) == -1)
                            {
                                if (strResult[1].ToString().ToUpper().Contains("THE DELETE STATEMENT CONFLICTED WITH THE REFERENCE CONSTRAINT"))
                                    MessageBox.Show("Hey! There are entries Associated with this Agent[" + GetSelectedRowData("Agent Code") + "].Please Check.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                //MessageBox.Show("Conflicted,  Hey! There are Items Associated with this BRand[].Please Check(" + GetSelectedRowData("Brand") + ")", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                else
                                    MessageBox.Show(strResult[1].ToString(), Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Question);
                            }
                            else
                                RemoveGridRowAfterDelete();
                        }
                        else
                            RemoveGridRowAfterDelete();
                        //GetDataAsperMenuClick("AGENT");
                    }
                }
                else
                    MessageBox.Show("Default Agent [" + GetSelectedRowData("Agent Code") + "] can't be deleted.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            else if (rdoItemMaster.Checked == true)
            {
                DialogResult dlgRsltItm = MessageBox.Show("Are you sure to delete Item[" + GetSelectedRowData("Item") + "] Permanently ?", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2);
                if (dlgRsltItm.Equals(DialogResult.Yes))
                {
                    int iActive = 0, iExpItem = 0, iIsIntNo = 0, iHSN = 0;
                    string sRet = "";
                    string[] strResult;
                    DataTable dtUspIt = new DataTable();

                    iAction = 2;
                    if (togglebtnActive.ToggleState == Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active)
                        iActive = 1;

                    iExpItem = 1;
                    iIsIntNo = 1;
                    itemInsertInfo.ItemID = GetSelectedRowID("ItemID");
                    itemInsertInfo.ItemCode = "";
                    itemInsertInfo.ItemName = "";
                    itemInsertInfo.CategoryID = 1;
                    itemInsertInfo.Description = "";
                    itemInsertInfo.PRate = 0;
                    itemInsertInfo.SrateCalcMode = 0;
                    itemInsertInfo.CRateAvg = 0;
                    itemInsertInfo.Srate1Per = 0;
                    itemInsertInfo.SRate1 = 0;
                    itemInsertInfo.Srate2Per = 0;
                    itemInsertInfo.SRate2 = 0;
                    itemInsertInfo.Srate3Per = 0;
                    itemInsertInfo.SRate3 = 0;
                    itemInsertInfo.Srate4 = 0;
                    itemInsertInfo.Srate4Per = 0;
                    itemInsertInfo.SRate5 = 0;
                    itemInsertInfo.Srate5Per = 0;
                    itemInsertInfo.MRP = 0;
                    itemInsertInfo.ROL = 0;
                    itemInsertInfo.Rack = "";
                    itemInsertInfo.Manufacturer = "";
                    itemInsertInfo.ActiveStatus = 1;
                    itemInsertInfo.IntLocal = 0;
                    itemInsertInfo.ProductType = "";
                    itemInsertInfo.ProductTypeID = 0;
                    itemInsertInfo.LedgerID = 0;
                    itemInsertInfo.UNITID = 0;
                    itemInsertInfo.Notes = "";
                    itemInsertInfo.agentCommPer = 0;
                    itemInsertInfo.BlnExpiryItem = iExpItem;
                    itemInsertInfo.Coolie = 0;
                    itemInsertInfo.FinishedGoodID = 0;
                    itemInsertInfo.MinRate = 0;
                    itemInsertInfo.MaxRate = 0;
                    itemInsertInfo.PLUNo = 0;
                    itemInsertInfo.HSNID = 0; //Convert.ToInt32(cboHSNCode.Text); Commented on 29-Sep-2021
                    itemInsertInfo.iCatDiscPer = 0;
                    itemInsertInfo.IPGDiscPer = 0;
                    itemInsertInfo.ImanDiscPer = 0;
                    itemInsertInfo.ItemNameUniCode = "";
                    itemInsertInfo.Minqty = 0;
                    itemInsertInfo.ItemCodeUniCode = "";
                    itemInsertInfo.UPC = "";
                    itemInsertInfo.BatchMode = "";
                    itemInsertInfo.Qty = 0;
                    itemInsertInfo.MaxQty = 0;
                    itemInsertInfo.IntNoOrWeight = iIsIntNo;
                    itemInsertInfo.SystemName = Environment.MachineName;
                    itemInsertInfo.UserID = Global.gblUserID;
                    itemInsertInfo.LastUpdateDate = DateTime.Today;
                    itemInsertInfo.LastUpdateTime = DateTime.Today;
                    itemInsertInfo.TenantID = Global.gblTenantID;
                    itemInsertInfo.blnCessOnTax = 0;
                    itemInsertInfo.CompCessQty = 0;
                    itemInsertInfo.CGSTTaxPer = 0;
                    itemInsertInfo.SGSTTaxPer = 0;
                    itemInsertInfo.IGSTTaxPer = 0;
                    itemInsertInfo.CessPer = 0;
                    itemInsertInfo.VAT = 0;

                    itemInsertInfo.CategoryIDs = "";
                    itemInsertInfo.ColorIDs = "";
                    itemInsertInfo.SizeIDs = "";
                    itemInsertInfo.BrandDisPer = 0;
                    itemInsertInfo.DGroupID = 0;
                    itemInsertInfo.DGroupDisPer = 0;
                    //itemInsertInfo.BatchCode = GetSelectedRowData("BatchCode");
                    itemInsertInfo.BatchCode = "";
                    itemInsertInfo.DiscPer = 0;
                    itemInsertInfo.CompCessQty = 0;
                    itemInsertInfo.DepartmentID = 0;
                    sRet = clsItem.InsertUpdateDeleteItemMasterInsert(itemInsertInfo, iAction);
                    if (sRet.Length > 2)
                    {
                        strResult = sRet.Split('|');
                        if (Convert.ToInt32(strResult[0].ToString().Trim()) == -1)
                        {
                            if (strResult[1].ToString().ToUpper().Contains("DUPLICATE"))
                                MessageBox.Show("Failed to Delete the Item Name (" + GetSelectedRowData("Item") + ")", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            else if (strResult[1].ToString().ToUpper().Contains("CONSTRAINT"))
                                MessageBox.Show("Failed to Delete the Item Name (" + GetSelectedRowData("Item") + "), It is referencing in someother Area !!", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            else
                                MessageBox.Show(strResult[1].ToString(), Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                            RemoveGridRowAfterDelete();
                    }
                    else
                    {
                        if (Convert.ToInt32(sRet) == -1)
                            MessageBox.Show("Failed to Delete! Contact your Administrator", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        else
                            RemoveGridRowAfterDelete();
                    }
                    //GetDataAsperMenuClick("ITEM");
                }
            }
            else if (rdoLedger.Checked == true)
            {
                if (GetSelectedRowID("LID") > 101)
                {
                    DialogResult dlgResult10 = MessageBox.Show("Are you sure to delete Ledger[" + GetSelectedRowData("Ledger Name") + "] Permanently ? ", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                    if (dlgResult10.Equals(DialogResult.Yes))
                    {
                        LedgerInsert.LID = GetSelectedRowID("LID");
                        LedgerInsert.LName = "";
                        LedgerInsert.LAliasName = "";
                        LedgerInsert.GroupName = "";
                        LedgerInsert.GSTType = "";
                        LedgerInsert.Address = "";
                        LedgerInsert.MobileNo = "";
                        LedgerInsert.Email = "";
                        LedgerInsert.StateID = 0;
                        LedgerInsert.TaxNo = "";
                        LedgerInsert.OpBalance = 0;
                        LedgerInsert.Type = "";
                        LedgerInsert.DiscPer = 0;
                        LedgerInsert.Area = "";
                        LedgerInsert.AreaID = 0;
                        LedgerInsert.AgentID = 0;
                        LedgerInsert.TenantID = Global.gblTenantID;
                        LedgerInsert.SystemName = Environment.MachineName;
                        LedgerInsert.UserID = Global.gblUserID;
                        LedgerInsert.LastUpdateDate = DateTime.Today;
                        LedgerInsert.LastUpdateTime = DateTime.Now;
                        LedgerInsert.TenantID = Global.gblTenantID;
                        LedgerInsert.EntryDate = DateTime.Today;
                        LedgerInsert.DOB = DateTime.Today;
                        LedgerInsert.DiscPer = 0;
                        LedgerInsert.PLID = 0;

                        sRet = clsLedger.InsertUpdateDeleteLedger(LedgerInsert, iAction);
                        if (sRet.Length > 2)
                        {
                            strResult = sRet.Split('|');
                            if (Convert.ToInt32(strResult[0].ToString()) == -1)
                            {
                                if (strResult[1].ToString().ToUpper().Contains("THE DELETE STATEMENT CONFLICTED WITH THE REFERENCE CONSTRAINT"))
                                    MessageBox.Show("Hey! There are entries Associated with this Ledger[" + GetSelectedRowData("Ledger Name") + "].Please Check.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                else
                                    MessageBox.Show(strResult[1].ToString(), Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Question);
                            }
                            else
                                RemoveGridRowAfterDelete();
                        }
                        else
                            RemoveGridRowAfterDelete();
                        //GetDataAsperMenuClick("LEDGER");
                    }
                }
                else
                    MessageBox.Show("Default Ledger [" + GetSelectedRowData("Ledger Name") + "] can't be deleted.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            else if (rdoSupplier.Checked == true)
            {
                if (GetSelectedRowID("LID") > 101)
                {
                    DialogResult dlgResult10 = MessageBox.Show("Are you sure to delete Supplier[" + GetSelectedRowData("Supplier Name") + "] Permanently ? ", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                    if (dlgResult10.Equals(DialogResult.Yes))
                    {
                        LedgerInsert.LID = GetSelectedRowID("LID");
                        LedgerInsert.LName = "";
                        LedgerInsert.LAliasName = "";
                        LedgerInsert.GroupName = "";
                        LedgerInsert.GSTType = "";
                        LedgerInsert.Address = "";
                        LedgerInsert.MobileNo = "";
                        LedgerInsert.Email = "";
                        LedgerInsert.StateID = 0;
                        LedgerInsert.TaxNo = "";
                        LedgerInsert.OpBalance = 0;
                        LedgerInsert.Type = "";
                        LedgerInsert.DiscPer = 0;
                        LedgerInsert.Area = "";
                        LedgerInsert.AreaID = 0;
                        LedgerInsert.AgentID = 0;
                        LedgerInsert.TenantID = Global.gblTenantID;
                        LedgerInsert.SystemName = Environment.MachineName;
                        LedgerInsert.UserID = Global.gblUserID;
                        LedgerInsert.LastUpdateDate = DateTime.Today;
                        LedgerInsert.LastUpdateTime = DateTime.Now;
                        LedgerInsert.TenantID = Global.gblTenantID;
                        LedgerInsert.EntryDate = DateTime.Today;
                        LedgerInsert.DOB = DateTime.Today;
                        LedgerInsert.DiscPer = 0;
                        LedgerInsert.PLID = 0;

                        sRet = clsLedger.InsertUpdateDeleteLedger(LedgerInsert, iAction);
                        if (sRet.Length > 2)
                        {
                            strResult = sRet.Split('|');
                            if (Convert.ToInt32(strResult[0].ToString()) == -1)
                            {
                                if (strResult[1].ToString().ToUpper().Contains("THE DELETE STATEMENT CONFLICTED WITH THE REFERENCE CONSTRAINT"))
                                    MessageBox.Show("Hey! There are entries Associated with this Supplier[" + GetSelectedRowData("Supplier Name") + "].Please Check.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                else
                                    MessageBox.Show(strResult[1].ToString(), Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Question);
                            }
                            else
                                RemoveGridRowAfterDelete();
                        }
                        else
                            RemoveGridRowAfterDelete();
                        //GetDataAsperMenuClick("LEDGER");
                    }
                }
                else
                    MessageBox.Show("Default Supplier [" + GetSelectedRowData("Supplier Name") + "] can't be deleted.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            else if (rdoCustomer.Checked == true)
            {
                if (GetSelectedRowID("LID") > 101)
                {
                    DialogResult dlgResult10 = MessageBox.Show("Are you sure to delete Customer[" + GetSelectedRowData("Customer Name") + "] Permanently ? ", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                    if (dlgResult10.Equals(DialogResult.Yes))
                    {
                        LedgerInsert.LID = GetSelectedRowID("LID");
                        LedgerInsert.LName = "";
                        LedgerInsert.LAliasName = "";
                        LedgerInsert.GroupName = "";
                        LedgerInsert.GSTType = "";
                        LedgerInsert.Address = "";
                        LedgerInsert.MobileNo = "";
                        LedgerInsert.Email = "";
                        LedgerInsert.StateID = 0;
                        LedgerInsert.TaxNo = "";
                        LedgerInsert.OpBalance = 0;
                        LedgerInsert.Type = "";
                        LedgerInsert.DiscPer = 0;
                        LedgerInsert.Area = "";
                        LedgerInsert.AreaID = 0;
                        LedgerInsert.AgentID = 0;
                        LedgerInsert.TenantID = Global.gblTenantID;
                        LedgerInsert.SystemName = Environment.MachineName;
                        LedgerInsert.UserID = Global.gblUserID;
                        LedgerInsert.LastUpdateDate = DateTime.Today;
                        LedgerInsert.LastUpdateTime = DateTime.Now;
                        LedgerInsert.TenantID = Global.gblTenantID;
                        LedgerInsert.EntryDate = DateTime.Today;
                        LedgerInsert.DOB = DateTime.Today;
                        LedgerInsert.DiscPer = 0;
                        LedgerInsert.PLID = 0;

                        sRet = clsLedger.InsertUpdateDeleteLedger(LedgerInsert, iAction);
                        if (sRet.Length > 2)
                        {
                            strResult = sRet.Split('|');
                            if (Convert.ToInt32(strResult[0].ToString()) == -1)
                            {
                                if (strResult[1].ToString().ToUpper().Contains("THE DELETE STATEMENT CONFLICTED WITH THE REFERENCE CONSTRAINT"))
                                    MessageBox.Show("Hey! There are entries Associated with this Customer[" + GetSelectedRowData("Customer Name") + "].Please Check.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                else
                                    MessageBox.Show(strResult[1].ToString(), Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Question);
                            }
                            else
                                RemoveGridRowAfterDelete();
                        }
                        else
                            RemoveGridRowAfterDelete();
                        //GetDataAsperMenuClick("LEDGER");
                    }
                }
                else
                    MessageBox.Show("Default Customer [" + GetSelectedRowData("Customer Name") + "] can't be deleted.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            else if (rdoTaxMode.Checked == true)
            {
                if (GetSelectedRowID("TaxModeID") > 5)
                {
                    DialogResult dlgResult11 = MessageBox.Show("Are you sure to delete Tax Mode[" + GetSelectedRowData("Tax Mode") + "] Permanently ? ", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                    if (dlgResult11.Equals(DialogResult.Yes))
                    {
                        TaxModeInsert.TaxModeID = GetSelectedRowID("TaxModeID");
                        TaxModeInsert.TaxMode = "";
                        TaxModeInsert.CalculationID = 0;
                        TaxModeInsert.SortNo = 0;
                        TaxModeInsert.SystemName = Environment.MachineName;
                        TaxModeInsert.UserID = Global.gblUserID;
                        TaxModeInsert.LastUpdateDate = DateTime.Today;
                        TaxModeInsert.LastUpdateTime = DateTime.Now;
                        TaxModeInsert.TenantID = Global.gblTenantID;
                        sRet = clsTax.InsertUpdateDeleteTaxMode(TaxModeInsert, iAction);
                        if (sRet.Length > 2)
                        {
                            strResult = sRet.Split('|');
                            if (Convert.ToInt32(strResult[0].ToString()) == -1)
                            {
                                MessageBox.Show(strResult[1].ToString(), Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Question);
                            }
                            else
                                RemoveGridRowAfterDelete();
                        }
                        else
                            RemoveGridRowAfterDelete();
                        //GetDataAsperMenuClick("TAXMODE");
                    }
                }
                else
                    MessageBox.Show("Default Tax Mode [" + GetSelectedRowData("Tax Mode") + "] can't be deleted.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            else if (rdoAccountGroup.Checked == true)
            {
                if (GetSelectedRowID("AccountGroupID") > 101)
                {
                    DialogResult dlgResult12 = MessageBox.Show("Are you sure to delete Account Group[" + GetSelectedRowData("Account Group") + "] Permanently ? ", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                    if (dlgResult12.Equals(DialogResult.Yes))
                    {
                        AccGroupInsert.AccountGroupID = GetSelectedRowID("AccountGroupID");
                        AccGroupInsert.AccountGroup = "";
                        AccGroupInsert.ParentID = 0;
                        AccGroupInsert.HID = "";
                        AccGroupInsert.Nature = "";
                        AccGroupInsert.SortOrder = 0;
                        AccGroupInsert.SystemName = Environment.MachineName;
                        AccGroupInsert.UserID = Global.gblUserID;
                        AccGroupInsert.LastUpdateDate = DateTime.Today;
                        AccGroupInsert.LastUpdateTime = DateTime.Now;
                        AccGroupInsert.TenantID = Global.gblTenantID;

                        GetAccGroupInfo.AccountGroupID = AccGroupInsert.AccountGroupID;
                        GetAccGroupInfo.TenantID = AccGroupInsert.TenantID;

                        DataTable dtParent = clsAccGroup.CheckParentIDExists(GetAccGroupInfo);
                        ParentCount = Convert.ToInt32(dtParent.Rows[0]["ParentIDCount"]);
                        if (ParentCount == 0)
                        {
                            sRet = clsAccGroup.InsertUpdateDeleteAccountGroup(AccGroupInsert, iAction);
                            if (sRet.Length > 2)
                            {
                                strResult = sRet.Split('|');
                                if (Convert.ToInt32(strResult[0].ToString()) == -1)
                                {
                                    MessageBox.Show(strResult[1].ToString(), Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Question);

                                }
                                else
                                    RemoveGridRowAfterDelete();
                            }
                            else
                                RemoveGridRowAfterDelete();
                            //GetDataAsperMenuClick("ACCOUNTGROUP");
                        }
                        else
                        {
                            MessageBox.Show("Can't allow to Delete the Parent Account Group (" + GetSelectedRowData("Account Group") + ")", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                else
                    MessageBox.Show("Default Account Group [" + GetSelectedRowData("Account Group") + "] can't be deleted.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (rdoState.Checked == true)
            {
                if (GetSelectedRowID("StateId") > 5)
                {
                    DialogResult dlgResult11 = MessageBox.Show("Are you sure to delete State Code[" + GetSelectedRowData("State Code") + "] Permanently ? ", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                    if (dlgResult11.Equals(DialogResult.Yes))
                    {
                        Stateinfo.StateId = GetSelectedRowID("StateId");
                        Stateinfo.StateCode = "";
                        Stateinfo.State = "";
                        Stateinfo.StateType = "";
                        Stateinfo.Country = "";
                        Stateinfo.CountryID = 0;
                        Stateinfo.SystemName = Environment.MachineName;
                        Stateinfo.UserID = Global.gblUserID;
                        Stateinfo.LastUpdateDate = DateTime.Today;
                        Stateinfo.LastUpdateTime = DateTime.Now;
                        Stateinfo.TenantID = Global.gblTenantID;
                        sRet = clsStat.InsertUpdateDeleteStates(Stateinfo, iAction);
                        if (sRet.Length > 2)
                        {
                            strResult = sRet.Split('|');
                            if (Convert.ToInt32(strResult[0].ToString()) == -1)
                            {
                                if (strResult[1].ToString().ToUpper().Contains("THE DELETE STATEMENT CONFLICTED WITH THE REFERENCE CONSTRAINT"))
                                    MessageBox.Show("Hey! There are entries Associated with this State Code[" + GetSelectedRowData("State Code") + "].Please Check.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                else
                                    MessageBox.Show(strResult[1].ToString(), Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Question);
                            }
                            else
                                RemoveGridRowAfterDelete();
                        }
                        else
                            RemoveGridRowAfterDelete();
                        //GetDataAsperMenuClick("STATE");
                    }
                }
                else
                    MessageBox.Show("Default State [" + GetSelectedRowData("State Code") + "] can't be deleted.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (rdoCostCentre.Checked == true)
            {
                if (GetSelectedRowID("CCID") > 5)
                {
                    DialogResult dlgResult11 = MessageBox.Show("Are you sure to delete Cost Centre[" + GetSelectedRowData("Cost Centre Name") + "] Permanently ? ", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                    if (dlgResult11.Equals(DialogResult.Yes))
                    {
                        CostCentrinfo.CCID = GetSelectedRowID("CCID");
                        CostCentrinfo.CCName = "";
                        CostCentrinfo.InCharge = "";
                        CostCentrinfo.Description1 = "";
                        CostCentrinfo.Description2 = "";
                        CostCentrinfo.Description3 = "";
                        CostCentrinfo.BLNDAMAGED = 0;
                        CostCentrinfo.SystemName = Environment.MachineName;
                        CostCentrinfo.UserID = Global.gblUserID;
                        CostCentrinfo.LastUpdateDate = DateTime.Today;
                        CostCentrinfo.LastUpdateTime = DateTime.Now;
                        CostCentrinfo.TenantID = Global.gblTenantID;

                        sRet = clsCostCtr.InsertUpdateDeleteCostCentre(CostCentrinfo, iAction);
                        if (sRet.Length > 2)
                        {
                            strResult = sRet.Split('|');
                            if (Convert.ToInt32(strResult[0].ToString()) == -1)
                            {
                                if (strResult[1].ToString().ToUpper().Contains("THE DELETE STATEMENT CONFLICTED WITH THE REFERENCE CONSTRAINT"))
                                    MessageBox.Show("Hey! There are entries Associated with this Cost Centre[" + GetSelectedRowData("Cost Centre Name") + "].Please Check.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                else
                                    MessageBox.Show(strResult[1].ToString(), Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Question);
                            }
                            else
                                RemoveGridRowAfterDelete();
                        }
                        else
                            RemoveGridRowAfterDelete();
                        //GetDataAsperMenuClick("COSTCENTRE");
                    }
                }
                else
                    MessageBox.Show("Default Cost Centre [" + GetSelectedRowData("Cost Centre Name") + "] can't be deleted.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            else if (rdoEmployee.Checked == true)
            {
                if (GetSelectedRowID("EmpID") > 5)
                {
                    DialogResult dlgResult11 = MessageBox.Show("Are you sure to delete Employee[" + GetSelectedRowData("Name") + "] Permanently ? ", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                    if (dlgResult11.Equals(DialogResult.Yes))
                    {
                        EmpInfo.EmpID = GetSelectedRowID("EmpID");
                        EmpInfo.Name = "";
                        EmpInfo.Address = "";
                        EmpInfo.NameOfFather = "";
                        EmpInfo.PhNo = "";
                        EmpInfo.MaritialStatus = "";
                        EmpInfo.NoOfFamilyMembers = "";
                        EmpInfo.NameOFNominee = "";
                        EmpInfo.Spouse = "";
                        EmpInfo.SpouseEmployed = false;
                        EmpInfo.OwnerOfResidence = false;
                        EmpInfo.PANNo = "";
                        EmpInfo.BloodGroup = "";
                        EmpInfo.Designation = "";
                        EmpInfo.Qualification = "";
                        EmpInfo.Sex = "";
                        EmpInfo.DOB = DateTime.Now;
                        EmpInfo.DOJ = DateTime.Now;
                        EmpInfo.DOI = DateTime.Now;
                        EmpInfo.PensionAccNo = "";
                        EmpInfo.GPFAccNo = "";
                        EmpInfo.GSLIAccNo = "";
                        EmpInfo.LICPolicyNo = "";
                        EmpInfo.LICMonthlyPremium = 0;
                        EmpInfo.LICDateofMaturity = DateTime.Now;
                        EmpInfo.CategoryID = 0;
                        EmpInfo.DateofPromotion = DateTime.Now;
                        EmpInfo.DateofRetirement = DateTime.Now;
                        EmpInfo.GISAccNo = "";
                        EmpInfo.BankAccNo = "";
                        EmpInfo.Commission = 0;
                        EmpInfo.CommissionAmt = 0;
                        EmpInfo.EmpFname = "";
                        EmpInfo.blnSalesStaff = 0;
                        EmpInfo.PhotoPath = "";
                        EmpInfo.InsCompany = "";
                        EmpInfo.CommissionCondition = 0;
                        EmpInfo.EmpCode = "";
                        EmpInfo.blnStatus = 0;
                        EmpInfo.DrivingLicenceNo = "";
                        EmpInfo.DrivingLicenceExpiry = DateTime.Now;
                        EmpInfo.PassportNo = "";
                        EmpInfo.PassportExpiry = DateTime.Now;
                        EmpInfo.Active = 0;
                        EmpInfo.SortOrder = 0;
                        EmpInfo.EnrollNo = 0;
                        EmpInfo.TargetAmount = 0;
                        EmpInfo.IncentivePer = 0;
                        EmpInfo.PWD = "";
                        EmpInfo.Holidays = "";
                        EmpInfo.LID = 0;
                        EmpInfo.salarypermonth = 0;
                        EmpInfo.SystemName = Environment.MachineName;
                        EmpInfo.UserID = Global.gblUserID;
                        EmpInfo.LastUpdateDate = DateTime.Today;
                        EmpInfo.LastUpdateTime = DateTime.Now;
                        EmpInfo.TenantID = Global.gblTenantID;

                        sRet = clsEmp.InsertUpdateDeleteEmployee(EmpInfo, iAction);
                        if (sRet.Length > 2)
                        {
                            strResult = sRet.Split('|');
                            if (Convert.ToInt32(strResult[0].ToString()) == -1)
                            {
                                if (strResult[1].ToString().ToUpper().Contains("THE DELETE STATEMENT CONFLICTED WITH THE REFERENCE CONSTRAINT"))
                                    MessageBox.Show("Hey! There are entries Associated with this Employee[" + GetSelectedRowData("Name") + "].Please Check.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                else
                                    MessageBox.Show(strResult[1].ToString(), Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Question);
                            }
                            else
                                RemoveGridRowAfterDelete();
                        }
                        else
                            RemoveGridRowAfterDelete();
                        //GetDataAsperMenuClick("EMPLOYEE");
                    }
                }
                else
                    MessageBox.Show("Default Employee [" + GetSelectedRowData("Name") + "] can't be deleted.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            else if (rdoStockDepartment.Checked == true)
            {
                if (GetSelectedRowID("DepartmentID") > 5)
                {
                    DialogResult dlgResult11 = MessageBox.Show("Are you sure to delete Department[" + GetSelectedRowData("Department") + "] Permanently ? ", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                    if (dlgResult11.Equals(DialogResult.Yes))
                    {
                        Departmentinfo.DepartmentID = GetSelectedRowID("DepartmentID");
                        Departmentinfo.Department = "";
                        Departmentinfo.Description = "";
                        Departmentinfo.SystemName = Environment.MachineName;
                        Departmentinfo.UserID = Global.gblUserID;
                        Departmentinfo.LastUpdateDate = DateTime.Today;
                        Departmentinfo.LastUpdateTime = DateTime.Now;
                        Departmentinfo.TenantID = Global.gblTenantID;
                        Departmentinfo.DepartmentType = 0;

                        sRet = clsDepart.InsertUpdateDeleteDepartment(Departmentinfo, iAction);
                        if (sRet.Length > 2)
                        {
                            strResult = sRet.Split('|');
                            if (Convert.ToInt32(strResult[0].ToString()) == -1)
                            {
                                if (strResult[1].ToString().ToUpper().Contains("THE DELETE STATEMENT CONFLICTED WITH THE REFERENCE CONSTRAINT"))
                                    MessageBox.Show("Hey! There are Items Associated with this Department[" + GetSelectedRowData("Department") + "] . Please Check.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                else
                                    MessageBox.Show(strResult[1].ToString(), Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Question);
                            }
                            else
                                RemoveGridRowAfterDelete();
                        }
                        else
                            RemoveGridRowAfterDelete();
                    }
                }
                else
                    MessageBox.Show("Default Department [" + GetSelectedRowData("Department") + "] can't be deleted.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            else if (rdoDepartment.Checked == true)
            {
                if (GetSelectedRowID("DepartmentID") > 5)
                {
                    DialogResult dlgResult11 = MessageBox.Show("Are you sure to delete Department[" + GetSelectedRowData("Department") + "] Permanently ? ", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                    if (dlgResult11.Equals(DialogResult.Yes))
                    {
                        Departmentinfo.DepartmentID = GetSelectedRowID("DepartmentID");
                        Departmentinfo.Department = "";
                        Departmentinfo.Description = "";
                        Departmentinfo.SystemName = Environment.MachineName;
                        Departmentinfo.UserID = Global.gblUserID;
                        Departmentinfo.LastUpdateDate = DateTime.Today;
                        Departmentinfo.LastUpdateTime = DateTime.Now;
                        Departmentinfo.TenantID = Global.gblTenantID;
                        Departmentinfo.DepartmentType = 1;

                        sRet = clsDepart.InsertUpdateDeleteDepartment(Departmentinfo, iAction);
                        if (sRet.Length > 2)
                        {
                            strResult = sRet.Split('|');
                            if (Convert.ToInt32(strResult[0].ToString()) == -1)
                            {
                                if (strResult[1].ToString().ToUpper().Contains("THE DELETE STATEMENT CONFLICTED WITH THE REFERENCE CONSTRAINT"))
                                    MessageBox.Show("Hey! There are Items Associated with this Department[" + GetSelectedRowData("Department") + "] . Please Check.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                else
                                    MessageBox.Show(strResult[1].ToString(), Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Question);
                            }
                            else
                                RemoveGridRowAfterDelete();
                        }
                        else
                            RemoveGridRowAfterDelete();
                    }
                }
                else
                    MessageBox.Show("Default Department [" + GetSelectedRowData("Department") + "] can't be deleted.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            else if (rdoCashDesk.Checked == true)
            {
                if (GetSelectedRowID("PaymentID") > 5)
                {
                    DialogResult dlgResult11 = MessageBox.Show("Are you sure to delete cash desk master [" + GetSelectedRowData("PaymentType") + "] Permanently ? ", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                    if (dlgResult11.Equals(DialogResult.Yes))
                    {
                        CashDeskinfo.PaymentID = GetSelectedRowID("PaymentID");
                        CashDeskinfo.PaymentType = "";
                        CashDeskinfo.LedgerID = 0;

                        sRet = clsCashDesk.InsertUpdateDeleteCashDeskMaster(CashDeskinfo, iAction);
                        if (sRet.Length > 2)
                        {
                            strResult = sRet.Split('|');
                            if (Convert.ToInt32(strResult[0].ToString()) == -1)
                            {
                                if (strResult[1].ToString().ToUpper().Contains("THE DELETE STATEMENT CONFLICTED WITH THE REFERENCE CONSTRAINT"))
                                    MessageBox.Show("Hey! There are Items Associated with this payment method[" + GetSelectedRowData("PaymentType") + "] . Please Check.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                else
                                    MessageBox.Show(strResult[1].ToString(), Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Question);
                            }
                            else
                                RemoveGridRowAfterDelete();
                        }
                        else
                            RemoveGridRowAfterDelete();
                    }
                }
                else
                    MessageBox.Show("Default cash desk master [" + GetSelectedRowData("PaymentType") + "] can't be deleted.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            else if (rdoUserGroup.Checked == true)
            {
                if (GetSelectedRowID("ID") > 5)
                {
                    DialogResult dlgResult11 = MessageBox.Show("Are you sure to delete User Group[" + GetSelectedRowData("GroupName") + "] Permanently ? ", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                    if (dlgResult11.Equals(DialogResult.Yes))
                    {
                        UserGroupInfo.ID = GetSelectedRowID("ID");
                        UserGroupInfo.GroupName = "";
                        UserGroupInfo.AccessLevel = "";
                        UserGroupInfo.StrCCID = "";
                        UserGroupInfo.RptAccesslevel = "";
                        UserGroupInfo.SystemName = Environment.MachineName;
                        UserGroupInfo.UserID = Global.gblUserID;
                        UserGroupInfo.LastUpdateDate = DateTime.Today;
                        UserGroupInfo.LastUpdateTime = DateTime.Now;
                        UserGroupInfo.TenantID = Global.gblTenantID;
                        UserGroupInfo.BillDisc = 0;
                        UserGroupInfo.ItemDisc = 0;
                        UserGroupInfo.CashDisc = 0;
                        sRet = clsuser.InsertUpdateDeleteUserGroup(UserGroupInfo, iAction);
                        if (sRet.Length > 2)
                        {
                            strResult = sRet.Split('|');
                            if (Convert.ToInt32(strResult[0].ToString()) == -1)
                            {
                                if (strResult[1].ToString().ToUpper().Contains("THE DELETE STATEMENT CONFLICTED WITH THE REFERENCE CONSTRAINT"))
                                    MessageBox.Show("Hey! There are entries Associated with this User Group[" + GetSelectedRowData("GroupName") + "] . Please Check.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                else
                                    MessageBox.Show(strResult[1].ToString(), Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Question);
                            }
                            else
                                RemoveGridRowAfterDelete();
                        }
                        else
                            RemoveGridRowAfterDelete();
                    }
                }
                else
                    MessageBox.Show("Default User Group [" + GetSelectedRowData("GroupName") + "] can't be deleted.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            else if (rdoUser.Checked == true)
            {
                if (GetSelectedRowID("UserID") > 5)
                {
                    DialogResult dlgResult11 = MessageBox.Show("Are you sure to delete User[" + GetSelectedRowData("User Name") + "] Permanently ? ", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                    if (dlgResult11.Equals(DialogResult.Yes))
                    {
                        UserInfo.UserID = GetSelectedRowID("UserID");
                        UserInfo.UserName = "";
                        UserInfo.Pwd = "";
                        UserInfo.GroupID = 0;
                        UserInfo.Status = 0;
                        UserInfo.changepwdonlogon = 0;
                        UserInfo.CostCentre = "";
                        UserInfo.HintAnswer = "";
                        UserInfo.HintQuestion = "";
                        UserInfo.WorkingDays = "0";
                        UserInfo.WorkFrom = Convert.ToDateTime("01-01-1900");
                        UserInfo.WorkTo = Convert.ToDateTime("01-01-1900");
                        UserInfo.godown = "";
                        UserInfo.SelectedCCID = 0;
                        UserInfo.SystemName = Environment.MachineName;
                        UserInfo.LastUpdateDate = DateTime.Today;
                        UserInfo.LastUpdateTime = DateTime.Now;
                        UserInfo.OrderVchtypeIDs = "";
                        UserInfo.SalesVchtypeIDs = "";
                        UserInfo.SalesReturnVchtypeIDs = "";
                        UserInfo.AccountsVchtypeIDs = "";
                        UserInfo.UserLedgerID = 0;
                        UserInfo.ActiveCounterID = 0;
                        UserInfo.PIN = 0;
                        UserInfo.CCIDs = "";
                        sRet = clsUsr.InsertUpdateDeleteUserMaster(UserInfo, iAction);
                        if (sRet.Length > 2)
                        {
                            strResult = sRet.Split('|');
                            if (Convert.ToInt32(strResult[0].ToString()) == -1)
                            {
                                MessageBox.Show(strResult[1].ToString(), Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Question);
                            }
                            else
                                RemoveGridRowAfterDelete();
                        }
                        else
                            RemoveGridRowAfterDelete();
                    }
                }
                else
                    MessageBox.Show("Default User[" + GetSelectedRowData("User Name") + "] can't be deleted.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            else
            {
                if (ITransParentID > 0)
                {
                    if (ITransParentID == 2) // PURCHASE
                    {
                        //DialogResult dlgResult12 = MessageBox.Show("Are you sure to delete Purchase Invoice No [" + GetSelectedRowData("Invoice No") + " ] Permanently ?", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                        //if (dlgResult12.Equals(DialogResult.Yes))
                        //{
                        //    if (dlgResult12 == DialogResult.Yes)
                        //    {
                          
                        int iSelectedID = GetSelectedRowID("InvId");
                                frmStockInVoucherNew frm = new frmStockInVoucherNew(itvwSelectedNodeID, iSelectedID, true, this.MdiParent);
                                frm.MdiParent = this.MdiParent;
                                frm.Show();
                        if (frm.DeleteVoucher() == true)
                        {
                            RemoveGridRowAfterDelete();
                        }

                                //PurchaseDelete(GetSelectedRowID("InvId"), itvwSelectedNodeID);
                                //}
                                //}
                    }
                    else if (ITransParentID == 1) // sales
                    {
                        int iSelectedID = GetSelectedRowID("InvId");
                        frmStockOutVoucherNew frm = new frmStockOutVoucherNew(itvwSelectedNodeID, iSelectedID, true, this.MdiParent);
                        frm.MdiParent = this.MdiParent;
                        frm.Show();
                        if (frm.DeleteVoucher() == true)
                        {
                            RemoveGridRowAfterDelete();
                        }
                    }
                }


                //MessageBox.Show("can't be deleted.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            Cursor.Current = Cursors.Default;
        }

        private void PurchaseDelete(decimal dTransID, decimal dVchTypeID)
        {
            string strJson = "";
            strJson = Comm.fnGetData("SELECT ISNULL(JsonData,'') as JsonData FROM tblPurchase WHERE InvId = " + dTransID + " AND VchTypeID = " + dVchTypeID + "").Tables[0].Rows[0][0].ToString();
            clsJSonPurchase clsPur = JsonConvert.DeserializeObject<clsJSonPurchase>(strJson);

            using (var sqlConn = Comm.GetDBConnection())
            {
                DataTable dbtl = clsPur.clsJsonPDetailsInfoList_.ToDataTable();
                using (SqlCommand sqlCmd = new SqlCommand("UspPurchaseItemInsert", sqlConn))
                {
                    sqlCmd.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter sqlDa = new SqlDataAdapter(sqlCmd);
                    DataSet dsDtl = new DataSet();
                    SqlParameter SpParam = new SqlParameter();

                    SpParam = sqlCmd.Parameters.Add("@InvID", SqlDbType.Decimal);
                    SpParam.Value = clsPur.clsJsonPMInfo_.InvId; //Convert.ToDecimal(dbtl.Rows[0]["InvID"].ToString());
                    SpParam = sqlCmd.Parameters.Add("@ItemId", SqlDbType.Decimal);
                    SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["ItemId"].ToString());
                    SpParam = sqlCmd.Parameters.Add("@Qty", SqlDbType.Decimal);
                    SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["Qty"].ToString());
                    SpParam = sqlCmd.Parameters.Add("@Rate", SqlDbType.Decimal);
                    SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["Rate"].ToString());
                    SpParam = sqlCmd.Parameters.Add("@UnitId", SqlDbType.Decimal);
                    SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["UnitId"].ToString());
                    SpParam = sqlCmd.Parameters.Add("@Batch", SqlDbType.VarChar);
                    SpParam.Value = dbtl.Rows[0]["Batch"].ToString();
                    SpParam = sqlCmd.Parameters.Add("@TaxPer", SqlDbType.Decimal);
                    SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["TaxPer"].ToString());
                    SpParam = sqlCmd.Parameters.Add("@TaxAmount", SqlDbType.Decimal);
                    SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["TaxAmount"].ToString());
                    SpParam = sqlCmd.Parameters.Add("@Discount", SqlDbType.Decimal);
                    SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["Discount"].ToString());
                    SpParam = sqlCmd.Parameters.Add("@MRP", SqlDbType.Decimal);
                    SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["MRP"].ToString());
                    SpParam = sqlCmd.Parameters.Add("@SlNo", SqlDbType.Decimal);
                    SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["SlNo"].ToString());
                    SpParam = sqlCmd.Parameters.Add("@Prate", SqlDbType.Decimal);
                    SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["Prate"].ToString());
                    SpParam = sqlCmd.Parameters.Add("@Free", SqlDbType.Decimal);
                    SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["Free"].ToString());
                    SpParam = sqlCmd.Parameters.Add("@SerialNos", SqlDbType.VarChar);
                    SpParam.Value = dbtl.Rows[0]["SerialNos"].ToString();
                    SpParam = sqlCmd.Parameters.Add("@ItemDiscount", SqlDbType.Decimal);
                    SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["ItemDiscount"].ToString());
                    SpParam = sqlCmd.Parameters.Add("@BatchCode", SqlDbType.VarChar);
                    SpParam.Value = dbtl.Rows[0]["BatchCode"].ToString();
                    SpParam = sqlCmd.Parameters.Add("@iCessOnTax", SqlDbType.Decimal);
                    SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["iCessOnTax"].ToString());
                    SpParam = sqlCmd.Parameters.Add("@blnCessOnTax", SqlDbType.Decimal);
                    SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["blnCessOnTax"].ToString());
                    SpParam = sqlCmd.Parameters.Add("@Expiry", SqlDbType.DateTime);
                    SpParam.Value = Convert.ToDateTime(dbtl.Rows[0]["Expiry"].ToString());
                    SpParam = sqlCmd.Parameters.Add("@ItemDiscountPer", SqlDbType.Decimal);
                    SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["ItemDiscountPer"].ToString());
                    SpParam = sqlCmd.Parameters.Add("@RateInclusive", SqlDbType.Decimal);
                    SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["RateInclusive"].ToString());
                    SpParam = sqlCmd.Parameters.Add("@ITaxableAmount", SqlDbType.Decimal);
                    SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["ITaxableAmount"].ToString());
                    SpParam = sqlCmd.Parameters.Add("@INetAmount", SqlDbType.Decimal);
                    SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["INetAmount"].ToString());
                    SpParam = sqlCmd.Parameters.Add("@CGSTTaxPer", SqlDbType.Decimal);
                    SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["CGSTTaxPer"].ToString());
                    SpParam = sqlCmd.Parameters.Add("@CGSTTaxAmt", SqlDbType.Decimal);
                    SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["CGSTTaxAmt"].ToString());
                    SpParam = sqlCmd.Parameters.Add("@SGSTTaxPer", SqlDbType.Decimal);
                    SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["SGSTTaxPer"].ToString());
                    SpParam = sqlCmd.Parameters.Add("@SGSTTaxAmt", SqlDbType.Decimal);
                    SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["SGSTTaxAmt"].ToString());
                    SpParam = sqlCmd.Parameters.Add("@IGSTTaxPer", SqlDbType.Decimal);
                    SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["IGSTTaxPer"].ToString());
                    SpParam = sqlCmd.Parameters.Add("@IGSTTaxAmt", SqlDbType.Decimal);
                    SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["IGSTTaxAmt"].ToString());
                    SpParam = sqlCmd.Parameters.Add("@iRateDiscPer", SqlDbType.Decimal);
                    SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["iRateDiscPer"].ToString());
                    SpParam = sqlCmd.Parameters.Add("@iRateDiscount", SqlDbType.Decimal);
                    SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["iRateDiscount"].ToString());
                    SpParam = sqlCmd.Parameters.Add("@BatchUnique", SqlDbType.VarChar);
                    SpParam.Value = dbtl.Rows[0]["BatchUnique"].ToString();
                    SpParam = sqlCmd.Parameters.Add("@blnQtyIN", SqlDbType.Decimal);
                    SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["blnQtyIN"].ToString());
                    SpParam = sqlCmd.Parameters.Add("@CRate", SqlDbType.Decimal);
                    SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["CRate"].ToString());
                    SpParam = sqlCmd.Parameters.Add("@Unit", SqlDbType.VarChar);
                    SpParam.Value = dbtl.Rows[0]["Unit"].ToString();
                    SpParam = sqlCmd.Parameters.Add("@ItemStockID", SqlDbType.Decimal);
                    SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["ItemStockID"].ToString());
                    SpParam = sqlCmd.Parameters.Add("@IcessPercent", SqlDbType.Decimal);
                    SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["IcessPercent"].ToString());
                    SpParam = sqlCmd.Parameters.Add("@IcessAmt", SqlDbType.Decimal);
                    SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["IcessAmt"].ToString());
                    SpParam = sqlCmd.Parameters.Add("@IQtyCompCessPer", SqlDbType.Decimal);
                    SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["IQtyCompCessPer"].ToString());
                    SpParam = sqlCmd.Parameters.Add("@IQtyCompCessAmt", SqlDbType.Decimal);
                    SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["IQtyCompCessAmt"].ToString());
                    SpParam = sqlCmd.Parameters.Add("@StockMRP", SqlDbType.Decimal);
                    SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["StockMRP"].ToString());
                    SpParam = sqlCmd.Parameters.Add("@BaseCRate", SqlDbType.Decimal);
                    SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["BaseCRate"].ToString());
                    SpParam = sqlCmd.Parameters.Add("@InonTaxableAmount", SqlDbType.Decimal);
                    SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["InonTaxableAmount"].ToString());
                    SpParam = sqlCmd.Parameters.Add("@IAgentCommPercent", SqlDbType.Decimal);
                    SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["IAgentCommPercent"].ToString());
                    SpParam = sqlCmd.Parameters.Add("@BlnDelete", SqlDbType.Decimal);
                    SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["BlnDelete"].ToString());
                    SpParam = sqlCmd.Parameters.Add("@Id", SqlDbType.Decimal);
                    SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["Id"].ToString());
                    SpParam = sqlCmd.Parameters.Add("@StrOfferDetails", SqlDbType.VarChar);
                    SpParam.Value = dbtl.Rows[0]["StrOfferDetails"].ToString();
                    SpParam = sqlCmd.Parameters.Add("@BlnOfferItem", SqlDbType.Decimal);
                    SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["BlnOfferItem"].ToString());
                    SpParam = sqlCmd.Parameters.Add("@BalQty", SqlDbType.Decimal);
                    SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["BalQty"].ToString());
                    SpParam = sqlCmd.Parameters.Add("@GrossAmount", SqlDbType.Decimal);
                    SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["GrossAmount"].ToString());
                    SpParam = sqlCmd.Parameters.Add("@iFloodCessPer", SqlDbType.Decimal);
                    SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["iFloodCessPer"].ToString());
                    SpParam = sqlCmd.Parameters.Add("@iFloodCessAmt", SqlDbType.Decimal);
                    SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["iFloodCessAmt"].ToString());
                    SpParam = sqlCmd.Parameters.Add("@Srate1", SqlDbType.Decimal);
                    SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["Srate1"].ToString());
                    SpParam = sqlCmd.Parameters.Add("@Srate2", SqlDbType.Decimal);
                    SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["Srate2"].ToString());
                    SpParam = sqlCmd.Parameters.Add("@Srate3", SqlDbType.Decimal);
                    SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["Srate3"].ToString());
                    SpParam = sqlCmd.Parameters.Add("@Srate4", SqlDbType.Decimal);
                    SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["Srate4"].ToString());
                    SpParam = sqlCmd.Parameters.Add("@Srate5", SqlDbType.Decimal);
                    SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["Srate5"].ToString());
                    SpParam = sqlCmd.Parameters.Add("@Costrate", SqlDbType.Decimal);
                    SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["Costrate"].ToString());
                    SpParam = sqlCmd.Parameters.Add("@CostValue", SqlDbType.Decimal);
                    SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["CostValue"].ToString());
                    SpParam = sqlCmd.Parameters.Add("@Profit", SqlDbType.Decimal);
                    SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["Profit"].ToString());
                    SpParam = sqlCmd.Parameters.Add("@ProfitPer", SqlDbType.Decimal);
                    SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["ProfitPer"].ToString());
                    SpParam = sqlCmd.Parameters.Add("@DiscMode", SqlDbType.Decimal);
                    SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["DiscMode"].ToString());
                    SpParam = sqlCmd.Parameters.Add("@Srate1Per", SqlDbType.Decimal);
                    SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["Srate1Per"].ToString());
                    SpParam = sqlCmd.Parameters.Add("@Srate2Per", SqlDbType.Decimal);
                    SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["Srate2Per"].ToString());
                    SpParam = sqlCmd.Parameters.Add("@Srate3Per", SqlDbType.Decimal);
                    SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["Srate3Per"].ToString());
                    SpParam = sqlCmd.Parameters.Add("@Srate4Per", SqlDbType.Decimal);
                    SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["Srate4Per"].ToString());
                    SpParam = sqlCmd.Parameters.Add("@Srate5Per", SqlDbType.Decimal);
                    SpParam.Value = Convert.ToDecimal(dbtl.Rows[0]["Srate5Per"].ToString());
                    SpParam = sqlCmd.Parameters.Add("@Action", SqlDbType.Int);
                    SpParam.Value = 2;

                    sqlDa.Fill(dsDtl);
                }

                using (SqlCommand sqlCmdM = new SqlCommand("UspPurchaseInsert", sqlConn))
                {
                    sqlCmdM.CommandType = CommandType.StoredProcedure;
                    SqlParameter SpParam = new SqlParameter();
                    SpParam = sqlCmdM.Parameters.Add("@InvId", SqlDbType.Decimal);
                    SpParam.Value = clsPur.clsJsonPMInfo_.InvId;
                    SpParam = sqlCmdM.Parameters.Add("@InvNo", SqlDbType.VarChar);
                    SpParam.Value = clsPur.clsJsonPMInfo_.InvNo;
                    SpParam = sqlCmdM.Parameters.Add("@AutoNum", SqlDbType.Decimal);
                    SpParam.Value = clsPur.clsJsonPMInfo_.AutoNum;
                    SpParam = sqlCmdM.Parameters.Add("@Prefix", SqlDbType.VarChar);
                    SpParam.Value = clsPur.clsJsonPMInfo_.Prefix;
                    SpParam = sqlCmdM.Parameters.Add("@InvDate", SqlDbType.DateTime);
                    SpParam.Value = clsPur.clsJsonPMInfo_.InvDate;
                    SpParam = sqlCmdM.Parameters.Add("@VchType", SqlDbType.VarChar);
                    SpParam.Value = clsPur.clsJsonPMInfo_.VchType;
                    SpParam = sqlCmdM.Parameters.Add("@MOP", SqlDbType.VarChar);
                    SpParam.Value = clsPur.clsJsonPMInfo_.MOP;
                    SpParam = sqlCmdM.Parameters.Add("@TaxModeID", SqlDbType.Decimal);
                    SpParam.Value = clsPur.clsJsonPMInfo_.TaxModeID;
                    SpParam = sqlCmdM.Parameters.Add("@LedgerId", SqlDbType.Decimal);
                    SpParam.Value = clsPur.clsJsonPMLedgerInfo_.LID;
                    SpParam = sqlCmdM.Parameters.Add("@Party", SqlDbType.VarChar);
                    SpParam.Value = clsPur.clsJsonPMLedgerInfo_.LName;
                    SpParam = sqlCmdM.Parameters.Add("@Discount", SqlDbType.Decimal);
                    SpParam.Value = clsPur.clsJsonPMInfo_.Discount;
                    SpParam = sqlCmdM.Parameters.Add("@TaxAmt", SqlDbType.Decimal);
                    SpParam.Value = clsPur.clsJsonPMInfo_.TaxAmt;
                    SpParam = sqlCmdM.Parameters.Add("@GrossAmt", SqlDbType.Decimal);
                    SpParam.Value = clsPur.clsJsonPMInfo_.GrossAmt;
                    SpParam = sqlCmdM.Parameters.Add("@BillAmt", SqlDbType.Decimal);
                    SpParam.Value = clsPur.clsJsonPMInfo_.BillAmt;
                    SpParam = sqlCmdM.Parameters.Add("@Cancelled", SqlDbType.Decimal);
                    SpParam.Value = clsPur.clsJsonPMInfo_.Cancelled;
                    SpParam = sqlCmdM.Parameters.Add("@OtherExpense", SqlDbType.Decimal);
                    SpParam.Value = clsPur.clsJsonPMInfo_.OtherExpense;
                    SpParam = sqlCmdM.Parameters.Add("@SalesManID", SqlDbType.Decimal);
                    SpParam.Value = clsPur.clsJsonPMEmployeeInfo_.EmpID;
                    SpParam = sqlCmdM.Parameters.Add("@Taxable", SqlDbType.Decimal);
                    SpParam.Value = clsPur.clsJsonPMInfo_.Taxable;
                    SpParam = sqlCmdM.Parameters.Add("@NonTaxable", SqlDbType.Decimal);
                    SpParam.Value = clsPur.clsJsonPMInfo_.NonTaxable;
                    SpParam = sqlCmdM.Parameters.Add("@ItemDiscountTotal", SqlDbType.Decimal);
                    SpParam.Value = clsPur.clsJsonPMInfo_.ItemDiscountTotal;
                    SpParam = sqlCmdM.Parameters.Add("@RoundOff", SqlDbType.Int);
                    SpParam.Value = clsPur.clsJsonPMInfo_.RoundOff;
                    SpParam = sqlCmdM.Parameters.Add("@UserNarration", SqlDbType.VarChar);
                    SpParam.Value = clsPur.clsJsonPMInfo_.UserNarration;
                    SpParam = sqlCmdM.Parameters.Add("@SortNumber", SqlDbType.Decimal);
                    SpParam.Value = clsPur.clsJsonPMInfo_.SortNumber;
                    SpParam = sqlCmdM.Parameters.Add("@DiscPer", SqlDbType.Decimal);
                    SpParam.Value = clsPur.clsJsonPMInfo_.DiscPer;
                    SpParam = sqlCmdM.Parameters.Add("@VchTypeID", SqlDbType.Decimal);
                    SpParam.Value = clsPur.clsJsonPMInfo_.VchTypeID;
                    SpParam = sqlCmdM.Parameters.Add("@CCID", SqlDbType.Decimal);
                    SpParam.Value = clsPur.clsJsonPMInfo_.CCID;
                    SpParam = sqlCmdM.Parameters.Add("@CurrencyID", SqlDbType.Decimal);
                    SpParam.Value = clsPur.clsJsonPMInfo_.CurrencyID;
                    SpParam = sqlCmdM.Parameters.Add("@PartyAddress", SqlDbType.VarChar);
                    SpParam.Value = clsPur.clsJsonPMLedgerInfo_.Address;
                    SpParam = sqlCmdM.Parameters.Add("@UserID", SqlDbType.Int);
                    SpParam.Value = clsPur.clsJsonPMInfo_.UserID;
                    SpParam = sqlCmdM.Parameters.Add("@AgentID", SqlDbType.Decimal);
                    SpParam.Value = clsPur.clsJsonPMInfo_.AgentID;
                    SpParam = sqlCmdM.Parameters.Add("@CashDiscount", SqlDbType.Decimal);
                    SpParam.Value = clsPur.clsJsonPMInfo_.CashDiscount;
                    SpParam = sqlCmdM.Parameters.Add("@DPerType_ManualCalc_Customer", SqlDbType.Decimal);
                    SpParam.Value = clsPur.clsJsonPMInfo_.DPerType_ManualCalc_Customer;
                    SpParam = sqlCmdM.Parameters.Add("@NetAmount", SqlDbType.Decimal);
                    SpParam.Value = clsPur.clsJsonPMInfo_.NetAmount;
                    SpParam = sqlCmdM.Parameters.Add("@RefNo", SqlDbType.VarChar);
                    SpParam.Value = clsPur.clsJsonPMInfo_.RefNo;
                    SpParam = sqlCmdM.Parameters.Add("@CashPaid", SqlDbType.Decimal);
                    SpParam.Value = clsPur.clsJsonPMInfo_.CashPaid;
                    SpParam = sqlCmdM.Parameters.Add("@CardPaid", SqlDbType.Decimal);
                    SpParam.Value = clsPur.clsJsonPMInfo_.CardPaid;
                    SpParam = sqlCmdM.Parameters.Add("@blnWaitforAuthorisation", SqlDbType.Decimal);
                    SpParam.Value = clsPur.clsJsonPMInfo_.blnWaitforAuthorisation;
                    SpParam = sqlCmdM.Parameters.Add("@UserIDAuth", SqlDbType.Decimal);
                    SpParam.Value = clsPur.clsJsonPMInfo_.UserIDAuth;
                    SpParam = sqlCmdM.Parameters.Add("@BillTime", SqlDbType.DateTime);
                    SpParam.Value = clsPur.clsJsonPMInfo_.BillTime;
                    SpParam = sqlCmdM.Parameters.Add("@StateID", SqlDbType.Decimal);
                    SpParam.Value = clsPur.clsJsonPMStateInfo_.StateId;
                    SpParam = sqlCmdM.Parameters.Add("@ImplementingStateCode", SqlDbType.VarChar);
                    SpParam.Value = clsPur.clsJsonPMInfo_.ImplementingStateCode;
                    SpParam = sqlCmdM.Parameters.Add("@GSTType", SqlDbType.VarChar);
                    SpParam.Value = clsPur.clsJsonPMLedgerInfo_.GSTType;
                    SpParam = sqlCmdM.Parameters.Add("@CGSTTotal", SqlDbType.Decimal);
                    SpParam.Value = clsPur.clsJsonPMInfo_.CGSTTotal;
                    SpParam = sqlCmdM.Parameters.Add("@SGSTTotal", SqlDbType.Decimal);
                    SpParam.Value = clsPur.clsJsonPMInfo_.SGSTTotal;
                    SpParam = sqlCmdM.Parameters.Add("@IGSTTotal", SqlDbType.Decimal);
                    SpParam.Value = clsPur.clsJsonPMInfo_.IGSTTotal;
                    SpParam = sqlCmdM.Parameters.Add("@PartyGSTIN", SqlDbType.VarChar);
                    SpParam.Value = clsPur.clsJsonPMLedgerInfo_.TaxNo;
                    SpParam = sqlCmdM.Parameters.Add("@BillType", SqlDbType.VarChar);
                    SpParam.Value = clsPur.clsJsonPMInfo_.BillType;
                    SpParam = sqlCmdM.Parameters.Add("@blnHold", SqlDbType.Decimal);
                    SpParam.Value = clsPur.clsJsonPMInfo_.blnHold;
                    SpParam = sqlCmdM.Parameters.Add("@PriceListID", SqlDbType.Decimal);
                    SpParam.Value = clsPur.clsJsonPMInfo_.PriceListID;
                    SpParam = sqlCmdM.Parameters.Add("@EffectiveDate", SqlDbType.DateTime);
                    SpParam.Value = clsPur.clsJsonPMInfo_.EffectiveDate;
                    SpParam = sqlCmdM.Parameters.Add("@partyCode", SqlDbType.VarChar);
                    SpParam.Value = clsPur.clsJsonPMLedgerInfo_.LAliasName;
                    SpParam = sqlCmdM.Parameters.Add("@MobileNo", SqlDbType.VarChar);
                    SpParam.Value = clsPur.clsJsonPMLedgerInfo_.MobileNo;
                    SpParam = sqlCmdM.Parameters.Add("@Email", SqlDbType.VarChar);
                    SpParam.Value = clsPur.clsJsonPMLedgerInfo_.Email;
                    SpParam = sqlCmdM.Parameters.Add("@TaxType", SqlDbType.VarChar);
                    SpParam.Value = clsPur.clsJsonPMInfo_.TaxType;
                    SpParam = sqlCmdM.Parameters.Add("@QtyTotal", SqlDbType.Decimal);
                    SpParam.Value = clsPur.clsJsonPMInfo_.QtyTotal;
                    SpParam = sqlCmdM.Parameters.Add("@DestCCID", SqlDbType.Decimal);
                    SpParam.Value = clsPur.clsJsonPMInfo_.DestCCID;
                    SpParam = sqlCmdM.Parameters.Add("@AgentCommMode", SqlDbType.VarChar);
                    SpParam.Value = clsPur.clsJsonPMInfo_.AgentCommMode;
                    SpParam = sqlCmdM.Parameters.Add("@AgentCommAmount", SqlDbType.Decimal);
                    SpParam.Value = clsPur.clsJsonPMInfo_.AgentCommAmount;
                    SpParam = sqlCmdM.Parameters.Add("@AgentLID", SqlDbType.Decimal);
                    SpParam.Value = clsPur.clsJsonPMInfo_.AgentLID;
                    SpParam = sqlCmdM.Parameters.Add("@BlnStockInsert", SqlDbType.Decimal);
                    SpParam.Value = clsPur.clsJsonPMInfo_.BlnStockInsert;
                    SpParam = sqlCmdM.Parameters.Add("@BlnConverted", SqlDbType.Decimal);
                    SpParam.Value = clsPur.clsJsonPMInfo_.BlnConverted;
                    SpParam = sqlCmdM.Parameters.Add("@ConvertedParentVchTypeID", SqlDbType.Decimal);
                    SpParam.Value = clsPur.clsJsonPMInfo_.ConvertedParentVchTypeID;
                    SpParam = sqlCmdM.Parameters.Add("@ConvertedVchTypeID", SqlDbType.Decimal);
                    SpParam.Value = clsPur.clsJsonPMInfo_.ConvertedVchTypeID;
                    SpParam = sqlCmdM.Parameters.Add("@ConvertedVchNo", SqlDbType.VarChar);
                    SpParam.Value = clsPur.clsJsonPMInfo_.ConvertedVchNo;
                    SpParam = sqlCmdM.Parameters.Add("@ConvertedVchID", SqlDbType.Decimal);
                    SpParam.Value = clsPur.clsJsonPMInfo_.ConvertedVchID;
                    SpParam = sqlCmdM.Parameters.Add("@DeliveryNoteDetails", SqlDbType.VarChar);
                    SpParam.Value = clsPur.clsJsonPMInfo_.DeliveryNoteDetails;
                    SpParam = sqlCmdM.Parameters.Add("@OrderDetails", SqlDbType.VarChar);
                    SpParam.Value = clsPur.clsJsonPMInfo_.OrderDetails;
                    SpParam = sqlCmdM.Parameters.Add("@IntegrityStatus", SqlDbType.VarChar);
                    SpParam.Value = clsPur.clsJsonPMInfo_.IntegrityStatus;
                    SpParam = sqlCmdM.Parameters.Add("@BalQty", SqlDbType.Decimal);
                    SpParam.Value = clsPur.clsJsonPMInfo_.BalQty;
                    SpParam = sqlCmdM.Parameters.Add("@CustomerpointsSettled", SqlDbType.Decimal);
                    SpParam.Value = clsPur.clsJsonPMInfo_.CustomerpointsSettled;
                    SpParam = sqlCmdM.Parameters.Add("@blnCashPaid", SqlDbType.Decimal);
                    SpParam.Value = clsPur.clsJsonPMInfo_.blnCashPaid;
                    SpParam = sqlCmdM.Parameters.Add("@originalsalesinvid", SqlDbType.Decimal);
                    SpParam.Value = clsPur.clsJsonPMInfo_.originalsalesinvid;
                    SpParam = sqlCmdM.Parameters.Add("@retuninvid", SqlDbType.Decimal);
                    SpParam.Value = clsPur.clsJsonPMInfo_.retuninvid;
                    SpParam = sqlCmdM.Parameters.Add("@returnamount", SqlDbType.Decimal);
                    SpParam.Value = clsPur.clsJsonPMInfo_.returnamount;
                    SpParam = sqlCmdM.Parameters.Add("@SystemName", SqlDbType.VarChar);
                    SpParam.Value = clsPur.clsJsonPMInfo_.SystemName;
                    SpParam = sqlCmdM.Parameters.Add("@LastUpdateDate", SqlDbType.DateTime);
                    SpParam.Value = clsPur.clsJsonPMInfo_.LastUpdateDate;
                    SpParam = sqlCmdM.Parameters.Add("@LastUpdateTime", SqlDbType.DateTime);
                    SpParam.Value = clsPur.clsJsonPMInfo_.LastUpdateTime;
                    SpParam = sqlCmdM.Parameters.Add("@DeliveryDetails", SqlDbType.VarChar);
                    SpParam.Value = clsPur.clsJsonPMInfo_.DeliveryDetails;
                    SpParam = sqlCmdM.Parameters.Add("@DespatchDetails", SqlDbType.VarChar);
                    SpParam.Value = clsPur.clsJsonPMInfo_.DespatchDetails;
                    SpParam = sqlCmdM.Parameters.Add("@TermsOfDelivery", SqlDbType.VarChar);
                    SpParam.Value = clsPur.clsJsonPMInfo_.TermsOfDelivery;
                    SpParam = sqlCmdM.Parameters.Add("@FloodCessTot", SqlDbType.Decimal);
                    SpParam.Value = clsPur.clsJsonPMInfo_.FloodCessTot;
                    SpParam = sqlCmdM.Parameters.Add("@CounterID", SqlDbType.Decimal);
                    SpParam.Value = clsPur.clsJsonPMInfo_.CounterID;
                    SpParam = sqlCmdM.Parameters.Add("@ExtraCharges", SqlDbType.Decimal);
                    SpParam.Value = clsPur.clsJsonPMInfo_.ExtraCharges;
                    SpParam = sqlCmdM.Parameters.Add("@ReferenceAutoNO", SqlDbType.Decimal);
                    SpParam.Value = clsPur.clsJsonPMInfo_.ReferenceAutoNO;
                    SpParam = sqlCmdM.Parameters.Add("@CashDiscPer", SqlDbType.Decimal);
                    SpParam.Value = clsPur.clsJsonPMInfo_.CashDisPer;
                    SpParam = sqlCmdM.Parameters.Add("@CostFactor", SqlDbType.Decimal);
                    SpParam.Value = clsPur.clsJsonPMInfo_.CostFactor;
                    SpParam = sqlCmdM.Parameters.Add("@TenantID", SqlDbType.Decimal);
                    SpParam.Value = clsPur.clsJsonPMInfo_.TenantID;
                    SpParam = sqlCmdM.Parameters.Add("@JsonData", SqlDbType.VarChar);
                    SpParam.Value = strJson;
                    SpParam = sqlCmdM.Parameters.Add("@Action", SqlDbType.Int);
                    SpParam.Value = 2;
                    SqlDataAdapter sqlDa = new SqlDataAdapter(sqlCmdM);
                    DataSet dsCommon = new DataSet();
                    sqlDa.Fill(dsCommon);

                    string sResult = "";
                    DataTable dtResult = dsCommon.Tables[0];
                    if (dtResult.Rows.Count > 0)
                        sResult = dtResult.Rows[0]["SqlSpResult"].ToString();

                    if (Convert.ToInt32(sResult) == -1)
                    {
                        sResult = sResult + " | " + dtResult.Rows[0]["ErrorMessage"].ToString();
                        Comm.WritetoSqlErrorLog(dtResult, Global.gblUserName);
                        MessageBox.Show(sResult);
                    }

                }
            }


        }

        private void btnCancelDeactive_Click(object sender, EventArgs e)
        {
            int iAction;
            if (rdoItemMaster.Checked == true)
            {
                DialogResult dlgRsltItm = MessageBox.Show("Are you sure to Cancel the Item[" + GetSelectedRowData("Item") + "] ?", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2);
                if (dlgRsltItm.Equals(DialogResult.Yes))
                {
                    int iActive = 0, iExpItem = 0, iIsIntNo = 0, iHSN = 0;
                    string sRet = "";
                    string[] strResult;
                    DataTable dtUspIt = new DataTable();
                    iAction = 1;
                    iExpItem = 1;
                    iIsIntNo = 1;

                    itemInsertInfo.ItemID = GetSelectedRowID("ItemID");
                    itemInsertInfo.ItemCode = "";
                    itemInsertInfo.ItemName = "";
                    itemInsertInfo.CategoryID = 1;
                    itemInsertInfo.Description = "";
                    itemInsertInfo.PRate = 0;
                    itemInsertInfo.SrateCalcMode = 0;
                    itemInsertInfo.CRateAvg = 0;
                    itemInsertInfo.Srate1Per = 0;
                    itemInsertInfo.SRate1 = 0;
                    itemInsertInfo.Srate2Per = 0;
                    itemInsertInfo.SRate2 = 0;
                    itemInsertInfo.Srate3Per = 0;
                    itemInsertInfo.SRate3 = 0;
                    itemInsertInfo.Srate4 = 0;
                    itemInsertInfo.Srate4Per = 0;
                    itemInsertInfo.SRate5 = 0;
                    itemInsertInfo.Srate5Per = 0;
                    itemInsertInfo.MRP = 0;
                    itemInsertInfo.ROL = 0;
                    itemInsertInfo.Rack = "";
                    itemInsertInfo.Manufacturer = "";
                    itemInsertInfo.ActiveStatus = 0;
                    itemInsertInfo.IntLocal = 0;
                    itemInsertInfo.ProductType = "";
                    itemInsertInfo.ProductTypeID = 0;
                    itemInsertInfo.LedgerID = 0;
                    itemInsertInfo.UNITID = 1;
                    itemInsertInfo.Notes = "";
                    itemInsertInfo.agentCommPer = 0;
                    itemInsertInfo.BlnExpiryItem = iExpItem;
                    itemInsertInfo.Coolie = 0;
                    itemInsertInfo.FinishedGoodID = 0;
                    itemInsertInfo.MinRate = 0;
                    itemInsertInfo.MaxRate = 0;
                    itemInsertInfo.PLUNo = 0;
                    itemInsertInfo.HSNID = 0; //Convert.ToInt32(cboHSNCode.Text); Commented on 29-Sep-2021
                    itemInsertInfo.iCatDiscPer = 0;
                    itemInsertInfo.IPGDiscPer = 0;
                    itemInsertInfo.ImanDiscPer = 0;
                    itemInsertInfo.ItemNameUniCode = "";
                    itemInsertInfo.Minqty = 0;
                    itemInsertInfo.MNFID = 1;
                    itemInsertInfo.ItemCodeUniCode = "";
                    itemInsertInfo.UPC = "";
                    itemInsertInfo.BatchMode = "";
                    itemInsertInfo.Qty = 0;
                    itemInsertInfo.MaxQty = 0;
                    itemInsertInfo.IntNoOrWeight = iIsIntNo;
                    itemInsertInfo.SystemName = Environment.MachineName;
                    itemInsertInfo.UserID = Global.gblUserID;
                    itemInsertInfo.LastUpdateDate = DateTime.Today;
                    itemInsertInfo.LastUpdateTime = DateTime.Today;
                    itemInsertInfo.TenantID = Global.gblTenantID;
                    itemInsertInfo.blnCessOnTax = 0;
                    itemInsertInfo.CompCessQty = 0;
                    itemInsertInfo.CGSTTaxPer = 0;
                    itemInsertInfo.SGSTTaxPer = 0;
                    itemInsertInfo.IGSTTaxPer = 0;
                    itemInsertInfo.CessPer = 0;
                    itemInsertInfo.VAT = 0;
                    itemInsertInfo.CategoryIDs = "";
                    itemInsertInfo.ColorIDs = "";
                    itemInsertInfo.SizeIDs = "";
                    itemInsertInfo.BrandDisPer = 0;
                    itemInsertInfo.DGroupID = 1;
                    itemInsertInfo.DGroupDisPer = 0;
                    itemInsertInfo.BatchCode = GetSelectedRowData("BatchCode");
                    itemInsertInfo.BrandID = 1;
                    sRet = clsItem.InsertUpdateDeleteItemMasterInsert(itemInsertInfo, iAction);

                    if (sRet.Length > 2)
                    {
                        strResult = sRet.Split('|');
                        if (Convert.ToInt32(strResult[0].ToString().Trim()) == -1)
                        {
                            if (strResult[1].ToString().ToUpper().Contains("DUPLICATE"))
                                MessageBox.Show("Failed to Delete the Item Name (" + GetSelectedRowData("Item") + ")", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            else if (strResult[1].ToString().ToUpper().Contains("CONSTRAINT"))
                                MessageBox.Show("Failed to Delete the Item Name (" + GetSelectedRowData("Item") + "), It is referencing in someother Area !!", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            else
                                MessageBox.Show(strResult[1].ToString(), Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        if (Convert.ToInt32(sRet) == -1)
                            MessageBox.Show("Failed to Delete! Contact your Administrator", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    GetDataAsperMenuClick("ITEM");
                }
            }
        }
        private void frmEditWindow_Resize(object sender, EventArgs e)
        {
            //rdoCategory.PerformClick();
        }

        private void trvwParentTransction_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            try
            {
                NewClick();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
            }
        }
        private void rdoCategory_DoubleClick(object sender, EventArgs e)
        {
            NewClick();
        }
        private void rdoManufacturer_DoubleClick(object sender, EventArgs e)
        {
            NewClick();
        }
        private void rdoBrand_DoubleClick(object sender, EventArgs e)
        {
            NewClick();
        }
        private void rdoDiscGroup_DoubleClick(object sender, EventArgs e)
        {
            NewClick();
        }
        private void rdoSize_DoubleClick(object sender, EventArgs e)
        {
            NewClick();
        }
        private void rdoColor_DoubleClick(object sender, EventArgs e)
        {
            NewClick();
        }
        private void rdoUnit_DoubleClick(object sender, EventArgs e)
        {
            NewClick();
        }
        private void rdoItemMaster_DoubleClick(object sender, EventArgs e)
        {
            NewClick();
        }
        private void rdoArea_DoubleClick(object sender, EventArgs e)
        {
            NewClick();
        }
        private void rdoAgent_DoubleClick(object sender, EventArgs e)
        {
            NewClick();
        }
        private void rdoSupplier_DoubleClick(object sender, EventArgs e)
        {

            NewClick();
        }
        private void rdoCustomer_DoubleClick(object sender, EventArgs e)
        {
            NewClick();
        }
        private void rdoLedger_DoubleClick(object sender, EventArgs e)
        {
            NewClick();
        }
        private void rdoTaxMode_DoubleClick(object sender, EventArgs e)
        {
            NewClick();
        }
        private void rdoAccountGroup_DoubleClick(object sender, EventArgs e)
        {
            NewClick();
        }
        private void rdoVoucherType_DoubleClick(object sender, EventArgs e)
        {
            NewClick();
        }
        private void rdoSettings_DoubleClick(object sender, EventArgs e)
        {
            NewClick();
        }
        private void rdoState_DoubleClick(object sender, EventArgs e)
        {
            NewClick();
        }
        private void rdoCostCentre_DoubleClick(object sender, EventArgs e)
        {
            NewClick();
        }
        private void rdoEmployee_DoubleClick(object sender, EventArgs e)
        {
            NewClick();
        }
        private void rdoDepartment_DoubleClick(object sender, EventArgs e)
        {
            NewClick();
        }
        private void rdoCashDesk_DoubleClick(object sender, EventArgs e)
        {
            NewClick();
        }
        private void rdoStockDepartment_DoubleClick(object sender, EventArgs e)
        {
            NewClick();
        }
        private void rdoStockAnalysis_DoubleClick(object sender, EventArgs e)
        {
            NewClick();
        }
        private void rdoUserGroup_DoubleClick(object sender, EventArgs e)
        {
            NewClick();
        }
        private void rdoUser_DoubleClick(object sender, EventArgs e)
        {
            NewClick();
        }
        private void rdoPurchaseReport_DoubleClick(object sender, EventArgs e)
        {
            NewClick();
        }
        private void rdoCategory_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                rdoCategory.Focus();
                NewClick();
            }
        }

        private void rdoManufacturer_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                rdoManufacturer.Focus();
                NewClick();
            }
        }

        private void rdoBrand_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                NewClick();
            }
        }

        private void rdoDiscGroup_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                NewClick();
            }
        }

        private void rdoSize_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                NewClick();
            }
        }

        private void rdoColor_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                NewClick();
            }
        }

        private void rdoCashDesk_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                NewClick();
            }
        }

        private void rdoUnit_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                NewClick();
            }
        }

        private void rdoItemMaster_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                NewClick();
            }
        }

        private void rdoArea_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                NewClick();
            }
        }

        private void rdoAgent_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                NewClick();
            }
        }

        private void rdoSupplier_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                NewClick();
            }
        }

        private void rdoCustomer_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                NewClick();
            }
        }

        private void rdoLedger_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                NewClick();
            }
        }

        private void rdoTaxMode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                NewClick();
            }
        }

        private void rdoAccountGroup_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                NewClick();
            }
        }

        private void rdoVoucherType_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                NewClick();
            }
        }

        private void rdoSettings_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                NewClick();
            }
        }

        private void rdoState_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                NewClick();
            }
        }

        private void rdoCostCentre_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                NewClick();
            }
        }

        private void rdoEmployee_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                NewClick();
            }
        }

        private void rdoDepartment_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                NewClick();
            }
        }

        private void rdoStockAnalysis_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                NewClick();
            }
        }

        private void rdoUserGroup_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                NewClick();
            }
        }

        private void rdoUser_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                NewClick();
            }
        }

        private void rdoPurchaseReport_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                NewClick();
            }
        }
        #endregion

        #region "METHODS ---------------------------------------------- >>"
        //Description : Get Data when Click on Menu
        private void GetDataAsperMenuClick(string sMenuType = "")
        {
            DataTable dtResult = new DataTable();
            //strFormHeaderName = sMenuType.ToUpper();

            if (strFormHeaderName == "")
                strFormHeaderName = sMenuType;

            if (sMenuType.ToUpper() == "CATEGORIES")
            {
                Catinfo.CategoryID = 0;
                Catinfo.TenantId = Global.gblTenantID;
                dtResult = EdtComm.GetCategories(Catinfo);
                strGtidColumnSize = "0,-1,410,100,300";
            }
            else if (sMenuType.ToUpper() == "MANUFACTURER")
            {
                Manfinfo.MnfID = 0;
                Manfinfo.TenantID = Global.gblTenantID;
                dtResult = EdtComm.GetManufacturer(Manfinfo);
                strGtidColumnSize = "0,-1,250,100";
            }
            else if (sMenuType.ToUpper() == "BRAND")
            {
                GetBrandInfo.brandID = 0;
                GetBrandInfo.TenantID = Global.gblTenantID;
                dtResult = clsBrand.GetBrandMaster(GetBrandInfo);
                strGtidColumnSize = "0,-1,250,100";
            }
            else if (sMenuType.ToUpper() == "COLOR")
            {
                GetcolorInfo.ColorID = 0;
                GetcolorInfo.TenantID = Global.gblTenantID;
                dtResult = clsColor.GetColorMaster(GetcolorInfo);
                strGtidColumnSize = "0,-1,250";
            }
            else if (sMenuType.ToUpper() == "CASHDESK")
            {
                GetCashDeskinfo.PaymentID = 0;
                GetCashDeskinfo.Paymentids = "";
                dtResult = clsCashDesk.GetCashDeskMaster(GetCashDeskinfo);
                strGtidColumnSize = "0,-1";
            }
            else if (sMenuType.ToUpper() == "SIZE")
            {
                Getsizeinfo.SizeID = 0;
                Getsizeinfo.TenantID = Global.gblTenantID;
                dtResult = clssize.GetSizeMaster(Getsizeinfo);
                strGtidColumnSize = "0,-1,240,100";
            }
            else if (sMenuType.ToUpper() == "DISCGROUP")
            {
                GetDiscGinfo.DiscountGroupID = 0;
                GetDiscGinfo.TenantID = Global.gblTenantID;
                dtResult = clsDiscG.GetDiscountGroup(GetDiscGinfo);
                strGtidColumnSize = "0,-1,100";
            }
            else if (sMenuType.ToUpper() == "UNIT")
            {
                GetUnitInfo.UnitID = 0;
                GetUnitInfo.TenantID = Global.gblTenantID;
                dtResult = clsUnit.GetUnitMaster(GetUnitInfo);
                strGtidColumnSize = "0,-1,250";
            }
            else if (sMenuType.ToUpper() == "AREA")
            {
                GetAreaInfo.AreaID = 0;
                GetAreaInfo.TenantID = Global.gblTenantID;
                dtResult = clsArea.GetAreaMaster(GetAreaInfo);
                strGtidColumnSize = "0,-1,300,200";
            }
            else if (sMenuType.ToUpper() == "ITEM" || sMenuType.ToUpper() == "FRMITEMMASTER" || sMenuType.ToUpper() == "ITEMMASTER")
            {
                // Commented and added By Dipu on 09-Feb-2022 ------ >>
                //GetItem.ItemID = 0;
                //GetItem.TenantID = Global.gblTenantID;
                //dtResult = clsItem.GetItemMaster(GetItem);
                dtResult = Comm.fnGetData("EXEC UspGetItmMasterWhole 0," + Global.gblTenantID + "").Tables[0];
                strGtidColumnSize = "0,250,-1,150,90,130,60,100,200,60";
            }
            else if (sMenuType.ToUpper() == "AGENT")
            {
                GetAgentinfo.AgentID = 0;
                GetAgentinfo.TenantID = Global.gblTenantID;
                dtResult = clsAgent.GetAgentMaster(GetAgentinfo);
                strGtidColumnSize = "0,200,-1,200,100,100,100";
            }
            else if (sMenuType.ToUpper() == "SUPPLIER")
            {
                GetLedgerInfo.LID = 0;
                GetLedgerInfo.TenantID = Global.gblTenantID;
                GetLedgerInfo.AccGroupID = 11;
                dtResult = clsLedger.GetLedgerDetail(GetLedgerInfo);
                strGtidColumnSize = "0,-1,200,100,80,100,80,100,100";
            }
            else if (sMenuType.ToUpper() == "CUSTOMER")
            {
                GetLedgerInfo.LID = 0;
                GetLedgerInfo.TenantID = Global.gblTenantID;
                GetLedgerInfo.AccGroupID = 10;
                dtResult = clsLedger.GetLedgerDetail(GetLedgerInfo);
                strGtidColumnSize = "0,-1,200,100,80,100,80,100,100";
            }
            else if (sMenuType.ToUpper() == "LEDGER")
            {
                GetLedgerInfo.LID = 0;
                GetLedgerInfo.TenantID = Global.gblTenantID;
                GetLedgerInfo.AccGroupID = 0;
                dtResult = clsLedger.GetLedgerDetail(GetLedgerInfo);
                strGtidColumnSize = "0,-1,200,100,80,100,80,100,100,100,80,100";
            }
            else if (sMenuType.ToUpper() == "TAXMODE")
            {
                GetTaxModeInfo.TaxModeID = 0;
                GetTaxModeInfo.TenantID = Global.gblTenantID;
                dtResult = clsTax.GetTaxMode(GetTaxModeInfo);
                strGtidColumnSize = "0,-1,100";
            }
            else if (sMenuType.ToUpper() == "ACCOUNTGROUP")
            {
                GetAccGroupInfo.AccountGroupID = 0;
                GetAccGroupInfo.TenantID = Global.gblTenantID;
                dtResult = clsAccGroup.GetAccountGroup(GetAccGroupInfo);
                strGtidColumnSize = "0,-1,400,100";
            }
            else if (sMenuType.ToUpper() == "VOUCHERTYP")
            {
                GetVch.VchTypeID = 0;
                GetVch.VchTypeIDs = "";
                GetVch.TenantID = Global.gblTenantID;
                dtResult = clsVchTyp.GetVchType(GetVch);
                strGtidColumnSize = "0,-1,100,100,560";
            }
            else if (sMenuType.ToUpper() == "STATE")
            {
                GetStateinfo.StateId = 0;
                GetStateinfo.TenantID = Global.gblTenantID;
                dtResult = clsStat.GetStates(GetStateinfo);
                strGtidColumnSize = "0,200,-1,200,200";
            }
            else if (sMenuType.ToUpper() == "COSTCENTRE")
            {
                GetCostCentreinfo.CCID = 0;
                GetCostCentreinfo.TenantID = Global.gblTenantID;
                dtResult = clsCostCtr.GetCostCentre(GetCostCentreinfo);
                strGtidColumnSize = "0,-1,600";
            }
            else if (sMenuType.ToUpper() == "EMPLOYEE")
            {
                GetEmpInfo.EmpID = 0;
                GetEmpInfo.TenantID = Global.gblTenantID;
                dtResult = clsEmp.GetEmployee(GetEmpInfo);
                strGtidColumnSize = "0,-1,200,300,100,200,100";
            }
            else if (sMenuType.ToUpper() == "DEPARTMENT")
            {
                GetDepartmentinfo.DepartmentID = 0;
                GetDepartmentinfo.TenantID = Global.gblTenantID;
                dtResult = clsDepart.GetDepartment(GetDepartmentinfo);
                strGtidColumnSize = "0,-1,600";
            }
            else if (sMenuType.ToUpper() == "STOCKDEPARTMENT")
            {
                GetDepartmentinfo.DepartmentID = 0;
                GetDepartmentinfo.TenantID = Global.gblTenantID;
                dtResult = clsDepart.GetStockDepartment(GetDepartmentinfo);
                strGtidColumnSize = "0,-1,600";
            }
            else if (sMenuType.ToUpper() == "USERGROUP")
            {
                GetuserInfo.GroupID = 0;
                GetuserInfo.TenantID = Global.gblTenantID;
                dtResult = clsuser.GetUserGroupMaster(GetuserInfo);
                strGtidColumnSize = "0,-1,100,100,100";
            }
            else if (sMenuType.ToUpper() == "USER")
            {
                GetUsrInfo.UserID = 0;
                dtResult = clsUsr.GetUserMaster(GetUsrInfo);
                strGtidColumnSize = "0,-1,300";
            }
            else if (sMenuType.ToUpper() == "PURCHASE")
            {
                GetPurchaseInfo.InvId = 0;
                GetPurchaseInfo.TenantID = Global.gblTenantID;
                GetPurchaseInfo.VchTypeID = itvwSelectedNodeID;
                dtResult = clsPur.GetPurchaseMaster(GetPurchaseInfo);
                strGtidColumnSize = "0,100,100,100,100,-1,100,100,100"; //,100,100
            }
            else if (sMenuType.ToUpper() == "PURCHASE_RETURN")
            {
                GetPurchaseInfo.InvId = 0;
                GetPurchaseInfo.TenantID = Global.gblTenantID;
                GetPurchaseInfo.VchTypeID = itvwSelectedNodeID;
                dtResult = clsPur.GetPurchaseMaster(GetPurchaseInfo);
                strGtidColumnSize = "0,100,100,100,100,-1,100,100,100"; //,100,100
            }
            else if (sMenuType.ToUpper() == "RECEIPT_NOTE")
            {
                GetPurchaseInfo.InvId = 0;
                GetPurchaseInfo.TenantID = Global.gblTenantID;
                GetPurchaseInfo.VchTypeID = itvwSelectedNodeID;
                dtResult = clsPur.GetPurchaseMaster(GetPurchaseInfo);
                strGtidColumnSize = "0,100,100,100,100,-1,100,100,100"; //,100,100
            }
            else if (sMenuType.ToUpper() == "SALES")
            {
                GetSalesInfo.InvId = 0;
                GetSalesInfo.TenantID = Global.gblTenantID;
                GetSalesInfo.VchTypeID = itvwSelectedNodeID;
                dtResult = clsSale.GetSalesMaster(GetSalesInfo);
                strGtidColumnSize = "0,100,100,100,100,-1,100,100,100"; 
            }
            else if (sMenuType.ToUpper() == "SALES_RETURN")
            {
                GetSalesInfo.InvId = 0;
                GetSalesInfo.TenantID = Global.gblTenantID;
                GetSalesInfo.VchTypeID = itvwSelectedNodeID;
                dtResult = clsSale.GetSalesMaster(GetSalesInfo);
                strGtidColumnSize = "0,100,100,100,100,-1,100,100,100"; 
            }
            else if (sMenuType.ToUpper() == "DELIVERY_NOTE")
            {
                GetSalesInfo.InvId = 0;
                GetSalesInfo.TenantID = Global.gblTenantID;
                GetSalesInfo.VchTypeID = itvwSelectedNodeID;
                dtResult = clsSale.GetSalesMaster(GetSalesInfo);
                strGtidColumnSize = "0,100,100,100,100,-1,100,100,100"; 
            }
            else if (sMenuType.ToUpper() == "RECEIPT")
            {
                GetAccInfo.InvId = 0;
                GetAccInfo.TenantID = Global.gblTenantID;
                GetAccInfo.VchTypeID = itvwSelectedNodeID;
                dtResult = clsAcc.GetAccountsMaster(GetAccInfo);
                strGtidColumnSize = "0,300,300,300,300,0";
            }
            else if (sMenuType.ToUpper() == "PAYMENT")
            {
                GetAccInfo.InvId = 0;
                GetAccInfo.TenantID = Global.gblTenantID;
                GetAccInfo.VchTypeID = itvwSelectedNodeID;
                dtResult = clsAcc.GetAccountsMaster(GetAccInfo);
                strGtidColumnSize = "0,300,300,300,300,0";
            }
            else if (sMenuType.ToUpper() == "JOURNAL")
            {
                GetAccInfo.InvId = 0;
                GetAccInfo.TenantID = Global.gblTenantID;
                GetAccInfo.VchTypeID = itvwSelectedNodeID;
                dtResult = clsAcc.GetAccountsMaster(GetAccInfo);
                strGtidColumnSize = "0,300,300,300,300,0";
            }
            else if (sMenuType.ToUpper() == "CONTRA")
            {
                GetAccInfo.InvId = 0;
                GetAccInfo.TenantID = Global.gblTenantID;
                GetAccInfo.VchTypeID = itvwSelectedNodeID;
                dtResult = clsAcc.GetAccountsMaster(GetAccInfo);
                strGtidColumnSize = "0,300,300,300,300,0";
            }
            //Added By Anjitha 22/03/2022 10:00 AM
            int setfrmwidth =this.Width;
            Comm.LoadGrdiControl(gridGroupingControlSearch, dtResult, true,false, strGtidColumnSize,setfrmwidth);

            if (bTransNode == false)
            {
                lblHeader.Text = "Edit Window " + "(" + strFormHeaderName + ")";
                this.Text = lblHeader.Text;
            }
        }
        //Description : Get Selected row ID of Syncfusion Grid
        private int GetSelectedRowID(string sColumnValue)
        {
            int iResult = 0;
              
            Syncfusion.Windows.Forms.Grid.GridRangeInfoList s1 = this.gridGroupingControlSearch.TableModel.Selections.GetSelectedRows(true, true);
        
            foreach (Syncfusion.Windows.Forms.Grid.GridRangeInfo info in s1)
            {
                Syncfusion.Grouping.Element el = this.gridGroupingControlSearch.TableModel.GetDisplayElementAt(info.Top);
            if (el != null)
            {
                    if (el.GetRecord() != null)
                        iResult = Convert.ToInt32(el.GetRecord().GetValue(sColumnValue).ToString());
            }
        }
            return iResult;
        }
        //Description : Get Selected row Data of Syncfusion Grid 
        private string GetSelectedRowData(string sColumnValue)
        {
            string sResult = "";
            Syncfusion.Windows.Forms.Grid.GridRangeInfoList s1 = this.gridGroupingControlSearch.TableModel.Selections.GetSelectedRows(true, true);
            foreach (Syncfusion.Windows.Forms.Grid.GridRangeInfo info in s1)
            {
                Syncfusion.Grouping.Element el = this.gridGroupingControlSearch.TableModel.GetDisplayElementAt(info.Top);
                if (el.GetRecord().GetValue(sColumnValue) != null) sResult = el.GetRecord().GetValue(sColumnValue).ToString();
            }
            return sResult;
        }
        //Description :Master Checked False for when Other Tab Click
        private void Masterchecked()
        {
            rdoCategory.Checked = false;
            rdoManufacturer.Checked = false;
            rdoBrand.Checked = false;
            rdoDiscGroup.Checked = false;
            rdoSize.Checked = false;
            rdoColor.Checked = false;
            rdoUnit.Checked = false;
            rdoItemMaster.Checked = false;
            rdoArea.Checked = false;
            rdoAgent.Checked = false;
            rdoSupplier.Checked = false;
            rdoCustomer.Checked = false;
            rdoLedger.Checked = false;
            rdoTaxMode.Checked = false;
            rdoAccountGroup.Checked = false;
            rdoVoucherType.Checked = false;
            rdoSettings.Checked = false;
            rdoState.Checked = false;
            rdoCostCentre.Checked = false;
            rdoEmployee.Checked = false;
            rdoStockDepartment.Checked = false;
            rdoDepartment.Checked = false;
            rdoCashDesk.Checked = false;
            bTransNode = false;
            trvwParentTransction.SelectedNode = null;
        }
        //Description :Analysis Checked False for when Other Tab Click
        private void AnalysisChecked()
        {
            rdoStockAnalysis.Checked = false;
            bTransNode = false;
            trvwParentTransction.SelectedNode = null;
        }
        //Description :UserManagement Checked False for when Other Tab Click
        private void UserManagementChecked()
        {
            rdoUserGroup.Checked = false;
            rdoUser.Checked = false;
            bTransNode = false;
        }
        //Description :Report Checked False for when Other Tab Click
        private void ReportChecked()
        {
            rdoPurchaseReport.Checked = false;
            bTransNode = false;
            trvwParentTransction.SelectedNode = null;
        }
        //Description :Fill Treeview for Transaction
        private void FillTreeview()
        {
            DataTable dtTreeView = new DataTable();
            TreeNode parentNode;
            dtTreeView = Comm.fnGetData("SELECT VchTypeID,VchType,ParentID,SystemName,UserID,LastUpdateDate,LastUpdateTime,TenantID FROM tblVchType WHERE ((VchTypeID BETWEEN 1 and 500) OR (VchTypeID >= 1005)) AND ParentID=VchTypeID").Tables[0];
            trvwParentTransction.Nodes.Clear();
            if (dtTreeView.Rows.Count > 0)
            {
                foreach (DataRow dr in dtTreeView.Rows)
                {
                    parentNode = trvwParentTransction.Nodes.Add(dr["VchTypeID"].ToString(), dr["VchType"].ToString());
                    PopulateTreeView(Convert.ToInt32(dr["VchTypeID"].ToString()), parentNode);
                }
                trvwParentTransction.ExpandAll();
            }
        }
        //Description :Fill Treeview for Child Transaction
        private void PopulateTreeView(int parentId, TreeNode parentNode)
        {
            DataTable dtgetData = new DataTable();
            dtgetData = Comm.fnGetData("SELECT VchTypeID,VchType,ParentID,SystemName,UserID,LastUpdateDate,LastUpdateTime,TenantID FROM tblVchType WHERE ((VchTypeID BETWEEN 1 and 500) OR (VchTypeID >= 1005)) AND (ParentID <> VchTypeID) AND ParentID =" + parentId + " ORDER BY VchTypeID Desc").Tables[0];
            TreeNode childNode;
            foreach (DataRow dr in dtgetData.Rows)
            {
                if (parentNode == null)
                {
                    childNode = trvwParentTransction.Nodes.Add(dr["VchTypeID"].ToString(), dr["VchType"].ToString());
                }
                else
                {
                    parentNode.Tag = dr["ParentID"].ToString();

                    childNode = parentNode.Nodes.Add(dr["VchTypeID"].ToString(), dr["VchType"].ToString());
                }
                PopulateTreeView(Convert.ToInt32(dr["VchTypeID"].ToString()), childNode);
            }
        }
        //Description :table Layout Panel Size Adjustment Based on Application Settings
        private void TlpNavigatorSizeBasedonAppSettings()
        {
            if (AppSettings.NeedBrand == false)
            {
                this.tlpNavigator.RowStyles[1].Height = tlpNavigator.RowStyles[1].Height - 40;
            }
            if (AppSettings.NeedSize == false)
            {
                this.tlpNavigator.RowStyles[1].Height = tlpNavigator.RowStyles[1].Height - 40;
            }
            if (AppSettings.NeedColor == false)
            {
                this.tlpNavigator.RowStyles[1].Height = tlpNavigator.RowStyles[1].Height - 40;
            }
            if (AppSettings.NeedAgent == false)
            {
                this.tlpNavigator.RowStyles[1].Height = tlpNavigator.RowStyles[1].Height - 40;
            }
            if (AppSettings.TaxEnabled == false)
            {
                this.tlpNavigator.RowStyles[1].Height = tlpNavigator.RowStyles[1].Height - 40;
            }
        }
        //Description :Based on Application Settings
        private void ApplicationSettings()
        {
            if (AppSettings.NeedAgent == false)
            {
                rdoAgent.Visible = false;

                this.tlpMasters.RowStyles[9].SizeType = SizeType.Absolute;
                this.tlpMasters.RowStyles[9].Height = 0;
                this.tlpNavigator.RowStyles[1].Height = tlpNavigator.RowStyles[1].Height - 40;
            }

            if (AppSettings.NeedBrand == false)
            {
                rdoBrand.Visible = false;

                this.tlpMasters.RowStyles[2].SizeType = SizeType.Absolute;
                this.tlpMasters.RowStyles[2].Height = 0;
                this.tlpNavigator.RowStyles[1].Height = tlpNavigator.RowStyles[1].Height - 40;
            }
            if (AppSettings.NeedSize == false)
            {
                rdoSize.Visible = false;

                this.tlpMasters.RowStyles[4].SizeType = SizeType.Absolute;
                this.tlpMasters.RowStyles[4].Height = 0;
                this.tlpNavigator.RowStyles[1].Height = tlpNavigator.RowStyles[1].Height - 40;
            }
            if (AppSettings.NeedColor == false)
            {
                rdoColor.Visible = false;

                this.tlpMasters.RowStyles[5].SizeType = SizeType.Absolute;
                this.tlpMasters.RowStyles[5].Height = 0;
                this.tlpNavigator.RowStyles[1].Height = tlpNavigator.RowStyles[1].Height - 40;
            }
            if (AppSettings.TaxEnabled == false)
            {
                rdoTaxMode.Visible = false;

                this.tlpMasters.RowStyles[13].SizeType = SizeType.Absolute;
                this.tlpMasters.RowStyles[13].Height = 0;
                this.tlpNavigator.RowStyles[1].Height = tlpNavigator.RowStyles[1].Height - 40;
            }
           
        }
        //Description :Hide Buttons when click menu button
        private void HideButtons()
        {
            btnCancelDeactive.Visible = false;
            lblCancel.Visible = false;
            //txtSearch.Visible = false;
            togglebtnActive.Visible = false;
        }
        //Description : Remove Grid Row After delete
        private void RemoveGridRowAfterDelete()
        {
            //Syncfusion.Grouping.Record r = this.gridGroupingControlSearch.Table.CurrentRecord;
            //if (r != null)
            //{
            //    r.Delete();
            //}
            Syncfusion.Windows.Forms.Grid.GridRangeInfoList s1 = this.gridGroupingControlSearch.TableModel.Selections.GetSelectedRows(true, true);
            foreach (Syncfusion.Windows.Forms.Grid.GridRangeInfo info in s1)
            {
                Syncfusion.Grouping.Element el = this.gridGroupingControlSearch.TableModel.GetDisplayElementAt(info.Top);
                Syncfusion.Grouping.Record r = el.GetRecord();
                if (r != null)
                {
                    r.Delete();
                }
            }
        }
        //Description : Radio Button Double Click Event
        private void RadioButtonDoubleClick()
        {
            MethodInfo m = typeof(RadioButton).GetMethod("SetStyle", BindingFlags.Instance | BindingFlags.NonPublic);
            if (m != null)
            {
                m.Invoke(rdoCategory, new object[] { ControlStyles.StandardClick | ControlStyles.StandardDoubleClick, true });
                m.Invoke(rdoManufacturer, new object[] { ControlStyles.StandardClick | ControlStyles.StandardDoubleClick, true });
                m.Invoke(rdoBrand, new object[] { ControlStyles.StandardClick | ControlStyles.StandardDoubleClick, true });
                m.Invoke(rdoDiscGroup, new object[] { ControlStyles.StandardClick | ControlStyles.StandardDoubleClick, true });
                m.Invoke(rdoSize, new object[] { ControlStyles.StandardClick | ControlStyles.StandardDoubleClick, true });
                m.Invoke(rdoColor, new object[] { ControlStyles.StandardClick | ControlStyles.StandardDoubleClick, true });
                m.Invoke(rdoUnit, new object[] { ControlStyles.StandardClick | ControlStyles.StandardDoubleClick, true });
                m.Invoke(rdoItemMaster, new object[] { ControlStyles.StandardClick | ControlStyles.StandardDoubleClick, true });
                m.Invoke(rdoArea, new object[] { ControlStyles.StandardClick | ControlStyles.StandardDoubleClick, true });
                m.Invoke(rdoAgent, new object[] { ControlStyles.StandardClick | ControlStyles.StandardDoubleClick, true });
                m.Invoke(rdoSupplier, new object[] { ControlStyles.StandardClick | ControlStyles.StandardDoubleClick, true });
                m.Invoke(rdoCustomer, new object[] { ControlStyles.StandardClick | ControlStyles.StandardDoubleClick, true });
                m.Invoke(rdoLedger, new object[] { ControlStyles.StandardClick | ControlStyles.StandardDoubleClick, true });
                m.Invoke(rdoTaxMode, new object[] { ControlStyles.StandardClick | ControlStyles.StandardDoubleClick, true });
                m.Invoke(rdoAccountGroup, new object[] { ControlStyles.StandardClick | ControlStyles.StandardDoubleClick, true });
                m.Invoke(rdoVoucherType, new object[] { ControlStyles.StandardClick | ControlStyles.StandardDoubleClick, true });
                m.Invoke(rdoSettings, new object[] { ControlStyles.StandardClick | ControlStyles.StandardDoubleClick, true });
                m.Invoke(rdoState, new object[] { ControlStyles.StandardClick | ControlStyles.StandardDoubleClick, true });
                m.Invoke(rdoCostCentre, new object[] { ControlStyles.StandardClick | ControlStyles.StandardDoubleClick, true });
                m.Invoke(rdoEmployee, new object[] { ControlStyles.StandardClick | ControlStyles.StandardDoubleClick, true });
                m.Invoke(rdoStockDepartment, new object[] { ControlStyles.StandardClick | ControlStyles.StandardDoubleClick, true });
                m.Invoke(rdoDepartment, new object[] { ControlStyles.StandardClick | ControlStyles.StandardDoubleClick, true });
                m.Invoke(rdoCashDesk, new object[] { ControlStyles.StandardClick | ControlStyles.StandardDoubleClick, true });
                m.Invoke(rdoStockAnalysis, new object[] { ControlStyles.StandardClick | ControlStyles.StandardDoubleClick, true });
                m.Invoke(rdoUserGroup, new object[] { ControlStyles.StandardClick | ControlStyles.StandardDoubleClick, true });
                m.Invoke(rdoUser, new object[] { ControlStyles.StandardClick | ControlStyles.StandardDoubleClick, true });
                m.Invoke(rdoPurchaseReport, new object[] { ControlStyles.StandardClick | ControlStyles.StandardDoubleClick, true });
            }
            rdoCategory.MouseDoubleClick += rdoCategory_DoubleClick;
            rdoManufacturer.MouseDoubleClick += rdoManufacturer_DoubleClick;
            rdoBrand.MouseDoubleClick += rdoBrand_DoubleClick;
            rdoDiscGroup.MouseDoubleClick += rdoDiscGroup_DoubleClick;
            rdoSize.MouseDoubleClick += rdoSize_DoubleClick;
            rdoColor.MouseDoubleClick += rdoColor_DoubleClick;
            rdoUnit.MouseDoubleClick += rdoUnit_DoubleClick;
            rdoItemMaster.MouseDoubleClick += rdoItemMaster_DoubleClick;
            rdoArea.MouseDoubleClick += rdoArea_DoubleClick;
            rdoAgent.MouseDoubleClick += rdoAgent_DoubleClick;
            rdoSupplier.MouseDoubleClick += rdoSupplier_DoubleClick;
            rdoCustomer.MouseDoubleClick += rdoCustomer_DoubleClick;
            rdoLedger.MouseDoubleClick += rdoLedger_DoubleClick;
            rdoTaxMode.MouseDoubleClick += rdoTaxMode_DoubleClick;
            rdoAccountGroup.MouseDoubleClick += rdoAccountGroup_DoubleClick;
            rdoVoucherType.MouseDoubleClick += rdoVoucherType_DoubleClick;
            rdoSettings.MouseDoubleClick += rdoSettings_DoubleClick;
            rdoState.MouseDoubleClick += rdoState_DoubleClick;
            rdoCostCentre.MouseDoubleClick += rdoCostCentre_DoubleClick;
            rdoEmployee.MouseDoubleClick += rdoEmployee_DoubleClick;
            //rdoStockDepartment.MouseDoubleClick += rdoDepartment_DoubleClick;
            rdoDepartment.MouseDoubleClick += rdoDepartment_DoubleClick;
            rdoCashDesk.MouseDoubleClick += rdoCashDesk_DoubleClick;
            rdoStockDepartment.MouseDoubleClick += rdoStockDepartment_DoubleClick;
            rdoStockAnalysis.MouseDoubleClick += rdoStockAnalysis_DoubleClick;
            rdoUserGroup.MouseDoubleClick += rdoUserGroup_DoubleClick;
            rdoUser.MouseDoubleClick += rdoUser_DoubleClick;
            rdoPurchaseReport.MouseDoubleClick += rdoPurchaseReport_DoubleClick;
        }
        private void NewClick()
        {
            if (rdoCategory.Checked == true)
            {
                frmItemCategory frmCat = new frmItemCategory(0, false);
                frmCat.MdiParent = this.MdiParent;
                frmCat.Show();
                frmCat.BringToFront();
            }
            else if (rdoManufacturer.Checked == true)
            {
                frmManufacturer frm = new frmManufacturer(0, false);
                frm.MdiParent = this.MdiParent;
                frm.Show();
                frm.BringToFront();
            }
            else if (rdoBrand.Checked == true)
            {
                frmBrandMaster frm = new frmBrandMaster(0, false);
                frm.MdiParent = this.MdiParent;
                frm.Show();
                frm.BringToFront();
            }
            else if (rdoDiscGroup.Checked == true)
            {
                frmDiscountGroup frm = new frmDiscountGroup(0, false);
                frm.MdiParent = this.MdiParent;
                frm.Show();
                frm.BringToFront();
            }
            else if (rdoSize.Checked == true)
            {
                FrmSizeMaster frm = new FrmSizeMaster(0, false);
                frm.MdiParent = this.MdiParent;
                frm.Show();
                frm.BringToFront();
            }
            else if (rdoColor.Checked == true)
            {
                frmColorMaster frm = new frmColorMaster(0, false);
                frm.MdiParent = this.MdiParent;
                frm.Show();
                frm.BringToFront();
            }
            else if (rdoUnit.Checked == true)
            {
                frmUnitMaster frm = new frmUnitMaster(0, false);
                frm.MdiParent = this.MdiParent;
                frm.Show();
                frm.BringToFront();
            }
            else if (rdoItemMaster.Checked == true)
            {
                frmItemMaster frm = new frmItemMaster(0, true);
                frm.MdiParent = this.MdiParent;
                frm.Show();
                frm.BringToFront();
            }
            else if (rdoArea.Checked == true)
            {
                frmAreaMaster frm = new frmAreaMaster(0, false);
                frm.MdiParent = this.MdiParent;
                frm.Show();
                frm.BringToFront();
            }
            else if (rdoAgent.Checked == true)
            {
                frmAgentMaster frm = new frmAgentMaster(0, false);
                frm.MdiParent = this.MdiParent;
                frm.Show();
                frm.BringToFront();
            }
            else if (rdoSupplier.Checked == true)
            {
                frmLedger frm = new frmLedger(0, false, 0, "SUPPLIER");
                frm.MdiParent = this.MdiParent;
                frm.Show();
                frm.BringToFront();
            }
            else if (rdoCustomer.Checked == true)
            {
                frmLedger frm = new frmLedger(0, false, 0, "CUSTOMER");
                frm.MdiParent = this.MdiParent;
                frm.Show();
                frm.BringToFront();
            }
            else if (rdoLedger.Checked == true)
            {
                frmLedger frm = new frmLedger(0, false);
                frm.MdiParent = this.MdiParent;
                frm.Show();
                frm.BringToFront();
            }
            else if (rdoTaxMode.Checked == true)
            {
                frmTaxMode frm = new frmTaxMode(0, false);
                frm.MdiParent = this.MdiParent;
                frm.Show();
                frm.BringToFront();
            }
            else if (rdoAccountGroup.Checked == true)
            {
                frmAccountGroup frm = new frmAccountGroup(0, false);
                frm.MdiParent = this.MdiParent;
                frm.Show();
                frm.BringToFront();
            }
            else if (rdoVoucherType.Checked == true)
            {
                frmVouchertype frm = new frmVouchertype(0, false);
                frm.MdiParent = this.MdiParent;
                frm.Show();
                frm.BringToFront();
            }
            else if (rdoSettings.Checked == true)
            {
                frmSettings frm = new frmSettings(0, false);
                frm.MdiParent = this.MdiParent;
                frm.Show();
                frm.BringToFront();
            }
            else if (rdoState.Checked == true)
            {
                frmState frm = new frmState(0, false);
                frm.MdiParent = this.MdiParent;
                frm.Show();
                frm.BringToFront();
            }
            else if (rdoCostCentre.Checked == true)
            {
                frmCostCentre frm = new frmCostCentre(0, false);
                frm.MdiParent = this.MdiParent;
                frm.Show();
                frm.BringToFront();
            }
            else if (rdoEmployee.Checked == true)
            {
                frmEmployee frm = new frmEmployee(0, false);
                frm.MdiParent = this.MdiParent;
                frm.Show();
                frm.BringToFront();
            }
            else if (rdoStockDepartment.Checked == true)
            {
                frmDepartment frm = new frmDepartment(0, false, 0);
                frm.MdiParent = this.MdiParent;
                frm.Show();
                frm.BringToFront();
            }
            else if (rdoDepartment.Checked == true)
            {
                frmDepartment frm = new frmDepartment(0, false, 1);
                frm.MdiParent = this.MdiParent;
                frm.Show();
                frm.BringToFront();
            }
            else if (rdoCashDesk.Checked == true)
            {
                frmCashDeskMaster frm = new frmCashDeskMaster(0, false);
                frm.MdiParent = this.MdiParent;
                frm.Show();
                frm.BringToFront();
            }
            else if (rdoStockAnalysis.Checked == true) //Analysis
            {
                frmItemAnalysis frm = new frmItemAnalysis();
                frm.MdiParent = this.MdiParent;
                frm.Show();
                frm.BringToFront();
            }
            else if (rdoUserGroup.Checked == true) //User Management
            {
                frmUserGroup frm = new frmUserGroup(0, false);
                frm.MdiParent = this.MdiParent;
                frm.Show();
                frm.BringToFront();
            }
            else if (rdoUser.Checked == true)
            {
                frmUser frm = new frmUser(0, false);
                frm.MdiParent = this.MdiParent;
                frm.Show();
                frm.BringToFront();
            }
            else if (rdoPurchaseReport.Checked == true) //Analysis
            {
                frmPurchaseReport frm = new frmPurchaseReport();
                frm.MdiParent = this.MdiParent;
                frm.Show();
                frm.BringToFront();
            }
            else //Transaction Treeview
            {
                if (ITransParentID > 0)
                {
                    if (ITransParentID == 2) // PURCHASE
                    {
                        frmStockInVoucherNew frm = new frmStockInVoucherNew(itvwSelectedNodeID, 0, false, this.MdiParent);
                        frm.MdiParent = this.MdiParent;
                        frm.Show();
                        frm.BringToFront();
                    }
                    else if (ITransParentID == 4) // PURCHAE RETURN
                    {
                        frmStockInVoucherNew frm = new frmStockInVoucherNew(itvwSelectedNodeID, 0, false, this.MdiParent);
                        frm.MdiParent = this.MdiParent;
                        frm.Show();
                        frm.BringToFront();
                    }
                    else if (ITransParentID == 6) // RECEIPT NOTE
                    {
                        frmStockInVoucherNew frm = new frmStockInVoucherNew(itvwSelectedNodeID, 0, false, this.MdiParent);
                        frm.MdiParent = this.MdiParent;
                        frm.Show();
                        frm.BringToFront();
                    }
                }
            }
        }
        #endregion

        private void rdoItemMaster_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void rdoColor_CheckedChanged(object sender, EventArgs e)
        {

        }
        private void rdoCashDesk_CheckedChanged(object sender, EventArgs e)
        {

        }
        public bool PreFilterMessage(ref Message m)
        {
            if (m.Msg == WM_LBUTTONDOWN &&
                        controlsToMove.Contains(Control.FromHandle(m.HWnd)))
            {
                ReleaseCapture();
                SendMessage(this.Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
                return true;
            }
            return false;
        }

        private void rdoStockDepartment_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void rdoEmployee_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void rdoCashDesk_Click_1(object sender, EventArgs e)
        {
            if (rdoCashDesk.Checked == true)
            {
                HideButtons();
                AnalysisChecked();
                UserManagementChecked();
                ReportChecked();
                strFormHeaderName = "CashDesk";
                GetDataAsperMenuClick("CASHDESK");
                ibtnNumber = 30;
            }
        }
        private void rdoDepartment_Click_1(object sender, EventArgs e)
        {
            if (rdoDepartment.Checked == true)
            {
                HideButtons();
                AnalysisChecked();
                UserManagementChecked();
                ReportChecked();
                strFormHeaderName = "Department";
                GetDataAsperMenuClick("DEPARTMENT");
                ibtnNumber = 29;
            }
        }

        private void rdoStockDepartment_Click(object sender, EventArgs e)
        {
            if (rdoStockDepartment.Checked == true)
            {
                HideButtons();
                AnalysisChecked();
                UserManagementChecked();
                ReportChecked();
                strFormHeaderName = "Stock Department";
                GetDataAsperMenuClick("STOCKDEPARTMENT");
                ibtnNumber = 19;
            }
        }

        private void rdoDepartment_KeyDown_1(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                NewClick();
            }
        }
        private void rdoCashDesk_KeyDown_1(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                NewClick();
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            try
            {

                //this.gridGroupingControlSearch.SearchController.Search(this.txtSearch.Text);
            }
            catch
            { }
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {

        }
    }
}


       
    

