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
using Syncfusion.Windows.Forms.Tools;
using Syncfusion.WinForms.Controls;
using DigiposZen.Forms;
using Syncfusion.Windows.Forms;
using System.Collections;
using DigiposZen.JsonClass;
using Newtonsoft.Json;
using System.Runtime.InteropServices;
using Microsoft.VisualBasic;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace DigiposZen
{
    public partial class frmItemMaster : Form //, IMessageFilter
    {
        // ======================================================== >>
        // Description:     Item Creation
        // Developed By:    Dipu Joseph
        // Completed Date & Time: 17-09-2021 12.40 PM
        // Last Edited By:  Anjitha k k
        // Last Edited Date & Time: 02-03-2022 04:00 PM
        // ======================================================== >>

        string strControlLIst = "";

        private System.ComponentModel.BackgroundWorker backgroundWorker1;  

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        public const int WM_LBUTTONDOWN = 0x0201;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        private HashSet<Control> controlsToMove = new HashSet<Control>();

        private Boolean mBLNRATECHECK = true;
        private Boolean mBLNSrateInc = true;
        private Boolean mBLNPrateInc = true;

        private bool mblnOnceActivated = false;
        public frmItemMaster(int iItemID = 0, bool bFromEditWindow = false, string sTransType = "", Control Controlpassed = null, bool blnDisableMinimize = false)
        {
            InitializeComponent();

            //backgroundWorker1 = new System.ComponentModel.BackgroundWorker();

            mblnOnceActivated = false;

            //Application.AddMessageFilter(this);

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
                lblDelete.Font = new Font("Tahoma", 10, FontStyle.Regular, GraphicsUnit.Point);
                lblFind.Font = new Font("Tahoma", 10, FontStyle.Regular, GraphicsUnit.Point);

                lblSave.ForeColor = Color.Black;
                lblDelete.ForeColor = Color.Black;
                lblFind.ForeColor = Color.Black;

                btnSave.Image = global::DigiposZen.Properties.Resources.save240402;
                btnDelete.Image = global::DigiposZen.Properties.Resources.delete340402;
                btnFind.Image = global::DigiposZen.Properties.Resources.find_finalised_3030;
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

            try
            {
                //because the code in activated should not work if its loadinga an item
                //background running should not happen if editing an item

                iIDFromEditWindow = iItemID;
                bFromEditWindowItem = bFromEditWindow;
                ApplicationSettings();
                //if (tlpRack.Visible == false)
                //if (tlpColor.Visible == false)
                //    tlpDisc.Size = new Size(250, 55);
                //else
                //    tlpDisc.Size = new Size(125, 55);
                ShowControlCheckboxList();
                LoadControlCheckboxList();
                SetShowHideValue();

                CtrlPassed = Controlpassed;

                //this.BackColor = Global.gblFormBorderColor;

                cboIGSTPerc.Items.Clear();
                if (AppSettings.AVAILABLETAXPER == null)
                {
                    AppSettings.AVAILABLETAXPER = "0,1,3,5,12,18,28";
                }
                if (AppSettings.AVAILABLETAXPER == "")
                {
                    AppSettings.AVAILABLETAXPER = "0,1,3,5,12,18,28";
                }
                if (AppSettings.AVAILABLETAXPER != "")
                {
                    string[] str = AppSettings.AVAILABLETAXPER.Split(',');
                    bool isNumeric = false;
                    for (int i = 0; i < str.Length - 1; i++)
                    {
                        isNumeric = false;
                        isNumeric = int.TryParse(str[i], out int n);
                        if (isNumeric == true)
                        {
                            cboIGSTPerc.Items.Add(str[i]);
                        }
                    }
                }

                chkSRateIncl.Enabled = AppSettings.BLNSRATEINC;
                chkPRateIncl.Enabled = AppSettings.BLNPRATEINC;

                if (iItemID != 0)
                {
                    mblnOnceActivated = true;
                    
                    LoadUnitMaster();
                    LoadBrand();
                    LoadDiscountGroup();
                    LoadProductClass();
                    LoadBarCodeMode();
                    LoadFromOneTimeMaster(0, "ITMRACK");
                    LoadFromOneTimeMaster(0, "PRODCLASS");
                    //LoadFromOneTimeMaster(0, "PROCLAS");
                    LoadHSNCode();
                    LoadCalPriceDetails();
                    LoadDepartment();

                    LoadData(iItemID);
                    cboBMode.Enabled = false;

                    txtItemName.Focus();
                    txtItemName.Select();
                    txtItemName.SelectionStart = txtItemName.Text.ToString().Length;

                }
                else
                {
                    btnDelete.Enabled = false;
                    if (chkExpiryItem.Checked == true)
                        txtshelflife.Enabled = true;
                    else
                        txtshelflife.Enabled = false;

                    if (iIDFromEditWindow == 0)
                    {
                        if (bFromEditWindowItem == true)
                            btnDelete.Enabled = true;
                        else
                            btnDelete.Enabled = false;

                        LoadTestPanel.Enabled = false;
                        ApplicationSettings();

                        LoadUnitMaster();
                        LoadBrand();
                        LoadDiscountGroup();
                        LoadProductClass();
                        LoadBarCodeMode();
                        LoadFromOneTimeMaster(0, "ITMRACK");
                        LoadFromOneTimeMaster(0, "PRODCLASS");
                        //LoadFromOneTimeMaster(0, "PROCLAS");
                        LoadHSNCode();
                        LoadCalPriceDetails();
                        LoadDepartment();

                        this.tlpMain.ColumnStyles[1].Width = 0;
                        togglebtnActive.ToggleState = ToggleButtonState.Active;
                        ClearFormControls();

                        if (tlpColor.Visible == true)
                        {
                            txtColor.Tag = "";
                            GetFromCheckedListColor("");
                        }
                        else
                        {
                            txtColor.Tag = "";
                            GetFromCheckedListColor("");
                        }
                        if (tlpSize.Visible == true)
                        {
                            txtSize.Tag = "";
                            GetFromCheckedListSize("");
                        }
                        else
                        {
                            txtSize.Tag = 1;
                            GetFromCheckedListSize("1");
                        }
                        txtCategoryList.Tag = "";
                        GetFromCheckedList("");

                        txtItemName.Select();
                        txtItemName.SelectionStart = txtItemName.Text.ToString().Length;

                    }
                    else
                    {
                        txtItemName.Focus();
                        //txtItemName.SelectAll();
                        txtItemName.SelectionStart = txtItemName.Text.ToString().Length;
                    }

                    if (CtrlPassed != null && iIDFromEditWindow == 0)
                    {
                        txtItemName.Text = CtrlPassed.Text.ToString();
                        txtItemCode.Text = CtrlPassed.Text.ToString();
                    }
                }

                if (CtrlPassed != null && iIDFromEditWindow == 0)
                    txtItemCode.Text = CtrlPassed.Text.ToString();

                txtItemName.SelectionStart = txtItemName.Text.ToString().Length;

                if (blnDisableMinimize == true) btnMinimize.Enabled = false;

                if (AppSettings.BLNBARCODE == false)
                {
                    grpBatchDetails.Visible = false;
                }
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        #region "VARIABLES  -------------------------------------------- >>"
        Common Comm = new Common();
        UspGetCategoriesinfo GetCatinfo = new UspGetCategoriesinfo();
        UspGetManufacturerInfo GetManinfo = new UspGetManufacturerInfo();
        UspGetUnitInfo GetUnitinfo = new UspGetUnitInfo();
        UspGetColorInfo GetColorinfo = new UspGetColorInfo();
        UspGetSizeInfo GetSizeinfo = new UspGetSizeInfo();
        UspGetBrandinfo GetBrandinfo = new UspGetBrandinfo();
        UspGetDiscountGroupInfo GetDiscGrpinfo = new UspGetDiscountGroupInfo();
        UspGetOnetimeMasterInfo GetOtminfo = new UspGetOnetimeMasterInfo();
        UspGetItemMasterInfo GetItmMstinfo = new UspGetItemMasterInfo();
        UspGetManufacturerForItemMasterInfo GetItmManfinfo = new UspGetManufacturerForItemMasterInfo();
        UspItemMasterInsertInfo itemInsertInfo = new UspItemMasterInsertInfo();
        UspGetHSNFromItemMasterInfo GetHSNInfo = new UspGetHSNFromItemMasterInfo();
        UspGetDepartmentInfo GetDeptInfo = new UspGetDepartmentInfo();

        clsCategory clsCat = new clsCategory();
        clsManufacturer clsMan = new clsManufacturer();
        clsUnitMaster clsUnit = new clsUnitMaster();
        clsColorMaster clsColor = new clsColorMaster();
        clsSizeMaster clsSize = new clsSizeMaster();
        clsBrandMaster clsBrand = new clsBrandMaster();
        clsDiscountGroup clsDiscGrp = new clsDiscountGroup();
        clsOneTimeMaster clsOtm = new clsOneTimeMaster();
        clsItemMaster clsItmMst = new clsItemMaster();
        clsDepartment clsDept = new clsDepartment();

        DataTable dtCheckBox = new DataTable();
        DataTable dtCheckList = new DataTable();

        bool dragging = false;
        int xOffset = 0;
        int yOffset = 0;
        int iIDFromEditWindow;
        int iAction = 0;
        string strtxtCategory;
        bool bFromEditWindowItem;
        string strCheck, strMRPName;
        Control ctrl;
        decimal dManfDiscPer = 0;
        decimal dCatDiscPer = 0;
        Control CtrlPassed;
        #endregion

        #region "EVENTS ------------------------------------------------ >>"
        //Form Drag
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


        #region "Move focus automatically when Enter ------------------ >>"
        private void textBox_KeyDown(object sender, KeyEventArgs e)
        {
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
        }
        #endregion
        private void textbox_KeyPress(Object sender, KeyPressEventArgs e)
        {
            try
            {
                e.Handled = Comm.CheckNumeric(sender, e);
            }
            catch
            {

            }

            ////Allow Numeric and decimal point only
            //if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            //{
            //    e.Handled = true;
            //}
            //// only allow one decimal point
            //if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            //{
            //    e.Handled = true;
            //}
        }
        private void txtItemName_KeyDown(object sender, KeyEventArgs e)
        {
           if (e.Shift == true && e.KeyCode == Keys.Enter)
           {
                txtItemName.Focus();
           }
           else if (e.KeyCode == Keys.Enter)
           {
                txtItemCode.Focus();
                txtItemCode.SelectAll();
           }
            else if (e.KeyCode == Keys.Down)
            {
                dgvShowItemSearch.Focus();
                ShowItemSearchDetailsinGrid();
            }
        }
        private void txtColor_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Shift == true && e.KeyCode == Keys.Enter)
                {
                    txtRack.Focus();
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    if (txtSize.Visible == true)
                    {
                        txtSize.Focus();
                        SendKeys.Send("{DOWN}");
                    }
                    else if (cboBrand.Visible == true)
                    {
                        cboBrand.Focus();
                        SendKeys.Send("{F4}");
                    }
                    else
                    {
                        cboDiscGroup.Focus();
                        SendKeys.Send("{F4}");
                    }
                }
                else if (e.KeyCode == Keys.Down)
                {
                    string sQuery = "select ColorID,ColorName from tblColor WHERE TenantID = " + Global.gblTenantID;
                    new frmCompactCheckedListSearch(GetFromCheckedListColor, sQuery, "ColorName", txtColor.Location.X + 480, txtColor.Location.Y + 390, 0, 2, txtColor.Text, 0, 0, "order by colorname", "", null, "Color", "frmColorMaster").ShowDialog();
                    //SendKeys.Send("{TAB}");
                    //if (txtSize.Visible == true)
                    //{
                    //    txtSize.Focus();
                    //    SendKeys.Send("{DOWN}");
                    //}
                }
                else if (e.KeyCode == Keys.F3)
                {
                    btnColorNew.PerformClick();
                }
                else if (e.KeyCode == Keys.F4)
                {
                    btnColorEdit.PerformClick();
                }
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void txtSize_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
            {
                if (txtColor.Visible == true)
                    txtColor.Focus();
                else
                    txtRack.Focus();
            }
            else if (e.KeyCode == Keys.Enter)
            {
                if (cboBrand.Visible == true)
                    cboBrand.Focus();
                else
                    cboDiscGroup.Focus();
                SendKeys.Send("{F4}");
            }
            else if (e.KeyCode == Keys.Down)
            {
                //string sQuery = "SELECT 0 as SizeID,'<None>' as SizeNameShort,'<None>' as SizeName FROM tblSize UNION SELECT SizeID,SizeNameShort,SizeName FROM tblSize WHERE TenantID =  " + Global.gblTenantID ;
                string sQuery = "SELECT SizeID,SizeNameShort,SizeName FROM tblSize WHERE TenantID =  " + Global.gblTenantID + " ";
                new frmCompactCheckedListSearch(GetFromCheckedListSize, sQuery, "SizeNameShort", txtSize.Location.X + 600, txtSize.Location.Y + 390, 0, 2, txtSize.Text, 0, 0, "order by SizeNameShort", "", null, "Size", "FrmSizeMaster").ShowDialog();
                //if (sfcbobrand.Visible == true)
                //    sfcbobrand.Focus();
                //else
                //    sfcboDiscGroup.Focus();
                //SendKeys.Send("{F4}");
            }
            else if (e.KeyCode == Keys.F3)
            {
                btnSizeNew.PerformClick();
            }
        }
        private void cboManufacturer_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                cboUnit.Focus();
        }
        private void txtMinRate_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Shift == true && e.KeyCode == Keys.Enter) || e.KeyCode == Keys.Up)
            {
                txtMOQ.Focus();
            }
            else if (e.KeyCode == Keys.Enter)
            {
                txtMaxRate.Focus();
            }
        }
        private void cboHSNCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                cboIGSTPerc.Focus();
                //SendKeys.Send("{F4}");
                //e.Handled = true;

                //cboIGSTPerc.ForeColor = Color.Blue;
                //Application.DoEvents();
                //SendKeys.Send("{F4}");
            }
        }
        private void cboBMode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
            {
                if (txtSRate5.Visible == true)
                    txtSRate5.Focus();
                else if (txtSRate4.Visible == true)
                    txtSRate5.Focus();
                else if (txtSRate4.Visible == true)
                    txtSRate5.Focus();
                else if (txtSRate3.Visible == true)
                    txtSRate3.Focus();
                else if (txtSRate2.Visible == true)
                    txtSRate2.Focus();
                else if (txtSRate1.Visible == true)
                    txtSRate1.Focus();
            }
            else if (e.KeyCode == Keys.Enter)
            {
                if (txtBarcode.Enabled == true && txtBarcode.Visible == true)
                    txtBarcode.Focus();
                else if (chkExpiryItem.Enabled == true && chkExpiryItem.Visible == true)
                    chkExpiryItem.Focus();
                else if (cboAlterUnit.Enabled == true && cboAlterUnit.Visible == true)
                    cboAlterUnit.Focus();
                else
                    SaveData();
            }
        }
        private void sfcboUnit_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Shift == true && e.KeyCode == Keys.Enter))
            {
                txtManufacturer.Focus();
                SendKeys.Send("+{DOWN}");
            }
            else if (e.KeyCode == Keys.Enter)
            {
                if (txtRack.Visible == true)
                {
                    txtRack.Focus();
                    SendKeys.Send("+{DOWN}");
                }
                else if (txtColor.Visible == true)
                {
                    txtColor.Focus();
                    SendKeys.Send("+{DOWN}");
                }
                else if (txtSize.Visible == true)
                {
                    txtSize.Focus();
                    SendKeys.Send("+{DOWN}");
                }
                else if (cboBrand.Visible == true)
                {
                    cboBrand.Focus();
                    SendKeys.Send("+{F4}");
                }
                else
                {
                    cboDiscGroup.Focus();
                    SendKeys.Send("+{F4}");
                }
            }
            else if (e.KeyCode == Keys.F3)
            {
                btnUnitNew.PerformClick();
            }
        }
        private void txtItemCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
            {
                txtItemName.Focus();
            }
            else if (e.KeyCode == Keys.Enter)
            {
                txtCategoryList.Focus();
                SendKeys.Send("{DOWN}");
            }
        }
        private void txtHSNCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Shift == true && e.KeyCode == Keys.Enter)
                {
                    txtMaxRate.Focus();
                }
                else if (e.KeyCode == Keys.Down)
                {
                    this.txtHSNCode.TextChanged -= this.txtHSNCode_TextChanged;
                    if (txtHSNCode.Text == "") txtHSNCode.Text = "~";
                    this.txtHSNCode.TextChanged += this.txtHSNCode_TextChanged;
                    string sQuery = "SELECT DISTINCT Top 25 ISNULL( Convert(Varchar(18),HSNCODE),0) +ISNULL( Convert(Varchar(4),IGSTTaxPer),0) +ISNULL( Convert(Varchar(4),CessPer),0) as AnyWhere,HSNCODE as [HSN Code],hsnid,IGSTTaxPer as [IGST %],CessPer as [Cess %],HSNCODE  FROM tblHSNCode where  HID > 0 AND TenantID=" + Global.gblTenantID + " ";
                    new frmCompactSearch(GetFromHSNCodeSearch, sQuery, "AnyWhere|Convert(varchar(50),HSNCODE)|Convert(varchar(50),IGSTTaxPer)|Convert(varchar(50),CessPer)", txtHSNCode.Location.X + 750, txtHSNCode.Location.Y + 30, 3, 0, txtHSNCode.Text, 4, 0, "ORDER BY HSNCODE ASC", 0, 0, "HSN Code Search ...", 0, "200,80,80,0", true, "HSNCode", 0, true, this.MdiParent).ShowDialog();
                  
                        this.txtHSNCode.TextChanged -= this.txtHSNCode_TextChanged;
                    if (txtHSNCode.Text == "~") txtHSNCode.Clear();
                    this.txtHSNCode.TextChanged += this.txtHSNCode_TextChanged;
                        SendKeys.Send("{Tab}");
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    if (cboIGSTPerc.Text == "")
                    {
                        int taxindex = -1;
                        taxindex = cboIGSTPerc.FindStringExact("0");
                        if (taxindex >= 0)
                        {
                            cboIGSTPerc.SelectedIndex = taxindex;
                            SplitTaxPercentages();
                        }
                        else if (taxindex == -1)
                        {
                            cboIGSTPerc.SelectedIndex = 0;
                        }

                    }
                    if (Convert.ToDecimal(cboIGSTPerc.Text) != 0 || cboIGSTPerc.Enabled == false)
                    {
                        txtPRate.Focus();
                    }
                    else
                    {
                        cboIGSTPerc.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void txtMRP_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Shift == true && e.KeyCode == Keys.Enter) || e.KeyCode == Keys.Up)
            {
                txtPRate.Focus();
            }
            else if (e.KeyCode == Keys.Enter)
            {
                CallSRateCalcAsperMRPAndPRate();
                gpTaxInclExcl.Visible = true;
                dgvTaxIncl.DataSource = TaxInclusiveExclusive();
                cboCalc.Focus();
                SendKeys.Send("{F4}");
                
                if (AppSettings.PLCALCULATION > 0)
                    cboCalc.SelectedValue = AppSettings.PLCALCULATION - 1;
                else
                    cboCalc.SelectedValue = 1;

            }
        }
        private void txtSRate5_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Shift == true && e.KeyCode == Keys.Enter) || e.KeyCode == Keys.Up)
            {
                txtPerc5.Focus();
            }
            else if (e.KeyCode == Keys.Enter)
            {
                if (grpBatchDetails.Visible == true)
                {
                    if (gpTaxInclExcl.Visible == true)
                        gpTaxInclExcl.Visible = false;

                    if (cboBMode.Enabled == true)
                        cboBMode.Focus();
                    else if (cboAlterUnit.Enabled == true)
                        cboAlterUnit.Focus();

                    SendKeys.Send("{F4}");
                }
                else
                {
                    SaveData();
                }
            }
        }
        private void txtSRate4_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Shift == true && e.KeyCode == Keys.Enter) || e.KeyCode == Keys.Up)
            {
                txtPerc4.Focus();
            }
            else if (e.KeyCode == Keys.Enter)
            {
                if (AppSettings.IsActiveSRate5 == true)
                {
                    txtPerc5.Focus();
                }
                else if (grpBatchDetails.Visible == true && cboBMode.Visible == true && cboBMode.Enabled == true)
                {
                    if (cboBMode.Visible == true && cboBMode.Enabled == true)
                    {
                        if (gpTaxInclExcl.Visible == true)
                            gpTaxInclExcl.Visible = false;

                        cboBMode.Focus();
                        SendKeys.Send("{F4}");
                    }
                }
                else if (grpUnitDetails.Visible == true)
                {
                    if (cboAlterUnit.Visible == true && cboAlterUnit.Enabled == true)
                    {
                        cboAlterUnit.Focus();
                        SendKeys.Send("{F4}");
                    }
                }
                else
                {
                    SaveData();
                }
            }
        }
        private void txtSRate3_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Shift == true && e.KeyCode == Keys.Enter) || e.KeyCode == Keys.Up)
            {
                txtPerc3.Focus();
            }
            else if (e.KeyCode == Keys.Enter)
            {
                if (AppSettings.IsActiveSRate4 == true)
                {
                    txtPerc4.Focus();
                }
                else if (AppSettings.IsActiveSRate5 == true)
                {
                    txtPerc5.Focus();
                }
                else if (grpBatchDetails.Visible == true && cboBMode.Visible == true && cboBMode.Enabled == true)
                {
                    if (cboBMode.Visible == true && cboBMode.Enabled == true)
                    {
                        if (gpTaxInclExcl.Visible == true)
                            gpTaxInclExcl.Visible = false;

                        cboBMode.Focus();
                        SendKeys.Send("{F4}");
                    }
                }
                else if (grpUnitDetails.Visible == true)
                {
                    if (cboAlterUnit.Visible == true && cboAlterUnit.Enabled == true)
                    {
                        cboAlterUnit.Focus();
                        SendKeys.Send("{F4}");
                    }
                }
                else
                {
                    SaveData();
                }
            }
        }
        private void txtSRate2_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Shift == true && e.KeyCode == Keys.Enter) || e.KeyCode == Keys.Up)
            {
                txtPerc2.Focus();
            }
            else if (e.KeyCode == Keys.Enter)
            {
                if (AppSettings.IsActiveSRate3 == true)
                {
                    txtPerc3.Focus();
                }
                else if (AppSettings.IsActiveSRate4 == true)
                {
                    txtPerc4.Focus();
                }
                else if (AppSettings.IsActiveSRate5 == true)
                {
                    txtPerc5.Focus();
                }
                else if (grpBatchDetails.Visible == true && cboBMode.Visible == true && cboBMode.Enabled == true)
                {
                    if (cboBMode.Visible == true && cboBMode.Enabled == true)
                    {
                        if (gpTaxInclExcl.Visible == true)
                            gpTaxInclExcl.Visible = false;

                        cboBMode.Focus();
                        SendKeys.Send("{F4}");
                    }
                }
                else if (grpUnitDetails.Visible == true)
                {
                    if (cboAlterUnit.Visible == true && cboAlterUnit.Enabled == true)
                    {
                        cboAlterUnit.Focus();
                        SendKeys.Send("{F4}");
                    }
                }
                else
                    SaveData();
            }
        }
        private void txtSRate1_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Shift == true && e.KeyCode == Keys.Enter) || e.KeyCode == Keys.Up)
            {
                txtPerc1.Focus();
            }
            else if (e.KeyCode == Keys.Enter)
            {
                if (AppSettings.IsActiveSRate2 == true)
                {
                    txtPerc2.Focus();
                }
                else if (AppSettings.IsActiveSRate3 == true)
                {
                    txtPerc3.Focus();
                }
                else if (AppSettings.IsActiveSRate4 == true)
                {
                    txtPerc4.Focus();
                }
                else if (AppSettings.IsActiveSRate5 == true)
                {
                    txtPerc5.Focus();
                }
                else if (grpBatchDetails.Visible == true && cboBMode.Visible == true && cboBMode.Enabled == true)
                {
                    if (cboBMode.Visible == true && cboBMode.Enabled == true)
                    {
                        if (gpTaxInclExcl.Visible == true)
                            gpTaxInclExcl.Visible = false;

                        cboBMode.Focus();
                        SendKeys.Send("{F4}");
                    }
                }
                else if (grpUnitDetails.Visible == true)
                {
                    if (cboAlterUnit.Visible == true && cboAlterUnit.Enabled == true)
                    {
                        cboAlterUnit.Focus();
                        SendKeys.Send("{F4}");
                    }
                }
                else
                    SaveData();
            }
        }
        private void txtMaxRate_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Shift == true && e.KeyCode == Keys.Enter) || e.KeyCode == Keys.Up)
            {
                txtMinRate.Focus();
            }
            else if (e.KeyCode == Keys.Enter)
            {
                if (grpTaxDetails.Visible == true)
                {
                    txtHSNCode.Focus();
                    SendKeys.Send("+{Down}");
                }
                else
                    txtPRate.Focus();
            }
        }
        private void txtDescription_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Shift == true && e.KeyCode == Keys.Enter) || e.KeyCode == Keys.Up)
            {
                cboDepmnt.Focus();
            }
            else if (e.KeyCode.Equals(Keys.Enter))
            {
                if (txtDescription.Text == "")
                {
                     SendKeys.Send("{TAB}");
                    e.SuppressKeyPress = true;
                }
                else
                {
                    if (Comm.IsCursorOnEmptyLine(txtDescription) == true)
                    {
                        SendKeys.Send("{TAB}");
                        e.SuppressKeyPress = true;
                    }
                }
            }
        }
        private void txtManufacturer_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Shift == true && e.KeyCode == Keys.Enter) || e.KeyCode == Keys.Up)
            {
                txtCategoryList.Focus();
            }
            else if (e.KeyCode == Keys.Enter)
            {
                cboUnit.Focus();
                SendKeys.Send("{F4}");
            }
            else if (e.KeyCode == Keys.Down)
            {
                string strManfText = txtManufacturer.Text;
                CallManufacturerCompactSearch("", true);

                txtManufacturer.TextChanged -= txtManufacturer_TextChanged;
                if (txtManufacturer.Text == "")
                    txtManufacturer.Text = strManfText;
                txtManufacturer.TextChanged += txtManufacturer_TextChanged;

                //sfcboUnit.Focus();
                //SendKeys.Send("{F4}");
            }
            else if (e.KeyCode == Keys.F3)
            {
                btnAddManufacturer.PerformClick();
            }
        }
        private void txtCategoryList_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if ((e.Shift == true && e.KeyCode == Keys.Enter) || e.KeyCode == Keys.Up)
                {
                    txtItemCode.Focus();
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    strtxtCategory = txtCategoryList.Text;
                    txtManufacturer.Focus();
                    SendKeys.Send("+{DOWN}");
                }
                else if (e.KeyCode == Keys.Down)
                {
                    lblCategoryIds.Text = Convert.ToString(txtCategoryList.Tag);
                    string sQuery = "SELECT Category,CategoryID FROM tblCategories WHERE TenantID = " + Global.gblTenantID + " ";
                    new frmCompactSearch(GetCategoryFromNormalList, sQuery, "Category", txtCategoryList.Location.X + 480, txtCategoryList.Location.Y + 260, 1, 2, txtCategoryList.Text, 1, 0, "order by Category", 0, 0, "Category Search ...", 0, "20,0", true, "frmItemCategory", 0, false, this.MdiParent).ShowDialog();

                    //new frmCompactCheckedListSearch(GetFromCheckedList, sQuery, "Category", txtCategoryList.Location.X + 480, txtCategoryList.Location.Y + 260, 0, 2, txtCategoryList.Text, 0, 0, "", lblCategoryIds.Text, null, "Category", "frmItemCategory").ShowDialog();
                    //txtManufacturer.Focus();
                    //SendKeys.Send("+{DOWN}");
                }
                else if (e.KeyCode == Keys.F3)
                {
                    CategoryCreateNew();
                    txtManufacturer.Focus();
                    SendKeys.Send("+{DOWN}");
                }
                else if (e.KeyCode == Keys.F4)
                    MessageBox.Show("Cannot Handle the Process, because Multi selected category Cannot Edit !!s");
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void chkSlabSysytem_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Shift == true && e.KeyCode == Keys.Enter) || e.KeyCode == Keys.Up)
            {
                chkPRateIncl.Focus();
            }
            else if (e.KeyCode == Keys.Enter)
            {
                txtPRate.Focus();
            }
        }
        private void txtAgentCommision_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Shift == true && e.KeyCode == Keys.Enter) || e.KeyCode == Keys.Up)
            {
                txtDescription.Focus();
            }
            else if (e.KeyCode == Keys.Enter)
            {
                if (txtAgentCommision.Text == "") txtAgentCommision.Text = "0";
                if (Comm.IsDiscountPercentageOutofLimit(Convert.ToDecimal(txtAgentCommision.Text), "Agent Commission") == false)
                {
                    //txtDiscPerc.Focus();
                    if (txtDiscPerc.Visible == true)
                        txtDiscPerc.Focus();
                    else if (txtCoolie.Visible == true)
                        txtCoolie.Focus();
                    else if (txtROL.Visible == true)
                        txtROL.Focus();
                    else if (txtMOQ.Visible == true)
                        txtMOQ.Focus();
                    else if (txtMinRate.Visible == true)
                        txtMinRate.Focus();
                    else if (txtMaxRate.Visible == true)
                        txtMaxRate.Focus();
                    else if (txtHSNCode.Visible == true)
                        txtHSNCode.Focus();
                    else
                        txtPRate.Focus();
                }
                else
                {
                    txtAgentCommision.Text = "99";
                    txtAgentCommision.SelectAll();
                }
            }
        }
        private void txtDiscPerc_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Shift == true && e.KeyCode == Keys.Enter) || e.KeyCode == Keys.Up)
            {
                if (txtAgentCommision.Visible == true)
                    txtAgentCommision.Focus();
                else
                    txtDescription.Focus();
            }
            else if (e.KeyCode == Keys.Enter)
            {
                if (txtDiscPerc.Text == "") txtDiscPerc.Text = "0";
                if (Comm.IsDiscountPercentageOutofLimit(Convert.ToDecimal(txtDiscPerc.Text), "Item Discount") == true)
                {
                    txtDiscPerc.Text = "99";
                    txtDiscPerc.SelectAll();
                }
                else
                //txtCoolie.Focus();
                {
                    if (txtCoolie.Visible == true)
                        txtCoolie.Focus();
                    else if (txtROL.Visible == true)
                        txtROL.Focus();
                    else if (txtMOQ.Visible == true)
                        txtMOQ.Focus();
                    else if (txtMinRate.Visible == true)
                        txtMinRate.Focus();
                    else if (txtMaxRate.Visible == true)
                        txtMaxRate.Focus();
                    else if (txtHSNCode.Visible == true)
                        txtHSNCode.Focus();
                    else
                        txtPRate.Focus();
                }
            }
        }
        private void rdoNo_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Shift == true && e.KeyCode == Keys.Enter) || e.KeyCode == Keys.Up)
            {
                txtPLUNo.Focus();
            }
            //else if (e.KeyCode == Keys.Enter)
                //rdoWeight.Focus();
        }
        private void rdoWeight_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Shift == true && e.KeyCode == Keys.Enter) || e.KeyCode == Keys.Up)
            {
                //rdoNo.Focus();
            }
            else if (e.KeyCode == Keys.Enter)
            {
                if (chkExpiryItem.Enabled == true)
                    chkExpiryItem.Focus();
                else
                    cboAlterUnit.Focus();
            }
        }
        private void cboAlterUnit_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Shift == true && e.KeyCode == Keys.Enter))
            {
                if (txtDefaultExpDays.Enabled == true)
                    txtDefaultExpDays.Focus();
                else if (chkExpiryItem.Enabled == true)
                    chkExpiryItem.Focus();
                //else if (rdoWeight.Enabled == true)
                    //rdoWeight.Focus();
                else
                    cboBMode.Focus();
            }
            else if (e.KeyCode == Keys.Enter)
                txtUnitCFactor.Focus();
        }
        private void cboProductClass_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if ((e.Shift == true && e.KeyCode == Keys.Enter))
                {
                    txtUnitCFactor.Focus();
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    SaveData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
            }
        }
        private void txtIGSTPerc_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Shift == true && e.KeyCode == Keys.Enter) || e.KeyCode == Keys.Up)
            {
                txtHSNCode.Focus();
            }
            else if (e.KeyCode == Keys.Enter)
            {
                if (txtCGSTPerc.Text == "") txtCGSTPerc.Text = "0";
                
                if (txtCGSTPerc.Visible == true && txtCGSTPerc.ReadOnly == false)
                    txtCGSTPerc.Focus();
                else if (txtSGSTPerc.Visible == true && txtSGSTPerc.ReadOnly == false)
                    txtSGSTPerc.Focus();
                else if (txtCessPerc.Visible == true && txtCessPerc.ReadOnly == false)
                    txtCessPerc.Focus();
                else //if (txtPRate.ReadOnly == false)
                    txtPRate.Focus();
                //else
                //    chkSRateIncl.Focus();
            }
        }
        private void txtPRate_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
            {
                if (grpTaxDetails.Visible == true)
                {
                    if (chkSlabSysytem.Visible == true)
                        chkSlabSysytem.Focus();
                    else
                        chkPRateIncl.Focus();
                }
                else
                    txtMaxRate.Focus();
            }
            else if (e.KeyCode == Keys.Enter)
            {
                CallSRateCalcAsperMRPAndPRate();
                gpTaxInclExcl.Visible = true;
                dgvTaxIncl.DataSource = TaxInclusiveExclusive();
                dgvTaxIncl.Columns["ExclRate"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvTaxIncl.Columns["InclRate"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvTaxIncl.Columns["ExclRate"].Width = 90;
                dgvTaxIncl.Columns["InclRate"].Width = 90;
                txtMRP.SelectAll();
                if (txtMRP.Visible == true)
                    txtMRP.Focus();
                else
                    cboCalc.Focus();
            }
        }
        private void chkExpiryItem_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
            {
                //if (rdoWeight.Enabled == true)
                    //rdoWeight.Focus();
                //else 
                if (txtBarcode.Enabled == true)
                    txtBarcode.Focus();
                else
                    cboBMode.Focus();
            }
            else if (e.KeyCode == Keys.Enter)
            {
                if (txtshelflife.Enabled == false)
                {
                    if(cboAlterUnit.Visible == true)
                        cboAlterUnit.Focus();
                    else
                        SaveData();
                }
                else
                    txtshelflife.Focus();
            }
        }
        private void txtshelflife_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
            {
                chkExpiryItem.Focus();
            }
            else if (e.KeyCode == Keys.Enter)
            {
                txtDefaultExpDays.Focus();
            }
        }
        private void txtDefaultExpDays_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
            {
                txtshelflife.Focus();
            }
            else if (e.KeyCode == Keys.Enter)
            {
                cboAlterUnit.Focus();
            }
        }
        private void txtBarcode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
            {
                cboBMode.Focus();
            }
            else if (e.KeyCode == Keys.Enter)
            {
                if (txtPLUNo.Enabled == true)
                    txtPLUNo.Focus();
                else if (chkExpiryItem.Enabled == true)
                {
                    chkExpiryItem.Focus();
                    chkExpiryItem.Select();
                }
                else if (grpUnitDetails.Visible == true)
                    cboAlterUnit.Focus();
                else if (grpProductClass.Visible == true)
                    cboProductClass.Focus();
                else
                    SaveData();
            }
        }
        private void txtPLUNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
            {
                txtBarcode.Focus();
            }
            else if (e.KeyCode == Keys.Enter)
            {
                //rdoNo.Focus();
                if (chkExpiryItem.Enabled == true)
                    chkExpiryItem.Focus();
                else if (cboAlterUnit.Enabled && cboAlterUnit.Visible)
                    cboAlterUnit.Focus();
                else
                    SaveData();
            }
        }
        private void txtCessPerc_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
            {
                if (txtSGSTPerc.Visible == true)
                {
                    txtSGSTPerc.Focus();
                }
                else
                {
                    cboIGSTPerc.Focus();
                    //SendKeys.Send("{F4}");
                    //e.Handled = true;

                }
            }
            else if (e.KeyCode == Keys.Enter)
            {
                txtCompCessPer.Focus();
            }
        }
        private void txtCompCsePer_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
            {
                if (txtCessPerc.Visible == true)
                    txtCessPerc.Focus();
                else if (txtSGSTPerc.Visible == true)
                    txtSGSTPerc.Focus();
                else
                {
                    cboIGSTPerc.Focus();
                    //SendKeys.Send("{F4}");
                    //e.Handled = true;

                }
            }
            else if (e.KeyCode == Keys.Enter)
            {
                if (chkSRateIncl.Enabled && chkSRateIncl.Visible) 
                    chkSRateIncl.Focus();
                else if (txtPRate.Enabled && txtPRate.Visible) 
                    txtPRate.Focus();
                else if (txtMRP.Enabled && txtMRP.Visible)
                    txtMRP.Focus();
                else if (txtSRate1.Enabled && txtSRate1.Visible)
                    txtSRate1.Focus();
                
            }
        }
        private void txtUnitCFactor_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
            {
                cboAlterUnit.Focus();
            }
            else if (e.KeyCode == Keys.Enter)
            {
                 if (grpProductClass.Visible == true)
                    cboProductClass.Focus();
                else
                    SaveData();
            }
        }
        private void sfcboDiscGroup_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Shift == true && e.KeyCode == Keys.Enter))
            {
                if (cboBrand.Visible == true)
                    cboBrand.Focus();
                else if (txtSize.Visible == true)
                    txtSize.Focus();
                else if (txtColor.Visible == true)
                    txtColor.Focus();
                else
                    txtRack.Focus();
            }
            else if (e.KeyCode == Keys.Enter)
            {
                if (cboDepmnt.Visible == true)
                    cboDepmnt.Focus();
                else if (txtDescription.Visible == true)
                    txtDescription.Focus();
                else if (txtAgentCommision.Visible == true)
                    txtAgentCommision.Focus();
                else if(txtDiscPerc.Visible == true)
                    txtDiscPerc.Focus();
                else if (txtCoolie.Visible == true)
                    txtCoolie.Focus();
                else if (txtROL.Visible == true)
                    txtROL.Focus();
                else if (txtMOQ.Visible == true)
                    txtMOQ.Focus();
                else if (txtMinRate.Visible == true)
                    txtMinRate.Focus();
                else if (txtMaxRate.Visible == true)
                    txtMaxRate.Focus();
                else if (txtHSNCode.Visible == true)
                    txtHSNCode.Focus();
                else
                    txtPRate.Focus();
                SendKeys.Send("{F4}");
            }
            else if (e.KeyCode == Keys.F3)
            {
                btnDiscNew.PerformClick();
            }
        }
        private void txtRack_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Shift == true && e.KeyCode == Keys.Enter))
            {
                cboUnit.Focus();
            }
            else if (e.KeyCode == Keys.Down)
            {
                string strRackText = txtRack.Text;
                this.txtRack.TextChanged -= this.txtRack_TextChanged;
                if (txtRack.Text == "") txtRack.Text = "~";
                this.txtRack.TextChanged += this.txtRack_TextChanged;
                string sQuery = "SELECT ISNULL(Convert(Varchar(18),OtmData),0) as AnyWhere,OtmData as Rack,OtmID FROM tblOnetimeMaster where OtmType = 'ITMRACK' AND TenantID = " + Global.gblTenantID + " ";
                new frmCompactSearch(GetFromRackSearch, sQuery, "AnyWhere|Convert(varchar(18),OtmData)", txtRack.Location.X + 598, txtRack.Location.Y + 195, 1, 0, "", 2, 0, "order by OtmData ASC", 0, 0, "Rack Search ...", 0, "20,0", true, "Rack", 0, false, this.MdiParent).ShowDialog();
                this.txtRack.TextChanged -= this.txtRack_TextChanged;
                if (txtRack.Text == "~") txtRack.Clear();
                this.txtRack.TextChanged += this.txtRack_TextChanged;

                txtRack.TextChanged -= txtRack_TextChanged;
                if (txtRack.Text == "")
                    txtRack.Text = strRackText;
                txtRack.TextChanged += txtRack_TextChanged;

            }
            else if (e.KeyCode == Keys.Enter)
            {
                if (txtColor.Visible == true)
                {
                    txtColor.Focus();
                    SendKeys.Send("{DOWN}");
                }
                else if (txtSize.Visible == true)
                {
                    txtSize.Focus();
                    SendKeys.Send("{DOWN}");
                }
                else if (cboBrand.Visible == true)
                {
                    cboBrand.Focus();
                    SendKeys.Send("{F4}");
                }
                else
                {
                    cboDiscGroup.Focus();
                    SendKeys.Send("{F4}");
                }
            }
        }
        private void sfcboBrand_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
            {
                if (txtSize.Visible == true)
                    txtSize.Focus();
                else if (txtColor.Visible == true)
                    txtColor.Focus();
                else
                    txtRack.Focus();
            }
            else if (e.KeyCode == Keys.Enter)
            {
                cboDiscGroup.Focus();
                SendKeys.Send("{F4}");
            }
            else if (e.KeyCode == Keys.F3)
            {
                btnBrandNew.PerformClick();
            }
        }
        private void sfcboDepmnt_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift == true && e.KeyCode == Keys.Enter)
            {
                cboDiscGroup.Focus();
            }
            else if (e.KeyCode == Keys.Enter)
            {
                txtDescription.Focus();
            }
            else if (e.KeyCode == Keys.F3)
            {
                btnNewDept.PerformClick();
            }
        }

        //For Tab
        private void txtManufacturer_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyData == Keys.Tab)
            {
                e.IsInputKey = true;
                cboUnit.Focus();
            }
        }
        private void txtMinRate_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyData == Keys.Tab)
            {
                e.IsInputKey = true;
                txtMaxRate.Focus();
            }
        }
        private void txtMaxRate_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyData == Keys.Tab)
            {
                e.IsInputKey = true;
                txtHSNCode.Focus();
            }
        }
        private void txtHSNCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyData == Keys.Tab)
            {
                e.IsInputKey = true;
                //cboIGSTPerc.Focus();
                //SendKeys.Send("{F4}");
            }
        }
        private void txtCessPerc_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyData == Keys.Tab)
            {
                e.IsInputKey = true;
                txtCompCessPer.Focus();
            }
        }
        private void txtCompCessPer_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyData == Keys.Tab)
            {
                e.IsInputKey = true;
                chkSRateIncl.Focus();
            }
        }
        private void txtSRate5_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyData == Keys.Tab)
            {
                e.IsInputKey = true;
                cboBMode.Focus();
            }
        }
        private void cboBMode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyData == Keys.Tab)
            {
                e.IsInputKey = true;
                if (txtBarcode.Enabled == true)
                    txtBarcode.Focus();
                else if (chkExpiryItem.Enabled == true)
                    chkExpiryItem.Focus();
                else
                    cboAlterUnit.Focus();
            }
        }
        private void rdoNo_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyData == Keys.Tab)
            {
                e.IsInputKey = true;
                //rdoWeight.Focus();
            }
        }
        private void txtUnitCFactor_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyData == Keys.Tab)
            {
                e.IsInputKey = true;
                cboProductClass.Focus();
            }
        }
        private void chkSlabSysytem_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyData == Keys.Tab)
            {
                e.IsInputKey = true;
                txtPRate.Focus();
            }
        }
        private void chkExpiryItem_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyData == Keys.Tab)
            {
                if (txtshelflife.Enabled == true)
                {
                    e.IsInputKey = true;
                    txtshelflife.Focus();
                }
                else
                {
                    e.IsInputKey = true;
                    cboAlterUnit.Focus();
                }

            }
        }
        private void cboUnit_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyData == Keys.Tab)
            {
                e.IsInputKey = true;
                txtRack.Focus();
            }
        }

        private void txtSize_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyData == Keys.Tab)
            {
                e.IsInputKey = true;
                cboBrand.Focus();
            }
        }

        private void cboBrand_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyData == Keys.Tab)
            {
                e.IsInputKey = true;
                cboDiscGroup.Focus();
            }
        }

        private void cboDiscGroup_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyData == Keys.Tab)
            {
                e.IsInputKey = true;
                cboDepmnt.Focus();
            }
        }
        private void txtshelflife_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyData == Keys.Tab)
            {
                e.IsInputKey = true;
                cboAlterUnit.Focus();
            }
        }
        private void cboProductClass_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyData == Keys.Tab)
            {
                e.IsInputKey = true;
                btnSave.Focus();
            }
        }
        private void sfcboDiscGroup_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyData == Keys.Tab)
            {
                e.IsInputKey = true;
                cboDepmnt.Focus();
            }
        }
        private void sfcboDepmnt_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyData == Keys.Tab)
            {
                e.IsInputKey = true;
                txtDescription.Focus();
            }
        }

        private void txtPerc1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (txtPerc1.Text == "")
                {
                    txtPerc1.Text = "0";
                    txtPerc1.SelectAll();
                }
                else if (txtPerc1.Text.TrimEnd().TrimStart() == ".")
                {
                    txtPerc1.Text = "0.";
                    txtPerc1.SelectAll();
                }
                if (Comm.IsDiscountPercentageOutofLimit(Convert.ToDecimal(txtPerc1.Text), "Item Master", 9999, false) == true)
                {
                    txtPerc1.SelectAll();
                }

                try
                {
                    if (this.ActiveControl != null)
                        if (this.ActiveControl.Name == txtPerc1.Name)
                            txtSRate1.Text = CalculationInPriceDetails(Convert.ToDecimal(txtPerc1.Text)).ToString("#0.00");
                }
                catch
                { }

               // gpTaxInclExcl.Visible = true;
                dgvTaxIncl.DataSource = TaxInclusiveExclusive();
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void txtPerc2_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (txtPerc2.Text == "")
                {
                    txtPerc2.Text = "0";
                    txtPerc2.SelectAll();
                }
                else if (txtPerc2.Text.TrimEnd().TrimStart() == ".")
                {
                    txtPerc2.Text = "0.";
                    txtPerc2.SelectAll();
                }
                Comm.IsDiscountPercentageOutofLimit(Convert.ToDecimal(txtPerc2.Text), "Item Master", 9999, false);

                try
                {
                    if (this.ActiveControl != null)
                        if (this.ActiveControl.Name == txtPerc2.Name)
                            txtSRate2.Text = CalculationInPriceDetails(Convert.ToDecimal(txtPerc2.Text)).ToString("#0.00");
                }
                catch
                { }

                //gpTaxInclExcl.Visible = true;
                dgvTaxIncl.DataSource = TaxInclusiveExclusive();
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void txtPerc3_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (txtPerc3.Text == "")
                {
                    txtPerc3.Text = "0";
                    txtPerc3.SelectAll();
                }
                else if (txtPerc3.Text.TrimEnd().TrimStart() == ".")
                {
                    txtPerc3.Text = "0.";
                    txtPerc3.SelectAll();
                }
                Comm.IsDiscountPercentageOutofLimit(Convert.ToDecimal(txtPerc3.Text), "Item Master", 9999, false);

                try
                {
                    if (this.ActiveControl != null)
                        if (this.ActiveControl.Name == txtPerc3.Name)
                            txtSRate3.Text = CalculationInPriceDetails(Convert.ToDecimal(txtPerc3.Text)).ToString("#0.00");
                }
                catch
                { }

                //gpTaxInclExcl.Visible = true;
                dgvTaxIncl.DataSource = TaxInclusiveExclusive();
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void txtPerc4_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (txtPerc4.Text == "")
                {
                    txtPerc4.Text = "0";
                    txtPerc4.SelectAll();
                }
                else if (txtPerc4.Text.TrimEnd().TrimStart() == ".")
                {
                    txtPerc4.Text = "0.";
                    txtPerc4.SelectAll();
                }
                Comm.IsDiscountPercentageOutofLimit(Convert.ToDecimal(txtPerc4.Text), "Item Master", 9999, false);

                try
                {
                    if (this.ActiveControl != null)
                        if (this.ActiveControl.Name == txtPerc4.Name)
                            txtSRate4.Text = CalculationInPriceDetails(Convert.ToDecimal(txtPerc4.Text)).ToString("#0.00");
                }
                catch
                { 
                
                
                }

                // gpTaxInclExcl.Visible = true;
                dgvTaxIncl.DataSource = TaxInclusiveExclusive();
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void txtPerc5_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (txtPerc5.Text == "")
                {
                    txtPerc5.Text = "0";
                    txtPerc5.SelectAll();
                }
                else if (txtPerc5.Text.TrimEnd().TrimStart() == ".")
                {
                    txtPerc5.Text = "0.";
                    txtPerc5.SelectAll();
                }
                Comm.IsDiscountPercentageOutofLimit(Convert.ToDecimal(txtPerc5.Text), "Item Master", 9999, false);

                try
                {
                    if (this.ActiveControl != null)
                        if (this.ActiveControl.Name == txtPerc5.Name)
                            txtSRate5.Text = CalculationInPriceDetails(Convert.ToDecimal(txtPerc5.Text)).ToString("#0.00");
                }
                catch
                { }

                //gpTaxInclExcl.Visible = true;
                dgvTaxIncl.DataSource = TaxInclusiveExclusive();
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void txtIGSTPerc_TextChanged(object sender, EventArgs e)
        {
            //try
            //{
            //    if (cboIGSTPerc.Text.TrimEnd().TrimStart() == ".")
            //    {
            //        int taxindex = -1;
            //        taxindex = cboIGSTPerc.FindStringExact("0");
            //        if (taxindex >= 0)
            //        {
            //            cboIGSTPerc.SelectedIndex = taxindex;
            //            SplitTaxPercentages();
            //        }
            //        else if (taxindex == -1)
            //        {
            //            cboIGSTPerc.SelectedIndex = -1;
            //        }
            //    }
            //    if (Convert.ToDecimal(Comm.Val(cboIGSTPerc.Text)) > 0)
            //    {
            //        txtCGSTPerc.Text = (Convert.ToDecimal(Comm.Val(cboIGSTPerc.Text)) / 2).ToString("#0.00");
            //        txtSGSTPerc.Text = (Convert.ToDecimal(Comm.Val(cboIGSTPerc.Text)) / 2).ToString("#0.00");
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
            //    MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            //}
        }
        private void txtManufacturer_TextChanged(object sender, EventArgs e)
        {
            try
            {
                CallManufacturerCompactSearch(txtManufacturer.Text, true);
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void txtHSNCode_TextChanged(object sender, EventArgs e)
        {
            /*--------Added by Anjitha----*/
            //string sQuery = "SELECT DISTINCT Top 25 ISNULL( Convert(Varchar(18),HSNID),0) +ISNULL( Convert(Varchar(4),IGSTTaxPer),0) +ISNULL( Convert(Varchar(4),CessPer),0) as AnyWhere,HSNID as [HSN Code],IGSTTaxPer as [IGST %],CessPer as [Cess %],HSNID  FROM tblItemMaster where ActiveStatus = 1 AND HSNID > 0 AND TenantID=" + Global.gblTenantID + "";
            //new frmCompactSearch(GetFromHSNCodeSearch, sQuery, "AnyWhere|HSN Code|IGST %|Cess %|HSNID", txtHSNCode.Location.X + 750, txtHSNCode.Location.Y + 30, 3, 0, txtHSNCode.Text, 4, 0, "ORDER BY HSNID ASC", 0, 0, "HSN Code Search ...", 0, "200,80,80,0", true, "HSNCode", 0, true).ShowDialog();

            string sQuery = "SELECT DISTINCT ISNULL( Convert(Varchar(18),HSNCODE),0) +ISNULL( Convert(Varchar(4),IGSTTaxPer),0) +ISNULL( Convert(Varchar(4),CessPer),0) as AnyWhere, HSNCODE as [HSN Code],hsnid,IGSTTaxPer as [IGST %],CessPer as [Cess %],HSNCODE,HID  FROM tblHSNCode WHERE  HID > 0  AND TenantID=" + Global.gblTenantID + " ";
            new frmCompactSearch(GetFromHSNCodeSearch, sQuery, "AnyWhere|Convert(varchar(50),HSNCODE)|Convert(varchar(50),IGSTTaxPer)|Convert(varchar(50),CessPer)", txtHSNCode.Location.X + 750, txtHSNCode.Location.Y + 30,3 , 0, txtHSNCode.Text, 4, 0, "ORDER BY HSNCODE ASC", 0, 0, "HSN Code Search ...", 0, "200,80,80,0", true, "HSNCode", 0, true, this.MdiParent).ShowDialog();
            if (cboIGSTPerc.Text == "0")
            {
                cboIGSTPerc.Focus();
                //SendKeys.Send("{F4}");
            }
            else
            {
                txtPRate.Focus();
            }
        }
        private void txtCategoryList_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtCategoryList.Text))
                {
                    lblCategoryIds.Text = Convert.ToString(txtCategoryList.Tag);
                    lblCategoryIds.Text = "";
                }
                if (this.ActiveControl != null)
                {
                    if (this.ActiveControl.Name != "txtCategoryList")
                        return;
                }
                string sQuery = "SELECT Category,CategoryID FROM tblCategories WHERE TenantID = " + Global.gblTenantID + " ";
                new frmCompactSearch(GetCategoryFromNormalList, sQuery, "Category", txtCategoryList.Location.X + 480, txtCategoryList.Location.Y + 260, 1, 2, txtCategoryList.Text, 1, 0, "order by Category", 0, 0, "Category Search ...", 0, "20,0", true, "frmItemCategory", 0, false, this.MdiParent).ShowDialog();
                //new frmCompactCheckedListSearch(GetFromCheckedList, sQuery, "Category", txtCategoryList.Location.X + 480, txtCategoryList.Location.Y + 260, 0, 2, txtCategoryList.Text, 0, 0, "", lblCategoryIds.Text, null, "Category", "frmItemCategory").ShowDialog();
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void txtPRate_KeyPress(object sender, KeyPressEventArgs e)
        {
            textbox_KeyPress(sender, e);
        }
        private void txtMRP_KeyPress(object sender, KeyPressEventArgs e)
        {
            textbox_KeyPress(sender, e);
        }
        private void txtPerc1_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                textbox_KeyPress(sender, e);
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void txtshelflife_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void frmItemMaster_Load(object sender, EventArgs e)
        {
            try
            {
                this.Size = new Size(626, 700);
                this.tlpMain.ColumnStyles[1].Width = 0;
            }
            catch
            {

            }
            //SetShowHideValue();

            //try
            //{
            //    if (iIDFromEditWindow == 0)
            //    {
            //        if (bFromEditWindowItem == true)
            //            btnDelete.Enabled = true;
            //        else
            //            btnDelete.Enabled = false;

            //        LoadTestPanel.Enabled = false;
            //        ApplicationSettings();
            //        LoadUnitMaster();
            //        LoadBrand();
            //        LoadDiscountGroup();
            //        LoadProductClass();
            //        LoadBarCodeMode();
            //        LoadFromOneTimeMaster(0, "ITMRACK");
            //        LoadFromOneTimeMaster(0, "PRODCLASS");
            //        //LoadFromOneTimeMaster(0, "PROCLAS");
            //        LoadHSNCode();
            //        LoadCalPriceDetails();
            //        LoadDepartment();
            //        this.tlpMain.ColumnStyles[1].Width = 0;
            //        togglebtnActive.ToggleState = ToggleButtonState.Active;
            //        ClearFormControls();

            //        if (tlpColor.Visible == true)
            //        {
            //            txtColor.Tag = "";
            //            GetFromCheckedListColor("");
            //        }
            //        else
            //        {
            //            txtColor.Tag = "";
            //            GetFromCheckedListColor("");
            //        }
            //        if (tlpSize.Visible == true)
            //        {
            //            txtSize.Tag = "";
            //            GetFromCheckedListSize("");
            //        }
            //        else
            //        {
            //            txtSize.Tag = 1;
            //            GetFromCheckedListSize("1");
            //        }
            //        txtCategoryList.Tag = "";
            //        GetFromCheckedList("");
            //        txtItemName.Select();
            //    }
            //    else
            //    {
            //        txtItemName.Focus();
            //        txtItemName.SelectAll();
            //    }

            //    if (CtrlPassed != null)
            //    {
            //        txtItemName.Text = CtrlPassed.Text.ToString();
            //        txtItemCode.Text = CtrlPassed.Text.ToString();
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
            //    MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            //}
        }
        private void frmItemMaster_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                //grpProductClass.Visible= !grpProductClass.Visible;

                strControlLIst = strControlLIst + this.ActiveControl.Name + "   " + this.ActiveControl.TabIndex;

                Cursor.Current = Cursors.WaitCursor;
                if (e.KeyCode == Keys.Escape)//Close
                {
                    if (txtItemName.Text != "")
                    {
                        if (txtItemName.Text != strCheck)
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
                //else if (e.KeyCode == Keys.F3)//Find
                //{
                //    frmEditWindow frmEdit = new frmEditWindow(this.Name.ToUpper());
                //    frmEdit.Show();
                //}
                else if (e.KeyCode == Keys.F5)//Save
                {
                    SaveData();
                }
                else if (e.KeyCode == Keys.F7)//Delete
                {
                    if (bFromEditWindowItem == true)
                    {
                        DialogResult diaRslt = MessageBox.Show("Are you sure to delete the Item [" + txtItemName.Text + "] Permanently ?", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2);
                        if (diaRslt.Equals(DialogResult.Yes))
                        {
                            DeleteData();
                        }
                    }
                }
        
                ClosePriceListGrid();
                
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

        private void ClosePriceListGrid()
        {
            try
            {
                if (this.ActiveControl != null)
                if (this.ActiveControl.Name != txtPRate.Name &&
                    this.ActiveControl.Name != txtMRP.Name &&
                    this.ActiveControl.Name != cboCalc.Name &&
                    this.ActiveControl.Name != txtSRate1.Name &&
                    this.ActiveControl.Name != txtSRate2.Name &&
                    this.ActiveControl.Name != txtSRate3.Name &&
                    this.ActiveControl.Name != txtSRate4.Name &&
                    this.ActiveControl.Name != txtSRate5.Name &&
                    this.ActiveControl.Name != txtPerc1.Name &&
                    this.ActiveControl.Name != txtPerc2.Name &&
                    this.ActiveControl.Name != txtPerc3.Name &&
                    this.ActiveControl.Name != txtPerc4.Name &&
                    this.ActiveControl.Name != txtPerc5.Name
                    )
                {
                    gpTaxInclExcl.Visible = false;
                }
            }
            catch
            {
                //Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                //MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void chkExpiryItem_CheckedChanged(object sender, EventArgs e)
        {
            if (chkExpiryItem.Checked == true)
            {
                txtshelflife.Enabled = true;
                txtshelflife.Focus();
                txtshelflife.SelectAll();
                txtDefaultExpDays.Enabled = true;
            }
            else
            {
                txtshelflife.Enabled = false;
                txtshelflife.Text = "0";
                txtshelflife.SelectAll();
                txtDefaultExpDays.Enabled = false;
                txtDefaultExpDays.Text = "0";
            }
        }
        private void cboCalc_SelectionChangeCommitted(object sender, EventArgs e)
        {
            try
            {
                CallSRateCalcAsperMRPAndPRate();
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void cboBMode_SelectionChangeCommitted(object sender, EventArgs e)
        {
            try
            {
                txtBarcode.Focus();
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void cboBMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboBMode.SelectedValue == null) cboBMode.SelectedIndex = 0;
                if (cboBMode.SelectedValue.ToString().TrimStart().TrimEnd() == "0")
                {
                    cboBMode.SelectedValue = "0";
                    txtBarcode.Enabled = false;
                    txtPLUNo.Enabled = false;
                    rdoNo.Enabled = false;
                    rdoWeight.Enabled = false;
                    chkExpiryItem.Enabled = false;
                    //txtBarcode.Text = txtItemCode.Text.ToString();
                }
                if (cboBMode.SelectedValue.ToString().TrimStart().TrimEnd() == "0") // None
                {
                    if (iIDFromEditWindow == 0)
                        txtBarcode.Text = "";
                    
                    txtBarcode.Enabled = false;
                    txtPLUNo.Enabled = false;
                    rdoNo.Enabled = false;
                    rdoWeight.Enabled = false;
                    chkExpiryItem.Enabled = false;
                }
                else if (cboBMode.SelectedValue.ToString().TrimStart().TrimEnd() == "1") // MNF
                {
                    if (iIDFromEditWindow == 0)
                    {
                        txtBarcode.Enabled = true;
                    }
                    else
                    {
                        txtBarcode.Enabled = false;
                        if (txtBarcode.Text.ToString().Trim() == "0" || txtBarcode.Text.ToString().Trim() == "")
                            txtBarcode.Enabled = true;
                    }
                    txtPLUNo.Enabled = false;
                    rdoNo.Enabled = false;
                    rdoWeight.Enabled = false;
                    chkExpiryItem.Enabled = true;
                }
                else if (cboBMode.SelectedValue.ToString().TrimStart().TrimEnd() == "2") // Auto
                {
                    if (iIDFromEditWindow == 0)
                        txtBarcode.Text = "";

                    txtBarcode.Enabled = false;
                    txtPLUNo.Enabled = false;
                    rdoNo.Enabled = false;
                    rdoWeight.Enabled = false;
                    chkExpiryItem.Enabled = true;
                }
                else if (cboBMode.SelectedValue.ToString().TrimStart().TrimEnd() == "3") // WMH
                {
                    if (iIDFromEditWindow == 0)
                    {
                        txtBarcode.Enabled = true;
                        txtPLUNo.Enabled = true;
                        txtPLUNo.ReadOnly = false;
                        rdoWeight.Checked = true;

                        int PLU = Comm.gfnGetNextSerialNo("tblItemMaster", "pluno", "  tenantid = " + Global.gblTenantID);
                        txtPLUNo.Text = PLU.ToString();
                        txtBarcode.Text = (1000 + PLU).ToString();

                    }
                    else
                    {
                        txtBarcode.Enabled = false;
                        if (txtBarcode.Text.ToString().Trim() == "0" || txtBarcode.Text.ToString().Trim() == "")
                            txtBarcode.Enabled = true;
                    }
                    rdoNo.Enabled = true;
                    rdoWeight.Enabled = true;
                    chkExpiryItem.Enabled = false;
                }
                else
                {
                    txtBarcode.Enabled = false;
                    txtPLUNo.Enabled = false;
                    rdoNo.Enabled = true;
                    rdoWeight.Enabled = true;
                    chkExpiryItem.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void txtManufacturer_Enter(object sender, EventArgs e)
        {
            txtManufacturer.TextChanged -= txtManufacturer_TextChanged;
            Comm.ControlEnterLeave(txtManufacturer, true);
            txtManufacturer.TextChanged += txtManufacturer_TextChanged;
        }
        private void txtAgentCommision_Leave(object sender, EventArgs e)
        {
            if (txtAgentCommision.Text == "") txtAgentCommision.Text = "0";
            else if (txtAgentCommision.Text.TrimEnd().TrimStart() == ".") txtAgentCommision.Text = ".0";
            if (Comm.IsDiscountPercentageOutofLimit(Convert.ToDecimal(txtAgentCommision.Text), "Agent Commission") == true)
            {
                txtAgentCommision.Text = "99";
                txtAgentCommision.SelectAll();
            }
            Comm.ControlEnterLeave(txtAgentCommision);
            txtAgentCommision.Text = FormatValue(Convert.ToDouble(txtAgentCommision.Text), true, "#0.00");
        }
        private void txtMRP_Leave(object sender, EventArgs e)
        {
            CallSRateCalcAsperMRPAndPRate();
            Comm.ControlEnterLeave(txtMRP);
            if (string.IsNullOrEmpty(txtMRP.Text))
                txtMRP.Text = "0";
            txtMRP.Text = Convert.ToDecimal(txtMRP.Text).ToString(AppSettings.CurrDecimalFormat);
        }
        private void txtPRate_Leave(object sender, EventArgs e)
        {
            CallSRateCalcAsperMRPAndPRate();
            Comm.ControlEnterLeave(txtPRate);
            if (string.IsNullOrEmpty(txtPRate.Text))
                txtPRate.Text = "0";
            txtPRate.Text = Comm.chkChangeValuetoZero(Convert.ToDecimal(txtPRate.Text).ToString(AppSettings.CurrDecimalFormat));
            
        }
        private void txtItemName_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtItemName, true);
        }
        private void txtItemCode_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtItemCode, true);
        }
        private void txtItemCode_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtItemCode);
            dgvShowItemSearch.DataSource = null;
            pnlShowItemSearch.Visible = false;
            grpTaxDetails.Visible = true;
            grpPriceDetails.Visible = true;

        }
        private void txtCategoryList_Enter(object sender, EventArgs e)
        {
            txtCategoryList.TextChanged -= txtCategoryList_TextChanged;
            Comm.ControlEnterLeave(txtCategoryList, true);
            txtCategoryList.TextChanged += txtCategoryList_TextChanged;
        }
        private void txtCategoryList_Leave(object sender, EventArgs e)
        {
            txtCategoryList.TextChanged -= txtCategoryList_TextChanged;
            Comm.ControlEnterLeave(txtCategoryList, false, false);
            txtCategoryList.TextChanged += txtCategoryList_TextChanged;
        }
        private void txtManufacturer_Leave(object sender, EventArgs e)
        {
            txtManufacturer.TextChanged -= txtManufacturer_TextChanged;
            Comm.ControlEnterLeave(txtManufacturer, false, false);
            txtManufacturer.TextChanged -= txtManufacturer_TextChanged;
        }
        private void txtColor_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtColor, false, false);
        }
        private void txtSize_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtSize, false, false);
        }
        private void txtDescription_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtDescription);
        }
        private void txtDiscPerc_Leave(object sender, EventArgs e)
        {

        }
        private void txtCoolie_Leave(object sender, EventArgs e)
        {

        }
        private void txtROL_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtROL);
        }
        private void txtMOQ_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtMOQ);
            if (string.IsNullOrEmpty(txtMOQ.Text))
                txtMOQ.Text = "0";
            txtMOQ.Text = FormatValue(Convert.ToDouble(txtMOQ.Text), false, "");
        }

        private void txtMinRate_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtMinRate);
            if (string.IsNullOrEmpty(txtMinRate.Text))
                txtMinRate.Text = "0";
            txtMinRate.Text = FormatValue(Convert.ToDouble(txtMinRate.Text), true, "");
        }
        private void txtMaxRate_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtMaxRate);
            if (string.IsNullOrEmpty(txtMaxRate.Text))
                txtMaxRate.Text = "0";
            txtMaxRate.Text = FormatValue(Convert.ToDouble(txtMaxRate.Text), true, "");
        }
        private void txtHSNCode_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtHSNCode, false, false);
        }
        private void txtIGSTPerc_Leave(object sender, EventArgs e)
        {

            Comm.ControlEnterLeave(cboIGSTPerc);
            if (string.IsNullOrEmpty(cboIGSTPerc.Text))
            {
                int taxindex = -1;
                taxindex = cboIGSTPerc.FindStringExact("0");
                if (taxindex >= 0)
                {
                    cboIGSTPerc.SelectedIndex = taxindex;
                    SplitTaxPercentages();
                }
                else if (taxindex == -1)
                {
                    cboIGSTPerc.SelectedIndex = 0;
                }
            }
            else if (cboIGSTPerc.Text.TrimEnd().TrimStart() == ".")
            {
                int taxindex = -1;
                taxindex = cboIGSTPerc.FindStringExact("0");
                if (taxindex >= 0)
                {
                    cboIGSTPerc.SelectedIndex = taxindex;
                    SplitTaxPercentages();
                }
                else if (taxindex == -1)
                {
                    cboIGSTPerc.SelectedIndex = -1;
                }
            }
            //cboIGSTPerc.Text = FormatValue(Convert.ToDouble(cboIGSTPerc.Text), true, "#0.00");
        }
        private void txtCGSTPerc_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtCGSTPerc);
            if (string.IsNullOrEmpty(txtCGSTPerc.Text))
                txtCGSTPerc.Text = "0";
            else if (txtCGSTPerc.Text.TrimEnd().TrimStart() == ".") txtCGSTPerc.Text = ".0";
            txtCGSTPerc.Text = FormatValue(Convert.ToDouble(txtCGSTPerc.Text), true, "#0.00");
        }
        private void txtSGSTPerc_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtSGSTPerc);
            if (string.IsNullOrEmpty(txtSGSTPerc.Text))
                txtSGSTPerc.Text = "0";
            else if (txtSGSTPerc.Text.TrimEnd().TrimStart() == ".") txtSGSTPerc.Text = ".0";
            txtSGSTPerc.Text = FormatValue(Convert.ToDouble(txtSGSTPerc.Text), true, "#0.00");
        }
        private void txtCessPerc_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtCessPerc);
            if (string.IsNullOrEmpty(txtCessPerc.Text))
                txtCessPerc.Text = "0";
            else if (txtCessPerc.Text.TrimEnd().TrimStart() == ".") txtCessPerc.Text = ".0";
            txtCessPerc.Text = FormatValue(Convert.ToDouble(txtCessPerc.Text), true, "#0.00");
        }
        private void txtCompCsePer_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtCompCessPer);
            if (string.IsNullOrEmpty(txtCompCessPer.Text))
                txtCompCessPer.Text = "0";
            else if (txtCompCessPer.Text.TrimEnd().TrimStart() == ".") txtCompCessPer.Text = ".0";
            txtCompCessPer.Text = FormatValue(Convert.ToDouble(txtCompCessPer.Text), true, "#0.00");
        }
        private void txtPerc1_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtPerc1);
            if (string.IsNullOrEmpty(txtPerc1.Text))
                txtPerc1.Text = "0";
            else if (txtPerc1.Text.TrimEnd().TrimStart() == ".") txtPerc1.Text = ".0";
            txtPerc1.Text = FormatValue(Convert.ToDouble(txtPerc1.Text), true, "#0.00");
        }
        private void txtSRate1_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtSRate1);
            if (string.IsNullOrEmpty(txtSRate1.Text))
                txtSRate1.Text = "0";
            txtSRate1.Text = FormatValue(Convert.ToDouble(txtSRate1.Text), true, "");

            if(AppSettings.IsActiveSRate2 == false)
                if(AppSettings.IsActiveSRate3 == false)
                    if(AppSettings.IsActiveSRate4 == false)
                        if(AppSettings.IsActiveSRate5 == false)
                            gpTaxInclExcl.Visible = false;

        }
        private void txtPerc2_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtPerc2);
            if (string.IsNullOrEmpty(txtPerc2.Text))
                txtPerc2.Text = "0";
            else if (txtPerc2.Text.TrimEnd().TrimStart() == ".") txtPerc2.Text = ".0";
            txtPerc2.Text = FormatValue(Convert.ToDouble(txtPerc2.Text), true, "#0.00");
        }
        private void txtSRate2_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtSRate2);
            if (string.IsNullOrEmpty(txtSRate2.Text))
                txtSRate2.Text = "0";
            
            txtSRate2.Text = FormatValue(Convert.ToDouble(txtSRate2.Text), true, "");

            if (AppSettings.IsActiveSRate3 == false)
                if (AppSettings.IsActiveSRate4 == false)
                    if (AppSettings.IsActiveSRate5 == false)
                        gpTaxInclExcl.Visible = false;
        }
        private void txtPerc3_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtPerc3);
            if (string.IsNullOrEmpty(txtPerc3.Text))
                txtPerc3.Text = "0";
            else if (txtPerc3.Text.TrimEnd().TrimStart() == ".") txtPerc3.Text = ".0";
            txtPerc3.Text = FormatValue(Convert.ToDouble(txtPerc3.Text), true, "#0.00");
        }
        private void txtSRate3_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtSRate3);
            if (string.IsNullOrEmpty(txtSRate1.Text))
                txtSRate3.Text = "0";
            txtSRate3.Text = FormatValue(Convert.ToDouble(txtSRate3.Text), true, "");

            if (AppSettings.IsActiveSRate4 == false)
                if (AppSettings.IsActiveSRate5 == false)
                    gpTaxInclExcl.Visible = false;
        }
        private void txtPerc4_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtPerc4);
            if (string.IsNullOrEmpty(txtPerc4.Text))
                txtPerc4.Text = "0";
            else if (txtPerc4.Text.TrimEnd().TrimStart() == ".") txtPerc4.Text = ".0";
            txtPerc4.Text = FormatValue(Convert.ToDouble(txtPerc4.Text), true, "#0.00");
        }
        private void txtSRate4_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtSRate4);
            if (string.IsNullOrEmpty(txtSRate4.Text))
                txtSRate4.Text = "0";
            txtSRate4.Text = FormatValue(Convert.ToDouble(txtSRate4.Text), true, "");

            if (AppSettings.IsActiveSRate5 == false)
                gpTaxInclExcl.Visible = false;
        }
        private void txtPerc5_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtPerc5);
            if (string.IsNullOrEmpty(txtPerc5.Text))
                txtPerc5.Text = "0";
            else if (txtPerc5.Text.TrimEnd().TrimStart() == ".") txtPerc5.Text = ".0";
            txtPerc5.Text = FormatValue(Convert.ToDouble(txtPerc5.Text), true, "#0.00");
        }
        private void txtSRate5_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtSRate5);
            if (string.IsNullOrEmpty(txtSRate5.Text))
                txtSRate5.Text = "0";
            txtSRate5.Text = FormatValue(Convert.ToDouble(txtSRate5.Text), true, "");

            gpTaxInclExcl.Visible = false;
        }
        private void txtBarcode_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtBarcode);
        }
        private void txtPLUNo_Leave(object sender, EventArgs e)
        {
            CheckingPLUNoisUnique();
            Comm.ControlEnterLeave(txtPLUNo);
        }
        private void txtshelflife_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtshelflife);
        }
        private void txtDefaultExpDays_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtDefaultExpDays);
        }
        private void txtUnitCFactor_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtUnitCFactor);
        }
        private void txtColor_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtColor, true);
        }
        private void txtSize_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtSize, true);
        }
        private void txtDescription_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtDescription, true);
        }
        private void txtAgentCommision_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtAgentCommision, true);
        }
        private void txtDiscPerc_Enter(object sender, EventArgs e)
        {

        }
        private void txtCoolie_Enter(object sender, EventArgs e)
        {

        }
        private void txtROL_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtROL, true);
        }
        private void txtMOQ_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtMOQ, true);
        }
        private void txtMinRate_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtMinRate, true);
        }
        private void txtMaxRate_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtMaxRate, true);
        }
        private void txtHSNCode_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtHSNCode, true,false);
        }
        private void txtIGSTPerc_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(cboIGSTPerc, true);
            SendKeys.Send("{F4}");
        }
        private void txtCGSTPerc_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtCGSTPerc, true);
        }
        private void txtSGSTPerc_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtSGSTPerc, true);
        }
        private void txtCessPerc_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtCessPerc, true);
        }
        private void txtCompCsePer_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtCompCessPer, true);
        }
        private void txtPRate_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtPRate, true);
        }
        private void txtMRP_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtMRP, true);
        }
        private void txtPerc1_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtPerc1, true);
        }
        private void txtSRate1_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtSRate1, true);
        }
        private void txtPerc2_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtPerc2, true);
        }
        private void txtSRate2_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtSRate2, true);
        }
        private void txtPerc3_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtPerc3, true);
        }
        private void txtSRate3_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtSRate3, true);
        }
        private void txtPerc4_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtPerc4, true);
        }
        private void txtSRate4_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtSRate4, true);
        }
        private void txtPerc5_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtPerc5, true);
        }
        private void txtSRate5_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtSRate5, true);
        }
        private void txtBarcode_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtBarcode, true);
        }
        private void txtPLUNo_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtPLUNo, true);
        }
        private void txtshelflife_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtshelflife, true);
        }
        private void txtUnitCFactor_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtUnitCFactor, true);
        }
        private void txtDefaultExpDays_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(txtDefaultExpDays, true);
        }
        private void cboProductClass_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(cboProductClass, true);
        }
        private void sfcboUnit_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(cboUnit, true);
        }
        private void txtRack_Enter(object sender, EventArgs e)
        {
            this.txtRack.TextChanged -= this.txtRack_TextChanged;
            Comm.ControlEnterLeave(txtRack, true);
            this.txtRack.TextChanged += this.txtRack_TextChanged;
        }
        private void sfcboDiscGroup_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(cboDiscGroup, true);
        }
        private void cboCalc_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(cboCalc, true);
        }
        private void cboBMode_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(cboBMode, true);
        }
        private void cboAlterUnit_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(cboAlterUnit, true);
        }
        private void txtRack_Leave(object sender, EventArgs e)
        {
            this.txtRack.TextChanged -= this.txtRack_TextChanged;
            Comm.ControlEnterLeave(txtRack);
            this.txtRack.TextChanged += this.txtRack_TextChanged;
        }
        private void sfcboDiscGroup_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(cboDiscGroup, false, false);
        }
        private void cboCalc_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(cboCalc, false, false);
        }
        private void cboBMode_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(cboBMode, false, false);
        }
        private void cboAlterUnit_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(cboAlterUnit);
        }
        private void cboProductClass_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(cboProductClass);
        }
        private void txtItemName_Leave(object sender, EventArgs e)
        {
            if (iIDFromEditWindow == 0)
            {
                if (txtItemName.Text.Length > 4)
                    txtItemCode.Text = txtItemName.Text;
                else
                    txtItemCode.Text = txtItemName.Text;
            }
            Comm.ControlEnterLeave(txtItemName);
        }
        private void sfcboUnit_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(cboUnit, false, false);
        }
        private void sfcboBrand_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(cboBrand, true);
        }
        private void sfcboBrand_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(cboBrand, false, false);
        }
        private void cboDepmnt_Enter(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(cboDepmnt, true);
        }

        private void cboDepmnt_Leave(object sender, EventArgs e)
        {
            Comm.ControlEnterLeave(cboDepmnt, false, false);
        }
        private void txtHSNCode_KeyPress(object sender, KeyPressEventArgs e)
        {
            //Allow Numeric and decimal point only
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        //For Help
        private void txtItemName_Click(object sender, EventArgs e)
        {
            ToolTipItemMaster.SetToolTip(txtItemName, "Please specify Item Name");
        }
        private void txtItemCode_Click(object sender, EventArgs e)
        {
            ToolTipItemMaster.SetToolTip(txtItemCode, "Please specify unique name for Item Code");
        }
        private void txtCategoryList_Click(object sender, EventArgs e)
        {
            ToolTipItemMaster.SetToolTip(txtCategoryList, "Please specify Category");
        }
        private void txtManufacturer_Click(object sender, EventArgs e)
        {
            ToolTipItemMaster.SetToolTip(txtManufacturer, "Please specify manufacturer");
        }
        private void sfcboUnit_Click(object sender, EventArgs e)
        {
            ToolTipItemMaster.SetToolTip(cboUnit, "Please select Unit");
        }
        private void txtRack_Click(object sender, EventArgs e)
        {
            ToolTipItemMaster.SetToolTip(txtRack, "Please specify or select Rack for Item");
        }
        private void txtColor_Click(object sender, EventArgs e)
        {
            ToolTipItemMaster.SetToolTip(txtColor, "Please select Color for Item");
        }
        private void txtSize_Click(object sender, EventArgs e)
        {
            ToolTipItemMaster.SetToolTip(txtSize, "Please select Size for Item");
        }
        private void sfcboBrand_Click(object sender, EventArgs e)
        {
            ToolTipItemMaster.SetToolTip(cboBrand, "Please select Brand for Item");
        }
        private void sfcboDiscGroup_Click(object sender, EventArgs e)
        {
            ToolTipItemMaster.SetToolTip(cboDiscGroup, "Please select Discount Group for Item");
        }
        private void txtDescription_Click(object sender, EventArgs e)
        {
            ToolTipItemMaster.SetToolTip(txtDescription, "Please specify Description for Item");
        }
        private void txtAgentCommision_Click(object sender, EventArgs e)
        {
            ToolTipItemMaster.SetToolTip(txtAgentCommision, "Please specify Agent Commision Percentage for Item");
        }
        private void txtDiscPerc_Click(object sender, EventArgs e)
        {

        }
        private void txtCoolie_Click(object sender, EventArgs e)
        {

        }
        private void txtROL_Click(object sender, EventArgs e)
        {
            ToolTipItemMaster.SetToolTip(txtROL, "Please specify ROL for Item");
        }
        private void txtMOQ_Click(object sender, EventArgs e)
        {
            ToolTipItemMaster.SetToolTip(txtMOQ, "Please specify Minimum Order Quantity for Item");
        }
        private void txtMinRate_Click(object sender, EventArgs e)
        {
            ToolTipItemMaster.SetToolTip(txtMinRate, "Please specify Minimum for Item");
        }
        private void txtMaxRate_Click(object sender, EventArgs e)
        {
            ToolTipItemMaster.SetToolTip(txtMaxRate, "Please specify Maximum for Item");
        }
        private void txtHSNCode_Click(object sender, EventArgs e)
        {
            ToolTipItemMaster.SetToolTip(txtHSNCode, "Please specify or select HSN Code for Item");
        }
        private void txtIGSTPerc_Click(object sender, EventArgs e)
        {
            ToolTipItemMaster.SetToolTip(cboIGSTPerc, "Please specify IGST Percentage for Item");
        }
        private void txtCGSTPerc_Click(object sender, EventArgs e)
        {
            ToolTipItemMaster.SetToolTip(txtCGSTPerc, "Please specify CGST Percentage for Item");
        }
        private void txtSGSTPerc_Click(object sender, EventArgs e)
        {
            ToolTipItemMaster.SetToolTip(txtSGSTPerc, "Please specify SGST Percentage for Item");
        }
        private void txtCessPerc_Click(object sender, EventArgs e)
        {
            ToolTipItemMaster.SetToolTip(txtCessPerc, "Please specify Cess Percentage for Item");
        }
        private void txtPRate_Click(object sender, EventArgs e)
        {
            ToolTipItemMaster.SetToolTip(txtPRate, "Please specify Purchase Rate for Item");
        }
        private void txtMRP_Click(object sender, EventArgs e)
        {
            ToolTipItemMaster.SetToolTip(txtMRP, "Please specify MRP for Item");
        }
        private void cboCalc_Click(object sender, EventArgs e)
        {
            ToolTipItemMaster.SetToolTip(cboCalc, "Please select calculation for Item");
        }
        private void txtPerc1_Click(object sender, EventArgs e)
        {
            ToolTipItemMaster.SetToolTip(txtPerc1, "Please specify Sales Rate1 Percentage for Item");
        }
        private void txtSRate1_Click(object sender, EventArgs e)
        {
            ToolTipItemMaster.SetToolTip(txtSRate1, "Please specify Sales Rate1 for Item");
        }
        private void txtPerc2_Click(object sender, EventArgs e)
        {
            ToolTipItemMaster.SetToolTip(txtPerc2, "Please specify Sales Rate2 Percentage for Item");
        }
        private void txtSRate2_Click(object sender, EventArgs e)
        {
            ToolTipItemMaster.SetToolTip(txtSRate2, "Please specify Sales Rate2 for Item");
        }
        private void txtPerc3_Click(object sender, EventArgs e)
        {
            ToolTipItemMaster.SetToolTip(txtPerc3, "Please specify Sales Rate3 Percentage for Item");
        }
        private void txtSRate3_Click(object sender, EventArgs e)
        {
            ToolTipItemMaster.SetToolTip(txtSRate3, "Please specify Sales Rate3 for Item");
        }
        private void txtPerc4_Click(object sender, EventArgs e)
        {
            ToolTipItemMaster.SetToolTip(txtPerc4, "Please specify Sales Rate4 Percentage for Item");
        }
        private void txtSRate4_Click(object sender, EventArgs e)
        {
            ToolTipItemMaster.SetToolTip(txtSRate4, "Please specify Sales Rate4 for Item");
        }
        private void txtPerc5_Click(object sender, EventArgs e)
        {
            ToolTipItemMaster.SetToolTip(txtPerc5, "Please specify Sales Rate5 Percentage for Item");
        }
        private void txtSRate5_Click(object sender, EventArgs e)
        {
            ToolTipItemMaster.SetToolTip(txtSRate5, "Please specify Sales Rate5 for Item");
        }
        private void cboBMode_Click(object sender, EventArgs e)
        {
            ToolTipItemMaster.SetToolTip(cboBMode, "Please select Barcode for Item");
        }
        private void txtBarcode_Click(object sender, EventArgs e)
        {
            ToolTipItemMaster.SetToolTip(txtBarcode, "Please specify Barcode");
        }
        private void txtPLUNo_Click(object sender, EventArgs e)
        {
            ToolTipItemMaster.SetToolTip(txtPLUNo, "Please specify PLU No for Item");
        }
        private void txtshelflife_Click(object sender, EventArgs e)
        {
            ToolTipItemMaster.SetToolTip(txtshelflife, "Please specify Shelf Life Days for Item");
        }
        private void txtDefaultExpDays_Click(object sender, EventArgs e)
        {
            ToolTipItemMaster.SetToolTip(txtDefaultExpDays, "Please specify Default Expiry days for Item");
        }
        private void cboAlterUnit_Click(object sender, EventArgs e)
        {
            ToolTipItemMaster.SetToolTip(cboAlterUnit, "Please select Alter Unit for Item");
        }
        private void txtUnitCFactor_Click(object sender, EventArgs e)
        {
            ToolTipItemMaster.SetToolTip(txtUnitCFactor, "Please specify Conversion Factor for Item");
        }
        private void cboProductClass_Click(object sender, EventArgs e)
        {
            ToolTipItemMaster.SetToolTip(cboProductClass, "Please specify or select Product Class for Item");
        }
        private void cboDepmnt_Click(object sender, EventArgs e)
        {
            ToolTipItemMaster.SetToolTip(cboDepmnt, "Please select Department for Item");
        }

        private void btnCatnew_Click(object sender, EventArgs e)
        {
            try
            {
                string Category = txtCategoryList.Text;
                string CatIDs = Convert.ToString(txtCategoryList.Tag);
                frmItemCategory frmCate = new frmItemCategory(0, false, txtCategoryList);
                frmCate.ShowDialog();

                //if (Category.Trim().Length > 0)
                //{
                //    txtCategoryList.Text = Category + "," + txtCategoryList.Text;
                //    txtCategoryList.Tag = CatIDs + "," + txtCategoryList.Tag;
                //}
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void btnAddManufacturer_Click(object sender, EventArgs e)
        {
            try
            {
                this.ActiveControl.Name = btnAddManufacturer.Name;
                frmManufacturer frmManf = new frmManufacturer(0, false, txtManufacturer);
                frmManf.Show();
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void btnCatEdit_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Cannot Handle the Process, because Multi selected category Cannot Edit !!s");
            txtManufacturer.Focus();
            SendKeys.Send("+{DOWN}");
        }
        private void btnEditManufacturer_Click(object sender, EventArgs e)
        {
            try
            {
                frmManufacturer frmManf = new frmManufacturer(Convert.ToInt32(txtManufacturer.Tag), true, txtManufacturer);
                frmManf.Show();
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void btnUnitNew_Click(object sender, EventArgs e)
        {
            try
            {
                frmUnitMaster frmUnit = new frmUnitMaster(0, false, cboUnit);
                frmUnit.Show();
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void btnUnitEdit_Click(object sender, EventArgs e)
        {
            try
            {
                frmUnitMaster frmUnit = new frmUnitMaster(Convert.ToInt32(cboUnit.SelectedValue), false, cboUnit);
                frmUnit.Show();
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void btnColorNew_Click(object sender, EventArgs e)
        {
            try
            {
                string Color = txtColor.Text;
                string ColorIDs = Convert.ToString(txtColor.Tag);

                frmColorMaster frmColor = new frmColorMaster(0, false, txtColor);
                frmColor.ShowDialog();
                //if (Color.Trim().Length > 0)
                //{
                //    txtColor.Text = Color + "," + txtColor.Text;
                //    txtColor.Tag = ColorIDs + "," + txtColor.Tag;
                //}
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void btnColorEdit_Click(object sender, EventArgs e)
        {
            try
            {
                MessageBox.Show("Cannot Handle the Process, because Multi selected Color Can't Edit !!s");
                txtColor.Focus();
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void btnSizeNew_Click(object sender, EventArgs e)
        {
            try
            {
                string Size = txtSize.Text;
                string SizeIDs = Convert.ToString(txtSize.Tag);

                FrmSizeMaster frmSize = new FrmSizeMaster(0, false, txtSize);
                frmSize.ShowDialog();

                //if (Size.Trim().Length > 0)
                //{
                //    txtSize.Text = Size + "," + txtSize.Text;
                //    txtSize.Tag = SizeIDs + "," + txtSize.Tag;
                //}
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void btnSizeEdit_Click(object sender, EventArgs e)
        {
            try
            {
                MessageBox.Show("Cannot Handle the Process, because Multi selected Size Can't Edit !!s");
                txtSize.Focus();
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void btnBrandNew_Click(object sender, EventArgs e)
        {
            try
            {
                frmBrandMaster frmBrand = new frmBrandMaster(0, false, cboBrand);
                frmBrand.Show();
                
               
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void btnBrandEdit_Click(object sender, EventArgs e)
        {
            try
            {
                frmBrandMaster frmBrand = new frmBrandMaster(Convert.ToInt32(cboBrand.SelectedValue), true, cboBrand);
                frmBrand.Show();
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void btnDiscNew_Click(object sender, EventArgs e)
        {
            try
            {
                frmDiscountGroup frmDisc = new frmDiscountGroup(0, false, cboDiscGroup);
                frmDisc.Show();
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void btnDiscEdit_Click(object sender, EventArgs e)
        {
            try
            {
                frmDiscountGroup frmDisc = new frmDiscountGroup(Convert.ToInt32(cboDiscGroup.SelectedValue), true, cboDiscGroup);
                frmDisc.Show();
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void btnNewDept_Click(object sender, EventArgs e)
        {
            try
            {
                frmDepartment frmStDept = new frmDepartment(0, false, 0, cboDepmnt);
                frmStDept.Show();
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void btnEditDept_Click(object sender, EventArgs e)
        {
            try
            {
                frmDepartment frmStDept = new frmDepartment(Convert.ToInt32(cboDepmnt.SelectedValue), true, 0, cboDepmnt);
                frmStDept.Show();
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                if (chkLoadTest.Checked == true)
                {
                    if (txtLoadTestFrm.Text == "") txtLoadTestFrm.Text = "0";
                    if (txtLoadTestTo.Text == "") txtLoadTestTo.Text = "1";
                    for (int t = Convert.ToInt32(txtLoadTestFrm.Text); t < Convert.ToInt32(txtLoadTestTo.Text); t++)
                    {
                        LoadTesting(t);
                        SaveData();
                    }
                }
                else
                {
                    SaveData();
                }
                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to Save..." + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }
        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult diaRslt = MessageBox.Show("Are you sure to delete the Item [" + txtItemName.Text + "] Permanently ?", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2);
                if (diaRslt.Equals(DialogResult.Yes))
                {
                    Cursor.Current = Cursors.WaitCursor;
                    DeleteData();
                    Cursor.Current = Cursors.Default;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to Delete..." + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }
        private void btnFind_Click(object sender, EventArgs e)
        {
            try
            {
                frmEditWindow frmEdit = new frmEditWindow(this.Name.ToUpper(), this.MdiParent, "ItemMaster");
                frmEdit.Show();
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show("Failed to Find..." + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            if (txtItemName.Text != "")
            {
                if (txtItemName.Text != strCheck)
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

        private void txtRack_TextChanged(object sender, EventArgs e)
        {
            string sQuery = "SELECT ISNULL(Convert(Varchar(18),OtmData),0) as AnyWhere,OtmData as Rack,OtmID FROM tblOnetimeMaster where OtmType = 'ITMRACK' AND TenantID = " + Global.gblTenantID + " ";
            new frmCompactSearch(GetFromRackSearch, sQuery, "AnyWhere|Convert(varchar(18),OtmData)", txtRack.Location.X + 598, txtRack.Location.Y + 195, 1, 0, txtRack.Text, 2, 0, "order by OtmData ASC", 0, 0, "Rack Search ...", 0, "20,0", true, "Rack", 0, false, this.MdiParent).ShowDialog();
        }

        private void txtItemName_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(iIDFromEditWindow==0)
            ShowItemSearchDetailsinGrid();
        }
        private void dgvShowItemDet_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                if (e.ColumnIndex >= 0)
                {
                    DataGridViewTextBoxCell cell = (DataGridViewTextBoxCell)dgvShowItemSearch.Rows[e.RowIndex].Cells[0];
                    if (cell.Value != null)
                    {
                        int ItemID = Convert.ToInt32(cell.Value);
                        LoadData(ItemID);
                        //txtItemCode.Focus();
                        ShowItemSearchDetailsinGrid(true);
                        
                    }
                }
            }
        }
        private void dgvShowItemDet_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                int ItemID = Convert.ToInt32(dgvShowItemSearch.CurrentRow.Cells[0].FormattedValue);
                LoadData(ItemID);

                int iColumn = dgvShowItemSearch.CurrentCell.ColumnIndex;
                int iRowNo = dgvShowItemSearch.CurrentCell.RowIndex;

                if (dgvShowItemSearch.CurrentCell.RowIndex > 0)
                    dgvShowItemSearch.CurrentCell = dgvShowItemSearch[iColumn, iRowNo - 1];
                else
                    dgvShowItemSearch.CurrentCell = dgvShowItemSearch[iColumn, iRowNo];
            }
            else if (e.KeyCode == Keys.Left)
            {
                txtItemCode.Focus();
            }
        }
        private void btnMenu_Click(object sender, EventArgs e)
        {
            if (this.tlpMain.ColumnStyles[1].Width == 0)
            {
                this.Size = new Size(890, 700);
                this.tlpMain.ColumnStyles[1].SizeType = SizeType.Absolute;
                this.tlpMain.ColumnStyles[1].Width = 100;

                tlpMain.SetColumnSpan(tlpHeader, 2);
            }
            else
            {
                this.Size = new Size(626, 700);
                this.tlpMain.ColumnStyles[1].Width = 0;
                tlpMain.SetColumnSpan(tlpHeader, 1);
            }

            
        }
        private void lblQuickSettingsClose_Click(object sender, EventArgs e)
        {
            this.Size = new Size(626, 700);
            this.tlpMain.ColumnStyles[1].Width = 0;
        }
        private void btnCtrlSave_Click(object sender, EventArgs e)
        {
            SaveControlCheckboxList();
            SetShowHideValue();
           
        }
        private void chkAll_CheckedChanged(object sender, EventArgs e)
        {
            if (chkAll.Checked == true)
            {
                for (int i = 0; i < chklstShowControl.Items.Count; i++)
                {
                    chklstShowControl.SetItemChecked(i, true);
                }
            }
            else
            {
                for (int i = 0; i < chklstShowControl.Items.Count; i++)
                {
                    chklstShowControl.SetItemChecked(i, false);
                }
            }
            SetShowHideValue();
        }
        private void chklstShowControl_Click(object sender, EventArgs e)
        {
            //SetShowHideValue();
        }

        private void chklstShowControl_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            //if(e.NewValue == CheckState.Checked)
            // SetShowHideValue();
        }

        private void chklstShowControl_MouseClick(object sender, MouseEventArgs e)
        {
            //SetShowHideValue();
        }
        private void chkPRateIncl_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Shift == true && e.KeyCode == Keys.Enter) || e.KeyCode == Keys.Up)
            {
                chkSRateIncl.Focus();
            }
            else if (e.KeyCode == Keys.Enter)
            {
                if(chkSlabSysytem.Visible==true)
                    chkSlabSysytem.Focus();
                else
                txtPRate.Focus();
            }
        }

        #endregion

        #region "METHODS -  -------------------------------------------- >>"
        //Description : Validating the Mandatory Fields Before Save Functionality
        private bool ItemValidate()
        {
            bool bResult = true;
            string ErroMessage = "";

            if (cboBMode.SelectedValue == null) cboBMode.SelectedIndex = 0;

            if (txtItemName.Text == "")
            {
                ErroMessage += "Please Enter the Item Name";
                txtItemName.Select();
                txtItemName.SelectionStart = txtItemName.Text.ToString().Length;

                bResult = false;
            }
            
            if (txtItemCode.Text == "")
            {
                ErroMessage += "Please Enter the Item Code";
                txtItemCode.Select();
                bResult = false;
            }

            if (txtCategoryList.Text == "")
            {
                ErroMessage += "Category not selected. Default category will be loaded. Please change to specified category if necessary.";
                lblCategoryIds.Text = "1";
                txtCategoryList.Tag = lblCategoryIds.Text;
                this.txtCategoryList.TextChanged -= this.txtCategoryList_TextChanged;
                txtCategoryList.Text = GetCategoriesAsperIDs(lblCategoryIds.Text);
                this.txtCategoryList.TextChanged += this.txtCategoryList_TextChanged;
                txtCategoryList.Focus();

                bResult = false;
            }
            
            if (cboUnit.Text == "")
            {
                ErroMessage += "Please Select the Unit";
                cboUnit.Select();

                bResult = false;
            }
            
            if (cboIGSTPerc.SelectedIndex == -1)
            {
                ErroMessage += "Please Select the Tax Percentage";
                cboIGSTPerc.Select();

                bResult = false;
            }

            if (cboBMode.SelectedValue == null) cboBMode.SelectedIndex = 0;

            if (cboBMode.SelectedValue.ToString().TrimStart().TrimEnd() == "2")
                txtBarcode.Text = "";

            if (cboBMode.Text != "")
            {
                if (cboBMode.SelectedValue.ToString().TrimStart().TrimEnd() == "1" || cboBMode.SelectedValue.ToString().TrimStart().TrimEnd() == "3")
                {
                    if (txtBarcode.Text.Trim() == "")
                    {
                        ErroMessage += "Batchcode is mandatory for selected option " + cboBMode.Text.ToString();
                        txtBarcode.Select();

                        bResult = false;
                    }
                }
            }
            
            if (txtPLUNo.Text.Length > 0 && iIDFromEditWindow == 0)
            {
                string sQuery = "select PLUNO from tblItemMaster where PLUNO > 0 and PLUNO = " + txtPLUNo.Text + " And TenantID = '" + Global.gblTenantID + "'";
                DataTable dtPLU = Comm.fnGetData(sQuery).Tables[0];
                if (dtPLU.Rows.Count > 0)
                {
                    ErroMessage += "This PLUNo " + txtPLUNo.Text + " is already Exist.Try another PLU Number";
                    txtPLUNo.Focus();
                    txtPLUNo.SelectAll();

                    bResult = false;
                }
                else
                    bResult = true;
            }

            if (txtBarcode.Text != "")
            {
                string strSubSql = "";
                
                if (iIDFromEditWindow != 0)
                    strSubSql = " And ItemID <> " + iIDFromEditWindow + "";

                //string sQuery = "Select tblItemMaster.ItemID,BatchCode From tblItemMaster Join tblStock on tblItemMaster.ItemID = tblStock.ItemID Where BatchCode = '" + txtBarcode.Text + "'";
                string sQuery = "Select ItemID,BatchCode From tblStock Where LTRIM(RTRIM(BatchCode)) = '" + txtBarcode.Text.Trim() + "' " + strSubSql + " And TenantID = '" + Global.gblTenantID + "'";
                DataTable dtBatch = Comm.fnGetData(sQuery).Tables[0];
                if (dtBatch.Rows.Count > 0)
                {
                    ErroMessage += "This BatchCode " + txtBarcode.Text + " is already Exist.";
                    txtBarcode.Focus();
                    txtBarcode.SelectAll();

                    bResult = false;
                }
                else
                    bResult = true;
            }
            if (ErroMessage != "")
                MessageBox.Show(ErroMessage, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);

            return bResult;
        }
        //Description :Set Decimal Point For Discount Percentage
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
        //Description :Set Manufacture DefaultValue
        private void SetDefaultValue()
        {
            if (string.IsNullOrEmpty(txtManufacturer.Text))
            {
                txtManufacturer.Tag = 1;
                this.txtManufacturer.TextChanged -= this.txtManufacturer_TextChanged;
                try
                {
                    txtManufacturer.Text = Comm.fnGetData("Select MnfName From tblManufacturer Where MnfID = '" + txtManufacturer.Tag + "'").Tables[0].Rows[0][0].ToString();
                }
                catch
                {

                }
                this.txtManufacturer.TextChanged += this.txtManufacturer_TextChanged;
            }
            if (string.IsNullOrEmpty(txtRack.Text))
            {
                this.txtRack.TextChanged -= this.txtRack_TextChanged;
                try
                {
                    txtRack.Text = Comm.fnGetData("Select Top 1 OtmData From tblOnetimeMaster Where OtmType = 'ITMRACK'").Tables[0].Rows[0][0].ToString();
                }
                catch
                {

                }
                this.txtRack.TextChanged += this.txtRack_TextChanged;
            }
        }
        //Description :Setting asper Application Settings
         public void ApplicationSettings()
         {
            //Show Agent
            if (AppSettings.NeedAgent == false)
            {
                tlpAgent.Visible = false;
            }
            //Show Color
            if (AppSettings.NeedColor == false)
            {
                tlpColor.Visible = false;
            }
            //Show Size
            if (AppSettings.NeedSize == false)
            {
                tlpSize.Visible = false;
                //this.tlpColorSize.ColumnStyles[2].Width = 0;
            }
            //Show Brand
            if (AppSettings.NeedBrand == false)
            {
                tlpBrand.Visible = false;
            }
            //Show PriceList And Set PriceList Name
            if (!string.IsNullOrEmpty(AppSettings.SRate1Name))
            {
                lblSR1.Text = AppSettings.SRate1Name;
            }
            if (AppSettings.IsActiveSRate2 == true)
            {
                if (!string.IsNullOrEmpty(AppSettings.SRate2Name))
                {
                    lblSR2.Text = AppSettings.SRate2Name;
                }
                lblSR2.Visible = true;
                txtSRate2.Visible = true;
                lblSR2Perc.Visible = true;
                txtPerc2.Visible = true;
            }
            else
            {
                lblSR2.Visible = false;
                txtSRate2.Visible = false;
                lblSR2Perc.Visible = false;
                txtPerc2.Visible = false;

                if (AppSettings.IsActiveSRate3 == true)
                {
                    lblSR3.Visible = true;
                    txtSRate3.Visible = true;
                    lblSR3Perc.Visible = true;
                    txtPerc3.Visible = true;
                    lblSR3Perc.Location = new Point(5, 65);
                    txtPerc3.Location = new Point(5, 85);
                    lblSR3.Location = new Point(75, 65);
                    txtSRate3.Location = new Point(75, 85);
                }
            }
            if (AppSettings.IsActiveSRate3 == false)
            {
                lblSR3.Visible = false;
                txtSRate3.Visible = false;
                lblSR3Perc.Visible = false;
                txtPerc3.Visible = false;
            }
            else
            {
                if (!string.IsNullOrEmpty(AppSettings.SRate3Name))
                {
                    lblSR3.Text = AppSettings.SRate3Name;
                }
            }
            if (AppSettings.IsActiveSRate4 == false)
            {
                lblSR4.Visible = false;
                txtSRate4.Visible = false;
                lblSR4Perc.Visible = false;
                txtPerc4.Visible = false;

                if(AppSettings.IsActiveSRate5 == true)
                {
                    lblSR5.Visible = true;
                    txtSRate5.Visible = true;
                    lblSR5Perc.Visible = true;
                    txtPerc5.Visible = true;

                    lblSR5Perc.Location = new Point(5, 115);
                    txtPerc5.Location = new Point(5, 135);
                    lblSR5.Location = new Point(75, 115);
                    txtSRate5.Location = new Point(75, 135);
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(AppSettings.SRate4Name))
                {
                    lblSR4.Text = AppSettings.SRate4Name;
                }
            }
            if (AppSettings.IsActiveSRate5 == false)
            {
                lblSR5.Visible = false;
                txtSRate5.Visible = false;
                lblSR5Perc.Visible = false;
                txtPerc5.Visible = false;
            }
            else
            {
                if (!string.IsNullOrEmpty(AppSettings.SRate5Name))
                {
                    lblSR5.Text = AppSettings.SRate5Name;
                }
            }
            if ((AppSettings.IsActiveSRate4 == false && AppSettings.IsActiveSRate5 == false) && (AppSettings.IsActiveSRate2 == true || AppSettings.IsActiveSRate3 == true))
            {
                grpPriceDetails.Size = new Size(303, 135);
                grpPriceDetails.Location = new Point(277, 157);
                grpBatchDetails.Location = new Point(277, 300);
            }
            if (AppSettings.IsActiveSRate2 == false && AppSettings.IsActiveSRate3 == false)
            {
                grpPriceDetails.Location = new Point(277, 160);
                grpPriceDetails.Size = new Size(303, 95);
                grpBatchDetails.Location = new Point(277, 265);
                grpUnitDetails.Location = new Point(277, 403);
                grpProductClass.Location = new Point(277, 460);
                //cboProductClass.Location = new Point(375, 460);
            }
            //Change MRP Name 
            if (!string.IsNullOrEmpty(AppSettings.MRPName))
            {
                lblMRP.Text = AppSettings.MRPName;
                strMRPName = AppSettings.MRPName;

            }
            else
                strMRPName = "MRP";
            //Show TaxMode
            if (AppSettings.TaxEnabled == false)
            {
                grpTaxDetails.Visible = false;

                //grpPriceDetails.Location = new Point(277, 20);
                //grpBatchDetails.Location = new Point(277, 190);
                //grpUnitDetails.Location = new Point(277, 320);
                ////lblProdClass.Location = new Point(277, 380);
                ////cboProductClass.Location = new Point(375, 380);
            }
            else
            {
                if (AppSettings.TaxMode == 2)//GST
                {
                    grpTaxDetails.Visible = true;
                }
                else if (AppSettings.TaxMode ==1)//VAT
                {
                    lblIGSTPer.Text = "VAT";
                    lblCGSTPer.Visible = false;
                    lblSGSTPer.Visible = false;
                    txtCGSTPerc.Visible = false;
                    txtSGSTPerc.Visible = false;

                    if (AppSettings.CessMode != 0)//No Cess
                    {
                        lblCessPer.Location = new Point(6,70);
                        txtCessPerc.Location = new Point(105,70);
                    }
                    else
                    {
                        lblCompCessPer.Location = new Point(6, 75);
                        txtCompCessPer.Location = new Point(105, 75);
                        chkSRateIncl.Location = new Point(6, 110);
                        chkPRateIncl.Location = new Point(100, 110);
                        chkSlabSysytem.Location = new Point(195, 110);
                    }

                    //    grpPriceDetails.Location = new Point(277, 20);
                    //grpBatchDetails.Location = new Point(277, 190);
                    //grpUnitDetails.Location = new Point(277, 320);
                    //lblProdClass.Location = new Point(277, 380);
                    //cboProductClass.Location = new Point(375, 380);
                }
                else
                {
                    grpTaxDetails.Visible = false;

                    grpPriceDetails.Location = new Point(277, 20);
                    grpBatchDetails.Location = new Point(277, 190);
                    grpUnitDetails.Location = new Point(277, 320);
                    grpProductClass.Location = new Point(277, 380);
                    //cboProductClass.Location = new Point(375, 380);
                }
                //Cess
                if (AppSettings.CessMode == 0)//No Cess
                {
                    lblCessPer.Visible = false;
                    txtCessPerc.Visible = false;
                }
            }
        }
        //Description :Create New Category From this Form
        private void CategoryCreateNew()
        {
            frmItemCategory frmCat = new frmItemCategory(0, true);
            frmCat.ShowDialog();
            //if (Global.GFormTransID != 0)
                //ShowCategoryAsperID(Global.GFormTransID);
        }

        //Description : Get all  and  Selected Category to show Checked Compact List
        private string GetCategory(string sIDs = "")
        {
            UspGetCategoryCheckedListInfo GetCatChk = new UspGetCategoryCheckedListInfo();
            string sRetResult = "";
            if (sIDs != "")
            {
                string[] strCatIDs = sIDs.Split(',');
                string CatID = strCatIDs[0].Split('|')[0].ToString();

                DataTable dtData = new DataTable();
                GetCatChk.IDs = CatID;
                GetCatChk.TenantId = Global.gblTenantID;
                dtData = clsCat.GetCategoryCheckedList(GetCatChk);
                if (dtData.Rows.Count > 0)
                {
                    sRetResult = dtData.Rows[0][0].ToString();
                }
            }
            return sRetResult;
        }
        private string GetCategoriesAsperIDs(string sIDs = "")
        {
            UspGetCategoryCheckedListInfo GetCatChk = new UspGetCategoryCheckedListInfo();
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
        //Description : Get all  and  Selected Color to show Checked Compact List
        private string GetColorAsperIDs(string sIDs = "")
        {
            if (sIDs == "") 
                return "";
            string sRetResult = "";
            DataTable dtData = new DataTable();
            GetColorinfo.ColorID = 0;
            GetColorinfo.ColorIds = sIDs;
            GetColorinfo.TenantID = Global.gblTenantID;
            dtData = clsColor.GetColorMaster(GetColorinfo);
            if (dtData.Rows.Count > 0)
            {
                sRetResult = dtData.Rows[0][0].ToString();
            }
            return sRetResult;
        }
        //Description : Get all  and  Selected Size to show Checked Compact List
        private string GetSizeAsperIDs(string sIDs = "")
        {
            if (sIDs == "")
                return "";
            string sRetResult = "";
            DataTable dtData = new DataTable();
            GetSizeinfo.SizeID = 0;
            GetSizeinfo.SizeIds = sIDs;
            GetSizeinfo.TenantID = Global.gblTenantID;
            dtData = clsSize.GetSizeMaster(GetSizeinfo);
            if (dtData.Rows.Count > 0)
            {
                sRetResult = dtData.Rows[0][0].ToString();
            }
            return sRetResult;
        }
        //Description : Set  Checked Category to Category TextBox 
        private Boolean GetFromCheckedList(string sSelIDs)
        {
            lblCategoryIds.Text = sSelIDs;
            txtCategoryList.Tag = lblCategoryIds.Text;
            this.txtCategoryList.TextChanged -= this.txtCategoryList_TextChanged;
            txtCategoryList.Text = GetCategoriesAsperIDs(sSelIDs);
            this.txtCategoryList.TextChanged += this.txtCategoryList_TextChanged;

            if(sSelIDs != "")
            {
                string[] strCatIDs = sSelIDs.Split(',');
                decimal CatID = 0;
                if (strCatIDs.Length > 0)
                {
                    for (int i = 0; i < 1; i++)
                    {
                        CatID = Convert.ToDecimal(strCatIDs[i]);
                    }
                    string sQuery = "Select CategoryID,CatDiscPer FROM tblCategories WHERE CategoryID = " + CatID + "";
                    DataTable dtCatPer = Comm.fnGetData(sQuery).Tables[0];
                    if (dtCatPer.Rows.Count > 0)
                    {
                        dCatDiscPer = Convert.ToDecimal(dtCatPer.Rows[0]["CatDiscPer"].ToString());
                    }
                }
            }
            return true;
        }
        private Boolean GetCategoryFromNormalList(string sSelIDs)
        {
            lblCategoryIds.Text = sSelIDs;
            txtCategoryList.Tag = lblCategoryIds.Text;
            this.txtCategoryList.TextChanged -= this.txtCategoryList_TextChanged;
            txtCategoryList.Text = GetCategory(sSelIDs);
            this.txtCategoryList.TextChanged += this.txtCategoryList_TextChanged;

            if(sSelIDs != "")
            {
                string[] strCatIDs = sSelIDs.Split(',');
                decimal CatID = 0;
                if (strCatIDs.Length > 0)
                {
                    for (int i = 0; i < 1; i++)
                    {
                        CatID = Convert.ToDecimal(strCatIDs[i].Split('|')[0].ToString());
                    }
                    string sQuery = "Select CategoryID,CatDiscPer FROM tblCategories WHERE CategoryID = " + CatID + "";
                    DataTable dtCatPer = Comm.fnGetData(sQuery).Tables[0];
                    if (dtCatPer.Rows.Count > 0)
                    {
                        dCatDiscPer = Convert.ToDecimal(dtCatPer.Rows[0]["CatDiscPer"].ToString());
                    }
                }
            }
            return true;
        }
        //Description : Set  Checked Color to Category TextBox 
        private Boolean GetFromCheckedListColor(string sSelIDs)
        {
            txtColor.Tag = sSelIDs;
            txtColor.Text = GetColorAsperIDs(sSelIDs);
            return true;
        }
        //Description : Set  Checked Size to Category TextBox 
        private Boolean GetFromCheckedListSize(string sSelIDs)
        {
            txtSize.Tag = sSelIDs;
            if (sSelIDs == "0")
                txtSize.Text = "<None>";
            else
                txtSize.Text = GetSizeAsperIDs(sSelIDs);
            //SendKeys.Send("{Tab}");
            return true;
        }
        //Description : Call Manufacturer Compact search for Search manufacturer
        private void CallManufacturerCompactSearch(string sSearchData = "", bool ShowWholeData = false)
        {
            if (this.ActiveControl.Name != "txtManufacturer")
                return;
            string sQuery = "SELECT  ISNULL(MnfName,'')+ISNULL(MnfShortName,'') + ISNULL( Convert(Varchar(5),DiscPer),0) as AnyWhere,MnfName as [Manufacturer],MnfShortName as [Short Name] ,DiscPer as [Discount %] ,MnfID  FROM tblManufacturer where TenantID=" + Global.gblTenantID + " ";
            new frmCompactSearch(GetFromManufacturerSearch, sQuery, "AnyWhere|ISNULL(MnfName,'')|ISNULL(MnfShortName,'')|ISNULL( Convert(Varchar(5),DiscPer),0)", txtManufacturer.Location.X + 480, txtManufacturer.Location.Y + 150, 3, 0, sSearchData, 4, 0, "ORDER BY MnfName ASC", 0, 0, "Manufacturer Search ...", 0, "200,100,80,0", ShowWholeData, "frmManufacture", 0, false, this.MdiParent).ShowDialog();
        }
        //Description : Fill Data  when Manufacture is Select from the Grid Compact Search
        private Boolean GetFromManufacturerSearch(string LstIDandText)
        {
            string[] sCompSearchData = LstIDandText.Split('|');
            DataTable dtManf = new DataTable();
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
                        GetManinfo.MnfID = Convert.ToInt32(sCompSearchData[0].ToString());
                        GetManinfo.TenantID = Global.gblTenantID;
                        dtManf = clsMan.GetManufacturer(GetManinfo);
                        if (dtManf.Rows.Count > 0)
                        {
                            this.txtManufacturer.TextChanged -= this.txtManufacturer_TextChanged;
                            txtManufacturer.Text = dtManf.Rows[0]["MnfName"].ToString();
                            this.txtManufacturer.TextChanged += this.txtManufacturer_TextChanged;
                            txtManufacturer.Tag = dtManf.Rows[0]["MnfID"].ToString();
                            dManfDiscPer=Convert.ToDecimal(dtManf.Rows[0]["DiscPer"].ToString());
                        }
                        return true;
                    }
                    else
                    {
                        this.txtManufacturer.TextChanged -= this.txtManufacturer_TextChanged;
                        txtManufacturer.Text = sCompSearchData[1].ToString();
                        this.txtManufacturer.TextChanged += this.txtManufacturer_TextChanged;
                        return true;
                    }
                }
                else
                    return false;
            }
        }
        //Description : Fill Data  when HSNCODE is Select or Type New HSN Code from the Grid Compact Search
        private Boolean GetFromHSNCodeSearch(string LstIDandText)
        {
            string[] sCompSearchData = LstIDandText.Split('|');
            DataTable dtHSN = new DataTable();
            if (sCompSearchData[0].ToString() == "NOTEXIST")
            {
                this.txtHSNCode.TextChanged -= this.txtHSNCode_TextChanged;
                txtHSNCode.Text = ""; // sCompSearchData[1].ToString();
                txtHSNCode.Tag = ""; // sCompSearchData[2].ToString();
                this.txtHSNCode.TextChanged += this.txtHSNCode_TextChanged;

                //cboIGSTPerc.Text = "0";
                int taxindex = -1;
                taxindex = cboIGSTPerc.FindStringExact("0");
                if (taxindex >= 0)
                {
                    cboIGSTPerc.SelectedIndex = taxindex;
                    SplitTaxPercentages();
                }
                else if (taxindex == -1)
                {
                    cboIGSTPerc.SelectedIndex = 0;
                }

                txtSGSTPerc.Text = "0";
                txtCGSTPerc.Text = "0";
                txtCessPerc.Text = "0";
                return true;
            }
            else
            {
                if (sCompSearchData.Length > 0)
                {
                    if (Convert.ToInt32(sCompSearchData[0].ToString()) != 0)
                    {
                        //Commented and Added by Anjitha 24/03/2022 12:21 PM
                         GetHSNInfo.TenantID = Global.gblTenantID;
                       // GetHSNInfo.HSNID = 0;
                        GetHSNInfo.HSNCODE = Convert.ToDouble(sCompSearchData[0].ToString());
                        GetHSNInfo.IGSTTaxPer = Convert.ToDouble(sCompSearchData[2].ToString());
                        dtHSN = clsItmMst.GetHSNFromItemMaster(GetHSNInfo);

                        if (dtHSN.Rows.Count > 0)
                        {
                            this.txtHSNCode.TextChanged -= this.txtHSNCode_TextChanged;
                            txtHSNCode.Text = dtHSN.Rows[0]["HSNCODE"].ToString();
                            txtHSNCode.Tag = dtHSN.Rows[0]["HSNID"].ToString();
                            this.txtHSNCode.TextChanged += this.txtHSNCode_TextChanged;

                            cboIGSTPerc.Text = Convert.ToDecimal(dtHSN.Rows[0]["IGSTTaxPer"].ToString()).ToString();
                            //int taxindex = -1;
                            //taxindex = cboIGSTPerc.FindStringExact(dtHSN.Rows[0]["IGSTTaxPer"].ToString());
                            //if (taxindex >= 0)
                            //{
                              
                             SplitTaxPercentages();
                            //}
                            //else if (taxindex == -1)
                            //{
                            //    cboIGSTPerc.SelectedIndex = -1;
                            //    MessageBox.Show("IGST Percentage " + dtHSN.Rows[0]["IGSTTaxPer"].ToString() + " not found in the list. Please select correct IGST Percentage or add to list from settings.");
                            //}

                            //txtSGSTPerc.Text = Convert.ToDecimal(dtHSN.Rows[0]["SGSTTaxPer"].ToString()).ToString("#0.00");
                            //txtCGSTPerc.Text = Convert.ToDecimal(dtHSN.Rows[0]["CGSTTaxPer"].ToString()).ToString("#0.00");
                            txtCessPerc.Text = Convert.ToDecimal(dtHSN.Rows[0]["CessPer"].ToString()).ToString("#0.00");
                        }
                        return true;
                    }
                    else
                    {
                        this.txtHSNCode.TextChanged -= this.txtHSNCode_TextChanged;
                        txtHSNCode.Text = sCompSearchData[1].ToString();
                        this.txtHSNCode.TextChanged += this.txtHSNCode_TextChanged;
                        return true;
                    }
                }
                else
                    return false;
            }
        }
        //Description : Fill Data  when Rack is Select or Type New Rack from the Grid Compact Search
        private Boolean GetFromRackSearch(string LstIDandText)
        {
            string[] sCompSearchData = LstIDandText.Split('|');
            DataTable dtRack = new DataTable();
            if (sCompSearchData[0].ToString() == "NOTEXIST")
            {
                this.txtRack.TextChanged -= this.txtRack_TextChanged;
                txtRack.Text = sCompSearchData[1].ToString();
                this.txtRack.TextChanged += this.txtRack_TextChanged;
                return true;
            }
            else
            {
                if (sCompSearchData.Length > 0)
                {
                    if (Convert.ToInt32(sCompSearchData[0].ToString()) != 0)
                    {
                        GetOtminfo.OtmID = Convert.ToInt32(sCompSearchData[0].ToString());
                        GetOtminfo.TenantID = Global.gblTenantID;
                        GetOtminfo.OtmType = "ITMRACK";
                        dtRack = clsOtm.GetOnetimeMaster(GetOtminfo);

                        if (dtRack.Rows.Count > 0)
                        {
                            this.txtRack.TextChanged -= this.txtRack_TextChanged;
                            txtRack.Text = dtRack.Rows[0]["OtmData"].ToString();
                            txtRack.Tag = dtRack.Rows[0]["OtmID"].ToString();
                            this.txtRack.TextChanged += this.txtRack_TextChanged;
                        }
                        return true;
                    }
                    else
                    {
                        this.txtRack.TextChanged -= this.txtRack_TextChanged;
                        txtRack.Text = sCompSearchData[1].ToString();
                        this.txtRack.TextChanged += this.txtRack_TextChanged;
                        return true;
                    }
                }
                else
                    return false;
            }
        }

        //Description : Calculations of Sales Rate 
        private void CallSRateCalcAsperMRPAndPRate()
        {
            txtSRate1.Text = Comm.chkChangeValuetoZero(CalculationInPriceDetails(Convert.ToDecimal(txtPerc1.Text)).ToString("#0.00"));
            txtSRate2.Text = Comm.chkChangeValuetoZero(CalculationInPriceDetails(Convert.ToDecimal(txtPerc2.Text)).ToString("#0.00"));
            txtSRate3.Text = Comm.chkChangeValuetoZero(CalculationInPriceDetails(Convert.ToDecimal(txtPerc3.Text)).ToString("#0.00"));
            txtSRate4.Text = Comm.chkChangeValuetoZero(CalculationInPriceDetails(Convert.ToDecimal(txtPerc4.Text)).ToString("#0.00"));
            txtSRate5.Text = Comm.chkChangeValuetoZero(CalculationInPriceDetails(Convert.ToDecimal(txtPerc5.Text)).ToString("#0.00"));
        }
        //Description : Calculations Of PriceDetails Based on +PRate & -MRP
        private decimal CalculationInPriceDetails(decimal dEntry = 0, bool bForward = true)
        {
            decimal dPRate = Convert.ToDecimal(Comm.Val(txtPRate.Text));
            decimal dMRP = Convert.ToDecimal(Comm.Val(txtMRP.Text));
            decimal dReturn = 0;

            decimal dCRate = Convert.ToDecimal(Comm.Val(txtPRate.Text));

            if (Conversion.Val(cboIGSTPerc.Text) > 0)
            {
                dCRate = dCRate + ((dCRate * Convert.ToDecimal(cboIGSTPerc.Text)) / 100);
            }

            if (txtPerc1.Text == "") txtPerc1.Text = "0";
            if (txtPerc2.Text == "") txtPerc2.Text = "0";
            if (txtPerc3.Text == "") txtPerc3.Text = "0";
            if (txtPerc4.Text == "") txtPerc4.Text = "0";
            if (txtPerc5.Text == "") txtPerc5.Text = "0";

            if (bForward == false) // && (Convert.ToDecimal(txtPerc1.Text) > 0 || Convert.ToDecimal(txtPerc2.Text) > 0 || Convert.ToDecimal(txtPerc3.Text) > 0 || Convert.ToDecimal(txtPerc4.Text) > 0 || Convert.ToDecimal(txtPerc5.Text) > 0))
            {
                if (cboCalc.SelectedIndex == 0) // + PRate or + costrate (costrate can be calculated on a stock in transaction only) 
                    if (dPRate != 0) dReturn = Convert.ToDecimal(((dEntry - dPRate) * 100) / dPRate);
                else if (cboCalc.SelectedIndex == 1) // - MRP
                    if (dMRP != 0) dReturn = Convert.ToDecimal(((dMRP - dEntry) * 100) / dPRate);
                else if (cboCalc.SelectedIndex == 2) // + CRATE
                    if (dCRate != 0) dReturn = Convert.ToDecimal(((dEntry - dCRate) * 100) / dPRate);
            }
            if (bForward == true) // && (Convert.ToDecimal(txtPerc1.Text) > 0 || Convert.ToDecimal(txtPerc2.Text) > 0 || Convert.ToDecimal(txtPerc3.Text) > 0 || Convert.ToDecimal(txtPerc4.Text) > 0 || Convert.ToDecimal(txtPerc5.Text) > 0))
            {
                if (cboCalc.SelectedIndex == 0) // + PRate or + costrate (costrate can be calculated on a stock in transaction only) 
                    dReturn = Convert.ToDecimal(dPRate + (dPRate * dEntry / 100));
                else if (cboCalc.SelectedIndex == 1) // - MRP
                    dReturn = Convert.ToDecimal(dMRP - (dMRP * dEntry / 100));
                else if (cboCalc.SelectedIndex == 2) // + CRATE
                    dReturn = Convert.ToDecimal(dCRate + (dCRate * dEntry / 100));
            }
            return dReturn;
        }

        //Description : Load Product Class To Combobox
        private void LoadProductClass(int iSelectedID = 0)
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

            cboProductClass.DataSource = dtProdClass;
            cboProductClass.DisplayMember = "ProdType";
            cboProductClass.ValueMember = "ProdTypeID";

            cboProductClass.SelectedIndex = 0;
        }
        //Description : Load Barcode To Combobox
        private void LoadBarCodeMode(int iSelectedID = 0)
        {
            int i = 0;
            try
            {

            DataTable dtBarcode = new DataTable();
            dtBarcode.Clear();

            dtBarcode.Columns.Add("BarModeID");
            dtBarcode.Columns.Add("BarCodeMode");

            if (chklstShowControl.Items.Count > 0)
            {
                int RowsCount = chklstShowControl.Items.Count;

                for (i = 0; i < RowsCount; i++)
                {
                    if (dtCheckBox.Rows[i]["strKeyName"].ToString().ToUpper() == "BLNBATCHMODENONE" && chklstShowControl.GetItemChecked(i) == true)
                    {
                        DataRow dRow1 = dtBarcode.NewRow();
                        dRow1["BarModeID"] = "0";
                        dRow1["BarCodeMode"] = "None";
                        dtBarcode.Rows.Add(dRow1);
                    }
                    if (dtCheckBox.Rows[i]["strKeyName"].ToString().ToUpper() == "BLNBATCHMODEMNF" && chklstShowControl.GetItemChecked(i) == true)
                    {
                        DataRow dRow2 = dtBarcode.NewRow();
                        dRow2["BarModeID"] = "1";
                        dRow2["BarCodeMode"] = "MNF Barcode";
                        dtBarcode.Rows.Add(dRow2);
                    }
                    if (dtCheckBox.Rows[i]["strKeyName"].ToString().ToUpper() == "BLNBATCHMODEAUTO" && chklstShowControl.GetItemChecked(i) == true)
                    {
                        DataRow dRow3 = dtBarcode.NewRow();
                        dRow3["BarModeID"] = "2";
                        dRow3["BarCodeMode"] = "<Auto Barcode>";
                        dtBarcode.Rows.Add(dRow3);
                    }
                    if (dtCheckBox.Rows[i]["strKeyName"].ToString().ToUpper() == "BLNBATCHMODEWM" && chklstShowControl.GetItemChecked(i) == true)
                    {
                        DataRow dRow4 = dtBarcode.NewRow();
                        dRow4["BarModeID"] = "3";
                        dRow4["BarCodeMode"] = "Weighing Barcode";
                        dtBarcode.Rows.Add(dRow4);
                    }
                }
            }

                if (dtBarcode.Rows.Count <= 0)
                {
                    DataRow dRow1 = dtBarcode.NewRow();
                    dRow1["BarModeID"] = "0";
                    dRow1["BarCodeMode"] = "None";
                    dtBarcode.Rows.Add(dRow1);
                }

                if (dtBarcode.Rows.Count > 0)
            { 
                cboBMode.DataSource = dtBarcode;
                cboBMode.DisplayMember = "BarCodeMode";
                cboBMode.ValueMember = "BarModeID";
            }
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        //Description : Load Unit To Combobox
        private void LoadUnitMaster(int iSelectedID = 0)
        {
            DataTable dtUnit = new DataTable();
            GetUnitinfo.UnitID = iSelectedID;
            GetUnitinfo.TenantID = Global.gblTenantID;
            dtUnit = clsUnit.GetUnitMaster(GetUnitinfo);
            if (dtUnit.Rows.Count > 0)
            {
                Comm.LoadControl(cboUnit, dtUnit, "", false, false, "Short Name", "UnitID");
                Comm.LoadControl(cboAlterUnit, dtUnit, "", false, false, "Short Name", "UnitID");
            }
            //dtUnit.Dispose();
        }
        //Description : Load Brand To Combobox
        public void LoadBrand(int iSelectedID = 0)
        {
            DataTable dtBrand = new DataTable();
            GetBrandinfo.brandID = iSelectedID;
            GetBrandinfo.TenantID = Global.gblTenantID;
            dtBrand = clsBrand.GetBrandMaster(GetBrandinfo);
            if (dtBrand.Rows.Count > 0)
            {
                Comm.LoadControl(cboBrand, dtBrand);
            }
        }
       
        //Description : Load Discount Group To Combobox
        private void LoadDiscountGroup(int iSelectedID = 0)
        {
            DataTable dtDisGrp = new DataTable();
            GetDiscGrpinfo.DiscountGroupID = iSelectedID;
            GetDiscGrpinfo.TenantID = Global.gblTenantID;
            dtDisGrp = clsDiscGrp.GetDiscountGroup(GetDiscGrpinfo);
            if (dtDisGrp.Rows.Count > 0)
            {
                Comm.LoadControl(cboDiscGroup, dtDisGrp);
            }
           // dtDisGrp.Dispose();
        }
        //Description : Load Rack and Product Class Data From Onetime Master To Combobox
        private void LoadFromOneTimeMaster(int iSelectedID = 0, string sOtmType = "")
        {
            DataTable dtOtm = new DataTable();
            GetOtminfo.OtmID = iSelectedID;
            GetOtminfo.OtmType = sOtmType.ToUpper();
            GetOtminfo.TenantID = Global.gblTenantID;
            dtOtm = clsOtm.GetOnetimeMaster(GetOtminfo);
            if (dtOtm.Rows.Count > 0)
            {
                //if (sOtmType.ToUpper() == "ITMRACK")
                //    Comm.LoadControl(cboRack, dtOtm, "", false, false, "OtmData", "OtmID");
                //if (sOtmType.ToUpper() == "PRODCLASS")
                //    Comm.LoadControl(cboProductClass, dtOtm, "", false, false, "OtmData", "OtmID");
            }
        }
        //Description : Load Product Class To Combobox
        private void LoadHSNCode(int iSelectedID = 0)
        {
            DataTable dtItmHSN = new DataTable();
            GetItmMstinfo.ItemID = iSelectedID;
            GetItmMstinfo.TenantID = Global.gblTenantID;
        }

        //Description : Load +PRate and -MRP in Combo
        private void LoadCalPriceDetails(int iSelectedID = 0)
        {
            DataTable dtPriceDetails = new DataTable();
            dtPriceDetails.Clear();

            dtPriceDetails.Columns.Add("CalcID");
            dtPriceDetails.Columns.Add("Calc");

            DataRow dRow1 = dtPriceDetails.NewRow();
            dRow1["CalcID"] = "0";
            dRow1["Calc"] = "+ PRate";
            dtPriceDetails.Rows.Add(dRow1);

            DataRow dRow2 = dtPriceDetails.NewRow();
            dRow2["CalcID"] = "1";
            dRow2["Calc"] = "- " + strMRPName;
            dtPriceDetails.Rows.Add(dRow2);

            DataRow dRow3 = dtPriceDetails.NewRow();
            dRow3["CalcID"] = "2";
            dRow3["Calc"] = "+ CRate";
            dtPriceDetails.Rows.Add(dRow3);

            cboCalc.DataSource = dtPriceDetails;
            cboCalc.DisplayMember = "Calc";
            cboCalc.ValueMember = "CalcID";
        }
        //Description : Load Department To Combobox
        private void LoadDepartment(int iSelectedID = 0)
        {
            DataTable dtDep = new DataTable();
            GetDeptInfo.DepartmentID = iSelectedID;
            GetDeptInfo.TenantID = Global.gblTenantID;
           // dtDep = clsDept.GetDepartment(GetDeptInfo);
            dtDep = Comm.fnGetData("SELECT DepartmentID,Department FROM tblDepartment WHERE DepartmentType=0 and TenantID = " + Global.gblTenantID + " ORDER BY Department Asc").Tables[0];
            if (dtDep.Rows.Count > 0)
            {
                Comm.LoadControl(cboDepmnt, dtDep);
            }
            //dtDep.Dispose();
        }
        //Description : Load Saved data from database from edit window
        private void LoadData(int iItemID)
        {
            DataTable dtLoad = new DataTable();
            GetItmMstinfo.ItemID = iItemID;
            GetItmMstinfo.TenantID = Global.gblTenantID;
            dtLoad = clsItmMst.GetItemMaster(GetItmMstinfo);
            if (dtLoad.Rows.Count > 0)
            {
                txtItemCode.Text = dtLoad.Rows[0]["ItemCode"].ToString();
                txtItemName.Text = dtLoad.Rows[0]["ItemName"].ToString();
                strCheck = dtLoad.Rows[0]["ItemName"].ToString();
                this.txtCategoryList.TextChanged -= this.txtCategoryList_TextChanged;
                txtCategoryList.Text = dtLoad.Rows[0]["Categories"].ToString();
                this.txtCategoryList.TextChanged += this.txtCategoryList_TextChanged;
                txtDescription.Text = dtLoad.Rows[0]["Description"].ToString();
                cboUnit.SelectedValue = dtLoad.Rows[0]["UNITID"].ToString();
                txtMRP.Text = Convert.ToDecimal(dtLoad.Rows[0]["MRP"].ToString()).ToString("#0.00");
                txtROL.Text = dtLoad.Rows[0]["ROL"].ToString();
                this.txtRack.TextChanged -= this.txtRack_TextChanged;
                txtRack.Text = dtLoad.Rows[0]["Rack"].ToString();
                this.txtRack.TextChanged += this.txtRack_TextChanged;
                this.txtManufacturer.TextChanged -= this.txtManufacturer_TextChanged;
                txtManufacturer.Text = dtLoad.Rows[0]["Manufacturer"].ToString();
                this.txtManufacturer.TextChanged += this.txtManufacturer_TextChanged;
                if (Convert.ToInt32(dtLoad.Rows[0]["ActiveStatus"].ToString()) == 1)
                    togglebtnActive.ToggleState = Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active;
                else
                    togglebtnActive.ToggleState = Syncfusion.Windows.Forms.Tools.ToggleButtonState.Inactive;
                cboProductClass.Text = dtLoad.Rows[0]["ProductType"].ToString();
                cboProductClass.SelectedValue = dtLoad.Rows[0]["ProductTypeID"].ToString();
                cboAlterUnit.SelectedValue = dtLoad.Rows[0]["UNITID"].ToString();
                txtAgentCommision.Text = dtLoad.Rows[0]["agentCommPer"].ToString();
                if (Convert.ToInt32(dtLoad.Rows[0]["BlnExpiryItem"].ToString()) == 1)
                    chkExpiryItem.Checked = true;
                else
                    chkExpiryItem.Checked = false;
                txtCoolie.Text = dtLoad.Rows[0]["Coolie"].ToString();
                txtMinRate.Text = dtLoad.Rows[0]["MinRate"].ToString();
                txtMaxRate.Text = dtLoad.Rows[0]["MaxRate"].ToString();
                txtPLUNo.Text = dtLoad.Rows[0]["PLUNo"].ToString();
                txtMOQ.Text = dtLoad.Rows[0]["Minqty"].ToString();
                if (dtLoad.Rows[0]["BatchMode"].ToString() == "")
                    cboBMode.SelectedIndex = 0;
                else
                    cboBMode.SelectedValue = Convert.ToInt32(dtLoad.Rows[0]["BatchMode"].ToString());
                if (dtLoad.Rows[0]["IntNoOrWeight"].ToString() != "")
                {
                    if (Convert.ToInt32(dtLoad.Rows[0]["IntNoOrWeight"].ToString()) == 1)
                        rdoNo.Checked = true;
                    else
                        rdoWeight.Checked = true;
                }
                string strCatIds, strColorIds, strSizeIds;
                strCatIds = dtLoad.Rows[0]["CategoryIDs"].ToString().Substring(1, dtLoad.Rows[0]["CategoryIDs"].ToString().Length - 2);
                string FirstCatID = "";
                lblCategoryIds.Text = strCatIds;
                if (lblCategoryIds.Text != "")
                {
                    int index = lblCategoryIds.Text.IndexOf(',');
                    if (index == -1)
                    {
                        int index1 = lblCategoryIds.Text.IndexOf('|');
                        if (index1 == -1)
                            FirstCatID = lblCategoryIds.Text;
                        else
                            FirstCatID = lblCategoryIds.Text.Substring(0, index1);
                    }
                    else
                    {
                        FirstCatID = lblCategoryIds.Text.Substring(0, index);
                        int index1 = FirstCatID.IndexOf('|');
                        if (index1 != -1)
                            FirstCatID = lblCategoryIds.Text.Substring(0, index1);
                    }
                }
                if (txtCategoryList.Text == "")
                    if (lblCategoryIds.Text != "")
                    {
                        txtCategoryList.TextChanged -= this.txtCategoryList_TextChanged;
                        txtCategoryList.Text = Comm.fnGetData("Select Category From tblcategories where categoryid in (" + FirstCatID + ")").Tables[0].Rows[0][0].ToString();
                        txtCategoryList.TextChanged += this.txtCategoryList_TextChanged;
                    }
                strColorIds = dtLoad.Rows[0]["ColorIDs"].ToString().Substring(1, dtLoad.Rows[0]["ColorIDs"].ToString().Length - 2);
                strSizeIds = dtLoad.Rows[0]["SizeIDs"].ToString().Substring(1, dtLoad.Rows[0]["SizeIDs"].ToString().Length - 2);

                lblCategoryIds.Text = FirstCatID;
                txtCategoryList.Tag = lblCategoryIds.Text;
                txtColor.Tag = strColorIds;
                txtColor.Text = Comm.fnGetData("EXEC UspGetCheckedList '" + txtColor.Tag + "'," + Global.gblTenantID + ",'COLOR'").Tables[0].Rows[0][0].ToString();
                txtSize.Tag = strSizeIds;
                if (strSizeIds == "0")
                    txtSize.Text = "<None>";
                else
                    txtSize.Text = Comm.fnGetData("EXEC UspGetCheckedList '" + txtSize.Tag + "'," + Global.gblTenantID + ",'SIZE'").Tables[0].Rows[0][0].ToString();
                if (dtLoad.Rows[0]["BrandID"].ToString() != "")
                    cboBrand.SelectedValue = Convert.ToDecimal(dtLoad.Rows[0]["BrandID"].ToString());
                if (dtLoad.Rows[0]["DGroupID"].ToString() != "")
                    cboDiscGroup.SelectedValue = Convert.ToDecimal(dtLoad.Rows[0]["DGroupID"].ToString());
                this.txtHSNCode.TextChanged -= this.txtHSNCode_TextChanged;
                txtHSNCode.Tag = dtLoad.Rows[0]["HSNID"].ToString();
                txtHSNCode.Text = dtLoad.Rows[0]["HSNCODE"].ToString();
                if (txtHSNCode.Text == "0")
                    txtHSNCode.Text = "";
                this.txtHSNCode.TextChanged += this.txtHSNCode_TextChanged;

                int taxindex = -1;
                taxindex = cboIGSTPerc.FindStringExact(dtLoad.Rows[0]["IGSTTaxPer"].ToString());
                if (taxindex >= 0)
                {
                    cboIGSTPerc.SelectedIndex = taxindex;
                    SplitTaxPercentages();
                }
                else if (taxindex == -1)
                {
                    cboIGSTPerc.SelectedIndex = -1;
                    MessageBox.Show("IGST Tax percentage is not found in the list. Please be sure to select the appropriate tax percentage before saving.", "Item", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                }
                //cboIGSTPerc.Text = Convert.ToDecimal(dtLoad.Rows[0]["IGSTTaxPer"].ToString()).ToString("#0.00");

                txtSGSTPerc.Text = Convert.ToDecimal(dtLoad.Rows[0]["SGSTTaxPer"].ToString()).ToString("#0.00");
                txtCGSTPerc.Text = Convert.ToDecimal(dtLoad.Rows[0]["CGSTTaxPer"].ToString()).ToString("#0.00");
                txtCessPerc.Text = Convert.ToDecimal(dtLoad.Rows[0]["CessPer"].ToString()).ToString("#0.00");
                txtCompCessPer.Text = Convert.ToDecimal(dtLoad.Rows[0]["CompCessQty"].ToString()).ToString("#0.00");
                if (dtLoad.Rows[0]["PRate"].ToString() == "")
                    txtPRate.Text = "0";
                else
                    txtPRate.Text = Convert.ToDecimal(dtLoad.Rows[0]["PRate"].ToString()).ToString("#0.00");

                if (dtLoad.Rows[0]["SrateCalcMode"].ToString() != "")
                    cboCalc.SelectedValue = Convert.ToInt32(dtLoad.Rows[0]["SrateCalcMode"].ToString());

                if (AppSettings.BLNBARCODE == true)
                {
                    this.txtPerc1.TextChanged -= this.txtPerc1_TextChanged;
                    txtPerc1.Text = Convert.ToDecimal(dtLoad.Rows[0]["Srate1Per"].ToString()).ToString("#0.00");
                    this.txtPerc1.TextChanged += this.txtPerc1_TextChanged;
                    this.txtPerc2.TextChanged -= this.txtPerc2_TextChanged;
                    txtPerc2.Text = Convert.ToDecimal(dtLoad.Rows[0]["Srate2Per"].ToString()).ToString("#0.00");
                    this.txtPerc2.TextChanged += this.txtPerc2_TextChanged;
                    this.txtPerc3.TextChanged -= this.txtPerc3_TextChanged;
                    txtPerc3.Text = Convert.ToDecimal(dtLoad.Rows[0]["Srate3Per"].ToString()).ToString("#0.00");
                    this.txtPerc3.TextChanged += this.txtPerc3_TextChanged;
                    this.txtPerc4.TextChanged -= this.txtPerc4_TextChanged;
                    txtPerc4.Text = Convert.ToDecimal(dtLoad.Rows[0]["Srate4Per"].ToString()).ToString("#0.00");
                    this.txtPerc4.TextChanged += this.txtPerc4_TextChanged;
                    this.txtPerc5.TextChanged -= this.txtPerc5_TextChanged;
                    txtPerc5.Text = Convert.ToDecimal(dtLoad.Rows[0]["Srate5Per"].ToString()).ToString("#0.00");
                    this.txtPerc5.TextChanged += this.txtPerc5_TextChanged;

                    txtSRate1.Text = Convert.ToDecimal(dtLoad.Rows[0]["SRate1"].ToString()).ToString("#0.00");
                    txtSRate2.Text = Convert.ToDecimal(dtLoad.Rows[0]["SRate2"].ToString()).ToString("#0.00");
                    txtSRate3.Text = Convert.ToDecimal(dtLoad.Rows[0]["SRate3"].ToString()).ToString("#0.00");
                    txtSRate4.Text = Convert.ToDecimal(dtLoad.Rows[0]["SRate4"].ToString()).ToString("#0.00");
                    txtSRate5.Text = Convert.ToDecimal(dtLoad.Rows[0]["SRate5"].ToString()).ToString("#0.00");
                }
                else
                {
                    sqlControl rs = new sqlControl();
                    rs.Open("Select Srate1,Srate2,Srate3,Srate4,Srate5 From tblStock Where Itemid=" + Convert.ToDecimal(dtLoad.Rows[0]["ItemID"].ToString()));
                    if (!rs.eof())
                    {
                        txtSRate1.Text = Convert.ToDecimal(rs.fields("SRate1").ToString()).ToString("#0.00");
                        txtSRate2.Text = Convert.ToDecimal(rs.fields("SRate2").ToString()).ToString("#0.00");
                        txtSRate3.Text = Convert.ToDecimal(rs.fields("SRate3").ToString()).ToString("#0.00");
                        txtSRate4.Text = Convert.ToDecimal(rs.fields("SRate4").ToString()).ToString("#0.00");
                        txtSRate5.Text = Convert.ToDecimal(rs.fields("SRate5").ToString()).ToString("#0.00");

                        txtPerc1.Text = CalculationInPriceDetails(Convert.ToDecimal(txtSRate1.Text), false).ToString("#0.00");
                        txtPerc2.Text = CalculationInPriceDetails(Convert.ToDecimal(txtSRate2.Text), false).ToString("#0.00");
                        txtPerc3.Text = CalculationInPriceDetails(Convert.ToDecimal(txtSRate3.Text), false).ToString("#0.00");
                        txtPerc4.Text = CalculationInPriceDetails(Convert.ToDecimal(txtSRate4.Text), false).ToString("#0.00");
                        txtPerc5.Text = CalculationInPriceDetails(Convert.ToDecimal(txtSRate5.Text), false).ToString("#0.00");
                    }
                }
                txtBarcode.Text = dtLoad.Rows[0]["Batchcode"].ToString();
                if (Convert.ToDecimal(dtLoad.Rows[0]["AltUnitID"].ToString()) == 0)
                    cboAlterUnit.SelectedIndex = -1;
                else
                    cboAlterUnit.SelectedValue = dtLoad.Rows[0]["AltUnitID"].ToString();
                txtUnitCFactor.Text = Convert.ToDecimal(dtLoad.Rows[0]["ConvFactor"].ToString()).ToString("#0.00");
                cboCalc.SelectedIndex = Convert.ToInt32(dtLoad.Rows[0]["SrateCalcMode"].ToString());
                if (Convert.ToInt32(cboBMode.SelectedValue.ToString().TrimStart().TrimEnd()) == 0)
                {
                    txtBarcode.Enabled = false;
                    txtPLUNo.Enabled = false;
                    rdoNo.Enabled = false;
                    rdoWeight.Enabled = false;
                    chkExpiryItem.Enabled = false;
                }
                txtshelflife.Text = dtLoad.Rows[0]["Shelflife"].ToString();
                if (Convert.ToInt32(dtLoad.Rows[0]["SRateInclusive"].ToString()) == 1)
                    chkSRateIncl.Checked = true;
                else
                    chkSRateIncl.Checked = false;
                if (Convert.ToInt32(dtLoad.Rows[0]["PRateInclusive"].ToString()) == 1)
                    chkPRateIncl.Checked = true;
                else
                    chkPRateIncl.Checked = false;
                if (Convert.ToInt32(dtLoad.Rows[0]["Slabsystem"].ToString()) == 1)
                    chkSlabSysytem.Checked = true;
                else
                    chkSlabSysytem.Checked = false;
                iAction = 1;
                txtDiscPerc.Text = Convert.ToDecimal(dtLoad.Rows[0]["DiscPer"].ToString()).ToString("#0.00");
                dCatDiscPer= Convert.ToDecimal(dtLoad.Rows[0]["iCatDiscPer"].ToString());
                dManfDiscPer = Convert.ToDecimal(dtLoad.Rows[0]["imanDiscPer"].ToString());

                txtManufacturer.Tag= dtLoad.Rows[0]["MnfID"].ToString();
                cboBrand.SelectedValue = dtLoad.Rows[0]["BrandID"].ToString();
                cboBrand.Tag = dtLoad.Rows[0]["BrandID"].ToString();
                cboUnit.SelectedValue = dtLoad.Rows[0]["UNITID"].ToString();
                cboUnit.Tag = dtLoad.Rows[0]["UNITID"].ToString();
                cboDiscGroup.Tag = dtLoad.Rows[0]["DGroupID"].ToString();
                txtColor.Tag= dtLoad.Rows[0]["ColorIDs"].ToString().Substring(1, dtLoad.Rows[0]["ColorIDs"].ToString().Length - 2);
                txtSize.Tag = dtLoad.Rows[0]["SizeIDs"].ToString().Substring(1, dtLoad.Rows[0]["SizeIDs"].ToString().Length - 2);

                cboDepmnt.SelectedValue = dtLoad.Rows[0]["DepartmentID"].ToString();
                cboDepmnt.Tag = dtLoad.Rows[0]["DepartmentID"].ToString();
                txtDefaultExpDays.Text = dtLoad.Rows[0]["DefaultExpInDays"].ToString();
            }
        }
        //Description : Save and Update Functionalities to the Database
        private void SaveData()
        {
            if (ItemValidate() == true)
            {
                int iActive = 0, iExpItem = 0, iIsIntNo = 0, iHSN = 0, iSRIncl = 0, iPRIncl = 0, iSlabsys = 0;
                string strRet = "", sCatIds = "";
                string[] strResult;
                decimal dCostRateInc = 0, dCostRateExcl = 0, dPRateIncl = 0, dPRateExcl = 0;
                DataTable dtUspIt = new DataTable();

                if (togglebtnActive.ToggleState == Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active)
                    iActive = 1;
                if (chkExpiryItem.Checked == true)
                    iExpItem = 1;
                if (rdoNo.Checked == true)
                    iIsIntNo = 1;
                if (chkSRateIncl.Checked == true)
                    iSRIncl = 1;
                if (chkPRateIncl.Checked == true)
                    iPRIncl = 1;
                if (chkSlabSysytem.Checked == true)
                    iSlabsys = 1;

                if (txtPRate.Text == "") txtPRate.Text = "0";
                if (txtMRP.Text == "") txtMRP.Text = "0";
                if (txtSRate1.Text == "") txtSRate1.Text = "0";
                if (txtSRate2.Text == "") txtSRate2.Text = "0";
                if (txtSRate3.Text == "") txtSRate3.Text = "0";
                if (txtSRate4.Text == "") txtSRate4.Text = "0";
                if (txtSRate5.Text == "") txtSRate5.Text = "0";
                
                if (cboIGSTPerc.Text == "")
                {
                    int taxindex = -1;
                    taxindex = cboIGSTPerc.FindStringExact("0");
                    if (taxindex >= 0)
                    {
                        cboIGSTPerc.SelectedIndex = taxindex;
                        SplitTaxPercentages();
                    }
                    else if (taxindex == -1)
                    {
                        cboIGSTPerc.SelectedIndex = 0;
                    }

                }

                if (Convert.ToDecimal(Comm.Val(cboIGSTPerc.Text)) > 0)
                {
                    if (chkPRateIncl.Checked == true)
                    {
                        dPRateIncl = Convert.ToDecimal(txtPRate.Text);
                        dPRateExcl = (Convert.ToDecimal(txtPRate.Text) / (100 + Convert.ToDecimal(cboIGSTPerc.Text)) * 100);
                    }
                    else
                    {
                        dPRateIncl = Convert.ToDecimal(txtPRate.Text);
                        dPRateExcl = (Convert.ToDecimal(txtPRate.Text) / (100 + Convert.ToDecimal(cboIGSTPerc.Text)) * 100);
                    }
                    if (chkSRateIncl.Checked == true)
                    {
                        dCostRateInc = Convert.ToDecimal(txtPRate.Text);
                        dCostRateExcl = (Convert.ToDecimal(txtPRate.Text) / (100 + Convert.ToDecimal(cboIGSTPerc.Text)) * 100);
                    }
                    else
                    {
                        dCostRateInc = Convert.ToDecimal(txtPRate.Text);
                        dCostRateExcl = (Convert.ToDecimal(txtPRate.Text) / (100 + Convert.ToDecimal(cboIGSTPerc.Text)) * 100);
                    }
                }

                decimal pratecheck = (Convert.ToDecimal(txtPRate.Text) / (100 + Convert.ToDecimal(cboIGSTPerc.Text)) * 100);
                decimal mrpcheck = (Convert.ToDecimal(txtMRP.Text) / (100 + Convert.ToDecimal(cboIGSTPerc.Text)) * 100);
                decimal srate1check = (Convert.ToDecimal(txtSRate1.Text) / (100 + Convert.ToDecimal(cboIGSTPerc.Text)) * 100);
                decimal srate2check = (Convert.ToDecimal(txtSRate2.Text) / (100 + Convert.ToDecimal(cboIGSTPerc.Text)) * 100);
                decimal srate3check = (Convert.ToDecimal(txtSRate3.Text) / (100 + Convert.ToDecimal(cboIGSTPerc.Text)) * 100);
                decimal srate4check = (Convert.ToDecimal(txtSRate4.Text) / (100 + Convert.ToDecimal(cboIGSTPerc.Text)) * 100);
                decimal srate5check = (Convert.ToDecimal(txtSRate5.Text) / (100 + Convert.ToDecimal(cboIGSTPerc.Text)) * 100);

                if (mBLNRATECHECK == true)
                {
                    if (pratecheck > 0 || mrpcheck > 0)
                    {
                        if (mrpcheck <= pratecheck)
                            if (MessageBox.Show("Maximum retail price is less than or equal to purchase rate. Do you wish to save anyway?", "Item", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No)
                            { txtMRP.Focus(); return; }

                        if (srate1check <= pratecheck)
                            if (MessageBox.Show(lblSR1.Text + " is less than or equal to purchase rate. Do you wish to save anyway?", "Item", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No)
                            { txtSRate1.Focus(); return; }

                        if (AppSettings.IsActiveSRate2 == true)
                            if (srate2check <= pratecheck)
                            if (MessageBox.Show(lblSR2.Text + " is less than or equal to purchase rate. Do you wish to save anyway?", "Item", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No)
                            { txtSRate2.Focus(); return; }

                        if (AppSettings.IsActiveSRate3 == true)
                            if (srate3check <= pratecheck)
                            if (MessageBox.Show(lblSR3.Text + " is less than or equal to purchase rate. Do you wish to save anyway?", "Item", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No)
                            { txtSRate3.Focus(); return; }

                        if (AppSettings.IsActiveSRate4 == true)
                            if (srate4check <= pratecheck)
                            if (MessageBox.Show(lblSR4.Text + " is less than or equal to purchase rate. Do you wish to save anyway?", "Item", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No)
                            { txtSRate4.Focus(); return; }

                        if (AppSettings.IsActiveSRate5 == true)
                            if (srate5check <= pratecheck)
                            if (MessageBox.Show(lblSR5.Text + " is less than or equal to purchase rate. Do you wish to save anyway?", "Item", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No)
                            { txtSRate5.Focus(); return; }

                        if (srate1check > mrpcheck)
                            if (MessageBox.Show(lblSR1.Text + " is greater MRP. Do you wish to save anyway?", "Item", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No)
                            { txtSRate1.Focus(); return; }

                        if (AppSettings.IsActiveSRate2 == true)
                            if (srate2check > mrpcheck)
                            if (MessageBox.Show(lblSR2.Text + " is greater MRP. Do you wish to save anyway?", "Item", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No)
                            { txtSRate2.Focus(); return; }

                        if (AppSettings.IsActiveSRate3 == true)
                            if (srate3check > mrpcheck)
                            if (MessageBox.Show(lblSR3.Text + " is greater MRP. Do you wish to save anyway?", "Item", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No)
                            { txtSRate3.Focus(); return; }

                        if (AppSettings.IsActiveSRate4 == true)
                            if (srate4check > mrpcheck)
                            if (MessageBox.Show(lblSR4.Text + " is greater MRP. Do you wish to save anyway?", "Item", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No)
                            { txtSRate4.Focus(); return; }

                        if (AppSettings.IsActiveSRate5 == true)
                            if (srate5check > mrpcheck)
                            if (MessageBox.Show(lblSR5.Text + " is greater MRP. Do you wish to save anyway?", "Item", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No)
                            { txtSRate5.Focus(); return; }
                    }
                }

                if (cboBMode.Visible == true)
                {
                    if (mrpcheck == 0 && Convert.ToInt32(cboBMode.SelectedValue.ToString().TrimStart().TrimEnd()) > 0)
                    {
                        MessageBox.Show("MRP cannot be zero if barcode is enabled. Please provide valid MRP.", "Item", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        { txtMRP.Focus(); return; }
                    }
                }

                if (string.IsNullOrEmpty(txtDefaultExpDays.Text))
                    txtDefaultExpDays.Text = "0";

                if (cboBMode.SelectedIndex == 0)
                {
                    if (txtBarcode.Text == "")
                        txtBarcode.Text = txtItemCode.Text;
                }

                DateTime dtExp;
                double dExpDays = Convert.ToDouble(txtDefaultExpDays.Text);
                if (chkExpiryItem.Checked == true && dExpDays > 0)
                    dtExp = DateTime.Today.AddDays(dExpDays);
                else
                    dtExp = DateTime.Today.AddYears(10);

                DataTable dtBatchUniq = new DataTable();
                if (cboBMode.Text.ToUpper() == "<AUTO BARCODE>")
                {
                    txtBarcode.Text = "<Auto Barcode>";

                    //dtBatchUniq = Comm.fnGetData("EXEC UspGetBatchCodeWhenAutoBarcode " + Convert.ToDecimal(itemInsertInfo.ItemID) + ",'" + "<Auto Barcode>" + "',''," + Convert.ToDecimal(txtMRP.Text.ToString()) + ",'" + Convert.ToDateTime(dtExp.ToString("dd-MMM-yyyy")) + "'," + Global.gblTenantID + "").Tables[0];
                    //if (dtBatchUniq.Rows.Count > 0)
                    //{
                    //    txtBarcode.Text = dtBatchUniq.Rows[0]["BatchUniq"].ToString();
                    //}
                }

                if (txtBarcode.Text.ToString().Trim() == "0" || txtBarcode.Text.ToString().Trim() == "")
                {
                    MessageBox.Show("Invalid barcode. Please enter a valid barcode to continue.", "Item", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    txtBarcode.Enabled = true;
                    txtBarcode.Focus();
                    return;
                }

                if (chkPRateIncl.Checked == true)
                {
                    dPRateIncl = Convert.ToDecimal(txtPRate.Text);
                    dPRateExcl = (Convert.ToDecimal(txtPRate.Text) / (100 + Convert.ToDecimal(cboIGSTPerc.Text)) * 100);
                }
                else
                {
                    dPRateIncl = Convert.ToDecimal(txtPRate.Text);
                    dPRateExcl = (Convert.ToDecimal(txtPRate.Text) / (100 + Convert.ToDecimal(cboIGSTPerc.Text)) * 100);
                }
                if (chkSRateIncl.Checked == true)
                {
                    dCostRateInc = Convert.ToDecimal(txtPRate.Text);
                    dCostRateExcl = (Convert.ToDecimal(txtPRate.Text) / (100 + Convert.ToDecimal(cboIGSTPerc.Text)) * 100);
                }
                else
                {
                    dCostRateInc = Convert.ToDecimal(txtPRate.Text);
                    dCostRateExcl = (Convert.ToDecimal(txtPRate.Text) / (100 + Convert.ToDecimal(cboIGSTPerc.Text)) * 100);
                }

                if (iAction == 0)
                    itemInsertInfo.ItemID = Comm.gfnGetNextSerialNo("tblItemMaster", "ItemID");
                else
                    itemInsertInfo.ItemID = iIDFromEditWindow;

                string FirstCatID = "";
                lblCategoryIds.Text = Convert.ToString(txtCategoryList.Tag);
                if (lblCategoryIds.Text != "")
                {
                    int index = lblCategoryIds.Text.IndexOf(',');
                    if (index == -1)
                    {
                        int index1 = lblCategoryIds.Text.IndexOf('|');
                        if (index1 == -1)
                            FirstCatID = lblCategoryIds.Text.ToString();
                        else
                            FirstCatID = lblCategoryIds.Text.Substring(0, index1);
                    }
                    else
                    {
                        FirstCatID = lblCategoryIds.Text.Substring(0, index);
                    }
                }

                if (cboCalc.SelectedIndex == -1)
                    cboCalc.SelectedIndex = 0;

                if (txtCoolie.Text == "") txtCoolie.Text = "0";
                this.txtHSNCode.TextChanged -= this.txtHSNCode_TextChanged;
                if (txtHSNCode.Text == "") txtHSNCode.Text = "0";
                this.txtHSNCode.TextChanged += this.txtHSNCode_TextChanged;
                if (txtPLUNo.Text == "") txtPLUNo.Text = "0";

                if (txtMOQ.Text == "") txtMOQ.Text = "0";
                if (txtManufacturer.Tag.ToString() == "") txtManufacturer.Tag = "1";

                itemInsertInfo.ItemCode = txtItemCode.Text.TrimStart().TrimEnd();
                itemInsertInfo.ItemName = txtItemName.Text.TrimStart().TrimEnd();
                itemInsertInfo.CategoryID = Convert.ToDecimal(FirstCatID);
                itemInsertInfo.Description = txtDescription.Text.TrimStart().TrimEnd();
                itemInsertInfo.PRate = Convert.ToDecimal(Comm.Val(txtPRate.Text));
                itemInsertInfo.SrateCalcMode = Convert.ToInt32(cboCalc.SelectedIndex);
                itemInsertInfo.CRateAvg = 0;
                itemInsertInfo.Srate1Per = Convert.ToDecimal(Comm.Val(txtPerc1.Text));
                itemInsertInfo.SRate1 = Convert.ToDecimal(Comm.Val(txtSRate1.Text));
                itemInsertInfo.Srate2Per = Convert.ToDecimal(Comm.Val(txtPerc2.Text));
                itemInsertInfo.SRate2 = Convert.ToDecimal(Comm.Val(txtSRate2.Text));
                itemInsertInfo.Srate3Per = Convert.ToDecimal(Comm.Val(txtPerc3.Text));
                itemInsertInfo.SRate3 = Convert.ToDecimal(Comm.Val(txtSRate3.Text));
                itemInsertInfo.Srate4Per = Convert.ToDecimal(Comm.Val(txtPerc4.Text));
                itemInsertInfo.Srate4 = Convert.ToDecimal(Comm.Val(txtSRate4.Text));
                itemInsertInfo.Srate5Per = Convert.ToDecimal(Comm.Val(txtPerc5.Text));
                itemInsertInfo.SRate5 = Convert.ToDecimal(Comm.Val(txtSRate5.Text));
                itemInsertInfo.MRP = Convert.ToDecimal(Comm.Val(txtMRP.Text));
                itemInsertInfo.ROL = Convert.ToDecimal(Comm.Val(txtROL.Text));
                this.txtRack.TextChanged -= this.txtRack_TextChanged;
                itemInsertInfo.Rack = txtRack.Text.TrimStart().TrimEnd();
                this.txtRack.TextChanged += this.txtRack_TextChanged;
                if (txtManufacturer.Text == "")
                    SetDefaultValue();
                itemInsertInfo.Manufacturer = txtManufacturer.Text;
                itemInsertInfo.ActiveStatus = iActive;
                itemInsertInfo.IntLocal = 0;
                itemInsertInfo.ProductType = cboProductClass.Text.TrimStart().TrimEnd();
                itemInsertInfo.ProductTypeID = Convert.ToDecimal(cboProductClass.SelectedValue);
                itemInsertInfo.LedgerID = 0;
                //itemInsertInfo.UNITID = Convert.ToDecimal(sfcboUnit.SelectedValue);
                itemInsertInfo.UNITID = Convert.ToDecimal(cboUnit.SelectedValue);
                itemInsertInfo.Notes = "";
                itemInsertInfo.agentCommPer = Convert.ToDecimal(Comm.Val(txtAgentCommision.Text));

                if (AppSettings.BLNBARCODE == true)
                {
                    itemInsertInfo.BatchMode = cboBMode.SelectedValue.ToString().TrimStart().TrimEnd();
                    itemInsertInfo.BatchCode = txtBarcode.Text;
                    itemInsertInfo.BlnExpiryItem = iExpItem;
                    itemInsertInfo.blnExpiry = iExpItem;
                    itemInsertInfo.DefaultExpInDays = Convert.ToDecimal(txtDefaultExpDays.Text);
                    itemInsertInfo.PLUNo = Convert.ToDecimal(txtPLUNo.Text);
                    itemInsertInfo.IntNoOrWeight = iIsIntNo;
                    if (string.IsNullOrEmpty(txtshelflife.Text))
                        itemInsertInfo.Shelflife = 0;
                    else
                        itemInsertInfo.Shelflife = Convert.ToDecimal(txtshelflife.Text);
                }
                else
                {
                    itemInsertInfo.BatchMode = "0".ToString().TrimStart().TrimEnd();
                    itemInsertInfo.BatchCode = txtBarcode.Text; // itemInsertInfo.ItemCode;
                    itemInsertInfo.BlnExpiryItem = 0;
                    itemInsertInfo.blnExpiry = 0;
                    itemInsertInfo.DefaultExpInDays = Convert.ToDecimal("7300");
                    itemInsertInfo.PLUNo = Convert.ToDecimal("0");
                    itemInsertInfo.IntNoOrWeight = 0;
                    itemInsertInfo.Shelflife = 0;
                }

                itemInsertInfo.Coolie = Convert.ToDecimal(txtCoolie.Text);
                itemInsertInfo.FinishedGoodID = 0;
                itemInsertInfo.MinRate = Convert.ToDecimal(Comm.Val(txtMinRate.Text));
                itemInsertInfo.MaxRate = Convert.ToDecimal(Comm.Val(txtMaxRate.Text));
                itemInsertInfo.HSNID = Comm.ToDecimal(txtHSNCode.Tag);
                itemInsertInfo.iCatDiscPer = dCatDiscPer;
                itemInsertInfo.IPGDiscPer = 0;
                itemInsertInfo.ImanDiscPer = dManfDiscPer;
                itemInsertInfo.ItemNameUniCode = "";
                itemInsertInfo.Minqty = Convert.ToDecimal(Comm.Val(txtMOQ.Text));
                itemInsertInfo.MNFID = Convert.ToDecimal(txtManufacturer.Tag);
                itemInsertInfo.ItemCodeUniCode = "";
                itemInsertInfo.UPC = "";
                itemInsertInfo.Qty = 0;
                itemInsertInfo.MaxQty = 0;
                itemInsertInfo.SystemName = Environment.MachineName;
                itemInsertInfo.UserID = Global.gblUserID;
                itemInsertInfo.LastUpdateDate = DateTime.Today;
                itemInsertInfo.LastUpdateTime = DateTime.Today;
                itemInsertInfo.TenantID = Global.gblTenantID;
                itemInsertInfo.blnCessOnTax = 0;
                itemInsertInfo.CompCessQty = Convert.ToDecimal(Comm.Val(txtCompCessPer.Text));
                itemInsertInfo.CGSTTaxPer = Convert.ToDecimal(Comm.Val(txtCGSTPerc.Text));
                itemInsertInfo.SGSTTaxPer = Convert.ToDecimal(Comm.Val(txtSGSTPerc.Text));
                itemInsertInfo.IGSTTaxPer = Convert.ToDecimal(Comm.Val(cboIGSTPerc.Text));
                itemInsertInfo.CessPer = Convert.ToDecimal(Comm.Val(txtCessPerc.Text));
                itemInsertInfo.VAT = Convert.ToDecimal(Comm.Val(cboIGSTPerc.Text));
               
                itemInsertInfo.CategoryIDs = "," + lblCategoryIds.Text + ",";
                if (txtColor.Text == "")
                    txtColor.Tag = " ";
                itemInsertInfo.ColorIDs = "," + txtColor.Tag + ",";
                if (txtSize.Text == "")
                    txtSize.Tag = " ";
                    itemInsertInfo.SizeIDs = "," + txtSize.Tag + ",";
                itemInsertInfo.BrandDisPer = 0;
                if (cboDiscGroup.SelectedValue == null)
                    itemInsertInfo.DGroupID = 1;
                else
                    itemInsertInfo.DGroupID = Convert.ToDecimal(cboDiscGroup.SelectedValue);

                itemInsertInfo.DGroupDisPer = 0;
                itemInsertInfo.CostRateInc = dCostRateInc;
                itemInsertInfo.CostRateExcl = dCostRateExcl;
                itemInsertInfo.PRateExcl = dPRateExcl;
                itemInsertInfo.PrateInc = dPRateIncl;
                if (cboBrand.SelectedValue == null)
                    itemInsertInfo.BrandID = 1;
                else
                    itemInsertInfo.BrandID = Convert.ToDecimal(cboBrand.SelectedValue);
                if (cboAlterUnit.SelectedValue == null)
                    itemInsertInfo.AltUnitID = 0;
                else
                    itemInsertInfo.AltUnitID = Convert.ToDecimal(cboAlterUnit.SelectedValue);
                if (txtUnitCFactor.Text == "") txtUnitCFactor.Text = "0";
                itemInsertInfo.ConvFactor = Convert.ToDecimal(txtUnitCFactor.Text);
                itemInsertInfo.SRIncl = iSRIncl;
                itemInsertInfo.PRIncl = iPRIncl;
                itemInsertInfo.SlabSys = iSlabsys;
                if (string.IsNullOrEmpty(txtDiscPerc.Text))
                    txtDiscPerc.Text = "0";
                itemInsertInfo.DiscPer = Convert.ToDecimal(txtDiscPerc.Text);
                if (cboDepmnt.SelectedValue == null)
                    itemInsertInfo.DepartmentID = 1;
                else
                    itemInsertInfo.DepartmentID = Convert.ToDecimal(cboDepmnt.SelectedValue);

                //if (cboBMode.SelectedValue.ToString().TrimStart().TrimEnd() == "0")
                //{
                //    string strItemCode = txtItemCode.Text.TrimStart().TrimEnd();
                //    itemInsertInfo.ItemCode = strItemCode.Replace(" ", "");
                //}

                strRet = clsItmMst.InsertUpdateDeleteItemMasterInsert(itemInsertInfo, iAction);

                if (cboBMode.Text.ToUpper() != "<AUTO BARCODE>")
                    Comm.StockInsert("STOCKADD", Convert.ToDecimal(itemInsertInfo.ItemID), txtBarcode.Text, 0, Convert.ToDecimal(Comm.Val(txtMRP.Text)), dCostRateInc, dCostRateExcl, dPRateExcl, dPRateIncl, Convert.ToDecimal(Comm.Val(cboIGSTPerc.Text)), Convert.ToDecimal(txtSRate1.Text), Convert.ToDecimal(txtSRate2.Text), Convert.ToDecimal(txtSRate3.Text), Convert.ToDecimal(txtSRate4.Text), Convert.ToDecimal(txtSRate5.Text), Convert.ToInt32(itemInsertInfo.BatchMode), "", DateTime.Today, dtExp, 0, 0, 1, Global.gblTenantID, false, true, chkExpiryItem.Checked, Convert.ToDecimal(Comm.Val(txtPRate.Text)));

                //if (cboBMode.Text.ToUpper() != "<AUTO BARCODE>")
                //    Comm.StockInsert("STOCKADD", Convert.ToInt32(itemInsertInfo.ItemID), txtBarcode.Text, 0, Convert.ToDecimal(Comm.Val(txtMRP.Text)), dCostRateInc, dCostRateExcl, dPRateExcl, dPRateIncl, Convert.ToDecimal(Comm.Val(txtIGSTPerc.Text)), Convert.ToDecimal(txtSRate1.Text), Convert.ToDecimal(txtSRate2.Text), Convert.ToDecimal(txtSRate3.Text), Convert.ToDecimal(txtSRate4.Text), Convert.ToDecimal(txtSRate5.Text), Convert.ToInt32(itemInsertInfo.BatchMode), "", DateTime.Today, dtExp, 0, 0, 1, Global.gblTenantID, false, true, chkExpiryItem.Checked, Convert.ToDecimal(Comm.Val(txtPRate.Text)));
                //Comm.StockInsert("STOCKADD", Convert.ToInt32(itemInsertInfo.ItemID), txtBarcode.Text, 0, Convert.ToDecimal(Comm.Val(txtMRP.Text)), dCostRateInc, dCostRateExcl, dPRateExcl, dPRateIncl, Convert.ToDecimal(Comm.Val(txtIGSTPerc.Text)), Convert.ToDecimal(txtSRate1.Text), Convert.ToDecimal(txtSRate2.Text), Convert.ToDecimal(txtSRate3.Text), Convert.ToDecimal(txtSRate4.Text), Convert.ToDecimal(txtSRate5.Text), Convert.ToInt32(itemInsertInfo.BatchMode), "", DateTime.Today, DateTime.Today.AddYears(8), 0, 0, 1, Global.gblTenantID, false, true, chkExpiryItem.Checked);

                if (strRet.Length > 2)
                {
                    strResult = strRet.Split('|');
                    if (Convert.ToInt32(strResult[0].ToString()) == -1)
                    {
                        if (strResult[1].ToString().ToUpper().Contains("DUPLICATE"))
                        {
                            MessageBox.Show("Can't allow duplicate entry in Item Code (" + txtItemCode.Text + ")", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            txtItemCode.Focus();
                            txtItemCode.SelectAll();
                        }
                        else
                            MessageBox.Show(strResult[1].ToString(), Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else if (CtrlPassed != null)//For Passed Value from this to Another Form Control
                    {
                        if (CtrlPassed is TextBox)
                        {
                            CtrlPassed.Text = txtItemName.Text;
                            CtrlPassed.Tag = itemInsertInfo.ItemID;
                            CtrlPassed.Focus();
                            this.Close();
                        }
                    }
                    else
                    {
                        //Global.SetGFormTransID(Convert.ToInt32(strResult[1].ToString()));
                        //Global.SetGFormTransType(strResult[2].ToString());
                        
                        if (bFromEditWindowItem == true)
                        {
                            if (iAction == 1)
                                this.Close();
                        }
                        ClearFormControls();
                        Cursor.Current = Cursors.WaitCursor;
                        Comm.MessageboxToasted("Item Master", "Item Saved Successfully");
                        Cursor.Current = Cursors.Default;
                    }
                }
                else
                {
                    if (Convert.ToInt32(strRet) == -1)
                        MessageBox.Show("Failed to Save! Contact your Administrator", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    else if (CtrlPassed != null)//For Passed Value from this to Another Form Control
                    {
                        if (CtrlPassed is TextBox)
                        {
                            CtrlPassed.Text = txtItemName.Text;
                            CtrlPassed.Tag = itemInsertInfo.ItemID;
                            CtrlPassed.Focus();
                            this.Close();
                        }
                    }
                    else
                    {
                        if (bFromEditWindowItem == true)
                        {
                            if (iAction == 1)
                            {
                                this.Close();
                            }
                            else

                                ClearFormControls();

                        }
                        else
                            ClearFormControls();
                        Comm.MessageboxToasted("Item Master", "Item Saved Successfully");

                    }
                }
            }
        }
        //Description :  Delete Data from Item Master table
        private void DeleteData()
        {
            int iActive = 0, iExpItem = 0, iIsIntNo = 0, iHSN = 0, iSRIncl = 0, iPRIncl = 0, iSlabsys = 0;
            string strRet = "";
            string[] strResult;
            iAction = 2;
            if (togglebtnActive.ToggleState == Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active)
                iActive = 1;
            if (chkExpiryItem.Checked == true)
                iExpItem = 1;
            if (rdoNo.Checked == true)
                iIsIntNo = 1;

            if (chkSRateIncl.Checked == true)
                iSRIncl = 1;
            if (chkPRateIncl.Checked == true)
                iPRIncl = 1;
            if (chkSlabSysytem.Checked == true)
                iSlabsys = 1;

            if (txtMRP.Text == "")
                txtMRP.Text = "0";
            if (txtROL.Text == "")
                txtROL.Text = "0";
            if (txtAgentCommision.Text == "")
                txtAgentCommision.Text = "0";
            if (txtMinRate.Text == "")
                txtMinRate.Text = "0";
            if (txtMaxRate.Text == "")
                txtMaxRate.Text = "0";
            if (txtMOQ.Text == "")
                txtMOQ.Text = "0";
            if (txtCessPerc.Text == "")
                txtCessPerc.Text = "0";

            itemInsertInfo.ItemID = iIDFromEditWindow;
            itemInsertInfo.ItemCode = txtItemCode.Text;
            itemInsertInfo.ItemName = txtItemName.Text;
            itemInsertInfo.CategoryID = 1;
            itemInsertInfo.Description = txtDescription.Text;
            itemInsertInfo.PRate = Convert.ToDecimal(txtPRate.Text);
            itemInsertInfo.SrateCalcMode = 0;
            itemInsertInfo.CRateAvg = 0;
            itemInsertInfo.Srate1Per = Convert.ToDecimal(txtPerc1.Text);
            itemInsertInfo.SRate1 = Convert.ToDecimal(txtSRate1.Text);
            itemInsertInfo.Srate2Per = Convert.ToDecimal(txtPerc2.Text);
            itemInsertInfo.SRate2 = Convert.ToDecimal(txtSRate2.Text);
            itemInsertInfo.Srate3Per = Convert.ToDecimal(txtPerc3.Text);
            itemInsertInfo.SRate3 = Convert.ToDecimal(txtSRate3.Text);
            itemInsertInfo.Srate4 = Convert.ToDecimal(txtPerc4.Text);
            itemInsertInfo.Srate4Per = Convert.ToDecimal(txtSRate4.Text);
            itemInsertInfo.SRate5 = Convert.ToDecimal(txtPerc5.Text);
            itemInsertInfo.Srate5Per = Convert.ToDecimal(txtSRate5.Text);
            itemInsertInfo.MRP = Convert.ToDecimal(txtMRP.Text);
            itemInsertInfo.ROL = Convert.ToDecimal(txtROL.Text);
            this.txtRack.TextChanged -= this.txtRack_TextChanged;
            itemInsertInfo.Rack = txtRack.Text;
            this.txtRack.TextChanged += this.txtRack_TextChanged;
            itemInsertInfo.Manufacturer = txtManufacturer.Text;
            itemInsertInfo.ActiveStatus = iActive;
            itemInsertInfo.IntLocal = 0;
            itemInsertInfo.ProductType = cboProductClass.Text;
            itemInsertInfo.ProductTypeID = Convert.ToDecimal(cboProductClass.SelectedValue);
            itemInsertInfo.LedgerID = 0;
            //itemInsertInfo.UNITID = Convert.ToInt32(sfcboUnit.SelectedValue);
            itemInsertInfo.UNITID = Convert.ToInt32(cboUnit.SelectedValue);
            itemInsertInfo.Notes = "";
            itemInsertInfo.agentCommPer = Convert.ToDecimal(txtAgentCommision.Text);
            itemInsertInfo.BlnExpiryItem = iExpItem;
            itemInsertInfo.Coolie = Convert.ToInt32(txtCoolie.Text);
            itemInsertInfo.FinishedGoodID = 0;
            itemInsertInfo.MinRate = Convert.ToDecimal(txtMinRate.Text);
            itemInsertInfo.MaxRate = Convert.ToDecimal(txtMaxRate.Text);
            itemInsertInfo.PLUNo = 0;
            itemInsertInfo.HSNID = 0;
            itemInsertInfo.iCatDiscPer = 0;
            itemInsertInfo.IPGDiscPer = 0;
            itemInsertInfo.ImanDiscPer = 0;
            itemInsertInfo.ItemNameUniCode = "";
            itemInsertInfo.Minqty = Convert.ToDecimal(txtMOQ.Text);
            itemInsertInfo.ItemCodeUniCode = "";
            itemInsertInfo.UPC = "";
            itemInsertInfo.BatchMode = cboBMode.Text;
            itemInsertInfo.Qty = 0;
            itemInsertInfo.MaxQty = 0;
            itemInsertInfo.IntNoOrWeight = iIsIntNo;
            itemInsertInfo.SystemName = Environment.MachineName;
            itemInsertInfo.UserID = Global.gblUserID;
            itemInsertInfo.LastUpdateDate = DateTime.Today;
            itemInsertInfo.LastUpdateTime = DateTime.Today;
            itemInsertInfo.TenantID = Global.gblTenantID;
            itemInsertInfo.blnCessOnTax = 0;
            itemInsertInfo.CompCessQty = Convert.ToDecimal(Comm.Val(txtCompCessPer.Text));
            itemInsertInfo.CGSTTaxPer = Convert.ToDecimal(txtCGSTPerc.Text);
            itemInsertInfo.SGSTTaxPer = Convert.ToDecimal(txtSGSTPerc.Text);
            itemInsertInfo.IGSTTaxPer = Convert.ToDecimal(cboIGSTPerc.Text);
            itemInsertInfo.CessPer = Convert.ToDecimal(txtCessPerc.Text);
            itemInsertInfo.VAT = Convert.ToDecimal(cboIGSTPerc.Text);

            itemInsertInfo.CategoryIDs = "";
            itemInsertInfo.ColorIDs = "";
            itemInsertInfo.SizeIDs = "";
            itemInsertInfo.BrandDisPer = 0;
            itemInsertInfo.DGroupID = Convert.ToInt32(cboDiscGroup.Tag);
            itemInsertInfo.DGroupDisPer = 0;
            itemInsertInfo.BatchCode = txtBarcode.Text;
            itemInsertInfo.AltUnitID = 0;
            if (txtUnitCFactor.Text == "") txtUnitCFactor.Text = "0";
            itemInsertInfo.ConvFactor = Convert.ToDecimal(txtUnitCFactor.Text);
            itemInsertInfo.Shelflife = Convert.ToDecimal(txtshelflife.Text);
            itemInsertInfo.DiscPer = 0;
            itemInsertInfo.DepartmentID = 1;

            strRet = clsItmMst.InsertUpdateDeleteItemMasterInsert(itemInsertInfo, iAction);
            if (strRet.Length > 2)
            {
                strResult = strRet.Split('|');
                if (Convert.ToInt32(strResult[0].ToString()) == -1)
                {
                    if (strResult[1].ToString().ToUpper().Contains("DUPLICATE"))
                        MessageBox.Show("Failed to Delete the Item Name (" + txtItemName.Text + ")", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else if (strResult[1].ToString().ToUpper().Contains("CONSTRAINT"))
                        MessageBox.Show("Failed to Delete the Item Name (" + txtItemName.Text + "), It is referencing in someother Area !!", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else
                        MessageBox.Show(strResult[1].ToString(), Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                if (Convert.ToInt32(strRet) == -1)
                    MessageBox.Show("Failed to Delete! Contact your Administrator", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            if (bFromEditWindowItem == true)
            {
                this.Close();
            }
        }
        //Description : Clear Data from Form
        public void ClearFormControls()
        {
            this.txtItemName.Clear();
            txtItemCode.Clear();
            txtDescription.Clear();
            txtAgentCommision.Clear();
            txtDiscPerc.Clear();
            txtCoolie.Clear();
            txtROL.Clear();
            txtMOQ.Clear();
            txtMinRate.Clear();
            txtMaxRate.Clear();
            txtPRate.Clear();
            txtMRP.Clear();
            cboCalc.SelectedIndex = -1;

            txtPerc1.Text = "0";
            txtSRate1.Text = "0";
            txtPerc2.Text = "0";
            txtSRate2.Text = "0";
            txtPerc3.Text = "0";
            txtSRate3.Text = "0";
            txtPerc4.Text = "0";
            txtSRate4.Text = "0";
            txtPerc5.Text = "0";
            txtSRate5.Text = "0";
            txtBarcode.Clear();
            txtPLUNo.Clear();
            chkExpiryItem.Checked = false;
            rdoNo.Checked = false;
            rdoWeight.Checked = false;


            cboAlterUnit.SelectedIndex = 0;
            cboProductClass.SelectedIndex = 0;
            txtUnitCFactor.Clear();
            btnDelete.Enabled = false;
            txtItemName.Focus();
            dManfDiscPer = 0;
            dCatDiscPer = 0;
            lblColorID.Text = "0";
            lblSizeID.Text = "0";
            SetMemmorizeValue();
            SetDefaultValue();
        }
        //Description : Show ItemName and Item Code when write 3 letter in  Itemname Textbox
        public void ShowItemSearchDetailsinGrid(bool blnClose = false)
        {
            if (blnClose == false)
            {
                if (txtItemName.Text.Trim().Length >= 3)
                {
                    string a = txtItemName.Text;
                    string sQuery = "Select ItemName,ItemCode,ItemId From tblItemMaster where ItemName LIKE '" + txtItemName.Text.Replace("'","''").TrimStart().TrimEnd() + "%' And TenantID = '" + Global.gblTenantID + "'";
                    DataTable dtItemExist = Comm.fnGetData(sQuery).Tables[0];
                    if (dtItemExist.Rows.Count > 0)
                    {
                        pnlShowItemSearch.Visible = true;
                        pnlShowItemSearch.Size = new Size(403, 384);
                        grpTaxDetails.Visible = false;
                        grpPriceDetails.Visible = false;

                        dgvShowItemSearch.AutoGenerateColumns = false;
                        dgvShowItemSearch.DataSource = dtItemExist;
                    }
                    else
                    {
                        pnlShowItemSearch.Size = new Size(403, 12);
                        pnlShowItemSearch.Visible = false;
                        grpTaxDetails.Visible = true;
                        grpPriceDetails.Visible = true;
                    }
                }
                else
                {
                    pnlShowItemSearch.Size = new Size(403, 12);
                    pnlShowItemSearch.Visible = false;
                    grpTaxDetails.Visible = true;
                    grpPriceDetails.Visible = true;
                }
            }
            else
            {
                pnlShowItemSearch.Size = new Size(403, 12);
                pnlShowItemSearch.Visible = false;
                grpTaxDetails.Visible = true;
                grpPriceDetails.Visible = true;
            }
        }
        //Description : Load Checkbox List for Show/Hide Controls
        private void ShowControlCheckboxList()
        {
            DataColumn dc = new DataColumn("strDescription", typeof(String));
            DataColumn dc1 = new DataColumn("strKeyName", typeof(String));
            DataColumn dc2 = new DataColumn("bValue", typeof(bool));

            dtCheckBox.Columns.Add(dc);
            dtCheckBox.Columns.Add(dc1);
            dtCheckBox.Columns.Add(dc2);

            if (AppSettings.TaxEnabled == true)
            {
                DataRow dRow28 = dtCheckBox.NewRow();
                dRow28[0] = "Check Rates";
                dRow28[1] = "blnRateCheck";
                dRow28[2] = CheckState.Checked;
                dtCheckBox.Rows.Add(dRow28);
            }

            DataRow dRowSrateInc = dtCheckBox.NewRow();
            dRowSrateInc[0] = "SRate Inclusive";
            dRowSrateInc[1] = "blnSRateInc";
            dRowSrateInc[2] = CheckState.Checked;
            dtCheckBox.Rows.Add(dRowSrateInc);

            DataRow dRowPrateInc = dtCheckBox.NewRow();
            dRowPrateInc[0] = "PRate Inclusive";
            dRowPrateInc[1] = "blnPRateInc";
            dRowPrateInc[2] = CheckState.Unchecked;
            dtCheckBox.Rows.Add(dRowPrateInc);

            DataRow dRow1 = dtCheckBox.NewRow();
            dRow1[0] = "Show Rack";
            dRow1[1] = "blnShowRack";
            dRow1[2] = 1;
            dtCheckBox.Rows.Add(dRow1);

            if (AppSettings.NeedColor == true)
            {
                DataRow dRow2 = dtCheckBox.NewRow();
                dRow2[0] = "Show Color";
                dRow2[1] = "blnShowColor";
                dRow2[2] = CheckState.Unchecked;
                dtCheckBox.Rows.Add(dRow2);
            }

            if (AppSettings.NeedSize == true)
            {
                DataRow dRow3 = dtCheckBox.NewRow();
                dRow3[0] = "Show Size";
                dRow3[1] = "blnShowSize";
                dRow3[2] = CheckState.Unchecked;
                dtCheckBox.Rows.Add(dRow3);
            }

            DataRow dRow4 = dtCheckBox.NewRow();
            dRow4[0] = "Show Department";
            dRow4[1] = "blnShowDepartment";
            dRow4[2] = CheckState.Unchecked;
            dtCheckBox.Rows.Add(dRow4);

            DataRow dRow5 = dtCheckBox.NewRow();
            dRow5[0] = "Show Description";
            dRow5[1] = "blnShowDescription";
            dRow5[2] = CheckState.Unchecked;
            dtCheckBox.Rows.Add(dRow5);

            if (AppSettings.NeedAgent == true)
            {
                DataRow dRow6 = dtCheckBox.NewRow();
                dRow6[0] = "Show Agent Commision";
                dRow6[1] = "blnShowAgentCommision";
                dRow6[2] = CheckState.Checked;
                dtCheckBox.Rows.Add(dRow6);
            }

            DataRow dRow7 = dtCheckBox.NewRow();
            dRow7[0] = "Show Disc %";
            dRow7[1] = "blnShowDiscPer";
            dRow7[2] = CheckState.Checked;
            dtCheckBox.Rows.Add(dRow7);

            DataRow dRow8 = dtCheckBox.NewRow();
            dRow8[0] = "Show Coolie per Qty";
            dRow8[1] = "blnShowCooliePerQty";
            dRow8[2] = CheckState.Unchecked;
            dtCheckBox.Rows.Add(dRow8);

            DataRow dRow9 = dtCheckBox.NewRow();
            dRow9[0] = "Show ROL";
            dRow9[1] = "blnShowROL";
            dRow9[2] = CheckState.Checked;
            dtCheckBox.Rows.Add(dRow9);

            DataRow dRow10 = dtCheckBox.NewRow();
            dRow10[0] = "Show MOQ";
            dRow10[1] = "blnShowMOQ";
            dRow10[2] = CheckState.Unchecked;
            dtCheckBox.Rows.Add(dRow10);

            DataRow dRow11 = dtCheckBox.NewRow();
            dRow11[0] = "Show Min.Rate";
            dRow11[1] = "blnShowMinRate";
            dRow11[2] = CheckState.Unchecked;
            dtCheckBox.Rows.Add(dRow11);

            DataRow dRow12 = dtCheckBox.NewRow();
            dRow12[0] = "Show Max. Rate";
            dRow12[1] = "blnShowMaxRate";
            dRow12[2] = CheckState.Unchecked;
            dtCheckBox.Rows.Add(dRow12);

            DataRow dRow13 = dtCheckBox.NewRow();
            dRow13[0] = "Show Slab System";
            dRow13[1] = "blnShowSlabSystem";
            dRow13[2] = CheckState.Unchecked;
            dtCheckBox.Rows.Add(dRow13);

            DataRow dRow14 = dtCheckBox.NewRow();
            dRow14[0] = "Show Shelf Life";
            dRow14[1] = "blnShowShelfLife";
            dRow14[2] = CheckState.Unchecked;
            dtCheckBox.Rows.Add(dRow14);

            DataRow dRow15 = dtCheckBox.NewRow();
            dRow15[0] = "Show Alter Unit";
            dRow15[1] = "blnShowAlterUnit";
            dRow15[2] = CheckState.Unchecked;
            dtCheckBox.Rows.Add(dRow15);

            DataRow dRow16 = dtCheckBox.NewRow();
            dRow16[0] = "Show Product Class";
            dRow16[1] = "blnShowProductClass";
            dRow16[2] = CheckState.Unchecked;
            dtCheckBox.Rows.Add(dRow16);

            DataRow dRow17 = dtCheckBox.NewRow();
            dRow17[0] = "Memmorize Category";
            dRow17[1] = "blnMemmorizeCategory";
            dRow17[2] = CheckState.Checked;
            dtCheckBox.Rows.Add(dRow17);

            DataRow dRow18 = dtCheckBox.NewRow();
            dRow18[0] = "Memmorize Manufacturer";
            dRow18[1] = "blnMemmorizeManufacturer";
            dRow18[2] = CheckState.Checked;
            dtCheckBox.Rows.Add(dRow18);

            DataRow dRow19 = dtCheckBox.NewRow();
            dRow19[0] = "Memmorize Unit";
            dRow19[1] = "blnMemmorizeUnit";
            dRow19[2] = CheckState.Checked;
            dtCheckBox.Rows.Add(dRow19);

            DataRow dRow20 = dtCheckBox.NewRow();
            dRow20[0] = "Memmorize Rack";
            dRow20[1] = "blnMemmorizeRack";
            dRow20[2] = CheckState.Checked;
            dtCheckBox.Rows.Add(dRow20);

            if (AppSettings.NeedColor == true)
            {
                DataRow dRow21 = dtCheckBox.NewRow();
                dRow21[0] = "Memmorize Color";
                dRow21[1] = "blnMemmorizeColor";
                dRow21[2] = CheckState.Unchecked;
                dtCheckBox.Rows.Add(dRow21);
            }
            if (AppSettings.NeedSize == true)
            {
                DataRow dRow22 = dtCheckBox.NewRow();
                dRow22[0] = "Memmorize Size";
                dRow22[1] = "blnMemmorizeSize";
                dRow22[2] = CheckState.Unchecked;
                dtCheckBox.Rows.Add(dRow22);
            }
            if (AppSettings.NeedBrand == true)
            {
                DataRow dRow23 = dtCheckBox.NewRow();
                dRow23[0] = "Memmorize Brand";
                dRow23[1] = "blnMemmorizeBrand";
                dRow23[2] = CheckState.Checked;
                dtCheckBox.Rows.Add(dRow23);
            }

            DataRow dRow24 = dtCheckBox.NewRow();
            dRow24[0] = "Memmorize Disc. Group";
            dRow24[1] = "blnMemmorizeDiscGroup";
            dRow24[2] = CheckState.Checked;
            dtCheckBox.Rows.Add(dRow24);

            DataRow dRow25 = dtCheckBox.NewRow();
            dRow25[0] = "Memmorize Department";
            dRow25[1] = "blnMemmorizeDepartment";
            dRow25[2] = CheckState.Checked;
            dtCheckBox.Rows.Add(dRow25);

            if (AppSettings.TaxEnabled == true)
            {
                DataRow dRow26 = dtCheckBox.NewRow();
                dRow26[0] = "Memmorize HSN Code and Tax";
                dRow26[1] = "blnMemmorizeHSNCode";
                dRow26[2] = CheckState.Unchecked;
                dtCheckBox.Rows.Add(dRow26);
            }
            if (AppSettings.TaxEnabled == true)
            {
                DataRow dRow27 = dtCheckBox.NewRow();
                dRow27[0] = "Memmorize S.Rate Inc";
                dRow27[1] = "blnMemmorizeSRateInc";
                dRow27[2] = CheckState.Checked;
                dtCheckBox.Rows.Add(dRow27);
            }
            if (AppSettings.TaxEnabled == true)
            {
                DataRow dRow28 = dtCheckBox.NewRow();
                dRow28[0] = "Memmorize PRate Inc";
                dRow28[1] = "blnMemmorizePRateInc";
                dRow28[2] = CheckState.Checked;
                dtCheckBox.Rows.Add(dRow28);
            }

            DataRow dRow29 = dtCheckBox.NewRow();
            dRow29[0] = "Memmorize BatchCode Mode";
            dRow29[1] = "blnMemmorizeBatchCodeMode";
            dRow29[2] = CheckState.Checked;
            dtCheckBox.Rows.Add(dRow29);

            DataRow dRow30 = dtCheckBox.NewRow();
            dRow30[0] = "Memmorize Shelf life";
            dRow30[1] = "blnMemmorizeShelflife";
            dRow30[2] = CheckState.Checked;
            dtCheckBox.Rows.Add(dRow30);

            DataRow dRow31 = dtCheckBox.NewRow();
            dRow31[0] = "BatchMode - None";
            dRow31[1] = "blnBatchModeNone";
            dRow31[2] = CheckState.Checked;
            dtCheckBox.Rows.Add(dRow31);

            DataRow dRow32 = dtCheckBox.NewRow();
            dRow32[0] = "BatchMode - MNF";
            dRow32[1] = "blnBatchModeMNF";
            dRow32[2] = CheckState.Checked;
            dtCheckBox.Rows.Add(dRow32);

            DataRow dRow33 = dtCheckBox.NewRow();
            dRow33[0] = "BatchMode - AUTO";
            dRow33[1] = "blnBatchModeAUTO";
            dRow33[2] = CheckState.Checked;
            dtCheckBox.Rows.Add(dRow33);

            DataRow dRow34 = dtCheckBox.NewRow();
            dRow34[0] = "BatchMode - WM";
            dRow34[1] = "blnBatchModeWM";
            dRow34[2] = CheckState.Checked;
            dtCheckBox.Rows.Add(dRow34);

            this.chklstShowControl.DataSource = dtCheckBox;
            this.chklstShowControl.DisplayMember = "strDescription";
            this.chklstShowControl.ValueMember = "strKeyName";


            //foreach (int i in chklstShowControl.Items)
            //{
            //    chklstShowControl.SetItemCheckState(i, CheckState.Checked);
            //}

            //Default Check
            int RowIndex=0;
            if (AppSettings.NeedColor == false)
                RowIndex=RowIndex + 1;
            if (AppSettings.NeedSize == false)
                RowIndex= RowIndex + 1;
            if (AppSettings.NeedAgent == false)
                RowIndex = RowIndex + 1;
            if (AppSettings.NeedAgent == true)
                chklstShowControl.SetItemCheckState(5- RowIndex, CheckState.Checked);//Agent
            chklstShowControl.SetItemCheckState(6- RowIndex, CheckState.Checked);//Disc%
            chklstShowControl.SetItemCheckState(8- RowIndex, CheckState.Checked);//ROL
            chklstShowControl.SetItemCheckState(6 - RowIndex, CheckState.Checked);//Disc%
            chklstShowControl.SetItemCheckState(14 - RowIndex, CheckState.Checked);//Alter Unit

            chklstShowControl.SetItemCheckState(16 - RowIndex, CheckState.Checked);//Memmorize Category
            chklstShowControl.SetItemCheckState(17 - RowIndex, CheckState.Checked);//Manuf
            chklstShowControl.SetItemCheckState(18 - RowIndex, CheckState.Checked);//Unit
            chklstShowControl.SetItemCheckState(19 - RowIndex, CheckState.Checked);//Rack

            if (AppSettings.NeedColor == false)
                RowIndex = RowIndex + 1;
            if (AppSettings.NeedSize == false)
                RowIndex = RowIndex + 1;
            if (AppSettings.NeedBrand == false)
                RowIndex = RowIndex + 1;
            if (AppSettings.NeedBrand == true)
                chklstShowControl.SetItemCheckState(22 - RowIndex, CheckState.Checked);//Brand
            chklstShowControl.SetItemCheckState(23 - RowIndex, CheckState.Checked);//DiscGroup
            chklstShowControl.SetItemCheckState(24 - RowIndex, CheckState.Checked);//Depart
            //if (AppSettings.TaxEnabled == false)
            //    RowIndex = RowIndex + 1;
            chklstShowControl.SetItemCheckState(26 - RowIndex, CheckState.Checked);//SR Inclu
            chklstShowControl.SetItemCheckState(27 - RowIndex, CheckState.Checked);//PR Inclu
            chklstShowControl.SetItemCheckState(28 - RowIndex, CheckState.Checked);//Batch
            chklstShowControl.SetItemCheckState(29 - RowIndex, CheckState.Checked);//ShelfLife

        }
        //Description : Save Checkbox List for Show/Hide Controls to Json 
        private void SaveControlCheckboxList()
        {

            string strJson = "";
            int i = 0;

            string strAvailableBarcodeList = "";
            int IndexOfNoneBarcode = 0;

            clsJsonItemMasterChkbxListctrlInfo clschklistCtrlInfo = new clsJsonItemMasterChkbxListctrlInfo();
            List<clsJsonItemMasterChkbxListctrlInfo> lstchklistCtrlinfo = new List<clsJsonItemMasterChkbxListctrlInfo>();
            foreach (var item in chklstShowControl.Items)
            {
                var row = (item as DataRowView).Row;

                clschklistCtrlInfo = new clsJsonItemMasterChkbxListctrlInfo();
                clschklistCtrlInfo.strDescription = row["strDescription"].ToString().ToUpper();
                clschklistCtrlInfo.strKeyName = row["strKeyName"].ToString().ToUpper();

                if (i < chklstShowControl.Items.Count)
                {
                    clschklistCtrlInfo.bValue = Convert.ToBoolean(chklstShowControl.GetItemChecked(i).ToString());
                }
                if (clschklistCtrlInfo.strKeyName == "blnBatchModeNone".ToUpper() || 
                    clschklistCtrlInfo.strKeyName == "blnBatchModeMNF".ToUpper() ||
                    clschklistCtrlInfo.strKeyName == "blnBatchModeAUTO".ToUpper() ||
                    clschklistCtrlInfo.strKeyName == "blnBatchModeWM".ToUpper())
                {
                    if (clschklistCtrlInfo.bValue == true)
                        strAvailableBarcodeList = strAvailableBarcodeList + clschklistCtrlInfo.strKeyName;
                }
                if (clschklistCtrlInfo.strKeyName == "blnBatchModeNone".ToUpper())
                    IndexOfNoneBarcode = i;


                i++;
                
                lstchklistCtrlinfo.Add(clschklistCtrlInfo);
            }

            if (strAvailableBarcodeList == "")
            {
                chklstShowControl.SetSelected(IndexOfNoneBarcode, true);
                chklstShowControl.SetItemChecked(IndexOfNoneBarcode, true);
                MessageBox.Show("Atleast one barcode mode should be selected. Assigning barcode mode <None> by default.", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            strJson = JsonConvert.SerializeObject(lstchklistCtrlinfo);

            decimal IMParentID = 502;
            Comm.fnExecuteNonQuery("UPDATE tblVchType SET VchJson = '" + strJson + "' WHERE ParentID = " + IMParentID + "");
            Comm.MessageboxToasted("Item Master Controls", "Item Master Controls Saved Successfully");
        }
        //Description : Load Checkbox List for Show/Hide Controls
        private void LoadControlCheckboxList()
        {
            try
            {
                string strJson = "";
                decimal IMParentID = 502;
                strJson = Comm.fnGetData("SELECT ISNULL(VchJson,'') as VchJson FROM tblVchType WHERE ParentID = " + IMParentID + "").Tables[0].Rows[0][0].ToString();
                if (strJson != "")
                {
                    List<clsJsonItemMasterChkbxListctrlInfo> lstchklistCtrlinfo = JsonConvert.DeserializeObject<List<clsJsonItemMasterChkbxListctrlInfo>>(strJson);

                    dtCheckList = lstchklistCtrlinfo.ToDataTable();

                    int RowsCount = lstchklistCtrlinfo.Count;

                    //if (AppSettings.NeedColor == false)
                    //{
                    //    RowsCount = RowsCount - 1;//Show Color
                    //    RowsCount = RowsCount - 1;//Memmorize Color
                    //}
                    //if (AppSettings.NeedSize == false)
                    //{
                    //    RowsCount = RowsCount - 1;//Show Size
                    //    RowsCount = RowsCount - 1;//Memmorize Color
                    //}
                    //if (AppSettings.NeedAgent == false)
                    //{
                    //    RowsCount = RowsCount - 1;//Show Agent
                    //}
                    //if (AppSettings.NeedBrand == false)
                    //    RowsCount = RowsCount - 1;//Memmorize Brand
                    //if (AppSettings.TaxEnabled == false)
                    //{
                    //    RowsCount = RowsCount - 1;//Memmorize Hsncode and Tax
                    //    RowsCount = RowsCount - 1;//Memmorize SRate Inclusive
                    //    RowsCount = RowsCount - 1;//Memmorize PRate Inclusive
                    //}

                    if (dtCheckList.Rows.Count > 0)
                    {
                        for (int i = 0; i < RowsCount; i++)
                        {
                            foreach (var item in chklstShowControl.Items)
                            {
                                if (dtCheckList.Rows.Count > i)
                                {
                                    var row = (item as DataRowView).Row;
                                    if (dtCheckList.Rows[i][1].ToString().ToUpper() == row["strKeyName"].ToString().ToUpper())
                                    {
                                        chklstShowControl.SetItemChecked(i, Convert.ToBoolean(dtCheckList.Rows[i][2].ToString()));
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            //else
                //ShowControlCheckboxList();
        }
      
        //Description : Set Show /Hide
        private void SetShowHideValue()
        {
            //flpItemMasterRight.Visible = true;

            tlpRack.Visible = true;
            if (AppSettings.NeedColor == true)
                tlpColor.Visible = true;
            if (AppSettings.NeedSize == true)
                tlpSize.Visible = true;
            tlpDepart.Visible = true;
            tlpDesc.Visible = true;
            if (AppSettings.NeedAgent == true)
                tlpAgent.Visible = true;
            tlpDiscPer.Visible = true;
            tlpCoolie.Visible = true;
            tlpRol.Visible = true;
            tlpMoq.Visible = true;
            tlpMinRt.Visible = true;
            tlpMaxRt.Visible = true;
            chkSlabSysytem.Visible = true;
            lblshelflife.Visible = true;
            txtshelflife.Visible = true;
            grpUnitDetails.Visible = true;

            grpProductClass.Visible = true;
            cboProductClass.Visible = true;
            lblProdClass.Visible = true;

            //flpItemMasterRight.Visible

            if (AppSettings.NeedDiscGrouping == false)
                tlpDisc.Visible = false;
            else
                tlpDisc.Visible = true;

            int RowsCount = 22;
            if (AppSettings.NeedColor == false)
            {
                RowsCount = RowsCount - 1;//Show Color
                RowsCount = RowsCount - 1;//Memmorize Color
            }
            if (AppSettings.NeedSize == false)
            {
                RowsCount = RowsCount - 1;//Show Size
                RowsCount = RowsCount - 1;//Memmorize Color
            }
            if (AppSettings.NeedAgent == false)
            {
                RowsCount = RowsCount - 1;//Show Agent
            }
            if (AppSettings.NeedBrand == false)
                RowsCount = RowsCount - 1;//Memmorize Brand
            if (AppSettings.TaxEnabled == false)
            {
                RowsCount = RowsCount - 1;//Memmorize Hsncode and Tax
                RowsCount = RowsCount - 1;//Memmorize SRate Inclusive
                RowsCount = RowsCount - 1;//Memmorize PRate Inclusive
            }

            if (dtCheckList.Rows.Count > 0)
            {
                RowsCount = dtCheckList.Rows.Count;

                for (int i = 0; i < RowsCount; i++)
                {
                    if (dtCheckList.Rows[i]["strKeyName"].ToString().ToUpper() == "BLNSHOWRACK" && chklstShowControl.GetItemChecked(i) == false)
                    {
                        tlpRack.Visible = false;
                    }
                    else if (dtCheckList.Rows[i]["strKeyName"].ToString().ToUpper() == "BLNSHOWCOLOR" && chklstShowControl.GetItemChecked(i) == false)
                    {
                        tlpColor.Visible = false;
                    }
                    else if (dtCheckList.Rows[i]["strKeyName"].ToString().ToUpper() == "BLNSHOWSIZE" && chklstShowControl.GetItemChecked(i) == false)
                    {
                        tlpSize.Visible = false;
                    }
                    else if (dtCheckList.Rows[i]["strKeyName"].ToString().ToUpper() == "BLNSHOWDEPARTMENT" && chklstShowControl.GetItemChecked(i) == false)//blnBatchModeNone
                    {
                        tlpDepart.Visible = false;
                    }
                    else if (dtCheckList.Rows[i]["strKeyName"].ToString().ToUpper() == "BLNSHOWDESCRIPTION" && chklstShowControl.GetItemChecked(i) == false)
                    {
                        tlpDesc.Visible = false;
                    }
                    else if (dtCheckList.Rows[i]["strKeyName"].ToString().ToUpper() == "BLNSHOWAGENTCOMMISION" && chklstShowControl.GetItemChecked(i) == false)
                    {
                        tlpAgent.Visible = false;
                    }
                    else if (dtCheckList.Rows[i]["strKeyName"].ToString().ToUpper() == "BLNSHOWDISCPER" && chklstShowControl.GetItemChecked(i) == false)
                    {
                        tlpDiscPer.Visible = false;
                    }
                    else if (dtCheckList.Rows[i]["strKeyName"].ToString().ToUpper() == "BLNSHOWCOOLIEPERQTY" && chklstShowControl.GetItemChecked(i) == false)
                    {
                        tlpCoolie.Visible = false;
                    }
                    else if (dtCheckList.Rows[i]["strKeyName"].ToString().ToUpper() == "BLNSHOWROL" && chklstShowControl.GetItemChecked(i) == false)
                    {
                        tlpRol.Visible = false;
                    }
                    else if (dtCheckList.Rows[i]["strKeyName"].ToString().ToUpper() == "BLNSHOWMOQ" && chklstShowControl.GetItemChecked(i) == false)
                    {
                        tlpMoq.Visible = false;
                    }
                    else if (dtCheckList.Rows[i]["strKeyName"].ToString().ToUpper() == "BLNSHOWMINRATE" && chklstShowControl.GetItemChecked(i) == false)
                    {
                        tlpMinRt.Visible = false;
                    }
                    else if (dtCheckList.Rows[i]["strKeyName"].ToString().ToUpper() == "BLNSHOWMAXRATE" && chklstShowControl.GetItemChecked(i) == false)
                    {
                        tlpMaxRt.Visible = false;
                    }
                    else if (dtCheckList.Rows[i]["strKeyName"].ToString().ToUpper() == "BLNSHOWSLABSYSTEM" && chklstShowControl.GetItemChecked(i) == false)
                    {
                        chkSlabSysytem.Visible = false;
                    }
                    else if (dtCheckList.Rows[i]["strKeyName"].ToString().ToUpper() == "BLNSHOWSHELFLIFE" && chklstShowControl.GetItemChecked(i) == false)
                    {
                        lblshelflife.Visible = false;
                        txtshelflife.Visible = false;
                    }
                    else if (dtCheckList.Rows[i]["strKeyName"].ToString().ToUpper() == "BLNRATECHECK" && chklstShowControl.GetItemChecked(i) == false)
                    {
                        mBLNRATECHECK = false;
                    }
                    else if (dtCheckList.Rows[i]["strKeyName"].ToString().ToUpper() == "blnSRateInc".ToUpper() && chklstShowControl.GetItemChecked(i) == false)
                    {
                        mBLNSrateInc = false;
                    }
                    else if (dtCheckList.Rows[i]["strKeyName"].ToString().ToUpper() == "blnPRateInc".ToUpper() && chklstShowControl.GetItemChecked(i) == false)
                    {
                        mBLNPrateInc = false;
                    }
                    else if (dtCheckList.Rows[i]["strKeyName"].ToString().ToUpper() == "BLNSHOWALTERUNIT" && chklstShowControl.GetItemChecked(i) == false)
                    {
                        grpUnitDetails.Visible = false;
                    }
                    else if (dtCheckList.Rows[i]["strKeyName"].ToString().ToUpper() == "BLNSHOWPRODUCTCLASS" && chklstShowControl.GetItemChecked(i) == false)
                    {
                        grpProductClass.Visible = false;
                    }
                }

                if (mBLNSrateInc == true)
                    chkSRateIncl.Checked = true;
                else
                    chkSRateIncl.Checked = false;
                if (mBLNPrateInc == true)
                    chkPRateIncl.Checked = true;
                else
                    chkPRateIncl.Checked = false;

            }
        }

        //Description : Set Memmorize 
        private void SetMemmorizeValue()
        {
            int Memmorizeindex = 16;

            if (AppSettings.NeedColor==false)
                Memmorizeindex = Memmorizeindex - 1;
            if (AppSettings.NeedSize == false)
                Memmorizeindex = Memmorizeindex - 1;
             if (AppSettings.NeedAgent == false)
                Memmorizeindex = Memmorizeindex - 1;

            if (dtCheckList.Rows.Count > 0)
            {
                for (int i = Memmorizeindex; i < chklstShowControl.Items.Count; i++)
                {
                    if (dtCheckList.Rows.Count > i)
                    {
                        if (dtCheckList.Rows[i]["strKeyName"].ToString().ToUpper() == "BLNMEMMORIZECATEGORY" && chklstShowControl.GetItemChecked(i) == false)
                        {
                            this.txtCategoryList.TextChanged -= this.txtCategoryList_TextChanged;
                            txtCategoryList.Clear();
                            this.txtCategoryList.TextChanged += this.txtCategoryList_TextChanged;
                            txtCategoryList.Tag = "";
                            lblCategoryIds.Text = "";
                            GetFromCheckedList("");
                        }
                        else if (dtCheckList.Rows[i]["strKeyName"].ToString().ToUpper() == "BLNMEMMORIZEMANUFACTURER" && chklstShowControl.GetItemChecked(i) == false)
                        {
                            txtManufacturer.TextChanged -= this.txtManufacturer_TextChanged;
                            txtManufacturer.Clear();
                            txtManufacturer.TextChanged += this.txtManufacturer_TextChanged;
                            txtManufacturer.Tag = 1;
                        }
                        else if (dtCheckList.Rows[i]["strKeyName"].ToString().ToUpper() == "BLNMEMMORIZEUNIT" && chklstShowControl.GetItemChecked(i) == false)
                        {
                            cboUnit.SelectedIndex = 0;
                            cboUnit.Tag = 1;
                            cboUnit.SelectedValue = 1;

                        }
                        else if (dtCheckList.Rows[i]["strKeyName"].ToString().ToUpper() == "BLNMEMMORIZERACK" && chklstShowControl.GetItemChecked(i) == false)
                        {
                            this.txtRack.TextChanged -= this.txtRack_TextChanged;
                            txtRack.Text = "";
                            this.txtRack.TextChanged += this.txtRack_TextChanged;
                        }
                        else if (dtCheckList.Rows[i]["strKeyName"].ToString().ToUpper() == "BLNMEMMORIZECOLOR" && chklstShowControl.GetItemChecked(i) == false)
                        {
                            txtColor.Clear();
                            txtColor.Tag = 1;
                            GetFromCheckedListColor("1");
                        }
                        else if (dtCheckList.Rows[i]["strKeyName"].ToString().ToUpper() == "BLNMEMMORIZESIZE" && chklstShowControl.GetItemChecked(i) == false)
                        {
                            txtSize.Clear();
                            txtSize.Tag = 1;
                            GetFromCheckedListSize("1");
                        }
                        else if (dtCheckList.Rows[i]["strKeyName"].ToString().ToUpper() == "BLNMEMMORIZEBRAND" && chklstShowControl.GetItemChecked(i) == false)
                        {
                            cboBrand.SelectedIndex = 0;
                            cboBrand.Tag = 1;
                            cboBrand.SelectedValue = 1;
                        }
                        else if (dtCheckList.Rows[i]["strKeyName"].ToString().ToUpper() == "BLNMEMMORIZEDISCGROUP" && chklstShowControl.GetItemChecked(i) == false)
                        {
                            cboDiscGroup.Tag = 1;
                            cboDiscGroup.SelectedValue = 1;
                        }
                        else if (dtCheckList.Rows[i]["strKeyName"].ToString().ToUpper() == "BLNMEMMORIZEDEPARTMENT" && chklstShowControl.GetItemChecked(i) == false)
                        {
                            cboDepmnt.SelectedIndex = 0;
                            cboDepmnt.Text = "";
                            cboDepmnt.Tag = 1;
                            cboDepmnt.SelectedValue = 1;
                        }
                        else if (dtCheckList.Rows[i]["strKeyName"].ToString().ToUpper() == "BLNMEMMORIZEHSNCODE" && chklstShowControl.GetItemChecked(i) == false)
                        {
                            this.txtHSNCode.TextChanged -= this.txtHSNCode_TextChanged;
                            txtHSNCode.Clear();
                            this.txtHSNCode.TextChanged += this.txtHSNCode_TextChanged;
                            cboIGSTPerc.SelectedIndex = 0;
                            txtCGSTPerc.Clear();
                            txtSGSTPerc.Clear();
                            txtCessPerc.Clear();
                            chkSlabSysytem.Checked = false;
                        }
                        //else if (dtCheckList.Rows[i]["strKeyName"].ToString().ToUpper() == "BLNMEMMORIZESRATEINC" && chklstShowControl.GetItemChecked(i) == false)
                        //{
                        //    chkSRateIncl.Checked = false;
                        //}
                        //else if (dtCheckList.Rows[i]["strKeyName"].ToString().ToUpper() == "BLNMEMMORIZEPRATEINC" && chklstShowControl.GetItemChecked(i) == false)
                        //{
                        //    chkPRateIncl.Checked = false;
                        //}
                        else if (dtCheckList.Rows[i]["strKeyName"].ToString().ToUpper() == "BLNMEMMORIZEBATCHCODEMODE" && chklstShowControl.GetItemChecked(i) == false)
                        {
                            cboBMode.SelectedIndex = -1;
                        }
                        else if (dtCheckList.Rows[i]["strKeyName"].ToString().ToUpper() == "BLNMEMMORIZESHELFLIFE" && chklstShowControl.GetItemChecked(i) == false)
                        {
                            txtshelflife.Text = "0";
                        }
                    }
                }
            }
        }

        //Description : check PLU is Unique
        private void CheckingPLUNoisUnique()
        {
            if (txtPLUNo.Text.Length > 0)
            {
                string sQuery = "select PLUNO from tblItemMaster where PLUNO > 0 and PLUNO = " + txtPLUNo.Text + " And TenantID = '" + Global.gblTenantID + "'";
                DataTable dtPLU = Comm.fnGetData(sQuery).Tables[0];
                if (dtPLU.Rows.Count > 0)
                {
                    MessageBox.Show("This PLUNo " + txtPLUNo.Text + " is already Exist.Try another PLU Number", Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtPLUNo.Focus();
                    txtPLUNo.SelectAll();
                }
            }
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            btnSave.PerformClick();
        }

        private DataTable TaxInclusiveExclusive()
        {
            String SR1, SR2, SR3, SR4, SR5,MRP;

            DataTable dtRet = new DataTable();

            dtRet.Columns.Add("Rate", typeof(string));
            dtRet.Columns.Add("ExclRate", typeof(decimal));
            dtRet.Columns.Add("InclRate", typeof(decimal));

            if (string.IsNullOrEmpty(cboIGSTPerc.Text))
                cboIGSTPerc.Text="0";
            //PRate
            if (string.IsNullOrEmpty(txtPRate.Text))
            {
                DataRow row1 = dtRet.NewRow();
                row1["Rate"] = "PRate";
                row1["ExclRate"] = 0;
                row1["InclRate"] = 0;
                dtRet.Rows.Add(row1);
            }
            else
            {
                DataRow row1 = dtRet.NewRow();
                row1["Rate"] = "PRate";
                double PRt1 = (Convert.ToDouble(txtPRate.Text) + (Convert.ToDouble(txtPRate.Text) * Convert.ToDouble(cboIGSTPerc.Text)) / 100);
                row1["ExclRate"] = FormatValue(Convert.ToDouble(txtPRate.Text), true, "#0.00"); 
                row1["InclRate"] = FormatValue(PRt1, true, "#0.00");
                dtRet.Rows.Add(row1);
            }
            //MRP
            if (string.IsNullOrEmpty(AppSettings.MRPName.Trim()))
                MRP = "MRP";
            else
                MRP = AppSettings.MRPName;

            if (string.IsNullOrEmpty(txtMRP.Text))
            {
                DataRow row2 = dtRet.NewRow();
                row2["Rate"] = MRP;
                row2["ExclRate"] = 0;
                row2["InclRate"] = 0;
                dtRet.Rows.Add(row2);
            }
            else
            {
                DataRow row2 = dtRet.NewRow();
                row2["Rate"] = MRP;
                row2["ExclRate"] = FormatValue(Convert.ToDouble(txtMRP.Text), true, "#0.00");
                row2["InclRate"] = FormatValue(Convert.ToDouble(txtMRP.Text), true, "#0.00");
                dtRet.Rows.Add(row2);
            }
            //SR1
            if (AppSettings.IsActiveSRate1 == true)
            {
                if (string.IsNullOrEmpty(AppSettings.SRate1Name.Trim()))
                    SR1 = "SRate 1";
                else
                    SR1 = AppSettings.SRate1Name;

                if (string.IsNullOrEmpty(txtSRate1.Text))
                {
                    DataRow row2 = dtRet.NewRow();
                    row2["Rate"] = SR1;
                    row2["ExclRate"] = 0;
                    row2["InclRate"] = 0;
                    dtRet.Rows.Add(row2);
                }
                else
                {
                    DataRow row2 = dtRet.NewRow();
                    row2["Rate"] = SR1;
                    double SalRt1 = (Convert.ToDouble(txtSRate1.Text) / (100 + Convert.ToDouble(cboIGSTPerc.Text)) * 100);
                    row2["ExclRate"] = FormatValue(SalRt1, true, "#0.00"); 
                    row2["InclRate"] = FormatValue(Convert.ToDouble(txtSRate1.Text), true, "#0.00");
                    dtRet.Rows.Add(row2);
                }

            }
            //SR2
            if (AppSettings.IsActiveSRate2 == true)
            {
                if (string.IsNullOrEmpty(AppSettings.SRate2Name.Trim()))
                    SR2 = "SRate 2";
                else
                    SR2 = AppSettings.SRate2Name;

                if (string.IsNullOrEmpty(txtSRate2.Text))
                {
                    DataRow row3 = dtRet.NewRow();
                    row3["Rate"] = SR2;
                    row3["ExclRate"] = 0;
                    row3["InclRate"] = 0;
                    dtRet.Rows.Add(row3);
                }
                else
                {
                    DataRow row3 = dtRet.NewRow();
                    row3["Rate"] = SR2;
                    double SalRt2 = (Convert.ToDouble(txtSRate2.Text) / (100 + Convert.ToDouble(cboIGSTPerc.Text)) * 100);
                    row3["ExclRate"] = FormatValue(SalRt2, true, "#0.00");
                    row3["InclRate"] = FormatValue(Convert.ToDouble(txtSRate2.Text), true, "#0.00");
                    dtRet.Rows.Add(row3);
                }
            }
            //SR3
            if (AppSettings.IsActiveSRate3 == true)
            {
                if (string.IsNullOrEmpty(AppSettings.SRate3Name.Trim()))
                    SR3 = "SRate 3";
                else
                    SR3 = AppSettings.SRate3Name;

                if (string.IsNullOrEmpty(txtSRate3.Text))
                {
                    DataRow row4 = dtRet.NewRow();
                    row4["Rate"] = SR3;
                    row4["ExclRate"] = 0;
                    row4["InclRate"] = 0;
                    dtRet.Rows.Add(row4);
                }
                else
                {
                    DataRow row4 = dtRet.NewRow();
                    row4["Rate"] = SR3;
                    double SalRt3 = (Convert.ToDouble(txtSRate3.Text) / (100 + Convert.ToDouble(cboIGSTPerc.Text)) * 100);
                    row4["ExclRate"] = FormatValue(SalRt3, true, "#0.00");
                    row4["InclRate"] =  FormatValue(Convert.ToDouble(txtSRate3.Text), true, "#0.00");
                    dtRet.Rows.Add(row4);
                }
            }
            //SR4
            if (AppSettings.IsActiveSRate4 == true)
            {
                if (string.IsNullOrEmpty(AppSettings.SRate4Name.Trim()))
                    SR4 = "SRate 4";
                else
                    SR4 = AppSettings.SRate4Name;

                if (string.IsNullOrEmpty(txtSRate4.Text))
                {
                    DataRow row5 = dtRet.NewRow();
                    row5["Rate"] = SR4;
                    row5["ExclRate"] = 0;
                    row5["InclRate"] = 0;
                    dtRet.Rows.Add(row5);
                }
                else
                {
                    DataRow row5 = dtRet.NewRow();
                    row5["Rate"] = SR4;
                    double SalRt4 = (Convert.ToDouble(txtSRate4.Text) / (100 + Convert.ToDouble(cboIGSTPerc.Text)) * 100);
                    row5["ExclRate"] = FormatValue(SalRt4, true, "#0.00");
                    row5["InclRate"] = FormatValue(Convert.ToDouble(txtSRate4.Text), true, "#0.00");
                    dtRet.Rows.Add(row5);
                }
            }
            //SR5
            if (AppSettings.IsActiveSRate5 == true)
            {
                if (string.IsNullOrEmpty(AppSettings.SRate5Name.Trim()))
                    SR5 = "SRate 5";
                else
                    SR5 = AppSettings.SRate5Name;

                if (string.IsNullOrEmpty(txtSRate5.Text))
                {
                    DataRow row6 = dtRet.NewRow();
                    row6["Rate"] = SR5;
                    row6["ExclRate"] = 0;
                    row6["InclRate"] = 0;
                    dtRet.Rows.Add(row6);
                }
                else
                {
                    DataRow row6 = dtRet.NewRow();
                    row6["Rate"] = SR5;
                    double SalRt5 = (Convert.ToDouble(txtSRate5.Text) / (100 + Convert.ToDouble(cboIGSTPerc.Text)) * 100);
                    row6["ExclRate"] = FormatValue(SalRt5, true, "#0.00");
                    row6["InclRate"] = FormatValue(Convert.ToDouble(txtSRate5.Text), true, "#0.00");
                    dtRet.Rows.Add(row6);
                }
            }

            return dtRet;
        }

        private void txtColor_TextChanged(object sender, EventArgs e)
        {

        }

        private void lblHeading_Click(object sender, EventArgs e)
        {

        }

        private void lblHeading_MouseDown(object sender, MouseEventArgs e)
        {

        }

        private void LoadTesting(int i)
        {
            if (i > 0)
            {
                txtItemName.Text = "Life Boy Soap " + i.ToString();
                txtItemCode.Text = "Life Boy Soap " + i.ToString();
            }
            else
            {
                txtItemName.Text = "Life Boy Soap";
                txtItemCode.Text = "Life Boy Soap";
            }

            lblCategoryIds.Text = "1";
            txtCategoryList.Text = "DEFAULT";

            cboUnit.SelectedValue = 1; //Nos

            cboIGSTPerc.SelectedIndex = 1;

            txtCGSTPerc.Text = "6";
            txtSGSTPerc.Text = "6";

            txtPRate.Text = "100";
            txtMRP.Text = "200";

            txtSRate1.Text = "150";
            txtSRate2.Text = "160";
            txtSRate3.Text = "170";
            txtSRate4.Text = "180";
            txtSRate5.Text = "190";

            cboBMode.SelectedValue = 1; //MNF
            txtBarcode.Text = "102040" + i.ToString();
        }

        private void frmItemMaster_Validating(object sender, CancelEventArgs e)
        {

        }

        private void txtSRate1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.ActiveControl != null)
                    if (this.ActiveControl.Name == txtSRate1.Name)
                        txtPerc1.Text = CalculationInPriceDetails(Convert.ToDecimal(txtSRate1.Text), false).ToString("#0.00");
            }
            catch
            { }
            
            try
            {
                dgvTaxIncl.DataSource = TaxInclusiveExclusive();

            }
            catch
            {

            }
        }

        private void txtSRate2_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.ActiveControl != null)
                    if (this.ActiveControl.Name == txtSRate2.Name)
                        txtPerc2.Text = CalculationInPriceDetails(Convert.ToDecimal(txtSRate2.Text), false).ToString("#0.00");
            }
            catch
            { }

            try
            {
                dgvTaxIncl.DataSource = TaxInclusiveExclusive();

            }
            catch
            {

            }

        }

        private void txtSRate3_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.ActiveControl != null)
                    if (this.ActiveControl.Name == txtSRate3.Name)
                        txtPerc3.Text = CalculationInPriceDetails(Convert.ToDecimal(txtSRate3.Text), false).ToString("#0.00");
            }
            catch
            { }

            try
            {
                dgvTaxIncl.DataSource = TaxInclusiveExclusive();

            }
            catch
            {

            }

        }

        private void txtSRate4_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.ActiveControl != null)
                    if (this.ActiveControl.Name == txtSRate4.Name)
                        txtPerc4.Text = CalculationInPriceDetails(Convert.ToDecimal(txtSRate4.Text), false).ToString("#0.00");
            }
            catch
            { }

            try
            {
                dgvTaxIncl.DataSource = TaxInclusiveExclusive();

            }
            catch
            {

            }

        }

        private void txtSRate5_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.ActiveControl != null)
                    if (this.ActiveControl.Name == txtSRate5.Name)
                        txtPerc5.Text = CalculationInPriceDetails(Convert.ToDecimal(txtSRate5.Text), false).ToString("#0.00");
            }
            catch
            { }

            try
            {
                dgvTaxIncl.DataSource = TaxInclusiveExclusive();

            }
            catch
            {

            }

        }

        private void frmItemMaster_Activated(object sender, EventArgs e)
        {

        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message + "| ItemMaster Background work", "Item master");
            }
        }

        private void lblSR2_Click(object sender, EventArgs e)
        {

        }

        private void txtItemName_TextChanged(object sender, EventArgs e)
        {

        }

        private void SplitTaxPercentages()
        {
            try
            {
                //if (Convert.ToDecimal(Comm.Val(cboIGSTPerc.Text)) > 0)
                //{
                    txtCGSTPerc.Text = (Convert.ToDecimal(Comm.Val(cboIGSTPerc.Text)) / 2).ToString("#0.00");
                    txtSGSTPerc.Text = (Convert.ToDecimal(Comm.Val(cboIGSTPerc.Text)) / 2).ToString("#0.00");
                //}
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void cboIGSTPerc_SelectedIndexChanged(object sender, EventArgs e)
        {
            SplitTaxPercentages();
        }

        private void togglebtnActive_Click(object sender, EventArgs e)
        {

        }

        private void picBackground_LoadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            this.BackgroundImageLayout = ImageLayout.Stretch;
            this.BackgroundImage = (Bitmap)picBackground.Image.Clone();
            
            //this.BackgroundImage = ScaleByPercent(picBackground.Image, 120);

            //BackgroundImage = global::DigiposZen.Properties.Resources.WallpaperVioletGradient;
            //picBackground.Visible = true;
        }

        private void frmItemMaster_Shown(object sender, EventArgs e)
        {

        }

        private void txtPLUNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                e.Handled = Comm.CheckNumeric(sender, e);
            }
            catch
            {

            }
        }

        private void txtDefaultExpDays_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                e.Handled = Comm.CheckNumeric(sender, e);
            }
            catch
            {

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
