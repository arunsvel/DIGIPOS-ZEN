using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DigiposZen.Forms;
using DigiposZen.InventorBL.Master;
using DigiposZen.InventorBL.Accounts;
using DigiposZen.InventorBL.Helper;
using DigiposZen.Info;
using Syncfusion.WinForms.DataGrid;
using DigiposZen.JsonClass;
using Newtonsoft.Json;
using DataRow = System.Data.DataRow;
using DigiposZen.InventorBL.Transaction;
using System.Collections;
using Microsoft.VisualBasic;
using System.Runtime.InteropServices;
using System.Data.SqlClient;

namespace DigiposZen
{

    public partial class frmStockInVoucherNew : Form, IMessageFilter
    {
        //=============================================================================
        // Created By       : Dipu Joseph
        // Created On       : 02-Feb-2022
        // Last Edited On   :
        // Last Edited By   : Arun
        // Description      : Working With Different Voucher Type. Mainly For PURCHASE, PURCHASE RETURN, RECEIPT NOTE
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

        public frmStockInVoucherNew(int iVchTpeId = 0, int iTransID = 0, bool bFromEdit = false, object MDIParent = null)
        {
            try
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
                lblEffectiveDate.ForeColor = Color.Black;
                lblReferenceNo.ForeColor = Color.Black;

                btnprev.Image = global::DigiposZen.Properties.Resources.fast_backwards;
                btnNext.Image = global::DigiposZen.Properties.Resources.fast_forward;
                btnSave.Image = global::DigiposZen.Properties.Resources.save240402;
                btnPause.Image = global::DigiposZen.Properties.Resources.pause_button;
                btnPrint.Image = global::DigiposZen.Properties.Resources.printer_finalised;
                btnArchive.Image = global::DigiposZen.Properties.Resources.archive123;
                btnDelete.Image = global::DigiposZen.Properties.Resources.delete340402;
                btnFind.Image = global::DigiposZen.Properties.Resources.find_finalised_3030;
                btnMenu.Image = global::DigiposZen.Properties.Resources.menu_hamburger;
                btnSettings.Image = global::DigiposZen.Properties.Resources.settings_finalised;
                btnMinimize.Image = global::DigiposZen.Properties.Resources.minimize_finalised;
                btnClose.Image = global::DigiposZen.Properties.Resources.logout_Final;
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

            AddColumnsToGrid();
            ClearControls();

            bFromEditPurchase = bFromEdit;
            iIDFromEditWindow = iTransID;
            vchtypeID = iVchTpeId;

            if (iIDFromEditWindow != 0)
                txtPrefix.Tag = 1;
            else
                txtPrefix.Tag = 0;

            if (iTransID != 0)
            {
                FillTaxMode();
                FillCostCentre();
                FillEmployee();
                FillAgent();
                FillStates();

                SetTransactionsthatVarying();
                LoadData(iTransID);
                txtInvAutoNo.Select();
            }
            else
                SetTransactionsthatVarying();

            dgvPurchase.Controls.Add(dtp);
            dtp.Visible = false;
            dtp.Format = DateTimePickerFormat.Custom;
            
            lblPause.Text = "Pause";

            }
            catch (Exception ex)
            {

            }
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
        bool bFromEditPurchase;
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
        UspGetPurchaseInfo GetPurchaseIfo = new UspGetPurchaseInfo();

        clsItemMaster clsItmMst = new clsItemMaster();
        clsEmployee clsEmp = new clsEmployee();
        clsCostCentre clscct = new clsCostCentre();
        clsTaxMode clsTax = new clsTaxMode();
        clsAgentMaster clsAgent = new clsAgentMaster();
        clsLedger clsLedg = new clsLedger();
        clsUnitMaster clsUnit = new clsUnitMaster();
        clsStockDetails clsStock = new clsStockDetails();

        clsJSonCommon JSonComm = new clsJSonCommon();
        clsPurchase clsPur = new clsPurchase();

        //Purchase Master Related Classes for Json
        clsJSonPurchase clsPM = new clsJSonPurchase();
        clsJsonPMInfo clsJPMinfo = new clsJsonPMInfo();
        clsJsonPMLedgerInfo clsJPMLedgerinfo = new clsJsonPMLedgerInfo();
        clsJsonPMTaxmodeInfo clsJPMTaxModinfo = new clsJsonPMTaxmodeInfo();
        clsJsonPMAgentInfo clsJPMAgentinfo = new clsJsonPMAgentInfo();
        clsJsonPMCCentreInfo clsJPMCostCentreinfo = new clsJsonPMCCentreInfo();
        clsJsonPMEmployeeInfo clsJPMEmployeeinfo = new clsJsonPMEmployeeInfo();
        clsJsonPMStateInfo clsJPMStateinfo = new clsJsonPMStateInfo();

        //Purchase Detail Related Classes For Json
        clsJsonPDetailsInfo clsJPDinfo = new clsJsonPDetailsInfo();
        clsJsonPDUnitinfo clsJPDUnitinfo = new clsJsonPDUnitinfo();
        clsJsonPDIteminfo clsJPDIteminfo = new clsJsonPDIteminfo();

        DataTable dtItemPublic = new DataTable();
        DataTable dtUnitPublic = new DataTable();
        DataTable dtBatchCode = new DataTable();
        DataTable dtBatchCodeData = new DataTable();

        DateTimePicker dtp = new DateTimePicker();
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
                    AddColumnsToGrid();
                    FillTaxMode();
                    FillCostCentre();
                    FillEmployee();
                    FillAgent();
                    FillStates();

                    txtSupplier.ReadOnly = false;
                }

                SetTransactionDefaults();
                SetApplicationSettings();

                cboState.SelectedValue = AppSettings.StateCode;
                Application.DoEvents();

                GridInitialize_dgvColWidth();

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

                    int iRowCnt = dgvPurchase.Rows.Count;
                    dgvPurchase.CurrentCell = dgvPurchase.Rows[iRowCnt - 1].Cells[GetEnum(GridColIndexes.CItemCode)];
                    dgvPurchase.Focus();
                    SendKeys.Send("{down}");
                }
                dgvPurchase.Columns["cRateinclusive"].Visible = false;
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

        private void dgvPurchase_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            decimal dResult = 0;
            try
            {
                if (dgvPurchase.CurrentCell.ColumnIndex == GetEnum(GridColIndexes.cQty))
                {
                    dResult = Comm.ToDecimal(dgvPurchase.Rows[dgvPurchase.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cQty)].Value);
                    SetValue(GetEnum(GridColIndexes.cQty), dResult.ToString(), "QTY_FLOAT");

                    if (dgvPurchase.Rows.Count - 1 == dgvPurchase.CurrentRow.Index)
                        dgvPurchase.Rows.Add();

                    //Added by Anjitha 28/01/2022 5:30 PM
                    bool bshellife = ShelfLifeEffect();
                    if (bshellife == false)
                    {
                        dgvPurchase.Focus();
                        SetValue(GetEnum(GridColIndexes.CExpiry), dtCurrExp.ToString("dd-MMM-yyyy").ToString(), "DATE");
                    }

                    SendKeys.Send("{up}");
                    SendKeys.Send("{right}");
                }
                else if (dgvPurchase.CurrentCell.ColumnIndex == GetEnum(GridColIndexes.cFree))
                {
                    dResult = Comm.ToDecimal(dgvPurchase.Rows[dgvPurchase.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cFree)].Value);
                    SetValue(GetEnum(GridColIndexes.cFree), dResult.ToString(), "QTY_FLOAT");

                    SendKeys.Send("{up}");
                    SendKeys.Send("{right}");
                }
                else if (dgvPurchase.CurrentCell.ColumnIndex == GetEnum(GridColIndexes.cDiscPer))
                {
                    dResult = Comm.ToDecimal(dgvPurchase.Rows[dgvPurchase.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cGrossAmt)].Value) * (Comm.ToDecimal(dgvPurchase.Rows[dgvPurchase.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cDiscPer)].Value) / 100);

                    dgvPurchase.CellEndEdit -= dgvPurchase_CellEndEdit;
                    SetValue(GetEnum(GridColIndexes.cDiscAmount), dResult.ToString(), "CURR_FLOAT");
                    dgvPurchase.CellEndEdit += dgvPurchase_CellEndEdit;

                    SendKeys.Send("{up}");
                    SendKeys.Send("{right}");
                }
                else if (dgvPurchase.CurrentCell.ColumnIndex == GetEnum(GridColIndexes.cDiscAmount))
                {
                    dResult = (Comm.ToDecimal(dgvPurchase.Rows[dgvPurchase.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cDiscAmount)].Value) * 100) / Comm.ToDecimal(dgvPurchase.Rows[dgvPurchase.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cGrossAmt)].Value);

                    dgvPurchase.CellEndEdit -= dgvPurchase_CellEndEdit;
                    SetValue(GetEnum(GridColIndexes.cDiscPer), Comm.FormatAmt(dResult, ""), "");
                    dgvPurchase.CellEndEdit += dgvPurchase_CellEndEdit;

                    SendKeys.Send("{up}");
                    SendKeys.Send("{right}");
                }
                else if (dgvPurchase.CurrentCell.ColumnIndex == GetEnum(GridColIndexes.cMRP))
                {
                    SendKeys.Send("{up}");
                    SendKeys.Send("{right}");
                }
                else if (dgvPurchase.CurrentCell.ColumnIndex == GetEnum(GridColIndexes.cPrate))
                {
                    SendKeys.Send("{up}");
                    SendKeys.Send("{right}");
                }
                else if (dgvPurchase.CurrentCell.ColumnIndex == GetEnum(GridColIndexes.cSRate1Per)) 
                {
                    SendKeys.Send("{up}");
                    SendKeys.Send("{right}");
                }
                else if (dgvPurchase.CurrentCell.ColumnIndex == GetEnum(GridColIndexes.cSRate2Per))                
                {
                    SendKeys.Send("{up}");
                    SendKeys.Send("{right}");
                }
                else if (dgvPurchase.CurrentCell.ColumnIndex == GetEnum(GridColIndexes.cSRate3Per))                
                {
                    SendKeys.Send("{up}");
                    SendKeys.Send("{right}");
                }
                else if (dgvPurchase.CurrentCell.ColumnIndex == GetEnum(GridColIndexes.cSRate4Per))                
                {
                    SendKeys.Send("{up}");
                    SendKeys.Send("{right}");
                }
                else if (dgvPurchase.CurrentCell.ColumnIndex == GetEnum(GridColIndexes.cSRate5Per))                
                {
                    SendKeys.Send("{up}");
                    SendKeys.Send("{right}");
                }
                else if (dgvPurchase.CurrentCell.ColumnIndex == GetEnum(GridColIndexes.cSRate1))                
                {
                    SendKeys.Send("{up}");
                    SendKeys.Send("{right}");
                }
                else if (dgvPurchase.CurrentCell.ColumnIndex == GetEnum(GridColIndexes.cSRate2))                
                {
                    SendKeys.Send("{up}");
                    SendKeys.Send("{right}");
                }
                else if (dgvPurchase.CurrentCell.ColumnIndex == GetEnum(GridColIndexes.cSRate3))                
                {
                    SendKeys.Send("{up}");
                    SendKeys.Send("{right}");
                }
                else if (dgvPurchase.CurrentCell.ColumnIndex == GetEnum(GridColIndexes.cSRate4))                
                {
                    SendKeys.Send("{up}");
                    SendKeys.Send("{right}");
                }
                else if (dgvPurchase.CurrentCell.ColumnIndex == GetEnum(GridColIndexes.cSRate5))
                {
                    SendKeys.Send("{up}");
                    SendKeys.Send("{right}");
                }



                #region "Srate Calculation on vale changing in cells"

                //If the tag value of srate colums are 1, it won't get calculated in calctotal function.
                //Else srate(s) will be forward calculated according to percentages
                //Tag vale is set to "1" if the user enters vale in srate columns.

                if (dgvPurchase.Rows[dgvPurchase.CurrentCell.RowIndex].Cells[GetEnum(GridColIndexes.cSRate1)].Tag == null)
                    dgvPurchase.Rows[dgvPurchase.CurrentCell.RowIndex].Cells[GetEnum(GridColIndexes.cSRate1)].Tag = "";
                if (dgvPurchase.Rows[dgvPurchase.CurrentCell.RowIndex].Cells[GetEnum(GridColIndexes.cSRate2)].Tag == null)
                    dgvPurchase.Rows[dgvPurchase.CurrentCell.RowIndex].Cells[GetEnum(GridColIndexes.cSRate2)].Tag = "";
                if (dgvPurchase.Rows[dgvPurchase.CurrentCell.RowIndex].Cells[GetEnum(GridColIndexes.cSRate3)].Tag == null)
                    dgvPurchase.Rows[dgvPurchase.CurrentCell.RowIndex].Cells[GetEnum(GridColIndexes.cSRate3)].Tag = "";
                if (dgvPurchase.Rows[dgvPurchase.CurrentCell.RowIndex].Cells[GetEnum(GridColIndexes.cSRate4)].Tag == null)
                    dgvPurchase.Rows[dgvPurchase.CurrentCell.RowIndex].Cells[GetEnum(GridColIndexes.cSRate4)].Tag = "";
                if (dgvPurchase.Rows[dgvPurchase.CurrentCell.RowIndex].Cells[GetEnum(GridColIndexes.cSRate5)].Tag == null)
                    dgvPurchase.Rows[dgvPurchase.CurrentCell.RowIndex].Cells[GetEnum(GridColIndexes.cSRate5)].Tag = "";

                if (dgvPurchase.CurrentCell.ColumnIndex == GetEnum(GridColIndexes.cSRate1Per))
                    SetTag(GetEnum(GridColIndexes.cSRate1), dgvPurchase.CurrentRow.Index, "");
                else if (dgvPurchase.CurrentCell.ColumnIndex == GetEnum(GridColIndexes.cSRate2Per))
                    SetTag(GetEnum(GridColIndexes.cSRate2), dgvPurchase.CurrentRow.Index, "");
                else if (dgvPurchase.CurrentCell.ColumnIndex == GetEnum(GridColIndexes.cSRate3Per))
                    SetTag(GetEnum(GridColIndexes.cSRate3), dgvPurchase.CurrentRow.Index, "");
                else if (dgvPurchase.CurrentCell.ColumnIndex == GetEnum(GridColIndexes.cSRate4Per))
                    SetTag(GetEnum(GridColIndexes.cSRate4), dgvPurchase.CurrentRow.Index, "");
                else if (dgvPurchase.CurrentCell.ColumnIndex == GetEnum(GridColIndexes.cSRate5Per))
                    SetTag(GetEnum(GridColIndexes.cSRate5), dgvPurchase.CurrentRow.Index, "");

                if (dgvPurchase.CurrentCell.ColumnIndex == GetEnum(GridColIndexes.cSRate1))
                {
                    SetValue(GetEnum(GridColIndexes.cSRate1Per), dgvPurchase.CurrentRow.Index, CalculateSratePercentageOnSrate(dgvPurchase.CurrentCell.RowIndex, dgvPurchase.CurrentCell.ColumnIndex));
                    SetTag(GetEnum(GridColIndexes.cSRate1), dgvPurchase.CurrentRow.Index, "1");
                }
                else if (dgvPurchase.CurrentCell.ColumnIndex == GetEnum(GridColIndexes.cSRate2))
                {
                    SetValue(GetEnum(GridColIndexes.cSRate2Per), dgvPurchase.CurrentRow.Index, CalculateSratePercentageOnSrate(dgvPurchase.CurrentCell.RowIndex, dgvPurchase.CurrentCell.ColumnIndex));
                    SetTag(GetEnum(GridColIndexes.cSRate2), dgvPurchase.CurrentRow.Index, "1");
                }
                else if (dgvPurchase.CurrentCell.ColumnIndex == GetEnum(GridColIndexes.cSRate3))
                {
                    SetValue(GetEnum(GridColIndexes.cSRate3Per), dgvPurchase.CurrentRow.Index, CalculateSratePercentageOnSrate(dgvPurchase.CurrentCell.RowIndex, dgvPurchase.CurrentCell.ColumnIndex));
                    SetTag(GetEnum(GridColIndexes.cSRate3), dgvPurchase.CurrentRow.Index, "1");
                }
                else if (dgvPurchase.CurrentCell.ColumnIndex == GetEnum(GridColIndexes.cSRate4))
                {
                    SetValue(GetEnum(GridColIndexes.cSRate4Per), dgvPurchase.CurrentRow.Index, CalculateSratePercentageOnSrate(dgvPurchase.CurrentCell.RowIndex, dgvPurchase.CurrentCell.ColumnIndex));
                    SetTag(GetEnum(GridColIndexes.cSRate4), dgvPurchase.CurrentRow.Index, "1");
                }
                else if (dgvPurchase.CurrentCell.ColumnIndex == GetEnum(GridColIndexes.cSRate5))
                {
                    SetValue(GetEnum(GridColIndexes.cSRate5Per), dgvPurchase.CurrentRow.Index, CalculateSratePercentageOnSrate(dgvPurchase.CurrentCell.RowIndex, dgvPurchase.CurrentCell.ColumnIndex));
                    SetTag(GetEnum(GridColIndexes.cSRate5), dgvPurchase.CurrentRow.Index, "1");
                }

                //if (dgvPurchase.CurrentCell.ColumnIndex == GetEnum(GridColIndexes.cSRate1) || 
                //    dgvPurchase.CurrentCell.ColumnIndex == GetEnum(GridColIndexes.cSRate2) || 
                //    dgvPurchase.CurrentCell.ColumnIndex == GetEnum(GridColIndexes.cSRate3) || 
                //    dgvPurchase.CurrentCell.ColumnIndex == GetEnum(GridColIndexes.cSRate4) || 
                //    dgvPurchase.CurrentCell.ColumnIndex == GetEnum(GridColIndexes.cSRate5))
                //{
                //    double dblcSRate1 = Comm.ToDouble(dgvPurchase.Rows[dgvPurchase.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cSRate1)].Value);
                //    double dblcsRate2 = Comm.ToDouble(dgvPurchase.Rows[dgvPurchase.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cSRate2)].Value);
                //    double dblcsRate3 = Comm.ToDouble(dgvPurchase.Rows[dgvPurchase.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cSRate3)].Value);
                //    double dblcsRate4 = Comm.ToDouble(dgvPurchase.Rows[dgvPurchase.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cSRate4)].Value);
                //    double dblcsRate5 = Comm.ToDouble(dgvPurchase.Rows[dgvPurchase.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cSRate5)].Value);

                //    double dblcRate = Comm.ToDouble(dgvPurchase.Rows[dgvPurchase.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cPrate)].Value);
                //    double dblcCRate = Comm.ToDouble(dgvPurchase.Rows[dgvPurchase.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cCrate)].Value);
                //    double dblcMRP = Comm.ToDouble(dgvPurchase.Rows[dgvPurchase.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cMRP)].Value);
                //    double dblcCRateWithTax = Comm.ToDouble(dgvPurchase.Rows[dgvPurchase.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cCRateWithTax)].Value);

                //    switch (Comm.ToDouble(dgvPurchase.Rows[dgvPurchase.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cSRate1Per)].Tag)) //SrateCalcMode
                //    {
                //        case 0:
                //            if (dblcSRate1 > 0) SetValue(GetEnum(GridColIndexes.cSRate1Per), dgvPurchase.CurrentRow.Index, (((dblcSRate1 - dblcRate) * 100) / dblcSRate1).ToString());
                //            if (dblcsRate2 > 0) SetValue(GetEnum(GridColIndexes.cSRate2Per), dgvPurchase.CurrentRow.Index, (((dblcsRate2 - dblcRate) * 100) / dblcsRate2).ToString());
                //            if (dblcsRate3 > 0) SetValue(GetEnum(GridColIndexes.cSRate3Per), dgvPurchase.CurrentRow.Index, (((dblcsRate3 - dblcRate) * 100) / dblcsRate3).ToString());
                //            if (dblcsRate4 > 0) SetValue(GetEnum(GridColIndexes.cSRate4Per), dgvPurchase.CurrentRow.Index, (((dblcsRate4 - dblcRate) * 100) / dblcsRate4).ToString());
                //            if (dblcsRate5 > 0) SetValue(GetEnum(GridColIndexes.cSRate5Per), dgvPurchase.CurrentRow.Index, (((dblcsRate5 - dblcRate) * 100) / dblcsRate5).ToString());
                //            break;
                //        case 3:
                //            if (dblcSRate1 > 0) SetValue(GetEnum(GridColIndexes.cSRate1Per), dgvPurchase.CurrentRow.Index, Comm.FormatValue(((dblcSRate1 - dblcCRate) * 100) / dblcSRate1));
                //            if (dblcsRate2 > 0) SetValue(GetEnum(GridColIndexes.cSRate2Per), dgvPurchase.CurrentRow.Index, Comm.FormatValue(((dblcsRate2 - dblcCRate) * 100) / dblcsRate2));
                //            if (dblcsRate3 > 0) SetValue(GetEnum(GridColIndexes.cSRate3Per), dgvPurchase.CurrentRow.Index, Comm.FormatValue(((dblcsRate3 - dblcCRate) * 100) / dblcsRate3));
                //            if (dblcsRate4 > 0) SetValue(GetEnum(GridColIndexes.cSRate4Per), dgvPurchase.CurrentRow.Index, Comm.FormatValue(((dblcsRate4 - dblcCRate) * 100) / dblcsRate4));
                //            if (dblcsRate5 > 0) SetValue(GetEnum(GridColIndexes.cSRate5Per), dgvPurchase.CurrentRow.Index, Comm.FormatValue(((dblcsRate5 - dblcCRate) * 100) / dblcsRate5));
                //            break;
                //        case 1:
                //            if (dblcSRate1 > 0) SetValue(GetEnum(GridColIndexes.cSRate1Per), dgvPurchase.CurrentRow.Index, (((dblcMRP - dblcSRate1) * 100) / dblcMRP).ToString());
                //            if (dblcsRate2 > 0) SetValue(GetEnum(GridColIndexes.cSRate2Per), dgvPurchase.CurrentRow.Index, (((dblcMRP - dblcsRate2) * 100) / dblcMRP).ToString());
                //            if (dblcsRate3 > 0) SetValue(GetEnum(GridColIndexes.cSRate3Per), dgvPurchase.CurrentRow.Index, (((dblcMRP - dblcsRate3) * 100) / dblcMRP).ToString());
                //            if (dblcsRate4 > 0) SetValue(GetEnum(GridColIndexes.cSRate4Per), dgvPurchase.CurrentRow.Index, (((dblcMRP - dblcsRate4) * 100) / dblcMRP).ToString());
                //            if (dblcsRate5 > 0) SetValue(GetEnum(GridColIndexes.cSRate5Per), dgvPurchase.CurrentRow.Index, (((dblcMRP - dblcsRate5) * 100) / dblcMRP).ToString());
                //            break;
                //        case 2:
                //            if (dblcSRate1 > 0) SetValue(GetEnum(GridColIndexes.cSRate1Per), dgvPurchase.CurrentRow.Index, Comm.FormatValue(((dblcSRate1 - dblcCRateWithTax) * 100) / dblcSRate1));
                //            if (dblcsRate2 > 0) SetValue(GetEnum(GridColIndexes.cSRate2Per), dgvPurchase.CurrentRow.Index, Comm.FormatValue(((dblcsRate2 - dblcCRateWithTax) * 100) / dblcsRate2));
                //            if (dblcsRate3 > 0) SetValue(GetEnum(GridColIndexes.cSRate3Per), dgvPurchase.CurrentRow.Index, Comm.FormatValue(((dblcsRate3 - dblcCRateWithTax) * 100) / dblcsRate3));
                //            if (dblcsRate4 > 0) SetValue(GetEnum(GridColIndexes.cSRate4Per), dgvPurchase.CurrentRow.Index, Comm.FormatValue(((dblcsRate4 - dblcCRateWithTax) * 100) / dblcsRate4));
                //            if (dblcsRate5 > 0) SetValue(GetEnum(GridColIndexes.cSRate5Per), dgvPurchase.CurrentRow.Index, Comm.FormatValue(((dblcsRate5 - dblcCRateWithTax) * 100) / dblcsRate5));
                //            break;
                //    }
                //}

                #endregion

                this.dgvEndEditCell = dgvPurchase[e.ColumnIndex, e.RowIndex];
                if (dgvPurchase.Rows.Count == e.RowIndex && e.ColumnIndex != dgvPurchase.Columns.Count - 1 && e.ColumnIndex <= GetEnum(GridColIndexes.cDiscAmount))
                {
                    if (dgvPurchase.CurrentCell.ColumnIndex != GetEnum(GridColIndexes.cRateinclusive))
                        SendKeys.Send("{Tab}");
                }
                else if (e.ColumnIndex == GetEnum(GridColIndexes.cDiscAmount))
                {
                    dgvPurchase.CurrentCell = dgvPurchase[GetEnum(GridColIndexes.CItemCode), e.RowIndex + 1];
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
                double dblcSRate = Comm.ToDouble(dgvPurchase.Rows[RowIndex].Cells[ColIndex].Value);

                double dblcRate = Comm.ToDouble(dgvPurchase.Rows[dgvPurchase.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cPrate)].Value);
                double dblcCRate = Comm.ToDouble(dgvPurchase.Rows[dgvPurchase.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cCrate)].Value);
                double dblcMRP = Comm.ToDouble(dgvPurchase.Rows[dgvPurchase.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cMRP)].Value);
                double dblcCRateWithTax = Comm.ToDouble(dgvPurchase.Rows[dgvPurchase.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cCRateWithTax)].Value);

                switch (Comm.ToDouble(dgvPurchase.Rows[dgvPurchase.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cSRate1Per)].Tag)) //SrateCalcMode
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
            if (txtSupplier.Text != "")
            {
                if (txtSupplier.Text != strCheck)
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
            else
            {
                this.Close();
            }
        }

        private void txtSupplier_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.ActiveControl.Name == txtSupplier.Name)
                {
                    if (txtSupplier.Text != "")
                    {
                        if (Comm.ConvertI32(clsVchType.blnShowSearchWindowByDefault) == 1)
                        {
                            string sQuery = "SELECT LName+LAliasName+MobileNo+Address as AnyWhere,LALiasname as [Ledger Code],lname as [Ledger Name] ,MobileNo ,Address,LID,Email  FROM tblLedger L";
                            if (clsVchType.CustomerSupplierAccGroupList != "")
                                sQuery = sQuery + " INNER JOIN tblAccountGroup A ON A.AccountGroupID = L.AccountGroupID AND A.AccountGroupID IN (" + clsVchType.CustomerSupplierAccGroupList + ")";
                            else
                                sQuery = sQuery + " INNER JOIN tblAccountGroup A ON A.AccountGroupID = L.AccountGroupID AND (A.AccountGroupID IN (10,11) OR A.ParentID IN (10,11)) ";

                            sQuery = sQuery + " WHERE L.ActiveStatus=1 AND L.TenantID=" + Global.gblTenantID + "";

                            new frmCompactSearch(GetFromSupplierSearch, sQuery, "AnyWhere|LAliasName|LName|MobileNo|Address", txtSupplier.Location.X + 800, txtSupplier.Location.Y - 20, 4, 0, txtSupplier.Text, 4, 0, "ORDER BY L.LAliasName ASC", 0, 0, "Supplier Search ...", 0, "100,200,100,200,0", true, "frmSupplier").ShowDialog();

                            dgvPurchase.CurrentCell = dgvPurchase.Rows[0].Cells[GetEnum(GridColIndexes.CItemCode)];
                            dgvPurchase.Focus();
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

        private void textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            //try
            //{
            //    if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            //    {
            //        e.Handled = true;
            //    }
            //    if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            //    {
            //        e.Handled = true;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
            //    MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);

            //}
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
                if (dtpEffective.Enabled == true)
                    dtpEffective.Focus();
                else
                    dtpInvDate.Focus();
            }
            else if (e.KeyCode == Keys.Enter)
            {
                txtReferenceAutoNo.Focus();
                txtReferenceAutoNo.SelectAll();
            }
        }

        private void txtTaxRegn_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
            {
                txtMobile.Focus();
            }
            else if (e.KeyCode == Keys.Enter)
            {
                cboState.Focus();
                SendKeys.Send("{F4}");
            }
        }

        private void txtAddress1_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Shift == true && e.KeyCode == Keys.Enter)
                {
                    cboBType.Focus();
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    dgvPurchase.CurrentCell = dgvPurchase.Rows[0].Cells[GetEnum(GridColIndexes.CItemCode)];
                    dgvPurchase.Focus();
                }
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
        }

        private void dgvPurchase_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        ComboBox BatchCode_GridCellComboBox = new ComboBox();
        private void dgvPurchase_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            try
            {
                if (dgvPurchase.CurrentCell.EditType == typeof(DataGridViewTextBoxEditingControl))
                {
                    if (dgvPurchase.CurrentCell.ColumnIndex == GetEnum(GridColIndexes.CItemCode))
                    {
                        DataGridViewTextBoxEditingControl tb = (DataGridViewTextBoxEditingControl)e.Control;
                        tb.KeyPress += new KeyPressEventHandler(dgvPurchase_TextBox_KeyPress);
                        e.Control.KeyPress += new KeyPressEventHandler(dgvPurchase_TextBox_KeyPress);
                    }
                    else if (dgvPurchase.CurrentCell.ColumnIndex == GetEnum(GridColIndexes.cBarCode))
                    {
                        CallBatchCodeCompact();

                        if (dgvPurchase.Rows[dgvPurchase.CurrentRow.Index].Cells[GetEnum(GridColIndexes.CExpiry)].Visible == true)
                            dgvPurchase.CurrentCell = dgvPurchase.Rows[dgvPurchase.CurrentRow.Index].Cells[GetEnum(GridColIndexes.CExpiry)];
                        else
                            dgvPurchase.CurrentCell = dgvPurchase.Rows[dgvPurchase.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cQty)];

                        dgvPurchase.Focus();
                    }
                    else if (dgvPurchase.CurrentCell.ColumnIndex >= GetEnum(GridColIndexes.cMRP) && dgvPurchase.CurrentCell.ColumnIndex < GetEnum(GridColIndexes.cNetAmount))
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

        private void dgvPurchase_TextBox_KeyPress(object sender, KeyPressEventArgs e)
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
                if (dgvPurchase.CurrentCell.ColumnIndex == (int)GridColIndexes.CItemCode)
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
                            //new frmDetailedSearch2(GetFromItemSearch, sQuery, "Anywhere|ItemCode|ItemName|BatchUnique", dgvPurchase.Location.X + 55, dgvPurchase.Location.Y + 150, 7, 0, EnterText, 6, 0, "ORDER BY ItemCode ASC, Batchunique", 0, 0, "Item Search...", 0, "150,250,150,150,150,0,0", true, "frmItemMaster", 20, false, this.MdiParent, 6).ShowDialog(this.MdiParent);

                            if (mblnInitialisedSubWindow == false)
                            {
                                mblnInitialisedSubWindow = true;
                                frmDetailedSearch2 frmN = new frmDetailedSearch2(GetFromItemSearch, sQuery, "Anywhere|ItemCode|ItemName|BatchUnique", dgvPurchase.Location.X + 55, dgvPurchase.Location.Y + 150, 7, 0, EnterText, 6, 0, "ORDER BY ItemCode ASC, Batchunique", 0, 0, "Item Search...", 0, "150,250,150,150,150,0,0", true, "frmItemMaster", 20, false, this.MdiParent, 6);
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
                            //new frmDetailedSearch2(GetFromItemSearch, sQuery, "Anywhere|ItemCode|ItemName|BatchUnique", dgvPurchase.Location.X + 55, dgvPurchase.Location.Y + 150, 7, 0, EnterText, 6, 0, "ORDER BY ItemCode ASC, Batchunique", 0, 0, "Item Search...", 0, "150,250,150,150,150,0,0", true, "frmItemMaster", 20, false, this.MdiParent, 6).ShowDialog(this.MdiParent);

                            if (mblnInitialisedSubWindow == false)
                            {
                                mblnInitialisedSubWindow = true;
                                frmDetailedSearch2 frmN = new frmDetailedSearch2(GetFromItemSearch, sQuery, "Anywhere|ItemCode|ItemName|BatchUnique", dgvPurchase.Location.X + 55, dgvPurchase.Location.Y + 150, 7, 0, EnterText, 6, 0, "ORDER BY ItemCode ASC, Batchunique", 0, 0, "Item Search...", 0, "150,250,150,150,150,0,0", true, "frmItemMaster", 20, false, this.MdiParent, 6);
                                frmN.MdiParent = this.MdiParent;
                                frmN.Show(); //12-SEP-2022
                            }
                        }

                        if (dgvPurchase.CurrentRow != null)
                        {
                            if (dgvPurchase.Rows[dgvPurchase.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cItemID)].Value != null)
                            {
                                this.dgvPurchase.EditingControlShowing -= this.dgvPurchase_EditingControlShowing;

                                if (dgvPurchase.Rows[dgvPurchase.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cBarCode)].Visible == true)
                                    dgvPurchase.CurrentCell = dgvPurchase.Rows[dgvPurchase.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cBarCode)];
                                else
                                    dgvPurchase.CurrentCell = dgvPurchase.Rows[dgvPurchase.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cQty)];


                                dgvPurchase.Focus();
                                this.dgvPurchase.EditingControlShowing += this.dgvPurchase_EditingControlShowing;
                            }
                        }
                    }
                }
                else if (dgvPurchase.CurrentCell.ColumnIndex == (int)GridColIndexes.cBarCode)
                {
                    //sEditedValueonKeyPress = "~";
                    if (dgvPurchase.Rows[dgvPurchase.CurrentRow.Index].Cells[(int)GridColIndexes.cBarCode].Value != null)
                        sEditedValueonKeyPress = dgvPurchase.Rows[dgvPurchase.CurrentRow.Index].Cells[(int)GridColIndexes.cBarCode].Value.ToString();
                    else
                        sEditedValueonKeyPress = "";
                    if (sEditedValueonKeyPress != null)
                    {
                        if (dgvPurchase.Rows[dgvPurchase.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cItemID)].Value != null)
                        {
                            Form fcC = Application.OpenForms["frmDetailedSearch2"];
                            if (fcC != null)
                            {
                                fcC.Focus();
                                fcC.BringToFront();
                                return;
                            }

                            CallBatchCodeCompact();

                            if (dgvPurchase.Rows[dgvPurchase.CurrentRow.Index].Cells[GetEnum(GridColIndexes.CExpiry)].Visible == true)
                                dgvPurchase.CurrentCell = dgvPurchase.Rows[dgvPurchase.CurrentRow.Index].Cells[GetEnum(GridColIndexes.CExpiry)];
                            else
                                dgvPurchase.CurrentCell = dgvPurchase.Rows[dgvPurchase.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cQty)];

                            dgvPurchase.Focus();
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

        private void dgvPurchase_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
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

        private void dgvPurchase_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Shift && e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    int iColumn = dgvPurchase.CurrentCell.ColumnIndex;
                    int iRow = dgvPurchase.CurrentCell.RowIndex;
                    if (iColumn == GetEnum(GridColIndexes.cRateinclusive))
                    {
                        if (dgvPurchase[GetEnum(GridColIndexes.cRateinclusive) - 1, iRow].Visible == true)
                            dgvPurchase.CurrentCell = dgvPurchase[GetEnum(GridColIndexes.cRateinclusive) - 1, iRow];
                        else
                            dgvPurchase.CurrentCell = dgvPurchase[1, iRow - 1];
                    }
                    else if (iColumn == dgvPurchase.Columns.Count - 1 && iRow != dgvPurchase.Rows.Count)
                        dgvPurchase.CurrentCell = dgvPurchase[1, iRow - 1];
                    else
                        SendKeys.Send("+{Tab}");
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    int iColumn = dgvPurchase.CurrentCell.ColumnIndex;
                    int iRow = dgvPurchase.CurrentCell.RowIndex;
                    if (iColumn == dgvPurchase.Columns.Count - 1 && iRow != dgvPurchase.Rows.Count)
                    {
                        dgvPurchase.CurrentCell = dgvPurchase[1, iRow + 1];
                    }
                    else if (iColumn == dgvPurchase.Columns.Count - 1 && iRow == dgvPurchase.Rows.Count)
                    {
                    }
                    else if (iColumn == GetEnum(GridColIndexes.cDiscAmount))
                    {
                        //Dipoos 22-03-2022----- >
                        dgvPurchase.Rows.Add();
                        dgvPurchase.CurrentCell = dgvPurchase[GetEnum(GridColIndexes.CItemCode), iRow + 1];
                    }
                    else if (iColumn == GetEnum(GridColIndexes.cRateinclusive))
                    {
                        if (dgvPurchase[GetEnum(GridColIndexes.cRateinclusive) + 1, iRow].Visible == true)
                            dgvPurchase.CurrentCell = dgvPurchase[GetEnum(GridColIndexes.cRateinclusive) + 1, iRow];
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
                //    if (dgvPurchase.CurrentCell.ColumnIndex == (int)GridColIndexes.CItemCode)
                //    {
                //        frmItemMaster frmim = new frmItemMaster(0, true, "S");
                //        frmim.ShowDialog();
                //    }
                //}
                //else if (e.KeyCode == Keys.F4)
                //{
                //    if (dgvPurchase.CurrentCell.ColumnIndex == (int)GridColIndexes.CItemCode)
                //    {
                //        int iSelectedItemID = 0;
                //        iSelectedItemID = Comm.ToInt32(dgvPurchase.Rows[dgvPurchase.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cItemID)].Value);
                //        if (iSelectedItemID > 0)
                //        {
                //            frmItemMaster frmIM = new frmItemMaster(iSelectedItemID, true, "E");
                //            frmIM.ShowDialog();
                //        }
                //    }
                //}
                else if (e.KeyCode == Keys.Delete)
                {
                    string SSelectedItemCode = Convert.ToString(dgvPurchase.Rows[dgvPurchase.CurrentRow.Index].Cells[GetEnum(GridColIndexes.CItemCode)].Value);
                    if (SSelectedItemCode != "")
                    {
                        DialogResult dlgResult = MessageBox.Show("Are you sure to delete Item Code[" + SSelectedItemCode + "] Permanently ?", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                        if (dlgResult == DialogResult.Yes)
                        {
                            Int32 selectedRowCount = dgvPurchase.Rows.GetRowCount(DataGridViewElementStates.Selected);
                            RowDelete();
                            //dipoos 21-03-2022
                            //if (dgvPurchase.Rows.Count < 2)
                            //    dgvPurchase.Rows.Add();
                            if (dgvPurchase.Rows.Count < 1)
                                dgvPurchase.Rows.Add();

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

                    if (dgvPurchase.CurrentCell.ColumnIndex == (int)GridColIndexes.CItemCode)
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
                                //new frmDetailedSearch2(GetFromItemSearch, sQuery, "Anywhere|ItemCode|ItemName|BatchUnique", dgvPurchase.Location.X + 55, dgvPurchase.Location.Y + 150, 7, 0, dgvPurchase.CurrentCell.Value.ToString(), 6, 0, "ORDER BY ItemCode ASC, Batchunique", 0, 0, "Item Search...", 0, "150,250,150,150,150,0,0", true, "frmItemMaster", 20, false, this.MdiParent, 6).ShowDialog(this.MdiParent);

                                if (mblnInitialisedSubWindow == false)
                                {
                                    mblnInitialisedSubWindow = true;
                                    string enteredtext = "";
                                    if (dgvPurchase.CurrentCell.Value != null)
                                        enteredtext = dgvPurchase.CurrentCell.Value.ToString();
                                    frmDetailedSearch2 frmN = new frmDetailedSearch2(GetFromItemSearch, sQuery, "Anywhere|ItemCode|ItemName|BatchUnique", dgvPurchase.Location.X + 55, dgvPurchase.Location.Y + 150, 7, 0, enteredtext, 6, 0, "ORDER BY ItemCode ASC, Batchunique", 0, 0, "Item Search...", 0, "150,250,150,150,150,0,0", true, "frmItemMaster", 20, false, this.MdiParent, 6);
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
                                //new frmDetailedSearch2(GetFromItemSearch, sQuery, "Anywhere|ItemCode|ItemName|BatchUnique", dgvPurchase.Location.X + 55, dgvPurchase.Location.Y + 150, 7, 0, dgvPurchase.CurrentCell.Value.ToString(), 6, 0, "ORDER BY ItemCode ASC, Batchunique", 0, 0, "Item Search...", 0, "150,250,150,150,150,0,0", true, "frmItemMaster", 20, false, this.MdiParent, 6);

                                if (mblnInitialisedSubWindow == false)
                                {
                                    mblnInitialisedSubWindow = true;
                                    frmDetailedSearch2 frmN = new frmDetailedSearch2(GetFromItemSearch, sQuery, "Anywhere|ItemCode|ItemName|BatchUnique", dgvPurchase.Location.X + 55, dgvPurchase.Location.Y + 150, 7, 0, dgvPurchase.CurrentCell.Value.ToString(), 6, 0, "ORDER BY ItemCode ASC, Batchunique", 0, 0, "Item Search...", 0, "150,250,150,150,150,0,0", true, "frmItemMaster", 20, false, this.MdiParent, 6);
                                    frmN.MdiParent = this.MdiParent;
                                    frmN.Show(); //12-SEP-2022
                                }
                            }


                            if (dgvPurchase.Rows[dgvPurchase.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cItemID)].Value != null)
                            {
                                this.dgvPurchase.EditingControlShowing -= this.dgvPurchase_EditingControlShowing;

                                if (dgvPurchase.Rows[dgvPurchase.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cBarCode)].Visible == true)
                                    dgvPurchase.CurrentCell = dgvPurchase.Rows[dgvPurchase.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cBarCode)];
                                else
                                    dgvPurchase.CurrentCell = dgvPurchase.Rows[dgvPurchase.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cQty)];

                                dgvPurchase.Focus();
                                this.dgvPurchase.EditingControlShowing += this.dgvPurchase_EditingControlShowing;
                            }
                        }
                    }
                    else if (dgvPurchase.CurrentCell.ColumnIndex == GetEnum(GridColIndexes.cBarCode))
                    {
                        Form fc = Application.OpenForms["frmDetailedSearch2"];
                        if (fc != null)
                        {
                            fcc.Focus();
                            fcc.BringToFront();
                            return;
                        }
                        // BatchCode List Will Work only to MNF and Auto BatchMode Cases... Asper Discuss with Anup sir and Team on 13-May-2022 Evening Meeting.
                        if (Comm.ToInt32(dgvPurchase.Rows[dgvPurchase.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cBatchMode)].Value) == 1) // MNF
                            CallBatchCodeCompact(true);
                        else if (Comm.ToInt32(dgvPurchase.Rows[dgvPurchase.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cBatchMode)].Value) == 2) // Auto
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

        private void dgvPurchase_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                //int iColumn = dgvPurchase.CurrentCell.ColumnIndex;
                //int iRowNo = dgvPurchase.CurrentCell.RowIndex;
                //if (iColumn == GetEnum(GridColIndexes.cQty))
                //{
                //    if (iRowNo < 0)
                //    {
                //        iRowNo = 0;
                //        if (dgvPurchase.Rows.Count <= iRowNo + 1)
                //            dgvPurchase.Rows.Add();
                //    }
                //    else
                //    {
                //        if (clsVchTypeFeatures.BLNSHOWFREEQUANTITY == true)
                //        {
                //            if (dgvPurchase[iColumn + 1, iRowNo].Visible == true)
                //                dgvPurchase.CurrentCell = dgvPurchase[iColumn + 1, iRowNo];
                //        }
                //        else
                //        {
                //            if (dgvPurchase[iColumn + 2, iRowNo].Visible == true)
                //                dgvPurchase.CurrentCell = dgvPurchase[iColumn + 2, iRowNo];
                //            else
                //                dgvPurchase.CurrentCell = dgvPurchase[iColumn + 1, iRowNo];
                //        }
                //    }
                //}

                //    //Added by Dipu Joseph on 14-Feb-2021 5.08 PM ---------- >>
                //    int iRow = 0;

                //    if (dgvPurchase.CurrentCell != null)
                //    {
                //        int iColumn = dgvPurchase.CurrentCell.ColumnIndex;
                //        int iRowNo = dgvPurchase.CurrentCell.RowIndex;

                //        if (dgvPurchase[GetEnum(GridColIndexes.CItemName), dgvPurchase.CurrentCell.RowIndex].Tag == null)
                //            dgvPurchase[GetEnum(GridColIndexes.CItemName), dgvPurchase.CurrentCell.RowIndex].Tag = "";
                //        if (dgvPurchase[GetEnum(GridColIndexes.CItemName), dgvPurchase.CurrentCell.RowIndex].Tag.ToString() == "")
                //        {
                //            dgvPurchase.CurrentCell = dgvPurchase[1, dgvPurchase.CurrentCell.RowIndex];
                //            return;
                //        }

                //        if (this._EnterMoveNext && MouseButtons == 0)
                //        {
                //            if (this.dgvEndEditCell != null && dgvPurchase.CurrentCell != null)
                //            {
                //                if (dgvPurchase.CurrentCell.RowIndex == this.dgvEndEditCell.RowIndex + 1
                //                    && dgvPurchase.CurrentCell.ColumnIndex == this.dgvEndEditCell.ColumnIndex)
                //                {
                //                    int iColNew;
                //                    int iRowNew;
                //                    if (this.dgvEndEditCell.ColumnIndex >= dgvPurchase.ColumnCount - 1)
                //                    {
                //                        iColNew = 0;
                //                        iRowNew = dgvPurchase.CurrentCell.RowIndex;
                //                    }
                //                    else
                //                    {
                //                        iColNew = this.dgvEndEditCell.ColumnIndex + 1;
                //                        iRow = this.dgvEndEditCell.RowIndex;
                //                    }

                //                    //    if (dgvPurchase[GetEnum(GridColIndexes.CItemName), dgvPurchase.CurrentCell.RowIndex].Tag == null)
                //                    //    dgvPurchase[GetEnum(GridColIndexes.CItemName), dgvPurchase.CurrentCell.RowIndex].Tag = "";
                //                    //if (dgvPurchase[GetEnum(GridColIndexes.CItemName), dgvPurchase.CurrentCell.RowIndex].Tag.ToString() == "")
                //                    //{
                //                    //    iColNew = 1;
                //                    //    iRowNew = dgvPurchase.CurrentCell.RowIndex;
                //                    //}

                //                    if (iColumn >= dgvPurchase.Columns.Count - 2)
                //                        dgvPurchase.CurrentCell = dgvPurchase[1, iRowNo + 1];
                //                    else
                //                    {
                //                        if (iColumn == GetEnum(GridColIndexes.cPrate))
                //                        {
                //                            SendKeys.Send("{Tab}");
                //                            //dgvPurchase.CurrentCell = dgvPurchase[iColumn + 1, iRow];
                //                        }
                //                        else if (iColumn == GetEnum(GridColIndexes.cMRP))
                //                        {
                //                            SendKeys.Send("{Tab}");
                //                            //dgvPurchase.CurrentCell = dgvPurchase[iColumn + 1, iRow];
                //                        }
                //                        else if (iColumn == GetEnum(GridColIndexes.cQty))
                //                        {
                //                            if (iRow < 0)
                //                            {
                //                                iRow = 0;
                //                                //dgvPurchase.CurrentCell = dgvPurchase[GetEnum(GridColIndexes.CItemCode), iRow];
                //                                if (dgvPurchase.Rows.Count <= iRow + 1)
                //                                    dgvPurchase.Rows.Add();
                //                            }
                //                            else
                //                            {
                //                                if (clsVchTypeFeatures.BLNSHOWFREEQUANTITY == true)
                //                                {
                //                                    if (dgvPurchase[iColumn + 1, iRow].Visible == true)
                //                                        dgvPurchase.CurrentCell = dgvPurchase[iColumn + 1, iRow];
                //                                    //else
                //                                    //        SendKeys.Send("{Tab}");

                //                                }
                //                                else
                //                                {
                //                                    if (dgvPurchase[iColumn + 2, iRow].Visible == true)
                //                                        dgvPurchase.CurrentCell = dgvPurchase[iColumn + 2, iRow];
                //                                    else
                //                                        dgvPurchase.CurrentCell = dgvPurchase[iColumn + 1, iRow];
                //                                }
                //                            }
                //                        }
                //                    }
                //                }
                //            }
                //        }
                //    }
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
        }

        private void dgvPurchase_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (dgvPurchase.CurrentCell.ColumnIndex == GetEnum(GridColIndexes.CExpiry))
                {
                    if (dgvPurchase.Rows[dgvPurchase.CurrentRow.Index].Cells[GetEnum(GridColIndexes.CExpiry)].ReadOnly == false)
                    {
                        _Rectangle = dgvPurchase.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true); //  
                        dtp.Size = new Size(_Rectangle.Width, _Rectangle.Height); //  
                        dtp.Location = new Point(_Rectangle.X, _Rectangle.Y); //  
                        dtp.Visible = true;
                        dtp.TextChanged += new EventHandler(dtp_TextChange);
                    }
                }
                if (dgvPurchase.CurrentCell.ColumnIndex == GetEnum(GridColIndexes.cSlNo))
                {
                    dgvPurchase.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                    dgvPurchase.Rows[dgvPurchase.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cSlNo)].ReadOnly = true;

                    strSelectedItemName = Convert.ToString(dgvPurchase.Rows[dgvPurchase.CurrentRow.Index].Cells[GetEnum(GridColIndexes.CItemCode)].Value);
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
            dgvPurchase.CurrentRow.Cells[GetEnum(GridColIndexes.CExpiry)].Value = dtp.Text.ToString();
            dtp.Visible = false;
        }

        private void cboPayment_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
            {
                if (txtReferenceAutoNo.Visible == true)
                {
                    txtReferenceAutoNo.Focus();
                    txtReferenceAutoNo.SelectAll();
                }
                else
                    if (dtpEffective.Enabled == true)
                    dtpEffective.Focus();
                else
                    dtpInvDate.Focus();
            }
            else if (e.KeyCode == Keys.Enter)
            {
                if (AppSettings.TaxEnabled == true)
                {
                    if (cboTaxMode.Enabled == true)
                        cboTaxMode.Focus();
                    else
                    {
                        if (cboCostCentre.Enabled == true)
                            cboCostCentre.Focus();
                        else
                        {
                            if (cboSalesStaff.Enabled == true)
                                cboSalesStaff.Focus();
                            else
                            {
                                if (cboAgent.Enabled == true)
                                    cboAgent.Focus();
                                else
                                {
                                    txtSupplier.Focus();
                                }
                            }
                        }
                    }
                    SendKeys.Send("{F4}");
                }
                else
                {
                    if (AppSettings.NeedCostCenter == true)
                        cboCostCentre.Focus();
                    else
                        cboSalesStaff.Focus();

                    SendKeys.Send("{F4}");
                }
            }
        }

        private void cboTaxMode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
            {
                cboPayment.Focus();
                SendKeys.Send("{F4}");
            }
            else if (e.KeyCode == Keys.Enter)
            {
                if (AppSettings.NeedCostCenter == true)
                {
                    if (cboCostCentre.Enabled == false)
                    {
                        cboSalesStaff.Focus();
                        SendKeys.Send("{F4}");
                    }
                    else
                    {
                        cboCostCentre.Focus();
                        SendKeys.Send("{F4}");
                    }
                    //SendKeys.Send("{F4}");
                }
                else
                {
                    cboSalesStaff.Focus();
                    SendKeys.Send("{F4}");
                }
            }
        }

        private void cboCostCentre_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
            {
                cboTaxMode.Focus();
                SendKeys.Send("{F4}");
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
                if (pnlCostCentre.Visible == true)
                    cboCostCentre.Focus();
                else
                    cboTaxMode.Focus();

                SendKeys.Send("{F4}");
            }
            else if (e.KeyCode == Keys.Enter)
            {
                if (AppSettings.NeedAgent == true)
                {
                    cboAgent.Focus();
                    SendKeys.Send("{F4}");
                }
                else
                {
                    txtSupplier.Focus();
                }
            }
        }

        private void txtOtherExp_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                    CalcTotal();
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
        }

        private void txtDiscPerc_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (txtDiscPerc.Text.Trim() != ".")
                {
                    if (txtDiscPerc.Text == "")
                    {
                        txtDiscPerc.Text = "0";
                        txtDiscPerc.SelectAll();
                    }
                    if (txtGrossAfterItmDisc.Text == "") txtGrossAfterItmDisc.Text = "0";
                    if (txtGrossAmt.Text == "") txtGrossAmt.Text = "0";
                    if (lblQtyTotal.Text == "") lblQtyTotal.Text = "0";
                    if (lblFreeTotal.Text == "") lblFreeTotal.Text = "0";
                    if (Comm.ToDouble(txtGrossAfterItmDisc.Text) > 0)
                    {
                        this.txtDiscAmt.TextChanged -= this.txtDiscAmt_TextChanged;
                        txtDiscAmt.Text = Comm.FormatValue((Comm.ToDouble(txtGrossAfterItmDisc.Text) * Comm.ToDouble(txtDiscPerc.Text) / 100));
                        this.txtDiscAmt.TextChanged += this.txtDiscAmt_TextChanged;
                    }
                    else
                    {
                        if (Comm.ToDouble(txtDiscPerc.Text) > 0)
                        {
                            this.txtDiscAmt.TextChanged -= this.txtDiscAmt_TextChanged;
                            txtDiscAmt.Text = Comm.FormatValue((Comm.ToDouble(txtGrossAmt.Text) * Comm.ToDouble(txtDiscPerc.Text) / 100));
                            this.txtDiscAmt.TextChanged += this.txtDiscAmt_TextChanged;
                        }
                    }
                    if (txtDiscAmt.Text == "") txtDiscAmt.Text = "0";
                }
                //if (Comm.ToDecimal(txtDiscAmt.Text) > 0)
                CalcTotal();
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
        }

        private void txtRoundOff_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Shift == true && e.KeyCode == Keys.Enter)
                {
                    txtCashDisc.Focus();
                    txtCashDisc.SelectAll();
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    CalcTotal();
                    txtNarration.Focus();
                }
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void txtDiscAmt_TextChanged(object sender, EventArgs e)
        {
            try
            {
                //    if (txtDiscAmt.Text.Trim() != ".")
                //    {
                //        if (txtDiscAmt.Text == "") { txtDiscAmt.Text = "0"; txtDiscAmt.SelectAll(); }
                //        if (Comm.ToDecimal(txtDiscAmt.Text) > 0)
                //        {
                //            this.txtDiscAmt.TextChanged -= this.txtDiscAmt_TextChanged;
                //            CalcTotal();
                //            this.txtDiscAmt.TextChanged += this.txtDiscAmt_TextChanged;
                //        }
                //    }

                //CalcTotal();
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
        }

        private void txtOtherExp_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (txtOtherExp.Text.Trim() != ".")
                {
                    if (txtOtherExp.Text == "") { txtOtherExp.Text = "0"; txtOtherExp.SelectAll(); }
                    CalcTotal();
                }
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
        }

        private void txtCostFactor_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (txtCostFactor.Text.Trim() != ".")
                {
                    if (txtCostFactor.Text == "") { txtCostFactor.Text = "0"; txtCostFactor.SelectAll(); }
                    CalcTotal();
                }
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void txtCashDisc_TextChanged(object sender, EventArgs e)
        {
            try
            {
                //if (txtcashDisper.Text.Trim() != ".")
                //{
                //    if (txtcashDisper.Text == "") { txtcashDisper.Text = "0"; txtcashDisper.SelectAll(); }
                //    if (Comm.ToDouble(txtcashDisper.Text) >= 0)
                //        txtCashDisc.Text = Comm.FormatValue((Comm.ToDouble(txtNetAmt.Text) * Comm.ToDouble(txtcashDisper.Text) / 100));
                //}
                if (txtCashDisc.Text.Trim() != ".")
                {
                    if (txtCashDisc.Text == "") { txtCashDisc.Text = "0"; txtCashDisc.SelectAll(); }
                    if (Comm.ToDouble(txtCashDisc.Text) > 0)
                    {
                        this.txtcashDisper.TextChanged -= this.txtcashDisper_TextChanged;
                        txtcashDisper.Text = Comm.FormatValue((Comm.ToDouble(txtCashDisc.Text) * 100) / Comm.ToDouble(txtNetAmt.Text));
                        this.txtcashDisper.TextChanged += this.txtcashDisper_TextChanged;
                    }
                    
                    CalcTotal();
                }
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
        }

        private void txtRoundOff_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (txtRoundOff.Text == "") { txtRoundOff.Text = "0"; txtRoundOff.SelectAll(); }
                if (lblBillAmount.Text == "") lblBillAmount.Text = "0";
                if (Conversion.Val(txtRoundOff.Text.ToString()) != 0 || txtRoundOff.Text.ToString() == "0")
                {
                    if (txtRoundOff.Text != ".")
                        lblBillAmount.Text = Comm.FormatValue((Comm.ToDouble(lblBillAmount.Text) + Conversion.Val(txtRoundOff.Text.ToString())));

                    CalcTotal();
                }

            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
        }

        private void txtDiscAmt_KeyDown_1(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
            {
                txtDiscPerc.Focus();
                txtDiscPerc.SelectAll();
            }
            else if (e.KeyCode == Keys.Enter)
            {
                txtOtherExp.Focus();
                txtOtherExp.SelectAll();
            }
        }

        private void txtOtherExp_KeyDown_1(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
            {
                if (tblpDiscAmt.Visible == true)
                {
                    txtDiscAmt.Focus();
                    txtDiscAmt.SelectAll();
                }
                else
                {
                    txtDiscPerc.Focus();
                    txtDiscPerc.SelectAll();
                }
            }
            else if (e.KeyCode == Keys.Enter)
            {
                txtCostFactor.Focus();
                txtCostFactor.SelectAll();
            }
        }

        private void txtNarration_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
            {
                txtRoundOff.Focus();
                txtRoundOff.SelectAll();
            }
            else if (e.KeyCode == Keys.Enter)
            {
                txtInstantReceipt.Focus();
                txtInstantReceipt.SelectAll();
            }
        }

        private void cboAgent_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
            {
                cboSalesStaff.Focus();
                SendKeys.Send("{F4}");
            }
            else if (e.KeyCode == Keys.Enter)
                txtSupplier.Focus();
        }

        private void LoadTest()
        {
            iIDFromEditWindow = 0;
            if (iIDFromEditWindow == 0)
            {
                for (int i = 0; i < 100000; i++)
                {
                    txtInvAutoNo.Text = Comm.gfnGetNextSerialNo("tblPurchase", "AutoNum", " VchTypeID=" + vchtypeID).ToString();
                    CRUD_Operations(0, true);

                    lblHeading.Text = "Purchase " + i.ToString() + " / 100000 ";

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

        private void dgvPurchase_ColumnWidthChanged(object sender, DataGridViewColumnEventArgs e)
        {
            try
            {
                if (this.ActiveControl == null) return;
                if (this.ActiveControl.Name != dgvPurchase.Name) return;
            }
            catch
            { }

            try
            {
                dtp.Visible = false;
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

        private void dgvPurchase_Scroll(object sender, ScrollEventArgs e)
        {
            dtp.Visible = false;
        }

        private void txtSupplier_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
            {
                if (pnlAgent.Visible == true)
                    cboAgent.Focus();
                else
                    cboSalesStaff.Focus();
                SendKeys.Send("{F4}");
            }
            else if (e.KeyCode == Keys.Enter)
            {
                if (txtMobile.Text != "")
                {
                    dgvPurchase.CurrentCell = dgvPurchase.Rows[0].Cells[GetEnum(GridColIndexes.CItemCode)];
                    dgvPurchase.Focus();
                }
                else
                    txtMobile.Focus();
            }
            else if (e.KeyCode == Keys.F3)
            {
                btnNewIcon.PerformClick();
            }
            else if (e.KeyCode == Keys.F4)
            {
                btnEditIcon.PerformClick();
            }

        }

        private void cboState_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
            {
                txtTaxRegn.Focus();
            }
            else if (e.KeyCode == Keys.Enter)
            {
                cboBType.Focus();
                SendKeys.Send("{F4}");
            }
        }

        private void txtCostFactor_KeyDown_1(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
            {
                txtOtherExp.Focus();
                txtOtherExp.SelectAll();
            }
            else if (e.KeyCode == Keys.Enter)
            {
                txtcashDisper.Focus();
                txtcashDisper.SelectAll();

            }
        }

        private void txtInstantReceipt_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
            {
                txtNarration.Focus();
            }
            else if (e.KeyCode == Keys.Enter)
            {
                btnSave.Focus();
            }
        }

        private void dgvPurchase_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                string sQuery = "";
                Cursor.Current = Cursors.WaitCursor;
                double dSelectedItemID = 0;
                if (dgvPurchase.Rows[dgvPurchase.CurrentRow.Index].Cells[GetEnum(GridColIndexes.CItemCode)].Value != null)
                {
                    dSelectedItemID = Comm.ToDouble(dgvPurchase.Rows[dgvPurchase.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cItemID)].Value);
                    if (dSelectedItemID > 0)
                    {
                        if (dgvPurchase.CurrentCell.ColumnIndex == (int)GridColIndexes.CItemCode)
                        {
                            frmItemMaster frmIM = new frmItemMaster(Comm.ToInt32(dSelectedItemID), true, "E");
                            frmIM.ShowDialog();
                        }
                        else if (dgvPurchase.CurrentCell.ColumnIndex == (int)GridColIndexes.CItemName)
                        {
                            frmItemMaster frmIM = new frmItemMaster(Comm.ToInt32(dSelectedItemID), true, "E");
                            frmIM.ShowDialog();
                        }
                    }
                }
                //else if (dgvPurchase.CurrentCell.ColumnIndex == GetEnum(GridColIndexes.cBarCode))
                //{
                //    Form fc = Application.OpenForms["frmDetailedSearch2"];
                //    if (fc != null)
                //        return;

                //    CallBatchCodeCompact();
                //}
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

        private void txtRoundOff_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                e.Handled = Comm.CheckNumeric(sender, e, true);
            }
            catch
            {

            }
        }

        private void txtcashDisper_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                e.Handled = Comm.CheckNumeric(sender, e);
            }
            catch
            {

            }
        }

        private void txtcashDisper_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
            {
                txtCostFactor.Focus();
                txtCostFactor.SelectAll();
            }
            else if (e.KeyCode == Keys.Enter)
            {
                txtCashDisc.Focus();
                txtCashDisc.SelectAll();
            }
        }

        private void txtDiscPerc_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
            {
                dgvPurchase.Focus();
            }
            else if (e.KeyCode == Keys.Enter)
            {
                if (tblpDiscAmt.Enabled == true)
                {
                    txtDiscAmt.Focus();
                    txtDiscAmt.SelectAll();
                }
                else
                {
                    txtOtherExp.Focus();
                    txtOtherExp.SelectAll();
                }
            }
        }

        private void dgvPurchase_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvPurchase.CurrentCell.ColumnIndex == GetEnum(GridColIndexes.cImgDel))
            {
                string SSelectedItemCode = Convert.ToString(dgvPurchase.Rows[dgvPurchase.CurrentRow.Index].Cells[GetEnum(GridColIndexes.CItemCode)].Value);
                if (SSelectedItemCode != "")
                {
                    DialogResult dlgResult = MessageBox.Show("Are you sure to delete Item Code[" + SSelectedItemCode + "] Permanently ?", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                    if (dlgResult == DialogResult.Yes)
                    {

                        Int32 selectedRowCount = dgvPurchase.Rows.GetRowCount(DataGridViewElementStates.Selected);
                        RowDelete();

                        dgvPurchase.Rows.Add();
                        dgvPurchase.CurrentCell = dgvPurchase.Rows[0].Cells[GetEnum(GridColIndexes.CItemCode)];

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

        private void txtMobile_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
            {
                txtSupplier.Focus();
            }
            else if (e.KeyCode == Keys.Enter)
            {
                if (clsVchTypeFeatures.blnpartydetails == false)
                {
                    dgvPurchase.CurrentCell = dgvPurchase.Rows[0].Cells[GetEnum(GridColIndexes.CItemCode)];
                    dgvPurchase.Focus();
                }
                else
                    txtTaxRegn.Focus();
            }

        }

        private void dgvPurchase_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {

        }

        private void btnprev_Click(object sender, EventArgs e)
        {
            if (txtInvAutoNo.Tag.ToString() == "0")
            {
                if (dgvPurchase.Rows.Count > 0)
                {
                    if (dgvPurchase.Rows[0].Cells[GetEnum(GridColIndexes.CItemCode)].Value != null)
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
                    if (dtpEffective.Enabled == true)
                        dtpEffective.Focus();
                    else if (txtReferencePrefix.Visible == true)
                        txtReferencePrefix.Focus();
                    else
                        txtReferenceAutoNo.Focus();
                    txtReferenceAutoNo.SelectAll();
                }
                else
                {
                    if (txtReferencePrefix.Visible == true)
                        txtReferencePrefix.Focus();
                    else
                    {
                        cboPayment.Focus();
                        SendKeys.Send("{F4}");
                    }
                }
            }
        }

        private void txtAddress1_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtAddress1);
        }

        private void txtAddress1_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtAddress1, true);
        }

        private void txtNarration_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtNarration);
        }

        private void txtNarration_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtNarration, true);
        }

        private void cboTaxMode_SelectedValueChanged(object sender, EventArgs e)
        {
            TaxCalculate();
            CalcTotal();
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            frmVouchertype frmV = new frmVouchertype(vchtypeID, false, true);
            frmV.StartPosition = FormStartPosition.CenterScreen;
            frmV.ShowDialog();
        }

        private void dgvPurchase_KeyUp(object sender, KeyEventArgs e)
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
                if (dgvPurchase.Rows.Count > 0)
                {
                    if (dgvPurchase.Rows[0].Cells[GetEnum(GridColIndexes.CItemCode)].Value != null)
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

        private void frmStockInVoucherNew_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                if (e.KeyCode == Keys.F3)
                {

                }
                else if (e.KeyCode == Keys.F12)
                {
                    if (txtSupplier.Enabled && txtSupplier.Visible)
                    {
                        if (Comm.ConvertI32(clsVchType.blnShowSearchWindowByDefault) == 1)
                        {
                            string sQuery = "SELECT LName+LAliasName+MobileNo+Address as AnyWhere,LALiasname as [Ledger Code],lname as [Ledger Name] ,MobileNo ,Address,LID,Email  FROM tblLedger L";
                            if (clsVchType.CustomerSupplierAccGroupList != "")
                                sQuery = sQuery + " INNER JOIN tblAccountGroup A ON A.AccountGroupID = L.AccountGroupID AND A.AccountGroupID IN (" + clsVchType.CustomerSupplierAccGroupList + ")";
                            else
                                sQuery = sQuery + " INNER JOIN tblAccountGroup A ON A.AccountGroupID = L.AccountGroupID AND (A.AccountGroupID IN (10,11) OR A.ParentID IN (10,11)) ";

                            sQuery = sQuery + " WHERE L.ActiveStatus=1 AND L.TenantID=" + Global.gblTenantID + "";

                            new frmCompactSearch(GetFromSupplierSearch, sQuery, "AnyWhere|LAliasName|LName|MobileNo|Address", txtSupplier.Location.X + 800, txtSupplier.Location.Y - 20, 4, 0, txtSupplier.Text, 4, 0, "ORDER BY L.LAliasName ASC", 0, 0, "Supplier Search ...", 0, "100,200,100,200,0", true, "frmSupplier").ShowDialog();

                            dgvPurchase.CurrentCell = dgvPurchase.Rows[0].Cells[GetEnum(GridColIndexes.CItemCode)];
                            dgvPurchase.Focus();
                        }
                    }
                }
                else if (e.KeyCode == Keys.F9)
                {
                    for (int i = 0; i <= dgvPurchase.Rows.Count - 1; i++)
                    {
                        if (dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.CItemName)].Tag == null) dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.CItemName)].Tag = "0";
                        if (Comm.ToDecimal(dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.CItemName)].Tag.ToString()) == 0)
                        {
                            dgvPurchase.CurrentCell = dgvPurchase[1, i];
                            dgvPurchase.Focus();
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
                        if (dgvPurchase.Rows.Count > 0)
                        {
                            if (dgvPurchase.Rows[0].Cells[GetEnum(GridColIndexes.CItemCode)].Value != null)
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

        private void dgvPurchase_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
        }

        private void tableLayoutPanel2_MouseUp(object sender, MouseEventArgs e)
        {
            dragging = false;
        }

        private void txtCashDisc_KeyDown_1(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
            {
                txtcashDisper.Focus();
                txtcashDisper.SelectAll();
            }
            else if (e.KeyCode == Keys.Enter)
            {
                txtRoundOff.Focus();
                txtRoundOff.SelectAll();
            }
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
                else if (dtpEffective.Enabled == true)
                    dtpEffective.Focus();
                else
                    dtpInvDate.Focus();

            }
            else if (e.KeyCode == Keys.Enter)
            {
                if (cboPayment.Enabled == true)
                {
                    cboPayment.Focus();
                    SendKeys.Send("{F4}");
                }
                else
                {
                    if (cboTaxMode.Enabled == true)
                    {
                        cboTaxMode.Focus();
                        SendKeys.Send("{F4}");
                    }
                    else
                    {
                        if (cboCostCentre.Enabled == true)
                        {
                            cboCostCentre.Focus();
                            SendKeys.Send("{F4}");
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
                                if (cboAgent.Enabled == true)
                                {
                                    cboAgent.Focus();
                                    SendKeys.Send("{F4}");
                                }
                                else
                                    txtSupplier.Focus();
                            }
                        }
                    }
                }
            }

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
                    if (txtReferenceAutoNo.ReadOnly == false)
                    {
                        if (txtReferencePrefix.Visible == true)
                            txtReferencePrefix.Focus();
                        else
                            txtReferenceAutoNo.Focus();
                    }
                    else
                    {
                        if (cboPayment.Enabled == true)
                        {
                            cboPayment.Focus();
                            SendKeys.Send("{F4}");
                        }
                        else
                        {
                            if (cboTaxMode.Enabled == true)
                            {
                                cboTaxMode.Focus();
                                SendKeys.Send("{F4}");
                            }
                            else
                            {
                                if (cboCostCentre.Enabled == true)
                                {
                                    cboCostCentre.Focus();
                                    SendKeys.Send("{F4}");
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
                                        if (cboAgent.Enabled == true)
                                        {
                                            cboAgent.Focus();
                                            SendKeys.Send("{F4}");
                                        }
                                        else
                                            txtSupplier.Focus();
                                    }
                                }
                            }
                        }
                    }
                    txtReferenceAutoNo.SelectAll();
                    //SendKeys.Send("{F4}");
                }
                else
                {
                    if (cboPayment.Enabled == true)
                    {
                        cboPayment.Focus();
                        SendKeys.Send("{F4}");
                    }
                    else
                    {
                        if (cboTaxMode.Enabled == true)
                        {
                            cboTaxMode.Focus();
                            SendKeys.Send("{F4}");
                        }
                        else
                        {
                            if (cboCostCentre.Enabled == true)
                            {
                                cboCostCentre.Focus();
                                SendKeys.Send("{F4}");
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
                                    if (cboAgent.Enabled == true)
                                    {
                                        cboAgent.Focus();
                                        SendKeys.Send("{F4}");
                                    }
                                    else
                                        txtSupplier.Focus();
                                }
                            }
                        }
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

        private void txtcashDisper_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (txtcashDisper.Text.Trim() != ".")
                {
                    if (txtcashDisper.Text == "") { txtcashDisper.Text = "0"; txtcashDisper.SelectAll(); }
                    if (Comm.ToDouble(txtcashDisper.Text) >= 0)
                    {
                        this.txtCashDisc.TextChanged -= this.txtCashDisc_TextChanged;
                        txtCashDisc.Text = Comm.FormatValue((Comm.ToDouble(txtNetAmt.Text) * Comm.ToDouble(txtcashDisper.Text) / 100));
                        this.txtCashDisc.TextChanged += this.txtCashDisc_TextChanged;
                    }
                }
                CalcTotal();
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void txtSupplier_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtSupplier, true);
            txtSupplier.SelectAll();
        }

        private void txtSupplier_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtSupplier);
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
                    DataTable dtInv = Comm.fnGetData("SELECT InvID, ISNULL(JsonData,'') as JsonData,Invid FROM tblPurchase WHERE InvNo = '" + txtInvAutoNo.Text.Replace("'","''") + "' AND VchTypeID=" + vchtypeID + " AND TenantID = " + Global.gblTenantID + "").Tables[0];
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

        private void cboAgent_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.ActiveControl != null)
                    if (this.ActiveControl.Name == cboAgent.Name.ToString())
                        GetAgentDiscountAsperVoucherType();
            }
            catch
            {

            }
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

        private void cboState_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (Comm.ToInt32(AppSettings.StateCode) != Comm.ToInt32(cboState.SelectedValue))
            {
                dgvPurchase.Columns[GetEnum(GridColIndexes.cCGST)].Visible = false;
                dgvPurchase.Columns[GetEnum(GridColIndexes.cSGST)].Visible = false;
            }
            else
            {
                dgvPurchase.Columns[GetEnum(GridColIndexes.cCGST)].Visible = true;
                dgvPurchase.Columns[GetEnum(GridColIndexes.cSGST)].Visible = true;
            }
            CalcTotal();
        }

        private void dgvPurchase_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {

        }

        private void btnNewIcon_Click(object sender, EventArgs e)
        {
            try
            {
                //if (this.ActiveControl.Name == "txtSupplier")
                //{
                this.ActiveControl.Name = btnNewIcon.Name;
                frmLedger frmLed = new frmLedger(0, true, 0, "SUPPLIER", txtSupplier);
                frmLed.Show();
                //}
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnEditIcon_Click(object sender, EventArgs e)
        {
            try
            {
                //if (this.ActiveControl.Name == "txtSupplier")
                //{
                if (txtSupplier.Text != "")
                {
                    if (lblLID.Text == "") lblLID.Text = "0";
                    frmLedger frmLed = new frmLedger(Comm.ToInt32(lblLID.Text), true, 0, "SUPPLIER", txtSupplier);
                    frmLed.Show();
                }
                //}
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
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

        private void txtSupplier_Click(object sender, EventArgs e)
        {
            txtSupplier.SelectAll();
        }

        private void txtDiscPerc_Leave(object sender, EventArgs e)
        {
            try
            {
                //if (Comm.ToDecimal(txtDiscAmt.Text) > 0)
                //    CalcTotal();
            }
            catch (Exception ex)
            {
            }
        }

        private void dtpInvDate_ValueChanged(object sender, EventArgs e)
        {
            dtpEffective.Value = dtpInvDate.Value;
        }

        private void btnMenu_Click(object sender, EventArgs e)
        {
            if (this.tlpMain.ColumnStyles[1].Width == 0)
                this.tlpMain.ColumnStyles[1].Width = 260;
            else
                this.tlpMain.ColumnStyles[1].Width = 0;
        }

        private void dgvPurchase_MouseUp(object sender, MouseEventArgs e)
        {
            //if (toggleWidthSettings.ToggleState == Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active)
            //{
            //    GridInitialize_dgvColWidth(false);
            //}
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
                        if (dgvPurchase.Columns[i].Name == dgvColWidth.Rows[i].Cells[3].Value.ToString())
                        {
                            dgvPurchase.Columns[i].Width = Comm.ToInt32(dgvColWidth.Rows[i].Cells[2].Value.ToString());
                            if (dgvColWidth.Rows[i].Cells[0].Value.ToString() == "")
                                dgvPurchase.Columns[i].Visible = false;
                            else
                                dgvPurchase.Columns[i].Visible = Convert.ToBoolean(dgvColWidth.Rows[i].Cells[0].Value);
                        }
                        if (dgvPurchase.Columns[i].Name == "cRateinclusive")
                            dgvPurchase.Columns[i].Visible = false;

                    }
                }
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void dgvPurchase_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
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
                        dtInv = Comm.fnGetData("SELECT TOP 1 ISNULL(InvId,0) FROM tblPurchase WHERE VchTypeID = " + vchtypeID + " AND TenantID = " + Global.gblTenantID + " ORDER BY InvId DESC").Tables[0];
                        if (dtInv.Rows.Count > 0)
                            dInvId = Comm.ToDecimal(dtInv.Rows[0][0].ToString());
                        else
                            dInvId = 0;
                    }
                    else
                    {
                        dtInv = Comm.fnGetData("SELECT TOP 1 ISNULL(InvId,0) FROM tblPurchase WHERE InvId < " + Comm.ToDecimal(txtInvAutoNo.Tag.ToString()) + " AND VchTypeID = " + vchtypeID + " AND TenantID = " + Global.gblTenantID + " ORDER BY InvId DESC").Tables[0];
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
                    }
                }
                else //Next
                {
                    if (txtInvAutoNo.Tag.ToString() != "0")
                    {
                        dtInv = Comm.fnGetData("SELECT TOP 1 ISNULL(InvId,0) FROM tblPurchase WHERE InvId > " + Comm.ToDecimal(txtInvAutoNo.Tag.ToString()) + " AND VchTypeID = " + vchtypeID + " AND TenantID = " + Global.gblTenantID + " ORDER BY InvId ASC").Tables[0];
                        if (dtInv.Rows.Count > 0)
                            dInvId = Comm.ToDecimal(dtInv.Rows[0][0].ToString());
                        else
                        {
                            dInvId = 0;
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

                            if (Comm.ConvertI32(clsVchType.TransactionNumberingValue) == 0) // Auto Locked
                            {
                                //txtInvAutoNo.Text = Comm.gfnGetNextSerialNo("tblPurchase", "AutoNum").ToString();
                                txtInvAutoNo.Text = Comm.gfnGetNextSerialNo("tblPurchase", "AutoNum", " VchTypeID=" + vchtypeID).ToString();
                                txtInvAutoNo.Tag = 0;

                                txtInvAutoNo.ReadOnly = true;
                                txtPrefix.ReadOnly = true;
                            }
                            else if (Comm.ConvertI32(clsVchType.TransactionNumberingValue) == 1) // Auto Editable
                            {
                                //txtInvAutoNo.Text = Comm.gfnGetNextSerialNo("tblPurchase", "AutoNum").ToString();
                                txtInvAutoNo.Text = Comm.gfnGetNextSerialNo("tblPurchase", "AutoNum", " VchTypeID=" + vchtypeID).ToString();
                                txtInvAutoNo.Tag = 0;

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
                                    txtReferenceAutoNo.Text = Comm.gfnGetNextSerialNo("tblPurchase", "ReferenceAutoNO").ToString();
                                txtReferenceAutoNo.ReadOnly = true;
                                txtReferencePrefix.ReadOnly = true;
                                txtReferencePrefix.Width = 55;
                            }
                            else if (Comm.ConvertI32(clsVchType.ReferenceNumberingValue) == 1) // Auto Editable
                            {
                                if (iIDFromEditWindow == 0)
                                    txtReferenceAutoNo.Text = Comm.gfnGetNextSerialNo("tblPurchase", "ReferenceAutoNO").ToString();
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
                sArrLedger[GetEnumLedger(LedgerIndexes.LName)] = txtSupplier.Text;
                sArrLedger[GetEnumLedger(LedgerIndexes.LAliasName)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["LedgerCode"].ToString());
                sArrLedger[GetEnumLedger(LedgerIndexes.Address)] = txtAddress1.Text;
                sArrLedger[GetEnumLedger(LedgerIndexes.TaxNo)] = txtTaxRegn.Text;
                sArrLedger[GetEnumLedger(LedgerIndexes.MobileNo)] = txtMobile.Text;

                if (cboState.SelectedValue == null)
                    FillStates(Comm.ToInt32(dtSupp.Rows[0]["StateID"].ToString()));

                if (cboBType.SelectedItem == null)
                {
                    if (cboBType.Items.Count >= 2)
                        cboBType.SelectedIndex = 1;
                }
                if (cboBType.SelectedItem != null)
                {
                    sArrLedger[GetEnumLedger(LedgerIndexes.StateID)] = cboState.SelectedValue.ToString();
                    sArrLedger[GetEnumLedger(LedgerIndexes.GSTType)] = cboBType.SelectedItem.ToString();
                }
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
                sArrLedger[GetEnumLedger(LedgerIndexes.AgentID)] = cboAgent.SelectedValue.ToString();

                GetSupplierDiscountAsperVoucherType();
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
                //if (txtSupplier.Text == "")
                //{
                //// dipoos
                this.txtSupplier.TextChanged -= this.txtSupplier_TextChanged;
                txtSupplier.Text = dtSupp.Rows[0]["LedgerName"].ToString();
                this.txtSupplier.TextChanged += this.txtSupplier_TextChanged;
                lblLID.Text = dtSupp.Rows[0]["LID"].ToString();
                txtAddress1.Text = dtSupp.Rows[0]["Address"].ToString();
                txtMobile.Text = dtSupp.Rows[0]["MobileNo"].ToString();
                txtTaxRegn.Text = dtSupp.Rows[0]["TaxNo"].ToString();
                FillStates(Comm.ToInt32(dtSupp.Rows[0]["StateID"].ToString()));
                txtSupplier.Tag = dtSupp.Rows[0]["LedgerCode"].ToString();
                txtAddress1.Tag = dtSupp.Rows[0]["Email"].ToString();
                dSupplierID = Comm.ToDecimal(dtSupp.Rows[0]["LID"].ToString());
                cboBType.Text = dtSupp.Rows[0]["GSTType"].ToString();

                if (cboBType.SelectedIndex < 0)
                    cboBType.SelectedIndex = 1;


                sArrLedger[GetEnumLedger(LedgerIndexes.LName)] = dtSupp.Rows[0]["LedgerName"].ToString();
                sArrLedger[GetEnumLedger(LedgerIndexes.LAliasName)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["LedgerCode"].ToString());
                sArrLedger[GetEnumLedger(LedgerIndexes.Address)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["Address"].ToString());
                sArrLedger[GetEnumLedger(LedgerIndexes.TaxNo)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["TaxNo"].ToString());
                sArrLedger[GetEnumLedger(LedgerIndexes.MobileNo)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["MobileNo"].ToString());
                sArrLedger[GetEnumLedger(LedgerIndexes.StateID)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["StateID"].ToString());
                sArrLedger[GetEnumLedger(LedgerIndexes.GSTType)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["GSTType"].ToString());
                //}
                //else
                //{
                //    sArrLedger[GetEnumLedger(LedgerIndexes.LName)] = txtSupplier.Text;
                //    sArrLedger[GetEnumLedger(LedgerIndexes.LAliasName)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["LAliasName"].ToString());
                //    sArrLedger[GetEnumLedger(LedgerIndexes.Address)] = txtAddress1.Text;
                //    sArrLedger[GetEnumLedger(LedgerIndexes.TaxNo)] = txtTaxRegn.Text;
                //    sArrLedger[GetEnumLedger(LedgerIndexes.MobileNo)] = txtMobile.Text;
                //    sArrLedger[GetEnumLedger(LedgerIndexes.StateID)] = cboState.SelectedValue.ToString();
                //    sArrLedger[GetEnumLedger(LedgerIndexes.GSTType)] = cboBType.SelectedItem.ToString();
                //}

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
                sArrLedger[GetEnumLedger(LedgerIndexes.AgentID)] = cboAgent.SelectedValue.ToString();

                GetSupplierDiscountAsperVoucherType();
                return true;
            }
            else
                return false;
        }

        // Description : When Select from Supplier Compact Search
        private Boolean GetFromSupplierSearch(string LstIDandText)
        {
            string[] sCompSearchData = LstIDandText.Split('|');
            DataTable dtSupp = new DataTable();
            if (sCompSearchData[0].ToString().ToUpper() == "NOTEXIST")
            {
                this.txtSupplier.TextChanged -= this.txtSupplier_TextChanged;
                txtSupplier.Text = sCompSearchData[1].ToString();
                this.txtSupplier.TextChanged += this.txtSupplier_TextChanged;
                lblLID.Text = "0";
                dSupplierID = 0;
                return true;
            }
            else
            {
                if (sCompSearchData.Length > 0)
                {
                    if (Comm.ToInt32(sCompSearchData[0].ToString()) != 0)
                    {
                        return FillSupplierUsingID(Comm.ToInt32(sCompSearchData[0].ToString()));
                    }
                    else
                    {
                        this.txtSupplier.TextChanged -= this.txtSupplier_TextChanged;
                        txtSupplier.Text = sCompSearchData[1].ToString();
                        this.txtSupplier.TextChanged += this.txtSupplier_TextChanged;
                        return true;
                    }
                }
                else
                    return false;
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
                GetStockInfo.ItemID = Comm.ToDouble(dgvPurchase.Rows[dgvPurchase.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cItemID)].Value);
                GetStockInfo.CCID = Comm.ToDouble(cboCostCentre.SelectedValue);
                dtData = clsStock.GetStockDetails(GetStockInfo);
                if (dtData.Rows.Count > 0)
                {
                    SetValue(GetEnum(GridColIndexes.CExpiry), dtData.Rows[0]["ExpiryDate"].ToString(), "DATE");
                    SetValue(GetEnum(GridColIndexes.cMRP), dtData.Rows[0]["MRP"].ToString(), "CURR_FLOAT");
                    SetValue(GetEnum(GridColIndexes.cPrate), dtData.Rows[0]["PRateExcl"].ToString(), "CURR_FLOAT");
                    SetValue(GetEnum(GridColIndexes.cGrossAmt), dtData.Rows[0]["PRateExcl"].ToString(), "CURR_FLOAT");
                    SetValue(GetEnum(GridColIndexes.cItemID), dtData.Rows[0]["ItemID"].ToString(), "INT");
                    dQty = Comm.ToDecimal(dgvPurchase.Rows[dgvPurchase.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cQty)].Value);
                    CalcTotal();
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
                GetStockInfo.ItemID = Comm.ToDouble(dgvPurchase.Rows[dgvPurchase.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cItemID)].Value);
                GetStockInfo.CCID = Comm.ToDouble(cboCostCentre.SelectedValue);
                dtData = clsStock.GetStockDetails(GetStockInfo);
                if (dtData.Rows.Count > 0)
                {
                    setTag(GetEnum(GridColIndexes.cBarCode), dtData.Rows[0]["BatchUnique"].ToString());
                    SetValue(GetEnum(GridColIndexes.cBarCode), dtData.Rows[0]["BatchCode"].ToString());
                    SetValue(GetEnum(GridColIndexes.CExpiry), dtData.Rows[0]["ExpiryDate"].ToString(), "DATE");
                    SetValue(GetEnum(GridColIndexes.cMRP), dtData.Rows[0]["MRP"].ToString(), "CURR_FLOAT");
                    SetValue(GetEnum(GridColIndexes.cQty), 1.ToString());
                    //SetValue(GetEnum(GridColIndexes.cPrate), dtData.Rows[0]["PRateExcl"].ToString(), "CURR_FLOAT");
                    //SetValue(GetEnum(GridColIndexes.cGrossAmt), dtData.Rows[0]["PRateExcl"].ToString(), "CURR_FLOAT");
                    SetValue(GetEnum(GridColIndexes.cPrate), dtData.Rows[0]["PRate"].ToString(), "CURR_FLOAT");
                    SetValue(GetEnum(GridColIndexes.cGrossAmt), dtData.Rows[0]["PrateInc"].ToString(), "CURR_FLOAT");
                    SetValue(GetEnum(GridColIndexes.cItemID), dtData.Rows[0]["ItemID"].ToString(), "INT");
                    dQty = Comm.ToDecimal(dgvPurchase.Rows[dgvPurchase.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cQty)].Value);
                    CalcTotal();
                }
            }
            else if (iStockID == 0)
            {
                sBarUnique = "<Auto Barcode>";
                SetValue(GetEnum(GridColIndexes.cBarCode), sBarUnique);
            }
        }

        //Description: Get Tax Mode details from Database
        public DataTable GetTaxMode(int iselID = 0)
        {
            GetTaxMinfo.TaxModeID = iselID;
            GetTaxMinfo.TenantID = Global.gblTenantID;
            return clsTax.GetTaxMode(GetTaxMinfo);
        }

        //Description: Fill Tax mode to the Combobox from GetTaxMode Method
        private void FillTaxMode(int iSelID = 0)
        {
            DataTable dtTax = new DataTable();

            //if (AppSettings.TaxMode > 1)
            //{
            //    dtTax = GetTaxMode(AppSettings.TaxMode + 1);
            //}
            //else
            //{
            dtTax = GetTaxMode(0);
            //}

            //dtTax = GetTaxMode(0);
            if (dtTax.Rows.Count > 0)
            {
                cboTaxMode.DataSource = dtTax;
                cboTaxMode.DisplayMember = "Tax Mode";
                cboTaxMode.ValueMember = "TaxModeID";

                if (iSelID != 0)
                    cboTaxMode.SelectedValue = iSelID;
            }
        }

        //Description: Get Agent Details from Database
        public DataTable GetAgent(int iSelID = 0)
        {
            GetAgentinfo.AgentID = iSelID;
            GetAgentinfo.TenantID = Global.gblTenantID;
            return clsAgent.GetAgentMaster(GetAgentinfo);
        }

        //Description: Fill Agent Details from GetAgent Method
        private void FillAgent(int iSelID = 0)
        {
            DataTable dtAgent = new DataTable();
            dtAgent = GetAgent(0);
            if (dtAgent.Rows.Count > 0)
            {
                cboAgent.DataSource = dtAgent;
                cboAgent.DisplayMember = "Agent Name";
                cboAgent.ValueMember = "AgentID";

                cboAgent.SelectedValue = 1;
                GetAgentDiscountAsperVoucherType();
            }
        }

        //Description : Get Ledger from Database and Fetching Only Supplier Details
        public DataTable GetLedger(decimal dSelNo = 0)
        {
            GetLedinfo.LID = dSelNo;
            GetLedinfo.GroupName = txtSupplier.Text;
            GetLedinfo.TenantID = Global.gblTenantID;
            return clsLedg.GetLedger(GetLedinfo);
        }

        //Description : Filling States using Query from Database
        private void FillStates(int iSelID = 0)
        {
            DataTable dtState = new DataTable();
            dtState = Comm.fnGetData("SELECT StateCode,State,StateId FROM tblStates WHERE TenantID =" + Global.gblTenantID + "").Tables[0];
            if (dtState.Rows.Count > 0)
            {
                cboState.DataSource = dtState;
                cboState.DisplayMember = "State";
                cboState.ValueMember = "StateId";
                if (iSelID != 0)
                {
                    cboState.SelectedValue = iSelID;
                    foreach (System.Data.DataRow row in dtState.Rows)
                    {
                        if (Comm.ToInt32(row["StateId"].ToString()) == iSelID)
                        {
                            lblStateCode.Text = row["StateCode"].ToString();
                        }
                    }
                }
            }
        }

        //Description : Seting Value to the Given Column, Row Indexes to Grid Cell
        private void SetValue(int iCellIndex, string sValue, string sConvertType = "")
        {
            if (dgvPurchase.Rows.Count > 0)
            {
                if (sConvertType.ToUpper() == "CURR_FLOAT")
                {
                    if (sValue == "") sValue = "0";
                    //Commented and Added By Dipu on 17-Feb-2022 ------------- >>
                    dgvPurchase.Rows[dgvPurchase.CurrentRow.Index].Cells[iCellIndex].Value = Comm.chkChangeValuetoZero(Comm.FormatValue(Comm.ToDouble(sValue)));
                    //Commented and Added By Dipu on 17-Feb-2022 ------------- >>

                    this.dgvPurchase.Columns[dgvPurchase.CurrentCell.ColumnIndex].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
                else if (sConvertType.ToUpper() == "QTY_FLOAT")
                {
                    if (sValue == "") sValue = "0";
                    dgvPurchase.Rows[dgvPurchase.CurrentRow.Index].Cells[iCellIndex].Value = Comm.chkChangeValuetoZero(Comm.FormatValue(Comm.ToDouble(sValue), false));
                    this.dgvPurchase.Columns[dgvPurchase.CurrentCell.ColumnIndex].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
                else if (sConvertType.ToUpper() == "PERC_FLOAT")
                {
                    if (sValue == "") sValue = "0";
                    dgvPurchase.Rows[dgvPurchase.CurrentRow.Index].Cells[iCellIndex].Value = Comm.chkChangeValuetoZero(Comm.ToDecimal(sValue).ToString("#.00"));
                    this.dgvPurchase.Columns[dgvPurchase.CurrentCell.ColumnIndex].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
                else if (sConvertType.ToUpper() == "DATE")
                {
                    dgvPurchase.Rows[dgvPurchase.CurrentRow.Index].Cells[iCellIndex].Value = Convert.ToDateTime(sValue).ToString("dd/MMM/yyyy");
                }
                else
                    dgvPurchase.Rows[dgvPurchase.CurrentRow.Index].Cells[iCellIndex].Value = Comm.chkChangeValuetoZero(sValue);
            }
        }

        //Description : Setting Value to Tag of the cells of Grid using the Given Column and Row Indexes
        private void setTag(int iCellIndex, string sTag, string sConvertType = "")
        {
            if (sConvertType.ToUpper() == "FLOAT")
            {
                if (sTag == "") sTag = "0";
                dgvPurchase.Rows[dgvPurchase.CurrentRow.Index].Cells[iCellIndex].Tag = Comm.ToDecimal(sTag).ToString("#.00");
            }
            else if (sConvertType.ToUpper() == "DATE")
            {
                dgvPurchase.Rows[dgvPurchase.CurrentRow.Index].Cells[iCellIndex].Tag = Convert.ToDateTime(sTag).ToString("dd/MMM/yyyy");
            }
            else
                dgvPurchase.Rows[dgvPurchase.CurrentRow.Index].Cells[iCellIndex].Tag = sTag;
        }

        //Description : Function Polymorphism for Seting the Value to Grid Asper Parameters Given
        private void SetValue(int iCellIndex, int iRowIndex, string sValue, string sConvertType = "")
        {
            //if(sConvertType.ToUpper() == "QTY")
            //    dgvPurchase.Rows[iRowIndex].Cells[iCellIndex].Value = Comm.chkChangeValuetoZero(Comm.FormatValue(Comm.ToDouble(sValue),false));
            //else
            dgvPurchase.Rows[iRowIndex].Cells[iCellIndex].Value = Comm.chkChangeValuetoZero(sValue);
        }

        //Description : Check the conditions of Supplier While Entered or Non Entred
        private bool CheckIsValidSupplier()
        {
            DataTable dtSupp = new DataTable();
            bool bResult = true;
            if (lblLID.Text == "") lblLID.Text = "0";
            if (txtSupplier.Text == "")
            {
                dtSupp = Comm.fnGetData("SELECT * FROM tblLedger WHERE LID = 100").Tables[0];
                if (dtSupp.Rows.Count > 0)
                {
                    this.txtSupplier.TextChanged -= this.txtSupplier_TextChanged;
                    txtSupplier.Text = dtSupp.Rows[0]["LName"].ToString();
                    this.txtSupplier.TextChanged += this.txtSupplier_TextChanged;

                    txtMobile.Text = dtSupp.Rows[0]["MobileNo"].ToString();
                    txtTaxRegn.Text = dtSupp.Rows[0]["TaxNo"].ToString();
                    cboState.SelectedValue = Comm.ToDecimal(dtSupp.Rows[0]["StateID"].ToString());
                    cboBType.Text = dtSupp.Rows[0]["GSTType"].ToString();
                    txtAddress1.Text = dtSupp.Rows[0]["Address"].ToString();
                    lblLID.Text = dtSupp.Rows[0]["LID"].ToString();
                    bResult = true;
                }
                else
                    bResult = false;
            }
            else if (Comm.ToInt32(lblLID.Text) == 0 && txtSupplier.Text != "")
            {
                if (cboPayment.SelectedIndex == 1) // Credit
                {
                    MessageBox.Show("Hi! You are Selected a Credit Bill for Onetime Supplier. This transaction is not allowed.");
                    bResult = false;
                }
                else
                    bResult = true;
            }
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

            //if (txtInvAutoNo.Text == "") txtInvAutoNo.Text = "0";
            if (txtInvAutoNo.Text == "")
            {
                bValidate = false;
                MessageBox.Show("Please Enter the Invoice No.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                txtInvAutoNo.Focus();
                goto FailsHere;
            }
            else if (Convert.ToString(dgvPurchase.Rows[0].Cells[GetEnum(GridColIndexes.CItemCode)].Value) == "")
            {
                bValidate = false;
                MessageBox.Show("No Items are Entered for Save. Please Enter the Item", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                goto FailsHere;
            }
            else if (sMsg[0].ToString() != "")
            {
                bValidate = false;
                MessageBox.Show("Sales Rates are Lesser Than of PRate of the Item[" + dgvPurchase.Rows[Comm.ToInt32(sMsg[1])].Cells[GetEnum(GridColIndexes.CItemName)].Value.ToString() + "], Check the Values [" + sMsg[0].ToString() + "].", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                goto FailsHere;
            }
            else if (txtTaxRegn.Text != "")
            {
                if (cboBType.SelectedIndex == 1)
                {
                    bValidate = false;
                    MessageBox.Show("The Bill Type Should be B2B When a Supplier have Tax Registration No.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    cboBType.Focus();
                    goto FailsHere;
                }
            }
            else
            {
                //if(Comm.ToDecimal(dgvPurchase.Rows[0].Cells[GetEnum(GridColIndexes.cPrate)].Value) == 0)
                for (int i = 0; i < dgvPurchase.Rows.Count; i++)
                {
                    if (dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.CItemCode)].Value != null)
                    {
                        bValidate = true;

                        string sQuery = "Select ItemID,BatchCode From tblStock Where LTRIM(RTRIM(BatchCode)) = '" + dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cBarCode)].Value.ToString() + "' and ItemID <> " + dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.CItemName)].Tag.ToString() + " ";
                        DataTable dtBatch = Comm.fnGetData(sQuery).Tables[0];
                        if (dtBatch.Rows.Count > 0)
                        {
                            MessageBox.Show("This BatchCode " + dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cBarCode)].Value.ToString() + " is already exist for another item.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            bValidate = false;
                            goto FailsHere;
                        }

                        if (Comm.ToDecimal(dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cPrate)].Value) == 0)
                        {
                            MessageBox.Show("Purchase rate cannot be zero. Please provide purchase rate for the item !", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            bValidate = false;
                            goto FailsHere;
                        }
                        if (Comm.ToDecimal(dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cQty)].Value) == 0 && Comm.ToDecimal(dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cFree)].Value) == 0)
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
                    dtInv = Comm.fnGetData("SELECT InvNo FROM tblPurchase WHERE vchtypeid = " + vchtypeID + " and LTRIM(RTRIM(InvNo)) = '" + txtInvAutoNo.Text.Trim() + "'").Tables[0];
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
                    if (txtSupplier.Enabled && txtSupplier.Visible) txtSupplier.Focus();
                    goto FailsHere;
                }
            }
            //if (Comm.ToInt32(cboPayment.SelectedIndex) == 1 || Comm.ToInt32(cboPayment.SelectedIndex) == 2)
            if (Comm.ToInt32(cboPayment.SelectedIndex) == 1)
            {
                if (txtSupplier.Text == "" || dSupplierID == 0)
                {
                    bValidate = false;
                    MessageBox.Show("Please Choose Party for Credit Purchase.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    txtSupplier.Focus();
                    goto FailsHere;
                }
            }
            else
            {
                for (int i = 0; i < dgvPurchase.Rows.Count; i++)
                {
                    if (iIDFromEditWindow == 0)
                    {
                        if (dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cBarCode)].Value != null)
                        {
                            if (dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cBatchMode)].Value.ToString().Trim() != "2")
                            {
                                string sQuery = "Select ItemID,BatchCode From tblStock Where LTRIM(RTRIM(BatchCode)) = '" + dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cBarCode)].Value.ToString() + "' AND ItemID <> " + Comm.ToDouble(dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cItemID)].Value.ToString()) + "";
                                DataTable dtBatch = Comm.fnGetData(sQuery).Tables[0];
                                if (dtBatch.Rows.Count > 0)
                                {
                                    bValidate = false;
                                    MessageBox.Show("This BatchCode " + dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cBarCode)].Value.ToString() + "of Item [" + dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.CItemName)].Value.ToString() + "] is already Exist.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    break;
                                }
                            }
                        }
                    }
                    //Dipu on 19-May-2022 -------------------- >> Do Not Allow Net Amount is Greater than of CRate and CRate With Tax
                    if (Comm.ToDouble(dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cCrate)].Value) > 0)
                    {
                        bValidate = true;
                        //if (Comm.ToDouble(dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cCrate)].Value) > Comm.ToDouble(dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cNetAmount)].Value))
                        //if (Math.Abs((Convert.ToDouble(dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cCrate)].Value) - (Convert.ToDouble(dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cNetAmount)].Value) / Convert.ToDouble(dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cQty)].Value)))) > 0.01)
                        //{
                        //    bValidate = false;
                        //    MessageBox.Show("Do not allow the Net Amount is Greater than of CRate or CRate With Tax. Check the Other Charges or Cost Factor are Correct !", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        //    goto FailsHere;
                        //    //break;
                        //}
                        //else if (Comm.ToDouble(dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cCRateWithTax)].Value) > Comm.ToDouble(dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cNetAmount)].Value))

                        //if (Math.Abs((Math.Round(Convert.ToDouble(dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cCRateWithTax)].Value), AppSettings.CurrencyDecimals) - Math.Round((Convert.ToDouble(dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cNetAmount)].Value) / Convert.ToDouble(dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cQty)].Value)), AppSettings.CurrencyDecimals))) > 0.01)
                        if (((Math.Round(Convert.ToDouble(dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cCRateWithTax)].Value), AppSettings.CurrencyDecimals) - Math.Round((Convert.ToDouble(dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cNetAmount)].Value) / Convert.ToDouble(dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cQty)].Value)), AppSettings.CurrencyDecimals))) < 0)
                        {
                            bValidate = false;
                            MessageBox.Show("Do not allow the Net Amount is Greater than of CRate With Tax. Check the Other Charges or Cost Factor are Correct !", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            goto FailsHere;
                            //break;
                        }
                        else if (Convert.ToDateTime(dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.CExpiry)].Value) <= Convert.ToDateTime(DateTime.Today))
                        {
                            bValidate = false;
                            MessageBox.Show("Do Not Allow the Previous Expiry Date !", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            goto FailsHere;
                            //break;
                        }
                    }
                }

                for (int j = 0; j < dgvPurchase.Rows.Count; j++)
                {
                    bValidate = true;
                    if (Comm.ToDecimal(dgvPurchase.Rows[j].Cells[GetEnum(GridColIndexes.cPrate)].Value) > Comm.ToDecimal(dgvPurchase.Rows[j].Cells[GetEnum(GridColIndexes.cMRP)].Value))
                    {
                        bValidate = false;
                        MessageBox.Show("Purchase Rate Above MRP.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        goto FailsHere;
                        //break;
                    }
                    else if (Comm.ToDecimal(dgvPurchase.Rows[j].Cells[GetEnum(GridColIndexes.cSRate1)].Value) > Comm.ToDecimal(dgvPurchase.Rows[j].Cells[GetEnum(GridColIndexes.cMRP)].Value))
                    {
                        bValidate = false;
                        MessageBox.Show("Sales Rate 1 Above MRP.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        goto FailsHere;
                        //break;
                    }
                    else if (Comm.ToDecimal(dgvPurchase.Rows[j].Cells[GetEnum(GridColIndexes.cSRate2)].Value) > Comm.ToDecimal(dgvPurchase.Rows[j].Cells[GetEnum(GridColIndexes.cMRP)].Value))
                    {
                        if (AppSettings.IsActiveSRate2 == true)
                        {
                            bValidate = false;
                            MessageBox.Show("Sales Rate 2 Above MRP.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            goto FailsHere;
                            //break;
                        }
                    }
                    else if (Comm.ToDecimal(dgvPurchase.Rows[j].Cells[GetEnum(GridColIndexes.cSRate3)].Value) > Comm.ToDecimal(dgvPurchase.Rows[j].Cells[GetEnum(GridColIndexes.cMRP)].Value))
                    {
                        if (AppSettings.IsActiveSRate3 == true)
                        {
                            bValidate = false;
                            MessageBox.Show("Sales Rate 3 Above MRP.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            goto FailsHere;
                            //break;
                        }
                    }
                    else if (Comm.ToDecimal(dgvPurchase.Rows[j].Cells[GetEnum(GridColIndexes.cSRate4)].Value) > Comm.ToDecimal(dgvPurchase.Rows[j].Cells[GetEnum(GridColIndexes.cMRP)].Value))
                    {
                        if (AppSettings.IsActiveSRate4 == true)
                        {
                            bValidate = false;
                            MessageBox.Show("Sales Rate 4 Above MRP.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            goto FailsHere;
                            //break;
                        }
                    }
                    else if (Comm.ToDecimal(dgvPurchase.Rows[j].Cells[GetEnum(GridColIndexes.cSRate5)].Value) > Comm.ToDecimal(dgvPurchase.Rows[j].Cells[GetEnum(GridColIndexes.cMRP)].Value))
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
            for (i = 0; i < dgvPurchase.Rows.Count; i++)
            {
                if (dgvPurchase.Rows[0].Cells[GetEnum(GridColIndexes.CItemCode)].Value != null)
                {
                    if (dgvPurchase.Rows[0].Cells[GetEnum(GridColIndexes.CItemCode)].Value.ToString() != "")
                    {
                        if (Comm.ToDecimal(dgvPurchase.Rows[0].Cells[GetEnum(GridColIndexes.cPrate)].Value) > Comm.ToDecimal(dgvPurchase.Rows[0].Cells[GetEnum(GridColIndexes.cSRate1)].Value))
                            sData = sData + AppSettings.SRate1Name + " ,";
                        else if (Comm.ToDecimal(dgvPurchase.Rows[0].Cells[GetEnum(GridColIndexes.cPrate)].Value) > Comm.ToDecimal(dgvPurchase.Rows[0].Cells[GetEnum(GridColIndexes.cSRate2)].Value))
                        {
                            if (AppSettings.IsActiveSRate2 == true)
                                sData = sData + AppSettings.SRate2Name + " ,";
                        }
                        else if (Comm.ToDecimal(dgvPurchase.Rows[0].Cells[GetEnum(GridColIndexes.cPrate)].Value) > Comm.ToDecimal(dgvPurchase.Rows[0].Cells[GetEnum(GridColIndexes.cSRate3)].Value))
                        {
                            if (AppSettings.IsActiveSRate3 == true)
                                sData = sData + AppSettings.SRate3Name + " ,";
                        }
                        else if (Comm.ToDecimal(dgvPurchase.Rows[0].Cells[GetEnum(GridColIndexes.cPrate)].Value) > Comm.ToDecimal(dgvPurchase.Rows[0].Cells[GetEnum(GridColIndexes.cSRate4)].Value))
                        {
                            if (AppSettings.IsActiveSRate4 == true)
                                sData = sData + AppSettings.SRate4Name + " ,";
                        }
                        else if (Comm.ToDecimal(dgvPurchase.Rows[0].Cells[GetEnum(GridColIndexes.cPrate)].Value) > Comm.ToDecimal(dgvPurchase.Rows[0].Cells[GetEnum(GridColIndexes.cSRate5)].Value))
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

        //Description : Serialize the Purchase table Fields asper instructions.
        private string SerializetoJson()
        {
            #region "Purchase Master (tblPurchase) ------------------------------- >>"

            if (txtcashDisper.Text == "") txtcashDisper.Text = "0";
            if (dSupplierID == 0)
            {
                if (Comm.ToInt32(cboPayment.SelectedIndex) == 0)
                {
                    DataTable dtDefaultSupp = Comm.fnGetData("select top 1 LID,LName,LAliasName,Address,MobileNo,AccountGroupID from tblLedger WHERE LID = 100 AND GroupName = 'SUPPLIER'").Tables[0];
                    if (dtDefaultSupp.Rows.Count > 0)
                    {
                        dSupplierID = Comm.ToDecimal(dtDefaultSupp.Rows[0]["LID"].ToString());
                        lblLID.Text = dSupplierID.ToString();
                        txtSupplier.Tag = dtDefaultSupp.Rows[0]["LAliasName"].ToString().Replace("'", "''");
                        cboBType.SelectedIndex = 1;
                        FillSupplierForSerializeJsonUsingID(Comm.ToInt32(lblLID.Text));
                    }
                    else
                    {
                        dSupplierID = 0;
                        lblLID.Text = "100";
                        txtSupplier.Tag = "";
                        cboBType.SelectedIndex = 1;
                        FillSupplierForSerializeJsonUsingID(100);
                    }
                }
                else
                    txtSupplier.Tag = txtSupplier.Text;
            }
            else if (dSupplierID == 100)
            {
                lblLID.Text = dSupplierID.ToString();
                cboBType.SelectedIndex = 1;
                FillSupplierForSerializeJsonUsingID(100);
            }
            if (iIDFromEditWindow == 0)
            {
                clsJPMinfo.InvId = Comm.gfnGetNextSerialNo("tblPurchase", "InvId");
                txtInvAutoNo.Tag = clsJPMinfo.InvId;
                clsJPMinfo.AutoNum = Comm.ToDecimal(Comm.gfnGetNextSerialNo("tblPurchase", "AutoNum", " VchTypeID=" + vchtypeID).ToString());
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
            clsJPMinfo.MOP = Convert.ToString(cboPayment.SelectedItem);
            clsJPMinfo.TaxModeID = Comm.ToDecimal(cboTaxMode.SelectedValue);
            clsJPMinfo.LedgerId = Comm.ToDecimal(lblLID.Text);
            clsJPMinfo.Party = txtSupplier.Text.Replace("'", "''");
            clsJPMinfo.Discount = Comm.ToDecimal(txtDiscAmt.Text);
            clsJPMinfo.TaxAmt = Comm.ToDecimal(txtTaxAmt.Text);
            clsJPMinfo.GrossAmt = Comm.ToDecimal(txtGrossAmt.Text);
            clsJPMinfo.QtyTotal = Comm.ToDecimal(lblQtyTotal.Text);
            clsJPMinfo.FreeTotal = Comm.ToDecimal(lblFreeTotal.Text);
            clsJPMinfo.BillAmt = Comm.ToDecimal(lblBillAmount.Text);
            clsJPMinfo.CoolieTotal = Comm.ToDecimal(txtCoolie.Text);

            clsJPMinfo.Cancelled = 0;
            clsJPMinfo.OtherExpense = Comm.ToDecimal(txtOtherExp.Text);
            clsJPMinfo.SalesManID = Comm.ToDecimal(cboSalesStaff.SelectedValue);
            clsJPMinfo.Taxable = Comm.ToDecimal(txtTaxable.Text);
            clsJPMinfo.NonTaxable = Comm.ToDecimal(txtNonTaxable.Text);
            clsJPMinfo.ItemDiscountTotal = Comm.ToDecimal(txtItemDiscTot.Text);
            clsJPMinfo.RoundOff = Comm.ToDecimal(txtRoundOff.Text);
            clsJPMinfo.UserNarration = txtNarration.Text;
            clsJPMinfo.SortNumber = 0;
            clsJPMinfo.DiscPer = Comm.ToDecimal(txtDiscPerc.Text);
            clsJPMinfo.VchTypeID = vchtypeID;
            clsJPMinfo.CCID = Comm.ToDecimal(cboCostCentre.SelectedValue);
            clsJPMinfo.CurrencyID = 0;
            clsJPMinfo.PartyAddress = txtAddress1.Text;
            clsJPMinfo.UserID = Global.gblUserID;
            clsJPMinfo.AgentID = Comm.ToDecimal(cboAgent.SelectedValue);
            clsJPMinfo.CashDiscount = Comm.ToDecimal(txtCashDisc.Text);
            clsJPMinfo.DPerType_ManualCalc_Customer = 0;
            clsJPMinfo.NetAmount = Comm.ToDecimal(txtNetAmt.Text);
            clsJPMinfo.RefNo = txtReferencePrefix.Text;
            clsJPMinfo.CashPaid = 0;
            clsJPMinfo.CardPaid = 0;
            clsJPMinfo.blnWaitforAuthorisation = 0;
            clsJPMinfo.UserIDAuth = 0;
            clsJPMinfo.BillTime = DateTime.Now;
            clsJPMinfo.StateID = Comm.ToDecimal(cboState.SelectedValue);
            clsJPMinfo.ImplementingStateCode = "";
            clsJPMinfo.GSTType = cboBType.SelectedText;
            clsJPMinfo.CGSTTotal = 0;
            clsJPMinfo.SGSTTotal = 0;
            clsJPMinfo.IGSTTotal = 0;
            clsJPMinfo.PartyGSTIN = txtTaxRegn.Text;
            clsJPMinfo.BillType = cboBType.Text;
            clsJPMinfo.blnHold = 0;
            clsJPMinfo.PriceListID = 0;
            clsJPMinfo.EffectiveDate = dtpEffective.Value;
            clsJPMinfo.partyCode = txtSupplier.Tag.ToString().Replace("'", "''");
            clsJPMinfo.MobileNo = txtMobile.Text.Replace("'", "''");
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
            clsJPMinfo.CashDisPer = Comm.ToDecimal(txtcashDisper.Text);
            clsJPMinfo.CostFactor = Comm.ToDecimal(txtCostFactor.Text);
            clsJPMinfo.TenantID = Global.gblTenantID;
            clsPM.clsJsonPMInfo_ = clsJPMinfo;

            #endregion

            #region "Supplier Data (tblLedger) ----------------------------------- >>"

            clsJPMLedgerinfo.LID = dSupplierID;
            clsJPMLedgerinfo.LName = txtSupplier.Text;
            clsJPMLedgerinfo.LAliasName = txtSupplier.Tag.ToString().Replace("'", "''");
            clsJPMLedgerinfo.GroupName = sArrLedger[GetEnumLedger(LedgerIndexes.GroupName)].ToString().Replace("'", "''");
            clsJPMLedgerinfo.Type = sArrLedger[GetEnumLedger(LedgerIndexes.Type)].ToString();
            clsJPMLedgerinfo.OpBalance = Comm.ToDecimal(sArrLedger[GetEnumLedger(LedgerIndexes.OpBalance)]);
            clsJPMLedgerinfo.AppearIn = sArrLedger[GetEnumLedger(LedgerIndexes.AppearIn)].ToString();
            clsJPMLedgerinfo.Address = txtAddress1.Text;
            clsJPMLedgerinfo.CreditDays = sArrLedger[GetEnumLedger(LedgerIndexes.CreditDays)].ToString();
            clsJPMLedgerinfo.Phone = sArrLedger[GetEnumLedger(LedgerIndexes.Phone)].ToString();
            clsJPMLedgerinfo.TaxNo = txtTaxRegn.Text;
            clsJPMLedgerinfo.AccountGroupID = Comm.ToDecimal(sArrLedger[GetEnumLedger(LedgerIndexes.AccountGroupID)].ToString());
            clsJPMLedgerinfo.RouteID = Comm.ToDecimal(sArrLedger[GetEnumLedger(LedgerIndexes.RouteID)].ToString());
            clsJPMLedgerinfo.Area = sArrLedger[GetEnumLedger(LedgerIndexes.Area)].ToString().Replace("'", "''");
            clsJPMLedgerinfo.Notes = sArrLedger[GetEnumLedger(LedgerIndexes.Notes)].ToString().Replace("'", "''");
            clsJPMLedgerinfo.TargetAmt = Comm.ToDecimal(sArrLedger[GetEnumLedger(LedgerIndexes.TargetAmt)].ToString());
            clsJPMLedgerinfo.SMSSchID = Comm.ToDecimal(sArrLedger[GetEnumLedger(LedgerIndexes.SMSSchID)].ToString());
            clsJPMLedgerinfo.Email = sArrLedger[GetEnumLedger(LedgerIndexes.Email)].ToString().Replace("'", "''");
            clsJPMLedgerinfo.MobileNo = txtMobile.Text;
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
            clsJPMLedgerinfo.StateID = Comm.ToDecimal(cboState.SelectedValue);
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
            //clsJPMLedgerinfo.SystemName = Global.gblSystemName;
            //clsJPMLedgerinfo.UserID = Global.gblUserID;
            //clsJPMLedgerinfo.LastUpdateDate = DateTime.Today;
            //clsJPMLedgerinfo.LastUpdateTime = DateTime.Now;
            clsJPMLedgerinfo.TenantID = Global.gblTenantID;
            clsJPMLedgerinfo.GSTType = Convert.ToString(cboBType.SelectedItem);
            clsJPMLedgerinfo.AgentID = Comm.ToDecimal(sArrLedger[GetEnumLedger(LedgerIndexes.AgentID)].ToString());
            clsPM.clsJsonPMLedgerInfo_ = clsJPMLedgerinfo;

            #endregion

            #region "TAX Mode (tblTaxMode) --------------------------------------- >>"

            string[] sArrTMod = GetTaxModeData(Comm.ToDecimal(cboTaxMode.SelectedValue));
            clsJPMTaxModinfo.TaxModeID = Comm.ToDecimal(cboTaxMode.SelectedValue);
            clsJPMTaxModinfo.TaxMode = cboTaxMode.SelectedItem.ToString();
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
            //clsJPMTaxModinfo.SystemName = Global.gblSystemName;
            //clsJPMTaxModinfo.UserID = Global.gblUserID;
            //clsJPMTaxModinfo.LastUpdateDate = DateTime.Today;
            //clsJPMTaxModinfo.LastUpdateTime = DateTime.Now;
            clsJPMTaxModinfo.TenantID = Global.gblTenantID;
            clsPM.clsJsonPMTaxmodeInfo_ = clsJPMTaxModinfo;

            #endregion

            #region "Agent Master (tblAgent) ------------------------------------- >>"

            string[] sArrAgent = GetAgentData(Comm.ToDecimal(cboAgent.SelectedValue));
            clsJPMAgentinfo.AgentID = Comm.ToDecimal(cboAgent.SelectedValue);
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
            //clsJPMAgentinfo.SystemName = Global.gblSystemName;
            //clsJPMAgentinfo.UserID = Global.gblUserID;
            //clsJPMAgentinfo.LastUpdateDate = DateTime.Today; ;
            //clsJPMAgentinfo.LastUpdateTime = DateTime.Now;
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

            string[] sArrState = GetStateData(Comm.ToDecimal(cboState.SelectedValue));
            clsJPMStateinfo.StateId = Comm.ToDecimal(cboState.SelectedValue);
            clsJPMStateinfo.StateCode = sArrState[0].ToString();
            clsJPMStateinfo.State = cboState.SelectedItem.ToString();
            clsJPMStateinfo.StateType = sArrState[1].ToString();
            clsJPMStateinfo.Country = sArrState[2].ToString();
            clsJPMStateinfo.CountryID = Comm.ToDecimal(sArrState[3].ToString());
            //Dipu 21-03-2022 ------- >>
            //clsJPMStateinfo.SystemName = Global.gblSystemName;
            //clsJPMStateinfo.UserID = Global.gblUserID;
            //clsJPMStateinfo.LastUpdateDate = DateTime.Today;
            //clsJPMStateinfo.LastUpdateTime = DateTime.Now;
            clsJPMStateinfo.TenantID = Global.gblTenantID;
            clsPM.clsJsonPMStateInfo_ = clsJPMStateinfo;

            #endregion

            #region "Employee Master (tblEmployee) ------------------------------- >>"

            string[] sArrEmp = GetEmpDetails(Comm.ToDecimal(cboSalesStaff.SelectedValue));
            clsJPMEmployeeinfo.EmpID = Comm.ToDecimal(cboSalesStaff.SelectedValue);
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
            //clsJPMEmployeeinfo.SystemName = Global.gblSystemName;
            //clsJPMEmployeeinfo.UserID = Global.gblUserID;
            //clsJPMEmployeeinfo.LastUpdateDate = DateTime.Today;
            //clsJPMEmployeeinfo.LastUpdateTime = DateTime.Now;
            clsJPMEmployeeinfo.TenantID = Global.gblTenantID;
            clsPM.clsJsonPMEmployeeInfo_ = clsJPMEmployeeinfo;

            #endregion

            #region "Purchase Details (tblPurchaseItem) -------------------------- >>"
            DataTable dtBatchUniq = new DataTable();
            List<clsJsonPDetailsInfo> lstJPDinfo = new List<clsJsonPDetailsInfo>();
            for (int i = 0; i < dgvPurchase.Rows.Count; i++)
            {
                if (dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.CItemCode)].Value != null)
                {
                    if (dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.CItemCode)].Value.ToString() != "")
                    {
                        clsJPDinfo = new clsJsonPDetailsInfo();

                        if (dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cBarCode)].Value.ToString().ToUpper() == "<AUTO BARCODE>")
                            dtBatchUniq = Comm.fnGetData("EXEC UspGetBatchCodeWhenAutoBarcode " + Comm.ToDecimal(dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cItemID)].Value) + ",'" + dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cBarCode)].Value.ToString() + "',''," + Comm.ToDecimal(dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cMRP)].Value) + ",'" + Convert.ToDateTime(dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.CExpiry)].Value).ToString("dd-MMM-yyyy") + "'," + Global.gblTenantID + "").Tables[0];

                        //clsJPDinfo.InvID = Comm.ToDecimal(txtInvAutoNo.Text);
                        clsJPDinfo.InvID = Comm.ToDecimal(txtInvAutoNo.Tag);
                        clsJPDinfo.ItemId = Comm.ToDecimal(dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cItemID)].Value);
                        clsJPDinfo.Qty = Comm.ToDecimal(dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cQty)].Value);
                        clsJPDinfo.Rate = Comm.ToDecimal(dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cCrate)].Value);
                        clsJPDinfo.UnitId = Comm.ToDecimal(dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.CUnit)].Tag);
                        clsJPDinfo.Batch = "";
                        clsJPDinfo.TaxPer = Comm.ToDecimal(dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.ctaxPer)].Value);
                        clsJPDinfo.TaxAmount = Comm.ToDecimal(dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.ctax)].Value);
                        clsJPDinfo.Discount = Comm.ToDecimal(dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cDiscAmount)].Value);
                        clsJPDinfo.MRP = Comm.ToDecimal(dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cMRP)].Value);
                        clsJPDinfo.SlNo = Comm.ToDecimal(dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cSlNo)].Value);
                        clsJPDinfo.Prate = Comm.ToDecimal(dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cPrate)].Value);
                        clsJPDinfo.Free = Comm.ToDecimal(dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cFree)].Value);
                        clsJPDinfo.SerialNos = "";
                        clsJPDinfo.ItemDiscount = Comm.ToDecimal(dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cDiscAmount)].Value);

                        if (dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cBarCode)].Value != null)
                        {
                            if (dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cBarCode)].Value.ToString() != "0")
                                clsJPDinfo.BatchCode = dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cBarCode)].Value.ToString();
                            else
                                clsJPDinfo.BatchCode = dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cBarCode)].Tag.ToString();
                        }
                        else
                            clsJPDinfo.BatchCode = dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cBarCode)].Tag.ToString();

                        clsJPDinfo.iCessOnTax = 0;
                        clsJPDinfo.blnCessOnTax = 0;
                        clsJPDinfo.Expiry = Convert.ToDateTime(dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.CExpiry)].Value);
                        clsJPDinfo.ItemDiscountPer = Comm.ToDecimal(dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cDiscPer)].Value);
                        clsJPDinfo.RateInclusive = Comm.ToDecimal(dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cRateinclusive)].Value);
                        clsJPDinfo.ITaxableAmount = Comm.ToDecimal(dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.ctaxable)].Value);
                        clsJPDinfo.InonTaxableAmount = Convert.ToDecimal(dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cNonTaxable)].Value);
                        clsJPDinfo.INetAmount = Comm.ToDecimal(dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cNetAmount)].Value);
                        clsJPDinfo.CGSTTaxPer = Comm.ToDecimal(dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cCGST)].Tag);
                        clsJPDinfo.CGSTTaxAmt = Comm.ToDecimal(dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cCGST)].Value);
                        clsJPDinfo.SGSTTaxPer = Comm.ToDecimal(dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cSGST)].Tag);
                        clsJPDinfo.SGSTTaxAmt = Comm.ToDecimal(dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cSGST)].Value);
                        clsJPDinfo.IGSTTaxPer = Comm.ToDecimal(dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cIGST)].Tag);
                        clsJPDinfo.IGSTTaxAmt = Comm.ToDecimal(dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cIGST)].Value);
                        clsJPDinfo.iRateDiscPer = Comm.ToDecimal(dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cDiscPer)].Value);
                        clsJPDinfo.iRateDiscount = Comm.ToDecimal(dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cDiscAmount)].Value);

                        string[] strBatchUniq;
                        //clsJPDinfo.BatchUnique = dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cBarCode)].Value.ToString();
                        if (dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cBarCode)].Value.ToString().ToUpper() == "<AUTO BARCODE>")
                        {
                            if (dtBatchUniq.Rows.Count > 0)
                                clsJPDinfo.BatchUnique = dtBatchUniq.Rows[0]["BatchUniq"].ToString();
                            else
                                clsJPDinfo.BatchUnique = dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cBarCode)].Value.ToString();
                        }
                        else
                        {
                            strBatchUniq = dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cBarCode)].Value.ToString().Split('@');
                            if (strBatchUniq.Length > 0)
                            {
                                if (strBatchUniq.Length == 2)
                                {
                                    if (Comm.ToDecimal(dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cMRP)].Value) != Comm.ToDecimal(strBatchUniq[1].ToString()))
                                    {
                                        clsJPDinfo.BatchUnique = strBatchUniq[0].ToString() + "@" + Comm.ToDecimal(dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cMRP)].Value).ToString(AppSettings.CurrDecimalFormat);
                                    }
                                    else
                                        clsJPDinfo.BatchUnique = dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cBarCode)].Value.ToString();
                                }
                                else if (strBatchUniq.Length == 3)
                                {
                                    if (Comm.ToDecimal(dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cMRP)].Value) != Comm.ToDecimal(strBatchUniq[1].ToString()))
                                    {
                                        clsJPDinfo.BatchUnique = strBatchUniq[0].ToString() + "@" + Comm.ToDecimal(dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cMRP)].Value).ToString(AppSettings.CurrDecimalFormat) + "@" + Convert.ToDateTime(dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.CExpiry)].Value).ToString("dd-MM-yy").Replace("-", "");
                                    }
                                    else
                                        clsJPDinfo.BatchUnique = dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cBarCode)].Value.ToString();
                                }
                                else
                                    clsJPDinfo.BatchUnique = dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cBarCode)].Value.ToString();
                            }
                            else
                            {
                                clsJPDinfo.BatchUnique = dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cBarCode)].Value.ToString();
                            }
                        }

                        clsJPDinfo.blnQtyIN = 1;
                        clsJPDinfo.CRate = Comm.ToDecimal(dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cCrate)].Value);
                        clsJPDinfo.CRateWithTax = Comm.ToDecimal(dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cCRateWithTax)].Value);
                        clsJPDinfo.Unit = dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.CUnit)].Value.ToString();
                        clsJPDinfo.ItemStockID = 0;
                        clsJPDinfo.IcessPercent = Comm.ToDecimal(dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cCCessPer)].Value);
                        clsJPDinfo.IcessAmt = 0;
                        clsJPDinfo.IQtyCompCessPer = 0;
                        clsJPDinfo.IQtyCompCessAmt = Comm.ToDecimal(dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cCCompCessQty)].Value);
                        clsJPDinfo.StockMRP = 0;
                        clsJPDinfo.IAgentCommPercent = 0;
                        clsJPDinfo.BlnDelete = 0;
                        clsJPDinfo.Id = Comm.ToDecimal(dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cID)].Value);
                        clsJPDinfo.StrOfferDetails = "";
                        clsJPDinfo.BlnOfferItem = 0;
                        clsJPDinfo.BalQty = 0;
                        clsJPDinfo.GrossAmount = Comm.ToDecimal(dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cGrossAmt)].Value);
                        clsJPDinfo.iFloodCessPer = Comm.ToDecimal(dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cFloodCessPer)].Value);
                        clsJPDinfo.iFloodCessAmt = Comm.ToDecimal(dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cFloodCessAmt)].Value);
                        clsJPDinfo.Srate1 = Comm.ToDecimal(dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cSRate1)].Value);
                        clsJPDinfo.Srate2 = Comm.ToDecimal(dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cSRate2)].Value);
                        clsJPDinfo.Srate3 = Comm.ToDecimal(dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cSRate3)].Value);
                        clsJPDinfo.Srate4 = Comm.ToDecimal(dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cSRate4)].Value);
                        clsJPDinfo.Srate5 = Comm.ToDecimal(dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cSRate5)].Value);
                        clsJPDinfo.Costrate = Comm.ToDecimal(dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cCrate)].Value);
                        clsJPDinfo.CostValue = 0;
                        clsJPDinfo.Profit = 0;
                        clsJPDinfo.ProfitPer = 0;
                        clsJPDinfo.DiscMode = 0;
                        clsJPDinfo.Srate1Per = Comm.ToDecimal(dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cSRate1Per)].Value);
                        clsJPDinfo.Srate2Per = Comm.ToDecimal(dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cSRate2Per)].Value);
                        clsJPDinfo.Srate3Per = Comm.ToDecimal(dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cSRate3Per)].Value);
                        clsJPDinfo.Srate4Per = Comm.ToDecimal(dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cSRate4Per)].Value);
                        clsJPDinfo.Srate5Per = Comm.ToDecimal(dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cSRate5Per)].Value);
                        lstJPDinfo.Add(clsJPDinfo);
                    }
                }
            }
            clsPM.clsJsonPDetailsInfoList_ = lstJPDinfo;

            #endregion

            #region "Item Unit Details ------------------------------------------- >>"

            List<clsJsonPDUnitinfo> lstJPDUnit = new List<clsJsonPDUnitinfo>();
            for (int j = 0; j < dgvPurchase.Rows.Count; j++)
            {
                if (dgvPurchase.Rows[j].Cells[GetEnum(GridColIndexes.CItemCode)].Value != null)
                {
                    if (dgvPurchase.Rows[j].Cells[GetEnum(GridColIndexes.CItemCode)].Value.ToString() != "")
                    {
                        DataTable dtUnit = new DataTable();
                        clsJPDUnitinfo = new clsJsonPDUnitinfo();
                        clsJPDUnitinfo.UnitID = Comm.ToDecimal(dgvPurchase.Rows[j].Cells[GetEnum(GridColIndexes.CUnit)].Tag);
                        clsJPDUnitinfo.UnitName = dgvPurchase.Rows[j].Cells[GetEnum(GridColIndexes.CUnit)].Value.ToString();
                        //dipu on 20-Apr-2022 ----->>
                        dtUnit = Comm.fnGetData("SELECT UnitShortName FROM tblUnit WHERE UnitID = " + Comm.ToDecimal(dgvPurchase.Rows[j].Cells[GetEnum(GridColIndexes.CUnit)].Tag) + "").Tables[0];
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
            for (int j = 0; j < dgvPurchase.Rows.Count; j++)
            {
                if (dgvPurchase.Rows[j].Cells[GetEnum(GridColIndexes.CItemCode)].Value != null)
                {
                    if (dgvPurchase.Rows[j].Cells[GetEnum(GridColIndexes.CItemCode)].Value.ToString() != "")
                    {
                        clsJPDIteminfo = new clsJsonPDIteminfo();
                        string[] sArrItm = GetItemDetails(Comm.ToDecimal(dgvPurchase.Rows[j].Cells[GetEnum(GridColIndexes.cItemID)].Value));
                        clsJPDIteminfo.ItemID = Comm.ToDecimal(dgvPurchase.Rows[j].Cells[GetEnum(GridColIndexes.cItemID)].Value);
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
            clsJSonPurchase clsPurchase = JsonConvert.DeserializeObject<clsJSonPurchase>(sToDeSerialize);

            txtPrefix.Text = clsVchType.TransactionPrefix;
            txtInvAutoNo.Text = Convert.ToString(clsPurchase.clsJsonPMInfo_.InvNo);
            txtInvAutoNo.Tag = Comm.ToDouble(clsPurchase.clsJsonPMInfo_.InvId);
            txtReferenceAutoNo.Tag = Comm.ToDouble(clsPurchase.clsJsonPMInfo_.AutoNum);
            dtpInvDate.Text = Convert.ToString(clsPurchase.clsJsonPMInfo_.InvDate);
            dtpEffective.Text = Convert.ToString(clsPurchase.clsJsonPMInfo_.EffectiveDate);
            txtReferencePrefix.Text = clsPurchase.clsJsonPMInfo_.RefNo;
            txtReferenceAutoNo.Text = Convert.ToString(clsPurchase.clsJsonPMInfo_.ReferenceAutoNO);
            if (clsPurchase.clsJsonPMInfo_.MOP.ToUpper() == "CASH")
                cboPayment.SelectedIndex = 0;
            else if (clsPurchase.clsJsonPMInfo_.MOP.ToUpper() == "CREDIT")
                cboPayment.SelectedIndex = 1;
            else if (clsPurchase.clsJsonPMInfo_.MOP.ToUpper() == "BOTH")
                cboPayment.SelectedIndex = 2;
            else if (clsPurchase.clsJsonPMInfo_.MOP.ToUpper() == "CASH DESK")
                cboPayment.SelectedIndex = 3;

            txtGrossAmt.Text = Comm.chkChangeValuetoZero(Convert.ToString(clsPurchase.clsJsonPMInfo_.GrossAmt));
            lblQtyTotal.Text = Comm.chkChangeValuetoZero(Convert.ToString(clsPurchase.clsJsonPMInfo_.QtyTotal));
            lblFreeTotal.Text = Comm.chkChangeValuetoZero(Convert.ToString(clsPurchase.clsJsonPMInfo_.FreeTotal));
            txtItemDiscTot.Text = Comm.chkChangeValuetoZero(Convert.ToString(clsPurchase.clsJsonPMInfo_.ItemDiscountTotal));
            this.txtDiscPerc.TextChanged -= this.txtDiscPerc_TextChanged;
            txtDiscPerc.Text = Comm.chkChangeValuetoZero(Convert.ToString(clsPurchase.clsJsonPMInfo_.DiscPer));
            this.txtDiscPerc.TextChanged += this.txtDiscPerc_TextChanged;
            Comm.chkChangeValuetoZero(Convert.ToString(clsPurchase.clsJsonPMInfo_.Discount));
            txtTaxable.Text = Comm.chkChangeValuetoZero(Convert.ToString(clsPurchase.clsJsonPMInfo_.Taxable));
            txtNonTaxable.Text = Comm.chkChangeValuetoZero(Convert.ToString(clsPurchase.clsJsonPMInfo_.NonTaxable));
            txtTaxAmt.Text = Comm.chkChangeValuetoZero(Convert.ToString(clsPurchase.clsJsonPMInfo_.TaxAmt));

            txtOtherExp.Text = Comm.chkChangeValuetoZero(Convert.ToString(clsPurchase.clsJsonPMInfo_.OtherExpense));
            txtNetAmt.Text = Comm.chkChangeValuetoZero(Convert.ToString(clsPurchase.clsJsonPMInfo_.NetAmount));
            txtCashDisc.Text = Comm.chkChangeValuetoZero(Convert.ToString(clsPurchase.clsJsonPMInfo_.CashDiscount));
            txtRoundOff.Text = Comm.chkChangeValuetoZero(Convert.ToString(clsPurchase.clsJsonPMInfo_.RoundOff));
            txtNarration.Text = Convert.ToString(clsPurchase.clsJsonPMInfo_.UserNarration);
            lblBillAmount.Text = Comm.chkChangeValuetoZero(Convert.ToString(clsPurchase.clsJsonPMInfo_.BillAmt));

            cboTaxMode.SelectedValue = clsPurchase.clsJsonPMTaxmodeInfo_.TaxModeID;
            cboCostCentre.SelectedValue = clsPurchase.clsJsonPMCCentreInfo_.CCID;
            cboSalesStaff.SelectedValue = clsPurchase.clsJsonPMEmployeeInfo_.EmpID;
            cboAgent.SelectedValue = clsPurchase.clsJsonPMAgentInfo_.AgentID;
            GetAgentDiscountAsperVoucherType();
            cboState.SelectedValue = clsPurchase.clsJsonPMStateInfo_.StateId;

            if (clsPurchase.clsJsonPMLedgerInfo_.LName.ToUpper() == "" || clsPurchase.clsJsonPMLedgerInfo_.LName.ToUpper() == "<GENERAL SUPPLIER>")
            {
                this.txtSupplier.TextChanged -= this.txtSupplier_TextChanged;
                txtSupplier.Text = "";
                this.txtSupplier.TextChanged += this.txtSupplier_TextChanged;
                txtMobile.Text = "";
                txtTaxRegn.Text = "";
                cboState.SelectedIndex = -1;
                cboBType.SelectedIndex = -1;
                txtAddress1.Text = "";
                dSupplierID = clsPurchase.clsJsonPMLedgerInfo_.LID;
                lblLID.Text = dSupplierID.ToString();
                txtSupplier.Tag = clsPurchase.clsJsonPMLedgerInfo_.LAliasName;
            }
            else
            {
                this.txtSupplier.TextChanged -= this.txtSupplier_TextChanged;
                txtSupplier.Text = clsPurchase.clsJsonPMLedgerInfo_.LName;
                this.txtSupplier.TextChanged += this.txtSupplier_TextChanged;
                txtMobile.Text = clsPurchase.clsJsonPMLedgerInfo_.MobileNo;
                txtTaxRegn.Text = clsPurchase.clsJsonPMLedgerInfo_.TaxNo;
                cboState.SelectedValue = clsPurchase.clsJsonPMLedgerInfo_.StateID;
                cboBType.SelectedItem = clsPurchase.clsJsonPMLedgerInfo_.GSTType;
                txtAddress1.Text = clsPurchase.clsJsonPMLedgerInfo_.Address;
                dSupplierID = clsPurchase.clsJsonPMLedgerInfo_.LID;
                lblLID.Text = dSupplierID.ToString();
                txtSupplier.Tag = clsPurchase.clsJsonPMLedgerInfo_.LAliasName;
                FillSupplierForSerializeJsonUsingID(Comm.ToInt32(dSupplierID));
            }
            DataTable dtGetPurDetail = clsPurchase.clsJsonPDetailsInfoList_.ToDataTable();
            DataTable dtItemFrmJson = clsPurchase.clsJsonPDIteminfoList_.ToDataTable();
            DataTable dtUnitFrmJson = clsPurchase.clsJsonPDUnitinfoList_.ToDataTable();
            if (dtGetPurDetail.Rows.Count > 0)
            {
                sqlControl rs = new sqlControl();

                AddColumnsToGrid();
                for (int i = 0; i < dtGetPurDetail.Rows.Count; i++)
                {
                    dgvPurchase.Rows.Add();

                    rs.Open("Select ItemCode,ItemName From tblItemMaster Where ItemID=" + dtGetPurDetail.Rows[i]["ItemId"].ToString());
                    if (!rs.eof())
                    {
                        dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.CItemCode)].Value = rs.fields("itemcode");
                        dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.CItemName)].Value = rs.fields("ItemName");
                    }

                    SetValue(GetEnum(GridColIndexes.cSlNo), i, (i + 1).ToString());
                    dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cID)].Value = dtGetPurDetail.Rows[i]["Id"].ToString();
                    dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.CUnit)].Value = dtUnitFrmJson.Rows[i]["UnitName"].ToString();
                    dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.CUnit)].Tag = dtGetPurDetail.Rows[i]["UnitId"].ToString();
                    dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cBarCode)].Tag = dtGetPurDetail.Rows[i]["BatchCode"].ToString();
                    dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cBarCode)].Value = dtGetPurDetail.Rows[i]["BatchUnique"].ToString();
                    dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.CExpiry)].Value = Convert.ToDateTime(dtGetPurDetail.Rows[i]["Expiry"]).ToString("dd-MMM-yyyy");
                    dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cMRP)].Value = Comm.FormatValue(Comm.ToDouble(dtGetPurDetail.Rows[i]["MRP"].ToString()), true);
                    dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cPrate)].Value = Comm.FormatValue(Comm.ToDouble(dtGetPurDetail.Rows[i]["Prate"].ToString()), true);
                    this.dgvPurchase.Columns[GetEnum(GridColIndexes.cQty)].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cQty)].Value = Comm.FormatValue(Comm.ToDouble(dtGetPurDetail.Rows[i]["Qty"].ToString()), false);
                    this.dgvPurchase.Columns[GetEnum(GridColIndexes.cFree)].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cFree)].Value = Comm.FormatValue(Comm.ToDouble(dtGetPurDetail.Rows[i]["Free"].ToString()), false);

                    dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cSRate1Per)].Value = Comm.FormatValue(Comm.ToDouble(dtGetPurDetail.Rows[i]["Srate1Per"].ToString()), true);
                    dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cSRate1)].Value = Comm.FormatValue(Comm.ToDouble(dtGetPurDetail.Rows[i]["Srate1"].ToString()), true);
                    dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cSRate2Per)].Value = Comm.FormatValue(Comm.ToDouble(dtGetPurDetail.Rows[i]["Srate2Per"].ToString()), true);
                    dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cSRate2)].Value = Comm.FormatValue(Comm.ToDouble(dtGetPurDetail.Rows[i]["Srate2"].ToString()), true);
                    dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cSRate3Per)].Value = Comm.FormatValue(Comm.ToDouble(dtGetPurDetail.Rows[i]["Srate3Per"].ToString()), true);
                    dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cSRate3)].Value = Comm.FormatValue(Comm.ToDouble(dtGetPurDetail.Rows[i]["Srate3"].ToString()), true);
                    dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cSRate4Per)].Value = Comm.FormatValue(Comm.ToDouble(dtGetPurDetail.Rows[i]["Srate4Per"].ToString()), true);
                    dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cSRate4)].Value = Comm.FormatValue(Comm.ToDouble(dtGetPurDetail.Rows[i]["Srate4"].ToString()), true);
                    dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cSRate5Per)].Value = Comm.FormatValue(Comm.ToDouble(dtGetPurDetail.Rows[i]["Srate5Per"].ToString()), true);
                    dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cSRate5)].Value = Comm.FormatValue(Comm.ToDouble(dtGetPurDetail.Rows[i]["Srate5"].ToString()), true);

                    dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cGrossAmt)].Value = Comm.FormatValue(Comm.ToDouble(dtGetPurDetail.Rows[i]["GrossAmount"].ToString()), true);
                    dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cDiscPer)].Value = Comm.FormatValue(Comm.ToDouble(dtGetPurDetail.Rows[i]["ItemDiscountPer"].ToString()), true);
                    this.dgvPurchase.Columns[GetEnum(GridColIndexes.cDiscPer)].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cDiscAmount)].Value = Comm.FormatValue(Comm.ToDouble(dtGetPurDetail.Rows[i]["ItemDiscount"].ToString()), true);
                    dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cBillDisc)].Value = Comm.FormatValue(Comm.ToDouble(dtGetPurDetail.Rows[i]["Discount"].ToString()), true);
                    dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cCrate)].Value = Comm.FormatValue(Comm.ToDouble(dtGetPurDetail.Rows[i]["CRate"].ToString()), true);
                    dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cItemID)].Value = dtGetPurDetail.Rows[i]["ItemId"].ToString();
                    dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.ctaxPer)].Value = Comm.FormatValue(Comm.ToDouble(dtGetPurDetail.Rows[i]["TaxPer"].ToString()), true);
                    this.dgvPurchase.Columns[GetEnum(GridColIndexes.ctaxPer)].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.ctax)].Value = Comm.FormatValue(Comm.ToDouble(dtGetPurDetail.Rows[i]["TaxAmount"].ToString()), true);
                    dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cIGST)].Tag = Comm.FormatValue(Comm.ToDouble(dtGetPurDetail.Rows[i]["IGSTTaxPer"].ToString()), true);
                    dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cIGST)].Value = Comm.FormatValue(Comm.ToDouble(dtGetPurDetail.Rows[i]["IGSTTaxAmt"].ToString()), true);
                    dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cSGST)].Tag = Comm.FormatValue(Comm.ToDouble(dtGetPurDetail.Rows[i]["CGSTTaxPer"].ToString()), true);
                    dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cSGST)].Value = Comm.FormatValue(Comm.ToDouble(dtGetPurDetail.Rows[i]["CGSTTaxAmt"].ToString()), true);
                    dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cCGST)].Tag = Comm.FormatValue(Comm.ToDouble(dtGetPurDetail.Rows[i]["SGSTTaxPer"].ToString()), true);
                    dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cCGST)].Value = Comm.FormatValue(Comm.ToDouble(dtGetPurDetail.Rows[i]["SGSTTaxAmt"].ToString()), true);
                    dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cNetAmount)].Value = Comm.FormatValue(Comm.ToDouble(dtGetPurDetail.Rows[i]["INetAmount"].ToString()), true);
                    dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cNonTaxable)].Value = Comm.FormatValue(Comm.ToDouble(dtGetPurDetail.Rows[i]["InonTaxableAmount"].ToString()), true);
                    this.dgvPurchase.Columns[GetEnum(GridColIndexes.cNonTaxable)].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cCCessPer)].Value = Comm.FormatValue(Comm.ToDouble(dtGetPurDetail.Rows[i]["IcessPercent"].ToString()), true);
                    this.dgvPurchase.Columns[GetEnum(GridColIndexes.cCCessPer)].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cCCompCessQty)].Value = Comm.FormatValue(Comm.ToDouble(dtGetPurDetail.Rows[i]["IQtyCompCessAmt"].ToString()), true);
                    this.dgvPurchase.Columns[GetEnum(GridColIndexes.cCCompCessQty)].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cFloodCessPer)].Value = Comm.FormatValue(Comm.ToDouble(dtGetPurDetail.Rows[i]["iFloodCessPer"].ToString()), true);
                    this.dgvPurchase.Columns[GetEnum(GridColIndexes.cFloodCessPer)].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cFloodCessAmt)].Value = Comm.FormatValue(Comm.ToDouble(dtGetPurDetail.Rows[i]["iFloodCessAmt"].ToString()), true);
                    this.dgvPurchase.Columns[GetEnum(GridColIndexes.cFloodCessAmt)].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cStockMRP)].Value = Comm.FormatValue(Comm.ToDouble(dtGetPurDetail.Rows[i]["StockMRP"].ToString()), true);
                    this.dgvPurchase.Columns[GetEnum(GridColIndexes.cStockMRP)].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cAgentCommPer)].Value = Comm.FormatValue(Comm.ToDouble(dtGetPurDetail.Rows[i]["IAgentCommPercent"].ToString()), true);
                    this.dgvPurchase.Columns[GetEnum(GridColIndexes.cAgentCommPer)].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cBlnOfferItem)].Value = dtGetPurDetail.Rows[i]["BlnOfferItem"].ToString();
                    dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cStrOfferDetails)].Value = dtGetPurDetail.Rows[i]["StrOfferDetails"].ToString();
                    dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cBatchMode)].Value = dtItemFrmJson.Rows[i]["BatchMode"].ToString();

                    dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cCoolie)].Value = dtItemFrmJson.Rows[i]["Coolie"].ToString();

                    if (Comm.ToDouble(dtGetPurDetail.Rows[i]["RateInclusive"].ToString()) == 1)
                        dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cRateinclusive)].Value = true;
                    else
                        dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cRateinclusive)].Value = false;

                    this.dgvPurchase.Columns[i].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                }

                if (Comm.ToInt32(AppSettings.StateCode) != Comm.ToInt32(cboState.SelectedValue))
                {
                    dgvPurchase.Columns[GetEnum(GridColIndexes.cCGST)].Visible = false;
                    dgvPurchase.Columns[GetEnum(GridColIndexes.cSGST)].Visible = false;
                    dgvPurchase.Columns[GetEnum(GridColIndexes.cIGST)].Visible = true;
                }
                else
                {
                    dgvPurchase.Columns[GetEnum(GridColIndexes.cCGST)].Visible = true;
                    dgvPurchase.Columns[GetEnum(GridColIndexes.cSGST)].Visible = true;
                    dgvPurchase.Columns[GetEnum(GridColIndexes.cIGST)].Visible = false;
                }
                CalcTotal();
            }
        }

        private void LoadBill(int iSelectedID)
        {
            try
            {

                sqlControl rs = new sqlControl();

                rs.Open("Select s.*,si.*,i.itemname,i.itemcode,i.BatchMode,i.Coolie,l.lid,l.lname,l.laliasname,l.TaxNo,l.Address,UnitName From tblLedger as l, tblPurchaseItem as si, tblItemMAster as i, tblPurchase as s, tblUnit as u Where si.itemid = i.itemid and si.unitid = u.unitid and s.LedgerID = l.LID and s.InvID = si.InvID and s.InvID = " + iSelectedID + " order by slno ");

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
                    dtpEffective.Text = Convert.ToString(rs.fields("EffectiveDate"));
                    txtReferencePrefix.Text = rs.fields("RefNo");
                    txtReferenceAutoNo.Text = Convert.ToString(rs.fields("ReferenceAutoNO"));
                    if (rs.fields("MOP").ToUpper() == "CASH")
                        cboPayment.SelectedIndex = 0;
                    else if (rs.fields("MOP").ToUpper() == "CREDIT")
                        cboPayment.SelectedIndex = 1;
                    else if (rs.fields("MOP").ToUpper() == "BOTH")
                        cboPayment.SelectedIndex = 2;
                    else if (rs.fields("MOP").ToUpper() == "CASH DESK")
                        cboPayment.SelectedIndex = 3;

                    txtGrossAmt.Text = Comm.chkChangeValuetoZero(Convert.ToString(rs.fields("GrossAmt")));
                    lblQtyTotal.Text = Comm.chkChangeValuetoZero(Convert.ToString(rs.fields("QtyTotal")));
                    //lblFreeTotal.Text = Comm.chkChangeValuetoZero(Convert.ToString(rs.fields("FreeTotal")));
                    txtItemDiscTot.Text = Comm.chkChangeValuetoZero(Convert.ToString(rs.fields("ItemDiscountTotal")));
                    this.txtDiscPerc.TextChanged -= this.txtDiscPerc_TextChanged;
                    txtDiscPerc.Text = Comm.chkChangeValuetoZero(Convert.ToString(rs.fields("DiscPer")));
                    this.txtDiscPerc.TextChanged += this.txtDiscPerc_TextChanged;
                    Comm.chkChangeValuetoZero(Convert.ToString(rs.fields("Discount")));
                    txtTaxable.Text = Comm.chkChangeValuetoZero(Convert.ToString(rs.fields("Taxable")));
                    txtNonTaxable.Text = Comm.chkChangeValuetoZero(Convert.ToString(rs.fields("NonTaxable")));
                    txtTaxAmt.Text = Comm.chkChangeValuetoZero(Convert.ToString(rs.fields("TaxAmt")));

                    txtOtherExp.Text = Comm.chkChangeValuetoZero(Convert.ToString(rs.fields("OtherExpense")));
                    txtNetAmt.Text = Comm.chkChangeValuetoZero(Convert.ToString(rs.fields("NetAmount")));
                    txtCashDisc.Text = Comm.chkChangeValuetoZero(Convert.ToString(rs.fields("CashDiscount")));
                    txtRoundOff.Text = Comm.chkChangeValuetoZero(Convert.ToString(rs.fields("RoundOff")));
                    txtNarration.Text = Convert.ToString(rs.fields("UserNarration"));
                    lblBillAmount.Text = Comm.chkChangeValuetoZero(Convert.ToString(rs.fields("BillAmt")));

                    cboTaxMode.SelectedValue = rs.fields("TaxModeID");
                    cboCostCentre.SelectedValue = rs.fields("CCID");
                    cboSalesStaff.SelectedValue = rs.fields("SalesManID");
                    cboAgent.SelectedValue = rs.fields("AgentID");
                    GetAgentDiscountAsperVoucherType();
                    cboState.SelectedValue = rs.fields("StateId");

                    if (rs.fields("LName").ToUpper() == "" || rs.fields("LName").ToUpper() == " < GENERAL SUPPLIER>")
                    {
                        this.txtSupplier.TextChanged -= this.txtSupplier_TextChanged;
                        txtSupplier.Text = "";
                        this.txtSupplier.TextChanged += this.txtSupplier_TextChanged;
                        txtMobile.Text = "";
                        txtTaxRegn.Text = "";
                        cboState.SelectedIndex = -1;
                        cboBType.SelectedIndex = -1;
                        txtAddress1.Text = "";
                        dSupplierID = Comm.ToDecimal(rs.fields("LID"));
                        lblLID.Text = dSupplierID.ToString();
                        txtSupplier.Tag = rs.fields("LAliasName");
                    }
                    else
                    {
                        this.txtSupplier.TextChanged -= this.txtSupplier_TextChanged;
                        txtSupplier.Text = rs.fields("LName");
                        this.txtSupplier.TextChanged += this.txtSupplier_TextChanged;
                        txtMobile.Text = rs.fields("MobileNo");
                        txtTaxRegn.Text = rs.fields("TaxNo");
                        cboState.SelectedValue = rs.fields("StateID");
                        cboBType.SelectedItem = rs.fields("GSTType");
                        txtAddress1.Text = rs.fields("Address");
                        dSupplierID = Comm.ToDecimal(rs.fields("LID"));
                        lblLID.Text = dSupplierID.ToString();
                        txtSupplier.Tag = rs.fields("LAliasName");
                        FillSupplierForSerializeJsonUsingID(Comm.ToInt32(dSupplierID));
                    }
                }

                AddColumnsToGrid();
                int i = 0;
                while (rs.eof() == false)
                {
                    dgvPurchase.Rows.Add();


                    dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.CItemCode)].Value = rs.fields("itemcode");
                    dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.CItemName)].Value = rs.fields("ItemName");

                    SetValue(GetEnum(GridColIndexes.cSlNo), i, (i + 1).ToString());
                    dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cID)].Value = rs.fields("Id").ToString();
                    dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.CUnit)].Value = rs.fields("UnitName").ToString();
                    dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.CUnit)].Tag = rs.fields("UnitId").ToString();
                    dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cBarCode)].Tag = rs.fields("BatchCode").ToString();
                    dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cBarCode)].Value = rs.fields("BatchUnique").ToString();
                    dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.CExpiry)].Value = Convert.ToDateTime(rs.fields("Expiry")).ToString("dd-MMM-yyyy");
                    dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cMRP)].Value = Comm.FormatValue(Comm.ToDouble(rs.fields("MRP").ToString()), true);
                    dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cPrate)].Value = Comm.FormatValue(Comm.ToDouble(rs.fields("Prate").ToString()), true);
                    this.dgvPurchase.Columns[GetEnum(GridColIndexes.cQty)].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cQty)].Value = Comm.FormatValue(Comm.ToDouble(rs.fields("Qty").ToString()), false);
                    this.dgvPurchase.Columns[GetEnum(GridColIndexes.cFree)].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cFree)].Value = Comm.FormatValue(Comm.ToDouble(rs.fields("Free").ToString()), false);

                    dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cSRate1Per)].Value = Comm.FormatValue(Comm.ToDouble(rs.fields("Srate1Per").ToString()), true);
                    dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cSRate1)].Value = Comm.FormatValue(Comm.ToDouble(rs.fields("Srate1").ToString()), true);
                    dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cSRate2Per)].Value = Comm.FormatValue(Comm.ToDouble(rs.fields("Srate2Per").ToString()), true);
                    dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cSRate2)].Value = Comm.FormatValue(Comm.ToDouble(rs.fields("Srate2").ToString()), true);
                    dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cSRate3Per)].Value = Comm.FormatValue(Comm.ToDouble(rs.fields("Srate3Per").ToString()), true);
                    dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cSRate3)].Value = Comm.FormatValue(Comm.ToDouble(rs.fields("Srate3").ToString()), true);
                    dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cSRate4Per)].Value = Comm.FormatValue(Comm.ToDouble(rs.fields("Srate4Per").ToString()), true);
                    dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cSRate4)].Value = Comm.FormatValue(Comm.ToDouble(rs.fields("Srate4").ToString()), true);
                    dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cSRate5Per)].Value = Comm.FormatValue(Comm.ToDouble(rs.fields("Srate5Per").ToString()), true);
                    dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cSRate5)].Value = Comm.FormatValue(Comm.ToDouble(rs.fields("Srate5").ToString()), true);

                    dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cGrossAmt)].Value = Comm.FormatValue(Comm.ToDouble(rs.fields("GrossAmount").ToString()), true);
                    dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cDiscPer)].Value = Comm.FormatValue(Comm.ToDouble(rs.fields("ItemDiscountPer").ToString()), true);
                    this.dgvPurchase.Columns[GetEnum(GridColIndexes.cDiscPer)].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cDiscAmount)].Value = Comm.FormatValue(Comm.ToDouble(rs.fields("ItemDiscount").ToString()), true);
                    dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cBillDisc)].Value = Comm.FormatValue(Comm.ToDouble(rs.fields("Discount").ToString()), true);
                    dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cCrate)].Value = Comm.FormatValue(Comm.ToDouble(rs.fields("CRate").ToString()), true);
                    dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cItemID)].Value = rs.fields("ItemId").ToString();
                    dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.CItemName)].Tag = rs.fields("ItemId").ToString();
                    dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.ctaxPer)].Value = Comm.FormatValue(Comm.ToDouble(rs.fields("TaxPer").ToString()), true);
                    this.dgvPurchase.Columns[GetEnum(GridColIndexes.ctaxPer)].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.ctax)].Value = Comm.FormatValue(Comm.ToDouble(rs.fields("TaxAmount").ToString()), true);
                    dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cIGST)].Tag = Comm.FormatValue(Comm.ToDouble(rs.fields("IGSTTaxPer").ToString()), true);
                    dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cIGST)].Value = Comm.FormatValue(Comm.ToDouble(rs.fields("IGSTTaxAmt").ToString()), true);
                    dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cSGST)].Tag = Comm.FormatValue(Comm.ToDouble(rs.fields("CGSTTaxPer").ToString()), true);
                    dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cSGST)].Value = Comm.FormatValue(Comm.ToDouble(rs.fields("CGSTTaxAmt").ToString()), true);
                    dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cCGST)].Tag = Comm.FormatValue(Comm.ToDouble(rs.fields("SGSTTaxPer").ToString()), true);
                    dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cCGST)].Value = Comm.FormatValue(Comm.ToDouble(rs.fields("SGSTTaxAmt").ToString()), true);
                    dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cNetAmount)].Value = Comm.FormatValue(Comm.ToDouble(rs.fields("INetAmount").ToString()), true);
                    dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cNonTaxable)].Value = Comm.FormatValue(Comm.ToDouble(rs.fields("InonTaxableAmount").ToString()), true);
                    this.dgvPurchase.Columns[GetEnum(GridColIndexes.cNonTaxable)].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cCCessPer)].Value = Comm.FormatValue(Comm.ToDouble(rs.fields("IcessPercent").ToString()), true);
                    this.dgvPurchase.Columns[GetEnum(GridColIndexes.cCCessPer)].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cCCompCessQty)].Value = Comm.FormatValue(Comm.ToDouble(rs.fields("IQtyCompCessAmt").ToString()), true);
                    this.dgvPurchase.Columns[GetEnum(GridColIndexes.cCCompCessQty)].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cFloodCessPer)].Value = Comm.FormatValue(Comm.ToDouble(rs.fields("iFloodCessPer").ToString()), true);
                    this.dgvPurchase.Columns[GetEnum(GridColIndexes.cFloodCessPer)].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cFloodCessAmt)].Value = Comm.FormatValue(Comm.ToDouble(rs.fields("iFloodCessAmt").ToString()), true);
                    this.dgvPurchase.Columns[GetEnum(GridColIndexes.cFloodCessAmt)].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cStockMRP)].Value = Comm.FormatValue(Comm.ToDouble(rs.fields("StockMRP").ToString()), true);
                    this.dgvPurchase.Columns[GetEnum(GridColIndexes.cStockMRP)].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cAgentCommPer)].Value = Comm.FormatValue(Comm.ToDouble(rs.fields("IAgentCommPercent").ToString()), true);
                    this.dgvPurchase.Columns[GetEnum(GridColIndexes.cAgentCommPer)].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cBlnOfferItem)].Value = rs.fields("BlnOfferItem").ToString();
                    dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cStrOfferDetails)].Value = rs.fields("StrOfferDetails").ToString();
                    dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cBatchMode)].Value = rs.fields("BatchMode").ToString();

                    dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cCoolie)].Value = rs.fields("Coolie").ToString();

                    if (Comm.ToDouble(rs.fields("RateInclusive").ToString()) == 1)
                        dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cRateinclusive)].Value = true;
                    else
                        dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cRateinclusive)].Value = false;

                    //this.dgvPurchase.Columns[i].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;

                    rs.MoveNext();
                    i++;
                }

                if (Comm.ToInt32(AppSettings.StateCode) != Comm.ToInt32(cboState.SelectedValue))
                {
                    dgvPurchase.Columns[GetEnum(GridColIndexes.cCGST)].Visible = false;
                    dgvPurchase.Columns[GetEnum(GridColIndexes.cSGST)].Visible = false;
                    dgvPurchase.Columns[GetEnum(GridColIndexes.cIGST)].Visible = true;
                }
                else
                {
                    dgvPurchase.Columns[GetEnum(GridColIndexes.cCGST)].Visible = true;
                    dgvPurchase.Columns[GetEnum(GridColIndexes.cSGST)].Visible = true;
                    dgvPurchase.Columns[GetEnum(GridColIndexes.cIGST)].Visible = false;

                }
                CalcTotal();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        //Description : CRUD Operational Method for Insert, Update and Delete.
        //private void CRUD_Operations(int iAction = 0)
        //{
        //    bool blnTransactionStarted = false;

        //    try
        //    {
        //        string[] strResult;
        //        string sRetDet;

        //        int maxid;

        //        DBConnection dBConnection = new DBConnection();
        //        var sqlConn = dBConnection.GetDBConnection();
        //        SqlTransaction trans = sqlConn.BeginTransaction();

        //        blnTransactionStarted = true;


        //        try
        //        {
        //            if (IsValidate() == true)
        //            {
        //                string strJson = SerializetoJson();

        //                if (iIDFromEditWindow == 0)
        //                {
        //                    maxid = Comm.gfnGetNextSerialNo("tblPurchase", "InvId");
        //                }
        //                else
        //                {
        //                    maxid = iIDFromEditWindow;

        //                }

        //                string sql = "insert into tblPurchase(InvId,LedgerId,Prefix,AutoNum, Party,PartyAddress,PARTYGSTIN,StateID,BillType,PriceListID,CCID,AGENTID,InvNo,InvDate,BillTime,MOP,taxmodeID,SalesManID,GrossAmt,ItemDiscountTotal,TaxAmt,Taxable,NonTaxable,CGSTTotal,SGSTTotal,IGSTTotal,NetAmount,OtherExpense,UserNarration,DiscPer,Discount,CashDiscount,RoundOFF,vchtypeID,sortNUmber,blnHold,EffectiveDate,RefNo,BillAmt,cancelled,userID,GStType,PartyCode,MobileNo,email,DestCCID,AgentCommMode,AgentCommAmount,AgentLID,BlnStockInsert,DeliveryNoteDetails,OrderDetails,ConvertedVchNo, ConvertedVchTypeID,ConvertedParentVchTypeID,CustomerpointsSettled,SystemName,LastUpdateDate,LastUpdateTime,DeliveryDetails, DespatchDetails, TermsOfDelivery,FloodCessTot,CounterID,ExtraCharges,CessAmountTot,QtyCompCessAmount) " +
        //                                    " values(" + maxid + "," + Conversion.Val(txtSupplier.Tag.ToString()) + ",'" + txtPrefix.Text.Trim() + "'," + Conversion.Val(txtInvAutoNo.Text.ToString()) + ",'" + txtSupplier.Text.ToString().Replace("'", "''") + "','" + "" + "','" + txtGSTNo.Text.ToString, "'", "''") + "'," + cmbStateCode.ItemData(cmbStateCode.SelectedIndex) + " ,'" + cmbBillType.Text + "'," + 0 + " ," + cmbCostCentre.ItemData(cmbCostCentre.SelectedIndex) + "," + 0 + ",'" + Replace(txtvchnoPrefix.Text.ToString & txtVchNo.Text.ToString, "'", "''") + "' ,'" + Format(dtpVchDate.Value, "dd/MMM/yyyy") + "','" + DateTime.Now.ToLongTimeString.ToString + "' ,'" + cmbMOP.Text + "','" + cmbTaxMode.ItemData(cmbTaxMode.SelectedIndex) + "'," + cmbStaff.ItemData(cmbStaff.SelectedIndex) + "," + Val(dgv.GetValue(LinkIDs.GrossAmt).ToString) + "," + Val(dgv.GetValue(LinkIDs.ItemDiscountTotal).ToString) + "," + Val(dgv.GetValue(LinkIDs.TaxAmount).ToString) + "," + Val(dgv.GetValue(LinkIDs.TaxableAmount).ToString) + "," + Val(dgv.GetValue(LinkIDs.NonTaxableAmount).ToString) + "," + Val(dgv.GetValue(LinkIDs.CGST).ToString) + "," + Val(dgv.GetValue(LinkIDs.SGST).ToString) + "," + Val(dgv.GetValue(LinkIDs.IGST).ToString) + "," + Val(dgv.GetValue(LinkIDs.NetAmount).ToString) + "," + Val(txtOtherExpense.Text) + ",'" + Replace(txtNarration.Text.ToString, "'", "''") + "'," + Val(txtDiscper.Text) + "," + Val(txtDiscAmount.Text.ToString) + "," + Val(txtCashDiscount.Text.ToString) + "," + Val(txtRoundOff.Text.ToString) + "," + Val(mytrans.MVchTypeID) + "," + Val(txtVchNo.Text) + "," + IntHold + ",'" + Format(DtpEffectiveDate.Value, "dd/MMM/yyyy") + "','" + Replace(txtRefNo.Text.ToString, "'", "''") + "'," +
        //                                    Val(lblBalance.Text) + ",0," + DCSApp.strUserID + ",'" + dgv.GetValue(LinkIDs.GSTType).ToString + "','" + txtpartySearch.Text.Replace("'", "''") + "','" + txtMobileNo.Text.Replace("'", "''") + "','',0,'" + dgv.GetValue(LinkIDs.AgentCommissionMode).ToString + "'," + Val(dgv.GetValue(LinkIDs.AgentCommission).ToString) + "," + Val(0) + "," + BlnStockinsert + ",'" + txtDeliveryNoteDetails.Text.Replace(" ", "") + "','" + txtOrderDetails.Text.Replace(" ", "") + "','" + ConvertedVchNo + "'," + Val(ConvertedVchTypeID) + "," + Val(ConvertedParentVchTypeID) + ",0,'" + DCSApp.StrComputerName + "' , '" + Format(Now, "dd/MMM/yyyy") + "','" + Format(CDate(Now), "HH:mm:ss") + "','" + txtDeliveryDetail.Text.Replace("'", "''") + "','" + txtDespatchDetail.Text.Replace("'", "''") + "','" + txtTermsofDelivery.Text.Replace("'", "''") + "'," + Val(dgv.GetValue(LinkIDs.FloodCessTotal).ToString) + "," + DCSApp.CounterID + "," + Val(btnLandingCost.Text.ToString) + "," + Val(dgv.GetValue(LinkIDs.CessAmount).ToString) + "," + Val(dgv.GetValue(LinkIDs.QtyCompCessAmount).ToString) + ")")

        //                    SqlCommand sqlCmd = new SqlCommand(sql, sqlConn, trans);



        //                recAffected = rs.RecordCount
        //        RecCount = RecCount + recAffected
        //    Else
        //        rs.Execute("update " + StrMastertableName + " set AutoNum=" + Val(txtVchNo.Text) + ",MobileNo='" + txtMobileNo.Text.Replace("'", "''") + "',email='', PartyCode='" + txtpartySearch.Text.Replace("'", "''") + "', Prefix='" + Trim(txtvchnoPrefix.Text) + "',sortNUmber=" + Val(txtVchNo.Text) + ",LedgerId= " + ledgerId + ", Party='" + Replace(txtpartySearch.Text.ToString, "'", "''") + "',PartyAddress='" + Replace("", "'", "''") + "', PARTYGSTIN ='" + Replace(txtGSTNo.Text.ToString, "'", "''") + "',StateID=" + cmbStateCode.ItemData(cmbStateCode.SelectedIndex) + ",BillType='" + cmbBillType.Text + "',PriceListID=" + 0 + ",CCID=" + cmbCostCentre.ItemData(cmbCostCentre.SelectedIndex) + ",AGENTID=" + 0 + ",InvNo='" + (txtvchnoPrefix.Text.ToString.Replace("'", "''") & txtVchNo.Text.ToString) + "',InvDate='" + Format(dtpVchDate.Value, "dd/MMM/yyyy") + "',BillTime='" + DateTime.Now.ToLongTimeString.ToString + "',MOP='" + cmbMOP.Text + "',taxmodeID='" + cmbTaxMode.ItemData(cmbTaxMode.SelectedIndex) + "',SalesManID=" + cmbStaff.ItemData(cmbStaff.SelectedIndex) + ",GrossAmt=" + Val(dgv.GetValue(LinkIDs.GrossAmt).ToString) + ",ItemDiscountTotal=" + Val(dgv.GetValue(LinkIDs.ItemDiscountTotal).ToString) + ",TaxAmt=" + Val(dgv.GetValue(LinkIDs.TaxAmount).ToString) + ",Taxable=" + Val(dgv.GetValue(LinkIDs.TaxableAmount).ToString) + ",NonTaxable=" + Val(dgv.GetValue(LinkIDs.NonTaxableAmount).ToString) + ",CGSTTotal=" + Val(dgv.GetValue(LinkIDs.CGST).ToString) + ",SGSTTotal=" + Val(dgv.GetValue(LinkIDs.SGST).ToString) + ",IGSTTotal=" + Val(dgv.GetValue(LinkIDs.IGST).ToString) + ",NetAmount=" + Val(dgv.GetValue(LinkIDs.NetAmount).ToString) + ",OtherExpense=" + Val(txtOtherExpense.Text) + ",UserNarration='" + Replace(txtNarration.Text.ToString, "'", "''") + "',DiscPer=" + Val(txtDiscper.Text) + ",Discount=" + Val(txtDiscAmount.Text.ToString) + ",CashDiscount=" + Val(txtCashDiscount.Text.ToString) + ",RoundOFF=" + Val(txtRoundOff.Text.ToString) + ",vchtypeID=" + Val(mytrans.MVchTypeID) + ",blnHold=" + IntHold + ",RefNo='" + Replace(txtRefNo.Text.ToString, "'", "''") + "',EffectiveDate='" + Format(DtpEffectiveDate.Value, "dd/MMM/yyyy") + "' " &
        //                   " ,BillAmt=" + Val(lblBalance.Text) + ",cancelled=" + IntCancel + ",userID=" + DCSApp.strUserID + ",GStType='" + dgv.GetValue(LinkIDs.GSTType).ToString + "',DestCCID=" + 0 + ",AgentCommMode='" + dgv.GetValue(LinkIDs.AgentCommissionMode).ToString + "',AgentCommAmount=" + Val(dgv.GetValue(LinkIDs.AgentCommission).ToString) + ",AgentLID=" + Val(0) + ",blnstockinsert=" + BlnStockinsert + ",DeliveryNoteDetails='" + txtDeliveryNoteDetails.Text.Replace(" ", "") + "',OrderDetails='" + txtOrderDetails.Text.Replace(" ", "") + "',ConvertedVchNo='" + ConvertedVchNo + "', ConvertedVchTypeID=" + Val(ConvertedVchTypeID) + ",ConvertedParentVchTypeID=" + Val(ConvertedParentVchTypeID) + ",CustomerpointsSettled=0,SystemName ='" + DCSApp.StrComputerName + "',LastUpdateDate ='" + Format(Now, "dd/MMM/yyyy") + "' ,LastUpdateTime ='" + Format(CDate(Now), "HH:mm:ss") + "',DeliveryDetails='" + txtDeliveryDetail.Text.Replace("'", "''") + "',DespatchDetails='" + txtDespatchDetail.Text.Replace("'", "''") + "',TermsOfDelivery='" + txtTermsofDelivery.Text.Replace("'", "''") + "',FloodCessTot=" + Val(dgv.GetValue(LinkIDs.FloodCessTotal).ToString) + ",ExtraCharges=" + Val(btnLandingCost.Text) + ",CessAmountTot=" + Val(dgv.GetValue(LinkIDs.CessAmount).ToString) + ",QtyCompCessAmount=" + Val(dgv.GetValue(LinkIDs.QtyCompCessAmount).ToString) + "  where InvID=" + Val(txtVchNo.Tag.ToString))

        //        recAffected = rs.RecordCount
        //        RecCount = RecCount + recAffected
        //        rs.Execute("Update " + StrDetailTableName + " Set blndelete = 0 where InvID=" + Val(txtVchNo.Tag.ToString))
        //    End If
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            MessageBox.Show(ex.Message, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
        //            if (blnTransactionStarted == true)
        //            {
        //                trans.Rollback();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
        //    }


        //}
        private void CRUD_Operations(int iAction = 0, bool blnLoadTest = false)
        {
            //return;
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

                        #region "DELETE THE ACCOUNT POSTING IF EDIT MODE"

                        if (iIDFromEditWindow != 0)
                        {
                            sqlControl rs = new sqlControl();
                            rs.ShowExceptionAutomatically = true;
                            rs.Execute("Delete from tblvoucher where refid = " + iIDFromEditWindow + " and vchtypeID = " + vchtypeID);
                            if (rs.Exception != "")
                            {
                                MessageBox.Show(rs.Exception, "Accounts Posting", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                trans.Rollback();
                                return;
                            }
                        }

                        #endregion

                        //for (int i = 0; i < LoadTestCount; i++)
                        //{ 
                        //}
                        #region "CRUD Operations for Purchase Master ------------------------- >>"
                        if (iAction != 2)
                        {
                            string sRet = clsPur.PurchaseMasterCRUD(clsPM, sqlConn, trans, strJson, iAction);
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

                        #region "CRUD Operations for Purchase Detail ------------------------- >>"
                        Hashtable hstPurStk = new Hashtable();

                        if (iAction == 1) // Edit
                        {
                            sRetDet = clsPur.PurchaseDetailCRUD(clsPM, sqlConn, trans, sBatchCode, 2);
                            sRetDet = clsPur.PurchaseDetailCRUD(clsPM, sqlConn, trans, sBatchCode, 0);
                        }
                        else
                            sRetDet = clsPur.PurchaseDetailCRUD(clsPM, sqlConn, trans, sBatchCode, iAction);

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

                        if (clsVchType.ParentID != 1005)
                        {
                            if (iAction == 0 || iAction == 1)
                            {
                                if (Comm.ToInt32(cboPayment.SelectedIndex) == 0)
                                {
                                    if (clsVchTypeFeatures.BLNPOSTCASHENTRY == true)
                                    {
                                        Comm.VoucherInsert(Comm.ToInt32(cboCostCentre.SelectedValue.ToString()), vchtypeID, dtpInvDate.Value, DateAndTime.Now.ToLocalTime(), 2, 2, 0, Comm.ToInt32(clsPM.clsJsonPMInfo_.InvId), clsPM.clsJsonPMInfo_.InvNo, txtNarration.Text.ToString(), Comm.ToDouble(lblBillAmount.Text.ToString()), 0, Comm.ToInt32(cboAgent.SelectedValue.ToString()), Comm.ToInt32(cboSalesStaff.SelectedValue.ToString()), 0, 0, false, txtNarration.Text.ToString());
                                        Comm.VoucherInsert(Comm.ToInt32(cboCostCentre.SelectedValue.ToString()), vchtypeID, dtpInvDate.Value, DateAndTime.Now.ToLocalTime(), Comm.ToInt32(lblLID.Text.ToString()), 0, Comm.ToInt32(lblLID.Text.ToString()), Comm.ToInt32(clsPM.clsJsonPMInfo_.InvId), clsPM.clsJsonPMInfo_.InvNo, txtNarration.Text.ToString(), 0, Comm.ToDouble(lblBillAmount.Text.ToString()), Comm.ToInt32(cboAgent.SelectedValue.ToString()), Comm.ToInt32(cboSalesStaff.SelectedValue.ToString()), 0, 0, false, txtNarration.Text.ToString());

                                        Comm.VoucherInsert(Comm.ToInt32(cboCostCentre.SelectedValue.ToString()), vchtypeID, dtpInvDate.Value, DateAndTime.Now.ToLocalTime(), 3, 0, 3, Comm.ToInt32(clsPM.clsJsonPMInfo_.InvId), clsPM.clsJsonPMInfo_.InvNo, txtNarration.Text.ToString(), 0, Comm.ToDouble(lblBillAmount.Text.ToString()), Comm.ToInt32(cboAgent.SelectedValue.ToString()), Comm.ToInt32(cboSalesStaff.SelectedValue.ToString()), 0, 0, false, txtNarration.Text.ToString());
                                        Comm.VoucherInsert(Comm.ToInt32(cboCostCentre.SelectedValue.ToString()), vchtypeID, dtpInvDate.Value, DateAndTime.Now.ToLocalTime(), Comm.ToInt32(lblLID.Text.ToString()), Comm.ToInt32(lblLID.Text.ToString()), 0, Comm.ToInt32(clsPM.clsJsonPMInfo_.InvId), clsPM.clsJsonPMInfo_.InvNo, txtNarration.Text.ToString(), Comm.ToDouble(lblBillAmount.Text.ToString()), 0, Comm.ToInt32(cboAgent.SelectedValue.ToString()), Comm.ToInt32(cboSalesStaff.SelectedValue.ToString()), 0, 0, false, txtNarration.Text.ToString());
                                    }
                                    else
                                    {
                                        Comm.VoucherInsert(Comm.ToInt32(cboCostCentre.SelectedValue.ToString()), vchtypeID, dtpInvDate.Value, DateAndTime.Now.ToLocalTime(), 2, 2, 0, Comm.ToInt32(clsPM.clsJsonPMInfo_.InvId), clsPM.clsJsonPMInfo_.InvNo, txtNarration.Text.ToString(), Comm.ToDouble(lblBillAmount.Text.ToString()), 0, Comm.ToInt32(cboAgent.SelectedValue.ToString()), Comm.ToInt32(cboSalesStaff.SelectedValue.ToString()), 0, 0, false, txtNarration.Text.ToString());
                                        Comm.VoucherInsert(Comm.ToInt32(cboCostCentre.SelectedValue.ToString()), vchtypeID, dtpInvDate.Value, DateAndTime.Now.ToLocalTime(), 3, 0, 3, Comm.ToInt32(clsPM.clsJsonPMInfo_.InvId), clsPM.clsJsonPMInfo_.InvNo, txtNarration.Text.ToString(), 0, Comm.ToDouble(lblBillAmount.Text.ToString()), Comm.ToInt32(cboAgent.SelectedValue.ToString()), Comm.ToInt32(cboSalesStaff.SelectedValue.ToString()), 0, 0, false, txtNarration.Text.ToString());
                                    }
                                }
                                if (Comm.ToInt32(cboPayment.SelectedIndex) == 1)
                                {
                                    Comm.VoucherInsert(Comm.ToInt32(cboCostCentre.SelectedValue.ToString()), vchtypeID, dtpInvDate.Value, DateAndTime.Now.ToLocalTime(), 2, 2, 0, Comm.ToInt32(clsPM.clsJsonPMInfo_.InvId), clsPM.clsJsonPMInfo_.InvNo, txtNarration.Text.ToString(), Comm.ToDouble(lblBillAmount.Text.ToString()), 0, Comm.ToInt32(cboAgent.SelectedValue.ToString()), Comm.ToInt32(cboSalesStaff.SelectedValue.ToString()), 0, 0, false, txtNarration.Text.ToString());
                                    Comm.VoucherInsert(Comm.ToInt32(cboCostCentre.SelectedValue.ToString()), vchtypeID, dtpInvDate.Value, DateAndTime.Now.ToLocalTime(), Comm.ToInt32(lblLID.Text.ToString()), 0, Comm.ToInt32(lblLID.Text.ToString()), Comm.ToInt32(clsPM.clsJsonPMInfo_.InvId), clsPM.clsJsonPMInfo_.InvNo, txtNarration.Text.ToString(), 0, Comm.ToDouble(lblBillAmount.Text.ToString()), Comm.ToInt32(cboAgent.SelectedValue.ToString()), Comm.ToInt32(cboSalesStaff.SelectedValue.ToString()), 0, 0, false, txtNarration.Text.ToString());

                                    if (txtInstantReceipt.Text != "")
                                    {
                                        if (Comm.ToDouble(txtInstantReceipt.Text) > 0)
                                        {
                                            Comm.VoucherInsert(Comm.ToInt32(cboCostCentre.SelectedValue.ToString()), vchtypeID, dtpInvDate.Value, DateAndTime.Now.ToLocalTime(), 3, 0, 3, Comm.ToInt32(clsPM.clsJsonPMInfo_.InvId), clsPM.clsJsonPMInfo_.InvNo, txtNarration.Text.ToString(), 0, Comm.ToDouble(txtInstantReceipt.Text), Comm.ToInt32(cboAgent.SelectedValue.ToString()), Comm.ToInt32(cboSalesStaff.SelectedValue.ToString()), 0, 0, false, txtNarration.Text.ToString());
                                            Comm.VoucherInsert(Comm.ToInt32(cboCostCentre.SelectedValue.ToString()), vchtypeID, dtpInvDate.Value, DateAndTime.Now.ToLocalTime(), Comm.ToInt32(lblLID.Text.ToString()), Comm.ToInt32(lblLID.Text.ToString()), 0, Comm.ToInt32(clsPM.clsJsonPMInfo_.InvId), clsPM.clsJsonPMInfo_.InvNo, txtNarration.Text.ToString(), Comm.ToDouble(txtInstantReceipt.Text), 0, Comm.ToInt32(cboAgent.SelectedValue.ToString()), Comm.ToInt32(cboSalesStaff.SelectedValue.ToString()), 0, 0, false, txtNarration.Text.ToString());
                                        }
                                    }
                                }
                            }
                        }

                        if (iAction == 2)
                        {
                            string sRet = clsPur.PurchaseMasterCRUD(clsPM, sqlConn, trans, strJson, iAction);
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
                                Comm.MessageboxToasted("Purchase", "Voucher[" + vchno + "] Saved Successfully");
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

                                Comm.MessageboxToasted("Purchase", "Voucher[" + vchno + "] Saved Successfully");
                            }
                        }
                        else if (iAction == 2)
                        {
                            this.Close();
                            Comm.MessageboxToasted("Purchase", "Voucher[" + vchno + "] deleted successfully");
                            return;
                        }
                        else if (iAction == 3)
                        {
                            this.Close();
                            Comm.MessageboxToasted("Purchase", "Voucher[" + vchno + "] is archived");
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

        //Description : Agent Discount Asper Voucher Settings Value
        private void GetAgentDiscountAsperVoucherType()
        {
            DataTable dtAgentDisc = new DataTable();
            if (clsVchType.BillWiseDiscFillXtraDiscFromValue == 2) //Agent Discount
            {
                if (Comm.ToInt32(cboAgent.SelectedValue) >= 0)
                {
                    dtAgentDisc = Comm.fnGetData("SELECT ISNULL(AgentDiscount,0) as AgentDiscount FROM tblAgent WHERE AgentID = " + Comm.ToInt32(cboAgent.SelectedValue) + " AND TenantID = " + Global.gblTenantID + "").Tables[0];
                    if (dtAgentDisc.Rows.Count > 0)
                    {
                        txtDiscPerc.Text = Comm.FormatValue(Comm.ToDouble(dtAgentDisc.Rows[0][0].ToString()), true, "#.00");
                        txtDiscPerc.Tag = "2";//0-Default, 1-Agent wise, 2-supplier disc
                    }

                }
            }
            CalcTotal();
        }

        //Description : Customer Discount Asper Voucher Settings Value
        private void GetSupplierDiscountAsperVoucherType()
        {
            DataTable dtSuppDisc = new DataTable();
            if (clsVchType.BillWiseDiscFillXtraDiscFromValue == 3) //Customer Discount
            {
                if (Comm.ToInt32(cboAgent.SelectedValue) >= 0)
                {
                    dtSuppDisc = Comm.fnGetData("SELECT ISNULL(DiscPer,0) as DiscPer FROM tblLedger WHERE LID = " + Comm.ToInt32(lblLID.Text) + " AND TenantID = " + Global.gblTenantID + "").Tables[0];
                    if (dtSuppDisc.Rows.Count > 0)
                    { 
                        txtDiscPerc.Text = Comm.FormatValue(Comm.ToDouble(dtSuppDisc.Rows[0][0].ToString()), true, "#.00");
                        txtDiscPerc.Tag = "2";//0-Default, 1-Agent wise, 2-supplier disc
                    }
                }
            }
        }

        //Description : Clear the Form and Grid 
        private void ClearControls()
        {
            txtSupplier.TextChanged -= txtSupplier_TextChanged;
            txtSupplier.Text = "";
            txtSupplier.TextChanged += txtSupplier_TextChanged;
            txtMobile.Text = "";
            txtTaxRegn.Text = "";
            txtAddress1.Text = "";
            txtReferenceAutoNo.Clear();

            cboState.SelectedValue = 32;
            cboBType.SelectedIndex = -1;
            cboPayment.SelectedIndex = 0;

            txtInstantReceipt.Text = "";
            txtInstantReceipt.Enabled = false;
            txtInstantReceipt.BackColor = Color.Gainsboro;


            SetTransactionDefaults();
            //SetTransactionsthatVarying();
            //SetApplicationSettings();

            dgvPurchase.Rows.Clear();
            dgvPurchase.Refresh();
            iIDFromEditWindow = 0;
            //AddColumnsToGrid();
            dgvPurchase.Rows.Add();
            dgvPurchase.CurrentCell = dgvPurchase[1, 0];

            dSupplierID = 0;

            txtGrossAmt.Text = "";
            lblQtyTotal.Text = "";
            lblFreeTotal.Text = "";
            txtItemDiscTot.Text = "0";
            txtGrossAfterItmDisc.Text = "0";
            txtDiscPerc.TextChanged -= txtDiscPerc_TextChanged;
            txtDiscPerc.Text = "0";
            txtDiscPerc.TextChanged += txtDiscPerc_TextChanged;
            txtDiscAmt.TextChanged -= txtDiscAmt_TextChanged;
            txtDiscAmt.Text = "0";
            txtDiscAmt.TextChanged += txtDiscAmt_TextChanged;
            txtAmount.Text = "0";
            txtTaxable.Text = "0";
            txtNonTaxable.Text = "0";
            txtTaxAmt.Text = "0";
            txtCess.Text = "0";
            txtCompCess.Text = "0";
            txtQtyCess.Text = "0";
            txtOtherExp.TextChanged -= txtOtherExp_TextChanged;
            txtOtherExp.Text = "0";
            txtOtherExp.TextChanged += txtOtherExp_TextChanged;
            txtNetAmt.Text = "0";
            txtCoolie.Text = "0";
            txtCostFactor.TextChanged -= txtCostFactor_TextChanged;
            txtCostFactor.Text = "0";
            txtCostFactor.TextChanged += txtCostFactor_TextChanged;
            txtcashDisper.TextChanged -= txtcashDisper_TextChanged;
            txtcashDisper.Text = "0";
            txtcashDisper.TextChanged += txtcashDisper_TextChanged;
            txtCashDisc.TextChanged -= txtCashDisc_TextChanged;
            txtCashDisc.Text = "0";
            txtCashDisc.TextChanged += txtCashDisc_TextChanged;
            
            txtRoundOff.TextChanged -= txtRoundOff_TextChanged;
            switch (clsVchType.RoundOffMode)
            {
                case 0://none
                    {
                        txtRoundOff.Text = "0";
                        txtRoundOff.Enabled = false;

                        break;
                    }
                case 1://normal
                    {
                        txtRoundOff.Text = "0";
                        txtRoundOff.Enabled = false;

                        break;
                    }
                case 2://upward
                    {
                        txtRoundOff.Text = "0";
                        txtRoundOff.Enabled = false;

                        break;
                    }
                case 3://downward
                    {
                        txtRoundOff.Text = "0";
                        txtRoundOff.Enabled = false;

                        break;
                    }
                case 4://manual
                    {
                        txtRoundOff.Text = "0";
                        txtRoundOff.Enabled = true;

                        break;
                    }
                default:
                    {
                        break;
                    }
            }
            //txtRoundOff.Text = "0";
            txtRoundOff.TextChanged += txtRoundOff_TextChanged;
            
            txtNarration.Text = "";
            lblBillAmount.Text = "";
            
            txtGrossAftRateDiscount.Text = "";
            txtRateDiscTot.Text = "";
            txtSupplier.ReadOnly = false;

            FillAgent();
            FillTaxMode();

            SetTransactionDefaults();
            SetApplicationSettings();

            if (Comm.ConvertI32(clsVchType.TransactionNumberingValue) == 2) // Custom
                txtInvAutoNo.Clear();
            if (Comm.ConvertI32(clsVchType.ReferenceNumberingValue) == 2) // Custom
                txtReferencePrefix.Clear();

            dgvPurchase.Columns["cRateinclusive"].Visible = false;

            dgvPurchase.Columns["cSlNo"].Frozen = true;
            dgvPurchase.Columns["cSlNo"].ReadOnly = true;
            //dgvPurchase.Columns["cImgDel"].Frozen = true;
            dgvPurchase.Columns["cImgDel"].Visible = true;
            dgvPurchase.Columns["cImgDel"].Width = 40;

            txtInvAutoNo.Focus();
        }

        //Description : Show the Supplier as Selected ID
        private void ShowLedgerAsperID(int iTransID = 0)
        {
            string sQuery = "";
            DataTable dtShow = new DataTable();
            if (iTransID != 0)
            {
                sQuery = "Select LName,MobileNo,TaxNo,GSTType,StateID,Address FROM tblLedger WHERE LID = " + iTransID + "";
                dtShow = Comm.fnGetData(sQuery).Tables[0];
                if (dtShow.Rows.Count > 0)
                {
                    this.txtSupplier.TextChanged -= this.txtSupplier_TextChanged;
                    txtSupplier.Text = dtShow.Rows[0]["LName"].ToString();
                    this.txtSupplier.TextChanged += this.txtSupplier_TextChanged;
                    txtMobile.Text = dtShow.Rows[0]["MobileNo"].ToString();
                    txtTaxRegn.Text = dtShow.Rows[0]["TaxNo"].ToString();
                    cboState.SelectedValue = dtShow.Rows[0]["StateID"].ToString();
                    cboBType.SelectedItem = dtShow.Rows[0]["GSTType"].ToString();
                    txtAddress1.Text = dtShow.Rows[0]["Address"].ToString();
                    
                    lblLID.Text = iTransID.ToString();
                }
            }
        }

        //Description : Function Polymorphism of SetTag
        private void SetTag(int iCellIndex, int iRowIndex, string sTag, string sConvertType = "")
        {
            if (sConvertType.ToUpper() == "PERC_FLOAT")
                dgvPurchase.Rows[iRowIndex].Cells[iCellIndex].Tag = Comm.ToDecimal(sTag).ToString("#.00");
            else
                dgvPurchase.Rows[iRowIndex].Cells[iCellIndex].Tag = sTag;
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
            DateTime sPurchaseDate = Convert.ToDateTime(dtpInvDate.Text);
            DateTime sExpiryDate = Convert.ToDateTime(dgvPurchase.CurrentRow.Cells[GetEnum(GridColIndexes.CExpiry)].Value);

            if (iShelfLifeDays > 0)
            {
                int iDaysCount = Comm.ToInt32((sExpiryDate - sPurchaseDate).TotalDays);
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

                                //SetValue(GetEnum(GridColIndexes.CItemName), dtItemPublic.Rows[0]["ItemName"].ToString());
                                //SetValue(GetEnum(GridColIndexes.CUnit), dtItemPublic.Rows[0]["Unit"].ToString());
                                //setTag(GetEnum(GridColIndexes.CUnit), dtItemPublic.Rows[0]["UNITID"].ToString());
                                //SetValue(GetEnum(GridColIndexes.cItemID), dtItemPublic.Rows[0]["ItemID"].ToString());
                                //setTag(GetEnum(GridColIndexes.CItemName), dtItemPublic.Rows[0]["ItemID"].ToString());

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
                                SetTag(GetEnum(GridColIndexes.cSRate1Per), dgvPurchase.CurrentRow.Index, dtItemPublic.Rows[0]["SrateCalcMode"].ToString(), "");

                                SetTag(GetEnum(GridColIndexes.cCoolie), dgvPurchase.CurrentRow.Index, dtItemPublic.Rows[0]["Coolie"].ToString(), "");
                                SetValue(GetEnum(GridColIndexes.cCoolie), dtItemPublic.Rows[0]["Coolie"].ToString(), "CURR_FLOAT");

                                SetTag(GetEnum(GridColIndexes.cAgentCommPer), dgvPurchase.CurrentRow.Index, dtItemPublic.Rows[0]["agentCommPer"].ToString(), "");
                                SetValue(GetEnum(GridColIndexes.cAgentCommPer), dtItemPublic.Rows[0]["agentCommPer"].ToString(), "CURR_FLOAT");

                                if (clsVchType.DefaultTaxModeValue == 3) //GST
                                {
                                    //SetValue(GetEnum(GridColIndexes.cCGST), dtItemPublic.Rows[0]["CGSTTaxPer"].ToString(), "PERC_FLOAT");
                                    //SetValue(GetEnum(GridColIndexes.cSGST), dtItemPublic.Rows[0]["SGSTTaxPer"].ToString(), "PERC_FLOAT");
                                    //SetValue(GetEnum(GridColIndexes.cIGST), dtItemPublic.Rows[0]["IGSTTaxPer"].ToString(), "PERC_FLOAT");

                                    SetTag(GetEnum(GridColIndexes.cCGST), dgvPurchase.CurrentRow.Index, dtItemPublic.Rows[0]["CGSTTaxPer"].ToString(), "PERC_FLOAT");
                                    SetTag(GetEnum(GridColIndexes.cSGST), dgvPurchase.CurrentRow.Index, dtItemPublic.Rows[0]["SGSTTaxPer"].ToString(), "PERC_FLOAT");
                                    SetTag(GetEnum(GridColIndexes.cIGST), dgvPurchase.CurrentRow.Index, dtItemPublic.Rows[0]["IGSTTaxPer"].ToString(), "PERC_FLOAT");
                                    //SetTag(GetEnum(GridColIndexes.ctaxPer), dgvPurchase.CurrentRow.Index, dtItemPublic.Rows[0]["IGSTTaxPer"].ToString(), "PERC_FLOAT");
                                    SetValue(GetEnum(GridColIndexes.ctaxPer), dtItemPublic.Rows[0]["IGSTTaxPer"].ToString(), "PERC_FLOAT");
                                }
                                else
                                {
                                    //SetValue(GetEnum(GridColIndexes.cCGST), "0", "0");
                                    //SetValue(GetEnum(GridColIndexes.cSGST), "0", "0");
                                    //SetValue(GetEnum(GridColIndexes.cIGST), "0", "0");
                                    //SetValue(GetEnum(GridColIndexes.ctaxPer), "0", "0");
                                    SetTag(GetEnum(GridColIndexes.cCGST), dgvPurchase.CurrentRow.Index, "0", "PERC_FLOAT");
                                    SetTag(GetEnum(GridColIndexes.cSGST), dgvPurchase.CurrentRow.Index, "0", "PERC_FLOAT");
                                    SetTag(GetEnum(GridColIndexes.cIGST), dgvPurchase.CurrentRow.Index, "0", "PERC_FLOAT");
                                    //SetTag(GetEnum(GridColIndexes.ctaxPer), dgvPurchase.CurrentRow.Index, "0", "PERC_FLOAT");
                                    SetValue(GetEnum(GridColIndexes.ctaxPer), "0", "0");
                                }

                                if (Comm.ToInt32(dtItemPublic.Rows[0]["PRateInclusive"].ToString()) == 1)
                                    dgvPurchase.Rows[dgvPurchase.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cRateinclusive)].Value = true;
                                else
                                    dgvPurchase.Rows[dgvPurchase.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cRateinclusive)].Value = false;

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
                                SetValue(GetEnum(GridColIndexes.cDiscPer), dItmWiseDisccount.ToString(), "");

                                dtCurrExp = DateTime.Today;
                                if (Comm.ToDouble(dtItemPublic.Rows[0]["DefaultExpInDays"].ToString()) > 0)
                                    dtCurrExp = dtCurrExp.AddDays(Comm.ToDouble(dtItemPublic.Rows[0]["DefaultExpInDays"].ToString()));
                                else
                                    dtCurrExp = dtCurrExp.AddYears(8);

                                SetValue(GetEnum(GridColIndexes.CExpiry), dtCurrExp.ToString("dd-MMM-yyyy").ToString(), "DATE");
                                SetTag(GetEnum(GridColIndexes.CExpiry), dgvPurchase.CurrentRow.Index, dtItemPublic.Rows[0]["BlnExpiryItem"].ToString());
                                if (Comm.ToInt32(dtItemPublic.Rows[0]["BlnExpiryItem"].ToString()) == 1)
                                {
                                    dgvPurchase.Rows[dgvPurchase.CurrentRow.Index].Cells[GetEnum(GridColIndexes.CExpiry)].ReadOnly = false;
                                }
                                else
                                {
                                    dgvPurchase.Rows[dgvPurchase.CurrentRow.Index].Cells[GetEnum(GridColIndexes.CExpiry)].ReadOnly = true;
                                }

                                iBatchmode = Comm.ToInt32(dtItemPublic.Rows[0]["batchMode"].ToString());
                                SetValue(GetEnum(GridColIndexes.cBatchMode), iBatchmode.ToString());
                                iShelfLifeDays = Comm.ToInt32(dtItemPublic.Rows[0]["Shelflife"].ToString());

                                if (iBatchmode == 1)
                                {
                                    if (dgvPurchase.Columns[GetEnum(GridColIndexes.cBarCode)].Visible == true)
                                        dgvPurchase.CurrentCell = dgvPurchase.Rows[dgvPurchase.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cBarCode)];
                                    else if (dgvPurchase.Columns[GetEnum(GridColIndexes.cQty)].Visible == true)
                                        dgvPurchase.CurrentCell = dgvPurchase.Rows[dgvPurchase.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cQty)];

                                    dgvPurchase.EndEdit();

                                    dgvPurchase.Rows[dgvPurchase.CurrentRow.Index].Cells[GetEnum(GridColIndexes.CItemCode)].Value = dtItemPublic.Rows[0]["ItemCode"].ToString();
                                    
                                    //dgvPurchase.BeginEdit(true);
                                }
                                else if (iBatchmode == 2)
                                {
                                    dgvPurchase.Rows[dgvPurchase.CurrentRow.Index].Cells[GetEnum(GridColIndexes.CItemCode)].Value = dtItemPublic.Rows[0]["ItemCode"].ToString();
                                    //FillGridAsperStockID(Comm.ToInt32(dtItemPublic.Rows[0]["StockID"].ToString()));

                                    if (dtItemPublic.Rows[0]["StockID"].ToString() != "")
                                        FillGridAsperStockID(Comm.ToInt32(dtItemPublic.Rows[0]["StockID"].ToString()));

                                    //After taking values from stock the batchcode, expiry fields are to be reset for auto batch code
                                    dgvPurchase.Rows[dgvPurchase.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cBarCode)].Value = "<Auto Barcode>";
                                    dgvPurchase.Rows[dgvPurchase.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cBarCode)].Tag = 0;

                                    dtCurrExp = DateTime.Today;
                                    if (Comm.ToDouble(dtItemPublic.Rows[0]["DefaultExpInDays"].ToString()) > 0)
                                        dtCurrExp = dtCurrExp.AddDays(Comm.ToDouble(dtItemPublic.Rows[0]["DefaultExpInDays"].ToString()));
                                    else
                                        dtCurrExp = dtCurrExp.AddYears(8);

                                    SetValue(GetEnum(GridColIndexes.CExpiry), dtCurrExp.ToString("dd-MMM-yyyy").ToString(), "DATE");
                                    SetTag(GetEnum(GridColIndexes.CExpiry), dgvPurchase.CurrentRow.Index, dtItemPublic.Rows[0]["BlnExpiryItem"].ToString());
                                    if (Comm.ToInt32(dtItemPublic.Rows[0]["BlnExpiryItem"].ToString()) == 1)
                                    {
                                        dgvPurchase.Rows[dgvPurchase.CurrentRow.Index].Cells[GetEnum(GridColIndexes.CExpiry)].ReadOnly = false;
                                    }
                                    else
                                    {
                                        dgvPurchase.Rows[dgvPurchase.CurrentRow.Index].Cells[GetEnum(GridColIndexes.CExpiry)].ReadOnly = true;
                                    }

                                    if (dgvPurchase.Rows[dgvPurchase.CurrentRow.Index].Cells[GetEnum(GridColIndexes.CExpiry)].Visible == true)
                                        dgvPurchase.CurrentCell = dgvPurchase.Rows[dgvPurchase.CurrentRow.Index].Cells[GetEnum(GridColIndexes.CExpiry)];
                                    else if (dgvPurchase.Rows[dgvPurchase.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cMRP)].Visible == true)
                                        dgvPurchase.CurrentCell = dgvPurchase.Rows[dgvPurchase.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cMRP)];
                                    else if (dgvPurchase.Rows[dgvPurchase.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cPrate)].Visible == true)
                                        dgvPurchase.CurrentCell = dgvPurchase.Rows[dgvPurchase.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cPrate)];
                                    else
                                        dgvPurchase.CurrentCell = dgvPurchase.Rows[dgvPurchase.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cQty)];
                                    
                                    dgvPurchase.Focus();
                                    CalcTotal();
                                }
                                else if (iBatchmode == 0 || iBatchmode == 3)
                                {
                                    if (dtItemPublic.Rows[0]["BatchUnique"].ToString() != "")
                                    {
                                        dgvPurchase.Rows[dgvPurchase.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cBarCode)].Value = dtItemPublic.Rows[0]["BatchCode"].ToString();
                                        dgvPurchase.Rows[dgvPurchase.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cBarCode)].Tag = dtItemPublic.Rows[0]["BatchUnique"].ToString();
                                    }
                                    else
                                    {
                                        dgvPurchase.Rows[dgvPurchase.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cBarCode)].Value = dtItemPublic.Rows[0]["BatchCode"].ToString();
                                        dgvPurchase.Rows[dgvPurchase.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cBarCode)].Tag = dtItemPublic.Rows[0]["BatchCode"].ToString();
                                    }

                                    dgvPurchase.Rows[dgvPurchase.CurrentRow.Index].Cells[GetEnum(GridColIndexes.CItemCode)].Value = dtItemPublic.Rows[0]["ItemCode"].ToString();
                                    
                                    if(dtItemPublic.Rows[0]["StockID"].ToString() != "")
                                        FillGridAsperStockID(Comm.ToInt32(dtItemPublic.Rows[0]["StockID"].ToString()));

                                    dgvPurchase.EndEdit();

                                    if (dgvPurchase.Rows[dgvPurchase.CurrentRow.Index].Cells[GetEnum(GridColIndexes.CExpiry)].Visible == true)
                                        dgvPurchase.CurrentCell = dgvPurchase.Rows[dgvPurchase.CurrentRow.Index].Cells[GetEnum(GridColIndexes.CExpiry)];
                                    else
                                        dgvPurchase.CurrentCell = dgvPurchase.Rows[dgvPurchase.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cQty)];

                                    dgvPurchase.Focus();
                                    CalcTotal();
                                }
                                SetValue(GetEnum(GridColIndexes.CItemCode), dtItemPublic.Rows[0]["ItemCode"].ToString());

                                if (dgvPurchase.Rows.Count - 1 == dgvPurchase.CurrentRow.Index)
                                    dgvPurchase.Rows.Add();

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

        // Created By : Dipu 
        // Created On : 21-Feb-2022
        // Description: To Calculate Tax When TaxMode Combo Box Change
        private void TaxCalculate()
        {
            try
            {
                if (cboTaxMode.SelectedValue == null)
                    if (cboTaxMode.Items.Count > 0)
                        cboTaxMode.SelectedIndex = 0;

                if (dgvPurchase.Rows.Count > 1)
                {
                    if (cboTaxMode.SelectedValue.ToString() == "3") //GST
                    {
                        dgvPurchase.Columns["cCGST"].Visible = true;
                        dgvPurchase.Columns["cSGST"].Visible = true;
                        dgvPurchase.Columns["cIGST"].Visible = true;
                        dgvPurchase.Columns["ctaxPer"].Visible = true;
                        dgvPurchase.Columns["ctax"].Visible = true;
                        dgvPurchase.Columns["ctaxable"].Visible = true;
                        dgvPurchase.Columns["cCRateWithTax"].Visible = true;

                        tblpTaxAmt.Visible = true;
                        tblpTaxable.Visible = true;
                    }
                    else if (cboTaxMode.SelectedValue.ToString() == "2") //GST
                    {
                        dgvPurchase.Columns["cCGST"].Visible = false;
                        dgvPurchase.Columns["cSGST"].Visible = false;
                        dgvPurchase.Columns["cIGST"].Visible = false;
                        dgvPurchase.Columns["ctaxPer"].Visible = false;
                        dgvPurchase.Columns["ctax"].Visible = false;
                        dgvPurchase.Columns["ctaxable"].Visible = false;
                        dgvPurchase.Columns["cCRateWithTax"].Visible = false;

                        tblpTaxAmt.Visible = false;
                        tblpTaxable.Visible = false;
                    }

                    for (int k = 0; k < dgvPurchase.Rows.Count; k++)
                    {
                        if (dgvPurchase.Rows[k].Cells[GetEnum(GridColIndexes.CItemCode)].Value != null)
                        {

                            GetItmMstinfo.ItemID = Comm.ToDecimal(dgvPurchase.Rows[k].Cells[GetEnum(GridColIndexes.cItemID)].Value.ToString());
                            GetItmMstinfo.TenantID = Global.gblTenantID;

                            dtItemPublic = clsItmMst.GetItemMaster(GetItmMstinfo);
                            if (dtItemPublic.Rows.Count > 0)
                            {
                                if (cboTaxMode.SelectedValue.ToString() == "3") //GST
                                {
                                    dgvPurchase.Rows[k].Cells[GetEnum(GridColIndexes.cCGST)].Tag = Comm.ToDecimal(dtItemPublic.Rows[0]["CGSTTaxPer"].ToString()).ToString("#.00");
                                    dgvPurchase.Rows[k].Cells[GetEnum(GridColIndexes.cSGST)].Tag = Comm.ToDecimal(dtItemPublic.Rows[0]["SGSTTaxPer"].ToString()).ToString("#.00");
                                    dgvPurchase.Rows[k].Cells[GetEnum(GridColIndexes.cIGST)].Tag = Comm.ToDecimal(dtItemPublic.Rows[0]["IGSTTaxPer"].ToString()).ToString("#.00");
                                    dgvPurchase.Rows[k].Cells[GetEnum(GridColIndexes.ctaxPer)].Tag = Comm.ToDecimal(dtItemPublic.Rows[0]["IGSTTaxPer"].ToString()).ToString("#.00");
                                    dgvPurchase.Rows[k].Cells[GetEnum(GridColIndexes.ctaxPer)].Value = Comm.ToDecimal(dtItemPublic.Rows[0]["IGSTTaxPer"].ToString()).ToString("#.00");
                                }
                                else if (cboTaxMode.SelectedValue.ToString() == "1") //none
                                {
                                    if (dgvPurchase.Columns.Count > 0)
                                    {
                                        dgvPurchase.Rows[k].Cells[GetEnum(GridColIndexes.cCGST)].Tag = "0";
                                        dgvPurchase.Rows[k].Cells[GetEnum(GridColIndexes.cSGST)].Tag = "0";
                                        dgvPurchase.Rows[k].Cells[GetEnum(GridColIndexes.cIGST)].Tag = "0";
                                        dgvPurchase.Rows[k].Cells[GetEnum(GridColIndexes.ctaxPer)].Tag = "0";
                                        dgvPurchase.Rows[k].Cells[GetEnum(GridColIndexes.ctaxPer)].Value = "0";
                                    }
                                }
                                else if (cboTaxMode.SelectedValue.ToString() == "2") //GST
                                {
                                    if (dgvPurchase.Columns.Count > 0)
                                    {
                                        dgvPurchase.Rows[k].Cells[GetEnum(GridColIndexes.cCGST)].Tag = "0";
                                        dgvPurchase.Rows[k].Cells[GetEnum(GridColIndexes.cSGST)].Tag = "0";
                                        dgvPurchase.Rows[k].Cells[GetEnum(GridColIndexes.cIGST)].Tag = "0";
                                        dgvPurchase.Rows[k].Cells[GetEnum(GridColIndexes.ctaxPer)].Tag = Comm.ToDecimal(dtItemPublic.Rows[0]["VAT"].ToString()).ToString("#.00");
                                        dgvPurchase.Rows[k].Cells[GetEnum(GridColIndexes.ctaxPer)].Value = Comm.ToDecimal(dtItemPublic.Rows[0]["VAT"].ToString()).ToString("#.00");

                                        SetTag(GetEnum(GridColIndexes.cCGST), dgvPurchase.CurrentRow.Index, "0", "0");
                                        SetTag(GetEnum(GridColIndexes.cSGST), dgvPurchase.CurrentRow.Index, "0", "0");
                                        SetTag(GetEnum(GridColIndexes.cIGST), dgvPurchase.CurrentRow.Index, "0", "0");
                                        SetTag(GetEnum(GridColIndexes.ctaxPer), dgvPurchase.CurrentRow.Index, Comm.ToDecimal(dtItemPublic.Rows[0]["VAT"].ToString()).ToString("#.00"), "0");

                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (cboTaxMode.SelectedValue.ToString() == "3") //GST
                    {
                        if (dgvPurchase.Columns.Count > 0)
                        {
                            dgvPurchase.Columns["cCGST"].Visible = true;
                            dgvPurchase.Columns["cSGST"].Visible = true;
                            dgvPurchase.Columns["cIGST"].Visible = true;
                            dgvPurchase.Columns["ctaxPer"].Visible = true;
                            dgvPurchase.Columns["ctax"].Visible = true;
                            dgvPurchase.Columns["ctaxable"].Visible = true;
                            dgvPurchase.Columns["cCRateWithTax"].Visible = true;

                            tblpTaxAmt.Visible = true;
                            tblpTaxable.Visible = true;
                        }
                    }
                    else
                    {
                        if (dgvPurchase.Columns.Count > 0)
                        {
                            SetTag(GetEnum(GridColIndexes.cCGST), dgvPurchase.CurrentRow.Index, "0", "0");
                            SetTag(GetEnum(GridColIndexes.cSGST), dgvPurchase.CurrentRow.Index, "0", "0");
                            SetTag(GetEnum(GridColIndexes.cIGST), dgvPurchase.CurrentRow.Index, "0", "0");
                            SetTag(GetEnum(GridColIndexes.ctaxPer), dgvPurchase.CurrentRow.Index, "0", "0");

                            dgvPurchase.Columns["cCGST"].Visible = false;
                            dgvPurchase.Columns["cSGST"].Visible = false;
                            dgvPurchase.Columns["cIGST"].Visible = false;
                            dgvPurchase.Columns["ctaxPer"].Visible = false;
                            dgvPurchase.Columns["ctax"].Visible = false;
                            dgvPurchase.Columns["ctaxable"].Visible = false;
                            dgvPurchase.Columns["cCRateWithTax"].Visible = false;
                        }
                        tblpTaxAmt.Visible = false;
                        tblpTaxable.Visible = false;
                    }
                    
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        //Description : Row Delete when Press Delete or Delete icon
        private void RowDelete()
        {
            int rowIndex = dgvPurchase.CurrentCell.RowIndex;
            dgvPurchase.Rows.RemoveAt(rowIndex);
            decimal dinvid = GetPurchaseIfo.InvId;
        }

        //Description : Initializing the Columns in The Grid
        private void AddColumnsToGrid()
        {
            this.dgvPurchase.Columns.Clear();

            this.dgvPurchase.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSlNo", HeaderText = "Sl.No", Width = 50, ReadOnly = true }); //1
            this.dgvPurchase.Columns.Add(new DataGridViewTextBoxColumn() { Name = "CItemCode", HeaderText = "Item Code", Width = 130 }); //1
            this.dgvPurchase.Columns.Add(new DataGridViewTextBoxColumn() { Name = "CItemName", HeaderText = "Item Name", Width = 200, ReadOnly = true }); //2
            this.dgvPurchase.Columns.Add(new DataGridViewTextBoxColumn() { Name = "CUnit", ReadOnly = true, Visible = true, HeaderText = "Unit", Width = 50 }); //3
            //Commented and added By Dipu on 23-Feb-2022 ------------- >>
            this.dgvPurchase.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cBarCode", HeaderText = "Batch Code", Width = 200 }); //4
            this.dgvPurchase.Columns.Add(new DataGridViewTextBoxColumn() { Name = "CExpiry", HeaderText = "Expiry Date", Width = 120 }); //5
            
            if (clsVchTypeFeatures.BLNEDITMRPRATE == true)
            {
                if (AppSettings.IsActiveMRP == true)
                    this.dgvPurchase.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cMRP", HeaderText = "" + AppSettings.MRPName + "", ReadOnly = false, Visible = true, Width = 80 }); //6
                else
                    this.dgvPurchase.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cMRP", HeaderText = "" + AppSettings.MRPName + "", ReadOnly = false, Visible = false, Width = 80 }); //6
            }
            else
            {
                if (AppSettings.IsActiveMRP == true)
                    this.dgvPurchase.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cMRP", HeaderText = "" + AppSettings.MRPName + "", ReadOnly = true, Visible = true, Width = 80 }); //6
                else
                    this.dgvPurchase.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cMRP", HeaderText = "" + AppSettings.MRPName + "", ReadOnly = true, Visible = false, Width = 80 }); //6
            }

            this.dgvPurchase.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cPrate", HeaderText = "PRate", Width = 80 }); //7

            if (AppSettings.TaxMode == 2) //GST
                this.dgvPurchase.Columns.Add(new DataGridViewCheckBoxColumn() { Name = "cRateinclusive", HeaderText = "Rate Inc.", Width = 80, ReadOnly = true }); //20
            else
                this.dgvPurchase.Columns.Add(new DataGridViewCheckBoxColumn() { Name = "cRateinclusive", HeaderText = "Rate Inc.", Visible = false, Width = 80, ReadOnly = true }); //20

            this.dgvPurchase.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cQty", HeaderText = "Qty", Width = 80 }); //8
            if (clsVchTypeFeatures.BLNSHOWFREEQUANTITY == true)
                this.dgvPurchase.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cFree", HeaderText = "Free", Visible = true, Width = 80 }); //9
            else
                this.dgvPurchase.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cFree", HeaderText = "Free", Visible = false, Width = 80 }); //9

            if (clsVchTypeFeatures.blneditsalerate == true)
            {
                if (clsVchTypeFeatures.BLNRECALCULATESalesRatesOnPercentage == true)
                    this.dgvPurchase.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate1Per", HeaderText = "" + AppSettings.SRate1Name + " %", ReadOnly = false, Width = 80 }); //10
                else
                    this.dgvPurchase.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate1Per", HeaderText = "" + AppSettings.SRate1Name + " %", ReadOnly = true, Width = 80 }); //10

                this.dgvPurchase.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate1", HeaderText = "" + AppSettings.SRate1Name + "", ReadOnly = false, Width = 80 }); //11
                if (AppSettings.IsActiveSRate2 == true)
                {
                    if (clsVchTypeFeatures.BLNRECALCULATESalesRatesOnPercentage == true)
                        this.dgvPurchase.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate2Per", HeaderText = "" + AppSettings.SRate2Name + " %", ReadOnly = false, Width = 80 }); //10
                    else
                        this.dgvPurchase.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate2Per", HeaderText = "" + AppSettings.SRate2Name + " %", ReadOnly = true, Width = 80 }); //10

                    //this.dgvPurchase.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate2Per", HeaderText = "" + AppSettings.SRate2Name + " %", ReadOnly = false, Visible=true, Width = 80 }); //12
                    this.dgvPurchase.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate2", HeaderText = "" + AppSettings.SRate2Name + "", ReadOnly = false, Visible=true, Width = 80 }); //13
                }
                else
                {
                    this.dgvPurchase.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate2Per", HeaderText = "" + AppSettings.SRate2Name + " %", ReadOnly = false, Visible=false, Width = 80 }); //12
                    this.dgvPurchase.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate2", HeaderText = "" + AppSettings.SRate2Name + "", ReadOnly = false, Visible=false, Width = 80 }); //13
                }

                if (AppSettings.IsActiveSRate3 == true)
                {
                    if (clsVchTypeFeatures.BLNRECALCULATESalesRatesOnPercentage == true)
                        this.dgvPurchase.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate3Per", HeaderText = "" + AppSettings.SRate1Name + " %", ReadOnly = false, Width = 80 }); //10
                    else
                        this.dgvPurchase.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate3Per", HeaderText = "" + AppSettings.SRate1Name + " %", ReadOnly = true, Width = 80 }); //10

                    //this.dgvPurchase.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate3Per", HeaderText = "" + AppSettings.SRate3Name + " %", ReadOnly = false, Visible = true, Width = 80 }); //14
                    this.dgvPurchase.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate3", HeaderText = AppSettings.SRate3Name, ReadOnly = false, Visible = true, Width = 80 }); //15
                }
                else
                {
                    this.dgvPurchase.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate3Per", HeaderText = "" + AppSettings.SRate3Name + " %", ReadOnly = false, Visible = false, Width = 80 }); //14
                    this.dgvPurchase.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate3", HeaderText = AppSettings.SRate3Name, ReadOnly = false, Visible = false, Width = 80 }); //15
                }

                if (AppSettings.IsActiveSRate4 == true)
                {
                    if (clsVchTypeFeatures.BLNRECALCULATESalesRatesOnPercentage == true)
                        this.dgvPurchase.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate4Per", HeaderText = "" + AppSettings.SRate1Name + " %", ReadOnly = false, Width = 80 }); //10
                    else
                        this.dgvPurchase.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate4Per", HeaderText = "" + AppSettings.SRate1Name + " %", ReadOnly = true, Width = 80 }); //10

                    //this.dgvPurchase.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate4Per", HeaderText = "" + AppSettings.SRate4Name + " %", ReadOnly = false, Visible = true, Width = 80 }); //16
                    this.dgvPurchase.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate4", HeaderText = AppSettings.SRate4Name, ReadOnly = false, Visible = true, Width = 80 }); //17
                }
                else
                {
                    this.dgvPurchase.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate4Per", HeaderText = "" + AppSettings.SRate4Name + " %", ReadOnly = false, Visible = false, Width = 80 }); //16
                    this.dgvPurchase.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate4", HeaderText = AppSettings.SRate4Name, ReadOnly = false, Visible = false, Width = 80 }); //17
                }

                if (AppSettings.IsActiveSRate5 == true)
                {
                    if (clsVchTypeFeatures.BLNRECALCULATESalesRatesOnPercentage == true)
                        this.dgvPurchase.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate5Per", HeaderText = "" + AppSettings.SRate1Name + " %", ReadOnly = false, Width = 80 }); //10
                    else
                        this.dgvPurchase.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate5Per", HeaderText = "" + AppSettings.SRate1Name + " %", ReadOnly = true, Width = 80 }); //10

                    //this.dgvPurchase.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate5Per", HeaderText = "" + AppSettings.SRate5Name + " %", ReadOnly = false, Visible = true, Width = 80 }); //18
                    this.dgvPurchase.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate5", HeaderText = AppSettings.SRate5Name, ReadOnly = false, Visible = true, Width = 80 }); //19
                }
                else
                {
                    this.dgvPurchase.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate5Per", HeaderText = "" + AppSettings.SRate5Name + " %", ReadOnly = false, Visible = false, Width = 80 }); //18
                    this.dgvPurchase.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate5", HeaderText = AppSettings.SRate5Name, ReadOnly = false, Visible = false, Width = 80 }); //19
                }
            }
            else
            {
                this.dgvPurchase.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate1Per", HeaderText = "" + AppSettings.SRate1Name + " %", ReadOnly = true, Width = 80 }); //10
                this.dgvPurchase.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate1", HeaderText = "" + AppSettings.SRate1Name + "", ReadOnly = true, Width = 80 }); //11
                if (AppSettings.IsActiveSRate2 == true)
                {
                    this.dgvPurchase.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate2Per", HeaderText = "" + AppSettings.SRate2Name + " %", ReadOnly = true, Visible = true, Width = 80 }); //12
                    this.dgvPurchase.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate2", HeaderText = "" + AppSettings.SRate2Name + "", ReadOnly = true, Visible = true, Width = 80 }); //13
                }
                else
                {
                    this.dgvPurchase.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate2Per", HeaderText = "" + AppSettings.SRate2Name + " %", ReadOnly = true, Visible = false, Width = 80 }); //12
                    this.dgvPurchase.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate2", HeaderText = "" + AppSettings.SRate2Name + "", ReadOnly = true, Visible = false, Width = 80 }); //13
                }

                if (AppSettings.IsActiveSRate3 == true)
                {
                    this.dgvPurchase.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate3Per", HeaderText = "" + AppSettings.SRate3Name + " %", ReadOnly = true, Visible = true, Width = 80 }); //14
                    this.dgvPurchase.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate3", HeaderText = AppSettings.SRate3Name, ReadOnly = true, Visible = true, Width = 80 }); //15
                }
                else
                {
                    this.dgvPurchase.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate3Per", HeaderText = "" + AppSettings.SRate3Name + " %", ReadOnly = true, Visible = false, Width = 80 }); //14
                    this.dgvPurchase.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate3", HeaderText = AppSettings.SRate3Name, ReadOnly = true, Visible = false, Width = 80 }); //15
                }

                if (AppSettings.IsActiveSRate4 == true)
                {
                    this.dgvPurchase.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate4Per", HeaderText = "" + AppSettings.SRate4Name + " %", ReadOnly = true, Visible = true, Width = 80 }); //16
                    this.dgvPurchase.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate4", HeaderText = AppSettings.SRate4Name, ReadOnly = true, Visible = true, Width = 80 }); //17
                }
                else
                {
                    this.dgvPurchase.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate4Per", HeaderText = "" + AppSettings.SRate4Name + " %", ReadOnly = true, Visible = false, Width = 80 }); //16
                    this.dgvPurchase.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate4", HeaderText = AppSettings.SRate4Name, ReadOnly = true, Visible = false, Width = 80 }); //17
                }

                if (AppSettings.IsActiveSRate5 == true)
                {
                    this.dgvPurchase.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate5Per", HeaderText = "" + AppSettings.SRate5Name + " %", ReadOnly = true, Visible = true, Width = 80 }); //18
                    this.dgvPurchase.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate5", HeaderText = AppSettings.SRate5Name, ReadOnly = true, Visible = true, Width = 80 }); //19
                }
                else
                {
                    this.dgvPurchase.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate5Per", HeaderText = "" + AppSettings.SRate5Name + " %", ReadOnly = true, Visible = false, Width = 80 }); //18
                    this.dgvPurchase.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate5", HeaderText = AppSettings.SRate5Name, ReadOnly = true, Visible = false, Width = 80 }); //19
                }
            }
            this.dgvPurchase.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cGrossAmt", HeaderText = "Gross Amt", Width = 80, ReadOnly = true }); //23

            if (clsVchType.ParentID != 1005)
            {
                if (clsVchType.blnItmWiseDiscPercentageandAmt == 1)
                {
                    this.dgvPurchase.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cDiscPer", HeaderText = "Discount %", ReadOnly = false, Width = 80 }); //24
                    this.dgvPurchase.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cDiscAmount", HeaderText = "Discount Amt", ReadOnly = false, Width = 80 }); //25
                }
                else
                {
                    if (clsVchType.blnItmWiseDiscPercentage == 1)
                        this.dgvPurchase.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cDiscPer", HeaderText = "Discount %", ReadOnly = false, Width = 80 }); //24
                    else
                        this.dgvPurchase.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cDiscPer", HeaderText = "Discount %", ReadOnly = true, Width = 80 }); //24

                    if (clsVchType.blnItmWiseDiscAmount == 1)
                        this.dgvPurchase.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cDiscAmount", HeaderText = "Discount Amt", ReadOnly = false, Width = 80 }); //25
                    else
                        this.dgvPurchase.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cDiscAmount", HeaderText = "Discount Amt", ReadOnly = true, Width = 80 }); //25
                } 
            }
            else
            {
                this.dgvPurchase.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cDiscPer", HeaderText = "Discount %", ReadOnly = true, Visible = false, Width = 80 }); //24
                this.dgvPurchase.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cDiscAmount", HeaderText = "Discount Amt", ReadOnly = true, Visible = false, Width = 80 }); //25
            }

            if (clsVchType.ParentID != 1005)
            {
                this.dgvPurchase.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cBillDisc", HeaderText = "Bill Discount", Width = 80, ReadOnly = true }); //26
            }
            else
            {
                this.dgvPurchase.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cBillDisc", HeaderText = "Bill Discount", Width = 80, ReadOnly = true, Visible = false }); //26
            }

            this.dgvPurchase.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cCrate", HeaderText = "CRate", Width = 80, ReadOnly = true }); //27

            if (AppSettings.TaxMode == 2) //GST
                this.dgvPurchase.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cCRateWithTax", HeaderText = "CRate With Tax", Width = 80, ReadOnly = true }); //28
            else
                this.dgvPurchase.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cCRateWithTax", HeaderText = "CRate With Tax", Visible=false, Width = 80, ReadOnly = true }); //28

            this.dgvPurchase.Columns.Add(new DataGridViewTextBoxColumn() { Name = "ctaxable", HeaderText = "Taxable", Width = 80, ReadOnly = true }); //29

            if (AppSettings.TaxMode == 2) //GST
            {
                if (clsVchTypeFeatures.blnEditTaxPer == true)
                    this.dgvPurchase.Columns.Add(new DataGridViewTextBoxColumn() { Name = "ctaxPer", HeaderText = "Tax %", ReadOnly = false, Width = 80 }); //30
                else
                    this.dgvPurchase.Columns.Add(new DataGridViewTextBoxColumn() { Name = "ctaxPer", HeaderText = "Tax %", ReadOnly = true, Width = 80 }); //30

                this.dgvPurchase.Columns.Add(new DataGridViewTextBoxColumn() { Name = "ctax", HeaderText = "Tax", Width = 80, ReadOnly = true }); //31
                this.dgvPurchase.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cIGST", HeaderText = "IGST", Width = 80, ReadOnly = true }); //32
                this.dgvPurchase.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSGST", HeaderText = "SGST", Width = 80, ReadOnly = true }); //33
                this.dgvPurchase.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cCGST", HeaderText = "CGST", Width = 80, ReadOnly = true }); //34
            }
            else
            {
                if (clsVchTypeFeatures.blnEditTaxPer == true)
                    this.dgvPurchase.Columns.Add(new DataGridViewTextBoxColumn() { Name = "ctaxPer", HeaderText = "Tax %", ReadOnly = false, Visible=false, Width = 80 }); //30
                else
                    this.dgvPurchase.Columns.Add(new DataGridViewTextBoxColumn() { Name = "ctaxPer", HeaderText = "Tax %", ReadOnly = true, Visible=false, Width = 80 }); //30

                this.dgvPurchase.Columns.Add(new DataGridViewTextBoxColumn() { Name = "ctax", HeaderText = "Tax", Visible = false, Width = 80, ReadOnly = true }); //31
                this.dgvPurchase.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cIGST", HeaderText = "IGST", Visible = false, Width = 80, ReadOnly = true }); //32
                this.dgvPurchase.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSGST", HeaderText = "SGST", Visible = false, Width = 80, ReadOnly = true }); //33
                this.dgvPurchase.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cCGST", HeaderText = "CGST", Visible = false, Width = 80, ReadOnly = true }); //34
            }

            
            this.dgvPurchase.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cNetAmount", HeaderText = "Net Amt", Width = 100, ReadOnly = true }); //35
            this.dgvPurchase.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cItemID", HeaderText = "ItemID", Visible = false, Width = 80, ReadOnly = true }); //36

            this.dgvPurchase.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cGrossValueAfterRateDiscount", HeaderText = "Gross Val", Visible = false, ReadOnly = true }); //37
            this.dgvPurchase.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cNonTaxable", HeaderText = "Non Taxable", Visible = false, ReadOnly = true }); //38
            this.dgvPurchase.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cCCessPer", HeaderText = "Cess %", Visible = false, ReadOnly = true }); //39
            this.dgvPurchase.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cCCompCessQty", HeaderText = "Comp Cess Qty", Visible = false, ReadOnly = true }); //40
            this.dgvPurchase.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cFloodCessPer", HeaderText = "Flood Cess %", Visible = false, ReadOnly = true }); //41
            this.dgvPurchase.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cFloodCessAmt", HeaderText = "Flood Cess Amt", Visible = false, ReadOnly = true }); //42
            this.dgvPurchase.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cStockMRP", HeaderText = "Stock MRP", Visible = false, ReadOnly = true }); //43
            this.dgvPurchase.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cAgentCommPer", HeaderText = "Agent Comm. %", Visible = false, ReadOnly = true }); //44
            this.dgvPurchase.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cCoolie", HeaderText = "Coolie", Visible = false, ReadOnly = true }); //45
            this.dgvPurchase.Columns.Add(new DataGridViewCheckBoxColumn() { Name = "cBlnOfferItem", HeaderText = "Offer Item", Visible = false, ReadOnly = true }); //46
            this.dgvPurchase.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cStrOfferDetails", HeaderText = "Offer Det.", Visible = false, ReadOnly = true }); //47
            this.dgvPurchase.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cBatchMode", HeaderText = "Batch Mode", Visible = false, ReadOnly = true }); //48
            this.dgvPurchase.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cID", HeaderText = "ID", Visible = false, ReadOnly = true });
            this.dgvPurchase.Columns.Add(new DataGridViewImageColumn() { Name = "cImgDel", HeaderText="", Image = DigiposZen.Properties.Resources.Delete_24_P4, Width=40, ReadOnly = true });
            this.dgvPurchase.Columns.Add(new DataGridViewImageColumn() { Name = "cBatchUnique", HeaderText="", Image = DigiposZen.Properties.Resources.Delete_24_P4, Width=40, Visible = false, ReadOnly = true });

            //Dipoos 21-03-2022
            //if (iIDFromEditWindow==0)
            //dgvPurchase.Rows.Add(2);
            //else

            dgvPurchase.Rows.Add(1);

            foreach (DataGridViewRow row in dgvPurchase.Rows)
            {
                dgvPurchase.Rows[row.Index].Cells[0].Value = string.Format("{0}  ", row.Index + 1).ToString();
            }

            foreach (DataGridViewColumn col in dgvPurchase.Columns)
            {
                col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
        }

        //Description : Initialize for Item Column Width Settings
        private void GridInitialize_dgvColWidth(bool bIsLoad = true)
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
                            if (dtGridSettings.Rows[k][3].ToString() == dgvPurchase.Columns[k].Name)
                            {
                                if (dgvPurchase.Columns[k].Name.ToUpper().Trim() == "cDiscAmount".ToUpper() || dgvPurchase.Columns[k].Name.ToUpper().Trim() == "cDiscPer".ToUpper() || dgvPurchase.Columns[k].Name.ToUpper().Trim() == "cBillDisc".ToUpper())
                                {
                                    if (clsVchType.ParentID == 1005)
                                    {
                                        dgvPurchase.Columns[k].Width = Convert.ToInt32(dtGridSettings.Rows[k][2].ToString());
                                        dgvPurchase.Columns[k].Visible = false;
                                    }
                                }
                                else if (dgvPurchase.Columns[k].Name.ToUpper().Trim() == "CFREE")
                                {
                                    if (clsVchTypeFeatures.BLNSHOWFREEQUANTITY == false || clsVchType.ParentID == 1005)
                                    {
                                        dgvPurchase.Columns[k].Width = Convert.ToInt32(dtGridSettings.Rows[k][2].ToString());
                                        dgvPurchase.Columns[k].Visible = false;
                                    }
                                }
                                else if (dgvPurchase.Columns[k].Name.ToUpper().Trim() == "ID")
                                {
                                    dgvPurchase.Columns[k].Width = Comm.ToInt32(dtGridSettings.Rows[k][2].ToString());
                                    dgvPurchase.Columns[k].Visible = false;
                                }
                                else if (dgvPurchase.Columns[k].Name.ToUpper().Trim() == "ItemID")
                                {
                                    dgvPurchase.Columns[k].Width = Comm.ToInt32(dtGridSettings.Rows[k][2].ToString());
                                    dgvPurchase.Columns[k].Visible = false;
                                }
                                else if (dgvPurchase.Columns[k].Name.ToUpper().Trim() == "CSRATE2PER" || dgvPurchase.Columns[k].Name.ToUpper().Trim() == "CSRATE2")
                                {
                                    if (AppSettings.IsActiveSRate2 == false)
                                    {
                                        dgvPurchase.Columns[k].Width = Comm.ToInt32(dtGridSettings.Rows[k][2].ToString());
                                        dgvPurchase.Columns[k].Visible = false;
                                    }
                                    else
                                    {
                                        dgvPurchase.Columns[k].Width = Comm.ToInt32(dtGridSettings.Rows[k][2].ToString());
                                        dgvPurchase.Columns[k].Visible = Convert.ToBoolean(dtGridSettings.Rows[k][0]);
                                    }
                                }
                                else if (dgvPurchase.Columns[k].Name.ToUpper().Trim() == "CSRATE3PER" || dgvPurchase.Columns[k].Name.ToUpper().Trim() == "CSRATE3")
                                {
                                    if (AppSettings.IsActiveSRate3 == false)
                                    {
                                        dgvPurchase.Columns[k].Width = Comm.ToInt32(dtGridSettings.Rows[k][2].ToString());
                                        dgvPurchase.Columns[k].Visible = false;
                                    }
                                    else
                                    {
                                        dgvPurchase.Columns[k].Width = Comm.ToInt32(dtGridSettings.Rows[k][2].ToString());
                                        dgvPurchase.Columns[k].Visible = Convert.ToBoolean(dtGridSettings.Rows[k][0]);
                                    }
                                }
                                else if (dgvPurchase.Columns[k].Name.ToUpper().Trim() == "CSRATE4PER" || dgvPurchase.Columns[k].Name.ToUpper().Trim() == "CSRATE4")
                                {
                                    if (AppSettings.IsActiveSRate4 == false)
                                    {
                                        dgvPurchase.Columns[k].Width = Comm.ToInt32(dtGridSettings.Rows[k][2].ToString());
                                        dgvPurchase.Columns[k].Visible = false;
                                    }
                                    else
                                    {
                                        dgvPurchase.Columns[k].Width = Comm.ToInt32(dtGridSettings.Rows[k][2].ToString());
                                        dgvPurchase.Columns[k].Visible = Convert.ToBoolean(dtGridSettings.Rows[k][0]);
                                    }
                                }
                                else if (dgvPurchase.Columns[k].Name.ToUpper().Trim() == "CSRATE5PER" || dgvPurchase.Columns[k].Name.ToUpper().Trim() == "CSRATE5")
                                {
                                    if (AppSettings.IsActiveSRate5 == false)
                                    {
                                        dgvPurchase.Columns[k].Width = Comm.ToInt32(dtGridSettings.Rows[k][2].ToString());
                                        dgvPurchase.Columns[k].Visible = false;
                                    }
                                    else
                                    {
                                        dgvPurchase.Columns[k].Width = Comm.ToInt32(dtGridSettings.Rows[k][2].ToString());
                                        dgvPurchase.Columns[k].Visible = Convert.ToBoolean(dtGridSettings.Rows[k][0]);
                                    }
                                }
                                else
                                {
                                    dgvPurchase.Columns[k].Width = Comm.ToInt32(dtGridSettings.Rows[k][2].ToString());
                                    dgvPurchase.Columns[k].Visible = Convert.ToBoolean(dtGridSettings.Rows[k][0]);
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
            
            dgvPurchase.Columns["cRateinclusive"].Visible = false;

            dgvPurchase.Columns["cSlNo"].Frozen = true;
            dgvPurchase.Columns["cSlNo"].ReadOnly = true;
            //dgvPurchase.Columns["cImgDel"].Frozen = true;
            dgvPurchase.Columns["cImgDel"].Visible = true;
            dgvPurchase.Columns["cImgDel"].Width = 40;

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

        private void txtQtyCess_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (txtQtyCess.Text.Trim() != ".")
                {
                    if (txtQtyCess.Text == "") { txtQtyCess.Text = "0"; txtQtyCess.SelectAll(); }
                    CalcTotal();
                }
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
        }

        private void txtDiscPerc_Enter(object sender, EventArgs e)
        {
            txtDiscPerc.SelectAll();
        }

        private void txtDiscAmt_Enter(object sender, EventArgs e)
        {
            txtDiscAmt.SelectAll();
        }

        private void txtQtyCess_Enter(object sender, EventArgs e)
        {
            txtQtyCess.SelectAll();
        }

        private void txtOtherExp_Enter(object sender, EventArgs e)
        {
            txtOtherExp.Select(0,txtOtherExp.Text.Length - 1);
        }

        private void txtOtherExp_Click(object sender, EventArgs e)
        {
            txtOtherExp.SelectAll();
        }

        private void txtDiscPerc_Click(object sender, EventArgs e)
        {
            txtDiscPerc.SelectAll();
        }

        private void txtDiscAmt_Click(object sender, EventArgs e)
        {
            txtDiscAmt.SelectAll();
        }

        private void txtQtyCess_Click(object sender, EventArgs e)
        {
            txtQtyCess.SelectAll();
        }

        private void txtCostFactor_Click(object sender, EventArgs e)
        {
            txtCostFactor.SelectAll();
        }

        private void txtCashDisc_Click(object sender, EventArgs e)
        {
            txtCashDisc.SelectAll();
        }

        private void txtcashDisper_Click(object sender, EventArgs e)
        {
            txtcashDisper.SelectAll();
        }

        private void txtDiscAmt_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                e.Handled = Comm.CheckNumeric(sender, e);
            }
            catch
            {

            }

            dSteadyBillDiscAmt = Comm.ToDecimal(txtDiscAmt.Text);
            dSteadyBillDiscPerc = 0;
        }

        private void txtOtherExp_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                e.Handled = Comm.CheckNumeric(sender, e);
            }
            catch
            {

            }
        }

        private void txtCostFactor_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                e.Handled = Comm.CheckNumeric(sender, e);
            }
            catch
            {

            }
        }

        private void txtQtyCess_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                e.Handled = Comm.CheckNumeric(sender, e);
            }
            catch
            {

            }
        }

        private void txtCashDisc_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                e.Handled = Comm.CheckNumeric(sender, e);
            }
            catch
            {

            }
        }

        private void txtCoolie_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                e.Handled = Comm.CheckNumeric(sender, e);
            }
            catch
            {

            }
        }

        private void txtDiscAmt_Leave(object sender, EventArgs e)
        {
            try
            {
                if (txtDiscAmt.Text.Trim() != ".")
                {
                    if (txtDiscAmt.Text == "") { txtDiscAmt.Text = "0"; txtDiscAmt.SelectAll(); }
                    if (Comm.ToDecimal(txtDiscAmt.Text) > 0)
                        CalcTotal();
                }
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
        }

        private void flowLPnlBottom_Paint(object sender, PaintEventArgs e)
        {

        }

        private void txtRoundOff_Click(object sender, EventArgs e)
        {
            txtRoundOff.SelectAll();
        }

        private void cboPayment_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboPayment.SelectedIndex==1)
            {
                txtInstantReceipt.Enabled = true;
                txtInstantReceipt.BackColor = Color.White;
            }
            else
            {
                txtInstantReceipt.Text = "";
                txtInstantReceipt.Enabled = false;
                txtInstantReceipt.BackColor = Color.Gainsboro;
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

        private void frmStockInVoucherNew_Activated(object sender, EventArgs e)
        {

        }


        private void dgvColWidth_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            //try
            //{
            //    if (dgvColWidth.CurrentRow.Index > 0)
            //    {
            //        if (dgvColWidth.Rows[dgvColWidth.CurrentCell.RowIndex].Cells[2].Value.ToString() == null) dgvColWidth.Rows[dgvColWidth.CurrentCell.RowIndex].Cells[2].Value = "0";
            //        if (dgvColWidth.Rows[dgvColWidth.CurrentCell.RowIndex].Cells[2].Value.ToString() == "") dgvColWidth.Rows[dgvColWidth.CurrentCell.RowIndex].Cells[2].Value = "0";

            //        if (dgvColWidth.Rows[dgvColWidth.CurrentCell.RowIndex].Cells[0].ReadOnly == true)
            //        {
            //            if (Comm.ToDecimal(dgvColWidth.Rows[dgvColWidth.CurrentCell.RowIndex].Cells[2].Value.ToString()) < 50)
            //            {
            //                dgvColWidth.Rows[dgvColWidth.CurrentCell.RowIndex].Cells[2].Value = "50";
            //            }
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
            //    MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            //}
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

        private void frmStockInVoucherNew_Shown(object sender, EventArgs e)
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

        private void txtCoolie_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (txtCoolie.Text.Trim() != ".")
                {
                    if (txtCoolie.Text == "") { txtCoolie.Text = "0"; txtCoolie.SelectAll(); }
                    CalcTotal();
                }
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void dgvPurchase_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                int iRow = dgvPurchase.CurrentCell.RowIndex;
                int iCol = dgvPurchase.CurrentCell.ColumnIndex;
                if (dgvPurchase.Columns[dgvPurchase.CurrentCell.ColumnIndex].Index == GetEnum(GridColIndexes.cNetAmount))
                {
                    dgvPurchase.CurrentCell = dgvPurchase[1, dgvPurchase.CurrentCell.RowIndex + 1];
                }
                else if (dgvPurchase.Columns[dgvPurchase.CurrentCell.ColumnIndex].Index == GetEnum(GridColIndexes.cSlNo))
                {
                    dgvPurchase.CurrentCell = dgvPurchase[1, dgvPurchase.CurrentCell.RowIndex + 1];
                }
                else if (dgvPurchase.Columns[dgvPurchase.CurrentCell.ColumnIndex].Index == GetEnum(GridColIndexes.cImgDel))
                {
                    dgvPurchase.CurrentCell = dgvPurchase[1, dgvPurchase.CurrentCell.RowIndex + 1];
                }
            }
            catch
            { }
        }

        private void dgvPurchase_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                int iRow = dgvPurchase.CurrentCell.RowIndex;
                int iCol = dgvPurchase.CurrentCell.ColumnIndex;
                if (dgvPurchase.Columns[dgvPurchase.CurrentCell.ColumnIndex].Index == GetEnum(GridColIndexes.cNetAmount))
                {
                    dgvPurchase.CellValidated -= dgvPurchase_CellValidated;
                    dgvPurchase.CurrentCell = dgvPurchase[1, dgvPurchase.CurrentCell.RowIndex + 1];
                    dgvPurchase.CellValidated += dgvPurchase_CellValidated;
                }
                else if (dgvPurchase.Columns[dgvPurchase.CurrentCell.ColumnIndex].Index == GetEnum(GridColIndexes.cSlNo))
                {
                    dgvPurchase.CellValidated -= dgvPurchase_CellValidated;
                    dgvPurchase.CurrentCell = dgvPurchase[1, dgvPurchase.CurrentCell.RowIndex + 1];
                    dgvPurchase.CellValidated += dgvPurchase_CellValidated;
                }
                else if (dgvPurchase.Columns[dgvPurchase.CurrentCell.ColumnIndex].Index == GetEnum(GridColIndexes.cImgDel))
                {
                    dgvPurchase.CellValidated -= dgvPurchase_CellValidated;
                    dgvPurchase.CurrentCell = dgvPurchase[1, dgvPurchase.CurrentCell.RowIndex + 1];
                    dgvPurchase.CellValidated += dgvPurchase_CellValidated;
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
                            dgvPurchase.ColumnWidthChanged -= dgvPurchase_ColumnWidthChanged;
                            dgvPurchase.Columns[dgvColWidth.Rows[RowIndex].Cells[3].Value.ToString()].Width = 50;
                            dgvPurchase.ColumnWidthChanged += dgvPurchase_ColumnWidthChanged;
                        }
                    }
                    else
                    {
                        if (Comm.ToDecimal(dgvColWidth.Rows[RowIndex].Cells[2].Value.ToString()) < 10)
                        {
                            dgvColWidth.Rows[RowIndex].Cells[2].Value = "50";
                            dgvColWidth.Rows[RowIndex].Cells[0].Value = false;
                            dgvPurchase.ColumnWidthChanged -= dgvPurchase_ColumnWidthChanged;
                            dgvPurchase.Columns[dgvColWidth.Rows[RowIndex].Cells[3].Value.ToString()].Visible = false;
                            dgvPurchase.ColumnWidthChanged += dgvPurchase_ColumnWidthChanged;
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
                if (dgvPurchase.Columns[i].Visible == false)
                {
                    drCol["Visible"] = false;
                }
                if (dgvPurchase.Columns[i].Width <= 10)
                {
                    drCol["Visible"] = false;
                }

                if (Enum.GetName(typeof(GridColIndexes), i) == "cRateinclusive")
                    drCol["Visible"] = false;

                drCol["Name"] = dgvPurchase.Columns[i].HeaderText; //Enum.GetName(typeof(GridColIndexes), i).Substring(1, Enum.GetName(typeof(GridColIndexes), i).Length - 1);
                if (Enum.GetName(typeof(GridColIndexes), i) == dgvPurchase.Columns[i].Name)
                    drCol["Width"] = dgvPurchase.Columns[i].Width;
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
                
                if (dgvPurchase.Columns[i].Name.ToUpper().Trim() == "cDiscAmount".ToUpper() || dgvPurchase.Columns[i].Name.ToUpper().Trim() == "cDiscPer".ToUpper() || dgvPurchase.Columns[i].Name.ToUpper().Trim() == "cBillDisc".ToUpper())
                {
                    if (clsVchType.ParentID == 1005)
                    {
                        dgvColWidth.Rows[i].Visible = false;
                    }
                }
            }

            //dgvPurchase.Columns["cRateinclusive"].Visible = false;
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

        //Description : Calculate the Entire Purchase in each and every Corner
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
            int iTaxMode = 2; //GST
            bool blnCalculateCoolie = false;

            if (txtDiscPerc.Tag == null) txtDiscPerc.Tag = "0";
            if (txtDiscPerc.Tag.ToString() == "")
                txtDiscPerc.Tag = "0";


            if (txtCoolie.Text == "" || txtCoolie.Text == "0")
            {
                blnCalculateCoolie = true;
            }

            for (int i = 0; i < dgvPurchase.Rows.Count; i++)
            {
                SetValue(GetEnum(GridColIndexes.cSlNo), i, (i + 1).ToString());
                if (dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.CItemName)].Value != null)
                {
                    if (dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.CItemName)].Value.ToString() != "")
                    {
                        if (Comm.ToDecimal(dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cItemID)].Value) != 0)
                        {
                            if (dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cQty)].Value == null)
                                SetValue(GetEnum(GridColIndexes.cQty), i, "0");
                            if (dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cFree)].Value == null)
                                SetValue(GetEnum(GridColIndexes.cFree), i, "0");

                            DblRate = Comm.ToDouble(dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cPrate)].Value);
                            //Dipu on 13-May-2022 ---------- >
                            dblQty = Comm.ToDouble(dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cQty)].Value);
                            //dblQty = Comm.ToDouble(dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cQty)].Value) + Comm.ToDouble(dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cFree)].Value);
                            //Dipu on 25-May-2022 -- Free Value Commented
                            QtyTotal = QtyTotal + dblQty;// + Comm.ToDouble(dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cFree)].Value);
                            
                            //SetValue(GetEnum(GridColIndexes.cSlNo), i, (i + 1).ToString());

                            //DblrateDiscper = Comm.ToDouble(dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cRateDiscPer)].Value);
                            DblRateAfterRDiscount = DblRate - (DblRate * DblrateDiscper / 100);

                            if (blnCalculateCoolie == true)
                            {
                                CoolieTotal += Comm.ToDouble(dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cCoolie)].Value);
                            }

                            dblTaxPer = Comm.ToDouble(dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.ctaxPer)].Value);
                            dblCessPer = Comm.ToDouble(dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cCCessPer)].Value);
                            dblQtyCessPer = Comm.ToDouble(dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cCCompCessQty)].Value);
                            if (dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cFloodCessPer)].Value == null)
                                SetValue(GetEnum(GridColIndexes.cFloodCessPer), i, "");

                            //If chkApplyFloodCess.CheckState = CheckState.Checked Then
                            if (dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cFloodCessPer)].Value.ToString() == "")
                                SetValue(GetEnum(GridColIndexes.cFloodCessPer), i, "0");
                            dblFloodCessPer = Comm.ToDouble(dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cFloodCessPer)].Value);
                            //End If

                            if (clsVchType.DefaultTaxInclusiveValue == 2)
                                dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cRateinclusive)].Value = true;
                            else if (clsVchType.DefaultTaxInclusiveValue == 3)
                                dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cRateinclusive)].Value = false;

                            if (Convert.ToBoolean(dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cRateinclusive)].Value) == true)
                                DblRateExclusive = GetRateExclusive(DblRateAfterRDiscount, (dblCessPer + dblTaxPer + dblFloodCessPer), 0);
                            else
                                DblRateExclusive = DblRateAfterRDiscount;

                            dblGrossValue = DblRateExclusive * dblQty;
                            SetValue(GetEnum(GridColIndexes.cGrossAmt), i, Comm.FormatValue(dblGrossValue));
                            dblGrossValueTot = dblGrossValueTot + dblGrossValue;
                            dblGrossValueAfterRateDiscount = dblQty * (DblRateExclusive);

                            dblQtyTot += Comm.ToDouble(dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cQty)].Value);
                            dblFreeTot += Comm.ToDouble(dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cFree)].Value);

                            SetValue(GetEnum(GridColIndexes.cGrossValueAfterRateDiscount), i, Comm.FormatValue(dblGrossValueAfterRateDiscount));

                            DblrateDiscAmt = dblQty * (DblRate - DblRateAfterRDiscount);
                            DblrateDiscAmtTot = DblrateDiscAmtTot + DblrateDiscAmt;

                            dblGrossValueAfterRateDiscountTot = dblGrossValueAfterRateDiscountTot + dblGrossValueAfterRateDiscount;
                            //dbltaxableValueAfterItemDiscountTot = dblGrossValueAfterDiscountsTot;

                            if (Comm.ToDouble(dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cDiscPer)].Value) > 0)
                            {
                                SetValue(GetEnum(GridColIndexes.cDiscAmount), i, Comm.FormatValue((dblGrossValueAfterRateDiscount * Comm.ToDouble(dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cDiscPer)].Value) / 100)));
                                dblItemDiscAmountTot = dblItemDiscAmountTot + Comm.ToDouble(dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cDiscAmount)].Value);
                            }
                            else if (Comm.ToDouble(dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cDiscAmount)].Value) > 0)
                            {
                                SetValue(GetEnum(GridColIndexes.cDiscAmount), i, Comm.FormatValue(Comm.ToDouble(dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cDiscAmount)].Value)));
                                dblItemDiscAmountTot = dblItemDiscAmountTot + Comm.ToDouble(dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cDiscAmount)].Value);
                            }

                            dblGrossValueAfterDiscounts = dblGrossValueAfterRateDiscount - Comm.ToDouble(dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cDiscAmount)].Value);
                            dblGrossValueAfterDiscountsTot = dblGrossValueAfterDiscountsTot + dblGrossValueAfterDiscounts;
                            //
                            //Arrived Taxable Value
                            dbltaxableValueAfterItemDiscount = dblGrossValueAfterDiscounts;
                            dbltaxableValueAfterItemDiscountTot = dblGrossValueAfterDiscountsTot;
                            SetValue(GetEnum(GridColIndexes.cGrossValueAfterRateDiscount), i, Comm.FormatValue(dbltaxableValueAfterItemDiscount));

                            iTaxMode = Comm.ToInt32(cboTaxMode.SelectedValue) - 1;

                            if (Comm.ToDouble(dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.ctaxPer)].Value) > 0)
                            {
                                DblNontaxableValue = 0;
                                dbltaxAmount = dbltaxableValueAfterItemDiscount * Comm.ToDouble(dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.ctaxPer)].Value) / 100;
                                dbltaxableAmountTot = dbltaxableAmountTot + dbltaxableValueAfterItemDiscount;
                            }
                            else
                            {
                                dbltaxAmount = 0;
                                DblNontaxableValue = dbltaxableValueAfterItemDiscount;
                                dblNontaxableAmountTot = dblNontaxableAmountTot + dbltaxableValueAfterItemDiscount;
                            }
                            //Tax Mode wise Calculation
                            if (iTaxMode == 2) //GST
                            {
                                if (Comm.ToDouble(dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.ctaxPer)].Value) > 0)
                                {
                                    SetValue(GetEnum(GridColIndexes.ctaxable), i, Comm.FormatValue(dbltaxableValueAfterItemDiscount));
                                    SetValue(GetEnum(GridColIndexes.cNonTaxable), i, "0");
                                }
                                else
                                {
                                    SetValue(GetEnum(GridColIndexes.ctaxable), i, "0");
                                    SetValue(GetEnum(GridColIndexes.cNonTaxable), i, Comm.FormatValue(dbltaxableValueAfterItemDiscount));
                                }
                                SetValue(GetEnum(GridColIndexes.ctax), i, Comm.FormatValue(dbltaxAmount));
                                if (cboState.SelectedValue != null)
                                {
                                    if (cboState.SelectedValue.ToString() != AppSettings.StateCode)
                                    {
                                        SetValue(GetEnum(GridColIndexes.cCGST), i, "0");
                                        SetValue(GetEnum(GridColIndexes.cSGST), i, "0");
                                        SetValue(GetEnum(GridColIndexes.cIGST), i, Comm.FormatValue(dbltaxAmount));
                                    }
                                    else
                                    {
                                        SetValue(GetEnum(GridColIndexes.cCGST), i, Comm.FormatValue((dbltaxAmount * 0.5)));
                                        SetValue(GetEnum(GridColIndexes.cSGST), i, Comm.FormatValue((dbltaxAmount * 0.5)));
                                        SetValue(GetEnum(GridColIndexes.cIGST), i, "0");
                                    }
                                }
                                else
                                {
                                    SetValue(GetEnum(GridColIndexes.cCGST), i, Comm.FormatValue((dbltaxAmount * 0.5)));
                                    SetValue(GetEnum(GridColIndexes.cSGST), i, Comm.FormatValue((dbltaxAmount * 0.5)));
                                    SetValue(GetEnum(GridColIndexes.cIGST), i, "0");
                                }

                                DbltaxAmountTot = DbltaxAmountTot + dbltaxAmount;
                                SetValue(GetEnum(GridColIndexes.cNetAmount), i, Comm.FormatValue((dbltaxableValueAfterItemDiscount + dbltaxAmount)));
                                DblNetAmountTotal = DblNetAmountTotal + Comm.ToDouble(dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cNetAmount)].Value);
                            }
                            else if (iTaxMode == 1) //VAT
                            {
                                if (Comm.ToDouble(dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.ctaxPer)].Value) > 0)
                                {
                                    SetValue(GetEnum(GridColIndexes.ctaxable), i, Comm.FormatValue(dbltaxableValueAfterItemDiscount));
                                    SetValue(GetEnum(GridColIndexes.cNonTaxable), i, "0");

                                    SetValue(GetEnum(GridColIndexes.cCGST), i, "0");
                                    SetValue(GetEnum(GridColIndexes.cSGST), i, "0");
                                    SetValue(GetEnum(GridColIndexes.cIGST), i, Comm.FormatValue(dbltaxAmount));

                                }
                                else
                                {
                                    SetValue(GetEnum(GridColIndexes.ctaxable), i, "0");
                                    SetValue(GetEnum(GridColIndexes.cNonTaxable), i, Comm.FormatValue(dbltaxableValueAfterItemDiscount));
                                }
                                SetValue(GetEnum(GridColIndexes.ctax), i, Comm.FormatValue(dbltaxAmount));

                                DbltaxAmountTot = DbltaxAmountTot + dbltaxAmount;
                                SetValue(GetEnum(GridColIndexes.cNetAmount), i, Comm.FormatValue((dbltaxableValueAfterItemDiscount + dbltaxAmount)));
                                DblNetAmountTotal = DblNetAmountTotal + Comm.ToDouble(dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cNetAmount)].Value);
                            }
                            else if (iTaxMode == 0) //NONE
                            {
                                SetValue(GetEnum(GridColIndexes.ctaxable), i, Comm.FormatValue(dbltaxableValueAfterItemDiscount));
                                SetValue(GetEnum(GridColIndexes.cNonTaxable), i, Comm.FormatValue(DblNontaxableValue));
                                //Check Dipu

                                SetValue(GetEnum(GridColIndexes.cCGST), i, "0");
                                SetValue(GetEnum(GridColIndexes.cSGST), i, "0");
                                SetValue(GetEnum(GridColIndexes.cIGST), i, "0");

                                SetValue(GetEnum(GridColIndexes.ctax), i, "0");


                                //DbltaxAmountTot = DbltaxAmountTot + dbltaxAmount;
                                SetValue(GetEnum(GridColIndexes.cNetAmount), i, Comm.FormatValue((dbltaxableValueAfterItemDiscount)));
                                DblNetAmountTotal = DblNetAmountTotal + Comm.ToDouble(dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cNetAmount)].Value);
                            }
                            //DbltaxAmountTot = DbltaxAmountTot + dbltaxAmount;
                            //SetValue(GetEnum(GridColIndexes.cNetAmount), i, Comm.FormatValue((dbltaxableValueAfterItemDiscount + dbltaxAmount)));
                            //DblNetAmountTotal = DblNetAmountTotal + Comm.ToDouble(dgvPurchase.Rows[i].Cells[GetEnum(GridColIndexes.cNetAmount)].Value);

                        }
                    }
                }
            }

            if (blnCalculateCoolie == true)
                txtCoolie.Text = CoolieTotal.ToString();

            if (txtDiscAmt.Text == "")
                txtDiscPerc.Text = "0";

            if (Comm.ToDouble(txtDiscAmt.Text) > 0)
            {
                if (dSteadyBillDiscAmt > 0 && dSteadyBillDiscPerc == 0)
                {
                    if (dblGrossValueAfterDiscountsTot > 0)
                    {
                        txtDiscPerc.TextChanged -= txtDiscPerc_TextChanged;
                        txtDiscPerc.Text = Comm.FormatValue((Comm.ToDouble(txtDiscAmt.Text) / dblGrossValueAfterDiscountsTot * 100));
                        txtDiscPerc.TextChanged += txtDiscPerc_TextChanged;
                    }
                }
                else
                {
                    if (Comm.ToDouble(txtDiscPerc.Text) > 0)
                    {
                        if (dblGrossValueAfterDiscountsTot > 0)
                            txtDiscAmt.Text = Comm.FormatValue((dblGrossValueAfterDiscountsTot * Comm.ToDouble(txtDiscPerc.Text) / 100));
                    }
                }

            }
            else
            {
                //txtDiscAmt.Text = "";
                if (dSteadyBillDiscPerc > 0 && dSteadyBillDiscAmt == 0)
                {
                    if (Comm.ToDouble(txtDiscPerc.Text) > 0)
                        txtDiscAmt.Text = Comm.FormatValue((dblGrossValueAfterDiscountsTot * Comm.ToDouble(txtDiscPerc.Text) / 100));
                }
                else
                {
                    if (dblGrossValueAfterDiscountsTot > 0)
                    {
                        if (Comm.ToInt32(txtDiscPerc.Tag.ToString()) == 0)
                            txtDiscPerc.Text = Comm.FormatValue((Comm.ToDouble(txtDiscAmt.Text) / dblGrossValueAfterDiscountsTot * 100));

                    }
                }
            }

            if (txtDiscAmt.Text.ToString() == "") txtDiscAmt.Text = "0";
            if (txtDiscPerc.Text.ToString() == "") txtDiscPerc.Text = "0";
            if (Comm.ToDouble(txtDiscAmt.Text.ToString()) == 0 && Comm.ToDouble(txtDiscPerc.Text.ToString()) != 0)
                txtDiscAmt.Text = Comm.FormatValue((dblGrossValueAfterDiscountsTot * Comm.ToDouble(txtDiscPerc.Text) / 100));
            if (Comm.ToDouble(txtDiscAmt.Text.ToString()) != 0 && Comm.ToDouble(txtDiscPerc.Text.ToString()) == 0)
                txtDiscPerc.Text = Comm.FormatValue((Comm.ToDouble(txtDiscAmt.Text) / dblGrossValueAfterDiscountsTot * 100));

            //''''''' Bill Dicount Calculation''''''''''''''''''''
            //'First Discount 

            if (txtDiscAmt.Text == "") txtDiscAmt.Text = "0";
            double Discountamount = Comm.ToDouble(txtDiscAmt.Text);
            DblNetAmountTotal = 0;
            dbltaxableAmountTot = 0;
            dblNontaxableAmountTot = 0;
            dbltaxAmount = 0;
            dbltaxAmountTot = 0;
            double TotalValueOfFree = 0;
            if (txtOtherExp.Text == "") txtOtherExp.Text = "0";
            if (txtCashDisc.Text == "") txtCashDisc.Text = "0";
            double BillExpeDisc = Comm.ToDouble(txtOtherExp.Text) - Comm.ToDouble(txtCashDisc.Text) - Comm.ToDouble(txtDiscAmt.Text);
            double Savings = 0;
            double dbltaxableAmount = 0;

            for (int j = 0; j < dgvPurchase.Rows.Count; j++)
            {
                if (dgvPurchase.Rows[j].Cells[GetEnum(GridColIndexes.CItemName)].Value != null)
                {
                    if (dgvPurchase.Rows[j].Cells[GetEnum(GridColIndexes.CItemName)].Value.ToString() != "")
                    {
                        dblTaxPer = Comm.ToDouble(dgvPurchase.Rows[j].Cells[GetEnum(GridColIndexes.ctaxPer)].Value);
                        dblCessPer = Comm.ToDouble(dgvPurchase.Rows[j].Cells[GetEnum(GridColIndexes.cCCessPer)].Value);
                        dblQtyCessPer = Comm.ToDouble(dgvPurchase.Rows[j].Cells[GetEnum(GridColIndexes.cCCompCessQty)].Value);
                        // check from Settings
                        dblFloodCessPer = 0;

                        SetValue(GetEnum(GridColIndexes.cBillDisc), j, "0");
                        dblGrossValueAfterDiscounts = Comm.ToDouble(dgvPurchase.Rows[j].Cells[GetEnum(GridColIndexes.cGrossValueAfterRateDiscount)].Value);
                        if (dblGrossValueAfterDiscountsTot > 0)
                            SetValue(GetEnum(GridColIndexes.cBillDisc), j, Comm.FormatValue((Discountamount / dblGrossValueAfterDiscountsTot * dblGrossValueAfterDiscounts)));

                        if (Comm.ToDouble(dgvPurchase.Rows[j].Cells[GetEnum(GridColIndexes.ctaxPer)].Value) > 0)
                        {
                            dbltaxableAmount = Comm.ToDouble(dgvPurchase.Rows[j].Cells[GetEnum(GridColIndexes.ctaxable)].Value) - Comm.ToDouble(dgvPurchase.Rows[j].Cells[GetEnum(GridColIndexes.cBillDisc)].Value);
                            DblNontaxableValue = 0;
                        }
                        else
                        {
                            DblNontaxableValue = Comm.ToDouble(dgvPurchase.Rows[j].Cells[GetEnum(GridColIndexes.cNonTaxable)].Value) - Comm.ToDouble(dgvPurchase.Rows[j].Cells[GetEnum(GridColIndexes.cBillDisc)].Value);
                            dbltaxableAmount = 0;
                        }

                        SetValue(GetEnum(GridColIndexes.ctaxable), j, Comm.FormatValue(dbltaxableAmount));
                        SetValue(GetEnum(GridColIndexes.cNonTaxable), j, Comm.FormatValue(DblNontaxableValue));

                        dbltaxAmount = Comm.ToDouble(dgvPurchase.Rows[j].Cells[GetEnum(GridColIndexes.ctaxable)].Value) * Comm.ToDouble(dgvPurchase.Rows[j].Cells[GetEnum(GridColIndexes.ctaxPer)].Value) / 100;
                        DblcessAmount = Comm.ToDouble(dgvPurchase.Rows[j].Cells[GetEnum(GridColIndexes.ctaxable)].Value) * dblCessPer / 100;
                        DblCompcessAmount = Comm.ToDouble(dgvPurchase.Rows[j].Cells[GetEnum(GridColIndexes.cQty)].Value) * dblQtyCessPer;
                        DblFloodcessAmount = Comm.ToDouble(dgvPurchase.Rows[j].Cells[GetEnum(GridColIndexes.ctaxable)].Value) * dblFloodCessPer / 100;

                        SetTag(GetEnum(GridColIndexes.cCCessPer), j, Comm.FormatValue(DblcessAmount, true, "#.00"));
                        SetTag(GetEnum(GridColIndexes.cCCompCessQty), j, Comm.FormatValue(DblCompcessAmount, false));

                        SetValue(GetEnum(GridColIndexes.cFloodCessAmt), j, Comm.FormatValue(DblFloodcessAmount));
                        DblFloodcessAmountTot = DblFloodcessAmountTot + DblFloodcessAmount;
                        DblcessAmountTot = DblcessAmountTot + DblcessAmount;
                        DblCompcessAmountTot = DblCompcessAmountTot + DblCompcessAmount;

                        if (iTaxMode == 2) //GST
                        {
                            SetValue(GetEnum(GridColIndexes.ctax), j, Comm.FormatValue(dbltaxAmount));
                            if (Comm.ToDouble(dgvPurchase.Rows[j].Cells[GetEnum(GridColIndexes.ctaxPer)].Value) > 0)
                            {
                                SetValue(GetEnum(GridColIndexes.ctaxable), j, Comm.FormatValue(Comm.ToDouble(dgvPurchase.Rows[j].Cells[GetEnum(GridColIndexes.ctaxable)].Value)));
                                SetValue(GetEnum(GridColIndexes.cNonTaxable), j, "0");
                            }
                            else
                            {
                                SetValue(GetEnum(GridColIndexes.cNonTaxable), j, Comm.FormatValue(Comm.ToDouble(dgvPurchase.Rows[j].Cells[GetEnum(GridColIndexes.cNonTaxable)].Value)));
                                SetValue(GetEnum(GridColIndexes.ctaxable), j, "0");
                            }

                            if (cboState.SelectedValue != null)
                            {
                                if (cboState.SelectedValue.ToString() != AppSettings.StateCode)
                                {
                                    SetValue(GetEnum(GridColIndexes.cCGST), j, "0");
                                    SetValue(GetEnum(GridColIndexes.cSGST), j, "0");
                                    SetValue(GetEnum(GridColIndexes.cIGST), j, Comm.FormatValue(dbltaxAmount));
                                    SetTag(GetEnum(GridColIndexes.cCGST), j, "0"); ;

                                    SetTag(GetEnum(GridColIndexes.cSGST), j, "0");
                                    SetTag(GetEnum(GridColIndexes.cIGST), j, Comm.ToDouble(dgvPurchase.Rows[j].Cells[GetEnum(GridColIndexes.ctaxPer)].Value).ToString());
                                }
                                else
                                {
                                    SetValue(GetEnum(GridColIndexes.cCGST), j, Comm.FormatValue((dbltaxAmount * 0.5)));
                                    SetValue(GetEnum(GridColIndexes.cSGST), j, Comm.FormatValue((dbltaxAmount * 0.5)));
                                    SetValue(GetEnum(GridColIndexes.cIGST), j, "0");

                                    SetTag(GetEnum(GridColIndexes.cCGST), j, (Comm.ToDouble(dgvPurchase.Rows[j].Cells[GetEnum(GridColIndexes.ctaxPer)].Value) * 0.5).ToString());
                                    SetTag(GetEnum(GridColIndexes.cSGST), j, (Comm.ToDouble(dgvPurchase.Rows[j].Cells[GetEnum(GridColIndexes.ctaxPer)].Value) * 0.5).ToString());
                                    SetTag(GetEnum(GridColIndexes.cIGST), j, "0");

                                }
                            }
                            else
                            {
                                SetValue(GetEnum(GridColIndexes.cCGST), j, Comm.FormatValue((dbltaxAmount * 0.5)));
                                SetValue(GetEnum(GridColIndexes.cSGST), j, Comm.FormatValue((dbltaxAmount * 0.5)));
                                SetValue(GetEnum(GridColIndexes.cIGST), j, "0");

                                SetTag(GetEnum(GridColIndexes.cCGST), j, (Comm.ToDouble(dgvPurchase.Rows[j].Cells[GetEnum(GridColIndexes.ctaxPer)].Value) * 0.5).ToString());
                                SetTag(GetEnum(GridColIndexes.cSGST), j, (Comm.ToDouble(dgvPurchase.Rows[j].Cells[GetEnum(GridColIndexes.ctaxPer)].Value) * 0.5).ToString());
                                SetTag(GetEnum(GridColIndexes.cIGST), j, "0");
                            }
                        }
                        else if (iTaxMode == 1) //VAT
                        {
                            SetValue(GetEnum(GridColIndexes.ctax), j, Comm.FormatValue(dbltaxAmount));
                            if (Comm.ToDouble(dgvPurchase.Rows[j].Cells[GetEnum(GridColIndexes.ctaxPer)].Value) > 0)
                            {
                                SetValue(GetEnum(GridColIndexes.ctaxable), j, Comm.FormatValue(Comm.ToDouble(dgvPurchase.Rows[j].Cells[GetEnum(GridColIndexes.ctaxable)].Value)));
                                SetValue(GetEnum(GridColIndexes.cNonTaxable), j, "0");
                            }
                            else
                            {
                                SetValue(GetEnum(GridColIndexes.cNonTaxable), j, Comm.FormatValue(Comm.ToDouble(dgvPurchase.Rows[j].Cells[GetEnum(GridColIndexes.cNonTaxable)].Value)));
                                SetValue(GetEnum(GridColIndexes.ctaxable), j, "0");
                            }

                            SetValue(GetEnum(GridColIndexes.cCGST), j, "0");
                            SetValue(GetEnum(GridColIndexes.cSGST), j, "0");
                            SetValue(GetEnum(GridColIndexes.cIGST), j, Comm.FormatValue(dbltaxAmount));
                            SetTag(GetEnum(GridColIndexes.cCGST), j, "0"); ;

                            SetTag(GetEnum(GridColIndexes.cSGST), j, "0");
                            SetTag(GetEnum(GridColIndexes.cIGST), j, Comm.ToDouble(dgvPurchase.Rows[j].Cells[GetEnum(GridColIndexes.ctaxPer)].Value).ToString());
                        }
                        else if (iTaxMode == 0) //NONE
                        {
                            //SetValue(GetEnum(GridColIndexes.ctaxable), j, Comm.FormatValue(dbltaxableValueAfterItemDiscount));
                            SetValue(GetEnum(GridColIndexes.ctaxable), j, Comm.FormatValue(dbltaxableAmount));
                            SetValue(GetEnum(GridColIndexes.cNonTaxable), j, Comm.FormatValue(DblNontaxableValue));
                            //Check Dipu
                            //SetValue(GetEnum(gridColIndexes.ctaxable), j, Comm.FormatValue(dbltaxableAmount));

                            SetValue(GetEnum(GridColIndexes.cCGST), j, "0");
                            SetValue(GetEnum(GridColIndexes.cSGST), j, "0");
                            SetValue(GetEnum(GridColIndexes.cIGST), j, "0");

                            //SetValue(GetEnum(GridColIndexes.cNetAmount), j, Comm.FormatValue((dbltaxableValueAfterItemDiscount)));
                        }

                        dblIGSTTot = dblIGSTTot + Comm.ToDouble(dgvPurchase.Rows[j].Cells[GetEnum(GridColIndexes.cIGST)].Value);
                        dblSSGTTot = dblSSGTTot + Comm.ToDouble(dgvPurchase.Rows[j].Cells[GetEnum(GridColIndexes.cSGST)].Value);
                        dblCSGTTot = dblCSGTTot + Comm.ToDouble(dgvPurchase.Rows[j].Cells[GetEnum(GridColIndexes.cCGST)].Value);

                        dbltaxAmountTot = dbltaxAmountTot + dbltaxAmount;
                        //dbltaxAmountTot = Comm.FormatAmt(Val(dbltaxAmountTot) + Val(Format(Val(dbltaxAmount), DCSApp.GBizAmt)), DCSApp.GBizAmt)
                        // dont know how to format ??

                        dbltaxableAmountTot = dbltaxableAmountTot + Comm.ToDouble(dgvPurchase.Rows[j].Cells[GetEnum(GridColIndexes.ctaxable)].Value);
                        dblNontaxableAmountTot = dblNontaxableAmountTot + Comm.ToDouble(dgvPurchase.Rows[j].Cells[GetEnum(GridColIndexes.cNonTaxable)].Value);

                        //DGVItem.Item(cNetAmount, i).Value = Comm.FormatAmt(Val(DGVItem.Item(ctaxable, i).Value) + Val(DGVItem.Item(cNonTaxable, i).Value) + Val(DGVItem.Item(ctax, i).Value) + Val(DblcessAmount) + Val(DblFloodcessAmount) + Val(DblCompcessAmount), "")
                        //Dont know what is Comm.FormatAmt ->
                        //if (iTaxMode != 0) //NOT NONE
                            
                        SetValue(GetEnum(GridColIndexes.cNetAmount), j, Comm.FormatValue((Comm.ToDouble(dgvPurchase.Rows[j].Cells[GetEnum(GridColIndexes.ctaxable)].Value) + Comm.ToDouble(dgvPurchase.Rows[j].Cells[GetEnum(GridColIndexes.cNonTaxable)].Value) + Comm.ToDouble(dgvPurchase.Rows[j].Cells[GetEnum(GridColIndexes.ctax)].Value) + DblcessAmount + DblFloodcessAmount + DblCompcessAmount)));
                        DblNetAmountTotal = DblNetAmountTotal + Comm.ToDouble(dgvPurchase.Rows[j].Cells[GetEnum(GridColIndexes.cNetAmount)].Value);

                        //valuation of Free
                        dblQty = Comm.ToDouble(dgvPurchase.Rows[j].Cells[GetEnum(GridColIndexes.cQty)].Value);
                        if (Comm.ToDouble(dgvPurchase.Rows[j].Cells[GetEnum(GridColIndexes.cFree)].Value) > 0)
                        {
                            double PerItemRate = Comm.ToDouble(dgvPurchase.Rows[j].Cells[GetEnum(GridColIndexes.cNetAmount)].Value) - Comm.ToDouble(dgvPurchase.Rows[j].Cells[GetEnum(GridColIndexes.ctax)].Value) / dblQty;
                            TotalValueOfFree = TotalValueOfFree + (PerItemRate * Comm.ToDouble(dgvPurchase.Rows[j].Cells[GetEnum(GridColIndexes.cFree)].Value));
                        }

                        //CALCULATION DECIMAL CHANGING
                        SetValue(GetEnum(GridColIndexes.cDiscAmount), j, Comm.FormatValue(Comm.ToDouble(dgvPurchase.Rows[j].Cells[GetEnum(GridColIndexes.cDiscAmount)].Value)));
                        SetValue(GetEnum(GridColIndexes.cBillDisc), j, Comm.FormatValue(Comm.ToDouble(dgvPurchase.Rows[j].Cells[GetEnum(GridColIndexes.cBillDisc)].Value)));

                        SetValue(GetEnum(GridColIndexes.ctaxable), j, Comm.FormatValue(Comm.ToDouble(dgvPurchase.Rows[j].Cells[GetEnum(GridColIndexes.ctaxable)].Value)));
                        SetTag(GetEnum(GridColIndexes.ctaxable), j, Comm.FormatValue(Comm.ToDouble(dgvPurchase.Rows[j].Cells[GetEnum(GridColIndexes.ctaxable)].Value)));
                        //DGVItem.Item(ctaxable, i).Tag = Format(Val(DGVItem.Item(ctaxable, i).Value), "#0.000000")
                        //Tag ??

                        SetValue(GetEnum(GridColIndexes.ctax), j, Comm.FormatValue(Comm.ToDouble(dgvPurchase.Rows[j].Cells[GetEnum(GridColIndexes.ctax)].Value)));
                        SetValue(GetEnum(GridColIndexes.cIGST), j, Comm.FormatValue(Comm.ToDouble(dgvPurchase.Rows[j].Cells[GetEnum(GridColIndexes.cIGST)].Value)));
                        SetValue(GetEnum(GridColIndexes.cSGST), j, Comm.FormatValue(Comm.ToDouble(dgvPurchase.Rows[j].Cells[GetEnum(GridColIndexes.cSGST)].Value)));
                        SetValue(GetEnum(GridColIndexes.cCGST), j, Comm.FormatValue(Comm.ToDouble(dgvPurchase.Rows[j].Cells[GetEnum(GridColIndexes.cCGST)].Value)));
                        SetValue(GetEnum(GridColIndexes.cNetAmount), j, Comm.FormatValue(Comm.ToDouble(dgvPurchase.Rows[j].Cells[GetEnum(GridColIndexes.cNetAmount)].Value)));
                        //SetValue(GetEnum(GridColIndexes.cNetAmount), j, Comm.FormatValue(Comm.ToDouble(dgvPurchase.Rows[j].Cells[GetEnum(GridColIndexes.cNetAmount)].Value)));
                        SetValue(GetEnum(GridColIndexes.cGrossValueAfterRateDiscount), j, Comm.FormatValue(Comm.ToDouble(dgvPurchase.Rows[j].Cells[GetEnum(GridColIndexes.cGrossValueAfterRateDiscount)].Value)));
                        SetValue(GetEnum(GridColIndexes.cNonTaxable), j, Comm.FormatValue(Comm.ToDouble(dgvPurchase.Rows[j].Cells[GetEnum(GridColIndexes.cNonTaxable)].Value)));

                        if (Comm.ToDouble(dgvPurchase.Rows[j].Cells[GetEnum(GridColIndexes.cAgentCommPer)].Value) > 0)
                            DblItemAgentCommission = (DblItemAgentCommission + Comm.ToDouble(dgvPurchase.Rows[j].Cells[GetEnum(GridColIndexes.cNetAmount)].Value) * Comm.ToDouble(dgvPurchase.Rows[j].Cells[GetEnum(GridColIndexes.cAgentCommPer)].Value) / 100);

                    }
                }
            }

            // What is the use of this ?? --------------------------- >>
            //If dgv.GetValue(LinkIDs.AgentCommissionMode) = "ITEM" Then
            //    dgv.SetValue(LinkIDs.AgentCommission, DblItemAgentCommission)
            //ElseIf dgv.GetValue(LinkIDs.AgentCommissionMode) = "NONE" Then
            //    dgv.SetValue(LinkIDs.AgentCommission, 0)
            //ElseIf InStr(dgv.GetValue(LinkIDs.AgentCommissionMode), "BILL", CompareMethod.Text) > 0 Then
            //    Dim MyVarStr() As String = Split(dgv.GetValue(LinkIDs.AgentCommissionMode), "@")
            //    If UBound(MyVarStr) > 0 Then
            //        DblItemAgentCommission = Val(lblBalance.Text) * Val(MyVarStr(1)) / 100
            //    End If
            //    dgv.SetValue(LinkIDs.AgentCommission, DblItemAgentCommission)
            //End If
            // What is the use of this ?? --------------------------- >>

            lblAgentCommissionTotal.Text = Comm.FormatValue(DblItemAgentCommission);
            txtNetAmt.Text = Comm.FormatValue(DblNetAmountTotal);
            txtGrossAmt.Text = Comm.FormatValue(dblGrossValueTot);
            lblQtyTotal.Text = Comm.FormatValue(dblQtyTot);
            lblFreeTotal.Text = Comm.FormatValue(dblFreeTot);
            //txtGrossAftRateDiscount.Text = Comm.FormatAmt(dblGrossValueAfterRateDiscountTot, Global.GDecimal);
            txtGrossAfterItmDisc.Text = Comm.FormatValue(dbltaxableValueAfterItemDiscountTot);
            txtRateDiscTot.Text = Comm.FormatValue(DblrateDiscAmtTot);
            txtItemDiscTot.Text = Comm.FormatValue(dblItemDiscAmountTot);
            txtTaxable.Text = Comm.FormatValue(dbltaxableAmountTot);
            txtNonTaxable.Text = Comm.FormatValue(dblNontaxableAmountTot);
            txtTaxAmt.Text = Comm.FormatValue(dbltaxAmountTot);

            txtCess.Text = Comm.FormatValue(DblcessAmountTot);
            txtCompCess.Text = Comm.FormatValue(DblCompcessAmountTot);
            txtNetAmt.Text = Comm.FormatValue(DblNetAmountTotal);

            double bALANCEFORrOUNDOFF = Comm.ToDouble(Comm.FormatAmt(DblNetAmountTotal - 0 - Comm.ToDouble(txtCashDisc.Text) + Comm.ToDouble(txtOtherExp.Text), ""));

            lblBillAmount.Text = Comm.FormatValue(bALANCEFORrOUNDOFF);

            if (txtRoundOff.Text.ToString() == "")
                txtRoundOff.Text = "0";

            if (clsVchType.RoundOffMode > 0)
            {
                if (clsVchType.RoundOffMode != 4) //Manual
                {
                    double RoundOffValue = 0;
                    clsJSonCommon Roundoff = new clsJSonCommon();
                    RoundOffValue = Roundoff.RoundOffAmount(bALANCEFORrOUNDOFF, clsVchType.RoundOffMode, clsVchType.RoundOffBlock);
                    txtRoundOff.Text = Comm.FormatValue(RoundOffValue - Comm.ToDouble(lblBillAmount.Text));
                }
                else
                {
                    lblBillAmount.Text = (bALANCEFORrOUNDOFF + Comm.ToDouble(txtRoundOff.Text.ToString())).ToString();
                }
            }
            //if (mytrans.IntRoundOffMode != 4) //Manual
            //{
            //    double RoundOffValue = 0;
            //    //RoundOffValue = = RoundOffAmount(bALANCEFORrOUNDOFF, mytrans.IntRoundOffMode, mytrans.DBLRoundOffBlock)
            //    txtRoundOff.Text = Comm.FormatValue(RoundOffValue - Comm.ToDouble(lblBillAmount.Text));
            //    lblBillAmount.Text = Comm.FormatValue(RoundOffValue);
            //}
            //else
            if (txtRoundOff.Text == "") txtRoundOff.Text = "0";
            lblBillAmount.Text = Comm.FormatValue(bALANCEFORrOUNDOFF + Comm.ToDouble(txtRoundOff.Text));
            //}
            lblBillAmount.Text = Comm.FormatValue(Comm.ToDouble(lblBillAmount.Text));
            double AdditionalCharges = 0;
            if (txtRoundOff.Text == "") txtRoundOff.Text = "0";
            if (txtCostFactor.Text == "") txtCostFactor.Text = "0";
            // When mytrans.IntRoundOffMode > 0 then Comment this
            if (txtRoundOff.Text == "") txtRoundOff.Text = "0";
            if (txtRoundOff.Text != "-")
            {
                if (txtRoundOff.Text != ".")
                {
                    //lblBillAmount.Text = Comm.FormatValue(Comm.ToDouble(lblBillAmount.Text) + Comm.ToDouble(txtRoundOff.Text));
                    AdditionalCharges = Comm.ToDouble(txtOtherExp.Text) - Comm.ToDouble(txtCashDisc.Text) + Comm.ToDouble(txtRoundOff.Text) + Comm.ToDouble(txtCostFactor.Text);
                }
            }
            // When mytrans.IntRoundOffMode > 0 then Comment this

            //Assuming that the rate is equally equated between items .....
            //if (txtRoundOff.Text == "") txtRoundOff.Text = "0";
            //if (txtCostFactor.Text == "") txtCostFactor.Text = "0";

            //'Tethering to itemwise rate
            double mytaxable = 0;
            double MyPRate = 0;
            double MyQty;
            double perpieceaddcharges;

            for (int k = 0; k < dgvPurchase.Rows.Count; k++)
            {
                if (dgvPurchase.Rows[k].Cells[GetEnum(GridColIndexes.CItemName)].Value != null)
                {
                    if (dgvPurchase.Rows[k].Cells[GetEnum(GridColIndexes.CItemName)].Value.ToString() != "")
                    {
                        //if (Comm.ToDouble(dgvPurchase.Rows[k].Cells[GetEnum(GridColIndexes.CItemName)].Tag) > 0)
                        //{
                            if (dgvPurchase.Rows[k].Cells[GetEnum(GridColIndexes.cQty)].Value == null)
                                SetValue(GetEnum(GridColIndexes.cQty), k, AppSettings.QtyDecimalFormat);
                            if (dgvPurchase.Rows[k].Cells[GetEnum(GridColIndexes.cFree)].Value == null)
                                SetValue(GetEnum(GridColIndexes.cFree), k, AppSettings.QtyDecimalFormat);

                            mytaxable = Comm.ToDouble(dgvPurchase.Rows[k].Cells[GetEnum(GridColIndexes.ctaxable)].Value) + Comm.ToDouble(dgvPurchase.Rows[k].Cells[GetEnum(GridColIndexes.cNonTaxable)].Value);
                            MyPRate = 0;
                            perpieceaddcharges = 0;
                        MyQty = Comm.ToDouble(dgvPurchase.Rows[k].Cells[GetEnum(GridColIndexes.cQty)].Value);// + Comm.ToDouble(dgvPurchase.Rows[k].Cells[GetEnum(GridColIndexes.cFree)].Value);
                        //Dipu on 25-May-2022 -- Free Value Commented
                            if ((dbltaxableAmountTot + dblNontaxableAmountTot) > 0)
                            {
                                if (MyQty > 0)
                                    perpieceaddcharges = (AdditionalCharges / (dbltaxableAmountTot + dblNontaxableAmountTot) * mytaxable) / (Comm.ToDouble(dgvPurchase.Rows[k].Cells[GetEnum(GridColIndexes.cQty)].Value) + Comm.ToDouble(dgvPurchase.Rows[k].Cells[GetEnum(GridColIndexes.cFree)].Value));
                            }
                            //perpieceaddcharges = (AdditionalCharges / (dbltaxableAmountTot + dblNontaxableAmountTot) * mytaxable) / Comm.ToDouble(dgvPurchase.Rows[k].Cells[GetEnum(GridColIndexes.cQty)].Value);
                            double MyPrateWithtax = 0;

                            if (mytaxable > 0)
                            {
                                dblGrossValueAfterDiscounts = Comm.ToDouble(dgvPurchase.Rows[k].Cells[GetEnum(GridColIndexes.cGrossValueAfterRateDiscount)].Value);

                            //Discountamount / dblGrossValueAfterDiscountsTot * dblGrossValueAfterDiscounts
                            //MyPRate = mytaxable / Comm.ToDouble(dgvPurchase.Rows[k].Cells[GetEnum(GridColIndexes.cQty)].Value) + Comm.ToDouble(dgvPurchase.Rows[k].Cells[GetEnum(GridColIndexes.cFree)].Value);

                            double CostFactorDistr = ((Convert.ToDouble(Comm.ToDecimal(txtCoolie.Text)) + Convert.ToDouble(Comm.ToDecimal(txtOtherExp.Text)) + Convert.ToDouble(Comm.ToDecimal(txtCostFactor.Text))) / dblGrossValueAfterDiscountsTot) * dblGrossValueAfterDiscounts;

                                MyPRate = (mytaxable + CostFactorDistr) / Comm.ToDouble(dgvPurchase.Rows[k].Cells[GetEnum(GridColIndexes.cQty)].Value);
                                MyPrateWithtax = (mytaxable + Comm.ToDouble(dgvPurchase.Rows[k].Cells[GetEnum(GridColIndexes.ctax)].Value)) / (Comm.ToDouble(dgvPurchase.Rows[k].Cells[GetEnum(GridColIndexes.cQty)].Value));
                            }

                            //Distributing CommonValues Betweeen Items

                            SetValue(GetEnum(GridColIndexes.cPrate), k, Comm.FormatValue(Comm.ToDouble(dgvPurchase.Rows[k].Cells[GetEnum(GridColIndexes.cPrate)].Value))); //cRate <--> cPrate
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
                                double dblcSRate1Per = Comm.ToDouble(dgvPurchase.Rows[k].Cells[GetEnum(GridColIndexes.cSRate1Per)].Value);
                                double dblcsRate2Per = Comm.ToDouble(dgvPurchase.Rows[k].Cells[GetEnum(GridColIndexes.cSRate2Per)].Value);
                                double dblcsRate3Per = Comm.ToDouble(dgvPurchase.Rows[k].Cells[GetEnum(GridColIndexes.cSRate3Per)].Value);
                                double dblcsRate4Per = Comm.ToDouble(dgvPurchase.Rows[k].Cells[GetEnum(GridColIndexes.cSRate4Per)].Value);
                                double dblcsRate5Per = Comm.ToDouble(dgvPurchase.Rows[k].Cells[GetEnum(GridColIndexes.cSRate5Per)].Value);

                                double dblcRate = Comm.ToDouble(dgvPurchase.Rows[k].Cells[GetEnum(GridColIndexes.cPrate)].Value);
                                double dblcCRate = Comm.ToDouble(dgvPurchase.Rows[k].Cells[GetEnum(GridColIndexes.cCrate)].Value);
                                double dblcMRP = Comm.ToDouble(dgvPurchase.Rows[k].Cells[GetEnum(GridColIndexes.cMRP)].Value);
                                double dblcCRateWithTax = Comm.ToDouble(dgvPurchase.Rows[k].Cells[GetEnum(GridColIndexes.cCRateWithTax)].Value);

                            if (dgvPurchase.Rows[k].Cells[GetEnum(GridColIndexes.cSRate1)].Tag == null)
                                dgvPurchase.Rows[k].Cells[GetEnum(GridColIndexes.cSRate1)].Tag = "";
                            if (dgvPurchase.Rows[k].Cells[GetEnum(GridColIndexes.cSRate2)].Tag == null)
                                dgvPurchase.Rows[k].Cells[GetEnum(GridColIndexes.cSRate2)].Tag = "";
                            if (dgvPurchase.Rows[k].Cells[GetEnum(GridColIndexes.cSRate3)].Tag == null)
                                dgvPurchase.Rows[k].Cells[GetEnum(GridColIndexes.cSRate3)].Tag = "";
                            if (dgvPurchase.Rows[k].Cells[GetEnum(GridColIndexes.cSRate4)].Tag == null)
                                dgvPurchase.Rows[k].Cells[GetEnum(GridColIndexes.cSRate4)].Tag = "";
                            if (dgvPurchase.Rows[k].Cells[GetEnum(GridColIndexes.cSRate5)].Tag == null)
                                dgvPurchase.Rows[k].Cells[GetEnum(GridColIndexes.cSRate5)].Tag = "";

                                switch (Comm.ToDouble(dgvPurchase.Rows[k].Cells[GetEnum(GridColIndexes.cSRate1Per)].Tag)) //DiscMode
                                {
                                    case 0:
                                        if (dblcSRate1Per > 0 && dgvPurchase.Rows[k].Cells[GetEnum(GridColIndexes.cSRate1)].Tag.ToString() == "") SetValue(GetEnum(GridColIndexes.cSRate1), k, Comm.FormatValue((dblcRate + dblcRate * dblcSRate1Per / 100)));
                                        if (dblcsRate2Per > 0 && dgvPurchase.Rows[k].Cells[GetEnum(GridColIndexes.cSRate2)].Tag.ToString() == "") SetValue(GetEnum(GridColIndexes.cSRate2), k, Comm.FormatValue((dblcRate + dblcRate * dblcsRate2Per / 100)));
                                        if (dblcsRate3Per > 0 && dgvPurchase.Rows[k].Cells[GetEnum(GridColIndexes.cSRate3)].Tag.ToString() == "") SetValue(GetEnum(GridColIndexes.cSRate3), k, Comm.FormatValue((dblcRate + dblcRate * dblcsRate3Per / 100)));
                                        if (dblcsRate4Per > 0 && dgvPurchase.Rows[k].Cells[GetEnum(GridColIndexes.cSRate4)].Tag.ToString() == "") SetValue(GetEnum(GridColIndexes.cSRate4), k, Comm.FormatValue((dblcRate + dblcRate * dblcsRate4Per / 100)));
                                        if (dblcsRate5Per > 0 && dgvPurchase.Rows[k].Cells[GetEnum(GridColIndexes.cSRate5)].Tag.ToString() == "") SetValue(GetEnum(GridColIndexes.cSRate5), k, Comm.FormatValue((dblcRate + dblcRate * dblcsRate5Per / 100)));
                                        break;
                                    case 3:
                                        if (dblcSRate1Per > 0 && dgvPurchase.Rows[k].Cells[GetEnum(GridColIndexes.cSRate1)].Tag.ToString() == "") SetValue(GetEnum(GridColIndexes.cSRate1), k, Comm.FormatValue(dblcCRate + dblcCRate * dblcSRate1Per / 100));
                                        if (dblcsRate2Per > 0 && dgvPurchase.Rows[k].Cells[GetEnum(GridColIndexes.cSRate2)].Tag.ToString() == "") SetValue(GetEnum(GridColIndexes.cSRate2), k, Comm.FormatValue(dblcCRate + dblcCRate * dblcsRate2Per / 100));
                                        if (dblcsRate3Per > 0 && dgvPurchase.Rows[k].Cells[GetEnum(GridColIndexes.cSRate3)].Tag.ToString() == "") SetValue(GetEnum(GridColIndexes.cSRate3), k, Comm.FormatValue(dblcCRate + dblcCRate * dblcsRate3Per / 100));
                                        if (dblcsRate4Per > 0 && dgvPurchase.Rows[k].Cells[GetEnum(GridColIndexes.cSRate4)].Tag.ToString() == "") SetValue(GetEnum(GridColIndexes.cSRate4), k, Comm.FormatValue(dblcCRate + dblcCRate * dblcsRate4Per / 100));
                                        if (dblcsRate5Per > 0 && dgvPurchase.Rows[k].Cells[GetEnum(GridColIndexes.cSRate5)].Tag.ToString() == "") SetValue(GetEnum(GridColIndexes.cSRate5), k, Comm.FormatValue(dblcCRate + dblcCRate * dblcsRate5Per / 100));
                                        break;
                                    case 1:
                                        if (dblcSRate1Per > 0 && dgvPurchase.Rows[k].Cells[GetEnum(GridColIndexes.cSRate1)].Tag.ToString() == "") SetValue(GetEnum(GridColIndexes.cSRate1), k, Comm.FormatValue(dblcMRP - dblcMRP * dblcSRate1Per / 100));
                                        if (dblcsRate2Per > 0 && dgvPurchase.Rows[k].Cells[GetEnum(GridColIndexes.cSRate2)].Tag.ToString() == "") SetValue(GetEnum(GridColIndexes.cSRate2), k, Comm.FormatValue(dblcMRP - dblcMRP * dblcsRate2Per / 100));
                                        if (dblcsRate3Per > 0 && dgvPurchase.Rows[k].Cells[GetEnum(GridColIndexes.cSRate3)].Tag.ToString() == "") SetValue(GetEnum(GridColIndexes.cSRate3), k, Comm.FormatValue(dblcMRP - dblcMRP * dblcsRate3Per / 100));
                                        if (dblcsRate4Per > 0 && dgvPurchase.Rows[k].Cells[GetEnum(GridColIndexes.cSRate4)].Tag.ToString() == "") SetValue(GetEnum(GridColIndexes.cSRate4), k, Comm.FormatValue(dblcMRP - dblcMRP * dblcsRate4Per / 100));
                                        if (dblcsRate5Per > 0 && dgvPurchase.Rows[k].Cells[GetEnum(GridColIndexes.cSRate5)].Tag.ToString() == "") SetValue(GetEnum(GridColIndexes.cSRate5), k, Comm.FormatValue(dblcMRP - dblcMRP * dblcsRate5Per / 100));
                                        break;
                                    case 2:
                                        if (dblcSRate1Per > 0 && dgvPurchase.Rows[k].Cells[GetEnum(GridColIndexes.cSRate1)].Tag.ToString() == "") SetValue(GetEnum(GridColIndexes.cSRate1), k, Comm.FormatValue(dblcCRateWithTax + dblcCRateWithTax * dblcSRate1Per / 100));
                                        if (dblcsRate2Per > 0 && dgvPurchase.Rows[k].Cells[GetEnum(GridColIndexes.cSRate2)].Tag.ToString() == "") SetValue(GetEnum(GridColIndexes.cSRate2), k, Comm.FormatValue(dblcCRateWithTax + dblcCRateWithTax * dblcsRate2Per / 100));
                                        if (dblcsRate3Per > 0 && dgvPurchase.Rows[k].Cells[GetEnum(GridColIndexes.cSRate3)].Tag.ToString() == "") SetValue(GetEnum(GridColIndexes.cSRate3), k, Comm.FormatValue(dblcCRateWithTax + dblcCRateWithTax * dblcsRate3Per / 100));
                                        if (dblcsRate4Per > 0 && dgvPurchase.Rows[k].Cells[GetEnum(GridColIndexes.cSRate4)].Tag.ToString() == "") SetValue(GetEnum(GridColIndexes.cSRate4), k, Comm.FormatValue(dblcCRateWithTax + dblcCRateWithTax * dblcsRate4Per / 100));
                                        if (dblcsRate5Per > 0 && dgvPurchase.Rows[k].Cells[GetEnum(GridColIndexes.cSRate5)].Tag.ToString() == "") SetValue(GetEnum(GridColIndexes.cSRate5), k, Comm.FormatValue(dblcCRateWithTax + dblcCRateWithTax * dblcsRate5Per / 100));
                                        break;
                                }

                            dgvPurchase.Rows[k].Cells[GetEnum(GridColIndexes.cSRate1)].Tag = "";
                            dgvPurchase.Rows[k].Cells[GetEnum(GridColIndexes.cSRate2)].Tag = "";
                            dgvPurchase.Rows[k].Cells[GetEnum(GridColIndexes.cSRate3)].Tag = "";
                            dgvPurchase.Rows[k].Cells[GetEnum(GridColIndexes.cSRate4)].Tag = "";
                            dgvPurchase.Rows[k].Cells[GetEnum(GridColIndexes.cSRate5)].Tag = "";
                        }

                        //double SavingsofItem = (Comm.ToDouble(dgvPurchase.Rows[k].Cells[GetEnum(GridColIndexes.cMRP)].Value) * MyQty) - (Val(DGVItem.Item(cRate, i).Value) * MyQty);
                        SavingsofItem = (Comm.ToDouble(dgvPurchase.Rows[k].Cells[GetEnum(GridColIndexes.cMRP)].Value) * MyQty) - (Comm.ToDouble(dgvPurchase.Rows[k].Cells[GetEnum(GridColIndexes.cPrate)].Value) * MyQty); //cRate <--> cPrate
                            if (MyQty > 0) Savings = Savings + SavingsofItem;

                        //}
                    }
                }
            }





            //ItemDiscount and Discount Amount are equal ??
            Savings = Savings + Comm.ToDouble(txtDiscAmt.Text) + Comm.ToDouble(txtDiscAmt.Text) + Comm.ToDouble(txtCashDisc.Text) - Comm.ToDouble(txtOtherExp.Text);
            // dgv.SetValue(LinkIDs.Savings, Comm.FormatAmt(Val(Val(Savings)), DCSApp.Gdecimal))

            if (Comm.ToDouble(lblBillAmount.Text) > 1000000000)
            {
                //NotifyIcon("Sales Value Calculation", "Software is unable to calculate higher values than 100000000. Kindly limit the values or correct if given by mistake")
                MessageBox.Show("Software is unable to calculate higher values than 100000000. Kindly limit the values or correct if given by mistake", "Sales Value Calculation");
                lblBillAmount.Text = "000";
            }

            //WriteToPoleDisplay(StrLastAddeddItemForPOleDisplay, "Amount :" & lblBalance.Text)
            //'Dim NoConv As New DcsDll.NoConversion
            //' NotifyIcon("", NoConv.NoConvertion(lblBalance.Text, True, "Rupees", "RS", False))
            //' Dim t As New Translator()
            //'txtInwords.Text = t.Translate(txtInwords.Text, "English", "Malayalam")
            //Me.Text = mytrans.MVchType & " .............. [" & IIf(mytrans.BlnEditMode, "Edit Mode", "New Mode") & "] ................VchNo : " & txtvchnoPrefix.Text.ToString & txtVchNo.Text.ToString & "............Party : " & txtpartySearch.Text
            //mecaption.Text = Me.Text
            //}
        }


        private void CallBatchCodeCompact(bool bWhenPressDownKey = false)
        {
            bool blnAutoCodeNeeded = false;
            string sQuery = "";

            if (Comm.ToInt32(dgvPurchase.Rows[dgvPurchase.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cBatchMode)].Value) == 0) // None
                blnAutoCodeNeeded = false;
            else if (Comm.ToInt32(dgvPurchase.Rows[dgvPurchase.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cBatchMode)].Value) == 1) // MNF
                blnAutoCodeNeeded = true;
            else if (Comm.ToInt32(dgvPurchase.Rows[dgvPurchase.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cBatchMode)].Value) == 2) // Auto
                blnAutoCodeNeeded = true;
            else if (Comm.ToInt32(dgvPurchase.Rows[dgvPurchase.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cBatchMode)].Value) == 3) // WMH
                blnAutoCodeNeeded = false;

            //string sQuery = "SELECT AnyWhere,BatchCode,ExpiryDate,MRP,QOH,StockID,ItemID FROM (SELECT (BatchCode+CONVERT(VARCHAR,ExpiryDate)+CONVERT(VARCHAR,ISNULL(QOH,0))+CONVERT(VARCHAR,ISNULL(MRP,0))) as AnyWhere,BatchCode,ExpiryDate,MRP,QOH,StockID,ItemID FROM tblStock )A WHERE A.ItemID = " + Comm.ToInt32(dgvPurchase.Rows[dgvPurchase.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cItemID)].Value.ToString()) + "";
            sQuery = "SELECT AnyWhere,BatchCode,ExpiryDate,MRP,QOH,StockID,ItemID FROM (";
            sQuery = sQuery + "SELECT (BatchCode+CONVERT(VARCHAR,ExpiryDate)+CONVERT(VARCHAR,ISNULL(QOH,0))+CONVERT(VARCHAR,ISNULL(MRP,0))) as AnyWhere,BatchUnique as BatchCode,ExpiryDate,MRP,QOH,StockID,ItemID FROM tblStock ";

            if (blnAutoCodeNeeded == true)
            {
                if (Comm.ToInt32(dgvPurchase.Rows[dgvPurchase.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cBatchMode)].Value) == 1)// MNF
                {
                    if (bWhenPressDownKey == true)
                        sQuery = sQuery + " UNION ALL SELECT '<Auto Barcode>' as AnyWhere,'<Auto Barcode>' as BatchCode,'23-Feb-2030' as ExpiryDate,0 as MRP,0 as QOH,0 as StockID," + Comm.ToInt32(dgvPurchase.Rows[dgvPurchase.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cItemID)].Value.ToString()) + " as ItemID";
                }
                else if (Comm.ToInt32(dgvPurchase.Rows[dgvPurchase.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cBatchMode)].Value) == 2)// Auto
                {
                    sQuery = sQuery + " UNION ALL SELECT '<Auto Barcode>' as AnyWhere,'<Auto Barcode>' as BatchCode,'23-Feb-2030' as ExpiryDate,0 as MRP,0 as QOH,0 as StockID," + Comm.ToInt32(dgvPurchase.Rows[dgvPurchase.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cItemID)].Value.ToString()) + " as ItemID";
                }
            }

            if (Comm.ToInt32(dgvPurchase.Rows[dgvPurchase.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cBatchMode)].Value) == 1 || Comm.ToInt32(dgvPurchase.Rows[dgvPurchase.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cBatchMode)].Value) == 2)// MNF & AUto
            {
                sQuery = sQuery + " )A WHERE A.ItemID = " + Comm.ToDecimal(dgvPurchase.Rows[dgvPurchase.CurrentRow.Index].Cells[GetEnum(GridColIndexes.cItemID)].Value.ToString()) + "";
                //frmBatchSearch = new frmCompactSearch(GetFromBatchCodeSearch, sQuery, "Anywhere|BatchCode|CONVERT(VARCHAR,ExpiryDate)|CONVERT(VARCHAR,ISNULL(MRP,0))|CONVERT(VARCHAR,ISNULL(QOH,0))", dgvPurchase.Location.X + 350, dgvPurchase.Location.Y + 150, 4, 0, "", 4, 0, "ORDER BY A.StockID ASC", 0, 0, "BatchCode Search...", 0, "150,90,80,80,0,0", true, "Bar Code", 10);
                frmBatchSearch = new frmCompactSearch(GetFromBatchCodeSearch, sQuery, "Anywhere|BatchCode|CONVERT(VARCHAR,ExpiryDate)|CONVERT(VARCHAR,ISNULL(MRP,0))|CONVERT(VARCHAR,ISNULL(QOH,0))", dgvPurchase.Location.X + 350, dgvPurchase.Location.Y + 150, 4, 0, "", 4, 0, "ORDER BY A.StockID ASC", 0, 0, "BatchCode Search...", 0, "150,90,80,80,0,0", true, "Bar Code", 10);
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
                    txtInvAutoNo.Text = Comm.gfnGetNextSerialNo("tblPurchase", "AutoNum", " VchTypeID=" + vchtypeID).ToString();
                    txtReferenceAutoNo.Tag = txtInvAutoNo.Text;
                    txtInvAutoNo.Tag = 0;
                }  
                txtInvAutoNo.ReadOnly = true;
                txtPrefix.ReadOnly = true;
            }
            else if (Comm.ConvertI32(clsVchType.TransactionNumberingValue) == 1) // Auto Editable
            {
                if (iIDFromEditWindow == 0) //New
                {
                    //txtInvAutoNo.Text = Comm.gfnGetNextSerialNo("tblPurchase", "AutoNum").ToString();
                    txtInvAutoNo.Text = Comm.gfnGetNextSerialNo("tblPurchase", "AutoNum", " VchTypeID=" + vchtypeID).ToString();
                    txtReferenceAutoNo.Tag = txtInvAutoNo.Text;
                    txtInvAutoNo.Tag = 0;
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

            if (clsVchType.blnBillWiseDiscPercentageandAmt == 1) // Enable Bill Discount
            {
                tblpDiscPerc.Enabled = true;
                tblpDiscAmt.Enabled = true;
            }
            else
            {
                if (clsVchType.blnBillWiseDiscPercentage == 1)
                    tblpDiscPerc.Enabled = true;
                else
                    tblpDiscPerc.Enabled = false;

                if (clsVchType.btnBillWiseDiscAmount == 1)
                    tblpDiscAmt.Enabled = true;
                else
                    tblpDiscAmt.Enabled = false;
            }

            if (clsVchTypeFeatures.blnenablecashdiscount == true)
            {
                tblpCashDicper.Enabled = true;
                tblpCashDisc.Enabled = true;
            }
            else
            {
                tblpCashDicper.Enabled = false;
                tblpCashDisc.Enabled = false;
            }

            if (clsVchTypeFeatures.blnenableEffeciveDate == true)
            {
                lblEffectiveDate.Visible = true;
                dtpEffective.Visible = true;
            }
            else
            {
                lblEffectiveDate.Visible = false;
                dtpEffective.Visible = false;
            }

            if (clsVchTypeFeatures.blnpartydetails == true)
            {
                lblSuppTaxReg.Visible = true;
                lblSuppAddress.Visible = true;
                lblSuppBType.Visible = true;
                lblSuppState.Visible = true;

                txtTaxRegn.Visible = true;
                txtAddress1.Visible = true;
                cboBType.Visible = true;
                cboState.Visible = true;
            }
            else
            {
                lblSuppTaxReg.Visible = false;
                lblSuppAddress.Visible = false;
                lblSuppBType.Visible = false;
                lblSuppState.Visible = false;

                txtTaxRegn.Visible = false;
                txtAddress1.Visible = false;
                cboBType.Visible = false;
                cboState.Visible = false;
            }

            if (clsVchTypeFeatures.blnshowbillnarration == true)
                tblpNarration.Visible = true;
            else
                tblpNarration.Visible = false;

            if (clsVchTypeFeatures.blnshowotherexpense)
                tblpOtherExp.Visible = true;
            else
                tblpOtherExp.Visible = false;

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
                    txtReferenceAutoNo.Text = Comm.gfnGetNextSerialNo("tblPurchase", "ReferenceAutoNO").ToString();
                txtReferenceAutoNo.ReadOnly = true;
                txtReferencePrefix.ReadOnly = true;
                txtReferencePrefix.Width = 55;
            }
            else if (Comm.ConvertI32(clsVchType.ReferenceNumberingValue) == 1) // Auto Editable
            {
                if (iIDFromEditWindow == 0)
                    txtReferenceAutoNo.Text = Comm.gfnGetNextSerialNo("tblPurchase", "ReferenceAutoNO").ToString();
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

            if (clsVchType.ParentID == 1005)
            {
                grpLedger.Visible = false;
                txtReferenceAutoNo.Visible = false;
                lblReferenceNo.Visible = false;
                txtReferencePrefix.Visible = false;
            }

            ////--------------------------------------------------------------////

            if (clsVchType.blnPrimaryLockWithSelection == 1)
                cboCostCentre.Enabled = false;
            else
                cboCostCentre.Enabled = true;

            if (clsVchType.blnTaxModeLockWSel == 1)
                cboTaxMode.Enabled = false;
            else
                cboTaxMode.Enabled = true;

            if (clsVchType.blnModeofPaymentLockWSel == 1)
                cboPayment.Enabled = false;
            else
                cboPayment.Enabled = true;

            if (clsVchType.blnSaleStaffLockWSel == 1)
                cboSalesStaff.Enabled = false;
            else
                cboSalesStaff.Enabled = true;

            if (clsVchType.blnAgentLockWSel == 1)
                cboAgent.Enabled = false;
            else
                cboAgent.Enabled = true;

            if (clsVchType.DefaultTaxModeValue == 3) //GST
            {
                if (dgvPurchase.Columns.Count > 0)
                {
                    dgvPurchase.Columns["cCGST"].Visible = true;
                    dgvPurchase.Columns["cSGST"].Visible = true;
                    dgvPurchase.Columns["cIGST"].Visible = true;
                    dgvPurchase.Columns["ctaxPer"].Visible = true;
                    dgvPurchase.Columns["ctax"].Visible = true;
                    dgvPurchase.Columns["ctaxable"].Visible = true;
                    dgvPurchase.Columns["cCRateWithTax"].Visible = true;
                    tblpTaxAmt.Visible = true;
                    tblpTaxable.Visible = true;
                }
            }
            else
            {
                if (dgvPurchase.Columns.Count > 0)
                {
                    dgvPurchase.Columns["cCGST"].Visible = false;
                    dgvPurchase.Columns["cSGST"].Visible = false;
                    dgvPurchase.Columns["cIGST"].Visible = false;
                    dgvPurchase.Columns["ctaxPer"].Visible = false;
                    dgvPurchase.Columns["ctax"].Visible = false;
                    dgvPurchase.Columns["ctaxable"].Visible = false;
                    dgvPurchase.Columns["cCRateWithTax"].Visible = false;
                    tblpTaxAmt.Visible = false;
                    tblpTaxable.Visible = false;
                }
            }

            if (iIDFromEditWindow == 0) //New
                GetAgentDiscountAsperVoucherType();

            if (clsVchType.ItmWiseDiscFillXtraDiscFromValue == 2) //Item Discount
            { }
            else if (clsVchType.ItmWiseDiscFillXtraDiscFromValue == 3) //Category Discount
            { }
            else if (clsVchType.ItmWiseDiscFillXtraDiscFromValue == 4) //Manufacturer Discount
            { }
            else if (clsVchType.ItmWiseDiscFillXtraDiscFromValue == 5) //Discount Group Discount
            { }
            else if (clsVchType.ItmWiseDiscFillXtraDiscFromValue == 6) //Highest Discount
            { }
        }

        //Description : Setting Transactions that Varying to the form
        private void SetTransactionsthatVarying()
        {
            if (Comm.ConvertI32(clsVchType.DefaultModeofPaymentValue) == 1) // Cash
                cboPayment.SelectedIndex = 0;
            else if (Comm.ConvertI32(clsVchType.DefaultModeofPaymentValue) == 2) // Credit
                cboPayment.SelectedIndex = 1;
            //else if (Comm.ConvertI32(clsVchType.DefaultModeofPaymentValue) == 3) // Mixed
            //    cboPayment.SelectedIndex = 2;
            //else if (Comm.ConvertI32(clsVchType.DefaultModeofPaymentValue) == 4) // Cash Counter
            //    cboPayment.SelectedIndex = 3;

            cboTaxMode.SelectedValue = Comm.ConvertI32(clsVchType.DefaultTaxModeValue);
            cboCostCentre.SelectedValue = Comm.ConvertI32(clsVchType.PrimaryCCValue);
            cboSalesStaff.SelectedValue = Comm.ConvertI32(clsVchType.DefaultSaleStaffValue);
            cboAgent.SelectedValue = Comm.ConvertI32(clsVchType.DefaultAgentValue);
            GetAgentDiscountAsperVoucherType();
        }

        //Description : Setting asper Application Settings
        private void SetApplicationSettings()
        {
            if (dgvPurchase.Columns.Count > 0)
            {
                if (AppSettings.TaxEnabled == true)
                {
                    if (AppSettings.TaxMode == 0) //No Tax
                    {
                        cboTaxMode.SelectedValue = 1; //none
                        tblpTaxAmt.Visible = false;
                        tblpTaxable.Visible = false;

                        dgvPurchase.Columns["cCGST"].Visible = false;
                        dgvPurchase.Columns["cSGST"].Visible = false;
                        dgvPurchase.Columns["cIGST"].Visible = false;
                        dgvPurchase.Columns["ctaxPer"].Visible = false;
                        dgvPurchase.Columns["ctax"].Visible = false;
                        dgvPurchase.Columns["ctaxable"].Visible = false;
                        dgvPurchase.Columns["cCRateWithTax"].Visible = false;
                    }
                    else if (AppSettings.TaxMode == 1) //VAT
                    {
                        cboTaxMode.SelectedValue = 2; //VAT
                        tblpTaxAmt.Visible = true;
                        tblpTaxable.Visible = true;
                        pnlTaxMode.Visible = true;

                        dgvPurchase.Columns["cCGST"].Visible = false;
                        dgvPurchase.Columns["cSGST"].Visible = false;

                        dgvPurchase.Columns["cIGST"].Visible = true;
                        dgvPurchase.Columns["ctaxPer"].Visible = true;
                        dgvPurchase.Columns["ctax"].Visible = true;
                        dgvPurchase.Columns["ctaxable"].Visible = true;
                        dgvPurchase.Columns["cCRateWithTax"].Visible = true;
                    }
                    else
                    {
                        dgvPurchase.Columns["cCGST"].Visible = true;
                        dgvPurchase.Columns["cSGST"].Visible = true;
                        dgvPurchase.Columns["cIGST"].Visible = true;
                        dgvPurchase.Columns["ctaxPer"].Visible = true;
                        dgvPurchase.Columns["ctax"].Visible = true;
                        dgvPurchase.Columns["ctaxable"].Visible = true;
                        dgvPurchase.Columns["cCRateWithTax"].Visible = true;

                        pnlTaxMode.Visible = true;
                        cboTaxMode.SelectedValue = AppSettings.TaxMode + 1;

                        tblpTaxAmt.Visible = true;
                        tblpTaxable.Visible = true;
                    }
                }
                else
                {
                    dgvPurchase.Columns[GetEnum(GridColIndexes.cCGST)].Visible = false;
                    dgvPurchase.Columns[GetEnum(GridColIndexes.cSGST)].Visible = false;
                    dgvPurchase.Columns[GetEnum(GridColIndexes.cIGST)].Visible = false;
                    dgvPurchase.Columns[GetEnum(GridColIndexes.ctaxPer)].Visible = false;
                    dgvPurchase.Columns[GetEnum(GridColIndexes.ctax)].Visible = false;
                    dgvPurchase.Columns[GetEnum(GridColIndexes.ctaxable)].Visible = false;
                    dgvPurchase.Columns[GetEnum(GridColIndexes.cCRateWithTax)].Visible = false;

                    pnlTaxMode.Visible = false;
                    cboTaxMode.SelectedValue = 1; //none
                    tblpTaxAmt.Visible = false;
                    tblpTaxable.Visible = false;
                }
            }

            if (AppSettings.NeedAgent == true)
                pnlAgent.Visible = true;
            else
                pnlAgent.Visible = false;

            if (dgvPurchase.Columns.Count > 0)
            {
                if (AppSettings.CessMode == 0)
                {
                    tblpCess.Visible = false;
                    tblpCompCess.Visible = false;
                    tblpQtyCess.Visible = false;

                    dgvPurchase.Columns[GetEnum(GridColIndexes.cCCessPer)].Visible = false;
                    dgvPurchase.Columns[GetEnum(GridColIndexes.cCCompCessQty)].Visible = false;
                    dgvPurchase.Columns[GetEnum(GridColIndexes.cFloodCessAmt)].Visible = false;
                    dgvPurchase.Columns[GetEnum(GridColIndexes.cFloodCessPer)].Visible = false;
                }
                else
                {
                    tblpCess.Visible = true;
                    tblpCompCess.Visible = true;
                    tblpQtyCess.Visible = true;

                    dgvPurchase.Columns[GetEnum(GridColIndexes.cCCessPer)].Visible = true;
                    dgvPurchase.Columns[GetEnum(GridColIndexes.cCCompCessQty)].Visible = true;
                    dgvPurchase.Columns[GetEnum(GridColIndexes.cFloodCessAmt)].Visible = true;
                    dgvPurchase.Columns[GetEnum(GridColIndexes.cFloodCessPer)].Visible = true;
                }
            }

            if (AppSettings.NeedCostCenter == true)
                pnlCostCentre.Visible = true;
            else
                pnlCostCentre.Visible = false;

            

            dtpInvDate.MinDate = AppSettings.FinYearStart;
            dtpInvDate.MaxDate = AppSettings.FinYearEnd;
            //dtpInvDate.MaxDate = Convert.ToDateTime(AppSettings.FinYearEnd).AddDays(1);

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

                dgvPurchase.Columns["cRateinclusive"].Visible = false;
            
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
