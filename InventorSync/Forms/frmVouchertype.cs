using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DigiposZen.InventorBL.Master;
using DigiposZen.InventorBL.Helper;
using DigiposZen.Info;
using Syncfusion.WinForms.Controls;
using DigiposZen.Forms;
using Newtonsoft.Json;
using System.Collections;
using DigiposZen.JsonClass;
using System.Runtime.InteropServices;
using System.IO;

namespace DigiposZen
{
    public partial class frmVouchertype : Form, IMessageFilter
    {
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        public const int WM_LBUTTONDOWN = 0x0201;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        private HashSet<Control> controlsToMove = new HashSet<Control>();
        public frmVouchertype(int iID = 0, bool bFromEdit = false, bool blnDisableMinimize = false)
        {
            InitializeComponent();

            Application.AddMessageFilter(this);

            try
            {
                Comm.TransparentControls(this);
                Comm.SetControlColours(this);

                tlpMain.BackColor = Color.FromArgb(249, 246, 238);
                this.BackColor = Color.Black;
                this.Padding = new Padding(1);

                //Comm.LoadBGImage(this, picBackground);

                lblHeading.Font = new Font("Tahoma", 21, FontStyle.Regular, GraphicsUnit.Pixel);
                lblSave.Font = new Font("Tahoma", 10, FontStyle.Regular, GraphicsUnit.Point);

                lblSave.ForeColor = Color.Black;

                btnSave.Image = global::DigiposZen.Properties.Resources.save240402;
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

            controlsToMove.Add(this);
            controlsToMove.Add(this.lblHeading);//Add whatever controls here you want to move the form when it is clicked and dragged

            DirectoryInfo dir = new DirectoryInfo(Application.StartupPath + @"\PrintScheme");
            FileInfo[] files = dir.GetFiles("*.rdlc");
            foreach (FileInfo file in files)
            {
                cboInvScheme1.Items.Add(file.Name.Replace(".rdlc", ""));
            }

            dIDFromEditWindow = iID;
            FillParentTransaction();
            FillBillWiseXtraDiscount();
            FillItemWiseXtraDiscount();
            FillCostCenter("PRIMARY");
            FillCostCenter("SECONDARY");
            FillTaxModeForDefault();
            FillModofPayForDefault();
            FillSalesStaffEmpForDefault();
            FillAgentForDefault();
            FillDefaultSearchmethod();
            FillMMRPSubWindowSearchMod();
            FillTaxInclusiveSettings();
            FillBarcodeMode();
            FillPriceList();
            ClearAll();
            LoadDefaults();
            FillTransSortOrder();
            this.BackColor = Global.gblFormBorderColor;

            togglebtnActive.ToggleState = Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active;

            btnMinimize.Enabled = !blnDisableMinimize;

            if (dIDFromEditWindow != 0)
            {
                LoadData(iID);

                if (dIDFromEditWindow <= 1005)
                {
                    txtTransName.Enabled = false;
                    cboParentTrans.Enabled = false;
                }
            }

            GrbBoardRate.Visible = false;
            if (cboParentTrans.SelectedValue != null)
            {
                if (Comm.ToInt32(cboParentTrans.SelectedValue.ToString()) == 40)
                    GrbBoardRate.Visible = true;
            }
        }

        #region "VARIABLES  -------------------------------------------- >>"
        Common Comm = new Common();
        UspGetVchTypeInfo GetVchTyp = new UspGetVchTypeInfo();
        UspVchTypeInsertInfo infoVchTyp = new UspVchTypeInsertInfo();
        UspGetOnetimeMasterInfo GetOtminfo = new UspGetOnetimeMasterInfo();

        clsVoucherType clsVouchTyp = new clsVoucherType();
        clsOneTimeMaster clsOtm = new clsOneTimeMaster();

        public List<clsJsonVchTypeFeatures> lstFeatures = new List<clsJsonVchTypeFeatures>();
        DataTable dtGetFromVchFeature = new DataTable();

        //For Drag Form
        bool dragging = false;
        int xOffset = 0;
        int yOffset = 0;

        Control ctrl;
        decimal dIDFromEditWindow;
        string strFillProdClass = "", strFillCatClass = "", strFillCustomer = "", strFillDebitAcc = "", strFillCreditAcc = "";

        #endregion

        //Description: Fill Pricelist Details according to settings
        private void FillPriceList(int iSelID = 0)
        {
            DataTable dtPriceList = new DataTable();
            //dtPriceList = GetAgent(0);

            DataColumn dc = new DataColumn("PLName", typeof(String));
            DataColumn dc1 = new DataColumn("PLID", typeof(int));

            dtPriceList.Columns.Add(dc);
            dtPriceList.Columns.Add(dc1);

            DataRow dRow0 = dtPriceList.NewRow();
            dRow0[0] = "<None>";
            dRow0[1] = 0;
            dtPriceList.Rows.Add(dRow0);

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
            if (AppSettings.IsActiveSRate3 == true)
            {
                DataRow dRow1 = dtPriceList.NewRow();
                dRow1[0] = AppSettings.SRate3Name;
                dRow1[1] = 3;
                dtPriceList.Rows.Add(dRow1);
            }
            if (AppSettings.IsActiveSRate4 == true)
            {
                DataRow dRow1 = dtPriceList.NewRow();
                dRow1[0] = AppSettings.SRate4Name;
                dRow1[1] = 4;
                dtPriceList.Rows.Add(dRow1);
            }
            if (AppSettings.IsActiveSRate5 == true)
            {
                DataRow dRow1 = dtPriceList.NewRow();
                dRow1[0] = AppSettings.SRate5Name;
                dRow1[1] = 5;
                dtPriceList.Rows.Add(dRow1);
            }

            if (dtPriceList.Rows.Count > 0)
            {
                cboDefaultPriceList.DataSource = dtPriceList;
                cboDefaultPriceList.DisplayMember = "PLName";
                cboDefaultPriceList.ValueMember = "PLID";

                cboDefaultPriceList.SelectedValue = 1;
            }
        }

        #region "EVENTS ------------------------------------------------ >>"
        //drag form
        private void tlpHeader_MouseUp(object sender, MouseEventArgs e)
        {
            dragging = false;
        }
        private void tlpHeader_MouseDown(object sender, MouseEventArgs e)
        {
            dragging = true;
            xOffset = Cursor.Position.X - this.Location.X;
            yOffset = Cursor.Position.Y - this.Location.Y;
        }
        private void tlpHeader_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragging)
            {
                this.Location = new Point(Cursor.Position.X - xOffset, Cursor.Position.Y - yOffset);
                this.Update();
            }
        }

        private void txtTransName_Click(object sender, EventArgs e)
        {
            ToolTipVoucher.SetToolTip(txtTransName, "Please specify TransactionName");
        }
        private void cboParentTrans_Click(object sender, EventArgs e)
        {
            ToolTipVoucher.SetToolTip(cboParentTrans, "Please Select Parent Transction");
        }
        private void cboTransNumbering_Click(object sender, EventArgs e)
        {
            ToolTipVoucher.SetToolTip(cboTransNumbering, "Select one Transaction Numbering");
        }
        private void txtTransPrefix_Click(object sender, EventArgs e)
        {
            ToolTipVoucher.SetToolTip(txtTransPrefix, "Specify Transaction Prefix");
        }
        private void cboRefNumbering_Click(object sender, EventArgs e)
        {
            ToolTipVoucher.SetToolTip(cboRefNumbering, "Select one Reference Numbering");
        }
        private void txtRefPrefix_Click(object sender, EventArgs e)
        {
            ToolTipVoucher.SetToolTip(txtRefPrefix, "Specify Reference Prefix");
        }
        private void txtSortOrder_Click(object sender, EventArgs e)
        {
            ToolTipVoucher.SetToolTip(txtSortOrder, "Please Sort Order");
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
                        this.SelectNextControl(ctrl, false, false, false, false);
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
                MessageBox.Show("Enter Key Press not working" + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void cboDefaultTaxMode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
                rdoFeatures.Focus();
           else if (e.KeyCode == Keys.Enter)
                tbtnTaxMode.Focus();
        }
        private void tbtnTaxMode_KeyDown(object sender, KeyEventArgs e)
        {
              if (e.Shift == true && e.KeyCode == Keys.Enter)
                cboDefaultTaxMode.Focus();
            else if(e.KeyCode == Keys.Enter)
            {
                cboDefaultModofPay.Focus();
                SendKeys.Send("{F4}");
            }
           
        }
        private void cboDefaultModofPay_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
                tbtnTaxMode.Focus();
            else if  (e.KeyCode == Keys.Enter)
                tbtnMOP.Focus();
        }
        private void tbtnMOP_KeyDown(object sender, KeyEventArgs e)
        {
              if (e.Shift == true && e.KeyCode == Keys.Enter)
                cboDefaultModofPay.Focus();
            else if(e.KeyCode == Keys.Enter)
            {
                cboDefaultSalesStaff.Focus();
                SendKeys.Send("{F4}");
            }
           
        }
        private void cboDefaultSalesStaff_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
                tbtnMOP.Focus();
            else if (e.KeyCode == Keys.Enter)
                tbtnSalesStaff.Focus();
        }
        private void tbtnSalesStaff_KeyDown(object sender, KeyEventArgs e)
        {
             if (e.Shift == true && e.KeyCode == Keys.Enter)
                cboDefaultSalesStaff.Focus();
            else if(e.KeyCode == Keys.Enter)
            {
                cboDefaultAgent.Focus();
                SendKeys.Send("{F4}");
            }
            
        }
        private void cboDefaultAgent_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
                tbtnSalesStaff.Focus();
            else if (e.KeyCode == Keys.Enter)
                tbtnAgent.Focus();
        }
        private void tbtnAgent_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
                cboDefaultAgent.Focus();
            else if (e.KeyCode == Keys.Enter)
                cboDefaultTaxInclusive.Focus();
        }
        private void cboDefaultSearchMethod_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
                rdoDefault.Focus();
            else if (e.KeyCode == Keys.Enter)
                tbtnSpaceforRateSearch.Focus();
        }
        private void tbtnSpaceforRateSearch_KeyDown(object sender, KeyEventArgs e)
        {
              if (e.Shift == true && e.KeyCode == Keys.Enter)
                cboDefaultSearchMethod.Focus();
            else if(e.KeyCode == Keys.Enter)
                tbtnItmSearchbydefault.Focus();
           
        }
        private void tbtnItmSearchbydefault_KeyDown(object sender, KeyEventArgs e)
        {
              if (e.Shift == true && e.KeyCode == Keys.Enter)
                tbtnItmSearchbydefault.Focus();
            else if(e.KeyCode == Keys.Enter)
                tbtnMovetonextafterselection.Focus();
           
        }
        private void tbtnMovetonextafterselection_KeyDown(object sender, KeyEventArgs e)
        {
             if (e.Shift == true && e.KeyCode == Keys.Enter)
                tbtnSpaceforRateSearch.Focus();
            else if(e.KeyCode == Keys.Enter)
                tbtnMMRPHideNeg.Focus();
            
        }
        private void tbtnMMRPHideNeg_KeyDown(object sender, KeyEventArgs e)
        {
              if (e.Shift == true && e.KeyCode == Keys.Enter)
                tbtnMovetonextafterselection.Focus();
            else if(e.KeyCode == Keys.Enter)
            {
                cboMMRPSubWindowSearchMod.Focus();
                SendKeys.Send("{F4}");
            }
           
        }
        private void cboMMRPSubWindowSearchMod_KeyDown(object sender, KeyEventArgs e)
        {
              if (e.Shift == true && e.KeyCode == Keys.Enter)
                tbtnMMRPHideNeg.Focus();
            else if(e.KeyCode == Keys.Enter)
                tbtnShowSearchWindowByDefault.Focus();
           
        }
        private void tbtnShowSearchWindowByDefault_KeyDown(object sender, KeyEventArgs e)
        {
              if (e.Shift == true && e.KeyCode == Keys.Enter)
                cboMMRPSubWindowSearchMod.Focus();
            else if(e.KeyCode == Keys.Enter)
                rdoFilters.Focus();
        }
        private void txtTransName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                cboParentTrans.Focus();
                SendKeys.Send("{F4}");
            }
        }
        private void cboParentTrans_KeyDown(object sender, KeyEventArgs e)
        {if (e.Shift == true && e.KeyCode == Keys.Enter)
            {
                txtTransName.Focus();
            }
            else if (e.KeyCode == Keys.Enter)
            {
                cboTransNumbering.Focus();
                SendKeys.Send("{F4}");
            }
         }
        private void txtTransPrefix_KeyDown(object sender, KeyEventArgs e)
        {
             if (e.Shift == true && e.KeyCode == Keys.Enter)
            {
                cboTransNumbering.Focus();
            }
            else if(e.KeyCode == Keys.Enter)
            {
                cboRefNumbering.Focus();
                SendKeys.Send("{F4}");
            }
            
        }
        private void txtCusrsorNavList_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Shift == true && e.KeyCode == Keys.Enter)
                    txtSortOrder.Focus();
                else if (e.KeyCode == Keys.Enter)
                {
                    cboDefaultCostCenterPrimary.Select();
                    cboDefaultCostCenterPrimary.Focus();
                    SendKeys.Send("{F4}");
                }
                else if (e.KeyCode == Keys.Down)
                {
                    string sQuery = "SELECT VchTypeID,VchType FROM tblVchType WHERE ActiveStatus = 1 AND TenantID = " + Global.gblTenantID + "";
                    new frmCompactCheckedListSearch(GetFromCheckedParent, sQuery, "VchType", txtCusrsorNavList.Location.X + 190, txtCusrsorNavList.Location.Y + 118, 0, 2, txtCusrsorNavList.Text).ShowDialog();
                    SendKeys.Send("{Tab}");
                }
                else if (e.Shift == true && e.KeyCode == Keys.Enter)
                {
                    txtCusrsorNavList.Focus();
                }
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void tbtnRateDisc_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
                cboItmWiseXtraDisc.Focus();
            else if (e.KeyCode == Keys.Enter)
                rdoFeatures.Focus();
        }
        private void rdoBillWisePercWise_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
                rdoSettings.Focus();
            else if (e.KeyCode == Keys.Enter)
                rdoBillWiseAmt.Focus();
        }
        private void rdoBillWiseAmt_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
                rdoBillWisePercWise.Focus();
            else if (e.KeyCode == Keys.Enter)
                rdoBillWisePercAmt.Focus();
        }
        private void cboBillWiseXtraDisc_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
                rdoBillWisePercAmt.Focus();
            else if (e.KeyCode == Keys.Enter)
                rdoItmWisePercWise.Focus();
        }
        private void rdoItmWisePercWise_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
                cboBillWiseXtraDisc.Focus();
            else if (e.KeyCode == Keys.Enter)
                rdoItmWiseAmt.Focus();
        }
        private void rdoItmWiseAmt_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
                rdoItmWisePercWise.Focus();
            else if (e.KeyCode == Keys.Enter)
                rdoItmWisePercAmt.Focus();
        }
        private void rdoItmWisePercAmt_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
                rdoItmWiseAmt.Focus();
            else if (e.KeyCode == Keys.Enter)
            {
                cboItmWiseXtraDisc.Focus();
                SendKeys.Send("{F4}");
            }
        }
        private void rdoBillWisePercAmt_KeyDown(object sender, KeyEventArgs e)
        {
             if (e.Shift == true && e.KeyCode == Keys.Enter)
                rdoBillWiseAmt.Focus();
            else if(e.KeyCode == Keys.Enter)
            {
                cboBillWiseXtraDisc.Focus();
                SendKeys.Send("{F4}");
            }
            
        }
        private void cboItmWiseXtraDisc_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
                rdoItmWisePercAmt.Focus();
            else if (e.KeyCode == Keys.Enter)
            tbtnRateDisc.Focus();
        }
        private void cboDefaultCostCenterPrimary_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
                txtCusrsorNavList.Focus();
            if (e.Shift == true && e.KeyCode == Keys.Enter)
                cboDefaultCostCenterSecondary.Focus();
            else if  (e.KeyCode == Keys.Enter)
                tbtnPrimaryCCenter.Focus();
        }
        private void tbtnPrimaryCCenter_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
                cboDefaultCostCenterPrimary.Focus();
            else if (e.KeyCode == Keys.Enter)
            {
                cboDefaultCostCenterSecondary.Focus();
                SendKeys.Send("{F4}");
            }
        }
        private void cboDefaultCostCenterSecondary_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
                tbtnPrimaryCCenter.Focus();
            else if (e.KeyCode == Keys.Enter)
                tbtnSecondaryCCenter.Focus();
        }
        private void tbtnSecondaryCCenter_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
                cboDefaultCostCenterSecondary.Focus();
            else if (e.KeyCode == Keys.Enter)
            {
                rdoDiscount.Checked = true;
                rdoDiscount_Click(sender,e);
            }
        }
        private void txtSortOrder_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
                txtRefPrefix.Focus();
            else if (e.KeyCode == Keys.Enter)
            {
                txtCusrsorNavList.Focus();
                SendKeys.Send("{DOWN}");
            }

            else if (e.Shift == true && e.KeyCode == Keys.Enter)
                txtRefPrefix.Focus();
        }
        private void txtFltItemType_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
                rdoSearch.Focus();
            else if (e.KeyCode == Keys.Enter)
            {
                txtFltCategories.Focus();
                SendKeys.Send("{DOWN}");
            }
            else if (e.KeyCode == Keys.Down)
            {
                //string sQuery = "Select OtmID,OtmData FROM tblOnetimeMaster WHERE OtmType = '" + "PRODCLASS" + "'  AND TenantID = " + Global.gblTenantID + " ORDER BY OtmID ASC";
                //new frmCompactCheckedListSearch(GetFromProductClassCheckedList, sQuery, "Product Class", txtFltDrAccGrp.Location.X + 198, txtFltDrAccGrp.Location.Y + 125, 0, 2, txtFltItemType.Text, 0, 0, "", lblItmTypeIds.Text).ShowDialog();

                DataTable dtProdClass = LoadProductClass();
                new frmCompactCheckedListSearch(GetFromProductClassCheckedList, "", "Product Class", txtFltDrAccGrp.Location.X + 198, txtFltDrAccGrp.Location.Y + 125, 0, 2, txtFltItemType.Text, 0, 0, "", lblItmTypeIds.Text, dtProdClass).ShowDialog();
            }
        }
        private DataTable LoadProductClass(int iSelectedID = 0)
        {
            DataTable dtProdClass = new DataTable();
            dtProdClass.Clear();

            dtProdClass.Columns.Add("ProdTypeID");
            dtProdClass.Columns.Add("ProdType");

            DataRow dRow1 = dtProdClass.NewRow();
            dRow1["ProdTypeID"] = "1";
            dRow1["ProdType"] = "Stock Item 1";
            dtProdClass.Rows.Add(dRow1);

            DataRow dRow2 = dtProdClass.NewRow();
            dRow2["ProdTypeID"] = "2";
            dRow2["ProdType"] = "Stock Item 2";
            dtProdClass.Rows.Add(dRow2);

            DataRow dRow3 = dtProdClass.NewRow();
            dRow3["ProdTypeID"] = "3";
            dRow3["ProdType"] = "Stock Item 3";
            dtProdClass.Rows.Add(dRow3);

            DataRow dRow4 = dtProdClass.NewRow();
            dRow4["ProdTypeID"] = "4";
            dRow4["ProdType"] = "Service Item 1";
            dtProdClass.Rows.Add(dRow4);

            DataRow dRow5 = dtProdClass.NewRow();
            dRow5["ProdTypeID"] = "5";
            dRow5["ProdType"] = "Service Item 2";
            dtProdClass.Rows.Add(dRow5);

            DataRow dRow6 = dtProdClass.NewRow();
            dRow6["ProdTypeID"] = "6";
            dRow6["ProdType"] = "Service Item 3";
            dtProdClass.Rows.Add(dRow6);

            return dtProdClass;

            //cboProductClass.DataSource = dtProdClass;
            //cboProductClass.DisplayMember = "ProdType";
            //cboProductClass.ValueMember = "ProdTypeID";

            //cboProductClass.SelectedIndex = 0;
        }

        private void txtFltCategories_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
                txtFltItemType.Focus();
           else if (e.KeyCode == Keys.Enter)
            {
                txtFltCustGrp.Focus();
                SendKeys.Send("{DOWN}");
            }
            else if (e.KeyCode == Keys.Down)
            {
                string sQuery = "SELECT CategoryID,Category FROM tblCategories WHERE TenantID = " + Global.gblTenantID + "";
                new frmCompactCheckedListSearch(GetFromCheckedList, sQuery, "Category", txtFltCategories.Location.X + 198, txtFltCategories.Location.Y + 120, 0, 2, txtFltCategories.Text, 0, 0, "", lblCategoryIds.Text).ShowDialog();
            }
        }
        private void txtFltCustGrp_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
                txtFltCategories.Focus();
            else if (e.KeyCode == Keys.Enter)
            {
                txtFltDrAccGrp.Focus();
                SendKeys.Send("{DOWN}");
            }
            else if (e.KeyCode == Keys.Down)
            {
                string sQuery = "select AccountGroupID, AccountGroup from tblAccountGroup WHERE TenantID = " + Global.gblTenantID + " ORDER BY AccountGroup ASC";
                new frmCompactCheckedListSearch(GetFromCustomerCheckedList, sQuery, "AccountGroup", txtFltCustGrp.Location.X + 198, txtFltCustGrp.Location.Y + 120, 0, 2, txtFltCustGrp.Text, 0, 0, "", txtFltCustGrp.Text).ShowDialog();
            }
        }
        private void txtFltDrAccGrp_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
                txtFltCustGrp.Focus();
            else if (e.KeyCode == Keys.Enter)
            {
                txtFltCrAccGrp.Focus();
                SendKeys.Send("{DOWN}");
            }
            else if (e.KeyCode == Keys.Down)
            {
                string sQuery = "select AccountGroupID, AccountGroup from tblAccountGroup WHERE TenantID = " + Global.gblTenantID + " ORDER BY AccountGroup ASC";
                new frmCompactCheckedListSearch(GetFromLedgerDrCheckedList, sQuery, "AccountGroup", txtFltDrAccGrp.Location.X + 210, txtFltDrAccGrp.Location.Y + 200, 0, 2, txtFltDrAccGrp.Text, 0, 0, "", txtFltDrAccGrp.Text).ShowDialog();
            }
        }
        private void rdoDiscount_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                rdoBillWisePercWise.Focus();
            }
        }
        private void frmVouchertype_Load(object sender, EventArgs e)
        {
            ShowFormsAsperClick(1);
            CallGridData(dIDFromEditWindow);
            txtTransName.Focus();
            txtTransName.SelectAll();
            
        }
        private void frmVouchertype_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                if (e.KeyCode == Keys.F5)//save
                {
                   btnSave_Click(sender, e);
                }
                else if (e.KeyCode == Keys.Escape)//close
                {
                    if (!String.IsNullOrEmpty(txtTransName.Text))
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
        private void txtFltCrAccGrp_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
                txtFltDrAccGrp.Focus();
           else if (e.KeyCode == Keys.Down)
            {
                string sQuery = "select AccountGroupID, AccountGroup from tblAccountGroup WHERE TenantID = " + Global.gblTenantID + " ORDER BY AccountGroup ASC";
                new frmCompactCheckedListSearch(GetFromLedgerCrCheckedList, sQuery, "AccountGroup", txtFltCrAccGrp.Location.X + 210, txtFltCrAccGrp.Location.Y + 220, 0, 2, txtFltCrAccGrp.Text, 0, 0, "", txtFltCrAccGrp.Text).ShowDialog();
            }
            else if (e.KeyCode == Keys.Enter)
            {
                btnSave.PerformClick();
            }
        }
        private void txtRefPrefix_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
                cboRefNumbering.Focus();
            else if (e.KeyCode == Keys.Enter)
            {
                txtSortOrder.Focus();
            }
        }
        private void rdoDefault_Click(object sender, EventArgs e)
        {
            ShowFormsAsperClick(5);
            cboDefaultTaxMode.Focus();
        }
        private void rdoDiscount_Click(object sender, EventArgs e)
        {
            ShowFormsAsperClick(3);
            panel1.Focus();
        }
        private void rdoFeatures_Click(object sender, EventArgs e)
        {
            ShowFormsAsperClick(4);
            txtFeatureSearch.Focus();
        }
        private void rdoSearch_Click(object sender, EventArgs e)
        {
            ShowFormsAsperClick(2);
            cboDefaultSearchMethod.Focus();
        }
        private void rdoFilters_Click(object sender, EventArgs e)
        {
            ShowFormsAsperClick(6);
            this.txtFltItemType.Enter -= this.txtFltItemType_Enter;
            txtFltItemType.Focus();
            this.txtFltItemType.Enter += this.txtFltItemType_Enter;
        }
        private void rdoSettings_Click(object sender, EventArgs e)
        {
            ShowFormsAsperClick(1);
            txtTransName.Select();
            txtTransName.Focus();
        }

        private void tbtnPrimaryCCenter_Enter(object sender, EventArgs e)
        {
            tbtnPrimaryCCenter.InactiveState.BorderColor = System.Drawing.ColorTranslator.FromHtml("#000000");
            tbtnPrimaryCCenter.ActiveState.BorderColor = System.Drawing.ColorTranslator.FromHtml("#000000");
        }
        private void tbtnPrimaryCCenter_Leave(object sender, EventArgs e)
        {
            tbtnPrimaryCCenter.InactiveState.BorderColor = System.Drawing.ColorTranslator.FromHtml("#979797");
            tbtnPrimaryCCenter.ActiveState.BorderColor = System.Drawing.ColorTranslator.FromHtml("#979797");
        }
        private void cboParentTrans_Leave(object sender, EventArgs e)
        {
            SelectParentVchtype();
        }
        private void SelectParentVchtype()
        {
            try
            {
                CallGridData();
                if (Convert.ToInt32(cboParentTrans.SelectedValue) == 16)
                    pnlSecondaryCC.Visible = true;
                else
                    pnlSecondaryCC.Visible = false;

                if (Convert.ToInt32(cboParentTrans.SelectedValue) == 506)
                    pnlLedger.Visible = true;
                else
                    pnlLedger.Visible = false;
                Comm.ControlEnterLeave(cboParentTrans, false, false);
            }
            catch
            {

            }
        }
        private void txtCusrsorNavList_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtCusrsorNavList, true, false);
        }
        private void txtFltCategories_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtFltCategories, true, false);
        }
        private void txtFltCustGrp_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtFltCustGrp, true, false);
        }
        private void txtFltDrAccGrp_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtFltDrAccGrp, true, false);
        }
        private void txtFltCrAccGrp_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtFltCrAccGrp, true, false);
        }
        private void txtFltItemType_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtFltItemType, true, false);
        }
        private void txtTransName_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtTransName, true, false);
        }
        private void txtTransName_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtTransName, false, false);
        }
        private void cboParentTrans_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(cboParentTrans, true, false);
        }
        private void cboTransNumbering_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(cboTransNumbering, true, false);
        }
        private void cboTransNumbering_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(cboTransNumbering, false, false);
        }
        private void txtTransPrefix_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtTransPrefix, true, false);
        }
        private void txtTransPrefix_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtTransPrefix, false, false);
        }
        private void cboRefNumbering_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(cboRefNumbering, true, false);
        }
        private void cboRefNumbering_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(cboRefNumbering, false, false);
        }
        private void txtRefPrefix_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtRefPrefix, true, false);
        }
        private void txtRefPrefix_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtRefPrefix, false, false);
        }
        private void txtSortOrder_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtSortOrder, true, false);
        }
        private void txtSortOrder_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtSortOrder, false, false);
        }
        private void txtCusrsorNavList_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtCusrsorNavList, false, false);
        }
        private void cboDefaultCostCenterPrimary_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(cboDefaultCostCenterPrimary, true, false);
        }
        private void cboDefaultCostCenterPrimary_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(cboDefaultCostCenterPrimary, false, false);
        }
        private void tbtnPrimaryCCenter_Enter_1(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(tbtnPrimaryCCenter, true, false);
        }
        private void tbtnPrimaryCCenter_Leave_1(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(tbtnPrimaryCCenter, false, false);
        }
        private void cboDefaultCostCenterSecondary_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(cboDefaultCostCenterSecondary, true, false);
        }
        private void cboDefaultCostCenterSecondary_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(cboDefaultCostCenterSecondary, false, false);
        }
        private void tbtnSecondaryCCenter_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(tbtnSecondaryCCenter, true, false);
        }
        private void tbtnSecondaryCCenter_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(tbtnSecondaryCCenter, false, false);
        }
        private void cboDefaultSearchMethod_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(cboDefaultSearchMethod, true, false);
        }
        private void cboDefaultSearchMethod_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(cboDefaultSearchMethod, false, false);
        }
        private void tbtnSpaceforRateSearch_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(tbtnSpaceforRateSearch, true, false);
        }
        private void tbtnSpaceforRateSearch_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(tbtnSpaceforRateSearch, false, false);
        }
        private void tbtnItmSearchbydefault_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(tbtnItmSearchbydefault, true, false);
        }
        private void tbtnItmSearchbydefault_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(tbtnItmSearchbydefault, false, false);
        }
        private void tbtnMovetonextafterselection_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(tbtnMovetonextafterselection, true, false);
        }
        private void tbtnMovetonextafterselection_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(tbtnMovetonextafterselection, false, false);
        }
        private void tbtnMMRPHideNeg_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(tbtnMMRPHideNeg, true, false);
        }
        private void tbtnMMRPHideNeg_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(tbtnMMRPHideNeg, false, false);
        }
        private void cboMMRPSubWindowSearchMod_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(cboMMRPSubWindowSearchMod, true, false);
        }
        private void cboMMRPSubWindowSearchMod_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(cboMMRPSubWindowSearchMod, false, false);
        }
        private void tbtnShowSearchWindowByDefault_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(tbtnShowSearchWindowByDefault, true, false);
        }
        private void tbtnShowSearchWindowByDefault_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(tbtnShowSearchWindowByDefault, false, false);
        }
        private void rdoBillWisePercWise_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(rdoBillWisePercWise, true, false);
        }
        private void rdoBillWisePercWise_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(rdoBillWisePercWise, false, false);
        }
        private void rdoBillWiseAmt_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(rdoBillWiseAmt, true, false);
        }
        private void rdoBillWiseAmt_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(rdoBillWiseAmt, false, false);
        }
        private void rdoBillWisePercAmt_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(rdoBillWisePercAmt, true, false);
        }
        private void rdoBillWisePercAmt_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(rdoBillWisePercAmt, false, false);
        }
        private void cboBillWiseXtraDisc_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(cboBillWiseXtraDisc, true, false);
        }
        private void cboBillWiseXtraDisc_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(cboBillWiseXtraDisc, false, false);
        }
        private void rdoItmWisePercWise_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(rdoItmWisePercWise, true, false);
        }
        private void rdoItmWisePercWise_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(rdoItmWisePercWise, false, false);
        }
        private void rdoItmWiseAmt_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(rdoItmWiseAmt, true, false);
        }
        private void rdoItmWiseAmt_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(rdoItmWiseAmt, false, false);
        }
        private void rdoItmWisePercAmt_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(rdoItmWisePercAmt, true, false);
        }
        private void rdoItmWisePercAmt_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(rdoItmWisePercAmt, false, false);
        }
        private void cboItmWiseXtraDisc_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(cboItmWiseXtraDisc, true, false);
        }
        private void cboItmWiseXtraDisc_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(cboItmWiseXtraDisc, false, false);
        }
        private void tbtnRateDisc_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(tbtnRateDisc, true, false);
        }
        private void tbtnRateDisc_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(tbtnRateDisc, false, false);
        }
        private void txtFeatureSearch_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtFeatureSearch, true, false);
        }
        private void txtFeatureSearch_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtFeatureSearch, false, false);
        }
        private void cboDefaultTaxMode_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(cboDefaultTaxMode, true, false);
        }
        private void cboDefaultTaxMode_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(cboDefaultTaxMode, false, false);
        }
        private void tbtnTaxMode_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(tbtnTaxMode, true, false);
        }
        private void tbtnTaxMode_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(tbtnTaxMode, false, false);
        }
        private void cboDefaultModofPay_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(cboDefaultModofPay, true, false);
        }
        private void cboDefaultModofPay_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(cboDefaultModofPay, false, false);
        }
        private void tbtnMOP_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(tbtnMOP, true, false);
        }
        private void tbtnMOP_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(tbtnMOP, false, false);
        }
        private void cboDefaultSalesStaff_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(cboDefaultSalesStaff, true, false);
        }
        private void cboDefaultSalesStaff_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(cboDefaultSalesStaff, false, false);
        }
        private void tbtnSalesStaff_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(tbtnSalesStaff, true, false);
        }
        private void tbtnSalesStaff_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(tbtnSalesStaff, false, false);
        }
        private void cboDefaultAgent_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(cboDefaultAgent, true, false);
        }
        private void cboDefaultAgent_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(cboDefaultAgent, false, false);
        }
        private void tbtnAgent_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(tbtnAgent, true, false);
        }
        private void tbtnAgent_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(tbtnAgent, false, false);
        }
        private void txtFltItemType_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtFltItemType, false, false);
        }
        private void txtFltCategories_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtFltCategories, false, false);
        }
        private void txtFltCustGrp_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtFltCustGrp, false, false);
        }
        private void txtFltDrAccGrp_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtFltDrAccGrp, false, false);
        }
        private void txtFltCrAccGrp_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtFltCrAccGrp, false, false);
        }


        private void txtFeatureSearch_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string sGenSet = "";
                for (int g = 0; g < dgvFeatures.Rows.Count - 1; g++)
                {
                    sGenSet = dgvFeatures.Rows[g].Cells["General Settings"].Value.ToString().ToUpper();
                    if (sGenSet.Contains(txtFeatureSearch.Text.ToUpper()) && txtFeatureSearch.Text != "")
                    {
                        dgvFeatures.Rows[g].DefaultCellStyle.BackColor = Color.Yellow;
                    }
                    else if (txtFeatureSearch.Text == "")
                    {
                        dgvFeatures.Rows[g].DefaultCellStyle.BackColor = Color.White;
                        dgvFeatures.AlternatingRowsDefaultCellStyle.BackColor = Color.White;
                    }
                    else
                    {
                        dgvFeatures.Rows[g].DefaultCellStyle.BackColor = Color.White;
                    }
                }
            }
            catch (Exception) { }
        }
        private void cboParentTrans_SelectionChangeCommitted(object sender, EventArgs e)
        {


        }
        private void cboTransNumbering_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboTransNumbering.SelectedIndex == 2)
            {
                txtTransPrefix.Clear();
                txtTransPrefix.Enabled = false;
            }
            else
                txtTransPrefix.Enabled = true;
        }

        private void cboRefNumbering_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboRefNumbering.SelectedIndex == 2)
            {
                txtRefPrefix.Clear();
                txtRefPrefix.Enabled = false;
            }
            else
                txtRefPrefix.Enabled = true;
        }

        private void cboDefaultTaxInclusive_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
                tbtnAgent.Focus();
            else if (e.KeyCode == Keys.Enter)
                tbtnTaxInclusive.Focus();
        }

        private void tbtnTaxInclusive_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
                cboDefaultTaxInclusive.Focus();
            else if (e.KeyCode == Keys.Enter)
                rdoSearch.Focus();
        }

        private void cboDefaultTaxInclusive_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(cboDefaultTaxInclusive, true, false);
        }

        private void cboDefaultTaxInclusive_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(cboDefaultTaxInclusive, false, false);
        }

        private void tbtnTaxInclusive_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(tbtnTaxInclusive, true, false);
        }

        private void tbtnTaxInclusive_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(tbtnTaxInclusive, false, false);
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            decimal dVchTypID = 0;
            if (IsValidate() == true)
            {
                dVchTypID = SaveData();
                if (dVchTypID > 0)
                {
                    DeleteGeneralSettings(dVchTypID);
                    SaveFeaturesData(dVchTypID);
                }
                if (dIDFromEditWindow == 0)
                {
                    ClearAll();
                    LoadDefaults();
                    FillTransSortOrder();
                }

                Comm.MessageboxToasted("Voucher Type", "Voucher Type Updated successfully");

                if (dIDFromEditWindow!=0)
                    this.Close();
            }
            Cursor.Current = Cursors.Default;
        }

        private void cboParentTrans_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboParentTrans.SelectedIndex == 1)
                lblCustIds.Text = "11";
            else
                lblCustIds.Text = "10";

            txtFltCustGrp.Text = GetLedgerAsperIDs(lblCustIds.Text, "SUPPLIER");

            GrbBoardRate.Visible = false;
            if (cboParentTrans.SelectedValue != null)
            {
                if (Comm.ToInt32(cboParentTrans.SelectedValue.ToString()) == 40)
                    GrbBoardRate.Visible = true;
            }
        }

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(txtTransName.Text))
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
        #endregion

        #region "METHODS ----------------------------------------------- >>"
        //Description : Validating the Mandatory Fields Before Save Functionality
        private bool IsValidate()
        {
            bool bResult = true;
            if (txtTransName.Text.Trim() == "")
            {
                MessageBox.Show("Please Enter a Transction Name", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                bResult = false;
                txtTransName.Focus();
            }
            else if (cboParentTrans.SelectedIndex == -1)
            {
                MessageBox.Show("Please Select the Parent Voucher Type", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                bResult = false;
                cboParentTrans.Focus();
            }

            return bResult;
        }
        //Description : Load Default Values
        private void LoadDefaults()
        {
            cboTransNumbering.SelectedIndex = 0; //Auto Lock
            cboRefNumbering.SelectedIndex = 2; //Custom
            cboDefaultCostCenterPrimary.SelectedValue = 1; //Default
            cboDefaultCostCenterSecondary.SelectedValue = 1; //Default
            cboBillWiseXtraDisc.SelectedValue = 1; //Disabled
            cboItmWiseXtraDisc.SelectedValue = 1; //Disabled
            cboRoundoff.SelectedValue = 0; //Disabled
            txtRoundOffBlock.Text = "0"; //Disabled
            rdoBillWisePercWise.Checked = true;
            rdoItmWisePercWise.Checked = true;
            cboDefaultTaxMode.SelectedValue = 3; //GST
            cboDefaultModofPay.SelectedValue = 1; //Cash
            cboDefaultPriceList.SelectedValue = 1; //Cash
            cboDefaultSalesStaff.SelectedValue = 1; //Default
            cboDefaultAgent.SelectedValue = 1; // Default
            cboDefaultBarcodeMode.SelectedValue = 0;//From Item Master
            cboDefaultTaxInclusive.SelectedValue = 1;//From Item Master
            cboDefaultSearchMethod.SelectedValue = 1; //Anywhere
            tbtnItmSearchbydefault.ToggleState = Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active; //Show Serach By Default
            cboMMRPSubWindowSearchMod.SelectedValue = 1; // Qty Asc
            tbtnShowSearchWindowByDefault.ToggleState = Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active; //Show Serach Window By Default

            lblCustIds.Text = "10";
            txtFltCustGrp.Text = GetLedgerAsperIDs(lblCustIds.Text, "SUPPLIER");


            cboExportType.SelectedIndex = 0; //export type
            if (txtQuery.Text.Trim() == "")
                txtQuery.Text = "SELECT TOP (1000) [ItemID] ,[ItemCode] ,[ItemName] ,[BatchUnique] ,[PLUNumber] ,[MRP] ,[SRATE1] ,[SRATE2] ,[SRATE3] ,[SRATE4] ,[SRATE5] ,[Unit] ,[Expiry]   FROM  [vwBoardRatePLU] ";
            txtFileName.Text = "";

        }

        //Description : table LayotPanel Size adjustment when tab Click
        private void ShowFormsAsperClick(int iColIndex = 1)
        {
            for (int g = 0; g < this.tblpForms.ColumnCount; g++)
            {
                if (iColIndex == g + 1)
                {
                    this.tblpForms.ColumnStyles[g].SizeType = SizeType.Absolute;
                    this.tblpForms.ColumnStyles[g].Width = 800;
                }
                else
                {
                    this.tblpForms.ColumnStyles[g].SizeType = SizeType.Absolute;
                    this.tblpForms.ColumnStyles[g].Width = 0;
                }
            }
        }
        //Description : Fill Sort Order in Textbox
        private void FillTransSortOrder()
        {
            DataTable dtTransOrder = new DataTable();
            dtTransOrder = Comm.fnGetData("SELECT MAX(ISNULL(SortOrder, 0)) + 1 as SortOrder FROM tblVchType WHERE TenantID = " + Global.gblTenantID + "").Tables[0];
            if (dtTransOrder.Rows.Count > 0)
            {
                txtSortOrder.Text = dtTransOrder.Rows[0]["SortOrder"].ToString();
            }
        }
        //Description : Fill Parent Transaction in Transaction Combobox
        private void FillParentTransaction()
        {
            DataTable dtTrans = new DataTable();
            dtTrans = Comm.fnGetData("SELECT VchTypeID,VchType,ParentID From tblVchType WHERE ((VchTypeID BETWEEN 1 and 500) OR (VchTypeID >= 1005)) AND (ParentID = VchTypeID) ORDER BY SortOrder Asc").Tables[0];
            cboParentTrans.DataSource = dtTrans;
            cboParentTrans.DisplayMember = "VchType";
            cboParentTrans.ValueMember = "VchTypeID";
        }
        //Description : Get all  and  Selected Transaction details to show Checked Compact List
        private string GetTransAsperIDs(string sIDs = "")
        {
            string sRetResult = "";
            DataTable dtData = new DataTable();
            GetVchTyp.VchTypeID = 0;
            GetVchTyp.VchTypeIDs = sIDs;
            GetVchTyp.TenantID = Global.gblTenantID;
            dtData = clsVouchTyp.GetVchType(GetVchTyp);
            if (dtData.Rows.Count > 0)
            {
                sRetResult = dtData.Rows[0][0].ToString();
            }
            return sRetResult;
        }
        //Description : Set  Checked Transaction to Cursor Navigation TextBox 
        private Boolean GetFromCheckedParent(string sSelIDs)
        {
            lblTransIds.Text = sSelIDs;
            txtCusrsorNavList.Text = GetTransAsperIDs(sSelIDs);

            return true;
        }
        //Description : Fill Cost Centre in Combobox
        private void FillCostCenter(string sType = "PRIMARY", int iExceptid = 0)
        {
            DataTable dtCCPr = new DataTable();
            DataTable dtCCSc = new DataTable();

            if (sType == "PRIMARY")
            {
                dtCCPr = Comm.fnGetData("SELECT CCID,CCName,InCharge,Description1,Description2,Description3,BLNDAMAGED,SystemName,UserID,LastUpdateDate,LastUpdateTime,TenantID FROM tblCostCentre WHERE TenantID =  " + Global.gblTenantID + " ORDER BY CCName ASC").Tables[0];
                if (dtCCPr.Rows.Count > 0)
                {
                    cboDefaultCostCenterPrimary.DataSource = dtCCPr;
                    cboDefaultCostCenterPrimary.DisplayMember = "CCName";
                    cboDefaultCostCenterPrimary.ValueMember = "CCID";

                    cboDefaultCostCenterPrimary.SelectedIndex = -1;
                }
            }
            else if (sType == "SECONDARY")
            {
                dtCCSc = Comm.fnGetData("SELECT CCID,CCName,InCharge,Description1,Description2,Description3,BLNDAMAGED,SystemName,UserID,LastUpdateDate,LastUpdateTime,TenantID FROM tblCostCentre WHERE TenantID =  " + Global.gblTenantID + " ORDER BY CCName ASC").Tables[0];
                if (dtCCSc.Rows.Count > 0)
                {
                    cboDefaultCostCenterSecondary.DataSource = dtCCSc;
                    cboDefaultCostCenterSecondary.DisplayMember = "CCName";
                    cboDefaultCostCenterSecondary.ValueMember = "CCID";

                    cboDefaultCostCenterSecondary.SelectedIndex = -1;
                }
            }
        }
        //Description : Fill Tax Mode in Combobox
        private void FillTaxModeForDefault()
        {
            DataTable dtTaxMod = new DataTable();
            dtTaxMod = Comm.fnGetData("SELECT TaxModeID,TaxMode FROM tblTaxMode WHERE TenantID = " + Global.gblTenantID + " ORDER BY SortNo ASC").Tables[0];

            cboDefaultTaxMode.DataSource = dtTaxMod;
            cboDefaultTaxMode.DisplayMember = "TaxMode";
            cboDefaultTaxMode.ValueMember = "TaxModeID";
        }
        //Description : Fill Staff in Combobox
        private void FillSalesStaffEmpForDefault()
        {
            DataTable dtEmp = new DataTable();
            dtEmp = Comm.fnGetData("SELECT EmpID,Name FROM tblEmployee WHERE Active = 1 AND TenantID = " + Global.gblTenantID + " ORDER BY Name ASC").Tables[0];

            cboDefaultSalesStaff.DataSource = dtEmp;
            cboDefaultSalesStaff.DisplayMember = "Name";
            cboDefaultSalesStaff.ValueMember = "EmpID";
            cboDefaultSalesStaff.SelectedIndex = -1;
        }
        //Description : Fill Agent in Combobox
        private void FillAgentForDefault(int iSelectedID = 0)
        {
            clsAgentMaster clsAgent = new clsAgentMaster();
            UspGetAgentinfo GetAgent = new UspGetAgentinfo();
            DataTable dtAgent = new DataTable();
            GetAgent.AgentID = iSelectedID;
            GetAgent.TenantID = Global.gblTenantID;
            dtAgent = clsAgent.GetAgentMaster(GetAgent);
            cboDefaultAgent.DataSource = dtAgent;
            cboDefaultAgent.DisplayMember = "Agent Name";
            cboDefaultAgent.ValueMember = "AgentID";
            cboDefaultAgent.SelectedIndex = -1;
        }
        //Description : Fill Mode of Payment in Combobox
        private void FillModofPayForDefault()
        {
            DataTable dtData = new DataTable();
            dtData.Clear();

            dtData.Columns.Add("ModPayID");
            dtData.Columns.Add("ModPayDescr");

            DataRow dRow1 = dtData.NewRow();
            dRow1["ModPayID"] = "1";
            dRow1["ModPayDescr"] = "Cash";
            dtData.Rows.Add(dRow1);

            DataRow dRow2 = dtData.NewRow();
            dRow2["ModPayID"] = "2";
            dRow2["ModPayDescr"] = "Credit";
            dtData.Rows.Add(dRow2);

            DataRow dRow3 = dtData.NewRow();
            dRow3["ModPayID"] = "3";
            dRow3["ModPayDescr"] = "Mixed";
            dtData.Rows.Add(dRow3);

            if (cboParentTrans.SelectedValue.ToString() == "1")
            {
                DataRow dRow4 = dtData.NewRow();
                dRow4["ModPayID"] = "4";
                dRow4["ModPayDescr"] = "Counter";
                dtData.Rows.Add(dRow4);
            }

            cboDefaultModofPay.DataSource = dtData;
            cboDefaultModofPay.DisplayMember = "ModPayDescr";
            cboDefaultModofPay.ValueMember = "ModPayID";
        }
        //Description : Fill Tax Inclusive Settings in Combobox
        private void FillTaxInclusiveSettings()
        {
            DataTable dtData = new DataTable();
            dtData.Clear();

            dtData.Columns.Add("TaxInclsvID");
            dtData.Columns.Add("TaxInclsvDescr");

            DataRow dRow1 = dtData.NewRow();
            dRow1["TaxInclsvID"] = "1";
            dRow1["TaxInclsvDescr"] = "From Item Master";
            dtData.Rows.Add(dRow1);

            DataRow dRow2 = dtData.NewRow();
            dRow2["TaxInclsvID"] = "2";
            dRow2["TaxInclsvDescr"] = "Always Inclusive";
            dtData.Rows.Add(dRow2);

            DataRow dRow3 = dtData.NewRow();
            dRow3["TaxInclsvID"] = "3";
            dRow3["TaxInclsvDescr"] = "Always Exclusive";
            dtData.Rows.Add(dRow3);

            cboDefaultTaxInclusive.DataSource = dtData;
            cboDefaultTaxInclusive.DisplayMember = "TaxInclsvDescr";
            cboDefaultTaxInclusive.ValueMember = "TaxInclsvID";
        }
        //Description : Fill Barcode Mode Settings in Combobox
        private void FillBarcodeMode()
        {
            try
            {
                DataTable dtData = new DataTable();
                dtData.Clear();

                dtData.Columns.Add("BarModeID");
                dtData.Columns.Add("BarMode");

                DataRow dRow1 = dtData.NewRow();
                dRow1["BarModeID"] = "0";
                dRow1["BarMode"] = "Barcode Dropdown";
                dtData.Rows.Add(dRow1);

                if (cboParentTrans.SelectedValue != null)
                {
                    if (cboParentTrans.SelectedValue.ToString() == "1" || cboParentTrans.SelectedValue.ToString() == "3" || cboParentTrans.SelectedValue.ToString() == "5")
                    {
                        DataRow dRow2 = dtData.NewRow();
                        dRow2["BarModeID"] = "1";
                        dRow2["BarMode"] = "Barcode Scanning";
                        dtData.Rows.Add(dRow2);

                        DataRow dRow3 = dtData.NewRow();
                        dRow3["BarModeID"] = "2";
                        dRow3["BarMode"] = "Barcode Keyboard";
                        dtData.Rows.Add(dRow3);
                    }
                }
                cboDefaultBarcodeMode.DataSource = dtData;
                cboDefaultBarcodeMode.DisplayMember = "BarMode";
                cboDefaultBarcodeMode.ValueMember = "BarModeID";
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
        }
        //Description : Fill Billwise Extra Discount in Combobox
        private void FillBillWiseXtraDiscount()
        {
            try
            { 
                DataTable dtData = new DataTable();
                dtData.Clear();

                dtData.Columns.Add("BDiscID");
                dtData.Columns.Add("BDiscDescription");

                DataRow dRow1 = dtData.NewRow();
                dRow1["BDiscID"] = "1";
                dRow1["BDiscDescription"] = "<Disabled>";
                dtData.Rows.Add(dRow1);

                DataRow dRow2 = dtData.NewRow();
                dRow2["BDiscID"] = "2";
                dRow2["BDiscDescription"] = "Agent Discount";
                dtData.Rows.Add(dRow2);

                DataRow dRow3 = dtData.NewRow();
                dRow3["BDiscID"] = "3";
                dRow3["BDiscDescription"] = "Customer Discount";
                dtData.Rows.Add(dRow3);

                cboBillWiseXtraDisc.DataSource = dtData;
                cboBillWiseXtraDisc.DisplayMember = "BDiscDescription";
                cboBillWiseXtraDisc.ValueMember = "BDiscID";
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
        }
        //Description : Fill Itemwise Extra Discount in Combobox
        private void FillItemWiseXtraDiscount()
        {
            try
            {
                DataTable dtData = new DataTable();
                dtData.Clear();

                dtData.Columns.Add("TtmDiscID");
                dtData.Columns.Add("ItmDiscDescription");

                DataRow dRow1 = dtData.NewRow();
                dRow1["TtmDiscID"] = "1";
                dRow1["ItmDiscDescription"] = "<Disabled>";
                dtData.Rows.Add(dRow1);

                DataRow dRow2 = dtData.NewRow();
                dRow2["TtmDiscID"] = "2";
                dRow2["ItmDiscDescription"] = "Item Discount";
                dtData.Rows.Add(dRow2);

                DataRow dRow3 = dtData.NewRow();
                dRow3["TtmDiscID"] = "3";
                dRow3["ItmDiscDescription"] = "Category Discount";
                dtData.Rows.Add(dRow3);

                DataRow dRow4 = dtData.NewRow();
                dRow4["TtmDiscID"] = "4";
                dRow4["ItmDiscDescription"] = "Manufacturer Discount";
                dtData.Rows.Add(dRow4);

                DataRow dRow5 = dtData.NewRow();
                dRow5["TtmDiscID"] = "5";
                dRow5["ItmDiscDescription"] = "Discount Group Discount";
                dtData.Rows.Add(dRow5);

                DataRow dRow6 = dtData.NewRow();
                dRow6["TtmDiscID"] = "6";
                dRow6["ItmDiscDescription"] = "Highest Discount";
                dtData.Rows.Add(dRow6);

                cboItmWiseXtraDisc.DataSource = dtData;
                cboItmWiseXtraDisc.DisplayMember = "ItmDiscDescription";
                cboItmWiseXtraDisc.ValueMember = "TtmDiscID";
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);

            }

        }
        //Description : Fill Default Search Method in Combobox
        private void FillDefaultSearchmethod()
        {
            try
            { 
            DataTable dtData = new DataTable();
            dtData.Clear();

            dtData.Columns.Add("MethodID");
            dtData.Columns.Add("MethodDesc");

            DataRow dRow1 = dtData.NewRow();
            dRow1["MethodID"] = "1";
            dRow1["MethodDesc"] = "Anywhere";
            dtData.Rows.Add(dRow1);

            DataRow dRow2 = dtData.NewRow();
            dRow2["MethodID"] = "2";
            dRow2["MethodDesc"] = "ItemCode";
            dtData.Rows.Add(dRow2);

            DataRow dRow3 = dtData.NewRow();
            dRow3["MethodID"] = "3";
            dRow3["MethodDesc"] = "ItemName";
            dtData.Rows.Add(dRow3);

            cboDefaultSearchMethod.DataSource = dtData;
            cboDefaultSearchMethod.DisplayMember = "MethodDesc";
            cboDefaultSearchMethod.ValueMember = "MethodID";
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
        }
        //Description : Fill MMRP Subwindow in Combobox
        private void FillMMRPSubWindowSearchMod()
        {
            try
            { 
            DataTable dtData = new DataTable();
            dtData.Clear();

            dtData.Columns.Add("ModeID");
            dtData.Columns.Add("ModeDesc");

            DataRow dRow1 = dtData.NewRow();
            dRow1["ModeID"] = "1";
            dRow1["ModeDesc"] = "Sort By Qty Asc";
            dtData.Rows.Add(dRow1);

            DataRow dRow2 = dtData.NewRow();
            dRow2["ModeID"] = "2";
            dRow2["ModeDesc"] = "Sort By Qty Desc";
            dtData.Rows.Add(dRow2);

            DataRow dRow3 = dtData.NewRow();
            dRow3["ModeID"] = "3";
            dRow3["ModeDesc"] = "Sort By MRP Asc";
            dtData.Rows.Add(dRow3);

            DataRow dRow4 = dtData.NewRow();
            dRow4["ModeID"] = "4";
            dRow4["ModeDesc"] = "Sort By MRP Desc";
            dtData.Rows.Add(dRow4);

            DataRow dRow5 = dtData.NewRow();
            dRow5["ModeID"] = "5";
            dRow5["ModeDesc"] = "Sort By Expiry Asc";
            dtData.Rows.Add(dRow5);

            DataRow dRow6 = dtData.NewRow();
            dRow6["ModeID"] = "6";
            dRow6["ModeDesc"] = "Sort By Expiry Desc";
            dtData.Rows.Add(dRow6);

            cboMMRPSubWindowSearchMod.DataSource = dtData;
            cboMMRPSubWindowSearchMod.DisplayMember = "ModeDesc";
            cboMMRPSubWindowSearchMod.ValueMember = "ModeID";

                //cboMMRPSubWindowSearchMod.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
        }
        //Description : Get all  and  Selected Categories details to show Checked Compact List
        private string GetCategoriesAsperIDs(string sIDs = "")
        {
            try
            { 
            UspGetCategoryCheckedListInfo GetCatChk = new UspGetCategoryCheckedListInfo();
            clsCategory clsCat = new clsCategory();
            string sRetResult = "";
            if (sIDs != "")
            {
                DataTable dtData = new DataTable();
                GetCatChk.IDs = sIDs;
                GetCatChk.TenantId = Global.gblTenantID;
                dtData = clsCat.GetCategoryCheckedList(GetCatChk);
                if (dtData.Rows.Count > 0)
                {
                    sRetResult = dtData.Rows[0][0].ToString();
                }
            }
            return sRetResult;
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return "";
            }
        }
        //Description : Get all  and  Selected Ledger details to show Checked Compact List
        private string GetLedgerAsperIDs(string sIDs = "", string sType = "")
        {
            try
            { 
            string sRetResult = "";
            if (sIDs != "")
            {
                DataTable dtData = new DataTable();
                dtData = Comm.fnGetData("EXEC UspGetLedgerForCheckedList '" + sIDs + "'," + Global.gblTenantID + ",'" + sType.ToUpper() + "'").Tables[0];
                if (dtData.Rows.Count > 0)
                {
                    sRetResult = dtData.Rows[0][0].ToString();
                }
            }
            return sRetResult;
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return "";

            }
        }
        //Description : Get all  and  Selected Product Class to show Checked Compact List
        //Commented and Added By Arun 20/09/2022 05:17 PM
        private string GetProductClsAsperIDs(string sIDs = "")
        {
            try
            {
                string sRetResult = "";
                if (sIDs != "")
                {
                    DataTable dtData = LoadProductClass().Select(" ProdTypeID in (" + sIDs + ")").CopyToDataTable();
                    if (dtData.Rows.Count > 0)
                    {
                        for(int i=0; i <= dtData.Rows.Count-1; i++)
                            sRetResult = sRetResult + "," + dtData.Rows[i][1].ToString();
                    }
                }
                return sRetResult;
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return "";

            }
        }

        //Commented and Added By Anjitha 08/03/2022 12:40 PM
        //private string GetProductClsAsperIDs(string sIDs = "")
        //{
        //    try
        //    { 
        //    string sRetResult = "";
        //    if (sIDs != "")
        //    {
        //        DataTable dtData = new DataTable();
        //        GetOtminfo.OtmIds = sIDs;
        //        GetOtminfo.TenantID = Global.gblTenantID;
        //        GetOtminfo.OtmType = "PRODCLASS";
        //        dtData = clsOtm.GetOnetimeMasterCheckedList(GetOtminfo);
        //        if (dtData.Rows.Count > 0)
        //        {
        //            sRetResult = dtData.Rows[0][0].ToString();
        //        }
        //    }
        //    return sRetResult;
        //    }
        //    catch (Exception ex)
        //    {
        //        Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
        //        MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
        //        return "";

        //    }
        //}
        ///Description : Set  Checked Transaction to FillCustomer TextBox 
        private Boolean GetFromCustomerCheckedList(string sSelIDs)
        {
            try
            { 
            lblCustIds.Text = sSelIDs;
            txtFltCustGrp.Text = GetLedgerAsperIDs(sSelIDs, "SUPPLIER");
            return true;
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;

            }
        }
        //Description : Set  Checked Transaction to Category TextBox 
        private Boolean GetFromCheckedList(string sSelIDs)
        {
            try
            {
            lblCategoryIds.Text = sSelIDs;
            txtFltCategories.Text = GetCategoriesAsperIDs(sSelIDs);
            return true;
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
        }
        //Description : Set  Checked Transaction to Fill LedgerDebit TextBox 
        private Boolean GetFromLedgerDrCheckedList(string sSelIDs)
        {
            try
            { 
            lblDrAccGrpIds.Text = sSelIDs;
            txtFltDrAccGrp.Text = GetLedgerAsperIDs(sSelIDs, "DR-LEDGER");
            return true;
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;

            }
        }
        //Description : Set  Checked Transaction to Fill LedgerCredit TextBox 
        private Boolean GetFromLedgerCrCheckedList(string sSelIDs)
        {
            try
            { 
            lblCrAccGrpIds.Text = sSelIDs;
            txtFltCrAccGrp.Text = GetLedgerAsperIDs(sSelIDs, "CR-LEDGER");
            return true;
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;

            }
        }
        //Description : Set  Checked Transaction to Product Class TextBox 
        private Boolean GetFromProductClassCheckedList(string sSelIDs)
        {
            try
            { 
                lblItmTypeIds.Text = sSelIDs;
                txtFltItemType.Text = GetProductClsAsperIDs(sSelIDs);
                return true;
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
        }

        private DataTable GetVchtypeFeatures()
        {
            try
            {
                DataTable dt = new DataTable();

                dt.Columns.Add("SettingsName");
                dt.Columns.Add("SettingsDescription");

                if (cboParentTrans.SelectedValue == null) return new DataTable();

                switch (cboParentTrans.SelectedValue.ToString())
                {
                    case "1": //Sales
                        {
                            //dt.Rows.Add("BLNAPPLYGIFTVOUCHER", "Apply Gift Voucher");
                            //dt.Rows.Add("BLNAPPLYOFFER", "Apply Offer Automatically");
                            dt.Rows.Add("BLNCASHDESK", "Enable Cash Desk");
                            dt.Rows.Add("BLNPOSTCASHENTRY", "Post Cash Entry In Ledger");
                            dt.Rows.Add("BLNDISPLAYLEDGERBALANCE", "Display Ledger Balance On Selection");
                            dt.Rows.Add("BLNNAMEANDMOBILENOMANDATORY", "Party Name & Mobile No Mandatory");
                            dt.Rows.Add("BLNEDITMRPRATE", "Allow User To Edit MRP In Grid");
                            dt.Rows.Add("BLNEDITSALERATE", "Allow User To Edit SRate");
                            dt.Rows.Add("BLNSHOWFREEQUANTITY", "Allow User To Enter Free Qty");
                            dt.Rows.Add("BLNEDITTAXPER", "Allow User To Edit Tax Percentage");
                            dt.Rows.Add("BLNAUTOCHANGERATEONPRICELIST", "Auto Change SRate On Pricelist Selection");
                            dt.Rows.Add("BLNSHOWPRICELIST", "Show Pricelist");
                            dt.Rows.Add("BLNSHOWOFFERINPOPUP", "Show Offer In Popup");
                            dt.Rows.Add("BLNPARTYDETAILS", "Show Party Details");
                            dt.Rows.Add("BLNENABLECASHDISCOUNT", "Show Cash Discount");
                            dt.Rows.Add("BLNENABLEEFFECIVEDATE", "Show Effective Date");
                            dt.Rows.Add("blnShowReferenceNo", "Show Reference No");
                            dt.Rows.Add("BLNSHOWBILLNARRATION", "Show Bill Narration");
                            dt.Rows.Add("BLNSHOWOTHEREXPENSE", "Show Other Expense");
                            dt.Rows.Add("BLNSHOWITEMCALCGRID", "Show Item Calculation Grid");
                            dt.Rows.Add("BLNSHOWITEMPROFITPER", "Show Item Previous Rates");
                            dt.Rows.Add("BLNSHOWPREVIEW", "Show Preview Before Print");
                            dt.Rows.Add("BLNPRINTCONFIRMATION", "Ask Confirmation On Print");
                            dt.Rows.Add("BLNPRINTIMMEDIATELY", "Send Bill to Printer on Save");
                            dt.Rows.Add("BLNSUMMARISEDUPLICATEITEMSINPRINT", "Summarise Duplicate Items In Print");
                            dt.Rows.Add("BLNSUMMARISEITEMSWHILEENTERING", "Summarise Duplicate Items While Entering");
                            //dt.Rows.Add("BLNSUMMARISEITEMSWHILEENTERING", "Summarise Items While Entering");



                            break;
                        }

                    case "2": //Purchase
                        {
                            dt.Rows.Add("BLNPOSTCASHENTRY", "Post Cash Entry In Ledger");
                            dt.Rows.Add("BLNRECALCULATESalesRatesOnPercentage", "Recalculate Sales Rates On Percentage");
                            dt.Rows.Add("blnShowRateFixer", "Enable Rate Fixer");
                            dt.Rows.Add("BLNCASHDESK", "Enable Cash Desk");
                            dt.Rows.Add("blnWarnifSRatelessthanPrate", "Warn If Sales Rate Is Less Than Purchase Rate");
                            dt.Rows.Add("BLNEDITMRPRATE", "Allow User To Edit MRP");
                            dt.Rows.Add("blneditsalerate", "Allow User To Edit Rate");
                            dt.Rows.Add("BLNSHOWFREEQUANTITY", "Allow User To Enter Free Qty");
                            dt.Rows.Add("blnEditTaxPer", "Allow User To Edit Tax Percentage");
                            dt.Rows.Add("blnpartydetails", "Show Party Details");
                            dt.Rows.Add("blnenablecashdiscount", "Show Cash Discount");
                            dt.Rows.Add("blnenableEffeciveDate", "Show Effective Date");
                            dt.Rows.Add("blnShowReferenceNo", "Show Reference No");
                            dt.Rows.Add("blnshowotherexpense", "Show Other Expenses");
                            dt.Rows.Add("blnshowbillnarration", "Show Bill Narration");
                            dt.Rows.Add("blnShowItemCalcGrid", "Show Item Calculation Grid");
                            dt.Rows.Add("blnShowItemProfitPer", "Show Product Profit Percentage");
                            dt.Rows.Add("blnshowpreview", "Show Preview Before Print");
                            dt.Rows.Add("blnprintconfirmation", "Ask Confirmation On Print");
                            dt.Rows.Add("blnprintimmediately", "Send Bill To Printer On Save");
                            dt.Rows.Add("blnSummariseDuplicateItemsInPrint", "Summarise Duplicate Items In Print");
                            dt.Rows.Add("blnSummariseItemsWhileEntering", "Summarise Duplicate Items While Entering");


                            break;
                        }

                    case "3": //Sales Return
                        {
                            //dt.Rows.Add("BLNAPPLYGIFTVOUCHER", "Apply Gift Voucher");
                            //dt.Rows.Add("BLNAPPLYOFFER", "Apply Offer Automatically");
                            dt.Rows.Add("BLNCASHDESK", "Enable Cash Desk");
                            dt.Rows.Add("BLNPOSTCASHENTRY", "Post Cash Entry In Ledger");
                            dt.Rows.Add("BLNDISPLAYLEDGERBALANCE", "Display Ledger Balance On Selection");
                            dt.Rows.Add("BLNNAMEANDMOBILENOMANDATORY", "Party Name & Mobile No Mandatory");
                            dt.Rows.Add("BLNEDITMRPRATE", "Allow User To Edit MRP In Grid");
                            dt.Rows.Add("BLNEDITSALERATE", "Allow User To Edit SRate");
                            dt.Rows.Add("BLNSHOWFREEQUANTITY", "Allow User To Enter Free Qty");
                            dt.Rows.Add("BLNEDITTAXPER", "Allow User To Edit Tax Percentage");
                            dt.Rows.Add("BLNAUTOCHANGERATEONPRICELIST", "Auto Change SRate On Pricelist Selection");
                            dt.Rows.Add("BLNSHOWPRICELIST", "Show Pricelist");
                            dt.Rows.Add("BLNSHOWOFFERINPOPUP", "Show Offer In Popup");
                            dt.Rows.Add("BLNPARTYDETAILS", "Show Party Details");
                            dt.Rows.Add("BLNENABLECASHDISCOUNT", "Show Cash Discount");
                            dt.Rows.Add("BLNENABLEEFFECIVEDATE", "Show Effective Date");
                            dt.Rows.Add("blnShowReferenceNo", "Show Reference No");
                            dt.Rows.Add("BLNSHOWBILLNARRATION", "Show Bill Narration");
                            dt.Rows.Add("BLNSHOWOTHEREXPENSE", "Show Other Expense");
                            dt.Rows.Add("BLNSHOWITEMCALCGRID", "Show Item Calculation Grid");
                            dt.Rows.Add("BLNSHOWITEMPROFITPER", "Show Item Previous Rates");
                            dt.Rows.Add("BLNSHOWPREVIEW", "Show Preview Before Print");
                            dt.Rows.Add("BLNPRINTCONFIRMATION", "Ask Confirmation On Print");
                            dt.Rows.Add("BLNPRINTIMMEDIATELY", "Send Bill to Printer on Save");
                            dt.Rows.Add("BLNSUMMARISEDUPLICATEITEMSINPRINT", "Summarise Duplicate Items In Print");
                            dt.Rows.Add("BLNSUMMARISEITEMSWHILEENTERING", "Summarise Duplicate Items While Entering");
                            //dt.Rows.Add("BLNSUMMARISEITEMSWHILEENTERING", "Summarise Items While Entering");

                            break;
                        }

                    case "4": //Purchase return
                        {
                            dt.Rows.Add("BLNPOSTCASHENTRY", "Post Cash Entry In Ledger");
                            dt.Rows.Add("BLNRECALCULATESalesRatesOnPercentage", "Recalculate Sales Rates On Percentage");
                            dt.Rows.Add("blnShowRateFixer", "Enable Rate Fixer");
                            dt.Rows.Add("BLNCASHDESK", "Enable Cash Desk");
                            dt.Rows.Add("blnWarnifSRatelessthanPrate", "Warn If Sales Rate Is Less Than Purchase Rate");
                            dt.Rows.Add("BLNEDITMRPRATE", "Allow User To Edit MRP");
                            dt.Rows.Add("blneditsalerate", "Allow User To Edit Rate");
                            dt.Rows.Add("BLNSHOWFREEQUANTITY", "Allow User To Enter Free Qty");
                            dt.Rows.Add("blnEditTaxPer", "Allow User To Edit Tax Percentage");
                            dt.Rows.Add("blnpartydetails", "Show Party Details");
                            dt.Rows.Add("blnenablecashdiscount", "Show Cash Discount");
                            dt.Rows.Add("blnenableEffeciveDate", "Show Effective Date");
                            dt.Rows.Add("blnShowReferenceNo", "Show Reference No");
                            dt.Rows.Add("blnshowotherexpense", "Show Other Expenses");
                            dt.Rows.Add("blnshowbillnarration", "Show Bill Narration");
                            dt.Rows.Add("blnShowItemCalcGrid", "Show Item Calculation Grid");
                            dt.Rows.Add("blnShowItemProfitPer", "Show Product Profit Percentage");
                            dt.Rows.Add("blnshowpreview", "Show Preview Before Print");
                            dt.Rows.Add("blnprintconfirmation", "Ask Confirmation On Print");
                            dt.Rows.Add("blnprintimmediately", "Send Bill To Printer On Save");
                            dt.Rows.Add("blnSummariseDuplicateItemsInPrint", "Summarise Duplicate Items In Print");
                            dt.Rows.Add("blnSummariseItemsWhileEntering", "Summarise Duplicate Items While Entering");

                            break;
                        }
                    case "5": //Delivery Note
                        {
                            //dt.Rows.Add("BLNAPPLYGIFTVOUCHER", "Apply Gift Voucher");
                            //dt.Rows.Add("BLNAPPLYOFFER", "Apply Offer Automatically");
                            dt.Rows.Add("BLNCASHDESK", "Enable Cash Desk");
                            dt.Rows.Add("BLNPOSTCASHENTRY", "Post Cash Entry In Ledger");
                            dt.Rows.Add("BLNDISPLAYLEDGERBALANCE", "Display Ledger Balance On Selection");
                            dt.Rows.Add("BLNNAMEANDMOBILENOMANDATORY", "Party Name & Mobile No Mandatory");
                            dt.Rows.Add("BLNEDITMRPRATE", "Allow User To Edit MRP In Grid");
                            dt.Rows.Add("BLNEDITSALERATE", "Allow User To Edit SRate");
                            dt.Rows.Add("BLNSHOWFREEQUANTITY", "Allow User To Enter Free Qty");
                            dt.Rows.Add("BLNEDITTAXPER", "Allow User To Edit Tax Percentage");
                            dt.Rows.Add("BLNAUTOCHANGERATEONPRICELIST", "Auto Change SRate On Pricelist Selection");
                            dt.Rows.Add("BLNSHOWPRICELIST", "Show Pricelist");
                            dt.Rows.Add("BLNSHOWOFFERINPOPUP", "Show Offer In Popup");
                            dt.Rows.Add("BLNPARTYDETAILS", "Show Party Details");
                            dt.Rows.Add("BLNENABLECASHDISCOUNT", "Show Cash Discount");
                            dt.Rows.Add("BLNENABLEEFFECIVEDATE", "Show Effective Date");
                            dt.Rows.Add("blnShowReferenceNo", "Show Reference No");
                            dt.Rows.Add("BLNSHOWBILLNARRATION", "Show Bill Narration");
                            dt.Rows.Add("BLNSHOWOTHEREXPENSE", "Show Other Expense");
                            dt.Rows.Add("BLNSHOWITEMCALCGRID", "Show Item Calculation Grid");
                            dt.Rows.Add("BLNSHOWITEMPROFITPER", "Show Item Previous Rates");
                            dt.Rows.Add("BLNSHOWPREVIEW", "Show Preview Before Print");
                            dt.Rows.Add("BLNPRINTCONFIRMATION", "Ask Confirmation On Print");
                            dt.Rows.Add("BLNPRINTIMMEDIATELY", "Send Bill to Printer on Save");
                            dt.Rows.Add("BLNSUMMARISEDUPLICATEITEMSINPRINT", "Summarise Duplicate Items In Print");
                            dt.Rows.Add("BLNSUMMARISEITEMSWHILEENTERING", "Summarise Duplicate Items While Entering");
                            //dt.Rows.Add("BLNSUMMARISEITEMSWHILEENTERING", "Summarise Items While Entering");

                            break;
                        }
                    case "6": //Receipt Note
                        {
                            dt.Rows.Add("BLNPOSTCASHENTRY", "Post Cash Entry In Ledger");
                            dt.Rows.Add("BLNRECALCULATESalesRatesOnPercentage", "Recalculate Sales Rates On Percentage");
                            dt.Rows.Add("blnShowRateFixer", "Enable Rate Fixer");
                            dt.Rows.Add("BLNCASHDESK", "Enable Cash Desk");
                            dt.Rows.Add("blnWarnifSRatelessthanPrate", "Warn If Sales Rate Is Less Than Purchase Rate");
                            dt.Rows.Add("BLNEDITMRPRATE", "Allow User To Edit MRP");
                            dt.Rows.Add("blneditsalerate", "Allow User To Edit Rate");
                            dt.Rows.Add("BLNSHOWFREEQUANTITY", "Allow User To Enter Free Qty");
                            dt.Rows.Add("blnEditTaxPer", "Allow User To Edit Tax Percentage");
                            dt.Rows.Add("blnpartydetails", "Show Party Details");
                            dt.Rows.Add("blnenablecashdiscount", "Show Cash Discount");
                            dt.Rows.Add("blnenableEffeciveDate", "Show Effective Date");
                            dt.Rows.Add("blnShowReferenceNo", "Show Reference No");
                            dt.Rows.Add("blnshowotherexpense", "Show Other Expenses");
                            dt.Rows.Add("blnshowbillnarration", "Show Bill Narration");
                            dt.Rows.Add("blnShowItemCalcGrid", "Show Item Calculation Grid");
                            dt.Rows.Add("blnShowItemProfitPer", "Show Product Profit Percentage");
                            dt.Rows.Add("blnshowpreview", "Show Preview Before Print");
                            dt.Rows.Add("blnprintconfirmation", "Ask Confirmation On Print");
                            dt.Rows.Add("blnprintimmediately", "Send Bill To Printer On Save");
                            dt.Rows.Add("blnSummariseDuplicateItemsInPrint", "Summarise Duplicate Items In Print");
                            dt.Rows.Add("blnSummariseItemsWhileEntering", "Summarise Duplicate Items While Entering");

                            break;
                        }
                    case "7": //Receipt
                        {
                            dt.Rows.Add("CHEQUEPRINTING", "Enable Cheque Printing");
                            dt.Rows.Add("BLNALLOWDUPLICATELEDGERS", "Allow Duplicate Ledgers");
                            dt.Rows.Add("BLNDISABLEANYWHERESEARCHINGOFLEDGERS", "Disable Anywhere Searching Of Ledgers");
                            //dt.Rows.Add("BLNDUALENTRYMODE", "Dual Entry Mode For Ledgers");
                            dt.Rows.Add("BLNDISPLAYLEDGERBALANCE", "Display Ledger Balance On Selection");
                            dt.Rows.Add("BLNPOSTONEFFECTIVEDATE", "Post Accounts On Effective Date");
                            dt.Rows.Add("BLNENABLEEFFECIVEDATE", "Show Effective Date");
                            dt.Rows.Add("BLNCHEQUEDETAILS", "Show Cheque Details");
                            dt.Rows.Add("BLNSHOWLEDGERBALANCES", "Show Ledger Balances");
                            dt.Rows.Add("BLNPARTYDETAILS", "Show Party Details");
                            dt.Rows.Add("BLNSHOWREFERENCENO", "Show Reference Number");
                            dt.Rows.Add("BLNSHOWBILLNARRATION", "Show Bill Narration");
                            dt.Rows.Add("BLNSHOWITEMCALCGRID", "Show Item Calculation Grid");
                            dt.Rows.Add("BLNSHOWQTY", "Show Qty Column");
                            dt.Rows.Add("BLNSHOWLEDGERBALANCES", "Show Ledger Balances");
                            //dt.Rows.Add("BLNSHOWLEDGERBALANCES", "Show Ledger Balances");
                            dt.Rows.Add("BLNSHOWPREVIEW", "Show Preview Before Print");
                            dt.Rows.Add("blnprintconfirmation", "Ask Confirmation on Print");
                            dt.Rows.Add("BLNPRINTIMMEDIATELY", "Send Bill to Printer On Save");

                            break;
                        }
                    case "8": //Payment
                        {
                            dt.Rows.Add("CHEQUEPRINTING", "Enable Cheque Printing");
                            dt.Rows.Add("BLNALLOWDUPLICATELEDGERS", "Allow Duplicate Ledgers");
                            dt.Rows.Add("BLNDISABLEANYWHERESEARCHINGOFLEDGERS", "Disable Anywhere Searching Of Ledgers");
                            //dt.Rows.Add("BLNDUALENTRYMODE", "Dual Entry Mode For Ledgers");
                            dt.Rows.Add("BLNDISPLAYLEDGERBALANCE", "Display Ledger Balance On Selection");
                            dt.Rows.Add("BLNPOSTONEFFECTIVEDATE", "Post Accounts On Effective Date");
                            dt.Rows.Add("BLNENABLEEFFECIVEDATE", "Show Effective Date");
                            dt.Rows.Add("BLNCHEQUEDETAILS", "Show Cheque Details");
                            dt.Rows.Add("BLNSHOWLEDGERBALANCES", "Show Ledger Balances");
                            dt.Rows.Add("BLNPARTYDETAILS", "Show Party Details");
                            dt.Rows.Add("BLNSHOWREFERENCENO", "Show Reference Number");
                            dt.Rows.Add("BLNSHOWBILLNARRATION", "Show Bill Narration");
                            dt.Rows.Add("BLNSHOWITEMCALCGRID", "Show Item Calculation Grid");
                            dt.Rows.Add("BLNSHOWQTY", "Show Qty Column");
                            dt.Rows.Add("BLNSHOWLEDGERBALANCES", "Show Ledger Balances");
                            dt.Rows.Add("BLNSHOWLEDGERBALANCES", "Show Ledger Balances");
                            dt.Rows.Add("BLNSHOWPREVIEW", "Show Preview Before Print");
                            dt.Rows.Add("blnprintconfirmation", "Ask Confirmation on Print");
                            dt.Rows.Add("BLNPRINTIMMEDIATELY", "Send Bill to Printer On Save");

                            break;
                        }
                    case "9": //Journal
                        {
                            dt.Rows.Add("CHEQUEPRINTING", "Enable Cheque Printing");
                            dt.Rows.Add("BLNALLOWDUPLICATELEDGERS", "Allow Duplicate Ledgers");
                            dt.Rows.Add("BLNDISABLEANYWHERESEARCHINGOFLEDGERS", "Disable Anywhere Searching Of Ledgers");
                            dt.Rows.Add("BLNDUALENTRYMODE", "Dual Entry Mode For Ledgers");
                            dt.Rows.Add("BLNDISPLAYLEDGERBALANCE", "Display Ledger Balance On Selection");
                            dt.Rows.Add("BLNPOSTONEFFECTIVEDATE", "Post Accounts On Effective Date");
                            dt.Rows.Add("BLNENABLEEFFECIVEDATE", "Show Effective Date");
                            dt.Rows.Add("BLNCHEQUEDETAILS", "Show Cheque Details");
                            dt.Rows.Add("BLNSHOWLEDGERBALANCES", "Show Ledger Balances");
                            dt.Rows.Add("BLNPARTYDETAILS", "Show Party Details");
                            dt.Rows.Add("BLNSHOWREFERENCENO", "Show Reference Number");
                            dt.Rows.Add("BLNSHOWBILLNARRATION", "Show Bill Narration");
                            dt.Rows.Add("BLNSHOWITEMCALCGRID", "Show Item Calculation Grid");
                            dt.Rows.Add("BLNSHOWQTY", "Show Qty Column");
                            dt.Rows.Add("BLNSHOWLEDGERBALANCES", "Show Ledger Balances");
                            dt.Rows.Add("BLNSHOWLEDGERBALANCES", "Show Ledger Balances");
                            dt.Rows.Add("BLNSHOWPREVIEW", "Show Preview Before Print");
                            dt.Rows.Add("blnprintconfirmation", "Ask Confirmation on Print");
                            dt.Rows.Add("BLNPRINTIMMEDIATELY", "Send Bill to Printer On Save");


                            break;
                        }
                    case "10": //Contra
                        {
                            dt.Rows.Add("CHEQUEPRINTING", "Enable Cheque Printing");
                            dt.Rows.Add("BLNALLOWDUPLICATELEDGERS", "Allow Duplicate Ledgers");
                            dt.Rows.Add("BLNDISABLEANYWHERESEARCHINGOFLEDGERS", "Disable Anywhere Searching Of Ledgers");
                            //dt.Rows.Add("BLNDUALENTRYMODE", "Dual Entry Mode For Ledgers");
                            dt.Rows.Add("BLNDISPLAYLEDGERBALANCE", "Display Ledger Balance On Selection");
                            dt.Rows.Add("BLNPOSTONEFFECTIVEDATE", "Post Accounts On Effective Date");
                            dt.Rows.Add("BLNENABLEEFFECIVEDATE", "Show Effective Date");
                            dt.Rows.Add("BLNCHEQUEDETAILS", "Show Cheque Details");
                            dt.Rows.Add("BLNSHOWLEDGERBALANCES", "Show Ledger Balances");
                            dt.Rows.Add("BLNPARTYDETAILS", "Show Party Details");
                            dt.Rows.Add("BLNSHOWREFERENCENO", "Show Reference Number");
                            dt.Rows.Add("BLNSHOWBILLNARRATION", "Show Bill Narration");
                            dt.Rows.Add("BLNSHOWITEMCALCGRID", "Show Item Calculation Grid");
                            dt.Rows.Add("BLNSHOWQTY", "Show Qty Column");
                            dt.Rows.Add("BLNSHOWLEDGERBALANCES", "Show Ledger Balances");
                            dt.Rows.Add("BLNSHOWLEDGERBALANCES", "Show Ledger Balances");
                            dt.Rows.Add("BLNSHOWPREVIEW", "Show Preview Before Print");
                            dt.Rows.Add("blnprintconfirmation", "Ask Confirmation on Print");
                            dt.Rows.Add("BLNPRINTIMMEDIATELY", "Send Bill to Printer On Save");


                            break;
                        }
                    case "14": //Sales Order
                        {
                            //dt.Rows.Add("BLNAPPLYGIFTVOUCHER", "Apply Gift Voucher");
                            //dt.Rows.Add("BLNAPPLYOFFER", "Apply Offer Automatically");
                            dt.Rows.Add("BLNCASHDESK", "Enable Cash Desk");
                            dt.Rows.Add("BLNPOSTCASHENTRY", "Post Cash Entry In Ledger");
                            dt.Rows.Add("BLNDISPLAYLEDGERBALANCE", "Display Ledger Balance On Selection");
                            dt.Rows.Add("BLNNAMEANDMOBILENOMANDATORY", "Party Name & Mobile No Mandatory");
                            dt.Rows.Add("BLNEDITMRPRATE", "Allow User To Edit MRP In Grid");
                            dt.Rows.Add("BLNEDITSALERATE", "Allow User To Edit SRate");
                            dt.Rows.Add("BLNSHOWFREEQUANTITY", "Allow User To Enter Free Qty");
                            dt.Rows.Add("BLNEDITTAXPER", "Allow User To Edit Tax Percentage");
                            dt.Rows.Add("BLNAUTOCHANGERATEONPRICELIST", "Auto Change SRate On Pricelist Selection");
                            dt.Rows.Add("BLNSHOWPRICELIST", "Show Pricelist");
                            dt.Rows.Add("BLNSHOWOFFERINPOPUP", "Show Offer In Popup");
                            dt.Rows.Add("BLNPARTYDETAILS", "Show Party Details");
                            dt.Rows.Add("BLNENABLECASHDISCOUNT", "Show Cash Discount");
                            dt.Rows.Add("BLNENABLEEFFECIVEDATE", "Show Effective Date");
                            dt.Rows.Add("blnShowReferenceNo", "Show Reference No");
                            dt.Rows.Add("BLNSHOWBILLNARRATION", "Show Bill Narration");
                            dt.Rows.Add("BLNSHOWOTHEREXPENSE", "Show Other Expense");
                            dt.Rows.Add("BLNSHOWITEMCALCGRID", "Show Item Calculation Grid");
                            dt.Rows.Add("BLNSHOWITEMPROFITPER", "Show Item Previous Rates");
                            dt.Rows.Add("BLNSHOWPREVIEW", "Show Preview Before Print");
                            dt.Rows.Add("BLNPRINTCONFIRMATION", "Ask Confirmation On Print");
                            dt.Rows.Add("BLNPRINTIMMEDIATELY", "Send Bill to Printer on Save");
                            dt.Rows.Add("BLNSUMMARISEDUPLICATEITEMSINPRINT", "Summarise Duplicate Items In Print");
                            dt.Rows.Add("BLNSUMMARISEITEMSWHILEENTERING", "Summarise Duplicate Items While Entering");
                            //dt.Rows.Add("BLNSUMMARISEITEMSWHILEENTERING", "Summarise Items While Entering");

                            break;
                        }
                    case "15": //Purchase Order
                        {
                            dt.Rows.Add("BLNPOSTCASHENTRY", "Post Cash Entry In Ledger");
                            dt.Rows.Add("BLNRECALCULATESalesRatesOnPercentage", "Recalculate Sales Rates On Percentage");
                            dt.Rows.Add("blnShowRateFixer", "Enable Rate Fixer");
                            dt.Rows.Add("BLNCASHDESK", "Enable Cash Desk");
                            dt.Rows.Add("blnWarnifSRatelessthanPrate", "Warn If Sales Rate Is Less Than Purchase Rate");
                            dt.Rows.Add("BLNEDITMRPRATE", "Allow User To Edit MRP");
                            dt.Rows.Add("blneditsalerate", "Allow User To Edit Rate");
                            dt.Rows.Add("BLNSHOWFREEQUANTITY", "Allow User To Enter Free Qty");
                            dt.Rows.Add("blnEditTaxPer", "Allow User To Edit Tax Percentage");
                            dt.Rows.Add("blnpartydetails", "Show Party Details");
                            dt.Rows.Add("blnenablecashdiscount", "Show Cash Discount");
                            dt.Rows.Add("blnenableEffeciveDate", "Show Effective Date");
                            dt.Rows.Add("blnShowReferenceNo", "Show Reference No");
                            dt.Rows.Add("blnshowotherexpense", "Show Other Expenses");
                            dt.Rows.Add("blnshowbillnarration", "Show Bill Narration");
                            dt.Rows.Add("blnShowItemCalcGrid", "Show Item Calculation Grid");
                            dt.Rows.Add("blnShowItemProfitPer", "Show Product Profit Percentage");
                            dt.Rows.Add("blnshowpreview", "Show Preview Before Print");
                            dt.Rows.Add("blnprintconfirmation", "Ask Confirmation On Print");
                            dt.Rows.Add("blnprintimmediately", "Send Bill To Printer On Save");
                            dt.Rows.Add("blnSummariseDuplicateItemsInPrint", "Summarise Duplicate Items In Print");
                            dt.Rows.Add("blnSummariseItemsWhileEntering", "Summarise Duplicate Items While Entering");

                            break;
                        }
                    case "16": //Stock Transfer
                        {
                            dt.Rows.Add("BLNPOSTCASHENTRY", "Post Cash Entry In Ledger");
                            dt.Rows.Add("BLNRECALCULATESalesRatesOnPercentage", "Recalculate Sales Rates On Percentage");
                            dt.Rows.Add("blnShowRateFixer", "Enable Rate Fixer");
                            dt.Rows.Add("BLNCASHDESK", "Enable Cash Desk");
                            dt.Rows.Add("blnWarnifSRatelessthanPrate", "Warn If Sales Rate Is Less Than Purchase Rate");
                            dt.Rows.Add("BLNEDITMRPRATE", "Allow User To Edit MRP");
                            dt.Rows.Add("blneditsalerate", "Allow User To Edit Rate");
                            dt.Rows.Add("BLNSHOWFREEQUANTITY", "Allow User To Enter Free Qty");
                            dt.Rows.Add("blnEditTaxPer", "Allow User To Edit Tax Percentage");
                            dt.Rows.Add("blnpartydetails", "Show Party Details");
                            dt.Rows.Add("blnenablecashdiscount", "Show Cash Discount");
                            dt.Rows.Add("blnenableEffeciveDate", "Show Effective Date");
                            dt.Rows.Add("blnShowReferenceNo", "Show Reference No");
                            dt.Rows.Add("blnshowotherexpense", "Show Other Expenses");
                            dt.Rows.Add("blnshowbillnarration", "Show Bill Narration");
                            dt.Rows.Add("blnShowItemCalcGrid", "Show Item Calculation Grid");
                            dt.Rows.Add("blnShowItemProfitPer", "Show Product Profit Percentage");
                            dt.Rows.Add("blnshowpreview", "Show Preview Before Print");
                            dt.Rows.Add("blnprintconfirmation", "Ask Confirmation On Print");
                            dt.Rows.Add("blnprintimmediately", "Send Bill To Printer On Save");
                            dt.Rows.Add("blnSummariseDuplicateItemsInPrint", "Summarise Duplicate Items In Print");
                            dt.Rows.Add("blnSummariseItemsWhileEntering", "Summarise Duplicate Items While Entering");

                            break;
                        }
                    case "18": //Quotation
                        {
                            //dt.Rows.Add("BLNAPPLYGIFTVOUCHER", "Apply Gift Voucher");
                            //dt.Rows.Add("BLNAPPLYOFFER", "Apply Offer Automatically");
                            dt.Rows.Add("BLNDISPLAYLEDGERBALANCE", "Display Ledger Balance On Selection");
                            dt.Rows.Add("BLNNAMEANDMOBILENOMANDATORY", "Party Name & Mobile No Mandatory");
                            dt.Rows.Add("BLNEDITMRPRATE", "Allow User To Edit MRP In Grid");
                            dt.Rows.Add("BLNEDITSALERATE", "Allow User To Edit SRate");
                            dt.Rows.Add("BLNSHOWFREEQUANTITY", "Allow User To Enter Free Qty");
                            dt.Rows.Add("BLNEDITTAXPER", "Allow User To Edit Tax Percentage");
                            dt.Rows.Add("BLNAUTOCHANGERATEONPRICELIST", "Auto Change SRate On Pricelist Selection");
                            dt.Rows.Add("BLNSHOWPRICELIST", "Show Pricelist");
                            dt.Rows.Add("BLNSHOWOFFERINPOPUP", "Show Offer In Popup");
                            dt.Rows.Add("BLNPARTYDETAILS", "Show Party Details");
                            dt.Rows.Add("BLNENABLECASHDISCOUNT", "Show Cash Discount");
                            dt.Rows.Add("BLNENABLEEFFECIVEDATE", "Show Effective Date");
                            dt.Rows.Add("blnShowReferenceNo", "Show Reference No");
                            dt.Rows.Add("BLNSHOWBILLNARRATION", "Show Bill Narration");
                            dt.Rows.Add("BLNSHOWOTHEREXPENSE", "Show Other Expense");
                            dt.Rows.Add("BLNSHOWITEMCALCGRID", "Show Item Calculation Grid");
                            dt.Rows.Add("BLNSHOWITEMPROFITPER", "Show Item Previous Rates");
                            dt.Rows.Add("BLNSHOWPREVIEW", "Show Preview Before Print");
                            dt.Rows.Add("BLNPRINTCONFIRMATION", "Ask Confirmation On Print");
                            dt.Rows.Add("BLNPRINTIMMEDIATELY", "Send Bill to Printer on Save");
                            dt.Rows.Add("BLNSUMMARISEDUPLICATEITEMSINPRINT", "Summarise Duplicate Items In Print");
                            dt.Rows.Add("BLNSUMMARISEITEMSWHILEENTERING", "Summarise Duplicate Items While Entering");
                            //dt.Rows.Add("BLNSUMMARISEITEMSWHILEENTERING", "Summarise Items While Entering");

                            break;
                        }
                    case "20": //
                        {
                            dt.Rows.Add("blnautofillrawmaterial", "Auto Fill Raw Material");
                            dt.Rows.Add("blnshowscrap", "Show Scrap");
                            dt.Rows.Add("blnpartydetails", "Show Party Details");
                            dt.Rows.Add("blnenablecashdiscount", "Show Cash Discount");
                            dt.Rows.Add("blnenableEffeciveDate", "Show Effective Date");
                            dt.Rows.Add("blnshowbillnarration", "Show Bill Narration");
                            dt.Rows.Add("blnshowotherexpense", "Show Other Expense");
                            dt.Rows.Add("blnShowItemCalcGrid", "Show Item Calculation Grid");
                            dt.Rows.Add("blnshowpreview", "Show Preview Before Print");
                            dt.Rows.Add("blnprintconfirmation", "Ask Confirmation On Print");
                            dt.Rows.Add("blnprintimmediately", "Send Bill To Printer On Save");
                            dt.Rows.Add("blnSummariseDuplicateItemsInPrint", "Summarise Duplicate Items In Print");
                            dt.Rows.Add("blnSummariseItemsWhileEntering", "Summarise Duplicate Items While Entering");
                            dt.Rows.Add("BLNEDITMRPRATE", "Allow User To Edit MRP In Grid");
                            dt.Rows.Add("blneditsalerate", "Allow User To Edit Rate");
                            dt.Rows.Add("blnEditTaxPer", "Allow User To Edit Tax Percentage");
                            break;
                        }
                    case "40": //Board Rate Updator
                        {
                            dt.Rows.Add("blnWarnifSRatelessthanPrate", "Warn If Sales Rate Is Less Than Purchase Rate");
                            dt.Rows.Add("BLNEDITMRPRATE", "Allow User To Edit MRP");
                            dt.Rows.Add("blneditsalerate", "Allow User To Edit Rate");
                            dt.Rows.Add("blnShowReferenceNo", "Show Reference No");

                            break;
                        }
                    case "41": //Physical Stock
                        {
                            dt.Rows.Add("blnWarnifSRatelessthanPrate", "Warn If Sales Rate Is Less Than Purchase Rate");
                            dt.Rows.Add("BLNEDITMRPRATE", "Allow User To Edit MRP");
                            dt.Rows.Add("blneditsalerate", "Allow User To Edit Rate");
                            dt.Rows.Add("blnShowReferenceNo", "Show Reference No");

                            break;
                        }
                    case "88": //Barcode Changer
                        {
                            break;
                        }
                    case "1005": //Opening
                        {
                            dt.Rows.Add("blnWarnifSRatelessthanPrate", "Warn If Sales Rate Is Less Than Purchase Rate");
                            dt.Rows.Add("BLNEDITMRPRATE", "Allow User To Edit MRP");
                            dt.Rows.Add("blneditsalerate", "Allow User To Edit Rate");
                            dt.Rows.Add("blnShowReferenceNo", "Show Reference No");

                            break;
                        }

                    //case "1": //Sales
                    //    {
                    //        //dt.Rows.Add("BLNAPPLYGIFTVOUCHER", "Apply Gift Voucher");
                    //        //dt.Rows.Add("BLNAPPLYOFFER", "Apply Offer Automatically");
                    //        dt.Rows.Add("BLNPOSTCASHENTRY", "Post Cash Entry In Supplier Ledger");
                    //        dt.Rows.Add("BLNDISPLAYLEDGERBALANCE", "Display ledger balance on Selection");
                    //        dt.Rows.Add("BLNNAMEANDMOBILENOMANDATORY", "Party Name & Mobile No Mandatory");
                    //        dt.Rows.Add("BLNAUTOCHANGERATEONPRICELIST", "Auto Change SRate on Pricelist Selection");
                    //        dt.Rows.Add("BLNSHOWPRICELIST", "Show Pricelist");
                    //        dt.Rows.Add("BLNSHOWOFFERINPOPUP", "Show Offer in Popup");
                    //        dt.Rows.Add("BLNPARTYDETAILS", "Show Party Details");
                    //        dt.Rows.Add("BLNENABLECASHDISCOUNT", "Show Cash Discount");
                    //        dt.Rows.Add("BLNENABLEEFFECIVEDATE", "Show Effective Date");
                    //        dt.Rows.Add("blnShowReferenceNo", "Show Reference No");
                    //        dt.Rows.Add("BLNSHOWBILLNARRATION", "Show Bill Narration");
                    //        dt.Rows.Add("BLNSHOWOTHEREXPENSE", "Show Other Expense");
                    //        dt.Rows.Add("BLNSHOWITEMCALCGRID", "Show Item Calculation Grid");
                    //        dt.Rows.Add("BLNSHOWITEMPROFITPER", "Show Item Rates Previous");
                    //        dt.Rows.Add("BLNSHOWPREVIEW", "Show Preview Before Print");
                    //        dt.Rows.Add("BLNPRINTCONFIRMATION", "Ask Confirmation on Print");
                    //        dt.Rows.Add("BLNPRINTIMMEDIATELY", "Send Bill to Printer on Save");
                    //        dt.Rows.Add("BLNSUMMARISEDUPLICATEITEMSINPRINT", "Summarise Duplicate Items In Print");
                    //        dt.Rows.Add("BLNSUMMARISEITEMSWHILEENTERING", "Summarise Duplicate Items While Entering");
                    //        //dt.Rows.Add("BLNSUMMARISEITEMSWHILEENTERING", "Summarise Items While Entering");
                    //        dt.Rows.Add("BLNEDITMRPRATE", "Allow user to Edit MRP in grid");
                    //        dt.Rows.Add("BLNEDITSALERATE", "Allow user to Edit SRate");
                    //        dt.Rows.Add("BLNSHOWFREEQUANTITY", "Allow user to Enter Free Qty");
                    //        dt.Rows.Add("BLNEDITTAXPER", "Allow user to Edit Tax Percentage");
                    //        dt.Rows.Add("BLNCASHDESK", "Enable Cash Desk");

                    //        break;
                    //    }

                    //case "2": //Purchase
                    //    {
                    //        dt.Rows.Add("BLNPOSTCASHENTRY", "Post Cash Entry In Supplier Ledger");
                    //        dt.Rows.Add("BLNRECALCULATESalesRatesOnPercentage", "Recalculate Sales Rates On Percentage");
                    //        dt.Rows.Add("blnpartydetails", "Show Party Details");
                    //        dt.Rows.Add("blnenablecashdiscount", "Show Cash Discount");
                    //        dt.Rows.Add("blnenableEffeciveDate", "Show Effective Date");
                    //        dt.Rows.Add("blnShowReferenceNo", "Show Reference No");
                    //        dt.Rows.Add("blnshowotherexpense", "Show Other Expenses");
                    //        dt.Rows.Add("blnshowbillnarration", "Show Bill Narration");
                    //        dt.Rows.Add("blnShowItemCalcGrid", "Show Item Calculation Grid");
                    //        dt.Rows.Add("blnShowItemProfitPer", "Show Product Profit Percentage");
                    //        dt.Rows.Add("blnshowpreview", "Show Preview before Print");
                    //        dt.Rows.Add("blnprintconfirmation", "Ask Confirmation on Print");
                    //        dt.Rows.Add("blnprintimmediately", "Send Bill to Printer on Save");
                    //        dt.Rows.Add("blnSummariseDuplicateItemsInPrint", "Summarise Duplicate Items In Print");
                    //        dt.Rows.Add("blnSummariseItemsWhileEntering", "Summarise Duplicate Items While Entering");
                    //        dt.Rows.Add("blnWarnifSRatelessthanPrate", "Warn If Sales Rate is less than Purchase Rate");
                    //        dt.Rows.Add("BLNEDITMRPRATE", "Allow user to Edit MRP");
                    //        dt.Rows.Add("blneditsalerate", "Allow user to Edit Rate");
                    //        dt.Rows.Add("BLNSHOWFREEQUANTITY", "Allow user to Enter Free Qty");
                    //        dt.Rows.Add("blnEditTaxPer", "Allow user to Edit Tax Percentage");
                    //        dt.Rows.Add("blnShowRateFixer", "Enable Rate Fixer");
                    //        dt.Rows.Add("BLNCASHDESK", "Enable Cash Desk");
                    //        break;
                    //    }

                    //case "3": //Sales Return
                    //    {
                    //        //dt.Rows.Add("BLNAPPLYGIFTVOUCHER", "Apply Gift Voucher");
                    //        //dt.Rows.Add("BLNAPPLYOFFER", "Apply Offer Automatically");
                    //        dt.Rows.Add("BLNPOSTCASHENTRY", "Post Cash Entry In Supplier Ledger");
                    //        dt.Rows.Add("BLNDISPLAYLEDGERBALANCE", "Display ledger balance on Selection");
                    //        dt.Rows.Add("BLNNAMEANDMOBILENOMANDATORY", "Party Name & Mobile No Mandatory");
                    //        dt.Rows.Add("BLNAUTOCHANGERATEONPRICELIST", "Auto Change SRate on Pricelist Selection");
                    //        dt.Rows.Add("BLNSHOWPRICELIST", "Show Pricelist");
                    //        dt.Rows.Add("BLNSHOWOFFERINPOPUP", "Show Offer in Popup");
                    //        dt.Rows.Add("BLNPARTYDETAILS", "Show Party Details");
                    //        dt.Rows.Add("BLNENABLECASHDISCOUNT", "Show Cash Discount");
                    //        dt.Rows.Add("BLNENABLEEFFECIVEDATE", "Show Effective Date");
                    //        dt.Rows.Add("blnShowReferenceNo", "Show Reference No");
                    //        dt.Rows.Add("BLNSHOWBILLNARRATION", "Show Bill Narration");
                    //        dt.Rows.Add("BLNSHOWOTHEREXPENSE", "Show Other Expense");
                    //        dt.Rows.Add("BLNSHOWITEMCALCGRID", "Show Item Calculation Grid");
                    //        dt.Rows.Add("BLNSHOWITEMPROFITPER", "Show Item Rates Previous");
                    //        dt.Rows.Add("BLNSHOWPREVIEW", "Show Preview Before Print");
                    //        dt.Rows.Add("BLNPRINTCONFIRMATION", "Ask Confirmation on Print");
                    //        dt.Rows.Add("BLNPRINTIMMEDIATELY", "Send Bill to Printer on Save");
                    //        dt.Rows.Add("BLNSUMMARISEDUPLICATEITEMSINPRINT", "Summarise Duplicate Items In Print");
                    //        dt.Rows.Add("BLNSUMMARISEITEMSWHILEENTERING", "Summarise Duplicate Items While Entering");
                    //        //dt.Rows.Add("BLNSUMMARISEITEMSWHILEENTERING", "Summarise Items While Entering");
                    //        dt.Rows.Add("BLNEDITMRPRATE", "Allow user to Edit MRP in grid");
                    //        dt.Rows.Add("BLNEDITSALERATE", "Allow user to Edit SRate");
                    //        dt.Rows.Add("BLNSHOWFREEQUANTITY", "Allow user to Enter Free Qty");
                    //        dt.Rows.Add("BLNEDITTAXPER", "Allow user to Edit Tax Percentage");
                    //        dt.Rows.Add("BLNCASHDESK", "Enable Cash Desk");

                    //        break;
                    //    }

                    //case "4": //Purchase return
                    //    {
                    //        dt.Rows.Add("BLNPOSTCASHENTRY", "Post Cash Entry In Supplier Ledger");
                    //        dt.Rows.Add("BLNRECALCULATESalesRatesOnPercentage", "Recalculate Sales Rates On Percentage");
                    //        dt.Rows.Add("blnpartydetails", "Show Party Details");
                    //        dt.Rows.Add("blnenablecashdiscount", "Show Cash Discount");
                    //        dt.Rows.Add("blnenableEffeciveDate", "Show Effective Date");
                    //        dt.Rows.Add("blnShowReferenceNo", "Show Reference No");
                    //        dt.Rows.Add("blnshowotherexpense", "Show Other Expenses");
                    //        dt.Rows.Add("blnshowbillnarration", "Show Bill Narration");
                    //        dt.Rows.Add("blnShowItemCalcGrid", "Show Item Calculation Grid");
                    //        dt.Rows.Add("blnShowItemProfitPer", "Show Product Profit Percentage");
                    //        dt.Rows.Add("blnshowpreview", "Show Preview before Print");
                    //        dt.Rows.Add("blnprintconfirmation", "Ask Confirmation on Print");
                    //        dt.Rows.Add("blnprintimmediately", "Send Bill to Printer on Save");
                    //        dt.Rows.Add("blnSummariseDuplicateItemsInPrint", "Summarise Duplicate Items In Print");
                    //        dt.Rows.Add("blnSummariseItemsWhileEntering", "Summarise Duplicate Items While Entering");
                    //        dt.Rows.Add("blnWarnifSRatelessthanPrate", "Warn If Sales Rate is less than Purchase Rate");
                    //        dt.Rows.Add("BLNEDITMRPRATE", "Allow user to Edit MRP");
                    //        dt.Rows.Add("blneditsalerate", "Allow user to Edit Rate");
                    //        dt.Rows.Add("BLNSHOWFREEQUANTITY", "Allow user to Enter Free Qty");
                    //        dt.Rows.Add("blnEditTaxPer", "Allow user to Edit Tax Percentage");
                    //        dt.Rows.Add("blnShowRateFixer", "Enable Rate Fixer");
                    //        dt.Rows.Add("BLNCASHDESK", "Enable Cash Desk");
                    //        break;
                    //    }
                    //case "5": //Delivery Note
                    //    {
                    //        //dt.Rows.Add("BLNAPPLYGIFTVOUCHER", "Apply Gift Voucher");
                    //        //dt.Rows.Add("BLNAPPLYOFFER", "Apply Offer Automatically");
                    //        dt.Rows.Add("BLNPOSTCASHENTRY", "Post Cash Entry In Supplier Ledger");
                    //        dt.Rows.Add("BLNDISPLAYLEDGERBALANCE", "Display ledger balance on Selection");
                    //        dt.Rows.Add("BLNNAMEANDMOBILENOMANDATORY", "Party Name & Mobile No Mandatory");
                    //        dt.Rows.Add("BLNAUTOCHANGERATEONPRICELIST", "Auto Change SRate on Pricelist Selection");
                    //        dt.Rows.Add("BLNSHOWPRICELIST", "Show Pricelist");
                    //        dt.Rows.Add("BLNSHOWOFFERINPOPUP", "Show Offer in Popup");
                    //        dt.Rows.Add("BLNPARTYDETAILS", "Show Party Details");
                    //        dt.Rows.Add("BLNENABLECASHDISCOUNT", "Show Cash Discount");
                    //        dt.Rows.Add("BLNENABLEEFFECIVEDATE", "Show Effective Date");
                    //        dt.Rows.Add("blnShowReferenceNo", "Show Reference No");
                    //        dt.Rows.Add("BLNSHOWBILLNARRATION", "Show Bill Narration");
                    //        dt.Rows.Add("BLNSHOWOTHEREXPENSE", "Show Other Expense");
                    //        dt.Rows.Add("BLNSHOWITEMCALCGRID", "Show Item Calculation Grid");
                    //        dt.Rows.Add("BLNSHOWITEMPROFITPER", "Show Item Rates Previous");
                    //        dt.Rows.Add("BLNSHOWPREVIEW", "Show Preview Before Print");
                    //        dt.Rows.Add("BLNPRINTCONFIRMATION", "Ask Confirmation on Print");
                    //        dt.Rows.Add("BLNPRINTIMMEDIATELY", "Send Bill to Printer on Save");
                    //        dt.Rows.Add("BLNSUMMARISEDUPLICATEITEMSINPRINT", "Summarise Duplicate Items In Print");
                    //        dt.Rows.Add("BLNSUMMARISEITEMSWHILEENTERING", "Summarise Duplicate Items While Entering");
                    //        //dt.Rows.Add("BLNSUMMARISEITEMSWHILEENTERING", "Summarise Items While Entering");
                    //        dt.Rows.Add("BLNEDITMRPRATE", "Allow user to Edit MRP in grid");
                    //        dt.Rows.Add("BLNEDITSALERATE", "Allow user to Edit SRate");
                    //        dt.Rows.Add("BLNSHOWFREEQUANTITY", "Allow user to Enter Free Qty");
                    //        dt.Rows.Add("BLNEDITTAXPER", "Allow user to Edit Tax Percentage");
                    //        dt.Rows.Add("BLNCASHDESK", "Enable Cash Desk");


                    //        break;
                    //    }
                    //case "6": //Receipt Note
                    //    {
                    //        dt.Rows.Add("BLNPOSTCASHENTRY", "Post Cash Entry In Supplier Ledger");
                    //        dt.Rows.Add("BLNRECALCULATESalesRatesOnPercentage", "Recalculate Sales Rates On Percentage");
                    //        dt.Rows.Add("blnpartydetails", "Show Party Details");
                    //        dt.Rows.Add("blnenablecashdiscount", "Show Cash Discount");
                    //        dt.Rows.Add("blnenableEffeciveDate", "Show Effective Date");
                    //        dt.Rows.Add("blnShowReferenceNo", "Show Reference No");
                    //        dt.Rows.Add("blnshowotherexpense", "Show Other Expenses");
                    //        dt.Rows.Add("blnshowbillnarration", "Show Bill Narration");
                    //        dt.Rows.Add("blnShowItemCalcGrid", "Show Item Calculation Grid");
                    //        dt.Rows.Add("blnShowItemProfitPer", "Show Product Profit Percentage");
                    //        dt.Rows.Add("blnshowpreview", "Show Preview before Print");
                    //        dt.Rows.Add("blnprintconfirmation", "Ask Confirmation on Print");
                    //        dt.Rows.Add("blnprintimmediately", "Send Bill to Printer on Save");
                    //        dt.Rows.Add("blnSummariseDuplicateItemsInPrint", "Summarise Duplicate Items In Print");
                    //        dt.Rows.Add("blnSummariseItemsWhileEntering", "Summarise Duplicate Items While Entering");
                    //        dt.Rows.Add("blnWarnifSRatelessthanPrate", "Warn If Sales Rate is less than Purchase Rate");
                    //        dt.Rows.Add("BLNEDITMRPRATE", "Allow user to Edit MRP");
                    //        dt.Rows.Add("blneditsalerate", "Allow user to Edit Rate");
                    //        dt.Rows.Add("BLNSHOWFREEQUANTITY", "Allow user to Enter Free Qty");
                    //        dt.Rows.Add("blnEditTaxPer", "Allow user to Edit Tax Percentage");
                    //        dt.Rows.Add("blnShowRateFixer", "Enable Rate Fixer");
                    //        dt.Rows.Add("BLNCASHDESK", "Enable Cash Desk");
                    //        break;
                    //    }
                    //case "7": //Receipt
                    //    {

                    //        dt.Rows.Add("BLNDISABLEANYWHERESEARCHINGOFLEDGERS", "Disable anywhere searching of ledgers");
                    //        dt.Rows.Add("BLNDUALENTRYMODE", "Dual Entry Mode For Ledgers");
                    //        dt.Rows.Add("BLNDISPLAYLEDGERBALANCE", "Display ledger balance on Selection");
                    //        dt.Rows.Add("BLNPOSTONEFFECTIVEDATE", "Post Accounts on Effective Date");
                    //        dt.Rows.Add("BLNENABLEEFFECIVEDATE", "Show Effective Date");
                    //        dt.Rows.Add("BLNCHEQUEDETAILS", "Show Cheque Details");
                    //        dt.Rows.Add("BLNSHOWLEDGERBALANCES", "Show ledger balances");
                    //        dt.Rows.Add("BLNPARTYDETAILS", "Show Party Details");
                    //        dt.Rows.Add("BLNSHOWREFERENCENO", "Show Reference Number");
                    //        dt.Rows.Add("BLNSHOWBILLNARRATION", "Show Bill Narration");
                    //        dt.Rows.Add("BLNSHOWITEMCALCGRID", "Show Item Calculation Grid");
                    //        dt.Rows.Add("BLNSHOWQTY", "Show Qty Column");
                    //        dt.Rows.Add("BLNPRINTIMMEDIATELY", "Send Bill to Printer on Save");
                    //        dt.Rows.Add("BLNSHOWLEDGERBALANCES", "Show ledger balances");
                    //        dt.Rows.Add("BLNSHOWPREVIEW", "Show Preview Before Print");
                    //        dt.Rows.Add("BLNALLOWDUPLICATELEDGERS", "Allow Duplicate ledgers");
                    //        dt.Rows.Add("CHEQUEPRINTING", "Enable Cheque Printing");
                    //        break;
                    //    }
                    //case "8": //Payment
                    //    {
                    //        dt.Rows.Add("BLNDISABLEANYWHERESEARCHINGOFLEDGERS", "Disable anywhere searching of ledgers");
                    //        dt.Rows.Add("BLNDUALENTRYMODE", "Dual Entry Mode For Ledgers");
                    //        dt.Rows.Add("BLNDISPLAYLEDGERBALANCE", "Display ledger balance on Selection");
                    //        dt.Rows.Add("BLNPOSTONEFFECTIVEDATE", "Post Accounts on Effective Date");
                    //        dt.Rows.Add("BLNENABLEEFFECIVEDATE", "Show Effective Date");
                    //        dt.Rows.Add("BLNCHEQUEDETAILS", "Show Cheque Details");
                    //        dt.Rows.Add("BLNSHOWLEDGERBALANCES", "Show ledger balances");
                    //        dt.Rows.Add("BLNPARTYDETAILS", "Show Party Details");
                    //        dt.Rows.Add("BLNSHOWREFERENCENO", "Show Reference Number");
                    //        dt.Rows.Add("BLNSHOWBILLNARRATION", "Show Bill Narration");
                    //        dt.Rows.Add("BLNSHOWITEMCALCGRID", "Show Item Calculation Grid");
                    //        dt.Rows.Add("BLNSHOWQTY", "Show Qty Column");
                    //        dt.Rows.Add("BLNPRINTIMMEDIATELY", "Send Bill to Printer on Save");
                    //        dt.Rows.Add("BLNSHOWLEDGERBALANCES", "Show ledger balances");
                    //        dt.Rows.Add("BLNSHOWPREVIEW", "Show Preview Before Print");
                    //        dt.Rows.Add("BLNALLOWDUPLICATELEDGERS", "Allow Duplicate ledgers");
                    //        dt.Rows.Add("CHEQUEPRINTING", "Enable Cheque Printing");

                    //        break;
                    //    }
                    //case "9": //Journal
                    //    {
                    //        dt.Rows.Add("BLNDISABLEANYWHERESEARCHINGOFLEDGERS", "Disable anywhere searching of ledgers");
                    //        dt.Rows.Add("BLNDUALENTRYMODE", "Dual Entry Mode For Ledgers");
                    //        dt.Rows.Add("BLNDISPLAYLEDGERBALANCE", "Display ledger balance on Selection");
                    //        dt.Rows.Add("BLNPOSTONEFFECTIVEDATE", "Post Accounts on Effective Date");
                    //        dt.Rows.Add("BLNENABLEEFFECIVEDATE", "Show Effective Date");
                    //        dt.Rows.Add("BLNCHEQUEDETAILS", "Show Cheque Details");
                    //        dt.Rows.Add("BLNSHOWLEDGERBALANCES", "Show ledger balances");
                    //        dt.Rows.Add("BLNPARTYDETAILS", "Show Party Details");
                    //        dt.Rows.Add("BLNSHOWREFERENCENO", "Show Reference Number");
                    //        dt.Rows.Add("BLNSHOWBILLNARRATION", "Show Bill Narration");
                    //        dt.Rows.Add("BLNSHOWITEMCALCGRID", "Show Item Calculation Grid");
                    //        dt.Rows.Add("BLNSHOWQTY", "Show Qty Column");
                    //        dt.Rows.Add("BLNPRINTIMMEDIATELY", "Send Bill to Printer on Save");
                    //        dt.Rows.Add("BLNSHOWLEDGERBALANCES", "Show ledger balances");
                    //        dt.Rows.Add("BLNSHOWPREVIEW", "Show Preview Before Print");
                    //        dt.Rows.Add("BLNALLOWDUPLICATELEDGERS", "Allow Duplicate ledgers");
                    //        dt.Rows.Add("CHEQUEPRINTING", "Enable Cheque Printing");

                    //        break;
                    //    }
                    //case "10": //Contra
                    //    {
                    //        dt.Rows.Add("BLNDISABLEANYWHERESEARCHINGOFLEDGERS", "Disable anywhere searching of ledgers");
                    //        dt.Rows.Add("BLNDUALENTRYMODE", "Dual Entry Mode For Ledgers");
                    //        dt.Rows.Add("BLNDISPLAYLEDGERBALANCE", "Display ledger balance on Selection");
                    //        dt.Rows.Add("BLNPOSTONEFFECTIVEDATE", "Post Accounts on Effective Date");
                    //        dt.Rows.Add("BLNENABLEEFFECIVEDATE", "Show Effective Date");
                    //        dt.Rows.Add("BLNCHEQUEDETAILS", "Show Cheque Details");
                    //        dt.Rows.Add("BLNSHOWLEDGERBALANCES", "Show ledger balances");
                    //        dt.Rows.Add("BLNPARTYDETAILS", "Show Party Details");
                    //        dt.Rows.Add("BLNSHOWREFERENCENO", "Show Reference Number");
                    //        dt.Rows.Add("BLNSHOWBILLNARRATION", "Show Bill Narration");
                    //        dt.Rows.Add("BLNSHOWITEMCALCGRID", "Show Item Calculation Grid");
                    //        dt.Rows.Add("BLNSHOWQTY", "Show Qty Column");
                    //        dt.Rows.Add("BLNPRINTIMMEDIATELY", "Send Bill to Printer on Save");
                    //        dt.Rows.Add("BLNSHOWLEDGERBALANCES", "Show ledger balances");
                    //        dt.Rows.Add("BLNSHOWPREVIEW", "Show Preview Before Print");
                    //        dt.Rows.Add("BLNALLOWDUPLICATELEDGERS", "Allow Duplicate ledgers");
                    //        dt.Rows.Add("CHEQUEPRINTING", "Enable Cheque Printing");

                    //        break;
                    //    }
                    //case "14": //Sales Order
                    //    {
                    //        //dt.Rows.Add("BLNAPPLYGIFTVOUCHER", "Apply Gift Voucher");
                    //        //dt.Rows.Add("BLNAPPLYOFFER", "Apply Offer Automatically");
                    //        dt.Rows.Add("BLNPOSTCASHENTRY", "Post Cash Entry In Supplier Ledger");
                    //        dt.Rows.Add("BLNDISPLAYLEDGERBALANCE", "Display ledger balance on Selection");
                    //        dt.Rows.Add("BLNNAMEANDMOBILENOMANDATORY", "Party Name & Mobile No Mandatory");
                    //        dt.Rows.Add("BLNAUTOCHANGERATEONPRICELIST", "Auto Change SRate on Pricelist Selection");
                    //        dt.Rows.Add("BLNSHOWPRICELIST", "Show Pricelist");
                    //        dt.Rows.Add("BLNSHOWOFFERINPOPUP", "Show Offer in Popup");
                    //        dt.Rows.Add("BLNPARTYDETAILS", "Show Party Details");
                    //        dt.Rows.Add("BLNENABLECASHDISCOUNT", "Show Cash Discount");
                    //        dt.Rows.Add("BLNENABLEEFFECIVEDATE", "Show Effective Date");
                    //        dt.Rows.Add("blnShowReferenceNo", "Show Reference No");
                    //        dt.Rows.Add("BLNSHOWBILLNARRATION", "Show Bill Narration");
                    //        dt.Rows.Add("BLNSHOWOTHEREXPENSE", "Show Other Expense");
                    //        dt.Rows.Add("BLNSHOWITEMCALCGRID", "Show Item Calculation Grid");
                    //        dt.Rows.Add("BLNSHOWITEMPROFITPER", "Show Item Rates Previous");
                    //        dt.Rows.Add("BLNSHOWPREVIEW", "Show Preview Before Print");
                    //        dt.Rows.Add("BLNPRINTCONFIRMATION", "Ask Confirmation on Print");
                    //        dt.Rows.Add("BLNPRINTIMMEDIATELY", "Send Bill to Printer on Save");
                    //        dt.Rows.Add("BLNSUMMARISEDUPLICATEITEMSINPRINT", "Summarise Duplicate Items In Print");
                    //        dt.Rows.Add("BLNSUMMARISEITEMSWHILEENTERING", "Summarise Duplicate Items While Entering");
                    //        //dt.Rows.Add("BLNSUMMARISEITEMSWHILEENTERING", "Summarise Items While Entering");
                    //        dt.Rows.Add("BLNEDITMRPRATE", "Allow user to Edit MRP in grid");
                    //        dt.Rows.Add("BLNEDITSALERATE", "Allow user to Edit SRate");
                    //        dt.Rows.Add("BLNSHOWFREEQUANTITY", "Allow user to Enter Free Qty");
                    //        dt.Rows.Add("BLNEDITTAXPER", "Allow user to Edit Tax Percentage");
                    //        dt.Rows.Add("BLNCASHDESK", "Enable Cash Desk");

                    //        break;
                    //    }
                    //case "15": //Purchase Order
                    //    {
                    //        dt.Rows.Add("BLNPOSTCASHENTRY", "Post Cash Entry In Supplier Ledger");
                    //        dt.Rows.Add("BLNRECALCULATESalesRatesOnPercentage", "Recalculate Sales Rates On Percentage");
                    //        dt.Rows.Add("blnpartydetails", "Show Party Details");
                    //        dt.Rows.Add("blnenablecashdiscount", "Show Cash Discount");
                    //        dt.Rows.Add("blnenableEffeciveDate", "Show Effective Date");
                    //        dt.Rows.Add("blnShowReferenceNo", "Show Reference No");
                    //        dt.Rows.Add("blnshowotherexpense", "Show Other Expenses");
                    //        dt.Rows.Add("blnshowbillnarration", "Show Bill Narration");
                    //        dt.Rows.Add("blnShowItemCalcGrid", "Show Item Calculation Grid");
                    //        dt.Rows.Add("blnShowItemProfitPer", "Show Product Profit Percentage");
                    //        dt.Rows.Add("blnshowpreview", "Show Preview before Print");
                    //        dt.Rows.Add("blnprintconfirmation", "Ask Confirmation on Print");
                    //        dt.Rows.Add("blnprintimmediately", "Send Bill to Printer on Save");
                    //        dt.Rows.Add("blnSummariseDuplicateItemsInPrint", "Summarise Duplicate Items In Print");
                    //        dt.Rows.Add("blnSummariseItemsWhileEntering", "Summarise Duplicate Items While Entering");
                    //        dt.Rows.Add("blnWarnifSRatelessthanPrate", "Warn If Sales Rate is less than Purchase Rate");
                    //        dt.Rows.Add("BLNEDITMRPRATE", "Allow user to Edit MRP");
                    //        dt.Rows.Add("blneditsalerate", "Allow user to Edit Rate");
                    //        dt.Rows.Add("BLNSHOWFREEQUANTITY", "Allow user to Enter Free Qty");
                    //        dt.Rows.Add("blnEditTaxPer", "Allow user to Edit Tax Percentage");
                    //        dt.Rows.Add("blnShowRateFixer", "Enable Rate Fixer");

                    //        break;
                    //    }
                    //case "16": //Stock Transfer
                    //    {
                    //        dt.Rows.Add("BLNPOSTCASHENTRY", "Post Cash Entry In Supplier Ledger");
                    //        dt.Rows.Add("BLNRECALCULATESalesRatesOnPercentage", "Recalculate Sales Rates On Percentage");
                    //        dt.Rows.Add("blnpartydetails", "Show Party Details");
                    //        dt.Rows.Add("blnenablecashdiscount", "Show Cash Discount");
                    //        dt.Rows.Add("blnenableEffeciveDate", "Show Effective Date");
                    //        dt.Rows.Add("blnShowReferenceNo", "Show Reference No");
                    //        dt.Rows.Add("blnshowotherexpense", "Show Other Expenses");
                    //        dt.Rows.Add("blnshowbillnarration", "Show Bill Narration");
                    //        dt.Rows.Add("blnShowItemCalcGrid", "Show Item Calculation Grid");
                    //        dt.Rows.Add("blnShowItemProfitPer", "Show Product Profit Percentage");
                    //        dt.Rows.Add("blnshowpreview", "Show Preview before Print");
                    //        dt.Rows.Add("blnprintconfirmation", "Ask Confirmation on Print");
                    //        dt.Rows.Add("blnprintimmediately", "Send Bill to Printer on Save");
                    //        dt.Rows.Add("blnSummariseDuplicateItemsInPrint", "Summarise Duplicate Items In Print");
                    //        dt.Rows.Add("blnSummariseItemsWhileEntering", "Summarise Duplicate Items While Entering");
                    //        dt.Rows.Add("blnWarnifSRatelessthanPrate", "Warn If Sales Rate is less than Purchase Rate");
                    //        dt.Rows.Add("BLNEDITMRPRATE", "Allow user to Edit MRP");
                    //        dt.Rows.Add("blneditsalerate", "Allow user to Edit Rate");
                    //        dt.Rows.Add("BLNSHOWFREEQUANTITY", "Allow user to Enter Free Qty");
                    //        dt.Rows.Add("blnEditTaxPer", "Allow user to Edit Tax Percentage");
                    //        dt.Rows.Add("blnShowRateFixer", "Enable Rate Fixer");

                    //        break;
                    //    }
                    //case "18": //Quotation
                    //    {
                    //        //dt.Rows.Add("BLNAPPLYGIFTVOUCHER", "Apply Gift Voucher");
                    //        //dt.Rows.Add("BLNAPPLYOFFER", "Apply Offer Automatically");
                    //        dt.Rows.Add("BLNPOSTCASHENTRY", "Post Cash Entry In Supplier Ledger");
                    //        dt.Rows.Add("BLNDISPLAYLEDGERBALANCE", "Display ledger balance on Selection");
                    //        dt.Rows.Add("BLNNAMEANDMOBILENOMANDATORY", "Party Name & Mobile No Mandatory");
                    //        dt.Rows.Add("BLNAUTOCHANGERATEONPRICELIST", "Auto Change SRate on Pricelist Selection");
                    //        dt.Rows.Add("BLNSHOWPRICELIST", "Show Pricelist");
                    //        dt.Rows.Add("BLNSHOWOFFERINPOPUP", "Show Offer in Popup");
                    //        dt.Rows.Add("BLNPARTYDETAILS", "Show Party Details");
                    //        dt.Rows.Add("BLNENABLECASHDISCOUNT", "Show Cash Discount");
                    //        dt.Rows.Add("BLNENABLEEFFECIVEDATE", "Show Effective Date");
                    //        dt.Rows.Add("blnShowReferenceNo", "Show Reference No");
                    //        dt.Rows.Add("BLNSHOWBILLNARRATION", "Show Bill Narration");
                    //        dt.Rows.Add("BLNSHOWOTHEREXPENSE", "Show Other Expense");
                    //        dt.Rows.Add("BLNSHOWITEMCALCGRID", "Show Item Calculation Grid");
                    //        dt.Rows.Add("BLNSHOWITEMPROFITPER", "Show Item Rates Previous");
                    //        dt.Rows.Add("BLNSHOWPREVIEW", "Show Preview Before Print");
                    //        dt.Rows.Add("BLNPRINTCONFIRMATION", "Ask Confirmation on Print");
                    //        dt.Rows.Add("BLNPRINTIMMEDIATELY", "Send Bill to Printer on Save");
                    //        dt.Rows.Add("BLNSUMMARISEDUPLICATEITEMSINPRINT", "Summarise Duplicate Items In Print");
                    //        dt.Rows.Add("BLNSUMMARISEITEMSWHILEENTERING", "Summarise Duplicate Items While Entering");
                    //        //dt.Rows.Add("BLNSUMMARISEITEMSWHILEENTERING", "Summarise Items While Entering");
                    //        dt.Rows.Add("BLNEDITMRPRATE", "Allow user to Edit MRP in grid");
                    //        dt.Rows.Add("BLNEDITSALERATE", "Allow user to Edit SRate");
                    //        dt.Rows.Add("BLNSHOWFREEQUANTITY", "Allow user to Enter Free Qty");
                    //        dt.Rows.Add("BLNEDITTAXPER", "Allow user to Edit Tax Percentage");
                    //        dt.Rows.Add("BLNCASHDESK", "Enable Cash Desk");

                    //        break;
                    //    }
                    //case "20": //
                    //    {
                    //        dt.Rows.Add("blnautofillrawmaterial", "Auto Fill Raw Material");
                    //        dt.Rows.Add("blnshowscrap", "Show Scrap");
                    //        dt.Rows.Add("blnpartydetails", "Show Party Details");
                    //        dt.Rows.Add("blnenablecashdiscount", "Show Cash Discount");
                    //        dt.Rows.Add("blnenableEffeciveDate", "Show Effective Date");
                    //        dt.Rows.Add("blnshowbillnarration", "Show Bill Narration");
                    //        dt.Rows.Add("blnshowotherexpense", "Show Other Expense");
                    //        dt.Rows.Add("blnShowItemCalcGrid", "Show Item Calculation Grid");
                    //        dt.Rows.Add("blnshowpreview", "Show Preview Before Print");
                    //        dt.Rows.Add("blnprintconfirmation", "Ask Confirmation on Print");
                    //        dt.Rows.Add("blnprintimmediately", "Send Bill to Printer on Save");
                    //        dt.Rows.Add("blnSummariseDuplicateItemsInPrint", "Summarise Duplicate Items In Print");
                    //        dt.Rows.Add("blnSummariseItemsWhileEntering", "Summarise Duplicate Items While Entering");
                    //        dt.Rows.Add("BLNEDITMRPRATE", "Allow user to Edit MRP in grid");
                    //        dt.Rows.Add("blneditsalerate", "Allow user to Edit Rate");
                    //        dt.Rows.Add("blnEditTaxPer", "Allow user to Edit Tax Percentage");
                    //        break;
                    //    }
                    //case "41": //Physical Stock
                    //    {
                    //        dt.Rows.Add("BLNPOSTCASHENTRY", "Post Cash Entry In Supplier Ledger");
                    //        dt.Rows.Add("BLNRECALCULATESalesRatesOnPercentage", "Recalculate Sales Rates On Percentage");
                    //        dt.Rows.Add("blnpartydetails", "Show Party Details");
                    //        dt.Rows.Add("blnenablecashdiscount", "Show Cash Discount");
                    //        dt.Rows.Add("blnenableEffeciveDate", "Show Effective Date");
                    //        dt.Rows.Add("blnShowReferenceNo", "Show Reference No");
                    //        dt.Rows.Add("blnshowotherexpense", "Show Other Expenses");
                    //        dt.Rows.Add("blnshowbillnarration", "Show Bill Narration");
                    //        dt.Rows.Add("blnShowItemCalcGrid", "Show Item Calculation Grid");
                    //        dt.Rows.Add("blnShowItemProfitPer", "Show Product Profit Percentage");
                    //        dt.Rows.Add("blnshowpreview", "Show Preview before Print");
                    //        dt.Rows.Add("blnprintconfirmation", "Ask Confirmation on Print");
                    //        dt.Rows.Add("blnprintimmediately", "Send Bill to Printer on Save");
                    //        dt.Rows.Add("blnSummariseDuplicateItemsInPrint", "Summarise Duplicate Items In Print");
                    //        dt.Rows.Add("blnSummariseItemsWhileEntering", "Summarise Duplicate Items While Entering");
                    //        dt.Rows.Add("blnWarnifSRatelessthanPrate", "Warn If Sales Rate is less than Purchase Rate");
                    //        dt.Rows.Add("BLNEDITMRPRATE", "Allow user to Edit MRP");
                    //        dt.Rows.Add("blneditsalerate", "Allow user to Edit Rate");
                    //        dt.Rows.Add("BLNSHOWFREEQUANTITY", "Allow user to Enter Free Qty");
                    //        dt.Rows.Add("blnEditTaxPer", "Allow user to Edit Tax Percentage");
                    //        dt.Rows.Add("blnShowRateFixer", "Enable Rate Fixer");

                    //        break;
                    //    }
                    //case "1005": //Opening
                    //    {
                    //        dt.Rows.Add("BLNPOSTCASHENTRY", "Post Cash Entry In Supplier Ledger");
                    //        dt.Rows.Add("BLNRECALCULATESalesRatesOnPercentage", "Recalculate Sales Rates On Percentage");
                    //        dt.Rows.Add("blnpartydetails", "Show Party Details");
                    //        dt.Rows.Add("blnenablecashdiscount", "Show Cash Discount");
                    //        dt.Rows.Add("blnenableEffeciveDate", "Show Effective Date");
                    //        dt.Rows.Add("blnShowReferenceNo", "Show Reference No");
                    //        dt.Rows.Add("blnshowotherexpense", "Show Other Expenses");
                    //        dt.Rows.Add("blnshowbillnarration", "Show Bill Narration");
                    //        dt.Rows.Add("blnShowItemCalcGrid", "Show Item Calculation Grid");
                    //        dt.Rows.Add("blnShowItemProfitPer", "Show Product Profit Percentage");
                    //        dt.Rows.Add("blnshowpreview", "Show Preview before Print");
                    //        dt.Rows.Add("blnprintconfirmation", "Ask Confirmation on Print");
                    //        dt.Rows.Add("blnprintimmediately", "Send Bill to Printer on Save");
                    //        dt.Rows.Add("blnSummariseDuplicateItemsInPrint", "Summarise Duplicate Items In Print");
                    //        dt.Rows.Add("blnSummariseItemsWhileEntering", "Summarise Duplicate Items While Entering");
                    //        dt.Rows.Add("blnWarnifSRatelessthanPrate", "Warn If Sales Rate is less than Purchase Rate");
                    //        dt.Rows.Add("BLNEDITMRPRATE", "Allow user to Edit MRP");
                    //        dt.Rows.Add("blneditsalerate", "Allow user to Edit Rate");
                    //        dt.Rows.Add("BLNSHOWFREEQUANTITY", "Allow user to Enter Free Qty");
                    //        dt.Rows.Add("blnEditTaxPer", "Allow user to Edit Tax Percentage");
                    //        dt.Rows.Add("blnShowRateFixer", "Enable Rate Fixer");

                    //        break;
                    //    }

                    //case "1":
                    //    {
                    //        //dt.Rows.Add("BLNAPPLYGIFTVOUCHER", "Apply Gift Voucher");
                    //        //dt.Rows.Add("BLNAPPLYOFFER", "Apply Offer Automatically");
                    //        dt.Rows.Add("BLNAUTOCHANGERATEONPRICELIST", "Autochange Rate on Pricelist Selection");
                    //        dt.Rows.Add("BLNCASHDESK", "Enable CashDesk");
                    //        dt.Rows.Add("BLNDISPLAYLEDGERBALANCE", "Display ledger balance on Selection");
                    //        dt.Rows.Add("BLNEDITMRPRATE", "Edit MRP in grid");
                    //        dt.Rows.Add("BLNEDITSALERATE", "Edit Rate");
                    //        dt.Rows.Add("BLNEDITTAXPER", "Edit Tax Percentage");
                    //        dt.Rows.Add("BLNENABLECASHDISCOUNT", "Enable Cash Discount");
                    //        dt.Rows.Add("BLNENABLEEFFECIVEDATE", "Enable Effective Date");
                    //        dt.Rows.Add("BLNNAMEANDMOBILENOMANDATORY", "Party Name & MobileNo Mandatory");
                    //        dt.Rows.Add("BLNPARTYDETAILS", "Show Party Details");
                    //        dt.Rows.Add("BLNPOSTCASHENTRY", "Post Cash Entry In Supplier Ledger");
                    //        dt.Rows.Add("BLNPRINTCONFIRMATION", "Show Print Confirmation");
                    //        dt.Rows.Add("BLNPRINTIMMEDIATELY", "Print Immediately After Save");
                    //        dt.Rows.Add("BLNSHOWBILLNARRATION", "Show Bill Narration");
                    //        dt.Rows.Add("BLNSHOWFREEQUANTITY", "Enter Free Qty");
                    //        dt.Rows.Add("BLNSHOWITEMCALCGRID", "Show Item Calculation Grid");
                    //        dt.Rows.Add("BLNSHOWITEMPROFITPER", "Show Item Rates Previous");
                    //        dt.Rows.Add("BLNSHOWOFFERINPOPUP", "Show Offer in Popup");
                    //        dt.Rows.Add("BLNSHOWOTHEREXPENSE", "Show Other Expense");
                    //        dt.Rows.Add("BLNSHOWPREVIEW", "Show Preview Before Print");
                    //        dt.Rows.Add("BLNSHOWPRICELIST", "Show Pricelist");
                    //        dt.Rows.Add("blnShowReferenceNo", "Enable Reference No");
                    //        dt.Rows.Add("BLNSUMMARISEDUPLICATEITEMSINPRINT", "Summarise DuplicateItems In Print");
                    //        dt.Rows.Add("BLNSUMMARISEITEMSWHILEENTERING", "Summarise Items While Entering");
                    //        dt.Rows.Add("BLNSUMMARISEITEMSWHILEENTERING", "Summarise Items While Entering");

                    //        break;
                    //    }

                    //case "2":
                    //    {
                    //        dt.Rows.Add("BLNEDITMRPRATE", "Allow user to Edit MRP");
                    //        dt.Rows.Add("blneditsalerate", "Allow user to Edit Rate");
                    //        dt.Rows.Add("blnEditTaxPer", "Allow user to Edit Tax Percentage");
                    //        dt.Rows.Add("blnenablecashdiscount", "View Cash Discount");
                    //        dt.Rows.Add("blnenableEffeciveDate", "View Effective Date");
                    //        dt.Rows.Add("blnpartydetails", "View Party Details");
                    //        dt.Rows.Add("BLNPOSTCASHENTRY", "Post Cash Entry In Supplier Ledger");
                    //        dt.Rows.Add("blnprintconfirmation", "Ask Confirmation on Print");
                    //        dt.Rows.Add("blnprintimmediately", "Send Bill to Printer on Save");
                    //        dt.Rows.Add("BLNRECALCULATESalesRatesOnPercentage", "Recalculate SalesRates On Percentage");
                    //        dt.Rows.Add("blnshowbillnarration", "Enable Bill Narration");
                    //        dt.Rows.Add("BLNSHOWFREEQUANTITY", "Enable Free Qty");
                    //        dt.Rows.Add("blnShowItemCalcGrid", "Show Item Calculation Grid");
                    //        dt.Rows.Add("blnShowItemProfitPer", "Show Product Profit Percentage");
                    //        dt.Rows.Add("blnshowotherexpense", "Enable Other Expenses");
                    //        dt.Rows.Add("blnshowpreview", "Show Preview before Print");
                    //        dt.Rows.Add("blnShowRateFixer", "Enable Rate Fixer");
                    //        dt.Rows.Add("blnShowReferenceNo", "Enable Reference No");
                    //        dt.Rows.Add("blnSummariseDuplicateItemsInPrint", "Summarise DuplicateItems In Print");
                    //        dt.Rows.Add("blnSummariseItemsWhileEntering", "Summarise Items While Entering");
                    //        dt.Rows.Add("blnWarnifSRatelessthanPrate", "Validate Sales Rate is less than Purchase Rate");

                    //        break;
                    //    }

                    //case "3":
                    //    {
                    //        dt.Rows.Add("BLNAPPLYGIFTVOUCHER", "Apply Gift Voucher");
                    //        dt.Rows.Add("BLNAPPLYOFFER", "Apply Offer Automatically");
                    //        dt.Rows.Add("blnautochangerateonpricelist", "Autochange Rate on Pricelist Selection");
                    //        dt.Rows.Add("blnCashDesk", "Enable CashDesk");
                    //        dt.Rows.Add("BLNEDITMRPRATE", "Edit MRP in grid");
                    //        dt.Rows.Add("blneditsalerate", "Edit Rate");
                    //        dt.Rows.Add("blnEditTaxPer", "Edit Tax Percentage");
                    //        dt.Rows.Add("blnenablecashdiscount", "Enable Cash Discount");
                    //        dt.Rows.Add("blnenableEffeciveDate", "Enable Effective Date");
                    //        dt.Rows.Add("blnNameandMObilenoMandatory", "Party Name & MobileNo Mandatory");
                    //        dt.Rows.Add("blnpartydetails", "Show Party Details");
                    //        dt.Rows.Add("blnprintconfirmation", "Show Print Confirmation");
                    //        dt.Rows.Add("blnprintimmediately", "Print Immediately After Save");
                    //        dt.Rows.Add("blnshowbillnarration", "Show Bill Narration");
                    //        dt.Rows.Add("BLNSHOWFREEQUANTITY", "Enter Free Qty");
                    //        dt.Rows.Add("blnShowItemCalcGrid", "Show Item Calculation Grid");
                    //        dt.Rows.Add("blnShowItemProfitPer", "Show Item Rates Previous");
                    //        dt.Rows.Add("blnShowOfferinPopup", "Show Offer in Popup");
                    //        dt.Rows.Add("blnshowotherexpense", "Show Other Expense");
                    //        dt.Rows.Add("blnshowpreview", "Show Preview Before Print");
                    //        dt.Rows.Add("blnshowpricelist", "Show Pricelist");
                    //        dt.Rows.Add("blnSummariseDuplicateItemsInPrint", "Summarise DuplicateItems In Print");
                    //        dt.Rows.Add("blnSummariseItemsWhileEntering", "Summarise Items While Entering");

                    //        break;
                    //    }

                    //case "4":
                    //    {
                    //        dt.Rows.Add("BLNEDITMRPRATE", "Edit MRP in grid");
                    //        dt.Rows.Add("blneditsalerate", "Edit Rate");
                    //        dt.Rows.Add("blnEditTaxPer", "Edit Tax Percentage");
                    //        dt.Rows.Add("blnenablecashdiscount", "Enable Cash Discount");
                    //        dt.Rows.Add("blnenableEffeciveDate", "Enable Effective Date");
                    //        dt.Rows.Add("blnpartydetails", "Show Party Details");
                    //        dt.Rows.Add("blnprintconfirmation", "Show Print Confirmation");
                    //        dt.Rows.Add("blnprintimmediately", "Print Immediately After Save");
                    //        dt.Rows.Add("BLNRECALCULATESalesRatesOnPercentage", "Recalculate SalesRates On Percentage");
                    //        dt.Rows.Add("blnshowbillnarration", "Show Bill Narration");
                    //        dt.Rows.Add("BLNSHOWFREEQUANTITY", "Enter Free Qty");
                    //        dt.Rows.Add("blnShowItemCalcGrid", "Show Item Calculation Grid");
                    //        dt.Rows.Add("blnShowItemProfitPer", "Show Item Rates Previous");
                    //        dt.Rows.Add("blnshowotherexpense", "Show Other Expense");
                    //        dt.Rows.Add("blnshowpreview", "Show Preview Before Print");
                    //        dt.Rows.Add("blnShowRateFixer", "Show Rate Fixer");
                    //        dt.Rows.Add("blnShowReferenceNo", "Show Reference Number");
                    //        dt.Rows.Add("blnSummariseDuplicateItemsInPrint", "Summarise DuplicateItems In Print");
                    //        dt.Rows.Add("blnSummariseItemsWhileEntering", "Summarise Items While Entering");
                    //        dt.Rows.Add("blnWarnifSRatelessthanPrate", "Warn If Sales Rate Less than Purchase Rate");

                    //        break;
                    //    }
                    //case "5":
                    //    {
                    //        dt.Rows.Add("BLNEDITMRPRATE", "Edit MRP in grid");
                    //        dt.Rows.Add("blneditsalerate", "Edit Rate");
                    //        dt.Rows.Add("blnEditTaxPer", "Edit Tax Percentage");
                    //        dt.Rows.Add("blnenablecashdiscount", "Enable Cash Discount");
                    //        dt.Rows.Add("blnenableEffeciveDate", "Enable Effective Date");
                    //        dt.Rows.Add("blnpartydetails", "Show Party Details");
                    //        dt.Rows.Add("blnprintconfirmation", "Show Print Confirmation");
                    //        dt.Rows.Add("blnprintimmediately", "Print Immediately After Save");
                    //        dt.Rows.Add("BLNRECALCULATESalesRatesOnPercentage", "Recalculate SalesRates On Percentage");
                    //        dt.Rows.Add("blnshowbillnarration", "Show Bill Narration");
                    //        dt.Rows.Add("BLNSHOWFREEQUANTITY", "Enter Free Qty");
                    //        dt.Rows.Add("blnShowItemCalcGrid", "Show Item Calculation Grid");
                    //        dt.Rows.Add("blnShowItemProfitPer", "Show Item Rates Previous");
                    //        dt.Rows.Add("blnshowotherexpense", "Show Other Expense");
                    //        dt.Rows.Add("blnshowpreview", "Show Preview Before Print");
                    //        dt.Rows.Add("blnShowRateFixer", "Show Rate Fixer");
                    //        dt.Rows.Add("blnShowReferenceNo", "Show Reference Number");
                    //        dt.Rows.Add("blnSummariseDuplicateItemsInPrint", "Summarise DuplicateItems In Print");
                    //        dt.Rows.Add("blnSummariseItemsWhileEntering", "Summarise Items While Entering");
                    //        dt.Rows.Add("blnWarnifSRatelessthanPrate", "Warn If Sales Rate Less than Purchase Rate");

                    //        break;
                    //    }
                    //case "6":
                    //    {
                    //        dt.Rows.Add("BLNEDITMRPRATE", "Edit MRP in grid");
                    //        dt.Rows.Add("blneditsalerate", "Edit Rate");
                    //        dt.Rows.Add("blnEditTaxPer", "Edit Tax Percentage");
                    //        dt.Rows.Add("blnenablecashdiscount", "Enable Cash Discount");
                    //        dt.Rows.Add("blnenableEffeciveDate", "Enable Effective Date");
                    //        dt.Rows.Add("blnpartydetails", "Show Party Details");
                    //        dt.Rows.Add("blnprintconfirmation", "Show Print Confirmation");
                    //        dt.Rows.Add("blnprintimmediately", "Print Immediately After Save");
                    //        dt.Rows.Add("BLNRECALCULATESalesRatesOnPercentage", "Recalculate SalesRates On Percentage");
                    //        dt.Rows.Add("blnshowbillnarration", "Show Bill Narration");
                    //        dt.Rows.Add("BLNSHOWFREEQUANTITY", "Enter Free Qty");
                    //        dt.Rows.Add("blnShowItemCalcGrid", "Show Item Calculation Grid");
                    //        dt.Rows.Add("blnShowItemProfitPer", "Show Item Rates Previous");
                    //        dt.Rows.Add("blnshowotherexpense", "Show Other Expense");
                    //        dt.Rows.Add("blnshowpreview", "Show Preview Before Print");
                    //        dt.Rows.Add("blnShowRateFixer", "Show Rate Fixer");
                    //        dt.Rows.Add("blnShowReferenceNo", "Show Reference Number");
                    //        dt.Rows.Add("blnSummariseDuplicateItemsInPrint", "Summarise DuplicateItems In Print");
                    //        dt.Rows.Add("blnSummariseItemsWhileEntering", "Summarise Items While Entering");
                    //        dt.Rows.Add("blnWarnifSRatelessthanPrate", "Warn If Sales Rate Less than Purchase Rate");

                    //        break;
                    //    }
                    //case "7":
                    //    {
                    //        dt.Rows.Add("BLNALLOWDUPLICATELEDGERS", "Allow Duplicate ledgers");
                    //        dt.Rows.Add("BLNCHEQUEDETAILS", "Cheque Details");
                    //        dt.Rows.Add("BLNDISABLEANYWHERESEARCHINGOFLEDGERS", "Disable anywhere searching of ledgers");
                    //        dt.Rows.Add("BLNDISPLAYLEDGERBALANCE", "Display ledger balance on Selection");
                    //        dt.Rows.Add("BLNDUALENTRYMODE", "Dual Entry Mode For Ledgers");
                    //        dt.Rows.Add("BLNENABLEEFFECIVEDATE", "Enable Effective Date");
                    //        dt.Rows.Add("BLNPARTYDETAILS", "Show Party Details");
                    //        dt.Rows.Add("BLNPOSTONEFFECTIVEDATE", "Post Accounts on Effective Date");
                    //        dt.Rows.Add("BLNPRINTIMMEDIATELY", "Print Immediately After Save");
                    //        dt.Rows.Add("BLNSHOWBILLNARRATION", "Show Bill Narration");
                    //        dt.Rows.Add("BLNSHOWITEMCALCGRID", "Show Item Calculation Grid");
                    //        dt.Rows.Add("BLNSHOWLEDGERBALANCES", "Show ledger balances");
                    //        dt.Rows.Add("BLNSHOWPREVIEW", "Show Preview Before Print");
                    //        dt.Rows.Add("BLNSHOWQTY", "Show Qty Column");
                    //        dt.Rows.Add("BLNSHOWREFERENCENO", "Show Reference Number");
                    //        dt.Rows.Add("CHEQUEPRINTING", "Cheque Printing");

                    //        break;
                    //    }
                    //case "8":
                    //    {
                    //        dt.Rows.Add("blnallowduplicateledgers", "Allow Duplicate ledgers");
                    //        dt.Rows.Add("blnchequedetails", "Cheque Details");
                    //        dt.Rows.Add("blndisableanywheresearchingofledgers", "Disable anywhere searching of ledgers");
                    //        dt.Rows.Add("blndisplayledgerbalance", "Display ledger balance on Selection");
                    //        dt.Rows.Add("blnenableEffeciveDate", "Enable Effective Date");
                    //        dt.Rows.Add("blnpartydetails", "Show Party Details");
                    //        dt.Rows.Add("BLNPOSTONEFFECTIVEDATE", "Post Accounts on Effective Date");
                    //        dt.Rows.Add("blnprintimmediately", "Print Immediately After Save");
                    //        dt.Rows.Add("blnshowbillnarration", "Show Bill Narration");
                    //        dt.Rows.Add("blnShowItemCalcGrid", "Show Item Calculation Grid");
                    //        dt.Rows.Add("blnshowledgerbalances", "Show ledger balances");
                    //        dt.Rows.Add("blnshowpreview", "Show Preview Before Print");
                    //        dt.Rows.Add("blnshowQty", "Show Qty Column");
                    //        dt.Rows.Add("blnShowReferenceNo", "Show Reference Number");
                    //        dt.Rows.Add("chequeprinting", "Cheque Printing");

                    //        break;
                    //    }
                    //case "9":
                    //    {
                    //        dt.Rows.Add("blnallowduplicateledgers", "Allow Duplicate ledgers");
                    //        dt.Rows.Add("blnchequedetails", "Cheque Details");
                    //        dt.Rows.Add("blndisableanywheresearchingofledgers", "Disable anywhere searching of ledgers");
                    //        dt.Rows.Add("blndisplayledgerbalance", "Display ledger balance on Selection");
                    //        dt.Rows.Add("blnenableEffeciveDate", "Enable Effective Date");
                    //        dt.Rows.Add("blnpartydetails", "Show Party Details");
                    //        dt.Rows.Add("BLNPOSTONEFFECTIVEDATE", "Post Accounts on Effective Date");
                    //        dt.Rows.Add("blnprintimmediately", "Print Immediately After Save");
                    //        dt.Rows.Add("blnshowbillnarration", "Show Bill Narration");
                    //        dt.Rows.Add("blnShowItemCalcGrid", "Show Item Calculation Grid");
                    //        dt.Rows.Add("blnshowledgerbalances", "Show ledger balances");
                    //        dt.Rows.Add("blnshowpreview", "Show Preview Before Print");
                    //        dt.Rows.Add("blnshowQty", "Show Qty Column");
                    //        dt.Rows.Add("blnShowReferenceNo", "Show Reference Number");
                    //        dt.Rows.Add("chequeprinting", "Cheque Printing");

                    //        break;
                    //    }
                    //case "10":
                    //    {
                    //        dt.Rows.Add("blnallowduplicateledgers", "Allow Duplicate ledgers");
                    //        dt.Rows.Add("blnchequedetails", "Cheque Details");
                    //        dt.Rows.Add("blndisableanywheresearchingofledgers", "Disable anywhere searching of ledgers");
                    //        dt.Rows.Add("blndisplayledgerbalance", "Display ledger balance on Selection");
                    //        dt.Rows.Add("blnenableEffeciveDate", "Enable Effective Date");
                    //        dt.Rows.Add("blnpartydetails", "Show Party Details");
                    //        dt.Rows.Add("BLNPOSTONEFFECTIVEDATE", "Post Accounts on Effective Date");
                    //        dt.Rows.Add("blnprintimmediately", "Print Immediately After Save");
                    //        dt.Rows.Add("blnshowbillnarration", "Show Bill Narration");
                    //        dt.Rows.Add("blnShowItemCalcGrid", "Show Item Calculation Grid");
                    //        dt.Rows.Add("blnshowledgerbalances", "Show ledger balances");
                    //        dt.Rows.Add("blnshowpreview", "Show Preview Before Print");
                    //        dt.Rows.Add("blnshowQty", "Show Qty Column");
                    //        dt.Rows.Add("blnShowReferenceNo", "Show Reference Number");
                    //        dt.Rows.Add("chequeprinting", "Cheque Printing");

                    //        break;
                    //    }
                    //case "14":
                    //    {
                    //        dt.Rows.Add("BLNEDITMRPRATE", "Edit MRP in grid");
                    //        dt.Rows.Add("blneditsalerate", "Edit Rate");
                    //        dt.Rows.Add("blnEditTaxPer", "Edit Tax Percentage");
                    //        dt.Rows.Add("blnenablecashdiscount", "Enable Cash Discount");
                    //        dt.Rows.Add("blnenableEffeciveDate", "Enable Effective Date");
                    //        dt.Rows.Add("blnpartydetails", "Show Party Details");
                    //        dt.Rows.Add("blnprintconfirmation", "Show Print Confirmation");
                    //        dt.Rows.Add("blnprintimmediately", "Print Immediately After Save");
                    //        dt.Rows.Add("BLNRECALCULATESalesRatesOnPercentage", "Recalculate SalesRates On Percentage");
                    //        dt.Rows.Add("blnshowbillnarration", "Show Bill Narration");
                    //        dt.Rows.Add("BLNSHOWFREEQUANTITY", "Enter Free Qty");
                    //        dt.Rows.Add("blnShowItemCalcGrid", "Show Item Calculation Grid");
                    //        dt.Rows.Add("blnShowItemProfitPer", "Show Item Rates Previous");
                    //        dt.Rows.Add("blnshowotherexpense", "Show Other Expense");
                    //        dt.Rows.Add("blnshowpreview", "Show Preview Before Print");
                    //        dt.Rows.Add("blnShowRateFixer", "Show Rate Fixer");
                    //        dt.Rows.Add("blnShowReferenceNo", "Show Reference Number");
                    //        dt.Rows.Add("blnSummariseDuplicateItemsInPrint", "Summarise DuplicateItems In Print");
                    //        dt.Rows.Add("blnSummariseItemsWhileEntering", "Summarise Items While Entering");
                    //        dt.Rows.Add("blnWarnifSRatelessthanPrate", "Warn If Sales Rate Less than Purchase Rate");

                    //        break;
                    //    }
                    //case "15":
                    //    {
                    //        dt.Rows.Add("BLNEDITMRPRATE", "Edit MRP in grid");
                    //        dt.Rows.Add("blneditsalerate", "Edit Rate");
                    //        dt.Rows.Add("blnEditTaxPer", "Edit Tax Percentage");
                    //        dt.Rows.Add("blnenablecashdiscount", "Enable Cash Discount");
                    //        dt.Rows.Add("blnenableEffeciveDate", "Enable Effective Date");
                    //        dt.Rows.Add("blnpartydetails", "Show Party Details");
                    //        dt.Rows.Add("blnprintconfirmation", "Show Print Confirmation");
                    //        dt.Rows.Add("blnprintimmediately", "Print Immediately After Save");
                    //        dt.Rows.Add("BLNRECALCULATESalesRatesOnPercentage", "Recalculate SalesRates On Percentage");
                    //        dt.Rows.Add("blnshowbillnarration", "Show Bill Narration");
                    //        dt.Rows.Add("BLNSHOWFREEQUANTITY", "Enter Free Qty");
                    //        dt.Rows.Add("blnShowItemCalcGrid", "Show Item Calculation Grid");
                    //        dt.Rows.Add("blnShowItemProfitPer", "Show Item Rates Previous");
                    //        dt.Rows.Add("blnshowotherexpense", "Show Other Expense");
                    //        dt.Rows.Add("blnshowpreview", "Show Preview Before Print");
                    //        dt.Rows.Add("blnShowRateFixer", "Show Rate Fixer");
                    //        dt.Rows.Add("blnShowReferenceNo", "Show Reference Number");
                    //        dt.Rows.Add("blnSummariseDuplicateItemsInPrint", "Summarise DuplicateItems In Print");
                    //        dt.Rows.Add("blnSummariseItemsWhileEntering", "Summarise Items While Entering");
                    //        dt.Rows.Add("blnWarnifSRatelessthanPrate", "Warn If Sales Rate Less than Purchase Rate");

                    //        break;
                    //    }
                    //case "16":
                    //    {
                    //        dt.Rows.Add("BLNEDITMRPRATE", "Edit MRP in grid");
                    //        dt.Rows.Add("blneditsalerate", "Edit Rate");
                    //        dt.Rows.Add("blnEditTaxPer", "Edit Tax Percentage");
                    //        dt.Rows.Add("blnenablecashdiscount", "Enable Cash Discount");
                    //        dt.Rows.Add("blnenableEffeciveDate", "Enable Effective Date");
                    //        dt.Rows.Add("blnpartydetails", "Show Party Details");
                    //        dt.Rows.Add("blnprintconfirmation", "Show Print Confirmation");
                    //        dt.Rows.Add("blnprintimmediately", "Print Immediately After Save");
                    //        dt.Rows.Add("BLNRECALCULATESalesRatesOnPercentage", "Recalculate SalesRates On Percentage");
                    //        dt.Rows.Add("blnshowbillnarration", "Show Bill Narration");
                    //        dt.Rows.Add("BLNSHOWFREEQUANTITY", "Enter Free Qty");
                    //        dt.Rows.Add("blnShowItemCalcGrid", "Show Item Calculation Grid");
                    //        dt.Rows.Add("blnShowItemProfitPer", "Show Item Rates Previous");
                    //        dt.Rows.Add("blnshowotherexpense", "Show Other Expense");
                    //        dt.Rows.Add("blnshowpreview", "Show Preview Before Print");
                    //        dt.Rows.Add("blnShowRateFixer", "Show Rate Fixer");
                    //        dt.Rows.Add("blnShowReferenceNo", "Show Reference Number");
                    //        dt.Rows.Add("blnSummariseDuplicateItemsInPrint", "Summarise DuplicateItems In Print");
                    //        dt.Rows.Add("blnSummariseItemsWhileEntering", "Summarise Items While Entering");
                    //        dt.Rows.Add("blnWarnifSRatelessthanPrate", "Warn If Sales Rate Less than Purchase Rate");

                    //        break;
                    //    }
                    //case "18":
                    //    {
                    //        dt.Rows.Add("BLNAPPLYGIFTVOUCHER", "Apply Gift Voucher");
                    //        dt.Rows.Add("BLNAPPLYOFFER", "Apply Offer Automatically");
                    //        dt.Rows.Add("blnautochangerateonpricelist", "Autochange Rate on Pricelist Selection");
                    //        dt.Rows.Add("blnCashDesk", "Enable CashDesk");
                    //        dt.Rows.Add("BLNEDITMRPRATE", "Edit MRP in grid");
                    //        dt.Rows.Add("blneditsalerate", "Edit Rate");
                    //        dt.Rows.Add("blnEditTaxPer", "Edit Tax Percentage");
                    //        dt.Rows.Add("blnenablecashdiscount", "Enable Cash Discount");
                    //        dt.Rows.Add("blnenableEffeciveDate", "Enable Effective Date");
                    //        dt.Rows.Add("blnNameandMObilenoMandatory", "Party Name & MobileNo Mandatory");
                    //        dt.Rows.Add("blnpartydetails", "Show Party Details");
                    //        dt.Rows.Add("blnprintconfirmation", "Show Print Confirmation");
                    //        dt.Rows.Add("blnprintimmediately", "Print Immediately After Save");
                    //        dt.Rows.Add("blnshowbillnarration", "Show Bill Narration");
                    //        dt.Rows.Add("BLNSHOWFREEQUANTITY", "Enter Free Qty");
                    //        dt.Rows.Add("blnShowItemCalcGrid", "Show Item Calculation Grid");
                    //        dt.Rows.Add("blnShowItemProfitPer", "Show Item Rates Previous");
                    //        dt.Rows.Add("blnShowOfferinPopup", "Show Offer in Popup");
                    //        dt.Rows.Add("blnshowotherexpense", "Show Other Expense");
                    //        dt.Rows.Add("blnshowpreview", "Show Preview Before Print");
                    //        dt.Rows.Add("blnshowpricelist", "Show Pricelist");
                    //        dt.Rows.Add("blnSummariseDuplicateItemsInPrint", "Summarise DuplicateItems In Print");
                    //        dt.Rows.Add("blnSummariseItemsWhileEntering", "Summarise Items While Entering");

                    //        break;
                    //    }
                    //case "20":
                    //    {
                    //        dt.Rows.Add("blnautofillrawmaterial", "Auto Fill Raw Material");
                    //        dt.Rows.Add("BLNEDITMRPRATE", "Edit MRP in grid");
                    //        dt.Rows.Add("blneditsalerate", "Edit Rate");
                    //        dt.Rows.Add("blnEditTaxPer", "Edit Tax Percentage");
                    //        dt.Rows.Add("blnenablecashdiscount", "Enable Cash Discount");
                    //        dt.Rows.Add("blnenableEffeciveDate", "Enable Effective Date");
                    //        dt.Rows.Add("blnpartydetails", "Show Party Details");
                    //        dt.Rows.Add("blnprintconfirmation", "Show Print Confirmation");
                    //        dt.Rows.Add("blnprintimmediately", "Print Immediately After Save");
                    //        dt.Rows.Add("blnshowbillnarration", "Show Bill Narration");
                    //        dt.Rows.Add("blnShowItemCalcGrid", "Show Item Calculation Grid");
                    //        dt.Rows.Add("blnshowotherexpense", "Show Other Expense");
                    //        dt.Rows.Add("blnshowpreview", "Show Preview Before Print");
                    //        dt.Rows.Add("blnshowscrap", "Show Scrap");
                    //        dt.Rows.Add("blnSummariseDuplicateItemsInPrint", "Summarise DuplicateItems In Print");
                    //        dt.Rows.Add("blnSummariseItemsWhileEntering", "Summarise Items While Entering");

                    //        break;
                    //    }
                    //case "41":
                    //    {
                    //        dt.Rows.Add("BLNEDITMRPRATE", "Edit MRP in grid");
                    //        dt.Rows.Add("BLNEDITSALERATE", "Edit Rate");
                    //        dt.Rows.Add("BLNEDITTAXPER", "Edit Tax Percentage");
                    //        dt.Rows.Add("BLNENABLECASHDISCOUNT", "Enable Cash Discount");
                    //        dt.Rows.Add("BLNENABLEEFFECIVEDATE", "Enable Effective Date");
                    //        dt.Rows.Add("BLNPARTYDETAILS", "Show Party Details");
                    //        dt.Rows.Add("BLNPRINTCONFIRMATION", "Show Print Confirmation");
                    //        dt.Rows.Add("BLNPRINTIMMEDIATELY", "Print Immediately After Save");
                    //        dt.Rows.Add("BLNRECALCULATESALESRATESONPERCENTAGE", "Recalculate SalesRates On Percentage");
                    //        dt.Rows.Add("BLNSHOWBILLNARRATION", "Show Bill Narration");
                    //        dt.Rows.Add("BLNSHOWFREEQUANTITY", "Enter Free Qty");
                    //        dt.Rows.Add("BLNSHOWITEMCALCGRID", "Show Item Calculation Grid");
                    //        dt.Rows.Add("BLNSHOWITEMPROFITPER", "Show Item Rates Previous");
                    //        dt.Rows.Add("BLNSHOWOTHEREXPENSE", "Show Other Expense");
                    //        dt.Rows.Add("BLNSHOWPREVIEW", "Show Preview Before Print");
                    //        dt.Rows.Add("BLNSHOWRATEFIXER", "Show Rate Fixer");
                    //        dt.Rows.Add("BLNSHOWREFERENCENO", "Show Reference Number");
                    //        dt.Rows.Add("BLNSUMMARISEDUPLICATEITEMSINPRINT", "Summarise DuplicateItems In Print");
                    //        dt.Rows.Add("BLNSUMMARISEITEMSWHILEENTERING", "Summarise Items While Entering");
                    //        dt.Rows.Add("BLNWARNIFSRATELESSTHANPRATE", "Warn If Sales Rate Less than Purchase Rate");

                    //        break;
                    //    }
                    //case "1005":
                    //    {
                    //        dt.Rows.Add("BLNEDITMRPRATE", "Edit MRP in grid");
                    //        dt.Rows.Add("blneditsalerate", "Edit Rate");
                    //        dt.Rows.Add("blnEditTaxPer", "Edit Tax Percentage");
                    //        dt.Rows.Add("blnenablecashdiscount", "Enable Cash Discount");
                    //        dt.Rows.Add("blnenableEffeciveDate", "Enable Effective Date");
                    //        dt.Rows.Add("blnpartydetails", "Show Party Details");
                    //        dt.Rows.Add("blnprintconfirmation", "Show Print Confirmation");
                    //        dt.Rows.Add("blnprintimmediately", "Print Immediately After Save");
                    //        dt.Rows.Add("BLNRECALCULATESalesRatesOnPercentage", "Recalculate SalesRates On Percentage");
                    //        dt.Rows.Add("blnshowbillnarration", "Show Bill Narration");
                    //        dt.Rows.Add("BLNSHOWFREEQUANTITY", "Enter Free Qty");
                    //        dt.Rows.Add("blnShowItemCalcGrid", "Show Item Calculation Grid");
                    //        dt.Rows.Add("blnShowItemProfitPer", "Show Item Rates Previous");
                    //        dt.Rows.Add("blnshowotherexpense", "Show Other Expense");
                    //        dt.Rows.Add("blnshowpreview", "Show Preview Before Print");
                    //        dt.Rows.Add("blnShowRateFixer", "Show Rate Fixer");
                    //        dt.Rows.Add("blnShowReferenceNo", "Show Reference Number");
                    //        dt.Rows.Add("blnSummariseDuplicateItemsInPrint", "Summarise DuplicateItems In Print");
                    //        dt.Rows.Add("blnSummariseItemsWhileEntering", "Summarise Items While Entering");
                    //        dt.Rows.Add("blnWarnifSRatelessthanPrate", "Warn If Sales Rate Less than Purchase Rate");

                    //        break;
                    //    }
                    case null:
                        return new DataTable();
                }

                return dt;
            }
            catch(Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return new DataTable();
            }
        }

        //Description : Load Features Grid Data
        private void CallGridData(decimal dEditId = 0)
        {
            try
            { 
            decimal dVchType = 0;

            if (dEditId == 0)
                dVchType = Convert.ToInt32(cboParentTrans.SelectedValue);
            else
                dVchType = dEditId;
            int iUserGrpId = 0;


                //This method fails if an error occur and the features is not saved
                //DataTable dtGen = Comm.fnGetData("SELECT * FROM tblvchtypeGenSettings  WHERE vchtypeID=" + dVchType + " and userid=1 order by SettingsDescription").Tables[0];

                DataTable dtGen = new DataTable();
                dtGen = GetVchtypeFeatures();



                DataGridTextBoxColumn txtGenSet = new DataGridTextBoxColumn();
            dgvFeatures.Rows.Clear();
            dgvFeatures.Columns.Clear();
            dgvFeatures.ColumnCount = 2;
            dgvFeatures.Columns[0].Name = "Settings";
            dgvFeatures.Columns[1].Name = "General Settings";

            DataTable dtUGroup = new DataTable();
            dtUGroup = Comm.fnGetData("SELECT GroupName,ID FROM tblUserGroupMaster order by groupname ASC").Tables[0];
            if (dtUGroup.Rows.Count > 0)
            {
                for (int b = 0; b < dtUGroup.Rows.Count - 1; b++)
                {
                    DataGridViewCheckBoxColumn chkUgroup = new DataGridViewCheckBoxColumn();
                    dgvFeatures.Columns.Add(chkUgroup);
                    chkUgroup.HeaderText = dtUGroup.Rows[b]["GroupName"].ToString();
                    chkUgroup.Tag = dtUGroup.Rows[b]["ID"].ToString();
                    chkUgroup.Name = dtUGroup.Rows[b]["GroupName"].ToString();
                }

                for (int c = 0; c < dtGen.Rows.Count; c++)
                {
                    dgvFeatures.Rows.Add(dtGen.Rows[c]["SettingsName"].ToString(), dtGen.Rows[c]["SettingsDescription"].ToString());
                }

                DataTable dtChk = new DataTable();
                //Comented And Added By Anjitha 08/03/2022 01:38 PM 
                //for (int d = 0; d < dgvFeatures.Rows.Count - 2; d++)
                for (int d = 0; d < dgvFeatures.Rows.Count; d++)
                {
                    for (int e = 2; e < dgvFeatures.Columns.Count; e++)
                    {
                        iUserGrpId = Convert.ToInt32(dgvFeatures.Columns[e].Tag.ToString());
                        dtChk = Comm.fnGetData("SELECT blnEnabled FROM tblvchtypeGenSettings WHERE UserID=" + iUserGrpId + " AND SettingsName='" + dgvFeatures.Rows[d].Cells[0].Value.ToString() + "' AND vchtypeid=" + dVchType + "").Tables[0];
                        if (dtChk.Rows.Count > 0)
                        {
                            if (Convert.ToInt32(dtChk.Rows[0]["blnEnabled"].ToString()) == 1)
                                dgvFeatures.Rows[d].Cells[e].Value = true;
                            else
                                dgvFeatures.Rows[d].Cells[e].Value = false;
                        }
                        dgvFeatures.Columns[e].Width = 80;
                        dgvFeatures.Columns[e].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    }
                }

                dgvFeatures.AllowUserToAddRows = false;
                dgvFeatures.Columns[0].Width = 0;
                dgvFeatures.Columns[0].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgvFeatures.Columns[0].Visible = false;
                dgvFeatures.Columns[1].Width = 200;
                dgvFeatures.Columns[1].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
        }
        //Description : Delete Data from Vchetype General Settings
        private void DeleteGeneralSettings(decimal iVchType)
        {
            try
            { 
            if (dIDFromEditWindow != 0)
            {
                DataTable dt = Comm.fnGetData("EXEC UspvchtypeGenSettingsInsert " + iVchType + ", '', '',0,1,'',1,'',''," + Global.gblTenantID + ",2").Tables[0];
            }
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
        }
        //Description : Load Saved data from database from edit window
        private void LoadData(decimal dSelID = 0, bool blnInherit = false)
        {
            try
            { 
            DataTable dtGet = new DataTable();
            string sVchJson = "", sVchFeatJson = "";
            if (dSelID > 0)
            {
                GetVchTyp.VchTypeID = dSelID;
                GetVchTyp.VchTypeIDs = "";
                GetVchTyp.TenantID = Global.gblTenantID;
                dtGet = clsVouchTyp.GetVchType(GetVchTyp);
                if (dtGet.Rows.Count > 0)
                {
                    sVchJson = dtGet.Rows[0]["VchJson"].ToString();
                    sVchFeatJson = dtGet.Rows[0]["FeaturesJson"].ToString();
                    JsonSerialLizeAndDeserializeObject(false, sVchJson, blnInherit);
                    FeaturesJsonSerializeAndDeserializeObject(false, sVchFeatJson);
                }
            }
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
        }
        //Description : Serialize and Deserialize Features Details for Save Data through Json
        private string FeaturesJsonSerializeAndDeserializeObject(bool bIsSerialize = true, string sToDeSerialize = "", decimal dVchtyID = 0)
        {
            try
            { 
            clsJsonVchTypeFeatures clsFVch = new clsJsonVchTypeFeatures();

                bool blnResult = false;
                string strFailedSettings = "";

            if (bIsSerialize == true)
            {
                for (int g = 2; g < dgvFeatures.Columns.Count; g++)
                {
                    for (int h = 1; h < dgvFeatures.Rows.Count; h++)
                    {
                        clsFVch.VchTypeID = dVchtyID;
                        clsFVch.SettingsName = dgvFeatures.Rows[h].Cells[0].Value.ToString().Trim().ToUpper();
                        clsFVch.SettingsDescription = dgvFeatures.Rows[h].Cells[1].Value.ToString();
                        if (Convert.ToBoolean(dgvFeatures.Rows[h].Cells[g].Value) == false)
                            clsFVch.BlnEnabled = 0;
                        else
                            clsFVch.BlnEnabled = 1;
                        clsFVch.UserID = Convert.ToDecimal(dgvFeatures.Columns[g].Tag.ToString());
                        clsFVch.SystemName = Global.gblSystemName;
                        clsFVch.UserID1 = Global.gblUserID;
                        clsFVch.LastUpdateDate = DateTime.Today;
                        clsFVch.LastUpdateTime = DateTime.Now;
                        clsFVch.TenantID = Global.gblTenantID;
                        //blnResult = SaveGeneralSettings(clsFVch);

                            //if (blnResult == false)
                            //    strFailedSettings = strFailedSettings + clsFVch.SettingsDescription + "  ;  ";

                            lstFeatures.Add(new clsJsonVchTypeFeatures { VchTypeID = clsFVch.VchTypeID, SettingsName = clsFVch.SettingsName, SettingsDescription = clsFVch.SettingsDescription, BlnEnabled = clsFVch.BlnEnabled, UserID = clsFVch.UserID, SystemName = clsFVch.SystemName, UserID1 = clsFVch.UserID1, LastUpdateDate = clsFVch.LastUpdateDate, LastUpdateTime = clsFVch.LastUpdateTime, TenantID = clsFVch.TenantID });
                    }
                }

                    //if (strFailedSettings != "")
                        //MessageBox.Show("These settings failed to save. " + Environment.NewLine + strFailedSettings);

                return JsonConvert.SerializeObject(lstFeatures);
            }
            else
            {
                clsJsonVchTypeFeatures clsVchFeatDz = new clsJsonVchTypeFeatures();
                List<clsJsonVchTypeFeatures> lst = new List<clsJsonVchTypeFeatures>();
                lst = JsonConvert.DeserializeObject<List<clsJsonVchTypeFeatures>>(sToDeSerialize);
                dtGetFromVchFeature = lst.ToDataTable();
                return "Success";
            }
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return "Failed";
            }
        }

        private bool SaveGeneralSettings(clsJsonVchTypeFeatures clsFVch)
        {
            try
            {
                sqlControl rs = new sqlControl();

                rs.Execute("Update tblvchtypeGenSettings set BlnEnabled=" + clsFVch.BlnEnabled.ToString() + " where SettingsName='" + clsFVch.SettingsName.ToString().Replace("'", "''") + "' and VchTypeID=" + clsFVch.VchTypeID.ToString() + " and userid=" + clsFVch.UserID.ToString() + " and tenantid=" + Global.gblTenantID.ToString());
                if (rs.RecordCount <= 0)
                {
                    rs.Execute("Insert Into tblvchtypeGenSettings ([VchTypeID],[SettingsName],[SettingsDescription],[BlnEnabled],[UserID],[SystemName],[UserID1],[LastUpdateDate],[LastUpdateTime],[TenantID]) VALUES (" + clsFVch.VchTypeID.ToString() + ",'" + clsFVch.SettingsName.ToString().Replace("'", "''") + "','" + clsFVch.SettingsDescription.ToString().Replace("'", "''") + "'," + clsFVch.BlnEnabled.ToString() + "," + clsFVch.UserID.ToString() + ",'" + clsFVch.SystemName.ToString().Replace("'","''") + "'," + clsFVch.UserID1 + ",'" + clsFVch.LastUpdateDate + "','" + clsFVch.LastUpdateTime + "'," + clsFVch.TenantID.ToString() + ")");
                        //  and tenantid=" + Global.gblTenantID.ToString());
                }

                if (rs.RecordCount <= 0)
                    return false;
                else
                    return true;
            }
            catch(Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                return false;
            }
        }

        //Description : Serialize and Deserialize All Vouycher Data Except Features Details for Save Data through Json
        private string JsonSerialLizeAndDeserializeObject(bool bIsSerialize = true, string sToDeSerialize = "", bool blnInherit = false)
        {
            try
            {
                clsJsonVoucherType clsVch = new clsJsonVoucherType();
                if (bIsSerialize == true)
                {
                    clsVch.TransactionName = txtTransName.Text;
                    clsVch.ParentID = Convert.ToDecimal(cboParentTrans.SelectedValue);
                    clsVch.TransactionNumberingValue = Convert.ToDecimal(cboTransNumbering.SelectedIndex);
                    clsVch.TransactionPrefix = txtTransPrefix.Text;
                    clsVch.ReferenceNumberingValue = Convert.ToDecimal(cboRefNumbering.SelectedIndex);
                    clsVch.ReferencePrefix = txtRefPrefix.Text;
                    clsVch.TransactinSortOrder = Convert.ToDecimal(txtSortOrder.Text);
                    if (string.IsNullOrEmpty(txtCusrsorNavList.Text))
                        lblTransIds.Text = "";
                    clsVch.CursorNavigationOrderList = lblTransIds.Text;
                    clsVch.PrimaryCCValue = Convert.ToDecimal(cboDefaultCostCenterPrimary.SelectedValue);

                    if (tbtnPrimaryCCenter.ToggleState == Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active)
                        clsVch.blnPrimaryLockWithSelection = 1;
                    else
                        clsVch.blnPrimaryLockWithSelection = 0;

                    if (togglebtnActive.ToggleState == Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active)
                        clsVch.ActiveStatus = 1;
                    else
                        clsVch.ActiveStatus = 0;

                    clsVch.SecondaryCCValue = Convert.ToDecimal(cboDefaultCostCenterSecondary.SelectedValue);

                    if (tbtnSecondaryCCenter.ToggleState == Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active)
                        clsVch.blnSecondaryLockWithSelection = 1;
                    else
                        clsVch.blnSecondaryLockWithSelection = 0;

                    //SearchMethod

                    clsVch.DefaultSearchMethodValue = Convert.ToDecimal(cboDefaultSearchMethod.SelectedValue);
                    if (tbtnSpaceforRateSearch.ToggleState == Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active)
                        clsVch.blnUseSpaceforRateSearch = 1;
                    else
                        clsVch.blnUseSpaceforRateSearch = 0;

                    if (tbtnItmSearchbydefault.ToggleState == Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active)
                        clsVch.btnShowItmSearchByDefault = 1;
                    else
                        clsVch.btnShowItmSearchByDefault = 0;

                    if (tbtnMovetonextafterselection.ToggleState == Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active)
                        clsVch.blnMovetoNextRowAfterSelection = 1;
                    else
                        clsVch.blnMovetoNextRowAfterSelection = 0;

                    if (tbtnMMRPHideNeg.ToggleState == Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active)
                        clsVch.blnHideNegativeorExpiredItmsfromMRRPSubWindow = 1;
                    else
                        clsVch.blnHideNegativeorExpiredItmsfromMRRPSubWindow = 0;

                    clsVch.MMRPSubWindowsSortModeValue = Convert.ToDecimal(cboMMRPSubWindowSearchMod.SelectedValue);

                    if (tbtnShowSearchWindowByDefault.ToggleState == Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active)
                        clsVch.blnShowSearchWindowByDefault = 1;
                    else
                        clsVch.blnShowSearchWindowByDefault = 0;


                    //Discount

                    if (rdoBillWisePercWise.Checked == true)
                        clsVch.blnBillWiseDiscPercentage = 1;
                    else
                        clsVch.blnBillWiseDiscPercentage = 0;

                    if (rdoBillWiseAmt.Checked == true)
                        clsVch.btnBillWiseDiscAmount = 1;
                    else
                        clsVch.btnBillWiseDiscAmount = 0;

                    if (rdoBillWisePercAmt.Checked == true)
                        clsVch.blnBillWiseDiscPercentageandAmt = 1;
                    else
                        clsVch.blnBillWiseDiscPercentageandAmt = 0;

                    clsVch.BillWiseDiscFillXtraDiscFromValue = Convert.ToDecimal(cboBillWiseXtraDisc.SelectedValue);
                    if (rdoItmWisePercWise.Checked == true)
                        clsVch.blnItmWiseDiscPercentage = 1;
                    else
                        clsVch.blnItmWiseDiscPercentage = 0;

                    if (rdoItmWiseAmt.Checked == true)
                        clsVch.blnItmWiseDiscAmount = 1;
                    else
                        clsVch.blnItmWiseDiscAmount = 0;

                    if (rdoItmWisePercAmt.Checked == true)
                        clsVch.blnItmWiseDiscPercentageandAmt = 1;
                    else
                        clsVch.blnItmWiseDiscPercentageandAmt = 0;

                    clsVch.ItmWiseDiscFillXtraDiscFromValue = Convert.ToDecimal(cboItmWiseXtraDisc.SelectedValue);
                    clsVch.RoundOffMode = Convert.ToInt32(cboRoundoff.SelectedIndex);
                    clsVch.RoundOffBlock = Convert.ToDouble(txtRoundOffBlock.Text.ToString());
                    if (tbtnRateDisc.ToggleState == Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active)
                        clsVch.blnRateDiscount = 1;
                    else
                        clsVch.blnRateDiscount = 0;


                    //Defaults

                    clsVch.DefaultTaxModeValue = Convert.ToDecimal(cboDefaultTaxMode.SelectedValue);
                    if (tbtnTaxMode.ToggleState == Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active)
                        clsVch.blnTaxModeLockWSel = 1;
                    else
                        clsVch.blnTaxModeLockWSel = 0;

                    clsVch.DefaultModeofPaymentValue = Convert.ToDecimal(cboDefaultModofPay.SelectedValue);
                    if (tbtnMOP.ToggleState == Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active)
                        clsVch.blnModeofPaymentLockWSel = 1;
                    else
                        clsVch.blnModeofPaymentLockWSel = 0;
                    if (tbtnMOP.ToggleState == Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active)
                        clsVch.blnModeofPaymentLockWSel = 1;
                    else
                        clsVch.blnModeofPaymentLockWSel = 0;

                    clsVch.DefaultPriceList = Convert.ToDecimal(cboDefaultPriceList.SelectedValue);
                    if (tbtnPriceList.ToggleState == Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active)
                        clsVch.blnPriceListLockWSel = 1;
                    else
                        clsVch.blnPriceListLockWSel = 0;
                    if (tbtnPriceList.ToggleState == Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active)
                        clsVch.blnPriceListLockWSel = 1;
                    else
                        clsVch.blnPriceListLockWSel = 0;

                    clsVch.DefaultSaleStaffValue = Convert.ToDecimal(cboDefaultSalesStaff.SelectedValue);
                    if (tbtnSalesStaff.ToggleState == Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active)
                        clsVch.blnSaleStaffLockWSel = 1;
                    else
                        clsVch.blnSaleStaffLockWSel = 0;

                    clsVch.DefaultAgentValue = Convert.ToDecimal(cboDefaultAgent.SelectedValue);
                    if (tbtnAgent.ToggleState == Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active)
                        clsVch.blnAgentLockWSel = 1;
                    else
                        clsVch.blnAgentLockWSel = 0;

                    clsVch.DefaultTaxInclusiveValue = Convert.ToDecimal(cboDefaultTaxInclusive.SelectedValue);
                    if (tbtnTaxInclusive.ToggleState == Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active)
                        clsVch.blnTaxInclusiveLockWSel = 1;
                    else
                        clsVch.blnTaxInclusiveLockWSel = 0;

                    clsVch.DefaultBarcodeMode = Convert.ToDecimal(cboDefaultBarcodeMode.SelectedValue);

                    //Filters
                    if (string.IsNullOrEmpty(txtFltItemType.Text))
                        lblItmTypeIds.Text = "";
                    clsVch.ProductClassList = lblItmTypeIds.Text;
                    if (string.IsNullOrEmpty(txtFltCategories.Text))
                        lblCategoryIds.Text = "";
                    clsVch.ItemCategoriesList = lblCategoryIds.Text;
                    if (string.IsNullOrEmpty(txtFltCustGrp.Text))
                        lblCustIds.Text = "";
                    clsVch.CustomerSupplierAccGroupList = lblCustIds.Text;
                    if (string.IsNullOrEmpty(txtFltDrAccGrp.Text))
                        lblDrAccGrpIds.Text = "";
                    clsVch.DebitAccGroupList = lblDrAccGrpIds.Text;
                    if (string.IsNullOrEmpty(txtFltCrAccGrp.Text))
                        lblCrAccGrpIds.Text = "";
                    clsVch.CreditAccGroupList = lblCrAccGrpIds.Text;

                    clsVch.PrintSettings = GetSetPrintsettingsString();

                    if (cboExportType.SelectedIndex < 0) cboExportType.SelectedIndex = 0;
                    clsVch.BoardRateExportType = cboExportType.SelectedIndex; //export type
                    if (txtQuery.Text.Trim() == "")
                        txtQuery.Text = "SELECT TOP (1000) [ItemID] ,[ItemCode] ,[ItemName] ,[BatchUnique] ,[PLUNumber] ,[MRP] ,[SRATE1] ,[SRATE2] ,[SRATE3] ,[SRATE4] ,[SRATE5] ,[Unit] ,[Expiry]   FROM  [vwBoardRatePLU] ";
                    clsVch.BoardRateQuery = txtQuery.Text;

                    clsVch.BoardRateFileName = txtFileName.Text;

                    return JsonConvert.SerializeObject(clsVch);
                }
                else
                {
                    clsJsonVoucherType clsVchDSer = JsonConvert.DeserializeObject<clsJsonVoucherType>(sToDeSerialize);

                    if (clsVchDSer == null) clsVchDSer = new clsJsonVoucherType();
                    //Settings

                    if (blnInherit == false)
                    {
                        txtTransName.Text = clsVchDSer.TransactionName;
                        cboParentTrans.SelectedValue = clsVchDSer.ParentID;
                    }

                    SelectParentVchtype();

                    FillBarcodeMode();

                    if (Convert.ToInt32(clsVchDSer.ActiveStatus) == 1)
                        togglebtnActive.ToggleState = Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active;
                    else
                        togglebtnActive.ToggleState = Syncfusion.Windows.Forms.Tools.ToggleButtonState.Inactive;

                    cboTransNumbering.SelectedIndex = Convert.ToInt32(clsVchDSer.TransactionNumberingValue);
                    txtTransPrefix.Text = clsVchDSer.TransactionPrefix;
                    cboRefNumbering.SelectedIndex = Convert.ToInt32(clsVchDSer.ReferenceNumberingValue);
                    txtRefPrefix.Text = clsVchDSer.ReferencePrefix;
                    txtSortOrder.Text = clsVchDSer.TransactinSortOrder.ToString();
                    lblTransIds.Text = clsVchDSer.CursorNavigationOrderList;
                    txtCusrsorNavList.Text = GetTransAsperIDs(lblTransIds.Text);
                    cboDefaultCostCenterPrimary.SelectedValue = clsVchDSer.PrimaryCCValue;
                    if (clsVchDSer.blnPrimaryLockWithSelection == 1)
                        tbtnPrimaryCCenter.ToggleState = Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active;
                    else
                        tbtnPrimaryCCenter.ToggleState = Syncfusion.Windows.Forms.Tools.ToggleButtonState.Inactive;
                    cboDefaultCostCenterSecondary.SelectedValue = clsVchDSer.SecondaryCCValue;
                    if (clsVchDSer.blnSecondaryLockWithSelection == 1)
                        tbtnSecondaryCCenter.ToggleState = Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active;
                    else
                        tbtnSecondaryCCenter.ToggleState = Syncfusion.Windows.Forms.Tools.ToggleButtonState.Inactive;

                    //SearchMethod

                    cboDefaultSearchMethod.SelectedValue = Convert.ToInt32(clsVchDSer.DefaultSearchMethodValue);
                    if (clsVchDSer.blnUseSpaceforRateSearch == 1)
                        tbtnSpaceforRateSearch.ToggleState = Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active;
                    else
                        tbtnSpaceforRateSearch.ToggleState = Syncfusion.Windows.Forms.Tools.ToggleButtonState.Inactive;
                    if (clsVchDSer.btnShowItmSearchByDefault == 1)
                        tbtnItmSearchbydefault.ToggleState = Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active;
                    else
                        tbtnItmSearchbydefault.ToggleState = Syncfusion.Windows.Forms.Tools.ToggleButtonState.Inactive;
                    if (clsVchDSer.blnMovetoNextRowAfterSelection == 1)
                        tbtnMovetonextafterselection.ToggleState = Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active;
                    else
                        tbtnMovetonextafterselection.ToggleState = Syncfusion.Windows.Forms.Tools.ToggleButtonState.Inactive;
                    if (clsVchDSer.blnHideNegativeorExpiredItmsfromMRRPSubWindow == 1)
                        tbtnMMRPHideNeg.ToggleState = Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active;
                    else
                        tbtnMMRPHideNeg.ToggleState = Syncfusion.Windows.Forms.Tools.ToggleButtonState.Inactive;
                    cboMMRPSubWindowSearchMod.SelectedValue = Convert.ToInt32(clsVchDSer.MMRPSubWindowsSortModeValue);
                    if (clsVchDSer.blnShowSearchWindowByDefault == 1)
                        tbtnShowSearchWindowByDefault.ToggleState = Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active;
                    else
                        tbtnShowSearchWindowByDefault.ToggleState = Syncfusion.Windows.Forms.Tools.ToggleButtonState.Inactive;

                    //Discount

                    if (clsVchDSer.blnBillWiseDiscPercentage == 1)
                        rdoBillWisePercWise.Checked = true;
                    else
                        rdoBillWisePercWise.Checked = false;
                    if (clsVchDSer.btnBillWiseDiscAmount == 1)
                        rdoBillWiseAmt.Checked = true;
                    else
                        rdoBillWiseAmt.Checked = false;
                    if (clsVchDSer.blnBillWiseDiscPercentageandAmt == 1)
                        rdoBillWisePercAmt.Checked = true;
                    else
                        rdoBillWisePercAmt.Checked = false;
                    cboBillWiseXtraDisc.SelectedValue = Convert.ToInt32(clsVchDSer.BillWiseDiscFillXtraDiscFromValue);
                    if (clsVchDSer.blnItmWiseDiscPercentage == 1)
                        rdoItmWisePercWise.Checked = true;
                    else
                        rdoItmWisePercWise.Checked = false;
                    if (clsVchDSer.blnItmWiseDiscAmount == 1)
                        rdoItmWiseAmt.Checked = true;
                    else
                        rdoItmWiseAmt.Checked = false;
                    if (clsVchDSer.blnItmWiseDiscPercentageandAmt == 1)
                        rdoItmWisePercAmt.Checked = true;
                    else
                        rdoItmWisePercAmt.Checked = false;
                    cboItmWiseXtraDisc.SelectedValue = Convert.ToInt32(clsVchDSer.ItmWiseDiscFillXtraDiscFromValue);
                    cboRoundoff.SelectedIndex = Convert.ToInt32(clsVchDSer.RoundOffMode);
                    txtRoundOffBlock.Text = clsVchDSer.RoundOffBlock.ToString();
                    if (clsVchDSer.blnRateDiscount == 1)
                        tbtnRateDisc.ToggleState = Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active;
                    else
                        tbtnRateDisc.ToggleState = Syncfusion.Windows.Forms.Tools.ToggleButtonState.Inactive;

                    //Defaults

                    cboDefaultTaxMode.SelectedValue = clsVchDSer.DefaultTaxModeValue;
                    if (clsVchDSer.blnTaxModeLockWSel == 1)
                        tbtnTaxMode.ToggleState = Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active;
                    else
                        tbtnTaxMode.ToggleState = Syncfusion.Windows.Forms.Tools.ToggleButtonState.Inactive;

                    cboDefaultModofPay.SelectedValue = Convert.ToInt32(clsVchDSer.DefaultModeofPaymentValue);
                    if (clsVchDSer.blnModeofPaymentLockWSel == 1)
                        tbtnMOP.ToggleState = Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active;
                    else
                        tbtnMOP.ToggleState = Syncfusion.Windows.Forms.Tools.ToggleButtonState.Inactive;

                    cboDefaultModofPay.SelectedValue = Convert.ToInt32(clsVchDSer.DefaultPriceList);
                    if (clsVchDSer.blnPriceListLockWSel == 1)
                        tbtnPriceList.ToggleState = Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active;
                    else
                        tbtnPriceList.ToggleState = Syncfusion.Windows.Forms.Tools.ToggleButtonState.Inactive;

                    cboDefaultSalesStaff.SelectedValue = clsVchDSer.DefaultSaleStaffValue;
                    if (clsVchDSer.blnSaleStaffLockWSel == 1)
                        tbtnSalesStaff.ToggleState = Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active;
                    else
                        tbtnSalesStaff.ToggleState = Syncfusion.Windows.Forms.Tools.ToggleButtonState.Inactive;
                    cboDefaultAgent.SelectedValue = clsVchDSer.DefaultAgentValue;
                    if (clsVchDSer.blnAgentLockWSel == 1)
                        tbtnAgent.ToggleState = Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active;
                    else
                        tbtnAgent.ToggleState = Syncfusion.Windows.Forms.Tools.ToggleButtonState.Inactive;

                    cboDefaultBarcodeMode.SelectedValue = Convert.ToInt32(clsVchDSer.DefaultBarcodeMode);

                    cboDefaultTaxInclusive.SelectedValue = Convert.ToInt32(clsVchDSer.DefaultTaxInclusiveValue);
                    if (clsVchDSer.blnTaxInclusiveLockWSel == 1)
                        tbtnTaxInclusive.ToggleState = Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active;
                    else
                        tbtnTaxInclusive.ToggleState = Syncfusion.Windows.Forms.Tools.ToggleButtonState.Inactive;

                    //Filters
                    lblItmTypeIds.Text = clsVchDSer.ProductClassList;
                    lblCategoryIds.Text = clsVchDSer.ItemCategoriesList;
                    lblCustIds.Text = clsVchDSer.CustomerSupplierAccGroupList;
                    lblDrAccGrpIds.Text = clsVchDSer.DebitAccGroupList;
                    lblCrAccGrpIds.Text = clsVchDSer.CreditAccGroupList;

                    FillCheckedCompactDataToTextBox();
                    txtFltCategories.Text = strFillCatClass;
                    txtFltItemType.Text = strFillProdClass;
                    txtFltCustGrp.Text = strFillCustomer;
                    txtFltDrAccGrp.Text = strFillDebitAcc;
                    txtFltCrAccGrp.Text = strFillCreditAcc;

                    GetSetPrintsettingsString(true, clsVchDSer.PrintSettings);

                    cboExportType.SelectedIndex = clsVchDSer.BoardRateExportType; //export type
                    if (cboExportType.SelectedIndex < 0) cboExportType.SelectedIndex = 0;
                    txtQuery.Text = clsVchDSer.BoardRateQuery;
                    txtFileName.Text = clsVchDSer.BoardRateFileName;

                    //if (Comm.ToInt32(cboParentTrans.SelectedValue.ToString()) == 40)
                    //{
                    //    if (txtQuery.Text.ToString() == "")
                    //    {
                    //        txtQuery.Text = clsVch.BoardRateQuery;
                    //    }
                    //    if (txtFileName.Text.ToString() == "")
                    //    {
                    //        txtFileName.Text = "jhma.csv";
                    //        txtFileName.Text = @"C:\DIGIDATA\" + txtFileName.Text;
                    //    }
                    //    if (cboExportType.SelectedIndex < 0)
                    //    {
                    //        cboExportType.SelectedIndex = 0;
                    //    }
                    //}

                    return "Success";
                }
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return "Failed";

            }
        }

        private void rdoFeatures_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void rdoDiscount_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void cboItmWiseXtraDisc_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void cboRoundOff_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(cboRoundoff, true, false);
        }

        private void cboRoundOff_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(cboRoundoff, false, false);
        }

        private void txtRoundOffBlock_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtRoundOffBlock, true, false);
        }

        private void txtRoundOffBlock_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtRoundOffBlock, false, false);
        }

        private void cboRoundoff_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            { 
            if (cboRoundoff.SelectedIndex > 0 && cboRoundoff.SelectedIndex < 40)
                txtRoundOffBlock.Text = "1";
            else
                txtRoundOffBlock.Text = "0";
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
        }



        //Description : Set Checked Data to Text box
        private void FillCheckedCompactDataToTextBox()
        {
            try
            { 
            strFillCatClass = Comm.fnGetData("EXEC UspGetCheckedList '" + lblCategoryIds.Text + "'," + Global.gblTenantID + ",'CATEGORY'").Tables[0].Rows[0][0].ToString();
                strFillProdClass = GetProductClsAsperIDs(lblItmTypeIds.Text); //Comm.fnGetData("EXEC UspGetCheckedList '" + lblItmTypeIds.Text + "'," + Global.gblTenantID + ",'PRODCLASS'").Tables[0].Rows[0][0].ToString();
            strFillCustomer = Comm.fnGetData("EXEC UspGetLedgerForCheckedList '" + lblCustIds.Text + "'," + Global.gblTenantID + ",'SUPPLIER'").Tables[0].Rows[0][0].ToString();
            strFillDebitAcc = Comm.fnGetData("EXEC UspGetLedgerForCheckedList '" + lblDrAccGrpIds.Text + "'," + Global.gblTenantID + ",'DR-LEDGER'").Tables[0].Rows[0][0].ToString();
            strFillCreditAcc = Comm.fnGetData("EXEC UspGetLedgerForCheckedList '" + lblCrAccGrpIds.Text + "'," + Global.gblTenantID + ",'CR-LEDGER'").Tables[0].Rows[0][0].ToString();
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
        }

        private void cboDefaultModofPay_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void rdoSettings_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void txtFltItemType_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtTransName_TextChanged(object sender, EventArgs e)
        {

        }

        private void togglebtnActive_Click(object sender, EventArgs e)
        {

        }

        private void btnAddPrintSettings_Click(object sender, EventArgs e)
        {
            try
            {
                int rowno = -1;
                for (int i = 0; i < dgvPrintSettings.Rows.Count; i++)
                {
                    if (dgvPrintSettings.Rows[i].Cells[1].Value == null) dgvPrintSettings.Rows[i].Cells[1].Value = "";
                    if (dgvPrintSettings.Rows[i].Cells[1].Value.ToString() == cboInvScheme1.Text)
                    {
                        rowno = i;
                    }
                }

                if (rowno == -1)
                {
                    dgvPrintSettings.Rows.Add();
                    rowno = dgvPrintSettings.Rows.Count - 1;
                }

                dgvPrintSettings.Rows[rowno].Cells[0].Value = rowno;
                dgvPrintSettings.Rows[rowno].Cells[1].Value = cboInvScheme1.Text.ToString();
                dgvPrintSettings.Rows[rowno].Cells[2].Value = txtPageWidth.Text.ToString();
                dgvPrintSettings.Rows[rowno].Cells[3].Value = txtHeaderHeight.Text.ToString();
                dgvPrintSettings.Rows[rowno].Cells[4].Value = txtItemHeight.Text.ToString();
                dgvPrintSettings.Rows[rowno].Cells[5].Value = txtFooterHeight.Text.ToString();
            }
            catch
            { }
        }

        private void rdoPrint_Click(object sender, EventArgs e)
        {
            ShowFormsAsperClick(7);
            cboInvScheme1.Focus();
        }

        private void rdoPrint_CheckedChanged(object sender, EventArgs e)
        {

        }

        //Description : Save and Update Features Functionalities to the Database
        private void SaveFeaturesData(decimal dVchType = 0)
        {
            try
            { 
            Hashtable hstFeatures = new Hashtable();
            for (int g = 2; g < dgvFeatures.Columns.Count; g++)
            {
                for (int h = 0; h < dgvFeatures.Rows.Count; h++)
                {
                    hstFeatures["VchTypeID"] = dVchType;
                    hstFeatures["SettingsName"] = dgvFeatures.Rows[h].Cells[0].Value.ToString().Trim().ToUpper();
                    hstFeatures["SettingsDescription"] = dgvFeatures.Rows[h].Cells[1].Value.ToString();
                    if (Convert.ToBoolean(dgvFeatures.Rows[h].Cells[g].Value) == false)
                        hstFeatures["BlnEnabled"] = 0;
                    else
                        hstFeatures["BlnEnabled"] = 1;
                    hstFeatures["UserID"] = Convert.ToDecimal(dgvFeatures.Columns[g].Tag.ToString());
                    hstFeatures["SystemName"] = Global.gblSystemName;
                    hstFeatures["UserID1"] = Global.gblUserID;
                    hstFeatures["LastUpdateDate"] = DateTime.Today.ToString("dd-MMM-yyyy");
                    hstFeatures["LastUpdateTime"] = DateTime.Now.ToString("dd-MMM-yyyy hh:mm tt");
                    hstFeatures["TenantID"] = Global.gblTenantID;

                    DataTable dt = Comm.fnGetData("EXEC UspvchtypeGenSettingsInsert " + hstFeatures["VchTypeID"].ToString() + ", '" + hstFeatures["SettingsName"].ToString() + "', '" + hstFeatures["SettingsDescription"].ToString() + "'," + hstFeatures["BlnEnabled"].ToString() + "," + hstFeatures["UserID"].ToString() + ",'" + hstFeatures["SystemName"].ToString() + "'," + hstFeatures["UserID1"].ToString() + ",'" + hstFeatures["LastUpdateDate"].ToString() + "','" + hstFeatures["LastUpdateTime"].ToString() + "'," + hstFeatures["TenantID"].ToString() + ",0").Tables[0];
                }
            }
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult fname = new DialogResult();
                
                saveFileDialog1.InitialDirectory = @"C:\";
                saveFileDialog1.RestoreDirectory = true;

                if (cboExportType.SelectedIndex < 0) cboExportType.SelectedIndex = 0;
                if (cboExportType.SelectedIndex == 0) 
                    saveFileDialog1.DefaultExt = "csv";
                else
                    saveFileDialog1.DefaultExt = "txt";

                saveFileDialog1.Filter = "csv files (*.csv)|*.csv|txt files (*.txt)|*.txt|All files (*.*)|*.*";
                saveFileDialog1.FilterIndex = 1;
                saveFileDialog1.CheckPathExists = true;

                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    txtFileName.Text = saveFileDialog1.FileName;
                }

            }
            catch
            { }
        }

        private void btnRestore_Click(object sender, EventArgs e)
        {
            try
            {
                if (Comm.ToInt32(cboParentTrans.SelectedValue.ToString()) == 40)
                {
                    if (txtQuery.Text.ToString() == "")
                    {
                        txtQuery.Text = "SELECT TOP (1000) [ItemID] ,[ItemCode] ,[ItemName] ,[BatchUnique] ,[PLUNumber] ,[MRP] ,[SRATE1] ,[SRATE2] ,[SRATE3] ,[SRATE4] ,[SRATE5] ,[Unit] ,[Expiry]   FROM  [vwBoardRatePLU] ";
                    }
                    if (txtFileName.Text.ToString() == "")
                    {
                        txtFileName.Text = "jhma.csv";
                        txtFileName.Text = @"C:\DIGIDATA\" + txtFileName.Text;
                    }
                    if (cboExportType.SelectedIndex < 0)
                    {
                        cboExportType.SelectedIndex = 0;
                    }
                }
            }
            catch
            { }
        }

        private void btnInherit_Click(object sender, EventArgs e)
        {
            try
            {
                if (cboParentTrans.SelectedIndex >= 0)
                {
                    btnInherit.Text = "Inheriting...";
                    LoadData(Comm.ToDecimal(cboParentTrans.SelectedValue), true);
                    btnInherit.Text = "Inherit";
                    MessageBox.Show("Successfully inherited all properties of parent vouchertype " + cboParentTrans.SelectedText, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                    MessageBox.Show("Please select the parent vouchertype before inheriting.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch 
            { }
        }

        private string GetSetPrintsettingsString(bool SetPrintToGrid = false, string strPrintSetting = "")
        {
            try
            {
                string Printsettings = "";

                if (SetPrintToGrid == false)
                {
                    for (int i = 0; i < dgvPrintSettings.Rows.Count; i++)
                    {
                        if (dgvPrintSettings.Rows[i].Cells[1].Value == null) dgvPrintSettings.Rows[i].Cells[1].Value = "";
                        if (dgvPrintSettings.Rows[i].Cells[1].Value.ToString() != "")
                        {
                            Printsettings += dgvPrintSettings.Rows[i].Cells[0].Value.ToString() + "||" + dgvPrintSettings.Rows[i].Cells[1].Value.ToString() + "||" + dgvPrintSettings.Rows[i].Cells[2].Value.ToString() + "||" + dgvPrintSettings.Rows[i].Cells[3].Value.ToString() + "||" + dgvPrintSettings.Rows[i].Cells[4].Value.ToString() + "||" + dgvPrintSettings.Rows[i].Cells[5].Value.ToString() + ";;";
                        }
                    }
                }
                else
                {
                    String[] separator1 = { ";;" };
                    String[] separator2 = { "||" };
                    string[] strSplit = strPrintSetting.Split(separator1, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < strSplit.Length; i++)
                    {
                        if (strSplit[i] != null)
                        {
                            dgvPrintSettings.Rows.Add();

                            string[] strSplit2 = new string[6];
                            strSplit2 = strSplit[i].Split(separator2, StringSplitOptions.None);

                            dgvPrintSettings.Rows[i].Cells[0].Value = strSplit2[0];
                            dgvPrintSettings.Rows[i].Cells[1].Value = strSplit2[1];
                            dgvPrintSettings.Rows[i].Cells[2].Value = strSplit2[2];
                            dgvPrintSettings.Rows[i].Cells[3].Value = strSplit2[3];
                            dgvPrintSettings.Rows[i].Cells[4].Value = strSplit2[4];
                            dgvPrintSettings.Rows[i].Cells[5].Value = strSplit2[5];
                        }
                    }
                }
                return Printsettings;
            }
            catch(Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return "";
            }
        }

        //Description : Save and Update Functionalities to the Database
        private decimal SaveData()
        {
            try
            { 
            DataTable dtUspVc = new DataTable();
            decimal dvchTyid = 0;

            int iActive = 0;
            if (togglebtnActive.ToggleState == Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active)
                iActive = 1;

            int iAction = 0;
            if (txtSortOrder.Text == "")
                txtSortOrder.Text = "1";

                if (dIDFromEditWindow == 0)
            {
                dvchTyid = Comm.gfnGetNextSerialNo("tblVchType", "VchTypeID", "TenantID = " + Global.gblTenantID + "");
            }
            else
            {
                dvchTyid = dIDFromEditWindow;
                iAction = 1;
                DeleteGeneralSettings(dvchTyid);
            }
            infoVchTyp.VchTypeID = dvchTyid;
            infoVchTyp.VchType = txtTransName.Text;
            infoVchTyp.ShortKey = "";
            infoVchTyp.EasyKey = "";
            infoVchTyp.SortOrder = Convert.ToDecimal(txtSortOrder.Text);
            infoVchTyp.ActiveStatus = iActive;
            infoVchTyp.ParentID = Convert.ToDecimal(cboParentTrans.SelectedValue);
            infoVchTyp.Description = txtTransName.Text;
            infoVchTyp.numberingCode = Convert.ToDecimal(cboTransNumbering.SelectedIndex);
            infoVchTyp.Prefix = txtTransPrefix.Text;
            infoVchTyp.Sufix = "";
            infoVchTyp.ItemClassIDS = lblItmTypeIds.Text;
            infoVchTyp.CreditGroupIDs = lblCrAccGrpIds.Text;
            infoVchTyp.DebitGroupIDs = lblDrAccGrpIds.Text;
            infoVchTyp.ProductTypeIDs = "";
            infoVchTyp.GeneralSettings = "";
            infoVchTyp.NegativeBalance = 0;
            infoVchTyp.RoundOffBlock = Convert.ToDecimal( txtRoundOffBlock.Text.ToString());
            infoVchTyp.RoundOffMode = cboRoundoff.SelectedIndex;
            infoVchTyp.ItemClassIDS2 = "";
            if (cboDefaultCostCenterSecondary.SelectedIndex == -1)
                infoVchTyp.SecondaryCCIDS = "";
            else
                infoVchTyp.SecondaryCCIDS = cboDefaultCostCenterSecondary.SelectedValue.ToString();

            if (cboDefaultCostCenterPrimary.SelectedIndex == -1)
                infoVchTyp.PrimaryCCIDS = "";
            else
                infoVchTyp.PrimaryCCIDS = cboDefaultCostCenterPrimary.SelectedValue.ToString();
            infoVchTyp.OrderVchTypeIDS = "";
            infoVchTyp.NoteVchTypeIDS = "";
            infoVchTyp.QuotationVchTypeIDS = "";
            infoVchTyp.DEFMOPID = Convert.ToInt32(cboDefaultModofPay.SelectedValue);
            if (tbtnMOP.ToggleState == Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active)
                infoVchTyp.BLNLOCKMOP = 1;
            else
                infoVchTyp.BLNLOCKMOP = 0;
            infoVchTyp.DEFPLID = Convert.ToInt32(cboDefaultPriceList.SelectedValue);
            if (tbtnMOP.ToggleState == Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active)
                infoVchTyp.BLNLOCKPLID = 1;
            else
                infoVchTyp.BLNLOCKPLID = 0;
            infoVchTyp.DEFTAXMODEID = Convert.ToInt32(cboDefaultTaxMode.SelectedValue);
            if (tbtnTaxMode.ToggleState == Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active)
                infoVchTyp.BLNLOCKTAXMODE = 1;
            else
                infoVchTyp.BLNLOCKTAXMODE = 1;
            infoVchTyp.DEFAGENTID = Convert.ToInt32(cboDefaultAgent.SelectedValue);
            if (tbtnAgent.ToggleState == Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active)
                infoVchTyp.BLNLOCKAGENT = 1;
            else
                infoVchTyp.BLNLOCKAGENT = 0;

            infoVchTyp.DEFTAXINCLUSIVEID = Convert.ToInt32(cboDefaultTaxInclusive.SelectedValue);
            if (tbtnTaxInclusive.ToggleState == Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active)
                infoVchTyp.BLNLOCKTAXINCLUSIVE = 1;
            else
                infoVchTyp.BLNLOCKTAXINCLUSIVE = 0;

            infoVchTyp.DEFPRICELISTID = 0;
            infoVchTyp.BLNLOCKPRICELIST = 0;
            infoVchTyp.DEFSALESMANID = Convert.ToInt32(cboDefaultSalesStaff.SelectedValue);
            if (tbtnSalesStaff.ToggleState == Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active)
                infoVchTyp.BLNLOCKSALESMAN = 1;
            else
                infoVchTyp.BLNLOCKSALESMAN = 1;
            infoVchTyp.DEFPRINTID = 0;
            infoVchTyp.BLNLOCKPRINT = 0;
            infoVchTyp.ColwidthStr = "";
            infoVchTyp.gridColor = "";
            infoVchTyp.DefaultGodownID = 0;
            infoVchTyp.ActCFasCostLedger = 0;
            infoVchTyp.ActCFasCostLedger4 = 0;
            infoVchTyp.gridHeaderColor = "";
            infoVchTyp.BLNUseForClientSync = 0;
            infoVchTyp.rateInclusiveIndex = 0;
            //Commentted For Doubt Clearance -->>
            //infoVchTyp.BlnBillWiseDisc = "";
            //infoVchTyp.BlnItemWisePerDisc = "";
            //infoVchTyp.BlnItemWiseAmtDisc = "";
            infoVchTyp.gridselectedRow = "";
            infoVchTyp.GridHeaderFont = "";
            infoVchTyp.GridBackColor = "";
            infoVchTyp.GridAlternatCellColor = "";
            infoVchTyp.GridCellColor = "";
            infoVchTyp.GridFontColor = "";
            infoVchTyp.Metatag = "";
            infoVchTyp.DefaultCriteria = "";
            infoVchTyp.SearchSql = "";
            infoVchTyp.SmartSearchBehavourMode = 0;
            infoVchTyp.criteriaconfig = "";
            infoVchTyp.intEnterKeyBehavourMode = 0;
            //infoVchTyp.BlnBillDiscAmtEntry = "";
            if (tbtnRateDisc.ToggleState == Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active)
                infoVchTyp.blnRateDiscount = 1;
            else
                infoVchTyp.blnRateDiscount = 0;
            infoVchTyp.IntdefaultFocusColumnID = 0;
            infoVchTyp.BlnTouchScreen = 0;
            infoVchTyp.StrTouchSetting = "";
            infoVchTyp.StrCalculationFields = "";
            infoVchTyp.CRateCalMethod = 0;
            infoVchTyp.MMRPSortOrder = Convert.ToDecimal(cboMMRPSubWindowSearchMod.SelectedValue);
            infoVchTyp.ItemDiscountFrom = 0;
            infoVchTyp.DEFPRINTID2 = 0;
            infoVchTyp.BLNLOCKPRINT2 = 0;
            infoVchTyp.BillDiscountFrom = 0;
            infoVchTyp.WindowBackColor = "";
            infoVchTyp.ContrastBackColor = "";
            infoVchTyp.BlnEnableCustomFormColor = 0;
            infoVchTyp.returnVchtypeID = 0;
            infoVchTyp.PrintCopies = 0;
            infoVchTyp.SystemName = "";
            infoVchTyp.UserID = 1;
            infoVchTyp.LastUpdateDate = DateTime.Today;
            infoVchTyp.LastUpdateTime = DateTime.Now;
            infoVchTyp.BlnMobileVoucher = 0;
            infoVchTyp.SearchSQLSettings = "";
            infoVchTyp.AdvancedSearchSQLEnabled = 0;
            infoVchTyp.TenantID = 1;
            infoVchTyp.PrintSettings = GetSetPrintsettingsString();

                if (cboExportType.SelectedIndex < 0) cboExportType.SelectedIndex = 0;
                infoVchTyp.BoardRateExportType = cboExportType.SelectedIndex; //export type
                if (txtQuery.Text.Trim() == "")
                    txtQuery.Text = "SELECT TOP (1000) [ItemID] ,[ItemCode] ,[ItemName] ,[BatchUnique] ,[PLUNumber] ,[MRP] ,[SRATE1] ,[SRATE2] ,[SRATE3] ,[SRATE4] ,[SRATE5] ,[Unit] ,[Expiry]   FROM  [vwBoardRatePLU] ";
                infoVchTyp.BoardRateQuery = txtQuery.Text;

                infoVchTyp.BoardRateFileName = txtFileName.Text;


                infoVchTyp.VchJson = JsonSerialLizeAndDeserializeObject(true);
            infoVchTyp.FeaturesJson = FeaturesJsonSerializeAndDeserializeObject(true, "", dvchTyid);
            string sResult = clsVouchTyp.InsertUpdateDeleteVchTypeInsert(infoVchTyp, iAction);

            return dvchTyid;
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return -1;
            }
        }
        //Description : Clear Data from Controls
        private void ClearAll()
        {
            try
            { 
            txtTransName.Text = "";
            cboParentTrans.SelectedIndex = -1;
                
                SelectParentVchtype();

                cboTransNumbering.SelectedIndex = -1;
            txtTransPrefix.Text = "";
            cboRefNumbering.SelectedIndex = -1;
            txtRefPrefix.Text = "";
            txtSortOrder.Text = "";
            lblTransIds.Text = "";
            cboDefaultCostCenterPrimary.SelectedIndex = -1;
            tbtnPrimaryCCenter.ToggleState = Syncfusion.Windows.Forms.Tools.ToggleButtonState.Inactive;
            cboDefaultCostCenterSecondary.SelectedIndex = -1;
            tbtnSecondaryCCenter.ToggleState = Syncfusion.Windows.Forms.Tools.ToggleButtonState.Inactive;
            //SearchMethod
            cboDefaultSearchMethod.SelectedIndex = -1;
            tbtnSpaceforRateSearch.ToggleState = Syncfusion.Windows.Forms.Tools.ToggleButtonState.Inactive;
            tbtnItmSearchbydefault.ToggleState = Syncfusion.Windows.Forms.Tools.ToggleButtonState.Inactive;
            tbtnMovetonextafterselection.ToggleState = Syncfusion.Windows.Forms.Tools.ToggleButtonState.Inactive;
            tbtnMMRPHideNeg.ToggleState = Syncfusion.Windows.Forms.Tools.ToggleButtonState.Inactive;
            cboMMRPSubWindowSearchMod.SelectedIndex = -1;
            tbtnShowSearchWindowByDefault.ToggleState = Syncfusion.Windows.Forms.Tools.ToggleButtonState.Inactive;
            //Discount
            rdoBillWisePercWise.Checked = false;
            rdoBillWiseAmt.Checked = false;
            rdoBillWisePercAmt.Checked = false;
            cboBillWiseXtraDisc.SelectedIndex = -1;
            rdoItmWisePercWise.Checked = false;
            rdoItmWiseAmt.Checked = false;
            rdoItmWisePercAmt.Checked = false;
            cboItmWiseXtraDisc.SelectedIndex = -1;
            cboRoundoff.SelectedIndex = 0;
            txtRoundOffBlock.Text = "0";
            tbtnRateDisc.ToggleState = Syncfusion.Windows.Forms.Tools.ToggleButtonState.Inactive;
            //Defaults
            cboDefaultTaxMode.SelectedIndex = -1;
            tbtnTaxMode.ToggleState = Syncfusion.Windows.Forms.Tools.ToggleButtonState.Inactive;
            cboDefaultModofPay.SelectedIndex = -1;
            tbtnMOP.ToggleState = Syncfusion.Windows.Forms.Tools.ToggleButtonState.Inactive;
            cboDefaultPriceList.SelectedIndex = -1;
            tbtnPriceList.ToggleState = Syncfusion.Windows.Forms.Tools.ToggleButtonState.Inactive;
            cboDefaultSalesStaff.SelectedIndex = -1;
            tbtnSalesStaff.ToggleState = Syncfusion.Windows.Forms.Tools.ToggleButtonState.Inactive;
            cboDefaultAgent.SelectedIndex = -1;
            tbtnAgent.ToggleState = Syncfusion.Windows.Forms.Tools.ToggleButtonState.Inactive;
            cboDefaultBarcodeMode.SelectedIndex = -1;
            cboDefaultTaxInclusive.SelectedIndex = -1;
            tbtnTaxInclusive.ToggleState = Syncfusion.Windows.Forms.Tools.ToggleButtonState.Inactive;
            //Filters
            lblItmTypeIds.Text = "";
            lblCategoryIds.Text = "";
            lblCustIds.Text = "";
            lblDrAccGrpIds.Text = "";
            lblCrAccGrpIds.Text = "";
            // Features
            dgvFeatures.Rows.Clear();
            dgvFeatures.Columns.Clear();
            // Call Settings
            rdoSettings.Checked = true;
            ShowFormsAsperClick(1);
            txtTransName.Select();
            txtTransName.Focus();
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
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
