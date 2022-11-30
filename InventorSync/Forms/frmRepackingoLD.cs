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

    public partial class frmRepacking : Form, IMessageFilter
    {
        //=============================================================================
        // Created By       : Dipu Joseph
        // Created On       : 02-Feb-2022
        // Last Edited On   :
        // Last Edited By   : Arun
        // Description      : Working With Different Voucher Type. Mainly For Sales, Sales RETURN, RECEIPT NOTE
        // Methods Used     : 
        //=============================================================================
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        public const int WM_LBUTTONDOWN = 0x0201;

        private ReportPrint prn = new ReportPrint();

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        private HashSet<Control> controlsToMove = new HashSet<Control>();

        private bool mblnInitialisedSubWindow = false;

        private frmCompactSearch frmSupplierSearch;
        private frmCompactSearch frmItemSearch;
        private frmCompactSearch frmBatchSearch;

        double outoflimitbillvalue = 0;

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
            int l = form.ClientSize.Width - 10; //(this.MdiParent.ClientSize.Width - this.Width) / 2;
            int t = form.ClientSize.Height - 80; //((this.MdiParent.ClientSize.Height - this.Height) / 2) - 30;
            this.SetBounds(5, 0, l, t);

            //this.SetBounds(l, t, this.Width, this.Height);

            //this.WindowState = FormWindowState.Maximized;

            clsVchType = JSonComm.GetVoucherType(iVchTpeId);
            clsVchTypeFeatures = JSonComm.GetVoucherTypeGeneralSettings(iVchTpeId, 1);



            AddColumnsToGrid();
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
                //FillTaxMode();
                FillCostCentre();
                //FillEmployee();
                //FillAgent();
                FillStates();
                //FillPriceList();

                SetTransactionsthatVarying();

                iIDFromEditWindow = Convert.ToInt32(iTransID);

                txtInvAutoNo.Select();
            }
            else
                SetTransactionsthatVarying();

            dgvStockOut.Controls.Add(dtp);
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
        bool bFromEditSales;
        decimal dCostRateInc = 0, dCostRateExcl = 0, dPRateIncl = 0, dPRateExcl = 0;
        decimal dSteadyBillDiscPerc, dSteadyBillDiscAmt;

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
        Rectangle _Rectangle;

        clsJsonVoucherType clsVchType = new clsJsonVoucherType();
        clsGetStockInVoucherSettings clsVchTypeFeatures = new clsGetStockInVoucherSettings();

        private RepackingGridColIndexes gridColIndexes = new RepackingGridColIndexes();


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


                string constr = Properties.Settings.Default.ConnectionString;


                DirectoryInfo dir = new DirectoryInfo(Application.StartupPath + @"\PrintScheme");
                FileInfo[] files = dir.GetFiles("*.rdlc");
                foreach (FileInfo file in files)
                {
                    cboInvScheme1.Items.Add(file.Name.Replace(".rdlc", ""));
                    comboBox8.Items.Add(file.Name.Replace(".rdlc", ""));
                }

                cboInvScheme1.SelectedIndex = 0;
                comboBox8.SelectedIndex = 0;

                SqlConnection conn = new SqlConnection();

                conn = new SqlConnection(constr);
                string query = "select InvScheme1,InvScheme2 from tblVchType where VchTypeID='" + vchtypeID + "' ";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.CommandText = query;
                conn.Open();
                SqlDataReader drd = cmd.ExecuteReader();

                if (drd.Read())
                {
                    try
                    {
                        for (int i = 0; i < cboInvScheme1.Items.Count; i++)
                        {
                            if (cboInvScheme1.Items[i].ToString() == drd["InvScheme1"].ToString())
                                cboInvScheme1.SelectedIndex = i;
                        }
                    }
                    catch
                    { }
                    try
                    {
                        for (int i = 0; i < comboBox8.Items.Count; i++)
                        {
                            if (comboBox8.Items[i].ToString() == drd["InvScheme2"].ToString())
                                comboBox8.SelectedIndex = i;
                        }
                    }
                    catch
                    { }
                }


                gridColIndexes.ChangeBarcodeMode(clsVchType.DefaultBarcodeMode);

                if (clsVchType.ParentID == 1 || clsVchType.ParentID == 3 || clsVchType.ParentID == 5)
                    lblParty.Text = "Customer";
                else
                    lblParty.Text = "Supplier";

                if (iIDFromEditWindow == 0)
                {
                    AddColumnsToGrid();
                    //FillTaxMode();
                    FillCostCentre();
                    //FillEmployee();
                    //FillAgent();
                    //FillPriceList();
                    FillStates();

                    txtSupplier.ReadOnly = false;
                }
                else
                {
                    LoadData(iIDFromEditWindow);

                    if (txtSupplier.Text != "")
                        txtSupplier.ReadOnly = true;
                }

                SetTransactionDefaults();
                SetApplicationSettings();

                cboState.SelectedValue = AppSettings.StateCode;
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
                    int iRowCnt = dgvStockOut.Rows.Count;
                    dgvStockOut.CurrentCell = dgvStockOut.Rows[iRowCnt - 1].Cells[GetEnum(gridColIndexes.CItemCode)];
                    dgvStockOut.Focus();
                    SendKeys.Send("{down}");
                }
                dgvStockOut.Columns["cRateinclusive"].Visible = false;
                dgvStockOut.Columns["cSRate1Per"].Visible = false;
                dgvStockOut.Columns["cSRate2Per"].Visible = false;
                dgvStockOut.Columns["cSRate3Per"].Visible = false;
                dgvStockOut.Columns["cSRate4Per"].Visible = false;
                dgvStockOut.Columns["cSRate5Per"].Visible = false;
                dgvStockOut.Columns["cSRate1"].Visible = false;
                dgvStockOut.Columns["cSRate2"].Visible = false;
                dgvStockOut.Columns["cSRate3"].Visible = false;
                dgvStockOut.Columns["cSRate4"].Visible = false;
                dgvStockOut.Columns["cSRate5"].Visible = false;
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
                            
                            //REQUESTED BY NIGIL ON 2-NOV-2022 TO SHOW ALL GROUPS ACCORDING TO SETTINGS
                            //sQuery = sQuery + " WHERE UPPER(L.groupName)='CUSTOMER' AND L.TenantID=" + Global.gblTenantID + "";
                            sQuery = sQuery + " WHERE ActiveStatus=1 and (UPPER(L.groupName)='SUPPLIER' or UPPER(L.groupName)='CUSTOMER') AND L.TenantID=" + Global.gblTenantID + "";
                            new frmCompactSearch(GetFromSupplierSearch, sQuery, "AnyWhere|LAliasName|LName|MobileNo|Address", txtSupplier.Location.X + 800, txtSupplier.Location.Y - 20, 4, 0, txtSupplier.Text, 4, 0, "ORDER BY L.LAliasName ASC", 0, 0, "Ledger Search ...", 0, "100,200,100,200,0", true, "frmCustomer").ShowDialog();

                            //if (txtMobile.Text != "")
                            //{
                            dgvStockOut.CurrentCell = dgvStockOut.Rows[0].Cells[1];
                            dgvStockOut.Focus();
                            //}
                            //else
                            //{
                            //    this.txtSupplier.TextChanged -= this.txtSupplier_TextChanged;
                            //    txtMobile.Focus();
                            //    this.txtSupplier.TextChanged += this.txtSupplier_TextChanged;
                            //}
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
            try
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
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
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
                    dgvStockOut.CurrentCell = dgvStockOut.Rows[0].Cells[1];
                    dgvStockOut.Focus();
                }
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
        }


        ComboBox BatchCode_GridCellComboBox = new ComboBox();

        private void gridColumn_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.') && ((int)gridColIndexes.cBarCode != dgvStockOut.CurrentCell.ColumnIndex))
            {
                e.Handled = true;
            }
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
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
                sEditedValueonKeyPress = e.KeyChar.ToString();

                //if (dgvStockOut.Rows.Count - 1 == dgvStockOut.CurrentRow.Index)
                //    dgvStockOut.Rows.Add();

                string EnterText = "";
                if (sender != null)
                {
                    TextBox tb = (TextBox)sender;
                    //if (tb.Text != null)
                    //{
                    //    EnterText = tb.Text.ToString();
                    //}
                    if (((int)e.KeyChar >= 65 && (int)e.KeyChar <= 90) || ((int)e.KeyChar >= 97 && (int)e.KeyChar <= 122) || ((int)e.KeyChar >= 48 && (int)e.KeyChar <= 57))
                        EnterText = EnterText + e.KeyChar;
                }

                if (dgvStockOut.CurrentCell.ColumnIndex == (int)gridColIndexes.CItemCode)
                {
                    if (sEditedValueonKeyPress != null)
                    {
                        if (AppSettings.TaxMode == 2) //GST
                        {
                            //sQuery = "SELECT (I.ItemCode+I.ItemName+CONVERT(VARCHAR,ISNULL(I.IGSTTaxPer,0))) as AnyWhere,I.ItemCode,I.ItemName,PRate,MRP,Rack,CONVERT(DECIMAL(18,2),I.IGSTTaxPer) as [GST %],I.ItemID,I.CategoryID,I.UNITID FROM tblCategories C INNER JOIN tblItemMaster I ON C.CategoryID = I.CategoryID AND I.ActiveStatus = 1 "; //

                            //if (clsVchType.ProductClassList != "")
                            //    sQuery = sQuery + " INNER JOIN tblItemMaster ITM ON ITM.ItemID = I.ItemID AND ITM.ProductTypeID IN (" + clsVchType.ProductClassList + ")";
                            //if (clsVchType.ItemCategoriesList != "")
                            //    sQuery = sQuery + " WHERE C.CategoryID IN (" + clsVchType.ItemCategoriesList + ")";
                            //frmDetailedSearch frmN = new frmDetailedSearch(GetFromItemSearch, sQuery, "Anywhere|I.ItemCode|I.ItemName|CONVERT(VARCHAR,ISNULL(I.IGSTTaxPer,0))", dgvStockOut.Location.X + 55, dgvStockOut.Location.Y + 150, 6, 0, sEditedValueonKeyPress, 6, 0, "ORDER BY I.ItemCode ASC", 0, 0, "Item Search...", 0, "150,250,100,100,100,100,0,0,0", true, "frmItemMaster", 20);
                            //frmN.Show(); //22-Apr-2022

                            sQuery = "SELECT TOP 200 ItemCode + ItemName + BatchUnique + CAST(MRP AS varchar) AS searchanywhere, ItemCode, ItemName, BatchUnique, QOH, MRP, ItemID, StockID " +
                                    " FROM     vwCompactSearch Where isnull(ActiveStatus, 1) = 1 and isnull(StockActiveStatus, 1) = 1 and BatchUnique <> '<AUTO BARCODE>' ";
                            if (clsVchType.ProductClassList != "")
                                sQuery = sQuery + " AND ProductTypeID IN (" + clsVchType.ProductClassList + ")";
                            if (clsVchType.ItemCategoriesList != "")
                                sQuery = sQuery + " AND CategoryID IN (" + clsVchType.ItemCategoriesList + ")";


                            if (mblnInitialisedSubWindow == false)
                            {
                                mblnInitialisedSubWindow = true;
                                frmDetailedSearch2 frmN = new frmDetailedSearch2(GetFromItemSearch, sQuery, "Anywhere|ItemCode|ItemName|BatchUnique", dgvStockOut.Location.X + 55, dgvStockOut.Location.Y + 150, 7, 0, EnterText, 6, 0, "ORDER BY ItemCode ASC, Batchunique", 0, 0, "Item Search...", 0, "150,250,150,150,150,0,0", true, "frmItemMaster", 20, false, this.MdiParent, 6);
                                frmN.MdiParent = this.MdiParent;
                                frmN.Show(); //12-SEP-2022
                            }
                        }
                        else
                        {
                            //sQuery = "SELECT (I.ItemCode+I.ItemName+CONVERT(VARCHAR,ISNULL(I.IGSTTaxPer,0))) as AnyWhere,I.ItemCode,I.ItemName,PRate,MRP,Rack,CONVERT(DECIMAL(18,2),I.IGSTTaxPer) as [GST %],I.ItemID,I.CategoryID,I.UNITID FROM tblCategories C INNER JOIN tblItemMaster I ON C.CategoryID = I.CategoryID AND I.ActiveStatus = 1 "; //
                            //if (clsVchType.ProductClassList != "")
                            //    sQuery = sQuery + " INNER JOIN tblItemMaster ITM ON ITM.ItemID = I.ItemID AND ITM.ProductTypeID IN (" + clsVchType.ProductClassList + ")";
                            //if (clsVchType.ItemCategoriesList != "")
                            //    sQuery = sQuery + " WHERE C.CategoryID IN (" + clsVchType.ItemCategoriesList + ")";
                            //frmDetailedSearch frmN = new frmDetailedSearch(GetFromItemSearch, sQuery, "Anywhere|I.ItemCode|I.ItemName", dgvStockOut.Location.X + 55, dgvStockOut.Location.Y + 150, 6, 0, sEditedValueonKeyPress, 4, 0, "ORDER BY I.ItemCode ASC", 0, 0, "Item Search...", 0, "150,250,100,100,0,0,0,0", true, "frmItemMaster", 20);
                            //frmN.Show(); //22-Apr-2022

                            sQuery = "SELECT TOP 200 ItemCode + ItemName + BatchUnique + CAST(MRP AS varchar) AS searchanywhere, ItemCode, ItemName, BatchUnique, QOH, MRP, ItemID, StockID " +
                                    " FROM     vwCompactSearch Where isnull(ActiveStatus, 1) = 1 and isnull(StockActiveStatus, 1) = 1 and BatchUnique <> '<AUTO BARCODE>' ";
                            if (clsVchType.ProductClassList != "")
                                sQuery = sQuery + " AND ProductTypeID IN (" + clsVchType.ProductClassList + ")";
                            if (clsVchType.ItemCategoriesList != "")
                                sQuery = sQuery + " AND CategoryID IN (" + clsVchType.ItemCategoriesList + ")";

                            if (mblnInitialisedSubWindow == false)
                            {
                                mblnInitialisedSubWindow = true;
                                frmDetailedSearch2 frmN = new frmDetailedSearch2(GetFromItemSearch, sQuery, "Anywhere|ItemCode|ItemName|BatchUnique", dgvStockOut.Location.X + 55, dgvStockOut.Location.Y + 150, 7, 0, EnterText, 6, 0, "ORDER BY ItemCode ASC, Batchunique", 0, 0, "Item Search...", 0, "150,250,150,150,150,0,0", true, "frmItemMaster", 20, false, this.MdiParent, 6);
                                frmN.MdiParent = this.MdiParent;
                                frmN.Show(); //12-SEP-2022
                            }
                        }

                        if (dgvStockOut.CurrentRow != null)
                        {
                            if (dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cItemID)].Value != null)
                            {
                                this.dgvStockOut.EditingControlShowing -= this.dgvStockOut_EditingControlShowing;
                                dgvStockOut.CurrentCell = dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cBarCode)];
                                dgvStockOut.Focus();
                                this.dgvStockOut.EditingControlShowing += this.dgvStockOut_EditingControlShowing;
                                //if (dgvStockOut.Rows.Count - 1 == dgvStockOut.CurrentRow.Index)
                                //    dgvStockOut.Rows.Add();
                            }
                        }
                    }
                }
                else if (dgvStockOut.CurrentCell.ColumnIndex == (int)gridColIndexes.cBarCode)
                {
                    if ((int)gridColIndexes.cBarCode > 1)
                    {
                        if (dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[(int)gridColIndexes.cBarCode].Value != null)
                            sEditedValueonKeyPress = dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[(int)gridColIndexes.cBarCode].Value.ToString();
                        else
                            sEditedValueonKeyPress = "";
                        if (sEditedValueonKeyPress != null)
                        {
                            if (dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cItemID)].Value != null)
                            {
                                Form fcC = Application.OpenForms["frmDetailedSearch2"];
                                if (fcC != null)
                                {
                                    fcC.Focus();
                                    fcC.BringToFront();
                                    return;
                                }

                                CallBatchCodeCompact();

                                dgvStockOut.CurrentCell = dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(gridColIndexes.CExpiry)];
                                dgvStockOut.Focus();

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

        private void dtp_TextChange(object sender, EventArgs e)
        {
            dgvStockOut.CurrentRow.Cells[GetEnum(gridColIndexes.CExpiry)].Value = dtp.Text.ToString();
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
                if (this.ActiveControl != null)
                {
                    if (this.ActiveControl.Name != txtDiscPerc.Name) return;
                }
                else
                {
                    return;
                }

                //txtDiscAmt.Enabled = false;
                //txtDiscAmt.Tag = "amount";

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
                    if (Convert.ToDouble(txtGrossAfterItmDisc.Text) > 0)
                    {
                        this.txtDiscAmt.TextChanged -= this.txtDiscAmt_TextChanged;
                        txtDiscAmt.Text = Comm.FormatValue((Convert.ToDouble(txtGrossAfterItmDisc.Text) * Convert.ToDouble(txtDiscPerc.Text) / 100));
                        this.txtDiscAmt.TextChanged += this.txtDiscAmt_TextChanged;
                    }
                    else
                    {
                        if (Convert.ToDouble(txtDiscPerc.Text) > 0)
                        {
                            this.txtDiscAmt.TextChanged -= this.txtDiscAmt_TextChanged;
                            txtDiscAmt.Text = Comm.FormatValue((Convert.ToDouble(txtGrossAmt.Text) * Convert.ToDouble(txtDiscPerc.Text) / 100));
                            this.txtDiscAmt.TextChanged += this.txtDiscAmt_TextChanged;
                        }
                    }
                    if (txtDiscAmt.Text == "") txtDiscAmt.Text = "0";
                }
                //if (Convert.ToDecimal(txtDiscAmt.Text) > 0)
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
                    //txtNarration.Focus();

                    SendKeys.Send("{TAB}");

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
            //try
            //{
            //    if (txtDiscAmt.Text.Trim() != ".")
            //    {
            //        if (txtDiscAmt.Text == "") { txtDiscAmt.Text = "0"; txtDiscAmt.SelectAll(); }
            //        if (Convert.ToDecimal(txtDiscAmt.Text) > 0)
            //        {
            //            this.txtDiscAmt.TextChanged -= this.txtDiscAmt_TextChanged;
            //            CalcTotal();
            //            this.txtDiscAmt.TextChanged += this.txtDiscAmt_TextChanged;
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
            //    MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);

            //}
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
                //    if (Convert.ToDouble(txtcashDisper.Text) >= 0)
                //        txtCashDisc.Text = Comm.FormatValue((Convert.ToDouble(txtNetAmt.Text) * Convert.ToDouble(txtcashDisper.Text) / 100));
                //}
                if (txtCashDisc.Text.Trim() != ".")
                {
                    if (txtCashDisc.Text == "") { txtCashDisc.Text = "0"; txtCashDisc.SelectAll(); }
                    if (Convert.ToDouble(txtCashDisc.Text) > 0)
                    {
                        this.txtcashDisper.TextChanged -= this.txtcashDisper_TextChanged;
                        txtcashDisper.Text = Comm.FormatValue((Convert.ToDouble(txtCashDisc.Text) * 100) / Convert.ToDouble(txtNetAmt.Text));
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
                        lblBillAmount.Text = Comm.FormatValue((Convert.ToDouble(lblBillAmount.Text) + Conversion.Val(txtRoundOff.Text.ToString())));

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
                //txtCostFactor.Focus();
                //txtCostFactor.SelectAll();

                SendKeys.Send("{TAB}");

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
                    txtInvAutoNo.Text = Comm.gfnGetNextSerialNo("tblSales", "AutoNum", " VchTypeID=" + vchtypeID).ToString();
                    CRUD_Operations(0, true);

                    lblHeading.Text = "Sales " + i.ToString() + " / 100000 ";

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

                string id = "";
                if (iIDFromEditWindow == 0)
                {
                    id = clsJPMinfo.InvId.ToString();
                }
                else
                {
                    id = iIDFromEditWindow.ToString();
                }
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
                    dgvStockOut.CurrentCell = dgvStockOut.Rows[0].Cells[1];
                    dgvStockOut.Focus();
                }
                else
                    txtMobile.Focus();
            }
            else if (e.KeyCode == Keys.F12)
            {
                if (Comm.ConvertI32(clsVchType.blnShowSearchWindowByDefault) == 1)
                {
                    //string sQuery = "SELECT  LedgerName+LedgerCode+Phone+MobileNo+Address as AnyWhere,LedgerCode as [Customer Code],LedgerName as [Customer Name] ,MobileNo ,Address,LID  FROM tblLedger where UPPER(groupName)='CUSTOMER' AND TenantID=" + Global.gblTenantID + "";
                    //new frmCompactSearch(GetFromSupplierSearch, sQuery, "AnyWhere|LedgerCode|LedgerName|MobileNo|Address", txtSupplier.Location.X + 800, txtSupplier.Location.Y - 20, 4, 0, txtSupplier.Text, 4, 0, "ORDER BY LedgerCode ASC", 0, 0, "Customer Search ...", 0, "100,200,100,200,0", true, "frmCustomer").ShowDialog();

                    string sQuery = "SELECT LName+LAliasName+MobileNo+Address as AnyWhere,LALiasname as [Ledger Code],lname as [Ledger Name] ,MobileNo ,Address,LID,Email  FROM tblLedger L";
                    if (clsVchType.CustomerSupplierAccGroupList != "")
                        sQuery = sQuery + " INNER JOIN tblAccountGroup A ON A.AccountGroupID = L.AccountGroupID AND A.AccountGroupID IN (" + clsVchType.CustomerSupplierAccGroupList + ")";

                    //REQUESTED BY NIGIL ON 2-NOV-2022 TO SHOW ALL GROUPS ACCORDING TO SETTINGS
                    //sQuery = sQuery + " WHERE UPPER(L.groupName)='CUSTOMER' AND L.TenantID=" + Global.gblTenantID + "";
                    sQuery = sQuery + " WHERE ActiveStatus=1 and (UPPER(L.groupName)='SUPPLIER' or UPPER(L.groupName)='CUSTOMER') AND L.TenantID=" + Global.gblTenantID + "";
                    new frmCompactSearch(GetFromSupplierSearch, sQuery, "AnyWhere|LAliasName|LName|MobileNo|Address", txtSupplier.Location.X + 800, txtSupplier.Location.Y - 20, 4, 0, txtSupplier.Text, 4, 0, "ORDER BY L.LAliasName ASC", 0, 0, "Customer Search ...", 0, "100,200,100,200,0", true, "frmCustomer").ShowDialog();
                    
                    //if (txtMobile.Text != "")
                    //{
                    dgvStockOut.CurrentCell = dgvStockOut.Rows[0].Cells[1];
                    dgvStockOut.Focus();
                }
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
                //txtcashDisper.Focus();
                //txtcashDisper.SelectAll();
                SendKeys.Send("{TAB}");

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

        private void txtRoundOff_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                e.Handled = Comm.CheckNumeric(sender, e, true);
            }
            catch
            {

            }

            //try
            //{
            //    if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.') && (e.KeyChar != '-'))
            //    {
            //        e.Handled = true;
            //    }
            //    if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            //    {
            //        e.Handled = true;
            //    }
            //    if ((e.KeyChar == '-') && ((sender as TextBox).Text.IndexOf('-') > -1))
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

        private void txtcashDisper_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                e.Handled = Comm.CheckNumeric(sender, e);
            }
            catch
            {

            }

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

        private void txtcashDisper_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
            {
                txtCostFactor.Focus();
                txtCostFactor.SelectAll();
            }
            else if (e.KeyCode == Keys.Enter)
            {
                //txtCashDisc.Focus();
                //txtCashDisc.SelectAll();

                SendKeys.Send("{TAB}");

            }
        }

        private void txtDiscPerc_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
            {
                dgvStockOut.Focus();
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
                    //txtOtherExp.Focus();
                    //txtOtherExp.SelectAll();

                    SendKeys.Send("{TAB}");

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
                    if (dgvStockOut.Rows.Count <= 0) dgvStockOut.Rows.Add();
                    dgvStockOut.CurrentCell = dgvStockOut.Rows[0].Cells[1];
                    dgvStockOut.Focus();
                }
                else
                    txtTaxRegn.Focus();
            }

        }

        private void btnprev_Click(object sender, EventArgs e)
        {
            if (txtInvAutoNo.Tag.ToString() == "0")
            {
                if (dgvStockOut.Rows.Count > 0)
                {
                    if (dgvStockOut.Rows[0].Cells[GetEnum(gridColIndexes.CItemCode)].Value != null)
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
                if (dgvStockOut.Rows.Count > 0)
                {
                    if (dgvStockOut.Rows[0].Cells[GetEnum(gridColIndexes.CItemCode)].Value != null)
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
                else if (e.KeyCode == Keys.F9)
                {
                    for (int i = 0; i <= dgvStockOut.Rows.Count - 1; i++)
                    {
                        if (dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.CItemName)].Tag == null) dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.CItemName)].Tag = "0";
                        if (Comm.ToDecimal(dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.CItemName)].Tag.ToString()) == 0)
                        {
                            dgvStockOut.CurrentCell = dgvStockOut[1, i];
                            dgvStockOut.Focus();
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
                    //if (txtSupplier.Text != "")
                    //{
                    //    if (txtSupplier.Text != strCheck)
                    //    {
                    //        DialogResult dlgResult = MessageBox.Show("Your are in the middle of an Entry. Do you want to exit?", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                    //        if (dlgResult.Equals(DialogResult.Yes))
                    //            this.Close();
                    //    }
                    //    else
                    //    {
                    //        this.Close();
                    //    }

                    //}
                    //else
                    //{
                    //    this.Close();
                    //}

                    if (iIDFromEditWindow == 0)
                    {
                        if (dgvStockOut.Rows.Count > 0)
                        {
                            if (dgvStockOut.Rows[0].Cells[GetEnum(gridColIndexes.CItemCode)].Value != null)
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

        private void txtCashDisc_KeyDown_1(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
            {
                txtcashDisper.Focus();
                txtcashDisper.SelectAll();
            }
            else if (e.KeyCode == Keys.Enter)
            {
                //txtRoundOff.Focus();
                //txtRoundOff.SelectAll();

                SendKeys.Send("{TAB}");

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
                    if (Convert.ToDouble(txtcashDisper.Text) >= 0)
                    {
                        this.txtCashDisc.TextChanged -= this.txtCashDisc_TextChanged;
                        txtCashDisc.Text = Comm.FormatValue((Convert.ToDouble(txtNetAmt.Text) * Convert.ToDouble(txtcashDisper.Text) / 100));
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
                DataTable dtInv = Comm.fnGetData("SELECT ISNULL(JsonData,'') as JsonData,Invid FROM tblSales WHERE InvNo = '" + txtInvAutoNo.Text + "' AND VchTypeID=" + vchtypeID + " AND TenantID = " + Global.gblTenantID + "").Tables[0];
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
            if (Convert.ToInt32(AppSettings.StateCode) != Convert.ToInt32(cboState.SelectedValue))
            {
                dgvStockOut.Columns[GetEnum(gridColIndexes.cCGST)].Visible = false;
                dgvStockOut.Columns[GetEnum(gridColIndexes.cSGST)].Visible = false;
            }
            else
            {
                dgvStockOut.Columns[GetEnum(gridColIndexes.cCGST)].Visible = true;
                dgvStockOut.Columns[GetEnum(gridColIndexes.cSGST)].Visible = true;
            }
            CalcTotal();
        }

        private void btnNewIcon_Click(object sender, EventArgs e)
        {
            try
            {
                //if (this.ActiveControl.Name == "txtSupplier")
                //{
                this.ActiveControl.Name = btnNewIcon.Name;
                frmLedger frmLed = new frmLedger(0, true, 0, "CUSTOMER", txtSupplier);
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
                    frmLedger frmLed = new frmLedger(Convert.ToInt32(lblLID.Text), true, 0, "CUSTOMER", txtSupplier);
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

        private void txtSupplier_Click(object sender, EventArgs e)
        {
            txtSupplier.SelectAll();
        }

        private void txtDiscPerc_Leave(object sender, EventArgs e)
        {
            try
            {
                //if (Convert.ToDecimal(txtDiscAmt.Text) > 0)
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

        private void dgvColWidth_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                ValidateWidth_dgvColWidth(e.RowIndex);

                if (toggleWidthSettings.ToggleState == Syncfusion.Windows.Forms.Tools.ToggleButtonState.Inactive)
                {
                    for (int i = 0; i < dgvColWidth.Rows.Count; i++)
                    {
                        if (dgvStockOut.Columns[i].Name == dgvColWidth.Rows[i].Cells[3].Value.ToString())
                        {
                            dgvStockOut.Columns[i].Width = Convert.ToInt32(dgvColWidth.Rows[i].Cells[2].Value.ToString());
                            if (dgvColWidth.Rows[i].Cells[0].Value.ToString() == "")
                                dgvStockOut.Columns[i].Visible = false;
                            else
                                dgvStockOut.Columns[i].Visible = Convert.ToBoolean(dgvColWidth.Rows[i].Cells[0].Value);
                        }
                        if (dgvStockOut.Columns[i].Name == "cFree")
                        {
                            if (clsVchTypeFeatures.BLNSHOWFREEQUANTITY == false)
                                dgvStockOut.Columns[i].Visible = false;
                        }
                        if (dgvStockOut.Columns[i].Name == "cRateinclusive")
                            dgvStockOut.Columns[i].Visible = false;
                        if (dgvStockOut.Columns[i].Name == "cSRate1Per")
                            dgvStockOut.Columns[i].Visible = false;
                        if (dgvStockOut.Columns[i].Name == "cSRate2Per")
                            dgvStockOut.Columns[i].Visible = false;
                        if (dgvStockOut.Columns[i].Name == "cSRate3Per")
                            dgvStockOut.Columns[i].Visible = false;
                        if (dgvStockOut.Columns[i].Name == "cSRate4Per")
                            dgvStockOut.Columns[i].Visible = false;
                        if (dgvStockOut.Columns[i].Name == "cSRate5Per")
                            dgvStockOut.Columns[i].Visible = false;
                        if (dgvStockOut.Columns[i].Name == "cSRate1")
                            dgvStockOut.Columns[i].Visible = false;
                        if (dgvStockOut.Columns[i].Name == "cSRate2")
                            dgvStockOut.Columns[i].Visible = false;
                        if (dgvStockOut.Columns[i].Name == "cSRate3")
                            dgvStockOut.Columns[i].Visible = false;
                        if (dgvStockOut.Columns[i].Name == "cSRate4")
                            dgvStockOut.Columns[i].Visible = false;
                        if (dgvStockOut.Columns[i].Name == "cSRate5")
                            dgvStockOut.Columns[i].Visible = false;
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
                        dtInv = Comm.fnGetData("SELECT TOP 1 ISNULL(InvId,0) FROM tblSales WHERE VchTypeID = " + vchtypeID + " AND TenantID = " + Global.gblTenantID + " ORDER BY InvId DESC").Tables[0];
                        if (dtInv.Rows.Count > 0)
                            dInvId = Convert.ToDecimal(dtInv.Rows[0][0].ToString());
                        else
                            dInvId = 0;
                    }
                    else
                    {
                        dtInv = Comm.fnGetData("SELECT TOP 1 ISNULL(InvId,0) FROM tblSales WHERE InvId < " + Convert.ToDecimal(txtInvAutoNo.Tag.ToString()) + " AND VchTypeID = " + vchtypeID + " AND TenantID = " + Global.gblTenantID + " ORDER BY InvId DESC").Tables[0];
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
                        dtInv = Comm.fnGetData("SELECT TOP 1 ISNULL(InvId,0) FROM tblSales WHERE InvId > " + Convert.ToDecimal(txtInvAutoNo.Tag.ToString()) + " AND VchTypeID = " + vchtypeID + " AND TenantID = " + Global.gblTenantID + " ORDER BY InvId ASC").Tables[0];
                        if (dtInv.Rows.Count > 0)
                            dInvId = Convert.ToDecimal(dtInv.Rows[0][0].ToString());
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
                                //txtInvAutoNo.Text = Comm.gfnGetNextSerialNo("tblSales", "AutoNum").ToString();
                                txtInvAutoNo.Text = Comm.gfnGetNextSerialNo("tblSales", "AutoNum", " VchTypeID=" + vchtypeID).ToString();
                                txtInvAutoNo.Tag = 0;

                                txtInvAutoNo.ReadOnly = true;
                                txtPrefix.ReadOnly = true;
                            }
                            else if (Comm.ConvertI32(clsVchType.TransactionNumberingValue) == 1) // Auto Editable
                            {
                                //txtInvAutoNo.Text = Comm.gfnGetNextSerialNo("tblSales", "AutoNum").ToString();
                                txtInvAutoNo.Text = Comm.gfnGetNextSerialNo("tblSales", "AutoNum", " VchTypeID=" + vchtypeID).ToString();
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
                                    txtReferenceAutoNo.Text = Comm.gfnGetNextSerialNo("tblSales", "ReferenceAutoNO").ToString();
                                txtReferenceAutoNo.ReadOnly = true;
                                txtReferencePrefix.ReadOnly = true;
                                txtReferencePrefix.Width = 55;
                            }
                            else if (Comm.ConvertI32(clsVchType.ReferenceNumberingValue) == 1) // Auto Editable
                            {
                                if (iIDFromEditWindow == 0)
                                    txtReferenceAutoNo.Text = Comm.gfnGetNextSerialNo("tblSales", "ReferenceAutoNO").ToString();
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
                            iIDFromEditWindow = Convert.ToInt32(dInvId);
                            LoadData(Convert.ToInt32(dInvId));
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

            sArrLedger = new string[namesCount];

            GetLedinfo.LID = iLedgerID;
            GetLedinfo.TenantID = Global.gblTenantID;
            GetLedinfo.GroupName = "CUSTOMER";
            dtSupp = clsLedg.GetLedger(GetLedinfo);
            if (dtSupp.Rows.Count > 0)
            {
                sArrLedger[GetEnumLedger(LedgerIndexes.LName)] = txtSupplier.Text;
                sArrLedger[GetEnumLedger(LedgerIndexes.LAliasName)] = Comm.CheckDBNullOrEmpty(dtSupp.Rows[0]["LedgerName"].ToString());
                sArrLedger[GetEnumLedger(LedgerIndexes.Address)] = txtAddress1.Text;
                sArrLedger[GetEnumLedger(LedgerIndexes.TaxNo)] = txtTaxRegn.Text;
                sArrLedger[GetEnumLedger(LedgerIndexes.MobileNo)] = txtMobile.Text;

                if (cboState.SelectedValue == null)
                    FillStates(Convert.ToInt32(dtSupp.Rows[0]["StateID"].ToString()));

                if (cboBType.SelectedIndex < 0)
                    cboBType.SelectedIndex = 1;

                sArrLedger[GetEnumLedger(LedgerIndexes.StateID)] = cboState.SelectedValue.ToString();
                sArrLedger[GetEnumLedger(LedgerIndexes.GSTType)] = cboBType.SelectedItem.ToString();
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
                this.txtSupplier.TextChanged -= this.txtSupplier_TextChanged;
                txtSupplier.Text = dtSupp.Rows[0]["LedgerName"].ToString();
                this.txtSupplier.TextChanged += this.txtSupplier_TextChanged;
                lblLID.Text = dtSupp.Rows[0]["LID"].ToString();
                txtAddress1.Text = dtSupp.Rows[0]["Address"].ToString();
                txtMobile.Text = dtSupp.Rows[0]["MobileNo"].ToString();
                txtTaxRegn.Text = dtSupp.Rows[0]["TaxNo"].ToString();
                FillStates(Convert.ToInt32(dtSupp.Rows[0]["StateID"].ToString()));
                txtSupplier.Tag = dtSupp.Rows[0]["LedgerCode"].ToString();
                txtAddress1.Tag = dtSupp.Rows[0]["Email"].ToString();
                dSupplierID = Convert.ToDecimal(dtSupp.Rows[0]["LID"].ToString());
                cboBType.Text = dtSupp.Rows[0]["GSTType"].ToString();
                cboPriceList.SelectedValue = dtSupp.Rows[0]["PLID"].ToString();

                if (cboPriceList.SelectedIndex < 0 && cboPriceList.Items.Count > 0)
                {
                    cboPriceList.SelectedIndex = 0;
                }


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
                    if (Convert.ToInt32(sCompSearchData[0].ToString()) != 0)
                    {
                        return FillSupplierUsingID(Convert.ToInt32(sCompSearchData[0].ToString()));
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
                SetValue(GetEnum(gridColIndexes.CExpiry), dtCurrExp.ToString("dd-MMM-yyyy").ToString(), "DATE");
            }
            else
            {
                DataTable dtData = new DataTable();
                GetStockInfo.BatchUnique = sBarUnique;
                GetStockInfo.BatchCode = "";
                GetStockInfo.StockID = 0;
                GetStockInfo.TenantID = Global.gblTenantID;
                GetStockInfo.ItemID = Convert.ToDouble(dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cItemID)].Value);
                GetStockInfo.CCID = Convert.ToDouble(cboCostCentre.SelectedValue);
                dtData = clsStock.GetStockDetails(GetStockInfo);
                if (dtData.Rows.Count > 0)
                {
                    SetValue(GetEnum(gridColIndexes.CExpiry), dtData.Rows[0]["ExpiryDate"].ToString(), "DATE");
                    SetValue(GetEnum(gridColIndexes.cMRP), dtData.Rows[0]["MRP"].ToString(), "CURR_FLOAT");
                    SetValue(GetEnum(gridColIndexes.cSrate), dtData.Rows[0]["SRate1"].ToString(), "CURR_FLOAT");
                    if (cboPriceList.Visible == true && cboPriceList.Enabled == true)
                    {
                        //SetPriceListForItems(dgvStockOut.CurrentRow.Index);
                        SetValue(GetEnum(gridColIndexes.cSrate), SetPriceListForItems(dgvStockOut.CurrentRow.Index).ToString(), "CURR_FLOAT");
                    }
                    SetValue(GetEnum(gridColIndexes.cGrossAmt), dtData.Rows[0]["PRateExcl"].ToString(), "CURR_FLOAT");
                    SetValue(GetEnum(gridColIndexes.cItemID), dtData.Rows[0]["ItemID"].ToString(), "INT");
                    dQty = Convert.ToDecimal(dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cQty)].Value);
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
                GetStockInfo.ItemID = Convert.ToDouble(dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cItemID)].Value);
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
                    SetValue(GetEnum(gridColIndexes.cSrate), dtData.Rows[0]["SRate1"].ToString(), "CURR_FLOAT");
                    if (cboPriceList.Visible == true && cboPriceList.Enabled == true)
                    {
                        //SetPriceListForItems(dgvStockOut.CurrentRow.Index);
                        SetValue(GetEnum(gridColIndexes.cSrate), SetPriceListForItems(dgvStockOut.CurrentRow.Index).ToString(), "CURR_FLOAT");
                    }
                    SetValue(GetEnum(gridColIndexes.cGrossAmt), dtData.Rows[0]["PrateInc"].ToString(), "CURR_FLOAT");
                    SetValue(GetEnum(gridColIndexes.cItemID), dtData.Rows[0]["ItemID"].ToString(), "INT");
                    dQty = Convert.ToDecimal(dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cQty)].Value);
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
                if (iSelID != 0)
                    cboAgent.SelectedValue = iSelID;

                GetAgentDiscountAsperVoucherType();
            }
        }
        //Description: Fill Pricelist Details according to settings
        private void FillPriceList(int iSelID = 0)
        {
            DataTable dtPriceList = new DataTable();
            //dtPriceList = GetAgent(0);

            DataColumn dc = new DataColumn("PLName", typeof(String));
            DataColumn dc1 = new DataColumn("PLID", typeof(int));

            dtPriceList.Columns.Add(dc);
            dtPriceList.Columns.Add(dc1);

            if (AppSettings.IsActiveSRate1 == true)
            {
                DataRow dRow1 = dtPriceList.NewRow();
                dRow1[0] = AppSettings.SRate1Name;
                dRow1[1] = 1;
                dtPriceList.Rows.Add(dRow1);
            }
            if (AppSettings.IsActiveSRate2 == true)
            {
                DataRow dRow1 = dtPriceList.NewRow();
                dRow1[0] = AppSettings.SRate2Name;
                dRow1[1] = 2;
                dtPriceList.Rows.Add(dRow1);
            }
            if (AppSettings.IsActiveSRate1 == true)
            {
                DataRow dRow1 = dtPriceList.NewRow();
                dRow1[0] = AppSettings.SRate3Name;
                dRow1[1] = 3;
                dtPriceList.Rows.Add(dRow1);
            }
            if (AppSettings.IsActiveSRate1 == true)
            {
                DataRow dRow1 = dtPriceList.NewRow();
                dRow1[0] = AppSettings.SRate4Name;
                dRow1[1] = 4;
                dtPriceList.Rows.Add(dRow1);
            }
            if (AppSettings.IsActiveSRate1 == true)
            {
                DataRow dRow1 = dtPriceList.NewRow();
                dRow1[0] = AppSettings.SRate5Name;
                dRow1[1] = 5;
                dtPriceList.Rows.Add(dRow1);
            }

            if (dtPriceList.Rows.Count > 0)
            {
                cboPriceList.DataSource = dtPriceList;
                cboPriceList.DisplayMember = "PLName";
                cboPriceList.ValueMember = "PLID";

                cboPriceList.SelectedValue = 1;
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
                        if (Convert.ToInt32(row["StateId"].ToString()) == iSelID)
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
            if (dgvStockOut.Rows.Count > 0)
            {
                if (sConvertType.ToUpper() == "CURR_FLOAT")
                {
                    if (sValue == "") sValue = "0";
                    //Commented and Added By Dipu on 17-Feb-2022 ------------- >>
                    dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[iCellIndex].Value = Comm.chkChangeValuetoZero(Comm.FormatValue(Convert.ToDouble(sValue)));
                    //Commented and Added By Dipu on 17-Feb-2022 ------------- >>

                    this.dgvStockOut.Columns[dgvStockOut.CurrentCell.ColumnIndex].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
                else if (sConvertType.ToUpper() == "QTY_FLOAT")
                {
                    if (sValue == "") sValue = "0";
                    dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[iCellIndex].Value = Comm.chkChangeValuetoZero(Comm.FormatValue(Convert.ToDouble(sValue), false));
                    this.dgvStockOut.Columns[dgvStockOut.CurrentCell.ColumnIndex].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
                else if (sConvertType.ToUpper() == "PERC_FLOAT")
                {
                    if (sValue == "") sValue = "0";
                    dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[iCellIndex].Value = Comm.chkChangeValuetoZero(Convert.ToDecimal(sValue).ToString("#.00"));
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
        private void setTag(int iCellIndex, string sTag, string sConvertType = "")
        {
            if (sConvertType.ToUpper() == "FLOAT")
            {
                if (sTag == "") sTag = "0";
                dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[iCellIndex].Tag = Convert.ToDecimal(sTag).ToString("#.00");
            }
            else if (sConvertType.ToUpper() == "DATE")
            {
                dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[iCellIndex].Tag = Convert.ToDateTime(sTag).ToString("dd/MMM/yyyy");
            }
            else
                dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[iCellIndex].Tag = sTag;
        }

        //Description : Function Polymorphism for Seting the Value to Grid Asper Parameters Given
        private void SetValue(int iCellIndex, int iRowIndex, string sValue, string sConvertType = "")
        {
            //if(sConvertType.ToUpper() == "QTY")
            //    dgvStockOut.Rows[iRowIndex].Cells[iCellIndex].Value = Comm.chkChangeValuetoZero(Comm.FormatValue(Convert.ToDouble(sValue),false));
            //else
            dgvStockOut.Rows[iRowIndex].Cells[iCellIndex].Value = Comm.chkChangeValuetoZero(sValue);
        }

        //Description : Check the conditions of Supplier While Entered or Non Entred
        private bool CheckIsValidSupplier()
        {
            DataTable dtSupp = new DataTable();
            bool bResult = true;
            if (lblLID.Text == "") lblLID.Text = "0";
            if (txtSupplier.Text == "")
            {
                dtSupp = Comm.fnGetData("SELECT * FROM tblLedger WHERE LID = 101").Tables[0];
                if (dtSupp.Rows.Count > 0)
                {
                    this.txtSupplier.TextChanged -= this.txtSupplier_TextChanged;
                    txtSupplier.Text = dtSupp.Rows[0]["LName"].ToString();
                    this.txtSupplier.TextChanged += this.txtSupplier_TextChanged;

                    txtMobile.Text = dtSupp.Rows[0]["MobileNo"].ToString();
                    txtTaxRegn.Text = dtSupp.Rows[0]["TaxNo"].ToString();
                    cboState.SelectedValue = Convert.ToDecimal(dtSupp.Rows[0]["StateID"].ToString());
                    cboBType.Text = dtSupp.Rows[0]["GSTType"].ToString();
                    txtAddress1.Text = dtSupp.Rows[0]["Address"].ToString();
                    lblLID.Text = dtSupp.Rows[0]["LID"].ToString();
                    bResult = true;
                }
                else
                    bResult = false;
            }
            else if (Convert.ToInt32(lblLID.Text) == 0 && txtSupplier.Text != "")
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

            CalcTotal();

            if (clsVchTypeFeatures.blnWarnifSRatelessthanPrate == true)
                sWarnMsg = WarnifSRatelessthanPrate();

            sMsg = sWarnMsg.Split('|');

            //if (clsVchTypeFeatures.BLNPOSTCASHENTRY == true)
            //{
            //    if (Convert.ToInt32(lblLID.Text.ToString()) <= 101)
            //    {
            //        bValidate = false;
            //        MessageBox.Show("Please select credit customer as you have chosen to post cash entries to supplier ledger.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            //        txtInvAutoNo.Focus();
            //        goto FailsHere;
            //    }
            //}
            //if (txtInvAutoNo.Text == "") txtInvAutoNo.Text = "0";
            if (txtInvAutoNo.Text == "")
            {
                bValidate = false;
                MessageBox.Show("Please Enter the Invoice No.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                txtInvAutoNo.Focus();
                goto FailsHere;
            }
            else if (Convert.ToString(dgvStockOut.Rows[0].Cells[GetEnum(gridColIndexes.CItemCode)].Value) == "")
            {
                bValidate = false;
                MessageBox.Show("No Items are Entered for Save. Please Enter the Item", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                goto FailsHere;
            }
            else if (cboAgent.SelectedIndex < 0)
            {
                bValidate = false;
                MessageBox.Show("Please select an agent.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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
                MessageBox.Show("Sales Rates are Lesser Than of PRate of the Item[" + dgvStockOut.Rows[Convert.ToInt32(sMsg[1])].Cells[GetEnum(gridColIndexes.CItemName)].Value.ToString() + "], Check the Values [" + sMsg[0].ToString() + "].", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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
                //if(Convert.ToDecimal(dgvStockOut.Rows[0].Cells[GetEnum(gridColIndexes.cPrate)].Value) == 0)
                for (int i = 0; i < dgvStockOut.Rows.Count; i++)
                {
                    if (dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.CItemCode)].Value != null)
                    {
                        bValidate = true;
                        if (Convert.ToDecimal(dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cSrate)].Value) == 0)
                        {
                            //MessageBox.Show("Purchase rate cannot be zero. Please provide purchase rate for the item !", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            //bValidate = false;
                            //goto FailsHere;
                        }
                        if (Convert.ToDecimal(dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cQty)].Value) == 0 && Convert.ToDecimal(dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cFree)].Value) == 0)
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


            if (Convert.ToDouble(lblBillAmount.Text) > 1000000000)
            {
                bValidate = false;
                MessageBox.Show("Software is unable to calculate higher values than 100000000. Kindly limit the values or correct if given by mistake", "Sales Value Calculation", MessageBoxButtons.OK,MessageBoxIcon.Error);
                lblBillAmount.Text = "000";
                goto FailsHere;
            }

            if (txtInvAutoNo.Text.Trim() != "")
            {
                if (iIDFromEditWindow == 0)
                {
                    dtInv = Comm.fnGetData("SELECT InvNo FROM tblSales WHERE vchtypeid=" + vchtypeID + " and LTRIM(RTRIM(InvNo)) = '" + txtInvAutoNo.Text.Trim() + "'").Tables[0];
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
                    if (txtSupplier.Enabled == true && txtSupplier.Visible == true)
                        txtSupplier.Focus();
                    else if (cboPayment.Enabled == true && cboPayment.Visible == true)
                        cboPayment.Focus();

                    goto FailsHere;
                }
            }
            //if (Convert.ToInt32(cboPayment.SelectedIndex) == 1 || Convert.ToInt32(cboPayment.SelectedIndex) == 2)
            if (Convert.ToInt32(cboPayment.SelectedIndex) == 1)
            {
                if (txtSupplier.Text == "" || dSupplierID == 0)
                {
                    bValidate = false;
                    MessageBox.Show("Please Choose Party for Credit Sales.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    txtSupplier.Focus();
                    goto FailsHere;
                }
            }
            else
            {
                for (int i = 0; i < dgvStockOut.Rows.Count; i++)
                {
                    if (iIDFromEditWindow == 0)
                    {
                        if (dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cBarCode)].Tag != null)
                        {
                            if (dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cBatchMode)].Value.ToString().Trim() != "2")
                            {
                                string sQuery = "Select ItemID,BatchCode From tblStock Where LTRIM(RTRIM(BatchCode)) = '" + dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cBarCode)].Tag.ToString() + "' AND ItemID <> " + Convert.ToDouble(dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cItemID)].Value.ToString()) + "";
                                DataTable dtBatch = Comm.fnGetData(sQuery).Tables[0];
                                if (dtBatch.Rows.Count > 0)
                                {
                                    bValidate = false;
                                    MessageBox.Show("This BatchCode " + dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cBarCode)].Tag.ToString() + "of Item [" + dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.CItemName)].Value.ToString() + "] is already Exist.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    break;
                                }
                            }
                        }
                    }
                    //Dipu on 19-May-2022 -------------------- >> Do Not Allow Net Amount is Greater than of CRate and CRate With Tax
                    if (Convert.ToDouble(dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cCrate)].Value) > 0)
                    {
                        bValidate = true;
                        if (Convert.ToDouble(dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cCrate)].Value) > (Convert.ToDouble(dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cNetAmount)].Value) / Convert.ToDouble(dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cQty)].Value)))
                        {
                            bValidate = false;
                            MessageBox.Show("Do not allow the Net Amount is Greater than of CRate or CRate With Tax. Check the Other Charges or Cost Factor are Correct !", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            goto FailsHere;
                            //break;
                        }
                        else if (Convert.ToDouble(dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cCRateWithTax)].Value) > (Convert.ToDouble(dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cNetAmount)].Value) / Convert.ToDouble(dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cQty)].Value)))
                        {
                            bValidate = false;
                            MessageBox.Show("Do not allow the Net Amount is Greater than of CRate or CRate With Tax. Check the Other Charges or Cost Factor are Correct !", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            goto FailsHere;
                            //break;
                        }
                        else if (Convert.ToDateTime(dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.CExpiry)].Value) <= Convert.ToDateTime(DateTime.Today))
                        {
                            bValidate = false;
                            MessageBox.Show("Do Not Allow the Previous Expiry Date !", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            goto FailsHere;
                            //break;
                        }
                    }
                }

                for (int j = 0; j < dgvStockOut.Rows.Count; j++)
                {
                    bValidate = true;
                    if (Convert.ToDecimal(dgvStockOut.Rows[j].Cells[GetEnum(gridColIndexes.cSrate)].Value) > Convert.ToDecimal(dgvStockOut.Rows[j].Cells[GetEnum(gridColIndexes.cMRP)].Value))
                    {
                        bValidate = false;
                        MessageBox.Show("Sales Rate Above MRP.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        goto FailsHere;
                        //break;
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
            for (i = 0; i < dgvStockOut.Rows.Count; i++)
            {
                if (dgvStockOut.Rows[0].Cells[GetEnum(gridColIndexes.CItemCode)].Value != null)
                {
                    if (dgvStockOut.Rows[0].Cells[GetEnum(gridColIndexes.CItemCode)].Value.ToString() != "")
                    {
                        if (Convert.ToDecimal(dgvStockOut.Rows[0].Cells[GetEnum(gridColIndexes.cSrate)].Value) > Convert.ToDecimal(dgvStockOut.Rows[0].Cells[GetEnum(gridColIndexes.cSRate1)].Value))
                            sData = sData + AppSettings.SRate1Name + " ,";
                        else if (Convert.ToDecimal(dgvStockOut.Rows[0].Cells[GetEnum(gridColIndexes.cSrate)].Value) > Convert.ToDecimal(dgvStockOut.Rows[0].Cells[GetEnum(gridColIndexes.cSRate2)].Value))
                        {
                            if (AppSettings.IsActiveSRate2 == true)
                                sData = sData + AppSettings.SRate2Name + " ,";
                        }
                        else if (Convert.ToDecimal(dgvStockOut.Rows[0].Cells[GetEnum(gridColIndexes.cSrate)].Value) > Convert.ToDecimal(dgvStockOut.Rows[0].Cells[GetEnum(gridColIndexes.cSRate3)].Value))
                        {
                            if (AppSettings.IsActiveSRate3 == true)
                                sData = sData + AppSettings.SRate3Name + " ,";
                        }
                        else if (Convert.ToDecimal(dgvStockOut.Rows[0].Cells[GetEnum(gridColIndexes.cSrate)].Value) > Convert.ToDecimal(dgvStockOut.Rows[0].Cells[GetEnum(gridColIndexes.cSRate4)].Value))
                        {
                            if (AppSettings.IsActiveSRate4 == true)
                                sData = sData + AppSettings.SRate4Name + " ,";
                        }
                        else if (Convert.ToDecimal(dgvStockOut.Rows[0].Cells[GetEnum(gridColIndexes.cSrate)].Value) > Convert.ToDecimal(dgvStockOut.Rows[0].Cells[GetEnum(gridColIndexes.cSRate5)].Value))
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
            #region "Sales Master (tblSales) ------------------------------- >>"

            if (txtcashDisper.Text == "") txtcashDisper.Text = "0";

            if (dSupplierID == 0)
            {
                if (Convert.ToInt32(cboPayment.SelectedIndex) == 0 || Convert.ToInt32(cboPayment.SelectedIndex) == 2)
                {
                    DataTable dtDefaultSupp = Comm.fnGetData("select top 1 LID,LName,LAliasName,Address,MobileNo,AccountGroupID from tblLedger WHERE LID=101 AND GroupName = 'CUSTOMER'").Tables[0];
                    if (dtDefaultSupp.Rows.Count > 0)
                    {
                        dSupplierID = Convert.ToDecimal(dtDefaultSupp.Rows[0]["LID"].ToString());
                        lblLID.Text = dSupplierID.ToString();
                        txtSupplier.Tag = dtDefaultSupp.Rows[0]["LAliasName"].ToString();
                        cboBType.SelectedIndex = 1;
                        FillSupplierForSerializeJsonUsingID(Convert.ToInt32(lblLID.Text));
                    }
                    else
                    {
                        lblLID.Text = "101";
                        dSupplierID = 101;
                        FillSupplierForSerializeJsonUsingID((int)dSupplierID);
                    }
                }
                else
                    txtSupplier.Tag = txtSupplier.Text;
            }
            else if (dSupplierID == 100 || dSupplierID == 101)
            {
                lblLID.Text = dSupplierID.ToString();
                cboBType.SelectedIndex = 1;
                FillSupplierForSerializeJsonUsingID((int)dSupplierID);
            }
            if (iIDFromEditWindow == 0)
            {
                clsJPMinfo.InvId = Comm.gfnGetNextSerialNo("tblSales", "InvId");
                txtInvAutoNo.Tag = clsJPMinfo.InvId;
                //clsJPMinfo.AutoNum = Convert.ToDecimal(Comm.gfnGetNextSerialNo("tblSales", "AutoNum", " VchTypeID=" + vchtypeID).ToString());
                clsJPMinfo.AutoNum = Convert.ToDecimal(txtInvAutoNo.Text.ToString());
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
            clsJPMinfo.MOP = Convert.ToString(cboPayment.SelectedItem);
            clsJPMinfo.TaxModeID = Convert.ToDecimal(cboTaxMode.SelectedValue);
            if (lblLID.Text == "") lblLID.Text = "0";
            clsJPMinfo.LedgerId = Convert.ToDecimal(lblLID.Text);
            clsJPMinfo.Party = txtSupplier.Text;
            clsJPMinfo.Discount = Convert.ToDecimal(txtDiscAmt.Text);
            clsJPMinfo.dSteadyBillDiscPerc = Convert.ToDecimal(dSteadyBillDiscPerc);
            clsJPMinfo.dSteadyBillDiscAmt = Convert.ToDecimal(dSteadyBillDiscAmt);
            
            clsJPMinfo.TaxAmt = Convert.ToDecimal(txtTaxAmt.Text);
            clsJPMinfo.GrossAmt = Convert.ToDecimal(txtGrossAmt.Text);
            clsJPMinfo.QtyTotal = Convert.ToDecimal(lblQtyTotal.Text);
            clsJPMinfo.FreeTotal = Convert.ToDecimal(lblFreeTotal.Text);
            clsJPMinfo.BillAmt = Convert.ToDecimal(lblBillAmount.Text);
            clsJPMinfo.CoolieTotal = Convert.ToDecimal(txtCoolie.Text);

            clsJPMinfo.Cancelled = 0;
            clsJPMinfo.OtherExpense = Convert.ToDecimal(txtOtherExp.Text);
            clsJPMinfo.SalesManID = Convert.ToDecimal(cboSalesStaff.SelectedValue);
            clsJPMinfo.Taxable = Convert.ToDecimal(txtTaxable.Text);
            clsJPMinfo.NonTaxable = Convert.ToDecimal(txtNonTaxable.Text);
            clsJPMinfo.ItemDiscountTotal = Convert.ToDecimal(txtItemDiscTot.Text);
            clsJPMinfo.RoundOff = Convert.ToDecimal(txtRoundOff.Text);
            clsJPMinfo.UserNarration = txtNarration.Text;
            clsJPMinfo.SortNumber = 0;
            clsJPMinfo.DiscPer = Convert.ToDecimal(txtDiscPerc.Text);
            clsJPMinfo.VchTypeID = vchtypeID;
            clsJPMinfo.CCID = Convert.ToDecimal(cboCostCentre.SelectedValue);
            clsJPMinfo.CurrencyID = 0;
            clsJPMinfo.PartyAddress = txtAddress1.Text;
            clsJPMinfo.UserID = Global.gblUserID;
            clsJPMinfo.AgentID = Convert.ToDecimal(cboAgent.SelectedValue);
            clsJPMinfo.CashDiscount = Convert.ToDecimal(txtCashDisc.Text);
            clsJPMinfo.DPerType_ManualCalc_Customer = 0;
            clsJPMinfo.NetAmount = Convert.ToDecimal(txtNetAmt.Text);
            clsJPMinfo.RefNo = txtReferencePrefix.Text;
            clsJPMinfo.CashPaid = 0;
            clsJPMinfo.CardPaid = 0;
            clsJPMinfo.blnWaitforAuthorisation = 0;
            clsJPMinfo.UserIDAuth = 0;
            clsJPMinfo.BillTime = DateTime.Now;
            clsJPMinfo.StateID = Convert.ToDecimal(cboState.SelectedValue);
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
            clsJPMinfo.partyCode = txtSupplier.Tag.ToString();
            clsJPMinfo.MobileNo = txtMobile.Text;
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
            clsJPMinfo.ReferenceAutoNO = txtReferenceAutoNo.Text;
            clsJPMinfo.CashDisPer = Convert.ToDecimal(txtcashDisper.Text);
            clsJPMinfo.CostFactor = Convert.ToDecimal(txtCostFactor.Text);
            clsJPMinfo.TenantID = Global.gblTenantID;
            clsPM.clsJsonPMInfo_ = clsJPMinfo;

            #endregion

            #region "Supplier Data (tblLedger) ----------------------------------- >>"

            clsJPMLedgerinfo.LID = dSupplierID;
            clsJPMLedgerinfo.LName = txtSupplier.Text;
            clsJPMLedgerinfo.LAliasName = txtSupplier.Tag.ToString();
            clsJPMLedgerinfo.GroupName = sArrLedger[GetEnumLedger(LedgerIndexes.GroupName)].ToString();
            clsJPMLedgerinfo.Type = sArrLedger[GetEnumLedger(LedgerIndexes.Type)].ToString();
            clsJPMLedgerinfo.OpBalance = Convert.ToDecimal(sArrLedger[GetEnumLedger(LedgerIndexes.OpBalance)]);
            clsJPMLedgerinfo.AppearIn = sArrLedger[GetEnumLedger(LedgerIndexes.AppearIn)].ToString();
            clsJPMLedgerinfo.Address = txtAddress1.Text;
            clsJPMLedgerinfo.CreditDays = sArrLedger[GetEnumLedger(LedgerIndexes.CreditDays)].ToString();
            clsJPMLedgerinfo.Phone = sArrLedger[GetEnumLedger(LedgerIndexes.Phone)].ToString();
            clsJPMLedgerinfo.TaxNo = txtTaxRegn.Text;
            clsJPMLedgerinfo.AccountGroupID = Convert.ToDecimal(sArrLedger[GetEnumLedger(LedgerIndexes.AccountGroupID)].ToString());
            clsJPMLedgerinfo.RouteID = Convert.ToDecimal(sArrLedger[GetEnumLedger(LedgerIndexes.RouteID)].ToString());
            clsJPMLedgerinfo.Area = sArrLedger[GetEnumLedger(LedgerIndexes.Area)].ToString();
            clsJPMLedgerinfo.Notes = sArrLedger[GetEnumLedger(LedgerIndexes.Notes)].ToString();
            clsJPMLedgerinfo.TargetAmt = Convert.ToDecimal(sArrLedger[GetEnumLedger(LedgerIndexes.TargetAmt)].ToString());
            clsJPMLedgerinfo.SMSSchID = Convert.ToDecimal(sArrLedger[GetEnumLedger(LedgerIndexes.SMSSchID)].ToString());
            clsJPMLedgerinfo.Email = sArrLedger[GetEnumLedger(LedgerIndexes.Email)].ToString();
            clsJPMLedgerinfo.MobileNo = txtMobile.Text;
            clsJPMLedgerinfo.DiscPer = Convert.ToDecimal(sArrLedger[GetEnumLedger(LedgerIndexes.DiscPer)].ToString());
            clsJPMLedgerinfo.InterestPer = Convert.ToDecimal(sArrLedger[GetEnumLedger(LedgerIndexes.InterestPer)].ToString());
            clsJPMLedgerinfo.DummyLName = sArrLedger[GetEnumLedger(LedgerIndexes.DummyLName)].ToString();
            clsJPMLedgerinfo.BlnBank = Convert.ToDecimal(sArrLedger[GetEnumLedger(LedgerIndexes.BlnBank)].ToString());
            clsJPMLedgerinfo.CurrencyID = Convert.ToDecimal(sArrLedger[GetEnumLedger(LedgerIndexes.CurrencyID)].ToString());
            clsJPMLedgerinfo.AreaID = Convert.ToDecimal(sArrLedger[GetEnumLedger(LedgerIndexes.AreaID)].ToString());
            clsJPMLedgerinfo.PLID = Convert.ToDecimal(sArrLedger[GetEnumLedger(LedgerIndexes.PLID)].ToString());
            clsJPMLedgerinfo.ActiveStatus = Convert.ToDecimal(sArrLedger[GetEnumLedger(LedgerIndexes.ActiveStatus)].ToString());
            clsJPMLedgerinfo.EmailAddress = sArrLedger[GetEnumLedger(LedgerIndexes.EmailAddress)].ToString();
            clsJPMLedgerinfo.EntryDate = Convert.ToDateTime(sArrLedger[GetEnumLedger(LedgerIndexes.EntryDate)].ToString());
            clsJPMLedgerinfo.blnBillWise = Convert.ToDecimal(sArrLedger[GetEnumLedger(LedgerIndexes.blnBillWise)].ToString());
            clsJPMLedgerinfo.CustomerCardID = Convert.ToDecimal(sArrLedger[GetEnumLedger(LedgerIndexes.CustomerCardID)].ToString());
            clsJPMLedgerinfo.TDSPer = Convert.ToDecimal(sArrLedger[GetEnumLedger(LedgerIndexes.TDSPer)].ToString());
            clsJPMLedgerinfo.DOB = Convert.ToDateTime(sArrLedger[GetEnumLedger(LedgerIndexes.DOB)].ToString());
            clsJPMLedgerinfo.StateID = Convert.ToDecimal(cboState.SelectedValue);
            clsJPMLedgerinfo.CCIDS = sArrLedger[GetEnumLedger(LedgerIndexes.CCIDS)].ToString();
            clsJPMLedgerinfo.CurrentBalance = Convert.ToDecimal(sArrLedger[GetEnumLedger(LedgerIndexes.CurrentBalance)].ToString());
            clsJPMLedgerinfo.LedgerName = sArrLedger[GetEnumLedger(LedgerIndexes.LedgerName)].ToString();
            clsJPMLedgerinfo.LedgerCode = sArrLedger[GetEnumLedger(LedgerIndexes.LedgerCode)].ToString();
            clsJPMLedgerinfo.BlnWallet = Convert.ToDecimal(sArrLedger[GetEnumLedger(LedgerIndexes.BlnWallet)].ToString());
            clsJPMLedgerinfo.blnCoupon = Convert.ToDecimal(sArrLedger[GetEnumLedger(LedgerIndexes.blnCoupon)].ToString());
            clsJPMLedgerinfo.TransComn = Convert.ToDecimal(sArrLedger[GetEnumLedger(LedgerIndexes.TransComn)].ToString());
            clsJPMLedgerinfo.BlnSmsWelcome = Convert.ToDecimal(sArrLedger[GetEnumLedger(LedgerIndexes.BlnSmsWelcome)].ToString());
            clsJPMLedgerinfo.DLNO = sArrLedger[GetEnumLedger(LedgerIndexes.DLNO)].ToString();
            clsJPMLedgerinfo.TDS = Convert.ToDecimal(sArrLedger[GetEnumLedger(LedgerIndexes.TDS)].ToString());
            clsJPMLedgerinfo.LedgerNameUnicode = sArrLedger[GetEnumLedger(LedgerIndexes.LedgerNameUnicode)].ToString();
            clsJPMLedgerinfo.LedgerAliasNameUnicode = sArrLedger[GetEnumLedger(LedgerIndexes.LedgerAliasNameUnicode)].ToString();
            clsJPMLedgerinfo.ContactPerson = sArrLedger[GetEnumLedger(LedgerIndexes.ContactPerson)].ToString();
            clsJPMLedgerinfo.TaxParameter = sArrLedger[GetEnumLedger(LedgerIndexes.TaxParameter)].ToString();
            clsJPMLedgerinfo.TaxParameterType = sArrLedger[GetEnumLedger(LedgerIndexes.TaxParameterType)].ToString();
            clsJPMLedgerinfo.HSNCODE = sArrLedger[GetEnumLedger(LedgerIndexes.HSNCODE)].ToString();
            clsJPMLedgerinfo.CGSTTaxPer = Convert.ToDecimal(sArrLedger[GetEnumLedger(LedgerIndexes.CGSTTaxPer)].ToString());
            clsJPMLedgerinfo.SGSTTaxPer = Convert.ToDecimal(sArrLedger[GetEnumLedger(LedgerIndexes.SGSTTaxPer)].ToString());
            clsJPMLedgerinfo.IGSTTaxPer = Convert.ToDecimal(sArrLedger[GetEnumLedger(LedgerIndexes.IGSTTaxPer)].ToString());
            clsJPMLedgerinfo.HSNID = Convert.ToDecimal(sArrLedger[GetEnumLedger(LedgerIndexes.HSNID)].ToString());
            clsJPMLedgerinfo.BankAccountNo = Convert.ToDecimal(sArrLedger[GetEnumLedger(LedgerIndexes.BankAccountNo)].ToString());
            clsJPMLedgerinfo.BankIFSCCode = sArrLedger[GetEnumLedger(LedgerIndexes.BankIFSCCode)].ToString();
            clsJPMLedgerinfo.BankNote = sArrLedger[GetEnumLedger(LedgerIndexes.BankNote)].ToString();
            clsJPMLedgerinfo.WhatsAppNo = Convert.ToDecimal(sArrLedger[GetEnumLedger(LedgerIndexes.WhatsAppNo)].ToString());
            //Dipu 21-03-2022 ------- >>
            //clsJPMLedgerinfo.SystemName = Global.gblSystemName;
            //clsJPMLedgerinfo.UserID = Global.gblUserID;
            //clsJPMLedgerinfo.LastUpdateDate = DateTime.Today;
            //clsJPMLedgerinfo.LastUpdateTime = DateTime.Now;
            clsJPMLedgerinfo.TenantID = Global.gblTenantID;
            clsJPMLedgerinfo.GSTType = Convert.ToString(cboBType.SelectedItem);
            clsJPMLedgerinfo.AgentID = Convert.ToDecimal(sArrLedger[GetEnumLedger(LedgerIndexes.AgentID)].ToString());
            clsPM.clsJsonPMLedgerInfo_ = clsJPMLedgerinfo;

            #endregion

            #region "TAX Mode (tblTaxMode) --------------------------------------- >>"

            string[] sArrTMod = GetTaxModeData(Convert.ToDecimal(cboTaxMode.SelectedValue));
            clsJPMTaxModinfo.TaxModeID = Convert.ToDecimal(cboTaxMode.SelectedValue);
            clsJPMTaxModinfo.TaxMode = cboTaxMode.SelectedItem.ToString();
            if (sArrTMod.Length > 0)
            {
                clsJPMTaxModinfo.CalculationID = Convert.ToInt32(sArrTMod[0].ToString());
                clsJPMTaxModinfo.SortNo = Convert.ToInt32(sArrTMod[1].ToString());
                clsJPMTaxModinfo.ActiveStatus = Convert.ToInt32(sArrTMod[1].ToString());
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

            string[] sArrAgent = GetAgentData(Convert.ToDecimal(cboAgent.SelectedValue));
            clsJPMAgentinfo.AgentID = Convert.ToDecimal(cboAgent.SelectedValue);
            clsJPMAgentinfo.AgentCode = sArrAgent[GetEnumAgent(AgentIndexes.AgentCode)];
            clsJPMAgentinfo.AgentName = cboAgent.SelectedItem.ToString();
            clsJPMAgentinfo.Area = sArrAgent[GetEnumAgent(AgentIndexes.Area)];
            clsJPMAgentinfo.Commission = Convert.ToDecimal(sArrAgent[GetEnumAgent(AgentIndexes.Commission)]);
            clsJPMAgentinfo.blnPOstAccounts = Convert.ToInt32(sArrAgent[GetEnumAgent(AgentIndexes.blnPOstAccounts)]);
            clsJPMAgentinfo.ADDRESS = sArrAgent[GetEnumAgent(AgentIndexes.ADDRESS)];
            clsJPMAgentinfo.LOCATION = sArrAgent[GetEnumAgent(AgentIndexes.LOCATION)];
            clsJPMAgentinfo.PHONE = sArrAgent[GetEnumAgent(AgentIndexes.PHONE)];
            clsJPMAgentinfo.WEBSITE = sArrAgent[GetEnumAgent(AgentIndexes.WEBSITE)];
            clsJPMAgentinfo.EMAIL = sArrAgent[GetEnumAgent(AgentIndexes.EMAIL)];
            clsJPMAgentinfo.BLNROOMRENT = Convert.ToInt32(sArrAgent[GetEnumAgent(AgentIndexes.BLNROOMRENT)]);
            clsJPMAgentinfo.BLNSERVICES = Convert.ToInt32(sArrAgent[GetEnumAgent(AgentIndexes.BLNSERVICES)]);
            clsJPMAgentinfo.blnItemwiseCommission = Convert.ToInt32(sArrAgent[GetEnumAgent(AgentIndexes.blnItemwiseCommission)]);
            clsJPMAgentinfo.AgentDiscount = Convert.ToDecimal(sArrAgent[GetEnumAgent(AgentIndexes.AgentDiscount)]);

            if (sArrAgent[GetEnumAgent(AgentIndexes.LID)] == "") sArrAgent[GetEnumAgent(AgentIndexes.LID)] = "0";
            clsJPMAgentinfo.LID = Convert.ToInt32(sArrAgent[GetEnumAgent(AgentIndexes.LID)]);

            //Dipu 21-03-2022 ------- >>
            //clsJPMAgentinfo.SystemName = Global.gblSystemName;
            //clsJPMAgentinfo.UserID = Global.gblUserID;
            //clsJPMAgentinfo.LastUpdateDate = DateTime.Today; ;
            //clsJPMAgentinfo.LastUpdateTime = DateTime.Now;
            clsJPMAgentinfo.TenantID = Global.gblTenantID;
            clsPM.clsJsonPMAgentInfo_ = clsJPMAgentinfo;

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

            #region "State Master (tblStates) ------------------------------------ >>"

            string[] sArrState = GetStateData(Convert.ToDecimal(cboState.SelectedValue));
            clsJPMStateinfo.StateId = Convert.ToDecimal(cboState.SelectedValue);
            clsJPMStateinfo.StateCode = sArrState[0].ToString();
            clsJPMStateinfo.State = cboState.SelectedItem.ToString();
            clsJPMStateinfo.StateType = sArrState[1].ToString();
            clsJPMStateinfo.Country = sArrState[2].ToString();
            clsJPMStateinfo.CountryID = Convert.ToDecimal(sArrState[3].ToString());
            //Dipu 21-03-2022 ------- >>
            //clsJPMStateinfo.SystemName = Global.gblSystemName;
            //clsJPMStateinfo.UserID = Global.gblUserID;
            //clsJPMStateinfo.LastUpdateDate = DateTime.Today;
            //clsJPMStateinfo.LastUpdateTime = DateTime.Now;
            clsJPMStateinfo.TenantID = Global.gblTenantID;
            clsPM.clsJsonPMStateInfo_ = clsJPMStateinfo;

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

            #region "Sales Details (tblSalesItem) -------------------------- >>"
            DataTable dtBatchUniq = new DataTable();
            List<clsJsonPDetailsInfo> lstJPDinfo = new List<clsJsonPDetailsInfo>();
            for (int i = 0; i < dgvStockOut.Rows.Count; i++)
            {
                if (dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.CItemCode)].Value != null)
                {
                    if (dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.CItemCode)].Value.ToString() != "")
                    {
                        clsJPDinfo = new clsJsonPDetailsInfo();

                        if (dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cBarCode)].Tag == null) dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cBarCode)].Tag = "";
                        if (dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cBarCode)].Tag.ToString().ToUpper() == "<AUTO BARCODE>")
                            dtBatchUniq = Comm.fnGetData("EXEC UspGetBatchCodeWhenAutoBarcode " + Convert.ToDecimal(dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cItemID)].Value) + ",'" + dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cBarCode)].Tag.ToString() + "',''," + Convert.ToDecimal(dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cMRP)].Value) + ",'" + Convert.ToDateTime(dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.CExpiry)].Value).ToString("dd-MMM-yyyy") + "'," + Global.gblTenantID + "").Tables[0];

                        //clsJPDinfo.InvID = Convert.ToDecimal(txtInvAutoNo.Text);
                        clsJPDinfo.InvID = Convert.ToDecimal(txtInvAutoNo.Tag);
                        clsJPDinfo.ItemId = Convert.ToDecimal(dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cItemID)].Value);
                        clsJPDinfo.Qty = Convert.ToDecimal(dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cQty)].Value);
                        clsJPDinfo.Rate = Convert.ToDecimal(dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cSrate)].Value);
                        clsJPDinfo.UnitId = Convert.ToDecimal(dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.CUnit)].Tag);
                        clsJPDinfo.Batch = "";
                        clsJPDinfo.TaxPer = Convert.ToDecimal(dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.ctaxPer)].Value);
                        clsJPDinfo.TaxAmount = Convert.ToDecimal(dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.ctax)].Value);
                        clsJPDinfo.Discount = Convert.ToDecimal(dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cDiscAmount)].Value);

                        clsJPDinfo.MRP = Convert.ToDecimal(dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cMRP)].Value);
                        clsJPDinfo.SlNo = Convert.ToDecimal(dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cSlNo)].Value);
                        clsJPDinfo.Prate = Convert.ToDecimal(dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cSrate)].Value);
                        clsJPDinfo.Free = Convert.ToDecimal(dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cFree)].Value);
                        clsJPDinfo.SerialNos = "";
                        clsJPDinfo.ItemDiscount = Convert.ToDecimal(dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cDiscAmount)].Value);

                        if (dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cBarCode)].Tag != null && dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cBarCode)].Tag.ToString() != "")
                        {
                            //if (dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cBarCode)].Tag.ToString() != "0")
                            clsJPDinfo.BatchCode = dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cBarCode)].Tag.ToString();
                            //else
                            //    clsJPDinfo.BatchCode = dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cBarCode)].Value.ToString();
                        }
                        else
                            clsJPDinfo.BatchCode = dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cBarCode)].Value.ToString();

                        clsJPDinfo.iCessOnTax = 0;
                        clsJPDinfo.blnCessOnTax = 0;
                        clsJPDinfo.Expiry = Convert.ToDateTime(dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.CExpiry)].Value);
                        clsJPDinfo.ItemDiscountPer = Convert.ToDecimal(dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cDiscPer)].Value);
                        clsJPDinfo.RateInclusive = Convert.ToDecimal(dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cRateinclusive)].Value);
                        clsJPDinfo.ITaxableAmount = Convert.ToDecimal(dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.ctaxable)].Value);
                        clsJPDinfo.INetAmount = Convert.ToDecimal(dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cNetAmount)].Value);
                        clsJPDinfo.CGSTTaxPer = Convert.ToDecimal(dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cCGST)].Tag);
                        clsJPDinfo.CGSTTaxAmt = Convert.ToDecimal(dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cCGST)].Value);
                        clsJPDinfo.SGSTTaxPer = Convert.ToDecimal(dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cSGST)].Tag);
                        clsJPDinfo.SGSTTaxAmt = Convert.ToDecimal(dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cSGST)].Value);
                        clsJPDinfo.IGSTTaxPer = Convert.ToDecimal(dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cIGST)].Tag);
                        clsJPDinfo.IGSTTaxAmt = Convert.ToDecimal(dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cIGST)].Value);
                        clsJPDinfo.iRateDiscPer = Convert.ToDecimal(dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cDiscPer)].Value);
                        clsJPDinfo.iRateDiscount = Convert.ToDecimal(dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cDiscAmount)].Value);

                        //string[] strBatchUniq;

                        clsJPDinfo.BatchUnique = dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cBarCode)].Value.ToString();

                        //clsJPDinfo.BatchUnique = dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cBarCode)].Tag.ToString();
                        //if (dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cBarCode)].Tag.ToString().ToUpper() == "<AUTO BARCODE>")
                        //{
                        //    if (dtBatchUniq.Rows.Count > 0)
                        //        clsJPDinfo.BatchUnique = dtBatchUniq.Rows[0]["BatchUniq"].ToString();
                        //    else
                        //        clsJPDinfo.BatchUnique = dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cBarCode)].Tag.ToString();
                        //}
                        //else
                        //{
                        //    strBatchUniq = dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cBarCode)].Tag.ToString().Split('@');
                        //    if (strBatchUniq.Length > 0)
                        //    {
                        //        if (strBatchUniq.Length == 2)
                        //        {
                        //            if (Convert.ToDecimal(dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cMRP)].Value) != Convert.ToDecimal(strBatchUniq[1].ToString()))
                        //            {
                        //                clsJPDinfo.BatchUnique = strBatchUniq[0].ToString() + "@" + Convert.ToDecimal(dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cMRP)].Value).ToString(AppSettings.CurrDecimalFormat);
                        //            }
                        //            else
                        //                clsJPDinfo.BatchUnique = dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cBarCode)].Tag.ToString();
                        //        }
                        //        else if (strBatchUniq.Length == 3)
                        //        {
                        //            if (Convert.ToDecimal(dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cMRP)].Value) != Convert.ToDecimal(strBatchUniq[1].ToString()))
                        //            {
                        //                clsJPDinfo.BatchUnique = strBatchUniq[0].ToString() + "@" + Convert.ToDecimal(dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cMRP)].Value).ToString(AppSettings.CurrDecimalFormat) + "@" + Convert.ToDateTime(dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.CExpiry)].Value).ToString("dd-MM-yy").Replace("-", "");
                        //            }
                        //            else
                        //                clsJPDinfo.BatchUnique = dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cBarCode)].Tag.ToString();
                        //        }
                        //        else
                        //            clsJPDinfo.BatchUnique = dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cBarCode)].Tag.ToString();
                        //    }
                        //    else
                        //    {
                        //        clsJPDinfo.BatchUnique = dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cBarCode)].Tag.ToString();
                        //    }
                        //}

                        clsJPDinfo.blnQtyIN = 0;
                        clsJPDinfo.CRate = Convert.ToDecimal(dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cCrate)].Value);
                        clsJPDinfo.CRate = Convert.ToDecimal(dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cCRateWithTax)].Value);
                        clsJPDinfo.Unit = dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.CUnit)].Value.ToString();
                        clsJPDinfo.ItemStockID = 0;
                        clsJPDinfo.IcessPercent = Convert.ToDecimal(dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cCCessPer)].Value);
                        clsJPDinfo.IcessAmt = 0;
                        clsJPDinfo.IQtyCompCessPer = 0;
                        clsJPDinfo.IQtyCompCessAmt = Convert.ToDecimal(dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cCCompCessQty)].Value);
                        clsJPDinfo.StockMRP = 0;
                        clsJPDinfo.InonTaxableAmount = 0;
                        clsJPDinfo.IAgentCommPercent = 0;
                        clsJPDinfo.BlnDelete = 0;
                        clsJPDinfo.Id = Convert.ToDecimal(dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cID)].Value);
                        clsJPDinfo.StrOfferDetails = "";
                        clsJPDinfo.BlnOfferItem = 0;
                        clsJPDinfo.BalQty = 0;
                        clsJPDinfo.GrossAmount = Convert.ToDecimal(dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cGrossAmt)].Value);
                        clsJPDinfo.iFloodCessPer = Convert.ToDecimal(dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cFloodCessPer)].Value);
                        clsJPDinfo.iFloodCessAmt = Convert.ToDecimal(dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cFloodCessAmt)].Value);
                        clsJPDinfo.Srate1 = Convert.ToDecimal(dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cSRate1)].Value);
                        clsJPDinfo.Srate2 = Convert.ToDecimal(dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cSRate2)].Value);
                        clsJPDinfo.Srate3 = Convert.ToDecimal(dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cSRate3)].Value);
                        clsJPDinfo.Srate4 = Convert.ToDecimal(dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cSRate4)].Value);
                        clsJPDinfo.Srate5 = Convert.ToDecimal(dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cSRate5)].Value);
                        clsJPDinfo.Costrate = Convert.ToDecimal(dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cCrate)].Value);
                        clsJPDinfo.CostValue = 0;
                        clsJPDinfo.Profit = 0;
                        clsJPDinfo.ProfitPer = 0;
                        clsJPDinfo.DiscMode = 0;
                        clsJPDinfo.Srate1Per = Convert.ToDecimal(dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cSRate1Per)].Value);
                        clsJPDinfo.Srate2Per = Convert.ToDecimal(dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cSRate2Per)].Value);
                        clsJPDinfo.Srate3Per = Convert.ToDecimal(dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cSRate3Per)].Value);
                        clsJPDinfo.Srate4Per = Convert.ToDecimal(dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cSRate4Per)].Value);
                        clsJPDinfo.Srate5Per = Convert.ToDecimal(dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cSRate5Per)].Value);
                        lstJPDinfo.Add(clsJPDinfo);
                    }
                }
            }
            clsPM.clsJsonPDetailsInfoList_ = lstJPDinfo;

            #endregion

            #region "Item Unit Details ------------------------------------------- >>"

            List<clsJsonPDUnitinfo> lstJPDUnit = new List<clsJsonPDUnitinfo>();
            for (int j = 0; j < dgvStockOut.Rows.Count; j++)
            {
                if (dgvStockOut.Rows[j].Cells[GetEnum(gridColIndexes.CItemCode)].Value != null)
                {
                    if (dgvStockOut.Rows[j].Cells[GetEnum(gridColIndexes.CItemCode)].Value.ToString() != "")
                    {
                        DataTable dtUnit = new DataTable();
                        clsJPDUnitinfo = new clsJsonPDUnitinfo();
                        clsJPDUnitinfo.UnitID = Convert.ToDecimal(dgvStockOut.Rows[j].Cells[GetEnum(gridColIndexes.CUnit)].Tag);
                        clsJPDUnitinfo.UnitName = dgvStockOut.Rows[j].Cells[GetEnum(gridColIndexes.CUnit)].Value.ToString();
                        //dipu on 20-Apr-2022 ----->>
                        dtUnit = Comm.fnGetData("SELECT UnitShortName FROM tblUnit WHERE UnitID = " + Convert.ToDecimal(dgvStockOut.Rows[j].Cells[GetEnum(gridColIndexes.CUnit)].Tag) + "").Tables[0];
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
            for (int j = 0; j < dgvStockOut.Rows.Count; j++)
            {
                if (dgvStockOut.Rows[j].Cells[GetEnum(gridColIndexes.CItemCode)].Value != null)
                {
                    if (dgvStockOut.Rows[j].Cells[GetEnum(gridColIndexes.CItemCode)].Value.ToString() != "")
                    {
                        clsJPDIteminfo = new clsJsonPDIteminfo();
                        string[] sArrItm = GetItemDetails(Convert.ToDecimal(dgvStockOut.Rows[j].Cells[GetEnum(gridColIndexes.cItemID)].Value));
                        clsJPDIteminfo.ItemID = Convert.ToDecimal(dgvStockOut.Rows[j].Cells[GetEnum(gridColIndexes.cItemID)].Value);
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
            clsJSonRepacking clsRepacking = JsonConvert.DeserializeObject<clsJSonRepacking>(sToDeSerialize);

            txtPrefix.Text = clsVchType.TransactionPrefix;
            txtInvAutoNo.Text = Convert.ToString(clsRepacking.clsJsonPMInfo_.InvNo);
            txtInvAutoNo.Tag = Convert.ToDouble(clsRepacking.clsJsonPMInfo_.InvId);
            txtReferenceAutoNo.Tag = Convert.ToDouble(clsRepacking.clsJsonPMInfo_.AutoNum);
            dtpInvDate.Text = Convert.ToString(clsRepacking.clsJsonPMInfo_.InvDate);
            dtpEffective.Text = Convert.ToString(clsRepacking.clsJsonPMInfo_.EffectiveDate);
            txtReferencePrefix.Text = clsRepacking.clsJsonPMInfo_.RefNo;
            txtReferenceAutoNo.Text = Convert.ToString(clsRepacking.clsJsonPMInfo_.ReferenceAutoNO);
            if (clsRepacking.clsJsonPMInfo_.MOP.ToUpper() == "CASH")
                cboPayment.SelectedIndex = 0;
            else if (clsRepacking.clsJsonPMInfo_.MOP.ToUpper() == "CREDIT")
                cboPayment.SelectedIndex = 1;
            else if (clsRepacking.clsJsonPMInfo_.MOP.ToUpper() == "BOTH")
                cboPayment.SelectedIndex = 2;
            else if (clsRepacking.clsJsonPMInfo_.MOP.ToUpper() == "CASH DESK")
                cboPayment.SelectedIndex = 3;

            cboTaxMode.SelectedValue = clsRepacking.clsJsonPMTaxmodeInfo_.TaxModeID;
            cboCostCentre.SelectedValue = clsRepacking.clsJsonPMCCentreInfo_.CCID;
            cboSalesStaff.SelectedValue = clsRepacking.clsJsonPMEmployeeInfo_.EmpID;
            cboAgent.SelectedValue = clsRepacking.clsJsonPMAgentInfo_.AgentID;
            GetAgentDiscountAsperVoucherType();
            cboState.SelectedValue = clsRepacking.clsJsonPMStateInfo_.StateId;

            if (clsRepacking.clsJsonPMLedgerInfo_.LName.ToUpper() == "" || clsRepacking.clsJsonPMLedgerInfo_.LName.ToUpper() == "<GENERAL SUPPLIER>")
            {
                this.txtSupplier.TextChanged -= this.txtSupplier_TextChanged;
                txtSupplier.Text = "";
                this.txtSupplier.TextChanged += this.txtSupplier_TextChanged;
                txtMobile.Text = "";
                txtTaxRegn.Text = "";
                cboState.SelectedIndex = -1;
                cboBType.SelectedIndex = -1;
                txtAddress1.Text = "";
                dSupplierID = clsRepacking.clsJsonPMLedgerInfo_.LID;
                lblLID.Text = dSupplierID.ToString();
                txtSupplier.Tag = clsRepacking.clsJsonPMLedgerInfo_.LAliasName;
            }
            else
            {
                this.txtSupplier.TextChanged -= this.txtSupplier_TextChanged;
                txtSupplier.Text = clsRepacking.clsJsonPMLedgerInfo_.LName;
                this.txtSupplier.TextChanged += this.txtSupplier_TextChanged;
                txtMobile.Text = clsRepacking.clsJsonPMLedgerInfo_.MobileNo;
                txtTaxRegn.Text = clsRepacking.clsJsonPMLedgerInfo_.TaxNo;
                cboState.SelectedValue = clsRepacking.clsJsonPMLedgerInfo_.StateID;
                cboBType.SelectedItem = clsRepacking.clsJsonPMLedgerInfo_.GSTType;
                txtAddress1.Text = clsRepacking.clsJsonPMLedgerInfo_.Address;
                dSupplierID = clsRepacking.clsJsonPMLedgerInfo_.LID;
                lblLID.Text = dSupplierID.ToString();
                txtSupplier.Tag = clsRepacking.clsJsonPMLedgerInfo_.LAliasName;
                FillSupplierForSerializeJsonUsingID(Convert.ToInt32(dSupplierID));
            }

            txtGrossAmt.Text = Comm.chkChangeValuetoZero(Convert.ToString(clsRepacking.clsJsonPMInfo_.GrossAmt));
            lblQtyTotal.Text = Comm.chkChangeValuetoZero(Convert.ToString(clsRepacking.clsJsonPMInfo_.QtyTotal));
            lblFreeTotal.Text = Comm.chkChangeValuetoZero(Convert.ToString(clsRepacking.clsJsonPMInfo_.FreeTotal));
            txtItemDiscTot.Text = Comm.chkChangeValuetoZero(Convert.ToString(clsRepacking.clsJsonPMInfo_.ItemDiscountTotal));

            dSteadyBillDiscPerc = Comm.ToDecimal(Comm.chkChangeValuetoZero(Convert.ToString(clsRepacking.clsJsonPMInfo_.dSteadyBillDiscPerc)));
            dSteadyBillDiscAmt = Comm.ToDecimal(Comm.chkChangeValuetoZero(Convert.ToString(clsRepacking.clsJsonPMInfo_.dSteadyBillDiscAmt)));

            this.txtDiscPerc.TextChanged -= this.txtDiscPerc_TextChanged;
            txtDiscPerc.Text = Comm.chkChangeValuetoZero(Convert.ToString(clsRepacking.clsJsonPMInfo_.DiscPer));
            this.txtDiscPerc.TextChanged += this.txtDiscPerc_TextChanged;

            txtDiscAmt.Text = Comm.chkChangeValuetoZero(Convert.ToString(clsRepacking.clsJsonPMInfo_.Discount));

            //Comm.chkChangeValuetoZero(Convert.ToString(clsRepacking.clsJsonPMInfo_.Discount));

            txtTaxable.Text = Comm.chkChangeValuetoZero(Convert.ToString(clsRepacking.clsJsonPMInfo_.Taxable));
            txtNonTaxable.Text = Comm.chkChangeValuetoZero(Convert.ToString(clsRepacking.clsJsonPMInfo_.NonTaxable));
            txtTaxAmt.Text = Comm.chkChangeValuetoZero(Convert.ToString(clsRepacking.clsJsonPMInfo_.TaxAmt));

            txtOtherExp.Text = Comm.chkChangeValuetoZero(Convert.ToString(clsRepacking.clsJsonPMInfo_.OtherExpense));
            txtNetAmt.Text = Comm.chkChangeValuetoZero(Convert.ToString(clsRepacking.clsJsonPMInfo_.NetAmount));
            txtCashDisc.Text = Comm.chkChangeValuetoZero(Convert.ToString(clsRepacking.clsJsonPMInfo_.CashDiscount));
            txtRoundOff.Text = Comm.chkChangeValuetoZero(Convert.ToString(clsRepacking.clsJsonPMInfo_.RoundOff));
            txtNarration.Text = Convert.ToString(clsRepacking.clsJsonPMInfo_.UserNarration);
            lblBillAmount.Text = Comm.chkChangeValuetoZero(Convert.ToString(clsRepacking.clsJsonPMInfo_.BillAmt));

            DataTable dtGetPurDetail = clsRepacking.clsJsonPDetailsInfoList_.ToDataTable();
            DataTable dtItemFrmJson = clsRepacking.clsJsonPDIteminfoList_.ToDataTable();
            DataTable dtUnitFrmJson = clsRepacking.clsJsonPDUnitinfoList_.ToDataTable();
            if (dtGetPurDetail.Rows.Count > 0)
            {
                sqlControl rs = new sqlControl();

                AddColumnsToGrid();
                for (int i = 0; i < dtGetPurDetail.Rows.Count; i++)
                {
                    dgvStockOut.Rows.Add();

                    rs.Open("Select ItemCode,ItemName From tblItemMaster Where ItemID=" + dtGetPurDetail.Rows[i]["ItemId"].ToString());
                    if (!rs.eof())
                    {
                        dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.CItemCode)].Value = rs.fields("itemcode");
                        dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.CItemName)].Value = rs.fields("ItemName");
                    }

                    SetValue(GetEnum(gridColIndexes.cSlNo), i, (i + 1).ToString());
                    dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cID)].Value = dtGetPurDetail.Rows[i]["Id"].ToString();
                    dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.CUnit)].Value = dtUnitFrmJson.Rows[i]["UnitName"].ToString();
                    dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.CUnit)].Tag = dtGetPurDetail.Rows[i]["UnitId"].ToString();
                    dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cBarCode)].Value = dtGetPurDetail.Rows[i]["BatchCode"].ToString();
                    dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cBarCode)].Tag = dtGetPurDetail.Rows[i]["BatchUnique"].ToString();
                    dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.CExpiry)].Value = Convert.ToDateTime(dtGetPurDetail.Rows[i]["Expiry"]).ToString("dd-MMM-yyyy");
                    dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cMRP)].Value = Comm.FormatValue(Convert.ToDouble(dtGetPurDetail.Rows[i]["MRP"].ToString()), true);
                    dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cSrate)].Value = Comm.FormatValue(Convert.ToDouble(dtGetPurDetail.Rows[i]["Rate"].ToString()), true);
                    this.dgvStockOut.Columns[GetEnum(gridColIndexes.cQty)].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cQty)].Value = Comm.FormatValue(Convert.ToDouble(dtGetPurDetail.Rows[i]["Qty"].ToString()), false);
                    this.dgvStockOut.Columns[GetEnum(gridColIndexes.cFree)].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cFree)].Value = Comm.FormatValue(Convert.ToDouble(dtGetPurDetail.Rows[i]["Free"].ToString()), false);

                    dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cSRate1Per)].Value = Comm.FormatValue(Convert.ToDouble(dtGetPurDetail.Rows[i]["Srate1Per"].ToString()), true);
                    dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cSRate1)].Value = Comm.FormatValue(Convert.ToDouble(dtGetPurDetail.Rows[i]["Srate1"].ToString()), true);
                    dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cSRate2Per)].Value = Comm.FormatValue(Convert.ToDouble(dtGetPurDetail.Rows[i]["Srate2Per"].ToString()), true);
                    dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cSRate2)].Value = Comm.FormatValue(Convert.ToDouble(dtGetPurDetail.Rows[i]["Srate2"].ToString()), true);
                    dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cSRate3Per)].Value = Comm.FormatValue(Convert.ToDouble(dtGetPurDetail.Rows[i]["Srate3Per"].ToString()), true);
                    dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cSRate3)].Value = Comm.FormatValue(Convert.ToDouble(dtGetPurDetail.Rows[i]["Srate3"].ToString()), true);
                    dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cSRate4Per)].Value = Comm.FormatValue(Convert.ToDouble(dtGetPurDetail.Rows[i]["Srate4Per"].ToString()), true);
                    dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cSRate4)].Value = Comm.FormatValue(Convert.ToDouble(dtGetPurDetail.Rows[i]["Srate4"].ToString()), true);
                    dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cSRate5Per)].Value = Comm.FormatValue(Convert.ToDouble(dtGetPurDetail.Rows[i]["Srate5Per"].ToString()), true);
                    dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cSRate5)].Value = Comm.FormatValue(Convert.ToDouble(dtGetPurDetail.Rows[i]["Srate5"].ToString()), true);

                    dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cGrossAmt)].Value = Comm.FormatValue(Convert.ToDouble(dtGetPurDetail.Rows[i]["GrossAmount"].ToString()), true);
                    dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cDiscPer)].Value = Comm.FormatValue(Convert.ToDouble(dtGetPurDetail.Rows[i]["ItemDiscountPer"].ToString()), true);
                    this.dgvStockOut.Columns[GetEnum(gridColIndexes.cDiscPer)].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cDiscAmount)].Value = Comm.FormatValue(Convert.ToDouble(dtGetPurDetail.Rows[i]["ItemDiscount"].ToString()), true);
                    dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cBillDisc)].Value = Comm.FormatValue(Convert.ToDouble(dtGetPurDetail.Rows[i]["Discount"].ToString()), true);
                    dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cCrate)].Value = Comm.FormatValue(Convert.ToDouble(dtGetPurDetail.Rows[i]["CRate"].ToString()), true);
                    dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cItemID)].Value = dtGetPurDetail.Rows[i]["ItemId"].ToString();
                    dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.ctaxPer)].Value = Comm.FormatValue(Convert.ToDouble(dtGetPurDetail.Rows[i]["TaxPer"].ToString()), true);
                    this.dgvStockOut.Columns[GetEnum(gridColIndexes.ctaxPer)].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.ctax)].Value = Comm.FormatValue(Convert.ToDouble(dtGetPurDetail.Rows[i]["TaxAmount"].ToString()), true);
                    dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cIGST)].Tag = Comm.FormatValue(Convert.ToDouble(dtGetPurDetail.Rows[i]["IGSTTaxPer"].ToString()), true);
                    dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cIGST)].Value = Comm.FormatValue(Convert.ToDouble(dtGetPurDetail.Rows[i]["IGSTTaxAmt"].ToString()), true);
                    dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cSGST)].Tag = Comm.FormatValue(Convert.ToDouble(dtGetPurDetail.Rows[i]["CGSTTaxPer"].ToString()), true);
                    dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cSGST)].Value = Comm.FormatValue(Convert.ToDouble(dtGetPurDetail.Rows[i]["CGSTTaxAmt"].ToString()), true);
                    dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cCGST)].Tag = Comm.FormatValue(Convert.ToDouble(dtGetPurDetail.Rows[i]["SGSTTaxPer"].ToString()), true);
                    dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cCGST)].Value = Comm.FormatValue(Convert.ToDouble(dtGetPurDetail.Rows[i]["SGSTTaxAmt"].ToString()), true);
                    dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cNetAmount)].Value = Comm.FormatValue(Convert.ToDouble(dtGetPurDetail.Rows[i]["INetAmount"].ToString()), true);
                    dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cNonTaxable)].Value = Comm.FormatValue(Convert.ToDouble(dtGetPurDetail.Rows[i]["InonTaxableAmount"].ToString()), true);
                    this.dgvStockOut.Columns[GetEnum(gridColIndexes.cNonTaxable)].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cCCessPer)].Value = Comm.FormatValue(Convert.ToDouble(dtGetPurDetail.Rows[i]["IcessPercent"].ToString()), true);
                    this.dgvStockOut.Columns[GetEnum(gridColIndexes.cCCessPer)].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cCCompCessQty)].Value = Comm.FormatValue(Convert.ToDouble(dtGetPurDetail.Rows[i]["IQtyCompCessAmt"].ToString()), true);
                    this.dgvStockOut.Columns[GetEnum(gridColIndexes.cCCompCessQty)].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cFloodCessPer)].Value = Comm.FormatValue(Convert.ToDouble(dtGetPurDetail.Rows[i]["iFloodCessPer"].ToString()), true);
                    this.dgvStockOut.Columns[GetEnum(gridColIndexes.cFloodCessPer)].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cFloodCessAmt)].Value = Comm.FormatValue(Convert.ToDouble(dtGetPurDetail.Rows[i]["iFloodCessAmt"].ToString()), true);
                    this.dgvStockOut.Columns[GetEnum(gridColIndexes.cFloodCessAmt)].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cStockMRP)].Value = Comm.FormatValue(Convert.ToDouble(dtGetPurDetail.Rows[i]["StockMRP"].ToString()), true);
                    this.dgvStockOut.Columns[GetEnum(gridColIndexes.cStockMRP)].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cAgentCommPer)].Value = Comm.FormatValue(Convert.ToDouble(dtGetPurDetail.Rows[i]["IAgentCommPercent"].ToString()), true);
                    this.dgvStockOut.Columns[GetEnum(gridColIndexes.cAgentCommPer)].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cBlnOfferItem)].Value = dtGetPurDetail.Rows[i]["BlnOfferItem"].ToString();
                    dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cStrOfferDetails)].Value = dtGetPurDetail.Rows[i]["StrOfferDetails"].ToString();
                    dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cBatchMode)].Value = dtItemFrmJson.Rows[i]["BatchMode"].ToString();

                    dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cCoolie)].Value = dtItemFrmJson.Rows[i]["Coolie"].ToString();

                    if (Convert.ToDouble(dtGetPurDetail.Rows[i]["RateInclusive"].ToString()) == 1)
                        dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cRateinclusive)].Value = true;
                    else
                        dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cRateinclusive)].Value = false;

                    this.dgvStockOut.Columns[i].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                }

                if (Convert.ToInt32(AppSettings.StateCode) != Convert.ToInt32(cboState.SelectedValue))
                {
                    dgvStockOut.Columns[GetEnum(gridColIndexes.cCGST)].Visible = false;
                    dgvStockOut.Columns[GetEnum(gridColIndexes.cSGST)].Visible = false;
                    dgvStockOut.Columns[GetEnum(gridColIndexes.cIGST)].Visible = true;
                }
                else
                {
                    dgvStockOut.Columns[GetEnum(gridColIndexes.cCGST)].Visible = true;
                    dgvStockOut.Columns[GetEnum(gridColIndexes.cSGST)].Visible = true;
                    dgvStockOut.Columns[GetEnum(gridColIndexes.cIGST)].Visible = false;
                }
                CalcTotal();
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
        //                    maxid = Comm.gfnGetNextSerialNo("tblSales", "InvId");
        //                }
        //                else
        //                {
        //                    maxid = iIDFromEditWindow;

        //                }

        //                string sql = "insert into tblSales(InvId,LedgerId,Prefix,AutoNum, Party,PartyAddress,PARTYGSTIN,StateID,BillType,PriceListID,CCID,AGENTID,InvNo,InvDate,BillTime,MOP,taxmodeID,SalesManID,GrossAmt,ItemDiscountTotal,TaxAmt,Taxable,NonTaxable,CGSTTotal,SGSTTotal,IGSTTotal,NetAmount,OtherExpense,UserNarration,DiscPer,Discount,CashDiscount,RoundOFF,vchtypeID,sortNUmber,blnHold,EffectiveDate,RefNo,BillAmt,cancelled,userID,GStType,PartyCode,MobileNo,email,DestCCID,AgentCommMode,AgentCommAmount,AgentLID,BlnStockInsert,DeliveryNoteDetails,OrderDetails,ConvertedVchNo, ConvertedVchTypeID,ConvertedParentVchTypeID,CustomerpointsSettled,SystemName,LastUpdateDate,LastUpdateTime,DeliveryDetails, DespatchDetails, TermsOfDelivery,FloodCessTot,CounterID,ExtraCharges,CessAmountTot,QtyCompCessAmount) " +
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

                        #region "CRUD Operations for Sales Master ------------------------- >>"

                        if (iAction != 2)
                        {
                            string sRet = clsPur.SalesMasterCRUD(clsPM, sqlConn, trans, strJson, iAction);
                            if (sRet.Length > 2)
                            {
                                strResult = sRet.Split('|');
                                if (Convert.ToInt32(strResult[0].ToString()) == -1)
                                {
                                    MessageBox.Show("Failed ? " + sRet, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                        }
                        #endregion

                        #region "CRUD Operations for Sales Detail ------------------------- >>"
                        Hashtable hstPurStk = new Hashtable();

                        if (iAction == 1) // Edit
                        {
                            //trans.Commit();

                            sRetDet = clsPur.SalesDetailCRUD(clsPM, sqlConn, trans, sBatchCode, 2);
                            sRetDet = clsPur.SalesDetailCRUD(clsPM, sqlConn, trans, sBatchCode, 0);
                        }
                        else
                            sRetDet = clsPur.SalesDetailCRUD(clsPM, sqlConn, trans, sBatchCode, iAction);
                        
                        if (sRetDet == "") sRetDet = "0";
                        if (sRetDet.Length > 2)
                        {
                            strResult = sRetDet.Split('|');
                            strResult[0] += strResult[0] + "    ";
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
                            if (Comm.ToInt32(sRetDet) == -1)
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

                        if (iAction == 2)
                        {
                            string sRet = clsPur.SalesMasterCRUD(clsPM, sqlConn, trans, strJson, iAction);
                            if (sRet.Length > 2)
                            {
                                strResult = sRet.Split('|');
                                if (Convert.ToInt32(strResult[0].ToString()) == -1)
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
                                else
                                {
                                    //if (iIDFromEditWindow != 0)
                                    //    this.Close();
                                    //else
                                    //    Comm.MessageboxToasted("Sales", "Sales Group saved successfully");
                                }
                            }
                        }

                        if (iAction == 0 || iAction == 1)
                        {
                            if (cboPayment.SelectedIndex == 0)
                            {
                                if (clsVchTypeFeatures.BLNPOSTCASHENTRY == true)
                                {
                                    Comm.VoucherInsert(Convert.ToInt32(cboCostCentre.SelectedValue.ToString()), vchtypeID, dtpInvDate.Value, DateAndTime.Now.ToLocalTime(), 1, 0, 1, Convert.ToInt32(clsPM.clsJsonPMInfo_.InvId), clsPM.clsJsonPMInfo_.InvNo, txtNarration.Text.ToString(), 0, Convert.ToDouble(lblBillAmount.Text.ToString()), Convert.ToInt32(cboAgent.SelectedValue.ToString()), Convert.ToInt32(cboSalesStaff.SelectedValue.ToString()), 0, 0, false, txtNarration.Text.ToString());
                                    Comm.VoucherInsert(Convert.ToInt32(cboCostCentre.SelectedValue.ToString()), vchtypeID, dtpInvDate.Value, DateAndTime.Now.ToLocalTime(), Convert.ToDecimal(lblLID.Text.ToString()), Convert.ToDecimal(lblLID.Text.ToString()), 0, Convert.ToInt32(clsPM.clsJsonPMInfo_.InvId), clsPM.clsJsonPMInfo_.InvNo, txtNarration.Text.ToString(), Convert.ToDouble(lblBillAmount.Text.ToString()), 0, Convert.ToInt32(cboAgent.SelectedValue.ToString()), Convert.ToInt32(cboSalesStaff.SelectedValue.ToString()), 0, 0, false, txtNarration.Text.ToString());

                                    Comm.VoucherInsert(Convert.ToInt32(cboCostCentre.SelectedValue.ToString()), vchtypeID, dtpInvDate.Value, DateAndTime.Now.ToLocalTime(), 3, 3, 0, Convert.ToInt32(clsPM.clsJsonPMInfo_.InvId), clsPM.clsJsonPMInfo_.InvNo, txtNarration.Text.ToString(), Convert.ToDouble(lblBillAmount.Text.ToString()), 0, Convert.ToInt32(cboAgent.SelectedValue.ToString()), Convert.ToInt32(cboSalesStaff.SelectedValue.ToString()), 0, 0, false, txtNarration.Text.ToString());
                                    Comm.VoucherInsert(Convert.ToInt32(cboCostCentre.SelectedValue.ToString()), vchtypeID, dtpInvDate.Value, DateAndTime.Now.ToLocalTime(), Convert.ToDecimal(lblLID.Text.ToString()), 0, Convert.ToDecimal(lblLID.Text.ToString()), Convert.ToInt32(clsPM.clsJsonPMInfo_.InvId), clsPM.clsJsonPMInfo_.InvNo, txtNarration.Text.ToString(), 0, Convert.ToDouble(lblBillAmount.Text.ToString()), Convert.ToInt32(cboAgent.SelectedValue.ToString()), Convert.ToInt32(cboSalesStaff.SelectedValue.ToString()), 0, 0, false, txtNarration.Text.ToString());
                                }
                                else
                                {
                                    Comm.VoucherInsert(Convert.ToInt32(cboCostCentre.SelectedValue.ToString()), vchtypeID, dtpInvDate.Value, DateAndTime.Now.ToLocalTime(), 1, 0, 1, Convert.ToInt32(clsPM.clsJsonPMInfo_.InvId), clsPM.clsJsonPMInfo_.InvNo, txtNarration.Text.ToString(), 0, Convert.ToDouble(lblBillAmount.Text.ToString()), Convert.ToInt32(cboAgent.SelectedValue.ToString()), Convert.ToInt32(cboSalesStaff.SelectedValue.ToString()), 0, 0, false, txtNarration.Text.ToString());
                                    Comm.VoucherInsert(Convert.ToInt32(cboCostCentre.SelectedValue.ToString()), vchtypeID, dtpInvDate.Value, DateAndTime.Now.ToLocalTime(), 3, 3, 0, Convert.ToInt32(clsPM.clsJsonPMInfo_.InvId), clsPM.clsJsonPMInfo_.InvNo, txtNarration.Text.ToString(), Convert.ToDouble(lblBillAmount.Text.ToString()), 0, Convert.ToInt32(cboAgent.SelectedValue.ToString()), Convert.ToInt32(cboSalesStaff.SelectedValue.ToString()), 0, 0, false, txtNarration.Text.ToString());
                                }
                            }
                            if (cboPayment.SelectedIndex == 1)
                            {
                                Comm.VoucherInsert(Convert.ToInt32(cboCostCentre.SelectedValue.ToString()), vchtypeID, dtpInvDate.Value, DateAndTime.Now.ToLocalTime(), 1, 0, 1, Convert.ToInt32(clsPM.clsJsonPMInfo_.InvId), clsPM.clsJsonPMInfo_.InvNo, txtNarration.Text.ToString(), 0, Convert.ToDouble(lblBillAmount.Text.ToString()), Convert.ToInt32(cboAgent.SelectedValue.ToString()), Convert.ToInt32(cboSalesStaff.SelectedValue.ToString()), 0, 0, false, txtNarration.Text.ToString());
                                Comm.VoucherInsert(Convert.ToInt32(cboCostCentre.SelectedValue.ToString()), vchtypeID, dtpInvDate.Value, DateAndTime.Now.ToLocalTime(), Convert.ToDecimal(lblLID.Text.ToString()), Convert.ToDecimal(lblLID.Text.ToString()), 0, Convert.ToInt32(clsPM.clsJsonPMInfo_.InvId), clsPM.clsJsonPMInfo_.InvNo, txtNarration.Text.ToString(), Convert.ToDouble(lblBillAmount.Text.ToString()), 0, Convert.ToInt32(cboAgent.SelectedValue.ToString()), Convert.ToInt32(cboSalesStaff.SelectedValue.ToString()), 0, 0, false, txtNarration.Text.ToString());

                                if (txtInstantReceipt.Text != "")
                                {
                                    if (Comm.ToDouble(txtInstantReceipt.Text) > 0)
                                    {
                                        Comm.VoucherInsert(Convert.ToInt32(cboCostCentre.SelectedValue.ToString()), vchtypeID, dtpInvDate.Value, DateAndTime.Now.ToLocalTime(), 3, 3, 0, Convert.ToInt32(clsPM.clsJsonPMInfo_.InvId), clsPM.clsJsonPMInfo_.InvNo, txtNarration.Text.ToString(), Comm.ToDouble(txtInstantReceipt.Text), 0, Convert.ToInt32(cboAgent.SelectedValue.ToString()), Convert.ToInt32(cboSalesStaff.SelectedValue.ToString()), 0, 0, false, txtNarration.Text.ToString());
                                        Comm.VoucherInsert(Convert.ToInt32(cboCostCentre.SelectedValue.ToString()), vchtypeID, dtpInvDate.Value, DateAndTime.Now.ToLocalTime(), Convert.ToDecimal(lblLID.Text.ToString()), 0, Convert.ToDecimal(lblLID.Text.ToString()), Convert.ToInt32(clsPM.clsJsonPMInfo_.InvId), clsPM.clsJsonPMInfo_.InvNo, txtNarration.Text.ToString(), 0, Comm.ToDouble(txtInstantReceipt.Text), Convert.ToInt32(cboAgent.SelectedValue.ToString()), Convert.ToInt32(cboSalesStaff.SelectedValue.ToString()), 0, 0, false, txtNarration.Text.ToString());
                                    }
                                }
                            }
                        }

                        trans.Commit();
                        blnTransactionStarted = false;

                        string vchno = txtInvAutoNo.Text;

                        string id = "";
                        if (iIDFromEditWindow == 0)
                        {
                            id = clsJPMinfo.InvId.ToString();
                        }
                        else
                        {
                            id = iIDFromEditWindow.ToString();
                        }

                        if (iAction < 2 && blnLoadTest == false)
                        {
                            if (iIDFromEditWindow != 0)
                            {
                                if (PrintTrans(id.ToString()) == true)
                                { 
                                    if (prn.Visible == true && prn.Enabled == true)
                                    {
                                        if (clsVchTypeFeatures.blnprintimmediately == true)
                                        {
                                            prn.PrintReport(clsVchType.PrintSettings, cboInvScheme1.SelectedItem.ToString(), GetNoOfItems());
                                        }
                                        if (clsVchTypeFeatures.blnshowpreview == true)
                                        {
                                            prn.BringToFront();
                                            prn.Focus();
                                        }
                                        else
                                        {
                                            prn.Close();
                                            prn.Dispose();
                                        }
                                    }
                                }

                                this.Close();
                                Comm.MessageboxToasted("Sales", "Voucher[" + vchno + "] Saved Successfully");
                                return;
                            }
                            else
                            {
                                if (PrintTrans(id.ToString()) == true)
                                {
                                    if (prn.Visible == true && prn.Enabled == true)
                                    {
                                        if (clsVchTypeFeatures.blnprintimmediately == true)
                                        {
                                            prn.PrintReport(clsVchType.PrintSettings, cboInvScheme1.SelectedItem.ToString(), GetNoOfItems());
                                        }
                                        if (clsVchTypeFeatures.blnshowpreview == true)
                                        {
                                            prn.BringToFront();
                                            prn.Focus();
                                        }
                                        else
                                        {
                                            prn.Close();
                                            prn.Dispose();
                                        }
                                    }
                                }

                                ////edited by rohith 20/08/2022
                                //string inv = id;
                                //string PrintScheme;
                                //PrintScheme = comboBox7.SelectedItem.ToString() + ".rdlc";
                                //new ReportPrint(inv, PrintScheme).Show();
                                ////---------------------------------------

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

                                if (prn.Visible == true && prn.Enabled == true)
                                {
                                    prn.BringToFront();
                                    prn.Focus();
                                }

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

        private bool PrintTrans(string inv)
        {
            try
            {
                bool blnConfirmPrint = false;

                if (clsVchTypeFeatures.blnshowpreview == true || clsVchTypeFeatures.blnprintimmediately == true)
                {
                    blnConfirmPrint = true;
                }
                if (blnConfirmPrint == true && clsVchTypeFeatures.blnprintconfirmation == true)
                {
                    if (MessageBox.Show("Do you like to print the invoice.", "Invoice Print", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    {
                        blnConfirmPrint = false;
                    }
                }

                if (blnConfirmPrint == true)
                {
                    //edited by rohith 20/08/2022
                    //string inv = id;
                    string PrintScheme;
                    PrintScheme = cboInvScheme1.SelectedItem.ToString() + ".rdlc";
                    prn = new ReportPrint(inv, PrintScheme, this.MdiParent);
                    prn.Show();
                    prn.Focus();
                    //---------------------------------------
                }

                return blnConfirmPrint;
            }
            catch
            {
                return false;
            }
        }

        //Description : Agent Discount Asper Voucher Settings Value
        private void GetAgentDiscountAsperVoucherType()
        {
            DataTable dtAgentDisc = new DataTable();

            if (clsVchType.BillWiseDiscFillXtraDiscFromValue == 2) //Agent Discount
            {
                if (Convert.ToInt32(cboAgent.SelectedValue) >= 0)
                {
                    dtAgentDisc = Comm.fnGetData("SELECT ISNULL(AgentDiscount,0) as AgentDiscount FROM tblAgent WHERE AgentID = " + Convert.ToInt32(cboAgent.SelectedValue) + " AND TenantID = " + Global.gblTenantID + "").Tables[0];
                    if (dtAgentDisc.Rows.Count > 0)
                    {
                        txtDiscPerc.TextChanged -= txtDiscPerc_TextChanged;
                        txtDiscAmt.TextChanged -= txtDiscPerc_TextChanged;
                        txtDiscPerc.Text = Comm.FormatValue(Convert.ToDouble(dtAgentDisc.Rows[0][0].ToString()), true, "#.00");
                        txtDiscPerc.Tag = "2";//0-Default, 1-Agent wise, 2-supplier disc
                        txtDiscPerc.TextChanged += txtDiscPerc_TextChanged;
                        txtDiscAmt.TextChanged += txtDiscPerc_TextChanged;
                    }
                    CalcTotal();
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
                if (Convert.ToInt32(cboAgent.SelectedValue) >= 0)
                {
                    dtSuppDisc = Comm.fnGetData("SELECT ISNULL(DiscPer,0) as DiscPer FROM tblLedger WHERE LID = " + Comm.ConvertI32(Convert.ToDecimal(lblLID.Text.ToString())) + " AND TenantID = " + Global.gblTenantID + "").Tables[0];
                    if (dtSuppDisc.Rows.Count > 0)
                    {
                        txtDiscPerc.Text = Comm.FormatValue(Convert.ToDouble(dtSuppDisc.Rows[0][0].ToString()), true, "#.00");
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
            txtInstantReceipt.BackColor = Color.Gray;

            FillAgent();
            FillTaxMode();
            FillPriceList();
            FillEmployee();

            SetTransactionDefaults();
            SetTransactionsthatVarying();
            SetApplicationSettings();

            dgvStockOut.Rows.Clear();
            dgvStockOut.Refresh();
            iIDFromEditWindow = 0;
            //AddColumnsToGrid();
            dgvStockOut.Rows.Add();

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

            if (Comm.ConvertI32(clsVchType.TransactionNumberingValue) == 2) // Custom
                txtInvAutoNo.Clear();
            if (Comm.ConvertI32(clsVchType.ReferenceNumberingValue) == 2) // Custom
                txtReferencePrefix.Clear();

            dgvStockOut.Columns["cSlNo"].Frozen = true;
            //dgvStockOut.Columns["cImgDel"].Frozen = true;
            dgvStockOut.Columns["cImgDel"].Visible = true;
            dgvStockOut.Columns["cImgDel"].Width = 40;

            dgvStockOut.Columns["cRateinclusive"].Visible = false;
            dgvStockOut.Columns["cSRate1Per"].Visible = false;
            dgvStockOut.Columns["cSRate2Per"].Visible = false;
            dgvStockOut.Columns["cSRate3Per"].Visible = false;
            dgvStockOut.Columns["cSRate4Per"].Visible = false;
            dgvStockOut.Columns["cSRate5Per"].Visible = false;
            dgvStockOut.Columns["cSRate1"].Visible = false;
            dgvStockOut.Columns["cSRate2"].Visible = false;
            dgvStockOut.Columns["cSRate3"].Visible = false;
            dgvStockOut.Columns["cSRate4"].Visible = false;
            dgvStockOut.Columns["cSRate5"].Visible = false;

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
                dgvStockOut.Rows[iRowIndex].Cells[iCellIndex].Tag = Convert.ToDecimal(sTag).ToString("#.00");
            else
                dgvStockOut.Rows[iRowIndex].Cells[iCellIndex].Tag = sTag;
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
            DateTime sExpiryDate = Convert.ToDateTime(dgvStockOut.CurrentRow.Cells[GetEnum(gridColIndexes.CExpiry)].Value);

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
                sgblBarcodeNoExists = "BARCODE_NOTEXIST";
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
                    sgblBarcodeNoExists = "";
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
                if (dgvStockOut.CurrentRow == null) return false;

                mblnInitialisedSubWindow = false;

                int MyRow = dgvStockOut.CurrentRow.Index;

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
                            //GetItmMstinfo.ItemID = Convert.ToInt32(sCompSearchData[0].ToString());
                            //GetItmMstinfo.TenantID = Global.gblTenantID;

                            //dtItemPublic = clsItmMst.GetItemMaster(GetItmMstinfo);

                            GetItmMststockinfo.StockID = Convert.ToInt32(sCompSearchData[0].ToString());
                            GetItmMststockinfo.TenantID = Global.gblTenantID;

                            //dtItemPublic = clsItmMst.GetItemMaster(GetItmMstinfo);
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

                                //if (clsVchType.blnMovetoNextRowAfterSelection == 1)
                                SetValue(GetEnum(gridColIndexes.cQty), 1.ToString());

                                SetValue(GetEnum(gridColIndexes.cSrate), dtItemPublic.Rows[0]["SRate1"].ToString(), "CURR_FLOAT");
                                if (cboPriceList.Visible == true && cboPriceList.Enabled == true)
                                {
                                    //SetPriceListForItems(dgvStockOut.CurrentRow.Index);
                                    SetValue(GetEnum(gridColIndexes.cSrate), SetPriceListForItems(dgvStockOut.CurrentRow.Index).ToString(), "CURR_FLOAT");
                                }
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
                                SetTag(GetEnum(gridColIndexes.cSRate1Per), dgvStockOut.CurrentRow.Index, dtItemPublic.Rows[0]["SrateCalcMode"].ToString(), "");

                                SetTag(GetEnum(gridColIndexes.cCoolie), dgvStockOut.CurrentRow.Index, dtItemPublic.Rows[0]["Coolie"].ToString(), "");
                                SetValue(GetEnum(gridColIndexes.cCoolie), dtItemPublic.Rows[0]["Coolie"].ToString(), "CURR_FLOAT");

                                SetTag(GetEnum(gridColIndexes.cAgentCommPer), dgvStockOut.CurrentRow.Index, dtItemPublic.Rows[0]["agentCommPer"].ToString(), "");
                                SetValue(GetEnum(gridColIndexes.cAgentCommPer), dtItemPublic.Rows[0]["agentCommPer"].ToString(), "CURR_FLOAT");

                                if (clsVchType.DefaultTaxModeValue == 3) //GST
                                {
                                    //SetValue(GetEnum(gridColIndexes.cCGST), dtItemPublic.Rows[0]["CGSTTaxPer"].ToString(), "PERC_FLOAT");
                                    //SetValue(GetEnum(gridColIndexes.cSGST), dtItemPublic.Rows[0]["SGSTTaxPer"].ToString(), "PERC_FLOAT");
                                    //SetValue(GetEnum(gridColIndexes.cIGST), dtItemPublic.Rows[0]["IGSTTaxPer"].ToString(), "PERC_FLOAT");

                                    SetTag(GetEnum(gridColIndexes.cCGST), dgvStockOut.CurrentRow.Index, dtItemPublic.Rows[0]["CGSTTaxPer"].ToString(), "PERC_FLOAT");
                                    SetTag(GetEnum(gridColIndexes.cSGST), dgvStockOut.CurrentRow.Index, dtItemPublic.Rows[0]["SGSTTaxPer"].ToString(), "PERC_FLOAT");
                                    SetTag(GetEnum(gridColIndexes.cIGST), dgvStockOut.CurrentRow.Index, dtItemPublic.Rows[0]["IGSTTaxPer"].ToString(), "PERC_FLOAT");
                                    //SetTag(GetEnum(gridColIndexes.ctaxPer), dgvStockOut.CurrentRow.Index, dtItemPublic.Rows[0]["IGSTTaxPer"].ToString(), "PERC_FLOAT");
                                    SetValue(GetEnum(gridColIndexes.ctaxPer), dtItemPublic.Rows[0]["IGSTTaxPer"].ToString(), "PERC_FLOAT");
                                }
                                else
                                {
                                    //SetValue(GetEnum(gridColIndexes.cCGST), "0", "0");
                                    //SetValue(GetEnum(gridColIndexes.cSGST), "0", "0");
                                    //SetValue(GetEnum(gridColIndexes.cIGST), "0", "0");
                                    //SetValue(GetEnum(gridColIndexes.ctaxPer), "0", "0");
                                    SetTag(GetEnum(gridColIndexes.cCGST), dgvStockOut.CurrentRow.Index, "0", "PERC_FLOAT");
                                    SetTag(GetEnum(gridColIndexes.cSGST), dgvStockOut.CurrentRow.Index, "0", "PERC_FLOAT");
                                    SetTag(GetEnum(gridColIndexes.cIGST), dgvStockOut.CurrentRow.Index, "0", "PERC_FLOAT");
                                    //SetTag(GetEnum(gridColIndexes.ctaxPer), dgvStockOut.CurrentRow.Index, "0", "PERC_FLOAT");
                                    SetValue(GetEnum(gridColIndexes.ctaxPer), "0", "0");
                                }

                                if (Convert.ToInt32(dtItemPublic.Rows[0]["SRateInclusive"].ToString()) == 1)
                                    dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cRateinclusive)].Value = true;
                                else
                                    dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cRateinclusive)].Value = false;

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
                                SetTag(GetEnum(gridColIndexes.CExpiry), dgvStockOut.CurrentRow.Index, dtItemPublic.Rows[0]["BlnExpiryItem"].ToString());
                                if (Convert.ToInt32(dtItemPublic.Rows[0]["BlnExpiryItem"].ToString()) == 1)
                                {
                                    dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(gridColIndexes.CExpiry)].ReadOnly = false;
                                }
                                else
                                {
                                    dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(gridColIndexes.CExpiry)].ReadOnly = true;
                                }

                                iBatchmode = Convert.ToInt32(dtItemPublic.Rows[0]["batchMode"].ToString());
                                SetValue(GetEnum(gridColIndexes.cBatchMode), iBatchmode.ToString());
                                iShelfLifeDays = Convert.ToInt32(dtItemPublic.Rows[0]["Shelflife"].ToString());

                                if (iBatchmode == 1)
                                {
                                    if (dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cBarCode)].Visible == true)
                                        dgvStockOut.CurrentCell = dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cBarCode)];
                                    else
                                        dgvStockOut.CurrentCell = dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cQty)];

                                    dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(gridColIndexes.CItemCode)].Value = dtItemPublic.Rows[0]["ItemCode"].ToString();
                                    
                                    //dgvStockOut.BeginEdit(true);
                                }
                                else if (iBatchmode == 2)
                                {
                                    //if (dtItemPublic.Rows[0]["BatchUnique"].ToString() != "")
                                    //{
                                    //    dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cBarCode)].Tag = "<Auto Barcode>";
                                    //    dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cBarCode)].Value = 0;
                                    //}
                                    //else
                                    //{
                                    //    dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cBarCode)].Tag = "<Auto Barcode>";
                                    //    dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cBarCode)].Value = 0;
                                    //}

                                    //dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cBarCode)].Tag = "<Auto Barcode>";
                                    //dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cBarCode)].Value = 0;

                                    dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(gridColIndexes.CItemCode)].Value = dtItemPublic.Rows[0]["ItemCode"].ToString();
                                    //FillGridAsperStockID(Convert.ToInt32(dtItemPublic.Rows[0]["StockID"].ToString()));

                                    if (clsVchType.blnMovetoNextRowAfterSelection == 1)
                                    {
                                        //SetValue(GetEnum(gridColIndexes.cQty), 1, "CURR_FLOAT");

                                        if (dgvStockOut.Rows.Count - 1 == dgvStockOut.CurrentRow.Index)
                                            dgvStockOut.Rows.Add();

                                        dgvStockOut.CurrentCell = dgvStockOut.Rows[dgvStockOut.CurrentRow.Index + 1].Cells[1];
                                    }
                                    else
                                    {
                                        dgvStockOut.CurrentCell = dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cQty)];
                                    }


                                    dgvStockOut.Focus();
                                }
                                else if (iBatchmode == 0 || iBatchmode == 3)
                                {
                                    if (dtItemPublic.Rows[0]["BatchUnique"].ToString() != "")
                                    {
                                        dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cBarCode)].Tag = dtItemPublic.Rows[0]["BatchCode"].ToString();
                                        dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cBarCode)].Value = dtItemPublic.Rows[0]["BatchUnique"].ToString();
                                    }
                                    else
                                    {
                                        dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cBarCode)].Tag = dtItemPublic.Rows[0]["BatchCode"].ToString();
                                        dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cBarCode)].Value = dtItemPublic.Rows[0]["BatchCode"].ToString();
                                    }

                                    dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(gridColIndexes.CItemCode)].Value = dtItemPublic.Rows[0]["ItemCode"].ToString();

                                    if (dtItemPublic.Rows[0]["StockID"].ToString() != "")
                                        FillGridAsperStockID(Convert.ToInt32(dtItemPublic.Rows[0]["StockID"].ToString()));

                                    if (clsVchType.blnMovetoNextRowAfterSelection == 1)
                                    {
                                        //SetValue(GetEnum(gridColIndexes.cQty), 1, "CURR_FLOAT");

                                        if (dgvStockOut.Rows.Count - 1 == dgvStockOut.CurrentRow.Index)
                                            dgvStockOut.Rows.Add();

                                        dgvStockOut.CurrentCell = dgvStockOut.Rows[dgvStockOut.CurrentRow.Index + 1].Cells[1];
                                    }
                                    else
                                    {
                                        dgvStockOut.CurrentCell = dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cQty)];
                                    }
                                    //dgvStockOut.CurrentCell = dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(gridColIndexes.CExpiry)];
                                    dgvStockOut.Focus();
                                }

                                //SetValue(GetEnum(gridColIndexes.CItemCode), dtItemPublic.Rows[0]["ItemCode"].ToString());
                                
                                dgvStockOut.Rows[MyRow].Cells[GetEnum(gridColIndexes.CItemCode)].Value = dtItemPublic.Rows[0]["ItemCode"].ToString();

                                //dgvStockOut.CurrentCell = dgvStockOut[gridColIndexes.cQty, dgvStockOut.CurrentRow.Index];

                                //SetValue(GetEnum(gridColIndexes.CItemCode), dtItemPublic.Rows[0]["ItemCode"].ToString());

                                if (dgvStockOut.Rows.Count - 1 == dgvStockOut.CurrentRow.Index)
                                    if (dgvStockOut[gridColIndexes.CItemName, dgvStockOut.CurrentRow.Index].Tag != null)
                                        if (Convert.ToInt32(dgvStockOut[gridColIndexes.CItemName, dgvStockOut.CurrentRow.Index].Tag) > 0)
                                            dgvStockOut.Rows.Add();

                                if (clsVchType.blnMovetoNextRowAfterSelection != 1)
                                {
                                    dgvStockOut.CurrentCell = dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cQty)];
                                }

                                //if (dgvStockOut.Rows.Count <= iRow + 1)
                                //    dgvStockOut.Rows.Add();

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
                            int WMTotalLength = 0;
                            string[] strWMIdentifier = { };
                            if (AppSettings.STRWMIDENTIFIER != "")
                            {
                                if (AppSettings.STRWMIDENTIFIER.Contains("/"))
                                    WMTotalLength = WMTotalLength + ((AppSettings.STRWMIDENTIFIER.Length - 1) / 2);
                                else
                                    WMTotalLength = WMTotalLength + ((AppSettings.STRWMIDENTIFIER.Length));

                                strWMIdentifier = AppSettings.STRWMIDENTIFIER.Split('/');
                            }
                            if (AppSettings.STRWMBARCODELENGTH != "")
                                if (Comm.ToInt32(AppSettings.STRWMBARCODELENGTH) > 0)
                                    WMTotalLength = WMTotalLength + Comm.ToInt32(AppSettings.STRWMBARCODELENGTH);
                            if (AppSettings.STRWMQTYLENGTH != "")
                                if (Comm.ToInt32(AppSettings.STRWMQTYLENGTH) > 0)
                                    WMTotalLength = WMTotalLength + Comm.ToInt32(AppSettings.STRWMQTYLENGTH);

                            string BCodeToSplit = sCompSearchData[0].ToString();
                            string WMBCode = "";
                            string WMQty = "";

                            if (WMTotalLength == BCodeToSplit.Length)
                            {
                                for (int i = 0; i < strWMIdentifier.Count(); i++)
                                {
                                    if (BCodeToSplit.Substring(0, strWMIdentifier[i].Length) == strWMIdentifier[i])
                                    {
                                        WMBCode = BCodeToSplit.Substring(strWMIdentifier[i].Length, Comm.ToInt32(AppSettings.STRWMBARCODELENGTH));
                                        WMQty = BCodeToSplit.Substring(strWMIdentifier[i].Length + Comm.ToInt32(AppSettings.STRWMBARCODELENGTH), Comm.ToInt32(AppSettings.STRWMQTYLENGTH));

                                        break;
                                    }
                                }
                            }

                            if (WMBCode != "")
                                GetItmMstBatchinfo.BatchUnique = WMBCode;
                            else
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

                                    //if (clsVchType.blnMovetoNextRowAfterSelection == 1)
                                    SetValue(GetEnum(gridColIndexes.cQty), 1.ToString());

                                    if (WMBCode != "" && WMQty != "")
                                        SetValue(GetEnum(gridColIndexes.cQty), (Comm.ToDecimal(WMQty.ToString())/1000).ToString());


                                    SetValue(GetEnum(gridColIndexes.cSrate), dtItemPublic.Rows[0]["SRate1"].ToString(), "CURR_FLOAT");
                                    if (cboPriceList.Visible == true && cboPriceList.Enabled == true)
                                    {
                                        Application.DoEvents();
                                        SetValue(GetEnum(gridColIndexes.cSrate), SetPriceListForItems(dgvStockOut.CurrentRow.Index).ToString(), "CURR_FLOAT");
                                    }
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
                                    SetTag(GetEnum(gridColIndexes.cSRate1Per), dgvStockOut.CurrentRow.Index, dtItemPublic.Rows[0]["SrateCalcMode"].ToString(), "");

                                    SetTag(GetEnum(gridColIndexes.cCoolie), dgvStockOut.CurrentRow.Index, dtItemPublic.Rows[0]["Coolie"].ToString(), "");
                                    SetValue(GetEnum(gridColIndexes.cCoolie), dtItemPublic.Rows[0]["Coolie"].ToString(), "CURR_FLOAT");

                                    SetTag(GetEnum(gridColIndexes.cAgentCommPer), dgvStockOut.CurrentRow.Index, dtItemPublic.Rows[0]["agentCommPer"].ToString(), "");
                                    SetValue(GetEnum(gridColIndexes.cAgentCommPer), dtItemPublic.Rows[0]["agentCommPer"].ToString(), "CURR_FLOAT");

                                    if (clsVchType.DefaultTaxModeValue == 3) //GST
                                    {
                                        //SetValue(GetEnum(gridColIndexes.cCGST), dtItemPublic.Rows[0]["CGSTTaxPer"].ToString(), "PERC_FLOAT");
                                        //SetValue(GetEnum(gridColIndexes.cSGST), dtItemPublic.Rows[0]["SGSTTaxPer"].ToString(), "PERC_FLOAT");
                                        //SetValue(GetEnum(gridColIndexes.cIGST), dtItemPublic.Rows[0]["IGSTTaxPer"].ToString(), "PERC_FLOAT");

                                        SetTag(GetEnum(gridColIndexes.cCGST), dgvStockOut.CurrentRow.Index, dtItemPublic.Rows[0]["CGSTTaxPer"].ToString(), "PERC_FLOAT");
                                        SetTag(GetEnum(gridColIndexes.cSGST), dgvStockOut.CurrentRow.Index, dtItemPublic.Rows[0]["SGSTTaxPer"].ToString(), "PERC_FLOAT");
                                        SetTag(GetEnum(gridColIndexes.cIGST), dgvStockOut.CurrentRow.Index, dtItemPublic.Rows[0]["IGSTTaxPer"].ToString(), "PERC_FLOAT");
                                        //SetTag(GetEnum(gridColIndexes.ctaxPer), dgvStockOut.CurrentRow.Index, dtItemPublic.Rows[0]["IGSTTaxPer"].ToString(), "PERC_FLOAT");
                                        SetValue(GetEnum(gridColIndexes.ctaxPer), dtItemPublic.Rows[0]["IGSTTaxPer"].ToString(), "PERC_FLOAT");
                                    }
                                    else
                                    {
                                        //SetValue(GetEnum(gridColIndexes.cCGST), "0", "0");
                                        //SetValue(GetEnum(gridColIndexes.cSGST), "0", "0");
                                        //SetValue(GetEnum(gridColIndexes.cIGST), "0", "0");
                                        //SetValue(GetEnum(gridColIndexes.ctaxPer), "0", "0");
                                        SetTag(GetEnum(gridColIndexes.cCGST), dgvStockOut.CurrentRow.Index, "0", "PERC_FLOAT");
                                        SetTag(GetEnum(gridColIndexes.cSGST), dgvStockOut.CurrentRow.Index, "0", "PERC_FLOAT");
                                        SetTag(GetEnum(gridColIndexes.cIGST), dgvStockOut.CurrentRow.Index, "0", "PERC_FLOAT");
                                        //SetTag(GetEnum(gridColIndexes.ctaxPer), dgvStockOut.CurrentRow.Index, "0", "PERC_FLOAT");
                                        SetValue(GetEnum(gridColIndexes.ctaxPer), "0", "0");
                                    }

                                    if (Convert.ToInt32(dtItemPublic.Rows[0]["SRateInclusive"].ToString()) == 1)
                                        dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cRateinclusive)].Value = true;
                                    else
                                        dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cRateinclusive)].Value = false;

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
                                    SetTag(GetEnum(gridColIndexes.CExpiry), dgvStockOut.CurrentRow.Index, dtItemPublic.Rows[0]["BlnExpiryItem"].ToString());
                                    if (Convert.ToInt32(dtItemPublic.Rows[0]["BlnExpiryItem"].ToString()) == 1)
                                    {
                                        dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(gridColIndexes.CExpiry)].ReadOnly = false;
                                    }
                                    else
                                    {
                                        dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(gridColIndexes.CExpiry)].ReadOnly = true;
                                    }

                                    iBatchmode = Convert.ToInt32(dtItemPublic.Rows[0]["batchMode"].ToString());
                                    SetValue(GetEnum(gridColIndexes.cBatchMode), iBatchmode.ToString());
                                    iShelfLifeDays = Convert.ToInt32(dtItemPublic.Rows[0]["Shelflife"].ToString());

                                    if (iBatchmode == 1)
                                    {
                                        //dgvStockOut.CurrentCell = dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cBarCode)];
                                        dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(gridColIndexes.CItemCode)].Value = dtItemPublic.Rows[0]["ItemCode"].ToString();
                                        dgvStockOut.BeginEdit(true);
                                    }
                                    else if (iBatchmode == 2)
                                    {
                                        //if (dtItemPublic.Rows[0]["BatchUnique"].ToString() != "")
                                        //{
                                        //    dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cBarCode)].Tag = "<Auto Barcode>";
                                        //    dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cBarCode)].Value = 0;
                                        //}
                                        //else
                                        //{
                                        //    dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cBarCode)].Tag = "<Auto Barcode>";
                                        //    dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cBarCode)].Value = 0;
                                        //}

                                        //dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cBarCode)].Tag = "<Auto Barcode>";
                                        //dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cBarCode)].Value = 0;

                                        dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(gridColIndexes.CItemCode)].Value = dtItemPublic.Rows[0]["ItemCode"].ToString();
                                        //FillGridAsperStockID(Convert.ToInt32(dtItemPublic.Rows[0]["StockID"].ToString()));

                                        if (dtItemPublic.Rows[0]["StockID"].ToString() != "")
                                            FillGridAsperStockID(Convert.ToInt32(dtItemPublic.Rows[0]["StockID"].ToString()));

                                        //dgvStockOut.CurrentCell = dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(gridColIndexes.CExpiry)];
                                        //dgvStockOut.Focus();
                                    }
                                    else if (iBatchmode == 0 || iBatchmode == 3)
                                    {
                                        //if (dtItemPublic.Rows[0]["BatchUnique"].ToString() != "")
                                        //{
                                        //    dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cBarCode)].Tag = dtItemPublic.Rows[0]["BatchCode"].ToString();
                                        //    dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cBarCode)].Value = dtItemPublic.Rows[0]["BatchUnique"].ToString();
                                        //}
                                        //else
                                        //{
                                        //    dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cBarCode)].Tag = dtItemPublic.Rows[0]["BatchCode"].ToString();
                                        //    dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cBarCode)].Value = dtItemPublic.Rows[0]["BatchCode"].ToString();
                                        //}

                                        dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(gridColIndexes.CItemCode)].Value = dtItemPublic.Rows[0]["ItemCode"].ToString();

                                        if (dtItemPublic.Rows[0]["StockID"].ToString() != "")
                                            FillGridAsperStockID(Convert.ToInt32(dtItemPublic.Rows[0]["StockID"].ToString()));

                                        //dgvStockOut.CurrentCell = dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(gridColIndexes.CExpiry)];
                                        //dgvStockOut.Focus();
                                    }
                                    SetValue(GetEnum(gridColIndexes.CItemCode), dtItemPublic.Rows[0]["ItemCode"].ToString());

                                    dgvStockOut.CellEndEdit -= dgvStockOut_CellEndEdit;
                                    if (dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(gridColIndexes.CExpiry)].Visible == true)
                                        dgvStockOut.CurrentCell = dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(gridColIndexes.CExpiry)];
                                    else if (dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cQty)].Visible == true)
                                        dgvStockOut.CurrentCell = dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cMRP)];
                                    dgvStockOut.Focus();
                                    dgvStockOut.CellEndEdit += dgvStockOut_CellEndEdit;

                                    if (dgvStockOut.Rows.Count - 1 == dgvStockOut.CurrentRow.Index)
                                        dgvStockOut.Rows.Add();

                                    //if (dgvStockOut.Rows.Count - 1 == dgvStockOut.CurrentRow.Index)
                                    //    dgvStockOut.Rows.Add();

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
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                    frmBatchSearch = new frmCompactSearch(GetFromBarcodeSearch, sQuery, "Anywhere|BatchCode|CONVERT(VARCHAR,ExpiryDate)|CONVERT(VARCHAR,ISNULL(MRP,0))|CONVERT(VARCHAR,ISNULL(QOH,0))", dgvStockOut.Location.X + 350, dgvStockOut.Location.Y + 150, 4, 0, "", 4, 0, "ORDER BY A.BatchCode ASC", 0, 0, "BatchCode Search...", 0, "150,90,80,80,0,0", true, "BatchCode", 10);
                    frmBatchSearch.Show();
                    frmBatchSearch.BringToFront();
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        // Created By : Dipu 
        // Created On : 21-Feb-2022
        // Description: To Calculate Tax When TaxMode Combo Box Change
        private void TaxCalculate()
        {
            if (cboTaxMode.SelectedValue == null)
                if (cboTaxMode.Items.Count > 0)
                    cboTaxMode.SelectedIndex = 0;

            if (dgvStockOut.Rows.Count > 1)
            {
                for (int k = 0; k < dgvStockOut.Rows.Count; k++)
                {
                    if (dgvStockOut.Rows[k].Cells[GetEnum(gridColIndexes.CItemCode)].Value != null)
                    {

                        GetItmMstinfo.ItemID = Convert.ToDecimal(dgvStockOut.Rows[k].Cells[GetEnum(gridColIndexes.cItemID)].Value.ToString());
                        GetItmMstinfo.TenantID = Global.gblTenantID;

                        dtItemPublic = clsItmMst.GetItemMaster(GetItmMstinfo);
                        if (dtItemPublic.Rows.Count > 0)
                        {
                            if (cboTaxMode.SelectedValue.ToString() == "3") //GST
                            {
                                dgvStockOut.Columns["cCGST"].Visible = true;
                                dgvStockOut.Columns["cSGST"].Visible = true;
                                dgvStockOut.Columns["cIGST"].Visible = true;
                                dgvStockOut.Columns["ctaxPer"].Visible = true;
                                dgvStockOut.Columns["ctax"].Visible = true;
                                dgvStockOut.Columns["ctaxable"].Visible = true;
                                dgvStockOut.Columns["cCRateWithTax"].Visible = true;

                                tblpTaxAmt.Visible = true;
                                tblpTaxable.Visible = true;

                                dgvStockOut.Rows[k].Cells[GetEnum(gridColIndexes.cCGST)].Tag = Convert.ToDecimal(dtItemPublic.Rows[0]["CGSTTaxPer"].ToString()).ToString("#.00");
                                dgvStockOut.Rows[k].Cells[GetEnum(gridColIndexes.cSGST)].Tag = Convert.ToDecimal(dtItemPublic.Rows[0]["SGSTTaxPer"].ToString()).ToString("#.00");
                                dgvStockOut.Rows[k].Cells[GetEnum(gridColIndexes.cIGST)].Tag = Convert.ToDecimal(dtItemPublic.Rows[0]["IGSTTaxPer"].ToString()).ToString("#.00");
                                dgvStockOut.Rows[k].Cells[GetEnum(gridColIndexes.ctaxPer)].Tag = Convert.ToDecimal(dtItemPublic.Rows[0]["IGSTTaxPer"].ToString()).ToString("#.00");
                                dgvStockOut.Rows[k].Cells[GetEnum(gridColIndexes.ctaxPer)].Value = Convert.ToDecimal(dtItemPublic.Rows[0]["IGSTTaxPer"].ToString()).ToString("#.00");
                            }
                            else if (cboTaxMode.SelectedValue.ToString() == "2") //GST
                            {
                                if (dgvStockOut.Columns.Count > 0)
                                {
                                    dgvStockOut.Rows[k].Cells[GetEnum(gridColIndexes.cCGST)].Tag = "0";
                                    dgvStockOut.Rows[k].Cells[GetEnum(gridColIndexes.cSGST)].Tag = "0";
                                    dgvStockOut.Rows[k].Cells[GetEnum(gridColIndexes.cIGST)].Tag = "0";
                                    dgvStockOut.Rows[k].Cells[GetEnum(gridColIndexes.ctaxPer)].Tag = Convert.ToDecimal(dtItemPublic.Rows[0]["VAT"].ToString()).ToString("#.00");
                                    dgvStockOut.Rows[k].Cells[GetEnum(gridColIndexes.ctaxPer)].Value = Convert.ToDecimal(dtItemPublic.Rows[0]["VAT"].ToString()).ToString("#.00");

                                    SetTag(GetEnum(gridColIndexes.cCGST), dgvStockOut.CurrentRow.Index, "0", "0");
                                    SetTag(GetEnum(gridColIndexes.cSGST), dgvStockOut.CurrentRow.Index, "0", "0");
                                    SetTag(GetEnum(gridColIndexes.cIGST), dgvStockOut.CurrentRow.Index, "0", "0");
                                    SetTag(GetEnum(gridColIndexes.ctaxPer), dgvStockOut.CurrentRow.Index, Convert.ToDecimal(dtItemPublic.Rows[0]["VAT"].ToString()).ToString("#.00"), "0");
                                    SetValue(GetEnum(gridColIndexes.ctaxPer), dgvStockOut.CurrentRow.Index, Convert.ToDecimal(dtItemPublic.Rows[0]["VAT"].ToString()).ToString("#.00"), "0");

                                    dgvStockOut.Columns["cCGST"].Visible = false;
                                    dgvStockOut.Columns["cSGST"].Visible = false;
                                    dgvStockOut.Columns["cIGST"].Visible = false;
                                    dgvStockOut.Columns["ctaxPer"].Visible = false;
                                    dgvStockOut.Columns["ctax"].Visible = false;
                                    dgvStockOut.Columns["ctaxable"].Visible = false;
                                    dgvStockOut.Columns["cCRateWithTax"].Visible = false;

                                    tblpTaxAmt.Visible = false;
                                    tblpTaxable.Visible = false;
                                }
                            }
                            else if (cboTaxMode.SelectedValue.ToString() == "1") //GST
                            {
                                if (dgvStockOut.Columns.Count > 0)
                                {
                                    dgvStockOut.Rows[k].Cells[GetEnum(gridColIndexes.cCGST)].Tag = "0";
                                    dgvStockOut.Rows[k].Cells[GetEnum(gridColIndexes.cSGST)].Tag = "0";
                                    dgvStockOut.Rows[k].Cells[GetEnum(gridColIndexes.cIGST)].Tag = "0";
                                    dgvStockOut.Rows[k].Cells[GetEnum(gridColIndexes.ctaxPer)].Tag = "0";
                                    dgvStockOut.Rows[k].Cells[GetEnum(gridColIndexes.ctaxPer)].Value = "0";

                                    SetTag(GetEnum(gridColIndexes.cCGST), dgvStockOut.CurrentRow.Index, "0", "0");
                                    SetTag(GetEnum(gridColIndexes.cSGST), dgvStockOut.CurrentRow.Index, "0", "0");
                                    SetTag(GetEnum(gridColIndexes.cIGST), dgvStockOut.CurrentRow.Index, "0", "0");
                                    SetTag(GetEnum(gridColIndexes.ctaxPer), dgvStockOut.CurrentRow.Index, "0", "0");
                                    SetValue(GetEnum(gridColIndexes.ctaxPer), dgvStockOut.CurrentRow.Index, "0", "0");

                                    dgvStockOut.Columns["cCGST"].Visible = false;
                                    dgvStockOut.Columns["cSGST"].Visible = false;
                                    dgvStockOut.Columns["cIGST"].Visible = false;
                                    dgvStockOut.Columns["ctaxPer"].Visible = false;
                                    dgvStockOut.Columns["ctax"].Visible = false;
                                    dgvStockOut.Columns["ctaxable"].Visible = false;
                                    dgvStockOut.Columns["cCRateWithTax"].Visible = false;

                                    tblpTaxAmt.Visible = false;
                                    tblpTaxable.Visible = false;
                                }
                            }
                        }
                    }
                }
                CalcTotal();
            }
            else
            {
                if (cboTaxMode.SelectedValue.ToString() == "3") //GST
                {
                    if (dgvStockOut.Columns.Count > 0)
                    {
                        dgvStockOut.Columns["cCGST"].Visible = true;
                        dgvStockOut.Columns["cSGST"].Visible = true;
                        dgvStockOut.Columns["cIGST"].Visible = true;
                        dgvStockOut.Columns["ctaxPer"].Visible = true;
                        dgvStockOut.Columns["ctax"].Visible = true;
                        dgvStockOut.Columns["ctaxable"].Visible = true;
                        dgvStockOut.Columns["cCRateWithTax"].Visible = true;

                        tblpTaxAmt.Visible = true;
                        tblpTaxable.Visible = true;
                    }
                }
                else
                {
                    if (dgvStockOut.Columns.Count > 0 && dgvStockOut.CurrentRow != null)
                    {
                        SetTag(GetEnum(gridColIndexes.cCGST), dgvStockOut.CurrentRow.Index, "0", "0");
                        SetTag(GetEnum(gridColIndexes.cSGST), dgvStockOut.CurrentRow.Index, "0", "0");
                        SetTag(GetEnum(gridColIndexes.cIGST), dgvStockOut.CurrentRow.Index, "0", "0");
                        SetTag(GetEnum(gridColIndexes.ctaxPer), dgvStockOut.CurrentRow.Index, "0", "0");
                        SetValue(GetEnum(gridColIndexes.ctaxPer), dgvStockOut.CurrentRow.Index, "0", "0");

                        dgvStockOut.Columns["cCGST"].Visible = false;
                        dgvStockOut.Columns["cSGST"].Visible = false;
                        dgvStockOut.Columns["cIGST"].Visible = false;
                        dgvStockOut.Columns["ctaxPer"].Visible = false;
                        dgvStockOut.Columns["ctax"].Visible = false;
                        dgvStockOut.Columns["ctaxable"].Visible = false;
                        dgvStockOut.Columns["cCRateWithTax"].Visible = false;
                    }
                    tblpTaxAmt.Visible = false;
                    tblpTaxable.Visible = false;
                }
                CalcTotal();
            }
        }

        //Description : Row Delete when Press Delete or Delete icon
        private void RowDelete()
        {
            if (dgvStockOut.Rows[dgvStockOut.CurrentCell.RowIndex].Cells[GetEnum(gridColIndexes.CItemName)].Tag == null) return;
            if  (dgvStockOut.Rows[dgvStockOut.CurrentCell.RowIndex].Cells[GetEnum(gridColIndexes.CItemName)].Tag.ToString() == "") return;
            if  (Convert.ToInt32(dgvStockOut.Rows[dgvStockOut.CurrentCell.RowIndex].Cells[GetEnum(gridColIndexes.CItemName)].Tag.ToString()) <= 0) return;

            int rowIndex = dgvStockOut.CurrentCell.RowIndex;
            dgvStockOut.Rows.RemoveAt(rowIndex);
            decimal dinvid = GetRepackingIfo.InvId;
        }

        //Description : Initializing the Columns in The Grid
        private void AddColumnsToGrid()
        {
            this.dgvStockOut.Columns.Clear();

            this.dgvStockOut.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSlNo", HeaderText = "Sl.No", Width = 50 }); //1

            if (AppSettings.BLNBARCODE == true)
            {
                switch (clsVchType.DefaultBarcodeMode)
                {
                    case 0:// BarcodeMode.BarcodeDropdown:
                        {
                            this.dgvStockOut.Columns.Add(new DataGridViewTextBoxColumn() { Name = "CItemCode", HeaderText = "Item Code", Width = 130 }); //1
                            this.dgvStockOut.Columns.Add(new DataGridViewTextBoxColumn() { Name = "CItemName", HeaderText = "Item Name", Width = 200 }); //2
                            this.dgvStockOut.Columns.Add(new DataGridViewTextBoxColumn() { Name = "CUnit", ReadOnly = true, Visible = true, HeaderText = "Unit", Width = 50 }); //3

                            this.dgvStockOut.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cBarCode", HeaderText = "Batch Code", Width = 200 }); //4

                            break;
                        }
                    case 1:// BarcodeMode.BarcodeKeyboard:
                        {

                            this.dgvStockOut.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cBarCode", HeaderText = "Batch Code", Width = 200 }); //4
                            this.dgvStockOut.Columns.Add(new DataGridViewTextBoxColumn() { Name = "CItemCode", HeaderText = "Item Code", Width = 130 }); //1
                            this.dgvStockOut.Columns.Add(new DataGridViewTextBoxColumn() { Name = "CItemName", HeaderText = "Item Name", Width = 200 }); //2
                            this.dgvStockOut.Columns.Add(new DataGridViewTextBoxColumn() { Name = "CUnit", ReadOnly = true, Visible = true, HeaderText = "Unit", Width = 50 }); //3

                            break;
                        }
                    case 2:// BarcodeMode.BarcodeScanning:
                        {
                            this.dgvStockOut.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cBarCode", HeaderText = "Batch Code", Width = 200 }); //4
                            this.dgvStockOut.Columns.Add(new DataGridViewTextBoxColumn() { Name = "CItemCode", HeaderText = "Item Code", Width = 130 }); //1
                            this.dgvStockOut.Columns.Add(new DataGridViewTextBoxColumn() { Name = "CItemName", HeaderText = "Item Name", Width = 200 }); //2
                            this.dgvStockOut.Columns.Add(new DataGridViewTextBoxColumn() { Name = "CUnit", ReadOnly = true, Visible = true, HeaderText = "Unit", Width = 50 }); //3

                            break;
                        }
                    default:
                        {
                            this.dgvStockOut.Columns.Add(new DataGridViewTextBoxColumn() { Name = "CItemCode", HeaderText = "Item Code", Width = 130 }); //1
                            this.dgvStockOut.Columns.Add(new DataGridViewTextBoxColumn() { Name = "CItemName", HeaderText = "Item Name", Width = 200 }); //2
                            this.dgvStockOut.Columns.Add(new DataGridViewTextBoxColumn() { Name = "CUnit", ReadOnly = true, Visible = true, HeaderText = "Unit", Width = 50 }); //3
                                                                                                                                                                             //Commented and added By Dipu on 23-Feb-2022 ------------- >>
                            this.dgvStockOut.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cBarCode", HeaderText = "Batch Code", Width = 200 }); //4

                            break;
                        }
                }

                this.dgvStockOut.Columns.Add(new DataGridViewTextBoxColumn() { Name = "CExpiry", HeaderText = "Expiry Date", Width = 120 }); //5
            }
            else
            {
                this.dgvStockOut.Columns.Add(new DataGridViewTextBoxColumn() { Name = "CItemCode", HeaderText = "Item Code", Width = 130 }); //1
                this.dgvStockOut.Columns.Add(new DataGridViewTextBoxColumn() { Name = "CItemName", HeaderText = "Item Name", Width = 200 }); //2
                this.dgvStockOut.Columns.Add(new DataGridViewTextBoxColumn() { Name = "CUnit", ReadOnly = true, Visible = true, HeaderText = "Unit", Width = 50 }); //3

                this.dgvStockOut.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cBarCode", HeaderText = "Batch Code", Width = 200, Visible = false, ReadOnly = true }); //4

                this.dgvStockOut.Columns.Add(new DataGridViewTextBoxColumn() { Name = "CExpiry", HeaderText = "Expiry Date", Width = 120, Visible = false, ReadOnly = true }); //5
            }


            if (clsVchTypeFeatures.BLNEDITMRPRATE == true)
            {
                if (AppSettings.IsActiveMRP == true)
                    this.dgvStockOut.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cMRP", HeaderText = "" + AppSettings.MRPName + "", ReadOnly = false, Visible = true, Width = 80 }); //6
                else
                    this.dgvStockOut.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cMRP", HeaderText = "" + AppSettings.MRPName + "", ReadOnly = false, Visible = false, Width = 80 }); //6
            }
            else
            {
                if (AppSettings.IsActiveMRP == true)
                    this.dgvStockOut.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cMRP", HeaderText = "" + AppSettings.MRPName + "", ReadOnly = true, Visible = true, Width = 80 }); //6
                else
                    this.dgvStockOut.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cMRP", HeaderText = "" + AppSettings.MRPName + "", ReadOnly = true, Visible = false, Width = 80 }); //6
            }

            this.dgvStockOut.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSrate", HeaderText = "SRate", Width = 80 }); //7

            if (AppSettings.TaxMode == 2) //GST
                this.dgvStockOut.Columns.Add(new DataGridViewCheckBoxColumn() { Name = "cRateinclusive", HeaderText = "Rate Inc.", Width = 80 }); //20
            else
                this.dgvStockOut.Columns.Add(new DataGridViewCheckBoxColumn() { Name = "cRateinclusive", HeaderText = "Rate Inc.", Visible = false, Width = 80 }); //20

            this.dgvStockOut.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cQty", HeaderText = "Qty", Width = 80 }); //8
            if (clsVchTypeFeatures.BLNSHOWFREEQUANTITY == true)
                this.dgvStockOut.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cFree", HeaderText = "Free", Visible = true, Width = 80 }); //9
            else
                this.dgvStockOut.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cFree", HeaderText = "Free", Visible = false, Width = 80 }); //9

            if (clsVchTypeFeatures.blneditsalerate == true)
            {
                this.dgvStockOut.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate1Per", HeaderText = "" + AppSettings.SRate1Name + " %", ReadOnly = false, Visible = false, Width = 80 }); //10
                this.dgvStockOut.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate1", HeaderText = "" + AppSettings.SRate1Name + "", ReadOnly = false, Visible = false, Width = 80 }); //11
                if (AppSettings.IsActiveSRate2 == true)
                {
                    this.dgvStockOut.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate2Per", HeaderText = "" + AppSettings.SRate2Name + " %", ReadOnly = false, Visible = false, Width = 80 }); //12
                    this.dgvStockOut.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate2", HeaderText = "" + AppSettings.SRate2Name + "", ReadOnly = false, Visible = false, Width = 80 }); //13
                }
                else
                {
                    this.dgvStockOut.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate2Per", HeaderText = "" + AppSettings.SRate2Name + " %", ReadOnly = false, Visible = false, Width = 80 }); //12
                    this.dgvStockOut.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate2", HeaderText = "" + AppSettings.SRate2Name + "", ReadOnly = false, Visible = false, Width = 80 }); //13
                }

                if (AppSettings.IsActiveSRate3 == true)
                {
                    this.dgvStockOut.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate3Per", HeaderText = "" + AppSettings.SRate3Name + " %", ReadOnly = false, Visible = false, Width = 80 }); //14
                    this.dgvStockOut.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate3", HeaderText = AppSettings.SRate3Name, ReadOnly = false, Visible = false, Width = 80 }); //15
                }
                else
                {
                    this.dgvStockOut.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate3Per", HeaderText = "" + AppSettings.SRate3Name + " %", ReadOnly = false, Visible = false, Width = 80 }); //14
                    this.dgvStockOut.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate3", HeaderText = AppSettings.SRate3Name, ReadOnly = false, Visible = false, Width = 80 }); //15
                }

                if (AppSettings.IsActiveSRate4 == true)
                {
                    this.dgvStockOut.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate4Per", HeaderText = "" + AppSettings.SRate4Name + " %", ReadOnly = false, Visible = false, Width = 80 }); //16
                    this.dgvStockOut.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate4", HeaderText = AppSettings.SRate4Name, ReadOnly = false, Visible = false, Width = 80 }); //17
                }
                else
                {
                    this.dgvStockOut.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate4Per", HeaderText = "" + AppSettings.SRate4Name + " %", ReadOnly = false, Visible = false, Width = 80 }); //16
                    this.dgvStockOut.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate4", HeaderText = AppSettings.SRate4Name, ReadOnly = false, Visible = false, Width = 80 }); //17
                }

                if (AppSettings.IsActiveSRate5 == true)
                {
                    this.dgvStockOut.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate5Per", HeaderText = "" + AppSettings.SRate5Name + " %", ReadOnly = false, Visible = false, Width = 80 }); //18
                    this.dgvStockOut.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate5", HeaderText = AppSettings.SRate5Name, ReadOnly = false, Visible = false, Width = 80 }); //19
                }
                else
                {
                    this.dgvStockOut.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate5Per", HeaderText = "" + AppSettings.SRate5Name + " %", ReadOnly = false, Visible = false, Width = 80 }); //18
                    this.dgvStockOut.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate5", HeaderText = AppSettings.SRate5Name, ReadOnly = false, Visible = false, Width = 80 }); //19
                }
            }
            else
            {
                this.dgvStockOut.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate1Per", HeaderText = "" + AppSettings.SRate1Name + " %", ReadOnly = true, Visible = false, Width = 80 }); //10
                this.dgvStockOut.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate1", HeaderText = "" + AppSettings.SRate1Name + "", ReadOnly = true, Visible = false, Width = 80 }); //11
                if (AppSettings.IsActiveSRate2 == true)
                {
                    this.dgvStockOut.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate2Per", HeaderText = "" + AppSettings.SRate2Name + " %", ReadOnly = true, Visible = false, Width = 80 }); //12
                    this.dgvStockOut.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate2", HeaderText = "" + AppSettings.SRate2Name + "", ReadOnly = true, Visible = false, Width = 80 }); //13
                }
                else
                {
                    this.dgvStockOut.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate2Per", HeaderText = "" + AppSettings.SRate2Name + " %", ReadOnly = true, Visible = false, Width = 80 }); //12
                    this.dgvStockOut.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate2", HeaderText = "" + AppSettings.SRate2Name + "", ReadOnly = true, Visible = false, Width = 80 }); //13
                }

                if (AppSettings.IsActiveSRate3 == true)
                {
                    this.dgvStockOut.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate3Per", HeaderText = "" + AppSettings.SRate3Name + " %", ReadOnly = true, Visible = false, Width = 80 }); //14
                    this.dgvStockOut.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate3", HeaderText = AppSettings.SRate3Name, ReadOnly = true, Visible = false, Width = 80 }); //15
                }
                else
                {
                    this.dgvStockOut.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate3Per", HeaderText = "" + AppSettings.SRate3Name + " %", ReadOnly = true, Visible = false, Width = 80 }); //14
                    this.dgvStockOut.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate3", HeaderText = AppSettings.SRate3Name, ReadOnly = true, Visible = false, Width = 80 }); //15
                }

                if (AppSettings.IsActiveSRate4 == true)
                {
                    this.dgvStockOut.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate4Per", HeaderText = "" + AppSettings.SRate4Name + " %", ReadOnly = true, Visible = false, Width = 80 }); //16
                    this.dgvStockOut.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate4", HeaderText = AppSettings.SRate4Name, ReadOnly = true, Visible = false, Width = 80 }); //17
                }
                else
                {
                    this.dgvStockOut.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate4Per", HeaderText = "" + AppSettings.SRate4Name + " %", ReadOnly = true, Visible = false, Width = 80 }); //16
                    this.dgvStockOut.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate4", HeaderText = AppSettings.SRate4Name, ReadOnly = true, Visible = false, Width = 80 }); //17
                }

                if (AppSettings.IsActiveSRate5 == true)
                {
                    this.dgvStockOut.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate5Per", HeaderText = "" + AppSettings.SRate5Name + " %", ReadOnly = true, Visible = false, Width = 80 }); //18
                    this.dgvStockOut.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate5", HeaderText = AppSettings.SRate5Name, ReadOnly = true, Visible = false, Width = 80 }); //19
                }
                else
                {
                    this.dgvStockOut.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate5Per", HeaderText = "" + AppSettings.SRate5Name + " %", ReadOnly = true, Visible = false, Width = 80 }); //18
                    this.dgvStockOut.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSRate5", HeaderText = AppSettings.SRate5Name, ReadOnly = true, Visible = false, Width = 80 }); //19
                }
            }
            this.dgvStockOut.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cGrossAmt", HeaderText = "Gross Amt", ReadOnly = true, Width = 80 }); //23

            try
            {
                if (clsVchType == null)
                {
                    MessageBox.Show("Voucher settings incorrect for the voucher. Please correct the settings and open the voucher again.", "Sales Settings", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            catch (Exception ex)
            {

            }

            if (clsVchType != null)
            {
                if (clsVchType.blnItmWiseDiscPercentageandAmt == 1)
                {
                    this.dgvStockOut.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cDiscPer", HeaderText = "Discount %", ReadOnly = false, Width = 80 }); //24
                    this.dgvStockOut.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cDiscAmount", HeaderText = "Discount Amt", ReadOnly = false, Width = 80 }); //25
                }
                else
                {
                    if (clsVchType.blnItmWiseDiscPercentage == 1)
                        this.dgvStockOut.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cDiscPer", HeaderText = "Discount %", ReadOnly = false, Width = 80 }); //24
                    else
                        this.dgvStockOut.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cDiscPer", HeaderText = "Discount %", ReadOnly = true, Width = 80 }); //24

                    if (clsVchType.blnItmWiseDiscAmount == 1)
                        this.dgvStockOut.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cDiscAmount", HeaderText = "Discount Amt", ReadOnly = false, Width = 80 }); //25
                    else
                        this.dgvStockOut.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cDiscAmount", HeaderText = "Discount Amt", ReadOnly = true, Width = 80 }); //25
                }
            }
            this.dgvStockOut.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cBillDisc", HeaderText = "Bill Discount", Width = 80, ReadOnly = true }); //26
            this.dgvStockOut.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cCrate", HeaderText = "CRate", Width = 80, ReadOnly = true }); //27

            if (AppSettings.TaxMode == 2) //GST
                this.dgvStockOut.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cCRateWithTax", HeaderText = "CRate With Tax", Width = 80, ReadOnly = true }); //28
            else
                this.dgvStockOut.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cCRateWithTax", HeaderText = "CRate With Tax", Visible = false, Width = 80, ReadOnly = true }); //28

            this.dgvStockOut.Columns.Add(new DataGridViewTextBoxColumn() { Name = "ctaxable", HeaderText = "Taxable", Width = 80, ReadOnly = true }); //29

            if (AppSettings.TaxMode == 2) //GST
            {
                if (clsVchTypeFeatures.blnEditTaxPer == true)
                    this.dgvStockOut.Columns.Add(new DataGridViewTextBoxColumn() { Name = "ctaxPer", HeaderText = "Tax %", ReadOnly = false, Width = 80 }); //30
                else
                    this.dgvStockOut.Columns.Add(new DataGridViewTextBoxColumn() { Name = "ctaxPer", HeaderText = "Tax %", ReadOnly = true, Width = 80 }); //30

                this.dgvStockOut.Columns.Add(new DataGridViewTextBoxColumn() { Name = "ctax", HeaderText = "Tax", Width = 80, ReadOnly = true }); //31
                this.dgvStockOut.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cIGST", HeaderText = "IGST", Width = 80, ReadOnly = true }); //32
                this.dgvStockOut.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSGST", HeaderText = "SGST", Width = 80, ReadOnly = true }); //33
                this.dgvStockOut.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cCGST", HeaderText = "CGST", Width = 80, ReadOnly = true }); //34
            }
            else
            {
                if (clsVchTypeFeatures.blnEditTaxPer == true)
                    this.dgvStockOut.Columns.Add(new DataGridViewTextBoxColumn() { Name = "ctaxPer", HeaderText = "Tax %", ReadOnly = false, Visible = false, Width = 80 }); //30
                else
                    this.dgvStockOut.Columns.Add(new DataGridViewTextBoxColumn() { Name = "ctaxPer", HeaderText = "Tax %", ReadOnly = true, Visible = false, Width = 80 }); //30

                this.dgvStockOut.Columns.Add(new DataGridViewTextBoxColumn() { Name = "ctax", HeaderText = "Tax", Visible = false, Width = 80, ReadOnly = true }); //31
                this.dgvStockOut.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cIGST", HeaderText = "IGST", Visible = false, Width = 80, ReadOnly = true }); //32
                this.dgvStockOut.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSGST", HeaderText = "SGST", Visible = false, Width = 80, ReadOnly = true }); //33
                this.dgvStockOut.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cCGST", HeaderText = "CGST", Visible = false, Width = 80, ReadOnly = true }); //34
            }


            this.dgvStockOut.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cNetAmount", HeaderText = "Net Amt", Width = 100, ReadOnly = true }); //35
            this.dgvStockOut.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cItemID", HeaderText = "ItemID", Visible = false, Width = 80, ReadOnly = true }); //36

            this.dgvStockOut.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cGrossValueAfterRateDiscount", HeaderText = "Gross Val", Visible = false, ReadOnly = true }); //37
            this.dgvStockOut.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cNonTaxable", HeaderText = "Non Taxable", Visible = false, ReadOnly = true }); //38
            this.dgvStockOut.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cCCessPer", HeaderText = "Cess %", Visible = false, ReadOnly = true }); //39
            this.dgvStockOut.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cCCompCessQty", HeaderText = "Comp Cess Qty", Visible = false, ReadOnly = true }); //40
            this.dgvStockOut.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cFloodCessPer", HeaderText = "Flood Cess %", Visible = false, ReadOnly = true }); //41
            this.dgvStockOut.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cFloodCessAmt", HeaderText = "Flood Cess Amt", Visible = false, ReadOnly = true }); //42
            this.dgvStockOut.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cStockMRP", HeaderText = "Stock MRP", Visible = false, ReadOnly = true }); //43
            this.dgvStockOut.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cAgentCommPer", HeaderText = "Agent Comm. %", Visible = false, ReadOnly = true }); //44
            this.dgvStockOut.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cCoolie", HeaderText = "Coolie", Visible = false, ReadOnly = true }); //45
            this.dgvStockOut.Columns.Add(new DataGridViewCheckBoxColumn() { Name = "cBlnOfferItem", HeaderText = "Offer Item", Visible = false, ReadOnly = true }); //46
            this.dgvStockOut.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cStrOfferDetails", HeaderText = "Offer Det.", Visible = false, ReadOnly = true }); //47
            this.dgvStockOut.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cBatchMode", HeaderText = "Batch Mode", Visible = false, ReadOnly = true }); //48
            this.dgvStockOut.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cID", HeaderText = "ID", Visible = false, ReadOnly = true });
            this.dgvStockOut.Columns.Add(new DataGridViewImageColumn() { Name = "cImgDel", HeaderText = "", Image = Properties.Resources.Delete_24_P4, Width = 40, ReadOnly = true });
            this.dgvStockOut.Columns.Add(new DataGridViewImageColumn() { Name = "cBatchUnique", HeaderText = "", Image = Properties.Resources.Delete_24_P4, Width = 40, Visible = false, ReadOnly = true });

            //Dipoos 21-03-2022
            //if (iIDFromEditWindow==0)
            //dgvStockOut.Rows.Add(2);
            //else

            dgvStockOut.Rows.Add(1);

            foreach (DataGridViewRow row in dgvStockOut.Rows)
            {
                dgvStockOut.Rows[row.Index].Cells[0].Value = string.Format("{0}  ", row.Index + 1).ToString();
            }

            foreach (DataGridViewColumn col in dgvStockOut.Columns)
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
                if (dtJson.Rows.Count > 0)
                    strJson = dtJson.Rows[0][0].ToString();

                if (strJson != "")
                {
                    List<clsJsonPurGridSettingsInfo> lstJPDGSinfo_ = JsonConvert.DeserializeObject<List<clsJsonPurGridSettingsInfo>>(strJson);
                    DataTable dtGridSettings = lstJPDGSinfo_.ToDataTable();
                    if (dtGridSettings.Rows.Count > 0)
                    {
                        for (int k = 0; k < dtGridSettings.Rows.Count; k++)
                        {
                            if (dtGridSettings.Rows[k][3].ToString() == dgvStockOut.Columns[k].Name)
                            {
                                if (dgvStockOut.Columns[k].Name.ToUpper().Trim() == "CFREE")
                                {
                                    if (clsVchTypeFeatures.BLNSHOWFREEQUANTITY == false)
                                    {
                                        dgvStockOut.Columns[k].Width = Convert.ToInt32(dtGridSettings.Rows[k][2].ToString());
                                        dgvStockOut.Columns[k].Visible = false;
                                    }
                                }
                                else if (dgvStockOut.Columns[k].Name.ToUpper().Trim() == "ID")
                                {
                                    dgvStockOut.Columns[k].Width = Convert.ToInt32(dtGridSettings.Rows[k][2].ToString());
                                    dgvStockOut.Columns[k].Visible = false;
                                }
                                else if (dgvStockOut.Columns[k].Name.ToUpper().Trim() == "ItemID")
                                {
                                    dgvStockOut.Columns[k].Width = Convert.ToInt32(dtGridSettings.Rows[k][2].ToString());
                                    dgvStockOut.Columns[k].Visible = false;
                                }
                                else if (dgvStockOut.Columns[k].Name.ToUpper().Trim() == "CSRATE1PER" || dgvStockOut.Columns[k].Name.ToUpper().Trim() == "CSRATE1")
                                {
                                    dgvStockOut.Columns[k].Width = Convert.ToInt32(dtGridSettings.Rows[k][2].ToString());
                                    dgvStockOut.Columns[k].Visible = false;
                                }
                                else if (dgvStockOut.Columns[k].Name.ToUpper().Trim() == "CSRATE2PER" || dgvStockOut.Columns[k].Name.ToUpper().Trim() == "CSRATE2")
                                {
                                    dgvStockOut.Columns[k].Width = Convert.ToInt32(dtGridSettings.Rows[k][2].ToString());
                                    dgvStockOut.Columns[k].Visible = false;
                                }
                                else if (dgvStockOut.Columns[k].Name.ToUpper().Trim() == "CSRATE3PER" || dgvStockOut.Columns[k].Name.ToUpper().Trim() == "CSRATE3")
                                {
                                    dgvStockOut.Columns[k].Width = Convert.ToInt32(dtGridSettings.Rows[k][2].ToString());
                                    dgvStockOut.Columns[k].Visible = false;
                                }
                                else if (dgvStockOut.Columns[k].Name.ToUpper().Trim() == "CSRATE4PER" || dgvStockOut.Columns[k].Name.ToUpper().Trim() == "CSRATE4")
                                {
                                    dgvStockOut.Columns[k].Width = Convert.ToInt32(dtGridSettings.Rows[k][2].ToString());
                                    dgvStockOut.Columns[k].Visible = false;
                                }
                                else if (dgvStockOut.Columns[k].Name.ToUpper().Trim() == "CSRATE5PER" || dgvStockOut.Columns[k].Name.ToUpper().Trim() == "CSRATE5")
                                {
                                    dgvStockOut.Columns[k].Width = Convert.ToInt32(dtGridSettings.Rows[k][2].ToString());
                                    dgvStockOut.Columns[k].Visible = false;
                                }
                                else
                                {
                                    dgvStockOut.Columns[k].Width = Convert.ToInt32(dtGridSettings.Rows[k][2].ToString());
                                    dgvStockOut.Columns[k].Visible = Convert.ToBoolean(dtGridSettings.Rows[k][0]);
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

            dgvStockOut.Columns["cSlNo"].Frozen = true;
            //dgvStockOut.Columns["cImgDel"].Frozen = true;
            dgvStockOut.Columns["cImgDel"].Visible = true;
            dgvStockOut.Columns["cImgDel"].Width = 40;

            dgvStockOut.Columns["cSlNo"].ReadOnly = true;
            
            dgvStockOut.Columns["cRateinclusive"].Visible = false;
            dgvStockOut.Columns["cSRate1Per"].Visible = false;
            dgvStockOut.Columns["cSRate2Per"].Visible = false;
            dgvStockOut.Columns["cSRate3Per"].Visible = false;
            dgvStockOut.Columns["cSRate4Per"].Visible = false;
            dgvStockOut.Columns["cSRate5Per"].Visible = false;
            dgvStockOut.Columns["cSRate1"].Visible = false;
            dgvStockOut.Columns["cSRate2"].Visible = false;
            dgvStockOut.Columns["cSRate3"].Visible = false;
            dgvStockOut.Columns["cSRate4"].Visible = false;
            dgvStockOut.Columns["cSRate5"].Visible = false;

            

            //DisableGridSettingsCheckbox();
        }

        private void txtDiscPerc_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                e.Handled = Comm.CheckNumeric(sender, e);

                dSteadyBillDiscPerc = Comm.ToDecimal(txtDiscPerc.Text);
                dSteadyBillDiscAmt = 0;
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            txtOtherExp.Select(0, txtOtherExp.Text.Length - 1);
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

                dSteadyBillDiscAmt = Comm.ToDecimal(txtDiscAmt.Text);
                dSteadyBillDiscPerc = 0;
            }
            catch
            {

            }
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
                    if (Convert.ToDecimal(txtDiscAmt.Text) > 0)
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
            if (cboPayment.SelectedIndex == 1)
            {
                txtInstantReceipt.Enabled = true;
                txtInstantReceipt.BackColor = Color.White;
            }
            else
            {
                txtInstantReceipt.Text = "";
                txtInstantReceipt.Enabled = false;
                txtInstantReceipt.BackColor = Color.Gray;
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

        private void dgvStockOut_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (dgvStockOut.CurrentCell.ColumnIndex == GetEnum(gridColIndexes.CExpiry))
                {
                    if (dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(gridColIndexes.CExpiry)].ReadOnly == false)
                    {
                        _Rectangle = dgvStockOut.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true); //  
                        dtp.Size = new Size(_Rectangle.Width, _Rectangle.Height); //  
                        dtp.Location = new Point(_Rectangle.X, _Rectangle.Y); //  
                        dtp.Visible = true;
                        dtp.TextChanged += new EventHandler(dtp_TextChange);
                    }
                }
                if (dgvStockOut.CurrentCell.ColumnIndex == GetEnum(gridColIndexes.cSlNo))
                {
                    dgvStockOut.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                    dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cSlNo)].ReadOnly = true;

                    strSelectedItemName = Convert.ToString(dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(gridColIndexes.CItemCode)].Value);
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
            if (dgvStockOut.CurrentCell.ColumnIndex == GetEnum(gridColIndexes.cImgDel))
            {
                string SSelectedItemCode = Convert.ToString(dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(gridColIndexes.CItemCode)].Value);
                if (SSelectedItemCode != "")
                {
                    DialogResult dlgResult = MessageBox.Show("Are you sure to delete Item Code[" + SSelectedItemCode + "] Permanently ?", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                    if (dlgResult == DialogResult.Yes)
                    {

                        Int32 selectedRowCount = dgvStockOut.Rows.GetRowCount(DataGridViewElementStates.Selected);
                        RowDelete();

                        dgvStockOut.Rows.Add();
                        dgvStockOut.CurrentCell = dgvStockOut.Rows[0].Cells[1];

                        CalcTotal();
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
                if (dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(gridColIndexes.CItemCode)].Value != null)
                {
                    dSelectedItemID = Convert.ToDouble(dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cItemID)].Value);
                    if (dSelectedItemID > 0)
                    {
                        if (dgvStockOut.CurrentCell.ColumnIndex == (int)gridColIndexes.CItemCode)
                        {
                            frmItemMaster frmIM = new frmItemMaster(Convert.ToInt32(dSelectedItemID), true, "E");
                            frmIM.ShowDialog();
                        }
                        else if (dgvStockOut.CurrentCell.ColumnIndex == (int)gridColIndexes.CItemName)
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

        private void dgvStockOut_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            decimal dResult = 0;
            //outoflimitbillvalue = 0;
            try
            {
                if (dgvStockOut.CurrentCell.ColumnIndex == GetEnum(gridColIndexes.cBarCode))
                {
                    if (dgvStockOut.CurrentCell.Value != null)
                    {
                        if (GetFromBarcodeSearch(dgvStockOut.CurrentCell.Value.ToString()) == false)
                        {
                            //CallForBatchSearch(dgvStockOut.CurrentCell.Value.ToString());
                            string sQuery = "SELECT TOP 200 ItemCode + ItemName + BatchUnique + CAST(MRP AS varchar) AS searchanywhere, ItemCode, ItemName, BatchUnique, QOH, MRP, ItemID, StockID " +
                                   " FROM     vwCompactSearch Where isnull(ActiveStatus, 1) = 1 and isnull(StockActiveStatus, 1) = 1 and BatchUnique <> '<AUTO BARCODE>' ";
                            if (clsVchType.ProductClassList != "")
                                sQuery = sQuery + " AND ProductTypeID IN (" + clsVchType.ProductClassList + ")";
                            if (clsVchType.ItemCategoriesList != "")
                                sQuery = sQuery + " AND CategoryID IN (" + clsVchType.ItemCategoriesList + ")";

                            if (mblnInitialisedSubWindow == false)
                            {
                                mblnInitialisedSubWindow = true;
                                if (dgvStockOut.CurrentCell.Value == null) dgvStockOut.CurrentCell.Value = "";
                                frmDetailedSearch2 frmN = new frmDetailedSearch2(GetFromItemSearch, sQuery, "Anywhere|ItemCode|ItemName|BatchUnique", dgvStockOut.Location.X + 55, dgvStockOut.Location.Y + 150, 7, 0, dgvStockOut.CurrentCell.Value.ToString(), 6, 0, "ORDER BY ItemCode ASC, Batchunique", 0, 0, "Item Search...", 0, "150,250,150,150,150,0,0", true, "frmItemMaster", 20, false, this.MdiParent, 6);
                                frmN.Show(); //12-SEP-2022
                            }
                        }
                        else
                        {
                            if (dgvStockOut.Rows.Count - 1 == dgvStockOut.CurrentRow.Index)
                                if (dgvStockOut[gridColIndexes.CItemName, dgvStockOut.CurrentRow.Index].Tag != null)
                                    if (Convert.ToInt32(dgvStockOut[gridColIndexes.CItemName, dgvStockOut.CurrentRow.Index].Tag) > 0)
                                        dgvStockOut.Rows.Add();

                            if (clsVchType.blnMovetoNextRowAfterSelection != 1)
                            {
                                //dgvStockOut.CurrentCell = dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(gridColIndexes.CExpiry)];
                                dgvStockOut.CurrentCell = dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cQty)];
                            }
                            else
                            {
                                dgvStockOut.CurrentCell = dgvStockOut.Rows[dgvStockOut.CurrentRow.Index + 1].Cells[1];
                            }
                            dgvStockOut.Focus();
                        }
                    }
                }
                else if (dgvStockOut.CurrentCell.ColumnIndex == GetEnum(gridColIndexes.cQty))
                {
                    dResult = Convert.ToDecimal(dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cQty)].Value);
                    SetValue(GetEnum(gridColIndexes.cQty), dResult.ToString(), "QTY_FLOAT");
                    SendKeys.Send("{Tab}");

                    if (dgvStockOut.Rows.Count - 1 == dgvStockOut.CurrentRow.Index)
                        dgvStockOut.Rows.Add();

                    //Added by Anjitha 28/01/2022 5:30 PM
                    bool bshellife = ShelfLifeEffect();
                    if (bshellife == false)
                    {
                        dgvStockOut.Focus();
                        SetValue(GetEnum(gridColIndexes.CExpiry), dtCurrExp.ToString("dd-MMM-yyyy").ToString(), "DATE");
                    }
                }
                else if (dgvStockOut.CurrentCell.ColumnIndex == GetEnum(gridColIndexes.cFree))
                {
                    dResult = Convert.ToDecimal(dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cFree)].Value);
                    SetValue(GetEnum(gridColIndexes.cFree), dResult.ToString(), "QTY_FLOAT");

                    SendKeys.Send("{up}");
                    SendKeys.Send("{right}");
                }
                else if (dgvStockOut.CurrentCell.ColumnIndex == GetEnum(gridColIndexes.cDiscPer))
                {
                    dResult = Convert.ToDecimal(dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cGrossAmt)].Value) * (Convert.ToDecimal(dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cDiscPer)].Value) / 100);
                    SetValue(GetEnum(gridColIndexes.cDiscAmount), dResult.ToString(), "CURR_FLOAT");
                }
                else if (dgvStockOut.CurrentCell.ColumnIndex == GetEnum(gridColIndexes.cDiscAmount))
                {
                    dResult = (Convert.ToDecimal(dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cDiscAmount)].Value) * 100) / Convert.ToDecimal(dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cGrossAmt)].Value);
                    SetValue(GetEnum(gridColIndexes.cDiscPer), dResult.ToString(), "PERC_FLOAT");
                }
                else if (dgvStockOut.CurrentCell.ColumnIndex == GetEnum(gridColIndexes.cMRP))
                {
                    SendKeys.Send("{up}");
                    SendKeys.Send("{right}");
                }
                else if (dgvStockOut.CurrentCell.ColumnIndex == GetEnum(gridColIndexes.cSrate))
                {
                    SendKeys.Send("{up}");
                    SendKeys.Send("{right}");
                }
                this.dgvEndEditCell = dgvStockOut[e.ColumnIndex, e.RowIndex];
                if (dgvStockOut.Rows.Count == e.RowIndex && e.ColumnIndex != dgvStockOut.Columns.Count - 1 && e.ColumnIndex <= GetEnum(gridColIndexes.cDiscAmount))
                {
                    if (dgvStockOut.CurrentCell.ColumnIndex != GetEnum(gridColIndexes.cRateinclusive))
                        SendKeys.Send("{Tab}");
                }
                else if (e.ColumnIndex == GetEnum(gridColIndexes.cDiscAmount))
                {
                    dgvStockOut.CurrentCell = dgvStockOut[GetEnum(gridColIndexes.CItemCode), dgvStockOut.CurrentRow.Index + 1];
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

        private void dgvStockOut_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                //Added by Dipu Joseph on 14-Feb-2021 5.08 PM ---------- >>
                int iRow = 0;

                if (dgvStockOut.CurrentCell != null)
                {
                    int iColumn = dgvStockOut.CurrentCell.ColumnIndex;
                    int iRowNo = dgvStockOut.CurrentCell.RowIndex;
                    
                    if (iColumn == 0) iColumn = 1;

                    if (this._EnterMoveNext && MouseButtons == 0)
                    {
                        if (this.dgvEndEditCell != null && dgvStockOut.CurrentCell != null)
                        {
                            if (dgvStockOut.CurrentCell.RowIndex == this.dgvEndEditCell.RowIndex + 1
                                && dgvStockOut.CurrentCell.ColumnIndex == this.dgvEndEditCell.ColumnIndex)
                            {
                                int iColNew;
                                int iRowNew;
                                if (this.dgvEndEditCell.ColumnIndex >= dgvStockOut.ColumnCount - 1)
                                {
                                    iColNew = 0;
                                    iRowNew = dgvStockOut.CurrentCell.RowIndex;
                                }
                                else
                                {
                                    iColNew = this.dgvEndEditCell.ColumnIndex + 1;
                                    iRow = this.dgvEndEditCell.RowIndex;
                                }

                                if (iColumn >= dgvStockOut.Columns.Count - 2)
                                    dgvStockOut.CurrentCell = dgvStockOut[iColumn, iRowNo + 1];
                                else
                                {
                                    if (iColumn == GetEnum(gridColIndexes.cSrate))
                                    {
                                        SendKeys.Send("{Tab}");
                                        //dgvStockOut.CurrentCell = dgvStockOut[iColumn + 1, iRow];
                                    }
                                    else if (iColumn == GetEnum(gridColIndexes.cMRP))
                                    {
                                        SendKeys.Send("{Tab}");
                                        //dgvStockOut.CurrentCell = dgvStockOut[iColumn + 1, iRow];
                                    }
                                    else if (iColumn == GetEnum(gridColIndexes.cQty))
                                    {
                                        if (iRow < 0)
                                        {
                                            iRow = 0;

                                            if (dgvStockOut.Rows.Count <= iRow + 1)
                                                dgvStockOut.Rows.Add();

                                            if (clsVchTypeFeatures.BLNSHOWFREEQUANTITY == true)
                                                dgvStockOut.CurrentCell = dgvStockOut[GetEnum(gridColIndexes.cFree), iRow];
                                            else if (dgvStockOut.Columns[GetEnum(gridColIndexes.cDiscPer)].Visible == true)
                                                dgvStockOut.CurrentCell = dgvStockOut[GetEnum(gridColIndexes.cDiscPer), iRow];
                                            else if (dgvStockOut.Columns[GetEnum(gridColIndexes.cDiscAmount)].Visible == true)
                                                dgvStockOut.CurrentCell = dgvStockOut[GetEnum(gridColIndexes.cDiscAmount), iRow];
                                            else if (GetEnum(gridColIndexes.CItemCode) == 1)
                                                dgvStockOut.CurrentCell = dgvStockOut[GetEnum(gridColIndexes.CItemCode), iRow + 1];
                                            else if (GetEnum(gridColIndexes.cBarCode) == 1)
                                                dgvStockOut.CurrentCell = dgvStockOut[GetEnum(gridColIndexes.cBarCode), iRow + 1];
                                        }
                                        else
                                        {
                                            if (clsVchTypeFeatures.BLNSHOWFREEQUANTITY == true)
                                                dgvStockOut.CurrentCell = dgvStockOut[GetEnum(gridColIndexes.cFree), iRow];
                                            else
                                            {
                                                if (dgvStockOut.Rows.Count <= iRow + 1)
                                                    dgvStockOut.Rows.Add();

                                                if (clsVchTypeFeatures.BLNSHOWFREEQUANTITY == true)
                                                    dgvStockOut.CurrentCell = dgvStockOut[GetEnum(gridColIndexes.cFree), iRow];
                                                else if (dgvStockOut.Columns[GetEnum(gridColIndexes.cDiscPer)].Visible == true)
                                                    dgvStockOut.CurrentCell = dgvStockOut[GetEnum(gridColIndexes.cDiscPer), iRow];
                                                else if (dgvStockOut.Columns[GetEnum(gridColIndexes.cDiscAmount)].Visible == true)
                                                    dgvStockOut.CurrentCell = dgvStockOut[GetEnum(gridColIndexes.cDiscAmount), iRow];
                                                else if (GetEnum(gridColIndexes.CItemCode) == 1)
                                                    dgvStockOut.CurrentCell = dgvStockOut[GetEnum(gridColIndexes.CItemCode), iRow + 1];
                                                else if (GetEnum(gridColIndexes.cBarCode) == 1)
                                                    dgvStockOut.CurrentCell = dgvStockOut[GetEnum(gridColIndexes.cBarCode), iRow + 1];

                                                //dgvStockOut.CurrentCell = dgvStockOut[iColumn + 2, iRow];
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

        private void dgvStockOut_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {

        }

        private void dgvStockOut_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {

        }

        private void dgvStockOut_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
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

        private void dgvStockOut_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {

        }

        private void dgvStockOut_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            try
            {
                //outoflimitbillvalue = 0;
                if (dgvStockOut.CurrentCell.EditType == typeof(DataGridViewTextBoxEditingControl))
                {
                    if (dgvStockOut.CurrentCell.ColumnIndex == GetEnum(gridColIndexes.CItemCode))
                    {
                        DataGridViewTextBoxEditingControl tb = (DataGridViewTextBoxEditingControl)e.Control;
                        tb.KeyPress += new KeyPressEventHandler(dgvStockOut_TextBox_KeyPress);
                        e.Control.KeyPress += new KeyPressEventHandler(dgvStockOut_TextBox_KeyPress);
                    }
                    else if (dgvStockOut.CurrentCell.ColumnIndex == GetEnum(gridColIndexes.cBarCode))
                    {
                        if (clsVchType.DefaultBarcodeMode != 0)
                        {
                            DataGridViewTextBoxEditingControl tb = (DataGridViewTextBoxEditingControl)e.Control;
                            tb.KeyPress += new KeyPressEventHandler(dgvStockOut_TextBox_KeyPress);
                            e.Control.KeyPress += new KeyPressEventHandler(dgvStockOut_TextBox_KeyPress);
                        }
                        else
                        {
                            CallBatchCodeCompact();

                            dgvStockOut.CurrentCell = dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(gridColIndexes.CExpiry)];
                            dgvStockOut.Focus();

                        }
                    }
                    else if (dgvStockOut.CurrentCell.ColumnIndex >= GetEnum(gridColIndexes.cMRP) && dgvStockOut.CurrentCell.ColumnIndex < GetEnum(gridColIndexes.cNetAmount))
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
                if (dgvStockOut.CurrentCell == null) return;

                int iRow = dgvStockOut.CurrentCell.RowIndex;
                //if (dgvStockOut.Rows.Count <= iRow + 1)
                //    dgvStockOut.Rows.Add();

                if (e.KeyCode == Keys.Shift && e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    int iColumn = dgvStockOut.CurrentCell.ColumnIndex;
                    iRow = dgvStockOut.CurrentCell.RowIndex;
                    if (iColumn == GetEnum(gridColIndexes.cRateinclusive))
                    {
                        dgvStockOut.CurrentCell = dgvStockOut[GetEnum(gridColIndexes.cRateinclusive) - 1, iRow];
                    }
                    else if (iColumn == dgvStockOut.Columns.Count - 1)//&& iRow != dgvStockOut.Rows.Count
                    {
                        if (dgvStockOut.Rows.Count <= iRow + 1)
                            dgvStockOut.Rows.Add();
                        dgvStockOut.CurrentCell = dgvStockOut[1, iRow - 1];
                    }
                    else
                        SendKeys.Send("+{Tab}");
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    int iColumn = dgvStockOut.CurrentCell.ColumnIndex;
                    iRow = dgvStockOut.CurrentCell.RowIndex;
                    if (iColumn == dgvStockOut.Columns.Count - 1 && iRow != dgvStockOut.Rows.Count)
                    {
                        dgvStockOut.CurrentCell = dgvStockOut[1, iRow + 1];
                    }
                    else if (iColumn == dgvStockOut.Columns.Count - 1 && iRow == dgvStockOut.Rows.Count)
                    {
                    }
                    else if (iColumn == GetEnum(gridColIndexes.cDiscAmount))
                    {
                        //Dipoos 22-03-2022----- >
                        if (dgvStockOut.Rows.Count <= iRow + 1)
                            dgvStockOut.Rows.Add();

                        if (GetEnum(gridColIndexes.CItemCode) == 1)
                            dgvStockOut.CurrentCell = dgvStockOut[GetEnum(gridColIndexes.CItemCode), iRow + 1];
                        if (GetEnum(gridColIndexes.cBarCode) == 1)
                            dgvStockOut.CurrentCell = dgvStockOut[GetEnum(gridColIndexes.cBarCode), iRow + 1];
                    }
                    else if (iColumn == GetEnum(gridColIndexes.cRateinclusive))
                    {
                        dgvStockOut.CurrentCell = dgvStockOut[GetEnum(gridColIndexes.cRateinclusive) + 1, iRow];
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
                    if (dgvStockOut.CurrentCell.ColumnIndex == (int)gridColIndexes.CItemCode)
                    {
                        frmItemMaster frmim = new frmItemMaster(0, true, "S");
                        frmim.ShowDialog();
                    }
                }
                else if (e.KeyCode == Keys.F4)
                {
                    if (dgvStockOut.CurrentCell.ColumnIndex == (int)gridColIndexes.CItemCode)
                    {
                        int iSelectedItemID = 0;
                        iSelectedItemID = Convert.ToInt32(dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cItemID)].Value);
                        if (iSelectedItemID > 0)
                        {
                            frmItemMaster frmIM = new frmItemMaster(iSelectedItemID, true, "E");
                            frmIM.ShowDialog();
                        }

                    }
                }
                else if (e.KeyCode == Keys.Delete)
                {
                    string SSelectedItemCode = Convert.ToString(dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(gridColIndexes.CItemCode)].Value);
                    if ((SSelectedItemCode != "" || dgvStockOut.Rows.Count > 1) && dgvStockOut.CurrentRow.Index >= 0)
                    {
                        DialogResult dlgResult = MessageBox.Show("Are you sure to delete Item Code[" + SSelectedItemCode + "] Permanently ?", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                        if (dlgResult == DialogResult.Yes)
                        {
                            Int32 selectedRowCount = dgvStockOut.Rows.GetRowCount(DataGridViewElementStates.Selected);
                            RowDelete();
                            //dipoos 21-03-2022
                            //if (dgvStockOut.Rows.Count < 2)
                            //    dgvStockOut.Rows.Add();
                            if (dgvStockOut.Rows.Count < 1)
                                dgvStockOut.Rows.Add();

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

                    if (dgvStockOut.CurrentCell.ColumnIndex == (int)gridColIndexes.CItemCode)
                    {
                        if (sEditedValueonKeyPress != null)
                        {
                            if (AppSettings.TaxMode == 2) //GST
                            {
                                sQuery = "SELECT TOP 200 ItemCode + ItemName + BatchUnique + CAST(MRP AS varchar) AS searchanywhere, ItemCode, ItemName, BatchUnique, QOH, MRP, ItemID, StockID " +
                                        " FROM     vwCompactSearch Where isnull(ActiveStatus, 1) = 1 and isnull(StockActiveStatus, 1) = 1 and BatchUnique <> '<AUTO BARCODE>' ";
                                if (clsVchType.ProductClassList != "")
                                    sQuery = sQuery + " AND ProductTypeID IN (" + clsVchType.ProductClassList + ")";
                                if (clsVchType.ItemCategoriesList != "")
                                    sQuery = sQuery + " AND CategoryID IN (" + clsVchType.ItemCategoriesList + ")";

                                if (mblnInitialisedSubWindow == false)
                                {
                                    mblnInitialisedSubWindow = true;
                                    if (dgvStockOut.CurrentCell.Value == null) dgvStockOut.CurrentCell.Value = "";
                                    frmDetailedSearch2 frmN = new frmDetailedSearch2(GetFromItemSearch, sQuery, "Anywhere|ItemCode|ItemName|BatchUnique", dgvStockOut.Location.X + 55, dgvStockOut.Location.Y + 150, 7, 0, dgvStockOut.CurrentCell.Value.ToString(), 6, 0, "ORDER BY ItemCode ASC, Batchunique", 0, 0, "Item Search...", 0, "150,250,150,150,150,0,0", true, "frmItemMaster", 20, false, this.MdiParent, 6);
                                    frmN.Show(); //12-SEP-2022
                                }
                            }
                            else
                            {
                                sQuery = "SELECT TOP 200 ItemCode + ItemName + BatchUnique + CAST(MRP AS varchar) AS searchanywhere, ItemCode, ItemName, BatchUnique, QOH, MRP, ItemID, StockID " +
                                        " FROM     vwCompactSearch Where isnull(ActiveStatus, 1) = 1 and isnull(StockActiveStatus, 1) = 1 and BatchUnique <> '<AUTO BARCODE>' ";
                                if (clsVchType.ProductClassList != "")
                                    sQuery = sQuery + " AND ProductTypeID IN (" + clsVchType.ProductClassList + ")";
                                if (clsVchType.ItemCategoriesList != "")
                                    sQuery = sQuery + " AND CategoryID IN (" + clsVchType.ItemCategoriesList + ")";


                                if (mblnInitialisedSubWindow == false)
                                {
                                    mblnInitialisedSubWindow = true;
                                    if (dgvStockOut.CurrentCell.Value == null) dgvStockOut.CurrentCell.Value = "";
                                    frmDetailedSearch2 frmN = new frmDetailedSearch2(GetFromItemSearch, sQuery, "Anywhere|ItemCode|ItemName|BatchUnique", dgvStockOut.Location.X + 55, dgvStockOut.Location.Y + 150, 7, 0, dgvStockOut.CurrentCell.Value.ToString(), 6, 0, "ORDER BY ItemCode ASC, Batchunique", 0, 0, "Item Search...", 0, "150,250,150,150,150,0,0", true, "frmItemMaster", 20, false, this.MdiParent, 6);
                                    frmN.Show(); //12-SEP-2022
                                }
                            }


                            if (dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cItemID)].Value != null)
                            {
                                this.dgvStockOut.EditingControlShowing -= this.dgvStockOut_EditingControlShowing;
                                dgvStockOut.CurrentCell = dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cBarCode)];
                                dgvStockOut.Focus();
                                this.dgvStockOut.EditingControlShowing += this.dgvStockOut_EditingControlShowing;
                            }
                        }
                    }
                    else if (dgvStockOut.CurrentCell.ColumnIndex == GetEnum(gridColIndexes.cBarCode))
                    {
                        Form fc = Application.OpenForms["frmDetailedSearch2"];
                        if (fc != null)
                        {
                            fcc.Focus();
                            fcc.BringToFront();
                            return;
                        }
                        // BatchCode List Will Work only to MNF and Auto BatchMode Cases... Asper Discuss with Anup sir and Team on 13-May-2022 Evening Meeting.
                        if (Convert.ToInt32(dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cBatchMode)].Value) == 1) // MNF
                            CallBatchCodeCompact(true);
                        else if (Convert.ToInt32(dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cBatchMode)].Value) == 2) // Auto
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

        private void dgvStockOut_KeyPress(object sender, KeyPressEventArgs e)
        {
            string sQuery = "";
            try
            {
                //Dipoos 05-May-2022
                //sEditedValueonKeyPress = e.KeyChar.ToString();
                //if (dgvStockOut.CurrentCell.ColumnIndex == (int)gridColIndexes.CItemCode)
                //{
                //    if (Comm.ConvertI32(clsVchType.btnShowItmSearchByDefault) == 1)
                //    {
                //        if (sEditedValueonKeyPress != null)
                //        {
                //            if (AppSettings.TaxMode == 2) //GST
                //            {
                //                sQuery = "SELECT (ItemCode+ItemName+CONVERT(VARCHAR,ISNULL(IGSTTaxPer,0))) as AnyWhere,ItemCode,ItemName,CONVERT(DECIMAL(18,2),IGSTTaxPer) as [GST %],ItemID,I.CategoryID FROM tblItemMaster I INNER JOIN tblCategories C ON C.CategoryID = I.CategoryID";
                //                new frmDetailedSearch(GetFromItemSearch, sQuery, "Anywhere|ItemCode|ItemName|CONVERT(VARCHAR,ISNULL(IGSTTaxPer,0))", dgvStockOut.Location.X + 50, dgvStockOut.Location.Y + 108, 3, 0, sEditedValueonKeyPress, 3, 0, "ORDER BY ItemCode ASC", 0, 0, "Item Name Search...", 0, "150,250,150,0,0", true, "frmItemMaster").ShowDialog();
                //            }
                //            else
                //            {
                //                sQuery = "SELECT (ItemCode+ItemName) as AnyWhere,ItemCode,ItemName,ItemID,I.CategoryID FROM tblItemMaster I INNER JOIN tblCategories C ON C.CategoryID = I.CategoryID";
                //                new frmDetailedSearch(GetFromItemSearch, sQuery, "Anywhere|ItemCode|ItemName", dgvStockOut.Location.X + 50, dgvStockOut.Location.Y + 108, 2, 0, sEditedValueonKeyPress, 2, 0, "ORDER BY ItemCode ASC", 0, 0, "Item Name Search...", 0, "150,250,150,0,0", true, "frmItemMaster").ShowDialog();
                //            }
                //        }
                //    }
                //}
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
        }

        private void dgvStockOut_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void dgvStockOut_MouseUp(object sender, MouseEventArgs e)
        {
            //if (toggleWidthSettings.ToggleState == Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active)
            //{
            //    GridInitialize_dgvColWidth(false);
            //    try
            //    {
            //        LoadGridWidthFromItemGrid();
            //        DisableGridSettingsCheckbox();
            //    }
            //    catch (Exception ex)
            //    {

            //    }
            //}
        }

        private void dgvStockOut_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {

        }

        private void dgvStockOut_Scroll(object sender, ScrollEventArgs e)
        {
            dtp.Visible = false;
        }

        private void cboAgent_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void cboPriceList_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetPriceListForItems();
            CalcTotal();
        }

        private void SetPriceListForItems()
        {
            try
            {
                for (int i = 0; i < dgvStockOut.Rows.Count - 1; i++)
                {
                    dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cSrate)].Value = SetPriceListForItems(i);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private decimal SetPriceListForItems(int RowNumber = 0)
        {
            DataTable dtPL = new DataTable();

            if (cboPriceList.SelectedIndex < 0 && cboPriceList.Items.Count > 0)
            {
                cboPriceList.SelectedIndex = 0;
            }

            decimal SRate = 0;

            //int itemid = Convert.ToInt32(dgvStockOut.Rows[RowNumber].Cells[GetEnum(gridColIndexes.CItemName)].Tag);
            decimal itemid = Convert.ToDecimal(dgvStockOut.Rows[RowNumber].Cells[GetEnum(gridColIndexes.cItemID)].Value);

            string batchunique = "";
            if (dgvStockOut.Rows[RowNumber].Cells[GetEnum(gridColIndexes.cBarCode)].Value != null)
            {
                batchunique = dgvStockOut.Rows[RowNumber].Cells[GetEnum(gridColIndexes.cBarCode)].Value.ToString();
            }
            if (itemid > 0 && cboPriceList.SelectedIndex >= 0)
            {
                dtPL = Comm.fnGetData("SELECT top 1 srate" + cboPriceList.SelectedValue + " FROM tblStock Where batchunique = '" + batchunique + "' and ItemID = " + itemid + " and CCID = " + cboCostCentre.SelectedValue + " and TenantID = " + Global.gblTenantID + " ").Tables[0];

                if (dtPL != null)
                {
                    if (dtPL.Rows.Count > 0)
                        SRate = Convert.ToDecimal(dtPL.Rows[0][0].ToString());
                    else
                    {
                        SRate = FetchRateFromItemMaster(RowNumber);
                    }
                }
                else
                {
                    SRate = FetchRateFromItemMaster(RowNumber);
                }

                //dgvStockOut.Rows[RowNumber].Cells[GetEnum(gridColIndexes.cSrate)].Value = SRate;
            }

            return SRate;
        }

        private decimal FetchRateFromItemMaster(int RowNumer)
        {
            decimal itemid = Convert.ToDecimal(dgvStockOut.Rows[RowNumer].Cells[GetEnum(gridColIndexes.cItemID)].Value);

            string batchunique = "";
            if (dgvStockOut.Rows[RowNumer].Cells[GetEnum(gridColIndexes.cBarCode)].Value != null)
                batchunique = dgvStockOut.Rows[RowNumer].Cells[GetEnum(gridColIndexes.cBarCode)].Value.ToString();

            DataTable dtPL = Comm.fnGetData("SELECT top 1 srate" + cboPriceList.SelectedValue + " FROM tblItemMaster Where ItemID=" + itemid + " ").Tables[0];
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

        private void comboBox7_Leave(object sender, EventArgs e)
        {

        }

        private void comboBox7_Leave_1(object sender, EventArgs e)
        {

        }


        private void comboBox8_Leave(object sender, EventArgs e)
        {

        }

        private void txtReferenceAutoNo_TextChanged(object sender, EventArgs e)
        {

        }

        private void frmRepacking_Activated(object sender, EventArgs e)
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

            lstDisableCol.Add("cSlNo");
            lstDisableCol.Add("CItemCode");
            
            if (AppSettings.BLNBARCODE == true)
                lstDisableCol.Add("cBarCode");
            
            lstDisableCol.Add("cPrate");
            lstDisableCol.Add("cSrate");
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
            { 
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                if (PrintTrans(iIDFromEditWindow.ToString()) == true)
                {
                    if (prn.Visible == true && prn.Enabled == true)
                    {
                        if (clsVchTypeFeatures.blnprintimmediately == true)
                        {
                            prn.PrintReport(clsVchType.PrintSettings, cboInvScheme1.SelectedItem.ToString(), GetNoOfItems());
                        }
                        if (clsVchTypeFeatures.blnshowpreview == true)
                        {
                            prn.BringToFront();
                            prn.Focus();
                        }
                        else
                        {
                            prn.Close();
                            prn.Dispose();
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

        private decimal GetNoOfItems()
        {
            try
            {
                decimal NoOfItems = 0;

                for (int i = 0; i < dgvStockOut.Rows.Count; i++)
                {
                    if (dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cItemID)].Value == null)
                        dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cItemID)].Value = "0";

                    if (Comm.ToDecimal(dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cItemID)].Value.ToString()) != 0)
                    {
                        NoOfItems += 1;
                    }
                }

                return NoOfItems;
            }
            catch
            { 
                return 1;
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
                            dgvStockOut.ColumnWidthChanged -= dgvStockOut_ColumnWidthChanged;
                            dgvStockOut.Columns[dgvColWidth.Rows[RowIndex].Cells[3].Value.ToString()].Width = 50;
                            dgvStockOut.ColumnWidthChanged += dgvStockOut_ColumnWidthChanged;
                        }
                    }
                    else
                    {
                        if (Convert.ToDecimal(dgvColWidth.Rows[RowIndex].Cells[2].Value.ToString()) < 10)
                        {
                            dgvColWidth.Rows[RowIndex].Cells[2].Value = "50";
                            dgvColWidth.Rows[RowIndex].Cells[0].Value = false;
                            dgvStockOut.ColumnWidthChanged -= dgvStockOut_ColumnWidthChanged;
                            dgvStockOut.Columns[dgvColWidth.Rows[RowIndex].Cells[3].Value.ToString()].Visible = false;
                            dgvStockOut.ColumnWidthChanged += dgvStockOut_ColumnWidthChanged;
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
                if (dgvStockOut.Columns[i].Visible == false)
                {
                    drCol["Visible"] = false;
                }
                if (dgvStockOut.Columns[i].Width <= 10)
                {
                    drCol["Visible"] = false;
                }

                if (gridColIndexes.GetColumnName(i) == "cRateinclusive")
                    drCol["Visible"] = false;

                if (AppSettings.BLNBARCODE == false && gridColIndexes.GetColumnName(i) == "cBarCode")
                    drCol["Visible"] = false;

                drCol["Name"] = dgvStockOut.Columns[i].HeaderText; //Enum.GetName(typeof(GridColIndexes), i).Substring(1, Enum.GetName(typeof(GridColIndexes), i).Length - 1);
                if (gridColIndexes.GetColumnName(i) == dgvStockOut.Columns[i].Name)
                    drCol["Width"] = dgvStockOut.Columns[i].Width;
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
            
            for (int i = 0; i < dgvColWidth.Rows.Count; i++)
            {
                if (dgvColWidth[3, i].Value.ToString() == "cSRate1Per" || dgvColWidth[3, i].Value.ToString() == "cSRate1" || 
                    dgvColWidth[3, i].Value.ToString() == "cSRate2Per" || dgvColWidth[3, i].Value.ToString() == "cSRate2" || 
                    dgvColWidth[3, i].Value.ToString() == "cSRate3Per" || dgvColWidth[3, i].Value.ToString() == "cSRate3" || 
                    dgvColWidth[3, i].Value.ToString() == "cSRate4Per" || dgvColWidth[3, i].Value.ToString() == "cSRate4" || 
                    dgvColWidth[3, i].Value.ToString() == "cSRate5Per" || dgvColWidth[3, i].Value.ToString() == "cSRate5" || 
                    dgvColWidth[3, i].Value.ToString() == "cRateinclusive" ||
                    dgvColWidth[3, i].Value.ToString() == "cItemID" ||
                    dgvColWidth[3, i].Value.ToString() == "cID"
                    )
                {
                    dgvColWidth.Rows[i].Visible = false;
                }

                if (dgvColWidth[3, i].Value.ToString() == "cFree")
                {
                    if (clsVchTypeFeatures.BLNSHOWFREEQUANTITY == false)
                    {
                        dgvColWidth.Rows[i].Visible = false;
                    }
                }
            }

            //dgvStockOut.Columns["cRateinclusive"].Visible = false;
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

        //Description : Calculate the Entire Sales in each and every Corner
        //private void CalcTotal()
        //{
        //    double DblItemAgentCommission = 0;

        //    double DblNontaxableValue = 0;

        //    double DbltaxAmountTot = 0;


        //    double DblcessAmount = 0;
        //    double DblFloodcessAmount = 0;
        //    double DblCompcessAmount = 0;

        //    double DblcessAmountTot = 0;
        //    double DblCompcessAmountTot = 0;
        //    double DblFloodcessAmountTot = 0;


        //    double DblNetAmountTotal = 0;
        //    double QtyTotal = 0;
        //    double DblRate = 0;
        //    double dblQty = 0;

        //    // Not Available in the Method ------------------ >>
        //    double DblrateDiscper = 0;
        //    double DblRateAfterRDiscount = 0;
        //    double dblTaxPer = 0;
        //    double dblCessPer = 0;
        //    double dblQtyCessPer = 0;
        //    double dblFloodCessPer = 0;
        //    double DblRateExclusive = 0;
        //    double dblGrossValue = 0;
        //    double dblGrossValueTot = 0;
        //    double dblQtyTot = 0;
        //    double dblFreeTot = 0;
        //    double dblGrossValueAfterRateDiscount = 0;
        //    double DblrateDiscAmt = 0;
        //    double DblrateDiscAmtTot = 0;
        //    double dblGrossValueAfterRateDiscountTot = 0;
        //    double dblItemDiscAmountTot = 0;
        //    double dblGrossValueAfterDiscounts = 0;
        //    double dblGrossValueAfterDiscountsTot = 0;
        //    double dbltaxableValueAfterItemDiscount = 0;
        //    double dbltaxableValueAfterItemDiscountTot = 0;
        //    double dbltaxAmount = 0;
        //    double dbltaxableAmountTot = 0;
        //    double dblNontaxableAmountTot = 0;
        //    double dbltaxAmountTot = 0;
        //    double dblIGSTTot = 0, dblSSGTTot = 0, dblCSGTTot = 0;
        //    double SavingsofItem = 0;
        //    double CoolieTotal = 0;
        //    int iTaxMode = 2; //GST
        //    bool blnCalculateCoolie = false;

        //    if (txtDiscPerc.Tag == null) txtDiscPerc.Tag = "0";
        //    if (txtDiscPerc.Tag.ToString() == "")
        //        txtDiscPerc.Tag = "0";


        //    if (txtCoolie.Text == "" || txtCoolie.Text == "0")
        //    {
        //        blnCalculateCoolie = true;
        //    }

        //    for (int i = 0; i < dgvStockOut.Rows.Count; i++)
        //    {
        //        SetValue(GetEnum(gridColIndexes.cSlNo), i, (i + 1).ToString());
        //        if (dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.CItemName)].Value != null)
        //        {
        //            if (dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.CItemName)].Value.ToString() != "")
        //            {
        //                if (dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cItemID)].Value.ToString() != "")
        //                {
        //                    if (Convert.ToDecimal(dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cItemID)].Value) != 0)
        //                    {
        //                        if (dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cQty)].Value == null)
        //                            SetValue(GetEnum(gridColIndexes.cQty), i, "0");
        //                        if (dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cFree)].Value == null)
        //                            SetValue(GetEnum(gridColIndexes.cFree), i, "0");

        //                        DblRate = Convert.ToDouble(dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cSrate)].Value);
        //                        //Dipu on 13-May-2022 ---------- >
        //                        dblQty = Convert.ToDouble(dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cQty)].Value);
        //                        //dblQty = Convert.ToDouble(dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cQty)].Value) + Convert.ToDouble(dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cFree)].Value);
        //                        //Dipu on 25-May-2022 -- Free Value Commented
        //                        QtyTotal = QtyTotal + dblQty;// + Convert.ToDouble(dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cFree)].Value);

        //                        //SetValue(GetEnum(gridColIndexes.cSlNo), i, (i + 1).ToString());

        //                        //DblrateDiscper = Convert.ToDouble(dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cRateDiscPer)].Value);
        //                        DblRateAfterRDiscount = DblRate - (DblRate * DblrateDiscper / 100);

        //                        if (blnCalculateCoolie == true)
        //                        {
        //                            CoolieTotal += Convert.ToDouble(dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cCoolie)].Value);
        //                        }

        //                        dblTaxPer = Convert.ToDouble(dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.ctaxPer)].Value);
        //                        dblCessPer = Convert.ToDouble(dgvSales.Rows[i].Cells[GetEnum(gridColIndexes.cCCessPer)].Value);
        //                        dblQtyCessPer = Convert.ToDouble(dgvSales.Rows[i].Cells[GetEnum(gridColIndexes.cCCompCessQty)].Value);
        //                        if (dgvSales.Rows[i].Cells[GetEnum(gridColIndexes.cFloodCessPer)].Value == null)
        //                            SetValue(GetEnum(gridColIndexes.cFloodCessPer), i, "");

        //                        //If chkApplyFloodCess.CheckState = CheckState.Checked Then
        //                        if (dgvSales.Rows[i].Cells[GetEnum(gridColIndexes.cFloodCessPer)].Value.ToString() == "")
        //                            SetValue(GetEnum(gridColIndexes.cFloodCessPer), i, "0");
        //                        dblFloodCessPer = Convert.ToDouble(dgvSales.Rows[i].Cells[GetEnum(gridColIndexes.cFloodCessPer)].Value);
        //                        //End If

        //                        if (clsVchType.DefaultTaxInclusiveValue == 2)
        //                            dgvSales.Rows[i].Cells[GetEnum(gridColIndexes.cRateinclusive)].Value = true;
        //                        else if (clsVchType.DefaultTaxInclusiveValue == 3)
        //                            dgvSales.Rows[i].Cells[GetEnum(gridColIndexes.cRateinclusive)].Value = false;

        //                        if (Convert.ToBoolean(dgvSales.Rows[i].Cells[GetEnum(gridColIndexes.cRateinclusive)].Value) == true)
        //                            DblRateExclusive = GetRateExclusive(DblRateAfterRDiscount, (dblCessPer + dblTaxPer + dblFloodCessPer), 0);
        //                        else
        //                            DblRateExclusive = DblRateAfterRDiscount;

        //                        dblGrossValue = DblRateExclusive * dblQty;
        //                        SetValue(GetEnum(gridColIndexes.cGrossAmt), i, Comm.FormatValue(dblGrossValue));
        //                        dblGrossValueTot = dblGrossValueTot + dblGrossValue;
        //                        dblGrossValueAfterRateDiscount = dblQty * (DblRateExclusive);

        //                        dblQtyTot += Convert.ToDouble(dgvSales.Rows[i].Cells[GetEnum(gridColIndexes.cQty)].Value);
        //                        dblFreeTot += Convert.ToDouble(dgvSales.Rows[i].Cells[GetEnum(gridColIndexes.cFree)].Value);

        //                        SetValue(GetEnum(gridColIndexes.cGrossValueAfterRateDiscount), i, Comm.FormatValue(dblGrossValueAfterRateDiscount));

        //                        DblrateDiscAmt = dblQty * (DblRate - DblRateAfterRDiscount);
        //                        DblrateDiscAmtTot = DblrateDiscAmtTot + DblrateDiscAmt;

        //                        dblGrossValueAfterRateDiscountTot = dblGrossValueAfterRateDiscountTot + dblGrossValueAfterRateDiscount;
        //                        //dbltaxableValueAfterItemDiscountTot = dblGrossValueAfterDiscountsTot;

        //                        if (Convert.ToDouble(dgvSales.Rows[i].Cells[GetEnum(gridColIndexes.cDiscPer)].Value) > 0)
        //                        {
        //                            SetValue(GetEnum(gridColIndexes.cDiscAmount), i, Comm.FormatValue((dblGrossValueAfterRateDiscount * Convert.ToDouble(dgvSales.Rows[i].Cells[GetEnum(gridColIndexes.cDiscPer)].Value) / 100)));
        //                            dblItemDiscAmountTot = dblItemDiscAmountTot + Convert.ToDouble(dgvSales.Rows[i].Cells[GetEnum(gridColIndexes.cDiscAmount)].Value);
        //                        }
        //                        else if (Convert.ToDouble(dgvSales.Rows[i].Cells[GetEnum(gridColIndexes.cDiscAmount)].Value) > 0)
        //                        {
        //                            SetValue(GetEnum(gridColIndexes.cDiscAmount), i, Comm.FormatValue(Convert.ToDouble(dgvSales.Rows[i].Cells[GetEnum(gridColIndexes.cDiscAmount)].Value)));
        //                            dblItemDiscAmountTot = dblItemDiscAmountTot + Convert.ToDouble(dgvSales.Rows[i].Cells[GetEnum(gridColIndexes.cDiscAmount)].Value);
        //                        }

        //                        dblGrossValueAfterDiscounts = dblGrossValueAfterRateDiscount - Convert.ToDouble(dgvSales.Rows[i].Cells[GetEnum(gridColIndexes.cDiscAmount)].Value);
        //                        dblGrossValueAfterDiscountsTot = dblGrossValueAfterDiscountsTot + dblGrossValueAfterDiscounts;
        //                        //
        //                        //Arrived Taxable Value
        //                        dbltaxableValueAfterItemDiscount = dblGrossValueAfterDiscounts;
        //                        dbltaxableValueAfterItemDiscountTot = dblGrossValueAfterDiscountsTot;
        //                        SetValue(GetEnum(gridColIndexes.cGrossValueAfterRateDiscount), i, Comm.FormatValue(dbltaxableValueAfterItemDiscount));

        //                        iTaxMode = Convert.ToInt32(cboTaxMode.SelectedValue) - 1;

        //                        if (Convert.ToDouble(dgvSales.Rows[i].Cells[GetEnum(gridColIndexes.ctaxPer)].Value) > 0)
        //                        {
        //                            DblNontaxableValue = 0;
        //                            dbltaxAmount = dbltaxableValueAfterItemDiscount * Convert.ToDouble(dgvSales.Rows[i].Cells[GetEnum(gridColIndexes.ctaxPer)].Value) / 100;
        //                            dbltaxableAmountTot = dbltaxableAmountTot + dbltaxableValueAfterItemDiscount;
        //                        }
        //                        else
        //                        {
        //                            dbltaxAmount = 0;
        //                            DblNontaxableValue = dbltaxableValueAfterItemDiscount;
        //                            dblNontaxableAmountTot = dblNontaxableAmountTot + dbltaxableValueAfterItemDiscount;
        //                        }
        //                        //Tax Mode wise Calculation
        //                        if (iTaxMode == 2) //GST
        //                        {
        //                            if (Convert.ToDouble(dgvSales.Rows[i].Cells[GetEnum(gridColIndexes.ctaxPer)].Value) > 0)
        //                            {
        //                                SetValue(GetEnum(gridColIndexes.ctaxable), i, Comm.FormatValue(dbltaxableValueAfterItemDiscount));
        //                                SetValue(GetEnum(gridColIndexes.cNonTaxable), i, "0");
        //                            }
        //                            else
        //                            {
        //                                SetValue(GetEnum(gridColIndexes.ctaxable), i, "0");
        //                                SetValue(GetEnum(gridColIndexes.cNonTaxable), i, Comm.FormatValue(dbltaxableValueAfterItemDiscount));
        //                            }
        //                            SetValue(GetEnum(gridColIndexes.ctax), i, Comm.FormatValue(dbltaxAmount));
        //                            if (cboState.SelectedValue != null)
        //                            {
        //                                if (cboState.SelectedValue.ToString() != "32")
        //                                {
        //                                    SetValue(GetEnum(gridColIndexes.cCGST), i, "0");
        //                                    SetValue(GetEnum(gridColIndexes.cSGST), i, "0");
        //                                    SetValue(GetEnum(gridColIndexes.cIGST), i, Comm.FormatValue(dbltaxAmount));
        //                                }
        //                                else
        //                                {
        //                                    SetValue(GetEnum(gridColIndexes.cCGST), i, Comm.FormatValue((dbltaxAmount * 0.5)));
        //                                    SetValue(GetEnum(gridColIndexes.cSGST), i, Comm.FormatValue((dbltaxAmount * 0.5)));
        //                                    SetValue(GetEnum(gridColIndexes.cIGST), i, "0");
        //                                }
        //                            }
        //                            else
        //                            {
        //                                SetValue(GetEnum(gridColIndexes.cCGST), i, Comm.FormatValue((dbltaxAmount * 0.5)));
        //                                SetValue(GetEnum(gridColIndexes.cSGST), i, Comm.FormatValue((dbltaxAmount * 0.5)));
        //                                SetValue(GetEnum(gridColIndexes.cIGST), i, "0");
        //                            }

        //                            DbltaxAmountTot = DbltaxAmountTot + dbltaxAmount;
        //                            SetValue(GetEnum(gridColIndexes.cNetAmount), i, Comm.FormatValue((dbltaxableValueAfterItemDiscount + dbltaxAmount)));
        //                            DblNetAmountTotal = DblNetAmountTotal + Convert.ToDouble(dgvSales.Rows[i].Cells[GetEnum(gridColIndexes.cNetAmount)].Value);
        //                        }
        //                        else if (iTaxMode == 1) //VAT
        //                        {
        //                            if (Convert.ToDouble(dgvSales.Rows[i].Cells[GetEnum(gridColIndexes.ctaxPer)].Value) > 0)
        //                            {
        //                                SetValue(GetEnum(gridColIndexes.ctaxable), i, Comm.FormatValue(dbltaxableValueAfterItemDiscount));
        //                                SetValue(GetEnum(gridColIndexes.cNonTaxable), i, "0");

        //                                SetValue(GetEnum(gridColIndexes.cCGST), i, "0");
        //                                SetValue(GetEnum(gridColIndexes.cSGST), i, "0");
        //                                SetValue(GetEnum(gridColIndexes.cIGST), i, Comm.FormatValue(dbltaxAmount));

        //                            }
        //                            else
        //                            {
        //                                SetValue(GetEnum(gridColIndexes.ctaxable), i, "0");
        //                                SetValue(GetEnum(gridColIndexes.cNonTaxable), i, Comm.FormatValue(dbltaxableValueAfterItemDiscount));
        //                            }
        //                            SetValue(GetEnum(gridColIndexes.ctax), i, Comm.FormatValue(dbltaxAmount));

        //                            DbltaxAmountTot = DbltaxAmountTot + dbltaxAmount;
        //                            SetValue(GetEnum(gridColIndexes.cNetAmount), i, Comm.FormatValue((dbltaxableValueAfterItemDiscount + dbltaxAmount)));
        //                            DblNetAmountTotal = DblNetAmountTotal + Convert.ToDouble(dgvSales.Rows[i].Cells[GetEnum(gridColIndexes.cNetAmount)].Value);
        //                        }
        //                        else if (iTaxMode == 0) //NONE
        //                        {
        //                            SetValue(GetEnum(gridColIndexes.ctaxable), i, "0"); // Comm.FormatValue(dbltaxableValueAfterItemDiscount));
        //                            //SetValue(GetEnum(gridColIndexes.cNonTaxable), i, Comm.FormatValue(DblNontaxableValue));
        //                            SetValue(GetEnum(gridColIndexes.cNonTaxable), i, Comm.FormatValue(DblNontaxableValue));
        //                            //Check Dipu

        //                            SetValue(GetEnum(gridColIndexes.cCGST), i, "0");
        //                            SetValue(GetEnum(gridColIndexes.cSGST), i, "0");
        //                            SetValue(GetEnum(gridColIndexes.cIGST), i, "0");
        //                            SetValue(GetEnum(gridColIndexes.ctax), i, "0");

        //                            //DbltaxAmountTot = DbltaxAmountTot + dbltaxAmount;
        //                            SetValue(GetEnum(gridColIndexes.cNetAmount), i, Comm.FormatValue((dbltaxableValueAfterItemDiscount)));
        //                            DblNetAmountTotal = DblNetAmountTotal + Convert.ToDouble(dgvSales.Rows[i].Cells[GetEnum(gridColIndexes.cNetAmount)].Value);
        //                        }
        //                        //DbltaxAmountTot = DbltaxAmountTot + dbltaxAmount;
        //                        //SetValue(GetEnum(gridColIndexes.cNetAmount), i, Comm.FormatValue((dbltaxableValueAfterItemDiscount + dbltaxAmount)));
        //                        //DblNetAmountTotal = DblNetAmountTotal + Convert.ToDouble(dgvSales.Rows[i].Cells[GetEnum(gridColIndexes.cNetAmount)].Value);

        //                    }
        //                }
        //            }
        //        }
        //    }

        //    if (blnCalculateCoolie == true)
        //        txtCoolie.Text = CoolieTotal.ToString();

        //    if (txtDiscAmt.Text == "")
        //    {
        //        txtDiscAmt.Text = "0";
        //        txtDiscPerc.Text = "0";
        //    }
        //    if (Convert.ToDouble(txtDiscAmt.Text) > 0)
        //    {
        //        if (dSteadyBillDiscAmt > 0 && dSteadyBillDiscPerc == 0)
        //        {
        //            if (dblGrossValueAfterDiscountsTot > 0)
        //                txtDiscPerc.Text = Comm.FormatValue((Convert.ToDouble(txtDiscAmt.Text) / dblGrossValueAfterDiscountsTot * 100));
        //        }
        //        else
        //        {
        //            if (Convert.ToDouble(txtDiscPerc.Text) > 0)
        //            {
        //                if (dblGrossValueAfterDiscountsTot > 0)
        //                    txtDiscAmt.Text = Comm.FormatValue((dblGrossValueAfterDiscountsTot * Convert.ToDouble(txtDiscPerc.Text) / 100));
        //            }
        //        }

        //    }
        //    else
        //    {
        //        if (dSteadyBillDiscPerc > 0 && dSteadyBillDiscAmt == 0)
        //        {
        //            if (Convert.ToDouble(txtDiscPerc.Text) > 0)
        //                txtDiscAmt.Text = Comm.FormatValue((dblGrossValueAfterDiscountsTot * Convert.ToDouble(txtDiscPerc.Text) / 100));
        //        }
        //        else
        //        {
        //            if (dblGrossValueAfterDiscountsTot > 0)
        //            {
        //                if (Convert.ToInt32(txtDiscPerc.Tag.ToString()) == 0)
        //                    txtDiscPerc.Text = Comm.FormatValue((Convert.ToDouble(txtDiscAmt.Text) / dblGrossValueAfterDiscountsTot * 100));

        //            }
        //        }
        //    }

        //    if (txtDiscAmt.Text.ToString() == "") txtDiscAmt.Text = "0";
        //    if (txtDiscPerc.Text.ToString() == "") txtDiscPerc.Text = "0";
        //    if (Convert.ToDouble(txtDiscAmt.Text.ToString()) == 0 && Convert.ToDouble(txtDiscPerc.Text.ToString()) != 0)
        //        txtDiscAmt.Text = Comm.FormatValue((dblGrossValueAfterDiscountsTot * Convert.ToDouble(txtDiscPerc.Text) / 100));
        //    if (Convert.ToDouble(txtDiscAmt.Text.ToString()) != 0 && Convert.ToDouble(txtDiscPerc.Text.ToString()) == 0)
        //        txtDiscPerc.Text = Comm.FormatValue((Convert.ToDouble(txtDiscAmt.Text) / dblGrossValueAfterDiscountsTot * 100));

        //    //''''''' Bill Dicount Calculation''''''''''''''''''''
        //    //'First Discount 

        //    if (txtDiscAmt.Text == "") txtDiscAmt.Text = "0";
        //    double Discountamount = Convert.ToDouble(txtDiscAmt.Text);
        //    DblNetAmountTotal = 0;
        //    dbltaxableAmountTot = 0;
        //    dblNontaxableAmountTot = 0;
        //    dbltaxAmount = 0;
        //    dbltaxAmountTot = 0;
        //    double TotalValueOfFree = 0;
        //    if (txtOtherExp.Text == "") txtOtherExp.Text = "0";
        //    if (txtCashDisc.Text == "") txtCashDisc.Text = "0";
        //    double BillExpeDisc = Convert.ToDouble(txtOtherExp.Text) - Convert.ToDouble(txtCashDisc.Text) - Convert.ToDouble(txtDiscAmt.Text);
        //    double Savings = 0;
        //    double dbltaxableAmount = 0;

        //    for (int j = 0; j < dgvSales.Rows.Count; j++)
        //    {
        //        if (dgvSales.Rows[j].Cells[GetEnum(gridColIndexes.CItemName)].Value != null)
        //        {
        //            if (dgvSales.Rows[j].Cells[GetEnum(gridColIndexes.CItemName)].Value.ToString() != "")
        //            {
        //                dblTaxPer = Convert.ToDouble(dgvSales.Rows[j].Cells[GetEnum(gridColIndexes.ctaxPer)].Value);
        //                dblCessPer = Convert.ToDouble(dgvSales.Rows[j].Cells[GetEnum(gridColIndexes.cCCessPer)].Value);
        //                dblQtyCessPer = Convert.ToDouble(dgvSales.Rows[j].Cells[GetEnum(gridColIndexes.cCCompCessQty)].Value);
        //                // check from Settings
        //                dblFloodCessPer = 0;

        //                SetValue(GetEnum(gridColIndexes.cBillDisc), j, "0");
        //                dblGrossValueAfterDiscounts = Convert.ToDouble(dgvSales.Rows[j].Cells[GetEnum(gridColIndexes.cGrossValueAfterRateDiscount)].Value);
        //                if (dblGrossValueAfterDiscountsTot > 0)
        //                    SetValue(GetEnum(gridColIndexes.cBillDisc), j, Comm.FormatValue((Discountamount / dblGrossValueAfterDiscountsTot * dblGrossValueAfterDiscounts)));

        //                if (Convert.ToDouble(dgvSales.Rows[j].Cells[GetEnum(gridColIndexes.ctaxPer)].Value) > 0)
        //                {
        //                    dbltaxableAmount = Convert.ToDouble(dgvSales.Rows[j].Cells[GetEnum(gridColIndexes.ctaxable)].Value) - Convert.ToDouble(dgvSales.Rows[j].Cells[GetEnum(gridColIndexes.cBillDisc)].Value);
        //                    DblNontaxableValue = 0;
        //                }
        //                else
        //                {
        //                    DblNontaxableValue = Convert.ToDouble(dgvSales.Rows[j].Cells[GetEnum(gridColIndexes.cNonTaxable)].Value) - Convert.ToDouble(dgvSales.Rows[j].Cells[GetEnum(gridColIndexes.cBillDisc)].Value);
        //                    dbltaxableAmount = 0;
        //                }

        //                SetValue(GetEnum(gridColIndexes.ctaxable), j, Comm.FormatValue(dbltaxableAmount));
        //                SetValue(GetEnum(gridColIndexes.cNonTaxable), j, Comm.FormatValue(DblNontaxableValue));

        //                dbltaxAmount = Convert.ToDouble(dgvSales.Rows[j].Cells[GetEnum(gridColIndexes.ctaxable)].Value) * Convert.ToDouble(dgvSales.Rows[j].Cells[GetEnum(gridColIndexes.ctaxPer)].Value) / 100;
        //                DblcessAmount = Convert.ToDouble(dgvSales.Rows[j].Cells[GetEnum(gridColIndexes.ctaxable)].Value) * dblCessPer / 100;
        //                DblCompcessAmount = Convert.ToDouble(dgvSales.Rows[j].Cells[GetEnum(gridColIndexes.cQty)].Value) * dblQtyCessPer;
        //                DblFloodcessAmount = Convert.ToDouble(dgvSales.Rows[j].Cells[GetEnum(gridColIndexes.ctaxable)].Value) * dblFloodCessPer / 100;

        //                SetTag(GetEnum(gridColIndexes.cCCessPer), j, Comm.FormatValue(DblcessAmount, true, "#.00"));
        //                SetTag(GetEnum(gridColIndexes.cCCompCessQty), j, Comm.FormatValue(DblCompcessAmount, false));

        //                SetValue(GetEnum(gridColIndexes.cFloodCessAmt), j, Comm.FormatValue(DblFloodcessAmount));
        //                DblFloodcessAmountTot = DblFloodcessAmountTot + DblFloodcessAmount;
        //                DblcessAmountTot = DblcessAmountTot + DblcessAmount;
        //                DblCompcessAmountTot = DblCompcessAmountTot + DblCompcessAmount;

        //                if (iTaxMode == 2) //GST
        //                {
        //                    SetValue(GetEnum(gridColIndexes.ctax), j, Comm.FormatValue(dbltaxAmount));
        //                    if (Convert.ToDouble(dgvSales.Rows[j].Cells[GetEnum(gridColIndexes.ctaxPer)].Value) > 0)
        //                    {
        //                        SetValue(GetEnum(gridColIndexes.ctaxable), j, Comm.FormatValue(Convert.ToDouble(dgvSales.Rows[j].Cells[GetEnum(gridColIndexes.ctaxable)].Value)));
        //                        SetValue(GetEnum(gridColIndexes.cNonTaxable), j, "0");
        //                    }
        //                    else
        //                    {
        //                        SetValue(GetEnum(gridColIndexes.cNonTaxable), j, Comm.FormatValue(Convert.ToDouble(dgvSales.Rows[j].Cells[GetEnum(gridColIndexes.cNonTaxable)].Value)));
        //                        SetValue(GetEnum(gridColIndexes.ctaxable), j, "0");
        //                    }

        //                    if (cboState.SelectedValue != null)
        //                    {
        //                        if (cboState.SelectedValue.ToString() != "32")
        //                        {
        //                            SetValue(GetEnum(gridColIndexes.cCGST), j, "0");
        //                            SetValue(GetEnum(gridColIndexes.cSGST), j, "0");
        //                            SetValue(GetEnum(gridColIndexes.cIGST), j, Comm.FormatValue(dbltaxAmount));
        //                            SetTag(GetEnum(gridColIndexes.cCGST), j, "0"); ;

        //                            SetTag(GetEnum(gridColIndexes.cSGST), j, "0");
        //                            SetTag(GetEnum(gridColIndexes.cIGST), j, Convert.ToDouble(dgvSales.Rows[j].Cells[GetEnum(gridColIndexes.ctaxPer)].Value).ToString());
        //                        }
        //                        else
        //                        {
        //                            SetValue(GetEnum(gridColIndexes.cCGST), j, Comm.FormatValue((dbltaxAmount * 0.5)));
        //                            SetValue(GetEnum(gridColIndexes.cSGST), j, Comm.FormatValue((dbltaxAmount * 0.5)));
        //                            SetValue(GetEnum(gridColIndexes.cIGST), j, "0");

        //                            SetTag(GetEnum(gridColIndexes.cCGST), j, (Convert.ToDouble(dgvSales.Rows[j].Cells[GetEnum(gridColIndexes.ctaxPer)].Value) * 0.5).ToString());
        //                            SetTag(GetEnum(gridColIndexes.cSGST), j, (Convert.ToDouble(dgvSales.Rows[j].Cells[GetEnum(gridColIndexes.ctaxPer)].Value) * 0.5).ToString());
        //                            SetTag(GetEnum(gridColIndexes.cIGST), j, "0");

        //                        }
        //                    }
        //                    else
        //                    {
        //                        SetValue(GetEnum(gridColIndexes.cCGST), j, Comm.FormatValue((dbltaxAmount * 0.5)));
        //                        SetValue(GetEnum(gridColIndexes.cSGST), j, Comm.FormatValue((dbltaxAmount * 0.5)));
        //                        SetValue(GetEnum(gridColIndexes.cIGST), j, "0");

        //                        SetTag(GetEnum(gridColIndexes.cCGST), j, (Convert.ToDouble(dgvSales.Rows[j].Cells[GetEnum(gridColIndexes.ctaxPer)].Value) * 0.5).ToString());
        //                        SetTag(GetEnum(gridColIndexes.cSGST), j, (Convert.ToDouble(dgvSales.Rows[j].Cells[GetEnum(gridColIndexes.ctaxPer)].Value) * 0.5).ToString());
        //                        SetTag(GetEnum(gridColIndexes.cIGST), j, "0");
        //                    }
        //                }
        //                else if (iTaxMode == 1) //VAT
        //                {
        //                    SetValue(GetEnum(gridColIndexes.ctax), j, Comm.FormatValue(dbltaxAmount));
        //                    if (Convert.ToDouble(dgvSales.Rows[j].Cells[GetEnum(gridColIndexes.ctaxPer)].Value) > 0)
        //                    {
        //                        SetValue(GetEnum(gridColIndexes.ctaxable), j, Comm.FormatValue(Convert.ToDouble(dgvSales.Rows[j].Cells[GetEnum(gridColIndexes.ctaxable)].Value)));
        //                        SetValue(GetEnum(gridColIndexes.cNonTaxable), j, "0");
        //                    }
        //                    else
        //                    {
        //                        SetValue(GetEnum(gridColIndexes.cNonTaxable), j, Comm.FormatValue(Convert.ToDouble(dgvSales.Rows[j].Cells[GetEnum(gridColIndexes.cNonTaxable)].Value)));
        //                        SetValue(GetEnum(gridColIndexes.ctaxable), j, "0");
        //                    }

        //                    SetValue(GetEnum(gridColIndexes.cCGST), j, "0");
        //                    SetValue(GetEnum(gridColIndexes.cSGST), j, "0");
        //                    SetValue(GetEnum(gridColIndexes.cIGST), j, Comm.FormatValue(dbltaxAmount));
        //                    SetTag(GetEnum(gridColIndexes.cCGST), j, "0"); ;

        //                    SetTag(GetEnum(gridColIndexes.cSGST), j, "0");
        //                    SetTag(GetEnum(gridColIndexes.cIGST), j, Convert.ToDouble(dgvSales.Rows[j].Cells[GetEnum(gridColIndexes.ctaxPer)].Value).ToString());
        //                }
        //                else if (iTaxMode == 0) //NONE
        //                {
        //                    //SetValue(GetEnum(gridColIndexes.ctaxable), j, Comm.FormatValue(dbltaxableAmount));
        //                    //SetValue(GetEnum(gridColIndexes.cNonTaxable), j, Comm.FormatValue(DblNontaxableValue));
        //                    SetValue(GetEnum(gridColIndexes.ctaxable), j, Comm.FormatValue(Convert.ToDouble(dgvSales.Rows[j].Cells[GetEnum(gridColIndexes.ctaxable)].Value)));
        //                    SetValue(GetEnum(gridColIndexes.cNonTaxable), j, Comm.FormatValue(Convert.ToDouble(dgvSales.Rows[j].Cells[GetEnum(gridColIndexes.cNonTaxable)].Value)));
        //                    //Check Dipu

        //                    SetValue(GetEnum(gridColIndexes.cCGST), j, "0");
        //                    SetValue(GetEnum(gridColIndexes.cSGST), j, "0");
        //                    SetValue(GetEnum(gridColIndexes.cIGST), j, "0");

        //                    //SetValue(GetEnum(gridColIndexes.cNetAmount), j, Comm.FormatValue((dbltaxableValueAfterItemDiscount)));
        //                }

        //                dblIGSTTot = dblIGSTTot + Convert.ToDouble(dgvSales.Rows[j].Cells[GetEnum(gridColIndexes.cIGST)].Value);
        //                dblSSGTTot = dblSSGTTot + Convert.ToDouble(dgvSales.Rows[j].Cells[GetEnum(gridColIndexes.cSGST)].Value);
        //                dblCSGTTot = dblCSGTTot + Convert.ToDouble(dgvSales.Rows[j].Cells[GetEnum(gridColIndexes.cCGST)].Value);

        //                dbltaxAmountTot = dbltaxAmountTot + dbltaxAmount;
        //                //dbltaxAmountTot = Comm.FormatAmt(Val(dbltaxAmountTot) + Val(Format(Val(dbltaxAmount), DCSApp.GBizAmt)), DCSApp.GBizAmt)
        //                // dont know how to format ??

        //                dbltaxableAmountTot = dbltaxableAmountTot + Convert.ToDouble(dgvSales.Rows[j].Cells[GetEnum(gridColIndexes.ctaxable)].Value);
        //                dblNontaxableAmountTot = dblNontaxableAmountTot + Convert.ToDouble(dgvSales.Rows[j].Cells[GetEnum(gridColIndexes.cNonTaxable)].Value);

        //                //DGVItem.Item(cNetAmount, i).Value = Comm.FormatAmt(Val(DGVItem.Item(ctaxable, i).Value) + Val(DGVItem.Item(cNonTaxable, i).Value) + Val(DGVItem.Item(ctax, i).Value) + Val(DblcessAmount) + Val(DblFloodcessAmount) + Val(DblCompcessAmount), "")
        //                //Dont know what is Comm.FormatAmt ->
        //                //if (iTaxMode != 0) //NOT NONE

        //                SetValue(GetEnum(gridColIndexes.cNetAmount), j, Comm.FormatValue((Convert.ToDouble(dgvSales.Rows[j].Cells[GetEnum(gridColIndexes.ctaxable)].Value) + Convert.ToDouble(dgvSales.Rows[j].Cells[GetEnum(gridColIndexes.cNonTaxable)].Value) + Convert.ToDouble(dgvSales.Rows[j].Cells[GetEnum(gridColIndexes.ctax)].Value) + DblcessAmount + DblFloodcessAmount + DblCompcessAmount)));

        //                DblNetAmountTotal = DblNetAmountTotal + Convert.ToDouble(dgvSales.Rows[j].Cells[GetEnum(gridColIndexes.cNetAmount)].Value);

        //                //valuation of Free
        //                dblQty = Convert.ToDouble(dgvSales.Rows[j].Cells[GetEnum(gridColIndexes.cQty)].Value);
        //                if (Convert.ToDouble(dgvSales.Rows[j].Cells[GetEnum(gridColIndexes.cFree)].Value) > 0)
        //                {
        //                    double PerItemRate = Convert.ToDouble(dgvSales.Rows[j].Cells[GetEnum(gridColIndexes.cNetAmount)].Value) - Convert.ToDouble(dgvSales.Rows[j].Cells[GetEnum(gridColIndexes.ctax)].Value) / dblQty;
        //                    TotalValueOfFree = TotalValueOfFree + (PerItemRate * Convert.ToDouble(dgvSales.Rows[j].Cells[GetEnum(gridColIndexes.cFree)].Value));
        //                }

        //                //CALCULATION DECIMAL CHANGING
        //                SetValue(GetEnum(gridColIndexes.cDiscAmount), j, Comm.FormatValue(Convert.ToDouble(dgvSales.Rows[j].Cells[GetEnum(gridColIndexes.cDiscAmount)].Value)));
        //                SetValue(GetEnum(gridColIndexes.cBillDisc), j, Comm.FormatValue(Convert.ToDouble(dgvSales.Rows[j].Cells[GetEnum(gridColIndexes.cBillDisc)].Value)));

        //                SetValue(GetEnum(gridColIndexes.ctaxable), j, Comm.FormatValue(Convert.ToDouble(dgvSales.Rows[j].Cells[GetEnum(gridColIndexes.ctaxable)].Value)));
        //                    SetTag(GetEnum(gridColIndexes.ctaxable), j, Comm.FormatValue(Convert.ToDouble(dgvSales.Rows[j].Cells[GetEnum(gridColIndexes.ctaxable)].Value)));
        //                //DGVItem.Item(ctaxable, i).Tag = Format(Val(DGVItem.Item(ctaxable, i).Value), "#0.000000")
        //                //Tag ??

        //                SetValue(GetEnum(gridColIndexes.ctax), j, Comm.FormatValue(Convert.ToDouble(dgvSales.Rows[j].Cells[GetEnum(gridColIndexes.ctax)].Value)));
        //                SetValue(GetEnum(gridColIndexes.cIGST), j, Comm.FormatValue(Convert.ToDouble(dgvSales.Rows[j].Cells[GetEnum(gridColIndexes.cIGST)].Value)));
        //                SetValue(GetEnum(gridColIndexes.cSGST), j, Comm.FormatValue(Convert.ToDouble(dgvSales.Rows[j].Cells[GetEnum(gridColIndexes.cSGST)].Value)));
        //                SetValue(GetEnum(gridColIndexes.cCGST), j, Comm.FormatValue(Convert.ToDouble(dgvSales.Rows[j].Cells[GetEnum(gridColIndexes.cCGST)].Value)));
        //                SetValue(GetEnum(gridColIndexes.cNetAmount), j, Comm.FormatValue(Convert.ToDouble(dgvSales.Rows[j].Cells[GetEnum(gridColIndexes.cNetAmount)].Value)));
        //                //SetValue(GetEnum(gridColIndexes.cNetAmount), j, Comm.FormatValue(Convert.ToDouble(dgvSales.Rows[j].Cells[GetEnum(gridColIndexes.cNetAmount)].Value)));
        //                SetValue(GetEnum(gridColIndexes.cGrossValueAfterRateDiscount), j, Comm.FormatValue(Convert.ToDouble(dgvSales.Rows[j].Cells[GetEnum(gridColIndexes.cGrossValueAfterRateDiscount)].Value)));
        //                SetValue(GetEnum(gridColIndexes.cNonTaxable), j, Comm.FormatValue(Convert.ToDouble(dgvSales.Rows[j].Cells[GetEnum(gridColIndexes.cNonTaxable)].Value)));

        //                if (Convert.ToDouble(dgvSales.Rows[j].Cells[GetEnum(gridColIndexes.cAgentCommPer)].Value) > 0)
        //                    DblItemAgentCommission = (DblItemAgentCommission + Convert.ToDouble(dgvSales.Rows[j].Cells[GetEnum(gridColIndexes.cNetAmount)].Value) * Convert.ToDouble(dgvSales.Rows[j].Cells[GetEnum(gridColIndexes.cAgentCommPer)].Value) / 100);

        //            }
        //        }
        //    }

        //    // What is the use of this ?? --------------------------- >>
        //    //If dgv.GetValue(LinkIDs.AgentCommissionMode) = "ITEM" Then
        //    //    dgv.SetValue(LinkIDs.AgentCommission, DblItemAgentCommission)
        //    //ElseIf dgv.GetValue(LinkIDs.AgentCommissionMode) = "NONE" Then
        //    //    dgv.SetValue(LinkIDs.AgentCommission, 0)
        //    //ElseIf InStr(dgv.GetValue(LinkIDs.AgentCommissionMode), "BILL", CompareMethod.Text) > 0 Then
        //    //    Dim MyVarStr() As String = Split(dgv.GetValue(LinkIDs.AgentCommissionMode), "@")
        //    //    If UBound(MyVarStr) > 0 Then
        //    //        DblItemAgentCommission = Val(lblBalance.Text) * Val(MyVarStr(1)) / 100
        //    //    End If
        //    //    dgv.SetValue(LinkIDs.AgentCommission, DblItemAgentCommission)
        //    //End If
        //    // What is the use of this ?? --------------------------- >>

        //    lblAgentCommissionTotal.Text = Comm.FormatValue(DblItemAgentCommission);
        //    txtNetAmt.Text = Comm.FormatValue(DblNetAmountTotal);
        //    txtGrossAmt.Text = Comm.FormatValue(dblGrossValueTot);
        //    lblQtyTotal.Text = Comm.FormatValue(dblQtyTot);
        //    lblFreeTotal.Text = Comm.FormatValue(dblFreeTot);
        //    //txtGrossAftRateDiscount.Text = Comm.FormatAmt(dblGrossValueAfterRateDiscountTot, Global.GDecimal);
        //    //txtGrossAfterItmDisc.Text = Comm.FormatValue(dbltaxableValueAfterItemDiscountTot);
        //    txtGrossAfterItmDisc.Text = Comm.FormatValue(dblGrossValueAfterDiscountsTot);
        //        txtRateDiscTot.Text = Comm.FormatValue(DblrateDiscAmtTot);
        //    txtItemDiscTot.Text = Comm.FormatValue(dblItemDiscAmountTot);
        //    txtTaxable.Text = Comm.FormatValue(dbltaxableAmountTot);
        //    txtNonTaxable.Text = Comm.FormatValue(dblNontaxableAmountTot);
        //    txtTaxAmt.Text = Comm.FormatValue(dbltaxAmountTot);

        //    txtCess.Text = Comm.FormatValue(DblcessAmountTot);
        //    txtCompCess.Text = Comm.FormatValue(DblCompcessAmountTot);
        //    txtNetAmt.Text = Comm.FormatValue(DblNetAmountTotal);

        //    double bALANCEFORrOUNDOFF = Convert.ToDouble(Comm.FormatAmt(DblNetAmountTotal - 0 - Convert.ToDouble(txtCashDisc.Text) + Convert.ToDouble(txtOtherExp.Text), ""));

        //    lblBillAmount.Text = Comm.FormatValue(bALANCEFORrOUNDOFF);

        //    if (txtRoundOff.Text.ToString() == "")
        //    {
        //        txtRoundOff.Text = "0";
        //    }

        //    if (clsVchType.RoundOffMode > 0)
        //    {
        //        if (clsVchType.RoundOffMode != 4) //Manual
        //        {
        //            double RoundOffValue = 0;
        //            clsJSonCommon Roundoff = new clsJSonCommon();
        //            RoundOffValue = Roundoff.RoundOffAmount(bALANCEFORrOUNDOFF, clsVchType.RoundOffMode, clsVchType.RoundOffBlock);
        //            txtRoundOff.Text = Comm.FormatValue(RoundOffValue - Convert.ToDouble(lblBillAmount.Text));
        //        }
        //        else
        //        {
        //            lblBillAmount.Text = (bALANCEFORrOUNDOFF + Convert.ToDouble(txtRoundOff.Text.ToString())).ToString();
        //        }
        //    }
        //    //if (mytrans.IntRoundOffMode != 4) //Manual
        //    //{
        //    //    double RoundOffValue = 0;
        //    //    //RoundOffValue = = RoundOffAmount(bALANCEFORrOUNDOFF, mytrans.IntRoundOffMode, mytrans.DBLRoundOffBlock)
        //    //    txtRoundOff.Text = Comm.FormatValue(RoundOffValue - Convert.ToDouble(lblBillAmount.Text));
        //    //    lblBillAmount.Text = Comm.FormatValue(RoundOffValue);
        //    //}
        //    //else
        //    if (txtRoundOff.Text == "") txtRoundOff.Text = "0";
        //    lblBillAmount.Text = Comm.FormatValue(bALANCEFORrOUNDOFF + Convert.ToDouble(txtRoundOff.Text));
        //    //}
        //    lblBillAmount.Text = Comm.FormatValue(Convert.ToDouble(lblBillAmount.Text));
        //    double AdditionalCharges = 0;
        //    if (txtRoundOff.Text == "") txtRoundOff.Text = "0";
        //    if (txtCostFactor.Text == "") txtCostFactor.Text = "0";
        //    // When mytrans.IntRoundOffMode > 0 then Comment this
        //    if (txtRoundOff.Text == "") txtRoundOff.Text = "0";
        //    if (txtRoundOff.Text != "-")
        //    {
        //        if (txtRoundOff.Text != ".")
        //        {
        //            //lblBillAmount.Text = Comm.FormatValue(Convert.ToDouble(lblBillAmount.Text) + Convert.ToDouble(txtRoundOff.Text));
        //            AdditionalCharges = Convert.ToDouble(txtOtherExp.Text) - Convert.ToDouble(txtCashDisc.Text) + Convert.ToDouble(txtRoundOff.Text) + Convert.ToDouble(txtCostFactor.Text);
        //        }
        //    }
        //    // When mytrans.IntRoundOffMode > 0 then Comment this

        //    //Assuming that the rate is equally equated between items .....
        //    //if (txtRoundOff.Text == "") txtRoundOff.Text = "0";
        //    //if (txtCostFactor.Text == "") txtCostFactor.Text = "0";

        //    //'Tethering to itemwise rate
        //    double mytaxable = 0;
        //    double MyPRate = 0;
        //    double MyQty;
        //    double perpieceaddcharges;

        //    for (int k = 0; k < dgvSales.Rows.Count; k++)
        //    {
        //        if (dgvSales.Rows[k].Cells[GetEnum(gridColIndexes.CItemName)].Value != null)
        //        {
        //            if (dgvSales.Rows[k].Cells[GetEnum(gridColIndexes.CItemName)].Value.ToString() != "")
        //            {
        //                //if (Convert.ToDouble(dgvSales.Rows[k].Cells[GetEnum(gridColIndexes.CItemName)].Tag) > 0)
        //                //{
        //                if (dgvSales.Rows[k].Cells[GetEnum(gridColIndexes.cQty)].Value == null)
        //                    SetValue(GetEnum(gridColIndexes.cQty), k, AppSettings.QtyDecimalFormat);
        //                if (dgvSales.Rows[k].Cells[GetEnum(gridColIndexes.cFree)].Value == null)
        //                    SetValue(GetEnum(gridColIndexes.cFree), k, AppSettings.QtyDecimalFormat);

        //                mytaxable = Convert.ToDouble(dgvSales.Rows[k].Cells[GetEnum(gridColIndexes.ctaxable)].Value) + Convert.ToDouble(dgvSales.Rows[k].Cells[GetEnum(gridColIndexes.cNonTaxable)].Value);
        //                MyPRate = 0;
        //                perpieceaddcharges = 0;
        //                MyQty = Convert.ToDouble(dgvSales.Rows[k].Cells[GetEnum(gridColIndexes.cQty)].Value);// + Convert.ToDouble(dgvSales.Rows[k].Cells[GetEnum(gridColIndexes.cFree)].Value);
        //                                                                                                     //Dipu on 25-May-2022 -- Free Value Commented
        //                if ((dbltaxableAmountTot + dblNontaxableAmountTot) > 0)
        //                {
        //                    if (MyQty > 0)
        //                        perpieceaddcharges = (AdditionalCharges / (dbltaxableAmountTot + dblNontaxableAmountTot) * mytaxable) / (Convert.ToDouble(dgvSales.Rows[k].Cells[GetEnum(gridColIndexes.cQty)].Value) + Convert.ToDouble(dgvSales.Rows[k].Cells[GetEnum(gridColIndexes.cFree)].Value));
        //                }
        //                //perpieceaddcharges = (AdditionalCharges / (dbltaxableAmountTot + dblNontaxableAmountTot) * mytaxable) / Convert.ToDouble(dgvSales.Rows[k].Cells[GetEnum(gridColIndexes.cQty)].Value);
        //                double MyPrateWithtax = 0;

        //                if (mytaxable > 0)
        //                {
        //                    //MyPRate = mytaxable / Convert.ToDouble(dgvSales.Rows[k].Cells[GetEnum(gridColIndexes.cQty)].Value) + Convert.ToDouble(dgvSales.Rows[k].Cells[GetEnum(gridColIndexes.cFree)].Value);
        //                    MyPRate = mytaxable / Convert.ToDouble(dgvSales.Rows[k].Cells[GetEnum(gridColIndexes.cQty)].Value);
        //                    MyPrateWithtax = (mytaxable + Convert.ToDouble(dgvSales.Rows[k].Cells[GetEnum(gridColIndexes.ctax)].Value)) / (Convert.ToDouble(dgvSales.Rows[k].Cells[GetEnum(gridColIndexes.cQty)].Value));
        //                }

        //                //Distributing CommonValues Betweeen Items

        //                SetValue(GetEnum(gridColIndexes.cSrate), k, Comm.FormatValue(Convert.ToDouble(dgvSales.Rows[k].Cells[GetEnum(gridColIndexes.cSrate)].Value))); //cRate <--> cPrate
        //                                                                                                                                                          //DGVItem.Item(cPrate, i).Value = DGVItem.Item(cRate, i).Value
        //                                                                                                                                                          //MyPRate = MyPRate; // + perpieceaddcharges;
        //                                                                                                                                                          //Added by Dipu on 23-Nov-2021 ---------------->>
        //                                                                                                                                                          //MyPrateWithtax = MyPrateWithtax; // + perpieceaddcharges;

        //                //-----------------------------12-Aug-2022 arun
        //                //SetValue(GetEnum(gridColIndexes.cCrate), k, Comm.FormatValue(MyPRate));
        //                //SetValue(GetEnum(gridColIndexes.cCRateWithTax), k, Comm.FormatValue(MyPrateWithtax));
        //                if (MyPRate > 0)
        //                {
        //                    //NotifyIcon("Sales Value Calculation", MyPRate)
        //                    //MessageBox.Show("Sales Value Calculation (" + MyPRate + ")");
        //                }

        //                //BLNRECALCULATESalesRatesOnPercentag
        //                //if (clsGetStockInSett.BLNRECALCULATESalesRatesOnPercentage == true)
        //                //{
        //                //double dblcSRate1Per = Convert.ToDouble(dgvSales.Rows[k].Cells[GetEnum(gridColIndexes.cSRate1Per)].Value);
        //                //double dblcsRate2Per = Convert.ToDouble(dgvSales.Rows[k].Cells[GetEnum(gridColIndexes.cSRate2Per)].Value);
        //                //double dblcsRate3Per = Convert.ToDouble(dgvSales.Rows[k].Cells[GetEnum(gridColIndexes.cSRate3Per)].Value);
        //                //double dblcsRate4Per = Convert.ToDouble(dgvSales.Rows[k].Cells[GetEnum(gridColIndexes.cSRate4Per)].Value);
        //                //double dblcsRate5Per = Convert.ToDouble(dgvSales.Rows[k].Cells[GetEnum(gridColIndexes.cSRate5Per)].Value);

        //                //double dblcRate = Convert.ToDouble(dgvSales.Rows[k].Cells[GetEnum(gridColIndexes.cSrate)].Value);
        //                //double dblcCRate = Convert.ToDouble(dgvSales.Rows[k].Cells[GetEnum(gridColIndexes.cCrate)].Value);
        //                //double dblcMRP = Convert.ToDouble(dgvSales.Rows[k].Cells[GetEnum(gridColIndexes.cMRP)].Value);
        //                //double dblcCRateWithTax = Convert.ToDouble(dgvSales.Rows[k].Cells[GetEnum(gridColIndexes.cCRateWithTax)].Value);

        //                //switch (Convert.ToDouble(dgvSales.Rows[k].Cells[GetEnum(gridColIndexes.cSRate1Per)].Tag)) //DiscMode
        //                //{
        //                //    case 0:
        //                //        if (dblcSRate1Per > 0) SetValue(GetEnum(gridColIndexes.cSRate1), k, Comm.FormatValue((dblcRate + dblcRate * dblcSRate1Per / 100)));
        //                //        if (dblcsRate2Per > 0) SetValue(GetEnum(gridColIndexes.cSRate2), k, Comm.FormatValue((dblcRate + dblcRate * dblcsRate2Per / 100)));
        //                //        if (dblcsRate3Per > 0) SetValue(GetEnum(gridColIndexes.cSRate3), k, Comm.FormatValue((dblcRate + dblcRate * dblcsRate3Per / 100)));
        //                //        if (dblcsRate4Per > 0) SetValue(GetEnum(gridColIndexes.cSRate4), k, Comm.FormatValue((dblcRate + dblcRate * dblcsRate4Per / 100)));
        //                //        if (dblcsRate5Per > 0) SetValue(GetEnum(gridColIndexes.cSRate5), k, Comm.FormatValue((dblcRate + dblcRate * dblcsRate5Per / 100)));
        //                //        break;
        //                //    case 3:
        //                //        if (dblcSRate1Per > 0) SetValue(GetEnum(gridColIndexes.cSRate1), k, Comm.FormatValue(dblcCRate + dblcCRate * dblcSRate1Per / 100));
        //                //        if (dblcsRate2Per > 0) SetValue(GetEnum(gridColIndexes.cSRate2), k, Comm.FormatValue(dblcCRate + dblcCRate * dblcsRate2Per / 100));
        //                //        if (dblcsRate3Per > 0) SetValue(GetEnum(gridColIndexes.cSRate3), k, Comm.FormatValue(dblcCRate + dblcCRate * dblcsRate3Per / 100));
        //                //        if (dblcsRate4Per > 0) SetValue(GetEnum(gridColIndexes.cSRate4), k, Comm.FormatValue(dblcCRate + dblcCRate * dblcsRate4Per / 100));
        //                //        if (dblcsRate5Per > 0) SetValue(GetEnum(gridColIndexes.cSRate5), k, Comm.FormatValue(dblcCRate + dblcCRate * dblcsRate5Per / 100));
        //                //        break;
        //                //    case 1:
        //                //        if (dblcSRate1Per > 0) SetValue(GetEnum(gridColIndexes.cSRate1), k, Comm.FormatValue(dblcMRP - dblcMRP * dblcSRate1Per / 100));
        //                //        if (dblcsRate2Per > 0) SetValue(GetEnum(gridColIndexes.cSRate2), k, Comm.FormatValue(dblcMRP - dblcMRP * dblcsRate2Per / 100));
        //                //        if (dblcsRate3Per > 0) SetValue(GetEnum(gridColIndexes.cSRate3), k, Comm.FormatValue(dblcMRP - dblcMRP * dblcsRate3Per / 100));
        //                //        if (dblcsRate4Per > 0) SetValue(GetEnum(gridColIndexes.cSRate4), k, Comm.FormatValue(dblcMRP - dblcMRP * dblcsRate4Per / 100));
        //                //        if (dblcsRate5Per > 0) SetValue(GetEnum(gridColIndexes.cSRate5), k, Comm.FormatValue(dblcMRP - dblcMRP * dblcsRate5Per / 100));
        //                //        break;
        //                //    case 2:
        //                //        if (dblcSRate1Per > 0) SetValue(GetEnum(gridColIndexes.cSRate1), k, Comm.FormatValue(dblcCRateWithTax + dblcCRateWithTax * dblcSRate1Per / 100));
        //                //        if (dblcsRate2Per > 0) SetValue(GetEnum(gridColIndexes.cSRate2), k, Comm.FormatValue(dblcCRateWithTax + dblcCRateWithTax * dblcsRate2Per / 100));
        //                //        if (dblcsRate3Per > 0) SetValue(GetEnum(gridColIndexes.cSRate3), k, Comm.FormatValue(dblcCRateWithTax + dblcCRateWithTax * dblcsRate3Per / 100));
        //                //        if (dblcsRate4Per > 0) SetValue(GetEnum(gridColIndexes.cSRate4), k, Comm.FormatValue(dblcCRateWithTax + dblcCRateWithTax * dblcsRate4Per / 100));
        //                //        if (dblcsRate5Per > 0) SetValue(GetEnum(gridColIndexes.cSRate5), k, Comm.FormatValue(dblcCRateWithTax + dblcCRateWithTax * dblcsRate5Per / 100));
        //                //        break;
        //                //}
        //                //}
        //                //double SavingsofItem = (Convert.ToDouble(dgvSales.Rows[k].Cells[GetEnum(gridColIndexes.cMRP)].Value) * MyQty) - (Val(DGVItem.Item(cRate, i).Value) * MyQty);
        //                SavingsofItem = (Convert.ToDouble(dgvSales.Rows[k].Cells[GetEnum(gridColIndexes.cMRP)].Value) * MyQty) - (Convert.ToDouble(dgvSales.Rows[k].Cells[GetEnum(gridColIndexes.cSrate)].Value) * MyQty); //cRate <--> cPrate
        //                if (MyQty > 0) Savings = Savings + SavingsofItem;



        //                //}
        //            }
        //        }
        //    }
        //    //ItemDiscount and Discount Amount are equal ??
        //    Savings = Savings + Convert.ToDouble(txtDiscAmt.Text) + Convert.ToDouble(txtDiscAmt.Text) + Convert.ToDouble(txtCashDisc.Text) - Convert.ToDouble(txtOtherExp.Text);
        //    // dgv.SetValue(LinkIDs.Savings, Comm.FormatAmt(Val(Val(Savings)), DCSApp.Gdecimal))


        //    //if (Convert.ToDouble(lblBillAmount.Text) > 1000000000)// && outoflimitbillvalue == 0)
        //    //{
        //    //    //NotifyIcon("Sales Value Calculation", "Software is unable to calculate higher values than 100000000. Kindly limit the values or correct if given by mistake")
        //    //    MessageBox.Show("Software is unable to calculate higher values than 100000000. Kindly limit the values or correct if given by mistake", "Sales Value Calculation");
        //    //    //outoflimitbillvalue = Convert.ToDouble(lblBillAmount.Text);
        //    //    lblBillAmount.Text = "000";
        //    //}

        //    //WriteToPoleDisplay(StrLastAddeddItemForPOleDisplay, "Amount :" & lblBalance.Text)
        //    //'Dim NoConv As New DcsDll.NoConversion
        //    //' NotifyIcon("", NoConv.NoConvertion(lblBalance.Text, True, "Rupees", "RS", False))
        //    //' Dim t As New Translator()
        //    //'txtInwords.Text = t.Translate(txtInwords.Text, "English", "Malayalam")
        //    //Me.Text = mytrans.MVchType & " .............. [" & IIf(mytrans.BlnEditMode, "Edit Mode", "New Mode") & "] ................VchNo : " & txtvchnoPrefix.Text.ToString & txtVchNo.Text.ToString & "............Party : " & txtpartySearch.Text
        //    //mecaption.Text = Me.Text
        //    //}
        //    }

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

            for (int i = 0; i < dgvStockOut.Rows.Count; i++)
            {
                SetValue(GetEnum(gridColIndexes.cSlNo), i, (i + 1).ToString());
                if (dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.CItemName)].Value != null)
                {
                    if (dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.CItemName)].Value.ToString() != "")
                    {
                        if (Comm.ToDecimal(dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cItemID)].Value) != 0)
                        {
                            if (dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cQty)].Value == null)
                                SetValue(GetEnum(gridColIndexes.cQty), i, "0");
                            if (dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cFree)].Value == null)
                                SetValue(GetEnum(gridColIndexes.cFree), i, "0");

                            DblRate = Comm.ToDouble(dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cSrate)].Value);
                            //Dipu on 13-May-2022 ---------- >
                            dblQty = Comm.ToDouble(dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cQty)].Value);
                            //dblQty = Comm.ToDouble(dgvSales.Rows[i].Cells[GetEnum(gridColIndexes.cQty)].Value) + Comm.ToDouble(dgvSales.Rows[i].Cells[GetEnum(gridColIndexes.cFree)].Value);
                            //Dipu on 25-May-2022 -- Free Value Commented
                            QtyTotal = QtyTotal + dblQty;// + Comm.ToDouble(dgvSales.Rows[i].Cells[GetEnum(gridColIndexes.cFree)].Value);

                            //SetValue(GetEnum(gridColIndexes.cSlNo), i, (i + 1).ToString());

                            //DblrateDiscper = Comm.ToDouble(dgvSales.Rows[i].Cells[GetEnum(gridColIndexes.cRateDiscPer)].Value);
                            DblRateAfterRDiscount = DblRate - (DblRate * DblrateDiscper / 100);

                            if (blnCalculateCoolie == true)
                            {
                                CoolieTotal += Comm.ToDouble(dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cCoolie)].Value);
                            }

                            dblTaxPer = Comm.ToDouble(dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.ctaxPer)].Value);
                            dblCessPer = Comm.ToDouble(dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cCCessPer)].Value);
                            dblQtyCessPer = Comm.ToDouble(dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cCCompCessQty)].Value);
                            if (dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cFloodCessPer)].Value == null)
                                SetValue(GetEnum(gridColIndexes.cFloodCessPer), i, "");

                            //If chkApplyFloodCess.CheckState = CheckState.Checked Then
                            if (dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cFloodCessPer)].Value.ToString() == "")
                                SetValue(GetEnum(gridColIndexes.cFloodCessPer), i, "0");
                            dblFloodCessPer = Comm.ToDouble(dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cFloodCessPer)].Value);
                            //End If

                            if (clsVchType.DefaultTaxInclusiveValue == 2)
                                dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cRateinclusive)].Value = true;
                            else if (clsVchType.DefaultTaxInclusiveValue == 3)
                                dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cRateinclusive)].Value = false;

                            if (Convert.ToBoolean(dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cRateinclusive)].Value) == true)
                                DblRateExclusive = GetRateExclusive(DblRateAfterRDiscount, (dblCessPer + dblTaxPer + dblFloodCessPer), 0);
                            else
                                DblRateExclusive = DblRateAfterRDiscount;

                            dblGrossValue = DblRateExclusive * dblQty;
                            SetValue(GetEnum(gridColIndexes.cGrossAmt), i, Comm.FormatValue(dblGrossValue));
                            dblGrossValueTot = dblGrossValueTot + dblGrossValue;
                            dblGrossValueAfterRateDiscount = dblQty * (DblRateExclusive);

                            dblQtyTot += Comm.ToDouble(dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cQty)].Value);
                            dblFreeTot += Comm.ToDouble(dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cFree)].Value);

                            SetValue(GetEnum(gridColIndexes.cGrossValueAfterRateDiscount), i, Comm.FormatValue(dblGrossValueAfterRateDiscount));

                            DblrateDiscAmt = dblQty * (DblRate - DblRateAfterRDiscount);
                            DblrateDiscAmtTot = DblrateDiscAmtTot + DblrateDiscAmt;

                            dblGrossValueAfterRateDiscountTot = dblGrossValueAfterRateDiscountTot + dblGrossValueAfterRateDiscount;
                            //dbltaxableValueAfterItemDiscountTot = dblGrossValueAfterDiscountsTot;

                            if (Comm.ToDouble(dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cDiscPer)].Value) > 0)
                            {
                                SetValue(GetEnum(gridColIndexes.cDiscAmount), i, Comm.FormatValue((dblGrossValueAfterRateDiscount * Comm.ToDouble(dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cDiscPer)].Value) / 100)));
                                dblItemDiscAmountTot = dblItemDiscAmountTot + Comm.ToDouble(dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cDiscAmount)].Value);
                            }
                            else if (Comm.ToDouble(dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cDiscAmount)].Value) > 0)
                            {
                                SetValue(GetEnum(gridColIndexes.cDiscAmount), i, Comm.FormatValue(Comm.ToDouble(dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cDiscAmount)].Value)));
                                dblItemDiscAmountTot = dblItemDiscAmountTot + Comm.ToDouble(dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cDiscAmount)].Value);
                            }

                            dblGrossValueAfterDiscounts = dblGrossValueAfterRateDiscount - Comm.ToDouble(dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cDiscAmount)].Value);
                            dblGrossValueAfterDiscountsTot = dblGrossValueAfterDiscountsTot + dblGrossValueAfterDiscounts;
                            //
                            //Arrived Taxable Value
                            dbltaxableValueAfterItemDiscount = dblGrossValueAfterDiscounts;
                            dbltaxableValueAfterItemDiscountTot = dblGrossValueAfterDiscountsTot;
                            SetValue(GetEnum(gridColIndexes.cGrossValueAfterRateDiscount), i, Comm.FormatValue(dbltaxableValueAfterItemDiscount));

                            iTaxMode = Comm.ToInt32(cboTaxMode.SelectedValue) - 1;

                            if (Comm.ToDouble(dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.ctaxPer)].Value) > 0)
                            {
                                DblNontaxableValue = 0;
                                dbltaxAmount = dbltaxableValueAfterItemDiscount * Comm.ToDouble(dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.ctaxPer)].Value) / 100;
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
                                if (Comm.ToDouble(dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.ctaxPer)].Value) > 0)
                                {
                                    SetValue(GetEnum(gridColIndexes.ctaxable), i, Comm.FormatValue(dbltaxableValueAfterItemDiscount));
                                    SetValue(GetEnum(gridColIndexes.cNonTaxable), i, "0");
                                }
                                else
                                {
                                    SetValue(GetEnum(gridColIndexes.ctaxable), i, "0");
                                    SetValue(GetEnum(gridColIndexes.cNonTaxable), i, Comm.FormatValue(dbltaxableValueAfterItemDiscount));
                                }
                                SetValue(GetEnum(gridColIndexes.ctax), i, Comm.FormatValue(dbltaxAmount));
                                if (cboState.SelectedValue != null)
                                {
                                    if (cboState.SelectedValue.ToString() != AppSettings.StateCode)
                                    {
                                        SetValue(GetEnum(gridColIndexes.cCGST), i, "0");
                                        SetValue(GetEnum(gridColIndexes.cSGST), i, "0");
                                        SetValue(GetEnum(gridColIndexes.cIGST), i, Comm.FormatValue(dbltaxAmount));
                                    }
                                    else
                                    {
                                        SetValue(GetEnum(gridColIndexes.cCGST), i, Comm.FormatValue((dbltaxAmount * 0.5)));
                                        SetValue(GetEnum(gridColIndexes.cSGST), i, Comm.FormatValue((dbltaxAmount * 0.5)));
                                        SetValue(GetEnum(gridColIndexes.cIGST), i, "0");
                                    }
                                }
                                else
                                {
                                    SetValue(GetEnum(gridColIndexes.cCGST), i, Comm.FormatValue((dbltaxAmount * 0.5)));
                                    SetValue(GetEnum(gridColIndexes.cSGST), i, Comm.FormatValue((dbltaxAmount * 0.5)));
                                    SetValue(GetEnum(gridColIndexes.cIGST), i, "0");
                                }

                                DbltaxAmountTot = DbltaxAmountTot + dbltaxAmount;
                                SetValue(GetEnum(gridColIndexes.cNetAmount), i, Comm.FormatValue((dbltaxableValueAfterItemDiscount + dbltaxAmount)));
                                DblNetAmountTotal = DblNetAmountTotal + Comm.ToDouble(dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cNetAmount)].Value);
                            }
                            else if (iTaxMode == 1) //VAT
                            {
                                if (Comm.ToDouble(dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.ctaxPer)].Value) > 0)
                                {
                                    SetValue(GetEnum(gridColIndexes.ctaxable), i, Comm.FormatValue(dbltaxableValueAfterItemDiscount));
                                    SetValue(GetEnum(gridColIndexes.cNonTaxable), i, "0");

                                    SetValue(GetEnum(gridColIndexes.cCGST), i, "0");
                                    SetValue(GetEnum(gridColIndexes.cSGST), i, "0");
                                    SetValue(GetEnum(gridColIndexes.cIGST), i, Comm.FormatValue(dbltaxAmount));

                                }
                                else
                                {
                                    SetValue(GetEnum(gridColIndexes.ctaxable), i, "0");
                                    SetValue(GetEnum(gridColIndexes.cNonTaxable), i, Comm.FormatValue(dbltaxableValueAfterItemDiscount));
                                }
                                SetValue(GetEnum(gridColIndexes.ctax), i, Comm.FormatValue(dbltaxAmount));

                                DbltaxAmountTot = DbltaxAmountTot + dbltaxAmount;
                                SetValue(GetEnum(gridColIndexes.cNetAmount), i, Comm.FormatValue((dbltaxableValueAfterItemDiscount + dbltaxAmount)));
                                DblNetAmountTotal = DblNetAmountTotal + Comm.ToDouble(dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cNetAmount)].Value);
                            }
                            else if (iTaxMode == 0) //NONE
                            {
                                SetValue(GetEnum(gridColIndexes.ctaxable), i, Comm.FormatValue(dbltaxableValueAfterItemDiscount));
                                SetValue(GetEnum(gridColIndexes.cNonTaxable), i, Comm.FormatValue(DblNontaxableValue));
                                //Check Dipu

                                SetValue(GetEnum(gridColIndexes.cCGST), i, "0");
                                SetValue(GetEnum(gridColIndexes.cSGST), i, "0");
                                SetValue(GetEnum(gridColIndexes.cIGST), i, "0");

                                SetValue(GetEnum(gridColIndexes.ctax), i, "0");


                                //DbltaxAmountTot = DbltaxAmountTot + dbltaxAmount;
                                SetValue(GetEnum(gridColIndexes.cNetAmount), i, Comm.FormatValue((dbltaxableValueAfterItemDiscount)));
                                DblNetAmountTotal = DblNetAmountTotal + Comm.ToDouble(dgvStockOut.Rows[i].Cells[GetEnum(gridColIndexes.cNetAmount)].Value);
                            }
                            //DbltaxAmountTot = DbltaxAmountTot + dbltaxAmount;
                            //SetValue(GetEnum(gridColIndexes.cNetAmount), i, Comm.FormatValue((dbltaxableValueAfterItemDiscount + dbltaxAmount)));
                            //DblNetAmountTotal = DblNetAmountTotal + Comm.ToDouble(dgvSales.Rows[i].Cells[GetEnum(gridColIndexes.cNetAmount)].Value);

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

            for (int j = 0; j < dgvStockOut.Rows.Count; j++)
            {
                if (dgvStockOut.Rows[j].Cells[GetEnum(gridColIndexes.CItemName)].Value != null)
                {
                    if (dgvStockOut.Rows[j].Cells[GetEnum(gridColIndexes.CItemName)].Value.ToString() != "")
                    {
                        dblTaxPer = Comm.ToDouble(dgvStockOut.Rows[j].Cells[GetEnum(gridColIndexes.ctaxPer)].Value);
                        dblCessPer = Comm.ToDouble(dgvStockOut.Rows[j].Cells[GetEnum(gridColIndexes.cCCessPer)].Value);
                        dblQtyCessPer = Comm.ToDouble(dgvStockOut.Rows[j].Cells[GetEnum(gridColIndexes.cCCompCessQty)].Value);
                        // check from Settings
                        dblFloodCessPer = 0;

                        SetValue(GetEnum(gridColIndexes.cBillDisc), j, "0");
                        dblGrossValueAfterDiscounts = Comm.ToDouble(dgvStockOut.Rows[j].Cells[GetEnum(gridColIndexes.cGrossValueAfterRateDiscount)].Value);
                        if (dblGrossValueAfterDiscountsTot > 0)
                            SetValue(GetEnum(gridColIndexes.cBillDisc), j, Comm.FormatValue((Discountamount / dblGrossValueAfterDiscountsTot * dblGrossValueAfterDiscounts)));

                        if (Comm.ToDouble(dgvStockOut.Rows[j].Cells[GetEnum(gridColIndexes.ctaxPer)].Value) > 0)
                        {
                            dbltaxableAmount = Comm.ToDouble(dgvStockOut.Rows[j].Cells[GetEnum(gridColIndexes.ctaxable)].Value) - Comm.ToDouble(dgvStockOut.Rows[j].Cells[GetEnum(gridColIndexes.cBillDisc)].Value);
                            DblNontaxableValue = 0;
                        }
                        else
                        {
                            DblNontaxableValue = Comm.ToDouble(dgvStockOut.Rows[j].Cells[GetEnum(gridColIndexes.cNonTaxable)].Value) - Comm.ToDouble(dgvStockOut.Rows[j].Cells[GetEnum(gridColIndexes.cBillDisc)].Value);
                            dbltaxableAmount = 0;
                        }

                        SetValue(GetEnum(gridColIndexes.ctaxable), j, Comm.FormatValue(dbltaxableAmount));
                        SetValue(GetEnum(gridColIndexes.cNonTaxable), j, Comm.FormatValue(DblNontaxableValue));

                        dbltaxAmount = Comm.ToDouble(dgvStockOut.Rows[j].Cells[GetEnum(gridColIndexes.ctaxable)].Value) * Comm.ToDouble(dgvStockOut.Rows[j].Cells[GetEnum(gridColIndexes.ctaxPer)].Value) / 100;
                        DblcessAmount = Comm.ToDouble(dgvStockOut.Rows[j].Cells[GetEnum(gridColIndexes.ctaxable)].Value) * dblCessPer / 100;
                        DblCompcessAmount = Comm.ToDouble(dgvStockOut.Rows[j].Cells[GetEnum(gridColIndexes.cQty)].Value) * dblQtyCessPer;
                        DblFloodcessAmount = Comm.ToDouble(dgvStockOut.Rows[j].Cells[GetEnum(gridColIndexes.ctaxable)].Value) * dblFloodCessPer / 100;

                        SetTag(GetEnum(gridColIndexes.cCCessPer), j, Comm.FormatValue(DblcessAmount, true, "#.00"));
                        SetTag(GetEnum(gridColIndexes.cCCompCessQty), j, Comm.FormatValue(DblCompcessAmount, false));

                        SetValue(GetEnum(gridColIndexes.cFloodCessAmt), j, Comm.FormatValue(DblFloodcessAmount));
                        DblFloodcessAmountTot = DblFloodcessAmountTot + DblFloodcessAmount;
                        DblcessAmountTot = DblcessAmountTot + DblcessAmount;
                        DblCompcessAmountTot = DblCompcessAmountTot + DblCompcessAmount;

                        if (iTaxMode == 2) //GST
                        {
                            SetValue(GetEnum(gridColIndexes.ctax), j, Comm.FormatValue(dbltaxAmount));
                            if (Comm.ToDouble(dgvStockOut.Rows[j].Cells[GetEnum(gridColIndexes.ctaxPer)].Value) > 0)
                            {
                                SetValue(GetEnum(gridColIndexes.ctaxable), j, Comm.FormatValue(Comm.ToDouble(dgvStockOut.Rows[j].Cells[GetEnum(gridColIndexes.ctaxable)].Value)));
                                SetValue(GetEnum(gridColIndexes.cNonTaxable), j, "0");
                            }
                            else
                            {
                                SetValue(GetEnum(gridColIndexes.cNonTaxable), j, Comm.FormatValue(Comm.ToDouble(dgvStockOut.Rows[j].Cells[GetEnum(gridColIndexes.cNonTaxable)].Value)));
                                SetValue(GetEnum(gridColIndexes.ctaxable), j, "0");
                            }

                            if (cboState.SelectedValue != null)
                            {
                                if (cboState.SelectedValue.ToString() != AppSettings.StateCode)
                                {
                                    SetValue(GetEnum(gridColIndexes.cCGST), j, "0");
                                    SetValue(GetEnum(gridColIndexes.cSGST), j, "0");
                                    SetValue(GetEnum(gridColIndexes.cIGST), j, Comm.FormatValue(dbltaxAmount));
                                    SetTag(GetEnum(gridColIndexes.cCGST), j, "0"); ;

                                    SetTag(GetEnum(gridColIndexes.cSGST), j, "0");
                                    SetTag(GetEnum(gridColIndexes.cIGST), j, Comm.ToDouble(dgvStockOut.Rows[j].Cells[GetEnum(gridColIndexes.ctaxPer)].Value).ToString());
                                }
                                else
                                {
                                    SetValue(GetEnum(gridColIndexes.cCGST), j, Comm.FormatValue((dbltaxAmount * 0.5)));
                                    SetValue(GetEnum(gridColIndexes.cSGST), j, Comm.FormatValue((dbltaxAmount * 0.5)));
                                    SetValue(GetEnum(gridColIndexes.cIGST), j, "0");

                                    SetTag(GetEnum(gridColIndexes.cCGST), j, (Comm.ToDouble(dgvStockOut.Rows[j].Cells[GetEnum(gridColIndexes.ctaxPer)].Value) * 0.5).ToString());
                                    SetTag(GetEnum(gridColIndexes.cSGST), j, (Comm.ToDouble(dgvStockOut.Rows[j].Cells[GetEnum(gridColIndexes.ctaxPer)].Value) * 0.5).ToString());
                                    SetTag(GetEnum(gridColIndexes.cIGST), j, "0");

                                }
                            }
                            else
                            {
                                SetValue(GetEnum(gridColIndexes.cCGST), j, Comm.FormatValue((dbltaxAmount * 0.5)));
                                SetValue(GetEnum(gridColIndexes.cSGST), j, Comm.FormatValue((dbltaxAmount * 0.5)));
                                SetValue(GetEnum(gridColIndexes.cIGST), j, "0");

                                SetTag(GetEnum(gridColIndexes.cCGST), j, (Comm.ToDouble(dgvStockOut.Rows[j].Cells[GetEnum(gridColIndexes.ctaxPer)].Value) * 0.5).ToString());
                                SetTag(GetEnum(gridColIndexes.cSGST), j, (Comm.ToDouble(dgvStockOut.Rows[j].Cells[GetEnum(gridColIndexes.ctaxPer)].Value) * 0.5).ToString());
                                SetTag(GetEnum(gridColIndexes.cIGST), j, "0");
                            }
                        }
                        else if (iTaxMode == 1) //VAT
                        {
                            SetValue(GetEnum(gridColIndexes.ctax), j, Comm.FormatValue(dbltaxAmount));
                            if (Comm.ToDouble(dgvStockOut.Rows[j].Cells[GetEnum(gridColIndexes.ctaxPer)].Value) > 0)
                            {
                                SetValue(GetEnum(gridColIndexes.ctaxable), j, Comm.FormatValue(Comm.ToDouble(dgvStockOut.Rows[j].Cells[GetEnum(gridColIndexes.ctaxable)].Value)));
                                SetValue(GetEnum(gridColIndexes.cNonTaxable), j, "0");
                            }
                            else
                            {
                                SetValue(GetEnum(gridColIndexes.cNonTaxable), j, Comm.FormatValue(Comm.ToDouble(dgvStockOut.Rows[j].Cells[GetEnum(gridColIndexes.cNonTaxable)].Value)));
                                SetValue(GetEnum(gridColIndexes.ctaxable), j, "0");
                            }

                            SetValue(GetEnum(gridColIndexes.cCGST), j, "0");
                            SetValue(GetEnum(gridColIndexes.cSGST), j, "0");
                            SetValue(GetEnum(gridColIndexes.cIGST), j, Comm.FormatValue(dbltaxAmount));
                            SetTag(GetEnum(gridColIndexes.cCGST), j, "0"); ;

                            SetTag(GetEnum(gridColIndexes.cSGST), j, "0");
                            SetTag(GetEnum(gridColIndexes.cIGST), j, Comm.ToDouble(dgvStockOut.Rows[j].Cells[GetEnum(gridColIndexes.ctaxPer)].Value).ToString());
                        }
                        else if (iTaxMode == 0) //NONE
                        {
                            //SetValue(GetEnum(gridColIndexes.ctaxable), j, Comm.FormatValue(dbltaxableValueAfterItemDiscount));
                            SetValue(GetEnum(gridColIndexes.ctaxable), j, Comm.FormatValue(dbltaxableAmount));
                            SetValue(GetEnum(gridColIndexes.cNonTaxable), j, Comm.FormatValue(DblNontaxableValue));
                            //Check Dipu
                            //SetValue(GetEnum(gridColIndexes.ctaxable), j, Comm.FormatValue(dbltaxableAmount));

                            SetValue(GetEnum(gridColIndexes.cCGST), j, "0");
                            SetValue(GetEnum(gridColIndexes.cSGST), j, "0");
                            SetValue(GetEnum(gridColIndexes.cIGST), j, "0");

                            //SetValue(GetEnum(gridColIndexes.cNetAmount), j, Comm.FormatValue((dbltaxableValueAfterItemDiscount)));
                        }

                        dblIGSTTot = dblIGSTTot + Comm.ToDouble(dgvStockOut.Rows[j].Cells[GetEnum(gridColIndexes.cIGST)].Value);
                        dblSSGTTot = dblSSGTTot + Comm.ToDouble(dgvStockOut.Rows[j].Cells[GetEnum(gridColIndexes.cSGST)].Value);
                        dblCSGTTot = dblCSGTTot + Comm.ToDouble(dgvStockOut.Rows[j].Cells[GetEnum(gridColIndexes.cCGST)].Value);

                        dbltaxAmountTot = dbltaxAmountTot + dbltaxAmount;
                        //dbltaxAmountTot = Comm.FormatAmt(Val(dbltaxAmountTot) + Val(Format(Val(dbltaxAmount), DCSApp.GBizAmt)), DCSApp.GBizAmt)
                        // dont know how to format ??

                        dbltaxableAmountTot = dbltaxableAmountTot + Comm.ToDouble(dgvStockOut.Rows[j].Cells[GetEnum(gridColIndexes.ctaxable)].Value);
                        dblNontaxableAmountTot = dblNontaxableAmountTot + Comm.ToDouble(dgvStockOut.Rows[j].Cells[GetEnum(gridColIndexes.cNonTaxable)].Value);

                        //DGVItem.Item(cNetAmount, i).Value = Comm.FormatAmt(Val(DGVItem.Item(ctaxable, i).Value) + Val(DGVItem.Item(cNonTaxable, i).Value) + Val(DGVItem.Item(ctax, i).Value) + Val(DblcessAmount) + Val(DblFloodcessAmount) + Val(DblCompcessAmount), "")
                        //Dont know what is Comm.FormatAmt ->
                        //if (iTaxMode != 0) //NOT NONE

                        SetValue(GetEnum(gridColIndexes.cNetAmount), j, Comm.FormatValue((Comm.ToDouble(dgvStockOut.Rows[j].Cells[GetEnum(gridColIndexes.ctaxable)].Value) + Comm.ToDouble(dgvStockOut.Rows[j].Cells[GetEnum(gridColIndexes.cNonTaxable)].Value) + Comm.ToDouble(dgvStockOut.Rows[j].Cells[GetEnum(gridColIndexes.ctax)].Value) + DblcessAmount + DblFloodcessAmount + DblCompcessAmount)));
                        DblNetAmountTotal = DblNetAmountTotal + Comm.ToDouble(dgvStockOut.Rows[j].Cells[GetEnum(gridColIndexes.cNetAmount)].Value);

                        //valuation of Free
                        dblQty = Comm.ToDouble(dgvStockOut.Rows[j].Cells[GetEnum(gridColIndexes.cQty)].Value);
                        if (Comm.ToDouble(dgvStockOut.Rows[j].Cells[GetEnum(gridColIndexes.cFree)].Value) > 0)
                        {
                            double PerItemRate = Comm.ToDouble(dgvStockOut.Rows[j].Cells[GetEnum(gridColIndexes.cNetAmount)].Value) - Comm.ToDouble(dgvStockOut.Rows[j].Cells[GetEnum(gridColIndexes.ctax)].Value) / dblQty;
                            TotalValueOfFree = TotalValueOfFree + (PerItemRate * Comm.ToDouble(dgvStockOut.Rows[j].Cells[GetEnum(gridColIndexes.cFree)].Value));
                        }

                        //CALCULATION DECIMAL CHANGING
                        SetValue(GetEnum(gridColIndexes.cDiscAmount), j, Comm.FormatValue(Comm.ToDouble(dgvStockOut.Rows[j].Cells[GetEnum(gridColIndexes.cDiscAmount)].Value)));
                        SetValue(GetEnum(gridColIndexes.cBillDisc), j, Comm.FormatValue(Comm.ToDouble(dgvStockOut.Rows[j].Cells[GetEnum(gridColIndexes.cBillDisc)].Value)));

                        SetValue(GetEnum(gridColIndexes.ctaxable), j, Comm.FormatValue(Comm.ToDouble(dgvStockOut.Rows[j].Cells[GetEnum(gridColIndexes.ctaxable)].Value)));
                        SetTag(GetEnum(gridColIndexes.ctaxable), j, Comm.FormatValue(Comm.ToDouble(dgvStockOut.Rows[j].Cells[GetEnum(gridColIndexes.ctaxable)].Value)));
                        //DGVItem.Item(ctaxable, i).Tag = Format(Val(DGVItem.Item(ctaxable, i).Value), "#0.000000")
                        //Tag ??

                        SetValue(GetEnum(gridColIndexes.ctax), j, Comm.FormatValue(Comm.ToDouble(dgvStockOut.Rows[j].Cells[GetEnum(gridColIndexes.ctax)].Value)));
                        SetValue(GetEnum(gridColIndexes.cIGST), j, Comm.FormatValue(Comm.ToDouble(dgvStockOut.Rows[j].Cells[GetEnum(gridColIndexes.cIGST)].Value)));
                        SetValue(GetEnum(gridColIndexes.cSGST), j, Comm.FormatValue(Comm.ToDouble(dgvStockOut.Rows[j].Cells[GetEnum(gridColIndexes.cSGST)].Value)));
                        SetValue(GetEnum(gridColIndexes.cCGST), j, Comm.FormatValue(Comm.ToDouble(dgvStockOut.Rows[j].Cells[GetEnum(gridColIndexes.cCGST)].Value)));
                        SetValue(GetEnum(gridColIndexes.cNetAmount), j, Comm.FormatValue(Comm.ToDouble(dgvStockOut.Rows[j].Cells[GetEnum(gridColIndexes.cNetAmount)].Value)));
                        //SetValue(GetEnum(gridColIndexes.cNetAmount), j, Comm.FormatValue(Comm.ToDouble(dgvSales.Rows[j].Cells[GetEnum(gridColIndexes.cNetAmount)].Value)));
                        SetValue(GetEnum(gridColIndexes.cGrossValueAfterRateDiscount), j, Comm.FormatValue(Comm.ToDouble(dgvStockOut.Rows[j].Cells[GetEnum(gridColIndexes.cGrossValueAfterRateDiscount)].Value)));
                        SetValue(GetEnum(gridColIndexes.cNonTaxable), j, Comm.FormatValue(Comm.ToDouble(dgvStockOut.Rows[j].Cells[GetEnum(gridColIndexes.cNonTaxable)].Value)));

                        if (Comm.ToDouble(dgvStockOut.Rows[j].Cells[GetEnum(gridColIndexes.cAgentCommPer)].Value) > 0)
                            DblItemAgentCommission = (DblItemAgentCommission + Comm.ToDouble(dgvStockOut.Rows[j].Cells[GetEnum(gridColIndexes.cNetAmount)].Value) * Comm.ToDouble(dgvStockOut.Rows[j].Cells[GetEnum(gridColIndexes.cAgentCommPer)].Value) / 100);

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

            for (int k = 0; k < dgvStockOut.Rows.Count; k++)
            {
                if (dgvStockOut.Rows[k].Cells[GetEnum(gridColIndexes.CItemName)].Value != null)
                {
                    if (dgvStockOut.Rows[k].Cells[GetEnum(gridColIndexes.CItemName)].Value.ToString() != "")
                    {
                        //if (Comm.ToDouble(dgvSales.Rows[k].Cells[GetEnum(gridColIndexes.CItemName)].Tag) > 0)
                        //{
                        if (dgvStockOut.Rows[k].Cells[GetEnum(gridColIndexes.cQty)].Value == null)
                            SetValue(GetEnum(gridColIndexes.cQty), k, AppSettings.QtyDecimalFormat);
                        if (dgvStockOut.Rows[k].Cells[GetEnum(gridColIndexes.cFree)].Value == null)
                            SetValue(GetEnum(gridColIndexes.cFree), k, AppSettings.QtyDecimalFormat);

                        mytaxable = Comm.ToDouble(dgvStockOut.Rows[k].Cells[GetEnum(gridColIndexes.ctaxable)].Value) + Comm.ToDouble(dgvStockOut.Rows[k].Cells[GetEnum(gridColIndexes.cNonTaxable)].Value);
                        MyPRate = 0;
                        perpieceaddcharges = 0;
                        MyQty = Comm.ToDouble(dgvStockOut.Rows[k].Cells[GetEnum(gridColIndexes.cQty)].Value);// + Comm.ToDouble(dgvSales.Rows[k].Cells[GetEnum(gridColIndexes.cFree)].Value);
                                                                                                             //Dipu on 25-May-2022 -- Free Value Commented
                        if ((dbltaxableAmountTot + dblNontaxableAmountTot) > 0)
                        {
                            if (MyQty > 0)
                                perpieceaddcharges = (AdditionalCharges / (dbltaxableAmountTot + dblNontaxableAmountTot) * mytaxable) / (Comm.ToDouble(dgvStockOut.Rows[k].Cells[GetEnum(gridColIndexes.cQty)].Value) + Comm.ToDouble(dgvStockOut.Rows[k].Cells[GetEnum(gridColIndexes.cFree)].Value));
                        }
                        //perpieceaddcharges = (AdditionalCharges / (dbltaxableAmountTot + dblNontaxableAmountTot) * mytaxable) / Comm.ToDouble(dgvSales.Rows[k].Cells[GetEnum(gridColIndexes.cQty)].Value);
                        double MyPrateWithtax = 0;

                        if (mytaxable > 0)
                        {
                            //MyPRate = mytaxable / Comm.ToDouble(dgvSales.Rows[k].Cells[GetEnum(gridColIndexes.cQty)].Value) + Comm.ToDouble(dgvSales.Rows[k].Cells[GetEnum(gridColIndexes.cFree)].Value);
                            MyPRate = mytaxable / Comm.ToDouble(dgvStockOut.Rows[k].Cells[GetEnum(gridColIndexes.cQty)].Value);
                            MyPrateWithtax = (mytaxable + Comm.ToDouble(dgvStockOut.Rows[k].Cells[GetEnum(gridColIndexes.ctax)].Value)) / (Comm.ToDouble(dgvStockOut.Rows[k].Cells[GetEnum(gridColIndexes.cQty)].Value));
                        }

                        //Distributing CommonValues Betweeen Items

                        SetValue(GetEnum(gridColIndexes.cSrate), k, Comm.FormatValue(Comm.ToDouble(dgvStockOut.Rows[k].Cells[GetEnum(gridColIndexes.cSrate)].Value))); //cRate <--> cSrate
                                                                                                                                                                       //DGVItem.Item(cSrate, i).Value = DGVItem.Item(cRate, i).Value
                                                                                                                                                                       //MyPRate = MyPRate; // + perpieceaddcharges;
                                                                                                                                                                       //Added by Dipu on 23-Nov-2021 ---------------->>
                                                                                                                                                                       //MyPrateWithtax = MyPrateWithtax; // + perpieceaddcharges;
                        //SetValue(GetEnum(gridColIndexes.cCrate), k, Comm.FormatValue(MyPRate));
                        //SetValue(GetEnum(gridColIndexes.cCRateWithTax), k, Comm.FormatValue(MyPrateWithtax));
                        if (MyPRate > 0)
                        {
                            //NotifyIcon("Sales Value Calculation", MyPRate)
                            //MessageBox.Show("Sales Value Calculation (" + MyPRate + ")");
                        }

                        //BLNRECALCULATESalesRatesOnPercentag
                        if (clsVchTypeFeatures.BLNRECALCULATESalesRatesOnPercentage == true)
                        {
                            double dblcSRate1Per = Comm.ToDouble(dgvStockOut.Rows[k].Cells[GetEnum(gridColIndexes.cSRate1Per)].Value);
                            double dblcsRate2Per = Comm.ToDouble(dgvStockOut.Rows[k].Cells[GetEnum(gridColIndexes.cSRate2Per)].Value);
                            double dblcsRate3Per = Comm.ToDouble(dgvStockOut.Rows[k].Cells[GetEnum(gridColIndexes.cSRate3Per)].Value);
                            double dblcsRate4Per = Comm.ToDouble(dgvStockOut.Rows[k].Cells[GetEnum(gridColIndexes.cSRate4Per)].Value);
                            double dblcsRate5Per = Comm.ToDouble(dgvStockOut.Rows[k].Cells[GetEnum(gridColIndexes.cSRate5Per)].Value);

                            double dblcRate = Comm.ToDouble(dgvStockOut.Rows[k].Cells[GetEnum(gridColIndexes.cSrate)].Value);
                            double dblcCRate = Comm.ToDouble(dgvStockOut.Rows[k].Cells[GetEnum(gridColIndexes.cCrate)].Value);
                            double dblcMRP = Comm.ToDouble(dgvStockOut.Rows[k].Cells[GetEnum(gridColIndexes.cMRP)].Value);
                            double dblcCRateWithTax = Comm.ToDouble(dgvStockOut.Rows[k].Cells[GetEnum(gridColIndexes.cCRateWithTax)].Value);

                            if (dgvStockOut.Rows[k].Cells[GetEnum(gridColIndexes.cSRate1)].Tag == null)
                                dgvStockOut.Rows[k].Cells[GetEnum(gridColIndexes.cSRate1)].Tag = "";
                            if (dgvStockOut.Rows[k].Cells[GetEnum(gridColIndexes.cSRate2)].Tag == null)
                                dgvStockOut.Rows[k].Cells[GetEnum(gridColIndexes.cSRate2)].Tag = "";
                            if (dgvStockOut.Rows[k].Cells[GetEnum(gridColIndexes.cSRate3)].Tag == null)
                                dgvStockOut.Rows[k].Cells[GetEnum(gridColIndexes.cSRate3)].Tag = "";
                            if (dgvStockOut.Rows[k].Cells[GetEnum(gridColIndexes.cSRate4)].Tag == null)
                                dgvStockOut.Rows[k].Cells[GetEnum(gridColIndexes.cSRate4)].Tag = "";
                            if (dgvStockOut.Rows[k].Cells[GetEnum(gridColIndexes.cSRate5)].Tag == null)
                                dgvStockOut.Rows[k].Cells[GetEnum(gridColIndexes.cSRate5)].Tag = "";

                            switch (Comm.ToDouble(dgvStockOut.Rows[k].Cells[GetEnum(gridColIndexes.cSRate1Per)].Tag)) //DiscMode
                            {
                                case 0:
                                    if (dblcSRate1Per > 0 && dgvStockOut.Rows[k].Cells[GetEnum(gridColIndexes.cSRate1)].Tag.ToString() == "") SetValue(GetEnum(gridColIndexes.cSRate1), k, Comm.FormatValue((dblcRate + dblcRate * dblcSRate1Per / 100)));
                                    if (dblcsRate2Per > 0 && dgvStockOut.Rows[k].Cells[GetEnum(gridColIndexes.cSRate2)].Tag.ToString() == "") SetValue(GetEnum(gridColIndexes.cSRate2), k, Comm.FormatValue((dblcRate + dblcRate * dblcsRate2Per / 100)));
                                    if (dblcsRate3Per > 0 && dgvStockOut.Rows[k].Cells[GetEnum(gridColIndexes.cSRate3)].Tag.ToString() == "") SetValue(GetEnum(gridColIndexes.cSRate3), k, Comm.FormatValue((dblcRate + dblcRate * dblcsRate3Per / 100)));
                                    if (dblcsRate4Per > 0 && dgvStockOut.Rows[k].Cells[GetEnum(gridColIndexes.cSRate4)].Tag.ToString() == "") SetValue(GetEnum(gridColIndexes.cSRate4), k, Comm.FormatValue((dblcRate + dblcRate * dblcsRate4Per / 100)));
                                    if (dblcsRate5Per > 0 && dgvStockOut.Rows[k].Cells[GetEnum(gridColIndexes.cSRate5)].Tag.ToString() == "") SetValue(GetEnum(gridColIndexes.cSRate5), k, Comm.FormatValue((dblcRate + dblcRate * dblcsRate5Per / 100)));
                                    break;
                                case 3:
                                    if (dblcSRate1Per > 0 && dgvStockOut.Rows[k].Cells[GetEnum(gridColIndexes.cSRate1)].Tag.ToString() == "") SetValue(GetEnum(gridColIndexes.cSRate1), k, Comm.FormatValue(dblcCRate + dblcCRate * dblcSRate1Per / 100));
                                    if (dblcsRate2Per > 0 && dgvStockOut.Rows[k].Cells[GetEnum(gridColIndexes.cSRate2)].Tag.ToString() == "") SetValue(GetEnum(gridColIndexes.cSRate2), k, Comm.FormatValue(dblcCRate + dblcCRate * dblcsRate2Per / 100));
                                    if (dblcsRate3Per > 0 && dgvStockOut.Rows[k].Cells[GetEnum(gridColIndexes.cSRate3)].Tag.ToString() == "") SetValue(GetEnum(gridColIndexes.cSRate3), k, Comm.FormatValue(dblcCRate + dblcCRate * dblcsRate3Per / 100));
                                    if (dblcsRate4Per > 0 && dgvStockOut.Rows[k].Cells[GetEnum(gridColIndexes.cSRate4)].Tag.ToString() == "") SetValue(GetEnum(gridColIndexes.cSRate4), k, Comm.FormatValue(dblcCRate + dblcCRate * dblcsRate4Per / 100));
                                    if (dblcsRate5Per > 0 && dgvStockOut.Rows[k].Cells[GetEnum(gridColIndexes.cSRate5)].Tag.ToString() == "") SetValue(GetEnum(gridColIndexes.cSRate5), k, Comm.FormatValue(dblcCRate + dblcCRate * dblcsRate5Per / 100));
                                    break;
                                case 1:
                                    if (dblcSRate1Per > 0 && dgvStockOut.Rows[k].Cells[GetEnum(gridColIndexes.cSRate1)].Tag.ToString() == "") SetValue(GetEnum(gridColIndexes.cSRate1), k, Comm.FormatValue(dblcMRP - dblcMRP * dblcSRate1Per / 100));
                                    if (dblcsRate2Per > 0 && dgvStockOut.Rows[k].Cells[GetEnum(gridColIndexes.cSRate2)].Tag.ToString() == "") SetValue(GetEnum(gridColIndexes.cSRate2), k, Comm.FormatValue(dblcMRP - dblcMRP * dblcsRate2Per / 100));
                                    if (dblcsRate3Per > 0 && dgvStockOut.Rows[k].Cells[GetEnum(gridColIndexes.cSRate3)].Tag.ToString() == "") SetValue(GetEnum(gridColIndexes.cSRate3), k, Comm.FormatValue(dblcMRP - dblcMRP * dblcsRate3Per / 100));
                                    if (dblcsRate4Per > 0 && dgvStockOut.Rows[k].Cells[GetEnum(gridColIndexes.cSRate4)].Tag.ToString() == "") SetValue(GetEnum(gridColIndexes.cSRate4), k, Comm.FormatValue(dblcMRP - dblcMRP * dblcsRate4Per / 100));
                                    if (dblcsRate5Per > 0 && dgvStockOut.Rows[k].Cells[GetEnum(gridColIndexes.cSRate5)].Tag.ToString() == "") SetValue(GetEnum(gridColIndexes.cSRate5), k, Comm.FormatValue(dblcMRP - dblcMRP * dblcsRate5Per / 100));
                                    break;
                                case 2:
                                    if (dblcSRate1Per > 0 && dgvStockOut.Rows[k].Cells[GetEnum(gridColIndexes.cSRate1)].Tag.ToString() == "") SetValue(GetEnum(gridColIndexes.cSRate1), k, Comm.FormatValue(dblcCRateWithTax + dblcCRateWithTax * dblcSRate1Per / 100));
                                    if (dblcsRate2Per > 0 && dgvStockOut.Rows[k].Cells[GetEnum(gridColIndexes.cSRate2)].Tag.ToString() == "") SetValue(GetEnum(gridColIndexes.cSRate2), k, Comm.FormatValue(dblcCRateWithTax + dblcCRateWithTax * dblcsRate2Per / 100));
                                    if (dblcsRate3Per > 0 && dgvStockOut.Rows[k].Cells[GetEnum(gridColIndexes.cSRate3)].Tag.ToString() == "") SetValue(GetEnum(gridColIndexes.cSRate3), k, Comm.FormatValue(dblcCRateWithTax + dblcCRateWithTax * dblcsRate3Per / 100));
                                    if (dblcsRate4Per > 0 && dgvStockOut.Rows[k].Cells[GetEnum(gridColIndexes.cSRate4)].Tag.ToString() == "") SetValue(GetEnum(gridColIndexes.cSRate4), k, Comm.FormatValue(dblcCRateWithTax + dblcCRateWithTax * dblcsRate4Per / 100));
                                    if (dblcsRate5Per > 0 && dgvStockOut.Rows[k].Cells[GetEnum(gridColIndexes.cSRate5)].Tag.ToString() == "") SetValue(GetEnum(gridColIndexes.cSRate5), k, Comm.FormatValue(dblcCRateWithTax + dblcCRateWithTax * dblcsRate5Per / 100));
                                    break;
                            }

                            dgvStockOut.Rows[k].Cells[GetEnum(gridColIndexes.cSRate1)].Tag = "";
                            dgvStockOut.Rows[k].Cells[GetEnum(gridColIndexes.cSRate2)].Tag = "";
                            dgvStockOut.Rows[k].Cells[GetEnum(gridColIndexes.cSRate3)].Tag = "";
                            dgvStockOut.Rows[k].Cells[GetEnum(gridColIndexes.cSRate4)].Tag = "";
                            dgvStockOut.Rows[k].Cells[GetEnum(gridColIndexes.cSRate5)].Tag = "";
                        }

                        //double SavingsofItem = (Comm.ToDouble(dgvSales.Rows[k].Cells[GetEnum(gridColIndexes.cMRP)].Value) * MyQty) - (Val(DGVItem.Item(cRate, i).Value) * MyQty);
                        SavingsofItem = (Comm.ToDouble(dgvStockOut.Rows[k].Cells[GetEnum(gridColIndexes.cMRP)].Value) * MyQty) - (Comm.ToDouble(dgvStockOut.Rows[k].Cells[GetEnum(gridColIndexes.cSrate)].Value) * MyQty); //cRate <--> cSrate
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

            if (Convert.ToInt32(dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cBatchMode)].Value) == 0) // None
                blnAutoCodeNeeded = false;
            else if (Convert.ToInt32(dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cBatchMode)].Value) == 1) // MNF
                blnAutoCodeNeeded = true;
            else if (Convert.ToInt32(dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cBatchMode)].Value) == 2) // Auto
                blnAutoCodeNeeded = true;
            else if (Convert.ToInt32(dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cBatchMode)].Value) == 3) // WMH
                blnAutoCodeNeeded = false;

            //string sQuery = "SELECT AnyWhere,BatchCode,ExpiryDate,MRP,QOH,StockID,ItemID FROM (SELECT (BatchCode+CONVERT(VARCHAR,ExpiryDate)+CONVERT(VARCHAR,ISNULL(QOH,0))+CONVERT(VARCHAR,ISNULL(MRP,0))) as AnyWhere,BatchCode,ExpiryDate,MRP,QOH,StockID,ItemID FROM tblStock )A WHERE A.ItemID = " + Convert.ToInt32(dgvSales.Rows[dgvSales.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cItemID)].Value.ToString()) + "";
            sQuery = "SELECT AnyWhere,BatchCode,ExpiryDate,MRP,QOH,StockID,ItemID FROM (";
            sQuery = sQuery + "SELECT (BatchCode+CONVERT(VARCHAR,ExpiryDate)+CONVERT(VARCHAR,ISNULL(QOH,0))+CONVERT(VARCHAR,ISNULL(MRP,0))) as AnyWhere,BatchUnique as BatchCode,ExpiryDate,MRP,QOH,StockID,ItemID FROM tblStock ";

            if (blnAutoCodeNeeded == true)
            {
                if (Convert.ToInt32(dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cBatchMode)].Value) == 1)// MNF
                {
                    //if (bWhenPressDownKey == true)
                    //sQuery = sQuery + " UNION ALL SELECT '<Auto Barcode>' as AnyWhere,'<Auto Barcode>' as BatchCode,'23-Feb-2030' as ExpiryDate,0 as MRP,0 as QOH,0 as StockID," + Convert.ToInt32(dgvSales.Rows[dgvSales.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cItemID)].Value.ToString()) + " as ItemID";
                }
                else if (Convert.ToInt32(dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cBatchMode)].Value) == 2)// Auto
                {
                    //sQuery = sQuery + " UNION ALL SELECT '<Auto Barcode>' as AnyWhere,'<Auto Barcode>' as BatchCode,'23-Feb-2030' as ExpiryDate,0 as MRP,0 as QOH,0 as StockID," + Convert.ToInt32(dgvSales.Rows[dgvSales.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cItemID)].Value.ToString()) + " as ItemID";
                }
            }

            if (Convert.ToInt32(dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cBatchMode)].Value) == 1 || Convert.ToInt32(dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cBatchMode)].Value) == 2)// MNF & AUto
            {
                sQuery = sQuery + " )A WHERE A.ItemID = " + Convert.ToDecimal(dgvStockOut.Rows[dgvStockOut.CurrentRow.Index].Cells[GetEnum(gridColIndexes.cItemID)].Value.ToString()) + "";
                frmBatchSearch = new frmCompactSearch(GetFromBatchCodeSearch, sQuery, "Anywhere|BatchCode|CONVERT(VARCHAR,ExpiryDate)|CONVERT(VARCHAR,ISNULL(MRP,0))|CONVERT(VARCHAR,ISNULL(QOH,0))", dgvStockOut.Location.X + 350, dgvStockOut.Location.Y + 150, 4, 0, "", 4, 0, "ORDER BY A.StockID ASC", 0, 0, "BatchCode Search...", 0, "150,90,80,80,0,0", true, "Bar Code", 10);
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
                    MessageBox.Show("Voucher settings incorrect for the voucher. Please correct the settings and open the voucher again.", "Sales Settings", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            catch (Exception ex)
            {

            }

            try
            {
                if (Comm.ConvertI32(clsVchType.TransactionNumberingValue) == 0) // Auto Locked
                {
                    if (iIDFromEditWindow == 0) //New
                    {
                        txtInvAutoNo.Text = Comm.gfnGetNextSerialNo("tblSales", "AutoNum", " VchTypeID=" + vchtypeID).ToString();
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
                        //txtInvAutoNo.Text = Comm.gfnGetNextSerialNo("tblSales", "AutoNum").ToString();
                        txtInvAutoNo.Text = Comm.gfnGetNextSerialNo("tblSales", "AutoNum", " VchTypeID=" + vchtypeID).ToString();
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
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            try
            {
                if (clsVchTypeFeatures.blnenablecashdiscount == null) clsVchTypeFeatures.blnenablecashdiscount = false;
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
                if (clsVchTypeFeatures.blnshowotherexpense)
                    tblpOtherExp.Visible = true;
                else
                    tblpOtherExp.Visible = false;
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
                if (Comm.ConvertI32(clsVchType.ReferenceNumberingValue) == 0) // Auto Locked
                {
                    if (iIDFromEditWindow == 0)
                        txtReferenceAutoNo.Text = Comm.gfnGetNextSerialNo("tblSales", "ReferenceAutoNO").ToString();
                    txtReferenceAutoNo.ReadOnly = true;
                    txtReferencePrefix.ReadOnly = true;
                    txtReferencePrefix.Width = 55;
                }
                else if (Comm.ConvertI32(clsVchType.ReferenceNumberingValue) == 1) // Auto Editable
                {
                    if (iIDFromEditWindow == 0)
                        txtReferenceAutoNo.Text = Comm.gfnGetNextSerialNo("tblSales", "ReferenceAutoNO").ToString();
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
                if (clsVchType.blnTaxModeLockWSel == 1)
                    cboTaxMode.Enabled = false;
                else
                    cboTaxMode.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            try
            {
                if (clsVchType.blnModeofPaymentLockWSel == 1)
                    cboPayment.Enabled = false;
                else
                    cboPayment.Enabled = true;
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
                if (clsVchType.blnAgentLockWSel == 1)
                    cboAgent.Enabled = false;
                else
                    cboAgent.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            try
            {
                if (clsVchType.DefaultTaxModeValue == 3) //GST
                {
                    if (dgvStockOut.Columns.Count > 0)
                    {
                        dgvStockOut.Columns["cCGST"].Visible = true;
                        dgvStockOut.Columns["cSGST"].Visible = true;
                        dgvStockOut.Columns["cIGST"].Visible = true;
                        dgvStockOut.Columns["ctaxPer"].Visible = true;
                        dgvStockOut.Columns["ctax"].Visible = true;
                        dgvStockOut.Columns["ctaxable"].Visible = true;
                        dgvStockOut.Columns["cCRateWithTax"].Visible = true;
                        tblpTaxAmt.Visible = true;
                        tblpTaxable.Visible = true;
                    }
                }
                else
                {
                    if (dgvStockOut.Columns.Count > 0)
                    {
                        dgvStockOut.Columns["cCGST"].Visible = false;
                        dgvStockOut.Columns["cSGST"].Visible = false;
                        dgvStockOut.Columns["cIGST"].Visible = false;
                        dgvStockOut.Columns["ctaxPer"].Visible = false;
                        dgvStockOut.Columns["ctax"].Visible = false;
                        dgvStockOut.Columns["ctaxable"].Visible = false;
                        dgvStockOut.Columns["cCRateWithTax"].Visible = false;
                        tblpTaxAmt.Visible = false;
                        tblpTaxable.Visible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            try
            {
                if (iIDFromEditWindow == 0) //New
                    GetAgentDiscountAsperVoucherType();
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
                    MessageBox.Show("Voucher settings incorrect for the voucher. Please correct the settings and open the voucher again.", "Sales Settings", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            catch (Exception ex)
            {

            }

            try
            {
                if (Comm.ConvertI32(clsVchType.DefaultModeofPaymentValue) == 1) // Cash
                    cboPayment.SelectedIndex = 0;
                else if (Comm.ConvertI32(clsVchType.DefaultModeofPaymentValue) == 2) // Credit
                    cboPayment.SelectedIndex = 1;
                else if (Comm.ConvertI32(clsVchType.DefaultModeofPaymentValue) == 3) // Mixed
                    cboPayment.SelectedIndex = 2;
                else if (Comm.ConvertI32(clsVchType.DefaultModeofPaymentValue) == 4) // Cash Counter
                    cboPayment.SelectedIndex = 3;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            try
            {
                cboTaxMode.SelectedValue = Comm.ConvertI32(clsVchType.DefaultTaxModeValue);
                cboCostCentre.SelectedValue = Comm.ConvertI32(clsVchType.PrimaryCCValue);
                cboSalesStaff.SelectedValue = Comm.ConvertI32(clsVchType.DefaultSaleStaffValue);
                cboAgent.SelectedValue = Comm.ConvertI32(clsVchType.DefaultAgentValue);
                GetAgentDiscountAsperVoucherType();
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
                if (dgvStockOut.Columns.Count > 0)
                {
                    if (AppSettings.TaxEnabled == true)
                    {
                        if (AppSettings.TaxMode == 0) //No Tax
                        {
                            cboTaxMode.SelectedValue = 1; //none
                            tblpTaxAmt.Visible = false;
                            tblpTaxable.Visible = false;

                            dgvStockOut.Columns["cCGST"].Visible = false;
                            dgvStockOut.Columns["cSGST"].Visible = false;
                            dgvStockOut.Columns["cIGST"].Visible = false;
                            dgvStockOut.Columns["ctaxPer"].Visible = false;
                            dgvStockOut.Columns["ctax"].Visible = false;
                            dgvStockOut.Columns["ctaxable"].Visible = false;
                            dgvStockOut.Columns["cCRateWithTax"].Visible = false;
                        }
                        else if (AppSettings.TaxMode == 1) //VAT
                        {
                            cboTaxMode.SelectedValue = 2; //VAT
                            tblpTaxAmt.Visible = true;
                            tblpTaxable.Visible = true;
                            pnlTaxMode.Visible = true;

                            dgvStockOut.Columns["cCGST"].Visible = false;
                            dgvStockOut.Columns["cSGST"].Visible = false;

                            dgvStockOut.Columns["cIGST"].Visible = true;
                            dgvStockOut.Columns["ctaxPer"].Visible = true;
                            dgvStockOut.Columns["ctax"].Visible = true;
                            dgvStockOut.Columns["ctaxable"].Visible = true;
                            dgvStockOut.Columns["cCRateWithTax"].Visible = true;
                        }
                        else
                        {
                            dgvStockOut.Columns["cCGST"].Visible = true;
                            dgvStockOut.Columns["cSGST"].Visible = true;
                            dgvStockOut.Columns["cIGST"].Visible = true;
                            dgvStockOut.Columns["ctaxPer"].Visible = true;
                            dgvStockOut.Columns["ctax"].Visible = true;
                            dgvStockOut.Columns["ctaxable"].Visible = true;
                            dgvStockOut.Columns["cCRateWithTax"].Visible = true;

                            pnlTaxMode.Visible = true;
                            //cboTaxMode.SelectedValue = AppSettings.TaxMode + 1;

                            tblpTaxAmt.Visible = true;
                            tblpTaxable.Visible = true;
                        }
                    }
                    else
                    {
                        dgvStockOut.Columns[GetEnum(gridColIndexes.cCGST)].Visible = false;
                        dgvStockOut.Columns[GetEnum(gridColIndexes.cSGST)].Visible = false;
                        dgvStockOut.Columns[GetEnum(gridColIndexes.cIGST)].Visible = false;
                        dgvStockOut.Columns[GetEnum(gridColIndexes.ctaxPer)].Visible = false;
                        dgvStockOut.Columns[GetEnum(gridColIndexes.ctax)].Visible = false;
                        dgvStockOut.Columns[GetEnum(gridColIndexes.ctaxable)].Visible = false;
                        dgvStockOut.Columns[GetEnum(gridColIndexes.cCRateWithTax)].Visible = false;

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

                if (dgvStockOut.Columns.Count > 0)
                {
                    if (AppSettings.CessMode == 0)
                    {
                        tblpCess.Visible = false;
                        tblpCompCess.Visible = false;
                        tblpQtyCess.Visible = false;

                        dgvStockOut.Columns[GetEnum(gridColIndexes.cCCessPer)].Visible = false;
                        dgvStockOut.Columns[GetEnum(gridColIndexes.cCCompCessQty)].Visible = false;
                        dgvStockOut.Columns[GetEnum(gridColIndexes.cFloodCessAmt)].Visible = false;
                        dgvStockOut.Columns[GetEnum(gridColIndexes.cFloodCessPer)].Visible = false;
                    }
                    else
                    {
                        tblpCess.Visible = true;
                        tblpCompCess.Visible = true;
                        tblpQtyCess.Visible = true;

                        dgvStockOut.Columns[GetEnum(gridColIndexes.cCCessPer)].Visible = true;
                        dgvStockOut.Columns[GetEnum(gridColIndexes.cCCompCessQty)].Visible = true;
                        dgvStockOut.Columns[GetEnum(gridColIndexes.cFloodCessAmt)].Visible = true;
                        dgvStockOut.Columns[GetEnum(gridColIndexes.cFloodCessPer)].Visible = true;
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
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        ////Description : Convert to Int32 of Decimal Value
        //private int ConvertI32(decimal dVal)
        //{
        //    return Convert.ToInt32(dVal);
        //}

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

                GetRepackingIfo.InvId = Convert.ToDecimal(iSelectedID);
                GetRepackingIfo.TenantID = Convert.ToDecimal(Global.gblTenantID);
                GetRepackingIfo.VchTypeID = vchtypeID;
                dtLoad = clsPur.GetSalesMaster(GetRepackingIfo, false);
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

                    iAction = 1;

                    dgvStockOut.Columns["cRateinclusive"].Visible = false;
                    dgvStockOut.Columns["cSRate1Per"].Visible = false;
                    dgvStockOut.Columns["cSRate2Per"].Visible = false;
                    dgvStockOut.Columns["cSRate3Per"].Visible = false;
                    dgvStockOut.Columns["cSRate4Per"].Visible = false;
                    dgvStockOut.Columns["cSRate5Per"].Visible = false;
                    dgvStockOut.Columns["cSRate1"].Visible = false;
                    dgvStockOut.Columns["cSRate2"].Visible = false;
                    dgvStockOut.Columns["cSRate3"].Visible = false;
                    dgvStockOut.Columns["cSRate4"].Visible = false;
                    dgvStockOut.Columns["cSRate5"].Visible = false;
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
    public class RepackingGridColIndexes
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
        public int cFree = 10;
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

                        break;
                    }
                case 1:
                case 2:
                case 3:
                case 4:
                    {
                        return GetMasterColName(colIndex);

                        break;
                    }
                case 5:
                    {
                        return nameof(CExpiry);

                        break;
                    }
                case 6:
                    {
                        return nameof(cMRP);

                        break;
                    }
                case 7:
                    {
                        return nameof(cSrate);

                        break;
                    }
                case 8:
                    {
                        return nameof(cRateinclusive);

                        break;
                    }
                case 9:
                    {
                        return nameof(cQty);

                        break;
                    }
                case 10:
                    {
                        return nameof(cFree);

                        break;
                    }
                case 11:
                    {
                        return nameof(cSRate1Per);

                        break;
                    }
                case 12:
                    {
                        return nameof(cSRate1);

                        break;
                    }
                case 13:
                    {
                        return nameof(cSRate2Per);

                        break;
                    }
                case 14:
                    {
                        return nameof(cSRate2);

                        break;
                    }
                case 15:
                    {
                        return nameof(cSRate3Per);

                        break;
                    }
                case 16:
                    {
                        return nameof(cSRate3);

                        break;
                    }
                case 17:
                    {
                        return nameof(cSRate4Per);

                        break;
                    }
                case 18:
                    {
                        return nameof(cSRate4);

                        break;
                    }
                case 19:
                    {
                        return nameof(cSRate5Per);

                        break;
                    }
                case 20:
                    {
                        return nameof(cSRate5);

                        break;
                    }
                case 21:
                    {
                        return nameof(cGrossAmt);

                        break;
                    }
                case 22:
                    {
                        return nameof(cDiscPer);

                        break;
                    }
                case 23:
                    {
                        return nameof(cDiscAmount);

                        break;
                    }
                case 24:
                    {
                        return nameof(cBillDisc);

                        break;
                    }
                case 25:
                    {
                        return nameof(cCrate);

                        break;
                    }
                case 26:
                    {
                        return nameof(cCRateWithTax);

                        break;
                    }
                case 27:
                    {
                        return nameof(ctaxable);

                        break;
                    }
                case 28:
                    {
                        return nameof(ctaxPer);

                        break;
                    }
                case 29:
                    {
                        return nameof(ctax);

                        break;
                    }
                case 30:
                    {
                        return nameof(cIGST);

                        break;
                    }
                case 31:
                    {
                        return nameof(cSGST);

                        break;
                    }
                case 32:
                    {
                        return nameof(cCGST);

                        break;
                    }
                case 33:
                    {
                        return nameof(cNetAmount);

                        break;
                    }
                case 34:
                    {
                        return nameof(cItemID);

                        break;
                    }
                case 35:
                    {
                        return nameof(cGrossValueAfterRateDiscount);

                        break;
                    }
                case 36:
                    {
                        return nameof(cNonTaxable);

                        break;
                    }
                case 37:
                    {
                        return nameof(cCCessPer);

                        break;
                    }
                case 38:
                    {
                        return nameof(cCCompCessQty);

                        break;
                    }
                case 39:
                    {
                        return nameof(cFloodCessPer);

                        break;
                    }
                case 40:
                    {
                        return nameof(cFloodCessAmt);

                        break;
                    }
                case 41:
                    {
                        return nameof(cStockMRP);

                        break;
                    }
                case 42:
                    {
                        return nameof(cAgentCommPer);

                        break;
                    }
                case 43:
                    {
                        return nameof(cCoolie);

                        break;
                    }
                case 44:
                    {
                        return nameof(cBlnOfferItem);

                        break;
                    }
                case 45:
                    {
                        return nameof(cStrOfferDetails);

                        break;
                    }
                case 46:
                    {
                        return nameof(cBatchMode);

                        break;
                    }
                case 47:
                    {
                        return nameof(cID);

                        break;
                    }
                case 48:
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
                        if (colIndex == 1) return nameof(CItemCode);
                        if (colIndex == 2) return nameof(CItemName);
                        if (colIndex == 3) return nameof(CUnit);
                        if (colIndex == 4) return nameof(cBarCode);

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
            if (AppSettings.BLNBARCODE == true)
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
            else
            {
                CItemCode = 1;
                CItemName = 2;
                CUnit = 3;
                cBarCode = 4;

                MyMode = BarcodeMode.BarcodeDropdown;
            }
        }
    }
    #endregion 
}
