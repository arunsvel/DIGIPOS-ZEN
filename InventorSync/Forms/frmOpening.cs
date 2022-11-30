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

    public partial class frmOpening : Form, IMessageFilter
    {
        //=============================================================================
        // Created By       : Dipu Joseph
        // Created On       : 02-Feb-2022
        // Last Edited On   :
        // Last Edited By   :
        // Description      : Working With Different Voucher Type. Mainly For Sales, Sales RETURN, RECEIPT NOTE
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

        private frmCompactSearch frmBatchSearch;

        sqlControl bsdata = new sqlControl();

        public frmOpening(int iVchTpeId = 0, int iTransID = 0, bool bFromEdit = false, object MDIParent = null)
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
                //lblEffectiveDate.ForeColor = Color.Black;
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

            flpMasters.Width = this.Width - 20;

            //this.SetBounds(l, t, this.Width, this.Height);

            //this.WindowState = FormWindowState.Maximized;

            clsVchType = JSonComm.GetVoucherType(iVchTpeId);
            clsVchTypeFeatures = JSonComm.GetVoucherTypeGeneralSettings(iVchTpeId, 1);

            ClearControls();

            bFromEditSales = bFromEdit;
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

            dgvItems.Controls.Add(dtp);
            dtp.Visible = false;
            dtp.Format = DateTimePickerFormat.Custom;
            lblPause.Text = "Pause";

        }

        #region "VARIABLES --------------------------------------------- >>"

        Control ctrl;
        string sEditedValueonKeyPress, sBatchCode = "";
        int iBatchmode, iShelfLifeDays;
        DateTime dtCurrExp;
        int iIDFromEditWindow, vchtypeID;
        bool dragging = false;
        int xOffset = 0, yOffset = 0;
        string strSelectedItemName ="";
        bool bFromEditSales;

        static int namesCount = Enum.GetNames(typeof(LedgerIndexes)).Length;
        string[] sArrLedger = new string[namesCount];
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
        UspGetStockJournalInfo GetStockJournalIfo = new UspGetStockJournalInfo();

        clsItemMaster clsItmMst = new clsItemMaster();
        clsEmployee clsEmp = new clsEmployee();
        clsCostCentre clscct = new clsCostCentre();
        clsTaxMode clsTax = new clsTaxMode();
        clsAgentMaster clsAgent = new clsAgentMaster();
        clsLedger clsLedg = new clsLedger();
        clsUnitMaster clsUnit = new clsUnitMaster();
        clsStockDetails clsStock = new clsStockDetails();

        clsJSonCommon JSonComm = new clsJSonCommon();
        clsStockJournal clsPur = new clsStockJournal();

        //Sales Master Related Classes for Json
        clsJSonStockJournal clsPM = new clsJSonStockJournal();
        clsJsonPMInfo clsJPMinfo = new clsJsonPMInfo();
        clsJsonPMCCentreInfo clsJPMCostCentreinfo = new clsJsonPMCCentreInfo();
        clsJsonPMDestCCentreInfo clsJPMDestCostCentreinfo = new clsJsonPMDestCCentreInfo();
        clsJsonPMEmployeeInfo clsJPMEmployeeinfo = new clsJsonPMEmployeeInfo();

        //Sales Detail Related Classes For Json
        clsJsonSJDetailsInfo clsJSJDinfo = new clsJsonSJDetailsInfo();
        clsJsonPDUnitinfo clsJPDUnitinfo = new clsJsonPDUnitinfo();
        clsJsonPDIteminfo clsJPDIteminfo = new clsJsonPDIteminfo();

        DataTable dtItemPublic = new DataTable();
        DataTable dtUnitPublic = new DataTable();
        DataTable dtBatchCode = new DataTable();
        DataTable dtBatchCodeData = new DataTable();

        DateTimePicker dtp = new DateTimePicker();

        clsJsonVoucherType clsVchType = new clsJsonVoucherType();
        clsGetStockInVoucherSettings clsVchTypeFeatures = new clsGetStockInVoucherSettings();

        private OpeningGridColIndexes gridColIndexes = new OpeningGridColIndexes();


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

                gridColIndexes.ChangeBarcodeMode(clsVchType.DefaultBarcodeMode);

                if (iIDFromEditWindow == 0)
                {
                    AddColumnsToGrid();
                    FillCostCentre();
                }
                else
                {
                    btnSave.Enabled = false;
                    btnDelete.Enabled = false;
                    btnArchive.Enabled = false;
                    btnPause.Enabled = false;
                }

                SetTransactionDefaults();
                SetApplicationSettings();

                Application.DoEvents();

                if (toggleWidthSettings.ToggleState == Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active)
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
                    int iRowCnt = dgvItems.Rows.Count;
                    dgvItems.CurrentCell = dgvItems.Rows[iRowCnt - 1].Cells[GetEnum(gridColIndexes.CItemCode)];
                    dgvItems.Focus();
                    SendKeys.Send("{down}");
                }
                dgvItems.Columns["cRateinclusive"].Visible = false;
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

        ComboBox BatchCode_GridCellComboBox = new ComboBox();

        private void gridColumn_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.') && ((int)gridColIndexes.cBarCode != dgvItems.CurrentCell.ColumnIndex))
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
                        sQuery = "SELECT ItemCode + ItemName + BatchUnique + CAST(MRP AS varchar) AS anywhere, ItemCode, ItemName, BatchUnique, QOH, MRP, ItemID, StockID " +
                                " FROM     vwCompactSearch Where isnull(ActiveStatus, 1) = 1 and isnull(StockActiveStatus, 1) = 1 ";
                        if (clsVchType.ProductClassList != "")
                            sQuery = sQuery + " AND ProductTypeID IN (" + clsVchType.ProductClassList + ")";
                        if (clsVchType.ItemCategoriesList != "")
                            sQuery = sQuery + " AND CategoryID IN (" + clsVchType.ItemCategoriesList + ")";

                        if (mblnInitialisedSubWindow == false)
                        {
                            mblnInitialisedSubWindow = true;
                            frmDetailedSearch2 frmN = new frmDetailedSearch2(GetFromItemSearch, sQuery, "Anywhere|ItemCode|ItemName|BatchUnique", dgvItems.Location.X + 55, dgvItems.Location.Y + 150, 7, 0, sEditedValueonKeyPress, 6, 0, "ORDER BY ItemCode ASC, Batchunique", 0, 0, "Item Search...", 0, "150,250,150,150,150,0,0", true, "frmItemMaster", 20, false, this.MdiParent, 6);
                            frmN.MdiParent = this.MdiParent;
                            frmN.Show(); //20-Aug-2022
                        }

                        if (dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cItemID)].Value != null)
                        {
                            this.dgvItems.EditingControlShowing -= this.dgvItems_EditingControlShowing;
                            dgvItems.CurrentCell = dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cBarCode)];
                            dgvItems.Focus();
                            this.dgvItems.EditingControlShowing += this.dgvItems_EditingControlShowing;
                        }
                    }
                }
                else if (dgvItems.CurrentCell.ColumnIndex == (int)gridColIndexes.cBarCode)
                {
                    if ((int)gridColIndexes.cBarCode > 1)
                    {
                        //sEditedValueonKeyPress = "~";
                        if (dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[(int)gridColIndexes.cBarCode].Value != null)
                            sEditedValueonKeyPress = dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[(int)gridColIndexes.cBarCode].Value.ToString();
                        else
                            sEditedValueonKeyPress = "";
                        if (sEditedValueonKeyPress != null)
                        {
                            if (dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cItemID)].Value != null)
                            {
                                Form fcC = Application.OpenForms["frmDetailedSearch2"];
                                if (fcC != null)
                                {
                                    fcC.Focus();
                                    fcC.BringToFront();
                                    return;
                                }

                                CallBatchCodeCompact();

                                dgvItems.CurrentCell = dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.CExpiry)];
                                dgvItems.Focus();
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

        private void cboCostCentre_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
            {
                txtReferenceAutoNo.Focus();
                //SendKeys.Send("{F4}");
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
                else if (pnlSalesStaff.Visible == true)
                    cboSalesStaff.Focus();

                SendKeys.Send("{F4}");
            }
            else if (e.KeyCode == Keys.Enter)
            {
                dgvItems.CurrentCell = dgvItems.Rows[0].Cells[GetEnum(gridColIndexes.CItemCode)];
                dgvItems.Focus();
            }
        }

        private void txtNarration_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
            {
                e.Handled = true; //nowhere to navigate back
            }
            else if (e.KeyCode == Keys.Enter)
            {
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                if (iIDFromEditWindow == 0)
                    CRUD_Operations(0);
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
                    else
                    {
                        cboCostCentre.Focus();
                        SendKeys.Send("{F4}");
                    }
                }
            }
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
                DialogResult dlgResult = MessageBox.Show("Are you sure to delete Invoice No [" + txtInvAutoNo.Text + "] Permanently ?", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                if (dlgResult == DialogResult.Yes)
                {
                    CRUD_Operations(2);
                }
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
                        DialogResult dlgResult = MessageBox.Show("Are you sure to delete Item Code [" + strSelectedItemName + "] Permanently ?", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
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
            if (Convert.ToInt32(txtPrefix.Tag.ToString()) == 3)
            {
                MessageBox.Show("This is a Archived Voucher", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void txtInvAutoNo_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtInvAutoNo);
            if (iIDFromEditWindow == 0)
            {
                DataTable dtInv = Comm.fnGetData("SELECT ISNULL(JsonData,'') as JsonData,Invid FROM tblStockJournal WHERE InvNo = '" + txtInvAutoNo.Text + "' AND VchTypeID=" + vchtypeID + " AND TenantID = " + Global.gblTenantID + "").Tables[0];
                if (dtInv.Rows.Count > 0)
                {
                    DialogResult dlgResult = MessageBox.Show("There is an Exisiting Bill Number in this Invoice No [" + txtInvAutoNo.Text + "], Do you want to load it?", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                    if (dlgResult == DialogResult.Yes)
                    {
                        //LoadData(Convert.ToInt32(dtInv.Rows[0]["InvId"].ToString()));
                        iIDFromEditWindow = Convert.ToInt32(dtInv.Rows[0]["InvId"].ToString());
                        DeserializeFromJSon(dtInv.Rows[0]["JsonData"].ToString());
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
                        sData = "UPDATE tblTransactionPause SET LastUpdateDt='" + DateTime.Today + "',UpdateStatus=1,'" + strJson + "' WHERE ID=" + Convert.ToInt32(lblPause.Tag) + " AND TenantID = " + Global.gblTenantID + " AND VchTypeID = " + vchtypeID + " AND VchParentID = 2";
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

        private void txtDiscPerc_Leave(object sender, EventArgs e)
        {
            try
            {
                //if (Convert.ToDecimal(txtDiscAmt.Text) > 0)
                //    CalcTotal();
            }
            catch 
            {
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
                        dtInv = Comm.fnGetData("SELECT TOP 1 ISNULL(InvId,0) FROM tblStockJournal WHERE VchTypeID = " + vchtypeID + " AND TenantID = " + Global.gblTenantID + " ORDER BY InvId DESC").Tables[0];
                        if (dtInv.Rows.Count > 0)
                            dInvId = Convert.ToDecimal(dtInv.Rows[0][0].ToString());
                        else
                            dInvId = 0;
                    }
                    else
                    {
                        dtInv = Comm.fnGetData("SELECT TOP 1 ISNULL(InvId,0) FROM tblStockJournal WHERE InvId < " + Convert.ToDecimal(txtInvAutoNo.Tag.ToString()) + " AND VchTypeID = " + vchtypeID + " AND TenantID = " + Global.gblTenantID + " ORDER BY InvId DESC").Tables[0];
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
                        dtInv = Comm.fnGetData("SELECT TOP 1 ISNULL(InvId,0) FROM tblStockJournal WHERE InvId > " + Convert.ToDecimal(txtInvAutoNo.Tag.ToString()) + " AND VchTypeID = " + vchtypeID + " AND TenantID = " + Global.gblTenantID + " ORDER BY InvId ASC").Tables[0];
                        if (dtInv.Rows.Count > 0)
                            dInvId = Convert.ToDecimal(dtInv.Rows[0][0].ToString());
                        else
                        {
                            dInvId = 0;
                            ClearControls();
                            if (ConvertI32(clsVchType.TransactionNumberingValue) == 0) // Auto Locked
                            {
                                //txtInvAutoNo.Text = Comm.gfnGetNextSerialNo("tblStockJournal", "AutoNum").ToString();
                                txtInvAutoNo.Text = Comm.gfnGetNextSerialNo("tblStockJournal", "AutoNum", " VchTypeID=" + vchtypeID).ToString();
                                txtInvAutoNo.Tag = 0;

                                txtInvAutoNo.ReadOnly = true;
                                txtPrefix.ReadOnly = true;
                            }
                            else if (ConvertI32(clsVchType.TransactionNumberingValue) == 1) // Auto Editable
                            {
                                //txtInvAutoNo.Text = Comm.gfnGetNextSerialNo("tblStockJournal", "AutoNum").ToString();
                                txtInvAutoNo.Text = Comm.gfnGetNextSerialNo("tblStockJournal", "AutoNum", " VchTypeID=" + vchtypeID).ToString();
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

                            if (ConvertI32(clsVchType.ReferenceNumberingValue) == 0) // Auto Locked
                            {
                                if (iIDFromEditWindow == 0)
                                    txtReferenceAutoNo.Text = Comm.gfnGetNextSerialNo("tblStockJournal", "ReferenceAutoNO").ToString();
                                txtReferenceAutoNo.ReadOnly = true;
                                txtReferencePrefix.ReadOnly = true;
                                txtReferencePrefix.Width = 55;
                            }
                            else if (ConvertI32(clsVchType.ReferenceNumberingValue) == 1) // Auto Editable
                            {
                                if (iIDFromEditWindow == 0)
                                    txtReferenceAutoNo.Text = Comm.gfnGetNextSerialNo("tblStockJournal", "ReferenceAutoNO").ToString();
                                txtReferenceAutoNo.ReadOnly = false;
                                txtReferencePrefix.ReadOnly = false;
                                txtReferencePrefix.Width = 55;
                            }
                            else if (ConvertI32(clsVchType.ReferenceNumberingValue) == 2) // Custom
                            {
                                txtReferencePrefix.Visible = true;
                                txtReferenceAutoNo.ReadOnly = true;
                                txtReferencePrefix.ReadOnly = false;
                                txtReferencePrefix.Width = txtReferenceAutoNo.Width;
                            }
                        }

                        if (dInvId == 0)
                            btnNext.Enabled = false;
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

                DataTable dtCct1 = new DataTable();
                dtCct1 = GetCostCentre(0);

                cboDestCostCentre.DataSource = dtCct1;
                cboDestCostCentre.DisplayMember = "Cost Centre Name";
                cboDestCostCentre.ValueMember = "CCID";
                if (iSelID != 0)
                    cboDestCostCentre.SelectedValue = iSelID;
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
                SetValue(GetEnum(gridColIndexes.CExpiry), dtCurrExp.ToString("dd-MMM-yyyy").ToString(), "DATE");
            }
            else
            {
                DataTable dtData = new DataTable();
                GetStockInfo.BatchUnique = sBarUnique;
                GetStockInfo.BatchCode = "";
                GetStockInfo.StockID = 0;
                GetStockInfo.TenantID = Global.gblTenantID;
                GetStockInfo.ItemID = Convert.ToDouble(dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cItemID)].Value);
                GetStockInfo.CCID = Convert.ToDouble(cboCostCentre.SelectedValue);
                dtData = clsStock.GetStockDetails(GetStockInfo);
                if (dtData.Rows.Count > 0)
                {
                    SetValue(GetEnum(gridColIndexes.CExpiry), dtData.Rows[0]["ExpiryDate"].ToString(), "DATE");
                    SetValue(GetEnum(gridColIndexes.cMRP), dtData.Rows[0]["MRP"].ToString(), "CURR_FLOAT");
                    SetValue(GetEnum(gridColIndexes.cSrate), dtData.Rows[0]["PRate"].ToString(), "CURR_FLOAT");

                    //SetValue(GetEnum(gridColIndexes.cSrate), SetPriceListForItems(dgvItems.CurrentRow.Index).ToString(), "CURR_FLOAT");

                    SetValue(GetEnum(gridColIndexes.cGrossAmt), dtData.Rows[0]["PRateExcl"].ToString(), "CURR_FLOAT");
                    SetValue(GetEnum(gridColIndexes.cItemID), dtData.Rows[0]["ItemID"].ToString(), "INT");
                    dQty = Convert.ToDecimal(dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cQty)].Value);
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
                if(dtstock.Rows.Count > 0)
                    sBarUnique = dtstock.Rows[0][0].ToString().Trim();

                GetStockInfo.BatchUnique = sBarUnique;
                GetStockInfo.BatchCode = "";
                GetStockInfo.StockID = iStockID;
                GetStockInfo.TenantID = Global.gblTenantID;
                GetStockInfo.ItemID = Convert.ToDouble(dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cItemID)].Value);
                GetStockInfo.CCID = Convert.ToDouble(cboCostCentre.SelectedValue);
                dtData = clsStock.GetStockDetails(GetStockInfo);
                if (dtData.Rows.Count > 0)
                {
                    setTag(GetEnum(gridColIndexes.cBarCode), dtData.Rows[0]["BatchCode"].ToString());
                    SetValue(GetEnum(gridColIndexes.cBarCode), dtData.Rows[0]["BatchUnique"].ToString());
                    SetValue(GetEnum(gridColIndexes.CExpiry), dtData.Rows[0]["ExpiryDate"].ToString(), "DATE");
                    SetValue(GetEnum(gridColIndexes.cMRP), dtData.Rows[0]["MRP"].ToString(), "CURR_FLOAT");
                    //SetValue(GetEnum(gridColIndexes.cPrate), dtData.Rows[0]["PRateExcl"].ToString(), "CURR_FLOAT");
                    //SetValue(GetEnum(gridColIndexes.cGrossAmt), dtData.Rows[0]["PRateExcl"].ToString(), "CURR_FLOAT");
                    SetValue(GetEnum(gridColIndexes.cSrate), dtData.Rows[0]["PRate"].ToString(), "CURR_FLOAT");

                    //SetValue(GetEnum(gridColIndexes.cSrate), SetPriceListForItems(dgvItems.CurrentRow.Index).ToString(), "CURR_FLOAT");

                    SetValue(GetEnum(gridColIndexes.cGrossAmt), dtData.Rows[0]["PrateInc"].ToString(), "CURR_FLOAT");
                    SetValue(GetEnum(gridColIndexes.cItemID), dtData.Rows[0]["ItemID"].ToString(), "INT");
                    dQty = Convert.ToDecimal(dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cQty)].Value);
                    CalcTotal();
                }
            }
            else if (iStockID == 0)
            {
                sBarUnique = "<Auto Barcode>";
                SetValue(GetEnum(gridColIndexes.cBarCode), sBarUnique);
                setTag(GetEnum(gridColIndexes.cBarCode), "");
            }
        }

        //Description : Seting Value to the Given Column, Row Indexes to Grid Cell
        private void SetValue(int iCellIndex, string sValue, string sConvertType = "")
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

        //Description : Setting Value to Tag of the cells of Grid using the Given Column and Row Indexes
        private void setTag(int iCellIndex, string sTag, string sConvertType = "")
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

        //Description : Function Polymorphism for Seting the Value to Grid Asper Parameters Given
        private void SetValue(int iCellIndex, int iRowIndex, string sValue, string sConvertType = "")
        {
            //if(sConvertType.ToUpper() == "QTY")
            //    dgvItems.Rows[iRowIndex].Cells[iCellIndex].Value = Comm.chkChangeValuetoZero(FormatValue(Convert.ToDouble(sValue),false));
            //else
                dgvItems.Rows[iRowIndex].Cells[iCellIndex].Value = Comm.chkChangeValuetoZero(sValue);
        }

        //Description : Validating the Method with Before Save Functionality
        private bool IsValidate()
        {
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
            else if (Convert.ToString(dgvItems.Rows[0].Cells[GetEnum(gridColIndexes.CItemCode)].Value) == "")
            {
                bValidate = false;
                MessageBox.Show("No Items are Entered for Save. Please Enter the Item", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                goto FailsHere;
            }
            else if (cboSalesStaff.SelectedIndex < 0)
            {
                bValidate = false;
                MessageBox.Show("Please select a sales staff.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                goto FailsHere;
            }
            else if (sMsg[0].ToString() != "")
            {
                bValidate = false;
                MessageBox.Show("Sales Rates are Lesser Than of PRate of the Item[" + dgvItems.Rows[Convert.ToInt32(sMsg[1])].Cells[GetEnum(gridColIndexes.CItemName)].Value.ToString() + "], Check the Values [" + sMsg[0].ToString() + "].", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                goto FailsHere;
            }
            else
            {
                //if(Convert.ToDecimal(dgvItems.Rows[0].Cells[GetEnum(gridColIndexes.cPrate)].Value) == 0)
                for (int i = 0; i < dgvItems.Rows.Count; i++)
                {
                    if (dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.CItemCode)].Value != null)
                    {
                        bValidate = true;
                        if (Convert.ToDecimal(dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cSrate)].Value) == 0)
                        {
                            //MessageBox.Show("Purchase rate cannot be zero. Please provide purchase rate for the item !", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            //bValidate = false;
                            //goto FailsHere;
                        }
                        if (Convert.ToDecimal(dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cQty)].Value) == 0 && Convert.ToDecimal(dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cQOH)].Value) == 0)
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
                    dtInv = Comm.fnGetData("SELECT InvNo FROM tblStockJournal WHERE vchtypeid=" + vchtypeID + " and LTRIM(RTRIM(InvNo)) = '" + txtInvAutoNo.Text.Trim() + "'").Tables[0];
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
                if (iIDFromEditWindow == 0)
                {
                    if (dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cBarCode)].Tag != null)
                    {
                        if (dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cBatchMode)].Value.ToString().Trim() != "2")
                        {
                            string sQuery = "Select ItemID,BatchCode From tblStock Where LTRIM(RTRIM(BatchCode)) = '" + dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cBarCode)].Tag.ToString() + "' AND ItemID <> " + Convert.ToDouble(dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cItemID)].Value.ToString()) + "";
                            DataTable dtBatch = Comm.fnGetData(sQuery).Tables[0];
                            if (dtBatch.Rows.Count > 0)
                            {
                                bValidate = false;
                                MessageBox.Show("This BatchCode " + dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cBarCode)].Tag.ToString() + "of Item [" + dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.CItemName)].Value.ToString() + "] is already Exist.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                break;
                            }
                        }
                    }
                }
            }

            for (int j = 0; j < dgvItems.Rows.Count; j++)
            {
                bValidate = true;
                if (Convert.ToDecimal(dgvItems.Rows[j].Cells[GetEnum(gridColIndexes.cSrate)].Value) > Convert.ToDecimal(dgvItems.Rows[j].Cells[GetEnum(gridColIndexes.cMRP)].Value))
                {
                    bValidate = false;
                    MessageBox.Show("Sales Rate Above MRP.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    goto FailsHere;
                    //break;
                }
                else if (Convert.ToDecimal(dgvItems.Rows[j].Cells[GetEnum(gridColIndexes.cSRate1)].Value) > Convert.ToDecimal(dgvItems.Rows[j].Cells[GetEnum(gridColIndexes.cMRP)].Value))
                {
                    bValidate = false;
                    MessageBox.Show("Sales Rate 1 Above MRP.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    goto FailsHere;
                    //break;
                }
                else if (Convert.ToDecimal(dgvItems.Rows[j].Cells[GetEnum(gridColIndexes.cSRate2)].Value) > Convert.ToDecimal(dgvItems.Rows[j].Cells[GetEnum(gridColIndexes.cMRP)].Value))
                {
                    if (AppSettings.IsActiveSRate2 == true)
                    {
                        bValidate = false;
                        MessageBox.Show("Sales Rate 2 Above MRP.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        goto FailsHere;
                        //break;
                    }
                }
                else if (Convert.ToDecimal(dgvItems.Rows[j].Cells[GetEnum(gridColIndexes.cSRate3)].Value) > Convert.ToDecimal(dgvItems.Rows[j].Cells[GetEnum(gridColIndexes.cMRP)].Value))
                {
                    if (AppSettings.IsActiveSRate3 == true)
                    {
                        bValidate = false;
                        MessageBox.Show("Sales Rate 3 Above MRP.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        goto FailsHere;
                        //break;
                    }
                }
                else if (Convert.ToDecimal(dgvItems.Rows[j].Cells[GetEnum(gridColIndexes.cSRate4)].Value) > Convert.ToDecimal(dgvItems.Rows[j].Cells[GetEnum(gridColIndexes.cMRP)].Value))
                {
                    if (AppSettings.IsActiveSRate4 == true)
                    {
                        bValidate = false;
                        MessageBox.Show("Sales Rate 4 Above MRP.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        goto FailsHere;
                        //break;
                    }
                }
                else if (Convert.ToDecimal(dgvItems.Rows[j].Cells[GetEnum(gridColIndexes.cSRate5)].Value) > Convert.ToDecimal(dgvItems.Rows[j].Cells[GetEnum(gridColIndexes.cMRP)].Value))
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
                    MessageBox.Show("MRP Should not be Greater than Prate or SRates !!", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    MessageBox.Show("Sales Rate 6 Above MRP.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    goto FailsHere;
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
            for (i = 0; i < dgvItems.Rows.Count; i++)
            {
                if (dgvItems.Rows[0].Cells[GetEnum(gridColIndexes.CItemCode)].Value != null)
                {
                    if (dgvItems.Rows[0].Cells[GetEnum(gridColIndexes.CItemCode)].Value.ToString() != "")
                    {
                        if (Convert.ToDecimal(dgvItems.Rows[0].Cells[GetEnum(gridColIndexes.cSrate)].Value) > Convert.ToDecimal(dgvItems.Rows[0].Cells[GetEnum(gridColIndexes.cSRate1)].Value))
                            sData = sData + AppSettings.SRate1Name + " ,";
                        else if (Convert.ToDecimal(dgvItems.Rows[0].Cells[GetEnum(gridColIndexes.cSrate)].Value) > Convert.ToDecimal(dgvItems.Rows[0].Cells[GetEnum(gridColIndexes.cSRate2)].Value))
                        {
                            if (AppSettings.IsActiveSRate2 == true)
                                sData = sData + AppSettings.SRate2Name + " ,";
                        }
                        else if (Convert.ToDecimal(dgvItems.Rows[0].Cells[GetEnum(gridColIndexes.cSrate)].Value) > Convert.ToDecimal(dgvItems.Rows[0].Cells[GetEnum(gridColIndexes.cSRate3)].Value))
                        {
                            if (AppSettings.IsActiveSRate3 == true)
                                sData = sData + AppSettings.SRate3Name + " ,";
                        }
                        else if (Convert.ToDecimal(dgvItems.Rows[0].Cells[GetEnum(gridColIndexes.cSrate)].Value) > Convert.ToDecimal(dgvItems.Rows[0].Cells[GetEnum(gridColIndexes.cSRate4)].Value))
                        {
                            if (AppSettings.IsActiveSRate4 == true)
                                sData = sData + AppSettings.SRate4Name + " ,";
                        }
                        else if (Convert.ToDecimal(dgvItems.Rows[0].Cells[GetEnum(gridColIndexes.cSrate)].Value) > Convert.ToDecimal(dgvItems.Rows[0].Cells[GetEnum(gridColIndexes.cSRate5)].Value))
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

        //Description : Serialize the Sales table Fields asper instructions.
        private string SerializetoJson()
        {
            #region "Sales Master (tblStockJournal) ------------------------------- >>"

            if (iIDFromEditWindow == 0)
            {
                clsJPMinfo.InvId = Comm.gfnGetNextSerialNo("tblStockJournal", "InvId");
                txtInvAutoNo.Tag = clsJPMinfo.InvId;
                clsJPMinfo.AutoNum = Convert.ToDecimal(Comm.gfnGetNextSerialNo("tblStockJournal", "AutoNum", " VchTypeID=" + vchtypeID).ToString());
            }
            else
            {
                clsJPMinfo.InvId = Convert.ToDecimal(iIDFromEditWindow);
                txtInvAutoNo.Tag = Convert.ToDecimal(iIDFromEditWindow);
                if (txtReferenceAutoNo.Tag.ToString() == "") txtReferenceAutoNo.Tag = 0;
                clsJPMinfo.AutoNum = Convert.ToDecimal(txtReferenceAutoNo.Tag.ToString());
            }

            clsJPMinfo.InvNo = txtInvAutoNo.Text;
            clsJPMinfo.Prefix = txtPrefix.Text.Trim();
            clsJPMinfo.InvDate = Convert.ToDateTime(dtpInvDate.Text);
            clsJPMinfo.VchType = clsVchType.TransactionName;
            clsJPMinfo.MOP = "CASH";
            clsJPMinfo.TaxModeID = Convert.ToDecimal("1");
            clsJPMinfo.LedgerId = Convert.ToDecimal("100");
            clsJPMinfo.Party = "";
            clsJPMinfo.Discount = Convert.ToDecimal("0");
            clsJPMinfo.TaxAmt = Convert.ToDecimal("0");
            clsJPMinfo.GrossAmt = Convert.ToDecimal(txtGrossAmt.Text);
            clsJPMinfo.QtyTotal = Convert.ToDecimal(lblQtyTotal.Text);
            clsJPMinfo.FreeTotal = Convert.ToDecimal("0");
            clsJPMinfo.BillAmt = Convert.ToDecimal(lblBillAmount.Text);
            clsJPMinfo.CoolieTotal = Convert.ToDecimal("0");

            clsJPMinfo.Cancelled = 0;
            clsJPMinfo.OtherExpense = Convert.ToDecimal("0");
            clsJPMinfo.SalesManID = Convert.ToDecimal(cboSalesStaff.SelectedValue);
            clsJPMinfo.Taxable = Convert.ToDecimal("0");
            clsJPMinfo.NonTaxable = Convert.ToDecimal("0");
            clsJPMinfo.ItemDiscountTotal = Convert.ToDecimal("0");
            clsJPMinfo.RoundOff = Convert.ToDecimal("0");
            clsJPMinfo.UserNarration = txtNarration.Text;
            clsJPMinfo.SortNumber = 0;
            clsJPMinfo.DiscPer = Convert.ToDecimal("0");
            clsJPMinfo.VchTypeID = vchtypeID;
            clsJPMinfo.CCID = Convert.ToDecimal(cboCostCentre.SelectedValue);
            clsJPMinfo.DestCCID = Convert.ToDecimal(cboDestCostCentre.SelectedValue);
            clsJPMinfo.CurrencyID = 0;
            clsJPMinfo.PartyAddress = "";
            clsJPMinfo.UserID = Global.gblUserID;
            clsJPMinfo.AgentID = Convert.ToDecimal("1");
            clsJPMinfo.CashDiscount = Convert.ToDecimal("0");
            clsJPMinfo.DPerType_ManualCalc_Customer = 0;
            clsJPMinfo.NetAmount = Convert.ToDecimal("0");
            clsJPMinfo.RefNo = txtReferencePrefix.Text;
            clsJPMinfo.CashPaid = 0;
            clsJPMinfo.CardPaid = 0;
            clsJPMinfo.blnWaitforAuthorisation = 0;
            clsJPMinfo.UserIDAuth = 0;
            clsJPMinfo.BillTime = DateTime.Now;
            clsJPMinfo.StateID = Convert.ToDecimal("32");
            clsJPMinfo.ImplementingStateCode = "";
            clsJPMinfo.GSTType = "";
            clsJPMinfo.CGSTTotal = 0;
            clsJPMinfo.SGSTTotal = 0;
            clsJPMinfo.IGSTTotal = 0;
            clsJPMinfo.PartyGSTIN = "";
            clsJPMinfo.BillType = "";
            clsJPMinfo.blnHold = 0;
            clsJPMinfo.PriceListID = 0;
            clsJPMinfo.EffectiveDate = dtpInvDate.Value;
            clsJPMinfo.partyCode = "";
            clsJPMinfo.MobileNo = "";
            clsJPMinfo.Email = "";
            clsJPMinfo.TaxType = "";
            clsJPMinfo.QtyTotal = 0;
            clsJPMinfo.DestCCID = Convert.ToDecimal(cboDestCostCentre.SelectedValue);
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
            clsJPMinfo.ReferenceAutoNO = txtReferenceAutoNo.Text;
            clsJPMinfo.CashDisPer = Convert.ToDecimal("0");
            clsJPMinfo.CostFactor = Convert.ToDecimal("0");
            clsJPMinfo.TenantID = Global.gblTenantID;
            clsPM.clsJsonPMInfo_ = clsJPMinfo;

            #endregion

            #region "Cost Center (tblCostCenter) --------------------------------- >>"

            clsJPMCostCentreinfo.CCID = Convert.ToDecimal(cboCostCentre.SelectedValue);
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

            #region "Dest Cost Center (tblCostCenter) --------------------------------- >>"

            clsJPMDestCostCentreinfo.CCID = Convert.ToDecimal(cboDestCostCentre.SelectedValue);
            clsJPMDestCostCentreinfo.CCName = cboDestCostCentre.SelectedItem.ToString();
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
            clsPM.clsJsonPMDestCCentreInfo_ = clsJPMDestCostCentreinfo;

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
            clsPM.clsJsonPMEmployeeInfo_ = clsJPMEmployeeinfo;

            #endregion

            #region "Sales Details (tblStockJournalItem) -------------------------- >>"
            DataTable dtBatchUniq = new DataTable();
            List<clsJsonSJDetailsInfo> lstJPDinfo = new List<clsJsonSJDetailsInfo>();
            for (int i = 0; i < dgvItems.Rows.Count; i++)
            {
                if (dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.CItemCode)].Value != null)
                {
                    if (dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.CItemCode)].Value.ToString() != "")
                    {
                        clsJSJDinfo = new clsJsonSJDetailsInfo();

                        if (dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cBarCode)].Tag.ToString().ToUpper() == "<AUTO BARCODE>")
                            dtBatchUniq = Comm.fnGetData("EXEC UspGetBatchCodeWhenAutoBarcode " + Convert.ToDecimal(dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cItemID)].Value) + ",'" + dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cBarCode)].Tag.ToString() + "',''," + Convert.ToDecimal(dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cMRP)].Value) + ",'" + Convert.ToDateTime(dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.CExpiry)].Value).ToString("dd-MMM-yyyy") + "'," + Global.gblTenantID + "").Tables[0];

                        //clsJSJDinfo.InvID = Convert.ToDecimal(txtInvAutoNo.Text);
                        clsJSJDinfo.InvID = Convert.ToDecimal(txtInvAutoNo.Tag);
                        clsJSJDinfo.ItemId = Convert.ToDecimal(dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cItemID)].Value);
                        clsJSJDinfo.Qty = Convert.ToDecimal(dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cQty)].Value);
                        clsJSJDinfo.Rate = Convert.ToDecimal(dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cRateinclusive)].Value);
                        clsJSJDinfo.UnitId = Convert.ToDecimal(dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.CUnit)].Tag);
                        clsJSJDinfo.Batch = "";
                        clsJSJDinfo.TaxPer = Convert.ToDecimal(dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.ctaxPer)].Value);
                        clsJSJDinfo.TaxAmount = Convert.ToDecimal(dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.ctax)].Value);
                        clsJSJDinfo.Discount = Convert.ToDecimal(dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cDiscAmount)].Value);
                        clsJSJDinfo.MRP = Convert.ToDecimal(dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cMRP)].Value);
                        clsJSJDinfo.SlNo = Convert.ToDecimal(dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cSlNo)].Value);
                        clsJSJDinfo.Prate = Convert.ToDecimal(dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cSrate)].Value);
                        clsJSJDinfo.Free = Convert.ToDecimal(dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cQOH)].Value);
                        clsJSJDinfo.SerialNos = "";
                        clsJSJDinfo.ItemDiscount = Convert.ToDecimal(dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cDiscAmount)].Value);

                        if (dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cBarCode)].Tag != null)
                        {
                            //if (dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cBarCode)].Tag.ToString() != "0")
                                clsJSJDinfo.BatchCode = dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cBarCode)].Tag.ToString();
                            //else
                            //    clsJSJDinfo.BatchCode = dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cBarCode)].Value.ToString();
                        }
                        else
                            clsJSJDinfo.BatchCode = dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cBarCode)].Value.ToString();

                        clsJSJDinfo.iCessOnTax = 0;
                        clsJSJDinfo.blnCessOnTax = 0;
                        clsJSJDinfo.Expiry = Convert.ToDateTime(dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.CExpiry)].Value);
                        clsJSJDinfo.ItemDiscountPer = Convert.ToDecimal(dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cDiscPer)].Value);
                        clsJSJDinfo.RateInclusive = Convert.ToDecimal(dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cRateinclusive)].Value);
                        clsJSJDinfo.ITaxableAmount = Convert.ToDecimal(dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.ctaxable)].Value);
                        clsJSJDinfo.INetAmount = Convert.ToDecimal(dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cNetAmount)].Value);
                        clsJSJDinfo.CGSTTaxPer = Convert.ToDecimal(dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cCGST)].Tag);
                        clsJSJDinfo.CGSTTaxAmt = Convert.ToDecimal(dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cCGST)].Value);
                        clsJSJDinfo.SGSTTaxPer = Convert.ToDecimal(dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cSGST)].Tag);
                        clsJSJDinfo.SGSTTaxAmt = Convert.ToDecimal(dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cSGST)].Value);
                        clsJSJDinfo.IGSTTaxPer = Convert.ToDecimal(dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cIGST)].Tag);
                        clsJSJDinfo.IGSTTaxAmt = Convert.ToDecimal(dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cIGST)].Value);
                        clsJSJDinfo.iRateDiscPer = Convert.ToDecimal(dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cDiscPer)].Value);
                        clsJSJDinfo.iRateDiscount = Convert.ToDecimal(dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cDiscAmount)].Value);

                        //string[] strBatchUniq;
                        
                        clsJSJDinfo.BatchUnique = dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cBarCode)].Value.ToString();
                        
                        //clsJSJDinfo.BatchUnique = dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cBarCode)].Tag.ToString();
                        //if (dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cBarCode)].Tag.ToString().ToUpper() == "<AUTO BARCODE>")
                        //{
                        //    if (dtBatchUniq.Rows.Count > 0)
                        //        clsJSJDinfo.BatchUnique = dtBatchUniq.Rows[0]["BatchUniq"].ToString();
                        //    else
                        //        clsJSJDinfo.BatchUnique = dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cBarCode)].Tag.ToString();
                        //}
                        //else
                        //{
                        //    strBatchUniq = dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cBarCode)].Tag.ToString().Split('@');
                        //    if (strBatchUniq.Length > 0)
                        //    {
                        //        if (strBatchUniq.Length == 2)
                        //        {
                        //            if (Convert.ToDecimal(dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cMRP)].Value) != Convert.ToDecimal(strBatchUniq[1].ToString()))
                        //            {
                        //                clsJSJDinfo.BatchUnique = strBatchUniq[0].ToString() + "@" + Convert.ToDecimal(dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cMRP)].Value).ToString(AppSettings.CurrDecimalFormat);
                        //            }
                        //            else
                        //                clsJSJDinfo.BatchUnique = dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cBarCode)].Tag.ToString();
                        //        }
                        //        else if (strBatchUniq.Length == 3)
                        //        {
                        //            if (Convert.ToDecimal(dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cMRP)].Value) != Convert.ToDecimal(strBatchUniq[1].ToString()))
                        //            {
                        //                clsJSJDinfo.BatchUnique = strBatchUniq[0].ToString() + "@" + Convert.ToDecimal(dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cMRP)].Value).ToString(AppSettings.CurrDecimalFormat) + "@" + Convert.ToDateTime(dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.CExpiry)].Value).ToString("dd-MM-yy").Replace("-", "");
                        //            }
                        //            else
                        //                clsJSJDinfo.BatchUnique = dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cBarCode)].Tag.ToString();
                        //        }
                        //        else
                        //            clsJSJDinfo.BatchUnique = dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cBarCode)].Tag.ToString();
                        //    }
                        //    else
                        //    {
                        //        clsJSJDinfo.BatchUnique = dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cBarCode)].Tag.ToString();
                        //    }
                        //}

                        clsJSJDinfo.blnQtyIN = 0;
                        clsJSJDinfo.CRate = Convert.ToDecimal(dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cCrate)].Value);
                        clsJSJDinfo.CRate = Convert.ToDecimal(dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cCRateWithTax)].Value);
                        clsJSJDinfo.Unit = dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.CUnit)].Value.ToString();
                        clsJSJDinfo.ItemStockID = 0;
                        clsJSJDinfo.IcessPercent = Convert.ToDecimal(dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cCCessPer)].Value);
                        clsJSJDinfo.IcessAmt = 0;
                        clsJSJDinfo.IQtyCompCessPer = 0;
                        clsJSJDinfo.IQtyCompCessAmt = Convert.ToDecimal(dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cCCompCessQty)].Value);
                        clsJSJDinfo.StockMRP = 0;
                        clsJSJDinfo.InonTaxableAmount = 0;
                        clsJSJDinfo.IAgentCommPercent = 0;
                        clsJSJDinfo.BlnDelete = 0;
                        clsJSJDinfo.Id = Convert.ToDecimal(dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cID)].Value);
                        clsJSJDinfo.StrOfferDetails = "";
                        clsJSJDinfo.BlnOfferItem = 0;
                        clsJSJDinfo.EnteredQty = Convert.ToDecimal(dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cQty)].Value);
                        clsJSJDinfo.StockQty = Convert.ToDecimal(dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cQOH)].Value);

                        clsJSJDinfo.BalQty = Convert.ToDecimal(dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cQty)].Value) - Convert.ToDecimal(dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cQOH)].Value);

                        clsJSJDinfo.GrossAmount = Convert.ToDecimal(dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cGrossAmt)].Value);
                        clsJSJDinfo.iFloodCessPer = Convert.ToDecimal(dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cFloodCessPer)].Value);
                        clsJSJDinfo.iFloodCessAmt = Convert.ToDecimal(dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cFloodCessAmt)].Value);
                        clsJSJDinfo.Srate1 = Convert.ToDecimal(dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cSRate1)].Value);
                        clsJSJDinfo.Srate2 = Convert.ToDecimal(dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cSRate2)].Value);
                        clsJSJDinfo.Srate3 = Convert.ToDecimal(dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cSRate3)].Value);
                        clsJSJDinfo.Srate4 = Convert.ToDecimal(dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cSRate4)].Value);
                        clsJSJDinfo.Srate5 = Convert.ToDecimal(dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cSRate5)].Value);
                        clsJSJDinfo.Costrate = Convert.ToDecimal(dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cCrate)].Value);
                        clsJSJDinfo.CostValue = 0;
                        clsJSJDinfo.Profit = 0;
                        clsJSJDinfo.ProfitPer = 0;
                        clsJSJDinfo.DiscMode = 0;
                        clsJSJDinfo.Srate1Per = Convert.ToDecimal(dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cSRate1Per)].Value);
                        clsJSJDinfo.Srate2Per = Convert.ToDecimal(dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cSRate2Per)].Value);
                        clsJSJDinfo.Srate3Per = Convert.ToDecimal(dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cSRate3Per)].Value);
                        clsJSJDinfo.Srate4Per = Convert.ToDecimal(dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cSRate4Per)].Value);
                        clsJSJDinfo.Srate5Per = Convert.ToDecimal(dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cSRate5Per)].Value);
                        lstJPDinfo.Add(clsJSJDinfo);
                    }
                }
            }
            clsPM.clsJsonSJDetailsInfoList_ = lstJPDinfo;

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
                        clsJPDUnitinfo.UnitID = Convert.ToDecimal(dgvItems.Rows[j].Cells[GetEnum(gridColIndexes.CUnit)].Tag);
                        clsJPDUnitinfo.UnitName = dgvItems.Rows[j].Cells[GetEnum(gridColIndexes.CUnit)].Value.ToString();
                        //dipu on 20-Apr-2022 ----->>
                        dtUnit = Comm.fnGetData("SELECT UnitShortName FROM tblUnit WHERE UnitID = " + Convert.ToDecimal(dgvItems.Rows[j].Cells[GetEnum(gridColIndexes.CUnit)].Tag) + "").Tables[0];
                        if(dtUnit.Rows.Count > 0)
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
                        clsJPDIteminfo.SrateCalcMode = Convert.ToInt32(sArrItm[GetEnumItem(ItemIndexes.SrateCalcMode)].ToString());
                        clsJPDIteminfo.CRateAvg = Convert.ToDecimal(sArrItm[GetEnumItem(ItemIndexes.CRateAvg)].ToString());
                        clsJPDIteminfo.Srate1Per = Convert.ToDecimal(sArrItm[GetEnumItem(ItemIndexes.Srate1Per)].ToString());
                        clsJPDIteminfo.SRate1 = Convert.ToDecimal(sArrItm[GetEnumItem(ItemIndexes.SRate1)].ToString());
                        clsJPDIteminfo.Srate2Per = Convert.ToDecimal(sArrItm[GetEnumItem(ItemIndexes.Srate2Per)].ToString());
                        clsJPDIteminfo.SRate2 = Convert.ToDecimal(sArrItm[GetEnumItem(ItemIndexes.SRate2)].ToString());
                        clsJPDIteminfo.Srate3Per = Convert.ToDecimal(sArrItm[GetEnumItem(ItemIndexes.Srate3Per)].ToString());
                        clsJPDIteminfo.SRate3 = Convert.ToDecimal(sArrItm[GetEnumItem(ItemIndexes.SRate3)].ToString());
                        clsJPDIteminfo.Srate4 = Convert.ToDecimal(sArrItm[GetEnumItem(ItemIndexes.Srate4)].ToString());
                        clsJPDIteminfo.Srate4Per = Convert.ToDecimal(sArrItm[GetEnumItem(ItemIndexes.Srate4Per)].ToString());
                        clsJPDIteminfo.SRate5 = Convert.ToDecimal(sArrItm[GetEnumItem(ItemIndexes.SRate5)].ToString());
                        clsJPDIteminfo.Srate5Per = Convert.ToDecimal(sArrItm[GetEnumItem(ItemIndexes.Srate5Per)].ToString());
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
            clsPM.clsJsonPDIteminfoList_ = lstJPDItem;
            #endregion

            return JsonConvert.SerializeObject(clsPM);
        }

        // Cash : 0, Credit: 1, Both: 2, Cash Desk : 3
        //Description : Deserialize the JSon to Controls asper instructions.
        private void DeserializeFromJSon(string sToDeSerialize = "")
        {
            clsJSonStockJournal clsStockJournal = JsonConvert.DeserializeObject<clsJSonStockJournal>(sToDeSerialize);

            txtPrefix.Text = clsVchType.TransactionPrefix;
            txtInvAutoNo.Text = Convert.ToString(clsStockJournal.clsJsonPMInfo_.InvNo);
            txtInvAutoNo.Tag = Convert.ToDouble(clsStockJournal.clsJsonPMInfo_.InvId);
            txtReferenceAutoNo.Tag = Convert.ToDouble(clsStockJournal.clsJsonPMInfo_.AutoNum);
            dtpInvDate.Text = Convert.ToString(clsStockJournal.clsJsonPMInfo_.InvDate);
            txtReferencePrefix.Text = clsStockJournal.clsJsonPMInfo_.RefNo;
            txtReferenceAutoNo.Text = Convert.ToString(clsStockJournal.clsJsonPMInfo_.ReferenceAutoNO);

            txtGrossAmt.Text = Comm.chkChangeValuetoZero(Convert.ToString(clsStockJournal.clsJsonPMInfo_.GrossAmt));
            lblQtyTotal.Text = Comm.chkChangeValuetoZero(Convert.ToString(clsStockJournal.clsJsonPMInfo_.QtyTotal));
                 Comm.chkChangeValuetoZero(Convert.ToString(clsStockJournal.clsJsonPMInfo_.Discount));

            txtNarration.Text = Convert.ToString(clsStockJournal.clsJsonPMInfo_.UserNarration);
            lblBillAmount.Text = Comm.chkChangeValuetoZero(Convert.ToString(clsStockJournal.clsJsonPMInfo_.BillAmt));

            cboCostCentre.SelectedValue = clsStockJournal.clsJsonPMCCentreInfo_.CCID;
            cboDestCostCentre.SelectedValue = clsStockJournal.clsJsonPMDestCCentreInfo_.CCID;
            cboSalesStaff.SelectedValue = clsStockJournal.clsJsonPMEmployeeInfo_.EmpID;

            DataTable dtGetPurDetail = clsStockJournal.clsJsonSJDetailsInfoList_.ToDataTable();
            DataTable dtItemFrmJson = clsStockJournal.clsJsonPDIteminfoList_.ToDataTable();
            DataTable dtUnitFrmJson = clsStockJournal.clsJsonPDUnitinfoList_.ToDataTable();
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
                    dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cID)].Value = dtGetPurDetail.Rows[i]["Id"].ToString();
                    dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.CItemName)].Tag = dtGetPurDetail.Rows[i]["ItemId"].ToString();
                    dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.CUnit)].Value = dtUnitFrmJson.Rows[i]["UnitName"].ToString();
                    dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.CUnit)].Tag = dtGetPurDetail.Rows[i]["UnitId"].ToString();
                    dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cBarCode)].Value = dtGetPurDetail.Rows[i]["BatchCode"].ToString();
                    dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cBarCode)].Tag = dtGetPurDetail.Rows[i]["BatchUnique"].ToString(); 
                    dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.CExpiry)].Value = Convert.ToDateTime(dtGetPurDetail.Rows[i]["Expiry"]).ToString("dd-MMM-yyyy");
                    dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cMRP)].Value = FormatValue(Convert.ToDouble(dtGetPurDetail.Rows[i]["MRP"].ToString()), true);
                    dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cSrate)].Value = FormatValue(Convert.ToDouble(dtGetPurDetail.Rows[i]["Prate"].ToString()), true);
                    this.dgvItems.Columns[GetEnum(gridColIndexes.cQty)].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cQty)].Value = FormatValue(Convert.ToDouble(dtGetPurDetail.Rows[i]["Qty"].ToString()),false);
                    this.dgvItems.Columns[GetEnum(gridColIndexes.cQOH)].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cQOH)].Value = FormatValue(Convert.ToDouble(dtGetPurDetail.Rows[i]["Free"].ToString()),false);

                    dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cSRate1Per)].Value = FormatValue(Convert.ToDouble(dtGetPurDetail.Rows[i]["Srate1Per"].ToString()),true);
                    dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cSRate1)].Value = FormatValue(Convert.ToDouble(dtGetPurDetail.Rows[i]["Srate1"].ToString()), true);
                    dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cSRate2Per)].Value = FormatValue(Convert.ToDouble(dtGetPurDetail.Rows[i]["Srate2Per"].ToString()), true);
                    dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cSRate2)].Value = FormatValue(Convert.ToDouble(dtGetPurDetail.Rows[i]["Srate2"].ToString()), true);
                    dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cSRate3Per)].Value = FormatValue(Convert.ToDouble(dtGetPurDetail.Rows[i]["Srate3Per"].ToString()), true);
                    dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cSRate3)].Value = FormatValue(Convert.ToDouble(dtGetPurDetail.Rows[i]["Srate3"].ToString()), true);
                    dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cSRate4Per)].Value = FormatValue(Convert.ToDouble(dtGetPurDetail.Rows[i]["Srate4Per"].ToString()), true);
                    dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cSRate4)].Value = FormatValue(Convert.ToDouble(dtGetPurDetail.Rows[i]["Srate4"].ToString()), true);
                    dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cSRate5Per)].Value = FormatValue(Convert.ToDouble(dtGetPurDetail.Rows[i]["Srate5Per"].ToString()), true);
                    dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cSRate5)].Value = FormatValue(Convert.ToDouble(dtGetPurDetail.Rows[i]["Srate5"].ToString()), true);

                    dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cGrossAmt)].Value = FormatValue(Convert.ToDouble(dtGetPurDetail.Rows[i]["GrossAmount"].ToString()), true);
                    dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cDiscPer)].Value = FormatValue(Convert.ToDouble(dtGetPurDetail.Rows[i]["ItemDiscountPer"].ToString()), true);
                    this.dgvItems.Columns[GetEnum(gridColIndexes.cDiscPer)].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cDiscAmount)].Value = FormatValue(Convert.ToDouble(dtGetPurDetail.Rows[i]["ItemDiscount"].ToString()), true);
                    dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cBillDisc)].Value = FormatValue(Convert.ToDouble(dtGetPurDetail.Rows[i]["Discount"].ToString()), true);
                    dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cCrate)].Value = FormatValue(Convert.ToDouble(dtGetPurDetail.Rows[i]["CRate"].ToString()), true);
                    dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cItemID)].Value = dtGetPurDetail.Rows[i]["ItemId"].ToString();
                    dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.ctaxPer)].Value = FormatValue(Convert.ToDouble(dtGetPurDetail.Rows[i]["TaxPer"].ToString()), true);
                    this.dgvItems.Columns[GetEnum(gridColIndexes.ctaxPer)].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.ctax)].Value = FormatValue(Convert.ToDouble(dtGetPurDetail.Rows[i]["TaxAmount"].ToString()), true);
                    dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cIGST)].Tag = FormatValue(Convert.ToDouble(dtGetPurDetail.Rows[i]["IGSTTaxPer"].ToString()), true);
                    dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cIGST)].Value = FormatValue(Convert.ToDouble(dtGetPurDetail.Rows[i]["IGSTTaxAmt"].ToString()), true);
                    dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cSGST)].Tag = FormatValue(Convert.ToDouble(dtGetPurDetail.Rows[i]["CGSTTaxPer"].ToString()), true);
                    dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cSGST)].Value = FormatValue(Convert.ToDouble(dtGetPurDetail.Rows[i]["CGSTTaxAmt"].ToString()), true);
                    dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cCGST)].Tag = FormatValue(Convert.ToDouble(dtGetPurDetail.Rows[i]["SGSTTaxPer"].ToString()), true);
                    dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cCGST)].Value = FormatValue(Convert.ToDouble(dtGetPurDetail.Rows[i]["SGSTTaxAmt"].ToString()), true);
                    dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cNetAmount)].Value = FormatValue(Convert.ToDouble(dtGetPurDetail.Rows[i]["INetAmount"].ToString()), true);
                    dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cNonTaxable)].Value = FormatValue(Convert.ToDouble(dtGetPurDetail.Rows[i]["InonTaxableAmount"].ToString()), true);
                    this.dgvItems.Columns[GetEnum(gridColIndexes.cNonTaxable)].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cCCessPer)].Value = FormatValue(Convert.ToDouble(dtGetPurDetail.Rows[i]["IcessPercent"].ToString()), true);
                    this.dgvItems.Columns[GetEnum(gridColIndexes.cCCessPer)].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cCCompCessQty)].Value = FormatValue(Convert.ToDouble(dtGetPurDetail.Rows[i]["IQtyCompCessAmt"].ToString()), true);
                    this.dgvItems.Columns[GetEnum(gridColIndexes.cCCompCessQty)].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cFloodCessPer)].Value = FormatValue(Convert.ToDouble(dtGetPurDetail.Rows[i]["iFloodCessPer"].ToString()), true);
                    this.dgvItems.Columns[GetEnum(gridColIndexes.cFloodCessPer)].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cFloodCessAmt)].Value = FormatValue(Convert.ToDouble(dtGetPurDetail.Rows[i]["iFloodCessAmt"].ToString()), true);
                    this.dgvItems.Columns[GetEnum(gridColIndexes.cFloodCessAmt)].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cStockMRP)].Value = FormatValue(Convert.ToDouble(dtGetPurDetail.Rows[i]["StockMRP"].ToString()), true);
                    this.dgvItems.Columns[GetEnum(gridColIndexes.cStockMRP)].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cAgentCommPer)].Value = FormatValue(Convert.ToDouble(dtGetPurDetail.Rows[i]["IAgentCommPercent"].ToString()), true);
                    this.dgvItems.Columns[GetEnum(gridColIndexes.cAgentCommPer)].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cBlnOfferItem)].Value = dtGetPurDetail.Rows[i]["BlnOfferItem"].ToString();
                    dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cStrOfferDetails)].Value = dtGetPurDetail.Rows[i]["StrOfferDetails"].ToString();
                    dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cBatchMode)].Value = dtItemFrmJson.Rows[i]["BatchMode"].ToString();

                    dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cCoolie)].Value = dtItemFrmJson.Rows[i]["Coolie"].ToString();

                    if (Convert.ToDouble(dtGetPurDetail.Rows[i]["RateInclusive"].ToString()) == 1)
                        dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cRateinclusive)].Value = true;
                    else
                        dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cRateinclusive)].Value = false;

                    this.dgvItems.Columns[i].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                }

                dgvItems.Columns[GetEnum(gridColIndexes.cCGST)].Visible = false;
                dgvItems.Columns[GetEnum(gridColIndexes.cSGST)].Visible = false;
                dgvItems.Columns[GetEnum(gridColIndexes.cIGST)].Visible = true;

                CalcTotal();
            }
        }

        private void CRUD_Operations(int iAction = 0)
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
                    if (IsValidate() == true)
                    {
                        string strJson = SerializetoJson();

                        #region "CRUD Operations for Sales Master ------------------------- >>"

                        string sRet = clsPur.StockJournalMasterCRUD(clsPM, sqlConn, trans, strJson, iAction);
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
                                //    Comm.MessageboxToasted("Sales", "Sales Group saved successfully");
                            }
                        }
                        #endregion

                        #region "CRUD Operations for Sales Detail ------------------------- >>"
                        Hashtable hstPurStk = new Hashtable();

                        if (iAction == 1) // Edit
                        {
                            //trans.Commit();

                            sRetDet = clsPur.StockJournalDetailCRUD(clsPM, sqlConn, trans, sBatchCode, 2);
                            sRetDet = clsPur.StockJournalDetailCRUD(clsPM, sqlConn, trans, sBatchCode, 0);
                        }
                        else
                            sRetDet = clsPur.StockJournalDetailCRUD(clsPM, sqlConn, trans, sBatchCode, iAction);

                        if (sRetDet == "") sRetDet = "0";
                        if (sRetDet.Length > 2)
                        {
                            strResult = sRetDet.Split('|');
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
                                //    Comm.MessageboxToasted("Sales", "Voucher[" + txtInvAutoNo.Text + "] Saved Successfully");

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
                                Comm.MessageboxToasted("Sales", "Voucher[" + vchno + "] Saved Successfully");
                                return;
                            }
                            else
                            {
                                ClearControls();
                                Comm.MessageboxToasted("Sales", "Voucher[" + vchno + "] Saved Successfully");
                            }
                        }
                        else if (iAction == 2)
                        {
                            this.Close();
                            Comm.MessageboxToasted("Sales", "Voucher[" + vchno + "] deleted successfully");
                            return;
                        }
                        else if (iAction == 3)
                        {
                            this.Close();
                            Comm.MessageboxToasted("Sales", "Voucher[" + vchno + "] is archived");
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


            FillEmployee();

            SetTransactionDefaults();
            SetTransactionsthatVarying();
            SetApplicationSettings();

            dgvItems.Rows.Clear();
            dgvItems.Refresh();
            iIDFromEditWindow = 0;
            AddColumnsToGrid();
            dgvItems.Rows.Add();
            
            txtGrossAmt.Text = "";
            lblQtyTotal.Text = "";
            
            txtNarration.Text = "";
            lblBillAmount.Text = "";
            
            if (ConvertI32(clsVchType.TransactionNumberingValue) == 2) // Custom
                txtInvAutoNo.Clear();
            if (ConvertI32(clsVchType.ReferenceNumberingValue) == 2) // Custom
                txtReferencePrefix.Clear();

            dgvItems.Columns["cRateinclusive"].Visible = false;

            dgvItems.Columns["cSlNo"].Frozen = true;
            //dgvItems.Columns["cImgDel"].Frozen = true;
            dgvItems.Columns["cImgDel"].Visible = true;
            dgvItems.Columns["cImgDel"].Width = 40;

            txtInvAutoNo.Focus();
        }

        //Description : Function Polymorphism of SetTag
        private void SetTag(int iCellIndex, int iRowIndex, string sTag, string sConvertType = "")
        {
            if (sConvertType.ToUpper() == "PERC_FLOAT")
                dgvItems.Rows[iRowIndex].Cells[iCellIndex].Tag = Convert.ToDecimal(sTag).ToString("#.00");
            else
                dgvItems.Rows[iRowIndex].Cells[iCellIndex].Tag = sTag;
        }

        //Description : Convert the Enum Members to Column index
        private int GetEnum(int ColIndexes)
        {
            return ColIndexes;
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
            DateTime sSalesDate = Convert.ToDateTime(dtpInvDate.Text);
            DateTime sExpiryDate = Convert.ToDateTime(dgvItems.CurrentRow.Cells[GetEnum(gridColIndexes.CExpiry)].Value);

            if (iShelfLifeDays > 0)
            {
                int iDaysCount = Convert.ToInt32((sExpiryDate - sSalesDate).TotalDays);
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
                dtBarCodeExist = Comm.fnGetData("SELECT COUNT(*) FROM tblStock WHERE LTRIM(RTRIM(UPPER(BatchCode))) = '" + sCompSearchData[1].ToString().Trim() + "'").Tables[0];
                if (Convert.ToInt32(dtBarCodeExist.Rows[0][0].ToString()) == 0)
                {
                    SetValue(GetEnum(gridColIndexes.cBarCode), sCompSearchData[1].ToString().Trim());
                    setTag(GetEnum(gridColIndexes.cBarCode), sCompSearchData[1].ToString().Trim());
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
                    if (Convert.ToInt32(sCompSearchData[0].ToString()) >= 0)
                    {
                        FillGridAsperStockID(Convert.ToInt32(sCompSearchData[0].ToString()));
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
                    if (Convert.ToInt32(sCompSearchData[0].ToString()) != 0)
                    {
                        DataTable dtGet = Comm.fnGetData("SELECT * FROM tblTransactionPause WHERE ID =" + Convert.ToInt32(sCompSearchData[0].ToString()) + "").Tables[0];
                        if (dtGet.Rows.Count > 0)
                        {
                            lblPause.Tag = Convert.ToInt32(sCompSearchData[0].ToString());
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
                        if (Convert.ToInt32(sCompSearchData[0].ToString()) != 0)
                        {
                            GetItmMststockinfo.StockID = Convert.ToInt32(sCompSearchData[0].ToString());
                            GetItmMststockinfo.TenantID = Global.gblTenantID;

                            
                            dtItemPublic = clsItmMst.GetItemMasterFromStock(GetItmMststockinfo);


                            if (dtItemPublic.Rows.Count > 0)
                            {
                                SetValue(GetEnum(gridColIndexes.CItemName), dtItemPublic.Rows[0]["ItemName"].ToString());
                                SetValue(GetEnum(gridColIndexes.CUnit), dtItemPublic.Rows[0]["Unit"].ToString());
                                setTag(GetEnum(gridColIndexes.CUnit), dtItemPublic.Rows[0]["UNITID"].ToString());
                                SetValue(GetEnum(gridColIndexes.cItemID), dtItemPublic.Rows[0]["ItemID"].ToString());
                                setTag(GetEnum(gridColIndexes.CItemName), dtItemPublic.Rows[0]["ItemID"].ToString());
                                SetValue(GetEnum(gridColIndexes.cBarCode), dtItemPublic.Rows[0]["BatchUnique"].ToString());
                                setTag(GetEnum(gridColIndexes.cBarCode), dtItemPublic.Rows[0]["BatchCode"].ToString());

                                SetValue(GetEnum(gridColIndexes.cSrate), dtItemPublic.Rows[0]["PRate"].ToString(), "CURR_FLOAT");

                                SetValue(GetEnum(gridColIndexes.cMRP), dtItemPublic.Rows[0]["MRP"].ToString(), "CURR_FLOAT");
                                SetValue(GetEnum(gridColIndexes.cQOH), dtItemPublic.Rows[0]["QOH"].ToString(), "CURR_FLOAT");

                                SetValue(GetEnum(gridColIndexes.cCrate), dtItemPublic.Rows[0]["CostRateExcl"].ToString(), "CURR_FLOAT");
                                SetValue(GetEnum(gridColIndexes.cCRateWithTax), dtItemPublic.Rows[0]["CostRateInc"].ToString(), "CURR_FLOAT");

                                SetValue(GetEnum(gridColIndexes.cSRate1Per), dtItemPublic.Rows[0]["Srate1Per"].ToString(), "PERC_FLOAT");
                                SetValue(GetEnum(gridColIndexes.cSRate1), dtItemPublic.Rows[0]["SRate1"].ToString(), "CURR_FLOAT");
                                SetValue(GetEnum(gridColIndexes.cSRate2Per), dtItemPublic.Rows[0]["Srate2Per"].ToString(), "PERC_FLOAT");
                                SetValue(GetEnum(gridColIndexes.cSRate2), dtItemPublic.Rows[0]["SRate2"].ToString(), "CURR_FLOAT");
                                SetValue(GetEnum(gridColIndexes.cSRate3Per), dtItemPublic.Rows[0]["Srate3Per"].ToString(), "PERC_FLOAT");
                                SetValue(GetEnum(gridColIndexes.cSRate3), dtItemPublic.Rows[0]["SRate3"].ToString(), "CURR_FLOAT");
                                SetValue(GetEnum(gridColIndexes.cSRate4Per), dtItemPublic.Rows[0]["Srate4Per"].ToString(), "PERC_FLOAT");
                                SetValue(GetEnum(gridColIndexes.cSRate4), dtItemPublic.Rows[0]["SRate4"].ToString(), "CURR_FLOAT");
                                SetValue(GetEnum(gridColIndexes.cSRate5Per), dtItemPublic.Rows[0]["Srate5Per"].ToString(), "PERC_FLOAT");
                                SetValue(GetEnum(gridColIndexes.cSRate5), dtItemPublic.Rows[0]["SRate5"].ToString(), "CURR_FLOAT");

                                SetValue(GetEnum(gridColIndexes.cCCessPer), dtItemPublic.Rows[0]["CessPer"].ToString(), "CURR_FLOAT");
                                SetValue(GetEnum(gridColIndexes.cCCompCessQty), dtItemPublic.Rows[0]["CompCessQty"].ToString(), "CURR_FLOAT");
                                SetTag(GetEnum(gridColIndexes.cSRate1Per), dgvItems.CurrentRow.Index, dtItemPublic.Rows[0]["SrateCalcMode"].ToString(), "");

                                SetTag(GetEnum(gridColIndexes.cCoolie), dgvItems.CurrentRow.Index, dtItemPublic.Rows[0]["Coolie"].ToString(), "");
                                SetValue(GetEnum(gridColIndexes.cCoolie), dtItemPublic.Rows[0]["Coolie"].ToString(), "CURR_FLOAT");

                                SetTag(GetEnum(gridColIndexes.cAgentCommPer), dgvItems.CurrentRow.Index, dtItemPublic.Rows[0]["agentCommPer"].ToString(), "");
                                SetValue(GetEnum(gridColIndexes.cAgentCommPer), dtItemPublic.Rows[0]["agentCommPer"].ToString(), "CURR_FLOAT");

                                if (clsVchType.DefaultTaxModeValue == 3) //GST
                                {
                                    //SetValue(GetEnum(gridColIndexes.cCGST), dtItemPublic.Rows[0]["CGSTTaxPer"].ToString(), "PERC_FLOAT");
                                    //SetValue(GetEnum(gridColIndexes.cSGST), dtItemPublic.Rows[0]["SGSTTaxPer"].ToString(), "PERC_FLOAT");
                                    //SetValue(GetEnum(gridColIndexes.cIGST), dtItemPublic.Rows[0]["IGSTTaxPer"].ToString(), "PERC_FLOAT");

                                    SetTag(GetEnum(gridColIndexes.cCGST), dgvItems.CurrentRow.Index, dtItemPublic.Rows[0]["CGSTTaxPer"].ToString(), "PERC_FLOAT");
                                    SetTag(GetEnum(gridColIndexes.cSGST), dgvItems.CurrentRow.Index, dtItemPublic.Rows[0]["SGSTTaxPer"].ToString(), "PERC_FLOAT");
                                    SetTag(GetEnum(gridColIndexes.cIGST), dgvItems.CurrentRow.Index, dtItemPublic.Rows[0]["IGSTTaxPer"].ToString(), "PERC_FLOAT");
                                    //SetTag(GetEnum(gridColIndexes.ctaxPer), dgvItems.CurrentRow.Index, dtItemPublic.Rows[0]["IGSTTaxPer"].ToString(), "PERC_FLOAT");
                                    SetValue(GetEnum(gridColIndexes.ctaxPer), dtItemPublic.Rows[0]["IGSTTaxPer"].ToString(), "PERC_FLOAT");
                                }
                                else
                                {
                                    //SetValue(GetEnum(gridColIndexes.cCGST), "0", "0");
                                    //SetValue(GetEnum(gridColIndexes.cSGST), "0", "0");
                                    //SetValue(GetEnum(gridColIndexes.cIGST), "0", "0");
                                    //SetValue(GetEnum(gridColIndexes.ctaxPer), "0", "0");
                                    SetTag(GetEnum(gridColIndexes.cCGST), dgvItems.CurrentRow.Index, "0", "PERC_FLOAT");
                                    SetTag(GetEnum(gridColIndexes.cSGST), dgvItems.CurrentRow.Index, "0", "PERC_FLOAT");
                                    SetTag(GetEnum(gridColIndexes.cIGST), dgvItems.CurrentRow.Index, "0", "PERC_FLOAT");
                                    //SetTag(GetEnum(gridColIndexes.ctaxPer), dgvItems.CurrentRow.Index, "0", "PERC_FLOAT");
                                    SetValue(GetEnum(gridColIndexes.ctaxPer), "0", "0");
                                }

                                if (Convert.ToInt32(dtItemPublic.Rows[0]["PRateInclusive"].ToString()) == 1)
                                    dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cRateinclusive)].Value = true;
                                else
                                    dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cRateinclusive)].Value = false;

                                if (clsVchType.ItmWiseDiscFillXtraDiscFromValue == 2) //Item Discount
                                    dItmWiseDisccount = Convert.ToDecimal(dtItemPublic.Rows[0]["DiscPer"].ToString());
                                else if (clsVchType.ItmWiseDiscFillXtraDiscFromValue == 3) //Category Discount
                                    dItmWiseDisccount = Convert.ToDecimal(dtItemPublic.Rows[0]["iCatDiscPer"].ToString());
                                else if (clsVchType.ItmWiseDiscFillXtraDiscFromValue == 4) //Manufacturer Discount
                                    dItmWiseDisccount = Convert.ToDecimal(dtItemPublic.Rows[0]["ImanDiscPer"].ToString());
                                else if (clsVchType.ItmWiseDiscFillXtraDiscFromValue == 5) //Discount Group Discount
                                    dItmWiseDisccount = Convert.ToDecimal(dtItemPublic.Rows[0]["DGroupDisPer"].ToString());
                                else if (clsVchType.ItmWiseDiscFillXtraDiscFromValue == 6) //Highest Discount
                                {
                                    lstItmDisc.Add(Convert.ToDecimal(dtItemPublic.Rows[0]["DiscPer"].ToString()));
                                    lstItmDisc.Add(Convert.ToDecimal(dtItemPublic.Rows[0]["iCatDiscPer"].ToString()));
                                    lstItmDisc.Add(Convert.ToDecimal(dtItemPublic.Rows[0]["ImanDiscPer"].ToString()));
                                    lstItmDisc.Add(Convert.ToDecimal(dtItemPublic.Rows[0]["DGroupDisPer"].ToString()));
                                    dDiscArray = lstItmDisc.ToArray();
                                    dItmWiseDisccount = dDiscArray.Max();
                                }
                                SetValue(GetEnum(gridColIndexes.cDiscPer), dItmWiseDisccount.ToString(), "PERC_FLOAT");

                                dtCurrExp = DateTime.Today;
                                if (Convert.ToDouble(dtItemPublic.Rows[0]["DefaultExpInDays"].ToString()) > 0)
                                    dtCurrExp = dtCurrExp.AddDays(Convert.ToDouble(dtItemPublic.Rows[0]["DefaultExpInDays"].ToString()));
                                else
                                    dtCurrExp = dtCurrExp.AddYears(8);

                                SetValue(GetEnum(gridColIndexes.CExpiry), dtCurrExp.ToString("dd-MMM-yyyy").ToString(), "DATE");
                                SetTag(GetEnum(gridColIndexes.CExpiry), dgvItems.CurrentRow.Index, dtItemPublic.Rows[0]["BlnExpiryItem"].ToString());
                                if (Convert.ToInt32(dtItemPublic.Rows[0]["BlnExpiryItem"].ToString()) == 1)
                                {
                                    dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.CExpiry)].ReadOnly = false;
                                }
                                else
                                {
                                    dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.CExpiry)].ReadOnly = true;
                                }

                                iBatchmode = Convert.ToInt32(dtItemPublic.Rows[0]["batchMode"].ToString());
                                SetValue(GetEnum(gridColIndexes.cBatchMode), iBatchmode.ToString());
                                iShelfLifeDays = Convert.ToInt32(dtItemPublic.Rows[0]["Shelflife"].ToString());

                                if (iBatchmode == 1)
                                {
                                    dgvItems.CurrentCell = dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cBarCode)];
                                    dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.CItemCode)].Value = dtItemPublic.Rows[0]["ItemCode"].ToString();
                                    //dgvItems.BeginEdit(true);
                                }
                                else if (iBatchmode == 2)
                                {
                                    //if (dtItemPublic.Rows[0]["BatchUnique"].ToString() != "")
                                    //{
                                    //    dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cBarCode)].Tag = "<Auto Barcode>";
                                    //    dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cBarCode)].Value = 0;
                                    //}
                                    //else
                                    //{
                                    //    dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cBarCode)].Tag = "<Auto Barcode>";
                                    //    dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cBarCode)].Value = 0;
                                    //}

                                    //dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cBarCode)].Tag = "<Auto Barcode>";
                                    //dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cBarCode)].Value = 0;

                                    dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.CItemCode)].Value = dtItemPublic.Rows[0]["ItemCode"].ToString();
                                    //FillGridAsperStockID(Convert.ToInt32(dtItemPublic.Rows[0]["StockID"].ToString()));

                                    dgvItems.CurrentCell = dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.CExpiry)];
                                    dgvItems.Focus();
                                }
                                else if (iBatchmode == 0 || iBatchmode == 3)
                                {
                                    if (dtItemPublic.Rows[0]["BatchUnique"].ToString() != "")
                                    {
                                        dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cBarCode)].Tag = dtItemPublic.Rows[0]["BatchCode"].ToString();
                                        dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cBarCode)].Value = dtItemPublic.Rows[0]["BatchUnique"].ToString();
                                    }
                                    else
                                    {
                                        dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cBarCode)].Tag = dtItemPublic.Rows[0]["BatchCode"].ToString();
                                        dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cBarCode)].Value = dtItemPublic.Rows[0]["BatchCode"].ToString();
                                    }

                                    dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.CItemCode)].Value = dtItemPublic.Rows[0]["ItemCode"].ToString();
                                    
                                    if(dtItemPublic.Rows[0]["StockID"].ToString() != "")
                                        FillGridAsperStockID(Convert.ToInt32(dtItemPublic.Rows[0]["StockID"].ToString()));

                                    dgvItems.CurrentCell = dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.CExpiry)];
                                    dgvItems.Focus();
                                }
                                dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.CItemCode)].Value = dtItemPublic.Rows[0]["ItemCode"].ToString();
                                //SetValue(GetEnum(gridColIndexes.CItemCode), dtItemPublic.Rows[0]["ItemCode"].ToString());
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

        //Created by : Arun
        //Description : What to happen when barcode is scanned
        private Boolean GetFromBarcodeSearch(string sReturn)
        {
            try
            {
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
                        if (sCompSearchData[0].ToString() != "")
                        {
                            GetItmMstBatchinfo.BatchUnique = sCompSearchData[0].ToString();
                            GetItmMstBatchinfo.TenantID = Global.gblTenantID;

                            dtItemPublic = clsItmMst.GetItemMasterBatchUnique(GetItmMstBatchinfo);

                            if (dtItemPublic.Rows.Count > 0)
                            {
                                if (dtItemPublic.Columns.Count > 11)
                                {
                                    SetValue(GetEnum(gridColIndexes.CItemName), dtItemPublic.Rows[0]["ItemName"].ToString());
                                    SetValue(GetEnum(gridColIndexes.CUnit), dtItemPublic.Rows[0]["Unit"].ToString());
                                    setTag(GetEnum(gridColIndexes.CUnit), dtItemPublic.Rows[0]["UNITID"].ToString());
                                    SetValue(GetEnum(gridColIndexes.cItemID), dtItemPublic.Rows[0]["ItemID"].ToString());
                                    setTag(GetEnum(gridColIndexes.CItemName), dtItemPublic.Rows[0]["ItemID"].ToString());
                                    SetValue(GetEnum(gridColIndexes.cBarCode), dtItemPublic.Rows[0]["BatchUnique"].ToString());
                                    setTag(GetEnum(gridColIndexes.cBarCode), dtItemPublic.Rows[0]["BatchCode"].ToString());
                                    //SetValue(GetEnum(gridColIndexes.CST), dtItemPublic.Rows[0]["StockID"].ToString());

                                    //
                                    SetValue(GetEnum(gridColIndexes.cSrate), dtItemPublic.Rows[0]["PRate"].ToString(), "CURR_FLOAT");

                                    SetValue(GetEnum(gridColIndexes.cMRP), dtItemPublic.Rows[0]["MRP"].ToString(), "CURR_FLOAT");
                                    
                                    SetValue(GetEnum(gridColIndexes.cCrate), dtItemPublic.Rows[0]["CostRateExcl"].ToString(), "CURR_FLOAT");
                                    SetValue(GetEnum(gridColIndexes.cCRateWithTax), dtItemPublic.Rows[0]["CostRateInc"].ToString(), "CURR_FLOAT");

                                    SetValue(GetEnum(gridColIndexes.cSRate1Per), dtItemPublic.Rows[0]["Srate1Per"].ToString(), "PERC_FLOAT");
                                    SetValue(GetEnum(gridColIndexes.cSRate1), dtItemPublic.Rows[0]["SRate1"].ToString(), "CURR_FLOAT");
                                    SetValue(GetEnum(gridColIndexes.cSRate2Per), dtItemPublic.Rows[0]["Srate2Per"].ToString(), "PERC_FLOAT");
                                    SetValue(GetEnum(gridColIndexes.cSRate2), dtItemPublic.Rows[0]["SRate2"].ToString(), "CURR_FLOAT");
                                    SetValue(GetEnum(gridColIndexes.cSRate3Per), dtItemPublic.Rows[0]["Srate3Per"].ToString(), "PERC_FLOAT");
                                    SetValue(GetEnum(gridColIndexes.cSRate3), dtItemPublic.Rows[0]["SRate3"].ToString(), "CURR_FLOAT");
                                    SetValue(GetEnum(gridColIndexes.cSRate4Per), dtItemPublic.Rows[0]["Srate4Per"].ToString(), "PERC_FLOAT");
                                    SetValue(GetEnum(gridColIndexes.cSRate4), dtItemPublic.Rows[0]["SRate4"].ToString(), "CURR_FLOAT");
                                    SetValue(GetEnum(gridColIndexes.cSRate5Per), dtItemPublic.Rows[0]["Srate5Per"].ToString(), "PERC_FLOAT");
                                    SetValue(GetEnum(gridColIndexes.cSRate5), dtItemPublic.Rows[0]["SRate5"].ToString(), "CURR_FLOAT");

                                    SetValue(GetEnum(gridColIndexes.cCCessPer), dtItemPublic.Rows[0]["CessPer"].ToString(), "CURR_FLOAT");
                                    SetValue(GetEnum(gridColIndexes.cCCompCessQty), dtItemPublic.Rows[0]["CompCessQty"].ToString(), "CURR_FLOAT");
                                    SetTag(GetEnum(gridColIndexes.cSRate1Per), dgvItems.CurrentRow.Index, dtItemPublic.Rows[0]["SrateCalcMode"].ToString(), "");

                                    SetTag(GetEnum(gridColIndexes.cCoolie), dgvItems.CurrentRow.Index, dtItemPublic.Rows[0]["Coolie"].ToString(), "");
                                    SetValue(GetEnum(gridColIndexes.cCoolie), dtItemPublic.Rows[0]["Coolie"].ToString(), "CURR_FLOAT");

                                    SetTag(GetEnum(gridColIndexes.cAgentCommPer), dgvItems.CurrentRow.Index, dtItemPublic.Rows[0]["agentCommPer"].ToString(), "");
                                    SetValue(GetEnum(gridColIndexes.cAgentCommPer), dtItemPublic.Rows[0]["agentCommPer"].ToString(), "CURR_FLOAT");

                                    if (clsVchType.DefaultTaxModeValue == 3) //GST
                                    {
                                        //SetValue(GetEnum(gridColIndexes.cCGST), dtItemPublic.Rows[0]["CGSTTaxPer"].ToString(), "PERC_FLOAT");
                                        //SetValue(GetEnum(gridColIndexes.cSGST), dtItemPublic.Rows[0]["SGSTTaxPer"].ToString(), "PERC_FLOAT");
                                        //SetValue(GetEnum(gridColIndexes.cIGST), dtItemPublic.Rows[0]["IGSTTaxPer"].ToString(), "PERC_FLOAT");

                                        SetTag(GetEnum(gridColIndexes.cCGST), dgvItems.CurrentRow.Index, dtItemPublic.Rows[0]["CGSTTaxPer"].ToString(), "PERC_FLOAT");
                                        SetTag(GetEnum(gridColIndexes.cSGST), dgvItems.CurrentRow.Index, dtItemPublic.Rows[0]["SGSTTaxPer"].ToString(), "PERC_FLOAT");
                                        SetTag(GetEnum(gridColIndexes.cIGST), dgvItems.CurrentRow.Index, dtItemPublic.Rows[0]["IGSTTaxPer"].ToString(), "PERC_FLOAT");
                                        //SetTag(GetEnum(gridColIndexes.ctaxPer), dgvItems.CurrentRow.Index, dtItemPublic.Rows[0]["IGSTTaxPer"].ToString(), "PERC_FLOAT");
                                        SetValue(GetEnum(gridColIndexes.ctaxPer), dtItemPublic.Rows[0]["IGSTTaxPer"].ToString(), "PERC_FLOAT");
                                    }
                                    else
                                    {
                                        //SetValue(GetEnum(gridColIndexes.cCGST), "0", "0");
                                        //SetValue(GetEnum(gridColIndexes.cSGST), "0", "0");
                                        //SetValue(GetEnum(gridColIndexes.cIGST), "0", "0");
                                        //SetValue(GetEnum(gridColIndexes.ctaxPer), "0", "0");
                                        SetTag(GetEnum(gridColIndexes.cCGST), dgvItems.CurrentRow.Index, "0", "PERC_FLOAT");
                                        SetTag(GetEnum(gridColIndexes.cSGST), dgvItems.CurrentRow.Index, "0", "PERC_FLOAT");
                                        SetTag(GetEnum(gridColIndexes.cIGST), dgvItems.CurrentRow.Index, "0", "PERC_FLOAT");
                                        //SetTag(GetEnum(gridColIndexes.ctaxPer), dgvItems.CurrentRow.Index, "0", "PERC_FLOAT");
                                        SetValue(GetEnum(gridColIndexes.ctaxPer), "0", "0");
                                    }

                                    if (Convert.ToInt32(dtItemPublic.Rows[0]["PRateInclusive"].ToString()) == 1)
                                        dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cRateinclusive)].Value = true;
                                    else
                                        dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cRateinclusive)].Value = false;

                                    if (clsVchType.ItmWiseDiscFillXtraDiscFromValue == 2) //Item Discount
                                        dItmWiseDisccount = Convert.ToDecimal(dtItemPublic.Rows[0]["DiscPer"].ToString());
                                    else if (clsVchType.ItmWiseDiscFillXtraDiscFromValue == 3) //Category Discount
                                        dItmWiseDisccount = Convert.ToDecimal(dtItemPublic.Rows[0]["iCatDiscPer"].ToString());
                                    else if (clsVchType.ItmWiseDiscFillXtraDiscFromValue == 4) //Manufacturer Discount
                                        dItmWiseDisccount = Convert.ToDecimal(dtItemPublic.Rows[0]["ImanDiscPer"].ToString());
                                    else if (clsVchType.ItmWiseDiscFillXtraDiscFromValue == 5) //Discount Group Discount
                                        dItmWiseDisccount = Convert.ToDecimal(dtItemPublic.Rows[0]["DGroupDisPer"].ToString());
                                    else if (clsVchType.ItmWiseDiscFillXtraDiscFromValue == 6) //Highest Discount
                                    {
                                        lstItmDisc.Add(Convert.ToDecimal(dtItemPublic.Rows[0]["DiscPer"].ToString()));
                                        lstItmDisc.Add(Convert.ToDecimal(dtItemPublic.Rows[0]["iCatDiscPer"].ToString()));
                                        lstItmDisc.Add(Convert.ToDecimal(dtItemPublic.Rows[0]["ImanDiscPer"].ToString()));
                                        lstItmDisc.Add(Convert.ToDecimal(dtItemPublic.Rows[0]["DGroupDisPer"].ToString()));
                                        dDiscArray = lstItmDisc.ToArray();
                                        dItmWiseDisccount = dDiscArray.Max();
                                    }
                                    SetValue(GetEnum(gridColIndexes.cDiscPer), dItmWiseDisccount.ToString(), "PERC_FLOAT");

                                    dtCurrExp = DateTime.Today;
                                    if (Convert.ToDouble(dtItemPublic.Rows[0]["DefaultExpInDays"].ToString()) > 0)
                                        dtCurrExp = dtCurrExp.AddDays(Convert.ToDouble(dtItemPublic.Rows[0]["DefaultExpInDays"].ToString()));
                                    else
                                        dtCurrExp = dtCurrExp.AddYears(8);

                                    SetValue(GetEnum(gridColIndexes.CExpiry), dtCurrExp.ToString("dd-MMM-yyyy").ToString(), "DATE");
                                    SetTag(GetEnum(gridColIndexes.CExpiry), dgvItems.CurrentRow.Index, dtItemPublic.Rows[0]["BlnExpiryItem"].ToString());
                                    if (Convert.ToInt32(dtItemPublic.Rows[0]["BlnExpiryItem"].ToString()) == 1)
                                    {
                                        dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.CExpiry)].ReadOnly = false;
                                    }
                                    else
                                    {
                                        dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.CExpiry)].ReadOnly = true;
                                    }

                                    iBatchmode = Convert.ToInt32(dtItemPublic.Rows[0]["batchMode"].ToString());
                                    SetValue(GetEnum(gridColIndexes.cBatchMode), iBatchmode.ToString());
                                    iShelfLifeDays = Convert.ToInt32(dtItemPublic.Rows[0]["Shelflife"].ToString());

                                    if (iBatchmode == 1)
                                    {
                                        //dgvItems.CurrentCell = dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cBarCode)];
                                        dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.CItemCode)].Value = dtItemPublic.Rows[0]["ItemCode"].ToString();
                                        dgvItems.BeginEdit(true);
                                    }
                                    else if (iBatchmode == 2)
                                    {
                                        //if (dtItemPublic.Rows[0]["BatchUnique"].ToString() != "")
                                        //{
                                        //    dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cBarCode)].Tag = "<Auto Barcode>";
                                        //    dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cBarCode)].Value = 0;
                                        //}
                                        //else
                                        //{
                                        //    dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cBarCode)].Tag = "<Auto Barcode>";
                                        //    dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cBarCode)].Value = 0;
                                        //}

                                        //dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cBarCode)].Tag = "<Auto Barcode>";
                                        //dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cBarCode)].Value = 0;

                                        dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.CItemCode)].Value = dtItemPublic.Rows[0]["ItemCode"].ToString();
                                        //FillGridAsperStockID(Convert.ToInt32(dtItemPublic.Rows[0]["StockID"].ToString()));

                                        if (dtItemPublic.Rows[0]["StockID"].ToString() != "")
                                            FillGridAsperStockID(Convert.ToInt32(dtItemPublic.Rows[0]["StockID"].ToString()));

                                        //dgvItems.CurrentCell = dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.CExpiry)];
                                        //dgvItems.Focus();
                                    }
                                    else if (iBatchmode == 0 || iBatchmode == 3)
                                    {
                                        dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.CItemCode)].Value = dtItemPublic.Rows[0]["ItemCode"].ToString();

                                        if (dtItemPublic.Rows[0]["StockID"].ToString() != "")
                                            FillGridAsperStockID(Convert.ToInt32(dtItemPublic.Rows[0]["StockID"].ToString()));
                                    }
                                    SetValue(GetEnum(gridColIndexes.CItemCode), dtItemPublic.Rows[0]["ItemCode"].ToString());

                                    dgvItems.CellEndEdit -= dgvItems_CellEndEdit;
                                    dgvItems.CurrentCell = dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.CExpiry)];
                                    dgvItems.Focus();
                                    dgvItems.CellEndEdit += dgvItems_CellEndEdit;


                                    return true;
                                }
                                else
                                {
                                    //CallForBatchSearch(sCompSearchData[0].ToString());
                                    return false;
                                }
                            }
                            else
                            {
                                //CallForBatchSearch(sCompSearchData[0].ToString());
                                return false;
                            }
                        }
                        else
                            return false;
                    }
                    else
                        return false;
                }

            }
            catch 
            {
                //MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }

        }

        private void CallForBatchSearch(string sCompSearchData)
        {
            try
            {
                string sQuery = "";
                if (sCompSearchData.Trim() == "")
                {
                    sQuery = "SELECT AnyWhere,BatchCode,ExpiryDate,MRP,QOH,StockID,ItemID FROM (";
                    sQuery = sQuery + "SELECT (BatchCode+CONVERT(VARCHAR,ExpiryDate)+CONVERT(VARCHAR,ISNULL(QOH,0))+CONVERT(VARCHAR,ISNULL(MRP,0))) as AnyWhere,BatchUnique as BatchCode,ExpiryDate,MRP,QOH,StockID,ItemID FROM tblStock  ";
                }
                else
                {
                    sQuery = "SELECT AnyWhere,BatchCode,ExpiryDate,MRP,QOH,StockID,ItemID FROM (";
                    sQuery = sQuery + "SELECT (BatchCode+CONVERT(VARCHAR,ExpiryDate)+CONVERT(VARCHAR,ISNULL(QOH,0))+CONVERT(VARCHAR,ISNULL(MRP,0))) as AnyWhere,BatchUnique as BatchCode,ExpiryDate,MRP,QOH,StockID,ItemID FROM tblStock Where BatchCode like '%" + sCompSearchData.ToString() + "%' ";
                }

                if (sQuery != "")
                {
                    sQuery = sQuery + " ) as A ";
                    frmBatchSearch = new frmCompactSearch(GetFromBarcodeSearch, sQuery, "Anywhere|BatchCode|CONVERT(VARCHAR,ExpiryDate)|CONVERT(VARCHAR,ISNULL(MRP,0))|CONVERT(VARCHAR,ISNULL(QOH,0))", dgvItems.Location.X + 350, dgvItems.Location.Y + 150, 4, 0, "", 4, 0, "ORDER BY A.BatchCode ASC", 0, 0, "BatchCode Search...", 0, "150,90,80,80,0,0", true, "BatchCode", 10);
                    frmBatchSearch.Show();
                    frmBatchSearch.BringToFront();
                }
            }
            catch 
            {
                //MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        // Created By : Dipu 
        // Created On : 21-Feb-2022
        // Description: To Calculate Tax When TaxMode Combo Box Change
        private void TaxCalculate()
        {
            if (dgvItems.Rows.Count > 0)
            {
                for (int k = 0; k < dgvItems.Rows.Count; k++)
                {
                    dgvItems.Rows[k].Cells[GetEnum(gridColIndexes.cCGST)].Tag = "0";
                    dgvItems.Rows[k].Cells[GetEnum(gridColIndexes.cSGST)].Tag = "0";
                    dgvItems.Rows[k].Cells[GetEnum(gridColIndexes.cIGST)].Tag = "0";
                    dgvItems.Rows[k].Cells[GetEnum(gridColIndexes.ctaxPer)].Tag = Convert.ToDecimal(dtItemPublic.Rows[0]["VAT"].ToString()).ToString("#.00");

                    SetTag(GetEnum(gridColIndexes.cCGST), dgvItems.CurrentRow.Index, "0", "0");
                    SetTag(GetEnum(gridColIndexes.cSGST), dgvItems.CurrentRow.Index, "0", "0");
                    SetTag(GetEnum(gridColIndexes.cIGST), dgvItems.CurrentRow.Index, "0", "0");
                    SetTag(GetEnum(gridColIndexes.ctaxPer), dgvItems.CurrentRow.Index, Convert.ToDecimal(dtItemPublic.Rows[0]["VAT"].ToString()).ToString("#.00"), "0");

                    dgvItems.Columns["cCGST"].Visible = false;
                    dgvItems.Columns["cSGST"].Visible = false;
                    dgvItems.Columns["cIGST"].Visible = false;
                    dgvItems.Columns["ctaxPer"].Visible = false;
                    dgvItems.Columns["ctax"].Visible = false;
                    dgvItems.Columns["ctaxable"].Visible = false;
                    dgvItems.Columns["cCRateWithTax"].Visible = false;
                }

            }

            if (dgvItems.Rows.Count > 1)
            {
                for (int k = 0; k < dgvItems.Rows.Count; k++)
                {
                    if (dgvItems.Rows[k].Cells[GetEnum(gridColIndexes.CItemCode)].Value != null)
                    {

                        GetItmMstinfo.ItemID = Convert.ToDecimal(dgvItems.Rows[k].Cells[GetEnum(gridColIndexes.cItemID)].Value.ToString());
                        GetItmMstinfo.TenantID = Global.gblTenantID;

                        dtItemPublic = clsItmMst.GetItemMaster(GetItmMstinfo);
                        if (dtItemPublic.Rows.Count > 0)
                        {
                            CalcTotal();
                        }
                    }
                }
            }
            else
            {
                CalcTotal();
            }
        }

        //Description : Row Delete when Press Delete or Delete icon
        private void RowDelete()
        {
            int rowIndex = dgvItems.CurrentCell.RowIndex;
            dgvItems.Rows.RemoveAt(rowIndex);
            //decimal dinvid = GetStockJournalIfo.InvId;
        }

        //Description : Initializing the Columns in The Grid
        private void AddColumnsToGrid()
        {
            this.dgvItems.Columns.Clear();

            this.dgvItems.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSlNo", HeaderText = "Sl.No", Width = 50 }); //1

            this.dgvItems.Columns.Add(new DataGridViewTextBoxColumn() { Name = "CItemCode", HeaderText = "Item Code", Width = 130 }); //1
            this.dgvItems.Columns.Add(new DataGridViewTextBoxColumn() { Name = "CItemName", HeaderText = "Item Name", Width = 200 }); //2
            this.dgvItems.Columns.Add(new DataGridViewTextBoxColumn() { Name = "CUnit", ReadOnly = true, Visible = true, HeaderText = "Unit", Width = 50 }); //3
                                                                                                                                                                //Commented and added By Dipu on 23-Feb-2022 ------------- >>
            this.dgvItems.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cBarCode", HeaderText = "Batch Code", Width = 200 }); //4

            this.dgvItems.Columns.Add(new DataGridViewTextBoxColumn() { Name = "CExpiry", HeaderText = "Expiry Date", Width = 120 }); //5

            if (AppSettings.IsActiveMRP == true)
                this.dgvItems.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cMRP", HeaderText = "" + AppSettings.MRPName + "", ReadOnly = false, Visible = true, Width = 80 }); //6
            else
                this.dgvItems.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cMRP", HeaderText = "" + AppSettings.MRPName + "", ReadOnly = false, Visible = false, Width = 80 }); //6

            this.dgvItems.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSrate", HeaderText = "SRate", ReadOnly = true, Visible = false, Width = 80 }); //7

            this.dgvItems.Columns.Add(new DataGridViewCheckBoxColumn() { Name = "cRateinclusive", HeaderText = "Rate Inc.", Visible = false, Width = 80 }); //20

            this.dgvItems.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cQty", HeaderText = "Qty", Width = 80 }); //8
            this.dgvItems.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cQOH", HeaderText = "QOH", Visible = false, ReadOnly = true, Width = 0 }); //9

            this.dgvItems.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate1Per", HeaderText = "" + AppSettings.SRate1Name + " %", ReadOnly = true, Visible = false, Width = 80 }); //10
            this.dgvItems.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate1", HeaderText = "" + AppSettings.SRate1Name + "", ReadOnly = false, Visible = true, Width = 80 }); //11
            if (AppSettings.IsActiveSRate2 == true)
            {
                this.dgvItems.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate2Per", HeaderText = "" + AppSettings.SRate2Name + " %", ReadOnly = true, Visible = false, Width = 80 }); //12
                this.dgvItems.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate2", HeaderText = "" + AppSettings.SRate2Name + "", ReadOnly = false, Visible = true, Width = 80 }); //13
            }
            else
            {
                this.dgvItems.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate2Per", HeaderText = "" + AppSettings.SRate2Name + " %", ReadOnly = true, Visible = false, Width = 80 }); //12
                this.dgvItems.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate2", HeaderText = "" + AppSettings.SRate2Name + "", ReadOnly = false, Visible = true, Width = 80 }); //13
            }

            if (AppSettings.IsActiveSRate3 == true)
            {
                this.dgvItems.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate3Per", HeaderText = "" + AppSettings.SRate3Name + " %", ReadOnly = true, Visible = false, Width = 80 }); //14
                this.dgvItems.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate3", HeaderText = AppSettings.SRate3Name, ReadOnly = false, Visible = true, Width = 80 }); //15
            }
            else
            {
                this.dgvItems.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate3Per", HeaderText = "" + AppSettings.SRate3Name + " %", ReadOnly = true, Visible = false, Width = 80 }); //14
                this.dgvItems.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate3", HeaderText = AppSettings.SRate3Name, ReadOnly = false, Visible = true, Width = 80 }); //15
            }

            if (AppSettings.IsActiveSRate4 == true)
            {
                this.dgvItems.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate4Per", HeaderText = "" + AppSettings.SRate4Name + " %", ReadOnly = true, Visible = false, Width = 80 }); //16
                this.dgvItems.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate4", HeaderText = AppSettings.SRate4Name, ReadOnly = false, Visible = true, Width = 80 }); //17
            }
            else
            {
                this.dgvItems.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate4Per", HeaderText = "" + AppSettings.SRate4Name + " %", ReadOnly = true, Visible = false, Width = 80 }); //16
                this.dgvItems.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate4", HeaderText = AppSettings.SRate4Name, ReadOnly = false, Visible = true, Width = 80 }); //17
            }

            if (AppSettings.IsActiveSRate5 == true)
            {
                this.dgvItems.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate5Per", HeaderText = "" + AppSettings.SRate5Name + " %", ReadOnly = true, Visible = false, Width = 80 }); //18
                this.dgvItems.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate5", HeaderText = AppSettings.SRate5Name, ReadOnly = false, Visible = true, Width = 80 }); //19
            }
            else
            {
                this.dgvItems.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate5Per", HeaderText = "" + AppSettings.SRate5Name + " %", ReadOnly = true, Visible = false, Width = 80 }); //18
                this.dgvItems.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate5", HeaderText = AppSettings.SRate5Name, ReadOnly = false, Visible = true, Width = 80 }); //19
            }

            this.dgvItems.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cGrossAmt", HeaderText = "Gross Amt", ReadOnly = true, Width = 80 }); //23

            try
            {
                if (clsVchType == null)
                {
                    MessageBox.Show("Voucher settings incorrect for the voucher. Please correct the settings and open the voucher again.", "Sales Settings", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            catch 
            {

            }

            if (clsVchType != null)
            {
                this.dgvItems.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cDiscPer", HeaderText = "Discount %", ReadOnly = true, Visible = false, Width = 80 }); //24
                this.dgvItems.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cDiscAmount", HeaderText = "Discount Amt", ReadOnly = true, Visible = false, Width = 80 }); //25
            }
            this.dgvItems.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cBillDisc", HeaderText = "Bill Discount", ReadOnly = true, Visible = false, Width = 80 }); //26
            this.dgvItems.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cCrate", HeaderText = "CRate", ReadOnly = true, Width = 80 }); //27

            this.dgvItems.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cCRateWithTax", HeaderText = "CRate With Tax", ReadOnly = true, Width = 80 }); //28

            this.dgvItems.Columns.Add(new DataGridViewTextBoxColumn() { Name = "ctaxable", HeaderText = "Taxable", ReadOnly = true, Visible = false, Width = 80 }); //29

            if (clsVchTypeFeatures.blnEditTaxPer == true)
                this.dgvItems.Columns.Add(new DataGridViewTextBoxColumn() { Name = "ctaxPer", HeaderText = "Tax %", ReadOnly = false, Visible=false, Width = 80 }); //30
            else
                this.dgvItems.Columns.Add(new DataGridViewTextBoxColumn() { Name = "ctaxPer", HeaderText = "Tax %", ReadOnly = true, Visible=false, Width = 80 }); //30

            this.dgvItems.Columns.Add(new DataGridViewTextBoxColumn() { Name = "ctax", HeaderText = "Tax", Visible = false, Width = 80 }); //31
            this.dgvItems.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cIGST", HeaderText = "IGST", Visible = false, Width = 80 }); //32
            this.dgvItems.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSGST", HeaderText = "SGST", Visible = false, Width = 80 }); //33
            this.dgvItems.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cCGST", HeaderText = "CGST", Visible = false, Width = 80 }); //34
            
            this.dgvItems.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cNetAmount", HeaderText = "Net Amt", Width = 100 }); //35
            this.dgvItems.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cItemID", HeaderText = "ItemID", Visible = false, Width = 80 }); //36

            this.dgvItems.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cGrossValueAfterRateDiscount", HeaderText = "Gross Val", Visible = false }); //37
            this.dgvItems.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cNonTaxable", HeaderText = "Non Taxable", Visible = false }); //38
            this.dgvItems.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cCCessPer", HeaderText = "Cess %", Visible = false }); //39
            this.dgvItems.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cCCompCessQty", HeaderText = "Comp Cess Qty", Visible = false }); //40
            this.dgvItems.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cFloodCessPer", HeaderText = "Flood Cess %", Visible = false }); //41
            this.dgvItems.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cFloodCessAmt", HeaderText = "Flood Cess Amt", Visible = false }); //42
            this.dgvItems.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cStockMRP", HeaderText = "Stock MRP", Visible = false }); //43
            this.dgvItems.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cAgentCommPer", HeaderText = "Agent Comm. %", Visible = false }); //44
            this.dgvItems.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cCoolie", HeaderText = "Coolie", Visible = false }); //45
            this.dgvItems.Columns.Add(new DataGridViewCheckBoxColumn() { Name = "cBlnOfferItem", HeaderText = "Offer Item", Visible = false }); //46
            this.dgvItems.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cStrOfferDetails", HeaderText = "Offer Det.", Visible = false }); //47
            this.dgvItems.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cBatchMode", HeaderText = "Batch Mode", Visible = false }); //48
            this.dgvItems.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cID", HeaderText = "ID", Visible = false });
            this.dgvItems.Columns.Add(new DataGridViewImageColumn() { Name = "cImgDel", HeaderText="", Image = Properties.Resources.Delete_24_P4, Width=40 });
            this.dgvItems.Columns.Add(new DataGridViewImageColumn() { Name = "cBatchUnique", HeaderText="", Image = Properties.Resources.Delete_24_P4, Width=40, Visible=false });

            //Dipoos 21-03-2022
            //if (iIDFromEditWindow==0)
            //dgvItems.Rows.Add(2);
            //else

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
                            if (dtGridSettings.Rows[k][3].ToString() == dgvItems.Columns[k].Name)
                            {
                                if (dgvItems.Columns[k].Name.ToUpper().Trim() == "ID")
                                {
                                    dgvItems.Columns[k].Width = Convert.ToInt32(dtGridSettings.Rows[k][2].ToString());
                                    dgvItems.Columns[k].Visible = false;
                                }
                                else if (dgvItems.Columns[k].Name.ToUpper().Trim() == "ItemID")
                                {
                                    dgvItems.Columns[k].Width = Convert.ToInt32(dtGridSettings.Rows[k][2].ToString());
                                    dgvItems.Columns[k].Visible = false;
                                }
                                else
                                {
                                    dgvItems.Columns[k].Width = Convert.ToInt32(dtGridSettings.Rows[k][2].ToString());
                                    dgvItems.Columns[k].Visible = Convert.ToBoolean(dtGridSettings.Rows[k][0]);
                                }
                            }

                        }
                    }
                }
                LoadGridWidthFromItemGrid();
            }
            else
            {
                LoadGridWidthFromItemGrid();
                SaveGridSettings();
            }
            
            dgvItems.Columns["cRateinclusive"].Visible = false;
            dgvItems.Columns[GetEnum(gridColIndexes.ctaxable)].Visible = false;

            dgvItems.Columns["cSlNo"].Frozen = true;
            //dgvItems.Columns["cImgDel"].Frozen = true;
            dgvItems.Columns["cImgDel"].Visible = true;
            dgvItems.Columns["cImgDel"].Width = 40;

            //DisableGridSettingsCheckbox();
        }

        private void flowLPnlBottom_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                frmEditWindow frmEdit = new frmEditWindow(this.Name.ToUpper(), this.MdiParent);
                //frmEdit.ShowDialog();
                frmEdit.Show();
                frmEdit.BringToFront();
                //this.Close();
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
                if (dgvItems.CurrentCell.ColumnIndex == GetEnum(gridColIndexes.cSlNo))
                {
                    dgvItems.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                    dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cSlNo)].ReadOnly = true;

                    strSelectedItemName = Convert.ToString(dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.CItemCode)].Value);
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
                string SSelectedItemCode = Convert.ToString(dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.CItemCode)].Value);
                if (SSelectedItemCode != "")
                {
                    DialogResult dlgResult = MessageBox.Show("Are you sure to delete Item Code[" + SSelectedItemCode + "] Permanently ?", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                    if (dlgResult == DialogResult.Yes)
                    {

                        Int32 selectedRowCount = dgvItems.Rows.GetRowCount(DataGridViewElementStates.Selected);
                        RowDelete();

                        dgvItems.Rows.Add();
                        dgvItems.CurrentCell = dgvItems.Rows[0].Cells[GetEnum(gridColIndexes.CItemCode)];

                        CalcTotal();
                    }
                }
            }

        }

        private void dgvItems_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                double dSelectedItemID = 0;
                if (dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.CItemCode)].Value != null)
                {
                    dSelectedItemID = Convert.ToDouble(dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cItemID)].Value);
                    if (dSelectedItemID > 0)
                    {
                        if (dgvItems.CurrentCell.ColumnIndex == (int)gridColIndexes.CItemCode)
                        {
                            frmItemMaster frmIM = new frmItemMaster(Convert.ToInt32(dSelectedItemID), true, "E");
                            frmIM.ShowDialog();
                        }
                        else if (dgvItems.CurrentCell.ColumnIndex == (int)gridColIndexes.CItemName)
                        {
                            frmItemMaster frmIM = new frmItemMaster(Convert.ToInt32(dSelectedItemID), true, "E");
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
                //if (dgvItems.CurrentCell.ColumnIndex == GetEnum(gridColIndexes.cBarCode))
                //{
                //    if (dgvItems.CurrentCell.Value != null)
                //    {
                //        if (GetFromBarcodeSearch(dgvItems.CurrentCell.Value.ToString()) == false)
                //        {
                //            CallForBatchSearch(dgvItems.CurrentCell.Value.ToString());
                //        }
                //        else
                //        {
                //            dgvItems.CurrentCell = dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.CExpiry)];
                //            dgvItems.Focus();
                //        }
                //    }
                //}
                //else if (dgvItems.CurrentCell.ColumnIndex == GetEnum(gridColIndexes.cQty))

                if (dgvItems.CurrentCell.ColumnIndex == GetEnum(gridColIndexes.cQty))
                {
                    dResult = Convert.ToDecimal(dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cQty)].Value);
                    SetValue(GetEnum(gridColIndexes.cQty), dResult.ToString(), "QTY_FLOAT");
                    SendKeys.Send("{Tab}");

                    if (dgvItems.Rows.Count - 1 == dgvItems.CurrentRow.Index)
                        dgvItems.Rows.Add();

                    //Added by Anjitha 28/01/2022 5:30 PM
                    bool bshellife = ShelfLifeEffect();
                    if (bshellife == false)
                    {
                        dgvItems.Focus();
                        SetValue(GetEnum(gridColIndexes.CExpiry), dtCurrExp.ToString("dd-MMM-yyyy").ToString(), "DATE");
                    }
                }
                else if (dgvItems.CurrentCell.ColumnIndex == GetEnum(gridColIndexes.cQOH))
                {
                    dResult = Convert.ToDecimal(dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cQOH)].Value);
                    SetValue(GetEnum(gridColIndexes.cQOH), dResult.ToString(), "QTY_FLOAT");

                    SendKeys.Send("{up}");
                    SendKeys.Send("{right}");
                }
                else if (dgvItems.CurrentCell.ColumnIndex == GetEnum(gridColIndexes.cDiscPer))
                {
                    dResult = Convert.ToDecimal(dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cGrossAmt)].Value) * (Convert.ToDecimal(dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cDiscPer)].Value) / 100);
                    SetValue(GetEnum(gridColIndexes.cDiscAmount), dResult.ToString(), "CURR_FLOAT");
                }
                else if (dgvItems.CurrentCell.ColumnIndex == GetEnum(gridColIndexes.cDiscAmount))
                {
                    dResult = (Convert.ToDecimal(dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cDiscAmount)].Value) * 100) / Convert.ToDecimal(dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cGrossAmt)].Value);
                    SetValue(GetEnum(gridColIndexes.cDiscPer), dResult.ToString(), "PERC_FLOAT");
                }
                else if (dgvItems.CurrentCell.ColumnIndex == GetEnum(gridColIndexes.cMRP))
                {
                    SendKeys.Send("{up}");
                    SendKeys.Send("{right}");
                }
                else if (dgvItems.CurrentCell.ColumnIndex == GetEnum(gridColIndexes.cSrate))
                {
                    SendKeys.Send("{up}");
                    SendKeys.Send("{right}");
                }
                this.dgvEndEditCell = dgvItems[e.ColumnIndex, e.RowIndex];
                if (dgvItems.Rows.Count == e.RowIndex && e.ColumnIndex != dgvItems.Columns.Count - 1 && e.ColumnIndex <= GetEnum(gridColIndexes.cDiscAmount))
                {
                    if (dgvItems.CurrentCell.ColumnIndex != GetEnum(gridColIndexes.cRateinclusive))
                        SendKeys.Send("{Tab}");
                }
                else if (e.ColumnIndex == GetEnum(gridColIndexes.cDiscAmount))
                {
                    dgvItems.CurrentCell = dgvItems[GetEnum(gridColIndexes.CItemCode), e.RowIndex + 1];
                }
                else if (e.ColumnIndex >= GetEnum(gridColIndexes.cSRate1Per) && e.ColumnIndex < GetEnum(gridColIndexes.cDiscAmount))
                {
                    SendKeys.Send("{up}");
                    SendKeys.Send("{right}");
                }
                CalcTotal();
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
                                    dgvItems.CurrentCell = dgvItems[iColumn, iRowNo + 1];
                                else
                                {
                                    if (iColumn == GetEnum(gridColIndexes.cSrate))
                                    {
                                        SendKeys.Send("{Tab}");
                                        //dgvItems.CurrentCell = dgvItems[iColumn + 1, iRow];
                                    }
                                    else if (iColumn == GetEnum(gridColIndexes.cMRP))
                                    {
                                        SendKeys.Send("{Tab}");
                                        //dgvItems.CurrentCell = dgvItems[iColumn + 1, iRow];
                                    }
                                    else if (iColumn == GetEnum(gridColIndexes.cQty))
                                    {
                                        if (iRow < 0)
                                        {
                                            iRow = 0;

                                            if (dgvItems.Rows.Count <= iRow + 1)
                                                dgvItems.Rows.Add();

                                            if (clsVchTypeFeatures.BLNSHOWFREEQUANTITY == true)
                                                dgvItems.CurrentCell = dgvItems[GetEnum(gridColIndexes.cQOH), iRow];
                                            else if (dgvItems.Columns[GetEnum(gridColIndexes.cDiscPer)].Visible == true)
                                                dgvItems.CurrentCell = dgvItems[GetEnum(gridColIndexes.cDiscPer), iRow];
                                            else if (dgvItems.Columns[GetEnum(gridColIndexes.cDiscAmount)].Visible == true)
                                                dgvItems.CurrentCell = dgvItems[GetEnum(gridColIndexes.cDiscAmount), iRow];
                                            else if (GetEnum(gridColIndexes.CItemCode) == 1)
                                                dgvItems.CurrentCell = dgvItems[GetEnum(gridColIndexes.CItemCode), iRow + 1];
                                            else if (GetEnum(gridColIndexes.cBarCode) == 1)
                                                dgvItems.CurrentCell = dgvItems[GetEnum(gridColIndexes.cBarCode), iRow + 1];
                                        }
                                        else
                                        {
                                            if (clsVchTypeFeatures.BLNSHOWFREEQUANTITY == true)
                                                dgvItems.CurrentCell = dgvItems[GetEnum(gridColIndexes.cQOH), iRow];
                                            else
                                            {
                                                if (dgvItems.Rows.Count <= iRow + 1)
                                                    dgvItems.Rows.Add();

                                                if (clsVchTypeFeatures.BLNSHOWFREEQUANTITY == true)
                                                    dgvItems.CurrentCell = dgvItems[GetEnum(gridColIndexes.cQOH), iRow];
                                                else if (dgvItems.Columns[GetEnum(gridColIndexes.cDiscPer)].Visible == true)
                                                    dgvItems.CurrentCell = dgvItems[GetEnum(gridColIndexes.cDiscPer), iRow];
                                                else if (dgvItems.Columns[GetEnum(gridColIndexes.cDiscAmount)].Visible == true)
                                                    dgvItems.CurrentCell = dgvItems[GetEnum(gridColIndexes.cDiscAmount), iRow];
                                                else if (GetEnum(gridColIndexes.CItemCode) == 1)
                                                    dgvItems.CurrentCell = dgvItems[GetEnum(gridColIndexes.CItemCode), iRow + 1];
                                                else if (GetEnum(gridColIndexes.cBarCode) == 1)
                                                    dgvItems.CurrentCell = dgvItems[GetEnum(gridColIndexes.cBarCode), iRow + 1];

                                                //dgvItems.CurrentCell = dgvItems[iColumn + 2, iRow];
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
                CalcTotal();
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
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
                    catch 
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
                    else if (dgvItems.CurrentCell.ColumnIndex == GetEnum(gridColIndexes.cBarCode))
                    {
                        if (clsVchType.DefaultBarcodeMode != 0)
                        {
                            DataGridViewTextBoxEditingControl tb = (DataGridViewTextBoxEditingControl)e.Control;
                            tb.KeyPress += new KeyPressEventHandler(dgvItems_TextBox_KeyPress);
                            e.Control.KeyPress += new KeyPressEventHandler(dgvItems_TextBox_KeyPress);
                        }
                        else
                        {
                            CallBatchCodeCompact();

                            dgvItems.CurrentCell = dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.CExpiry)];
                            dgvItems.Focus();

                        }
                    }
                    else if (dgvItems.CurrentCell.ColumnIndex >= GetEnum(gridColIndexes.cMRP) && dgvItems.CurrentCell.ColumnIndex < GetEnum(gridColIndexes.cNetAmount))
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
                    if (iColumn == GetEnum(gridColIndexes.cRateinclusive))
                    {
                        dgvItems.CurrentCell = dgvItems[GetEnum(gridColIndexes.cRateinclusive) - 1, iRow];
                    }
                    else if (iColumn == dgvItems.Columns.Count - 1)//&& iRow != dgvItems.Rows.Count
                    {
                        if (dgvItems.Rows.Count <= iRow + 1)
                            dgvItems.Rows.Add();
                        dgvItems.CurrentCell = dgvItems[0, iRow - 1];
                    }
                    else
                        SendKeys.Send("+{Tab}");
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    int iColumn = dgvItems.CurrentCell.ColumnIndex;
                    iRow = dgvItems.CurrentCell.RowIndex;
                    if (iColumn == dgvItems.Columns.Count - 1 && iRow != dgvItems.Rows.Count)
                    {
                        dgvItems.CurrentCell = dgvItems[0, iRow + 1];
                    }
                    else if (iColumn == dgvItems.Columns.Count - 1 && iRow == dgvItems.Rows.Count)
                    {
                    }
                    else if (iColumn == GetEnum(gridColIndexes.cDiscAmount))
                    {
                        //Dipoos 22-03-2022----- >
                        if (dgvItems.Rows.Count <= iRow + 1)
                            dgvItems.Rows.Add();
                        
                        if (GetEnum(gridColIndexes.CItemCode) == 1)
                            dgvItems.CurrentCell = dgvItems[GetEnum(gridColIndexes.CItemCode), iRow + 1];
                        if (GetEnum(gridColIndexes.cBarCode) == 1)
                            dgvItems.CurrentCell = dgvItems[GetEnum(gridColIndexes.cBarCode), iRow + 1];
                    }
                    else if (iColumn == GetEnum(gridColIndexes.cRateinclusive))
                    {
                        dgvItems.CurrentCell = dgvItems[GetEnum(gridColIndexes.cRateinclusive) + 1, iRow];
                    }
                    else
                    {
                        SendKeys.Send("{Tab}");
                        //SendKeys.Send("{up}");
                        //SendKeys.Send("{right}");
                    }
                }
                else if (e.KeyCode == Keys.F3)
                {
                    if (dgvItems.CurrentCell.ColumnIndex == (int)gridColIndexes.CItemCode)
                    {
                        frmItemMaster frmim = new frmItemMaster(0, true, "S");
                        frmim.ShowDialog();
                    }
                }
                else if (e.KeyCode == Keys.F4)
                {
                    if (dgvItems.CurrentCell.ColumnIndex == (int)gridColIndexes.CItemCode)
                    {
                        int iSelectedItemID = 0;
                        iSelectedItemID = Convert.ToInt32(dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cItemID)].Value);
                        if (iSelectedItemID > 0)
                        {
                            frmItemMaster frmIM = new frmItemMaster(iSelectedItemID, true, "E");
                            frmIM.ShowDialog();
                        }

                    }
                }
                else if (e.KeyCode == Keys.Delete)
                {
                    string SSelectedItemCode = Convert.ToString(dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.CItemCode)].Value);
                    if ((SSelectedItemCode != "" || dgvItems.Rows.Count > 1) && dgvItems.CurrentRow.Index >= 0)
                    {
                        DialogResult dlgResult = MessageBox.Show("Are you sure to delete Item Code[" + SSelectedItemCode + "] Permanently ?", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                        if (dlgResult == DialogResult.Yes)
                        {
                            Int32 selectedRowCount = dgvItems.Rows.GetRowCount(DataGridViewElementStates.Selected);
                            RowDelete();
                            //dipoos 21-03-2022
                            //if (dgvItems.Rows.Count < 2)
                            //    dgvItems.Rows.Add();
                            if (dgvItems.Rows.Count < 1)
                                dgvItems.Rows.Add();

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

                    if (dgvItems.CurrentCell.ColumnIndex == (int)gridColIndexes.CItemCode)
                    {
                        if (sEditedValueonKeyPress != null)
                        {
                            sQuery = "SELECT ItemCode + ItemName + BatchUnique + CAST(MRP AS varchar) AS anywhere, ItemCode, ItemName, BatchUnique, QOH, MRP, ItemID, StockID " +
                                    " FROM     vwCompactSearch Where isnull(ActiveStatus, 1) = 1 and isnull(StockActiveStatus, 1) = 1 ";
                            if (clsVchType.ProductClassList != "")
                                sQuery = sQuery + " AND ProductTypeID IN (" + clsVchType.ProductClassList + ")";
                            if (clsVchType.ItemCategoriesList != "")
                                sQuery = sQuery + " AND CategoryID IN (" + clsVchType.ItemCategoriesList + ")";

                            if (mblnInitialisedSubWindow == false)
                            {
                                mblnInitialisedSubWindow = true;
                                frmDetailedSearch2 frmN = new frmDetailedSearch2(GetFromItemSearch, sQuery, "Anywhere|ItemCode|ItemName|BatchUnique", dgvItems.Location.X + 55, dgvItems.Location.Y + 150, 7, 0, sEditedValueonKeyPress, 6, 0, "ORDER BY ItemCode ASC, Batchunique", 0, 0, "Item Search...", 0, "150,250,150,150,150,0,0", true, "frmItemMaster", 20, false, this.MdiParent, 6);
                                frmN.MdiParent = this.MdiParent;
                                frmN.Show(); //22-Apr-2022
                            }
                            if (dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cItemID)].Value != null)
                            {
                                this.dgvItems.EditingControlShowing -= this.dgvItems_EditingControlShowing;
                                dgvItems.CurrentCell = dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cBarCode)];
                                dgvItems.Focus();
                                this.dgvItems.EditingControlShowing += this.dgvItems_EditingControlShowing;
                            }
                        }
                    }
                    else if (dgvItems.CurrentCell.ColumnIndex == GetEnum(gridColIndexes.cBarCode))
                    {
                        Form fc = Application.OpenForms["frmDetailedSearch2"];
                        if (fc != null)
                        {
                            fcc.Focus();
                            fcc.BringToFront();
                            return;
                        }
                        // BatchCode List Will Work only to MNF and Auto BatchMode Cases... Asper Discuss with Anup sir and Team on 13-May-2022 Evening Meeting.
                        if (Convert.ToInt32(dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cBatchMode)].Value) == 1) // MNF
                            CallBatchCodeCompact(true);
                        else if (Convert.ToInt32(dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cBatchMode)].Value) == 2) // Auto
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

        private void dgvItems_KeyPress(object sender, KeyPressEventArgs e)
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
            dtp.Visible = false;
        }

        private decimal FetchRateFromItemMaster(int RowNumer)
        {
            int itemid = Convert.ToInt32(dgvItems.Rows[RowNumer].Cells[GetEnum(gridColIndexes.cItemID)].Value);

            string batchunique = "";
            if (dgvItems.Rows[RowNumer].Cells[GetEnum(gridColIndexes.cBarCode)].Value != null)
                batchunique = dgvItems.Rows[RowNumer].Cells[GetEnum(gridColIndexes.cBarCode)].Value.ToString();

            DataTable dtPL = Comm.fnGetData("SELECT top 1 Prate FROM tblItemMaster Where ItemID=" + itemid + " ").Tables[0];
            if (dtPL != null)
            {
                if (dtPL.Rows.Count > 0)
                    return Convert.ToDecimal(dtPL.Rows[0][0].ToString());
                else
                    return 0;
            }
            else
                return 0;
        }

        private void txtQtyCess_KeyDown(object sender, KeyEventArgs e)
        {
            SendKeys.Send("{TAB}");

        }

        private void txtCoolie_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtCoolie_KeyDown(object sender, KeyEventArgs e)
        {
            SendKeys.Send("{TAB}");

        }

        private void frmOpening_Activated(object sender, EventArgs e)
        {
            try
            {
                LoadGridWidthFromItemGrid();
                DisableGridSettingsCheckbox();

                GridSettingsEnableDisable(Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active);
            }
            catch 
            {

            }
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
            catch 
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

        private void toggleWidthSettings_ToggleStateChanged(object sender, Syncfusion.Windows.Forms.Tools.ToggleStateChangedEventArgs e)
        {
            try
            {
                GridSettingsEnableDisable(toggleWidthSettings.ToggleState);
            }
            catch
            { }
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
                clsJPDGSinfo.iWidth = Convert.ToInt32(dgvColWidth.Rows[i].Cells[2].Value.ToString());
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

        //Description : Format the Amount using Supplied Values
        public string FormatAmt(double myValue, string myFormat)
        {
            //https://msdn.microsoft.com/en-us/library/microsoft.visualbasic.strings.format(v=vs.110).aspx
            //DateTime.Now.ToString("dd/MMM/yyyy HH:mm:ss")
            //"29-Jan-2018 10:16:16"
            //FormatAmt = String.Format("{0:N3}", Val(myValue))
            //FormatAmt = Format(Val(myValue), "f" & DCSApp.Gdecimal.ToString & "")

            if (myFormat == "")
                myFormat = "#.00";
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
                myFormat = "#.00";

            if (sMyFormat != "")
                myFormat = sMyFormat;

            return Convert.ToDouble(myValue).ToString(myFormat);
        }

        //Description : Calculate the Entire Sales in each and every Corner
        private void CalcTotal()
        {
            double DblNetAmountTotal = 0;
            double QtyTotal = 0;
            double DblRate = 0;
            double dblQty = 0;

            // Not Available in the Method ------------------ >>
            double DblrateDiscper = 0;
            double DblRateAfterRDiscount = 0;
            double DblRateExclusive = 0;
            double dblGrossValue = 0;
            double dblGrossValueTot = 0;
            double dblQtyTot = 0;
            double dblFreeTot = 0;
            double dblGrossValueAfterRateDiscount = 0;
            double dblGrossValueAfterRateDiscountTot = 0;
            double dblGrossValueAfterDiscounts = 0;
            double dblGrossValueAfterDiscountsTot = 0;

            for (int i = 0; i < dgvItems.Rows.Count; i++)
            {
                SetValue(GetEnum(gridColIndexes.cSlNo), i, (i + 1).ToString());
                if (dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.CItemName)].Value != null)
                {
                    if (dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.CItemName)].Value.ToString() != "")
                    {
                        if (dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cItemID)].Value.ToString() != "")
                        {
                            if (Convert.ToDecimal(dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cItemID)].Value) != 0)
                            {
                                if (dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cQty)].Value == null)
                                    SetValue(GetEnum(gridColIndexes.cQty), i, "0");
                                if (dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cQOH)].Value == null)
                                    SetValue(GetEnum(gridColIndexes.cQOH), i, "0");

                                DblRate = Convert.ToDouble(dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cSrate)].Value);
                                //Dipu on 13-May-2022 ---------- >
                                dblQty = Convert.ToDouble(dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cQty)].Value);
                                //dblQty = Convert.ToDouble(dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cQty)].Value) + Convert.ToDouble(dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cQOH)].Value);
                                //Dipu on 25-May-2022 -- Free Value Commented
                                QtyTotal = QtyTotal + dblQty;// + Convert.ToDouble(dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cQOH)].Value);

                                //SetValue(GetEnum(gridColIndexes.cSlNo), i, (i + 1).ToString());

                                //DblrateDiscper = Convert.ToDouble(dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cRateDiscPer)].Value);
                                DblRateAfterRDiscount = DblRate - (DblRate * DblrateDiscper / 100);

                                if (dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cFloodCessPer)].Value == null)
                                    SetValue(GetEnum(gridColIndexes.cFloodCessPer), i, "");

                                //If chkApplyFloodCess.CheckState = CheckState.Checked Then
                                if (dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cFloodCessPer)].Value.ToString() == "")
                                    SetValue(GetEnum(gridColIndexes.cFloodCessPer), i, "0");
                                //End If

                                if (Convert.ToBoolean(dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cRateinclusive)].Value) == true)
                                    DblRateExclusive = GetRateExclusive(DblRateAfterRDiscount, (0), 0);
                                else
                                    DblRateExclusive = DblRateAfterRDiscount;

                                dblGrossValue = DblRateExclusive * dblQty;
                                SetValue(GetEnum(gridColIndexes.cGrossAmt), i, FormatValue(dblGrossValue));
                                dblGrossValueTot = dblGrossValueTot + dblGrossValue;
                                dblGrossValueAfterRateDiscount = dblQty * (DblRateExclusive);

                                dblQtyTot += Convert.ToDouble(dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cQty)].Value);
                                dblFreeTot += Convert.ToDouble(dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cQOH)].Value);

                                SetValue(GetEnum(gridColIndexes.cGrossValueAfterRateDiscount), i, FormatValue(dblGrossValueAfterRateDiscount));

                                dblGrossValueAfterRateDiscountTot = dblGrossValueAfterRateDiscountTot + dblGrossValueAfterRateDiscount;
                                //dbltaxableValueAfterItemDiscountTot = dblGrossValueAfterDiscountsTot;

                                if (Convert.ToDouble(dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cDiscPer)].Value) > 0)
                                {
                                    SetValue(GetEnum(gridColIndexes.cDiscAmount), i, FormatValue((dblGrossValueAfterRateDiscount * Convert.ToDouble(dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cDiscPer)].Value) / 100)));
                                }
                                else if (Convert.ToDouble(dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cDiscAmount)].Value) > 0)
                                {
                                    SetValue(GetEnum(gridColIndexes.cDiscAmount), i, FormatValue(Convert.ToDouble(dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cDiscAmount)].Value)));
                                }

                                dblGrossValueAfterDiscounts = dblGrossValueAfterRateDiscount - Convert.ToDouble(dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cDiscAmount)].Value);
                                dblGrossValueAfterDiscountsTot = dblGrossValueAfterDiscountsTot + dblGrossValueAfterDiscounts;
                                //
                                //Arrived Taxable Value
                                SetValue(GetEnum(gridColIndexes.cGrossValueAfterRateDiscount), i, FormatValue(0));

                                SetValue(GetEnum(gridColIndexes.ctaxable), i, FormatValue(0));
                                SetValue(GetEnum(gridColIndexes.cNonTaxable), i, "0");
                                SetValue(GetEnum(gridColIndexes.ctax), i, FormatValue(0));
                                SetValue(GetEnum(gridColIndexes.cCGST), i, "0");
                                SetValue(GetEnum(gridColIndexes.cSGST), i, "0");
                                SetValue(GetEnum(gridColIndexes.cIGST), i, "0");

                                SetValue(GetEnum(gridColIndexes.cNetAmount), i, FormatValue(0));
                                DblNetAmountTotal = DblNetAmountTotal + Convert.ToDouble(dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cNetAmount)].Value);
                            }
                        }
                    }
                }
            }


            //''''''' Bill Dicount Calculation''''''''''''''''''''
            //'First Discount 

            DblNetAmountTotal = 0;
            double TotalValueOfFree = 0;
            double Savings = 0;

            for (int j = 0; j < dgvItems.Rows.Count; j++)
            {
                if (dgvItems.Rows[j].Cells[GetEnum(gridColIndexes.CItemName)].Value != null)
                {
                    if (dgvItems.Rows[j].Cells[GetEnum(gridColIndexes.CItemName)].Value.ToString() != "")
                    {

                        SetValue(GetEnum(gridColIndexes.cBillDisc), j, "0");
                        dblGrossValueAfterDiscounts = Convert.ToDouble(dgvItems.Rows[j].Cells[GetEnum(gridColIndexes.cGrossValueAfterRateDiscount)].Value);
                        if (dblGrossValueAfterDiscountsTot > 0)
                            SetValue(GetEnum(gridColIndexes.cBillDisc), j, FormatValue(0));

                        SetValue(GetEnum(gridColIndexes.ctaxable), j, FormatValue(0));
                        SetValue(GetEnum(gridColIndexes.cNonTaxable), j, FormatValue(0));

                        SetTag(GetEnum(gridColIndexes.cCCessPer), j, FormatValue(0));
                        SetTag(GetEnum(gridColIndexes.cCCompCessQty), j, FormatValue(0));

                        SetValue(GetEnum(gridColIndexes.cFloodCessAmt), j, FormatValue(0));

                        SetValue(GetEnum(gridColIndexes.ctax), j, FormatValue(0));
                        SetValue(GetEnum(gridColIndexes.ctaxable), j, FormatValue(Convert.ToDouble(dgvItems.Rows[j].Cells[GetEnum(gridColIndexes.ctaxable)].Value)));
                        SetValue(GetEnum(gridColIndexes.cNonTaxable), j, "0");

                        SetValue(GetEnum(gridColIndexes.cCGST), j, "0");
                        SetValue(GetEnum(gridColIndexes.cSGST), j, "0");
                        SetValue(GetEnum(gridColIndexes.cIGST), j, "0");

                        SetTag(GetEnum(gridColIndexes.cCGST), j, "0"); ;
                        SetTag(GetEnum(gridColIndexes.cSGST), j, "0");
                        SetTag(GetEnum(gridColIndexes.cIGST), j, "0");

                        SetValue(GetEnum(gridColIndexes.cNetAmount), j, FormatValue((Convert.ToDouble(dgvItems.Rows[j].Cells[GetEnum(gridColIndexes.ctaxable)].Value) + Convert.ToDouble(dgvItems.Rows[j].Cells[GetEnum(gridColIndexes.cNonTaxable)].Value) + Convert.ToDouble(dgvItems.Rows[j].Cells[GetEnum(gridColIndexes.ctax)].Value))));

                        DblNetAmountTotal = DblNetAmountTotal + Convert.ToDouble(dgvItems.Rows[j].Cells[GetEnum(gridColIndexes.cNetAmount)].Value);

                        //valuation of Free
                        dblQty = Convert.ToDouble(dgvItems.Rows[j].Cells[GetEnum(gridColIndexes.cQty)].Value);
                        if (Convert.ToDouble(dgvItems.Rows[j].Cells[GetEnum(gridColIndexes.cQOH)].Value) > 0)
                        {
                            double PerItemRate = Convert.ToDouble(dgvItems.Rows[j].Cells[GetEnum(gridColIndexes.cNetAmount)].Value) - Convert.ToDouble(dgvItems.Rows[j].Cells[GetEnum(gridColIndexes.ctax)].Value) / dblQty;
                            TotalValueOfFree = TotalValueOfFree + (PerItemRate * Convert.ToDouble(dgvItems.Rows[j].Cells[GetEnum(gridColIndexes.cQOH)].Value));
                        }

                        //CALCULATION DECIMAL CHANGING
                        SetValue(GetEnum(gridColIndexes.cDiscAmount), j, FormatValue(Convert.ToDouble(dgvItems.Rows[j].Cells[GetEnum(gridColIndexes.cDiscAmount)].Value)));
                        SetValue(GetEnum(gridColIndexes.cBillDisc), j, FormatValue(Convert.ToDouble(dgvItems.Rows[j].Cells[GetEnum(gridColIndexes.cBillDisc)].Value)));

                        SetValue(GetEnum(gridColIndexes.ctaxable), j, FormatValue(Convert.ToDouble(dgvItems.Rows[j].Cells[GetEnum(gridColIndexes.ctaxable)].Value)));
                        SetTag(GetEnum(gridColIndexes.ctaxable), j, FormatValue(Convert.ToDouble(dgvItems.Rows[j].Cells[GetEnum(gridColIndexes.ctaxable)].Value)));
                        //DGVItem.Item(ctaxable, i).Tag = Format(Val(DGVItem.Item(ctaxable, i).Value), "#0.000000")
                        //Tag ??

                        SetValue(GetEnum(gridColIndexes.ctax), j, FormatValue(Convert.ToDouble(dgvItems.Rows[j].Cells[GetEnum(gridColIndexes.ctax)].Value)));
                        SetValue(GetEnum(gridColIndexes.cIGST), j, FormatValue(Convert.ToDouble(dgvItems.Rows[j].Cells[GetEnum(gridColIndexes.cIGST)].Value)));
                        SetValue(GetEnum(gridColIndexes.cSGST), j, FormatValue(Convert.ToDouble(dgvItems.Rows[j].Cells[GetEnum(gridColIndexes.cSGST)].Value)));
                        SetValue(GetEnum(gridColIndexes.cCGST), j, FormatValue(Convert.ToDouble(dgvItems.Rows[j].Cells[GetEnum(gridColIndexes.cCGST)].Value)));
                        SetValue(GetEnum(gridColIndexes.cNetAmount), j, FormatValue(Convert.ToDouble(dgvItems.Rows[j].Cells[GetEnum(gridColIndexes.cNetAmount)].Value)));
                        //SetValue(GetEnum(gridColIndexes.cNetAmount), j, FormatValue(Convert.ToDouble(dgvItems.Rows[j].Cells[GetEnum(gridColIndexes.cNetAmount)].Value)));
                        SetValue(GetEnum(gridColIndexes.cGrossValueAfterRateDiscount), j, FormatValue(Convert.ToDouble(dgvItems.Rows[j].Cells[GetEnum(gridColIndexes.cGrossValueAfterRateDiscount)].Value)));
                        SetValue(GetEnum(gridColIndexes.cNonTaxable), j, FormatValue(Convert.ToDouble(dgvItems.Rows[j].Cells[GetEnum(gridColIndexes.cNonTaxable)].Value)));

                    }
                }
            }

            txtGrossAmt.Text = FormatValue(dblGrossValueTot);
            lblQtyTotal.Text = FormatValue(dblQtyTot);

            double bALANCEFORrOUNDOFF = Convert.ToDouble(FormatAmt(DblNetAmountTotal, ""));

            lblBillAmount.Text = FormatValue(bALANCEFORrOUNDOFF);

            lblBillAmount.Text = FormatValue(bALANCEFORrOUNDOFF);
            lblBillAmount.Text = FormatValue(Convert.ToDouble(lblBillAmount.Text));
            double AdditionalCharges = 0;

            //'Tethering to itemwise rate
            double MyQty;

            for (int k = 0; k < dgvItems.Rows.Count; k++)
            {
                if (dgvItems.Rows[k].Cells[GetEnum(gridColIndexes.CItemName)].Value != null)
                {
                    if (dgvItems.Rows[k].Cells[GetEnum(gridColIndexes.CItemName)].Value.ToString() != "")
                    {
                        //if (Convert.ToDouble(dgvItems.Rows[k].Cells[GetEnum(gridColIndexes.CItemName)].Tag) > 0)
                        //{
                            if (dgvItems.Rows[k].Cells[GetEnum(gridColIndexes.cQty)].Value == null)
                                SetValue(GetEnum(gridColIndexes.cQty), k, AppSettings.QtyDecimalFormat);
                            if (dgvItems.Rows[k].Cells[GetEnum(gridColIndexes.cQOH)].Value == null)
                                SetValue(GetEnum(gridColIndexes.cQOH), k, AppSettings.QtyDecimalFormat);

                        MyQty = Convert.ToDouble(dgvItems.Rows[k].Cells[GetEnum(gridColIndexes.cQty)].Value);
                        //Dipu on 25-May-2022 -- Free Value Commented

                            //Distributing CommonValues Betweeen Items

                            SetValue(GetEnum(gridColIndexes.cSrate), k, FormatValue(Convert.ToDouble(dgvItems.Rows[k].Cells[GetEnum(gridColIndexes.cSrate)].Value))); 

                            //-----------------------------12-Aug-2022 arun

                            if (MyQty > 0) Savings = Savings + 0;
                    }
                }
            }

            if (Convert.ToDouble(lblBillAmount.Text) > 1000000000)
            {
                MessageBox.Show("Software is unable to calculate higher values than 100000000. Kindly limit the values or correct if given by mistake", clsVchType.TransactionName + " Value Calculation");
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

            if (Convert.ToInt32(dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cBatchMode)].Value) == 0) // None
                blnAutoCodeNeeded = false;
            else if (Convert.ToInt32(dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cBatchMode)].Value) == 1) // MNF
                blnAutoCodeNeeded = true;
            else if (Convert.ToInt32(dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cBatchMode)].Value) == 2) // Auto
                blnAutoCodeNeeded = true;
            else if (Convert.ToInt32(dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cBatchMode)].Value) == 3) // WMH
                blnAutoCodeNeeded = false;

            //string sQuery = "SELECT AnyWhere,BatchCode,ExpiryDate,MRP,QOH,StockID,ItemID FROM (SELECT (BatchCode+CONVERT(VARCHAR,ExpiryDate)+CONVERT(VARCHAR,ISNULL(QOH,0))+CONVERT(VARCHAR,ISNULL(MRP,0))) as AnyWhere,BatchCode,ExpiryDate,MRP,QOH,StockID,ItemID FROM tblStock )A WHERE A.ItemID = " + Convert.ToInt32(dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cItemID)].Value.ToString()) + "";
            sQuery = "SELECT AnyWhere,BatchCode,ExpiryDate,MRP,QOH,StockID,ItemID FROM (";
            sQuery = sQuery + "SELECT (BatchCode+CONVERT(VARCHAR,ExpiryDate)+CONVERT(VARCHAR,ISNULL(QOH,0))+CONVERT(VARCHAR,ISNULL(MRP,0))) as AnyWhere,BatchUnique as BatchCode,ExpiryDate,MRP,QOH,StockID,ItemID FROM tblStock ";

            if (blnAutoCodeNeeded == true)
            {
                if (Convert.ToInt32(dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cBatchMode)].Value) == 1)// MNF
                {
                    //if (bWhenPressDownKey == true)
                        //sQuery = sQuery + " UNION ALL SELECT '<Auto Barcode>' as AnyWhere,'<Auto Barcode>' as BatchCode,'23-Feb-2030' as ExpiryDate,0 as MRP,0 as QOH,0 as StockID," + Convert.ToInt32(dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cItemID)].Value.ToString()) + " as ItemID";
                }
                else if (Convert.ToInt32(dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cBatchMode)].Value) == 2)// Auto
                {
                    //sQuery = sQuery + " UNION ALL SELECT '<Auto Barcode>' as AnyWhere,'<Auto Barcode>' as BatchCode,'23-Feb-2030' as ExpiryDate,0 as MRP,0 as QOH,0 as StockID," + Convert.ToInt32(dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cItemID)].Value.ToString()) + " as ItemID";
                }
            }

            if (Convert.ToInt32(dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cBatchMode)].Value) == 1 || Convert.ToInt32(dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cBatchMode)].Value) == 2)// MNF & AUto
            {
                sQuery = sQuery + " )A WHERE A.ItemID = " + Convert.ToDecimal(dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cItemID)].Value.ToString()) + "";
                frmBatchSearch = new frmCompactSearch(GetFromBatchCodeSearch, sQuery, "Anywhere|BatchCode|CONVERT(VARCHAR,ExpiryDate)|CONVERT(VARCHAR,ISNULL(MRP,0))|CONVERT(VARCHAR,ISNULL(QOH,0))", dgvItems.Location.X + 350, dgvItems.Location.Y + 150, 4, 0, "", 4, 0, "ORDER BY A.StockID ASC", 0, 0, "BatchCode Search...", 0, "150,90,80,80,0,0", true, "Bar Code", 10);
                frmBatchSearch.Show();
                frmBatchSearch.BringToFront();
            }
        }

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
                    txtInvAutoNo.Text = Comm.gfnGetNextSerialNo("tblStockJournal", "AutoNum", " VchTypeID=" + vchtypeID).ToString();
                    txtReferenceAutoNo.Tag = txtInvAutoNo.Text;
                    txtInvAutoNo.Tag = 0;
                }  
                txtInvAutoNo.ReadOnly = true;
                txtPrefix.ReadOnly = true;
            }
            else if (ConvertI32(clsVchType.TransactionNumberingValue) == 1) // Auto Editable
            {
                if (iIDFromEditWindow == 0) //New
                {
                    //txtInvAutoNo.Text = Comm.gfnGetNextSerialNo("tblStockJournal", "AutoNum").ToString();
                    txtInvAutoNo.Text = Comm.gfnGetNextSerialNo("tblStockJournal", "AutoNum", " VchTypeID=" + vchtypeID).ToString();
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
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            try
            { 
            if (clsVchType.TransactionPrefix != "") // Transactoin Prefix
            {
                txtPrefix.Text = clsVchType.TransactionPrefix.Trim();
                txtPrefix.Visible = true;
            }
            else
                txtPrefix.Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            try
            {
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            try
            { 
            if (clsVchTypeFeatures.blnshowbillnarration == true)
                tblpNarration.Visible = true;
            else
                tblpNarration.Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            try
            { 
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
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            try
            { 
            if (clsVchType.ReferencePrefix != "") // ReferencePrefix
            {
                txtReferencePrefix.Text = clsVchType.ReferencePrefix.Trim();
                txtReferencePrefix.Visible = true;
                txtReferencePrefix.Width = 55;
            }
            else
                txtReferencePrefix.Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            try
            { 
            if (ConvertI32(clsVchType.ReferenceNumberingValue) == 0) // Auto Locked
            {
                if(iIDFromEditWindow == 0)
                    txtReferenceAutoNo.Text = Comm.gfnGetNextSerialNo("tblStockJournal", "ReferenceAutoNO").ToString();
                txtReferenceAutoNo.ReadOnly = true;
                txtReferencePrefix.ReadOnly = true;
                txtReferencePrefix.Width = 55;
            }
            else if (ConvertI32(clsVchType.ReferenceNumberingValue) == 1) // Auto Editable
            {
                if (iIDFromEditWindow == 0)
                    txtReferenceAutoNo.Text = Comm.gfnGetNextSerialNo("tblStockJournal", "ReferenceAutoNO").ToString();
                txtReferenceAutoNo.ReadOnly = false;
                txtReferencePrefix.ReadOnly = false;
                txtReferencePrefix.Width = 55;
            }
            else if (ConvertI32(clsVchType.ReferenceNumberingValue) == 2) // Custom
            {
                txtReferenceAutoNo.ReadOnly = true;
                txtReferencePrefix.ReadOnly = false;
                txtReferencePrefix.Visible = true;
                txtReferencePrefix.Width = txtReferenceAutoNo.Width;
            }
                ////--------------------------------------------------------------////
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            try
            { 
            if (clsVchType.blnPrimaryLockWithSelection == 1)
                cboCostCentre.Enabled = false;
            else
                cboCostCentre.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            try
            { 
            if (clsVchType.blnSecondaryLockWithSelection == 1)
                cboDestCostCentre.Enabled = false;
            else
                cboDestCostCentre.Enabled = true;
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

            try
            { 
            if (clsVchType.DefaultTaxModeValue == 3) //GST
            {
                if (dgvItems.Columns.Count > 0)
                {
                    dgvItems.Columns["cCGST"].Visible = true;
                    dgvItems.Columns["cSGST"].Visible = true;
                    dgvItems.Columns["cIGST"].Visible = true;
                    dgvItems.Columns["ctaxPer"].Visible = true;
                    dgvItems.Columns["ctax"].Visible = true;
                    dgvItems.Columns["ctaxable"].Visible = true;
                    dgvItems.Columns["cCRateWithTax"].Visible = true;
                }
            }
            else
            {
                if (dgvItems.Columns.Count > 0)
                {
                    dgvItems.Columns["cCGST"].Visible = false;
                    dgvItems.Columns["cSGST"].Visible = false;
                    dgvItems.Columns["cIGST"].Visible = false;
                    dgvItems.Columns["ctaxPer"].Visible = false;
                    dgvItems.Columns["ctax"].Visible = false;
                    dgvItems.Columns["ctaxable"].Visible = false;
                    dgvItems.Columns["cCRateWithTax"].Visible = false;
                }
            }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            try
            { 
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
            catch 
            {

            }

            try
            { 
                cboCostCentre.SelectedValue = ConvertI32(clsVchType.PrimaryCCValue);
                cboDestCostCentre.SelectedValue = ConvertI32(clsVchType.SecondaryCCValue);
                cboSalesStaff.SelectedValue = ConvertI32(clsVchType.DefaultSaleStaffValue);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        //Description : Setting asper Application Settings
        private void SetApplicationSettings()
        {
            try
            { 
            if (dgvItems.Columns.Count > 0)
            {
                if (AppSettings.TaxEnabled == true)
                {
                    if (AppSettings.TaxMode == 0) //No Tax
                    {
                        dgvItems.Columns["cCGST"].Visible = false;
                        dgvItems.Columns["cSGST"].Visible = false;
                        dgvItems.Columns["cIGST"].Visible = false;
                        dgvItems.Columns["ctaxPer"].Visible = false;
                        dgvItems.Columns["ctax"].Visible = false;
                        dgvItems.Columns["ctaxable"].Visible = false;
                        dgvItems.Columns["cCRateWithTax"].Visible = false;
                    }
                    else if (AppSettings.TaxMode == 1) //VAT
                    {
                        dgvItems.Columns["cCGST"].Visible = false;
                        dgvItems.Columns["cSGST"].Visible = false;

                        dgvItems.Columns["cIGST"].Visible = true;
                        dgvItems.Columns["ctaxPer"].Visible = true;
                        dgvItems.Columns["ctax"].Visible = true;
                        dgvItems.Columns["ctaxable"].Visible = true;
                        dgvItems.Columns["cCRateWithTax"].Visible = true;
                    }
                    else
                    {
                        dgvItems.Columns["cCGST"].Visible = true;
                        dgvItems.Columns["cSGST"].Visible = true;
                        dgvItems.Columns["cIGST"].Visible = true;
                        dgvItems.Columns["ctaxPer"].Visible = true;
                        dgvItems.Columns["ctax"].Visible = true;
                        dgvItems.Columns["ctaxable"].Visible = true;
                        dgvItems.Columns["cCRateWithTax"].Visible = true;
                    }
                }
                else
                {
                    dgvItems.Columns[GetEnum(gridColIndexes.cCGST)].Visible = false;
                    dgvItems.Columns[GetEnum(gridColIndexes.cSGST)].Visible = false;
                    dgvItems.Columns[GetEnum(gridColIndexes.cIGST)].Visible = false;
                    dgvItems.Columns[GetEnum(gridColIndexes.ctaxPer)].Visible = false;
                    dgvItems.Columns[GetEnum(gridColIndexes.ctax)].Visible = false;
                    dgvItems.Columns[GetEnum(gridColIndexes.ctaxable)].Visible = false;
                    dgvItems.Columns[GetEnum(gridColIndexes.cCRateWithTax)].Visible = false;
                }
            }

            if (dgvItems.Columns.Count > 0)
            {
                if (AppSettings.CessMode == 0)
                {
                    dgvItems.Columns[GetEnum(gridColIndexes.cCCessPer)].Visible = false;
                    dgvItems.Columns[GetEnum(gridColIndexes.cCCompCessQty)].Visible = false;
                    dgvItems.Columns[GetEnum(gridColIndexes.cFloodCessAmt)].Visible = false;
                    dgvItems.Columns[GetEnum(gridColIndexes.cFloodCessPer)].Visible = false;
                }
                else
                {
                    dgvItems.Columns[GetEnum(gridColIndexes.cCCessPer)].Visible = true;
                    dgvItems.Columns[GetEnum(gridColIndexes.cCCompCessQty)].Visible = true;
                    dgvItems.Columns[GetEnum(gridColIndexes.cFloodCessAmt)].Visible = true;
                    dgvItems.Columns[GetEnum(gridColIndexes.cFloodCessPer)].Visible = true;
                }
            }

                if (AppSettings.NeedCostCenter == true)
                {
                    pnlCostCentre.Visible = true;
                    if (clsVchType.ParentID == 41)
                        panel1.Visible = false;
                }
                else
                {
                    pnlCostCentre.Visible = false;
                    if (clsVchType.ParentID == 41)
                        panel1.Visible = false;
                }
                dtpInvDate.MinDate = AppSettings.FinYearStart;
            dtpInvDate.MaxDate = AppSettings.FinYearEnd;
                //dtpInvDate.MaxDate = Convert.ToDateTime(AppSettings.FinYearEnd).AddDays(1);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        //Description : Convert to Int32 of Decimal Value
        private int ConvertI32(decimal dVal)
        {
            return Convert.ToInt32(dVal);
        }

        private void LoadDataFromJSon(string strJson = "")
        {
            DeserializeFromJSon(strJson);
        }

        //Description : Load Saved data from database from edit window or Navigation buttons
        private void LoadData(int iSelectedID = 0)
        {
            try
            { 
            DataTable dtLoad = new DataTable();

                GetStockJournalIfo.InvId = Convert.ToDecimal(iSelectedID);
                GetStockJournalIfo.TenantID = Convert.ToDecimal(Global.gblTenantID);
                GetStockJournalIfo.VchTypeID = vchtypeID;
            dtLoad = clsPur.GetStockJournalMaster(GetStockJournalIfo, false);
            if (dtLoad.Rows.Count > 0)
            {
                DeserializeFromJSon(dtLoad.Rows[0]["JsonData"].ToString());
                if (Convert.ToInt32(dtLoad.Rows[0]["Cancelled"].ToString()) == 1)
                {
                    btnArchive.Enabled = false;
                    txtPrefix.Tag = 3; // Archive
                }
                else
                {
                    btnArchive.Enabled = true;
                    txtPrefix.Tag = 0;
                }

                    if (iIDFromEditWindow == 0)
                        btnArchive.Enabled = false;

                dgvItems.Columns["cRateinclusive"].Visible = false;
            }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
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
    public class OpeningGridColIndexes
    {
        public int cSlNo = 0;
        public int CItemCode = 1;
        public int CItemName = 2;
        public int CUnit = 3;
        public int cBarCode = 4;

        public int CExpiry = 5;
        public int cMRP = 6;
        public int cSrate = 7;
        public int cRateinclusive = 8;
        public int cQty = 9;
        public int cQOH = 10;
        public int cSRate1Per = 11;
        public int cSRate1 = 12;
        public int cSRate2Per = 13;
        public int cSRate2 = 14;
        public int cSRate3Per = 15;
        public int cSRate3 = 16;
        public int cSRate4Per = 17;
        public int cSRate4 = 18;
        public int cSRate5Per = 19;
        public int cSRate5 = 20;
        public int cGrossAmt = 21;
        public int cDiscPer = 22;
        public int cDiscAmount = 23;
        public int cBillDisc = 24;
        public int cCrate = 25;
        public int cCRateWithTax = 26;
        public int ctaxable = 27;
        public int ctaxPer = 28;
        public int ctax = 29;
        public int cIGST = 30;
        public int cSGST = 31;
        public int cCGST = 32;
        public int cNetAmount = 33;
        public int cItemID = 34;
        public int cGrossValueAfterRateDiscount = 35;
        public int cNonTaxable = 36;
        public int cCCessPer = 37;
        public int cCCompCessQty = 38;
        public int cFloodCessPer = 39;
        public int cFloodCessAmt = 40;
        public int cStockMRP = 41;
        public int cAgentCommPer = 42;
        public int cCoolie = 43;
        public int cBlnOfferItem = 44;
        public int cStrOfferDetails = 45;
        public int cBatchMode = 46;
        public int cID = 47;
        public int cImgDel = 48;


        //This variabl;e holds the maximum cols index in this class
        public int MaxColIndex = 48;

        public string GetColumnName(int colIndex)
        {
            switch (colIndex)
            {
                case 0:
                    {
                        return nameof(cSlNo);
                    }
                case 1: case 2: case 3: case 4:
                    {
                        return GetMasterColName(colIndex);
                    }
                case 5:
                    {
                        return nameof(CExpiry);
                    }
                case 6:
                    {
                        return nameof(cMRP);
                    }
                case 7:
                    {
                        return nameof(cSrate);
                    }
                case 8:
                    {
                        return nameof(cRateinclusive);
                    }
                case 9:
                    {
                        return nameof(cQty);
                    }
                case 10:
                    {
                        return nameof(cQOH);
                    }
                case 11:
                    {
                        return nameof(cSRate1Per);
                    }
                case 12:
                    {
                        return nameof(cSRate1);
                    }
                case 13:
                    {
                        return nameof(cSRate2Per);
                    }
                case 14:
                    {
                        return nameof(cSRate2);
                    }
                case 15:
                    {
                        return nameof(cSRate3Per);
                    }
                case 16:
                    {
                        return nameof(cSRate3);
                    }
                case 17:
                    {
                        return nameof(cSRate4Per);
                    }
                case 18:
                    {
                        return nameof(cSRate4);
                    }
                case 19:
                    {
                        return nameof(cSRate5Per);
                    }
                case 20:
                    {
                        return nameof(cSRate5);
                    }
                case 21:
                    {
                        return nameof(cGrossAmt);
                    }
                case 22:
                    {
                        return nameof(cDiscPer);
                    }
                case 23:
                    {
                        return nameof(cDiscAmount);
                    }
                case 24:
                    {
                        return nameof(cBillDisc);
                    }
                case 25:
                    {
                        return nameof(cCrate);
                    }
                case 26:
                    {
                        return nameof(cCRateWithTax);
                    }
                case 27:
                    {
                        return nameof(ctaxable);
                    }
                case 28:
                    {
                        return nameof(ctaxPer);
                    }
                case 29:
                    {
                        return nameof(ctax);
                    }
                case 30:
                    {
                        return nameof(cIGST);
                    }
                case 31:
                    {
                        return nameof(cSGST);
                    }
                case 32:
                    {
                        return nameof(cCGST);
                    }
                case 33:
                    {
                        return nameof(cNetAmount);
                    }
                case 34:
                    {
                        return nameof(cItemID);
                    }
                case 35:
                    {
                        return nameof(cGrossValueAfterRateDiscount);
                    }
                case 36:
                    {
                        return nameof(cNonTaxable);
                    }
                case 37:
                    {
                        return nameof(cCCessPer);
                    }
                case 38:
                    {
                        return nameof(cCCompCessQty);
                    }
                case 39:
                    {
                        return nameof(cFloodCessPer);
                    }
                case 40:
                    {
                        return nameof(cFloodCessAmt);
                    }
                case 41:
                    {
                        return nameof(cStockMRP);
                    }
                case 42:
                    {
                        return nameof(cAgentCommPer);
                    }
                case 43:
                    {
                        return nameof(cCoolie);
                    }
                case 44:
                    {
                        return nameof(cBlnOfferItem);
                    }
                case 45:
                    {
                        return nameof(cStrOfferDetails);
                    }
                case 46:
                    {
                        return nameof(cBatchMode);
                    }
                case 47:
                    {
                        return nameof(cID);
                    }
                case 48:
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

        public enum BarcodeMode
        {
            BarcodeDropdown,
            BarcodeScanning,
            BarcodeKeyboard
        }



        public string GetMasterColName(int colIndex)
        {
            if (colIndex > 4 || colIndex < 1)
            {
                MessageBox.Show("Invalid column index | " + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return "";
            }
            switch (MyMode)
            {
                case BarcodeMode.BarcodeDropdown:
                    {
                        if(colIndex == 1) return nameof(CItemCode);
                        if(colIndex == 2) return nameof(CItemName);
                        if(colIndex == 3) return nameof(CUnit);
                        if(colIndex == 4) return nameof(cBarCode);

                        break;
                    }
                case BarcodeMode.BarcodeKeyboard:
                    {
                        if (colIndex == 1) return nameof(cBarCode);
                        if (colIndex == 2) return nameof(CItemCode);
                        if (colIndex == 3) return nameof(CItemName);
                        if (colIndex == 4) return nameof(CUnit);

                        break;
                    }
                case BarcodeMode.BarcodeScanning:
                    {
                        if (colIndex == 1) return nameof(cBarCode);
                        if (colIndex == 2) return nameof(CItemCode);
                        if (colIndex == 3) return nameof(CItemName);
                        if (colIndex == 4) return nameof(CUnit);

                        break;
                    }
                default:
                    {
                        if (colIndex == 1) return nameof(CItemCode);
                        if (colIndex == 2) return nameof(CItemName);
                        if (colIndex == 3) return nameof(CUnit);
                        if (colIndex == 4) return nameof(cBarCode);

                        break;
                    }
            }

            MessageBox.Show("Invalid column index | " + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            return "";
        }

        private BarcodeMode MyMode;

        public void ChangeBarcodeMode(decimal mode)
        {
            switch (mode)
            {
                case 0:
                    {
                        CItemCode = 1;
                        CItemName = 2;
                        CUnit = 3;
                        cBarCode = 4;

                        MyMode = BarcodeMode.BarcodeDropdown;

                        break;
                    }
                case 1:
                    {
                        cBarCode = 1;
                        CItemCode = 2;
                        CItemName = 3;
                        CUnit = 4;

                        MyMode = BarcodeMode.BarcodeKeyboard;

                        break;
                    }
                case 2:
                    {
                        cBarCode = 1;
                        CItemCode = 2;
                        CItemName = 3;
                        CUnit = 4;

                        MyMode = BarcodeMode.BarcodeScanning;

                        break;
                    }
                default:
                    {
                        CItemCode = 1;
                        CItemName = 2;
                        CUnit = 3;
                        cBarCode = 4;

                        MyMode = BarcodeMode.BarcodeDropdown;

                        break;
                    }
            }
        }
    }
    #endregion 
}
