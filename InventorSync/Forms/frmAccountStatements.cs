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

    public partial class frmAccountStatements : Form, IMessageFilter
    {
        //=============================================================================
        // Created By       : Arun 
        // Created On       : 29-Nov-2022 
        // Last Edited On   : 
        // Last Edited By   : Arun 
        // Description      : Working With Account Statements 
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

        string mReportType = "";
        public frmAccountStatements(string ReportType, object MDIParent = null)
        {
            InitializeComponent();
            Application.AddMessageFilter(this);

            mReportType = ReportType;

            try
            {
                Comm.TransparentControls(this);
                Comm.SetControlColours(this);

                this.BackColor = Color.FromArgb(249, 246, 238);

                lblHeading.Font = new Font("Tahoma", 21, FontStyle.Regular, GraphicsUnit.Pixel);
                lblSave.Font = new Font("Tahoma", 10, FontStyle.Regular, GraphicsUnit.Point);
                lblPrint.Font = new Font("Tahoma", 10, FontStyle.Regular, GraphicsUnit.Point);

                lblSave.ForeColor = Color.Black;
                lblPrint.ForeColor = Color.Black;

                lblHeading.ForeColor = Color.Black;

                lblInvDate.ForeColor = Color.Black;

                btnSave.Image = global::InventorSync.Properties.Resources.save240402;
                btnPrint.Image = global::InventorSync.Properties.Resources.printer_finalised;
                btnMinimize.Image = global::InventorSync.Properties.Resources.minimize_finalised;
                btnClose.Image = global::InventorSync.Properties.Resources.logout_Final;
            }
            catch
            { }

            controlsToMove.Add(this);
            controlsToMove.Add(this.lblHeading);//Add whatever controls here you want to move the form when it is clicked and dragged

            frmMDI form = (frmMDI)MDIParent;
            this.MdiParent = form;
            int l = form.ClientSize.Width - 10; 
            int t = form.ClientSize.Height - 80; 
            this.SetBounds(5, 0, l, t);

            panel1.Visible = false;

            AddColumnsToGrid(dgvDetails);
            ClearControls();
        }

        #region "VARIABLES --------------------------------------------- >>"

        Control ctrl;
        string sEditedValueonKeyPress;
        bool dragging = false;
        int xOffset = 0, yOffset = 0, d = 0;

        Common Comm = new Common();

        #endregion

        #region "EVENTS ------------------------------------------------ >>"

        private void frmStockVoucher_Load(object sender, EventArgs e)
        {
            try
            {
                AddColumnsToGrid(dgvDetails);

                Application.DoEvents();

                this.tlpMain.ColumnStyles[1].SizeType = SizeType.Absolute;
                this.tlpMain.ColumnStyles[1].Width = 0;

                FillCostCentre(true);
                FillReport();
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void FillCostCentre(bool blnSelectAll = false)
        {
            try
            {
                Comm.LoadControl(lstCostCentre, new DataTable(), "Select CCID, CCName From tblCostCentre Order By CCName", true, blnSelectAll, "CCName", "CCID");
            }
            catch
            { }
        }

        private void FillReport()
        {
            try
            {
                switch(mReportType)
                {
                    case "DAYBOOK":
                        dayBook();

                        break;
                    case null:

                        break;
                }
            }
            catch(Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
        }

        public double GetLedgerBalance(long LedgerID, DateTime AsOnDate, DateTime StartDate = default(DateTime), string VchtypeID = "")
        {
            try
            {
                double returnvalue = 0;
                sqlControl rs = new sqlControl();
                string StrSqlDate = "";
                string strVchtypeID = "";
                if (Strings.Len(VchtypeID) > 0)
                {
                    if (Conversion.Val(VchtypeID) > 0)
                        strVchtypeID = " And vchTypeID In(" + VchtypeID + ") ";
                }

                if (Strings.InStr(1, StartDate.ToString(), "00") > 1)
                    StrSqlDate = " And vchDate <='" + Strings.Format((DateTime)AsOnDate, "dd/MMM/yyyy") + "'";
                else
                    StrSqlDate = " and vchDate between '" + Strings.Format((DateTime)AsOnDate, "dd/MMM/yyyy") + "' and '" + Strings.Format(StartDate, "dd/MMM/yyyy") + "'";
                // =================

                string strCCIDsql = Comm.GetCheckedData(lstCostCentre);

                string StrCCSQL1 = "";
                if (strCCIDsql != "")
                    StrCCSQL1 = strCCIDsql.Replace("and", "") + " AND ";

                rs.Open("Select Sum(AmountD)-Sum(AmountC) as Balance from tblVoucher where " + StrCCSQL1 + " Optional=0  " + strVchtypeID + " and LedgerID=" + LedgerID + StrSqlDate);

                if (!rs.eof())
                {
                    if (rs.fields("Balance") != null)
                        returnvalue = Comm.ToDouble(rs.fields("Balance"));
                }
                return returnvalue;
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return 0;
            }
        }
        
        private void dayBook()
        {
            dgvDetails.Columns.Clear();
            tgsDetailed.Visible = true;
            
            lblHeading.Text = "Day Book between " + dtpfrom.Value.ToString("dd/MMM/yyyy") + " and " + dtpfrom.Value.ToString("dd/MMM/yyyy");

            string strCCIDsql = Comm.GetCheckedData(lstCostCentre);
            string StrCCSQL1 = "";
            if (strCCIDsql != "")
                StrCCSQL1 = strCCIDsql.Replace("and", "") + " AND ";

            DataGridViewTextBoxColumn myDate = new DataGridViewTextBoxColumn();
            myDate.HeaderText = "Date";
            myDate.Name = "Date";
            myDate.ReadOnly = true;
            myDate.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            myDate.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
            myDate.SortMode = DataGridViewColumnSortMode.NotSortable;
            myDate.DefaultCellStyle.BackColor = Color.LightSkyBlue;
            dgvDetails.Columns.Add(myDate);

            DataGridViewTextBoxColumn Particulars = new DataGridViewTextBoxColumn();
            Particulars.HeaderText = "Particulars";
            Particulars.Name = "Particulars";
            Particulars.ReadOnly = true;
            Particulars.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            Particulars.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
            Particulars.SortMode = DataGridViewColumnSortMode.NotSortable;
            Particulars.DefaultCellStyle.BackColor = Color.LightSkyBlue;
            dgvDetails.Columns.Add(Particulars);

            DataGridViewTextBoxColumn vchType = new DataGridViewTextBoxColumn();
            vchType.HeaderText = "Vchtype";
            vchType.Name = "Vchtype";
            vchType.ReadOnly = true;
            vchType.SortMode = DataGridViewColumnSortMode.NotSortable;
            vchType.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            vchType.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvDetails.Columns.Add(vchType);

            DataGridViewTextBoxColumn VchNo = new DataGridViewTextBoxColumn();
            VchNo.HeaderText = "VchNo";
            VchNo.Name = "VchNo";
            VchNo.ReadOnly = true;
            VchNo.SortMode = DataGridViewColumnSortMode.NotSortable;
            VchNo.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            VchNo.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvDetails.Columns.Add(VchNo);

            DataGridViewTextBoxColumn narration = new DataGridViewTextBoxColumn();
            narration.HeaderText = "Narration";
            narration.Name = "Narration";
            narration.ReadOnly = true;
            narration.SortMode = DataGridViewColumnSortMode.NotSortable;
            narration.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            narration.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvDetails.Columns.Add(narration);

            DataGridViewTextBoxColumn DebitSub = new DataGridViewTextBoxColumn();
            DebitSub.HeaderText = "Debit";
            DebitSub.Name = "DebitSub";
            DebitSub.ReadOnly = true;
            DebitSub.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            DebitSub.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
            DebitSub.SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvDetails.Columns.Add(DebitSub);

            DataGridViewTextBoxColumn CreditSub = new DataGridViewTextBoxColumn();
            CreditSub.HeaderText = "Credit";
            CreditSub.Name = "CreditSub";
            CreditSub.ReadOnly = true;
            CreditSub.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            CreditSub.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
            CreditSub.SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvDetails.Columns.Add(CreditSub);

            DataGridViewTextBoxColumn Balance = new DataGridViewTextBoxColumn();
            Balance.HeaderText = "Balance";
            Balance.Name = "Balance";
            Balance.ReadOnly = true;
            Balance.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            Balance.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
            Balance.SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvDetails.Columns.Add(Balance);

            DataGridViewTextBoxColumn DrillDownID = new DataGridViewTextBoxColumn();
            DrillDownID.HeaderText = "DrillDownID";
            DrillDownID.Name = "DrillDownID";
            DrillDownID.ReadOnly = true;
            DrillDownID.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            DrillDownID.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
            DrillDownID.Visible = false;
            dgvDetails.Columns.Add(DrillDownID);

            DataGridViewTextBoxColumn DrillDownType = new DataGridViewTextBoxColumn();
            DrillDownType.HeaderText = "DrillDownType";
            DrillDownType.Name = "DrillDownType";
            DrillDownType.ReadOnly = true;
            DrillDownType.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            DrillDownType.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
            DrillDownType.Visible = false;
            dgvDetails.Columns.Add(DrillDownType);

            DataGridViewTextBoxColumn nature = new DataGridViewTextBoxColumn();
            nature.HeaderText = "Nature";
            nature.Name = "Nature";
            nature.ReadOnly = true;
            nature.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            DrillDownType.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
            nature.Visible = false;
            dgvDetails.Columns.Add(nature);

            double AmountD = 0;
            double AmountC = 0;
            double CashBalance = 0;

            if (dtpfrom.Value.Date == Global.FyStartDate.Date)
                CashBalance = CashBalance + GetLedgerBalance(3, dtpfrom.Value.Date, dtpfrom.Value.Date, "1005");
            else if (dtpfrom.Value.Date < Global.FyStartDate.Date)
                CashBalance = GetLedgerBalance(3, dtpfrom.Value.Date.AddDays(-1), dtpfrom.Value.Date.AddDays(-1), "");
            else if (dtpfrom.Value.Date > Global.FyStartDate)
                CashBalance = GetLedgerBalance(3, dtpfrom.Value.Date.AddDays(-1), dtpfrom.Value.Date.AddDays(-1), "");
            sqlControl Rs = new sqlControl();

            double SubTotalDebit = 0;
            double SubTotalCredit = 0;
            double SubBalance = 0;

            if (tgsDetailed.ToggleState == Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active)
                Rs.Open("SELECT        TOP (100) PERCENT dbo.tblledger.laliasname as AccountGroup, dbo.tblVoucher.VchDate, dbo.tblVoucher.vchTime, dbo.tblVoucher.VchNo, SUM(dbo.tblVoucher.AmountD) AS AmountD, SUM(dbo.tblVoucher.AmountC) AS AmountC, dbo.tblVoucher.RefID, dbo.tblVoucher.VchTypeID, dbo.tblVchType.VchType, dbo.tblVoucher.mynarration,dbo.tblledger.AccountGroupID as CheckID,dbo.tblledger.lid as AccountGroupID  FROM            dbo.tblledger inner join dbo.tblVoucher ON dbo.tblLedger.LID = dbo.tblVoucher.LedgerID INNER JOIN dbo.tblVchType ON dbo.tblVoucher.VchTypeID = dbo.tblVchType.VchTypeID WHERE tblledger.lid <> 0 and tblvoucher.optional=0 and   (dbo.tblVoucher.LedgerID = 3 AND VCHDATE BETWEEN  '" + dtpfrom.Value.ToString("dd/MMM/yyyy") + "' AND '" + dtpto.Value.ToString("dd/MMM/yyyy") + "')  " + strCCIDsql + " and tblVoucher.vchtypeid not in(1005)  GROUP BY dbo.tblledger.lid, dbo.tblledger.laliasname, dbo.tblVoucher.VchDate, dbo.tblVoucher.VchNo, dbo.tblVoucher.RefID, dbo.tblVoucher.vchTime, dbo.tblVoucher.VchTypeID, dbo.tblVchType.VchType, dbo.tblVoucher.mynarration ,dbo.tblledger.AccountGroupID  having Sum(dbo.tblvoucher.amountd) - Sum(dbo.tblvoucher.amountc) <> 0 " + " Union " + " SELECT        TOP (100) PERCENT dbo.tblledger.laliasname as AccountGroup, dbo.tblVoucher.VchDate, dbo.tblVoucher.vchTime, dbo.tblVoucher.VchNo, SUM(dbo.tblVoucher.AmountD) AS AmountD, SUM(dbo.tblVoucher.AmountC) AS AmountC,  dbo.tblVoucher.RefID, dbo.tblVoucher.VchTypeID, dbo.tblVchType.VchType, dbo.tblVoucher.mynarration,dbo.tblledger.AccountGroupID as CheckID,dbo.tblledger.lid as AccountGroupID  FROM            dbo.tblledger INNER JOIN dbo.tblVoucher ON dbo.tblLedger.LID = dbo.tblVoucher.LedgerID INNER JOIN dbo.tblVchType ON dbo.tblVoucher.VchTypeID = dbo.tblVchType.VchTypeID WHERE    tblledger.lid <> 0 and tblvoucher.optional=0 and    (dbo.tblVoucher.LedgerID <> 3) " + StrCCSQL1 + " AND VCHDATE BETWEEN  '" + dtpfrom.Value.ToString("dd/MMM/yyyy") + "' AND '" + dtpto.Value.ToString("dd/MMM/yyyy") + "' and tblVoucher.vchtypeid not in(1005) GROUP BY dbo.tblledger.lid, dbo.tblledger.laliasname, dbo.tblVoucher.VchDate, dbo.tblVoucher.VchNo, dbo.tblVoucher.RefID, dbo.tblVoucher.vchTime, dbo.tblVoucher.VchTypeID, dbo.tblVchType.VchType, dbo.tblVoucher.mynarration ,dbo.tblledger.AccountGroupID  having Sum(dbo.tblvoucher.amountd) - Sum(dbo.tblvoucher.amountc) <> 0 " + " ORDER BY dbo.tblVoucher.VchDate, dbo.tblVoucher.vchTime, dbo.tblVoucher.vchtypeid, dbo.tblVoucher.VchNo, dbo.tblVoucher.mynarration, AmountD ");
            else
                Rs.Open("SELECT        TOP (100) PERCENT dbo.tblAccountGroup.AccountGroup, dbo.tblVoucher.VchDate, dbo.tblVoucher.vchTime, dbo.tblVoucher.VchNo, SUM(dbo.tblVoucher.AmountD) AS AmountD, SUM(dbo.tblVoucher.AmountC) AS AmountC, dbo.tblVoucher.RefID, dbo.tblVoucher.VchTypeID, dbo.tblVchType.VchType, dbo.tblVoucher.mynarration,dbo.tblAccountGroup.AccountGroupID as CheckID,dbo.tblAccountGroup.AccountGroupID  FROM            dbo.tblAccountGroup INNER JOIN dbo.tblLedger ON dbo.tblAccountGroup.AccountGroupID = dbo.tblLedger.AccountGroupID and tblLedger.LID <> 0 INNER JOIN  dbo.tblVoucher ON dbo.tblLedger.LID = dbo.tblVoucher.LedgerID INNER JOIN dbo.tblVchType ON dbo.tblVoucher.VchTypeID = dbo.tblVchType.VchTypeID WHERE tblvoucher.optional=0 and   (dbo.tblVoucher.LedgerID = 3 AND VCHDATE BETWEEN  '" + dtpfrom.Value.ToString("dd/MMM/yyyy") + "' AND '" + dtpto.Value.ToString("dd/MMM/yyyy") + "')  " + StrCCSQL1 + " and tblVoucher.vchtypeid not in(1005)  GROUP BY dbo.tblAccountGroup.AccountGroup, dbo.tblAccountGroup.AccountGroupID, dbo.tblVoucher.VchDate, dbo.tblVoucher.VchNo, dbo.tblVoucher.RefID, dbo.tblVoucher.vchTime, dbo.tblVoucher.VchTypeID, dbo.tblVchType.VchType, dbo.tblVoucher.mynarration  having Sum(dbo.tblvoucher.amountd) - Sum(dbo.tblvoucher.amountc) <> 0 " + " Union " + " SELECT        TOP (100) PERCENT dbo.tblAccountGroup.AccountGroup, dbo.tblVoucher.VchDate, dbo.tblVoucher.vchTime, dbo.tblVoucher.VchNo, SUM(dbo.tblVoucher.AmountD) AS AmountD, SUM(dbo.tblVoucher.AmountC) AS AmountC,  dbo.tblVoucher.RefID, dbo.tblVoucher.VchTypeID, dbo.tblVchType.VchType, dbo.tblVoucher.mynarration,dbo.tblAccountGroup.AccountGroupID as CheckID,dbo.tblAccountGroup.AccountGroupID  FROM            dbo.tblAccountGroup INNER JOIN dbo.tblLedger ON dbo.tblAccountGroup.AccountGroupID = dbo.tblLedger.AccountGroupID and tblLedger.LID <> 0 INNER JOIN dbo.tblVoucher ON dbo.tblLedger.LID = dbo.tblVoucher.LedgerID INNER JOIN dbo.tblVchType ON dbo.tblVoucher.VchTypeID = dbo.tblVchType.VchTypeID WHERE    tblvoucher.optional=0 and    (dbo.tblVoucher.LedgerID <> 3) " + StrCCSQL1 + " AND VCHDATE BETWEEN  '" + dtpfrom.Value.ToString("dd/MMM/yyyy") + "' AND '" + dtpto.Value.ToString("dd/MMM/yyyy") + "' and tblVoucher.vchtypeid not in(1005) GROUP BY dbo.tblAccountGroup.AccountGroup, dbo.tblAccountGroup.AccountGroupID, dbo.tblVoucher.VchDate, dbo.tblVoucher.vchTime, dbo.tblVoucher.VchTypeID, dbo.tblVchType.VchType, dbo.tblVoucher.VchNo, dbo.tblVoucher.RefID, dbo.tblVoucher.mynarration having Sum(dbo.tblvoucher.amountd) - Sum(dbo.tblvoucher.amountc) <> 0 " + " ORDER BY dbo.tblVoucher.VchDate, dbo.tblVoucher.vchTime, dbo.tblVoucher.VchNo,dbo.tblVoucher.mynarration, AmountD ");

            Font MySubTotalFont = new Font("Segoe UI", 10, FontStyle.Bold);
            double DebitVal = 0;
            double CreditVal = 0;
            {
                var withBlock = dgvDetails;
                dgvDetails.Rows.Add("");
                dgvDetails["Date", dgvDetails.Rows.Count - 1].Value = dtpfrom.Value.ToString("dd/MMM/yyyy");
                dgvDetails["Particulars", dgvDetails.Rows.Count - 1].Value = "Opening Balance Of Cash";

                if (CashBalance > 0)
                {
                    dgvDetails["DebitSub", dgvDetails.Rows.Count - 1].Value = Comm.FormatValue(Comm.ToDouble(Math.Abs(CashBalance)));
                    DebitVal = DebitVal + Comm.ToDouble(dgvDetails["DebitSub", dgvDetails.Rows.Count - 1].Value);
                }
                else
                {
                    dgvDetails["CreditSub", dgvDetails.Rows.Count - 1].Value = Comm.FormatValue(Comm.ToDouble(Math.Abs(CashBalance)), true);
                    CreditVal = CreditVal + Comm.ToDouble(dgvDetails["CreditSub", dgvDetails.Rows.Count - 1].Value);
                }

                dgvDetails["Balance", dgvDetails.Rows.Count - 1].Value = Comm.FormatValue(DebitVal - CreditVal, true) + Interaction.IIf(Comm.ToDouble(DebitVal - CreditVal) > 0, " Dr", " Cr");
                AmountD = AmountD + Comm.ToDouble(dgvDetails["DebitSub", dgvDetails.Rows.Count - 1].Value);
                AmountC = AmountC + Comm.ToDouble(dgvDetails["CreditSub", dgvDetails.Rows.Count - 1].Value);

                SubTotalDebit = SubTotalDebit + Comm.ToDouble(dgvDetails["DebitSub", dgvDetails.Rows.Count - 1].Value);
                SubTotalCredit = SubTotalCredit + Comm.ToDouble(dgvDetails["CreditSub", dgvDetails.Rows.Count - 1].Value);
                SubBalance = SubTotalDebit - SubTotalCredit;

                DateTime PrevDate = dtpfrom.Value; //.ToString("dd/MMM/YYYY");
                SubTotalCredit = 0;
                SubTotalDebit = 0;

                int PrevVchtypeID = 0;
                int PrevRefID = 0;

                double VchAmountC = 0;
                double VchAmountD = 0;

                string AccountGroup = "";

                while (!Rs.eof())
                {
                    if (Comm.ToDouble(Rs.fields("checkid")) != 17)
                    {
                        if (PrevDate != Convert.ToDateTime(Rs.fields("VchDate")))
                        {
                            PrevDate = Convert.ToDateTime(Rs.fields("VchDate"));

                            dgvDetails.Rows.Add("");
                            dgvDetails["DebitSub", dgvDetails.Rows.Count - 1].Value = SubTotalDebit;
                            dgvDetails["CreditSub", dgvDetails.Rows.Count - 1].Value = SubTotalCredit;
                            dgvDetails["CreditSub", dgvDetails.Rows.Count - 1].Style.Font = MySubTotalFont;
                            dgvDetails["DebitSub", dgvDetails.Rows.Count - 1].Style.Font = MySubTotalFont;
                            dgvDetails.Rows.Add("");

                            SubBalance = SubTotalDebit - SubTotalCredit;
                            SubTotalDebit = 0;
                            SubTotalCredit = 0;

                            if (SubBalance != 0)
                            {
                                dgvDetails.Rows.Add("");
                                dgvDetails["Date", dgvDetails.Rows.Count - 1].Value = Convert.ToDateTime(Rs.fields("VchDate")).ToString("dd /MMM/yyyy");
                                dgvDetails["Particulars", dgvDetails.Rows.Count - 1].Value = "Opening Balance Of Cash";
                                if (SubBalance > 0)
                                    dgvDetails["DebitSub", dgvDetails.Rows.Count - 1].Value = Comm.FormatValue(Comm.ToDouble(Math.Abs(SubBalance)), true);
                                else
                                    dgvDetails["CreditSub", dgvDetails.Rows.Count - 1].Value = Comm.FormatValue(Comm.ToDouble(Math.Abs(SubBalance)), true);
                                SubTotalDebit = SubTotalDebit + Comm.ToDouble(dgvDetails["DebitSub", dgvDetails.Rows.Count - 1].Value);
                                SubTotalCredit = SubTotalCredit + Comm.ToDouble(dgvDetails["CreditSub", dgvDetails.Rows.Count - 1].Value);

                                dgvDetails["Balance", dgvDetails.Rows.Count - 1].Value = Comm.FormatValue(AmountD - AmountC, true) + Interaction.IIf(Comm.ToDouble(AmountD - AmountC) > 0, " Dr", " Cr");
                            }
                        }

                        if (tgsDetailed.ToggleState == Syncfusion.Windows.Forms.Tools.ToggleButtonState.Inactive)
                        {
                            if (PrevVchtypeID != 0 && PrevRefID != 0)
                            {
                                if (PrevVchtypeID != Comm.ToInt32(Rs.fields("VchtypeID")) || PrevRefID != Comm.ToInt32(Rs.fields("refID")))
                                {
                                    VchAmountC = 0;
                                    VchAmountD = 0;
                                    AccountGroup = "";

                                    dgvDetails.Rows.Add("");
                                }
                                else if ((PrevVchtypeID == Comm.ToInt32(Rs.fields("VchtypeID")) && PrevRefID == Comm.ToInt32(Rs.fields("refID")))
                                         && (VchAmountC > 0 && Comm.ToInt32(Rs.fields("AmountD")) > 0))
                                {
                                    VchAmountC = 0;
                                    VchAmountD = 0;
                                    AccountGroup = "";

                                    dgvDetails.Rows.Add("");
                                }
                            }
                        }
                        else
                        {
                            VchAmountC = 0;
                            VchAmountD = 0;
                            AccountGroup = "";

                            dgvDetails.Rows.Add("");
                        }

                        VchAmountC += Comm.ToDouble(Rs.fields("AmountC"));
                        VchAmountD += Comm.ToDouble(Rs.fields("AmountD"));

                        if (AccountGroup != "") 
                            AccountGroup += ",";
                        
                        AccountGroup += Rs.fields("AccountGroup");

                        dgvDetails["Date", dgvDetails.Rows.Count - 1].Value = Convert.ToDateTime(Rs.fields("VchDate")).ToString("dd/MMM/yyyy");
                        dgvDetails["Particulars", dgvDetails.Rows.Count - 1].Value = AccountGroup;
                        dgvDetails["Vchtype", dgvDetails.Rows.Count - 1].Value = Rs.fields("Vchtype");
                        dgvDetails["VchNo", dgvDetails.Rows.Count - 1].Value = Rs.fields("Vchno");
                        dgvDetails["Narration", dgvDetails.Rows.Count - 1].Value = Rs.fields("mynarration");
                        dgvDetails["DebitSub", dgvDetails.Rows.Count - 1].Value = Comm.FormatValue(VchAmountC, true);
                        dgvDetails["CreditSub", dgvDetails.Rows.Count - 1].Value = Comm.FormatValue(VchAmountD, true);
                        dgvDetails["DrillDownID", dgvDetails.Rows.Count - 1].Value = Rs.fields("refID");
                        dgvDetails["DrillDownType", dgvDetails.Rows.Count - 1].Value = Rs.fields("vchtypeID");
                        dgvDetails["Nature", dgvDetails.Rows.Count - 1].Value = "Opentrans";

                        PrevVchtypeID = Comm.ToInt32(Rs.fields("VchtypeID"));
                        PrevRefID = Comm.ToInt32(Rs.fields("refID"));

                        SubTotalDebit = SubTotalDebit + Comm.ToDouble(dgvDetails["DebitSub", dgvDetails.Rows.Count - 1].Value);
                        SubTotalCredit = SubTotalCredit + Comm.ToDouble(dgvDetails["CreditSub", dgvDetails.Rows.Count - 1].Value);

                        AmountD = AmountD + Comm.ToDouble(dgvDetails["DebitSub", dgvDetails.Rows.Count - 1].Value);
                        AmountC = AmountC + Comm.ToDouble(dgvDetails["CreditSub", dgvDetails.Rows.Count - 1].Value);
                        dgvDetails["Balance", dgvDetails.Rows.Count - 1].Value = Comm.FormatValue(AmountD - AmountC, true) + Interaction.IIf(Comm.ToDouble(AmountD - AmountC) > 0, " Dr", " Cr");
                    }
                    Rs.MoveNext();
                }

                //if (tgsDetailed.ToggleState == Syncfusion.Windows.Forms.Tools.ToggleButtonState.Inactive)
                //{
                //    if (VchAmountC != 0 || VchAmountD != 0)
                //    {
                //        dgvDetails["Date", dgvDetails.Rows.Count - 1].Value = Convert.ToDateTime(Rs.fields("VchDate")).ToString("dd/MMM/yyyy");
                //        dgvDetails["Particulars", dgvDetails.Rows.Count - 1].Value = AccountGroup;
                //        dgvDetails["Vchtype", dgvDetails.Rows.Count - 1].Value = Rs.fields("Vchtype");
                //        dgvDetails["VchNo", dgvDetails.Rows.Count - 1].Value = Rs.fields("Vchno");
                //        dgvDetails["Narration", dgvDetails.Rows.Count - 1].Value = Rs.fields("mynarration");
                //        dgvDetails["DebitSub", dgvDetails.Rows.Count - 1].Value = Comm.FormatValue(VchAmountD, true);
                //        dgvDetails["CreditSub", dgvDetails.Rows.Count - 1].Value = Comm.FormatValue(VchAmountC, true);
                //        dgvDetails["DrillDownID", dgvDetails.Rows.Count - 1].Value = Rs.fields("refID");
                //        dgvDetails["DrillDownType", dgvDetails.Rows.Count - 1].Value = Rs.fields("vchtypeID");
                //        dgvDetails["Nature", dgvDetails.Rows.Count - 1].Value = "Opentrans";
                //    }
                //}
            }

            dgvDetails.Rows.Add("");
            dgvDetails["DebitSub", dgvDetails.Rows.Count - 1].Value = Comm.FormatValue(SubTotalDebit, true);
            dgvDetails["CreditSub", dgvDetails.Rows.Count - 1].Value = Comm.FormatValue(SubTotalCredit, true);
            dgvDetails["CreditSub", dgvDetails.Rows.Count - 1].Style.Font = MySubTotalFont;
            dgvDetails["DebitSub", dgvDetails.Rows.Count - 1].Style.Font = MySubTotalFont;
            dgvDetails.Rows.Add("");

            dgvDetails.Rows.Add("");
            dgvDetails["DebitSub", dgvDetails.Rows.Count - 1].Value = "=============";
            dgvDetails["CreditSub", dgvDetails.Rows.Count - 1].Value = "=============";

            dgvDetails.Rows.Add("");
            dgvDetails["DebitSub", dgvDetails.Rows.Count - 1].Value = Comm.FormatValue(AmountD, true);
            dgvDetails["CreditSub", dgvDetails.Rows.Count - 1].Value = Comm.FormatValue(AmountC, true);
            dgvDetails["CreditSub", dgvDetails.Rows.Count - 1].Style.Font = MySubTotalFont;
            dgvDetails["DebitSub", dgvDetails.Rows.Count - 1].Style.Font = MySubTotalFont;


            dgvDetails.Rows.Add("");
            dgvDetails["DebitSub", dgvDetails.Rows.Count - 1].Value = "=============";
            dgvDetails["CreditSub", dgvDetails.Rows.Count - 1].Value = "=============";

            dgvDetails.Rows.Add("");
            if (Comm.ToDouble(AmountD - AmountC) > 0)
                dgvDetails["DebitSub", dgvDetails.Rows.Count - 1].Value = Comm.FormatValue(Comm.ToDouble(AmountD - AmountC), true);
            else
                dgvDetails["CreditSub", dgvDetails.Rows.Count - 1].Value = Comm.FormatValue(Math.Abs(Comm.ToDouble(AmountD - AmountC)), true);
            dgvDetails["Particulars", dgvDetails.Rows.Count - 1].Value = "Daybook Balance";

            dgvDetails["Particulars", dgvDetails.Rows.Count - 1].Style.Font = MySubTotalFont;
            dgvDetails["DebitSub", dgvDetails.Rows.Count - 1].Style.Font = MySubTotalFont;
            dgvDetails["DebitSub", dgvDetails.Rows.Count - 1].Style.Font = MySubTotalFont;


            dgvDetails.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
                DialogResult dlgResult = MessageBox.Show("Are you sure to close the window?", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                if (dlgResult.Equals(DialogResult.Yes))
                    this.Close();
        }

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            if (this.WindowState != FormWindowState.Minimized)
            {
                this.WindowState = FormWindowState.Minimized;
            }
        }

        private void cboCostCentre_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                dgvDetails.Focus();
                SendKeys.Send("{F4}");
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;

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

        private void dgvStockIn_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {

        }

        private void btnSettings_Click(object sender, EventArgs e)
        {

        }

        private void dgvStockIn_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void frmAccountStatements_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                if (e.KeyCode == Keys.F3)
                {

                }
                else if (e.KeyCode == Keys.Escape)
                {
                    DialogResult dlgResult = MessageBox.Show("Are you sure to exit Account Statement?", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                    if (dlgResult.Equals(DialogResult.Yes))
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

        private void txtTaxRegn_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsLetter(e.KeyChar) && !char.IsDigit(e.KeyChar);
        }

        private void cboState_SelectedValueChanged(object sender, EventArgs e)
        {

        }

        private void dgvStockIn_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {

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

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void button13_Click(object sender, EventArgs e)
        {

        }

        #endregion

        #region "METHODS ----------------------------------------------- >>"

        //Description : Seting Value to the Given Column, Row Indexes to Grid Cell
        private void SetValueOut(int iCellIndex, string sValue, string sConvertType = "")
        {
            if (dgvDetails.Rows.Count > 0)
            {
                if (sConvertType.ToUpper() == "CURR_FLOAT")
                {
                    if (sValue == "") sValue = "0";
                    //Commented and Added By Dipu on 17-Feb-2022 ------------- >>
                    dgvDetails.Rows[dgvDetails.CurrentRow.Index].Cells[iCellIndex].Value = Comm.chkChangeValuetoZero(Comm.FormatValue(Comm.ToDouble(sValue)));
                    //Commented and Added By Dipu on 17-Feb-2022 ------------- >>

                    this.dgvDetails.Columns[dgvDetails.CurrentCell.ColumnIndex].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
                else if (sConvertType.ToUpper() == "QTY_FLOAT")
                {
                    if (sValue == "") sValue = "0";
                    dgvDetails.Rows[dgvDetails.CurrentRow.Index].Cells[iCellIndex].Value = Comm.chkChangeValuetoZero(Comm.FormatValue(Comm.ToDouble(sValue), false));
                    this.dgvDetails.Columns[dgvDetails.CurrentCell.ColumnIndex].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
                else if (sConvertType.ToUpper() == "PERC_FLOAT")
                {
                    if (sValue == "") sValue = "0";
                    dgvDetails.Rows[dgvDetails.CurrentRow.Index].Cells[iCellIndex].Value = Comm.chkChangeValuetoZero(Comm.ToDecimal(sValue).ToString("#.00"));
                    this.dgvDetails.Columns[dgvDetails.CurrentCell.ColumnIndex].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
                else if (sConvertType.ToUpper() == "DATE")
                {
                    dgvDetails.Rows[dgvDetails.CurrentRow.Index].Cells[iCellIndex].Value = Convert.ToDateTime(sValue).ToString("dd/MMM/yyyy");
                }
                else
                    dgvDetails.Rows[dgvDetails.CurrentRow.Index].Cells[iCellIndex].Value = Comm.chkChangeValuetoZero(sValue);
            }
        }

        //Description : Setting Value to Tag of the cells of Grid using the Given Column and Row Indexes
        private void setTagOut(int iCellIndex, string sTag, string sConvertType = "")
        {
            if (sConvertType.ToUpper() == "FLOAT")
            {
                if (sTag == "") sTag = "0";
                dgvDetails.Rows[dgvDetails.CurrentRow.Index].Cells[iCellIndex].Tag = Comm.ToDecimal(sTag).ToString("#.00");
            }
            else if (sConvertType.ToUpper() == "DATE")
            {
                dgvDetails.Rows[dgvDetails.CurrentRow.Index].Cells[iCellIndex].Tag = Convert.ToDateTime(sTag).ToString("dd/MMM/yyyy");
            }
            else
                dgvDetails.Rows[dgvDetails.CurrentRow.Index].Cells[iCellIndex].Tag = sTag;
        }

        //Description : Function Polymorphism for Seting the Value to Grid Asper Parameters Given
        private void SetValueOut(int iCellIndex, int iRowIndex, string sValue, string sConvertType = "")
        {
            dgvDetails.Rows[iRowIndex].Cells[iCellIndex].Value = Comm.chkChangeValuetoZero(sValue);
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

        //Description : Clear the Form and Grid 
        private void ClearControls()
        {
            dgvDetails.Rows.Clear();
            dgvDetails.Refresh();
            dgvDetails.Rows.Add();
            dgvDetails.CurrentCell = dgvDetails[1, 0];
        }

        //Description : Function Polymorphism of SetTag
        private void SetTagOut(int iCellIndex, int iRowIndex, string sTag, string sConvertType = "")
        {
            if (sConvertType.ToUpper() == "PERC_FLOAT")
                dgvDetails.Rows[iRowIndex].Cells[iCellIndex].Tag = Comm.ToDecimal(sTag).ToString("#.00");
            else
                dgvDetails.Rows[iRowIndex].Cells[iCellIndex].Tag = sTag;
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
            
            dgvStock.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cPrate", HeaderText = "PRate", Width = 80 }); //7

            if (AppSettings.TaxMode == 2) //GST
                dgvStock.Columns.Add(new DataGridViewCheckBoxColumn() { Name = "cRateinclusive", HeaderText = "Rate Inc.", Width = 80, ReadOnly = true }); //20
            else
                dgvStock.Columns.Add(new DataGridViewCheckBoxColumn() { Name = "cRateinclusive", HeaderText = "Rate Inc.", Visible = false, Width = 80, ReadOnly = true }); //20

            dgvStock.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cQty", HeaderText = "Qty", Width = 80 }); //8
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

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
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

            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
        }

        private void frmAccountStatements_Activated(object sender, EventArgs e)
        {

        }


        private void dgvColWidth_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            
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

        private void frmAccountStatements_Shown(object sender, EventArgs e)
        {
            try
            {

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

            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void dgvStockOut_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void dgvStockOut_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;

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
            try
            {

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
            }
            catch
            { }
        }

        private void dgvStockOut_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
            }
            catch
            { }
        }

        private void dgvStockOut_ColumnWidthChanged(object sender, DataGridViewColumnEventArgs e)
        {
            try
            {
                if (this.ActiveControl == null) return;
                if (this.ActiveControl.Name != dgvDetails.Name) return;
            }
            catch
            { }

        }

        private void dgvStockOut_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
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

        private void dgvStockOut_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            FillReport();
        }

        private void dgvStockOut_Scroll(object sender, ScrollEventArgs e)
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

        #endregion
    }
}
