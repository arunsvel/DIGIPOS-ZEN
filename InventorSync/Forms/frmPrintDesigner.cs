using DigiposZen.InventorBL.Helper;
using Gma.QrCodeNet.Encoding;
using Gma.QrCodeNet.Encoding.Windows.Render;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.PowerPacks.Printing.Compatibility.VB6;
using QRCoder;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DigiposZen.Forms
{
    public partial class frmPrintDesigner : Form
    {
        private long i;
        private long j;
        private int ActivePageNo;
        private bool mblnpreview;
        private ColorDialog CDCOLOR = new ColorDialog();
        private float PScale;
        private bool BlnFooter;

        private const int MyScale = 96;
        private double TopScrollScale;
        private float footerdiff;
        private double lastitemtop;

        private bool Mblnvariable;
        private float zoom;
        private int cbCount;
        public System.Object SelControl;
        public bool Drag1, Drag2, Drag3, Drag4, isMoving;
        private double dblLeftMargin;
        private bool blnSendToprinter;
        // Barcode declaratios
        private string BINARY;
        private int CheckSumVal;
        private string BINVAL;
        private Int32 BINARYLength;
        private PrintDocument _printDoc;

        Common Comm = new Common();

        //Font SelectedItemFont = new Font("Tahoma", 9, FontStyle.Regular);

        internal PrintDocument printDoc
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _printDoc;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                if (_printDoc != null)
                {
                }

                _printDoc = value;
                if (_printDoc != null)
                {
                }
            }
        }

        private Bitmap bmpBarcode;
        // =======================


        private Bitmap[] b1 = new Bitmap[6];
        // Graphics object (printing buffer)
        private Graphics g1;
        private Point m_PanStartPoint = new Point();
        private double IncTop;
        private int MIntPages;
        private long NoOfPages;
        private int MParentVchtypeID;
        private int MprintSchemeID;
        private int mVchtypeID;
        private string mPrintSchemeName;
        private Printer gObjPrinter = new Printer();
        
        frmMDI frmMDIParent = null;

        public frmPrintDesigner(object MDIParent)
        {
            InitializeComponent();

            if (MdiParent != null) 
                frmMDIParent = (frmMDI)MdiParent;
        }

        private void OptDesign_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void btnImport_Click(object sender, EventArgs e)
        {

        }

        private void btnExport_Click(object sender, EventArgs e)
        {

        }

        private void frmPrintDesigner_Activated(object sender, EventArgs e)
        {
            InitializePages();

        }

        public int InitializePages()
        {
            try
            {
                bool done = false;

                if (!done)
                {
                    // Size and describe the form
                    // Size, Locate, & Describe the button
                    // AUTOREDRAW INITIALIZATION
                    // Create the initial bitmap from Form
                    long i;
                    for (i = 0; i <= b1.GetUpperBound(0); i++)
                    {
                        b1[i] = new Bitmap(this.ClientSize.Width * 2, this.ClientSize.Height * 16, this.picDocument.CreateGraphics());
                    }
                    
                    // Create the Graphics Object buffer
                    // which ties the bitmap to it so that
                    // when you draw something on the object
                    // the bitmap is updated
                    if (b1.GetUpperBound(0) > 0)
                    {
                        g1 = Graphics.FromImage(b1[ActivePageNo]);
                        // Prevent reentry to initialization
                        done = true;
                    }
                }
                return 0;
            }
            catch
            {
                return 0;
            }
        }

        private void frmPrintDesigner_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Dispose();

        }

        private void frmPrintDesigner_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F6)
            {
                btnprint_Click(sender, e);
            }
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }

        }

        private void frmPrintDesigner_Load(object sender, EventArgs e)
        {
            try
            {
                double l = (frmMDIParent.ClientSize.Width - this.Width - 200) / (double)2;
                double t = ((frmMDIParent.ClientSize.Height - this.Height - 200) / (double)2);
                this.SetBounds(0, 0, this.Width - 200, this.Height - 200);
                this.MdiParent = frmMDIParent;
                this.Icon = frmMDIParent.Icon;
                BtnFontColor.BackColor = Color.Black;
                BtnFontColor.ForeColor = Color.White;
                InitializePages();
            }
            catch (Exception ex)
            {
            }
        }

        public int LoadTree(int parentVchTypeID)
        {
            try
            {
                SetPropertyWindow();
                trvPrint.Nodes.Clear();
                TreeNode TrvNode;
                TrvNode = trvPrint.Nodes.Add("Header");
                TrvNode.Nodes.Add("CompanyName", "CompanyName");
                TrvNode.Nodes.Add("CST No", "CST No", "CST No");
                TrvNode.Nodes.Add("CompanyLogo", "CompanyLogo");
                TrvNode.Nodes.Add("CompanyAddress", "CompanyAddress");
                TrvNode.Nodes.Add("TaxDeclaration", "TaxDeclaration");
                switch (MParentVchtypeID)
                {
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                    case 5:
                    case 6:
                    case 18:
                    case 14:
                    case 15:
                        {
                            TrvNode.Nodes.Add("CashBill", "CashBill");
                            TrvNode.Nodes.Add("EffectiveDate", "EffectiveDate");
                            TrvNode.Nodes.Add("PageNo", "PageNo");
                            TrvNode.Nodes.Add("PageNo", "TotalPages");
                            TrvNode.Nodes.Add("PrintCopy", "PrintCopy");

                            TrvNode.Nodes.Add("Party", "Party");
                            TrvNode.Nodes.Add("PartyAddress", "PartyAddress");
                            TrvNode.Nodes.Add("GStType", "GStType");
                            TrvNode.Nodes.Add("PartyCode", "PartyCode");
                            TrvNode.Nodes.Add("MobileNo", "MobileNo");
                            TrvNode.Nodes.Add("email", "email");
                            TrvNode.Nodes.Add("PARTYGSTIN", "PARTYGSTIN");
                            TrvNode.Nodes.Add("StateCode", "StateCode");
                            // TrvNode.Nodes.Add("BillType", "BillType")
                            TrvNode.Nodes.Add("PriceList", "PriceList");
                            TrvNode.Nodes.Add("Costcentre", "Costcentre");
                            TrvNode.Nodes.Add("AGENT", "AGENT");
                            TrvNode.Nodes.Add("InvNo", "InvNo");
                            TrvNode.Nodes.Add("RefNo", "RefNo");

                            TrvNode.Nodes.Add("EInvoiceIRN", "EInvoiceIRN");

                            TrvNode.Nodes.Add("AckNo", "AckNo");
                            TrvNode.Nodes.Add("AckDt", "AckDt");
                            TrvNode.Nodes.Add("SignedInvoice", "SignedInvoice");
                            TrvNode.Nodes.Add("SignedQRCode", "SignedQRCode");
                            TrvNode.Nodes.Add("Status", "Status");
                            TrvNode.Nodes.Add("EwbNo", "EwbNo");
                            TrvNode.Nodes.Add("EwbDt", "EwbDt");
                            TrvNode.Nodes.Add("EwbValidTill", "EwbValidTill");
                            TrvNode.Nodes.Add("Remarks", "Remarks");

                            TrvNode.Nodes.Add("InvDate", "InvDate");
                            TrvNode.Nodes.Add("InvNoBarcodeH", "InvNoBarcodeH");
                            TrvNode.Nodes.Add("InvNoQRcodeH", "InvNoQRcodeH");
                            TrvNode.Nodes.Add("IRNQRcodeH", "IRNQRcodeH");
                            TrvNode.Nodes.Add("UserQRcodeH", "UserQRcodeH");



                            TrvNode.Nodes.Add("DeliveryDetails", "DeliveryDetails");
                            TrvNode.Nodes.Add("DespatchDetails", "DespatchDetails");
                            TrvNode.Nodes.Add("Termsofdelivery", "Termsofdelivery");




                            TrvNode.Nodes.Add("BillTime", "BillTime");
                            TrvNode.Nodes.Add("MOP", "MOP");
                            TrvNode.Nodes.Add("PaymentType", "PaymentType");
                            TrvNode.Nodes.Add("Taxmode", "Taxmode");
                            TrvNode.Nodes.Add("SalesMan", "SalesMan");

                            TrvNode.Nodes.Add("Box1", "Box1");
                            TrvNode.Nodes.Add("Box2", "Box2");
                            TrvNode.Nodes.Add("Box3", "Box3");
                            TrvNode.Nodes.Add("Box4", "Box4");
                            TrvNode.Nodes.Add("Box5", "Box5");
                            TrvNode.Nodes.Add("Box6", "Box6");
                            TrvNode.Nodes.Add("Box7", "Box7");
                            TrvNode.Nodes.Add("Box8", "Box8");
                            TrvNode.Nodes.Add("Box9", "Box9");
                            TrvNode.Nodes.Add("Box10", "Box10");
                            TrvNode.Nodes[1].ExpandAll();
                            TrvNode.Nodes.Add("Line1", "Line1");
                            TrvNode.Nodes.Add("Line2", "Line2");
                            TrvNode.Nodes.Add("Line3", "Line3");
                            TrvNode.Nodes.Add("Line4", "Line4");
                            TrvNode.Nodes.Add("Line5", "Line5");
                            TrvNode.Nodes.Add("Line6", "Line6");
                            TrvNode.Nodes.Add("Line7", "Line7");
                            TrvNode.Nodes.Add("Line8", "Line8");
                            TrvNode.Nodes.Add("Line9", "Line9");
                            TrvNode.Nodes.Add("Line10", "Line10");


                            TrvNode.ExpandAll();

                            TrvNode = trvPrint.Nodes.Add("ItemDetails");
                            TrvNode.Nodes.Add("SlNo", "SlNo");
                            TrvNode.Nodes.Add("HSNCode", "HSNCode");
                            TrvNode.Nodes.Add("ItemName", "ItemName");
                            TrvNode.Nodes.Add("ItemCode", "ItemCode");

                            TrvNode.Nodes.Add("ItemNameUnicode", "ItemNameUnicode");
                            TrvNode.Nodes.Add("ItemCodeUnicode", "ItemCodeUnicode");

                            TrvNode.Nodes.Add("BatchCode", "BatchCode");
                            TrvNode.Nodes.Add("SerialNumber", "SerialNumber");
                            TrvNode.Nodes.Add("Description", "Description");
                            TrvNode.Nodes.Add("MRP", "MRP");
                            TrvNode.Nodes.Add("UNIT", "UNIT");
                            TrvNode.Nodes.Add("TaxPer", "TaxPer");
                            if (AppSettings.TaxMode == 2)
                            {
                                TrvNode.Nodes.Add("CGSTP", "CGSTP");
                                TrvNode.Nodes.Add("SGSTP", "SGSTP");
                                TrvNode.Nodes.Add("IGSTP", "IGSTP");
                                TrvNode.Nodes.Add("CGSTA", "CGSTA");
                                TrvNode.Nodes.Add("SGSTA", "SGSTA");
                                TrvNode.Nodes.Add("IGSTA", "IGSTA");
                            }
                            TrvNode.Nodes.Add("Rate", "Rate");
                            TrvNode.Nodes.Add("RateINC", "RateINC");
                            TrvNode.Nodes.Add("ARate", "ARate");

                            TrvNode.Nodes.Add("PRate", "PRate");
                            TrvNode.Nodes.Add("SRate", "SRate");

                            TrvNode.Nodes.Add("Qty", "Qty");

                            TrvNode.Nodes.Add("FreeQty", "FreeQty");
                            TrvNode.Nodes.Add("GrossVal", "GrossVal");
                            TrvNode.Nodes.Add("DiscAmt", "DiscAmt");
                            TrvNode.Nodes.Add("ISavings", "ISavings");
                            TrvNode.Nodes.Add("ItemDiscAmt", "ItemDiscAmt");
                            if (parentVchTypeID == 1)
                            {
                                TrvNode.Nodes.Add("KFC", "KFC");
                                TrvNode.Nodes.Add("NetvalExclKFC", "NetvalExclKFC");
                            }

                            TrvNode.Nodes.Add("NetVal", "NetVal");
                            TrvNode.Nodes.Add("TaxAmt", "TaxAmt");
                            TrvNode.Nodes.Add("icessper", "icessper");
                            TrvNode.Nodes.Add("icessamt", "icessamt");
                            TrvNode.Nodes.Add("icompcessper", "icompcessper");
                            TrvNode.Nodes.Add("icompcessamt", "icompcessamt");




                            // TrvNode.Nodes.Add("Total", "Total")
                            TrvNode.ExpandAll();

                            // 'Second Item row
                            TrvNode = trvPrint.Nodes.Add("ItemDetails2");
                            TrvNode.Nodes.Add("SlNo2", "SlNo2");
                            TrvNode.Nodes.Add("HSNCode2", "HSNCode2");
                            TrvNode.Nodes.Add("ItemName2", "ItemName2");
                            TrvNode.Nodes.Add("ItemCode2", "ItemCode2");

                            TrvNode.Nodes.Add("ItemNameUnicode2", "ItemNameUnicode2");
                            TrvNode.Nodes.Add("ItemCodeUnicode2", "ItemCodeUnicode2");

                            TrvNode.Nodes.Add("BatchCode2", "BatchCode2");
                            TrvNode.Nodes.Add("SerialNumber2", "SerialNumber2");
                            TrvNode.Nodes.Add("Description2", "Description2");
                            TrvNode.Nodes.Add("MRP2", "MRP2");
                            TrvNode.Nodes.Add("UNIT2", "UNIT2");
                            TrvNode.Nodes.Add("TaxPer2", "TaxPer2");
                            if (AppSettings.TaxMode == 2)
                            {
                                TrvNode.Nodes.Add("CGSTP3", "CGSTP2");
                                TrvNode.Nodes.Add("SGSTP2", "SGSTP2");
                                TrvNode.Nodes.Add("IGSTP2", "IGSTP2");
                                TrvNode.Nodes.Add("CGSTA2", "CGSTA2");
                                TrvNode.Nodes.Add("SGSTA2", "SGSTA2");
                                TrvNode.Nodes.Add("IGSTA2", "IGSTA2");
                            }
                            TrvNode.Nodes.Add("Rate2", "Rate2");
                            TrvNode.Nodes.Add("RateINC2", "RateINC2");
                            TrvNode.Nodes.Add("ARate2", "ARate2");

                            TrvNode.Nodes.Add("SRate2", "SRate2");

                            TrvNode.Nodes.Add("Qty2", "Qty2");
                            TrvNode.Nodes.Add("FreeQty2", "FreeQty2");
                            TrvNode.Nodes.Add("GrossVal2", "GrossVal2");
                            TrvNode.Nodes.Add("DiscAmt2", "DiscAmt2");
                            TrvNode.Nodes.Add("ISavings2", "ISavings2");
                            TrvNode.Nodes.Add("ItemDiscAmt2", "ItemDiscAmt2");
                            if (parentVchTypeID == 1)
                            {
                                TrvNode.Nodes.Add("KFC2", "KFC2");
                                TrvNode.Nodes.Add("NetvalExclKFC2", "NetvalExclKFC2");
                            }
                            TrvNode.Nodes.Add("NetVal2", "NetVal2");
                            TrvNode.Nodes.Add("TaxAmt2", "TaxAmt2");

                            TrvNode.Nodes.Add("icessper2", "icessper2");
                            TrvNode.Nodes.Add("icessamt2", "icessamt2");
                            TrvNode.Nodes.Add("icompcessper2", "icompcessper2");
                            TrvNode.Nodes.Add("icompcessamt2", "icompcessamt2");

                            // TrvNode.Nodes.Add("Total2", "Total2")
                            TrvNode.ExpandAll();
                            // third item row

                            TrvNode = trvPrint.Nodes.Add("ItemDetails3");
                            TrvNode.Nodes.Add("SlNo3", "SlNo3");
                            TrvNode.Nodes.Add("HSNCode3", "HSNCode3");
                            TrvNode.Nodes.Add("ItemName3", "ItemName3");
                            TrvNode.Nodes.Add("ItemCode3", "ItemCode3");

                            TrvNode.Nodes.Add("ItemNameUnicode3", "ItemNameUnicode3");
                            TrvNode.Nodes.Add("ItemCodeUnicode3", "ItemCodeUnicode3");

                            TrvNode.Nodes.Add("BatchCode3", "BatchCode3");
                            TrvNode.Nodes.Add("SerialNumber3", "SerialNumber3");
                            TrvNode.Nodes.Add("Description3", "Description3");
                            TrvNode.Nodes.Add("MRP3", "MRP3");
                            TrvNode.Nodes.Add("UNIT3", "UNIT3");
                            TrvNode.Nodes.Add("TaxPer3", "TaxPer3");
                            if (AppSettings.TaxMode == 2)
                            {
                                TrvNode.Nodes.Add("CGSTP3", "CGSTP3");
                                TrvNode.Nodes.Add("SGSTP3", "SGSTP3");
                                TrvNode.Nodes.Add("IGSTP3", "IGSTP3");
                                TrvNode.Nodes.Add("CGSTA3", "CGSTA3");
                                TrvNode.Nodes.Add("SGSTA3", "SGSTA3");
                                TrvNode.Nodes.Add("IGSTA3", "IGSTA3");
                            }
                            TrvNode.Nodes.Add("Rate3", "Rate3");
                            TrvNode.Nodes.Add("RateINC3", "RateINC3");
                            TrvNode.Nodes.Add("ARate3", "ARate3");

                            TrvNode.Nodes.Add("SRate3", "SRate3");

                            TrvNode.Nodes.Add("Qty3", "Qty3");
                            TrvNode.Nodes.Add("FreeQty3", "FreeQty3");
                            TrvNode.Nodes.Add("GrossVal3", "GrossVal3");
                            TrvNode.Nodes.Add("DiscAmt3", "DiscAmt3");
                            TrvNode.Nodes.Add("ISavings3", "ISavings3");
                            TrvNode.Nodes.Add("ItemDiscAmt3", "ItemDiscAmt3");
                            if (parentVchTypeID == 1)
                            {
                                TrvNode.Nodes.Add("KFC3", "KFC3");
                                TrvNode.Nodes.Add("NetvalExclKFC3", "NetvalExclKFC3");
                            }
                            TrvNode.Nodes.Add("NetVal3", "NetVal3");
                            TrvNode.Nodes.Add("TaxAmt3", "TaxAmt3");

                            TrvNode.Nodes.Add("icessper3", "icessper3");
                            TrvNode.Nodes.Add("icessamt3", "icessamt3");
                            TrvNode.Nodes.Add("icompcessper3", "icompcessper3");
                            TrvNode.Nodes.Add("icompcessamt3", "icompcessamt3");

                            // TrvNode.Nodes.Add("Total3", "Total3")
                            TrvNode.ExpandAll();

                            TrvNode = trvPrint.Nodes.Add("ReportFooter");
                            TrvNode.Nodes.Add("RateXQtyTot", "RateXQtyTot");
                            TrvNode.Nodes.Add("GrossAmt", "GrossAmt");
                            TrvNode.Nodes.Add("ItemDiscountTotal", "ItemDiscountTotal");
                            TrvNode.Nodes.Add("TaxAmtTot", "TaxAmtTot");
                            TrvNode.Nodes.Add("TaxableTot", "TaxableTot");
                            TrvNode.Nodes.Add("NonTaxable", "NonTaxable");
                            if (AppSettings.TaxMode == 2)
                            {
                                TrvNode.Nodes.Add("CGSTTotal", "CGSTTotal");
                                TrvNode.Nodes.Add("SGSTTotal", "SGSTTotal");
                                TrvNode.Nodes.Add("IGSTTotal", "IGSTTotal");
                                TrvNode.Nodes.Add("FloodCesstot", "FloodCesstot");
                                TrvNode.Nodes.Add("cesstot", "cesstot");
                                TrvNode.Nodes.Add("qtycesstot", "qtycesstot");
                            }
                            TrvNode.Nodes.Add("PrateValue", "PrateValue");
                            TrvNode.Nodes.Add("SrateValue", "SrateValue");
                            TrvNode.Nodes.Add("NetAmount", "NetAmount");

                            TrvNode.Nodes.Add("OtherExpense", "OtherExpense");
                            TrvNode.Nodes.Add("UserNarration", "UserNarration");
                            TrvNode.Nodes.Add("DiscPer", "DiscPer");
                            TrvNode.Nodes.Add("Discount", "Discount");
                            TrvNode.Nodes.Add("CashDiscount", "CashDiscount");
                            TrvNode.Nodes.Add("RoundOFF", "RoundOFF");
                            TrvNode.Nodes.Add("InvNoBarcodeF", "InvNoBarcodeF");
                            TrvNode.Nodes.Add("InvNoQRcodeF", "InvNoQRcodeF");
                            TrvNode.Nodes.Add("SoudiQRCode", "SoudiQRCode");
                            TrvNode.Nodes.Add("UserQRcodeF", "UserQRcodeF");
                            TrvNode.Nodes.Add("SlNoTot", "SLNoTot");
                            TrvNode.Nodes.Add("MRPTot", "MRPTot");
                            TrvNode.Nodes.Add("Outstanding", "Outstanding");
                            TrvNode.Nodes.Add("OutstandingOnly", "OutstandingOnly");
                            TrvNode.Nodes.Add("ReturnAmt", "ReturnAmt");
                            TrvNode.Nodes.Add("RefNoF", "RefNoF");
                            TrvNode.Nodes.Add("BillAmt", "BillAmt");

                            if (parentVchTypeID == 1)
                                TrvNode.Nodes.Add("NetAmtW/OKFC", "NetAmtW/OKFC");

                            TrvNode.Nodes.Add("InWords", "InWords");
                            TrvNode.Nodes.Add("QtyTotal", "QtyTotal");
                            TrvNode.Nodes.Add("iQtyTotal", "iQtyTotal");
                            TrvNode.Nodes.Add("iFreeTotal", "iFreeTotal");
                            TrvNode.Nodes.Add("CustomerPoints", "CustomerPoints");

                            TrvNode.Nodes.Add("GiftVchNoRedeem", "GiftVchNoRedeem");
                            TrvNode.Nodes.Add("GiftVchAmtRedeem", "GiftVchAmtRedeem");
                            TrvNode.Nodes.Add("Savings", "Savings");
                            TrvNode.Nodes.Add("TaxSplit", "TaxSplit");
                            TrvNode.Nodes.Add("UserName", "UserName");
                            TrvNode.Nodes.Add("CardAmount", "CardAmount");
                            TrvNode.Nodes.Add("CardNotes", "CardNotes");
                            TrvNode.Nodes.Add("CreditAmount", "CreditAmount");
                            TrvNode.Nodes.Add("BankTransferAmount", "BankTransferAmount");
                            TrvNode.Nodes.Add("ChequeAmount", "ChequeAmount");
                            TrvNode.Nodes.Add("WalletsAmount", "WalletsAmount");
                            TrvNode.Nodes.Add("Cash", "Cash");
                            TrvNode.Nodes.Add("BalanceAmount", "BalanceAmount");
                            if (parentVchTypeID == 2)
                            {
                                TrvNode.Nodes.Add("SrateTotal1", "SrateTotal1");
                                TrvNode.Nodes.Add("SrateTotal2", "SrateTotal2");
                                TrvNode.Nodes.Add("SrateTotal3", "SrateTotal3");
                                TrvNode.Nodes.Add("SrateTotal4", "SrateTotal4");
                                TrvNode.Nodes.Add("SrateTotal5", "SrateTotal5");
                            }
                            TrvNode.ExpandAll();

                            // TrvNode.Nodes.Add("Footer1", "Footer2")
                            // TrvNode.Nodes.Add("Footer2", "Footer2")
                            // TrvNode.Nodes.Add("Footer3", "Footer3")
                            TrvNode.Nodes.Add("SignatureImage", "SignatureImage");
                            TrvNode.Nodes.Add("InvConditions", "InvConditions");
                            TrvNode.Nodes.Add("Narration", "Narration");
                            TrvNode.Nodes.Add("FooterLine", "FooterLine");
                            TrvNode.Nodes.Add("Box11", "Box11");
                            TrvNode.Nodes.Add("Box12", "Box12");
                            TrvNode.Nodes.Add("Box13", "Box13");
                            TrvNode.Nodes.Add("Box14", "Box14");
                            TrvNode.Nodes.Add("Box15", "Box15");
                            TrvNode.Nodes.Add("Box16", "Box16");
                            TrvNode.Nodes.Add("Box17", "Box17");
                            TrvNode.Nodes.Add("Box18", "Box18");
                            TrvNode.Nodes.Add("Box19", "Box19");
                            TrvNode.Nodes.Add("Box20", "Box20");

                            TrvNode.Nodes.Add("Line11", "Line11");
                            TrvNode.Nodes.Add("Line12", "Line12");
                            TrvNode.Nodes.Add("Line13", "Line13");
                            TrvNode.Nodes.Add("Line14", "Line14");
                            TrvNode.Nodes.Add("Line15", "Line15");
                            TrvNode.Nodes.Add("Line16", "Line16");
                            TrvNode.Nodes.Add("Line17", "Line17");
                            TrvNode.Nodes.Add("Line18", "Line18");
                            TrvNode.Nodes.Add("Line19", "Line19");
                            TrvNode.Nodes.Add("Line20", "Line20");


                            TrvNode.Nodes.Add("CompanyNameF", "CompanyNameF");
                            TrvNode.Nodes.Add("CompanyNameF", "CompanyNameF");
                            TrvNode.Nodes.Add("CompanyLogoF", "CompanyLogoF");
                            TrvNode.Nodes.Add("CompanyAddressF", "CompanyAddressF");
                            TrvNode.Nodes.Add("PartyF", "PartyF");
                            TrvNode.Nodes.Add("MobileNoF", "MobileNoF");
                            TrvNode.Nodes.Add("InvNoF", "InvNoF");
                            TrvNode.Nodes.Add("InvDateF", "InvDateF");

                            TrvNode = trvPrint.Nodes.Add("GiftVoucher");

                            TrvNode.Nodes.Add("GiftVoucherNo", "GiftVoucherNo");
                            TrvNode.Nodes.Add("GiftVoucherAmount", "GiftVoucherAmount");
                            TrvNode.Nodes.Add("VoucherValidity", "VoucherValidity");
                            TrvNode.Nodes.Add("GiftVchConditionsText", "GiftVchConditionsText");
                            TrvNode.Nodes.Add("GiftVoucherNoBarcode", "GiftVoucherNoBarcode");
                            TrvNode.Nodes.Add("GiftVoucherNoQRcode", "GiftVoucherNoQRcode");

                            TrvNode.Nodes.Add("Box21", "Box21");
                            TrvNode.Nodes.Add("Box22", "Box22");
                            TrvNode.Nodes.Add("Box23", "Box23");
                            TrvNode.Nodes.Add("Box24", "Box24");
                            TrvNode.Nodes.Add("Box25", "Box25");

                            TrvNode.Nodes.Add("LINE21", "LINE21");
                            TrvNode.Nodes.Add("LINE22", "LINE22");
                            TrvNode.Nodes.Add("LINE23", "LINE23");
                            TrvNode.Nodes.Add("LINE24", "LINE24");
                            TrvNode.Nodes.Add("LINE25", "LINE25");
                            break;
                        }

                    case 7:
                    case 9:
                    case 8:
                    case 10:
                        {
                            TrvNode.Nodes.Add("PageNo", "PageNo");
                            TrvNode.Nodes.Add("PageNo", "TotalPages");
                            TrvNode.Nodes.Add("PartyAddress", "PartyAddress");
                            TrvNode.Nodes.Add("Party", "Party");
                            TrvNode.Nodes.Add("GStType", "GStType");
                            TrvNode.Nodes.Add("PartyCode", "PartyCode");
                            TrvNode.Nodes.Add("MobileNo", "MobileNo");
                            TrvNode.Nodes.Add("email", "email");
                            TrvNode.Nodes.Add("PARTYGSTIN", "PARTYGSTIN");
                            TrvNode.Nodes.Add("StateCode", "StateCode");




                            TrvNode.Nodes.Add("BillType", "BillType");

                            TrvNode.Nodes.Add("Costcentre", "Costcentre");
                            TrvNode.Nodes.Add("LedgerName", "LedgerName");
                            TrvNode.Nodes.Add("LedgerCode", "LedgerCode");
                            TrvNode.Nodes.Add("InvNoBarcodeH", "InvNoBarcodeH");
                            TrvNode.Nodes.Add("InvNo", "InvNo");
                            TrvNode.Nodes.Add("InvDate", "InvDate");
                            TrvNode.Nodes.Add("Ch_Day", "Ch_Day");
                            TrvNode.Nodes.Add("Ch_Month", "Ch_Month");
                            TrvNode.Nodes.Add("Ch_Year1", "Ch_Year1");
                            TrvNode.Nodes.Add("Ch_Year2", "Ch_Year2");
                            TrvNode.Nodes.Add("BillTime", "BillTime");



                            TrvNode.Nodes.Add("Taxmode", "Taxmode");
                            TrvNode.Nodes.Add("SalesMan", "SalesMan");

                            TrvNode.Nodes.Add("Box1", "Box1");
                            TrvNode.Nodes.Add("Box2", "Box2");
                            TrvNode.Nodes.Add("Box3", "Box3");
                            TrvNode.Nodes.Add("Box4", "Box4");
                            TrvNode.Nodes.Add("Box5", "Box5");
                            TrvNode.Nodes.Add("Box6", "Box6");
                            TrvNode.Nodes.Add("Box7", "Box7");
                            TrvNode.Nodes.Add("Box8", "Box8");
                            TrvNode.Nodes.Add("Box9", "Box9");
                            TrvNode.Nodes.Add("Box10", "Box10");
                            TrvNode.Nodes[1].ExpandAll();
                            TrvNode.Nodes.Add("Line1", "Line1");
                            TrvNode.Nodes.Add("Line2", "Line2");
                            TrvNode.Nodes.Add("Line3", "Line3");
                            TrvNode.Nodes.Add("Line4", "Line4");
                            TrvNode.Nodes.Add("Line5", "Line5");
                            TrvNode.Nodes.Add("Line6", "Line6");
                            TrvNode.Nodes.Add("Line7", "Line7");
                            TrvNode.Nodes.Add("Line8", "Line8");
                            TrvNode.Nodes.Add("Line9", "Line9");
                            TrvNode.Nodes.Add("Line10", "Line10");

                            TrvNode = trvPrint.Nodes.Add("ItemDetails");

                            TrvNode.Nodes.Add("SlNo", "SlNo");
                            TrvNode.Nodes.Add("LAliasName", "LAliasName");
                            TrvNode.Nodes.Add("LName", "LName");
                            TrvNode.Nodes.Add("Qty", "Qty");
                            TrvNode.Nodes.Add("Amount", "Amount");
                            TrvNode.Nodes.Add("TaxPer", "TaxPer");
                            if (AppSettings.TaxMode == 2)
                            {
                                TrvNode.Nodes.Add("CGSTP", "CGSTP");
                                TrvNode.Nodes.Add("SGSTP", "SGSTP");
                                TrvNode.Nodes.Add("IGSTP", "IGSTP");
                                TrvNode.Nodes.Add("CGSTA", "CGSTA");
                                TrvNode.Nodes.Add("SGSTA", "SGSTA");
                                TrvNode.Nodes.Add("IGSTA", "IGSTA");
                            }
                            TrvNode.Nodes.Add("GrossVal", "GrossVal");
                            TrvNode.Nodes.Add("NetVal", "NetVal");
                            TrvNode.Nodes.Add("TaxAmt", "TaxAmt");
                            TrvNode.Nodes.Add("icessper", "icessper");
                            TrvNode.Nodes.Add("icessamt", "icessamt");
                            TrvNode.Nodes.Add("icompcessper", "icompcessper");
                            TrvNode.Nodes.Add("icompcessamt", "icompcessamt");


                            TrvNode = trvPrint.Nodes.Add("ItemDetails2");

                            TrvNode.Nodes.Add("SlNo2", "SlNo2");
                            TrvNode.Nodes.Add("LAliasName2", "LAliasName2");
                            TrvNode.Nodes.Add("LName2", "LName2");
                            TrvNode.Nodes.Add("Qty2", "Qty2");
                            TrvNode.Nodes.Add("Amount2", "Amount2");
                            TrvNode.Nodes.Add("TaxPer2", "TaxPer2");
                            if (AppSettings.TaxMode == 2)
                            {
                                TrvNode.Nodes.Add("CGSTP2", "CGSTP2");
                                TrvNode.Nodes.Add("SGSTP2", "SGSTP2");
                                TrvNode.Nodes.Add("IGSTP2", "IGSTP2");
                                TrvNode.Nodes.Add("CGSTA2", "CGSTA2");
                                TrvNode.Nodes.Add("SGSTA2", "SGSTA2");
                                TrvNode.Nodes.Add("IGSTA2", "IGSTA2");
                            }
                            TrvNode.Nodes.Add("GrossVal2", "GrossVal2");
                            TrvNode.Nodes.Add("NetVal2", "NetVal2");
                            TrvNode.Nodes.Add("TaxAmt2", "TaxAmt2");


                            TrvNode = trvPrint.Nodes.Add("ItemDetails3");

                            TrvNode.Nodes.Add("SlNo3", "SlNo3");
                            TrvNode.Nodes.Add("LAliasName3", "LAliasName3");
                            TrvNode.Nodes.Add("LName3", "LName3");
                            TrvNode.Nodes.Add("Qty3", "Qty3");
                            TrvNode.Nodes.Add("Amount3", "Amount3");
                            TrvNode.Nodes.Add("TaxPer3", "TaxPer3");
                            if (AppSettings.TaxMode == 2)
                            {
                                TrvNode.Nodes.Add("CGSTP3", "CGSTP3");
                                TrvNode.Nodes.Add("SGSTP3", "SGSTP3");
                                TrvNode.Nodes.Add("IGSTP3", "IGSTP3");
                                TrvNode.Nodes.Add("CGSTA3", "CGSTA3");
                                TrvNode.Nodes.Add("SGSTA3", "SGSTA3");
                                TrvNode.Nodes.Add("IGSTA3", "IGSTA3");
                            }
                            TrvNode.Nodes.Add("GrossVal3", "GrossVal3");
                            TrvNode.Nodes.Add("NetVal3", "NetVal3");
                            TrvNode.Nodes.Add("TaxAmt3", "TaxAmt3");

                            TrvNode = trvPrint.Nodes.Add("ReportFooter");

                            TrvNode.Nodes.Add("InvNoBarcodeF", "InvNoBarcodeF");
                            TrvNode.Nodes.Add("GrossAmt", "GrossAmt");
                            TrvNode.Nodes.Add("TaxAmt", "TaxAmt");
                            TrvNode.Nodes.Add("Taxable", "Taxable");
                            TrvNode.Nodes.Add("NonTaxable", "NonTaxable");
                            TrvNode.Nodes.Add("icessper", "icessper");
                            TrvNode.Nodes.Add("icessamt", "icessamt");
                            TrvNode.Nodes.Add("icompcessper", "icompcessper");
                            TrvNode.Nodes.Add("icompcessamt", "icompcessamt");
                            if (AppSettings.TaxMode == 2)
                            {
                                TrvNode.Nodes.Add("CGSTTotal", "CGSTTotal");
                                TrvNode.Nodes.Add("SGSTTotal", "SGSTTotal");
                                TrvNode.Nodes.Add("IGSTTotal", "IGSTTotal");
                            }

                            TrvNode.Nodes.Add("NetAmount", "NetAmount");
                            TrvNode.Nodes.Add("OtherExpense", "OtherExpense");
                            TrvNode.Nodes.Add("UserNarration", "UserNarration");

                            TrvNode.Nodes.Add("EffectiveDate", "EffectiveDate");




                            TrvNode.Nodes.Add("SlNoTot", "SlNoTot");
                            TrvNode.Nodes.Add("RefNoF", "RefNoF");
                            TrvNode.Nodes.Add("Outstanding", "Outstanding");
                            TrvNode.Nodes.Add("BillAmt", "BillAmt");
                            TrvNode.Nodes.Add("InWords", "InWords");
                            TrvNode.Nodes.Add("QtyTotal", "QtyTotal");
                            TrvNode.Nodes.Add("TaxSplit", "TaxSplit");
                            TrvNode.Nodes.Add("UserName", "UserName");
                            TrvNode.Nodes.Add("ChequeNumber", "ChequeNumber");
                            TrvNode.Nodes.Add("ChequeDate", "ChequeDate");
                            TrvNode.Nodes.Add("IssuingBank", "IssuingBank");
                            // ChequeDate,ChequeNumber,IssuingBank.ledgerName,LedgerCode
                            // "QtyTotal"

                            TrvNode.Nodes.Add("SignatureImage", "SignatureImage");
                            TrvNode.Nodes.Add("InvConditions", "InvConditions");
                            TrvNode.Nodes.Add("Narration", "Narration");
                            TrvNode.Nodes.Add("FooterLine", "FooterLine");
                            TrvNode.Nodes.Add("Box11", "Box11");
                            TrvNode.Nodes.Add("Box12", "Box12");
                            TrvNode.Nodes.Add("Box13", "Box13");
                            TrvNode.Nodes.Add("Box14", "Box14");
                            TrvNode.Nodes.Add("Box15", "Box15");
                            TrvNode.Nodes.Add("Box16", "Box16");
                            TrvNode.Nodes.Add("Box17", "Box17");
                            TrvNode.Nodes.Add("Box18", "Box18");
                            TrvNode.Nodes.Add("Box19", "Box19");
                            TrvNode.Nodes.Add("Box20", "Box20");

                            TrvNode.Nodes.Add("Line11", "Line12");
                            TrvNode.Nodes.Add("Line12", "Line12");
                            TrvNode.Nodes.Add("Line13", "Line13");
                            TrvNode.Nodes.Add("Line14", "Line14");
                            TrvNode.Nodes.Add("Line15", "Line15");
                            TrvNode.Nodes.Add("Line16", "Line16");
                            TrvNode.Nodes.Add("Line17", "Line17");
                            TrvNode.Nodes.Add("Line18", "Line18");
                            TrvNode.Nodes.Add("Line19", "Line19");
                            TrvNode.Nodes.Add("Line20", "Line20");
                            break;
                        }
                }



                TrvNode.ExpandAll();
                TrvNode.Expand();
                trvPrint.Nodes[0].EnsureVisible();
                trvPrint.Nodes[1].Expand();
                trvPrint.Nodes[0].ExpandAll();

                SetSaveString();
                // PRINTSETTING()

                int i;
                try
                {
                    cboPages.Items.Clear();
                }
                // MsgBox(cboPages.Items.Count)
                catch (Exception ex)
                {
                }
                try
                {
                    for (i = 0; i <= b1.GetUpperBound(0) - 1; i++)
                        cboPages.Items.Add("Pages" + i + 1);
                }
                catch (Exception ex)
                {
                }
                if (cboPages.Items.Count == 0)
                    cboPages.SelectedIndex = 0;


                return 0;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public void SaveInTreTagFromObjResize(bool BlnFromDirectControlEntry)
        {
            try
            {

                if (trvPrint.SelectedNode == null)
                    return;
                // Exit Sub
                if (tlpPrintTags.Enabled == false)
                    return;
                if (!(isMoving == true | Drag3 == true | Drag1 == true | Drag2 == true | Drag4 == true))
                    return;

                if (BlnFromDirectControlEntry == false)
                {
                    txtTop.Text = Comm.FormatValue((Comm.ToDouble(Sizer.Top) + Comm.ToDouble(TopScrollScale)) / Comm.ToDouble(MyScale), false, "#0.00");
                    txtLeft.Text = Comm.FormatValue(Comm.ToDouble(Sizer.Left) / Comm.ToDouble(MyScale), false, "#0.00");
                    txtheight.Text = Comm.FormatValue(Comm.ToDouble(Sizer.Height) / Comm.ToDouble(MyScale), false, "#0.00");
                    txtWidth.Text = Comm.FormatValue(Comm.ToDouble(Sizer.Width) / Comm.ToDouble(MyScale), false, "#0.00");
                    txtcaption.Text = Sizer.Text;
                }

                if (trvPrint.SelectedNode.Parent == null)
                    return;
                //TreeNode tnode = new TreeNode();
                //TreeNode Cnode = new TreeNode();
                TreeNode MyNode = new TreeNode();

                foreach (TreeNode tnode in trvPrint.Nodes)
                {
                    if (tnode.Nodes.Count > 0)
                    {
                        foreach (TreeNode Cnode in tnode.Nodes)
                        {
                            if (Cnode.Text == trvPrint.SelectedNode.Text)
                            {
                                MyNode = trvPrint.SelectedNode;
                                goto lbl;
                            }
                        }
                    }
                }

            lbl:
                string strTagString = "";
                // 0keyString,1Font,2Size,3Bold,4aLIGNMENT,5Top,6left,7Width,8height,9Checked,Caption.Text,10NoOfLines,11ItemHeight,ItemTop,shpfontcolorcnu

                if (txtfontname.Text == "")
                    txtfontname.Text = "Tahoma";
                if (Comm.ToDecimal(txtFontSize.Text) == 0) 
                    txtFontSize.Text = "8";
                if (txtFontStyle.Text == "")
                    txtFontStyle.Text = "Normal";

                string txtcaptionText;

                if (txtcaption.Text.Length == 0)
                    txtcaptionText = " "; // trvPrint.SelectedNode.Text
                else
                {
                    txtcaptionText = txtcaption.Text;
                    Sizer.Text = txtcaptionText;
                }

                string nodecheckstate = (trvPrint.SelectedNode.Checked == true) ? "1" : "0";
                string drawbox = (chkDrawBox.Checked == true) ? "1" : "0";
                strTagString = trvPrint.SelectedNode.Text + "£" + txtfontname.Text + "£" + Comm.ToDecimal(txtFontSize.Text) + "£" + txtFontSize.Text + "£" + cboAlignment.Text + "£" + Comm.ToDecimal(txtTop.Text) + "£" + Comm.ToDecimal(txtLeft.Text) + "£" + Comm.ToDecimal(txtWidth.Text) + "£" + Comm.ToDecimal(txtheight.Text) + "£" + nodecheckstate + "£" + txtcaptionText + "£" + txtItemLines.Text + "£" + txtItemHeight.Text + "£" + txtItemTop.Text + "£" + BtnFontColor.BackColor.Name + "£" + drawbox + "£";
                trvPrint.SelectedNode.Tag = strTagString;
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "SaveInTreTagFromObjResize", "Print Designer", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private TreeNode getNodeOnText(object NodeText)
        {
            try
            {
                foreach (TreeNode tnode in trvPrint.Nodes)
                {
                    if (tnode.Nodes.Count > 0)
                    {
                        foreach (TreeNode Cnode in tnode.Nodes)
                        {
                            if (Cnode.Text.ToUpper() == NodeText.ToString().ToUpper())
                            {
                                return Cnode;
                            }
                        }
                    }
                }
                return null/* TODO Change to default(_) if this is not a reference type */;
            }
            catch (Exception ex)
            {
                return null/* TODO Change to default(_) if this is not a reference type */;
            }
        }

        private bool DrawStringPrinter(string sText, float Ileft, float Itop, float Iwidth, double iHeight, string IfColor = "", string IbColor = "", string iAlignMent = "lEFT", string sFontName = "Tahoma", float SFontSize = 10, string MFontStyle = "Normal", bool BlnIsfooter = false)
        {
            if (sText.Trim() == "")
            {
                return true;
            }

            try
            {
                if (zoom == 0)
                    zoom = 1;
                Ileft = Ileft * PScale * zoom;
                Itop = Itop * PScale * zoom;
                Iwidth = Iwidth * PScale * zoom;
                iHeight = iHeight * PScale * zoom;
                // 10

                string[] STRSPLIT;
                //string strRetTemp;
                if (sText.Contains(Strings.Chr(13)) == true)
                    STRSPLIT = Strings.Split(sText, Strings.Chr(13).ToString());
                else
                    STRSPLIT = Strings.Split("1:", ":");

                // '''''''''''''''

                if (BlnIsfooter == true)
                {
                }

                if (MFontStyle == "Bold")
                {
                    gObjPrinter.FontBold = true;
                    gObjPrinter.FontItalic = false;
                    gObjPrinter.FontUnderline = false;
                }
                else if (MFontStyle == "Italic")
                {
                    gObjPrinter.FontBold = false;
                    gObjPrinter.FontItalic = true;
                    gObjPrinter.FontUnderline = false;
                }
                else if (MFontStyle == "BoldItalic")
                {
                    gObjPrinter.FontBold = true;
                    gObjPrinter.FontItalic = false;
                    gObjPrinter.FontUnderline = false;
                }
                else if (MFontStyle == "NormalUnderline")
                {
                    gObjPrinter.FontBold = false;
                    gObjPrinter.FontItalic = false;
                    gObjPrinter.FontUnderline = true;
                }
                else if (MFontStyle == "BoldUnderline")
                {
                    gObjPrinter.FontBold = true;
                    gObjPrinter.FontItalic = false;
                    gObjPrinter.FontUnderline = true;
                }
                else if (MFontStyle == "ItalicUnderline")
                {
                    gObjPrinter.FontBold = false;
                    gObjPrinter.FontItalic = true;
                    gObjPrinter.FontUnderline = true;
                }
                else
                {
                    gObjPrinter.FontBold = false;
                    gObjPrinter.FontItalic = false;
                    gObjPrinter.FontUnderline = false;
                }

                // ==============
                if (Strings.Trim(STRSPLIT[1]) == "")
                {
                    {
                        var withBlock = gObjPrinter;
                        if (Mblnvariable & BlnIsfooter & mblnpreview == true)
                            Itop = Itop + ((footerdiff * PScale * MyScale));

                        gObjPrinter.CurrentY = Itop;
                        if (Strings.Trim(sFontName) != "")
                            gObjPrinter.FontName = sFontName;
                        if (Conversion.Val(SFontSize) > 4)
                            gObjPrinter.FontSize = SFontSize * zoom;

                        // ====================================================================
                        if (iAlignMent == "Centre")
                            gObjPrinter.CurrentX = Ileft + ((Iwidth / 2) - (gObjPrinter.TextWidth(sText) / 2));
                        else if (iAlignMent == "Right")
                            gObjPrinter.CurrentX = ((Ileft + Iwidth) - gObjPrinter.TextWidth(sText)) - 20;
                        else
                            gObjPrinter.CurrentX = 10 + Ileft;

                        // If IfColor = "" Then IfColor = Color.Black.ToString
                        // gObjPrinter.ForeColor = Color.Black.ToArgb
                        // If IbColor <> 0 Then gObjPrinter.BackColor = IbColor
                        gObjPrinter.Print(sText);
                    }
                }
                else
                {
                    // If MblnVariable And BlnFooter And Itop - (FotterDiff * 10 * Zoom) > 1 And MblnPreview Then
                    // Itop = Itop - (FotterDiff * 10 * Zoom)
                    // End If
                    if (Mblnvariable & BlnIsfooter & mblnpreview == true)
                        Itop = Itop + ((footerdiff * PScale * MyScale));

                    gObjPrinter.CurrentY = Itop;

                    if (Strings.Trim(sFontName) != "")
                        gObjPrinter.FontName = sFontName;
                    if (Conversion.Val(SFontSize) > 4)
                        gObjPrinter.FontSize = SFontSize * zoom;

                    for (var i = 0; i <= Information.UBound(STRSPLIT); i++)
                    {
                        sText = Strings.Replace(STRSPLIT[i], Strings.Chr(10).ToString(), "");
                        sText = Strings.Replace(sText, Strings.Chr(13).ToString(), "");
                        if (sText != null)
                        {
                            gObjPrinter.CurrentY = Itop;
                            if (iAlignMent == "Centre")
                                gObjPrinter.CurrentX = Ileft + ((Iwidth / 2) - (gObjPrinter.TextWidth(sText) / 2));
                            else if (iAlignMent == "Right")
                                gObjPrinter.CurrentX = ((Ileft + Iwidth) - gObjPrinter.TextWidth(sText));
                            else
                                gObjPrinter.CurrentX = Ileft;
                            // gObjPrinter.ForeColor = IfColor
                            // If IbColor <> 0 Then gObjPrinter.BackColor = IbColor
                            // gObjPrinter.ForeColor = Color.Black.ToArgb
                            gObjPrinter.Print(sText);
                            Itop = Itop + gObjPrinter.TextHeight(sText);
                        }
                    }
                }
                
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        private void DrawString(string sText, float Ileft, float Itop, float Iwidth, float iHeight, string IfColor = "", string IbColor = "", string iAlignMent = "lEFT", string sFontName = "Tahoma", float SFontSize = 10, string MFontStyle = "Normal", bool BlnIsfooter = false)
        {
            // Printing to the graphics buffer (not form)
            // updates the associated bitmap b1
            // On Error GoTo err
            if (blnSendToprinter)
            {
                // MsgBox(sText)
                DrawStringPrinter(sText, Ileft, Itop, Iwidth, iHeight, IfColor, IbColor, iAlignMent, sFontName, SFontSize, MFontStyle, BlnIsfooter);
                return;
            }

            if (BlnIsfooter)
            {
            }


            Font mainFont;
            FontStyle fs;
            if (MFontStyle == "Bold")
                fs = FontStyle.Bold;
            else if (MFontStyle == "Italic")
                fs = FontStyle.Italic;
            else if (MFontStyle == "BoldItalic")
            {
                fs = FontStyle.Italic;
                fs = FontStyle.Bold;
            }
            else if (MFontStyle == "NormalUnderline")
            {
                fs = FontStyle.Regular;
                fs = FontStyle.Underline;
            }
            else if (MFontStyle == "BoldUnderline")
            {
                fs = FontStyle.Regular;
                fs = FontStyle.Underline;
                fs = FontStyle.Bold;
            }
            else if (MFontStyle == "ItalicUnderline")
            {
                fs = FontStyle.Italic;
                fs = FontStyle.Underline;
            }
            else
                fs = FontStyle.Regular;


            mainFont = new Font(sFontName, SFontSize, fs);

            if (Mblnvariable & BlnIsfooter & mblnpreview == true)
                Itop = Itop + ((footerdiff) * MyScale);


            // Dim textArea As Rectangle = New Rectangle(CInt(Ileft - 5), CInt(Itop - 5), CInt(Iwidth), CInt(iHeight))
            Rectangle textArea = new Rectangle(System.Convert.ToInt32(Ileft - 1), System.Convert.ToInt32(Itop - 1), System.Convert.ToInt32(Iwidth), System.Convert.ToInt32(iHeight));
            StringFormat textFormat = new StringFormat();

            if (Strings.UCase(iAlignMent) == "")
                iAlignMent = "LEFT";
            if (Strings.UCase(iAlignMent) == "LEFT")
                textFormat.Alignment = StringAlignment.Near;
            else if (Strings.UCase(iAlignMent) == "CENTRE")
                textFormat.Alignment = StringAlignment.Center;
            else if (Strings.UCase(iAlignMent) == "RIGHT")
                textFormat.Alignment = StringAlignment.Far;
            // g1.TextContrast = 0
            // g1.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault
            g1.PageScale = (float)System.Drawing.Drawing2D.CompositingQuality.HighQuality;

            g1.DrawRectangle(Pens.Transparent, textArea);
            g1.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            g1.DrawString(sText, mainFont, Brushes.Black, textArea, textFormat);

            // DrawBox(Ileft, Itop, Iwidth, iHeight, Color.Aquamarine.ToString, Color.BlanchedAlmond.ToString, iAlignMent)
            // Copy the bitmap to the form
            // b1(ActivePageNo)
            this.picDocument.CreateGraphics().DrawImage(b1[ActivePageNo], 0, 0);
            // MsgBox(sText)
            return;
        }
        private void drawBarcode(string Input, float Ileft, float Itop, float Iwidth, float iHeight, bool blnisfooter = false)
        {
            try
            {
                // converting 
                Input = GenerateBINARY(Input);



                int num = 0;
                foreach (char one in Input)
                    num = num + 1;

                if (iHeight == 0)
                    iHeight = 1;
                if (Iwidth == 0)
                    Iwidth = 2;
                Rectangle rec = new Rectangle(1, 1, num, System.Convert.ToInt32(iHeight));
                Bitmap img = new Bitmap(num, Convert.ToInt32(System.Convert.ToInt32(iHeight)));



                int count = 0;
                int length = 0;

                Pen aBlackPen = new Pen(Color.Black);
                Pen aWhitePen = new Pen(Color.White);

                aBlackPen.Width = 3;
                aWhitePen.Width = 3;

                length = length + System.Convert.ToInt32(iHeight);

                /// -----------------
                Bitmap bm = new Bitmap(System.Convert.ToInt32(Iwidth), System.Convert.ToInt32(iHeight));
                Graphics graph = Graphics.FromImage(bm);
                graph.Clear(Color.Transparent);
                /// --------------




                foreach (char item in Input)
                {
                    count = count + 1;
                    if (item == 1)
                        graph.DrawLine(aBlackPen, count, 1, count, System.Convert.ToInt32(length));
                    else
                        graph.DrawLine(aWhitePen, count, 1, count, System.Convert.ToInt32(length));
                }

                if (Mblnvariable & blnisfooter & mblnpreview == true)
                    Itop = Itop + (footerdiff * MyScale);


                if (blnSendToprinter)
                {
                    gObjPrinter.PaintPicture(bm, Ileft * PScale, Itop * PScale, Iwidth * PScale, iHeight * PScale);
                    return;
                }

                Rectangle destRect = new Rectangle(System.Convert.ToInt32(Ileft), System.Convert.ToInt32(Itop), System.Convert.ToInt32(Iwidth), System.Convert.ToInt32(iHeight));

                // Draw image to screen.
                g1.DrawImage(bm, destRect);
                this.picDocument.CreateGraphics().DrawImage(b1[ActivePageNo], 0, 0);
            }
            catch (Exception ex)
            {
                Interaction.MsgBox(ex.Message + "drawBarcode");
            }
        }
        private void DrawImage(float Ileft, float Itop, float Iwidth, float iHeight, string IfColor = "", string IbColor = "", string iAlignMent = "Normal", string IMagepath = "", bool BlnIsfooter = false)
        {
            // Create image.

            if (File.Exists(IMagepath))
            {
            }
            else
                return;


            if (Mblnvariable & BlnIsfooter & mblnpreview == true)
                Itop = Itop + ((footerdiff) * MyScale);

            Image newImage = Image.FromFile(IMagepath);

            // Create rectangle for displaying image.
            Rectangle destRect = new Rectangle(System.Convert.ToInt32(Ileft), System.Convert.ToInt32(Itop), System.Convert.ToInt32(Iwidth), System.Convert.ToInt32(iHeight));

            if (blnSendToprinter)
            {
                gObjPrinter.PaintPicture(newImage, Ileft * PScale, Itop * PScale, Iwidth * PScale, iHeight * PScale);
                return;
            }

            // Draw image to screen.
            g1.DrawImage(newImage, destRect);
            this.picDocument.CreateGraphics().DrawImage(b1[ActivePageNo], 0, 0);
        }
        private void drawQrCode(string Input, float Ileft, float Itop, float Iwidth, float iHeight, bool blnIsFooter = false)
        {
            try
            {
                if (Input == "")
                    return;

                // =================================================================================
                QrEncoder objqrcodeenc = new QrEncoder();
                Gma.QrCodeNet.Encoding.QrCode objQrCode = new Gma.QrCodeNet.Encoding.QrCode();

                Image imgimage;
                Bitmap objbitmap;
                string s;
                s = Input;

                byte[] byt = System.Text.Encoding.UTF8.GetBytes(s);
                s = Convert.ToBase64String(byt).ToString();

                objqrcodeenc.TryEncode(s, out objQrCode);


                GraphicsRenderer renderer = new GraphicsRenderer(new FixedCodeSize(400, QuietZoneModules.Zero), Brushes.Black, Brushes.White);

                MemoryStream MS = new MemoryStream();
                renderer.WriteToStream(objQrCode.Matrix, ImageFormat.Png, MS);
                imgimage = new Bitmap(MS);
                objbitmap = new Bitmap(imgimage, new Size(new Point(200, 200)));
                objbitmap.Save("QRCode.jpg", ImageFormat.Bmp);

                // objQrCode.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE
                // objQrCode.QRCodeScale = 3
                // objqrcode.QRCodeVersion = 6
                // objqrcode.QRCodeErrorCorrect = ThoughtWorks.QRCode.Codec.QRCodeEncoder.ERROR_CORRECTION.L
                // imgimage = objqrcode.Encode(Mid(s, 1, 130))
                // objbitmap = New Bitmap(imgimage)
                // objbitmap.Save("QRCode.jpg")

                if (Mblnvariable & blnIsFooter & mblnpreview == true)
                    Itop = Itop + ((footerdiff) * MyScale);

                Rectangle destRect = new Rectangle(System.Convert.ToInt32(Ileft), System.Convert.ToInt32(Itop), System.Convert.ToInt32(Iwidth), System.Convert.ToInt32(iHeight));


                if (blnSendToprinter)
                {
                    gObjPrinter.PaintPicture(objbitmap, Ileft * PScale, Itop * PScale, Iwidth * PScale, iHeight * PScale);
                    return;
                }

                // Draw image to screen.
                g1.DrawImage(objbitmap, destRect);
            }

            // Pimage.ImageLocation = "QRCode.jpg"
            catch (Exception ex)
            {
                Interaction.MsgBox(ex.Message, MsgBoxStyle.Information);
            }
        }
        private void drawQrCodeRaw(string Input, float Ileft, float Itop, float Iwidth, float iHeight, bool blnIsFooter = false)
        {
            try
            {
                if (Input == "")
                    return;

                QRCodeGenerator qrGenerator = new QRCodeGenerator();

                string s;
                s = Input;

                QRCodeData QRCodeData = qrGenerator.CreateQrCode(s, QRCodeGenerator.ECCLevel.Q);
                QRCoder.QRCode QrCode = new QRCoder.QRCode(QRCodeData);
                Bitmap QRCodeImage = QrCode.GetGraphic(60);
                // Dim QrCode As QRCoder.SvgQRCode = New QRCoder.SvgQRCode(QRCodeData)
                // Dim qrCodeAsSvg As String = QrCode.GetGraphic(20)
                // QRCodeImage
                // =================================================================================
                GraphicsRenderer renderer = new GraphicsRenderer(new FixedCodeSize(400, QuietZoneModules.Zero), Brushes.Black, Brushes.White);

                // Dim MS As MemoryStream = New MemoryStream()
                QRCodeImage.Save("QRCode.jpg", ImageFormat.Bmp);

                if (Mblnvariable & blnIsFooter & mblnpreview == true)
                    Itop = Itop + ((footerdiff) * MyScale);

                Rectangle destRect = new Rectangle(System.Convert.ToInt32(Ileft), System.Convert.ToInt32(Itop), System.Convert.ToInt32(Iwidth), System.Convert.ToInt32(iHeight));

                if (blnSendToprinter)
                {
                    gObjPrinter.PaintPicture(QRCodeImage, Ileft * PScale, Itop * PScale, Iwidth * PScale, iHeight * PScale);
                    return;
                }

                // Draw image to screen.
                g1.DrawImage(QRCodeImage, destRect);
            }

            // Pimage.ImageLocation = "QRCode.jpg"
            catch (Exception ex)
            {
                Interaction.MsgBox(ex.Message, MsgBoxStyle.Information);
            }
        }
        private bool DrawBoxPrinter(double Ileft, double Itop, double Iwidth, double iHeight, string IfColor = "", string IbColor = "", string iAlignMent = "Normal", string sFontName = "Tahoma", float SFontSize = 10, bool BlnBold = false, bool blnisfooter = false)
        {
            if (Convert.ToInt32(txtlineThickness.Text) < 1)
            {
                return true;
            }
            if (Comm.ToInt32(txtlineThickness.Text) < 1)
                gObjPrinter.DrawWidth = 1;
            else
                gObjPrinter.DrawWidth = (short)(Convert.ToInt32(txtlineThickness.Text) + 1);

            if (Iwidth == 0)
                Iwidth = Ileft;

            if (Mblnvariable & blnisfooter & mblnpreview == true)
                Itop = Itop + (footerdiff * MyScale);

            // gObjPrinter.Line (((Ileft ), (Itop * 1440 * Zoom))-((Iwidth * 1440 * Zoom), ((iHeight + Itop) * 1440 * Zoom)), , B)
            gObjPrinter.Line((float)(Comm.ToDecimal(Ileft * PScale)), (float)(Comm.ToDecimal(Itop * PScale)), (float)(Comm.ToDecimal((Iwidth + Ileft) * PScale)), (float)(Comm.ToDecimal((iHeight + Itop) * PScale)), -1, true, false);

            // gObjPrinter.Line(720,720,

            return true;

        //err:
        //    Interaction.MsgBox(Information.Err.Description + "Draw Box Printer", Constants.vbExclamation);

        //    return false;
        }
        private void DrawBox(double Ileft, double Itop, double Iwidth, double iHeight, string IfColor = "", string IbColor = "", string iAlignMent = "Normal", string sFontName = "Tahoma", float SFontSize = 10, bool BlnBold = false, bool blnisfooter = false)
        {
            // Printing to the graphics buffer (not form)
            // updates the associated bitmap b1
            // it paints my rectangle as I would expect drawing it on Form1 at 0,0   'with width 50 and height 10.

            if (Comm.ToDecimal(txtlineThickness.Text) == 0)
                return;

            if (blnSendToprinter)
            {
                DrawBoxPrinter(Ileft, Itop, Iwidth, iHeight, IfColor, IbColor, iAlignMent, sFontName, SFontSize, BlnBold, blnisfooter);
                return;
            }

            try
            {
                if (Mblnvariable & blnisfooter & mblnpreview == true)
                    Itop = Itop + (footerdiff * MyScale);



                if (IfColor != "")
                {
                    var aPen = new Pen(Color.FromName(IfColor));
                    aPen.Width = (float)(Comm.ToDecimal(txtlineThickness.Text));
                    g1.DrawRectangle(aPen, System.Convert.ToInt32(Ileft), System.Convert.ToInt32(Itop), System.Convert.ToInt32(Iwidth), System.Convert.ToInt32(iHeight));
                }
                else
                {
                    var aPen = new Pen(Color.Black);
                    aPen.Width = (float)(Comm.ToDecimal(txtlineThickness.Text));
                    g1.DrawRectangle(aPen, System.Convert.ToInt32(Ileft), System.Convert.ToInt32(Itop), System.Convert.ToInt32(Iwidth), System.Convert.ToInt32(iHeight));
                }



                // g1.DrawString(sText, New Font(sFontName, SFontSize, FontStyle.Bold), Brushes.Black, Ileft, Itop - 10)
                // Copy the bitmap to the form

                this.picDocument.CreateGraphics().DrawImage(b1[ActivePageNo], 0, 0);
            }
            catch (Exception ex)
            {
            }
        }
        public bool drawLinePrinter(float Ileft, float Itop, float iHeight, float Iwidth = 0, string Icolor = "", bool blnisfooter = false)
        {
            // (Left,Top)-(Left,Height+Top)
            // On Error GoTo err
            if (Comm.ToInt32(txtlineThickness.Text) < 1)
            {
                return true;
            }
            if (Comm.ToInt32(txtlineThickness.Text) < 1)
                gObjPrinter.DrawWidth = 1;
            else
                gObjPrinter.DrawWidth = (short)(Comm.ToInt32(txtlineThickness.Text));

            if (Mblnvariable & blnisfooter & mblnpreview == true)
                Itop = Itop + (footerdiff * MyScale);

            if (iHeight > 1)
            {
                gObjPrinter.Line(Ileft * PScale, Itop * PScale, Ileft * PScale, iHeight * PScale, null/* Conversion error: Set to default value for this argument */, false, false);
            }
            else
                gObjPrinter.Line(Ileft * PScale, Itop * PScale, Iwidth * PScale, Itop * PScale, null/* Conversion error: Set to default value for this argument */, false, false);

            return true;
        //err:
        //    ;
        //    drawLinePrinter = false;
        //    Interaction.MsgBox(Information.Err.Description + "draw Line Printer", Constants.vbExclamation);
        }
        private void DrawLine(float Ileft, float Itop, float Iwidth, float iHeight, string IfColor = "", string IbColor = "", bool blnisfooter = false)
        {
            // Printing to the graphics buffer (not form)
            // updates the associated bitmap b1
            // it paints my rectangle as I would expect drawing it on Form1 at 0,0   'with width 50 and height 10.
            if (Comm.ToInt32(txtlineThickness.Text) == 0)
                return;
            if (blnSendToprinter)
            {
                drawLinePrinter(Ileft, Itop, iHeight, Iwidth, "", blnisfooter);
                return;
            }

            if (Mblnvariable & blnisfooter & mblnpreview == true)
                Itop = Itop + (footerdiff * MyScale);

            var aPen = new Pen(Color.Black);
            aPen.Width = Comm.ToInt32(txtlineThickness.Text);

            // On Error GoTo err
            if (Iwidth > 1)
                g1.DrawLine(aPen, System.Convert.ToInt32(Ileft), System.Convert.ToInt32(Itop), System.Convert.ToInt32(Iwidth), System.Convert.ToInt32(Itop));
            else
                g1.DrawLine(aPen, System.Convert.ToInt32(Ileft), System.Convert.ToInt32(Itop), System.Convert.ToInt32(Ileft), System.Convert.ToInt32(iHeight));

            // Copy the bitmap to the form
            this.picDocument.CreateGraphics().DrawImage(b1[ActivePageNo], 0, 0);
            return;
        }
        private void PrintParagraph(string strParagraph, ref Graphics g)
        {
            Font f = new Font("Verdana", 24);
            int i;
            Point pos = new Point(0, 0);
            Color col = new Color();
            SizeF WordSize = new SizeF();
            string[] s = Strings.Split(strParagraph, " ");
            for (i = 0; i <= Information.UBound(s); i++)
            {
                col = Color.FromArgb(System.Convert.ToInt32(VBMath.Rnd() * 255), System.Convert.ToInt32(VBMath.Rnd() * 255), System.Convert.ToInt32(VBMath.Rnd() * 255));
                WordSize = g.MeasureString(s[i], f);
                if ((WordSize.Width + pos.X) > 850)
                {
                    pos.X = 0;
                    pos.Y = pos.Y + System.Convert.ToInt32(WordSize.Height);
                }
                g.DrawString(s[i], f, new SolidBrush(col), pos.X, pos.Y);
                pos.X = pos.X + System.Convert.ToInt32(WordSize.Width);
            }
        }
        private void Sizer_LocationChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (Sizer.Focused)
                    SaveInTreTagFromObjResize(true);
            }
            catch (Exception ex)
            {
                Interaction.MsgBox(ex.Message + "Sizer_LocationChanged", MsgBoxStyle.Exclamation);
            }
        }
        private void trvPrint_NodeMouseClick(object sender, System.Windows.Forms.TreeNodeMouseClickEventArgs e)
        {
            // 'declare a copy of your subclass
            myTreeNode mynode;
            // 'cast the incoming node into your subclass
            // 'mynode = CType(trvPrint.SelectedNode, myTreeNode)
            // 'do whatever you want with your subclass fields
            // 'MsgBox("Node selected is: " & mynode.MyStrng & vbCrLf & _
            // '    "With a value of: " & CType(mynode.MyIntg, String) & vbCrLf & _
            // '    "showing as: " & mynode.MyShowText)
            // 'Sizer.Visible = False
            tlpPrintTags.Enabled = false;
            e.Node.TreeView.SelectedNode = e.Node;
            string[] SplitVar;
            if (e.Node.Checked == true)
            {


                // 'sizer.Show()

                txtcaption.Text = "";
                txtfontname.Text = "Tahoma";
                txtFontSize.Text = "8";
                txtFontStyle.Text = "Normal";
                txtTop.Text = "1";
                txtLeft.Text = "1";
                txtWidth.Text = "1";
                txtheight.Text = "0.25";

                string strtag = trvPrint.SelectedNode.Tag.ToString();

                SplitVar = Strings.Split(strtag, "£");
                if (Information.UBound(SplitVar) == 0)
                {
                    double txtLeftText = 1;
                    double txtTopText = 1;
                    double txtheightText = 0.25;
                    double txtwidthText = 1;
                    trvPrint.SelectedNode.Tag = trvPrint.SelectedNode.Text + "£" + txtfontname.Text + "£" + txtFontSize.Text + "£" + txtFontStyle.Text + "£" + cboAlignment.Text + "£" + txtTopText.ToString() + "£" + txtLeftText.ToString() + "£" + txtwidthText.ToString() + "£" + txtheightText.ToString() + "£" + (trvPrint.SelectedNode.Checked == true ? 1 : 0).ToString() + "£" + trvPrint.SelectedNode.Text + "£" + txtItemLines.Text + "£" + txtItemHeight.Text + "£" + txtItemTop.Text + "£" + BtnFontColor.BackColor.Name + "£0£";
                    tlpPrintTags.Enabled = true;
                    return;
                }

                string[] separator = { "£" };
                SplitVar = e.Node.Tag.ToString().Split(separator, StringSplitOptions.None);
                if (Information.UBound(SplitVar) > 0)
                {
                    // '0keyString,1Font,2Size,3Bold,4aLIGNMENT,5Top,6left,7Width,8height,9Checked,Caption.Text,10NoOfLines,11ItemHeight,ItemTop,shpfontcolorcnu
                    txtcaption.Text = Strings.Trim(SplitVar[10]) == "" ? SplitVar[10] : SplitVar[10];
                    Sizer.Text = Strings.Trim(SplitVar[10]) == "" ? SplitVar[10] : SplitVar[10];
                    txtfontname.Text = SplitVar[1];
                    txtFontSize.Text = SplitVar[2];
                    txtFontStyle.Text = SplitVar[3];

                    // If Val(SplitVar(5)) > Val(txtPaperHeight.Text) Then
                    // txtTop.Text = Val(txtPaperHeight.Text) / 2
                    // Else
                    txtTop.Text = SplitVar[5];
                    // End If
                    // ===========
                    if (Comm.ToDecimal(SplitVar[6]) > Comm.ToDecimal(txtPaperWidth.Text))
                        txtLeft.Text = (Comm.ToDecimal(txtPaperWidth.Text) / 2).ToString();
                    else
                        txtLeft.Text = SplitVar[6];

                    txtWidth.Text = SplitVar[7];
                    txtheight.Text = SplitVar[8];
                    cboAlignment.Text = Strings.Trim(SplitVar[4]);
                    if ((Information.UBound(SplitVar)) > 15)
                    {
                        if (Conversion.Val(SplitVar[15]) == 1)
                            chkDrawBox.Checked = true;
                        else
                            chkDrawBox.Checked = false;
                    }

                    double Mleft = Comm.ToDouble(txtLeft.Text);
                    double Mheight = Comm.ToDouble(txtheight.Text);
                    double MWidth = Comm.ToDouble(txtWidth.Text);
                    double Mtop = Comm.ToDouble(txtTop.Text);
                    string MFont = txtfontname.Text;
                    decimal MFSize = Comm.ToDecimal(txtFontSize.Text);
                    string MFontStyle = txtFontStyle.Text.Trim();
                    string MAlignment = cboAlignment.Text.ToUpper();


                    if (MFSize <= 0)
                        MFSize = 2;

                    if (Comm.ToDecimal(txtFontSize.Text) < 2)
                        txtFontSize.Text = "8";
                    FontStyle fs;


                    if (MFontStyle == "Bold")
                        fs = FontStyle.Bold;
                    else if (MFontStyle == "Italic")
                        fs = FontStyle.Italic;
                    else if (MFontStyle == "BoldItalic")
                    {
                        fs = FontStyle.Italic;
                        fs = FontStyle.Bold;
                    }
                    else if (MFontStyle == "NormalUnderline")
                    {
                        fs = FontStyle.Regular;
                        fs = FontStyle.Underline;
                    }
                    else if (MFontStyle == "BoldUnderline")
                    {
                        fs = FontStyle.Bold;
                        fs = FontStyle.Underline;
                    }
                    else if (MFontStyle == "ItalicUnderline")
                    {
                        fs = FontStyle.Italic;
                        fs = FontStyle.Underline;
                    }
                    else
                        fs = FontStyle.Regular;

                    Sizer.Font = new Font(MFont, (float)MFSize, fs);
                    Sizer.Text = txtcaption.Text;

                    if (MAlignment == "LEFT")
                        Sizer.TextAlign = HorizontalAlignment.Left;
                    else if (MAlignment == "CENTRE")
                        Sizer.TextAlign = HorizontalAlignment.Center;
                    else if (MAlignment == "RIGHT")
                        Sizer.TextAlign = HorizontalAlignment.Right;
                    try
                    {
                        if (trvPrint.SelectedNode.Parent.Text == "ItemDetails")
                            Mtop = Comm.ToDouble(txtItemTop.Text) * MyScale;
                        else
                            Mtop = Comm.ToDouble(txtTop.Text) * MyScale;
                    }
                    catch (Exception ex)
                    {
                    }

                    tlpPrintTags.Enabled = true;
                    if (this.IsDisposed == false)
                        Application.DoEvents();
                    SetTextContrl(Comm.ToDecimal(txtWidth.Text) * MyScale, Comm.ToDecimal(txtheight.Text) * MyScale, Comm.ToDecimal(txtLeft.Text) * MyScale, Mtop);
                    tlpPrintTags.Enabled = false;
                }
            }
            else
            {
                string strtag = trvPrint.SelectedNode.Tag.ToString();

                if (strtag == null)
                    strtag = "";
                try
                {
                    if (strtag.Contains("£"))
                    {
                    }
                    else
                    {
                        txtcaption.Text = "";
                        txtfontname.Text = "Tahoma";
                        txtFontSize.Text = "8";
                        txtFontStyle.Text = "Normal";
                        txtTop.Text = "1";
                        txtLeft.Text = "1";
                        txtWidth.Text = "1";
                        txtheight.Text = "0.25";
                    }
                }
                catch (Exception ex)
                {
                }
                isMoving = true;
                SetActive(Sizer, false);
                tlpPrintTags.Enabled = true;
                SaveInTreTagFromObjResize(true);

                return;
            }
            isMoving = true;
            SaveInTreTagFromObjResize(true);
            isMoving = false;
            if (txtcaption.Text.Trim() == "")
            {
            }
            // 'Sizer.Text = txtcaption.text
            // 'Dim fontSam As New Font("Verdana", 12, FontStyle.Bold)
            // 'Sizer.Font = fontSam
            tlpPrintTags.Enabled = true;
        }









    }
}
