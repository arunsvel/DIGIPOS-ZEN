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
                    case "DAYBOOKSUMMARY":
                        DAYBOOKSUMMARY();

                        break;
                    case "DAYBOOK":
                        dayBook();

                        break;
                    case "TRIALBALANCE":
                        trialbalance();

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
        
        private void DAYBOOKSUMMARY()
        {
            try
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

                sqlControl Rs = new sqlControl();

                double SubTotalDebit = 0;
                double SubTotalCredit = 0;
                double SubBalance = 0;

                tgsDetailed.Visible = false;

                Rs.Open("SELECT        TOP (100) PERCENT dbo.tblVoucher.VchDate, dbo.tblVoucher.vchTime, dbo.tblVoucher.VchNo, SUM(dbo.tblVoucher.AmountD) AS AmountD, SUM(dbo.tblVoucher.AmountC) AS AmountC, dbo.tblVoucher.RefID, dbo.tblVoucher.VchTypeID, dbo.tblVchType.VchType, dbo.tblVoucher.mynarration  FROM   dbo.tblVoucher INNER JOIN dbo.tblVchType ON dbo.tblVoucher.VchTypeID = dbo.tblVchType.VchTypeID WHERE tblvoucher.optional=0 and   (dbo.tblVoucher.LedgerID = 3 AND VCHDATE BETWEEN  '" + dtpfrom.Value.ToString("dd/MMM/yyyy") + "' AND '" + dtpto.Value.ToString("dd/MMM/yyyy") + "')  " + StrCCSQL1 + " and tblVoucher.vchtypeid not in (1005)  GROUP BY dbo.tblVoucher.VchDate, dbo.tblVoucher.VchNo, dbo.tblVoucher.RefID, dbo.tblVoucher.vchTime, dbo.tblVoucher.VchTypeID, dbo.tblVchType.VchType, dbo.tblVoucher.mynarration  having Sum(dbo.tblvoucher.amountd) - Sum(dbo.tblvoucher.amountc) <> 0 " + 
                        " Union " + 
                        "SELECT        TOP (100) PERCENT dbo.tblVoucher.VchDate, dbo.tblVoucher.vchTime, dbo.tblVoucher.VchNo, SUM(dbo.tblVoucher.AmountD) AS AmountD, SUM(dbo.tblVoucher.AmountC) AS AmountC, dbo.tblVoucher.RefID, dbo.tblVoucher.VchTypeID, dbo.tblVchType.VchType, dbo.tblVoucher.mynarration  FROM   dbo.tblVoucher INNER JOIN dbo.tblVchType ON dbo.tblVoucher.VchTypeID = dbo.tblVchType.VchTypeID WHERE tblvoucher.optional=0 and   (dbo.tblVoucher.LedgerID <> 3) " + StrCCSQL1 + " AND VCHDATE BETWEEN  '" + dtpfrom.Value.ToString("dd/MMM/yyyy") + "' AND '" + dtpto.Value.ToString("dd/MMM/yyyy") + "' and tblVoucher.vchtypeid not in (1005)  GROUP BY dbo.tblVoucher.VchDate, dbo.tblVoucher.vchTime, dbo.tblVoucher.VchTypeID, dbo.tblVchType.VchType, dbo.tblVoucher.VchNo, dbo.tblVoucher.RefID, dbo.tblVoucher.mynarration having Sum(dbo.tblvoucher.amountd) - Sum(dbo.tblvoucher.amountc) <> 0 " + " ORDER BY dbo.tblVoucher.VchDate, dbo.tblVoucher.vchTime, dbo.tblVoucher.VchNo,dbo.tblVoucher.mynarration, AmountD ");

                Font MySubTotalFont = new Font("Segoe UI", 10, FontStyle.Bold);
                double DebitVal = 0;
                double CreditVal = 0;
                {
                    DateTime PrevDate = dtpfrom.Value; //.ToString("dd/MMM/YYYY");
                    SubTotalCredit = 0;
                    SubTotalDebit = 0;

                    int PrevVchtypeID = 0;
                    int PrevRefID = 0;

                    double VchAmountC = 0;
                    double VchAmountD = 0;

                    while (!Rs.eof())
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

                                    dgvDetails.Rows.Add("");
                                }
                                else if ((PrevVchtypeID == Comm.ToInt32(Rs.fields("VchtypeID")) && PrevRefID == Comm.ToInt32(Rs.fields("refID")))
                                         && (VchAmountC > 0 && Comm.ToInt32(Rs.fields("AmountD")) > 0))
                                {
                                    VchAmountC = 0;
                                    VchAmountD = 0;

                                    dgvDetails.Rows.Add("");
                                }
                            }
                        }
                        else
                        {
                            VchAmountC = 0;
                            VchAmountD = 0;

                            dgvDetails.Rows.Add("");
                        }

                        VchAmountC += Comm.ToDouble(Rs.fields("AmountC"));
                        VchAmountD += Comm.ToDouble(Rs.fields("AmountD"));


                        dgvDetails["Date", dgvDetails.Rows.Count - 1].Value = Convert.ToDateTime(Rs.fields("VchDate")).ToString("dd/MMM/yyyy");
                        dgvDetails["Vchtype", dgvDetails.Rows.Count - 1].Value = Rs.fields("Vchtype");
                        dgvDetails["VchNo", dgvDetails.Rows.Count - 1].Value = Rs.fields("Vchno");
                        dgvDetails["DebitSub", dgvDetails.Rows.Count - 1].Value = Comm.FormatValue(VchAmountC, true);
                        dgvDetails["CreditSub", dgvDetails.Rows.Count - 1].Value = Comm.FormatValue(VchAmountD, true);
                        dgvDetails["DrillDownID", dgvDetails.Rows.Count - 1].Value = Rs.fields("refID");
                        dgvDetails["DrillDownType", dgvDetails.Rows.Count - 1].Value = Rs.fields("vchtypeID");
                        dgvDetails["Nature", dgvDetails.Rows.Count - 1].Value = "Opentrans";

                        PrevVchtypeID = Comm.ToInt32(Rs.fields("VchtypeID"));
                        PrevRefID = Comm.ToInt32(Rs.fields("refID"));

                        SubTotalDebit = SubTotalDebit + Comm.ToDouble(Rs.fields("AmountC")); // Comm.ToDouble(dgvDetails["DebitSub", dgvDetails.Rows.Count - 1].Value);
                        SubTotalCredit = SubTotalCredit + Comm.ToDouble(Rs.fields("AmountD")); // Comm.ToDouble(dgvDetails["CreditSub", dgvDetails.Rows.Count - 1].Value);

                        AmountD = AmountD + Comm.ToDouble(Rs.fields("AmountC")); // Comm.ToDouble(dgvDetails["DebitSub", dgvDetails.Rows.Count - 1].Value);
                        AmountC = AmountC + Comm.ToDouble(Rs.fields("AmountD")); // Comm.ToDouble(dgvDetails["CreditSub", dgvDetails.Rows.Count - 1].Value);
                        //dgvDetails["Balance", dgvDetails.Rows.Count - 1].Value = Comm.FormatValue(AmountD - AmountC, true) + Interaction.IIf(Comm.ToDouble(AmountD - AmountC) > 0, " Dr", " Cr");

                        Rs.MoveNext();
                    }
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

                dgvDetails["DebitSub", dgvDetails.Rows.Count - 1].Style.Font = MySubTotalFont;
                dgvDetails["DebitSub", dgvDetails.Rows.Count - 1].Style.Font = MySubTotalFont;

                dgvDetails.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void dayBook()
        {
            try
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

                            SubTotalDebit = SubTotalDebit + Comm.ToDouble(Rs.fields("AmountC")); // Comm.ToDouble(dgvDetails["DebitSub", dgvDetails.Rows.Count - 1].Value);
                            SubTotalCredit = SubTotalCredit + Comm.ToDouble(Rs.fields("AmountD")); // Comm.ToDouble(dgvDetails["CreditSub", dgvDetails.Rows.Count - 1].Value);

                            AmountD = AmountD + Comm.ToDouble(Rs.fields("AmountC")); // Comm.ToDouble(dgvDetails["DebitSub", dgvDetails.Rows.Count - 1].Value);
                            AmountC = AmountC + Comm.ToDouble(Rs.fields("AmountD")); // Comm.ToDouble(dgvDetails["CreditSub", dgvDetails.Rows.Count - 1].Value);
                            dgvDetails["Balance", dgvDetails.Rows.Count - 1].Value = Comm.FormatValue(AmountD - AmountC, true) + Interaction.IIf(Comm.ToDouble(AmountD - AmountC) > 0, " Dr", " Cr");
                        }
                        Rs.MoveNext();
                    }
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
                dgvDetails["Particulars", dgvDetails.Rows.Count - 1].Value = "Closing Balance Of Cash";

                dgvDetails["Particulars", dgvDetails.Rows.Count - 1].Style.Font = MySubTotalFont;
                dgvDetails["DebitSub", dgvDetails.Rows.Count - 1].Style.Font = MySubTotalFont;
                dgvDetails["DebitSub", dgvDetails.Rows.Count - 1].Style.Font = MySubTotalFont;


                dgvDetails.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            catch(Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        public double GetStockValue(DateTime fromDate, bool blnisOpening, string StrCCID)
        {
            if (blnisOpening == false)
            {
                try
                {
                    if (Strings.Trim(StrCCID) != "")
                    {
                        if (Strings.Right(StrCCID, 1) == ",")
                            StrCCID = Strings.Left(StrCCID, Strings.Len(StrCCID) - 1);
                    }
                    if (Strings.Trim(StrCCID) != "")
                        StrCCID = " where ccid in (" + StrCCID + ") ";

                    sqlControl Rs = new sqlControl();
                    string SQl;
                    double TOTALVALUEnew = 0;
                    Rs.Open("Select sum(qtyin * CostRateExcl)-sum(qtyout * CostRateExcl) as Clsstk from tblstockhistory " + StrCCID + " and VchDate <= '" + fromDate.ToString("dd/MMM/yyyy") + "' ");
                    if (!Rs.eof())
                    {
                        TOTALVALUEnew = Comm.ToDouble(Rs.fields("Clsstk"));
                    }
                    
                    return TOTALVALUEnew;

                }
                catch (Exception ex)
                {
                    Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return 0;
                }
            }
            else
            {
                try
                {
                    double TOTALVALUE = 0;
                    int IntOpening = 0;
                    if (blnisOpening == true)
                        IntOpening = 1;

                    int i = 0;

                    sqlControl Rs = new sqlControl();
                    string SQl;
                    string[] CCIDS;
                    if (Strings.InStr(1, StrCCID, ",") > 0)
                        CCIDS = Strings.Split(StrCCID, ",");
                    else
                        CCIDS = Strings.Split(StrCCID + ",", ",");

                    for (i = 0; i <= Information.UBound(CCIDS); i++)
                    {
                        if (CCIDS[i] == "" & i != 0)
                            break;

                        Rs.Open("Select sum(qtyin * CostRateExcl)-sum(qtyout * CostRateExcl) as Clsstk from tblstockhistory Where VchDate <= '" + fromDate.ToString("dd/MMM/yyyy") + "' " + StrCCID + " ");
                        if (!Rs.eof())
                        {
                            TOTALVALUE += Comm.ToDouble(Rs.fields("Clsstk"));
                        }

                        //string Connstring = Properties.Settings.Default.ConnectionString;
                        //using (SqlConnection Connection = new SqlConnection(Connstring))
                        //{
                        //    Connection.Open();
                        //    SqlCommand Command = new SqlCommand("OpeningStock", Connection);
                        //    Command.CommandType = CommandType.StoredProcedure;
                        //    Command.Parameters.AddWithValue("@blnisOpening", IntOpening);
                        //    Command.Parameters.AddWithValue("@CCIDs", CCIDS[i]);
                        //    Command.Parameters.AddWithValue("@VchDate", Strings.Format(fromDate, "dd-MMM-yyyy"));
                        //    Command.CommandTimeout = 0;
                        //    SqlParameter PramCOIDRet = new SqlParameter();
                        //    PramCOIDRet.ParameterName = "@TotalStockValue";
                        //    PramCOIDRet.SqlDbType = SqlDbType.Float;
                        //    PramCOIDRet.Size = 800;
                        //    PramCOIDRet.Direction = ParameterDirection.Output;
                        //    Command.Parameters.Add(PramCOIDRet);
                        //    Command.ExecuteNonQuery();
                        //    TOTALVALUE = 0;

                        //    TOTALVALUE = Command.Parameters("@TotalStockValue").Value;
                        //}
                        //GetStockValue = GetStockValue + Comm.ToDouble(TOTALVALUE);

                        if (CCIDS[i] == "")
                            break;
                    }

                    return TOTALVALUE;
                }
                catch (Exception ex)
                {
                    Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return 0;
                }
            }
        }

        private void trialbalance()
        {
            try
            {
                dgvDetails.Columns.Clear();
                DataGridViewTextBoxColumn Particulars = new DataGridViewTextBoxColumn();
                tgsDetailed.Visible = true;
                lblHeading.Text = "Trial Balance between " + dtpfrom.Value.ToString("dd/MMM/yyyy") + "  and " + dtpto.Value.ToString("dd/MMM/yyyy");
                Particulars.HeaderText = "Particulars";
                Particulars.Name = "Particulars";
                Particulars.ReadOnly = true;
                Particulars.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                Particulars.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
                Particulars.SortMode = DataGridViewColumnSortMode.NotSortable;
                Particulars.DefaultCellStyle.BackColor = Color.LightSkyBlue;
                dgvDetails.Columns.Add(Particulars);

                string strCCIDsql = Comm.GetCheckedData(lstCostCentre);
                string StrCCSQL1 = "";
                if (strCCIDsql != "")
                    StrCCSQL1 = strCCIDsql.Replace("and", "") + " AND ";

                DataGridViewTextBoxColumn DebitSub = new DataGridViewTextBoxColumn();
                DebitSub.HeaderText = "DebitSub";
                DebitSub.Name = "DebitSub";
                DebitSub.ReadOnly = true;
                DebitSub.SortMode = DataGridViewColumnSortMode.NotSortable;
                DebitSub.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                DebitSub.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvDetails.Columns.Add(DebitSub);

                DataGridViewTextBoxColumn DebitGroupTotal = new DataGridViewTextBoxColumn();
                DebitGroupTotal.HeaderText = "Debit";
                DebitGroupTotal.Name = "DebitGroupTotal";
                DebitGroupTotal.SortMode = DataGridViewColumnSortMode.NotSortable;
                DebitGroupTotal.ReadOnly = true;
                DebitGroupTotal.DefaultCellStyle.Font = new Font("Tahoma", Convert.ToSingle(9), FontStyle.Bold);
                DebitGroupTotal.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                DebitGroupTotal.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvDetails.Columns.Add(DebitGroupTotal);




                DataGridViewTextBoxColumn CreditSub = new DataGridViewTextBoxColumn();
                CreditSub.HeaderText = "CreditSub";
                CreditSub.Name = "CreditSub";
                CreditSub.ReadOnly = true;
                CreditSub.SortMode = DataGridViewColumnSortMode.NotSortable;
                CreditSub.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                CreditSub.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;

                dgvDetails.Columns.Add(CreditSub);

                DataGridViewTextBoxColumn CreditGroupTotal = new DataGridViewTextBoxColumn();
                CreditGroupTotal.HeaderText = "Credit";
                CreditGroupTotal.Name = "CreditGroupTotal";
                CreditGroupTotal.ReadOnly = true;
                CreditGroupTotal.SortMode = DataGridViewColumnSortMode.NotSortable;
                CreditGroupTotal.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                CreditGroupTotal.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                CreditGroupTotal.DefaultCellStyle.Font = new Font("Tahoma", Convert.ToSingle(9), FontStyle.Bold);
                dgvDetails.Columns.Add(CreditGroupTotal);

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
                DrillDownType.SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvDetails.Columns.Add(DrillDownType);

                DataGridViewTextBoxColumn nature = new DataGridViewTextBoxColumn();
                nature.HeaderText = "Nature";
                nature.Name = "Nature";
                nature.ReadOnly = true;
                nature.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                DrillDownType.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
                nature.Visible = false;
                dgvDetails.Columns.Add(nature);

                if (tgsDetailed.ToggleState == Syncfusion.Windows.Forms.Tools.ToggleButtonState.Inactive)
                {
                    CreditSub.Visible = false;
                    DebitSub.Visible = false;
                }
                else
                {
                    CreditSub.Visible = true;
                    DebitSub.Visible = true;
                }

                sqlControl rs = new sqlControl();
                sqlControl rs1 = new sqlControl();

                //dgvSettings.CurrentCell = dgvSettings.Rows(2).Cells(0);

                // Two functions
                double OpeningBalance = 0;
                double OpStockBalance = 0;

                double AmountD = 0;
                double AmountC = 0;
                dgvDetails.Rows.Add("==");

                string MstrDatecriteria;

                if (dtpfrom.Value == Global.FyStartDate)
                    MstrDatecriteria = "and vchdate between '" + dtpfrom.Value.ToString("dd/MMM/yyyy") + "' and '" + dtpto.Value.ToString("dd/MMM/yyyy") + "'";
                else
                    MstrDatecriteria = "and vchdate between '" + dtpfrom.Value.ToString("dd/MMM/yyyy") + "' and '" + dtpto.Value.ToString("dd/MMM/yyyy") + "'";

                if (dtpfrom.Value <= Global.FyStartDate)
                {
                    sqlControl rsOp = new sqlControl();
                    // difference In opening Balance
                    rsOp.Open("select sum(AmountC-AmountD) as Total from tblvoucher where 1=1 " + StrCCSQL1 + " AND Optional=0    and vchtypeid=1005 and  LedgerID=999 ");
                    if (!rsOp.eof())
                    {
                        if (rsOp.fields("Total") != null)
                            OpeningBalance = Comm.ToDouble(rsOp.fields("Total").ToString());
                    }
                    // 'opening stock balance
                    //string CCIDString = getcheckeditemdataSpecial(LstCostCentre);
                    OpStockBalance = Comm.ToDouble(GetStockValue(dtpfrom.Value, true, strCCIDsql));
                }
                else
                    OpeningBalance = 0;

                dgvDetails["Particulars", dgvDetails.Rows.Count - 1].Value = "OPENING STOCK";

                if (Comm.ToDouble(OpStockBalance) < 0)
                    dgvDetails["CreditGroupTotal", dgvDetails.Rows.Count - 1].Value = Comm.FormatAmt(Math.Abs(OpStockBalance), AppSettings.CurrDecimalFormat);
                else
                    dgvDetails["DebitGroupTotal", dgvDetails.Rows.Count - 1].Value = Comm.FormatAmt(OpStockBalance, AppSettings.CurrDecimalFormat);

                dgvDetails.Rows.Add("==");

                if (Math.Abs(OpeningBalance + OpStockBalance) != 0)
                {
                    dgvDetails["Particulars", dgvDetails.Rows.Count - 1].Value = "Diff In Op Balance";

                    if (Comm.ToDouble(OpeningBalance + OpStockBalance) < 0)
                        dgvDetails["DebitGroupTotal", dgvDetails.Rows.Count - 1].Value = Comm.FormatAmt(Math.Abs(OpeningBalance + OpStockBalance), AppSettings.CurrDecimalFormat);
                    else
                        dgvDetails["CreditGroupTotal", dgvDetails.Rows.Count - 1].Value = Comm.FormatAmt(Math.Abs(OpeningBalance + OpStockBalance), AppSettings.CurrDecimalFormat);

                    // End If
                    dgvDetails.Rows.Add("==");
                }
                else
                {
                }

                FillAccountGroupTB(6, true, "Direct Expense", false);

                FillAccountGroupTB(7, true, "Direct Income");

                FillAccountGroupTB(9, true, "InDirect Expense");
                FillAccountGroupTB(1, true, "InDirect Income");

                FillAccountGroupTB(2, true, "Current Asset");
                FillAccountGroupTB(3, true, "Current Liability");

                FillAccountGroupTB(24, true, "Capital");
                FillAccountGroupTB(5, true, "LongTerm Liability");

                FillAccountGroupTB(8, true, "Fixed Asset");


                int i = 0;



                for (i = 0; i <= dgvDetails.Rows.Count - 1; i++)
                {
                    AmountD = AmountD + Comm.ToDouble(dgvDetails["DebitGroupTotal", i].Value);
                    AmountC = AmountC + Comm.ToDouble(dgvDetails["CreditGroupTotal", i].Value);
                }
                dgvDetails.Rows.Add("");
                dgvDetails["DebitGroupTotal", dgvDetails.Rows.Count - 1].Value = AmountD;
                dgvDetails["CreditGroupTotal", dgvDetails.Rows.Count - 1].Value = AmountC;
                dgvDetails.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            catch (Exception ex)
            {
                Interaction.MsgBox(ex.Message);
            }
        }

        public bool FillAccountGroupTB(long AccountgroupID, bool BlnFirstLevel, string groupName = "", bool BlnTotaltype = false, bool FORCEDETAILEDVIEW = false)
        {
            try
            {
                this.Visible = true;
                if (dgvDetails.Columns.Count == 0)
                    InitialiseGridforDrilling();

                string MstrDatecriteria;

                if (dtpfrom.Value == Global.FyStartDate)
                    MstrDatecriteria = "and vchdate between '" + dtpfrom.Value.ToString("dd/MMM/yyyy") + "' and '" + dtpto.Value.ToString("dd/MMM/yyyy") + "'";
                else
                    MstrDatecriteria = "and vchdate between '" + dtpfrom.Value.ToString("dd/MMM/yyyy") + "' and '" + dtpto.Value.ToString("dd/MMM/yyyy") + "'";

                sqlControl rs = new sqlControl();
                string StrSqbQry = "";

                string strCCIDsql = "";
                strCCIDsql = Comm.GetCheckedData(lstCostCentre);
                if (strCCIDsql != "")
                    strCCIDsql = " and CCID IN (" + strCCIDsql + ") ";

                if (BlnFirstLevel)
                    StrSqbQry = " OR tblAccountgroup.AccountGroupID = " + AccountgroupID;

                sqlControl rs1 = new sqlControl();
                rs1.Open(" select Sum(Amountd-Amountc) as Amount,AccountGroup as GroupName,ParentID,tblAccountgroup.AccountGroupID,HID from tblVoucher,tblLedger,tblAccountgroup where " + " tblVoucher.LedgerID = tblLedger.lid  and optional = 0 And tblAccountgroup.Accountgroupid = tblLedger.Accountgroupid   " + strCCIDsql + MstrDatecriteria + " and (ParentID = " + AccountgroupID + StrSqbQry + " ) " + " group by AccountGroup,ParentID,tblAccountgroup.AccountGroupID,HID  ");

                rs.Open("select DISTINCT SortOrder,nature,TBLaCCOUNTgroup.accountGroup as GroupName,tblAccountgroup.AccountGroupID,HID,len(HID) from  " + " tblAccountgroup  " + " Where (ParentID = " + AccountgroupID + StrSqbQry + " )  group by SortOrder,nature,TBLaCCOUNTgroup.accountGroup,ParentID,tblAccountgroup.AccountGroupID,HID,Len(HID) order by len(HID),SortOrder ");

                {
                    var withBlock = dgvDetails;
                    if (BlnFirstLevel && rs.eof())
                    {
                        dgvDetails["Particulars", withBlock.Rows.Count - 1].Value = groupName.ToUpper();
                        dgvDetails.Rows.Add("");
                    }
                    else if (BlnFirstLevel && rs.eof() == false)
                    {
                        bool BlnParentExists = false;
                        while (!rs.eof())
                        {
                            if (Comm.ToInt32(rs.fields("AccountgroupID")) == AccountgroupID)
                            {
                                BlnParentExists = true; break;
                            }
                            rs.MoveNext();
                        }
                        if (BlnParentExists == false)
                            dgvDetails["Particulars", withBlock.Rows.Count - 1].Value = Strings.UCase(groupName);
                    }
                    // 0,1,2,5,6 are shown columns
                    // dgvDetails.Rows.Add("==")
                    if (rs.RecordCount > 0)
                        rs.MoveFirst();
                    while (!rs.eof())
                    {
                        this.Visible = true;
                        {
                            var withBlock1 = dgvDetails;
                            dgvDetails["Particulars", dgvDetails.Rows.Count - 1].Value = Comm.ToInt32(rs.fields("AccountgroupID")) == AccountgroupID ? "" : new string(' ', rs.fields("HID").ToString().Length) + rs.fields("groupName");
                            dgvDetails["Particulars", dgvDetails.Rows.Count - 1].Style.Font = new Font("Tahoma", Convert.ToSingle(9), FontStyle.Bold);
                            dgvDetails["DrillDownID", dgvDetails.Rows.Count - 1].Value = rs.fields("AccountgroupID");
                            dgvDetails["DrillDownType", dgvDetails.Rows.Count - 1].Value = "AccountGROUP";
                            dgvDetails["Nature", dgvDetails.Rows.Count - 1].Value = "Accountgroup".ToUpper();

                            if (rs1.RecordCount > 0)
                                rs1.MoveFirst();
                            rs1.Find("AccountGroupID", rs.fields("AccountgroupID").ToString(), true);
                            if (rs1.eof() == false)
                            {
                                if (Comm.ToDouble(rs1.fields("Amount")) > 0)
                                    dgvDetails["DebitGroupTotal", dgvDetails.Rows.Count - 1].Value = Comm.FormatAmt(Comm.ToDouble(rs1.fields("Amount")), AppSettings.CurrDecimalFormat);
                                else
                                    dgvDetails["CreditGroupTotal", dgvDetails.Rows.Count - 1].Value = Comm.FormatAmt(Comm.ToDouble(Math.Abs(Comm.ToDouble(rs1.fields("Amount")))), AppSettings.CurrDecimalFormat);
                            }
                            dgvDetails.Rows.Add("--");

                            if (tgsDetailed.ToggleState == Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active || FORCEDETAILEDVIEW == true)
                                FillTrialBalanceLedger(Comm.ToInt32(rs.fields("AccountgroupID")), new string(' ', rs.fields("HID").Length), rs.fields("Nature"));
                            if (AccountgroupID != Comm.ToDouble(rs.fields("AccountgroupID")))
                                FillAccountGroupTB(Comm.ToInt32(rs.fields("AccountgroupID")), false, "", false);
                            // FillAccountGroupTB rs!AccountGroupId, msg, False

                            rs.MoveNext();
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
        }

        private void FillTrialBalanceLedger(int AccountgroupID, string StrSpacing, string AccountgroupNature = "")
        {
            string MstrDatecriteria;

            if (dtpfrom.Value == Global.FyStartDate)
                MstrDatecriteria = "and vchdate between '" + dtpfrom.Value.ToString("dd/MMM/yyyy") + "' and '" + dtpto.Value.ToString("dd/MMM/yyyy") + "'";
            else
                MstrDatecriteria = "and vchdate between '" + dtpfrom.Value.ToString("dd/MMM/yyyy") + "' and '" + dtpto.Value.ToString("dd/MMM/yyyy") + "'";

            string strCCIDsql = Comm.GetCheckedData(lstCostCentre);

            string StrCCSQL1 = "";
            if (strCCIDsql != "")
                StrCCSQL1 = strCCIDsql.Replace("and", "") + " AND ";

            // 1,2,5,6
            sqlControl rs = new sqlControl();
            rs.Open("Select LID,lAliasName as Name,Sum(AmountD-Amountc) as Amount from tblVoucher,tblLedger where  tblVoucher.LedgerID = tblLedger.LID " + MstrDatecriteria + strCCIDsql + " and optional = 0 and AccountgroupID=" + AccountgroupID + " group By lAliasName,LID having round(Sum(AmountD-Amountc),4) <>0 order by lAliasName  ");
            {
                var withBlock = dgvDetails;
                while (!rs.eof())
                {
                    // ====================================================================
                    withBlock["Particulars", withBlock.Rows.Count - 1].Value = StrSpacing + " ○ " + rs.fields("Name").ToLower();
                    withBlock["Particulars", dgvDetails.Rows.Count - 1].Style.Font = new Font("Tahoma", Convert.ToSingle(12), FontStyle.Italic);
                    if (Comm.ToDouble(rs.fields("Amount")) > 0)
                        withBlock["DebitSub", withBlock.Rows.Count - 1].Value = Comm.FormatAmt(Comm.ToDouble(rs.fields("Amount")), AppSettings.CurrDecimalFormat);
                    else
                        withBlock["CreditSub", withBlock.Rows.Count - 1].Value = Comm.FormatAmt(Math.Abs(Comm.ToDouble(rs.fields("Amount"))), AppSettings.CurrDecimalFormat);
                    withBlock["DrillDownID", withBlock.Rows.Count - 1].Value = rs.fields("LID");

                    withBlock["Nature", withBlock.Rows.Count - 1].Value = "LEDGER";

                    if (Strings.Trim(AccountgroupNature) != "")
                        withBlock["DrillDownType", withBlock.Rows.Count - 1].Value = AccountgroupNature;
                    dgvDetails.Rows.Add("");
                    // ====================================================================
                    rs.MoveNext();
                }
            }
            return;
        }

        public void InitialiseGridforDrilling()
        {
            dgvDetails.Columns.Clear();
            tgsDetailed.Visible = true;
            //lblCaption.Text = "Ledger between " + Comm.FormatAmt(dtpfrom.Value, "dd/MMM/yyyy") + "  and " + Comm.FormatAmt(dtpto.Value, "dd/MMM/yyyy");
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
            vchType.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            vchType.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvDetails.Columns.Add(vchType);

            DataGridViewTextBoxColumn VchNo = new DataGridViewTextBoxColumn();
            VchNo.HeaderText = "VchNo";
            VchNo.Name = "VchNo";
            VchNo.ReadOnly = true;
            // VchNo.DefaultCellStyle.Font = New Font("Tahoma", Convert.ToSingle(My.Settings.FontSize), FontStyle.Bold)
            VchNo.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            VchNo.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvDetails.Columns.Add(VchNo);


            DataGridViewTextBoxColumn narration = new DataGridViewTextBoxColumn();
            narration.HeaderText = "Narration";
            narration.Name = "Narration";
            narration.ReadOnly = true;
            // VchNo.DefaultCellStyle.Font = New Font("Tahoma", Convert.ToSingle(My.Settings.FontSize), FontStyle.Bold)
            narration.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            narration.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvDetails.Columns.Add(narration);


            DataGridViewTextBoxColumn DebitSub = new DataGridViewTextBoxColumn();
            DebitSub.HeaderText = "Debit";
            DebitSub.Name = "DebitSub";
            DebitSub.ReadOnly = true;
            DebitSub.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            DebitSub.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvDetails.Columns.Add(DebitSub);


            DataGridViewTextBoxColumn CreditSub = new DataGridViewTextBoxColumn();
            CreditSub.HeaderText = "Credit";
            CreditSub.Name = "CreditSub";
            CreditSub.ReadOnly = true;
            CreditSub.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            CreditSub.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvDetails.Columns.Add(CreditSub);

            DataGridViewTextBoxColumn Balance = new DataGridViewTextBoxColumn();
            Balance.HeaderText = "Balance";
            Balance.Name = "Balance";
            Balance.ReadOnly = true;
            Balance.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            Balance.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
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

            tgsDetailed.Visible = false;
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

        //Description : Clear the Form and Grid 
        private void ClearControls()
        {
            dgvDetails.Rows.Clear();
            dgvDetails.Refresh();

            dgvDetails.Columns.Clear();
            dgvDetails.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSlNo", HeaderText = "Sl.No", Width = 50 }); //1

            dgvDetails.Rows.Add();
            dgvDetails.CurrentCell = dgvDetails[0, 0];
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
