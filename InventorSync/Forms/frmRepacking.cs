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

namespace InventorSync
{

    public partial class frmRepacking : Form, IMessageFilter
    {
        //=============================================================================
        // Created By       : Arun
        // Created On       : 12-Nov-2022
        // Last Edited On   :
        // Last Edited By   : Arun
        // Description      : Working With Single Voucher Type. Repacking
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

        private bool mblnInitialisedSubWindow = false;

        private frmCompactSearch frmSupplierSearch;
        private frmCompactSearch frmItemSearch;
        private frmCompactSearch frmBatchSearch;

        public frmRepacking(int iVchTpeId = 0, int iTransID = 0, bool bFromEdit = false, object MDIParent = null)
        {
            InitializeComponent();
            Application.AddMessageFilter(this);

            try
            {
                Comm.TransparentControls(this);
                Comm.SetControlColours(this);

                this.BackColor = Color.FromArgb(249, 246, 238);

                lblHeading.Font = new Font("Tahoma", 21, FontStyle.Regular, GraphicsUnit.Pixel);
                lblSave.Font = new Font("Tahoma", 10, FontStyle.Regular, GraphicsUnit.Point);
                lblPause.Font = new Font("Tahoma", 10, FontStyle.Regular, GraphicsUnit.Point);
                lblPrint.Font = new Font("Tahoma", 10, FontStyle.Regular, GraphicsUnit.Point);
                lblArchive.Font = new Font("Tahoma", 10, FontStyle.Regular, GraphicsUnit.Point);
                lblDelete.Font = new Font("Tahoma", 10, FontStyle.Regular, GraphicsUnit.Point);
                lblFind.Font = new Font("Tahoma", 10, FontStyle.Regular, GraphicsUnit.Point);

                lblSave.ForeColor = Color.Black;
                lblPause.ForeColor = Color.Black;
                lblPrint.ForeColor = Color.Black;
                lblArchive.ForeColor = Color.Black;
                lblDelete.ForeColor = Color.Black;
                lblFind.ForeColor = Color.Black;

                lblHeading.ForeColor = Color.Black;

                lblInvNo.ForeColor = Color.Black;
                lblInvDate.ForeColor = Color.Black;
                lblReferenceNo.ForeColor = Color.Black;

                btnprev.Image = global::InventorSync.Properties.Resources.fast_backwards;
                btnNext.Image = global::InventorSync.Properties.Resources.fast_forward;
                btnSave.Image = global::InventorSync.Properties.Resources.save240402;
                btnPause.Image = global::InventorSync.Properties.Resources.pause_button;
                btnPrint.Image = global::InventorSync.Properties.Resources.printer_finalised;
                btnArchive.Image = global::InventorSync.Properties.Resources.archive123;
                btnDelete.Image = global::InventorSync.Properties.Resources.delete340402;
                btnFind.Image = global::InventorSync.Properties.Resources.find_finalised_3030;
                btnMenu.Image = global::InventorSync.Properties.Resources.menu_hamburger;
                btnSettings.Image = global::InventorSync.Properties.Resources.settings_finalised;
                btnMinimize.Image = global::InventorSync.Properties.Resources.minimize_finalised;
                btnClose.Image = global::InventorSync.Properties.Resources.logout_Final;
            }
            catch
            { }

            controlsToMove.Add(this);
            controlsToMove.Add(this.lblHeading);//Add whatever controls here you want to move the form when it is clicked and dragged

            frmMDI form = (frmMDI)MDIParent;
            this.MdiParent = form;
            int l = form.ClientSize.Width - 10; //(this.MdiParent.ClientSize.Width - this.Width) / 2;
            int t = form.ClientSize.Height - 80; //((this.MdiParent.ClientSize.Height - this.Height) / 2) - 30;
            this.SetBounds(5, 0, l, t);

            //this.SetBounds(l, t, this.Width, this.Height);

            //this.WindowState = FormWindowState.Maximized;

            clsVchType = JSonComm.GetVoucherType(iVchTpeId);
            clsVchTypeFeatures = JSonComm.GetVoucherTypeGeneralSettings(iVchTpeId, 1);

            AddColumnsToGrid(dgvStockIn);
            AddColumnsToGrid(dgvStockOut);
            ClearControls();

            bFromEditRepacking = bFromEdit;
            iIDFromEditWindow = iTransID;
            vchtypeID = iVchTpeId;

            if (iIDFromEditWindow != 0)
                txtPrefix.Tag = 1;
            else
                txtPrefix.Tag = 0;

            if (iTransID != 0)
            {
                FillCostCentre();

                SetTransactionsthatVarying();
                LoadData(iTransID);
                txtInvAutoNo.Select();
            }
            else
                SetTransactionsthatVarying();

            dgvStockIn.Controls.Add(dtp);
            dtp.Visible = false;
            dtp.Format = DateTimePickerFormat.Custom;
            
            lblPause.Text = "Pause";

        }

        #region "VARIABLES --------------------------------------------- >>"

        Control ctrl;
        string sEditedValueonKeyPress, sBatchCode;
        int iBatchmode, iShelfLifeDays, iPurGridID = 0;
        DateTime dtCurrExp;
        int iAction = 0, iIDFromEditWindow, vchtypeID;
        decimal dSupplierID = 0, dUnitID = 0;
        bool dragging = false;
        int xOffset = 0, yOffset = 0, d = 0;
        string strCheck = "", strSelectedItemName = "", sgblBarcodeNoExists;
        int iprevVchNo, iNextVchNo;
        bool bFromEditRepacking;
        decimal dCostRateInc = 0, dCostRateExcl = 0, dPRateIncl = 0, dPRateExcl = 0;
        decimal dSteadyBillDiscPerc, dSteadyBillDiscAmt;

        static int namesCount = Enum.GetNames(typeof(LedgerIndexes)).Length;
        string[] sArrLedger = new string[namesCount];
        Common Comm = new Common();


        UspGetItemMasterInfo GetItmMstinfo = new UspGetItemMasterInfo();
        UspGetItemMasterFromStockInfo GetItmMststockinfo = new UspGetItemMasterFromStockInfo();
        UspGetEmployeeInfo GetEmpInfo = new UspGetEmployeeInfo();
        UspGetCostCentreInfo GetCctinfo = new UspGetCostCentreInfo();
        UspGetTaxModeInfo GetTaxMinfo = new UspGetTaxModeInfo();
        UspGetAgentinfo GetAgentinfo = new UspGetAgentinfo();
        UspGetLedgerInfo GetLedinfo = new UspGetLedgerInfo();
        UspGetUnitInfo GetUnitInfo = new UspGetUnitInfo();
        UspGetStockDetailsInfo GetStockInfo = new UspGetStockDetailsInfo();
        UspGetRepackingInfo GetRepackingIfo = new UspGetRepackingInfo();

        clsItemMaster clsItmMst = new clsItemMaster();
        clsEmployee clsEmp = new clsEmployee();
        clsCostCentre clscct = new clsCostCentre();
        clsTaxMode clsTax = new clsTaxMode();
        clsAgentMaster clsAgent = new clsAgentMaster();
        clsLedger clsLedg = new clsLedger();
        clsUnitMaster clsUnit = new clsUnitMaster();
        clsStockDetails clsStock = new clsStockDetails();

        clsJSonCommon JSonComm = new clsJSonCommon();
        clsRepacking clsPur = new clsRepacking();

        //Repacking Master Related Classes for Json
        clsJSonRepacking clsPM = new clsJSonRepacking();
        clsJsonPMInfo clsJPMinfo = new clsJsonPMInfo();
        clsJsonPMLedgerInfo clsJPMLedgerinfo = new clsJsonPMLedgerInfo();
        clsJsonPMTaxmodeInfo clsJPMTaxModinfo = new clsJsonPMTaxmodeInfo();
        clsJsonPMAgentInfo clsJPMAgentinfo = new clsJsonPMAgentInfo();
        clsJsonPMCCentreInfo clsJPMCostCentreinfo = new clsJsonPMCCentreInfo();
        clsJsonPMEmployeeInfo clsJPMEmployeeinfo = new clsJsonPMEmployeeInfo();
        clsJsonPMStateInfo clsJPMStateinfo = new clsJsonPMStateInfo();

        //Repacking Detail Related Classes For Json
        clsJsonPDetailsInfo clsJPDinfo = new clsJsonPDetailsInfo();
        clsJsonPDUnitinfo clsJPDUnitinfo = new clsJsonPDUnitinfo();
        clsJsonPDIteminfo clsJPDIteminfo = new clsJsonPDIteminfo();

        DataTable dtItemPublic = new DataTable();
        DataTable dtUnitPublic = new DataTable();
        DataTable dtBatchCode = new DataTable();
        DataTable dtBatchCodeData = new DataTable();

        DateTimePicker dtp = new DateTimePicker();
        DateTimePicker dtpOut = new DateTimePicker();
        Rectangle _Rectangle;

        clsJsonVoucherType clsVchType = new clsJsonVoucherType();
        clsGetStockInVoucherSettings clsVchTypeFeatures = new clsGetStockInVoucherSettings();

        enum GridColIndexes
        {
            cSlNo, //0
            CItemCode,
            CItemName,
            CUnit,
            cBarCode,
            CExpiry,
            cMRP,
            cPrate,
            cRateinclusive,
            cQty,
            cFree,
            cSRate1Per,
            cSRate1,
            cSRate2Per,
            cSRate2,
            cSRate3Per,
            cSRate3,
            cSRate4Per,
            cSRate4,
            cSRate5Per,
            cSRate5,
            cGrossAmt,
            cDiscPer,
            cDiscAmount,
            cBillDisc,
            cCrate,
            cCRateWithTax,
            ctaxable,
            ctaxPer,
            ctax,
            cIGST,
            cSGST,
            cCGST,
            cNetAmount,
            cItemID,
            cGrossValueAfterRateDiscount,
            cNonTaxable,
            cCCessPer,
            cCCompCessQty,
            cFloodCessPer,
            cFloodCessAmt,
            cStockMRP,
            cAgentCommPer,
            cCoolie,
            cBlnOfferItem,
            cStrOfferDetails,
            cBatchMode,
            cID,
            cImgDel,
        }

        enum GridBottomColumns
        {
            QtyTotal, //0
            GrossAmt,
            GrossAfterRateDiscount,
            RateDiscountTotal,
            BillDisc,
            GrossAfterItemDiscount,
            ItemDiscountTotal,
            TaxableAmount,
            NonTaxableAmount,
            TaxAmount,
            VatTotal,
            INTERSTATE,
            GSTType,
            CGST,
            SGST,
            IGST,
            CessAmount,
            FloodCessTotal,
            QtyCompCessAmount,
            NetAmount,
            AgentCommission,
            AgentCommissionMode,
            Coolie,
            Savings

        }

        enum LedgerIndexes
        {
            LID, //0
            LName,
            LAliasName,
            GroupName,
            Type,
            OpBalance,
            AppearIn,
            Address,
            CreditDays,
            Phone,
            TaxNo,
            AccountGroupID,
            RouteID,
            Area,
            Notes,
            TargetAmt,
            SMSSchID,
            Email,
            MobileNo,
            DiscPer,
            InterestPer,
            DummyLName,
            BlnBank,
            CurrencyID,
            AreaID,
            PLID,
            ActiveStatus,
            EmailAddress,
            EntryDate,
            blnBillWise,
            CustomerCardID,
            TDSPer,
            DOB,
            StateID,
            CCIDS,
            CurrentBalance,
            LedgerName,
            LedgerCode,
            BlnWallet,
            blnCoupon,
            TransComn,
            BlnSmsWelcome,
            DLNO,
            TDS,
            LedgerNameUnicode,
            LedgerAliasNameUnicode,
            ContactPerson,
            TaxParameter,
            TaxParameterType,
            HSNCODE,
            CGSTTaxPer,
            SGSTTaxPer,
            IGSTTaxPer,
            HSNID,
            BankAccountNo,
            BankIFSCCode,
            BankNote,
            WhatsAppNo,
            SystemName,
            UserID,
            LastUpdateDate,
            LastUpdateTime,
            TenantID,
            GSTType,
            AgentID
        }

        enum AgentIndexes
        {
            AgentID, //0
            AgentCode,
            AgentName,
            Area,
            Commission,
            blnPOstAccounts,
            ADDRESS,
            LOCATION,
            PHONE,
            WEBSITE,
            EMAIL,
            BLNROOMRENT,
            BLNSERVICES,
            blnItemwiseCommission,
            AgentDiscount,
            LID,
            SystemName,
            UserID,
            LastUpdateDate,
            LastUpdateTime,
            TenantID
        }

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
                    AddColumnsToGrid(dgvStockIn);
                    AddColumnsToGrid(dgvStockOut);
                    FillCostCentre();
                }

                SetTransactionDefaults();
                SetApplicationSettings(dgvStockOut);
                SetApplicationSettings(dgvStockIn);

                Application.DoEvents();

                GridInitialize_dgvColWidth(true, dgvStockIn);
                GridInitialize_dgvColWidth(true, dgvStockOut);

                this.tlpMain.ColumnStyles[1].SizeType = SizeType.Absolute;
                this.tlpMain.ColumnStyles[1].Width = 0;

                if (iIDFromEditWindow == 0)
                {
                    if (txtPrefix.Visible == true)
                    {
                        txtPrefix.Focus();
                        txtPrefix.Select();
                    }
                    else
                    {
                        txtInvAutoNo.Focus();
                        txtInvAutoNo.Select();
                    }
                }
                else
                {
                    LoadData(iIDFromEditWindow);

                    int iRowCnt = dgvStockIn.Rows.Count;
                    dgvStockIn.CurrentCell = dgvStockIn.Rows[iRowCnt - 1].Cells[GetEnum(GridColIndexes.CItemCode)];
                    dgvStockOut.CurrentCell = dgvStockIn.Rows[iRowCnt - 1].Cells[GetEnum(GridColIndexes.CItemCode)];
                    dgvStockOut.Focus();
                    SendKeys.Send("{down}");
                }
                dgvStockIn.Columns["cRateinclusive"].Visible = false;

                //DisableGridSettingsCheckbox();

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

        private void dgvStockIn_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            decimal dResult = 0;
            try
            {
                if (dgvStockIn.CurrentCell.ColumnIndex == GetEnum(GridColIndexes.cQty))
                {
                    dResult = Comm.ToDecimal(dgvStockIn.Rows[dgvStockIn.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cQty)].Value);
                    SetValue(GetEnum(GridColIndexes.cQty), dResult.ToString(), "QTY_FLOAT");

                    if (dgvStockIn.Rows.Count - 1 == dgvStockIn.CurrentRow.Index)
                        dgvStockIn.Rows.Add();

                    //Added by Anjitha 28/01/2022 5:30 PM
                    bool bshellife = ShelfLifeEffect();
                    if (bshellife == false)
                    {
                        dgvStockIn.Focus();
                        SetValue(GetEnum(GridColIndexes.CExpiry), dtCurrExp.ToString("dd-MMM-yyyy").ToString(), "DATE");
                    }

                    SendKeys.Send("{up}");
                    SendKeys.Send("{right}");
                }
                else if (dgvStockIn.CurrentCell.ColumnIndex == GetEnum(GridColIndexes.cFree))
                {
                    dResult = Comm.ToDecimal(dgvStockIn.Rows[dgvStockIn.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cFree)].Value);
                    SetValue(GetEnum(GridColIndexes.cFree), dResult.ToString(), "QTY_FLOAT");

                    SendKeys.Send("{up}");
                    SendKeys.Send("{right}");
                }
                else if (dgvStockIn.CurrentCell.ColumnIndex == GetEnum(GridColIndexes.cDiscPer))
                {
                    dResult = Comm.ToDecimal(dgvStockIn.Rows[dgvStockIn.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cGrossAmt)].Value) * (Comm.ToDecimal(dgvStockIn.Rows[dgvStockIn.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cDiscPer)].Value) / 100);

                    dgvStockIn.CellEndEdit -= dgvStockIn_CellEndEdit;
                    SetValue(GetEnum(GridColIndexes.cDiscAmount), dResult.ToString(), "CURR_FLOAT");
                    dgvStockIn.CellEndEdit += dgvStockIn_CellEndEdit;

                    SendKeys.Send("{up}");
                    SendKeys.Send("{right}");
                }
                else if (dgvStockIn.CurrentCell.ColumnIndex == GetEnum(GridColIndexes.cDiscAmount))
                {
                    dResult = (Comm.ToDecimal(dgvStockIn.Rows[dgvStockIn.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cDiscAmount)].Value) * 100) / Comm.ToDecimal(dgvStockIn.Rows[dgvStockIn.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cGrossAmt)].Value);

                    dgvStockIn.CellEndEdit -= dgvStockIn_CellEndEdit;
                    SetValue(GetEnum(GridColIndexes.cDiscPer), "0", "");
                    dgvStockIn.CellEndEdit += dgvStockIn_CellEndEdit;

                    SendKeys.Send("{up}");
                    SendKeys.Send("{right}");
                }
                else if (dgvStockIn.CurrentCell.ColumnIndex == GetEnum(GridColIndexes.cMRP))
                {
                    SendKeys.Send("{up}");
                    SendKeys.Send("{right}");
                }
                else if (dgvStockIn.CurrentCell.ColumnIndex == GetEnum(GridColIndexes.cPrate))
                {
                    SendKeys.Send("{up}");
                    SendKeys.Send("{right}");
                }
                else if (dgvStockIn.CurrentCell.ColumnIndex == GetEnum(GridColIndexes.cSRate1Per)) 
                {
                    SendKeys.Send("{up}");
                    SendKeys.Send("{right}");
                }
                else if (dgvStockIn.CurrentCell.ColumnIndex == GetEnum(GridColIndexes.cSRate2Per))                
                {
                    SendKeys.Send("{up}");
                    SendKeys.Send("{right}");
                }
                else if (dgvStockIn.CurrentCell.ColumnIndex == GetEnum(GridColIndexes.cSRate3Per))                
                {
                    SendKeys.Send("{up}");
                    SendKeys.Send("{right}");
                }
                else if (dgvStockIn.CurrentCell.ColumnIndex == GetEnum(GridColIndexes.cSRate4Per))                
                {
                    SendKeys.Send("{up}");
                    SendKeys.Send("{right}");
                }
                else if (dgvStockIn.CurrentCell.ColumnIndex == GetEnum(GridColIndexes.cSRate5Per))                
                {
                    SendKeys.Send("{up}");
                    SendKeys.Send("{right}");
                }
                else if (dgvStockIn.CurrentCell.ColumnIndex == GetEnum(GridColIndexes.cSRate1))                
                {
                    SendKeys.Send("{up}");
                    SendKeys.Send("{right}");
                }
                else if (dgvStockIn.CurrentCell.ColumnIndex == GetEnum(GridColIndexes.cSRate2))                
                {
                    SendKeys.Send("{up}");
                    SendKeys.Send("{right}");
                }
                else if (dgvStockIn.CurrentCell.ColumnIndex == GetEnum(GridColIndexes.cSRate3))                
                {
                    SendKeys.Send("{up}");
                    SendKeys.Send("{right}");
                }
                else if (dgvStockIn.CurrentCell.ColumnIndex == GetEnum(GridColIndexes.cSRate4))                
                {
                    SendKeys.Send("{up}");
                    SendKeys.Send("{right}");
                }
                else if (dgvStockIn.CurrentCell.ColumnIndex == GetEnum(GridColIndexes.cSRate5))
                {
                    SendKeys.Send("{up}");
                    SendKeys.Send("{right}");
                }



                #region "Srate Calculation on vale changing in cells"

                //If the tag value of srate colums are 1, it won't get calculated in calctotal function.
                //Else srate(s) will be forward calculated according to percentages
                //Tag vale is set to "1" if the user enters vale in srate columns.

                if (dgvStockIn.Rows[dgvStockIn.CurrentCell.RowIndex].Cells[GetEnum(GridColIndexes.cSRate1)].Tag == null)
                    dgvStockIn.Rows[dgvStockIn.CurrentCell.RowIndex].Cells[GetEnum(GridColIndexes.cSRate1)].Tag = "";
                if (dgvStockIn.Rows[dgvStockIn.CurrentCell.RowIndex].Cells[GetEnum(GridColIndexes.cSRate2)].Tag == null)
                    dgvStockIn.Rows[dgvStockIn.CurrentCell.RowIndex].Cells[GetEnum(GridColIndexes.cSRate2)].Tag = "";
                if (dgvStockIn.Rows[dgvStockIn.CurrentCell.RowIndex].Cells[GetEnum(GridColIndexes.cSRate3)].Tag == null)
                    dgvStockIn.Rows[dgvStockIn.CurrentCell.RowIndex].Cells[GetEnum(GridColIndexes.cSRate3)].Tag = "";
                if (dgvStockIn.Rows[dgvStockIn.CurrentCell.RowIndex].Cells[GetEnum(GridColIndexes.cSRate4)].Tag == null)
                    dgvStockIn.Rows[dgvStockIn.CurrentCell.RowIndex].Cells[GetEnum(GridColIndexes.cSRate4)].Tag = "";
                if (dgvStockIn.Rows[dgvStockIn.CurrentCell.RowIndex].Cells[GetEnum(GridColIndexes.cSRate5)].Tag == null)
                    dgvStockIn.Rows[dgvStockIn.CurrentCell.RowIndex].Cells[GetEnum(GridColIndexes.cSRate5)].Tag = "";

                if (dgvStockIn.CurrentCell.ColumnIndex == GetEnum(GridColIndexes.cSRate1Per))
                    SetTag(GetEnum(GridColIndexes.cSRate1), dgvStockIn.CurrentRow.Index, "");
                else if (dgvStockIn.CurrentCell.ColumnIndex == GetEnum(GridColIndexes.cSRate2Per))
                    SetTag(GetEnum(GridColIndexes.cSRate2), dgvStockIn.CurrentRow.Index, "");
                else if (dgvStockIn.CurrentCell.ColumnIndex == GetEnum(GridColIndexes.cSRate3Per))
                    SetTag(GetEnum(GridColIndexes.cSRate3), dgvStockIn.CurrentRow.Index, "");
                else if (dgvStockIn.CurrentCell.ColumnIndex == GetEnum(GridColIndexes.cSRate4Per))
                    SetTag(GetEnum(GridColIndexes.cSRate4), dgvStockIn.CurrentRow.Index, "");
                else if (dgvStockIn.CurrentCell.ColumnIndex == GetEnum(GridColIndexes.cSRate5Per))
                    SetTag(GetEnum(GridColIndexes.cSRate5), dgvStockIn.CurrentRow.Index, "");

                if (dgvStockIn.CurrentCell.ColumnIndex == GetEnum(GridColIndexes.cSRate1))
                {
                    SetValue(GetEnum(GridColIndexes.cSRate1Per), dgvStockIn.CurrentRow.Index, CalculateSratePercentageOnSrate(dgvStockIn.CurrentCell.RowIndex, dgvStockIn.CurrentCell.ColumnIndex));
                    SetTag(GetEnum(GridColIndexes.cSRate1), dgvStockIn.CurrentRow.Index, "1");
                }
                else if (dgvStockIn.CurrentCell.ColumnIndex == GetEnum(GridColIndexes.cSRate2))
                {
                    SetValue(GetEnum(GridColIndexes.cSRate2Per), dgvStockIn.CurrentRow.Index, CalculateSratePercentageOnSrate(dgvStockIn.CurrentCell.RowIndex, dgvStockIn.CurrentCell.ColumnIndex));
                    SetTag(GetEnum(GridColIndexes.cSRate2), dgvStockIn.CurrentRow.Index, "1");
                }
                else if (dgvStockIn.CurrentCell.ColumnIndex == GetEnum(GridColIndexes.cSRate3))
                {
                    SetValue(GetEnum(GridColIndexes.cSRate3Per), dgvStockIn.CurrentRow.Index, CalculateSratePercentageOnSrate(dgvStockIn.CurrentCell.RowIndex, dgvStockIn.CurrentCell.ColumnIndex));
                    SetTag(GetEnum(GridColIndexes.cSRate3), dgvStockIn.CurrentRow.Index, "1");
                }
                else if (dgvStockIn.CurrentCell.ColumnIndex == GetEnum(GridColIndexes.cSRate4))
                {
                    SetValue(GetEnum(GridColIndexes.cSRate4Per), dgvStockIn.CurrentRow.Index, CalculateSratePercentageOnSrate(dgvStockIn.CurrentCell.RowIndex, dgvStockIn.CurrentCell.ColumnIndex));
                    SetTag(GetEnum(GridColIndexes.cSRate4), dgvStockIn.CurrentRow.Index, "1");
                }
                else if (dgvStockIn.CurrentCell.ColumnIndex == GetEnum(GridColIndexes.cSRate5))
                {
                    SetValue(GetEnum(GridColIndexes.cSRate5Per), dgvStockIn.CurrentRow.Index, CalculateSratePercentageOnSrate(dgvStockIn.CurrentCell.RowIndex, dgvStockIn.CurrentCell.ColumnIndex));
                    SetTag(GetEnum(GridColIndexes.cSRate5), dgvStockIn.CurrentRow.Index, "1");
                }

                #endregion

                this.dgvEndEditCell = dgvStockIn[e.ColumnIndex, e.RowIndex];
                if (dgvStockIn.Rows.Count == e.RowIndex && e.ColumnIndex != dgvStockIn.Columns.Count - 1 && e.ColumnIndex <= GetEnum(GridColIndexes.cDiscAmount))
                {
                    if (dgvStockIn.CurrentCell.ColumnIndex != GetEnum(GridColIndexes.cRateinclusive))
                        SendKeys.Send("{Tab}");
                }
                else if (e.ColumnIndex == GetEnum(GridColIndexes.cDiscAmount))
                {
                    dgvStockIn.CurrentCell = dgvStockIn[GetEnum(GridColIndexes.CItemCode), e.RowIndex + 1];
                }
                else if (e.ColumnIndex >= GetEnum(GridColIndexes.cSRate1Per) && e.ColumnIndex < GetEnum(GridColIndexes.cDiscAmount))
                {
                    //SendKeys.Send("{up}");
                    //SendKeys.Send("{right}");
                }
                CalcTotal();
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
        }

        private string CalculateSratePercentageOnSrate(int RowIndex, int ColIndex)
        {
            if (ColIndex == GetEnum(GridColIndexes.cSRate1) ||
                ColIndex == GetEnum(GridColIndexes.cSRate2) ||
                ColIndex == GetEnum(GridColIndexes.cSRate3) ||
                ColIndex == GetEnum(GridColIndexes.cSRate4) ||
                ColIndex == GetEnum(GridColIndexes.cSRate5))
            {
                double dblcSRate = Comm.ToDouble(dgvStockIn.Rows[RowIndex].Cells[ColIndex].Value);

                double dblcRate = Comm.ToDouble(dgvStockIn.Rows[dgvStockIn.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cPrate)].Value);
                double dblcCRate = Comm.ToDouble(dgvStockIn.Rows[dgvStockIn.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cCrate)].Value);
                double dblcMRP = Comm.ToDouble(dgvStockIn.Rows[dgvStockIn.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cMRP)].Value);
                double dblcCRateWithTax = Comm.ToDouble(dgvStockIn.Rows[dgvStockIn.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cCRateWithTax)].Value);

                switch (Comm.ToDouble(dgvStockIn.Rows[dgvStockIn.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cSRate1Per)].Tag)) //SrateCalcMode
                {
                    case 0:
                        if (dblcSRate > 0)
                            return Comm.FormatValue(((dblcSRate - dblcRate) * 100) / dblcRate).ToString();
                        else
                            return 0.ToString();
                    case 3:
                        if (dblcSRate > 0) 
                            return Comm.FormatValue(((dblcSRate - dblcCRate) * 100) / dblcCRate).ToString();
                        else
                            return 0.ToString();
                    case 1:
                        if (dblcSRate > 0) 
                            return Comm.FormatValue(((dblcMRP - dblcSRate) * 100) / dblcMRP).ToString();
                        else
                            return 0.ToString();
                    case 2:
                        if (dblcSRate > 0) 
                            return Comm.FormatValue(((dblcSRate - dblcCRateWithTax) * 100) / dblcCRateWithTax).ToString();
                        else
                            return 0.ToString();
                    default:
                        return 0.ToString();
                }
            }
            else
                return 0.ToString();
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            if (dgvStockIn.Rows.Count > 0)
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

        private void txtReferenceNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
            {
                    dtpInvDate.Focus();
            }
            else if (e.KeyCode == Keys.Enter)
            {
                txtReferenceAutoNo.Focus();
                txtReferenceAutoNo.SelectAll();
            }
        }


        private void dgvStockIn_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        ComboBox BatchCode_GridCellComboBox = new ComboBox();
        private void dgvStockIn_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            try
            {
                if (dgvStockIn.CurrentCell.EditType == typeof(DataGridViewTextBoxEditingControl))
                {
                    if (dgvStockIn.CurrentCell.ColumnIndex == GetEnum(GridColIndexes.CItemCode))
                    {
                        DataGridViewTextBoxEditingControl tb = (DataGridViewTextBoxEditingControl)e.Control;
                        tb.KeyPress += new KeyPressEventHandler(dgvStockIn_TextBox_KeyPress);
                        e.Control.KeyPress += new KeyPressEventHandler(dgvStockIn_TextBox_KeyPress);
                    }
                    else if (dgvStockIn.CurrentCell.ColumnIndex == GetEnum(GridColIndexes.cBarCode))
                    {
                        CallBatchCodeCompact();

                        if (dgvStockIn.Rows[dgvStockIn.CurrentRow.Index].Cells[GetEnum(GridColIndexes.CExpiry)].Visible == true)
                            dgvStockIn.CurrentCell = dgvStockIn.Rows[dgvStockIn.CurrentRow.Index].Cells[GetEnum(GridColIndexes.CExpiry)];
                        else
                            dgvStockIn.CurrentCell = dgvStockIn.Rows[dgvStockIn.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cQty)];

                        dgvStockIn.Focus();
                    }
                    else if (dgvStockIn.CurrentCell.ColumnIndex >= GetEnum(GridColIndexes.cMRP) && dgvStockIn.CurrentCell.ColumnIndex < GetEnum(GridColIndexes.cNetAmount))
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

        private void dgvStockIn_TextBox_KeyPress(object sender, KeyPressEventArgs e)
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

                string EnterText = "";
                if (sender != null)
                {
                    TextBox tb = (TextBox)sender;

                    if (((int)e.KeyChar >= 65 && (int)e.KeyChar <= 90) || ((int)e.KeyChar >= 97 && (int)e.KeyChar <= 122) || ((int)e.KeyChar >= 48 && (int)e.KeyChar <= 57))
                        EnterText = EnterText + e.KeyChar;
                }

                sEditedValueonKeyPress = e.KeyChar.ToString();
                if (dgvStockIn.CurrentCell.ColumnIndex == (int)GridColIndexes.CItemCode)
                {
                    if (sEditedValueonKeyPress != null)
                    {
                        if (AppSettings.TaxMode == 2) //GST
                        {
                            sQuery = "SELECT ItemCode + ItemName + BatchUnique + CAST(MRP AS varchar) AS anywhere, ItemCode, ItemName, BatchUnique, QOH, MRP, ItemID, StockID " +
                                    " FROM     vwCompactSearchPurchase Where isnull(ActiveStatus, 1) = 1 and isnull(StockActiveStatus, 1) = 1 ";
                            if (clsVchType.ProductClassList != "")
                                sQuery = sQuery + " AND ProductTypeID IN (" + clsVchType.ProductClassList + ")";
                            if (clsVchType.ItemCategoriesList != "")
                                sQuery = sQuery + " AND CategoryID IN (" + clsVchType.ItemCategoriesList + ")";
                            //new frmDetailedSearch2(GetFromItemSearch, sQuery, "Anywhere|ItemCode|ItemName|BatchUnique", dgvStockIn.Location.X + ((dgvStockIn.Width / 2) - 500), dgvStockIn.Location.Y, 7, 0, EnterText, 6, 0, "ORDER BY ItemCode ASC, Batchunique", 0, 0, "Item Search...", 0, "150,250,150,150,150,0,0", true, "frmItemMaster", 20, false, this.MdiParent, 6).ShowDialog(this.MdiParent);

                            if (mblnInitialisedSubWindow == false)
                            {
                                mblnInitialisedSubWindow = true;
                                frmDetailedSearch2 frmN = new frmDetailedSearch2(GetFromItemSearch, sQuery, "Anywhere|ItemCode|ItemName|BatchUnique", dgvStockOut.Location.X + ((dgvStockOut.Width / 2) - 500), dgvStockOut.Location.Y, 7, 0, EnterText, 6, 0, "ORDER BY ItemCode ASC, Batchunique", 0, 0, "Item Search...", 0, "150,250,150,150,150,0,0", true, "frmItemMaster", 20, false, this.MdiParent, 6);
                                frmN.MdiParent = this.MdiParent;
                                frmN.Show(); //12-SEP-2022
                            }
                        }
                        else
                        {
                            sQuery = "SELECT ItemCode + ItemName + BatchUnique + CAST(MRP AS varchar) AS anywhere, ItemCode, ItemName, BatchUnique, QOH, MRP, ItemID, StockID " +
                                    " FROM     vwCompactSearchPurchase Where isnull(ActiveStatus, 1) = 1 and isnull(StockActiveStatus, 1) = 1 ";
                            if (clsVchType.ProductClassList != "")
                                sQuery = sQuery + " AND ProductTypeID IN (" + clsVchType.ProductClassList + ")";
                            if (clsVchType.ItemCategoriesList != "")
                                sQuery = sQuery + " AND CategoryID IN (" + clsVchType.ItemCategoriesList + ")";
                            //new frmDetailedSearch2(GetFromItemSearch, sQuery, "Anywhere|ItemCode|ItemName|BatchUnique", dgvStockIn.Location.X + ((dgvStockIn.Width / 2) - 500), dgvStockIn.Location.Y, 7, 0, EnterText, 6, 0, "ORDER BY ItemCode ASC, Batchunique", 0, 0, "Item Search...", 0, "150,250,150,150,150,0,0", true, "frmItemMaster", 20, false, this.MdiParent, 6).ShowDialog(this.MdiParent);

                            if (mblnInitialisedSubWindow == false)
                            {
                                mblnInitialisedSubWindow = true;
                                frmDetailedSearch2 frmN = new frmDetailedSearch2(GetFromItemSearch, sQuery, "Anywhere|ItemCode|ItemName|BatchUnique", dgvStockOut.Location.X + ((dgvStockOut.Width / 2) - 500), dgvStockOut.Location.Y, 7, 0, EnterText, 6, 0, "ORDER BY ItemCode ASC, Batchunique", 0, 0, "Item Search...", 0, "150,250,150,150,150,0,0", true, "frmItemMaster", 20, false, this.MdiParent, 6);
                                frmN.MdiParent = this.MdiParent;
                                frmN.Show(); //12-SEP-2022
                            }
                        }

                        if (dgvStockIn.CurrentRow != null)
                        {
                            if (dgvStockIn.Rows[dgvStockIn.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cItemID)].Value != null)
                            {
                                this.dgvStockIn.EditingControlShowing -= this.dgvStockIn_EditingControlShowing;

                                if (dgvStockIn.Rows[dgvStockIn.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cBarCode)].Visible == true)
                                    dgvStockIn.CurrentCell = dgvStockIn.Rows[dgvStockIn.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cBarCode)];
                                else
                                    dgvStockIn.CurrentCell = dgvStockIn.Rows[dgvStockIn.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cQty)];


                                dgvStockIn.Focus();
                                this.dgvStockIn.EditingControlShowing += this.dgvStockIn_EditingControlShowing;
                            }
                        }
                    }
                }
                else if (dgvStockIn.CurrentCell.ColumnIndex == (int)GridColIndexes.cBarCode)
                {
                    //sEditedValueonKeyPress = "~";
                    if (dgvStockIn.Rows[dgvStockIn.CurrentRow.Index].Cells[(int)GridColIndexes.cBarCode].Value != null)
                        sEditedValueonKeyPress = dgvStockIn.Rows[dgvStockIn.CurrentRow.Index].Cells[(int)GridColIndexes.cBarCode].Value.ToString();
                    else
                        sEditedValueonKeyPress = "";
                    if (sEditedValueonKeyPress != null)
                    {
                        if (dgvStockIn.Rows[dgvStockIn.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cItemID)].Value != null)
                        {
                            Form fcC = Application.OpenForms["frmDetailedSearch2"];
                            if (fcC != null)
                            {
                                fcC.Focus();
                                fcC.BringToFront();
                                return;
                            }

                            CallBatchCodeCompact();

                            if (dgvStockIn.Rows[dgvStockIn.CurrentRow.Index].Cells[GetEnum(GridColIndexes.CExpiry)].Visible == true)
                                dgvStockIn.CurrentCell = dgvStockIn.Rows[dgvStockIn.CurrentRow.Index].Cells[GetEnum(GridColIndexes.CExpiry)];
                            else
                                dgvStockIn.CurrentCell = dgvStockIn.Rows[dgvStockIn.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cQty)];

                            dgvStockIn.Focus();
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

        private void dgvStockOut_TextBox_KeyPress(object sender, KeyPressEventArgs e)
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

                string EnterText = "";
                if (sender != null)
                {
                    TextBox tb = (TextBox)sender;

                    if (((int)e.KeyChar >= 65 && (int)e.KeyChar <= 90) || ((int)e.KeyChar >= 97 && (int)e.KeyChar <= 122) || ((int)e.KeyChar >= 48 && (int)e.KeyChar <= 57))
                        EnterText = EnterText + e.KeyChar;
                }

                sEditedValueonKeyPress = e.KeyChar.ToString();
                if (dgvStockOut.CurrentCell.ColumnIndex == (int)GridColIndexes.CItemCode)
                {
                    if (sEditedValueonKeyPress != null)
                    {
                        if (AppSettings.TaxMode == 2) //GST
                        {
                            //sQuery = "SELECT ItemCode + ItemName + BatchUnique + CAST(MRP AS varchar) AS anywhere, ItemCode, ItemName, BatchUnique, QOH, MRP, ItemID, StockID " +
                            //        " FROM     vwCompactSearchPurchase Where isnull(ActiveStatus, 1) = 1 and isnull(StockActiveStatus, 1) = 1 ";
                            sQuery = "SELECT TOP 200 ItemCode + ItemName + BatchUnique + CAST(MRP AS varchar) AS searchanywhere, ItemCode, ItemName, BatchUnique, QOH, MRP, ItemID, StockID " +
                                    " FROM     vwCompactSearch Where isnull(ActiveStatus, 1) = 1 and isnull(StockActiveStatus, 1) = 1 and BatchUnique <> '<AUTO BARCODE>' ";
                            if (clsVchType.ProductClassList != "")
                                sQuery = sQuery + " AND ProductTypeID IN (" + clsVchType.ProductClassList + ")";
                            if (clsVchType.ItemCategoriesList != "")
                                sQuery = sQuery + " AND CategoryID IN (" + clsVchType.ItemCategoriesList + ")";
                            //new frmDetailedSearch2(GetFromItemSearchOut, sQuery, "Anywhere|ItemCode|ItemName|BatchUnique", dgvStockOut.Location.X  + ((dgvStockOut.Width / 2) - 500), dgvStockOut.Location.Y, 7, 0, EnterText, 6, 0, "ORDER BY ItemCode ASC, Batchunique", 0, 0, "Item Search...", 0, "150,250,150,150,150,0,0", true, "frmItemMaster", 20, false, this.MdiParent, 6).ShowDialog(this.MdiParent);

                            if (mblnInitialisedSubWindow == false)
                            {
                                mblnInitialisedSubWindow = true;
                                frmDetailedSearch2 frmN = new frmDetailedSearch2(GetFromItemSearchOut, sQuery, "Anywhere|ItemCode|ItemName|BatchUnique", dgvStockIn.Location.X + ((dgvStockIn.Width / 2) - 500), dgvStockIn.Location.Y, 7, 0, EnterText, 6, 0, "ORDER BY ItemCode ASC, Batchunique", 0, 0, "Item Search...", 0, "150,250,150,150,150,0,0", true, "frmItemMaster", 20, false, this.MdiParent, 6);
                                frmN.MdiParent = this.MdiParent;
                                frmN.Show(); //12-SEP-2022
                            }
                        }
                        else
                        {
                            //sQuery = "SELECT ItemCode + ItemName + BatchUnique + CAST(MRP AS varchar) AS anywhere, ItemCode, ItemName, BatchUnique, QOH, MRP, ItemID, StockID " +
                            //        " FROM     vwCompactSearchPurchase Where isnull(ActiveStatus, 1) = 1 and isnull(StockActiveStatus, 1) = 1 ";

                            sQuery = "SELECT TOP 200 ItemCode + ItemName + BatchUnique + CAST(MRP AS varchar) AS searchanywhere, ItemCode, ItemName, BatchUnique, QOH, MRP, ItemID, StockID " +
                                    " FROM     vwCompactSearch Where isnull(ActiveStatus, 1) = 1 and isnull(StockActiveStatus, 1) = 1 and BatchUnique <> '<AUTO BARCODE>' ";

                            if (clsVchType.ProductClassList != "")
                                sQuery = sQuery + " AND ProductTypeID IN (" + clsVchType.ProductClassList + ")";
                            if (clsVchType.ItemCategoriesList != "")
                                sQuery = sQuery + " AND CategoryID IN (" + clsVchType.ItemCategoriesList + ")";
                            //new frmDetailedSearch2(GetFromItemSearchOut, sQuery, "Anywhere|ItemCode|ItemName|BatchUnique", dgvStockOut.Location.X  + ((dgvStockOut.Width / 2) - 500), dgvStockOut.Location.Y, 7, 0, EnterText, 6, 0, "ORDER BY ItemCode ASC, Batchunique", 0, 0, "Item Search...", 0, "150,250,150,150,150,0,0", true, "frmItemMaster", 20, false, this.MdiParent, 6).ShowDialog(this.MdiParent);

                            if (mblnInitialisedSubWindow == false)
                            {
                                mblnInitialisedSubWindow = true;
                                frmDetailedSearch2 frmN = new frmDetailedSearch2(GetFromItemSearchOut, sQuery, "Anywhere|ItemCode|ItemName|BatchUnique", dgvStockIn.Location.X + ((dgvStockIn.Width / 2) - 500), dgvStockIn.Location.Y, 7, 0, EnterText, 6, 0, "ORDER BY ItemCode ASC, Batchunique", 0, 0, "Item Search...", 0, "150,250,150,150,150,0,0", true, "frmItemMaster", 20, false, this.MdiParent, 6);
                                frmN.MdiParent = this.MdiParent;
                                frmN.Show(); //12-SEP-2022
                            }
                        }

                        if (dgvStockOut.CurrentRow != null)
                        {
                            if (dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cItemID)].Value != null)
                            {
                                this.dgvStockOut.EditingControlShowing -= this.dgvStockOut_EditingControlShowing;

                                if (dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cBarCode)].Visible == true)
                                    dgvStockOut.CurrentCell = dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cBarCode)];
                                else
                                    dgvStockOut.CurrentCell = dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cQty)];


                                dgvStockOut.Focus();
                                this.dgvStockOut.EditingControlShowing += this.dgvStockOut_EditingControlShowing;
                            }
                        }
                    }
                }
                else if (dgvStockOut.CurrentCell.ColumnIndex == (int)GridColIndexes.cBarCode)
                {
                    //sEditedValueonKeyPress = "~";
                    if (dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[(int)GridColIndexes.cBarCode].Value != null)
                        sEditedValueonKeyPress = dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[(int)GridColIndexes.cBarCode].Value.ToString();
                    else
                        sEditedValueonKeyPress = "";
                    if (sEditedValueonKeyPress != null)
                    {
                        if (dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cItemID)].Value != null)
                        {
                            Form fcC = Application.OpenForms["frmDetailedSearch2"];
                            if (fcC != null)
                            {
                                fcC.Focus();
                                fcC.BringToFront();
                                return;
                            }

                            CallBatchCodeCompactOut();

                            if (dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(GridColIndexes.CExpiry)].Visible == true)
                                dgvStockOut.CurrentCell = dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(GridColIndexes.CExpiry)];
                            else
                                dgvStockOut.CurrentCell = dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cQty)];

                            dgvStockOut.Focus();
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

        private void dgvStockIn_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
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

        private void dgvStockIn_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Shift && e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    int iColumn = dgvStockIn.CurrentCell.ColumnIndex;
                    int iRow = dgvStockIn.CurrentCell.RowIndex;
                    if (iColumn == GetEnum(GridColIndexes.cRateinclusive))
                    {
                        if (dgvStockIn[GetEnum(GridColIndexes.cRateinclusive) - 1, iRow].Visible == true)
                            dgvStockIn.CurrentCell = dgvStockIn[GetEnum(GridColIndexes.cRateinclusive) - 1, iRow];
                        else
                            dgvStockIn.CurrentCell = dgvStockIn[1, iRow - 1];
                    }
                    else if (iColumn == dgvStockIn.Columns.Count - 1 && iRow != dgvStockIn.Rows.Count)
                        dgvStockIn.CurrentCell = dgvStockIn[1, iRow - 1];
                    else
                        SendKeys.Send("+{Tab}");
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    int iColumn = dgvStockIn.CurrentCell.ColumnIndex;
                    int iRow = dgvStockIn.CurrentCell.RowIndex;
                    if (iColumn == dgvStockIn.Columns.Count - 1 && iRow != dgvStockIn.Rows.Count)
                    {
                        dgvStockIn.CurrentCell = dgvStockIn[1, iRow + 1];
                    }
                    else if (iColumn == dgvStockIn.Columns.Count - 1 && iRow == dgvStockIn.Rows.Count)
                    {
                    }
                    else if (iColumn == GetEnum(GridColIndexes.cDiscAmount))
                    {
                        //Dipoos 22-03-2022----- >
                        dgvStockIn.Rows.Add();
                        dgvStockIn.CurrentCell = dgvStockIn[GetEnum(GridColIndexes.CItemCode), iRow + 1];
                    }
                    else if (iColumn == GetEnum(GridColIndexes.cRateinclusive))
                    {
                        if (dgvStockIn[GetEnum(GridColIndexes.cRateinclusive) + 1, iRow].Visible == true)
                            dgvStockIn.CurrentCell = dgvStockIn[GetEnum(GridColIndexes.cRateinclusive) + 1, iRow];
                    }
                    else
                    {
                        SendKeys.Send("{Tab}");
                        //SendKeys.Send("{up}");
                        //SendKeys.Send("{right}");
                    }
                }
                //else if (e.KeyCode == Keys.F3)
                //{
                //    if (dgvStockIn.CurrentCell.ColumnIndex == (int)GridColIndexes.CItemCode)
                //    {
                //        frmItemMaster frmim = new frmItemMaster(0, true, "S");
                //        frmim.ShowDialog();
                //    }
                //}
                //else if (e.KeyCode == Keys.F4)
                //{
                //    if (dgvStockIn.CurrentCell.ColumnIndex == (int)GridColIndexes.CItemCode)
                //    {
                //        int iSelectedItemID = 0;
                //        iSelectedItemID = Comm.ToInt32(dgvStockIn.Rows[dgvStockIn.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cItemID)].Value);
                //        if (iSelectedItemID > 0)
                //        {
                //            frmItemMaster frmIM = new frmItemMaster(iSelectedItemID, true, "E");
                //            frmIM.ShowDialog();
                //        }
                //    }
                //}
                else if (e.KeyCode == Keys.Delete)
                {
                    string SSelectedItemCode = Convert.ToString(dgvStockIn.Rows[dgvStockIn.CurrentRow.Index].Cells[GetEnum(GridColIndexes.CItemCode)].Value);
                    if (SSelectedItemCode != "")
                    {
                        DialogResult dlgResult = MessageBox.Show("Are you sure to delete Item Code[" + SSelectedItemCode + "] Permanently ?", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                        if (dlgResult == DialogResult.Yes)
                        {
                            Int32 selectedRowCount = dgvStockIn.Rows.GetRowCount(DataGridViewElementStates.Selected);
                            RowDelete();
                            //dipoos 21-03-2022
                            //if (dgvStockIn.Rows.Count < 2)
                            //    dgvStockIn.Rows.Add();
                            if (dgvStockIn.Rows.Count < 1)
                                dgvStockIn.Rows.Add();

                            CalcTotal();
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

                    if (dgvStockIn.CurrentCell.ColumnIndex == (int)GridColIndexes.CItemCode)
                    {
                        if (sEditedValueonKeyPress != null)
                        {
                            if (AppSettings.TaxMode == 2) //GST
                            {
                                sQuery = "SELECT ItemCode + ItemName + BatchUnique + CAST(MRP AS varchar) AS anywhere, ItemCode, ItemName, BatchUnique, QOH, MRP, ItemID, StockID " +
                                        " FROM     vwCompactSearchPurchase Where isnull(ActiveStatus, 1) = 1 and isnull(StockActiveStatus, 1) = 1 ";
                                if (clsVchType.ProductClassList != "")
                                    sQuery = sQuery + " AND ProductTypeID IN (" + clsVchType.ProductClassList + ")";
                                if (clsVchType.ItemCategoriesList != "")
                                    sQuery = sQuery + " AND CategoryID IN (" + clsVchType.ItemCategoriesList + ")";
                                //new frmDetailedSearch2(GetFromItemSearch, sQuery, "Anywhere|ItemCode|ItemName|BatchUnique", dgvStockIn.Location.X + ((dgvStockIn.Width / 2) - 500), dgvStockIn.Location.Y, 7, 0, dgvStockIn.CurrentCell.Value.ToString(), 6, 0, "ORDER BY ItemCode ASC, Batchunique", 0, 0, "Item Search...", 0, "150,250,150,150,150,0,0", true, "frmItemMaster", 20, false, this.MdiParent, 6).ShowDialog(this.MdiParent);

                                if (mblnInitialisedSubWindow == false)
                                {
                                    mblnInitialisedSubWindow = true;
                                    string enteredtext = "";
                                    if (dgvStockIn.CurrentCell.Value != null)
                                        enteredtext = dgvStockIn.CurrentCell.Value.ToString();
                                    frmDetailedSearch2 frmN = new frmDetailedSearch2(GetFromItemSearch, sQuery, "Anywhere|ItemCode|ItemName|BatchUnique", dgvStockOut.Location.X + ((dgvStockOut.Width / 2) - 500), dgvStockOut.Location.Y, 7, 0, enteredtext, 6, 0, "ORDER BY ItemCode ASC, Batchunique", 0, 0, "Item Search...", 0, "150,250,150,150,150,0,0", true, "frmItemMaster", 20, false, this.MdiParent, 6);
                                    frmN.MdiParent = this.MdiParent;
                                    frmN.Show(); //12-SEP-2022
                                }
                            }
                            else
                            {
                                sQuery = "SELECT ItemCode + ItemName + BatchUnique + CAST(MRP AS varchar) AS anywhere, ItemCode, ItemName, BatchUnique, QOH, MRP, ItemID, StockID " +
                                        " FROM     vwCompactSearchPurchase Where isnull(ActiveStatus, 1) = 1 and isnull(StockActiveStatus, 1) = 1 ";
                                if (clsVchType.ProductClassList != "")
                                    sQuery = sQuery + " AND ProductTypeID IN (" + clsVchType.ProductClassList + ")";
                                if (clsVchType.ItemCategoriesList != "")
                                    sQuery = sQuery + " AND CategoryID IN (" + clsVchType.ItemCategoriesList + ")";
                                //new frmDetailedSearch2(GetFromItemSearch, sQuery, "Anywhere|ItemCode|ItemName|BatchUnique", dgvStockIn.Location.X + ((dgvStockIn.Width / 2) - 500), dgvStockIn.Location.Y, 7, 0, dgvStockIn.CurrentCell.Value.ToString(), 6, 0, "ORDER BY ItemCode ASC, Batchunique", 0, 0, "Item Search...", 0, "150,250,150,150,150,0,0", true, "frmItemMaster", 20, false, this.MdiParent, 6);

                                if (mblnInitialisedSubWindow == false)
                                {
                                    mblnInitialisedSubWindow = true;
                                    frmDetailedSearch2 frmN = new frmDetailedSearch2(GetFromItemSearch, sQuery, "Anywhere|ItemCode|ItemName|BatchUnique", dgvStockOut.Location.X + ((dgvStockOut.Width / 2) - 500), dgvStockOut.Location.Y, 7, 0, dgvStockIn.CurrentCell.Value.ToString(), 6, 0, "ORDER BY ItemCode ASC, Batchunique", 0, 0, "Item Search...", 0, "150,250,150,150,150,0,0", true, "frmItemMaster", 20, false, this.MdiParent, 6);
                                    frmN.MdiParent = this.MdiParent;
                                    frmN.Show(); //12-SEP-2022
                                }
                            }


                            if (dgvStockIn.Rows[dgvStockIn.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cItemID)].Value != null)
                            {
                                this.dgvStockIn.EditingControlShowing -= this.dgvStockIn_EditingControlShowing;

                                if (dgvStockIn.Rows[dgvStockIn.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cBarCode)].Visible == true)
                                    dgvStockIn.CurrentCell = dgvStockIn.Rows[dgvStockIn.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cBarCode)];
                                else
                                    dgvStockIn.CurrentCell = dgvStockIn.Rows[dgvStockIn.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cQty)];

                                dgvStockIn.Focus();
                                this.dgvStockIn.EditingControlShowing += this.dgvStockIn_EditingControlShowing;
                            }
                        }
                    }
                    else if (dgvStockIn.CurrentCell.ColumnIndex == GetEnum(GridColIndexes.cBarCode))
                    {
                        Form fc = Application.OpenForms["frmDetailedSearch2"];
                        if (fc != null)
                        {
                            fcc.Focus();
                            fcc.BringToFront();
                            return;
                        }
                        // BatchCode List Will Work only to MNF and Auto BatchMode Cases... Asper Discuss with Anup sir and Team on 13-May-2022 Evening Meeting.
                        if (Comm.ToInt32(dgvStockIn.Rows[dgvStockIn.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cBatchMode)].Value) == 1) // MNF
                            CallBatchCodeCompact(true);
                        else if (Comm.ToInt32(dgvStockIn.Rows[dgvStockIn.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cBatchMode)].Value) == 2) // Auto
                            CallBatchCodeCompact(true);
                    }
                }
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
        }

        private void dgvStockIn_SelectionChanged(object sender, EventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
        }

        private void dgvStockIn_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (dgvStockIn.CurrentCell.ColumnIndex == GetEnum(GridColIndexes.CExpiry))
                {
                    if (dgvStockIn.Rows[dgvStockIn.CurrentRow.Index].Cells[GetEnum(GridColIndexes.CExpiry)].ReadOnly == false)
                    {
                        _Rectangle = dgvStockIn.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true); //  
                        dtp.Size = new Size(_Rectangle.Width, _Rectangle.Height); //  
                        dtp.Location = new Point(_Rectangle.X, _Rectangle.Y); //  
                        dtp.Visible = true;
                        dtp.TextChanged += new EventHandler(dtp_TextChange);
                    }
                }
                if (dgvStockIn.CurrentCell.ColumnIndex == GetEnum(GridColIndexes.cSlNo))
                {
                    dgvStockIn.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                    dgvStockIn.Rows[dgvStockIn.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cSlNo)].ReadOnly = true;

                    strSelectedItemName = Convert.ToString(dgvStockIn.Rows[dgvStockIn.CurrentRow.Index].Cells[GetEnum(GridColIndexes.CItemCode)].Value);
                }
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void dtp_TextChange(object sender, EventArgs e)
        {
            dgvStockIn.CurrentRow.Cells[GetEnum(GridColIndexes.CExpiry)].Value = dtp.Text.ToString();
            dtp.Visible = false;
        }
        private void dtpOut_TextChange(object sender, EventArgs e)
        {
            dgvStockIn.CurrentRow.Cells[GetEnum(GridColIndexes.CExpiry)].Value = dtp.Text.ToString();
            dtp.Visible = false;
        }


        private void cboCostCentre_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                dgvStockOut.Focus();
                SendKeys.Send("{F4}");
            }
        }

        private void LoadTest()
        {
            iIDFromEditWindow = 0;
            if (iIDFromEditWindow == 0)
            {
                for (int i = 0; i < 100000; i++)
                {
                    txtInvAutoNo.Text = Comm.gfnGetNextSerialNo("tblRepacking", "AutoNum", " VchTypeID=" + vchtypeID).ToString();
                    if (txtInvAutoNo.Text == "") txtInvAutoNo.Text = "1";

                    CRUD_Operations(0, true);

                    lblHeading.Text = "Repacking " + i.ToString() + " / 100000 ";

                    Application.DoEvents();
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            //LoadTest();
            //return;


            try
            {
                Cursor.Current = Cursors.WaitCursor;
                if (iIDFromEditWindow == 0)
                    CRUD_Operations(0, false);
                else
                    CRUD_Operations(1);

                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private void dgvStockIn_ColumnWidthChanged(object sender, DataGridViewColumnEventArgs e)
        {
            try
            {
                if (this.ActiveControl == null) return;
                if (this.ActiveControl.Name != dgvStockIn.Name) return;
            }
            catch
            { }

            try
            {
                dtp.Visible = false;
                if (toggleWidthSettings.ToggleState == Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active)
                {
                    GridInitialize_dgvColWidth(false, dgvStockIn);
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

        private void dgvStockIn_Scroll(object sender, ScrollEventArgs e)
        {
            dtp.Visible = false;
        }

        private void dgvStockIn_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                string sQuery = "";
                Cursor.Current = Cursors.WaitCursor;
                double dSelectedItemID = 0;
                if (dgvStockIn.Rows[dgvStockIn.CurrentRow.Index].Cells[GetEnum(GridColIndexes.CItemCode)].Value != null)
                {
                    dSelectedItemID = Comm.ToDouble(dgvStockIn.Rows[dgvStockIn.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cItemID)].Value);
                    if (dSelectedItemID > 0)
                    {
                        if (dgvStockIn.CurrentCell.ColumnIndex == (int)GridColIndexes.CItemCode)
                        {
                            frmItemMaster frmIM = new frmItemMaster(Comm.ToInt32(dSelectedItemID), true, "E");
                            frmIM.ShowDialog();
                        }
                        else if (dgvStockIn.CurrentCell.ColumnIndex == (int)GridColIndexes.CItemName)
                        {
                            frmItemMaster frmIM = new frmItemMaster(Comm.ToInt32(dSelectedItemID), true, "E");
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

        private void dgvStockIn_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvStockIn.CurrentCell.ColumnIndex == GetEnum(GridColIndexes.cImgDel))
            {
                string SSelectedItemCode = Convert.ToString(dgvStockIn.Rows[dgvStockIn.CurrentRow.Index].Cells[GetEnum(GridColIndexes.CItemCode)].Value);
                if (SSelectedItemCode != "")
                {
                    DialogResult dlgResult = MessageBox.Show("Are you sure to delete Item Code[" + SSelectedItemCode + "] Permanently ?", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                    if (dlgResult == DialogResult.Yes)
                    {

                        Int32 selectedRowCount = dgvStockIn.Rows.GetRowCount(DataGridViewElementStates.Selected);
                        RowDelete();

                        dgvStockIn.Rows.Add();
                        dgvStockIn.CurrentCell = dgvStockIn.Rows[0].Cells[GetEnum(GridColIndexes.CItemCode)];

                        CalcTotal();
                    }
                }
            }
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

        private void dgvStockIn_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {

        }

        private void btnprev_Click(object sender, EventArgs e)
        {
            if (txtInvAutoNo.Tag.ToString() == "0")
            {
                if (dgvStockIn.Rows.Count > 0)
                {
                    if (dgvStockIn.Rows[0].Cells[GetEnum(GridColIndexes.CItemCode)].Value != null)
                    {
                        DialogResult dlgResult = MessageBox.Show("An Unsaved Voucher is Pending. Invoice Navigation will clear the unsaved Voucher. Do you want to proceed any way ?", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                        if (dlgResult == DialogResult.Yes)
                        {
                            PreVNext(true);
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
        }

        private void dtpInvDate_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
            {
                if (txtInvAutoNo.Visible == true)
                    txtInvAutoNo.Focus();
                else
                    txtPrefix.Focus();

            }
            else if (e.KeyCode == Keys.Enter)
            {
                if (clsVchTypeFeatures.blnenableEffeciveDate == true)
                {
                    if (txtReferencePrefix.Visible == true)
                        txtReferencePrefix.Focus();
                    else
                        txtReferenceAutoNo.Focus();
                    txtReferenceAutoNo.SelectAll();
                }
                else
                {
                    if (txtReferencePrefix.Visible == true)
                        txtReferencePrefix.Focus();
                }
            }
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            frmVouchertype frmV = new frmVouchertype(vchtypeID, false, true);
            frmV.StartPosition = FormStartPosition.CenterScreen;
            frmV.ShowDialog();
        }

        private void dgvStockIn_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void txtInvAutoNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
            {
                if (txtPrefix.Enabled == true)
                    txtPrefix.Focus();
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
                if (dgvStockIn.Rows.Count > 0)
                {
                    if (dgvStockIn.Rows[0].Cells[GetEnum(GridColIndexes.CItemCode)].Value != null)
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
            DeleteVoucher();
        }

        public bool DeleteVoucher()
        {
            bool ReturnResult = false;
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                DialogResult dlgResult = MessageBox.Show("Are you sure to delete Invoice No [" + txtInvAutoNo.Text + "] Permanently ?", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                if (dlgResult == DialogResult.Yes)
                {
                    CRUD_Operations(2);
        
                    ReturnResult = true;
                }
                Cursor.Current = Cursors.Default;
            
                return ReturnResult;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to Delete" + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private void frmRepacking_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                if (e.KeyCode == Keys.F3)
                {

                }
                else if (e.KeyCode == Keys.F9)
                {
                    if (this.ActiveControl != null)
                    {
                        if (this.ActiveControl.Name == dgvStockIn.Name)
                        {
                            for (int i = 0; i <= dgvStockIn.Rows.Count - 1; i++)
                            {
                                if (dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.CItemName)].Tag == null) dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.CItemName)].Tag = "0";
                                if (Comm.ToDecimal(dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.CItemName)].Tag.ToString()) == 0)
                                {
                                    dgvStockIn.CurrentCell = dgvStockIn[1, i];
                                    dgvStockIn.Focus();
                                }
                            }
                        }
                        else
                        {
                            for (int i = 0; i <= dgvStockOut.Rows.Count - 1; i++)
                            {
                                if (dgvStockOut.Rows[i].Cells[GetEnum(GridColIndexes.CItemName)].Tag == null) dgvStockOut.Rows[i].Cells[GetEnum(GridColIndexes.CItemName)].Tag = "0";
                                if (Comm.ToDecimal(dgvStockOut.Rows[i].Cells[GetEnum(GridColIndexes.CItemName)].Tag.ToString()) == 0)
                                {
                                    dgvStockOut.CurrentCell = dgvStockOut[1, i];
                                    dgvStockOut.Focus();
                                }
                            }
                        }
                    }
                }
                else if (e.KeyCode == Keys.F5)
                {
                    Cursor.Current = Cursors.WaitCursor;
                    if (iIDFromEditWindow == 0)
                        CRUD_Operations(0);
                    else
                        CRUD_Operations(1);

                    Cursor.Current = Cursors.Default;

                }
                else if (e.KeyCode == Keys.F7)
                {
                    try
                    {
                        Cursor.Current = Cursors.WaitCursor;
                        DialogResult dlgResult = MessageBox.Show("Are you sure to delete the bill Permanently ?", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                        if (dlgResult == DialogResult.Yes)
                        {
                            CRUD_Operations(2);
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
                else if (e.KeyCode == Keys.Escape)
                {
                    if (iIDFromEditWindow == 0)
                    {
                        if (dgvStockIn.Rows.Count > 0)
                        {
                            if (dgvStockIn.Rows[0].Cells[GetEnum(GridColIndexes.CItemCode)].Value != null)
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

        private void dgvStockIn_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
        }

        private void tableLayoutPanel2_MouseUp(object sender, MouseEventArgs e)
        {
            dragging = false;
        }

        private void txtReferenceAutoNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
            {
                if (txtReferencePrefix.Visible == true)
                {
                    txtReferencePrefix.Focus();
                    txtReferencePrefix.SelectAll();
                }
                else
                    dtpInvDate.Focus();

            }
            else if (e.KeyCode == Keys.Enter)
            {
                        if (cboCostCentre.Enabled == true)
                        {
                            cboCostCentre.Focus();
                            SendKeys.Send("{F4}");
                        }
                        else
                        {
                            dgvStockOut.Focus();
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
            if (Comm.ToInt32(txtPrefix.Tag.ToString()) == 3)
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
                    DataTable dtInv = Comm.fnGetData("SELECT InvID, ISNULL(JsonData,'') as JsonData,Invid FROM tblRepacking WHERE InvNo = '" + txtInvAutoNo.Text.Replace("'","''") + "' AND VchTypeID=" + vchtypeID + " AND TenantID = " + Global.gblTenantID + "").Tables[0];
                    if (dtInv.Rows.Count > 0)
                    {
                        DialogResult dlgResult = MessageBox.Show("There is an Exisiting Bill Number in this Invoice No [" + txtInvAutoNo.Text + "], Do you want to load it?", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                        if (dlgResult == DialogResult.Yes)
                        {
                            //LoadData(Comm.ToInt32(dtInv.Rows[0]["InvId"].ToString()));
                            iIDFromEditWindow = Comm.ToInt32(dtInv.Rows[0]["InvId"].ToString());
                            LoadBill(iIDFromEditWindow);
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
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void txtReferenceAutoNo_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtReferenceAutoNo, true);
        }

        private void txtReferenceAutoNo_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtReferenceAutoNo);
        }

        private void txtTaxRegn_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsLetter(e.KeyChar) && !char.IsDigit(e.KeyChar);
        }

        private void btnArchive_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                DialogResult dlgResult = MessageBox.Show("Are you sure to Archive the Bill ? Invoice No [" + txtInvAutoNo.Text + "].", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                if (dlgResult == DialogResult.Yes)
                {
                    CRUD_Operations(3);
                }
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

        private void cboState_SelectedValueChanged(object sender, EventArgs e)
        {

        }

        private void dgvStockIn_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {

        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            // Status : 0 - Saved But No longer Requireed
            // Status : 1 - Hold / Pause
            // Status : 2 : Auto Save
            if (Comm.fnGetData("SELECT * FROM tblTransactionPause WHERE UpdateStatus = 1").Tables[0].Rows.Count > 0)
            {
                DialogResult dlgResult = MessageBox.Show("Do You Want to Load Paused Transactions?", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                if (dlgResult.Equals(DialogResult.Yes))
                {
                    string sQuery = "SELECT TransNo as [Invoice No],LastUpdateDt as [Updated Date],ID,TransID,UpdateStatus,JsonData FROM tblTransactionPause WHERE UpdateStatus = 1 AND VchTypeID = " + vchtypeID + " AND VchParentID = 2 AND TenantID = " + Global.gblTenantID + "";
                    new frmCompactSearch(GetFromPauseSearch, sQuery, "Anywhere|Invoice No|ItemName|Updated Date", txtInvAutoNo.Location.X + 50, txtInvAutoNo.Location.Y + 108, 2, 0, "", 2, 0, "ORDER BY LastUpdateDt DESC", 0, 0, "Paus/Hold List...", 0, "100,150,0,0,0,0", true, "").ShowDialog();
                }
                else
                {
                    string strJson = SerializetoJson();
                    string sData = "";
                    if (lblPause.Tag.ToString() == "")
                    {
                        sData = "INSERT INTO tblTransactionPause(VchTypeID,VchParentID,TransID,TransNo,LastUpdateDt,UpdateStatus,JsonData,TenantID)  " +
                                "VALUES(" + vchtypeID + ",2,0,'" + txtInvAutoNo.Text + "','" + DateTime.Today + "',1,'" + strJson + "'," + Global.gblTenantID + ")";
                    }
                    else
                    {
                        sData = "UPDATE tblTransactionPause SET LastUpdateDt='" + DateTime.Today + "',UpdateStatus=1,'" + strJson + "' WHERE ID=" + Comm.ToInt32(lblPause.Tag) + " AND TenantID = " + Global.gblTenantID + " AND VchTypeID = " + vchtypeID + " AND VchParentID = 2";
                    }
                    Comm.fnExecuteNonQuery(sData);
                }
            }
            else
            {
                string strJson = SerializetoJson();
                string sData = "INSERT INTO tblTransactionPause(VchTypeID,VchParentID,TransID,TransNo,LastUpdateDt,UpdateStatus,JsonData,TenantID)  " +
                "VALUES(" + vchtypeID + ",2,0,'" + txtInvAutoNo.Text + "','" + DateTime.Today + "',1,'" + strJson + "'," + Global.gblTenantID + ")";
                Comm.fnExecuteNonQuery(sData);
            }
        }


        private void dtpInvDate_ValueChanged(object sender, EventArgs e)
        {
            
        }

        private void btnMenu_Click(object sender, EventArgs e)
        {
            if (this.tlpMain.ColumnStyles[1].Width == 0)
                this.tlpMain.ColumnStyles[1].Width = 260;
            else
                this.tlpMain.ColumnStyles[1].Width = 0;
        }

        private void dgvStockIn_MouseUp(object sender, MouseEventArgs e)
        {

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
                        if (dgvStockIn.Columns[i].Name == dgvColWidth.Rows[i].Cells[3].Value.ToString())
                        {
                            dgvStockIn.Columns[i].Width = Comm.ToInt32(dgvColWidth.Rows[i].Cells[2].Value.ToString());
                            if (dgvColWidth.Rows[i].Cells[0].Value.ToString() == "")
                                dgvStockIn.Columns[i].Visible = false;
                            else
                                dgvStockIn.Columns[i].Visible = Convert.ToBoolean(dgvColWidth.Rows[i].Cells[0].Value);
                        }
                        if (dgvStockIn.Columns[i].Name == "cRateinclusive")
                            dgvStockIn.Columns[i].Visible = false;

                    }
                }
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void dgvStockIn_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {

        }

        private void lblQuickSettingsClose_Click(object sender, EventArgs e)
        {
            this.tlpMain.ColumnStyles[1].Width = 0;
        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void button13_Click(object sender, EventArgs e)
        {

        }

        #endregion

        #region "METHODS ----------------------------------------------- >>"

        // Description : Works when click on Previous/Next Invoice Buttons
        private void PreVNext(bool bIsPrev = true)
        {
            DataTable dtInv = new DataTable();
            decimal dInvId = 0;

            btnNext.Enabled = true;
            btnprev.Enabled = true;

            if (txtInvAutoNo.Tag.ToString() != "")
            {
                if (bIsPrev == true)
                {
                    if (txtInvAutoNo.Tag.ToString() == "0")
                    {
                        dtInv = Comm.fnGetData("SELECT TOP 1 ISNULL(InvId,0) FROM tblRepacking WHERE VchTypeID = " + vchtypeID + " AND TenantID = " + Global.gblTenantID + " ORDER BY InvId DESC").Tables[0];
                        if (dtInv.Rows.Count > 0)
                            dInvId = Comm.ToDecimal(dtInv.Rows[0][0].ToString());
                        else
                            dInvId = 0;
                    }
                    else
                    {
                        dtInv = Comm.fnGetData("SELECT TOP 1 ISNULL(InvId,0) FROM tblRepacking WHERE InvId < " + Comm.ToDecimal(txtInvAutoNo.Tag.ToString()) + " AND VchTypeID = " + vchtypeID + " AND TenantID = " + Global.gblTenantID + " ORDER BY InvId DESC").Tables[0];
                        if (dtInv.Rows.Count > 0)
                            dInvId = Comm.ToDecimal(dtInv.Rows[0][0].ToString());
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
                        iIDFromEditWindow = Comm.ToInt32(dInvId);
                        LoadData(Comm.ToInt32(dInvId));
                        btnprev.Enabled = true;

                        GridInitialize_dgvColWidth(true, dgvStockIn);
                        GridInitialize_dgvColWidth(true, dgvStockOut);
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
                }
                else //Next
                {
                    if (txtInvAutoNo.Tag.ToString() != "0")
                    {
                        dtInv = Comm.fnGetData("SELECT TOP 1 ISNULL(InvId,0) FROM tblRepacking WHERE InvId > " + Comm.ToDecimal(txtInvAutoNo.Tag.ToString()) + " AND VchTypeID = " + vchtypeID + " AND TenantID = " + Global.gblTenantID + " ORDER BY InvId ASC").Tables[0];
                        if (dtInv.Rows.Count > 0)
                            dInvId = Comm.ToDecimal(dtInv.Rows[0][0].ToString());
                        else
                        {
                            dInvId = 0;
                            ClearControls();

                            GridInitialize_dgvColWidth(true, dgvStockIn);
                            GridInitialize_dgvColWidth(true, dgvStockOut);
                            try
                            {
                                LoadGridWidthFromItemGrid();
                                DisableGridSettingsCheckbox();

                                GridSettingsEnableDisable(Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active);
                            }
                            catch (Exception ex)
                            {

                            }

                            if (Comm.ConvertI32(clsVchType.TransactionNumberingValue) == 0) // Auto Locked
                            {
                                //txtInvAutoNo.Text = Comm.gfnGetNextSerialNo("tblRepacking", "AutoNum").ToString();
                                txtInvAutoNo.Text = Comm.gfnGetNextSerialNo("tblRepacking", "AutoNum", " VchTypeID=" + vchtypeID).ToString();
                                txtInvAutoNo.Tag = 0;

                                if (txtInvAutoNo.Text == "") txtInvAutoNo.Text = "1";

                                txtInvAutoNo.ReadOnly = true;
                                txtPrefix.ReadOnly = true;
                            }
                            else if (Comm.ConvertI32(clsVchType.TransactionNumberingValue) == 1) // Auto Editable
                            {
                                //txtInvAutoNo.Text = Comm.gfnGetNextSerialNo("tblRepacking", "AutoNum").ToString();
                                txtInvAutoNo.Text = Comm.gfnGetNextSerialNo("tblRepacking", "AutoNum", " VchTypeID=" + vchtypeID).ToString();
                                txtInvAutoNo.Tag = 0;

                                if (txtInvAutoNo.Text == "") txtInvAutoNo.Text = "1";

                                txtInvAutoNo.ReadOnly = false;
                                txtPrefix.ReadOnly = false;
                            }
                            else
                            {
                                txtInvAutoNo.Tag = 0;
                                txtInvAutoNo.ReadOnly = false;
                                txtPrefix.ReadOnly = false;
                            }

                            if (Comm.ConvertI32(clsVchType.ReferenceNumberingValue) == 0) // Auto Locked
                            {
                                if (iIDFromEditWindow == 0)
                                    txtReferenceAutoNo.Text = Comm.gfnGetNextSerialNo("tblRepacking", "ReferenceAutoNO").ToString();
                                txtReferenceAutoNo.ReadOnly = true;
                                txtReferencePrefix.ReadOnly = true;
                                txtReferencePrefix.Width = 55;
                            }
                            else if (Comm.ConvertI32(clsVchType.ReferenceNumberingValue) == 1) // Auto Editable
                            {
                                if (iIDFromEditWindow == 0)
                                    txtReferenceAutoNo.Text = Comm.gfnGetNextSerialNo("tblRepacking", "ReferenceAutoNO").ToString();
                                txtReferenceAutoNo.ReadOnly = false;
                                txtReferencePrefix.ReadOnly = false;
                                txtReferencePrefix.Width = 55;
                            }
                            else if (Comm.ConvertI32(clsVchType.ReferenceNumberingValue) == 2) // Custom
                            {
                                txtReferencePrefix.Visible = true;
                                txtReferenceAutoNo.ReadOnly = true;
                                txtReferencePrefix.ReadOnly = false;
                                txtReferencePrefix.Width = txtReferenceAutoNo.Width;
                            }
                        }

                        if (dInvId == 0)
                        {
                            iIDFromEditWindow = 0;
                            btnNext.Enabled = false;
                        }
                        else
                        {
                            btnNext.Enabled = true;

                            iIDFromEditWindow = Comm.ToInt32(dInvId);
                            LoadData(Comm.ToInt32(dInvId));

                            GridInitialize_dgvColWidth(true, dgvStockIn);
                            GridInitialize_dgvColWidth(true, dgvStockOut);
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
                    }
                    else
                    {
                        iIDFromEditWindow = 0;
                        btnNext.Enabled = false;
                    }
                }
            }
        }

        // Description : Fill Supplier For Serialize Json UsingID
        private bool FillSupplierForSerializeJsonUsingID(int iLedgerID)
        {
            DataTable dtSupp = new DataTable();

            GetLedinfo.LID = iLedgerID;
            GetLedinfo.TenantID = Global.gblTenantID;
            GetLedinfo.GroupName = "SUPPLIER";
            dtSupp = clsLedg.GetLedger(GetLedinfo);
            if (dtSupp.Rows.Count > 0)
            {
                sArrLedger[GetEnumLedger(LedgerIndexes.LName)] = "";
                sArrLedger[GetEnumLedger(LedgerIndexes.LAliasName)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["LedgerCode"].ToString());
                sArrLedger[GetEnumLedger(LedgerIndexes.Address)] = "";
                sArrLedger[GetEnumLedger(LedgerIndexes.TaxNo)] = "";
                sArrLedger[GetEnumLedger(LedgerIndexes.MobileNo)] = "";

                sArrLedger[GetEnumLedger(LedgerIndexes.StateID)] = "32";
                sArrLedger[GetEnumLedger(LedgerIndexes.GSTType)] = "";

                sArrLedger[GetEnumLedger(LedgerIndexes.LID)] = dtSupp.Rows[0]["LID"].ToString();
                sArrLedger[GetEnumLedger(LedgerIndexes.GroupName)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["GroupName"].ToString());
                sArrLedger[GetEnumLedger(LedgerIndexes.Type)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["Type"].ToString());
                sArrLedger[GetEnumLedger(LedgerIndexes.OpBalance)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["OpBalance"].ToString());
                sArrLedger[GetEnumLedger(LedgerIndexes.AppearIn)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["AppearIn"].ToString());
                sArrLedger[GetEnumLedger(LedgerIndexes.CreditDays)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["CreditDays"].ToString());
                sArrLedger[GetEnumLedger(LedgerIndexes.Phone)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["Phone"].ToString());
                sArrLedger[GetEnumLedger(LedgerIndexes.AccountGroupID)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["AccountGroupID"].ToString());
                sArrLedger[GetEnumLedger(LedgerIndexes.RouteID)] = "0";
                sArrLedger[GetEnumLedger(LedgerIndexes.Area)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["Area"].ToString());
                sArrLedger[GetEnumLedger(LedgerIndexes.Notes)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["Notes"].ToString());
                sArrLedger[GetEnumLedger(LedgerIndexes.TargetAmt)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["TargetAmt"].ToString());
                sArrLedger[GetEnumLedger(LedgerIndexes.SMSSchID)] = "0";
                sArrLedger[GetEnumLedger(LedgerIndexes.Email)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["Email"].ToString());
                sArrLedger[GetEnumLedger(LedgerIndexes.DiscPer)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["DiscPer"].ToString());
                sArrLedger[GetEnumLedger(LedgerIndexes.InterestPer)] = "0";
                sArrLedger[GetEnumLedger(LedgerIndexes.DummyLName)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["DummyLName"].ToString());
                sArrLedger[GetEnumLedger(LedgerIndexes.BlnBank)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["BlnBank"].ToString());
                sArrLedger[GetEnumLedger(LedgerIndexes.CurrencyID)] = "0";
                sArrLedger[GetEnumLedger(LedgerIndexes.AreaID)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["AreaID"].ToString());
                sArrLedger[GetEnumLedger(LedgerIndexes.PLID)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["PLID"].ToString());
                sArrLedger[GetEnumLedger(LedgerIndexes.ActiveStatus)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["ActiveStatus"].ToString());
                sArrLedger[GetEnumLedger(LedgerIndexes.EmailAddress)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["EmailAddress"].ToString());
                if (dtSupp.Rows[0]["EntryDate"].ToString() == "")
                    sArrLedger[GetEnumLedger(LedgerIndexes.EntryDate)] = DateTime.Today.ToString();
                else
                    sArrLedger[GetEnumLedger(LedgerIndexes.EntryDate)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["EntryDate"].ToString());
                sArrLedger[GetEnumLedger(LedgerIndexes.blnBillWise)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["blnBillWise"].ToString());
                sArrLedger[GetEnumLedger(LedgerIndexes.CustomerCardID)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["CustomerCardID"].ToString());
                sArrLedger[GetEnumLedger(LedgerIndexes.TDSPer)] = "0";
                sArrLedger[GetEnumLedger(LedgerIndexes.DOB)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["DOB"].ToString());
                sArrLedger[GetEnumLedger(LedgerIndexes.CCIDS)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["CCIDS"].ToString());
                sArrLedger[GetEnumLedger(LedgerIndexes.CurrentBalance)] = "0";
                sArrLedger[GetEnumLedger(LedgerIndexes.LedgerName)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["LedgerName"].ToString());
                sArrLedger[GetEnumLedger(LedgerIndexes.LedgerCode)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["LedgerCode"].ToString());
                sArrLedger[GetEnumLedger(LedgerIndexes.BlnWallet)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["BlnWallet"].ToString());
                sArrLedger[GetEnumLedger(LedgerIndexes.blnCoupon)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["blnCoupon"].ToString());
                sArrLedger[GetEnumLedger(LedgerIndexes.TransComn)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["TransComn"].ToString());
                sArrLedger[GetEnumLedger(LedgerIndexes.BlnSmsWelcome)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["BlnSmsWelcome"].ToString());
                sArrLedger[GetEnumLedger(LedgerIndexes.DLNO)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["DLNO"].ToString());
                sArrLedger[GetEnumLedger(LedgerIndexes.TDS)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["TDS"].ToString());
                sArrLedger[GetEnumLedger(LedgerIndexes.LedgerNameUnicode)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["LedgerNameUnicode"].ToString());
                sArrLedger[GetEnumLedger(LedgerIndexes.LedgerAliasNameUnicode)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["LedgerAliasNameUnicode"].ToString());
                sArrLedger[GetEnumLedger(LedgerIndexes.ContactPerson)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["ContactPerson"].ToString());
                sArrLedger[GetEnumLedger(LedgerIndexes.TaxParameter)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["TaxParameter"].ToString());
                sArrLedger[GetEnumLedger(LedgerIndexes.TaxParameterType)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["TaxParameterType"].ToString());
                sArrLedger[GetEnumLedger(LedgerIndexes.HSNCODE)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["HSNCODE"].ToString());
                sArrLedger[GetEnumLedger(LedgerIndexes.CGSTTaxPer)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["CGSTTaxPer"].ToString());
                sArrLedger[GetEnumLedger(LedgerIndexes.SGSTTaxPer)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["SGSTTaxPer"].ToString());
                sArrLedger[GetEnumLedger(LedgerIndexes.IGSTTaxPer)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["IGSTTaxPer"].ToString());
                sArrLedger[GetEnumLedger(LedgerIndexes.HSNID)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["HSNID"].ToString());
                sArrLedger[GetEnumLedger(LedgerIndexes.BankAccountNo)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["BankAccountNo"].ToString());
                sArrLedger[GetEnumLedger(LedgerIndexes.BankIFSCCode)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["BankIFSCCode"].ToString());
                sArrLedger[GetEnumLedger(LedgerIndexes.BankNote)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["BankNote"].ToString());
                sArrLedger[GetEnumLedger(LedgerIndexes.WhatsAppNo)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["WhatsAppNo"].ToString());
                sArrLedger[GetEnumLedger(LedgerIndexes.SystemName)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["SystemName"].ToString());
                sArrLedger[GetEnumLedger(LedgerIndexes.UserID)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["UserID"].ToString());
                sArrLedger[GetEnumLedger(LedgerIndexes.LastUpdateDate)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["LastUpdateDate"].ToString());
                sArrLedger[GetEnumLedger(LedgerIndexes.LastUpdateTime)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["LastUpdateTime"].ToString());
                sArrLedger[GetEnumLedger(LedgerIndexes.TenantID)] = Global.gblTenantID.ToString();
                sArrLedger[GetEnumLedger(LedgerIndexes.AgentID)] = "0";

                return true;
            }
            else
                return false;
        }

        // Description : Fill Supplier UsingID
        private bool FillSupplierUsingID(int iLedgerID)
        {
            DataTable dtSupp = new DataTable();

            GetLedinfo.LID = iLedgerID;
            GetLedinfo.TenantID = Global.gblTenantID;
            GetLedinfo.GroupName = "SUPPLIER";
            dtSupp = clsLedg.GetLedger(GetLedinfo);
            if (dtSupp.Rows.Count > 0)
            {
                dSupplierID = Comm.ToDecimal(dtSupp.Rows[0]["LID"].ToString());

                sArrLedger[GetEnumLedger(LedgerIndexes.LName)] = dtSupp.Rows[0]["LedgerName"].ToString();
                sArrLedger[GetEnumLedger(LedgerIndexes.LAliasName)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["LedgerCode"].ToString());
                sArrLedger[GetEnumLedger(LedgerIndexes.Address)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["Address"].ToString());
                sArrLedger[GetEnumLedger(LedgerIndexes.TaxNo)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["TaxNo"].ToString());
                sArrLedger[GetEnumLedger(LedgerIndexes.MobileNo)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["MobileNo"].ToString());
                sArrLedger[GetEnumLedger(LedgerIndexes.StateID)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["StateID"].ToString());
                sArrLedger[GetEnumLedger(LedgerIndexes.GSTType)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["GSTType"].ToString());

                sArrLedger[GetEnumLedger(LedgerIndexes.LID)] = dtSupp.Rows[0]["LID"].ToString();
                sArrLedger[GetEnumLedger(LedgerIndexes.GroupName)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["GroupName"].ToString());
                sArrLedger[GetEnumLedger(LedgerIndexes.Type)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["Type"].ToString());
                sArrLedger[GetEnumLedger(LedgerIndexes.OpBalance)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["OpBalance"].ToString());
                sArrLedger[GetEnumLedger(LedgerIndexes.AppearIn)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["AppearIn"].ToString());
                sArrLedger[GetEnumLedger(LedgerIndexes.CreditDays)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["CreditDays"].ToString());
                sArrLedger[GetEnumLedger(LedgerIndexes.Phone)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["Phone"].ToString());
                sArrLedger[GetEnumLedger(LedgerIndexes.AccountGroupID)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["AccountGroupID"].ToString());
                sArrLedger[GetEnumLedger(LedgerIndexes.RouteID)] = "0";
                sArrLedger[GetEnumLedger(LedgerIndexes.Area)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["Area"].ToString());
                sArrLedger[GetEnumLedger(LedgerIndexes.Notes)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["Notes"].ToString());
                sArrLedger[GetEnumLedger(LedgerIndexes.TargetAmt)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["TargetAmt"].ToString());
                sArrLedger[GetEnumLedger(LedgerIndexes.SMSSchID)] = "0";
                sArrLedger[GetEnumLedger(LedgerIndexes.Email)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["Email"].ToString());
                sArrLedger[GetEnumLedger(LedgerIndexes.DiscPer)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["DiscPer"].ToString());
                sArrLedger[GetEnumLedger(LedgerIndexes.InterestPer)] = "0";
                sArrLedger[GetEnumLedger(LedgerIndexes.DummyLName)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["DummyLName"].ToString());
                sArrLedger[GetEnumLedger(LedgerIndexes.BlnBank)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["BlnBank"].ToString());
                sArrLedger[GetEnumLedger(LedgerIndexes.CurrencyID)] = "0";
                sArrLedger[GetEnumLedger(LedgerIndexes.AreaID)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["AreaID"].ToString());
                sArrLedger[GetEnumLedger(LedgerIndexes.PLID)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["PLID"].ToString());
                sArrLedger[GetEnumLedger(LedgerIndexes.ActiveStatus)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["ActiveStatus"].ToString());
                sArrLedger[GetEnumLedger(LedgerIndexes.EmailAddress)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["EmailAddress"].ToString());
                if (dtSupp.Rows[0]["EntryDate"].ToString() == "")
                    sArrLedger[GetEnumLedger(LedgerIndexes.EntryDate)] = DateTime.Today.ToString();
                else
                    sArrLedger[GetEnumLedger(LedgerIndexes.EntryDate)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["EntryDate"].ToString());
                sArrLedger[GetEnumLedger(LedgerIndexes.blnBillWise)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["blnBillWise"].ToString());
                sArrLedger[GetEnumLedger(LedgerIndexes.CustomerCardID)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["CustomerCardID"].ToString());
                sArrLedger[GetEnumLedger(LedgerIndexes.TDSPer)] = "0";
                sArrLedger[GetEnumLedger(LedgerIndexes.DOB)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["DOB"].ToString());
                sArrLedger[GetEnumLedger(LedgerIndexes.CCIDS)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["CCIDS"].ToString());
                sArrLedger[GetEnumLedger(LedgerIndexes.CurrentBalance)] = "0";
                sArrLedger[GetEnumLedger(LedgerIndexes.LedgerName)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["LedgerName"].ToString());
                sArrLedger[GetEnumLedger(LedgerIndexes.LedgerCode)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["LedgerCode"].ToString());
                sArrLedger[GetEnumLedger(LedgerIndexes.BlnWallet)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["BlnWallet"].ToString());
                sArrLedger[GetEnumLedger(LedgerIndexes.blnCoupon)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["blnCoupon"].ToString());
                sArrLedger[GetEnumLedger(LedgerIndexes.TransComn)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["TransComn"].ToString());
                sArrLedger[GetEnumLedger(LedgerIndexes.BlnSmsWelcome)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["BlnSmsWelcome"].ToString());
                sArrLedger[GetEnumLedger(LedgerIndexes.DLNO)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["DLNO"].ToString());
                sArrLedger[GetEnumLedger(LedgerIndexes.TDS)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["TDS"].ToString());
                sArrLedger[GetEnumLedger(LedgerIndexes.LedgerNameUnicode)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["LedgerNameUnicode"].ToString());
                sArrLedger[GetEnumLedger(LedgerIndexes.LedgerAliasNameUnicode)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["LedgerAliasNameUnicode"].ToString());
                sArrLedger[GetEnumLedger(LedgerIndexes.ContactPerson)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["ContactPerson"].ToString());
                sArrLedger[GetEnumLedger(LedgerIndexes.TaxParameter)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["TaxParameter"].ToString());
                sArrLedger[GetEnumLedger(LedgerIndexes.TaxParameterType)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["TaxParameterType"].ToString());
                sArrLedger[GetEnumLedger(LedgerIndexes.HSNCODE)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["HSNCODE"].ToString());
                sArrLedger[GetEnumLedger(LedgerIndexes.CGSTTaxPer)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["CGSTTaxPer"].ToString());
                sArrLedger[GetEnumLedger(LedgerIndexes.SGSTTaxPer)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["SGSTTaxPer"].ToString());
                sArrLedger[GetEnumLedger(LedgerIndexes.IGSTTaxPer)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["IGSTTaxPer"].ToString());
                sArrLedger[GetEnumLedger(LedgerIndexes.HSNID)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["HSNID"].ToString());
                sArrLedger[GetEnumLedger(LedgerIndexes.BankAccountNo)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["BankAccountNo"].ToString());
                sArrLedger[GetEnumLedger(LedgerIndexes.BankIFSCCode)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["BankIFSCCode"].ToString());
                sArrLedger[GetEnumLedger(LedgerIndexes.BankNote)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["BankNote"].ToString());
                sArrLedger[GetEnumLedger(LedgerIndexes.WhatsAppNo)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["WhatsAppNo"].ToString());
                sArrLedger[GetEnumLedger(LedgerIndexes.SystemName)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["SystemName"].ToString());
                sArrLedger[GetEnumLedger(LedgerIndexes.UserID)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["UserID"].ToString());
                sArrLedger[GetEnumLedger(LedgerIndexes.LastUpdateDate)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["LastUpdateDate"].ToString());
                sArrLedger[GetEnumLedger(LedgerIndexes.LastUpdateTime)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["LastUpdateTime"].ToString());
                sArrLedger[GetEnumLedger(LedgerIndexes.TenantID)] = Global.gblTenantID.ToString();
                sArrLedger[GetEnumLedger(LedgerIndexes.AgentID)] = 32.ToString();

                return true;
            }
            else
                return false;
        }

        //Description : Get Employee Details from Database
        public DataTable GetEmployee(int iSelID = 0)
        {
            GetEmpInfo.EmpID = iSelID;
            GetEmpInfo.TenantID = Global.gblTenantID;
            GetEmpInfo.blnSalesStaff = true;
            return clsEmp.GetEmployee(GetEmpInfo);
        }

        //Description: Get Cost Centre Details from the Database
        public DataTable GetCostCentre(int iSelID = 0)
        {
            GetCctinfo.CCID = iSelID;
            GetCctinfo.TenantID = Global.gblTenantID;
            return clscct.GetCostCentre(GetCctinfo);
        }

        //Description: Fill CostCentre from Get CostCentre Method
        private void FillCostCentre(int iSelID = 0)
        {
            DataTable dtCct = new DataTable();
            dtCct = GetCostCentre(0);
            if (dtCct.Rows.Count > 0)
            {
                cboCostCentre.DataSource = dtCct;
                cboCostCentre.DisplayMember = "Cost Centre Name";
                cboCostCentre.ValueMember = "CCID";
                if (iSelID != 0)
                    cboCostCentre.SelectedValue = iSelID;
            }
        }

        //Description: Fill Grid According to the BatchUnique as Paramter
        private void FillGridAsperBatchCode(string sBarUnique = "")
        {
            DateTime dtCurrExp = DateTime.Today;
            dtCurrExp = dtCurrExp.AddYears(8);
            decimal dQty = 0;
            if (sBarUnique == "<Auto Barcode>")
            {
                SetValue(GetEnum(GridColIndexes.CExpiry), dtCurrExp.ToString("dd-MMM-yyyy").ToString(), "DATE");
            }
            else
            {
                DataTable dtData = new DataTable();
                GetStockInfo.BatchUnique = sBarUnique;
                GetStockInfo.BatchCode = "";
                GetStockInfo.StockID = 0;
                GetStockInfo.TenantID = Global.gblTenantID;
                GetStockInfo.ItemID = Comm.ToDouble(dgvStockIn.Rows[dgvStockIn.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cItemID)].Value);
                GetStockInfo.CCID = Comm.ToDouble(cboCostCentre.SelectedValue);
                dtData = clsStock.GetStockDetails(GetStockInfo);
                if (dtData.Rows.Count > 0)
                {
                    SetValue(GetEnum(GridColIndexes.CExpiry), dtData.Rows[0]["ExpiryDate"].ToString(), "DATE");
                    SetValue(GetEnum(GridColIndexes.cMRP), dtData.Rows[0]["MRP"].ToString(), "CURR_FLOAT");
                    SetValue(GetEnum(GridColIndexes.cPrate), dtData.Rows[0]["PRateExcl"].ToString(), "CURR_FLOAT");
                    SetValue(GetEnum(GridColIndexes.cGrossAmt), dtData.Rows[0]["PRateExcl"].ToString(), "CURR_FLOAT");
                    SetValue(GetEnum(GridColIndexes.cCrate), dtData.Rows[0]["CostRateExcl"].ToString(), "CURR_FLOAT");
                    SetValue(GetEnum(GridColIndexes.cCRateWithTax), dtData.Rows[0]["CostRateInc"].ToString(), "CURR_FLOAT");
                    SetValue(GetEnum(GridColIndexes.cItemID), dtData.Rows[0]["ItemID"].ToString(), "INT");
                    dQty = Comm.ToDecimal(dgvStockIn.Rows[dgvStockIn.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cQty)].Value);
                    CalcTotal();
                }
            }
        }
        //Description: Fill Grid According to the BatchUnique as Paramter
        private void FillGridAsperBatchCodeOut(string sBarUnique = "")
        {
            DateTime dtCurrExp = DateTime.Today;
            dtCurrExp = dtCurrExp.AddYears(8);
            decimal dQty = 0;
            if (sBarUnique == "<Auto Barcode>")
            {
                SetValueOut(GetEnum(GridColIndexes.CExpiry), dtCurrExp.ToString("dd-MMM-yyyy").ToString(), "DATE");
            }
            else
            {
                DataTable dtData = new DataTable();
                GetStockInfo.BatchUnique = sBarUnique;
                GetStockInfo.BatchCode = "";
                GetStockInfo.StockID = 0;
                GetStockInfo.TenantID = Global.gblTenantID;
                GetStockInfo.ItemID = Comm.ToDouble(dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cItemID)].Value);
                GetStockInfo.CCID = Comm.ToDouble(cboCostCentre.SelectedValue);
                dtData = clsStock.GetStockDetails(GetStockInfo);
                if (dtData.Rows.Count > 0)
                {
                    SetValueOut(GetEnum(GridColIndexes.CExpiry), dtData.Rows[0]["ExpiryDate"].ToString(), "DATE");
                    SetValueOut(GetEnum(GridColIndexes.cMRP), dtData.Rows[0]["MRP"].ToString(), "CURR_FLOAT");
                    SetValueOut(GetEnum(GridColIndexes.cPrate), dtData.Rows[0]["PRateExcl"].ToString(), "CURR_FLOAT");
                    SetValueOut(GetEnum(GridColIndexes.cCrate), dtData.Rows[0]["CostRateExcl"].ToString(), "CURR_FLOAT");
                    SetValueOut(GetEnum(GridColIndexes.cCRateWithTax), dtData.Rows[0]["CostRateInc"].ToString(), "CURR_FLOAT");
                    SetValueOut(GetEnum(GridColIndexes.cGrossAmt), dtData.Rows[0]["PRateExcl"].ToString(), "CURR_FLOAT");
                    SetValueOut(GetEnum(GridColIndexes.cItemID), dtData.Rows[0]["ItemID"].ToString(), "INT");
                    dQty = Comm.ToDecimal(dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cQty)].Value);

                    CalcTotalOut();

                }
            }
        }

        //Description: Fill Grid Data using StockID that giving as Parameter
        private void FillGridAsperStockID(int iStockID)
        {
            DataTable dtstock = new DataTable();
            DateTime dtCurrExp = DateTime.Today;
            dtCurrExp = dtCurrExp.AddYears(8);
            decimal dQty = 0;
            string sBarUnique = "";
            if (iStockID != 0)
            {
                DataTable dtData = new DataTable();
                dtstock = Comm.fnGetData("SELECT BatchUnique FROM tblStock WHERE StockID = " + iStockID + "").Tables[0];
                if (dtstock.Rows.Count > 0)
                    sBarUnique = dtstock.Rows[0][0].ToString().Trim();

                GetStockInfo.BatchUnique = sBarUnique;
                GetStockInfo.BatchCode = "";
                GetStockInfo.StockID = iStockID;
                GetStockInfo.TenantID = Global.gblTenantID;
                GetStockInfo.ItemID = Comm.ToDouble(dgvStockIn.Rows[dgvStockIn.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cItemID)].Value);
                GetStockInfo.CCID = Comm.ToDouble(cboCostCentre.SelectedValue);
                dtData = clsStock.GetStockDetails(GetStockInfo);
                if (dtData.Rows.Count > 0)
                {
                    SetValue(GetEnum(GridColIndexes.cBarCode), dtData.Rows[0]["BatchUnique"].ToString());
                    setTag(GetEnum(GridColIndexes.cBarCode), dtData.Rows[0]["BatchCode"].ToString());
                    SetValue(GetEnum(GridColIndexes.CExpiry), dtData.Rows[0]["ExpiryDate"].ToString(), "DATE");
                    SetValue(GetEnum(GridColIndexes.cMRP), dtData.Rows[0]["MRP"].ToString(), "CURR_FLOAT");
                    SetValue(GetEnum(GridColIndexes.cQty), 1.ToString());
                    SetValue(GetEnum(GridColIndexes.cPrate), dtData.Rows[0]["PRate"].ToString(), "CURR_FLOAT");
                    SetValue(GetEnum(GridColIndexes.cGrossAmt), dtData.Rows[0]["PrateInc"].ToString(), "CURR_FLOAT");
                    SetValue(GetEnum(GridColIndexes.cCrate), dtData.Rows[0]["CostRateExcl"].ToString(), "CURR_FLOAT");
                    SetValue(GetEnum(GridColIndexes.cCRateWithTax), dtData.Rows[0]["CostRateInc"].ToString(), "CURR_FLOAT");
                    SetValue(GetEnum(GridColIndexes.cItemID), dtData.Rows[0]["ItemID"].ToString(), "INT");
                    dQty = Comm.ToDecimal(dgvStockIn.Rows[dgvStockIn.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cQty)].Value);
                    CalcTotal();
                }
            }
            else if (iStockID == 0)
            {
                sBarUnique = "<Auto Barcode>";
                SetValue(GetEnum(GridColIndexes.cBarCode), sBarUnique);
            }
        }
        //Description: Fill Grid Data using StockID that giving as Parameter
        private void FillGridAsperStockIDOut(int iStockID)
        {
            DataTable dtstock = new DataTable();
            DateTime dtCurrExp = DateTime.Today;
            dtCurrExp = dtCurrExp.AddYears(8);
            decimal dQty = 0;
            string sBarUnique = "";
            if (iStockID != 0)
            {
                DataTable dtData = new DataTable();
                dtstock = Comm.fnGetData("SELECT BatchUnique FROM tblStock WHERE StockID = " + iStockID + "").Tables[0];
                if (dtstock.Rows.Count > 0)
                    sBarUnique = dtstock.Rows[0][0].ToString().Trim();

                GetStockInfo.BatchUnique = sBarUnique;
                GetStockInfo.BatchCode = "";
                GetStockInfo.StockID = iStockID;
                GetStockInfo.TenantID = Global.gblTenantID;
                GetStockInfo.ItemID = Comm.ToDouble(dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cItemID)].Value);
                GetStockInfo.CCID = Comm.ToDouble(cboCostCentre.SelectedValue);
                dtData = clsStock.GetStockDetails(GetStockInfo);
                if (dtData.Rows.Count > 0)
                {
                    SetValueOut(GetEnum(GridColIndexes.cBarCode), dtData.Rows[0]["BatchUnique"].ToString());
                    setTagOut(GetEnum(GridColIndexes.cBarCode), dtData.Rows[0]["BatchCode"].ToString());
                    SetValueOut(GetEnum(GridColIndexes.CExpiry), dtData.Rows[0]["ExpiryDate"].ToString(), "DATE");
                    SetValueOut(GetEnum(GridColIndexes.cMRP), dtData.Rows[0]["MRP"].ToString(), "CURR_FLOAT");
                    SetValueOut(GetEnum(GridColIndexes.cQty), 1.ToString());
                    SetValueOut(GetEnum(GridColIndexes.cPrate), dtData.Rows[0]["PRate"].ToString(), "CURR_FLOAT");
                    SetValueOut(GetEnum(GridColIndexes.cCrate), dtData.Rows[0]["CostRateExcl"].ToString(), "CURR_FLOAT");
                    SetValueOut(GetEnum(GridColIndexes.cCRateWithTax), dtData.Rows[0]["CostRateInc"].ToString(), "CURR_FLOAT");
                    SetValueOut(GetEnum(GridColIndexes.cGrossAmt), dtData.Rows[0]["PrateInc"].ToString(), "CURR_FLOAT");
                    SetValueOut(GetEnum(GridColIndexes.cItemID), dtData.Rows[0]["ItemID"].ToString(), "INT");
                    dQty = Comm.ToDecimal(dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cQty)].Value);
                    CalcTotalOut();

                }
            }
            else if (iStockID == 0)
            {
                sBarUnique = "<Auto Barcode>";
                SetValueOut(GetEnum(GridColIndexes.cBarCode), sBarUnique);
            }
        }

        //Description: Get Tax Mode details from Database
        public DataTable GetTaxMode(int iselID = 0)
        {
            GetTaxMinfo.TaxModeID = iselID;
            GetTaxMinfo.TenantID = Global.gblTenantID;
            return clsTax.GetTaxMode(GetTaxMinfo);
        }

        //Description: Get Agent Details from Database
        public DataTable GetAgent(int iSelID = 0)
        {
            GetAgentinfo.AgentID = iSelID;
            GetAgentinfo.TenantID = Global.gblTenantID;
            return clsAgent.GetAgentMaster(GetAgentinfo);
        }

        //Description : Get Ledger from Database and Fetching Only Supplier Details
        public DataTable GetLedger(decimal dSelNo = 0)
        {
            GetLedinfo.LID = dSelNo;
            GetLedinfo.GroupName = "";
            GetLedinfo.TenantID = Global.gblTenantID;
            return clsLedg.GetLedger(GetLedinfo);
        }

        //Description : Seting Value to the Given Column, Row Indexes to Grid Cell
        private void SetValue(int iCellIndex, string sValue, string sConvertType = "")
        {
            if (dgvStockIn.Rows.Count > 0)
            {
                if (sConvertType.ToUpper() == "CURR_FLOAT")
                {
                    if (sValue == "") sValue = "0";
                    //Commented and Added By Dipu on 17-Feb-2022 ------------- >>
                    dgvStockIn.Rows[dgvStockIn.CurrentRow.Index].Cells[iCellIndex].Value = Comm.chkChangeValuetoZero(Comm.FormatValue(Comm.ToDouble(sValue)));
                    //Commented and Added By Dipu on 17-Feb-2022 ------------- >>

                    this.dgvStockIn.Columns[dgvStockIn.CurrentCell.ColumnIndex].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
                else if (sConvertType.ToUpper() == "QTY_FLOAT")
                {
                    if (sValue == "") sValue = "0";
                    dgvStockIn.Rows[dgvStockIn.CurrentRow.Index].Cells[iCellIndex].Value = Comm.chkChangeValuetoZero(Comm.FormatValue(Comm.ToDouble(sValue), false));
                    this.dgvStockIn.Columns[dgvStockIn.CurrentCell.ColumnIndex].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
                else if (sConvertType.ToUpper() == "PERC_FLOAT")
                {
                    if (sValue == "") sValue = "0";
                    dgvStockIn.Rows[dgvStockIn.CurrentRow.Index].Cells[iCellIndex].Value = Comm.chkChangeValuetoZero(Comm.ToDecimal(sValue).ToString("#.00"));
                    this.dgvStockIn.Columns[dgvStockIn.CurrentCell.ColumnIndex].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
                else if (sConvertType.ToUpper() == "DATE")
                {
                    dgvStockIn.Rows[dgvStockIn.CurrentRow.Index].Cells[iCellIndex].Value = Convert.ToDateTime(sValue).ToString("dd/MMM/yyyy");
                }
                else
                    dgvStockIn.Rows[dgvStockIn.CurrentRow.Index].Cells[iCellIndex].Value = Comm.chkChangeValuetoZero(sValue);
            }
        }

        //Description : Setting Value to Tag of the cells of Grid using the Given Column and Row Indexes
        private void setTag(int iCellIndex, string sTag, string sConvertType = "")
        {
            if (sConvertType.ToUpper() == "FLOAT")
            {
                if (sTag == "") sTag = "0";
                dgvStockIn.Rows[dgvStockIn.CurrentRow.Index].Cells[iCellIndex].Tag = Comm.ToDecimal(sTag).ToString("#.00");
            }
            else if (sConvertType.ToUpper() == "DATE")
            {
                dgvStockIn.Rows[dgvStockIn.CurrentRow.Index].Cells[iCellIndex].Tag = Convert.ToDateTime(sTag).ToString("dd/MMM/yyyy");
            }
            else
                dgvStockIn.Rows[dgvStockIn.CurrentRow.Index].Cells[iCellIndex].Tag = sTag;
        }

        //Description : Function Polymorphism for Seting the Value to Grid Asper Parameters Given
        private void SetValue(int iCellIndex, int iRowIndex, string sValue, string sConvertType = "")
        {
            dgvStockIn.Rows[iRowIndex].Cells[iCellIndex].Value = Comm.chkChangeValuetoZero(sValue);
        }

        //Description : Seting Value to the Given Column, Row Indexes to Grid Cell
        private void SetValueOut(int iCellIndex, string sValue, string sConvertType = "")
        {
            if (dgvStockOut.Rows.Count > 0)
            {
                if (sConvertType.ToUpper() == "CURR_FLOAT")
                {
                    if (sValue == "") sValue = "0";
                    //Commented and Added By Dipu on 17-Feb-2022 ------------- >>
                    dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[iCellIndex].Value = Comm.chkChangeValuetoZero(Comm.FormatValue(Comm.ToDouble(sValue)));
                    //Commented and Added By Dipu on 17-Feb-2022 ------------- >>

                    this.dgvStockOut.Columns[dgvStockOut.CurrentCell.ColumnIndex].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
                else if (sConvertType.ToUpper() == "QTY_FLOAT")
                {
                    if (sValue == "") sValue = "0";
                    dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[iCellIndex].Value = Comm.chkChangeValuetoZero(Comm.FormatValue(Comm.ToDouble(sValue), false));
                    this.dgvStockOut.Columns[dgvStockOut.CurrentCell.ColumnIndex].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
                else if (sConvertType.ToUpper() == "PERC_FLOAT")
                {
                    if (sValue == "") sValue = "0";
                    dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[iCellIndex].Value = Comm.chkChangeValuetoZero(Comm.ToDecimal(sValue).ToString("#.00"));
                    this.dgvStockOut.Columns[dgvStockOut.CurrentCell.ColumnIndex].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
                else if (sConvertType.ToUpper() == "DATE")
                {
                    dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[iCellIndex].Value = Convert.ToDateTime(sValue).ToString("dd/MMM/yyyy");
                }
                else
                    dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[iCellIndex].Value = Comm.chkChangeValuetoZero(sValue);
            }
        }

        //Description : Setting Value to Tag of the cells of Grid using the Given Column and Row Indexes
        private void setTagOut(int iCellIndex, string sTag, string sConvertType = "")
        {
            if (sConvertType.ToUpper() == "FLOAT")
            {
                if (sTag == "") sTag = "0";
                dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[iCellIndex].Tag = Comm.ToDecimal(sTag).ToString("#.00");
            }
            else if (sConvertType.ToUpper() == "DATE")
            {
                dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[iCellIndex].Tag = Convert.ToDateTime(sTag).ToString("dd/MMM/yyyy");
            }
            else
                dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[iCellIndex].Tag = sTag;
        }

        //Description : Function Polymorphism for Seting the Value to Grid Asper Parameters Given
        private void SetValueOut(int iCellIndex, int iRowIndex, string sValue, string sConvertType = "")
        {
            dgvStockOut.Rows[iRowIndex].Cells[iCellIndex].Value = Comm.chkChangeValuetoZero(sValue);
        }

        //Description : Check the conditions of Supplier While Entered or Non Entred
        private bool CheckIsValidSupplier()
        {
            DataTable dtSupp = new DataTable();
            bool bResult = true;

                dtSupp = Comm.fnGetData("SELECT * FROM tblLedger WHERE LID = 100").Tables[0];
                if (dtSupp.Rows.Count > 0)
                {
                    bResult = true;
                }
                else
                    bResult = false;

            return bResult;
        }

        //Description : Validating the Method with Before Save Functionality
        private bool IsValidate(int iAction = 0)
        {
            if (iAction == 2 || iAction == 3)
                return true;


            DataTable dtInv = new DataTable();
            bool bValidate = true;
            string sWarnMsg = "|";
            string[] sMsg;

            if (clsVchTypeFeatures.blnWarnifSRatelessthanPrate == true)
                sWarnMsg = WarnifSRatelessthanPrate();

            sMsg = sWarnMsg.Split('|');

            if (txtInvAutoNo.Text == "")
            {
                bValidate = false;
                MessageBox.Show("Please Enter the Invoice No.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                txtInvAutoNo.Focus();
                goto FailsHere;
            }
            else if (Convert.ToString(dgvStockIn.Rows[0].Cells[GetEnum(GridColIndexes.CItemCode)].Value) == "")
            {
                bValidate = false;
                MessageBox.Show("No Items are Entered for Save. Please Enter the Item", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                goto FailsHere;
            }
            else if (sMsg[0].ToString() != "")
            {
                bValidate = false;
                MessageBox.Show("Sales Rates are Lesser Than of PRate of the Item[" + dgvStockIn.Rows[Comm.ToInt32(sMsg[1])].Cells[GetEnum(GridColIndexes.CItemName)].Value.ToString() + "], Check the Values [" + sMsg[0].ToString() + "].", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                goto FailsHere;
            }
            else
            {
                //if(Comm.ToDecimal(dgvStockIn.Rows[0].Cells[GetEnum(GridColIndexes.cPrate)].Value) == 0)
                for (int i = 0; i < dgvStockIn.Rows.Count; i++)
                {
                    if (dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.CItemCode)].Value != null)
                    {
                        bValidate = true;

                        string sQuery = "Select ItemID,BatchCode From tblStock Where LTRIM(RTRIM(BatchCode)) = '" + dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cBarCode)].Value.ToString() + "' and ItemID <> " + dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.CItemName)].Tag.ToString() + " ";
                        DataTable dtBatch = Comm.fnGetData(sQuery).Tables[0];
                        if (dtBatch.Rows.Count > 0)
                        {
                            MessageBox.Show("This BatchCode " + dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cBarCode)].Value.ToString() + " is already exist for another item.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            bValidate = false;
                            goto FailsHere;
                        }

                        if (Comm.ToDecimal(dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cPrate)].Value) == 0)
                        {
                            MessageBox.Show("Purchase rate cannot be zero. Please provide purchase rate for the item !", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            bValidate = false;
                            goto FailsHere;
                        }
                        if (Comm.ToDecimal(dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cQty)].Value) == 0 && Comm.ToDecimal(dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cFree)].Value) == 0)
                        {
                            MessageBox.Show("Quantity or Free Quantity is mandatory. Please provide any of them !", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            bValidate = false;
                            goto FailsHere;
                        }
                        if (bValidate == false)
                        {
                            MessageBox.Show("PRate or Qty Could not be Zero.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            goto FailsHere;
                        }
                    }
                }

            }
            if (txtInvAutoNo.Text.Trim() != "")
            {
                if (iIDFromEditWindow == 0)
                {
                    dtInv = Comm.fnGetData("SELECT InvNo FROM tblRepacking WHERE vchtypeid = " + vchtypeID + " and LTRIM(RTRIM(InvNo)) = '" + txtInvAutoNo.Text.Trim() + "'").Tables[0];
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
            if (clsVchTypeFeatures.BLNPOSTCASHENTRY == true)
            {
                if (dSupplierID <= 101)
                {
                    bValidate = false;
                    MessageBox.Show("Please select credit customer as you have chosen to post cash entries to supplier ledger.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    txtInvAutoNo.Focus();
                    goto FailsHere;
                }
            }
            else
            {
                for (int i = 0; i < dgvStockIn.Rows.Count; i++)
                {
                    if (iIDFromEditWindow == 0)
                    {
                        if (dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cBarCode)].Value != null)
                        {
                            if (dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cBatchMode)].Value.ToString().Trim() != "2")
                            {
                                string sQuery = "Select ItemID,BatchCode From tblStock Where LTRIM(RTRIM(BatchCode)) = '" + dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cBarCode)].Value.ToString() + "' AND ItemID <> " + Comm.ToDouble(dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cItemID)].Value.ToString()) + "";
                                DataTable dtBatch = Comm.fnGetData(sQuery).Tables[0];
                                if (dtBatch.Rows.Count > 0)
                                {
                                    bValidate = false;
                                    MessageBox.Show("This BatchCode " + dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cBarCode)].Value.ToString() + "of Item [" + dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.CItemName)].Value.ToString() + "] is already Exist.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    break;
                                }
                            }
                        }
                    }
                    //Dipu on 19-May-2022 -------------------- >> Do Not Allow Net Amount is Greater than of CRate and CRate With Tax
                    if (Comm.ToDouble(dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cCrate)].Value) > 0)
                    {
                        bValidate = true;
                        if (Comm.ToDouble(dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cCrate)].Value) > Comm.ToDouble(dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cNetAmount)].Value))
                        {
                            bValidate = false;
                            MessageBox.Show("Do not allow the Net Amount is Greater than of CRate or CRate With Tax. Check the Other Charges or Cost Factor are Correct !", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            goto FailsHere;
                            //break;
                        }
                        else if (Comm.ToDouble(dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cCRateWithTax)].Value) > Comm.ToDouble(dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cNetAmount)].Value))
                        {
                            bValidate = false;
                            MessageBox.Show("Do not allow the Net Amount is Greater than of CRate or CRate With Tax. Check the Other Charges or Cost Factor are Correct !", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            goto FailsHere;
                            //break;
                        }
                        else if (Convert.ToDateTime(dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.CExpiry)].Value) <= Convert.ToDateTime(DateTime.Today))
                        {
                            bValidate = false;
                            MessageBox.Show("Do Not Allow the Previous Expiry Date !", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            goto FailsHere;
                            //break;
                        }
                    }
                }

                for (int j = 0; j < dgvStockIn.Rows.Count; j++)
                {
                    bValidate = true;
                    if (Comm.ToDecimal(dgvStockIn.Rows[j].Cells[GetEnum(GridColIndexes.cPrate)].Value) > Comm.ToDecimal(dgvStockIn.Rows[j].Cells[GetEnum(GridColIndexes.cMRP)].Value))
                    {
                        bValidate = false;
                        MessageBox.Show("Purchase Rate Above MRP.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        goto FailsHere;
                        //break;
                    }
                    else if (Comm.ToDecimal(dgvStockIn.Rows[j].Cells[GetEnum(GridColIndexes.cSRate1)].Value) > Comm.ToDecimal(dgvStockIn.Rows[j].Cells[GetEnum(GridColIndexes.cMRP)].Value))
                    {
                        bValidate = false;
                        MessageBox.Show("Sales Rate 1 Above MRP.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        goto FailsHere;
                        //break;
                    }
                    else if (Comm.ToDecimal(dgvStockIn.Rows[j].Cells[GetEnum(GridColIndexes.cSRate2)].Value) > Comm.ToDecimal(dgvStockIn.Rows[j].Cells[GetEnum(GridColIndexes.cMRP)].Value))
                    {
                        if (AppSettings.IsActiveSRate2 == true)
                        {
                            bValidate = false;
                            MessageBox.Show("Sales Rate 2 Above MRP.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            goto FailsHere;
                            //break;
                        }
                    }
                    else if (Comm.ToDecimal(dgvStockIn.Rows[j].Cells[GetEnum(GridColIndexes.cSRate3)].Value) > Comm.ToDecimal(dgvStockIn.Rows[j].Cells[GetEnum(GridColIndexes.cMRP)].Value))
                    {
                        if (AppSettings.IsActiveSRate3 == true)
                        {
                            bValidate = false;
                            MessageBox.Show("Sales Rate 3 Above MRP.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            goto FailsHere;
                            //break;
                        }
                    }
                    else if (Comm.ToDecimal(dgvStockIn.Rows[j].Cells[GetEnum(GridColIndexes.cSRate4)].Value) > Comm.ToDecimal(dgvStockIn.Rows[j].Cells[GetEnum(GridColIndexes.cMRP)].Value))
                    {
                        if (AppSettings.IsActiveSRate4 == true)
                        {
                            bValidate = false;
                            MessageBox.Show("Sales Rate 4 Above MRP.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            goto FailsHere;
                            //break;
                        }
                    }
                    else if (Comm.ToDecimal(dgvStockIn.Rows[j].Cells[GetEnum(GridColIndexes.cSRate5)].Value) > Comm.ToDecimal(dgvStockIn.Rows[j].Cells[GetEnum(GridColIndexes.cMRP)].Value))
                    {
                        if (AppSettings.IsActiveSRate5 == true)
                        {
                            bValidate = false;
                            MessageBox.Show("Sales Rate 5 Above MRP.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            goto FailsHere;
                            //break;
                        }
                    }
                    if (bValidate == false)
                    {
                        MessageBox.Show("MRP Should be Greater than Prate or SRates !!", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        goto FailsHere;
                    }
                }

            }
            FailsHere:
            return bValidate;
        }

        //Description : Show Warning Message When SRate is Less Than of PRate.
        private string WarnifSRatelessthanPrate()
        {
            string sData = "";
            int i;
            for (i = 0; i < dgvStockIn.Rows.Count; i++)
            {
                if (dgvStockIn.Rows[0].Cells[GetEnum(GridColIndexes.CItemCode)].Value != null)
                {
                    if (dgvStockIn.Rows[0].Cells[GetEnum(GridColIndexes.CItemCode)].Value.ToString() != "")
                    {
                        if (Comm.ToDecimal(dgvStockIn.Rows[0].Cells[GetEnum(GridColIndexes.cPrate)].Value) > Comm.ToDecimal(dgvStockIn.Rows[0].Cells[GetEnum(GridColIndexes.cSRate1)].Value))
                            sData = sData + AppSettings.SRate1Name + " ,";
                        else if (Comm.ToDecimal(dgvStockIn.Rows[0].Cells[GetEnum(GridColIndexes.cPrate)].Value) > Comm.ToDecimal(dgvStockIn.Rows[0].Cells[GetEnum(GridColIndexes.cSRate2)].Value))
                        {
                            if (AppSettings.IsActiveSRate2 == true)
                                sData = sData + AppSettings.SRate2Name + " ,";
                        }
                        else if (Comm.ToDecimal(dgvStockIn.Rows[0].Cells[GetEnum(GridColIndexes.cPrate)].Value) > Comm.ToDecimal(dgvStockIn.Rows[0].Cells[GetEnum(GridColIndexes.cSRate3)].Value))
                        {
                            if (AppSettings.IsActiveSRate3 == true)
                                sData = sData + AppSettings.SRate3Name + " ,";
                        }
                        else if (Comm.ToDecimal(dgvStockIn.Rows[0].Cells[GetEnum(GridColIndexes.cPrate)].Value) > Comm.ToDecimal(dgvStockIn.Rows[0].Cells[GetEnum(GridColIndexes.cSRate4)].Value))
                        {
                            if (AppSettings.IsActiveSRate4 == true)
                                sData = sData + AppSettings.SRate4Name + " ,";
                        }
                        else if (Comm.ToDecimal(dgvStockIn.Rows[0].Cells[GetEnum(GridColIndexes.cPrate)].Value) > Comm.ToDecimal(dgvStockIn.Rows[0].Cells[GetEnum(GridColIndexes.cSRate5)].Value))
                        {
                            if (AppSettings.IsActiveSRate5 == true)
                                sData = sData + AppSettings.SRate5Name + " ,";
                        }
                        if (sData != "")
                            break;
                    }
                }
            }
            return sData + "|" + i.ToString();
        }

        //Description : Get Whole data from Tax Mode and return to Array
        private string[] GetTaxModeData(decimal dTaxMode = 0)
        {
            if (dTaxMode != 0)
            {
                List<string> lstTaxMod = new List<string>();
                DataTable dtTaxMd = new DataTable();
                dtTaxMd = Comm.fnGetData("SELECT * FROM tblTaxMode WHERE TaxModeID = " + dTaxMode + "").Tables[0];
                if (dtTaxMd.Rows.Count > 0)
                {
                    lstTaxMod.Add(dtTaxMd.Rows[0]["CalculationID"].ToString());
                    lstTaxMod.Add(dtTaxMd.Rows[0]["SortNo"].ToString());
                    lstTaxMod.Add(dtTaxMd.Rows[0]["ActiveStatus"].ToString());
                }
                return lstTaxMod.ToArray();
            }
            else
                return null;
        }

        //Description : Get Whole data from Agent Master and return to Array
        private string[] GetAgentData(decimal dAgentID = 0)
        {
            if (dAgentID != 0)
            {
                List<string> lstAgent = new List<string>();
                DataTable dtAgentdat = new DataTable();
                dtAgentdat = Comm.fnGetData("SELECT * FROM tblAgent WHERE AgentID = " + dAgentID + "").Tables[0];
                if (dtAgentdat.Rows.Count > 0)
                {
                    lstAgent.Add(dtAgentdat.Rows[0][GetEnumAgent(AgentIndexes.AgentID)].ToString());
                    lstAgent.Add(dtAgentdat.Rows[0][GetEnumAgent(AgentIndexes.AgentCode)].ToString());
                    lstAgent.Add(dtAgentdat.Rows[0][GetEnumAgent(AgentIndexes.AgentName)].ToString());
                    lstAgent.Add(dtAgentdat.Rows[0][GetEnumAgent(AgentIndexes.Area)].ToString());
                    lstAgent.Add(dtAgentdat.Rows[0][GetEnumAgent(AgentIndexes.Commission)].ToString());
                    lstAgent.Add(dtAgentdat.Rows[0][GetEnumAgent(AgentIndexes.blnPOstAccounts)].ToString());
                    lstAgent.Add(dtAgentdat.Rows[0][GetEnumAgent(AgentIndexes.ADDRESS)].ToString());
                    lstAgent.Add(dtAgentdat.Rows[0][GetEnumAgent(AgentIndexes.LOCATION)].ToString());
                    lstAgent.Add(dtAgentdat.Rows[0][GetEnumAgent(AgentIndexes.PHONE)].ToString());
                    lstAgent.Add(dtAgentdat.Rows[0][GetEnumAgent(AgentIndexes.WEBSITE)].ToString());
                    lstAgent.Add(dtAgentdat.Rows[0][GetEnumAgent(AgentIndexes.EMAIL)].ToString());
                    lstAgent.Add(dtAgentdat.Rows[0][GetEnumAgent(AgentIndexes.BLNROOMRENT)].ToString());
                    lstAgent.Add(dtAgentdat.Rows[0][GetEnumAgent(AgentIndexes.BLNSERVICES)].ToString());
                    lstAgent.Add(dtAgentdat.Rows[0][GetEnumAgent(AgentIndexes.blnItemwiseCommission)].ToString());
                    lstAgent.Add(dtAgentdat.Rows[0][GetEnumAgent(AgentIndexes.AgentDiscount)].ToString());
                    lstAgent.Add(dtAgentdat.Rows[0][GetEnumAgent(AgentIndexes.LID)].ToString());
                    lstAgent.Add(dtAgentdat.Rows[0][GetEnumAgent(AgentIndexes.SystemName)].ToString());
                    lstAgent.Add(dtAgentdat.Rows[0][GetEnumAgent(AgentIndexes.UserID)].ToString());
                    lstAgent.Add(dtAgentdat.Rows[0][GetEnumAgent(AgentIndexes.LastUpdateDate)].ToString());
                    lstAgent.Add(dtAgentdat.Rows[0][GetEnumAgent(AgentIndexes.LastUpdateTime)].ToString());
                    lstAgent.Add(dtAgentdat.Rows[0][GetEnumAgent(AgentIndexes.TenantID)].ToString());
                }
                return lstAgent.ToArray();
            }
            else
                return null;
        }

        //Description : Get Whole data from State Master and return to Array
        private string[] GetStateData(decimal dStateID = 0)
        {
            if (dStateID != 0)
            {
                List<string> lstState = new List<string>();
                DataTable dtStatedat = new DataTable();
                dtStatedat = Comm.fnGetData("SELECT * FROM tblStates WHERE StateID = " + dStateID + "").Tables[0];
                if (dtStatedat.Rows.Count > 0)
                {
                    lstState.Add(dtStatedat.Rows[0]["StateCode"].ToString());
                    lstState.Add(dtStatedat.Rows[0]["StateType"].ToString());
                    lstState.Add(dtStatedat.Rows[0]["Country"].ToString());
                    lstState.Add(dtStatedat.Rows[0]["CountryID"].ToString());
                }
                return lstState.ToArray();
            }
            else
                return null;
        }

        //Description : Get Whole data from Employee Master and return to Array
        private string[] GetEmpDetails(decimal dEmpID = 0)
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
                    lstItm.Add(Comm.CheckDBNullOrEmpty(dtItm.Rows[0][GetEnumItem(ItemIndexes.SrateCalcMode)].ToString()));
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

        //Description : Serialize the Repacking table Fields asper instructions.
        private string SerializetoJson()
        {
            #region "Repacking Master (tblRepacking) ------------------------------- >>"

            if (dSupplierID == 0)
            {
                    DataTable dtDefaultSupp = Comm.fnGetData("select top 1 LID,LName,LAliasName,Address,MobileNo,AccountGroupID from tblLedger WHERE LID = 100 AND GroupName = 'SUPPLIER'").Tables[0];
                    if (dtDefaultSupp.Rows.Count > 0)
                    {
                        dSupplierID = Comm.ToDecimal(dtDefaultSupp.Rows[0]["LID"].ToString());
                        FillSupplierForSerializeJsonUsingID(decimal.ToInt32(dSupplierID));
                    }
                    else
                    {
                        dSupplierID = 0;
                        FillSupplierForSerializeJsonUsingID(100);
                    }
            }
            else if (dSupplierID == 100)
            {
                FillSupplierForSerializeJsonUsingID(100);
            }
            if (iIDFromEditWindow == 0)
            {
                clsJPMinfo.InvId = Comm.gfnGetNextSerialNo("tblRepacking", "InvId");
                txtInvAutoNo.Tag = clsJPMinfo.InvId;
                clsJPMinfo.AutoNum = Comm.ToDecimal(Comm.gfnGetNextSerialNo("tblRepacking", "AutoNum", " VchTypeID=" + vchtypeID).ToString());
            }
            else
            {
                clsJPMinfo.InvId = Comm.ToDecimal(iIDFromEditWindow);
                txtInvAutoNo.Tag = Comm.ToDecimal(iIDFromEditWindow);
                if (txtReferenceAutoNo.Tag.ToString() == "") txtReferenceAutoNo.Tag = 0;
                clsJPMinfo.AutoNum = Comm.ToDecimal(txtReferenceAutoNo.Tag.ToString().Replace("'", "''"));
            }

            clsJPMinfo.InvNo = txtInvAutoNo.Text.Replace("'","''");
            clsJPMinfo.Prefix = txtPrefix.Text.Replace("'", "''").Trim();
            clsJPMinfo.InvDate = Convert.ToDateTime(dtpInvDate.Text);
            clsJPMinfo.VchType = clsVchType.TransactionName.Replace("'", "''");
            clsJPMinfo.MOP = Convert.ToString("CASH");
            clsJPMinfo.TaxModeID = Comm.ToDecimal(1);
            clsJPMinfo.LedgerId = Comm.ToDecimal(100);
            clsJPMinfo.Party = "";
            clsJPMinfo.Discount = Comm.ToDecimal(0);
            clsJPMinfo.TaxAmt = Comm.ToDecimal(0);
            clsJPMinfo.GrossAmt = Comm.ToDecimal(0);
            clsJPMinfo.QtyTotal = Comm.ToDecimal(0);
            clsJPMinfo.FreeTotal = Comm.ToDecimal(0);
            clsJPMinfo.BillAmt = Comm.ToDecimal(0);
            clsJPMinfo.CoolieTotal = Comm.ToDecimal(0);

            clsJPMinfo.Cancelled = 0;
            clsJPMinfo.OtherExpense = Comm.ToDecimal(0);
            clsJPMinfo.SalesManID = Comm.ToDecimal(1);
            clsJPMinfo.Taxable = Comm.ToDecimal(0);
            clsJPMinfo.NonTaxable = Comm.ToDecimal(0);
            clsJPMinfo.ItemDiscountTotal = Comm.ToDecimal(0);
            clsJPMinfo.RoundOff = Comm.ToDecimal(0);
            clsJPMinfo.UserNarration = "";
            clsJPMinfo.SortNumber = 0;
            clsJPMinfo.DiscPer = Comm.ToDecimal(0);
            clsJPMinfo.VchTypeID = vchtypeID;
            clsJPMinfo.CCID = Comm.ToDecimal(cboCostCentre.SelectedValue);
            clsJPMinfo.CurrencyID = 0;
            clsJPMinfo.PartyAddress = "";
            clsJPMinfo.UserID = Global.gblUserID;
            clsJPMinfo.AgentID = Comm.ToDecimal(0);
            clsJPMinfo.CashDiscount = Comm.ToDecimal(0);
            clsJPMinfo.DPerType_ManualCalc_Customer = 0;
            clsJPMinfo.NetAmount = Comm.ToDecimal(0);
            clsJPMinfo.RefNo = txtReferencePrefix.Text;
            clsJPMinfo.CashPaid = 0;
            clsJPMinfo.CardPaid = 0;
            clsJPMinfo.blnWaitforAuthorisation = 0;
            clsJPMinfo.UserIDAuth = 0;
            clsJPMinfo.BillTime = DateTime.Now;
            clsJPMinfo.StateID = Comm.ToDecimal(32);
            clsJPMinfo.ImplementingStateCode = "";
            clsJPMinfo.GSTType = "";
            clsJPMinfo.CGSTTotal = 0;
            clsJPMinfo.SGSTTotal = 0;
            clsJPMinfo.IGSTTotal = 0;
            clsJPMinfo.PartyGSTIN = "";
            clsJPMinfo.BillType = "";
            clsJPMinfo.blnHold = 0;
            clsJPMinfo.PriceListID = 0;
            clsJPMinfo.EffectiveDate = Convert.ToDateTime("01-Jan-2000");
            clsJPMinfo.partyCode = "";
            clsJPMinfo.MobileNo = "";
            clsJPMinfo.Email = "";
            clsJPMinfo.TaxType = "";
            clsJPMinfo.QtyTotal = 0;
            clsJPMinfo.DestCCID = 0;
            clsJPMinfo.AgentCommMode = "";
            clsJPMinfo.AgentCommAmount = 0;
            clsJPMinfo.AgentLID = 0;
            clsJPMinfo.BlnStockInsert = 0;
            clsJPMinfo.BlnConverted = 0;
            clsJPMinfo.ConvertedParentVchTypeID = 0;
            clsJPMinfo.ConvertedVchTypeID = 0;
            clsJPMinfo.ConvertedVchNo = "";
            clsJPMinfo.ConvertedVchID = 0;
            clsJPMinfo.DeliveryNoteDetails = "";
            clsJPMinfo.OrderDetails = "";
            clsJPMinfo.IntegrityStatus = "";
            clsJPMinfo.BalQty = 0;
            clsJPMinfo.CustomerpointsSettled = 0;
            clsJPMinfo.blnCashPaid = 0;
            clsJPMinfo.originalsalesinvid = 0;
            clsJPMinfo.retuninvid = 0;
            clsJPMinfo.returnamount = 0;
            clsJPMinfo.SystemName = Global.gblSystemName;
            clsJPMinfo.LastUpdateDate = DateTime.Today;
            clsJPMinfo.LastUpdateTime = DateTime.Now;
            clsJPMinfo.DeliveryDetails = "";
            clsJPMinfo.DespatchDetails = "";
            clsJPMinfo.TermsOfDelivery = "";
            clsJPMinfo.FloodCessTot = 0;
            clsJPMinfo.CounterID = 0;
            clsJPMinfo.ExtraCharges = 0;
            clsJPMinfo.ReferenceAutoNO = txtReferenceAutoNo.Text.Replace("'", "''");
            clsJPMinfo.CashDisPer = Comm.ToDecimal(0);
            clsJPMinfo.CostFactor = Comm.ToDecimal(0);
            clsJPMinfo.TenantID = Global.gblTenantID;
            clsPM.clsJsonPMInfo_ = clsJPMinfo;

            #endregion

            #region "Supplier Data (tblLedger) ----------------------------------- >>"

            clsJPMLedgerinfo.LID = dSupplierID;
            clsJPMLedgerinfo.LName = "";
            clsJPMLedgerinfo.LAliasName = "";
            clsJPMLedgerinfo.GroupName = sArrLedger[GetEnumLedger(LedgerIndexes.GroupName)].ToString().Replace("'", "''");
            clsJPMLedgerinfo.Type = sArrLedger[GetEnumLedger(LedgerIndexes.Type)].ToString();
            clsJPMLedgerinfo.OpBalance = Comm.ToDecimal(sArrLedger[GetEnumLedger(LedgerIndexes.OpBalance)]);
            clsJPMLedgerinfo.AppearIn = sArrLedger[GetEnumLedger(LedgerIndexes.AppearIn)].ToString();
            clsJPMLedgerinfo.Address = "";
            clsJPMLedgerinfo.CreditDays = sArrLedger[GetEnumLedger(LedgerIndexes.CreditDays)].ToString();
            clsJPMLedgerinfo.Phone = sArrLedger[GetEnumLedger(LedgerIndexes.Phone)].ToString();
            clsJPMLedgerinfo.TaxNo = "";
            clsJPMLedgerinfo.AccountGroupID = Comm.ToDecimal(sArrLedger[GetEnumLedger(LedgerIndexes.AccountGroupID)].ToString());
            clsJPMLedgerinfo.RouteID = Comm.ToDecimal(sArrLedger[GetEnumLedger(LedgerIndexes.RouteID)].ToString());
            clsJPMLedgerinfo.Area = sArrLedger[GetEnumLedger(LedgerIndexes.Area)].ToString().Replace("'", "''");
            clsJPMLedgerinfo.Notes = sArrLedger[GetEnumLedger(LedgerIndexes.Notes)].ToString().Replace("'", "''");
            clsJPMLedgerinfo.TargetAmt = Comm.ToDecimal(sArrLedger[GetEnumLedger(LedgerIndexes.TargetAmt)].ToString());
            clsJPMLedgerinfo.SMSSchID = Comm.ToDecimal(sArrLedger[GetEnumLedger(LedgerIndexes.SMSSchID)].ToString());
            clsJPMLedgerinfo.Email = sArrLedger[GetEnumLedger(LedgerIndexes.Email)].ToString().Replace("'", "''");
            clsJPMLedgerinfo.MobileNo = "";
            clsJPMLedgerinfo.DiscPer = Comm.ToDecimal(sArrLedger[GetEnumLedger(LedgerIndexes.DiscPer)].ToString());
            clsJPMLedgerinfo.InterestPer = Comm.ToDecimal(sArrLedger[GetEnumLedger(LedgerIndexes.InterestPer)].ToString());
            clsJPMLedgerinfo.DummyLName = sArrLedger[GetEnumLedger(LedgerIndexes.DummyLName)].ToString().Replace("'", "''");
            clsJPMLedgerinfo.BlnBank = Comm.ToDecimal(sArrLedger[GetEnumLedger(LedgerIndexes.BlnBank)].ToString());
            clsJPMLedgerinfo.CurrencyID = Comm.ToDecimal(sArrLedger[GetEnumLedger(LedgerIndexes.CurrencyID)].ToString());
            clsJPMLedgerinfo.AreaID = Comm.ToDecimal(sArrLedger[GetEnumLedger(LedgerIndexes.AreaID)].ToString());
            clsJPMLedgerinfo.PLID = Comm.ToDecimal(sArrLedger[GetEnumLedger(LedgerIndexes.PLID)].ToString());
            clsJPMLedgerinfo.ActiveStatus = Comm.ToDecimal(sArrLedger[GetEnumLedger(LedgerIndexes.ActiveStatus)].ToString());
            clsJPMLedgerinfo.EmailAddress = sArrLedger[GetEnumLedger(LedgerIndexes.EmailAddress)].ToString().Replace("'", "''");
            clsJPMLedgerinfo.EntryDate = Convert.ToDateTime(sArrLedger[GetEnumLedger(LedgerIndexes.EntryDate)].ToString());
            clsJPMLedgerinfo.blnBillWise = Comm.ToDecimal(sArrLedger[GetEnumLedger(LedgerIndexes.blnBillWise)].ToString());
            clsJPMLedgerinfo.CustomerCardID = Comm.ToDecimal(sArrLedger[GetEnumLedger(LedgerIndexes.CustomerCardID)].ToString());
            clsJPMLedgerinfo.TDSPer = Comm.ToDecimal(sArrLedger[GetEnumLedger(LedgerIndexes.TDSPer)].ToString());
            clsJPMLedgerinfo.DOB = Convert.ToDateTime(sArrLedger[GetEnumLedger(LedgerIndexes.DOB)].ToString());
            clsJPMLedgerinfo.StateID = Comm.ToDecimal(32);
            clsJPMLedgerinfo.CCIDS = sArrLedger[GetEnumLedger(LedgerIndexes.CCIDS)].ToString();
            clsJPMLedgerinfo.CurrentBalance = Comm.ToDecimal(sArrLedger[GetEnumLedger(LedgerIndexes.CurrentBalance)].ToString());
            clsJPMLedgerinfo.LedgerName = sArrLedger[GetEnumLedger(LedgerIndexes.LedgerName)].ToString().Replace("'", "''");
            clsJPMLedgerinfo.LedgerCode = sArrLedger[GetEnumLedger(LedgerIndexes.LedgerCode)].ToString().Replace("'", "''");
            clsJPMLedgerinfo.BlnWallet = Comm.ToDecimal(sArrLedger[GetEnumLedger(LedgerIndexes.BlnWallet)].ToString());
            clsJPMLedgerinfo.blnCoupon = Comm.ToDecimal(sArrLedger[GetEnumLedger(LedgerIndexes.blnCoupon)].ToString());
            clsJPMLedgerinfo.TransComn = Comm.ToDecimal(sArrLedger[GetEnumLedger(LedgerIndexes.TransComn)].ToString());
            clsJPMLedgerinfo.BlnSmsWelcome = Comm.ToDecimal(sArrLedger[GetEnumLedger(LedgerIndexes.BlnSmsWelcome)].ToString().Replace("'", "''"));
            clsJPMLedgerinfo.DLNO = sArrLedger[GetEnumLedger(LedgerIndexes.DLNO)].ToString().Replace("'", "''");
            clsJPMLedgerinfo.TDS = Comm.ToDecimal(sArrLedger[GetEnumLedger(LedgerIndexes.TDS)].ToString());
            clsJPMLedgerinfo.LedgerNameUnicode = sArrLedger[GetEnumLedger(LedgerIndexes.LedgerNameUnicode)].ToString().Replace("'", "''");
            clsJPMLedgerinfo.LedgerAliasNameUnicode = sArrLedger[GetEnumLedger(LedgerIndexes.LedgerAliasNameUnicode)].ToString().Replace("'", "''");
            clsJPMLedgerinfo.ContactPerson = sArrLedger[GetEnumLedger(LedgerIndexes.ContactPerson)].ToString().Replace("'", "''");
            clsJPMLedgerinfo.TaxParameter = sArrLedger[GetEnumLedger(LedgerIndexes.TaxParameter)].ToString();
            clsJPMLedgerinfo.TaxParameterType = sArrLedger[GetEnumLedger(LedgerIndexes.TaxParameterType)].ToString();
            clsJPMLedgerinfo.HSNCODE = sArrLedger[GetEnumLedger(LedgerIndexes.HSNCODE)].ToString().Replace("'", "''");
            clsJPMLedgerinfo.CGSTTaxPer = Comm.ToDecimal(sArrLedger[GetEnumLedger(LedgerIndexes.CGSTTaxPer)].ToString());
            clsJPMLedgerinfo.SGSTTaxPer = Comm.ToDecimal(sArrLedger[GetEnumLedger(LedgerIndexes.SGSTTaxPer)].ToString());
            clsJPMLedgerinfo.IGSTTaxPer = Comm.ToDecimal(sArrLedger[GetEnumLedger(LedgerIndexes.IGSTTaxPer)].ToString());
            clsJPMLedgerinfo.HSNID = Comm.ToDecimal(sArrLedger[GetEnumLedger(LedgerIndexes.HSNID)].ToString());
            clsJPMLedgerinfo.BankAccountNo = Comm.ToDecimal(sArrLedger[GetEnumLedger(LedgerIndexes.BankAccountNo)].ToString().Replace("'", "''"));
            clsJPMLedgerinfo.BankIFSCCode = sArrLedger[GetEnumLedger(LedgerIndexes.BankIFSCCode)].ToString().Replace("'", "''");
            clsJPMLedgerinfo.BankNote = sArrLedger[GetEnumLedger(LedgerIndexes.BankNote)].ToString().Replace("'", "''");
            clsJPMLedgerinfo.WhatsAppNo = Comm.ToDecimal(sArrLedger[GetEnumLedger(LedgerIndexes.WhatsAppNo)].ToString());
            //Dipu 21-03-2022 ------- >>
            clsJPMLedgerinfo.TenantID = Global.gblTenantID;
            clsJPMLedgerinfo.GSTType = "";
            clsJPMLedgerinfo.AgentID = Comm.ToDecimal(sArrLedger[GetEnumLedger(LedgerIndexes.AgentID)].ToString());
            clsPM.clsJsonPMLedgerInfo_ = clsJPMLedgerinfo;

            #endregion

            #region "TAX Mode (tblTaxMode) --------------------------------------- >>"

            string[] sArrTMod = GetTaxModeData(Comm.ToDecimal(1));
            clsJPMTaxModinfo.TaxModeID = Comm.ToDecimal(1);
            clsJPMTaxModinfo.TaxMode = "<None>";
            if (sArrTMod.Length > 0)
            {
                clsJPMTaxModinfo.CalculationID = Comm.ToInt32(sArrTMod[0].ToString());
                clsJPMTaxModinfo.SortNo = Comm.ToInt32(sArrTMod[1].ToString());
                clsJPMTaxModinfo.ActiveStatus = Comm.ToInt32(sArrTMod[1].ToString());
            }
            else
            {
                clsJPMTaxModinfo.CalculationID = 0;
                clsJPMTaxModinfo.SortNo = 0;
                clsJPMTaxModinfo.ActiveStatus = 1;
            }
            //Dipu 21-03-2022 ------- >>
            clsJPMTaxModinfo.TenantID = Global.gblTenantID;
            clsPM.clsJsonPMTaxmodeInfo_ = clsJPMTaxModinfo;

            #endregion

            #region "Agent Master (tblAgent) ------------------------------------- >>"

            string[] sArrAgent = GetAgentData(Comm.ToDecimal(1));
            clsJPMAgentinfo.AgentID = Comm.ToDecimal(0);
            clsJPMAgentinfo.AgentCode = sArrAgent[GetEnumAgent(AgentIndexes.AgentCode)];
            clsJPMAgentinfo.AgentName = sArrAgent[GetEnumAgent(AgentIndexes.AgentName)];
            clsJPMAgentinfo.Area = sArrAgent[GetEnumAgent(AgentIndexes.Area)];
            clsJPMAgentinfo.Commission = Comm.ToDecimal(sArrAgent[GetEnumAgent(AgentIndexes.Commission)]);
            clsJPMAgentinfo.blnPOstAccounts = Comm.ToInt32(sArrAgent[GetEnumAgent(AgentIndexes.blnPOstAccounts)]);
            clsJPMAgentinfo.ADDRESS = sArrAgent[GetEnumAgent(AgentIndexes.ADDRESS)];
            clsJPMAgentinfo.LOCATION = sArrAgent[GetEnumAgent(AgentIndexes.LOCATION)];
            clsJPMAgentinfo.PHONE = sArrAgent[GetEnumAgent(AgentIndexes.PHONE)];
            clsJPMAgentinfo.WEBSITE = sArrAgent[GetEnumAgent(AgentIndexes.WEBSITE)];
            clsJPMAgentinfo.EMAIL = sArrAgent[GetEnumAgent(AgentIndexes.EMAIL)];
            clsJPMAgentinfo.BLNROOMRENT = Comm.ToInt32(sArrAgent[GetEnumAgent(AgentIndexes.BLNROOMRENT)]);
            clsJPMAgentinfo.BLNSERVICES = Comm.ToInt32(sArrAgent[GetEnumAgent(AgentIndexes.BLNSERVICES)]);
            clsJPMAgentinfo.blnItemwiseCommission = Comm.ToInt32(sArrAgent[GetEnumAgent(AgentIndexes.blnItemwiseCommission)]);
            clsJPMAgentinfo.AgentDiscount = Comm.ToDecimal(sArrAgent[GetEnumAgent(AgentIndexes.AgentDiscount)]);

            if (sArrAgent[GetEnumAgent(AgentIndexes.LID)] == "") sArrAgent[GetEnumAgent(AgentIndexes.LID)] = "0";
            clsJPMAgentinfo.LID = Comm.ToInt32(sArrAgent[GetEnumAgent(AgentIndexes.LID)]);

            //Dipu 21-03-2022 ------- >>
            clsJPMAgentinfo.TenantID = Global.gblTenantID;
            clsPM.clsJsonPMAgentInfo_ = clsJPMAgentinfo;

            #endregion

            #region "Cost Center (tblCostCenter) --------------------------------- >>"

            clsJPMCostCentreinfo.CCID = Comm.ToDecimal(cboCostCentre.SelectedValue);
            clsJPMCostCentreinfo.CCName = cboCostCentre.SelectedItem.ToString();
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
            clsPM.clsJsonPMCCentreInfo_ = clsJPMCostCentreinfo;

            #endregion

            #region "State Master (tblStates) ------------------------------------ >>"

            string[] sArrState = GetStateData(Comm.ToDecimal(32));
            clsJPMStateinfo.StateId = Comm.ToDecimal(32);
            clsJPMStateinfo.StateCode = sArrState[0].ToString();
            clsJPMStateinfo.State = "";
            clsJPMStateinfo.StateType = sArrState[1].ToString();
            clsJPMStateinfo.Country = sArrState[2].ToString();
            clsJPMStateinfo.CountryID = Comm.ToDecimal(sArrState[3].ToString());
            clsJPMStateinfo.TenantID = Global.gblTenantID;
            clsPM.clsJsonPMStateInfo_ = clsJPMStateinfo;

            #endregion

            #region "Employee Master (tblEmployee) ------------------------------- >>"

            string[] sArrEmp = GetEmpDetails(Comm.ToDecimal(1));
            clsJPMEmployeeinfo.EmpID = Comm.ToDecimal(1);
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
            clsJPMEmployeeinfo.LICMonthlyPremium = Comm.ToDecimal(sArrEmp[GetEnumEmp(EmpIndexes.LICMonthlyPremium)]);
            clsJPMEmployeeinfo.LICDateofMaturity = Convert.ToDateTime(sArrEmp[GetEnumEmp(EmpIndexes.LICDateofMaturity)]);
            clsJPMEmployeeinfo.CategoryID = Comm.ToInt32(sArrEmp[GetEnumEmp(EmpIndexes.CategoryID)]);
            clsJPMEmployeeinfo.DateofPromotion = Convert.ToDateTime(sArrEmp[GetEnumEmp(EmpIndexes.DateofPromotion)]);
            clsJPMEmployeeinfo.DateofRetirement = Convert.ToDateTime(sArrEmp[GetEnumEmp(EmpIndexes.DateofRetirement)]);
            clsJPMEmployeeinfo.GISAccNo = sArrEmp[GetEnumEmp(EmpIndexes.GISAccNo)];
            clsJPMEmployeeinfo.BankAccNo = sArrEmp[GetEnumEmp(EmpIndexes.BankAccNo)];
            clsJPMEmployeeinfo.Commission = Comm.ToDecimal(sArrEmp[GetEnumEmp(EmpIndexes.Commission)]);
            clsJPMEmployeeinfo.CommissionAmt = Comm.ToDecimal(sArrEmp[GetEnumEmp(EmpIndexes.CommissionAmt)]);
            clsJPMEmployeeinfo.EmpFname = Convert.ToString(sArrEmp[GetEnumEmp(EmpIndexes.EmpFname)]);
            clsJPMEmployeeinfo.blnSalesStaff = Comm.ToInt32(sArrEmp[GetEnumEmp(EmpIndexes.blnSalesStaff)]);
            clsJPMEmployeeinfo.PhotoPath = Convert.ToString(sArrEmp[GetEnumEmp(EmpIndexes.PhotoPath)]);
            clsJPMEmployeeinfo.InsCompany = Convert.ToString(sArrEmp[GetEnumEmp(EmpIndexes.InsCompany)]);
            clsJPMEmployeeinfo.CommissionCondition = Comm.ToInt32(sArrEmp[GetEnumEmp(EmpIndexes.CommissionCondition)]);
            clsJPMEmployeeinfo.EmpCode = Convert.ToString(sArrEmp[GetEnumEmp(EmpIndexes.EmpCode)]);
            clsJPMEmployeeinfo.blnStatus = Comm.ToInt32(sArrEmp[GetEnumEmp(EmpIndexes.blnStatus)]);
            clsJPMEmployeeinfo.DrivingLicenceNo = Convert.ToString(sArrEmp[GetEnumEmp(EmpIndexes.DrivingLicenceNo)]);
            clsJPMEmployeeinfo.DrivingLicenceExpiry = Convert.ToDateTime(sArrEmp[GetEnumEmp(EmpIndexes.DrivingLicenceExpiry)]);
            clsJPMEmployeeinfo.PassportNo = Convert.ToString(sArrEmp[GetEnumEmp(EmpIndexes.PassportNo)]);
            clsJPMEmployeeinfo.PassportExpiry = Convert.ToDateTime(sArrEmp[GetEnumEmp(EmpIndexes.PassportExpiry)]);
            clsJPMEmployeeinfo.Active = Comm.ToInt32(sArrEmp[GetEnumEmp(EmpIndexes.Active)]);
            clsJPMEmployeeinfo.SortOrder = Comm.ToInt32(sArrEmp[GetEnumEmp(EmpIndexes.SortOrder)]);
            clsJPMEmployeeinfo.EnrollNo = Comm.ToInt32(sArrEmp[GetEnumEmp(EmpIndexes.EnrollNo)]);
            clsJPMEmployeeinfo.TargetAmount = Comm.ToDecimal(sArrEmp[GetEnumEmp(EmpIndexes.TargetAmount)]);
            clsJPMEmployeeinfo.IncentivePer = Comm.ToDecimal(sArrEmp[GetEnumEmp(EmpIndexes.IncentivePer)]);
            clsJPMEmployeeinfo.PWD = Convert.ToString(sArrEmp[GetEnumEmp(EmpIndexes.PWD)]);
            clsJPMEmployeeinfo.Holidays = Convert.ToString(sArrEmp[GetEnumEmp(EmpIndexes.Holidays)]);
            clsJPMEmployeeinfo.LID = Comm.ToInt32(sArrEmp[GetEnumEmp(EmpIndexes.LID)]);
            clsJPMEmployeeinfo.salarypermonth = Comm.ToDecimal(sArrEmp[GetEnumEmp(EmpIndexes.salarypermonth)]);
            //Dipu 21-03-2022 ------- >>
            clsJPMEmployeeinfo.TenantID = Global.gblTenantID;
            clsPM.clsJsonPMEmployeeInfo_ = clsJPMEmployeeinfo;

            #endregion

            #region "Repacking Details (tblRepackingItem) -------------------------- >>"
            DataTable dtBatchUniq = new DataTable();
            List<clsJsonPDetailsInfo> lstJPDinfo = new List<clsJsonPDetailsInfo>();

            for (int i = 0; i < dgvStockIn.Rows.Count; i++)
            {
                if (dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.CItemCode)].Value != null)
                {
                    if (dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.CItemCode)].Value.ToString() != "")
                    {
                        clsJPDinfo = new clsJsonPDetailsInfo();

                        if (dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cBarCode)].Value.ToString().ToUpper() == "<AUTO BARCODE>")
                            dtBatchUniq = Comm.fnGetData("EXEC UspGetBatchCodeWhenAutoBarcode " + Comm.ToDecimal(dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cItemID)].Value) + ",'" + dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cBarCode)].Value.ToString() + "',''," + Comm.ToDecimal(dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cMRP)].Value) + ",'" + Convert.ToDateTime(dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.CExpiry)].Value).ToString("dd-MMM-yyyy") + "'," + Global.gblTenantID + "").Tables[0];

                        //clsJPDinfo.InvID = Comm.ToDecimal(txtInvAutoNo.Text);
                        clsJPDinfo.InvID = Comm.ToDecimal(txtInvAutoNo.Tag);
                        clsJPDinfo.ItemId = Comm.ToDecimal(dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cItemID)].Value);
                        clsJPDinfo.Qty = Comm.ToDecimal(dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cQty)].Value);
                        clsJPDinfo.Rate = Comm.ToDecimal(dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cCrate)].Value);
                        clsJPDinfo.UnitId = Comm.ToDecimal(dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.CUnit)].Tag);
                        clsJPDinfo.Batch = "";
                        clsJPDinfo.TaxPer = Comm.ToDecimal(dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.ctaxPer)].Value);
                        clsJPDinfo.TaxAmount = Comm.ToDecimal(dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.ctax)].Value);
                        clsJPDinfo.Discount = Comm.ToDecimal(dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cDiscAmount)].Value);
                        clsJPDinfo.MRP = Comm.ToDecimal(dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cMRP)].Value);
                        clsJPDinfo.SlNo = Comm.ToDecimal(dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cSlNo)].Value);
                        clsJPDinfo.Prate = Comm.ToDecimal(dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cPrate)].Value);
                        clsJPDinfo.Free = Comm.ToDecimal(dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cFree)].Value);
                        clsJPDinfo.SerialNos = "";
                        clsJPDinfo.ItemDiscount = Comm.ToDecimal(dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cDiscAmount)].Value);

                        if (dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cBarCode)].Value != null)
                        {
                            if (dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cBarCode)].Value.ToString() != "0")
                                clsJPDinfo.BatchCode = dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cBarCode)].Value.ToString();
                            else
                                clsJPDinfo.BatchCode = dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cBarCode)].Tag.ToString();
                        }
                        else
                            clsJPDinfo.BatchCode = dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cBarCode)].Tag.ToString();

                        clsJPDinfo.iCessOnTax = 0;
                        clsJPDinfo.blnCessOnTax = 0;
                        clsJPDinfo.Expiry = Convert.ToDateTime(dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.CExpiry)].Value);
                        clsJPDinfo.ItemDiscountPer = Comm.ToDecimal(dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cDiscPer)].Value);
                        clsJPDinfo.RateInclusive = Comm.ToDecimal(dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cRateinclusive)].Value);
                        clsJPDinfo.ITaxableAmount = Comm.ToDecimal(dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.ctaxable)].Value);
                        clsJPDinfo.INetAmount = Comm.ToDecimal(dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cNetAmount)].Value);
                        clsJPDinfo.CGSTTaxPer = Comm.ToDecimal(dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cCGST)].Tag);
                        clsJPDinfo.CGSTTaxAmt = Comm.ToDecimal(dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cCGST)].Value);
                        clsJPDinfo.SGSTTaxPer = Comm.ToDecimal(dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cSGST)].Tag);
                        clsJPDinfo.SGSTTaxAmt = Comm.ToDecimal(dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cSGST)].Value);
                        clsJPDinfo.IGSTTaxPer = Comm.ToDecimal(dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cIGST)].Tag);
                        clsJPDinfo.IGSTTaxAmt = Comm.ToDecimal(dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cIGST)].Value);
                        clsJPDinfo.iRateDiscPer = Comm.ToDecimal(dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cDiscPer)].Value);
                        clsJPDinfo.iRateDiscount = Comm.ToDecimal(dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cDiscAmount)].Value);

                        string[] strBatchUniq;
                        //clsJPDinfo.BatchUnique = dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cBarCode)].Value.ToString();
                        if (dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cBarCode)].Value.ToString().ToUpper() == "<AUTO BARCODE>")
                        {
                            if (dtBatchUniq.Rows.Count > 0)
                                clsJPDinfo.BatchUnique = dtBatchUniq.Rows[0]["BatchUniq"].ToString();
                            else
                                clsJPDinfo.BatchUnique = dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cBarCode)].Value.ToString();
                        }
                        else
                        {
                            strBatchUniq = dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cBarCode)].Value.ToString().Split('@');
                            if (strBatchUniq.Length > 0)
                            {
                                if (strBatchUniq.Length == 2)
                                {
                                    if (Comm.ToDecimal(dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cMRP)].Value) != Comm.ToDecimal(strBatchUniq[1].ToString()))
                                    {
                                        clsJPDinfo.BatchUnique = strBatchUniq[0].ToString() + "@" + Comm.ToDecimal(dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cMRP)].Value).ToString(AppSettings.CurrDecimalFormat);
                                    }
                                    else
                                        clsJPDinfo.BatchUnique = dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cBarCode)].Value.ToString();
                                }
                                else if (strBatchUniq.Length == 3)
                                {
                                    if (Comm.ToDecimal(dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cMRP)].Value) != Comm.ToDecimal(strBatchUniq[1].ToString()))
                                    {
                                        clsJPDinfo.BatchUnique = strBatchUniq[0].ToString() + "@" + Comm.ToDecimal(dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cMRP)].Value).ToString(AppSettings.CurrDecimalFormat) + "@" + Convert.ToDateTime(dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.CExpiry)].Value).ToString("dd-MM-yy").Replace("-", "");
                                    }
                                    else
                                        clsJPDinfo.BatchUnique = dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cBarCode)].Value.ToString();
                                }
                                else
                                    clsJPDinfo.BatchUnique = dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cBarCode)].Value.ToString();
                            }
                            else
                            {
                                clsJPDinfo.BatchUnique = dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cBarCode)].Value.ToString();
                            }
                        }

                        clsJPDinfo.blnQtyIN = 1;
                        clsJPDinfo.CRate = Comm.ToDecimal(dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cCrate)].Value);
                        clsJPDinfo.CRateWithTax = Comm.ToDecimal(dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cCRateWithTax)].Value);
                        clsJPDinfo.Unit = dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.CUnit)].Value.ToString();
                        clsJPDinfo.ItemStockID = 0;
                        clsJPDinfo.IcessPercent = Comm.ToDecimal(dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cCCessPer)].Value);
                        clsJPDinfo.IcessAmt = 0;
                        clsJPDinfo.IQtyCompCessPer = 0;
                        clsJPDinfo.IQtyCompCessAmt = Comm.ToDecimal(dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cCCompCessQty)].Value);
                        clsJPDinfo.StockMRP = 0;
                        clsJPDinfo.InonTaxableAmount = 0;
                        clsJPDinfo.IAgentCommPercent = 0;
                        clsJPDinfo.BlnDelete = 0;
                        clsJPDinfo.Id = Comm.ToDecimal(dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cID)].Value);
                        clsJPDinfo.StrOfferDetails = "";
                        clsJPDinfo.BlnOfferItem = 0;
                        clsJPDinfo.BalQty = 0;
                        clsJPDinfo.GrossAmount = Comm.ToDecimal(dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cGrossAmt)].Value);
                        clsJPDinfo.iFloodCessPer = Comm.ToDecimal(dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cFloodCessPer)].Value);
                        clsJPDinfo.iFloodCessAmt = Comm.ToDecimal(dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cFloodCessAmt)].Value);
                        clsJPDinfo.Srate1 = Comm.ToDecimal(dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cSRate1)].Value);
                        clsJPDinfo.Srate2 = Comm.ToDecimal(dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cSRate2)].Value);
                        clsJPDinfo.Srate3 = Comm.ToDecimal(dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cSRate3)].Value);
                        clsJPDinfo.Srate4 = Comm.ToDecimal(dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cSRate4)].Value);
                        clsJPDinfo.Srate5 = Comm.ToDecimal(dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cSRate5)].Value);
                        clsJPDinfo.Costrate = Comm.ToDecimal(dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cCrate)].Value);
                        clsJPDinfo.CostValue = 0;
                        clsJPDinfo.Profit = 0;
                        clsJPDinfo.ProfitPer = 0;
                        clsJPDinfo.DiscMode = 0;
                        clsJPDinfo.Srate1Per = Comm.ToDecimal(dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cSRate1Per)].Value);
                        clsJPDinfo.Srate2Per = Comm.ToDecimal(dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cSRate2Per)].Value);
                        clsJPDinfo.Srate3Per = Comm.ToDecimal(dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cSRate3Per)].Value);
                        clsJPDinfo.Srate4Per = Comm.ToDecimal(dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cSRate4Per)].Value);
                        clsJPDinfo.Srate5Per = Comm.ToDecimal(dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cSRate5Per)].Value);
                        lstJPDinfo.Add(clsJPDinfo);
                    }
                }
            }

            for (int i = 0; i < dgvStockOut.Rows.Count; i++)
            {
                if (dgvStockOut.Rows[i].Cells[GetEnum(GridColIndexes.CItemCode)].Value != null)
                {
                    if (dgvStockOut.Rows[i].Cells[GetEnum(GridColIndexes.CItemCode)].Value.ToString() != "")
                    {
                        clsJPDinfo = new clsJsonPDetailsInfo();

                        if (dgvStockOut.Rows[i].Cells[GetEnum(GridColIndexes.cBarCode)].Value.ToString().ToUpper() == "<AUTO BARCODE>")
                            dtBatchUniq = Comm.fnGetData("EXEC UspGetBatchCodeWhenAutoBarcode " + Comm.ToDecimal(dgvStockOut.Rows[i].Cells[GetEnum(GridColIndexes.cItemID)].Value) + ",'" + dgvStockOut.Rows[i].Cells[GetEnum(GridColIndexes.cBarCode)].Value.ToString() + "',''," + Comm.ToDecimal(dgvStockOut.Rows[i].Cells[GetEnum(GridColIndexes.cMRP)].Value) + ",'" + Convert.ToDateTime(dgvStockOut.Rows[i].Cells[GetEnum(GridColIndexes.CExpiry)].Value).ToString("dd-MMM-yyyy") + "'," + Global.gblTenantID + "").Tables[0];

                        //clsJPDinfo.InvID = Comm.ToDecimal(txtInvAutoNo.Text);
                        clsJPDinfo.InvID = Comm.ToDecimal(txtInvAutoNo.Tag);
                        clsJPDinfo.ItemId = Comm.ToDecimal(dgvStockOut.Rows[i].Cells[GetEnum(GridColIndexes.cItemID)].Value);
                        clsJPDinfo.Qty = Comm.ToDecimal(dgvStockOut.Rows[i].Cells[GetEnum(GridColIndexes.cQty)].Value);
                        clsJPDinfo.Rate = Comm.ToDecimal(dgvStockOut.Rows[i].Cells[GetEnum(GridColIndexes.cCrate)].Value);
                        clsJPDinfo.UnitId = Comm.ToDecimal(dgvStockOut.Rows[i].Cells[GetEnum(GridColIndexes.CUnit)].Tag);
                        clsJPDinfo.Batch = "";
                        clsJPDinfo.TaxPer = Comm.ToDecimal(dgvStockOut.Rows[i].Cells[GetEnum(GridColIndexes.ctaxPer)].Value);
                        clsJPDinfo.TaxAmount = Comm.ToDecimal(dgvStockOut.Rows[i].Cells[GetEnum(GridColIndexes.ctax)].Value);
                        clsJPDinfo.Discount = Comm.ToDecimal(dgvStockOut.Rows[i].Cells[GetEnum(GridColIndexes.cDiscAmount)].Value);
                        clsJPDinfo.MRP = Comm.ToDecimal(dgvStockOut.Rows[i].Cells[GetEnum(GridColIndexes.cMRP)].Value);
                        clsJPDinfo.SlNo = Comm.ToDecimal(dgvStockOut.Rows[i].Cells[GetEnum(GridColIndexes.cSlNo)].Value);
                        clsJPDinfo.Prate = Comm.ToDecimal(dgvStockOut.Rows[i].Cells[GetEnum(GridColIndexes.cPrate)].Value);
                        clsJPDinfo.Free = Comm.ToDecimal(dgvStockOut.Rows[i].Cells[GetEnum(GridColIndexes.cFree)].Value);
                        clsJPDinfo.SerialNos = "";
                        clsJPDinfo.ItemDiscount = Comm.ToDecimal(dgvStockOut.Rows[i].Cells[GetEnum(GridColIndexes.cDiscAmount)].Value);

                        if (dgvStockOut.Rows[i].Cells[GetEnum(GridColIndexes.cBarCode)].Value != null)
                        {
                            if (dgvStockOut.Rows[i].Cells[GetEnum(GridColIndexes.cBarCode)].Value.ToString() != "0")
                                clsJPDinfo.BatchCode = dgvStockOut.Rows[i].Cells[GetEnum(GridColIndexes.cBarCode)].Value.ToString();
                            else
                                clsJPDinfo.BatchCode = dgvStockOut.Rows[i].Cells[GetEnum(GridColIndexes.cBarCode)].Tag.ToString();
                        }
                        else
                            clsJPDinfo.BatchCode = dgvStockOut.Rows[i].Cells[GetEnum(GridColIndexes.cBarCode)].Tag.ToString();

                        clsJPDinfo.iCessOnTax = 0;
                        clsJPDinfo.blnCessOnTax = 0;
                        clsJPDinfo.Expiry = Convert.ToDateTime(dgvStockOut.Rows[i].Cells[GetEnum(GridColIndexes.CExpiry)].Value);
                        clsJPDinfo.ItemDiscountPer = Comm.ToDecimal(dgvStockOut.Rows[i].Cells[GetEnum(GridColIndexes.cDiscPer)].Value);
                        clsJPDinfo.RateInclusive = Comm.ToDecimal(dgvStockOut.Rows[i].Cells[GetEnum(GridColIndexes.cRateinclusive)].Value);
                        clsJPDinfo.ITaxableAmount = Comm.ToDecimal(dgvStockOut.Rows[i].Cells[GetEnum(GridColIndexes.ctaxable)].Value);
                        clsJPDinfo.INetAmount = Comm.ToDecimal(dgvStockOut.Rows[i].Cells[GetEnum(GridColIndexes.cNetAmount)].Value);
                        clsJPDinfo.CGSTTaxPer = Comm.ToDecimal(dgvStockOut.Rows[i].Cells[GetEnum(GridColIndexes.cCGST)].Tag);
                        clsJPDinfo.CGSTTaxAmt = Comm.ToDecimal(dgvStockOut.Rows[i].Cells[GetEnum(GridColIndexes.cCGST)].Value);
                        clsJPDinfo.SGSTTaxPer = Comm.ToDecimal(dgvStockOut.Rows[i].Cells[GetEnum(GridColIndexes.cSGST)].Tag);
                        clsJPDinfo.SGSTTaxAmt = Comm.ToDecimal(dgvStockOut.Rows[i].Cells[GetEnum(GridColIndexes.cSGST)].Value);
                        clsJPDinfo.IGSTTaxPer = Comm.ToDecimal(dgvStockOut.Rows[i].Cells[GetEnum(GridColIndexes.cIGST)].Tag);
                        clsJPDinfo.IGSTTaxAmt = Comm.ToDecimal(dgvStockOut.Rows[i].Cells[GetEnum(GridColIndexes.cIGST)].Value);
                        clsJPDinfo.iRateDiscPer = Comm.ToDecimal(dgvStockOut.Rows[i].Cells[GetEnum(GridColIndexes.cDiscPer)].Value);
                        clsJPDinfo.iRateDiscount = Comm.ToDecimal(dgvStockOut.Rows[i].Cells[GetEnum(GridColIndexes.cDiscAmount)].Value);

                        string[] strBatchUniq;
                        //clsJPDinfo.BatchUnique = dgvStockOut.Rows[i].Cells[GetEnum(GridColIndexes.cBarCode)].Value.ToString();
                        if (dgvStockOut.Rows[i].Cells[GetEnum(GridColIndexes.cBarCode)].Value.ToString().ToUpper() == "<AUTO BARCODE>")
                        {
                            if (dtBatchUniq.Rows.Count > 0)
                                clsJPDinfo.BatchUnique = dtBatchUniq.Rows[0]["BatchUniq"].ToString();
                            else
                                clsJPDinfo.BatchUnique = dgvStockOut.Rows[i].Cells[GetEnum(GridColIndexes.cBarCode)].Value.ToString();
                        }
                        else
                        {
                            strBatchUniq = dgvStockOut.Rows[i].Cells[GetEnum(GridColIndexes.cBarCode)].Value.ToString().Split('@');
                            if (strBatchUniq.Length > 0)
                            {
                                if (strBatchUniq.Length == 2)
                                {
                                    if (Comm.ToDecimal(dgvStockOut.Rows[i].Cells[GetEnum(GridColIndexes.cMRP)].Value) != Comm.ToDecimal(strBatchUniq[1].ToString()))
                                    {
                                        clsJPDinfo.BatchUnique = strBatchUniq[0].ToString() + "@" + Comm.ToDecimal(dgvStockOut.Rows[i].Cells[GetEnum(GridColIndexes.cMRP)].Value).ToString(AppSettings.CurrDecimalFormat);
                                    }
                                    else
                                        clsJPDinfo.BatchUnique = dgvStockOut.Rows[i].Cells[GetEnum(GridColIndexes.cBarCode)].Value.ToString();
                                }
                                else if (strBatchUniq.Length == 3)
                                {
                                    if (Comm.ToDecimal(dgvStockOut.Rows[i].Cells[GetEnum(GridColIndexes.cMRP)].Value) != Comm.ToDecimal(strBatchUniq[1].ToString()))
                                    {
                                        clsJPDinfo.BatchUnique = strBatchUniq[0].ToString() + "@" + Comm.ToDecimal(dgvStockOut.Rows[i].Cells[GetEnum(GridColIndexes.cMRP)].Value).ToString(AppSettings.CurrDecimalFormat) + "@" + Convert.ToDateTime(dgvStockOut.Rows[i].Cells[GetEnum(GridColIndexes.CExpiry)].Value).ToString("dd-MM-yy").Replace("-", "");
                                    }
                                    else
                                        clsJPDinfo.BatchUnique = dgvStockOut.Rows[i].Cells[GetEnum(GridColIndexes.cBarCode)].Value.ToString();
                                }
                                else
                                    clsJPDinfo.BatchUnique = dgvStockOut.Rows[i].Cells[GetEnum(GridColIndexes.cBarCode)].Value.ToString();
                            }
                            else
                            {
                                clsJPDinfo.BatchUnique = dgvStockOut.Rows[i].Cells[GetEnum(GridColIndexes.cBarCode)].Value.ToString();
                            }
                        }

                        clsJPDinfo.blnQtyIN = 0;
                        clsJPDinfo.CRate = Comm.ToDecimal(dgvStockOut.Rows[i].Cells[GetEnum(GridColIndexes.cCrate)].Value);
                        clsJPDinfo.CRateWithTax = Comm.ToDecimal(dgvStockOut.Rows[i].Cells[GetEnum(GridColIndexes.cCRateWithTax)].Value);
                        clsJPDinfo.Unit = dgvStockOut.Rows[i].Cells[GetEnum(GridColIndexes.CUnit)].Value.ToString();
                        clsJPDinfo.ItemStockID = 0;
                        clsJPDinfo.IcessPercent = Comm.ToDecimal(dgvStockOut.Rows[i].Cells[GetEnum(GridColIndexes.cCCessPer)].Value);
                        clsJPDinfo.IcessAmt = 0;
                        clsJPDinfo.IQtyCompCessPer = 0;
                        clsJPDinfo.IQtyCompCessAmt = Comm.ToDecimal(dgvStockOut.Rows[i].Cells[GetEnum(GridColIndexes.cCCompCessQty)].Value);
                        clsJPDinfo.StockMRP = 0;
                        clsJPDinfo.InonTaxableAmount = 0;
                        clsJPDinfo.IAgentCommPercent = 0;
                        clsJPDinfo.BlnDelete = 0;
                        clsJPDinfo.Id = Comm.ToDecimal(dgvStockOut.Rows[i].Cells[GetEnum(GridColIndexes.cID)].Value);
                        clsJPDinfo.StrOfferDetails = "";
                        clsJPDinfo.BlnOfferItem = 0;
                        clsJPDinfo.BalQty = 0;
                        clsJPDinfo.GrossAmount = Comm.ToDecimal(dgvStockOut.Rows[i].Cells[GetEnum(GridColIndexes.cGrossAmt)].Value);
                        clsJPDinfo.iFloodCessPer = Comm.ToDecimal(dgvStockOut.Rows[i].Cells[GetEnum(GridColIndexes.cFloodCessPer)].Value);
                        clsJPDinfo.iFloodCessAmt = Comm.ToDecimal(dgvStockOut.Rows[i].Cells[GetEnum(GridColIndexes.cFloodCessAmt)].Value);
                        clsJPDinfo.Srate1 = Comm.ToDecimal(dgvStockOut.Rows[i].Cells[GetEnum(GridColIndexes.cSRate1)].Value);
                        clsJPDinfo.Srate2 = Comm.ToDecimal(dgvStockOut.Rows[i].Cells[GetEnum(GridColIndexes.cSRate2)].Value);
                        clsJPDinfo.Srate3 = Comm.ToDecimal(dgvStockOut.Rows[i].Cells[GetEnum(GridColIndexes.cSRate3)].Value);
                        clsJPDinfo.Srate4 = Comm.ToDecimal(dgvStockOut.Rows[i].Cells[GetEnum(GridColIndexes.cSRate4)].Value);
                        clsJPDinfo.Srate5 = Comm.ToDecimal(dgvStockOut.Rows[i].Cells[GetEnum(GridColIndexes.cSRate5)].Value);
                        clsJPDinfo.Costrate = Comm.ToDecimal(dgvStockOut.Rows[i].Cells[GetEnum(GridColIndexes.cCrate)].Value);
                        clsJPDinfo.CostValue = 0;
                        clsJPDinfo.Profit = 0;
                        clsJPDinfo.ProfitPer = 0;
                        clsJPDinfo.DiscMode = 0;
                        clsJPDinfo.Srate1Per = Comm.ToDecimal(dgvStockOut.Rows[i].Cells[GetEnum(GridColIndexes.cSRate1Per)].Value);
                        clsJPDinfo.Srate2Per = Comm.ToDecimal(dgvStockOut.Rows[i].Cells[GetEnum(GridColIndexes.cSRate2Per)].Value);
                        clsJPDinfo.Srate3Per = Comm.ToDecimal(dgvStockOut.Rows[i].Cells[GetEnum(GridColIndexes.cSRate3Per)].Value);
                        clsJPDinfo.Srate4Per = Comm.ToDecimal(dgvStockOut.Rows[i].Cells[GetEnum(GridColIndexes.cSRate4Per)].Value);
                        clsJPDinfo.Srate5Per = Comm.ToDecimal(dgvStockOut.Rows[i].Cells[GetEnum(GridColIndexes.cSRate5Per)].Value);
                        lstJPDinfo.Add(clsJPDinfo);
                    }
                }
            }

            clsPM.clsJsonPDetailsInfoList_ = lstJPDinfo;

            #endregion

            #region "Item Unit Details ------------------------------------------- >>"

            List<clsJsonPDUnitinfo> lstJPDUnit = new List<clsJsonPDUnitinfo>();
            for (int j = 0; j < dgvStockIn.Rows.Count; j++)
            {
                if (dgvStockIn.Rows[j].Cells[GetEnum(GridColIndexes.CItemCode)].Value != null)
                {
                    if (dgvStockIn.Rows[j].Cells[GetEnum(GridColIndexes.CItemCode)].Value.ToString() != "")
                    {
                        DataTable dtUnit = new DataTable();
                        clsJPDUnitinfo = new clsJsonPDUnitinfo();
                        clsJPDUnitinfo.UnitID = Comm.ToDecimal(dgvStockIn.Rows[j].Cells[GetEnum(GridColIndexes.CUnit)].Tag);
                        clsJPDUnitinfo.UnitName = dgvStockIn.Rows[j].Cells[GetEnum(GridColIndexes.CUnit)].Value.ToString();
                        //dipu on 20-Apr-2022 ----->>
                        dtUnit = Comm.fnGetData("SELECT UnitShortName FROM tblUnit WHERE UnitID = " + Comm.ToDecimal(dgvStockIn.Rows[j].Cells[GetEnum(GridColIndexes.CUnit)].Tag) + "").Tables[0];
                        if (dtUnit.Rows.Count > 0)
                            clsJPDUnitinfo.UnitShortName = dtUnit.Rows[0][0].ToString();
                        else
                            clsJPDUnitinfo.UnitShortName = "";

                        clsJPDUnitinfo.TenantID = Global.gblTenantID;
                        lstJPDUnit.Add(clsJPDUnitinfo);
                    }
                }
            }
            clsPM.clsJsonPDUnitinfoList_ = lstJPDUnit;

            #endregion

            #region "Item Details ------------------------------------------------ >>"

            List<clsJsonPDIteminfo> lstJPDItem = new List<clsJsonPDIteminfo>();
            for (int j = 0; j < dgvStockIn.Rows.Count; j++)
            {
                if (dgvStockIn.Rows[j].Cells[GetEnum(GridColIndexes.CItemCode)].Value != null)
                {
                    if (dgvStockIn.Rows[j].Cells[GetEnum(GridColIndexes.CItemCode)].Value.ToString() != "")
                    {
                        clsJPDIteminfo = new clsJsonPDIteminfo();
                        string[] sArrItm = GetItemDetails(Comm.ToDecimal(dgvStockIn.Rows[j].Cells[GetEnum(GridColIndexes.cItemID)].Value));
                        clsJPDIteminfo.ItemID = Comm.ToDecimal(dgvStockIn.Rows[j].Cells[GetEnum(GridColIndexes.cItemID)].Value);
                        clsJPDIteminfo.ItemCode = sArrItm[GetEnumItem(ItemIndexes.ItemCode)].ToString();
                        clsJPDIteminfo.ItemName = sArrItm[GetEnumItem(ItemIndexes.ItemName)].ToString();
                        clsJPDIteminfo.CategoryID = Comm.ToInt32(sArrItm[GetEnumItem(ItemIndexes.CategoryID)].ToString());
                        clsJPDIteminfo.Description = sArrItm[GetEnumItem(ItemIndexes.Description)].ToString();
                        clsJPDIteminfo.PRate = Comm.ToDecimal(sArrItm[GetEnumItem(ItemIndexes.PRate)].ToString());
                        clsJPDIteminfo.SrateCalcMode = Comm.ToInt32(sArrItm[GetEnumItem(ItemIndexes.SrateCalcMode)].ToString());
                        clsJPDIteminfo.CRateAvg = Comm.ToDecimal(sArrItm[GetEnumItem(ItemIndexes.CRateAvg)].ToString());
                        clsJPDIteminfo.Srate1Per = Comm.ToDecimal(sArrItm[GetEnumItem(ItemIndexes.Srate1Per)].ToString());
                        clsJPDIteminfo.SRate1 = Comm.ToDecimal(sArrItm[GetEnumItem(ItemIndexes.SRate1)].ToString());
                        clsJPDIteminfo.Srate2Per = Comm.ToDecimal(sArrItm[GetEnumItem(ItemIndexes.Srate2Per)].ToString());
                        clsJPDIteminfo.SRate2 = Comm.ToDecimal(sArrItm[GetEnumItem(ItemIndexes.SRate2)].ToString());
                        clsJPDIteminfo.Srate3Per = Comm.ToDecimal(sArrItm[GetEnumItem(ItemIndexes.Srate3Per)].ToString());
                        clsJPDIteminfo.SRate3 = Comm.ToDecimal(sArrItm[GetEnumItem(ItemIndexes.SRate3)].ToString());
                        clsJPDIteminfo.Srate4 = Comm.ToDecimal(sArrItm[GetEnumItem(ItemIndexes.Srate4)].ToString());
                        clsJPDIteminfo.Srate4Per = Comm.ToDecimal(sArrItm[GetEnumItem(ItemIndexes.Srate4Per)].ToString());
                        clsJPDIteminfo.SRate5 = Comm.ToDecimal(sArrItm[GetEnumItem(ItemIndexes.SRate5)].ToString());
                        clsJPDIteminfo.Srate5Per = Comm.ToDecimal(sArrItm[GetEnumItem(ItemIndexes.Srate5Per)].ToString());
                        clsJPDIteminfo.MRP = Comm.ToDecimal(sArrItm[GetEnumItem(ItemIndexes.MRP)].ToString());
                        clsJPDIteminfo.ROL = Comm.ToDecimal(sArrItm[GetEnumItem(ItemIndexes.ROL)].ToString());
                        clsJPDIteminfo.Rack = sArrItm[GetEnumItem(ItemIndexes.Rack)].ToString();
                        clsJPDIteminfo.Manufacturer = sArrItm[GetEnumItem(ItemIndexes.Manufacturer)].ToString();
                        clsJPDIteminfo.ActiveStatus = Comm.ToInt32(sArrItm[GetEnumItem(ItemIndexes.ActiveStatus)].ToString());
                        clsJPDIteminfo.IntLocal = Comm.ToInt32(sArrItm[GetEnumItem(ItemIndexes.IntLocal)].ToString());
                        clsJPDIteminfo.ProductType = sArrItm[GetEnumItem(ItemIndexes.ProductType)].ToString();
                        clsJPDIteminfo.ProductTypeID = Comm.ToDecimal(sArrItm[GetEnumItem(ItemIndexes.ProductTypeID)].ToString());
                        clsJPDIteminfo.LedgerID = Comm.ToDecimal(sArrItm[GetEnumItem(ItemIndexes.LedgerID)].ToString());
                        clsJPDIteminfo.UNITID = Comm.ToDecimal(sArrItm[GetEnumItem(ItemIndexes.UNITID)].ToString());
                        clsJPDIteminfo.Notes = sArrItm[GetEnumItem(ItemIndexes.Notes)].ToString();
                        clsJPDIteminfo.agentCommPer = Comm.ToDecimal(sArrItm[GetEnumItem(ItemIndexes.agentCommPer)].ToString());
                        clsJPDIteminfo.BlnExpiryItem = Comm.ToInt32(sArrItm[GetEnumItem(ItemIndexes.BlnExpiryItem)].ToString());
                        clsJPDIteminfo.Coolie = Comm.ToInt32(sArrItm[GetEnumItem(ItemIndexes.Coolie)].ToString());
                        clsJPDIteminfo.FinishedGoodID = Comm.ToInt32(sArrItm[GetEnumItem(ItemIndexes.FinishedGoodID)].ToString());
                        clsJPDIteminfo.MinRate = Comm.ToDecimal(sArrItm[GetEnumItem(ItemIndexes.MinRate)].ToString());
                        clsJPDIteminfo.MaxRate = Comm.ToDecimal(sArrItm[GetEnumItem(ItemIndexes.MaxRate)].ToString());
                        clsJPDIteminfo.PLUNo = Comm.ToDecimal(sArrItm[GetEnumItem(ItemIndexes.PLUNo)].ToString());
                        clsJPDIteminfo.HSNID = Comm.ToInt32(sArrItm[GetEnumItem(ItemIndexes.HSNID)].ToString());
                        clsJPDIteminfo.iCatDiscPer = Comm.ToDecimal(sArrItm[GetEnumItem(ItemIndexes.iCatDiscPer)].ToString());
                        clsJPDIteminfo.IPGDiscPer = Comm.ToDecimal(sArrItm[GetEnumItem(ItemIndexes.IPGDiscPer)].ToString());
                        clsJPDIteminfo.ImanDiscPer = Comm.ToDecimal(sArrItm[GetEnumItem(ItemIndexes.ImanDiscPer)].ToString());
                        clsJPDIteminfo.ItemNameUniCode = sArrItm[GetEnumItem(ItemIndexes.ItemNameUniCode)].ToString();
                        clsJPDIteminfo.Minqty = Comm.ToDecimal(sArrItm[GetEnumItem(ItemIndexes.Minqty)].ToString());
                        clsJPDIteminfo.MNFID = Comm.ToInt32(sArrItm[GetEnumItem(ItemIndexes.MNFID)].ToString());
                        clsJPDIteminfo.PGID = Comm.ToInt32(sArrItm[GetEnumItem(ItemIndexes.PGID)].ToString());
                        clsJPDIteminfo.PGID = Comm.ToInt32(sArrItm[GetEnumItem(ItemIndexes.PGID)].ToString());
                        clsJPDIteminfo.ItemCodeUniCode = sArrItm[GetEnumItem(ItemIndexes.ItemCodeUniCode)].ToString();
                        clsJPDIteminfo.UPC = sArrItm[GetEnumItem(ItemIndexes.UPC)].ToString();
                        clsJPDIteminfo.BatchMode = sArrItm[GetEnumItem(ItemIndexes.BatchMode)].ToString();
                        clsJPDIteminfo.blnExpiry = Comm.ToInt32(sArrItm[GetEnumItem(ItemIndexes.blnExpiry)].ToString());
                        clsJPDIteminfo.Qty = Comm.ToDecimal(sArrItm[GetEnumItem(ItemIndexes.Qty)].ToString());
                        clsJPDIteminfo.MaxQty = Comm.ToDecimal(sArrItm[GetEnumItem(ItemIndexes.MaxQty)].ToString());
                        clsJPDIteminfo.IntNoOrWeight = Comm.ToInt32(sArrItm[GetEnumItem(ItemIndexes.IntNoOrWeight)].ToString());
                        clsJPDIteminfo.SystemName = Global.gblSystemName;
                        clsJPDIteminfo.UserID = Global.gblUserID;
                        clsJPDIteminfo.LastUpdateDate = DateTime.Today; ;
                        clsJPDIteminfo.LastUpdateTime = DateTime.Now;
                        clsJPDIteminfo.TenantID = Global.gblTenantID;
                        clsJPDIteminfo.blnCessOnTax = Comm.ToInt32(sArrItm[GetEnumItem(ItemIndexes.blnCessOnTax)].ToString());
                        clsJPDIteminfo.CompCessQty = Comm.ToDecimal(sArrItm[GetEnumItem(ItemIndexes.CompCessQty)].ToString());
                        clsJPDIteminfo.CGSTTaxPer = Comm.ToDecimal(sArrItm[GetEnumItem(ItemIndexes.CGSTTaxPer)].ToString());
                        clsJPDIteminfo.SGSTTaxPer = Comm.ToDecimal(sArrItm[GetEnumItem(ItemIndexes.SGSTTaxPer)].ToString());
                        clsJPDIteminfo.IGSTTaxPer = Comm.ToDecimal(sArrItm[GetEnumItem(ItemIndexes.IGSTTaxPer)].ToString());
                        clsJPDIteminfo.CessPer = Comm.ToDecimal(sArrItm[GetEnumItem(ItemIndexes.CessPer)].ToString());
                        clsJPDIteminfo.VAT = Comm.ToDecimal(sArrItm[GetEnumItem(ItemIndexes.VAT)].ToString());
                        clsJPDIteminfo.CategoryIDs = sArrItm[GetEnumItem(ItemIndexes.CategoryIDs)].ToString();
                        clsJPDIteminfo.ColorIDs = sArrItm[GetEnumItem(ItemIndexes.ColorIDs)].ToString();
                        clsJPDIteminfo.SizeIDs = sArrItm[GetEnumItem(ItemIndexes.SizeIDs)].ToString();
                        clsJPDIteminfo.BrandDisPer = Comm.ToDecimal(sArrItm[GetEnumItem(ItemIndexes.BrandDisPer)].ToString());
                        clsJPDIteminfo.DGroupID = Comm.ToInt32(sArrItm[GetEnumItem(ItemIndexes.DGroupID)].ToString());
                        clsJPDIteminfo.DGroupDisPer = Comm.ToDecimal(sArrItm[GetEnumItem(ItemIndexes.DGroupDisPer)].ToString());
                        clsJPDIteminfo.BrandID = Comm.ToInt32(sArrItm[GetEnumItem(ItemIndexes.BrandID)].ToString());
                        clsJPDIteminfo.AltUnitID = Comm.ToDecimal(sArrItm[GetEnumItem(ItemIndexes.AltUnitID)].ToString());
                        clsJPDIteminfo.ConvFactor = Comm.ToDecimal(sArrItm[GetEnumItem(ItemIndexes.ConvFactor)].ToString());
                        clsJPDIteminfo.Shelflife = Comm.ToDecimal(sArrItm[GetEnumItem(ItemIndexes.Shelflife)].ToString());
                        clsJPDIteminfo.SRateInclusive = Comm.ToDecimal(sArrItm[GetEnumItem(ItemIndexes.SRateInclusive)].ToString());
                        clsJPDIteminfo.PRateInclusive = Comm.ToDecimal(sArrItm[GetEnumItem(ItemIndexes.PRateInclusive)].ToString());
                        clsJPDIteminfo.Slabsys = Comm.ToDecimal(sArrItm[GetEnumItem(ItemIndexes.Slabsys)].ToString());
                        clsJPDIteminfo.ParentID = Comm.ToDecimal(sArrItm[GetEnumItem(ItemIndexes.ParentID)].ToString());
                        clsJPDIteminfo.ParentConv = Comm.ToDecimal(sArrItm[GetEnumItem(ItemIndexes.ParentConv)].ToString());
                        clsJPDIteminfo.blnParentEqlRate = Comm.ToInt32(sArrItm[GetEnumItem(ItemIndexes.blnParentEqlRate)].ToString());
                        clsJPDIteminfo.ItmConvType = sArrItm[GetEnumItem(ItemIndexes.ItmConvType)].ToString();
                        clsJPDIteminfo.DiscPer = Comm.ToDecimal(sArrItm[GetEnumItem(ItemIndexes.DiscPer)].ToString());
                        lstJPDItem.Add(clsJPDIteminfo);
                    }
                }
            }
            clsPM.clsJsonPDIteminfoList_ = lstJPDItem;
            #endregion

            return JsonConvert.SerializeObject(clsPM);
        }

        // Cash : 0, Credit: 1, Both: 2, Cash Desk : 3
        //Description : Deserialize the JSon to Controls asper instructions.
        private void DeserializeFromJSon(string sToDeSerialize = "")
        {
            clsJSonRepacking clsRepacking = JsonConvert.DeserializeObject<clsJSonRepacking>(sToDeSerialize);

            txtPrefix.Text = clsVchType.TransactionPrefix;
            txtInvAutoNo.Text = Convert.ToString(clsRepacking.clsJsonPMInfo_.InvNo);
            txtInvAutoNo.Tag = Comm.ToDouble(clsRepacking.clsJsonPMInfo_.InvId);
            txtReferenceAutoNo.Tag = Comm.ToDouble(clsRepacking.clsJsonPMInfo_.AutoNum);
            dtpInvDate.Text = Convert.ToString(clsRepacking.clsJsonPMInfo_.InvDate);
            txtReferencePrefix.Text = clsRepacking.clsJsonPMInfo_.RefNo;
            txtReferenceAutoNo.Text = Convert.ToString(clsRepacking.clsJsonPMInfo_.ReferenceAutoNO);

            Comm.chkChangeValuetoZero(Convert.ToString(clsRepacking.clsJsonPMInfo_.Discount));

            cboCostCentre.SelectedValue = clsRepacking.clsJsonPMCCentreInfo_.CCID;

            dSupplierID = 100;
            FillSupplierForSerializeJsonUsingID(Comm.ToInt32(dSupplierID));

            DataTable dtGetPurDetail = clsRepacking.clsJsonPDetailsInfoList_.ToDataTable();
            DataTable dtItemFrmJson = clsRepacking.clsJsonPDIteminfoList_.ToDataTable();
            DataTable dtUnitFrmJson = clsRepacking.clsJsonPDUnitinfoList_.ToDataTable();
            if (dtGetPurDetail.Rows.Count > 0)
            {
                sqlControl rs = new sqlControl();

                AddColumnsToGrid(dgvStockIn);
                AddColumnsToGrid(dgvStockOut);
                for (int i = 0; i < dtGetPurDetail.Rows.Count; i++)
                {
                    dgvStockIn.Rows.Add();

                    rs.Open("Select ItemCode,ItemName From tblItemMaster Where ItemID=" + dtGetPurDetail.Rows[i]["ItemId"].ToString());
                    if (!rs.eof())
                    {
                        dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.CItemCode)].Value = rs.fields("itemcode");
                        dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.CItemName)].Value = rs.fields("ItemName");
                    }

                    SetValue(GetEnum(GridColIndexes.cSlNo), i, (i + 1).ToString());
                    dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cID)].Value = dtGetPurDetail.Rows[i]["Id"].ToString();
                    dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.CUnit)].Value = dtUnitFrmJson.Rows[i]["UnitName"].ToString();
                    dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.CUnit)].Tag = dtGetPurDetail.Rows[i]["UnitId"].ToString();
                    dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cBarCode)].Tag = dtGetPurDetail.Rows[i]["BatchCode"].ToString();
                    dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cBarCode)].Value = dtGetPurDetail.Rows[i]["BatchUnique"].ToString();
                    dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.CExpiry)].Value = Convert.ToDateTime(dtGetPurDetail.Rows[i]["Expiry"]).ToString("dd-MMM-yyyy");
                    dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cMRP)].Value = Comm.FormatValue(Comm.ToDouble(dtGetPurDetail.Rows[i]["MRP"].ToString()), true);
                    dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cPrate)].Value = Comm.FormatValue(Comm.ToDouble(dtGetPurDetail.Rows[i]["Prate"].ToString()), true);
                    this.dgvStockIn.Columns[GetEnum(GridColIndexes.cQty)].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cQty)].Value = Comm.FormatValue(Comm.ToDouble(dtGetPurDetail.Rows[i]["Qty"].ToString()), false);
                    this.dgvStockIn.Columns[GetEnum(GridColIndexes.cFree)].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cFree)].Value = Comm.FormatValue(Comm.ToDouble(dtGetPurDetail.Rows[i]["Free"].ToString()), false);

                    dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cSRate1Per)].Value = Comm.FormatValue(Comm.ToDouble(dtGetPurDetail.Rows[i]["Srate1Per"].ToString()), true);
                    dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cSRate1)].Value = Comm.FormatValue(Comm.ToDouble(dtGetPurDetail.Rows[i]["Srate1"].ToString()), true);
                    dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cSRate2Per)].Value = Comm.FormatValue(Comm.ToDouble(dtGetPurDetail.Rows[i]["Srate2Per"].ToString()), true);
                    dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cSRate2)].Value = Comm.FormatValue(Comm.ToDouble(dtGetPurDetail.Rows[i]["Srate2"].ToString()), true);
                    dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cSRate3Per)].Value = Comm.FormatValue(Comm.ToDouble(dtGetPurDetail.Rows[i]["Srate3Per"].ToString()), true);
                    dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cSRate3)].Value = Comm.FormatValue(Comm.ToDouble(dtGetPurDetail.Rows[i]["Srate3"].ToString()), true);
                    dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cSRate4Per)].Value = Comm.FormatValue(Comm.ToDouble(dtGetPurDetail.Rows[i]["Srate4Per"].ToString()), true);
                    dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cSRate4)].Value = Comm.FormatValue(Comm.ToDouble(dtGetPurDetail.Rows[i]["Srate4"].ToString()), true);
                    dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cSRate5Per)].Value = Comm.FormatValue(Comm.ToDouble(dtGetPurDetail.Rows[i]["Srate5Per"].ToString()), true);
                    dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cSRate5)].Value = Comm.FormatValue(Comm.ToDouble(dtGetPurDetail.Rows[i]["Srate5"].ToString()), true);

                    dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cGrossAmt)].Value = Comm.FormatValue(Comm.ToDouble(dtGetPurDetail.Rows[i]["GrossAmount"].ToString()), true);
                    dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cDiscPer)].Value = Comm.FormatValue(Comm.ToDouble(dtGetPurDetail.Rows[i]["ItemDiscountPer"].ToString()), true);
                    this.dgvStockIn.Columns[GetEnum(GridColIndexes.cDiscPer)].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cDiscAmount)].Value = Comm.FormatValue(Comm.ToDouble(dtGetPurDetail.Rows[i]["ItemDiscount"].ToString()), true);
                    dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cBillDisc)].Value = Comm.FormatValue(Comm.ToDouble(dtGetPurDetail.Rows[i]["Discount"].ToString()), true);
                    dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cCrate)].Value = Comm.FormatValue(Comm.ToDouble(dtGetPurDetail.Rows[i]["CRate"].ToString()), true);
                    dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cItemID)].Value = dtGetPurDetail.Rows[i]["ItemId"].ToString();
                    dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.ctaxPer)].Value = Comm.FormatValue(Comm.ToDouble(dtGetPurDetail.Rows[i]["TaxPer"].ToString()), true);
                    this.dgvStockIn.Columns[GetEnum(GridColIndexes.ctaxPer)].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.ctax)].Value = Comm.FormatValue(Comm.ToDouble(dtGetPurDetail.Rows[i]["TaxAmount"].ToString()), true);
                    dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cIGST)].Tag = Comm.FormatValue(Comm.ToDouble(dtGetPurDetail.Rows[i]["IGSTTaxPer"].ToString()), true);
                    dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cIGST)].Value = Comm.FormatValue(Comm.ToDouble(dtGetPurDetail.Rows[i]["IGSTTaxAmt"].ToString()), true);
                    dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cSGST)].Tag = Comm.FormatValue(Comm.ToDouble(dtGetPurDetail.Rows[i]["CGSTTaxPer"].ToString()), true);
                    dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cSGST)].Value = Comm.FormatValue(Comm.ToDouble(dtGetPurDetail.Rows[i]["CGSTTaxAmt"].ToString()), true);
                    dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cCGST)].Tag = Comm.FormatValue(Comm.ToDouble(dtGetPurDetail.Rows[i]["SGSTTaxPer"].ToString()), true);
                    dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cCGST)].Value = Comm.FormatValue(Comm.ToDouble(dtGetPurDetail.Rows[i]["SGSTTaxAmt"].ToString()), true);
                    dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cNetAmount)].Value = Comm.FormatValue(Comm.ToDouble(dtGetPurDetail.Rows[i]["INetAmount"].ToString()), true);
                    dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cNonTaxable)].Value = Comm.FormatValue(Comm.ToDouble(dtGetPurDetail.Rows[i]["InonTaxableAmount"].ToString()), true);
                    this.dgvStockIn.Columns[GetEnum(GridColIndexes.cNonTaxable)].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cCCessPer)].Value = Comm.FormatValue(Comm.ToDouble(dtGetPurDetail.Rows[i]["IcessPercent"].ToString()), true);
                    this.dgvStockIn.Columns[GetEnum(GridColIndexes.cCCessPer)].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cCCompCessQty)].Value = Comm.FormatValue(Comm.ToDouble(dtGetPurDetail.Rows[i]["IQtyCompCessAmt"].ToString()), true);
                    this.dgvStockIn.Columns[GetEnum(GridColIndexes.cCCompCessQty)].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cFloodCessPer)].Value = Comm.FormatValue(Comm.ToDouble(dtGetPurDetail.Rows[i]["iFloodCessPer"].ToString()), true);
                    this.dgvStockIn.Columns[GetEnum(GridColIndexes.cFloodCessPer)].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cFloodCessAmt)].Value = Comm.FormatValue(Comm.ToDouble(dtGetPurDetail.Rows[i]["iFloodCessAmt"].ToString()), true);
                    this.dgvStockIn.Columns[GetEnum(GridColIndexes.cFloodCessAmt)].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cStockMRP)].Value = Comm.FormatValue(Comm.ToDouble(dtGetPurDetail.Rows[i]["StockMRP"].ToString()), true);
                    this.dgvStockIn.Columns[GetEnum(GridColIndexes.cStockMRP)].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cAgentCommPer)].Value = Comm.FormatValue(Comm.ToDouble(dtGetPurDetail.Rows[i]["IAgentCommPercent"].ToString()), true);
                    this.dgvStockIn.Columns[GetEnum(GridColIndexes.cAgentCommPer)].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cBlnOfferItem)].Value = dtGetPurDetail.Rows[i]["BlnOfferItem"].ToString();
                    dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cStrOfferDetails)].Value = dtGetPurDetail.Rows[i]["StrOfferDetails"].ToString();
                    dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cBatchMode)].Value = dtItemFrmJson.Rows[i]["BatchMode"].ToString();

                    dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cCoolie)].Value = dtItemFrmJson.Rows[i]["Coolie"].ToString();

                    if (Comm.ToDouble(dtGetPurDetail.Rows[i]["RateInclusive"].ToString()) == 1)
                        dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cRateinclusive)].Value = true;
                    else
                        dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cRateinclusive)].Value = false;

                    this.dgvStockIn.Columns[i].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                }

                CalcTotal();
                CalcTotalOut();
            }
        }

        private void LoadBill(int iSelectedID)
        {
            try
            {

                sqlControl rs = new sqlControl();

                rs.Open("Select s.*,si.*,i.itemname,i.itemcode,i.BatchMode,i.Coolie,l.lid,l.lname,l.laliasname,l.TaxNo,l.Address,UnitName From tblLedger as l, tblRepackingItem as si, tblItemMAster as i, tblRepacking as s, tblUnit as u Where si.itemid = i.itemid and si.unitid = u.unitid and s.LedgerID = l.LID and s.InvID = si.InvID and s.InvID = " + iSelectedID + " order by slno ");

                if (rs.eof() == false)
                {
                    iIDFromEditWindow = iSelectedID;

                    if (rs.fields("Cancelled") == "1")
                    {
                        MessageBox.Show("This is an archived bill.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                    txtPrefix.Text = clsVchType.TransactionPrefix;
                    txtInvAutoNo.Text = Convert.ToString(rs.fields("InvNo"));
                    txtInvAutoNo.Tag = Comm.ToDouble(rs.fields("InvId"));
                    txtReferenceAutoNo.Tag = Comm.ToDouble(rs.fields("AutoNum"));
                    dtpInvDate.Text = Convert.ToString(rs.fields("InvDate"));
                    txtReferencePrefix.Text = rs.fields("RefNo");
                    txtReferenceAutoNo.Text = Convert.ToString(rs.fields("ReferenceAutoNO"));

                    Comm.chkChangeValuetoZero(Convert.ToString(rs.fields("Discount")));

                    cboCostCentre.SelectedValue = rs.fields("CCID");

                    dSupplierID = Comm.ToDecimal(rs.fields("LID"));
                    FillSupplierForSerializeJsonUsingID(Comm.ToInt32(dSupplierID));
                }

                AddColumnsToGrid(dgvStockIn);
                AddColumnsToGrid(dgvStockOut);
                int i = 0;
                int j = 0;
                while (rs.eof() == false)
                {
                    if (rs.fields("blnQtyIN").ToString() == "1")
                    {
                        dgvStockIn.Rows.Add();

                        dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.CItemCode)].Value = rs.fields("itemcode");
                        dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.CItemName)].Value = rs.fields("ItemName");

                        SetValue(GetEnum(GridColIndexes.cSlNo), i, (i + 1).ToString());
                        dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cID)].Value = rs.fields("Id").ToString();
                        dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.CUnit)].Value = rs.fields("UnitName").ToString();
                        dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.CUnit)].Tag = rs.fields("UnitId").ToString();
                        dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cBarCode)].Tag = rs.fields("BatchCode").ToString();
                        dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cBarCode)].Value = rs.fields("BatchUnique").ToString();
                        dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.CExpiry)].Value = Convert.ToDateTime(rs.fields("Expiry")).ToString("dd-MMM-yyyy");
                        dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cMRP)].Value = Comm.FormatValue(Comm.ToDouble(rs.fields("MRP").ToString()), true);
                        dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cPrate)].Value = Comm.FormatValue(Comm.ToDouble(rs.fields("Prate").ToString()), true);
                        this.dgvStockIn.Columns[GetEnum(GridColIndexes.cQty)].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                        dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cQty)].Value = Comm.FormatValue(Comm.ToDouble(rs.fields("Qty").ToString()), false);
                        this.dgvStockIn.Columns[GetEnum(GridColIndexes.cFree)].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                        dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cFree)].Value = Comm.FormatValue(Comm.ToDouble(rs.fields("Free").ToString()), false);

                        dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cSRate1Per)].Value = Comm.FormatValue(Comm.ToDouble(rs.fields("Srate1Per").ToString()), true);
                        dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cSRate1)].Value = Comm.FormatValue(Comm.ToDouble(rs.fields("Srate1").ToString()), true);
                        dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cSRate2Per)].Value = Comm.FormatValue(Comm.ToDouble(rs.fields("Srate2Per").ToString()), true);
                        dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cSRate2)].Value = Comm.FormatValue(Comm.ToDouble(rs.fields("Srate2").ToString()), true);
                        dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cSRate3Per)].Value = Comm.FormatValue(Comm.ToDouble(rs.fields("Srate3Per").ToString()), true);
                        dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cSRate3)].Value = Comm.FormatValue(Comm.ToDouble(rs.fields("Srate3").ToString()), true);
                        dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cSRate4Per)].Value = Comm.FormatValue(Comm.ToDouble(rs.fields("Srate4Per").ToString()), true);
                        dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cSRate4)].Value = Comm.FormatValue(Comm.ToDouble(rs.fields("Srate4").ToString()), true);
                        dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cSRate5Per)].Value = Comm.FormatValue(Comm.ToDouble(rs.fields("Srate5Per").ToString()), true);
                        dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cSRate5)].Value = Comm.FormatValue(Comm.ToDouble(rs.fields("Srate5").ToString()), true);

                        dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cGrossAmt)].Value = Comm.FormatValue(Comm.ToDouble(rs.fields("GrossAmount").ToString()), true);
                        dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cDiscPer)].Value = Comm.FormatValue(Comm.ToDouble(rs.fields("ItemDiscountPer").ToString()), true);
                        this.dgvStockIn.Columns[GetEnum(GridColIndexes.cDiscPer)].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                        dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cDiscAmount)].Value = Comm.FormatValue(Comm.ToDouble(rs.fields("ItemDiscount").ToString()), true);
                        dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cBillDisc)].Value = Comm.FormatValue(Comm.ToDouble(rs.fields("Discount").ToString()), true);
                        dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cCrate)].Value = Comm.FormatValue(Comm.ToDouble(rs.fields("CRate").ToString()), true);
                        dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cItemID)].Value = rs.fields("ItemId").ToString();
                        dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.CItemName)].Tag = rs.fields("ItemId").ToString();
                        dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.ctaxPer)].Value = Comm.FormatValue(Comm.ToDouble(rs.fields("TaxPer").ToString()), true);
                        this.dgvStockIn.Columns[GetEnum(GridColIndexes.ctaxPer)].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                        dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.ctax)].Value = Comm.FormatValue(Comm.ToDouble(rs.fields("TaxAmount").ToString()), true);
                        dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cIGST)].Tag = Comm.FormatValue(Comm.ToDouble(rs.fields("IGSTTaxPer").ToString()), true);
                        dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cIGST)].Value = Comm.FormatValue(Comm.ToDouble(rs.fields("IGSTTaxAmt").ToString()), true);
                        dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cSGST)].Tag = Comm.FormatValue(Comm.ToDouble(rs.fields("CGSTTaxPer").ToString()), true);
                        dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cSGST)].Value = Comm.FormatValue(Comm.ToDouble(rs.fields("CGSTTaxAmt").ToString()), true);
                        dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cCGST)].Tag = Comm.FormatValue(Comm.ToDouble(rs.fields("SGSTTaxPer").ToString()), true);
                        dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cCGST)].Value = Comm.FormatValue(Comm.ToDouble(rs.fields("SGSTTaxAmt").ToString()), true);
                        dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cNetAmount)].Value = Comm.FormatValue(Comm.ToDouble(rs.fields("INetAmount").ToString()), true);
                        dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cNonTaxable)].Value = Comm.FormatValue(Comm.ToDouble(rs.fields("InonTaxableAmount").ToString()), true);
                        this.dgvStockIn.Columns[GetEnum(GridColIndexes.cNonTaxable)].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                        dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cCCessPer)].Value = Comm.FormatValue(Comm.ToDouble(rs.fields("IcessPercent").ToString()), true);
                        this.dgvStockIn.Columns[GetEnum(GridColIndexes.cCCessPer)].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                        dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cCCompCessQty)].Value = Comm.FormatValue(Comm.ToDouble(rs.fields("IQtyCompCessAmt").ToString()), true);
                        this.dgvStockIn.Columns[GetEnum(GridColIndexes.cCCompCessQty)].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                        dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cFloodCessPer)].Value = Comm.FormatValue(Comm.ToDouble(rs.fields("iFloodCessPer").ToString()), true);
                        this.dgvStockIn.Columns[GetEnum(GridColIndexes.cFloodCessPer)].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                        dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cFloodCessAmt)].Value = Comm.FormatValue(Comm.ToDouble(rs.fields("iFloodCessAmt").ToString()), true);
                        this.dgvStockIn.Columns[GetEnum(GridColIndexes.cFloodCessAmt)].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                        dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cStockMRP)].Value = Comm.FormatValue(Comm.ToDouble(rs.fields("StockMRP").ToString()), true);
                        this.dgvStockIn.Columns[GetEnum(GridColIndexes.cStockMRP)].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                        dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cAgentCommPer)].Value = Comm.FormatValue(Comm.ToDouble(rs.fields("IAgentCommPercent").ToString()), true);
                        this.dgvStockIn.Columns[GetEnum(GridColIndexes.cAgentCommPer)].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                        dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cBlnOfferItem)].Value = rs.fields("BlnOfferItem").ToString();
                        dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cStrOfferDetails)].Value = rs.fields("StrOfferDetails").ToString();
                        dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cBatchMode)].Value = rs.fields("BatchMode").ToString();

                        dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cCoolie)].Value = rs.fields("Coolie").ToString();

                        if (Comm.ToDouble(rs.fields("RateInclusive").ToString()) == 1)
                            dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cRateinclusive)].Value = true;
                        else
                            dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cRateinclusive)].Value = false;

                        i++;
                    }
                    else
                    {
                        {
                            dgvStockOut.Rows.Add();

                            dgvStockOut.Rows[j].Cells[GetEnum(GridColIndexes.CItemCode)].Value = rs.fields("itemcode");
                            dgvStockOut.Rows[j].Cells[GetEnum(GridColIndexes.CItemName)].Value = rs.fields("ItemName");

                            SetValueOut(GetEnum(GridColIndexes.cSlNo), j, (j + 1).ToString());
                            dgvStockOut.Rows[j].Cells[GetEnum(GridColIndexes.cID)].Value = rs.fields("Id").ToString();
                            dgvStockOut.Rows[j].Cells[GetEnum(GridColIndexes.CUnit)].Value = rs.fields("UnitName").ToString();
                            dgvStockOut.Rows[j].Cells[GetEnum(GridColIndexes.CUnit)].Tag = rs.fields("UnitId").ToString();
                            dgvStockOut.Rows[j].Cells[GetEnum(GridColIndexes.cBarCode)].Tag = rs.fields("BatchCode").ToString();
                            dgvStockOut.Rows[j].Cells[GetEnum(GridColIndexes.cBarCode)].Value = rs.fields("BatchUnique").ToString();
                            dgvStockOut.Rows[j].Cells[GetEnum(GridColIndexes.CExpiry)].Value = Convert.ToDateTime(rs.fields("Expiry")).ToString("dd-MMM-yyyy");
                            dgvStockOut.Rows[j].Cells[GetEnum(GridColIndexes.cMRP)].Value = Comm.FormatValue(Comm.ToDouble(rs.fields("MRP").ToString()), true);
                            dgvStockOut.Rows[j].Cells[GetEnum(GridColIndexes.cPrate)].Value = Comm.FormatValue(Comm.ToDouble(rs.fields("Prate").ToString()), true);
                            this.dgvStockOut.Columns[GetEnum(GridColIndexes.cQty)].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                            dgvStockOut.Rows[j].Cells[GetEnum(GridColIndexes.cQty)].Value = Comm.FormatValue(Comm.ToDouble(rs.fields("Qty").ToString()), false);
                            this.dgvStockOut.Columns[GetEnum(GridColIndexes.cFree)].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                            dgvStockOut.Rows[j].Cells[GetEnum(GridColIndexes.cFree)].Value = Comm.FormatValue(Comm.ToDouble(rs.fields("Free").ToString()), false);

                            dgvStockOut.Rows[j].Cells[GetEnum(GridColIndexes.cSRate1Per)].Value = Comm.FormatValue(Comm.ToDouble(rs.fields("Srate1Per").ToString()), true);
                            dgvStockOut.Rows[j].Cells[GetEnum(GridColIndexes.cSRate1)].Value = Comm.FormatValue(Comm.ToDouble(rs.fields("Srate1").ToString()), true);
                            dgvStockOut.Rows[j].Cells[GetEnum(GridColIndexes.cSRate2Per)].Value = Comm.FormatValue(Comm.ToDouble(rs.fields("Srate2Per").ToString()), true);
                            dgvStockOut.Rows[j].Cells[GetEnum(GridColIndexes.cSRate2)].Value = Comm.FormatValue(Comm.ToDouble(rs.fields("Srate2").ToString()), true);
                            dgvStockOut.Rows[j].Cells[GetEnum(GridColIndexes.cSRate3Per)].Value = Comm.FormatValue(Comm.ToDouble(rs.fields("Srate3Per").ToString()), true);
                            dgvStockOut.Rows[j].Cells[GetEnum(GridColIndexes.cSRate3)].Value = Comm.FormatValue(Comm.ToDouble(rs.fields("Srate3").ToString()), true);
                            dgvStockOut.Rows[j].Cells[GetEnum(GridColIndexes.cSRate4Per)].Value = Comm.FormatValue(Comm.ToDouble(rs.fields("Srate4Per").ToString()), true);
                            dgvStockOut.Rows[j].Cells[GetEnum(GridColIndexes.cSRate4)].Value = Comm.FormatValue(Comm.ToDouble(rs.fields("Srate4").ToString()), true);
                            dgvStockOut.Rows[j].Cells[GetEnum(GridColIndexes.cSRate5Per)].Value = Comm.FormatValue(Comm.ToDouble(rs.fields("Srate5Per").ToString()), true);
                            dgvStockOut.Rows[j].Cells[GetEnum(GridColIndexes.cSRate5)].Value = Comm.FormatValue(Comm.ToDouble(rs.fields("Srate5").ToString()), true);

                            dgvStockOut.Rows[j].Cells[GetEnum(GridColIndexes.cGrossAmt)].Value = Comm.FormatValue(Comm.ToDouble(rs.fields("GrossAmount").ToString()), true);
                            dgvStockOut.Rows[j].Cells[GetEnum(GridColIndexes.cDiscPer)].Value = Comm.FormatValue(Comm.ToDouble(rs.fields("ItemDiscountPer").ToString()), true);
                            this.dgvStockOut.Columns[GetEnum(GridColIndexes.cDiscPer)].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                            dgvStockOut.Rows[j].Cells[GetEnum(GridColIndexes.cDiscAmount)].Value = Comm.FormatValue(Comm.ToDouble(rs.fields("ItemDiscount").ToString()), true);
                            dgvStockOut.Rows[j].Cells[GetEnum(GridColIndexes.cBillDisc)].Value = Comm.FormatValue(Comm.ToDouble(rs.fields("Discount").ToString()), true);
                            dgvStockOut.Rows[j].Cells[GetEnum(GridColIndexes.cCrate)].Value = Comm.FormatValue(Comm.ToDouble(rs.fields("CRate").ToString()), true);
                            dgvStockOut.Rows[j].Cells[GetEnum(GridColIndexes.cItemID)].Value = rs.fields("ItemId").ToString();
                            dgvStockOut.Rows[j].Cells[GetEnum(GridColIndexes.CItemName)].Tag = rs.fields("ItemId").ToString();
                            dgvStockOut.Rows[j].Cells[GetEnum(GridColIndexes.ctaxPer)].Value = Comm.FormatValue(Comm.ToDouble(rs.fields("TaxPer").ToString()), true);
                            this.dgvStockOut.Columns[GetEnum(GridColIndexes.ctaxPer)].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                            dgvStockOut.Rows[j].Cells[GetEnum(GridColIndexes.ctax)].Value = Comm.FormatValue(Comm.ToDouble(rs.fields("TaxAmount").ToString()), true);
                            dgvStockOut.Rows[j].Cells[GetEnum(GridColIndexes.cIGST)].Tag = Comm.FormatValue(Comm.ToDouble(rs.fields("IGSTTaxPer").ToString()), true);
                            dgvStockOut.Rows[j].Cells[GetEnum(GridColIndexes.cIGST)].Value = Comm.FormatValue(Comm.ToDouble(rs.fields("IGSTTaxAmt").ToString()), true);
                            dgvStockOut.Rows[j].Cells[GetEnum(GridColIndexes.cSGST)].Tag = Comm.FormatValue(Comm.ToDouble(rs.fields("CGSTTaxPer").ToString()), true);
                            dgvStockOut.Rows[j].Cells[GetEnum(GridColIndexes.cSGST)].Value = Comm.FormatValue(Comm.ToDouble(rs.fields("CGSTTaxAmt").ToString()), true);
                            dgvStockOut.Rows[j].Cells[GetEnum(GridColIndexes.cCGST)].Tag = Comm.FormatValue(Comm.ToDouble(rs.fields("SGSTTaxPer").ToString()), true);
                            dgvStockOut.Rows[j].Cells[GetEnum(GridColIndexes.cCGST)].Value = Comm.FormatValue(Comm.ToDouble(rs.fields("SGSTTaxAmt").ToString()), true);
                            dgvStockOut.Rows[j].Cells[GetEnum(GridColIndexes.cNetAmount)].Value = Comm.FormatValue(Comm.ToDouble(rs.fields("INetAmount").ToString()), true);
                            dgvStockOut.Rows[j].Cells[GetEnum(GridColIndexes.cNonTaxable)].Value = Comm.FormatValue(Comm.ToDouble(rs.fields("InonTaxableAmount").ToString()), true);
                            this.dgvStockOut.Columns[GetEnum(GridColIndexes.cNonTaxable)].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                            dgvStockOut.Rows[j].Cells[GetEnum(GridColIndexes.cCCessPer)].Value = Comm.FormatValue(Comm.ToDouble(rs.fields("IcessPercent").ToString()), true);
                            this.dgvStockOut.Columns[GetEnum(GridColIndexes.cCCessPer)].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                            dgvStockOut.Rows[j].Cells[GetEnum(GridColIndexes.cCCompCessQty)].Value = Comm.FormatValue(Comm.ToDouble(rs.fields("IQtyCompCessAmt").ToString()), true);
                            this.dgvStockOut.Columns[GetEnum(GridColIndexes.cCCompCessQty)].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                            dgvStockOut.Rows[j].Cells[GetEnum(GridColIndexes.cFloodCessPer)].Value = Comm.FormatValue(Comm.ToDouble(rs.fields("iFloodCessPer").ToString()), true);
                            this.dgvStockOut.Columns[GetEnum(GridColIndexes.cFloodCessPer)].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                            dgvStockOut.Rows[j].Cells[GetEnum(GridColIndexes.cFloodCessAmt)].Value = Comm.FormatValue(Comm.ToDouble(rs.fields("iFloodCessAmt").ToString()), true);
                            this.dgvStockOut.Columns[GetEnum(GridColIndexes.cFloodCessAmt)].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                            dgvStockOut.Rows[j].Cells[GetEnum(GridColIndexes.cStockMRP)].Value = Comm.FormatValue(Comm.ToDouble(rs.fields("StockMRP").ToString()), true);
                            this.dgvStockOut.Columns[GetEnum(GridColIndexes.cStockMRP)].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                            dgvStockOut.Rows[j].Cells[GetEnum(GridColIndexes.cAgentCommPer)].Value = Comm.FormatValue(Comm.ToDouble(rs.fields("IAgentCommPercent").ToString()), true);
                            this.dgvStockOut.Columns[GetEnum(GridColIndexes.cAgentCommPer)].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                            dgvStockOut.Rows[j].Cells[GetEnum(GridColIndexes.cBlnOfferItem)].Value = rs.fields("BlnOfferItem").ToString();
                            dgvStockOut.Rows[j].Cells[GetEnum(GridColIndexes.cStrOfferDetails)].Value = rs.fields("StrOfferDetails").ToString();
                            dgvStockOut.Rows[j].Cells[GetEnum(GridColIndexes.cBatchMode)].Value = rs.fields("BatchMode").ToString();

                            dgvStockOut.Rows[j].Cells[GetEnum(GridColIndexes.cCoolie)].Value = rs.fields("Coolie").ToString();

                            if (Comm.ToDouble(rs.fields("RateInclusive").ToString()) == 1)
                                dgvStockOut.Rows[j].Cells[GetEnum(GridColIndexes.cRateinclusive)].Value = true;
                            else
                                dgvStockOut.Rows[j].Cells[GetEnum(GridColIndexes.cRateinclusive)].Value = false;

                            j++;
                        }
                    }

                    rs.MoveNext();
                }

                CalcTotal();
                CalcTotalOut();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void CRUD_Operations(int iAction = 0, bool blnLoadTest = false)
        {
            bool blnTransactionStarted = false;

            try
            {
                string[] strResult;
                string sRetDet;

                DBConnection dBConnection = new DBConnection();
                var sqlConn = dBConnection.GetDBConnection();
                SqlTransaction trans = sqlConn.BeginTransaction();

                blnTransactionStarted = true;

                try
                {
                    if (IsValidate(iAction) == true)
                    {
                        string strJson = SerializetoJson();

                        #region "CRUD Operations for Repacking Master ------------------------- >>"
                        if (iAction != 2)
                        {
                            string sRet = clsPur.RepackingMasterCRUD(clsPM, sqlConn, trans, strJson, iAction);
                            if (sRet.Length > 2)
                            {
                                strResult = sRet.Split('|');
                                if (Comm.ToInt32(strResult[0].ToString()) == -1)
                                {
                                    MessageBox.Show("Failed ? " + sRet, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    trans.Rollback();
                                    blnTransactionStarted = false;

                                    return;
                                }
                            }
                            else
                            {
                                if (Comm.ToInt32(sRet) == -1)
                                {
                                    MessageBox.Show("Failed to Save ? ", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    trans.Rollback();
                                    blnTransactionStarted = false;

                                    return;
                                }
                            }
                        }
                        #endregion

                        #region "CRUD Operations for Repacking Detail ------------------------- >>"
                        Hashtable hstPurStk = new Hashtable();

                        if (iAction == 1) // Edit
                        {
                            sRetDet = clsPur.RepackingDetailCRUD(clsPM, sqlConn, trans, sBatchCode, 2);
                            sRetDet = clsPur.RepackingDetailCRUD(clsPM, sqlConn, trans, sBatchCode, 0);
                        }
                        else
                            sRetDet = clsPur.RepackingDetailCRUD(clsPM, sqlConn, trans, sBatchCode, iAction);

                        if (sRetDet == "") sRetDet = "0";
                        if (sRetDet.Length > 2)
                        {
                            strResult = sRetDet.Split('|');
                            strResult[0] += strResult[0] + "    ";
                            if (strResult[0].ToString().Replace(" ", "").Substring(0,2) == "-1")
                            {
                                MessageBox.Show("Failed to Save ? " + strResult[1].ToString(), Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                                trans.Rollback();
                                blnTransactionStarted = false;

                                return;
                            }
                        }
                        else
                        {
                            if (Comm.ToInt32(sRetDet) == -1)
                            {
                                MessageBox.Show("Failed to Save ? ", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                                trans.Rollback();
                                blnTransactionStarted = false;

                                return;
                            }
                        }
                        #endregion

                        if (iAction == 2)
                        {
                            string sRet = clsPur.RepackingMasterCRUD(clsPM, sqlConn, trans, strJson, iAction);
                            if (sRet.Length > 2)
                            {
                                strResult = sRet.Split('|');
                                if (Comm.ToInt32(strResult[0].ToString()) == -1)
                                {
                                    MessageBox.Show("Failed ? " + sRet, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    trans.Rollback();
                                    blnTransactionStarted = false;

                                    return;
                                }
                            }
                            else
                            {
                                if (Comm.ToInt32(sRet) == -1)
                                {
                                    MessageBox.Show("Failed to Save ? ", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    trans.Rollback();
                                    blnTransactionStarted = false;

                                    return;
                                }
                            }
                        }

                        trans.Commit();
                        blnTransactionStarted = false;

                        string vchno = txtInvAutoNo.Text;


                        if (iAction < 2 && blnLoadTest == false)
                        {
                            if (iIDFromEditWindow != 0)
                            {
                                this.Close();
                                Comm.MessageboxToasted("Repacking", "Voucher[" + vchno + "] Saved Successfully");
                                return;
                            }
                            else
                            {
                                ClearControls();

                                GridInitialize_dgvColWidth(true, dgvStockIn);
                                GridInitialize_dgvColWidth(true, dgvStockOut);
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

                                Comm.MessageboxToasted("Repacking", "Voucher[" + vchno + "] Saved Successfully");
                            }
                        }
                        else if (iAction == 2)
                        {
                            this.Close();
                            Comm.MessageboxToasted("Repacking", "Voucher[" + vchno + "] deleted successfully");
                            return;
                        }
                        else if (iAction == 3)
                        {
                            this.Close();
                            Comm.MessageboxToasted("Repacking", "Voucher[" + vchno + "] is archived");
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

        //Description : Clear the Form and Grid 
        private void ClearControls()
        {
            txtReferenceAutoNo.Clear();

            SetTransactionDefaults();

            dgvStockIn.Rows.Clear();
            dgvStockIn.Refresh();
            dgvStockIn.Rows.Add();
            dgvStockIn.CurrentCell = dgvStockIn[1, 0];

            dgvStockOut.Rows.Clear();
            dgvStockOut.Refresh();
            dgvStockOut.Rows.Add();
            dgvStockOut.CurrentCell = dgvStockOut[1, 0];

            iIDFromEditWindow = 0;
            dSupplierID = 0;

            SetTransactionDefaults();
            SetApplicationSettings(dgvStockOut);
            SetApplicationSettings(dgvStockIn);

            if (Comm.ConvertI32(clsVchType.TransactionNumberingValue) == 2) // Custom
                txtInvAutoNo.Clear();
            if (Comm.ConvertI32(clsVchType.ReferenceNumberingValue) == 2) // Custom
                txtReferencePrefix.Clear();

            dgvStockIn.Columns["cRateinclusive"].Visible = false;

            dgvStockIn.Columns["cSlNo"].Frozen = true;
            dgvStockIn.Columns["cSlNo"].ReadOnly = true;
            dgvStockIn.Columns["cImgDel"].Visible = true;
            dgvStockIn.Columns["cImgDel"].Width = 40;

            txtInvAutoNo.Focus();
        }

        //Description : Function Polymorphism of SetTag
        private void SetTag(int iCellIndex, int iRowIndex, string sTag, string sConvertType = "")
        {
            if (sConvertType.ToUpper() == "PERC_FLOAT")
                dgvStockIn.Rows[iRowIndex].Cells[iCellIndex].Tag = Comm.ToDecimal(sTag).ToString("#.00");
            else
                dgvStockIn.Rows[iRowIndex].Cells[iCellIndex].Tag = sTag;
        }
        //Description : Function Polymorphism of SetTag
        private void SetTagOut(int iCellIndex, int iRowIndex, string sTag, string sConvertType = "")
        {
            if (sConvertType.ToUpper() == "PERC_FLOAT")
                dgvStockOut.Rows[iRowIndex].Cells[iCellIndex].Tag = Comm.ToDecimal(sTag).ToString("#.00");
            else
                dgvStockOut.Rows[iRowIndex].Cells[iCellIndex].Tag = sTag;
        }

        //Description : Convert the Enum Members to Column index
        private int GetEnum(GridColIndexes ColIndexes)
        {
            return (int)ColIndexes;
        }

        //Description : Convert the Ledger Enum Members to Array Index
        private int GetEnumLedger(LedgerIndexes LedIndexes)
        {
            return (int)LedIndexes;
        }

        //Description : Convert the Agent Enum Members to Array Index
        private int GetEnumAgent(AgentIndexes AgntIndex)
        {
            return (int)AgntIndex;
        }

        //Description : Convert the Employee Enum Members to Array Index
        private int GetEnumEmp(EmpIndexes EmpIndx)
        {
            return (int)EmpIndx;
        }

        //Description : Convert the Employee Enum Members to Array Index
        private int GetEnumItem(ItemIndexes ItmIndex)
        {
            return (int)ItmIndex;
        }

        //Description : Shelf Life Stock Effect Asper Expiry Date
        private bool ShelfLifeEffect()
        {
            DateTime sRepackingDate = Convert.ToDateTime(dtpInvDate.Text);
            DateTime sExpiryDate = Convert.ToDateTime(dgvStockIn.CurrentRow.Cells[GetEnum(GridColIndexes.CExpiry)].Value);

            if (iShelfLifeDays > 0)
            {
                int iDaysCount = Comm.ToInt32((sExpiryDate - sRepackingDate).TotalDays);
                if (iDaysCount < iShelfLifeDays)
                {
                    MessageBox.Show("[" + dtItemPublic.Rows[0]["ItemName"].ToString() + " ]" + "Expiry date is below Shelf Life days", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                return true;
            }
            return true;
        }

        //Description : What to happen when BatchCode/BarUnique Select from the Grid Compact Search
        private Boolean GetFromBatchCodeSearch(string sReturn)
        {
            DataTable dtBarCodeExist = new DataTable();
            DataTable dtSelBatch = new DataTable();
            string[] sCompSearchData = sReturn.Split('|');
            if (sCompSearchData[0].ToString() == "NOTEXIST")
            {
                sgblBarcodeNoExists = "BARCODE_NOTEXIST";
                dtBarCodeExist = Comm.fnGetData("SELECT COUNT(*) FROM tblStock WHERE LTRIM(RTRIM(UPPER(BatchCode))) = '" + sCompSearchData[1].ToString().Trim() + "'").Tables[0];
                if (Comm.ToInt32(dtBarCodeExist.Rows[0][0].ToString()) == 0)
                {
                    SetValue(GetEnum(GridColIndexes.cBarCode), sCompSearchData[1].ToString().Trim());
                    setTag(GetEnum(GridColIndexes.cBarCode), sCompSearchData[1].ToString().Trim());
                    return true;
                }
                else
                {
                    MessageBox.Show("This BatchCode [" + sCompSearchData[1].ToString().Trim() + "] is already Exist.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }
            else
            {
                if (sCompSearchData.Length > 0)
                {
                    sgblBarcodeNoExists = "";
                    if (Comm.ToInt32(sCompSearchData[0].ToString()) >= 0)
                    {
                        FillGridAsperStockID(Comm.ToInt32(sCompSearchData[0].ToString()));
                        return true;
                    }
                    else
                        return false;
                }
                else
                    return false;
            }
        }
        //Description : What to happen when BatchCode/BarUnique Select from the Grid Compact Search
        private Boolean GetFromBatchCodeSearchOut(string sReturn)
        {
            DataTable dtBarCodeExist = new DataTable();
            DataTable dtSelBatch = new DataTable();
            string[] sCompSearchData = sReturn.Split('|');
            if (sCompSearchData[0].ToString() == "NOTEXIST")
            {
                sgblBarcodeNoExists = "BARCODE_NOTEXIST";
                dtBarCodeExist = Comm.fnGetData("SELECT COUNT(*) FROM tblStock WHERE LTRIM(RTRIM(UPPER(BatchCode))) = '" + sCompSearchData[1].ToString().Trim() + "'").Tables[0];
                if (Comm.ToInt32(dtBarCodeExist.Rows[0][0].ToString()) == 0)
                {
                    SetValueOut(GetEnum(GridColIndexes.cBarCode), sCompSearchData[1].ToString().Trim());
                    setTagOut(GetEnum(GridColIndexes.cBarCode), sCompSearchData[1].ToString().Trim());
                    return true;
                }
                else
                {
                    MessageBox.Show("This BatchCode [" + sCompSearchData[1].ToString().Trim() + "] is already Exist.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }
            else
            {
                if (sCompSearchData.Length > 0)
                {
                    sgblBarcodeNoExists = "";
                    if (Comm.ToInt32(sCompSearchData[0].ToString()) >= 0)
                    {
                        FillGridAsperStockIDOut(Comm.ToInt32(sCompSearchData[0].ToString()));
                        return true;
                    }
                    else
                        return false;
                }
                else
                    return false;
            }
        }

        //Description : Deligate Returns the True/False from the method from Pause Search List
        private Boolean GetFromPauseSearch(string sRet)
        {
            string[] sCompSearchData = sRet.Split('|');
            List<decimal> lstItmDisc = new List<decimal>();
            string strJson = "";
            if (sCompSearchData[0].ToString() == "NOTEXIST")
            {
                return true;
            }
            else
            {
                if (sCompSearchData.Length > 0)
                {
                    if (Comm.ToInt32(sCompSearchData[0].ToString()) != 0)
                    {
                        DataTable dtGet = Comm.fnGetData("SELECT * FROM tblTransactionPause WHERE ID =" + Comm.ToInt32(sCompSearchData[0].ToString()) + "").Tables[0];
                        if (dtGet.Rows.Count > 0)
                        {
                            lblPause.Tag = Comm.ToInt32(sCompSearchData[0].ToString());
                            strJson = dtGet.Rows[0]["JsonData"].ToString();
                            DeserializeFromJSon(strJson);
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

        //Description : What to happen when Item is Select from the Grid Compact Search
        private Boolean GetFromItemSearch(string sReturn)
        {
            try
            {
                mblnInitialisedSubWindow = false;

                DataTable dtBatch = new DataTable();
                string[] sCompSearchData = sReturn.Split('|');
                List<decimal> lstItmDisc = new List<decimal>();
                decimal dItmWiseDisccount = 0;
                decimal[] dDiscArray;
                if (sCompSearchData[0].ToString() == "NOTEXIST")
                {
                    return true;
                }
                else
                {
                    if (sCompSearchData.Length > 0)
                    {
                        if (Comm.ToInt32(sCompSearchData[0].ToString()) != 0)
                        {
                            GetItmMststockinfo.StockID = Comm.ToInt32(sCompSearchData[0].ToString());
                            GetItmMststockinfo.TenantID = Global.gblTenantID;

                            //dtItemPublic = clsItmMst.GetItemMaster(GetItmMstinfo);
                            dtItemPublic = clsItmMst.GetItemMasterFromStock(GetItmMststockinfo);
                            if (dtItemPublic.Rows.Count > 0)
                            {
                                SetValue(GetEnum(GridColIndexes.CItemName), dtItemPublic.Rows[0]["ItemName"].ToString());
                                SetValue(GetEnum(GridColIndexes.CUnit), dtItemPublic.Rows[0]["Unit"].ToString());
                                setTag(GetEnum(GridColIndexes.CUnit), dtItemPublic.Rows[0]["UNITID"].ToString());
                                SetValue(GetEnum(GridColIndexes.cItemID), dtItemPublic.Rows[0]["ItemID"].ToString());
                                setTag(GetEnum(GridColIndexes.CItemName), dtItemPublic.Rows[0]["ItemID"].ToString());
                                SetValue(GetEnum(GridColIndexes.cBarCode), dtItemPublic.Rows[0]["BatchUnique"].ToString());
                                setTag(GetEnum(GridColIndexes.cBarCode), dtItemPublic.Rows[0]["BatchCode"].ToString());

                                SetValue(GetEnum(GridColIndexes.cPrate), dtItemPublic.Rows[0]["PRate"].ToString(), "CURR_FLOAT");
                                SetValue(GetEnum(GridColIndexes.cMRP), dtItemPublic.Rows[0]["MRP"].ToString(), "CURR_FLOAT");

                                SetValue(GetEnum(GridColIndexes.cQty), 1.ToString());

                                SetValue(GetEnum(GridColIndexes.cSRate1Per), dtItemPublic.Rows[0]["Srate1Per"].ToString(), "PERC_FLOAT");
                                SetValue(GetEnum(GridColIndexes.cSRate1), dtItemPublic.Rows[0]["SRate1"].ToString(), "CURR_FLOAT");
                                SetValue(GetEnum(GridColIndexes.cSRate2Per), dtItemPublic.Rows[0]["Srate2Per"].ToString(), "PERC_FLOAT");
                                SetValue(GetEnum(GridColIndexes.cSRate2), dtItemPublic.Rows[0]["SRate2"].ToString(), "CURR_FLOAT");
                                SetValue(GetEnum(GridColIndexes.cSRate3Per), dtItemPublic.Rows[0]["Srate3Per"].ToString(), "PERC_FLOAT");
                                SetValue(GetEnum(GridColIndexes.cSRate3), dtItemPublic.Rows[0]["SRate3"].ToString(), "CURR_FLOAT");
                                SetValue(GetEnum(GridColIndexes.cSRate4Per), dtItemPublic.Rows[0]["Srate4Per"].ToString(), "PERC_FLOAT");
                                SetValue(GetEnum(GridColIndexes.cSRate4), dtItemPublic.Rows[0]["SRate4"].ToString(), "CURR_FLOAT");
                                SetValue(GetEnum(GridColIndexes.cSRate5Per), dtItemPublic.Rows[0]["Srate5Per"].ToString(), "PERC_FLOAT");
                                SetValue(GetEnum(GridColIndexes.cSRate5), dtItemPublic.Rows[0]["SRate5"].ToString(), "CURR_FLOAT");

                                SetValue(GetEnum(GridColIndexes.cCCessPer), dtItemPublic.Rows[0]["CessPer"].ToString(), "CURR_FLOAT");
                                SetValue(GetEnum(GridColIndexes.cCCompCessQty), dtItemPublic.Rows[0]["CompCessQty"].ToString(), "CURR_FLOAT");
                                SetTag(GetEnum(GridColIndexes.cSRate1Per), dgvStockIn.CurrentRow.Index, dtItemPublic.Rows[0]["SrateCalcMode"].ToString(), "");

                                SetTag(GetEnum(GridColIndexes.cCoolie), dgvStockIn.CurrentRow.Index, dtItemPublic.Rows[0]["Coolie"].ToString(), "");
                                SetValue(GetEnum(GridColIndexes.cCoolie), dtItemPublic.Rows[0]["Coolie"].ToString(), "CURR_FLOAT");

                                SetTag(GetEnum(GridColIndexes.cAgentCommPer), dgvStockIn.CurrentRow.Index, dtItemPublic.Rows[0]["agentCommPer"].ToString(), "");
                                SetValue(GetEnum(GridColIndexes.cAgentCommPer), dtItemPublic.Rows[0]["agentCommPer"].ToString(), "CURR_FLOAT");

                                    SetTag(GetEnum(GridColIndexes.cCGST), dgvStockIn.CurrentRow.Index, "0", "PERC_FLOAT");
                                    SetTag(GetEnum(GridColIndexes.cSGST), dgvStockIn.CurrentRow.Index, "0", "PERC_FLOAT");
                                    SetTag(GetEnum(GridColIndexes.cIGST), dgvStockIn.CurrentRow.Index, "0", "PERC_FLOAT");
                                    SetValue(GetEnum(GridColIndexes.ctaxPer), "0", "0");

                                if (Comm.ToInt32(dtItemPublic.Rows[0]["PRateInclusive"].ToString()) == 1)
                                    dgvStockIn.Rows[dgvStockIn.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cRateinclusive)].Value = true;
                                else
                                    dgvStockIn.Rows[dgvStockIn.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cRateinclusive)].Value = false;

                                if (clsVchType.ItmWiseDiscFillXtraDiscFromValue == 2) //Item Discount
                                    dItmWiseDisccount = Comm.ToDecimal(dtItemPublic.Rows[0]["DiscPer"].ToString());
                                else if (clsVchType.ItmWiseDiscFillXtraDiscFromValue == 3) //Category Discount
                                    dItmWiseDisccount = Comm.ToDecimal(dtItemPublic.Rows[0]["iCatDiscPer"].ToString());
                                else if (clsVchType.ItmWiseDiscFillXtraDiscFromValue == 4) //Manufacturer Discount
                                    dItmWiseDisccount = Comm.ToDecimal(dtItemPublic.Rows[0]["ImanDiscPer"].ToString());
                                else if (clsVchType.ItmWiseDiscFillXtraDiscFromValue == 5) //Discount Group Discount
                                    dItmWiseDisccount = Comm.ToDecimal(dtItemPublic.Rows[0]["DGroupDisPer"].ToString());
                                else if (clsVchType.ItmWiseDiscFillXtraDiscFromValue == 6) //Highest Discount
                                {
                                    lstItmDisc.Add(Comm.ToDecimal(dtItemPublic.Rows[0]["DiscPer"].ToString()));
                                    lstItmDisc.Add(Comm.ToDecimal(dtItemPublic.Rows[0]["iCatDiscPer"].ToString()));
                                    lstItmDisc.Add(Comm.ToDecimal(dtItemPublic.Rows[0]["ImanDiscPer"].ToString()));
                                    lstItmDisc.Add(Comm.ToDecimal(dtItemPublic.Rows[0]["DGroupDisPer"].ToString()));
                                    dDiscArray = lstItmDisc.ToArray();
                                    dItmWiseDisccount = dDiscArray.Max();
                                }
                                SetValue(GetEnum(GridColIndexes.cDiscPer), "0", "");

                                dtCurrExp = DateTime.Today;
                                if (Comm.ToDouble(dtItemPublic.Rows[0]["DefaultExpInDays"].ToString()) > 0)
                                    dtCurrExp = dtCurrExp.AddDays(Comm.ToDouble(dtItemPublic.Rows[0]["DefaultExpInDays"].ToString()));
                                else
                                    dtCurrExp = dtCurrExp.AddYears(8);

                                SetValue(GetEnum(GridColIndexes.CExpiry), dtCurrExp.ToString("dd-MMM-yyyy").ToString(), "DATE");
                                SetTag(GetEnum(GridColIndexes.CExpiry), dgvStockIn.CurrentRow.Index, dtItemPublic.Rows[0]["BlnExpiryItem"].ToString());
                                if (Comm.ToInt32(dtItemPublic.Rows[0]["BlnExpiryItem"].ToString()) == 1)
                                {
                                    dgvStockIn.Rows[dgvStockIn.CurrentRow.Index].Cells[GetEnum(GridColIndexes.CExpiry)].ReadOnly = false;
                                }
                                else
                                {
                                    dgvStockIn.Rows[dgvStockIn.CurrentRow.Index].Cells[GetEnum(GridColIndexes.CExpiry)].ReadOnly = true;
                                }

                                iBatchmode = Comm.ToInt32(dtItemPublic.Rows[0]["batchMode"].ToString());
                                SetValue(GetEnum(GridColIndexes.cBatchMode), iBatchmode.ToString());
                                iShelfLifeDays = Comm.ToInt32(dtItemPublic.Rows[0]["Shelflife"].ToString());

                                if (iBatchmode == 1)
                                {
                                    if (dgvStockIn.Columns[GetEnum(GridColIndexes.cBarCode)].Visible == true)
                                        dgvStockIn.CurrentCell = dgvStockIn.Rows[dgvStockIn.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cBarCode)];

                                    dgvStockIn.Rows[dgvStockIn.CurrentRow.Index].Cells[GetEnum(GridColIndexes.CItemCode)].Value = dtItemPublic.Rows[0]["ItemCode"].ToString();
                                    
                                }
                                else if (iBatchmode == 2)
                                {
                                    dgvStockIn.Rows[dgvStockIn.CurrentRow.Index].Cells[GetEnum(GridColIndexes.CItemCode)].Value = dtItemPublic.Rows[0]["ItemCode"].ToString();

                                    if (dtItemPublic.Rows[0]["StockID"].ToString() != "")
                                        FillGridAsperStockID(Comm.ToInt32(dtItemPublic.Rows[0]["StockID"].ToString()));

                                    //After taking values from stock the batchcode, expiry fields are to be reset for auto batch code
                                    dgvStockIn.Rows[dgvStockIn.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cBarCode)].Value = "<Auto Barcode>";
                                    dgvStockIn.Rows[dgvStockIn.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cBarCode)].Tag = 0;

                                    dtCurrExp = DateTime.Today;
                                    if (Comm.ToDouble(dtItemPublic.Rows[0]["DefaultExpInDays"].ToString()) > 0)
                                        dtCurrExp = dtCurrExp.AddDays(Comm.ToDouble(dtItemPublic.Rows[0]["DefaultExpInDays"].ToString()));
                                    else
                                        dtCurrExp = dtCurrExp.AddYears(8);

                                    SetValue(GetEnum(GridColIndexes.CExpiry), dtCurrExp.ToString("dd-MMM-yyyy").ToString(), "DATE");
                                    SetTag(GetEnum(GridColIndexes.CExpiry), dgvStockIn.CurrentRow.Index, dtItemPublic.Rows[0]["BlnExpiryItem"].ToString());
                                    if (Comm.ToInt32(dtItemPublic.Rows[0]["BlnExpiryItem"].ToString()) == 1)
                                    {
                                        dgvStockIn.Rows[dgvStockIn.CurrentRow.Index].Cells[GetEnum(GridColIndexes.CExpiry)].ReadOnly = false;
                                    }
                                    else
                                    {
                                        dgvStockIn.Rows[dgvStockIn.CurrentRow.Index].Cells[GetEnum(GridColIndexes.CExpiry)].ReadOnly = true;
                                    }

                                    if (dgvStockIn.Rows[dgvStockIn.CurrentRow.Index].Cells[GetEnum(GridColIndexes.CExpiry)].Visible == true)
                                        dgvStockIn.CurrentCell = dgvStockIn.Rows[dgvStockIn.CurrentRow.Index].Cells[GetEnum(GridColIndexes.CExpiry)];
                                    else if (dgvStockIn.Rows[dgvStockIn.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cMRP)].Visible == true)
                                        dgvStockIn.CurrentCell = dgvStockIn.Rows[dgvStockIn.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cMRP)];
                                    else if (dgvStockIn.Rows[dgvStockIn.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cPrate)].Visible == true)
                                        dgvStockIn.CurrentCell = dgvStockIn.Rows[dgvStockIn.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cPrate)];
                                    else
                                        dgvStockIn.CurrentCell = dgvStockIn.Rows[dgvStockIn.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cQty)];
                                    
                                    dgvStockIn.Focus();
                                    CalcTotal();
                                }
                                else if (iBatchmode == 0 || iBatchmode == 3)
                                {
                                    if (dtItemPublic.Rows[0]["BatchUnique"].ToString() != "")
                                    {
                                        dgvStockIn.Rows[dgvStockIn.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cBarCode)].Value = dtItemPublic.Rows[0]["BatchCode"].ToString();
                                        dgvStockIn.Rows[dgvStockIn.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cBarCode)].Tag = dtItemPublic.Rows[0]["BatchUnique"].ToString();
                                    }
                                    else
                                    {
                                        dgvStockIn.Rows[dgvStockIn.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cBarCode)].Value = dtItemPublic.Rows[0]["BatchCode"].ToString();
                                        dgvStockIn.Rows[dgvStockIn.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cBarCode)].Tag = dtItemPublic.Rows[0]["BatchCode"].ToString();
                                    }

                                    dgvStockIn.Rows[dgvStockIn.CurrentRow.Index].Cells[GetEnum(GridColIndexes.CItemCode)].Value = dtItemPublic.Rows[0]["ItemCode"].ToString();
                                    
                                    if(dtItemPublic.Rows[0]["StockID"].ToString() != "")
                                        FillGridAsperStockID(Comm.ToInt32(dtItemPublic.Rows[0]["StockID"].ToString()));

                                    if (dgvStockIn.Rows[dgvStockIn.CurrentRow.Index].Cells[GetEnum(GridColIndexes.CExpiry)].Visible == true)
                                        dgvStockIn.CurrentCell = dgvStockIn.Rows[dgvStockIn.CurrentRow.Index].Cells[GetEnum(GridColIndexes.CExpiry)];
                                    else
                                        dgvStockIn.CurrentCell = dgvStockIn.Rows[dgvStockIn.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cQty)];

                                    dgvStockIn.Focus();
                                    CalcTotal();
                                }
                                SetValue(GetEnum(GridColIndexes.CItemCode), dtItemPublic.Rows[0]["ItemCode"].ToString());

                                if (dgvStockIn.Rows.Count - 1 == dgvStockIn.CurrentRow.Index)
                                    dgvStockIn.Rows.Add();

                                CalcTotal();
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

        //Description : What to happen when Item is Select from the Grid Compact Search
        private Boolean GetFromItemSearchOut(string sReturn)
        {
            try
            {
                mblnInitialisedSubWindow = false;

                DataTable dtBatch = new DataTable();
                string[] sCompSearchData = sReturn.Split('|');
                List<decimal> lstItmDisc = new List<decimal>();
                decimal dItmWiseDisccount = 0;
                decimal[] dDiscArray;
                if (sCompSearchData[0].ToString() == "NOTEXIST")
                {
                    return true;
                }
                else
                {
                    if (sCompSearchData.Length > 0)
                    {
                        if (Comm.ToInt32(sCompSearchData[0].ToString()) != 0)
                        {
                            GetItmMststockinfo.StockID = Comm.ToInt32(sCompSearchData[0].ToString());
                            GetItmMststockinfo.TenantID = Global.gblTenantID;

                            //dtItemPublic = clsItmMst.GetItemMaster(GetItmMstinfo);
                            dtItemPublic = clsItmMst.GetItemMasterFromStock(GetItmMststockinfo);
                            if (dtItemPublic.Rows.Count > 0)
                            {
                                SetValueOut(GetEnum(GridColIndexes.CItemName), dtItemPublic.Rows[0]["ItemName"].ToString());
                                SetValueOut(GetEnum(GridColIndexes.CUnit), dtItemPublic.Rows[0]["Unit"].ToString());
                                setTagOut(GetEnum(GridColIndexes.CUnit), dtItemPublic.Rows[0]["UNITID"].ToString());
                                SetValueOut(GetEnum(GridColIndexes.cItemID), dtItemPublic.Rows[0]["ItemID"].ToString());
                                setTagOut(GetEnum(GridColIndexes.CItemName), dtItemPublic.Rows[0]["ItemID"].ToString());
                                SetValueOut(GetEnum(GridColIndexes.cBarCode), dtItemPublic.Rows[0]["BatchUnique"].ToString());
                                setTagOut(GetEnum(GridColIndexes.cBarCode), dtItemPublic.Rows[0]["BatchCode"].ToString());

                                SetValueOut(GetEnum(GridColIndexes.cPrate), dtItemPublic.Rows[0]["PRate"].ToString(), "CURR_FLOAT");
                                SetValueOut(GetEnum(GridColIndexes.cMRP), dtItemPublic.Rows[0]["MRP"].ToString(), "CURR_FLOAT");

                                SetValueOut(GetEnum(GridColIndexes.cCrate), dtItemPublic.Rows[0]["CostRateExcl"].ToString(), "CURR_FLOAT");
                                SetValueOut(GetEnum(GridColIndexes.cCRateWithTax), dtItemPublic.Rows[0]["CostRateInc"].ToString(), "CURR_FLOAT");

                                SetValueOut(GetEnum(GridColIndexes.cQty), 1.ToString());

                                SetValueOut(GetEnum(GridColIndexes.cSRate1Per), dtItemPublic.Rows[0]["Srate1Per"].ToString(), "PERC_FLOAT");
                                SetValueOut(GetEnum(GridColIndexes.cSRate1), dtItemPublic.Rows[0]["SRate1"].ToString(), "CURR_FLOAT");
                                SetValueOut(GetEnum(GridColIndexes.cSRate2Per), dtItemPublic.Rows[0]["Srate2Per"].ToString(), "PERC_FLOAT");
                                SetValueOut(GetEnum(GridColIndexes.cSRate2), dtItemPublic.Rows[0]["SRate2"].ToString(), "CURR_FLOAT");
                                SetValueOut(GetEnum(GridColIndexes.cSRate3Per), dtItemPublic.Rows[0]["Srate3Per"].ToString(), "PERC_FLOAT");
                                SetValueOut(GetEnum(GridColIndexes.cSRate3), dtItemPublic.Rows[0]["SRate3"].ToString(), "CURR_FLOAT");
                                SetValueOut(GetEnum(GridColIndexes.cSRate4Per), dtItemPublic.Rows[0]["Srate4Per"].ToString(), "PERC_FLOAT");
                                SetValueOut(GetEnum(GridColIndexes.cSRate4), dtItemPublic.Rows[0]["SRate4"].ToString(), "CURR_FLOAT");
                                SetValueOut(GetEnum(GridColIndexes.cSRate5Per), dtItemPublic.Rows[0]["Srate5Per"].ToString(), "PERC_FLOAT");
                                SetValueOut(GetEnum(GridColIndexes.cSRate5), dtItemPublic.Rows[0]["SRate5"].ToString(), "CURR_FLOAT");

                                SetValueOut(GetEnum(GridColIndexes.cCCessPer), dtItemPublic.Rows[0]["CessPer"].ToString(), "CURR_FLOAT");
                                SetValueOut(GetEnum(GridColIndexes.cCCompCessQty), dtItemPublic.Rows[0]["CompCessQty"].ToString(), "CURR_FLOAT");
                                SetTagOut(GetEnum(GridColIndexes.cSRate1Per), dgvStockOut.CurrentRow.Index, dtItemPublic.Rows[0]["SrateCalcMode"].ToString(), "");

                                SetTagOut(GetEnum(GridColIndexes.cCoolie), dgvStockOut.CurrentRow.Index, dtItemPublic.Rows[0]["Coolie"].ToString(), "");
                                SetValueOut(GetEnum(GridColIndexes.cCoolie), dtItemPublic.Rows[0]["Coolie"].ToString(), "CURR_FLOAT");

                                SetTagOut(GetEnum(GridColIndexes.cAgentCommPer), dgvStockOut.CurrentRow.Index, dtItemPublic.Rows[0]["agentCommPer"].ToString(), "");
                                SetValueOut(GetEnum(GridColIndexes.cAgentCommPer), dtItemPublic.Rows[0]["agentCommPer"].ToString(), "CURR_FLOAT");

                                    SetTagOut(GetEnum(GridColIndexes.cCGST), dgvStockOut.CurrentRow.Index, "0", "PERC_FLOAT");
                                    SetTagOut(GetEnum(GridColIndexes.cSGST), dgvStockOut.CurrentRow.Index, "0", "PERC_FLOAT");
                                    SetTagOut(GetEnum(GridColIndexes.cIGST), dgvStockOut.CurrentRow.Index, "0", "PERC_FLOAT");
                                    SetValueOut(GetEnum(GridColIndexes.ctaxPer), "0", "0");

                                if (Comm.ToInt32(dtItemPublic.Rows[0]["PRateInclusive"].ToString()) == 1)
                                    dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cRateinclusive)].Value = true;
                                else
                                    dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cRateinclusive)].Value = false;

                                SetValueOut(GetEnum(GridColIndexes.cDiscPer), dItmWiseDisccount.ToString(), "");

                                dtCurrExp = DateTime.Today;
                                if (Comm.ToDouble(dtItemPublic.Rows[0]["DefaultExpInDays"].ToString()) > 0)
                                    dtCurrExp = dtCurrExp.AddDays(Comm.ToDouble(dtItemPublic.Rows[0]["DefaultExpInDays"].ToString()));
                                else
                                    dtCurrExp = dtCurrExp.AddYears(8);

                                SetValueOut(GetEnum(GridColIndexes.CExpiry), dtCurrExp.ToString("dd-MMM-yyyy").ToString(), "DATE");
                                SetTagOut(GetEnum(GridColIndexes.CExpiry), dgvStockOut.CurrentRow.Index, dtItemPublic.Rows[0]["BlnExpiryItem"].ToString());
                                if (Comm.ToInt32(dtItemPublic.Rows[0]["BlnExpiryItem"].ToString()) == 1)
                                {
                                    dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(GridColIndexes.CExpiry)].ReadOnly = false;
                                }
                                else
                                {
                                    dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(GridColIndexes.CExpiry)].ReadOnly = true;
                                }

                                iBatchmode = Comm.ToInt32(dtItemPublic.Rows[0]["batchMode"].ToString());
                                SetValueOut(GetEnum(GridColIndexes.cBatchMode), iBatchmode.ToString());
                                iShelfLifeDays = Comm.ToInt32(dtItemPublic.Rows[0]["Shelflife"].ToString());

                                if (iBatchmode == 1)
                                {
                                    if (dgvStockOut.Columns[GetEnum(GridColIndexes.cBarCode)].Visible == true)
                                        dgvStockOut.CurrentCell = dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cBarCode)];

                                    dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(GridColIndexes.CItemCode)].Value = dtItemPublic.Rows[0]["ItemCode"].ToString();
                                }
                                else if (iBatchmode == 2)
                                {
                                    dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(GridColIndexes.CItemCode)].Value = dtItemPublic.Rows[0]["ItemCode"].ToString();

                                    if (dtItemPublic.Rows[0]["StockID"].ToString() != "")
                                        FillGridAsperStockIDOut(Comm.ToInt32(dtItemPublic.Rows[0]["StockID"].ToString()));

                                    //After taking values from stock the batchcode, expiry fields are to be reset for auto batch code
                                    dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cBarCode)].Value = "<Auto Barcode>";
                                    dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cBarCode)].Tag = 0;

                                    dtCurrExp = DateTime.Today;
                                    if (Comm.ToDouble(dtItemPublic.Rows[0]["DefaultExpInDays"].ToString()) > 0)
                                        dtCurrExp = dtCurrExp.AddDays(Comm.ToDouble(dtItemPublic.Rows[0]["DefaultExpInDays"].ToString()));
                                    else
                                        dtCurrExp = dtCurrExp.AddYears(8);

                                    SetValueOut(GetEnum(GridColIndexes.CExpiry), dtCurrExp.ToString("dd-MMM-yyyy").ToString(), "DATE");
                                    SetTagOut(GetEnum(GridColIndexes.CExpiry), dgvStockOut.CurrentRow.Index, dtItemPublic.Rows[0]["BlnExpiryItem"].ToString());
                                    if (Comm.ToInt32(dtItemPublic.Rows[0]["BlnExpiryItem"].ToString()) == 1)
                                    {
                                        dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(GridColIndexes.CExpiry)].ReadOnly = false;
                                    }
                                    else
                                    {
                                        dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(GridColIndexes.CExpiry)].ReadOnly = true;
                                    }

                                    if (dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(GridColIndexes.CExpiry)].Visible == true)
                                        dgvStockOut.CurrentCell = dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(GridColIndexes.CExpiry)];
                                    else if (dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cMRP)].Visible == true)
                                        dgvStockOut.CurrentCell = dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cMRP)];
                                    else if (dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cPrate)].Visible == true)
                                        dgvStockOut.CurrentCell = dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cPrate)];
                                    else
                                        dgvStockOut.CurrentCell = dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cQty)];
                                    
                                    dgvStockOut.Focus();
                                    CalcTotalOut();

                                }
                                else if (iBatchmode == 0 || iBatchmode == 3)
                                {
                                    if (dtItemPublic.Rows[0]["BatchUnique"].ToString() != "")
                                    {
                                        dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cBarCode)].Value = dtItemPublic.Rows[0]["BatchCode"].ToString();
                                        dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cBarCode)].Tag = dtItemPublic.Rows[0]["BatchUnique"].ToString();
                                    }
                                    else
                                    {
                                        dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cBarCode)].Value = dtItemPublic.Rows[0]["BatchCode"].ToString();
                                        dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cBarCode)].Tag = dtItemPublic.Rows[0]["BatchCode"].ToString();
                                    }

                                    dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(GridColIndexes.CItemCode)].Value = dtItemPublic.Rows[0]["ItemCode"].ToString();
                                    
                                    if(dtItemPublic.Rows[0]["StockID"].ToString() != "")
                                        FillGridAsperStockIDOut(Comm.ToInt32(dtItemPublic.Rows[0]["StockID"].ToString()));

                                    if (dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(GridColIndexes.CExpiry)].Visible == true)
                                        dgvStockOut.CurrentCell = dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(GridColIndexes.CExpiry)];
                                    else
                                        dgvStockOut.CurrentCell = dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cQty)];

                                    dgvStockOut.Focus();
                                    CalcTotalOut();

                                }
                                SetValueOut(GetEnum(GridColIndexes.CItemCode), dtItemPublic.Rows[0]["ItemCode"].ToString());

                                if (dgvStockOut.Rows.Count - 1 == dgvStockOut.CurrentRow.Index)
                                    dgvStockOut.Rows.Add();

                                CalcTotalOut();

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
            int rowIndex = dgvStockIn.CurrentCell.RowIndex;
            dgvStockIn.Rows.RemoveAt(rowIndex);
        }
        private void RowDeleteOut()
        {
            int rowIndex = dgvStockOut.CurrentCell.RowIndex;
            dgvStockOut.Rows.RemoveAt(rowIndex);
        }

        //Description : Initializing the Columns in The Grid
        private void AddColumnsToGrid(DataGridView dgvStock)
        {
            dgvStock.Columns.Clear();

            dgvStock.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSlNo", HeaderText = "Sl.No", Width = 50, ReadOnly = true }); //1
            dgvStock.Columns.Add(new DataGridViewTextBoxColumn() { Name = "CItemCode", HeaderText = "Item Code", Width = 130 }); //1
            dgvStock.Columns.Add(new DataGridViewTextBoxColumn() { Name = "CItemName", HeaderText = "Item Name", Width = 200, ReadOnly = true }); //2
            dgvStock.Columns.Add(new DataGridViewTextBoxColumn() { Name = "CUnit", ReadOnly = true, Visible = true, HeaderText = "Unit", Width = 50 }); //3
            //Commented and added By Dipu on 23-Feb-2022 ------------- >>
            dgvStock.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cBarCode", HeaderText = "Batch Code", Width = 200 }); //4
            dgvStock.Columns.Add(new DataGridViewTextBoxColumn() { Name = "CExpiry", HeaderText = "Expiry Date", Width = 120 }); //5
            
            if (clsVchTypeFeatures.BLNEDITMRPRATE == true)
            {
                if (AppSettings.IsActiveMRP == true)
                    dgvStock.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cMRP", HeaderText = "" + AppSettings.MRPName + "", ReadOnly = false, Visible = true, Width = 80 }); //6
                else
                    dgvStock.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cMRP", HeaderText = "" + AppSettings.MRPName + "", ReadOnly = false, Visible = false, Width = 80 }); //6
            }
            else
            {
                if (AppSettings.IsActiveMRP == true)
                    dgvStock.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cMRP", HeaderText = "" + AppSettings.MRPName + "", ReadOnly = true, Visible = true, Width = 80 }); //6
                else
                    dgvStock.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cMRP", HeaderText = "" + AppSettings.MRPName + "", ReadOnly = true, Visible = false, Width = 80 }); //6
            }

            dgvStock.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cPrate", HeaderText = "PRate", Width = 80 }); //7

            if (AppSettings.TaxMode == 2) //GST
                dgvStock.Columns.Add(new DataGridViewCheckBoxColumn() { Name = "cRateinclusive", HeaderText = "Rate Inc.", Width = 80, ReadOnly = true }); //20
            else
                dgvStock.Columns.Add(new DataGridViewCheckBoxColumn() { Name = "cRateinclusive", HeaderText = "Rate Inc.", Visible = false, Width = 80, ReadOnly = true }); //20

            dgvStock.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cQty", HeaderText = "Qty", Width = 80 }); //8
            if (clsVchTypeFeatures.BLNSHOWFREEQUANTITY == true)
                dgvStock.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cFree", HeaderText = "Free", Visible = true, Width = 80 }); //9
            else
                dgvStock.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cFree", HeaderText = "Free", Visible = false, Width = 80 }); //9

            if (clsVchTypeFeatures.blneditsalerate == true)
            {
                dgvStock.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate1Per", HeaderText = "" + AppSettings.SRate1Name + " %", ReadOnly = false, Width = 80 }); //10
                dgvStock.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate1", HeaderText = "" + AppSettings.SRate1Name + "", ReadOnly = false, Width = 80 }); //11
                if (AppSettings.IsActiveSRate2 == true)
                {
                    dgvStock.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate2Per", HeaderText = "" + AppSettings.SRate2Name + " %", ReadOnly = false, Visible=true, Width = 80 }); //12
                    dgvStock.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate2", HeaderText = "" + AppSettings.SRate2Name + "", ReadOnly = false, Visible=true, Width = 80 }); //13
                }
                else
                {
                    dgvStock.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate2Per", HeaderText = "" + AppSettings.SRate2Name + " %", ReadOnly = false, Visible=false, Width = 80 }); //12
                    dgvStock.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate2", HeaderText = "" + AppSettings.SRate2Name + "", ReadOnly = false, Visible=false, Width = 80 }); //13
                }

                if (AppSettings.IsActiveSRate3 == true)
                {
                    dgvStock.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate3Per", HeaderText = "" + AppSettings.SRate3Name + " %", ReadOnly = false, Visible = true, Width = 80 }); //14
                    dgvStock.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate3", HeaderText = AppSettings.SRate3Name, ReadOnly = false, Visible = true, Width = 80 }); //15
                }
                else
                {
                    dgvStock.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate3Per", HeaderText = "" + AppSettings.SRate3Name + " %", ReadOnly = false, Visible = false, Width = 80 }); //14
                    dgvStock.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate3", HeaderText = AppSettings.SRate3Name, ReadOnly = false, Visible = false, Width = 80 }); //15
                }

                if (AppSettings.IsActiveSRate4 == true)
                {
                    dgvStock.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate4Per", HeaderText = "" + AppSettings.SRate4Name + " %", ReadOnly = false, Visible = true, Width = 80 }); //16
                    dgvStock.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate4", HeaderText = AppSettings.SRate4Name, ReadOnly = false, Visible = true, Width = 80 }); //17
                }
                else
                {
                    dgvStock.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate4Per", HeaderText = "" + AppSettings.SRate4Name + " %", ReadOnly = false, Visible = false, Width = 80 }); //16
                    dgvStock.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate4", HeaderText = AppSettings.SRate4Name, ReadOnly = false, Visible = false, Width = 80 }); //17
                }

                if (AppSettings.IsActiveSRate5 == true)
                {
                    dgvStock.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate5Per", HeaderText = "" + AppSettings.SRate5Name + " %", ReadOnly = false, Visible = true, Width = 80 }); //18
                    dgvStock.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate5", HeaderText = AppSettings.SRate5Name, ReadOnly = false, Visible = true, Width = 80 }); //19
                }
                else
                {
                    dgvStock.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate5Per", HeaderText = "" + AppSettings.SRate5Name + " %", ReadOnly = false, Visible = false, Width = 80 }); //18
                    dgvStock.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate5", HeaderText = AppSettings.SRate5Name, ReadOnly = false, Visible = false, Width = 80 }); //19
                }
            }
            else
            {
                dgvStock.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate1Per", HeaderText = "" + AppSettings.SRate1Name + " %", ReadOnly = true, Width = 80 }); //10
                dgvStock.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate1", HeaderText = "" + AppSettings.SRate1Name + "", ReadOnly = true, Width = 80 }); //11
                if (AppSettings.IsActiveSRate2 == true)
                {
                    dgvStock.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate2Per", HeaderText = "" + AppSettings.SRate2Name + " %", ReadOnly = true, Visible = true, Width = 80 }); //12
                    dgvStock.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate2", HeaderText = "" + AppSettings.SRate2Name + "", ReadOnly = true, Visible = true, Width = 80 }); //13
                }
                else
                {
                    dgvStock.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate2Per", HeaderText = "" + AppSettings.SRate2Name + " %", ReadOnly = true, Visible = false, Width = 80 }); //12
                    dgvStock.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate2", HeaderText = "" + AppSettings.SRate2Name + "", ReadOnly = true, Visible = false, Width = 80 }); //13
                }

                if (AppSettings.IsActiveSRate3 == true)
                {
                    dgvStock.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate3Per", HeaderText = "" + AppSettings.SRate3Name + " %", ReadOnly = true, Visible = true, Width = 80 }); //14
                    dgvStock.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate3", HeaderText = AppSettings.SRate3Name, ReadOnly = true, Visible = true, Width = 80 }); //15
                }
                else
                {
                    dgvStock.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate3Per", HeaderText = "" + AppSettings.SRate3Name + " %", ReadOnly = true, Visible = false, Width = 80 }); //14
                    dgvStock.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate3", HeaderText = AppSettings.SRate3Name, ReadOnly = true, Visible = false, Width = 80 }); //15
                }

                if (AppSettings.IsActiveSRate4 == true)
                {
                    dgvStock.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate4Per", HeaderText = "" + AppSettings.SRate4Name + " %", ReadOnly = true, Visible = true, Width = 80 }); //16
                    dgvStock.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate4", HeaderText = AppSettings.SRate4Name, ReadOnly = true, Visible = true, Width = 80 }); //17
                }
                else
                {
                    dgvStock.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate4Per", HeaderText = "" + AppSettings.SRate4Name + " %", ReadOnly = true, Visible = false, Width = 80 }); //16
                    dgvStock.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate4", HeaderText = AppSettings.SRate4Name, ReadOnly = true, Visible = false, Width = 80 }); //17
                }

                if (AppSettings.IsActiveSRate5 == true)
                {
                    dgvStock.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate5Per", HeaderText = "" + AppSettings.SRate5Name + " %", ReadOnly = true, Visible = true, Width = 80 }); //18
                    dgvStock.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate5", HeaderText = AppSettings.SRate5Name, ReadOnly = true, Visible = true, Width = 80 }); //19
                }
                else
                {
                    dgvStock.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate5Per", HeaderText = "" + AppSettings.SRate5Name + " %", ReadOnly = true, Visible = false, Width = 80 }); //18
                    dgvStock.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate5", HeaderText = AppSettings.SRate5Name, ReadOnly = true, Visible = false, Width = 80 }); //19
                }
            }
            dgvStock.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cGrossAmt", HeaderText = "Gross Amt", Width = 80, ReadOnly = true }); //23

                dgvStock.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cDiscPer", HeaderText = "Discount %", ReadOnly = true, Visible = false, Width = 80 }); //24
                dgvStock.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cDiscAmount", HeaderText = "Discount Amt", ReadOnly = true, Visible = false, Width = 80 }); //25

                dgvStock.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cBillDisc", HeaderText = "Bill Discount", Width = 80, ReadOnly = true, Visible = false }); //26

            dgvStock.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cCrate", HeaderText = "CRate", Width = 80, ReadOnly = true }); //27

            if (AppSettings.TaxMode == 2) //GST
                dgvStock.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cCRateWithTax", HeaderText = "CRate With Tax", Width = 80, ReadOnly = true }); //28
            else
                dgvStock.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cCRateWithTax", HeaderText = "CRate With Tax", Visible=false, Width = 80, ReadOnly = true }); //28

            dgvStock.Columns.Add(new DataGridViewTextBoxColumn() { Name = "ctaxable", HeaderText = "Taxable", Width = 80, ReadOnly = true }); //29

                if (clsVchTypeFeatures.blnEditTaxPer == true)
                    dgvStock.Columns.Add(new DataGridViewTextBoxColumn() { Name = "ctaxPer", HeaderText = "Tax %", ReadOnly = false, Width = 80, Visible = false }); //30
                else
                    dgvStock.Columns.Add(new DataGridViewTextBoxColumn() { Name = "ctaxPer", HeaderText = "Tax %", ReadOnly = true, Width = 80, Visible = false }); //30

                dgvStock.Columns.Add(new DataGridViewTextBoxColumn() { Name = "ctax", HeaderText = "Tax", Width = 80, ReadOnly = true, Visible = false }); //31
                dgvStock.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cIGST", HeaderText = "IGST", Width = 80, ReadOnly = true, Visible = false }); //32
                dgvStock.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSGST", HeaderText = "SGST", Width = 80, ReadOnly = true, Visible = false }); //33
                dgvStock.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cCGST", HeaderText = "CGST", Width = 80, ReadOnly = true, Visible = false }); //34

            
            dgvStock.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cNetAmount", HeaderText = "Net Amt", Width = 100, ReadOnly = true, Visible = false }); //35
            dgvStock.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cItemID", HeaderText = "ItemID", Visible = false, Width = 80, ReadOnly = true }); //36

            dgvStock.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cGrossValueAfterRateDiscount", HeaderText = "Gross Val", Visible = false, ReadOnly = true }); //37
            dgvStock.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cNonTaxable", HeaderText = "Non Taxable", Visible = false, ReadOnly = true }); //38
            dgvStock.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cCCessPer", HeaderText = "Cess %", Visible = false, ReadOnly = true }); //39
            dgvStock.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cCCompCessQty", HeaderText = "Comp Cess Qty", Visible = false, ReadOnly = true }); //40
            dgvStock.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cFloodCessPer", HeaderText = "Flood Cess %", Visible = false, ReadOnly = true }); //41
            dgvStock.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cFloodCessAmt", HeaderText = "Flood Cess Amt", Visible = false, ReadOnly = true }); //42
            dgvStock.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cStockMRP", HeaderText = "Stock MRP", Visible = false, ReadOnly = true }); //43
            dgvStock.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cAgentCommPer", HeaderText = "Agent Comm. %", Visible = false, ReadOnly = true }); //44
            dgvStock.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cCoolie", HeaderText = "Coolie", Visible = false, ReadOnly = true }); //45
            dgvStock.Columns.Add(new DataGridViewCheckBoxColumn() { Name = "cBlnOfferItem", HeaderText = "Offer Item", Visible = false, ReadOnly = true }); //46
            dgvStock.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cStrOfferDetails", HeaderText = "Offer Det.", Visible = false, ReadOnly = true }); //47
            dgvStock.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cBatchMode", HeaderText = "Batch Mode", Visible = false, ReadOnly = true }); //48
            dgvStock.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cID", HeaderText = "ID", Visible = false, ReadOnly = true });
            dgvStock.Columns.Add(new DataGridViewImageColumn() { Name = "cImgDel", HeaderText="", Image = Properties.Resources.Delete_24_P4, Width=40, ReadOnly = true });
            dgvStock.Columns.Add(new DataGridViewImageColumn() { Name = "cBatchUnique", HeaderText="", Image = Properties.Resources.Delete_24_P4, Width=40, Visible = false, ReadOnly = true });

            //Dipoos 21-03-2022
            //if (iIDFromEditWindow==0)
            //dgvStock.Rows.Add(2);
            //else

            dgvStock.Rows.Add(1);

            foreach (DataGridViewRow row in dgvStock.Rows)
            {
                dgvStock.Rows[row.Index].Cells[0].Value = string.Format("{0}  ", row.Index + 1).ToString();
            }

            foreach (DataGridViewColumn col in dgvStock.Columns)
            {
                col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
        }

        //Description : Initialize for Item Column Width Settings
        private void GridInitialize_dgvColWidth(bool bIsLoad = true, DataGridView dgvStock = null)
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
                            if (dtGridSettings.Rows[k][3].ToString() == dgvStock.Columns[k].Name)
                            {
                                if (dgvStock.Columns[k].Name.ToUpper().Trim() == "cDiscAmount".ToUpper() || dgvStock.Columns[k].Name.ToUpper().Trim() == "cDiscPer".ToUpper() || dgvStock.Columns[k].Name.ToUpper().Trim() == "cBillDisc".ToUpper())
                                {
                                    if (clsVchType.ParentID == 1005)
                                    {
                                        dgvStock.Columns[k].Width = Convert.ToInt32(dtGridSettings.Rows[k][2].ToString());
                                        dgvStock.Columns[k].Visible = false;
                                    }
                                }
                                else if (dgvStock.Columns[k].Name.ToUpper().Trim() == "CFREE")
                                {
                                    if (clsVchTypeFeatures.BLNSHOWFREEQUANTITY == false || clsVchType.ParentID == 1005)
                                    {
                                        dgvStock.Columns[k].Width = Convert.ToInt32(dtGridSettings.Rows[k][2].ToString());
                                        dgvStock.Columns[k].Visible = false;
                                    }
                                }
                                else if (dgvStock.Columns[k].Name.ToUpper().Trim() == "ID")
                                {
                                    dgvStock.Columns[k].Width = Comm.ToInt32(dtGridSettings.Rows[k][2].ToString());
                                    dgvStock.Columns[k].Visible = false;
                                }
                                else if (dgvStock.Columns[k].Name.ToUpper().Trim() == "ItemID")
                                {
                                    dgvStock.Columns[k].Width = Comm.ToInt32(dtGridSettings.Rows[k][2].ToString());
                                    dgvStock.Columns[k].Visible = false;
                                }
                                else if (dgvStock.Columns[k].Name.ToUpper().Trim() == "CSRATE2PER" || dgvStock.Columns[k].Name.ToUpper().Trim() == "CSRATE2")
                                {
                                    if (AppSettings.IsActiveSRate2 == false)
                                    {
                                        dgvStock.Columns[k].Width = Comm.ToInt32(dtGridSettings.Rows[k][2].ToString());
                                        dgvStock.Columns[k].Visible = false;
                                    }
                                    else
                                    {
                                        dgvStock.Columns[k].Width = Comm.ToInt32(dtGridSettings.Rows[k][2].ToString());
                                        dgvStock.Columns[k].Visible = Convert.ToBoolean(dtGridSettings.Rows[k][0]);
                                    }
                                }
                                else if (dgvStock.Columns[k].Name.ToUpper().Trim() == "CSRATE3PER" || dgvStock.Columns[k].Name.ToUpper().Trim() == "CSRATE3")
                                {
                                    if (AppSettings.IsActiveSRate3 == false)
                                    {
                                        dgvStock.Columns[k].Width = Comm.ToInt32(dtGridSettings.Rows[k][2].ToString());
                                        dgvStock.Columns[k].Visible = false;
                                    }
                                    else
                                    {
                                        dgvStock.Columns[k].Width = Comm.ToInt32(dtGridSettings.Rows[k][2].ToString());
                                        dgvStock.Columns[k].Visible = Convert.ToBoolean(dtGridSettings.Rows[k][0]);
                                    }
                                }
                                else if (dgvStock.Columns[k].Name.ToUpper().Trim() == "CSRATE4PER" || dgvStock.Columns[k].Name.ToUpper().Trim() == "CSRATE4")
                                {
                                    if (AppSettings.IsActiveSRate4 == false)
                                    {
                                        dgvStock.Columns[k].Width = Comm.ToInt32(dtGridSettings.Rows[k][2].ToString());
                                        dgvStock.Columns[k].Visible = false;
                                    }
                                    else
                                    {
                                        dgvStock.Columns[k].Width = Comm.ToInt32(dtGridSettings.Rows[k][2].ToString());
                                        dgvStock.Columns[k].Visible = Convert.ToBoolean(dtGridSettings.Rows[k][0]);
                                    }
                                }
                                else if (dgvStock.Columns[k].Name.ToUpper().Trim() == "CSRATE5PER" || dgvStock.Columns[k].Name.ToUpper().Trim() == "CSRATE5")
                                {
                                    if (AppSettings.IsActiveSRate5 == false)
                                    {
                                        dgvStock.Columns[k].Width = Comm.ToInt32(dtGridSettings.Rows[k][2].ToString());
                                        dgvStock.Columns[k].Visible = false;
                                    }
                                    else
                                    {
                                        dgvStock.Columns[k].Width = Comm.ToInt32(dtGridSettings.Rows[k][2].ToString());
                                        dgvStock.Columns[k].Visible = Convert.ToBoolean(dtGridSettings.Rows[k][0]);
                                    }
                                }
                                else
                                {
                                    dgvStock.Columns[k].Width = Comm.ToInt32(dtGridSettings.Rows[k][2].ToString());
                                    dgvStock.Columns[k].Visible = Convert.ToBoolean(dtGridSettings.Rows[k][0]);
                                }
                            }

                        }

                        dgvStock.Columns["cCGST"].Visible = false;
                        dgvStock.Columns["cSGST"].Visible = false;
                        dgvStock.Columns["cIGST"].Visible = false;
                        dgvStock.Columns["ctaxPer"].Visible = false;
                        dgvStock.Columns["ctax"].Visible = false;
                        dgvStock.Columns["ctaxable"].Visible = false;
                        dgvStock.Columns["cCRateWithTax"].Visible = false;
                        dgvStock.Columns["cRateinclusive"].Visible = false;
                        dgvStock.Columns["cItemID"].Visible = false;
                        dgvStock.Columns["cFree"].Visible = false;
                        dgvStock.Columns["cDiscPer"].Visible = false;
                        dgvStock.Columns["cDiscAmount"].Visible = false;
                        dgvStock.Columns["cBillDisc"].Visible = false;
                        dgvStock.Columns["ctaxable"].Visible = false;
                        dgvStock.Columns["ctaxPer"].Visible = false;
                        dgvStock.Columns["ctax"].Visible = false;
                        dgvStock.Columns["cIGST"].Visible = false;
                        dgvStock.Columns["cSGST"].Visible = false;
                        dgvStock.Columns["cCGST"].Visible = false;
                        dgvStock.Columns["cGrossValueAfterRateDiscount"].Visible = false;
                        dgvStock.Columns["cNonTaxable"].Visible = false;
                        dgvStock.Columns["cCCessPer"].Visible = false;
                        dgvStock.Columns["cCCompCessQty"].Visible = false;
                        dgvStock.Columns["cFloodCessPer"].Visible = false;
                        dgvStock.Columns["cFloodCessAmt"].Visible = false;
                        dgvStock.Columns["cStockMRP"].Visible = false;
                        dgvStock.Columns["cAgentCommPer"].Visible = false;
                        dgvStock.Columns["cCoolie"].Visible = false;
                        dgvStock.Columns["cBlnOfferItem"].Visible = false;
                        dgvStock.Columns["cStrOfferDetails"].Visible = false;
                        dgvStock.Columns["cBatchMode"].Visible = false;
                        dgvStock.Columns["cBatchUnique"].Visible = false;
                        dgvStock.Columns["cID"].Visible = false;
                    }
                }
                //LoadGridWidthFromItemGrid();
            }
            else
            {
                //LoadGridWidthFromItemGrid();
                //SaveGridSettings();
            }
            
            dgvStock.Columns["cRateinclusive"].Visible = false;

            dgvStock.Columns["cSlNo"].Frozen = true;
            dgvStock.Columns["cSlNo"].ReadOnly = true;
            //dgvStock.Columns["cImgDel"].Frozen = true;
            dgvStock.Columns["cImgDel"].Visible = true;
            dgvStock.Columns["cImgDel"].Width = 40;

            //DisableGridSettingsCheckbox();
        }

        private void txtDiscPerc_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                e.Handled = Comm.CheckNumeric(sender, e);
            }
            catch
            {

            }
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

        private void frmRepacking_Activated(object sender, EventArgs e)
        {

        }


        private void dgvColWidth_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            
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

        private void picBackground_Click(object sender, EventArgs e)
        {

        }

        private void picBackground_LoadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            try
            {
                this.BackgroundImageLayout = ImageLayout.Stretch;
                this.BackgroundImage = (Bitmap)picBackground.Image.Clone();
            }
            catch
            { }
        }

        private void frmRepacking_Shown(object sender, EventArgs e)
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

        private void btnPrint_Click(object sender, EventArgs e)
        {

        }

        private void dgvStockOut_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (dgvStockOut.CurrentCell.ColumnIndex == GetEnum(GridColIndexes.CExpiry))
                {
                    if (dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(GridColIndexes.CExpiry)].ReadOnly == false)
                    {
                        _Rectangle = dgvStockOut.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true); //  
                        dtpOut.Size = new Size(_Rectangle.Width, _Rectangle.Height); //  
                        dtpOut.Location = new Point(_Rectangle.X, _Rectangle.Y); //  
                        dtpOut.Visible = true;
                        dtpOut.TextChanged += new EventHandler(dtpOut_TextChange);
                    }
                }
                if (dgvStockOut.CurrentCell.ColumnIndex == GetEnum(GridColIndexes.cSlNo))
                {
                    dgvStockOut.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                    dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cSlNo)].ReadOnly = true;

                    strSelectedItemName = Convert.ToString(dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(GridColIndexes.CItemCode)].Value);
                }
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void dgvStockOut_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvStockOut.CurrentCell.ColumnIndex == GetEnum(GridColIndexes.cImgDel))
            {
                string SSelectedItemCode = Convert.ToString(dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(GridColIndexes.CItemCode)].Value);
                if (SSelectedItemCode != "")
                {
                    DialogResult dlgResult = MessageBox.Show("Are you sure to delete Item Code[" + SSelectedItemCode + "] Permanently ?", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                    if (dlgResult == DialogResult.Yes)
                    {

                        Int32 selectedRowCount = dgvStockOut.Rows.GetRowCount(DataGridViewElementStates.Selected);
                        RowDeleteOut();

                        dgvStockOut.Rows.Add();
                        dgvStockOut.CurrentCell = dgvStockOut.Rows[0].Cells[GetEnum(GridColIndexes.CItemCode)];

                        CalcTotalOut();

                    }
                }
            }
        }

        private void dgvStockOut_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                string sQuery = "";
                Cursor.Current = Cursors.WaitCursor;
                double dSelectedItemID = 0;
                if (dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(GridColIndexes.CItemCode)].Value != null)
                {
                    dSelectedItemID = Comm.ToDouble(dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cItemID)].Value);
                    if (dSelectedItemID > 0)
                    {
                        if (dgvStockOut.CurrentCell.ColumnIndex == (int)GridColIndexes.CItemCode)
                        {
                            frmItemMaster frmIM = new frmItemMaster(Comm.ToInt32(dSelectedItemID), true, "E");
                            frmIM.ShowDialog();
                        }
                        else if (dgvStockOut.CurrentCell.ColumnIndex == (int)GridColIndexes.CItemName)
                        {
                            frmItemMaster frmIM = new frmItemMaster(Comm.ToInt32(dSelectedItemID), true, "E");
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

        private void dgvStockOut_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            decimal dResult = 0;
            try
            {
                if (dgvStockOut.CurrentCell.ColumnIndex == GetEnum(GridColIndexes.cQty))
                {
                    dResult = Comm.ToDecimal(dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cQty)].Value);
                    SetValueOut(GetEnum(GridColIndexes.cQty), dResult.ToString(), "QTY_FLOAT");

                    if (dgvStockOut.Rows.Count - 1 == dgvStockOut.CurrentRow.Index)
                        dgvStockOut.Rows.Add();

                    //Added by Anjitha 28/01/2022 5:30 PM
                    bool bshellife = ShelfLifeEffect();
                    if (bshellife == false)
                    {
                        dgvStockOut.Focus();
                        SetValueOut(GetEnum(GridColIndexes.CExpiry), dtCurrExp.ToString("dd-MMM-yyyy").ToString(), "DATE");
                    }

                    SendKeys.Send("{up}");
                    SendKeys.Send("{right}");
                }
                else if (dgvStockOut.CurrentCell.ColumnIndex == GetEnum(GridColIndexes.cFree))
                {
                    dResult = Comm.ToDecimal(dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cFree)].Value);
                    SetValueOut(GetEnum(GridColIndexes.cFree), dResult.ToString(), "QTY_FLOAT");

                    SendKeys.Send("{up}");
                    SendKeys.Send("{right}");
                }
                else if (dgvStockOut.CurrentCell.ColumnIndex == GetEnum(GridColIndexes.cDiscPer))
                {
                    dResult = Comm.ToDecimal(dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cGrossAmt)].Value) * (Comm.ToDecimal(dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cDiscPer)].Value) / 100);

                    dgvStockOut.CellEndEdit -= dgvStockOut_CellEndEdit;
                    SetValueOut(GetEnum(GridColIndexes.cDiscAmount), dResult.ToString(), "CURR_FLOAT");
                    dgvStockOut.CellEndEdit += dgvStockOut_CellEndEdit;

                    SendKeys.Send("{up}");
                    SendKeys.Send("{right}");
                }
                else if (dgvStockOut.CurrentCell.ColumnIndex == GetEnum(GridColIndexes.cDiscAmount))
                {
                    dResult = (Comm.ToDecimal(dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cDiscAmount)].Value) * 100) / Comm.ToDecimal(dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cGrossAmt)].Value);

                    dgvStockOut.CellEndEdit -= dgvStockOut_CellEndEdit;
                    SetValueOut(GetEnum(GridColIndexes.cDiscPer), Comm.FormatAmt(dResult, ""), "");
                    dgvStockOut.CellEndEdit += dgvStockOut_CellEndEdit;

                    SendKeys.Send("{up}");
                    SendKeys.Send("{right}");
                }
                else if (dgvStockOut.CurrentCell.ColumnIndex == GetEnum(GridColIndexes.cMRP))
                {
                    SendKeys.Send("{up}");
                    SendKeys.Send("{right}");
                }
                else if (dgvStockOut.CurrentCell.ColumnIndex == GetEnum(GridColIndexes.cPrate))
                {
                    SendKeys.Send("{up}");
                    SendKeys.Send("{right}");
                }
                else if (dgvStockOut.CurrentCell.ColumnIndex == GetEnum(GridColIndexes.cSRate1Per))
                {
                    SendKeys.Send("{up}");
                    SendKeys.Send("{right}");
                }
                else if (dgvStockOut.CurrentCell.ColumnIndex == GetEnum(GridColIndexes.cSRate2Per))
                {
                    SendKeys.Send("{up}");
                    SendKeys.Send("{right}");
                }
                else if (dgvStockOut.CurrentCell.ColumnIndex == GetEnum(GridColIndexes.cSRate3Per))
                {
                    SendKeys.Send("{up}");
                    SendKeys.Send("{right}");
                }
                else if (dgvStockOut.CurrentCell.ColumnIndex == GetEnum(GridColIndexes.cSRate4Per))
                {
                    SendKeys.Send("{up}");
                    SendKeys.Send("{right}");
                }
                else if (dgvStockOut.CurrentCell.ColumnIndex == GetEnum(GridColIndexes.cSRate5Per))
                {
                    SendKeys.Send("{up}");
                    SendKeys.Send("{right}");
                }
                else if (dgvStockOut.CurrentCell.ColumnIndex == GetEnum(GridColIndexes.cSRate1))
                {
                    SendKeys.Send("{up}");
                    SendKeys.Send("{right}");
                }
                else if (dgvStockOut.CurrentCell.ColumnIndex == GetEnum(GridColIndexes.cSRate2))
                {
                    SendKeys.Send("{up}");
                    SendKeys.Send("{right}");
                }
                else if (dgvStockOut.CurrentCell.ColumnIndex == GetEnum(GridColIndexes.cSRate3))
                {
                    SendKeys.Send("{up}");
                    SendKeys.Send("{right}");
                }
                else if (dgvStockOut.CurrentCell.ColumnIndex == GetEnum(GridColIndexes.cSRate4))
                {
                    SendKeys.Send("{up}");
                    SendKeys.Send("{right}");
                }
                else if (dgvStockOut.CurrentCell.ColumnIndex == GetEnum(GridColIndexes.cSRate5))
                {
                    SendKeys.Send("{up}");
                    SendKeys.Send("{right}");
                }



                #region "Srate Calculation on vale changing in cells"

                //If the tag value of srate colums are 1, it won't get calculated in calctotal function.
                //Else srate(s) will be forward calculated according to percentages
                //Tag vale is set to "1" if the user enters vale in srate columns.

                if (dgvStockOut.Rows[dgvStockOut.CurrentCell.RowIndex].Cells[GetEnum(GridColIndexes.cSRate1)].Tag == null)
                    dgvStockOut.Rows[dgvStockOut.CurrentCell.RowIndex].Cells[GetEnum(GridColIndexes.cSRate1)].Tag = "";
                if (dgvStockOut.Rows[dgvStockOut.CurrentCell.RowIndex].Cells[GetEnum(GridColIndexes.cSRate2)].Tag == null)
                    dgvStockOut.Rows[dgvStockOut.CurrentCell.RowIndex].Cells[GetEnum(GridColIndexes.cSRate2)].Tag = "";
                if (dgvStockOut.Rows[dgvStockOut.CurrentCell.RowIndex].Cells[GetEnum(GridColIndexes.cSRate3)].Tag == null)
                    dgvStockOut.Rows[dgvStockOut.CurrentCell.RowIndex].Cells[GetEnum(GridColIndexes.cSRate3)].Tag = "";
                if (dgvStockOut.Rows[dgvStockOut.CurrentCell.RowIndex].Cells[GetEnum(GridColIndexes.cSRate4)].Tag == null)
                    dgvStockOut.Rows[dgvStockOut.CurrentCell.RowIndex].Cells[GetEnum(GridColIndexes.cSRate4)].Tag = "";
                if (dgvStockOut.Rows[dgvStockOut.CurrentCell.RowIndex].Cells[GetEnum(GridColIndexes.cSRate5)].Tag == null)
                    dgvStockOut.Rows[dgvStockOut.CurrentCell.RowIndex].Cells[GetEnum(GridColIndexes.cSRate5)].Tag = "";

                if (dgvStockOut.CurrentCell.ColumnIndex == GetEnum(GridColIndexes.cSRate1Per))
                    SetTagOut(GetEnum(GridColIndexes.cSRate1), dgvStockOut.CurrentRow.Index, "");
                else if (dgvStockOut.CurrentCell.ColumnIndex == GetEnum(GridColIndexes.cSRate2Per))
                    SetTagOut(GetEnum(GridColIndexes.cSRate2), dgvStockOut.CurrentRow.Index, "");
                else if (dgvStockOut.CurrentCell.ColumnIndex == GetEnum(GridColIndexes.cSRate3Per))
                    SetTagOut(GetEnum(GridColIndexes.cSRate3), dgvStockOut.CurrentRow.Index, "");
                else if (dgvStockOut.CurrentCell.ColumnIndex == GetEnum(GridColIndexes.cSRate4Per))
                    SetTagOut(GetEnum(GridColIndexes.cSRate4), dgvStockOut.CurrentRow.Index, "");
                else if (dgvStockOut.CurrentCell.ColumnIndex == GetEnum(GridColIndexes.cSRate5Per))
                    SetTagOut(GetEnum(GridColIndexes.cSRate5), dgvStockOut.CurrentRow.Index, "");

                if (dgvStockOut.CurrentCell.ColumnIndex == GetEnum(GridColIndexes.cSRate1))
                {
                    SetValueOut(GetEnum(GridColIndexes.cSRate1Per), dgvStockOut.CurrentRow.Index, CalculateSratePercentageOnSrate(dgvStockOut.CurrentCell.RowIndex, dgvStockOut.CurrentCell.ColumnIndex));
                    SetTagOut(GetEnum(GridColIndexes.cSRate1), dgvStockOut.CurrentRow.Index, "1");
                }
                else if (dgvStockOut.CurrentCell.ColumnIndex == GetEnum(GridColIndexes.cSRate2))
                {
                    SetValueOut(GetEnum(GridColIndexes.cSRate2Per), dgvStockOut.CurrentRow.Index, CalculateSratePercentageOnSrate(dgvStockOut.CurrentCell.RowIndex, dgvStockOut.CurrentCell.ColumnIndex));
                    SetTagOut(GetEnum(GridColIndexes.cSRate2), dgvStockOut.CurrentRow.Index, "1");
                }
                else if (dgvStockOut.CurrentCell.ColumnIndex == GetEnum(GridColIndexes.cSRate3))
                {
                    SetValueOut(GetEnum(GridColIndexes.cSRate3Per), dgvStockOut.CurrentRow.Index, CalculateSratePercentageOnSrate(dgvStockOut.CurrentCell.RowIndex, dgvStockOut.CurrentCell.ColumnIndex));
                    SetTagOut(GetEnum(GridColIndexes.cSRate3), dgvStockOut.CurrentRow.Index, "1");
                }
                else if (dgvStockOut.CurrentCell.ColumnIndex == GetEnum(GridColIndexes.cSRate4))
                {
                    SetValueOut(GetEnum(GridColIndexes.cSRate4Per), dgvStockOut.CurrentRow.Index, CalculateSratePercentageOnSrate(dgvStockOut.CurrentCell.RowIndex, dgvStockOut.CurrentCell.ColumnIndex));
                    SetTagOut(GetEnum(GridColIndexes.cSRate4), dgvStockOut.CurrentRow.Index, "1");
                }
                else if (dgvStockOut.CurrentCell.ColumnIndex == GetEnum(GridColIndexes.cSRate5))
                {
                    SetValueOut(GetEnum(GridColIndexes.cSRate5Per), dgvStockOut.CurrentRow.Index, CalculateSratePercentageOnSrate(dgvStockOut.CurrentCell.RowIndex, dgvStockOut.CurrentCell.ColumnIndex));
                    SetTagOut(GetEnum(GridColIndexes.cSRate5), dgvStockOut.CurrentRow.Index, "1");
                }

                #endregion

                this.dgvEndEditCell = dgvStockOut[e.ColumnIndex, e.RowIndex];
                if (dgvStockOut.Rows.Count == e.RowIndex && e.ColumnIndex != dgvStockOut.Columns.Count - 1 && e.ColumnIndex <= GetEnum(GridColIndexes.cDiscAmount))
                {
                    if (dgvStockOut.CurrentCell.ColumnIndex != GetEnum(GridColIndexes.cRateinclusive))
                        SendKeys.Send("{Tab}");
                }
                else if (e.ColumnIndex == GetEnum(GridColIndexes.cDiscAmount))
                {
                    dgvStockOut.CurrentCell = dgvStockOut[GetEnum(GridColIndexes.CItemCode), e.RowIndex + 1];
                }
                else if (e.ColumnIndex >= GetEnum(GridColIndexes.cSRate1Per) && e.ColumnIndex < GetEnum(GridColIndexes.cDiscAmount))
                {
                    //SendKeys.Send("{up}");
                    //SendKeys.Send("{right}");
                }
                CalcTotalOut();
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
        }

        private void dgvStockOut_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                int iRow = dgvStockOut.CurrentCell.RowIndex;
                int iCol = dgvStockOut.CurrentCell.ColumnIndex;
                if (dgvStockOut.Columns[dgvStockOut.CurrentCell.ColumnIndex].Index == GetEnum(GridColIndexes.cNetAmount))
                {
                    dgvStockOut.CurrentCell = dgvStockOut[1, dgvStockOut.CurrentCell.RowIndex + 1];
                }
                else if (dgvStockOut.Columns[dgvStockOut.CurrentCell.ColumnIndex].Index == GetEnum(GridColIndexes.cSlNo))
                {
                    dgvStockOut.CurrentCell = dgvStockOut[1, dgvStockOut.CurrentCell.RowIndex + 1];
                }
                else if (dgvStockOut.Columns[dgvStockOut.CurrentCell.ColumnIndex].Index == GetEnum(GridColIndexes.cImgDel))
                {
                    dgvStockOut.CurrentCell = dgvStockOut[1, dgvStockOut.CurrentCell.RowIndex + 1];
                }
            }
            catch
            { }
        }

        private void dgvStockOut_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                int iRow = dgvStockOut.CurrentCell.RowIndex;
                int iCol = dgvStockOut.CurrentCell.ColumnIndex;
                if (dgvStockOut.Columns[dgvStockOut.CurrentCell.ColumnIndex].Index == GetEnum(GridColIndexes.cNetAmount))
                {
                    dgvStockOut.CellValidated -= dgvStockOut_CellValidated;
                    dgvStockOut.CurrentCell = dgvStockOut[1, dgvStockOut.CurrentCell.RowIndex + 1];
                    dgvStockOut.CellValidated += dgvStockOut_CellValidated;
                }
                else if (dgvStockOut.Columns[dgvStockOut.CurrentCell.ColumnIndex].Index == GetEnum(GridColIndexes.cSlNo))
                {
                    dgvStockOut.CellValidated -= dgvStockOut_CellValidated;
                    dgvStockOut.CurrentCell = dgvStockOut[1, dgvStockOut.CurrentCell.RowIndex + 1];
                    dgvStockOut.CellValidated += dgvStockOut_CellValidated;
                }
                else if (dgvStockOut.Columns[dgvStockOut.CurrentCell.ColumnIndex].Index == GetEnum(GridColIndexes.cImgDel))
                {
                    dgvStockOut.CellValidated -= dgvStockOut_CellValidated;
                    dgvStockOut.CurrentCell = dgvStockOut[1, dgvStockOut.CurrentCell.RowIndex + 1];
                    dgvStockOut.CellValidated += dgvStockOut_CellValidated;
                }
            }
            catch
            { }
        }

        private void dgvStockOut_ColumnWidthChanged(object sender, DataGridViewColumnEventArgs e)
        {
            try
            {
                if (this.ActiveControl == null) return;
                if (this.ActiveControl.Name != dgvStockOut.Name) return;
            }
            catch
            { }

            try
            {
                dtp.Visible = false;
                if (toggleWidthSettings.ToggleState == Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active)
                {
                    GridInitialize_dgvColWidth(false, dgvStockOut);
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

        private void dgvStockOut_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            try
            {
                if (dgvStockOut.CurrentCell.EditType == typeof(DataGridViewTextBoxEditingControl))
                {
                    if (dgvStockOut.CurrentCell.ColumnIndex == GetEnum(GridColIndexes.CItemCode))
                    {
                        DataGridViewTextBoxEditingControl tb = (DataGridViewTextBoxEditingControl)e.Control;
                        tb.KeyPress += new KeyPressEventHandler(dgvStockOut_TextBox_KeyPress);
                        e.Control.KeyPress += new KeyPressEventHandler(dgvStockOut_TextBox_KeyPress);
                    }
                    else if (dgvStockOut.CurrentCell.ColumnIndex == GetEnum(GridColIndexes.cBarCode))
                    {
                        CallBatchCodeCompactOut();

                        if (dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(GridColIndexes.CExpiry)].Visible == true)
                            dgvStockOut.CurrentCell = dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(GridColIndexes.CExpiry)];
                        else
                            dgvStockOut.CurrentCell = dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cQty)];

                        dgvStockOut.Focus();
                    }
                    else if (dgvStockOut.CurrentCell.ColumnIndex >= GetEnum(GridColIndexes.cMRP) && dgvStockOut.CurrentCell.ColumnIndex < GetEnum(GridColIndexes.cNetAmount))
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

        private void dgvStockOut_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Shift && e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    int iColumn = dgvStockOut.CurrentCell.ColumnIndex;
                    int iRow = dgvStockOut.CurrentCell.RowIndex;
                    if (iColumn == GetEnum(GridColIndexes.cRateinclusive))
                    {
                        if (dgvStockOut[GetEnum(GridColIndexes.cRateinclusive) - 1, iRow].Visible == true)
                            dgvStockOut.CurrentCell = dgvStockOut[GetEnum(GridColIndexes.cRateinclusive) - 1, iRow];
                        else
                            dgvStockOut.CurrentCell = dgvStockOut[1, iRow - 1];
                    }
                    else if (iColumn == dgvStockOut.Columns.Count - 1 && iRow != dgvStockOut.Rows.Count)
                        dgvStockOut.CurrentCell = dgvStockOut[1, iRow - 1];
                    else
                        SendKeys.Send("+{Tab}");
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    int iColumn = dgvStockOut.CurrentCell.ColumnIndex;
                    int iRow = dgvStockOut.CurrentCell.RowIndex;
                    if (iColumn == dgvStockOut.Columns.Count - 1 && iRow != dgvStockOut.Rows.Count)
                    {
                        dgvStockOut.CurrentCell = dgvStockOut[1, iRow + 1];
                    }
                    else if (iColumn == dgvStockOut.Columns.Count - 1 && iRow == dgvStockOut.Rows.Count)
                    {
                    }
                    else if (iColumn == GetEnum(GridColIndexes.cDiscAmount))
                    {
                        //Dipoos 22-03-2022----- >
                        dgvStockOut.Rows.Add();
                        dgvStockOut.CurrentCell = dgvStockOut[GetEnum(GridColIndexes.CItemCode), iRow + 1];
                    }
                    else if (iColumn == GetEnum(GridColIndexes.cRateinclusive))
                    {
                        if (dgvStockOut[GetEnum(GridColIndexes.cRateinclusive) + 1, iRow].Visible == true)
                            dgvStockOut.CurrentCell = dgvStockOut[GetEnum(GridColIndexes.cRateinclusive) + 1, iRow];
                    }
                    else
                    {
                        SendKeys.Send("{Tab}");
                        //SendKeys.Send("{up}");
                        //SendKeys.Send("{right}");
                    }
                }
                else if (e.KeyCode == Keys.Delete)
                {
                    string SSelectedItemCode = Convert.ToString(dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(GridColIndexes.CItemCode)].Value);
                    if (SSelectedItemCode != "")
                    {
                        DialogResult dlgResult = MessageBox.Show("Are you sure to delete Item Code[" + SSelectedItemCode + "] Permanently ?", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                        if (dlgResult == DialogResult.Yes)
                        {
                            Int32 selectedRowCount = dgvStockOut.Rows.GetRowCount(DataGridViewElementStates.Selected);
                            RowDeleteOut();

                            if (dgvStockOut.Rows.Count < 1)
                                dgvStockOut.Rows.Add();

                            CalcTotalOut();

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

                    if (dgvStockOut.CurrentCell.ColumnIndex == (int)GridColIndexes.CItemCode)
                    {
                        if (sEditedValueonKeyPress != null)
                        {
                            if (AppSettings.TaxMode == 2) //GST
                            {
                                sQuery = "SELECT ItemCode + ItemName + BatchUnique + CAST(MRP AS varchar) AS anywhere, ItemCode, ItemName, BatchUnique, QOH, MRP, ItemID, StockID " +
                                        " FROM     vwCompactSearchPurchase Where isnull(ActiveStatus, 1) = 1 and isnull(StockActiveStatus, 1) = 1 ";
                                if (clsVchType.ProductClassList != "")
                                    sQuery = sQuery + " AND ProductTypeID IN (" + clsVchType.ProductClassList + ")";
                                if (clsVchType.ItemCategoriesList != "")
                                    sQuery = sQuery + " AND CategoryID IN (" + clsVchType.ItemCategoriesList + ")";
                                //new frmDetailedSearch2(GetFromItemSearchOut, sQuery, "Anywhere|ItemCode|ItemName|BatchUnique", dgvStockOut.Location.X  + ((dgvStockOut.Width / 2) - 500), dgvStockOut.Location.Y, 7, 0, dgvStockOut.CurrentCell.Value.ToString(), 6, 0, "ORDER BY ItemCode ASC, Batchunique", 0, 0, "Item Search...", 0, "150,250,150,150,150,0,0", true, "frmItemMaster", 20, false, this.MdiParent, 6).ShowDialog(this.MdiParent);

                                if (mblnInitialisedSubWindow == false)
                                {
                                    mblnInitialisedSubWindow = true;
                                    string enteredtext = "";
                                    if (dgvStockOut.CurrentCell.Value != null)
                                        enteredtext = dgvStockOut.CurrentCell.Value.ToString();
                                    //frmDetailedSearch2 frmN = new frmDetailedSearch2(GetFromItemSearchOut, sQuery, "Anywhere|ItemCode|ItemName|BatchUnique", (this.Width / 2), tableLayoutPanel3.Location.Y + 150, 7, 0, enteredtext, 6, 0, "ORDER BY ItemCode ASC, Batchunique", 0, 0, "Item Search...", 0, "150,250,150,150,150,0,0", true, "frmItemMaster", 20, false, this.MdiParent, 6);
                                    frmDetailedSearch2 frmN = new frmDetailedSearch2(GetFromItemSearchOut, sQuery, "Anywhere|ItemCode|ItemName|BatchUnique", dgvStockIn.Location.X + ((dgvStockIn.Width / 2) - 500), dgvStockIn.Location.Y, 7, 0, enteredtext, 6, 0, "ORDER BY ItemCode ASC, Batchunique", 0, 0, "Item Search...", 0, "150,250,150,150,150,0,0", true, "frmItemMaster", 20, false, this.MdiParent, 6);
                                    frmN.MdiParent = this.MdiParent;
                                    frmN.Show(); //12-SEP-2022
                                }
                            }
                            else
                            {
                                sQuery = "SELECT ItemCode + ItemName + BatchUnique + CAST(MRP AS varchar) AS anywhere, ItemCode, ItemName, BatchUnique, QOH, MRP, ItemID, StockID " +
                                        " FROM     vwCompactSearchPurchase Where isnull(ActiveStatus, 1) = 1 and isnull(StockActiveStatus, 1) = 1 ";
                                if (clsVchType.ProductClassList != "")
                                    sQuery = sQuery + " AND ProductTypeID IN (" + clsVchType.ProductClassList + ")";
                                if (clsVchType.ItemCategoriesList != "")
                                    sQuery = sQuery + " AND CategoryID IN (" + clsVchType.ItemCategoriesList + ")";
                                //new frmDetailedSearch2(GetFromItemSearchOut, sQuery, "Anywhere|ItemCode|ItemName|BatchUnique", dgvStockOut.Location.X  + ((dgvStockOut.Width / 2) - 500), dgvStockOut.Location.Y, 7, 0, dgvStockOut.CurrentCell.Value.ToString(), 6, 0, "ORDER BY ItemCode ASC, Batchunique", 0, 0, "Item Search...", 0, "150,250,150,150,150,0,0", true, "frmItemMaster", 20, false, this.MdiParent, 6);

                                if (mblnInitialisedSubWindow == false)
                                {
                                    mblnInitialisedSubWindow = true;
                                    frmDetailedSearch2 frmN = new frmDetailedSearch2(GetFromItemSearchOut, sQuery, "Anywhere|ItemCode|ItemName|BatchUnique", dgvStockIn.Location.X + ((dgvStockIn.Width / 2) - 500), dgvStockIn.Location.Y, 7, 0, dgvStockOut.CurrentCell.Value.ToString(), 6, 0, "ORDER BY ItemCode ASC, Batchunique", 0, 0, "Item Search...", 0, "150,250,150,150,150,0,0", true, "frmItemMaster", 20, false, this.MdiParent, 6);
                                    frmN.MdiParent = this.MdiParent;
                                    frmN.Show(); //12-SEP-2022
                                }
                            }


                            if (dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cItemID)].Value != null)
                            {
                                this.dgvStockOut.EditingControlShowing -= this.dgvStockOut_EditingControlShowing;

                                if (dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cBarCode)].Visible == true)
                                    dgvStockOut.CurrentCell = dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cBarCode)];
                                else
                                    dgvStockOut.CurrentCell = dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cQty)];

                                dgvStockOut.Focus();
                                this.dgvStockOut.EditingControlShowing += this.dgvStockOut_EditingControlShowing;
                            }
                        }
                    }
                    else if (dgvStockOut.CurrentCell.ColumnIndex == GetEnum(GridColIndexes.cBarCode))
                    {
                        Form fc = Application.OpenForms["frmDetailedSearch2"];
                        if (fc != null)
                        {
                            fcc.Focus();
                            fcc.BringToFront();
                            return;
                        }
                        // BatchCode List Will Work only to MNF and Auto BatchMode Cases... Asper Discuss with Anup sir and Team on 13-May-2022 Evening Meeting.
                        if (Comm.ToInt32(dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cBatchMode)].Value) == 1) // MNF
                            CallBatchCodeCompactOut(true);
                        else if (Comm.ToInt32(dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cBatchMode)].Value) == 2) // Auto
                            CallBatchCodeCompactOut(true);
                    }
                }
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
        }

        private void dgvStockOut_Scroll(object sender, ScrollEventArgs e)
        {
            dtpOut.Visible = false;
        }

        private void dgvStockIn_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                int iRow = dgvStockIn.CurrentCell.RowIndex;
                int iCol = dgvStockIn.CurrentCell.ColumnIndex;
                if (dgvStockIn.Columns[dgvStockIn.CurrentCell.ColumnIndex].Index == GetEnum(GridColIndexes.cNetAmount))
                {
                    dgvStockIn.CurrentCell = dgvStockIn[1, dgvStockIn.CurrentCell.RowIndex + 1];
                }
                else if (dgvStockIn.Columns[dgvStockIn.CurrentCell.ColumnIndex].Index == GetEnum(GridColIndexes.cSlNo))
                {
                    dgvStockIn.CurrentCell = dgvStockIn[1, dgvStockIn.CurrentCell.RowIndex + 1];
                }
                else if (dgvStockIn.Columns[dgvStockIn.CurrentCell.ColumnIndex].Index == GetEnum(GridColIndexes.cImgDel))
                {
                    dgvStockIn.CurrentCell = dgvStockIn[1, dgvStockIn.CurrentCell.RowIndex + 1];
                }
            }
            catch
            { }
        }

        private void dgvStockIn_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                int iRow = dgvStockIn.CurrentCell.RowIndex;
                int iCol = dgvStockIn.CurrentCell.ColumnIndex;
                if (dgvStockIn.Columns[dgvStockIn.CurrentCell.ColumnIndex].Index == GetEnum(GridColIndexes.cNetAmount))
                {
                    dgvStockIn.CellValidated -= dgvStockIn_CellValidated;
                    dgvStockIn.CurrentCell = dgvStockIn[1, dgvStockIn.CurrentCell.RowIndex + 1];
                    dgvStockIn.CellValidated += dgvStockIn_CellValidated;
                }
                else if (dgvStockIn.Columns[dgvStockIn.CurrentCell.ColumnIndex].Index == GetEnum(GridColIndexes.cSlNo))
                {
                    dgvStockIn.CellValidated -= dgvStockIn_CellValidated;
                    dgvStockIn.CurrentCell = dgvStockIn[1, dgvStockIn.CurrentCell.RowIndex + 1];
                    dgvStockIn.CellValidated += dgvStockIn_CellValidated;
                }
                else if (dgvStockIn.Columns[dgvStockIn.CurrentCell.ColumnIndex].Index == GetEnum(GridColIndexes.cImgDel))
                {
                    dgvStockIn.CellValidated -= dgvStockIn_CellValidated;
                    dgvStockIn.CurrentCell = dgvStockIn[1, dgvStockIn.CurrentCell.RowIndex + 1];
                    dgvStockIn.CellValidated += dgvStockIn_CellValidated;
                }
            }
            catch
            { }
        }

        // Description : Disabling the Checkbox of Mandatory fields in Column Width Settings Grid
        private void DisableGridSettingsCheckbox()
        {
            string[] strDisableCol;
            List<string> lstDisableCol = new List<string>();

            lstDisableCol.Add("cSlNo");
            lstDisableCol.Add("CItemCode");

            if (AppSettings.BLNBARCODE == true)
                lstDisableCol.Add("cBarCode");
            
            lstDisableCol.Add("cPrate");
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
                        if (Comm.ToDecimal(dgvColWidth.Rows[RowIndex].Cells[2].Value.ToString()) < 50)
                        {
                            dgvColWidth.Rows[RowIndex].Cells[2].Value = "50";
                            dgvStockIn.ColumnWidthChanged -= dgvStockIn_ColumnWidthChanged;
                            dgvStockIn.Columns[dgvColWidth.Rows[RowIndex].Cells[3].Value.ToString()].Width = 50;
                            dgvStockIn.ColumnWidthChanged += dgvStockIn_ColumnWidthChanged;
                        }
                    }
                    else
                    {
                        if (Comm.ToDecimal(dgvColWidth.Rows[RowIndex].Cells[2].Value.ToString()) < 10)
                        {
                            dgvColWidth.Rows[RowIndex].Cells[2].Value = "50";
                            dgvColWidth.Rows[RowIndex].Cells[0].Value = false;
                            dgvStockIn.ColumnWidthChanged -= dgvStockIn_ColumnWidthChanged;
                            dgvStockIn.Columns[dgvColWidth.Rows[RowIndex].Cells[3].Value.ToString()].Visible = false;
                            dgvStockIn.ColumnWidthChanged += dgvStockIn_ColumnWidthChanged;
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
            int iHideColIndex = 34;
            DataTable dt = new DataTable();

            dt.Clear();
            dt.Columns.Add("Visible");
            dt.Columns.Add("Name");
            dt.Columns.Add("Width");
            dt.Columns.Add("ColName");

            for (int i = 0; i < Enum.GetNames(typeof(GridColIndexes)).Length; i++)
            {
                if (Enum.GetName(typeof(GridColIndexes), i) == "cNetAmount")
                    iHideColIndex = i;

                DataRow drCol = dt.NewRow();

                drCol["Visible"] = true;
                if (iHideColIndex > 0)
                {
                    if (i > iHideColIndex)
                        drCol["Visible"] = false;
                }
                if (dgvStockIn.Columns[i].Visible == false)
                {
                    drCol["Visible"] = false;
                }
                if (dgvStockIn.Columns[i].Width <= 10)
                {
                    drCol["Visible"] = false;
                }

                if (Enum.GetName(typeof(GridColIndexes), i) == "cRateinclusive")
                    drCol["Visible"] = false;

                drCol["Name"] = dgvStockIn.Columns[i].HeaderText; //Enum.GetName(typeof(GridColIndexes), i).Substring(1, Enum.GetName(typeof(GridColIndexes), i).Length - 1);
                if (Enum.GetName(typeof(GridColIndexes), i) == dgvStockIn.Columns[i].Name)
                    drCol["Width"] = dgvStockIn.Columns[i].Width;
                else
                    drCol["Width"] = "100";
                drCol["ColName"] = Enum.GetName(typeof(GridColIndexes), i);
                dt.Rows.Add(drCol);
            }

            dgvColWidth.Columns[0].DataPropertyName = "Visible";
            dgvColWidth.Columns[1].DataPropertyName = "Name";
            dgvColWidth.Columns[2].DataPropertyName = "Width";
            dgvColWidth.Columns[3].DataPropertyName = "ColName";
            dgvColWidth.DataSource = dt;
            dgvColWidth.Rows[8].Visible = false;

            for (int i = 0; i < dgvColWidth.Rows.Count; i++)
            {
                if (dgvColWidth[3, i].Value.ToString() == "cRateinclusive" ||
                    dgvColWidth[3, i].Value.ToString() == "cItemID" ||
                    dgvColWidth[3, i].Value.ToString() == "cFree" ||
                    dgvColWidth[3, i].Value.ToString() == "cDiscPer" ||
                    dgvColWidth[3, i].Value.ToString() == "cDiscAmount" ||
                    dgvColWidth[3, i].Value.ToString() == "cBillDisc" ||
                    dgvColWidth[3, i].Value.ToString() == "ctaxable" ||
                    dgvColWidth[3, i].Value.ToString() == "ctaxPer" ||
                    dgvColWidth[3, i].Value.ToString() == "ctax" ||
                    dgvColWidth[3, i].Value.ToString() == "cIGST" ||
                    dgvColWidth[3, i].Value.ToString() == "cSGST" ||
                    dgvColWidth[3, i].Value.ToString() == "cCGST" ||
                    dgvColWidth[3, i].Value.ToString() == "cGrossValueAfterRateDiscount" ||
                    dgvColWidth[3, i].Value.ToString() == "cNonTaxable" ||
                    dgvColWidth[3, i].Value.ToString() == "cCCessPer" ||
                    dgvColWidth[3, i].Value.ToString() == "cCCompCessQty" ||
                    dgvColWidth[3, i].Value.ToString() == "cFloodCessPer" ||
                    dgvColWidth[3, i].Value.ToString() == "cFloodCessAmt" ||
                    dgvColWidth[3, i].Value.ToString() == "cStockMRP" ||
                    dgvColWidth[3, i].Value.ToString() == "cAgentCommPer" ||
                    dgvColWidth[3, i].Value.ToString() == "cCoolie" ||
                    dgvColWidth[3, i].Value.ToString() == "cBlnOfferItem" ||
                    dgvColWidth[3, i].Value.ToString() == "cStrOfferDetails" ||
                    dgvColWidth[3, i].Value.ToString() == "cBatchMode" ||
                    dgvColWidth[3, i].Value.ToString() == "cBatchUnique" ||
                    dgvColWidth[3, i].Value.ToString() == "ID" || 
                    dgvColWidth[3, i].Value.ToString() == "cID"
                    )
                {
                    dgvColWidth.Rows[i].Visible = false;
                }

                if (dgvColWidth[3, i].Value.ToString() == "cFree")
                {
                    if (clsVchTypeFeatures.BLNSHOWFREEQUANTITY == false || clsVchType.ParentID == 1005)
                    {
                        dgvColWidth.Rows[i].Visible = false;
                    }
                }
                
                if (dgvStockIn.Columns[i].Name.ToUpper().Trim() == "cDiscAmount".ToUpper() || dgvStockIn.Columns[i].Name.ToUpper().Trim() == "cDiscPer".ToUpper() || dgvStockIn.Columns[i].Name.ToUpper().Trim() == "cBillDisc".ToUpper())
                {
                    if (clsVchType.ParentID == 1005)
                    {
                        dgvColWidth.Rows[i].Visible = false;
                    }
                }
            }

            //dgvStockIn.Columns["cRateinclusive"].Visible = false;
        }

        // Description : Save Grid Settings of Json to Voucher Type table
        private void SaveGridSettings()
        {
            string strJson = "";
            clsJsonPurGridSettingsInfo clsJPDGSinfo = new clsJsonPurGridSettingsInfo();
            List<clsJsonPurGridSettingsInfo> lstJPDGSinfo = new List<clsJsonPurGridSettingsInfo>();

            if (dgvColWidth.CurrentCell.ColumnIndex == 0) dgvColWidth.CurrentCell = dgvColWidth[1, dgvColWidth.CurrentCell.RowIndex];

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
                clsJPDGSinfo.iWidth = Comm.ToInt32(dgvColWidth.Rows[i].Cells[2].Value.ToString());
                clsJPDGSinfo.sColName = dgvColWidth.Rows[i].Cells[3].Value.ToString();

                lstJPDGSinfo.Add(clsJPDGSinfo);
            }
            strJson = JsonConvert.SerializeObject(lstJPDGSinfo);
            Comm.fnExecuteNonQuery("UPDATE tblVchType SET GridSettingsJson = '" + strJson + "' WHERE VchTypeID = " + vchtypeID + "");

            Comm.MessageboxToasted(clsVchType.TransactionName + " Settings", "Settings Saved Successfully for " + clsVchType.TransactionName);

        }



        //Description : Calculating Rate Exclusive of the Item
        public double GetRateExclusive(Double Rate, Double TaxPer, Double astPer, Double EdPer = 0, Double Edcess1 = 0, Double Edcess2 = 0)
        {
            double vatExclusiveRate, EDExclusiveRate;
            vatExclusiveRate = (Rate / (1 + (TaxPer / 100) + ((TaxPer * astPer) / (100 * 100))));
            EDExclusiveRate = (vatExclusiveRate / (1 + (EdPer / 100) + ((EdPer * (Edcess1 + Edcess2)) / (100 * 100))));

            return EDExclusiveRate;
        }

        //Description : Calculate the Entire Repacking in each and every Corner
        private void CalcTotal()
        {
            //return;

            double DblItemAgentCommission = 0;
            double DblGrossValueAfterDiscounts = 0;
            double DblGrossValueAfterDiscountsTot = 0;

            double DblGrossValueExclusive = 0;
            double DbltaxableValueAfterItemDiscount = 0;
            double DblNontaxableValue = 0;

            double DbltaxAmount = 0;
            double DbltaxAmountTot = 0;


            double DblcessAmount = 0;
            double DblFloodcessAmount = 0;
            double DblCompcessAmount = 0;
            double DblQtyCellAmt = 0;

            double DblcessAmountTot = 0;
            double DblCompcessAmountTot = 0;
            double DblFloodcessAmountTot = 0;
            double DblQtyCellAmtTot = 0;

            double DblISGTTot = 0;
            double DblSSGTTot = 0;
            double DblCSGTTot = 0;

            double DbltaxableAmountTot = 0;
            double DblNontaxableAmountTot = 0;
            double DblTaxPer = 0;

            double DblCessPer = 0;
            double DblFloodCessPer = 0;
            double DblQtyCessPer = 0;
            double DblItemDiscper = 0;
            double DblItemDiscAmountTot = 0;
            double DblNetAmountTotal = 0;
            double QtyTotal = 0;
            double DblRate = 0;
            double dblQty = 0;

            // Not Available in the Method ------------------ >>
            double DblrateDiscper = 0;
            double DblRateAfterRDiscount = 0;
            double dblTaxPer = 0;
            double dblCessPer = 0;
            double dblQtyCessPer = 0;
            double dblFloodCessPer = 0;
            double DblRateExclusive = 0;
            double dblGrossValue = 0;
            double dblGrossValueTot = 0;
            double dblQtyTot = 0;
            double dblFreeTot = 0;
            double dblGrossValueAfterRateDiscount = 0;
            double DblrateDiscAmt = 0;
            double DblrateDiscAmtTot = 0;
            double dblGrossValueAfterRateDiscountTot = 0;
            double dblItemDiscAmountTot = 0;
            double dblGrossValueAfterDiscounts = 0;
            double dblGrossValueAfterDiscountsTot = 0;
            double dbltaxableValueAfterItemDiscount = 0;
            double dbltaxableValueAfterItemDiscountTot = 0;
            double dbltaxAmount = 0;
            double dbltaxableAmountTot = 0;
            double dblNontaxableAmountTot = 0;
            double dbltaxAmountTot = 0;
            double dblIGSTTot = 0, dblSSGTTot = 0, dblCSGTTot = 0;
            double SavingsofItem = 0;
            double CoolieTotal = 0;
            bool blnCalculateCoolie = false;
            
            for (int i = 0; i < dgvStockIn.Rows.Count; i++)
            {
                SetValue(GetEnum(GridColIndexes.cSlNo), i, (i + 1).ToString());
                if (dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.CItemName)].Value != null)
                {
                    if (dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.CItemName)].Value.ToString() != "")
                    {
                        if (Comm.ToDecimal(dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cItemID)].Value) != 0)
                        {
                            if (dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cQty)].Value == null)
                                SetValue(GetEnum(GridColIndexes.cQty), i, "0");
                            if (dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cFree)].Value == null)
                                SetValue(GetEnum(GridColIndexes.cFree), i, "0");

                            DblRate = Comm.ToDouble(dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cPrate)].Value);
                            //Dipu on 13-May-2022 ---------- >
                            dblQty = Comm.ToDouble(dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cQty)].Value);
                            //dblQty = Comm.ToDouble(dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cQty)].Value) + Comm.ToDouble(dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cFree)].Value);
                            //Dipu on 25-May-2022 -- Free Value Commented
                            QtyTotal = QtyTotal + dblQty;// + Comm.ToDouble(dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cFree)].Value);

                            //SetValue(GetEnum(GridColIndexes.cSlNo), i, (i + 1).ToString());

                            //DblrateDiscper = Comm.ToDouble(dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cRateDiscPer)].Value);
                            DblRateAfterRDiscount = DblRate - (DblRate * DblrateDiscper / 100);

                                CoolieTotal += 0;

                            dblTaxPer = 0;
                            dblCessPer = 0;
                            dblQtyCessPer = 0;
                            if (dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cFloodCessPer)].Value == null)
                                SetValue(GetEnum(GridColIndexes.cFloodCessPer), i, "");

                            //If chkApplyFloodCess.CheckState = CheckState.Checked Then
                            if (dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cFloodCessPer)].Value.ToString() == "")
                                SetValue(GetEnum(GridColIndexes.cFloodCessPer), i, "0");
                            dblFloodCessPer = 0;
                            //End If

                            if (clsVchType.DefaultTaxInclusiveValue == 2)
                                dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cRateinclusive)].Value = true;
                            else if (clsVchType.DefaultTaxInclusiveValue == 3)
                                dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cRateinclusive)].Value = false;

                            if (Convert.ToBoolean(dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cRateinclusive)].Value) == true)
                                DblRateExclusive = GetRateExclusive(DblRateAfterRDiscount, (dblCessPer + dblTaxPer + dblFloodCessPer), 0);
                            else
                                DblRateExclusive = DblRateAfterRDiscount;

                            dblGrossValue = DblRateExclusive * dblQty;
                            SetValue(GetEnum(GridColIndexes.cGrossAmt), i, Comm.FormatValue(dblGrossValue));
                            dblGrossValueTot = dblGrossValueTot + dblGrossValue;
                            dblGrossValueAfterRateDiscount = dblQty * (DblRateExclusive);

                            dblQtyTot += Comm.ToDouble(dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cQty)].Value);
                            dblFreeTot += Comm.ToDouble(dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cFree)].Value);

                            SetValue(GetEnum(GridColIndexes.cGrossValueAfterRateDiscount), i, Comm.FormatValue(dblGrossValueAfterRateDiscount));

                            DblrateDiscAmt = dblQty * (DblRate - DblRateAfterRDiscount);
                            DblrateDiscAmtTot = DblrateDiscAmtTot + DblrateDiscAmt;

                            dblGrossValueAfterRateDiscountTot = dblGrossValueAfterRateDiscountTot + dblGrossValueAfterRateDiscount;
                            //dbltaxableValueAfterItemDiscountTot = dblGrossValueAfterDiscountsTot;

                            if (Comm.ToDouble(dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cDiscPer)].Value) > 0)
                            {
                                SetValue(GetEnum(GridColIndexes.cDiscAmount), i, Comm.FormatValue((dblGrossValueAfterRateDiscount * Comm.ToDouble(dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cDiscPer)].Value) / 100)));
                                dblItemDiscAmountTot = dblItemDiscAmountTot + Comm.ToDouble(dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cDiscAmount)].Value);
                            }
                            else if (Comm.ToDouble(dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cDiscAmount)].Value) > 0)
                            {
                                SetValue(GetEnum(GridColIndexes.cDiscAmount), i, Comm.FormatValue(Comm.ToDouble(dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cDiscAmount)].Value)));
                                dblItemDiscAmountTot = dblItemDiscAmountTot + Comm.ToDouble(dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cDiscAmount)].Value);
                            }

                            dblGrossValueAfterDiscounts = dblGrossValueAfterRateDiscount - Comm.ToDouble(dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cDiscAmount)].Value);
                            dblGrossValueAfterDiscountsTot = dblGrossValueAfterDiscountsTot + dblGrossValueAfterDiscounts;
                            //
                            //Arrived Taxable Value
                            dbltaxableValueAfterItemDiscount = dblGrossValueAfterDiscounts;
                            dbltaxableValueAfterItemDiscountTot = dblGrossValueAfterDiscountsTot;
                            SetValue(GetEnum(GridColIndexes.cGrossValueAfterRateDiscount), i, Comm.FormatValue(dbltaxableValueAfterItemDiscount));

                                dbltaxAmount = 0;
                                DblNontaxableValue = dbltaxableValueAfterItemDiscount;
                                dblNontaxableAmountTot = dblNontaxableAmountTot + dbltaxableValueAfterItemDiscount;
                            //Tax Mode wise Calculation

                                SetValue(GetEnum(GridColIndexes.ctaxable), i, Comm.FormatValue(dbltaxableValueAfterItemDiscount));
                                SetValue(GetEnum(GridColIndexes.cNonTaxable), i, Comm.FormatValue(DblNontaxableValue));
                                //Check Dipu

                                SetValue(GetEnum(GridColIndexes.cCGST), i, "0");
                                SetValue(GetEnum(GridColIndexes.cSGST), i, "0");
                                SetValue(GetEnum(GridColIndexes.cIGST), i, "0");

                                SetValue(GetEnum(GridColIndexes.ctax), i, "0");


                                //DbltaxAmountTot = DbltaxAmountTot + dbltaxAmount;
                                SetValue(GetEnum(GridColIndexes.cNetAmount), i, Comm.FormatValue((dbltaxableValueAfterItemDiscount)));
                                DblNetAmountTotal = DblNetAmountTotal + Comm.ToDouble(dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cNetAmount)].Value);

                            //DbltaxAmountTot = DbltaxAmountTot + dbltaxAmount;
                            //SetValue(GetEnum(GridColIndexes.cNetAmount), i, Comm.FormatValue((dbltaxableValueAfterItemDiscount + dbltaxAmount)));
                            //DblNetAmountTotal = DblNetAmountTotal + Comm.ToDouble(dgvStockIn.Rows[i].Cells[GetEnum(GridColIndexes.cNetAmount)].Value);

                        }
                    }
                }
            }

            //''''''' Bill Dicount Calculation''''''''''''''''''''
            //'First Discount 

            double Discountamount = 0;
            DblNetAmountTotal = 0;
            dbltaxableAmountTot = 0;
            dblNontaxableAmountTot = 0;
            dbltaxAmount = 0;
            dbltaxAmountTot = 0;
            double TotalValueOfFree = 0;
            double BillExpeDisc = 0;
            double Savings = 0;
            double dbltaxableAmount = 0;

            for (int j = 0; j < dgvStockIn.Rows.Count; j++)
            {
                if (dgvStockIn.Rows[j].Cells[GetEnum(GridColIndexes.CItemName)].Value != null)
                {
                    if (dgvStockIn.Rows[j].Cells[GetEnum(GridColIndexes.CItemName)].Value.ToString() != "")
                    {
                        dblTaxPer = 0;
                        dblCessPer = 0;
                        dblQtyCessPer = 0;
                        // check from Settings
                        dblFloodCessPer = 0;

                        SetValue(GetEnum(GridColIndexes.cBillDisc), j, "0");
                        dblGrossValueAfterDiscounts = Comm.ToDouble(dgvStockIn.Rows[j].Cells[GetEnum(GridColIndexes.cGrossValueAfterRateDiscount)].Value);
                        if (dblGrossValueAfterDiscountsTot > 0)
                            SetValue(GetEnum(GridColIndexes.cBillDisc), j, Comm.FormatValue((Discountamount / dblGrossValueAfterDiscountsTot * dblGrossValueAfterDiscounts)));

                        if (Comm.ToDouble(dgvStockIn.Rows[j].Cells[GetEnum(GridColIndexes.ctaxPer)].Value) > 0)
                        {
                            dbltaxableAmount = Comm.ToDouble(dgvStockIn.Rows[j].Cells[GetEnum(GridColIndexes.ctaxable)].Value) - Comm.ToDouble(dgvStockIn.Rows[j].Cells[GetEnum(GridColIndexes.cBillDisc)].Value);
                            DblNontaxableValue = 0;
                        }
                        else
                        {
                            DblNontaxableValue = Comm.ToDouble(dgvStockIn.Rows[j].Cells[GetEnum(GridColIndexes.cNonTaxable)].Value) - Comm.ToDouble(dgvStockIn.Rows[j].Cells[GetEnum(GridColIndexes.cBillDisc)].Value);
                            dbltaxableAmount = 0;
                        }

                        SetValue(GetEnum(GridColIndexes.ctaxable), j, Comm.FormatValue(dbltaxableAmount));
                        SetValue(GetEnum(GridColIndexes.cNonTaxable), j, Comm.FormatValue(DblNontaxableValue));

                        dbltaxAmount = 0;
                        DblcessAmount = 0;
                        DblCompcessAmount = 0;
                        DblFloodcessAmount = 0;

                        SetTag(GetEnum(GridColIndexes.cCCessPer), j, Comm.FormatValue(DblcessAmount, true, "#.00"));
                        SetTag(GetEnum(GridColIndexes.cCCompCessQty), j, Comm.FormatValue(DblCompcessAmount, false));

                        SetValue(GetEnum(GridColIndexes.cFloodCessAmt), j, Comm.FormatValue(DblFloodcessAmount));
                        DblFloodcessAmountTot = DblFloodcessAmountTot + DblFloodcessAmount;
                        DblcessAmountTot = DblcessAmountTot + DblcessAmount;
                        DblCompcessAmountTot = DblCompcessAmountTot + DblCompcessAmount;

                        SetValue(GetEnum(GridColIndexes.ctax), j, Comm.FormatValue(dbltaxAmount));
                        if (Comm.ToDouble(dgvStockIn.Rows[j].Cells[GetEnum(GridColIndexes.ctaxPer)].Value) > 0)
                        {
                            SetValue(GetEnum(GridColIndexes.ctaxable), j, Comm.FormatValue(Comm.ToDouble(dgvStockIn.Rows[j].Cells[GetEnum(GridColIndexes.ctaxable)].Value)));
                            SetValue(GetEnum(GridColIndexes.cNonTaxable), j, "0");
                        }
                        else
                        {
                            SetValue(GetEnum(GridColIndexes.cNonTaxable), j, Comm.FormatValue(Comm.ToDouble(dgvStockIn.Rows[j].Cells[GetEnum(GridColIndexes.cNonTaxable)].Value)));
                            SetValue(GetEnum(GridColIndexes.ctaxable), j, "0");
                        }

                        SetValue(GetEnum(GridColIndexes.ctaxable), j, Comm.FormatValue(dbltaxableAmount));
                        SetValue(GetEnum(GridColIndexes.cNonTaxable), j, Comm.FormatValue(DblNontaxableValue));

                        SetValue(GetEnum(GridColIndexes.cCGST), j, "0");
                        SetValue(GetEnum(GridColIndexes.cSGST), j, "0");
                        SetValue(GetEnum(GridColIndexes.cIGST), j, "0");
                        SetTag(GetEnum(GridColIndexes.cCGST), j, "0"); ;

                        SetTag(GetEnum(GridColIndexes.cSGST), j, "0");
                        SetTag(GetEnum(GridColIndexes.cIGST), j, "0");

                        dblIGSTTot = dblIGSTTot + Comm.ToDouble(dgvStockIn.Rows[j].Cells[GetEnum(GridColIndexes.cIGST)].Value);
                        dblSSGTTot = dblSSGTTot + Comm.ToDouble(dgvStockIn.Rows[j].Cells[GetEnum(GridColIndexes.cSGST)].Value);
                        dblCSGTTot = dblCSGTTot + Comm.ToDouble(dgvStockIn.Rows[j].Cells[GetEnum(GridColIndexes.cCGST)].Value);

                        dbltaxAmountTot = dbltaxAmountTot + dbltaxAmount;
                        // dont know how to format ??

                        dbltaxableAmountTot = dbltaxableAmountTot + Comm.ToDouble(dgvStockIn.Rows[j].Cells[GetEnum(GridColIndexes.ctaxable)].Value);
                        dblNontaxableAmountTot = dblNontaxableAmountTot + Comm.ToDouble(dgvStockIn.Rows[j].Cells[GetEnum(GridColIndexes.cNonTaxable)].Value);

                        //DGVItem.Item(cNetAmount, i).Value = Comm.FormatAmt(Val(DGVItem.Item(ctaxable, i).Value) + Val(DGVItem.Item(cNonTaxable, i).Value) + Val(DGVItem.Item(ctax, i).Value) + Val(DblcessAmount) + Val(DblFloodcessAmount) + Val(DblCompcessAmount), "")
                        //Dont know what is Comm.FormatAmt ->

                        SetValue(GetEnum(GridColIndexes.cNetAmount), j, Comm.FormatValue((Comm.ToDouble(dgvStockIn.Rows[j].Cells[GetEnum(GridColIndexes.ctaxable)].Value) + Comm.ToDouble(dgvStockIn.Rows[j].Cells[GetEnum(GridColIndexes.cNonTaxable)].Value) + Comm.ToDouble(dgvStockIn.Rows[j].Cells[GetEnum(GridColIndexes.ctax)].Value) + DblcessAmount + DblFloodcessAmount + DblCompcessAmount)));
                        DblNetAmountTotal = DblNetAmountTotal + Comm.ToDouble(dgvStockIn.Rows[j].Cells[GetEnum(GridColIndexes.cNetAmount)].Value);

                        //valuation of Free
                        dblQty = Comm.ToDouble(dgvStockIn.Rows[j].Cells[GetEnum(GridColIndexes.cQty)].Value);
                        if (Comm.ToDouble(dgvStockIn.Rows[j].Cells[GetEnum(GridColIndexes.cFree)].Value) > 0)
                        {
                            double PerItemRate = Comm.ToDouble(dgvStockIn.Rows[j].Cells[GetEnum(GridColIndexes.cNetAmount)].Value) - Comm.ToDouble(dgvStockIn.Rows[j].Cells[GetEnum(GridColIndexes.ctax)].Value) / dblQty;
                            TotalValueOfFree = TotalValueOfFree + (PerItemRate * Comm.ToDouble(dgvStockIn.Rows[j].Cells[GetEnum(GridColIndexes.cFree)].Value));
                        }

                        //CALCULATION DECIMAL CHANGING
                        SetValue(GetEnum(GridColIndexes.cDiscAmount), j, Comm.FormatValue(Comm.ToDouble(dgvStockIn.Rows[j].Cells[GetEnum(GridColIndexes.cDiscAmount)].Value)));
                        SetValue(GetEnum(GridColIndexes.cBillDisc), j, Comm.FormatValue(Comm.ToDouble(dgvStockIn.Rows[j].Cells[GetEnum(GridColIndexes.cBillDisc)].Value)));

                        SetValue(GetEnum(GridColIndexes.ctaxable), j, Comm.FormatValue(Comm.ToDouble(dgvStockIn.Rows[j].Cells[GetEnum(GridColIndexes.ctaxable)].Value)));
                        SetTag(GetEnum(GridColIndexes.ctaxable), j, Comm.FormatValue(Comm.ToDouble(dgvStockIn.Rows[j].Cells[GetEnum(GridColIndexes.ctaxable)].Value)));
                        //DGVItem.Item(ctaxable, i).Tag = Format(Val(DGVItem.Item(ctaxable, i).Value), "#0.000000")
                        //Tag ??

                        SetValue(GetEnum(GridColIndexes.ctax), j, "0");
                        SetValue(GetEnum(GridColIndexes.cIGST), j, "0");
                        SetValue(GetEnum(GridColIndexes.cSGST), j, "0");
                        SetValue(GetEnum(GridColIndexes.cCGST), j, "0");
                        SetValue(GetEnum(GridColIndexes.cNetAmount), j, Comm.FormatValue(Comm.ToDouble(dgvStockIn.Rows[j].Cells[GetEnum(GridColIndexes.cNetAmount)].Value)));
                        //SetValue(GetEnum(GridColIndexes.cNetAmount), j, Comm.FormatValue(Comm.ToDouble(dgvStockIn.Rows[j].Cells[GetEnum(GridColIndexes.cNetAmount)].Value)));
                        SetValue(GetEnum(GridColIndexes.cGrossValueAfterRateDiscount), j, Comm.FormatValue(Comm.ToDouble(dgvStockIn.Rows[j].Cells[GetEnum(GridColIndexes.cGrossValueAfterRateDiscount)].Value)));
                        SetValue(GetEnum(GridColIndexes.cNonTaxable), j, Comm.FormatValue(Comm.ToDouble(dgvStockIn.Rows[j].Cells[GetEnum(GridColIndexes.cNonTaxable)].Value)));

                        if (Comm.ToDouble(dgvStockIn.Rows[j].Cells[GetEnum(GridColIndexes.cAgentCommPer)].Value) > 0)
                            DblItemAgentCommission = (DblItemAgentCommission + Comm.ToDouble(dgvStockIn.Rows[j].Cells[GetEnum(GridColIndexes.cNetAmount)].Value) * Comm.ToDouble(dgvStockIn.Rows[j].Cells[GetEnum(GridColIndexes.cAgentCommPer)].Value) / 100);

                    }
                }
            }

            double bALANCEFORrOUNDOFF = Comm.ToDouble(Comm.FormatAmt(DblNetAmountTotal - 0 - Comm.ToDouble(0) + Comm.ToDouble(0), ""));

            double AdditionalCharges = 0;

            //'Tethering to itemwise rate
            double mytaxable = 0;
            double MyPRate = 0;
            double MyQty;
            double perpieceaddcharges;

            for (int k = 0; k < dgvStockIn.Rows.Count; k++)
            {
                if (dgvStockIn.Rows[k].Cells[GetEnum(GridColIndexes.CItemName)].Value != null)
                {
                    if (dgvStockIn.Rows[k].Cells[GetEnum(GridColIndexes.CItemName)].Value.ToString() != "")
                    {
                        //if (Comm.ToDouble(dgvStockIn.Rows[k].Cells[GetEnum(GridColIndexes.CItemName)].Tag) > 0)
                        //{
                        if (dgvStockIn.Rows[k].Cells[GetEnum(GridColIndexes.cQty)].Value == null)
                            SetValue(GetEnum(GridColIndexes.cQty), k, AppSettings.QtyDecimalFormat);
                        if (dgvStockIn.Rows[k].Cells[GetEnum(GridColIndexes.cFree)].Value == null)
                            SetValue(GetEnum(GridColIndexes.cFree), k, AppSettings.QtyDecimalFormat);

                        mytaxable = Comm.ToDouble(dgvStockIn.Rows[k].Cells[GetEnum(GridColIndexes.ctaxable)].Value) + Comm.ToDouble(dgvStockIn.Rows[k].Cells[GetEnum(GridColIndexes.cNonTaxable)].Value);
                        MyPRate = 0;
                        perpieceaddcharges = 0;
                        MyQty = Comm.ToDouble(dgvStockIn.Rows[k].Cells[GetEnum(GridColIndexes.cQty)].Value);// + Comm.ToDouble(dgvStockIn.Rows[k].Cells[GetEnum(GridColIndexes.cFree)].Value);
                                                                                                             //Dipu on 25-May-2022 -- Free Value Commented
                        if ((dbltaxableAmountTot + dblNontaxableAmountTot) > 0)
                        {
                            if (MyQty > 0)
                                perpieceaddcharges = (AdditionalCharges / (dbltaxableAmountTot + dblNontaxableAmountTot) * mytaxable) / (Comm.ToDouble(dgvStockIn.Rows[k].Cells[GetEnum(GridColIndexes.cQty)].Value) + Comm.ToDouble(dgvStockIn.Rows[k].Cells[GetEnum(GridColIndexes.cFree)].Value));
                        }
                        //perpieceaddcharges = (AdditionalCharges / (dbltaxableAmountTot + dblNontaxableAmountTot) * mytaxable) / Comm.ToDouble(dgvStockIn.Rows[k].Cells[GetEnum(GridColIndexes.cQty)].Value);
                        double MyPrateWithtax = 0;

                        if (mytaxable > 0)
                        {
                            //MyPRate = mytaxable / Comm.ToDouble(dgvStockIn.Rows[k].Cells[GetEnum(GridColIndexes.cQty)].Value) + Comm.ToDouble(dgvStockIn.Rows[k].Cells[GetEnum(GridColIndexes.cFree)].Value);
                            MyPRate = mytaxable / Comm.ToDouble(dgvStockIn.Rows[k].Cells[GetEnum(GridColIndexes.cQty)].Value);
                            MyPrateWithtax = (mytaxable + Comm.ToDouble(dgvStockIn.Rows[k].Cells[GetEnum(GridColIndexes.ctax)].Value)) / (Comm.ToDouble(dgvStockIn.Rows[k].Cells[GetEnum(GridColIndexes.cQty)].Value));
                        }

                        //Distributing CommonValues Betweeen Items

                        SetValue(GetEnum(GridColIndexes.cPrate), k, Comm.FormatValue(Comm.ToDouble(dgvStockIn.Rows[k].Cells[GetEnum(GridColIndexes.cPrate)].Value))); //cRate <--> cPrate
                                                                                                                                                                       //DGVItem.Item(cPrate, i).Value = DGVItem.Item(cRate, i).Value
                                                                                                                                                                       //MyPRate = MyPRate; // + perpieceaddcharges;
                                                                                                                                                                       //Added by Dipu on 23-Nov-2021 ---------------->>
                                                                                                                                                                       //MyPrateWithtax = MyPrateWithtax; // + perpieceaddcharges;
                        SetValue(GetEnum(GridColIndexes.cCrate), k, Comm.FormatValue(MyPRate));
                        SetValue(GetEnum(GridColIndexes.cCRateWithTax), k, Comm.FormatValue(MyPrateWithtax));
                        if (MyPRate > 0)
                        {
                            //NotifyIcon("Sales Value Calculation", MyPRate)
                            //MessageBox.Show("Sales Value Calculation (" + MyPRate + ")");
                        }

                        //BLNRECALCULATESalesRatesOnPercentag
                        if (clsVchTypeFeatures.BLNRECALCULATESalesRatesOnPercentage == true)
                        {
                            double dblcSRate1Per = Comm.ToDouble(dgvStockIn.Rows[k].Cells[GetEnum(GridColIndexes.cSRate1Per)].Value);
                            double dblcsRate2Per = Comm.ToDouble(dgvStockIn.Rows[k].Cells[GetEnum(GridColIndexes.cSRate2Per)].Value);
                            double dblcsRate3Per = Comm.ToDouble(dgvStockIn.Rows[k].Cells[GetEnum(GridColIndexes.cSRate3Per)].Value);
                            double dblcsRate4Per = Comm.ToDouble(dgvStockIn.Rows[k].Cells[GetEnum(GridColIndexes.cSRate4Per)].Value);
                            double dblcsRate5Per = Comm.ToDouble(dgvStockIn.Rows[k].Cells[GetEnum(GridColIndexes.cSRate5Per)].Value);

                            double dblcRate = Comm.ToDouble(dgvStockIn.Rows[k].Cells[GetEnum(GridColIndexes.cPrate)].Value);
                            double dblcCRate = Comm.ToDouble(dgvStockIn.Rows[k].Cells[GetEnum(GridColIndexes.cCrate)].Value);
                            double dblcMRP = Comm.ToDouble(dgvStockIn.Rows[k].Cells[GetEnum(GridColIndexes.cMRP)].Value);
                            double dblcCRateWithTax = Comm.ToDouble(dgvStockIn.Rows[k].Cells[GetEnum(GridColIndexes.cCRateWithTax)].Value);

                            if (dgvStockIn.Rows[k].Cells[GetEnum(GridColIndexes.cSRate1)].Tag == null)
                                dgvStockIn.Rows[k].Cells[GetEnum(GridColIndexes.cSRate1)].Tag = "";
                            if (dgvStockIn.Rows[k].Cells[GetEnum(GridColIndexes.cSRate2)].Tag == null)
                                dgvStockIn.Rows[k].Cells[GetEnum(GridColIndexes.cSRate2)].Tag = "";
                            if (dgvStockIn.Rows[k].Cells[GetEnum(GridColIndexes.cSRate3)].Tag == null)
                                dgvStockIn.Rows[k].Cells[GetEnum(GridColIndexes.cSRate3)].Tag = "";
                            if (dgvStockIn.Rows[k].Cells[GetEnum(GridColIndexes.cSRate4)].Tag == null)
                                dgvStockIn.Rows[k].Cells[GetEnum(GridColIndexes.cSRate4)].Tag = "";
                            if (dgvStockIn.Rows[k].Cells[GetEnum(GridColIndexes.cSRate5)].Tag == null)
                                dgvStockIn.Rows[k].Cells[GetEnum(GridColIndexes.cSRate5)].Tag = "";

                            switch (Comm.ToDouble(dgvStockIn.Rows[k].Cells[GetEnum(GridColIndexes.cSRate1Per)].Tag)) //DiscMode
                            {
                                case 0:
                                    if (dblcSRate1Per > 0 && dgvStockIn.Rows[k].Cells[GetEnum(GridColIndexes.cSRate1)].Tag.ToString() == "") SetValue(GetEnum(GridColIndexes.cSRate1), k, Comm.FormatValue((dblcRate + dblcRate * dblcSRate1Per / 100)));
                                    if (dblcsRate2Per > 0 && dgvStockIn.Rows[k].Cells[GetEnum(GridColIndexes.cSRate2)].Tag.ToString() == "") SetValue(GetEnum(GridColIndexes.cSRate2), k, Comm.FormatValue((dblcRate + dblcRate * dblcsRate2Per / 100)));
                                    if (dblcsRate3Per > 0 && dgvStockIn.Rows[k].Cells[GetEnum(GridColIndexes.cSRate3)].Tag.ToString() == "") SetValue(GetEnum(GridColIndexes.cSRate3), k, Comm.FormatValue((dblcRate + dblcRate * dblcsRate3Per / 100)));
                                    if (dblcsRate4Per > 0 && dgvStockIn.Rows[k].Cells[GetEnum(GridColIndexes.cSRate4)].Tag.ToString() == "") SetValue(GetEnum(GridColIndexes.cSRate4), k, Comm.FormatValue((dblcRate + dblcRate * dblcsRate4Per / 100)));
                                    if (dblcsRate5Per > 0 && dgvStockIn.Rows[k].Cells[GetEnum(GridColIndexes.cSRate5)].Tag.ToString() == "") SetValue(GetEnum(GridColIndexes.cSRate5), k, Comm.FormatValue((dblcRate + dblcRate * dblcsRate5Per / 100)));
                                    break;
                                case 3:
                                    if (dblcSRate1Per > 0 && dgvStockIn.Rows[k].Cells[GetEnum(GridColIndexes.cSRate1)].Tag.ToString() == "") SetValue(GetEnum(GridColIndexes.cSRate1), k, Comm.FormatValue(dblcCRate + dblcCRate * dblcSRate1Per / 100));
                                    if (dblcsRate2Per > 0 && dgvStockIn.Rows[k].Cells[GetEnum(GridColIndexes.cSRate2)].Tag.ToString() == "") SetValue(GetEnum(GridColIndexes.cSRate2), k, Comm.FormatValue(dblcCRate + dblcCRate * dblcsRate2Per / 100));
                                    if (dblcsRate3Per > 0 && dgvStockIn.Rows[k].Cells[GetEnum(GridColIndexes.cSRate3)].Tag.ToString() == "") SetValue(GetEnum(GridColIndexes.cSRate3), k, Comm.FormatValue(dblcCRate + dblcCRate * dblcsRate3Per / 100));
                                    if (dblcsRate4Per > 0 && dgvStockIn.Rows[k].Cells[GetEnum(GridColIndexes.cSRate4)].Tag.ToString() == "") SetValue(GetEnum(GridColIndexes.cSRate4), k, Comm.FormatValue(dblcCRate + dblcCRate * dblcsRate4Per / 100));
                                    if (dblcsRate5Per > 0 && dgvStockIn.Rows[k].Cells[GetEnum(GridColIndexes.cSRate5)].Tag.ToString() == "") SetValue(GetEnum(GridColIndexes.cSRate5), k, Comm.FormatValue(dblcCRate + dblcCRate * dblcsRate5Per / 100));
                                    break;
                                case 1:
                                    if (dblcSRate1Per > 0 && dgvStockIn.Rows[k].Cells[GetEnum(GridColIndexes.cSRate1)].Tag.ToString() == "") SetValue(GetEnum(GridColIndexes.cSRate1), k, Comm.FormatValue(dblcMRP - dblcMRP * dblcSRate1Per / 100));
                                    if (dblcsRate2Per > 0 && dgvStockIn.Rows[k].Cells[GetEnum(GridColIndexes.cSRate2)].Tag.ToString() == "") SetValue(GetEnum(GridColIndexes.cSRate2), k, Comm.FormatValue(dblcMRP - dblcMRP * dblcsRate2Per / 100));
                                    if (dblcsRate3Per > 0 && dgvStockIn.Rows[k].Cells[GetEnum(GridColIndexes.cSRate3)].Tag.ToString() == "") SetValue(GetEnum(GridColIndexes.cSRate3), k, Comm.FormatValue(dblcMRP - dblcMRP * dblcsRate3Per / 100));
                                    if (dblcsRate4Per > 0 && dgvStockIn.Rows[k].Cells[GetEnum(GridColIndexes.cSRate4)].Tag.ToString() == "") SetValue(GetEnum(GridColIndexes.cSRate4), k, Comm.FormatValue(dblcMRP - dblcMRP * dblcsRate4Per / 100));
                                    if (dblcsRate5Per > 0 && dgvStockIn.Rows[k].Cells[GetEnum(GridColIndexes.cSRate5)].Tag.ToString() == "") SetValue(GetEnum(GridColIndexes.cSRate5), k, Comm.FormatValue(dblcMRP - dblcMRP * dblcsRate5Per / 100));
                                    break;
                                case 2:
                                    if (dblcSRate1Per > 0 && dgvStockIn.Rows[k].Cells[GetEnum(GridColIndexes.cSRate1)].Tag.ToString() == "") SetValue(GetEnum(GridColIndexes.cSRate1), k, Comm.FormatValue(dblcCRateWithTax + dblcCRateWithTax * dblcSRate1Per / 100));
                                    if (dblcsRate2Per > 0 && dgvStockIn.Rows[k].Cells[GetEnum(GridColIndexes.cSRate2)].Tag.ToString() == "") SetValue(GetEnum(GridColIndexes.cSRate2), k, Comm.FormatValue(dblcCRateWithTax + dblcCRateWithTax * dblcsRate2Per / 100));
                                    if (dblcsRate3Per > 0 && dgvStockIn.Rows[k].Cells[GetEnum(GridColIndexes.cSRate3)].Tag.ToString() == "") SetValue(GetEnum(GridColIndexes.cSRate3), k, Comm.FormatValue(dblcCRateWithTax + dblcCRateWithTax * dblcsRate3Per / 100));
                                    if (dblcsRate4Per > 0 && dgvStockIn.Rows[k].Cells[GetEnum(GridColIndexes.cSRate4)].Tag.ToString() == "") SetValue(GetEnum(GridColIndexes.cSRate4), k, Comm.FormatValue(dblcCRateWithTax + dblcCRateWithTax * dblcsRate4Per / 100));
                                    if (dblcsRate5Per > 0 && dgvStockIn.Rows[k].Cells[GetEnum(GridColIndexes.cSRate5)].Tag.ToString() == "") SetValue(GetEnum(GridColIndexes.cSRate5), k, Comm.FormatValue(dblcCRateWithTax + dblcCRateWithTax * dblcsRate5Per / 100));
                                    break;
                            }

                            dgvStockIn.Rows[k].Cells[GetEnum(GridColIndexes.cSRate1)].Tag = "";
                            dgvStockIn.Rows[k].Cells[GetEnum(GridColIndexes.cSRate2)].Tag = "";
                            dgvStockIn.Rows[k].Cells[GetEnum(GridColIndexes.cSRate3)].Tag = "";
                            dgvStockIn.Rows[k].Cells[GetEnum(GridColIndexes.cSRate4)].Tag = "";
                            dgvStockIn.Rows[k].Cells[GetEnum(GridColIndexes.cSRate5)].Tag = "";
                        }

                        //double SavingsofItem = (Comm.ToDouble(dgvStockIn.Rows[k].Cells[GetEnum(GridColIndexes.cMRP)].Value) * MyQty) - (Val(DGVItem.Item(cRate, i).Value) * MyQty);
                        SavingsofItem = (Comm.ToDouble(dgvStockIn.Rows[k].Cells[GetEnum(GridColIndexes.cMRP)].Value) * MyQty) - (Comm.ToDouble(dgvStockIn.Rows[k].Cells[GetEnum(GridColIndexes.cPrate)].Value) * MyQty); //cRate <--> cPrate
                        if (MyQty > 0) Savings = Savings + SavingsofItem;

                        //}
                    }
                }
            }

            //ItemDiscount and Discount Amount are equal ??
            Savings = Savings + 0 + 0 + 0 - 0;
            // dgv.SetValue(LinkIDs.Savings, Comm.FormatAmt(Val(Val(Savings)), DCSApp.Gdecimal))
        }

        private void CalcTotalOut()
        {
            //return;

            double DblItemAgentCommission = 0;
            double DblGrossValueAfterDiscounts = 0;
            double DblGrossValueAfterDiscountsTot = 0;

            double DblGrossValueExclusive = 0;
            double DbltaxableValueAfterItemDiscount = 0;
            double DblNontaxableValue = 0;

            double DbltaxAmount = 0;
            double DbltaxAmountTot = 0;


            double DblcessAmount = 0;
            double DblFloodcessAmount = 0;
            double DblCompcessAmount = 0;
            double DblQtyCellAmt = 0;

            double DblcessAmountTot = 0;
            double DblCompcessAmountTot = 0;
            double DblFloodcessAmountTot = 0;
            double DblQtyCellAmtTot = 0;

            double DblISGTTot = 0;
            double DblSSGTTot = 0;
            double DblCSGTTot = 0;

            double DbltaxableAmountTot = 0;
            double DblNontaxableAmountTot = 0;
            double DblTaxPer = 0;

            double DblCessPer = 0;
            double DblFloodCessPer = 0;
            double DblQtyCessPer = 0;
            double DblItemDiscper = 0;
            double DblItemDiscAmountTot = 0;
            double DblNetAmountTotal = 0;
            double QtyTotal = 0;
            double DblRate = 0;
            double dblQty = 0;

            // Not Available in the Method ------------------ >>
            double DblrateDiscper = 0;
            double DblRateAfterRDiscount = 0;
            double dblTaxPer = 0;
            double dblCessPer = 0;
            double dblQtyCessPer = 0;
            double dblFloodCessPer = 0;
            double DblRateExclusive = 0;
            double dblGrossValue = 0;
            double dblGrossValueTot = 0;
            double dblQtyTot = 0;
            double dblFreeTot = 0;
            double dblGrossValueAfterRateDiscount = 0;
            double DblrateDiscAmt = 0;
            double DblrateDiscAmtTot = 0;
            double dblGrossValueAfterRateDiscountTot = 0;
            double dblItemDiscAmountTot = 0;
            double dblGrossValueAfterDiscounts = 0;
            double dblGrossValueAfterDiscountsTot = 0;
            double dbltaxableValueAfterItemDiscount = 0;
            double dbltaxableValueAfterItemDiscountTot = 0;
            double dbltaxAmount = 0;
            double dbltaxableAmountTot = 0;
            double dblNontaxableAmountTot = 0;
            double dbltaxAmountTot = 0;
            double dblIGSTTot = 0, dblSSGTTot = 0, dblCSGTTot = 0;
            double SavingsofItem = 0;
            double CoolieTotal = 0;
            bool blnCalculateCoolie = false;

            for (int i = 0; i < dgvStockOut.Rows.Count; i++)
            {
                SetValueOut(GetEnum(GridColIndexes.cSlNo), i, (i + 1).ToString());
                if (dgvStockOut.Rows[i].Cells[GetEnum(GridColIndexes.CItemName)].Value != null)
                {
                    if (dgvStockOut.Rows[i].Cells[GetEnum(GridColIndexes.CItemName)].Value.ToString() != "")
                    {
                        if (Comm.ToDecimal(dgvStockOut.Rows[i].Cells[GetEnum(GridColIndexes.cItemID)].Value) != 0)
                        {
                            if (dgvStockOut.Rows[i].Cells[GetEnum(GridColIndexes.cQty)].Value == null)
                                SetValueOut(GetEnum(GridColIndexes.cQty), i, "0");
                            if (dgvStockOut.Rows[i].Cells[GetEnum(GridColIndexes.cFree)].Value == null)
                                SetValueOut(GetEnum(GridColIndexes.cFree), i, "0");
                            
                            DblRate = Comm.ToDouble(dgvStockOut.Rows[i].Cells[GetEnum(GridColIndexes.cPrate)].Value);
                            //Dipu on 13-May-2022 ---------- >
                            dblQty = Comm.ToDouble(dgvStockOut.Rows[i].Cells[GetEnum(GridColIndexes.cQty)].Value);
                            //dblQty = Comm.ToDouble(dgvStockOut.Rows[i].Cells[GetEnum(GridColIndexes.cQty)].Value) + Comm.ToDouble(dgvStockOut.Rows[i].Cells[GetEnum(GridColIndexes.cFree)].Value);
                            //Dipu on 25-May-2022 -- Free Value Commented
                            QtyTotal = QtyTotal + dblQty;// + Comm.ToDouble(dgvStockOut.Rows[i].Cells[GetEnum(GridColIndexes.cFree)].Value);

                            //SetValueOut(GetEnum(GridColIndexes.cSlNo), i, (i + 1).ToString());

                            //DblrateDiscper = Comm.ToDouble(dgvStockOut.Rows[i].Cells[GetEnum(GridColIndexes.cRateDiscPer)].Value);
                            DblRateAfterRDiscount = DblRate - (DblRate * DblrateDiscper / 100);

                                CoolieTotal += 0;

                            dblTaxPer = 0;
                            dblCessPer = 0;
                            dblQtyCessPer = 0;

                            if (dgvStockOut.Rows[i].Cells[GetEnum(GridColIndexes.cFloodCessPer)].Value == null)
                                SetValueOut(GetEnum(GridColIndexes.cFloodCessPer), i, "0");

                            if (dgvStockOut.Rows[i].Cells[GetEnum(GridColIndexes.cFloodCessPer)].Value.ToString() == "")
                                SetValueOut(GetEnum(GridColIndexes.cFloodCessPer), i, "0");
                            
                            dblFloodCessPer = 0;
                            //End If

                            if (clsVchType.DefaultTaxInclusiveValue == 2)
                                dgvStockOut.Rows[i].Cells[GetEnum(GridColIndexes.cRateinclusive)].Value = true;
                            else if (clsVchType.DefaultTaxInclusiveValue == 3)
                                dgvStockOut.Rows[i].Cells[GetEnum(GridColIndexes.cRateinclusive)].Value = false;

                            if (Convert.ToBoolean(dgvStockOut.Rows[i].Cells[GetEnum(GridColIndexes.cRateinclusive)].Value) == true)
                                DblRateExclusive = GetRateExclusive(DblRateAfterRDiscount, (dblCessPer + dblTaxPer + dblFloodCessPer), 0);
                            else
                                DblRateExclusive = DblRateAfterRDiscount;

                            dblGrossValue = DblRateExclusive * dblQty;
                            SetValueOut(GetEnum(GridColIndexes.cGrossAmt), i, Comm.FormatValue(dblGrossValue));
                            dblGrossValueTot = dblGrossValueTot + dblGrossValue;
                            dblGrossValueAfterRateDiscount = dblQty * (DblRateExclusive);

                            dblQtyTot += Comm.ToDouble(dgvStockOut.Rows[i].Cells[GetEnum(GridColIndexes.cQty)].Value);
                            dblFreeTot += Comm.ToDouble(dgvStockOut.Rows[i].Cells[GetEnum(GridColIndexes.cFree)].Value);

                            SetValueOut(GetEnum(GridColIndexes.cGrossValueAfterRateDiscount), i, Comm.FormatValue(dblGrossValueAfterRateDiscount));

                            DblrateDiscAmt = dblQty * (DblRate - DblRateAfterRDiscount);
                            DblrateDiscAmtTot = DblrateDiscAmtTot + DblrateDiscAmt;

                            dblGrossValueAfterRateDiscountTot = dblGrossValueAfterRateDiscountTot + dblGrossValueAfterRateDiscount;
                            //dbltaxableValueAfterItemDiscountTot = dblGrossValueAfterDiscountsTot;

                            if (Comm.ToDouble(dgvStockOut.Rows[i].Cells[GetEnum(GridColIndexes.cDiscPer)].Value) > 0)
                            {
                                SetValueOut(GetEnum(GridColIndexes.cDiscAmount), i, Comm.FormatValue((dblGrossValueAfterRateDiscount * Comm.ToDouble(dgvStockOut.Rows[i].Cells[GetEnum(GridColIndexes.cDiscPer)].Value) / 100)));
                                dblItemDiscAmountTot = dblItemDiscAmountTot + Comm.ToDouble(dgvStockOut.Rows[i].Cells[GetEnum(GridColIndexes.cDiscAmount)].Value);
                            }
                            else if (Comm.ToDouble(dgvStockOut.Rows[i].Cells[GetEnum(GridColIndexes.cDiscAmount)].Value) > 0)
                            {
                                SetValueOut(GetEnum(GridColIndexes.cDiscAmount), i, Comm.FormatValue(Comm.ToDouble(dgvStockOut.Rows[i].Cells[GetEnum(GridColIndexes.cDiscAmount)].Value)));
                                dblItemDiscAmountTot = dblItemDiscAmountTot + Comm.ToDouble(dgvStockOut.Rows[i].Cells[GetEnum(GridColIndexes.cDiscAmount)].Value);
                            }

                            dblGrossValueAfterDiscounts = dblGrossValueAfterRateDiscount - Comm.ToDouble(dgvStockOut.Rows[i].Cells[GetEnum(GridColIndexes.cDiscAmount)].Value);
                            dblGrossValueAfterDiscountsTot = dblGrossValueAfterDiscountsTot + dblGrossValueAfterDiscounts;
                            //
                            //Arrived Taxable Value
                            dbltaxableValueAfterItemDiscount = dblGrossValueAfterDiscounts;
                            dbltaxableValueAfterItemDiscountTot = dblGrossValueAfterDiscountsTot;
                            SetValueOut(GetEnum(GridColIndexes.cGrossValueAfterRateDiscount), i, Comm.FormatValue(dbltaxableValueAfterItemDiscount));

                                dbltaxAmount = 0;
                                DblNontaxableValue = dbltaxableValueAfterItemDiscount;
                                dblNontaxableAmountTot = dblNontaxableAmountTot + dbltaxableValueAfterItemDiscount;

                            SetValueOut(GetEnum(GridColIndexes.ctaxable), i, Comm.FormatValue(dbltaxableValueAfterItemDiscount));
                            SetValueOut(GetEnum(GridColIndexes.cNonTaxable), i, Comm.FormatValue(DblNontaxableValue));
                            //Check Dipu

                            SetValueOut(GetEnum(GridColIndexes.cCGST), i, "0");
                            SetValueOut(GetEnum(GridColIndexes.cSGST), i, "0");
                            SetValueOut(GetEnum(GridColIndexes.cIGST), i, "0");

                            SetValueOut(GetEnum(GridColIndexes.ctax), i, "0");

                            SetValueOut(GetEnum(GridColIndexes.cNetAmount), i, Comm.FormatValue((dbltaxableValueAfterItemDiscount)));
                            DblNetAmountTotal = DblNetAmountTotal + Comm.ToDouble(dgvStockOut.Rows[i].Cells[GetEnum(GridColIndexes.cNetAmount)].Value);
                        }
                    }
                }
            }

            //''''''' Bill Dicount Calculation''''''''''''''''''''
            //'First Discount 

            double Discountamount = 0;
            DblNetAmountTotal = 0;
            dbltaxableAmountTot = 0;
            dblNontaxableAmountTot = 0;
            dbltaxAmount = 0;
            dbltaxAmountTot = 0;
            double TotalValueOfFree = 0;
            double BillExpeDisc = 0;
            double Savings = 0;
            double dbltaxableAmount = 0;

            for (int j = 0; j < dgvStockOut.Rows.Count; j++)
            {
                if (dgvStockOut.Rows[j].Cells[GetEnum(GridColIndexes.CItemName)].Value != null)
                {
                    if (dgvStockOut.Rows[j].Cells[GetEnum(GridColIndexes.CItemName)].Value.ToString() != "")
                    {
                        dblTaxPer = 0;
                        dblCessPer = 0;
                        dblQtyCessPer = 0;
                        // check from Settings
                        dblFloodCessPer = 0;

                        SetValueOut(GetEnum(GridColIndexes.cBillDisc), j, "0");
                        dblGrossValueAfterDiscounts = Comm.ToDouble(dgvStockOut.Rows[j].Cells[GetEnum(GridColIndexes.cGrossValueAfterRateDiscount)].Value);
                        if (dblGrossValueAfterDiscountsTot > 0)
                            SetValueOut(GetEnum(GridColIndexes.cBillDisc), j, Comm.FormatValue((Discountamount / dblGrossValueAfterDiscountsTot * dblGrossValueAfterDiscounts)));

                        if (Comm.ToDouble(dgvStockOut.Rows[j].Cells[GetEnum(GridColIndexes.ctaxPer)].Value) > 0)
                        {
                            dbltaxableAmount = Comm.ToDouble(dgvStockOut.Rows[j].Cells[GetEnum(GridColIndexes.ctaxable)].Value) - Comm.ToDouble(dgvStockOut.Rows[j].Cells[GetEnum(GridColIndexes.cBillDisc)].Value);
                            DblNontaxableValue = 0;
                        }
                        else
                        {
                            DblNontaxableValue = Comm.ToDouble(dgvStockOut.Rows[j].Cells[GetEnum(GridColIndexes.cNonTaxable)].Value) - Comm.ToDouble(dgvStockOut.Rows[j].Cells[GetEnum(GridColIndexes.cBillDisc)].Value);
                            dbltaxableAmount = 0;
                        }

                        SetValueOut(GetEnum(GridColIndexes.ctaxable), j, Comm.FormatValue(dbltaxableAmount));
                        SetValueOut(GetEnum(GridColIndexes.cNonTaxable), j, Comm.FormatValue(DblNontaxableValue));

                        dbltaxAmount = 0;
                        DblcessAmount = 0;
                        DblCompcessAmount = 0;
                        DblFloodcessAmount = 0;

                        SetTagOut(GetEnum(GridColIndexes.cCCessPer), j, Comm.FormatValue(DblcessAmount, true, "#.00"));
                        SetTagOut(GetEnum(GridColIndexes.cCCompCessQty), j, Comm.FormatValue(DblCompcessAmount, false));

                        SetValueOut(GetEnum(GridColIndexes.cFloodCessAmt), j, Comm.FormatValue(DblFloodcessAmount));
                        DblFloodcessAmountTot = DblFloodcessAmountTot + DblFloodcessAmount;
                        DblcessAmountTot = DblcessAmountTot + DblcessAmount;
                        DblCompcessAmountTot = DblCompcessAmountTot + DblCompcessAmount;

                        //SetValueOut(GetEnum(GridColIndexes.ctaxable), j, Comm.FormatValue(dbltaxableValueAfterItemDiscount));
                        SetValueOut(GetEnum(GridColIndexes.ctaxable), j, Comm.FormatValue(dbltaxableAmount));
                        SetValueOut(GetEnum(GridColIndexes.cNonTaxable), j, Comm.FormatValue(DblNontaxableValue));
                        //Check Dipu
                        //SetValueOut(GetEnum(GridColIndexes.ctaxable), j, Comm.FormatValue(dbltaxableAmount));

                        SetValueOut(GetEnum(GridColIndexes.cCGST), j, "0");
                        SetValueOut(GetEnum(GridColIndexes.cSGST), j, "0");
                        SetValueOut(GetEnum(GridColIndexes.cIGST), j, "0");

                        //SetValueOut(GetEnum(GridColIndexes.cNetAmount), j, Comm.FormatValue((dbltaxableValueAfterItemDiscount)));

                        dblIGSTTot = dblIGSTTot + Comm.ToDouble(dgvStockOut.Rows[j].Cells[GetEnum(GridColIndexes.cIGST)].Value);
                        dblSSGTTot = dblSSGTTot + Comm.ToDouble(dgvStockOut.Rows[j].Cells[GetEnum(GridColIndexes.cSGST)].Value);
                        dblCSGTTot = dblCSGTTot + Comm.ToDouble(dgvStockOut.Rows[j].Cells[GetEnum(GridColIndexes.cCGST)].Value);

                        dbltaxAmountTot = dbltaxAmountTot + dbltaxAmount;
                        //dbltaxAmountTot = Comm.FormatAmt(Val(dbltaxAmountTot) + Val(Format(Val(dbltaxAmount), DCSApp.GBizAmt)), DCSApp.GBizAmt)
                        // dont know how to format ??

                        dbltaxableAmountTot = dbltaxableAmountTot + Comm.ToDouble(dgvStockOut.Rows[j].Cells[GetEnum(GridColIndexes.ctaxable)].Value);
                        dblNontaxableAmountTot = dblNontaxableAmountTot + Comm.ToDouble(dgvStockOut.Rows[j].Cells[GetEnum(GridColIndexes.cNonTaxable)].Value);

                        //DGVItem.Item(cNetAmount, i).Value = Comm.FormatAmt(Val(DGVItem.Item(ctaxable, i).Value) + Val(DGVItem.Item(cNonTaxable, i).Value) + Val(DGVItem.Item(ctax, i).Value) + Val(DblcessAmount) + Val(DblFloodcessAmount) + Val(DblCompcessAmount), "")
                        //Dont know what is Comm.FormatAmt ->

                        SetValueOut(GetEnum(GridColIndexes.cNetAmount), j, Comm.FormatValue((Comm.ToDouble(dgvStockOut.Rows[j].Cells[GetEnum(GridColIndexes.ctaxable)].Value) + Comm.ToDouble(dgvStockOut.Rows[j].Cells[GetEnum(GridColIndexes.cNonTaxable)].Value) + Comm.ToDouble(dgvStockOut.Rows[j].Cells[GetEnum(GridColIndexes.ctax)].Value) + DblcessAmount + DblFloodcessAmount + DblCompcessAmount)));
                        DblNetAmountTotal = DblNetAmountTotal + Comm.ToDouble(dgvStockOut.Rows[j].Cells[GetEnum(GridColIndexes.cNetAmount)].Value);

                        //valuation of Free
                        dblQty = Comm.ToDouble(dgvStockOut.Rows[j].Cells[GetEnum(GridColIndexes.cQty)].Value);
                        if (Comm.ToDouble(dgvStockOut.Rows[j].Cells[GetEnum(GridColIndexes.cFree)].Value) > 0)
                        {
                            double PerItemRate = Comm.ToDouble(dgvStockOut.Rows[j].Cells[GetEnum(GridColIndexes.cNetAmount)].Value) - Comm.ToDouble(dgvStockOut.Rows[j].Cells[GetEnum(GridColIndexes.ctax)].Value) / dblQty;
                            TotalValueOfFree = TotalValueOfFree + (PerItemRate * Comm.ToDouble(dgvStockOut.Rows[j].Cells[GetEnum(GridColIndexes.cFree)].Value));
                        }

                        //CALCULATION DECIMAL CHANGING
                        SetValueOut(GetEnum(GridColIndexes.cDiscAmount), j, Comm.FormatValue(Comm.ToDouble(dgvStockOut.Rows[j].Cells[GetEnum(GridColIndexes.cDiscAmount)].Value)));
                        SetValueOut(GetEnum(GridColIndexes.cBillDisc), j, Comm.FormatValue(Comm.ToDouble(dgvStockOut.Rows[j].Cells[GetEnum(GridColIndexes.cBillDisc)].Value)));

                        SetValueOut(GetEnum(GridColIndexes.ctaxable), j, Comm.FormatValue(Comm.ToDouble(dgvStockOut.Rows[j].Cells[GetEnum(GridColIndexes.ctaxable)].Value)));
                        SetTagOut(GetEnum(GridColIndexes.ctaxable), j, Comm.FormatValue(Comm.ToDouble(dgvStockOut.Rows[j].Cells[GetEnum(GridColIndexes.ctaxable)].Value)));
                        //DGVItem.Item(ctaxable, i).Tag = Format(Val(DGVItem.Item(ctaxable, i).Value), "#0.000000")
                        //Tag ??

                        SetValueOut(GetEnum(GridColIndexes.ctax), j, "0");
                        SetValueOut(GetEnum(GridColIndexes.cIGST), j, "0");
                        SetValueOut(GetEnum(GridColIndexes.cSGST), j, "0");
                        SetValueOut(GetEnum(GridColIndexes.cCGST), j, "0");
                        SetValueOut(GetEnum(GridColIndexes.cNetAmount), j, Comm.FormatValue(Comm.ToDouble(dgvStockOut.Rows[j].Cells[GetEnum(GridColIndexes.cNetAmount)].Value)));
                        //SetValueOut(GetEnum(GridColIndexes.cNetAmount), j, Comm.FormatValue(Comm.ToDouble(dgvStockOut.Rows[j].Cells[GetEnum(GridColIndexes.cNetAmount)].Value)));
                        SetValueOut(GetEnum(GridColIndexes.cGrossValueAfterRateDiscount), j, Comm.FormatValue(Comm.ToDouble(dgvStockOut.Rows[j].Cells[GetEnum(GridColIndexes.cGrossValueAfterRateDiscount)].Value)));
                        SetValueOut(GetEnum(GridColIndexes.cNonTaxable), j, Comm.FormatValue(Comm.ToDouble(dgvStockOut.Rows[j].Cells[GetEnum(GridColIndexes.cNonTaxable)].Value)));

                        if (Comm.ToDouble(dgvStockOut.Rows[j].Cells[GetEnum(GridColIndexes.cAgentCommPer)].Value) > 0)
                            DblItemAgentCommission = (DblItemAgentCommission + Comm.ToDouble(dgvStockOut.Rows[j].Cells[GetEnum(GridColIndexes.cNetAmount)].Value) * Comm.ToDouble(dgvStockOut.Rows[j].Cells[GetEnum(GridColIndexes.cAgentCommPer)].Value) / 100);

                    }
                }
            }

            // What is the use of this ?? --------------------------- >>
            //If dgv.GetValue(LinkIDs.AgentCommissionMode) = "ITEM" Then
            //    dgv.SetValueOut(LinkIDs.AgentCommission, DblItemAgentCommission)
            //ElseIf dgv.GetValue(LinkIDs.AgentCommissionMode) = "NONE" Then
            //    dgv.SetValueOut(LinkIDs.AgentCommission, 0)
            //ElseIf InStr(dgv.GetValue(LinkIDs.AgentCommissionMode), "BILL", CompareMethod.Text) > 0 Then
            //    Dim MyVarStr() As String = Split(dgv.GetValue(LinkIDs.AgentCommissionMode), "@")
            //    If UBound(MyVarStr) > 0 Then
            //        DblItemAgentCommission = Val(lblBalance.Text) * Val(MyVarStr(1)) / 100
            //    End If
            //    dgv.SetValueOut(LinkIDs.AgentCommission, DblItemAgentCommission)
            //End If
            // What is the use of this ?? --------------------------- >>

            double bALANCEFORrOUNDOFF = Comm.ToDouble(Comm.FormatAmt(DblNetAmountTotal - 0 - Comm.ToDouble(0) + Comm.ToDouble(0), ""));


            double AdditionalCharges = 0;
            // When mytrans.IntRoundOffMode > 0 then Comment this

            //Assuming that the rate is equally equated between items .....
            //if (txtRoundOff.Text == "") txtRoundOff.Text = "0";
            //if (txtCostFactor.Text == "") txtCostFactor.Text = "0";

            //'Tethering to itemwise rate
            double mytaxable = 0;
            double MyPRate = 0;
            double MyQty;
            double perpieceaddcharges;

            for (int k = 0; k < dgvStockOut.Rows.Count; k++)
            {
                if (dgvStockOut.Rows[k].Cells[GetEnum(GridColIndexes.CItemName)].Value != null)
                {
                    if (dgvStockOut.Rows[k].Cells[GetEnum(GridColIndexes.CItemName)].Value.ToString() != "")
                    {
                        //if (Comm.ToDouble(dgvStockOut.Rows[k].Cells[GetEnum(GridColIndexes.CItemName)].Tag) > 0)
                        //{
                        if (dgvStockOut.Rows[k].Cells[GetEnum(GridColIndexes.cQty)].Value == null)
                            SetValueOut(GetEnum(GridColIndexes.cQty), k, AppSettings.QtyDecimalFormat);
                        if (dgvStockOut.Rows[k].Cells[GetEnum(GridColIndexes.cFree)].Value == null)
                            SetValueOut(GetEnum(GridColIndexes.cFree), k, AppSettings.QtyDecimalFormat);

                        mytaxable = Comm.ToDouble(dgvStockOut.Rows[k].Cells[GetEnum(GridColIndexes.ctaxable)].Value) + Comm.ToDouble(dgvStockOut.Rows[k].Cells[GetEnum(GridColIndexes.cNonTaxable)].Value);
                        MyPRate = 0;
                        perpieceaddcharges = 0;
                        MyQty = Comm.ToDouble(dgvStockOut.Rows[k].Cells[GetEnum(GridColIndexes.cQty)].Value);// + Comm.ToDouble(dgvStockOut.Rows[k].Cells[GetEnum(GridColIndexes.cFree)].Value);
                                                                                                          //Dipu on 25-May-2022 -- Free Value Commented
                        if ((dbltaxableAmountTot + dblNontaxableAmountTot) > 0)
                        {
                            if (MyQty > 0)
                                perpieceaddcharges = (AdditionalCharges / (dbltaxableAmountTot + dblNontaxableAmountTot) * mytaxable) / (Comm.ToDouble(dgvStockOut.Rows[k].Cells[GetEnum(GridColIndexes.cQty)].Value) + Comm.ToDouble(dgvStockOut.Rows[k].Cells[GetEnum(GridColIndexes.cFree)].Value));
                        }
                        //perpieceaddcharges = (AdditionalCharges / (dbltaxableAmountTot + dblNontaxableAmountTot) * mytaxable) / Comm.ToDouble(dgvStockOut.Rows[k].Cells[GetEnum(GridColIndexes.cQty)].Value);
                        double MyPrateWithtax = 0;

                        if (mytaxable > 0)
                        {
                            //MyPRate = mytaxable / Comm.ToDouble(dgvStockOut.Rows[k].Cells[GetEnum(GridColIndexes.cQty)].Value) + Comm.ToDouble(dgvStockOut.Rows[k].Cells[GetEnum(GridColIndexes.cFree)].Value);
                            MyPRate = mytaxable / Comm.ToDouble(dgvStockOut.Rows[k].Cells[GetEnum(GridColIndexes.cQty)].Value);
                            MyPrateWithtax = (mytaxable + Comm.ToDouble(dgvStockOut.Rows[k].Cells[GetEnum(GridColIndexes.ctax)].Value)) / (Comm.ToDouble(dgvStockOut.Rows[k].Cells[GetEnum(GridColIndexes.cQty)].Value));
                        }

                        //Distributing CommonValues Betweeen Items

                        SetValueOut(GetEnum(GridColIndexes.cPrate), k, Comm.FormatValue(Comm.ToDouble(dgvStockOut.Rows[k].Cells[GetEnum(GridColIndexes.cPrate)].Value))); //cRate <--> cSrate
                                                                                                                                                                    //DGVItem.Item(cSrate, i).Value = DGVItem.Item(cRate, i).Value
                                                                                                                                                                    //MyPRate = MyPRate; // + perpieceaddcharges;
                                                                                                                                                                    //Added by Dipu on 23-Nov-2021 ---------------->>



                        //MyPrateWithtax = MyPrateWithtax; // + perpieceaddcharges;
                        //SetValueOut(GetEnum(GridColIndexes.cCrate), k, Comm.FormatValue(MyPRate));
                        //SetValueOut(GetEnum(GridColIndexes.cCRateWithTax), k, Comm.FormatValue(MyPrateWithtax));
                        if (MyPRate > 0)
                        {
                            //NotifyIcon("Sales Value Calculation", MyPRate)
                            //MessageBox.Show("Sales Value Calculation (" + MyPRate + ")");
                        }

                        ////BLNRECALCULATESalesRatesOnPercentag
                        //if (clsVchTypeFeatures.BLNRECALCULATESalesRatesOnPercentage == true)
                        //{
                        //    double dblcSRate1Per = Comm.ToDouble(dgvStockOut.Rows[k].Cells[GetEnum(GridColIndexes.cSRate1Per)].Value);
                        //    double dblcsRate2Per = Comm.ToDouble(dgvStockOut.Rows[k].Cells[GetEnum(GridColIndexes.cSRate2Per)].Value);
                        //    double dblcsRate3Per = Comm.ToDouble(dgvStockOut.Rows[k].Cells[GetEnum(GridColIndexes.cSRate3Per)].Value);
                        //    double dblcsRate4Per = Comm.ToDouble(dgvStockOut.Rows[k].Cells[GetEnum(GridColIndexes.cSRate4Per)].Value);
                        //    double dblcsRate5Per = Comm.ToDouble(dgvStockOut.Rows[k].Cells[GetEnum(GridColIndexes.cSRate5Per)].Value);

                        //    double dblcRate = Comm.ToDouble(dgvStockOut.Rows[k].Cells[GetEnum(GridColIndexes.cSRate1)].Value);
                        //    double dblcCRate = Comm.ToDouble(dgvStockOut.Rows[k].Cells[GetEnum(GridColIndexes.cCrate)].Value);
                        //    double dblcMRP = Comm.ToDouble(dgvStockOut.Rows[k].Cells[GetEnum(GridColIndexes.cMRP)].Value);
                        //    double dblcCRateWithTax = Comm.ToDouble(dgvStockOut.Rows[k].Cells[GetEnum(GridColIndexes.cCRateWithTax)].Value);

                        //    if (dgvStockOut.Rows[k].Cells[GetEnum(GridColIndexes.cSRate1)].Tag == null)
                        //        dgvStockOut.Rows[k].Cells[GetEnum(GridColIndexes.cSRate1)].Tag = "";
                        //    if (dgvStockOut.Rows[k].Cells[GetEnum(GridColIndexes.cSRate2)].Tag == null)
                        //        dgvStockOut.Rows[k].Cells[GetEnum(GridColIndexes.cSRate2)].Tag = "";
                        //    if (dgvStockOut.Rows[k].Cells[GetEnum(GridColIndexes.cSRate3)].Tag == null)
                        //        dgvStockOut.Rows[k].Cells[GetEnum(GridColIndexes.cSRate3)].Tag = "";
                        //    if (dgvStockOut.Rows[k].Cells[GetEnum(GridColIndexes.cSRate4)].Tag == null)
                        //        dgvStockOut.Rows[k].Cells[GetEnum(GridColIndexes.cSRate4)].Tag = "";
                        //    if (dgvStockOut.Rows[k].Cells[GetEnum(GridColIndexes.cSRate5)].Tag == null)
                        //        dgvStockOut.Rows[k].Cells[GetEnum(GridColIndexes.cSRate5)].Tag = "";

                        //    switch (Comm.ToDouble(dgvStockOut.Rows[k].Cells[GetEnum(GridColIndexes.cSRate1Per)].Tag)) //DiscMode
                        //    {
                        //        case 0:
                        //            if (dblcSRate1Per > 0 && dgvStockOut.Rows[k].Cells[GetEnum(GridColIndexes.cSRate1)].Tag.ToString() == "") SetValueOut(GetEnum(GridColIndexes.cSRate1), k, Comm.FormatValue((dblcRate + dblcRate * dblcSRate1Per / 100)));
                        //            if (dblcsRate2Per > 0 && dgvStockOut.Rows[k].Cells[GetEnum(GridColIndexes.cSRate2)].Tag.ToString() == "") SetValueOut(GetEnum(GridColIndexes.cSRate2), k, Comm.FormatValue((dblcRate + dblcRate * dblcsRate2Per / 100)));
                        //            if (dblcsRate3Per > 0 && dgvStockOut.Rows[k].Cells[GetEnum(GridColIndexes.cSRate3)].Tag.ToString() == "") SetValueOut(GetEnum(GridColIndexes.cSRate3), k, Comm.FormatValue((dblcRate + dblcRate * dblcsRate3Per / 100)));
                        //            if (dblcsRate4Per > 0 && dgvStockOut.Rows[k].Cells[GetEnum(GridColIndexes.cSRate4)].Tag.ToString() == "") SetValueOut(GetEnum(GridColIndexes.cSRate4), k, Comm.FormatValue((dblcRate + dblcRate * dblcsRate4Per / 100)));
                        //            if (dblcsRate5Per > 0 && dgvStockOut.Rows[k].Cells[GetEnum(GridColIndexes.cSRate5)].Tag.ToString() == "") SetValueOut(GetEnum(GridColIndexes.cSRate5), k, Comm.FormatValue((dblcRate + dblcRate * dblcsRate5Per / 100)));
                        //            break;
                        //        case 3:
                        //            if (dblcSRate1Per > 0 && dgvStockOut.Rows[k].Cells[GetEnum(GridColIndexes.cSRate1)].Tag.ToString() == "") SetValueOut(GetEnum(GridColIndexes.cSRate1), k, Comm.FormatValue(dblcCRate + dblcCRate * dblcSRate1Per / 100));
                        //            if (dblcsRate2Per > 0 && dgvStockOut.Rows[k].Cells[GetEnum(GridColIndexes.cSRate2)].Tag.ToString() == "") SetValueOut(GetEnum(GridColIndexes.cSRate2), k, Comm.FormatValue(dblcCRate + dblcCRate * dblcsRate2Per / 100));
                        //            if (dblcsRate3Per > 0 && dgvStockOut.Rows[k].Cells[GetEnum(GridColIndexes.cSRate3)].Tag.ToString() == "") SetValueOut(GetEnum(GridColIndexes.cSRate3), k, Comm.FormatValue(dblcCRate + dblcCRate * dblcsRate3Per / 100));
                        //            if (dblcsRate4Per > 0 && dgvStockOut.Rows[k].Cells[GetEnum(GridColIndexes.cSRate4)].Tag.ToString() == "") SetValueOut(GetEnum(GridColIndexes.cSRate4), k, Comm.FormatValue(dblcCRate + dblcCRate * dblcsRate4Per / 100));
                        //            if (dblcsRate5Per > 0 && dgvStockOut.Rows[k].Cells[GetEnum(GridColIndexes.cSRate5)].Tag.ToString() == "") SetValueOut(GetEnum(GridColIndexes.cSRate5), k, Comm.FormatValue(dblcCRate + dblcCRate * dblcsRate5Per / 100));
                        //            break;
                        //        case 1:
                        //            if (dblcSRate1Per > 0 && dgvStockOut.Rows[k].Cells[GetEnum(GridColIndexes.cSRate1)].Tag.ToString() == "") SetValueOut(GetEnum(GridColIndexes.cSRate1), k, Comm.FormatValue(dblcMRP - dblcMRP * dblcSRate1Per / 100));
                        //            if (dblcsRate2Per > 0 && dgvStockOut.Rows[k].Cells[GetEnum(GridColIndexes.cSRate2)].Tag.ToString() == "") SetValueOut(GetEnum(GridColIndexes.cSRate2), k, Comm.FormatValue(dblcMRP - dblcMRP * dblcsRate2Per / 100));
                        //            if (dblcsRate3Per > 0 && dgvStockOut.Rows[k].Cells[GetEnum(GridColIndexes.cSRate3)].Tag.ToString() == "") SetValueOut(GetEnum(GridColIndexes.cSRate3), k, Comm.FormatValue(dblcMRP - dblcMRP * dblcsRate3Per / 100));
                        //            if (dblcsRate4Per > 0 && dgvStockOut.Rows[k].Cells[GetEnum(GridColIndexes.cSRate4)].Tag.ToString() == "") SetValueOut(GetEnum(GridColIndexes.cSRate4), k, Comm.FormatValue(dblcMRP - dblcMRP * dblcsRate4Per / 100));
                        //            if (dblcsRate5Per > 0 && dgvStockOut.Rows[k].Cells[GetEnum(GridColIndexes.cSRate5)].Tag.ToString() == "") SetValueOut(GetEnum(GridColIndexes.cSRate5), k, Comm.FormatValue(dblcMRP - dblcMRP * dblcsRate5Per / 100));
                        //            break;
                        //        case 2:
                        //            if (dblcSRate1Per > 0 && dgvStockOut.Rows[k].Cells[GetEnum(GridColIndexes.cSRate1)].Tag.ToString() == "") SetValueOut(GetEnum(GridColIndexes.cSRate1), k, Comm.FormatValue(dblcCRateWithTax + dblcCRateWithTax * dblcSRate1Per / 100));
                        //            if (dblcsRate2Per > 0 && dgvStockOut.Rows[k].Cells[GetEnum(GridColIndexes.cSRate2)].Tag.ToString() == "") SetValueOut(GetEnum(GridColIndexes.cSRate2), k, Comm.FormatValue(dblcCRateWithTax + dblcCRateWithTax * dblcsRate2Per / 100));
                        //            if (dblcsRate3Per > 0 && dgvStockOut.Rows[k].Cells[GetEnum(GridColIndexes.cSRate3)].Tag.ToString() == "") SetValueOut(GetEnum(GridColIndexes.cSRate3), k, Comm.FormatValue(dblcCRateWithTax + dblcCRateWithTax * dblcsRate3Per / 100));
                        //            if (dblcsRate4Per > 0 && dgvStockOut.Rows[k].Cells[GetEnum(GridColIndexes.cSRate4)].Tag.ToString() == "") SetValueOut(GetEnum(GridColIndexes.cSRate4), k, Comm.FormatValue(dblcCRateWithTax + dblcCRateWithTax * dblcsRate4Per / 100));
                        //            if (dblcsRate5Per > 0 && dgvStockOut.Rows[k].Cells[GetEnum(GridColIndexes.cSRate5)].Tag.ToString() == "") SetValueOut(GetEnum(GridColIndexes.cSRate5), k, Comm.FormatValue(dblcCRateWithTax + dblcCRateWithTax * dblcsRate5Per / 100));
                        //            break;
                        //    }

                        //    dgvStockOut.Rows[k].Cells[GetEnum(GridColIndexes.cSRate1)].Tag = "";
                        //    dgvStockOut.Rows[k].Cells[GetEnum(GridColIndexes.cSRate2)].Tag = "";
                        //    dgvStockOut.Rows[k].Cells[GetEnum(GridColIndexes.cSRate3)].Tag = "";
                        //    dgvStockOut.Rows[k].Cells[GetEnum(GridColIndexes.cSRate4)].Tag = "";
                        //    dgvStockOut.Rows[k].Cells[GetEnum(GridColIndexes.cSRate5)].Tag = "";
                        //}

                        //double SavingsofItem = (Comm.ToDouble(dgvStockOut.Rows[k].Cells[GetEnum(GridColIndexes.cMRP)].Value) * MyQty) - (Val(DGVItem.Item(cRate, i).Value) * MyQty);
                        SavingsofItem = (Comm.ToDouble(dgvStockOut.Rows[k].Cells[GetEnum(GridColIndexes.cMRP)].Value) * MyQty) - (Comm.ToDouble(dgvStockOut.Rows[k].Cells[GetEnum(GridColIndexes.cPrate)].Value) * MyQty); //cRate <--> cSrate
                        if (MyQty > 0) Savings = Savings + SavingsofItem;

                        //}
                    }
                }
            }

            //ItemDiscount and Discount Amount are equal ??
            Savings = Savings + Comm.ToDouble(0) + Comm.ToDouble(0) + Comm.ToDouble(0) - Comm.ToDouble(0);

        }

        private void CallBatchCodeCompact(bool bWhenPressDownKey = false)
        {
            bool blnAutoCodeNeeded = false;
            string sQuery = "";

            if (Comm.ToInt32(dgvStockIn.Rows[dgvStockIn.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cBatchMode)].Value) == 0) // None
                blnAutoCodeNeeded = false;
            else if (Comm.ToInt32(dgvStockIn.Rows[dgvStockIn.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cBatchMode)].Value) == 1) // MNF
                blnAutoCodeNeeded = true;
            else if (Comm.ToInt32(dgvStockIn.Rows[dgvStockIn.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cBatchMode)].Value) == 2) // Auto
                blnAutoCodeNeeded = true;
            else if (Comm.ToInt32(dgvStockIn.Rows[dgvStockIn.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cBatchMode)].Value) == 3) // WMH
                blnAutoCodeNeeded = false;

            //string sQuery = "SELECT AnyWhere,BatchCode,ExpiryDate,MRP,QOH,StockID,ItemID FROM (SELECT (BatchCode+CONVERT(VARCHAR,ExpiryDate)+CONVERT(VARCHAR,ISNULL(QOH,0))+CONVERT(VARCHAR,ISNULL(MRP,0))) as AnyWhere,BatchCode,ExpiryDate,MRP,QOH,StockID,ItemID FROM tblStock )A WHERE A.ItemID = " + Comm.ToInt32(dgvStockIn.Rows[dgvStockIn.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cItemID)].Value.ToString()) + "";
            sQuery = "SELECT AnyWhere,BatchCode,ExpiryDate,MRP,QOH,StockID,ItemID FROM (";
            sQuery = sQuery + "SELECT (BatchCode+CONVERT(VARCHAR,ExpiryDate)+CONVERT(VARCHAR,ISNULL(QOH,0))+CONVERT(VARCHAR,ISNULL(MRP,0))) as AnyWhere,BatchUnique as BatchCode,ExpiryDate,MRP,QOH,StockID,ItemID FROM tblStock ";

            if (blnAutoCodeNeeded == true)
            {
                if (Comm.ToInt32(dgvStockIn.Rows[dgvStockIn.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cBatchMode)].Value) == 1)// MNF
                {
                    if (bWhenPressDownKey == true)
                        sQuery = sQuery + " UNION ALL SELECT '<Auto Barcode>' as AnyWhere,'<Auto Barcode>' as BatchCode,'23-Feb-2030' as ExpiryDate,0 as MRP,0 as QOH,0 as StockID," + Comm.ToInt32(dgvStockIn.Rows[dgvStockIn.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cItemID)].Value.ToString()) + " as ItemID";
                }
                else if (Comm.ToInt32(dgvStockIn.Rows[dgvStockIn.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cBatchMode)].Value) == 2)// Auto
                {
                    sQuery = sQuery + " UNION ALL SELECT '<Auto Barcode>' as AnyWhere,'<Auto Barcode>' as BatchCode,'23-Feb-2030' as ExpiryDate,0 as MRP,0 as QOH,0 as StockID," + Comm.ToInt32(dgvStockIn.Rows[dgvStockIn.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cItemID)].Value.ToString()) + " as ItemID";
                }
            }

            if (Comm.ToInt32(dgvStockIn.Rows[dgvStockIn.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cBatchMode)].Value) == 1 || Comm.ToInt32(dgvStockIn.Rows[dgvStockIn.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cBatchMode)].Value) == 2)// MNF & AUto
            {
                sQuery = sQuery + " )A WHERE A.ItemID = " + Comm.ToDecimal(dgvStockIn.Rows[dgvStockIn.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cItemID)].Value.ToString()) + "";
                //frmBatchSearch = new frmCompactSearch(GetFromBatchCodeSearch, sQuery, "Anywhere|BatchCode|CONVERT(VARCHAR,ExpiryDate)|CONVERT(VARCHAR,ISNULL(MRP,0))|CONVERT(VARCHAR,ISNULL(QOH,0))", dgvStockIn.Location.X + 350, dgvStockIn.Location.Y, 4, 0, "", 4, 0, "ORDER BY A.StockID ASC", 0, 0, "BatchCode Search...", 0, "150,90,80,80,0,0", true, "Bar Code", 10);
                
                frmBatchSearch = new frmCompactSearch(GetFromBatchCodeSearch, sQuery, "Anywhere|BatchCode|CONVERT(VARCHAR,ExpiryDate)|CONVERT(VARCHAR,ISNULL(MRP,0))|CONVERT(VARCHAR,ISNULL(QOH,0))", dgvStockIn.Location.X + 350, dgvStockIn.Location.Y, 4, 0, "", 4, 0, "ORDER BY A.StockID ASC", 0, 0, "BatchCode Search...", 0, "150,90,80,80,0,0", true, "Bar Code", 10);
                frmBatchSearch.Show();
                frmBatchSearch.BringToFront();
            }
        }

        private void CallBatchCodeCompactOut(bool bWhenPressDownKey = false)
        {
            bool blnAutoCodeNeeded = false;
            string sQuery = "";

            if (Comm.ToInt32(dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cBatchMode)].Value) == 0) // None
                blnAutoCodeNeeded = false;
            else if (Comm.ToInt32(dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cBatchMode)].Value) == 1) // MNF
                blnAutoCodeNeeded = true;
            else if (Comm.ToInt32(dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cBatchMode)].Value) == 2) // Auto
                blnAutoCodeNeeded = true;
            else if (Comm.ToInt32(dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cBatchMode)].Value) == 3) // WMH
                blnAutoCodeNeeded = false;

            //string sQuery = "SELECT AnyWhere,BatchCode,ExpiryDate,MRP,QOH,StockID,ItemID FROM (SELECT (BatchCode+CONVERT(VARCHAR,ExpiryDate)+CONVERT(VARCHAR,ISNULL(QOH,0))+CONVERT(VARCHAR,ISNULL(MRP,0))) as AnyWhere,BatchCode,ExpiryDate,MRP,QOH,StockID,ItemID FROM tblStock )A WHERE A.ItemID = " + Comm.ToInt32(dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cItemID)].Value.ToString()) + "";
            sQuery = "SELECT AnyWhere,BatchCode,ExpiryDate,MRP,QOH,StockID,ItemID FROM (";
            sQuery = sQuery + "SELECT (BatchCode+CONVERT(VARCHAR,ExpiryDate)+CONVERT(VARCHAR,ISNULL(QOH,0))+CONVERT(VARCHAR,ISNULL(MRP,0))) as AnyWhere,BatchUnique as BatchCode,ExpiryDate,MRP,QOH,StockID,ItemID FROM tblStock ";

            if (blnAutoCodeNeeded == true)
            {
                if (Comm.ToInt32(dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cBatchMode)].Value) == 1)// MNF
                {
                    if (bWhenPressDownKey == true)
                        sQuery = sQuery + " UNION ALL SELECT '<Auto Barcode>' as AnyWhere,'<Auto Barcode>' as BatchCode,'23-Feb-2030' as ExpiryDate,0 as MRP,0 as QOH,0 as StockID," + Comm.ToInt32(dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cItemID)].Value.ToString()) + " as ItemID";
                }
                else if (Comm.ToInt32(dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cBatchMode)].Value) == 2)// Auto
                {
                    sQuery = sQuery + " UNION ALL SELECT '<Auto Barcode>' as AnyWhere,'<Auto Barcode>' as BatchCode,'23-Feb-2030' as ExpiryDate,0 as MRP,0 as QOH,0 as StockID," + Comm.ToInt32(dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cItemID)].Value.ToString()) + " as ItemID";
                }
            }

            if (Comm.ToInt32(dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cBatchMode)].Value) == 1 || Comm.ToInt32(dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cBatchMode)].Value) == 2)// MNF & AUto
            {
                sQuery = sQuery + " )A WHERE A.ItemID = " + Comm.ToDecimal(dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cItemID)].Value.ToString()) + "";
                //frmBatchSearch = new frmCompactSearch(GetFromBatchCodeSearchOut, sQuery, "Anywhere|BatchCode|CONVERT(VARCHAR,ExpiryDate)|CONVERT(VARCHAR,ISNULL(MRP,0))|CONVERT(VARCHAR,ISNULL(QOH,0))", dgvStockOut.Location.X + 350, dgvStockOut.Location.Y, 4, 0, "", 4, 0, "ORDER BY A.StockID ASC", 0, 0, "BatchCode Search...", 0, "150,90,80,80,0,0", true, "Bar Code", 10);
                
                frmBatchSearch = new frmCompactSearch(GetFromBatchCodeSearchOut, sQuery, "Anywhere|BatchCode|CONVERT(VARCHAR,ExpiryDate)|CONVERT(VARCHAR,ISNULL(MRP,0))|CONVERT(VARCHAR,ISNULL(QOH,0))", dgvStockIn.Location.X + 350, dgvStockIn.Location.Y, 4, 0, "", 4, 0, "ORDER BY A.StockID ASC", 0, 0, "BatchCode Search...", 0, "150,90,80,80,0,0", true, "Bar Code", 10);
                frmBatchSearch.Show();
                frmBatchSearch.BringToFront();
            }
        }

        //Description : Setting Default Transactional Settings to the form
        private void SetTransactionDefaults()
        {
            if (Comm.ConvertI32(clsVchType.TransactionNumberingValue) == 0) // Auto Locked
            {
                if (iIDFromEditWindow == 0) //New
                {
                    txtInvAutoNo.Text = Comm.gfnGetNextSerialNo("tblRepacking", "AutoNum", " VchTypeID=" + vchtypeID).ToString();
                    txtReferenceAutoNo.Tag = txtInvAutoNo.Text;
                    txtInvAutoNo.Tag = 0;
                    if (txtInvAutoNo.Text == "") txtInvAutoNo.Text = "1";
                }  
                txtInvAutoNo.ReadOnly = true;
                txtPrefix.ReadOnly = true;
            }
            else if (Comm.ConvertI32(clsVchType.TransactionNumberingValue) == 1) // Auto Editable
            {
                if (iIDFromEditWindow == 0) //New
                {
                    //txtInvAutoNo.Text = Comm.gfnGetNextSerialNo("tblRepacking", "AutoNum").ToString();
                    txtInvAutoNo.Text = Comm.gfnGetNextSerialNo("tblRepacking", "AutoNum", " VchTypeID=" + vchtypeID).ToString();
                    txtReferenceAutoNo.Tag = txtInvAutoNo.Text;
                    txtInvAutoNo.Tag = 0;
                    if (txtInvAutoNo.Text == "") txtInvAutoNo.Text = "1";
                }

                txtInvAutoNo.ReadOnly = false;
                txtPrefix.ReadOnly = false;
            }
            else
            {
                txtInvAutoNo.Tag = 0;
                txtInvAutoNo.ReadOnly = false;
                txtPrefix.ReadOnly = false;
            }

            if (clsVchType.TransactionPrefix != "") // Transactoin Prefix
            {
                txtPrefix.Text = clsVchType.TransactionPrefix.Trim();
                txtPrefix.Visible = true;
            }
            else
                txtPrefix.Visible = false;

            if (clsVchTypeFeatures.blnShowReferenceNo)//Show Reference No
            {
                lblReferenceNo.Visible = true;
                txtReferencePrefix.Visible = true;
                txtReferenceAutoNo.Visible = true;
            }
            else
            {
                lblReferenceNo.Visible = false;
                txtReferencePrefix.Visible = false;
                txtReferenceAutoNo.Visible = false;
            }

            if (clsVchType.ReferencePrefix != "") // ReferencePrefix
            {
                txtReferencePrefix.Text = clsVchType.ReferencePrefix.Trim();
                txtReferencePrefix.Visible = true;
                txtReferencePrefix.Width = 55;
            }
            else
                txtReferencePrefix.Visible = false;


            if (Comm.ConvertI32(clsVchType.ReferenceNumberingValue) == 0) // Auto Locked
            {
                if(iIDFromEditWindow == 0)
                    txtReferenceAutoNo.Text = Comm.gfnGetNextSerialNo("tblRepacking", "ReferenceAutoNO").ToString();
                txtReferenceAutoNo.ReadOnly = true;
                txtReferencePrefix.ReadOnly = true;
                txtReferencePrefix.Width = 55;
            }
            else if (Comm.ConvertI32(clsVchType.ReferenceNumberingValue) == 1) // Auto Editable
            {
                if (iIDFromEditWindow == 0)
                    txtReferenceAutoNo.Text = Comm.gfnGetNextSerialNo("tblRepacking", "ReferenceAutoNO").ToString();
                txtReferenceAutoNo.ReadOnly = false;
                txtReferencePrefix.ReadOnly = false;
                txtReferencePrefix.Width = 55;
            }
            else if (Comm.ConvertI32(clsVchType.ReferenceNumberingValue) == 2) // Custom
            {
                txtReferenceAutoNo.ReadOnly = true;
                txtReferencePrefix.ReadOnly = false;
                txtReferencePrefix.Visible = true;
                txtReferencePrefix.Width = txtReferenceAutoNo.Width;
                
            }

            ////--------------------------------------------------------------////

            if (clsVchType.blnPrimaryLockWithSelection == 1)
                cboCostCentre.Enabled = false;
            else
                cboCostCentre.Enabled = true;

            dgvStockIn.Columns["cCGST"].Visible = false;
            dgvStockIn.Columns["cSGST"].Visible = false;
            dgvStockIn.Columns["cIGST"].Visible = false;
            dgvStockIn.Columns["ctaxPer"].Visible = false;
            dgvStockIn.Columns["ctax"].Visible = false;
            dgvStockIn.Columns["ctaxable"].Visible = false;
            dgvStockIn.Columns["cCRateWithTax"].Visible = false;
            dgvStockIn.Columns["cRateinclusive" ].Visible = false;
            dgvStockIn.Columns["cItemID" ].Visible = false;
            dgvStockIn.Columns["cFree" ].Visible = false;
            dgvStockIn.Columns["cDiscPer" ].Visible = false;
            dgvStockIn.Columns["cDiscAmount" ].Visible = false;
            dgvStockIn.Columns["cBillDisc" ].Visible = false;
            dgvStockIn.Columns["ctaxable" ].Visible = false;
            dgvStockIn.Columns["ctaxPer" ].Visible = false;
            dgvStockIn.Columns["ctax" ].Visible = false;
            dgvStockIn.Columns["cIGST" ].Visible = false;
            dgvStockIn.Columns["cSGST" ].Visible = false;
            dgvStockIn.Columns["cCGST" ].Visible = false;
            dgvStockIn.Columns["cGrossValueAfterRateDiscount" ].Visible = false;
            dgvStockIn.Columns["cNonTaxable" ].Visible = false;
            dgvStockIn.Columns["cCCessPer" ].Visible = false;
            dgvStockIn.Columns["cCCompCessQty" ].Visible = false;
            dgvStockIn.Columns["cFloodCessPer" ].Visible = false;
            dgvStockIn.Columns["cFloodCessAmt" ].Visible = false;
            dgvStockIn.Columns["cStockMRP" ].Visible = false;
            dgvStockIn.Columns["cAgentCommPer" ].Visible = false;
            dgvStockIn.Columns["cCoolie" ].Visible = false;
            dgvStockIn.Columns["cBlnOfferItem" ].Visible = false;
            dgvStockIn.Columns["cStrOfferDetails" ].Visible = false;
            dgvStockIn.Columns["cBatchMode" ].Visible = false;
            dgvStockIn.Columns["cBatchUnique" ].Visible = false;
            dgvStockIn.Columns["cID"].Visible = false;


            dgvStockOut.Columns["cCGST"].Visible = false;
            dgvStockOut.Columns["cSGST"].Visible = false;
            dgvStockOut.Columns["cIGST"].Visible = false;
            dgvStockOut.Columns["ctaxPer"].Visible = false;
            dgvStockOut.Columns["ctax"].Visible = false;
            dgvStockOut.Columns["ctaxable"].Visible = false;
            dgvStockOut.Columns["cCRateWithTax"].Visible = false;
            dgvStockOut.Columns["cRateinclusive"].Visible = false;
            dgvStockOut.Columns["cItemID"].Visible = false;
            dgvStockOut.Columns["cFree"].Visible = false;
            dgvStockOut.Columns["cDiscPer"].Visible = false;
            dgvStockOut.Columns["cDiscAmount"].Visible = false;
            dgvStockOut.Columns["cBillDisc"].Visible = false;
            dgvStockOut.Columns["ctaxable"].Visible = false;
            dgvStockOut.Columns["ctaxPer"].Visible = false;
            dgvStockOut.Columns["ctax"].Visible = false;
            dgvStockOut.Columns["cIGST"].Visible = false;
            dgvStockOut.Columns["cSGST"].Visible = false;
            dgvStockOut.Columns["cCGST"].Visible = false;
            dgvStockOut.Columns["cGrossValueAfterRateDiscount"].Visible = false;
            dgvStockOut.Columns["cNonTaxable"].Visible = false;
            dgvStockOut.Columns["cCCessPer"].Visible = false;
            dgvStockOut.Columns["cCCompCessQty"].Visible = false;
            dgvStockOut.Columns["cFloodCessPer"].Visible = false;
            dgvStockOut.Columns["cFloodCessAmt"].Visible = false;
            dgvStockOut.Columns["cStockMRP"].Visible = false;
            dgvStockOut.Columns["cAgentCommPer"].Visible = false;
            dgvStockOut.Columns["cCoolie"].Visible = false;
            dgvStockOut.Columns["cBlnOfferItem"].Visible = false;
            dgvStockOut.Columns["cStrOfferDetails"].Visible = false;
            dgvStockOut.Columns["cBatchMode"].Visible = false;
            dgvStockOut.Columns["cBatchUnique"].Visible = false;
            dgvStockOut.Columns["cID"].Visible = false;
        }

        //Description : Setting Transactions that Varying to the form
        private void SetTransactionsthatVarying()
        {
            cboCostCentre.SelectedValue = Comm.ConvertI32(clsVchType.PrimaryCCValue);
        }

        //Description : Setting asper Application Settings
        private void SetApplicationSettings(DataGridView dgv)
        {
            if (dgv.Columns.Count > 0)
            {
                if (AppSettings.TaxEnabled == true)
                {
                    if (AppSettings.TaxMode == 0) //No Tax
                    {
                        dgv.Columns["cCGST"].Visible = false;
                        dgv.Columns["cSGST"].Visible = false;
                        dgv.Columns["cIGST"].Visible = false;
                        dgv.Columns["ctaxPer"].Visible = false;
                        dgv.Columns["ctax"].Visible = false;
                        dgv.Columns["ctaxable"].Visible = false;
                        dgv.Columns["cCRateWithTax"].Visible = false;
                    }
                    else if (AppSettings.TaxMode == 1) //VAT
                    {
                        dgv.Columns["cCGST"].Visible = false;
                        dgv.Columns["cSGST"].Visible = false;

                        dgv.Columns["cIGST"].Visible = true;
                        dgv.Columns["ctaxPer"].Visible = true;
                        dgv.Columns["ctax"].Visible = true;
                        dgv.Columns["ctaxable"].Visible = true;
                        dgv.Columns["cCRateWithTax"].Visible = true;
                    }
                    else
                    {
                        dgv.Columns["cCGST"].Visible = true;
                        dgv.Columns["cSGST"].Visible = true;
                        dgv.Columns["cIGST"].Visible = true;
                        dgv.Columns["ctaxPer"].Visible = true;
                        dgv.Columns["ctax"].Visible = true;
                        dgv.Columns["ctaxable"].Visible = true;
                        dgv.Columns["cCRateWithTax"].Visible = true;
                    }
                }
                else
                {
                    dgv.Columns[GetEnum(GridColIndexes.cCGST)].Visible = false;
                    dgv.Columns[GetEnum(GridColIndexes.cSGST)].Visible = false;
                    dgv.Columns[GetEnum(GridColIndexes.cIGST)].Visible = false;
                    dgv.Columns[GetEnum(GridColIndexes.ctaxPer)].Visible = false;
                    dgv.Columns[GetEnum(GridColIndexes.ctax)].Visible = false;
                    dgv.Columns[GetEnum(GridColIndexes.ctaxable)].Visible = false;
                    dgv.Columns[GetEnum(GridColIndexes.cCRateWithTax)].Visible = false;
                }
            }

            if (dgv.Columns.Count > 0)
            {
                if (AppSettings.CessMode == 0)
                {
                    dgv.Columns[GetEnum(GridColIndexes.cCCessPer)].Visible = false;
                    dgv.Columns[GetEnum(GridColIndexes.cCCompCessQty)].Visible = false;
                    dgv.Columns[GetEnum(GridColIndexes.cFloodCessAmt)].Visible = false;
                    dgv.Columns[GetEnum(GridColIndexes.cFloodCessPer)].Visible = false;
                }
                else
                {
                    dgv.Columns[GetEnum(GridColIndexes.cCCessPer)].Visible = true;
                    dgv.Columns[GetEnum(GridColIndexes.cCCompCessQty)].Visible = true;
                    dgv.Columns[GetEnum(GridColIndexes.cFloodCessAmt)].Visible = true;
                    dgv.Columns[GetEnum(GridColIndexes.cFloodCessPer)].Visible = true;
                }
            }

            dgvStockIn.Columns["cCGST"].Visible = false;
            dgvStockIn.Columns["cSGST"].Visible = false;
            dgvStockIn.Columns["cIGST"].Visible = false;
            dgvStockIn.Columns["ctaxPer"].Visible = false;
            dgvStockIn.Columns["ctax"].Visible = false;
            dgvStockIn.Columns["ctaxable"].Visible = false;
            dgvStockIn.Columns["cCRateWithTax"].Visible = false;
            dgvStockIn.Columns["cRateinclusive"].Visible = false;
            dgvStockIn.Columns["cItemID"].Visible = false;
            dgvStockIn.Columns["cFree"].Visible = false;
            dgvStockIn.Columns["cDiscPer"].Visible = false;
            dgvStockIn.Columns["cDiscAmount"].Visible = false;
            dgvStockIn.Columns["cBillDisc"].Visible = false;
            dgvStockIn.Columns["ctaxable"].Visible = false;
            dgvStockIn.Columns["ctaxPer"].Visible = false;
            dgvStockIn.Columns["ctax"].Visible = false;
            dgvStockIn.Columns["cIGST"].Visible = false;
            dgvStockIn.Columns["cSGST"].Visible = false;
            dgvStockIn.Columns["cCGST"].Visible = false;
            dgvStockIn.Columns["cGrossValueAfterRateDiscount"].Visible = false;
            dgvStockIn.Columns["cNonTaxable"].Visible = false;
            dgvStockIn.Columns["cCCessPer"].Visible = false;
            dgvStockIn.Columns["cCCompCessQty"].Visible = false;
            dgvStockIn.Columns["cFloodCessPer"].Visible = false;
            dgvStockIn.Columns["cFloodCessAmt"].Visible = false;
            dgvStockIn.Columns["cStockMRP"].Visible = false;
            dgvStockIn.Columns["cAgentCommPer"].Visible = false;
            dgvStockIn.Columns["cCoolie"].Visible = false;
            dgvStockIn.Columns["cBlnOfferItem"].Visible = false;
            dgvStockIn.Columns["cStrOfferDetails"].Visible = false;
            dgvStockIn.Columns["cBatchMode"].Visible = false;
            dgvStockIn.Columns["cBatchUnique"].Visible = false;
            dgvStockIn.Columns["cID"].Visible = false;

            dgvStockOut.Columns["cCGST"].Visible = false;
            dgvStockOut.Columns["cSGST"].Visible = false;
            dgvStockOut.Columns["cIGST"].Visible = false;
            dgvStockOut.Columns["ctaxPer"].Visible = false;
            dgvStockOut.Columns["ctax"].Visible = false;
            dgvStockOut.Columns["ctaxable"].Visible = false;
            dgvStockOut.Columns["cCRateWithTax"].Visible = false;
            dgvStockOut.Columns["cRateinclusive"].Visible = false;
            dgvStockOut.Columns["cItemID"].Visible = false;
            dgvStockOut.Columns["cFree"].Visible = false;
            dgvStockOut.Columns["cDiscPer"].Visible = false;
            dgvStockOut.Columns["cDiscAmount"].Visible = false;
            dgvStockOut.Columns["cBillDisc"].Visible = false;
            dgvStockOut.Columns["ctaxable"].Visible = false;
            dgvStockOut.Columns["ctaxPer"].Visible = false;
            dgvStockOut.Columns["ctax"].Visible = false;
            dgvStockOut.Columns["cIGST"].Visible = false;
            dgvStockOut.Columns["cSGST"].Visible = false;
            dgvStockOut.Columns["cCGST"].Visible = false;
            dgvStockOut.Columns["cGrossValueAfterRateDiscount"].Visible = false;
            dgvStockOut.Columns["cNonTaxable"].Visible = false;
            dgvStockOut.Columns["cCCessPer"].Visible = false;
            dgvStockOut.Columns["cCCompCessQty"].Visible = false;
            dgvStockOut.Columns["cFloodCessPer"].Visible = false;
            dgvStockOut.Columns["cFloodCessAmt"].Visible = false;
            dgvStockOut.Columns["cStockMRP"].Visible = false;
            dgvStockOut.Columns["cAgentCommPer"].Visible = false;
            dgvStockOut.Columns["cCoolie"].Visible = false;
            dgvStockOut.Columns["cBlnOfferItem"].Visible = false;
            dgvStockOut.Columns["cStrOfferDetails"].Visible = false;
            dgvStockOut.Columns["cBatchMode"].Visible = false;
            dgvStockOut.Columns["cBatchUnique"].Visible = false;
            dgvStockOut.Columns["cID"].Visible = false;

            if (AppSettings.NeedCostCenter == true)
                pnlCostCentre.Visible = true;
            else
                pnlCostCentre.Visible = false;

            dtpInvDate.MinDate = AppSettings.FinYearStart;
            dtpInvDate.MaxDate = AppSettings.FinYearEnd;
        }

        private void LoadDataFromJSon(string strJson = "")
        {
            DeserializeFromJSon(strJson);
        }

        //Description : Load Saved data from database from edit window or Navigation buttons
        private void LoadData(int iSelectedID = 0)
        {
            DataTable dtLoad = new DataTable();
            
            LoadBill(iSelectedID);

            iAction = 1;

            dgvStockIn.Columns["cRateinclusive"].Visible = false;
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

        #endregion
    }
}
