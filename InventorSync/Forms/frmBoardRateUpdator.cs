using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using InventorSync.Forms;
using InventorSync.InventorBL.Master;
using InventorSync.InventorBL.Accounts;
using InventorSync.InventorBL.Helper;
using InventorSync.Info;
using Syncfusion.WinForms.DataGrid;
using InventorSync.JsonClass;
using Newtonsoft.Json;
using DataRow = System.Data.DataRow;
using InventorSync.InventorBL.Transaction;
using System.Collections;
using Microsoft.VisualBasic;
using System.Runtime.InteropServices;
using System.Data.SqlClient;
using System.IO;

namespace InventorSync
{

    public partial class frmBoardRateUpdator : Form, IMessageFilter
    {
        //=============================================================================
        // Created By       : Arun
        // Created On       : 02-Feb-2022
        // Last Edited On   :
        // Last Edited By   : Arun
        // Description      : Working With Journal, Receipt, Payment, Contra Voucher Type. 
        // Methods Used     : 
        //=============================================================================
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        public const int WM_LBUTTONDOWN = 0x0201;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        private HashSet<Control> controlsToMove = new HashSet<Control>();

        int PrefixTag;

        public frmBoardRateUpdator(int iVchTpeId = 0, int iTransID = 0, bool bFromEdit = false, object MDIParent = null)
        {
            InitializeComponent();
            Application.AddMessageFilter(this);

            try
            {
                Comm.TransparentControls(this);
                Comm.SetControlColours(this);

                tlpMain.BackColor = Color.FromArgb(249, 246, 238);
                this.BackColor = Color.Black;
                this.Padding = new Padding(1);

                //Comm.LoadBGImage(this, picBackground);

                lblHeading.Font = new Font("Tahoma", 21, FontStyle.Regular, GraphicsUnit.Pixel);
                lblSave.Font = new Font("Tahoma", 10, FontStyle.Regular, GraphicsUnit.Point);
                lblFind.Font = new Font("Tahoma", 10, FontStyle.Regular, GraphicsUnit.Point);

                lblSave.ForeColor = Color.Black;
                lblFind.ForeColor = Color.Black;

                btnprev.Image = global::DigiposZen.Properties.Resources.fast_backwards;
                btnNext.Image = global::DigiposZen.Properties.Resources.fast_forward;
                btnSave.Image = global::DigiposZen.Properties.Resources.save240402;
                btnFind.Image = global::DigiposZen.Properties.Resources.find_finalised_3030;
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

            controlsToMove.Add(this);
            controlsToMove.Add(this.lblHeading);//Add whatever controls here you want to move the form when it is clicked and dragged

            frmMDI form = (frmMDI)MDIParent;
            this.MdiParent = form;
            int l = form.ClientSize.Width - 10; //(this.MdiParent.ClientSize.Width - this.Width) / 2;
            int t = form.ClientSize.Height - 80; //((this.MdiParent.ClientSize.Height - this.Height) / 2) - 30;
            this.SetBounds(5, 0, l, t);

            clsVchType = JSonComm.GetVoucherType(iVchTpeId);
            clsVchTypeFeatures = JSonComm.GetVoucherTypeGeneralSettings(iVchTpeId, 1);

            AddColumnsToGrid();

            ClearControls();

            iIDFromEditWindow = iTransID;
            vchtypeID = iVchTpeId;

            if (iIDFromEditWindow != 0)
                PrefixTag = 1;
            else
                PrefixTag = 0;

            if (iTransID != 0)
            {
                SetTransactionsthatVarying();
                LoadData(iTransID);
                txtInvAutoNo.Select();
            }
            else
                SetTransactionsthatVarying();
        }

        #region "VARIABLES --------------------------------------------- >>"

        Control ctrl;
        string sEditedValueonKeyPress = "";
        int iIDFromEditWindow, vchtypeID;
        bool dragging = false;
        int xOffset = 0, yOffset = 0, d=0;
        string strSelectedLedgerName = "";
        string constr = DigiposZen.Properties.Settings.Default.ConnectionString;
        //static int namesCount = Enum.GetNames(typeof(LedgerIndexes)).Length;
        Common Comm = new Common();

        UspGetItemMasterInfo GetItmMstinfo = new UspGetItemMasterInfo();
        UspGetItemMasterFromStockInfo GetItmMststockinfo = new UspGetItemMasterFromStockInfo();
        UspgetitemmasterBatchUniqueInfo GetItmMstBatchinfo = new UspgetitemmasterBatchUniqueInfo();
        UspGetEmployeeInfo GetEmpInfo = new UspGetEmployeeInfo();
        UspGetCostCentreInfo GetCctinfo = new UspGetCostCentreInfo();
        UspGetTaxModeInfo GetTaxMinfo = new UspGetTaxModeInfo();
        UspGetAgentinfo GetAgentinfo = new UspGetAgentinfo();
        UspGetLedgerInfo GetLedinfo = new UspGetLedgerInfo();
        UspGetUnitInfo GetUnitInfo = new UspGetUnitInfo();
        UspGetStockDetailsInfo GetStockInfo = new UspGetStockDetailsInfo();
        UspGetPriceListInfo GetPriceListInfo = new UspGetPriceListInfo();

        clsItemMaster clsItmMst = new clsItemMaster();
        clsEmployee clsEmp = new clsEmployee();
        clsCostCentre clscct = new clsCostCentre();
        clsTaxMode clsTax = new clsTaxMode();
        clsAgentMaster clsAgent = new clsAgentMaster();
        clsLedger clsLedg = new clsLedger();
        clsUnitMaster clsUnit = new clsUnitMaster();
        clsStockDetails clsStock = new clsStockDetails();

        clsJSonCommon JSonComm = new clsJSonCommon();
        clsPriceList clsPL = new clsPriceList();

        //Sales Master Related Classes for Json
        clsJSonPriceList clsJPL = new clsJSonPriceList();
        clsJsonPLInfo clsJPLinfo = new clsJsonPLInfo();
        clsJsonPMCCentreInfo clsJPMCostCentreinfo = new clsJsonPMCCentreInfo();
        clsJsonPMDestCCentreInfo clsJPMDestCostCentreinfo = new clsJsonPMDestCCentreInfo();
        clsJsonPMEmployeeInfo clsJPMEmployeeinfo = new clsJsonPMEmployeeInfo();

        //Sales Detail Related Classes For Json
        clsJsonPLDInfo clsJSJDinfo = new clsJsonPLDInfo();
        clsJsonPDUnitinfo clsJPDUnitinfo = new clsJsonPDUnitinfo();
        clsJsonPDIteminfo clsJPDIteminfo = new clsJsonPDIteminfo();

        DataTable dtItemPublic = new DataTable();
        DataTable dtUnitPublic = new DataTable();
        DataTable dtBatchCode = new DataTable();
        DataTable dtBatchCodeData = new DataTable();

        //Purchase Detail Related Classes For Json
        clsJsonPDetailsInfo clsJPDinfo = new clsJsonPDetailsInfo();

        clsCostCentre clsccntr = new clsCostCentre();
        clsJsonVoucherType clsVchType = new clsJsonVoucherType();
        clsGetStockInVoucherSettings clsVchTypeFeatures = new clsGetStockInVoucherSettings();

        private BoardRateGridColIndexes gridColIndexes = new BoardRateGridColIndexes();

        enum EmpIndexes
        {
            EmpID, //0
            Name,
            Address,
            NameOfFather,
            PhNo,
            MaritialStatus,
            NoOfFamilyMembers,
            NameOFNominee,
            Spouse,
            SpouseEmployed,
            OwnerOfResidence,
            PANNo,
            BloodGroup,
            Designation,
            Qualification,
            Sex,
            DOB,
            DOJ,
            DOI,
            PensionAccNo,
            GPFAccNo,
            GSLIAccNo,
            LICPolicyNo,
            LICMonthlyPremium,
            LICDateofMaturity,
            CategoryID,
            DateofPromotion,
            DateofRetirement,
            GISAccNo,
            BankAccNo,
            Commission,
            CommissionAmt,
            EmpFname,
            blnSalesStaff,
            PhotoPath,
            InsCompany,
            CommissionCondition,
            EmpCode,
            blnStatus,
            DrivingLicenceNo,
            DrivingLicenceExpiry,
            PassportNo,
            PassportExpiry,
            Active,
            SortOrder,
            EnrollNo,
            TargetAmount,
            IncentivePer,
            PWD,
            Holidays,
            LID,
            salarypermonth,
            SystemName,
            UserID,
            LastUpdateDate,
            LastUpdateTime,
            TenantID
        }

        enum ItemIndexes
        {
            ItemID,
            ItemCode,
            ItemName,
            CategoryID,
            Description,
            PRate,
            SrateCalcMode,
            CRateAvg,
            Srate1Per,
            SRate1,
            Srate2Per,
            SRate2,
            Srate3Per,
            SRate3,
            Srate4,
            Srate4Per,
            SRate5,
            Srate5Per,
            MRP,
            ROL,
            Rack,
            Manufacturer,
            ActiveStatus,
            IntLocal,
            ProductType,
            ProductTypeID,
            LedgerID,
            UNITID,
            Notes,
            agentCommPer,
            BlnExpiryItem,
            Coolie,
            FinishedGoodID,
            MinRate,
            MaxRate,
            PLUNo,
            HSNID,
            iCatDiscPer,
            IPGDiscPer,
            ImanDiscPer,
            ItemNameUniCode,
            Minqty,
            MNFID,
            PGID,
            ItemCodeUniCode,
            UPC,
            BatchMode,
            blnExpiry,
            Qty,
            MaxQty,
            IntNoOrWeight,
            SystemName,
            UserID,
            LastUpdateDate,
            LastUpdateTime,
            TenantID,
            blnCessOnTax,
            CompCessQty,
            CGSTTaxPer,
            SGSTTaxPer,
            IGSTTaxPer,
            CessPer,
            VAT,
            CategoryIDs,
            ColorIDs,
            SizeIDs,
            BrandDisPer,
            DGroupID,
            DGroupDisPer,
            BrandID,
            AltUnitID,
            ConvFactor,
            Shelflife,
            SRateInclusive,
            PRateInclusive,
            Slabsys,
            ParentID,
            ParentConv,
            blnParentEqlRate,
            ItmConvType,
            DiscPer
        }

        #endregion

        #region "EVENTS ------------------------------------------------ >>"

        private void frmStockVoucher_Load(object sender, EventArgs e)
        {
            try
            {
                
                lblHeading.Text = clsVchType.TransactionName;
                this.Text = clsVchType.TransactionName;

                if (iIDFromEditWindow == 0)
                {
                    AddColumnsToGrid();
                    
                }
                else
                {
                    AddColumnsToGrid();

                    btnSave.Enabled = false;
                    //btn.Enabled = false;
                }

                SetTransactionDefaults();
                SetApplicationSettings();

                Application.DoEvents();

                //if (toggleWidthSettings.ToggleState == Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active)
                GridInitialize_dgvColWidth();

                this.tlpMain.ColumnStyles[1].SizeType = SizeType.Absolute;
                this.tlpMain.ColumnStyles[1].Width = 0;

                if (iIDFromEditWindow == 0)
                {
                        txtInvAutoNo.Focus();
                        txtInvAutoNo.Select();
                }
                else
                {
                    int iRowCnt = dgvItems.Rows.Count;
                    dgvItems.CurrentCell = dgvItems.Rows[iRowCnt - 1].Cells[GetEnum(gridColIndexes.CItemCode)];
                    SendKeys.Send("{down}");
                }

                cboDisplayRate.SelectedIndex = 1;

                Comm.LoadControl(cboMachineModel, new DataTable(), "Select ID, ModelName From tblWMDetails Order BY ModelName");


            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
        }

        private DataGridViewCell dgvEndEditCell;

        private bool _EnterMoveNext = true;

        [System.ComponentModel.DefaultValue(true)]
        public bool OnEnterKeyMoveNext
        {
            get
            {
                return this._EnterMoveNext;
            }
            set
            {
                this._EnterMoveNext = value;
            }
        }


        private void btnClose_Click(object sender, EventArgs e)
        {
            DialogResult dlgResult = MessageBox.Show("Your are in the middle of an Entry. Do you want to exit?", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            if (dlgResult.Equals(DialogResult.Yes))
                this.Close();
        }

        private void textBox_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void textBox_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                ctrl = (Control)sender;
                if (ctrl is TextBox)
                {
                    if (e.Shift == true && e.KeyCode == Keys.Enter)
                    {
                        this.SelectNextControl(ctrl, false, true, true, true);
                    }
                    else if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Down)
                    {
                        this.SelectNextControl(ctrl, true, true, true, true);
                    }
                    else if (e.KeyCode == Keys.Up)
                    {
                        this.SelectNextControl(ctrl, false, true, true, true);
                    }
                    else
                        return;
                }
                else
                {
                    if (e.Shift == true && e.KeyCode == Keys.Enter)
                    {
                        this.SelectNextControl(ctrl, false, true, true, true);
                    }
                    else if (e.KeyCode == Keys.Enter)
                    {
                        this.SelectNextControl(ctrl, true, true, true, true);

                    }
                    else if (e.KeyCode == Keys.Up && e.Control)
                    {
                        this.SelectNextControl(ctrl, false, true, true, true);

                    }
                    else
                        return;
                }
                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }

        }

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            if (this.WindowState != FormWindowState.Minimized)
            {
                this.WindowState = FormWindowState.Minimized;
            }
        }

        ComboBox BatchCode_GridCellComboBox = new ComboBox();

        private void gridColumn_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }

        private void  dgvItems_TextBox_KeyPress(object sender, KeyPressEventArgs e)
        {

            string sQuery;
            try
            {
                Form fc = Application.OpenForms["frmDetailedSearch2"];
                if (fc != null)
                {
                    fc.Focus();
                    fc.BringToFront();
                    return;
                }
                sEditedValueonKeyPress = e.KeyChar.ToString();

                if (dgvItems.Rows.Count - 1 == dgvItems.CurrentRow.Index)
                    dgvItems.Rows.Add();

                if (dgvItems.CurrentCell.ColumnIndex == (int)gridColIndexes.CItemCode)
                {
                    if (sEditedValueonKeyPress != null)
                    {
                        if (btnShow.Visible == true)
                        {

                            sQuery = "SELECT ItemCode + ItemName + BatchUnique + CAST(MRP AS varchar) AS anywhere, ItemCode, ItemName, BatchUnique, Expiry, MRP, ItemID, StockID " +
                                " FROM     vwCompactSearch Where isnull(ActiveStatus, 1) = 1 and isnull(StockActiveStatus, 1) = 1 ";
                            if (clsVchType.ProductClassList != "")
                                sQuery = sQuery + " AND ProductTypeID IN (" + clsVchType.ProductClassList + ")";
                            if (clsVchType.ItemCategoriesList != "")
                                sQuery = sQuery + " AND CategoryID IN (" + clsVchType.ItemCategoriesList + ")";
                            //frmDetailedSearch2 frmN = new frmDetailedSearch2(GetFromItemSearch, sQuery, "Anywhere|ItemCode|ItemName|BatchUnique", dgvItems.Location.X + 55, dgvItems.Location.Y + 150, 7, 0, sEditedValueonKeyPress, 6, 0, "ORDER BY ItemCode ASC, Batchunique", 0, 0, "Item Search...", 0, "150,250,150,150,150,0,0", true, "frmItemMaster", 20);
                            //frmN.MdiParent = this.MdiParent;
                            //frmN.Show(); //20-Aug-2022
                            
                            if (dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cItemID)].Value != null)
                            {
                                this.dgvItems.EditingControlShowing -= this.dgvItems_EditingControlShowing;
                                    if (dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cSRate1)].Visible)
                                        dgvItems.CurrentCell = dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cSRate1)];
                                dgvItems.Focus();

                                this.dgvItems.EditingControlShowing += this.dgvItems_EditingControlShowing;
                            }
                        }
                    }
                }
                else if (dgvItems.CurrentCell.ColumnIndex == (int)gridColIndexes.CExpiry)
                {
                    if (dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cMRP)].Visible)
                        dgvItems.CurrentCell = dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cMRP)];
                    dgvItems.Focus();
                }
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void cboCostCentre_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
            {
                dtpInvDate.Focus();
            }
            else if (e.KeyCode == Keys.Enter)
            {
                cboSalesStaff.Focus();
                SendKeys.Send("{F4}");
            }
        }

        private void cboSalesStaff_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
            {
                if (pnlSalesStaff.Visible == true)
                    cboSalesStaff.Focus();

                SendKeys.Send("{F4}");
            }
            else if (e.KeyCode == Keys.Enter)
            {
                dgvItems.CurrentCell = dgvItems.Rows[0].Cells[GetEnum(gridColIndexes.CItemCode)];
                dgvItems.Focus();
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            sqlControl rs = new sqlControl();
            bool TransactionStarted = false;

            try
            {

                Cursor.Current = Cursors.WaitCursor;

                try
                {
                    if (ConvertI32(clsVchType.TransactionNumberingValue) == 0) // Auto Locked
                    {
                        if (iIDFromEditWindow == 0) //New
                        {
                            txtInvAutoNo.Text = Comm.gfnGetNextSerialNo("tblBoardRateMaster", "INVNO", " VchTypeID=" + vchtypeID).ToString();
                            txtInvAutoNo.Tag = 0;
                        }
                    }
                    else if (ConvertI32(clsVchType.TransactionNumberingValue) == 1) // Auto Editable
                    {
                        if (iIDFromEditWindow == 0) //New
                        {
                            txtInvAutoNo.Text = Comm.gfnGetNextSerialNo("tblBoardRateMaster", "INVNO", " VchTypeID=" + vchtypeID).ToString();
                            txtInvAutoNo.Tag = 0;
                        }
                    }
                    else
                    {
                        txtInvAutoNo.Tag = 0;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                string FileName = "";

                if (cboMachineModel.Text == "") cboMachineModel.Text = "DEFAULTMACHINE";
                if (clsVchType.BoardRateFileName == null) clsVchType.BoardRateFileName = "";
                if (clsVchType.BoardRateFileName.Trim() == "")
                {
                    FileName = cboMachineModel.Text.Replace(".", "").Replace(@"\", "").Replace(" / ", "").Replace(" - ", "").Replace(" ", "") + ".csv";
                    FileName = @"C:\DIGIDATA\" + FileName;
                }
                else
                    FileName = clsVchType.BoardRateFileName;

                string Query = clsVchType.BoardRateQuery;
                if (Query == null) Query = "";
                if (Query.Trim() == "")
                    Query = "SELECT TOP (1000) [ItemID] ,[ItemCode] ,[ItemName] ,[BatchUnique] ,[PLUNumber] ,[MRP] ,[SRATE1] ,[SRATE2] ,[SRATE3] ,[SRATE4] ,[SRATE5] ,[Unit] ,[Expiry]   FROM  [vwBoardRatePLU] ";


                rs.BeginTrans = true;
                TransactionStarted = true;

                rs.Execute("Update  tblWMDetails set BlnSkipHeader=" + ((chkSkipHeader.Checked == true) ? 1 : 0).ToString() + ",  SqlQuery = '" + Query.Replace("'", "''") + "',CSVPath ='" + FileName + "',ExportFileType='" + clsVchType.BoardRateExportType + "' ,ExportFileFormat='" + clsVchType.BoardRateExportType + "'  where ModelName ='" + cboMachineModel.Text + "'");
                long RecCountW = rs.RecordCount;
                if (RecCountW == 0)
                {
                    rs.Execute("Insert into tblWMDetails (ModelName,SqlQuery,CSVPath,ExportFileType,ExportFileFormat,BlnSkipHeader) values ('" + cboMachineModel.Text + "', '" + clsVchType.BoardRateQuery.Replace("'", "''") + "','" + FileName + "','" + clsVchType.BoardRateExportType + "','" + clsVchType.BoardRateExportType + "'," + ((chkSkipHeader.Checked == true) ? 1 : 0).ToString() + ") ");
                    cboMachineModel.Tag = Comm.fnGetData("Select ID From tblWMDetails where ModelName ='" + cboMachineModel.Text + "'");
                }

                //if (iIDFromEditWindow == 0)
                //    CRUD_Operations(0);

                int MaxID;
                int WMID;
                long RecordCount = 0;
                long RecCount = 0;

                if (iIDFromEditWindow != 0)
                {
                    MaxID = Comm.ToInt32(txtInvAutoNo.Tag);
                    WMID = Comm.ToInt32(cboMachineModel.Tag.ToString());
                    if (MaxID == 0)
                        MaxID = Comm.gfnGetNextSerialNo("tblBoardRateMaster", "InvID");
                    rs.Execute("update tblBoardRateMaster set InvNo=" + Comm.ToInt32(txtInvAutoNo.Text) + ",InvDate='" + dtpInvDate.Value.ToString("dd/MMM/yyyy") + "',InvTime='" + DateTime.Now.ToString("HH:mm:ss") + "',CCID=1, MachineModel = '" + cboMachineModel.Text + "', MachineModelId = " + Comm.ToInt32(cboMachineModel.SelectedValue) + ", DisplayRate = " + cboDisplayRate.SelectedValue + " Where InvId=" + MaxID);
                    RecordCount = rs.RecordCount;
                    RecCount = RecCount + RecordCount;
                    if ((RecCount > 0))
                    {
                        rs.Execute("delete from tblBoardRateDetail where InvId=" + MaxID);
                        RecordCount = rs.RecordCount;
                        RecCount = RecCount + RecordCount;
                    }
                    if (RecCount == 0)
                    {
                        Interaction.MsgBox("No Data is Updated", MsgBoxStyle.Critical);
                        return;
                    }
                }
                else
                {
                    MaxID = Comm.gfnGetNextSerialNo("tblBoardRateMaster", "InvID");
                    WMID = Comm.ToInt32(Comm.GetTableValue("tblWMDetails", "ID", " Where ModelName = '" + cboMachineModel.Text + "'"));
                    rs.Execute("insert into tblBoardRateMaster(InvID,InvNo,InvDate,InvTime,CCID,MachineModel,VchTypeID,MachineModelID,DisplayRate,TenantID)values(" + MaxID + "," + Comm.ToInt32(txtInvAutoNo.Text) + ",'" + dtpInvDate.Value.ToString("dd/MMM/yyyy") + "','" + DateTime.Now.ToString("HH:mm:ss") + "',1,'" + cboMachineModel.Text + "'," + vchtypeID + ", " + Conversion.Val(WMID) + "," + cboDisplayRate.SelectedIndex + "," + Global.gblTenantID + ")");
                    RecordCount = rs.RecordCount;
                    RecCount = RecCount + RecordCount;
                    if (RecCount == 0)
                    {
                        Interaction.MsgBox("Please Save again", MsgBoxStyle.Critical);
                        return;
                    }
                }
                sqlControl cn = new sqlControl();
                if ((RecCount > 0))
                {
                    for (var i = 0; i <= dgvItems.RowCount - 2; i++)
                    {
                        if (dgvItems["cItemID", i] != null)
                        {
                            if (dgvItems["cItemID", i].Value != null)
                            {
                                rs.Execute("insert into tblBoardRateDetail(InvID,ItemID,BatchUnique,MRP,Srate1,Srate2,Srate3,Srate4,Srate5)values(" + MaxID + "," + Comm.ToInt32(dgvItems["cItemID", i].Value.ToString()) + ",'" + dgvItems["CBatchUnique", i].Value.ToString() + "'," + Comm.ToDouble(dgvItems["cMRP", i].Value) + "," + Comm.ToDouble(dgvItems["cSrate1", i].Value) + "," + Comm.ToDouble(dgvItems["cSrate2", i].Value) + "," + Comm.ToDouble(dgvItems["cSrate3", i].Value) + "," + Comm.ToDouble(dgvItems["cSrate4", i].Value) + "," + Comm.ToDouble(dgvItems["cSrate5", i].Value) + ")");

                                cn.Execute("   Update [dbo].[tblStock]    SET        [MRP] =" + Comm.ToDouble(dgvItems["cMRP", i].Value) + ",[Srate1] =" + Comm.ToDouble(dgvItems["cSrate1", i].Value) + ",[Srate2] = " + Comm.ToDouble(dgvItems["cSrate2", i].Value) + ",[Srate3] = " + Comm.ToDouble(dgvItems["cSrate3", i].Value) + "   ,[Srate4] = " + Comm.ToDouble(dgvItems["cSrate4", i].Value) + " ,[Srate5] = " + Comm.ToDouble(dgvItems["cSrate5", i].Value) + "   WHERE [BatchUnique] = '" + dgvItems["CBatchUnique", i].Value.ToString().Replace("'", "''") + "' and [ItemID] = " + Comm.Val(dgvItems["cItemID", i].Value.ToString()));

                                RecordCount = rs.RecordCount;
                                RecCount = RecCount + RecordCount;
                            }
                        }
                    }
                }
                if (RecCount > 0)
                {
                    rs.CommitTrans = true;
                    TransactionStarted = false;

                    ExportFile(cboMachineModel.Text,FileName,Query);
                }
                else
                {
                    Cursor.Current = Cursors.Default;
                    MessageBox.Show("No data Saved", "Board Rate Updator", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    rs.RollbackTrans = true;
                    return;
                }
                if (RecCount == 0)
                {
                    MessageBox.Show("No data Saved", "Board Rate Updator", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    if (TransactionStarted == true)
                        rs.RollbackTrans = true;
                    return;
                }

                ClearControls();

                GridInitialize_dgvColWidth();
                try
                {
                    LoadGridWidthFromItemGrid();
                    DisableGridSettingsCheckbox();

                    GridSettingsEnableDisable(Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active);
                }
                catch (Exception ex)
                {

                }

                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);

                if (TransactionStarted == true)
                    rs.RollbackTrans = true;
            }
            finally
            {
                rs.Dispose();
                Cursor.Current = Cursors.Default;
            }
        }

        private void ExportFile(string MachineModel, string FileName, string BoardRateQuery)
        {
            try
            {
                this.Cursor = Cursors.AppStarting;
                createCSV(BoardRateQuery, FileName, "", chkSkipHeader.Checked);

                //Comm.MessageboxToasted("Board Rate Update", "Exported to csv file " + FileName);
                MessageBox.Show("Exported to csv file " + FileName, "Board Rate Update", MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void createCSV(string SQLSTRing, string strcsvpath, string SumFields = "", bool blnSkipHeader = false)
        {
            sqlControl Rs = new sqlControl();
            int i;
            int No;
            string strLine = "";
            string fileExcel;
            FileStream objFileStream;
            StreamWriter objStreamWriter;
            // Create a pseudo-random file name.
            fileExcel = strcsvpath; // "c:\temp\test.xls"

            // Use FileStream to create the .xls file.
            objFileStream = new FileStream(fileExcel, FileMode.OpenOrCreate, FileAccess.Write);
            objStreamWriter = new StreamWriter(objFileStream);

            // Use a DataReader to connect to the Pubs database.
            // cnn.Open()
            string sql = SQLSTRing;
            Rs.Open(sql);


            No = Rs.FieldCount() - 1;
            string[] StrSumFields = new string[No + 1];

            // Enumerate the field names and records that are used to build the file.
            if (blnSkipHeader == false)
            {
                for (i = 0; i <= Rs.FieldCount() - 1; i++)
                {
                    strLine = strLine + "";
                    strLine = strLine + Rs.FieldName(i).ToString();
                    strLine = strLine + "";
                    strLine = strLine + ",";
                    if (SumFields != "")
                    {
                        if (SumFields.Contains(Rs.FieldName(i).ToString()) == true)
                            StrSumFields[i] = "0";
                    }
                }
                // Write the field name information to file.
                objStreamWriter.WriteLine(strLine);
            }


            // Reinitialize the string for data.
            strLine = "";

            // Enumerate the database that is used to populate the file.
            while (!Rs.eof())
            {
                // If Rs.fields("ItemCode").Contains("AFGHAN") Then
                // 'MsgBox("X")
                // End If
                for (i = 0; i <= Rs.FieldCount() - 1; i++)
                {
                    if (SumFields != "")
                    {
                        if (StrSumFields[i] != "")
                            StrSumFields[i] = (System.Convert.ToDouble(StrSumFields[i]) + System.Convert.ToDouble(Rs.fields(i))).ToString();
                    }

                    // strLine = strLine & Replace(dr.GetValue(i) & "", ",", " ") & ","

                    strLine = strLine + "";
                    strLine = strLine + Rs.fields(i) + "";
                    strLine = strLine + "";
                    strLine = strLine + ",";
                }
                objStreamWriter.WriteLine(strLine);
                strLine = "";
                Rs.MoveNext();
            }

            if (SumFields != "")
            {
                objStreamWriter.WriteLine(strLine);

                for (i = 0; i <= No; i++)
                {
                    strLine = strLine + "\"";
                    strLine = strLine + StrSumFields[i];
                    strLine = strLine + "\"";
                    strLine = strLine + ",";
                }
                objStreamWriter.WriteLine(strLine);
            }
            // Clean up.

            Rs.Close();
            // mycn.Close()
            objStreamWriter.Close();
            objFileStream.Close();
        }

        private void txtInstantReceipt_KeyDown(object sender, KeyEventArgs e)
        {
                btnSave.Focus();
        }

        private void tableLayoutPanel2_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragging)
            {
                this.Location = new Point(Cursor.Position.X - xOffset, Cursor.Position.Y - yOffset);
                this.Update();
            }
        }

        private void tableLayoutPanel2_MouseDown(object sender, MouseEventArgs e)
        {
            dragging = true;
            xOffset = Cursor.Position.X - this.Location.X;
            yOffset = Cursor.Position.Y - this.Location.Y;
        }

        private void btnprev_Click(object sender, EventArgs e)
        {
            btnShow.Visible = false;
            btnSave.Enabled = false;
            btnNext.Enabled = true;
            if (txtInvAutoNo.Tag.ToString() == "0")
            {
                if (dgvItems.Rows.Count > 0)
                {
                    if (dgvItems.Rows[0].Cells[GetEnum(gridColIndexes.CItemCode)].Value != null)
                    {

                        DialogResult dlgResult = MessageBox.Show("An Unsaved Voucher is Pending. Invoice Navigation will clear the unsaved Voucher. Do you want to proceed any way ?", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                        if (dlgResult == DialogResult.Yes)
                        {
                           

                            PreVNext(true);
                        }
                        else
                        {

                            btnShow.Visible = true;
                            btnSave.Enabled = true;

                        }
                    }
                    else
                        PreVNext(true);
                }
                else
                    PreVNext(true);
            }
            else
                PreVNext(true);
            //if (txtInvAutoNo.Tag.ToString() == "0")
            //{
            //    if (dgvItems.Rows.Count > 0)
            //    {
            //        if (dgvItems.Rows[0].Cells[GetEnum(gridColIndexes.CItemCode)].Value != null)
            //        {
            //            DialogResult dlgResult = MessageBox.Show("An Unsaved Voucher is Pending. Invoice Navigation will clear the unsaved Voucher. Do you want to proceed any way ?", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            //            if (dlgResult == DialogResult.Yes)
            //            {
            //                PreVNext(true);
            //            }
            //        }
            //        else
            //            PreVNext(true);
            //    }
            //    else
            //        PreVNext(true);
            //}
            //else
            //    PreVNext(true);
        }

        private void dtpInvDate_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
            {
                if (txtInvAutoNo.Visible == true)
                    txtInvAutoNo.Focus();

            }
            else if (e.KeyCode == Keys.Enter)
            {
            }
        }

        private void txtNarration_Leave(object sender, EventArgs e)
        {
            
        }

        private void txtNarration_Enter(object sender, EventArgs e)
        {
            
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            frmVouchertype frmV = new frmVouchertype(vchtypeID, false, true);
            frmV.StartPosition = FormStartPosition.CenterScreen;
            frmV.ShowDialog();
        }

        private void txtInvAutoNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
            {

            }
            else if (e.KeyCode == Keys.Enter)
            {
                dtpInvDate.Focus();
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            //PreVNext(false);
            if (txtInvAutoNo.Tag.ToString() == "0")
            {
                if (dgvItems.Rows.Count > 0)
                {
                    if (dgvItems.Rows[0].Cells[GetEnum(gridColIndexes.CItemCode)].Value != null)
                    {
                        DialogResult dlgResult = MessageBox.Show("An Unsaved Voucher is Pending. Invoice Navigation will clear the unsaved Voucher. Do you want to proceed any way ?", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                        if (dlgResult == DialogResult.Yes)
                        {
                            PreVNext(false);
                        }
                    }
                }
            }
            else
                PreVNext(false);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                //DialogResult dlgResult = MessageBox.Show("Are you sure to delete Invoice No [" + txtInvAutoNo.Text + "] Permanently ?", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                //if (dlgResult == DialogResult.Yes)
                //{
                //    CRUD_Operations(2);
                //}
                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to Delete" + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private void frmStockInVoucherNew_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                if (e.KeyCode == Keys.F3)
                {

                }
                else if (e.KeyCode == Keys.F5)
                {
                    Cursor.Current = Cursors.WaitCursor;
                    if (iIDFromEditWindow == 0)
                        btnSave.PerformClick();

                        //CRUD_Operations(0);
                    //else
                        //CRUD_Operations(1);

                    Cursor.Current = Cursors.Default;

                }
                else if (e.KeyCode == Keys.F7)
                {
                    try
                    {
                        //Cursor.Current = Cursors.WaitCursor;
                        //DialogResult dlgResult = MessageBox.Show("Are you sure to delete Item Code [" + strSelectedLedgerName + "] Permanently ?", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                        //if (dlgResult == DialogResult.Yes)
                        //{
                        //    CRUD_Operations(2);
                        //}
                        //Cursor.Current = Cursors.Default;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    finally
                    {
                        Cursor.Current = Cursors.Default;
                    }
                }
                else if (e.KeyCode == Keys.Escape)
                {
                    if (iIDFromEditWindow == 0)
                    {
                        if (dgvItems.Rows.Count > 0)
                        {
                            if (dgvItems.Rows[0].Cells[GetEnum(gridColIndexes.CItemCode)].Value != null)
                            {
                                DialogResult dlgResult = MessageBox.Show("Your are in the middle of an Entry. Do you want to exit?", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                                if (dlgResult.Equals(DialogResult.Yes))
                                    this.Close();
                            }
                            else
                                this.Close();
                        }
                        else
                            this.Close();
                    }
                    else
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

        private void tableLayoutPanel2_MouseUp(object sender, MouseEventArgs e)
        {
            dragging = false;
        }

        private void dtpEffective_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
            {
                dtpInvDate.Focus();
            }
            else if (e.KeyCode == Keys.Enter)
            {
                if (clsVchTypeFeatures.blnShowReferenceNo == true)
                {
                    if (cboSalesStaff.Enabled == true)
                    {
                        cboSalesStaff.Focus();
                        SendKeys.Send("{F4}");
                    }
                    else
                    {
                        dgvItems.CurrentCell = dgvItems.Rows[0].Cells[GetEnum(gridColIndexes.CItemCode)];
                        dgvItems.Focus();
                    }
                }
                else
                {
                    if (cboSalesStaff.Enabled == true)
                    {
                        cboSalesStaff.Focus();
                        SendKeys.Send("{F4}");
                    }
                    else
                    {
                        dgvItems.CurrentCell = dgvItems.Rows[0].Cells[GetEnum(gridColIndexes.CItemCode)];
                        dgvItems.Focus();
                    }
                }
            }
        }

        private void txtPrefix_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txtInvAutoNo.Focus();
            }
        }


        private void txtInvAutoNo_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtInvAutoNo, true);
            if (txtInvAutoNo.Tag == null) txtInvAutoNo.Tag = 0;
            if (Convert.ToInt32(PrefixTag.ToString()) == 3)
            {
                MessageBox.Show("This is a Archived Voucher", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void txtInvAutoNo_Leave(object sender, EventArgs e)
        {
            try
            {

            Comm.ControlEnterLeave(txtInvAutoNo);
            if (iIDFromEditWindow == 0)
            {
                DataTable dtInv = Comm.fnGetData("SELECT * FROM tblBoardRateMaster WHERE InvNo = '" + txtInvAutoNo.Text + "' AND VchTypeID=" + vchtypeID + " AND TenantID = " + Global.gblTenantID + "").Tables[0];
                if (dtInv.Rows.Count > 0)
                {
                    DialogResult dlgResult = MessageBox.Show("There is an Exisiting Bill Number in this Invoice No [" + txtInvAutoNo.Text + "], Do you want to load it?", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                    if (dlgResult == DialogResult.Yes)
                    {
                        LoadData(Convert.ToInt32(dtInv.Rows[0]["InvId"].ToString()));
                        iIDFromEditWindow = Convert.ToInt32(dtInv.Rows[0]["InvId"].ToString());
                    }
                    else
                    {
                        txtInvAutoNo.Clear();
                        txtInvAutoNo.Tag = 0;
                        txtInvAutoNo.Focus();
                    }
                }
            }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
        }

        private void btnArchive_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                //DialogResult dlgResult = MessageBox.Show("Are you sure to Archive the Bill ? Invoice No [" + txtInvAutoNo.Text + "].", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                //if (dlgResult == DialogResult.Yes)
                //{
                //    CRUD_Operations(3);
                //}
                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to Archive" + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private void dtpInvDate_ValueChanged(object sender, EventArgs e)
        {
            //dtpEffective.Value = dtpInvDate.Value;
        }

        private void btnMenu_Click(object sender, EventArgs e)
        {
            if (this.tlpMain.ColumnStyles[1].Width == 0)
                this.tlpMain.ColumnStyles[1].Width = 260;
            else
                this.tlpMain.ColumnStyles[1].Width = 0;
        }

        private void dgvColWidth_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                ValidateWidth_dgvColWidth(e.RowIndex);

                if (toggleWidthSettings.ToggleState == Syncfusion.Windows.Forms.Tools.ToggleButtonState.Inactive)
                {
                    for (int i = 0; i < dgvColWidth.Rows.Count; i++)
                    {
                        if (dgvItems.Columns[i].Name == dgvColWidth.Rows[i].Cells[3].Value.ToString())
                        {
                            dgvItems.Columns[i].Width = Convert.ToInt32(dgvColWidth.Rows[i].Cells[2].Value.ToString());
                            if (dgvColWidth.Rows[i].Cells[0].Value.ToString() == "")
                                dgvItems.Columns[i].Visible = false;
                            else
                                dgvItems.Columns[i].Visible = Convert.ToBoolean(dgvColWidth.Rows[i].Cells[0].Value);
                        }
                        if (dgvItems.Columns[i].Name == "cRateinclusive")
                            dgvItems.Columns[i].Visible = false;

                    }
                }
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void lblQuickSettingsClose_Click(object sender, EventArgs e)
        {
            this.tlpMain.ColumnStyles[1].Width = 0;
        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        #endregion

        #region "METHODS ----------------------------------------------- >>"

        // Description : Works when click on Previous/Next Invoice Buttons
        private void PreVNext(bool bIsPrev = true)
        {
            try
            { 
            DataTable dtInv = new DataTable();
            decimal dInvId = 0;
            
            //btnNext.Enabled = true;
            //btnprev.Enabled = true;

            if (txtInvAutoNo.Tag.ToString () != "")
            {
                if (bIsPrev == true)
                {
                    if (txtInvAutoNo.Tag.ToString() == "0")
                    {
                        dtInv = Comm.fnGetData("SELECT TOP 1 ISNULL(InvId,0) FROM tblBoardRateMaster WHERE VchTypeID = " + vchtypeID + " AND TenantID = " + Global.gblTenantID + " ORDER BY InvId DESC").Tables[0];
                        if (dtInv.Rows.Count > 0)
                            dInvId = Convert.ToDecimal(dtInv.Rows[0][0].ToString());
                        else
                            dInvId = 0;
                    }
                    else
                    {
                        dtInv = Comm.fnGetData("SELECT TOP 1 ISNULL(InvId,0) FROM tblBoardRateMaster WHERE InvId < " + Convert.ToDecimal(txtInvAutoNo.Tag.ToString()) + " AND VchTypeID = " + vchtypeID + " AND TenantID = " + Global.gblTenantID + " ORDER BY InvId DESC").Tables[0];
                        if (dtInv.Rows.Count > 0)
                            dInvId = Convert.ToDecimal(dtInv.Rows[0][0].ToString());
                        else
                            dInvId = 0;
                    }

                    if (dInvId == 0)
                    {
                        iIDFromEditWindow = 0;
                        btnprev.Enabled = false;
                       
                    }
                    else
                    {
                        iIDFromEditWindow = Convert.ToInt32(dInvId);
                        LoadData(Convert.ToInt32(dInvId));
                        btnprev.Enabled = true;
                    }
                }
                else //Next
                {
                    if (txtInvAutoNo.Tag.ToString() != "0")
                    {
                        dtInv = Comm.fnGetData("SELECT TOP 1 ISNULL(InvId,0) FROM tblBoardRateMaster WHERE InvId > " + Convert.ToDecimal(txtInvAutoNo.Tag.ToString()) + " AND VchTypeID = " + vchtypeID + " AND TenantID = " + Global.gblTenantID + " ORDER BY InvId ASC").Tables[0];
                        if (dtInv.Rows.Count > 0)
                            dInvId = Convert.ToDecimal(dtInv.Rows[0][0].ToString());
                        else
                        {
                            dInvId = 0;
                            ClearControls();
                            if (ConvertI32(clsVchType.TransactionNumberingValue) == 0) // Auto Locked
                            {
                                txtInvAutoNo.Text = Comm.gfnGetNextSerialNo("tblBoardRateMaster", "INVNO", " VchTypeID=" + vchtypeID).ToString();
                                txtInvAutoNo.Tag = 0;

                                txtInvAutoNo.ReadOnly = true;
                            }
                            else if (ConvertI32(clsVchType.TransactionNumberingValue) == 1) // Auto Editable
                            {
                                txtInvAutoNo.Text = Comm.gfnGetNextSerialNo("tblBoardRateMaster", "INVNO", " VchTypeID=" + vchtypeID).ToString();
                                txtInvAutoNo.Tag = 0;

                                txtInvAutoNo.ReadOnly = false;
                            }
                            else
                            {
                                txtInvAutoNo.Tag = 0;
                                txtInvAutoNo.ReadOnly = false;
                            }
                        }

                        if (dInvId == 0)
                        {
                            btnNext.Enabled = false;
                           
                            btnShow.Visible = true;
                            btnSave.Enabled = true;
                        }
                        else
                        {
                            btnNext.Enabled = true;
                            LoadData(Convert.ToInt32(dInvId));
                        }
                    }
                    else
                        btnNext.Enabled = false;
                   
                }
            }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
        }

        //Description : Get Employee Details from Database
        public DataTable GetEmployee(int iSelID = 0)
        {
            GetEmpInfo.EmpID = iSelID;
            GetEmpInfo.TenantID = Global.gblTenantID;
            GetEmpInfo.blnSalesStaff = true;
            return clsEmp.GetEmployee(GetEmpInfo);
        }
        
        //Description: Fill Employee Details from GetEmployee Method to Combobox
        private void FillEmployee(int iSelID = 0)
        {
            DataTable dtEmp = new DataTable();
            dtEmp = GetEmployee(0);
            if (dtEmp.Rows.Count > 0)
            {
                Comm.LoadControl(cboSalesStaff, dtEmp, "", false, false, "Name", "EmpID");
                if (iSelID != 0)
                    cboSalesStaff.SelectedValue = iSelID;
            }
        }

        //Description: Get Cost Centre Details from the Database
        public DataTable GetCostCentre(int iSelID = 0)
        {
            GetCctinfo.CCID = iSelID;
            GetCctinfo.TenantID = Global.gblTenantID;
            return clscct.GetCostCentre(GetCctinfo);
        }

        //Description : Seting Value to the Given Column, Row Indexes to Grid Cell
        private void SetValue(int iCellIndex, string sValue, string sConvertType = "")
        {
            try
            { 
            if (dgvItems.Rows.Count > 0)
            {
                if (sConvertType.ToUpper() == "CURR_FLOAT")
                {
                    if (sValue == "") sValue = "0";
                    //Commented and Added By Dipu on 17-Feb-2022 ------------- >>
                    dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[iCellIndex].Value = Comm.chkChangeValuetoZero(FormatValue(Convert.ToDouble(sValue)));
                    //Commented and Added By Dipu on 17-Feb-2022 ------------- >>

                    this.dgvItems.Columns[dgvItems.CurrentCell.ColumnIndex].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
                else if (sConvertType.ToUpper() == "QTY_FLOAT")
                {
                    if (sValue == "") sValue = "0";
                    dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[iCellIndex].Value = Comm.chkChangeValuetoZero(FormatValue(Convert.ToDouble(sValue), false));
                    this.dgvItems.Columns[dgvItems.CurrentCell.ColumnIndex].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
                else if (sConvertType.ToUpper() == "PERC_FLOAT")
                {
                    if (sValue == "") sValue = "0";
                    dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[iCellIndex].Value = Comm.chkChangeValuetoZero(Convert.ToDecimal(sValue).ToString("#.00"));
                    this.dgvItems.Columns[dgvItems.CurrentCell.ColumnIndex].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
                else if (sConvertType.ToUpper() == "DATE")
                {
                    dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[iCellIndex].Value = Convert.ToDateTime(sValue).ToString("dd/MMM/yyyy");
                }
                else
                    dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[iCellIndex].Value = Comm.chkChangeValuetoZero(sValue);
            }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
        }

        //Description : Setting Value to Tag of the cells of Grid using the Given Column and Row Indexes
        private void setTag(int iCellIndex, string sTag, string sConvertType = "")
        {
            try
            { 
            if (sConvertType.ToUpper() == "FLOAT")
            {
                if (sTag == "") sTag = "0";
                dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[iCellIndex].Tag = Convert.ToDecimal(sTag).ToString("#.00");
            }
            else if (sConvertType.ToUpper() == "DATE")
            {
                dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[iCellIndex].Tag = Convert.ToDateTime(sTag).ToString("dd/MMM/yyyy");
            }
            else
                dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[iCellIndex].Tag = sTag;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
        }

        //Description : Function Polymorphism for Seting the Value to Grid Asper Parameters Given
        private void SetValue(int iCellIndex, int iRowIndex, string sValue, string sConvertType = "")
        {
            try
            {

            //if(sConvertType.ToUpper() == "QTY")
            //    dgvItems.Rows[iRowIndex].Cells[iCellIndex].Value = Comm.chkChangeValuetoZero(FormatValue(Convert.ToDouble(sValue),false));
            //else
                dgvItems.Rows[iRowIndex].Cells[iCellIndex].Value = Comm.chkChangeValuetoZero(sValue);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
        }

        //Description : Validating the Method with Before Save Functionality
        private bool IsValidate()
        {
            try
            { 
            DataTable dtInv = new DataTable();
            bool bValidate = true;

            if (txtInvAutoNo.Text == "")
            {
                bValidate = false;
                MessageBox.Show("Please Enter the Invoice No.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                txtInvAutoNo.Focus();
                goto FailsHere;
            }
            else if (Convert.ToString(dgvItems.Rows[0].Cells[GetEnum(gridColIndexes.CItemCode)].Value) == "")
            {
                bValidate = false;
                MessageBox.Show("No Items are selected for Save. Please select a Items", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                goto FailsHere;
            }
            else if (cboSalesStaff.SelectedIndex < 0)
            {
                bValidate = false;
                MessageBox.Show("Please select an employee.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                goto FailsHere;
            }
            else
            {

            }
            if (txtInvAutoNo.Text.Trim() != "")
            {
                if (iIDFromEditWindow == 0)
                {
                    dtInv = Comm.fnGetData("SELECT InvNo FROM tblBoardRateMaster WHERE vchtypeid=" + vchtypeID + " and LTRIM(RTRIM(InvNo)) = '" + txtInvAutoNo.Text.Trim() + "'").Tables[0];
                    if (dtInv.Rows.Count > 0)
                    {
                        bValidate = false;
                        MessageBox.Show("Could not allow to enter Duplicate Invoice No. Please Try Another No.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        txtInvAutoNo.Clear();
                        txtInvAutoNo.Focus();
                        goto FailsHere;
                    }
                }
            }
            for (int i = 0; i < dgvItems.Rows.Count; i++)
            {
                if (dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cMRP)].Value == null)
                    dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cMRP)].Value = "0";
                if (dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cMRP)].Value.ToString().Trim() == "")
                    dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cMRP)].Value = "0";
            }

        FailsHere:
            return bValidate;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
        }

        //Description : Get Whole data from Employee Master and return to Array
        private string[] GetEmpDetails(decimal dEmpID = 0)
        {
            try
            { 
            if (dEmpID != 0)
            {
                List<string> lstEmp = new List<string>();
                DataTable dtEmp = new DataTable();
                dtEmp = Comm.fnGetData("SELECT * FROM tblEmployee WHERE EmpID = " + dEmpID + "").Tables[0];
                if (dtEmp.Rows.Count > 0)
                {
                    lstEmp.Add(Comm.CheckDBNullOrEmpty(dtEmp.Rows[0][GetEnumEmp(EmpIndexes.EmpID)].ToString()));
                    lstEmp.Add(Comm.CheckDBNullOrEmpty(dtEmp.Rows[0][GetEnumEmp(EmpIndexes.Name)].ToString()));
                    lstEmp.Add(Comm.CheckDBNullOrEmpty(dtEmp.Rows[0][GetEnumEmp(EmpIndexes.Address)].ToString()));
                    lstEmp.Add(Comm.CheckDBNullOrEmpty(dtEmp.Rows[0][GetEnumEmp(EmpIndexes.NameOfFather)].ToString()));
                    lstEmp.Add(Comm.CheckDBNullOrEmpty(dtEmp.Rows[0][GetEnumEmp(EmpIndexes.PhNo)].ToString()));
                    lstEmp.Add(Comm.CheckDBNullOrEmpty(dtEmp.Rows[0][GetEnumEmp(EmpIndexes.MaritialStatus)].ToString()));
                    lstEmp.Add(Comm.CheckDBNullOrEmpty(dtEmp.Rows[0][GetEnumEmp(EmpIndexes.NoOfFamilyMembers)].ToString()));
                    lstEmp.Add(Comm.CheckDBNullOrEmpty(dtEmp.Rows[0][GetEnumEmp(EmpIndexes.NameOFNominee)].ToString()));
                    lstEmp.Add(Comm.CheckDBNullOrEmpty(dtEmp.Rows[0][GetEnumEmp(EmpIndexes.Spouse)].ToString()));
                    lstEmp.Add(Comm.CheckDBNullOrEmpty(dtEmp.Rows[0][GetEnumEmp(EmpIndexes.SpouseEmployed)].ToString()));
                    lstEmp.Add(Comm.CheckDBNullOrEmpty(dtEmp.Rows[0][GetEnumEmp(EmpIndexes.OwnerOfResidence)].ToString()));
                    lstEmp.Add(Comm.CheckDBNullOrEmpty(dtEmp.Rows[0][GetEnumEmp(EmpIndexes.PANNo)].ToString()));
                    lstEmp.Add(Comm.CheckDBNullOrEmpty(dtEmp.Rows[0][GetEnumEmp(EmpIndexes.BloodGroup)].ToString()));
                    lstEmp.Add(Comm.CheckDBNullOrEmpty(dtEmp.Rows[0][GetEnumEmp(EmpIndexes.Designation)].ToString()));
                    lstEmp.Add(Comm.CheckDBNullOrEmpty(dtEmp.Rows[0][GetEnumEmp(EmpIndexes.Qualification)].ToString()));
                    lstEmp.Add(Comm.CheckDBNullOrEmpty(dtEmp.Rows[0][GetEnumEmp(EmpIndexes.Sex)].ToString()));
                    lstEmp.Add(Comm.CheckDBNullOrEmpty(dtEmp.Rows[0][GetEnumEmp(EmpIndexes.DOB)].ToString()));
                    lstEmp.Add(Comm.CheckDBNullOrEmpty(dtEmp.Rows[0][GetEnumEmp(EmpIndexes.DOJ)].ToString()));
                    lstEmp.Add(Comm.CheckDBNullOrEmpty(dtEmp.Rows[0][GetEnumEmp(EmpIndexes.DOI)].ToString()));
                    lstEmp.Add(Comm.CheckDBNullOrEmpty(dtEmp.Rows[0][GetEnumEmp(EmpIndexes.PensionAccNo)].ToString()));
                    lstEmp.Add(Comm.CheckDBNullOrEmpty(dtEmp.Rows[0][GetEnumEmp(EmpIndexes.GPFAccNo)].ToString()));
                    lstEmp.Add(Comm.CheckDBNullOrEmpty(dtEmp.Rows[0][GetEnumEmp(EmpIndexes.GSLIAccNo)].ToString()));
                    lstEmp.Add(Comm.CheckDBNullOrEmpty(dtEmp.Rows[0][GetEnumEmp(EmpIndexes.LICPolicyNo)].ToString()));
                    lstEmp.Add(Comm.CheckDBNullOrEmpty(dtEmp.Rows[0][GetEnumEmp(EmpIndexes.LICMonthlyPremium)].ToString()));
                    lstEmp.Add(Comm.CheckDBNullOrEmpty(dtEmp.Rows[0][GetEnumEmp(EmpIndexes.LICDateofMaturity)].ToString()));
                    lstEmp.Add(Comm.CheckDBNullOrEmpty(dtEmp.Rows[0][GetEnumEmp(EmpIndexes.CategoryID)].ToString()));
                    lstEmp.Add(Comm.CheckDBNullOrEmpty(dtEmp.Rows[0][GetEnumEmp(EmpIndexes.DateofPromotion)].ToString()));
                    lstEmp.Add(Comm.CheckDBNullOrEmpty(dtEmp.Rows[0][GetEnumEmp(EmpIndexes.DateofRetirement)].ToString()));
                    lstEmp.Add(Comm.CheckDBNullOrEmpty(dtEmp.Rows[0][GetEnumEmp(EmpIndexes.GISAccNo)].ToString()));
                    lstEmp.Add(Comm.CheckDBNullOrEmpty(dtEmp.Rows[0][GetEnumEmp(EmpIndexes.BankAccNo)].ToString()));
                    lstEmp.Add(Comm.CheckDBNullOrEmpty(dtEmp.Rows[0][GetEnumEmp(EmpIndexes.Commission)].ToString()));
                    lstEmp.Add(Comm.CheckDBNullOrEmpty(dtEmp.Rows[0][GetEnumEmp(EmpIndexes.CommissionAmt)].ToString()));
                    lstEmp.Add(Comm.CheckDBNullOrEmpty(dtEmp.Rows[0][GetEnumEmp(EmpIndexes.EmpFname)].ToString()));
                    lstEmp.Add(Comm.CheckDBNullOrEmpty(dtEmp.Rows[0][GetEnumEmp(EmpIndexes.blnSalesStaff)].ToString()));
                    lstEmp.Add(Comm.CheckDBNullOrEmpty(dtEmp.Rows[0][GetEnumEmp(EmpIndexes.PhotoPath)].ToString()));
                    lstEmp.Add(Comm.CheckDBNullOrEmpty(dtEmp.Rows[0][GetEnumEmp(EmpIndexes.InsCompany)].ToString()));
                    lstEmp.Add(Comm.CheckDBNullOrEmpty(dtEmp.Rows[0][GetEnumEmp(EmpIndexes.CommissionCondition)].ToString()));
                    lstEmp.Add(Comm.CheckDBNullOrEmpty(dtEmp.Rows[0][GetEnumEmp(EmpIndexes.EmpCode)].ToString()));
                    lstEmp.Add(Comm.CheckDBNullOrEmpty(dtEmp.Rows[0][GetEnumEmp(EmpIndexes.blnStatus)].ToString()));
                    lstEmp.Add(Comm.CheckDBNullOrEmpty(dtEmp.Rows[0][GetEnumEmp(EmpIndexes.DrivingLicenceNo)].ToString()));
                    lstEmp.Add(Comm.CheckDBNullOrEmpty(dtEmp.Rows[0][GetEnumEmp(EmpIndexes.DrivingLicenceExpiry)].ToString()));
                    lstEmp.Add(Comm.CheckDBNullOrEmpty(dtEmp.Rows[0][GetEnumEmp(EmpIndexes.PassportNo)].ToString()));
                    lstEmp.Add(Comm.CheckDBNullOrEmpty(dtEmp.Rows[0][GetEnumEmp(EmpIndexes.PassportExpiry)].ToString()));
                    lstEmp.Add(Comm.CheckDBNullOrEmpty(dtEmp.Rows[0][GetEnumEmp(EmpIndexes.Active)].ToString()));
                    lstEmp.Add(Comm.CheckDBNullOrEmpty(dtEmp.Rows[0][GetEnumEmp(EmpIndexes.SortOrder)].ToString()));
                    lstEmp.Add(Comm.CheckDBNullOrEmpty(dtEmp.Rows[0][GetEnumEmp(EmpIndexes.EnrollNo)].ToString()));
                    lstEmp.Add(Comm.CheckDBNullOrEmpty(dtEmp.Rows[0][GetEnumEmp(EmpIndexes.TargetAmount)].ToString()));
                    lstEmp.Add(Comm.CheckDBNullOrEmpty(dtEmp.Rows[0][GetEnumEmp(EmpIndexes.IncentivePer)].ToString()));
                    lstEmp.Add(Comm.CheckDBNullOrEmpty(dtEmp.Rows[0][GetEnumEmp(EmpIndexes.PWD)].ToString()));
                    lstEmp.Add(Comm.CheckDBNullOrEmpty(dtEmp.Rows[0][GetEnumEmp(EmpIndexes.Holidays)].ToString()));
                    lstEmp.Add(Comm.CheckDBNullOrEmpty(dtEmp.Rows[0][GetEnumEmp(EmpIndexes.LID)].ToString()));
                    lstEmp.Add(Comm.CheckDBNullOrEmpty(dtEmp.Rows[0][GetEnumEmp(EmpIndexes.salarypermonth)].ToString()));
                    lstEmp.Add(Comm.CheckDBNullOrEmpty(dtEmp.Rows[0][GetEnumEmp(EmpIndexes.SystemName)].ToString()));
                    lstEmp.Add(Comm.CheckDBNullOrEmpty(dtEmp.Rows[0][GetEnumEmp(EmpIndexes.UserID)].ToString()));
                    lstEmp.Add(Comm.CheckDBNullOrEmpty(dtEmp.Rows[0][GetEnumEmp(EmpIndexes.LastUpdateDate)].ToString()));
                    lstEmp.Add(Comm.CheckDBNullOrEmpty(dtEmp.Rows[0][GetEnumEmp(EmpIndexes.LastUpdateTime)].ToString()));
                    lstEmp.Add(Comm.CheckDBNullOrEmpty(dtEmp.Rows[0][GetEnumEmp(EmpIndexes.TenantID)].ToString()));
                }
                return lstEmp.ToArray();
            }
            else
                return null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return null;
            }
        }

        //Description : Clear the Form and Grid 
        private void ClearControls()
        {
            try
            { 
            FillEmployee();

            SetTransactionDefaults();
            SetTransactionsthatVarying();
            SetApplicationSettings();

                if (dgvItems.Rows.Count > 0)
                    dgvItems.CurrentCell = dgvItems[1, 0];

                dgvItems.Refresh();
                Application.DoEvents();
                try
                {
                    dgvItems.Rows.Clear();
                    dgvItems.Refresh();
                    Application.DoEvents();

                }
                catch
                { }
            
                dgvItems.Refresh();
                Application.DoEvents();

                iIDFromEditWindow = 0;
                //AddColumnsToGrid();
                if (dgvItems.EditingControl != null)
                {
                    if (dgvItems.EditingControl.Visible == true)
                    {
                        dgvItems.EndEdit();
                    }
                }
            dgvItems.Rows.Add();

            //dSupplierID = 0;

            if (ConvertI32(clsVchType.TransactionNumberingValue) == 2) // Custom
                txtInvAutoNo.Clear();

            dgvItems.Columns["cSlNo"].Frozen = true;
           // dgvItems.Columns["cImgDel"].Frozen = true;
            dgvItems.Columns["cImgDel"].Visible = true;
            dgvItems.Columns["cImgDel"].Width = 40;

            txtInvAutoNo.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
        }

        //Description : Function Polymorphism of SetTag
        private void SetTag(int iCellIndex, int iRowIndex, string sTag, string sConvertType = "")
        {
            try
            { 
            if (sConvertType.ToUpper() == "PERC_FLOAT")
                dgvItems.Rows[iRowIndex].Cells[iCellIndex].Tag = Convert.ToDecimal(sTag).ToString("#.00");
            else
                dgvItems.Rows[iRowIndex].Cells[iCellIndex].Tag = sTag;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
        }

        //Description : Convert the Enum Members to Column index
        private int GetEnum(int ColIndexes)
        {
            return ColIndexes;
        }

        //Description : Convert the Employee Enum Members to Array Index
        private int GetEnumEmp(EmpIndexes EmpIndx)
        {
            return (int)EmpIndx;
        }
        private int GetEnumItem(ItemIndexes ItmIndex)
        {
            return (int)ItmIndex;
        }

        //Description : What to happen when Item is Select from the Grid Compact Search
        private Boolean GetFromItemSearch(string sReturn)
        {
            try
            {
                DataTable dtSupp = new DataTable();

                string[] sCompSearchData = sReturn.Split('|');
                List<decimal> lstItmDisc = new List<decimal>();
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
                            GetItmMststockinfo.StockID = Convert.ToInt32(sCompSearchData[0].ToString());
                            GetItmMststockinfo.TenantID = Global.gblTenantID;

                            dtItemPublic = clsItmMst.GetItemMasterFromStock(GetItmMststockinfo);

                            if (dtItemPublic.Rows.Count > 0)
                            {
                                SetValue(GetEnum(gridColIndexes.CItemCode), dtItemPublic.Rows[0]["ItemCode"].ToString());
                                SetValue(GetEnum(gridColIndexes.CItemName), dtItemPublic.Rows[0]["ItemName"].ToString());
                                SetValue(GetEnum(gridColIndexes.cItemID), dtItemPublic.Rows[0]["cItemID"].ToString());
                                setTag(GetEnum(gridColIndexes.CItemName), dtItemPublic.Rows[0]["cItemID"].ToString());
                                SetValue(GetEnum(gridColIndexes.CBarCode), dtItemPublic.Rows[0]["BatchCode"].ToString());
                                SetValue(GetEnum(gridColIndexes.CBatchUnique), dtItemPublic.Rows[0]["BatchUnique"].ToString());
                                
                                setTag(GetEnum(gridColIndexes.CUnit), dtItemPublic.Rows[0]["intnoorweight"].ToString());
                                if (dtItemPublic.Rows[0]["intnoorweight"].ToString() == "0")
                                    SetValue(GetEnum(gridColIndexes.CUnit), "KG");
                                else    
                                    SetValue(GetEnum(gridColIndexes.CUnit), "Nos");

                                SetValue(GetEnum(gridColIndexes.cPRate), dtItemPublic.Rows[0]["PRate"].ToString(), "CURR_FLOAT");

                                SetValue(GetEnum(gridColIndexes.cMRP), dtItemPublic.Rows[0]["MRP"].ToString(), "CURR_FLOAT");
                                SetValue(GetEnum(gridColIndexes.CExpiry), dtItemPublic.Rows[0]["Expiry"].ToString(), "DATE");

                                SetValue(GetEnum(gridColIndexes.cSRate1), dtItemPublic.Rows[0]["SRate1"].ToString(), "CURR_FLOAT");
                                SetValue(GetEnum(gridColIndexes.cSRate2), dtItemPublic.Rows[0]["SRate2"].ToString(), "CURR_FLOAT");
                                SetValue(GetEnum(gridColIndexes.cSRate3), dtItemPublic.Rows[0]["SRate3"].ToString(), "CURR_FLOAT");
                                SetValue(GetEnum(gridColIndexes.cSRate4), dtItemPublic.Rows[0]["SRate4"].ToString(), "CURR_FLOAT");
                                SetValue(GetEnum(gridColIndexes.cSRate5), dtItemPublic.Rows[0]["SRate5"].ToString(), "CURR_FLOAT");

                                if (dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cSRate1)].Visible)
                                    dgvItems.CurrentCell = dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cSRate1)];
                                dgvItems.Focus();

                                SetValue(GetEnum(gridColIndexes.CItemCode), dtItemPublic.Rows[0]["ItemCode"].ToString());

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

        //Description : Row Delete when Press Delete or Delete icon
        private void RowDelete()
        {
            int rowIndex = dgvItems.CurrentCell.RowIndex;
            dgvItems.Rows.RemoveAt(rowIndex);
            //decimal dinvid = GetPriceListInfo.InvId;
        }

        //Description : Initializing the Columns in The Grid
        private void AddColumnsToGrid()
        {
            try
            { 
            this.dgvItems.Columns.Clear();

            this.dgvItems.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSlNo", HeaderText = "Sl.No", Width = 50, ReadOnly = true }); //1

            this.dgvItems.Columns.Add(new DataGridViewTextBoxColumn() { Name = "CItemCode", HeaderText = "ItemCode", Width = 200, ReadOnly = true }); 
            this.dgvItems.Columns.Add(new DataGridViewTextBoxColumn() { Name = "CItemName", HeaderText = "ItemName", Width = 200, ReadOnly = true }); 
            this.dgvItems.Columns.Add(new DataGridViewTextBoxColumn() { Name = "CBarCode", HeaderText = "Barcode", Width = 130, ReadOnly = true }); 
            this.dgvItems.Columns.Add(new DataGridViewTextBoxColumn() { Name = "CBatchUnique", HeaderText = "BatchUnique", Width = 130, ReadOnly = true }); 
            this.dgvItems.Columns.Add(new DataGridViewTextBoxColumn() { Name = "CUnit", HeaderText = "Unit", Width = 130, ReadOnly = true }); 
            this.dgvItems.Columns.Add(new DataGridViewTextBoxColumn() { Name = "CExpiry", HeaderText = "Expiry", Width = 130, Visible = false, ReadOnly = true }); 
            this.dgvItems.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cMRP", HeaderText = "MRP", Width = 130}); 
            this.dgvItems.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cPRate", HeaderText = "PRate", Width = 130}); 
            this.dgvItems.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate1", HeaderText = AppSettings.SRate1Name, Width = 130});
            this.dgvItems.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate2", HeaderText = AppSettings.SRate2Name, Width = 130});
            this.dgvItems.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate3", HeaderText = AppSettings.SRate3Name, Width = 130});
            this.dgvItems.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate4", HeaderText = AppSettings.SRate4Name, Width = 130});
            this.dgvItems.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate5", HeaderText = AppSettings.SRate5Name, Width = 130});
            this.dgvItems.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cItemID", HeaderText = "ItemID", Width = 130, Visible = false });
            this.dgvItems.Columns.Add(new DataGridViewImageColumn() { Name = "cImgDel", HeaderText = "", Image = DigiposZen.Properties.Resources.Delete_24_P4, Width = 40 });

            dgvItems.Rows.Add(1);

            foreach (DataGridViewRow row in dgvItems.Rows)
            {
                dgvItems.Rows[row.Index].Cells[0].Value = string.Format("{0}  ", row.Index + 1).ToString();
            }

            foreach (DataGridViewColumn col in dgvItems.Columns)
            {
                col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
        }

        //Description : Initialize for Item Column Width Settings
        private void GridInitialize_dgvColWidth(bool bIsLoad = true)
        {
            try
            { 
            DataTable dtJson = new DataTable();
            string strJson = "";

            if (bIsLoad == true)
            {
                dtJson = Comm.fnGetData("SELECT ISNULL(GridSettingsJson,'') as GridSettingsJson FROM tblVchType WHERE VchTypeID = " + vchtypeID + "").Tables[0];
                if(dtJson.Rows.Count > 0)
                    strJson = dtJson.Rows[0][0].ToString();

                if (strJson != "")
                {
                    List<clsJsonPurGridSettingsInfo> lstJPDGSinfo_ = JsonConvert.DeserializeObject<List<clsJsonPurGridSettingsInfo>>(strJson);
                    DataTable dtGridSettings = lstJPDGSinfo_.ToDataTable();
                    if (dtGridSettings.Rows.Count > 0)
                    {
                        for (int k = 0; k < dtGridSettings.Rows.Count; k++)
                        {
                            if (dgvItems.Columns.Count > k)
                            {
                                if (dtGridSettings.Rows[k][3].ToString() == dgvItems.Columns[k].Name)
                                {
                                    dgvItems.Columns[k].Width = Convert.ToInt32(dtGridSettings.Rows[k][2].ToString());
                                    dgvItems.Columns[k].Visible = Convert.ToBoolean(dtGridSettings.Rows[k][0]);
                                }
                            }
                        }
                    }
                }
                //LoadGridWidthFromItemGrid();
            }
            else
            {
                //LoadGridWidthFromItemGrid();
                //SaveGridSettings();
            }
            
            dgvItems.Columns["cSlNo"].Frozen = true;
          //  dgvItems.Columns["cImgDel"].Frozen = true;
            dgvItems.Columns["cImgDel"].Visible = true;
            dgvItems.Columns["cImgDel"].Width = 40;

                //DisableGridSettingsCheckbox();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
        }

        private void flowLPnlBottom_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                frmEditWindow frmEdit = new frmEditWindow(this.Name.ToUpper(), this.MdiParent, "", vchtypeID, Comm.ToInt32(clsVchType.ParentID));
                frmEdit.Show();
                frmEdit.BringToFront();
                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to Find..." + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void dgvColWidth_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void btnSaveSettings_Click(object sender, EventArgs e)
        {
            try
            {
                SaveGridSettings();

            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
        }

        private void dgvItems_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (dgvItems.CurrentCell == null) return;

                if (dgvItems.CurrentCell.ColumnIndex == GetEnum(gridColIndexes.cSlNo))
                {
                    dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cSlNo)].ReadOnly = true;

                    strSelectedLedgerName = Convert.ToString(dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.CItemCode)].Value);
                }
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void dgvItems_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvItems.CurrentCell.ColumnIndex == GetEnum(gridColIndexes.cImgDel))
            {
                string SSelectedLedgerCode = Convert.ToString(dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.CItemCode)].Value);
                if (SSelectedLedgerCode != "")
                {
                    DialogResult dlgResult = MessageBox.Show("Are you sure to delete Item Code[" + SSelectedLedgerCode + "] Permanently ?", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                    if (dlgResult == DialogResult.Yes)
                    {

                        Int32 selectedRowCount = dgvItems.Rows.GetRowCount(DataGridViewElementStates.Selected);
                        RowDelete();

                        dgvItems.Rows.Add();
                        dgvItems.CurrentCell = dgvItems.Rows[0].Cells[GetEnum(gridColIndexes.CItemCode)];


                    }
                }
            }

        }

        private void dgvItems_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                string sQuery = "";
                Cursor.Current = Cursors.WaitCursor;
                double dSelectedLedgerID = 0;
                if (dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.CItemCode)].Value != null)
                {
                    dSelectedLedgerID = Convert.ToInt32(dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cItemID)].Value);
                    if (dSelectedLedgerID > 0)
                    {
                        

                            if (dgvItems.CurrentCell.ColumnIndex == (int)gridColIndexes.CItemName)
                            {
                                frmItemMaster frmIM = new frmItemMaster(Convert.ToInt32(dSelectedLedgerID), true);
                                frmIM.ShowDialog();
                            }
                            else if (dgvItems.CurrentCell.ColumnIndex == (int)gridColIndexes.CItemName)
                            {
                                frmItemMaster frmIM = new frmItemMaster(Convert.ToInt32(dSelectedLedgerID), true);
                                frmIM.ShowDialog();
                            }
                        
                    }
                }
                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private void dgvItems_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            decimal dResult = 0;
            try
            {
                if (dgvItems.CurrentCell.ColumnIndex == GetEnum(gridColIndexes.CExpiry))
                {
                    if (dgvItems.CurrentCell.Value != null)
                    {
                        if (dgvItems.CurrentCell.Value.ToString() != "")
                        {
                                if (dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cMRP)].Visible)
                                    dgvItems.CurrentCell = dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cMRP)];
                            dgvItems.Focus();
                        }
                    }
                }
                //else if (dgvItems.CurrentCell.ColumnIndex == GetEnum(gridColIndexes.cMRP))
                //{
                //    if (dgvItems.CurrentCell.Value != null)
                //    {
                //        if (dgvItems.CurrentCell.Value.ToString() != "")
                //        {
                //            if (dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cPRate)].Visible)
                //                dgvItems.CurrentCell = dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cPRate)];
                //            else if (dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cSRate1)].Visible)
                //                dgvItems.CurrentCell = dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cSRate1)];
                //            else if (dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cSRate2)].Visible)
                //                dgvItems.CurrentCell = dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cSRate2)];
                //            else if (dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cSRate3)].Visible)
                //                dgvItems.CurrentCell = dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cSRate3)];
                //            else if (dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cSRate4)].Visible)
                //                dgvItems.CurrentCell = dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cSRate4)];
                //            else if (dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cSRate5)].Visible)
                //                dgvItems.CurrentCell = dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cSRate5)];

                //            dgvItems.Focus();
                //            //dgvItems.BeginEdit(true);
                //        }
                //    }
                //}
                //else if (dgvItems.CurrentCell.ColumnIndex == GetEnum(gridColIndexes.cMRP))
                //{
                //    dResult = Convert.ToDecimal(dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cMRP)].Value);
                //    SetValue(GetEnum(gridColIndexes.cMRP), dResult.ToString(), "CURR_FLOAT");

                //    if (dgvItems.Rows.Count - 1 == dgvItems.CurrentRow.Index)
                //        dgvItems.Rows.Add();
                //    if (dResult != 0)
                //    {
                //        dgvItems.CurrentCell = dgvItems[GetEnum(gridColIndexes.cPRate), dgvItems.CurrentRow.Index];
                //    }
                //    else
                //        SendKeys.Send("{Tab}");
                //}
                //else if (dgvItems.CurrentCell.ColumnIndex == GetEnum(gridColIndexes.cSRate1) || dgvItems.CurrentCell.ColumnIndex == GetEnum(gridColIndexes.cSRate2) || dgvItems.CurrentCell.ColumnIndex == GetEnum(gridColIndexes.cSRate3) || dgvItems.CurrentCell.ColumnIndex == GetEnum(gridColIndexes.cSRate4) || dgvItems.CurrentCell.ColumnIndex == GetEnum(gridColIndexes.cSRate5))
                //{
                //    if (dgvItems.Rows.Count - 1 == dgvItems.CurrentRow.Index)
                //        dgvItems.Rows.Add();
                //}

                this.dgvEndEditCell = dgvItems[e.ColumnIndex, e.RowIndex];
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void dgvItems_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                //Added by Dipu Joseph on 14-Feb-2021 5.08 PM ---------- >>
                int iRow = 0;

                if (dgvItems.CurrentCell != null)
                {
                    int iColumn = dgvItems.CurrentCell.ColumnIndex;
                    int iRowNo = dgvItems.CurrentCell.RowIndex;

                    if (this._EnterMoveNext && MouseButtons == 0)
                    {
                        if (this.dgvEndEditCell != null && dgvItems.CurrentCell != null)
                        {
                            if (dgvItems.CurrentCell.RowIndex == this.dgvEndEditCell.RowIndex + 1
                                && dgvItems.CurrentCell.ColumnIndex == this.dgvEndEditCell.ColumnIndex)
                            {
                                int iColNew;
                                int iRowNew;
                                if (this.dgvEndEditCell.ColumnIndex >= dgvItems.ColumnCount - 1)
                                {
                                    iColNew = 0;
                                    iRowNew = dgvItems.CurrentCell.RowIndex;
                                }
                                else
                                {
                                    iColNew = this.dgvEndEditCell.ColumnIndex + 1;
                                    iRow = this.dgvEndEditCell.RowIndex;
                                }

                                if (iColumn >= dgvItems.Columns.Count - 2)
                                {
                                if (dgvItems[iColumn, iRowNo + 1].Visible)
                                        dgvItems.CurrentCell = dgvItems[iColumn, iRowNo + 1];
                                    dgvItems.Focus();
                                }
                                else
                                {
                                    if (iColumn == GetEnum(gridColIndexes.cMRP) || iColumn == GetEnum(gridColIndexes.cSRate1) || iColumn == GetEnum(gridColIndexes.cSRate2) || iColumn == GetEnum(gridColIndexes.cSRate3) || iColumn == GetEnum(gridColIndexes.cSRate3) || iColumn == GetEnum(gridColIndexes.cSRate4))
                                    {
                                        //if (dgvItems[iColumn + 1, iRow].Visible)
                                        //    dgvItems.CurrentCell = dgvItems[iColumn + 1, iRow];
                                        int i = 0;
                                        for (i = iColumn + 1; i < dgvItems.Columns.Count - 1; i++)
                                        {
                                            if (dgvItems[i, iRow].Visible)
                                            {
                                                dgvItems.CurrentCell = dgvItems[i, iRow];

                                                break;
                                            }
                                        }

                                        if (i >= 15)
                                        {
                                            if (dgvItems[GetEnum(gridColIndexes.cMRP), iRow + 1].Visible)
                                                dgvItems.CurrentCell = dgvItems[GetEnum(gridColIndexes.cMRP), iRow + 1];
                                            else if (dgvItems[GetEnum(gridColIndexes.cPRate), iRow + 1].Visible)
                                                dgvItems.CurrentCell = dgvItems[GetEnum(gridColIndexes.cPRate), iRow + 1];
                                            else if (dgvItems[GetEnum(gridColIndexes.cSRate1), iRow + 1].Visible)
                                                dgvItems.CurrentCell = dgvItems[GetEnum(gridColIndexes.cSRate1), iRow + 1];
                                            else if (dgvItems[GetEnum(gridColIndexes.cSRate2), iRow + 1].Visible)
                                                dgvItems.CurrentCell = dgvItems[GetEnum(gridColIndexes.cSRate2), iRow + 1];
                                            else if (dgvItems[GetEnum(gridColIndexes.cSRate3), iRow + 1].Visible)
                                                dgvItems.CurrentCell = dgvItems[GetEnum(gridColIndexes.cSRate3), iRow + 1];
                                            else if (dgvItems[GetEnum(gridColIndexes.cSRate4), iRow + 1].Visible)
                                                dgvItems.CurrentCell = dgvItems[GetEnum(gridColIndexes.cSRate4), iRow + 1];
                                            else if (dgvItems[GetEnum(gridColIndexes.cSRate5), iRow + 1].Visible)
                                                dgvItems.CurrentCell = dgvItems[GetEnum(gridColIndexes.cSRate5), iRow + 1];
                                            else
                                                dgvItems.CurrentCell = dgvItems[GetEnum(gridColIndexes.CItemCode), iRow + 1];
                                        }
                                        //dgvItems.Focus();
                                        //dgvItems.BeginEdit(true);
                                    }
                                    else if (iColumn == GetEnum(gridColIndexes.cSRate5))
                                    {
                                        if (iRow < 0)
                                        {
                                            iRow = 0;

                                            if (dgvItems.Rows.Count <= 1)
                                                dgvItems.Rows.Add();

                                            if (GetEnum(gridColIndexes.CItemCode) == 1)
                                            {
                                                if (dgvItems[GetEnum(gridColIndexes.CItemCode), iRow + 1].Visible)
                                                    dgvItems.CurrentCell = dgvItems[GetEnum(gridColIndexes.CItemCode), iRow + 1];
                                            }
                                            else if (GetEnum(gridColIndexes.CExpiry) == 1)
                                            {
                                                if (dgvItems[GetEnum(gridColIndexes.CExpiry), iRow + 1].Visible) 
                                                    dgvItems.CurrentCell = dgvItems[GetEnum(gridColIndexes.CExpiry), iRow + 1];
                                            }
                                        }
                                        else
                                        {
                                            if (dgvItems.Rows.Count <= iRow + 1)
                                                dgvItems.Rows.Add();

                                            if (GetEnum(gridColIndexes.CItemCode) == 1)
                                            {
                                                if (dgvItems[GetEnum(gridColIndexes.CItemCode), iRow + 1].Visible) 
                                                    dgvItems.CurrentCell = dgvItems[GetEnum(gridColIndexes.CItemCode), iRow + 1];
                                            }
                                            else if (GetEnum(gridColIndexes.CExpiry) == 1)
                                            {
                                                if (dgvItems[GetEnum(gridColIndexes.CExpiry), iRow + 1].Visible) 
                                                    dgvItems.CurrentCell = dgvItems[GetEnum(gridColIndexes.CExpiry), iRow + 1];
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
        }

        private void dgvItems_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {

        }

        private void dgvItems_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {

        }

        private void dgvItems_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            try
            {
                //CalcTotal();
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
        }

        private void dgvItems_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {

        }

        private void dgvItems_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            try
            {
                

                    if (dgvItems.CurrentCell.EditType == typeof(DataGridViewTextBoxEditingControl))
                    {
                        if (dgvItems.CurrentCell.ColumnIndex == GetEnum(gridColIndexes.CItemCode))
                        {
                            DataGridViewTextBoxEditingControl tb = (DataGridViewTextBoxEditingControl)e.Control;
                            tb.KeyPress += new KeyPressEventHandler(dgvItems_TextBox_KeyPress);
                            e.Control.KeyPress += new KeyPressEventHandler(dgvItems_TextBox_KeyPress);
                        }
                        else if (dgvItems.CurrentCell.ColumnIndex >= GetEnum(gridColIndexes.CExpiry) && dgvItems.CurrentCell.ColumnIndex < GetEnum(gridColIndexes.cItemID))
                        {
                            e.Control.KeyPress -= new KeyPressEventHandler(gridColumn_KeyPress);
                            TextBox tb = e.Control as TextBox;
                            if (tb != null)
                            {
                                tb.KeyPress += new KeyPressEventHandler(gridColumn_KeyPress);
                            }
                        }
                    }
                
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
        }

        private void dgvItems_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (dgvItems.CurrentCell == null) return;

                int iRow = dgvItems.CurrentCell.RowIndex;
                if (dgvItems.Rows.Count <= iRow + 1)
                    dgvItems.Rows.Add();

                if (e.KeyCode == Keys.Shift && e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    int iColumn = dgvItems.CurrentCell.ColumnIndex;
                    iRow = dgvItems.CurrentCell.RowIndex;
                    if (iColumn == dgvItems.Columns.Count - 1)
                    {
                        if (dgvItems.Rows.Count <= iRow + 1)
                            dgvItems.Rows.Add();
                        dgvItems.CurrentCell = dgvItems[1, iRow - 1];
                    }
                    else
                        SendKeys.Send("+{Tab}");
                }
                else if (e.KeyCode == Keys.Enter) // || e.KeyCode == Keys.Tab)
                {
                    e.SuppressKeyPress = true;
                    int iColumn = dgvItems.CurrentCell.ColumnIndex;
                    iRow = dgvItems.CurrentCell.RowIndex;

                    if (dgvItems.Rows[iRow].Cells[GetEnum(gridColIndexes.cMRP)].Value == null) dgvItems.Rows[iRow].Cells[GetEnum(gridColIndexes.cMRP)].Value = "0";

                    if (iColumn == dgvItems.Columns.Count - 1 && iRow != dgvItems.Rows.Count)
                    {
                        dgvItems.CurrentCell = dgvItems[1, iRow + 1];
                    }
                    else if (iColumn == dgvItems.Columns.Count - 1 && iRow == dgvItems.Rows.Count)
                    {
                    }
                    else if (iColumn == gridColIndexes.cSRate5 && Convert.ToDouble(dgvItems.Rows[iRow].Cells[GetEnum(gridColIndexes.cSRate5)].Value.ToString()) != 0)
                    {
                        if (dgvItems.Rows.Count <= iRow + 1)
                            dgvItems.Rows.Add();

                        dgvItems.CurrentCell = dgvItems[GetEnum(gridColIndexes.CItemCode), iRow + 1];
                    }
                    else if (iColumn == GetEnum(gridColIndexes.cSRate5) && Convert.ToInt32(dgvItems.Rows[iRow].Cells[GetEnum(gridColIndexes.cSRate5)].Value.ToString()) != 0)
                    {
                        if (dgvItems.Rows.Count <= iRow + 1)
                            dgvItems.Rows.Add();

                        dgvItems.CurrentCell = dgvItems[GetEnum(gridColIndexes.CItemCode), iRow + 1];
                    }
                    else
                    {
                        SendKeys.Send("{Tab}");
                    }
                }
                else if (e.KeyCode == Keys.F3)
                {
                    if (dgvItems.CurrentCell.ColumnIndex == (int)gridColIndexes.CItemCode)
                    {
                        frmLedger frmim = new frmLedger(0, true);
                        frmim.ShowDialog();
                    }
                }
                else if (e.KeyCode == Keys.F4)
                {
                    if (dgvItems.CurrentCell.ColumnIndex == (int)gridColIndexes.CItemCode)
                    {
                        int iSelectedLedgerID = 0;
                        iSelectedLedgerID = Convert.ToInt32(dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cItemID)].Value);
                        if (iSelectedLedgerID > 0)
                        {
                            frmLedger frmIM = new frmLedger(iSelectedLedgerID, true);
                            frmIM.ShowDialog();
                        }
                    }
                }
                else if (e.KeyCode == Keys.Delete)
                {
                    string SSelectedLedgerCode = Convert.ToString(dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.CItemCode)].Value);
                    if ((SSelectedLedgerCode != "" || dgvItems.Rows.Count > 1) && dgvItems.CurrentRow.Index >= 0)
                    {
                        DialogResult dlgResult = MessageBox.Show("Are you sure to delete Item Code[" + SSelectedLedgerCode + "] Permanently ?", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                        if (dlgResult == DialogResult.Yes)
                        {
                            Int32 selectedRowCount = dgvItems.Rows.GetRowCount(DataGridViewElementStates.Selected);
                            RowDelete();
                            if (dgvItems.Rows.Count < 1)
                                dgvItems.Rows.Add();

                            
                        }
                    }
                }
                else if (e.KeyCode == Keys.Down)
                {
                    
                        string sQuery = "";
                        Form fcc = Application.OpenForms["frmDetailedSearch2"];
                        if (fcc != null)
                        {
                            fcc.Focus();
                            fcc.BringToFront();
                            return;
                        }

                        if (dgvItems.CurrentCell.ColumnIndex == (int)gridColIndexes.CItemCode)
                        {
                            if (sEditedValueonKeyPress != null)
                            {
                                sQuery = "SELECT ItemCode + ItemName + BatchUnique + CAST(MRP AS varchar) AS anywhere, ItemCode, ItemName, BatchUnique, Expiry, MRP, ItemID, StockID " +
                                        " FROM     vwCompactSearch Where isnull(ActiveStatus, 1) = 1 and isnull(StockActiveStatus, 1) = 1 ";
                                if (clsVchType.ProductClassList != "")
                                    sQuery = sQuery + " AND ProductTypeID IN (" + clsVchType.ProductClassList + ")";
                                if (clsVchType.ItemCategoriesList != "")
                                    sQuery = sQuery + " AND CategoryID IN (" + clsVchType.ItemCategoriesList + ")";
                                //frmDetailedSearch2 frmN = new frmDetailedSearch2(GetFromItemSearch, sQuery, "Anywhere|ItemCode|ItemName|BatchUnique", dgvItems.Location.X + 55, dgvItems.Location.Y + 150, 7, 0, sEditedValueonKeyPress, 6, 0, "ORDER BY ItemCode ASC, Batchunique", 0, 0, "Item Search...", 0, "150,250,150,150,150,0,0", true, "frmItemMaster", 20);
                                //frmN.MdiParent = this.MdiParent;
                                //frmN.Show(); //20-Aug-2022

                                if (dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cItemID)].Value != null)
                                {
                                    this.dgvItems.EditingControlShowing -= this.dgvItems_EditingControlShowing;
                                    if (dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.CExpiry)].Visible)
                                        dgvItems.CurrentCell = dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.CExpiry)];
                                    this.dgvItems.EditingControlShowing += this.dgvItems_EditingControlShowing;
                                }
                            }
                        }
                    
                }
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
        }

        private void dgvItems_KeyPress(object sender, KeyPressEventArgs e)
        {
            string sQuery = "";
            try
            {

            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
        }

        private void dgvItems_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void dgvItems_MouseUp(object sender, MouseEventArgs e)
        {
            //if (toggleWidthSettings.ToggleState == Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active)
            //{
            //    GridInitialize_dgvColWidth(false);
            //}
        }

        private void dgvItems_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {

        }

        private void dgvItems_Scroll(object sender, ScrollEventArgs e)
        {
            //dtp.Visible = false;
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void GridSettingsEnableDisable(Syncfusion.Windows.Forms.Tools.ToggleButtonState State)
        {
            try
            {
                toggleWidthSettings.ToggleState = State;
                if (State == Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active)
                {
                    dgvColWidth.Columns[2].ReadOnly = true;
                    DataGridViewCellStyle s = new DataGridViewCellStyle();
                    s.BackColor = Color.DarkGray;
                    s.ForeColor = Color.White;

                    for (int i = 0; i < dgvColWidth.Rows.Count - 1; i++)
                    {
                        dgvColWidth[2, i].Style = s;
                    }
                }
                else
                {
                    dgvColWidth.Columns[2].ReadOnly = false;
                    DataGridViewCellStyle s = new DataGridViewCellStyle();
                    s.BackColor = Color.White;
                    s.ForeColor = Color.Black;

                    for (int i = 0; i < dgvColWidth.Rows.Count - 1; i++)
                    {
                        dgvColWidth[2, i].Style = s;
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        // Description : Disabling the Checkbox of Mandatory fields in Column Width Settings Grid
        private void DisableGridSettingsCheckbox()
        {
            string[] strDisableCol;
            List<string> lstDisableCol = new List<string>();
            //lstDisableCol.Add("cSlNo");
            //lstDisableCol.Add("CItemCode");
            //lstDisableCol.Add("CItemName");
            //lstDisableCol.Add("CUnit");
            //lstDisableCol.Add("cBarCode");
            //lstDisableCol.Add("CExpiry");
            //lstDisableCol.Add("cMRP");
            //lstDisableCol.Add("cPrate");
            //lstDisableCol.Add("cRateinclusive");
            //lstDisableCol.Add("cQty");
            //lstDisableCol.Add("cGrossAmt");
            //lstDisableCol.Add("cNetAmount");

            lstDisableCol.Add("cSlNo");
            lstDisableCol.Add("CItemCode");
            lstDisableCol.Add("cBarCode");
            //lstDisableCol.Add("cPrate");
            lstDisableCol.Add("cQty");
            lstDisableCol.Add("cNetAmount");
            strDisableCol = lstDisableCol.ToArray();

            for (int i = 0; i < dgvColWidth.Rows.Count; i++)
            {
                for (int k = 0; k < strDisableCol.Length; k++)
                {
                    if (dgvColWidth.Rows[i].Cells[03].Value.ToString() == strDisableCol[k].ToString())
                    {
                        DataGridViewCell cell = dgvColWidth.Rows[i].Cells[0];
                        DataGridViewCheckBoxCell chkCell = cell as DataGridViewCheckBoxCell;
                        chkCell.Value = true;
                        chkCell.FlatStyle = FlatStyle.Flat;
                        chkCell.Style.ForeColor = Color.DarkGray;
                        cell.ReadOnly = true;

                        break;
                    }
                }

                ValidateWidth_dgvColWidth(i);
            }
        }
        private void ValidateWidth_dgvColWidth(int RowIndex)
        {
            try
            {
                if (RowIndex > 0 && RowIndex < dgvColWidth.Rows.Count)
                {
                    if (dgvColWidth.Rows[RowIndex].Cells[2].Value.ToString() == null) dgvColWidth.Rows[RowIndex].Cells[2].Value = "0";
                    if (dgvColWidth.Rows[RowIndex].Cells[2].Value.ToString() == "") dgvColWidth.Rows[RowIndex].Cells[2].Value = "0";

                    if (dgvColWidth.Rows[RowIndex].Cells[0].ReadOnly == true)
                    {
                        if (Convert.ToDecimal(dgvColWidth.Rows[RowIndex].Cells[2].Value.ToString()) < 50)
                        {
                            dgvColWidth.Rows[RowIndex].Cells[2].Value = "50";
                            dgvItems.ColumnWidthChanged -= dgvItems_ColumnWidthChanged;
                            dgvItems.Columns[dgvColWidth.Rows[RowIndex].Cells[3].Value.ToString()].Width = 50;
                            dgvItems.ColumnWidthChanged += dgvItems_ColumnWidthChanged;
                        }
                    }
                    else
                    {
                        if (Convert.ToDecimal(dgvColWidth.Rows[RowIndex].Cells[2].Value.ToString()) < 10)
                        {
                            dgvColWidth.Rows[RowIndex].Cells[2].Value = "50";
                            dgvColWidth.Rows[RowIndex].Cells[0].Value = false;
                            dgvItems.ColumnWidthChanged -= dgvItems_ColumnWidthChanged;
                            dgvItems.Columns[dgvColWidth.Rows[RowIndex].Cells[3].Value.ToString()].Visible = false;
                            dgvItems.ColumnWidthChanged += dgvItems_ColumnWidthChanged;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // Description : Load Grid Width From Item Grid for Settings
        private void LoadGridWidthFromItemGrid()
        {
            int iHideColIndex = 0;
            DataTable dt = new DataTable();

            dt.Clear();
            dt.Columns.Add("Visible");
            dt.Columns.Add("Name");
            dt.Columns.Add("Width");
            dt.Columns.Add("ColName");

            for (int i = 0; i < gridColIndexes.MaxColIndex; i++)
            {
                if (gridColIndexes.GetColumnName(i) == "cNetAmount")
                    iHideColIndex = i;

                DataRow drCol = dt.NewRow();

                drCol["Visible"] = true;
                if (iHideColIndex > 0)
                {
                    if (i > iHideColIndex)
                        drCol["Visible"] = false;
                }
                if (dgvItems.Columns[i].Visible == false)
                {
                    drCol["Visible"] = false;
                }
                if (dgvItems.Columns[i].Width <= 10)
                {
                    drCol["Visible"] = false;
                }

                if (gridColIndexes.GetColumnName(i) == "cRateinclusive")
                    drCol["Visible"] = false;

                drCol["Name"] = dgvItems.Columns[i].HeaderText; //Enum.GetName(typeof(GridColIndexes), i).Substring(1, Enum.GetName(typeof(GridColIndexes), i).Length - 1);
                if (gridColIndexes.GetColumnName(i) == dgvItems.Columns[i].Name)
                    drCol["Width"] = dgvItems.Columns[i].Width;
                else
                    drCol["Width"] = "100";
                drCol["ColName"] = gridColIndexes.GetColumnName(i);
                dt.Rows.Add(drCol);
            }

            dgvColWidth.Columns[0].DataPropertyName = "Visible";
            dgvColWidth.Columns[1].DataPropertyName = "Name";
            dgvColWidth.Columns[2].DataPropertyName = "Width";
            dgvColWidth.Columns[3].DataPropertyName = "ColName";
            dgvColWidth.DataSource = dt;
            //dgvColWidth.Rows[8].Visible = false;
            //dgvItems.Columns["cRateinclusive"].Visible = false;

            for (int i = 0; i < dgvColWidth.Rows.Count; i++)
            {
                if (dgvColWidth[3, i].Value.ToString() == "cRateinclusive" ||
                    dgvColWidth[3, i].Value.ToString() == "cItemID" ||
                    dgvColWidth[3, i].Value.ToString() == "cID"
                    )
                {
                    dgvColWidth.Rows[i].Visible = false;
                }
            }
        }

        // Description : Save Grid Settings of Json to Voucher Type table
        private void SaveGridSettings()
        {
            string strJson = "";
            clsJsonPurGridSettingsInfo clsJPDGSinfo = new clsJsonPurGridSettingsInfo();
            List<clsJsonPurGridSettingsInfo> lstJPDGSinfo = new List<clsJsonPurGridSettingsInfo>();

            if (dgvColWidth.CurrentCell.ColumnIndex == 0)
            {
                dgvColWidth.CurrentCell = dgvColWidth[1, dgvColWidth.CurrentCell.RowIndex];
            }
            for (int i = 0; i < dgvColWidth.Rows.Count; i++)
            {
                clsJPDGSinfo = new clsJsonPurGridSettingsInfo();
                if (dgvColWidth.Rows[i].Cells[2].Value.ToString() == "") dgvColWidth.Rows[i].Cells[2].Value = "0";
                if (dgvColWidth.Rows[i].Cells[0].Value.ToString() == "" || dgvColWidth.Rows[i].Cells[0].Value.ToString() == "0")
                    clsJPDGSinfo.blnVisible = false;
                else if (dgvColWidth.Rows[i].Cells[2].Value.ToString() == "" || dgvColWidth.Rows[i].Cells[0].Value.ToString() == "0")
                    clsJPDGSinfo.blnVisible = false;
                else
                    clsJPDGSinfo.blnVisible = Convert.ToBoolean(dgvColWidth.Rows[i].Cells[0].Value);
                //clsJPDGSinfo.blnVisible = Convert.ToBoolean(dgvColWidth.Rows[i].Cells[0].Value);
                clsJPDGSinfo.sName = dgvColWidth.Rows[i].Cells[1].Value.ToString();
                clsJPDGSinfo.iWidth = Convert.ToInt32(dgvColWidth.Rows[i].Cells[2].Value.ToString());
                clsJPDGSinfo.sColName = dgvColWidth.Rows[i].Cells[3].Value.ToString();

                lstJPDGSinfo.Add(clsJPDGSinfo);
            }
            strJson = JsonConvert.SerializeObject(lstJPDGSinfo);
            Comm.fnExecuteNonQuery("UPDATE tblVchType SET GridSettingsJson = '" + strJson + "' WHERE VchTypeID = " + vchtypeID + "");

            Comm.MessageboxToasted(clsVchType.TransactionName + " Settings", "Settings Saved Successfully for " + clsVchType.TransactionName);

        }

        //Description : Format the Amount using Supplied Values
        public string FormatAmt(double myValue, string myFormat)
        {
            //https://msdn.microsoft.com/en-us/library/microsoft.visualbasic.strings.format(v=vs.110).aspx
            //DateTime.Now.ToString("dd/MMM/yyyy HH:mm:ss")
            //"29-Jan-2018 10:16:16"

            if (myFormat == "")
                myFormat = "#0.00";
            return Convert.ToDouble(myValue).ToString(myFormat);
        }

        //Description : Format Values like Currency/Quantity to the Formated Values asper App Settings
        public string FormatValue(double myValue, bool blnIsCurrency = true, string sMyFormat = "")
        {
            string myFormat = "";
            if (blnIsCurrency == true)
                myFormat = AppSettings.CurrDecimalFormat;
            else
                myFormat = AppSettings.QtyDecimalFormat;

            if (myFormat == "")
                myFormat = "#0.00";

            if (sMyFormat != "")
                myFormat = sMyFormat;

            return Convert.ToDouble(myValue).ToString(myFormat);
        }

        //Description : Calculate the Entire Accounts in each and every Corner
        //Description : Setting Default Transactional Settings to the form
        private void SetTransactionDefaults()
        {

            try
            {
                if (clsVchType == null)
                {
                    MessageBox.Show("Voucher settings incorrect for the voucher. Please correct the settings and open the voucher again.", clsVchType.TransactionName + " Settings", MessageBoxButtons.OK ,MessageBoxIcon.Error);
                    return;
                }
            }
            catch 
            {

            }

            try
            {
                if (ConvertI32(clsVchType.TransactionNumberingValue) == 0) // Auto Locked
                {
                if (iIDFromEditWindow == 0) //New
                {
                    txtInvAutoNo.Text = Comm.gfnGetNextSerialNo("tblBoardRateMaster", "INVNO", " VchTypeID=" + vchtypeID).ToString();
                    txtInvAutoNo.Tag = 0;
                }  
                txtInvAutoNo.ReadOnly = true;
            }
            else if (ConvertI32(clsVchType.TransactionNumberingValue) == 1) // Auto Editable
            {
                if (iIDFromEditWindow == 0) //New
                {
                    txtInvAutoNo.Text = Comm.gfnGetNextSerialNo("tblBoardRateMaster", "INVNO", " VchTypeID=" + vchtypeID).ToString();
                    txtInvAutoNo.Tag = 0;
                }
                    
                txtInvAutoNo.ReadOnly = false;
            }
            else
            {
                txtInvAutoNo.Tag = 0;
                txtInvAutoNo.ReadOnly = false;
            }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            try
            { 
            if (clsVchType.blnSaleStaffLockWSel == 1)
                cboSalesStaff.Enabled = false;
            else
                cboSalesStaff.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        //Description : Setting Transactions that Varying to the form
        private void SetTransactionsthatVarying()
        {
            try
            {
                if (clsVchType == null)
                {
                    MessageBox.Show("Voucher settings incorrect for the voucher. Please correct the settings and open the voucher again.", clsVchType.TransactionName + " Settings", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            catch (Exception ex)
            {

            }
        }

        //Description : Setting asper Application Settings
        private void SetApplicationSettings()
        {
            try
            { 

            dtpInvDate.MinDate = AppSettings.FinYearStart;
            dtpInvDate.MaxDate = AppSettings.FinYearEnd;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void txtChequeno_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnShow_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvItems.Rows.Count > 0)
                    dgvItems.CurrentCell = dgvItems[1, 0];

                dgvItems.Refresh();
                dgvItems.Rows.Clear();
                Application.DoEvents();

                //AddColumnsToGrid();

                sqlControl rs = new sqlControl();

                switch (cboDisplayRate.SelectedIndex)
                {
                    case 0:
                        rs.Open(@"SELECT tblItemMaster.ItemID,ItemCode,ItemName,BatchCode,BatchUnique,0 as MRP,tblstock.Expiry,0 as PRate,
	                        0 as SRate1,0 as SRate2,0 as SRate3,0 as Srate4,0 as Srate5, intnoorweight 
                            FROM tblItemMaster join tblstock on tblItemMaster.ItemID=tblStock.ItemID and tblItemMaster.BatchMode = 3 order by ItemCode ASC");

                        break;

                    case 1:
                        rs.Open(@"SELECT tblItemMaster.ItemID,ItemCode,ItemName,BatchCode,tblStock.BatchUnique,'31-Dec-2050' as Expiry,tblstock.PRate, intnoorweight,
	                            (Select top 1 MRP From tblBoardRateDetail, tblBoardRateMaster Where tblBoardRateDetail.InvID = tblBoardRateMaster.InvID and tblBoardRateDetail.ItemID = tblStock.ItemID and tblBoardRateDetail.BatchUnique = tblStock.BatchUnique order by tblBoardRateMaster.InvDate desc, tblBoardRateMaster.invid desc) MRP,
	                            (Select top 1 Srate1 From tblBoardRateDetail, tblBoardRateMaster Where tblBoardRateDetail.InvID = tblBoardRateMaster.InvID and tblBoardRateDetail.ItemID = tblStock.ItemID and tblBoardRateDetail.BatchUnique = tblStock.BatchUnique order by tblBoardRateMaster.InvDate desc, tblBoardRateMaster.invid desc) Srate1,
	                            (Select top 1 Srate2 From tblBoardRateDetail, tblBoardRateMaster Where tblBoardRateDetail.InvID = tblBoardRateMaster.InvID and tblBoardRateDetail.ItemID = tblStock.ItemID and tblBoardRateDetail.BatchUnique = tblStock.BatchUnique order by tblBoardRateMaster.InvDate desc, tblBoardRateMaster.invid desc) Srate2,
	                            (Select top 1 Srate3 From tblBoardRateDetail, tblBoardRateMaster Where tblBoardRateDetail.InvID = tblBoardRateMaster.InvID and tblBoardRateDetail.ItemID = tblStock.ItemID and tblBoardRateDetail.BatchUnique = tblStock.BatchUnique order by tblBoardRateMaster.InvDate desc, tblBoardRateMaster.invid desc) Srate3,
	                            (Select top 1 Srate4 From tblBoardRateDetail, tblBoardRateMaster Where tblBoardRateDetail.InvID = tblBoardRateMaster.InvID and tblBoardRateDetail.ItemID = tblStock.ItemID and tblBoardRateDetail.BatchUnique = tblStock.BatchUnique order by tblBoardRateMaster.InvDate desc, tblBoardRateMaster.invid desc) Srate4,
	                            (Select top 1 Srate5 From tblBoardRateDetail, tblBoardRateMaster Where tblBoardRateDetail.InvID = tblBoardRateMaster.InvID and tblBoardRateDetail.ItemID = tblStock.ItemID and tblBoardRateDetail.BatchUnique = tblStock.BatchUnique order by tblBoardRateMaster.InvDate desc, tblBoardRateMaster.invid desc) Srate5 
                            FROM tblItemMaster join tblstock on tblItemMaster.ItemID=tblStock.ItemID and tblItemMaster.BatchMode = 3 
                            order by ItemCode ASC");

                        break;

                    case 2:
                        rs.Open(@"SELECT tblItemMaster.ItemID,ItemCode,ItemName,BatchCode,BatchUnique,tblItemMaster.MRP,tblstock.Expiry,tblItemMaster.PRate, intnoorweight,
	                            tblItemMaster.SRate1,tblItemMaster.SRate2,tblItemMaster.SRate3,tblItemMaster.Srate4,tblItemMaster.Srate5 
                            FROM tblItemMaster join tblstock on tblItemMaster.ItemID=tblStock.ItemID and tblItemMaster.BatchMode = 3 order by ItemCode ASC");

                        break;

                    case 3:
                        rs.Open(@"SELECT tblItemMaster.ItemID,ItemCode,ItemName,BatchCode,tblStock.BatchUnique,'31-Dec-2050' as Expiry,tblstock.PRate, intnoorweight,
	                            (Select top 1 MRP From tblPurchaseItem, tblPurchase Where tblPurchaseItem.InvID = tblPurchase.InvID and tblPurchaseItem.ItemID = tblStock.ItemID and tblPurchaseItem.BatchUnique = tblStock.BatchUnique order by tblPurchase.InvDate desc) MRP,
	                            (Select top 1 Srate1 From tblPurchaseItem, tblPurchase Where tblPurchaseItem.InvID = tblPurchase.InvID and tblPurchaseItem.ItemID = tblStock.ItemID and tblPurchaseItem.BatchUnique = tblStock.BatchUnique order by tblPurchase.InvDate desc) Srate1,
	                            (Select top 1 Srate2 From tblPurchaseItem, tblPurchase Where tblPurchaseItem.InvID = tblPurchase.InvID and tblPurchaseItem.ItemID = tblStock.ItemID and tblPurchaseItem.BatchUnique = tblStock.BatchUnique order by tblPurchase.InvDate desc) Srate2,
	                            (Select top 1 Srate3 From tblPurchaseItem, tblPurchase Where tblPurchaseItem.InvID = tblPurchase.InvID and tblPurchaseItem.ItemID = tblStock.ItemID and tblPurchaseItem.BatchUnique = tblStock.BatchUnique order by tblPurchase.InvDate desc) Srate3,
	                            (Select top 1 Srate4 From tblPurchaseItem, tblPurchase Where tblPurchaseItem.InvID = tblPurchase.InvID and tblPurchaseItem.ItemID = tblStock.ItemID and tblPurchaseItem.BatchUnique = tblStock.BatchUnique order by tblPurchase.InvDate desc) Srate4,
	                            (Select top 1 Srate5 From tblPurchaseItem, tblPurchase Where tblPurchaseItem.InvID = tblPurchase.InvID and tblPurchaseItem.ItemID = tblStock.ItemID and tblPurchaseItem.BatchUnique = tblStock.BatchUnique order by tblPurchase.InvDate desc) Srate5 
                            FROM tblItemMaster join tblstock on tblItemMaster.ItemID=tblStock.ItemID and tblItemMaster.BatchMode = 3 
                            order by ItemCode ASC");

                        break;

                    default:
                        break;
                }

                int i = 0;

                dgvItems.Rows.Add();

                while (!rs.eof())
                {
                    dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cSlNo)].Value = i + 1;
                    dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.CItemCode)].Value = rs.sqlDT.Rows[i]["ItemCode"].ToString();
                    dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.CItemName)].Value = rs.sqlDT.Rows[i]["ItemName"].ToString();
                    dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cItemID)].Value = rs.sqlDT.Rows[i]["ItemID"].ToString();
                    dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.CItemName)].Tag = rs.sqlDT.Rows[i]["ItemID"].ToString();
                    dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.CBarCode)].Value = rs.sqlDT.Rows[i]["BatchCode"].ToString();
                    dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.CBatchUnique)].Value = rs.sqlDT.Rows[i]["BatchUnique"].ToString();

                    dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.CUnit)].Tag = rs.sqlDT.Rows[i]["intnoorweight"].ToString(); 
                    if (rs.sqlDT.Rows[i]["intnoorweight"].ToString() == "0")
                        dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.CUnit)].Value = "KG";
                    else
                        dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.CUnit)].Value = "Nos";

                    dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cPRate)].Value = rs.sqlDT.Rows[i]["PRate"].ToString();
                    dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cMRP)].Value = rs.sqlDT.Rows[i]["MRP"].ToString();
                    dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.CExpiry)].Value = rs.sqlDT.Rows[i]["Expiry"].ToString();

                    dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cSRate1)].Value = rs.sqlDT.Rows[i]["SRate1"].ToString();
                    dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cSRate2)].Value = rs.sqlDT.Rows[i]["SRate2"].ToString();
                    dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cSRate3)].Value = rs.sqlDT.Rows[i]["SRate3"].ToString();
                    dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cSRate4)].Value = rs.sqlDT.Rows[i]["SRate4"].ToString();
                    dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cSRate5)].Value = rs.sqlDT.Rows[i]["SRate5"].ToString();

                    dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cSlNo)].ReadOnly = true;
                    dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.CItemCode)].ReadOnly = true;
                    dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.CItemName)].ReadOnly = true;
                    dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.CBarCode)].ReadOnly = true;
                    dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.CBatchUnique)].ReadOnly = true;
                    dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.CUnit)].ReadOnly = true;
                    dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cMRP)].ReadOnly = false;
                    dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cPRate)].ReadOnly = true;
                    dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cSRate1)].ReadOnly = false;
                    dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cSRate2)].ReadOnly = false;
                    dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cSRate3)].ReadOnly = false;
                    dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cSRate4)].ReadOnly = false;
                    dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cSRate5)].ReadOnly = false;

                    i++;

                    dgvItems.Rows.Add();

                    rs.MoveNext();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            return base.ProcessCmdKey(ref msg, keyData);

            //try
            //{
            //    //dgvItems.BeginEdit(true);

            //    if (dgvItems.CurrentCell == null) return true;


            //        int currentRowIndex = dgvItems.CurrentCell.RowIndex;
            //    switch (keyData)
            //    {

            //        case Keys.Down:
            //        // Check not already at the last row in the grid before moving down one row
            //        if (dgvItems.Rows[dgvItems.CurrentCell.RowIndex].Cells[dgvItems.CurrentCell.ColumnIndex] == dgvItems.Rows[dgvItems.CurrentCell.RowIndex].Cells[GetEnum(gridColIndexes.cSRate1)]|| dgvItems.Rows[dgvItems.CurrentCell.RowIndex].Cells[dgvItems.CurrentCell.ColumnIndex] == dgvItems.Rows[dgvItems.CurrentCell.RowIndex].Cells[GetEnum(gridColIndexes.cSRate2)] || dgvItems.Rows[dgvItems.CurrentCell.RowIndex].Cells[dgvItems.CurrentCell.ColumnIndex] == dgvItems.Rows[dgvItems.CurrentCell.RowIndex].Cells[GetEnum(gridColIndexes.cSRate3)]|| dgvItems.Rows[dgvItems.CurrentCell.RowIndex].Cells[dgvItems.CurrentCell.ColumnIndex] == dgvItems.Rows[dgvItems.CurrentCell.RowIndex].Cells[GetEnum(gridColIndexes.cSRate4)]|| dgvItems.Rows[dgvItems.CurrentCell.RowIndex].Cells[dgvItems.CurrentCell.ColumnIndex] == dgvItems.Rows[dgvItems.CurrentCell.RowIndex].Cells[GetEnum(gridColIndexes.cSRate1)]|| dgvItems.Rows[dgvItems.CurrentCell.RowIndex].Cells[dgvItems.CurrentCell.ColumnIndex] == dgvItems.Rows[dgvItems.CurrentCell.RowIndex].Cells[GetEnum(gridColIndexes.cSRate5)])
            //        {
            //            if (currentRowIndex + 2 < dgvItems.Rows.Count)
            //                dgvItems.CurrentCell = dgvItems.Rows[currentRowIndex + 1].Cells[dgvItems.CurrentCell.ColumnIndex];
            //            dgvItems.CurrentCell.Value = dgvItems.Rows[currentRowIndex].Cells[dgvItems.CurrentCell.ColumnIndex].Value;

            //            dgvItems.BeginEdit(true);
            //        }
            //        return true;
            //}

            //    // Line below is reached if we didn't handle the key in this method, it tells the form/control to handle it
            //    return base.ProcessCmdKey(ref msg, keyData);

            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    return false;
            //}
        }

        private Boolean GetFromCheckedListCat(string sSelIDs)
        {
            try
            {
                lblCatIds.Text = sSelIDs;
                lblCatIds.Tag = lblVoucherIds.Text;
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
            
        }

        //Description : Convert to Int32 of Decimal Value
        private int ConvertI32(decimal dVal)
        {
            return Convert.ToInt32(dVal);
        }

        private string[] GetItemDetails(decimal dItemID = 0)
        {
            if (dItemID != 0)
            {
                List<string> lstItm = new List<string>();
                DataTable dtItm = new DataTable();
                dtItm = Comm.fnGetData("SELECT * FROM tblItemMaster WHERE ItemID = " + dItemID + "").Tables[0];
                if (dtItm.Rows.Count > 0)
                {
                    lstItm.Add(Comm.CheckDBNullOrEmpty(dtItm.Rows[0][GetEnumItem(ItemIndexes.ItemID)].ToString()));
                    lstItm.Add(Comm.CheckDBNullOrEmpty(dtItm.Rows[0][GetEnumItem(ItemIndexes.ItemCode)].ToString()));
                    lstItm.Add(Comm.CheckDBNullOrEmpty(dtItm.Rows[0][GetEnumItem(ItemIndexes.ItemName)].ToString()));
                    lstItm.Add(Comm.CheckDBNullOrEmpty(dtItm.Rows[0][GetEnumItem(ItemIndexes.CategoryID)].ToString()));
                    lstItm.Add(Comm.CheckDBNullOrEmpty(dtItm.Rows[0][GetEnumItem(ItemIndexes.Description)].ToString()));
                    lstItm.Add(Comm.CheckDBNullOrEmpty(dtItm.Rows[0][GetEnumItem(ItemIndexes.PRate)].ToString()));
                    lstItm.Add(Comm.CheckDBNullOrEmpty(dtItm.Rows[0][GetEnumItem(ItemIndexes.CRateAvg)].ToString()));
                    lstItm.Add(Comm.CheckDBNullOrEmpty(dtItm.Rows[0][GetEnumItem(ItemIndexes.Srate1Per)].ToString()));
                    lstItm.Add(Comm.CheckDBNullOrEmpty(dtItm.Rows[0][GetEnumItem(ItemIndexes.SRate1)].ToString()));
                    lstItm.Add(Comm.CheckDBNullOrEmpty(dtItm.Rows[0][GetEnumItem(ItemIndexes.Srate2Per)].ToString()));
                    lstItm.Add(Comm.CheckDBNullOrEmpty(dtItm.Rows[0][GetEnumItem(ItemIndexes.SRate2)].ToString()));
                    lstItm.Add(Comm.CheckDBNullOrEmpty(dtItm.Rows[0][GetEnumItem(ItemIndexes.Srate3Per)].ToString()));
                    lstItm.Add(Comm.CheckDBNullOrEmpty(dtItm.Rows[0][GetEnumItem(ItemIndexes.SRate3)].ToString()));
                    lstItm.Add(Comm.CheckDBNullOrEmpty(dtItm.Rows[0][GetEnumItem(ItemIndexes.Srate4)].ToString()));
                    lstItm.Add(Comm.CheckDBNullOrEmpty(dtItm.Rows[0][GetEnumItem(ItemIndexes.Srate4Per)].ToString()));
                    lstItm.Add(Comm.CheckDBNullOrEmpty(dtItm.Rows[0][GetEnumItem(ItemIndexes.SRate5)].ToString()));
                    lstItm.Add(Comm.CheckDBNullOrEmpty(dtItm.Rows[0][GetEnumItem(ItemIndexes.Srate5Per)].ToString()));
                    lstItm.Add(Comm.CheckDBNullOrEmpty(dtItm.Rows[0][GetEnumItem(ItemIndexes.MRP)].ToString()));
                    lstItm.Add(Comm.CheckDBNullOrEmpty(dtItm.Rows[0][GetEnumItem(ItemIndexes.ROL)].ToString()));
                    lstItm.Add(Comm.CheckDBNullOrEmpty(dtItm.Rows[0][GetEnumItem(ItemIndexes.Rack)].ToString()));
                    lstItm.Add(Comm.CheckDBNullOrEmpty(dtItm.Rows[0][GetEnumItem(ItemIndexes.Manufacturer)].ToString()));
                    lstItm.Add(Comm.CheckDBNullOrEmpty(dtItm.Rows[0][GetEnumItem(ItemIndexes.ActiveStatus)].ToString()));
                    lstItm.Add(Comm.CheckDBNullOrEmpty(dtItm.Rows[0][GetEnumItem(ItemIndexes.IntLocal)].ToString()));
                    lstItm.Add(Comm.CheckDBNullOrEmpty(dtItm.Rows[0][GetEnumItem(ItemIndexes.ProductType)].ToString()));
                    lstItm.Add(Comm.CheckDBNullOrEmpty(dtItm.Rows[0][GetEnumItem(ItemIndexes.ProductTypeID)].ToString()));
                    lstItm.Add(Comm.CheckDBNullOrEmpty(dtItm.Rows[0][GetEnumItem(ItemIndexes.LedgerID)].ToString()));
                    lstItm.Add(Comm.CheckDBNullOrEmpty(dtItm.Rows[0][GetEnumItem(ItemIndexes.UNITID)].ToString()));
                    lstItm.Add(Comm.CheckDBNullOrEmpty(dtItm.Rows[0][GetEnumItem(ItemIndexes.Notes)].ToString()));
                    lstItm.Add(Comm.CheckDBNullOrEmpty(dtItm.Rows[0][GetEnumItem(ItemIndexes.agentCommPer)].ToString()));
                    lstItm.Add(Comm.CheckDBNullOrEmpty(dtItm.Rows[0][GetEnumItem(ItemIndexes.BlnExpiryItem)].ToString()));
                    lstItm.Add(Comm.CheckDBNullOrEmpty(dtItm.Rows[0][GetEnumItem(ItemIndexes.Coolie)].ToString()));
                    lstItm.Add(Comm.CheckDBNullOrEmpty(dtItm.Rows[0][GetEnumItem(ItemIndexes.FinishedGoodID)].ToString()));
                    lstItm.Add(Comm.CheckDBNullOrEmpty(dtItm.Rows[0][GetEnumItem(ItemIndexes.MinRate)].ToString()));
                    lstItm.Add(Comm.CheckDBNullOrEmpty(dtItm.Rows[0][GetEnumItem(ItemIndexes.MaxRate)].ToString()));
                    lstItm.Add(Comm.CheckDBNullOrEmpty(dtItm.Rows[0][GetEnumItem(ItemIndexes.PLUNo)].ToString()));
                    lstItm.Add(Comm.CheckDBNullOrEmpty(dtItm.Rows[0][GetEnumItem(ItemIndexes.HSNID)].ToString()));
                    lstItm.Add(Comm.CheckDBNullOrEmpty(dtItm.Rows[0][GetEnumItem(ItemIndexes.iCatDiscPer)].ToString()));
                    lstItm.Add(Comm.CheckDBNullOrEmpty(dtItm.Rows[0][GetEnumItem(ItemIndexes.IPGDiscPer)].ToString()));
                    lstItm.Add(Comm.CheckDBNullOrEmpty(dtItm.Rows[0][GetEnumItem(ItemIndexes.ImanDiscPer)].ToString()));
                    lstItm.Add(Comm.CheckDBNullOrEmpty(dtItm.Rows[0][GetEnumItem(ItemIndexes.ItemNameUniCode)].ToString()));
                    lstItm.Add(Comm.CheckDBNullOrEmpty(dtItm.Rows[0][GetEnumItem(ItemIndexes.Minqty)].ToString()));
                    lstItm.Add(Comm.CheckDBNullOrEmpty(dtItm.Rows[0][GetEnumItem(ItemIndexes.MNFID)].ToString()));
                    lstItm.Add(Comm.CheckDBNullOrEmpty(dtItm.Rows[0][GetEnumItem(ItemIndexes.PGID)].ToString()));
                    lstItm.Add(Comm.CheckDBNullOrEmpty(dtItm.Rows[0][GetEnumItem(ItemIndexes.ItemCodeUniCode)].ToString()));
                    lstItm.Add(Comm.CheckDBNullOrEmpty(dtItm.Rows[0][GetEnumItem(ItemIndexes.UPC)].ToString()));
                    lstItm.Add(Comm.CheckDBNullOrEmpty(dtItm.Rows[0][GetEnumItem(ItemIndexes.BatchMode)].ToString()));
                    lstItm.Add(Comm.CheckDBNullOrEmpty(dtItm.Rows[0][GetEnumItem(ItemIndexes.blnExpiry)].ToString()));
                    lstItm.Add(Comm.CheckDBNullOrEmpty(dtItm.Rows[0][GetEnumItem(ItemIndexes.Qty)].ToString()));
                    lstItm.Add(Comm.CheckDBNullOrEmpty(dtItm.Rows[0][GetEnumItem(ItemIndexes.MaxQty)].ToString()));
                    lstItm.Add(Comm.CheckDBNullOrEmpty(dtItm.Rows[0][GetEnumItem(ItemIndexes.IntNoOrWeight)].ToString()));
                    lstItm.Add(Comm.CheckDBNullOrEmpty(dtItm.Rows[0][GetEnumItem(ItemIndexes.SystemName)].ToString()));
                    lstItm.Add(Comm.CheckDBNullOrEmpty(dtItm.Rows[0][GetEnumItem(ItemIndexes.UserID)].ToString()));
                    lstItm.Add(Comm.CheckDBNullOrEmpty(dtItm.Rows[0][GetEnumItem(ItemIndexes.LastUpdateDate)].ToString()));
                    lstItm.Add(Comm.CheckDBNullOrEmpty(dtItm.Rows[0][GetEnumItem(ItemIndexes.LastUpdateTime)].ToString()));
                    lstItm.Add(Comm.CheckDBNullOrEmpty(dtItm.Rows[0][GetEnumItem(ItemIndexes.TenantID)].ToString()));
                    lstItm.Add(Comm.CheckDBNullOrEmpty(dtItm.Rows[0][GetEnumItem(ItemIndexes.blnCessOnTax)].ToString()));
                    lstItm.Add(Comm.CheckDBNullOrEmpty(dtItm.Rows[0][GetEnumItem(ItemIndexes.CompCessQty)].ToString()));
                    lstItm.Add(Comm.CheckDBNullOrEmpty(dtItm.Rows[0][GetEnumItem(ItemIndexes.CGSTTaxPer)].ToString()));
                    lstItm.Add(Comm.CheckDBNullOrEmpty(dtItm.Rows[0][GetEnumItem(ItemIndexes.SGSTTaxPer)].ToString()));
                    lstItm.Add(Comm.CheckDBNullOrEmpty(dtItm.Rows[0][GetEnumItem(ItemIndexes.IGSTTaxPer)].ToString()));
                    lstItm.Add(Comm.CheckDBNullOrEmpty(dtItm.Rows[0][GetEnumItem(ItemIndexes.CessPer)].ToString()));
                    lstItm.Add(Comm.CheckDBNullOrEmpty(dtItm.Rows[0][GetEnumItem(ItemIndexes.VAT)].ToString()));
                    lstItm.Add(Comm.CheckDBNullOrEmpty(dtItm.Rows[0][GetEnumItem(ItemIndexes.CategoryIDs)].ToString()));
                    lstItm.Add(Comm.CheckDBNullOrEmpty(dtItm.Rows[0][GetEnumItem(ItemIndexes.ColorIDs)].ToString()));
                    lstItm.Add(Comm.CheckDBNullOrEmpty(dtItm.Rows[0][GetEnumItem(ItemIndexes.SizeIDs)].ToString()));
                    lstItm.Add(Comm.CheckDBNullOrEmpty(dtItm.Rows[0][GetEnumItem(ItemIndexes.BrandDisPer)].ToString()));
                    lstItm.Add(Comm.CheckDBNullOrEmpty(dtItm.Rows[0][GetEnumItem(ItemIndexes.DGroupID)].ToString()));
                    lstItm.Add(Comm.CheckDBNullOrEmpty(dtItm.Rows[0][GetEnumItem(ItemIndexes.DGroupDisPer)].ToString()));
                    lstItm.Add(Comm.CheckDBNullOrEmpty(dtItm.Rows[0][GetEnumItem(ItemIndexes.BrandID)].ToString()));
                    lstItm.Add(Comm.CheckDBNullOrEmpty(dtItm.Rows[0][GetEnumItem(ItemIndexes.AltUnitID)].ToString()));
                    lstItm.Add(Comm.CheckDBNullOrEmpty(dtItm.Rows[0][GetEnumItem(ItemIndexes.ConvFactor)].ToString()));
                    lstItm.Add(Comm.CheckDBNullOrEmpty(dtItm.Rows[0][GetEnumItem(ItemIndexes.Shelflife)].ToString()));
                    lstItm.Add(Comm.CheckDBNullOrEmpty(dtItm.Rows[0][GetEnumItem(ItemIndexes.SRateInclusive)].ToString()));
                    lstItm.Add(Comm.CheckDBNullOrEmpty(dtItm.Rows[0][GetEnumItem(ItemIndexes.PRateInclusive)].ToString()));
                    lstItm.Add(Comm.CheckDBNullOrEmpty(dtItm.Rows[0][GetEnumItem(ItemIndexes.Slabsys)].ToString()));
                    lstItm.Add(Comm.CheckDBNullOrEmpty(dtItm.Rows[0][GetEnumItem(ItemIndexes.ParentID)].ToString()));
                    lstItm.Add(Comm.CheckDBNullOrEmpty(dtItm.Rows[0][GetEnumItem(ItemIndexes.ParentConv)].ToString()));
                    lstItm.Add(Comm.CheckDBNullOrEmpty(dtItm.Rows[0][GetEnumItem(ItemIndexes.blnParentEqlRate)].ToString()));
                    lstItm.Add(Comm.CheckDBNullOrEmpty(dtItm.Rows[0][GetEnumItem(ItemIndexes.ItmConvType)].ToString()));
                    lstItm.Add(Comm.CheckDBNullOrEmpty(dtItm.Rows[0][GetEnumItem(ItemIndexes.DiscPer)].ToString()));
                }
                return lstItm.ToArray();
            }
            else
                return null;
        }

        //Description : Serialize the Sales table Fields asper instructions.
        private string SerializetoJson()
        {
            #region "Sales Master (tblBoardRateMaster) ------------------------------- >>"

            if (iIDFromEditWindow == 0)
            {
                clsJPLinfo.InvID = Comm.gfnGetNextSerialNo("tblBoardRateMaster", "InvId");
                txtInvAutoNo.Tag = clsJPLinfo.InvID;
                //clsJPLinfo.AutoNum = Convert.ToDecimal(Comm.gfnGetNextSerialNo("tblBoardRateMaster", "INVNO", " VchTypeID=" + vchtypeID).ToString());
            }
            else
            {
                clsJPLinfo.InvID = Convert.ToInt32(iIDFromEditWindow);
                txtInvAutoNo.Tag = Convert.ToDecimal(iIDFromEditWindow);
                //clsJPLinfo.AutoNum = Convert.ToDecimal(txtReferenceAutoNo.Tag.ToString());
            }

            clsJPLinfo.InvNo = txtInvAutoNo.Text;
            clsJPLinfo.Prefix = "".Trim();
            clsJPLinfo.AutoNum = 0;
            clsJPLinfo.InvDate = Convert.ToDateTime(dtpInvDate.Text);

            clsJPLinfo.SalesManID = Convert.ToInt32(cboSalesStaff.SelectedValue);
            clsJPLinfo.Narration = "";
            clsJPLinfo.VchtypeID = vchtypeID;
            clsJPLinfo.UserID = Global.gblUserID;
            clsJPLinfo.ReferenceAutoNO = "";
            clsJPLinfo.RefNo = "";
            clsJPLinfo.TenantID = Global.gblTenantID;
            clsJPL.clsJsonPLInfo_ = clsJPLinfo;

            #endregion

            #region "Cost Center (tblCostCenter) --------------------------------- >>"

            clsJPMCostCentreinfo.CCID = 1;
            clsJPMCostCentreinfo.CCName = "<MAIN>";
            clsJPMCostCentreinfo.Description1 = "";
            clsJPMCostCentreinfo.Description2 = "";
            clsJPMCostCentreinfo.Description3 = "";
            clsJPMCostCentreinfo.BLNDAMAGED = 0;
            //Dipu 21-03-2022 ------- >>
            //clsJPMCostCentreinfo.SystemName = Global.gblSystemName;
            //clsJPMCostCentreinfo.UserID = Global.gblUserID;
            //clsJPMCostCentreinfo.LastUpdateDate = DateTime.Today;
            //clsJPMCostCentreinfo.LastUpdateTime = DateTime.Now;
            clsJPMCostCentreinfo.TenantID = Global.gblTenantID;
            clsJPL.clsJsonPMCCentreInfo_ = clsJPMCostCentreinfo;

            #endregion

            #region "Dest Cost Center (tblCostCenter) --------------------------------- >>"

            clsJPMDestCostCentreinfo.CCID = 1;
            clsJPMDestCostCentreinfo.CCName = "<MAIN>";
            clsJPMDestCostCentreinfo.Description1 = "";
            clsJPMDestCostCentreinfo.Description2 = "";
            clsJPMDestCostCentreinfo.Description3 = "";
            clsJPMDestCostCentreinfo.BLNDAMAGED = 0;
            //Dipu 21-03-2022 ------- >>
            //clsJPMDestCostCentreinfo.SystemName = Global.gblSystemName;
            //clsJPMDestCostCentreinfo.UserID = Global.gblUserID;
            //clsJPMDestCostCentreinfo.LastUpdateDate = DateTime.Today;
            //clsJPMDestCostCentreinfo.LastUpdateTime = DateTime.Now;
            clsJPMDestCostCentreinfo.TenantID = Global.gblTenantID;
            clsJPL.clsJsonPMDestCCentreInfo_ = clsJPMDestCostCentreinfo;

            #endregion

            #region "Employee Master (tblEmployee) ------------------------------- >>"

            string[] sArrEmp = GetEmpDetails(Convert.ToDecimal(cboSalesStaff.SelectedValue));
            clsJPMEmployeeinfo.EmpID = Convert.ToDecimal(cboSalesStaff.SelectedValue);
            clsJPMEmployeeinfo.Name = sArrEmp[GetEnumEmp(EmpIndexes.Name)];
            clsJPMEmployeeinfo.Address = sArrEmp[GetEnumEmp(EmpIndexes.Address)];
            clsJPMEmployeeinfo.NameOfFather = sArrEmp[GetEnumEmp(EmpIndexes.NameOfFather)];
            clsJPMEmployeeinfo.PhNo = sArrEmp[GetEnumEmp(EmpIndexes.PhNo)];
            clsJPMEmployeeinfo.MaritialStatus = sArrEmp[GetEnumEmp(EmpIndexes.MaritialStatus)];
            clsJPMEmployeeinfo.NoOfFamilyMembers = sArrEmp[GetEnumEmp(EmpIndexes.NoOfFamilyMembers)];
            clsJPMEmployeeinfo.NameOFNominee = sArrEmp[GetEnumEmp(EmpIndexes.NameOFNominee)];
            clsJPMEmployeeinfo.Spouse = sArrEmp[GetEnumEmp(EmpIndexes.Spouse)];
            clsJPMEmployeeinfo.SpouseEmployed = Convert.ToBoolean(sArrEmp[GetEnumEmp(EmpIndexes.SpouseEmployed)]);
            clsJPMEmployeeinfo.OwnerOfResidence = Convert.ToBoolean(sArrEmp[GetEnumEmp(EmpIndexes.OwnerOfResidence)]);
            clsJPMEmployeeinfo.PANNo = sArrEmp[GetEnumEmp(EmpIndexes.PANNo)];
            clsJPMEmployeeinfo.BloodGroup = sArrEmp[GetEnumEmp(EmpIndexes.BloodGroup)];
            clsJPMEmployeeinfo.Designation = sArrEmp[GetEnumEmp(EmpIndexes.Designation)];
            clsJPMEmployeeinfo.Qualification = sArrEmp[GetEnumEmp(EmpIndexes.Qualification)];
            clsJPMEmployeeinfo.Sex = sArrEmp[GetEnumEmp(EmpIndexes.Sex)];
            clsJPMEmployeeinfo.DOB = Convert.ToDateTime(sArrEmp[GetEnumEmp(EmpIndexes.DOB)]);
            clsJPMEmployeeinfo.DOJ = Convert.ToDateTime(sArrEmp[GetEnumEmp(EmpIndexes.DOJ)]);
            clsJPMEmployeeinfo.DOI = Convert.ToDateTime(sArrEmp[GetEnumEmp(EmpIndexes.DOI)]);
            clsJPMEmployeeinfo.PensionAccNo = sArrEmp[GetEnumEmp(EmpIndexes.PensionAccNo)];
            clsJPMEmployeeinfo.GPFAccNo = sArrEmp[GetEnumEmp(EmpIndexes.GPFAccNo)];
            clsJPMEmployeeinfo.GSLIAccNo = sArrEmp[GetEnumEmp(EmpIndexes.GSLIAccNo)];
            clsJPMEmployeeinfo.LICPolicyNo = sArrEmp[GetEnumEmp(EmpIndexes.LICPolicyNo)];
            clsJPMEmployeeinfo.LICMonthlyPremium = Convert.ToDecimal(sArrEmp[GetEnumEmp(EmpIndexes.LICMonthlyPremium)]);
            clsJPMEmployeeinfo.LICDateofMaturity = Convert.ToDateTime(sArrEmp[GetEnumEmp(EmpIndexes.LICDateofMaturity)]);
            clsJPMEmployeeinfo.CategoryID = Convert.ToInt32(sArrEmp[GetEnumEmp(EmpIndexes.CategoryID)]);
            clsJPMEmployeeinfo.DateofPromotion = Convert.ToDateTime(sArrEmp[GetEnumEmp(EmpIndexes.DateofPromotion)]);
            clsJPMEmployeeinfo.DateofRetirement = Convert.ToDateTime(sArrEmp[GetEnumEmp(EmpIndexes.DateofRetirement)]);
            clsJPMEmployeeinfo.GISAccNo = sArrEmp[GetEnumEmp(EmpIndexes.GISAccNo)];
            clsJPMEmployeeinfo.BankAccNo = sArrEmp[GetEnumEmp(EmpIndexes.BankAccNo)];
            clsJPMEmployeeinfo.Commission = Convert.ToDecimal(sArrEmp[GetEnumEmp(EmpIndexes.Commission)]);
            clsJPMEmployeeinfo.CommissionAmt = Convert.ToDecimal(sArrEmp[GetEnumEmp(EmpIndexes.CommissionAmt)]);
            clsJPMEmployeeinfo.EmpFname = Convert.ToString(sArrEmp[GetEnumEmp(EmpIndexes.EmpFname)]);
            clsJPMEmployeeinfo.blnSalesStaff = Convert.ToInt32(sArrEmp[GetEnumEmp(EmpIndexes.blnSalesStaff)]);
            clsJPMEmployeeinfo.PhotoPath = Convert.ToString(sArrEmp[GetEnumEmp(EmpIndexes.PhotoPath)]);
            clsJPMEmployeeinfo.InsCompany = Convert.ToString(sArrEmp[GetEnumEmp(EmpIndexes.InsCompany)]);
            clsJPMEmployeeinfo.CommissionCondition = Convert.ToInt32(sArrEmp[GetEnumEmp(EmpIndexes.CommissionCondition)]);
            clsJPMEmployeeinfo.EmpCode = Convert.ToString(sArrEmp[GetEnumEmp(EmpIndexes.EmpCode)]);
            clsJPMEmployeeinfo.blnStatus = Convert.ToInt32(sArrEmp[GetEnumEmp(EmpIndexes.blnStatus)]);
            clsJPMEmployeeinfo.DrivingLicenceNo = Convert.ToString(sArrEmp[GetEnumEmp(EmpIndexes.DrivingLicenceNo)]);
            clsJPMEmployeeinfo.DrivingLicenceExpiry = Convert.ToDateTime(sArrEmp[GetEnumEmp(EmpIndexes.DrivingLicenceExpiry)]);
            clsJPMEmployeeinfo.PassportNo = Convert.ToString(sArrEmp[GetEnumEmp(EmpIndexes.PassportNo)]);
            clsJPMEmployeeinfo.PassportExpiry = Convert.ToDateTime(sArrEmp[GetEnumEmp(EmpIndexes.PassportExpiry)]);
            clsJPMEmployeeinfo.Active = Convert.ToInt32(sArrEmp[GetEnumEmp(EmpIndexes.Active)]);
            clsJPMEmployeeinfo.SortOrder = Convert.ToInt32(sArrEmp[GetEnumEmp(EmpIndexes.SortOrder)]);
            clsJPMEmployeeinfo.EnrollNo = Convert.ToInt32(sArrEmp[GetEnumEmp(EmpIndexes.EnrollNo)]);
            clsJPMEmployeeinfo.TargetAmount = Convert.ToDecimal(sArrEmp[GetEnumEmp(EmpIndexes.TargetAmount)]);
            clsJPMEmployeeinfo.IncentivePer = Convert.ToDecimal(sArrEmp[GetEnumEmp(EmpIndexes.IncentivePer)]);
            clsJPMEmployeeinfo.PWD = Convert.ToString(sArrEmp[GetEnumEmp(EmpIndexes.PWD)]);
            clsJPMEmployeeinfo.Holidays = Convert.ToString(sArrEmp[GetEnumEmp(EmpIndexes.Holidays)]);
            clsJPMEmployeeinfo.LID = Convert.ToInt32(sArrEmp[GetEnumEmp(EmpIndexes.LID)]);
            clsJPMEmployeeinfo.salarypermonth = Convert.ToDecimal(sArrEmp[GetEnumEmp(EmpIndexes.salarypermonth)]);
            //Dipu 21-03-2022 ------- >>
            //clsJPMEmployeeinfo.SystemName = Global.gblSystemName;
            //clsJPMEmployeeinfo.UserID = Global.gblUserID;
            //clsJPMEmployeeinfo.LastUpdateDate = DateTime.Today;
            //clsJPMEmployeeinfo.LastUpdateTime = DateTime.Now;
            clsJPMEmployeeinfo.TenantID = Global.gblTenantID;
            clsJPL.clsJsonPMEmployeeInfo_ = clsJPMEmployeeinfo;

            #endregion

            #region "Sales Details (tblPriceListDetail) -------------------------- >>"
            List<clsJsonPLDInfo> lstJPDinfo = new List<clsJsonPLDInfo>();
            for (int i = 0; i < dgvItems.Rows.Count; i++)
            {
                if (dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.CItemCode)].Value != null)
                {
                    if (dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.CItemCode)].Value.ToString() != "")
                    {
                        clsJSJDinfo = new clsJsonPLDInfo();

                        //clsJSJDinfo.InvID = Convert.ToDecimal(txtInvAutoNo.Text);
                        clsJSJDinfo.InvID = Convert.ToInt32(txtInvAutoNo.Tag);
                        clsJSJDinfo.ItemID = Convert.ToInt32(dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cItemID)].Value);

                        clsJSJDinfo.BatchCode = dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.CBarCode)].Value.ToString();
                        clsJSJDinfo.BatchUnique = dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.CBatchUnique)].Value.ToString();
                        //clsJSJDinfo.uni = dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.CBatchUnique)].Value.ToString();

                        clsJSJDinfo.MRP = Convert.ToDecimal(dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cMRP)].Value);
                        clsJSJDinfo.PRate = Convert.ToDecimal(dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cPRate)].Value);
                        clsJSJDinfo.CRate = 0;

                        clsJSJDinfo.Srate1 = Convert.ToDecimal(dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cSRate1)].Value);
                        clsJSJDinfo.Srate2 = Convert.ToDecimal(dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cSRate2)].Value);
                        clsJSJDinfo.Srate3 = Convert.ToDecimal(dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cSRate3)].Value);
                        clsJSJDinfo.Srate4 = Convert.ToDecimal(dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cSRate4)].Value);
                        clsJSJDinfo.Srate5 = Convert.ToDecimal(dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cSRate5)].Value);
                        clsJSJDinfo.Srate1Perc = 0;
                        clsJSJDinfo.Srate2Perc = 0;
                        clsJSJDinfo.Srate3Perc = 0;
                        clsJSJDinfo.Srate4Perc = 0;
                        clsJSJDinfo.Srate5Perc = 0;
                        lstJPDinfo.Add(clsJSJDinfo);
                    }
                }
            }
            clsJPL.clsJsonSJDetailsInfoList_ = lstJPDinfo;

            #endregion

            #region "Item Unit Details ------------------------------------------- >>"

            List<clsJsonPDUnitinfo> lstJPDUnit = new List<clsJsonPDUnitinfo>();
            for (int j = 0; j < dgvItems.Rows.Count; j++)
            {
                if (dgvItems.Rows[j].Cells[GetEnum(gridColIndexes.CItemCode)].Value != null)
                {
                    if (dgvItems.Rows[j].Cells[GetEnum(gridColIndexes.CItemCode)].Value.ToString() != "")
                    {
                        DataTable dtUnit = new DataTable();
                        clsJPDUnitinfo = new clsJsonPDUnitinfo();
                        clsJPDUnitinfo.UnitID = 0;
                        clsJPDUnitinfo.UnitName = "";
                        //dipu on 20-Apr-2022 ----->>
                        clsJPDUnitinfo.UnitShortName = "";

                        clsJPDUnitinfo.TenantID = Global.gblTenantID;
                        lstJPDUnit.Add(clsJPDUnitinfo);
                    }
                }
            }
            clsJPL.clsJsonPDUnitinfoList_ = lstJPDUnit;

            #endregion

            #region "Item Details ------------------------------------------------ >>"

            List<clsJsonPDIteminfo> lstJPDItem = new List<clsJsonPDIteminfo>();
            for (int j = 0; j < dgvItems.Rows.Count; j++)
            {
                if (dgvItems.Rows[j].Cells[GetEnum(gridColIndexes.CItemCode)].Value != null)
                {
                    if (dgvItems.Rows[j].Cells[GetEnum(gridColIndexes.CItemCode)].Value.ToString() != "")
                    {
                        clsJPDIteminfo = new clsJsonPDIteminfo();
                        string[] sArrItm = GetItemDetails(Convert.ToDecimal(dgvItems.Rows[j].Cells[GetEnum(gridColIndexes.cItemID)].Value));
                        clsJPDIteminfo.ItemID = Convert.ToDecimal(dgvItems.Rows[j].Cells[GetEnum(gridColIndexes.cItemID)].Value);
                        clsJPDIteminfo.ItemCode = sArrItm[GetEnumItem(ItemIndexes.ItemCode)].ToString();
                        clsJPDIteminfo.ItemName = sArrItm[GetEnumItem(ItemIndexes.ItemName)].ToString();
                        clsJPDIteminfo.CategoryID = Convert.ToInt32(sArrItm[GetEnumItem(ItemIndexes.CategoryID)].ToString());
                        clsJPDIteminfo.Description = sArrItm[GetEnumItem(ItemIndexes.Description)].ToString();
                        clsJPDIteminfo.PRate = Convert.ToDecimal(sArrItm[GetEnumItem(ItemIndexes.PRate)].ToString());
                        clsJPDIteminfo.CRateAvg = Convert.ToDecimal(sArrItm[GetEnumItem(ItemIndexes.CRateAvg)].ToString());
                        clsJPDIteminfo.SRate1 = Convert.ToDecimal(sArrItm[GetEnumItem(ItemIndexes.SRate1)].ToString());
                        clsJPDIteminfo.SRate2 = Convert.ToDecimal(sArrItm[GetEnumItem(ItemIndexes.SRate2)].ToString());
                        clsJPDIteminfo.SRate3 = Convert.ToDecimal(sArrItm[GetEnumItem(ItemIndexes.SRate3)].ToString());
                        clsJPDIteminfo.Srate4 = Convert.ToDecimal(sArrItm[GetEnumItem(ItemIndexes.Srate4)].ToString());
                        clsJPDIteminfo.SRate5 = Convert.ToDecimal(sArrItm[GetEnumItem(ItemIndexes.SRate5)].ToString());
                        clsJPDIteminfo.MRP = Convert.ToDecimal(sArrItm[GetEnumItem(ItemIndexes.MRP)].ToString());
                        clsJPDIteminfo.ROL = Convert.ToDecimal(sArrItm[GetEnumItem(ItemIndexes.ROL)].ToString());
                        clsJPDIteminfo.Rack = sArrItm[GetEnumItem(ItemIndexes.Rack)].ToString();
                        clsJPDIteminfo.Manufacturer = sArrItm[GetEnumItem(ItemIndexes.Manufacturer)].ToString();
                        clsJPDIteminfo.ActiveStatus = Convert.ToInt32(sArrItm[GetEnumItem(ItemIndexes.ActiveStatus)].ToString());
                        clsJPDIteminfo.IntLocal = Convert.ToInt32(sArrItm[GetEnumItem(ItemIndexes.IntLocal)].ToString());
                        clsJPDIteminfo.ProductType = sArrItm[GetEnumItem(ItemIndexes.ProductType)].ToString();
                        clsJPDIteminfo.ProductTypeID = Convert.ToDecimal(sArrItm[GetEnumItem(ItemIndexes.ProductTypeID)].ToString());
                        clsJPDIteminfo.LedgerID = Convert.ToDecimal(sArrItm[GetEnumItem(ItemIndexes.LedgerID)].ToString());
                        clsJPDIteminfo.UNITID = Convert.ToDecimal(sArrItm[GetEnumItem(ItemIndexes.UNITID)].ToString());
                        clsJPDIteminfo.Notes = sArrItm[GetEnumItem(ItemIndexes.Notes)].ToString();
                        clsJPDIteminfo.agentCommPer = Convert.ToDecimal(sArrItm[GetEnumItem(ItemIndexes.agentCommPer)].ToString());
                        clsJPDIteminfo.BlnExpiryItem = Convert.ToInt32(sArrItm[GetEnumItem(ItemIndexes.BlnExpiryItem)].ToString());
                        clsJPDIteminfo.Coolie = Convert.ToInt32(sArrItm[GetEnumItem(ItemIndexes.Coolie)].ToString());
                        clsJPDIteminfo.FinishedGoodID = Convert.ToInt32(sArrItm[GetEnumItem(ItemIndexes.FinishedGoodID)].ToString());
                        clsJPDIteminfo.MinRate = Convert.ToDecimal(sArrItm[GetEnumItem(ItemIndexes.MinRate)].ToString());
                        clsJPDIteminfo.MaxRate = Convert.ToDecimal(sArrItm[GetEnumItem(ItemIndexes.MaxRate)].ToString());
                        clsJPDIteminfo.PLUNo = Convert.ToDecimal(sArrItm[GetEnumItem(ItemIndexes.PLUNo)].ToString());
                        clsJPDIteminfo.HSNID = Convert.ToInt32(sArrItm[GetEnumItem(ItemIndexes.HSNID)].ToString());
                        clsJPDIteminfo.iCatDiscPer = Convert.ToDecimal(sArrItm[GetEnumItem(ItemIndexes.iCatDiscPer)].ToString());
                        clsJPDIteminfo.IPGDiscPer = Convert.ToDecimal(sArrItm[GetEnumItem(ItemIndexes.IPGDiscPer)].ToString());
                        clsJPDIteminfo.ImanDiscPer = Convert.ToDecimal(sArrItm[GetEnumItem(ItemIndexes.ImanDiscPer)].ToString());
                        clsJPDIteminfo.ItemNameUniCode = sArrItm[GetEnumItem(ItemIndexes.ItemNameUniCode)].ToString();
                        clsJPDIteminfo.Minqty = Convert.ToDecimal(sArrItm[GetEnumItem(ItemIndexes.Minqty)].ToString());
                        clsJPDIteminfo.MNFID = Convert.ToInt32(sArrItm[GetEnumItem(ItemIndexes.MNFID)].ToString());
                        clsJPDIteminfo.PGID = Convert.ToInt32(sArrItm[GetEnumItem(ItemIndexes.PGID)].ToString());
                        clsJPDIteminfo.PGID = Convert.ToInt32(sArrItm[GetEnumItem(ItemIndexes.PGID)].ToString());
                        clsJPDIteminfo.ItemCodeUniCode = sArrItm[GetEnumItem(ItemIndexes.ItemCodeUniCode)].ToString();
                        clsJPDIteminfo.UPC = sArrItm[GetEnumItem(ItemIndexes.UPC)].ToString();
                        clsJPDIteminfo.BatchMode = sArrItm[GetEnumItem(ItemIndexes.BatchMode)].ToString();
                        clsJPDIteminfo.blnExpiry = Convert.ToInt32(sArrItm[GetEnumItem(ItemIndexes.blnExpiry)].ToString());
                        clsJPDIteminfo.Qty = Convert.ToDecimal(sArrItm[GetEnumItem(ItemIndexes.Qty)].ToString());
                        clsJPDIteminfo.MaxQty = Convert.ToDecimal(sArrItm[GetEnumItem(ItemIndexes.MaxQty)].ToString());
                        clsJPDIteminfo.IntNoOrWeight = Convert.ToInt32(sArrItm[GetEnumItem(ItemIndexes.IntNoOrWeight)].ToString());
                        clsJPDIteminfo.SystemName = Global.gblSystemName;
                        clsJPDIteminfo.UserID = Global.gblUserID;
                        clsJPDIteminfo.LastUpdateDate = DateTime.Today; ;
                        clsJPDIteminfo.LastUpdateTime = DateTime.Now;
                        clsJPDIteminfo.TenantID = Global.gblTenantID;
                        clsJPDIteminfo.blnCessOnTax = Convert.ToInt32(sArrItm[GetEnumItem(ItemIndexes.blnCessOnTax)].ToString());
                        clsJPDIteminfo.CompCessQty = Convert.ToDecimal(sArrItm[GetEnumItem(ItemIndexes.CompCessQty)].ToString());
                        clsJPDIteminfo.CGSTTaxPer = Convert.ToDecimal(sArrItm[GetEnumItem(ItemIndexes.CGSTTaxPer)].ToString());
                        clsJPDIteminfo.SGSTTaxPer = Convert.ToDecimal(sArrItm[GetEnumItem(ItemIndexes.SGSTTaxPer)].ToString());
                        clsJPDIteminfo.IGSTTaxPer = Convert.ToDecimal(sArrItm[GetEnumItem(ItemIndexes.IGSTTaxPer)].ToString());
                        clsJPDIteminfo.CessPer = Convert.ToDecimal(sArrItm[GetEnumItem(ItemIndexes.CessPer)].ToString());
                        clsJPDIteminfo.VAT = Convert.ToDecimal(sArrItm[GetEnumItem(ItemIndexes.VAT)].ToString());
                        clsJPDIteminfo.CategoryIDs = sArrItm[GetEnumItem(ItemIndexes.CategoryIDs)].ToString();
                        clsJPDIteminfo.ColorIDs = sArrItm[GetEnumItem(ItemIndexes.ColorIDs)].ToString();
                        clsJPDIteminfo.SizeIDs = sArrItm[GetEnumItem(ItemIndexes.SizeIDs)].ToString();
                        clsJPDIteminfo.BrandDisPer = Convert.ToDecimal(sArrItm[GetEnumItem(ItemIndexes.BrandDisPer)].ToString());
                        clsJPDIteminfo.DGroupID = Convert.ToInt32(sArrItm[GetEnumItem(ItemIndexes.DGroupID)].ToString());
                        clsJPDIteminfo.DGroupDisPer = Convert.ToDecimal(sArrItm[GetEnumItem(ItemIndexes.DGroupDisPer)].ToString());
                        clsJPDIteminfo.BrandID = Convert.ToInt32(sArrItm[GetEnumItem(ItemIndexes.BrandID)].ToString());
                        clsJPDIteminfo.AltUnitID = Convert.ToDecimal(sArrItm[GetEnumItem(ItemIndexes.AltUnitID)].ToString());
                        clsJPDIteminfo.ConvFactor = Convert.ToDecimal(sArrItm[GetEnumItem(ItemIndexes.ConvFactor)].ToString());
                        clsJPDIteminfo.Shelflife = Convert.ToDecimal(sArrItm[GetEnumItem(ItemIndexes.Shelflife)].ToString());
                        clsJPDIteminfo.SRateInclusive = Convert.ToDecimal(sArrItm[GetEnumItem(ItemIndexes.SRateInclusive)].ToString());
                        clsJPDIteminfo.PRateInclusive = Convert.ToDecimal(sArrItm[GetEnumItem(ItemIndexes.PRateInclusive)].ToString());
                        clsJPDIteminfo.Slabsys = Convert.ToDecimal(sArrItm[GetEnumItem(ItemIndexes.Slabsys)].ToString());
                        clsJPDIteminfo.ParentID = Convert.ToDecimal(sArrItm[GetEnumItem(ItemIndexes.ParentID)].ToString());
                        clsJPDIteminfo.ParentConv = Convert.ToDecimal(sArrItm[GetEnumItem(ItemIndexes.ParentConv)].ToString());
                        clsJPDIteminfo.blnParentEqlRate = Convert.ToInt32(sArrItm[GetEnumItem(ItemIndexes.blnParentEqlRate)].ToString());
                        clsJPDIteminfo.ItmConvType = sArrItm[GetEnumItem(ItemIndexes.ItmConvType)].ToString();
                        clsJPDIteminfo.DiscPer = Convert.ToDecimal(sArrItm[GetEnumItem(ItemIndexes.DiscPer)].ToString());
                        lstJPDItem.Add(clsJPDIteminfo);
                    }
                }
            }
            clsJPL.clsJsonPDIteminfoList_ = lstJPDItem;
            #endregion

            return JsonConvert.SerializeObject(clsJPL);
        }

        // Cash : 0, Credit: 1, Both: 2, Cash Desk : 3
        //Description : Deserialize the JSon to Controls asper instructions.
        private void DeserializeFromJSon(string sToDeSerialize = "")
        {
            clsJSonPriceList clsPriceList = JsonConvert.DeserializeObject<clsJSonPriceList>(sToDeSerialize);

            txtInvAutoNo.Text = Convert.ToString(clsPriceList.clsJsonPLInfo_.InvNo);
            txtInvAutoNo.Tag = Convert.ToDouble(clsPriceList.clsJsonPLInfo_.InvID);
            dtpInvDate.Text = Convert.ToString(clsPriceList.clsJsonPLInfo_.InvDate);
            
            cboSalesStaff.SelectedValue = clsPriceList.clsJsonPMEmployeeInfo_.EmpID;

            DataTable dtGetPurDetail = clsPriceList.clsJsonSJDetailsInfoList_.ToDataTable();
            DataTable dtItemFrmJson = clsPriceList.clsJsonPDIteminfoList_.ToDataTable();
            DataTable dtUnitFrmJson = clsPriceList.clsJsonPDUnitinfoList_.ToDataTable();
            if (dtGetPurDetail.Rows.Count > 0)
            {
                sqlControl rs = new sqlControl();

                AddColumnsToGrid();
                for (int i = 0; i < dtGetPurDetail.Rows.Count; i++)
                {
                    dgvItems.Rows.Add();

                    rs.Open("Select ItemCode,ItemName From tblItemMaster Where ItemID=" + dtGetPurDetail.Rows[i]["ItemId"].ToString());
                    if (!rs.eof())
                    {
                        dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.CItemCode)].Value = rs.fields("itemcode");
                        dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.CItemName)].Value = rs.fields("ItemName");
                    }

                    SetValue(GetEnum(gridColIndexes.cSlNo), i, (i + 1).ToString());
                    dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.CItemCode)].Value = dtItemFrmJson.Rows[i]["itemcode"].ToString();
                    dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.CItemName)].Value = dtItemFrmJson.Rows[i]["ItemName"].ToString();

                    dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cMRP)].Value = FormatValue(Convert.ToDouble(dtGetPurDetail.Rows[i]["MRP"].ToString()), true);
                    dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cPRate)].Value = FormatValue(Convert.ToDouble(dtGetPurDetail.Rows[i]["PRate"].ToString()), true);
                    

                    dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.CExpiry)].Value = dtGetPurDetail.Rows[i]["Expiry"].ToString();

                    dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.CBarCode)].Value = dtGetPurDetail.Rows[i]["BatchCode"].ToString();
                    dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.CBatchUnique)].Value = dtGetPurDetail.Rows[i]["BatchUnique"].ToString();

                    dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.CUnit)].Tag = dtGetPurDetail.Rows[i]["intnoorweight"].ToString();
                    if (dtGetPurDetail.Rows[i]["intnoorweight"].ToString() == "0")
                        dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.CUnit)].Value = "KG";
                    else
                        dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.CUnit)].Value = "Nos";

                    dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cSRate1)].Value = FormatValue(Convert.ToDouble(dtGetPurDetail.Rows[i]["Srate1"].ToString()), true);
                    dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cSRate2)].Value = FormatValue(Convert.ToDouble(dtGetPurDetail.Rows[i]["Srate2"].ToString()), true);
                    dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cSRate3)].Value = FormatValue(Convert.ToDouble(dtGetPurDetail.Rows[i]["Srate3"].ToString()), true);
                    dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cSRate4)].Value = FormatValue(Convert.ToDouble(dtGetPurDetail.Rows[i]["Srate4"].ToString()), true);
                    dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cSRate5)].Value = FormatValue(Convert.ToDouble(dtGetPurDetail.Rows[i]["Srate5"].ToString()), true);

                    dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cItemID)].Value = dtGetPurDetail.Rows[i]["ItemId"].ToString();

                    //this.dgvItems.Columns[i].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
            }
        }

        private void frmBoardRateUpdator_Activated(object sender, EventArgs e)
        {
            //try
            //{
            //    LoadGridWidthFromItemGrid();
            //    DisableGridSettingsCheckbox();

            //    GridSettingsEnableDisable(Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active);
            //}
            //catch (Exception ex)
            //{

            //}
        }

        private void dgvItems_ColumnWidthChanged(object sender, DataGridViewColumnEventArgs e)
        {
            try
            {
                if (this.ActiveControl == null) return;
                if (this.ActiveControl.Name != dgvItems.Name) return;
            }
            catch
            { }

            try
            {
                if (toggleWidthSettings.ToggleState == Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active)
                {
                    GridInitialize_dgvColWidth(false);
                    try
                    {
                        LoadGridWidthFromItemGrid();
                        DisableGridSettingsCheckbox();
                        SaveGridSettings();

                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void toggleWidthSettings_ToggleStateChanged(object sender, Syncfusion.Windows.Forms.Tools.ToggleStateChangedEventArgs e)
        {
            try
            {
                GridSettingsEnableDisable(toggleWidthSettings.ToggleState);
            }
            catch
            { }
        }

        private void chklastInvdate_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void cbCalc_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void togglebtnActive_Click(object sender, EventArgs e)
        {

        }

        private void btnPrint_Click(object sender, EventArgs e)
        {

        }

        private void frmBoardRateUpdator_Shown(object sender, EventArgs e)
        {
            try
            {
                LoadGridWidthFromItemGrid();
                DisableGridSettingsCheckbox();

                GridSettingsEnableDisable(Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active);
            }
            catch (Exception ex)
            {

            }
        }

        private void dgvItems_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (dgvItems.CurrentCell != null)
                {
                    if (dgvItems.CurrentCell.ColumnIndex == gridColIndexes.cMRP
                      || dgvItems.CurrentCell.ColumnIndex == gridColIndexes.cSRate1
                      || dgvItems.CurrentCell.ColumnIndex == gridColIndexes.cSRate2
                      || dgvItems.CurrentCell.ColumnIndex == gridColIndexes.cSRate3
                      || dgvItems.CurrentCell.ColumnIndex == gridColIndexes.cSRate4
                      || dgvItems.CurrentCell.ColumnIndex == gridColIndexes.cSRate5)
                    {
                        dgvItems.Focus();
                        dgvItems.BeginEdit(true);
                    }
                }
            }
            catch
            { }
        }



        //Description : Load Saved data from database from edit window or Navigation buttons
        private void LoadData(int iSelectedID = 0)
        {
            try
            {

                MessageBox.Show("Edit provision is not available for board rate updator. You can open a new board rate and fill the items to know the last updated rate.", "Board Rate Updator", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;

                DataTable dtLoad = new DataTable();

                GetPriceListInfo.InvId = Convert.ToDecimal(iSelectedID);
                GetPriceListInfo.TenantID = Convert.ToDecimal(Global.gblTenantID);
                GetPriceListInfo.VchTypeID = vchtypeID;
                dtLoad = clsPL.GetPriceListMaster(GetPriceListInfo, false);
                if (dtLoad.Rows.Count > 0)
                {
                    DeserializeFromJSon(dtLoad.Rows[0]["JsonData"].ToString());
                    PrefixTag = 0;
                    //if (Convert.ToInt32(dtLoad.Rows[0]["Cancelled"].ToString()) == 1)
                    //{
                    //    PrefixTag = 3; // Archive
                    //}
                    //else
                    //{
                    //    PrefixTag = 0;
                    //}

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void CRUD_Operations(int iAction = 0)
        {
            return;

            bool blnTransactionStarted = false;

            try
            {
                string[] strResult;
                string sRetDet = "";

                DBConnection dBConnection = new DBConnection();
                var sqlConn = dBConnection.GetDBConnection();
                SqlTransaction trans = sqlConn.BeginTransaction();

                blnTransactionStarted = true;

                try
                {
                    if (IsValidate() == true)
                    {
                        string strJson = SerializetoJson();

                        #region "CRUD Operations for Sales Master ------------------------- >>"

                        string sRet = "";
                        sRet = clsPL.PriceListMasterCRUD(clsJPL, sqlConn, trans, strJson, iAction);
                        if (sRet.Length > 2)
                        {
                            strResult = sRet.Split('|');
                            if (Convert.ToInt32(strResult[0].ToString()) == -1)
                            {
                                MessageBox.Show("Failed to Save ? ", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                                trans.Rollback();
                                blnTransactionStarted = false;

                                return;
                            }
                        }
                        else
                        {
                            if (Convert.ToInt32(sRet) == -1)
                            {
                                MessageBox.Show("Failed to Save ? ", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                                trans.Rollback();
                                blnTransactionStarted = false;

                                return;
                            }
                            else
                            {
                                //if (iIDFromEditWindow != 0)
                                //    this.Close();
                                //else
                                //    Comm.MessageboxToasted("Board Rate", "Sales Group saved successfully");
                            }
                        }
                        #endregion

                        #region "CRUD Operations for Sales Detail ------------------------- >>"
                        Hashtable hstPurStk = new Hashtable();

                        if (iAction == 1) // Edit
                        {
                            sRetDet = clsPL.PriceListDetailCRUD(clsJPL, sqlConn, trans, "", 2);
                            sRetDet = clsPL.PriceListDetailCRUD(clsJPL, sqlConn, trans, "", 0);
                        }
                        else
                            sRetDet = clsPL.PriceListDetailCRUD(clsJPL, sqlConn, trans, "", iAction);

                        if (sRetDet == "") sRetDet = "0";
                        if (sRetDet.Length > 2)
                        {
                            strResult = sRetDet.Split('|');
                            if (strResult[0].ToString().Replace(" ", "").Substring(0, 2) == "-1")
                            {
                                MessageBox.Show("Failed to Save ? " + strResult[1].ToString(), Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                                trans.Rollback();
                                blnTransactionStarted = false;

                                return;
                            }
                        }
                        else
                        {
                            if (Convert.ToInt32(sRetDet) == -1)
                            {
                                MessageBox.Show("Failed to Save ? ", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                                trans.Rollback();
                                blnTransactionStarted = false;

                                return;
                            }
                            else
                            {
                                //if (iIDFromEditWindow != 0)
                                //    this.Close();
                                //else
                                //    Comm.MessageboxToasted("Board Rate", "Voucher[" + txtInvAutoNo.Text + "] Saved Successfully");
                            }
                        }
                        #endregion

                        trans.Commit();
                        blnTransactionStarted = false;

                        string vchno = txtInvAutoNo.Text;

                        if (iAction < 2)
                        {
                            if (iIDFromEditWindow != 0)
                            {
                                this.Close();
                                Comm.MessageboxToasted("Board Rate", "Voucher[" + vchno + "] Saved Successfully");
                                return;
                            }
                            else
                            {
                                ClearControls();

                                GridInitialize_dgvColWidth();
                                try
                                {
                                    LoadGridWidthFromItemGrid();
                                    DisableGridSettingsCheckbox();

                                    GridSettingsEnableDisable(Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active);

                                    SetTransactionsthatVarying();
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }

                                Comm.MessageboxToasted("Board Rate", "Voucher[" + vchno + "] Saved Successfully");
                            }
                        }
                        else if (iAction == 2)
                        {
                            this.Close();
                            Comm.MessageboxToasted("Board Rate", "Voucher[" + vchno + "] deleted successfully");
                            return;
                        }
                        else if (iAction == 3)
                        {
                            this.Close();
                            Comm.MessageboxToasted("Board Rate", "Voucher[" + vchno + "] is archived");
                            return;
                        }
                    }

                    else
                    {
                        if (blnTransactionStarted == true)
                        {
                            trans.Rollback();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    if (blnTransactionStarted == true)
                    {
                        trans.Rollback();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        public bool PreFilterMessage(ref Message m)
        {
            try
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
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
        }

        #endregion
    }

    #region "gridcolindexes"
    public class BoardRateGridColIndexes
    {
        public int cSlNo = 0;
        public int CItemCode = 1;
        public int CItemName = 2;
        public int CBarCode = 3;
        public int CBatchUnique = 4;
        public int CUnit = 5;
        public int CExpiry = 6;
        public int cMRP = 7;
        public int cPRate = 8;
        public int cSRate1 = 9;
        public int cSRate2 = 10;
        public int cSRate3 = 11;
        public int cSRate4 = 12;
        public int cSRate5 = 13;
        public int cItemID = 14;
        public int cImgDel = 15;

        //This variabl;e holds the maximum cols index in this class
        public int MaxColIndex = 16;

        public string GetColumnName(int colIndex)
        {
            switch (colIndex)
            {
                case 0:
                    {
                        return nameof(cSlNo);
                    }
                case 1:
                    {
                        return nameof(CItemCode);
                    }
                case 2:
                    {
                        return nameof(CItemName);
                    }
                case 3:
                    {
                        return nameof(CBarCode);
                    }
                case 4:
                    {
                        return nameof(CBatchUnique);
                    }
                case 5:
                    {
                        return nameof(CUnit);
                    }
                case 6:
                    {
                        return nameof(CExpiry);
                    }
                case 7:
                    {
                        return nameof(cMRP);
                    }
                case 8:
                    {
                        return nameof(cPRate);
                    }
                case 9:
                    {
                        return nameof(cSRate1);
                    }
                case 10:
                    {
                        return nameof(cSRate2);
                    }
                case 11:
                    {
                        return nameof(cSRate3);
                    }
                case 12:
                    {
                        return nameof(cSRate4);
                    }
                case 13:
                    {
                        return nameof(cSRate5);
                    }
                case 14:
                    {
                        return nameof(cItemID);
                    }
                case 15:
                    {
                        return nameof(cImgDel);
                    }
                default:
                    {
                        MessageBox.Show("Invalid column index | " + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);

                        return "";
                    }
            }
        }
    }
    #endregion 
}
