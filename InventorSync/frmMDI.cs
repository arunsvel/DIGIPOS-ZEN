using DigiposZen.Forms;
using DigiposZen.Forms;
using DigiposZen.InventorBL.Helper;
using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace DigiposZen
{
    public partial class frmMDI : Form
    {
        public frmMDI()
        {
            InitializeComponent();

            var frm1 = new Login();
            frm1.ShowDialog();

            Global.SetTenantID(1);

            Common Comm = new Common();

            Comm.LoadAppSettings();

            LoadTransMenu();

        }

        private void LoadTransMenu()
        {
            Common Comm = new Common();
            tsmTransactions.DropDownItems.Clear();

            DataTable dtTreeView = new DataTable();
            ToolStripMenuItem parentNode = new ToolStripMenuItem();

            dtTreeView = Comm.fnGetData("SELECT VchTypeID,VchType,ParentID,SystemName,UserID,LastUpdateDate,LastUpdateTime,TenantID FROM tblVchType WHERE ((VchTypeID BETWEEN 1 and 500) OR (VchTypeID >= 1005)) AND ParentID=VchTypeID AND isnull(ActiveStatus,0)=1 ").Tables[0];
            if (dtTreeView.Rows.Count > 0)
            {
                foreach (DataRow dr in dtTreeView.Rows)
                {
                    ToolStripMenuItem trans = new ToolStripMenuItem(); // = new ToolStripItem
                    
                    trans.Text = dr["VchType"].ToString();
                    trans.Tag = Convert.ToInt32(dr["VchTypeID"].ToString());
                    trans.Click += new EventHandler(MenuItemClickHandler);

                    DataTable dtgetData = new DataTable();
                    dtgetData = Comm.fnGetData("SELECT VchTypeID,VchType,ParentID,SystemName,UserID,LastUpdateDate,LastUpdateTime,TenantID FROM tblVchType WHERE ((VchTypeID BETWEEN 1 and 500) OR (VchTypeID >= 1005)) AND (ParentID <> VchTypeID) AND isnull(ActiveStatus,0)=1 AND ParentID =" + dr["VchTypeID"].ToString() + " ORDER BY VchTypeID Desc").Tables[0];

                    foreach (DataRow dr1 in dtgetData.Rows)
                    {
                        ToolStripMenuItem submenu = new ToolStripMenuItem();

                        submenu.Text = dr1["VchType"].ToString();
                        submenu.Tag = Convert.ToInt32(dr1["VchTypeID"].ToString());
                        submenu.Click += new EventHandler(MenuItemClickHandler);

                        trans.DropDownItems.Add(submenu);
                    }

                    tsmTransactions.DropDownItems.Add(trans);
                }
            }

            void MenuItemClickHandler(object sender, EventArgs e)
            {
                ToolStripMenuItem clickedItem = (ToolStripMenuItem)sender;
                // Take some action based on the data in clickedItem


                OpenMenu("", Convert.ToInt32(clickedItem.Tag.ToString()));


            }
        }

        private void MDI_Load(object sender, EventArgs e)
        {
            //this.WindowState = FormWindowState.Maximized;
            frmConnectionProperties frmcn = new frmConnectionProperties();
        }

        private void brandToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem t = (ToolStripMenuItem)sender;
            OpenMenu(t.Text.ToString(), 0);
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {

        }

        private void itemMasterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem t = (ToolStripMenuItem)sender;
            OpenMenu(t.Text.ToString(), 0);
        }

        private Boolean GetFromItemSearch(string sReturn)
        {

            return true;
        }

        private void OpenMenu(string MenuName, Int32 VchtypeID)
        {
            MenuName = MenuName.Replace("&", "").Replace(" ", "").ToString().ToUpper();

            String sQuery = "";

            switch (MenuName)
            {
                //case "CONNECTIONDETAILS":
                //    frmAccountStatements frmACC = new frmAccountStatements("DAYBOOK", this);
                //    frmACC.Text = "DAYBOOK";
                //    frmACC.MdiParent = this;
                //    frmACC.Show();
                //    frmACC.Focus();
                //    frmACC.BringToFront();
                //    break;

                case "DAYBOOK":
                    frmAccountStatements frmACC = new frmAccountStatements("DAYBOOK", this);
                    frmACC.Text = "DAYBOOK";
                    frmACC.MdiParent = this;
                    frmACC.Show();
                    frmACC.Focus();
                    frmACC.BringToFront();
                    break;

                case "DAYBOOKSUMMARY":
                    frmAccountStatements frmDBS = new frmAccountStatements("DAYBOOKSUMMARY", this);
                    frmDBS.Text = "DAYBOOK SUMMARY";
                    frmDBS.MdiParent = this;
                    frmDBS.Show();
                    frmDBS.Focus();
                    frmDBS.BringToFront();
                    break;

                case "TRIALBALANCE":
                    frmAccountStatements frmTB = new frmAccountStatements("TRIALBALANCE", this);
                    frmTB.Text = "TRIAL BALANCE";
                    frmTB.MdiParent = this;
                    frmTB.Show();
                    frmTB.Focus();
                    frmTB.BringToFront();
                    break;

                case "PROFITLOSS":
                    frmAccountStatements frmPL = new frmAccountStatements("PROFITLOSS", this);
                    frmPL.Text = "PROFIT AND LOSS";
                    frmPL.MdiParent = this;
                    frmPL.Show();
                    frmPL.Focus();
                    frmPL.BringToFront();
                    break;

                case "BALANCESHEET":
                    frmAccountStatements frmBS = new frmAccountStatements("BALANCESHEET", this);
                    frmBS.Text = "BALANCE SHEET";
                    frmBS.MdiParent = this;
                    frmBS.Show();
                    frmBS.Focus();
                    frmBS.BringToFront();
                    break;

                case "GSTR1":
                    frmGstReport frmGSTR1 = new frmGstReport(this);
                    frmGSTR1.Text = "GSTR1";
                    frmGSTR1.MdiParent = this;
                    frmGSTR1.Show();
                    frmGSTR1.Focus();
                    frmGSTR1.BringToFront();
                    break;

                case "CREATECOMPANY":
                    frmCompanySettings frmComp = new frmCompanySettings();
                    frmComp.Text = "CREATE COMPANY";
                    frmComp.MdiParent = this;
                    frmComp.Show();
                    frmComp.Focus();
                    frmComp.BringToFront();
                    break;

                case "DASHBOARD":
                    frmDashBoard frmDB = new frmDashBoard();
                    frmDB.Text = "DASH BOARD";
                    frmDB.MdiParent = this;
                    frmDB.Show();
                    frmDB.Focus();
                    frmDB.BringToFront();
                    break;

                case "ITEMVIEW":
                    sQuery = "SELECT ItemCode + ItemName + BatchUnique + CAST(MRP AS varchar) AS anywhere, ItemCode, ItemName, BatchUnique, QOH, MRP, ItemID, StockID " +
                            " FROM     vwCompactSearch Where isnull(ActiveStatus, 1) = 1 and isnull(StockActiveStatus, 1) = 1 ";
                    frmDetailedSearch2 frmN = new frmDetailedSearch2(GetFromItemSearch, sQuery, "Anywhere|ItemCode|ItemName|BatchUnique", (this.Width / 2) - 535, (this.Height / 2) - 245, 7, 0, "", 6, 0, "ORDER BY ItemCode ASC, Batchunique", 0, 0, "Item Search...", 0, "150,250,150,150,150,0,0", true, "frmItemMaster", 20, false, this, 6);
                    frmN.MdiParent = this;
                    frmN.Show(); //20-Aug-2022

                    break;

                case "CUSTOMERVIEW":
                    sQuery = "SELECT LName+LAliasName+MobileNo+Address as AnyWhere,LALiasname as [Supplier Code],lname as [Supplier Name] ,MobileNo ,Address,LID,Email  FROM tblLedger L";
                    sQuery = sQuery + " WHERE UPPER(L.groupName)='CUSTOMER' AND L.TenantID=" + Global.gblTenantID + "";
                    frmDetailedSearch2 frmC = new frmDetailedSearch2(GetFromItemSearch, sQuery, "Anywhere|ItemCode|ItemName|BatchUnique", (this.Width / 2) - 535, (this.Height / 2) - 245, 5, 0, "", 5, 0, "ORDER BY ItemCode ASC, Batchunique", 0, 0, "Item Search...", 0, "150,250,150,150,150,0,0", true, "frmItemMaster", 20, false, this, 5);
                    frmC.MdiParent = this;
                    frmC.Show(); //20-Aug-2022

                    break;

                case "SUPPLIERVIEW":
                    sQuery = "SELECT LName+LAliasName+MobileNo+Address as AnyWhere,LALiasname as [Supplier Code],lname as [Supplier Name] ,MobileNo ,Address,LID,Email  FROM tblLedger L";
                    sQuery = sQuery + " WHERE UPPER(L.groupName)='SUPPLIER' AND L.TenantID=" + Global.gblTenantID + "";
                    frmDetailedSearch2 frmS = new frmDetailedSearch2(GetFromItemSearch, sQuery, "Anywhere|ItemCode|ItemName|BatchUnique", (this.Width / 2) - 535, (this.Height / 2) - 245, 5, 0, "", 5, 0, "ORDER BY ItemCode ASC, Batchunique", 0, 0, "Item Search...", 0, "150,250,150,150,150,0,0", true, "frmItemMaster", 20, false, this, 5);
                    frmS.MdiParent = this;
                    frmS.Show(); //20-Aug-2022

                    break;

                case "COMPANYSETTINGS":
                    frmCompanySettings frmCS = new frmCompanySettings();
                    frmCS.Text = "Settings";
                    frmCS.MdiParent = this;
                    frmCS.Show();
                    frmCS.Focus();
                    frmCS.BringToFront();
                    break;

                case "STOCKHISTORY":
                    frmItemAnalysis frmIA = new frmItemAnalysis();
                    frmIA.Text = "Settings";
                    frmIA.MdiParent = this;
                    frmIA.Show();
                    frmIA.Focus();
                    frmIA.BringToFront();
                    break;

                case "APPLICATIONSETTINGS":
                    frmSettings frmSet = new frmSettings(0, false);
                    frmSet.Text = "Settings";
                    frmSet.MdiParent = this;
                    frmSet.Show();
                    frmSet.Focus();
                    frmSet.BringToFront();
                    break;

                case "EDITWINDOW":
                    frmEditWindow frmEdit = new frmEditWindow("", this);
                    frmEdit.Text = "Edit Window";
                    frmEdit.MdiParent = this;
                    frmEdit.Show();
                    frmEdit.Focus();
                    frmEdit.BringToFront();
                    break;
                case "BARCODEMANAGER":
                    frmBarcode frmBCode = new frmBarcode(2,0,"","",this);
                    frmBCode.Text = "Barcode Manager";
                    frmBCode.MdiParent = this;
                    frmBCode.Show();
                    frmBCode.Focus();
                    frmBCode.BringToFront();
                    break;
                case "HSN":
                    frmHSN frmhsn = new frmHSN(0, false);
                    frmhsn.MdiParent = this;
                    frmhsn.Show();
                    frmhsn.Focus();
                    frmhsn.BringToFront();
                    break;
                case "CATEGORY":
                    frmItemCategory frmCat = new frmItemCategory(0, false);
                    frmCat.MdiParent = this;
                    frmCat.Show();
                    frmCat.Focus();
                    frmCat.BringToFront();
                    break;
                case "MANUFACTURER":
                    frmManufacturer frmM = new frmManufacturer(0, false);
                    frmM.MdiParent = this;
                    frmM.Show();
                    frmM.Focus();
                    frmM.BringToFront();
                    break;
                case "BRAND":
                    frmBrandMaster frmB = new frmBrandMaster(0, false);
                    frmB.MdiParent = this;
                    frmB.Show();
                    frmB.Focus();
                    frmB.BringToFront();
                    break;
                case "DISCGROUP":
                    frmDiscountGroup frmDisc = new frmDiscountGroup(0, false);
                    frmDisc.MdiParent = this;
                    frmDisc.Show();
                    frmDisc.Focus();
                    frmDisc.BringToFront();
                    break;
                case "SIZE":
                    FrmSizeMaster frmSize = new FrmSizeMaster(0, false);
                    frmSize.MdiParent = this;
                    frmSize.Show();
                    frmSize.Focus();
                    frmSize.BringToFront();
                    break;
                case "COLOUR":
                    frmColorMaster frmColor = new frmColorMaster(0, false);
                    frmColor.MdiParent = this;
                    frmColor.Show();
                    frmColor.Focus();
                    frmColor.BringToFront();
                    break;
                case "UPIMASTER":
                    frmCashDeskMaster frmCDM = new frmCashDeskMaster(0, false);
                    frmCDM.MdiParent = this;
                    frmCDM.Show();
                    frmCDM.Focus();
                    frmCDM.BringToFront();
                    break;
                case "UNIT":
                    frmUnitMaster frmUnit = new frmUnitMaster(0, false);
                    frmUnit.MdiParent = this;
                    frmUnit.Show();
                    frmUnit.Focus();
                    frmUnit.BringToFront();
                    break;
                case "STOCKDEPARTMENT":
                    frmDepartment frmStDep = new frmDepartment(0, false, 0);
                    frmStDep.MdiParent = this;
                    frmStDep.Show();
                    frmStDep.Focus();
                    frmStDep.BringToFront();
                    break;
                case "ITEM":
                    frmItemMaster frmItem = new frmItemMaster(0, true);
                    frmItem.MdiParent = this;
                    frmItem.Show();
                    frmItem.Focus();
                    frmItem.BringToFront();
                    break;
                case "AREA":
                    frmAreaMaster frmArea = new frmAreaMaster(0, false);
                    frmArea.MdiParent = this;
                    frmArea.Show();
                    frmArea.Focus();
                    frmArea.BringToFront();
                    break;
                case "AGENT":
                    frmAgentMaster frmAgent = new frmAgentMaster(0, false);
                    frmAgent.MdiParent = this;
                    frmAgent.Show();
                    frmAgent.Focus();
                    frmAgent.BringToFront();
                    break;
                case "SUPPLIER":
                    frmLedger frmSup = new frmLedger(0, false, 0, "SUPPLIER");
                    frmSup.MdiParent = this;
                    frmSup.Show();
                    frmSup.Focus();
                    frmSup.BringToFront();
                    break;
                case "CUSTOMER":
                    frmLedger frmCust = new frmLedger(0, false, 0, "CUSTOMER");
                    frmCust.MdiParent = this;
                    frmCust.Show();
                    frmCust.Focus();
                    frmCust.BringToFront();
                    break;
                case "LEDGER":
                    frmLedger frmLed = new frmLedger(0, false);
                    frmLed.MdiParent = this;
                    frmLed.Show();
                    frmLed.Focus();
                    frmLed.BringToFront();
                    break;
                case "TAXMODE":
                    frmTaxMode frmTax = new frmTaxMode(0, false);
                    frmTax.MdiParent = this;
                    frmTax.Show();
                    frmTax.Focus();
                    frmTax.BringToFront();
                    break;
                case "ACCOUNTGROUP":
                    frmAccountGroup frmAcc = new frmAccountGroup(0, false);
                    frmAcc.MdiParent = this;
                    frmAcc.Show();
                    frmAcc.Focus();
                    frmAcc.BringToFront();
                    break;
                case "VOUCHERTYPE":
                    frmVouchertype frmVch = new frmVouchertype(0, false);
                    frmVch.MdiParent = this;
                    frmVch.Show();
                    frmVch.Focus();
                    frmVch.BringToFront();
                    break;
                case "STATE":
                    frmState frmSt = new frmState(0, false);
                    frmSt.MdiParent = this;
                    frmSt.Show();
                    frmSt.Focus();
                    frmSt.BringToFront();
                    break;
                case "COSTCENTRE":
                    frmCostCentre frmCC = new frmCostCentre(0, false);
                    frmCC.MdiParent = this;
                    frmCC.Show();
                    frmCC.Focus();
                    frmCC.BringToFront();
                    break;
                case "EMPLOYEECATEGORY":
                    break;
                case "EMPLOYEE":
                    frmEmployee frmEmp = new frmEmployee(0, false);
                    frmEmp.MdiParent = this;
                    frmEmp.Show();
                    frmEmp.Focus();
                    frmEmp.BringToFront();
                    break;
                case "DEPARTMENT":
                    frmDepartment frmDep = new frmDepartment(0, false, 1);
                    frmDep.MdiParent = this;
                    frmDep.Show();
                    frmDep.Focus();
                    frmDep.BringToFront();
                    break;
                case "STOCKANALYSIS":
                    frmItemAnalysis frmStAn = new frmItemAnalysis();
                    frmStAn.MdiParent = this;
                    frmStAn.Show();
                    frmStAn.Focus();
                    frmStAn.BringToFront();
                    break;
                case "USERGROUP":
                    frmUserGroup frmUG = new frmUserGroup(0, false);
                    frmUG.MdiParent = this;
                    frmUG.Show();
                    frmUG.Focus();
                    frmUG.BringToFront();
                    break;
                case "USERS":
                    frmUser frmUsr = new frmUser(0, false);
                    frmUsr.MdiParent = this;
                    frmUsr.Show();
                    frmUsr.Focus();
                    frmUsr.BringToFront();
                    break;
                case "REPACKINGREPORT":
                    frmRepackingReport frmRPR = new frmRepackingReport();
                    frmRPR.MdiParent = this;
                    frmRPR.Show();
                    frmRPR.Focus();
                    frmRPR.BringToFront();
                    break;
                case "CASHDESKREPORT":
                    frmCashDeskReport frmCDR = new frmCashDeskReport();
                    frmCDR.MdiParent = this;
                    frmCDR.Show();
                    frmCDR.Focus();
                    frmCDR.BringToFront();
                    break;
                case "PURCHASEREPORT":
                    frmPurchaseReport frmPR = new frmPurchaseReport();
                    frmPR.MdiParent = this;
                    frmPR.Show();
                    frmPR.Focus();
                    frmPR.BringToFront();
                    break;
                case "SALESREPORT":
                    frmSalesReport frmSR = new frmSalesReport();
                    frmSR.MdiParent = this;
                    frmSR.Show();
                    frmSR.Focus();
                    frmSR.BringToFront();
                    break;
                case "PURCHASERETURNREPORT":
                    frmPurchaseReturnReport frmPRR = new frmPurchaseReturnReport();
                    frmPRR.MdiParent = this;
                    frmPRR.Show();
                    frmPRR.Focus();
                    frmPRR.BringToFront();
                    break;
                case "SALESRETURNREPORT":
                    frmSalesReturnReport frmSRR = new frmSalesReturnReport();
                    frmSRR.MdiParent = this;
                    frmSRR.Show();
                    frmSRR.Focus();
                    frmSRR.BringToFront();
                    break;
                case "STOCKREPORT":
                    frmStockReport frmSTOCKREPORT = new frmStockReport();
                    frmSTOCKREPORT.MdiParent = this;
                    frmSTOCKREPORT.Show();
                    frmSTOCKREPORT.Focus();
                    frmSTOCKREPORT.BringToFront();
                    break;
                case "ACCOUNTSREPORT":
                    frmAccountReport frmaccreport = new frmAccountReport();
                    frmaccreport.MdiParent = this;
                    frmaccreport.Show();
                    frmaccreport.Focus();
                    frmaccreport.BringToFront();
                    break;
                case "STOCKADJUSTMENTREPORT":
                    frmStockMovementReport frmSTOCKADJREPORT = new frmStockMovementReport();
                    frmSTOCKADJREPORT.MdiParent = this;
                    frmSTOCKADJREPORT.Show();
                    frmSTOCKADJREPORT.Focus();
                    frmSTOCKADJREPORT.BringToFront();
                    break;
                case "DELIVERYNOTEREPORT":
                    frmDeliveryNote frmdelnoterep = new frmDeliveryNote();
                    frmdelnoterep.MdiParent = this;
                    frmdelnoterep.Show();
                    frmdelnoterep.Focus();
                    frmdelnoterep.BringToFront();
                    break;
                case "RECEIPTNOTEREPORT":
                    frmReceiptNote frmrecnotrep = new frmReceiptNote();
                    frmrecnotrep.MdiParent = this;
                    frmrecnotrep.Show();
                    frmrecnotrep.Focus();
                    frmrecnotrep.BringToFront();
                    break;

                case null:
                    break;

                default:
                    break;
            }

            Int32 ParentVchtypeID = -1;
            sqlControl rs = new sqlControl();
            rs.Open("Select ParentID From tblVchtype Where isnull(ActiveStatus,0)=1 and VchtypeID=" + VchtypeID.ToString());
            if (!rs.eof())
            {
                //if(rs.fields("ParentID").ToString() != null)
                {
                    ParentVchtypeID = Convert.ToInt32(rs.fields("ParentID").ToString());
                    switch (ParentVchtypeID)
                    {
                        case 1: //Sales
                            frmStockOutVoucherNew frmSale = new frmStockOutVoucherNew(VchtypeID, 0, false, this);
                            frmSale.Show();
                            frmSale.BringToFront();
                            break;

                        case 2: //Purchase
                            frmStockInVoucherNew frmPurch = new frmStockInVoucherNew(VchtypeID, 0, false, this);
                            frmPurch.Show();
                            frmPurch.BringToFront();
                            break;

                        case 3: //Sales Return
                            frmStockOutVoucherNew frmSaleRet = new frmStockOutVoucherNew(VchtypeID, 0, false, this);
                            frmSaleRet.Show();
                            frmSaleRet.BringToFront();
                            break;

                        case 4: //Purchase Return
                            frmStockInVoucherNew frmPurchRet = new frmStockInVoucherNew(VchtypeID, 0, false, this);
                            frmPurchRet.Show();
                            frmPurchRet.BringToFront();
                            break;

                        case 5: //Delivery Note
                            frmStockOutVoucherNew frmDelNote = new frmStockOutVoucherNew(VchtypeID, 0, false, this);
                            frmDelNote.Show();
                            frmDelNote.BringToFront();
                            break;

                        case 6: //Receipt Note
                            frmStockInVoucherNew frmRecNote = new frmStockInVoucherNew(VchtypeID, 0, false, this);
                            frmRecNote.Show();
                            frmRecNote.BringToFront();
                            break;

                        case 20: //Purchase
                            frmRepacking frmRepack = new frmRepacking(VchtypeID, 0, false, this);
                            frmRepack.Show();
                            frmRepack.BringToFront();
                            break;

                        case 40: //board rate updator
                            frmBoardRateUpdator frmBRU = new frmBoardRateUpdator(VchtypeID, 0, false, this);
                            frmBRU.Show();
                            frmBRU.BringToFront();
                            break;

                        case 41: //Physical Stock
                            frmPhysicalStock frmPhSt = new frmPhysicalStock(VchtypeID, 0, false, this);
                            frmPhSt.Show();
                            frmPhSt.BringToFront();
                            break;

                        case 16: //Stock Transfer
                            frmPhysicalStock frmStTr = new frmPhysicalStock(VchtypeID, 0, false, this);
                            frmStTr.Show();
                            frmStTr.BringToFront();
                            break;

                        case 1005: //Opening
                            frmStockInVoucherNew frmOP = new frmStockInVoucherNew(VchtypeID, 0, false, this);
                            frmOP.Show();
                            frmOP.BringToFront();
                            break;

                        case 42: //Price List
                            frmPriceListUpdator frmPL = new frmPriceListUpdator(VchtypeID, 0, false, this);
                            frmPL.Show();
                            frmPL.BringToFront();
                            break;

                        case 7: //Receipt
                        case 8: //Payment
                        case 9: //Contra
                        case 10: //Journal
                            frmReceipt frmRec = new frmReceipt(VchtypeID, 0, false, this);
                            frmRec.Show();
                            frmRec.BringToFront();
                            break;

                        case 89: //PriceList Updator
                            frmPriceListUpdator frmPLU = new frmPriceListUpdator(VchtypeID, 0, false, this);
                            frmPLU.Show();
                            frmPLU.BringToFront();
                            break;

                        default:
                            break;
                    }
                }
            }
        }

        private void categoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem t = (ToolStripMenuItem)sender;
            OpenMenu(t.Text.ToString(), 0);

        }

        private void manufacturerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem t = (ToolStripMenuItem)sender;
            OpenMenu(t.Text.ToString(), 0);

        }

        private void sizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem t = (ToolStripMenuItem)sender;
            OpenMenu(t.Text.ToString(), 0);

        }

        private void colourToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem t = (ToolStripMenuItem)sender;
            OpenMenu(t.Text.ToString(), 0);

        }

        private void unitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem t = (ToolStripMenuItem)sender;
            OpenMenu(t.Text.ToString(), 0);

        }

        private void itemDepartmenttoolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem t = (ToolStripMenuItem)sender;
            OpenMenu(t.Text.ToString(), 0);

        }

        private void ledgerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem t = (ToolStripMenuItem)sender;
            OpenMenu(t.Text.ToString(), 0);

        }

        private void customerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem t = (ToolStripMenuItem)sender;
            OpenMenu(t.Text.ToString(), 0);

        }

        private void supplierToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem t = (ToolStripMenuItem)sender;
            OpenMenu(t.Text.ToString(), 0);

        }

        private void accountGroupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem t = (ToolStripMenuItem)sender;
            OpenMenu(t.Text.ToString(), 0);

        }

        private void areaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem t = (ToolStripMenuItem)sender;
            OpenMenu(t.Text.ToString(), 0);

        }

        private void agentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem t = (ToolStripMenuItem)sender;
            OpenMenu(t.Text.ToString(), 0);

        }

        private void employeeCategoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem t = (ToolStripMenuItem)sender;
            OpenMenu(t.Text.ToString(), 0);

        }

        private void employeeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem t = (ToolStripMenuItem)sender;
            OpenMenu(t.Text.ToString(), 0);

        }

        private void departmentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem t = (ToolStripMenuItem)sender;
            OpenMenu(t.Text.ToString(), 0);

        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExitApp();
        }

        private void customizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem t = (ToolStripMenuItem)sender;
            OpenMenu(t.Text.ToString(), 0);

        }

        private void tsmTools_Click(object sender, EventArgs e)
        {

        }

        private void editWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem t = (ToolStripMenuItem)sender;
            OpenMenu(t.Text.ToString(), 0);

        }

        private void ExitApp()
        {
            DialogResult result = MessageBox.Show("Are you sure to shutdown the application.", Application.ProductName, MessageBoxButtons.YesNo,MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                this.Close();
                Environment.Exit(0);
            }
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            ExitApp();
        }

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void tsmManufacturer_Click(object sender, EventArgs e)
        {

        }

        private void dISCGROUPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem t = (ToolStripMenuItem)sender;
            OpenMenu(t.Text.ToString(), 0);

        }

        private void voucherTypeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem t = (ToolStripMenuItem)sender;
            OpenMenu(t.Text.ToString(), 0);

        }

        private void costCentreToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem t = (ToolStripMenuItem)sender;
            OpenMenu(t.Text.ToString(), 0);
        }

        private void stateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem t = (ToolStripMenuItem)sender;
            OpenMenu(t.Text.ToString(), 0);
        }

        private void taxModeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem t = (ToolStripMenuItem)sender;
            OpenMenu(t.Text.ToString(), 0);
        }

        private void stockReportsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem t = (ToolStripMenuItem)sender;
            OpenMenu(t.Text.ToString(), 0);

        }

        private void stockAdjustmentReportsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem t = (ToolStripMenuItem)sender;
            OpenMenu(t.Text.ToString(), 0);

        }

        private void orderReportsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem t = (ToolStripMenuItem)sender;
            OpenMenu(t.Text.ToString(), 0);

        }

        private void accountsReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem t = (ToolStripMenuItem)sender;
            OpenMenu(t.Text.ToString(), 0);

        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem t = (ToolStripMenuItem)sender;
            OpenMenu(t.Text.ToString(), 0);

        }

        private void barcodeManagerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem t = (ToolStripMenuItem)sender;
            OpenMenu(t.Text.ToString(), 0);

        }

        private void analysisReportsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem t = (ToolStripMenuItem)sender;
            OpenMenu(t.Text.ToString(), 0);

        }

        private void purchaseReturnReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem t = (ToolStripMenuItem)sender;
            OpenMenu(t.Text.ToString(), 0);

        }

        private void salesReturnReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem t = (ToolStripMenuItem)sender;
            OpenMenu(t.Text.ToString(), 0);

        }

        private void userGroupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem t = (ToolStripMenuItem)sender;
            OpenMenu(t.Text.ToString(), 0);

        }

        private void usersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem t = (ToolStripMenuItem)sender;
            OpenMenu(t.Text.ToString(), 0);

        }

        private void applicationSettingsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem t = (ToolStripMenuItem)sender;
            OpenMenu(t.Text.ToString(), 0);

        }

        private void stockHistoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem t = (ToolStripMenuItem)sender;
            OpenMenu(t.Text.ToString(), 0);

        }

        private void itemViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem t = (ToolStripMenuItem)sender;
            OpenMenu(t.Text.ToString(), 0);

        }

        private void customerViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem t = (ToolStripMenuItem)sender;
            OpenMenu(t.Text.ToString(), 0);

        }

        private void supplierViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem t = (ToolStripMenuItem)sender;
            OpenMenu(t.Text.ToString(), 0);

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void dashBoardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem t = (ToolStripMenuItem)sender;
            OpenMenu(t.Text.ToString(), 0);

        }

        private void deliveryNoteReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem t = (ToolStripMenuItem)sender;
            OpenMenu(t.Text.ToString(), 0);

        }

        private void receiptNoteReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem t = (ToolStripMenuItem)sender;
            OpenMenu(t.Text.ToString(), 0);

        }

        private void frmMDI_Shown(object sender, EventArgs e)
        {
            try
            {
                if (File.Exists(Application.StartupPath + @"\Resources\WallpaperMain.jpg") == true)
                {
                    Bitmap img = new Bitmap(Application.StartupPath + @"\Resources\WallpaperMain.jpg");
                    this.BackgroundImage = img;
                }
            }
            catch
            { }
        }

        private void createCompanyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem t = (ToolStripMenuItem)sender;
            OpenMenu(t.Text.ToString(), 0);

        }

        private void companySettingsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem t = (ToolStripMenuItem)sender;
            OpenMenu(t.Text.ToString(), 0);

        }

        private void gSTR1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem t = (ToolStripMenuItem)sender;
            OpenMenu(t.Text.ToString(), 0);

        }

        private void uPIMasterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem t = (ToolStripMenuItem)sender;
            OpenMenu(t.Text.ToString(), 0);

        }

        private void cashDeskReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem t = (ToolStripMenuItem)sender;
            OpenMenu(t.Text.ToString(), 0);
        }

        private void rEPACKINGREPORTToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem t = (ToolStripMenuItem)sender;
            OpenMenu(t.Text.ToString(), 0);

        }

        private void daybookToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem t = (ToolStripMenuItem)sender;
            OpenMenu(t.Text.ToString(), 0);

        }

        private void trialBalanceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem t = (ToolStripMenuItem)sender;
            OpenMenu(t.Text.ToString(), 0);

        }

        private void daybookSummaryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem t = (ToolStripMenuItem)sender;
            OpenMenu(t.Text.ToString(), 0);

        }

        private void profitLossToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem t = (ToolStripMenuItem)sender;
            OpenMenu(t.Text.ToString(), 0);

        }

        private void balancesheetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem t = (ToolStripMenuItem)sender;
            OpenMenu(t.Text.ToString(), 0);

        }

        private void tsmConnectionDetails_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem t = (ToolStripMenuItem)sender;
            OpenMenu(t.Text.ToString(), 0);

        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            AboutBoxUs ab = new AboutBoxUs();

            ab.ShowDialog();
        }

        private void hsnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem t = (ToolStripMenuItem)sender;
            OpenMenu(t.Text.ToString(), 0);

        }
    }
}
