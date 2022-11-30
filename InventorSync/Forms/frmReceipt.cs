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

    public partial class frmReceipt : Form, IMessageFilter
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

        private frmCompactSearch frmSupplierSearch;
        private frmCompactSearch frmItemSearch;
        private frmCompactSearch frmBatchSearch;

        sqlControl bsdata = new sqlControl();

        public frmReceipt(int iVchTpeId = 0, int iTransID = 0, bool bFromEdit = false, object MDIParent = null)
        {
            InitializeComponent();
            Application.AddMessageFilter(this);

            try
            {
                Comm.TransparentControls(this);
                Comm.SetControlColours(this);

                //this.BackColor = Color.FromArgb(249, 246, 238);
                tlpMain.BackColor = Color.FromArgb(249, 246, 238);
                this.BackColor = Color.Black;
                this.Padding = new Padding(1);

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
            //int l = form.ClientSize.Width - 10; //(this.MdiParent.ClientSize.Width - this.Width) / 2;
            //int t = form.ClientSize.Height - 80; //((this.MdiParent.ClientSize.Height - this.Height) / 2) - 30;
            //this.SetBounds(5, 0, l, t);

            clsVchType = JSonComm.GetVoucherType(iVchTpeId);
            clsVchTypeFeatures = JSonComm.GetVoucherTypeGeneralSettings(iVchTpeId, 1);

            ClearControls();

            //bFromEditSales = bFromEdit;
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

            lblPause.Text = "Pause";

            if (clsVchType.ParentID == 7)
            {
                cboDrCr.SelectedIndex = 0;
                cboDrCr.Enabled = false;
            }
            else if (clsVchType.ParentID == 8)
            {
                cboDrCr.SelectedIndex = 1;
                cboDrCr.Enabled = false;
            }
            else if (clsVchType.ParentID == 9)
            {
                cboDrCr.SelectedIndex = 0;
                cboDrCr.Enabled = false;
            }

            if (clsVchType.ParentID == 10)
            {
                pnlDrCr.Visible = false;
                pnlLedger.Visible = false;
                cboDrCr.SelectedIndex = 0;
            }
            else
            {
                pnlDrCr.Visible = true;
                pnlLedger.Visible = true;
            }
            SetColumnsAsPerVchtype();
        }

        #region "VARIABLES --------------------------------------------- >>"

        Control ctrl;
        string sEditedValueonKeyPress = "";
        int iIDFromEditWindow, vchtypeID;
        //decimal dSupplierID = 0, dUnitID = 0;
        bool dragging = false;
        int xOffset = 0, yOffset = 0, d=0;
        //string strCheck = "", sgblBarcodeNoExists;
        string strSelectedLedgerName = "";
        //int iprevVchNo, iNextVchNo;
        //bool bFromEditSales;
        //decimal dCostRateInc = 0, dCostRateExcl = 0, dPRateIncl = 0, dPRateExcl = 0;
        //decimal dSteadyBillDiscPerc, dSteadyBillDiscAmt;

        static int namesCount = Enum.GetNames(typeof(LedgerIndexes)).Length;
        //string[] sArrLedger = new string[namesCount];
        Common Comm = new Common();
        
        
        UspGetEmployeeInfo GetEmpInfo = new UspGetEmployeeInfo();
        UspGetCostCentreInfo GetCctinfo = new UspGetCostCentreInfo();
        UspGetLedgerInfo GetLedinfo = new UspGetLedgerInfo();
        UspGetAccountsInfo GetAccountsIfo = new UspGetAccountsInfo();

        clsEmployee clsEmp = new clsEmployee();
        clsCostCentre clscct = new clsCostCentre();
        clsLedger clsLedg = new clsLedger();

        clsJSonCommon JSonComm = new clsJSonCommon();
        clsJSonAccounts clsJAcc = new clsJSonAccounts();

        clsAccounts clsAcc = new clsAccounts();

        //Accounts Master Related Classes for Json
        clsJsonACCInfo clsJPMinfo = new clsJsonACCInfo();
        clsJsonACCDetailsInfo clsJPDinfo = new clsJsonACCDetailsInfo();
        clsJsonPMLedgerInfo clsJPMLedgerinfo = new clsJsonPMLedgerInfo();
        clsJsonPMCCentreInfo clsJPMCostCentreinfo = new clsJsonPMCCentreInfo();
        clsJsonPMEmployeeInfo clsJPMEmployeeinfo = new clsJsonPMEmployeeInfo();

        //Accounts Detail Related Classes For Json
        clsJsonACCDetailsInfo clsJACDinfo = new clsJsonACCDetailsInfo();
        //clsJsonACIteminfo clsJACIteminfo = new clsJsonACIteminfo();

        //DataTable dtItemPublic = new DataTable();
        //DataTable dtUnitPublic = new DataTable();
        //DataTable dtBatchCode = new DataTable();
        //DataTable dtBatchCodeData = new DataTable();

        //Rectangle Rectangle;

        clsJsonVoucherType clsVchType = new clsJsonVoucherType();
        clsGetStockInVoucherSettings clsVchTypeFeatures = new clsGetStockInVoucherSettings();

        private ReceiptGridColIndexes gridColIndexes = new ReceiptGridColIndexes();

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
                    FillCostCentre();
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
                    dgvItems.CurrentCell = dgvItems.Rows[iRowCnt - 1].Cells[GetEnum(gridColIndexes.CLedgerCode)];
                    dgvItems.Focus();
                    SendKeys.Send("{down}");
                }
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

                if (dgvItems.CurrentCell.ColumnIndex == (int)gridColIndexes.CLedgerCode)
                {
                    if (sEditedValueonKeyPress != null)
                    {
                        string strCondition = "";
                        if (clsVchType.ParentID == 7)
                            strCondition = " and  accountgroupid not in (16,17) ";
                        else if (clsVchType.ParentID == 8)
                            strCondition = " and  accountgroupid not in (16,17) ";
                        else if (clsVchType.ParentID == 9)
                            strCondition = " and  accountgroupid in (16,17) ";

                        sQuery = "SELECT LedgerCode + LedgerName + MobileNo + Email + TaxNo AS anywhere, LedgerCode, LedgerName, MobileNo, CurBal, Email, TaxNo, LID " +
                                " FROM     vwLedger   Where isnull(ActiveStatus, 1) = 1 " + strCondition;
                        frmDetailedSearch2 frmN = new frmDetailedSearch2(GetFromSupplierSearch, sQuery, "Anywhere|LedgerCode|LedgerName", dgvItems.Location.X + 55, dgvItems.Location.Y + 150, 7, 0, sEditedValueonKeyPress, 7, 0, "ORDER BY LedgerCode ASC", 0, 0, "Ledger Search...", 0, "250,250,150,150,150,150,0", true, "frmLedger", 20);
                        frmN.MdiParent = this.MdiParent;
                        frmN.Show();

                        if (dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cLedgerID)].Value != null)
                        {
                            this.dgvItems.EditingControlShowing -= this.dgvItems_EditingControlShowing;
                            dgvItems.CurrentCell = dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.CCurBal)];
                            dgvItems.Focus();
                            this.dgvItems.EditingControlShowing += this.dgvItems_EditingControlShowing;
                        }
                    }
                }
                else if (dgvItems.CurrentCell.ColumnIndex == (int)gridColIndexes.CCurBal)
                {
                    dgvItems.CurrentCell = dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cAmountDr)];
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
                dgvItems.CurrentCell = dgvItems.Rows[0].Cells[GetEnum(gridColIndexes.CLedgerCode)];
                dgvItems.Focus();
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
                //ClearControls();
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
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
                    if (dgvItems.Rows[0].Cells[GetEnum(gridColIndexes.CLedgerCode)].Value != null)
                    {
                        DialogResult dlgResult = MessageBox.Show("An Unsaved Voucher is Pending. Invoice Navigation will clear the unsaved Voucher. Do you want to proceed any way ?", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                        if (dlgResult == DialogResult.Yes)
                        {
                            SetColumnsAsPerVchtype();
                            PreVNext(true);
                        }
                    }
                    else
                    {
                        SetColumnsAsPerVchtype();
                        PreVNext(true);
                    }
                }
                else
                {
                    SetColumnsAsPerVchtype();
                    PreVNext(true);
                }
            }
            else
            {
                SetColumnsAsPerVchtype();
                PreVNext(true);
            }
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
                        if (cboCostCentre.Visible == true)
                        {
                            cboCostCentre.Focus();
                            SendKeys.Send("{F4}");
                        }
                        else
                        {
                            if (cboSalesStaff.Visible == true)
                            {
                                cboSalesStaff.Focus();
                                SendKeys.Send("{F4}");
                            }
                            else
                            {
                                if (txtLedger.Visible == true)
                                {
                                    txtLedger.Focus();
                                }
                            }
                        }
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
                    if (dgvItems.Rows[0].Cells[GetEnum(gridColIndexes.CLedgerCode)].Value != null)
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
                        DialogResult dlgResult = MessageBox.Show("Are you sure to delete Item Code [" + strSelectedLedgerName + "] Permanently ?", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
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
                            if (dgvItems.Rows[0].Cells[GetEnum(gridColIndexes.CLedgerCode)].Value != null)
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
                else if (dtpEffective.Enabled == true)
                    dtpEffective.Focus();
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
                            dgvItems.CurrentCell = dgvItems.Rows[0].Cells[GetEnum(gridColIndexes.CLedgerCode)];
                            dgvItems.Focus();
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
                        if(txtReferencePrefix.Visible == true)
                            txtReferencePrefix.Focus();
                        else
                            txtReferenceAutoNo.Focus();
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
                                dgvItems.CurrentCell = dgvItems.Rows[0].Cells[GetEnum(gridColIndexes.CLedgerCode)];
                                dgvItems.Focus();
                            }
                        }
                    }
                    txtReferenceAutoNo.SelectAll();
                    //SendKeys.Send("{F4}");
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
                            dgvItems.CurrentCell = dgvItems.Rows[0].Cells[GetEnum(gridColIndexes.CLedgerCode)];
                            dgvItems.Focus();
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
                DataTable dtInv = Comm.fnGetData("SELECT ISNULL(JsonData,'') as JsonData,Invid FROM tblAccVoucher WHERE InvNo = '" + txtInvAutoNo.Text + "' AND VchTypeID=" + vchtypeID + " AND TenantID = " + Global.gblTenantID + "").Tables[0];
                if (dtInv.Rows.Count > 0)
                {
                    DialogResult dlgResult = MessageBox.Show("There is an Exisiting Bill Number in this Invoice No [" + txtInvAutoNo.Text + "], Do you want to load it?", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                    if (dlgResult == DialogResult.Yes)
                    {
                        LoadData(Convert.ToInt32(dtInv.Rows[0]["InvId"].ToString()));
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
                    }
                }
            }
            //SetColumnsAsPerVchtype();
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
                        dtInv = Comm.fnGetData("SELECT TOP 1 ISNULL(InvId,0) FROM tblAccVoucher WHERE VchTypeID = " + vchtypeID + " AND TenantID = " + Global.gblTenantID + " ORDER BY InvId DESC").Tables[0];
                        if (dtInv.Rows.Count > 0)
                            dInvId = Convert.ToDecimal(dtInv.Rows[0][0].ToString());
                        else
                            dInvId = 0;
                    }
                    else
                    {
                        dtInv = Comm.fnGetData("SELECT TOP 1 ISNULL(InvId,0) FROM tblAccVoucher WHERE InvId < " + Convert.ToDecimal(txtInvAutoNo.Tag.ToString()) + " AND VchTypeID = " + vchtypeID + " AND TenantID = " + Global.gblTenantID + " ORDER BY InvId DESC").Tables[0];
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
                        dtInv = Comm.fnGetData("SELECT TOP 1 ISNULL(InvId,0) FROM tblAccVoucher WHERE InvId > " + Convert.ToDecimal(txtInvAutoNo.Tag.ToString()) + " AND VchTypeID = " + vchtypeID + " AND TenantID = " + Global.gblTenantID + " ORDER BY InvId ASC").Tables[0];
                        if (dtInv.Rows.Count > 0)
                            dInvId = Convert.ToDecimal(dtInv.Rows[0][0].ToString());
                        else
                        {
                            dInvId = 0;
                            ClearControls();
                            SetColumnsAsPerVchtype();
                            if (ConvertI32(clsVchType.TransactionNumberingValue) == 0) // Auto Locked
                            {
                                txtInvAutoNo.Text = Comm.gfnGetNextSerialNo("tblAccVoucher", "AutoNum", " VchTypeID=" + vchtypeID).ToString();
                                txtInvAutoNo.Tag = 0;

                                txtInvAutoNo.ReadOnly = true;
                                txtPrefix.ReadOnly = true;
                            }
                            else if (ConvertI32(clsVchType.TransactionNumberingValue) == 1) // Auto Editable
                            {
                                txtInvAutoNo.Text = Comm.gfnGetNextSerialNo("tblAccVoucher", "AutoNum", " VchTypeID=" + vchtypeID).ToString();
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
                                    txtReferenceAutoNo.Text = Comm.gfnGetNextSerialNo("tblAccVoucher", "ReferenceAutoNO").ToString();
                                txtReferenceAutoNo.ReadOnly = true;
                                txtReferencePrefix.ReadOnly = true;
                                txtReferencePrefix.Width = 55;
                            }
                            else if (ConvertI32(clsVchType.ReferenceNumberingValue) == 1) // Auto Editable
                            {
                                if (iIDFromEditWindow == 0)
                                    txtReferenceAutoNo.Text = Comm.gfnGetNextSerialNo("tblAccVoucher", "ReferenceAutoNO").ToString();
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

            for (int i = 0; i < dgvItems.Rows.Count - 1; i++)
            {
                if (clsVchType.ParentID != 10)
                {
                    if (cboDrCr.SelectedIndex == 0)
                        dgvItems.Rows[0].Cells[GetEnum(gridColIndexes.cAmountDr)].Value = "0";
                    else if (cboDrCr.SelectedIndex == 1)
                        dgvItems.Rows[0].Cells[GetEnum(gridColIndexes.cAmountCr)].Value = "0";
                }
            }

            CalcTotal();

            if (txtInvAutoNo.Text == "")
            {
                bValidate = false;
                MessageBox.Show("Please Enter the Invoice No.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                txtInvAutoNo.Focus();
                goto FailsHere;
            }
            else if (Convert.ToString(dgvItems.Rows[0].Cells[GetEnum(gridColIndexes.CLedgerCode)].Value) == "")
            {
                bValidate = false;
                MessageBox.Show("No Ledgers are selected for Save. Please select a ledger", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                goto FailsHere;
            }
            else if (cboSalesStaff.SelectedIndex < 0)
            {
                bValidate = false;
                MessageBox.Show("Please select an employee.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                goto FailsHere;
            }
            else if (cboDrCr.SelectedIndex < 0)
            {
                bValidate = false;
                MessageBox.Show("Please select Dr/Cr.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                goto FailsHere;
            }
            else if (Convert.ToDouble(lblBillAmount.Text) != 0 && clsVchType.ParentID == 10)
            {
                bValidate = false;
                MessageBox.Show("Balance should be zero. Correct all the debit and credit figures to save the voucher.", clsVchType.TransactionName + " Value Calculation", MessageBoxButtons.OK, MessageBoxIcon.Error);
                goto FailsHere;
            }
            else if (Convert.ToDouble(lblBillAmount.Text) == 0 && clsVchType.ParentID != 10)
            {
                bValidate = false;
                MessageBox.Show("Bill amount is zero. Correct all the ledger detail amounts to save the voucher.", clsVchType.TransactionName + " Value Calculation", MessageBoxButtons.OK, MessageBoxIcon.Error);
                goto FailsHere;
            }
            else
            {

            }

            if (txtInvAutoNo.Text.Trim() != "")
            {
                if (iIDFromEditWindow == 0)
                {
                    dtInv = Comm.fnGetData("SELECT InvNo FROM tblAccVoucher WHERE vchtypeid=" + vchtypeID + " and LTRIM(RTRIM(InvNo)) = '" + txtInvAutoNo.Text.Trim() + "'").Tables[0];
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

            bool blnFoundLedgerToSave = false;

            for (int i = 0; i < dgvItems.Rows.Count; i++)
            {
                if (dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cAmountDr)].Value == null)
                    dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cAmountDr)].Value = "0";
                if (dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cAmountDr)].Value.ToString().Trim() == "")
                    dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cAmountDr)].Value = "0";
                if (dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cAmountCr)].Value == null)
                    dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cAmountCr)].Value = "0";
                if (dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cAmountCr)].Value.ToString().Trim() == "")
                    dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cAmountCr)].Value = "0";

                //if (iIDFromEditWindow == 0)
                {
                    if (dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cLedgerID)].Value != null && dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.CLedgerCode)].Value != null)
                    {
                        if (Convert.ToDouble(dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cAmountDr)].Value.ToString().Trim()) != 0 || Convert.ToDecimal(dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cAmountCr)].Value.ToString().Trim()) != 0)
                        {
                            string sQuery = "Select LID,LAliasName From tblLedger Where LTRIM(RTRIM(LAliasName)) = '" + dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.CLedgerCode)].Value.ToString() + "' AND LID <> " + Convert.ToDouble(dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cLedgerID)].Value.ToString()) + "";
                            DataTable dtLedger = Comm.fnGetData(sQuery).Tables[0];
                            if (dtLedger.Rows.Count > 0)
                            {
                                bValidate = false;
                                MessageBox.Show("Please select the ledger properly.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                dgvItems.Rows[i].Cells[1].Selected = true;

                                break;
                            }
                            else
                            {
                                if (blnFoundLedgerToSave == false) blnFoundLedgerToSave = true;
                            }
                        }
                        if (Convert.ToDouble(dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cAmountDr)].Value.ToString().Trim()) == 0 && Convert.ToDouble(dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cAmountCr)].Value.ToString().Trim()) == 0)
                        {
                            bValidate = false;
                            MessageBox.Show("Please provide amount in debit or credit column for all ledgers.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            dgvItems.Rows[i].Cells[4].Selected = true;

                            break;
                        }
                    }
                }
            }

            if (blnFoundLedgerToSave == false)
            {
                bValidate = false;
                MessageBox.Show("Please provide atleast one ledger in details.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            FailsHere:
            return bValidate;
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

        //Description : Serialize the Accounts table Fields asper instructions.
        private string SerializetoJson()
        {
            #region "Accounts Master (tblAccVoucher) ------------------------------- >>"

            if (iIDFromEditWindow == 0)
            {
                clsJPMinfo.InvId = Comm.gfnGetNextSerialNo("tblAccVoucher", "InvId");
                txtInvAutoNo.Tag = clsJPMinfo.InvId;
                clsJPMinfo.AutoNum = Convert.ToInt32(Comm.gfnGetNextSerialNo("tblAccVoucher", "AutoNum", " VchTypeID=" + vchtypeID).ToString());
            }
            else
            {
                clsJPMinfo.InvId = Convert.ToInt32(iIDFromEditWindow);
                txtInvAutoNo.Tag = Convert.ToDecimal(iIDFromEditWindow);
                if (txtReferenceAutoNo.Tag.ToString() == "") txtReferenceAutoNo.Tag = 0;
                clsJPMinfo.AutoNum = Convert.ToInt32(txtReferenceAutoNo.Tag.ToString());
            }

            clsJPMinfo.InvNo = txtInvAutoNo.Text;
            clsJPMinfo.Prefix = txtPrefix.Text.Trim();
            clsJPMinfo.InvDate = Convert.ToDateTime(dtpInvDate.Text);
            clsJPMinfo.VchType = clsVchType.TransactionName;
            clsJPMinfo.TaxModeID = "1";
            clsJPMinfo.DebitCredit = cboDrCr.SelectedIndex == 0? "DR" : "CR";
            clsJPMinfo.LedgerId = Convert.ToDecimal("100");
            clsJPMinfo.Party = "";
            clsJPMinfo.TaxAmt = Convert.ToDecimal(0);
            clsJPMinfo.QtyTotal = Convert.ToDecimal(txtTotalDr.Text);
            clsJPMinfo.BillAmt = Convert.ToDecimal(lblBillAmount.Text);

            clsJPMinfo.Cancelled = 0;
            clsJPMinfo.SalesManID = Convert.ToInt32(cboSalesStaff.SelectedValue);
            clsJPMinfo.Taxable = Convert.ToDecimal(0);
            clsJPMinfo.NonTaxable = Convert.ToDecimal(0);
            clsJPMinfo.UserNarration = txtNarration.Text;

            clsJPMinfo.SortNumber = 0;
            clsJPMinfo.VchTypeID = vchtypeID;
            clsJPMinfo.CCID = Convert.ToInt32(cboCostCentre.SelectedValue);
            clsJPMinfo.CurrencyID = 0;
            clsJPMinfo.PartyAddress = "";
            clsJPMinfo.UserID = Global.gblUserID;
            clsJPMinfo.CashDiscount = Convert.ToDecimal(0);
            clsJPMinfo.NetAmount = Convert.ToDecimal(0);
            clsJPMinfo.RefNo = txtReferencePrefix.Text;
            clsJPMinfo.blnWaitforAuthorisation = 0;
            clsJPMinfo.UserIDAuth = 0;
            clsJPMinfo.BillTime = DateTime.Now;
            clsJPMinfo.StateID = 32;
            
            clsJPMinfo.ImplementingStateCode = "";
            clsJPMinfo.GSTType = "";
            clsJPMinfo.CGSTTotal = 0;
            clsJPMinfo.SGSTTotal = 0;
            clsJPMinfo.IGSTTotal = 0;
            clsJPMinfo.PartyGSTIN = "";
            clsJPMinfo.BillType = "";
            clsJPMinfo.blnHold = 0;
            clsJPMinfo.ChequeNo = txtChequeno.Text;
            clsJPMinfo.BankName = txtBankName.Text;
            clsJPMinfo.Status = cmbStatus.SelectedText;
            clsJPMinfo.ChequeDate = dtpChequedate.Value;

            if (txtLedger.Tag == null)
                txtLedger.Tag = "0";
            
            clsJPMinfo.ACCLedgerID = Comm.ToInt32(txtLedger.Tag);
            clsJPMinfo.TransType = "Inputs";
            clsJPMinfo.SalesTaxtype = "B2B";
            clsJPMinfo.REconciled = 0;
            clsJPMinfo.LedgerId = Comm.ToInt32(txtLedger.Tag);
            clsJPMinfo.ImplementingStateCode = "32";
            
            clsJPMinfo.AdavancedMode = "";

            clsJPMinfo.EffectiveDate = dtpEffective.Value;
            clsJPMinfo.partyCode = "";
            clsJPMinfo.MobileNo = "";
            clsJPMinfo.Email = "";
            clsJPMinfo.TaxType = "";
            clsJPMinfo.QtyTotal = 0;
            clsJPMinfo.SystemName = Global.gblSystemName;
            clsJPMinfo.LastUpdateDate = DateTime.Today;
            clsJPMinfo.LastUpdateTime = DateTime.Now;
            clsJPMinfo.CounterID = 0;
            clsJPMinfo.ReferenceAutoNO = txtReferenceAutoNo.Text;
            clsJPMinfo.TenantID = Global.gblTenantID;
            clsJAcc.clsJsonPMInfo_ = clsJPMinfo;

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
            clsJAcc.clsJsonPMCCentreInfo_ = clsJPMCostCentreinfo;

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
            clsJAcc.clsJsonPMEmployeeInfo_ = clsJPMEmployeeinfo;

            #endregion

            #region "Accounts Details (tblAccVoucherItem) -------------------------- >>"
            List<clsJsonACCDetailsInfo> lstJPDinfo = new List<clsJsonACCDetailsInfo>();
            for (int i = 0; i < dgvItems.Rows.Count; i++)
            {
                if (dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.CLedgerCode)].Value != null)
                {
                    if (dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.CLedgerCode)].Value.ToString() != "")
                    {
                        clsJACDinfo = new clsJsonACCDetailsInfo();

                        clsJACDinfo.InvID = Convert.ToInt32(txtInvAutoNo.Tag);

                        clsJACDinfo.SlNo = Convert.ToInt32(dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cSlNo)].Value);
                        clsJACDinfo.LID = Convert.ToInt32(dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cLedgerID)].Value);
                        clsJACDinfo.Amount = Convert.ToDecimal(dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cAmountDr)].Value) + Convert.ToDecimal(dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cAmountCr)].Value);
                        clsJACDinfo.AmountD = Convert.ToDecimal(dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cAmountDr)].Value);
                        clsJACDinfo.AmountC = Convert.ToDecimal(dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cAmountCr)].Value);

                        lstJPDinfo.Add(clsJACDinfo);
                    }
                }
            }
            clsJAcc.clsJsonACCDetailsInfoList_ = lstJPDinfo;

            #endregion

            return JsonConvert.SerializeObject(clsJAcc);
        }

        // Cash : 0, Credit: 1, Both: 2, Cash Desk : 3
        //Description : Deserialize the JSon to Controls asper instructions.
        private void DeserializeFromJSon(string sToDeSerialize = "")
        {
            clsJSonAccounts clsTransaction = JsonConvert.DeserializeObject<clsJSonAccounts>(sToDeSerialize);

            txtPrefix.Text = clsVchType.TransactionPrefix;
            txtInvAutoNo.Text = Convert.ToString(clsTransaction.clsJsonPMInfo_.InvNo);
            txtInvAutoNo.Tag = Convert.ToDouble(clsTransaction.clsJsonPMInfo_.InvId);
            txtReferenceAutoNo.Tag = Convert.ToDouble(clsTransaction.clsJsonPMInfo_.AutoNum);
            dtpInvDate.Text = Convert.ToString(clsTransaction.clsJsonPMInfo_.InvDate);
            dtpEffective.Text = Convert.ToString(clsTransaction.clsJsonPMInfo_.EffectiveDate);
            txtReferencePrefix.Text = clsTransaction.clsJsonPMInfo_.RefNo;
            txtReferenceAutoNo.Text = Convert.ToString(clsTransaction.clsJsonPMInfo_.ReferenceAutoNO);
            lblLID.Text = Convert.ToString(clsTransaction.clsJsonPMInfo_.LedgerId);
            txtLedger.Tag = lblLID.Text;
            txtLedger.Text = Comm.GetTableValue("tblLedger", "LAliasName", " Where LID = " + lblLID.Text.ToString());

            if (clsTransaction.clsJsonPMInfo_.DebitCredit.ToUpper() == "DR")
                cboDrCr.SelectedIndex = 0; //Comm.GetTableValue("tblLedger", "LAliasName", " Where LID = " + lblLID.Text.ToString());
            else if (clsTransaction.clsJsonPMInfo_.DebitCredit.ToUpper() == "CR")
                cboDrCr.SelectedIndex = 1;

            txtTotalDr.Text = Comm.chkChangeValuetoZero(Convert.ToString(clsTransaction.clsJsonPMInfo_.QtyTotal));
            txtNarration.Text = Convert.ToString(clsTransaction.clsJsonPMInfo_.UserNarration);
            lblBillAmount.Text = Comm.chkChangeValuetoZero(Convert.ToString(clsTransaction.clsJsonPMInfo_.BillAmt));

            txtChequeno.Text = clsTransaction.clsJsonPMInfo_.ChequeNo;
            txtBankName.Text = clsTransaction.clsJsonPMInfo_.BankName;
            cmbStatus.SelectedText = clsTransaction.clsJsonPMInfo_.Status;
            dtpChequedate.Value = clsTransaction.clsJsonPMInfo_.ChequeDate;

            cboCostCentre.SelectedValue = clsTransaction.clsJsonPMCCentreInfo_.CCID;
            cboSalesStaff.SelectedValue = clsTransaction.clsJsonPMEmployeeInfo_.EmpID;

            DataTable dtGetPurDetail = clsTransaction.clsJsonACCDetailsInfoList_.ToDataTable();
            DataTable dtItemFrmJson = clsTransaction.clsJsonACCIteminfoList_.ToDataTable();
            //DataTable dtUnitFrmJson = clsTransaction.clsJsonACCUnitinfoList_.ToDataTable();
            if (dtGetPurDetail.Rows.Count > 0)
            {
                AddColumnsToGrid();
                for (int i = 0; i < dtGetPurDetail.Rows.Count; i++)
                {
                    dgvItems.Rows.Add();
                    SetValue(GetEnum(gridColIndexes.cSlNo), i, (i + 1).ToString());
                    dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.CLedgerCode)].Value = Comm.GetTableValue("tblLedger", "LALIASNAME", " Where LID=" + dtGetPurDetail.Rows[i]["LID"].ToString()); //dtGetPurDetail.Rows[i]["LALiasName"].ToString();
                    dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.CLedgerName)].Value = Comm.GetTableValue("tblLedger", "LALIASNAME", " Where LID=" + dtGetPurDetail.Rows[i]["LID"].ToString()); //dtItemFrmJson.Rows[i]["LName"].ToString();
                    dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.CCurBal)].Value = "NA"; // dtGetPurDetail.Rows[i]["CurBal"].ToString();
                    dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cAmountDr)].Value = dtGetPurDetail.Rows[i]["AmountD"].ToString();
                    dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cAmountCr)].Value = dtGetPurDetail.Rows[i]["AmountC"].ToString();
                    dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cLedgerID)].Value = dtGetPurDetail.Rows[i]["LID"].ToString();
                }

                dgvItems.Columns[GetEnum(gridColIndexes.cLedgerID)].Visible = false;

                CalcTotal();
            }
        }

        //Description : CRUD Operational Method for Insert, Update and Delete.
        private void CRUD_Operations(int iAction = 0)
        {
            dgvItems.EndEdit();
            //dgvItems.CurrentCell = dgvItems[1, dgvItems.CurrentRow.Index];

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

                        #region "CRUD Operations for Accounts Master ------------------------- >>"

                        string sRet = clsAcc.InsertUpdateDeleteAccVoucherInsert(clsJPMinfo, sqlConn, trans, strJson, iAction);
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
                                //    Comm.MessageboxToasted("Accounts", "Accounts Group saved successfully");
                            }
                        }
                        #endregion

                        #region "CRUD Operations for Accounts Detail ------------------------- >>"
                        Hashtable hstPurStk = new Hashtable();

                        if (iAction == 1) // Edit
                        {
                            //trans.Commit();

                            sRetDet = clsAcc.InsertUpdateDeleteAccVoucherItemInsert(clsJAcc, sqlConn, trans, 2);
                            sRetDet = clsAcc.InsertUpdateDeleteAccVoucherItemInsert(clsJAcc, sqlConn, trans, 0);
                        }
                        else
                            sRetDet = clsAcc.InsertUpdateDeleteAccVoucherItemInsert(clsJAcc, sqlConn, trans, iAction);

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
                                //    Comm.MessageboxToasted("Accounts", "Voucher[" + txtInvAutoNo.Text + "] Saved Successfully");

                            }
                        }
                        #endregion

                        if (iAction < 2)
                        {
                            int IntOPtional = 0;
                            if (cmbStatus.SelectedIndex == 1) //"Collected"
                                IntOPtional = 0;
                            else
                                IntOPtional = 1;

                            decimal drlid = 0;
                            decimal crlid = 0;
                            double AmountD = 0;
                            double AmountC = 0;

                            if (clsVchType.ParentID != 10)
                            {
                                if (cboDrCr.SelectedIndex == 0)
                                {
                                    drlid = Convert.ToDecimal(txtLedger.Tag.ToString());
                                    crlid = 0;
                                    AmountD = Convert.ToDouble(lblBillAmount.Text.ToString());
                                    AmountC = 0;
                                }
                                else
                                {
                                    crlid = Convert.ToDecimal(txtLedger.Tag.ToString());
                                    drlid = 0;
                                    AmountD = 0;
                                    AmountC = Convert.ToDouble(lblBillAmount.Text.ToString());
                                }
                                Comm.VoucherInsert(Convert.ToInt32(cboCostCentre.SelectedValue.ToString()), vchtypeID, dtpInvDate.Value, DateAndTime.Now.ToLocalTime(), (drlid > 0) ? drlid : crlid, drlid, crlid, Convert.ToInt32(clsJAcc.clsJsonPMInfo_.InvId), clsJAcc.clsJsonPMInfo_.InvNo, txtNarration.Text.ToString(), AmountD, AmountC, 1, Convert.ToInt32(cboSalesStaff.SelectedValue.ToString()), IntOPtional, 0, false, txtNarration.Text.ToString());
                            }

                            for (int i = 0; i < dgvItems.Rows.Count - 1; i++)
                            {
                                if (dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cLedgerID)].Value == null) dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cLedgerID)].Value = "0";
                                if (dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cAmountDr)].Value == null) dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cAmountDr)].Value = "0";
                                if (dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cAmountCr)].Value == null) dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cAmountCr)].Value = "0";

                                if (Convert.ToInt32(dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cLedgerID)].Value.ToString()) != 0)
                                {
                                    if (clsVchType.ParentID == 10)
                                    {
                                        if (Convert.ToDouble(dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cAmountDr)].Value.ToString()) != 0)
                                        {
                                            drlid = Convert.ToDecimal(Convert.ToDecimal(dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cLedgerID)].Value.ToString()));
                                            crlid = 0;
                                            AmountD = Convert.ToDouble(dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cAmountDr)].Value.ToString());
                                            AmountC = 0;
                                        }
                                        else if (Convert.ToDouble(dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cAmountCr)].Value.ToString()) != 0)
                                        {
                                            drlid = 0;
                                            crlid = Convert.ToDecimal(dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cLedgerID)].Value.ToString());
                                            AmountD = 0;
                                            AmountC = Convert.ToDouble(dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cAmountCr)].Value.ToString());
                                        }
                                    }
                                    else
                                    {
                                        if (cboDrCr.SelectedIndex == 0)
                                        {
                                            drlid = 0;
                                            crlid = Convert.ToInt32(dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cLedgerID)].Value.ToString());
                                            AmountD = 0;
                                            AmountC = Convert.ToDouble(dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cAmountCr)].Value.ToString());
                                        }
                                        else
                                        {
                                            drlid = Convert.ToInt32(dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cLedgerID)].Value.ToString());
                                            crlid = 0;
                                            AmountD = Convert.ToDouble(dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cAmountDr)].Value.ToString());
                                            AmountC = 0;
                                        }
                                    }

                                    //Comm.VoucherInsert(Convert.ToInt32(cboCostCentre.SelectedValue.ToString()), vchtypeID, dtpInvDate.Value, DateAndTime.Now.ToLocalTime(), Convert.ToDecimal(dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cLedgerID)].Value.ToString()), drlid, crlid, Convert.ToInt32(clsJAcc.clsJsonPMInfo_.InvId), clsJAcc.clsJsonPMInfo_.InvNo, txtNarration.Text.ToString(), Convert.ToDouble(dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cAmountDr)].Value.ToString()), 0, 1, Convert.ToInt32(cboSalesStaff.SelectedValue.ToString()), IntOPtional, 0, false, txtNarration.Text.ToString());
                                    Comm.VoucherInsert(Convert.ToInt32(cboCostCentre.SelectedValue.ToString()), vchtypeID, dtpInvDate.Value, DateAndTime.Now.ToLocalTime(), (drlid > 0) ? drlid : crlid, drlid, crlid, Convert.ToInt32(clsJAcc.clsJsonPMInfo_.InvId), clsJAcc.clsJsonPMInfo_.InvNo, txtNarration.Text.ToString(), AmountD, AmountC, 1, Convert.ToInt32(cboSalesStaff.SelectedValue.ToString()), IntOPtional, 0, false, txtNarration.Text.ToString());

                                }
                            }
                        }

                        trans.Commit();
                        blnTransactionStarted = false;

                        string vchno = txtInvAutoNo.Text;


                        if (iAction < 2)
                        {
                            if (iIDFromEditWindow != 0)
                            {
                                this.Close();
                                Comm.MessageboxToasted("Accounts", "Voucher[" + vchno + "] Saved Successfully");
                                return;
                            }
                            else
                            {
                                ClearControls();
                                Comm.MessageboxToasted("Accounts", "Voucher[" + vchno + "] Saved Successfully");
                            }
                        }
                        else if (iAction == 2)
                        {
                            this.Close();
                            Comm.MessageboxToasted("Accounts", "Voucher[" + vchno + "] deleted successfully");
                            return;
                        }
                        else if (iAction == 3)
                        {
                            this.Close();
                            Comm.MessageboxToasted("Accounts", "Voucher[" + vchno + "] is archived");
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

            //dSupplierID = 0;

            txtChequeno.Text = "";
            txtBankName.Text = "";
            cmbStatus.SelectedIndex = 1;
            cmbStatus.SelectedText = "Collected";
            dtpChequedate.Value = DateTime.Now;

            txtTotalDr.Text = "";
            txtTotalCr.Text = "";
            
            txtNarration.Text = "";
            lblBillAmount.Text = "";
            
            if (ConvertI32(clsVchType.TransactionNumberingValue) == 2) // Custom
                txtInvAutoNo.Clear();
            if (ConvertI32(clsVchType.ReferenceNumberingValue) == 2) // Custom
                txtReferencePrefix.Clear();

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
        private Boolean GetFromSupplierSearch(string sReturn)
        {
            try
            {
                DataTable dtSupp = new DataTable();

                string[] sCompSearchData = sReturn.Split('|');
                List<decimal> lstItmDisc = new List<decimal>();
                if (sCompSearchData[0].ToString() == "NOTEXIST")
                {
                    return true;
                }
                else
                {
                    if (sCompSearchData.Length > 0)
                    {
                        if (sCompSearchData[0] != null)
                        {
                            if (sCompSearchData[0].ToString() != "")
                            {
                                if (Convert.ToInt32(sCompSearchData[0].ToString()) != 0)
                                {
                                    GetLedinfo.LID = Convert.ToInt32(sCompSearchData[0].ToString());
                                    GetLedinfo.TenantID = Global.gblTenantID;
                                    GetLedinfo.GroupName = "SUPPLIER";
                                    dtSupp = clsLedg.GetLedger(GetLedinfo);

                                    if (dtSupp.Rows.Count > 0)
                                    {
                                        SetValue(GetEnum(gridColIndexes.CLedgerCode), dtSupp.Rows[0]["LedgerCode"].ToString());
                                        SetValue(GetEnum(gridColIndexes.CLedgerName), dtSupp.Rows[0]["LedgerName"].ToString());
                                        SetValue(GetEnum(gridColIndexes.CCurBal), dtSupp.Rows[0]["CurBal"].ToString());
                                        //SetValue(GetEnum(gridColIndexes.cAmountDr), dtSupp.Rows[0]["AmountD"].ToString());
                                        //setTag(GetEnum(gridColIndexes.cAmountCr), dtSupp.Rows[0]["AmountC"].ToString());
                                        SetValue(GetEnum(gridColIndexes.cLedgerID), dtSupp.Rows[0]["LID"].ToString());

                                        if (clsVchType.ParentID == 10)
                                        {
                                            dgvItems.CurrentCell = dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cAmountDr)];
                                        }
                                        else
                                        {
                                            if (dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cAmountDr)].Visible == true)
                                                dgvItems.CurrentCell = dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cAmountDr)];
                                            else
                                                dgvItems.CurrentCell = dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cAmountCr)];
                                        }
                                        SetValue(GetEnum(gridColIndexes.CLedgerCode), dtSupp.Rows[0]["LedgerCode"].ToString());

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
        private Boolean GetFromLedgerSearch(string sReturn)
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
                            GetLedinfo.LID = Convert.ToInt32(sCompSearchData[0].ToString());
                            GetLedinfo.TenantID = Global.gblTenantID;
                            GetLedinfo.GroupName = "SUPPLIER";
                            dtSupp = clsLedg.GetLedger(GetLedinfo);

                            if (dtSupp.Rows.Count > 0)
                            {
                                this.txtLedger.TextChanged -= this.txtLedger_TextChanged;
                                txtLedger.Text = dtSupp.Rows[0].Field<string>("LedgerCode"); //sCompSearchData[1].ToString();
                                this.txtLedger.TextChanged += this.txtLedger_TextChanged;
                                lblLID.Text = dtSupp.Rows[0].Field<decimal>("LID").ToString();
                                txtLedger.Tag = lblLID.Text;

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
            //decimal dinvid = GetStockJournalIfo.InvId;
        }

        //Description : Initializing the Columns in The Grid
        private void AddColumnsToGrid()
        {
            this.dgvItems.Columns.Clear();

            this.dgvItems.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSlNo", HeaderText = "Sl.No", Width = 50 }); //1

            this.dgvItems.Columns.Add(new DataGridViewTextBoxColumn() { Name = "LedgerCode", HeaderText = "LedgerCode", Width = 200 }); 
            this.dgvItems.Columns.Add(new DataGridViewTextBoxColumn() { Name = "LedgerName", HeaderText = "LedgerName", Width = 200 }); 
            this.dgvItems.Columns.Add(new DataGridViewTextBoxColumn() { Name = "CurBal", HeaderText = "CurBal", Width = 130 }); 
            //if (clsVchType.ParentID == 10)
            //{
                this.dgvItems.Columns.Add(new DataGridViewTextBoxColumn() { Name = "AmountD", HeaderText = "Amount Dr", Width = 130 });
                this.dgvItems.Columns.Add(new DataGridViewTextBoxColumn() { Name = "AmountC", HeaderText = "Amount Cr", Width = 130 });
            //}
            //else
            //{
            //    this.dgvItems.Columns.Add(new DataGridViewTextBoxColumn() { Name = "AmountD", HeaderText = "Amount", Width = 130 });
            //    this.dgvItems.Columns.Add(new DataGridViewTextBoxColumn() { Name = "AmountC", HeaderText = "Amount Cr", Width = 130, Visible = false });
            //}
            this.dgvItems.Columns.Add(new DataGridViewTextBoxColumn() { Name = "LedgerID", HeaderText = "LedgerID", Width = 130, Visible = false });
            this.dgvItems.Columns.Add(new DataGridViewImageColumn() { Name = "cImgDel", HeaderText = "", Image = Properties.Resources.Delete_24_P4, Width = 40 });

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
                                dgvItems.Columns[k].Width = Convert.ToInt32(dtGridSettings.Rows[k][2].ToString());
                                dgvItems.Columns[k].Visible = Convert.ToBoolean(dtGridSettings.Rows[k][0]);
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
            
            dgvItems.Columns["cSlNo"].Frozen = true;
            //dgvItems.Columns["cImgDel"].Frozen = true;
            dgvItems.Columns["cImgDel"].Visible = true;
            dgvItems.Columns["cImgDel"].Width = 40;

            //DisableGridSettingsCheckbox();

            SetColumnsAsPerVchtype();

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
                if (dgvItems.CurrentCell.ColumnIndex == GetEnum(gridColIndexes.cSlNo))
                {
                    dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cSlNo)].ReadOnly = true;

                    strSelectedLedgerName = Convert.ToString(dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.CLedgerCode)].Value);
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
                string SSelectedLedgerCode = Convert.ToString(dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.CLedgerCode)].Value);
                if (SSelectedLedgerCode != "")
                {
                    DialogResult dlgResult = MessageBox.Show("Are you sure to delete Item Code[" + SSelectedLedgerCode + "] Permanently ?", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                    if (dlgResult == DialogResult.Yes)
                    {

                        Int32 selectedRowCount = dgvItems.Rows.GetRowCount(DataGridViewElementStates.Selected);
                        RowDelete();

                        dgvItems.Rows.Add();
                        dgvItems.CurrentCell = dgvItems.Rows[0].Cells[GetEnum(gridColIndexes.CLedgerCode)];

                        CalcTotal();
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
                if (dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.CLedgerCode)].Value != null)
                {
                    dSelectedLedgerID = Convert.ToInt32(dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cLedgerID)].Value);
                    if (dSelectedLedgerID > 0)
                    {
                        if (dgvItems.CurrentCell.ColumnIndex == (int)gridColIndexes.CLedgerName)
                        {
                            frmLedger frmIM = new frmLedger(Convert.ToInt32(dSelectedLedgerID), true);
                            frmIM.ShowDialog();
                        }
                        else if (dgvItems.CurrentCell.ColumnIndex == (int)gridColIndexes.CLedgerName)
                        {
                            frmLedger frmIM = new frmLedger(Convert.ToInt32(dSelectedLedgerID), true);
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
                if (dgvItems.CurrentCell.ColumnIndex == GetEnum(gridColIndexes.CCurBal))
                {
                    if (dgvItems.CurrentCell.Value != null)
                    {
                        dgvItems.CurrentCell = dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cAmountDr)];
                        dgvItems.Focus();
                    }
                }
                else if (dgvItems.CurrentCell.ColumnIndex == GetEnum(gridColIndexes.cAmountDr))
                {
                    dResult = Convert.ToDecimal(dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cAmountDr)].Value);
                    SetValue(GetEnum(gridColIndexes.cAmountDr), dResult.ToString(), "CURR_FLOAT");
                    //SendKeys.Send("{Tab}");

                    if (dgvItems.Rows.Count - 1 == dgvItems.CurrentRow.Index)
                        dgvItems.Rows.Add();
                    if (dResult != 0)
                    {
                        SendKeys.Send("{up}");
                        SendKeys.Send("{right}");
                    }
                    else
                        SendKeys.Send("{Tab}");
                }
                else if (dgvItems.CurrentCell.ColumnIndex == GetEnum(gridColIndexes.cAmountCr))
                {
                    dResult = Convert.ToDecimal(dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cAmountCr)].Value);
                    SetValue(GetEnum(gridColIndexes.cAmountCr), dResult.ToString(), "CURR_FLOAT");

                    if (dgvItems.Rows.Count - 1 == dgvItems.CurrentRow.Index)
                        dgvItems.Rows.Add();

                    if (dResult != 0)
                    {
                        SendKeys.Send("{up}");
                        SendKeys.Send("{right}");
                    }
                    else
                        SendKeys.Send("{Tab}");
                }

                this.dgvEndEditCell = dgvItems[e.ColumnIndex, e.RowIndex];
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
                                    if (iColumn == GetEnum(gridColIndexes.cAmountCr))
                                    {
                                        SendKeys.Send("{Tab}");
                                    }
                                    else if (iColumn == GetEnum(gridColIndexes.cAmountDr))
                                    {
                                        if (iRow < 0)
                                        {
                                            iRow = 0;

                                            if (dgvItems.Rows.Count <= iRow + 1)
                                                dgvItems.Rows.Add();

                                            if (GetEnum(gridColIndexes.CLedgerCode) == 1)
                                                dgvItems.CurrentCell = dgvItems[GetEnum(gridColIndexes.CLedgerCode), iRow + 1];
                                            else if (GetEnum(gridColIndexes.CCurBal) == 1)
                                                dgvItems.CurrentCell = dgvItems[GetEnum(gridColIndexes.CCurBal), iRow + 1];
                                        }
                                        else
                                        {
                                            if (dgvItems.Rows.Count <= iRow + 1)
                                                dgvItems.Rows.Add();

                                            if (GetEnum(gridColIndexes.CLedgerCode) == 1)
                                                dgvItems.CurrentCell = dgvItems[GetEnum(gridColIndexes.CLedgerCode), iRow + 1];
                                            else if (GetEnum(gridColIndexes.CCurBal) == 1)
                                                dgvItems.CurrentCell = dgvItems[GetEnum(gridColIndexes.CCurBal), iRow + 1];

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

        private void dgvItems_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            try
            {

                if (dgvItems.CurrentCell.EditType == typeof(DataGridViewTextBoxEditingControl))
                {
                    if (dgvItems.CurrentCell.ColumnIndex == GetEnum(gridColIndexes.CLedgerCode))
                    {
                        DataGridViewTextBoxEditingControl tb = (DataGridViewTextBoxEditingControl)e.Control;
                        tb.KeyPress += new KeyPressEventHandler(dgvItems_TextBox_KeyPress);
                        e.Control.KeyPress += new KeyPressEventHandler(dgvItems_TextBox_KeyPress);
                    }
                    else if (dgvItems.CurrentCell.ColumnIndex >= GetEnum(gridColIndexes.CCurBal) && dgvItems.CurrentCell.ColumnIndex < GetEnum(gridColIndexes.cLedgerID))
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

                    if (dgvItems.Rows[iRow].Cells[GetEnum(gridColIndexes.cAmountDr)].Value == null) dgvItems.Rows[iRow].Cells[GetEnum(gridColIndexes.cAmountDr)].Value = "0";
                    if (dgvItems.Rows[iRow].Cells[GetEnum(gridColIndexes.cAmountCr)].Value == null) dgvItems.Rows[iRow].Cells[GetEnum(gridColIndexes.cAmountCr)].Value = "0";

                    if (iColumn == dgvItems.Columns.Count - 1 && iRow != dgvItems.Rows.Count)
                    {
                        dgvItems.CurrentCell = dgvItems[0, iRow + 1];
                    }
                    else if (iColumn == dgvItems.Columns.Count - 1 && iRow == dgvItems.Rows.Count)
                    {
                    }
                    else if (iColumn == GetEnum(gridColIndexes.cAmountDr) && Convert.ToDecimal(dgvItems.Rows[iRow].Cells[GetEnum(gridColIndexes.cAmountDr)].Value.ToString()) != 0)
                    {
                        if (dgvItems.Rows.Count <= iRow + 1)
                            dgvItems.Rows.Add();
                        
                        dgvItems.CurrentCell = dgvItems[GetEnum(gridColIndexes.CLedgerCode), iRow + 1];
                    }
                    else if (iColumn == GetEnum(gridColIndexes.cAmountCr) && Convert.ToDecimal(dgvItems.Rows[iRow].Cells[GetEnum(gridColIndexes.cAmountCr)].Value.ToString()) != 0)
                    {
                        if (dgvItems.Rows.Count <= iRow + 1)
                            dgvItems.Rows.Add();
                        
                        dgvItems.CurrentCell = dgvItems[GetEnum(gridColIndexes.CLedgerCode), iRow + 1];
                    }
                    else
                    {
                        SendKeys.Send("{Tab}");
                    }
                }
                else if (e.KeyCode == Keys.F3)
                {
                    if (dgvItems.CurrentCell.ColumnIndex == (int)gridColIndexes.CLedgerCode)
                    {
                        frmLedger frmim = new frmLedger(0, true);
                        frmim.ShowDialog();
                    }
                }
                else if (e.KeyCode == Keys.F4)
                {
                    if (dgvItems.CurrentCell.ColumnIndex == (int)gridColIndexes.CLedgerCode)
                    {
                        int iSelectedLedgerID = 0;
                        iSelectedLedgerID = Convert.ToInt32(dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cLedgerID)].Value);
                        if (iSelectedLedgerID > 0)
                        {
                            frmLedger frmIM = new frmLedger(iSelectedLedgerID, true);
                            frmIM.ShowDialog();
                        }
                    }
                }
                else if (e.KeyCode == Keys.Delete)
                {
                    string SSelectedLedgerCode = Convert.ToString(dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.CLedgerCode)].Value);
                    if ((SSelectedLedgerCode != "" || dgvItems.Rows.Count > 1) && dgvItems.CurrentRow.Index >= 0)
                    {
                        DialogResult dlgResult = MessageBox.Show("Are you sure to delete Item Code[" + SSelectedLedgerCode + "] Permanently ?", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                        if (dlgResult == DialogResult.Yes)
                        {
                            Int32 selectedRowCount = dgvItems.Rows.GetRowCount(DataGridViewElementStates.Selected);
                            RowDelete();
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

                    if (dgvItems.CurrentCell.ColumnIndex == (int)gridColIndexes.CLedgerCode)
                    {
                        sEditedValueonKeyPress = ((char)e.KeyValue).ToString();
                        if (sEditedValueonKeyPress != null)
                        {
                            string strCondition = "";

                            if (clsVchType.ParentID == 7)
                                strCondition = " and  accountgroupid not in (16,17) ";
                            else if (clsVchType.ParentID == 8)
                                strCondition = " and  accountgroupid not in (16,17) ";
                            else if (clsVchType.ParentID == 9)
                                strCondition = " and  accountgroupid in (16,17) ";

                            sQuery = "SELECT LedgerCode + LedgerName + MobileNo + Email + TaxNo AS anywhere, LedgerCode, LedgerName, MobileNo, CurBal, Email, TaxNo, LID " +
                                    " FROM     vwLedger   Where isnull(ActiveStatus, 1) = 1 " + strCondition;
                            frmDetailedSearch2 frmN = new frmDetailedSearch2(GetFromSupplierSearch, sQuery, "Anywhere|LedgerCode|LedgerName", dgvItems.Location.X + 55, dgvItems.Location.Y + 150, 7, 0, sEditedValueonKeyPress, 7, 0, "ORDER BY LedgerCode ASC", 0, 0, "Ledger Search...", 0, "250,250,150,150,150,150,0", true, "frmLedger", 20);
                            frmN.MdiParent = this.MdiParent;
                            frmN.Show(); 

                            if (dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cLedgerID)].Value != null)
                            {
                                this.dgvItems.EditingControlShowing -= this.dgvItems_EditingControlShowing;
                                dgvItems.CurrentCell = dgvItems.Rows[dgvItems.CurrentRow.Index].Cells[GetEnum(gridColIndexes.CCurBal)];
                                dgvItems.Focus();
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


        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        // Description : Disabling the Checkbox of Mandatory fields in Column Width Settings Grid
        private void DisableGridSettingsCheckbox()
        {
            string[] strDisableCol;
            List<string> lstDisableCol = new List<string>();
            lstDisableCol.Add("cSlNo");
            lstDisableCol.Add("CLedgerCode");
            lstDisableCol.Add("CLedgerName");
            lstDisableCol.Add("CCurBal");
            lstDisableCol.Add("cAmountDr");
            lstDisableCol.Add("cAmountCr");
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
                        
                        ValidateWidth_dgvColWidth(cell.RowIndex);

                        break;
                    }
                }
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
                if (i == gridColIndexes.cLedgerID)
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

                drCol["Name"] = dgvItems.Columns[i].HeaderText; 
                if (i == dgvItems.Columns[i].Index)
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
            dgvColWidth.Rows[5].Visible = false;
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
                else if (dgvColWidth.Rows[i].Cells[3].Value.ToString().ToUpper() == "CSRATE1" || dgvColWidth.Rows[i].Cells[3].Value.ToString().ToUpper() == "CSRATE1PER"
                    || dgvColWidth.Rows[i].Cells[3].Value.ToString().ToUpper() == "CSRATE2" || dgvColWidth.Rows[i].Cells[3].Value.ToString().ToUpper() == "CSRATE2PER"
                    || dgvColWidth.Rows[i].Cells[3].Value.ToString().ToUpper() == "CSRATE3" || dgvColWidth.Rows[i].Cells[3].Value.ToString().ToUpper() == "CSRATE3PER"
                    || dgvColWidth.Rows[i].Cells[3].Value.ToString().ToUpper() == "CSRATE4" || dgvColWidth.Rows[i].Cells[3].Value.ToString().ToUpper() == "CSRATE4PER"
                    || dgvColWidth.Rows[i].Cells[3].Value.ToString().ToUpper() == "CSRATE5" || dgvColWidth.Rows[i].Cells[3].Value.ToString().ToUpper() == "CSRATE5PER"
                    )
                    clsJPDGSinfo.blnVisible = false;
                else
                    clsJPDGSinfo.blnVisible = Convert.ToBoolean(dgvColWidth.Rows[i].Cells[0].Value);
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
        private void CalcTotal()
        {
            txtTotalDr.Text = "";
            txtTotalCr.Text = "";

            for (int i = 0; i < dgvItems.Rows.Count - 1; i++)
            {
                if (clsVchType.ParentID != 10)
                {
                    if (cboDrCr.SelectedIndex == 0)
                        dgvItems.Rows[0].Cells[GetEnum(gridColIndexes.cAmountDr)].Value = "0";
                    else if (cboDrCr.SelectedIndex == 1)
                        dgvItems.Rows[0].Cells[GetEnum(gridColIndexes.cAmountCr)].Value = "0";
                }
            }

            for (int i = 0; i < dgvItems.Rows.Count; i++)
            {
                if (dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cAmountDr)].Value == null) dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cAmountDr)].Value = "0";
                if (dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cAmountCr)].Value == null) dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cAmountCr)].Value = "0";
                if (txtTotalDr.Text == "") txtTotalDr.Text = "0";
                if (txtTotalCr.Text == "") txtTotalCr.Text = "0";

                dgvItems[gridColIndexes.cSlNo, i].Value = i + 1;

                if (clsVchType.ParentID != 10)
                {
                    if (cboDrCr.SelectedIndex == 0)
                    {
                        txtTotalCr.Text = FormatValue(Convert.ToDouble(txtTotalCr.Text.ToString()) + Convert.ToDouble(dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cAmountCr)].Value.ToString()));
                        txtTotalDr.Text = "0";
                    }
                    else
                    {
                        txtTotalDr.Text = FormatValue(Convert.ToDouble(txtTotalDr.Text.ToString()) + Convert.ToDouble(dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cAmountDr)].Value.ToString()));
                        txtTotalCr.Text = "0";
                    }
                }
                else
                {
                    txtTotalDr.Text = FormatValue(Convert.ToDouble(txtTotalDr.Text.ToString()) + Convert.ToDouble(dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cAmountDr)].Value.ToString()));
                    txtTotalCr.Text = FormatValue(Convert.ToDouble(txtTotalCr.Text.ToString()) + Convert.ToDouble(dgvItems.Rows[i].Cells[GetEnum(gridColIndexes.cAmountCr)].Value.ToString()));
                }
            }

            if (clsVchType.ParentID != 10)
                lblBillAmount.Text = FormatValue(Convert.ToDouble(txtTotalDr.Text) + Convert.ToDouble(txtTotalCr.Text));
            else
                lblBillAmount.Text = FormatValue(Convert.ToDouble(txtTotalDr.Text) - Convert.ToDouble(txtTotalCr.Text));
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
                    txtInvAutoNo.Text = Comm.gfnGetNextSerialNo("tblAccVoucher", "AutoNum", " VchTypeID=" + vchtypeID).ToString();
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
                    txtInvAutoNo.Text = Comm.gfnGetNextSerialNo("tblAccVoucher", "AutoNum", " VchTypeID=" + vchtypeID).ToString();
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
                    txtReferenceAutoNo.Text = Comm.gfnGetNextSerialNo("tblAccVoucher", "ReferenceAutoNO").ToString();
                txtReferenceAutoNo.ReadOnly = true;
                txtReferencePrefix.ReadOnly = true;
                txtReferencePrefix.Width = 55;
            }
            else if (ConvertI32(clsVchType.ReferenceNumberingValue) == 1) // Auto Editable
            {
                if (iIDFromEditWindow == 0)
                    txtReferenceAutoNo.Text = Comm.gfnGetNextSerialNo("tblAccVoucher", "ReferenceAutoNO").ToString();
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

            try
            { 
                cboCostCentre.SelectedValue = ConvertI32(clsVchType.PrimaryCCValue);
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
            if (AppSettings.NeedCostCenter == true)
                pnlCostCentre.Visible = true;
            else
                pnlCostCentre.Visible = false;

            dtpInvDate.MinDate = AppSettings.FinYearStart;
            dtpInvDate.MaxDate = AppSettings.FinYearEnd;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void txtLedger_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.ActiveControl != null)
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
                        {
                            lblLID.Text = "0";
                            txtLedger.Tag = lblLID.Text;
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

        private void txtLedger_Click(object sender, EventArgs e)
        {
            txtLedger.SelectAll();
        }

        private void txtLedger_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtLedger, true);
            txtLedger.SelectAll();
        }

        private void txtLedger_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
            {
                if (cboDrCr.Visible == true && cboDrCr.Enabled == true)
                    cboDrCr.Focus();
                else
                    cboSalesStaff.Focus();
                SendKeys.Send("{F4}");
            }
            else if (e.KeyCode == Keys.Enter)
            {
                dgvItems.CurrentCell = dgvItems.Rows[0].Cells[GetEnum(gridColIndexes.CLedgerCode)];
                dgvItems.Focus();
            }
            else //if (e.KeyCode == Keys.F12)
            {
                if (ConvertI32(clsVchType.blnShowSearchWindowByDefault) == 1)
                {
                    //string sQuery = "SELECT  LedgerName+LedgerCode+Phone+MobileNo+Address as AnyWhere,LedgerCode as [Supplier Code],LedgerName as [Supplier Name] ,MobileNo ,Address,LID  FROM tblLedger where UPPER(groupName)='SUPPLIER' AND TenantID=" + Global.gblTenantID + "";
                    //new frmCompactSearch(GetFromSupplierSearch, sQuery, "AnyWhere|LedgerCode|LedgerName|MobileNo|Address", txtSupplier.Location.X + 800, txtSupplier.Location.Y - 20, 4, 0, txtSupplier.Text, 4, 0, "ORDER BY LedgerCode ASC", 0, 0, "Supplier Search ...", 0, "100,200,100,200,0", true, "frmSupplier").ShowDialog();

                    Form fc = Application.OpenForms["frmDetailedSearch2"];
                    if (fc != null)
                    {
                        fc.Focus();
                        fc.BringToFront();
                        return;
                    }

                    sEditedValueonKeyPress = ((char)e.KeyValue).ToString();

                    string strCondition = "";
                    if (clsVchType.ParentID == 7)
                        strCondition = " and  accountgroupid in (16,17) ";
                    else if (clsVchType.ParentID == 8)
                        strCondition = " and  accountgroupid in (16,17) ";
                    else if (clsVchType.ParentID == 9)
                        strCondition = " and  accountgroupid in (16,17) ";

                    string sQuery = "SELECT LedgerCode + LedgerName + MobileNo + Email + TaxNo AS anywhere, LedgerCode, LedgerName, MobileNo, CurBal, Email, TaxNo, LID " +
                            " FROM     vwLedger   Where isnull(ActiveStatus, 1) = 1 " + strCondition;
                    frmDetailedSearch2 frmN = new frmDetailedSearch2(GetFromLedgerSearch, sQuery, "Anywhere|LedgerCode|LedgerName", this.Left + 155, this.Top + 20, 7, 0, sEditedValueonKeyPress, 7, 0, "ORDER BY LedgerCode ASC", 0, 0, "Ledger Search...", 0, "250,250,150,150,150,150,0", true, "frmLedger", 20);
                    frmN.MdiParent = this.MdiParent;
                    frmN.Show();
                }
            }
            //else if (e.KeyCode == Keys.F3)
            //{
            //    btnNewIcon.PerformClick();
            //}
            //else if (e.KeyCode == Keys.F4)
            //{
            //    btnEditIcon.PerformClick();
            //}
        }

        private void txtLedger_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtLedger);
        }

        private void txtChequeno_TextChanged(object sender, EventArgs e)
        {

        }

        private void cboDrCr_SelectedIndexChanged(object sender, EventArgs e)
        {
            //SetColumnsAsPerVchtype();
        }

        private void SetColumnsAsPerVchtype()
        {
            try
            {
                if (clsVchType.ParentID != 10)
                {
                    if (cboDrCr.SelectedIndex == 0)
                    {
                        if (this.ActiveControl != null)
                        {
                            if (this.ActiveControl.Name == cboDrCr.Name)
                            {
                                for (int i = 0; i < dgvItems.Rows.Count - 1; i++)
                                {
                                    dgvItems[gridColIndexes.cAmountCr, i].Value = dgvItems[gridColIndexes.cAmountDr, i].Value;
                                }
                            }
                        }
                        dgvItems.Columns[gridColIndexes.cAmountDr].Visible = false;
                        dgvItems.Columns[gridColIndexes.cAmountCr].Visible = true;
                    }
                    else
                    {
                        if (this.ActiveControl != null)
                        {
                            if (this.ActiveControl.Name == cboDrCr.Name)
                            {
                                for (int i = 0; i < dgvItems.Rows.Count - 1; i++)
                                {
                                    dgvItems[gridColIndexes.cAmountDr, i].Value = dgvItems[gridColIndexes.cAmountCr, i].Value;
                                }
                            }
                        }
                        dgvItems.Columns[gridColIndexes.cAmountDr].Visible = true;
                        dgvItems.Columns[gridColIndexes.cAmountCr].Visible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void frmReceipt_Activated(object sender, EventArgs e)
        {
            try
            {
                LoadGridWidthFromItemGrid();
                DisableGridSettingsCheckbox();
                SetColumnsAsPerVchtype();
            }
            catch (Exception ex)
            {

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
                            dgvItems.Columns[dgvColWidth.Rows[RowIndex].Cells[3].Value.ToString()].Width = 50;
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

        private void dgvItems_ColumnWidthChanged(object sender, DataGridViewColumnEventArgs e)
        {
            if (toggleWidthSettings.ToggleState == Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active)
            {
                GridInitialize_dgvColWidth(false);
                try
                {
                    LoadGridWidthFromItemGrid();
                    DisableGridSettingsCheckbox();
                }
                catch (Exception ex)
                {

                }
            }
        }

        private void label11_Click(object sender, EventArgs e)
        {
            if (cboDrCr.Visible == true) cboDrCr.Enabled = true;
        }

        //Description : Convert to Int32 of Decimal Value
        private int ConvertI32(decimal dVal)
        {
            return Convert.ToInt32(dVal);
        }

        //Description : Load Saved data from database from edit window or Navigation buttons
        private void LoadData(int iSelectedID = 0)
        {
            try
            { 
            DataTable dtLoad = new DataTable();

                GetAccountsIfo.InvId = Convert.ToDecimal(iSelectedID);
                GetAccountsIfo.TenantID = Convert.ToDecimal(Global.gblTenantID);
                GetAccountsIfo.VchTypeID = vchtypeID;
            dtLoad = clsAcc.GetAccountsMaster(GetAccountsIfo, false);
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

                    //iAction = 1;

                }

                SetColumnsAsPerVchtype();

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
    public class ReceiptGridColIndexes
    {
        public int cSlNo = 0;
        public int CLedgerCode = 1;
        public int CLedgerName = 2;
        public int CCurBal = 3;
        public int cAmountDr = 4;
        public int cAmountCr = 5;
        public int cLedgerID = 6;
        public int cImgDel = 7;

        //This variabl;e holds the maximum cols index in this class
        public int MaxColIndex = 6;

        public string GetColumnName(int colIndex)
        {
            switch (colIndex)
            {
                case 0:
                    {
                        return nameof(cSlNo);

                        break;
                    }
                case 1:
                    {
                        return nameof(CLedgerCode);

                        break;
                    }
                case 2:
                    {
                        return nameof(CLedgerName);

                        break;
                    }
                case 3:
                    {
                        return nameof(CCurBal);

                        break;
                    }
                case 4:
                    {
                        return nameof(cAmountDr);

                        break;
                    }
                case 5:
                    {
                        return nameof(cAmountCr);

                        break;
                    }
                case 6:
                    {
                        return nameof(cLedgerID);

                        break;
                    }
                case 7:
                    {
                        return nameof(cImgDel);

                        break;
                    }
                default:
                    {
                        MessageBox.Show("Invalid column index | " + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);

                        return "";

                        break;
                    }
            }
        }

    }
    #endregion 
}
