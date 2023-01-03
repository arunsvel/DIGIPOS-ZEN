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
using System.IO;
using System.Threading;
using System.Drawing.Printing;

namespace DigiposZen
{

    public partial class frmBarcode : Form //, IMessageFilter
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

        //private frmCompactSearch frmItemSearch;
        //private frmCompactSearch frmBatchSearch;

        private TabPage SettingsTab;
        sqlControl bsdata = new sqlControl();
        sqlControl rs = new sqlControl();
        string mReportName = "";
        int mVchtypeID;
        decimal mInvID;
        string MyPrinterName = "";
        string strFileData = "";

        public frmBarcode(int VchtypeID, decimal InvID, string InvNo, string SchemeName, object MDIParent)//(int iVchTpeId = 0, int iTransID = 0, bool bFromEdit = false, object MDIParent = null)
        {
            InitializeComponent();
            //Application.AddMessageFilter(this);

            controlsToMove.Add(this);
            controlsToMove.Add(this.lblHeading);//Add whatever controls here you want to move the form when it is clicked and dragged


            try
            {
                Comm.TransparentControls(this);
                Comm.SetControlColours(this);

                tlpMain.BackColor = Color.FromArgb(249, 246, 238);
                this.BackColor = Color.Black;
                this.Padding = new Padding(1);

                //Comm.LoadBGImage(this, picBackground);

                //lblHeading.Font = new Font("Tahoma", 21, FontStyle.Regular, GraphicsUnit.Pixel);

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

            frmMDI form = (frmMDI)MDIParent;
            this.MdiParent = form;
            //int l = form.ClientSize.Width - 10; //(this.MdiParent.ClientSize.Width - this.Width) / 2;
            //int t = form.ClientSize.Height - 80; //((this.MdiParent.ClientSize.Height - this.Height) / 2) - 30;
            //this.SetBounds(5, 0, l, t);

            //bFromEditSales = bFromEdit;
            //iIDFromEditWindow = iTransID;
            //vchtypeID = iVchTpeId;

            SettingsTab = tpgSettings;
            //tabControl1.TabPages.Remove(tpgSettings);

            dgvPreview.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.DisplayedCells);

            try
            {
                mVchtypeID = VchtypeID;
                mReportName = SchemeName;
                mInvID = InvID;

                btnPrintFromPreview.Enabled = false;
                btnTestPrint.Enabled = false;


                //loadcontrol(cmbVoucherType, "select VchTypeID,VchType from tblvchtype where ParentID in(2,6,16,20,41,1005,13) order by SortOrder");

                Common Comm1 = new Common();

                cmbVoucherType.DisplayMember = "VchType";
                cmbVoucherType.ValueMember = "VchTypeID";
                cmbVoucherType.DataSource = Comm1.fnGetData("Select VchTypeID,VchType from tblvchtype where ParentID in(2,6,16,20,41,1005,13) order by SortOrder").Tables[0];

                if ((rdbSearchSearchInvoice.Checked == true))
                    txtSearchBarcode.Enabled = false;
                else if ((rdbSearchBarcode.Checked == true))
                {
                    cmbVoucherType.Enabled = false;
                    txtInvoiceNumber.Enabled = false;
                }

                // cmbPrintScheme.AddItem("<NONE>", 0, 0)
                //cmbPrintScheme.DataSource = Comm1.fnGetData("SELECT  ReportID, ReportName, ReportName as SchemeName FROM tblReportXML where vchtypeID=" + mVchtypeID + " and isnull(isbarcode,0)=1 order by ReportName").Tables[0];
                
                cmbPrintScheme.DisplayMember = "ReportName";
                cmbPrintScheme.ValueMember = "ReportID";

                LoadPrintSchemes();

                int i;
                string pkInstalledPrinters;
                PrinterSettings p;
                int DefaultPrinterIndex = -1;

                for (i = 0; i <= PrinterSettings.InstalledPrinters.Count - 1; i++)
                {
                    pkInstalledPrinters = PrinterSettings.InstalledPrinters[i].ToString();
                    cmbInstalledPrinters.Items.Add(pkInstalledPrinters);

                    p = new PrinterSettings();
                    p.PrinterName = pkInstalledPrinters;
                    if (p.IsDefaultPrinter)
                        DefaultPrinterIndex = cmbInstalledPrinters.Items.Count - 1;
                }

                string MySavedPrinterName = MyPrinterName;
                if (cmbInstalledPrinters.Items.Count > 0 & DefaultPrinterIndex >= 0)
                {
                    MySavedPrinterName = MyPrinterName;
                    cmbInstalledPrinters.SelectedIndex = DefaultPrinterIndex;
                    MyPrinterName = MySavedPrinterName;
                }
                if (MyPrinterName != "")
                {
                    //cmbInstalledPrinters.SelectedItem = MyPrinterName;
                    for (int j = 0; j < cmbInstalledPrinters.Items.Count; j++)
                    {
                        if (cmbInstalledPrinters.Items[j].ToString() == MyPrinterName)
                        {
                            cmbInstalledPrinters.SelectedIndex = j;
                        }
                    }
                }

                //loadcontrol(cmbPrintScheme, "SELECT  ReportID, ReportName, ReportName as SchemeName FROM tblReportXML where vchtypeID=" + mVchtypeID + " and isnull(isbarcode,0)=1 order by ReportName", false, false, false, false);
                cmbPrintScheme.SelectedText = mReportName;

                if (InvID > 0)
                {
                    rdbSearchSearchInvoice.Checked = true;
                    cmbVoucherType.SelectedValue = VchtypeID;
                    txtInvoiceNumber.Text = InvNo;
                    txtInvoiceNumber.Tag = InvID;
                    // txtInvoiceNumber_TButtonClick(txtInvoiceNumber, New System.EventArgs)
                    //SearchInvoice(false);
                }

                // If SchemeName <> "" Then
                rs.Close();
                if (SchemeName == "")
                {
                    rs.Open("SELECT        dbo.tblReportXML.ReportName From dbo.tblVchType INNER Join               dbo.tblReportXML ON dbo.tblVchType.DEFPRINTID = dbo.tblReportXML.ReportID  Where (dbo.tblVchType.VchTypeID = 2)");
                    if (!rs.eof())
                        SchemeName = rs.fields("ReportName");
                }
                rs.Open("Select * from tblReportXml Where ReportName='" + SchemeName + "' and VchtypeID=" + VchtypeID + " and isnull(isbarcode,0)=1");
                if (rs.eof()) //If no schemes are saved for selected vchtype then scheme from purchase will be opened if present
                    rs.Open("Select * from tblReportXml Where ReportName='" + SchemeName + "' and VchtypeID=2 and isnull(isbarcode,0)=1");
                if (!rs.eof())
                {
                    txtBarcodeString.Text = rs.fields("DesignData");
                    // If txtBarcodeString.Text.Length > 0 Then If txtBarcodeString.Text.Substring(txtBarcodeString.Text.Length - 1, 1) <> vbLf Then txtBarcodeString.Text = txtBarcodeString.Text & vbLf
                    txtLabelsPerRow.Text = rs.fields("noofitems");
                    cmbPrintScheme.SelectedText = rs.fields("ReportName");
                    txtEncKey.Text = rs.fields("ReportData");

                    if (Convert.ToInt32(rs.fields("EncryptDecimals")) == 0)
                        chkEncryptDecimals.Checked = false;
                    else
                        chkEncryptDecimals.Checked = true;


                    if (rs.fields("PrinterName") != "")
                    {
                        MyPrinterName = rs.fields("PrinterName");
                        cmbInstalledPrinters.SelectedText = MyPrinterName;

                        for (int j = 0; j < cmbInstalledPrinters.Items.Count; j++)
                        {
                            if(cmbInstalledPrinters.Items[j].ToString() == MyPrinterName)
                            {
                                cmbInstalledPrinters.SelectedIndex = j;
                            }
                        }

                    }

                    btnPrintFromPreview.Enabled = true;
                    btnTestPrint.Enabled = true;
                }
                // End If
                FNLISTING();

                trvBarcodeTags.Nodes.Add("<InvNo>", "<InvNo>");
                trvBarcodeTags.Nodes.Add("<SINo>", "<SINo>");
                trvBarcodeTags.Nodes.Add("<SerialNO>", "<SerialNO>");
                trvBarcodeTags.Nodes.Add("<BatchCode>", "<BatchCode>");
                trvBarcodeTags.Nodes.Add("<CostRate>", "<CostRate>");
                trvBarcodeTags.Nodes.Add("<ItemName>", "<ItemName>");
                trvBarcodeTags.Nodes.Add("<ItemCode>", "<ItemCode>");
                trvBarcodeTags.Nodes.Add("<ItemNameUniCode>", "<ItemNameUniCode>");
                trvBarcodeTags.Nodes.Add("<ItemCodeUniCode>", "<ItemCodeUniCode>");
                trvBarcodeTags.Nodes.Add("<Category>", "<Category>");
                trvBarcodeTags.Nodes.Add("<Manufacturer>", "<Manufacturer>");
                trvBarcodeTags.Nodes.Add("<ProductGroup>", "<ProductGroup>");
                trvBarcodeTags.Nodes.Add("<Type>", "<ProductType>");
                trvBarcodeTags.Nodes.Add("<HSNCODE>", "<HSNCODE>");
                trvBarcodeTags.Nodes.Add("<PackedDate>", "<PackedDate>");
                trvBarcodeTags.Nodes.Add("<PartNumber>", "<PartNumber>");
                trvBarcodeTags.Nodes.Add("<UPC>", "<UPC>");
                trvBarcodeTags.Nodes.Add("<Rack>", "<Rack>");
                trvBarcodeTags.Nodes.Add("<Unit>", "<Unit>");
                trvBarcodeTags.Nodes.Add("<Description>", "<Description>");
                trvBarcodeTags.Nodes.Add("<Notes>", "<Notes>");

                trvBarcodeTags.Nodes.Add("<InvDate>", "<InvDate>");
                trvBarcodeTags.Nodes.Add("<InvDateDDMMMYY>", "<InvDateDDMMMYY>");
                trvBarcodeTags.Nodes.Add("<InvDateDDMMMYYYY>", "<InvDateDDMMMYYYY>");
                trvBarcodeTags.Nodes.Add("<InvDateDDMMYY>", "<InvDateDDMMYY>");
                trvBarcodeTags.Nodes.Add("<InvDateMMYY>", "<InvDateMMYY>");
                trvBarcodeTags.Nodes.Add("<InvDateMMYYYY>", "<InvDateMMYYYY>");
                trvBarcodeTags.Nodes.Add("<InvDateMMMYY>", "<InvDateMMMYY>");
                trvBarcodeTags.Nodes.Add("<InvDateMMMYYYY>", "<InvDateMMMYYYY>");

                trvBarcodeTags.Nodes.Add("<PackedDate>", "<PackedDate>");
                trvBarcodeTags.Nodes.Add("<PackedDateDDMMMYY>", "<PackedDateDDMMMYY>");
                trvBarcodeTags.Nodes.Add("<PackedDateDDMMMYYYY>", "<PackedDateDDMMMYYYY>");
                trvBarcodeTags.Nodes.Add("<PackedDateDDMMYY>", "<PackedDateDDMMYY>");
                trvBarcodeTags.Nodes.Add("<PackedDateMMYY>", "<PackedDateMMYY>");
                trvBarcodeTags.Nodes.Add("<PackedDateMMYYYY>", "<PackedDateMMYYYY>");
                trvBarcodeTags.Nodes.Add("<PackedDateMMMYY>", "<PackedDateMMMYY>");
                trvBarcodeTags.Nodes.Add("<PackedDateMMMYYYY>", "<PackedDateMMMYYYY>");

                trvBarcodeTags.Nodes.Add("<Party>", "<Party>");
                trvBarcodeTags.Nodes.Add("<PartyName>", "<PartyName>");
                trvBarcodeTags.Nodes.Add("<PartyNotes>", "<PartyNotes>");
                // trvBarcodeTags.Nodes.Add("<Qty>", "<Qty>")
                trvBarcodeTags.Nodes.Add("<Expiry>", "<Expiry>");
                trvBarcodeTags.Nodes.Add("<Prate>", "<Prate>");
                trvBarcodeTags.Nodes.Add("<Prate_ENC>", "<Prate_ENC>");
                trvBarcodeTags.Nodes.Add("<Srate>", "<Srate>");
                trvBarcodeTags.Nodes.Add("<Crate>", "<Crate>");
                trvBarcodeTags.Nodes.Add("<Srate_ENC>", "<Srate_ENC>");
                trvBarcodeTags.Nodes.Add("<CRate_ENC>", "<CRate_ENC>");
                trvBarcodeTags.Nodes.Add("<MRP>", "<MRP>");
                trvBarcodeTags.Nodes.Add("<Srate1>", "<Srate1>");
                trvBarcodeTags.Nodes.Add("<Srate2", "<Srate2>");
                trvBarcodeTags.Nodes.Add("<Srate3", "<Srate3>");
                trvBarcodeTags.Nodes.Add("<Srate4", "<Srate4>");
                trvBarcodeTags.Nodes.Add("<Srate5", "<Srate5>");

                // InvNo,SINo,SerialNO,BatchCode,CostRate,ItemName,ItemCode,Category,Manufacturer,ProductGroup,Type,HSNCODE,PartNumber,UPC,Rack,Unit,Description,InvDate,InvDateMY,Party,PartyName,PartyNotes,Qty,Expiry,Prate,PrateENC,Srate,Crate,SrateENC,CRateEnc,MRP,Srate1,Srate2,Srate3,Srate4,Srate5
                // BindSubwindow()

                dgvBarcodeDetails.Rows.Clear();
                dgvBarcodeDetails.Columns.Clear();
                dgvBarcodeDetails.Columns.Add("InvNo", "InvNo");
                dgvBarcodeDetails.Columns.Add("InvDate", "InvDate");
                dgvBarcodeDetails.Columns.Add("Party", "Party");
                dgvBarcodeDetails.Columns.Add("LAliasName", "LAliasName");
                dgvBarcodeDetails.Columns.Add("LName", "LName");
                dgvBarcodeDetails.Columns.Add("PartyCode", "PartyCode");
                dgvBarcodeDetails.Columns.Add("MobileNo", "MobileNo");
                dgvBarcodeDetails.Columns.Add("ItemCode", "ItemCode");
                dgvBarcodeDetails.Columns.Add("ItemName", "ItemName");
                dgvBarcodeDetails.Columns.Add("Qty", "Qty");
                dgvBarcodeDetails.Columns.Add("PrintQty", "PrintQty");
                dgvBarcodeDetails.Columns.Add("PackedDate", "PackedDate");
                dgvBarcodeDetails.Columns.Add("HsnCode", "HsnCode");
                //dgvBarcodeDetails.Columns.Add("PGName", "PGName");
                dgvBarcodeDetails.Columns.Add("Category", "Category");
                dgvBarcodeDetails.Columns.Add("MnfName", "MnfName");
                dgvBarcodeDetails.Columns.Add("BatchCode", "BatchCode");
                dgvBarcodeDetails.Columns.Add("Unit", "Unit");
                dgvBarcodeDetails.Columns.Add("BatchUnique", "BatchUnique");
                dgvBarcodeDetails.Columns.Add("MRP", "MRP");
                dgvBarcodeDetails.Columns.Add("Prate", "Prate");
                dgvBarcodeDetails.Columns.Add("Crate", "Crate");
                dgvBarcodeDetails.Columns.Add("Srate1", "Srate1");
                dgvBarcodeDetails.Columns.Add("Srate2", "Srate2");
                dgvBarcodeDetails.Columns.Add("Srate3", "Srate3");
                dgvBarcodeDetails.Columns.Add("Srate4", "Srate4");
                dgvBarcodeDetails.Columns.Add("Srate5", "Srate5");
                dgvBarcodeDetails.Columns.Add("Expiry", "Expiry");
                dgvBarcodeDetails.Columns.Add("Description", "Description");
                dgvBarcodeDetails.Columns.Add("ROL", "ROL");
                dgvBarcodeDetails.Columns.Add("Rack", "Rack");
                dgvBarcodeDetails.Columns.Add("Notes", "Notes");
                dgvBarcodeDetails.Columns.Add("PLUNo", "PLUNo");
                dgvBarcodeDetails.Columns.Add("ItemNameUniCode", "ItemNameUniCode");
                dgvBarcodeDetails.Columns.Add("ItemCodeUniCode", "ItemCodeUniCode");
                dgvBarcodeDetails.Columns.Add("UPC", "UPC");

                dgvBarcodeDetails.AllowUserToAddRows = false;

                //txtSearchBarcode.SuppressNavigation = true;
                ShowSubWindow("");
                panelsearch.Visible = false;

                FNLISTING();


            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        } 

        private void ClearControl()
        {
            try
            {
                txtSearchBarcode.Text = "";
                txtInvoiceNumber.Text = "";

                // dgvSubMenu.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None

                // If Conversion.Val(cmbVoucherType.SelectedIndex) < 0 Then
                // Exit Sub
                // End If

                string MyQry = "dbo.tblPurchase";

                int ParentID = Convert.ToInt32(Comm.GetTableValue("tblVchtype", "ParentID", " Where VchtypeID=" + Conversion.Val(cmbVoucherType.SelectedValue.ToString())));
                switch (ParentID) // cmbVoucherType.SelectedValue
                {
                    case 2:
                        {
                            MyQry = " Select INVID,invno from dbo.tblPurchase where cancelled=0 and blnHOLD=0 and vchtypeid=" + cmbVoucherType.SelectedValue + " Order by invno  ";
                            break;
                        }

                    case 20:
                        {
                            MyQry = " Select INVID,invno from dbo.tblRepacking where cancelled=0 and blnHOLD=0 and vchtypeid=" + cmbVoucherType.SelectedValue + " Order by invno  ";
                            break;
                        }

                    case 1005:
                        {
                            MyQry = " Select INVID,invno from dbo.tblPurchase where cancelled=0 and blnHOLD=0 and vchtypeid=" + cmbVoucherType.SelectedValue + " Order by invno  ";
                            break;
                        }

                    default:
                        {
                            MyQry = " Select INVID,invno from dbo.tblStockJournal where cancelled=0 and blnHOLD=0 Order by invno  ";
                            break;
                        }
                }

                txtInvoiceNumber.DisplayMember = "invno";
                txtInvoiceNumber.ValueMember = "INVID";
                txtInvoiceNumber.DataSource = Comm.fnGetData(MyQry).Tables[0];
                
                //loadcontrol(txtInvoiceNumber, MyQry);
            }

            // dgvBarcodeDetails.DataSource = Nothing
            // lblBarcodeDetails.Visible = False
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void FNLISTING()
        {
            try
            {
                // If rdbSearchSearchInvoice.Tag = "" Then
                // BindSubwindow()
                // rdbSearchSearchInvoice.Tag = "loaded"
                // End If

                if ((rdbSearchBarcode.Checked == true))
                {
                    cmbVoucherType.Enabled = false;
                    txtInvoiceNumber.Enabled = false;
                    btnFill.Enabled = false;
                }
                else
                {
                    if (cmbVoucherType.SelectedIndex < 0)
                        return;
                    string MyQry = "dbo.tblPurchase";

                        Common Comm1 = new Common();

                        int ParentID = Convert.ToInt32(Comm1.GetTableValue("tblVchtype", "ParentID", " Where VchtypeID=" + Conversion.Val(cmbVoucherType.SelectedValue)));
                    switch (ParentID) // cmbVoucherType.SelectedValue
                    {
                        case 2:
                            {
                                MyQry = " Select INVID,invno from dbo.tblPurchase where cancelled=0 and blnHOLD=0 Order by invno  ";
                                break;
                            }

                        case 1005:
                            {
                                MyQry = " Select INVID,invno from dbo.tblPurchase where cancelled=0 and blnHOLD=0 Order by invno  ";
                                break;
                            }

                        case 20:
                            {
                                MyQry = " Select INVID,invno from dbo.tblRepacking where cancelled=0 and blnHOLD=0 Order by invno  ";
                                break;
                            }

                        default:
                            {
                                MyQry = " Select INVID,invno from dbo.tblStockJournal where cancelled=0 and blnHOLD=0 Order by invno  ";
                                break;
                            }
                    }
                    // loadcontrol(txtInvoiceNumber, MyQry)
                    dgvBarcodeDetails.DataSource = null;
                    lblBarcodeDetails.Visible = false;

                    cmbVoucherType.Enabled = true;
                    txtInvoiceNumber.Enabled = true;
                    btnFill.Enabled = true;
                }

                if ((rdbSearchSearchInvoice.Checked == true))
                    txtSearchBarcode.Enabled = false;
                else
                    txtSearchBarcode.Enabled = true;

                ClearControl();
                // ClearPreviewAndDisablePrint()
                dgvPreview.Rows.Clear();
                dgvPreview.Columns.Clear();
            }
            catch (Exception ex)
            {
                Interaction.MsgBox(ex.Message, MsgBoxStyle.Information);
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
        bool bFromEditSales;
        decimal dCostRateInc = 0, dCostRateExcl = 0, dPRateIncl = 0, dPRateExcl = 0;
        decimal dSteadyBillDiscPerc, dSteadyBillDiscAmt;

        //static int namesCount = Enum.GetNames(typeof(LedgerIndexes)).Length;
        //string[] sArrLedger = new string[namesCount];
        Common Comm = new Common();


        #endregion

        #region "EVENTS ------------------------------------------------ >>"

        private void frmStockVoucher_Load(object sender, EventArgs e)
        {
            try
            {
                if (rdbSearchSearchInvoice.Tag == null) rdbSearchSearchInvoice.Tag = "";
                if (rdbSearchSearchInvoice.Tag.ToString() == "")
                {
                    BindSubwindow();
                    //Application.DoEvents()
                    rdbSearchSearchInvoice.Tag = "loaded";
                }

                if (mInvID > 0)
                {
                    txtInvoiceNumber.SelectedValue = mInvID;
                    SearchInvoice(false);
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
            DialogResult dlgResult = MessageBox.Show("Do you want to exit?", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            if (dlgResult.Equals(DialogResult.Yes))
                this.Close();
        }

        private void txtSupplier_TextChanged(object sender, EventArgs e)
        {

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

        }

        private void txtTaxRegn_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void txtAddress1_KeyDown(object sender, KeyEventArgs e)
        {

        }


        ComboBox BatchCode_GridCellComboBox = new ComboBox();

        private void gridColumn_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void dgvBarcodeDetails_TextBox_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void dtp_TextChange(object sender, EventArgs e)
        {

        }

        private void cboPayment_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void cboTaxMode_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void cboCostCentre_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void cboSalesStaff_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void txtDiscPerc_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtRoundOff_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void txtDiscAmt_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtOtherExp_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtCostFactor_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtCashDisc_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtRoundOff_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtDiscAmt_KeyDown_1(object sender, KeyEventArgs e)
        {

        }

        private void txtOtherExp_KeyDown_1(object sender, KeyEventArgs e)
        {

        }

        private void txtNarration_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void cboAgent_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void btnSave_Click(object sender, EventArgs e)
        {

        }

        private void txtSupplier_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void cboState_KeyDown(object sender, KeyEventArgs e)
        {
        }

        private void txtInstantReceipt_KeyDown(object sender, KeyEventArgs e)
        {
        }

        private void txtRoundOff_KeyPress(object sender, KeyPressEventArgs e)
        {
        }

        private void txtcashDisper_KeyPress(object sender, KeyPressEventArgs e)
        {
        }

        private void txtcashDisper_KeyDown(object sender, KeyEventArgs e)
        {
        }

        private void txtDiscPerc_KeyDown(object sender, KeyEventArgs e)
        {
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




        private void btnSettings_Click(object sender, EventArgs e)
        {
            frmVouchertype frmV = new frmVouchertype(vchtypeID, false, true);
            frmV.StartPosition = FormStartPosition.CenterScreen;
            frmV.ShowDialog();
        }


        private void btnDelete_Click(object sender, EventArgs e)
        {

        }

        private void frmStockInVoucherNew_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                if (e.KeyCode == Keys.Escape)
                {
                    btnClose_Click(new object(), new EventArgs());
                    //this.Close();
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

        }


        private void btnArchive_Click(object sender, EventArgs e)
        {
        }


        private void btnPause_Click(object sender, EventArgs e)
        {

        }

        private void txtInvoiceNumber_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void txtInvoiceNumber_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return)
                SearchInvoice(false);
        }

        private void SearchInvoice(bool blnSingleBarcode)
        {
            try
            {
                string strSelectFields = "MasterTable.InvNo, MasterTable.InvDate, MasterTable.Party, dbo.tblLedger.LAliasName, dbo.tblLedger.LName, MasterTable.partyCode, MasterTable.MobileNo, ";
                if (blnSingleBarcode)
                    strSelectFields = "'' as InvNo, '' as InvDate, '' as Party, '' as LAliasName, '' as LName, '' as partyCode, '' as MobileNo, ";
                string strCondition = "MasterTable.InvNo='" + txtInvoiceNumber.Text.ToString() + "' and vchtypeid=" + cmbVoucherType.SelectedValue;
                if (blnSingleBarcode == true)
                    strCondition = " tblStock.BatchCode='" + txtSearchBarcode.Text.Trim() + "'";
                string MasterTable = "dbo.tblPurchase";
                string DetailTable = "dbo.tblPurchaseItem";
                if (blnSingleBarcode == false)
                    dgvBarcodeDetails.Rows.Clear();
                
                var ParentID = Conversion.Val(Comm.GetTableValue("tblVchtype", "ParentID", " Where VchtypeID=" + Conversion.Val(cmbVoucherType.SelectedValue)));
                switch (ParentID) // cmbVoucherType.SelectedValue
                {
                    case 2:
                        {
                            MasterTable = "dbo.tblPurchase";
                            DetailTable = "dbo.tblPurchaseItem";
                            break;
                        }

                    case 1005:
                        {
                            MasterTable = "dbo.tblPurchase";
                            DetailTable = "dbo.tblPurchaseItem";
                            break;
                        }

                    case 20:
                        {
                            MasterTable = "dbo.tblRepacking";
                            DetailTable = "dbo.tblRepackingItem";
                            break;
                        }

                    default:
                        {
                            MasterTable = "dbo.tblStockJournal";
                            DetailTable = "dbo.tblStockJournalItem";
                            break;
                        }
                }

                string SQL;
                if (blnSingleBarcode)
                    SQL = "SELECT  '' as InvNo, '' as InvDate, '' as Party, '' as LAliasName, '' as LName, '' as partyCode, '' as MobileNo, dbo.tblItemMaster.ItemCode, dbo.tblItemMaster.ItemName, dbo.tblStock.QOH as Qty, dbo.tblStock.QOH as PrintQty,GetDate() as PackedDate , tblItemMaster.hsnid as HSNCODE, dbo.tblCategories.Category, dbo.tblManufacturer.MnfName, dbo.tblStock.BatchCode, UnitName as Unit, dbo.tblStock.BatchUnique, " + Comm.FormatSQL(" dbo.tblStock.MRP") + " as MRP, dbo.tblStock.Prate, dbo.tblStock.CostRateExcl as Crate," + Comm.FormatSQL(" dbo.tblStock.Srate1") + " as Srate1, dbo.tblStock.Srate2, dbo.tblStock.Srate3, dbo.tblStock.Srate4, dbo.tblStock.Srate5, case dbo.tblItemMaster.BlnExpiry when 0 then 'NA' else cast(dbo.tblStock.Expirydate as varchar) end as Expiry, dbo.tblItemMaster.Description, dbo.tblItemMaster.ROL, dbo.tblItemMaster.Rack, dbo.tblItemMaster.Notes, dbo.tblItemMaster.PLUNo, dbo.tblItemMaster.ItemNameUniCode, dbo.tblItemMaster.ItemCodeUniCode, dbo.tblItemMaster.UPC " + "FROM            dbo.tblItemMaster INNER JOIN " + "     dbo.tblStock ON dbo.tblItemMaster.ItemID = dbo.tblStock.ItemID  LEFT OUTER JOIN dbo.tblUnit ON dbo.tblItemMaster.UnitID = dbo.tblUnit.UnitID  inner JOIN " + "     dbo.tblManufacturer ON dbo.tblItemMaster.MNFID = dbo.tblManufacturer.MnfID  INNER JOIN " + "     dbo.tblCategories ON dbo.tblItemMaster.CategoryID = dbo.tblCategories.CategoryID " + " WHERE           tblItemMaster.activestatus=1 and isnull(StockActiveStatus,1)=1 and tblStock.BatchCode='" + txtSearchBarcode.Text.Trim() + "' ";
                else if (Convert.ToInt32(cmbVoucherType.SelectedValue) != 20)
                    SQL = "SELECT        MasterTable.InvNo, MasterTable.InvDate, MasterTable.Party, dbo.tblLedger.LAliasName, dbo.tblLedger.LName, MasterTable.partyCode, MasterTable.MobileNo, dbo.tblItemMaster.ItemCode, dbo.tblItemMaster.ItemName,DetailTable.Qty, DetailTable.Qty as PrintQty,GetDate() as PackedDate, tblItemMaster.hsnid as HSNCODE, dbo.tblCategories.Category, dbo.tblManufacturer.MnfName, DetailTable.BatchCode, UnitName as Unit, dbo.tblStock.BatchUnique, " + Comm.FormatSQL(" dbo.tblStock.MRP") + " as MRP, dbo.tblStock.Prate, dbo.tblStock.CostRateExcl as Crate, " + Comm.FormatSQL(" dbo.tblStock.Srate1") + " as Srate1, dbo.tblStock.Srate2, dbo.tblStock.Srate3, dbo.tblStock.Srate4, dbo.tblStock.Srate5, case dbo.tblItemMaster.BlnExpiry when 0 then 'NA' else cast(dbo.tblStock.Expirydate as varchar) end as Expiry, dbo.tblItemMaster.Description, dbo.tblItemMaster.ROL, dbo.tblItemMaster.Rack, dbo.tblItemMaster.Notes, dbo.tblItemMaster.PLUNo, dbo.tblItemMaster.ItemNameUniCode, dbo.tblItemMaster.ItemCodeUniCode, dbo.tblItemMaster.UPC, DetailTable.SlNo " + "FROM            " + MasterTable + " as MasterTable INNER JOIN " + DetailTable + " DetailTable ON MasterTable.InvId = DetailTable.InvID INNER JOIN   dbo.tblItemMaster ON DetailTable.ItemId = dbo.tblItemMaster.ItemID INNER JOIN " + "     dbo.tblStock ON dbo.tblItemMaster.ItemID = dbo.tblStock.ItemID AND DetailTable.BatchUnique = dbo.tblStock.BatchUnique INNER JOIN tblUnit ON tblItemMaster.UNITID=tblUnit.UNITID INNER JOIN " + "     dbo.tblManufacturer ON dbo.tblItemMaster.MNFID = dbo.tblManufacturer.MnfID INNER JOIN " + "     dbo.tblCategories ON dbo.tblItemMaster.CategoryID = dbo.tblCategories.CategoryID LEFT OUTER JOIN " + "     dbo.tblLedger ON MasterTable.LedgerId = dbo.tblLedger.LID " + "WHERE           tblItemMaster.activestatus=1 and isnull(StockActiveStatus,1)=1 and MasterTable.InvNo='" + txtInvoiceNumber.Text.ToString() + "' and vchtypeid=" + cmbVoucherType.SelectedValue + " Order By DetailTable.SlNo ";
                else
                    SQL = "SELECT        MasterTable.InvNo, MasterTable.InvDate, '' as Party, '' as LAliasName, '' as LName, '' as partyCode, '' as MobileNo, dbo.tblItemMaster.ItemCode, dbo.tblItemMaster.ItemName, dbo.tblStock.QOH as Qty, dbo.tblStock.QOH as PrintQty,GetDate() as PackedDate, tblItemMaster.hsnid as HSNCODE, dbo.tblCategories.Category, dbo.tblManufacturer.MnfName, DetailTable.BatchCode, UnitName as Unit, dbo.tblStock.BatchUnique, " + Comm.FormatSQL(" dbo.tblStock.MRP") + " as MRP, dbo.tblStock.Prate, dbo.tblStock.CostRateExcl as Crate, " + Comm.FormatSQL(" dbo.tblStock.Srate1") + " as Srate1, dbo.tblStock.Srate2, dbo.tblStock.Srate3, dbo.tblStock.Srate4, dbo.tblStock.Srate5, case dbo.tblItemMaster.BlnExpiry when 0 then 'NA' else cast(dbo.tblStock.Expirydate as varchar) end as Expiry, dbo.tblItemMaster.Description, dbo.tblItemMaster.ROL, dbo.tblItemMaster.Rack, dbo.tblItemMaster.Notes, dbo.tblItemMaster.PLUNo, dbo.tblItemMaster.ItemNameUniCode, dbo.tblItemMaster.ItemCodeUniCode, dbo.tblItemMaster.UPC, DetailTable.SlNo " + "FROM            " + MasterTable + " as MasterTable INNER JOIN " + DetailTable + " DetailTable ON MasterTable.InvId = DetailTable.InvID INNER JOIN   dbo.tblItemMaster ON DetailTable.ItemId = dbo.tblItemMaster.ItemID INNER JOIN " + "     dbo.tblStock ON dbo.tblItemMaster.ItemID = dbo.tblStock.ItemID AND DetailTable.BatchUnique = dbo.tblStock.BatchUnique INNER JOIN tblUnit ON tblItemMaster.UNITID=tblUnit.UNITID INNER JOIN " + "     dbo.tblManufacturer ON dbo.tblItemMaster.MNFID = dbo.tblManufacturer.MnfID INNER JOIN " + "     dbo.tblCategories ON dbo.tblItemMaster.CategoryID = dbo.tblCategories.CategoryID " + " WHERE     tblItemMaster.activestatus=1 and isnull(StockActiveStatus,1)=1 and DetailTable.intItemType in (0,2)  and  MasterTable.InvNo='" + txtInvoiceNumber.Text.ToString() + "' and vchtypeid=" + cmbVoucherType.SelectedValue + " Order By DetailTable.slno ";

                rs.Open(SQL);

                foreach (DataGridViewColumn c1 in dgvBarcodeDetails.Columns)
                {
                    if (c1.Name.ToUpper() != "PRINTQTY")
                        c1.ReadOnly = true;
                    dgvBarcodeDetails.AutoResizeColumn(c1.Index);
                }

                DataGridViewColumn c;

                while (!rs.eof())
                {
                    dgvBarcodeDetails.Rows.Add();
                    for (var i = 0; i <= dgvBarcodeDetails.Columns.Count - 1; i++)
                    {
                        c = dgvBarcodeDetails.Columns[i];
                        for (var j = 0; j <= rs.sqlDT.Columns.Count - 1; j++)
                        {
                            // If rs.sqlDT.Columns(j).ColumnName.ToUpper = "EXPIRYDATE" Then
                            // MsgBox(rs.sqlDT.Columns(j).ColumnName.ToUpper)
                            // End If
                            if (c.Name.ToUpper() == rs.sqlDT.Columns[j].ColumnName.ToUpper())
                            {
                                dgvBarcodeDetails.Rows[dgvBarcodeDetails.RowCount - 1].Cells[j].Value = rs.fields(rs.sqlDT.Columns[j].ColumnName);
                                break;
                            }
                        }
                    }
                    rs.MoveNext();
                }

                if (blnSingleBarcode)
                {
                    dgvBarcodeDetails.Columns[0].Visible = false;
                    dgvBarcodeDetails.Columns[1].Visible = false;
                    dgvBarcodeDetails.Columns[2].Visible = false;
                    dgvBarcodeDetails.Columns[3].Visible = false;
                    dgvBarcodeDetails.Columns[4].Visible = false;
                    dgvBarcodeDetails.Columns[5].Visible = false;
                    dgvBarcodeDetails.Columns[6].Visible = false;
                }

                lblBarcodeDetails.Visible = true;
                dgvBarcodeDetails.Columns["PackedDate"].ReadOnly = false;
                dgvBarcodeDetails.Columns["Description"].ReadOnly = false;
            }
            catch (Exception ex)
            {
                Interaction.MsgBox(ex.Message, MsgBoxStyle.Information);
            }
        }

        private void txtSearchBarcode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    panelsearch.Visible = false;
                    if (dgvSubMenu.Rows.Count == 0)
                        return;
                    if (dgvSubMenu.SelectedRows.Count == 0)
                        dgvSubMenu.Rows[0].Selected = true;
                    if (dgvSubMenu.SelectedRows[0].Cells[0].Value.ToString() != "")
                    {
                        txtSearchBarcode.Tag = dgvSubMenu.SelectedRows[0].Cells[0].Value.ToString();
                        if (FillItem("", 0, Convert.ToInt32(txtSearchBarcode.Tag), "") == true)
                            panelsearch.Visible = false;
                        panelsearch.Visible = false;
                    }
                    txtSearchBarcode.Text = "";
                }
                if (e.KeyValue == (int)Keys.Down | e.KeyValue == (int)Keys.Up)
                {
                    try
                    {
                        if (panelsearch.IsDisposed)
                            panelsearch.Show();
                        panelsearch.Visible = true;
                        panelsearch.BringToFront();
                        Application.DoEvents();
                    }
                    catch (Exception ex)
                    {
                    }
                    try
                    {
                        panelsearch.BringToFront();
                        Application.DoEvents();
                    }
                    catch (Exception ex)
                    {
                    }
                }
                int MyDesiredIndex = 0;

                if (e.KeyValue == ((int)Keys.Down))
                {
                    if (dgvSubMenu.SelectedRows[0].Index < dgvSubMenu.RowCount - 1)
                        MyDesiredIndex = dgvSubMenu.SelectedRows[0].Index + 1;
                    dgvSubMenu.ClearSelection();
                    if (MyDesiredIndex > -1)
                        dgvSubMenu.Rows[MyDesiredIndex].Selected = true;
                    e.Handled = true;
                }
                else if (e.KeyValue == ((int)Keys.Up))
                {
                    if (dgvSubMenu.SelectedRows[0].Index < dgvSubMenu.RowCount - 1)
                        MyDesiredIndex = dgvSubMenu.SelectedRows[0].Index - 1;
                    if (MyDesiredIndex < 0)
                        MyDesiredIndex = 0;
                    dgvSubMenu.ClearSelection();
                    if (MyDesiredIndex > -1)
                        dgvSubMenu.Rows[MyDesiredIndex].Selected = true;
                    e.Handled = true;
                }
                lblSelected.Text = dgvSubMenu.SelectedRows[0].Cells[1].Value.ToString();
                dgvSubMenu.Columns["AnyWhere"].Visible = false;
                dgvSubMenu.Columns["ItemID"].Visible = false;
                dgvSubMenu.Columns["StockID"].Visible = false;
                // dgvSubMenu.AutoSizeColumnsMode = DataGridViewAutoSizeColumnMode.Fill

                txtSearchBarcode.Focus();
            }
            catch (Exception ex)
            {
                string s = ex.Message;
            }
        }

        private void ShowSubWindow(string SearchString)
        {
            try
            {
                if (SearchString != "")
                {
                    string Query = "";
                    string Fields = "";

                    bsdata.Filter(string.Format("ANYWHERE  like  '%{0}%' ", SearchString.ToString().Replace("'", "''")));

                    panelsearch.Visible = true;
                    panelsearch.BringToFront();

                    dgvSubMenu.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                    if (dgvSubMenu.SelectedRows.Count == 0)
                    {
                        if (dgvSubMenu.Rows.Count > 0)
                            dgvSubMenu.Rows[0].Selected = true;
                        else
                        {
                            bsdata.Filter(string.Format("ANYWHERE  like  '%{0}%' ", ""));
                            panelsearch.Visible = true;
                            panelsearch.BringToFront();
                        }
                    }
                }
                dgvSubMenu.Visible = true;
                if (dgvSubMenu.Columns.Count > 2)
                    dgvSubMenu.Columns[0].Visible = false;

                panelsearch.BackColor = dgvSubMenu.AlternatingRowsDefaultCellStyle.BackColor;
            }
            catch (Exception EX)
            {
                Interaction.MsgBox(EX.Message);
            }
        }

        private bool FillItem(string SearchStringItemCode, int ItemID, int StockID, string BatchCode)
        {
            try
            {
                sqlControl rs = new sqlControl();
                if (SearchStringItemCode + StockID.ToString() + ItemID.ToString() + BatchCode.ToString() != "")
                {
                    if (StockID > 0)
                        rs.Open("Select tblStock.ItemID,BatchCode from tblStock, tblItemMaster where tblStock.itemid = tblItemMaster.itemid and tblItemMaster.activestatus=1 and isnull(StockActiveStatus,1)=1 and StockID=" + StockID + " ");
                    else if (ItemID > 0)
                        rs.Open("Select tblStock.ItemID,BatchCode from tblStock, tblItemMaster where tblStock.itemid = tblItemMaster.itemid and tblItemMaster.activestatus=1 and isnull(StockActiveStatus,1)=1 and ItemID=" + ItemID + "");
                    else if (SearchStringItemCode.Trim() != "")
                        rs.Open("Select tblStock.ItemID,BatchCode from tblStock, tblItemMaster where tblStock.itemid = tblItemMaster.itemid and tblItemMaster.activestatus=1 and isnull(StockActiveStatus,1)=1 and ItemCode='" + SearchStringItemCode.ToString() + "'");
                    else if (BatchCode.Trim() != "")
                        rs.Open("Select tblStock.ItemID,BatchCode from tblStock, tblItemMaster where tblStock.itemid = tblItemMaster.itemid and tblItemMaster.activestatus=1 and isnull(StockActiveStatus,1)=1 and BatchCode='" + BatchCode.ToString() + "'");
                    // activestatus=1 and StockActiveStatus=1 and 
                    if (!rs.eof())
                    {
                        txtSearchBarcode.Tag = rs.fields("ItemID");
                        txtSearchBarcode.Text = rs.fields("BatchCode");
                        SearchInvoice(true);
                        panelsearch.Visible = false;
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Interaction.MsgBox(ex.Message);
                return false;
            }
        }

        private void BindSubwindow()
        {
            try
            {
                string strsmartsearchQuery = "";
                strsmartsearchQuery = " select StockID as ID,ItemCode,ItemName,BatchCode,BatchUnique,tblstock.MRP as MRP, (cast(ItemCode as varchar(50)) + cast(ItemName as varchar(50)) + cast(BatchUnique as varchar(50)) + cast(tblstock.MRP as varchar(50))) AS Anywhere,tblStock.ItemId,tblStock.StockID from tblStock, tblItemMaster where tblStock.itemid = tblItemMaster.itemid order by ItemCode,ItemName,tblStock.MRP,tblStock.ItemId,tblStock.StockID";

                //strsmartsearchQuery = strsmartsearchQuery.ToUpper().Replace("select ItemCode".ToUpper(), "Select StockID as ID,ItemCode");

                //strsmartsearchQuery = strsmartsearchQuery.ToUpper().Replace("cast(ItemNameUniCode as varchar(50))".ToUpper(), "ItemNameUniCode");
                //strsmartsearchQuery = strsmartsearchQuery.ToUpper().Replace("CAST(ITEMNAME AS VARCHAR(50))".ToUpper(), "ITEMNAME");

                //if (strsmartsearchQuery.ToUpper().Contains("StockID as".ToUpper()) == true)
                //    strsmartsearchQuery = strsmartsearchQuery.ToUpper().Replace("select ".ToUpper(), "Select StockID as ID,");

                //if (strsmartsearchQuery.ToUpper().Contains("StockID as".ToUpper()) == true)
                //    Interaction.MsgBox("Query not working");

                strsmartsearchQuery = strsmartsearchQuery.Replace("Anywhere", "ANYWHERE");
                bsdata.Open(strsmartsearchQuery);

                if (bsdata.RecordCount == 0)
                {
                    strsmartsearchQuery = " select StockID as ID, ItemCode,ItemName,MRP, (cast(ItemCode as varchar(50)) + cast(ItemName as varchar(50)) + cast(MRP as varchar(50))) AS Anywhere,ItemId,StockID from  vwsmartSearch order by ItemCode,ItemName,MRP,ItemId,StockID";
                    bsdata.Open(strsmartsearchQuery);
                }

                dgvSubMenu.DataSource = bsdata.bsData;

                // dgvSubMenu.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.DisplayedCells)

                for (var i = 1; i <= dgvSubMenu.Columns.Count - 1; i++)
                {
                    if (dgvSubMenu.Columns[i].Name.ToUpper() == "ANYWHERE")
                        dgvSubMenu.Columns[i].Visible = false;
                    else if (dgvSubMenu.Columns[i].Name.ToUpper() == "ITEMID" | dgvSubMenu.Columns[i].Name.ToUpper() == "StockID")
                        dgvSubMenu.Columns[i].Visible = false;
                    else if (dgvSubMenu.Columns[i].Name.ToUpper() == "MRP" | dgvSubMenu.Columns[i].Name.ToUpper() == "QTY")
                    {
                        dgvSubMenu.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                        dgvSubMenu.Columns[i].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                        dgvSubMenu.Columns[i].DefaultCellStyle.BackColor = Color.LightPink;
                    }
                }
            }

            catch (Exception ex)
            {
                Interaction.MsgBox("SubwindowBinding " + ex.Message, MsgBoxStyle.Information);
            }
        }

        private void trvBarcodeTags_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            try
            {
                if (txtBarcodeString.SelectionStart > 0)
                {
                    int CurPos = txtBarcodeString.SelectionStart;
                    txtBarcodeString.Text = txtBarcodeString.Text.Insert(txtBarcodeString.SelectionStart, trvBarcodeTags.SelectedNode.Text);
                    // If txtBarcodeString.Text.Length > 0 Then If txtBarcodeString.Text.Substring(txtBarcodeString.Text.Length - 1, 1) <> vbLf Then txtBarcodeString.Text = txtBarcodeString.Text & vbLf
                    txtBarcodeString.SelectionStart = CurPos + trvBarcodeTags.SelectedNode.Text.Length;
                }
            }
            catch (Exception ex)
            {
                Interaction.MsgBox(ex.Message, MsgBoxStyle.Information);
            }
        }

        private void btnSaveAndExport_Click(object sender, EventArgs e)
        {
            try
            {
                string strFileData = "";

                ClsFileOperation fso = new ClsFileOperation();
                if (System.IO.File.Exists(Application.StartupPath + @"\Resources\Template.ini") == true)
                {
                }

                if (mReportName == "" & cmbPrintScheme.Text.ToString() != "")
                    mReportName = cmbPrintScheme.Text.ToString();
                if (txtBarcodeString.Text.ToUpper().Contains("<SPLIT>") == false)
                {
                    Interaction.MsgBox("An essential tag <SPLIT> not found in the barcode settings. Please place the <SPLIT> tag in appropriate position and continue.", MsgBoxStyle.Critical);
                    return;
                }

                int EncryptDecimals = 0;
                if (chkEncryptDecimals.Checked == true)
                    EncryptDecimals = 1;
                else
                    EncryptDecimals = 0;

                int cnt = 0;
                sqlControl rs1 = new sqlControl();
                if (mReportName == "")
                {
                    while (cnt < 100)
                    {
                        mReportName = "BARCODE" + mVchtypeID.ToString() + "_" + cnt.ToString();
                        rs1 = new sqlControl();
                        rs1.Open("Select ReportName From tblReportXml Where ReportName='" + mReportName + "' and VchtypeID=" + mVchtypeID + " ");
                        if (rs1.eof())
                            break;
                        cnt = cnt + 1;
                    }
                    if (cnt >= 100)
                    {
                        Interaction.MsgBox("A report name could not be assigned. Please provide a report name to continue.", MsgBoxStyle.OkOnly);
                        mReportName = Interaction.InputBox("Please provide a scheme name.", "Scheme name", mReportName);
                    }
                }

                if (txtBarcodeString.Text.Length > 0)
                {
                    if (txtBarcodeString.Text.Substring(txtBarcodeString.Text.Length - 1, 1) != Constants.vbLf)
                        txtBarcodeString.Text = txtBarcodeString.Text + Constants.vbLf;
                }

                string blnTemplateFile = "";
                if (chkFileName.Checked == true)
                    blnTemplateFile = "1";
                else
                    blnTemplateFile = "0";

                if (txtSaveAs.Text.ToString() == "")
                {
                    rs.Close();
                    rs.Execute("Update tblReportXml Set FileData='" + strFileData + "',DesignData=N'" + txtBarcodeString.Text.Replace("'", "''").ToString() + "',ReportData='" + txtEncKey.Text.Replace("'", "''").ToString() + "',NoOfItems=" + Conversion.Val(txtLabelsPerRow.Text) + ",PrinterName='" + MyPrinterName.Replace("'", "''").ToString() + "',isBarcode=1,EncryptDecimals=" + EncryptDecimals + ",CharWidth=" + Conversion.Val(txtCharWidth.Text.ToString()) + ",blnTemplateFile=" + blnTemplateFile + ",TemplateFileName='' Where ReportName='" + mReportName + "' and VchtypeID=" + mVchtypeID + " ");
                    if (rs.RecordCount <= 0)
                        rs.Execute("Insert Into tblReportXml(ReportName,VchtypeID,DesignData,ReportData,NoOfItems,PrinterName,isBarcode,EncryptDecimals,CharWidth,FileData) Values ('" + mReportName + "'," + mVchtypeID + ",'" + txtBarcodeString.Text.Replace("'", "''").ToString() + "','" + txtEncKey.Text.Replace("'", "''").ToString() + "'," + Conversion.Val(txtLabelsPerRow.Text) + ",'" + cmbInstalledPrinters.SelectedText.Replace("'", "''").ToString() + "',1," + EncryptDecimals + "," + Conversion.Val(txtCharWidth.Text.ToString()) + ",'" + strFileData + "')");
                }
                else
                {
                    rs.Close();
                    rs.Execute("Insert Into tblReportXml(ReportName,VchtypeID,DesignData,ReportData,NoOfItems,PrinterName,isBarcode,EncryptDecimals,CharWidth,blnTemplateFile,TemplateFileName,FileData) Values ('" + txtSaveAs.Text.Replace("'", "''").ToString() + "'," + mVchtypeID + ",'" + txtBarcodeString.Text.Replace("'", "''").ToString() + "','" + txtEncKey.Text.Replace("'", "''").ToString() + "'," + Conversion.Val(txtLabelsPerRow.Text) + ",'" + cmbInstalledPrinters.SelectedText.Replace("'", "''").ToString() + "',1," + EncryptDecimals + "," + Conversion.Val(txtCharWidth.Text.ToString()) + "," + blnTemplateFile + ",'','" + strFileData + "')");
                    // blnTemplateFile=,TemplateFileName=
                    if (rs.Exception == "")
                    {
                        LoadPrintSchemes();

                        txtSaveAs.Text = "";
                    }
                }
                if (rs.RecordCount <= 0)
                {
                    Interaction.MsgBox("Could not save barcode details. Run update database or contact vendor for resolving the problem.", MsgBoxStyle.Critical);
                    btnPrintFromPreview.Enabled = false;
                    btnTestPrint.Enabled = false;
                }
                else
                {
                    ExportToGrid();
                    btnPrintFromPreview.Enabled = true;
                    btnTestPrint.Enabled = true;

                    Comm.MessageboxToasted("Sales", "Barcode settings saved successfully");
                }
            }
            catch (Exception ex)
            {
                Interaction.MsgBox(ex.Message, MsgBoxStyle.OkOnly);
            }
        }

        private void LoadPrintSchemes()
        {
            try
            {
                Comm.LoadControl(cmbPrintScheme, new DataTable(), "SELECT  ReportID, ReportName, ReportName as SchemeName FROM tblReportXML where vchtypeID=" + mVchtypeID + " and isnull(isbarcode,0)=1 order by ReportName");
                for (int i = 0; i < cmbPrintScheme.Items.Count; i++)
                {
                    if (cmbPrintScheme.Items[i].ToString() == mReportName)
                    {
                        cmbPrintScheme.SelectedIndex = i;
                    }
                }
                try
                {
                    if (cmbPrintScheme.SelectedIndex < 0 && cmbPrintScheme.Items.Count > 0)
                    {
                        cmbPrintScheme.SelectedIndex = 0;
                    }
                    SelectPrintScheme();
                }
                catch
                { }

                //cmbPrintScheme.SelectedText = mReportName;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void ExportToGrid()
        {
            try
            {
                string[] bcode;
                int CurrentBound = 0;
                int CurrentCol = 0;
                string strTagReplacedBcode = "";
                string[] v = { Environment.NewLine + "<SPLIT>" };
                bcode = txtBarcodeString.Text.Split(v, StringSplitOptions.None);
                // Section 0 is printer starting command, Last section is printer ending command. So avoid taking first and last portion of the bcode string
                if ((Information.UBound(bcode) >= 2 & Conversion.Val(txtLabelsPerRow.Text) > 0 & chkFileName.Checked == false))
                    CurrentBound = 1;
                else if ((Information.UBound(bcode) >= 1 & Conversion.Val(txtLabelsPerRow.Text) > 0 & chkFileName.Checked == true))
                    CurrentBound = 0;
                dgvPreview.Rows.Clear();
                dgvPreview.Columns.Clear();
                dgvPreview.AllowUserToAddRows = false;
                if ((Information.UBound(bcode) >= 2 & Conversion.Val(txtLabelsPerRow.Text) > 0 & chkFileName.Checked == false) | (Information.UBound(bcode) >= 1 & Conversion.Val(txtLabelsPerRow.Text) > 0 & chkFileName.Checked == true))
                {
                    // If (UBound(bcode) >= 2 And Conversion.Val(txtLabelsPerRow.Text) > 0) Then

                    if (txtLabelsPerRow.Text.Trim() != "")
                    {
                        dgvPreview.ColumnCount = Convert.ToInt32(txtLabelsPerRow.Text);

                        for (int i = 0; i <= dgvPreview.ColumnCount - 1; i++)
                        {
                            dgvPreview.Columns[i].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                            dgvPreview.Columns[i].ReadOnly = true;
                        }
                        dgvPreview.Rows.Add();
                        dgvPreview.RowsDefaultCellStyle.WrapMode = DataGridViewTriState.True;
                        dgvPreview.Rows[0].Height = 150;
                        for (int i = 0; i <= dgvBarcodeDetails.Rows.Count - 1; i++)
                        {
                            for (var j = 1; j <= Conversion.Val(dgvBarcodeDetails.Rows[i].Cells["PrintQty"].Value); j++)
                            {
                                strTagReplacedBcode = ReplaceTags(bcode[CurrentBound], i);
                                dgvPreview.Rows[dgvPreview.Rows.Count - 1].Cells[CurrentCol].Value = strTagReplacedBcode;
                                CurrentCol = CurrentCol + 1;
                                if (CurrentCol >= dgvPreview.ColumnCount)
                                {
                                    CurrentCol = 0;
                                    dgvPreview.Rows.Add();
                                    dgvPreview.Rows[dgvPreview.Rows.Count - 1].Height = 150;
                                }
                                CurrentBound = CurrentBound + 1;
                                if (CurrentBound >= Information.UBound(bcode))
                                {
                                    if ((Information.UBound(bcode) >= 2 & Conversion.Val(txtLabelsPerRow.Text) > 0 & chkFileName.Checked == false))
                                        CurrentBound = 1;
                                    else if ((Information.UBound(bcode) >= 1 & Conversion.Val(txtLabelsPerRow.Text) > 0 & chkFileName.Checked == true))
                                        CurrentBound = 0;
                                }
                            }
                        } 
                    }
                }
            }
            catch (Exception ex)
            {
                Interaction.MsgBox(ex.Message, MsgBoxStyle.Information);
            }
        }
        private string ReplaceTags(string strBcode, int RowIndex)
        {
            try
            {
                string strReplaced = strBcode;
                string strSearch = "";
                if (Information.IsDate(dgvBarcodeDetails.Rows[RowIndex].Cells["InvDate"].Value))
                {
                    strReplaced = strReplaced.Replace("<InvDate>", Format(dgvBarcodeDetails.Rows[RowIndex].Cells["InvDate"].Value.ToString(), "dd/MM/yy"));
                    strReplaced = strReplaced.Replace("<InvDateDDMMMYY>", Format(dgvBarcodeDetails.Rows[RowIndex].Cells["InvDate"].Value.ToString(), "dd/MMM/yy"));
                    strReplaced = strReplaced.Replace("<InvDateDDMMMYYYY>", Format(dgvBarcodeDetails.Rows[RowIndex].Cells["InvDate"].Value.ToString(), "dd/MMM/yyyy"));
                    strReplaced = strReplaced.Replace("<InvDateDDMMYY>", Format(dgvBarcodeDetails.Rows[RowIndex].Cells["InvDate"].Value.ToString(), "dd/MM/yy"));
                    strReplaced = strReplaced.Replace("<InvDateMMYY>", Format(dgvBarcodeDetails.Rows[RowIndex].Cells["InvDate"].Value.ToString(), "MM/yy"));
                    strReplaced = strReplaced.Replace("<InvDateMMYYYY>", Format(dgvBarcodeDetails.Rows[RowIndex].Cells["InvDate"].Value.ToString(), "MM/yyyy"));
                    strReplaced = strReplaced.Replace("<InvDateMMMYY>", Format(dgvBarcodeDetails.Rows[RowIndex].Cells["InvDate"].Value.ToString(), "MMM/yy"));
                    strReplaced = strReplaced.Replace("<InvDateMMMYYYY>", Format(dgvBarcodeDetails.Rows[RowIndex].Cells["InvDate"].Value.ToString(), "MMM/yyyy"));
                }
                else
                {
                    strReplaced = strReplaced.Replace("<InvDate>", "");
                    strReplaced = strReplaced.Replace("<InvDateDDMMMYY>", "");
                    strReplaced = strReplaced.Replace("<InvDateDDMMMYYYY>", "");
                    strReplaced = strReplaced.Replace("<InvDateDDMMYY>", "");
                    strReplaced = strReplaced.Replace("<InvDateMMYY>", "");
                    strReplaced = strReplaced.Replace("<InvDateMMYYYY>", "");
                    strReplaced = strReplaced.Replace("<InvDateMMMYY>", "");
                    strReplaced = strReplaced.Replace("<InvDateMMMYYYY>", "");
                }
                if (Information.IsDate(dgvBarcodeDetails.Rows[RowIndex].Cells["PackedDate"].Value.ToString()))
                {
                    strReplaced = strReplaced.Replace("<PackedDateDDMMMYY>", Format(dgvBarcodeDetails.Rows[RowIndex].Cells["PackedDate"].Value.ToString(), "dd/MMM/yy"));
                    strReplaced = strReplaced.Replace("<PackedDateDDMMMYYYY>", Format(dgvBarcodeDetails.Rows[RowIndex].Cells["PackedDate"].Value.ToString(), "dd/MMM/yyyy"));
                    strReplaced = strReplaced.Replace("<PackedDateDDMMYY>", Format(dgvBarcodeDetails.Rows[RowIndex].Cells["PackedDate"].Value.ToString(), "dd/MM/yy"));
                    strReplaced = strReplaced.Replace("<PackedDateMMYY>", Format(dgvBarcodeDetails.Rows[RowIndex].Cells["PackedDate"].Value.ToString(), "MM/yy"));
                    strReplaced = strReplaced.Replace("<PackedDateMMYYYY>", Format(dgvBarcodeDetails.Rows[RowIndex].Cells["PackedDate"].Value.ToString(), "MM/yyyy"));
                    strReplaced = strReplaced.Replace("<PackedDateMMMYY>", Format(dgvBarcodeDetails.Rows[RowIndex].Cells["PackedDate"].Value.ToString(), "MMM/yy"));
                    strReplaced = strReplaced.Replace("<PackedDateMMMYYYY>", Format(dgvBarcodeDetails.Rows[RowIndex].Cells["PackedDate"].Value.ToString(), "MMM/yyyy"));
                }
                else
                {
                    strReplaced = strReplaced.Replace("<PackedDateDDMMMYY>", "");
                    strReplaced = strReplaced.Replace("<PackedDateDDMMMYYYY>", "");
                    strReplaced = strReplaced.Replace("<PackedDateDDMMYY>", "");
                    strReplaced = strReplaced.Replace("<PackedDateMMYY>", "");
                    strReplaced = strReplaced.Replace("<PackedDateMMYYYY>", "");
                    strReplaced = strReplaced.Replace("<PackedDateMMMYY>", "");
                    strReplaced = strReplaced.Replace("<PackedDateMMMYYYY>", "");
                }

                if (Information.IsDate(dgvBarcodeDetails.Rows[RowIndex].Cells["Expiry"].Value.ToString()))
                    strReplaced = strReplaced.Replace("<Expiry>", Format(dgvBarcodeDetails.Rows[RowIndex].Cells["Expiry"].Value.ToString(), "dd/MMM/yyyy"));
                else
                    strReplaced = strReplaced.Replace("<Expiry>", "NA");

                strReplaced = strReplaced.Replace("<N>", "1");
                strReplaced = strReplaced.Replace("<n>", "1");

                string strValue;
                for (int i = 0; i <= dgvBarcodeDetails.ColumnCount - 1; i++)
                {
                    strSearch = dgvBarcodeDetails.Rows[0].Cells[i].OwningColumn.HeaderText.ToString();
                    if (strReplaced.Contains(strSearch))
                    {
                        strValue = dgvBarcodeDetails.Rows[RowIndex].Cells[i].Value.ToString().ToUpper();
                        if (strValue.Length > Convert.ToInt32(txtCharWidth.Text))
                            strValue = strValue.Substring(0, Convert.ToInt32(txtCharWidth.Text));
                        strReplaced = strReplaced.Replace("<" + strSearch + ">", Strings.UCase(strValue));
                        strReplaced = strReplaced.Replace("<" + strSearch + "_ENC>", EncrypttoAlphabets(Strings.UCase(strValue), chkEncryptDecimals.Checked));
                    }
                }

                return strReplaced;
            }
            catch (Exception ex)
            {
                Interaction.MsgBox(ex.Message, MsgBoxStyle.Information);
                return "";
            }
        }
        private string Format(string date, string format)
        {
            try
            {
                DateTime dt;
                if (Information.IsDate(date) == true)
                {
                    dt = Convert.ToDateTime(date);
                    return dt.ToString(format);
                }
                else
                    return date;
            }
            catch
            {
                return date.ToString();

            }
        }
        public string EncrypttoAlphabets(string StrVal, bool BlnDecimalEncrypt = false)
        {
            int i = 0;
            string StrReturn = "";
            string OrgVal = "";

            OrgVal = StrVal;
            BlnDecimalEncrypt = true;
            if (BlnDecimalEncrypt == true)
                StrVal = (Conversion.Val(StrVal)).ToString();
            else
                StrVal = Conversion.Int(Conversion.Val(StrVal)).ToString();

            if ((txtEncKey.Text.Length) == 0)
            {
                for (i = 1; i <= Strings.Len(StrVal); i++)
                    StrReturn = StrReturn + Strings.Chr(65 + Convert.ToInt32(Strings.Mid(StrVal, i, 1)));
            }
            else
            {
                string Encrypt;
                Encrypt = "";
                string[] Varstr;
                //Varstr = Strings.Split(Encrypt, Strings.Chr(13));
                char v1 = Strings.Chr(13);
                char[] v = { v1 };
                Varstr = Encrypt.Split(v);

                for (i = 0; i <= Strings.Len(StrVal) - 1; i++)
                {
                    switch (Strings.Mid(StrVal, i + 1, 1))
                    {
                        case "0":
                            {
                                if (txtEncKey.Text.Length > 1)
                                    Encrypt = Encrypt + txtEncKey.Text.Substring(1, 1);
                                break;
                            }

                        case "1":
                            {
                                if (txtEncKey.Text.Length > 2)
                                    Encrypt = Encrypt + txtEncKey.Text.Substring(2, 1);
                                break;
                            }

                        case "2":
                            {
                                if (txtEncKey.Text.Length > 3)
                                    Encrypt = Encrypt + txtEncKey.Text.Substring(3, 1);
                                break;
                            }

                        case "3":
                            {
                                if (txtEncKey.Text.Length > 4)
                                    Encrypt = Encrypt + txtEncKey.Text.Substring(4, 1);
                                break;
                            }

                        case "4":
                            {
                                if (txtEncKey.Text.Length > 5)
                                    Encrypt = Encrypt + txtEncKey.Text.Substring(5, 1);
                                break;
                            }

                        case "5":
                            {
                                if (txtEncKey.Text.Length > 6)
                                    Encrypt = Encrypt + txtEncKey.Text.Substring(6, 1);
                                break;
                            }

                        case "6":
                            {
                                if (txtEncKey.Text.Length > 7)
                                    Encrypt = Encrypt + txtEncKey.Text.Substring(7, 1);
                                break;
                            }

                        case "7":
                            {
                                if (txtEncKey.Text.Length > 8)
                                    Encrypt = Encrypt + txtEncKey.Text.Substring(8, 1);
                                break;
                            }

                        case "8":
                            {
                                if (txtEncKey.Text.Length > 9)
                                    Encrypt = Encrypt + txtEncKey.Text.Substring(9, 1);
                                break;
                            }

                        case "9":
                            {
                                if (txtEncKey.Text.Length > 10)
                                    Encrypt = Encrypt + txtEncKey.Text.Substring(10, 1);
                                break;
                            }

                        case ".":
                            {
                                Encrypt = Encrypt + "/";
                                break;
                            }

                        default:
                            {
                                if (txtEncKey.Text.Length > 1)
                                    Encrypt = Encrypt + txtEncKey.Text.Substring(1, 1);
                                break;
                            }
                    }
                }
                StrReturn = Encrypt;
            }

            //EncrypttoAlphabets = StrReturn;
            return StrReturn;
        }

        private async void btnPrintFromPreview_Click(object sender, EventArgs e)
        {
            try
            {
                if (tabControl1.SelectedTab.Name != tpgPreview.Name)
                {
                    tabControl1.SelectedTab = tpgPreview;
                    tpgPreview.Show();
                    tpgPreview.Focus();
                }
                ExportToGrid();
                await CreateBarcodeFile(true);
                if (MyPrinterName != "")
                {
                }
                else
                    Interaction.MsgBox("Please select an installed printer to print the barcode.", MsgBoxStyle.Information);
            }
            catch (Exception ex)
            {
                Interaction.MsgBox(ex.Message, MsgBoxStyle.Information);
            }

        }
        private async Task CreateBarcodeFile(bool blnPrint)
        {
            try
            {
                string[] bcode;
                string StartCommand;
                string EndCommand;

                string strBarcodes = "";

                string[] v = { Environment.NewLine + "<SPLIT>" };

                txtBarcodeString.Text = txtBarcodeString.Text.Replace("<split>", "<SPLIT>");
                bcode = txtBarcodeString.Text.Split(v, StringSplitOptions.None);
                if ((Information.UBound(bcode) == Convert.ToInt32(txtLabelsPerRow.Text) + 1))
                {
                    StartCommand = bcode[0];
                    EndCommand = bcode[Information.UBound(bcode)];
                }
                else if ((Information.UBound(bcode) == Convert.ToInt32(txtLabelsPerRow.Text) & chkFileName.Checked == true))
                {
                    StartCommand = "";
                    EndCommand = bcode[Information.UBound(bcode)];
                }
                else
                {
                    Interaction.MsgBox("Could not identify start and end commands of the printer. check Barcode nos per row. Number of <SPLIT> tags is Upper case", MsgBoxStyle.Information);
                    return;
                }

                ClsFileOperation fso = new ClsFileOperation();

                // Dim TemplateHeader = File.ReadAllText(Application.StartupPath & "\Resources\TemplateHeader.ini", System.Text.Encoding.UTF32)
                File.Delete(@"C:\DIGIDATA\barcode.txt");

                for (var i = 0; i <= dgvPreview.Rows.Count - 1; i++)
                {
                    strBarcodes = "";
                    if (dgvPreview.Rows[i].Cells[0].Value != null)
                    {
                        // strBarcodes = strBarcodes & StartCommand
                        fso.FileOperation(@"C:\DIGIDATA\barcode.txt", false, "", false);
                        if (chkFileName.Checked == true)
                            File.Copy(Application.StartupPath + @"\Resources\TemplateHeader.ini", @"C:\DIGIDATA\barcode.txt", true);
                        if (StartCommand != "")
                            fso.FileOperation(@"C:\DIGIDATA\barcode.txt", false, StartCommand, true);
                        // If TemplateHeader <> "" Then fso.FileOperation("C:\DIGIDATA\barcode.txt", False, TemplateHeader, True)
                        // File.AppendAllText("C:\DIGIDATA\barcode.txt", StartCommand)
                        for (var j = 0; j <= dgvPreview.ColumnCount - 1; j++)
                        {
                            if (dgvPreview.Rows[i].Cells[j].Value != null)
                                strBarcodes = strBarcodes + dgvPreview.Rows[i].Cells[j].Value.ToString();
                        }
                        // File.AppendAllText("C:\DIGIDATA\barcode.txt", dgvPreview.Item(j, i).Value.ToString)
                        // strBarcodes = strBarcodes & EndCommand
                        // File.AppendAllText("C:\DIGIDATA\barcode.txt", EndCommand)

                        fso.FileOperation(@"C:\DIGIDATA\barcode.txt", false, strBarcodes, true);
                        if (EndCommand != "")
                            fso.FileOperation(@"C:\DIGIDATA\barcode.txt", false, EndCommand, true);

                        if (blnPrint)
                        {
                            // clsPrinter.RawPrinterHelper.SendStringToPrinter(MyPrinterName, strBarcodes)
                            // PreparePrinter(MyPrinterName)
                            // SendFileToPrinter(MyPrinterName, "C:\DIGIDATA\barcode.txt")
                            Comm.SalesPrint(@"C:\DIGIDATA\barcode.txt", MyPrinterName);
                            Thread.Sleep(50);
                        }
                    }
                }
            }


            // Await WriteTextAsync("C:\DIGIDATA\barcode.txt", strBarcodes)
            catch (Exception ex)
            {
                Interaction.MsgBox(ex.Message, MsgBoxStyle.Information);
            }
        }

        private async Task WriteTextAsync(string filePath, string text)
        {
            try
            {
                StreamWriter objStreamWriter;

                // Pass the file path and the file name to the StreamWriter constructor.
                objStreamWriter = new StreamWriter(filePath);

                // Write a line of text.
                await objStreamWriter.WriteLineAsync(text);

                // Close the file.
                objStreamWriter.Close();
            }
            catch (Exception ex)
            {
                Interaction.MsgBox(ex.Message, MsgBoxStyle.Information);
            }
        }

        private async Task<string> ReadFileAsync(string filePath)
        {
            try
            {
                StreamReader objStreamReader;
                string strLine;
                StringBuilder sb = new StringBuilder();

                // Pass the file path and the file name to the StreamReader constructor.
                objStreamReader = new StreamReader(filePath);

                // Read the first line of text.
                strLine = await objStreamReader.ReadLineAsync();
                if (strLine != null)
                    sb.Append(strLine);

                // Continue to read until you reach the end of the file.
                while (strLine != null)
                {
                    // Read the next line.
                    strLine = await objStreamReader.ReadLineAsync();
                    sb.Append(strLine);
                }

                // Close the file.
                objStreamReader.Close();

                return sb.ToString();
            }
            catch (Exception ex)
            {
                Interaction.MsgBox(ex.Message, MsgBoxStyle.Information);
                return "";
            }
        }

        private void txtLabelsPerRow_TextChanged(object sender, EventArgs e)
        {
            ClearPreviewAndDisablePrint();

        }

        private void txtBarcodeString_TextChanged(object sender, EventArgs e)
        {
            ClearPreviewAndDisablePrint();

        }

        private void ClearPreviewAndDisablePrint()
        {
            try
            {
                dgvPreview.Rows.Clear();
                dgvPreview.Columns.Clear();

                btnPrintFromPreview.Enabled = false;
                btnTestPrint.Enabled = false;
            }
            catch (Exception ex)
            {
                Interaction.MsgBox(ex.Message, MsgBoxStyle.Information);
            }
        }

        private void txtSearchBarcode_TextChanged(object sender, EventArgs e)
        {
            try
            {
                TextBox txtMy = (TextBox)sender;

                ShowSubWindow(txtMy.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void cmbVoucherType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.ActiveControl != null)
                if (this.ActiveControl.Name != cmbVoucherType.Name) return;

            ClearControl();

            mVchtypeID = Comm.ToInt32(cmbVoucherType.SelectedValue.ToString());
            LoadPrintSchemes();
        }

        private async void btnTestPrint_Click(object sender, EventArgs e)
        {
            if (chkFileName.Checked == true)
            {
            }

            try
            {
                // ExportToGrid()
                // Await CreateBarcodeFile()

                try
                {
                    if (MyPrinterName != "")
                        await CreateBarcodeFile(false);
                    else
                        Interaction.MsgBox("Please select an installed printer to print the barcode.", MsgBoxStyle.Information);
                }
                catch (Exception ex)
                {
                    Interaction.MsgBox(ex.Message, MsgBoxStyle.Information);
                }


                if (File.Exists(@"C:\DIGIDATA\barcode.txt"))
                    System.Diagnostics.Process.Start(@"C:\DIGIDATA\barcode.txt");
            }
            catch (Exception ex)
            {
                Interaction.MsgBox(ex.Message, MsgBoxStyle.Information);
            }
        }

        private void cmbPrintScheme_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Delete)
                {
                    if (MessageBox.Show("DO YOU WANT TO DELETE THIS BARCODE SCHEME ", "Barcode", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                        return;
                    sqlControl rs = new sqlControl();
                    rs.Execute("Delete from tblReportXML where ReportID=" + cmbPrintScheme.SelectedValue);
                    if (rs.RecordCount > 0)
                    {
                        Comm.LoadControl(cmbPrintScheme, new DataTable(), "SELECT  ReportID, ReportName, ReportName as SchemeName FROM tblReportXML where vchtypeID=" + mVchtypeID + " and isnull(isbarcode,0)=1 order by ReportName");
                        Interaction.MsgBox("Deleted", Constants.vbInformation);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void cmbPrintScheme_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                SelectPrintScheme();
            }
            catch
            { }
        }

        private void SelectPrintScheme()
        {
            try
            {
                btnPrintFromPreview.Enabled = false;
                btnTestPrint.Enabled = false;
                txtBarcodeString.Text = "";
                strFileData = "";
                txtLabelsPerRow.Text = "";
                txtEncKey.Text = "";
                chkEncryptDecimals.Checked = false;

                if (cmbPrintScheme.SelectedIndex >= 0)
                {
                    string SchemeName = cmbPrintScheme.Text.ToString();
                    rs.Close();
                    rs.Open("Select * from tblReportXml Where ReportName='" + SchemeName + "' and VchtypeID=" + mVchtypeID + " and isnull(isbarcode,0)=1");
                    if (!rs.eof())
                    {
                        txtBarcodeString.Text = rs.fields("DesignData");
                        strFileData = rs.fields("FileData");
                        txtLabelsPerRow.Text = rs.fields("noofitems");
                        txtEncKey.Text = rs.fields("ReportData");
                        txtCharWidth.Text = rs.fields("CharWidth");

                        if (rs.fields("blnTemplateFile") == "1")
                            chkFileName.Checked = true;
                        else
                            chkFileName.Checked = false;

                        if (Convert.ToInt32(rs.fields("EncryptDecimals")) == 0)
                            chkEncryptDecimals.Checked = false;
                        else
                            chkEncryptDecimals.Checked = true;

                        if (rs.fields("PrinterName") != "")
                        {
                            MyPrinterName = rs.fields("PrinterName");
                            cmbInstalledPrinters.SelectedText = rs.fields("PrinterName");
                        }

                        btnPrintFromPreview.Enabled = true;
                        btnTestPrint.Enabled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Interaction.MsgBox(ex.Message, MsgBoxStyle.Information);
            }
        }
        private void btnPreview_Click(object sender, EventArgs e)
        {
            try
            {
                tabControl1.SelectedTab = tpgPreview;
                tpgPreview.Show();
                tpgPreview.Focus();
                ExportToGrid();
            }
            catch (Exception ex)
            {
                Interaction.MsgBox(ex.Message, MsgBoxStyle.Information);
            }
        }

        private void btnBarcodeSettings_Click(object sender, EventArgs e)
        {
            try
            {
                if (tabControl1.TabPages.Contains(SettingsTab) == true)
                {
                    tabControl1.SelectedTab = SettingsTab;
                    SettingsTab.Show();
                    SettingsTab.Focus();
                    return;
                }
                else
                {
                    tabControl1.TabPages.Add(SettingsTab);
                    tabControl1.SelectedTab = SettingsTab;
                }


                SettingsTab.Show();
                SettingsTab.Focus();
            }
            catch (Exception ex)
            {
                Interaction.MsgBox(ex.Message, MsgBoxStyle.Information);
            }
        }

        private void dgvBarcodeDetails_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgvBarcodeDetails_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            dgvBarcodeDetails.BeginEdit(true);

        }

        private void dgvSubMenu_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                // Your code here
                try
                {
                    int StockID;
                    StockID = Convert.ToInt32(dgvSubMenu.Rows[dgvSubMenu.SelectedCells[0].RowIndex].Cells[0].Value.ToString());
                    // txtSearch.Text = dgvSubMenu.Rows(dgvSubMenu.SelectedCells(0).RowIndex).Cells(1).Value.ToString


                    txtSearchBarcode.Text = Comm.GetTableValue("tblStock", "batchunique", "where StockID=" + StockID);
                    // If FillItem("", 0, StockID, "") Then

                    // End If
                    txtSearchBarcode.Focus();
                    SendKeys.Send("{ENTER}");
                }
                catch (Exception ex)
                {
                }
                e.SuppressKeyPress = true;
            }
        }

        private void cmbInstalledPrinters_SelectedIndexChanged(object sender, EventArgs e)
        {
            MyPrinterName = cmbInstalledPrinters.Text;

        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            try
            {
                string StrFilepath = "";
                OpenFileDialog fd = new OpenFileDialog();
                fd.Title = "Select Application Configration Files Path";
                fd.InitialDirectory = Application.StartupPath;
                fd.Filter = "ini files (*.ini)|*.ini";
                fd.FilterIndex = 2;
                fd.RestoreDirectory = true;
                if (fd.ShowDialog() == DialogResult.OK)
                    StrFilepath = fd.FileName;
                if (Path.GetExtension(StrFilepath).ToLower() != ".ini")
                    MessageBox.Show("Please select template file for migration process", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            // Await AddLog("Selected excel file for migration")
            // ExcelMigration(StrFilepath)

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnFill_Click(object sender, EventArgs e)
        {
            SearchInvoice(false);

        }

        private void dgvSubMenu_Resize(object sender, EventArgs e)
        {
            dgvSubMenu.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

        }

        private async void btnPrintDirect_Click(object sender, EventArgs e)
        {
            try
            {
                if (tabControl1.SelectedTab.Name != tpgPreview.Name)
                {
                    tabControl1.SelectedTab = tpgPreview;
                    tpgPreview.Show();
                    tpgPreview.Focus();
                }
                ExportToGrid();
                await CreateBarcodeFile(true);
                if (MyPrinterName != "")
                {
                }
                else
                    Interaction.MsgBox("Please select an installed printer to print the barcode.", MsgBoxStyle.Information);
            }
            catch (Exception ex)
            {
                Interaction.MsgBox(ex.Message, MsgBoxStyle.Information);
            }
        }

        private void txtSearchBarcode_Leave(object sender, EventArgs e)
        {
            try
            {
                panelsearch.Visible = false;
            }
            catch
            { }
        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void dgvBarcodeDetails_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                dgvBarcodeDetails.EndEdit();
            }
            catch
            { }
        }

        private void rdbSearchSearchInvoice_CheckedChanged(object sender, EventArgs e)
        {
            if (this.ActiveControl != null)
                if (this.ActiveControl.Name != rdbSearchSearchInvoice.Name) return;

            ClearControl();

            mVchtypeID = Comm.ToInt32(cmbVoucherType.SelectedValue.ToString());
            LoadPrintSchemes();
        }

        private void rdbSearchBarcode_CheckedChanged(object sender, EventArgs e)
        {
            if (this.ActiveControl != null)
                if (this.ActiveControl.Name != rdbSearchBarcode.Name) return;

            ClearControl();

            mVchtypeID = 0;
            LoadPrintSchemes();
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
            if (toggleWidthSettings.ToggleState == Syncfusion.Windows.Forms.Tools.ToggleButtonState.Inactive)
            {
                for (int i = 0; i < dgvColWidth.Rows.Count; i++)
                {
                    if (dgvBarcodeDetails.Columns[i].Name == dgvColWidth.Rows[i].Cells[3].Value.ToString())
                    {
                        dgvBarcodeDetails.Columns[i].Width = Convert.ToInt32(dgvColWidth.Rows[i].Cells[2].Value.ToString());
                        if (dgvColWidth.Rows[i].Cells[0].Value.ToString() == "")
                            dgvBarcodeDetails.Columns[i].Visible = false;
                        else
                            dgvBarcodeDetails.Columns[i].Visible = Convert.ToBoolean(dgvColWidth.Rows[i].Cells[0].Value);
                    }
                    if (dgvBarcodeDetails.Columns[i].Name == "cRateinclusive")
                        dgvBarcodeDetails.Columns[i].Visible = false;
                }
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

        }


        //Description : Seting Value to the Given Column, Row Indexes to Grid Cell
        private void SetValue(int iCellIndex, string sValue, string sConvertType = "")
        {
            if (dgvBarcodeDetails.Rows.Count > 0)
            {
                if (sConvertType.ToUpper() == "CURR_FLOAT")
                {
                    if (sValue == "") sValue = "0";
                    //Commented and Added By Dipu on 17-Feb-2022 ------------- >>
                    //dgvBarcodeDetails.Rows[dgvBarcodeDetails.CurrentRow.Index].Cells[iCellIndex].Value = Comm.chkChangeValuetoZero(FormatValue(Convert.ToDouble(sValue)));
                    //Commented and Added By Dipu on 17-Feb-2022 ------------- >>

                    this.dgvBarcodeDetails.Columns[dgvBarcodeDetails.CurrentCell.ColumnIndex].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
                else if (sConvertType.ToUpper() == "QTY_FLOAT")
                {
                    if (sValue == "") sValue = "0";
                    //dgvBarcodeDetails.Rows[dgvBarcodeDetails.CurrentRow.Index].Cells[iCellIndex].Value = Comm.chkChangeValuetoZero(FormatValue(Convert.ToDouble(sValue), false));
                    this.dgvBarcodeDetails.Columns[dgvBarcodeDetails.CurrentCell.ColumnIndex].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
                else if (sConvertType.ToUpper() == "PERC_FLOAT")
                {
                    if (sValue == "") sValue = "0";
                    dgvBarcodeDetails.Rows[dgvBarcodeDetails.CurrentRow.Index].Cells[iCellIndex].Value = Comm.chkChangeValuetoZero(Convert.ToDecimal(sValue).ToString("#.00"));
                    this.dgvBarcodeDetails.Columns[dgvBarcodeDetails.CurrentCell.ColumnIndex].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
                else if (sConvertType.ToUpper() == "DATE")
                {
                    dgvBarcodeDetails.Rows[dgvBarcodeDetails.CurrentRow.Index].Cells[iCellIndex].Value = Convert.ToDateTime(sValue).ToString("dd/MMM/yyyy");
                }
                else
                    dgvBarcodeDetails.Rows[dgvBarcodeDetails.CurrentRow.Index].Cells[iCellIndex].Value = Comm.chkChangeValuetoZero(sValue);
            }
        }

        //Description : Setting Value to Tag of the cells of Grid using the Given Column and Row Indexes
        private void setTag(int iCellIndex, string sTag, string sConvertType = "")
        {
            if (sConvertType.ToUpper() == "FLOAT")
            {
                if (sTag == "") sTag = "0";
                dgvBarcodeDetails.Rows[dgvBarcodeDetails.CurrentRow.Index].Cells[iCellIndex].Tag = Convert.ToDecimal(sTag).ToString("#.00");
            }
            else if (sConvertType.ToUpper() == "DATE")
            {
                dgvBarcodeDetails.Rows[dgvBarcodeDetails.CurrentRow.Index].Cells[iCellIndex].Tag = Convert.ToDateTime(sTag).ToString("dd/MMM/yyyy");
            }
            else
                dgvBarcodeDetails.Rows[dgvBarcodeDetails.CurrentRow.Index].Cells[iCellIndex].Tag = sTag;
        }

        //Description : Function Polymorphism for Seting the Value to Grid Asper Parameters Given
        private void SetValue(int iCellIndex, int iRowIndex, string sValue, string sConvertType = "")
        {
            //if(sConvertType.ToUpper() == "QTY")
            //    dgvBarcodeDetails.Rows[iRowIndex].Cells[iCellIndex].Value = Comm.chkChangeValuetoZero(FormatValue(Convert.ToDouble(sValue),false));
            //else
            dgvBarcodeDetails.Rows[iRowIndex].Cells[iCellIndex].Value = Comm.chkChangeValuetoZero(sValue);
        }

        //Description : Validating the Method with Before Save Functionality
        private bool IsValidate()
        {
            DataTable dtInv = new DataTable();
            bool bValidate = true;
            string sWarnMsg = "|";
            string[] sMsg;

            sMsg = sWarnMsg.Split('|');

        FailsHere:
            return bValidate;
        }

        //Description : Function Polymorphism of SetTag
        private void SetTag(int iCellIndex, int iRowIndex, string sTag, string sConvertType = "")
        {
            if (sConvertType.ToUpper() == "PERC_FLOAT")
                dgvBarcodeDetails.Rows[iRowIndex].Cells[iCellIndex].Tag = Convert.ToDecimal(sTag).ToString("#.00");
            else
                dgvBarcodeDetails.Rows[iRowIndex].Cells[iCellIndex].Tag = sTag;
        }

        //Description : Convert the Enum Members to Column index
        private int GetEnum(int ColIndexes)
        {
            return ColIndexes;
        }

        //Description : Convert the Ledger Enum Members to Array Index

        //Description : What to happen when BatchCode/BarUnique Select from the Grid Compact Search

        //Description : Deligate Returns the True/False from the method from Pause Search List

        //Description : Row Delete when Press Delete or Delete icon

        //Description : Initializing the Columns in The Grid
        private void AddColumnsToGrid()
        {
            this.dgvBarcodeDetails.Columns.Clear();

            this.dgvBarcodeDetails.Columns.Add(new DataGridViewTextBoxColumn() { Name = "cSlNo", HeaderText = "Sl.No", Width = 50 }); //1



        }

        //Description : Initialize for Item Column Width Settings
        private void GridInitialize_dgvColWidth(bool bIsLoad = true)
        {

        }

        private void txtDiscPerc_KeyPress(object sender, KeyPressEventArgs e)
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

                //dSteadyBillDiscPerc = Convert.ToDecimal(txtDiscPerc.Text);
                dSteadyBillDiscAmt = 0;
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
        }

    }
    #endregion
}
